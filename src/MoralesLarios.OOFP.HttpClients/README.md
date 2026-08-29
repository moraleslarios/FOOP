# MoralesLarios.OOFP.HttpClients — consumo de APIs REST sin `try/catch` y sin excepciones

`MoralesLarios.OOFP.HttpClients` es la **capa cliente** de la solución: convierte cualquier llamada HTTP
(`GET`, `POST`, `PUT`, `DELETE`, paginada o no) en un **`MlResult<T>`**. Nunca lanza excepciones al llamador,
nunca devuelve `null` silencioso y nunca obliga a comprobar `IsSuccessStatusCode` a mano: todo el ruido
(creación del `HttpClient`, serialización JSON, cabeceras, comprobación de estado, deserialización, logging)
queda encapsulado dentro de una tubería ferroviaria (*railway oriented programming*).

Está construido sobre `IHttpClientFactory` (por lo que hereda su gestión de pool de conexiones y su
configuración por nombre) y es el **espejo exacto** de `MoralesLarios.OOFP.WebControllers`: si el servidor
expone sus CRUD heredando de `SimpleMlControllerBase<TEntity, TDto, TPk>`, el cliente los consume heredando
de `GenClientFp<TDto>` sin escribir una sola línea de infraestructura.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Arquitectura: las tres capas de cliente](#arquitectura-las-tres-capas-de-cliente)
5. [`IHttpClientFactoryManager` — la capa de transporte](#ihttpclientfactorymanager--la-capa-de-transporte)
6. [`GenClientFp<TDto>` — cliente CRUD de clave simple](#genclientfptdto--cliente-crud-de-clave-simple)
7. [`GenComplexClientFp<TDto>` — cliente CRUD de clave compuesta](#gencomplexclientfptdto--cliente-crud-de-clave-compuesta)
8. [Records de parámetros: `CallRequestParamsInfo` y paginación](#records-de-parámetros-callrequestparamsinfo-y-paginación)
9. [Helpers de cabeceras y de respuestas](#helpers-de-cabeceras-y-de-respuestas)
10. [`RegisterServices` — registro en el contenedor](#registerservices--registro-en-el-contenedor)
11. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
12. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
13. [Ejemplos prácticos](#ejemplos-prácticos)
14. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
15. [Mejores prácticas](#mejores-prácticas)
16. [Resumen](#resumen)
17. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

Consumir una API REST con `HttpClient` "a pelo" obliga a repetir siempre el mismo ritual: crear el cliente,
serializar, añadir cabeceras, enviar, comprobar el código de estado, leer el cuerpo de error, deserializar,
capturar `HttpRequestException`, `TaskCanceledException`, `JsonException`… y decidir qué devolver.

### ❌ Sin la librería

```csharp
public class PruebasClient(IHttpClientFactory factory, ILogger<PruebasClient> logger)
{
    public async Task<IEnumerable<PruebasDto>?> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var client = factory.CreateClient("PruebasClient");

            logger.LogInformation("Consultando {Url}", client.BaseAddress);

            var response = await client.GetAsync(string.Empty, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("Error {Code}: {Body}", (int)response.StatusCode, body);
                return null;                       // ← el llamador no sabe qué pasó
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<PruebasDto>>(cancellationToken: ct);
        }
        catch (HttpRequestException ex) { logger.LogError(ex, "Red"); return null; }
        catch (JsonException        ex) { logger.LogError(ex, "JSON"); return null; }
        catch (TaskCanceledException)   { return null; }
    }
    // … y ahora repite esto para GetById, Post, Put, Delete, DeleteById …
}
```

Problemas: 35 líneas por verbo, `null` como valor de error (sin motivo, sin código HTTP, sin cuerpo),
logging duplicado en cada método y cinco `catch` que se olvidan a la primera.

### ✅ Con la librería

```csharp
public interface IPruebasClient : IGenClientFp<PruebasDto> { }

public class PruebasClient(ILogger<GenClientFp<PruebasDto>> logger,
                           IHttpClientFactoryManager        manager,
                           Key                              key)
    : GenClientFp<PruebasDto>(logger, manager, key), IPruebasClient { }
```

Eso es **todo**. Con esas cuatro líneas ya tienes `GetAllAsync`, `GetByIdAsync`, `PostAsync`, `PutAsync`,
`PutByIdAsync`, `DeleteAsync` y `DeleteByIdAsync`, todos devolviendo `MlResult<…>`, todos con logging de
entrada/salida y todos con la descripción completa del error del servidor (código, razón y cuerpo JSON
indentado) dentro de `MlErrorsDetails`.

> 💡 **La idea clave:** el error de red o de negocio deja de ser una excepción o un `null` y pasa a ser
> **un valor de primera clase** que puedes encadenar con `Bind`, `Map`, `TryMap` o `ExecSelf` igual que el
> resultado feliz. El `if (response.IsSuccessStatusCode)` desaparece del código de aplicación.

---

## Instalación y dependencias

```xml
<TargetFramework>net8.0</TargetFramework>
<Version>1.0.14</Version>

<PackageReference Include="Microsoft.Extensions.Http" Version="9.0.6" />

<ProjectReference Include="..\MoralesLarios.FOOP\MoralesLarios.OOFP.csproj" />
<ProjectReference Include="..\MoralesLarios.OOFP.Extensions.Loggers\MoralesLarios.OOFP.Extensions.Loggers.csproj" />
<ProjectReference Include="..\MoralesLarios.OOFP.Internals\MoralesLarios.OOFP.Internals.csproj" />
<ProjectReference Include="..\MoralesLarios.OOFP.Validation.Dataannotations\MoralesLarios.OOFP.Validation.Dataannotations.csproj" />
<ProjectReference Include="..\MoralesLarios.OOFP.Validation\MoralesLarios.OOFP.Validation.csproj" />
<ProjectReference Include="..\MoralesLarios.OOFP.ValueObjects\MoralesLarios.OOFP.ValueObjects.csproj" />
```

| Dependencia | Para qué se usa aquí |
|---|---|
| `Microsoft.Extensions.Http` | `IHttpClientFactory`, `AddHttpClient(name, configure)` y el pool de `HttpMessageHandler`. |
| `MoralesLarios.OOFP` (núcleo) | `MlResult<T>`, `Bind/Map/TryMap/ExecSelf`, `EnsureFp`, `Empty`. |
| `MoralesLarios.OOFP.Extensions.Loggers` | `LogMlResultInformationAsync`, `MyMethodFinalLogAsync` (logging ferroviario). |
| `MoralesLarios.OOFP.Internals` | `PaginationResultInfo<T>` (resultado paginado). |
| `MoralesLarios.OOFP.ValueObjects` | `Key` (nombre del cliente), `Name`, `NotEmptyString`, `IntNotNegative`. |
| `MoralesLarios.OOFP.Validation.Dataannotations` | `DataannotationsValidator.ValidateAsync(parameters)` sobre los records de parámetros. |

> ⚠️ Ten en cuenta que `Microsoft.Extensions.Http` está en la versión **9.0.6** mientras el
> `TargetFramework` es **net8.0**. Funciona (el paquete es compatible hacia atrás), pero conviene alinear
> ambos: o subes a `net9.0` o bajas el paquete a `8.0.*`.

Los `global using` del proyecto (`GlobalUsings.cs`) ya incluyen `MoralesLarios.OOFP.Types`,
`MoralesLarios.OOFP.Helpers`, `…ValueObjects`, `…HttpClients.ParamsInfo` y `…HttpClients.Helpers`,
así que dentro de este proyecto no necesitas `using` explícitos.

---

## Estructura del proyecto

```text
MoralesLarios.OOFP.HttpClients/
├── IHttpClientFactoryManager.cs      ← contrato de transporte (9 métodos)
├── HttpClientFactoryManager.cs       ← implementación real: tuberías MlResult sobre IHttpClientFactory
├── IGenClientFp.cs                   ← contrato CRUD simplex + duplex (8 miembros cada uno)
├── GenClientFp.cs                    ← CRUD de clave simple (2 clases: simplex y duplex)
├── IGenComplexClientFp.cs            ← contrato CRUD con clave compuesta (10 miembros cada uno)
├── GenComplexClientFp.cs             ← CRUD de clave compuesta (2 clases: simplex y duplex)
├── ParamsInfo/
│   ├── CallRequestParamsInfo.cs      ← record de parámetros (+ variante genérica con RequestBody)
│   └── CallRequestPaginationParamsInfo.cs  ← añade PageNumber y PageSize
├── Helpers/
│   ├── MlHttpRequestExtensions.cs    ← cabeceras sobre HttpClient y HttpRequestMessage
│   └── MlResponseWebExtensions.cs    ← descripción legible del error de una HttpResponseMessage
├── RegisterServices.cs               ← AddHttpClientsFp + 3 helpers de registro
└── GlobalUsings.cs
```

---

## Arquitectura: las tres capas de cliente

El proyecto ofrece **tres niveles de abstracción**. Elige el más alto que te sirva y baja solo cuando lo necesites.

```text
┌───────────────────────────────────────────────────────────────────────────────┐
│ Nivel 3 — GenComplexClientFp<TDto> / <TRequest, TResponse>                    │
│   CRUD con CLAVE COMPUESTA. Recibe params object[] pk, lo formatea a          │
│   "v1,v2,v3" y delega en el nivel 2.                                          │
└───────────────────────────────┬───────────────────────────────────────────────┘
                                │  IGenClientFp<TDto>
┌───────────────────────────────▼───────────────────────────────────────────────┐
│ Nivel 2 — GenClientFp<TDto> / <TRequest, TResponse>                           │
│   CRUD con CLAVE SIMPLE. Traduce cada verbo a una URL relativa                │
│   ("", "id-str/{id}", "{id}") y delega en el nivel 1.                         │
└───────────────────────────────┬───────────────────────────────────────────────┘
                                │  IHttpClientFactoryManager
┌───────────────────────────────▼───────────────────────────────────────────────┐
│ Nivel 1 — HttpClientFactoryManager                                            │
│   TRANSPORTE. Crea el HttpClient, serializa, pone cabeceras, envía,           │
│   valida el status, deserializa y loguea. Devuelve MlResult<T>.               │
└───────────────────────────────┬───────────────────────────────────────────────┘
                                │  IHttpClientFactory (Microsoft.Extensions.Http)
                                ▼
                        Red / API REST remota
```

Y esta es la tubería real que ejecuta **cada** llamada del nivel 1 (extraída de
`HttpClientFactoryManager.ActionPostAsync<T>`):

```csharp
await MlResult.EmptyAsync()
        .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
        .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo post a la url …"))
        .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
        .BindAsync           (response => response.IsSuccessStatusCode
                                            ? response.ToMlResultValid()
                                            : response.ToResponseErrorsDescription()
                                                      .ToMlResultFail<HttpResponseMessage>())
        .TryMapAsync         (response => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct))
                .MyMethodFinalLogAsync(_logger, $"{nameof(PostAsync)}<{typeof(T).Name}");
```

Lee la cadena de arriba abajo: **cada eslabón solo se ejecuta si el anterior fue válido**. Un fallo de red
en `SendAsync` lo captura `TryMapAsync` y lo convierte en `MlResult` fallido con la excepción en los
`Details`; un 404 del servidor lo convierte `BindAsync` en un fallo con el cuerpo del error como mensaje;
un JSON malformado lo captura el último `TryMapAsync`. **Ningún camino sale por una excepción.**

---

## `IHttpClientFactoryManager` — la capa de transporte

Es la única clase que toca `HttpClient` directamente. Regístrala con `services.AddHttpClientsFp()`
(`AddTransient<IHttpClientFactoryManager, HttpClientFactoryManager>()`).

### Contrato real

```csharp
public interface IHttpClientFactoryManager
{
    MlResult<HttpClient> CreateHttpClient(Key httpClientFactoryKey);

    Task<MlResult<T>>     GetAsync       <T>(Key httpClientFactoryKey, string url = "",
                                             Dictionary<string, string> headers = null,
                                             CancellationToken ct = default);

    Task<MlResult<T>>     PostAsync      <T>(Key httpClientFactoryKey, T itemBody, string url = null!,
                                             Dictionary<string, string> headers = null!,
                                             CancellationToken ct = default);

    Task<MlResult<K>>     PostAsync   <T, K>(Key httpClientFactoryKey, T itemBody, string url = null,
                                             Dictionary<string, string> headers = null,
                                             CancellationToken ct = default);

    Task<MlResult<TResult>> PostGetAsync<T, TResult>(Key httpClientFactoryKey, T itemBody,
                                                     string url = null!,
                                                     Dictionary<string, string> headers = null!,
                                                     CancellationToken ct = default);

    Task<MlResult<PaginationResultInfo<TEnumrableResponse>>>
        PostGetPaginationAsync<TRequest, TEnumrableResponse>(
            CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default);

    Task<MlResult<Empty>> PutAsync       <T>(Key httpClientFactoryKey, T itemBody, string url = null!,
                                             Dictionary<string, string> headers = null!,
                                             CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteAsync    <T>(Key httpClientFactoryKey, T itemBody, string url = null!,
                                             Dictionary<string, string> headers = null!,
                                             CancellationToken ct = default);
    Task<MlResult<Empty>> DeleteByIdAsync<T>(Key httpClientFactoryKey, NotEmptyString url, Dictionary<string, string> headers = null!,
                                             CancellationToken ct = default);
}
```

| Miembro | Verbo HTTP | Cuerpo enviado | Devuelve | Notas |
|---|---|---|---|---|
| `CreateHttpClient(key)` | — | — | `MlResult<HttpClient>` | Único método **sincrónico**. `EnsureFp.NotNull(key)` + `TryMap(CreateClient)`. |
| `GetAsync<T>(key, url, headers, ct)` | `GET` | ninguno | `MlResult<T>` | Construye un `HttpRequestMessage`, aplica cabeceras y deserializa a `T`. |
| `PostAsync<T>(key, item, url, …)` | `POST` | `T` | `MlResult<T>` | Envía y **espera del servidor el mismo tipo** `T`. |
| `PostAsync<T, K>(key, item, url, …)` | `POST` | `T` | `MlResult<K>` | Variante *duplex*: envía `T`, recibe `K`. |
| `PostGetAsync<T, TResult>(key, item, url, …)` | `POST` | `T` | `MlResult<TResult>` | Idéntico a `PostAsync<T, K>`; nombre alternativo para consultas que viajan en el cuerpo. |
| `PostGetPaginationAsync<TRequest, TEnumrableResponse>(parameters, ct)` | `POST` | `TRequest` | `MlResult<PaginationResultInfo<…>>` | Valida el record con *Data Annotations*, añade `X-Page-Number`/`X-Page-Size` y deserializa el sobre paginado. |
| `PutAsync<T>(key, item, url, …)` | `PUT` | `T` | `MlResult<Empty>` | No lee el cuerpo de respuesta: solo confirma éxito. |
| `DeleteAsync<T>(key, item, url, …)` | `DELETE` | `T` | `MlResult<Empty>` | ⚠️ `DELETE` **con cuerpo**: muchos proxies lo descartan. |
| `DeleteByIdAsync<T>(key, url, …)` | `DELETE` | ninguno | `MlResult<Empty>` | La `url` es un `NotEmptyString`, así que un id vacío falla antes de salir a la red. El genérico `T` **solo se usa para el texto del log**. |

### Miembros públicos que están en la clase pero **no** en la interfaz

`HttpClientFactoryManager` expone dos sobrecargas adicionales que **no forman parte de
`IHttpClientFactoryManager`**, por lo que son invisibles si resuelves el servicio por la interfaz:

```csharp
public virtual Task<MlResult<T>> GetAsync<T>(CallRequestParamsInfo parameters, CancellationToken ct = default);
public virtual Task<MlResult<T>> GetPaginationAsync<T>(CallRequestPaginationParamsInfo parameters, CancellationToken ct = default);
```

Para usarlas tienes que inyectar la **clase concreta** (`HttpClientFactoryManager`) o extender la interfaz.

### Cómo se transforma un error del servidor

Cuando `IsSuccessStatusCode` es `false`, el pipeline llama a `ToResponseErrorsDescription()`
(`Helpers/MlResponseWebExtensions.cs`), que produce un mensaje con **tres datos**:

```text
Se ha producido un error en la llamada al servicio.
                    Código de error: 404
                    Razón: Not Found
                    Detalle error: {
  "type": "https://www.puntonetalpunto.net/",
  "title": "Elemento no encontrado",
  "status": 404,
  "detail": "No existe el elemento con la clave 42"
}
```

Si el cuerpo empieza por `{`, se reserializa **indentado** con `JsonDocument` para que sea legible en el log.
Si la lectura o el *parse* fallan, el detalle pasa a ser `"[No se pudo leer el contenido del error]"` —
nunca se propaga una excepción por culpa del formateo del error.

> ⚠️ **El código HTTP se pierde como dato estructurado.** Llega solo *dentro del texto* del mensaje.
> No hay ningún `Details["StatusCode"]`, así que el llamador **no puede distinguir programáticamente**
> un 404 de un 500 sin hacer *string matching*. Si necesitas esa distinción, envuelve la llamada y añade
> tú la clave con `MlErrorsDetails.AddDetail`.

---

## `GenClientFp<TDto>` — cliente CRUD de clave simple

Dos clases hermanas con la misma forma: **simplex** (un solo DTO para ida y vuelta) y **duplex**
(`TRequest` para enviar, `TResponse` para recibir).

```csharp
public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>> _logger,
                               IHttpClientFactoryManager  _httpClientFactoryManager,
                               Key                        _httpClientFactoryKey) : IGenClientFp<TDto>

public class GenClientFp<TRequest, TResponse>(ILogger<GenClientFp<TRequest, TResponse>> _logger,
                                              IHttpClientFactoryManager                 _httpClientFactoryManager,
                                              Key                                       _httpClientFactoryKey)
    : IGenClientFp<TRequest, TResponse>
```

### Contrato `IGenClientFp<TDto>`

```csharp
public interface IGenClientFp<TDto>
{
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync   (Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>>              GetByIdAsync  (NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<TDto>>              PostAsync     (TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>>             PutAsync      (TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>>             PutByIdAsync  (NotEmptyString idStr, TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>>             DeleteAsync   (TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    Task<MlResult<Empty>>             DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    IHttpClientFactoryManager         GetIHttpClientFactoryManager();
}
```

La versión *duplex* `IGenClientFp<TRequest, TResponse>` es idéntica cambiando
`GetAllAsync → IEnumerable<TResponse>`, `GetByIdAsync → TResponse` y `PostAsync(TRequest) → TResponse`.

### URL relativa que genera cada método

Todos los métodos son `virtual` y son **delegaciones de una línea** al `IHttpClientFactoryManager`.
Esta tabla es la parte que de verdad importa, porque define el contrato de rutas con el servidor:

| Método del cliente | Llamada al manager | URL relativa enviada | Petición completa (`BaseAddress = https://host/api/Pruebas/`) |
|---|---|---|---|
| `GetAllAsync(headers, ct)` | `GetAsync<IEnumerable<TDto>>(key, string.Empty, …)` | `""` | `GET https://host/api/Pruebas/` |
| `GetByIdAsync(idStr, …)` | `GetAsync<TDto>(key, $"id-str/{idStr}", …)` | `id-str/42` | `GET https://host/api/Pruebas/id-str/42` |
| `PostAsync(itemBody, …)` | `PostAsync(key, itemBody, string.Empty, …)` | `""` | `POST https://host/api/Pruebas/` |
| `PutAsync(itemBody, …)` | `PutAsync(key, itemBody, string.Empty, …)` | `""` | `PUT https://host/api/Pruebas/` |
| `PutByIdAsync(idStr, itemBody, …)` | `PutAsync(key, itemBody, idStr, …)` | `42` | `PUT https://host/api/Pruebas/42` |
| `DeleteAsync(itemBody, …)` | `DeleteAsync(key, itemBody, string.Empty, …)` | `""` | `DELETE https://host/api/Pruebas/` (con cuerpo) |
| `DeleteByIdAsync(idStr, …)` | `DeleteByIdAsync<TDto>(key, $"{idStr}", …)` | `42` | `DELETE https://host/api/Pruebas/42` |

> ⚠️ **Asimetría deliberada pero peligrosa:** `GetByIdAsync` añade el prefijo **`id-str/`** y
> `PutByIdAsync` / `DeleteByIdAsync` **no**. Esto encaja exactamente con las rutas que generan los
> controladores de `MoralesLarios.OOFP.WebControllers` (`[HttpGet("id-str/{id}")]` frente a
> `[HttpPut("{id}")]` y `[HttpDelete("{ids}")]`), pero si tu API no sigue ese convenio tendrás que
> sobrescribir los métodos.

### Miembros de infraestructura

```csharp
public IHttpClientFactoryManager GetIHttpClientFactoryManager();   // en la interfaz
public Key                       GetHttpClientFactoryKey();        // ⚠️ NO está en la interfaz
```

`GetIHttpClientFactoryManager()` es la vía oficial para bajar al nivel 1 desde un cliente derivado
(por ejemplo para llamar a un endpoint que no es CRUD). `GetHttpClientFactoryKey()` existe en la clase
pero **no en `IGenClientFp<>`**, así que solo es accesible desde dentro de la jerarquía o con la clase concreta.

### Cliente personalizado: el patrón recomendado

```csharp
// 1) Interfaz de tu dominio, que hereda el CRUD completo
public interface IPruebasClient : IGenClientFp<PruebasDto>
{
    Task<MlResult<IEnumerable<PruebasDto>>> GetActivasAsync(CancellationToken ct = default);
}

// 2) Implementación: hereda la infraestructura, añade lo tuyo
public class PruebasClient(ILogger<GenClientFp<PruebasDto>> logger,
                           IHttpClientFactoryManager        manager,
                           Key                              key)
    : GenClientFp<PruebasDto>(logger, manager, key), IPruebasClient
{
    // Endpoint que no es CRUD: bajamos al nivel 1 con el manager heredado
    public Task<MlResult<IEnumerable<PruebasDto>>> GetActivasAsync(CancellationToken ct = default)
        => GetIHttpClientFactoryManager()
               .GetAsync<IEnumerable<PruebasDto>>(GetHttpClientFactoryKey(), "activas", null!, ct);

    // Sobrescribimos un verbo para adaptar la ruta al servidor real
    public override Task<MlResult<PruebasDto>> GetByIdAsync(NotEmptyString idStr,
                                                            Dictionary<string, string> headers = null!,
                                                            CancellationToken ct = default)
        => GetIHttpClientFactoryManager()
               .GetAsync<PruebasDto>(GetHttpClientFactoryKey(), $"{idStr}", headers, ct);
}
```

> ⚠️ El parámetro `Key key` **no lo puede resolver el contenedor por sí solo**: por eso el registro se
> hace con `ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClientFactoryKey)`
> (ver [`RegisterServices`](#registerservices--registro-en-el-contenedor)).

---

## `GenComplexClientFp<TDto>` — cliente CRUD de clave compuesta

Cuando la entidad tiene **clave primaria compuesta** (por ejemplo `(int Id, DateTime Fecha)`), no puedes
pasar un solo `idStr`. `GenComplexClientFp<TDto>` resuelve eso: recibe `params object[] pk`, lo convierte
en una cadena `"v1,v2"` con formato estable y delega en un `IGenClientFp<TDto>` inyectado.

```csharp
public class GenComplexClientFp<TDto>(ILogger<GenComplexClientFp<TDto>> _logger,
                                      IGenClientFp<TDto>                _genClientFp)
    : IGenComplexClientFp<TDto>

public class GenComplexClientFp<TRequest, TResponse>(
        ILogger<GenComplexClientFp<TRequest, TResponse>> _logger,
        IGenClientFp<TRequest, TResponse>                _genClientFp)
    : IGenComplexClientFp<TRequest, TResponse>
```

**Es composición, no herencia:** el cliente complejo *usa* un cliente simple. Eso significa que el
`IGenClientFp<TDto>` también debe estar registrado (lo hace `AddGenClientComplexFp<…>` por ti).

### Contrato `IGenComplexClientFp<TDto>` (10 miembros)

```csharp
Task<MlResult<IEnumerable<TDto>>> GetAllAsync   (Dictionary<string,string> headers = null!, CancellationToken ct = default);

Task<MlResult<TDto>>  GetByIdAsync   (Dictionary<string,string> headers = null!, CancellationToken ct = default, params object[] pk);
Task<MlResult<TDto>>  GetByIdAsync   (object[] pk, Dictionary<string,string> headers = null!, CancellationToken ct = default);

Task<MlResult<TDto>>  PostAsync      (TDto itemBody, Dictionary<string,string> headers = null!, CancellationToken ct = default);
Task<MlResult<Empty>> PutAsync       (TDto itemBody, Dictionary<string,string> headers = null!, CancellationToken ct = default);

Task<MlResult<Empty>> PutByIdAsync   (TDto itemBody, Dictionary<string,string> headers = null!, CancellationToken ct = default, params object[] pk);
Task<MlResult<Empty>> PutByIdAsync   (object[] pk, TDto itemBody, Dictionary<string,string> headers = null!, CancellationToken ct = default);

Task<MlResult<Empty>> DeleteAsync    (TDto itemBody, Dictionary<string,string> headers = null!, CancellationToken ct = default);
Task<MlResult<Empty>> DeleteByIdAsync(Dictionary<string,string> headers = null!, CancellationToken ct = default, params object[] pk);
Task<MlResult<Empty>> DeleteByIdAsync(object[] pk, Dictionary<string,string> headers = null!, CancellationToken ct = default);
```

Cada operación "por id" viene **duplicada** con dos formas de pasar la clave:

| Forma | Firma | Cuándo usarla |
|---|---|---|
| `params` al final | `GetByIdAsync(headers, ct, params object[] pk)` | Cuando ya vas a especificar cabeceras y token: `GetByIdAsync(null!, ct, 42, fecha)`. |
| Array al principio | `GetByIdAsync(object[] pk, headers, ct)` | **La más legible** y la que evita ambigüedades: `GetByIdAsync([42, fecha])`. |

> 💡 **Recomendación:** usa siempre la sobrecarga que recibe el `object[]` en primera posición. La variante
> con `params` al final obliga a rellenar `headers` y `ct` explícitamente y, si la llamas con un único
> `object[]`, el compilador puede elegir la otra sobrecarga sin que te des cuenta.

### Formateo de la clave compuesta

El método protegido `GetPkValuesString` (idéntico en las dos clases) es el corazón de la traducción:

```csharp
protected virtual string GetPkValuesString(object[] pkValues)
    => string.Join(",", pkValues.Select(v => v switch
    {
        DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
        DateOnly d  => d.ToString("yyyy-MM-dd"),
        TimeOnly t  => t.ToString("HH:mm:ss.fff"),
        _           => v.ToString()
    }));
```

| Tipo de la parte de PK | Formato generado | Ejemplo |
|---|---|---|
| `DateTime` | `yyyy-MM-ddTHH:mm:ss.fff` | `2024-03-15T00:00:00.000` |
| `DateOnly` | `yyyy-MM-dd` | `2024-03-15` |
| `TimeOnly` | `HH:mm:ss.fff` | `14:30:00.000` |
| Cualquier otro | `v.ToString()` (cultura actual) | `42`, `ABC` |

El resultado se pasa por `NotEmptyString.ByString(...)`, de modo que una clave vacía produce un
`MlResult` fallido **sin salir a la red**, y después se envía como un único segmento de URL.

> ⚠️ **Tres limitaciones reales:**
> 1. El separador es la **coma**, igual que el `ids.Split(',')` del servidor. Si alguna parte de la clave
>    es un `string` que contiene comas, el servidor la partirá mal.
> 2. Un elemento `null` dentro de `pk` provoca **`NullReferenceException`** en `v.ToString()`
>    (el `switch` no tiene rama `null`).
> 3. Los tipos "otros" se formatean con la **cultura del hilo**: un `decimal` puede viajar como `1,5`
>    (coma decimal) y romper el `Convert.ChangeType` del servidor, que usa `InvariantCulture`.

### Cliente personalizado con clave compuesta

```csharp
public interface IReservasClient : IGenComplexClientFp<ReservaDto> { }

public class ReservasClient(ILogger<GenComplexClientFp<ReservaDto>> logger,
                            IGenClientFp<ReservaDto>                inner)
    : GenComplexClientFp<ReservaDto>(logger, inner), IReservasClient { }
```

Uso:

```csharp
// PK = (int SalaId, DateOnly Dia)
var reserva = await _reservasClient.GetByIdAsync([7, new DateOnly(2024, 3, 15)]);
// → GET https://host/api/Reservas/id-str/7,2024-03-15
```

---

## Records de parámetros: `CallRequestParamsInfo` y paginación

Cuando una llamada necesita muchos datos (url + clave + cuerpo + cabeceras + token + página + tamaño),
pasarlos como parámetros posicionales es frágil. El proyecto ofrece cuatro `record` inmutables con
**conversiones implícitas desde tuplas**.

```csharp
public record CallRequestParamsInfo(string                      Url,
                                    Key                         HttpClientFactoryKey,
                                    Dictionary<string, string>? Headers           = null!,
                                    CancellationToken           CancellationToken = default);

public record CallRequestParamsInfo<TRequest>(
                         string                      Url,
                         Key                         HttpClientFactoryKey,
    [property: Required] TRequest                    RequestBody,
                         Dictionary<string, string>? Headers           = null!,
                         CancellationToken           CancellationToken = default);

public record CallRequestPaginationParamsInfo(string Url, Key HttpClientFactoryKey,
                                              IntNotNegative PageNumber, IntNotNegative PageSize,
                                              Dictionary<string, string>? Headers = null!,
                                              CancellationToken CancellationToken = default)
    : CallRequestParamsInfo(Url, HttpClientFactoryKey, Headers, CancellationToken);

public record CallRequestPaginationParamsInfo<TRequest>(
                         string Url, Key HttpClientFactoryKey,
    [property: Required] TRequest RequestBody,
                         IntNotNegative PageNumber, IntNotNegative PageSize,
                         Dictionary<string, string>? Headers = null!,
                         CancellationToken CancellationToken = default)
    : CallRequestParamsInfo<TRequest>(Url, HttpClientFactoryKey, RequestBody, Headers, CancellationToken);
```

| Record | Hereda de | Añade | Se usa en |
|---|---|---|---|
| `CallRequestParamsInfo` | — | `Url`, `HttpClientFactoryKey`, `Headers`, `CancellationToken` | `HttpClientFactoryManager.GetAsync<T>(parameters, ct)`. |
| `CallRequestParamsInfo<TRequest>` | — | `RequestBody` con `[Required]` | `InternalPostGetAsync` (protegido). |
| `CallRequestPaginationParamsInfo` | `CallRequestParamsInfo` | `PageNumber`, `PageSize` | `GetPaginationAsync<T>(parameters, ct)`. |
| `CallRequestPaginationParamsInfo<TRequest>` | `CallRequestParamsInfo<TRequest>` | `PageNumber`, `PageSize` | `PostGetPaginationAsync<TRequest, TEnumrableResponse>`. |

### Construcción desde tupla (conversión implícita)

Cada record define un `implicit operator` desde la tupla con **todos** sus miembros, así que puedes escribir:

```csharp
// Con nombre de tipo explícito
var p1 = new CallRequestPaginationParamsInfo("pruebas", "PruebasClient", 1, 20, null, default);

// Con tupla: el compilador aplica la conversión implícita
CallRequestPaginationParamsInfo p2 = ("pruebas", "PruebasClient", 1, 20, null, CancellationToken.None);

var page = await _manager.GetPaginationAsync<IEnumerable<PruebasDto>>(p2);
```

> 💡 `Key`, `IntNotNegative` y `NotEmptyString` son *value objects* con conversiones implícitas desde
> `string`/`int`, de ahí que `"PruebasClient"` y `1` funcionen directamente. Consulta
> [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md).

### Validación automática

`GetPaginationAsync` y `PostGetPaginationAsync` **empiezan** validando el record:

```csharp
await DataannotationsValidator.ValidateAsync(parameters)
        .TryMapAsync(_ => _httpClientFactory.CreateClient(parameters.HttpClientFactoryKey))
        …
```

Gracias al `[property: Required]` sobre `RequestBody`, un cuerpo nulo produce un `MlResult` fallido
**antes** de crear el `HttpClient`. Puedes añadir tus propias anotaciones heredando del record.

---

## Helpers de cabeceras y de respuestas

### `MlHttpRequestExtensions` (namespace `MoralesLarios.OOFP.HttpClients.Helpers`)

Extensiones que devuelven `MlResult<…>` en lugar de lanzar. Todas tienen su gemela `…Async`
(que es simplemente `.ToAsync()`, es decir `Task.FromResult`, **no** una operación asíncrona real).

**Sobre `HttpClient`:**

```csharp
MlResult<HttpClient> SetHeaderInfo     (this HttpClient client, Name headerKey, string headerValue);
MlResult<HttpClient> SetHeaderInfoAsInt(this HttpClient client, Name headerKey, int    headerValue);
MlResult<HttpClient> SetHeaderPageNumber(this HttpClient client, IntNotNegative pageNumber); // "X-Page-Number"
MlResult<HttpClient> SetHeaderPageSize  (this HttpClient client, IntNotNegative pageSize);   // "X-Page-Size"
MlResult<HttpClient> SetHeaderPageInfo  (this HttpClient client, IntNotNegative pageNumber, IntNotNegative pageSize);
```

**Sobre `HttpRequestMessage`:**

```csharp
MlResult<HttpRequestMessage> SetHeaderInfo(this HttpRequestMessage request, Name headerKey, NotEmptyString headerValue);
MlResult<HttpRequestMessage> SetHeaders   (this HttpRequestMessage request, Dictionary<string, string> headerKeyValues);
```

| Extensión | Cabecera / efecto | Detalle de implementación |
|---|---|---|
| `SetHeaderInfo` (client) | `DefaultRequestHeaders.Add(key, value)` | `EnsureFp.NotNull(client)` → `TryExecSelfIfValid` → `Map(_ => client)`. |
| `SetHeaderInfoAsInt` | Igual, con `value.ToString()` | Base de las dos siguientes. |
| `SetHeaderPageNumber` | **`X-Page-Number`** | Constante literal, no expuesta como `const`. |
| `SetHeaderPageSize` | **`X-Page-Size`** | Idem. |
| `SetHeaderPageInfo` | ambas, encadenadas con `Bind` | Si la primera falla, la segunda no se ejecuta. |
| `SetHeaderInfo` (request) | `request.Headers.Add(…)` | Valida `request` **y** `headerValue`. |
| `SetHeaders` (request) | recorre el diccionario | Es el que usa el `HttpClientFactoryManager` en cada verbo, siempre con `headers ?? []`. |

> ⚠️ `HttpHeaders.Add` **lanza** si la cabecera ya existe; `TryExecSelfIfValid` la captura y la convierte
> en `MlResult` fallido, así que llamar dos veces a `SetHeaderPageInfo` sobre el mismo `HttpClient`
> devuelve un fallo en vez de sobrescribir. Usa un `HttpClient` recién creado por llamada (que es lo que
> hace el manager) o `TryAddWithoutValidation` en tu propio código.
>
> ⚠️ Las cabeceras `X-Page-Number` y `X-Page-Size` son exactamente las que lee
> `MlRequestWebExtensions.GetHeaderPaginationInfo()` en [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md).
> Cliente y servidor están acoplados por esos dos literales, duplicados en ambos proyectos.

### `MlResponseWebExtensions`

```csharp
string       ToResponseErrorsDescription     (this HttpResponseMessage response);
Task<string> ToResponseErrorsDescriptionAsync(this HttpResponseMessage response);
```

Ambas devuelven `string.Empty` si la respuesta fue correcta y, si no, el bloque de tres líneas
(código, razón, detalle) descrito [más arriba](#cómo-se-transforma-un-error-del-servidor).

> ⚠️ La versión **sincrónica** hace `response.Content.ReadAsStringAsync().Result`, y es precisamente la que
> usan todas las tuberías del `HttpClientFactoryManager`. Es un bloqueo de hilo dentro de un método `async`:
> en un servidor con carga puede agotar el *thread pool*. Lo correcto sería usar
> `ToResponseErrorsDescriptionAsync` con `BindAsync`.

---

## `RegisterServices` — registro en el contenedor

```csharp
public static IServiceCollection AddHttpClientsFp(this IServiceCollection services);

public static IServiceCollection AddGenClientFp<TService, TImplementation>(
        this IServiceCollection services,
        Action<Key>        configureHttpClientKey = null!,
        Action<HttpClient> configureClient        = null!)
    where TService : class where TImplementation : class, TService;

public static IServiceCollection AddGenClientComplexFp<TService, TImplementation, TDto>(…)
    where TService : class where TImplementation : class, TService where TDto : class;

public static IServiceCollection AddGenClientDuplexComplexFp<TService, TImplementation, TRequest, TResponse>(…)
    where TService : class where TImplementation : class, TService
    where TRequest : class where TResponse : class;
```

| Método | Qué registra | Cuándo usarlo |
|---|---|---|
| `AddHttpClientsFp()` | `IHttpClientFactoryManager → HttpClientFactoryManager` (**Transient**) | **Siempre**, una sola vez. Es el prerrequisito de todo lo demás. |
| `AddGenClientFp<TService, TImpl>()` | `AddHttpClient(nombre, configureClient)` + `TService → TImpl` (Transient) con la `Key` inyectada | Cliente de **clave simple** (derivado de `GenClientFp<…>`). |
| `AddGenClientComplexFp<TService, TImpl, TDto>()` | Lo anterior **más** `IGenClientFp<TDto> → GenClientFp<TDto>` | Cliente de **clave compuesta simplex** (derivado de `GenComplexClientFp<TDto>`). |
| `AddGenClientDuplexComplexFp<TService, TImpl, TRequest, TResponse>()` | Idem con `IGenClientFp<TRequest, TResponse>` | Cliente de **clave compuesta duplex**. |

Registro típico en `Program.cs`:

```csharp
builder.Services.AddHttpClientsFp();

builder.Services.AddGenClientFp<IPruebasClient, PruebasClient>(
    configureClient: c =>
    {
        c.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/");
        c.Timeout     = TimeSpan.FromSeconds(30);
    });

builder.Services.AddGenClientComplexFp<IReservasClient, ReservasClient, ReservaDto>(
    configureClient: c => c.BaseAddress = new Uri("https://localhost:7197/api/Reservas/"));
```

> ⚠️⚠️ **NO uses el parámetro `configureHttpClientKey`.** El código es:
>
> ```csharp
> Key httpClientFactoryKey = null!;
> if (configureHttpClientKey is not null) configureHttpClientKey(httpClientFactoryKey);  // ← recibe null y no asigna nada
> else httpClientFactoryKey = Key.FromString(typeof(TImplementation).Name!);
> ```
>
> Al ser `Key` un tipo de referencia pasado **por valor**, el `Action<Key>` no puede devolver nada: si
> pasas `configureHttpClientKey`, la clave se queda en `null` y el `AddHttpClient(null, …)` posterior
> revienta. **Usa siempre solo `configureClient`** y deja que la clave se derive del nombre de la
> implementación (`nameof(PruebasClient)`), que es el comportamiento correcto de la rama `else`.

> ⚠️ La barra final del `BaseAddress` **es obligatoria**. `new Uri("https://host/api/Pruebas")` (sin `/`)
> hace que `HttpClient` sustituya el último segmento, así que `GET id-str/42` iría a
> `https://host/api/id-str/42`. Escribe siempre `".../api/Pruebas/"`.

---

## ⚠️ Particularidades reales del código fuente

Estas son observaciones verificadas leyendo la implementación. Consúltalas antes de depurar algo "raro".

1. **`configureHttpClientKey` no funciona** en los tres helpers de registro (ver el bloque anterior).
   Es el problema más grave del proyecto.
2. **El `ILogger<>` inyectado en `GenClientFp<…>` y en `GenComplexClientFp<…>` nunca se usa.** El logging
   real ocurre una capa más abajo, en `HttpClientFactoryManager` (que sí usa su logger).
3. **`GetByIdAsync` usa el prefijo `id-str/` pero `PutByIdAsync` y `DeleteByIdAsync` no.** Encaja con
   `WebControllers`, pero es una asimetría que sorprende.
4. **El `DeleteByIdAsync` duplex llama a `DeleteByIdAsync<TResponse>` mientras el simplex llama a
   `DeleteByIdAsync<TDto>`.** El genérico solo se usa para el texto del log, así que es inocuo, pero es
   incoherente.
5. **`GetHttpClientFactoryKey()` no está en `IGenClientFp<>`**: si resuelves por la interfaz no lo verás.
6. **`GetAsync<T>(CallRequestParamsInfo)` y `GetPaginationAsync<T>(CallRequestPaginationParamsInfo)`
   tampoco están en `IHttpClientFactoryManager`.** Son públicos en la clase concreta, inaccesibles por DI.
7. **El código HTTP de error se pierde como dato estructurado**: viaja dentro del texto del mensaje.
   No hay 404 vs 500 distinguible sin *string matching*.
8. **`ToResponseErrorsDescription()` (síncrono) usa `.Result`** dentro de tuberías `async`: bloqueo de hilo.
9. **Ni `HttpResponseMessage` ni `HttpRequestMessage` se liberan.** En `InternalHttpActionAsync` hay una
   línea `//.ExecSelfIfValidAsync(response => response.Dispose())` comentada. En .NET moderno el
   `HttpClient` cierra el socket, pero el patrón correcto sería `using`.
10. **Serialización asimétrica:** al enviar se usa `JsonSerializer.Serialize(itemBody)` con las opciones
    **por defecto** (nombres en `PascalCase`), y al recibir `ReadFromJsonAsync<T>()` con las opciones
    **web** (`camelCase`, insensible a mayúsculas). Funciona por casualidad, porque la lectura es
    insensible; pero si el servidor espera `camelCase` estricto, el `POST`/`PUT` fallará. No hay forma de
    inyectar un `JsonSerializerOptions`.
11. **`Path.Combine` para construir las URL de los mensajes de log.** En Windows produce
    `https://host\pruebas`. Solo afecta a los logs, no a la petición real, pero ensucia la traza.
12. **Erratas en nombres públicos:** el genérico `TEnumrableResponse` (falta la `e` de *Enumerable*) en
    `PostGetPaginationAsync`, el genérico `K` en vez de `TResult` en `PostAsync<T, K>`, y la palabra
    `BaseAdress` en los comentarios de la interfaz pública.
13. **Anotaciones de nulabilidad inconsistentes**: unos parámetros usan `= null!` y otros `= null`
    (`GetAsync<T>`, `PostAsync<T, K>`), lo que genera avisos distintos según el método.
14. **Sobrecarga `PostGetAsync` comentada** dentro de `IHttpClientFactoryManager` (la que recibía
    `CallRequestParamsInfo<TRequest>`): código muerto en un archivo de contrato público.
15. **`PostGetAsync<T, TResult>` y `PostAsync<T, K>` son idénticos**: ambos serializan, hacen `POST` y
    deserializan a otro tipo. Dos nombres para la misma operación.
16. **`GetPkValuesString` no contempla `null`** (`NullReferenceException`) y formatea los tipos no
    temporales con la **cultura del hilo**.
17. **La coma como separador de PK compuesta** rompe si una parte de la clave es un `string` con comas.
18. **`SetHeaders` valida `request` pero el mensaje dice `nameof(headerKeyValues)`**, y el diccionario en sí
    no se valida (aunque el manager siempre pasa `headers ?? []`, así que en la práctica no falla).
19. **Mezcla de idiomas en los mensajes**: los de `EnsureFp` en el manager están en español
    (`"itemBody no puede ser nulo"`) y los de los helpers de cabeceras en inglés
    (`"client cannot be null if we want to set information in the header. "`, con espacio final).
20. **`Microsoft.Extensions.Http` 9.0.6 sobre `net8.0`**: desalineación de versiones.

---

## ⚠️ Lo que NO incluye

- **Sin reintentos, *circuit breaker* ni *timeout* por política.** No hay integración con Polly ni con
  `AddStandardResilienceHandler`. Puedes añadirla tú encadenando sobre el `AddHttpClient` que hace
  `AddGenClientFp` (el nombre del cliente es `nameof(TImplementation)`).
- **Sin autenticación.** No hay soporte de `Bearer`, API key ni refresco de token: pásalos a mano en el
  diccionario `headers` o registra un `DelegatingHandler`.
- **Sin `PATCH`.** Solo `GET`, `POST`, `PUT` y `DELETE`.
- **Sin control del `JsonSerializerOptions`.** Ni `camelCase` configurable, ni conversores personalizados,
  ni `ReferenceHandler`.
- **Sin `IAsyncEnumerable` ni *streaming*.** Todo se deserializa en memoria completo.
- **Sin caché de cliente.** El *bypass* de caché (`X-Bypass-Cache`) solo tiene sentido contra un servidor
  que use `MoralesLarios.OOFP.WebControllers.Cache`; aquí es simplemente una cabecera más.
- **Sin `GetAllPaginationAsync` en `IGenClientFp`.** La paginación solo está en el nivel 1
  (`GetPaginationAsync` / `PostGetPaginationAsync`), no en los clientes CRUD generados.
- **Sin *tests* propios** en la solución para este proyecto (existen
  `MoralesLarios.OOFP.HttpClients.Tests.Unit` y `…Tests.Integration` como esqueletos).

---

## Ejemplos prácticos

### Ejemplo 1 — El cliente mínimo de principio a fin

```csharp
// ── DTO ────────────────────────────────────────────────────────────────────
public record PruebasDto(int Id, string Nombre, bool Activa);

// ── Contrato e implementación ─────────────────────────────────────────────
public interface IPruebasClient : IGenClientFp<PruebasDto> { }

public class PruebasClient(ILogger<GenClientFp<PruebasDto>> logger,
                           IHttpClientFactoryManager        manager,
                           Key                              key)
    : GenClientFp<PruebasDto>(logger, manager, key), IPruebasClient { }

// ── Registro ──────────────────────────────────────────────────────────────
builder.Services.AddHttpClientsFp();
builder.Services.AddGenClientFp<IPruebasClient, PruebasClient>(
    configureClient: c => c.BaseAddress = new Uri("https://localhost:7197/api/Pruebas/"));

// ── Consumo ───────────────────────────────────────────────────────────────
public class PruebasAppService(IPruebasClient _client)
{
    public Task<MlResult<IEnumerable<PruebasDto>>> TodasAsync(CancellationToken ct = default)
        => _client.GetAllAsync(ct: ct);
}
```

### Ejemplo 2 — Tratar el resultado con `Match` (nunca con `Value`)

```csharp
var resultado = await _client.GetByIdAsync("42");

string mensaje = resultado.Match(
    valid: dto   => $"Encontrada: {dto.Nombre}",
    fail : error => $"Error: {error.ToErrorsDescription()}");

Console.WriteLine(mensaje);
```

> ⚠️ **Regla de oro:** `Value` y `ErrorsDetails` son `internal protected`. Desde código de aplicación
> accede **siempre** con `Match(valid:…, fail:…)` o, si estás seguro de la validez, con
> `SecureValidValue()`. Ver [`MlResult`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md).

### Ejemplo 3 — Encadenar varias llamadas HTTP en una sola tubería

```csharp
public Task<MlResult<FacturaDto>> CrearFacturaCompletaAsync(int clienteId, LineaDto[] lineas)
    => _clientesClient.GetByIdAsync($"{clienteId}")
            .BindAsync(cliente  => _facturasClient.PostAsync(new FacturaDto(0, cliente.Id, lineas)))
            .BindAsync(factura  => _lineasClient.PostAsync(new LineasLoteDto(factura.Id, lineas))
                                                .MapAsync(_ => factura))
            .ExecSelfIfValidAsync(factura => _logger.LogMlResultInformationAsync(
                                                 $"Factura {factura.Id} creada correctamente"));
```

Si el cliente no existe (404), **nada más se ejecuta** y el error del primer `GET` llega intacto al final.

### Ejemplo 4 — Cabeceras personalizadas (autenticación y correlación)

```csharp
var headers = new Dictionary<string, string>
{
    ["Authorization"]  = $"Bearer {token}",
    ["X-Correlation-Id"] = Activity.Current?.Id ?? Guid.NewGuid().ToString(),
    ["Accept-Language"]  = "es-ES"
};

var dto = await _client.GetByIdAsync("42", headers);
```

Las cabeceras se aplican al `HttpRequestMessage` de **esa** petición (vía `SetHeadersAsync`), no al
`HttpClient`, así que no contaminan llamadas posteriores.

### Ejemplo 5 — Bypass de la caché del servidor

```csharp
// Reconocido por PerControllerOutputCachePolicy.BypassHeader en MoralesLarios.OOFP.WebControllers.Cache.
// Valores admitidos: "1", "true", "yes", "on", "no-cache", "no-store", "bypass".
private static readonly Dictionary<string, string> _sinCache = new() { ["X-Bypass-Cache"] = "true" };

public Task<MlResult<IEnumerable<PruebasDto>>> TodasFrescasAsync()
    => _client.GetAllAsync(_sinCache);
```

### Ejemplo 6 — CRUD completo con clave compuesta

```csharp
public record ReservaDto(int SalaId, DateOnly Dia, string Titulo);

public interface IReservasClient : IGenComplexClientFp<ReservaDto> { }

public class ReservasClient(ILogger<GenComplexClientFp<ReservaDto>> logger,
                            IGenClientFp<ReservaDto>                inner)
    : GenComplexClientFp<ReservaDto>(logger, inner), IReservasClient { }

// Registro (¡registra también el IGenClientFp<ReservaDto> interno!)
builder.Services.AddGenClientComplexFp<IReservasClient, ReservasClient, ReservaDto>(
    configureClient: c => c.BaseAddress = new Uri("https://localhost:7197/api/Reservas/"));

// Uso
object[] pk = [7, new DateOnly(2024, 3, 15)];

var leer     = await _reservas.GetByIdAsync(pk);                     // GET    .../id-str/7,2024-03-15
var crear    = await _reservas.PostAsync(new ReservaDto(7, new DateOnly(2024,3,16), "Retro"));
var modif    = await _reservas.PutByIdAsync(pk, crear.SecureValidValue());  // PUT    .../7,2024-03-15
var borrar   = await _reservas.DeleteByIdAsync(pk);                  // DELETE .../7,2024-03-15
```

### Ejemplo 7 — Cliente *duplex*: enviar `Request`, recibir `Response`

```csharp
public record CrearUsuarioRequest(string Nombre, string Email, string Password);
public record UsuarioResponse   (int Id, string Nombre, string Email, DateTime Alta);

public interface IUsuariosClient : IGenClientFp<CrearUsuarioRequest, UsuarioResponse> { }

public class UsuariosClient(ILogger<GenClientFp<CrearUsuarioRequest, UsuarioResponse>> logger,
                            IHttpClientFactoryManager manager,
                            Key                       key)
    : GenClientFp<CrearUsuarioRequest, UsuarioResponse>(logger, manager, key), IUsuariosClient { }

// El POST envía el Request y devuelve el Response, sin exponer nunca el password de vuelta
MlResult<UsuarioResponse> creado =
    await _usuarios.PostAsync(new CrearUsuarioRequest("Ana", "ana@x.com", "s3cr3t"));
```

### Ejemplo 8 — Paginación con el nivel 1

```csharp
public class PruebasPagedService(HttpClientFactoryManager _manager)   // ← clase concreta, no la interfaz
{
    public Task<MlResult<PaginationResultInfo<PruebasDto>>> PaginaAsync(int pagina, int tamano)
    {
        CallRequestPaginationParamsInfo parametros =
            ("pruebas/paged", "PruebasClient", pagina, tamano, null, CancellationToken.None);

        return _manager.GetPaginationAsync<PaginationResultInfo<PruebasDto>>(parametros);
    }
}
```

El manager añade `X-Page-Number` y `X-Page-Size` al `HttpClient` antes de llamar, exactamente las
cabeceras que lee `GetHeaderPaginationInfo()` en el servidor.

> ⚠️ Recuerda: `GetPaginationAsync` **no** está en `IHttpClientFactoryManager`, así que hay que inyectar
> `HttpClientFactoryManager`. Si prefieres depender de la interfaz, usa
> `PostGetPaginationAsync<TRequest, TEnumerableResponse>`, que sí está declarado.

### Ejemplo 9 — Consulta compleja que viaja en el cuerpo (`PostGetAsync`)

```csharp
public record FiltroPruebas(string? Nombre, bool? Activa, DateOnly? Desde, DateOnly? Hasta);

public class BusquedaService(IHttpClientFactoryManager _manager)
{
    public Task<MlResult<IEnumerable<PruebasDto>>> BuscarAsync(FiltroPruebas filtro,
                                                                CancellationToken ct = default)
        => _manager.PostGetAsync<FiltroPruebas, IEnumerable<PruebasDto>>(
               httpClientFactoryKey: "PruebasClient",
               itemBody:             filtro,
               url:                  "buscar",
               ct:                   ct);
}
```

Un filtro con demasiados campos no cabe cómodamente en la *query string*; `PostGetAsync` lo envía como
JSON en el cuerpo de un `POST` y deserializa la lista de resultados.

### Ejemplo 10 — Bajar al `HttpClient` crudo cuando algo no encaja

```csharp
public class DescargasClient(IHttpClientFactoryManager _manager)
{
    public async Task<MlResult<byte[]>> DescargarPdfAsync(int id, CancellationToken ct = default)
        => await _manager.CreateHttpClient("DescargasClient")                      // MlResult<HttpClient>
                         .TryMapAsync(client => client.GetByteArrayAsync($"pdf/{id}", ct));
}
```

`CreateHttpClient` es la puerta de escape: te devuelve el `HttpClient` **ya envuelto en `MlResult`**, y a
partir de ahí sigues en la tubería con `TryMapAsync` / `BindAsync` para lo que la librería no cubre
(descargas binarias, `multipart/form-data`, `PATCH`, *streaming*…).

### Ejemplo 11 — ❌ vs ✅: manejo de errores del lado cliente

```csharp
// ❌ MAL: reintroduce excepciones y pierde la información del error
public async Task<PruebasDto> MalAsync(int id)
{
    var r = await _client.GetByIdAsync($"{id}");
    if (r.IsFail) throw new Exception("No encontrado");   // se pierde código, razón y cuerpo
    return r.SecureValidValue();
}

// ❌ MAL: string matching sobre el mensaje para adivinar el código HTTP
public async Task<IActionResult> TambienMalAsync(int id)
{
    var r = await _client.GetByIdAsync($"{id}");
    if (r.IsFail && r.Match(_ => "", e => e.ToErrorsDescription()).Contains("404"))
        return NotFound();
    return Ok(r.SecureValidValue());
}
```

```csharp
// ✅ BIEN: el MlResult viaja intacto hasta la frontera y allí se traduce una sola vez
public Task<MlResult<PruebasDto>> BienAsync(int id)
    => _client.GetByIdAsync($"{id}")
              .ExecSelfIfFailAsync(error => _logger.LogMlResultFailAsync(error));

// ✅ BIEN: en el controlador, la traducción a HTTP la hace WebApi
[HttpGet("{id}")]
public Task<IActionResult> GetAsync(int id)
    => _appService.BienAsync(id)
                  .ToGetPdActionResultAsync();
```

La regla es siempre la misma: **el `MlResult` no se abre hasta el borde de la aplicación**. Dentro se
compone; en la frontera (controlador, UI, cola) se traduce con `Match` o con las extensiones de
[`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md).

---

## Tabla de decisión rápida

### ¿Qué nivel de abstracción uso?

| Tu escenario | Usa | Cómo |
|---|---|---|
| CRUD estándar con PK de un solo campo | `GenClientFp<TDto>` | Hereda + `AddGenClientFp<IX, X>()`. |
| CRUD con PK de un solo campo y DTOs distintos de ida/vuelta | `GenClientFp<TRequest, TResponse>` | Hereda + `AddGenClientFp<IX, X>()`. |
| CRUD con **PK compuesta** | `GenComplexClientFp<TDto>` | Hereda + `AddGenClientComplexFp<IX, X, TDto>()`. |
| CRUD con PK compuesta y DTOs distintos | `GenComplexClientFp<TRequest, TResponse>` | Hereda + `AddGenClientDuplexComplexFp<…>()`. |
| Endpoint suelto que no es CRUD | `IHttpClientFactoryManager` | Inyéctalo directo o usa `GetIHttpClientFactoryManager()`. |
| Consulta con muchos filtros | `PostGetAsync<TFiltro, TResultado>` | El filtro viaja como JSON en el cuerpo. |
| Listado paginado | `PostGetPaginationAsync<…>` (interfaz) o `GetPaginationAsync<T>` (clase) | Con `CallRequestPaginationParamsInfo`. |
| Descarga binaria, `PATCH`, `multipart`, *streaming* | `CreateHttpClient(key)` + `TryMapAsync` | Puerta de escape al `HttpClient` crudo. |

### ¿Qué método del manager llamo?

| Necesito | Método | Devuelve |
|---|---|---|
| Leer una colección o un elemento | `GetAsync<T>` | `MlResult<T>` |
| Crear y recuperar lo creado (mismo tipo) | `PostAsync<T>` | `MlResult<T>` |
| Crear y recuperar otro tipo | `PostAsync<T, K>` / `PostGetAsync<T, TResult>` | `MlResult<K>` |
| Actualizar sin leer respuesta | `PutAsync<T>` | `MlResult<Empty>` |
| Borrar enviando la entidad | `DeleteAsync<T>` | `MlResult<Empty>` |
| Borrar por id | `DeleteByIdAsync<T>` | `MlResult<Empty>` |
| Página de resultados | `PostGetPaginationAsync<TRequest, TEnumrableResponse>` | `MlResult<PaginationResultInfo<…>>` |
| El `HttpClient` en bruto | `CreateHttpClient` | `MlResult<HttpClient>` (**síncrono**) |

### ¿Cómo consumo el `MlResult` que me devuelven?

| Quiero | Operador del núcleo | Documentación |
|---|---|---|
| Transformar el valor válido | `Map` / `MapAsync` | [`Map`](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) |
| Encadenar otra operación que devuelve `MlResult` | `Bind` / `BindAsync` | [`Bind`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) |
| Transformar con código que puede lanzar | `TryMap` / `TryMapAsync` | [`Map`](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) |
| Salir a un tipo concreto (`string`, `IActionResult`…) | `Match` / `MatchAsync` | [`Match`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md) |
| Efecto lateral (log, métrica) sin romper la cadena | `ExecSelfIfValid` / `ExecSelfIfFail` | [`ExecSelf`](../MoralesLarios.FOOP/__Doc/ExecSelf/2_ExecSelfIfValid.md) |
| Validar antes de llamar | `EnsureFp.NotNull`, `EnsureFp.That` | [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) |
| Recorrer una colección haciendo N llamadas | `Projection*` / `FusionErrosIfExists` | [`Bucles`](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) |

---

## Mejores prácticas

1. **Registra siempre `AddHttpClientsFp()` primero.** Sin `IHttpClientFactoryManager` en el contenedor,
   los clientes generados no se pueden construir.

2. **Nunca pases `configureHttpClientKey`.** Está roto (ver particularidad 1). Usa solo `configureClient`
   y deja que la clave sea `nameof(TImplementation)`.

3. **Pon la barra final en el `BaseAddress`.** `"https://host/api/Pruebas/"`, no `".../Pruebas"`. Es la
   causa número uno de 404 inexplicables.

4. **Define una interfaz de dominio por cliente** (`IPruebasClient : IGenClientFp<PruebasDto>`) en lugar de
   inyectar `IGenClientFp<PruebasDto>`. Así añades métodos propios, puedes tener varios clientes del mismo
   DTO apuntando a hosts distintos y los mocks de los tests quedan con nombre claro.

5. **No abras el `MlResult` dentro de la capa de aplicación.** Compón con `Bind`/`Map` y traduce solo en la
   frontera; y cuando lo abras, hazlo con `Match`, nunca leyendo `Value`.

6. **Propaga siempre el `CancellationToken`.** Todos los métodos lo aceptan como último parámetro; si no lo
   pasas, una petición colgada seguirá viva después de que el usuario cancele.

7. **Sobrescribe el verbo, no reimplementes el cliente.** Si tu API no usa `id-str/{id}`, marca `override`
   sobre `GetByIdAsync` y llama al manager con la URL correcta: todos los verbos son `virtual`.

8. **Usa la sobrecarga `object[]` en los clientes de PK compuesta** (`GetByIdAsync([7, dia])`) en lugar de
   la variante con `params` al final: es la única que admite cabeceras y `CancellationToken` con claridad.

9. **Formatea tú los valores no temporales de una PK compuesta** con `InvariantCulture` antes de pasarlos
   (`decimal`, `double`, `float`), porque `GetPkValuesString` usa la cultura del hilo.

10. **No metas comas ni `null` en una PK compuesta.** Las comas rompen el `Split(',')` del servidor y el
    `null` provoca `NullReferenceException` en el formateo.

11. **Pasa las cabeceras por petición, no por `HttpClient`.** El diccionario `headers` de cada método se
    aplica al `HttpRequestMessage`; usar `SetHeaderInfo` sobre un `HttpClient` compartido acumula cabeceras
    y falla en la segunda llamada.

12. **Añade resiliencia por fuera.** Tras `AddGenClientFp<IX, X>()` puedes escribir
    `services.AddHttpClient(nameof(X)).AddStandardResilienceHandler()` para sumar reintentos, *timeout* y
    *circuit breaker* sin tocar la librería.

13. **La autenticación, en un `DelegatingHandler`.** Es más limpio que repetir el `Bearer` en cada
    diccionario de cabeceras y te permite refrescar el token en un solo sitio.

14. **Alinea la serialización con el servidor.** No puedes inyectar `JsonSerializerOptions`: si tu API exige
    `camelCase` estricto, decora los DTO con `[JsonPropertyName]`.

15. **Logea el fallo una sola vez, en la frontera**, con `ExecSelfIfFailAsync`. El manager ya deja traza de
    entrada y salida de cada llamada; repetirlo en cada capa multiplica el ruido.

16. **Si necesitas distinguir el código HTTP, envuelve la llamada.** El status solo llega dentro del texto
    del error; añade tú la clave con `AddDetail` en el punto de la petición si vas a decidir según 404 / 409.

17. **Para paginación desde una interfaz, usa `PostGetPaginationAsync`.** `GetPaginationAsync` obliga a
    depender de la clase concreta `HttpClientFactoryManager`.

18. **Un `Key` = un `BaseAddress`.** No reutilices el mismo nombre de cliente para dos hosts: la
    configuración de `IHttpClientFactory` es por nombre y la última gana.

---

## Resumen

`MoralesLarios.OOFP.HttpClients` es el lado cliente del enfoque ferroviario de la solución:

- **Tres niveles**: `HttpClientFactoryManager` (transporte), `GenClientFp<…>` (CRUD de clave simple) y
  `GenComplexClientFp<…>` (CRUD de clave compuesta), cada uno construido sobre el anterior.
- **Cero excepciones y cero `null`**: todo devuelve `MlResult<T>`; los fallos de red, de estado HTTP y de
  deserialización se convierten en errores componibles con código, razón y cuerpo del error.
- **Cuatro líneas por cliente**: heredar de `GenClientFp<TDto>` y llamar a `AddGenClientFp<IX, X>()` te da
  siete verbos CRUD con logging incluido.
- **Espejo de `WebControllers`**: las rutas que genera (`""`, `id-str/{id}`, `{id}`) encajan con las que
  publican los controladores base del lado servidor, y las cabeceras `X-Page-Number` / `X-Page-Size` con
  las que lee `WebApi`.
- **Puerta de escape siempre disponible**: `CreateHttpClient(key)` y `GetIHttpClientFactoryManager()` te
  devuelven al `HttpClient` crudo, ya envuelto en `MlResult`, para todo lo que la librería no cubre.
- **Con aristas conocidas**: `configureHttpClientKey` no funciona, el status HTTP no viaja estructurado, no
  hay control del JSON ni resiliencia integrada y `ToResponseErrorsDescription` bloquea con `.Result`.

---

## Ver también

### Navegación general

- [README de la solución](../README.md) — mapa de todos los proyectos.
- [Núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) — índice de la documentación completa.
- [Introducción a la programación ferroviaria](../MoralesLarios.FOOP/__Doc/1_Intro.md) — empieza aquí si es
  tu primer contacto con `MlResult`.

### Proyectos relacionados

- [`MoralesLarios.OOFP.WebControllers`](../MoralesLarios.OOFP.WebControllers/README.md) — el **servidor
  espejo** de este cliente: publica exactamente las rutas que aquí se consumen.
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — traducción de `MlResult` a
  `IActionResult` / `ProblemDetails` y lectura de `X-Page-Number` / `X-Page-Size`.
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — capa de servicio
  genérica del lado servidor.
- [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) — `PaginationResultInfo<T>` y
  `PaginationInfo`, el sobre de los resultados paginados.
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — `Key`, `Name`,
  `NotEmptyString` e `IntNotNegative`, los tipos que blindan los parámetros.
- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) —
  `DataannotationsValidator`, que valida los records de parámetros antes de salir a la red.
- [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) — el logging
  ferroviario que ves en las tuberías.

### Documentación del núcleo útil aquí

- [`MlResult`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md) — el tipo que devuelven todos los métodos.
- [`MlResult` y errores](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md) — `MlError`,
  `MlErrorsDetails`, `ToErrorsDescription()`, `GetDetailException()`.
- [`Bind`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) — encadenar llamadas HTTP dependientes.
- [`Map`](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) — transformar el DTO recibido.
- [`Match`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md) — cerrar la tubería en la frontera.
- [`ExecSelfIfValid`](../MoralesLarios.FOOP/__Doc/ExecSelf/2_ExecSelfIfValid.md) — logs y métricas sin
  romper la cadena.
- [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) — validaciones previas a la llamada.
- [Bucles y colecciones](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) — N peticiones y fusión de errores.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`,
> `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()` o
> `ToDetailsDescription()`.
