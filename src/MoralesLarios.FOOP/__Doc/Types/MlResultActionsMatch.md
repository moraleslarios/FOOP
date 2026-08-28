# `MlResultActionsMatch` (`Types/MlResultActionsMatch.cs`)

`Match` es la **salida** de la tubería funcional: el único punto donde se abandona el mundo
`MlResult<T>` y se obtiene un valor concreto, tratando de forma explícita **ambas ramas**
(válido y fallido). Por eso nunca hace falta acceder a `.Value` ni comprobar `IsValid` a mano.

---

## Semántica

| Estado de `source` | Función ejecutada |
| --- | --- |
| `IsValid == true` | `valid(value)` — recibe el valor válido. |
| `IsFail == true` | `fail(errorsDetails)` — recibe el `MlErrorsDetails` completo. |

Es **exhaustivo por construcción**: el compilador te obliga a proporcionar las dos funciones, así
que es imposible olvidar el tratamiento del error.

---

## Dos grupos de sobrecargas

El fichero fuente contiene dos regiones con comportamientos distintos. Es importante distinguirlas:

### Región `Match` — dos funciones, una por rama

Devuelve **directamente `TReturn`** (no lo envuelve en `MlResult`). Es la forma habitual de cerrar
una tubería y devolver, por ejemplo, un `IActionResult`, un `string` o un `bool`.

```csharp
TReturn Match<T, TReturn>(this MlResult<T> source,
                          Func<T, TReturn>               valid,
                          Func<MlErrorsDetails, TReturn> fail);
```

### Región `MatchAll` — una sola función, sin parámetros

Las sobrecargas de esta región reciben un `Func<TReturn> funcAll` **sin parámetros** y se ejecutan
igual sea el estado del origen; el resultado sí se envuelve en `MlResult<TReturn>`.

```csharp
MlResult<TReturn> Match<T, TReturn>(this MlResult<T> source,
                                    Func<TReturn>    funcAll);
```

> ⚠️ **Aunque la región del fuente se llama `MatchAll`, los métodos se siguen llamando `Match` /
> `MatchAsync` / `TryMatch` / `TryMatchAsync`.** No existe ningún método público llamado `MatchAll`.
> La resolución entre ambos grupos la hace el compilador según el número de delegados que pases:
> **dos delegados → región `Match`; un delegado sin parámetros → región `MatchAll`**.

Si lo que quieres es "ejecutar algo siempre", suele ser más legible usar
[`MapAlways` / `BindAlways`](./MlResultActionsMap.md), que expresan la intención en el propio nombre.

---

## Métodos de la clase

| Método | Sobrecargas | Devuelve | Descripción |
| --- | --- | --- | --- |
| `Match` (2 funciones) | 1 | `TReturn` | Resolución síncrona de ambas ramas. |
| `MatchAsync` (2 funciones) | 8 | `Task<TReturn>` | 4 con origen `MlResult<T>` y 4 con origen `Task<MlResult<T>>`; delegados síncronos y/o asíncronos. |
| `TryMatch` (2 funciones) | 2 | `MlResult<TResult>` | Como `Match`, capturando excepciones lanzadas por los delegados. |
| `TryMatchAsync` (2 funciones) | 8 | `Task<MlResult<TResult>>` | Versiones asíncronas de `TryMatch`. |
| `Match` (1 función) | 1 | `MlResult<TReturn>` | Ejecuta `funcAll()` sea válido o fallido. |
| `MatchAsync` (1 función) | 4 | `Task<MlResult<TReturn>>` | Versiones asíncronas. |
| `TryMatch` (1 función) | 2 | `MlResult<TReturn>` | Idem con captura de excepciones. |
| `TryMatchAsync` (1 función) | 2 | `Task<MlResult<TReturn>>` | Versiones asíncronas. |

Nombres de los parámetros (útiles para invocación con argumentos nombrados, muy recomendable aquí):

| Variante | Parámetros |
| --- | --- |
| Síncrona | `valid`, `fail` |
| Asíncrona | `validAsync`, `failAsync` (o combinaciones `valid` + `failAsync`) |
| `Try*` | añade `errorMessageBuilder` (`Func<Exception, string>`) o `errorMessage` (`string`) |
| Región `MatchAll` | `funcAll`, `funcAllAsync` |

---

## Ejemplos

### `Match`: cerrar la tubería en un controlador

