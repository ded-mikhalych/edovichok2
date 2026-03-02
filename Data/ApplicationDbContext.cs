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
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
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
                    CreatedAt = DateTime.UtcNow.AddDays(-8)
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
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
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
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
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
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
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
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            modelBuilder.Entity<Recipe>().HasData(recipes);
        }
    }
}
