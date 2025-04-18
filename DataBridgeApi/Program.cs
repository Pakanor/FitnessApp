var builder = WebApplication.CreateBuilder(args);

// Dodaj MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja routingu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
