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

        if (c.Contains("РїРµСЂРІ") || c.Contains("СЃСѓРї") || c.Contains("Р±СѓР»СЊРѕРЅ"))
            return 1;

        if (c.Contains("РІС‹РїРµС‡") || c.Contains("РїРёСЂРѕРі") || c.Contains("РґРµСЃРµСЂС‚"))
            return 3;

        if (c.Contains("РЅР°РїРёС‚") || c.Contains("С‡Р°Р№") || c.Contains("РєРѕС„Рµ") || c.Contains("РєРѕРјРїРѕС‚") || c.Contains("Р»РёРјРѕРЅР°Рґ"))
            return 4;

        if (c.Contains("Р·Р°РІС‚"))
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
            var s when s.Contains("РєСѓСЂ") => "РљСѓСЂРёС†Р°",
            var s when s.Contains("РєР°СЂС‚РѕС„") => "РљР°СЂС‚РѕС„РµР»СЊ",
            var s when s.Contains("С‚С‹РєРІ") => "РўС‹РєРІР°",
            var s when s.Contains("С‚РІРѕСЂРѕРі") => "РўРІРѕСЂРѕРі",
            var s when s.Contains("РІР°СЂРµРЅСЊ") => "Р’Р°СЂРµРЅСЊРµ",
            var s when s.Contains("СЂРёСЃ") => "Р РёСЃ",
            var s when s.Contains("РїРѕРјРёРґРѕСЂ") || s.Contains("С‚РѕРјР°С‚") => "РўРѕРјР°С‚С‹",
            var s when s.Contains("СЏРіРѕРґ") => "РЇРіРѕРґС‹",
            var s when s.Contains("Р»РёРјРѕРЅ") => "Р›РёРјРѕРЅ",
            var s when s.Contains("С„РµС‚") => "Р¤РµС‚Р°",
            _ => string.Empty
        };
    }

    public static async Task<string> SaveImageAsync(IFormFile file, string folderPath, string filePrefix)
    {
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        var safeExt = extension.ToLowerInvariant();
        if (safeExt != ".jpg" && safeExt != ".jpeg" && safeExt != ".png" && safeExt != ".webp")
        {
            safeExt = ".jpg";
        }

        var fileName = $"{filePrefix}-{Guid.NewGuid():N}{safeExt}";
        var fullPath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"user/{fileName}";
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
