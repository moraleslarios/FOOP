# Sistema de errores (`Types/Errors/*.cs`)

El sistema de errores de la librería está formado por **tres tipos** y **dos clases de extensiones**.
Su diseño es deliberadamente minimalista: la información obligatoria es un simple mensaje, y todo lo
demás (excepciones, valores de contexto, metadatos de API) viaja en un diccionario abierto de
detalles.

| Fichero | Tipo / clase | Responsabilidad |
| --- | --- | --- |
| `Types/Errors/MlError.cs` | `record MlError` | Un error individual: **solo un mensaje**. |
| `Types/Errors/MlError.cs` | `static class MlErrorExtensions` | Conversiones `string` ⇄ `MlError`. |
| `Types/Errors/MlErrorsDetails.cs` | `class MlErrorsDetails` | Conjunto de errores + diccionario de detalles. |
| `Types/Errors/MlErrorsDetails.cs` | `static class MlErrorDetailsExtensions` | Conversiones y descripciones legibles. |
| `Types/Errors/MlErrorsDetailsActions.cs` | `static class MlErrorsDetailsActions` | Operaciones sobre detalles: añadir, fusionar, consultar. |
| `Types/Errors/ErrorMessage.cs` | `record ErrorMessage(string Message)` | Envoltorio ligero de un mensaje. |

---

## `MlError`

```csharp
public record MlError
{
    public string Message { get; init; }
}
```

Un `MlError` **solo tiene `Message`**. No hay `Code` ni `Metadata`: si necesitas más información,
va en el diccionario `Details` de `MlErrorsDetails`.

| Miembro | Descripción |
| --- | --- |
| `Message` | Texto del error. Si se construye con cadena vacía o nula, se sustituye por `Constants.DEFAULT_ERROR_MESSAGE`. |
| `ToString()` | Devuelve `Message`. |
| `static FromErrorMessage(string)` | Fábrica explícita. |
| `implicit operator MlError(string)` | Permite asignar un `string` directamente. |

```csharp
// Conversión implícita: la forma habitual.
MlError error = "El email no tiene formato válido";

// Fábrica explícita, equivalente.
MlError otro = MlError.FromErrorMessage("El email no tiene formato válido");

Console.WriteLine(error);   // El email no tiene formato válido
```

Si el mensaje llega vacío, el error nunca queda "mudo":

```csharp
MlError vacio = "";
Console.WriteLine(vacio.Message);
// Without custom error message. For more info, view 'Ex(s) details exceptions.
```

### `MlErrorExtensions`

| Extensión | Devuelve |
| --- | --- |
| `ToMlError(this string)` | `MlError` |
| `ToMlErrors(this MlError)` | `IEnumerable<MlError>` con un único elemento |
| `ToMlErrors(this string)` | `IEnumerable<MlError>` con un único elemento |
| `ToMlErrors(this IEnumerable<string>)` | `IEnumerable<MlError>` |

```csharp
IEnumerable<MlError> errores = new[]
{
    "El nombre es obligatorio",
    "El email no tiene formato válido"
}.ToMlErrors();
```

---

## `MlErrorsDetails`

```csharp
public class MlErrorsDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
```

Es el objeto que transporta **todo** el fallo. Tiene exactamente **dos propiedades**:

| Propiedad | Tipo | Descripción |
| --- | --- | --- |
| `Errors` | `IEnumerable<MlError>` | Lista de errores acumulados. |
| `Details` | `Dictionary<string, object>` | Información adicional por clave. |

> ⚠️ **No existen** las propiedades `Exception`, `Value`, `HasException` ni `HasValue`. Esa
> información se guarda en `Details` bajo claves convencionales y se consulta con los métodos de
> extensión de `MlErrorsDetailsActions` (ver más abajo).

### Claves convencionales de `Details`

Definidas en `Helpers/Constants.cs`:

| Constante | Valor | Uso |
| --- | --- | --- |
| `Constants.EX_DESC_KEY` | `"Ex"` | Excepción capturada por los métodos `Try*`. Si hay varias se numeran `Ex`, `Ex2`, `Ex3`… |
| `Constants.VALUE_KEY` | `"Value"` | Valor de entrada que provocó el fallo (familias `*WithValue`). |

### Fábricas

