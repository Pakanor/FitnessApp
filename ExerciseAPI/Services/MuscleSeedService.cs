using ExerciseAPI.Data;
using ExerciseAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ExerciseAPI.Services
{
    public class MuscleSeedService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MuscleSeedService> _logger;

        public MuscleSeedService(AppDbContext context, ILogger<MuscleSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedMuscleMappingsAsync()
        {
            if (await _context.ExerciseMuscleMappings.AnyAsync())
            {
                _logger.LogInformation("Muscle mappings already exist, skipping seed.");
                return;
            }

            var exercises = await _context.Exercises.ToListAsync();
            var muscles = await _context.Muscles.ToDictionaryAsync(m => m.NameKey);

            if (!exercises.Any() || !muscles.Any())
            {
                _logger.LogWarning("Exercises or muscles not found. Ensure base data is seeded first.");
                return;
            }

            var mappings = new List<ExerciseMuscleMapping>();

            foreach (var exercise in exercises)
            {
                var exerciseMappings = GetMuscleMappings(exercise, muscles);
                mappings.AddRange(exerciseMappings);
            }

            _context.ExerciseMuscleMappings.AddRange(mappings);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} muscle mappings for {ExerciseCount} exercises.", mappings.Count, exercises.Count);
        }

        private List<ExerciseMuscleMapping> GetMuscleMappings(Exercise exercise, Dictionary<string, Muscle> muscles)
        {
            var mappings = new List<ExerciseMuscleMapping>();
            var name = exercise.Name.ToLowerInvariant();
            var category = exercise.Category.ToLowerInvariant();

            // Category-based primary mappings
            switch (category)
            {
                case "chest":
                    AddMapping(mappings, exercise.Id, muscles["chest_main"], "primary", 1.0m);
                    if (name.Contains("dip")) AddMapping(mappings, exercise.Id, muscles["triceps"], "secondary", 0.5m);
                    if (name.Contains("fly") || name.Contains("crossover")) AddMapping(mappings, exercise.Id, muscles["deltoid_anterior"], "secondary", 0.3m);
                    break;

                case "upper arms":
                    if (name.Contains("biceps") || name.Contains("curl"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["biceps"], "primary", 1.0m);
                        AddMapping(mappings, exercise.Id, muscles["forearms"], "secondary", 0.4m);
                    }
                    else if (name.Contains("triceps") || name.Contains("extension") || name.Contains("pushdown"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["triceps"], "primary", 1.0m);
                    }
                    else
                    {
                        AddMapping(mappings, exercise.Id, muscles["biceps"], "primary", 0.7m);
                        AddMapping(mappings, exercise.Id, muscles["triceps"], "secondary", 0.5m);
                    }
                    break;

                case "shoulders":
                    if (name.Contains("front") || name.Contains("anterior"))
                        AddMapping(mappings, exercise.Id, muscles["deltoid_anterior"], "primary", 1.0m);
                    else if (name.Contains("lateral") || name.Contains("side"))
                        AddMapping(mappings, exercise.Id, muscles["deltoid_lateral"], "primary", 1.0m);
                    else if (name.Contains("rear") || name.Contains("posterior") || name.Contains("reverse"))
                        AddMapping(mappings, exercise.Id, muscles["deltoid_posterior"], "primary", 1.0m);
                    else
                    {
                        AddMapping(mappings, exercise.Id, muscles["deltoid_anterior"], "primary", 0.6m);
                        AddMapping(mappings, exercise.Id, muscles["deltoid_lateral"], "primary", 0.8m);
                        AddMapping(mappings, exercise.Id, muscles["deltoid_posterior"], "secondary", 0.4m);
                    }
                    break;

                case "back":
                    if (name.Contains("lat") || name.Contains("pulldown") || name.Contains("pull-down"))
                        AddMapping(mappings, exercise.Id, muscles["lats"], "primary", 1.0m);
                    else if (name.Contains("row"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["lats"], "primary", 0.7m);
                        AddMapping(mappings, exercise.Id, muscles["rhomboids_trapezius"], "primary", 0.8m);
                    }
                    else if (name.Contains("shrug") || name.Contains("trap"))
                        AddMapping(mappings, exercise.Id, muscles["rhomboids_trapezius"], "primary", 1.0m);
                    else
                    {
                        AddMapping(mappings, exercise.Id, muscles["lats"], "primary", 0.8m);
                        AddMapping(mappings, exercise.Id, muscles["rhomboids_trapezius"], "secondary", 0.5m);
                    }
                    if (name.Contains("back") && name.Contains("extens")) AddMapping(mappings, exercise.Id, muscles["lower_back"], "primary", 1.0m);
                    break;

                case "waist":
                    if (name.Contains("crunch") || name.Contains("sit-up") || name.Contains("sit up"))
                        AddMapping(mappings, exercise.Id, muscles["abs"], "primary", 1.0m);
                    else if (name.Contains("plank"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["abs"], "primary", 0.6m);
                        AddMapping(mappings, exercise.Id, muscles["core_stabilizers"], "primary", 0.8m);
                    }
                    else if (name.Contains("twist") || name.Contains("russian"))
                        AddMapping(mappings, exercise.Id, muscles["abs"], "primary", 0.8m);
                    else if (name.Contains("leg raise") || name.Contains("knee raise"))
                        AddMapping(mappings, exercise.Id, muscles["abs"], "primary", 0.9m);
                    else
                    {
                        AddMapping(mappings, exercise.Id, muscles["abs"], "primary", 0.7m);
                        AddMapping(mappings, exercise.Id, muscles["core_stabilizers"], "secondary", 0.5m);
                    }
                    break;

                case "upper legs":
                    if (name.Contains("squat") || name.Contains("press") && name.Contains("leg"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["quadriceps"], "primary", 1.0m);
                        AddMapping(mappings, exercise.Id, muscles["glutes"], "secondary", 0.6m);
                    }
                    else if (name.Contains("deadlift") || name.Contains("romanian"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["hamstrings"], "primary", 1.0m);
                        AddMapping(mappings, exercise.Id, muscles["glutes"], "secondary", 0.7m);
                        AddMapping(mappings, exercise.Id, muscles["lower_back"], "secondary", 0.5m);
                    }
                    else if (name.Contains("lunge"))
                    {
                        AddMapping(mappings, exercise.Id, muscles["quadriceps"], "primary", 0.8m);
                        AddMapping(mappings, exercise.Id, muscles["glutes"], "primary", 0.7m);
                    }
                    else if (name.Contains("leg curl") || name.Contains("hamstring"))
                        AddMapping(mappings, exercise.Id, muscles["hamstrings"], "primary", 1.0m);
                    else if (name.Contains("hip") || name.Contains("abduct"))
                        AddMapping(mappings, exercise.Id, muscles["glutes"], "primary", 1.0m);
                    else
                    {
                        AddMapping(mappings, exercise.Id, muscles["quadriceps"], "primary", 0.8m);
                        AddMapping(mappings, exercise.Id, muscles["hamstrings"], "secondary", 0.5m);
                    }
                    break;

                case "lower legs":
                    if (name.Contains("calf") || name.Contains("raise"))
                        AddMapping(mappings, exercise.Id, muscles["calves"], "primary", 1.0m);
                    else
                        AddMapping(mappings, exercise.Id, muscles["calves"], "primary", 0.9m);
                    break;

                case "lower arms":
                    AddMapping(mappings, exercise.Id, muscles["forearms"], "primary", 1.0m);
                    break;

                case "neck":
                    AddMapping(mappings, exercise.Id, muscles["core_stabilizers"], "secondary", 0.3m);
                    break;

                case "cardio":
                    // Cardio exercises primarily engage legs and core
                    AddMapping(mappings, exercise.Id, muscles["quadriceps"], "secondary", 0.4m);
                    AddMapping(mappings, exercise.Id, muscles["core_stabilizers"], "secondary", 0.3m);
                    break;
            }

            return mappings;
        }

        private void AddMapping(List<ExerciseMuscleMapping> mappings, int exerciseId, Muscle muscle, string role, decimal factor)
        {
            mappings.Add(new ExerciseMuscleMapping
            {
                ExerciseId = exerciseId,
                MuscleId = muscle.Id,
                Role = role,
                Factor = factor
            });
        }
    }
}
