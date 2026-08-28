# `MlResultActionsBind` (`Types/MlResultActionsBind.cs`)

Familia **monádica**: encadena operaciones cuyo resultado es, a su vez, un `MlResult`.

```text
MlResult<T> → (T → MlResult<TReturn>) → MlResult<TReturn>
```

---

## Semántica

- Si el origen es **válido**, ejecuta la función con el valor y devuelve **su** resultado.
- Si el origen es **fallido**, **no ejecuta nada** y propaga los errores tal cual (cortocircuito).
- No se anidan resultados: `Bind` "aplana" `MlResult<MlResult<T>>` en `MlResult<T>`.

**Cuándo usar `Bind` y cuándo `Map`:**

| La función que quieres aplicar devuelve... | Usa |
|---|---|
| `MlResult<TReturn>` (puede fallar) | `Bind` |
| `TReturn` (nunca falla) | [`Map`](./MlResultActionsMap.md) |

---

## Métodos de la clase

| Método | Overloads | Para qué sirve |
|---|---:|---|
| `Bind` | 1 | Encadenamiento base síncrono. |
| `BindAsync` | 4 | Origen y/o función asíncronos. |
| `TryBind` | 2 | Igual que `Bind` capturando excepciones. |
| `TryBindAsync` | 8 | Combinaciones async de `TryBind`. |
| `BindMulti` | 3 | Aplica **varias** funciones al mismo valor. |
| `BindMultiAsync` | 18 | Versiones asíncronas de `BindMulti`. |
| `BindIf` | 2 | Encadena solo si se cumple un predicado. |
| `BindIfAsync` | 11 | Versiones asíncronas de `BindIf`. |
| `TryBindIf` / `TryBindIfAsync` | 3 / 24 | `BindIf` con captura de excepciones. |
| `BindIfFail` | 2 | Recuperación: se ejecuta solo si el origen es `Fail`. |
| `BindIfFailAsync` | 12 | Versiones asíncronas. |
| `TryBindIfFail` / `TryBindIfFailAsync` | 4 / 24 | Recuperación con captura de excepciones. |
| `BindIfFailWithValue` | 2 | Recuperación usando el valor guardado en `Details["Value"]`. |
| `BindIfFailWithValueAsync` | 12 | Versiones asíncronas. |
| `TryBindIfFailWithValue` / `...Async` | 4 / 24 | Idem con captura de excepciones. |
| `BindIfFailWithException` | 4 | Recuperación usando la excepción guardada en `Details["Ex"]`. |
| `BindIfFailWithExceptionAsync` | 24 | Versiones asíncronas. |
| `TryBindIfFailWithException` / `...Async` | 8 / 44 | Idem con captura de excepciones. |
| `BindIfFailWithExceptionError` | 4 | Como el anterior, pero la función recibe **la excepción y los errores**. |
| `BindIfFailWithExceptionErrorAsync` | 24 | Versiones asíncronas. |
| `TryBindIfFailWithExceptionError` / `...Async` | 8 / 44 | Idem con captura de excepciones. |
| `BindIfFailWithoutException` | 1 | Recuperación **solo** si el fallo **no** lleva excepción en `Details["Ex"]` (error de negocio). |
| `BindIfFailWithoutExceptionAsync` | 3 | Versiones asíncronas. |
| `TryBindIfFailWithoutException` / `...Async` | 2 / 4 | Idem con captura de excepciones. |
| `BindAlways` | 2 | Ejecuta la función **siempre**, ignorando el estado del origen (válido o fallido). |
| `BindAlwaysAsync` | 4 + 8 | Versiones asíncronas de las dos formas de `BindAlways`. |
| `TryBindAlways` / `TryBindAlwaysAsync` | 4 / 16 | Idem con captura de excepciones. |
| `BindSaveValueInDetailsIfFaildFuncResult` | 1 | Si la función encadenada falla, guarda el **valor de entrada** en `Details["Value"]`. |
| `BindSaveValueInDetailsIfFaildFuncResultAsync` | 4 | Versiones asíncronas. |
| `TryBindSaveValueInDetailsIfFaildFuncResult` / `...Async` | 2 / 8 | Idem con captura de excepciones. |
| `TryBindBuild` | 3 | Construye una instancia de `TResult` invocando **N funciones** que producen cada argumento del constructor. |
| `TryBindBuildSyncAsync` / `TryBindBuildAsync` | 6 / 3 | Versiones asíncronas de `TryBindBuild`. |
| `TryBindBuildWhile` | 3 | Como `TryBindBuild`, pero **corta en el primer fallo** (`breakInError = true`). |
| `TryBindBuildWhileAsync` | 9 | Versiones asíncronas de `TryBindBuildWhile`. |
| `TryBindBuildTuple` / `TryBindBuildTupleAsync` | muchos | Igual que `TryBindBuild`, pero el resultado es una **tupla** (`(TR1, TR2)`, `(TR1, TR2, TR3)`, …) en lugar de un tipo con constructor. |

