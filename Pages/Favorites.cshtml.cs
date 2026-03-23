using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Pages;

public class FavoritesModel : PageModel
{
    private readonly SiteContentService _contentService;

    public FavoritesModel(SiteContentService contentService)
    {
        _contentService = contentService;
    }

    public IReadOnlyList<RecipeLinkViewModel> Recipes { get; private set; } = Array.Empty<RecipeLinkViewModel>();

    public async Task OnGetAsync()
    {
        Recipes = await _contentService.GetFavoritesAsync();
    }
}
