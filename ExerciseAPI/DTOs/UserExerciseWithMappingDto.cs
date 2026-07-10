namespace ExerciseAPI.DTOs
{
public class UserExerciseWithMappingDto
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public int Sets { get; set; }
    public int MuscleId { get; set; }
    public string MuscleNameKey { get; set; }
    public string MuscleNamePl { get; set; }
    public decimal Factor { get; set; }
    public bool IsFront { get; set; }
    public int? RPE { get; set; }
}
}
