using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class V106Hu153ImplementosDisponibilidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadDisponible",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadTotal",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadDisponible",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos");

            migrationBuilder.DropColumn(
                name: "CantidadTotal",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos");

            migrationBuilder.DropColumn(
                name: "Estado",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos");
        }
    }
}
