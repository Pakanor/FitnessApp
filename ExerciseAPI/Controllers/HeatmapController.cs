using ExerciseAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [Route("api/heatmap")]
    [ApiController]
    [Authorize]
    public class HeatmapController : ControllerBase
    {
        private readonly HeatmapService _heatmapService;

        public HeatmapController(HeatmapService heatmapService)
        {
            _heatmapService = heatmapService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHeatmap()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var result = await _heatmapService.GetHeatmapData(userId);
            return Ok(result);
        }
    }
}
