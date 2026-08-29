# MoralesLarios.OOFP.WebServices — Servicios CRUD genéricos con DTOs y ProblemDetails

Capa de aplicación que se sitúa entre los repositorios de [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) y los controladores web. Aporta tres cosas en un solo tipo genérico: **CRUD completo**, **mapeo automático entidad ⇄ DTO** con [Mapster](https://github.com/MapsterMapper/Mapster) y **trazado del inicio y del final** de cada operación, todo sobre el raíl de [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md).

Incluye además `MlProblemsDetails`, una fábrica de errores con la forma de **RFC 7807 (`ProblemDetails`)** para que la capa HTTP los convierta en respuestas sin trabajo adicional.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Los dos modos: simple y duplex](#los-dos-modos-simple-y-duplex)
5. [Anatomía de una operación](#anatomía-de-una-operación)
6. [`IGenServiceFp<TEntity, TDto>` — modo simple](#igenservicefptentity-tdto--modo-simple)
7. [`IGenServiceFp<TEntity, TRequest, TResponse>` — modo duplex](#igenservicefptentity-trequest-tresponse--modo-duplex)
8. [Los parámetros de mensajes](#los-parámetros-de-mensajes)
9. [Las parejas `Async` / `ProblemDetailsAsync`](#las-parejas-async--problemdetailsasync)
10. [`MlProblemsDetails` — errores RFC 7807](#mlproblemsdetails--errores-rfc-7807)
11. [Registro en el contenedor](#registro-en-el-contenedor)
12. [Mapeo con Mapster](#mapeo-con-mapster)
13. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
14. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
15. [Ejemplos prácticos](#ejemplos-prácticos)
16. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
17. [Mejores prácticas](#mejores-prácticas)
18. [Resumen](#resumen)
19. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

En una API típica, el servicio de aplicación de cada entidad repite el mismo guion: traza la entrada, valida, mapea el DTO a la entidad, llama al repositorio, mapea el resultado de vuelta y traza el desenlace. Multiplicado por cinco operaciones y por cada entidad.

❌ **Escrito a mano para cada entidad:**

```csharp
public async Task<VinoDto> CreateAsync(VinoDto dto, CancellationToken ct)
{
    _logger.LogInformation("Creando vino…");

    if (dto is null) throw new ArgumentNullException(nameof(dto));

    var entidad = _mapper.Map<Vino>(dto);

    try
    {
        var creada = await _repo.AddAsync(entidad, ct);
        _logger.LogInformation("Vino {Id} creado", creada.Id);
        return _mapper.Map<VinoDto>(creada);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear el vino");
        throw;
    }
}
// …y lo mismo para All, FindById, Update y Delete. Por cada entidad.
```

✅ **Con `GenServiceFp<TEntity, TDto>`:**

```csharp
// No escribes nada: el servicio genérico ya lo hace
services.AddScopedtGenServicesFpWithoutReposGeneral();

// Y lo usas directamente
public class VinosController(IGenServiceFp<Vino, VinoDto> _service)
{
    public Task<MlResult<VinoDto>> Crear(VinoDto dto, CancellationToken ct)
        => _service.CreateAsync(dto, ct);
}
```

| Problema | Cómo lo resuelve |
|---|---|
| CRUD repetido por entidad | Un único servicio genérico registrado con tipos abiertos |
| Mapeo manual entidad ⇄ DTO | `Adapt<T>()` de Mapster por convención |
| Trazas dispersas | Log automático al inicio y al final de cada operación |
| Excepciones para el "no encontrado" | `MlErrorsDetails` con estructura `ProblemDetails` |
| DTO de entrada ≠ DTO de salida | El modo **duplex** de tres parámetros genéricos |

---

## Instalación y dependencias

| Dependencia | Versión | Para qué |
|---|---|---|
| `Mapster` | 7.4.0 | 🔑 `Adapt<T>()`, el mapeo entidad ⇄ DTO |
| `Microsoft.Extensions.Configuration.Abstractions` | 9.0.6 | ⚠️ Declarada pero **no se usa** |
| [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) | — | 🔑 `IEFRepoFp<TEntity>`, `GetPkValues` |
| [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) | — | 🔑 `LogMlResultInformationAsync`, `LogMlResultFinalAsync` |
| [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) | — | Tipos internos compartidos |

Destino: **`net8.0`**. Versión del paquete: **1.0.10**.

> ⚠️ **Arrastra `EFCore` como dependencia obligatoria.** El servicio depende de `IEFRepoFp<TEntity>` en el constructor, así que **no puedes usar esta capa con otro tipo de persistencia** (Dapper, MongoDB, un servicio remoto…) sin implementar tú `IEFRepoFp<T>`.

```csharp
using MoralesLarios.OOFP.WebServices;             // MlProblemsDetails, RegisterServices
using MoralesLarios.OOFP.WebServices.Services;    // IGenServiceFp<,>, GenServiceFp<,>
```

---

## Estructura del proyecto

```
MoralesLarios.OOFP.WebServices/
├── Services/
│   ├── IGenServiceFp.cs         → los dos contratos: <TEntity,TDto> y <TEntity,TRequest,TResponse>
│   ├── GenServiceFp.cs          → las dos implementaciones (todos los métodos virtual)
│   └── GenService.cs            → ⚠️ FICHERO VACÍO
├── Helpers/
│   ├── Extensions.cs            → BuildNotFoundPkError
│   └── Constants.cs             → Name ProblemsDetails => "ProblemsDetails"
├── MlProblemsDetails.cs         → 15 fábricas de errores RFC 7807
├── RegisterServices.cs          → 6 extensiones de registro
└── GlobalUsings.cs
```

> ⚠️ **`Services/GenService.cs` está completamente vacío** (0 bytes). Aparece en el proyecto pero no aporta nada: **no existe ninguna variante OOP** del servicio, a diferencia de lo que ocurre en `EFCore`.

---

## Los dos modos: simple y duplex

| Modo | Interfaz | Entrada | Salida | Cuándo |
|---|---|---|---|---|
| **Simple** | `IGenServiceFp<TEntity, TDto>` | `TDto` | `TDto` | El mismo DTO sirve para leer y escribir |
| **Duplex** | `IGenServiceFp<TEntity, TRequest, TResponse>` | `TRequest` | `TResponse` | El DTO de entrada difiere del de salida |

```csharp
// Simple: VinoDto entra y sale
IGenServiceFp<Vino, VinoDto> servicioSimple;

// Duplex: entra VinoRequest (sin Id), sale VinoResponse (con Id y campos calculados)
IGenServiceFp<Vino, VinoRequest, VinoResponse> servicioDuplex;
```

> 💡 **Usa el modo duplex en APIs públicas**: el DTO de entrada no debería exponer `Id`, campos de auditoría ni claves ajenas internas, mientras que el de salida sí necesita el `Id` generado.

Ambos modos tienen **exactamente los mismos 11 métodos**; solo cambian los tipos.

---

## Anatomía de una operación

Todas las operaciones siguen la misma estructura de cinco pasos. Este es `CreateAsync` completo:

```csharp
public virtual Task<MlResult<TDto>> CreateAsync(TDto dto, CancellationToken ct = default!, …)
{
    var result = _logger.LogMlResultInformationAsync(                    // 1️⃣ traza de entrada
                             initialMessage ?? $"Creating a new record …")
                        .BindAsync  ( _     => EnsureFp.NotNull(dto, …)) // 2️⃣ validación
                        .TryMapAsync( _     => dto.Adapt<TEntity>())     // 3️⃣ DTO → entidad
                        .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))  // 4️⃣ persistencia
                        .MapAsync   (bdData => bdData.Adapt<TDto>())     // 5️⃣ entidad → DTO
                        .LogMlResultFinalAsync(logger: _logger,          // 6️⃣ traza de salida
                                               validBuildMessage: …,
                                               failBuildMessage : …);
    return result;
}
```

**El detalle más curioso**: la cadena **arranca desde el propio logger**.

```csharp
_logger.LogMlResultInformationAsync(mensaje)   // devuelve Task<MlResult<ILogger>>
```

Es decir, el primer eslabón del raíl es un `MlResult<ILogger>` que envuelve al propio logger. Los `BindAsync` posteriores descartan ese valor con `_ =>` y el raíl continúa con los tipos reales.

> 💡 **Consecuencia práctica**: la traza de entrada se emite **siempre**, incluso si la validación posterior falla. Es intencionado: sabes que la operación se intentó.

> 💡 **Todos los métodos son `virtual`**: puedes heredar de `GenServiceFp<Vino, VinoDto>` y sobrescribir solo `CreateAsync` para añadir una regla de negocio, dejando el resto tal cual.

---

## `IGenServiceFp<TEntity, TDto>` — modo simple

```csharp
public interface IGenServiceFp<TEntity, TDto>
    where TEntity : class
    where TDto    : class
{
    // Consulta de todos
    Task<MlResult<IEnumerable<TDto>>> AllAsync(CancellationToken ct = default!,
                                               string initialMessage = null!,
                                               Func<IEnumerable<TDto>, string> validMessageBuilder = null!,
                                               Func<MlErrorsDetails, string>   failMessageBuilder  = null!);

    // Búsqueda por clave
    Task<MlResult<TDto?>> FindByIdAsync                (…, params object[] pk);
    Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync (MlErrorsDetails notFoundErrorDetails, …, params object[] pk);

    // Creación
    Task<MlResult<TDto>>  CreateAsync(TDto dto, …);

    // Modificación: con y sin comprobación de existencia
    Task<MlResult<TDto>>  UpdateAsync              (TDto dto, …, params object[] pk);
    Task<MlResult<TDto>>  UpdateAsync              (TDto dto, …);                       // sin pk
    Task<MlResult<TDto>>  UpdateProblemDetailsAsync(TDto dto, MlErrorsDetails notFoundErrorDetails, …, params object[] pk);

    // Borrado: por clave o por entidad
    Task<MlResult<TDto>>  DeleteAsync              (…, params object[] pk);
    Task<MlResult<TDto>>  DeleteAsync              (TDto dto, …);
    Task<MlResult<TDto>>  DeleteProblemDetailsAsync(MlErrorsDetails notFoundErrorDetails, …, params object[] pk);
}
```

| Método | Qué hace | Comprueba existencia |
|---|---|---|
| `AllAsync` | `TryAllAsync` → `Adapt<IEnumerable<TDto>>` | — |
| `FindByIdAsync` | `TryFindAsync` por PK | ✅ (mensaje por defecto) |
| `FindByIdProblemsDetailsAsync` | Igual, con tu `MlErrorsDetails` | ✅ (tu mensaje) |
| `CreateAsync` | Valida, mapea e inserta | — |
| `UpdateAsync(dto, …, pk)` | `TryFind` + `TryUpdate` | ✅ |
| `UpdateAsync(dto, …)` | `TryUpdate` directo | ❌ |
| `DeleteAsync(…, pk)` | `TryFind` + `TryRemove` | ✅ |
| `DeleteAsync(dto, …)` | Mapea y borra directamente | ❌ |

> 💡 **`FindByIdAsync` devuelve `MlResult<TDto?>`** (con `?`). Sin embargo, si la fila no existe el repositorio ya devuelve `Fail`, así que en la práctica **un `Valid` nunca trae `null`**. La anulabilidad es defensiva.

---

## `IGenServiceFp<TEntity, TRequest, TResponse>` — modo duplex

Mismos 11 métodos, con `TRequest` en la entrada y `TResponse` en la salida:

```csharp
Task<MlResult<IEnumerable<TResponse>>> AllAsync(…);
Task<MlResult<TResponse?>>             FindByIdAsync(…, params object[] pk);
Task<MlResult<TResponse>>              CreateAsync(TRequest dtoRequest, …);
Task<MlResult<TResponse>>              UpdateAsync(TRequest dtoRequest, …, params object[] pk);
Task<MlResult<TResponse>>              DeleteAsync(TRequest dtoRequest, …);
// …
```

El flujo mapea **dos veces con tipos distintos**:

```csharp
.TryMapAsync( _     => dtoRequest.Adapt<TEntity>())    // TRequest → TEntity
.BindAsync  (bdData => _repo.TryAddAsync(bdData, ct))
.MapAsync   (bdData => bdData.Adapt<TResponse>())      // TEntity  → TResponse
```

> 💡 **Los mensajes del modo duplex son más descriptivos**: usan la propiedad privada `DtosDescType`, que genera `"dto Request VinoRequest to dto Response VinoResponse"`.

---

## Los parámetros de mensajes

Cada operación acepta **tres parámetros opcionales** para controlar el trazado:

| Parámetro | Tipo | Cuándo se usa |
|---|---|---|
| `initialMessage` | `string` | Traza de entrada. Si es `null`, mensaje por defecto en inglés |
| `validMessageBuilder` | `Func<TDto, string>` | Traza de éxito, recibe el resultado |
| `failMessageBuilder` | `Func<MlErrorsDetails, string>` | Traza de error, recibe los errores |

```csharp
await _service.CreateAsync(
          dto,
          ct,
          initialMessage     : $"Alta de vino solicitada por {usuario}",
          validMessageBuilder: v => $"Vino creado con Id {v.Id}",
          failMessageBuilder : e => $"No se pudo crear el vino: {e.ToErrorsDescription()}");
```

Si no los pasas, los mensajes por defecto son genéricos y **en inglés**:

```
"Creating a new record in the table corresponding to dto VinoDto"
"The record was created successfully in the table corresponding to dto VinoDto"
"An error occurred while creating a new record in the table corresponding to dto VinoDto. Error: …"
```

> ⚠️ **`DeleteAsync` no acepta `validMessageBuilder`.** Solo `initialMessage` y `failMessageBuilder`: el mensaje de éxito del borrado **está fijado en el código** y no se puede personalizar. Es una asimetría respecto al resto de operaciones.

> 💡 **Los mensajes por defecto exponen el nombre del tipo DTO.** Van al log, no a la respuesta HTTP, así que no hay fuga de información al cliente; pero si el log es accesible, tenlo en cuenta.

---

## Las parejas `Async` / `ProblemDetailsAsync`

Tres operaciones vienen duplicadas, y la diferencia es **quién construye el error de "no encontrado"**:

```csharp
// Versión corta: el mensaje lo genera la librería
public virtual Task<MlResult<TDto?>> FindByIdAsync(…, params object[] pk)
    => FindByIdProblemsDetailsAsync(
           notFoundErrorDetails: typeof(TDto).Name.BuildNotFoundPkError(pk),   // 🔑 automático
           …);
```

Y el helper que lo genera:

```csharp
public static MlErrorsDetails BuildNotFoundPkError(this string tableName, params object[] pk)
    => MlErrorsDetails.FromErrorMessageDetails(
           $"No data found for the {tableName} table by Id ({pk.GetPkValues()})",
           new Dictionary<string, object> { ["NotFound"] = $"No data found for the {tableName} table by Id ({pk.GetPkValues()})" });
```

| Pareja | Usa |
|---|---|
| `FindByIdAsync` / `FindByIdProblemsDetailsAsync` | Búsqueda |
| `UpdateAsync(…, pk)` / `UpdateProblemDetailsAsync` | Modificación |
| `DeleteAsync(…, pk)` / `DeleteProblemDetailsAsync` | Borrado |

> ✅ **Usa las variantes `ProblemDetailsAsync` con `MlProblemsDetails.NotFoundError(…)`** cuando el error vaya a convertirse en respuesta HTTP: así obtienes el `Status = 404` y la estructura RFC 7807 completa, mientras que la versión corta solo produce una clave `"NotFound"` sin código de estado.

> ⚠️ **El nombre es engañoso**: `FindByIdProblemsDetailsAsync` no *genera* un `ProblemDetails`, solo **acepta** el `MlErrorsDetails` que le pases. Y la versión corta **tampoco** produce un `ProblemDetails` con `Status`: el diccionario que crea `BuildNotFoundPkError` usa la clave `"NotFound"`, no `"ProblemsDetails"`, así que **la capa HTTP no la reconocerá como 404**.

---

## `MlProblemsDetails` — errores RFC 7807

Clase estática con **15 fábricas** que devuelven un `MlErrorsDetails` cuyo diccionario de detalles contiene la clave `"ProblemsDetails"` con la forma estándar.

```csharp
public static MlErrorsDetails NotFoundError(string title = null!, string detail = null!,
                                            string type = null!, Dictionary<string, object> errors = null!)
    => (title ?? "Not found",
        new Dictionary<string, object>
        {
            { "ProblemsDetails", new
                {
                      Status     = 404,
                      Title      = title  ?? "Not found",
                      Detail     = detail ?? "The requested resource was not found.",
                      Type       = type   ?? "https://www.puntonetalpunto.net/",
                      Errors     = errors ?? new Dictionary<string, object>(),
                      StatusCode = 404
                }
            }
        });
```

| Fábrica | Status | Título por defecto |
|---|---|---|
| `CreateProblemDetails(statusCode, …)` | *el que pases* | `"Error Details"` |
| `BadRequestError` | 400 | `"Bad request"` |
| `UnauthorizedError` | 401 | `"Unauthorized"` |
| `ForbiddenError` | 403 | `"Forbidden"` |
| `NotFoundError` | 404 | `"Not found"` |
| `MethodNotAllowedError` | 405 | `"Method not allowed"` |
| `ConflictError` | 409 | `"Conflict"` |
| `UnprocessableContentError` | 422 | `"Unprocessable content"` |
| `TooManyRequestsError` | 429 | `"Too many requests"` |
| `InternalServerError` | 500 | `"Internal server error"` |
| `NotImplementedError` | 501 | `"Not implemented"` |
| `BadGatewayError` | 502 | `"Bad gateway"` |
| `ServiceUnavailableError` | 503 | `"Service unavailable"` |
| `GatewayTimeoutError` | 504 | `"Gateway timeout"` |

### La sobrecarga para validación de DataAnnotations

`BadRequestError` tiene una segunda forma que agrupa `ValidationResult` por miembro:

```csharp
public static MlErrorsDetails BadRequestError(string title, string detail, string type,
                                              IEnumerable<ValidationResult> validationResults)
{
    var errors = validationResults?
                    .GroupBy    (x => string.Join(", ", x.MemberNames))
                    .ToDictionary(g => string.IsNullOrEmpty(g.Key) ? "validation" : g.Key,
                                  g => (object)g.Select(v => v.ErrorMessage).ToList())
                 ?? new Dictionary<string, object>();

    return BadRequestError(title, detail, type, errors);
}
```

> 💡 **Encaja directamente con [`Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md)**, que produce `IEnumerable<ValidationResult>`. El resultado es un `errors` con la forma `{ "Nombre": ["El nombre es obligatorio"] }`, idéntica a la de `ModelState` de ASP.NET Core.

> ⚠️ **`Status` y `StatusCode` están duplicados** en cada objeto con el mismo valor. Redundante, pero inofensivo.

> ⚠️ **La URL `type` por defecto es `https://www.puntonetalpunto.net/`**, el sitio del autor. Según RFC 7807, `type` debería ser un URI que identifique **el tipo de problema**. **Pasa siempre tu propio `type`** en APIs públicas.

> ⚠️ **El objeto interno es anónimo**, no un `ProblemDetails` de ASP.NET Core. La capa que lo consuma debe leerlo por reflexión o serializarlo tal cual.

---

## Registro en el contenedor

Seis extensiones, tres ciclos de vida × dos modos:

```csharp
// Modo simple: IGenServiceFp<,>
services.AddTransientGenServicesFpWithoutReposGeneral();
services.AddScopedtGenServicesFpWithoutReposGeneral();      // ⚠️ "Scopedt", con t
services.AddSingletonGenServicesFpWithoutReposGeneral();

// Modo duplex: IGenServiceFp<,,>
services.AddTransientGenServicesDuplexFpWithoutReposGeneral();
services.AddScopedtGenServicesDuplexFpWithoutReposGeneral(); // ⚠️ "Scopedt", con t
services.AddSingletonGenServicesDuplexFpWithoutReposGeneral();
```

Cada una registra el tipo genérico **abierto**, así que sirve para cualquier combinación de entidad y DTO sin registrar nada más:

```csharp
services.AddScoped(typeof(IGenServiceFp<,>), typeof(GenServiceFp<,>));
```

> ⚠️ **Errata en el nombre: `AddScopedt…`** (con una `t` de más). Está así en el código de ambas variantes, simple y duplex. Corregirlo sería un cambio de API, por lo que **debes escribirlo con la errata**.

> ⚠️ **El nombre `WithoutReposGeneral` sugiere que existe una variante `WithRepos`, y no existe.** Solo hay estas seis. También hay un `AddWebServices` completamente comentado.

### Configuración completa mínima

```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));

// 1️⃣ Los repositorios, por entidad
builder.Services.AddScopedOOFPRepos<Vino, AppDbContext>();

// 2️⃣ El servicio genérico, una sola vez
builder.Services.AddScopedtGenServicesFpWithoutReposGeneral();
```

> 🔑 **El registro de repositorios es por entidad; el del servicio, una sola vez.** El servicio genérico resuelve `IEFRepoFp<TEntity>` por constructor, así que si olvidas el `AddScopedOOFPRepos<TEntity, TContext>()` la inyección del servicio fallará al resolverse.

> ⚠️ **Evita `AddSingleton…`**: el servicio depende de `IEFRepoFp<TEntity>`, que a su vez depende del `DbContext` (`Scoped`). Es una *captive dependency*, igual que en [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md).

---

## Mapeo con Mapster

El mapeo se hace con `Adapt<T>()`, que **no requiere configuración** si los nombres de propiedad coinciden:

```csharp
dto.Adapt<TEntity>()      // DTO  → entidad
bdData.Adapt<TDto>()      // entidad → DTO
```

Si los nombres difieren, configura Mapster **una vez** al arrancar:

```csharp
TypeAdapterConfig<Vino, VinoDto>.NewConfig()
    .Map(dest => dest.NombreCompleto, src => $"{src.Nombre} ({src.Bodega})")
    .Ignore(dest => dest.CampoCalculado);
```

> ⚠️ **El mapeo va dentro de `TryMapAsync`**, así que un error de configuración de Mapster **no lanza: se convierte en `Fail`**. Es seguro, pero el error puede pasar desapercibido si no revisas el log. Valida tus mapeos con `TypeAdapterConfig.Global.Compile()` al arrancar.

> ⚠️ **`DeleteAsync(dto, …)` usa `MapAsync`, no `TryMapAsync`** para el mapeo:
> ```csharp
> .MapAsync( _ => dto.Adapt<TEntity>())    // ⚠️ Map, no TryMap
> ```
> Es la **única** operación con esta inconsistencia. Si el mapeo lanza, **la excepción se propaga** en lugar de convertirse en `Fail`.

---

## ⚠️ Particularidades reales del código fuente

### 1. ❗ `UpdateProblemDetailsAsync` descarta la entidad recuperada

```csharp
.BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails, ct, pk))  // 1️⃣ busca…
.TryMapAsync( _     => dto.Adapt<TEntity>())                              // 2️⃣ …y descarta el resultado
.BindAsync  (bdData => _repo.TryUpdateAsync(item: bdData, …, pk: pk))     // 3️⃣ actualiza el mapeado
```

El `TryFindAsync` sirve **solo como comprobación de existencia**: su resultado se ignora con `_ =>` y se persiste la entidad recién mapeada desde el DTO.

**El problema real**: `TryFindAsync` deja la entidad de la base de datos **con seguimiento** en el `DbContext`. Después, `TryUpdateAsync` recibe **otra instancia** con la misma clave. EF Core puede lanzar:

> *The instance of entity type 'Vino' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked.*

Y para colmo, `TryUpdateAsync(item, notFoundErrorDetails, ct, pk)` del repositorio **vuelve a hacer un `TryFind` interno`, así que hay **dos búsquedas** para una sola actualización.

> ❗ **Es el defecto más importante de esta capa.** Si `UpdateAsync` con `pk` te falla con un error de seguimiento, usa la sobrecarga **sin `pk`**:
> ```csharp
> await _service.UpdateAsync(dto, ct);   // ✅ un solo Update, sin Find previo
> ```
> A cambio pierdes la comprobación de existencia (obtendrás un error de concurrencia en lugar de un 404 limpio).

### 2. ⚠️ Los mensajes de "no encontrado" no producen un 404 real

`BuildNotFoundPkError` genera el diccionario con la clave **`"NotFound"`**:

```csharp
new Dictionary<string, object> { ["NotFound"] = "No data found for the …" }
```

Pero la capa HTTP busca la clave **`"ProblemsDetails"`** (la constante de `Helpers/Constants.cs`) para extraer el `StatusCode`.

| Llamada | Clave del diccionario | ¿Se traduce a 404? |
|---|---|---|
| `FindByIdAsync(ct, id)` | `"NotFound"` | ❌ No: será un 400 o 500 genérico |
| `FindByIdProblemsDetailsAsync(MlProblemsDetails.NotFoundError(…), ct, id)` | `"ProblemsDetails"` | ✅ Sí |

> ❗ **Para obtener un 404 correcto, usa siempre las variantes `ProblemDetailsAsync` con `MlProblemsDetails.NotFoundError(…)`:**
> ```csharp
> await _service.FindByIdProblemsDetailsAsync(
>           MlProblemsDetails.NotFoundError(detail: $"El vino {id} no existe"),
>           ct, pk: id);
> ```

### 3. ⚠️ `Constants.ProblemsDetails` es de tipo `Name`, y `MlProblemsDetails` no lo usa

```csharp
// Helpers/Constants.cs — público, tipo Name (value object)
public static Name ProblemsDetails => "ProblemsDetails";

// MlProblemsDetails.cs — constante privada propia, tipo string
private const string ProblemsDetails = nameof(ProblemsDetails);
```

Hay **dos definiciones del mismo literal** con tipos distintos. Funcionan porque ambas valen `"ProblemsDetails"`, pero es una duplicación frágil: cambiar una sin la otra rompería la integración con la capa HTTP.

### 4. ⚠️ Inconsistencia en las claves de "no encontrado" del modo duplex

```csharp
// Modo simple: usa el nombre del DTO
typeof(TDto).Name.BuildNotFoundPkError(pk)          // "No data found for the VinoDto table…"

// Modo duplex FindById: usa el nombre de la RESPUESTA
typeof(TResponse).Name.BuildNotFoundPkError(pk)     // "…for the VinoResponse table…"

// Modo duplex Update/Delete: usa el nombre de la ENTIDAD
typeof(TEntity).Name.BuildNotFoundPkError(pk)       // "…for the Vino table…"
```

Tres criterios distintos para el mismo concepto. Además, **ninguno es realmente el nombre de la tabla**: es el nombre del tipo.

### 5. ⚠️ `DeleteAsync` no permite personalizar el mensaje de éxito

```csharp
validBuildMessage: _ => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
```

El parámetro `validMessageBuilder` **no existe** en la firma de `DeleteAsync`, así que el mensaje está fijado. Las otras cuatro operaciones sí lo aceptan.

### 6. ⚠️ `DeleteAsync(dto, …)` usa `MapAsync` en lugar de `TryMapAsync`

Ya descrito en [Mapeo con Mapster](#mapeo-con-mapster): es la única operación cuyo mapeo **no está protegido**, por lo que una excepción de Mapster se propagaría en lugar de convertirse en `Fail`.

### 7. ⚠️ `errors.ToString()` en los mensajes de error por defecto

```csharp
failBuildMessage: errors => failMessageBuilder is null
                                ? $"An error occurred … Error: {errors.ToString()}"
                                : failMessageBuilder(errors);
```

Se llama a `ToString()` sobre el `MlErrorsDetails`. Si el tipo no lo sobrescribe de forma útil, el log podría mostrar solo el nombre del tipo en lugar de los mensajes.

> 💡 **Pasa siempre `failMessageBuilder`** con `e => e.ToErrorsDescription()` para asegurarte de que el log contiene los mensajes reales.

### 8. ⚠️ `EnsureFp.That(pk, pk is not null && pk.Any(), …)` evalúa `pk.Any()` de forma anticipada

```csharp
EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty")
```

La condición se evalúa **antes** de llamar a `EnsureFp.That`, no dentro. El cortocircuito de `&&` evita la `NullReferenceException`, así que es correcto, pero significa que la comprobación **no forma parte del raíl**: si `pk` fuera un enumerable de un solo uso, ya se habría consumido. Con `object[]` no hay problema.

### 9. ⚠️ `Microsoft.Extensions.Configuration.Abstractions` se declara sin usarse

Está en el `.csproj` y en los `GlobalUsings`, pero **ninguna clase usa `IConfiguration`**. Residuo del `AddWebServices(this IServiceCollection, IConfiguration)` que quedó comentado en `RegisterServices`.

### 10. ⚠️ `GenService.cs` está vacío

Fichero de 0 bytes en `Services/`. No hay variante OOP del servicio.

### 11. ⚠️ La segunda mitad de `IGenServiceFp.cs` está escrita en una sola línea por método

El contrato del modo simple está formateado con alineación cuidadosa; el del modo duplex, y las últimas dos firmas del simple, están en líneas de más de 300 caracteres con `= null` (sin `!`), lo que genera advertencias de nulabilidad. Cosmético.

### 12. ⚠️ No hay `CancellationToken` en la traza inicial

`LogMlResultInformationAsync` no recibe el `ct`, así que la primera traza se escribe aunque la operación ya esté cancelada. Detalle menor.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay paginación.** `AllAsync` trae **toda la tabla** y la mapea completa. Aunque [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md) ofrece `IEFRepoPaginationFp<T>`, esta capa depende solo de `IEFRepoFp<T>` y **no expone ningún método paginado**. En una tabla grande, `AllAsync` es un problema de rendimiento serio.

> ⚠️ **No hay filtrado ni búsqueda.** No existe un `GetDataAsync(filter)`: solo `AllAsync` y `FindByIdAsync`.

> ⚠️ **No hay validación integrada.** El servicio solo comprueba `NotNull`. No invoca [`Validation`](../MoralesLarios.OOFP.Validation/README.md) ni DataAnnotations: debes validar **antes** de llamarlo.

> ⚠️ **No hay operaciones por lotes.** No hay `CreateRangeAsync` ni equivalentes, aunque el repositorio los ofrece.

> ⚠️ **No hay transacciones.** Hereda la limitación de `EFCore`: cada operación confirma por su cuenta.

> ⚠️ **No hay caché.**

> ⚠️ **No hay autorización.** Las fábricas `UnauthorizedError` y `ForbiddenError` existen, pero **el servicio nunca las usa**: son para que las emplee tu código.

> ⚠️ **No hay proyecciones eficientes.** El mapeo a DTO ocurre **en memoria** tras traer la entidad completa: no se traduce a un `SELECT` de columnas concretas.

> ⚠️ **No hay tests unitarios propios.** El proyecto `MoralesLarios.OOFP.WebServices.Tests.Unit` existe como proyecto separado.

---

## Ejemplos prácticos

### El modelo de los ejemplos

```csharp
// Entidad de base de datos
public class Vino
{
    public int      Id           { get; set; }
    public string   Nombre       { get; set; } = string.Empty;
    public string   Bodega       { get; set; } = string.Empty;
    public int      Anyo         { get; set; }
    public DateTime FechaEntrada { get; set; }
}

// DTO único (modo simple)
public class VinoDto
{
    public int    Id     { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public int    Anyo   { get; set; }
}

// DTOs separados (modo duplex)
public class VinoRequest                    // sin Id: lo genera la BD
{
    public string Nombre { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public int    Anyo   { get; set; }
}

public class VinoResponse                   // con Id y fecha
{
    public int      Id           { get; set; }
    public string   Nombre       { get; set; } = string.Empty;
    public string   Bodega       { get; set; } = string.Empty;
    public int      Anyo         { get; set; }
    public DateTime FechaEntrada { get; set; }
}
```

### Ejemplo 1 — Configuración completa

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(cs));

// Repositorios: una llamada por entidad
builder.Services.AddScopedOOFPRepos<Vino, AppDbContext>();

// Servicios genéricos: una sola vez cada modo
builder.Services.AddScopedtGenServicesFpWithoutReposGeneral();        // ⚠️ "Scopedt"
builder.Services.AddScopedtGenServicesDuplexFpWithoutReposGeneral();

var app = builder.Build();
```

### Ejemplo 2 — Uso directo del servicio genérico

```csharp
public class VinosAppService(IGenServiceFp<Vino, VinoDto> _service)
{
    public Task<MlResult<IEnumerable<VinoDto>>> TodosAsync(CancellationToken ct)
        => _service.AllAsync(ct);

    public Task<MlResult<VinoDto?>> BuscarAsync(int id, CancellationToken ct)
        => _service.FindByIdProblemsDetailsAsync(
                        MlProblemsDetails.NotFoundError(detail: $"El vino {id} no existe"),
                        ct, pk: id);   // 🔑 así sí se traduce a 404

    public Task<MlResult<VinoDto>> CrearAsync(VinoDto dto, CancellationToken ct)
        => _service.CreateAsync(dto, ct);
}
```

### Ejemplo 3 — Modo duplex

```csharp
public class VinosDuplexService(IGenServiceFp<Vino, VinoRequest, VinoResponse> _service)
{
    public Task<MlResult<VinoResponse>> CrearAsync(VinoRequest request, CancellationToken ct)
        => _service.CreateAsync(
                        request,
                        ct,
                        initialMessage     : $"Alta de vino: {request.Nombre}",
                        validMessageBuilder: r => $"Vino creado con Id {r.Id}",
                        failMessageBuilder : e => $"Alta fallida: {e.ToErrorsDescription()}");
}
```

### Ejemplo 4 — Validación previa (el servicio no valida)

```csharp
using MoralesLarios.OOFP.Validation.FluentValidations;

public class VinosValidadosService(IGenServiceFp<Vino, VinoDto> _service,
                                   IValidator<VinoDto>          _validator)
{
    public Task<MlResult<VinoDto>> CrearAsync(VinoDto dto, CancellationToken ct)
        => _validator.MlValidate(dto)                          // 1️⃣ valida primero
                     .BindAsync(v => _service.CreateAsync(v, ct));   // 2️⃣ y solo entonces crea
}
```

### Ejemplo 5 — Extender por herencia

```csharp
public class VinosServiceConReglas(IEFRepoFp<Vino>                          repo,
                                   ILogger<GenServiceFp<Vino, VinoDto>>     logger)
    : GenServiceFp<Vino, VinoDto>(repo, logger)
{
    public override Task<MlResult<VinoDto>> CreateAsync(
            VinoDto dto, CancellationToken ct = default!,
            string initialMessage = null!,
            Func<VinoDto, string> validMessageBuilder = null!,
            Func<MlErrorsDetails, string> failMessageBuilder = null!)
        => EnsureFp.That(dto, d => d.Anyo <= DateTime.Now.Year,
                         MlProblemsDetails.UnprocessableContentError(
                             detail: "El año de la cosecha no puede ser futuro"))
                   .ToAsync()
                   .BindAsync(d => base.CreateAsync(d, ct, initialMessage,
                                                    validMessageBuilder, failMessageBuilder));
}
```

Registro de la clase derivada (sustituye al genérico para esa entidad):

```csharp
builder.Services.AddScoped<IGenServiceFp<Vino, VinoDto>, VinosServiceConReglas>();
```

### Ejemplo 6 — Errores HTTP con `MlProblemsDetails`

```csharp
// 404 con detalle propio
MlProblemsDetails.NotFoundError(detail: $"No existe el vino {id}");

// 409 al detectar un duplicado
MlProblemsDetails.ConflictError(detail: $"Ya existe un vino llamado '{nombre}'");

// 400 con errores por campo
MlProblemsDetails.BadRequestError(
    detail: "Datos inválidos",
    errors: new Dictionary<string, object>
    {
        ["Nombre"] = new[] { "El nombre es obligatorio" },
        ["Anyo"]   = new[] { "El año debe estar entre 1900 y el actual" }
    });

// Código arbitrario con tu propio type
MlProblemsDetails.CreateProblemDetails(
    statusCode: 418,
    title     : "I'm a teapot",
    detail    : "Este endpoint no sirve café",
    type      : "https://api.miempresa.com/problems/teapot");
```

### Ejemplo 7 — ❌ Qué no hacer / ✅ qué hacer

**El ciclo de vida:**

```csharp
// ❌ MAL: Singleton, y el repositorio depende de un DbContext Scoped
services.AddSingletonGenServicesFpWithoutReposGeneral();

// ✅ BIEN
services.AddScopedtGenServicesFpWithoutReposGeneral();
```

**Obtener un 404 de verdad:**

```csharp
// ❌ MAL: la clave "NotFound" no la interpreta la capa HTTP
await _service.FindByIdAsync(ct, pk: id);

// ✅ BIEN: MlProblemsDetails genera la clave "ProblemsDetails" con Status 404
await _service.FindByIdProblemsDetailsAsync(
                   MlProblemsDetails.NotFoundError(detail: $"El vino {id} no existe"),
                   ct, pk: id);
```

**Modificar sin conflictos de seguimiento:**

```csharp
// ❌ MAL: hace TryFind + TryUpdate(con otro TryFind interno) → posible error de tracking
await _service.UpdateAsync(dto, ct, pk: dto.Id);

// ✅ BIEN: un solo Update, sin Find previo
await _service.UpdateAsync(dto, ct);
```

**Leer tablas grandes:**

```csharp
// ❌ MAL: AllAsync trae y mapea TODA la tabla
await _service.AllAsync(ct);

// ✅ BIEN: pagina con el repositorio directamente
await _repoPaginado.TryGetDataPaginationAsync((1, 25), OrderBy.Ascending, x => x.Id, x => true, ct)
                   .MapAsync(p => p.Items.Adapt<IEnumerable<VinoDto>>());
```

**Validar antes de persistir:**

```csharp
// ❌ MAL: el servicio solo comprueba que no sea null
await _service.CreateAsync(dtoSinValidar, ct);

// ✅ BIEN: valida en el raíl y solo entonces crea
await _validator.MlValidate(dto).BindAsync(v => _service.CreateAsync(v, ct));
```

**Trazar el error real:**

```csharp
// ❌ MAL: el mensaje por defecto usa errors.ToString()
await _service.CreateAsync(dto, ct);

// ✅ BIEN: construye el mensaje con la descripción de los errores
await _service.CreateAsync(dto, ct,
                           failMessageBuilder: e => $"Alta fallida: {e.ToErrorsDescription()}");
```

**El `type` del ProblemDetails:**

```csharp
// ❌ MAL: usa la URL por defecto del autor de la librería
MlProblemsDetails.NotFoundError(detail: "No existe");

// ✅ BIEN: un URI que identifique el tipo de problema en TU API
MlProblemsDetails.NotFoundError(detail: "No existe",
                                type  : "https://api.miempresa.com/problems/not-found");
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| CRUD con un solo DTO | `IGenServiceFp<TEntity, TDto>` |
| DTO de entrada ≠ de salida | `IGenServiceFp<TEntity, TRequest, TResponse>` ✅ en APIs públicas |
| Listar todo | `AllAsync(ct)` ⚠️ sin paginación |
| Buscar por PK con 404 correcto | `FindByIdProblemsDetailsAsync(MlProblemsDetails.NotFoundError(…), ct, pk)` ✅ |
| Crear | `CreateAsync(dto, ct)` |
| Modificar sin problemas de tracking | `UpdateAsync(dto, ct)` ✅ sin `pk` |
| Modificar comprobando existencia | `UpdateProblemDetailsAsync(dto, error, ct, …, pk)` ⚠️ ver particularidad 1 |
| Borrar por PK | `DeleteProblemDetailsAsync(error, ct, …, pk)` |
| Borrar una entidad que ya tengo | `DeleteAsync(dto, ct)` |
| Un error 400 con errores por campo | `MlProblemsDetails.BadRequestError(…, validationResults)` |
| Un código de estado arbitrario | `MlProblemsDetails.CreateProblemDetails(statusCode, …)` |
| Añadir reglas de negocio | Heredar de `GenServiceFp<,>` y sobrescribir el método |
| Paginar, filtrar o proyectar | ❌ No disponible: usa `IEFRepoPaginationFp<T>` directamente |
| Validar | ❌ No lo hace: usa [`Validation`](../MoralesLarios.OOFP.Validation/README.md) antes |
| Registrar | `AddScopedtGenServicesFpWithoutReposGeneral()` ✅ (con la errata) |

---

## Mejores prácticas

1. **Usa el modo duplex en APIs públicas**: separa el DTO de entrada del de salida para no exponer `Id` ni campos internos.
2. **Registra con `AddScopedt…`** (sí, con la errata). Evita `AddSingleton…` por la dependencia cautiva del `DbContext`.
3. **No olvides `AddScopedOOFPRepos<TEntity, TContext>()`** por cada entidad: el servicio resuelve `IEFRepoFp<TEntity>` por constructor.
4. **Usa siempre las variantes `ProblemDetailsAsync` con `MlProblemsDetails`** si el error va a convertirse en respuesta HTTP. Las versiones cortas **no producen un 404 real**.
5. **Prefiere `UpdateAsync(dto, ct)` sin `pk`** para evitar el doble `Find` y los conflictos de seguimiento.
6. **No uses `AllAsync` en tablas grandes**: no pagina. Baja al repositorio paginado y mapea con `Adapt`.
7. **Valida antes de llamar al servicio**, con [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) o [`Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md).
8. **Pasa `failMessageBuilder` con `e => e.ToErrorsDescription()`** para que el log contenga los mensajes reales.
9. **Pasa siempre tu propio `type`** en `MlProblemsDetails`: el valor por defecto apunta al sitio del autor de la librería.
10. **Configura y compila los mapeos de Mapster al arrancar** (`TypeAdapterConfig.Global.Compile()`): los errores de mapeo se convierten en `Fail` silenciosos.
11. **Hereda y sobrescribe** para añadir reglas de negocio en lugar de escribir un servicio nuevo: todos los métodos son `virtual`.
12. **Personaliza `initialMessage`** con contexto útil (usuario, correlación): los mensajes por defecto son genéricos y en inglés.
13. **Si necesitas filtrado, proyecciones o transacciones, baja al repositorio.** Esta capa cubre el CRUD por clave, no las consultas.

---

## Resumen

- Servicio de aplicación **genérico** que implementa el CRUD completo sobre `IEFRepoFp<TEntity>`, mapeando entidad ⇄ DTO con **Mapster** y trazando **entrada y salida** de cada operación en el raíl de `MlResult<T>`.
- **Dos modos**: simple (`IGenServiceFp<TEntity, TDto>`)
- Registro con tipos genéricos abiertos: **una sola llamada** sirve para todas las entidades. ⚠️ El nombre tiene la errata `AddScopedt…`.
- **`MlProblemsDetails`**: 15 fábricas de `MlErrorsDetails` con la estructura **RFC 7807** (400, 401, 403, 404, 409, 422, 429, 500…), más una sobrecarga que agrupa `ValidationResult` por propiedad.
- ❗ **Para obtener un 404 real, usa las variantes `ProblemDetailsAsync` con `MlProblemsDetails.NotFoundError(…)`**: las versiones cortas generan la clave `"NotFound"`, que la capa HTTP no interpreta.
- ❗ **`UpdateProblemDetailsAsync` hace dos búsquedas y descarta la entidad recuperada**, lo que puede provocar conflictos de seguimiento en EF Core. Prefiere `UpdateAsync(dto, ct)` sin `pk`.
- ⚠️ **No hay paginación, filtrado ni validación**: `AllAsync` trae toda la tabla, y debes validar antes de llamar al servicio.
- ⚠️ **Detalles menores**: `DeleteAsync` no permite personalizar el mensaje de éxito, `DeleteAsync(dto)` usa `MapAsync` sin protección, y `GenService.cs` está vacío.
- **Todos los métodos son `virtual`**: extiende por herencia para añadir reglas de negocio.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — 🔑 el repositorio que consume este servicio
- [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) — 🔑 `LogMlResultInformationAsync`, `LogMlResultFinalAsync`
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — convierte el `MlResult` y el `ProblemDetails` en respuesta HTTP
- [`MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md) — controladores base que usan este servicio
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validar el DTO antes de llamar al servicio
- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — 🔑 produce los `ValidationResult` de `BadRequestError`
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — validación fluida
- [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) — tipos compartidos

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — mensajes y detalles del error](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md) 🔑 la base de `MlProblemsDetails`
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` — transformación con cortocircuito](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`TryMap` y `TryBind` — capturar excepciones en el raíl](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`EnsureFp` — validaciones de guarda](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [Métodos asíncronos en el raíl](../MoralesLarios.FOOP/__Doc/1_Intro.md#sufijos-de-asincronía) 🔑 todo el servicio es asíncrono
