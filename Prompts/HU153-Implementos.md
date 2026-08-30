# HU153 — Obtener implementos disponibles

| | |
|---|---|
| **Historia** | HU153 |
| **Componente** | `gestionAdminTECOCApi` (backend .NET 10) |
| **Rama** | `feature-HU153-Implementos` (desde `develop`) |

## Historia de usuario

> **Como** usuario del sistema,
> **quiero** obtener el listado de implementos que se encuentran disponibles,
> **para** conocer los implementos que puedo solicitar en préstamo.

## Regla de negocio

Un implemento está disponible cuando tiene **una o más unidades disponibles**
(`CantidadDisponible > 0`) **y se encuentra activo** (`Activo = true`). Las dos
condiciones se exigen a la vez.

## Criterios de aceptación

| | | Estado |
|---|---|---|
| **CA01** | El sistema consulta los registros de la entidad `Implemento`. | Cubierto |
| **CA02** | Retorna únicamente los implementos con `CantidadDisponible > 0` y `Activo = true`. | Cubierto |
| **CA03** | Retorna `Id`, `Nombre`, `Codigo`, `Descripcion`, `CantidadTotal`, `CantidadDisponible` y `Estado`. | Cubierto |
| **CA04** | Los implementos con `CantidadDisponible = 0` no se incluyen. | Cubierto |
| **CA05** | Los implementos con `Activo = false` no se retornan aunque tengan unidades. | Cubierto |
| **CA06** | Sin resultados, retorna listado vacío e informa que no hay implementos disponibles. | Cubierto |

## Contrato del endpoint

`GET /api/implementos/disponibles`

Respuesta `200 OK` con resultados:

```json
{
  "implementos": [
    {
      "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
      "nombre": "Multímetro digital",
      "codigo": "MT-014",
      "descripcion": "Multímetro digital de gama media",
      "cantidadTotal": 8,
      "cantidadDisponible": 3,
      "estado": "Bueno"
    }
  ],
  "mensaje": null
}
```

Respuesta `200 OK` sin resultados (CA06):

```json
{
  "implementos": [],
  "mensaje": "No hay implementos disponibles"
}
```

## Estructura de la entidad Implemento

| Campo | Descripción | Regla |
|---|---|---|
| `Id` | Identificador del implemento | `Guid`, generado por el sistema |
| `Nombre` | Nombre del implemento | Obligatorio, máximo 100 caracteres |
| `Codigo` | Código del implemento | Obligatorio, único, máximo 20 caracteres |
| `Descripcion` | Descripción del implemento | Obligatorio, máximo 250 caracteres |
| `CantidadTotal` | Cantidad total registrada | Entero |
| `CantidadDisponible` | Cantidad disponible para préstamo | Entero |
| `Estado` | Estado actual del implemento | Obligatorio, texto libre, máximo 50 caracteres |
| `Activo` | Indica si el implemento está activo | Booleano |

## Archivos

| Capa | Archivo |
|---|---|
| Domain | `gestionAdminTECOCApi.Domain/Implementos/Implemento.cs` |
| Application | `gestionAdminTECOCApi.Application/Features/Implementos/GetImplementosDisponibles/GetImplementosDisponiblesQuery.cs` |
| Infrastructure | `gestionAdminTECOCApi.Infrastructure.PostgreSql/Configurations/ImplementoConfiguration.cs` |
| Infrastructure | `Migrations/20260830154520_V1-0-5-Hu153Implementos.cs` (+ `.Designer.cs`) |
| Api | `gestionAdminTECOCApi.Api/Controllers/ImplementosController.cs` |
| Tests | `gestionAdminTECOCApi.Api.Tests/Integration/ImplementosDisponiblesTests.cs` |

## Decisiones de implementación

- **La entidad se llama `Implemento`, en español.** La entidad `Prestamo` que
  entró en `develop` con el PR #25 declara `public Guid ImplementoId`, así que el
  modelo del equipo ya espera ese nombre. Namespace `Domain.Implementos`, tabla
  `Implementos`.
- **La ruta es la de la historia**, `api/implementos/disponibles`. Se aparta de la
  convención `api/v1/[controller]` que usan los demás controllers, pero el
  criterio de aceptación fija esa URL y manda la historia.
- **`Activo` se apoya en el `Enabled` que hereda `Entity<Guid>`.** La clase base ya
  persiste una columna booleana; agregar además una columna `Activo` dejaría dos
  banderas con el mismo significado en la misma tabla. El filtro del CA02 y CA05
  se aplica sobre `Enabled`. El constructor privado recibe el parámetro como
  `enabled` porque EF Core enlaza los parámetros del constructor por nombre de
  propiedad, y con `activo` falla al construir el modelo.
- **`Activo` no se retorna** (CA03 no lo lista). Como el endpoint solo devuelve
  implementos activos, el campo sería siempre `true` y no aporta información.
- **El filtro se aplica en base de datos**, no en memoria, para no traer todo el
  inventario y descartarlo después.
- **`estado` es texto libre**, igual que `EstadoTipo` en `Prestamo`. No se
  restringe a una lista de valores para no rechazar lo que envíe el `POST` de la
  historia de alta.
- **El ordenamiento por nombre se hace en el handler**, no en base de datos, para
  no depender de la colación del servidor PostgreSQL: los nombres llevan tildes y
  el orden debe ser estable entre el entorno local y el desplegado.
- **La tabla nace vacía y no se siembra.** Los implementos del mock del frontend
  (`loans-mock.api.ts`) son datos de relleno para maquetar, no inventario real, y
  cargarlos con `HasData` los dejaría en la base de producción. El alta se hace
  desde la aplicación con el `POST` de la historia correspondiente.
- **Solo lectura.** La historia pide obtener los implementos disponibles; no se
  implementó `POST`, `PUT` ni `DELETE`.
- **No se tocó el frontend.** `app.config.ts` sigue con
  `{ provide: LoansApi, useClass: LoansMockApi }`.
