using BackendLogicApi.DataAccess;
using BackendLogicApi.Interfaces;
using BackendLogicApi.Models;
using BackendLogicApi.Services;
using BackendLogicApi.Services.Validators;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductOperationsService, ProductOperationsService>();
builder.Services.AddScoped<ICalorieCalculatorService, CalorieCalculatorService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IProductServiceAPI, ProductServiceAPI>();
builder.Services.AddLogging();
builder.Services.AddScoped<ProductLogRepository>();
builder.Services.AddScoped<UserLogrepository>();
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>();
    });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=product;Username=postgres;Password=Pakan135@"));


builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
