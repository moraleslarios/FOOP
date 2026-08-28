# ExecSelf — Efectos secundarios sin alterar el resultado

## Índice
1. [Introducción](#introducción)
2. [Familias de la región `ExecSelf`](#familias-de-la-región-execself)
3. [`ExecSelf` — las dos formas](#execself--las-dos-formas)
4. [Variantes `Try*` — cuando el efecto secundario puede lanzar](#variantes-try--cuando-el-efecto-secundario-puede-lanzar)
5. [Variantes asíncronas](#variantes-asíncronas)
6. [Particularidades reales del código fuente](#particularidades-reales-del-código-fuente)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Comparación con Bind, Map y Match](#comparación-con-bind-map-y-match)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

`ExecSelf` es la operación de **efecto secundario** de la librería: ejecuta código (un log, una
métrica, una notificación, una auditoría, invalidar una caché…) y **devuelve el mismo `MlResult<T>`
que recibió**, sin modificarlo.

Ese «sin modificarlo» es la clave. Todas las demás operaciones transforman:

| Operación | ¿Cambia el resultado? |
| --- | :---: |
| `Bind` | Sí (puede pasar de válido a fallido) |
| `Map` | Sí (cambia el tipo del valor) |
| `MapIfFail` / `BindIfFail` | Sí (puede recuperar un fallo) |
| `Match` | Sí (sale del mundo `MlResult`) |
| **`ExecSelf*`** | **No. Devuelve exactamente lo que recibió** |

Por eso `ExecSelf` se puede insertar en **cualquier punto** de una tubería sin miedo a romperla:

```csharp
var resultado = await ValidarPedidoAsync(dto)
    .ExecSelfIfValidAsync(p  => _log.LogInformation("Pedido {Id} validado", p.Id))
    .BindAsync(p             => ReservarStockAsync(p))
    .ExecSelfIfFailAsync(er  => _log.LogWarning("Reserva falló: {E}", er.ToErrorsDescription()))
    .BindAsync(p             => CobrarAsync(p));
```

Si quitases las dos llamadas a `ExecSelf*`, el resultado final sería **idéntico**. Solo cambiaría
que no habría rastro en el log. Eso es lo que hace a `ExecSelf` seguro y fácil de razonar.

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone dos propiedades: `Errors`
> (`IEnumerable<MlError>`) y `Details` (`Dictionary<string, object>`). **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception` ni `HasValue`. Para consultarlo:
>
> | Necesitas | Usa |
> | --- | --- |
> | Array de mensajes | `errores.ToErrorsMessages()` |
> | Un texto con todos los errores | `errores.ToErrorsDescription()` |
> | El primer mensaje | `errores.Errors.First().Message` |
> | La excepción capturada por un `Try*` | `errores.GetDetailException()` → `MlResult<Exception>` |
> | Un dato adjunto | `errores.GetDetailValue<T>()` / `errores.GetDetail<T>("clave")` |

---

## Familias de la región `ExecSelf`

El archivo `Types/MlResultActionsExecSelf.cs` contiene estas familias:

| Familia | Se ejecuta cuando… | Recibe el delegado |
| --- | --- | --- |
| `ExecSelf` | **Siempre** (dos delegados, uno por rama) | `T` / `MlErrorsDetails` |
| `ExecSelfIf` | Según un predicado | `T` |
| `ExecSelfIfValid` | Solo si es válido | `T` |
| `ExecSelfIfFail` | Solo si es fallido | `MlErrorsDetails` |
| `ExecSelfIfFailWithValue` | Fallido **y** hay un valor adjunto en `Details["Value"]` | `MlErrorsDetails`, `TValue` |
| `ExecSelfIfFailWithException` | Fallido **y** hay una excepción en `Details["Ex"]` | `MlErrorsDetails`, `Exception` |
| `ExecSelfIfFailWithoutException` | Fallido y **no** hay excepción (error de negocio puro) | `MlErrorsDetails` |

Cada familia tiene su versión `*Async` y su versión `Try*` / `Try*Async`. El recuento exacto de
sobrecargas está en [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md).

---

## `ExecSelf` — las dos formas

### Forma 1: dos delegados, uno por rama

```csharp
public static MlResult<T> ExecSelf<T>(this MlResult<T>        source,
                                      Action<T>               actionValid,
                                      Action<MlErrorsDetails> actionFail)
```

Siempre se ejecuta **uno** de los dos. Es el equivalente a `Match` pero sin salir de la tubería:

```csharp
MlResult<Factura> resultado = EmitirFactura(datos)
    .ExecSelf(
        actionValid: f       => _metricas.Incrementar("facturas.emitidas"),
        actionFail : errores => _metricas.Incrementar("facturas.fallidas"));

// `resultado` sigue siendo el MlResult<Factura> original.
```

### Forma 2: un solo delegado sobre el propio resultado

```csharp
public static MlResult<T> ExecSelf<T>(this MlResult<T> source, Action<MlResult<T>> action)
```

Útil cuando el efecto secundario necesita ver el estado completo:

```csharp
var resultado = ProcesarLote(lineas)
    .ExecSelf(r => _auditoria.Registrar(new Entrada
    {
        Operacion = "ProcesarLote",
        Correcto  = r.IsValid,      // Nombres reales: IsValid / IsFail
        Momento   = DateTime.UtcNow
    }));
```

> 📌 Los nombres son **`IsValid`** e **`IsFail`**, no `IsSuccess` / `IsFailure`.

### `ExecSelfIfValid` y `ExecSelfIfFail`

Son la forma más habitual: solo te interesa una rama.

```csharp
var resultado = ObtenerCliente(id)
    .ExecSelfIfValid(c       => _cache.Guardar(c.Id, c))
    .ExecSelfIfFail (errores => _log.LogWarning("Cliente {Id} no disponible: {E}",
                                                id, errores.ToErrorsDescription()));
```

### `ExecSelfIfFailWithException` frente a `ExecSelfIfFailWithoutException`

Esta pareja permite **separar los fallos técnicos de los de negocio**, algo muy valioso para los
logs y las alertas:

```csharp
var resultado = await ConsultarProveedorAsync(peticion)

    // Fallo técnico: hubo excepción. Nivel Error + alerta.
    .ExecSelfIfFailWithExceptionAsync(async (errores, ex) =>
    {
        _log.LogError(ex, "Fallo técnico consultando al proveedor");
        await _alertas.EnviarAsync($"Proveedor caído: {ex.Message}");
    })

    // Fallo de negocio: sin excepción. Nivel Warning, sin alerta.
    .ExecSelfIfFailWithoutExceptionAsync(async errores =>
    {
        _log.LogWarning("Petición rechazada: {E}", errores.ToErrorsDescription());
        await Task.CompletedTask;
    });
```

### `ExecSelfIfFailWithValue`

Cuando un `Try*` o un `AddValueIfFail` guardó el dato que provocó el fallo en `Details["Value"]`,
esta familia te lo entrega ya tipado:

```csharp
var resultado = ImportarLinea(linea)
    .AddValueIfFail(linea)                                  // Adjunta la línea al fallo
    .ExecSelfIfFailWithValue<Resultado, LineaCsv>((errores, l) =>
        _log.LogWarning("Línea {N} rechazada: {E}", l.Numero, errores.ToErrorsDescription()));
```

### `ExecSelfIf` — condicional

```csharp
var resultado = GuardarPedido(pedido)
    .ExecSelfIf(p => p.Importe > 10_000,                    // Solo pedidos grandes
                p => _log.LogInformation("Pedido de alto importe: {Id}", p.Id));
```

---

## Variantes `Try*` — cuando el efecto secundario puede lanzar

Un log que escribe en disco, un envío HTTP o una escritura en caché **pueden lanzar**. Si eso pasa
dentro de un `ExecSelf` normal, la excepción sube y rompe la tubería. Las variantes `Try*` la
capturan y la convierten en un fallo del `MlResult`:

```csharp
MlResult<Pedido> resultado = GuardarPedido(pedido)
    .TryExecSelfIfValid(p => _colaExterna.Publicar(p),      // Puede lanzar
                        ex => $"No se pudo publicar el pedido: {ex.Message}");
```

> ⚠️ **Consecuencia importante**: con `Try*`, un fallo del efecto secundario **sí convierte** el
> resultado en fallido. Si el efecto es puramente informativo (un log) y no quieres que su fallo
> afecte al proceso, envuélvelo tú en un `try/catch` dentro del delegado y usa `ExecSelf` normal.

Igual que en el resto de la librería, el mensaje se indica con
`Func<Exception, string> errorMessageBuilder` o con `string exceptionAditionalMessage`; si se omite,
se usa `DEFAULT_EX_ERROR_MESSAGE(ex)` de `Helpers/Constants.cs`.

---

## Variantes asíncronas

Hay una sobrecarga para cada combinación de fuente (`MlResult<T>` o `Task<MlResult<T>>`) y delegado
(`Action<...>` o `Func<..., Task>`), de modo que **nunca necesitas `await` intermedios**:

```csharp
// Fuente async + delegado async: encadenado limpio de principio a fin.
var resultado = await ObtenerPedidoAsync(id)
    .ExecSelfIfValidAsync(async p  => await _bus.PublicarAsync(new PedidoLeido(p.Id)))
    .BindAsync           (async p  => await ValidarAsync(p))
    .ExecSelfIfFailAsync (async er => await _alertas.EnviarAsync(er.ToErrorsDescription()));
```

---

## Particularidades reales del código fuente

Dos detalles que conviene conocer porque rompen la simetría habitual de la librería:

| Esperarías | Realidad |
| --- | --- |
| `TryExecSelfIfFail` (síncrono) | **No existe.** El método síncrono se llama **`TryExecSelfFail`**. La versión asíncrona sí existe con ambos nombres (`TryExecSelfFailAsync` y `TryExecSelfIfFailAsync`). |
| `TryExecSelfIfFailWithoutException` | **No existe.** Solo hay la versión sin `Try`: `ExecSelfIfFailWithoutException` y su `*Async`. |

---

## Ejemplos Prácticos

### Ejemplo 1: Tubería completamente instrumentada

Un caso realista donde `ExecSelf*` aporta observabilidad **sin** ensuciar la lógica de negocio:

```csharp
public class ServicioPedidos
{
    private readonly ILogger<ServicioPedidos> _log;
    private readonly IMetricas                _metricas;
    private readonly IBusEventos              _bus;

    public Task<MlResult<Pedido>> ConfirmarPedidoAsync(Guid pedidoId)
    {
        var cronometro = Stopwatch.StartNew();

        return ObtenerPedidoAsync(pedidoId)

            // Trazamos la entrada.
            .ExecSelfIfValidAsync(p => _log.LogInformation(
                "Confirmando pedido {Id} ({Lineas} líneas)", p.Id, p.Lineas.Count))

            .BindAsync(p => ValidarDisponibilidadAsync(p))
            .BindAsync(p => CobrarAsync(p))
            .BindAsync(p => MarcarConfirmadoAsync(p))

            // Éxito: métrica + evento de dominio.
            .ExecSelfIfValidAsync(async p =>
            {
                _metricas.Registrar("pedido.confirmado", cronometro.ElapsedMilliseconds);
                await _bus.PublicarAsync(new PedidoConfirmado(p.Id, p.Importe));
            })

            // Fallo técnico: log de error con la excepción original.
            .ExecSelfIfFailWithExceptionAsync((errores, ex) =>
                _log.LogError(ex, "Error técnico confirmando el pedido {Id}", pedidoId))

            // Fallo de negocio: log de advertencia, sin ruido de excepciones.
            .ExecSelfIfFailWithoutExceptionAsync(errores =>
                _log.LogWarning("Pedido {Id} rechazado: {E}",
                                pedidoId, errores.ToErrorsDescription()))

            // Siempre: cerramos la métrica de duración.
            .ExecSelfAsync(r => _metricas.Registrar(
                r.IsValid ? "pedido.ok" : "pedido.ko", cronometro.ElapsedMilliseconds));
    }
}
```

Observa que **la tubería de negocio son solo los tres `BindAsync`**. Todo lo demás es instrumentación
que se puede añadir o quitar sin cambiar el comportamiento.

### Ejemplo 2: Auditoría de una operación sensible

```csharp
public Task<MlResult<Transferencia>> TransferirAsync(SolicitudTransferencia solicitud)
    => ValidarSolicitud(solicitud)
        .BindAsync(s => ComprobarSaldoAsync(s))
        .BindAsync(s => EjecutarAsync(s))
        .ExecSelfAsync(async resultado =>
        {
            // Una única entrada de auditoría, tanto si salió bien como si no.
            await _auditoria.RegistrarAsync(new AsientoAuditoria
            {
                Operacion = nameof(TransferirAsync),
                Origen    = solicitud.CuentaOrigen,
                Destino   = solicitud.CuentaDestino,
                Importe   = solicitud.Importe,
                Correcto  = resultado.IsValid,
                Detalle   = resultado.Match(valid: t       => $"Ref: {t.Referencia}",
                                            fail : errores => errores.ToErrorsDescription()),
                Momento   = DateTime.UtcNow
            });
        });
```

> 💡 Fíjate en el uso de `Match` **dentro** del delegado de `ExecSelf`: es la forma correcta de leer
> el contenido del resultado, porque `MlResult<T>.Value` es `internal protected`.

### Ejemplo 3: Invalidar caché solo en caso de éxito

```csharp
public Task<MlResult<int>> ActualizarTarifasAsync(IEnumerable<Tarifa> tarifas)
    => ValidarTarifas(tarifas)
        .BindAsync(t => GuardarAsync(t))
        .TryExecSelfIfValidAsync(
            async filas =>
            {
                await _cache.InvalidarAsync("tarifas");
                await _cache.InvalidarAsync("catalogo");
            },
            ex => $"Las tarifas se guardaron pero no se pudo invalidar la caché: {ex.Message}");
```

Aquí `TryExecSelfIfValidAsync` es la elección correcta: si la invalidación de caché falla, **queremos
saberlo** y que el resultado refleje el problema, porque el sistema quedaría sirviendo datos viejos.

---

## Mejores Prácticas

### 1. Elige la variante más específica

Cuanto más concreta sea la variante, menos `if` habrá dentro del delegado:

```csharp
// ❌ Comprobación manual dentro del delegado.
resultado.ExecSelf(r => { if (r.IsFail) _log.LogWarning("Error"); });

// ✅ La variante ya expresa la condición.
resultado.ExecSelfIfFail(errores => _log.LogWarning("Error: {E}", errores.ToErrorsDescription()));
```

### 2. `ExecSelf` no es para transformar

Si dentro del delegado necesitas devolver algo, la operación que buscas no es `ExecSelf`:

| Necesitas | Usa |
| --- | --- |
| Ejecutar código y seguir igual | `ExecSelf*` |
| Transformar el valor | `Map` |
| Encadenar algo que puede fallar | `Bind` |
| Recuperarte de un fallo | `MapIfFail` / `BindIfFail` |
| Salir con un valor concreto | `Match` |

### 3. `Try*` solo cuando el fallo del efecto importa

- Log o métrica informativos → `ExecSelf*` (y protege tú el delegado si hace falta).
- Publicar un evento, invalidar caché, escribir en una cola → `TryExecSelf*`, porque su fallo **sí**
  afecta a la corrección del sistema.

### 4. Separa fallos técnicos de fallos de negocio

Usar `ExecSelfIfFailWithException` + `ExecSelfIfFailWithoutException` en lugar de un único
`ExecSelfIfFail` evita que los logs de error se llenen de validaciones rechazadas, que no son
incidencias.

### 5. No abuses

Una tubería con un `ExecSelf` entre cada paso se vuelve ilegible. Instrumenta la **entrada**, la
**salida** y los **puntos de fallo interesantes**.

---

## Comparación con Bind, Map y Match

| Operación | Delegado recibe | Devuelve | ¿Puede cambiar el estado? |
| --- | --- | --- | :---: |
| `Bind` | `T` | `MlResult<TReturn>` | Sí |
| `Map` | `T` | `MlResult<TReturn>` | Sí (el tipo) |
| `MapIfFail` | `MlErrorsDetails` | `MlResult<T>` | Sí (recupera) |
| `ExecSelf*` | `T` y/o `MlErrorsDetails` | **El mismo `MlResult<T>`** | **No** |
| `TryExecSelf*` | `T` y/o `MlErrorsDetails` | `MlResult<T>` | Solo si el delegado lanza |
| `Match` | `T` y `MlErrorsDetails` | `TReturn` crudo | Sale de la tubería |

---

## Resumen

- `ExecSelf*` ejecuta efectos secundarios y **devuelve el mismo resultado**: es la operación segura
  para instrumentar una tubería.
- Hay siete familias, de la más general (`ExecSelf`) a las más específicas
  (`ExecSelfIfFailWithException`, `ExecSelfIfFailWithoutException`, `ExecSelfIfFailWithValue`).
- Las variantes `Try*` capturan las excepciones del delegado y las convierten en un fallo del
  `MlResult`, guardando la excepción en `Details["Ex"]`.
- Existen sobrecargas asíncronas para todas las combinaciones de fuente y delegado.
- ⚠️ El método síncrono es `TryExecSelfFail` (no `TryExecSelfIfFail`) y **no existe**
  `TryExecSelfIfFailWithoutException`.
- Los nombres del estado son `IsValid` / `IsFail`.
- `MlErrorsDetails` solo tiene `Errors` y `Details`: consúltalo con `ToErrorsMessages()`,
  `ToErrorsDescription()`, `GetDetailException()` y `GetDetailValue<T>()`.

## Ver también

- [`2_ExecSelfIfValid.md`](./2_ExecSelfIfValid.md) — efectos solo en la rama válida.
- [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md) — efectos solo en la rama fallida.
- [`4_ExecSelfIfFailWithValue.md`](./4_ExecSelfIfFailWithValue.md) — recuperar el valor adjunto al fallo.
- [`5_ExecSelfIfFailWithException.md`](./5_ExecSelfIfFailWithException.md) — fallos con excepción.
- [`6_ExecSelfIfFailWithoutException.md`](./6_ExecSelfIfFailWithoutException.md) — fallos de negocio puros.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia completa con el recuento de sobrecargas.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — `MlError` y `MlErrorsDetails`.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir de la tubería.
- [`../Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md`](../Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md) — la familia `BindSaveValueInDetailsIfFaildFuncResult`, que antes se documentaba aquí por error.