# MoralesLarios.OOFP.AspNetCore.Results — el puente entre el raíl `MlResult<T>` y ASP.NET Core MVC

Este proyecto es la **capa de salida HTTP** del ecosistema FOOP. Su única responsabilidad es traducir un `MlResult<T>` —el tipo del raíl funcional del núcleo— en un `IActionResult` de ASP.NET Core, con el código de estado correcto y un cuerpo `ProblemDetails` (RFC 7807) enriquecido. También ofrece el camino inverso: leer cabeceras HTTP y convertirlas en *value objects* validados que entran limpiamente en el raíl.

La idea de fondo es simple: **tu controlador no debería contener ni un solo `if`**. La lógica de negocio devuelve `MlResult<T>`; una sola llamada de extensión decide si eso es un `200 OK`, un `201 Created`, un `204 No Content`, un `404 Not Found` o un `500 Internal Server Error`. El controlador queda reducido a una línea por acción.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Arquitectura: el flujo completo de un error](#arquitectura-el-flujo-completo-de-un-error)
5. [`ExtendedProblemDetails`](#extendedproblemdetails)
6. [`MlResults` — fábrica de respuestas de error](#mlactionresults--fábrica-de-respuestas-de-error)
7. [`ProblemDetailsInfo`](#problemdetailsinfo)
8. [`MlErrorDetailsExtensions` — el puente por reflexión](#mlerrorsdetailsextensions--el-puente-por-reflexión)
9. [`MlResultActionResultExtensions` — la capa recomendada](#mlresultwebextensionsplus--la-capa-recomendada)
10. [`HttpRequestPaginationExtensions` — cabeceras HTTP al raíl](#mlrequestwebextensions--cabeceras-http-al-raíl)
11. [`MlResultActionResultExtensionsLegacy` — capa legacy (`[Obsolete]`)](#mlresultwebextensions--capa-legacy-obsolete)
12. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
13. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
14. [Ejemplos prácticos](#ejemplos-prácticos)
15. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
16. [Mejores prácticas](#mejores-prácticas)
17. [Resumen](#resumen)
18. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

Cuando adoptas el patrón *railway* con `MlResult<T>`, tus servicios dejan de lanzar excepciones y dejan de devolver `null`. Todo viaja como "válido" o "fallido". Pero ASP.NET Core no entiende `MlResult<T>`: espera un `IActionResult`. Ese salto de mundos es donde normalmente se acumula el código repetitivo y donde se pierde la información del error.

**❌ Sin este proyecto** — cada acción del controlador se convierte en un árbol de decisiones:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id)
{
    var result = await _service.FindByIdAsync(id);

    if (result.IsSuccess)
        return Ok(result.GetValueOrThrow());

    var errors = result.GetFailureDetailsOrThrow();

    // ¿Es un "no encontrado"? ¿Cómo lo sé? Comparando cadenas...
    if (errors.GetErrorsSummary().Contains("not found", StringComparison.OrdinalIgnoreCase))
        return NotFound(new ProblemDetails { Status = 404, Title = "No encontrado" });

    if (errors.ContainsException())
        return StatusCode(500, new ProblemDetails { Status = 500, Title = "Error interno" });

    return StatusCode(500, new ProblemDetails { Status = 500, Detail = errors.GetErrorsSummary() });
}
```

Repite eso en 40 endpoints y tendrás 40 variantes ligeramente distintas del mismo `switch`, cada una perdiendo un matiz diferente del error original.

**✅ Con este proyecto** — el error ya viaja *tipado* dentro del `MlResult` y la traducción es una llamada:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id)
    => await _service.FindByIdProblemsDetailsAsync(id).ToOkProblemDetailsResultAsync();
```

💡 **La clave**: quien conoce el significado del error es la capa que lo produce, no el controlador. Por eso `MoralesLarios.OOFP.Application` adjunta al `MlErrorDetails` un detalle bajo la clave `"ProblemsDetails"` con el `Status`, `Title`, `Detail`, `Type` y `Errors` definitivos. Este proyecto solo lo **lee y lo re-emite**. El controlador no decide nada.

---

## Instalación y dependencias

```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.1.0" />
```

Referencias de proyecto:

| Proyecto referenciado | Para qué |
|---|---|
| `MoralesLarios.OOFP` | `MlResult<T>`, `MlErrorDetails`, `Match`, `Bind`, `Map`, `MapEnsure` |
| `MoralesLarios.OOFP.Shared` | `PageRequest` (resultado de `GetHeaderPaginationInfo`) |
| `MoralesLarios.OOFP.ValueObjects` | `MemberName`, `NotEmptyString`, `IntNonNegative` (tipos de las cabeceras) |

> ⚠️ **Aviso sobre el paquete NuGet.** El proyecto compila contra `net8.0` pero referencia `Microsoft.AspNetCore.Mvc.Core` **2.1.0**, un paquete de la era .NET Core 2.1. En una aplicación ASP.NET Core moderna eso genera duplicidad de ensamblados y advertencias de resolución: lo correcto sería `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. Ver [Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente).

`GlobalUsings.cs` ya expone, sin que tengas que importar nada:

```csharp
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;
global using MoralesLarios.OOFP.Types;
global using MoralesLarios.OOFP.Types.Errors;
global using MoralesLarios.OOFP.ValueObjects;
global using MoralesLarios.OOFP.Shared.Pagination;
global using MoralesLarios.OOFP.AspNetCore.Results.Data;
global using MoralesLarios.OOFP.AspNetCore.Results.Helpers;
global using MoralesLarios.OOFP.Helpers;
global using static MoralesLarios.OOFP.Helpers.Constants;
global using static MoralesLarios.OOFP.AspNetCore.Results.Helpers.Extensions;
```

> 💡 **No hay `RegisterServices`.** Este proyecto **no registra nada en el contenedor de dependencias**: todo son clases y métodos de extensión estáticos. Basta con referenciarlo.

---

## Estructura del proyecto

```text
MoralesLarios.OOFP.AspNetCore.Results/
├── MlResults.cs                    → ExtendedProblemDetails + 15 fábricas de IActionResult
├── Data/
│   └── ProblemDetailsInfo.cs             → record con la forma canónica de un ProblemDetails
├── Helpers/
│   ├── MlResultActionResultExtensions.cs      → ⭐ capa recomendada (ToGetPd…, ToPostPd…, ToPutPd…)
│   ├── MlErrorDetailsExtensions.cs      → MlErrorDetails → ProblemDetailsInfo (por reflexión)
│   ├── HttpRequestPaginationExtensions.cs         → HttpRequest → MlResult<value object>
│   └── MlResultActionResultExtensionsLegacy.cs          → ⛔ capa legacy, la clase entera está [Obsolete]
└── GlobalUsings.cs
```

Tres bloques funcionales bien separados:

| Bloque | Dirección | Tipos implicados |
|---|---|---|
| **Salida de datos** | `MlResult<T>` → `IActionResult` | `MlResultActionResultExtensions`, `MlResults` |
| **Traducción de errores** | `MlErrorDetails` → `ProblemDetailsInfo` | `MlErrorDetailsExtensions` |
| **Entrada de datos** | `HttpRequest` → `MlResult<T>` | `HttpRequestPaginationExtensions` |

---

## Arquitectura: el flujo completo de un error

Para entender este proyecto hay que ver el recorrido completo de un error desde el repositorio hasta el JSON que recibe el cliente:

```text
1. EFCore                 TryFindAsync(id) devuelve fail
                              │
2. WebServices            MlProblemDetails.NotFoundError("Cliente 7 no existe")
   (MlProblemDetails)    crea un MlErrorDetails con:
                              Errors  = [ "Cliente 7 no existe" ]
                              Details = { ["ProblemsDetails"] = new {
                                             Status = 404, Title = "...",
                                             Detail = "...", Type = "...",
                                             Errors = {...}, StatusCode = 404 } }
                              │
3. WebApi                 errors.GetProblemDetails()
   (MlErrorDetails-        → lee Details["ProblemsDetails"]
    Extensions)            → por reflexión lo convierte en ProblemDetailsInfo
                              │
4. WebApi                 pdInfo.ToMlActionResult()
   (MlResults)       → ObjectResult(ExtendedProblemDetails) { StatusCode = 404 }
                              │
5. ASP.NET Core           HTTP/1.1 404 Not Found
                          { "status":404, "title":"...", "detail":"...",
                            "type":"...", "errors":{...} }
```

> ❗ **El punto crítico está en el paso 2.** Si la capa de servicio **no** adjunta el detalle `"ProblemsDetails"`, el paso 3 falla y `MlResultActionResultExtensions` cae al *fallback*: `MlResults.InternalServerError()`. Es decir: **un error de negocio perfectamente legítimo se convierte en un 500**. Por eso en `WebServices` debes usar los métodos con sufijo `ProblemsDetails` (`FindByIdProblemsDetailsAsync`, `UpdateAsync`, `DeleteProblemDetailsAsync`) y construir tus errores con `MlProblemDetails.*`.

---

## `ExtendedProblemDetails`

`ProblemDetails` estándar de ASP.NET Core no tiene un sitio natural para un diccionario de errores por campo (lo que sí tiene `ValidationProblemDetails`, pero solo con `string[]`). Este tipo lo añade:

```csharp
public class ExtendedProblemDetails : ProblemDetails
{
    public Dictionary<string, object> Errors { get; set; } = new();
}
```

Al heredar de `ProblemDetails` mantiene `Status`, `Title`, `Detail`, `Type`, `Instance` y `Extensions`, y añade `Errors` como propiedad de primer nivel, serializada como `"errors"`. El valor es `object`, de modo que puedes anidar estructuras arbitrarias (no solo listas de cadenas).

```jsonc
{
  "type": "https://www.puntonetalpunto.net/",
  "title": "Datos de entrada no válidos",
  "status": 400,
  "detail": "Revise los campos marcados",
  "errors": {
    "Email":  [ "El formato del email no es válido" ],
    "Edad":   [ "Debe ser mayor que 0", "Debe ser menor que 120" ]
  }
}
```

---

## `MlResults` — fábrica de respuestas de error

Clase estática con **15 fábricas**. Todas devuelven `IActionResult` (concretamente un `ObjectResult` cuyo cuerpo es un `ExtendedProblemDetails` y cuyo `StatusCode` coincide con el `Status` del cuerpo).

```csharp
public static class MlResults
{
    public static IActionResult CreateProblemsDetails(int statusCode,
                                                     string? title  = null,
                                                     string? detail = null,
                                                     string? type   = null,
                                                     Dictionary<string, object>? errors = null);
    // …y 14 atajos por código de estado
}
```

| Método | Código | Cuándo usarlo |
|---|---:|---|
| `CreateProblemsDetails(int statusCode, …)` | *cualquiera* | Base genérica; úsala para códigos no cubiertos |
| `BadRequest(…)` | 400 | Entrada mal formada o inválida |
| `BadRequest(IEnumerable<ValidationResult>, …)` | 400 | Sobrecarga que agrupa los `ValidationResult` por `MemberNames` |
| `Unauthorized(…)` | 401 | Falta autenticación o el token no es válido |
| `Forbidden(…)` | 403 | Autenticado pero sin permisos |
| `NotFound(…)` | 404 | El recurso no existe |
| `MethodNotAllowed(…)` | 405 | Verbo HTTP no soportado por el recurso |
| `Conflict(…)` | 409 | Conflicto de estado (duplicado, concurrencia) |
| `UnprocessableEntity(…)` | 422 | Sintaxis correcta pero semántica inválida |
| `TooManyRequests(…)` | 429 | *Rate limiting* |
| `InternalServerError(…)` | 500 | Fallo inesperado del servidor |
| `NotImplemented(…)` | 501 | Operación no implementada |
| `BadGateway(…)` | 502 | Un servicio aguas abajo respondió mal |
| `ServiceUnavailable(…)` | 503 | Servicio temporalmente caído / en mantenimiento |
| `GatewayTimeout(…)` | 504 | Un servicio aguas abajo no respondió en plazo |

**Uso directo** (poco frecuente: normalmente las invoca `MlResultActionResultExtensions` por ti):

```csharp
[HttpGet("secreto")]
public IActionResult Secreto()
    => User.IsInRole("admin")
        ? Ok(new { mensaje = "Hola, admin" })
        : MlResults.Forbidden("Acceso denegado", "Se requiere el rol 'admin'");
```

**Sobrecarga de validación** — convierte los `ValidationResult` de DataAnnotations en el diccionario `Errors`, agrupando por nombre de miembro:

```csharp
var validaciones = new List<ValidationResult>
{
    new("El formato del email no es válido", new[] { "Email" }),
    new("Debe ser mayor que 0",              new[] { "Edad"  }),
    new("Debe ser menor que 120",            new[] { "Edad"  })
};

return MlResults.BadRequest(validaciones, "Datos de entrada no válidos");
// → 400 con errors = { "Email": [...], "Edad": [ "...", "..." ] }
```

> ⚠️ **`type` por defecto.** Si no pasas `type`, se usa la cadena literal `"https://www.puntonetalpunto.net/"` (el dominio del autor de la librería). Según la RFC 7807 el `type` debe ser un URI que **documente tu error**, así que en producción **pásalo siempre** o centralízalo en un *wrapper* propio.

> 💡 **Espejo de `MlProblemDetails`.** Este catálogo de 15 métodos es el reflejo exacto del catálogo `MlProblemDetails` de `MoralesLarios.OOFP.Application`. La diferencia: `MlProblemDetails.*` devuelve `MlErrorDetails` (para meterlo en el raíl) y `MlResults.*` devuelve `IActionResult` (para salir por HTTP).

---

## `ProblemDetailsInfo`

Un `record` que representa la forma canónica y **tipada** de un problema, sin acoplarse a ASP.NET Core:

```csharp
public record ProblemDetailsInfo(int                        Status,
                                 string                     Title,
                                 string                     Detail,
                                 string                     Type,
                                 Dictionary<string, object> Errors,
                                 int                        StatusCode);
```

Es el tipo intermedio del puente: `MlErrorDetails` → **`ProblemDetailsInfo`** → `IActionResult`.

> ⚠️ `Status` y `StatusCode` son **redundantes**: en todos los caminos del código llevan el mismo valor. `Status` sirve al cuerpo JSON y `StatusCode` a la respuesta HTTP, pero nada garantiza que coincidan si construyes el record a mano.

---

## `MlErrorDetailsExtensions` — el puente por reflexión

Dos métodos de extensión:

```csharp
public static MlResult<ProblemDetailsInfo> GetProblemDetails(this MlErrorDetails source);
public static MlResult<ProblemDetailsInfo> ToProblemDetails(this object obj);
```

### `GetProblemDetails`

Es la implementación literal, escrita en el propio raíl:

```csharp
public static MlResult<ProblemDetailsInfo> GetProblemDetails(this MlErrorDetails source)
    => MlResult.Empty()
        .MapEnsure(_   => source.ContainsMetadata(ProblemsDetails),
                   _   => "The MlErrorDetails does not have details.")
        .Map      (_   => source.Metadata[ProblemsDetails])
        .MapEnsure(obj => obj is not null,
                   _   => "The details ProblemsDetails key has a null object.")
        .Bind     (obj => obj.ToProblemDetails());
```

| Paso | Qué comprueba | Si falla |
|---|---|---|
| `MapEnsure` #1 | Existe la clave `"ProblemsDetails"` en `Details` | fail `"The MlErrorDetails does not have details."` |
| `Map` | Extrae el objeto asociado | — |
| `MapEnsure` #2 | El objeto no es `null` | fail `"The details ProblemsDetails key has a null object."` |
| `Bind` | Delegación a `ToProblemDetails` | propaga el fail |

### `ToProblemDetails`

Recibe un `object` (en la práctica, el **tipo anónimo** que crea `MlProblemDetails`) y lo convierte a `ProblemDetailsInfo` **leyendo sus propiedades por reflexión**: `Status`, `Title`, `Detail`, `Type`, `Errors` y `StatusCode`. Si una propiedad no existe o no se puede convertir, aplica un valor por defecto (`500`, `"Error"`, cadena vacía, diccionario vacío).

> ⚠️ **Contrato implícito y frágil.** El acoplamiento entre `MlProblemDetails` (proyecto `WebServices`) y este método es **por nombre de propiedad, en tiempo de ejecución, sin interfaz común**. Renombrar `StatusCode` en un lado degrada silenciosamente a `500` en el otro, sin error de compilación y sin excepción. Es una de las deudas técnicas más relevantes de la solución (ver [Particularidades](#️-particularidades-reales-del-código-fuente)).

**Uso manual** (cuando escribes tu propio *mapper* de errores):

```csharp
IActionResult Traducir(MlErrorDetails errors)
    => errors.GetProblemDetails()
             .Match(valid: pd => pd.ToMlActionResult(),
                    fail : _  => MlResults.InternalServerError(
                                     "Error no clasificado",
                                     errors.GetErrorsSummary()));
```

---

## `MlResultActionResultExtensions` — la capa recomendada

⭐ **Este es el 95 % del uso real del proyecto.** Un método por verbo HTTP, con su variante `Async`, que decide el `IActionResult` por ti.

### Comportamiento en el camino válido

| Método | Resultado válido | Cuerpo |
|---|---|---|
| `ToOkProblemDetailsResult<T>()` | `200 OK` (`OkObjectResult`) | el valor `T` |
| `ToCreatedProblemDetailsResult<T>(Uri uri)` | `201 Created` (`CreatedResult`) | el valor `T`, con cabecera `Location: uri` |
| `ToCreatedProblemDetailsResult<T>()` | `201 Created` | el valor `T`, ⚠️ con `Location` **fija** |
| `ToNoContentProblemDetailsResult<T>()` | `204 No Content` | *(vacío)* |
| `ToPatchedProblemDetailsResult<T>()` | `204 No Content` | *(vacío)* |
| `ToDeletedProblemDetailsResult<T>()` | `204 No Content` | *(vacío)* |

### Comportamiento en el camino fallido

Idéntico en **todos** los métodos:

```csharp
errors.GetProblemDetails()
      .Match(valid: pd => pd.ToMlActionResult(),          // el status real: 404, 409, 422…
             fail : _  => MlResults.InternalServerError());  // fallback
```

### Sobrecargas asíncronas

Cada método tiene su gemelo para trabajar directamente sobre `Task<MlResult<T>>`, de modo que no necesitas `await` intermedio:

```csharp
// sobre MlResult<T>            → devuelve IActionResult
// sobre Task<MlResult<T>>      → devuelve Task<IActionResult>
public static async Task<IActionResult> ToOkProblemDetailsResultAsync<T>(this Task<MlResult<T>> source);
public static async Task<IActionResult> ToNoContentProblemDetailsResultAsync  <T>(this Task<MlResult<T>> source);
public static async Task<IActionResult> ToPatchedProblemDetailsResultAsync<T>(this Task<MlResult<T>> source);
public static async Task<IActionResult> ToDeletedProblemDetailsResultAsync<T>(this Task<MlResult<T>> source);
public static async Task<IActionResult> ToPostActionResultAsync   <T>(this Task<MlResult<T>> source, Uri uri);
public static async Task<IActionResult> ToPostActionResultAsync   <T>(this Task<MlResult<T>> source);
```

> ⚠️ **Nomenclatura asimétrica en POST.** Las sobrecargas que reciben `Task<MlResult<T>>` se llaman `ToPostActionResultAsync` (**sin `Pd`**), mientras que el resto de verbos sí conservan el `Pd` (`ToNoContentProblemDetailsResultAsync`). Si buscas `ToCreatedProblemDetailsResultAsync` sobre un `Task`, no lo encontrarás.

### Puente auxiliar

```csharp
public static IActionResult ToMlActionResult(this ProblemDetailsInfo source)
    => MlResults.CreateProblemsDetails(source.StatusCode,
                                             source.Title,
                                             source.Detail,
                                             source.Type,
                                             source.Errors);
```

### Controlador completo con la capa Plus

```csharp
[ApiController]
[Route("api/clientes")]
public class ClientesController(ICrudServiceFp<Cliente, ClienteDto> service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public Task<IActionResult> Get(int id)
        => service.FindByIdProblemsDetailsAsync(id).ToOkProblemDetailsResultAsync();

    [HttpPost]
    public Task<IActionResult> Post(ClienteDto dto)
        => service.CreateAsync(dto)
                  .ToPostActionResultAsync(new Uri($"/api/clientes", UriKind.Relative));

    [HttpPut("{id:int}")]
    public Task<IActionResult> Put(int id, ClienteDto dto)
        => service.UpdateAsync(dto, id).ToNoContentProblemDetailsResultAsync();

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id)
        => service.DeleteProblemDetailsAsync(id).ToDeletedProblemDetailsResultAsync();
}
```

Cuatro endpoints, cuatro líneas, cero `if`, cero `try/catch`, y los códigos de estado los decide la capa de negocio.

---

## `HttpRequestPaginationExtensions` — cabeceras HTTP al raíl

La dirección contraria: leer cabeceras de un `HttpRequest` y devolverlas **ya validadas** dentro de un `MlResult`. Nunca lanza; si la cabecera falta, está vacía o no es un entero, obtienes un `fail`.

```csharp
// Cabecera genérica
public static MlResult<NotEmptyString> GetHeaderInfo(this HttpRequest source, Name headerKey);
public static Task<MlResult<NotEmptyString>> GetHeaderInfoAsync(this HttpRequest source, Name headerKey);

// Cabecera genérica convertida a entero no negativo
public static MlResult<IntNonNegative> GetHeaderInfoAsIntNonNegative(this HttpRequest source, Name headerKey);
public static Task<MlResult<IntNonNegative>> GetHeaderInfoAsIntNotNegativeAsync(this HttpRequest source, Name headerKey);

// Cabeceras de paginación (nombres fijos)
public static MlResult<IntNonNegative> GetHeaderPageNumber(this HttpRequest source);   // "X-Page-Number"
public static MlResult<IntNonNegative> GetHeaderPageSize  (this HttpRequest source);   // "X-Page-Size"

// Combinaciones
public static MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> GetHeaderPageInfo(this HttpRequest source);
public static MlResult<PageRequest> GetHeaderPaginationInfo(this HttpRequest source);
```

*(todos tienen su variante `…Async` que devuelve `Task<MlResult<…>>`)*

| Método | Devuelve | Cabecera(s) leída(s) |
|---|---|---|
| `GetHeaderInfo` | `NotEmptyString` | la que indiques |
| `GetHeaderInfoAsIntNonNegative` | `IntNonNegative` | la que indiques |
| `GetHeaderPageNumber` | `IntNonNegative` | `X-Page-Number` |
| `GetHeaderPageSize` | `IntNonNegative` | `X-Page-Size` |
| `GetHeaderPageInfo` | tupla `(PageNumber, PageSize)` | ambas |
| `GetHeaderPaginationInfo` | `PageRequest` | ambas |

**Ejemplo — endpoint paginado que lee la paginación de las cabeceras:**
```csharp
[HttpGet]
public Task<IActionResult> All()
    => Request.GetHeaderPaginationInfo()                       // MlResult<PageRequest>
              .BindAsync(pag => _repo.TryAllPaginationAsync(pag.PageNumber, pag.PageSize))
              .ToOkProblemDetailsResultAsync();
```

Si el cliente envía `X-Page-Size: abc`, el raíl se corta en la primera etapa y nunca se toca la base de datos.

**Ejemplo — cabecera de negocio arbitraria:**
```csharp
[HttpGet("informe")]
public Task<IActionResult> Informe()
    => Request.GetHeaderInfo("X-Tenant-Id")                    // MlResult<NotEmptyString>
              .Bind(tenant => _service.ValidarTenant(tenant))
              .BindAsync(t  => _service.GenerarInformeAsync(t))
              .ToOkProblemDetailsResultAsync();
```

> ⚠️ Los nombres `"X-Page-Number"` y `"X-Page-Size"` están **incrustados como literales** dentro de los métodos y no se exponen como constantes públicas, así que tu cliente HTTP tiene que replicarlos a mano. Además, los mensajes de fallo están redactados en inglés y no son configurables.

> 💡 `PageRequest` (de `MoralesLarios.OOFP.Shared`) **normaliza** los valores en el constructor: `PageNumber` se eleva a un mínimo de 1 y `PageSize` se recorta al rango `[1, 1000]`. Así que un `X-Page-Size: 999999` no tumba el servidor: se convierte en 1000.

---

## `MlResultActionResultExtensionsLegacy` — capa legacy (`[Obsolete]`)

```csharp
[Obsolete("This class is deprecated and should not be used.")]
public static class MlResultActionResultExtensionsLegacy
```

**La clase entera está marcada como obsoleta.** Se documenta aquí únicamente para que puedas **reconocerla y migrarla** en código existente; no la uses en desarrollo nuevo.

Su diferencia conceptual es importante: en lugar de leer el detalle `"ProblemsDetails"`, **adivina** el código de estado inspeccionando el texto del error:

```csharp
private static IEnumerable<string> notFoundKeys =
    [ "NotFound", "Not Found", "Not_Found", "not found", "NoEncontrado", /* … */ ];
```

Lógica de `ToRepoActionResult`:

| Condición | Resultado |
|---|---|
| `IsSuccess` | lo que devuelva el `OkHandler` que le pases |
| la descripción del error contiene alguna `notFoundKeys` | `NotFoundObjectResult` |
| `errors.ContainsMetadata("NotFound")` | `NotFoundObjectResult` |
| `errors.ContainsException()` | `ObjectResult` con `StatusCode = 500` y la descripción de la excepción |
| resto | `ObjectResult` con `StatusCode = 500` |

Métodos que expone (todos obsoletos): `ToRepoGetActionResult` (+2 `Async`), `ToRepoPutActionResult` (+`Async`), `ToRepoPostActionResult` (×2 +3 `Async`), `ToCreatedResult` (+2 `Async`), `ToRepoDeleteActionResult` (+2 `Async`), `ToRepoActionResult` (+2 `Async`).

**Tabla de migración:**

| Legacy (`MlResultActionResultExtensionsLegacy`) | Reemplazo (`MlResultActionResultExtensions`) |
|---|---|
| `ToRepoGetActionResult` / `…Async` | `ToOkProblemDetailsResult` / `ToOkProblemDetailsResultAsync` |
| `ToRepoPostActionResult` / `ToCreatedResult` | `ToCreatedProblemDetailsResult(uri)` / `ToPostActionResultAsync(uri)` |
| `ToRepoPutActionResult` / `…Async` | `ToNoContentProblemDetailsResult` / `ToNoContentProblemDetailsResultAsync` |
| `ToRepoDeleteActionResult` / `…Async` | `ToDeletedProblemDetailsResult` / `ToDeletedProblemDetailsResultAsync` |
| `ToRepoActionResult` / `…Async` | combinación de `GetProblemDetails()` + `ToMlActionResult()` |

> ❗ **Defectos conocidos de la capa legacy** (motivo de su obsolescencia): `ToRepoPostActionResult` invoca `controllerBase.Created("NotUri", new object())` —literal `"NotUri"` como `Location` y **descarta el valor**—; `ToCreatedResult` devuelve `Created` **incluso cuando el resultado es fallido**; la detección de "no encontrado" por comparación de cadenas es intrínsecamente frágil; y hay mensajes y nombres de método en español mezclados (`ContainsAnyKey`).

---

## ⚠️ Particularidades reales del código fuente

Cosas que **no** deducirías de las firmas y que conviene conocer antes de apoyarte en este proyecto:

1. **`Microsoft.AspNetCore.Mvc.Core` 2.1.0 sobre `net8.0`.** El `.csproj` referencia un paquete NuGet de la era .NET Core 2.1. En una app ASP.NET Core moderna esto puede producir advertencias de resolución y ensamblados duplicados. Lo correcto sería `<FrameworkReference Include="Microsoft.AspNetCore.App" />`.

2. **El paquete no registra servicios.** No existe `RegisterServices`. Si buscas un `AddOOFPWebApi()`, no lo hay: todo son extensiones estáticas.

3. **El `type` por defecto es el dominio del autor.** `MlResults` usa `"https://www.puntonetalpunto.net/"` cuando no pasas `type`. Toda tu API expondrá esa URL en cada error si no lo sobrescribes.

4. **`ToCreatedProblemDetailsResult<T>(this MlResult<T>)` tiene el `Location` incrustado.** La sobrecarga sin `Uri` genera `CreatedResult("https://www.netalpunto.net", x)`. **Usa siempre la sobrecarga que recibe `Uri`.**

5. **Nomenclatura asimétrica en las sobrecargas asíncronas de POST**: `ToPostActionResultAsync` (sobre `Task<MlResult<T>>`) frente a `ToCreatedProblemDetailsResultAsync` (sobre `MlResult<T>`). El `Pd` desaparece justo en las sobrecargas de `Task`.

6. **El puente `MlErrorDetails` → `ProblemDetailsInfo` funciona por reflexión.** `ToProblemDetails` lee propiedades por nombre de un **tipo anónimo** creado en otro proyecto (`MlProblemDetails`, en `WebServices`). No hay interfaz ni contrato compilado: un renombrado degrada silenciosamente el resultado a `500` / `"Error"`.

7. **El *fallback* a 500 oculta errores de negocio.** Si el `MlErrorDetails` no trae el detalle `"ProblemsDetails"`, `MlResultActionResultExtensions` devuelve `InternalServerError()` **sin incluir el mensaje original**. Un 404 legítimo se convierte en un 500 mudo. Ocurre, por ejemplo, con los errores que genera `MoralesLarios.OOFP.Application.Helpers.Extensions.BuildEntityNotFoundError`, que usa la clave `"NotFound"` en vez de `"ProblemsDetails"`.

8. **`ProblemDetailsInfo` duplica el código de estado** en `Status` y `StatusCode`, sin ninguna garantía de coherencia entre ambos.

9. **`MlResults` duplica el catálogo de `MlProblemDetails`.** Las mismas 15 variantes existen dos veces en la solución, con la misma firma y los mismos textos, en proyectos distintos. Cualquier cambio hay que hacerlo en los dos sitios.

10. **`MlResultActionResultExtensionsLegacy` está `[Obsolete]` al completo pero se sigue distribuyendo**, con los defectos descritos en la sección anterior (`"NotUri"`, `Created` en fallo, detección de 404 por cadenas, identificadores en español).

11. **Los nombres de las cabeceras de paginación son literales privados.** `"X-Page-Number"` y `"X-Page-Size"` no se exponen como constantes.

12. **Todos los mensajes de error del proyecto están en inglés y son fijos.** A diferencia de otras capas del ecosistema, aquí no hay parámetros `initialMessage` / `failMessageBuilder` para personalizarlos.

---

## ⚠️ Lo que NO incluye

Para fijar expectativas, este proyecto **no** aporta:

- ❌ **Controladores base.** Para eso está [`MoralesLarios.OOFP.AspNetCore.Controllers`](../MoralesLarios.OOFP.AspNetCore.Controllers/README.md).
- ❌ **Filtros, *middleware* ni manejador global de excepciones.** No hay `IExceptionFilter` ni `UseMlResultExceptionHandler()`.
- ❌ **Validación de modelos.** Usa [`MoralesLarios.OOFP.Validation.DataAnnotations`](../MoralesLarios.OOFP.Validation.DataAnnotations/README.md) o [`…FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidation/README.md) y convierte el resultado con `MlResults.BadRequest(IEnumerable<ValidationResult>)`.
- ❌ **Registro/logging.** Ver [`MoralesLarios.OOFP.Logging`](../MoralesLarios.OOFP.Logging/README.md).
- ❌ **Soporte de *Minimal APIs*.** Todo devuelve `IActionResult` (MVC), no `IResult`.
- ❌ **Negociación de contenido** más allá de lo que haga MVC por defecto (siempre `ObjectResult`).
- ❌ **Cabeceras de respuesta de paginación.** Sabe *leer* `X-Page-Number` / `X-Page-Size`, pero no las escribe en la respuesta.
- ❌ **Escritura de cabeceras en general.** `HttpRequestPaginationExtensions` solo lee.

---

## Ejemplos prácticos

### Ejemplo 1 — CRUD completo sin un solo `if`

```csharp
[ApiController]
[Route("api/facturas")]
public class FacturasController(ICrudServiceFp<Factura, FacturaDto> service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public Task<IActionResult> Get(int id)
        => service.FindByIdProblemsDetailsAsync(id)
                  .ToOkProblemDetailsResultAsync();

    [HttpPost]
    public Task<IActionResult> Post(FacturaDto dto)
        => service.CreateAsync(dto)
                  .ToPostActionResultAsync(new Uri("/api/facturas", UriKind.Relative));

    [HttpPut("{id:int}")]
    public Task<IActionResult> Put(int id, FacturaDto dto)
        => service.UpdateAsync(dto, id)
                  .ToNoContentProblemDetailsResultAsync();

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete(int id)
        => service.DeleteProblemDetailsAsync(id)
                  .ToDeletedProblemDetailsResultAsync();
}
```

### Ejemplo 2 — cadena de negocio con varios códigos de estado distintos

El controlador no sabe qué código va a salir: lo deciden las reglas.

```csharp
// --- Capa de negocio ---
public Task<MlResult<PedidoDto>> ConfirmarAsync(int id) =>
    _repo.TryFindAsync(id)
         .BindAsync(p => p is null
             ? MlProblemDetails.NotFoundError($"El pedido {id} no existe")           // → 404
             : MlResult<Pedido>.Valid(p))
         .BindAsync(p => p.Estado == EstadoPedido.Confirmado
             ? MlProblemDetails.ConflictError("El pedido ya estaba confirmado")      // → 409
             : MlResult<Pedido>.Valid(p))
         .BindAsync(p => p.Lineas.Any()
             ? MlResult<Pedido>.Valid(p)
             : MlProblemDetails.UnprocessableContentError("El pedido no tiene líneas")) // → 422
         .BindAsync(p => _repo.TryUpdateAsync(p.With(Estado: EstadoPedido.Confirmado)))
         .MapAsync (p => p.Adapt<PedidoDto>());

// --- Controlador ---
[HttpPost("{id:int}/confirmar")]
public Task<IActionResult> Confirmar(int id)
    => _service.ConfirmarAsync(id).ToOkProblemDetailsResultAsync();   // 200 / 404 / 409 / 422
```

### Ejemplo 3 — validación con DataAnnotations → 400 con `errors` por campo

```csharp
[HttpPost("validado")]
public async Task<IActionResult> PostValidado(ClienteDto dto)
{
    var validaciones = new List<ValidationResult>();
    var contexto     = new ValidationContext(dto, serviceProvider: HttpContext.RequestServices, items: null);

    if (! Validator.TryValidateObject(dto, contexto, validaciones, validateAllProperties: true))
        return MlResults.BadRequest(validaciones,
                                          title : "Datos de entrada no válidos",
                                          detail: "Revise los campos marcados",
                                          type  : "https://miapi.example.com/errors/validacion");

    return await _service.CreateAsync(dto)
                         .ToPostActionResultAsync(new Uri("/api/clientes", UriKind.Relative));
}
```

### Ejemplo 4 — paginación tomada de las cabeceras HTTP

```csharp
[HttpGet]
public Task<IActionResult> All()
    => Request.GetHeaderPaginationInfo()
              .BindAsync(pag => _repo.TryAllPaginationAsync(pag.PageNumber, pag.PageSize))
              .MapAsync (res => new
                                {
                                    res.PageNumber,
                                    res.PageSize,
                                    res.TotalCount,
                                    Items = res.Items.Adapt<IEnumerable<ClienteDto>>()
                                })
              .ToOkProblemDetailsResultAsync();
```

Petición:

```http
GET /api/clientes HTTP/1.1
X-Page-Number: 2
X-Page-Size: 25
```

Si falta `X-Page-Size`, la respuesta es un `500` con el mensaje del fallo (porque el error de cabecera **no** trae detalle `"ProblemsDetails"`; ver Ejemplo 8 para la solución).

### Ejemplo 5 — cabecera de negocio (multi-tenant)

```csharp
[HttpGet("resumen")]
public Task<IActionResult> Resumen()
    => Request.GetHeaderInfo("X-Tenant-Id")
              .Bind     (t => _tenants.Validar(t))
              .BindAsync(t => _service.ResumenAsync(t))
              .ToOkProblemDetailsResultAsync();
```

### Ejemplo 6 — traducir errores a mano cuando necesitas control total

```csharp
[HttpGet("{id:int}/pdf")]
public async Task<IActionResult> Pdf(int id)
{
    var resultado = await _service.GenerarPdfAsync(id);

    return resultado.Match(
        valid: bytes  => File(bytes, "application/pdf", $"factura-{id}.pdf"),  // no es JSON: no sirve ToGetPd…
        fail : errors => errors.GetProblemDetails()
                               .Match(valid: pd => pd.ToMlActionResult(),
                                      fail : _  => MlResults.InternalServerError(
                                                       "No se pudo generar el PDF",
                                                       errors.GetErrorsSummary())));
}
```

💡 Cuando el camino válido **no** es un JSON (`FileResult`, `RedirectResult`, `ContentResult`…) `ToOkProblemDetailsResult` no encaja: usa `Match` y reutiliza solo la rama de error.

### Ejemplo 7 — envoltorio propio para fijar `type` y no repetir el dominio del autor

```csharp
public static class ApiProblems
{
    private const string Base = "https://miapi.example.com/errors/";

    public static IActionResult NotFound(string detail)
        => MlResults.NotFound("Recurso no encontrado", detail, $"{Base}not-found");

    public static IActionResult Conflict(string detail)
        => MlResults.Conflict("Conflicto de estado", detail, $"{Base}conflict");

    public static IActionResult Internal(MlErrorDetails errors)
        => MlResults.InternalServerError("Error interno", errors.GetErrorsSummary(),
                                               $"{Base}internal");
}
```

Así centralizas el `type` en un único punto y evitas la particularidad #3.

### Ejemplo 8 — errores frecuentes (❌ / ✅)

**Devolver `MlResult<T>` directamente desde la acción:**

```csharp
// ❌ ASP.NET Core serializará un objeto sin Value ni ErrorsDetails accesibles → 200 con {} 
[HttpGet("{id:int}")]
public Task<MlResult<ClienteDto>> Get(int id) => _service.FindByIdAsync(id);

// ✅
[HttpGet("{id:int}")]
public Task<IActionResult> Get(int id)
    => _service.FindByIdProblemsDetailsAsync(id).ToOkProblemDetailsResultAsync();
```

**Usar la sobrecarga de POST sin `Uri`:**

```csharp
// ❌ Location = "https://www.netalpunto.net" (literal incrustado en la librería)
return await _service.CreateAsync(dto).ToPostActionResultAsync();

// ✅
return await _service.CreateAsync(dto)
                     .ToPostActionResultAsync(new Uri("/api/clientes", UriKind.Relative));
```

**Usar los métodos sin sufijo `ProblemsDetails` de la capa de servicio:**
```csharp
// ❌ FindByIdAsync genera un error con la clave "NotFound" → GetProblemDetails() falla → 500
return await _service.FindByIdAsync(id).ToOkProblemDetailsResultAsync();

// ✅ FindByIdProblemsDetailsAsync adjunta el detalle "ProblemsDetails" → 404 real
return await _service.FindByIdProblemsDetailsAsync(id).ToOkProblemDetailsResultAsync();
```

**Enriquecer un error de cabecera para que no acabe en 500:**
```csharp
// ❌ el fail de GetHeaderPaginationInfo no lleva "ProblemsDetails" → 500
return await Request.GetHeaderPaginationInfo()
                    .BindAsync(p => _repo.TryAllPaginationAsync(p.PageNumber, p.PageSize))
                    .ToOkProblemDetailsResultAsync();

// ✅ se reclasifica como 400 antes de salir del raíl
return await Request.GetHeaderPaginationInfo()
                    .MapIfFail(errors => MlProblemDetails.BadRequestError(
                                             "Cabeceras de paginación no válidas",
                                             errors.GetErrorsSummary()))
                    .BindAsync(p => _repo.TryAllPaginationAsync(p.PageNumber, p.PageSize))
                    .ToOkProblemDetailsResultAsync();
```

**Confiar en el `type` por defecto:**
```csharp
// ❌ tu API publica el dominio del autor de la librería en cada error
return MlResults.NotFound("No encontrado", $"El cliente {id} no existe");

// ✅
return MlResults.NotFound("No encontrado", $"El cliente {id} no existe",
                                "https://miapi.example.com/errors/not-found");
```

**Seguir usando la capa legacy:**
```csharp
// ❌ clase completa [Obsolete]; además puede devolver 201 en caso de fallo
return await _service.CreateAsync(dto).ToCreatedResultAsync();

// ✅
return await _service.CreateAsync(dto)
                     .ToPostActionResultAsync(new Uri("/api/clientes", UriKind.Relative));
```

**Acceder a `Value` en lugar de dejar que la extensión lo haga:**
```csharp
// ❌ Value y ErrorsDetails son internal protected: no compila desde tu proyecto
if (resultado.IsSuccess) return Ok(resultado.Value);

// ✅
return resultado.ToOkProblemDetailsResult();
```

---

## Tabla de decisión rápida

| Necesito… | Usa |
|---|---|
| Devolver `200 OK` con el valor | `ToOkProblemDetailsResult()` / `ToOkProblemDetailsResultAsync()` |
| Devolver `201 Created` con `Location` correcto | `ToCreatedProblemDetailsResult(uri)` / `ToPostActionResultAsync(uri)` |
| Devolver `204 No Content` tras un PUT | `ToNoContentProblemDetailsResult()` / `ToNoContentProblemDetailsResultAsync()` |
| Devolver `204 No Content` tras un PATCH | `ToPatchedProblemDetailsResult()` / `ToPatchedProblemDetailsResultAsync()` |
| Devolver `204 No Content` tras un DELETE | `ToDeletedProblemDetailsResult()` / `ToDeletedProblemDetailsResultAsync()` |
| Devolver un error concreto **a mano** | `MlResults.NotFound/Conflict/BadRequest/…` |
| Devolver un error con un código no cubierto | `MlResults.CreateProblemsDetails(statusCode, …)` |
| Convertir `ValidationResult` en un `400` con `errors` por campo | `MlResults.BadRequest(IEnumerable<ValidationResult>, …)` |
| Devolver algo que **no** es JSON (`File`, `Redirect`…) | `Match(valid: …, fail: errors => errors.GetProblemDetails()…)` |
| Extraer el `ProblemDetails` real de un `MlErrorDetails` | `errors.GetProblemDetails()` |
| Convertir un `ProblemDetailsInfo` en `IActionResult` | `pdInfo.ToMlActionResult()` |
| Leer una cabecera obligatoria como texto | `Request.GetHeaderInfo("X-…")` |
| Leer una cabecera obligatoria como entero | `Request.GetHeaderInfoAsIntNonNegative("X-…")` |
| Leer solo el número de página | `Request.GetHeaderPageNumber()` |
| Leer solo el tamaño de página | `Request.GetHeaderPageSize()` |
| Leer ambas como tupla | `Request.GetHeaderPageInfo()` |
| Leer ambas como `PageRequest` normalizado | `Request.GetHeaderPaginationInfo()` |
| Que un error de negocio salga con su código real | Genera el error con `MlProblemDetails.*` en la capa de servicio |
| Migrar código antiguo | Sustituye `ToRepo*ActionResult` por su equivalente `*Pd*` |

---

## Mejores prácticas

1. **Usa siempre `MlResultActionResultExtensions`, nunca `MlResultActionResultExtensionsLegacy`.** La segunda está `[Obsolete]` al completo y adivina los códigos de estado comparando cadenas.

2. **Que el error nazca ya clasificado.** El código de estado es una decisión de negocio: constrúyelo con `MlProblemDetails.*` (proyecto `WebServices`) en el momento en que detectas el problema, no en el controlador.

3. **Prefiere los métodos `…ProblemsDetails…` de la capa de servicio** (`FindByIdProblemsDetailsAsync`, `UpdateAsync`, `DeleteProblemDetailsAsync`). Los que no llevan ese sufijo generan errores sin el detalle `"ProblemsDetails"` y acaban en un `500`.

4. **Pasa siempre el `Uri` en POST.** La sobrecarga sin `Uri` incrusta un `Location` literal ajeno a tu API.

5. **Sobrescribe siempre el `type`** o encapsula `MlResults` en tu propia clase (Ejemplo 7) para no publicar el dominio del autor de la librería en cada error.

6. **Reclasifica los errores de entrada.** Los fallos de `HttpRequestPaginationExtensions` no traen `"ProblemsDetails"`: conviértelos a `400` con `MapIfFail` + `MlProblemDetails.BadRequestError` (Ejemplo 8).

7. **Un endpoint = una expresión.** Si aparece un `if` en tu controlador, la decisión está en el sitio equivocado: muévela al raíl.

8. **No accedas a `Value` ni a `ErrorsDetails`.** Son `internal protected`. Usa `Match`, `GetValueOrThrow()` o directamente las extensiones de este proyecto.

9. **Para respuestas no-JSON usa `Match`** y reutiliza únicamente la rama de error (`GetProblemDetails()` + `ToMlActionResult()`).

10. **Define constantes propias para las cabeceras** (`X-Page-Number`, `X-Page-Size`) y documéntalas en tu OpenAPI: la librería no las expone.

11. **Documenta los códigos posibles con `[ProducesResponseType]`.** Como el controlador devuelve `IActionResult`, Swagger no puede inferir nada: decláralo explícitamente.

12. **Registra antes de traducir.** Encadena `LogCompletedResultAsync` (proyecto `Extensions.Loggers`) *antes* del `ToXxxPdActionResultAsync`, para que el log conserve el `MlErrorDetails` completo y no solo el `ProblemDetails` recortado.

13. **Cuida el `PageSize`.** `PageRequest` lo recorta a `[1, 1000]` silenciosamente: si tu cliente pide 5000 y recibe 1000, no habrá ningún error que lo avise.

14. **Añade `Instance` cuando puedas.** Ninguna fábrica lo rellena; si lo necesitas (muy útil para correlacionar incidencias), añádelo con un *middleware* o un filtro propio.

---

## Resumen

1. **Un único cometido**: traducir `MlResult<T>` ⇄ HTTP. Nada de lógica de negocio, nada de acceso a datos.
2. **`MlResultActionResultExtensions` es la puerta de entrada**: un método por verbo, con variantes `Async`, y el controlador queda en una línea por acción.
3. **`MlResults`** ofrece 15 fábricas de error que devuelven `ObjectResult` con cuerpo `ExtendedProblemDetails` (RFC 7807 + diccionario `Errors`).
4. **`MlErrorDetailsExtensions`** es el puente: lee el detalle `"ProblemsDetails"` del `MlErrorDetails` y lo convierte —**por reflexión**— en `ProblemDetailsInfo`.
5. **El código de estado lo decide la capa que genera el error**, no el controlador. Si el error no viene clasificado, la respuesta será un `500`.
6. **`HttpRequestPaginationExtensions`** cierra el círculo en la dirección de entrada: cabeceras HTTP → *value objects* validados dentro del raíl.
7. **`MlResultActionResultExtensionsLegacy` está obsoleta al completo** y contiene defectos reales; migra con la tabla de la sección correspondiente.
8. **No hay `RegisterServices`**: basta con referenciar el proyecto, todo son extensiones estáticas.
9. **Puntos a vigilar**: paquete MVC 2.1.0 sobre `net8.0`, `type` y `Location` con valores literales del autor, nomenclatura asimétrica en POST y el contrato por reflexión con `MlProblemDetails`.

---

## Ver también

### Navegación general

- [📄 Índice de la solución](../README.md)
- [📄 Biblioteca núcleo `MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md)
- [📄 Introducción al enfoque FOOP](../docs/1_Intro.md)

### Proyectos relacionados

- [📄 `MoralesLarios.OOFP.Application`](../MoralesLarios.OOFP.Application/README.md) — genera los errores con `MlProblemDetails`; **es el compañero natural de este proyecto**
- [📄 `MoralesLarios.OOFP.AspNetCore.Controllers`](../MoralesLarios.OOFP.AspNetCore.Controllers/README.md) — controladores base genéricos que ya usan estas extensiones
- [📄 `MoralesLarios.OOFP.EntityFrameworkCore`](../MoralesLarios.OOFP.EntityFrameworkCore/README.md) — repositorios funcionales que alimentan los servicios
- [📄 `MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — `MemberName`, `NotEmptyString`, `IntNonNegative` usados por las cabeceras
- [📄 `MoralesLarios.OOFP.Shared`](../MoralesLarios.OOFP.Shared/README.md) — `PageRequest` y `PagedResult`
- [📄 `MoralesLarios.OOFP.Validation.DataAnnotations`](../MoralesLarios.OOFP.Validation.DataAnnotations/README.md) — produce los `ValidationResult` que consume `MlResults.BadRequest`
- [📄 `MoralesLarios.OOFP.Logging`](../MoralesLarios.OOFP.Logging/README.md) — registro dentro del raíl antes de traducir a HTTP

### Documentación del núcleo útil aquí

- [📄 `MlResult<T>`](../docs/core-types/MlResult.md) — el tipo que se traduce a `IActionResult`
- [📄 Errores y `MlErrorDetails`](../docs/core-types/MlResultErrors.md) — estructura de `Errors` y `Details`
- [📄 `Match`](../docs/Match/1_Match.md) — la base de toda la traducción de este proyecto
- [📄 `Bind`](../docs/Bind/3_Bind.md) — encadenar reglas antes de responder
- [📄 `Map`](../docs/Map/1_Map.md) — transformar la entidad en DTO antes de serializar
- [📄 `EnsureFp`](../docs/EnsureFp/EnsureFp.md) — validaciones que cortan el raíl con un error clasificado
- [📄 Métodos asíncronos en el raíl](../docs/1_Intro.md#sufijos-de-asincronía) — las sobrecargas `…Async` de este proyecto
