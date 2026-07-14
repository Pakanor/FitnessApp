using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using ExerciseAPI.DTOs;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/workout-start")]
    [Authorize]
    public class WorkoutStartController : ControllerBase
    {
        private readonly IWorkoutStartModeService _workoutStartModeService;
        private readonly ITemplateService _templateService;

        public WorkoutStartController(IWorkoutStartModeService workoutStartModeService, ITemplateService templateService)
        {
            _workoutStartModeService = workoutStartModeService;
            _templateService = templateService;
        }

        [HttpGet("previous")]
        public async Task<IActionResult> GetPreviousWorkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var exercises = await _workoutStartModeService.GetPreviousWorkoutExercises(userId);
            if (!exercises.Any())
                return NotFound("No previous workouts to copy");

            var response = exercises.Select(e => new
            {
                e.ExerciseId,
                ExerciseName = e.Exercise?.Name ?? string.Empty,
                Category = e.Exercise?.Category ?? string.Empty,
                e.Sets,
                e.Reps,
                e.Weight,
                e.RPE,
                e.RIR
            }).ToList();

            return Ok(response);
        }

        // Type-safe: returns the most recent COMPLETED workout for the given template
        // so the UI can render "Poprzednio" hints matched by ExerciseId.
        [HttpGet("previous-by-template/{templateId}")]
        public async Task<IActionResult> GetPreviousWorkoutByTemplate(int templateId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var exercises = await _workoutStartModeService.GetPreviousWorkoutByTemplate(userId, templateId);
            if (!exercises.Any())
                return NotFound("No previous workouts for this template");

            var response = exercises.Select(e => new
            {
                e.ExerciseId,
                ExerciseName = e.Exercise?.Name ?? string.Empty,
                e.Sets,
                e.Reps,
                e.Weight,
                e.RPE,
                e.RIR
            }).ToList();

            return Ok(response);
        }

        [HttpPost("from-template/{templateId}")]
        public async Task<IActionResult> StartFromTemplate(int templateId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var exercises = await _workoutStartModeService.CreateWorkoutFromTemplate(userId, templateId);
                return Ok(new { message = "Workout created from template", exerciseCount = exercises.Count });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("copy-previous")]
        public async Task<IActionResult> CopyPreviousWorkout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var exercises = await _workoutStartModeService.CopyPreviousWorkout(userId);
                return Ok(new { message = "Workout copied from previous session", exerciseCount = exercises.Count });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Type-safe copy: copies the most recent COMPLETED workout of the given template.
        [HttpPost("copy-previous/{templateId}")]
        public async Task<IActionResult> CopyPreviousWorkoutByTemplate(int templateId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var exercises = await _workoutStartModeService.CopyPreviousWorkout(userId, templateId);
                return Ok(new { message = "Workout copied from previous session for template", exerciseCount = exercises.Count });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("templates")]
        public async Task<IActionResult> GetUserTemplates()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var templates = await _templateService.GetUserTemplates(userId);
            var response = templates.Select(t => new
            {
                t.Id,
                t.Name,
                ExerciseCount = t.TemplateExercises.Count,
                t.CreatedAt
            }).ToList();

            return Ok(response);
        }
    }
}