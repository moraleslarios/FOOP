# MoralesLarios.OOFP.WebControllers.Cache

Capa de **caché HTTP** sobre los controladores genéricos de
[`MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers).

Ofrece:

- Una **clase base de controlador** con los endpoints CRUD ya cacheados (`GET`)
  e invalidación automática del caché en escrituras (`POST`, `PUT`, `DELETE`),
  tanto para escenarios de DTO único como para escenarios **duplex**.
- Una **política de OutputCache por controlador** (`PerControllerOutputCachePolicy`)
  que etiqueta cada respuesta con un *tag* único por controlador, lo que permite
  invalidar de forma selectiva sin afectar a otros endpoints.
- Un **atributo personalizado** (`MlControllerCacheAttribute`) para declarar el
  cache de manera concisa, con duración opcional.
- **Bypass dinámico del caché** mediante el header `X-Bypass-Cache` o el
  estándar HTTP `Cache-Control: no-cache` / `no-store`.
- Un endpoint listo para uso (`GET clear-cache/now`) para invalidar el caché del
  controlador en caliente.

---

## Tabla de contenido

- [Dependencias](#dependencias)
- [Registro en `Program.cs`](#registro-en-programcs)
- [Estructura del proyecto](#estructura-del-proyecto)
- [`PerControllerOutputCachePolicy`](#percontrolleroutputcachepolicy)
- [`MlControllerCacheAttribute`](#mlcontrollercacheattribute)
- [`SimpleMlCacheControllerBase<TEntity, TDto, TPk>`](#simplemlcachecontrollerbasetentity-tdto-tpk)
- [`SimpleMlCacheControllerBase<TEntity, TRequest, TResponse, TPk>`](#simplemlcachecontrollerbasetentity-trequest-tresponse-tpk)
- [`SimpleMlComplexCacheControllerBase<TEntity, TDto>` (PK compuesta)](#simplemlcomplexcachecontrollerbasetentity-tdto-pk-compuesta)
- [`SimpleMlComplexCacheControllerBase<TEntity, TRequest, TResponse>` (PK compuesta duplex)](#simplemlcomplexcachecontrollerbasetentity-trequest-tresponse-pk-compuesta-duplex)
- [Bypass dinámico del caché](#bypass-dinámico-del-caché)
- [Invalidación manual](#invalidación-manual)
- [Personalización avanzada](#personalización-avanzada)

---

## Dependencias

| Dependencia                                       | Tipo               |
|---------------------------------------------------|--------------------|
| `Microsoft.AspNetCore.App` (framework)            | `FrameworkReference` |
| `Microsoft.AspNetCore.OutputCaching` (incluido)   | Implícito ASP.NET 8  |
| `MoralesLarios.OOFP.WebControllers`               | `ProjectReference`   |

Target framework: **`net8.0`**.
La caché se basa enteramente en `Microsoft.AspNetCore.OutputCaching`, no en el
viejo `ResponseCaching`.

---

## Registro en `Program.cs`

```csharp
using MoralesLarios.OOFP.WebControllers.Cache;

var builder = WebApplication.CreateBuilder(args);

// Registra la política "PerControllerTag" y los servicios de OutputCache.
builder.Services.AddWebControllersCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(2); // opcional
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseOutputCache();   // <-- imprescindible. Antes de MapControllers.

app.MapControllers();

app.Run();
```

> ?? **Importante**: `app.UseOutputCache()` debe colocarse **después** de
> `UseRouting` y **antes** de `MapControllers`/`UseEndpoints` para que el
> middleware sepa qué endpoint se está sirviendo.

`AddWebControllersCache` recibe un `Action<OutputCacheOptions>` opcional para
configurar el `DefaultExpirationTimeSpan`, registrar otras políticas, etc.

```csharp
public static IServiceCollection AddWebControllersCache(
    this IServiceCollection services,
    Action<OutputCacheOptions> options = null!)
```

Internamente registra la política con el nombre **`"PerControllerTag"`** apuntando
a `PerControllerOutputCachePolicy.Instance`.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.WebControllers.Cache/
??? Controllers/
?   ??? SimpleMlCacheControllerBase.cs            // controlador base con caché (PK simple)
?   ??? SimpleMlComplexCacheControllerBase.cs     // controlador base con caché (PK compuesta)
??? Policies/
?   ??? PerControllerOutputCachePolicy.cs         // política IOutputCachePolicy
?   ??? MlControllerCacheAttribute.cs             // atributo IOutputCachePolicy
??? GlobalUsings.cs
??? RegisterServices.cs                            // AddWebControllersCache
??? README.md
```

---

## `PerControllerOutputCachePolicy`

Política de `OutputCache` (`IOutputCachePolicy`) que se aplica a cada request
cacheable y se encarga de:

1. Decidir si la request **debe ser cacheada** o **debe saltarse el caché** según
   los headers (`X-Bypass-Cache`, `Cache-Control`).
2. Etiquetar la respuesta con un *tag* `oc:{nombreDelController}` para poder
   invalidarla luego con `IOutputCacheStore.EvictByTagAsync`.

### API pública

```csharp
public sealed class PerControllerOutputCachePolicy : IOutputCachePolicy
{
    public const string BypassHeader = "X-Bypass-Cache";

    public static readonly PerControllerOutputCachePolicy Instance;

    public static string GetControllerTag(HttpContext httpContext);
}
```

### Tag por controlador

```csharp
PerControllerOutputCachePolicy.GetControllerTag(HttpContext);
// devuelve "oc:Pruebas" para un PruebasController
```

### Valores de `X-Bypass-Cache` aceptados

`1`, `true`, `yes`, `on`, `no-cache`, `no-store`, `bypass` (case-insensitive).

Si el header `Cache-Control` contiene `no-cache` o `no-store` el bypass también
se activa.

### Uso directo (sin atributo)

```csharp
[HttpGet]
[OutputCache(PolicyName = "PerControllerTag")]
public Task<IActionResult> GetAllAsync(CancellationToken ct = default)
    => _service.AllAsync(ct).ToGetPdActionResultAsync();
```

---

## `MlControllerCacheAttribute`

Atributo que **implementa `IOutputCachePolicy`** y delega en
`PerControllerOutputCachePolicy`. Equivale a
`[OutputCache(PolicyName = "PerControllerTag")]` pero con un constructor adicional
para indicar la duración solo en ese endpoint.

### Firma

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MlControllerCacheAttribute : Attribute, IOutputCachePolicy
{
    public MlControllerCacheAttribute();
    public MlControllerCacheAttribute(int durationSeconds);
}
```

- **Sin parámetros** ? utiliza `OutputCacheOptions.DefaultExpirationTimeSpan`
  (configurado en `AddWebControllersCache`).
- **`durationSeconds > 0`** ? fija `ResponseExpirationTimeSpan` solo para ese
  endpoint. Si se pasa `<= 0` se ignora y se usa el por defecto.

### Ejemplos

```csharp
public class PruebasController : SimpleMlCacheControllerBase<Prueba, PruebaDto, int>
{
    // 1. Duración por defecto del proyecto
    [HttpGet("simple")]
    [MlControllerCache]
    public Task<IActionResult> Simple() => /* ... */;

    // 2. Duración específica de 30 segundos
    [HttpGet("short")]
    [MlControllerCache(30)]
    public Task<IActionResult> Short() => /* ... */;

    // 3. Aplicado a todo el controlador
    [MlControllerCache(120)]
    public class CatalogoController : ControllerBase { /* ... */ }
}
```

---

## `SimpleMlCacheControllerBase<TEntity, TDto, TPk>`

Hereda de `SimpleMlControllerBase<TEntity, TDto, TPk>`
(en `MoralesLarios.OOFP.WebControllers`) y añade:

- `[MlControllerCache]` en `GetAllAsync` y `GetByIdAsync`.
- Invalidación automática del *tag* del controlador en `POST`, `PUT`, `DELETE`
  llamando a `EvictControllerCacheAsync` antes de delegar en `base.*`.
- Endpoint público `GET clear-cache/now` para vaciar el caché manualmente.

### Firma

### Ejemplo de uso

```csharp
[ApiController]
[Route("api/[controller]")]
public class PruebasController(
        IGenServiceFp<Prueba, PruebaDto> service,
        IOutputCacheStore                cacheStore)
    : SimpleMlCacheControllerBase<Prueba, PruebaDto, int>(service, cacheStore)
{
}
```

Con esto el controlador gana automáticamente:

| Verbo  | Ruta                       | Comportamiento                                  |
|--------|----------------------------|-------------------------------------------------|
| GET    | `/api/Pruebas`             | Cacheado con tag `oc:Pruebas`                   |
| GET    | `/api/Pruebas/id-str/{id}` | Cacheado con tag `oc:Pruebas`                   |
| POST   | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el insert  |
| PUT    | `/api/Pruebas/{id}`        | Invalida `oc:Pruebas`, luego ejecuta el update  |
| PUT    | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el update  |
| DELETE | `/api/Pruebas/{id}`        | Invalida `oc:Pruebas`, luego ejecuta el delete  |
| DELETE | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el delete  |
| GET    | `/api/Pruebas/clear-cache/now` | Vacía manualmente el caché del controlador  |

### Sobreescribir endpoints concretos

Puedes seguir añadiendo endpoints personalizados sobre el mismo controlador:

```csharp
[HttpGet("with-cache1")]
[MlControllerCache(60)]    // 1 minuto solo aquí
public async Task<IActionResult> GetWithCache1(CancellationToken ct = default)
    => await base.GetAllAsync(ct);
```

> ?? Devuelve directamente el `IActionResult` que produce el método base; no lo
> envuelvas en `Ok(result)` porque ya es un `OkObjectResult` y se serializaría
> como objeto en lugar de array.

---

## `SimpleMlCacheControllerBase<TEntity, TRequest, TResponse, TPk>`

Variante duplex de `SimpleMlCacheControllerBase<TEntity, TDto, TPk>` para escenarios donde la petición y la respuesta usan modelos distintos. Hereda de `SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>` y mantiene exactamente las mismas garantías de cacheado e invalidación.

### Firma

```csharp
public class SimpleMlCacheControllerBase<TEntity, TRequest, TResponse, TPk>(IGenServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                            IOutputCacheStore                           _outputCacheStore)
    : SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>(_genServiceFp)
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{
    [MlControllerCache] public override Task<IActionResult> GetAllAsync(CancellationToken ct = default!);
    [MlControllerCache] public override Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default);

    public override Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync(string id, [FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default);
    public override Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!);

    [HttpGet("clear-cache/now")]
    public virtual Task EvictControllerCacheAsync(CancellationToken ct = default);
}
```

### Cuándo usarlo

Úsalo cuando quieras cachear un controlador estándar pero la capa de escritura y la de lectura no compartan el mismo DTO.

---

## `SimpleMlComplexCacheControllerBase<TEntity, TDto>` (PK compuesta)

Variante de `SimpleMlCacheControllerBase<,,>` para entidades con **clave primaria compuesta**. Hereda de `SimpleMlComplexPkControllerBase<TEntity, TDto>` (en `MoralesLarios.OOFP.WebControllers`) y añade exactamente las mismas garantías que `SimpleMlCacheControllerBase`:

- `[MlControllerCache]` en `GetAllAsync` y `GetByIdAsync`.
- Invalidación automática del *tag* del controlador en `POST`, `PUT`, `DELETE` antes de delegar en `base.*`.
- Endpoint público `GET clear-cache/now` heredado para vaciar el caché manualmente.

### Firma

```csharp
public class SimpleMlComplexCacheControllerBase<TEntity, TDto>(
        IGenServiceFp<TEntity, TDto> _genServiceFp,
        Func<TEntity, object[]>      _pkFields,
        IOutputCacheStore            _outputCacheStore)
    : SimpleMlComplexPkControllerBase<TEntity, TDto>(_genServiceFp, _pkFields)
    where TEntity : class
    where TDto    : class
{
    [MlControllerCache] public override Task<IActionResult> GetAllAsync(CancellationToken ct = default!);
    [MlControllerCache] public override Task<IActionResult> GetByIdAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);

    public override Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync([FromRoute][PkParameter] string ids, [FromBody] TDto dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!);
    public override Task<IActionResult> DeleteAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);
    public override Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!);

    [HttpGet("clear-cache/now")]
    public virtual Task EvictControllerCacheAsync(CancellationToken ct = default);  // heredado
}
```

### Ejemplo de uso

```csharp
[ApiController]
[Route("api/[controller]")]
public class PruebaComplexController(
        IGenServiceFp<PruebaComplex, PruebaComplexDto> service,
        IOutputCacheStore                              cacheStore)
    : SimpleMlComplexCacheControllerBase<PruebaComplex, PruebaComplexDto>(
          service,
          e => new object[] { e.Nombre, e.Lugar, e.Precio, e.Fecha },
          cacheStore) { }
