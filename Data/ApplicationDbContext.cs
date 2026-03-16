using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Categories seed data
            var categories = new Category[]
            {
                new Category { Id = 1, Name = "Russian", DisplayName = "Русская кухня" },
                new Category { Id = 2, Name = "European", DisplayName = "Европейская кухня" },
                new Category { Id = 3, Name = "Asian", DisplayName = "Азиатская кухня" }
            };

            modelBuilder.Entity<Category>().HasData(categories);

            // Recipes seed data
            var recipes = new Recipe[]
            {
                new Recipe
                {
                    Id = 1,
                    Name = "Сырники",
                    Description = "Традиционный русский завтрак из творога",
                    Difficulty = 1,
                    CategoryId = 1,
                    ImageFileName = "syrniki.png",
                    CookingTime = 20,
                    CreatedAt = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 2,
                    Name = "Харчо",
                    Description = "Грузинский суп с говядиной и рисом",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "kharcho.png",
                    CookingTime = 60,
                    CreatedAt = new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 3,
                    Name = "Грибы в сметане",
                    Description = "Ароматное блюдо из лесных грибов",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "mushrooms.png",
                    CookingTime = 40,
                    CreatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 4,
                    Name = "Борщ",
                    Description = "Украинский свёкольный суп с говядиной",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "borsch.png",
                    CookingTime = 90,
                    CreatedAt = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 5,
                    Name = "Оливье",
                    Description = "Классический русский салат",
                    Difficulty = 1,
                    CategoryId = 1,
                    ImageFileName = "olivie.png",
                    CookingTime = 30,
                    CreatedAt = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 6,
                    Name = "Греческий салат",
                    Description = "Свежий салат с помидорами, огурцами и фетой",
                    Difficulty = 1,
                    CategoryId = 2,
                    ImageFileName = "greece_salad.png",
                    CookingTime = 15,
                    CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                }
            };

            modelBuilder.Entity<Recipe>().HasData(recipes);

            // News seed data
            var news = new News[]
            {
                new News
                {
                    Id = 1,
                    Title = "Налог на чипсы и газировку",
                    Summary = "В России предлагают ввести налог на вредные продукты питания.",
                    ImageFileName = "news1.png",
                    CreatedAt = new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 2,
                    Title = "Фрукт против похмелья",
                    Summary = "Учёные нашли фрукт, который помогает быстрее восстановиться.",
                    ImageFileName = "news2.png",
                    CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 3,
                    Title = "Подсластители под угрозой",
                    Summary = "Исследователи признали безопасным только один вид подсластителя.",
                    ImageFileName = "news3.png",
                    CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 4,
                    Title = "История грузинской кухни",
                    Summary = "Читайте о традициях и вкусах грузинской кухни.",
                    ImageFileName = "news4.jpg",
                    CreatedAt = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            modelBuilder.Entity<News>().HasData(news);
        }
    }
}