| Método | Uso típico |
| --- | --- |
| `FromErrorMessage(string)` | Un único error a partir de un mensaje. |
| `FromErrorMessageDetails(string, Dictionary<string, object>)` | Mensaje + detalles arbitrarios. |
| `FromErrorsMessagesDetails(IEnumerable<string>, Dictionary<string, object>)` | Varios mensajes + detalles. |
| `FromError(MlError)` / `FromErrorDetails(...)` | A partir de un `MlError`. |
| `FromEnumerableErrors(...)` / `FromEnumerableErrorsDetails(...)` | A partir de una colección de `MlError`. |
| `FromEnumerableStrings(IEnumerable<string>)` | A partir de mensajes sueltos. |
| `FromErrorMessageWithException(string, Exception)` | Mensaje + excepción en `Details["Ex"]`. |
| `FromErrorMessageWithValue(string, object)` / `<T>(string, T)` | Mensaje + valor en `Details["Value"]`. |

Además dispone de una decena de **operadores implícitos**, por lo que en la práctica casi nunca hace
falta invocar las fábricas:

```csharp
// Todas estas asignaciones funcionan por conversión implícita:
MlErrorsDetails d1 = "Error único";
MlErrorsDetails d2 = new[] { "Error 1", "Error 2" };
MlErrorsDetails d3 = new MlError("Error de dominio");

// Y también al construir un MlResult fallido:
MlResult<Cliente> fallo = MlResult<Cliente>.Fail("No se encontró el cliente");
```

### `ToString()`

Produce una descripción legible con dos bloques, `MlError:` y `Details:`, ideal para logs:

```csharp
var detalles = MlErrorsDetails.FromErrorMessageWithException(
    "No se pudo conectar con el servicio de tarifas",
    new TimeoutException("The operation has timed out."));

Console.WriteLine(detalles);
// MlError:
//   No se pudo conectar con el servicio de tarifas
// Details:
//   Ex: System.TimeoutException: The operation has timed out.
```

### `MlErrorDetailsExtensions`

| Extensión | Descripción |
| --- | --- |
| `ToMlErrorsDetails(...)` (8 sobrecargas) | Convierte `string`, `MlError`, colecciones, etc. en `MlErrorsDetails`. |
| `ToErrorsMessages()` | `IEnumerable<string>` con los mensajes. Perfecto para respuestas HTTP. |
| `ToDescription()` | Descripción completa (errores + detalles) en una cadena. |
| `ToErrorsDescription()` (2 sobrecargas) | Solo los errores, concatenados. |
| `ToDetailsDescription()` | Solo el bloque de detalles. |

```csharp
IActionResult respuesta = resultado.Match(
    valid: dto     => Ok(dto),
    fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));

_log.LogError("Fallo en la operación: {Detalle}", errores.ToDescription());
```

---

## `MlErrorsDetailsActions`

Clase de extensión (27 métodos) para **construir y consultar** el contenido de un `MlErrorsDetails`.
Se agrupa en cuatro bloques.

### 1. Añadir errores

| Método | Descripción |
| --- | --- |
| `AddError(MlError)` | Añade un error. |
| `AddErrorMessage(string)` | Añade un error a partir de su mensaje. |
| `AddErrors(IEnumerable<MlError>)` / `AddErrors(params MlError[])` | Añade varios errores. |
| `AddErrorsMessages(IEnumerable<string>)` / `(params string[])` | Añade varios mensajes. |

### 2. Añadir detalles

| Método | Descripción |
| --- | --- |
| `AddDetail<T>(string key, T value)` | Añade una entrada al diccionario. |
| `AddDetails(Dictionary<string, object>)` | Añade varias entradas. |
| `AddDetails(params (string key, object value)[])` | Añade varias entradas con sintaxis de tuplas. |
| `AddDetailValue<T>(T value)` | Añade el valor bajo la clave convencional `"Value"`. |
| `AppendExDetails(Exception)` | Devuelve un `Dictionary<string, object>` con la excepción añadida bajo `"Ex"` (numerando si ya existía). |
| `AppendExDetailsToMlDetails(...)` (2) | Igual, pero devolviendo `MlErrorsDetails`. |
| `AppendExErrorDetail(...)` (2) | Añade a la vez un error y su excepción. |

```csharp
MlErrorsDetails detalles = MlErrorsDetails.FromErrorMessage("Validación fallida")
    .AddErrorsMessages("El nombre es obligatorio", "El email no es válido")
    .AddDetails(("Entidad", "Cliente"), ("Operacion", "Alta"))
    .AddDetailValue(clienteEntrante);
```

### 3. Consultar

