using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using ExerciseAPI.Models;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/workout-status")]
    [Authorize]
    public class WorkoutStatusController : ControllerBase
    {
        private readonly IWorkoutStatusService _workoutStatusService;

        public WorkoutStatusController(IWorkoutStatusService workoutStatusService)
        {
            _workoutStatusService = workoutStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkoutStatus([FromQuery] DateTime? date = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var targetDate = date ?? DateTime.UtcNow.Date;

            var status = await _workoutStatusService.GetWorkoutStatus(userId, targetDate);
            return Ok(new { status = status?.ToString() ?? "None" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWorkoutStatus([FromBody] UpdateWorkoutStatusDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            if (!Enum.TryParse<WorkoutStatus>(dto.Status, out var status))
                return BadRequest("Invalid status");

            await _workoutStatusService.UpdateWorkoutStatus(userId, dto.Date, status);
            return Ok();
        }

        [HttpGet("active")]
        public async Task<IActionResult> IsWorkoutActive([FromQuery] DateTime? date = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var targetDate = date ?? DateTime.UtcNow.Date;

            var isActive = await _workoutStatusService.IsWorkoutActive(userId, targetDate);
            return Ok(new { isActive });
        }
    }

    public class UpdateWorkoutStatusDto
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}