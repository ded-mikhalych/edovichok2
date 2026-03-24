using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

public class PreviewModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public PreviewModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return new JsonResult(new { success = false, message = "Р РµС†РµРїС‚ РЅРµ РЅР°Р№РґРµРЅ." }) { StatusCode = 404 };
            }

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    recipe.Id,
                    recipe.Slug,
                    Name = recipe.Name ?? string.Empty,
                    Category = recipe.Category?.DisplayName ?? "РќРµ СѓРєР°Р·Р°РЅ",
                    recipe.CookingTime,
                    Description = recipe.Description ?? string.Empty,
                    Ingredients = await _context.RecipeIngredients
                        .Where(i => i.RecipeId == recipe.Id)
                        .OrderBy(i => i.SortOrder)
                        .Take(5)
                        .Select(i => i.DisplayText)
                        .ToListAsync()
                }
            });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
