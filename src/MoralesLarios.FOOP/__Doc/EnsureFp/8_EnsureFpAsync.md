# `EnsureFp` — Variantes asíncronas

> Archivo fuente: `Helpers/EnsureFp.Async.cs` (más los envoltorios históricos de `Helpers/EnsureFp.cs`).

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [Los tres ejes de asincronía](#los-tres-ejes-de-asincronía)
- [1. `ThatAsync` con fuente asíncrona](#1-thatasync-con-fuente-asíncrona)
- [2. `ThatAsync` con predicado asíncrono](#2-thatasync-con-predicado-asíncrono)
- [3. Sobrecarga con `CancellationToken`](#3-sobrecarga-con-cancellationtoken)
- [4. `ThatArgAsync`: mensaje automático](#4-thatargasync-mensaje-automático)
- [5. `TryThatAsync`: predicados asíncronos que pueden lanzar](#5-trythatasync-predicados-asíncronos-que-pueden-lanzar)
- [6. Guardas clásicas con fuente asíncrona](#6-guardas-clásicas-con-fuente-asíncrona)
- [7. Semántica defensiva: `SecureAwait` y `EvaluatePredicateAsync`](#7-semántica-defensiva-secureawait-y-evaluatepredicateasync)
- [8. Los envoltorios históricos de `EnsureFp.cs`](#8-los-envoltorios-históricos-de-ensurefpcs)
- [9. Ejemplos completos](#9-ejemplos-completos)
- [10. Mejores prácticas](#10-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

Las primeras versiones de `EnsureFp` solo tenían sobrecargas `*Async` que envolvían el resultado
síncrono con `.ToAsync()`. Eran cómodas para encadenar, pero **no permitían nada realmente
asíncrono**: ni esperar el valor a validar, ni ejecutar una comprobación contra la base de datos o un
servicio remoto.

Este bloque cubre los casos que la práctica exige:

- validar un valor que **todavía no está disponible** (`Task<T>`);
- validar con un predicado que **necesita esperar** (`Func<T, Task<bool>>`);
- propagar un `CancellationToken` a la comprobación;
- capturar las excepciones del predicado asíncrono (`TryThatAsync`);
- generar el mensaje automáticamente también en asíncrono (`*ArgAsync`).

---

## Los tres ejes de asincronía

| Eje | Firma característica | Cuándo se usa |
|---|---|---|
| **Fuente asíncrona** | `ThatAsync<T>(Task<T> valueAsync, …)` | el valor viene de una consulta o de una llamada HTTP |
| **Predicado asíncrono** | `ThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync, …)` | la comprobación necesita ir a la base de datos o a otro servicio |
| **Ambos** | `ThatAsync<T>(Task<T>, Func<T, Task<bool>>, …)` | el valor y la comprobación son asíncronos |

Todas las sobrecargas devuelven `Task<MlResult<T>>` y se combinan con `BindAsync`, `MapAsync` y
`MatchAsync`.

---

## 1. `ThatAsync` con fuente asíncrona

```csharp
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, bool> predicate, string errorMessage);
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, bool> predicate, MlErrorsDetails errorsDetails);
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, bool condition, string errorMessage);
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, bool condition, MlErrorsDetails errorsDetails);
```

Evita el `await` intermedio y la variable temporal:

```csharp
// ❌ Antes
var cliente = await repositorio.GetAsync(id);
var r = That(cliente, c => c.Activo, "El cliente no está activo.");

// ✅ Ahora
var r = await ThatAsync(repositorio.GetAsync(id),
                        c => c.Activo,
                        "El cliente no está activo.");
```

La sobrecarga con `bool condition` sirve cuando la condición ya está evaluada y solo el valor es
asíncrono:

```csharp
var r = await ThatAsync(repositorio.GetAsync(id),
                        usuarioActual.EsAdministrador,
                        "Se requieren permisos de administrador.");
```

> Recuerda que con `bool condition` la condición **se evalúa antes** de la llamada, incluso si el
> valor acabara siendo inválido. Si la evaluación es costosa, usa la sobrecarga con predicado.

---

## 2. `ThatAsync` con predicado asíncrono

```csharp
public static Task<MlResult<T>> ThatAsync<T>(T value,       Func<T, Task<bool>> predicateAsync, string errorMessage);
public static Task<MlResult<T>> ThatAsync<T>(T value,       Func<T, Task<bool>> predicateAsync, MlErrorsDetails errorsDetails);
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, Task<bool>> predicateAsync, string errorMessage);
public static Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, Task<bool>> predicateAsync, MlErrorsDetails errorsDetails);
```

Este es el caso que antes no existía: **la regla de negocio necesita consultar el almacén**.

```csharp
// Unicidad: el email no puede estar registrado
var r1 = await ThatAsync(dto.Email,
                         async e => ! await repositorio.ExisteEmailAsync(e),
                         "Ese correo electrónico ya está registrado.");

// Stock disponible en el momento de confirmar
var r2 = await ThatAsync(linea,
                         async l => await almacen.HayStockAsync(l.ProductoId, l.Cantidad),
                         "No hay stock suficiente para esa línea.");

// Valor asíncrono y comprobación asíncrona
var r3 = await ThatAsync(repositorio.GetPedidoAsync(id),
                         async p => await politicas.PuedeConfirmarseAsync(p),
                         "El pedido no cumple la política de confirmación.");
```

El resultado sigue siendo un `MlResult<T>`, por lo que se compone igual que cualquier otra regla:

```csharp
public async Task<MlResult<Usuario>> RegistrarAsync(RegistroDto dto) =>
    await NotNullEmptyOrWhitespaceArg(dto.Email)
              .BindAsync(e => ThatAsync(e, async x => ! await repo.ExisteEmailAsync(x),
                                        "Ese correo ya está registrado."))
              .BindAsync(e => IsValidEmailArg(e).ToAsync())
              .MapAsync(e => new Usuario(e));
```

---

## 3. Sobrecarga con `CancellationToken`

```csharp
public static Task<MlResult<T>> ThatAsync<T>(T value,
                                             Func<T, CancellationToken, Task<bool>> predicateAsync,
                                             string errorMessage,
                                             CancellationToken cancellationToken = default);

public static Task<MlResult<T>> ThatAsync<T>(T value,
                                             Func<T, CancellationToken, Task<bool>> predicateAsync,
                                             MlErrorsDetails errorsDetails,
                                             CancellationToken cancellationToken = default);
```

Propaga la cancelación hasta la comprobación, algo imprescindible en ASP.NET Core:

```csharp
[HttpPost]
public async Task<IActionResult> Crear(CrearDto dto, CancellationToken ct) =>
    (await ThatAsync(dto.Codigo,
                     (c, token) => repositorio.EsCodigoLibreAsync(c, token),
                     "Ese código ya está en uso.",
                     ct))
        .Match(valid: c => Ok(c), fail: e => BadRequest(e.ToErrorsMessages()));
```

Si el predicado es `null`, la regla delega en `That(value, false, …)`: falla de forma controlada, sin
excepción.

---

## 4. `ThatArgAsync`: mensaje automático

```csharp
public static Task<MlResult<T>> ThatArgAsync<T>(T value, Func<T, Task<bool>> predicateAsync,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static Task<MlResult<T>> ThatArgAsync<T>(Task<T> valueAsync, Func<T, bool> predicate,
    [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null);
```

Igual que las guardas `*Arg` síncronas: genera el mensaje a partir de la expresión escrita en la
llamada y añade `ParamName` y `Value` a los detalles.

```csharp
var r1 = await ThatArgAsync(dto.Codigo, c => repositorio.EsCodigoLibreAsync(c));
// "'dto.Codigo' no cumple la condición requerida."

var r2 = await ThatArgAsync(repositorio.GetSaldoAsync(id), s => s >= 0m);
// El paramName se toma de la expresión de valueAsync.
```

Fíjate en el segundo: el `[CallerArgumentExpression]` apunta a `valueAsync`, de modo que el mensaje
identifica la **fuente** del valor.

---

## 5. `TryThatAsync`: predicados asíncronos que pueden lanzar

```csharp
public static Task<MlResult<T>> TryThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync,
                                                string errorMessage,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static Task<MlResult<T>> TryThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync,
                                                MlErrorsDetails errorsDetails,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static Task<MlResult<T>> TryThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync,
                                                Func<Exception, string> errorMessageBuilder,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Una comprobación asíncrona toca infraestructura, y la infraestructura falla: tiempos de espera
agotados, conexiones caídas, respuestas 500. `TryThatAsync` captura la excepción, la convierte en un
fallo del carril y la guarda en los detalles bajo la clave `Ex` (`EX_DESC_KEY`).

```csharp
var r = await TryThatAsync(dto.Nif,
                           nif => servicioExterno.ValidarNifAsync(nif),
                           ex => $"No se pudo validar el NIF contra el servicio externo: {ex.Message}");

if (r.IsFail)
{
    var detalles = r.SecureFailErrorsDetails();
    var excepcion = detalles.GetDetailException();   // la excepción original, para el log
    logger.LogError(excepcion, "Fallo validando NIF");
}
```

La tercera sobrecarga, con `Func<Exception, string>`, permite construir el mensaje a partir de la
excepción concreta y es la más útil para diagnóstico. Ver también
[`TryThat`](./1_EnsureFpCore.md#3-trythat-predicados-que-pueden-lanzar) para la versión síncrona.

---

## 6. Guardas clásicas con fuente asíncrona

```csharp
public static Task<MlResult<T>> NotNullAsync<T>(Task<T> valueAsync, string errorMessage);
public static Task<MlResult<T>> NotNullAsync<T>(Task<T> valueAsync, MlErrorsDetails errorsDetails);
public static Task<MlResult<T>> NotNullArgAsync<T>(Task<T> valueAsync,
    [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null);

public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(Task<IEnumerable<T>> valueAsync, string errorMessage);
public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(Task<IEnumerable<T>> valueAsync, MlErrorsDetails errorsDetails);
public static Task<MlResult<IEnumerable<T>>> NotEmptyArgAsync<T>(Task<IEnumerable<T>> valueAsync,
    [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null);

public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(Task<string> valueAsync, string errorMessage);
public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(Task<string> valueAsync, MlErrorsDetails errorsDetails);
public static Task<MlResult<string>> NotNullEmptyOrWhitespaceArgAsync(Task<string> valueAsync,
    [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null);

public static Task<MlResult<T>> NotNullValueAsync<T>(Task<T?> valueAsync, string errorMessage) where T : struct;
```

Son el patrón idiomático para las consultas de Entity Framework:

```csharp
// "No encontrado" pasa a ser un fallo del carril, sin if ni excepciones
public Task<MlResult<Cliente>> ObtenerAsync(int id) =>
    NotNullAsync(contexto.Clientes.FirstOrDefaultAsync(c => c.Id == id)!,
                 $"No existe el cliente {id}.");

public Task<MlResult<IEnumerable<Pedido>>> PendientesAsync(int clienteId) =>
    NotEmptyAsync(ObtenerPedidosAsync(clienteId),
                  "El cliente no tiene pedidos pendientes.");

public Task<MlResult<decimal>> SaldoAsync(Guid clienteId) =>
    NotNullValueAsync(BuscarSaldoAsync(clienteId),
                      "El cliente no tiene saldo registrado.");
```

`NotNullValueAsync` **desenvuelve** el `Nullable<T>`: devuelve `MlResult<decimal>`, no
`MlResult<decimal?>`. Ver [7. Tipos `Nullable<T>`](./7_EnsureFpNullables.md).

---

## 7. Semántica defensiva: `SecureAwait` y `EvaluatePredicateAsync`

Dos helpers privados garantizan que **ninguna sobrecarga asíncrona lance una excepción por causas
estructurales**:

| Helper | Situación | Comportamiento |
|---|---|---|
| `SecureAwait<T>(Task<T> task)` | `task` es `null` | devuelve `default!` en lugar de `NullReferenceException` |
| `EvaluatePredicateAsync<T>` | `predicateAsync` es `null` | devuelve `false` (la regla falla) |
| `EvaluatePredicateAsync<T>` | el predicado devuelve una `Task` `null` | devuelve `false` (la regla falla) |

Tabla resumen del comportamiento frente a entradas degeneradas:

| Entrada | Resultado |
|---|---|
| `Task<T>` es `null` | el valor se trata como `default!` → la guarda de nulidad **falla** |
| Predicado `null` | la regla **falla** con el mensaje indicado |
| Predicado devuelve `Task` `null` | la regla **falla** |
| El predicado lanza una excepción (`ThatAsync`) | **la excepción se propaga** |
| El predicado lanza una excepción (`TryThatAsync`) | se convierte en fallo con la excepción en `Details["Ex"]` |
| `CancellationToken` cancelado | se propaga la `OperationCanceledException` del predicado |

La regla de oro: **`ThatAsync` propaga las excepciones, `TryThatAsync` las convierte en fallos**. Usa
la segunda siempre que la comprobación toque infraestructura.

---

## 8. Los envoltorios históricos de `EnsureFp.cs`

Las sobrecargas asíncronas originales siguen existiendo por compatibilidad:

```csharp
public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage);
public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, MlErrorsDetails errorsDetails);
public static Task<MlResult<T>> NotNullAsync<T>(T value, string errorMessage);
public static Task<MlResult<T>> NotNullAsync<T>(T value, MlErrorsDetails errorsDetails);
public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, string errorMessage);
public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails);
public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, string errorMessage);
public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, MlErrorsDetails errorsDetails);
```

**No son asíncronas de verdad:** evalúan la regla en el hilo actual y envuelven el resultado con
`.ToAsync()`. Su única utilidad es encajar en una cadena asíncrona sin romperla:

```csharp
var r = await NotNullAsync(cliente, "Cliente obligatorio.")
                  .BindAsync(c => repositorio.GuardarAsync(c));
```

Alternativa equivalente y más explícita:

```csharp
var r = await NotNull(cliente, "Cliente obligatorio.")
                  .ToAsync()
                  .BindAsync(c => repositorio.GuardarAsync(c));
```

> Distingue bien las dos familias: `NotNullAsync(cliente, …)` recibe un **valor** (envoltorio);
> `NotNullAsync(tarea, …)` recibe una **`Task<T>`** (asíncrono real). El compilador elige por el tipo
> del primer argumento.

---

## 9. Ejemplos completos

### 9.1. Registro con reglas de unicidad

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public async Task<MlResult<Usuario>> RegistrarAsync(RegistroDto dto, CancellationToken ct)
{
    var sintaxis = All(dto,
        d => NotNullEmptyOrWhitespace(d.Email, "El correo es obligatorio.").Map(_ => d),
        d => IsValidEmail(d.Email, "El correo no tiene un formato válido.").Map(_ => d),
        d => MinLength(d.Password, 8, "La contraseña debe tener al menos 8 caracteres.").Map(_ => d));

    if (sintaxis.IsFail) return sintaxis.SecureFailErrorsDetails().ToMlResultFail<Usuario>();

    return await TryThatAsync(dto,
                              async d => ! await repositorio.ExisteEmailAsync(d.Email, ct),
                              ex => $"No se pudo comprobar la unicidad del correo: {ex.Message}")
                     .MapAsync(d => new Usuario(d.Email, Hash(d.Password)));
}
```

Primero las reglas **baratas y síncronas** (agregadas con `All`, para devolverlas todas de golpe);
después la regla **caraytransaccional** que va a la base de datos. Ese orden ahorra viajes al almacén.

### 9.2. Confirmación de pedido con varias comprobaciones remotas

```csharp
public async Task<MlResult<Pedido>> ConfirmarAsync(int pedidoId, CancellationToken ct) =>
    await NotNullAsync(contexto.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, ct)!,
                       $"No existe el pedido {pedidoId}.")
              .BindAsync(p => ThatAsync(p, x => x.Estado == EstadoPedido.Borrador,
                                        "Solo se pueden confirmar pedidos en borrador."))
              .BindAsync(p => TryThatAsync(p,
                                           async x => await almacen.HayStockAsync(x, ct),
                                           ex => $"El servicio de almacén no respondió: {ex.Message}"))
              .BindAsync(p => TryThatAsync(p,
                                           async x => await creditos.TieneCreditoAsync(x.ClienteId, x.Total, ct),
                                           ex => $"El servicio de crédito no respondió: {ex.Message}"))
              .BindAsync(p => repositorio.ConfirmarAsync(p, ct));
```

### 9.3. Controlador ASP.NET Core

```csharp
[HttpPost("pedidos/{id:int}/confirmar")]
public async Task<IActionResult> Confirmar(int id, CancellationToken ct) =>
    (await ConfirmarAsync(id, ct))
        .Match(
            valid: p => Ok(p),
            fail:  e => e.Details.ContainsKey(EX_DESC_KEY)
                            ? StatusCode(StatusCodes.Status503ServiceUnavailable, e.ToErrorsMessages())
                            : BadRequest(e.ToErrorsMessages()));
```

La presencia de la clave `Ex` en los detalles distingue un fallo de **infraestructura** (503) de un
fallo de **validación** (400).

### 9.4. Agregación asíncrona

```csharp
public Task<MlResult<Pedido>> ValidarRemotoAsync(Pedido p) =>
    AllAsync(p,
        x => ThatAsync(x, async v => await almacen.HayStockAsync(v),      "Sin stock."),
        x => ThatAsync(x, async v => await creditos.TieneCreditoAsync(v), "Sin crédito suficiente."),
        x => ThatAsync(x, async v => await fraude.EsSeguroAsync(v),       "Marcado por control de fraude."));
```

`AllAsync`, `AllOrFirstAsync` y `AnyAsync` están documentadas en
[2. Agregación de reglas](./2_EnsureFpAggregation.md#5-variantes-asíncronas).

---

## 10. Mejores prácticas

1. **Valida primero lo síncrono.** Las reglas de formato y rango no cuestan nada; ejecútalas antes de
   ir a la base de datos.
2. **`TryThatAsync` para todo lo que toque infraestructura.** `ThatAsync` propaga las excepciones;
   solo úsalo con predicados que no puedan fallar.
3. **Propaga el `CancellationToken`** con la sobrecarga
   `Func<T, CancellationToken, Task<bool>>` en aplicaciones web.
4. **No mezcles las dos familias `*Async` por descuido.** Si el primer argumento es un valor, la
   sobrecarga es un simple envoltorio; si es una `Task<T>`, es asíncrona de verdad.
5. **No dispares las reglas asíncronas en paralelo sobre el mismo `DbContext`**: no es seguro para
   varios hilos. `AllAsync` las ejecuta de forma secuencial precisamente por eso.
6. **Devuelve `Task<MlResult<T>>` sin `await` innecesarios** cuando solo reenvías el resultado: ahorra
   una máquina de estados.
7. **Usa `NotNullAsync` / `NotEmptyAsync` / `NotNullValueAsync` con las consultas EF** para convertir
   «no encontrado» en un fallo del carril.
8. **Consulta `Details["Ex"]`** para distinguir fallos técnicos de fallos de validación y elegir el
   código HTTP adecuado.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [7. Tipos `Nullable<T>`](./7_EnsureFpNullables.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
- [`Bind` y sus variantes asíncronas](../Bind/3_Bind.md)
- [`Map` y sus variantes asíncronas](../Map/1_Map.md)
