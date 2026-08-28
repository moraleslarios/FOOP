# `MlResultActions` (`Types/MlResultActions.cs`)

Clase de extensiones **transversales**: enriquecer errores con contexto, completar resultados con
datos adicionales y acceder de forma **segura** al valor o a los errores de un `MlResult<T>`.

Es la clase que resuelve las necesidades "de fontanería" que aparecen al construir tuberías reales:
adjuntar el identificador de la petición a un error, transportar dos valores a la vez, o leer el valor
válido dentro de un método sin salir del modelo funcional.

---

## Métodos de la clase

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `AddMlErrorDetailIfFail` | 1 | Si el resultado es `Fail`, añade una entrada al diccionario `Details`. Si es válido, no hace nada. |
| `AddMlErrorDetailIfFailAsync` | 2 | Versiones asíncronas. |
| `AddValueDetailIfFail` | 1 | Si el resultado es `Fail`, guarda un valor bajo la clave convencional `"Value"`. |
| `AddValueDetailIfFailAsync` | 2 | Versiones asíncronas. |
| `CompleteWithDataValueIfValid` | 1 | Si es válido, combina el valor actual con un dato adicional, produciendo un resultado compuesto. |
| `CompleteWithDataValueIfValidAsync` | 4 | Versiones asíncronas. |
| `CompleteWithDetailsValueIfFail` | 1 | Si es fallido, completa los detalles del error con el valor indicado. |
| `CompleteWithDetailsValueIfFailAsync` | 1 | Versión asíncrona. |
| `CompleteWithDataValue` | 1 | Combina el resultado con un dato adicional, actuando tanto en la rama válida como en la fallida. |
| `CompleteWithDataValueAsync` | 4 | Versiones asíncronas. |
| `SecureValidValue` | 1 | Acceso **seguro** al valor válido: devuelve `MlResult<T>` en lugar de lanzar o exponer `Value`. |
| `SecureValidValueAsync` | 1 | Versión asíncrona. |
| `SecureFailErrorsDetails` | 1 | Acceso **seguro** al `MlErrorsDetails`: si el resultado es válido, devuelve `Fail` indicándolo. |
| `SecureFailErrorsDetailsAsync` | 2 | Versiones asíncronas. |
| `CreateCompleteMlResult` | 3 | Crea un resultado que **transporta dos valores a la vez** (el actual y uno nuevo), fusionando errores si alguno falla. |
| `CreateCompleteMlResultAsync` | 8 | Versiones asíncronas. |

---

## Enriquecer errores con contexto

### `AddMlErrorDetailIfFail`

El problema clásico: un método de bajo nivel produce un error correcto pero **sin contexto**
("Timeout"), y quien lo lee en el log no sabe a qué petición correspondía. Esta extensión añade el
contexto solo cuando hace falta, sin ramificar el código:

```csharp
MlResult<Tarifa> tarifa = await _servicio.ObtenerTarifaAsync(divisa)
    .AddMlErrorDetailIfFailAsync("Divisa"      , divisa)
    .AddMlErrorDetailIfFailAsync("CorrelationId", _contexto.CorrelationId);

// Si falla, ErrorsDetails.Details contiene ahora "Divisa" y "CorrelationId".
// Si no falla, la llamada es un no-op sin coste conceptual.
```

Después, en el punto de log, todo el contexto está disponible de golpe:

```csharp
tarifa.ExecSelfIfFail(e => _log.LogError("Error de tarifa: {Detalle}", e.ToDescription()));
```

### `AddValueDetailIfFail`

Variante especializada que guarda un valor bajo la clave convencional `"Value"` (`Constants.VALUE_KEY`).
Es lo que habilita las familias `*IfFailWithValue` de `Bind`, `Map` y `ExecSelf`:

```csharp
MlResult<Precio> precio = CalcularPrecio(articulo)
    .AddValueDetailIfFail(articulo)                     // guarda el artículo en Details["Value"]
    .MapIfFailWithValue<Precio, Articulo>(art => Precio.Estimado(art.CategoriaBase));
```

---

## Transportar datos adicionales

### `CreateCompleteMlResult`

Cuando en medio de una tubería necesitas **dos valores a la vez** (el que ya llevabas y uno nuevo),
`CreateCompleteMlResult` los junta en un resultado compuesto. Si alguno de los dos falla, **fusiona
los errores** en lugar de perder uno.

