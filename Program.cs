// Source - https://stackoverflow.com/a/75445858
// Posted by AngelaG, modified by community. See post 'Timeline' for change history
// Retrieved 2026-03-02, License - CC BY-SA 4.0

using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Models;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    // Populate new content columns for existing rows once after migration.
    if (dbContext.Recipes.Any(r => string.IsNullOrEmpty(r.Slug)))
    {
        var contentByName = new Dictionary<string, (string Slug, string Cuisine, string Folder, string Ingredients, string Steps, bool IsFavorite)>
        {
            ["Сырники"] = (
                "syrniki",
                "Русская",
                "syrniki",
                "Творог — 300 г\nЯйцо — 1 шт.\nСахар — 2 ст.л.\nМука — 4 ст.л.\nЩепотка соли\nМасло для жарки",
                "Смешайте творог, яйцо, сахар и соль до однородности.\nДобавьте муку и замесите мягкое тесто.\nСформируйте небольшие сырники и обваляйте в муке.\nОбжарьте на среднем огне с двух сторон до золотистой корочки.\nПодавайте горячими со сметаной или ягодным соусом.",
                true
            ),
            ["Харчо"] = (
                "kharcho",
                "Грузинская",
                "kharcho",
                "Вода — 2.5 л\nГовядина — 700 г\nПомидоры — 400 г\nКартофель — 300 г\nРис — 100 г\nЛук — 150 г\nЧеснок — 3 зубчика\nПетрушка — 20 г\nХмели-сунели — 1.5 ч.л.\nСоль и перец — по вкусу",
                "Подготовьте ингредиенты и нарежьте мясо кусочками.\nСварите мясной бульон на медленном огне, снимая пену.\nПромойте и замочите рис на 20 минут.\nНарежьте овощи, помидоры очистите от кожицы.\nДобавьте в бульон рис, картофель и часть помидоров.\nОбжарьте лук, добавьте оставшиеся помидоры и потушите.\nПереложите зажарку в суп, добавьте специи, чеснок и зелень.\nДайте супу настояться 10 минут и подавайте горячим.",
                true
            ),
            ["Грибы в сметане"] = (
                "mushrooms",
                "Домашняя",
                "mushrooms",
                "Опята — 1.2 кг\nСоль — 80 г\nПерец горошком — 8-10 шт.\nЛавровый лист — 3 шт.\nЧеснок — 6 зубчиков\nВода — 1 л",
                "Подготовьте грибы и специи, промойте опята.\nУдалите корешки и тщательно переберите грибы.\nОтварите опята до готовности и остудите.\nПриготовьте рассол с солью и специями.\nДобавьте грибы в рассол и проварите 25 минут.\nПростерилизуйте банки и крышки.\nРазложите грибы по банкам и закатайте.\nВыдержите заготовку 4-6 недель в прохладном месте.",
                true
            ),
            ["Борщ"] = (
                "borsch",
                "Домашняя",
                "zazharka",
                "Морковь — 200 г\nСвекла — 300 г\nЛук — 150 г\nЧеснок — 3 зубчика\nТоматный сок — 700 мл\nСоль — 1-1.5 ч.л.\nСахар — 1 ч.л.\nМасло — 2 ст.л.",
                "Подготовьте ингредиенты и вымойте овощи.\nНарежьте лук и обжарьте до мягкости.\nДобавьте свеклу и морковь, тушите на среднем огне.\nВлейте томатный сок, добавьте соль и сахар.\nТушите 40-50 минут до нужной густоты.\nДобавьте чеснок и специи за 10 минут до конца.\nРазложите заготовку по стерильным банкам и закатайте.",
                true
            ),
            ["Оливье"] = (
                "olivie",
                "Русская",
                "olivie",
                "Варёная колбаса — 300 г\nЗелёный горошек — 200 г\nЯйца — 4 шт.\nКартошка — 600 г\nСоленые огурцы — 200 г\nМорковь — 150 г\nМайонез — 150-200 г\nСоль — по вкусу",
                "Подготовьте и отварите картофель, морковь и яйца.\nСлейте жидкость из банки с горошком.\nНарежьте колбасу кубиками.\nНарежьте картофель, морковь, яйца и огурцы одинаково.\nСмешайте все ингредиенты в большой миске.\nДобавьте майонез и соль по вкусу.\nОхладите салат в холодильнике перед подачей.",
                true
            ),
            ["Греческий салат"] = (
                "greece-salad",
                "Европейская",
                "greece_salad",
                "Помидоры — 250 г\nОгурцы — 200 г\nФета — 150 г\nМаслины — 80 г\nКрасный лук — 1/2 шт.\nОливковое масло — 2 ст.л.\nЛимонный сок — 1 ст.л.",
                "Подготовьте овощи и зелень.\nНарежьте огурцы и помидоры крупными кусочками.\nДобавьте тонкие полукольца лука и маслины.\nПоложите кубики феты поверх салата.\nЗаправьте маслом и лимонным соком, аккуратно перемешайте.",
                false
            )
        };

        foreach (var recipe in dbContext.Recipes)
        {
            if (recipe.Name == null || !contentByName.TryGetValue(recipe.Name, out var value))
            {
                continue;
            }

            recipe.Slug = value.Slug;
            recipe.Cuisine = value.Cuisine;
            recipe.StepImagesFolder = value.Folder;
            recipe.IngredientsText = value.Ingredients;
            recipe.StepsText = value.Steps;
            recipe.IsFavorite = value.IsFavorite;
        }

        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
