# ExecSelfIfFailWithException — Efectos secundarios solo ante fallos técnicos

## Índice
1. [Introducción](#introducción)
2. [Cómo llega la excepción a los detalles](#cómo-llega-la-excepción-a-los-detalles)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con las demás variantes](#comparación-con-las-demás-variantes)
8. [Resumen](#resumen)
9. [Ver también](#ver-también)

---

## Introducción

En una tubería de `MlResult<T>` conviven dos clases de fallo muy distintas:

| Tipo de fallo | Origen | Ejemplo | Qué hacer |
| --- | --- | --- | --- |
| **De negocio** | Una regla que no se cumple | «El importe supera el límite» | Informar al usuario, `LogWarning` |
| **Técnico** | Una excepción capturada | `SqlException`, `TimeoutException` | `LogError` con *stack trace*, alerta, métrica |

`ExecSelfIfFailWithException` ejecuta una acción **solo cuando el fallo es técnico**, es decir, cuando
hay una excepción guardada en `MlErrorsDetails.Details` bajo la clave convencional `"Ex"`
(`Constants.EX_DESC_KEY`), y te la entrega ya tipada como `Exception`:

```csharp
resultado.ExecSelfIfFailWithException((errores, ex) =>
    _log.LogError(ex, "Fallo técnico: {E}", errores.ToErrorsDescription()));
```

Si el resultado es válido, o es fallido pero **sin** excepción, la acción **no se ejecuta** y el
resultado se propaga intacto. Su complemento exacto es
[`ExecSelfIfFailWithoutException`](./6_ExecSelfIfFailWithoutException.md).

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** las propiedades `Exception`, `HasException`,
> `AllErrors`, `FirstErrorMessage` ni `HasValue`. La excepción se obtiene con `GetDetailException()`
> —que devuelve `MlResult<Exception>`— o directamente a través de esta familia de métodos.

---

## Cómo llega la excepción a los detalles

No hace falta que la guardes a mano: **todos los métodos `Try*` de la librería lo hacen por ti** al
capturar una excepción.

| Origen | Qué guarda |
| --- | --- |
| `TryBind`, `TryMap`, `TryMatch`, `TryExecSelf*`… | La excepción capturada en `Details["Ex"]` |
| `MlResult.Fail<T>(mensaje, exception)` | Idem, de forma explícita |
| `MlErrorsDetails.FromErrorMessageWithException(mensaje, ex)` | Idem |
| `AppendExDetails(this Dictionary<string, object>, Exception)` | Añade `Ex`, y si ya existe, `Ex2`, `Ex3`… |

Es decir: si en tu tubería hay un `TryBindAsync` y salta una `SqlException`, el fallo resultante ya
viene «marcado» como técnico y `ExecSelfIfFailWithException` lo detectará.

```csharp
var resultado = ObtenerCliente(id)
    .TryBind(c => _repositorio.Guardar(c),      // ← si lanza, la excepción va a Details["Ex"]
             ex => $"No se pudo guardar el cliente: {ex.Message}")

    .ExecSelfIfFailWithException((errores, ex) =>
        _log.LogError(ex, "Persistencia caída para el cliente {Id}", id));
```

> 📌 Cuando se acumulan varias excepciones, las claves son `Ex`, `Ex2`, `Ex3`… Esta familia trabaja
> con la principal (`"Ex"`). Para obtener una de un tipo concreto usa
> `errores.GetDetailException<SqlException>()`.

---

## Firmas reales

```csharp
// Síncrono
public static MlResult<T> ExecSelfIfFailWithException<T>(
        this MlResult<T>                    source,
        Action<MlErrorsDetails, Exception>  actionFailWithException)

// Con captura de excepciones en el propio efecto secundario
public static MlResult<T> TryExecSelfIfFailWithException<T>(
        this MlResult<T>                    source,
        Action<MlErrorsDetails, Exception>  actionFailWithException,
        Func<Exception, string>             errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithException<T>(
        this MlResult<T>                    source,
        Action<MlErrorsDetails, Exception>  actionFailWithException,
        string                              exceptionAditionalMessage = null!)
```

A diferencia de [`ExecSelfIfFailWithValue`](./4_ExecSelfIfFailWithValue.md), aquí solo hay **un**
genérico: el tipo de la excepción es siempre `Exception`.

**Comportamiento**:

| Estado de `source` | `Details["Ex"]` | ¿Se ejecuta la acción? | Resultado |
| --- | --- | :---: | --- |
| Válido | — | No | El mismo, válido |
| Fallido | Existe | Sí | El mismo, fallido |
| Fallido | No existe | No | El mismo, fallido |

---

## Variantes asíncronas

| Fuente | Delegado | Método |
| --- | --- | --- |
| `MlResult<T>` | `Action<MlErrorsDetails, Exception>` | `ExecSelfIfFailWithException` |
| `MlResult<T>` | `Func<MlErrorsDetails, Exception, Task>` | `ExecSelfIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `Action<MlErrorsDetails, Exception>` | `ExecSelfIfFailWithExceptionAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, Exception, Task>` | `ExecSelfIfFailWithExceptionAsync` |

Y las 8 sobrecargas de `TryExecSelfIfFailWithExceptionAsync` (4 combinaciones × 2 formas de indicar
el mensaje de error).

```csharp
await ConsultarProveedorAsync(referencia)
    .ExecSelfIfFailWithExceptionAsync(async (errores, ex) =>
        await _alertas.EnviarAsync($"Proveedor caído: {ex.GetType().Name} — {ex.Message}"));
```

---

## Ejemplos Prácticos

### Ejemplo 1: Separar fallos técnicos de fallos de negocio

Es el patrón estrella de esta familia: dos efectos secundarios excluyentes, sin un solo `if`.

```csharp
public class ServicioPedidos
{
    private readonly ILogger<ServicioPedidos> _log;
    private readonly IAlertas  _alertas;
    private readonly IMetricas _metricas;

    public Task<MlResult<Pedido>> ConfirmarAsync(Guid pedidoId)
        => ObtenerPedidoAsync(pedidoId)
            .BindAsync(p => ValidarConfirmableAsync(p))
            .BindAsync(p => ReservarStockAsync(p))
            .BindAsync(p => CobrarAsync(p))

            // Rama técnica: hay excepción → error + alerta + métrica de infraestructura.
            .ExecSelfIfFailWithExceptionAsync(async (errores, ex) =>
            {
                _log.LogError(ex, "Error técnico confirmando el pedido {Id}", pedidoId);
                _metricas.Incrementar("pedidos.error_tecnico", ("tipo", ex.GetType().Name));
                await _alertas.EnviarAsync($"Confirmación de pedidos con incidencias: {ex.Message}");
            })

            // Rama de negocio: no hay excepción → simple aviso, sin despertar a nadie.
            .ExecSelfIfFailWithoutExceptionAsync(errores =>
            {
                _log.LogWarning("Pedido {Id} no confirmable: {E}",
                                pedidoId, errores.ToErrorsDescription());
                _metricas.Incrementar("pedidos.rechazado_negocio");
                return Task.CompletedTask;
            });
}
```

**Nunca se ejecutan las dos**: son mutuamente excluyentes por construcción.

### Ejemplo 2: Política de reintentos según el tipo de excepción

```csharp
public Task<MlResult<Cotizacion>> ObtenerCotizacionAsync(string simbolo)
    => ConsultarProveedorAsync(simbolo)
        .ExecSelfIfFailWithExceptionAsync(async (errores, ex) =>
        {
            // Solo los fallos transitorios merecen un reintento.
            var reintentable = ex switch
            {
                TimeoutException                                        => true,
                HttpRequestException                                    => true,
                IOException                                             => true,
                _ when ex.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase) => true,
                _                                                       => false
            };

            if (reintentable)
            {
                _log.LogWarning(ex, "Fallo transitorio consultando {Simbolo}; se reintentará", simbolo);
                await _colaReintentos.EncolarAsync(simbolo, TimeSpan.FromSeconds(30));
            }
            else
            {
                _log.LogError(ex, "Fallo permanente consultando {Simbolo}", simbolo);
                await _incidencias.AbrirAsync($"Cotización {simbolo}", ex);
            }
        });
```

### Ejemplo 3: Traducir la excepción a un código HTTP en el controlador

```csharp
[HttpPost("pedidos/{id}/confirmar")]
public async Task<IActionResult> Confirmar(Guid id)
{
    var resultado = await _servicio.ConfirmarAsync(id);

    return await resultado
        // Efecto: dejar traza de la excepción antes de responder.
        .ExecSelfIfFailWithExceptionAsync((errores, ex) =>
        {
            _log.LogError(ex, "Confirmación fallida para {Id}", id);
            return Task.CompletedTask;
        })

        // Salida: traducimos estado a respuesta HTTP.
        .MatchAsync(
            valid: p       => Task.FromResult<IActionResult>(Ok(p)),
            fail:  errores => Task.FromResult<IActionResult>(
                errores.GetDetailException().Match(
                    valid: ex => ex switch
                    {
                        TimeoutException            => StatusCode(504, "El servicio tardó demasiado"),
                        HttpRequestException        => StatusCode(502, "Dependencia no disponible"),
                        UnauthorizedAccessException => Forbid(),
                        _                           => StatusCode(500, "Error interno")
                    },
                    // Sin excepción → es un fallo de negocio: 400 con los mensajes.
                    fail: _ => BadRequest(errores.ToErrorsMessages()))));
}
```

### Ejemplo 4: Enriquecer un sistema de diagnóstico (efecto que puede lanzar)

```csharp
public Task<MlResult<Informe>> GenerarAsync(Peticion peticion)
    => CargarDatosAsync(peticion)
        .BindAsync(d => CalcularAsync(d))
        .BindAsync(c => RenderizarAsync(c))

        // Enviar a la telemetría remota puede fallar: si falla, queremos saberlo.
        .TryExecSelfIfFailWithExceptionAsync(
            async (errores, ex) => await _telemetria.RegistrarAsync(new Traza
            {
                Operacion   = nameof(GenerarAsync),
                TipoExcepcion = ex.GetType().FullName!,
                Mensaje     = ex.Message,
                Pila        = ex.StackTrace,
                Contexto    = errores.ToDetailsDescription(),   // Todo el diccionario Details
                Momento     = DateTimeOffset.UtcNow
            }),
            ex => $"El informe falló y además no se pudo enviar la telemetría: {ex.Message}");
```

> ⚠️ Con `Try*`, si el envío de telemetría lanza, la nueva excepción se **añade** a los errores
> existentes (quedará como `Ex2`). El resultado sigue fallido, pero con más contexto.

---

## Mejores Prácticas

### 1. Úsalo emparejado con `ExecSelfIfFailWithoutException`

Juntos cubren el 100 % de los fallos sin solaparse. Es la forma más limpia de dar a cada tipo de
fallo el tratamiento que merece.

### 2. Registra la excepción como excepción, no como texto

```csharp
// ❌ Se pierden el tipo, la pila y la excepción interna.
.ExecSelfIfFailWithException((e, ex) => _log.LogError("Error: {M}", ex.Message));

// ✅ El logger conserva toda la información.
.ExecSelfIfFailWithException((e, ex) => _log.LogError(ex, "Error al procesar el pedido {Id}", id));
```

### 3. No confundas «hay excepción» con «es grave»

Un `ValidationException` capturado por un `TryMap` acabará en `Details["Ex"]` aunque
conceptualmente sea un fallo de negocio. Si necesitas más finura, filtra por tipo dentro del
delegado o usa `GetDetailException<TConcreta>()`.

### 4. `Try*` solo cuando el fallo del efecto importe

Para un `LogError` local basta `ExecSelfIfFailWithException`. Para telemetría remota, alertas o
persistencia, `TryExecSelfIfFailWithException`.

### 5. Sigue sin recuperar el resultado

Esta familia **observa**. Para recuperar según la excepción, usa
[`BindIfFailWithException`](../Bind/8_BindIfFailWithException.md) o
[`MapIfFailWithException`](../Map/6_MapIfFailWithException.md).

---

## Comparación con las demás variantes

| Método | Se ejecuta si… | El delegado recibe | ¿Cambia el resultado? |
| --- | --- | --- | :---: |
| `ExecSelfIfFail` | Es fallido (siempre) | `MlErrorsDetails` | No |
| **`ExecSelfIfFailWithException`** | **Fallido y hay `Details["Ex"]`** | **`MlErrorsDetails`, `Exception`** | **No** |
| `ExecSelfIfFailWithoutException` | Fallido y **sin** `Details["Ex"]` | `MlErrorsDetails` | No |
| `ExecSelfIfFailWithValue` | Fallido y hay `Details["Value"]` | `MlErrorsDetails`, `TValue` | No |
| `TryExecSelfIfFailWithException` | Fallido y hay excepción | `MlErrorsDetails`, `Exception` | Solo si el delegado lanza |
| `MapIfFailWithException` | Fallido y hay excepción | `MlErrorsDetails`, `Exception` | **Sí**: devuelve un valor |
| `BindIfFailWithException` | Fallido y hay excepción | `MlErrorsDetails`, `Exception` | **Sí**: devuelve otro `MlResult` |

---

## Resumen

- `ExecSelfIfFailWithException` ejecuta una acción **solo ante fallos técnicos**: resultado fallido
  **con** excepción en `Details["Ex"]`.
- La excepción la guardan automáticamente todos los métodos `Try*` de la librería.
- Solo tiene **un** genérico (`<T>`); la excepción llega siempre como `Exception`.
- Es complementario y excluyente con `ExecSelfIfFailWithoutException`; usarlos juntos cubre todos los
  fallos.
- Existen las cuatro combinaciones asíncronas y las variantes `Try*`.
- No recupera el resultado: para eso están `MapIfFailWithException` y `BindIfFailWithException`.

## Ver también

- [`1_ExecSelf.md`](./1_ExecSelf.md) — visión general de la familia.
- [`2_ExecSelfIfValid.md`](./2_ExecSelfIfValid.md) — efectos en la rama válida.
- [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md) — efectos ante cualquier fallo.
- [`4_ExecSelfIfFailWithValue.md`](./4_ExecSelfIfFailWithValue.md) — el valor adjunto al fallo.
- [`6_ExecSelfIfFailWithoutException.md`](./6_ExecSelfIfFailWithoutException.md) — el complemento exacto de este documento.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y las claves convencionales.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException()`, `GetDetailException<T>()`.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia y recuento de sobrecargas.
- [`../Bind/8_BindIfFailWithException.md`](../Bind/8_BindIfFailWithException.md) y [`../Map/6_MapIfFailWithException.md`](../Map/6_MapIfFailWithException.md) — cuando sí quieres recuperar.