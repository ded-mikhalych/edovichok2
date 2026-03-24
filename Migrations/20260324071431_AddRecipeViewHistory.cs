using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeViewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    ContentHtml = table.Column<string>(type: "text", nullable: true),
                    ImageFileName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Slug = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Cuisine = table.Column<string>(type: "text", nullable: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    ImageFileName = table.Column<string>(type: "text", nullable: true),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    CookingTime = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RatingSum = table.Column<int>(type: "integer", nullable: false),
                    RatingCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    DisplayText = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    StepNumber = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImagePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeSteps_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeViewHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientKey = table.Column<string>(type: "text", nullable: false),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeViewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeViewHistories_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Name" },
                values: new object[,]
                {
                    { 1, "Первое", "first-course" },
                    { 2, "Второе", "second-course" },
                    { 3, "Выпечка", "pastry" },
                    { 4, "Напитки", "drinks" }
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "ContentHtml", "CreatedAt", "ImageFileName", "Summary", "Title" },
                values: new object[,]
                {
                    { 1, "<p>В России обсуждается инициатива по введению налога на отдельные категории вредных продуктов. Эксперты считают, что мера может снизить потребление ультрапереработанной еды и стимулировать производителей пересматривать состав продукции.</p><p>Пока документ находится на стадии обсуждения, но тема уже вызвала широкий общественный резонанс.</p>", new DateTime(2026, 3, 7, 0, 0, 0, 0, DateTimeKind.Utc), "news1.png", "В России предлагают ввести налог на вредные продукты питания.", "Налог на чипсы и газировку" },
                    { 2, "<p>Исследователи сообщили о фрукте, содержащем вещества, которые могут облегчить симптомы похмелья и ускорить восстановление после употребления алкоголя. В публикации подчеркивается, что наибольший эффект достигается в сочетании с полноценной гидратацией и сном.</p>", new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "news2.png", "Учёные нашли фрукт, который помогает быстрее восстановиться.", "Фрукт против похмелья" },
                    { 3, "<p>Как <a href=\"https://www.neurology.org/doi/10.1212/WNL.0000000000214023\">заявлено</a> в статье для журнала Neurology, группа бразильских специалистов в течение 8 лет изучала сведения о питании и здоровье 12 тыс. человек, средний возраст которых составлял 50 лет. Добровольцы регулярно сообщали, сколько продуктов и напитков с подсластителями они употребляют.</p><p><strong>Как проводилось исследование</strong></p><p>Чтобы оценить состояние нервной системы, испытуемым давали интеллектуальные упражнения на память, владение речью, концентрацию и быстроту мыслительных процессов.</p><p>Ученые установили, что регулярное включение искусственных подсластителей в рацион существенно отражается на работе мозга. В частности, интеллектуальные способности снижаются быстрее на 60%.</p><p><strong>Результаты</strong></p><p>К провоцирующим факторам отнесли злоупотребление популярными сахарозаменителями, в том числе:</p><ul><li>аспартамом;</li><li>сахарином;</li><li>эритритом;</li><li>сорбитом;</li><li>ксилитом.</li></ul><p>Исключением из правил стала только тагатоза — ее связь с ухудшением когнитивных функций обнаружена не была. Этот редкий натуральный подсластитель содержится в ягодах, фруктах, некоторых овощах, какао-бобах, а также в молочных продуктах.</p><table style=\"border-collapse:collapse;width:100%;max-width:720px;margin:16px 0;background:#fff;\"><tr><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Происхождение</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Калорийность</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Гликемический индекс</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Особенности</th></tr><tr><td style=\"border:1px solid var(--border);padding:8px;\">натуральное</td><td style=\"border:1px solid var(--border);padding:8px;\">1,5 ккал/г</td><td style=\"border:1px solid var(--border);padding:8px;\">3</td><td style=\"border:1px solid var(--border);padding:8px;\">Не провоцирует кариес, обладает пробиотическими свойствами</td></tr></table><p>Ранее профессор выделил <a href=\"https://www.gastronom.ru/news/professor-vydelil-produkty-kotorye-ukreplyayut-immunitet-luchshe-dobavok-1026997\">продукты, которые укрепляют иммунитет лучше добавок</a>.</p>", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "news3.png", "Исследователи признали безопасным только один вид подсластителя.", "Подсластители под угрозой" },
                    { 4, "<p>Грузинская кухня славится балансом специй, свежей зелени и медленного приготовления. Хачапури, хинкали, пхали и сациви стали визитной карточкой региона и давно вышли за его пределы.</p><p>В основе подхода — сезонные продукты, яркие соусы и уважение к семейным традициям.</p>", new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), "news4.jpg", "Читайте о традициях и вкусах грузинской кухни.", "История грузинской кухни" },
                    { 5, "<p>Систематические обзоры показывают, что средиземноморская модель питания связана с более низким риском сердечно-сосудистых событий. В рационе преобладают овощи, бобовые, цельные злаки, рыба и оливковое масло.</p><p>Эксперты подчеркивают, что ключевой эффект достигается не отдельным продуктом, а устойчивым режимом питания и умеренной физической активностью.</p>", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "news1.png", "Кардиологи подтверждают пользу рациона с овощами, рыбой и оливковым маслом.", "Средиземноморская диета и здоровье сердца" },
                    { 6, "<p>Диетологи отмечают, что регулярное употребление ферментированных продуктов способно положительно влиять на микробиоту кишечника. Наиболее часто исследуются кефир, натуральный йогурт, квашеная капуста и кимчи.</p><p>При этом специалисты напоминают о важности состава: продукты с избытком соли или сахара могут нивелировать потенциальную пользу.</p>", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "news2.png", "Йогурт, кефир и квашеные овощи могут поддерживать разнообразие кишечной микрофлоры.", "Ферментированные продукты и микробиота" },
                    { 7, "<p>Бобовые остаются одним из самых доступных источников растительного белка и клетчатки. По данным профильных рекомендаций по питанию, их регулярное включение в меню может улучшать липидный профиль и насыщаемость.</p><p>Шеф-повара советуют начинать с простых блюд: чечевичного супа, хумуса и салатов с фасолью.</p>", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc), "news3.png", "Фасоль, чечевица и нут помогают добирать белок и пищевые волокна.", "Почему важно есть больше бобовых" },
                    { 8, "<p>Нутрициологи рекомендуют чаще выбирать цельнозерновой хлеб, крупы и макароны из твердых сортов пшеницы. Такие продукты содержат больше пищевых волокон и медленнее повышают уровень глюкозы в крови.</p><p>Для постепенного перехода достаточно заменить хотя бы половину привычных гарниров на цельнозерновые аналоги.</p>", new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "news4.jpg", "Замена рафинированной муки на цельнозерновую снижает гликемическую нагрузку рациона.", "Цельнозерновые продукты против скачков сахара" },
                    { 9, "<p>Санитарные рекомендации напоминают: сырую рыбу нужно хранить отдельно от готовых продуктов и использовать отдельные разделочные поверхности. Термическая обработка до полной готовности значительно снижает микробиологические риски.</p><p>Также важно не размораживать рыбу при комнатной температуре: лучше делать это в холодильнике.</p>", new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "news1.png", "Температурный контроль и правильное хранение снижают риск пищевых инфекций.", "Как безопасно готовить рыбу дома" },
                    { 10, "<p>Весной специалисты советуют делать акцент на капусте, моркови, свекле, зелени и замороженных овощных смесях высокого качества. Такой набор помогает закрыть потребность в клетчатке и микронутриентах.</p><p>Для практичного меню подойдут овощные супы, запеканки и теплые салаты с бобовыми.</p>", new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Utc), "news2.png", "Диетологи составили список доступных овощей для сбалансированного меню весной.", "Сезонные овощи весной: что выбирать" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Author", "CategoryId", "CookingTime", "CreatedAt", "Cuisine", "Description", "Difficulty", "ImageFileName", "IsFavorite", "Name", "RatingCount", "RatingSum", "Slug" },
                values: new object[,]
                {
                    { 1, null, 1, 20, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Русская", "Традиционный русский завтрак из творога", 1, "syrniki.png", true, "Сырники", 0, 0, "syrniki" },
                    { 2, null, 1, 60, new DateTime(2026, 2, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Грузинская", "Грузинский суп с говядиной и рисом", 2, "kharcho.png", true, "Харчо", 0, 0, "kharcho" },
                    { 3, null, 1, 40, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Домашняя", "Ароматное блюдо из лесных грибов", 2, "mushrooms.png", true, "Грибы в сметане", 0, 0, "mushrooms" },
                    { 4, null, 1, 90, new DateTime(2026, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Домашняя", "Украинский свёкольный суп с говядиной", 2, "borsch.png", true, "Борщ", 0, 0, "borsch" },
                    { 5, null, 1, 30, new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Русская", "Классический русский салат", 1, "olivie.png", true, "Оливье", 0, 0, "olivie" },
                    { 6, null, 2, 15, new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Европейская", "Свежий салат с помидорами, огурцами и фетой", 1, "greece_salad.png", false, "Греческий салат", 0, 0, "greece-salad" },
                    { 7, null, 3, 90, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Узбекская", "Классический плов с бараниной, рисом и морковью в казане", 2, "kharcho.png", false, "Узбекский плов", 0, 0, "plov" },
                    { 8, null, 3, 45, new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Тайская", "Пряный тайский суп на кокосовом молоке с креветками и лаймом", 3, "mushrooms.png", false, "Том ям с креветками", 0, 0, "tom-yum" },
                    { 9, null, 2, 60, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Французская", "Французское овощное рагу из баклажанов, кабачков и томатов", 2, "greece_salad.png", false, "Рататуй", 0, 0, "ratatouille" },
                    { 10, null, 1, 50, new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Русская", "Говядина в сливочно-сметанном соусе с луком и грибами", 2, "olivie.png", false, "Бефстроганов", 0, 0, "beef-stroganoff" }
                });

            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "Id", "DisplayText", "RecipeId", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Творог — 300 г", 1, 1 },
                    { 2, "Яйцо — 1 шт.", 1, 2 },
                    { 3, "Сахар — 2 ст.л.", 1, 3 },
                    { 4, "Мука — 4 ст.л.", 1, 4 },
                    { 5, "Щепотка соли", 1, 5 },
                    { 6, "Масло для жарки", 1, 6 },
                    { 7, "Вода — 2.5 л", 2, 1 },
                    { 8, "Говядина — 700 г", 2, 2 },
                    { 9, "Помидоры — 400 г", 2, 3 },
                    { 10, "Картофель — 300 г", 2, 4 },
                    { 11, "Рис — 100 г", 2, 5 },
                    { 12, "Лук — 150 г", 2, 6 },
                    { 13, "Чеснок — 3 зубчика", 2, 7 },
                    { 14, "Петрушка — 20 г", 2, 8 },
                    { 15, "Хмели-сунели — 1.5 ч.л.", 2, 9 },
                    { 16, "Соль и перец — по вкусу", 2, 10 },
                    { 17, "Опята — 1.2 кг", 3, 1 },
                    { 18, "Соль — 80 г", 3, 2 },
                    { 19, "Перец горошком — 8-10 шт.", 3, 3 },
                    { 20, "Лавровый лист — 3 шт.", 3, 4 },
                    { 21, "Чеснок — 6 зубчиков", 3, 5 },
                    { 22, "Вода — 1 л", 3, 6 },
                    { 23, "Морковь — 200 г", 4, 1 },
                    { 24, "Свекла — 300 г", 4, 2 },
                    { 25, "Лук — 150 г", 4, 3 },
                    { 26, "Чеснок — 3 зубчика", 4, 4 },
                    { 27, "Томатный сок — 700 мл", 4, 5 },
                    { 28, "Соль — 1-1.5 ч.л.", 4, 6 },
                    { 29, "Сахар — 1 ч.л.", 4, 7 },
                    { 30, "Масло — 2 ст.л.", 4, 8 },
                    { 31, "Варёная колбаса — 300 г", 5, 1 },
                    { 32, "Зелёный горошек — 200 г", 5, 2 },
                    { 33, "Яйца — 4 шт.", 5, 3 },
                    { 34, "Картошка — 600 г", 5, 4 },
                    { 35, "Соленые огурцы — 200 г", 5, 5 },
                    { 36, "Морковь — 150 г", 5, 6 },
                    { 37, "Майонез — 150-200 г", 5, 7 },
                    { 38, "Соль — по вкусу", 5, 8 },
                    { 39, "Помидоры — 250 г", 6, 1 },
                    { 40, "Огурцы — 200 г", 6, 2 },
                    { 41, "Фета — 150 г", 6, 3 },
                    { 42, "Маслины — 80 г", 6, 4 },
                    { 43, "Красный лук — 1/2 шт.", 6, 5 },
                    { 44, "Оливковое масло — 2 ст.л.", 6, 6 },
                    { 45, "Лимонный сок — 1 ст.л.", 6, 7 },
                    { 46, "Рис девзира — 700 г", 7, 1 },
                    { 47, "Баранина — 700 г", 7, 2 },
                    { 48, "Морковь — 700 г", 7, 3 },
                    { 49, "Лук — 300 г", 7, 4 },
                    { 50, "Чеснок — 2 головки", 7, 5 },
                    { 51, "Зира — 1.5 ч.л.", 7, 6 },
                    { 52, "Барбарис — 1 ст.л.", 7, 7 },
                    { 53, "Растительное масло — 120 мл", 7, 8 },
                    { 54, "Соль — по вкусу", 7, 9 },
                    { 55, "Креветки — 400 г", 8, 1 },
                    { 56, "Кокосовое молоко — 400 мл", 8, 2 },
                    { 57, "Куриный бульон — 600 мл", 8, 3 },
                    { 58, "Паста том ям — 2 ст.л.", 8, 4 },
                    { 59, "Лемонграсс — 2 стебля", 8, 5 },
                    { 60, "Листья каффир-лайма — 4 шт.", 8, 6 },
                    { 61, "Шампиньоны — 200 г", 8, 7 },
                    { 62, "Рыбный соус — 2 ст.л.", 8, 8 },
                    { 63, "Сок лайма — 2 ст.л.", 8, 9 },
                    { 64, "Баклажан — 1 крупный", 9, 1 },
                    { 65, "Кабачок — 1 крупный", 9, 2 },
                    { 66, "Помидоры — 5 шт.", 9, 3 },
                    { 67, "Болгарский перец — 2 шт.", 9, 4 },
                    { 68, "Лук — 1 шт.", 9, 5 },
                    { 69, "Чеснок — 3 зубчика", 9, 6 },
                    { 70, "Оливковое масло — 3 ст.л.", 9, 7 },
                    { 71, "Тимьян и базилик — по вкусу", 9, 8 },
                    { 72, "Говяжья вырезка — 600 г", 10, 1 },
                    { 73, "Лук — 2 шт.", 10, 2 },
                    { 74, "Шампиньоны — 250 г", 10, 3 },
                    { 75, "Сметана — 250 г", 10, 4 },
                    { 76, "Сливки 20% — 150 мл", 10, 5 },
                    { 77, "Горчица — 1 ч.л.", 10, 6 },
                    { 78, "Мука — 1 ст.л.", 10, 7 },
                    { 79, "Соль и перец — по вкусу", 10, 8 }
                });

            migrationBuilder.InsertData(
                table: "RecipeSteps",
                columns: new[] { "Id", "Description", "ImagePath", "RecipeId", "StepNumber" },
                values: new object[,]
                {
                    { 1, "Смешайте творог, яйцо, сахар и соль до однородности.", "syrniki/step1.jpg", 1, 1 },
                    { 2, "Добавьте муку и замесите мягкое тесто.", "syrniki/step2.jpg", 1, 2 },
                    { 3, "Сформируйте небольшие сырники и обваляйте в муке.", "syrniki/step3.jpg", 1, 3 },
                    { 4, "Обжарьте на среднем огне с двух сторон до золотистой корочки.", "syrniki/step4.jpg", 1, 4 },
                    { 5, "Подавайте горячими со сметаной или ягодным соусом.", "syrniki/step5.jpg", 1, 5 },
                    { 6, "Подготовьте ингредиенты и нарежьте мясо кусочками.", "kharcho/step1.jpg", 2, 1 },
                    { 7, "Сварите мясной бульон на медленном огне, снимая пену.", "kharcho/step2.jpg", 2, 2 },
                    { 8, "Промойте и замочите рис на 20 минут.", "kharcho/step3.jpg", 2, 3 },
                    { 9, "Нарежьте овощи, помидоры очистите от кожицы.", "kharcho/step4.jpg", 2, 4 },
                    { 10, "Добавьте в бульон рис, картофель и часть помидоров.", "kharcho/step5.jpg", 2, 5 },
                    { 11, "Обжарьте лук, добавьте оставшиеся помидоры и потушите.", "kharcho/step6.jpg", 2, 6 },
                    { 12, "Переложите зажарку в суп, добавьте специи, чеснок и зелень.", "kharcho/step7.jpg", 2, 7 },
                    { 13, "Дайте супу настояться 10 минут и подавайте горячим.", "kharcho/step8.jpg", 2, 8 },
                    { 14, "Подготовьте грибы и специи, промойте опята.", "mushrooms/step1.jpg", 3, 1 },
                    { 15, "Удалите корешки и тщательно переберите грибы.", "mushrooms/step2.jpg", 3, 2 },
                    { 16, "Отварите опята до готовности и остудите.", "mushrooms/step3.jpg", 3, 3 },
                    { 17, "Приготовьте рассол с солью и специями.", "mushrooms/step4.jpg", 3, 4 },
                    { 18, "Добавьте грибы в рассол и проварите 25 минут.", "mushrooms/step5.jpg", 3, 5 },
                    { 19, "Простерилизуйте банки и крышки.", "mushrooms/step6.jpg", 3, 6 },
                    { 20, "Разложите грибы по банкам и закатайте.", "mushrooms/step7.jpg", 3, 7 },
                    { 21, "Выдержите заготовку 4-6 недель в прохладном месте.", "mushrooms/step8.jpg", 3, 8 },
                    { 22, "Подготовьте ингредиенты и вымойте овощи.", "zazharka/step1.jpg", 4, 1 },
                    { 23, "Нарежьте лук и обжарьте до мягкости.", "zazharka/step2.jpg", 4, 2 },
                    { 24, "Добавьте свеклу и морковь, тушите на среднем огне.", "zazharka/step3.jpg", 4, 3 },
                    { 25, "Влейте томатный сок, добавьте соль и сахар.", "zazharka/step4.jpg", 4, 4 },
                    { 26, "Тушите 40-50 минут до нужной густоты.", "zazharka/step5.jpg", 4, 5 },
                    { 27, "Добавьте чеснок и специи за 10 минут до конца.", "zazharka/step6.jpg", 4, 6 },
                    { 28, "Подготовьте и отварите картофель, морковь и яйца.", "olivie/step1.jpg", 5, 1 },
                    { 29, "Слейте жидкость из банки с горошком.", "olivie/step2.jpg", 5, 2 },
                    { 30, "Нарежьте колбасу кубиками.", "olivie/step3.jpg", 5, 3 },
                    { 31, "Нарежьте картофель, морковь, яйца и огурцы одинаково.", "olivie/step4.jpg", 5, 4 },
                    { 32, "Смешайте все ингредиенты в большой миске.", "olivie/step5.jpg", 5, 5 },
                    { 33, "Добавьте майонез и соль по вкусу.", "olivie/step6.jpg", 5, 6 },
                    { 34, "Охладите салат в холодильнике перед подачей.", "olivie/step7.jpg", 5, 7 },
                    { 35, "Подготовьте овощи и зелень.", "greece_salad/step1.jpg", 6, 1 },
                    { 36, "Нарежьте огурцы и помидоры крупными кусочками.", "greece_salad/step2.jpg", 6, 2 },
                    { 37, "Добавьте тонкие полукольца лука и маслины.", "greece_salad/step3.jpg", 6, 3 },
                    { 38, "Положите кубики феты поверх салата.", "greece_salad/step4.jpg", 6, 4 },
                    { 39, "Заправьте маслом и лимонным соком, аккуратно перемешайте.", "greece_salad/step5.jpg", 6, 5 },
                    { 40, "Промойте рис до прозрачной воды и замочите на 30 минут.", "kharcho/step1.jpg", 7, 1 },
                    { 41, "Нарежьте баранину кубиками, лук полукольцами, морковь крупной соломкой.", "kharcho/step2.jpg", 7, 2 },
                    { 42, "Разогрейте казан с маслом, обжарьте мясо до румяной корочки.", "kharcho/step3.jpg", 7, 3 },
                    { 43, "Добавьте лук и обжарьте до золотистого цвета.", "kharcho/step4.jpg", 7, 4 },
                    { 44, "Всыпьте морковь и готовьте 8-10 минут, затем добавьте зиру и барбарис.", "kharcho/step5.jpg", 7, 5 },
                    { 45, "Влейте кипяток, чтобы покрыть содержимое, и тушите зирвак 30 минут.", "kharcho/step6.jpg", 7, 6 },
                    { 46, "Добавьте рис ровным слоем, долейте воду на 1 см выше риса и готовьте без крышки.", "kharcho/step7.jpg", 7, 7 },
                    { 47, "Когда вода уйдет, вставьте головки чеснока, накройте и томите 20 минут.", "kharcho/step8.jpg", 7, 8 },
                    { 48, "Очистите креветки, оставив хвостики, и подготовьте остальные ингредиенты.", "mushrooms/step1.jpg", 8, 1 },
                    { 49, "Доведите бульон до кипения, добавьте лемонграсс и листья каффир-лайма.", "mushrooms/step2.jpg", 8, 2 },
                    { 50, "Положите пасту том ям и размешайте до полного растворения.", "mushrooms/step3.jpg", 8, 3 },
                    { 51, "Добавьте нарезанные грибы и варите 3-4 минуты.", "mushrooms/step4.jpg", 8, 4 },
                    { 52, "Влейте кокосовое молоко и прогрейте суп, не доводя до активного кипения.", "mushrooms/step5.jpg", 8, 5 },
                    { 53, "Положите креветки и готовьте 2-3 минуты до розового цвета.", "mushrooms/step6.jpg", 8, 6 },
                    { 54, "Приправьте рыбным соусом и соком лайма, отрегулируйте вкус.", "mushrooms/step7.jpg", 8, 7 },
                    { 55, "Подавайте горячим с кинзой и долькой лайма.", "mushrooms/step8.jpg", 8, 8 },
                    { 56, "Нарежьте баклажан, кабачок и томаты тонкими кружками.", "greece_salad/step1.jpg", 9, 1 },
                    { 57, "Обжарьте лук с перцем, добавьте половину томатов и потушите соус 10 минут.", "greece_salad/step2.jpg", 9, 2 },
                    { 58, "Переложите соус в форму, сверху выложите овощи чередуя кружки.", "greece_salad/step3.jpg", 9, 3 },
                    { 59, "Сбрызните маслом, посыпьте чесноком, тимьяном и базиликом.", "greece_salad/step4.jpg", 9, 4 },
                    { 60, "Запекайте при 180°C около 40 минут до мягкости овощей.", "greece_salad/step5.jpg", 9, 5 },
                    { 61, "Нарежьте говядину тонкой соломкой поперек волокон.", "olivie/step1.jpg", 10, 1 },
                    { 62, "Обваляйте мясо в муке и быстро обжарьте на сильном огне.", "olivie/step2.jpg", 10, 2 },
                    { 63, "Отдельно обжарьте лук и грибы до испарения влаги.", "olivie/step3.jpg", 10, 3 },
                    { 64, "Верните мясо в сковороду, добавьте горчицу, соль и перец.", "olivie/step4.jpg", 10, 4 },
                    { 65, "Влейте сливки и добавьте сметану, аккуратно перемешайте.", "olivie/step5.jpg", 10, 5 },
                    { 66, "Тушите 8-10 минут на слабом огне до загустения соуса.", "olivie/step6.jpg", 10, 6 },
                    { 67, "Подавайте с картофельным пюре, рисом или пастой.", "olivie/step7.jpg", 10, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CategoryId",
                table: "Recipes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeSteps_RecipeId",
                table: "RecipeSteps",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeViewHistories_ClientKey_RecipeId",
                table: "RecipeViewHistories",
                columns: new[] { "ClientKey", "RecipeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeViewHistories_RecipeId",
                table: "RecipeViewHistories",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropTable(
                name: "RecipeSteps");

            migrationBuilder.DropTable(
                name: "RecipeViewHistories");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
