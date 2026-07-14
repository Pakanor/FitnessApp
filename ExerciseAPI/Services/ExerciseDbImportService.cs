using ExerciseAPI.Data;
using ExerciseAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExerciseAPI.Services
{
    public class ExerciseDbImportService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExerciseDbImportService> _logger;

        public ExerciseDbImportService(IServiceScopeFactory scopeFactory, ILogger<ExerciseDbImportService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ImportExercisesAsync()
        {
            var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "exercise-everything.csv");

            if (!File.Exists(csvPath))
            {
                _logger.LogWarning("exercise-everything.csv not found at {Path}", csvPath);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var existingExercises = await context.Exercises.ToDictionaryAsync(e => e.Id);
            var lines = await File.ReadAllLinesAsync(csvPath);
            var header = lines[0].Split(',');
            var muscleColumns = header[3..].Select(c => c.Trim()).ToList();

            var newExercises = new List<Exercise>();
            var updatedCount = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = SplitCsvLine(lines[i]);
                if (parts.Length < 4) continue;

                if (!int.TryParse(parts[0], out int id)) continue;

                if (existingExercises.TryGetValue(id, out var existing))
                {
                    var changed = false;
                    for (int j = 0; j < muscleColumns.Count && (3 + j) < parts.Length; j++)
                    {
                        var val = parts[3 + j].Trim();
                        if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal coeff))
                        {
                            if (SetMuscleCoefficient(existing, muscleColumns[j], coeff))
                                changed = true;
                        }
                    }
                    if (changed) updatedCount++;
                }
                else
                {
                    var exercise = new Exercise
                    {
                        Id = id,
                        ExternalId = parts[0],
                        Name = parts[1].Trim('"'),
                        Description = null,
                        Category = parts[2].Trim('"'),
                    };

                    for (int j = 0; j < muscleColumns.Count && (3 + j) < parts.Length; j++)
                    {
                        var val = parts[3 + j].Trim();
                        if (decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal coeff))
                        {
                            SetMuscleCoefficient(exercise, muscleColumns[j], coeff);
                        }
                    }

                    newExercises.Add(exercise);
                }
            }

            if (newExercises.Count > 0)
            {
                context.Exercises.AddRange(newExercises);
            }

            if (updatedCount > 0 || newExercises.Count > 0)
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("Imported {NewCount} new exercises, updated {UpdatedCount} existing", newExercises.Count, updatedCount);
            }
        }

        private static bool SetMuscleCoefficient(Exercise e, string column, decimal value)
        {
            switch (column)
            {
                case "ChestMain": e.ChestMain = value; return true;
                case "DeltoidAnterior": e.DeltoidAnterior = value; return true;
                case "DeltoidLateral": e.DeltoidLateral = value; return true;
                case "DeltoidPosterior": e.DeltoidPosterior = value; return true;
                case "Biceps": e.Biceps = value; return true;
                case "Triceps": e.Triceps = value; return true;
                case "Forearms": e.Forearms = value; return true;
                case "Lats": e.Lats = value; return true;
                case "Rhomboids": e.Rhomboids = value; return true;
                case "LowerBack": e.LowerBack = value; return true;
                case "Abs": e.Abs = value; return true;
                case "CoreStabilizers": e.CoreStabilizers = value; return true;
                case "Quadriceps": e.Quadriceps = value; return true;
                case "Hamstrings": e.Hamstrings = value; return true;
                case "Glutes": e.Glutes = value; return true;
                case "Calves": e.Calves = value; return true;
                default: return false;
            }
        }

        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(line[i]);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }
    }
}
