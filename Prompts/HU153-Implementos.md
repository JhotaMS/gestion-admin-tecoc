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
| `Nombre` | Nombre del implemento | Obligatorio, máximo 150 caracteres |
| `Codigo` | Código del implemento | Obligatorio, único, máximo 30 caracteres |
| `Descripcion` | Descripción del implemento | Máximo 500 caracteres |
| `CantidadTotal` | Cantidad total registrada | Entero |
| `CantidadDisponible` | Cantidad disponible para préstamo | Entero |
| `Estado` | Estado actual del implemento | Texto libre, máximo 50 caracteres |
| `Activo` | Indica si el implemento está activo | Booleano |

Las longitudes de `Nombre`, `Codigo` y `Descripcion` las fija la configuración que
ya existía en `develop`; no se modificaron.

## Archivos

| Capa | Archivo | |
|---|---|---|
| Domain | `gestionAdminTECOCApi.Domain/Loans/Implemento.cs` | ampliado |
| Application | `Features/Implementos/GetImplementosDisponibles/GetImplementosDisponiblesQuery.cs` | nuevo |
| Infrastructure | `Configurations/ImplementoDisponibilidadConfiguration.cs` | nuevo |
| Infrastructure | `Migrations/20260830201049_V1-0-6-Hu153ImplementosDisponibilidad.cs` | nuevo |
| Api | `Controllers/ImplementosDisponiblesController.cs` | nuevo |
| Tests | `Api.Tests/Integration/ImplementosDisponiblesTests.cs` | nuevo |

## Decisiones de implementación

- **Se reutiliza la entidad `Implemento` que ya existe en `Domain/Loans`.** Entró a
  `develop` con el PR #29, creada para el módulo de préstamos. La historia necesita
  tres campos que esa entidad no tenía, así que se le agregaron `CantidadTotal`,
  `CantidadDisponible` y `Estado`. La alternativa —una segunda entidad `Implemento`
  propia— era inviable: dos tipos no pueden mapearse a la misma tabla `Implementos`,
  y duplicar la tabla habría partido el inventario en dos.
- **La ampliación de la entidad es aditiva y no rompe nada.** Los parámetros nuevos
  de `Create` son opcionales y con valor por defecto, así que las llamadas que ya
  existían siguen compilando y comportándose igual.
- **Las columnas nuevas se configuran en un archivo aparte**,
  `ImplementoDisponibilidadConfiguration`, en lugar de modificar el
  `ImplementoConfiguration` existente. Entity Framework admite varias
  `IEntityTypeConfiguration<T>` para la misma entidad y aplica todas.
- **El endpoint vive en su propio controller**, `ImplementosDisponiblesController`,
  y no dentro del `ImplementosController` que ya existía. Las rutas no chocan:
  `api/implementos/disponibles` frente a `api/v1/Implementos`.
- **La ruta es la de la historia**, `api/implementos/disponibles`. Se aparta de la
  convención `api/v1/[controller]` que usan los demás controllers, pero el criterio
  de aceptación fija esa URL.
- **`Activo` se apoya en el `Enabled` que hereda `Entity<Guid>`.** La clase base ya
  persiste una columna booleana; agregar además una columna `Activo` dejaría dos
  banderas con el mismo significado. El filtro del CA02 y CA05 opera sobre ella.
- **`Activo` no se retorna** (CA03 no lo lista). Como el endpoint solo devuelve
  implementos activos, el campo sería siempre `true` y no aporta información.
- **El filtro se aplica en base de datos**, no en memoria, para no traer todo el
  inventario y descartarlo después.
- **El ordenamiento por nombre se hace en el handler**, no en base de datos, para no
  depender de la colación del servidor PostgreSQL: los nombres llevan tildes y el
  orden debe ser estable entre el entorno local y el desplegado.
- **La migración solo agrega columnas.** No recrea la tabla `Implementos` ni toca
  los datos existentes; las tres columnas entran con valor por defecto.
- **La tabla no se siembra.** Los implementos del mock del frontend son datos de
  relleno para maquetar, no inventario real. El alta se hace desde la aplicación.
- **Solo lectura.** La historia pide obtener los implementos disponibles; no se
  implementó `POST`, `PUT` ni `DELETE`.
