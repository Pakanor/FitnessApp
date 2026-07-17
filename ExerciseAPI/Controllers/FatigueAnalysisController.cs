using ExerciseAPI.DTOs;
using ExerciseAPI.Interfaces;
using ExerciseAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Infrastructure;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/fatigue-analysis")]
    public class FatigueAnalysisController : UserHeaderControllerBase
    {
        private readonly IAcwrService _acwr;
        private readonly IMuscleRecoveryService _recovery;

        public FatigueAnalysisController(IAcwrService acwr, IMuscleRecoveryService recovery, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _acwr = acwr;
            _recovery = recovery;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Unauthorized();

            var acwr = await _acwr.GetAcwrAsync(userId.Value);
            var recovery = await _recovery.ComputeAsync(userId.Value);

            return Ok(new FatigueAnalysisResponseDto
            {
                Acwr = acwr,
                MuscleRecovery = recovery,
            });
        }
    }
}
