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

        public IActionResult Favorites()
        {
            return View();
        }

        public IActionResult AddRecipe()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