```

Endpoints disponibles automáticamente:

| Verbo  | Ruta                            | Comportamiento                                       |
|--------|---------------------------------|------------------------------------------------------|
| GET    | `/api/PruebaComplex`            | Cacheado con tag `oc:PruebaComplex`                  |
| GET    | `/api/PruebaComplex/id-str/{ids}` | Cacheado con tag `oc:PruebaComplex` (PK compuesta) |
| POST   | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el insert |
| PUT    | `/api/PruebaComplex/{ids}`      | Invalida `oc:PruebaComplex`, luego ejecuta el update |
| PUT    | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el update |
| DELETE | `/api/PruebaComplex/{ids}`      | Invalida `oc:PruebaComplex`, luego ejecuta el delete |
| DELETE | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el delete |
| GET    | `/api/PruebaComplex/clear-cache/now` | Vacía manualmente el caché del controlador      |

Ver detalles del formato de `ids` (PK compuesta con `DateTime`, `DateOnly`, etc.) en el [README de `MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md).

---

## `SimpleMlComplexCacheControllerBase<TEntity, TRequest, TResponse>` (PK compuesta duplex)

Variante duplex de `SimpleMlComplexCacheControllerBase<TEntity, TDto>` para entidades con clave primaria compuesta cuando la petición y la respuesta usan modelos distintos. Hereda de `SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>` y mantiene el mismo cacheado por controlador, invalidación automática y endpoint de borrado manual.

