using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Data;
using ExerciseAPI.Models;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/records")]
    [Authorize]
    public class RecordsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecordsController(AppDbContext context)
        {
            _context = context;
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
    }
}
