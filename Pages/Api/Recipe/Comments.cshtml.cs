using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.Pages.Api.Recipe;

[IgnoreAntiforgeryToken]
public class CommentsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CommentsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int id, int limit = 2)
    {
        try
        {
            var normalizedLimit = Math.Clamp(limit, 1, 5);
            var comments = await _context.RecipeComments
                .Where(c => c.RecipeId == id)
                .OrderByDescending(c => c.CreatedAt)
                .Take(normalizedLimit)
                .Select(c => new
                {
                    c.Id,
                    c.Body,
                    c.CreatedAt
                })
                .ToListAsync();

            return new JsonResult(new { success = true, data = comments });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var clientKey = RecipeApiHelpers.GetClientKey(Request);
            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return new JsonResult(new { success = false, message = "Missing client key." }) { StatusCode = 400 };
            }

            var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == id);
            if (!recipeExists)
            {
                return new JsonResult(new { success = false, message = "Recipe not found." }) { StatusCode = 404 };
            }

            var request = await JsonSerializer.DeserializeAsync<CreateCommentRequest>(
                Request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var body = request?.Body?.Trim() ?? string.Empty;
            if (body.Length < 3)
            {
                return new JsonResult(new { success = false, message = "Комментарий должен содержать минимум 3 символа." }) { StatusCode = 400 };
            }

            if (body.Length > 160)
            {
                return new JsonResult(new { success = false, message = "Комментарий не должен превышать 160 символов." }) { StatusCode = 400 };
            }

            var throttleThreshold = DateTime.UtcNow.AddMinutes(-1);
            var hasRecentComment = await _context.RecipeComments.AnyAsync(c =>
                c.RecipeId == id &&
                c.ClientKey == clientKey &&
                c.CreatedAt >= throttleThreshold);

            if (hasRecentComment)
            {
                return new JsonResult(new { success = false, message = "Подождите минуту перед следующим комментарием." }) { StatusCode = 429 };
            }

            var comment = new RecipeComment
            {
                RecipeId = id,
                ClientKey = clientKey,
                Body = body,
                CreatedAt = DateTime.UtcNow
            };

            _context.RecipeComments.Add(comment);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    comment.Id,
                    comment.Body,
                    comment.CreatedAt
                }
            });
        }
        catch (JsonException)
        {
            return new JsonResult(new { success = false, message = "Некорректный формат комментария." }) { StatusCode = 400 };
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = ex.Message }) { StatusCode = 500 };
        }
    }

    private sealed class CreateCommentRequest
    {
        public string? Body { get; set; }
    }
}
