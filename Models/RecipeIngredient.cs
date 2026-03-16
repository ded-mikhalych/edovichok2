namespace WebApplication.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }

        // Human-readable ingredient line from the source recipes.
        public string DisplayText { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
