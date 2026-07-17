using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using ExerciseAPI.Models;
using ExerciseAPI.Infrastructure;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/workout-status")]
    [Authorize]
    public class WorkoutStatusController : UserHeaderControllerBase
    {
        private readonly IWorkoutStatusService _workoutStatusService;
        private readonly IMuscleDamageService _muscleDamageService;

        public WorkoutStatusController(IWorkoutStatusService workoutStatusService, IMuscleDamageService muscleDamageService, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _workoutStatusService = workoutStatusService;
            _muscleDamageService = muscleDamageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkoutStatus([FromQuery] DateTime? date = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var targetDate = date ?? DateTime.UtcNow.Date;

            var status = await _workoutStatusService.GetWorkoutStatus(userId.Value, targetDate);
            return Ok(new { status = status?.ToString() ?? "None" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWorkoutStatus([FromBody] UpdateWorkoutStatusDto dto)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            if (!Enum.TryParse<WorkoutStatus>(dto.Status, out var status))
                return BadRequest("Invalid status");

            await _workoutStatusService.UpdateWorkoutStatus(userId.Value, dto.Date, status);

            // A newly completed session changes the damage baseline -> rebuild it.
            if (status == WorkoutStatus.Completed)
                await _muscleDamageService.RecordSessionDamageAsync(userId.Value, dto.Date);

            return Ok();
        }

        [HttpGet("active")]
        public async Task<IActionResult> IsWorkoutActive([FromQuery] DateTime? date = null)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var targetDate = date ?? DateTime.UtcNow.Date;

            var isActive = await _workoutStatusService.IsWorkoutActive(userId.Value, targetDate);
            return Ok(new { isActive });
        }
    }

    public class UpdateWorkoutStatusDto
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}