using System.Net.Http.Headers;
using System.Text.Json;
using ExerciseAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace ExerciseAPI.Services
{
    public class ExerciseDbImportService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public ExerciseDbImportService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<int> ImportAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://exercisedb.p.rapidapi.com/exercises?limit=1000"),
                Headers =
                {
                    { "x-rapidapi-key", "b7550e5dcemsh5957bdfba9e4ccap1a2997jsnf861439e9228" },
                    { "x-rapidapi-host", "exercisedb.p.rapidapi.com" },
                }
            };

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var rawExercises = JsonSerializer.Deserialize<List<ExerciseDbDto>>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (rawExercises == null || !rawExercises.Any())
                return 0;

            var exercises = rawExercises.Select(dto => new Exercise
            {
                ExternalId = dto.Id ?? Guid.NewGuid().ToString(),
                Name = string.IsNullOrWhiteSpace(dto.Name) ? "Brak nazwy" : dto.Name,
                Description = dto.Instructions != null ? string.Join("\n", dto.Instructions) : "",
                Category = dto.BodyPart ?? "unknown",
                ImageUrl = null,
                GifUrl = dto.GifUrl
            }).ToList();

            foreach (var exercise in exercises)
            {
                var existing = _context.Exercises.FirstOrDefault(e => e.ExternalId == exercise.ExternalId);
                if (existing != null)
                {
                    existing.Name = exercise.Name;
                    existing.Description = exercise.Description;
                    existing.Category = exercise.Category;
                    existing.ImageUrl = exercise.ImageUrl;
                    existing.GifUrl = exercise.GifUrl;
                }
                else
                {
                    _context.Exercises.Add(exercise);
                }
            }

            await _context.SaveChangesAsync();

            return exercises.Count;
        }
    }
}