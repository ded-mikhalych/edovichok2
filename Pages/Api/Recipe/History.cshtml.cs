using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

[IgnoreAntiforgeryToken]
public class HistoryModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public HistoryModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int limit = 6)
    {
        try
        {
            var clientKey = RecipeApiHelpers.GetClientKey(Request);
            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return new JsonResult(new { success = false, message = "Missing client key." }) { StatusCode = 400 };
            }

            var normalizedLimit = Math.Clamp(limit, 1, 12);
            var history = await _context.RecipeViewHistories
                .Where(h => h.ClientKey == clientKey)
                .OrderByDescending(h => h.ViewedAt)
                .Include(h => h.Recipe)
                .ThenInclude(r => r!.Category)
                .Take(normalizedLimit)
                .Select(h => new
                {
                    h.RecipeId,
                    Name = h.Recipe!.Name ?? string.Empty,
                    h.Recipe.Slug,
                    h.Recipe.ImageFileName,
                    Category = h.Recipe.Category != null ? h.Recipe.Category.DisplayName : null,
                    h.ViewedAt
                })
                .ToListAsync();

            return new JsonResult(new { success = true, data = history });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    public async Task<IActionResult> OnDeleteAsync()
    {
        try
        {
            var clientKey = RecipeApiHelpers.GetClientKey(Request);
            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return new JsonResult(new { success = false, message = "Missing client key." }) { StatusCode = 400 };
            }

            var history = await _context.RecipeViewHistories
                .Where(h => h.ClientKey == clientKey)
                .ToListAsync();

            if (history.Count > 0)
            {
                _context.RecipeViewHistories.RemoveRange(history);
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { success = true });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
