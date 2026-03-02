using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all recipes with optional search and filters
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query = "",
            [FromQuery] string[]? categories = null,
            [FromQuery] int[]? difficulties = null)
        {
            try
            {
                var recipesQuery = _context.Recipes.Include(r => r.Category).AsQueryable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(query))
                {
                    query = query.ToLower().Trim();
                    recipesQuery = recipesQuery.Where(r =>
                        r.Name.ToLower().Contains(query) ||
                        r.Description.ToLower().Contains(query));
                }

                // Category filter
                if (categories != null && categories.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        categories.Contains(r.Category.Name));
                }

                // Difficulty filter
                if (difficulties != null && difficulties.Length > 0)
                {
                    recipesQuery = recipesQuery.Where(r =>
                        difficulties.Contains(r.Difficulty));
                }

                var recipes = await recipesQuery
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        r.Description,
                        r.Difficulty,
                        r.ImageFileName,
                        r.CookingTime,
                        Category = r.Category.DisplayName
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = recipes, count = recipes.Count });
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
                        r.Name.ToLower().Contains(query) ||
                        r.Description.ToLower().Contains(query))
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
                    .Where(c => c.Name.ToLower().Contains(query) ||
                               c.DisplayName.ToLower().Contains(query))
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
    }
}
