# MoralesLarios.OOFP.WebServices

Capa de **servicios genéricos funcionales** que conecta el repositorio EF Core (`IEFRepoFp<TEntity>`) con los controladores web. Implementa el CRUD estándar devolviendo siempre `MlResult<T>`, mapea entidades ? DTOs con [Mapster](https://github.com/MapsterMapper/Mapster) y produce `ProblemDetails` ya estructurados (`MlProblemsDetails`) listos para que `MoralesLarios.OOFP.WebApi` los transforme en `IActionResult`.

Componentes principales:

- `IGenServiceFp<TEntity, TDto>` / `GenServiceFp<TEntity, TDto>`: servicio genérico de aplicación.
- `MlProblemsDetails`: factory de `MlErrorsDetails` con la estructura RFC 7807.
- `RegisterServices`: extensiones para registrar el servicio genérico (transient/scoped/singleton).

---

## Tabla de contenido

- [Dependencias](#dependencias)
- [Registro](#registro)
- [Estructura del proyecto](#estructura-del-proyecto)
- [`IGenServiceFp<TEntity, TDto>`](#igenservicefptentity-tdto)
- [`GenServiceFp<TEntity, TDto>`](#genservicefptentity-tdto)
- [`MlProblemsDetails`](#mlproblemsdetails)
- [Receta completa](#receta-completa)
- [Combinarlo con `WebControllers`](#combinarlo-con-webcontrollers)

---

## Dependencias

| Dependencia                                          | Tipo     |
|------------------------------------------------------|----------|
| `Mapster` 7.x                                        | NuGet    |
| `Microsoft.Extensions.Configuration.Abstractions` 9.x| NuGet    |
| `MoralesLarios.OOFP.EFCore`                          | Proyecto |
| `MoralesLarios.OOFP.Extensions.Loggers`              | Proyecto |
| `MoralesLarios.OOFP.Internals`                       | Proyecto |

Target framework: **`net8.0`**.

---

## Registro

`RegisterServices` ofrece tres extensiones para elegir el ciclo de vida del servicio genérico:

```csharp
using MoralesLarios.OOFP.WebServices;

services.AddTransientGenServicesFpWithoutReposGeneral();
// o
services.AddScopedtGenServicesFpWithoutReposGeneral();
// o
services.AddSingletonGenServicesFpWithoutReposGeneral();
```

Cada una registra:

```csharp
typeof(IGenServiceFp<,>) ? typeof(GenServiceFp<,>)
```

Necesitas también registrar el repositorio EF, p. ej.:

```csharp
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));
services.AddTransient(typeof(IEFRepoFp<>), typeof(EFRepoFp<>));
```

---

## Estructura del proyecto

```
MoralesLarios.OOFP.WebServices/
??? Helpers/
?   ??? Constants.cs
??? Services/
?   ??? IGenServiceFp.cs            // contrato del servicio genérico
?   ??? GenServiceFp.cs             // implementación funcional con Mapster
?   ??? GenService.cs               // (alternativa OOP, no usada por SimpleMlControllerBase)
??? MlProblemsDetails.cs            // factories de MlErrorsDetails (RFC 7807)
??? RegisterServices.cs
??? GlobalUsings.cs
```

---

## `IGenServiceFp<TEntity, TDto>`

Contrato funcional con todas las operaciones CRUD habituales. Cada método admite *callbacks* opcionales para construir mensajes de log de éxito y error.

```csharp
public interface IGenServiceFp<TEntity, TDto>
    where TEntity : class
    where TDto    : class
{
    Task<MlResult<IEnumerable<TDto>>> AllAsync(CancellationToken ct = default!,
                                               string                          initialMessage      = null!,
                                               Func<IEnumerable<TDto>, string> validMessageBuilder = null!,
                                               Func<MlErrorsDetails, string>   failMessageBuilder  = null!);

    Task<MlResult<TDto?>> FindByIdAsync                (..., params object[] pk);
    Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync (MlErrorsDetails notFoundErrorDetails, ..., params object[] pk);

    Task<MlResult<TDto>>  CreateAsync(TDto dto, ...);

    Task<MlResult<TDto>>  UpdateAsync                  (TDto dto, ..., params object[] pk);
    Task<MlResult<TDto>>  UpdateProblemDetailsAsync    (TDto dto, MlErrorsDetails notFoundErrorDetails, ..., params object[] pk);
    Task<MlResult<TDto>>  UpdateAsync                  (TDto dto, ...); // PK contenida en el DTO

    Task<MlResult<TDto>>  DeleteAsync                  (..., params object[] pk);
    Task<MlResult<TDto>>  DeleteProblemDetailsAsync    (MlErrorsDetails notFoundErrorDetails, ..., params object[] pk);
    Task<MlResult<TDto>>  DeleteAsync                  (TDto dto, ...);
}
```

### Diferencia entre `*Async` y `*ProblemDetailsAsync`

- `FindByIdAsync` / `UpdateAsync` / `DeleteAsync` construyen un *NotFound* genérico cuando la PK no existe.
- `FindByIdProblemsDetailsAsync` / `UpdateProblemDetailsAsync` / `DeleteProblemDetailsAsync` permiten que **tú decidas** qué `MlErrorsDetails` poner en el `fail` de "no encontrado" — útil para devolver `ProblemDetails` con título/detalle/tipo personalizados (e.g. `MlProblemsDetails.NotFoundError(...)`).

---

## `GenServiceFp<TEntity, TDto>`

Implementación funcional que:

1. Usa `IEFRepoFp<TEntity>` para acceder a la BD.
2. Mapea entidad ? DTO con `Mapster.Adapt<TX>()`.
3. Encadena operaciones con `BindAsync`/`MapAsync`/`TryMapAsync` para no lanzar excepciones.
4. Loguea en cada paso vía `LogMlResultInformationAsync` y `LogMlResultFinalAsync`.

### Ejemplo: comportamiento de `AllAsync`

```csharp
_logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of {typeof(TDto).Name}")
       .BindAsync ( _      => _repo.TryAllAsync(ct))
       .MapAsync  (entities => entities.Adapt<IEnumerable<TDto>>())
       .LogMlResultFinalAsync(_logger,
            validBuildMessage: x      => validMessageBuilder?.Invoke(x)      ?? $"Found {x.Count()} of {typeof(TDto).Name}",
            failBuildMessage : errors => failMessageBuilder?.Invoke(errors) ?? $"Error querying {typeof(TDto).Name}: {errors}");
```

### Ejemplo de uso

```csharp
public class UserAppService(IGenServiceFp<User, UserDto> _svc)
{
    public Task<MlResult<UserDto>> CreateAsync(UserDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct: ct,
            validMessageBuilder: x => $"User {x.Id} created",
            failMessageBuilder : e => $"User creation failed: {e}");

    public Task<MlResult<UserDto?>> FindAsync(int id, CancellationToken ct)
        => _svc.FindByIdProblemsDetailsAsync(
            notFoundErrorDetails: MlProblemsDetails.NotFoundError(detail: $"User {id} not found"),
            ct                  : ct,
            pk                  : id);
}
```

---

## `MlProblemsDetails`

Factory estática que produce `MlErrorsDetails` con la estructura RFC 7807, **lista** para que `MlResultWebExtensionsPlus.ToXxxPdActionResult` la transforme en `ObjectResult`/`ProblemDetails`.

Internamente todos los errores se almacenan bajo la clave `"ProblemsDetails"` con un objeto anónimo que contiene `Status`, `Title`, `Detail`, `Type`, `Errors`, `StatusCode`.

### API

```csharp
public static class MlProblemsDetails
{
    public static MlErrorsDetails CreateProblemDetails(int statusCode, string title = null!, string detail = null!, string type = null!, Dictionary<string,object> errors = null!);

    public static MlErrorsDetails BadRequestError(...);
    public static MlErrorsDetails BadRequestError(string title, string detail, string type, IEnumerable<ValidationResult> validationResults);
    public static MlErrorsDetails UnauthorizedError(...);
    public static MlErrorsDetails ForbiddenError(...);
    public static MlErrorsDetails NotFoundError(...);
    public static MlErrorsDetails ConflictError(...);
    public static MlErrorsDetails InternalServerError(...);
    // ...
}
```

### Ejemplo

```csharp
// En un servicio
return MlProblemsDetails.NotFoundError(
            title : "User not found",
            detail: $"No user exists with id {id}")
       .ToMlResultFail<UserDto>();

// El controlador hace:
//   .ToGetPdActionResultAsync()  -> 404 con ProblemDetails serializado
```

Se integra **automáticamente** con:

- `IGenServiceFp<,>.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemsDetails.NotFoundError())`
- `MlResultWebExtensionsPlus.ToGetPdActionResultAsync` / `ToPostPdActionResultAsync` / etc.

---

## Receta completa

```csharp
// 1. Repos EF
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
services.AddTransient(typeof(IEFRepoFp<>), typeof(EFRepoFp<>));

// 2. Servicios funcionales
services.AddTransientGenServicesFpWithoutReposGeneral();

// 3. Mapster (mappings personalizados, opcional)
TypeAdapterConfig<User, UserDto>.NewConfig().Map(d => d.FullName, s => s.Name + " " + s.Surname);

// 4. Controllers
services.AddControllers();
```

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> svc)
    : SimpleMlControllerBase<User, UserDto, int>(svc) { }
```

Con eso `WebControllers` + `WebServices` + `WebApi` orquestan todo el ciclo: petición HTTP ? servicio funcional ? repositorio EF ? DTO ? `IActionResult` con `ProblemDetails` correcto.

---

## Combinarlo con `WebControllers`

`SimpleMlControllerBase<TEntity, TDto, TPk>` (en `MoralesLarios.OOFP.WebControllers`) consume directamente `IGenServiceFp<TEntity, TDto>`. Registra primero las extensiones de este proyecto y heredar tus controladores es suficiente para tener el CRUD HTTP completo.

Ver:

- [`MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md)
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md)
- [`MoralesLarios.OOFP.WebControllers.Cache`](../MoralesLarios.OOFP.WebControllers.Cache/README.md) (variante con caché)
