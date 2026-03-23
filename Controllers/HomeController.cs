using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<IActionResult> Index()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Index"));
    }

    public IActionResult Privacy()
    {
        return RedirectToPage("/About");
    }

    public IActionResult Catalog()
    {
        return RedirectToPage("/Catalog");
    }

    public Task<IActionResult> Restaurants()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Restaurants"));
    }

    public IActionResult AddRecipe()
    {
        return RedirectToPage("/AddRecipe");
    }

    public IActionResult About()
    {
        return RedirectToPage("/About");
    }

    public IActionResult InDevelopment()
    {
        return RedirectToPage("/InDevelopment");
    }

    public Task<IActionResult> Article(int id)
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Article", new { id }));
    }

    // Backward-compatibility redirect
    public async Task<IActionResult> Article3()
    {
        var article = await _context.News
            .FirstOrDefaultAsync(n => n.Title == "РЎ СЃРѕР»СЊСЋ, РЅРѕ СЃРЅРёР¶Р°РµС‚ РґР°РІР»РµРЅРёРµ: РЅР°Р№РґРµРЅ В«СЃСѓРїРµСЂС„СѓРґВ», РєРѕС‚РѕСЂС‹Р№ РјРѕР¶РЅРѕ РїСЂРёРіРѕС‚РѕРІРёС‚СЊ РґРѕРјР°");

        if (article == null)
        {
            return RedirectToPage("/InDevelopment");
        }

        return RedirectToPage("/Article", new { id = article.Id });
    }

    public Task<IActionResult> Soups()
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Soups"));
    }

    [HttpGet("home/recipe/{slug}")]
    public Task<IActionResult> Recipe(string slug)
    {
        return Task.FromResult<IActionResult>(RedirectToPage("/Recipe", new { slug }));
    }

    public IActionResult Kharcho()
    {
        return RedirectToPage("/Recipe", new { slug = "kharcho" });
    }

    public IActionResult Mushrooms()
    {
        return RedirectToPage("/Recipe", new { slug = "mushrooms" });
    }

    public IActionResult Olivie()
    {
        return RedirectToPage("/Recipe", new { slug = "olivie" });
    }

    public IActionResult Borsch()
    {
        return RedirectToPage("/Recipe", new { slug = "borsch" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return RedirectToPage("/ErrorPage");
    }
}
