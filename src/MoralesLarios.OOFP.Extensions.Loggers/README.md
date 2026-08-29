# MoralesLarios.OOFP.Extensions.Loggers — Logging sin romper la cadena

Extensiones que permiten **registrar trazas en medio de un pipeline funcional** sin interrumpirlo. Cada método recibe un [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md), escribe en el `ILogger` y **devuelve el mismo `MlResult<T>` intacto**, de modo que la cadena continúa exactamente igual que si el log no existiera.

Es el equivalente funcional del "punto de observación": no transforma, no valida, no decide. **Solo observa y pasa.**

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [El patrón fundamental: log transparente](#el-patrón-fundamental-log-transparente)
5. [Familia 1 — `LogMlResult` con `LogLevel` explícito](#familia-1--logmlresult-con-loglevel-explícito)
6. [Familia 2 — `LogMlResultIfValid` / `LogMlResultIfFail`](#familia-2--logmlresultifvalid--logmlresultiffail)
7. [Familia 3 — Filtrado por contenido de `Details`](#familia-3--filtrado-por-contenido-de-details)
8. [Familia 4 — `LogMlResultFinal` y `LogGeneralErrorIfFail`](#familia-4--logmlresultfinal-y-loggeneralerroriffail)
9. [Familia 5 — Los seis ficheros por nivel](#familia-5--los-seis-ficheros-por-nivel)
10. [Familia 6 — `MyMethodFinalLog`](#familia-6--mymethodfinallog)
11. [Variantes síncronas y asíncronas](#variantes-síncronas-y-asíncronas)
12. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
13. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
14. [Ejemplos prácticos](#ejemplos-prácticos)
15. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
16. [Mejores prácticas](#mejores-prácticas)
17. [Resumen](#resumen)
18. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

En un pipeline funcional, meter un log obliga normalmente a **partir la cadena**: guardar el resultado en una variable, mirar si es válido, escribir la traza y seguir.

❌ **Sin estas extensiones:**

```csharp
public async Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
{
    var validado = dto.Validate();
    if (validado.IsFail)
    {
        _logger.LogWarning($"Validación fallida: {validado.ErrorsDetails.ToErrorsDescription()}");
        return validado.ToMlResultFail<Pedido>();       // hay que reconstruir el Fail
    }

    var guardado = await _repo.AddAsync(Mapear(validado.Value));
    if (guardado.IsFail)
    {
        _logger.LogError($"Error al guardar: {guardado.ErrorsDetails.ToErrorsDescription()}");
        return guardado;
    }

    _logger.LogInformation($"Pedido {guardado.Value.Id} creado");
    return guardado;
}
```

✅ **Con estas extensiones:**

```csharp
public Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
    => dto.Validate()
          .LogMlResultWarningIfFail(_logger, e => $"Validación fallida: {e.ToErrorsDescription()}")
          .BindAsync(v => _repo.AddAsync(Mapear(v)))
          .LogMlResultFinalAsync(_logger,
                                 validBuildMessage: p => $"Pedido {p.Id} creado",
                                 failBuildMessage : e => $"Error al crear pedido: {e.ToErrorsDescription()}");
```

> 💡 **La clave**: el log es **un eslabón transparente**. No cambia el tipo, no cambia el valor, no cambia el estado. Puedes insertarlo o quitarlo de cualquier punto de la cadena sin tocar nada más.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| `Microsoft.Extensions.Logging.Abstractions` (8.0.1) | `ILogger`, `LogLevel` |
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) | `MlResult<T>`, `MlErrorsDetails`, `Match`, `Bind` |

Destino: **`net8.0`**. Versión del paquete: **1.0.9**.

```csharp
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Extensions.Loggers;   // 🔑 todas las extensiones viven aquí
```

> 💡 **Un único namespace para todo el proyecto**: `MoralesLarios.OOFP.Extensions.Loggers`. No hay subnamespace `.Helpers` como en otros proyectos de la solución.

**No requiere registro en el contenedor de dependencias.** Solo necesitas un `ILogger` (inyectado como `ILogger<T>` en tus servicios, o creado con `ILoggerFactory`).

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Extensions.Loggers/
├── GlobalUsings.cs
├── GeneralExtensionLoggers.cs               → núcleo: LogLevel como parámetro
├── GeneralExtensionLoggersTrace.cs          → nivel Trace fijado
├── GeneralExtensionLoggersDebug.cs          → nivel Debug fijado
├── GeneralExtensionLoggersInformation.cs    → nivel Information fijado
├── GeneralExtensionLoggersWarning.cs        → nivel Warning fijado
├── GeneralExtensionLoggersError.cs          → nivel Error fijado
├── GeneralExtensionLoggersCritical.cs       → nivel Critical fijado
└── Extensions.cs                            → MyMethodFinalLog (atajo de más alto nivel)
```

Todo son **clases estáticas con métodos de extensión**. La arquitectura es una pirámide:

```
Extensions.MyMethodFinalLog                    ← atajo con mensajes prefabricados
        ↓
GeneralExtensionLoggers.LogMlResultFinal        ← Information si válido / Error si fallo
        ↓
GeneralExtensionLoggersXxx.LogMlResultXxx…      ← LogLevel fijado por fichero
        ↓
GeneralExtensionLoggers.LogMlResult…            ← LogLevel como parámetro
        ↓
ILogger.Log(logLevel, message)                  ← la API estándar de .NET
```

> 💡 Cada capa delega en la de abajo. Si entiendes `LogMlResult` con `LogLevel`, entiendes **todo el proyecto**: el resto son atajos.

---

## El patrón fundamental: log transparente

Este es el método más simple, y explica todo lo demás:

```csharp
public static MlResult<T> LogMlResult<T>(this MlResult<T> source,
                                              ILogger     logger,
                                              LogLevel    logLevel,
                                              string      message)
{
    logger.Log(logLevel, message);
    return source;              // 🔑 el resultado sale intacto
}
```

Y la variante con constructores de mensaje:

```csharp
public static MlResult<T> LogMlResult<T>(this MlResult<T>                   source,
                                              ILogger                       logger,
                                              LogLevel                      logLevel,
                                              Func<T, string>               validBuilMessage = null!,
                                              Func<MlErrorsDetails, string> failBuildMessage = null!)
{
    source.Match(
        valid: x      => validBuilMessage != null ? logger.LogMlResult(logLevel, validBuilMessage(x))      : null!,
        fail : errors => failBuildMessage != null ? logger.LogMlResult(logLevel, failBuildMessage(errors)) : null!)
    );

    return source;
}
```

Tres propiedades que se cumplen en **todo** el proyecto:

| Propiedad | Consecuencia |
|---|---|
| **Devuelve `source` sin tocarlo** | Puedes insertar o quitar el log en cualquier punto de la cadena |
| **Los `Func` se evalúan solo en su rama** | Si el resultado es válido, `failBuildMessage` **no se ejecuta** ⇒ no hay coste de formateo |
| **Un `Func` a `null` ⇒ no se registra nada** | Así se implementan las variantes `IfValid` / `IfFail` |

> 💡 **La evaluación diferida es importante para el rendimiento**: `e => $"Error: {e.ToErrorsDescription()}"` solo construye la cadena si realmente hay fallo.

Además existe la variante que registra **cada rama en un nivel distinto** usando tuplas:

```csharp
public static MlResult<T> LogMlResult<T>(this MlResult<T> source,
                                              ILogger     logger,
                                              (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                              (LogLevel logLevel, Func<MlErrorsDetails, string> buildMessage) failBuildMessage);
```

```csharp
resultado.LogMlResult(_logger,
                      validBuildMessage: (LogLevel.Debug, p => $"Pedido {p.Id} procesado"),
                      failBuildMessage : (LogLevel.Error, e => $"Fallo: {e.ToErrorsDescription()}"));
```

> ⚠️ **Ojo con el nombre del parámetro**: en la sobrecarga con `Func` y `LogLevel` suelto se llama **`validBuilMessage`** (le falta la `d` de "Build"), mientras que en la sobrecarga con tuplas se llama **`validBuildMessage`** (correcto). Si usas argumentos con nombre, tienes que respetar la errata en unas y no en otras. Ver [Particularidades](#️-particularidades-reales-del-código-fuente).

---

## Familia 1 — `LogMlResult` con `LogLevel` explícito

En `GeneralExtensionLoggers.cs`. El nivel es siempre un parámetro.

| Firma | Cuándo registra |
|---|---|
| `logger.LogMlResult(logLevel, message)` → `MlResult<ILogger>` | Siempre. Punto de entrada de bajo nivel |
| `source.LogMlResult(logger, logLevel, message)` | Siempre, con mensaje fijo |
| `source.LogMlResult(logger, logLevel, validMessage, failMessage)` | Siempre, mensaje distinto según estado |
| `source.LogMlResult(logger, logLevel, validBuilMessage, failBuildMessage)` | Según qué `Func` pases (los `null` no registran) |
| `source.LogMlResult(logger, (nivel, funcValid), (nivel, funcFail))` | Siempre, **nivel distinto por rama** |

> 💡 La primera sobrecarga (`this ILogger`) devuelve `MlResult<ILogger>`. Es el ladrillo interno que usan las demás; rara vez lo necesitarás directamente, pero permite encadenar logs: `_logger.LogMlResult(LogLevel.Debug, "inicio").Bind(l => …)`.

---

## Familia 2 — `LogMlResultIfValid` / `LogMlResultIfFail`

Cuando solo te interesa una de las dos ramas:

```csharp
// Solo si es válido
source.LogMlResultIfValid(logger, logLevel, "Operación completada");
source.LogMlResultIfValid(logger, logLevel, x => $"Creado el id {x.Id}");

// Solo si es fallo
source.LogMlResultIfFail(logger, logLevel, "La operación no se pudo completar");
source.LogMlResultIfFail(logger, logLevel, e => $"Fallo: {e.ToErrorsDescription()}");
```

Internamente son un `Match` que en la rama contraria devuelve `source` sin llamar al logger:

```csharp
=> source.Match(
        valid: x      => source.LogMlResult(logger, logLevel, validBuildMessage(x)),
        fail : errors => source              // ← no registra nada
    );
```

---

## Familia 3 — Filtrado por contenido de `Details`

Estas tres familias son las más específicas de la biblioteca: **deciden si registrar según lo que haya en [`MlErrorsDetails.Details`](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)**.

### `LogMlResultIfFailWithException` — solo si el fallo trae excepción

```csharp
public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                   source,
                                                                 ILogger                       logger,
                                                                 LogLevel                      logLevel,
                                                                 Func<MlErrorsDetails, string> failBuildMessage)
    => source.Match(
            valid: x             => source,
            fail : errorsDetails => errorsDetails.GetDetailException()
                                        .Bind(_ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
        );
```

Como `GetDetailException()` devuelve `Fail` cuando **no** hay excepción registrada, el `Bind` no se ejecuta y **no se registra nada**.

> 💡 **Uso típico**: separar errores técnicos (excepción capturada por un `TryMap`) de errores de negocio (una validación incumplida). Los primeros merecen `LogLevel.Error` con la excepción; los segundos, `LogLevel.Warning` sin ruido.

Existe además una sobrecarga que **te da la excepción** en el constructor del mensaje:

```csharp
source.LogMlResultIfFailWithException(_logger, LogLevel.Error,
        (errores, ex) => $"Fallo técnico: {errores.ToErrorsDescription()} | {ex.GetType().Name}: {ex.Message}");
```

### `LogMlResultIfFailWithoutException` — solo si el fallo **no** trae excepción

El complementario exacto. Usa `BindIfFail`, así que registra cuando `GetDetailException()` falla:

```csharp
source.LogMlResultIfFailWithoutException(_logger, LogLevel.Warning,
        e => $"Error de negocio: {e.ToErrorsDescription()}");
```

> 💡 **Combinando las dos** obtienes una clasificación completa sin un solo `if`:
> ```csharp
> resultado
>     .LogMlResultIfFailWithException   (_logger, LogLevel.Error  , (e, ex) => $"Técnico: {ex.Message}")
>     .LogMlResultIfFailWithoutException(_logger, LogLevel.Warning, e       => $"Negocio: {e.ToErrorsDescription()}");
> ```

### `LogMlResultIfFailWithValue` — solo si el fallo arrastra un valor

Cuando el `Fail` conserva el valor original en los detalles (patrón habitual de `…WithValue` del núcleo):

```csharp
// El valor debe ser del mismo tipo T
source.LogMlResultIfFailWithValue(_logger, LogLevel.Error,
        e => $"Falló procesando el elemento: {e.ToErrorsDescription()}");

// O de un tipo distinto, y lo recibes en el mensaje
source.LogMlResultIfFailWithValue<Pedido, int>(_logger, LogLevel.Error,
        (errores, idFila) => $"Falló la fila {idFila}: {errores.ToErrorsDescription()}");
```

---

## Familia 4 — `LogMlResultFinal` y `LogGeneralErrorIfFail`

### `LogMlResultFinal` — la convención "éxito = Information, fallo = Error"

```csharp
public static MlResult<T> LogMlResultFinal<T>(this MlResult<T>                   source,
                                                   ILogger                       logger,
                                                   Func<T, string>               validBuildMessage,
                                                   Func<MlErrorsDetails, string> failBuildMessage)
    => source.LogMlResult(logger,
                          (LogLevel.Information, validBuildMessage),
                          (LogLevel.Error      , failBuildMessage));
```

Es el método que querrás **al cerrar un caso de uso**: un único log que dice cómo terminó todo.

```csharp
await pipeline.LogMlResultFinalAsync(_logger,
        validBuildMessage: p => $"Pedido {p.Id} creado correctamente",
        failBuildMessage : e => $"No se pudo crear el pedido: {e.ToErrorsDescription()}");
```

También acepta cadenas fijas en lugar de `Func`.

### `LogGeneralErrorIfFail` — el atajo mínimo

```csharp
public static MlResult<T> LogGeneralErrorIfFail<T>(this MlResult<T> source, ILogger logger)
    => source.LogMlResultErrorIfFail(logger, failBuildMessage: errorDetals => errorDetals.ToErrorsDescription());
```

Sin mensaje, sin nivel, sin `Func`: **registra la descripción de los errores en `LogLevel.Error` si algo falló, y nada si fue bien.**

```csharp
return await _repo.GetByIdAsync(id)
                  .LogGeneralErrorIfFailAsync(_logger);   // ← una línea, cero configuración
```

> 💡 **El método más útil para empezar**: añádelo al final de cada método de repositorio o servicio y ya tendrás trazabilidad de todos los fallos.

---

## Familia 5 — Los seis ficheros por nivel

Cada nivel de `LogLevel` tiene su propio fichero con **el mismo catálogo de métodos**, pero con el nivel ya fijado. Ejemplo para `Error`:

```csharp
public static MlResult<T> LogMlResultError<T>(this MlResult<T> source, ILogger logger, string message)
    => source.LogMlResult<T>(logger, LogLevel.Error, message);
```

| Fichero | Prefijo de los métodos | `LogLevel` |
|---|---|---|
| `GeneralExtensionLoggersTrace.cs` | `LogMlResultTrace…` | `Trace` |
| `GeneralExtensionLoggersDebug.cs` | `LogMlResultDebug…` | `Debug` |
| `GeneralExtensionLoggersInformation.cs` | `LogMlResultInformation…` | `Information` |
| `GeneralExtensionLoggersWarning.cs` | `LogMlResultWarning…` | `Warning` |
| `GeneralExtensionLoggersError.cs` | `LogMlResultError…` | `Error` |
| `GeneralExtensionLoggersCritical.cs` | `LogMlResultCritical…` | `Critical` |

Y dentro de cada fichero, estas variantes (sustituye `Xxx` por el nivel):

| Método | Registra si… |
|---|---|
| `logger.LogMlResultXxx(message)` | Siempre (devuelve `MlResult<ILogger>`) |
| `source.LogMlResultXxx(logger, message)` | Siempre |
| `source.LogMlResultXxx(logger, validMessage, failMessage)` | Siempre, texto según estado |
| `source.LogMlResultXxx(logger, validBuilMessage, failBuildMessage)` | Según los `Func` no nulos |
| `source.LogMlResultXxxIfValid(logger, …)` | Solo si es válido |
| `source.LogMlResultXxxIfFail(logger, …)` | Solo si es fallo |
| `source.LogMlResultXxxIfFailWithValue(logger, …)` | Fallo **con valor** en `Details` |
| `source.LogMlResultXxxIfFailWithException(logger, …)` | Fallo **con excepción** en `Details` |
| `source.LogMlResultXxxIfFailWithoutException(logger, …)` | Fallo **sin excepción** en `Details` |

Todas tienen su pareja `…Async`.

> 💡 **Usa siempre las versiones por nivel** en código de aplicación: `LogMlResultWarningIfFail(_logger, "…")` se lee mucho mejor que `LogMlResultIfFail(_logger, LogLevel.Warning, "…")`. Reserva las de `LogLevel` explícito para cuando el nivel sea una variable.

> ⚠️ **`LogMlResultFinal` y `LogGeneralErrorIfFail` solo existen en `GeneralExtensionLoggers.cs`**, no tienen versión por nivel (no la necesitan: ya llevan el nivel decidido).

---

## Familia 6 — `MyMethodFinalLog`

En `Extensions.cs`. Es el atajo de más alto nivel: **tú solo dices qué estaba haciendo el método**, y él construye los dos mensajes.

```csharp
public static MlResult<T> MyMethodFinalLog<T>(this MlResult<T> source,
                                                   ILogger     logger,
                                                   string      methodActionDesc)
    => source.LogMlResultFinal(logger,
            validBuildMessage: item   => $"{methodActionDesc} done correctly.",
            failBuildMessage : errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");
```

```csharp
return await _repo.AddAsync(pedido)
                  .MyMethodFinalLogAsync(_logger, "creating order");
// Válido → Information: "creating order done correctly."
// Fallo  → Error      : "Error when creating order Error: <detalle completo>"
```

| Método | Uso |
|---|---|
| `MyMethodFinalLog(logger, methodActionDesc)` | Versión síncrona |
| `MyMethodFinalLogAsync(logger, methodActionDesc)` | Sobre `Task<MlResult<T>>` |

> ⚠️ **Los mensajes están en inglés y son fijos.** Si tu aplicación registra en español, o si el formato no te sirve, usa `LogMlResultFinal` directamente. El nombre (`MyMethodFinalLog`) delata que es una plantilla personal del autor más que una API pensada para consumo general.

> 💡 Nótese que usa `ToErrorsDetailsDescription()` (errores **y** detalles), mientras que `LogGeneralErrorIfFail` usa `ToErrorsDescription()` (solo los mensajes). El primero es más verboso y más útil para diagnóstico.

---

## Variantes síncronas y asíncronas

Casi todos los métodos existen en tres formas:

| Forma | Recibe | Devuelve | Para qué |
|---|---|---|---|
| `LogXxx` | `MlResult<T>` | `MlResult<T>` | Cadena síncrona |
| `LogXxxAsync` | `MlResult<T>` | `Task<MlResult<T>>` | Pasar de síncrono a asíncrono |
| `LogXxxAsync` | `Task<MlResult<T>>` | `Task<MlResult<T>>` | 🔑 **La habitual**: encadenar tras un `await` implícito |

```csharp
// Cadena totalmente asíncrona: no hace falta ningún await intermedio
return await _repo.GetAsync(id)                               // Task<MlResult<Pedido>>
                  .LogMlResultDebugIfValidAsync(_logger, p => $"Encontrado {p.Id}")
                  .BindAsync(p => _servicio.ProcesarAsync(p))
                  .LogGeneralErrorIfFailAsync(_logger);
```

> ⚠️ **Las versiones `Async` que reciben `MlResult<T>` (no `Task<…>`) no son asíncronas de verdad**: registran el log de forma síncrona y envuelven con `.ToAsync()` (`Task.FromResult`). Solo sirven para adaptar tipos, no para no bloquear.

---

## ⚠️ Particularidades reales del código fuente

### 1. El parámetro `validBuilMessage` está mal escrito

En las sobrecargas con `LogLevel` suelto, el parámetro se llama **`validBuilMessage`** (falta la `d`), mientras que su pareja es **`failBuildMessage`** (correcta).

```csharp
// ✅ Compila
source.LogMlResult(_logger, LogLevel.Information,
                   validBuilMessage: x => $"OK {x}",
                   failBuildMessage: e => $"KO {e}");

// ❌ NO compila: no existe un parámetro llamado validBuildMessage en esta sobrecarga
source.LogMlResult(_logger, LogLevel.Information,
                   validBuildMessage: x => $"OK {x}", …);
```

En cambio, la sobrecarga con **tuplas** y `LogMlResultFinal` sí usan `validBuildMessage` bien escrito. **Si usas argumentos con nombre, confía en IntelliSense** en lugar de en la memoria.

### 2. Hay una sobrecarga `async` que no se llama `Async`

```csharp
public static async Task<MlResult<T>> LogMlResultFinal<T>(this Task<MlResult<T>> sourceAsync,
                                                               ILogger           logger,
                                                               string            validMessage,
                                                               string            failMessage)
```

Recibe una `Task<MlResult<T>>` y es `async`, pero **se llama `LogMlResultFinal`, sin sufijo `Async`**. Funciona correctamente; solo rompe la convención de nombres del resto del proyecto.

### 3. `GeneralExtensionLoggersTrace.cs` tiene una región mal etiquetada

La primera región del fichero de `Trace` se llama `#region LogMlResultCritical` (copia-pega desde el fichero de `Critical`). **Los métodos sí usan `LogLevel.Trace`**: solo el comentario está equivocado. No afecta al comportamiento.

### 4. `GeneralExtensionLoggers.cs` arrastra un `using` inútil

```csharp
using static System.Runtime.InteropServices.JavaScript.JSType;
```

Residuo de una sugerencia automática del IDE. No se usa para nada y es inocuo, pero puede confundir al leer el fichero.

### 5. No se comprueba que `logger` sea `null`

Ningún método valida el `ILogger`. Con un `logger` nulo obtendrás una **`NullReferenceException` propagada**, no un `Fail`. El logging aquí **no está protegido por el raíl funcional**.

> 💡 En ASP.NET Core esto rara vez ocurre (el contenedor siempre inyecta un logger). Pero en tests con dobles mal configurados, sí.

### 6. Una excepción dentro de tu `Func` rompe la cadena

```csharp
source.LogMlResultIfValid(_logger, LogLevel.Debug, x => $"Id: {x.Cliente.Nombre}");
//                                                        ↑ si Cliente es null → 💥
```

Los constructores de mensaje **no están envueltos en `TryMap`**. Una excepción al formatear el texto **sale del pipeline como excepción**, no como `Fail`. Mantén los `Func` triviales y a prueba de nulos.

### 7. El `Match` interno descarta su resultado

En `LogMlResult` con `Func`, la llamada a `source.Match(...)` **no se asigna a nada**: se ejecuta solo por su efecto secundario (escribir el log) y luego se devuelve `source`. Es intencionado — así se garantiza la transparencia —, pero explica por qué las ramas pueden devolver `null!` sin consecuencias.

### 8. Se usa `logger.Log(logLevel, message)`, no logging estructurado

```csharp
logger.Log(logLevel, message);
```

El mensaje llega **ya interpolado**, como una cadena plana. Ver [Lo que NO incluye](#️-lo-que-no-incluye).

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay logging estructurado.** Los mensajes se interpolan antes de llegar al logger, así que **Serilog, Seq, Application Insights, etc. no podrán indexar campos**. No hay sobrecargas con plantilla y `params object[] args`.
> ```csharp
> // ❌ Esto NO existe
> source.LogMlResultInformation(_logger, "Pedido {OrderId} creado", pedido.Id);
> // ✅ Solo esto: el {OrderId} se pierde como campo
> source.LogMlResultInformation(_logger, $"Pedido {pedido.Id} creado");
> ```

> ⚠️ **No se pasa la `Exception` al `ILogger`.** Aunque `LogMlResultIfFailWithException` **detecta** la excepción y te la deja usar en el mensaje, **nunca** llama a `logger.Log(level, exception, message)`. Por tanto **el stack trace no se registra** salvo que lo incluyas tú en el texto (`ex.ToString()`).

> ⚠️ **No hay `EventId`, ni `scopes`, ni `BeginScope`.** No se puede correlacionar un conjunto de logs.

> ⚠️ **No comprueba `logger.IsEnabled(logLevel)`.** El mensaje se construye siempre que se entre en la rama correspondiente, incluso si el nivel está filtrado y el texto se va a descartar. Para `Trace`/`Debug` en bucles muy calientes, esto cuesta.

> ⚠️ **No existe `LogMlResultFinal` ni `LogGeneralErrorIfFail` por nivel** (`LogMlResultErrorFinal`, etc.).

> ⚠️ **No hay `IfValidWithValue`** ni variantes de filtrado por contenido en la rama válida: el filtrado por `Details` solo aplica a fallos.

> ⚠️ **No incluye `RegisterServices`.** A diferencia de [`EFCore`](../MoralesLarios.OOFP.EFCore/README.md), aquí no hay nada que registrar.

---

## Ejemplos prácticos

### Ejemplo 1 — Trazabilidad completa de un caso de uso

```csharp
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Extensions.Loggers;
using MoralesLarios.OOFP.Types;

public class ServicioPedidos
{
    private readonly ILogger<ServicioPedidos> _logger;
    private readonly IPedidosRepo             _repo;

    public Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
        => dto.Validate()
              .LogMlResultDebugIfValid  (_logger, d => $"DTO válido para cliente {d.ClienteId}")
              .LogMlResultWarningIfFail (_logger, e => $"DTO inválido: {e.ToErrorsDescription()}")
              .Map                      (Mapear)
              .BindAsync                (p => _repo.AddAsync(p))
              .LogMlResultFinalAsync    (_logger,
                                         validBuildMessage: p => $"Pedido {p.Id} creado",
                                         failBuildMessage : e => $"No se pudo crear el pedido: {e.ToErrorsDescription()}");
}
```

### Ejemplo 2 — Clasificar técnico vs. negocio sin un solo `if`

```csharp
public Task<MlResult<Factura>> Emitir(int pedidoId)
    => _repo.GetByIdAsync(pedidoId)
            .BindAsync(p => _facturador.EmitirAsync(p))
            .LogMlResultIfFailWithExceptionAsync   (_logger, LogLevel.Error,
                    (errores, ex) => $"Fallo técnico al emitir la factura del pedido {pedidoId}. " +
                                     $"{ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}")
            .LogMlResultIfFailWithoutExceptionAsync(_logger, LogLevel.Warning,
                    errores => $"No se pudo emitir la factura del pedido {pedidoId}: " +
                               errores.ToErrorsDescription());
```

> 💡 Como las dos condiciones son mutuamente excluyentes, **solo se registra una** de las dos trazas. El stack trace se incluye a mano porque la biblioteca no lo pasa al logger.

### Ejemplo 3 — El mínimo esfuerzo en un repositorio

```csharp
public class ClientesRepo
{
    private readonly ILogger<ClientesRepo> _logger;

    public Task<MlResult<Cliente>> GetByIdAsync(int id)
        => _ctx.Clientes.FindMlAsync(id)
                        .LogGeneralErrorIfFailAsync(_logger);

    public Task<MlResult<Cliente>> AddAsync(Cliente cliente)
        => _ctx.AddMlAsync(cliente)
               .LogGeneralErrorIfFailAsync(_logger);
}
```

> 💡 Una línea por método y todos los fallos quedan registrados en `Error`. Es la mejor relación esfuerzo/beneficio de la biblioteca.

### Ejemplo 4 — Niveles distintos por rama con tuplas

```csharp
resultado.LogMlResult(_logger,
        validBuildMessage: (LogLevel.Trace      , x => $"Caché resuelta: {x.Clave}"),
        failBuildMessage : (LogLevel.Information, e => $"Caché no disponible, se recalcula: {e.ToErrorsDescription()}"));
```

> 💡 Aquí el "fallo" no es grave (un fallo de caché), así que se registra en `Information` y no en `Error`. Con `LogMlResultFinal` no podrías hacerlo, porque fija `Error`.

### Ejemplo 5 — Log dentro de un bucle sobre una colección

```csharp
public MlResult<IEnumerable<Fila>> ProcesarFichero(IEnumerable<Fila> filas)
    => filas.Select((fila, i) =>
                Procesar(fila)
                    .AddMlErrorDetailIfFail("fila", i + 1)
                    .LogMlResultWarningIfFail(_logger,
                        e => $"Fila {i + 1} descartada: {e.ToErrorsDescription()}"))
            .FusionErrosIfExists();
```

> ⚠️ Con miles de filas esto genera miles de trazas. Considera registrar solo el resumen final, o usar `LogMlResultTraceIfFail` para poder filtrarlas por configuración.

### Ejemplo 6 — Traza de entrada y salida de un método

```csharp
public Task<MlResult<Informe>> Generar(int año)
{
    _logger.LogMlResultDebug($"Generando informe del año {año}");   // sobre ILogger, sin MlResult

    return _datos.CargarAsync(año)
                 .BindAsync(d => _motor.CalcularAsync(d))
                 .MyMethodFinalLogAsync(_logger, $"generating report for {año}");
}
```

### Ejemplo 7 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: esperar logging estructurado
source.LogMlResultInformation(_logger, "Pedido {Id} creado", pedido.Id);   // 💥 no existe

// ✅ BIEN: interpolar (aceptando que se pierde el campo indexable)
source.LogMlResultInformation(_logger, $"Pedido {pedido.Id} creado");


// ❌ MAL: usar el nombre "correcto" del parámetro en la sobrecarga con LogLevel
source.LogMlResult(_logger, LogLevel.Debug, validBuildMessage: x => $"{x}");   // 💥 no compila

// ✅ BIEN: respetar la errata
source.LogMlResult(_logger, LogLevel.Debug, validBuilMessage: x => $"{x}");

// ❌ MAL: Func que puede lanzar → rompe el pipeline con excepción
source.LogMlResultIfValid(_logger, LogLevel.Debug, x => $"{x.Cliente.Nombre}");
//                                                        ↑ si Cliente es null → 💥

// ✅ BIEN: a prueba de nulos
source.LogMlResultIfValid(_logger, LogLevel.Debug, x => $"{x.Cliente?.Nombre ?? "(sin cliente)"}");

// ❌ MAL: creer que se registra el stack trace
source.LogMlResultErrorIfFail(_logger, e => "Error al guardar");   // se pierde la excepción

// ✅ BIEN: incluir la excepción explícitamente
source.LogMlResultIfFailWithException(_logger, LogLevel.Error,
        (e, ex) => $"Error al guardar. {ex}");

// ❌ MAL: partir la cadena para registrar
var r = await pipeline;
if (r.IsFail) _logger.LogError(r.ErrorsDetails.ToErrorsDescription());
return r;

// ✅ BIEN: log transparente
return await pipeline.LogGeneralErrorIfFailAsync(_logger);
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Registrar todos los fallos con el mínimo código | `LogGeneralErrorIfFailAsync(_logger)` |
| Un único log de cierre de caso de uso | `LogMlResultFinalAsync(_logger, funcValid, funcFail)` |
| Lo mismo con mensajes prefabricados (en inglés) | `MyMethodFinalLogAsync(_logger, "doing X")` |
| Registrar solo cuando va bien | `LogMlResultXxxIfValid(_logger, …)` |
| Registrar solo cuando falla | `LogMlResultXxxIfFail(_logger, …)` |
| Separar fallo técnico de fallo de negocio | `…IfFailWithException` + `…IfFailWithoutException` |
| Registrar el valor que arrastra el fallo | `LogMlResultXxxIfFailWithValue(_logger, …)` |
| Nivel distinto en la rama válida y en la de fallo | `LogMlResult(_logger, (nivel, func), (nivel, func))` |
| Nivel decidido en tiempo de ejecución | `LogMlResult(_logger, logLevel, …)` |
| Escribir una traza suelta y seguir en el raíl | `_logger.LogMlResultXxx("mensaje")` → `MlResult<ILogger>` |
| Registrar dentro de una cadena `async` | La sobrecarga `…Async` que recibe `Task<MlResult<T>>` |

---

## Mejores prácticas

1. **Prefiere las extensiones por nivel** (`LogMlResultWarningIfFail`) sobre las de `LogLevel` explícito: se leen mejor.
2. **Un `LogMlResultFinal` por caso de uso**, no uno por paso: evita el ruido.
3. **`LogGeneralErrorIfFailAsync` en repositorios y servicios de infraestructura**: coste cero, cobertura total de fallos.
4. **Usa `Func` en lugar de cadenas ya formateadas**: así el texto solo se construye si esa rama se ejecuta.
5. **Mantén los `Func` triviales y a prueba de nulos**: una excepción dentro rompe la cadena de verdad.
6. **Incluye `ex.ToString()` en el mensaje** cuando quieras stack trace: la biblioteca no lo pasa al logger.
7. **`Trace`/`Debug` para el detalle, `Information` para hitos, `Warning` para fallos de negocio, `Error` para técnicos, `Critical` para lo que despierta a alguien.**
8. **No abuses en bucles grandes**: no se comprueba `IsEnabled`, así que el mensaje se construye igualmente.
9. **Si necesitas logging estructurado de verdad**, llama a `ILogger` directamente en un `ExecSelf`/`Match` en lugar de usar estas extensiones.
10. **Decide un idioma para las trazas** y sé coherente: recuerda que `MyMethodFinalLog` fuerza inglés.
11. **`ToErrorsDescription()` para trazas de negocio, `ToErrorsDetailsDescription()` para diagnóstico técnico** (el segundo incluye los `Details`).
12. **No registres información sensible**: los `MlErrorsDetails` pueden arrastrar el valor de entrada completo (`…WithValue`).
13. **Confía en IntelliSense para los nombres de parámetros**: hay erratas (`validBuilMessage`) y una sobrecarga `async` sin sufijo `Async`.

---

## Resumen

- Permite **registrar trazas en medio de un pipeline funcional sin romperlo**: cada método devuelve el `MlResult<T>` recibido, intacto.
- Arquitectura en pirámide: `MyMethodFinalLog` → `LogMlResultFinal` → métodos por nivel → `LogMlResult` con `LogLevel` → `ILogger.Log`.
- **Seis ficheros por nivel** (`Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`), cada uno con el mismo catálogo: variante general, `IfValid`, `IfFail`, `IfFailWithValue`, `IfFailWithException`, `IfFailWithoutException`, y todas sus parejas `Async`.
- Lo más distintivo: **filtrar el log según el contenido de `Details`**, lo que permite separar errores técnicos de errores de negocio sin un solo `if`.
- Atajos de alto nivel: **`LogGeneralErrorIfFail`** (una línea, registra los fallos en `Error`) y **`LogMlResultFinal`** (`Information` si va bien, `Error` si falla).
- ⚠️ Límites importantes: **no hay logging estructurado**, **no se pasa la `Exception` al logger** (ni stack trace), **no se comprueba `IsEnabled`**, y las excepciones dentro de tus `Func` **no están protegidas**.
- ⚠️ Erratas del código: el parámetro **`validBuilMessage`** (sin `d`) en algunas sobrecargas, una sobrecarga `async` llamada **`LogMlResultFinal`** sin sufijo, y una región mal etiquetada en el fichero de `Trace`.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.EFCore`](../MoralesLarios.OOFP.EFCore/README.md) — repositorios funcionales donde encadenar los logs
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — servicios genéricos, punto natural para `LogMlResultFinal`
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — conversión de `MlResult` en respuestas HTTP
- [`MoralesLarios.OOFP.HttpClients`](../MoralesLarios.OOFP.HttpClients/README.md) — llamadas HTTP funcionales, otro buen sitio para trazar
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación cuyos fallos conviene registrar en `Warning`

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores, detalles, `GetDetailException`, `GetDetailValue`](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`Match` — la base del log condicional](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`ExecSelf` — efectos secundarios sin cambiar el resultado](../MoralesLarios.FOOP/__Doc/ExecSelf/1_ExecSelf.md)
- [`FusionErrosIfExists` y bucles funcionales](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)
