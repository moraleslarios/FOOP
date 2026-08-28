# `MlResultActionsMap` (`Types/MlResultActionsMap.cs`)

Familia de extensiones para **transformar el valor válido** de un `MlResult<T>` sin alterar la
estructura de error. Es la operación más frecuente de toda la librería: convertir entidades a DTOs,
proyectar campos, formatear textos, calcular totales…

---

## Semántica

| Estado de `source` | Comportamiento de `Map` |
| --- | --- |
| `IsValid == true` | Ejecuta la función y devuelve `MlResult<TReturn>` válido con el nuevo valor. |
| `IsFail == true` | **No ejecuta nada**: propaga tal cual el `MlErrorsDetails` original, cambiando solo el tipo genérico. |

Es decir, `Map` es un **cortocircuito**: los errores viajan intactos por la tubería hasta el
`Match` final, sin `if`s ni `try/catch` intermedios.

---

## `Map` vs `Bind`: cuál usar

La diferencia está únicamente en **lo que devuelve tu función**:

| Lo que devuelve tu función | Método correcto | Motivo |
| --- | --- | --- |
| `TReturn` (un valor normal, no puede fallar) | `Map` | La operación es total: siempre produce un resultado. |
| `MlResult<TReturn>` (puede fallar) | [`Bind`](./MlResultActionsBind.md) | La operación es parcial: hay que unir dos capas de `MlResult`. |
| `TReturn` pero **puede lanzar excepción** | `TryMap` | Convierte la excepción en `Fail`, guardándola en `Details["Ex"]`. |

```csharp
// ✅ Map: proyección pura, nunca falla.
MlResult<string> nombre = clienteResult.Map(c => c.Nombre.ToUpperInvariant());

// ✅ Bind: la operación puede fallar (el cliente puede no tener crédito).
MlResult<Credito> credito = clienteResult.Bind(c => ConcederCredito(c));

// ❌ Si usaras Map con una función que devuelve MlResult, obtendrías MlResult<MlResult<Credito>>.
```

---

## Métodos de la clase

Todas las familias siguen la convención `[Try] Map [Contexto] [Async]`.

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `Map` | 1 | Transformación base síncrona. |
| `MapAsync` | 4 | Origen `Task<MlResult<T>>` y/o función asíncrona. |
| `TryMap` | 2 | Igual que `Map` capturando excepciones (mensaje `string` o `Func<Exception, string>`). |
| `TryMapAsync` | 8 | Combinaciones asíncronas de `TryMap`. |
| `MapEnsure` | 4 | Comprueba un predicado sobre el valor válido; si no se cumple, convierte a `Fail`. |
| `MapEnsureAsync` | 8 | Versiones asíncronas de `MapEnsure`. |
| `MapIf` | 2 | Aplica una transformación u otra según un predicado. |
| `MapIfAsync` | 12 | Versiones asíncronas de `MapIf`. |
| `TryMapIf` / `TryMapIfAsync` | 4 / 19 | `MapIf` con captura de excepciones. |
| `MapIfFail` | 2 | Se ejecuta **solo si el origen es `Fail`**: recupera devolviendo un valor por defecto. |
| `MapIfFailAsync` | 12 | Versiones asíncronas de `MapIfFail`. |
| `TryMapIfFail` / `TryMapIfFailAsync` | 4 / 24 | Recuperación con captura de excepciones. |
| `MapIfFailWithValue` | 2 | Recuperación usando el valor original guardado en `Details["Value"]`. |
| `MapIfFailWithValueAsync` | 9 | Versiones asíncronas. |
| `TryMapIfFailWithValue` / `...Async` | 4 / 24 | Idem con captura de excepciones. |
| `MapIfFailWithException` | 4 | Recuperación usando la excepción guardada en `Details["Ex"]`. |
| `MapIfFailWithExceptionAsync` | 24 | Versiones asíncronas. |
| `TryMapIfFailWithException` / `...Async` | 8 / 44 | Idem con captura de excepciones. |
| `MapIfFailWithExceptionError` | 4 | Como el anterior, pero la función recibe **la excepción y los errores**. |
| `MapIfFailWithExceptionErrorAsync` | 24 | Versiones asíncronas. |
| `TryMapIfFailWithExceptionError` / `...Async` | 8 / 44 | Idem con captura de excepciones. |
| `MapIfFailWithoutException` | 1 | Recuperación **solo** si el fallo **no** lleva excepción (error de negocio puro). |
| `MapIfFailWithoutExceptionAsync` | 3 | Versiones asíncronas. |
| `TryMapIfFailWithoutException` | 1+ | Idem con captura de excepciones. |
| `MapAlways` | 2 | Ejecuta la transformación **siempre**, sea válido o fallido. |
| `MapAlwaysAsync` | 3 + 4 | Versiones asíncronas de las dos formas de `MapAlways`. |
| `TryMapAlways` / `TryMapAlwaysAsync` | 2 / 6 | Idem con captura de excepciones. |
| `MapDefault` | 1 | **Solo para depuración.** Devuelve siempre `Fail` con el aviso `"Warning, MapDefault method is only valid tu debug code"`. |
| `MapDefaultAsync` | 1 | Versión asíncrona de `MapDefault`. |

