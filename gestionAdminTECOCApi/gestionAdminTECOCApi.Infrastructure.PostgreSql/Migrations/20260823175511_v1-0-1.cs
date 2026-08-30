using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class v101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledClasses",
                schema: "gestionAdminTECOCApiMS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ScheduledTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Topic = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CourseLevel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledClasses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledClasses_ScheduledDate_ScheduledTime",
                schema: "gestionAdminTECOCApiMS",
                table: "ScheduledClasses",
                columns: new[] { "ScheduledDate", "ScheduledTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledClasses",
                schema: "gestionAdminTECOCApiMS");
        }
    }
}
