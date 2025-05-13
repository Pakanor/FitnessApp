using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _context.Exercises.ToListAsync();
            return Ok(exercises);
        }

    }
}
