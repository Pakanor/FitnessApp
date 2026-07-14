using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExerciseAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRirAndDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RIR",
                table: "UserExercise",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBenchmark",
                table: "Exercises",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserExercise_Date",
                table: "UserExercise",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_UserExercise_UserId_Date",
                table: "UserExercise",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserExercise_Date",
                table: "UserExercise");

            migrationBuilder.DropIndex(
                name: "IX_UserExercise_UserId_Date",
                table: "UserExercise");

            migrationBuilder.DropColumn(
                name: "RIR",
                table: "UserExercise");

            migrationBuilder.DropColumn(
                name: "IsBenchmark",
                table: "Exercises");
        }
    }
}
