# `MlResultActionsErrorsDetails` (`Types/MlResultActionsErrorsDetails.cs`)

Extensiones para **leer y fusionar** el contenido del diccionario `Details` de un `MlErrorsDetails`.

Recuerda que `MlErrorsDetails` solo tiene dos propiedades, `Errors` y `Details`. Todo lo demás
(excepciones, valores originales, metadatos) vive en `Details` bajo claves. Esta clase es la puerta de
entrada **tipada y segura** a ese diccionario.

> 🔑 **Clave de diseño:** todos los `Get*` devuelven `MlResult<T>`, **no el valor crudo**. Si la clave
> no existe o el tipo no coincide, obtienes un `Fail` descriptivo en lugar de una
> `KeyNotFoundException` o un `InvalidCastException`. Así la lectura de detalles se integra en la misma
> tubería que el resto del código.

---

## Métodos de la clase

### Lectura de detalles

| Método | Devuelve | Descripción |
| --- | --- | --- |
| `GetDetail<T>(this MlErrorsDetails, string key)` | `MlResult<T>` | Valor de una clave arbitraria, tipado. |
| `GetDetailAsync<T>(...)` | `Task<MlResult<T>>` | Versión asíncrona. |
| `GetDetailValue<T>(this MlErrorsDetails)` | `MlResult<T>` | Valor guardado bajo la clave convencional `"Value"`. |
| `GetDetailValueAsync<T>(...)` (2) | `Task<MlResult<T>>` | Versiones asíncronas. |
| `GetDetailException(this MlErrorsDetails)` | `MlResult<Exception>` | Excepción guardada bajo `"Ex"`. |
| `GetDetailExceptionAsync(...)` (2) | `Task<MlResult<Exception>>` | Versiones asíncronas. |
| `GetDetailException<T>(this MlErrorsDetails) where T : Exception` | `MlResult<T>` | Excepción **tipada**; falla si el tipo real no coincide. |
| `GetDetailException<T>Async(...)` (2) | `Task<MlResult<T>>` | Versiones asíncronas. |

### Escritura de detalles

| Método | Descripción |
| --- | --- |
| `AddValueIfFail<T, TValue>(this MlResult<T>, TValue value)` | Si el resultado es fallido, guarda `value` bajo la clave `"Value"`. |
| `AddValueIfFailAsync<T, TValue>(...)` (2) | Versiones asíncronas. |

### Fusión de errores

| Método | Descripción |
| --- | --- |
| `MergeErrorsDetailsIfFail<T>(this MlResult<T>, MlResult<T> other)` | Si alguno falla, une los errores y detalles de ambos. |
| `MergeErrorsDetailsIfFailAsync<T>(...)` (4) | Versiones asíncronas. |
| `MergeErrorsDetailsIfFailDiferentTypes<T, T2>(...)` | Igual, pero permitiendo que los dos resultados sean de **tipos distintos**. El resultado conserva el tipo `T`. |
| `MergeErrorsDetailsIfFailDiferentTypesAsync<T, T2>(...)` (4) | Versiones asíncronas. |

> 📝 El nombre `DiferentTypes` está escrito así en el código fuente (con una sola `f`); se documenta
> tal cual para que puedas buscarlo.

---

## Claves convencionales

Definidas en `Helpers/Constants.cs` y usadas por toda la librería:

| Constante | Valor | Quién la escribe |
| --- | --- | --- |
| `Constants.EX_DESC_KEY` | `"Ex"` | Todos los métodos `Try*` al capturar una excepción. Si ya existía, se numeran `Ex2`, `Ex3`… |
| `Constants.VALUE_KEY` | `"Value"` | `AddValueIfFail`, `AddValueDetailIfFail` y las familias `*IfFailWithValue`. |

---

## Ejemplo 1: distinguir error técnico de error de negocio

`GetDetailException` es la forma canónica de saber **qué** excepción se produjo, no solo que se produjo:

```csharp
public IActionResult TraducirError(MlErrorsDetails errores)
{
    return errores.GetDetailException().Match(
        // Había una excepción: es un fallo técnico.
        valid: ex => ex switch
        {
            TimeoutException        => StatusCode(504, new { errores = errores.ToErrorsMessages() }),
            HttpRequestException    => StatusCode(502, new { errores = errores.ToErrorsMessages() }),
            UnauthorizedAccessException => StatusCode(403, new { errores = errores.ToErrorsMessages() }),
            _                       => StatusCode(500, new { errores = errores.ToErrorsMessages() })
        },
        // No había excepción: es un error de validación o de reglas de negocio.
        fail: _ => BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

### Versión tipada: `GetDetailException<T>`

Cuando solo te interesa **un** tipo concreto de excepción, la sobrecarga genérica evita el `switch` y
falla limpiamente si la excepción es de otra clase:

```csharp
// ¿Fue concretamente un timeout? Si fue otra cosa (o no hubo excepción), obtenemos Fail.
MlResult<TimeoutException> timeout = errores.GetDetailException<TimeoutException>();

bool debeReintentar = timeout.IsValid;
```

---

## Ejemplo 2: recuperar el valor de entrada que falló

Escenario típico: un lote de importación. Cuando una línea falla, queremos poder mostrar **qué línea**
fue, sin ensuciar la firma de los métodos con parámetros de diagnóstico.

```csharp
public MlResult<ClienteImportado> Importar(LineaCsv linea)
    => ParsearLinea(linea)
          // Guardamos la línea original en Details["Value"] si algo falla.
          .AddValueIfFail(linea)
          .Bind(ValidarCliente)
          .Bind(GuardarCliente);

