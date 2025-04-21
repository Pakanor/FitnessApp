using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendLogicApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    Brands = table.Column<string>(type: "text", nullable: false),
                    Nutriments_Energy = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_EnergyUnit = table.Column<string>(type: "text", nullable: false),
                    Nutriments_Fat = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Carbs = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Nutriments_Salt = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    Brands = table.Column<string>(type: "text", nullable: false),
                    Grams = table.Column<double>(type: "double precision", nullable: false),
                    Energy = table.Column<double>(type: "double precision", nullable: false),
                    Fat = table.Column<double>(type: "double precision", nullable: false),
                    Sugars = table.Column<double>(type: "double precision", nullable: false),
                    Proteins = table.Column<double>(type: "double precision", nullable: false),
                    Salt = table.Column<double>(type: "double precision", nullable: false),
                    EnergyUnit = table.Column<string>(type: "text", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLogEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductLogEntries");
        }
    }
}
