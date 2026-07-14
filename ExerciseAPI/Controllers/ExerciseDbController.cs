using Microsoft.AspNetCore.Mvc;
using ExerciseAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using ExerciseAPI.Data;
using ExerciseAPI.Models;
using ExerciseAPI.DTOs;
using ExerciseAPI.Interfaces;
using System.Text.Json;
namespace ExerciseAPI.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class ExerciseDbController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ExerciseDbImportService _importService;
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMuscleDamageService _muscleDamageService;


        public ExerciseDbController(ExerciseDbImportService importService, AppDbContext context, HttpClient httpClient, IHttpClientFactory httpClientFactory, IMuscleDamageService muscleDamageService)
        {
            _importService = importService;
            _context = context;
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _muscleDamageService = muscleDamageService;


        }
        //[Authorize(Roles = "Admin")]

        [HttpPost("import")]
        public async Task<IActionResult> Import()
        {
            await _importService.ImportExercisesAsync();
            return Ok("Import ćwiczeń zakończony.");
        }


        [HttpDelete("exercises/clear")]
        public async Task<IActionResult> ClearExercises()
        {
            _context.Exercises.RemoveRange(_context.Exercises);
            await _context.SaveChangesAsync();
            return Ok("Wyczyszczono wszystkie ćwiczenia.");
        }
        [HttpGet("exercise")]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _context.Exercises
                .Select(e => new
                {
                    e.Id,
                    e.ExternalId,
                    e.Name,
                    e.Description,
                    e.Category,
                    e.ImageUrl,
                    e.GifUrl,
                    MuscleMappings = _context.ExerciseMuscleGroups
                        .Where(emg => emg.ExerciseId == e.Id)
                        .Select(emg => new
                        {
                            MuscleGroupKey = emg.MuscleGroupKey,
                            WeightPercentage = emg.WeightPercentage
                        })
                        .ToList()
                })
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
        [HttpGet("exercise/{bodyPart}")]
        public async Task<IActionResult> GetExercisesByBodyParts(string bodyPart)
        {
            var exercises = await _context.Exercises
                .Where(e => e.Category.ToLower() == bodyPart.ToLower())
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Category,
                    e.ImageUrl,
                    e.GifUrl,
                    MuscleMappings = _context.ExerciseMuscleGroups
                        .Where(emg => emg.ExerciseId == e.Id)
                        .Select(emg => new
                        {
                            emg.MuscleGroupKey,
                            emg.WeightPercentage
                        })
                        .ToList()
                })
                .ToListAsync();
            return Ok(exercises);
        }
        
        [HttpGet("exercise/id/{id}")]
        public async Task<IActionResult> GetExerciseById(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
                return NotFound();

            var mappings = await _context.ExerciseMuscleGroups
                .Where(emg => emg.ExerciseId == id)
                .Select(emg => new
                {
                    emg.MuscleGroupKey,
                    emg.WeightPercentage
                })
                .ToListAsync();

            return Ok(new
            {
                exercise.Id,
                exercise.ExternalId,
                exercise.Name,
                exercise.Description,
                exercise.Category,
                exercise.ImageUrl,
                exercise.GifUrl,
                MuscleMappings = mappings
            });
        }
        [HttpGet("exercise/search/{term}")]
        public async Task<IActionResult> SearchExercises(string term)
        {
            var exercises = await _context.Exercises
                .Where(e => e.Name.ToLower().Contains(term.ToLower()))
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Category,
                    MuscleMappings = _context.ExerciseMuscleGroups
                        .Where(emg => emg.ExerciseId == e.Id)
                        .Select(emg => new
                        {
                            emg.MuscleGroupKey,
                            emg.WeightPercentage
                        })
                        .ToList()
                })
                .ToListAsync();
            return Ok(exercises);
        }
        [HttpPut("exercise/{id}/mappings")]
        public async Task<IActionResult> UpdateMappings(int id, [FromBody] List<ExerciseAPI.Models.ExerciseMuscleGroup> mappings)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
                return NotFound("Ćwiczenie nie istnieje");

            var existing = await _context.ExerciseMuscleGroups
                .Where(emg => emg.ExerciseId == id)
                .ToListAsync();
            _context.ExerciseMuscleGroups.RemoveRange(existing);

            foreach (var m in mappings)
            {
                m.ExerciseId = id;
                m.Id = 0;
                _context.ExerciseMuscleGroups.Add(m);
            }

            await _context.SaveChangesAsync();
            return Ok("Zapisano");
        }

        [HttpPost("userexercise/add")]
        [Authorize]
        public async Task<IActionResult> AddUserExercise([FromBody] AddUserExerciseDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var exerciseExists = await _context.Exercises.AnyAsync(e => e.Id == dto.ExerciseId);
            if (!exerciseExists)
                return BadRequest("Niepoprawne ćwiczenie");

            if (dto.RPE.HasValue && (dto.RPE < 1 || dto.RPE > 10))
                return BadRequest("RPE musi być w zakresie 1-10");

            if (dto.RIR.HasValue && (dto.RIR < 0 || dto.RIR > 4))
                return BadRequest("RIR musi być w zakresie 0-4");

            var entity = new UserExercise
            {
                UserId = int.Parse(userIdClaim),
                ExerciseId = dto.ExerciseId,
                Sets = dto.Sets,
                Reps = dto.Reps,
                Weight = dto.Weight,
                RPE = dto.RPE,
                RIR = dto.RIR,
                Date = dto.Date.HasValue
                    ? DateTime.SpecifyKind(dto.Date.Value.Date, DateTimeKind.Utc)
                    : DateTime.UtcNow
            };

            _context.UserExercise.Add(entity);
            await _context.SaveChangesAsync();

            if (entity.Weight.HasValue && entity.Reps.HasValue)
            {
                var previousRecord = await _context.PersonalRecords
                    .Where(pr => pr.UserId == entity.UserId && pr.ExerciseId == entity.ExerciseId && pr.Reps == entity.Reps.Value)
                    .OrderByDescending(pr => pr.Weight)
                    .FirstOrDefaultAsync();

                if (previousRecord == null || entity.Weight > previousRecord.Weight)
                {
                    decimal? userWeight = null;
                    int? userAge = null;
                    try
                    {
                        var profileClient = _httpClientFactory.CreateClient();
                        profileClient.BaseAddress = new Uri("http://localhost:5010");
                        var profileResponse = await profileClient.GetAsync($"/api/user/profile");
                        if (profileResponse.IsSuccessStatusCode)
                        {
                            var profileJson = await profileResponse.Content.ReadAsStringAsync();
                            var profile = JsonSerializer.Deserialize<JsonElement>(profileJson);
                            if (profile.TryGetProperty("currentWeight", out var w) && w.ValueKind == JsonValueKind.Number)
                                userWeight = w.GetDecimal();
                            if (profile.TryGetProperty("birthDate", out var b) && b.ValueKind == JsonValueKind.String)
                            {
                                if (DateTime.TryParse(b.GetString(), out var birthDate))
                                    userAge = DateTime.UtcNow.Year - birthDate.Year - (DateTime.UtcNow.DayOfYear < birthDate.DayOfYear ? 1 : 0);
                            }
                        }
                    }
                    catch { }

                    var pr = new PersonalRecord
                    {
                        UserId = entity.UserId,
                        ExerciseId = entity.ExerciseId,
                        Weight = entity.Weight.Value,
                        Reps = entity.Reps.Value,
                        Date = entity.Date,
                        UserWeightAtTime = userWeight,
                        UserAgeAtTime = userAge,
                        StrengthToWeightRatio = userWeight.HasValue && userWeight > 0
                            ? Math.Round(entity.Weight.Value / userWeight.Value, 2)
                            : null
                    };

                    _context.PersonalRecords.Add(pr);
                    await _context.SaveChangesAsync();
                }
            }

            var response = new UserExerciseResponseDto
            {
                Id = entity.Id,
                ExerciseId = entity.ExerciseId,
                Sets = entity.Sets,
                Reps = entity.Reps,
                Weight = entity.Weight,
                RPE = entity.RPE,
                RIR = entity.RIR,
                Date = entity.Date
            };

            return Ok(response);
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
                    rpe = ue.RPE,
                    rir = ue.RIR,
                    templateId = ue.TemplateId,
                    date = ue.Date
                };
            }).ToList();

            return Ok(result);
        }


        [HttpGet("userexercise/byuser")]
        [Authorize]
        public async Task<IActionResult> GetUserExercises()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var exercises = await _context.UserExercise
                .Where(ue => ue.UserId == userId)
                .Select(ue => new { ue.ExerciseId })
                .ToListAsync();

            return Ok(exercises);
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

            // Editing/deleting a session changes the damage baseline -> rebuild it.
            await _muscleDamageService.RecordSessionDamageAsync(userId, entry.Date);

            return NoContent();
        }
}
}