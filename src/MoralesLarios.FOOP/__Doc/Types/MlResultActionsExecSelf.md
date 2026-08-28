# `MlResultActionsExecSelf` (`Types/MlResultActionsExecSelf.cs`)

Familia para ejecutar **efectos laterales** (logs, métricas, auditoría, notificaciones, invalidación
de caché…) **sin modificar** el `MlResult` que recorre la tubería.

---

## Semántica

Todos los métodos `ExecSelf*` devuelven **el mismo `MlResult` que recibieron**, intacto:

| Operación | Transforma el valor | Cambia el estado | Devuelve |
| --- | --- | --- | --- |
| [`Map`](./MlResultActionsMap.md) | Sí | No (salvo `Try*`) | `MlResult<TReturn>` |
| [`Bind`](./MlResultActionsBind.md) | Sí | Sí | `MlResult<TReturn>` |
| **`ExecSelf`** | **No** | **No** | **`MlResult<T>` original** |
| [`Match`](./MlResultActionsMatch.md) | Sí | — (sale de la tubería) | `TReturn` |

Esto lo convierte en el punto ideal para **instrumentar** una tubería sin contaminar su lógica:
puedes insertar y quitar `ExecSelf` sin que el resto de la cadena cambie ni un tipo.

```csharp
// Insertar trazas no altera el tipo ni el flujo:
MlResult<Pedido> pedido = ObtenerPedido(id)
    .ExecSelfIfValid(p => _log.LogInformation("Pedido {Id} recuperado", p.Id))
    .ExecSelfIfFail (e => _log.LogWarning("Fallo al recuperar el pedido {Id}: {Errores}", id, e.ToErrorsDescription()));
```

---

## Métodos de la clase

| Método | Sobrecargas | Se ejecuta cuando… |
| --- | --- | --- |
| `ExecSelf` | 2 | **Siempre**: recibe dos acciones, una para la rama válida y otra para la fallida. |
| `ExecSelfAsync` | 12 | Versiones asíncronas de `ExecSelf` (origen y/o acciones asíncronas). |
| `TryExecSelf` | 4 | Como `ExecSelf`, capturando excepciones de las acciones. |
| `TryExecSelfAsync` | 18 | Versiones asíncronas de `TryExecSelf`. |
| `ExecSelfIf` | 1 | Según un **predicado** sobre el valor: ejecuta `actionTrue` o `actionFalse`. |
| `TryExecSelfIf` | 2 | Idem con captura de excepciones. |
| `ExecSelfIfValid` | 1 | Solo si `IsValid`. Recibe el valor. |
| `ExecSelfIfValidAsync` | 4 | Versiones asíncronas. |
| `TryExecSelfIfValid` / `TryExecSelfIfValidAsync` | 2 / 8 | Idem con captura de excepciones. |
| `ExecSelfIfFail` | 1 | Solo si `IsFail`. Recibe el `MlErrorsDetails`. |
| `ExecSelfIfFailAsync` | 4 | Versiones asíncronas. |
| `ExecSelfFailAsync` | 2 | Variante asíncrona con nombre abreviado (sin `If`). |
| `TryExecSelfFail` | 2 | Versión síncrona con captura de excepciones. |
| `TryExecSelfFailAsync` | 4 | Versiones asíncronas. |
| `TryExecSelfIfFailAsync` | 8 | Versiones asíncronas con el nombre completo. |
| `ExecSelfIfFailWithValue` | 1 | Solo si `IsFail` **y** hay valor en `Details["Value"]`. |
| `ExecSelfIfFailWithValueAsync` | 4 | Versiones asíncronas. |
| `TryExecSelfIfFailWithValue` / `...Async` | 2 / 8 | Idem con captura de excepciones. |
| `ExecSelfIfFailWithException` | 1 | Solo si `IsFail` **y** hay excepción en `Details["Ex"]`. |
| `ExecSelfIfFailWithExceptionAsync` | 4 | Versiones asíncronas. |
| `TryExecSelfIfFailWithException` / `...Async` | 2 / 8 | Idem con captura de excepciones. |
| `ExecSelfIfFailWithoutException` | 1 | Solo si `IsFail` **y no** hay excepción (fallo de negocio). |
| `ExecSelfIfFailWithoutExceptionAsync` | 4 | Versiones asíncronas. |