```csharp
// Llevamos el pedido, y ahora necesitamos también el cliente para calcular el descuento.
MlResult<Pedido> pedidoResult = ObtenerPedido(pedidoId);

var pedidoYCliente = pedidoResult
    .CreateCompleteMlResult(ObtenerCliente(pedidoResult.Match(valid: p => p.ClienteId, fail: _ => 0)));

MlResult<decimal> total = pedidoYCliente
    .Map(par => CalcularTotal(par.Item1, par.Item2));
```

En la práctica, para juntar resultados **independientes** suele ser más directo usar
[`Combine`](./MlResultActionsSeveral.md); `CreateCompleteMlResult` brilla cuando el segundo resultado
**depende** del primero.

### `CompleteWithDataValueIfValid` y `CompleteWithDataValue`

Añaden un dato al resultado, con la diferencia de en qué rama actúan:

| Método | Actúa en |
| --- | --- |
| `CompleteWithDataValueIfValid` | Solo en la rama válida. |
| `CompleteWithDataValue` | En ambas ramas (en la fallida, el dato se refleja en los detalles). |
| `CompleteWithDetailsValueIfFail` | Solo en la rama fallida, completando `Details`. |

```csharp
MlResult<(Pedido pedido, Usuario usuario)> conUsuario = ObtenerPedido(pedidoId)
    .CompleteWithDataValueIfValid(_contexto.UsuarioActual);
```

---

## Acceso seguro al contenido

`MlResult<T>.Value` y `MlResult<T>.ErrorsDetails` son `internal protected`: **no son accesibles desde
tu código de aplicación**, y eso es intencionado. Cuando de verdad necesitas leerlos (por ejemplo,
dentro de un método privado que ya ha comprobado el estado), usa estas extensiones.

### `SecureValidValue`

```csharp
MlResult<Cliente> resultado = ObtenerCliente(id);

// Devuelve el propio valor envuelto si es válido; si es fallido, propaga el error.
MlResult<Cliente> valor = resultado.SecureValidValue();
```

Frente al acceso directo, `SecureValidValue` **nunca lanza**: el caso "no hay valor" se representa
como `Fail`, coherente con el resto de la librería.

### `SecureFailErrorsDetails`

El simétrico: obtiene el `MlErrorsDetails` de un resultado fallido. Si el resultado **es válido**,
devuelve un `Fail` indicando que no hay errores que leer.

```csharp
MlResult<MlErrorsDetails> errores = resultado.SecureFailErrorsDetails();

string informe = errores.Match(
    valid: e => e.ToDescription(),
    fail : _ => "La operación se completó sin errores.");
```

> 💡 **Preferencia de estilo:** en el 95 % de los casos no necesitas ninguna de estas dos
> extensiones. Usa [`Match`](./MlResultActionsMatch.md), que resuelve ambas ramas de una vez y es más
> legible. Reserva `Secure*` para código de infraestructura y helpers internos.

---

## Ejemplo completo

```csharp
public async Task<IActionResult> AplicarDescuentoAsync(int pedidoId, string cupon)
{
    return await _repo.ObtenerPedidoAsync(pedidoId)
        // Contexto para el diagnóstico, solo si algo falla.
        .AddMlErrorDetailIfFailAsync("PedidoId", pedidoId)
        .AddMlErrorDetailIfFailAsync("Cupon"   , cupon)
        // Necesitamos el cupón además del pedido: resultado compuesto.
        .CreateCompleteMlResultAsync(_cupones.ValidarAsync(cupon))
        .MapAsync(par => par.Item1 with { Descuento = par.Item2.Porcentaje })
        .TryBindAsync(funcAsync          : p  => _repo.GuardarAsync(p),
                      errorMessageBuilder: ex => $"Error al guardar el pedido {pedidoId}: {ex.Message}")
        .ExecSelfIfFailAsync(e => _log.LogWarning("Descuento no aplicado: {Detalle}", e.ToDescription()))
        .MatchAsync(valid: p       => Ok(new { p.Id, p.Descuento, p.Total }),
                    fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

---

## Ver también

- [`MlResult<T>`](./MlResult.md) — visibilidad de `Value` y `ErrorsDetails`.
- [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) — lectura tipada de los detalles añadidos aquí.
- [`MlResultErrors`](./MlResultErrors.md) — `MlError`, `MlErrorsDetails` y sus operaciones.
- [`MlResultActionsSeveral`](./MlResultActionsSeveral.md) — `Combine` para resultados independientes.
- [`MlResultActionsMatch`](./MlResultActionsMatch.md) — la alternativa recomendada a `Secure*`.
