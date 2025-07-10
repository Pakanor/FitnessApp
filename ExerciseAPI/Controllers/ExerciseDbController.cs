using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

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
        [Authorize(Roles = "Admin")]

        [HttpPost("import")]
        public async Task<IActionResult> Import()
        {
            var firstImported = await _importService.ImportAsync();

            if (firstImported == null)
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




    }
}
