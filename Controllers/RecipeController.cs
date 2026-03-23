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
                return BadRequest(new { success = false, message = "Название должно содержать минимум 3 символа" });
            }

            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 10)
            {
                return BadRequest(new { success = false, message = "Описание должно содержать минимум 10 символов" });
            }

            if (request.Difficulty < 1 || request.Difficulty > 3)
            {
                return BadRequest(new { success = false, message = "Неверная сложность" });
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
                return BadRequest(new { success = false, message = "Добавьте хотя бы один ингредиент" });
            }

            if (steps.Count == 0)
            {
                return BadRequest(new { success = false, message = "Добавьте хотя бы один шаг" });
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
                Ingredients = ingredients
                    .Select((text, idx) => new RecipeIngredient
                    {
                        DisplayText = text,
                        SortOrder = idx + 1
                    })
                    .ToList(),
                Steps = new List<RecipeStep>()
            };

            for (var i = 0; i < steps.Count; i++)
            {
                var stepImagePath = "salads.png";
                if (i < request.StepImages.Count && request.StepImages[i] != null && request.StepImages[i].Length > 0)
                {
                    stepImagePath = await SaveImageAsync(request.StepImages[i], imageFolder, $"{slug}-step-{i + 1}");
                }

                recipe.Steps.Add(new RecipeStep
                {
                    StepNumber = i + 1,
                    Description = steps[i],
                    ImagePath = stepImagePath
                });
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Рецепт успешно сохранен",
                data = new
                {
                    recipe.Id,
                    recipe.Slug
                }
            });
        }

        /// <summary>
        /// Get all recipes with optional search, filters and pagination
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query = "",
            [FromQuery] string[]? categories = null,
            [FromQuery] int[]? difficulties = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 4)
        {
            try
            {
                var recipesQuery = _context.Recipes.Include(r => r.Category).AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.ToLower().Trim();
                    recipesQuery = recipesQuery.Where(r =>
                        (r.Name ?? string.Empty).ToLower().Contains(query) ||
                        (r.Description ?? string.Empty).ToLower().Contains(query));
                }

                // Category filter
                if (categories != null && categories.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        r.Category != null &&
                        r.Category.Name != null &&
                        categories.Contains(r.Category.Name));
                }

                // Difficulty filter
                if (difficulties != null && difficulties.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        difficulties.Contains(r.Difficulty));
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
                        Category = r.Category != null ? r.Category.DisplayName : null,
                        r.RatingSum,
                        r.RatingCount,
                        AverageRating = r.RatingCount > 0 ? (double)r.RatingSum / r.RatingCount : 0.0
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = recipes,
                    count = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = totalPages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    
        /// <summary>
        /// Get search suggestions (autocomplete)
        /// </summary>
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string query = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query) || query.Length < 1)
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

                // Also add category suggestions
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

                var allSuggestions = suggestions.Cast<object>().Concat(categorySuggestions.Cast<object>()).ToList();

                return Ok(new { success = true, data = allSuggestions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all available categories and difficulties for filter UI
        /// </summary>
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new { c.Id, c.Name, c.DisplayName })
                    .OrderBy(c => c.DisplayName)
                    .ToListAsync();

                var difficulties = new[]
                {
                    new { Id = 1, Name = "Easy", DisplayName = "Лёгкое" },
                    new { Id = 2, Name = "Medium", DisplayName = "Среднее" },
                    new { Id = 3, Name = "Hard", DisplayName = "Сложное" }
                };

                return Ok(new
                {
                    success = true,
                    categories = categories,
                    difficulties = difficulties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/rate")]
        public async Task<IActionResult> RateRecipe(int id, [FromBody] RateRequest request)
        {
            try
            {
                if (request.Rating < 1 || request.Rating > 5)
                    return BadRequest(new { success = false, message = "Оценка должна быть от 1 до 5" });

                var recipe = await _context.Recipes.FindAsync(id);
                if (recipe == null)
                    return NotFound(new { success = false, message = "Рецепт не найден" });

                recipe.RatingSum += request.Rating;
                recipe.RatingCount++;
                await _context.SaveChangesAsync();

                double averageRating = (double)recipe.RatingSum / recipe.RatingCount;
                return Ok(new
                {
                    success = true,
                    averageRating = Math.Round(averageRating, 1),
                    ratingCount = recipe.RatingCount
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
            var chars = normalized
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
                .ToArray();
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
            {
                return 2;
            }

            var c = cuisine.ToLowerInvariant();
            if (c.Contains("рус") || c.Contains("slav") || c.Contains("борщ"))
            {
                return 1;
            }

            if (c.Contains("ази") || c.Contains("asia") || c.Contains("thai") || c.Contains("китай") || c.Contains("япон"))
            {
                return 3;
            }

            return 2;
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
