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

    }
}