# `MlResultTransformations` (`Types/MlResultTransformations.cs`)

Clase de extensiones que actúa como **frontera** entre el mundo imperativo tradicional (valores
sueltos, excepciones, `Task`) y el mundo de `MlResult<T>`.

Aquí están los métodos que:

- **Entran** en la tubería: convierten un valor, un mensaje o una función peligrosa en un `MlResult<T>`.
- **Salen** de la tubería o cambian su forma: boxing/unboxing a `MlResult<object>`, paso a `Task`, etc.

Son también los cimientos internos de la librería: `TryToMlResult` es lo que hace funcionar a
`TryBindAlways`, `TryMapAlways` y `TryMatch`.

---

## Familias de métodos

### Entrar en la tubería desde un valor

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `ToMlResult<T>(this T)` | 1 | Envuelve un valor en un `MlResult<T>` **válido**. |
| `ToMlResultAsync<T>(this T)` | 1 | Igual, devolviendo `Task<MlResult<T>>`. |
| `ToMlResultValid<T>(this T)` | 1 | Alias explícito de `ToMlResult`, más legible en tuberías. |
| `ToMlResultValidAsync<T>(this T)` | 1 | Versión asíncrona. |
| `ToMlTaskResult<T>(this MlResult<T>)` | 1 | Convierte `MlResult<T>` en `Task<MlResult<T>>`. |

### Entrar en la tubería con un fallo

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `ToMlResultFail<T>(...)` | 14 | Crea un `MlResult<T>` **fallido** desde `string`, `MlError`, colecciones, `MlErrorsDetails`, `Exception`, mensaje + excepción, mensaje + valor… |
| `ToMlResultFailAsync<T>(...)` | 15 | Versiones asíncronas. |
| `TryToMlResultErrors<T>(...)` | 2 | Ejecuta una función y, si lanza, produce un fallo con los errores ya construidos. |
| `TryToMlResultErrorsAsync<T>(...)` | 2 | Versiones asíncronas. |

### Ejecutar código peligroso: `TryToMlResult`

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `TryToMlResult<T>(this Func<T>, ...)` | 11 | Ejecuta la función dentro de un `try/catch`; si lanza, devuelve `Fail` con la excepción en `Details["Ex"]`. |
| `TryToMlResultAsync<T>(this Func<Task<T>>, ...)` | 11 | Versiones asíncronas. |

### Boxing / unboxing (`MlResult<object>`)

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `ToMlResultObject<T>(this MlResult<T>)` | 2 | Convierte `MlResult<T>` en `MlResult<object>`. |
| `ToMlResultObjectAsync<T>(...)` | 4 | Versiones asíncronas. |
| `FromMlResultObject<T>(this MlResult<object>)` | 1 | Vuelve a `MlResult<T>`; si el tipo real no coincide, devuelve `Fail`. |
| `FromMlResultObjectAsync<T>(...)` | 2 | Versiones asíncronas. |
| `SecureGetValueFromMlResultBoxed<T>(this object)` | 1 | Extrae el valor de un `MlResult<T>` recibido como `object` (reflexión segura). |
| `SecureGetValueFromMlResultBoxedAsync<T>(...)` | 2 | Versiones asíncronas. |

### Utilidad

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `BuildErrorMessage(...)` | 2 | Construye el mensaje estándar de error a partir de una excepción y un mensaje adicional opcional. |

---

## `ToMlResult` / `ToMlResultValid`: empezar una tubería

El punto de entrada más común. Convierte cualquier valor en un resultado válido para poder empezar a
encadenar:

```csharp
// Sin extensiones (verboso):
MlResult<int> a = MlResult<int>.Valid(42);

// Con extensiones (idiomático):
MlResult<int> b = 42.ToMlResult();
MlResult<int> c = 42.ToMlResultValid();   // idéntico, pero explícito sobre la intención

// Encadenando desde el primer momento:
MlResult<string> nombreNormalizado = entrada
    .ToMlResult()
    .MapEnsure(s => !string.IsNullOrWhiteSpace(s), "El nombre es obligatorio")
    .Map(s => s.Trim().ToUpperInvariant());
```

En métodos que ya son `async`, `ToMlResultAsync` evita un `await` innecesario:

