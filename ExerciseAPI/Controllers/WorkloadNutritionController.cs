using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/workload-nutrition")]
    [Authorize]
    public class WorkloadNutritionController : ControllerBase
    {
        private readonly IWorkloadCalculationService _workloadCalculationService;
        private readonly ICarbohydrateScalingService _carbohydrateScalingService;

        public WorkloadNutritionController(
            IWorkloadCalculationService workloadCalculationService,
            ICarbohydrateScalingService carbohydrateScalingService)
        {
            _workloadCalculationService = workloadCalculationService;
            _carbohydrateScalingService = carbohydrateScalingService;
        }

        [HttpGet("workload")]
        public async Task<IActionResult> GetWorkload([FromQuery] DateTime? date = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var targetDate = date ?? DateTime.UtcNow.Date;

            var tonnage = await _workloadCalculationService.CalculateTonnage(userId, targetDate);
            var workload = await _workloadCalculationService.CalculateWorkload(userId, targetDate);
            var averageWorkload = await _workloadCalculationService.GetUserAverageWorkload(userId);
            var isAboveAverage = await _workloadCalculationService.IsWorkloadAboveAverage(userId, workload);

            return Ok(new
            {
                tonnage,
                workload,
                averageWorkload,
                isAboveAverage,
                date = targetDate
            });
        }

        [HttpGet("carbs")]
        public async Task<IActionResult> GetRecommendedCarbs([FromQuery] decimal bodyWeight, [FromQuery] DateTime? date = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var targetDate = date ?? DateTime.UtcNow.Date;

            var recommendedCarbs = await _carbohydrateScalingService.CalculatePostWorkoutCarbs(userId, bodyWeight, targetDate);
            var workload = await _workloadCalculationService.CalculateWorkload(userId, targetDate);
            var isAboveAverage = await _workloadCalculationService.IsWorkloadAboveAverage(userId, workload);
            var acwr = await _carbohydrateScalingService.GetAcwrValue(userId);
            bool isHeavyWorkout = isAboveAverage || (acwr.HasValue && acwr.Value > _carbohydrateScalingService.AcwrThreshold);

            return Ok(new
            {
                recommendedCarbs,
                isHeavyWorkout,
                isAboveAverage,
                acwr,
                carbMultiplier = isHeavyWorkout ? 1.4m : 1.0m,
                bodyWeight,
                date = targetDate
            });
        }
    }
}