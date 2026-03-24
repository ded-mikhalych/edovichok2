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
            new Category { Id = 1, Name = "first-course", DisplayName = "Первое" },
            new Category { Id = 2, Name = "second-course", DisplayName = "Второе" },
            new Category { Id = 3, Name = "pastry", DisplayName = "Выпечка" },
            new Category { Id = 4, Name = "drinks", DisplayName = "Напитки" }
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
            new SeedRecipe(1, "Тыквенная запеканка с фаршем", "pumpkin-casserole-with-mince", "Сытная запеканка с тыквой, фаршем и мягкой сырной шапкой: тёплое блюдо для полноценного ужина из одной формы.", "Второе", 2, 2, "tykva.png", 65, new DateTime(2026, 3, 12, 0, 0, 0, DateTimeKind.Utc), 1001, 2001, new[] { "Тыква — 600 г", "Говяжий фарш — 450 г", "Лук — 1 шт.", "Сметана — 120 г", "Твёрдый сыр — 120 г", "Чеснок — 2 зубчика", "Соль и перец — по вкусу" }, new[] { "Нарежьте тыкву тонкими ломтиками и слегка запеките до полуготовности.", "Обжарьте фарш с луком и чесноком, затем приправьте его солью и перцем.", "Выложите в форму слоями тыкву и фарш, сверху распределите сметану и сыр.", "Запекайте до мягкости тыквы и румяной корочки, затем дайте блюду постоять 5 минут." }),
            new SeedRecipe(2, "Молодой картофель в духовке", "young-potatoes-in-oven", "Запечённый молодой картофель с чесноком и травами: румяные дольки с хрустящими краями и мягкой серединой.", "Второе", 1, 2, "kartoha.png", 40, new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc), 1101, 2101, new[] { "Молодой картофель — 1 кг", "Оливковое масло — 3 ст. л.", "Чеснок — 3 зубчика", "Розмарин — 2 веточки", "Паприка — 1 ч. л.", "Соль — по вкусу" }, new[] { "Тщательно вымойте картофель, обсушите и разрежьте крупные клубни пополам.", "Смешайте картофель с маслом, чесноком, паприкой, солью и листочками розмарина.", "Разложите картофель по противню в один слой и запекайте до золотистого цвета.", "Подавайте горячим, когда корочка станет румяной, а середина мягкой." }),
            new SeedRecipe(3, "Домашние куриные котлеты", "chicken-cutlets", "Сочные котлеты с курицей: понятный состав, мягкая текстура и привычный вкус без лишней возни у плиты.", "Второе", 2, 2, "https://commons.wikimedia.org/wiki/Special:FilePath/Chicken_cutlets_10.jpg", 45, new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc), 1201, 2201, new[] { "Куриный фарш — 700 г", "Лук — 1 шт.", "Белый хлеб — 2 ломтика", "Молоко — 80 мл", "Яйцо — 1 шт.", "Соль и перец — по вкусу" }, new[] { "Замочите хлеб в молоке и мелко нарежьте лук.", "Смешайте фарш, хлеб, лук, яйцо, соль и перец.", "Сформируйте котлеты одинакового размера.", "Обжарьте котлеты с двух сторон и доведите до готовности под крышкой." }),
            new SeedRecipe(4, "Пирог с вареньем", "jam-pie", "Пирог с густым вареньем: рассыпчатое тесто и знакомый ягодный вкус для спокойного чаепития.", "Выпечка", 1, 3, "https://commons.wikimedia.org/wiki/Special:FilePath/My_first_jam_tart.jpg", 50, new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc), 1301, 2301, new[] { "Мука — 320 г", "Сливочное масло — 180 г", "Сахар — 120 г", "Яйцо — 1 шт.", "Разрыхлитель — 1 ч. л.", "Густое варенье — 200 г" }, new[] { "Разотрите масло с сахаром и добавьте яйцо.", "Введите муку с разрыхлителем и быстро соберите мягкое тесто.", "Большую часть теста распределите по форме, сверху выложите варенье.", "Натрите оставшееся тесто сверху и выпекайте до румяного цвета." }),
            new SeedRecipe(5, "Тёплый салат с овощами и фетой", "warm-vegetable-salad", "Лёгкое блюдо с овощами и фетой: запечённые перцы, кабачок и солоноватый сыр собираются в быстрый тёплый ужин.", "Второе", 1, 2, "https://commons.wikimedia.org/wiki/Special:FilePath/Greek_feta_salad.jpg", 25, new DateTime(2026, 3, 16, 0, 0, 0, DateTimeKind.Utc), 1401, 2401, new[] { "Кабачок — 1 шт.", "Болгарский перец — 2 шт.", "Помидоры черри — 200 г", "Фета — 150 г", "Оливковое масло — 2 ст. л.", "Лимонный сок — 1 ст. л." }, new[] { "Нарежьте овощи крупно и запеките до мягкости.", "Переложите овощи в большую миску и слегка остудите.", "Добавьте фету, масло и лимонный сок.", "Перемешайте салат аккуратно, чтобы сохранить текстуру овощей." }),
            new SeedRecipe(6, "Овощной плов", "vegetable-plov", "Сытное блюдо на основе риса, моркови и специй: без мяса, но с плотной текстурой и ярким ароматом зиры.", "Второе", 2, 2, "https://commons.wikimedia.org/wiki/Special:FilePath/Rice_pilaf.jpg", 60, new DateTime(2026, 3, 17, 0, 0, 0, DateTimeKind.Utc), 1501, 2501, new[] { "Рис — 350 г", "Морковь — 2 шт.", "Лук — 1 шт.", "Чеснок — 1 головка", "Зира — 1 ч. л.", "Растительное масло — 60 мл", "Соль — по вкусу" }, new[] { "Промойте рис, а морковь и лук нарежьте соломкой и полукольцами.", "Обжарьте лук и морковь в казане до мягкости.", "Добавьте специи, рис и влейте горячую воду на палец выше уровня крупы.", "Томите плов под крышкой до готовности риса, затем дайте ему отдохнуть 10 минут." }),
            new SeedRecipe(7, "Куриный суп с вермишелью", "chicken-noodle-soup", "Суп с лёгким бульоном, курицей и вермишелью: спокойный вариант на каждый день без тяжёлой подачи.", "Первое", 1, 1, "https://commons.wikimedia.org/wiki/Special:FilePath/Chicken_noodle_soup_%281%29.jpg", 50, new DateTime(2026, 3, 18, 0, 0, 0, DateTimeKind.Utc), 1601, 2601, new[] { "Куриное филе — 400 г", "Вода — 2 л", "Картофель — 3 шт.", "Морковь — 1 шт.", "Лук — 1 шт.", "Вермишель — 80 г", "Укроп — 1/2 пучка", "Соль — по вкусу" }, new[] { "Отварите куриное филе до мягкости и снимите пену, чтобы бульон остался прозрачным.", "Добавьте нарезанный картофель, лук и морковь, затем варите до мягкости овощей.", "Всыпьте вермишель и готовьте ещё несколько минут, пока она не станет мягкой.", "Верните разобранную курицу в кастрюлю, посолите суп и добавьте укроп перед подачей." }),
            new SeedRecipe(8, "Томатный суп с рисом", "tomato-rice-soup", "Суп с томатной основой, рисом и мягкой овощной сладостью: лёгкий вариант для будничного обеда.", "Первое", 1, 1, "https://commons.wikimedia.org/wiki/Special:FilePath/Tomato_soup.jpg", 40, new DateTime(2026, 3, 19, 0, 0, 0, DateTimeKind.Utc), 1701, 2701, new[] { "Томатный сок — 1 л", "Рис — 80 г", "Лук — 1 шт.", "Морковь — 1 шт.", "Чеснок — 2 зубчика", "Растительное масло — 2 ст. л.", "Соль и перец — по вкусу" }, new[] { "Промойте рис и поставьте его вариться почти до готовности в небольшой части воды.", "Обжарьте лук, морковь и чеснок до мягкости в кастрюле с толстым дном.", "Влейте томатный сок, добавьте рис и доведите суп до лёгкого кипения.", "Посолите, поперчите и дайте супу настояться несколько минут перед подачей." }),
            new SeedRecipe(9, "Ягодный морс", "berry-mors", "Освежающий морс с насыщенным ягодным вкусом и лёгкой кислинкой, который хорошо работает и тёплым, и охлаждённым.", "Напитки", 1, 4, "https://commons.wikimedia.org/wiki/Special:FilePath/Black_currant_juice.jpg", 20, new DateTime(2026, 3, 20, 0, 0, 0, DateTimeKind.Utc), 1801, 2801, new[] { "Замороженные ягоды — 300 г", "Вода — 1.5 л", "Сахар — 80 г", "Мёд — 1 ст. л." }, new[] { "Разомните ягоды и отожмите сок через сито, уберите его пока в холодильник.", "Жмых залейте водой, доведите до кипения и проварите 5 минут.", "Процедите отвар, добавьте сахар и остудите до тёплого состояния.", "Смешайте отвар с ягодным соком и мёдом, затем подавайте морс охлаждённым." }),
            new SeedRecipe(10, "Домашний лимонад с мятой", "mint-lemonade", "Лёгкий лимонад с лимоном, мятой и мягкой сладостью: быстрый освежающий напиток для жаркого дня.", "Напитки", 1, 4, "https://commons.wikimedia.org/wiki/Special:FilePath/Mint_lemonade.jpg", 15, new DateTime(2026, 3, 21, 0, 0, 0, DateTimeKind.Utc), 1901, 2901, new[] { "Лимон — 2 шт.", "Вода — 1 л", "Сахар — 60 г", "Мята — 4 веточки", "Лёд — по желанию" }, new[] { "Снимите с одного лимона немного цедры и выжмите сок из обоих лимонов.", "Смешайте сок, цедру, сахар и несколько ложек тёплой воды, чтобы сахар быстрее растворился.", "Добавьте холодную воду и слегка разомните в напитке листья мяты.", "Остудите лимонад и подавайте со льдом, если нужен более освежающий вкус." })
        };

        var seedIds = recipes.Select(r => r.Id).ToArray();

        var currentNames = recipes.Select(r => r.Name).ToArray();
        var currentSlugs = recipes.Select(r => r.Slug).ToArray();
        var legacyRemovedNames = new[]
        {
            "Тыквенная запеканка",
            "Молодой картофель с укропом"
        };
        var legacyRemovedSlugs = new[]
        {
            "pumpkin-casserole",
            "young-potatoes"
        };

        var duplicatesToRemove = dbContext.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Where(r =>
                (!seedIds.Contains(r.Id) && currentNames.Contains(r.Name!)) ||
                (!seedIds.Contains(r.Id) && currentSlugs.Contains(r.Slug!)) ||
                legacyRemovedNames.Contains(r.Name!) ||
                legacyRemovedSlugs.Contains(r.Slug!))
            .ToList();

        if (duplicatesToRemove.Count > 0)
        {
            dbContext.Recipes.RemoveRange(duplicatesToRemove);
            dbContext.SaveChanges();
        }

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
