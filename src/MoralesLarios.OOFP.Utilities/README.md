# MoralesLarios.OOFP.Utilities — Configuración como `MlResult<T>`

Proyecto **muy pequeño y muy concreto**: una única abstracción, [`IMlConfigManager`](#imlconfigmanager), que envuelve `IConfiguration` para que **leer una clave de configuración devuelva un `MlResult<T>` en lugar de un `null` silencioso**.

Es la puerta de entrada de la configuración al raíl funcional: si la clave no existe, no obtienes `null` ni una excepción a los diez minutos, obtienes un `Fail` con un mensaje que dice exactamente qué clave falta.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Registro en el contenedor](#registro-en-el-contenedor)
5. [`IMlConfigManager`](#imlconfigmanager)
6. [Los tres métodos de lectura](#los-tres-métodos-de-lectura)
7. [Las dos formas de personalizar el error](#las-dos-formas-de-personalizar-el-error)
8. [Cómo funciona internamente](#cómo-funciona-internamente)
9. [Claves anidadas](#claves-anidadas)
10. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
11. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
12. [Ejemplos prácticos](#ejemplos-prácticos)
13. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
14. [Mejores prácticas](#mejores-prácticas)
15. [Resumen](#resumen)
16. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

`IConfiguration` es **silenciosamente permisivo**: si la clave no existe, devuelve `null` (o el `default` del tipo) sin avisar. El error aparece mucho más tarde, en un punto del código que no tiene nada que ver.

❌ **Con `IConfiguration` a pelo:**

```csharp
public class ServicioApi
{
    private readonly string _url;

    public ServicioApi(IConfiguration config)
    {
        _url = config["Api:Url"];          // ⚠️ si la clave no existe → null, sin error
    }

    public async Task<Respuesta> Llamar()
        => await _http.GetAsync(_url);     // 💥 ArgumentNullException aquí, lejos de la causa
}
```

El mensaje que verás será `Value cannot be null. (Parameter 'requestUri')`. **Nada indica que el problema real es una clave ausente en `appsettings.json`.**

✅ **Con `IMlConfigManager`:**

```csharp
public class ServicioApi(IMlConfigManager _config, HttpClient _http)
{
    public Task<MlResult<Respuesta>> Llamar()
        => _config.ReadAppSettingKey<string>("Api:Url")
                  .BindAsync(url => _http.GetMlAsync<Respuesta>(url));
}
```

Si la clave falta, el `Fail` dice: `No value found configured with the key 'Api:Url'`. **El error nace donde está la causa** y viaja por el raíl hasta donde se decida qué hacer con él.

> 💡 **La idea de fondo**: la configuración es una **entrada externa no fiable**, igual que un DTO de un cliente HTTP o el contenido de un fichero. Merece el mismo tratamiento: validación en el borde y `MlResult` hacia dentro.

---

## Instalación y dependencias

| Dependencia | Versión | Para qué |
|---|---|---|
| `Microsoft.Extensions.Configuration` | 9.0.2 | `ConfigurationBuilder` |
| `Microsoft.Extensions.Configuration.Abstractions` | 9.0.2 | `IConfiguration`, `GetConnectionString` |
| `Microsoft.Extensions.Configuration.Binder` | 9.0.2 | 🔑 `GetValue<T>()` — la conversión de tipos |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 9.0.2 | `IServiceCollection` |
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) | — | `MlResult<T>`, `EnsureFp`, `MapEnsure` |

Destino: **`net8.0`**. Versión del paquete: **1.0.2**. Genera paquete NuGet al compilar (`GeneratePackageOnBuild`).

```csharp
using MoralesLarios.OOFP.Utilities;          // RegisterServices
using MoralesLarios.OOFP.Utilities.Config;   // IMlConfigManager
```

> ⚠️ **Las dependencias son 9.0.2 pero el destino es `net8.0`.** Funciona (los paquetes 9.x soportan `net8.0`), pero si tu aplicación fija las de la versión 8.x tendrás que resolver la unificación de versiones.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Utilities/
├── GlobalUsings.cs
├── RegisterServices.cs              → AddMlUtilitiesConfig
└── Config/
    ├── IMlConfigManager.cs          → la abstracción (6 métodos)
    └── MlConfigManager.cs           → la implementación sobre IConfiguration
```

**Tres ficheros de código. Eso es todo el proyecto.**

> 💡 A pesar del nombre genérico "Utilities", **hoy solo contiene gestión de configuración**. El nombre sugiere un cajón de sastre para futuras utilidades transversales, pero no hay nada más.

---

## Registro en el contenedor

```csharp
public static IServiceCollection AddMlUtilitiesConfig(this IServiceCollection services,
                                                          IConfiguration      configuration)
{
    services.AddTransient<IMlConfigManager>(x => new MlConfigManager(configuration));
    return services;
}
```

En una aplicación ASP.NET Core:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMlUtilitiesConfig(builder.Configuration);   // 🔑
```

En un test con `Xunit.DependencyInjection` (así lo hace el proyecto de pruebas):

```csharp
public class Startup
{
    private readonly IConfiguration _configuration =
        new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();

    public void ConfigureServices(IServiceCollection services)
        => services.AddMlUtilitiesConfig(_configuration);
}
```

> ⚠️ **Se registra como `Transient`**, y la lambda **captura la `IConfiguration` que le pasas**, no la resuelve del contenedor. Si en tu aplicación recargas la configuración o registras una `IConfiguration` distinta después, `MlConfigManager` seguirá usando la que capturó aquí.

> 💡 Un `Singleton` habría sido más eficiente (la clase no tiene estado mutable), pero `Transient` es inofensivo: crear el objeto es trivial.

---

## `IMlConfigManager`

```csharp
public interface IMlConfigManager
{
    MlResult<T>      ReadAppSettingKey<T>    (string appSettingKey      , MlErrorsDetails errorsDetails = null!);
    MlResult<object> ReadAppSettingKey       (string appSettingKey      , MlErrorsDetails errorsDetails = null!);
    MlResult<string> ReadConnectionStringKey (string connectionStringKey, MlErrorsDetails errorsDetails = null!);

    MlResult<T>      ReadAppSettingKey<T>    (string appSettingKey      , string errorMessage);
    MlResult<object> ReadAppSettingKey       (string appSettingKey      , string errorMessage);
    MlResult<string> ReadConnectionStringKey (string connectionStringKey, string errorMessage);
}
```

**Tres operaciones × dos formas de describir el error = seis métodos.** No hay más superficie pública.

Implementación con constructor primario (C# 12):

```csharp
public class MlConfigManager(IConfiguration configuration) : IMlConfigManager
```

---

## Los tres métodos de lectura

| Método | Delega en | Devuelve |
|---|---|---|
| `ReadAppSettingKey<T>(clave)` | `configuration.GetValue<T>(clave)` | `MlResult<T>` — **con conversión de tipo** |
| `ReadAppSettingKey(clave)` | `configuration[clave]` | `MlResult<object>` — **el valor crudo (una `string`)** |
| `ReadConnectionStringKey(clave)` | `configuration.GetConnectionString(clave)` | `MlResult<string>` |

### `ReadAppSettingKey<T>` — con conversión

```csharp
MlResult<string> url      = _config.ReadAppSettingKey<string>("Api:Url");
MlResult<int>    timeout  = _config.ReadAppSettingKey<int>   ("Api:TimeoutSeconds");
MlResult<bool>   useProxy = _config.ReadAppSettingKey<bool>  ("Proxy:UseProxy");
```

Usa `GetValue<T>()` del `Binder`, que **convierte la cadena al tipo pedido**. `"true"` → `true`, `"30"` → `30`.

### `ReadAppSettingKey` (sin genérico) — el valor crudo

```csharp
MlResult<object> valor = _config.ReadAppSettingKey("SimpleKey");
```

Devuelve `MlResult<object>`, pero **el contenido real siempre es una `string`** (es lo que devuelve el indexador `configuration[clave]`). Tendrás que convertir tú.

> ⚠️ **Salvo que realmente necesites el valor sin convertir, usa la versión genérica.** `MlResult<object>` obliga a un cast en el consumidor y pierde toda la seguridad de tipos que aporta el resto de la biblioteca.

### `ReadConnectionStringKey` — cadenas de conexión

```csharp
MlResult<string> cs = _config.ReadConnectionStringKey("DemoDb");
```

Lee de la sección `ConnectionStrings` (`GetConnectionString` es azúcar para `configuration["ConnectionStrings:DemoDb"]`).

```jsonc
{
  "ConnectionStrings": {
    "DemoDb": "Data Source=.\\LOCAL;Initial Catalog=DemoDb;..."
  }
}
```

> 💡 **Combínalo con [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md)**: leer la cadena de conexión en el raíl funcional y encadenar el registro del `DbContext` es un uso natural.

---

## Las dos formas de personalizar el error

Cada uno de los tres métodos tiene dos sobrecargas.

### Con `MlErrorsDetails` — control total

```csharp
var errores = MlErrorsDetails.FromErrorMessage("Falta la URL de la API en la configuración")
                             .AddDetail("clave"  , "Api:Url")
                             .AddDetail("fichero", "appsettings.json");

MlResult<string> url = _config.ReadAppSettingKey<string>("Api:Url", errores);
```

### Con `string` — atajo cómodo

```csharp
MlResult<string> url = _config.ReadAppSettingKey<string>("Api:Url",
        "Falta la URL de la API en la configuración");
```

Internamente hace `MlErrorsDetails.FromErrorMessage(errorMessage)`.

### Sin nada — mensaje por defecto

```csharp
MlResult<string> url = _config.ReadAppSettingKey<string>("Api:Url");
// Fail → "No value found configured with the key 'Api:Url'"
```

> 💡 **El mensaje por defecto ya incluye el nombre de la clave**, y suele ser suficiente para diagnóstico. Personaliza solo cuando el mensaje vaya a llegar a un usuario o cuando quieras añadir contexto (qué fichero, qué entorno).

> ⚠️ **El mensaje por defecto está en inglés** (`"No value found configured with the key '…'"`). Si tu aplicación reporta errores en español, usa la sobrecarga con `string`.

---

## Cómo funciona internamente

Todo el proyecto se reduce a este método privado:

```csharp
private MlResult<T> ReadConfigKey<T>(string          configKey,
                                     Func<string, T> configSearch,
                                     MlErrorsDetails errorsDetails = null!)
{
    var result = EnsureFp.NotNullEmptyOrWhitespace(configKey, "Tkey cannot be null white or empty")
                         .Map      (_   => configSearch(configKey))
                         .MapEnsure(res => res is not null,
                                    errorsDetails is not null
                                        ? errorsDetails
                                        : $"No value found configured with the key '{configKey}'");
    return result!;
}
```

Tres pasos en el raíl:

| Paso | Qué hace | Si falla |
|---|---|---|
| `EnsureFp.NotNullEmptyOrWhitespace` | Valida que **la clave** no sea nula ni vacía | `Fail` → `"Tkey cannot be null white or empty"` |
| `.Map(configSearch)` | Ejecuta la lectura (`GetValue<T>`, indexador o `GetConnectionString`) | — |
| `.MapEnsure(res is not null)` | Comprueba que **el valor leído** no sea nulo | `Fail` → mensaje por defecto o el tuyo |

Los tres métodos públicos son la **misma tubería con un `Func` distinto**:

```csharp
ReadConnectionStringKey → configSearch = _configuration.GetConnectionString
ReadAppSettingKey<T>    → configSearch = key => _configuration.GetValue<T>(key)!
ReadAppSettingKey       → configSearch = key => _configuration[key]!
```

> 💡 **Es un buen ejemplo de diseño funcional**: la lógica común (validar clave → leer → validar valor) se escribe una vez, y la variación se inyecta como función.

---

## Claves anidadas

`IConfiguration` usa `:` como separador de niveles, y `IMlConfigManager` lo hereda tal cual.

Dado este `appsettings.json`:

```jsonc
{
  "SimpleKey": "SimpleValue",
  "ComplexKey": {
    "ComplexKey1": "ComplexValue1",
    "ComplexKey2": "ComplexValue2"
  },
  "Proxy": {
    "UseProxy": "true",
    "ProxyUrl": "proxy.ceca.es"
  }
}
```

```csharp
_config.ReadAppSettingKey<string>("SimpleKey");                // ✅ "SimpleValue"
_config.ReadAppSettingKey<string>("ComplexKey:ComplexKey1");   // ✅ "ComplexValue1"
_config.ReadAppSettingKey<bool>  ("Proxy:UseProxy");           // ✅ true
_config.ReadAppSettingKey<string>("NoExiste");                 // ❌ Fail
_config.ReadAppSettingKey<string>("NoExiste:Sub");             // ❌ Fail
```

> ⚠️ **No se puede leer una sección completa como objeto.** No hay equivalente a `configuration.GetSection("Proxy").Get<ProxyOptions>()`. Ver [Lo que NO incluye](#️-lo-que-no-incluye).

---

## ⚠️ Particularidades reales del código fuente

### 1. `MapEnsure` recibe una expresión ternaria, no un fallback funcional

```csharp
.MapEnsure(res => res is not null,
           errorsDetails is not null ? errorsDetails
                                     : $"No value found configured with the key '{configKey}'")
```

Los dos brazos del ternario son de tipos distintos (`MlErrorsDetails` y `string`) y funciona por **conversión implícita de `string` a `MlErrorsDetails`**. Es válido, pero **la interpolación de la cadena se evalúa siempre**, incluso cuando `errorsDetails` no es nulo y ese texto se va a descartar.

> 💡 Coste irrelevante en la práctica (leer configuración no está en un bucle caliente), pero conviene saberlo.

### 2. El mensaje de clave inválida tiene una errata

```csharp
EnsureFp.NotNullEmptyOrWhitespace(configKey, "Tkey cannot be null white or empty")
```

**`"Tkey"`** (debería ser `"The key"`) y **`"white"`** (debería ser `"whitespace"`). El mensaje llega tal cual al `Fail`. Si escribes tests que comparen el texto exacto, hazlo con esta cadena.

### 3. Ese mensaje **no es personalizable**

Los parámetros `errorsDetails` / `errorMessage` **solo afectan al caso "valor no encontrado"**. Si pasas una clave vacía, obtendrás siempre el mensaje con la errata, ignorando lo que hayas pasado:

```csharp
_config.ReadAppSettingKey<string>("", "Mi mensaje personalizado");
// Fail → "Tkey cannot be null white or empty"   ← tu mensaje se ignora
```

### 4. `null!` y `result!` por todas partes

El proyecto tiene `Nullable` activado, pero usa el operador `!` de forma generosa (`= null!`, `return result!`, `configSearch(configKey)!`). **Los avisos de nulabilidad están silenciados, no resueltos.** En particular, `ReadAppSettingKey<T>` con un `T` de tipo valor (`int`, `bool`) que no exista devolverá `default(T)`…

### 5. ⚠️ Los tipos valor pueden dar falsos positivos

```csharp
// appsettings.json NO contiene "Api:Timeout"
MlResult<int> timeout = _config.ReadAppSettingKey<int>("Api:Timeout");
```

`GetValue<int>()` devuelve **`0`** cuando la clave no existe, no `null`. Como la comprobación es `res is not null`, **`0` pasa el filtro y obtienes un `Valid` con valor `0`**.

> ⚠️ **Este es el riesgo más importante del proyecto.** Con tipos valor no anulables, "clave ausente" es indistinguible de "clave con valor por defecto".
>
> **Solución**: usa el tipo anulable y valida después.
> ```csharp
> // ✅ Ahora sí detecta la ausencia
> MlResult<int> timeout = _config.ReadAppSettingKey<int?>("Api:Timeout")
>                                .Map(v => v!.Value);
> ```
> O bien lee como `string` y convierte:
> ```csharp
> MlResult<int> timeout = _config.ReadAppSettingKey<string>("Api:Timeout")
>                                .TryMap(s => int.Parse(s), "Api:Timeout no es un número");
> ```

### 6. Una conversión inválida lanza excepción, no devuelve `Fail`

```jsonc
{ "Api": { "TimeoutSeconds": "no-soy-un-numero" } }
```

```csharp
_config.ReadAppSettingKey<int>("Api:TimeoutSeconds");   // 💥 InvalidOperationException
```

`GetValue<T>()` lanza cuando no puede convertir, y **el `Map` no está envuelto en un `TryMap`**, así que la excepción **sale del raíl**. Si no controlas el contenido del `appsettings.json`, envuelve la llamada:

```csharp
var timeout = MlResult.TryMap(() => _config.ReadAppSettingKey<int>("Api:TimeoutSeconds"),
                              "Api:TimeoutSeconds no es un entero válido")
                      .Bind(r => r);
```

### 7. Una cadena vacía se considera un valor válido

```jsonc
{ "Rutas": { "RutaFicheros": "" } }
```

```csharp
_config.ReadAppSettingKey<string>("Rutas:RutaFicheros");   // ✅ Valid con ""
```

La comprobación es `res is not null`, **no** `NotNullEmptyOrWhitespace`. Curiosamente, **la clave se valida más estrictamente que el valor**. Si necesitas rechazar vacíos, encadena:

```csharp
_config.ReadAppSettingKey<string>("Rutas:RutaFicheros")
       .BindEnsure(s => !string.IsNullOrWhiteSpace(s), "La ruta de ficheros está vacía");
```

### 8. No hay validación de arranque

La lectura ocurre **cuando llamas al método**, no al iniciar la aplicación. Una clave ausente se descubre en la primera petición que la use, no en el arranque. Ver [Mejores prácticas](#mejores-prácticas) para el patrón de validación temprana.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay binding de secciones a objetos.** No existe `ReadSection<TOptions>("Proxy")`. Para configuración compleja tendrás que leer clave a clave o seguir usando `IOptions<T>` en paralelo.

> ⚠️ **No hay métodos asíncronos.** Toda la API es síncrona (lo cual es correcto: `IConfiguration` también lo es).

> ⚠️ **No hay valores por defecto.** No existe `ReadAppSettingKey<T>(clave, valorPorDefecto)`. Si quieres un fallback, usa `.ValueOr(...)` o `BindIfFail` del núcleo:
> ```csharp
> int timeout = _config.ReadAppSettingKey<int?>("Api:Timeout")
>                      .Map(v => v ?? 30)
>                      .ValueOr(30);
> ```

> ⚠️ **No hay lectura de variables de entorno ni de secretos de forma específica.** Funcionan si están en el `IConfiguration` que le pasas (porque `ConfigurationBuilder` las incorpora), pero no hay API dedicada.

> ⚠️ **No hay soporte para recarga (`IOptionsMonitor`, `reloadOnChange`).** Cada llamada lee del `IConfiguration` capturado; si el proveedor soporta recarga, verás el valor nuevo, pero no hay notificación de cambios.

> ⚠️ **No hay escritura.** Es solo lectura, como `IConfiguration`.

> ⚠️ **No contiene ninguna otra utilidad.** Pese al nombre "Utilities", el proyecto es **exclusivamente** gestión de configuración.

---

## Ejemplos prácticos

### Ejemplo 1 — Uso básico en un servicio

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Utilities.Config;

public class ClienteApi(IMlConfigManager _config, HttpClient _http)
{
    public Task<MlResult<Pedido>> ObtenerPedido(int id)
        => _config.ReadAppSettingKey<string>("Api:BaseUrl", "Falta 'Api:BaseUrl' en la configuración")
                  .BindAsync(baseUrl => _http.GetMlAsync<Pedido>($"{baseUrl}/pedidos/{id}"));
}
```

### Ejemplo 2 — Componer varias claves en un objeto de configuración

```csharp
public record ConfigProxy(bool Usar, string Url, string Usuario);

public MlResult<ConfigProxy> LeerConfigProxy()
    => _config.ReadAppSettingKey<bool>  ("Proxy:UseProxy"  , "Falta 'Proxy:UseProxy'")
              .Bind(usar => _config.ReadAppSettingKey<string>("Proxy:ProxyUrl" , "Falta 'Proxy:ProxyUrl'")
              .Bind(url  => _config.ReadAppSettingKey<string>("Proxy:ProxyUser", "Falta 'Proxy:ProxyUser'")
              .Map (user => new ConfigProxy(usar, url, user))));
```

> 💡 Este anidamiento es la forma clásica; con `MlResult` también puedes usar `Fusion`/`Join` del núcleo para acumular **todos** los errores en lugar de parar en el primero. Ver [`MlResultActions`](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md).

### Ejemplo 3 — Validación al arrancar (patrón recomendado)

```csharp
// Program.cs
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IMlConfigManager>();

    var comprobacion = config.ReadConnectionStringKey("DemoDb")
                             .Bind(_ => config.ReadAppSettingKey<string>("Api:BaseUrl"))
                             .Bind(_ => config.ReadAppSettingKey<string>("Rutas:RutaFicheros"));

    if (comprobacion.IsFail)
        throw new InvalidOperationException(
            $"Configuración incompleta: {comprobacion.ErrorsDetails.ToErrorsDescription()}");
}

app.Run();
```

> 💡 **Fallar rápido en el arranque** es mucho mejor que descubrir la clave ausente en la primera petición de un cliente.

### Ejemplo 4 — Cadena de conexión + `DbContext`

```csharp
builder.Services.AddMlUtilitiesConfig(builder.Configuration);

var configManager = new MlConfigManager(builder.Configuration);

configManager.ReadConnectionStringKey("DemoDb", "Falta la cadena de conexión 'DemoDb'")
             .ExecSelf(cs => builder.Services.AddDbContext<DemoDbContext>(o => o.UseSqlServer(cs)))
             .ExecSelfIfFail(e => throw new InvalidOperationException(e.ToErrorsDescription()));
```

### Ejemplo 5 — Combinado con logging

```csharp
using MoralesLarios.OOFP.Extensions.Loggers;

public Task<MlResult<Informe>> Generar()
    => _config.ReadAppSettingKey<string>("Rutas:RutaFicheros")
              .LogMlResultErrorIfFail(_logger, e => $"Configuración: {e.ToErrorsDescription()}")
              .BindAsync(ruta => _generador.GenerarAsync(ruta));
```

### Ejemplo 6 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: tipo valor no anulable → una clave ausente devuelve Valid con 0
MlResult<int> timeout = _config.ReadAppSettingKey<int>("Api:Timeout");

// ✅ BIEN: anulable, así el Fail sí se detecta
MlResult<int> timeout = _config.ReadAppSettingKey<int?>("Api:Timeout")
                               .Map(v => v!.Value);


// ❌ MAL: la versión no genérica devuelve object y obliga a un cast
var valor = _config.ReadAppSettingKey("Api:Timeout").Map(o => (int)o);   // 💥 InvalidCastException: es string

// ✅ BIEN: usa la genérica
var valor = _config.ReadAppSettingKey<int?>("Api:Timeout");


// ❌ MAL: confiar en que una conversión inválida devuelva Fail
_config.ReadAppSettingKey<int>("Api:Timeout");        // 💥 lanza si el valor no es numérico

// ✅ BIEN: leer como string y convertir en el raíl
_config.ReadAppSettingKey<string>("Api:Timeout")
       .TryMap(s => int.Parse(s), "Api:Timeout no es un entero válido");


// ❌ MAL: dar por bueno un valor presente pero vacío
_config.ReadAppSettingKey<string>("Rutas:RutaFicheros");   // "" es Valid

// ✅ BIEN: exigir contenido
_config.ReadAppSettingKey<string>("Rutas:RutaFicheros")
       .BindEnsure(s => !string.IsNullOrWhiteSpace(s), "La ruta de ficheros está vacía");


// ❌ MAL: leer configuración en cada petición dentro de un bucle
foreach (var item in items)
    var url = _config.ReadAppSettingKey<string>("Api:Url");   // repetido N veces

// ✅ BIEN: leer una vez y pasar el valor
var urlResult = _config.ReadAppSettingKey<string>("Api:Url");
foreach (var item in items) { /* usa urlResult.Value */ }
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Una cadena de `appsettings.json` | `ReadAppSettingKey<string>("Seccion:Clave")` |
| Un número o booleano con seguridad | `ReadAppSettingKey<int?>("…")` + `.Map(v => v!.Value)` |
| Una cadena de conexión | `ReadConnectionStringKey("Nombre")` |
| El valor sin convertir | `ReadAppSettingKey("…")` → `MlResult<object>` |
| Un mensaje de error propio | La sobrecarga con `string errorMessage` |
| Detalles estructurados en el error | La sobrecarga con `MlErrorsDetails` |
| Un valor por defecto si falta | `.Map(v => v ?? porDefecto)` o `.ValueOr(porDefecto)` |
| Rechazar valores vacíos | `.BindEnsure(s => !string.IsNullOrWhiteSpace(s), "…")` |
| Tolerar valores mal formados | Leer como `string` + `TryMap` |
| Bindear una sección completa a una clase | ❌ No disponible: usa `IOptions<T>` |

---

## Mejores prácticas

1. **Valida toda la configuración crítica en el arranque**, no en la primera petición (ver [Ejemplo 3](#ejemplo-3--validación-al-arranque-patrón-recomendado)).
2. **Usa tipos anulables (`int?`, `bool?`) para tipos valor**: es la única forma de distinguir "ausente" de "cero".
3. **Prefiere `ReadAppSettingKey<T>` sobre la versión no genérica**: `MlResult<object>` pierde la seguridad de tipos.
4. **Lee cada clave una sola vez** y pasa el valor: no llames al gestor dentro de bucles.
5. **Si el `appsettings.json` no está bajo tu control, envuelve con `TryMap`**: una conversión inválida lanza excepción.
6. **Añade `BindEnsure` cuando una cadena vacía no sea aceptable**: la biblioteca solo comprueba `null`.
7. **Personaliza el mensaje si va a verlo alguien que no seas tú**: el de por defecto está en inglés.
8. **Para configuración compleja y jerárquica, sigue usando `IOptions<T>`**: este proyecto es para claves sueltas.
9. **No metas secretos en `appsettings.json`**: usa el gestor de secretos o variables de entorno (funcionan igual a través de `IConfiguration`).
10. **Combina con [`Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md)** para dejar traza de qué clave faltaba.
11. **Registra `AddMlUtilitiesConfig` una sola vez** y pásale la `IConfiguration` definitiva: la lambda la captura.
12. **Depende de `IMlConfigManager`, no de `MlConfigManager`**: facilita los tests con dobles.

---

## Resumen

- Proyecto **minimalista**: tres ficheros de código y una única abstracción, `IMlConfigManager`.
- Convierte la lectura de configuración en una operación del raíl funcional: **si la clave falta, obtienes un `Fail` con el nombre de la clave**, no un `null` silencioso.
- **Tres operaciones**: `ReadAppSettingKey<T>` (con conversión), `ReadAppSettingKey` (crudo, `object`) y `ReadConnectionStringKey`.
- **Dos formas de describir el error** para cada una: `MlErrorsDetails` o `string`. Sin ninguna, se usa un mensaje por defecto en inglés.
- Internamente todo pasa por una única tubería: `EnsureFp.NotNullEmptyOrWhitespace(clave)` → `Map(lectura)` → `MapEnsure(valor is not null)`.
- Registro: `services.AddMlUtilitiesConfig(configuration)` (como `Transient`, capturando la `IConfiguration`).
- ⚠️ **El riesgo principal**: con tipos valor no anulables (`int`, `bool`), una clave ausente devuelve `Valid` con el valor por defecto. **Usa siempre `int?`, `bool?`.**
- ⚠️ Otros límites: una conversión inválida **lanza excepción**, una cadena vacía se considera **valor válido**, el mensaje de clave inválida tiene erratas (`"Tkey … white"`) y **no es personalizable**, y **no hay binding de secciones a objetos**.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — consumidor natural de `ReadConnectionStringKey`
- [`MoralesLarios.OOFP.HttpClients`](../MoralesLarios.OOFP.HttpClients/README.md) — consumidor natural de las URLs base
- [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) — trazar los fallos de configuración
- [`MoralesLarios.OOFP.IO`](../MoralesLarios.OOFP.IO/README.md) — operaciones con las rutas leídas de configuración
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validaciones más elaboradas sobre los valores leídos

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — mensajes y detalles del error](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — validaciones de guarda](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Map` y `MapEnsure` — transformación y comprobación](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`TryMap` — capturar excepciones en el raíl](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
