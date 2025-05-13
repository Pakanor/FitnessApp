namespace ExerciseAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string? ImageUrl { get; set; }
        public string? GifUrl { get; set; }  // Nowe pole na URL do gifa
    }
}