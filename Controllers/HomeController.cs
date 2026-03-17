using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestNews = await _context.News
                .OrderByDescending(n => n.CreatedAt)
                .Take(4)
                .ToListAsync();

            var newestRecipes = await _context.Recipes
                .Include(r => r.Category)
                .OrderByDescending(r => r.CreatedAt)
                .Take(4)
                .ToListAsync();

            var popularCategories = await _context.Categories
                .Select(c => new
                {
                    c.Name,
                    c.DisplayName,
                    RecipeCount = c.Recipes.Count
                })
                .OrderByDescending(c => c.RecipeCount)
                .Take(4)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestNews = latestNews.Select(n => new NewsCardViewModel
                {
                    ArticleId = n.Id,
                    Title = n.Title ?? string.Empty,
                    Summary = n.Summary ?? string.Empty,
                    ImageSrc = ResolveImagePath(n.ImageFileName),
                    ActionName = nameof(Article)
                }).ToList(),
                NewestRecipes = newestRecipes.Select(r => new RecipeCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty,
                    Slug = r.Slug,
                    ImageSrc = ResolveImagePath(r.ImageFileName),
                    ActionName = string.IsNullOrWhiteSpace(r.Slug) ? nameof(Catalog) : nameof(Recipe)
                }).ToList(),
                PopularCategories = popularCategories.Select(c => new CategoryCardViewModel
                {
                    DisplayName = c.DisplayName ?? string.Empty,
                    ActionName = c.Name == "Russian" ? nameof(Soups) : nameof(InDevelopment),
                    ImageFileName = c.Name == "Russian" ? "soups.png" : "salads.png",
                    RecipeCount = c.RecipeCount
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            return View();
        }

        public async Task<IActionResult> Favorites()
        {
            var favoriteRecipes = await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.IsFavorite)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var model = favoriteRecipes.Select(r => new RecipeLinkViewModel
            {
                Name = r.Name ?? string.Empty,
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? nameof(InDevelopment) : nameof(Recipe),
                Slug = r.Slug
            }).ToList();

            return View(model);
        }

        public IActionResult AddRecipe()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult InDevelopment()
        {
            return View();
        }

        public async Task<IActionResult> Article(int id)
        {
            var article = await _context.News.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            var model = new ArticleViewModel
            {
                Title = article.Title ?? string.Empty,
                ImageSrc = ResolveImagePath(article.ImageFileName),
                Summary = article.Summary ?? string.Empty,
                ContentHtml = article.ContentHtml
            };

            return View(model);
        }

        // Backward-compatibility redirect
        public async Task<IActionResult> Article3()
        {
            var article = await _context.News
                .FirstOrDefaultAsync(n => n.Title == "С солью, но снижает давление: найден «суперфуд», который можно приготовить дома");

            if (article == null)
            {
                return RedirectToAction(nameof(InDevelopment));
            }

            return RedirectToAction(nameof(Article), new { id = article.Id });
        }

        public async Task<IActionResult> Soups()
        {
            var soups = await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.Category != null && r.Category.Name == "Russian")
                .OrderBy(r => r.Name)
                .ToListAsync();

            var model = soups.Select(r => new RecipeLinkViewModel
            {
                Name = r.Name ?? string.Empty,
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? nameof(InDevelopment) : nameof(Recipe),
                Slug = r.Slug
            }).ToList();

            return View(model);
        }

        [HttpGet("home/recipe/{slug}")]
        public async Task<IActionResult> Recipe(string slug)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefaultAsync(r => r.Slug == slug);

            if (recipe == null)
            {
                return RedirectToAction(nameof(InDevelopment));
            }

            var model = new RecipePageViewModel
            {
                Name = recipe.Name ?? string.Empty,
                AuthorText = string.IsNullOrWhiteSpace(recipe.Author) ? "Не указан" : recipe.Author,
                Description = recipe.Description ?? string.Empty,
                MainImageSrc = ResolveImagePath(recipe.ImageFileName),
                DifficultyText = GetDifficultyText(recipe.Difficulty),
                CuisineText = string.IsNullOrWhiteSpace(recipe.Cuisine)
                    ? recipe.Category?.DisplayName ?? "Не указано"
                    : recipe.Cuisine,
                RatingText = recipe.RatingCount > 0
                    ? ((double)recipe.RatingSum / recipe.RatingCount).ToString("0.0")
                    : "—",
                Ingredients = recipe.Ingredients
                    .OrderBy(i => i.SortOrder)
                    .Select(i => i.DisplayText)
                    .ToList(),
                Steps = recipe.Steps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new RecipeStepViewModel
                    {
                        StepNumber = s.StepNumber,
                        Description = s.Description,
                        ImageSrc = ResolveImagePath(s.ImagePath)
                    })
                    .ToList()
            };

            return View(model);
        }

        public IActionResult Kharcho()
        {
            return RedirectToAction(nameof(Recipe), new { slug = "kharcho" });
        }

        public IActionResult Mushrooms()
        {
            return RedirectToAction(nameof(Recipe), new { slug = "mushrooms" });
        }

        public IActionResult Olivie()
        {
            return RedirectToAction(nameof(Recipe), new { slug = "olivie" });
        }

        public IActionResult Borsch()
        {
            return RedirectToAction(nameof(Recipe), new { slug = "borsch" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string ResolveImagePath(string? imageFileName)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                return "/images/placeholder.png";
            }

            return imageFileName.StartsWith("https://")
                ? imageFileName
                : $"/images/{imageFileName}";
        }

        private static string GetDifficultyText(int difficulty)
        {
            return difficulty switch
            {
                1 => "Легко",
                2 => "Средне",
                3 => "Сложно",
                _ => "Не указано"
            };
        }
    }
}
