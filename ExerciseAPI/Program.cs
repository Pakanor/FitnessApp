using ExerciseAPI.Services;
using Microsoft.EntityFrameworkCore;
using ExerciseAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "AuthAPI",
            ValidateAudience = true,
            ValidAudience = "FitnessAppUsers", 
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SUPER_SECRET_KEY_123456789012345678901234567890"))
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



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=exercise;Username=fitnessapp;Password=Pakan135@"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ExerciseDbImportService>();
builder.Services.AddScoped<MuscleSeedService>();
builder.Services.AddScoped<IFatigueService, TimeDecayFatigueService>();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var muscleSeedService = scope.ServiceProvider.GetRequiredService<MuscleSeedService>();
    await muscleSeedService.SeedMuscleMappingsAsync();
}


app.Run();

