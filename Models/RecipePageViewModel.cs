namespace WebApplication.Models
{
    public class RecipePageViewModel
    {
        public Recipe Recipe { get; set; } = new();
        public List<RecipeIngredient> Ingredients { get; set; } = new();
        public List<RecipeStep> Steps { get; set; } = new();
        public double AverageRating => Recipe.RatingCount > 0 ? (double)Recipe.RatingSum / Recipe.RatingCount : 0.0;
    }
}
