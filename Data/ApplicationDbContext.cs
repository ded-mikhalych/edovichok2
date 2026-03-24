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
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeViewHistory> RecipeViewHistories { get; set; }
        public DbSet<News> News { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Categories seed data
            var categories = new Category[]
            {
                new Category { Id = 1, Name = "first-course", DisplayName = "Первое" },
                new Category { Id = 2, Name = "second-course", DisplayName = "Второе" },
                new Category { Id = 3, Name = "pastry", DisplayName = "Выпечка" },
                new Category { Id = 4, Name = "drinks", DisplayName = "Напитки" }
            };

            modelBuilder.Entity<Category>().HasData(categories);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeStep>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.Steps)
                .HasForeignKey(s => s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeViewHistory>()
                .HasOne(h => h.Recipe)
                .WithMany()
                .HasForeignKey(h => h.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeViewHistory>()
                .HasIndex(h => new { h.ClientKey, h.RecipeId })
                .IsUnique();

            // Recipes seed data
            var recipes = new Recipe[]
            {
                new Recipe
                {
                    Id = 1,
                    Name = "Сырники",
                    Slug = "syrniki",
                    Description = "Традиционный русский завтрак из творога",
                    Cuisine = "Русская",
                    Difficulty = 1,
                    CategoryId = 1,
                    ImageFileName = "syrniki.png",
                    IsFavorite = true,
                    CookingTime = 20,
                    CreatedAt = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 2,
                    Name = "Харчо",
                    Slug = "kharcho",
                    Description = "Грузинский суп с говядиной и рисом",
                    Cuisine = "Грузинская",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "kharcho.png",
                    IsFavorite = true,
                    CookingTime = 60,
                    CreatedAt = new DateTime(2026, 2, 27, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 3,
                    Name = "Грибы в сметане",
                    Slug = "mushrooms",
                    Description = "Ароматное блюдо из лесных грибов",
                    Cuisine = "Домашняя",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "mushrooms.png",
                    IsFavorite = true,
                    CookingTime = 40,
                    CreatedAt = new DateTime(2026, 3, 2, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 4,
                    Name = "Борщ",
                    Slug = "borsch",
                    Description = "Украинский свёкольный суп с говядиной",
                    Cuisine = "Домашняя",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "borsch.png",
                    IsFavorite = true,
                    CookingTime = 90,
                    CreatedAt = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 5,
                    Name = "Оливье",
                    Slug = "olivie",
                    Description = "Классический русский салат",
                    Cuisine = "Русская",
                    Difficulty = 1,
                    CategoryId = 1,
                    ImageFileName = "olivie.png",
                    IsFavorite = true,
                    CookingTime = 30,
                    CreatedAt = new DateTime(2026, 3, 4, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 6,
                    Name = "Греческий салат",
                    Slug = "greece-salad",
                    Description = "Свежий салат с помидорами, огурцами и фетой",
                    Cuisine = "Европейская",
                    Difficulty = 1,
                    CategoryId = 2,
                    ImageFileName = "greece_salad.png",
                    IsFavorite = false,
                    CookingTime = 15,
                    CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 7,
                    Name = "Узбекский плов",
                    Slug = "plov",
                    Description = "Классический плов с бараниной, рисом и морковью в казане",
                    Cuisine = "Узбекская",
                    Difficulty = 2,
                    CategoryId = 3,
                    ImageFileName = "kharcho.png",
                    IsFavorite = false,
                    CookingTime = 90,
                    CreatedAt = new DateTime(2026, 3, 6, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 8,
                    Name = "Том ям с креветками",
                    Slug = "tom-yum",
                    Description = "Пряный тайский суп на кокосовом молоке с креветками и лаймом",
                    Cuisine = "Тайская",
                    Difficulty = 3,
                    CategoryId = 3,
                    ImageFileName = "mushrooms.png",
                    IsFavorite = false,
                    CookingTime = 45,
                    CreatedAt = new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 9,
                    Name = "Рататуй",
                    Slug = "ratatouille",
                    Description = "Французское овощное рагу из баклажанов, кабачков и томатов",
                    Cuisine = "Французская",
                    Difficulty = 2,
                    CategoryId = 2,
                    ImageFileName = "greece_salad.png",
                    IsFavorite = false,
                    CookingTime = 60,
                    CreatedAt = new DateTime(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                },
                new Recipe
                {
                    Id = 10,
                    Name = "Бефстроганов",
                    Slug = "beef-stroganoff",
                    Description = "Говядина в сливочно-сметанном соусе с луком и грибами",
                    Cuisine = "Русская",
                    Difficulty = 2,
                    CategoryId = 1,
                    ImageFileName = "olivie.png",
                    IsFavorite = false,
                    CookingTime = 50,
                    CreatedAt = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                    RatingSum = 0,
                    RatingCount = 0
                }
            };

            modelBuilder.Entity<Recipe>().HasData(recipes);

            var recipeIngredients = new RecipeIngredient[]
            {
                new RecipeIngredient { Id = 1, RecipeId = 1, SortOrder = 1, DisplayText = "Творог — 300 г" },
                new RecipeIngredient { Id = 2, RecipeId = 1, SortOrder = 2, DisplayText = "Яйцо — 1 шт." },
                new RecipeIngredient { Id = 3, RecipeId = 1, SortOrder = 3, DisplayText = "Сахар — 2 ст.л." },
                new RecipeIngredient { Id = 4, RecipeId = 1, SortOrder = 4, DisplayText = "Мука — 4 ст.л." },
                new RecipeIngredient { Id = 5, RecipeId = 1, SortOrder = 5, DisplayText = "Щепотка соли" },
                new RecipeIngredient { Id = 6, RecipeId = 1, SortOrder = 6, DisplayText = "Масло для жарки" },

                new RecipeIngredient { Id = 7, RecipeId = 2, SortOrder = 1, DisplayText = "Вода — 2.5 л" },
                new RecipeIngredient { Id = 8, RecipeId = 2, SortOrder = 2, DisplayText = "Говядина — 700 г" },
                new RecipeIngredient { Id = 9, RecipeId = 2, SortOrder = 3, DisplayText = "Помидоры — 400 г" },
                new RecipeIngredient { Id = 10, RecipeId = 2, SortOrder = 4, DisplayText = "Картофель — 300 г" },
                new RecipeIngredient { Id = 11, RecipeId = 2, SortOrder = 5, DisplayText = "Рис — 100 г" },
                new RecipeIngredient { Id = 12, RecipeId = 2, SortOrder = 6, DisplayText = "Лук — 150 г" },
                new RecipeIngredient { Id = 13, RecipeId = 2, SortOrder = 7, DisplayText = "Чеснок — 3 зубчика" },
                new RecipeIngredient { Id = 14, RecipeId = 2, SortOrder = 8, DisplayText = "Петрушка — 20 г" },
                new RecipeIngredient { Id = 15, RecipeId = 2, SortOrder = 9, DisplayText = "Хмели-сунели — 1.5 ч.л." },
                new RecipeIngredient { Id = 16, RecipeId = 2, SortOrder = 10, DisplayText = "Соль и перец — по вкусу" },

                new RecipeIngredient { Id = 17, RecipeId = 3, SortOrder = 1, DisplayText = "Опята — 1.2 кг" },
                new RecipeIngredient { Id = 18, RecipeId = 3, SortOrder = 2, DisplayText = "Соль — 80 г" },
                new RecipeIngredient { Id = 19, RecipeId = 3, SortOrder = 3, DisplayText = "Перец горошком — 8-10 шт." },
                new RecipeIngredient { Id = 20, RecipeId = 3, SortOrder = 4, DisplayText = "Лавровый лист — 3 шт." },
                new RecipeIngredient { Id = 21, RecipeId = 3, SortOrder = 5, DisplayText = "Чеснок — 6 зубчиков" },
                new RecipeIngredient { Id = 22, RecipeId = 3, SortOrder = 6, DisplayText = "Вода — 1 л" },

                new RecipeIngredient { Id = 23, RecipeId = 4, SortOrder = 1, DisplayText = "Морковь — 200 г" },
                new RecipeIngredient { Id = 24, RecipeId = 4, SortOrder = 2, DisplayText = "Свекла — 300 г" },
                new RecipeIngredient { Id = 25, RecipeId = 4, SortOrder = 3, DisplayText = "Лук — 150 г" },
                new RecipeIngredient { Id = 26, RecipeId = 4, SortOrder = 4, DisplayText = "Чеснок — 3 зубчика" },
                new RecipeIngredient { Id = 27, RecipeId = 4, SortOrder = 5, DisplayText = "Томатный сок — 700 мл" },
                new RecipeIngredient { Id = 28, RecipeId = 4, SortOrder = 6, DisplayText = "Соль — 1-1.5 ч.л." },
                new RecipeIngredient { Id = 29, RecipeId = 4, SortOrder = 7, DisplayText = "Сахар — 1 ч.л." },
                new RecipeIngredient { Id = 30, RecipeId = 4, SortOrder = 8, DisplayText = "Масло — 2 ст.л." },

                new RecipeIngredient { Id = 31, RecipeId = 5, SortOrder = 1, DisplayText = "Варёная колбаса — 300 г" },
                new RecipeIngredient { Id = 32, RecipeId = 5, SortOrder = 2, DisplayText = "Зелёный горошек — 200 г" },
                new RecipeIngredient { Id = 33, RecipeId = 5, SortOrder = 3, DisplayText = "Яйца — 4 шт." },
                new RecipeIngredient { Id = 34, RecipeId = 5, SortOrder = 4, DisplayText = "Картошка — 600 г" },
                new RecipeIngredient { Id = 35, RecipeId = 5, SortOrder = 5, DisplayText = "Соленые огурцы — 200 г" },
                new RecipeIngredient { Id = 36, RecipeId = 5, SortOrder = 6, DisplayText = "Морковь — 150 г" },
                new RecipeIngredient { Id = 37, RecipeId = 5, SortOrder = 7, DisplayText = "Майонез — 150-200 г" },
                new RecipeIngredient { Id = 38, RecipeId = 5, SortOrder = 8, DisplayText = "Соль — по вкусу" },

                new RecipeIngredient { Id = 39, RecipeId = 6, SortOrder = 1, DisplayText = "Помидоры — 250 г" },
                new RecipeIngredient { Id = 40, RecipeId = 6, SortOrder = 2, DisplayText = "Огурцы — 200 г" },
                new RecipeIngredient { Id = 41, RecipeId = 6, SortOrder = 3, DisplayText = "Фета — 150 г" },
                new RecipeIngredient { Id = 42, RecipeId = 6, SortOrder = 4, DisplayText = "Маслины — 80 г" },
                new RecipeIngredient { Id = 43, RecipeId = 6, SortOrder = 5, DisplayText = "Красный лук — 1/2 шт." },
                new RecipeIngredient { Id = 44, RecipeId = 6, SortOrder = 6, DisplayText = "Оливковое масло — 2 ст.л." },
                new RecipeIngredient { Id = 45, RecipeId = 6, SortOrder = 7, DisplayText = "Лимонный сок — 1 ст.л." },

                new RecipeIngredient { Id = 46, RecipeId = 7, SortOrder = 1, DisplayText = "Рис девзира — 700 г" },
                new RecipeIngredient { Id = 47, RecipeId = 7, SortOrder = 2, DisplayText = "Баранина — 700 г" },
                new RecipeIngredient { Id = 48, RecipeId = 7, SortOrder = 3, DisplayText = "Морковь — 700 г" },
                new RecipeIngredient { Id = 49, RecipeId = 7, SortOrder = 4, DisplayText = "Лук — 300 г" },
                new RecipeIngredient { Id = 50, RecipeId = 7, SortOrder = 5, DisplayText = "Чеснок — 2 головки" },
                new RecipeIngredient { Id = 51, RecipeId = 7, SortOrder = 6, DisplayText = "Зира — 1.5 ч.л." },
                new RecipeIngredient { Id = 52, RecipeId = 7, SortOrder = 7, DisplayText = "Барбарис — 1 ст.л." },
                new RecipeIngredient { Id = 53, RecipeId = 7, SortOrder = 8, DisplayText = "Растительное масло — 120 мл" },
                new RecipeIngredient { Id = 54, RecipeId = 7, SortOrder = 9, DisplayText = "Соль — по вкусу" },

                new RecipeIngredient { Id = 55, RecipeId = 8, SortOrder = 1, DisplayText = "Креветки — 400 г" },
                new RecipeIngredient { Id = 56, RecipeId = 8, SortOrder = 2, DisplayText = "Кокосовое молоко — 400 мл" },
                new RecipeIngredient { Id = 57, RecipeId = 8, SortOrder = 3, DisplayText = "Куриный бульон — 600 мл" },
                new RecipeIngredient { Id = 58, RecipeId = 8, SortOrder = 4, DisplayText = "Паста том ям — 2 ст.л." },
                new RecipeIngredient { Id = 59, RecipeId = 8, SortOrder = 5, DisplayText = "Лемонграсс — 2 стебля" },
                new RecipeIngredient { Id = 60, RecipeId = 8, SortOrder = 6, DisplayText = "Листья каффир-лайма — 4 шт." },
                new RecipeIngredient { Id = 61, RecipeId = 8, SortOrder = 7, DisplayText = "Шампиньоны — 200 г" },
                new RecipeIngredient { Id = 62, RecipeId = 8, SortOrder = 8, DisplayText = "Рыбный соус — 2 ст.л." },
                new RecipeIngredient { Id = 63, RecipeId = 8, SortOrder = 9, DisplayText = "Сок лайма — 2 ст.л." },

                new RecipeIngredient { Id = 64, RecipeId = 9, SortOrder = 1, DisplayText = "Баклажан — 1 крупный" },
                new RecipeIngredient { Id = 65, RecipeId = 9, SortOrder = 2, DisplayText = "Кабачок — 1 крупный" },
                new RecipeIngredient { Id = 66, RecipeId = 9, SortOrder = 3, DisplayText = "Помидоры — 5 шт." },
                new RecipeIngredient { Id = 67, RecipeId = 9, SortOrder = 4, DisplayText = "Болгарский перец — 2 шт." },
                new RecipeIngredient { Id = 68, RecipeId = 9, SortOrder = 5, DisplayText = "Лук — 1 шт." },
                new RecipeIngredient { Id = 69, RecipeId = 9, SortOrder = 6, DisplayText = "Чеснок — 3 зубчика" },
                new RecipeIngredient { Id = 70, RecipeId = 9, SortOrder = 7, DisplayText = "Оливковое масло — 3 ст.л." },
                new RecipeIngredient { Id = 71, RecipeId = 9, SortOrder = 8, DisplayText = "Тимьян и базилик — по вкусу" },

                new RecipeIngredient { Id = 72, RecipeId = 10, SortOrder = 1, DisplayText = "Говяжья вырезка — 600 г" },
                new RecipeIngredient { Id = 73, RecipeId = 10, SortOrder = 2, DisplayText = "Лук — 2 шт." },
                new RecipeIngredient { Id = 74, RecipeId = 10, SortOrder = 3, DisplayText = "Шампиньоны — 250 г" },
                new RecipeIngredient { Id = 75, RecipeId = 10, SortOrder = 4, DisplayText = "Сметана — 250 г" },
                new RecipeIngredient { Id = 76, RecipeId = 10, SortOrder = 5, DisplayText = "Сливки 20% — 150 мл" },
                new RecipeIngredient { Id = 77, RecipeId = 10, SortOrder = 6, DisplayText = "Горчица — 1 ч.л." },
                new RecipeIngredient { Id = 78, RecipeId = 10, SortOrder = 7, DisplayText = "Мука — 1 ст.л." },
                new RecipeIngredient { Id = 79, RecipeId = 10, SortOrder = 8, DisplayText = "Соль и перец — по вкусу" }
            };

            modelBuilder.Entity<RecipeIngredient>().HasData(recipeIngredients);

            var recipeSteps = new RecipeStep[]
            {
                new RecipeStep { Id = 1, RecipeId = 1, StepNumber = 1, Description = "Смешайте творог, яйцо, сахар и соль до однородности.", ImagePath = "syrniki/step1.jpg" },
                new RecipeStep { Id = 2, RecipeId = 1, StepNumber = 2, Description = "Добавьте муку и замесите мягкое тесто.", ImagePath = "syrniki/step2.jpg" },
                new RecipeStep { Id = 3, RecipeId = 1, StepNumber = 3, Description = "Сформируйте небольшие сырники и обваляйте в муке.", ImagePath = "syrniki/step3.jpg" },
                new RecipeStep { Id = 4, RecipeId = 1, StepNumber = 4, Description = "Обжарьте на среднем огне с двух сторон до золотистой корочки.", ImagePath = "syrniki/step4.jpg" },
                new RecipeStep { Id = 5, RecipeId = 1, StepNumber = 5, Description = "Подавайте горячими со сметаной или ягодным соусом.", ImagePath = "syrniki/step5.jpg" },

                new RecipeStep { Id = 6, RecipeId = 2, StepNumber = 1, Description = "Подготовьте ингредиенты и нарежьте мясо кусочками.", ImagePath = "kharcho/step1.jpg" },
                new RecipeStep { Id = 7, RecipeId = 2, StepNumber = 2, Description = "Сварите мясной бульон на медленном огне, снимая пену.", ImagePath = "kharcho/step2.jpg" },
                new RecipeStep { Id = 8, RecipeId = 2, StepNumber = 3, Description = "Промойте и замочите рис на 20 минут.", ImagePath = "kharcho/step3.jpg" },
                new RecipeStep { Id = 9, RecipeId = 2, StepNumber = 4, Description = "Нарежьте овощи, помидоры очистите от кожицы.", ImagePath = "kharcho/step4.jpg" },
                new RecipeStep { Id = 10, RecipeId = 2, StepNumber = 5, Description = "Добавьте в бульон рис, картофель и часть помидоров.", ImagePath = "kharcho/step5.jpg" },
                new RecipeStep { Id = 11, RecipeId = 2, StepNumber = 6, Description = "Обжарьте лук, добавьте оставшиеся помидоры и потушите.", ImagePath = "kharcho/step6.jpg" },
                new RecipeStep { Id = 12, RecipeId = 2, StepNumber = 7, Description = "Переложите зажарку в суп, добавьте специи, чеснок и зелень.", ImagePath = "kharcho/step7.jpg" },
                new RecipeStep { Id = 13, RecipeId = 2, StepNumber = 8, Description = "Дайте супу настояться 10 минут и подавайте горячим.", ImagePath = "kharcho/step8.jpg" },

                new RecipeStep { Id = 14, RecipeId = 3, StepNumber = 1, Description = "Подготовьте грибы и специи, промойте опята.", ImagePath = "mushrooms/step1.jpg" },
                new RecipeStep { Id = 15, RecipeId = 3, StepNumber = 2, Description = "Удалите корешки и тщательно переберите грибы.", ImagePath = "mushrooms/step2.jpg" },
                new RecipeStep { Id = 16, RecipeId = 3, StepNumber = 3, Description = "Отварите опята до готовности и остудите.", ImagePath = "mushrooms/step3.jpg" },
                new RecipeStep { Id = 17, RecipeId = 3, StepNumber = 4, Description = "Приготовьте рассол с солью и специями.", ImagePath = "mushrooms/step4.jpg" },
                new RecipeStep { Id = 18, RecipeId = 3, StepNumber = 5, Description = "Добавьте грибы в рассол и проварите 25 минут.", ImagePath = "mushrooms/step5.jpg" },
                new RecipeStep { Id = 19, RecipeId = 3, StepNumber = 6, Description = "Простерилизуйте банки и крышки.", ImagePath = "mushrooms/step6.jpg" },
                new RecipeStep { Id = 20, RecipeId = 3, StepNumber = 7, Description = "Разложите грибы по банкам и закатайте.", ImagePath = "mushrooms/step7.jpg" },
                new RecipeStep { Id = 21, RecipeId = 3, StepNumber = 8, Description = "Выдержите заготовку 4-6 недель в прохладном месте.", ImagePath = "mushrooms/step8.jpg" },

                new RecipeStep { Id = 22, RecipeId = 4, StepNumber = 1, Description = "Подготовьте ингредиенты и вымойте овощи.", ImagePath = "zazharka/step1.jpg" },
                new RecipeStep { Id = 23, RecipeId = 4, StepNumber = 2, Description = "Нарежьте лук и обжарьте до мягкости.", ImagePath = "zazharka/step2.jpg" },
                new RecipeStep { Id = 24, RecipeId = 4, StepNumber = 3, Description = "Добавьте свеклу и морковь, тушите на среднем огне.", ImagePath = "zazharka/step3.jpg" },
                new RecipeStep { Id = 25, RecipeId = 4, StepNumber = 4, Description = "Влейте томатный сок, добавьте соль и сахар.", ImagePath = "zazharka/step4.jpg" },
                new RecipeStep { Id = 26, RecipeId = 4, StepNumber = 5, Description = "Тушите 40-50 минут до нужной густоты.", ImagePath = "zazharka/step5.jpg" },
                new RecipeStep { Id = 27, RecipeId = 4, StepNumber = 6, Description = "Добавьте чеснок и специи за 10 минут до конца.", ImagePath = "zazharka/step6.jpg" },

                new RecipeStep { Id = 28, RecipeId = 5, StepNumber = 1, Description = "Подготовьте и отварите картофель, морковь и яйца.", ImagePath = "olivie/step1.jpg" },
                new RecipeStep { Id = 29, RecipeId = 5, StepNumber = 2, Description = "Слейте жидкость из банки с горошком.", ImagePath = "olivie/step2.jpg" },
                new RecipeStep { Id = 30, RecipeId = 5, StepNumber = 3, Description = "Нарежьте колбасу кубиками.", ImagePath = "olivie/step3.jpg" },
                new RecipeStep { Id = 31, RecipeId = 5, StepNumber = 4, Description = "Нарежьте картофель, морковь, яйца и огурцы одинаково.", ImagePath = "olivie/step4.jpg" },
                new RecipeStep { Id = 32, RecipeId = 5, StepNumber = 5, Description = "Смешайте все ингредиенты в большой миске.", ImagePath = "olivie/step5.jpg" },
                new RecipeStep { Id = 33, RecipeId = 5, StepNumber = 6, Description = "Добавьте майонез и соль по вкусу.", ImagePath = "olivie/step6.jpg" },
                new RecipeStep { Id = 34, RecipeId = 5, StepNumber = 7, Description = "Охладите салат в холодильнике перед подачей.", ImagePath = "olivie/step7.jpg" },

                new RecipeStep { Id = 35, RecipeId = 6, StepNumber = 1, Description = "Подготовьте овощи и зелень.", ImagePath = "greece_salad/step1.jpg" },
                new RecipeStep { Id = 36, RecipeId = 6, StepNumber = 2, Description = "Нарежьте огурцы и помидоры крупными кусочками.", ImagePath = "greece_salad/step2.jpg" },
                new RecipeStep { Id = 37, RecipeId = 6, StepNumber = 3, Description = "Добавьте тонкие полукольца лука и маслины.", ImagePath = "greece_salad/step3.jpg" },
                new RecipeStep { Id = 38, RecipeId = 6, StepNumber = 4, Description = "Положите кубики феты поверх салата.", ImagePath = "greece_salad/step4.jpg" },
                new RecipeStep { Id = 39, RecipeId = 6, StepNumber = 5, Description = "Заправьте маслом и лимонным соком, аккуратно перемешайте.", ImagePath = "greece_salad/step5.jpg" },

                new RecipeStep { Id = 40, RecipeId = 7, StepNumber = 1, Description = "Промойте рис до прозрачной воды и замочите на 30 минут.", ImagePath = "kharcho/step1.jpg" },
                new RecipeStep { Id = 41, RecipeId = 7, StepNumber = 2, Description = "Нарежьте баранину кубиками, лук полукольцами, морковь крупной соломкой.", ImagePath = "kharcho/step2.jpg" },
                new RecipeStep { Id = 42, RecipeId = 7, StepNumber = 3, Description = "Разогрейте казан с маслом, обжарьте мясо до румяной корочки.", ImagePath = "kharcho/step3.jpg" },
                new RecipeStep { Id = 43, RecipeId = 7, StepNumber = 4, Description = "Добавьте лук и обжарьте до золотистого цвета.", ImagePath = "kharcho/step4.jpg" },
                new RecipeStep { Id = 44, RecipeId = 7, StepNumber = 5, Description = "Всыпьте морковь и готовьте 8-10 минут, затем добавьте зиру и барбарис.", ImagePath = "kharcho/step5.jpg" },
                new RecipeStep { Id = 45, RecipeId = 7, StepNumber = 6, Description = "Влейте кипяток, чтобы покрыть содержимое, и тушите зирвак 30 минут.", ImagePath = "kharcho/step6.jpg" },
                new RecipeStep { Id = 46, RecipeId = 7, StepNumber = 7, Description = "Добавьте рис ровным слоем, долейте воду на 1 см выше риса и готовьте без крышки.", ImagePath = "kharcho/step7.jpg" },
                new RecipeStep { Id = 47, RecipeId = 7, StepNumber = 8, Description = "Когда вода уйдет, вставьте головки чеснока, накройте и томите 20 минут.", ImagePath = "kharcho/step8.jpg" },

                new RecipeStep { Id = 48, RecipeId = 8, StepNumber = 1, Description = "Очистите креветки, оставив хвостики, и подготовьте остальные ингредиенты.", ImagePath = "mushrooms/step1.jpg" },
                new RecipeStep { Id = 49, RecipeId = 8, StepNumber = 2, Description = "Доведите бульон до кипения, добавьте лемонграсс и листья каффир-лайма.", ImagePath = "mushrooms/step2.jpg" },
                new RecipeStep { Id = 50, RecipeId = 8, StepNumber = 3, Description = "Положите пасту том ям и размешайте до полного растворения.", ImagePath = "mushrooms/step3.jpg" },
                new RecipeStep { Id = 51, RecipeId = 8, StepNumber = 4, Description = "Добавьте нарезанные грибы и варите 3-4 минуты.", ImagePath = "mushrooms/step4.jpg" },
                new RecipeStep { Id = 52, RecipeId = 8, StepNumber = 5, Description = "Влейте кокосовое молоко и прогрейте суп, не доводя до активного кипения.", ImagePath = "mushrooms/step5.jpg" },
                new RecipeStep { Id = 53, RecipeId = 8, StepNumber = 6, Description = "Положите креветки и готовьте 2-3 минуты до розового цвета.", ImagePath = "mushrooms/step6.jpg" },
                new RecipeStep { Id = 54, RecipeId = 8, StepNumber = 7, Description = "Приправьте рыбным соусом и соком лайма, отрегулируйте вкус.", ImagePath = "mushrooms/step7.jpg" },
                new RecipeStep { Id = 55, RecipeId = 8, StepNumber = 8, Description = "Подавайте горячим с кинзой и долькой лайма.", ImagePath = "mushrooms/step8.jpg" },

                new RecipeStep { Id = 56, RecipeId = 9, StepNumber = 1, Description = "Нарежьте баклажан, кабачок и томаты тонкими кружками.", ImagePath = "greece_salad/step1.jpg" },
                new RecipeStep { Id = 57, RecipeId = 9, StepNumber = 2, Description = "Обжарьте лук с перцем, добавьте половину томатов и потушите соус 10 минут.", ImagePath = "greece_salad/step2.jpg" },
                new RecipeStep { Id = 58, RecipeId = 9, StepNumber = 3, Description = "Переложите соус в форму, сверху выложите овощи чередуя кружки.", ImagePath = "greece_salad/step3.jpg" },
                new RecipeStep { Id = 59, RecipeId = 9, StepNumber = 4, Description = "Сбрызните маслом, посыпьте чесноком, тимьяном и базиликом.", ImagePath = "greece_salad/step4.jpg" },
                new RecipeStep { Id = 60, RecipeId = 9, StepNumber = 5, Description = "Запекайте при 180°C около 40 минут до мягкости овощей.", ImagePath = "greece_salad/step5.jpg" },

                new RecipeStep { Id = 61, RecipeId = 10, StepNumber = 1, Description = "Нарежьте говядину тонкой соломкой поперек волокон.", ImagePath = "olivie/step1.jpg" },
                new RecipeStep { Id = 62, RecipeId = 10, StepNumber = 2, Description = "Обваляйте мясо в муке и быстро обжарьте на сильном огне.", ImagePath = "olivie/step2.jpg" },
                new RecipeStep { Id = 63, RecipeId = 10, StepNumber = 3, Description = "Отдельно обжарьте лук и грибы до испарения влаги.", ImagePath = "olivie/step3.jpg" },
                new RecipeStep { Id = 64, RecipeId = 10, StepNumber = 4, Description = "Верните мясо в сковороду, добавьте горчицу, соль и перец.", ImagePath = "olivie/step4.jpg" },
                new RecipeStep { Id = 65, RecipeId = 10, StepNumber = 5, Description = "Влейте сливки и добавьте сметану, аккуратно перемешайте.", ImagePath = "olivie/step5.jpg" },
                new RecipeStep { Id = 66, RecipeId = 10, StepNumber = 6, Description = "Тушите 8-10 минут на слабом огне до загустения соуса.", ImagePath = "olivie/step6.jpg" },
                new RecipeStep { Id = 67, RecipeId = 10, StepNumber = 7, Description = "Подавайте с картофельным пюре, рисом или пастой.", ImagePath = "olivie/step7.jpg" }
            };

            modelBuilder.Entity<RecipeStep>().HasData(recipeSteps);

            // News seed data
            var news = new News[]
            {
                new News
                {
                    Id = 1,
                    Title = "Налог на чипсы и газировку",
                    Summary = "В России предлагают ввести налог на вредные продукты питания.",
                    ContentHtml = "<p>В России обсуждается инициатива по введению налога на отдельные категории вредных продуктов. Эксперты считают, что мера может снизить потребление ультрапереработанной еды и стимулировать производителей пересматривать состав продукции.</p><p>Пока документ находится на стадии обсуждения, но тема уже вызвала широкий общественный резонанс.</p>",
                    ImageFileName = "news1.png",
                    CreatedAt = new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 2,
                    Title = "Фрукт против похмелья",
                    Summary = "Учёные нашли фрукт, который помогает быстрее восстановиться.",
                    ContentHtml = "<p>Исследователи сообщили о фрукте, содержащем вещества, которые могут облегчить симптомы похмелья и ускорить восстановление после употребления алкоголя. В публикации подчеркивается, что наибольший эффект достигается в сочетании с полноценной гидратацией и сном.</p>",
                    ImageFileName = "news2.png",
                    CreatedAt = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 3,
                    Title = "Подсластители под угрозой",
                    Summary = "Исследователи признали безопасным только один вид подсластителя.",
                    ContentHtml = "<p>Как <a href=\"https://www.neurology.org/doi/10.1212/WNL.0000000000214023\">заявлено</a> в статье для журнала Neurology, группа бразильских специалистов в течение 8 лет изучала сведения о питании и здоровье 12 тыс. человек, средний возраст которых составлял 50 лет. Добровольцы регулярно сообщали, сколько продуктов и напитков с подсластителями они употребляют.</p><p><strong>Как проводилось исследование</strong></p><p>Чтобы оценить состояние нервной системы, испытуемым давали интеллектуальные упражнения на память, владение речью, концентрацию и быстроту мыслительных процессов.</p><p>Ученые установили, что регулярное включение искусственных подсластителей в рацион существенно отражается на работе мозга. В частности, интеллектуальные способности снижаются быстрее на 60%.</p><p><strong>Результаты</strong></p><p>К провоцирующим факторам отнесли злоупотребление популярными сахарозаменителями, в том числе:</p><ul><li>аспартамом;</li><li>сахарином;</li><li>эритритом;</li><li>сорбитом;</li><li>ксилитом.</li></ul><p>Исключением из правил стала только тагатоза — ее связь с ухудшением когнитивных функций обнаружена не была. Этот редкий натуральный подсластитель содержится в ягодах, фруктах, некоторых овощах, какао-бобах, а также в молочных продуктах.</p><table style=\"border-collapse:collapse;width:100%;max-width:720px;margin:16px 0;background:#fff;\"><tr><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Происхождение</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Калорийность</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Гликемический индекс</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Особенности</th></tr><tr><td style=\"border:1px solid var(--border);padding:8px;\">натуральное</td><td style=\"border:1px solid var(--border);padding:8px;\">1,5 ккал/г</td><td style=\"border:1px solid var(--border);padding:8px;\">3</td><td style=\"border:1px solid var(--border);padding:8px;\">Не провоцирует кариес, обладает пробиотическими свойствами</td></tr></table><p>Ранее профессор выделил <a href=\"https://www.gastronom.ru/news/professor-vydelil-produkty-kotorye-ukreplyayut-immunitet-luchshe-dobavok-1026997\">продукты, которые укрепляют иммунитет лучше добавок</a>.</p>",
                    ImageFileName = "news3.png",
                    CreatedAt = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 4,
                    Title = "История грузинской кухни",
                    Summary = "Читайте о традициях и вкусах грузинской кухни.",
                    ContentHtml = "<p>Грузинская кухня славится балансом специй, свежей зелени и медленного приготовления. Хачапури, хинкали, пхали и сациви стали визитной карточкой региона и давно вышли за его пределы.</p><p>В основе подхода — сезонные продукты, яркие соусы и уважение к семейным традициям.</p>",
                    ImageFileName = "news4.jpg",
                    CreatedAt = new DateTime(2026, 2, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 5,
                    Title = "Средиземноморская диета и здоровье сердца",
                    Summary = "Кардиологи подтверждают пользу рациона с овощами, рыбой и оливковым маслом.",
                    ContentHtml = "<p>Систематические обзоры показывают, что средиземноморская модель питания связана с более низким риском сердечно-сосудистых событий. В рационе преобладают овощи, бобовые, цельные злаки, рыба и оливковое масло.</p><p>Эксперты подчеркивают, что ключевой эффект достигается не отдельным продуктом, а устойчивым режимом питания и умеренной физической активностью.</p>",
                    ImageFileName = "news1.png",
                    CreatedAt = new DateTime(2026, 3, 11, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 6,
                    Title = "Ферментированные продукты и микробиота",
                    Summary = "Йогурт, кефир и квашеные овощи могут поддерживать разнообразие кишечной микрофлоры.",
                    ContentHtml = "<p>Диетологи отмечают, что регулярное употребление ферментированных продуктов способно положительно влиять на микробиоту кишечника. Наиболее часто исследуются кефир, натуральный йогурт, квашеная капуста и кимчи.</p><p>При этом специалисты напоминают о важности состава: продукты с избытком соли или сахара могут нивелировать потенциальную пользу.</p>",
                    ImageFileName = "news2.png",
                    CreatedAt = new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 7,
                    Title = "Почему важно есть больше бобовых",
                    Summary = "Фасоль, чечевица и нут помогают добирать белок и пищевые волокна.",
                    ContentHtml = "<p>Бобовые остаются одним из самых доступных источников растительного белка и клетчатки. По данным профильных рекомендаций по питанию, их регулярное включение в меню может улучшать липидный профиль и насыщаемость.</p><p>Шеф-повара советуют начинать с простых блюд: чечевичного супа, хумуса и салатов с фасолью.</p>",
                    ImageFileName = "news3.png",
                    CreatedAt = new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 8,
                    Title = "Цельнозерновые продукты против скачков сахара",
                    Summary = "Замена рафинированной муки на цельнозерновую снижает гликемическую нагрузку рациона.",
                    ContentHtml = "<p>Нутрициологи рекомендуют чаще выбирать цельнозерновой хлеб, крупы и макароны из твердых сортов пшеницы. Такие продукты содержат больше пищевых волокон и медленнее повышают уровень глюкозы в крови.</p><p>Для постепенного перехода достаточно заменить хотя бы половину привычных гарниров на цельнозерновые аналоги.</p>",
                    ImageFileName = "news4.jpg",
                    CreatedAt = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 9,
                    Title = "Как безопасно готовить рыбу дома",
                    Summary = "Температурный контроль и правильное хранение снижают риск пищевых инфекций.",
                    ContentHtml = "<p>Санитарные рекомендации напоминают: сырую рыбу нужно хранить отдельно от готовых продуктов и использовать отдельные разделочные поверхности. Термическая обработка до полной готовности значительно снижает микробиологические риски.</p><p>Также важно не размораживать рыбу при комнатной температуре: лучше делать это в холодильнике.</p>",
                    ImageFileName = "news1.png",
                    CreatedAt = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new News
                {
                    Id = 10,
                    Title = "Сезонные овощи весной: что выбирать",
                    Summary = "Диетологи составили список доступных овощей для сбалансированного меню весной.",
                    ContentHtml = "<p>Весной специалисты советуют делать акцент на капусте, моркови, свекле, зелени и замороженных овощных смесях высокого качества. Такой набор помогает закрыть потребность в клетчатке и микронутриентах.</p><p>Для практичного меню подойдут овощные супы, запеканки и теплые салаты с бобовыми.</p>",
                    ImageFileName = "news2.png",
                    CreatedAt = new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            modelBuilder.Entity<News>().HasData(news);
        }
    }
}
