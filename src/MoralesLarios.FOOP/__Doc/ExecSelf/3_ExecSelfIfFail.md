# ExecSelfIfFail — Efectos secundarios solo en la rama fallida

## Índice
1. [Introducción](#introducción)
2. [Firmas reales](#firmas-reales)
3. [Cómo consultar los errores](#cómo-consultar-los-errores)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [Particularidad real del código fuente](#particularidad-real-del-código-fuente)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con las demás variantes](#comparación-con-las-demás-variantes)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

`ExecSelfIfFail` ejecuta una acción **únicamente si el resultado es fallido** y devuelve **el mismo
`MlResult<T>`** que recibió, intacto. Si el resultado es válido, la acción **no se ejecuta**.

Es el simétrico de [`ExecSelfIfValid`](./2_ExecSelfIfValid.md) y el sitio natural para el *logging*
de errores, las alertas y las métricas de fallo, sin ensuciar la tubería de negocio:

```csharp
MlResult<Pedido> resultado = ProcesarPedido(dto)
    .ExecSelfIfFail(errores => _log.LogWarning("Pedido rechazado: {E}", errores.ToErrorsDescription()));

// Si ProcesarPedido fue bien, el log NO se escribe y `resultado` sigue siendo válido.
```

**Punto clave:** `ExecSelfIfFail` **no recupera** el resultado. Sigue fallido después de ejecutarse.
Si lo que quieres es *convertir el fallo en un valor*, la operación correcta es
[`MapIfFail`](../Map/4_MapIfFail.md) o [`BindIfFail`](../Bind/6_BindIfFail.md).

```csharp
// ❌ Esto NO recupera nada: el resultado sigue fallido.
var r = Consultar(id).ExecSelfIfFail(_ => valorPorDefecto);

// ✅ Para recuperar, usa MapIfFail / BindIfFail.
var r = Consultar(id).MapIfFail(_ => valorPorDefecto);
```

---

## Firmas reales

```csharp
// Síncrono
public static MlResult<T> ExecSelfIfFail<T>(this MlResult<T>       source,
                                            Action<MlErrorsDetails> actionFail)

// Con captura de excepciones (ojo al nombre: TryExecSelfFail, sin "If")
public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>        source,
                                            Action<MlErrorsDetails> actionFail,
                                            Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfFail<T>(this MlResult<T>        source,
                                            Action<MlErrorsDetails> actionFail,
                                            string                  exceptionAditionalMessage = null!)
```

**Comportamiento**:

| Estado de `source` | ¿Se ejecuta `actionFail`? | Resultado devuelto |
| --- | :---: | --- |
| Válido | No | El mismo `MlResult<T>` válido |
| Fallido | Sí | El mismo `MlResult<T>` fallido, **con los mismos errores** |
| Fallido y `actionFail` lanza (`ExecSelfIfFail`) | Sí | La excepción **sube** |
| Fallido y `actionFail` lanza (`TryExecSelfFail`) | Sí | `MlResult<T>` fallido; la excepción se añade a `Details["Ex"]` |

---

## Cómo consultar los errores

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone dos propiedades: `Errors`
> (`IEnumerable<MlError>`) y `Details` (`Dictionary<string, object>`). **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`.

| Quieres… | Usa |
| --- | --- |
| Todos los mensajes como colección | `errores.ToErrorsMessages()` |
| Todos los mensajes en un solo texto | `errores.ToErrorsDescription()` |
| Solo el primer mensaje | `errores.Errors.First().Message` |
| La excepción original, si la hay | `errores.GetDetailException()` → `MlResult<Exception>` |
| Una excepción de un tipo concreto | `errores.GetDetailException<TimeoutException>()` |
| El valor que provocó el fallo | `errores.GetDetailValue<T>()` |
| Un dato arbitrario adjunto | `errores.GetDetail<T>("miClave")` |

Todos los `Get*` devuelven `MlResult<...>`, nunca lanzan. Para leerlos, encadena un `Match`:

```csharp
.ExecSelfIfFail(errores =>
{
    var nivel = errores.GetDetailException()
                       .Match(valid: _  => LogLevel.Error,     // Fallo técnico
                              fail:  _  => LogLevel.Warning);  // Fallo de negocio

    _log.Log(nivel, "Operación fallida: {E}", errores.ToErrorsDescription());
});
```

---

## Variantes asíncronas

| Fuente | Delegado | Método |
| --- | --- | --- |
| `MlResult<T>` | `Action<MlErrorsDetails>` | `ExecSelfIfFail` |
| `MlResult<T>` | `Func<MlErrorsDetails, Task>` | `ExecSelfIfFailAsync` |
| `Task<MlResult<T>>` | `Action<MlErrorsDetails>` | `ExecSelfIfFailAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, Task>` | `ExecSelfIfFailAsync` |

Además existe `ExecSelfFailAsync` (2 sobrecargas) como alias asíncrono, y las variantes seguras
`TryExecSelfFailAsync` (4) y `TryExecSelfIfFailAsync` (8).

```csharp
var resultado = await ProcesarPedidoAsync(dto)
    .ExecSelfIfFailAsync(async errores =>
        await _alertas.EnviarAsync($"Pedido rechazado: {errores.ToErrorsDescription()}"))
    .BindAsync(p => FacturarAsync(p));   // No se ejecuta si hubo fallo
```

---

## Particularidad real del código fuente

| Esperarías | Realidad |
| --- | --- |
| `TryExecSelfIfFail` (síncrono) | **No existe.** El método síncrono se llama **`TryExecSelfFail`** (sin el `If`). |
| Solo un nombre asíncrono | Existen **ambos**: `TryExecSelfFailAsync` y `TryExecSelfIfFailAsync`. |

Es una asimetría heredada de la evolución del código. Si el compilador no encuentra
`TryExecSelfIfFail`, usa `TryExecSelfFail`.

---

## Ejemplos Prácticos

### Ejemplo 1: Log con nivel según el tipo de fallo

```csharp
public class ServicioFacturacion
{
    private readonly ILogger<ServicioFacturacion> _log;
    private readonly IAlertas _alertas;

    public Task<MlResult<Factura>> EmitirAsync(Guid pedidoId)
        => ObtenerPedidoAsync(pedidoId)
            .BindAsync(p => ValidarFacturableAsync(p))
            .BindAsync(p => CalcularImpuestosAsync(p))
            .BindAsync(p => GuardarFacturaAsync(p))

            .ExecSelfIfFailAsync(async errores =>
            {
                // ¿Hay excepción? Entonces es un fallo técnico: nivel Error + alerta.
                await errores.GetDetailException()
                    .Match(
                        valid: async ex =>
                        {
                            _log.LogError(ex, "Fallo técnico al facturar el pedido {Id}", pedidoId);
                            await _alertas.EnviarAsync($"Facturación caída: {ex.Message}");
                        },
                        fail: async _ =>
                        {
                            // Sin excepción: es una regla de negocio, no una avería.
                            _log.LogWarning("Pedido {Id} no facturable: {E}",
                                            pedidoId, errores.ToErrorsDescription());
                            await Task.CompletedTask;
                        });
            });
}
```

> 💡 Si esta separación técnico/negocio es habitual en tu código, tienes variantes dedicadas que
> hacen el filtro por ti: [`ExecSelfIfFailWithException`](./5_ExecSelfIfFailWithException.md) y
> [`ExecSelfIfFailWithoutException`](./6_ExecSelfIfFailWithoutException.md).

### Ejemplo 2: Contador de fallos por código de error

```csharp
public MlResult<Reserva> Reservar(PeticionReserva peticion)
    => ValidarPeticion(peticion)
        .Bind(p => ComprobarDisponibilidad(p))
        .Bind(p => ConfirmarReserva(p))

        .ExecSelfIfFail(errores =>
        {
            // Una métrica por cada error acumulado, no solo por el primero.
            foreach (var mensaje in errores.ToErrorsMessages())
                _metricas.Incrementar("reservas.fallidas", ("motivo", Clasificar(mensaje)));
        });

private static string Clasificar(string mensaje) => mensaje switch
{
    var m when m.Contains("disponib", StringComparison.OrdinalIgnoreCase) => "sin_disponibilidad",
    var m when m.Contains("fecha",    StringComparison.OrdinalIgnoreCase) => "fecha_invalida",
    var m when m.Contains("pago",     StringComparison.OrdinalIgnoreCase) => "pago_rechazado",
    _                                                                     => "otro"
};
```

### Ejemplo 3: Registrar el fallo en una tabla de auditoría (efecto que puede lanzar)

```csharp
public Task<MlResult<Transferencia>> TransferirAsync(OrdenTransferencia orden)
    => ValidarOrdenAsync(orden)
        .BindAsync(o => ComprobarSaldoAsync(o))
        .BindAsync(o => EjecutarAsync(o))

        // Escribir en BD puede fallar; queremos enterarnos si la auditoría no se guarda.
        .TryExecSelfIfFailAsync(
            async errores => await _auditoria.RegistrarFalloAsync(new RegistroFallo
            {
                Operacion = "Transferencia",
                Origen    = orden.CuentaOrigen,
                Destino   = orden.CuentaDestino,
                Importe   = orden.Importe,
                Motivo    = errores.ToErrorsDescription(),
                Momento   = DateTime.UtcNow
            }),
            ex => $"La transferencia falló y además no se pudo auditar el fallo: {ex.Message}");
```

> ⚠️ Cuidado: si el efecto lanza, `TryExecSelfIfFailAsync` **añade** la excepción a los errores
> existentes. El resultado sigue fallido, pero ahora con más información.

### Ejemplo 4: Reintento manual sin recuperar el resultado

```csharp
public async Task<MlResult<Cotizacion>> ObtenerCotizacionAsync(string simbolo)
{
    var intentos = 0;

    return await ConsultarProveedorAsync(simbolo)
        .ExecSelfIfFailAsync(errores =>
        {
            intentos++;
            _log.LogInformation("Intento {N} fallido para {Simbolo}: {E}",
                                intentos, simbolo, errores.ToErrorsDescription());
        })

        // Aquí sí recuperamos: BindIfFail devuelve una nueva tubería.
        .BindIfFailAsync(_ => ConsultarProveedorAlternativoAsync(simbolo));
}
```

Se ve con claridad la división de responsabilidades: **`ExecSelfIfFail` observa`**,
**`BindIfFail` recupera**.

---

## Mejores Prácticas

### 1. `ExecSelfIfFail` observa, no recupera

```csharp
// ❌ No hace lo que parece: el resultado sigue fallido.
var config = Cargar().ExecSelfIfFail(_ => Configuracion.PorDefecto);

// ✅
var config = Cargar().MapIfFail(_ => Configuracion.PorDefecto);
```

### 2. Colócalo justo después del eslabón que puede fallar

Cuanto más cerca esté del origen del fallo, más contexto podrás registrar. Un único
`ExecSelfIfFail` al final del método solo sabe *que* algo falló, no *dónde*.

### 3. Nunca uses miembros inventados de `MlErrorsDetails`

`ToErrorsDescription()` para el texto completo, `ToErrorsMessages()` para la colección,
`Errors.First().Message` para el primero. Nada de `FirstErrorMessage` ni `AllErrors`.

### 4. Usa las variantes especializadas cuando aporten claridad

Si tu delegado empieza con un `if` sobre el contenido de `Details`, probablemente exista ya la
variante concreta: `ExecSelfIfFailWithException`, `ExecSelfIfFailWithoutException` o
`ExecSelfIfFailWithValue`.

### 5. `Try*` solo si el fallo del efecto importa

Para un log en memoria, `ExecSelfIfFail` basta. Para escribir en BD, enviar una alerta crítica o
publicar en una cola, `TryExecSelfFail` / `TryExecSelfIfFailAsync`.

---

## Comparación con las demás variantes

| Método | Se ejecuta si… | El delegado recibe | ¿Cambia el resultado? |
| --- | --- | --- | :---: |
| `ExecSelfIfValid` | Es válido | `T` | No |
| **`ExecSelfIfFail`** | **Es fallido** | **`MlErrorsDetails`** | **No** |
| `ExecSelfIfFailWithValue` | Fallido y hay valor en `Details["Value"]` | `MlErrorsDetails`, `TValue` | No |
| `ExecSelfIfFailWithException` | Fallido y hay excepción en `Details["Ex"]` | `MlErrorsDetails`, `Exception` | No |
| `ExecSelfIfFailWithoutException` | Fallido y **sin** excepción | `MlErrorsDetails` | No |
| `TryExecSelfFail` | Es fallido | `MlErrorsDetails` | Solo si el delegado lanza |
| `MapIfFail` | Es fallido | `MlErrorsDetails` | **Sí**: devuelve un valor |
| `BindIfFail` | Es fallido | `MlErrorsDetails` | **Sí**: devuelve otro `MlResult<T>` |

---

## Resumen

- `ExecSelfIfFail` ejecuta una acción **solo cuando el resultado es fallido** y devuelve el mismo
  `MlResult<T>`, con los mismos errores.
- **No recupera** el resultado: para eso están `MapIfFail` y `BindIfFail`.
- Los errores se consultan con `ToErrorsMessages()`, `ToErrorsDescription()`,
  `Errors.First().Message`, `GetDetailException()` y `GetDetailValue<T>()`.
- ⚠️ El método síncrono con captura de excepciones se llama **`TryExecSelfFail`**, no
  `TryExecSelfIfFail`. En asíncrono existen ambos nombres.
- Es el sitio idiomático para *logging*, métricas, alertas y auditoría de fallos.

## Ver también

- [`1_ExecSelf.md`](./1_ExecSelf.md) — visión general de la familia.
- [`2_ExecSelfIfValid.md`](./2_ExecSelfIfValid.md) — el simétrico para la rama válida.
- [`4_ExecSelfIfFailWithValue.md`](./4_ExecSelfIfFailWithValue.md) — recuperar el valor adjunto al fallo.
- [`5_ExecSelfIfFailWithException.md`](./5_ExecSelfIfFailWithException.md) — solo fallos con excepción.
- [`6_ExecSelfIfFailWithoutException.md`](./6_ExecSelfIfFailWithoutException.md) — solo fallos de negocio.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia y recuento de sobrecargas.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — sistema de errores completo.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — lectura y fusión de `Details`.
- [`../Map/4_MapIfFail.md`](../Map/4_MapIfFail.md) y [`../Bind/6_BindIfFail.md`](../Bind/6_BindIfFail.md) — cuando sí quieres recuperar.