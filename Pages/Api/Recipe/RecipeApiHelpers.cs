using Microsoft.EntityFrameworkCore;
using WebApplication.Data;

namespace WebApplication.Pages.Api.Recipe;

internal static class RecipeApiHelpers
{
    public const string ClientKeyHeaderName = "X-Client-Key";

    public static string ToSlug(string text)
    {
        var normalized = text.Trim().ToLowerInvariant();
        var chars = normalized.Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray();
        var slug = new string(chars);

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }

    public static int ResolveCategoryId(string? cuisine)
    {
        if (string.IsNullOrWhiteSpace(cuisine))
            return 2;

        var c = cuisine.ToLowerInvariant();
        if (c == "first-course")
            return 1;
        if (c == "second-course")
            return 2;
        if (c == "pastry")
            return 3;
        if (c == "drinks")
            return 4;

        if (c.Contains("перв") || c.Contains("суп") || c.Contains("бульон"))
            return 1;
        if (c.Contains("выпеч") || c.Contains("пирог") || c.Contains("десерт"))
            return 3;
        if (c.Contains("напит") || c.Contains("чай") || c.Contains("кофе") || c.Contains("компот") || c.Contains("лимонад"))
            return 4;
        if (c.Contains("завт"))
            return 2;

        return 2;
    }

    public static string GetPrimaryIngredientFilter(string displayText)
    {
        if (string.IsNullOrWhiteSpace(displayText))
            return string.Empty;

        var normalized = displayText.ToLowerInvariant();

        return normalized switch
        {
            var s when s.Contains("кур") => "Курица",
            var s when s.Contains("картоф") || s.Contains("картош") => "Картофель",
            var s when s.Contains("тыкв") => "Тыква",
            var s when s.Contains("творог") => "Творог",
            var s when s.Contains("варень") => "Варенье",
            var s when s.Contains("рис") => "Рис",
            var s when s.Contains("помидор") || s.Contains("томат") => "Томаты",
            var s when s.Contains("ягод") => "Ягоды",
            var s when s.Contains("лимон") || s.Contains("лайм") => "Лимон",
            var s when s.Contains("фет") => "Фета",
            _ => string.Empty
        };
    }

    public static IReadOnlyList<string> GetIngredientSearchPatterns(string filterName)
    {
        if (string.IsNullOrWhiteSpace(filterName))
            return Array.Empty<string>();

        var normalized = filterName.ToLowerInvariant().Trim();

        if (normalized.Contains("кур"))
            return new[] { "кур" };
        if (normalized.Contains("картоф"))
            return new[] { "картоф", "картош" };
        if (normalized.Contains("тыкв"))
            return new[] { "тыкв" };
        if (normalized.Contains("творог"))
            return new[] { "творог" };
        if (normalized.Contains("варень"))
            return new[] { "варень" };
        if (normalized.Contains("рис"))
            return new[] { "рис" };
        if (normalized.Contains("томат") || normalized.Contains("помидор"))
            return new[] { "томат", "помидор" };
        if (normalized.Contains("ягод"))
            return new[] { "ягод" };
        if (normalized.Contains("лимон"))
            return new[] { "лимон", "лайм" };
        if (normalized.Contains("фет"))
            return new[] { "фет" };

        return new[] { normalized };
    }

    public static bool MatchesIngredientFilter(IEnumerable<string> ingredientTexts, string filterName)
    {
        var patterns = GetIngredientSearchPatterns(filterName);
        if (patterns.Count == 0)
            return false;

        return ingredientTexts.Any(text =>
        {
            var normalized = (text ?? string.Empty).ToLowerInvariant();
            return patterns.Any(pattern => normalized.Contains(pattern));
        });
    }

    public static async Task<string> SaveImageAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
            ? "image/jpeg"
            : file.ContentType.Trim().ToLowerInvariant();

        if (contentType != "image/jpeg" &&
            contentType != "image/png" &&
            contentType != "image/webp")
        {
            contentType = "image/jpeg";
        }

        var base64 = Convert.ToBase64String(memoryStream.ToArray());
        return $"data:{contentType};base64,{base64}";
    }

    public static string? GetClientKey(HttpRequest request)
    {
        if (!request.Headers.TryGetValue(ClientKeyHeaderName, out var headerValue))
        {
            return null;
        }

        var clientKey = headerValue.ToString().Trim();
        return string.IsNullOrWhiteSpace(clientKey) ? null : clientKey;
    }

    public static async Task SyncPrimaryKeySequencesAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync(@"
SELECT setval(pg_get_serial_sequence('""Recipes""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""Recipes"";
SELECT setval(pg_get_serial_sequence('""RecipeIngredients""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeIngredients"";
SELECT setval(pg_get_serial_sequence('""RecipeSteps""', 'Id'), COALESCE(MAX(""Id""), 1)) FROM ""RecipeSteps"";
");
    }
}
