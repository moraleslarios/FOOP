# MoralesLarios.OOFP.WebControllers

Controlador genérico de ASP.NET Core que expone un **CRUD HTTP estándar** sobre cualquier entidad/DTO, conectando `IGenServiceFp<TEntity, TDto>` con `IActionResult` mediante `MlResult<T>` y `ProblemDetails`.

En la práctica:

```csharp
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

…y ya tienes el CRUD completo (`GET`, `POST`, `PUT`, `DELETE`, búsqueda por ID) listo, con tipado de PK y manejo de errores funcional.

---

## Tabla de contenido

- [Dependencias](#dependencias)
- [Registro](#registro)
- [Estructura del proyecto](#estructura-del-proyecto)
- [`SimpleMlControllerBase<TEntity, TDto, TPk>`](#simplemlcontrollerbasetentity-tdto-tpk)
- [`SimpleMlComplexPkControllerBase<TEntity, TDto>` (PK compuesta)](#simplemlcomplexpkcontrollerbasetentity-tdto-pk-compuesta)
- [`Attributes.PkParameterAttribute`](#attributespkparameterattribute)
- [`Helpers.Extensions.ConverterTo`](#helpersextensionsconverterto)
- [Receta completa](#receta-completa)
- [Combinarlo con caché](#combinarlo-con-caché)

---

## Dependencias

| Dependencia                              | Tipo     |
|------------------------------------------|----------|
| `Microsoft.AspNetCore.Mvc.Core` 2.x      | NuGet    |
| `MoralesLarios.OOFP.WebApi`              | Proyecto |
| `MoralesLarios.OOFP.WebServices`         | Proyecto |

Target framework: **`net8.0`**.

> El proyecto trae transitivamente todo lo necesario:
> `MlActionResults`, `MlResultWebExtensionsPlus`, `MlErrorsDetailsExtensions`,
> `MlProblemsDetails`, `IGenServiceFp<,>`, etc.

---

## Registro

No expone `AddXxx`. Para usarlo solo necesitas:

1. Registrar los servicios genéricos (`MoralesLarios.OOFP.WebServices`):

```csharp
using MoralesLarios.OOFP.WebServices;

services.AddTransientGenServicesFpWithoutReposGeneral();
// o AddScopedtGenServicesFpWithoutReposGeneral / AddSingletonGenServicesFpWithoutReposGeneral
```

2. Registrar tu repositorio EF (proyecto `MoralesLarios.OOFP.EFCore`).

3. Heredar tus controladores de `SimpleMlControllerBase<,,>`.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.WebControllers/
??? Attributes/
?   ??? PkParameterAttribute.cs    // documentación del parámetro PK en Swagger
??? Controllers/
?   ??? SimpleMlControllerBase.cs            // controlador CRUD con PK simple (TPk genérico)
?   ??? SimpleMlComplexPkControllerBase.cs   // controlador CRUD con PK compuesta
??? Helpers/
?   ??? Extensions.cs               // ConverterTo (string -> tipo PK)
??? GlobalUsings.cs
??? RegisterServices.cs             // AddWebControllers (placeholder, reservado)
```

---

## `SimpleMlControllerBase<TEntity, TDto, TPk>`

Controlador `[ApiController]` que delega cada acción HTTP en `IGenServiceFp<TEntity, TDto>` y convierte el `MlResult<>` en `IActionResult` mediante `MlResultWebExtensionsPlus`.

### Firma

```csharp
[ApiController]
public class SimpleMlControllerBase<TEntity, TDto, TPk>(IGenServiceFp<TEntity, TDto> _genServiceFp)
    : ControllerBase
    where TEntity : class
    where TDto    : class
{
    [HttpGet]                         public virtual Task<IActionResult> GetAllAsync(CancellationToken ct = default!);
    [HttpGet("id-str/{id}", Name = "[controller]_[action]")]
                                      public virtual Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!);
    [HttpPost]                        public virtual Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!);
    [HttpPut("{id}")]                 public virtual Task<IActionResult> PutAsync(string id, [FromBody] TDto dto, CancellationToken ct = default!);
    [HttpPut]                         public virtual Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!);
    [HttpDelete("{id}")]              public virtual Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default!);
    [HttpDelete]                      public virtual Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!);
}
```

### Endpoints expuestos

