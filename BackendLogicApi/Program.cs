using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodaj MVC
builder.Services.AddControllersWithViews();

// Rejestracja serwis�w
builder.Services.AddScoped<IProductOperationsService, ProductOperationsService>();
builder.Services.AddScoped<ICalorieCalculatorService, CalorieCalculatorService>();
builder.Services.AddScoped<IProductServiceAPI, ProductServiceAPI>();
builder.Services.AddLogging();
// Rejestracja repozytori�w i DbContext
builder.Services.AddScoped<ProductLogRepository>();
builder.Services.AddScoped<AppDbContext>();

// Konfiguracja DbContext (je�li u�ywasz bazy danych)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=product;Username=postgres;Password=Pakan135@"));


// Rejestracja kontroler�w
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();

// Mapa routingu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapa kontroler�w API
app.MapControllers();

app.Run();
