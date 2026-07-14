using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExerciseAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    GifUrl = table.Column<string>(type: "text", nullable: true),
                    ChestMain = table.Column<decimal>(type: "numeric", nullable: false),
                    DeltoidAnterior = table.Column<decimal>(type: "numeric", nullable: false),
                    DeltoidLateral = table.Column<decimal>(type: "numeric", nullable: false),
                    DeltoidPosterior = table.Column<decimal>(type: "numeric", nullable: false),
                    Biceps = table.Column<decimal>(type: "numeric", nullable: false),
                    Triceps = table.Column<decimal>(type: "numeric", nullable: false),
                    Forearms = table.Column<decimal>(type: "numeric", nullable: false),
                    Lats = table.Column<decimal>(type: "numeric", nullable: false),
                    Rhomboids = table.Column<decimal>(type: "numeric", nullable: false),
                    LowerBack = table.Column<decimal>(type: "numeric", nullable: false),
                    Abs = table.Column<decimal>(type: "numeric", nullable: false),
                    CoreStabilizers = table.Column<decimal>(type: "numeric", nullable: false),
                    Quadriceps = table.Column<decimal>(type: "numeric", nullable: false),
                    Hamstrings = table.Column<decimal>(type: "numeric", nullable: false),
                    Glutes = table.Column<decimal>(type: "numeric", nullable: false),
                    Calves = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserWeightAtTime = table.Column<decimal>(type: "numeric", nullable: true),
                    UserAgeAtTime = table.Column<int>(type: "integer", nullable: true),
                    DietStatusAtTime = table.Column<int>(type: "integer", nullable: true),
                    StrengthToWeightRatio = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserExercise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: true),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    RPE = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExercise", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "PersonalRecords");

            migrationBuilder.DropTable(
                name: "UserExercise");
        }
    }
}
