using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Pages;

public class IndexModel : PageModel
{
    private readonly SiteContentService _contentService;

    public IndexModel(SiteContentService contentService)
    {
        _contentService = contentService;
    }

    public HomeViewModel Home { get; private set; } = new();
    public int RecipeCount { get; private set; }
    public int ArticleCount { get; private set; }
    public int CategoryCount { get; private set; }

    public async Task OnGetAsync()
    {
        Home = await _contentService.GetHomeViewModelAsync();
        var metrics = await _contentService.GetMetricsAsync();
        RecipeCount = metrics.recipeCount;
        ArticleCount = metrics.articleCount;
        CategoryCount = metrics.categoryCount;
    }
}
