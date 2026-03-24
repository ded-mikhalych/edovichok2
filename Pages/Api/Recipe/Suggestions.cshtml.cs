using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

public class SuggestionsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SuggestionsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(string query = "")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new JsonResult(new { success = true, data = new List<object>() });
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
                .Select(RecipeApiHelpers.GetPrimaryIngredientFilter)
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

            return new JsonResult(new { success = true, data = allSuggestions });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
