namespace WebApplication.Models
{
    public class RecipeStep
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;

        // Relative path under wwwroot/images, e.g. kharcho/step1.jpg.
        public string ImagePath { get; set; } = string.Empty;
    }
}
