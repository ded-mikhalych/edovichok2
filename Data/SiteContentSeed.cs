using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data;

public static class SiteContentSeed
{
    public static void Sync(ApplicationDbContext dbContext)
    {
        SyncCategories(dbContext);
        SyncRestaurantReviews(dbContext);
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

    private static void SyncRestaurantReviews(ApplicationDbContext dbContext)
    {
        var reviews = new[]
        {
            new News
            {
                Id = 1,
                Title = "Basil Market: спокойный ресторан на каждый день",
                Summary = "Проверили новое городское бистро: сильные завтраки, понятное меню и ровный сервис без претензии на роскошь.",
                ContentHtml = "<p><strong>Basil Market</strong> работает в формате городского бистро: открытая кухня, короткое меню и акцент на сезонные продукты. Интерьер спокойный, без лишней декоративности, поэтому место легко воспринимается и для быстрого ужина, и для более длинной встречи.</p><p>Лучше всего здесь получаются завтраки и простые горячие блюда. Омлет с зеленью, картофель с травами и куриные котлеты подаются без перегруза соусами и спецэффектами, зато с хорошей базовой техникой. Десертная витрина небольшая, но аккуратная.</p><p>Сервис вежливый и собранный: официанты знают меню и не пытаются навязывать лишние позиции. Это тот случай, когда ресторан выигрывает не вау-эффектом, а устойчивым качеством.</p>",
                ImageFileName = "news1.png",
                CreatedAt = new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 2,
                Title = "Nord Bread: честный обзор пекарни с полноценной кухней",
                Summary = "Место, где пекарня постепенно стала рестораном: удачная выпечка, хороший суп дня и мягкая посадка для долгих разговоров.",
                ContentHtml = "<p><strong>Nord Bread</strong> производит сильное первое впечатление витриной с хлебом и слойкой, но интереснее всего здесь работает именно кухня. На обеде хорошо раскрываются крем-супы, сезонные салаты и простые горячие блюда из духовки.</p><p>Зал компактный, освещение мягкое, посадка удобная даже в часы пик. Команда держит хороший темп и не проваливается по выдаче, хотя самые популярные позиции к вечеру могут заканчиваться.</p><p>Если идти сюда целенаправленно, лучше брать сочетание из хлебной корзины, супа и основного блюда. Формат не про сложную гастрономию, а про тёплый городской ритм и понятную еду.</p>",
                ImageFileName = "news2.png",
                CreatedAt = new DateTime(2026, 3, 19, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 3,
                Title = "Saffron Yard: ресторан, который держится на специях и темпе",
                Summary = "Плотный зал, яркие вкусы и меню без лишней осторожности. Не самый тихий вариант, но один из самых выразительных.",
                ContentHtml = "<p><strong>Saffron Yard</strong> работает громко и уверенно: насыщенные специи, открытая подача и заметный темп обслуживания. Место не пытается всем понравиться, и именно поэтому кажется живым.</p><p>Лучшие позиции здесь связаны с жареными и запечёнными блюдами: ароматный рис, овощи из печи, пряные закуски. Менее удачны десерты, они выглядят спокойнее и уступают основной линии меню по характеру.</p><p>Это ресторан для тех случаев, когда хочется не нейтрального фона, а выраженного вкуса и энергии в зале. Для спокойного семейного ужина он подходит меньше, чем для компании или короткой эмоциональной встречи.</p>",
                ImageFileName = "news3.png",
                CreatedAt = new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new News
            {
                Id = 4,
                Title = "Garden Room: тихое место с сильными овощными блюдами",
                Summary = "Ресторан без шума и показной модности, где лучше всего удаются запеканки, тёплые салаты и сезонные овощи.",
                ContentHtml = "<p><strong>Garden Room</strong> понравится тем, кто ищет спокойную атмосферу и предсказуемо хороший ужин. Здесь нет агрессивной музыки и избыточной подачи, зато есть аккуратная работа с овощами, сливочными соусами и простыми десертами.</p><p>Особенно хорошо звучат блюда из духовки: тыквенная запеканка, картофель с травами, овощные формы для запекания. Порции средние, но собраны грамотно, без ощущения случайного конструктора на тарелке.</p><p>Это не ресторан-праздник, а ресторан-привычка. И в этом его главное достоинство: сюда хочется возвращаться не ради галочки, а ради устойчивого, спокойного качества.</p>",
                ImageFileName = "news4.png",
                CreatedAt = new DateTime(2026, 3, 17, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        var existing = dbContext.News.ToList();
        if (existing.Count > 0)
        {
            dbContext.News.RemoveRange(existing);
            dbContext.SaveChanges();
        }

        dbContext.News.AddRange(reviews);
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

        var removable = dbContext.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Where(r => r.Id <= 10 && recipes.All(seed => seed.Id != r.Id))
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
