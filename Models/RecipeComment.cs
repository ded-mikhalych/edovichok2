namespace WebApplication.Models
{
    public class RecipeComment
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public string ClientKey { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
