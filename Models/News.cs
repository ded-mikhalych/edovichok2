namespace WebApplication.Models
{
    public class News
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? ContentHtml { get; set; }
        public string? ImageFileName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
