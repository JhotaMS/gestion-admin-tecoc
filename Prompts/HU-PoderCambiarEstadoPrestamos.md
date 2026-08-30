# Historia de Usuario: Poder cambiar el estado de los préstamos

|                 |                                                                                           |
| --------------- | ----------------------------------------------------------------------------------------- |
| **Historia**    | HU — Poder cambiar el estado de los préstamos                                             |
| **Componentes** | `gestionAdminTECOCApi` (Backend .NET 10) / `gestionAdminTECOCFrontend` (Frontend Angular) |
| **Módulo**      | `Loans / ImplementosPrestados`                                                            |

---

## 1. Descripción de la Historia

**Como** docente o encargado de inventario y laboratorio,  
**quiero** disponer de un módulo y endpoints para registrar y cambiar el estado de los préstamos de implementos tecnológicos,  
**para** controlar la entrega y devolución de los recursos, registrar su estado físico (Bueno, Regular, Malo), definir el rango de fechas de uso (Fecha Inicio y Fecha Fin) y registrar observaciones sobre la condición del equipo.

---

## 2. Criterios de Aceptación

1. **Selección de Usuario (`userId`)**:
   - El sistema debe permitir seleccionar el usuario solicitante a partir de los usuarios registrados en el sistema.
   - Debe existir un endpoint para consultar el listado de usuarios disponibles.
2. **Selección de Implemento (`implementoId`)**:
   - El sistema debe permitir seleccionar el implemento a prestar a partir del catálogo de implementos.
   - Debe existir un endpoint para consultar el listado de implementos.
3. **Selección de Tipo de Revisión (`tipoRevisionId`)**:
   - El sistema debe permitir clasificar la revisión según el momento del préstamo (_Inicio Préstamo_ o _Fin Préstamo_).
   - Debe existir un endpoint para listar los tipos de revisión disponibles.
4. **Estado Físico del Implemento (`estadoTipo`)**:
   - El estado físico debe ser obligatorio y corresponder a uno de los valores válidos: `Malo`, `Regular`, `Bueno`.
5. **Rango de Fechas del Préstamo (`fechaInicio` y `fechaFin`)**:
   - Campos presentados en modo solo lectura en el formulario lateral 'Estado de la implementación'.
   - La `fechaFin` debe ser igual o posterior a la `fechaInicio`.
6. **Observación (`observacion`)**:
   - Campo de texto para detallar notas sobre el estado o condiciones de la entrega.
7. **Registro y Persistencia**:
   - Al registrar exitosamente (`POST /api/v1/implementos-prestados`), los datos deben persistirse en la tabla `ImplementosPrestados` y retornar el identificador (`id`) y detalle del registro con código de estado HTTP `201 Created`.
   - Si faltan datos obligatorios o las reglas de validación no se cumplen, debe retornar código `400 Bad Request` indicando el error.

---

## 3. Modelo de Datos — Tabla `ImplementosPrestados`

Esquema: `gestionAdminTECOCApiMS`

| Columna          | Tipo de Dato               | Nulo | Descripción                                                |
| ---------------- | -------------------------- | ---- | ---------------------------------------------------------- |
| `id`             | `uuid`                     | NO   | Identificador único del registro (Primary Key)             |
| `userId`         | `uuid`                     | NO   | Clave foránea al usuario (`Users.Id`)                      |
| `implementoId`   | `uuid`                     | NO   | Identificador / Clave foránea del implemento               |
| `tipoRevisionId` | `integer`                  | NO   | Identificador del tipo de revisión (1: Inicio, 2: Entrega) |
| `estadoTipo`     | `varchar(20)`              | NO   | Estado del implemento (`Malo`, `Regular`, `Bueno`)         |
| `fechaInicio`    | `timestamp with time zone` | NO   | Fecha y hora de inicio del préstamo                        |
| `fechaFin`       | `timestamp with time zone` | NO   | Fecha y hora de fin del préstamo                           |
| `observacion`    | `varchar(500)`             | SÍ   | Observaciones o notas adicionales                          |
| `enabled`        | `boolean`                  | NO   | Estado de activación del registro (auditoría)              |

---

## 4. Endpoints de la API

| Método | Ruta                           | Descripción                                              |
| ------ | ------------------------------ | -------------------------------------------------------- |
| `GET`  | `/api/v1/User`                 | Lista los usuarios/docentes disponibles para el selector |
| `GET`  | `/api/v1/Implementos`          | Lista el catálogo de implementos disponibles             |
| `GET`  | `/api/v1/TiposRevision`        | Lista los tipos de revisión (_Inicio_, _Entrega_)        |
| `POST` | `/api/v1/ImplementosPrestados` | Registra el préstamo / cambio de estado del implemento   |
| `GET`  | `/api/v1/ImplementosPrestados` | Obtiene el historial de implementos prestados            |
