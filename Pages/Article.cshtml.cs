using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Pages;

public class ArticleModel : PageModel
{
    private readonly SiteContentService _contentService;

    public ArticleModel(SiteContentService contentService)
    {
        _contentService = contentService;
    }

    public ArticleViewModel Article { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var model = await _contentService.GetArticleAsync(id);
        if (model == null)
        {
            return RedirectToPage("/InDevelopment");
        }

        Article = model;
        return Page();
    }
}
