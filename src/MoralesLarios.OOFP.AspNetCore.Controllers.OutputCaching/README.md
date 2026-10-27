# MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching

Capa de **cach� HTTP** sobre los controladores gen�ricos de
[`MoralesLarios.OOFP.AspNetCore.Controllers`](../MoralesLarios.OOFP.AspNetCore.Controllers).

Ofrece:

- Una **clase base de controlador** con los endpoints CRUD ya cacheados (`GET`)
  e invalidaci�n autom�tica del cach� en escrituras (`POST`, `PUT`, `DELETE`),
  tanto para escenarios de DTO �nico como para escenarios **duplex**.
- Una **pol�tica de OutputCache por controlador** (`PerControllerOutputCachePolicy`)
  que etiqueta cada respuesta con un *tag* �nico por controlador, lo que permite
  invalidar de forma selectiva sin afectar a otros endpoints.
- Un **atributo personalizado** (`MlControllerCacheAttribute`) para declarar el
  cache de manera concisa, con duraci�n opcional.
- **Bypass din�mico del cach�** mediante el header `X-Bypass-Cache` o el
  est�ndar HTTP `Cache-Control: no-cache` / `no-store`.
- Un endpoint listo para uso (`GET clear-cache/now`) para invalidar el cach� del
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
- [Bypass din�mico del cach�](#bypass-din�mico-del-cach�)
- [Invalidaci�n manual](#invalidaci�n-manual)
- [Personalizaci�n avanzada](#personalizaci�n-avanzada)

---

## Dependencias

| Dependencia                                       | Tipo               |
|---------------------------------------------------|--------------------|
| `Microsoft.AspNetCore.App` (framework)            | `FrameworkReference` |
| `Microsoft.AspNetCore.OutputCaching` (incluido)   | Impl�cito ASP.NET 8  |
| `MoralesLarios.OOFP.AspNetCore.Controllers`               | `ProjectReference`   |

Target framework: **`net8.0`**.
La cach� se basa enteramente en `Microsoft.AspNetCore.OutputCaching`, no en el
viejo `ResponseCaching`.

---

## Registro en `Program.cs`

```csharp
using MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

// Registra la pol�tica "PerControllerTag" y los servicios de OutputCache.
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

> ?? **Importante**: `app.UseOutputCache()` debe colocarse **despu�s** de
> `UseRouting` y **antes** de `MapControllers`/`UseEndpoints` para que el
> middleware sepa qu� endpoint se est� sirviendo.

`AddWebControllersCache` recibe un `Action<OutputCacheOptions>` opcional para
configurar el `DefaultExpirationTimeSpan`, registrar otras pol�ticas, etc.

```csharp
public static IServiceCollection AddWebControllersCache(
    this IServiceCollection services,
    Action<OutputCacheOptions> options = null!)
```

Internamente registra la pol�tica con el nombre **`"PerControllerTag"`** apuntando
a `PerControllerOutputCachePolicy.Instance`.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching/
??? Controllers/
?   ??? SimpleMlCacheControllerBase.cs            // controlador base con cach� (PK simple)
?   ??? SimpleMlComplexCacheControllerBase.cs     // controlador base con cach� (PK compuesta)
??? Policies/
?   ??? PerControllerOutputCachePolicy.cs         // pol�tica IOutputCachePolicy
?   ??? MlControllerCacheAttribute.cs             // atributo IOutputCachePolicy
??? GlobalUsings.cs
??? RegisterServices.cs                            // AddWebControllersCache
??? README.md
```

---

## `PerControllerOutputCachePolicy`

Pol�tica de `OutputCache` (`IOutputCachePolicy`) que se aplica a cada request
cacheable y se encarga de:

1. Decidir si la request **debe ser cacheada** o **debe saltarse el cach�** seg�n
   los headers (`X-Bypass-Cache`, `Cache-Control`).
2. Etiquetar la respuesta con un *tag* `oc:{nombreDelController}` para poder
   invalidarla luego con `IOutputCacheStore.EvictByTagAsync`.

### API p�blica

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

Si el header `Cache-Control` contiene `no-cache` o `no-store` el bypass tambi�n
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
para indicar la duraci�n solo en ese endpoint.

### Firma

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class MlControllerCacheAttribute : Attribute, IOutputCachePolicy
{
    public MlControllerCacheAttribute();
    public MlControllerCacheAttribute(int durationSeconds);
}
```

- **Sin par�metros** ? utiliza `OutputCacheOptions.DefaultExpirationTimeSpan`
  (configurado en `AddWebControllersCache`).
- **`durationSeconds > 0`** ? fija `ResponseExpirationTimeSpan` solo para ese
  endpoint. Si se pasa `<= 0` se ignora y se usa el por defecto.

### Ejemplos

```csharp
public class PruebasController : SimpleMlCacheControllerBase<Prueba, PruebaDto, int>
{
    // 1. Duraci�n por defecto del proyecto
    [HttpGet("simple")]
    [MlControllerCache]
    public Task<IActionResult> Simple() => /* ... */;

    // 2. Duraci�n espec�fica de 30 segundos
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
(en `MoralesLarios.OOFP.AspNetCore.Controllers`) y a�ade:

- `[MlControllerCache]` en `GetAllAsync` y `GetByIdAsync`.
- Invalidaci�n autom�tica del *tag* del controlador en `POST`, `PUT`, `DELETE`
  llamando a `EvictControllerCacheAsync` antes de delegar en `base.*`.
- Endpoint p�blico `GET clear-cache/now` para vaciar el cach� manualmente.

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

Con esto el controlador gana autom�ticamente:

| Verbo  | Ruta                       | Comportamiento                                  |
|--------|----------------------------|-------------------------------------------------|
| GET    | `/api/Pruebas`             | Cacheado con tag `oc:Pruebas`                   |
| GET    | `/api/Pruebas/id-str/{id}` | Cacheado con tag `oc:Pruebas`                   |
| POST   | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el insert  |
| PUT    | `/api/Pruebas/{id}`        | Invalida `oc:Pruebas`, luego ejecuta el update  |
| PUT    | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el update  |
| DELETE | `/api/Pruebas/{id}`        | Invalida `oc:Pruebas`, luego ejecuta el delete  |
| DELETE | `/api/Pruebas`             | Invalida `oc:Pruebas`, luego ejecuta el delete  |
| GET    | `/api/Pruebas/clear-cache/now` | Vac�a manualmente el cach� del controlador  |

### Sobreescribir endpoints concretos

Puedes seguir a�adiendo endpoints personalizados sobre el mismo controlador:

```csharp
[HttpGet("with-cache1")]
[MlControllerCache(60)]    // 1 minuto solo aqu�
public async Task<IActionResult> GetWithCache1(CancellationToken ct = default)
    => await base.GetAllAsync(ct);
```

> ?? Devuelve directamente el `IActionResult` que produce el m�todo base; no lo
> envuelvas en `Ok(result)` porque ya es un `OkObjectResult` y se serializar�a
> como objeto en lugar de array.

---

## `SimpleMlCacheControllerBase<TEntity, TRequest, TResponse, TPk>`

Variante duplex de `SimpleMlCacheControllerBase<TEntity, TDto, TPk>` para escenarios donde la petici�n y la respuesta usan modelos distintos. Hereda de `SimpleMlControllerBase<TEntity, TRequest, TResponse, TPk>` y mantiene exactamente las mismas garant�as de cacheado e invalidaci�n.

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

### Cu�ndo usarlo

�salo cuando quieras cachear un controlador est�ndar pero la capa de escritura y la de lectura no compartan el mismo DTO.

---

## `SimpleMlComplexCacheControllerBase<TEntity, TDto>` (PK compuesta)

Variante de `SimpleMlCacheControllerBase<,,>` para entidades con **clave primaria compuesta**. Hereda de `SimpleMlComplexPkControllerBase<TEntity, TDto>` (en `MoralesLarios.OOFP.AspNetCore.Controllers`) y a�ade exactamente las mismas garant�as que `SimpleMlCacheControllerBase`:

- `[MlControllerCache]` en `GetAllAsync` y `GetByIdAsync`.
- Invalidaci�n autom�tica del *tag* del controlador en `POST`, `PUT`, `DELETE` antes de delegar en `base.*`.
- Endpoint p�blico `GET clear-cache/now` heredado para vaciar el cach� manualmente.

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

Endpoints disponibles autom�ticamente:

| Verbo  | Ruta                            | Comportamiento                                       |
|--------|---------------------------------|------------------------------------------------------|
| GET    | `/api/PruebaComplex`            | Cacheado con tag `oc:PruebaComplex`                  |
| GET    | `/api/PruebaComplex/id-str/{ids}` | Cacheado con tag `oc:PruebaComplex` (PK compuesta) |
| POST   | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el insert |
| PUT    | `/api/PruebaComplex/{ids}`      | Invalida `oc:PruebaComplex`, luego ejecuta el update |
| PUT    | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el update |
| DELETE | `/api/PruebaComplex/{ids}`      | Invalida `oc:PruebaComplex`, luego ejecuta el delete |
| DELETE | `/api/PruebaComplex`            | Invalida `oc:PruebaComplex`, luego ejecuta el delete |
| GET    | `/api/PruebaComplex/clear-cache/now` | Vac�a manualmente el cach� del controlador      |

Ver detalles del formato de `ids` (PK compuesta con `DateTime`, `DateOnly`, etc.) en el [README de `MoralesLarios.OOFP.AspNetCore.Controllers`](../MoralesLarios.OOFP.AspNetCore.Controllers/README.md).

---

## `SimpleMlComplexCacheControllerBase<TEntity, TRequest, TResponse>` (PK compuesta duplex)

Variante duplex de `SimpleMlComplexCacheControllerBase<TEntity, TDto>` para entidades con clave primaria compuesta cuando la petici�n y la respuesta usan modelos distintos. Hereda de `SimpleMlComplexPkControllerBase<TEntity, TRequest, TResponse>` y mantiene el mismo cacheado por controlador, invalidaci�n autom�tica y endpoint de borrado manual.

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

### Cu�ndo usarlo

�salo cuando combines PK compuesta, cach� por controlador y separaci�n entre modelos de lectura y escritura.

---

## Bypass din�mico del cach�

Cualquier cliente puede pedir que la respuesta NO se sirva desde cach� ni se
almacene, **solo para esa petici�n**, enviando uno de:

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

con lo que la request **no consulta** el almac�n ni **escribe** sobre �l.

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

## Invalidaci�n manual

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

Disponible autom�ticamente en todos los controladores que heredan de
`SimpleMlCacheControllerBase<,,>`.

---

## Personalizaci�n avanzada

### Cambiar el `DefaultExpirationTimeSpan`

```csharp
services.AddWebControllersCache(o =>
{
    o.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(30);
});
```

### A�adir m�s pol�ticas custom

```csharp
services.AddWebControllersCache(o =>
{
    o.AddPolicy("LongLived", policy => policy.Expire(TimeSpan.FromHours(1)));
});
```

### Mezclar pol�ticas en un mismo endpoint

```csharp
[HttpGet]
[MlControllerCache]
[OutputCache(PolicyName = "LongLived")]
public Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => /* ... */;
```

> Las pol�ticas se aplican en orden; la �ltima que escriba sobre el contexto
> manda. �salas con cuidado y prefiere una sola pol�tica por endpoint.

---

## Resumen

- Anotas `[MlControllerCache]` (con o sin segundos) ? cach� por controlador con
  tag e invalidaci�n selectiva.
- Heredas `SimpleMlCacheControllerBase<,,>` ? CRUD cacheado e invalidaci�n
  autom�tica gratis.
- Cliente env�a `X-Bypass-Cache: 1` o `Cache-Control: no-cache` ? respuesta
  fresca solo para esa request.
- `IOutputCacheStore.EvictByTagAsync("oc:NombreController", ct)` o
  `GET /clear-cache/now` ? flush manual.
