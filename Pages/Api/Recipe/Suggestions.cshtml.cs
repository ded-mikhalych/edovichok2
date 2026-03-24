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
                .Where(r => (r.Name ?? string.Empty).ToLower().Contains(query))
                .OrderBy(r => r.Name)
                .Take(10)
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return new JsonResult(new { success = true, data = suggestions });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }
}
