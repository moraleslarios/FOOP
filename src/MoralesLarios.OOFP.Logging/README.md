# MoralesLarios.OOFP.Logging — Logging sin romper la cadena

Extensiones que permiten **registrar trazas en medio de un pipeline funcional** sin interrumpirlo. Cada método recibe un [`MlResult<T>`](../docs/core-types/MlResult.md), escribe en el `ILogger` y **devuelve el mismo `MlResult<T>` intacto**, de modo que la cadena continúa exactamente igual que si el log no existiera.

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
8. [Familia 4 — `LogCompletedResult` y `LogGeneralErrorIfFail`](#familia-4--logcompletedresult-y-loggeneralerroriffail)
9. [Familia 5 — Los seis ficheros por nivel](#familia-5--los-seis-ficheros-por-nivel)
10. [Familia 6 — `LogActionResult`](#familia-6--logactionresult)
11. [Familia 7 — `EventId` e `ILogger<TCategory>`](#familia-7--eventid-e-iloggertcategory)
12. [Variantes síncronas y asíncronas](#variantes-síncronas-y-asíncronas)
13. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
14. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
15. [Ejemplos prácticos](#ejemplos-prácticos)
16. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
17. [Mejores prácticas](#mejores-prácticas)
18. [Resumen](#resumen)
19. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

En un pipeline funcional, meter un log obliga normalmente a **partir la cadena**: guardar el resultado en una variable, mirar si es válido, escribir la traza y seguir.

❌ **Sin estas extensiones:**

```csharp
public async Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
{
    var validado = dto.Validate();
    if (validado.IsFailure)
    {
        _logger.LogWarning($"Validación fallida: {validado.FailureDetails.GetErrorsSummary()}");
        return validado.ToMlResultFail<Pedido>();       // hay que reconstruir el Fail
    }

    var guardado = await _repo.AddAsync(Mapear(validado.Value));
    if (guardado.IsFailure)
    {
        _logger.LogError($"Error al guardar: {guardado.FailureDetails.GetErrorsSummary()}");
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
          .LogMlResultWarningIfFail(_logger, e => $"Validación fallida: {e.GetErrorsSummary()}")
          .BindAsync(v => _repo.AddAsync(Mapear(v)))
          .LogCompletedResultAsync(_logger,
                                 validBuildMessage: p => $"Pedido {p.Id} creado",
                                 failBuildMessage : e => $"Error al crear pedido: {e.GetErrorsSummary()}");
```

> 💡 **La clave**: el log es **un eslabón transparente**. No cambia el tipo, no cambia el valor, no cambia el estado. Puedes insertarlo o quitarlo de cualquier punto de la cadena sin tocar nada más.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| `Microsoft.Extensions.Logging.Abstractions` (8.0.1) | `ILogger`, `LogLevel` |
| [`MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md) | `MlResult<T>`, `MlErrorDetails`, `Match`, `Bind` |

Destino: **`net8.0`**. Versión del paquete: **2.2.0**.

```csharp
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Logging;   // 🔑 todas las extensiones viven aquí
```

> 💡 **Un único namespace para todo el proyecto**: `MoralesLarios.OOFP.Logging`. No hay subnamespace `.Helpers` como en otros proyectos de la solución.

**No requiere registro en el contenedor de dependencias.** Solo necesitas un `ILogger` (inyectado como `ILogger<T>` en tus servicios, o creado con `ILoggerFactory`).

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Logging/
├── GlobalUsings.cs
├── MlResultLogGuards.cs                        → guardas internas: logger nulo + IsEnabled
├── MlResultLogExtensions.cs                    → núcleo: LogLevel como parámetro
├── MlResultLogExtensions.Trace.cs              → nivel Trace fijado
├── MlResultLogExtensions.Debug.cs              → nivel Debug fijado
├── MlResultLogExtensions.Information.cs        → nivel Information fijado
├── MlResultLogExtensions.Warning.cs            → nivel Warning fijado
├── MlResultLogExtensions.Error.cs              → nivel Error fijado
├── MlResultLogExtensions.Critical.cs           → nivel Critical fijado
├── MlResultLogExtensions.EventId.cs            → sobrecargas con EventId
├── MlResultLogExtensions.Categoria.cs          → sobrecargas con ILogger<TCategory>
├── MlResultLogExtensions.Obsolete.cs           → puentes [Obsolete] de LogMlResultFinal
├── MlResultLogActionExtensions.cs              → LogActionResult (atajo de más alto nivel)
└── MlResultLogActionExtensions.Obsolete.cs     → puente [Obsolete] de MyMethodFinalLog
```

Las seis clases por nivel y las de `EventId`/`Categoria` son **`partial` de la misma clase
`MlResultLogExtensions`**: se reparten en ficheros por legibilidad, pero para el consumidor es un
único tipo. Todo son **métodos de extensión estáticos**. La arquitectura es una pirámide:

```
MlResultLogActionExtensions.LogActionResult     ← atajo con mensajes prefabricados
        ↓
MlResultLogExtensions.LogCompletedResult        ← Information si válido / Error si fallo
        ↓
MlResultLogExtensions.LogMlResultXxx…           ← LogLevel fijado por fichero
        ↓
MlResultLogExtensions.LogMlResult…              ← LogLevel como parámetro
        ↓
MlResultLogGuards.IsEnabled(logger, logLevel)   ← corta antes de construir el mensaje
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
    if (logger is null) return source;                              // 🔑 guarda: nunca lanza
    if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return source;

    logger.Log(logLevel, message);
    return source;              // 🔑 el resultado sale intacto
}
```

Y la variante con constructores de mensaje:

```csharp
public static MlResult<T> LogMlResult<T>(this MlResult<T>                   source,
                                              ILogger                       logger,
                                              LogLevel                      logLevel,
                                              Func<T, string>               validBuildMessage = null!,
                                              Func<MlErrorDetails, string> failBuildMessage  = null!)
{
    if (! MlResultLogGuards.IsEnabled(logger, logLevel)) return source;   // 🔑 pereza

    source.Match(
        valid: x      => validBuildMessage != null ? logger.LogMlResult(logLevel, validBuildMessage(x)) : null!,
        fail : errors => failBuildMessage  != null ? logger.LogMlResult(logLevel, failBuildMessage(errors)) : null!)
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
| **Un `logger` nulo ⇒ devuelve `source` intacto** | Nunca lanza `NullReferenceException` (desde la 2.2.0) |
| **Si el nivel está apagado ⇒ ni se construye el mensaje** | `IsEnabled` se consulta **antes** de invocar tu `Func` (desde la 2.2.0) |

> 💡 **La evaluación diferida es importante para el rendimiento**: `e => $"Error: {e.GetErrorsSummary()}"` solo construye la cadena si realmente hay fallo.

> 💡 **Y desde la 2.2.0 no se construye en absoluto si el nivel está filtrado.** Medido sobre
> `LogMlResultTrace` con `Trace` apagado: **240,0 B** por llamada frente a los **792,6 B** de antes
> (**−69,7 %** de asignaciones y **−93 %** de tiempo). Ya puedes dejar trazas `Trace`/`Debug` en
> bucles calientes sin pagarlas en producción.

Además existe la variante que registra **cada rama en un nivel distinto** usando tuplas:

```csharp
public static MlResult<T> LogMlResult<T>(this MlResult<T> source,
                                              ILogger     logger,
                                              (LogLevel logLevel, Func<T              , string> buildMessage) validBuildMessage,
                                              (LogLevel logLevel, Func<MlErrorDetails, string> buildMessage) failBuildMessage);
```

```csharp
resultado.LogMlResult(_logger,
                      validBuildMessage: (LogLevel.Debug, p => $"Pedido {p.Id} procesado"),
                      failBuildMessage : (LogLevel.Error, e => $"Fallo: {e.GetErrorsSummary()}"));
```

> ✅ **El nombre del parámetro es uniforme**: `validBuildMessage` / `failBuildMessage` en **todas** las
> sobrecargas. Hasta la 2.1.1 la sobrecarga con `LogLevel` suelto lo llamaba `validBuilMessage` (sin la
> `d`) y la de tuplas `faildMessage`; ambas erratas se corrigieron en la 2.2.0. Ver
> [Particularidades](#️-particularidades-reales-del-código-fuente).

---

## Familia 1 — `LogMlResult` con `LogLevel` explícito

En `MlResultLogExtensions.cs`. El nivel es siempre un parámetro.

| Firma | Cuándo registra |
|---|---|
| `logger.LogMlResult(logLevel, message)` → `MlResult<ILogger>` | Siempre. Punto de entrada de bajo nivel |
| `source.LogMlResult(logger, logLevel, message)` | Siempre, con mensaje fijo |
| `source.LogMlResult(logger, logLevel, validMessage, failMessage)` | Siempre, mensaje distinto según estado |
| `source.LogMlResult(logger, logLevel, validBuildMessage, failBuildMessage)` | Según qué `Func` pases (los `null` no registran) |
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
source.LogMlResultIfFail(logger, logLevel, e => $"Fallo: {e.GetErrorsSummary()}");
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

Estas tres familias son las más específicas de la biblioteca: **deciden si registrar según lo que haya en [`MlErrorDetails.Metadata`](../docs/core-types/MlResultErrors.md)**.

### `LogMlResultIfFailWithException` — solo si el fallo trae excepción

```csharp
public static MlResult<T> LogMlResultIfFailWithException<T>(this MlResult<T>                   source,
                                                                 ILogger                       logger,
                                                                 LogLevel                      logLevel,
                                                                 Func<MlErrorDetails, string> failBuildMessage)
    => source.Match(
            valid: x             => source,
            fail : errorsDetails => errorsDetails.GetException()
                                        .Bind(_ => source.LogMlResult(logger, logLevel, failBuildMessage(errorsDetails)))
        );
```

Como `GetException()` devuelve `Fail` cuando **no** hay excepción registrada, el `Bind` no se ejecuta y **no se registra nada**.

> 💡 **Uso típico**: separar errores técnicos (excepción capturada por un `TryMap`) de errores de negocio (una validación incumplida). Los primeros merecen `LogLevel.Error` con la excepción; los segundos, `LogLevel.Warning` sin ruido.

Existe además una sobrecarga que **te da la excepción** en el constructor del mensaje:

```csharp
source.LogMlResultIfFailWithException(_logger, LogLevel.Error,
        (errores, ex) => $"Fallo técnico: {errores.GetErrorsSummary()} | {ex.GetType().Name}: {ex.Message}");
```

### `LogMlResultIfFailWithoutException` — solo si el fallo **no** trae excepción

El complementario exacto. Usa `BindIfFail`, así que registra cuando `GetException()` falla:

```csharp
source.LogMlResultIfFailWithoutException(_logger, LogLevel.Warning,
        e => $"Error de negocio: {e.GetErrorsSummary()}");
```

> 💡 **Combinando las dos** obtienes una clasificación completa sin un solo `if`:
> ```csharp
> resultado
>     .LogMlResultIfFailWithException   (_logger, LogLevel.Error  , (e, ex) => $"Técnico: {ex.Message}")
>     .LogMlResultIfFailWithoutException(_logger, LogLevel.Warning, e       => $"Negocio: {e.GetErrorsSummary()}");
> ```

### `LogMlResultIfFailWithValue` — solo si el fallo arrastra un valor

Cuando el `Fail` conserva el valor original en los detalles (patrón habitual de `…WithValue` del núcleo):

```csharp
// El valor debe ser del mismo tipo T
source.LogMlResultIfFailWithValue(_logger, LogLevel.Error,
        e => $"Falló procesando el elemento: {e.GetErrorsSummary()}");

// O de un tipo distinto, y lo recibes en el mensaje
source.LogMlResultIfFailWithValue<Pedido, int>(_logger, LogLevel.Error,
        (errores, idFila) => $"Falló la fila {idFila}: {errores.GetErrorsSummary()}");
```

---

## Familia 4 — `LogCompletedResult` y `LogGeneralErrorIfFail`

### `LogCompletedResult` — la convención "éxito = Information, fallo = Error"

```csharp
public static MlResult<T> LogCompletedResult<T>(this MlResult<T>                   source,
                                                   ILogger                       logger,
                                                   Func<T, string>               validBuildMessage,
                                                   Func<MlErrorDetails, string> failBuildMessage)
    => source.LogMlResult(logger,
                          (LogLevel.Information, validBuildMessage),
                          (LogLevel.Error      , failBuildMessage));
```

Es el método que querrás **al cerrar un caso de uso**: un único log que dice cómo terminó todo.

```csharp
await pipeline.LogCompletedResultAsync(_logger,
        validBuildMessage: p => $"Pedido {p.Id} creado correctamente",
        failBuildMessage : e => $"No se pudo crear el pedido: {e.GetErrorsSummary()}");
```

También acepta cadenas fijas en lugar de `Func`.

### `LogGeneralErrorIfFail` — el atajo mínimo

```csharp
public static MlResult<T> LogGeneralErrorIfFail<T>(this MlResult<T> source, ILogger logger)
    => source.LogMlResultErrorIfFail(logger, failBuildMessage: errorDetals => errorDetals.GetErrorsSummary());
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
| `MlResultLogExtensions.cs` | `LogMlResultTrace…` | `Trace` |
| `MlResultLogExtensions.cs` | `LogMlResultDebug…` | `Debug` |
| `MlResultLogExtensions.cs` | `LogMlResultInformation…` | `Information` |
| `MlResultLogExtensions.cs` | `LogMlResultWarning…` | `Warning` |
| `MlResultLogExtensions.cs` | `LogMlResultError…` | `Error` |
| `MlResultLogExtensions.cs` | `LogMlResultCritical…` | `Critical` |

Y dentro de cada fichero, estas variantes (sustituye `Xxx` por el nivel):

| Método | Registra si… |
|---|---|
| `logger.LogMlResultXxx(message)` | Siempre (devuelve `MlResult<ILogger>`) |
| `source.LogMlResultXxx(logger, message)` | Siempre |
| `source.LogMlResultXxx(logger, validMessage, failMessage)` | Siempre, texto según estado |
| `source.LogMlResultXxx(logger, validBuildMessage, failBuildMessage)` | Según los `Func` no nulos |
| `source.LogMlResultXxxIfValid(logger, …)` | Solo si es válido |
| `source.LogMlResultXxxIfFail(logger, …)` | Solo si es fallo |
| `source.LogMlResultXxxIfFailWithValue(logger, …)` | Fallo **con valor** en `Details` |
| `source.LogMlResultXxxIfFailWithException(logger, …)` | Fallo **con excepción** en `Details` |
| `source.LogMlResultXxxIfFailWithoutException(logger, …)` | Fallo **sin excepción** en `Details` |

Todas tienen su pareja `…Async`.

> 💡 **Usa siempre las versiones por nivel** en código de aplicación: `LogMlResultWarningIfFail(_logger, "…")` se lee mucho mejor que `LogMlResultIfFail(_logger, LogLevel.Warning, "…")`. Reserva las de `LogLevel` explícito para cuando el nivel sea una variable.

> ⚠️ **`LogCompletedResult` y `LogGeneralErrorIfFail` solo existen en `MlResultLogExtensions.cs`**, no tienen versión por nivel (no la necesitan: ya llevan el nivel decidido).

---

## Familia 6 — `LogActionResult`

En `MlResultLogActionExtensions.cs`. Es el atajo de más alto nivel: **tú solo dices qué estaba haciendo el método**, y él construye los dos mensajes.

```csharp
public static MlResult<T> LogActionResult<T>(this MlResult<T> source,
                                                  ILogger     logger,
                                                  string      methodActionDesc)
    => source.LogCompletedResult(logger,
            validBuildMessage: item   => $"{methodActionDesc} done correctly.",
            failBuildMessage : errors => $"Error when {methodActionDesc}: {errors.ToErrorsDetailsDescription()}");
```

```csharp
return await _repo.AddAsync(pedido)
                  .LogActionResultAsync(_logger, "creating order");
// Válido → Information: "creating order done correctly."
// Fallo  → Error      : "Error when creating order: <detalle completo>"
```

| Método | Uso |
|---|---|
| `LogActionResult(logger, methodActionDesc)` | Versión síncrona |
| `LogActionResultAsync(logger, methodActionDesc)` | Sobre `Task<MlResult<T>>` |

> ⚠️ **Renombrado en la 2.2.0.** Antes esta familia vivía en `Extensions.cs` y se llamaba
> `MyMethodFinalLog` / `MyMethodFinalLogAsync`. Los nombres antiguos siguen existiendo en
> `MlResultLogActionExtensions.Obsolete.cs` marcados con `[Obsolete(error: false)]`, así que tu código
> **sigue compilando con un aviso `CS0618`**, pero desaparecen en la 3.0.0. Consulta el
> [prompt de migración](../docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md).

> ⚠️ **Los mensajes están en inglés y son fijos**, por decisión de diseño: todo el texto que genera la
> librería se emite en inglés. Si tu aplicación registra en español, usa `LogCompletedResult`
> directamente con tus propios `Func`.

> 💡 Nótese que usa `ToErrorsDetailsDescription()` (errores **y** detalles), mientras que `LogGeneralErrorIfFail` usa `GetErrorsSummary()` (solo los mensajes). El primero es más verboso y más útil para diagnóstico.

---

## Familia 7 — `EventId` e `ILogger<TCategory>`

Dos ejes de sobrecargas añadidos en la **2.2.0**, ambos **puramente aditivos**: no sustituyen a nada.

### `EventId` — correlacionar trazas

Todas las sobrecargas de `LogMlResult` (y sus variantes por nivel) aceptan un `EventId` **después** del
`LogLevel`. Están en `MlResultLogExtensions.EventId.cs`.

```csharp
private static readonly EventId PedidoCreado = new(1001, nameof(PedidoCreado));

return await _repo.AddAsync(pedido)
                  .LogMlResultInformationAsync(_logger, PedidoCreado,
                                               validBuildMessage: p => $"Pedido {p.Id} creado");
```

El `EventId` llega al `ILogger` tal cual (`logger.Log(logLevel, eventId, message)`), así que **Serilog,
Seq o Application Insights pueden filtrar y agrupar por él**. Es la forma recomendada de correlacionar
un conjunto de trazas sin recurrir a `BeginScope`.

### `ILogger<TCategory>` — fijar la categoría

Las mismas familias, pero recibiendo un **registrador tipado** `ILogger<TCategory>` en lugar de un
`ILogger` pelado. No cambian el comportamiento: el compilador elige estas sobrecargas cuando el
argumento ya es tipado (la conversión es de identidad) y las anteriores cuando no lo es, así que el
`TCategory` queda **inferido** y no hay que escribirlo. Están en `MlResultLogExtensions.Categoria.cs`.

```csharp
// 'logger' es el ILogger<ServicioPedidos> que inyecta el contenedor:
// no hace falta ningún cast a ILogger ni escribir los parámetros de tipo.
source.LogMlResult(_logger, LogLevel.Information,
                   validBuildMessage: p      => $"Pedido {p.Id} creado",
                   failBuildMessage : errors => errors.GetErrorsSummary());
```

| Eje | Fichero | Firmas públicas |
|---|---|---|
| `EventId` | `MlResultLogExtensions.EventId.cs` | 35 |
| `ILogger<TCategory>` | `MlResultLogExtensions.Categoria.cs` | 21 |

> 💡 El eje de `EventId` cubre la familia genérica `LogMlResult` y las seis de nivel; el de categoría,
> lo mismo. **Los dos ejes no se combinan entre sí**, para no multiplicar la superficie pública.

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

> ✅ **Convenio de nomenclatura `Async`**: un método lleva el sufijo `Async` cuando devuelve
> `Task<…>`. La única excepción histórica es la sobrecarga de `LogCompletedResult` que recibe
> `Task<MlResult<T>>` y se dejó **sin** sufijo para no romper a los consumidores; ver el punto 2 de
> [Particularidades](#️-particularidades-reales-del-código-fuente).

---

## ⚠️ Particularidades reales del código fuente

### 1. La errata `validBuilMessage` se corrigió en la 2.2.0

Hasta la 2.1.1, las sobrecargas con `LogLevel` suelto llamaban al parámetro **`validBuilMessage`**
(sin la `d`), mientras que su pareja se llamaba `faildMessage`. **Desde la 2.2.0 el nombre es uniforme
en toda la superficie pública**: `validBuildMessage` y `failBuildMessage`.

```csharp
// ✅ Desde la 2.2.0, en TODAS las sobrecargas
source.LogMlResult(_logger, LogLevel.Information,
                   validBuildMessage: x => $"OK {x}",
                   failBuildMessage : e => $"KO {e}");
```

> ⚠️ **Ruptura.** Si invocabas con argumentos con nombre, hay que renombrar `validBuilMessage` →
> `validBuildMessage` (107 ocurrencias en la librería) y `faildMessage` → `failBuildMessage` (84).
> El compilador lo detecta: `CS1739`. Está registrada en el
> [anexo de renombrados](../docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md).

### 2. Hay una sobrecarga `async` que no se llama `Async`

```csharp
public static async Task<MlResult<T>> LogCompletedResult<T>(this Task<MlResult<T>> sourceAsync,
                                                               ILogger           logger,
                                                               string            validMessage,
                                                               string            failMessage)
```

Recibe una `Task<MlResult<T>>` y es `async`, pero **se llama `LogCompletedResult`, sin sufijo `Async`**. Funciona correctamente; solo rompe la convención de nombres del resto del proyecto.

### 3. `MlResultLogExtensions.Trace.cs` tiene una región mal etiquetada

La primera región del fichero de `Trace` se llama `#region LogMlResultCritical` (copia-pega desde el fichero de `Critical`). **Los métodos sí usan `LogLevel.Trace`**: solo el comentario está equivocado. No afecta al comportamiento. Se conserva a la espera de la 3.0.0 para no mezclar cambios cosméticos con la poda de los puentes `[Obsolete]`.

### 4. Un `logger` nulo devuelve el `source` intacto, no un `Fail`

Desde la 2.2.0 **sí se valida el `ILogger`**: hay siete guardas que, ante un `logger` nulo, **devuelven el resultado de origen sin tocarlo y sin registrar nada**, en lugar de propagar una `NullReferenceException`.

```csharp
internal const string NullLoggerMessage = "El parámetro 'logger' no puede ser nulo.";
```

La decisión (`D-06`) fue **no convertir el logging en una fuente de fallos**: que falte el registrador no debe hacer fracasar el caso de uso. Si prefieres enterarte, `MlResultLogGuards.NullLoggerFail()` produce el `Fail` correspondiente, pero no se usa en la ruta por defecto.

> 💡 **Ruptura silenciosa**: el código que dependía de la `NullReferenceException` para detectar un doble mal configurado en tests ya no la recibe. Ahora el test pasa y simplemente no hay trazas.

### 5. Una excepción dentro de tu `Func` rompe la cadena

```csharp
source.LogMlResultIfValid(_logger, LogLevel.Debug, x => $"Id: {x.Cliente.Nombre}");
//                                                        ↑ si Cliente es null → 💥
```

Los constructores de mensaje **no están envueltos en `TryMap`**. Una excepción al formatear el texto **sale del pipeline como excepción**, no como `Fail`. Mantén los `Func` triviales y a prueba de nulos.

### 6. El `Match` interno descarta su resultado

En `LogMlResult` con `Func`, la llamada a `source.Match(...)` **no se asigna a nada**: se ejecuta solo por su efecto secundario (escribir el log) y luego se devuelve `source`. Es intencionado — así se garantiza la transparencia —, pero explica por qué las ramas pueden devolver `null!` sin consecuencias.

### 7. Se usa `logger.Log(logLevel, message)`, no logging estructurado

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

> ⚠️ **No hay `scopes` ni `BeginScope`.** Para correlacionar un conjunto de trazas, usa el `EventId` de
> la [familia 7](#familia-7--eventid-e-iloggertcategory), que **sí existe desde la 2.2.0**.

> ⚠️ **No existe `LogCompletedResult` ni `LogGeneralErrorIfFail` por nivel** (`LogMlResultErrorFinal`, etc.).

> ⚠️ **No hay `IfValidWithValue`** ni variantes de filtrado por contenido en la rama válida: el filtrado por `Details` solo aplica a fallos.

> ⚠️ **No incluye `RegisterServices`.** A diferencia de [`EntityFrameworkCore`](../MoralesLarios.OOFP.EntityFrameworkCore/README.md), aquí no hay nada que registrar.

---

## Ejemplos prácticos

### Ejemplo 1 — Trazabilidad completa de un caso de uso

```csharp
using Microsoft.Extensions.Logging;
using MoralesLarios.OOFP.Logging;
using MoralesLarios.OOFP.Types;

public class ServicioPedidos
{
    private readonly ILogger<ServicioPedidos> _logger;
    private readonly IPedidosRepo             _repo;

    public Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
        => dto.Validate()
              .LogMlResultDebugIfValid  (_logger, d => $"DTO válido para cliente {d.ClienteId}")
              .LogMlResultWarningIfFail (_logger, e => $"DTO inválido: {e.GetErrorsSummary()}")
              .Map                      (Mapear)
              .BindAsync                (p => _repo.AddAsync(p))
              .LogCompletedResultAsync    (_logger,
                                         validBuildMessage: p => $"Pedido {p.Id} creado",
                                         failBuildMessage : e => $"No se pudo crear el pedido: {e.GetErrorsSummary()}");
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
                               errores.GetErrorsSummary());
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
        failBuildMessage : (LogLevel.Information, e => $"Caché no disponible, se recalcula: {e.GetErrorsSummary()}"));
```

> 💡 Aquí el "fallo" no es grave (un fallo de caché), así que se registra en `Information` y no en `Error`. Con `LogCompletedResult` no podrías hacerlo, porque fija `Error`.

### Ejemplo 5 — Log dentro de un bucle sobre una colección

```csharp
public MlResult<IEnumerable<Fila>> ProcesarFichero(IEnumerable<Fila> filas)
    => filas.Select((fila, i) =>
                Procesar(fila)
                    .AddMlErrorDetailIfFail("fila", i + 1)
                    .LogMlResultWarningIfFail(_logger,
                        e => $"Fila {i + 1} descartada: {e.GetErrorsSummary()}"))
            .MergeErrorsIfExists();
```

> ⚠️ Con miles de filas esto genera miles de trazas. Considera registrar solo el resumen final, o usar `LogMlResultTraceIfFail` para poder filtrarlas por configuración.

### Ejemplo 6 — Traza de entrada y salida de un método

```csharp
public Task<MlResult<Informe>> Generar(int año)
{
    _logger.LogMlResultDebug($"Generando informe del año {año}");   // sobre ILogger, sin MlResult

    return _datos.CargarAsync(año)
                 .BindAsync(d => _motor.CalcularAsync(d))
                 .LogActionResultAsync(_logger, $"generating report for {año}");
}
```

### Ejemplo 7 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: esperar logging estructurado
source.LogMlResultInformation(_logger, "Pedido {Id} creado", pedido.Id);   // 💥 no existe

// ✅ BIEN: interpolar (aceptando que se pierde el campo indexable)
source.LogMlResultInformation(_logger, $"Pedido {pedido.Id} creado");


// ❌ MAL: el nombre con la errata de la 2.1.1 ya no existe
source.LogMlResult(_logger, LogLevel.Debug, validBuilMessage: x => $"{x}");   // 💥 CS1739

// ✅ BIEN: el nombre uniforme de la 2.2.0
source.LogMlResult(_logger, LogLevel.Debug, validBuildMessage: x => $"{x}");

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
if (r.IsFailure) _logger.LogError(r.FailureDetails.GetErrorsSummary());
return r;

// ✅ BIEN: log transparente
return await pipeline.LogGeneralErrorIfFailAsync(_logger);
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Registrar todos los fallos con el mínimo código | `LogGeneralErrorIfFailAsync(_logger)` |
| Un único log de cierre de caso de uso | `LogCompletedResultAsync(_logger, funcValid, funcFail)` |
| Lo mismo con mensajes prefabricados (en inglés) | `LogActionResultAsync(_logger, "doing X")` |
| Registrar solo cuando va bien | `LogMlResultXxxIfValid(_logger, …)` |
| Registrar solo cuando falla | `LogMlResultXxxIfFail(_logger, …)` |
| Separar fallo técnico de fallo de negocio | `…IfFailWithException` + `…IfFailWithoutException` |
| Registrar el valor que arrastra el fallo | `LogMlResultXxxIfFailWithValue(_logger, …)` |
| Nivel distinto en la rama válida y en la de fallo | `LogMlResult(_logger, (nivel, func), (nivel, func))` |
| Nivel decidido en tiempo de ejecución | `LogMlResult(_logger, logLevel, …)` |
| Escribir una traza suelta y seguir en el raíl | `_logger.LogMlResultXxx("mensaje")` → `MlResult<ILogger>` |
| Registrar dentro de una cadena `async` | La sobrecarga `…Async` que recibe `Task<MlResult<T>>` |
| Correlacionar un conjunto de trazas | La sobrecarga con `EventId` ([familia 7](#familia-7--eventid-e-iloggertcategory)) |
| Fijar la categoría del logger en el punto de uso | La sobrecarga con `ILogger<TCategory>` |

---

## Mejores prácticas

1. **Prefiere las extensiones por nivel** (`LogMlResultWarningIfFail`) sobre las de `LogLevel` explícito: se leen mejor.
2. **Un `LogCompletedResult` por caso de uso**, no uno por paso: evita el ruido.
3. **`LogGeneralErrorIfFailAsync` en repositorios y servicios de infraestructura**: coste cero, cobertura total de fallos.
4. **Usa `Func` en lugar de cadenas ya formateadas**: así el texto solo se construye si esa rama se ejecuta.
5. **Mantén los `Func` triviales y a prueba de nulos**: una excepción dentro rompe la cadena de verdad.
6. **Incluye `ex.ToString()` en el mensaje** cuando quieras stack trace: la biblioteca no lo pasa al logger.
7. **`Trace`/`Debug` para el detalle, `Information` para hitos, `Warning` para fallos de negocio, `Error` para técnicos, `Critical` para lo que despierta a alguien.**
8. **No temas los bucles grandes**: desde la 2.2.0 se consulta `IsEnabled` **antes** de invocar tus `Func`, así que con el nivel apagado el mensaje **ni se construye** (−69,7 % de asignaciones, −93 % de tiempo).
9. **Si necesitas logging estructurado de verdad**, llama a `ILogger` directamente en un `ExecSelf`/`Match` en lugar de usar estas extensiones.
10. **Decide un idioma para las trazas** y sé coherente: recuerda que `LogActionResult` fuerza inglés, igual que todo el texto que genera la librería.
11. **`GetErrorsSummary()` para trazas de negocio, `ToErrorsDetailsDescription()` para diagnóstico técnico** (el segundo incluye los `Details`).
12. **No registres información sensible**: los `MlErrorDetails` pueden arrastrar el valor de entrada completo (`…WithValue`).
13. **Usa `validBuildMessage` / `failBuildMessage`**: desde la 2.2.0 los nombres son uniformes en toda la superficie pública; los antiguos (`validBuilMessage`, `faildMessage`) ya no compilan.
14. **Usa un `EventId` estático por tipo de suceso** en lugar de literales: así puedes filtrar y agrupar en Seq o Application Insights.

---

## Resumen

- Permite **registrar trazas en medio de un pipeline funcional sin romperlo**: cada método devuelve el `MlResult<T>` recibido, intacto.
- Arquitectura en pirámide: `LogActionResult` → `LogCompletedResult` → métodos por nivel → `LogMlResult` con `LogLevel` → `MlResultLogGuards.IsEnabled` → `ILogger.Log`.
- **Seis ficheros por nivel** (`Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`), cada uno con el mismo catálogo: variante general, `IfValid`, `IfFail`, `IfFailWithValue`, `IfFailWithException`, `IfFailWithoutException`, y todas sus parejas `Async`.
- Lo más distintivo: **filtrar el log según el contenido de `Details`**, lo que permite separar errores técnicos de errores de negocio sin un solo `if`.
- Atajos de alto nivel: **`LogGeneralErrorIfFail`** (una línea, registra los fallos en `Error`) y **`LogCompletedResult`** (`Information` si va bien, `Error` si falla).
- Desde la 2.2.0: **`EventId`** y **`ILogger<TCategory>`** como ejes aditivos, **guarda de `logger` nulo** y **cortocircuito por `IsEnabled`** antes de construir el mensaje.
- ⚠️ Límites importantes: **no hay logging estructurado**, **no se pasa la `Exception` al logger** (ni stack trace), **no hay `scopes`**, y las excepciones dentro de tus `Func` **no están protegidas**.
- ⚠️ Lo que queda por limpiar en la 3.0.0: una sobrecarga `async` llamada **`LogCompletedResult`** sin sufijo, una región mal etiquetada en el fichero de `Trace`, y los puentes **`[Obsolete]`** de `MyMethodFinalLog` y `LogMlResultFinal`.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md)
- [Introducción general al núcleo funcional](../docs/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.EntityFrameworkCore`](../MoralesLarios.OOFP.EntityFrameworkCore/README.md) — repositorios funcionales donde encadenar los logs
- [`MoralesLarios.OOFP.Application`](../MoralesLarios.OOFP.Application/README.md) — servicios genéricos, punto natural para `LogCompletedResult`
- [`MoralesLarios.OOFP.AspNetCore.Results`](../MoralesLarios.OOFP.AspNetCore.Results/README.md) — conversión de `MlResult` en respuestas HTTP
- [`MoralesLarios.OOFP.Http`](../MoralesLarios.OOFP.Http/README.md) — llamadas HTTP funcionales, otro buen sitio para trazar
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación cuyos fallos conviene registrar en `Warning`

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../docs/core-types/MlResult.md)
- [`MlErrorDetails` — errores, detalles, `GetException`, `GetMetadataValue`](../docs/core-types/MlResultErrors.md)
- [`Match` — la base del log condicional](../docs/Match/1_Match.md)
- [`Bind` — encadenar operaciones que pueden fallar](../docs/Bind/3_Bind.md)
- [`ExecSelf` — efectos secundarios sin cambiar el resultado](../docs/ExecSelf/1_ExecSelf.md)
- [`MergeErrorsIfExists` y bucles funcionales](../docs/loops/Collections.md)
