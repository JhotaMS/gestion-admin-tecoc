using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class v100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gestionAdminTECOCApiMS");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DocumentType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecasts",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureC = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Temperature = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecastsHistories",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Proccess = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedByUser = table.Column<string>(type: "text", nullable: true),
                    LastModifiedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastModifiedByUser = table.Column<string>(type: "text", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecastsHistories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentType_DocumentNumber",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                columns: new[] { "DocumentType", "DocumentNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropTable(
                name: "WeatherForecasts",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropTable(
                name: "WeatherForecastsHistories",
                schema: "gestionAdminTECOCApiMS");
        }
    }
}
