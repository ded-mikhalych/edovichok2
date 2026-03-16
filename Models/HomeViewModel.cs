namespace WebApplication.Models
{
    public class HomeViewModel
    {
        public IEnumerable<News> LatestNews { get; set; } = new List<News>();
        public IEnumerable<Recipe> NewestRecipes { get; set; } = new List<Recipe>();
        public IEnumerable<CategoryWithCount> PopularCategories { get; set; } = new List<CategoryWithCount>();
    }

    public class CategoryWithCount
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public int RecipeCount { get; set; }
    }
}
