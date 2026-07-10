namespace ExerciseAPI.Models
{
    public class ExerciseMuscleMapping
    {
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        public int MuscleId { get; set; }
        public Muscle Muscle { get; set; }
        public string Role { get; set; }
        public decimal Factor { get; set; }
    }
}
