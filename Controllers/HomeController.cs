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
                .Select(c => new CategoryWithCount
                {
                    Id = c.Id,
                    Name = c.Name,
                    DisplayName = c.DisplayName,
                    RecipeCount = c.Recipes.Count
                })
                .OrderByDescending(c => c.RecipeCount)
                .Take(4)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestNews = latestNews,
                NewestRecipes = newestRecipes,
                PopularCategories = popularCategories
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

            return View(favoriteRecipes);
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

        public async Task<IActionResult> Article3()
        {
            var article = await _context.News
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync(n => n.Title == "Подсластители под угрозой");

            if (article == null)
            {
                return RedirectToAction(nameof(InDevelopment));
            }

            return View(article);
        }

        public async Task<IActionResult> Soups()
        {
            var soups = await _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.Category != null && r.Category.Name == "Russian")
                .OrderBy(r => r.Name)
                .ToListAsync();

            return View(soups);
        }

        [HttpGet("home/recipe/{slug}")]
        public async Task<IActionResult> Recipe(string slug)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Slug == slug);

            if (recipe == null)
            {
                return RedirectToAction(nameof(InDevelopment));
            }

            var ingredients = (recipe.IngredientsText ?? string.Empty)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var stepLines = (recipe.StepsText ?? string.Empty)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var imageFolder = string.IsNullOrWhiteSpace(recipe.StepImagesFolder)
                ? recipe.Slug
                : recipe.StepImagesFolder;

            var steps = stepLines
                .Select((text, index) => new RecipeStepViewModel
                {
                    Number = index + 1,
                    Text = text,
                    ImagePath = $"/images/{imageFolder}/step{index + 1}.jpg"
                })
                .ToList();

            var model = new RecipePageViewModel
            {
                Recipe = recipe,
                Ingredients = ingredients,
                Steps = steps
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
    }
}