### Firma

```csharp
public class SimpleMlComplexCacheControllerBase<TEntity, TRequest, TResponse>(IGenServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                              Func<TEntity, object[]>                     _pkFields,
                                                                              IOutputCacheStore                           _outputCacheStore)
    : SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>(_genServiceFp, _pkFields)
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{
    [MlControllerCache] public override Task<IActionResult> GetAllAsync(CancellationToken ct = default!);
    [MlControllerCache] public override Task<IActionResult> GetByIdAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);

    public override Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync([FromRoute][PkParameter] string ids, [FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!);
    public override Task<IActionResult> DeleteAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!);
    public override Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!);

    [HttpGet("clear-cache/now")]
    public virtual Task EvictControllerCacheAsync(CancellationToken ct = default);
}
```

### Cuándo usarlo

Úsalo cuando combines PK compuesta, caché por controlador y separación entre modelos de lectura y escritura.

---

## Bypass dinámico del caché

Cualquier cliente puede pedir que la respuesta NO se sirva desde caché ni se
almacene, **solo para esa petición**, enviando uno de:

```http
GET /api/Pruebas
X-Bypass-Cache: 1
```

```http
GET /api/Pruebas
Cache-Control: no-cache
```

Internamente la policy hace:

```csharp
context.EnableOutputCaching = false;
context.AllowCacheLookup    = false;
context.AllowCacheStorage   = false;
context.AllowLocking        = false;
```