// Más tarde, al construir el informe de errores:
public string DescribirFallo(MlErrorsDetails errores)
    => errores.GetDetailValue<LineaCsv>().Match(
           valid: l => $"Línea {l.Numero}: {string.Join(';', l.Campos)} → {errores.ToErrorsDescription()}",
           fail : _ => $"Error sin línea asociada → {errores.ToErrorsDescription()}");
```

---

## Ejemplo 3: leer una clave arbitraria con `GetDetail<T>`

Combínalo con `AddMlErrorDetailIfFail` de [`MlResultActions`](./MlResultActions.md) para transportar
cualquier contexto:

```csharp
MlResult<Tarifa> tarifa = await _servicio.ObtenerTarifaAsync(divisa)
    .AddMlErrorDetailIfFailAsync("Divisa"       , divisa)
    .AddMlErrorDetailIfFailAsync("CorrelationId", _contexto.CorrelationId)
    .AddMlErrorDetailIfFailAsync("Intento"      , intento);

// En el manejador de errores:
tarifa.ExecSelfIfFail(errores =>
{
    string divisaFallida = errores.GetDetail<string>("Divisa").Match(valid: d => d, fail: _ => "?");
    int    numIntento    = errores.GetDetail<int>("Intento").Match(valid: i => i, fail: _ => 0);

    _log.LogWarning("Tarifa {Divisa} falló en el intento {Intento}", divisaFallida, numIntento);
});
```

Fíjate en el patrón `.Match(valid: x => x, fail: _ => valorPorDefecto)`: es la forma idiomática de
"extraer con valor por defecto" sin salir del modelo funcional.

---

## Ejemplo 4: fusionar errores de dos resultados

`Bind` es **cortocircuitante**: en cuanto algo falla, lo demás no se ejecuta y solo verás el primer
error. Cuando quieres **acumular**, fusiona.

```csharp
MlResult<Cliente>   cliente   = ValidarCliente(dto);
MlResult<Direccion> direccion = ValidarDireccion(dto.Direccion);

// El resultado conserva el tipo Cliente, pero acumula los errores de ambos.
MlResult<Cliente> validado = cliente
    .MergeErrorsDetailsIfFailDiferentTypes(direccion);

// Si ambos fallan, el usuario ve TODOS los problemas de una sola vez:
// - El nombre es obligatorio
// - El código postal no es válido
```

Cuando los dos resultados son del **mismo tipo**, usa la sobrecarga simple:

```csharp
MlResult<Importe> total = ValidarImporteBase(dto)
    .MergeErrorsDetailsIfFail(ValidarImporteImpuestos(dto));
```

### Comparativa de estrategias de acumulación

| Herramienta | Ejecuta todo | Devuelve | Cuándo usarla |
| --- | --- | --- | --- |
| `Bind` | No (corta al primer fallo) | El primer error | Pasos dependientes. |
| `MergeErrorsDetailsIfFail*` | Sí (ambos ya calculados) | Errores unidos | Validaciones independientes ya evaluadas. |
| [`Combine`](./MlResultActionsSeveral.md) | Sí | Tupla o errores unidos | Varios resultados independientes que además quieres **transportar**. |
| `TryBindBuild` | Sí | Objeto construido o errores unidos | Construir un objeto a partir de varias fuentes. |

---

## Ejemplo 5: tubería completa con diagnóstico enriquecido

```csharp
public async Task<IActionResult> ProcesarPagoAsync(PagoDto dto)
{
    return await ValidarPago(dto)
        .AddValueIfFail(dto)                                     // el DTO viaja en Details["Value"]
        .TryBindAsync(funcAsync          : p  => _pasarela.CobrarAsync(p),
                      errorMessageBuilder: ex => $"Error en la pasarela: {ex.Message}")
        .AddMlErrorDetailIfFailAsync("Pasarela", _pasarela.Nombre)
        .ExecSelfIfFailAsync(errores =>
        {
            // 1) ¿Excepción técnica?
            errores.GetDetailException().ExecSelfIfValid(ex =>
                _log.LogError(ex, "Fallo técnico procesando el pago"));

            // 2) ¿Tenemos el DTO original para reintentar?
            errores.GetDetailValue<PagoDto>().ExecSelfIfValid(p =>
                _colaReintentos.Encolar(p));
        })
        .MatchAsync(
            valid: recibo  => Ok(recibo),
            fail : errores => errores.GetDetailException<TimeoutException>().IsValid
                                  ? Accepted(new { mensaje = "Pago encolado para reintento" })
                                  : BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

---

## Ver también

- [`MlResultErrors`](./MlResultErrors.md) — `MlError`, `MlErrorsDetails` y `MlErrorsDetailsActions`.
- [`MlResultActions`](./MlResultActions.md) — `AddMlErrorDetailIfFail`, `AddValueDetailIfFail`.
- [`MlResultActionsMap`](./MlResultActionsMap.md) — `MapIfFailWithException`, `MapIfFailWithValue`.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — `BindIfFailWithException`, `BindIfFailWithValue`.
- [`MlResultActionsExecSelf`](./MlResultActionsExecSelf.md) — efectos laterales sobre los detalles del error.
