using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class V1_0_4_ImplementosPrestados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Implementos",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Implementos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImplementosPrestados",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImplementoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoRevisionId = table.Column<int>(type: "integer", nullable: false),
                    EstadoTipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImplementosPrestados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposRevision",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposRevision", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "gestionAdminTECOCApiMS",
                table: "TiposRevision",
                columns: new[] { "Id", "Descripcion", "Enabled", "Nombre" },
                values: new object[,]
                {
                    { 1, "Revisión al inicio del préstamo del implemento", true, "Inicio Préstamo" },
                    { 2, "Revisión al finalizar el préstamo del implemento", true, "Fin Préstamo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Implementos_Codigo",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImplementosPrestados_FechaInicio",
                schema: "gestionAdminTECOCApiMS",
                table: "ImplementosPrestados",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementosPrestados_ImplementoId",
                schema: "gestionAdminTECOCApiMS",
                table: "ImplementosPrestados",
                column: "ImplementoId");

            migrationBuilder.CreateIndex(
                name: "IX_ImplementosPrestados_UserId",
                schema: "gestionAdminTECOCApiMS",
                table: "ImplementosPrestados",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Implementos",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropTable(
                name: "ImplementosPrestados",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropTable(
                name: "TiposRevision",
                schema: "gestionAdminTECOCApiMS");
        }
    }
}
