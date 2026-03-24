namespace WebApplication.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Cuisine { get; set; }
        public int Difficulty { get; set; } // 1=Easy, 2=Medium, 3=Hard
        public string? ImageFileName { get; set; }
        public bool IsFavorite { get; set; }
        public int CookingTime { get; set; } // in minutes

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
        public ICollection<RecipeComment> Comments { get; set; } = new List<RecipeComment>();

        public DateTime CreatedAt { get; set; }
        
        public int RatingSum { get; set; }
        public int RatingCount { get; set; }
    }
}
