using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Services;

public class SiteContentService
{
    private readonly ApplicationDbContext _context;

    public SiteContentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HomeViewModel> GetHomeViewModelAsync()
    {
        var latestNews = await _context.News
            .OrderByDescending(n => n.CreatedAt)
            .Take(4)
            .ToListAsync();

        var newestRecipes = await _context.Recipes
            .Include(r => r.Category)
            .OrderByDescending(r => r.CreatedAt)
            .Take(6)
            .ToListAsync();

        return new HomeViewModel
        {
            LatestNews = latestNews.Select(n => new NewsCardViewModel
            {
                ArticleId = n.Id,
                Title = n.Title ?? string.Empty,
                Summary = n.Summary ?? string.Empty,
                ImageSrc = ResolveImagePath(n.ImageFileName),
                ActionName = "Article"
            }).ToList(),
            NewestRecipes = newestRecipes.Select(r => new RecipeCardViewModel
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                Slug = r.Slug,
                ImageSrc = ResolveImagePath(r.ImageFileName),
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? "InDevelopment" : "Recipe"
            }).ToList(),
            RestaurantReviews = GetRestaurantReviews(),
            PopularCategories = new List<CategoryCardViewModel>()
        };
    }

    public Task<List<RestaurantReviewCardViewModel>> GetRestaurantReviewsAsync()
    {
        return Task.FromResult(GetRestaurantReviews());
    }

    public async Task<ArticleViewModel?> GetArticleAsync(int id)
    {
        var article = await _context.News.FindAsync(id);
        if (article == null)
        {
            return null;
        }

        return new ArticleViewModel
        {
            Title = article.Title ?? string.Empty,
            ImageSrc = ResolveImagePath(article.ImageFileName),
            Summary = article.Summary ?? string.Empty,
            ContentHtml = article.ContentHtml
        };
    }

    public async Task<RecipePageViewModel?> GetRecipeAsync(string slug)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Category)
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Slug == slug);

        if (recipe == null)
        {
            return null;
        }

        return new RecipePageViewModel
        {
            Name = recipe.Name ?? string.Empty,
            AuthorText = string.IsNullOrWhiteSpace(recipe.Author) ? "Не указан" : recipe.Author,
            Description = recipe.Description ?? string.Empty,
            MainImageSrc = ResolveImagePath(recipe.ImageFileName),
            DifficultyText = GetDifficultyText(recipe.Difficulty),
            CuisineText = string.IsNullOrWhiteSpace(recipe.Cuisine)
                ? recipe.Category?.DisplayName ?? "Не указано"
                : recipe.Cuisine,
            Ingredients = recipe.Ingredients
                .OrderBy(i => i.SortOrder)
                .Select(i => i.DisplayText)
                .ToList(),
            Steps = recipe.Steps
                .OrderBy(s => s.StepNumber)
                .Select(s => new RecipeStepViewModel
                {
                    StepNumber = s.StepNumber,
                    Description = s.Description
                })
                .ToList()
        };
    }

    public async Task<List<RecipeLinkViewModel>> GetSoupsAsync()
    {
        return await _context.Recipes
            .Include(r => r.Category)
            .Where(r => r.Category != null && r.Category.Name == "Russian")
            .OrderBy(r => r.Name)
            .Select(r => new RecipeLinkViewModel
            {
                Name = r.Name ?? string.Empty,
                ActionName = string.IsNullOrWhiteSpace(r.Slug) ? "InDevelopment" : "Recipe",
                Slug = r.Slug
            })
            .ToListAsync();
    }

    public async Task<(int recipeCount, int articleCount, int categoryCount)> GetMetricsAsync()
    {
        var recipeCount = await _context.Recipes.CountAsync();
        var articleCount = await _context.News.CountAsync();
        var categoryCount = await _context.Categories.CountAsync();
        return (recipeCount, articleCount, categoryCount);
    }

    public static string ResolveImagePath(string? imageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageFileName))
        {
            return "/images/placeholder.png";
        }

        return imageFileName.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? imageFileName
            : $"/images/{imageFileName}";
    }

    public static string GetDifficultyText(int difficulty)
    {
        return difficulty switch
        {
            1 => "Легко",
            2 => "Средне",
            3 => "Сложно",
            _ => "Не указано"
        };
    }

    private static List<RestaurantReviewCardViewModel> GetRestaurantReviews()
    {
        return new List<RestaurantReviewCardViewModel>
        {
            new()
            {
                Title = "Basil Market",
                Summary = "Спокойное городское бистро с сильными завтраками, ровным сервисом и хорошей базовой кухней без показной сложности.",
                ImageSrc = "/images/news1.png",
                District = "Ленинский район",
                Format = "Бистро на каждый день"
            },
            new()
            {
                Title = "Nord Bread",
                Summary = "Пекарня с полноценной кухней: особенно хороши хлебная витрина, суп дня и тёплая посадка для длинных разговоров.",
                ImageSrc = "/images/news2.png",
                District = "Центр",
                Format = "Пекарня-ресторан"
            },
            new()
            {
                Title = "Saffron Yard",
                Summary = "Яркое место с громким характером, пряной кухней и хорошей энергией в зале. Лучше для компании, чем для тихого ужина.",
                ImageSrc = "/images/news3.png",
                District = "Набережная",
                Format = "Современный casual"
            },
            new()
            {
                Title = "Garden Room",
                Summary = "Тихий ресторан с сильной овощной линией, аккуратной подачей и блюдами из духовки, к которым хочется возвращаться.",
                ImageSrc = "/images/news4.png",
                District = "Старая Самара",
                Format = "Спокойный ресторан"
            }
        };
    }
}
