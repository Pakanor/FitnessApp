using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Data;
using ExerciseAPI.Models;
using ExerciseAPI.Services;
using System.Security.Claims;
using System.Text.Json;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/records")]
    [Authorize]
    public class RecordsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RecordsService _recordsService;

        public RecordsController(AppDbContext context, RecordsService recordsService)
        {
            _context = context;
            _recordsService = recordsService;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var records = await _context.PersonalRecords
                .Where(pr => pr.UserId == userId)
                .Join(_context.Exercises,
                    pr => pr.ExerciseId,
                    e => e.Id,
                    (pr, e) => new
                    {
                        pr.Id,
                        ExerciseId = pr.ExerciseId,
                        ExerciseName = e.Name,
                        ExerciseCategory = e.Category,
                        pr.Weight,
                        pr.Reps,
                        pr.Date,
                        pr.UserWeightAtTime,
                        pr.UserAgeAtTime,
                        pr.DietStatusAtTime,
                        pr.StrengthToWeightRatio
                    })
                .OrderByDescending(r => r.Weight)
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<object>());

            int userId = int.Parse(userIdClaim);

            var exercises = await _context.Exercises
                .Where(e => EF.Functions.ILike(e.Name, $"%{query}%"))
                .Take(20)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Category,
                    HasPR = _context.PersonalRecords.Any(pr => pr.UserId == userId && pr.ExerciseId == e.Id)
                })
                .ToListAsync();

            return Ok(exercises);
        }

        [HttpGet("exercise/{exerciseId}")]
        public async Task<IActionResult> GetExerciseHistory(int exerciseId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var records = await _context.PersonalRecords
                .Where(pr => pr.UserId == userId && pr.ExerciseId == exerciseId)
                .Join(_context.Exercises,
                    pr => pr.ExerciseId,
                    e => e.Id,
                    (pr, e) => new
                    {
                        pr.Id,
                        ExerciseId = pr.ExerciseId,
                        ExerciseName = e.Name,
                        pr.Weight,
                        pr.Reps,
                        pr.Date,
                        pr.UserWeightAtTime,
                        pr.UserAgeAtTime,
                        pr.DietStatusAtTime,
                        pr.StrengthToWeightRatio
                    })
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("1rm-progression")]
        public async Task<IActionResult> Get1RMProgression()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var result = await _recordsService.Get1RMProgression(userId);
            return Ok(result);
        }
    }
}
