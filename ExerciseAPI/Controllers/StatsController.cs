using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Data;
using ExerciseAPI.DTOs;
using ExerciseAPI.Services;
using System.Security.Claims;
using System.Text.Json;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/stats")]
    [Authorize]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IFatigueService _fatigueService;
        private readonly IHttpClientFactory _httpClientFactory;

        public StatsController(AppDbContext context, IFatigueService fatigueService, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _fatigueService = fatigueService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("muscles")]
        public async Task<IActionResult> GetMuscles()
        {
            var muscles = await _context.Muscles
                .OrderBy(m => m.IsFront)
                .ToListAsync();

            return Ok(muscles);
        }

        [HttpGet("anatomic-dashboard")]
        public async Task<IActionResult> GetAnatomicDashboard()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var now = DateTime.UtcNow;
            var sevenDaysAgo = now.Date.AddDays(-7);

            var logs = await _context.UserExercise
                .Where(ue => ue.UserId == userId && ue.Date >= sevenDaysAgo)
                .Join(_context.Exercises,
                    ue => ue.ExerciseId,
                    e => e.Id,
                    (ue, e) => new { ue, e })
                .SelectMany(
                    ue_e => ue_e.e.MuscleMappings,
                    (ue_e, em) => new UserExerciseWithMappingDto
                    {
                        UserId = ue_e.ue.UserId,
                        Date = ue_e.ue.Date,
                        Sets = ue_e.ue.Sets ?? 0,
                        MuscleId = em.MuscleId,
                        MuscleNameKey = em.Muscle.NameKey,
                        MuscleNamePl = em.Muscle.NamePl,
                        Factor = em.Factor,
                        IsFront = em.Muscle.IsFront,
                        RPE = ue_e.ue.RPE
                    })
                .ToListAsync();

            int? caloriesDelta = null;
            try
            {
                var profileClient = _httpClientFactory.CreateClient();
                profileClient.BaseAddress = new Uri("http://localhost:5010");
                var profileResponse = await profileClient.GetAsync($"/api/user/profile");
                if (profileResponse.IsSuccessStatusCode)
                {
                    var profileJson = await profileResponse.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<JsonElement>(profileJson);
                    if (profile.TryGetProperty("caloriesDelta", out var c) && c.ValueKind == JsonValueKind.Number)
                        caloriesDelta = c.GetInt32();
                }
            }
            catch { }

            var fatigueResult = _fatigueService.CalculateFatigue(logs, caloriesDelta);

            var currentWeekEnd = now.Date;
            var previousWeekStart = now.Date.AddDays(-14);
            var previousWeekEnd = now.Date.AddDays(-7);

            var allUserExercises = await _context.UserExercise
                .Where(ue => ue.UserId == userId && ue.Date >= previousWeekStart && ue.Date < currentWeekEnd)
                .Join(_context.Exercises,
                    ue => ue.ExerciseId,
                    e => e.Id,
                    (ue, e) => new { ue, e })
                .SelectMany(
                    ue_e => ue_e.e.MuscleMappings,
                    (ue_e, em) => new
                    {
                        Week = ue_e.ue.Date < previousWeekEnd ? "previous" : "current",
                        em.Muscle.NameKey,
                        em.Muscle.NamePl,
                        em.Muscle.IsFront,
                        ue_e.ue.Sets,
                        ue_e.ue.Weight
                    })
                .ToListAsync();

            var currentWeekData = allUserExercises.Where(x => x.Week == "current").ToList();
            var previousWeekData = allUserExercises.Where(x => x.Week == "previous").ToList();

            var currentVolumeByMuscle = currentWeekData
                .GroupBy(x => x.NameKey)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Sets ?? 0));

            var previousVolumeByMuscle = previousWeekData
                .GroupBy(x => x.NameKey)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Sets ?? 0));

            var currentAvgWeightByMuscle = currentWeekData
                .Where(x => x.Weight.HasValue)
                .GroupBy(x => x.NameKey)
                .ToDictionary(g => g.Key, g => g.Average(x => x.Weight ?? 0));

            var previousAvgWeightByMuscle = previousWeekData
                .Where(x => x.Weight.HasValue)
                .GroupBy(x => x.NameKey)
                .ToDictionary(g => g.Key, g => g.Average(x => x.Weight ?? 0));

            var dashboardResult = fatigueResult.Select(muscle =>
            {
                currentVolumeByMuscle.TryGetValue(muscle.NameKey, out var currentVol);
                previousVolumeByMuscle.TryGetValue(muscle.NameKey, out var previousVol);
                currentAvgWeightByMuscle.TryGetValue(muscle.NameKey, out var currentAvgW);
                previousAvgWeightByMuscle.TryGetValue(muscle.NameKey, out var previousAvgW);

                decimal? weightProgress = null;
                if (previousAvgW > 0 && currentAvgW > 0)
                    weightProgress = Math.Round((currentAvgW - previousAvgW) / previousAvgW * 100, 1);

                return new
                {
                    muscle.NameKey,
                    muscle.NamePl,
                    muscle.Volume,
                    muscle.FatiguePercentage,
                    muscle.IsFront,
                    CurrentWeekVolume = currentVol,
                    PreviousWeekVolume = previousVol,
                    WeightProgressDeltaPercentage = weightProgress
                };
            }).ToList();

            string? balanceWarning = null;
            var chestVolume = currentVolumeByMuscle.GetValueOrDefault("chest_main", 0);
            var backVolume = currentVolumeByMuscle.GetValueOrDefault("lats", 0) +
                             currentVolumeByMuscle.GetValueOrDefault("rhomboids_trapezius", 0) +
                             currentVolumeByMuscle.GetValueOrDefault("lower_back", 0);
            if (chestVolume > 0 && backVolume > 0 && chestVolume > backVolume * 1.5m)
                balanceWarning = "Uwaga: objętość klatki piersiowej znacząco przewyższa objętość pleców. Może to prowadzić do dysbalansu posturalnego.";
            var quadVolume = currentVolumeByMuscle.GetValueOrDefault("quadriceps", 0);
            var hamVolume = currentVolumeByMuscle.GetValueOrDefault("hamstrings", 0);
            if (quadVolume > 0 && hamVolume > 0 && quadVolume > hamVolume * 1.5m)
                balanceWarning = (balanceWarning != null ? balanceWarning + " " : "") +
                    "Uwaga: objętość czwórek znacząco przewyższa objętość dwugłowych uda. Może to prowadzić do dysbalansu mięśniowego.";

            return Ok(new
            {
                Muscles = dashboardResult,
                BalanceWarning = balanceWarning
            });
        }
    }
}
