using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Pages;

public class RecipeModel : PageModel
{
    private readonly SiteContentService _contentService;

    public RecipeModel(SiteContentService contentService)
    {
        _contentService = contentService;
    }

    public RecipePageViewModel Recipe { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var model = await _contentService.GetRecipeAsync(slug);
        if (model == null)
        {
            return RedirectToPage("/InDevelopment");
        }

        Recipe = model;
        return Page();
    }
}
