using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class V105Hu153Implementos : Migration
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
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CantidadTotal = table.Column<int>(type: "integer", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Implementos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Implementos_Codigo",
                schema: "gestionAdminTECOCApiMS",
                table: "Implementos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Implementos",
                schema: "gestionAdminTECOCApiMS");
        }
    }
}