> Nota sobre `BindAlways`: existen **dos formas** distintas.
> - `BindAlways<T, TReturn>(Func<MlResult<TReturn>> funcAlways)`: la función **no recibe nada**; se ejecuta igual en válido y en fallido.
> - `BindAlways<T, TResult>(Func<T, MlResult<TResult>> funcValidAlways, Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)`: dos funciones, una para cada rama, pero **ambas devuelven `MlResult<TResult>`**, por lo que la cadena continúa siempre en el mismo tipo.

---

## Ejemplos

### `Bind`: composición de pasos que pueden fallar

Cada paso devuelve `MlResult`, así que el fallo de cualquiera detiene la cadena y conserva el error original.

```csharp
MlResult<OrderConfirmation> result = ValidateRequest(request)   // MlResult<OrderRequest>
    .Bind(req      => LoadCustomer(req.CustomerId))            // MlResult<Customer>
    .Bind(customer => CheckCredit(customer, request.Total))    // MlResult<Customer>
    .Bind(customer => CreateOrder(customer, request.Lines))    // MlResult<Order>
    .Bind(order    => Confirm(order));                         // MlResult<OrderConfirmation>
```

Si `CheckCredit` devuelve `Fail("Crédito insuficiente")`, ni `CreateOrder` ni `Confirm` se ejecutan y `result` contiene ese mismo error.

### `BindAsync`: mezclando síncrono y asíncrono

No hace falta envolver nada a mano: las sobrecargas aceptan origen `Task<MlResult<T>>` y/o función asíncrona.

```csharp
MlResult<InvoiceDto> result = await ValidateId(invoiceId)                        // MlResult<int>
    .BindAsync(id      => repository.FindInvoiceAsync(id))                      // Func async
    .BindAsync(invoice => ApplyTaxes(invoice))                                   // Func sync
    .BindAsync(async invoice => await pdfService.RenderAsync(invoice));          // Func async
```

### `TryBind`: encapsular código que lanza excepciones

Convierte la excepción en `Fail` y la guarda en `Details["Ex"]`, de donde puedes recuperarla con `GetDetailException()`.

```csharp
MlResult<Config> result = pathResult.TryBind(
    func        : path => MlResult<Config>.Valid(JsonSerializer.Deserialize<Config>(File.ReadAllText(path))!),
    errorMessage: ex   => $"No se pudo leer la configuración: {ex.Message}");
```

### `BindIf`: aplicar un paso solo cuando toca

```csharp
// Solo se recalcula el descuento si el cliente es VIP; en otro caso el valor pasa intacto.
MlResult<Order> result = orderResult.BindIf(
    condition: order => order.Customer.IsVip,
    func     : order => ApplyVipDiscount(order));
```

### `BindIfFail`: recuperación con plan B

```csharp
// Si la caché falla por cualquier motivo, se va a la base de datos.
MlResult<Product> result = GetFromCache(sku)
    .BindIfFail(_ => GetFromDatabase(sku));
```

### `BindIfFailWithException`: distinguir errores técnicos

```csharp
MlResult<Product> result = GetFromRemoteApi(sku)
    .BindIfFailWithException(
        // Solo entra aquí si el fallo trae una excepción real (timeout, red, ...)
        funcException: ex => ex is TimeoutException
                                ? GetFromDatabase(sku)
                                : MlResult<Product>.Fail($"Error técnico irrecuperable: {ex.Message}"));
```

### `BindMulti`: varias validaciones sobre el mismo valor

```csharp
// Aplica todas las funciones al mismo origen para no perder ninguna comprobación.
MlResult<Customer> result = customerResult.BindMulti(
    c => ValidateName(c),
    c => ValidateEmail(c),
    c => ValidateAddress(c));
```

### `BindSaveValueInDetailsIfFaildFuncResult`: conservar la entrada del fallo

Muy útil para diagnóstico: si el paso encadenado falla, el valor que se le pasó queda guardado en `Details["Value"]`.

```csharp
MlResult<PaymentReceipt> result = orderResult
    .BindSaveValueInDetailsIfFaildFuncResult(order => paymentGateway.Charge(order));

// Más adelante, al tratar el error:
result.ExecSelfIfFailWithValue<PaymentReceipt, Order>(
    (errors, failedOrder) => logger.LogError("Falló el cobro del pedido {Id}", failedOrder.Id));
```

### `BindIfFailWithoutException`: separar errores de negocio de errores técnicos

`BindIfFailWithException` actúa cuando el fallo **lleva** una excepción; `BindIfFailWithoutException` es su
complementario: actúa cuando el fallo es de **negocio** (validación, regla de dominio) y no hay excepción
guardada en `Details["Ex"]`. Combinando ambos se obtiene un enrutado completo del error:

```csharp
MlResult<Pedido> resultado = await ObtenerPedidoAsync(id)
    // Fallo técnico (timeout, red, BD): reintentamos contra la réplica.
    .BindIfFailWithExceptionAsync(async ex => await ObtenerPedidoDesdeReplicaAsync(id))
    // Fallo de negocio (no existe, cancelado): devolvemos un pedido "vacío" navegable.
    .BindIfFailWithoutExceptionAsync(errores => Pedido.NoDisponible(id, errores.ToErrorsMessages()));
```

Regla práctica: si necesitas **reintentar**, usa `...WithException`; si necesitas **degradar
elegantemente** una regla de negocio, usa `...WithoutException`.

### `BindAlways`: continuar sí o sí

Útil para cerrar una cadena con un paso que debe ejecutarse en cualquier caso, por ejemplo liberar
un recurso, escribir una marca de auditoría o convertir cualquier estado a una respuesta única:

```csharp
// Forma con una sola función: no recibe el valor ni los errores.
MlResult<AuditoriaId> auditoria = await ProcesarLoteAsync(lote)
    .BindAlwaysAsync(() => _auditoria.RegistrarFinDeProcesoAsync(lote.Id));

// Forma con dos funciones: cada rama construye el mismo tipo de salida.
MlResult<RespuestaApi> respuesta = ProcesarLote(lote)
    .BindAlways(funcValidAlways: resumen  => RespuestaApi.Ok(resumen),
                funcFailAlways : errores  => RespuestaApi.ConErrores(errores));
```

> Si lo único que quieres es **observar** (log, métrica) sin cambiar el resultado, no uses `BindAlways`:
> usa [`ExecSelf`](./MlResultActionsExecSelf.md), que devuelve el `MlResult` original intacto.

### `TryBindBuild`: construir un objeto a partir de N funciones

`TryBindBuild<T, TResult>` recibe un `params Func<T, MlResult<object>>[]` con una función por cada
argumento del constructor de `TResult`, las ejecuta todas sobre el valor de entrada y, si todas son
válidas, crea la instancia por reflexión (`Activator.CreateInstance`).

**Importante:** el orden de las funciones debe coincidir **exactamente** con el orden de los parámetros
del constructor de `TResult`.

```csharp
public record FacturaCompleta(Cliente Cliente, IReadOnlyList<Linea> Lineas, Impuestos Impuestos);

MlResult<FacturaCompleta> factura =
    MlResult<int>.Valid(facturaId)
        .TryBindBuild<int, FacturaCompleta>(
            errorMessageBuilder: ex => $"No se pudo componer la factura {facturaId}: {ex.Message}",
            funcArgs: new Func<int, MlResult<object>>[]
            {
                id => CargarCliente(id).ToMlResultObject(),
                id => CargarLineas(id).ToMlResultObject(),
                id => CalcularImpuestos(id).ToMlResultObject()
            });
```

Diferencia clave entre las dos variantes:

| Método | Comportamiento ante un fallo intermedio |
| --- | --- |
| `TryBindBuild` | Ejecuta **todas** las funciones y **acumula** los errores de todas las que fallen. Ideal para mostrar al usuario todos los problemas de una vez. |
| `TryBindBuildWhile` | **Corta en la primera** función que falla. Ideal cuando los pasos son costosos o dependientes. |

### `TryBindBuildTuple`: lo mismo, pero sin tipo destino

Cuando no quieres declarar un tipo solo para agrupar resultados, usa la variante de tupla:

```csharp
MlResult<(Cliente, IReadOnlyList<Linea>)> datos =
    MlResult<int>.Valid(facturaId)
        .TryBindBuildTuple<int, Cliente, IReadOnlyList<Linea>>(
            id => CargarCliente(id),
            id => CargarLineas(id));

// Y después se consume cómodamente por deconstrucción:
string texto = datos.Match(
    valid: t     => $"{t.Item1.Nombre} con {t.Item2.Count} líneas",
    fail : errs  => errs.ToErrorsDescription());
```

Existen sobrecargas para tuplas de 2, 3, 4 y más elementos, y sus equivalentes `TryBindBuildTupleAsync`.

---

## Documentación detallada por concepto

- [Visión general de `MlResultActions`](../Bind/2_MlResultActions.md)
- [`Bind`](../Bind/3_Bind.md)
- [`BindMulti`](../Bind/4_BindMulti.md)
- [`BindIf`](../Bind/5_BindIf.md)
- [`BindIfFail`](../Bind/6_BindIfFail.md)
- [`BindIfFailWithValue`](../Bind/7_BindIfFailWithValue.md)
- [`BindIfFailWithException`](../Bind/8_BindIfFailWithException.md)
- [`BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md)
- [`BindAlways`](../Bind/10_BindAlways.md)
- [`BindSaveValueInDetailsIfFaildFuncResultAsync`](../Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md)
