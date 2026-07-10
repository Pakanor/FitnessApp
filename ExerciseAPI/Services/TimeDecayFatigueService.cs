using ExerciseAPI.DTOs;

namespace ExerciseAPI.Services
{
    public class TimeDecayFatigueService : IFatigueService
    {
        private static readonly HashSet<string> LargeMuscleKeys = new()
        {
            "chest_main", "quadriceps", "lats", "glutes", "lower_back"
        };

        private const int LargeRecoveryHours = 72;
        private const int SmallRecoveryHours = 48;

        public List<MuscleStatsDto> CalculateFatigue(IEnumerable<UserExerciseWithMappingDto> logs, int? caloriesDelta = null)
        {
            var logList = logs.ToList();
            if (logList.Count == 0)
                return new List<MuscleStatsDto>();

            decimal dietMultiplier = 1m;
            if (caloriesDelta.HasValue)
            {
                if (caloriesDelta < 0)
                    dietMultiplier = 1m + ((decimal)caloriesDelta.Value / 2000m);
                else if (caloriesDelta > 0)
                    dietMultiplier = 1m - ((decimal)caloriesDelta.Value / 3000m);
                dietMultiplier = Math.Max(0.5m, Math.Min(1.5m, dietMultiplier));
            }

            var grouped = logList
                .GroupBy(l => l.MuscleNameKey)
                .ToList();

            var results = new List<MuscleStatsDto>();

            foreach (var group in grouped)
            {
                var first = group.First();
                bool isLarge = LargeMuscleKeys.Contains(first.MuscleNameKey);
                int recoveryHours = isLarge ? LargeRecoveryHours : SmallRecoveryHours;
                decimal recoveryRatePerHour = 100m / recoveryHours;
                recoveryRatePerHour = recoveryRatePerHour * dietMultiplier;

                int totalVolume = group.Sum(g => g.Sets);
                decimal baselineFatigue = 0;

                foreach (var log in group)
                {
                    decimal rpeModifier = log.RPE.HasValue ? log.RPE.Value / 10m : 0.8m;
                    baselineFatigue += log.Sets * log.Factor * 10m * rpeModifier;
                }
                baselineFatigue = Math.Min(100, baselineFatigue);

                DateTime earliestDate = group.Min(g => g.Date);
                if (earliestDate.Kind != DateTimeKind.Utc)
                    earliestDate = DateTime.SpecifyKind(earliestDate, DateTimeKind.Utc);
                double elapsedHours = (DateTime.UtcNow - earliestDate).TotalHours;
                if (elapsedHours < 0) elapsedHours = 0;

                decimal fatigue = baselineFatigue - (decimal)elapsedHours * recoveryRatePerHour;
                fatigue = Math.Max(0, Math.Min(100, fatigue));

                results.Add(new MuscleStatsDto
                {
                    NameKey = first.MuscleNameKey,
                    NamePl = first.MuscleNamePl,
                    Volume = totalVolume,
                    FatiguePercentage = Math.Round(fatigue, 1),
                    IsFront = first.IsFront
                });
            }

            return results;
        }
    }
}
