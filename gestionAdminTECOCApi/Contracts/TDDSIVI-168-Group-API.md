# TDDSIVI-168 — Contrato API de grupos

## Base

- Recurso: `/api/v1/Group`
- Formato: `application/json`
- Los identificadores usan UUID/GUID.
- `name` es obligatorio y admite hasta 100 caracteres.
- `code` es obligatorio y admite hasta 30 caracteres; la API lo recorta y normaliza a mayúsculas.

## Listar grupos

`GET /api/v1/Group`

Respuesta `200 OK`:

```json
{
  "groups": [
    {
      "id": "660938bc-d86a-43bd-8cf1-3f5faaf004c6",
      "name": "Grupo A",
      "code": "GRP-A",
      "enabled": true
    }
  ]
}
```

Cuando no existen grupos, `groups` es un arreglo vacío.

## Crear un grupo

`POST /api/v1/Group`

Solicitud:

```json
{
  "name": "Grupo A",
  "code": "GRP-A"
}
```

Respuesta `201 Created`:

```json
{
  "id": "660938bc-d86a-43bd-8cf1-3f5faaf004c6",
  "name": "Grupo A",
  "code": "GRP-A",
  "enabled": true
}
```

## Actualizar un grupo

`PUT /api/v1/Group/{groupId}`

Solicitud:

```json
{
  "groupId": "660938bc-d86a-43bd-8cf1-3f5faaf004c6",
  "name": "Grupo A actualizado",
  "code": "GRP-A"
}
```

Respuesta `200 OK`:

```json
{
  "id": "660938bc-d86a-43bd-8cf1-3f5faaf004c6",
  "name": "Grupo A actualizado",
  "code": "GRP-A",
  "enabled": true
}
```

El `groupId` de la ruta debe coincidir con el del cuerpo.

## Eliminar un grupo

`DELETE /api/v1/Group/{groupId}`

Respuesta `204 No Content`.

La eliminación es física. La relación está configurada con `ON DELETE SET NULL`, por lo que los usuarios asociados quedan con `groupId: null`.

## Errores

Formato común:

```json
{
  "statusCode": 409,
  "message": "Ya existe un grupo con ese código"
}
```

| Estado | Situación |
| --- | --- |
| `400 Bad Request` | Nombre o código vacío, longitud inválida o IDs diferentes entre ruta y cuerpo. |
| `404 Not Found` | No existe el grupo que se intenta actualizar o eliminar. |
| `409 Conflict` | Ya existe otro grupo con el mismo código normalizado. |

## Alcance excluido

- Asignar o retirar grupos de usuarios.
- Crear interfaces de frontend.
- Filtrar o paginar el listado.
