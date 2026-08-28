# BindIfFailWithException — Recuperarse según la excepción que causó el fallo

## Índice

1. [Introducción](#introducción)
2. [Cómo llega la excepción a los detalles](#cómo-llega-la-excepción-a-los-detalles)
3. [Las cuatro formas de `BindIfFailWithException`](#las-cuatro-formas-de-bindiffailwithexception)
4. [Firmas reales](#firmas-reales)
5. [Diferencia clave con `BindIfFailWithValue`](#diferencia-clave-con-bindiffailwithvalue)
6. [La familia `BindIfFailWithExceptionError`](#la-familia-bindiffailwithexceptionerror)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [`TryBindIfFailWithException` — cuando la recuperación puede lanzar](#trybindiffailwithexception--cuando-la-recuperación-puede-lanzar)
9. [Ejemplos Prácticos](#ejemplos-prácticos)
10. [Mejores Prácticas](#mejores-prácticas)
11. [Resumen](#resumen)
12. [Ver también](#ver-también)

---

## Introducción

Hay fallos que se tratan igual sea cual sea su causa, y hay fallos cuya reacción correcta **depende de la excepción concreta** que los provocó: un `TimeoutException` merece un reintento, un `HttpRequestException` con 503 merece un *fallback*, y un `ArgumentException` no merece ninguno de los dos porque es un error de programación.

Con `BindIfFail` solo recibes el `MlErrorsDetails`, y para averiguar la causa técnica tendrías que rebuscar dentro de los detalles a mano:

```csharp
// ❌ Rebuscando la excepción a mano dentro del fallo
resultado.BindIfFail(errores =>
{
    var resultadoEx = errores.GetDetailException();

    if (resultadoEx.IsFail) return errores;                  // no había excepción

    var ex = resultadoEx.Match(valid: e => e, fail: _ => null!);

    if (ex is TimeoutException) return ReintentarLectura();
    if (ex is HttpRequestException) return LeerDeCache();

    return errores;
});
```

`BindIfFailWithException` hace ese trabajo por ti: extrae la excepción de los detalles y **solo llama a tu función si realmente hay una**, pasándotela ya tipada.

```csharp
// ✅ La excepción llega directamente como parámetro
resultado.BindIfFailWithException(ex => ex switch
{
    TimeoutException       => ReintentarLectura(),
    HttpRequestException    => LeerDeCache(),
    _                       => ex.Message.ToMlResultFail<Tarifa>()
});
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`; para llegar al valor original, `GetDetailValue<T>()`.

---

## Cómo llega la excepción a los detalles

`BindIfFailWithException` no captura excepciones: **las lee** de `Details`, bajo la clave convencional `EX_DESC_KEY` (`"Ex"`). Esa clave la rellena automáticamente cualquier método de la familia `Try*` cuando captura una excepción:

```csharp
// TryBind captura la excepción y la guarda en Details["Ex"]
var resultado = cliente.ToMlResultValid()
                       .TryBind(c => _api.Consultar(c.Id),        // ← puede lanzar
                                ex => $"Fallo consultando la API: {ex.Message}");

// Ahora el fallo lleva la excepción dentro, y BindIfFailWithException puede leerla
var recuperado = resultado.BindIfFailWithException(ex => LeerDeCache(cliente.Id));
```

También puedes ponerla tú mismo cuando construyes el fallo:

```csharp
try
{
    return _api.Consultar(id);
}
catch (Exception ex)
{
    return MlErrorsDetails.FromErrorMessageWithException("Fallo consultando la API", ex)
                          .ToMlResultFail<Respuesta>();
}
```

| Origen del fallo | ¿Lleva `Details["Ex"]`? | ¿Se ejecuta tu función? |
|---|---|---|
| `TryBind` / `TryMap` / `TryBindIfFail`… que capturó una excepción | Sí | ✅ Sí |
| `MlErrorsDetails.FromErrorMessageWithException(...)` | Sí | ✅ Sí |
| `"mensaje".ToMlResultFail<T>()` (fallo de negocio) | No | ❌ No |
| `MapEnsure` / `EnsureFp.That(...)` (validación) | No | ❌ No |

> 📌 Cuando no hay excepción, el resultado **se devuelve tal cual llegó**: no se añade ningún error nuevo ni se pierde información.

---

## Las cuatro formas de `BindIfFailWithException`

| Forma | Firma resumida | Para qué sirve |
|---|---|---|
| **A — Recuperación simple** | `BindIfFailWithException<T>(funcException)` | Intentar volver al camino válido usando la excepción. Mismo tipo de entrada y salida. |
| **B — Recuperación filtrada por tipo** | `BindIfFailWithException<T, TException>(funcException)` | Igual que A, pero **solo actúa si la excepción es del tipo `TException`**. |
| **C — Ambos caminos** | `BindIfFailWithException<T, TReturn>(funcValid, funcFail)` | Transformar tanto el éxito como el fallo, cambiando de tipo. |
| **D — Ambos caminos filtrados** | `BindIfFailWithException<T, TReturn, TException>(funcValid, funcFail)` | Como C, con el filtro de tipo de excepción. |

```csharp
// Forma A — recuperar sin mirar el tipo
var tarifa = ObtenerTarifaRemota(id)
                .BindIfFailWithException(ex => LeerDeCache(id));

// Forma B — recuperar SOLO si fue un timeout
var tarifa = ObtenerTarifaRemota(id)
                .BindIfFailWithException<Tarifa, TimeoutException>(ex => LeerDeCache(id));

// Forma C — decidir la respuesta final en ambas ramas
IActionResult respuesta = ObtenerTarifaRemota(id)
                .BindIfFailWithException(
                        funcValid: tarifa => Ok(tarifa).ToMlResultValid<IActionResult>(),
                        funcFail : ex     => StatusCode(502, ex.Message).ToMlResultValid<IActionResult>())
                .Match(valid: r => r, fail: e => (IActionResult)BadRequest(e.ToErrorsMessages()));
```

---

## Firmas reales

```csharp
// FORMA A
public static MlResult<T> BindIfFailWithException<T>(this MlResult<T>                  source,
                                                          Func<Exception, MlResult<T>> funcException)
    => source.Match(
            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => source,          // ← sin excepción: devuelve el fallo intacto
                                            valid:      funcException),
            valid: value         => value);

// FORMA C
public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(this MlResult<T>                        source,
                                                                         Func<T        , MlResult<TReturn>> funcValid,
                                                                         Func<Exception, MlResult<TReturn>> funcFail)
    => source.Match(
            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => errorsDetails.ToMlResultFail<TReturn>(),
                                            valid:      funcFail),
            valid: funcValid);

// FORMA D (la B es su equivalente de un solo camino)
public static MlResult<TReturn> BindIfFailWithException<T, TReturn, TException>(this MlResult<T>                         source,
                                                                                     Func<T         , MlResult<TReturn>> funcValid,
                                                                                     Func<TException, MlResult<TReturn>> funcFail)
    where TException : Exception
    => source.Match(
            fail : errorsDetails => errorsDetails.GetDetailException<TException>().Match(
                                            fail : _ => errorsDetails.ToMlResultFail<TReturn>(),
                                            valid:      funcFail),
            valid: funcValid);
```

🔑 El patrón interno es siempre el mismo: **`GetDetailException()` devuelve un `MlResult<Exception>`**, y sobre él se hace un segundo `Match`. Ese doble `Match` es lo que garantiza que tu función de recuperación no se ejecute nunca con una excepción inexistente.

| Estado de entrada | `Details["Ex"]` | Resultado |
|---|---|---|
| Válido | — | Forma A/B: el valor intacto · Forma C/D: `funcValid(value)` |
| Fallo | presente y del tipo esperado | Se ejecuta tu función de recuperación |
| Fallo | ausente | Forma A/B: el fallo intacto · Forma C/D: el fallo convertido a `MlResult<TReturn>` |
| Fallo | presente pero de otro tipo (formas B/D) | Igual que si estuviera ausente: tu función **no** se ejecuta |

---

## Diferencia clave con `BindIfFailWithValue`

El propio código fuente lo documenta con un comentario, y merece la pena repetirlo porque es una fuente habitual de confusión:

| | `BindIfFailWithValue` | `BindIfFailWithException` |
|---|---|---|
| Dato que busca en `Details` | `Details["Value"]` (clave `VALUE_KEY`) | `Details["Ex"]` (clave `EX_DESC_KEY`) |
| Quién lo pone | Tú, con `AddValueIfFail(...)` | Automáticamente cualquier método `Try*` |
| Si el dato **no está** | Añade un error nuevo al fallo que ya venía | **Devuelve el fallo exactamente como llegó** |

> 📌 Es decir: `BindIfFailWithException` es *silencioso* cuando no encuentra excepción. No te avisará de que tu función no se ha ejecutado. Si sospechas que ese es tu caso, diagnostica con `errores.ToDetailsDescription()`.

---

## La familia `BindIfFailWithExceptionError`

Existe una familia gemela con el sufijo **`Error`**. La condición de disparo es idéntica —*solo actúa si hay excepción en los detalles*— pero lo que recibe tu función es distinto:

```csharp
public static MlResult<T> BindIfFailWithExceptionError<T>(this MlResult<T>                   source,
                                                               Func<MlErrorsDetails, MlResult<T>> funcFail)
    => source.Match(
            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => source,
                                            valid: _ => funcFail(errorsDetails)),   // ← recibe los detalles completos
            valid: value         => value);
```

| | Recibe | Cuándo usarla |
|---|---|---|
| `BindIfFailWithException` | `Exception` | Te basta con la causa técnica. |
| `BindIfFailWithExceptionError` | `MlErrorsDetails` completo | Necesitas también los mensajes de error acumulados, otras claves de `Details`, o quieres reenviar el fallo enriquecido. |

```csharp
// Solo reacciona a fallos técnicos, pero conserva TODO el contexto acumulado
var resultado = await ProcesarPagoAsync(dto)
                        .BindIfFailWithExceptionErrorAsync(async errores =>
                        {
                            await _auditoria.RegistrarIncidenciaAsync(errores.ToDetailsDescription());
                            return await EncolarParaReintentoAsync(dto);
                        });
```

Igual que la familia principal, tiene sus cuatro formas: `<T>`, `<T, TException>`, `<T, TReturn>` y `<T, TReturn, TException>`.

---

## Variantes asíncronas

Cada familia ofrece **24 sobrecargas asíncronas**, resultado de combinar tres ejes:

| Eje | Opciones |
|---|---|
| Origen | `MlResult<T>` · `Task<MlResult<T>>` |
| Delegado de recuperación | síncrono · asíncrono |
| Forma | A · B · C · D (y en C/D, `funcValid` síncrono o asíncrono) |

```csharp
public async Task<MlResult<Tarifa>> ObtenerAsync(int id)
    => await _api.ObtenerTarifaAsync(id)                       // Task<MlResult<Tarifa>>
                 .TryBindAsync(t => ValidarVigenciaAsync(t),
                               ex => $"Error validando la tarifa: {ex.Message}")
                 .BindIfFailWithExceptionAsync<Tarifa, TimeoutException>(
                        async ex => await _cache.LeerAsync(id))
                 .ExecSelfIfFailAsync(errores =>
                        _log.LogWarning("Tarifa {Id} no disponible: {Errores}",
                                        id, errores.ToErrorsDescription()));
```

> 💡 Cuando pasas un delegado síncrono a una sobrecarga asíncrona, la librería lo adapta internamente con `func.ToFuncTask()`. No necesitas envolverlo tú en `Task.FromResult`.

---

## `TryBindIfFailWithException` — cuando la recuperación puede lanzar

Tu propio *fallback* suele tocar disco, red o base de datos, así que también puede reventar. Las variantes `Try*` capturan esa segunda excepción y la convierten en un fallo:

```csharp
public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T>                  source,
                                                             Func<Exception, MlResult<T>> funcException,
                                                             Func<Exception, string>      errorMessageBuilder);

public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T>                  source,
                                                             Func<Exception, MlResult<T>> funcException,
                                                             string                       errorMessage = null!);
```

Recuento de sobrecargas:

| Método | Síncronas | Asíncronas |
|---|---|---|
| `BindIfFailWithException` | 4 | 24 |
| `TryBindIfFailWithException` | 8 | 44 |
| `BindIfFailWithExceptionError` | 4 | 24 |
| `TryBindIfFailWithExceptionError` | 8 | 44 |

```csharp
var tarifa = ObtenerTarifaRemota(id)                       // falló con HttpRequestException
                .TryBindIfFailWithException(
                        ex            => LeerDeFicheroLocal(id),   // ← puede lanzar IOException
                        errorMessage  => $"Fallback local también falló: {errorMessage.Message}");
```

> 🔑 El fallo resultante acumula ambos contextos: el mensaje que tú construyes y la nueva excepción en `Details["Ex2"]` (la numeración de claves es `Ex`, `Ex2`, `Ex3`…).

---

## Ejemplos Prácticos

### Ejemplo 1: Política de resiliencia por tipo de excepción

```csharp
public class ServicioTarifas
{
    private readonly ITarifasApi   _api;
    private readonly ITarifasCache _cache;
    private readonly ILogger       _log;

    public MlResult<Tarifa> Obtener(int id)
        => id.ToMlResultValid()
             .MapEnsure(x => x > 0, "El identificador debe ser positivo")
             .TryBind(x => _api.Obtener(x),
                      ex => $"La API de tarifas no respondió: {ex.Message}")

             // 1) Un timeout se reintenta una vez
             .BindIfFailWithException<Tarifa, TimeoutException>(_ =>
                    _api.Obtener(id))

             // 2) Un problema de red cae a la caché
             .TryBindIfFailWithException<Tarifa, HttpRequestException>(
                    _  => _cache.Leer(id),
                    ex => $"La caché de tarifas tampoco está disponible: {ex.Message}")

             // 3) Cualquier otro fallo se registra pero no se recupera
             .ExecSelfIfFail(errores =>
                    _log.LogError("Tarifa {Id} irrecuperable: {Detalle}",
                                  id, errores.ToDetailsDescription()));
}
```

El orden importa: los filtros más específicos primero. Un fallo de negocio (el `MapEnsure`) **no lleva excepción**, así que atraviesa los tres pasos sin que ninguno lo toque.

### Ejemplo 2: Traducir excepciones de infraestructura a códigos HTTP

Este es el caso de uso natural de la **forma C**: convertir el resultado en la respuesta final decidiendo en ambas ramas.

```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> Obtener(int id)
    => await _servicio.ObtenerAsync(id)
                      .BindIfFailWithExceptionAsync<Tarifa, IActionResult>(
                            funcValidAsync: tarifa => Ok(TarifaDto.Desde(tarifa))
                                                        .ToMlResultValidAsync<IActionResult>(),
                            funcFailAsync : ex     => TraducirAsync(ex))
                      .MatchAsync(
                            valid: respuesta => respuesta,
                            // Sin excepción ⇒ fallo de negocio ⇒ 400
                            fail : errores   => BadRequest(errores.ToErrorsMessages()));

private static Task<MlResult<IActionResult>> TraducirAsync(Exception ex)
    => (ex switch
        {
            TimeoutException          => new ObjectResult("Tiempo de espera agotado") { StatusCode = 504 },
            HttpRequestException      => new ObjectResult("Servicio no disponible")   { StatusCode = 502 },
            UnauthorizedAccessException => new ObjectResult("Sin permisos")           { StatusCode = 403 },
            _                         => new ObjectResult("Error interno")            { StatusCode = 500 }
        } as IActionResult)
       .ToMlResultValidAsync();
```

Fíjate en la simetría: **excepción presente ⇒ error técnico ⇒ 5xx/403**; **excepción ausente ⇒ error de negocio ⇒ 400**. Toda la clasificación sale gratis del comportamiento de `BindIfFailWithException`.

### Ejemplo 3: Reintento con espera usando la familia `...Error`

Aquí necesitamos la excepción *y* el contexto acumulado, así que usamos `BindIfFailWithExceptionError`.

```csharp
public async Task<MlResult<Confirmacion>> EnviarAsync(Pedido pedido, int intentos = 3)
    => await EnviarUnaVezAsync(pedido)
                .BindIfFailWithExceptionErrorAsync(async errores =>
                {
                    // El contexto completo va al log, no solo la excepción
                    _log.LogWarning("Intento fallido para el pedido {Id}. Contexto: {Ctx}",
                                    pedido.Id, errores.ToDetailsDescription());

                    if (intentos <= 1)
                        return errores.AddErrorMessage("Agotados todos los reintentos")
                                      .ToMlResultFail<Confirmacion>();

                    await Task.Delay(TimeSpan.FromMilliseconds(200));

                    // Reintento conservando el error original si vuelve a fallar
                    return await EnviarAsync(pedido, intentos - 1)
                                    .MergeErrorsDetailsIfFailAsync(errores);
                });

private Task<MlResult<Confirmacion>> EnviarUnaVezAsync(Pedido pedido)
    => _pasarela.EnviarAsync(pedido)
                .TryBindAsync(r => ValidarAcuseAsync(r),
                              ex => $"La pasarela falló: {ex.Message}");
```

### Ejemplo 4: Diagnóstico — «mi función no se ejecuta»

```csharp
// ❌ El fallo viene de una validación de negocio: NO hay excepción en Details
var r1 = "El importe no puede ser negativo".ToMlResultFail<Factura>()
            .BindIfFailWithException(ex => Recuperar());   // ← Recuperar() nunca se llama

// ✅ Compruébalo antes de dar por buena la recuperación
r1.ExecSelfIfFail(errores =>
    errores.GetDetailException()
           .Match(valid: ex => _log.LogError(ex, "Fallo técnico"),
                  fail : _  => _log.LogWarning("Fallo de negocio: {M}",
                                               errores.ToErrorsMessages())));

// ✅ Si quieres reaccionar a AMBOS tipos de fallo, usa BindIfFail
var r2 = "El importe no puede ser negativo".ToMlResultFail<Factura>()
            .BindIfFail(errores => Recuperar());          // ← ahora sí se ejecuta
```

---

## Mejores Prácticas

1. **Usa el filtro de tipo (`TException`) en lugar de un `switch` dentro de la función.** Declarar `BindIfFailWithException<T, TimeoutException>(...)` documenta la intención en la firma y evita ramas muertas.

2. **Encadena de lo específico a lo genérico.** Cada `BindIfFailWithException` solo actúa si el resultado sigue en fallo, así que la primera política que encaje gana.

3. **No recuperes errores de programación.** `NullReferenceException`, `ArgumentException` o `InvalidCastException` indican un bug: regístralos y déjalos fallar. Reserva la recuperación para `TimeoutException`, `HttpRequestException`, `IOException`, `SqlException`…

4. **Recuerda que necesita un `Try*` antes.** Si el fallo no lo generó un método `Try*` ni un `FromErrorMessageWithException`, no habrá excepción que leer y tu función quedará inerte, sin ningún aviso.

5. **Elige `...Error` cuando el mensaje importa.** Si vas a auditar o reenviar el fallo, `BindIfFailWithExceptionError` te da los mensajes y todas las claves de `Details`, no solo la excepción.

6. **Usa `Try*` si tu *fallback* toca infraestructura.** Un `catch` olvidado en el camino de recuperación es especialmente traicionero: rompe el flujo justo cuando estabas intentando salvarlo.

7. **Conserva el fallo original al reintentar.** `MergeErrorsDetailsIfFail(erroresOriginales)` evita que el diagnóstico final solo muestre el último intento.

---

## Resumen

- `BindIfFailWithException` ejecuta tu función de recuperación **solo si el fallo lleva una excepción** en `Details["Ex"]`, y te la entrega ya extraída.
- Tiene **cuatro formas**: recuperación simple `<T>`, recuperación filtrada `<T, TException>`, ambos caminos `<T, TReturn>` y ambos caminos filtrados `<T, TReturn, TException>`.
- A diferencia de `BindIfFailWithValue`, cuando **no hay** excepción **devuelve el fallo intacto**: no añade ningún error nuevo, pero tampoco te avisa.
- La familia gemela `BindIfFailWithExceptionError` se dispara con la misma condición pero recibe el `MlErrorsDetails` completo.
- Las variantes `Try*` protegen el propio camino de recuperación; la nueva excepción se acumula como `Details["Ex2"]`.
- La presencia o ausencia de excepción es un clasificador natural: **excepción ⇒ error técnico (5xx)**, **sin excepción ⇒ error de negocio (4xx)**.

---

## Ver también

- [`6_BindIfFail.md`](6_BindIfFail.md) — recuperación sin filtrar por causa.
- [`7_BindIfFailWithValue.md`](7_BindIfFailWithValue.md) — recuperación usando el valor de entrada.
- [`9_BindIfFailWithoutException.md`](9_BindIfFailWithoutException.md) — el caso complementario: actuar solo si **no** hay excepción.
- [`3_Bind.md`](3_Bind.md) — el encadenamiento básico y `TryBind`, origen habitual de `Details["Ex"]`.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException`, `GetDetailException<TException>`, `MergeErrorsDetailsIfFail`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — claves convencionales `Ex`, `Ex2`, `Value` y factorías de `MlErrorsDetails`.
- [`../ExecSelf/5_ExecSelfIfFailWithException.md`](../ExecSelf/5_ExecSelfIfFailWithException.md) — el mismo filtro, pero solo para efectos laterales.
- [`../Map/6_MapIfFailWithException.md`](../Map/6_MapIfFailWithException.md) — cuando la recuperación no puede fallar.