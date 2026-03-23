using Microsoft.AspNetCore.Http;

namespace WebApplication.Models.Requests
{
    public class CreateRecipeRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Cuisine { get; set; }
        public int Difficulty { get; set; }
        public int CookingTime { get; set; } = 30;

        public List<string> Ingredients { get; set; } = new();
        public List<string> Steps { get; set; } = new();

        public IFormFile? MainImage { get; set; }
    }
}
