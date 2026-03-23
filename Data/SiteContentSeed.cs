using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data;

public static class SiteContentSeed
{
    public static void Sync(ApplicationDbContext dbContext)
    {
        SyncCategories(dbContext);
        SyncNewsArticles(dbContext);
        SyncCoreRecipes(dbContext);
    }

    private static void SyncCategories(ApplicationDbContext dbContext)
    {
        var categories = new[]
        {
            new Category { Id = 1, Name = "Russian", DisplayName = "Домашняя кухня" },
            new Category { Id = 2, Name = "European", DisplayName = "Европейская кухня" },
            new Category { Id = 3, Name = "Asian", DisplayName = "Азиатская кухня" }
        };

        foreach (var seed in categories)
        {
            var existing = dbContext.Categories.Find(seed.Id);
            if (existing == null)
            {
                dbContext.Categories.Add(seed);
                continue;
            }

            existing.Name = seed.Name;
            existing.DisplayName = seed.DisplayName;
        }

        dbContext.SaveChanges();
    }

    private static void SyncNewsArticles(ApplicationDbContext dbContext)
    {
        var articles = new[]
        {
            new News
            {
                Id = 1,
                Title = "Налог на чипсы и газировку",
                Summary = "В России обсуждают налог на вредные продукты и сладкие напитки как часть мер по оздоровлению рациона.",
                ContentHtml = "<p>В России обсуждается инициатива по введению дополнительного налога на отдельные категории вредных продуктов, включая чипсы и сладкую газировку. Сторонники идеи считают, что такой шаг поможет сократить потребление ультрапереработанной еды и подтолкнёт производителей к пересмотру составов.</p><p>Эксперты подчеркивают, что подобные меры работают лучше в сочетании с просветительскими программами и более понятной маркировкой на упаковке. Пока инициатива находится на стадии обсуждения, но тема уже вызвала широкий общественный интерес.</p>",
                ImageFileName = "news1.png",
                CreatedAt = new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 2,
                Title = "Фрукт против похмелья",
                Summary = "Исследователи рассказали о фрукте, который может облегчить симптомы похмелья и ускорить восстановление.",
                ContentHtml = "<p>Учёные сообщили о фрукте, содержащем вещества, которые могут смягчать симптомы похмелья и помогать организму быстрее восстановиться после алкоголя. Особенно заметный эффект связывают не с самим фруктом по отдельности, а с его сочетанием с водой, сном и нормальным питанием.</p><p>Специалисты напоминают, что универсального средства не существует, а лучший способ избежать тяжёлого состояния наутро по-прежнему связан с умеренностью и достаточной гидратацией.</p>",
                ImageFileName = "news2.png",
                CreatedAt = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 3,
                Title = "Подсластители под угрозой",
                Summary = "Исследователи признали безопасным только один вид натурального подсластителя, остальные требуют дополнительного внимания.",
                ContentHtml = "<p>В новом обзоре исследователи проанализировали данные о популярных заменителях сахара и их возможном влиянии на когнитивные функции. Авторы подчёркивают, что часть искусственных подсластителей всё ещё требует более аккуратной оценки и долгосрочных наблюдений.</p><p>На этом фоне специалисты советуют реже полагаться на сверхсладкие продукты, даже если они позиционируются как более лёгкая альтернатива. Более устойчивым решением по-прежнему остаётся постепенное снижение общей сладости рациона.</p>",
                ImageFileName = "news3.png",
                CreatedAt = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 4,
                Title = "История грузинской кухни",
                Summary = "Короткий обзор традиций, вкусов и ключевых блюд грузинской кухни.",
                ContentHtml = "<p>Грузинская кухня строится на выразительных специях, свежей зелени, соусах и уважении к семейной трапезе. Хачапури, хинкали, пхали и сациви давно стали гастрономическими символами региона далеко за его пределами.</p><p>Основа подхода проста: сезонные продукты, яркий вкус и медленное приготовление там, где оно действительно нужно. Именно сочетание доступности и характера сделало грузинскую кухню одной из самых узнаваемых в мире.</p>",
                ImageFileName = "news4.png",
                CreatedAt = new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        var existing = dbContext.News.ToList();
        if (existing.Count > 0)
        {
            dbContext.News.RemoveRange(existing);
            dbContext.SaveChanges();
        }

        dbContext.News.AddRange(articles);
        dbContext.SaveChanges();
    }

    private static void SyncCoreRecipes(ApplicationDbContext dbContext)
    {
        var recipes = new[]
        {
            new SeedRecipe(1, "Тыквенная запеканка", "pumpkin-casserole", "Нежная домашняя запеканка с тыквой, творогом и тёплыми специями.", "Домашняя", 1, 1, "tykva.png", 55, new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc), 1001, 2001, new[] { "Тыква — 500 г", "Творог — 300 г", "Яйца — 2 шт.", "Сахар — 3 ст. л.", "Манная крупа — 3 ст. л.", "Сливочное масло — 20 г", "Корица — 1 ч. л." }, new[] { "Нарежьте тыкву кубиками и запеките до мягкости.", "Смешайте творог, яйца, сахар, манку и корицу в одной миске.", "Добавьте размятую тыкву и перемешайте массу до однородности.", "Переложите смесь в форму, смажьте маслом и запекайте до золотистой корочки." }),
            new SeedRecipe(2, "Молодой картофель с укропом", "young-potatoes", "Простой сезонный рецепт, где всё держится на хорошем картофеле, сливочном масле и свежем укропе.", "Домашняя", 1, 1, "kartoha.png", 30, new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc), 1101, 2101, new[] { "Молодой картофель — 1 кг", "Сливочное масло — 40 г", "Укроп — 1 пучок", "Чеснок — 2 зубчика", "Соль — по вкусу" }, new[] { "Тщательно вымойте картофель щёткой и отварите до мягкости.", "Слейте воду, оставьте картофель в кастрюле на слабом огне на 1 минуту.", "Добавьте сливочное масло, рубленый чеснок и соль, аккуратно перемешайте.", "Перед подачей щедро посыпьте картофель свежим укропом." }),
            new SeedRecipe(3, "Домашние куриные котлеты", "chicken-cutlets", "Сочные котлеты без сложной техники: мягкий фарш, немного лука и спокойный домашний вкус.", "Домашняя", 2, 1, "kotlety.png", 45, new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc), 1201, 2201, new[] { "Куриный фарш — 700 г", "Лук — 1 шт.", "Белый хлеб — 2 ломтика", "Молоко — 80 мл", "Яйцо — 1 шт.", "Соль и перец — по вкусу" }, new[] { "Замочите хлеб в молоке и мелко нарежьте лук.", "Смешайте фарш, хлеб, лук, яйцо, соль и перец.", "Сформируйте котлеты одинакового размера.", "Обжарьте котлеты с двух сторон и доведите до готовности под крышкой." }),
            new SeedRecipe(4, "Пирог с вареньем", "jam-pie", "Небольшой домашний пирог с рассыпчатым тестом и густой ягодной начинкой.", "Домашняя", 1, 2, "jam.png", 50, new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc), 1301, 2301, new[] { "Мука — 320 г", "Сливочное масло — 180 г", "Сахар — 120 г", "Яйцо — 1 шт.", "Разрыхлитель — 1 ч. л.", "Густое варенье — 200 г" }, new[] { "Разотрите масло с сахаром и добавьте яйцо.", "Введите муку с разрыхлителем и быстро соберите мягкое тесто.", "Большую часть теста распределите по форме, сверху выложите варенье.", "Натрите оставшееся тесто сверху и выпекайте до румяного цвета." }),
            new SeedRecipe(5, "Тёплый салат с овощами и фетой", "warm-vegetable-salad", "Запечённые овощи, мягкий солоноватый сыр и лёгкая лимонная заправка.", "Европейская", 1, 2, "greece_salad.png", 25, new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc), 1401, 2401, new[] { "Кабачок — 1 шт.", "Болгарский перец — 2 шт.", "Помидоры черри — 200 г", "Фета — 150 г", "Оливковое масло — 2 ст. л.", "Лимонный сок — 1 ст. л." }, new[] { "Нарежьте овощи крупно и запеките до мягкости.", "Переложите овощи в большую миску и слегка остудите.", "Добавьте фету, масло и лимонный сок.", "Перемешайте салат аккуратно, чтобы сохранить текстуру овощей." }),
            new SeedRecipe(6, "Овощной плов", "vegetable-plov", "Плотный рис с морковью, луком и специями без тяжёлого мясного блока.", "Азиатская", 2, 3, "plov.png", 60, new DateTime(2026, 3, 17, 0, 0, 0, DateTimeKind.Utc), 1501, 2501, new[] { "Рис — 350 г", "Морковь — 2 шт.", "Лук — 1 шт.", "Чеснок — 1 головка", "Зира — 1 ч. л.", "Растительное масло — 60 мл", "Соль — по вкусу" }, new[] { "Промойте рис, а морковь и лук нарежьте соломкой и полукольцами.", "Обжарьте лук и морковь в казане до мягкости.", "Добавьте специи, рис и влейте горячую воду на палец выше уровня крупы.", "Томите плов под крышкой до готовности риса, затем дайте ему отдохнуть 10 минут." })
        };

        var seedIds = recipes.Select(r => r.Id).ToArray();

        var removable = dbContext.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Where(r => r.Id <= 10 && !seedIds.Contains(r.Id))
            .ToList();

        if (removable.Count > 0)
        {
            dbContext.Recipes.RemoveRange(removable);
            dbContext.SaveChanges();
        }

        foreach (var seed in recipes)
        {
            var recipe = dbContext.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefault(r => r.Id == seed.Id);

            if (recipe == null)
            {
                recipe = new Recipe { Id = seed.Id };
                dbContext.Recipes.Add(recipe);
            }
            else
            {
                if (recipe.Ingredients.Count > 0)
                {
                    dbContext.RecipeIngredients.RemoveRange(recipe.Ingredients);
                }

                if (recipe.Steps.Count > 0)
                {
                    dbContext.RecipeSteps.RemoveRange(recipe.Steps);
                }

                dbContext.SaveChanges();
            }

            recipe.Name = seed.Name;
            recipe.Author = "Редакция";
            recipe.Slug = seed.Slug;
            recipe.Description = seed.Description;
            recipe.Cuisine = seed.Cuisine;
            recipe.Difficulty = seed.Difficulty;
            recipe.ImageFileName = seed.ImageFileName;
            recipe.IsFavorite = false;
            recipe.CookingTime = seed.CookingTime;
            recipe.CategoryId = seed.CategoryId;
            recipe.CreatedAt = seed.CreatedAt;
            recipe.RatingSum = 0;
            recipe.RatingCount = 0;

            dbContext.SaveChanges();

            dbContext.RecipeIngredients.AddRange(seed.Ingredients.Select((text, index) => new RecipeIngredient
            {
                Id = seed.IngredientIdStart + index,
                RecipeId = recipe.Id,
                DisplayText = text,
                SortOrder = index + 1
            }));

            dbContext.RecipeSteps.AddRange(seed.Steps.Select((text, index) => new RecipeStep
            {
                Id = seed.StepIdStart + index,
                RecipeId = recipe.Id,
                StepNumber = index + 1,
                Description = text,
                ImagePath = string.Empty
            }));

            dbContext.SaveChanges();
        }
    }

    private sealed record SeedRecipe(
        int Id,
        string Name,
        string Slug,
        string Description,
        string Cuisine,
        int Difficulty,
        int CategoryId,
        string ImageFileName,
        int CookingTime,
        DateTime CreatedAt,
        int IngredientIdStart,
        int StepIdStart,
        string[] Ingredients,
        string[] Steps);
}