| Verbo  | Ruta                       | Acción                                                 | Respuesta éxito | Respuesta error                |
|--------|----------------------------|--------------------------------------------------------|-----------------|--------------------------------|
| GET    | `/`                        | Lista todas las entidades                              | `200 OK`        | `ProblemDetails`              |
| GET    | `/id-str/{id}`             | Recupera por PK (convierte `id` a `TPk`)               | `200 OK`        | `404` o `ProblemDetails`      |
| POST   | `/`                        | Crea una entidad                                       | `201 Created`   | `ProblemDetails`              |
| PUT    | `/{id}`                    | Actualiza por PK                                       | `204 NoContent` | `404` o `ProblemDetails`      |
| PUT    | `/`                        | Actualiza usando la PK contenida en el DTO             | `204 NoContent` | `ProblemDetails`              |
| DELETE | `/{id}`                    | Elimina por PK                                         | `204 NoContent` | `404` o `ProblemDetails`      |
| DELETE | `/`                        | Elimina usando la PK del DTO                           | `204 NoContent` | `ProblemDetails`              |

### Conversión automática del `id` a `TPk`

`GetByIdAsync`, `PutAsync(id, ...)` y `DeleteAsync(id, ...)` convierten el `string` recibido a `TPk` con `ConverterTo`. Si la conversión falla se devuelve un `404 NotFound` con el mensaje *"Path '{id}' not exists or is diferent type to PK of '{TPk}' was not found."*

### Sobreescritura selectiva

Todos los métodos son `virtual`. Puedes sobreescribir solo lo que necesites:

```csharp
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc)
{
    [HttpGet("active")]
    public Task<IActionResult> GetActiveAsync(CancellationToken ct = default)
        => _genServiceFp.AllAsync(ct: ct)
                        .MapAsync(users => users.Where(u => u.IsActive))
                        .ToGetPdActionResultAsync();
}
```

> ?? Si sobreescribes un método y devuelves directamente lo del `base.*`, **no** lo envuelvas en `Ok(...)`: `base` ya devuelve un `IActionResult`. Hacer `Ok(result)` lo serializaría como objeto y deformaría el JSON.

---

## `Helpers.Extensions.ConverterTo`

Utilidad para convertir `string` a un tipo primitivo o nullable usando el `Type` destino.

```csharp
public static object ConverterTo(this string value, Type property);
```

Soporta:

- Tipos primitivos: `int`, `long`, `short`, `byte`, `sbyte`, `uint`, `ulong`, `ushort`, `float`, `double`, `decimal`, `char`, `bool`, `DateTime`, `string`.
- Sus equivalentes `Nullable<T>`.

Si la cadena no se puede parsear, lanza la excepción del `Parse` correspondiente. Si el tipo no está soportado, lanza `FormatException`.

`SimpleMlControllerBase` lo invoca dentro de un `TryMapAsync` para convertir la excepción en un `MlResult.Fail` que se traduce a `404 NotFound`.

---

## `SimpleMlComplexPkControllerBase<TEntity, TDto>` (PK compuesta)

Variante de `SimpleMlControllerBase<,,>` pensada para entidades con **clave primaria compuesta** (más de un campo, o tipos no triviales como `DateTime`/`DateOnly`/`TimeOnly`). En lugar de un parámetro de tipo `TPk` recibe en el constructor primario un `Func<TEntity, object[]> _pkFields` que indica cómo extraer los valores de PK de una entidad.

### Firma

```csharp
[ApiController]
public class SimpleMlComplexPkControllerBase<TEntity, TDto>(
        IGenServiceFp<TEntity, TDto> _genServiceFp,
        Func<TEntity, object[]>      _pkFields)
    : ControllerBase
    where TEntity : class
    where TDto    : class
{
    [HttpGet]                             public virtual Task<IActionResult> GetAllAsync(CancellationToken ct = default!);
    [HttpGet("id-str/{ids}", Name = "[controller]_[action]")]
                                          public virtual Task<IActionResult> GetByIdAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);
    [HttpPost]                            public virtual Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!);
    [HttpPut("{ids}")]                    public virtual Task<IActionResult> PutAsync([FromRoute][PkParameter] string ids, [FromBody] TDto dto, CancellationToken ct = default!);
    [HttpPut]                             public virtual Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!);
    [HttpDelete("{ids}")]                 public virtual Task<IActionResult> DeleteAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);
    [HttpDelete]                          public virtual Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!);

    protected object[] GetPkValues(string[] values, Func<TEntity, object[]> pkFields);
}
```

### Formato del parámetro `ids`

El parámetro `ids` que viaja por la ruta es un **string con todos los valores de la PK separados por comas**, en el orden definido por `_pkFields`. El controlador los convierte automáticamente a los tipos correctos usando una instancia *sample* de `TEntity` (`Activator.CreateInstance<TEntity>()`) para inferir los `Type` esperados.

Formatos soportados de elementos:

| Tipo .NET     | Formato esperado           | Ejemplo                       |
|---------------|----------------------------|-------------------------------|
| `int`/`long`/numerics | Texto numérico       | `1`, `42`, `100`              |
| `string`      | Texto literal              | `Prueba`                      |
| `DateTime`    | ISO 8601 con ms            | `2026-05-16T07:34:29.239`     |
| `DateOnly`    | `yyyy-MM-dd`               | `2026-05-16`                  |
| `TimeOnly`    | `HH:mm:ss.fff`             | `07:34:29.239`                |
| `Guid`        | Texto canónico             | `00000000-0000-...`           |

