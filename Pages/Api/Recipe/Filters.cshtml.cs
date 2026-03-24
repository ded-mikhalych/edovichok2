using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

public class FiltersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public FiltersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
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
                .Select(RecipeApiHelpers.GetPrimaryIngredientFilter)
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(i => i)
                .Select(i => new { Name = i })
                .ToList();

            return new JsonResult(new
            {
                success = true,
                categories,
                ingredients = ingredientFilters
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
