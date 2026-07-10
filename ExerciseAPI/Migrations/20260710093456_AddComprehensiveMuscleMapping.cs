using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExerciseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddComprehensiveMuscleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameKey = table.Column<string>(type: "text", nullable: false),
                    NamePl = table.Column<string>(type: "text", nullable: false),
                    IsFront = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseMuscleMappings",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    MuscleId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Factor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseMuscleMappings", x => new { x.ExerciseId, x.MuscleId });
                    table.ForeignKey(
                        name: "FK_ExerciseMuscleMappings_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscleMappings_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Muscles",
                columns: new[] { "Id", "IsFront", "NameKey", "NamePl" },
                values: new object[,]
                {
                    { 1, true, "chest_main", "Klatka piersiowa" },
                    { 2, true, "deltoid_anterior", "Bark przedni" },
                    { 3, true, "deltoid_lateral", "Bark boczny" },
                    { 4, true, "biceps", "Biceps" },
                    { 5, true, "forearms", "Przedramiona" },
                    { 6, true, "abs", "Brzuch" },
                    { 7, true, "quadriceps", "Czwórki" },
                    { 8, true, "core_stabilizers", "Stabilizatory tułowia" },
                    { 9, false, "lats", "Plecy szerokie" },
                    { 10, false, "lower_back", "Dolny odcinek pleców" },
                    { 11, false, "rhomboids_trapezius", "Romby i czworoboczny" },
                    { 12, false, "deltoid_posterior", "Bark tylny" },
                    { 13, false, "triceps", "Triceps" },
                    { 14, false, "glutes", "Pośladki" },
                    { 15, false, "hamstrings", "Dwugłowe uda" },
                    { 16, false, "calves", "Łydki" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscleMappings_MuscleId",
                table: "ExerciseMuscleMappings",
                column: "MuscleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseMuscleMappings");

            migrationBuilder.DropTable(
                name: "Muscles");
        }
    }
}
