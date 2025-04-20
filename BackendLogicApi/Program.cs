using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dodaj MVC
builder.Services.AddControllersWithViews();

// Rejestracja serwisów
builder.Services.AddScoped<IProductOperationsService, ProductOperationsService>();
builder.Services.AddScoped<ICalorieCalculatorService, CalorieCalculatorService>();
builder.Services.AddScoped<IProductServiceAPI, ProductServiceAPI>();
builder.Services.AddLogging();
// Rejestracja repozytoriów i DbContext
builder.Services.AddScoped<ProductLogRepository>();
builder.Services.AddScoped<AppDbContext>();

// Konfiguracja DbContext (jeœli u¿ywasz bazy danych)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=product;Username=postgres;Password=Pakan135@"));


// Rejestracja kontrolerów
builder.Services.AddControllers();

var app = builder.Build();
app.UseAuthorization();

// Mapa routingu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapa kontrolerów API
app.MapControllers();

app.Run();
