using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Api.Recipe;

[IgnoreAntiforgeryToken]
public class TrackHistoryModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TrackHistoryModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var clientKey = RecipeApiHelpers.GetClientKey(Request);
            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return new JsonResult(new { success = false, message = "Missing client key." }) { StatusCode = 400 };
            }

            var recipe = await _context.Recipes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return new JsonResult(new { success = false, message = "Recipe not found." }) { StatusCode = 404 };
            }

            var existingHistory = await _context.RecipeViewHistories
                .FirstOrDefaultAsync(h => h.ClientKey == clientKey && h.RecipeId == id);

            if (existingHistory == null)
            {
                _context.RecipeViewHistories.Add(new RecipeViewHistory
                {
                    ClientKey = clientKey,
                    RecipeId = id,
                    ViewedAt = DateTime.UtcNow
                });
            }
            else
            {
                existingHistory.ViewedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