> ⚠️ **Dos particularidades reales del código fuente:**
> - **No existe** una versión síncrona llamada `TryExecSelfIfFail`; la síncrona se llama
>   **`TryExecSelfFail`** (sin `If`). Las asíncronas existen con los dos nombres:
>   `TryExecSelfFailAsync` y `TryExecSelfIfFailAsync`.
> - **No existe** `TryExecSelfIfFailWithoutException`: la familia `...WithoutException` solo tiene
>   variantes sin `Try`.

---

## Nombres de parámetros

Usar argumentos nombrados es especialmente útil en esta familia, porque varias sobrecargas reciben
dos acciones del mismo aspecto:

| Método | Parámetros |
| --- | --- |
| `ExecSelf` | `actionValid`, `actionFail` |
| `ExecSelfAsync` | `actionValidAsync`, `actionFailAsync` |
| `ExecSelfIf` | `condition`, `actionTrue`, `actionFalse` |
| `ExecSelfIfValid` | `actionValid` |
| `ExecSelfIfFail` | `actionFail` |
| `ExecSelfIfFailWithValue` | `actionFailValue` |
| `ExecSelfIfFailWithException` | `actionFailException` |
| `ExecSelfIfFailWithoutException` | `actionFail` |
| Variantes `Try*` | añaden `errorMessageBuilder` (`Func<Exception, string>`) o `errorMessage` (`string`) |

---

## Ejemplos

### `ExecSelf`: trazar ambas ramas de una vez

```csharp
MlResult<Pedido> pedido = ProcesarPedido(comanda)
    .ExecSelf(actionValid: p => _log.LogInformation("Pedido {Id} procesado por {Total} €", p.Id, p.Total),
              actionFail : e => _log.LogError("Error procesando la comanda {Ref}: {Errores}",
                                              comanda.Referencia, e.ToErrorsDescription()));
```

### `ExecSelfAsync`: instrumentación asíncrona

```csharp
MlResult<Pedido> pedido = await ProcesarPedidoAsync(comanda)
    .ExecSelfAsync(actionValidAsync: async p => await _metricas.IncrementarAsync("pedidos.ok", p.Total),
                   actionFailAsync : async e => await _alertas.NotificarAsync("pedidos.error", e.ToErrorsMessages()));
```

### `ExecSelfIfValid` / `ExecSelfIfFail`: una sola rama

Cuando solo te interesa un lado, estas variantes son más legibles que pasar una acción vacía:

```csharp
MlResult<Factura> factura = EmitirFactura(pedido)
    .ExecSelfIfValid(f => _cache.Invalidar($"facturas:{f.ClienteId}"))
    .ExecSelfIfFail (e => _log.LogWarning("Factura no emitida: {Errores}", e.ToErrorsDescription()));
```

### `ExecSelfIf`: efecto condicional sobre el valor

```csharp
MlResult<Pedido> pedido = ObtenerPedido(id)
    .ExecSelfIf(condition  : p => p.Total > 10_000m,
                actionTrue : p => _log.LogWarning("Pedido de importe elevado: {Id} ({Total} €)", p.Id, p.Total),
                actionFalse: p => _log.LogDebug("Pedido ordinario: {Id}", p.Id));
```

### `TryExecSelf*`: cuando el propio efecto puede fallar

Escribir en un fichero, publicar en una cola o llamar a un servicio externo puede lanzar. Si no
quieres que un fallo de instrumentación tumbe la operación de negocio, usa la variante `Try*`: la
excepción se convierte en `Fail` y queda registrada en `Details["Ex"]`.

```csharp
MlResult<Pedido> pedido = ProcesarPedido(comanda)
    .TryExecSelfFail(actionFail         : e  => _ficheroAuditoria.Escribir(e.ToErrorsDescription()),
                     errorMessageBuilder: ex => $"No se pudo auditar el fallo del pedido: {ex.Message}");
```

