## Endpoints Principales y Ejemplos de Uso

### 1. Crear Usuario

`POST /Users`

Crea un nuevo usuario.

**Request Body**
```json
{
  "username": "lazaro_dev"
}
```

**Response 201**
```json
{
  "id": "9c2b0dd6-1234-4d4e-b18b-000000000000",
  "username": "lazaro_dev"
}
```

---

### 2. Obtener Todos los Usuarios

`GET /Users`

Devuelve un listado de todos los usuarios.

**Response 200**
```json
[
  {
    "id": "9c2b0dd6-1234-4d4e-b18b-000000000000",
    "username": "lazaro_dev"
  }
]
```

---

### 3. Crear Tweet

`POST /Tweets`  
Requiere Header: `X-User-Id`

**Request Header**
```
X-User-Id: 9c2b0dd6-1234-4d4e-b18b-000000000000
```

**Request Body**
```json
{
  "content": "Mi primer tweet!"
}
```

**Response 201**
```json
{
  "id": 1,
  "content": "Mi primer tweet!",
  "createdAt": "2024-06-01T14:25:00Z",
  "authorId": "9c2b0dd6-1234-4d4e-b18b-000000000000",
  "authorUsername": "lazaro_dev"
}
```

---

### 4. Obtener Todos los Tweets

`GET /Tweets`

Devuelve todos los tweets creados.

**Response 200**
```json
[
  {
    "id": 1,
    "content": "Mi primer tweet!",
    "createdAt": "2024-06-01T14:25:00Z",
    "authorId": "9c2b0dd6-1234-4d4e-b18b-000000000000",
    "authorUsername": "lazaro_dev"
  }
]
```

---

### 5. Seguir a un Usuario

`POST /Users/{userId}/followers`  
Requiere Header: `X-User-Id`

**Request Header**
```
X-User-Id: ID_DEL_SEGUIDOR
```

**Request URL**
```
/Users/ID_DEL_USUARIO_A_SEGUIR/followers
```

**Response 201**
```json
{
  "follower": {
    "id": "ID_DEL_SEGUIDOR",
    "username": "lazaro_dev"
  },
  "followed": {
    "id": "ID_DEL_USUARIO_A_SEGUIR",
    "username": "juanito"
  },
  "followedAt": "2024-06-01T15:00:00Z"
}
```

---

### 6. Dejar de seguir a un Usuario

`DELETE /Users/{userId}/followers`  
Requiere Header: `X-User-Id`

**Response 204**
(No hay body)

---

### 7. Ver Timeline

`GET /Timeline?page=1&pageSize=20`  
Requiere Header: `X-User-Id`

**Response 200**
```json
{
  "page": 1,
  "pageSize": 20,
  "pages": 1,
  "totalItems": 2,
  "items": [
    {
      "id": 10,
      "content": "Hola mundo!",
      "createdAt": "2024-06-01T16:00:00Z",
      "authorId": "UUID_DE_AUTOR",
      "authorUsername": "user123"
    }
  ]
}
```

## Manejo de Errores

La API implementa un sistema robusto de manejo de errores usando un middleware centralizado. Cuando ocurre un error, se devuelve una respuesta en formato `application/problem+json` con la siguiente estructura:

```json
{
  "type": "ValidationError",
  "message": "User name is required",
  "referenceValue": null
}
```

- `type`: es una constante que representa el tipo de error.
- `message`: una descripción legible del error.
- `referenceValue`: opcional, usado para indicar parámetros inválidos o faltantes.


### Códigos de estado HTTP comunes

| Código | Tipo de error               | Descripción general                                                         |
|--------|-----------------------------|-----------------------------------------------------------------------------|
| 400    | BadRequest                  | Errores de dominio genéricos (datos mal formateados o inválidos).          |
| 401    | Unauthorized                | Falta el header `X-User-Id` o es inválido.                                 |
| 404    | NotFound                    | Entidad no encontrada, como usuario o tweet inexistente.                    |
| 409    | Conflict                    | Conflicto como seguir a un usuario ya seguido.                             |
| 422    | ValidationError             | Violaciones de reglas de validación o parámetros inválidos.                 |
| 500    | InternalServerError         | Error inesperado en el servidor.                                            |

## Lista de errores posibles y sus causas

Estos son los posibles errores que podés recibir desde la API. Cada error incluye el `type`, el `message` y su correspondiente código HTTP:

- `UsernameIsRequired`: User name is required (422)
- `UsernameMustBeBetween4And15CharsLength`: User name must be between 4 and 15 chars length (422)
- `CannotFollowYourself`: Cannot follow yourself (400)
- `TweetContentIsRequired`: Tweet content is required. (422)
- `TweetContentCannotBeMoreThan280Chars`: Tweet content cannot be more than 280 chars (422)
- `UserNotFound`: User not found (404)
- `FollowerUserNotFound`: Follower user not found (404)
- `UserToFollowNotFound`: User to follow not found (404)
- `AlreadyFollowingUser`: Already following uaser (409)
- `FollowRelationshipNotFound`: Follow relationship not found (404)
- `UserToFollowIsRequired`: User to follow is required (422)
- `TweetAuthorIsRequired`: Tweet autho is required (422)
- `PageCannotBeZeroOrLess`: Invalid 'Page' parameter with message: 'Cannot be zero or less'. (422)
- `PageCannotBeNullWhenIsPaginatedIsTrue`: Invalid 'Page' parameter with message: 'Cannot be null'. (422)
- `PageSizeCannotBeNullWhenIsPaginatedIsTrue`: Invalid 'PageSize' parameter with message: 'Cannot be null'. (422)
- `PageSizeCannotBeZeroOrLess`: Invalid 'PageSize' parameter with message: 'Cannot be zero or less'. (422)
- `UsernameAlreadyExists`: User name already exists (409)
- `MissingUserIdHeader`: Missing X-User-Id header (401)
- `InvalidUserIdHeader`: Invalid X-User-Id header (401)
- `InternalError`: Server internal error (500)
