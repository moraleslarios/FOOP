# ExecSelfIfValid — Efectos secundarios solo en la rama válida

## Índice
1. [Introducción](#introducción)
2. [Firmas reales](#firmas-reales)
3. [Variantes asíncronas](#variantes-asíncronas)
4. [`TryExecSelfIfValid` — cuando el efecto puede lanzar](#tryexecselfifvalid--cuando-el-efecto-puede-lanzar)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con las demás variantes](#comparación-con-las-demás-variantes)
8. [Resumen](#resumen)
9. [Ver también](#ver-también)

---

## Introducción

`ExecSelfIfValid` ejecuta una acción **únicamente si el resultado es válido** y devuelve **el mismo
`MlResult<T>`** que recibió, intacto. Si el resultado es fallido, la acción **no se ejecuta** y el
fallo se propaga sin cambios.

Es la variante de `ExecSelf` que más se usa, porque el caso típico es: «cuando esto salga bien,
además quiero registrar / publicar / cachear algo».

```csharp
MlResult<Usuario> resultado = ObtenerUsuario(id)
    .ExecSelfIfValid(u => _log.LogInformation("Usuario {Id} recuperado", u.Id));

// Si ObtenerUsuario falló, el log NO se escribe y `resultado` sigue fallido.
```

**Lo que aporta frente a un `if`:**

```csharp
// ❌ Comprobación manual: rompe el encadenamiento y accede al valor directamente.
var r = ObtenerUsuario(id);
if (r.IsValid) _log.LogInformation("Usuario recuperado");   // ¿y el valor? Value es internal protected

// ✅ Encadenable y con el valor ya desempaquetado.
var r = ObtenerUsuario(id)
    .ExecSelfIfValid(u => _log.LogInformation("Usuario {Nombre} recuperado", u.Nombre));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception` ni
> `HasValue`. Para consultar los errores usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.
> (`ExecSelfIfValid` no recibe los errores; esta nota aplica a las variantes hermanas
> [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md).)

---

## Firmas reales

```csharp
// Síncrono
public static MlResult<T> ExecSelfIfValid<T>(this MlResult<T> source,
                                             Action<T>        actionValid)

// Con captura de excepciones
public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T>        source,
                                               Action<T>               actionValid,
                                               Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T> source,
                                               Action<T>        actionValid,
                                               string           exceptionAditionalMessage = null!)
```

**Comportamiento**:

| Estado de `source` | ¿Se ejecuta `actionValid`? | Resultado devuelto |
| --- | :---: | --- |
| Válido | Sí | El mismo `MlResult<T>` |
| Fallido | No | El mismo `MlResult<T>` (con sus errores) |
| Válido y `actionValid` lanza (`ExecSelfIfValid`) | Sí | La excepción **sube** |
| Válido y `actionValid` lanza (`TryExecSelfIfValid`) | Sí | `MlResult<T>` **fallido**, con la excepción en `Details["Ex"]` |

---

## Variantes asíncronas

Existen las cuatro combinaciones de fuente y delegado, de modo que nunca hacen falta `await`
intermedios:

| Fuente | Delegado | Método |
| --- | --- | --- |
| `MlResult<T>` | `Action<T>` | `ExecSelfIfValid` |
| `MlResult<T>` | `Func<T, Task>` | `ExecSelfIfValidAsync` |
| `Task<MlResult<T>>` | `Action<T>` | `ExecSelfIfValidAsync` |
| `Task<MlResult<T>>` | `Func<T, Task>` | `ExecSelfIfValidAsync` |

```csharp
var resultado = await ObtenerPedidoAsync(id)
    .ExecSelfIfValidAsync(async p => await _bus.PublicarAsync(new PedidoLeido(p.Id)))
    .BindAsync(p => ValidarAsync(p));
```

Las variantes `TryExecSelfIfValidAsync` cubren esas mismas cuatro combinaciones × las dos formas de
indicar el mensaje de error (`Func<Exception, string>` o `string`).

---

## `TryExecSelfIfValid` — cuando el efecto puede lanzar

Un log en memoria no falla, pero **publicar en una cola, escribir en caché o llamar a un servicio
externo sí**. Ahí es donde entra `TryExecSelfIfValid`:

```csharp
MlResult<Pedido> resultado = GuardarPedido(pedido)
    .TryExecSelfIfValid(
        p  => _colaExterna.Publicar(p),                    // Puede lanzar
        ex => $"El pedido se guardó pero no se pudo publicar en la cola: {ex.Message}");
```

> ⚠️ **Decisión importante**: con `Try*`, si el efecto secundario falla el resultado pasa a ser
> **fallido**, aunque la operación principal hubiera ido bien. Elige según lo que signifique ese
> fallo para tu sistema:
>
> | El efecto secundario es… | Usa |
> | --- | --- |
> | Informativo (log, métrica) y su fallo es irrelevante | `ExecSelfIfValid` |
> | Necesario para la corrección (evento de dominio, invalidar caché) | `TryExecSelfIfValid` |

Si omites `errorMessageBuilder` y `exceptionAditionalMessage`, se usa el mensaje por defecto
`DEFAULT_EX_ERROR_MESSAGE(ex)` de `Helpers/Constants.cs`.

---

## Ejemplos Prácticos

### Ejemplo 1: Cachear el resultado de una consulta

```csharp
public class RepositorioClientes
{
    private readonly ICache _cache;
    private readonly ILogger<RepositorioClientes> _log;

    public Task<MlResult<Cliente>> ObtenerAsync(Guid id)
        => ConsultarBaseDatosAsync(id)

            // Solo si se encontró: lo guardamos en caché y lo trazamos.
            .ExecSelfIfValidAsync(async c =>
            {
                await _cache.GuardarAsync($"cliente:{c.Id}", c, TimeSpan.FromMinutes(10));
                _log.LogDebug("Cliente {Id} cacheado", c.Id);
            })

            // Si no se encontró, esto sí se ejecuta (variante hermana).
            .ExecSelfIfFailAsync(errores =>
                _log.LogInformation("Cliente {Id} no encontrado: {E}",
                                    id, errores.ToErrorsDescription()));
}
```

### Ejemplo 2: Publicar eventos de dominio tras una escritura

```csharp
public Task<MlResult<Pedido>> ConfirmarAsync(Guid pedidoId)
    => ObtenerPedidoAsync(pedidoId)
        .BindAsync(p => ValidarConfirmableAsync(p))
        .BindAsync(p => MarcarConfirmadoAsync(p))

        // El evento es parte del contrato: si falla, queremos saberlo.
        .TryExecSelfIfValidAsync(
            async p => await _bus.PublicarAsync(new PedidoConfirmado(p.Id, p.Importe)),
            ex => $"El pedido se confirmó pero el evento no se publicó: {ex.Message}")

        // La métrica es informativa: su fallo no debe romper nada.
        .ExecSelfIfValidAsync(p => _metricas.Incrementar("pedidos.confirmados"));
```

### Ejemplo 3: Medir la duración de una tubería

```csharp
public async Task<MlResult<Informe>> GenerarInformeAsync(Peticion peticion)
{
    var cronometro = Stopwatch.StartNew();

    return await CargarDatosAsync(peticion)
        .ExecSelfIfValidAsync(d => _log.LogDebug(
            "Datos cargados ({Filas} filas) en {Ms} ms", d.Filas.Count, cronometro.ElapsedMilliseconds))

        .BindAsync(d => CalcularAsync(d))
        .ExecSelfIfValidAsync(_ => _log.LogDebug(
            "Cálculos terminados en {Ms} ms", cronometro.ElapsedMilliseconds))

        .BindAsync(c => RenderizarAsync(c))
        .ExecSelfIfValidAsync(inf => _metricas.Registrar(
            "informe.generado", cronometro.ElapsedMilliseconds));
}
```

Cada `ExecSelfIfValidAsync` marca un **hito** de la tubería. Si alguno de los `BindAsync` falla, los
`ExecSelfIfValidAsync` posteriores simplemente no se ejecutan: no hace falta ni un `if`.

### Ejemplo 4: Encadenar varios efectos independientes

```csharp
var resultado = await RegistrarUsuarioAsync(dto)
    .ExecSelfIfValidAsync(async u => await _email.EnviarBienvenidaAsync(u.Email))
    .ExecSelfIfValidAsync(async u => await _crm.CrearContactoAsync(u))
    .ExecSelfIfValidAsync(     u  => _metricas.Incrementar("usuarios.registrados"));
```

Los tres se ejecutan en orden y **ninguno** modifica el resultado. Si prefieres que el fallo del
email sí cuente como error, cambia ese eslabón por `TryExecSelfIfValidAsync`.

---

## Mejores Prácticas

### 1. Usa `ExecSelfIfValid` en lugar de `ExecSelf` con un `if`

```csharp
// ❌
resultado.ExecSelf(r => { if (r.IsValid) Notificar(); });

// ✅
resultado.ExecSelfIfValid(v => Notificar(v));
```

### 2. Un efecto por llamada

Encadenar varias llamadas cortas se lee mucho mejor que un delegado con cinco responsabilidades, y
permite decidir individualmente cuál necesita `Try*`.

### 3. No transformes dentro del delegado

Si el delegado necesita **devolver** algo, la operación correcta es `Map` (transformar) o `Bind`
(transformar pudiendo fallar), no `ExecSelfIfValid`.

### 4. No accedas a `Value` directamente

`MlResult<T>.Value` es `internal protected` a propósito. `ExecSelfIfValid` es precisamente uno de los
mecanismos previstos para trabajar con el valor de forma segura; los otros son `Map`, `Bind` y
`Match`.

---

## Comparación con las demás variantes

| Método | Se ejecuta si… | El delegado recibe |
| --- | --- | --- |
| `ExecSelf` (2 delegados) | Siempre (una rama u otra) | `T` / `MlErrorsDetails` |
| `ExecSelf` (1 delegado) | Siempre | `MlResult<T>` completo |
| **`ExecSelfIfValid`** | **Solo si es válido** | **`T`** |
| `ExecSelfIfFail` | Solo si es fallido | `MlErrorsDetails` |
| `ExecSelfIf` | El predicado se cumple | `T` |
| `ExecSelfIfFailWithValue` | Fallido y hay valor adjunto | `MlErrorsDetails`, `TValue` |
| `ExecSelfIfFailWithException` | Fallido y hay excepción | `MlErrorsDetails`, `Exception` |
| `ExecSelfIfFailWithoutException` | Fallido y **sin** excepción | `MlErrorsDetails` |

---

## Resumen

- `ExecSelfIfValid` ejecuta una acción **solo cuando el resultado es válido** y devuelve el mismo
  `MlResult<T>` sin modificarlo.
- Si el resultado es fallido, la acción se omite y el fallo se propaga: no necesitas ningún `if`.
- `TryExecSelfIfValid` captura las excepciones del delegado y las convierte en un fallo del
  `MlResult`, guardando la excepción en `Details["Ex"]`.
- Existen las cuatro combinaciones asíncronas (fuente sync/async × delegado sync/async).
- Es la forma idiomática de trabajar con el valor sin acceder a `Value`, que es `internal protected`.

## Ver también

- [`1_ExecSelf.md`](./1_ExecSelf.md) — la familia completa y su visión general.
- [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md) — el simétrico para la rama fallida.
- [`4_ExecSelfIfFailWithValue.md`](./4_ExecSelfIfFailWithValue.md) — recuperar el valor adjunto al fallo.
- [`5_ExecSelfIfFailWithException.md`](./5_ExecSelfIfFailWithException.md) — fallos con excepción.
- [`6_ExecSelfIfFailWithoutException.md`](./6_ExecSelfIfFailWithoutException.md) — fallos de negocio puros.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia completa con el recuento de sobrecargas.
- [`../Map/1_Map.md`](../Map/1_Map.md) y [`../Bind/3_Bind.md`](../Bind/3_Bind.md) — cuando sí quieres transformar.