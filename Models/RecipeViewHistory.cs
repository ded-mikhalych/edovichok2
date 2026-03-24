namespace WebApplication.Models
{
    public class RecipeViewHistory
    {
        public int Id { get; set; }
        public string ClientKey { get; set; } = string.Empty;
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
