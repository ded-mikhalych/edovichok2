using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.Requests;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RecipeController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] CreateRecipeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length < 3)
            {
                return BadRequest(new { success = false, message = "Название должно содержать минимум 3 символа." });
            }

            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 10)
            {
                return BadRequest(new { success = false, message = "Описание должно содержать минимум 10 символов." });
            }

            if (request.Difficulty < 1 || request.Difficulty > 3)
            {
                return BadRequest(new { success = false, message = "Выберите корректную сложность." });
            }

            var ingredients = request.Ingredients
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => i.Trim())
                .ToList();

            var steps = request.Steps
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (ingredients.Count == 0)
            {
                return BadRequest(new { success = false, message = "Добавьте хотя бы один ингредиент." });
            }

            if (steps.Count == 0)
            {
                return BadRequest(new { success = false, message = "Добавьте хотя бы один шаг." });
            }

            await SyncPrimaryKeySequencesAsync();

            var categoryId = ResolveCategoryId(request.Cuisine);
            var slug = await GenerateUniqueSlugAsync(request.Title);
            var imageFolder = Path.Combine(_environment.WebRootPath, "images", "user");
            Directory.CreateDirectory(imageFolder);

            var mainImageFileName = request.MainImage != null
                ? await SaveImageAsync(request.MainImage, imageFolder, slug + "-main")
                : "salads.png";

            var recipe = new Recipe
            {
                Name = request.Title.Trim(),
                Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim(),
                Slug = slug,
                Description = request.Description.Trim(),
                Cuisine = string.IsNullOrWhiteSpace(request.Cuisine) ? null : request.Cuisine.Trim(),
                Difficulty = request.Difficulty,
                ImageFileName = mainImageFileName,
                IsFavorite = false,
                CookingTime = request.CookingTime > 0 ? request.CookingTime : 30,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                RatingSum = 0,
                RatingCount = 0,
                Ingredients = ingredients.Select((text, idx) => new RecipeIngredient
                {
                    DisplayText = text,
                    SortOrder = idx + 1
                }).ToList(),
                Steps = steps.Select((text, idx) => new RecipeStep
                {
                    StepNumber = idx + 1,
                    Description = text,
                    ImagePath = string.Empty
                }).ToList()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Рецепт успешно сохранён.",
                data = new
                {
                    recipe.Id,
                    recipe.Slug
                }
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query = "",
            [FromQuery] string[]? categories = null,
            [FromQuery] string[]? ingredients = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 4)
        {
            try
            {
                var recipesQuery = _context.Recipes.Include(r => r.Category).AsQueryable();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.ToLower().Trim();
                    recipesQuery = recipesQuery.Where(r =>
                        (r.Name ?? string.Empty).ToLower().Contains(query) ||
                        (r.Description ?? string.Empty).ToLower().Contains(query));
                }

                if (categories != null && categories.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        r.Category != null &&
                        r.Category.Name != null &&
                        categories.Contains(r.Category.Name));
                }

                if (ingredients != null && ingredients.Length > 0)
                {
                    foreach (var ingredient in ingredients.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()))
                    {
                        var localIngredient = ingredient;
                        recipesQuery = recipesQuery.Where(r =>
                            r.Ingredients.Any(i => EF.Functions.ILike(i.DisplayText, $"%{localIngredient}%")));
                    }
                }

                var totalCount = await recipesQuery.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var recipes = await recipesQuery
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Slug,
                        r.Description,
                        r.Difficulty,
                        r.ImageFileName,
                        r.CookingTime,
                        Category = r.Category != null ? r.Category.DisplayName : null
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = recipes,
                    count = totalCount,
                    page,
                    pageSize,
                    totalPages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id:int}/preview")]
        public async Task<IActionResult> GetPreview(int id)
        {
            try
            {
                var recipe = await _context.Recipes
                    .Include(r => r.Category)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (recipe == null)
                {
                    return NotFound(new { success = false, message = "Рецепт не найден." });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        recipe.Id,
                        Name = recipe.Name ?? string.Empty,
                        Category = recipe.Category?.DisplayName ?? "Не указан",
                        recipe.CookingTime,
                        Description = recipe.Description ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string query = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Ok(new { success = true, data = new List<object>() });
                }

                query = query.ToLower().Trim();

                var suggestions = await _context.Recipes
                    .Where(r =>
                        (r.Name ?? string.Empty).ToLower().Contains(query) ||
                        (r.Description ?? string.Empty).ToLower().Contains(query))
                    .OrderBy(r => r.Name)
                    .Take(10)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        type = "recipe"
                    })
                    .ToListAsync();

                var categorySuggestions = await _context.Categories
                    .Where(c => (c.Name ?? string.Empty).ToLower().Contains(query) ||
                                (c.DisplayName ?? string.Empty).ToLower().Contains(query))
                    .OrderBy(c => c.DisplayName)
                    .Take(5)
                    .Select(c => new
                    {
                        c.Id,
                        Name = c.DisplayName,
                        type = "category"
                    })
                    .ToListAsync();

                var ingredientSuggestions = await _context.RecipeIngredients
                    .Select(i => i.DisplayText)
                    .AsNoTracking()
                    .ToListAsync();

                var normalizedIngredientSuggestions = ingredientSuggestions
                    .Select(GetPrimaryIngredientFilter)
                    .Where(i => !string.IsNullOrWhiteSpace(i) && i.ToLower().Contains(query))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(i => i)
                    .Take(5)
                    .Select(i => new
                    {
                        Name = i,
                        type = "ingredient"
                    })
                    .Cast<object>()
                    .ToList();

                var allSuggestions = suggestions.Cast<object>()
                    .Concat(categorySuggestions.Cast<object>())
                    .Concat(normalizedIngredientSuggestions)
                    .ToList();

                return Ok(new { success = true, data = allSuggestions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new { c.Id, c.Name, c.DisplayName })
                    .OrderBy(c => c.DisplayName)
                    .ToListAsync();

                var ingredients = await _context.RecipeIngredients
                    .Select(i => i.DisplayText)
                    .AsNoTracking()
                    .ToListAsync();

                var ingredientFilters = ingredients
                    .Select(GetPrimaryIngredientFilter)
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(i => i)
                    .Select(i => new { Name = i })
                    .ToList();

                return Ok(new
                {
                    success = true,
                    categories,
                    ingredients = ingredientFilters
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task<string> GenerateUniqueSlugAsync(string title)
        {
            var baseSlug = ToSlug(title);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                baseSlug = "recipe";
            }

            var slug = baseSlug;
            var index = 1;
            while (await _context.Recipes.AnyAsync(r => r.Slug == slug))
            {
                slug = $"{baseSlug}-{index}";
                index++;
            }

            return slug;
        }

        private static string ToSlug(string text)
        {
            var normalized = text.Trim().ToLowerInvariant();
            var chars = normalized.Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray();
            var slug = new string(chars);

            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            return slug.Trim('-');
        }

        private static int ResolveCategoryId(string? cuisine)
        {
            if (string.IsNullOrWhiteSpace(cuisine))
                return 2;

            var c = cuisine.ToLowerInvariant();
            if (c == "first-course")
                return 1;

            if (c == "second-course")
                return 2;

            if (c == "pastry")
                return 3;

            if (c == "drinks")
                return 4;

            if (c.Contains("перв") || c.Contains("суп") || c.Contains("бульон"))
                return 1;

            if (c.Contains("выпеч") || c.Contains("пирог") || c.Contains("десерт"))
                return 3;

            if (c.Contains("напит") || c.Contains("чай") || c.Contains("кофе") || c.Contains("компот") || c.Contains("лимонад"))
                return 4;

            if (c.Contains("завт"))
                return 2;

            return 2;
        }

        private static string GetPrimaryIngredientFilter(string displayText)
        {
            if (string.IsNullOrWhiteSpace(displayText))
                return string.Empty;

            var normalized = displayText.ToLowerInvariant();

            return normalized switch
            {
                var s when s.Contains("кур") => "Курица",
                var s when s.Contains("картоф") => "Картофель",
                var s when s.Contains("тыкв") => "Тыква",
                var s when s.Contains("творог") => "Творог",
                var s when s.Contains("варень") => "Варенье",
                var s when s.Contains("рис") => "Рис",
                var s when s.Contains("помидор") || s.Contains("томат") => "Томаты",
                var s when s.Contains("ягод") => "Ягоды",
                var s when s.Contains("лимон") => "Лимон",
                var s when s.Contains("фет") => "Фета",
                _ => string.Empty
            };
        }

        private static async Task<string> SaveImageAsync(IFormFile file, string folderPath, string filePrefix)
        {
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            var safeExt = extension.ToLowerInvariant();
            if (safeExt != ".jpg" && safeExt != ".jpeg" && safeExt != ".png" && safeExt != ".webp")
            {
                safeExt = ".jpg";
            }

            var fileName = $"{filePrefix}-{Guid.NewGuid():N}{safeExt}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"user/{fileName}";
        }

        private async Task SyncPrimaryKeySequencesAsync()
        {
            await _context.Database.ExecuteSqlRawAsync(@"
SELECT setval(pg_get_serial_sequence('""Recipes""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Recipes"";
SELECT setval(pg_get_serial_sequence('""RecipeIngredients""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeIngredients"";
SELECT setval(pg_get_serial_sequence('""RecipeSteps""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeSteps"";
");
        }
    }
}
