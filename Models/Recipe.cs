namespace WebApplication.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Difficulty { get; set; } // 1=Easy, 2=Medium, 3=Hard
        public string? ImageFileName { get; set; }
        public int CookingTime { get; set; } // in minutes

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RatingSum { get; set; }
        public int RatingCount { get; set; }
    }
}
