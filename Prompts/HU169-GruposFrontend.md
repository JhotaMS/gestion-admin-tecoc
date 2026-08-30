# HU169 — Frontend módulo CRUD de grupos

| | |
|---|---|
| **Historia** | HU169 |
| **Componente** | `gestionAdminTECOCFrontend` (Angular 22) |
| **Rama** | `feature-HU169-GruposFrontend` (desde `develop`) |
| **Depende de** | HU168 / TDDSIVI-168, backend de grupos (Néstor Guarín) |

## Historia de usuario

> **Como** administrador del portal,
> **quiero** consultar, crear, editar y eliminar los grupos desde una pantalla del
> sistema,
> **para** mantener al día el catálogo de grupos con el que se clasifica a los
> usuarios, sin depender de que alguien ejecute peticiones a la API a mano.

## Criterios de aceptación

1. La pantalla lista los grupos existentes mostrando nombre, código y estado.
2. Cuando no hay grupos registrados, se muestra un mensaje que lo indica en lugar
   de una tabla vacía.
3. Se puede crear un grupo indicando nombre y código; al guardarlo aparece en el
   listado sin recargar la página.
4. Se puede editar el nombre y el código de un grupo existente; el cambio se
   refleja en el listado.
5. Se puede eliminar un grupo, con una confirmación previa que advierte que los
   usuarios de ese grupo quedarán sin grupo asignado.
6. El nombre es obligatorio y admite hasta 100 caracteres; el código es
   obligatorio y admite hasta 30. La validación ocurre antes de llamar a la API.
7. Los errores que devuelve la API se muestran al usuario con su mensaje, en
   particular el código duplicado (`409`) y el grupo inexistente (`404`).

## API que consume

Definida en el contrato de TDDSIVI-168, implementada por Néstor Guarín.

| Acción | Petición | Respuesta |
|---|---|---|
| Listar | `GET /api/v1/Group` | `200` `{ "groups": [ { id, name, code, enabled } ] }` |
| Crear | `POST /api/v1/Group` | `201` `{ id, name, code, enabled }` |
| Actualizar | `PUT /api/v1/Group/{groupId}` | `200` `{ id, name, code, enabled }` |
| Eliminar | `DELETE /api/v1/Group/{groupId}` | `204` |

Errores: `{ "statusCode": 409, "message": "Ya existe un grupo con ese código" }`.

## Archivos

| Rol | Archivo |
|---|---|
| Modelos | `src/app/groups/groups.models.ts` |
| Contrato | `src/app/groups/groups-api.ts` |
| Implementación HTTP | `src/app/core/groups/groups-http.api.ts` |
| Componente | `src/app/groups/groups.component.ts` + `.html` |
| Proveedor | `src/app/app.config.ts` |
| Ruta | `src/app/app.routes.ts` (`/grupos`) |
| Menú | `src/app/layout/sidebar/sidebar-menu.ts` |

## Decisiones de implementación

- **Se sigue el molde de usuarios**: una clase abstracta `GroupsApi` en la carpeta
  de la funcionalidad y una implementación `GroupsHttpApi` en `core/`, registradas
  en `app.config.ts`. Es el mismo patrón de `UsersApi` / `UsersHttpApi`.
- **No se creó un mock.** El backend de grupos ya existe, así que el proveedor
  apunta directo a la implementación HTTP, igual que `UserRegistrationApi`.
- **La URL se arma como `${environment.apiBaseUrl}/api/v1/Group`**, siguiendo a
  `auth-http.api.ts`, que es el que hoy funciona contra el backend desplegado.
  `users-http.api.ts` y `user-registration-http.api.ts` omiten el tramo `api/v1`;
  eso parece un error de esos archivos y no se tocó por ser de otra historia.
- **El listado no tiene búsqueda ni paginación**, porque el contrato las declara
  fuera de alcance. Agregar un filtro en cliente serían pocas líneas si el equipo
  lo pide.
- **La validación de longitud se hace en el cliente y también la valida el
  backend.** Los `maxlength` de los campos evitan que el usuario escriba de más,
  y el mensaje de error se muestra al intentar guardar.
- **La normalización del código a mayúsculas la hace la API**, no el formulario.
  El campo muestra una nota indicándolo para que el usuario no se sorprenda al ver
  el valor guardado.
- **El borrado pide confirmación** y advierte el efecto colateral: la relación con
  usuarios está configurada con `ON DELETE SET NULL`, así que los usuarios del
  grupo quedan sin grupo, no se eliminan.

## Fuera de alcance

- Asignar o retirar grupos de usuarios.
- Filtrar o paginar el listado.
- Cambiar el estado `enabled` de un grupo: la API no expone esa operación.
