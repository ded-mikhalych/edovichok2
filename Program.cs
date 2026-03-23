// Source - https://stackoverflow.com/a/75445858
// Posted by AngelaG, modified by community. See post 'Timeline' for change history
// Retrieved 2026-03-02, License - CC BY-SA 4.0

using Microsoft.EntityFrameworkCore;
using WebApplication;
using WebApplication.Data;
using WebApplication.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// On Railway the DATABASE_URL env var is provided as a postgres:// URI.
// Parse it into an Npgsql connection string so the rest of the app is unchanged.
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (databaseUrl != null)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var connStr = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                  $"Username={userInfo[0]};Password={userInfo[1]};" +
                  "SSL Mode=Require;Trust Server Certificate=true;";
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connStr;
}

// Listen on the port Railway (or any host) assigns via $PORT.
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://+:{port}");

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<SiteContentService>();

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    NewsContentSeed.ApplyMissingContent(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
