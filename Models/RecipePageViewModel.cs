namespace WebApplication.Models
{
    public class RecipePageViewModel
    {
        public Recipe Recipe { get; set; } = new();
        public List<string> Ingredients { get; set; } = new();
        public List<RecipeStepViewModel> Steps { get; set; } = new();
        public double AverageRating => Recipe.RatingCount > 0 ? (double)Recipe.RatingSum / Recipe.RatingCount : 0.0;
    }

    public class RecipeStepViewModel
    {
        public int Number { get; set; }
        public string Text { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
