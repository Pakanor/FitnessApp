using Microsoft.EntityFrameworkCore;
using ExerciseAPI.Models;

namespace ExerciseAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<UserExercise> UserExercise { get; set; }
        public DbSet<Muscle> Muscles { get; set; }
        public DbSet<ExerciseMuscleMapping> ExerciseMuscleMappings { get; set; }
        public DbSet<PersonalRecord> PersonalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExerciseMuscleMapping>()
                .HasKey(em => new { em.ExerciseId, em.MuscleId });

            modelBuilder.Entity<ExerciseMuscleMapping>()
                .HasOne(em => em.Exercise)
                .WithMany(e => e.MuscleMappings)
                .HasForeignKey(em => em.ExerciseId);

            modelBuilder.Entity<ExerciseMuscleMapping>()
                .HasOne(em => em.Muscle)
                .WithMany()
                .HasForeignKey(em => em.MuscleId);

            modelBuilder.Entity<Muscle>().HasData(
                new Muscle { Id = 1, NameKey = "chest_main", NamePl = "Klatka piersiowa", IsFront = true },
                new Muscle { Id = 2, NameKey = "deltoid_anterior", NamePl = "Bark przedni", IsFront = true },
                new Muscle { Id = 3, NameKey = "deltoid_lateral", NamePl = "Bark boczny", IsFront = true },
                new Muscle { Id = 4, NameKey = "biceps", NamePl = "Biceps", IsFront = true },
                new Muscle { Id = 5, NameKey = "forearms", NamePl = "Przedramiona", IsFront = true },
                new Muscle { Id = 6, NameKey = "abs", NamePl = "Brzuch", IsFront = true },
                new Muscle { Id = 7, NameKey = "quadriceps", NamePl = "Czwórki", IsFront = true },
                new Muscle { Id = 8, NameKey = "core_stabilizers", NamePl = "Stabilizatory tułowia", IsFront = true },
                new Muscle { Id = 9, NameKey = "lats", NamePl = "Plecy szerokie", IsFront = false },
                new Muscle { Id = 10, NameKey = "lower_back", NamePl = "Dolny odcinek pleców", IsFront = false },
                new Muscle { Id = 11, NameKey = "rhomboids_trapezius", NamePl = "Romby i czworoboczny", IsFront = false },
                new Muscle { Id = 12, NameKey = "deltoid_posterior", NamePl = "Bark tylny", IsFront = false },
                new Muscle { Id = 13, NameKey = "triceps", NamePl = "Triceps", IsFront = false },
                new Muscle { Id = 14, NameKey = "glutes", NamePl = "Pośladki", IsFront = false },
                new Muscle { Id = 15, NameKey = "hamstrings", NamePl = "Dwugłowe uda", IsFront = false },
                new Muscle { Id = 16, NameKey = "calves", NamePl = "Łydki", IsFront = false }
            );
        }
    }
}