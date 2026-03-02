namespace WebApplication.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
