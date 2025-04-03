using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    Brands = table.Column<string>(type: "text", nullable: false),
                    EnergyValue = table.Column<string>(type: "text", nullable: false),
                    Nutriments_Energy = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_EnergyUnit = table.Column<string>(type: "text", nullable: false),
                    Nutriments_Fat = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Sugars = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Salt = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
