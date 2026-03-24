using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

public class SearchModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SearchModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(string query = "", string[]? categories = null, string[]? ingredients = null, int page = 1, int pageSize = 4)
    {
        try
        {
            var recipesQuery = _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Ingredients)
                .AsNoTracking()
                .AsQueryable();

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

            var recipes = await recipesQuery
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            if (ingredients != null && ingredients.Length > 0)
            {
                foreach (var ingredient in ingredients.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()))
                {
                    recipes = recipes
                        .Where(r => RecipeApiHelpers.MatchesIngredientFilter(
                            r.Ingredients.Select(i => i.DisplayText),
                            ingredient))
                        .ToList();
                }
            }

            var totalCount = recipes.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedRecipes = recipes
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
                .ToList();

            return new JsonResult(new
            {
                success = true,
                data = pagedRecipes,
                count = totalCount,
                page,
                pageSize,
                totalPages
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