```csharp
[HttpGet("{id:int}")]
public IActionResult ObtenerCliente(int id)
    => _servicio.ObtenerCliente(id)
                .Match(valid: cliente => Ok(cliente),
                       fail : errores => NotFound(errores.ToErrorsMessages()));
```

Usar **argumentos nombrados** (`valid:` / `fail:`) es la práctica recomendada: hace el código
autoexplicativo e inmune a errores por orden de parámetros.

### `Match`: proyectar a un tipo primitivo

```csharp
bool sePudoGuardar = GuardarPedido(pedido)
    .Match(valid: _ => true,
           fail : _ => false);

string mensaje = GuardarPedido(pedido)
    .Match(valid: id      => $"Pedido {id} creado correctamente.",
           fail : errores => $"No se pudo crear el pedido: {errores.ToErrorsDescription()}");
```

### `MatchAsync`: origen y delegados asíncronos

```csharp
public async Task<IActionResult> ActualizarAsync(int id, ClienteDto dto)
    => await _servicio.ActualizarAsync(id, dto)          // Task<MlResult<Cliente>>
                      .MatchAsync(validAsync: async c       => Ok(await _mapper.MapAsync(c)),
                                  failAsync : async errores => { await _log.RegistrarAsync(errores);
                                                                 return BadRequest(errores.ToErrorsMessages()); });
```

También puedes mezclar: una rama síncrona y otra asíncrona.

```csharp
IActionResult respuesta = await ObtenerClienteAsync(id)
    .MatchAsync(valid    : c       => Ok(c),                       // síncrona
                failAsync: async e => { await _log.RegistrarAsync(e);
                                        return StatusCode(500); });  // asíncrona
```

### `TryMatch`: cuando los propios delegados pueden lanzar

Si la construcción de la respuesta puede fallar (serialización, mapeo por reflexión, formateo de
cultura…), `TryMatch` envuelve el resultado en un `MlResult` en lugar de propagar la excepción:

```csharp
MlResult<string> json = ObtenerCliente(id)
    .TryMatch(valid              : c       => JsonSerializer.Serialize(c),
              fail               : errores => JsonSerializer.Serialize(new { errores = errores.ToErrorsMessages() }),
              errorMessageBuilder: ex      => $"Error al serializar la respuesta del cliente {id}: {ex.Message}");

// json.IsFail == true si alguno de los dos serializadores lanzó excepción.
```

### Región `MatchAll`: mismo resultado en cualquier caso

```csharp
// Se ejecuta igual si hubo éxito o error: útil para marcar un instante o cerrar un tramo.
MlResult<DateTime> instanteCierre = ProcesarLote(lote)
    .Match(funcAll: () => DateTime.UtcNow);

MlResult<string> estado = await ProcesarLoteAsync(lote)
    .MatchAsync(funcAllAsync: async () => await _estado.LeerAsync(lote.Id));
```

---

## Antipatrones que `Match` elimina

```csharp
// ❌ Acceso directo al valor: Value es internal protected y además rompe el modelo.
var cliente = resultado.Value;

// ❌ Ramificación manual: verbosa, fácil de olvidar el else.
if (resultado.IsValid) { /* ... */ } else { /* ... */ }

// ✅ Match: exhaustivo, conciso y sin acceso al estado interno.
var respuesta = resultado.Match(valid: c => Ok(c), fail: e => BadRequest(e.ToErrorsMessages()));
```

---

## Cuándo usar cada operación de salida

| Necesidad | Operación |
| --- | --- |
| Salir de la tubería con un valor de otro tipo | `Match` / `MatchAsync` |
| Salir de la tubería y el delegado puede lanzar | `TryMatch` / `TryMatchAsync` |
| Seguir dentro de `MlResult` transformando el valor válido | [`Map`](./MlResultActionsMap.md) |
| Seguir dentro de `MlResult` con una operación que puede fallar | [`Bind`](./MlResultActionsBind.md) |
| Solo observar (log, métrica) sin cambiar nada | [`ExecSelf`](./MlResultActionsExecSelf.md) |
| Obtener el valor con garantías dentro de la tubería | `SecureValidValue` ([`MlResultActions`](./MlResultActions.md)) |

---

## Documentación detallada por concepto

- [1. `Match`](../Match/1_Match.md)
- [2. `MatchAll`](../Match/2_MatchAll.md) — sobrecargas de un único delegado `funcAll`.

## Ver también

- [`MlResult<T>`](./MlResult.md) — el tipo sobre el que opera `Match`.
- [`MlErrorsDetails`](./MlResultErrors.md) — lo que recibe la rama `fail`.
