using ExerciseAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Infrastructure;

namespace ExerciseAPI.Controllers
{
    [Route("api/heatmap")]
    [ApiController]
    [Authorize]
    public class HeatmapController : UserHeaderControllerBase
    {
        private readonly HeatmapService _heatmapService;

        public HeatmapController(HeatmapService heatmapService, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _heatmapService = heatmapService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHeatmap()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return Unauthorized();

            var result = await _heatmapService.GetHeatmapData(userId.Value);
            return Ok(result);
        }
    }
}
