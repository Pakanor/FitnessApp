using ExerciseAPI.DTOs;

namespace ExerciseAPI.Services
{
    public interface IFatigueService
    {
        List<MuscleStatsDto> CalculateFatigue(IEnumerable<UserExerciseWithMappingDto> logs, int? caloriesDelta = null);
    }
}