> ⚠️ **`MapDefault` no debe usarse en producción.** Es un marcador temporal para dejar una rama sin
> implementar mientras se desarrolla; se comporta siempre como un fallo y su aparición en el log es
> la señal de que hay código pendiente.

> 📝 **Nota sobre un nombre del código fuente:** además de `TryMapIfAsync`, en el fuente existen tres
> sobrecargas con el nombre `TryMapIAsyncf` (errata tipográfica original). Son funcionalmente
> equivalentes; se recomienda usar `TryMapIfAsync`.

---

## Por qué hay tantas sobrecargas

Cada familia se multiplica por tres ejes independientes:

1. **Origen:** `MlResult<T>` (síncrono) o `Task<MlResult<T>>` (asíncrono).
2. **Delegado:** `Func<T, TReturn>` (síncrono) o `Func<T, Task<TReturn>>` (asíncrono).
3. **Mensaje de error** (solo en las variantes `Try*`): `string` fijo o `Func<Exception, string>` que
   construye el mensaje a partir de la excepción capturada.

Gracias a esto **nunca necesitas `await` en medio de la cadena**: puedes mezclar pasos síncronos y
asíncronos libremente y hacer un único `await` al final.

---

## Ejemplos

### `Map`: proyección simple

```csharp
public record Cliente(int Id, string Nombre, string Email, decimal Saldo);
public record ClienteDto(int Id, string NombreCompleto, string Email);

MlResult<Cliente> clienteResult = ObtenerCliente(42);

MlResult<ClienteDto> dto = clienteResult
    .Map(c => new ClienteDto(c.Id, c.Nombre.Trim(), c.Email.ToLowerInvariant()));

// Si ObtenerCliente devolvió Fail, la lambda no se ejecuta nunca
// y `dto` contiene exactamente los mismos errores.
```

### `Map` encadenado: varias transformaciones seguidas

```csharp
MlResult<string> resumen = ObtenerCliente(42)
    .Map(c        => new ClienteDto(c.Id, c.Nombre.Trim(), c.Email.ToLowerInvariant()))
    .Map(dto      => $"{dto.Id} - {dto.NombreCompleto}")
    .Map(texto    => texto.PadRight(40, '.'));
```

### `TryMap`: encapsular código que lanza excepciones

Cualquier operación de terceros (parseo, serialización, reflexión) es candidata a `TryMap`:

```csharp
MlResult<int> cantidad = MlResult<string>.Valid(entradaUsuario)
    .TryMap(func               : texto => int.Parse(texto),
            errorMessageBuilder: ex    => $"La cantidad '{entradaUsuario}' no es un número válido: {ex.Message}");

// Si int.Parse lanza FormatException:
//   - cantidad.IsFail == true
//   - el mensaje es el construido por errorMessageBuilder
//   - la excepción original queda accesible en ErrorsDetails.Details["Ex"]
```

Recuperar después esa excepción es inmediato:

```csharp
MlResult<int> conFallback = cantidad
    .MapIfFailWithException(ex => ex is FormatException ? 0 : -1);
```

### `MapEnsure`: validar sin salir de la tubería

`MapEnsure` no transforma: **filtra**. Si el predicado falla, el resultado pasa a `Fail` con el
mensaje indicado.

```csharp
MlResult<Cliente> clienteValidado = ObtenerCliente(42)
    .MapEnsure(c => !string.IsNullOrWhiteSpace(c.Email), "El cliente no tiene email registrado")
    .MapEnsure(c => c.Email.Contains('@')              , "El email del cliente no tiene formato válido")
    .MapEnsure(c => c.Saldo >= 0                       , "El cliente tiene saldo negativo");
```

Cada `MapEnsure` es una regla de negocio legible y aislada. La primera que falle corta la cadena.

### `MapIf`: dos transformaciones alternativas

```csharp
MlResult<decimal> precioFinal = ObtenerPedido(id)
    .MapIf(condition: p => p.Cliente.EsVip,
           funcTrue : p => p.Total * 0.85m,   // 15 % de descuento VIP
           funcFalse: p => p.Total);
```

### `MapAsync`: mezclando síncrono y asíncrono

```csharp
MlResult<ClienteDto> dto = await ObtenerClienteAsync(42)   // Task<MlResult<Cliente>>
    .MapEnsureAsync(c   => c.Saldo >= 0, "Saldo negativo") // predicado síncrono
    .MapAsync      (c   => _mapper.MapAsync(c))            // función asíncrona
    .MapAsync      (dto => dto with { Email = dto.Email.ToLowerInvariant() }); // función síncrona

// Un único await para toda la cadena.
```

### `MapIfFail`: valor por defecto ante error

```csharp
// Si no hay configuración guardada, usamos la configuración por defecto.
MlResult<Configuracion> config = LeerConfiguracion(rutaFichero)
    .MapIfFail(errores => Configuracion.PorDefecto());
```

### `MapIfFailWithValue`: recuperar usando la entrada que provocó el fallo