### `ExecSelfIfFailWithException`: reaccionar solo a errores técnicos

```csharp
MlResult<Cotizacion> cotizacion = await ObtenerCotizacionAsync(divisa)
    .ExecSelfIfFailWithExceptionAsync(async ex => await _telemetria.RegistrarExcepcionAsync(ex, new
     {
         Operacion = nameof(ObtenerCotizacionAsync),
         Divisa    = divisa
     }));
```

### `ExecSelfIfFailWithoutException`: reaccionar solo a errores de negocio

Complementario del anterior. Sirve para métricas de negocio, que no deben mezclarse con las de
infraestructura:

```csharp
MlResult<Cotizacion> cotizacion = await ObtenerCotizacionAsync(divisa)
    .ExecSelfIfFailWithoutExceptionAsync(async e => await _metricas.IncrementarAsync("cotizaciones.rechazadas"));
```

### `ExecSelfIfFailWithValue`: registrar la entrada que provocó el fallo

Requiere que un paso anterior haya guardado el valor en `Details["Value"]` (por ejemplo con
`AddValueIfFail` o `BindSaveValueInDetailsIfFaildFuncResult`):

```csharp
MlResult<Precio> precio = CalcularPrecio(articulo)
    .AddValueIfFail(articulo)
    .ExecSelfIfFailWithValue<Precio, Articulo>(art =>
        _log.LogError("No se pudo calcular el precio del artículo {Sku}", art.Sku));
```

---

## Ejemplo completo: tubería instrumentada de extremo a extremo

```csharp
public async Task<IActionResult> ConfirmarPedidoAsync(int pedidoId)
{
    using var actividad = _telemetria.IniciarActividad("ConfirmarPedido");

    return await _repo.ObtenerPedidoAsync(pedidoId)
        .ExecSelfIfValidAsync(async p => await _telemetria.MarcarAsync("pedido.cargado", p.Id))
        .MapEnsureAsync      (p => p.Estado == EstadoPedido.Pendiente, "El pedido no está pendiente")
        .BindAsync           (p => _stock.ReservarAsync(p.Lineas).MapAsync(_ => p))
        .ExecSelfIfFailWithExceptionAsync (async ex => await _alertas.CriticaAsync("stock.caido", ex))
        .ExecSelfIfFailWithoutExceptionAsync(async e  => await _metricas.IncrementarAsync("pedidos.rechazados"))
        .TryBindAsync        (funcAsync          : p  => _repo.ConfirmarAsync(p),
                              errorMessageBuilder: ex => $"Error al confirmar el pedido {pedidoId}: {ex.Message}")
        .ExecSelfIfValidAsync(async p => await _notificaciones.EnviarConfirmacionAsync(p))
        .MatchAsync          (valid: p       => Ok(new { p.Id, p.Estado }),
                              fail : errores => BadRequest(errores.ToErrorsMessages()));
}
```

Observa que **todos los `ExecSelf*` podrían eliminarse** y la tubería seguiría compilando y
funcionando igual: la instrumentación es completamente ortogonal a la lógica.

---

## Documentación detallada por concepto

- [1. `ExecSelf`](../ExecSelf/1_ExecSelf.md)
- [2. `ExecSelfIfValid`](../ExecSelf/2_ExecSelfIfValid.md)
- [3. `ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md)
- [4. `ExecSelfIfFailWithValue`](../ExecSelf/4_ExecSelfIfFailWithValue.md)
- [5. `ExecSelfIfFailWithException`](../ExecSelf/5_ExecSelfIfFailWithException.md)
- [6. `ExecSelfIfFailWithoutException`](../ExecSelf/6_ExecSelfIfFailWithoutException.md)

## Ver también

- [`MlResultActionsMap`](./MlResultActionsMap.md) — transformar el valor válido.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — encadenar operaciones que pueden fallar.
- [`MlResultActionsMatch`](./MlResultActionsMatch.md) — salir de la tubería.
- [`MlErrorsDetails`](./MlResultErrors.md) — el objeto de error que reciben las acciones `fail`.
