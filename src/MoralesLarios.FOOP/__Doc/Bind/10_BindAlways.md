# BindAlways — Ejecutar siempre, sea válido o fallido

## Índice

1. [Introducción](#introducción)
2. [Las dos formas de `BindAlways`](#las-dos-formas-de-bindalways)
3. [Firmas reales](#firmas-reales)
4. [Forma A: el punto de convergencia](#forma-a-el-punto-de-convergencia)
5. [Forma B: la bifurcación final](#forma-b-la-bifurcación-final)
6. [`BindAlways` no es un `finally`](#bindalways-no-es-un-finally)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [`TryBindAlways` — cuando la operación final puede lanzar](#trybindalways--cuando-la-operación-final-puede-lanzar)
9. [Relación con `Match`, `MapAlways` y `ExecSelf`](#relación-con-match-mapalways-y-execself)
10. [Ejemplos Prácticos](#ejemplos-prácticos)
11. [Mejores Prácticas](#mejores-prácticas)
12. [Resumen](#resumen)
13. [Ver también](#ver-también)

---

## Introducción

Todos los métodos que hemos visto hasta ahora son *condicionales*: `Bind` solo actúa si el resultado es válido, `BindIfFail` solo si ha fallado. `BindAlways` rompe esa regla: **se ejecuta siempre**, con independencia del estado del resultado.

Su utilidad es la de un punto de convergencia: el momento del *pipeline* en el que dejas de propagar el estado anterior y produces un resultado nuevo, ya sea porque los dos caminos deben acabar en lo mismo o porque quieres decidir explícitamente qué devolver en cada caso.

```csharp
// ❌ Con un if sobre el estado: verboso y expone el estado interno
var respuesta = resultado.IsValid
                    ? ConstruirRespuestaOk(resultado)
                    : ConstruirRespuestaError(resultado);

// ✅ Con BindAlways: las dos ramas quedan a la vista, sin tocar IsValid
var respuesta = resultado.BindAlways(
                    funcValidAlways: pedido  => ConstruirRespuestaOk(pedido),
                    funcFailAlways : errores => ConstruirRespuestaError(errores));
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`; para llegar al valor original, `GetDetailValue<T>()`.

---

## Las dos formas de `BindAlways`

| Forma | Firma resumida | Qué recibe | Para qué sirve |
|---|---|---|---|
| **A — Descartar y sustituir** | `BindAlways<T, TReturn>(funcAlways)` | **nada** | Producir un resultado nuevo ignorando por completo lo anterior. |
| **B — Bifurcar y converger** | `BindAlways<T, TResult>(funcValidAlways, funcFailAlways)` | el valor o los errores | Tratar los dos caminos por separado y unificar el tipo de salida. |

```csharp
// Forma A — el delegado no recibe ningún parámetro
MlResult<Estado> estado = resultado.BindAlways(() => LeerEstadoActual());

// Forma B — un delegado por rama
MlResult<Informe> informe = resultado.BindAlways(
                                funcValidAlways: datos   => ConstruirInforme(datos),
                                funcFailAlways : errores => ConstruirInformeDeFallo(errores));
```

> 📌 La forma A **descarta el resultado anterior por completo**, incluidos los errores. No es un `finally`: es un «olvida lo anterior y devuelve esto».

---

## Firmas reales

```csharp
// FORMA A
public static MlResult<TReturn> BindAlways<T, TReturn>(this MlResult<T>              source,
                                                            Func<MlResult<TReturn>> funcAlways)
    => funcAlways();

// FORMA B
public static MlResult<TResult> BindAlways<T, TResult>(this MlResult<T>                              source,
                                                            Func<T              , MlResult<TResult>> funcValidAlways,
                                                            Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
    => source.Match(
            valid: funcValidAlways,
            fail : funcFailAlways);
```

Merece la pena detenerse en la forma A: **el cuerpo es literalmente `funcAlways()`**. El parámetro `source` no se usa para nada más que para permitir la sintaxis de extensión. Eso explica todo su comportamiento:

- No comprueba el estado.
- No propaga errores.
- No accede al valor.
- Si `funcAlways()` falla, ese es el único fallo que verás.

La forma B, en cambio, es **exactamente `Match` con las dos ramas devolviendo `MlResult<TResult>`**.

---

## Forma A: el punto de convergencia

Úsala cuando lo que venga después **no dependa en absoluto** de lo anterior. El caso típico es un paso de recarga o de recuento tras una operación cuyo éxito ya se ha registrado en otro sitio:

```csharp
// Se intenta refrescar la caché; el resultado final es siempre el recuento actual,
// tanto si el refresco funcionó como si no.
MlResult<int> elementosEnCache = RefrescarCache()
                                    .ExecSelfIfFail(e => _log.LogWarning("Refresco fallido: {E}",
                                                                        e.ToErrorsMessages()))
                                    .BindAlways(() => ContarElementosEnCache());
```

⚠️ El peligro es evidente: **si no registras el fallo antes, desaparece sin dejar rastro**. Combina siempre la forma A con un `ExecSelfIfFail` previo, o usa la forma B para tener el control.

---

## Forma B: la bifurcación final

Es la forma que usarás casi siempre. Su valor es que **unifica el tipo**: partes de un `MlResult<T>` y llegas a un `MlResult<TResult>` decidiendo en ambas ramas.

```csharp
public MlResult<RespuestaApi> Procesar(SolicitudDto dto)
    => ValidarSolicitud(dto)
         .Bind(s => EjecutarSolicitud(s))
         .BindAlways(
             funcValidAlways: r       => RespuestaApi.Ok(r.Identificador,
                                                        r.FechaProceso).ToMlResultValid(),
             funcFailAlways : errores => RespuestaApi.Error(
                                                codigo  : ClasificarCodigo(errores),
                                                mensajes: errores.ToErrorsMessages())
                                            .ToMlResultValid());

private static string ClasificarCodigo(MlErrorsDetails errores)
    => errores.GetDetailException()
              .Match(valid: _ => "ERROR_TECNICO",
                     fail : _ => "ERROR_NEGOCIO");
```

Fíjate en que las dos ramas devuelven `ToMlResultValid()`: hemos convertido un fallo del dominio en un **resultado válido** que contiene la descripción del error. Eso es habitual en la frontera de la aplicación (controladores, adaptadores de mensajería), donde el fallo de negocio deja de ser un fallo y pasa a ser un dato de la respuesta.

Nada te obliga a hacerlo así: `funcFailAlways` puede perfectamente devolver otro fallo, enriquecido con más contexto.

```csharp
.BindAlways(
    funcValidAlways: pedido  => pedido.ToMlResultValid(),
    funcFailAlways : errores => errores.AddErrorMessage($"Fallo procesando el lote {loteId}")
                                       .ToMlResultFail<Pedido>())
```

---

## `BindAlways` no es un `finally`

Es la confusión más frecuente, y conviene desmontarla:

| | `try/finally` | `BindAlways` |
|---|---|---|
| ¿Se ejecuta ante una excepción no capturada? | Sí | **No**: la excepción sube y el *pipeline* se rompe |
| ¿Conserva el resultado anterior? | Sí, `finally` no altera el retorno | Forma A: **no**. Forma B: solo si tú lo devuelves |
| ¿Sirve para liberar recursos? | Sí | No: usa `using` / `try-finally` de C# |

Si lo que quieres es **observar sin alterar** el resultado, no uses `BindAlways`: usa [`ExecSelf`](../ExecSelf/1_ExecSelf.md), que devuelve el resultado intacto.

```csharp
// ❌ BindAlways forma A para "solo registrar": destruye el resultado
var r = Procesar(dto).BindAlways(() => { _log.LogInformation("Fin"); return Unit().ToMlResultValid(); });

// ✅ ExecSelf conserva el resultado
var r = Procesar(dto).ExecSelf(
            actionValid: p => _log.LogInformation("Procesado {Id}", p.Id),
            actionFail : e => _log.LogWarning("Fallo: {E}", e.ToErrorsMessages()));
```

---

## Variantes asíncronas

Ambas formas tienen su familia asíncrona completa, combinando origen síncrono/asíncrono y delegados síncronos/asíncronos.

| Forma | Método | Sobrecargas asíncronas |
|---|---|---|
| A | `BindAlwaysAsync<T, TReturn>(Func<Task<MlResult<TReturn>>>)` | 4 |
| B | `BindAlwaysAsync<T, TResult>(funcValidAlwaysAsync, funcFailAlwaysAsync)` | 4 |
| A | `TryBindAlwaysAsync` | 8 |
| B | `TryBindAlwaysAsync` | 8 |

```csharp
public Task<MlResult<RespuestaApi>> ProcesarAsync(SolicitudDto dto)
    => ValidarSolicitudAsync(dto)
         .BindAsync(s => EjecutarAsync(s))
         .BindAlwaysAsync(
             funcValidAlwaysAsync: async r       => await ConstruirOkAsync(r),
             funcFailAlwaysAsync : async errores => await RegistrarYConstruirErrorAsync(errores));
```

> 💡 `funcFailAlwaysAsync` es un buen sitio para tareas de cierre que sí deben esperarse (auditar, publicar un evento de fallo), porque a diferencia de `ExecSelfIfFailAsync` aquí el resultado de esa tarea **sí** forma parte del flujo.

---

## `TryBindAlways` — cuando la operación final puede lanzar

Los pasos de convergencia suelen tocar infraestructura (recargar, contar, publicar un evento), así que también pueden lanzar. Las variantes `Try*` lo capturan:

```csharp
public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T>              source,
                                                               Func<MlResult<TReturn>> funcAlways,
                                                               Func<Exception, string> errorMessageBuilder);

public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T>              source,
                                                               Func<MlResult<TReturn>> funcAlways,
                                                               string                  errorMessage = null!);
```

Internamente usan `funcAlways.TryToMlResult(errorMessageBuilder)`, así que la excepción capturada queda en `Details["Ex"]` y podrás clasificarla después con `GetDetailException()`.

```csharp
var resumen = ProcesarLote(lineas)
                .TryBindAlways(() => _repo.LeerResumenDelLote(loteId),
                               ex => $"No se pudo leer el resumen del lote {loteId}: {ex.Message}");
```

---

## Relación con `Match`, `MapAlways` y `ExecSelf`

Este es el punto donde más se confunden las herramientas, así que aquí está la tabla completa:

| Necesito… | Herramienta | Devuelve |
|---|---|---|
| Salir del mundo `MlResult` con un valor crudo | `Match(valid, fail)` | `TReturn` (crudo) |
| Decidir en ambas ramas y **seguir** en `MlResult`, con función que **puede fallar** | `BindAlways(funcValid, funcFail)` | `MlResult<TResult>` |
| Decidir en ambas ramas y seguir en `MlResult`, con función que **no puede fallar** | `MapAlways(funcValid, funcFail)` | `MlResult<TResult>` |
| Producir un resultado nuevo ignorando el anterior | `BindAlways(funcAlways)` | `MlResult<TReturn>` |
| Observar ambas ramas sin alterar nada | `ExecSelf(actionValid, actionFail)` | el `MlResult<T>` original |
| Continuar solo si es válido | `Bind` / `Map` | `MlResult<TReturn>` |
| Continuar solo si ha fallado | `BindIfFail` / `MapIfFail` | `MlResult<T>` |

> 📌 Curiosidad reveladora: en el código fuente, `BindAlways(funcAlways)`, `MapAlways(funcAlways)` y `Match(funcAll)` (el de la región `MatchAll`) están implementados **exactamente igual**: invocan el delegado y devuelven su resultado. Elige el nombre que mejor exprese tu intención.

---

## Ejemplos Prácticos

### Ejemplo 1: Frontera de la API — todo acaba en una respuesta

```csharp
[HttpPost("pedidos")]
public async Task<IActionResult> Crear([FromBody] PedidoDto dto)
    => await _servicio.CrearAsync(dto)
        .BindAlwaysAsync(
            funcValidAlwaysAsync: async pedido =>
            {
                await _eventos.PublicarAsync(new PedidoCreado(pedido.Id));
                return ((IActionResult)CreatedAtAction(nameof(Obtener),
                                                       new { id = pedido.Id },
                                                       PedidoDto.Desde(pedido)))
                        .ToMlResultValid();
            },
            funcFailAlwaysAsync: async errores =>
            {
                await _auditoria.RegistrarAsync(errores.ToDetailsDescription());

                IActionResult respuesta = errores.GetDetailException()
                    .Match(valid: _ => StatusCode(500, "Error interno"),
                           fail : _ => BadRequest(errores.ToErrorsMessages()));

                return respuesta.ToMlResultValid();
            })
        .MatchAsync(
            valid: respuesta => respuesta,
            fail : errores   => StatusCode(500, errores.ToErrorsDescription()));
```

El `MatchAsync` final solo se dispara si alguna de las dos ramas de convergencia falló a su vez (por ejemplo, si el publicador de eventos devolvió un fallo).

### Ejemplo 2: Informe de ejecución de un proceso por lotes

```csharp
public record ResumenLote(int LoteId, int Procesadas, int Rechazadas,
                          bool Completado, IEnumerable<string> Incidencias);

public MlResult<ResumenLote> Ejecutar(int loteId)
    => _repo.LeerLineas(loteId)
            .Bind(lineas => ValidarLote(lineas))
            .Bind(lineas => lineas.Projection(ProcesarLinea))   // procesa todas
            .BindAlways(
                funcValidAlways: procesadas => new ResumenLote(
                                                    LoteId     : loteId,
                                                    Procesadas : procesadas.Count(),
                                                    Rechazadas : 0,
                                                    Completado : true,
                                                    Incidencias: Enumerable.Empty<string>())
                                                .ToMlResultValid(),

                funcFailAlways : errores    => new ResumenLote(
                                                    LoteId     : loteId,
                                                    Procesadas : 0,
                                                    Rechazadas : errores.Errors.Count(),
                                                    Completado : false,
                                                    Incidencias: errores.ToErrorsMessages())
                                                .ToMlResultValid());
```

El proceso por lotes **nunca falla**: siempre produce un resumen. Los fallos individuales se convierten en filas de incidencias, que es exactamente lo que necesita el operador que revisa la ejecución.

### Ejemplo 3: Cierre garantizado con registro previo (forma A bien usada)

```csharp
public MlResult<EstadoSincronizacion> Sincronizar()
    => DescargarCambios()
         .Bind(cambios => AplicarCambios(cambios))

         // 1) Primero se registra lo que ha pasado, gane o pierda
         .ExecSelf(
            actionValid: aplicados => _log.LogInformation("Aplicados {N} cambios", aplicados.Count),
            actionFail : errores   => _log.LogError("Sincronización fallida: {E}",
                                                   errores.ToDetailsDescription()))

         // 2) Y solo entonces se descarta el resultado anterior a favor del estado real
         .TryBindAlways(() => _repo.LeerEstadoSincronizacion(),
                        ex => $"No se pudo leer el estado de sincronización: {ex.Message}");
```

El orden es la clave: **registrar y luego descartar**. Al revés, el fallo se perdería en silencio.

### Ejemplo 4: Elegir mal la herramienta

```csharp
// ❌ Forma A sin registro previo: el fallo de GuardarPedido desaparece
var r = GuardarPedido(pedido)
            .BindAlways(() => _repo.ContarPedidos());
// Si el guardado falló, r es válido y nadie lo sabrá.

// ❌ BindAlways donde solo querías observar: destruye el tipo y el resultado
var r = GuardarPedido(pedido)
            .BindAlways(() => { _metricas.Incrementar("pedidos"); return 0.ToMlResultValid(); });

// ✅ Observar sin alterar
var r = GuardarPedido(pedido)
            .ExecSelf(actionValid: _ => _metricas.Incrementar("pedidos.ok"),
                      actionFail : _ => _metricas.Incrementar("pedidos.error"));

// ✅ Convergir de forma explícita
var r = GuardarPedido(pedido)
            .BindAlways(funcValidAlways: p       => p.Id.ToMlResultValid(),
                        funcFailAlways : errores => errores.ToMlResultFail<int>());
```

---

## Mejores Prácticas

1. **Prefiere la forma B.** Tener `funcValidAlways` y `funcFailAlways` a la vista documenta la decisión y hace imposible perder el fallo por descuido.

2. **Si usas la forma A, registra antes.** Un `ExecSelfIfFail` justo delante es prácticamente obligatorio: la forma A descarta los errores sin dejar rastro.

3. **No lo confundas con `finally`.** No se ejecuta ante excepciones no capturadas y no sirve para liberar recursos. Para eso, `using` y `try/finally` de C#.

4. **Úsalo en las fronteras, no en el medio.** Su sitio natural es el final del *pipeline*: controladores, adaptadores, generadores de informes. En medio de la lógica de dominio, `Bind` y `BindIfFail` expresan mejor la intención.

5. **Si la función no puede fallar, usa `MapAlways`.** Envolver un valor seguro en `ToMlResultValid()` solo para satisfacer la firma de `BindAlways` es ruido innecesario.

6. **Si solo quieres observar, usa `ExecSelf`.** Conserva el resultado y el tipo.

7. **Usa `Try*` si el paso de convergencia toca infraestructura.** Es el último eslabón del *pipeline*: una excepción ahí tira por la borda todo el trabajo anterior.

---

## Resumen

- `BindAlways` **se ejecuta siempre**, sea el resultado válido o fallido.
- Tiene **dos formas**: la **A** (`funcAlways` sin parámetros) descarta por completo el resultado anterior —su implementación es literalmente `funcAlways()`—; la **B** (`funcValidAlways` + `funcFailAlways`) es `Match` con `MlResult` en ambas ramas.
- La forma B es la recomendable: unifica el tipo de salida y hace explícita la decisión en cada rama.
- **No es un `finally`**: no reacciona a excepciones no capturadas y no conserva el resultado anterior por sí solo.
- Si solo quieres observar, usa `ExecSelf`; si la función no puede fallar, `MapAlways`; si quieres salir del mundo `MlResult`, `Match`.
- Las variantes `Try*` capturan las excepciones del paso de convergencia y las dejan en `Details["Ex"]`.
- Su sitio natural es la **frontera** de la aplicación, donde un fallo de dominio se convierte en un dato de la respuesta.

---

## Ver también

- [`3_Bind.md`](3_Bind.md) — el encadenamiento condicional básico.
- [`6_BindIfFail.md`](6_BindIfFail.md) — actuar solo cuando ha fallado.
- [`11_BindSaveValueInDetailsIfFaildFuncResultAsync.md`](11_BindSaveValueInDetailsIfFaildFuncResultAsync.md) — conservar el valor de entrada al fallar.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir del mundo `MlResult` con un valor crudo.
- [`../Match/2_MatchAll.md`](../Match/2_MatchAll.md) — el `Match` sin parámetros, hermano gemelo de la forma A.
- [`../Map/8_MapAlways.md`](../Map/8_MapAlways.md) — la versión para funciones que no pueden fallar.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — observar ambas ramas sin alterar el resultado.
- [`../Types/MlResultActionsBind.md`](../Types/MlResultActionsBind.md) — mapa completo de la clase.