Cuando un paso anterior guardó el valor de entrada en `Details["Value"]` (por ejemplo mediante
`AddValueIfFail` o `BindSaveValueInDetailsIfFaildFuncResult`), puedes usarlo para construir la
respuesta de degradación:

```csharp
MlResult<Precio> precio = CalcularPrecio(articulo)
    .AddValueIfFail(articulo)                                       // guarda el artículo en Details["Value"]
    .MapIfFailWithValue<Articulo, Precio>(art => Precio.Estimado(art.CategoriaBase));
```

### `MapIfFailWithException` vs `MapIfFailWithoutException`

Son complementarios y permiten enrutar el error según su naturaleza:

```csharp
MlResult<Cotizacion> cotizacion = await ObtenerCotizacionAsync(divisa)
    // Error técnico: el servicio externo cayó → usamos la última cotización cacheada.
    .MapIfFailWithExceptionAsync(ex => _cache.UltimaCotizacion(divisa))
    // Error de negocio: la divisa no está soportada → cotización neutra.
    .MapIfFailWithoutExceptionAsync(errores => Cotizacion.NoDisponible(divisa));
```

| Método | Se ejecuta cuando… |
| --- | --- |
| `MapIfFailWithException` | El fallo **tiene** excepción en `Details["Ex"]` (`HasExceptionDetails() == true`). |
| `MapIfFailWithoutException` | El fallo **no tiene** excepción: es una regla de negocio o validación. |
| `MapIfFailWithExceptionError` | Igual que `WithException`, pero la función recibe además el `MlErrorsDetails` completo, útil para logs enriquecidos. |

### `MapAlways`: una salida única para ambas ramas

```csharp
// Forma con una sola función: no recibe nada, se ejecuta igual en válido y en fallido.
MlResult<DateTime> instante = ProcesarLote(lote)
    .MapAlways(() => DateTime.UtcNow);

// Forma con dos funciones: cada rama produce el mismo tipo de salida.
MlResult<RespuestaApi> respuesta = ProcesarLote(lote)
    .MapAlways(funcValidAlways: resumen => RespuestaApi.Ok(resumen),
               funcFailAlways : errores => RespuestaApi.ConErrores(errores.ToErrorsMessages()));
```

> Si solo quieres **observar** sin transformar (log, métrica, auditoría), usa
> [`ExecSelf`](./MlResultActionsExecSelf.md), que devuelve el `MlResult` original intacto.
> Si vas a **consumir** definitivamente el resultado y salir de la tubería, usa
> [`Match`](./MlResultActionsMatch.md).

---

## Ejemplo completo: pipeline realista

```csharp
public async Task<IActionResult> ActualizarEmail(int clienteId, string nuevoEmail)
{
    return await EnsureFp.NotNullEmptyOrWhitespace(nuevoEmail, "El email es obligatorio")
        .MapEnsure     (email  => email.Contains('@'), "El email no tiene formato válido")
        .Map           (email  => email.Trim().ToLowerInvariant())      // normalización pura
        .BindAsync     (email  => _repo.ObtenerClienteAsync(clienteId)  // puede fallar → Bind
                                       .MapAsync(c => c with { Email = email }))
        .TryBindAsync  (funcAsync          : c  => _repo.GuardarAsync(c),
                        errorMessageBuilder: ex => $"Error al guardar el cliente {clienteId}: {ex.Message}")
        .MapAsync      (c      => new ClienteDto(c.Id, c.Nombre, c.Email))
        .MatchAsync    (valid  : dto     => Ok(dto),
                        fail   : errores => BadRequest(errores.ToErrorsMessages()));
}
```

Puntos a destacar:

1. `MapEnsure` valida sin salir de la tubería.
2. `Map` normaliza (operación pura, no puede fallar).
3. `Bind`/`TryBind` se usan cuando la operación **puede** fallar o lanzar.
4. `Map` final proyecta a DTO.
5. `Match` es el **único** punto donde se sale del mundo `MlResult`.

---

## Documentación detallada por concepto

Cada familia tiene su propio documento con la lista completa de sobrecargas y más ejemplos:

- [1. `Map`](../Map/1_Map.md)
- [2. `MapEnsure`](../Map/2_MapEnsure.md)
- [3. `MapIf`](../Map/3_MapIf.md)
- [4. `MapIfFail`](../Map/4_MapIfFail.md)
- [5. `MapIfFailWithValue`](../Map/5_MapIfFailWithValue.md)
- [6. `MapIfFailWithException`](../Map/6_MapIfFailWithException.md)
- [7. `MapIfFailWithoutException`](../Map/7_MapIfFailWithoutException.md)
- [8. `MapAlways`](../Map/8_MapAlways.md)

## Ver también

- [`MlResultActionsBind`](./MlResultActionsBind.md) — cuando la función devuelve `MlResult<T>`.
- [`MlResultActionsMatch`](./MlResultActionsMatch.md) — salida final de la tubería.
- [`MlResultActionsExecSelf`](./MlResultActionsExecSelf.md) — efectos laterales sin transformar.
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — creación de resultados validados al inicio de la cadena.
