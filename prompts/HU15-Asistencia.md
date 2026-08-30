# HU15 — Programar las clases

| | |
|---|---|
| **Historia** | HU15 |
| **Componente** | `gestionAdminTECOCApi` (backend .NET 9) |
| **Rama** | `feature-HU15-Asistencia` |

## Historia de usuario

> **Como** docente,
> **quiero** poder programar las clases colocando la fecha, la hora, el tema y el
> nivel o unidad del curso,
> **para** tener organizadas las clases que voy a realizar durante el semestre.

## Campos requeridos (todos obligatorios)

| Campo | Contrato | Regla |
|---|---|---|
| Fecha | `scheduledDate` | Formato `yyyy-MM-dd` |
| Hora | `scheduledTime` | Formato `HH:mm` (24 horas); se acepta `HH:mm:ss` y se normaliza a `HH:mm` |
| Tema | `topic` | Texto libre, máximo 200 caracteres |
| Nivel o unidad del curso | `courseLevel` | Texto libre, máximo 100 caracteres |

## Criterios de aceptación

1. El endpoint debe recibir y validar los 4 campos obligatorios.
2. Si algún campo obligatorio falta, es nulo o está vacío, debe rechazar la
   solicitud e indicar cuál campo tiene el error.
3. Si la fecha o la hora no cumplen el formato configurado en el sistema, debe
   rechazar la solicitud e indicarlo.
4. Si ya existe una clase programada para la misma fecha y hora, NO debe crear un
   duplicado; debe responder indicando el cruce de horario (patrón
   `Result.Failure`, no una excepción).
5. Si todos los datos son válidos, debe crear y almacenar la clase programada.
6. Al crear exitosamente, debe devolver la información de la clase programada y
   su identificador.

## Contrato del endpoint

`POST /v1/ScheduledClass`

```json
{
  "scheduledDate": "2026-09-01",
  "scheduledTime": "14:30",
  "topic": "Ecuaciones diferenciales de primer orden",
  "courseLevel": "Unidad 3"
}
```

Respuesta `201 Created`:

```json
{
  "id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
  "scheduledDate": "2026-09-01",
  "scheduledTime": "14:30",
  "topic": "Ecuaciones diferenciales de primer orden",
  "courseLevel": "Unidad 3"
}
```

Respuestas de error:

| Código | Situación | `code` |
|---|---|---|
| `400` | Campo obligatorio faltante o vacío | `ScheduledClass.ValidationFailed` |
| `400` | Formato de fecha u hora inválido | `ScheduledClass.ValidationFailed` |
| `409` | Ya hay una clase en esa fecha y hora | `ScheduledClass.ScheduleAlreadyTaken` |

Los `400` acumulan en un solo mensaje todos los campos que fallaron, separados por
punto y espacio.

## Prompt inicial

```text
Contexto del proyecto:
Este es el backend de TECOC, en la carpeta gestionAdminTECOCApi/. Es una API
en .NET 9 con arquitectura por capas y patrón Result. Antes de escribir código,
lee estos archivos para entender las convenciones EXACTAS del proyecto:

1. gestionAdminTECOCApi.Domain/Abstractions/Result.cs y Error.cs
2. gestionAdminTECOCApi.Domain/Abstractions/Entity.cs
3. gestionAdminTECOCApi.Domain/Users/User.cs, UserService.cs, UserErrors.cs
   y DocumentTypeCodes.cs
4. gestionAdminTECOCApi.Application/Messaging/*.cs (ICommand, ICommandHandler, IDispatch)
5. gestionAdminTECOCApi.Application/Features/Users/CreateUser/*.cs
6. gestionAdminTECOCApi.Infrastructure.PostgreSql/Configurations/UserConfiguration.cs
7. gestionAdminTECOCApi.Api/Controllers/UserController.cs
8. gestionAdminTECOCApi.Api.Tests/Features/Users/*.cs

Reglas de arquitectura que debes seguir (son las que ya usa el proyecto):
- Los comandos implementan ICommand<TResponse> (no IRequest de MediatR directo).
- Los handlers implementan ICommandHandler<TCommand, TResponse> y devuelven
  Result<TResponse>, no lanzan excepciones para casos de negocio esperados.
- Las validaciones de formato/obligatoriedad van en un Validator de
  FluentValidation en la misma carpeta del comando.
- La entidad de dominio se crea con un método estático Create(...) y constructor
  privado, como en User.cs.
- La configuración EF Core va en Infrastructure.PostgreSql/Configurations/.
- El controller usa IDispatch.Send(request, cancellationToken).

Mi tarea (Jira HU15 - "Programar las clases"):

Como docente, quiero poder programar las clases colocando la fecha, la hora, el
tema y el nivel o unidad del curso, para tener organizadas las clases que voy a
realizar durante el semestre.

[Campos requeridos y criterios de aceptación — ver arriba]

Lo que necesito que hagas:
1. Crea la entidad de dominio con sus errores y su servicio de dominio.
2. Crea el Command + Handler en Application, con su Validator de FluentValidation.
3. Implementa la validación de cruce de horario usando el UnitOfWork existente.
4. Crea la configuración EF Core y genera la migración correspondiente.
5. Crea el endpoint en un controller nuevo.
6. Agrega pruebas unitarias del Validator y del Handler siguiendo el estilo de
   las pruebas de Users.

REGLAS ESTRICTAS sobre el manejo del repositorio y los commits:
- No hagas ningún commit ni push por tu cuenta.
- No agregues comentarios ni texto en el código que mencione que fue generado
  por IA — el código debe verse como código escrito por un desarrollador del equipo.
- No modifiques archivos de configuración del repo (.gitignore, README, CI/CD).
- No toques WeatherForecast ni Users; solo lo relacionado con esta historia.
```

## Decisiones de implementación

- **Fecha y hora se reciben como texto** (`"2026-09-01"`, `"14:30"`) y se parsean
  en `ClassScheduleFormats`, igual que `DocumentTypeCodes` hace con el tipo de
  documento. Así el formato inválido responde con un mensaje de negocio en
  español y no con un error de deserialización del binder de JSON.
- **Fecha y hora se persisten como `DateOnly` y `TimeOnly`** (`date` y
  `time without time zone` en PostgreSQL), no como `DateTime`, porque la historia
  pide una fecha y una hora de agenda, no un instante con zona horaria.
- **El cruce de horario se valida sobre (fecha, hora)** con un índice único en
  base de datos, bajo el supuesto de un único docente. Cuando el sistema modele
  docente y curso, la restricción debe moverse a (docente, fecha, hora).
- **Se permiten fechas pasadas.** Se evaluó rechazarlas, pero no está en la
  historia y bloquea registrar clases ya dictadas.
- **Las validaciones NO usan FluentValidation**, a diferencia del endpoint de
  usuarios. `ValidationBehavior` lanza `ValidationApplicationException` cuando un
  validador falla, y esa excepción se lanza *después* de que
  `UnitOfWorkBehevior` ya abrió un `TransactionScope`, así que cada petición
  inválida abría una transacción contra la base solo para abortarla. Las reglas
  viven en `ScheduledClassCommandRules` y el handler devuelve `Result.Failure`,
  que es lo que pide la propia guía del proyecto para casos de negocio
  esperados. El endpoint de usuarios se dejó como estaba.
