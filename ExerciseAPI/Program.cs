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
builder.Services.AddScoped<HeatmapService>();
builder.Services.AddScoped<OneRepMaxCalculator>();
builder.Services.AddScoped<RecordsService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.ITemplateService, ExerciseAPI.Services.TemplateService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IWorkoutStartModeService, ExerciseAPI.Services.WorkoutStartModeService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IWorkoutStatusService, ExerciseAPI.Services.WorkoutStatusService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IWorkloadCalculationService, ExerciseAPI.Services.WorkloadCalculationService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.ICarbohydrateScalingService, ExerciseAPI.Services.CarbohydrateScalingService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IAcwrService, ExerciseAPI.Services.AcwrService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IMuscleRecoveryService, ExerciseAPI.Services.MuscleRecoveryService>();
builder.Services.AddScoped<ExerciseAPI.Interfaces.IMuscleDamageService, ExerciseAPI.Services.MuscleDamageService>();
builder.Services.AddMemoryCache();

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

    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex) when (ex is Npgsql.PostgresException or InvalidOperationException)
    {
        // Fallback: apply schema changes directly if migration fails
        db.Database.ExecuteSqlRaw("""
            ALTER TABLE "Exercises" ADD COLUMN IF NOT EXISTS "IsBenchmark" boolean NOT NULL DEFAULT false;
            CREATE TABLE IF NOT EXISTS "MuscleGroups" (
                "Key" text PRIMARY KEY,
                "NamePl" text NOT NULL,
                "IsFront" boolean NOT NULL,
                "HalfLife" double precision NOT NULL
            );
            CREATE TABLE IF NOT EXISTS "ExerciseMuscleGroups" (
                "Id" serial PRIMARY KEY,
                "ExerciseId" integer NOT NULL REFERENCES "Exercises"("Id"),
                "MuscleGroupKey" text NOT NULL REFERENCES "MuscleGroups"("Key"),
                "WeightPercentage" numeric NOT NULL DEFAULT 0
            );
        """);

        var existingMg = db.MuscleGroups.Any();
        if (!existingMg)
        {
            db.Database.ExecuteSqlRaw("""
                INSERT INTO "MuscleGroups" ("Key", "NamePl", "IsFront", "HalfLife") VALUES
                ('chest_main', 'Klatka piersiowa', true, 42),
                ('deltoid_anterior', 'Bark przedni', true, 30),
                ('deltoid_lateral', 'Bark boczny', true, 30),
                ('deltoid_posterior', 'Bark tylny', false, 30),
                ('biceps', 'Biceps', true, 30),
                ('triceps', 'Triceps', false, 30),
                ('forearms', 'Przedramiona', true, 24),
                ('lats', 'Plecy szerokie', false, 42),
                ('rhomboids', 'Romby i czworoboczny', false, 30),
                ('lower_back', 'Dolny odcinek pleców', false, 30),
                ('abs', 'Brzuch', true, 24),
                ('core_stabilizers', 'Stabilizatory tułowia', true, 24),
                ('quadriceps', 'Czwórki', true, 42),
                ('hamstrings', 'Dwugłowe uda', false, 30),
                ('glutes', 'Pośladki', false, 42),
                ('calves', 'Łydki', false, 24);
            """);
        }

        var existingMappings = db.ExerciseMuscleGroups.Any();
        if (!existingMappings)
        {
            db.Database.ExecuteSqlRaw("""
                INSERT INTO "ExerciseMuscleGroups" ("ExerciseId", "MuscleGroupKey", "WeightPercentage") VALUES
                (1028, 'lower_back', 40),
                (1028, 'glutes', 30),
                (1028, 'hamstrings', 30),
                (1111, 'glutes', 40),
                (1111, 'hamstrings', 30),
                (1111, 'quadriceps', 20),
                (1111, 'lower_back', 10);
            """);
        }

        db.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS "WorkoutTemplates" (
                "Id" serial PRIMARY KEY,
                "UserId" integer NOT NULL,
                "Name" character varying(100) NOT NULL,
                "CreatedAt" timestamp with time zone NOT NULL,
                "UpdatedAt" timestamp with time zone NOT NULL
            );
            CREATE INDEX IF NOT EXISTS "IX_WorkoutTemplates_UserId" ON "WorkoutTemplates" ("UserId");

            CREATE TABLE IF NOT EXISTS "TemplateExercises" (
                "Id" serial PRIMARY KEY,
                "TemplateId" integer NOT NULL REFERENCES "WorkoutTemplates"("Id") ON DELETE CASCADE,
                "ExerciseId" integer NOT NULL REFERENCES "Exercises"("Id") ON DELETE CASCADE,
                "Order" integer NOT NULL
            );
            CREATE INDEX IF NOT EXISTS "IX_TemplateExercises_TemplateId_Order" ON "TemplateExercises" ("TemplateId", "Order");
            CREATE INDEX IF NOT EXISTS "IX_TemplateExercises_ExerciseId" ON "TemplateExercises" ("ExerciseId");

            ALTER TABLE "UserExercise" ADD COLUMN IF NOT EXISTS "StartMode" integer;
            ALTER TABLE "UserExercise" ADD COLUMN IF NOT EXISTS "TemplateId" integer;
            ALTER TABLE "UserExercise" ADD COLUMN IF NOT EXISTS "Status" integer;

            CREATE TABLE IF NOT EXISTS "MuscleDamage" (
                "UserId" integer NOT NULL,
                "MuscleGroupKey" text NOT NULL,
                "SessionDate" timestamp with time zone NOT NULL,
                "DamagePercent" double precision NOT NULL,
                "IsPrimary" boolean NOT NULL,
                PRIMARY KEY ("UserId", "MuscleGroupKey")
            );
        """);
    }

    var importService = scope.ServiceProvider.GetRequiredService<ExerciseDbImportService>();
    await importService.ImportExercisesAsync();
}


app.Run();
