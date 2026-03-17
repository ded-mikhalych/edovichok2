namespace WebApplication.Models
{
    public class RecipePageViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string AuthorText { get; set; } = "Не указан";
        public string Description { get; set; } = string.Empty;
        public string MainImageSrc { get; set; } = string.Empty;
        public string DifficultyText { get; set; } = string.Empty;
        public string RatingText { get; set; } = "-";
        public string CuisineText { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
        public List<RecipeStepViewModel> Steps { get; set; } = new();
    }

    public class RecipeStepViewModel
    {
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageSrc { get; set; } = string.Empty;
    }
}
