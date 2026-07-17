using AuthAPI.DataAccess;
using AuthAPI.Interfaces;
using AuthAPI.Services;
using AuthAPI.Services.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AnthropometryService>();
builder.Services.AddScoped<UserLogrepository>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://localhost:8000")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("FitnessApp-Auth"))
                {
                    context.Token = context.Request.Cookies["FitnessApp-Auth"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=auth;Username=fitnessapp;Password=Pakan135@"));
    /*tu w dockerze pozniej host=db-auth*/

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.ExecuteSqlRaw("""ALTER TABLE "Users" DROP COLUMN IF EXISTS "CaloriesDelta" """);
    db.Database.ExecuteSqlRaw("""ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Height" numeric """);
    db.Database.ExecuteSqlRaw("""ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Gender" text """);
    db.Database.ExecuteSqlRaw("""ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "JobType" text """);
    db.Database.ExecuteSqlRaw("""ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Goal" text """);

    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "BodyMeasurements" (
            "Id" serial PRIMARY KEY,
            "UserId" integer NOT NULL,
            "Height" numeric NOT NULL DEFAULT 0,
            "Neck" numeric NOT NULL DEFAULT 0,
            "Waist" numeric NOT NULL DEFAULT 0,
            "Hips" numeric NOT NULL DEFAULT 0,
            "Shoulders" numeric NOT NULL DEFAULT 0,
            "Weight" numeric NOT NULL DEFAULT 0,
            "Chest" numeric,
            "Biceps" numeric,
            "Thigh" numeric,
            "Calf" numeric,
            "BicepsLeft" numeric,
            "BicepsRight" numeric,
            "ThighLeft" numeric,
            "ThighRight" numeric,
            "CalfLeft" numeric,
            "CalfRight" numeric,
            "Belly" numeric,
            "ForearmLeft" numeric,
            "ForearmRight" numeric,
            "BfPercent" numeric,
            "Whr" numeric,
            "Vtaper" numeric,
            "MeasuredAt" timestamptz NOT NULL DEFAULT NOW()
        );
    """);
}

app.Run();
