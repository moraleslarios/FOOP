# Bind — Encadenar operaciones que pueden fallar

## Índice
1. [Introducción](#introducción)
2. [`Bind` frente a `Map`](#bind-frente-a-map)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [`TryBind` — cuando la operación puede lanzar](#trybind--cuando-la-operación-puede-lanzar)
6. [`BindMulti` — varias validaciones a la vez](#bindmulti--varias-validaciones-a-la-vez)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Comparación con el resto de la familia](#comparación-con-el-resto-de-la-familia)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

`Bind` es **la operación fundamental** de toda la librería. Encadena funciones que devuelven
`MlResult<TReturn>` propagando el fallo automáticamente: si el resultado de entrada es fallido, la
función **no se ejecuta** y los errores viajan intactos hasta el final de la tubería.

Es lo que convierte esto:

```csharp
// ❌ Programación defensiva: el flujo feliz está enterrado entre comprobaciones.
public Factura Emitir(int pedidoId)
{
    var pedido = _repo.ObtenerPedido(pedidoId);
    if (pedido is null) throw new NotFoundException($"Pedido {pedidoId} no existe");

    var cliente = _repo.ObtenerCliente(pedido.ClienteId);
    if (cliente is null) throw new NotFoundException("Cliente no existe");

    if (!cliente.Activo) throw new BusinessException("Cliente inactivo");

    var tarifa = _tarifas.Obtener(cliente.Categoria);
    if (tarifa is null) throw new ConfigurationException("Sin tarifa");

    return _facturador.Emitir(pedido, cliente, tarifa);
}
```

en esto:

```csharp
// ✅ El flujo feliz se lee de arriba abajo. Los fallos se propagan solos.
public MlResult<Factura> Emitir(int pedidoId)
    => ObtenerPedido(pedidoId)
        .Bind(pedido  => ObtenerCliente(pedido.ClienteId)
        .Bind(cliente => ComprobarActivo(cliente)
        .Bind(activo  => ObtenerTarifa(activo.Categoria)
        .Bind(tarifa  => EmitirFactura(pedido, activo, tarifa)))));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## `Bind` frente a `Map`

Esta es **la decisión que más veces vas a tomar** usando la librería, y la regla es sencilla:

| Tu función devuelve… | Usa | Motivo |
| --- | --- | --- |
| `MlResult<TReturn>` (puede fallar) | **`Bind`** | Evita el anidamiento `MlResult<MlResult<T>>` |
| `TReturn` (no puede fallar) | **`Map`** | No hay nada que aplanar |
| `Task<MlResult<TReturn>>` | **`BindAsync`** | Igual que `Bind`, pero asíncrono |
| `Task<TReturn>` | **`MapAsync`** | Igual que `Map`, pero asíncrono |

```csharp
// La función puede fallar (el cliente puede no existir) → Bind
MlResult<Cliente> ObtenerCliente(int id);
resultado.Bind(id => ObtenerCliente(id));       // MlResult<Cliente>  ✅

// La función NO puede fallar (formatear un nombre siempre funciona) → Map
string NombreCompleto(Cliente c) => $"{c.Nombre} {c.Apellidos}";
resultado.Map(c => NombreCompleto(c));          // MlResult<string>   ✅
```

Si te equivocas y usas `Map` con una función que devuelve `MlResult`, el compilador te lo dirá: te
quedará un `MlResult<MlResult<Cliente>>` inutilizable. Es el síntoma inequívoco de que necesitabas
`Bind`.

---

## Firmas reales

```csharp
// La operación completa cabe en una línea. Ahí está toda la magia.
public static MlResult<TReturn> Bind<T, TReturn>(this MlResult<T>            source,
                                                 Func<T, MlResult<TReturn>>  func)
    => source.Match(valid: func,
                    fail : errorsDetails => errorsDetails);
```

Léelo con calma: **`Bind` es `Match` con la rama de fallo ya escrita por ti**. Si es válido, aplica la
función; si no, convierte los errores en el nuevo `MlResult<TReturn>` mediante la conversión implícita
de `MlErrorsDetails`.

**Comportamiento**:

| Estado de `source` | ¿Se ejecuta `func`? | Resultado |
| --- | :---: | --- |
| Válido | Sí | Lo que devuelva `func` (válido **o fallido**) |
| Fallido | **No** | El mismo fallo, con el tipo cambiado a `TReturn` |

📌 La clave: `Bind` puede **introducir** un fallo (si `func` devuelve uno) pero nunca **recuperar** uno.
Para recuperar están [`BindIfFail`](./6_BindIfFail.md) y [`MapIfFail`](../Map/4_MapIfFail.md).

---

## Variantes asíncronas

`BindAsync` tiene **4 sobrecargas** que cubren todas las combinaciones de fuente y delegado:

| Fuente | Delegado | ¿Existe? |
| --- | --- | :---: |
| `MlResult<T>` | `Func<T, MlResult<TReturn>>` | `Bind` (síncrono) |
| `MlResult<T>` | `Func<T, Task<MlResult<TReturn>>>` | `BindAsync` |
| `Task<MlResult<T>>` | `Func<T, MlResult<TReturn>>` | `BindAsync` |
| `Task<MlResult<T>>` | `Func<T, Task<MlResult<TReturn>>>` | `BindAsync` |

Gracias a esto **nunca necesitas `await` intermedios`: la tubería asíncrona se lee igual que la
síncrona.

```csharp
// Un solo await, al principio de la expresión. Nada de await anidados.
public async Task<MlResult<Factura>> EmitirAsync(int pedidoId)
    => await ObtenerPedidoAsync(pedidoId)                    // Task<MlResult<Pedido>>
            .BindAsync(p => ObtenerClienteAsync(p.ClienteId)) // delegado asíncrono
            .BindAsync(c => ComprobarActivo(c))               // delegado SÍNCRONO, mezclable
            .BindAsync(c => ObtenerTarifaAsync(c.Categoria))
            .BindAsync(t => EmitirFacturaAsync(pedidoId, t));
```

> 💡 Regla práctica: en cuanto un solo paso de la tubería sea asíncrono, usa `BindAsync` en **todos**
> los pasos siguientes. Las sobrecargas se encargan del resto.

---

## `TryBind` — cuando la operación puede lanzar

`Bind` asume que tu función devuelve un `MlResult` y no lanza. Cuando llamas a código que **sí puede
lanzar** (ADO.NET, `HttpClient`, deserialización, E/S), usa `TryBind`: captura la excepción, la
convierte en fallo y la guarda en `Details["Ex"]`.

```csharp
// Con constructor de mensaje: control total del texto.
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>           source,
                                                    Func<T, MlResult<TReturn>> func,
                                                    Func<Exception, string>    errorMessageBuilder)

// Con mensaje adicional (opcional): se compone con el mensaje por defecto.
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T>           source,
                                                    Func<T, MlResult<TReturn>> func,
                                                    string exceptionAditionalMessage = null!)
```

```csharp
var resultado = ObtenerConsulta(criterios)
    .TryBind(sql => _dapper.Query<Cliente>(sql).ToMlResult(),
             ex  => $"Error consultando clientes: {ex.Message}");

// Después puedes distinguir el fallo técnico del de negocio:
resultado.ExecSelfIfFailWithException((errores, ex) => _log.LogError(ex, "Fallo de BD"));
```

Sobrecargas disponibles: `TryBind` (2), `TryBindAsync` (8).

| Método | ¿Captura excepciones? | Excepción en `Details["Ex"]` |
| --- | :---: | :---: |
| `Bind` / `BindAsync` | No (la excepción sube) | — |
| `TryBind` / `TryBindAsync` | Sí | Sí |

---

## `BindMulti` — varias validaciones a la vez

`BindMulti` **no bifurca**: ejecuta **todas** las funciones que le pasas sobre el mismo valor y, si
alguna falla, fusiona **todos** los errores en un único resultado fallido. Solo si todas van bien
ejecuta la función final.

Sobrecargas: `BindMulti` (3), `BindMultiAsync` (18).

```csharp
var resultado = ObtenerSolicitud(dto)
    .BindMulti(
        solicitud => RegistrarAsync(solicitud),      // returnFunc: solo si todo lo demás va bien
        s => ValidarTitular(s),                      // ↓ se ejecutan TODAS
        s => ValidarDomicilio(s),
        s => ValidarSolvencia(s));

// Si fallan titular y solvencia, el resultado contiene AMBOS mensajes de error.
```

Es la herramienta de **acumulación de errores** de la familia `Bind`: perfecta para validar
formularios, donde quieres decirle al usuario todo lo que está mal de una sola vez, no solo el primer
problema.

> 💡 Contrasta con `Bind` encadenado, que **corta en el primer fallo**. Y si lo que necesitas es
> filtrar (seguir o fallar según una condición), lo que buscas es [`BindIf`](./5_BindIf.md) o
> `MapEnsure`. Detalle completo en [`4_BindMulti.md`](./4_BindMulti.md).

---

## Ejemplos Prácticos

### Ejemplo 1: Alta de pedido completa

```csharp
public class ServicioPedidos
{
    private readonly IRepositorioPedidos  _pedidos;
    private readonly IRepositorioClientes _clientes;
    private readonly IAlmacen             _almacen;
    private readonly ILogger<ServicioPedidos> _log;

    public async Task<MlResult<Pedido>> CrearAsync(PedidoDto dto)
        => await ValidarDto(dto).ToAsync()
                .BindAsync(d => _clientes.ObtenerAsync(d.ClienteId))
                .BindAsync(c => ComprobarCredito(c, dto.Total))
                .BindAsync(c => ReservarStockAsync(dto.Lineas).Map(_ => c))
                .BindAsync(c => _pedidos.GuardarAsync(Pedido.Nuevo(c, dto.Lineas)))
                .ExecSelfIfValidAsync(p => _log.LogInformation("Pedido {Id} creado", p.Id))
                .ExecSelfIfFailAsync(e =>
                {
                    _log.LogWarning("Alta rechazada: {E}", e.ToErrorsDescription());
                    return Task.CompletedTask;
                });

    private static MlResult<PedidoDto> ValidarDto(PedidoDto dto)
        => EnsureFp.That(dto, d => d.Lineas.Any(),   "El pedido no tiene líneas")
            .Bind(d => EnsureFp.That(d, x => x.Total > 0, "El total debe ser positivo"));

    private MlResult<Cliente> ComprobarCredito(Cliente cliente, decimal total)
        => cliente.CreditoDisponible >= total
            ? cliente
            : $"Crédito insuficiente: disponible {cliente.CreditoDisponible:C}, necesario {total:C}";

    private Task<MlResult<Reserva>> ReservarStockAsync(IEnumerable<LineaPedido> lineas)
        => _almacen.ReservarAsync(lineas);
}
```

Fíjate en dos detalles idiomáticos:

- `return cliente;` y `return "mensaje de error";` funcionan gracias a las **conversiones implícitas**
  de `MlResult<T>`. No hace falta escribir `MlResult<Cliente>.Valid(...)` ni `.Fail(...)`.
- `.Map(_ => c)` descarta el resultado de la reserva y continúa con el cliente: un truco muy útil
  cuando un paso solo importa por su efecto.

### Ejemplo 2: Deserializar y validar entrada externa con `TryBind`

```csharp
public MlResult<ConfiguracionApp> CargarConfiguracion(string rutaJson)
    => EnsureFp.NotNullEmptyOrWhitespace(rutaJson, "La ruta del fichero es obligatoria")

        // Leer del disco puede lanzar: TryBind
        .TryBind(ruta => File.ReadAllText(ruta).ToMlResult(),
                 ex   => $"No se pudo leer '{rutaJson}': {ex.Message}")

        // Deserializar puede lanzar: TryBind
        .TryBind(json => JsonSerializer.Deserialize<ConfiguracionApp>(json)!.ToMlResult(),
                 ex   => $"El fichero de configuración no es un JSON válido: {ex.Message}")

        // Validar NO lanza: Bind normal
        .Bind(cfg => EnsureFp.NotNullEmptyOrWhitespace(cfg.CadenaConexion,
                                                       "Falta 'CadenaConexion'").Map(_ => cfg))
        .Bind(cfg => EnsureFp.That(cfg, c => c.TimeoutSegundos is > 0 and <= 300,
                                   "'TimeoutSegundos' debe estar entre 1 y 300"));
```

La distinción es deliberada: **`TryBind` para lo que puede explotar, `Bind` para lo que solo puede
rechazar.** Así el fallo técnico llega con excepción en `Details["Ex"]` y el de negocio, sin ella,
lo que te permite tratarlos por separado con
[`ExecSelfIfFailWithException`](../ExecSelf/5_ExecSelfIfFailWithException.md) y
[`ExecSelfIfFailWithoutException`](../ExecSelf/6_ExecSelfIfFailWithoutException.md).

### Ejemplo 3: Transferencia bancaria con propagación de contexto

```csharp
public async Task<MlResult<Movimiento>> TransferirAsync(int origen, int destino, decimal importe)
    => await EnsureFp.ThatAsync(importe, i => i > 0, "El importe debe ser positivo")
            .BindAsync(_ => ObtenerCuentaAsync(origen))
            .AddMlErrorDetailIfFailAsync($"[Transferencia] Cuenta de origen {origen}")

            .BindAsync(o => ComprobarSaldo(o, importe))
            .BindAsync(o => ObtenerCuentaAsync(destino).Map(d => (Origen: o, Destino: d)))
            .AddMlErrorDetailIfFailAsync($"[Transferencia] Cuenta de destino {destino}")

            .BindAsync(par => EjecutarAsync(par.Origen, par.Destino, importe))
            .AddMlErrorDetailIfFailAsync($"[Transferencia] {importe:C} de {origen} a {destino}");
```

`.Map(d => (Origen: o, Destino: d))` es el patrón para **arrastrar dos valores** por la tubería sin
variables intermedias. Para más de dos, considera
[`Combine`](../Several/4_Combine.md) o `TryBindBuildTuple`.

### Ejemplo 4: Salir de la tubería en un controlador

```csharp
[HttpPost("pedidos")]
public async Task<IActionResult> Crear(PedidoDto dto)
    => await _servicio.CrearAsync(dto)
            .MatchAsync(
                valid: p       => Task.FromResult<IActionResult>(
                                      CreatedAtAction(nameof(Obtener), new { id = p.Id }, p)),
                fail:  errores => Task.FromResult<IActionResult>(
                                      errores.GetDetailException().Match(
                                          valid: _ => StatusCode(500, "Error interno"),
                                          fail:  _ => BadRequest(errores.ToErrorsMessages()))));
```

Fallo con excepción → **500** (avería nuestra). Fallo sin excepción → **400** (el cliente puede
corregirlo). Una regla, dos líneas, cero `if`.

---

## Mejores Prácticas

### 1. Una función, un paso

Cada delegado de `Bind` debería hacer **una sola cosa** y tener nombre propio. Si el lambda ocupa más
de tres líneas, extráelo a un método privado: la tubería se vuelve casi prosa.

### 2. `TryBind` solo donde puede lanzar

Usar `TryBind` en todas partes «por si acaso» oculta errores de programación (`NullReferenceException`,
`IndexOutOfRange`) convirtiéndolos en fallos de datos. Resérvalo para la E/S y las librerías externas.

### 3. Nunca accedas al valor directamente

```csharp
// ❌ Ni siquiera compila fuera de la librería: Value es internal protected.
var cliente = resultado.Value;

// ✅
resultado.Match(valid: c => Ok(c), fail: e => NotFound(e.ToErrorsMessages()));
```

### 4. Aprovecha las conversiones implícitas

`return cliente;` y `return "mensaje";` dentro de un método que devuelve `MlResult<Cliente>` son
válidos y mucho más legibles que las llamadas explícitas a `Valid`/`Fail`.

### 5. Añade contexto en las fronteras

Un `AddMlErrorDetailIfFail` por capa produce una traza de causas encadenadas sin coste alguno. Ver
[`2_MlResultActions.md`](./2_MlResultActions.md).

### 6. Nombra siempre los argumentos de `Match`

`Match(valid: ..., fail: ...)` es autoexplicativo; `Match(x => ..., y => ...)` obliga a recordar el
orden.

---

## Comparación con el resto de la familia

| Método | Se ejecuta si… | Recibe | Devuelve | Puede recuperar el fallo |
| --- | --- | --- | --- | :---: |
| **`Bind`** | Válido | `T` | `MlResult<TReturn>` | No |
| `Map` | Válido | `T` | `TReturn` (se envuelve) | No |
| `BindIf` | Válido y cumple condición | `T` | `MlResult<TReturn>` | No |
| `BindMulti` | Válido (ejecuta **todas** las funciones) | `T` | `MlResult<TReturn>` | No |
| [`BindIfFail`](./6_BindIfFail.md) | **Fallido** | `MlErrorsDetails` | `MlResult<T>` | **Sí** |
| [`BindAlways`](./10_BindAlways.md) | Siempre | — o ambas ramas | `MlResult<TReturn>` | Sí |
| `ExecSelf` | Según variante | Según variante | El **mismo** resultado | No |
| `Match` | Siempre | Ambas ramas | `TReturn` **crudo** | Sale de la tubería |

---

## Resumen

- `Bind` encadena funciones que devuelven `MlResult<TReturn>` **propagando el fallo automáticamente**;
  su implementación es literalmente `source.Match(valid: func, fail: errors => errors)`.
- **`Bind` si tu función puede fallar; `Map` si no.** Es la decisión más frecuente de la librería.
- `BindAsync` tiene 4 sobrecargas que permiten mezclar pasos síncronos y asíncronos sin `await`
  intermedios.
- `TryBind` (2) y `TryBindAsync` (8) capturan excepciones y las guardan en `Details["Ex"]`; úsalos solo
  donde el código realmente pueda lanzar.
- `BindMulti` (3) y `BindMultiAsync` (18) ejecutan **todas** las funciones y **acumulan** los errores.
- `Bind` puede introducir un fallo pero **nunca recuperarlo**: para eso están `BindIfFail` y sus
  variantes especializadas.

## Ver también

- [`2_MlResultActions.md`](./2_MlResultActions.md) — enriquecer errores y acceso seguro.
- [`4_BindMulti.md`](./4_BindMulti.md) — acumulación de errores en detalle.
- [`5_BindIf.md`](./5_BindIf.md) — ejecución condicional.
- [`6_BindIfFail.md`](./6_BindIfFail.md) — recuperación de fallos.
- [`10_BindAlways.md`](./10_BindAlways.md) — ejecutar sea cual sea el estado.
- [`../Map/1_Map.md`](../Map/1_Map.md) — la alternativa cuando la función no puede fallar.
- [`../Match/1_Match.md`](../Match/1_Match.md) — cómo salir de la tubería.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — efectos secundarios sin alterar el flujo.
- [`../EnsureFp/EnsureFp.md`](../EnsureFp/EnsureFp.md) — crear el primer `MlResult` de la cadena.
- [`../Types/MlResultActionsBind.md`](../Types/MlResultActionsBind.md) — referencia completa con todas las sobrecargas.
