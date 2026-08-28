# MapIfFailWithoutException — Recuperarse solo de los fallos de negocio

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [La regla de oro: si hay excepción, no hay recuperación](#la-regla-de-oro-si-hay-excepción-no-hay-recuperación)
4. [Espejo exacto de `MapIfFailWithException`](#espejo-exacto-de-mapiffailwithexception)
5. [Las dos formas de `MapIfFailWithoutException`](#las-dos-formas-de-mapiffailwithoutexception)
6. [Firmas reales e implementación](#firmas-reales-e-implementación)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [`TryMapIfFailWithoutException` — cuando la recuperación puede lanzar](#trymapiffailwithoutexception--cuando-la-recuperación-puede-lanzar)
9. [⚠️ No existe la subfamilia `...Error`](#️-no-existe-la-subfamilia-error)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`MapIfFailWithoutException` es un `MapIfFail` **selectivo**: solo recupera el resultado cuando el fallo **no** lleva una excepción adjunta en `Details["Ex"]`.

La idea de fondo es una distinción que aparece constantemente en aplicaciones reales:

| Tipo de fallo | ¿Lleva excepción en `Details["Ex"]`? | ¿Se puede recuperar de forma segura? |
|---|---|---|
| **Fallo de negocio / validación** (`"El NIF no es válido"`, `"Saldo insuficiente"`) | ❌ No | ✅ Sí: es una decisión esperada del dominio |
| **Fallo técnico** (`SqlException`, `TimeoutException`, `IOException`) | ✅ Sí | ⚠️ Normalmente no: hay que propagarlo y que alguien lo vea |

`MapIfFailWithoutException` implementa exactamente esa política:

```csharp
// ❌ MapIfFail recupera TODO: se traga también el timeout de la base de datos
var resultado = ObtenerDescuento(clienteId)
                    .MapIfFail(_ => 0m);          // ¿era un descuento inexistente o una BD caída?

// ✅ MapIfFailWithoutException solo recupera lo que es decisión de negocio
var resultado = ObtenerDescuento(clienteId)
                    .MapIfFailWithoutException(_ => 0m);   // el SqlException sigue subiendo intacto
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema que resuelve

Los valores por defecto son útiles, pero **peligrosos si se aplican sin filtro**. Considera este código:

```csharp
// ❌ El bug silencioso más habitual del patrón railway
public async Task<decimal> ObtenerLimiteCreditoAsync(int clienteId)
    => (await _repo.ObtenerLimiteAsync(clienteId))
            .MapIfFail(_ => 0m)                      // "si no hay límite, cero"
            .Match(valid: x => x, fail: _ => 0m);
```

Si la base de datos está caída, `ObtenerLimiteAsync` devuelve un fallo **con** `SqlException` en los detalles… y el método responde `0m` como si el cliente simplemente no tuviera límite. El sistema sigue funcionando, rechaza operaciones legítimas y **nadie se entera**: no hay log de error, no hay alerta, no hay 503.

Con `MapIfFailWithoutException` la política es explícita:

```csharp
// ✅ Solo el "no tiene límite configurado" se convierte en 0
public async Task<MlResult<decimal>> ObtenerLimiteCreditoAsync(int clienteId)
    => await _repo.ObtenerLimiteAsync(clienteId)
                  .MapIfFailWithoutExceptionAsync(_ => 0m.ToAsync());
    // Si hubo SqlException → el MlResult sigue siendo Fail y el llamante decide (503, retry, etc.)
```

---

## La regla de oro: si hay excepción, no hay recuperación

Tres escenarios, tres comportamientos:

| Estado de entrada | ¿Se ejecuta el delegado? | Resultado |
|---|---|---|
| **Valid** | ❌ No | El valor pasa intacto (o se transforma con `funcValid` en la Forma B) |
| **Fail sin excepción** en `Details["Ex"]` | ✅ **Sí** | Se recupera: `Valid(func(errorsDetails))` |
| **Fail con excepción** en `Details["Ex"]` | ❌ No | **El mismo `Fail` sale intacto**, con todos sus errores y detalles |

La comprobación se hace con `GetDetailException()`, que devuelve `MlResult<Exception>`:

```csharp
errorsDetails.GetDetailException().Match(
    fail : _ => /* NO hay excepción → recuperamos */,
    valid: _ => /* SÍ hay excepción → devolvemos el fallo original */)
```

Fíjate en la inversión: aquí la rama **`fail`** de `GetDetailException()` (es decir, "no encontré ninguna excepción") es la que dispara la recuperación. Es lo contrario de `MapIfFailWithException`.

---

## Espejo exacto de `MapIfFailWithException`

Ambos métodos son complementarios y cubren, juntos, el 100 % de los fallos:

```csharp
// Los dos se pueden apilar para dar respuestas distintas a cada clase de fallo
var respuesta = await ProcesarPedidoAsync(dto)
                        .MapIfFailWithoutExceptionAsync(err => Rechazo.PorNegocio(err.ToErrorsMessages()).ToAsync())
                        .MapIfFailWithExceptionAsync(ex   => Rechazo.PorAveria(ex.GetType().Name).ToAsync());
```

| | `MapIfFailWithException` | `MapIfFailWithoutException` |
|---|---|---|
| Recupera cuando… | **sí** hay excepción en `Details["Ex"]` | **no** hay excepción en `Details["Ex"]` |
| Firma del delegado de fallo | `Func<Exception, T>` (recibe la excepción) | `Func<MlErrorsDetails, T>` (recibe los errores) |
| Si no aplica | devuelve el `Fail` original intacto | devuelve el `Fail` original intacto |
| Filtrado por tipo (`TException`) | ✅ Sí | ❌ No (no hay excepción que filtrar) |
| Subfamilia `...Error` | ✅ Sí (`MapIfFailWithExceptionError`) | ❌ No (ya recibe `MlErrorsDetails`) |
| Uso típico | fallback ante averías técnicas | valores por defecto de negocio |

📌 Detalle importante: como el delegado de `MapIfFailWithoutException` ya recibe `MlErrorsDetails`, **no necesita** una subfamilia `...Error` como sí ocurría en `MapIfFailWithException` (donde el delegado solo recibía la `Exception` y hacía falta otra variante para acceder a los errores completos).

---

## Las dos formas de `MapIfFailWithoutException`

### Forma A — recuperar sin cambiar de tipo

```csharp
MlResult<T> MapIfFailWithoutException<T>(this MlResult<T> source, Func<MlErrorsDetails, T> func)
```

La rama válida **no se toca**. Solo se define qué hacer con el fallo de negocio. Es la forma **apilable**: puedes encadenarla con más operaciones `MlResult<T>` porque el tipo no cambia.

```csharp
MlResult<Tarifa> tarifa = BuscarTarifaCliente(clienteId)
                              .MapIfFailWithoutException(_ => Tarifa.General);
```

### Forma B — transformar ambas ramas a un tipo común

```csharp
MlResult<TReturn> MapIfFailWithoutException<T, TReturn>(this MlResult<T>               source,
                                                            Func<T, TReturn>           funcValid,
                                                            Func<MlErrorsDetails, TReturn> funcFail)
```

Convergencia de tipos: el éxito se proyecta con `funcValid` y el fallo de negocio con `funcFail`, ambos hacia `TReturn`. Ideal para construir un DTO de respuesta.

```csharp
MlResult<RespuestaDto> respuesta =
    ValidarSolicitud(dto)
        .MapIfFailWithoutException(
            funcValid: s   => RespuestaDto.Aceptada(s.Id),
            funcFail : err => RespuestaDto.Rechazada(err.ToErrorsMessages()));
    // Si el fallo traía excepción, `respuesta` sigue siendo Fail: no se fabrica un DTO falso
```

⚠️ Ojo con la Forma B: **si el fallo trae excepción, `funcValid` tampoco se ejecuta** (obviamente) y el resultado es el `Fail` original convertido a `MlResult<TReturn>`. Es decir, la conversión de tipo se produce, pero sigue siendo un fallo.

---

## Firmas reales e implementación

### Forma A (código fuente literal)

```csharp
public static MlResult<T> MapIfFailWithoutException<T>(this MlResult<T>              source,
                                                            Func<MlErrorsDetails, T> func)
    => source.Match(
                fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => func(errorsDetails).ToMlResultValid<T>(),
                                            valid: _ => errorsDetails),
                valid: x => x);
```

Tres puntos que explican todo el comportamiento:

1. `valid: x => x` — el valor válido se devuelve sin tocar (conversión implícita `T → MlResult<T>`).
2. `fail: _ => func(errorsDetails).ToMlResultValid<T>()` — **no hay excepción**: se llama al delegado y su resultado se envuelve como `Valid`. Los errores originales **se descartan** (la recuperación es total).
3. `valid: _ => errorsDetails` — **sí hay excepción**: se devuelve `errorsDetails` tal cual, que por conversión implícita vuelve a ser un `MlResult<T>` en fallo con **todos** sus errores y detalles intactos.

### Forma B (código fuente literal)

```csharp
public static MlResult<TReturn> MapIfFailWithoutException<T, TReturn>(this MlResult<T>                    source,
                                                                           Func<T              , TReturn> funcValid,
                                                                           Func<MlErrorsDetails, TReturn> funcFail)
    => source.Match(
                fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => funcFail(errorsDetails).ToMlResultValid(),
                                            valid: _ => errorsDetails),
                valid: x => funcValid(x).ToMlResultValid());
```

Idéntica estructura; la única diferencia es que la rama válida pasa por `funcValid` y el tipo de salida es `TReturn`.

📌 Como los delegados devuelven **valores desnudos** (`T` / `TReturn`, no `MlResult<...>`), esto es `Map` y no `Bind`. Si tu recuperación puede a su vez fallar, necesitas [`BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md).

---

## Variantes asíncronas

Las sobrecargas asíncronas combinan dos ejes independientes:

| Origen | Delegado(s) | Método |
|---|---|---|
| `MlResult<T>` | `Func<MlErrorsDetails, Task<T>>` | `MapIfFailWithoutExceptionAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, Task<T>>` | `MapIfFailWithoutExceptionAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, T>` (síncrono) | `MapIfFailWithoutExceptionAsync` |

La Forma B replica el mismo esquema con el par `(funcValidAsync, funcFailAsync)`, incluyendo combinaciones mixtas (uno síncrono y otro asíncrono).

```csharp
// Origen asíncrono + recuperación asíncrona
var config = await _api.LeerConfigRemotaAsync(entorno)
                       .MapIfFailWithoutExceptionAsync(async err =>
                       {
                           await _auditoria.RegistrarAsync($"Config no definida: {err.ToErrorsMessages()}");
                           return Configuracion.PorDefecto;
                       });

// Origen asíncrono + recuperación síncrona (lo más frecuente)
var config = await _api.LeerConfigRemotaAsync(entorno)
                       .MapIfFailWithoutExceptionAsync(_ => Configuracion.PorDefecto);
```

📌 Internamente usan `MatchAsync` y `GetDetailExceptionAsync()`, la versión asíncrona de la comprobación. Si tienes un valor síncrono y la sobrecarga exige `Task<T>`, envuélvelo con `.ToAsync()`.

---

## `TryMapIfFailWithoutException` — cuando la recuperación puede lanzar

Si el delegado de recuperación puede lanzar (leer un fichero de respaldo, deserializar, calcular algo frágil), usa la variante protegida:

```csharp
public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                               Func<MlErrorsDetails, T> func,
                                                               Func<Exception, string>  errorMessageBuilder)
    => source.Match(
                fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : _ => func.TryToMlResult(errorsDetails, errorMessageBuilder),
                                            valid: _ => errorsDetails),
                valid: x => x);

public static MlResult<T> TryMapIfFailWithoutException<T>(this MlResult<T>              source,
                                                               Func<MlErrorsDetails, T> func,
                                                               string                   errorMessage = null!)
    => source.TryMapIfFailWithoutException(func, _ => errorMessage!);
```

Dos formas de expresar el mensaje de error: un `string` fijo o un `Func<Exception, string>` que puede inspeccionar la excepción capturada.

```csharp
var plantilla = ObtenerPlantillaBD(codigo)
                    .TryMapIfFailWithoutException(
                        _  => File.ReadAllText(RutaPlantillaLocal(codigo)),
                        ex => $"La plantilla '{codigo}' no está en BD y el respaldo local falló: {ex.Message}");
```

Si `func` lanza, `TryToMlResult` captura la excepción, la guarda en `Details["Ex"]` y devuelve un `Fail` nuevo con el mensaje construido.

> ⚠️ **Diferencia con `TryMapIfFailWithException`**: aquella variante llama a `MergeErrorsDetailsIfFail(source)` para **fusionar** los errores originales con el nuevo. `TryMapIfFailWithoutException` **no lo hace**: si la recuperación lanza, el fallo resultante contiene únicamente el error de la excepción capturada y **se pierde el mensaje de negocio original**. Si necesitas conservar ambos, añade el merge tú mismo:
>
> ```csharp
> var r = origen.TryMapIfFailWithoutException(func, "Respaldo fallido")
>               .MergeErrorsDetailsIfFail(origen);   // conserva el error de negocio inicial
> ```

Existen también las variantes `TryMapIfFailWithoutExceptionAsync` para todas las combinaciones de origen y delegado, en ambas formas (A y B).

---

## ⚠️ No existe la subfamilia `...Error`

En `MapIfFailWithException` hay una subfamilia `MapIfFailWithExceptionError` cuyo delegado recibe `MlErrorsDetails` en vez de la `Exception`. **Aquí no existe ningún `MapIfFailWithoutExceptionError`** (verificado en el código fuente) y es lógico: el delegado de `MapIfFailWithoutException` **ya recibe** `MlErrorsDetails`, así que no hace falta otra variante para acceder a los errores.

Tampoco existen sobrecargas con `TException`: no tiene sentido filtrar por tipo de excepción en un método que precisamente solo actúa **cuando no hay excepción**.

---

## Tabla de decisión rápida

| Necesito… | Método |
|---|---|
| Un valor por defecto para **cualquier** fallo | [`MapIfFail`](4_MapIfFail.md) |
| Un valor por defecto **solo para fallos de negocio** (sin excepción) | **`MapIfFailWithoutException`** |
| Un fallback **solo ante averías técnicas** (con excepción) | [`MapIfFailWithException`](6_MapIfFailWithException.md) |
| Recuperación que **también puede fallar** (devuelve `MlResult`) | [`BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md) |
| Recuperación que puede **lanzar** una excepción | `TryMapIfFailWithoutException` |
| Convertir éxito y fallo de negocio a un **tipo común** | Forma B (`funcValid` + `funcFail`) |
| Ejecutar algo **siempre**, ignorando el estado | [`MapAlways`](8_MapAlways.md) |
| Solo **registrar** el fallo sin alterar el resultado | [`ExecSelfIfFailWithoutException`](../ExecSelf/6_ExecSelfIfFailWithoutException.md) |

---

## Ejemplos Prácticos

### Ejemplo 1: Preferencias de usuario con valores por defecto seguros

El caso canónico. "Si el usuario no ha configurado sus preferencias, usa las predeterminadas" — pero **no** si la base de datos está caída, porque entonces sobrescribiríamos las preferencias reales del usuario en la siguiente escritura.

```csharp
public async Task<MlResult<Preferencias>> ObtenerPreferenciasAsync(int usuarioId)
    => await _repo.BuscarPreferenciasAsync(usuarioId)
                  .MapIfFailWithoutExceptionAsync(err =>
                  {
                      // Solo llegamos aquí si el fallo fue "no existen preferencias" (negocio)
                      _log.LogInformation("Usuario {Id} sin preferencias: aplico predeterminadas. {D}",
                                          usuarioId, err.ToErrorsDescription());
                      return Preferencias.PorDefecto(usuarioId).ToAsync();
                  });
```

Comportamiento resultante:

| Situación en el repositorio | Resultado |
|---|---|
| Registro encontrado | `Valid(preferenciasDelUsuario)` |
| `Fail("No existen preferencias para el usuario 42")` | `Valid(Preferencias.PorDefecto(42))` |
| `Fail` con `SqlException` en `Details["Ex"]` | `Fail` intacto → el controlador puede devolver 503 |

### Ejemplo 2: Controlador que distingue 404 de 503

```csharp
[HttpGet("{id:int}")]
public async Task<IActionResult> Obtener(int id)
{
    var resultado = await _servicio.ObtenerFichaAsync(id)
                                   .MapIfFailWithoutExceptionAsync(_ => FichaDto.Vacia(id).ToAsync());

    return resultado.Match(
        valid: ficha  => Ok(ficha),
        fail : errores =>
        {
            // Si llegamos aquí, el fallo SÍ traía excepción: es técnico
            _log.LogError("Fallo técnico al obtener ficha {Id}: {D}", id, errores.ToErrorsDescription());
            return StatusCode(503, new { mensaje = "Servicio temporalmente no disponible" });
        });
}
```

Toda la lógica de "esto es negocio / esto es avería" queda encapsulada en una única llamada, sin `try/catch` ni comprobaciones manuales del diccionario de detalles.

### Ejemplo 3: Respuesta unificada con la Forma B

```csharp
public async Task<ResultadoAltaDto> AltaAsync(AltaClienteDto dto)
{
    var resultado = await ValidarAlta(dto)
                            .BindAsync(d => _repo.InsertarAsync(d))
                            .MapIfFailWithoutExceptionAsync(
                                funcValidAsync: cliente => ResultadoAltaDto.Ok(cliente.Id).ToAsync(),
                                funcFailAsync : err     => ResultadoAltaDto
                                                               .Rechazada(err.ToErrorsMessages())
                                                               .ToAsync());

    // Los fallos de validación ya vienen convertidos en DTO de rechazo.
    // Solo los fallos técnicos siguen siendo Fail y hay que traducirlos aquí.
    return resultado.Match(
        valid: dtoRespuesta => dtoRespuesta,
        fail : errores      => ResultadoAltaDto.ErrorInterno(errores.ToErrorsDescription()));
}
```

### Ejemplo 4: Cadena de políticas por clase de fallo

Apilar los dos métodos complementarios permite escribir la política completa de forma declarativa:

```csharp
public async Task<MlResult<Precio>> CalcularPrecioAsync(string sku, int cantidad)
    => await _tarifas.ObtenerPrecioAsync(sku, cantidad)
                     // 1) Sin tarifa específica (negocio) → tarifa general
                     .MapIfFailWithoutExceptionAsync(_ => _tarifas.PrecioGeneral(sku).ToAsync())
                     // 2) Avería del servicio de tarifas → último precio cacheado, marcado como no fiable
                     .MapIfFailWithExceptionAsync(ex =>
                     {
                         _log.LogWarning(ex, "Servicio de tarifas caído para {Sku}", sku);
                         return (_cache.UltimoPrecio(sku) with { EsFiable = false }).ToAsync();
                     })
                     .ExecSelfIfValidAsync(p => _metricas.RegistrarPrecioAsync(sku, p));
```

Se lee como una tabla de políticas: primero el negocio, luego la degradación técnica, y al final la instrumentación.

### Ejemplo 5: Qué **no** hacer

```csharp
// ❌ MAL: MapIfFail se traga las averías junto con las reglas de negocio
var stock = await _almacen.ConsultarStockAsync(sku)
                          .MapIfFailAsync(_ => 0.ToAsync());
// Un timeout de red se convierte en "no hay stock" → se rechazan ventas de producto disponible

// ❌ MAL: inspeccionar los detalles a mano reimplementa el método y es frágil
var stock2 = (await _almacen.ConsultarStockAsync(sku)).Match(
    valid: x => x,
    fail : err => err.Details.ContainsKey("Ex") ? throw new Exception("boom") : 0);

// ❌ MAL: usar .Value directamente rompe el carril
// var s = resultado.Value;

// ✅ BIEN: la política queda explícita y los fallos técnicos se propagan
var stock3 = await _almacen.ConsultarStockAsync(sku)
                           .MapIfFailWithoutExceptionAsync(_ => 0.ToAsync());
```

---

## Mejores Prácticas

1. **Usa `MapIfFailWithoutException` en lugar de `MapIfFail` siempre que apliques un valor por defecto.** Es la versión segura: te obliga a decidir por separado qué hacer con las averías, en vez de enmascararlas.

2. **Recuerda que el fallo con excepción sale intacto.** Eso significa que la tubería sigue en fallo: hay que darle salida más adelante (`MapIfFailWithException`, `Match`, o dejar que el llamante lo traduzca a 5xx).

3. **La distinción depende de que las excepciones se guarden en `Details["Ex"]`.** Eso solo ocurre si usas `Try*` (`TryMap`, `TryBind`, `TryMapIfFail`…) o creas el fallo con `MlErrorsDetails.FromErrorMessageWithException`. Si capturas una excepción con `try/catch` y creas el fallo con `"mensaje".ToMlResultFail<T>()`, ese fallo pasará por "negocio" y **sí** se recuperará.

4. **No pierdas el error de negocio original en las variantes `Try`.** A diferencia de `TryMapIfFailWithException`, aquí no hay merge automático; añade `.MergeErrorsDetailsIfFail(origen)` si necesitas conservarlo.

5. **Registra antes de recuperar.** Una recuperación silenciosa dificulta el diagnóstico. Un `ExecSelfIfFail` previo, o un log dentro del propio delegado, deja rastro de que se aplicó el valor por defecto.

6. **Marca los valores degradados.** Si el valor por defecto no es tan bueno como el real, señálalo en el propio modelo (`EsFiable = false`, `Origen = "PorDefecto"`) para que las capas superiores puedan decidir.

7. **Si la recuperación puede fallar, usa `Bind` y no `Map`.** El delegado de `MapIfFailWithoutException` devuelve un valor desnudo; devolver un `MlResult` desde él produce el anidamiento `MlResult<MlResult<T>>`.

8. **Prefiere la Forma A cuando quieras seguir encadenando** (mantiene el tipo y es apilable) y la Forma B cuando estés cerrando la tubería hacia un DTO de respuesta.

9. **Documenta la política en el nombre del método de servicio.** `ObtenerLimiteOCero` comunica mejor que `ObtenerLimite` que hay una recuperación de negocio aplicada.

---

## Resumen

- `MapIfFailWithoutException` recupera **solo** los fallos que **no** llevan excepción en `Details["Ex"]`: los fallos de negocio y validación.
- Si el fallo **sí** trae excepción, el delegado no se ejecuta y **el `Fail` original se devuelve intacto**, con todos sus errores y detalles.
- Es el **espejo exacto** de [`MapIfFailWithException`](6_MapIfFailWithException.md); juntos cubren todos los fallos y permiten políticas distintas para cada clase.
- Tiene **dos formas**: A `<T>(Func<MlErrorsDetails, T>)` que preserva el tipo y es apilable, y B `<T, TReturn>(funcValid, funcFail)` que hace converger ambas ramas a un tipo común.
- **No existe** ninguna subfamilia `...Error` ni sobrecargas con `TException`: el delegado ya recibe `MlErrorsDetails` y no hay excepción que filtrar.
- Las variantes `TryMapIfFailWithoutException` protegen el delegado con `TryToMlResult`, pero **no fusionan** los errores originales (a diferencia de su hermana `WithException`).
- El delegado devuelve un **valor desnudo**; si tu recuperación puede fallar, usa [`BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md).

---

## Ver también

- [`6_MapIfFailWithException.md`](6_MapIfFailWithException.md) — el método complementario: recupera solo ante excepciones.
- [`4_MapIfFail.md`](4_MapIfFail.md) — recuperación sin filtro, para cualquier fallo.
- [`8_MapAlways.md`](8_MapAlways.md) — ejecutar algo con independencia del estado.
- [`1_Map.md`](1_Map.md) — la operación base de transformación.
- [`../Bind/9_BindIfFailWithoutException.md`](../Bind/9_BindIfFailWithoutException.md) — la versión `Bind`, cuando la recuperación devuelve `MlResult`.
- [`../Bind/8_BindIfFailWithException.md`](../Bind/8_BindIfFailWithException.md) — su complementaria en la familia `Bind`.
- [`../ExecSelf/6_ExecSelfIfFailWithoutException.md`](../ExecSelf/6_ExecSelfIfFailWithoutException.md) — efectos secundarios sin alterar el resultado.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException()`, `MergeErrorsDetailsIfFail` y el resto de utilidades de detalles.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — estructura real de `MlError` y `MlErrorsDetails`.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la familia `Map`.