```csharp
public Task<MlResult<Configuracion>> ObtenerConfiguracionCacheadaAsync()
    => _cache is not null
           ? _cache.ToMlResultAsync()
           : CargarConfiguracionAsync();
```

---

## `ToMlResultFail`: crear un fallo (14 sobrecargas)

Las 14 sobrecargas cubren todas las formas de describir un error. Las más usadas:

```csharp
// 1) Desde un mensaje.
MlResult<Cliente> r1 = "No se encontró el cliente".ToMlResultFail<Cliente>();

// 2) Desde varios mensajes (validación acumulada).
MlResult<Cliente> r2 = new[] { "El nombre es obligatorio",
                               "El email no es válido" }.ToMlResultFail<Cliente>();

// 3) Desde una excepción: la excepción queda en Details["Ex"].
try { /* ... */ }
catch (Exception ex) { return ex.ToMlResultFail<Cliente>(); }

// 4) Mensaje + excepción: lo mejor de ambos mundos.
MlResult<Cliente> r4 = "Error al leer el cliente de la base de datos"
                           .ToMlResultFail<Cliente>(ex);

// 5) Mensaje + valor de contexto: el valor queda en Details["Value"].
MlResult<Cliente> r5 = "El DNI no supera la validación de dígito de control"
                           .ToMlResultFail<Cliente>(dniEntrante);

// 6) Desde un MlErrorsDetails ya construido.
MlResult<Cliente> r6 = detalles.ToMlResultFail<Cliente>();
```

Compara la versión imperativa con la funcional en un método de dominio:

```csharp
// ❌ Estilo imperativo: el contrato no dice nada del fallo.
public Cliente Crear(string nombre, string email)
{
    if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre obligatorio");
    return new Cliente(nombre, email);
}

// ✅ Estilo funcional: el fallo es parte del tipo de retorno.
public MlResult<Cliente> Crear(string nombre, string email)
    => string.IsNullOrWhiteSpace(nombre)
           ? "El nombre es obligatorio".ToMlResultFail<Cliente>()
           : new Cliente(nombre, email).ToMlResult();
```

---

## `TryToMlResult`: envolver código que lanza excepciones

Este es el método que **convierte una excepción en un `Fail`**. Es el mecanismo que sustenta a todos los
métodos `Try*` de la librería (`TryBind`, `TryMap`, `TryMatch`, `TryBindAlways`, `TryMapAlways`…).

```csharp
// Una función que puede lanzar.
Func<Configuracion> cargar = () => JsonSerializer.Deserialize<Configuracion>(
                                       File.ReadAllText(ruta))!;

// La convertimos en un resultado, con mensaje de error a medida.
MlResult<Configuracion> config = cargar.TryToMlResult(
    ex => $"No se pudo cargar la configuración de '{ruta}': {ex.Message}");

// Si el fichero no existe, obtenemos:
//   Errors : ["No se pudo cargar la configuración de 'app.json': Could not find file..."]
//   Details: { "Ex": FileNotFoundException }
```

Las sobrecargas se distinguen por cómo se construye el mensaje y por si hay detalles previos que
conservar:

| Forma del parámetro | Cuándo usarla |
| --- | --- |
| `Func<Exception, string> errorMessageBuilder` | Quieres incluir datos de la excepción en el mensaje. |
| `string exceptionAditionalMessage` | Te basta un prefijo fijo; se combina con `Constants.DEFAULT_EX_ERROR_MESSAGE`. |
| Sin mensaje | Se usa el mensaje por defecto de la excepción. |
| Con `MlErrorsDetails` previo | Se **fusionan** los errores anteriores con la excepción nueva. |

Versión asíncrona:

```csharp
Func<Task<Tarifa>> descargar = () => _http.GetFromJsonAsync<Tarifa>(url)!;

MlResult<Tarifa> tarifa = await descargar.TryToMlResultAsync(
    ex => $"Error descargando la tarifa desde {url}: {ex.Message}");
```

> 💡 En una tubería ya establecida no llames a `TryToMlResult` directamente: usa `TryBind` o `TryMap`,
> que lo hacen por ti y además propagan el estado anterior.

---

## `BuildErrorMessage`

Genera el mensaje estándar de la librería a partir de una excepción, opcionalmente con un texto
adicional. Es lo que hace que todos los errores técnicos tengan el mismo formato:

```csharp
string mensaje = ex.BuildErrorMessage();
// An error occurred while executing the function. Error: The operation has timed out..More info in Ex Details.

string conContexto = ex.BuildErrorMessage("Al consultar el servicio de tarifas");
```

Úsalo si construyes tus propios helpers `Try*` y quieres que los mensajes sean coherentes con el resto.

---

## Boxing: `ToMlResultObject` y `FromMlResultObject`

Necesarios cuando trabajas con **colecciones heterogéneas de resultados** o con API basadas en
reflexión. Es exactamente lo que hace `TryBindBuild` internamente: recibe
`Func<T, MlResult<object>>[]` porque cada función puede devolver un tipo distinto.

```csharp
// Varios resultados de tipos diferentes, homogeneizados a MlResult<object>.
var pasos = new List<MlResult<object>>
{
    ValidarNombre(dto.Nombre).ToMlResultObject(),
    ValidarEdad(dto.Edad).ToMlResultObject(),
    ValidarEmail(dto.Email).ToMlResultObject()
};

bool todoOk = pasos.All(p => p.IsValid);

// Recuperar el tipo original de forma segura.
MlResult<string> nombre = pasos[0].FromMlResultObject<string>();

// Si el tipo no coincide, no hay InvalidCastException: hay Fail.
MlResult<int> mal = pasos[0].FromMlResultObject<int>();   // Fail
```

### `SecureGetValueFromMlResultBoxed`

Un escalón más abajo: recibe un `object` que **es** un `MlResult<T>` (por ejemplo, obtenido por
reflexión) y extrae su valor sin arriesgarse a excepciones de casting.

```csharp
object resultadoDesconocido = metodo.Invoke(instancia, args)!;   // devuelve MlResult<Factura>

MlResult<Factura> factura = resultadoDesconocido
    .SecureGetValueFromMlResultBoxed<Factura>();
```

Es un método de **infraestructura**: en código de aplicación no deberías necesitarlo.

---

## `ToMlTaskResult`: adaptar firmas

Cuando una interfaz exige `Task<MlResult<T>>` pero tu implementación es sincrónica:

```csharp
public interface IValidador
{
    Task<MlResult<Pedido>> ValidarAsync(Pedido pedido);
}

public class ValidadorEnMemoria : IValidador
{
    public Task<MlResult<Pedido>> ValidarAsync(Pedido pedido)
        => Validar(pedido).ToMlTaskResult();      // sin async/await innecesario

    private MlResult<Pedido> Validar(Pedido pedido)
        => pedido.Lineas.Any()
               ? pedido.ToMlResult()
               : "El pedido no tiene líneas".ToMlResultFail<PedidoDto>(dto));
}
```

---

## Ejemplo completo: de la entrada cruda al resultado

```csharp
public async Task<MlResult<PedidoConfirmado>> ConfirmarAsync(string json)
{
    // 1) Deserialización peligrosa → TryToMlResult.
    Func<PedidoDto> deserializar = () => JsonSerializer.Deserialize<PedidoDto>(json)!;

    return await deserializar
        .TryToMlResult(ex => $"El JSON recibido no es un pedido válido: {ex.Message}")
        // 2) Validaciones de dominio → ToMlResultFail explícito.
        .Bind(dto => dto.Lineas.Any()
                         ? dto.ToMlResult()
                         : "El pedido no tiene líneas".ToMlResultFail<PedidoDto>(dto))
        // 3) Llamada externa peligrosa → TryBindAsync (que usa TryToMlResultAsync por dentro).
        .TryBindAsync(funcAsync          : dto => _pasarela.ConfirmarAsync(dto),
                      errorMessageBuilder: ex  => $"La pasarela rechazó la confirmación: {ex.Message}")
        .MapAsync(respuesta => new PedidoConfirmado(respuesta.Id, respuesta.Fecha));
}
```

---

## Ver también

- [`MlResult<T>`](./MlResult.md) — el tipo destino de todas estas conversiones.
- [`MlResultErrors`](./MlResultErrors.md) — `MlError`, `MlErrorsDetails` y sus conversiones implícitas.
- [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) — leer la excepción guardada por `TryToMlResult`.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — `TryBind*`, construidos sobre `TryToMlResult`.
- [`MlResultActionsMap`](./MlResultActionsMap.md) — `TryMap*`, idem.
- [Documentación por concepto: `Transformations`](../Transformations/Transformations.md)
