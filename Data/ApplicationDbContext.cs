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
                    Slug = "syrniki",
                    Description = "Традиционный русский завтрак из творога",
                    Cuisine = "Русская",
                    Difficulty = 1,
                    CategoryId = 1,
                    ImageFileName = "syrniki.png",
                    StepImagesFolder = "syrniki",
                    IngredientsText = "Творог — 300 г\nЯйцо — 1 шт.\nСахар — 2 ст.л.\nМука — 4 ст.л.\nЩепотка соли\nМасло для жарки",
                    StepsText = "Смешайте творог, яйцо, сахар и соль до однородности.\nДобавьте муку и замесите мягкое тесто.\nСформируйте небольшие сырники и обваляйте в муке.\nОбжарьте на среднем огне с двух сторон до золотистой корочки.\nПодавайте горячими со сметаной или ягодным соусом.",
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
                    StepImagesFolder = "kharcho",
                    IngredientsText = "Вода — 2.5 л\nГовядина — 700 г\nПомидоры — 400 г\nКартофель — 300 г\nРис — 100 г\nЛук — 150 г\nЧеснок — 3 зубчика\nПетрушка — 20 г\nХмели-сунели — 1.5 ч.л.\nСоль и перец — по вкусу",
                    StepsText = "Подготовьте ингредиенты и нарежьте мясо кусочками.\nСварите мясной бульон на медленном огне, снимая пену.\nПромойте и замочите рис на 20 минут.\nНарежьте овощи, помидоры очистите от кожицы.\nДобавьте в бульон рис, картофель и часть помидоров.\nОбжарьте лук, добавьте оставшиеся помидоры и потушите.\nПереложите зажарку в суп, добавьте специи, чеснок и зелень.\nДайте супу настояться 10 минут и подавайте горячим.",
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
                    StepImagesFolder = "mushrooms",
                    IngredientsText = "Опята — 1.2 кг\nСоль — 80 г\nПерец горошком — 8-10 шт.\nЛавровый лист — 3 шт.\nЧеснок — 6 зубчиков\nВода — 1 л",
                    StepsText = "Подготовьте грибы и специи, промойте опята.\nУдалите корешки и тщательно переберите грибы.\nОтварите опята до готовности и остудите.\nПриготовьте рассол с солью и специями.\nДобавьте грибы в рассол и проварите 25 минут.\nПростерилизуйте банки и крышки.\nРазложите грибы по банкам и закатайте.\nВыдержите заготовку 4-6 недель в прохладном месте.",
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
                    StepImagesFolder = "zazharka",
                    IngredientsText = "Морковь — 200 г\nСвекла — 300 г\nЛук — 150 г\nЧеснок — 3 зубчика\nТоматный сок — 700 мл\nСоль — 1-1.5 ч.л.\nСахар — 1 ч.л.\nМасло — 2 ст.л.",
                    StepsText = "Подготовьте ингредиенты и вымойте овощи.\nНарежьте лук и обжарьте до мягкости.\nДобавьте свеклу и морковь, тушите на среднем огне.\nВлейте томатный сок, добавьте соль и сахар.\nТушите 40-50 минут до нужной густоты.\nДобавьте чеснок и специи за 10 минут до конца.\nРазложите заготовку по стерильным банкам и закатайте.",
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
                    StepImagesFolder = "olivie",
                    IngredientsText = "Варёная колбаса — 300 г\nЗелёный горошек — 200 г\nЯйца — 4 шт.\nКартошка — 600 г\nСоленые огурцы — 200 г\nМорковь — 150 г\nМайонез — 150-200 г\nСоль — по вкусу",
                    StepsText = "Подготовьте и отварите картофель, морковь и яйца.\nСлейте жидкость из банки с горошком.\nНарежьте колбасу кубиками.\nНарежьте картофель, морковь, яйца и огурцы одинаково.\nСмешайте все ингредиенты в большой миске.\nДобавьте майонез и соль по вкусу.\nОхладите салат в холодильнике перед подачей.",
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
                    StepImagesFolder = "greece_salad",
                    IngredientsText = "Помидоры — 250 г\nОгурцы — 200 г\nФета — 150 г\nМаслины — 80 г\nКрасный лук — 1/2 шт.\nОливковое масло — 2 ст.л.\nЛимонный сок — 1 ст.л.",
                    StepsText = "Подготовьте овощи и зелень.\nНарежьте огурцы и помидоры крупными кусочками.\nДобавьте тонкие полукольца лука и маслины.\nПоложите кубики феты поверх салата.\nЗаправьте маслом и лимонным соком, аккуратно перемешайте.",
                    IsFavorite = false,
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
