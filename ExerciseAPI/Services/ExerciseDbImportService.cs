using System.Net.Http.Headers;
using System.Text.Json;
using ExerciseAPI.Models;
using Microsoft.EntityFrameworkCore;
using ExerciseAPI.Data;

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

            var gifsFolder = Path.Combine("wwwroot", "gifs");
            Directory.CreateDirectory(gifsFolder);

            foreach (var dto in rawExercises)
            {
                var existing = _context.Exercises.FirstOrDefault(e => e.ExternalId == dto.Id);
                
                // Pobierz gif lokalnie
                string? localGifUrl = null;
                if (!string.IsNullOrEmpty(dto.GifUrl))
                {
                    try
                    {
                        var gifFileName = $"{dto.Id}.gif";
                        var gifPath = Path.Combine(gifsFolder, gifFileName);

                        if (!File.Exists(gifPath))
                        {
                            var gifRequest = new HttpRequestMessage(HttpMethod.Get, dto.GifUrl);
                            gifRequest.Headers.Add("x-rapidapi-key", "b7550e5dcemsh5957bdfba9e4ccap1a2997jsnf861439e9228");
                            gifRequest.Headers.Add("x-rapidapi-host", "exercisedb.p.rapidapi.com");

                            var gifResponse = await _httpClient.SendAsync(gifRequest);
                            if (gifResponse.IsSuccessStatusCode)
                            {
                                var gifBytes = await gifResponse.Content.ReadAsByteArrayAsync();
                                await File.WriteAllBytesAsync(gifPath, gifBytes);
                            }
                        }

                        localGifUrl = $"/gifs/{gifFileName}";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd pobierania gifa dla {dto.Id}: {ex.Message}");
                    }
                }

                if (existing != null)
                {
                    existing.Name = string.IsNullOrWhiteSpace(dto.Name) ? "Brak nazwy" : dto.Name;
                    existing.Description = dto.Instructions != null ? string.Join("\n", dto.Instructions) : "";
                    existing.Category = dto.BodyPart ?? "unknown";
                    existing.GifUrl = localGifUrl ?? existing.GifUrl;
                }
                else
                {
                    _context.Exercises.Add(new Exercise
                    {
                        ExternalId = dto.Id ?? Guid.NewGuid().ToString(),
                        Name = string.IsNullOrWhiteSpace(dto.Name) ? "Brak nazwy" : dto.Name,
                        Description = dto.Instructions != null ? string.Join("\n", dto.Instructions) : "",
                        Category = dto.BodyPart ?? "unknown",
                        ImageUrl = null,
                        GifUrl = localGifUrl
                    });
                }
            }

            await _context.SaveChangesAsync();
            return rawExercises.Count;
        }
    }
}