Ejemplo de URL: `GET /api/PruebaComplex/id-str/Prueba,Lugar,100,2020-01-01T00:00:00.000`

### Ejemplo de uso

```csharp
[Route("api/[controller]")]
public class PruebaComplexController(IGenServiceFp<PruebaComplex, PruebaComplexDto> svc)
    : SimpleMlComplexPkControllerBase<PruebaComplex, PruebaComplexDto>(
          svc,
          e => new object[] { e.Nombre, e.Lugar, e.Precio, e.Fecha }) { }
```

### Endpoints expuestos

| Verbo  | Ruta                       | Acción                                            | Respuesta éxito  |
|--------|----------------------------|---------------------------------------------------|------------------|
| GET    | `/`                        | Lista todas las entidades                          | `200 OK`        |
| GET    | `/id-str/{ids}`            | Recupera por PK compuesta (ids separados por `,`) | `200 OK` / `404`|
| POST   | `/`                        | Crea una entidad                                  | `201 Created`   |
| PUT    | `/{ids}`                   | Actualiza por PK compuesta                        | `204 NoContent` |
| PUT    | `/`                        | Actualiza usando la PK del DTO                    | `204 NoContent` |
| DELETE | `/{ids}`                   | Elimina por PK compuesta                          | `204 NoContent` |
| DELETE | `/`                        | Elimina usando la PK del DTO                      | `204 NoContent` |

> ? **Requisito**: `TEntity` debe tener un constructor sin parámetros para que `Activator.CreateInstance<TEntity>()` pueda crear el *sample*. Si la PK contiene tipos `string` u otros reference types nullables, el sample expondrá `null` y `GetPkValues` los tratará como `string`. Para escenarios más exigentes, sobreescribe `GetPkValues` en el controlador hijo.

---

## `Attributes.PkParameterAttribute`

Atributo para documentar parámetros de tipo PK compuesta en Swagger/OpenAPI. Lo aplica internamente `SimpleMlComplexPkControllerBase` a sus parámetros `ids`, pero también está disponible para que lo uses en tus propios endpoints.

### Firma

```csharp
[AttributeUsage(AttributeTargets.Parameter)]
public class PkParameterAttribute : Attribute
{
    public string Description { get; set; }

    public PkParameterAttribute(string description = null!);  // descripción por defecto si null
}
```

La descripción por defecto es:

> *"Valores de la clave primaria separados por comas. Para DateTime usa formato ISO 8601: yyyy-MM-ddTHH:mm:ss.fff (Ejemplo: '1,2' para PKs compuestas o '2026-05-16T07:34:29.239' para DateTime)"*

### Ejemplo

```csharp
[HttpGet("by-pk/{ids}")]
public Task<IActionResult> GetByPk(
    [FromRoute]
    [PkParameter("PK compuesta: 'A,B,C' — separados por comas en orden A>B>C")]
    string ids)
{
    // ...
}
```

> Para que Swagger consuma la descripción del atributo necesitas que tu generador OpenAPI (`Swashbuckle`, `NSwag`, o el `Microsoft.AspNetCore.OpenApi` integrado) inspeccione atributos personalizados. Con `Swashbuckle.AspNetCore` basta con habilitar XML doc o un `IOperationFilter` que lea `PkParameterAttribute.Description` desde los `ParameterDescriptor`.

---

## Receta completa

```csharp
// 1. Servicios y repos
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
services.AddTransient(typeof(IEFRepoFp<>), typeof(EFRepoFp<>));
services.AddTransientGenServicesFpWithoutReposGeneral();

// 2. Mvc
services.AddControllers();

// 3. Controlador concreto
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

Quedan disponibles automáticamente:

```
GET    /api/users
GET    /api/users/id-str/{id}
POST   /api/users
PUT    /api/users/{id}
PUT    /api/users
DELETE /api/users/{id}
DELETE /api/users
```

---

## Combinarlo con caché

Para CRUD con caché HTTP automática (incluida invalidación al escribir, *tag* por controlador, bypass por header, etc.) hereda de `SimpleMlCacheControllerBase<,,>` del proyecto **`MoralesLarios.OOFP.WebControllers.Cache`**.

```csharp
public class UsersController(IGenServiceFp<User, UserDto> svc, IOutputCacheStore store)
    : SimpleMlCacheControllerBase<User, UserDto, int>(svc, store) { }
```

Ver el [README de `MoralesLarios.OOFP.WebControllers.Cache`](../MoralesLarios.OOFP.WebControllers.Cache/README.md) para detalles.
