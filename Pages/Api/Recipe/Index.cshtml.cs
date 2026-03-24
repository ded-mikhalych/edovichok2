using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;
using WebApplication.Models.Requests;
using WebApplication.Services;
using RecipeEntity = WebApplication.Models.Recipe;

namespace WebApplication.Pages.Api.Recipe;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> OnPostAsync(CreateRecipeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length < 3)
        {
            return new JsonResult(new { success = false, message = "РќР°Р·РІР°РЅРёРµ РґРѕР»Р¶РЅРѕ СЃРѕРґРµСЂР¶Р°С‚СЊ РјРёРЅРёРјСѓРј 3 СЃРёРјРІРѕР»Р°." }) { StatusCode = 400 };
        }

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 10)
        {
            return new JsonResult(new { success = false, message = "РћРїРёСЃР°РЅРёРµ РґРѕР»Р¶РЅРѕ СЃРѕРґРµСЂР¶Р°С‚СЊ РјРёРЅРёРјСѓРј 10 СЃРёРјРІРѕР»РѕРІ." }) { StatusCode = 400 };
        }

        if (request.Difficulty < 1 || request.Difficulty > 3)
        {
            return new JsonResult(new { success = false, message = "Р’С‹Р±РµСЂРёС‚Рµ РєРѕСЂСЂРµРєС‚РЅСѓСЋ СЃР»РѕР¶РЅРѕСЃС‚СЊ." }) { StatusCode = 400 };
        }

        var ingredients = request.Ingredients
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(i => i.Trim())
            .ToList();

        var steps = request.Steps
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToList();

        if (ingredients.Count == 0)
        {
            return new JsonResult(new { success = false, message = "Р”РѕР±Р°РІСЊС‚Рµ С…РѕС‚СЏ Р±С‹ РѕРґРёРЅ РёРЅРіСЂРµРґРёРµРЅС‚." }) { StatusCode = 400 };
        }

        if (steps.Count == 0)
        {
            return new JsonResult(new { success = false, message = "Р”РѕР±Р°РІСЊС‚Рµ С…РѕС‚СЏ Р±С‹ РѕРґРёРЅ С€Р°Рі." }) { StatusCode = 400 };
        }

        await RecipeApiHelpers.SyncPrimaryKeySequencesAsync(_context);

        var slug = await GenerateUniqueSlugAsync(request.Title);
        var categoryId = RecipeApiHelpers.ResolveCategoryId(request.Cuisine);
        var imageFolder = Path.Combine(_environment.WebRootPath, "images", "user");
        Directory.CreateDirectory(imageFolder);

        var mainImageFileName = request.MainImage != null
            ? await RecipeApiHelpers.SaveImageAsync(request.MainImage, imageFolder, slug + "-main")
            : SiteContentService.DefaultExternalImage;

        var recipe = new RecipeEntity
        {
            Name = request.Title.Trim(),
            Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim(),
            Slug = slug,
            Description = request.Description.Trim(),
            Cuisine = string.IsNullOrWhiteSpace(request.Cuisine) ? null : request.Cuisine.Trim(),
            Difficulty = request.Difficulty,
            ImageFileName = mainImageFileName,
            IsFavorite = false,
            CookingTime = request.CookingTime > 0 ? request.CookingTime : 30,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow,
            RatingSum = 0,
            RatingCount = 0,
            Ingredients = ingredients.Select((text, idx) => new RecipeIngredient
            {
                DisplayText = text,
                SortOrder = idx + 1
            }).ToList(),
            Steps = steps.Select((text, idx) => new RecipeStep
            {
                StepNumber = idx + 1,
                Description = text,
                ImagePath = string.Empty
            }).ToList()
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return new JsonResult(new
        {
            success = true,
            message = "Р РµС†РµРїС‚ СѓСЃРїРµС€РЅРѕ СЃРѕС…СЂР°РЅС‘РЅ.",
            data = new
            {
                recipe.Id,
                recipe.Slug
            }
        });
    }

    private async Task<string> GenerateUniqueSlugAsync(string title)
    {
        var baseSlug = RecipeApiHelpers.ToSlug(title);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "recipe";
        }

        var slug = baseSlug;
        var index = 1;
        while (await _context.Recipes.AnyAsync(r => r.Slug == slug))
        {
            slug = $"{baseSlug}-{index}";
            index++;
        }

        return slug;
    }
}
