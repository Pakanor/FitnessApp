using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExerciseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBiomechanicsAndHeatmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MuscleGroups",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    NamePl = table.Column<string>(type: "text", nullable: false),
                    IsFront = table.Column<bool>(type: "boolean", nullable: false),
                    HalfLife = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleGroups", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseMuscleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    MuscleGroupKey = table.Column<string>(type: "text", nullable: false),
                    WeightPercentage = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseMuscleGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscleGroups_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscleGroups_MuscleGroups_MuscleGroupKey",
                        column: x => x.MuscleGroupKey,
                        principalTable: "MuscleGroups",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MuscleGroups",
                columns: new[] { "Key", "HalfLife", "IsFront", "NamePl" },
                values: new object[,]
                {
                    { "abs", 24.0, true, "Brzuch" },
                    { "biceps", 30.0, true, "Biceps" },
                    { "calves", 24.0, false, "Łydki" },
                    { "chest_main", 42.0, true, "Klatka piersiowa" },
                    { "core_stabilizers", 24.0, true, "Stabilizatory tułowia" },
                    { "deltoid_anterior", 30.0, true, "Bark przedni" },
                    { "deltoid_lateral", 30.0, true, "Bark boczny" },
                    { "deltoid_posterior", 30.0, false, "Bark tylny" },
                    { "forearms", 24.0, true, "Przedramiona" },
                    { "glutes", 42.0, false, "Pośladki" },
                    { "hamstrings", 30.0, false, "Dwugłowe uda" },
                    { "lats", 42.0, false, "Plecy szerokie" },
                    { "lower_back", 30.0, false, "Dolny odcinek pleców" },
                    { "quadriceps", 42.0, true, "Czwórki" },
                    { "rhomboids", 30.0, false, "Romby i czworoboczny" },
                    { "triceps", 30.0, false, "Triceps" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscleGroups_ExerciseId",
                table: "ExerciseMuscleGroups",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscleGroups_MuscleGroupKey",
                table: "ExerciseMuscleGroups",
                column: "MuscleGroupKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseMuscleGroups");

            migrationBuilder.DropTable(
                name: "MuscleGroups");
        }
    }
}
