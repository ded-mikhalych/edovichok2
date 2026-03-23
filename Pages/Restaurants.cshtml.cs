using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Pages;

public class RestaurantsModel : PageModel
{
    private readonly SiteContentService _contentService;

    public RestaurantsModel(SiteContentService contentService)
    {
        _contentService = contentService;
    }

    public IReadOnlyList<NewsCardViewModel> Reviews { get; private set; } = Array.Empty<NewsCardViewModel>();

    public async Task OnGetAsync()
    {
        Reviews = await _contentService.GetRestaurantReviewsAsync();
    }
}
