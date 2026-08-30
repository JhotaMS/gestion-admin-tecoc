using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionAdminTECOCApi.Infrastructure.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class FixTipoRevisionIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TipoRevisionId era uuid pero TiposRevision.Id es integer: no podían unirse.
            // USING 1 asigna "Inicio Préstamo" (Id=1) a cualquier fila existente cuyo valor
            // anterior no tenga una conversión numérica válida.
            migrationBuilder.Sql(
                """
                ALTER TABLE "gestionAdminTECOCApiMS"."Prestamos"
                ALTER COLUMN "TipoRevisionId" TYPE integer USING 1
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "gestionAdminTECOCApiMS"."Prestamos"
                ALTER COLUMN "TipoRevisionId" TYPE uuid USING gen_random_uuid()
                """);
        }
    }
}
