using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations;
/// <inheritdoc />
public partial class v101 : Migration {
    /// <inheritdoc />
    protected override void Up( MigrationBuilder migrationBuilder ) {
        migrationBuilder.DropColumn(
            name: "FirstName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "SecondName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "SecondSurName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "SurName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.AddColumn<string>(
            name: "DocumentNumber",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "character varying(15)",
            maxLength: 15,
            nullable: false,
            defaultValue: "" );

        migrationBuilder.AddColumn<string>(
            name: "DocumentType",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "character varying(40)",
            maxLength: 40,
            nullable: false,
            defaultValue: "" );

        migrationBuilder.AddColumn<string>(
            name: "FullName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "character varying(150)",
            maxLength: 150,
            nullable: false,
            defaultValue: "" );

        migrationBuilder.AddColumn<string>(
            name: "Position",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "" );

        migrationBuilder.CreateIndex(
            name: "IX_Users_DocumentType_DocumentNumber",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            columns: new[] { "DocumentType", "DocumentNumber" },
            unique: true );
    }

    /// <inheritdoc />
    protected override void Down( MigrationBuilder migrationBuilder ) {
        migrationBuilder.DropIndex(
            name: "IX_Users_DocumentType_DocumentNumber",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "DocumentNumber",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "DocumentType",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "FullName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.DropColumn(
            name: "Position",
            schema: "gestionAdminTECOCApiMS",
            table: "Users" );

        migrationBuilder.AddColumn<string>(
            name: "FirstName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "" );

        migrationBuilder.AddColumn<string>(
            name: "SecondName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "text",
            nullable: true );

        migrationBuilder.AddColumn<string>(
            name: "SecondSurName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "text",
            nullable: true );

        migrationBuilder.AddColumn<string>(
            name: "SurName",
            schema: "gestionAdminTECOCApiMS",
            table: "Users",
            type: "text",
            nullable: false,
            defaultValue: "" );
    }
}
