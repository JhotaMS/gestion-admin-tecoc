# TDDSIVI-17 — Crear endpoint registrar usuario

| | |
|---|---|
| **Historia** | TDDSIVI-17 |
| **Componente** | `gestionAdminTECOCApi` (backend .NET 9) |
| **Rama** | `feature/TDDSIVI-17-crear-endpoint-registrar-usuario` |

## Prompt inicial

```text
Contexto del proyecto:
Este es el backend de TECOC, en la carpeta gestionAdminTECOCApi/. Es una API
en .NET 9 (verifica TargetFramework en los .csproj, todos deben decir net9.0)
con arquitectura por capas y patrón Result. Antes de escribir código, lee
estos archivos para entender las convenciones EXACTAS del proyecto:

1. gestionAdminTECOCApi.Domain/Abstractions/Result.cs y Error.cs
2. gestionAdminTECOCApi.Domain/Abstractions/Entity.cs
3. gestionAdminTECOCApi.Domain/Users/User.cs y UserService.cs
4. gestionAdminTECOCApi.Application/Messaging/*.cs (ICommand, ICommandHandler, IDispatch)
5. gestionAdminTECOCApi.Application/Features/Users/CreateUser/*.cs (el ejemplo existente)
6. gestionAdminTECOCApi.Application/Abstractions/Behaviors/ValidationBehavior.cs
7. gestionAdminTECOCApi.Application/Exceptions/ValidationApplicationException.cs
8. gestionAdminTECOCApi.Infrastructure.PostgreSql/Configurations/UserConfiguration.cs
9. gestionAdminTECOCApi.Api/Controllers/UserController.cs

Reglas de arquitectura que debes seguir (son las que ya usa el proyecto, no las
cambies ni introduzcas MediatR "puro" ni excepciones de dominio nuevas):
- Usa sintaxis y features válidas para .NET 9 / C# 13, nada de paquetes o
  sintaxis que solo apliquen a versiones anteriores.
- Los comandos implementan ICommand<TResponse> (no IRequest de MediatR directo).
- Los handlers implementan ICommandHandler<TCommand, TResponse> y devuelven
  Result<TResponse>, no lanzan excepciones para casos de negocio esperados
  (ej. documento duplicado) — usan Result.Failure(new Error(...)).
- Las validaciones de formato/obligatoriedad van en un Validator de
  FluentValidation (AbstractValidator<TCommand>) en la misma carpeta del
  comando — el pipeline ya las recoge automáticamente vía ValidationBehavior.
- La entidad de dominio se crea con un método estático Create(...) como en User.cs,
  con constructor privado.
- La configuración EF Core va en Infrastructure.PostgreSql/Configurations/,
  siguiendo el estilo de UserConfiguration.cs.
- El controller usa IDispatch.Send(request, cancellationToken), como en
  UserController.cs, no llames a MediatR directamente.

Mi tarea (Jira TDDSIVI-17 - "Crear endpoint registrar usuario"):

Como sistema consumidor del servicio, quiero disponer de un endpoint que permita
crear un nuevo registro, para almacenar la información básica de identificación
y cargo de la persona.

Campos requeridos (todos obligatorios):
- Nombre completo
- Tipo de documento
- Número de documento
- Cargo

Criterios de aceptación:
1. El endpoint debe recibir y validar los 4 campos obligatorios.
2. Si algún campo obligatorio falta, es nulo o está vacío, debe rechazar la
   solicitud e indicar cuál campo tiene el error.
3. Si el tipo de documento enviado no es un valor válido configurado en el
   sistema, debe rechazar la solicitud e indicarlo.
4. El número de documento debe cumplir las reglas de validación definidas
   para el sistema (documento numérico, longitud razonable — usa tu criterio
   y explícamelo).
5. Si ya existe un registro con el mismo tipo y número de documento, NO debe
   crear un duplicado, debe responder indicando que el registro ya existe
   (usa el patrón Result.Failure, no una excepción).
6. Si todos los datos son válidos, debe crear y almacenar el registro.
7. Al crear exitosamente, debe devolver una respuesta con la información del
   registro creado y su identificador.

IMPORTANTE sobre el ejemplo existente de User:
El ejemplo actual (User.cs, UserCommand.cs, etc.) NO tiene los campos que pide
mi historia. Extiende la entidad User existente agregando los campos nuevos
(tipo de documento, número de documento, cargo) en vez de crear una entidad
paralela, para no duplicar configuración de EF ni el patrón ya armado.

Lo que necesito que hagas:
1. Ajusta la entidad de dominio con los campos requeridos.
2. Ajusta el Command + Handler en Application, con su Validator de
   FluentValidation (obligatoriedad, tipo de documento válido, formato de
   número de documento).
3. Implementa la validación de duplicado (mismo tipo + número de documento)
   usando el repositorio/UnitOfWork existente.
4. Ajusta la configuración EF Core (Configurations/) con los nuevos campos,
   y genera la migración correspondiente de EF Core.
5. Ajusta el endpoint en UserController.
6. Al final, dime en español y de forma simple qué archivos creaste o
   modificaste y por qué.

REGLAS ESTRICTAS sobre el manejo del repositorio y los commits:
- No hagas ningún commit ni push por tu cuenta. Yo reviso el código primero
  y hago los commits manualmente.
- No agregues comentarios, docstrings, mensajes ni ningún texto en el código
  que mencione que fue generado por IA, Claude, Copilot, ni nada similar.
  Ningún "Generated by AI", ningún emoji de robot, nada de eso — el código
  debe verse como código escrito normalmente por un desarrollador del equipo.
- No modifiques archivos de configuración del repo (.gitignore, README, CI/CD)
  a menos que te lo pida explícitamente.
- No toques WeatherForecast ni otras features que no tengan que ver con esta
  historia. Solo los archivos relacionados con Users/registro de usuario.
- Sé cuidadoso: no borres ni sobreescribas nada del ejemplo existente que no
  necesite cambiar. Modifica solo lo estrictamente necesario para cumplir
  los criterios de aceptación.
```

## Ajuste posterior

El prompt inicial pedía extender la entidad `User` existente agregando solo tres
campos (tipo de documento, número de documento y cargo), dando por cubierto el
"nombre completo" con los campos de nombre que ya tenía el ejemplo
(`FirstName`, `SecondName`, `SurName`, `SecondSurName`).

Al contrastar el resultado contra la historia se pidió alinear el contrato de
forma literal:

```text
hagalo como dice la historia de usuario tal cual
```

Con ese ajuste el endpoint recibe exactamente los cuatro campos de la historia
(`fullName`, `documentType`, `documentNumber`, `position`), y la entidad `User`
reemplaza los cuatro campos de nombre por un único `FullName`.
