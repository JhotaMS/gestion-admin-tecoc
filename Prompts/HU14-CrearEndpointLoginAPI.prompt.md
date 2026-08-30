# Prompt — HU14 Crear endpoint login API

> **Jira:** `TDDSIVI-14` — `En curso` → `En revisión` al terminar
> **Módulo:** `gestionAdminTECOCApi` Hexagonal `Domain/Users` + `Application/Features/Auth/Login` + `Infrastructure/Services` + `Api/Controllers/AuthController`
> **Stack:** `net9.0` + `Argon2 2.0.0` + `System.IdentityModel.Tokens.Jwt 7.1.2` + `StackExchange.Redis 2.8.16` (YARP)

## Rol
Actúa como Tech Lead implementando HU14 en `feature/HU14-CrearEndpointLoginAPI` sobre `develop`. No inventes fuera de `Documents` y `02-appsettings.prod.json`.

## Fuentes de verdad (leer antes de codificar)
- `Documents/05-documentacion-final/01-Arquitectura-Sistema-TECOC.md` §2 IAM
- `Documents/03-diseño-bajo-nivel-lld/01-Patrones-GoF.md` (Core sin EF)
- `Documents/04-infraestructura-despliegue/02-appsettings.prod.json:16-55` (Jwt 15m/7d, RateLimit login5per15min)
- `Documents/08-backlog-normalizado/nestor_asignadas_detailed.json` HU14

## Alcance HU14
Como **docente o administrador** requiero ingresar al portal mediante inicio de sesión para interactuar con la aplicación.
**Criterio:** usuario y contraseña deben ser válidas → `200 {accessToken, refreshToken, expiresAt}` / `401` inválidas / `423` bloqueada (`LockedUntil`).

## Reglas inmutables (rompen build)
1. `Domain` sin `EF` — `User : Entity<Guid>` puro + `IPasswordHasher`, `IJwtService` en `Domain/Ports`
2. `JWT` HS256 doble clave `JWT_SIGNING_KEY` / `JWT_SIGNING_KEY_NEW` ventana 15min, `AccessTokenMinutes 15`, `RefreshTokenDays 7`
3. `User` con `Email unique 320`, `PasswordHash`, `FailedLoginAttempts`, `LockedUntil`, `IsLocked`, `RecordFailedLogin()` → 5/15min
4. `UserConfiguration` Fluent API `HasIndex Email unique`
5. `Api/Controllers/AuthController POST v1/auth/login` via `IDispatch` + `LoginCommand`

## Tareas
- Extender `Domain/Users/User.cs` con `Email, PasswordHash, LockedUntil`
- `Domain/Ports/IPasswordHasher`, `IJwtService`
- `Application/Features/Auth/Login/{LoginCommand,LoginResponse,LoginCommandHandler}` (verifica `IsLocked`, `Argon2.Verify`, `RecordFailedLogin/Reset`, `GenerateAccessToken/RefreshToken`)
- `Infrastructure/Services/Argon2PasswordHasher`, `JwtService` (HS256 15m) + `Extensions/DependencyInjection.AddInfrastructure()`
- `Infrastructure.PostgreSql/Configurations/UserConfiguration` Email unique
- `Api/Program.cs` `AddInfrastructure()` + `Api.csproj` ref `Infrastructure`

## Validación
`dotnet build` 0 errores + `dotnet test` 0 fallos + `TargetFramework net9.0` + `Core 0 EF` (`dotnet list`).
No push sin preguntar. Branch `feature/HU14-CrearEndpointLoginAPI`.

## Entregable
Commit `feat(iam): HU14 login API` con 14 archivos (ver `git diff --cached --stat`).
