using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using ExerciseAPI.Data;
using ExerciseAPI.Models;
namespace ExerciseAPI.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class ExerciseDbController : ControllerBase
    {
        private readonly ExerciseDbImportService _importService;
        private readonly AppDbContext _context;


        public ExerciseDbController(ExerciseDbImportService importService, AppDbContext context)
        {
            _importService = importService;
            _context = context;
        }
        //[Authorize(Roles = "Admin")]

        [HttpPost("import")]
        public async Task<IActionResult> Import()
        {
            var firstImported = await _importService.ImportAsync();

            if (firstImported == 0)
                return NotFound(new { message = "Nie udało się zaimportować żadnych ćwiczeń." });

            var count = await _importService.ImportAsync();
            return Ok($"{count} ćwiczeń zostało zaimportowanych.");
        }
        [HttpGet("exercise")]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _context.Exercises.ToListAsync();
            return Ok(exercises);
        }
        [HttpGet("exercise/{bodyPart}")]
        public async Task<IActionResult> GetExercisesByBodyParts(string bodyPart)
        {
            var exercises = await _context.Exercises
                .Where(e => e.Category.ToLower() == bodyPart.ToLower())
                .ToListAsync();
            return Ok(exercises);
        }
        [HttpGet("exercise/categories")]
        public async Task<IActionResult> GetExerciseCategories()
        {
            var categories = await _context.Exercises
            .Select(e => e.Category)
            .Distinct()
            .ToListAsync();
            return Ok(categories);
        }
        [HttpGet("exercise/id/{id}")]
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            return Ok(exercise);
        }
        [HttpGet("exercise/search/{term}")]
        public async Task<IActionResult> SearchExercises(string term)
        {
            var exercise = await _context.Exercises
                .Where(e => e.Name.ToLower().Contains(term.ToLower()))
                .ToListAsync();
            return Ok(exercise);

        }
        [HttpPost("userexercise/add")]
        [Authorize]
        public async Task<IActionResult> AddUserExercise([FromBody] UserExercise model)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            model.Date = DateTime.SpecifyKind(model.Date.Date, DateTimeKind.Utc);

            model.UserId = int.Parse(userIdClaim);
            var exerciseExists = await _context.Exercises.AnyAsync(e => e.Id == model.ExerciseId);
            if (!exerciseExists)
                return BadRequest("Niepoprawne ćwiczenie");

            _context.UserExercise.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }


        [HttpGet("userexercise/bydate")]
        [Authorize]
        public async Task<IActionResult> GetExercisesByDate([FromQuery] string date)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Nieprawidłowy format daty");

            var all = await _context.UserExercise
                .Where(ue => ue.UserId == userId)
                .ToListAsync();

            var filtered = all
            .Where(ue => ue.Date.Date == parsedDate.Date) 
            .ToList();

            var exerciseIds = filtered.Select(ue => ue.ExerciseId).Distinct().ToList();

            var exercises = await _context.Exercises
                .Where(e => exerciseIds.Contains(e.Id))
                .ToListAsync();

            var result = filtered.Select(ue => {
                var exercise = exercises.FirstOrDefault(e => e.Id == ue.ExerciseId);
                return new {
                    userExerciseId = ue.Id,
                    exerciseId = ue.ExerciseId,
                    name = exercise?.Name ?? "Nieznane",
                    category = exercise?.Category ?? "",
                    gifUrl = exercise?.GifUrl ?? "",
                    sets = ue.Sets,
                    reps = ue.Reps,
                    weight = ue.Weight,
                    date = ue.Date
                };
            }).ToList();

            return Ok(result);
        }


       [HttpDelete("userexercise/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserExercise(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var entry = await _context.UserExercise.FindAsync(id);

            if (entry == null || entry.UserId != userId)
                return NotFound();

            _context.UserExercise.Remove(entry);
            await _context.SaveChangesAsync();
            return NoContent();
        }
}
}