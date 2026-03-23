namespace WebApplication.Models
{
    public class HomeViewModel
    {
        public IEnumerable<NewsCardViewModel> LatestNews { get; set; } = new List<NewsCardViewModel>();
        public IEnumerable<RecipeCardViewModel> NewestRecipes { get; set; } = new List<RecipeCardViewModel>();
        public IEnumerable<RestaurantReviewCardViewModel> RestaurantReviews { get; set; } = new List<RestaurantReviewCardViewModel>();
        public IEnumerable<CategoryCardViewModel> PopularCategories { get; set; } = new List<CategoryCardViewModel>();
    }

    public class NewsCardViewModel
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageSrc { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
    }

    public class RecipeCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string ImageSrc { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
    }

    public class RestaurantReviewCardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageSrc { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
    }

    public class CategoryCardViewModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public int RecipeCount { get; set; }
    }
}
