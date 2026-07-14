using ExerciseAPI.DTOs;
using ExerciseAPI.Interfaces;
using ExerciseAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/fatigue-analysis")]
    public class FatigueAnalysisController : ControllerBase
    {
        private readonly IAcwrService _acwr;
        private readonly IMuscleRecoveryService _recovery;

        public FatigueAnalysisController(IAcwrService acwr, IMuscleRecoveryService recovery)
        {
            _acwr = acwr;
            _recovery = recovery;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var acwr = await _acwr.GetAcwrAsync(userId, Request.Cookies["FitnessApp-Auth"]);
            var recovery = await _recovery.ComputeAsync(userId);

            return Ok(new FatigueAnalysisResponseDto
            {
                Acwr = acwr,
                MuscleRecovery = recovery,
            });
        }
    }
}