| Método | Devuelve | Descripción |
| --- | --- | --- |
| `HasValueDetails()` / `HasValueDetailsAsync()` | `bool` | ¿Existe la clave `"Value"`? |
| `HasExceptionDetails()` / `HasExceptionDetailsAsync()` | `bool` | ¿Existe la clave `"Ex"`? |
| `HasKeyDetails(string key)` | `bool` | ¿Existe una clave arbitraria? |

Estos predicados son exactamente los que usan internamente las familias `*WithException` y
`*WithoutException` de `Bind`, `Map` y `ExecSelf` para decidir si actúan.

```csharp
if (errores.HasExceptionDetails())
    _telemetria.RegistrarExcepcion(errores.GetDetailException().Match(valid: ex => ex, fail: _ => null!));
else
    _metricas.Incrementar("errores.negocio");
```

### 4. Fusionar

| Método | Descripción |
| --- | --- |
| `Merge(...)` (2 sobrecargas) | Une dos `MlErrorsDetails` en uno (errores y detalles). |
| `MergeErrorsDetails<T, TReturn>(...)` | Fusiona y devuelve un `MlResult<TReturn>` fallido con el resultado. |
| `MergeErrorsDetailsAsync<T, TReturn>(...)` (4 sobrecargas) | Versiones asíncronas. |

Fusionar es lo que permite **acumular** errores en operaciones como
[`Combine`](./MlResultActionsSeveral.md) o `TryBindBuild`, en lugar de perder todos menos el primero.

---

## Recuperar los detalles: `MlResultActionsErrorsDetails`

La lectura tipada de los detalles (por ejemplo, recuperar la excepción o el valor original) está en
una clase aparte, documentada en su propio fichero:
[`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md).

Métodos más usados:

```csharp
// Excepción guardada bajo "Ex":
MlResult<Exception>       ex        = errores.GetDetailException();
MlResult<TimeoutException> exTipada = errores.GetDetailException<TimeoutException>();

// Valor guardado bajo "Value":
MlResult<Cliente> original = errores.GetDetailValue<Cliente>();

// Cualquier otra clave:
MlResult<string> entidad = errores.GetDetail<string>("Entidad");
```

Fíjate en que **devuelven `MlResult<T>`, no el valor crudo**: si la clave no existe o el tipo no
coincide, obtienes un `Fail` en lugar de una excepción.

---

## `ErrorMessage`

```csharp
public record ErrorMessage(string Message);
```

Envoltorio mínimo de un mensaje de error. No tiene métodos ni conversiones: es un tipo de apoyo para
firmas que necesitan distinguir un mensaje de error de un `string` cualquiera. En el código de la
tubería se usa muy poco; lo habitual es trabajar con `MlError` y `MlErrorsDetails`.

---

## Ejemplo completo: del error al log y a la respuesta HTTP

```csharp
public async Task<IActionResult> ObtenerTarifaAsync(string divisa)
{
    return await _servicio.ObtenerTarifaAsync(divisa)
        // Guardamos el contexto de la petición en los detalles del error, si falla.
        .AddMlErrorDetailIfFailAsync("Divisa", divisa)
        .ExecSelfIfFailAsync(errores =>
        {
            if (errores.HasExceptionDetails())
                _log.LogError("Error técnico obteniendo la tarifa: {Detalle}", errores.ToDescription());
            else
                _log.LogWarning("Error de negocio obteniendo la tarifa: {Errores}", errores.ToErrorsDescription());
        })
        .MatchAsync(valid: tarifa  => Ok(tarifa),
                    fail : errores => errores.HasExceptionDetails()
                                          ? StatusCode(502, new { errores = errores.ToErrorsMessages() })
                                          : BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

---

## Relación con Web API

`MlProblemsDetails` (en `MoralesLarios.OOFP.WebServices`) escribe en `Details["ProblemsDetails"]`.
Luego `MlErrorsDetailsExtensions.GetProblemDetails()` (en `MoralesLarios.OOFP.WebApi`) lo convierte en
`ProblemDetailsInfo`, permitiendo devolver respuestas conformes a RFC 7807 sin acoplar el núcleo de la
librería a ASP.NET Core.

---

## Ver también

- [`MlResult<T>`](./MlResult.md) — el tipo que transporta estos errores.
- [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) — lectura tipada de `Details`.
- [`MlResultActions`](./MlResultActions.md) — `AddMlErrorDetailIfFail`, `AddValueDetailIfFail`, etc.
- [`MlResultActionsMap`](./MlResultActionsMap.md) — familias `MapIfFailWith*`.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — familias `BindIfFailWith*`.
