# MoralesLarios.OOFP.WebApi

Capa **funcional** sobre ASP.NET Core MVC para Web API: convierte `MlResult<T>` en `IActionResult` con códigos HTTP correctos, genera `ProblemDetails` (RFC 7807) extendidos a partir de los errores funcionales y proporciona helpers para leer/escribir cabeceras de paginación y otros datos del request.

Es la pieza que pega tu **núcleo funcional** (`MlResult<T>`) con el **stack web** estándar de ASP.NET (`ControllerBase`, `IActionResult`, `ProblemDetails`).

---

## Tabla de contenido

- [Dependencias](#dependencias)
- [Registro](#registro)
- [Estructura del proyecto](#estructura-del-proyecto)
- [`MlActionResults`](#mlactionresults)
- [`ExtendedProblemDetails`](#extendedproblemdetails)
- [`ProblemDetailsInfo`](#problemdetailsinfo)
- [`MlResultWebExtensionsPlus` (uso recomendado)](#mlresultwebextensionsplus-uso-recomendado)
- [`MlErrorsDetailsExtensions`](#mlerrorsdetailsextensions)
- [`MlRequestWebExtensions`](#mlrequestwebextensions)
- [`MlResultWebExtensions` (legacy)](#mlresultwebextensions-legacy)
- [Receta completa](#receta-completa)

---

## Dependencias

| Dependencia                            | Tipo     |
|----------------------------------------|----------|
| `Microsoft.AspNetCore.Mvc.Core` 2.x    | NuGet    |
| `MoralesLarios.OOFP` (FOOP)            | Proyecto |
| `MoralesLarios.OOFP.Internals`         | Proyecto |
| `MoralesLarios.OOFP.ValueObjects`      | Proyecto |

Target framework: **`net8.0`**.

---

## Registro

Este proyecto **no registra servicios** propios (no tiene `RegisterServices`). Se consume importando los namespaces:

```csharp
using MoralesLarios.OOFP.WebApi;
using MoralesLarios.OOFP.WebApi.Helpers;
using MoralesLarios.OOFP.WebApi.Data;
```

> Sugerencia: muévelos al `GlobalUsings.cs` de tu Web API para no repetirlos en cada controlador.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.WebApi/
??? Data/
?   ??? ProblemDetailsInfo.cs            // record con la info de ProblemDetails
??? Helpers/
?   ??? MlErrorsDetailsExtensions.cs     // MlErrorsDetails -> ProblemDetailsInfo
?   ??? MlRequestWebExtensions.cs        // headers del HttpRequest
?   ??? MlResultWebExtensions.cs         // [Obsolete] viejas conversiones repo*
?   ??? MlResultWebExtensionsPlus.cs     // conversiones a IActionResult RECOMENDADAS
??? MlActionResults.cs                   // factories de IActionResult con ProblemDetails
??? GlobalUsings.cs
```

---

## `MlActionResults`

Factories estáticas de `IActionResult` con `ExtendedProblemDetails` ya rellenado para los códigos HTTP más habituales.

```csharp
public static class MlActionResults
{
    public static IActionResult CreateProblemsDetails(int statusCode, string title = null!, string detail = null!, string type = null!, Dictionary<string,object> errors = null!);

    public static IActionResult BadRequest         (...);
    public static IActionResult Unauthorized       (...);
    public static IActionResult Forbidden          (...);
    public static IActionResult NotFound           (...);
    public static IActionResult Conflict           (...);
    public static IActionResult InternalServerError(...);
    // ...
}
```

Todas devuelven un `ObjectResult` con `Status`, `Title`, `Detail`, `Type`, `Errors` y el `StatusCode` HTTP correcto.

```csharp
return MlActionResults.NotFound(detail: $"User '{id}' not found");

return MlActionResults.BadRequest(validationResults: results); // con errores agrupados por miembro
```

---

## `ExtendedProblemDetails`

Extensión de `ProblemDetails` con un diccionario de errores adicionales:

```csharp
public class ExtendedProblemDetails : ProblemDetails
{
    public Dictionary<string, object> Errors { get; set; } = new();
}
```

Es lo que devuelven las factories de `MlActionResults` y la respuesta JSON sigue el formato:

```json
{
  "type"      : "https://www.puntonetalpunto.net/",
  "title"     : "Not found",
  "status"    : 404,
  "detail"    : "User '42' not found",
  "errors"    : { }
}
```

---

## `ProblemDetailsInfo`

Record inmutable con los datos crudos de un problem details.

```csharp
public record ProblemDetailsInfo(
    int                        Status,
    string                     Title,
    string                     Detail,
    string                     Type,
    Dictionary<string, object> Errors,
    int                        StatusCode);
```

Se utiliza para transportar la información extraída de `MlErrorsDetails` antes de convertirla a `IActionResult`.

---

## `MlResultWebExtensionsPlus` (uso recomendado)

API moderna para convertir `MlResult<T>` (síncrono o asíncrono) en `IActionResult` por verbo HTTP, generando automáticamente `ProblemDetails` cuando el resultado es `fail`.

| Método                                                | Caso valid                                | Caso fail (ProblemDetails)            |
|-------------------------------------------------------|-------------------------------------------|---------------------------------------|
| `ToGetPdActionResult` / `ToGetPdActionResultAsync`    | `Ok(value)`                               | `ProblemDetails` o `500`              |
| `ToPostPdActionResult(uri)`                           | `Created(uri, value)`                     | `ProblemDetails` o `500`              |
| `ToPostPdActionResult()`                              | `Created("default-uri", value)`           | `ProblemDetails` o `500`              |
| `ToPutPdActionResult` / `Async`                       | `NoContent()` (204)                       | `ProblemDetails` o `500`              |
| `ToPatchPdActionResult` / `Async`                     | `NoContent()` (204)                       | `ProblemDetails` o `500`              |
| `ToDeletePdActionResult` / `Async`                    | `NoContent()` (204)                       | `ProblemDetails` o `500`              |
| `ToMlActionResult(this ProblemDetailsInfo)`           | -                                         | Construye el `IActionResult` final    |

### Ejemplos

```csharp
[HttpGet]
public Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    => _service.AllAsync(ct: ct).ToGetPdActionResultAsync();

[HttpPost]
public Task<IActionResult> CreateAsync([FromBody] UserDto dto)
    => _service.CreateAsync(dto).ToPostPdActionResultAsync(new Uri("/users", UriKind.Relative));

[HttpDelete("{id}")]
public Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default)
    => _service.DeleteByIdAsync(id, ct).ToDeletePdActionResultAsync();
```

> Para que se genere `ProblemDetails` automáticamente, los `fail` deben llevar el detalle bajo la clave `ProblemsDetails` (lo hace por ti `MoralesLarios.OOFP.WebServices.MlProblemsDetails`).

---

## `MlErrorsDetailsExtensions`

Convierte `MlErrorsDetails` (la parte de error de un `MlResult<T>`) en `ProblemDetailsInfo` listo para serializar.

```csharp
public static MlResult<ProblemDetailsInfo> GetProblemDetails(this MlErrorsDetails source);
public static MlResult<ProblemDetailsInfo> ToProblemsDetailsInfo(this object obj);
```

Si los detalles del error contienen una clave `ProblemsDetails` con un objeto que tenga las propiedades estándar (`Status`, `Title`, `Detail`, `Type`, `Errors`, `StatusCode`), se construye un `ProblemDetailsInfo` por reflexión. Si falta alguna propiedad o la clave no está, se devuelve `fail`.

Es el motor que utiliza `MlResultWebExtensionsPlus` internamente.

---

## `MlRequestWebExtensions`

Helpers funcionales para extraer cabeceras de un `HttpRequest`.

```csharp
public static MlResult<NotEmptyString> GetHeaderInfo(this HttpRequest request, Name headerKey);
public static MlResult<IntNotNegative> GetHeaderInfoAsIntNotNegative(this HttpRequest request, Name headerKey);

public static MlResult<IntNotNegative>                                       GetHeaderPageNumber(this HttpRequest request);   // X-Page-Number
public static MlResult<IntNotNegative>                                       GetHeaderPageSize  (this HttpRequest request);   // X-Page-Size
public static MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> GetHeaderPageInfo  (this HttpRequest request);
```

### Ejemplo

```csharp
[HttpGet("page")]
public async Task<IActionResult> GetPageAsync(CancellationToken ct = default)
{
    var pageInfo = HttpContext.Request.GetHeaderPageInfo();

    var result = await pageInfo
        .BindAsync(p => _service.PageAsync(p.PageNumber, p.PageSize, ct))
        .ToGetPdActionResultAsync();

    return result;
}
```

---

## `MlResultWebExtensions` (legacy)

```csharp
[Obsolete("This class is deprecated and should not be used.")]
public static class MlResultWebExtensions
{
    public static IActionResult ToRepoGetActionResult <T>(this MlResult<T> source, ControllerBase ctrl);
    public static IActionResult ToRepoPutActionResult <T>(this MlResult<T> source, ControllerBase ctrl);
    public static IActionResult ToRepoPostActionResult<T>(this MlResult<T> source, ControllerBase ctrl);
    // ...
}
```

Se mantiene únicamente por compatibilidad. **Usa `MlResultWebExtensionsPlus`** para nuevos desarrollos.

---

## Receta completa

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> _service) : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> GetAllAsync(CancellationToken ct = default)
        => _service.AllAsync(ct: ct)
                   .ToGetPdActionResultAsync();

    [HttpGet("{id}")]
    public Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default)
        => EnsureFp.NotNullAsync(id, "id required")
                   .BindAsync(_ => _service.FindByIdProblemsDetailsAsync(
                       notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                       ct: ct,
                       pk: id))
                   .ToGetPdActionResultAsync();

    [HttpPost]
    public Task<IActionResult> CreateAsync([FromBody] UserDto dto)
        => _service.CreateAsync(dto)
                   .ToPostPdActionResultAsync(new Uri("/api/users", UriKind.Relative));
}
```
