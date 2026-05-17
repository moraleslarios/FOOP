# MoralesLarios.OOFP.HttpClients

Cliente HTTP **funcional** integrado con `MlResult<T>` y `IHttpClientFactory`. Encapsula los patrones más habituales (CRUD JSON, paginación por headers, gestión de errores y de cabeceras) de forma componible y *railway-oriented*.

Permite:

- Llamar a APIs HTTP devolviendo siempre `MlResult<T>` (válido o fallido).
- Registrar clientes tipados con `IHttpClientFactory` mediante una sola extensión.
- Heredar de `GenClientFp<TDto>` (PK simple) o `GenComplexClientFp<TDto>` (PK **compuesta**) para tener listo todo el CRUD estándar.
- Componer llamadas custom con `IHttpClientFactoryManager` y *value-objects* (`Key`, `NotEmptyString`, `IntNotNegative`, etc.).
- Serializar automáticamente PKs compuestas (`DateTime`, `DateOnly`, `TimeOnly` con formato ISO 8601) cuando se llama por ID.

---

## Tabla de contenido

- [Dependencias](#dependencias)
- [Registro en `Program.cs`](#registro-en-programcs)
- [Estructura del proyecto](#estructura-del-proyecto)
- [`IHttpClientFactoryManager` / `HttpClientFactoryManager`](#ihttpclientfactorymanager--httpclientfactorymanager)
- [`GenClientFp<TDto>` / `IGenClientFp<TDto>`](#genclientfptdto--igenclientfptdto)
- [`GenComplexClientFp<TDto>` / `IGenComplexClientFp<TDto>` (PK compuesta)](#gencomplexclientfptdto--igencomplexclientfptdto-pk-compuesta)
- [Records de parámetros](#records-de-parámetros)
- [Helpers de cabeceras y respuestas](#helpers-de-cabeceras-y-respuestas)
- [Receta completa de uso](#receta-completa-de-uso)

---

## Dependencias

| Dependencia                                    | Tipo        |
|------------------------------------------------|-------------|
| `Microsoft.Extensions.Http` 9.x                | NuGet       |
| `MoralesLarios.OOFP` (FOOP)                    | Proyecto    |
| `MoralesLarios.OOFP.Extensions.Loggers`        | Proyecto    |
| `MoralesLarios.OOFP.Internals`                 | Proyecto    |
| `MoralesLarios.OOFP.Validation.Dataannotations`| Proyecto    |
| `MoralesLarios.OOFP.Validation`                | Proyecto    |
| `MoralesLarios.OOFP.ValueObjects`              | Proyecto    |

Target framework: **`net8.0`**.

---

## Registro en `Program.cs`

```csharp
using MoralesLarios.OOFP.HttpClients;

builder.Services.AddHttpClientsFp(); // registra IHttpClientFactoryManager

// Registro de clientes tipados con PK simple
builder.Services.AddGenClientFp<IPruebasClient, PruebasClient>(
    configureClient: client =>
    {
        client.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/");
    });

// Registro de clientes tipados con PK compuesta (heredan de GenComplexClientFp<TDto>)
builder.Services.AddGenClientComplexFp<IPruebaComplexClient, PruebaComplexClient, PruebaComplexDto>(
    configureClient: client =>
    {
        client.BaseAddress = new Uri("https://localhost:7197/api/PruebaComplex/");
    });
```

`AddGenClientFp<TService, TImplementation>` (PK simple):

- Registra automáticamente un `HttpClient` con nombre `typeof(TImplementation).Name`.
- Inyecta ese `Key` en el constructor de la implementación junto con el resto de dependencias resueltas por DI.
- Permite configurar el `HttpClient` (`BaseAddress`, headers comunes, etc.) y/o el `Key`.

`AddGenClientComplexFp<TService, TImplementation, TDto>` (PK compuesta):

- Hace lo mismo que `AddGenClientFp` **y además** registra automáticamente `IGenClientFp<TDto>` ? `GenClientFp<TDto>` para que `GenComplexClientFp<TDto>` pueda recibirlo por DI.
- Requiere el parámetro de tipo `TDto` adicional porque el contenedor DI no puede deducir el genérico de la cadena de herencia.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.HttpClients/
??? Helpers/
?   ??? MlHttpRequestExtensions.cs      // Cabeceras en HttpClient/HttpRequestMessage
?   ??? MlResponseWebExtensions.cs      // Lectura del cuerpo de error de HttpResponseMessage
??? ParamsInfo/
?   ??? CallRequestParamsInfo.cs        // record para llamadas no paginadas
?   ??? CallRequestPaginationParamsInfo.cs
??? GenClientFp.cs                       // Cliente genérico CRUD
??? IGenClientFp.cs
??? HttpClientFactoryManager.cs          // Wrapper funcional sobre IHttpClientFactory
??? IHttpClientFactoryManager.cs
??? RegisterServices.cs
??? GlobalUsings.cs
```

---

## `IHttpClientFactoryManager` / `HttpClientFactoryManager`

API funcional que centraliza las llamadas HTTP. Todas devuelven `MlResult<T>` y nunca lanzan excepciones de I/O al consumidor (las convierten en `fail`).

### Operaciones principales

```csharp
public interface IHttpClientFactoryManager
{
    Task<MlResult<T>>     GetAsync<T>(Key key, string url = "", Dictionary<string,string> headers = null!, CancellationToken ct = default);
    Task<MlResult<T>>     GetAsync<T>(CallRequestParamsInfo parameters, CancellationToken ct = default);
    Task<MlResult<T>>     GetPaginationAsync<T>(CallRequestPaginationParamsInfo parameters, CancellationToken ct = default);

    Task<MlResult<T>>     PostAsync<T>(Key key, T itemBody, string url = null!, Dictionary<string,string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TResult>> PostGetAsync<T, TResult>(Key key, T itemBody, string url = null!, Dictionary<string,string> headers = null!, CancellationToken ct = default);
    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>>
                          PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);

    Task<MlResult<Empty>> PutAsync<T>(Key key, T itemBody, string url = null!, Dictionary<string,string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteAsync<T>(Key key, T itemBody, string url = null!, Dictionary<string,string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync<T>(Key key, NotEmptyString url, Dictionary<string,string> headers = null!, CancellationToken ct = default);

    MlResult<HttpClient>  CreateHttpClient(Key key);
}
```

### Ejemplos directos

```csharp
public class PruebasClient(IHttpClientFactoryManager mgr, Key key) : IPruebasClient
{
    public Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data) =>
        mgr.GetAsync<PruebasDto>(key, "with-header",
            new Dictionary<string, string> { { "data", data } });

    public Task<MlResult<PruebasDto>> CreateAsync(PruebasDto dto) =>
        mgr.PostAsync(key, dto);

    public Task<MlResult<Empty>> DeleteOneAsync(NotEmptyString id) =>
        mgr.DeleteByIdAsync<PruebasDto>(key, id);
}
```

### Comportamiento por defecto

- Lectura de respuesta JSON con `System.Net.Http.Json` (`ReadFromJsonAsync`).
- Si el `HttpResponseMessage` **no** es `IsSuccessStatusCode`, se construye un `fail` con la descripción rica del cuerpo (vía `ToResponseErrorsDescription`).
- Cualquier excepción durante la llamada / deserialización se captura en el bind con `TryMapAsync` y se transforma en `fail` con detalles.

---

## `GenClientFp<TDto>` / `IGenClientFp<TDto>`

Cliente genérico CRUD que ya implementa los métodos típicos. Se utiliza como base para clientes tipados.

```csharp
public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>>      logger,
                               IHttpClientFactoryManager       mgr,
                               Key                             key) : IGenClientFp<TDto>
{
    public Task<MlResult<IEnumerable<TDto>>> GetAllAsync(...);
    public Task<MlResult<TDto>>              GetByIdAsync(NotEmptyString idStr, ...);
    public Task<MlResult<TDto>>              PostAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             PutAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             PutByIdAsync(NotEmptyString idStr, TDto itemBody, ...);
    public Task<MlResult<Empty>>             DeleteAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             DeleteByIdAsync(NotEmptyString idStr, ...);
}
```

Mapea contra los endpoints estándar de `SimpleMlControllerBase<,,>`:

| Método del cliente            | Verbo  | URL                  |
|-------------------------------|--------|----------------------|
| `GetAllAsync`                 | GET    | (BaseAddress)        |
| `GetByIdAsync(idStr)`         | GET    | `id-str/{idStr}`     |
| `PostAsync(dto)`              | POST   | (BaseAddress)        |
| `PutAsync(dto)`               | PUT    | (BaseAddress)        |
| `PutByIdAsync(idStr, dto)`    | PUT    | `{idStr}`            |
| `DeleteAsync(dto)`            | DELETE | (BaseAddress)        |
| `DeleteByIdAsync(idStr)`      | DELETE | `{idStr}`            |

### Cliente personalizado heredando de `GenClientFp<TDto>`

```csharp
public interface IPruebasClient : IGenClientFp<PruebasDto>
{
    Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data);
}

public class PruebasClient(ILogger<PruebasClient>    logger,
                           IHttpClientFactoryManager mgr,
                           Key                       key)
    : GenClientFp<PruebasDto>(logger, mgr, key), IPruebasClient
{
    public Task<MlResult<PruebasDto>> MyGetAsync(NotEmptyString data) =>
        mgr.GetAsync<PruebasDto>(key, "with-header",
            new Dictionary<string, string> { { "data", data } });
}
```

Y se registra:

```csharp
services.AddHttpClientsFp();
services.AddGenClientFp<IPruebasClient, PruebasClient>(
    configureClient: c => c.BaseAddress = new Uri("https://api.example.com/api/Pruebas/"));
```

---

## `GenComplexClientFp<TDto>` / `IGenComplexClientFp<TDto>` (PK compuesta)

Cliente genérico CRUD para entidades con **clave primaria compuesta** (más de un campo, o tipos no triviales como `DateTime`/`DateOnly`/`TimeOnly`). Mapea contra los endpoints de `SimpleMlComplexPkControllerBase<,>` del proyecto `MoralesLarios.OOFP.WebControllers`.

A diferencia de `GenClientFp<TDto>`, los métodos `*ByIdAsync` reciben `params object[] pk` y los serializan internamente a un string separado por comas, aplicando automáticamente el formato correcto para tipos temporales:

```csharp
public class GenComplexClientFp<TDto>(ILogger<GenComplexClientFp<TDto>> logger,
                                      IGenClientFp<TDto>                genClientFp) : IGenComplexClientFp<TDto>
{
    public Task<MlResult<IEnumerable<TDto>>> GetAllAsync(...);
    public Task<MlResult<TDto>>              GetByIdAsync(..., params object[] pk);
    public Task<MlResult<TDto>>              GetByIdAsync(object[] pk, ...);     // sobrecarga ergonómica
    public Task<MlResult<TDto>>              PostAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             PutAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             PutByIdAsync(TDto itemBody, ..., params object[] pk);
    public Task<MlResult<Empty>>             PutByIdAsync(object[] pk, TDto itemBody, ...);
    public Task<MlResult<Empty>>             DeleteAsync(TDto itemBody, ...);
    public Task<MlResult<Empty>>             DeleteByIdAsync(..., params object[] pk);
    public Task<MlResult<Empty>>             DeleteByIdAsync(object[] pk, ...);

    protected virtual string GetPkValuesString(object[] pkValues);  // override para customizar el formato
}
```

### Formato automático de PKs temporales

El método `GetPkValuesString(object[] pkValues)` aplica este pattern matching:

| Tipo .NET     | Formato resultante           | Ejemplo                       |
|---------------|------------------------------|-------------------------------|
| `DateTime`    | `yyyy-MM-ddTHH:mm:ss.fff`    | `2026-05-16T07:34:29.239`     |
| `DateOnly`    | `yyyy-MM-dd`                 | `2026-05-16`                  |
| `TimeOnly`    | `HH:mm:ss.fff`               | `07:34:29.239`                |
| Otros         | `ToString()` por defecto     | `42`, `"abc"`, `Guid`, etc.   |

Es `virtual protected`, por lo que cualquier cliente concreto puede sobrescribirlo para cambiar el formato.

### Cliente personalizado heredando de `GenComplexClientFp<TDto>`

```csharp
public interface IPruebaComplexClient : IGenComplexClientFp<PruebaComplexDto> { }

public class PruebaComplexClient(ILogger<PruebaComplexClient>      logger,
                                  IGenClientFp<PruebaComplexDto>    genClientFp)
    : GenComplexClientFp<PruebaComplexDto>(logger, genClientFp), IPruebaComplexClient { }
```

Y se registra con `AddGenClientComplexFp<TService, TImplementation, TDto>`:

```csharp
services.AddHttpClientsFp();
services.AddGenClientComplexFp<IPruebaComplexClient, PruebaComplexClient, PruebaComplexDto>(
    configureClient: c => c.BaseAddress = new Uri("https://api.example.com/api/PruebaComplex/"));
```

El registro hace **tres** cosas en un solo paso:

1. Añade un `HttpClient` nombrado con la `Key` derivada del tipo `TImplementation`.
2. Registra `IGenClientFp<TDto>` ? `GenClientFp<TDto>` con esa misma `Key` (requerido por el constructor primario de `GenComplexClientFp<TDto>`).
3. Registra `TService` ? `TImplementation` para inyectarlo en consumidores.

### Mapeo a endpoints del controlador con PK compuesta

| Método del cliente                | Verbo  | URL                       | Endpoint del servidor             |
|-----------------------------------|--------|---------------------------|------------------------------------|
| `GetAllAsync()`                   | GET    | (BaseAddress)             | `GET  /`                          |
| `GetByIdAsync(pk: ...)`           | GET    | `id-str/{pkValues}`       | `GET  /id-str/{ids}`              |
| `PostAsync(dto)`                  | POST   | (BaseAddress)             | `POST /`                          |
| `PutAsync(dto)`                   | PUT    | (BaseAddress)             | `PUT  /`                          |
| `PutByIdAsync(dto, pk: ...)`      | PUT    | `{pkValues}`              | `PUT  /{ids}`                     |
| `DeleteAsync(dto)`                | DELETE | (BaseAddress)             | `DELETE /`                        |
| `DeleteByIdAsync(pk: ...)`        | DELETE | `{pkValues}`              | `DELETE /{ids}`                   |

### Llamada típica

```csharp
// PK compuesta: (Nombre, Lugar, Precio, Fecha)
var dto = await _client.GetByIdAsync(
    pk: new object[]
    {
        "Prueba",
        "Lugar de prueba",
        100,
        new DateTime(2020, 1, 1)
    });
// URL final: GET id-str/Prueba,Lugar de prueba,100,2020-01-01T00:00:00.000
```

---

## Records de parámetros

Para escenarios donde quieres pasar todos los parámetros como un único objeto.

```csharp
public record CallRequestParamsInfo(
    string                      Url,
    Key                         HttpClientFactoryKey,
    Dictionary<string, string>? Headers           = null!,
    CancellationToken           CancellationToken = default);

public record CallRequestParamsInfo<TRequest>(
    string                      Url,
    Key                         HttpClientFactoryKey,
    [property: Required] TRequest RequestBody,
    Dictionary<string, string>? Headers           = null!,
    CancellationToken           CancellationToken = default);

public record CallRequestPaginationParamsInfo(
    string                      Url,
    Key                         HttpClientFactoryKey,
    IntNotNegative              PageNumber,
    IntNotNegative              PageSize,
    Dictionary<string, string>? Headers           = null!,
    CancellationToken           CancellationToken = default)
    : CallRequestParamsInfo(...);

public record CallRequestPaginationParamsInfo<TRequest>(...)
    : CallRequestParamsInfo<TRequest>(...);
```

Todos los records incluyen `implicit operator` desde tuplas para facilitar la construcción:

```csharp
CallRequestPaginationParamsInfo p = ("with-cache1", key, 0, 50, null, default);
```

Ejemplo de uso paginado:

```csharp
var parameters = new CallRequestPaginationParamsInfo("page", key, 0, 25);
var page = await mgr.GetPaginationAsync<PaginationResultInfo<PruebasDto>>(parameters);
```

---

## Helpers de cabeceras y respuestas

### `MlHttpRequestExtensions`

Extensiones para añadir headers a `HttpClient` y `HttpRequestMessage`, con valor *funcional* (`MlResult<>`).

```csharp
client.SetHeaderInfo("X-Custom", "value");
client.SetHeaderInfoAsInt("X-Page-Number", 1);
client.SetHeaderPageInfo(pageNumber, pageSize);

request.SetHeaderInfo("X-Custom", "value");
request.SetHeaders(new Dictionary<string, string> { { "k", "v" } });
```

Headers de paginación estándar:

| Constante       | Header           |
|-----------------|------------------|
| `pageNumber`    | `X-Page-Number`  |
| `pageSize`      | `X-Page-Size`    |

### `MlResponseWebExtensions`

Construye una descripción legible del error cuando una respuesta HTTP no es exitosa:

```csharp
public static Task<string> ToResponseErrorsDescriptionAsync(this HttpResponseMessage response);
public static string       ToResponseErrorsDescription(this HttpResponseMessage response);
```

Internamente lee el cuerpo, intenta indentarlo si es JSON y produce un mensaje con `Status`, `ReasonPhrase` y detalle. Esa descripción es la que va al `MlResult<>.Fail` cuando una llamada devuelve 4xx/5xx.

---

## Receta completa de uso

```csharp
// 1. Registro
services.AddHttpClientsFp();
services.AddGenClientFp<IPruebasClient, PruebasClient>(
    configureClient: c => c.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/"));

// 2. Inyección
public class MyService(IPruebasClient _client)
{
    public Task<MlResult<IEnumerable<PruebasDto>>> AllAsync()
        => _client.GetAllAsync();
}

// 3. Bypass de cache servidor (header reconocido por MoralesLarios.OOFP.WebControllers.Cache)
var bypass = new Dictionary<string, string> { { "X-Bypass-Cache", "1" } };
var fresh  = await _client.GetAllAsync(bypass);
```
