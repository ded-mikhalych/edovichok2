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

    if (dbContext.News.Any(n => string.IsNullOrEmpty(n.ContentHtml)))
    {
        var newsContentByTitle = new Dictionary<string, string>
        {
            ["Налог на чипсы и газировку"] = "<p>В России обсуждается инициатива по введению налога на отдельные категории вредных продуктов. Эксперты считают, что мера может снизить потребление ультрапереработанной еды и стимулировать производителей пересматривать состав продукции.</p><p>Пока документ находится на стадии обсуждения, но тема уже вызвала широкий общественный резонанс.</p>",
            ["Фрукт против похмелья"] = "<p>Исследователи сообщили о фрукте, содержащем вещества, которые могут облегчить симптомы похмелья и ускорить восстановление после употребления алкоголя. В публикации подчеркивается, что наибольший эффект достигается в сочетании с полноценной гидратацией и сном.</p>",
            ["Подсластители под угрозой"] = "<p>Как <a href=\"https://www.neurology.org/doi/10.1212/WNL.0000000000214023\">заявлено</a> в статье для журнала Neurology, группа бразильских специалистов в течение 8 лет изучала сведения о питании и здоровье 12 тыс. человек, средний возраст которых составлял 50 лет. Добровольцы регулярно сообщали, сколько продуктов и напитков с подсластителями они употребляют.</p><p><strong>Как проводилось исследование</strong></p><p>Чтобы оценить состояние нервной системы, испытуемым давали интеллектуальные упражнения на память, владение речью, концентрацию и быстроту мыслительных процессов.</p><p>Ученые установили, что регулярное включение искусственных подсластителей в рацион существенно отражается на работе мозга. В частности, интеллектуальные способности снижаются быстрее на 60%.</p><p><strong>Результаты</strong></p><p>К провоцирующим факторам отнесли злоупотребление популярными сахарозаменителями, в том числе:</p><ul><li>аспартамом;</li><li>сахарином;</li><li>эритритом;</li><li>сорбитом;</li><li>ксилитом.</li></ul><p>Исключением из правил стала только тагатоза — ее связь с ухудшением когнитивных функций обнаружена не была. Этот редкий натуральный подсластитель содержится в ягодах, фруктах, некоторых овощах, какао-бобах, а также в молочных продуктах.</p><table style=\"border-collapse:collapse;width:100%;max-width:720px;margin:16px 0;background:#fff;\"><tr><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Происхождение</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Калорийность</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Гликемический индекс</th><th style=\"border:1px solid var(--border);padding:8px;text-align:left;\">Особенности</th></tr><tr><td style=\"border:1px solid var(--border);padding:8px;\">натуральное</td><td style=\"border:1px solid var(--border);padding:8px;\">1,5 ккал/г</td><td style=\"border:1px solid var(--border);padding:8px;\">3</td><td style=\"border:1px solid var(--border);padding:8px;\">Не провоцирует кариес, обладает пробиотическими свойствами</td></tr></table><p>Ранее профессор выделил <a href=\"https://www.gastronom.ru/news/professor-vydelil-produkty-kotorye-ukreplyayut-immunitet-luchshe-dobavok-1026997\">продукты, которые укрепляют иммунитет лучше добавок</a>.</p>",
            ["История грузинской кухни"] = "<p>Грузинская кухня славится балансом специй, свежей зелени и медленного приготовления. Хачапури, хинкали, пхали и сациви стали визитной карточкой региона и давно вышли за его пределы.</p><p>В основе подхода — сезонные продукты, яркие соусы и уважение к семейным традициям.</p>"
        };

        foreach (var news in dbContext.News)
        {
            if (news.Title == null || !newsContentByTitle.TryGetValue(news.Title, out var content))
            {
                continue;
            }

            news.ContentHtml = content;
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
