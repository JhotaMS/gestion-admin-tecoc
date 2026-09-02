using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class V107Hu175UsuarioGrupoPrograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramasAcademicos",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasAcademicos", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAcademicos_Code",
                schema: "gestionAdminTECOCApiMS",
                table: "ProgramasAcademicos",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                column: "ProgramaAcademicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ProgramasAcademicos_ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                column: "ProgramaAcademicoId",
                principalSchema: "gestionAdminTECOCApiMS",
                principalTable: "ProgramasAcademicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ProgramasAcademicos_ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ProgramasAcademicos",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProgramaAcademicoId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");
        }
    }
}
