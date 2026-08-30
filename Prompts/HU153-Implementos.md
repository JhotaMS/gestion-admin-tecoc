# HU153 — Obtener los implementos

| | |
|---|---|
| **Historia** | HU153 |
| **Componente** | `gestionAdminTECOCApi` (backend .NET 10) |
| **Rama** | `feature-HU153-Implementos` (desde `develop`) |

## Historia de usuario

> **Como** encargado del préstamo de implementos,
> **quiero** consultar desde el sistema el listado de los implementos registrados
> con su código, nombre, descripción, cantidades total y disponible, estado y si
> están activos,
> **para** saber qué puedo prestar, en qué condición está cada equipo y cuántas
> unidades quedan antes de atender una solicitud.

## Campos del implemento

| Campo | Contrato | Regla |
|---|---|---|
| Identificador | `implementoId` | `Guid`, generado por el sistema |
| Código | `codigo` | Obligatorio, único, máximo 20 caracteres |
| Nombre | `nombre` | Obligatorio, máximo 100 caracteres |
| Descripción | `descripcion` | Obligatorio, máximo 250 caracteres |
| Cantidad total | `cantidadTotal` | Entero, unidades que posee la institución |
| Cantidad disponible | `cantidadDisponible` | Entero, unidades no prestadas |
| Estado | `estado` | Obligatorio, texto libre, máximo 50 caracteres |
| Activo | `activo` | Booleano |

## Criterios de aceptación

1. El endpoint devuelve la lista completa de los implementos registrados.
2. Cada implemento incluye identificador, código, nombre, descripción, cantidad
   total, cantidad disponible, estado y si está activo.
3. Si no hay implementos registrados, responde `200` con una lista vacía, no un
   error.
4. La lista se devuelve ordenada alfabéticamente por nombre.
5. El código de cada implemento es único en el sistema.
6. La consulta devuelve también los implementos marcados como no activos; filtrar
   por ese campo es responsabilidad de quien consume el endpoint.

## Contrato del endpoint

`GET /api/v1/Implemento`

Respuesta `200 OK`:

```json
{
  "implementos": [
    {
      "implementoId": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
      "codigo": "MT-014",
      "nombre": "Multímetro digital",
      "descripcion": "Multímetro digital de gama media",
      "cantidadTotal": 8,
      "cantidadDisponible": 3,
      "estado": "Bueno",
      "activo": true
    }
  ]
}
```

## Archivos

| Capa | Archivo |
|---|---|
| Domain | `gestionAdminTECOCApi.Domain/Implementos/Implemento.cs` |
| Application | `gestionAdminTECOCApi.Application/Features/Implementos/GetAllImplementos/GetAllImplementosQuery.cs` |
| Infrastructure | `gestionAdminTECOCApi.Infrastructure.PostgreSql/Configurations/ImplementoConfiguration.cs` |
| Infrastructure | `Migrations/20260830154520_V1-0-5-Hu153Implementos.cs` (+ `.Designer.cs`) |
| Api | `gestionAdminTECOCApi.Api/Controllers/ImplementoController.cs` |
| Tests | `gestionAdminTECOCApi.Api.Tests/Integration/ImplementoTests.cs` |

## Decisiones de implementación

- **La entidad se llama `Implemento`, en español.** La entidad `Prestamo` que
  entró en `develop` con el PR #25 declara `public Guid ImplementoId`, así que el
  modelo del equipo ya espera ese nombre. Namespace `Domain.Implementos`, tabla
  `Implementos`.
- **`activo` se apoya en el `Enabled` que hereda `Entity<Guid>`.** La clase base
  ya persiste una columna booleana; agregar además una columna `Activo` dejaría
  dos banderas con el mismo significado en la misma tabla. La API expone el campo
  como `activo` y por dentro lee `Enabled`, así que el contrato queda completo con
  una sola columna. El constructor privado recibe el parámetro como `enabled`
  porque EF Core enlaza los parámetros del constructor por nombre de propiedad, y
  con `activo` falla al construir el modelo.
- **`estado` es texto libre**, igual que `EstadoTipo` en `Prestamo`. No se
  restringe a una lista de valores para no rechazar lo que envíe el `POST` de la
  historia de alta.
- **La consulta no filtra por `activo`.** Devuelve todos los implementos y deja
  el filtro a quien consume, que es lo que necesita la pantalla de inventario
  para poder mostrar también los dados de baja.
- **El ordenamiento por nombre se hace en el handler**, no en base de datos, para
  no depender de la colación del servidor PostgreSQL: los nombres llevan tildes y
  el orden debe ser estable entre el entorno local y el desplegado.
- **La ruta lleva el prefijo `api/` en el atributo** (`[Route("api/v1/[controller]")]`),
  igual que el resto de los controllers. `app.UsePathBase("/api")` está comentado
  en `Program.cs`, así que el prefijo NO lo agrega el pipeline.
- **La tabla nace vacía y no se siembra.** Los implementos del mock del frontend
  (`loans-mock.api.ts`) son datos de relleno para maquetar, no inventario real, y
  cargarlos con `HasData` los dejaría en la base de producción. El alta se hace
  desde la aplicación con el `POST` de la historia correspondiente. Hasta
  entonces el endpoint responde `200` con lista vacía, comportamiento cubierto
  por la prueba `Listar_implementos_sin_registros_retorna_lista_vacia`.
- **Solo lectura.** La historia pide obtener los implementos; no se implementó
  `POST`, `PUT` ni `DELETE`.
- **No se tocó el frontend.** `app.config.ts` sigue con
  `{ provide: LoansApi, useClass: LoansMockApi }`.
