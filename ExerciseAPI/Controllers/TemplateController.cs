using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExerciseAPI.Interfaces;
using ExerciseAPI.DTOs;
using ExerciseAPI.Models;
using System.Security.Claims;

namespace ExerciseAPI.Controllers
{
    [ApiController]
    [Route("api/templates")]
    [Authorize]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var template = await _templateService.CreateTemplate(userId, dto.Name, dto.ExerciseIds);
                var response = MapToResponseDto(template);
                return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var template = await _templateService.GetTemplateById(id, userId);
            if (template == null)
                return NotFound();

            var response = MapToResponseDto(template);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTemplates()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var templates = await _templateService.GetUserTemplates(userId);
            var response = templates.Select(MapToResponseDto).ToList();
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] UpdateTemplateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            try
            {
                var template = await _templateService.UpdateTemplate(id, userId, dto.Name, dto.ExerciseIds);
                var response = MapToResponseDto(template);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var deleted = await _templateService.DeleteTemplate(id, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        private TemplateResponseDto MapToResponseDto(WorkoutTemplate template)
        {
            return new TemplateResponseDto
            {
                Id = template.Id,
                Name = template.Name,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                Exercises = template.TemplateExercises
                    .OrderBy(te => te.Order)
                    .Select(te => new TemplateExerciseDto
                    {
                        ExerciseId = te.ExerciseId,
                        ExerciseName = te.Exercise?.Name ?? string.Empty,
                        Category = te.Exercise?.Category ?? string.Empty,
                        Order = te.Order
                    }).ToList()
            };
        }
    }
}