using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class V105Hu165UserGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Code",
                schema: "gestionAdminTECOCApiMS",
                table: "Groups",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users",
                column: "GroupId",
                principalSchema: "gestionAdminTECOCApiMS",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "gestionAdminTECOCApiMS");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupId",
                schema: "gestionAdminTECOCApiMS",
                table: "Users");
        }
    }
}