con lo que la request **no consulta** el almacén ni **escribe** sobre él.

### Ejemplo desde `HttpClient`

```csharp
var bypass = new Dictionary<string, string>
{
    { PerControllerOutputCachePolicy.BypassHeader, "1" }
};

var result = await _httpClientFactoryManager.GetAsync<IEnumerable<PruebaDto>>(
    httpClientFactoryKey,
    "with-cache1",
    bypass);
```

---

## Invalidación manual

### Desde un controlador hijo

```csharp
public class PruebasController(
        IGenServiceFp<Prueba, PruebaDto> service,
        IOutputCacheStore                store)
    : SimpleMlCacheControllerBase<Prueba, PruebaDto, int>(service, store)
{
    [HttpPost("flush")]
    public Task Flush(CancellationToken ct) => EvictControllerCacheAsync(ct);
}
```

### Desde fuera del controlador

```csharp
public class CacheService(IOutputCacheStore _store)
{
    public Task EvictAsync(string controllerName, CancellationToken ct = default)
        => _store.EvictByTagAsync($"oc:{controllerName}", ct);
}
```

### Endpoint integrado

```
GET /api/Pruebas/clear-cache/now
```

Disponible automáticamente en todos los controladores que heredan de
`SimpleMlCacheControllerBase<,,>`.

---

## Personalización avanzada

### Cambiar el `DefaultExpirationTimeSpan`

```csharp
services.AddWebControllersCache(o =>
{
    o.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
});
```

### Añadir más políticas custom

```csharp
services.AddWebControllersCache(o =>
{
    o.AddPolicy("LongLived", policy => policy.Expire(TimeSpan.FromHours(1)));
});
```

### Mezclar políticas en un mismo endpoint

```csharp
[HttpGet]
[MlControllerCache]
[OutputCache(PolicyName = "LongLived")]
public Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => /* ... */;
```

> Las políticas se aplican en orden; la última que escriba sobre el contexto
> manda. Úsalas con cuidado y prefiere una sola política por endpoint.

---

## Resumen

- Anotas `[MlControllerCache]` (con o sin segundos) ? caché por controlador con
  tag e invalidación selectiva.
- Heredas `SimpleMlCacheControllerBase<,,>` ? CRUD cacheado e invalidación
  automática gratis.
- Cliente envía `X-Bypass-Cache: 1` o `Cache-Control: no-cache` ? respuesta
  fresca solo para esa request.
- `IOutputCacheStore.EvictByTagAsync("oc:NombreController", ct)` o
  `GET /clear-cache/now` ? flush manual.
