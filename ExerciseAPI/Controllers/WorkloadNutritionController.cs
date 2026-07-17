using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using ExerciseAPI.Infrastructure;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/workload-nutrition")]
    [Authorize]
    public class WorkloadNutritionController : UserHeaderControllerBase
    {
        private readonly IWorkloadCalculationService _workloadCalculationService;
        private readonly ICarbohydrateScalingService _carbohydrateScalingService;

        public WorkloadNutritionController(
            IWorkloadCalculationService workloadCalculationService,
            ICarbohydrateScalingService carbohydrateScalingService,
            IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _workloadCalculationService = workloadCalculationService;
            _carbohydrateScalingService = carbohydrateScalingService;
        }

        [HttpGet("workload")]
        public async Task<IActionResult> GetWorkload([FromQuery] DateTime? date = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var targetDate = date ?? DateTime.UtcNow.Date;

            var tonnage = await _workloadCalculationService.CalculateTonnage(userId.Value, targetDate);
            var workload = await _workloadCalculationService.CalculateWorkload(userId.Value, targetDate);
            var averageWorkload = await _workloadCalculationService.GetUserAverageWorkload(userId.Value);
            var isAboveAverage = await _workloadCalculationService.IsWorkloadAboveAverage(userId.Value, workload);

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
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var targetDate = date ?? DateTime.UtcNow.Date;

            var recommendedCarbs = await _carbohydrateScalingService.CalculatePostWorkoutCarbs(userId.Value, bodyWeight, targetDate);
            var workload = await _workloadCalculationService.CalculateWorkload(userId.Value, targetDate);
            var isAboveAverage = await _workloadCalculationService.IsWorkloadAboveAverage(userId.Value, workload);
            var acwr = await _carbohydrateScalingService.GetAcwrValue(userId.Value);
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