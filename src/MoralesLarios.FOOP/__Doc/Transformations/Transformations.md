# Transformations — Fábricas, conversiones y captura de excepciones

## Índice

1. [Introducción](#introducción)
2. [Grupo 1: `ToMlResultValid` — entrar al carril](#grupo-1-tomlresultvalid--entrar-al-carril)
3. [Grupo 2: `ToMlResultFail` — 14 formas de fallar](#grupo-2-tomlresultfail--14-formas-de-fallar)
4. [Grupo 3: `TryToMlResult` — envolver código que lanza](#grupo-3-trytomlresult--envolver-código-que-lanza)
5. [Grupo 4: `TryToMlResultErrors` — ejecutar en la rama de fallo](#grupo-4-trytomlresulterrors--ejecutar-en-la-rama-de-fallo)
6. [Grupo 5: cambio de tipo del carril](#grupo-5-cambio-de-tipo-del-carril)
7. [Grupo 6: `object` y reflexión](#grupo-6-object-y-reflexión)
8. [`BuildErrorMessage` y el mensaje por defecto](#builderrormessage-y-el-mensaje-por-defecto)
9. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`MlResultTransformations` es la clase de **fábricas y conversiones** de la librería. Es la
menos vistosa y la más usada: todos los demás operadores (`Map`, `Bind`, `ExecSelf`…)
dependen de ella internamente. Cubre cuatro necesidades:

1. **Entrar al carril**: convertir un valor o un error en `MlResult<T>`.
2. **Capturar excepciones**: envolver código que lanza y convertir la excepción en un fallo.
3. **Cambiar el tipo** del carril conservando los errores.
4. **Trabajar con `object`** cuando el tipo concreto no se conoce en compilación.

```csharp
// Entrar al carril
MlResult<Cliente> ok   = cliente.ToMlResultValid();
MlResult<Cliente> mal  = "El cliente no existe".ToMlResultFail<Cliente>();

// Capturar una excepción de código legado
Func<string, Cliente> parsear = Legacy.ParsearCliente;
MlResult<Cliente> r = parsear.TryToMlResult(linea, ex => $"Línea ilegible: {ex.Message}");
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Grupo 1: `ToMlResultValid` — entrar al carril

```csharp
public static MlResult<T> ToMlResultValid<T>(this T source) => source;

public static Task<MlResult<T>> ToMlResultValidAsync<T>(this T source)
    => source.ToMlResultValid().ToAsync();
```

Es la conversión más simple de toda la librería: se apoya en la **conversión implícita** de
`T` a `MlResult<T>` que declara el propio tipo. Funciona con cualquier `T`, incluidos value
types y `null`.

```csharp
var r1 = cliente.ToMlResultValid();          // MlResult<Cliente>
var r2 = 42.ToMlResultValid();               // MlResult<int>
var r3 = await cliente.ToMlResultValidAsync();

// ⚠️ No comprueba null: esto es un resultado VÁLIDO que contiene null
Cliente? c = null;
var r4 = c.ToMlResultValid();                // Valid(null)

// ✅ Si quieres rechazar el null, usa NullToFailed
var r5 = c.NullToFailed("El cliente es obligatorio");
```

💡 Como existe la conversión implícita, muchas veces `ToMlResultValid()` es opcional: puedes
devolver el valor directamente si el tipo de retorno es `MlResult<T>`. Aun así, escribirlo
hace el código más explícito y es imprescindible en expresiones lambda donde el compilador
necesita ayuda para inferir el tipo.

---

## Grupo 2: `ToMlResultFail` — 14 formas de fallar

Esta es la familia más numerosa. Todas las sobrecargas construyen un `MlResult<T>` fallido a
partir de distintas representaciones del error:

```csharp
public static MlResult<T> ToMlResultFail<T>(this MlErrorsDetails      source) => source;
public static MlResult<T> ToMlResultFail<T>(this MlError              source) => source;
public static MlResult<T> ToMlResultFail<T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>();
public static MlResult<T> ToMlResultFail<T>(this List<MlError>        source) => source;
public static MlResult<T> ToMlResultFail<T>(this List<string>         source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
public static MlResult<T> ToMlResultFail<T>(this MlError[]            source) => source;
public static MlResult<T> ToMlResultFail<T>(this string[]             source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
public static MlResult<T> ToMlResultFail<T>(this IEnumerable<MlError> source) => new MlErrorsDetails(source);
public static MlResult<T> ToMlResultFail<T>(this IEnumerable<string>  source) => MlErrorsDetails.FromErrorsMessagesDetails(source);
// … más las cuatro sobrecargas de TUPLA (error + Details)
```

### Resumen de las 14 sobrecargas

| Origen | Uso típico |
|--------|-----------|
| `MlErrorsDetails` | Propagar errores ya construidos |
| `MlError` | Error de catálogo |
| `string` | El caso más habitual |
| `List<MlError>` / `MlError[]` / `IEnumerable<MlError>` | Varios errores de catálogo |
| `List<string>` / `string[]` / `IEnumerable<string>` | Varios mensajes |
| `(IEnumerable<MlError>, Dictionary<string,object>)` | Errores + diagnóstico |
| `(IEnumerable<string>, Dictionary<string,object>)` | Mensajes + diagnóstico |
| `(MlError, Dictionary<string,object>)` | Un error + diagnóstico |
| `(string, Dictionary<string,object>)` | Un mensaje + diagnóstico |

🔑 **Las sobrecargas de tupla son muy cómodas** y suelen pasar desapercibidas: permiten
adjuntar `Details` sin llamar explícitamente a `MlErrorsDetails.FromErrorMessageDetails`:

```csharp
// Con tupla: conciso
return ("El pedido no existe", new Dictionary<string, object> { ["PedidoId"] = id, ["NoEncontrado"] = true })
           .ToMlResultFail<Pedido>();

// Equivalente explícito
return MlErrorsDetails.FromErrorMessageDetails("El pedido no existe",
           new Dictionary<string, object> { ["PedidoId"] = id, ["NoEncontrado"] = true })
       .ToMlResultFail<Pedido>();
```

⚠️ **Siempre hay que indicar el tipo genérico** `<T>`, porque no se puede inferir del
receptor:

```csharp
// ❌ No compila: falta el tipo del resultado
// return "Error".ToMlResultFail();

// ✅
return "Error".ToMlResultFail<Cliente>();
```

Las 14 sobrecargas tienen su gemela `ToMlResultFailAsync<T>`, que simplemente añade
`.ToAsync()`.

---

## Grupo 3: `TryToMlResult` — envolver código que lanza

Aquí está el valor real de esta clase: **convertir excepciones en fallos del carril**. Es la
maquinaria que usan por debajo todos los operadores `Try*` (`TryMap`, `TryBind`,
`TryExecSelf`…).

### Firmas principales

```csharp
// Delegado con argumento
public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, TReturn> source,
                                                               T                value,
                                                               string           exceptionAditionalMessage = null!)

public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, TReturn>        source,
                                                               T                       value,
                                                               Func<Exception, string> errorMessageBuilder)

// El delegado ya devuelve MlResult
public static MlResult<TReturn> TryToMlResult<T, TReturn>(this Func<T, MlResult<TReturn>> source,
                                                               T                          value, /* … */)

// Sin argumentos
public static MlResult<T> TryToMlResult<T>(this Func<T>           source, Func<Exception, string> b = null!)
public static MlResult<T> TryToMlResult<T>(this Func<MlResult<T>> source, Func<Exception, string> b = null!)

// Acciones (efectos laterales): devuelven el valor de entrada
public static MlResult<T> TryToMlResult<T>(this Action<T> source, T value, Func<Exception, string> b = null!)
public static MlResult<T> TryToMlResult<T>(this Action    source, T value, Func<Exception, string> b = null!)
```

### Qué contiene el fallo

Cuando se captura una excepción, el resultado se construye así:

```csharp
string message = BuildErrorMessage(errorMessageBuilder, ex);

var errorDetails = new MlErrorsDetails(
        Errors : new List<MlError> { new MlError(message) },
        Details: new Dictionary<string, object> { { EX_DESC_KEY, ex } });

return errorDetails.ToMlResultFail<T>();
```

🔑 **La excepción se guarda en `Details`** bajo la clave `EX_DESC_KEY`. Por eso puedes
recuperarla después con `GetDetailException()`, y por eso funcionan
[`MapIfFailWithException`](../Map/6_MapIfFailWithException.md) y
[`BindIfFailWithException`](../Bind/8_BindIfFailWithException.md).

```csharp
Func<string, Pedido> deserializar = json => JsonSerializer.Deserialize<Pedido>(json)!;

var r = deserializar.TryToMlResult(json, ex => $"JSON inválido: {ex.Message}");

// La excepción original sigue disponible
r.ExecSelfIfFailWithException(ex => _log.LogError(ex, "Fallo al deserializar"));
```

### Uso con `Action`: efectos laterales seguros

Las sobrecargas de `Action` devuelven el **valor de entrada**, no el resultado de la acción
(que no existe). Sirven para envolver operaciones de E/S:

```csharp
Action<Pedido> guardar = p => _archivo.Escribir(p);

var r = guardar.TryToMlResult(pedido, ex => $"No se pudo guardar el pedido: {ex.Message}");
// Si no lanza → Valid(pedido). Si lanza → Fail con la excepción en Details.
```

### Variantes asíncronas

Cada firma tiene su `TryToMlResultAsync` correspondiente: `Func<T, Task<TReturn>>`,
`Func<Task<T>>`, `Func<Task<MlResult<T>>>`, `Func<T, Task>`, `Func<Task>`. Todas capturan la
excepción **después del `await`**, que es lo correcto.

---

## Grupo 4: `TryToMlResultErrors` — ejecutar en la rama de fallo

Estas sobrecargas son especiales: **el resultado es siempre fallido**. Están pensadas para
ejecutar un efecto lateral cuando el carril ya viene roto, sin perder los errores
originales.

```csharp
public static MlResult<T> TryToMlResultErrors<T>(this Action<MlErrorsDetails> source,
                                                      MlErrorsDetails         errorsDetails,
                                                      Func<Exception, string> errorMessageBuilder = null!)
{
    try
    {
        source(errorsDetails);
        result = errorsDetails.ToMlResultFail<T>();     // ← devuelve los errores ORIGINALES
    }
    catch (Exception ex)
    {
        result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);  // ← los AMPLÍA
    }
    return result;
}
```

| Situación | Resultado |
|-----------|-----------|
| La acción se ejecuta sin lanzar | `Fail` con los **errores originales** intactos |
| La acción lanza | `Fail` con los originales **más** la excepción añadida (`AppendExErrorDetail`) |

🔑 Este es el mecanismo que hace que `TryExecSelfIfFail` y compañía **nunca pierdan el error
original**, ni siquiera cuando el propio logger falla. Es un detalle de diseño excelente:
un fallo en la telemetría no puede ocultar el fallo de negocio.

```csharp
// Uso interno típico (lo verás en ExecSelf/Bind/Map con Try*)
Action<MlErrorsDetails> notificar = err => _alertas.Enviar(err.ToErrorsDescription());

var resultado = notificar.TryToMlResultErrors<Pedido>(erroresOriginales,
                            ex => $"Además, falló la notificación: {ex.Message}");
```

También existen `TryToMlResultErrors<T>(this Action source, …)` y las versiones asíncronas
`TryToMlResultErrorsAsync` con `Func<MlErrorsDetails, Task>` y `Func<Task>`.

---

## Grupo 5: cambio de tipo del carril

### `ToMlResultFail<T, TReturn>` — cambiar el tipo conservando los errores

```csharp
public static MlResult<TReturn> ToMlResultFail<T, TReturn>(this MlResult<T> source)
    => source.Match(
           fail : errorDetails => errorDetails,
           valid: _           => MlResult<TReturn>.Fail("Don't change MlResult Fail of valid source.")
       );
```

⚠️ **Atención al comportamiento con un resultado válido:** si `source` es válido, **no
lanza ni lo convierte**: devuelve un fallo con el mensaje literal
`"Don't change MlResult Fail of valid source."`. Es un error de programación disfrazado de
fallo de negocio.

```csharp
// ✅ Uso correcto: propagar errores cambiando el tipo del carril
if (!validacion.IsValid)
    return validacion.ToMlResultFail<Cliente, PedidoDto>();

// ❌ Si validacion es válido, obtienes un fallo con un mensaje en inglés que no dice nada
//    al usuario. Comprueba SIEMPRE antes.
```

💡 En la práctica, dentro de una tubería es más idiomático usar `Match` o
`ErrorsDetails.ToMlResultFail<TReturn>()`, que no tienen esta trampa.

---

## Grupo 6: `object` y reflexión

Este grupo existe para escenarios de infraestructura (serialización, middleware genérico) en
los que el tipo concreto no se conoce en compilación.

### `ToMlResultObject` / `FromMlResultObject`

```csharp
// MlResult<T> → MlResult<object>, conservando los errores
public static MlResult<object> ToMlResultObject<T>(this MlResult<T> source)

// T → MlResult<object> (siempre válido)
public static MlResult<object> ToMlResultObject<T>(this T source) => ((object)source!).ToMlResultValid();

// MlResult<object> → MlResult<T>, con comprobación de tipo
public static MlResult<T> FromMlResultObject<T>(this MlResult<object> source)
    => source.Match(
           fail : errorDetails => errorDetails.ToMlResultFail<T>(),
           valid: value        => (value is T tValue)
                                     ? tValue.ToMlResultValid()
                                     : MlResult<T>.Fail($"The value '{value}' of type '{value?.GetType()}' cannot be cast to the requested type '{typeof(T)}'.")
       );
```

🔑 `FromMlResultObject` es **seguro**: si el tipo no coincide, devuelve un fallo con un
mensaje descriptivo en lugar de lanzar `InvalidCastException`. Es la forma correcta de
volver del mundo `object` al mundo tipado.

```csharp
MlResult<object> generico = ObtenerDesdeCache(clave);

MlResult<Pedido> tipado = generico.FromMlResultObject<Pedido>();
// Si en la caché había otra cosa → Fail describiendo el tipo real y el esperado
```

⚠️ Las dos sobrecargas de `ToMlResultObject` (una desde `MlResult<T>`, otra desde `T`)
pueden generar ambigüedad si el receptor es un `MlResult<T>` tratado como `T`. En caso de
duda, tipa explícitamente la variable.

### `SecureGetValueFromMlResultBoxed`

```csharp
public static object SecureGetValueFromMlResultBoxed(this object source)
```

Extrae por **reflexión** la propiedad `Value` de un `MlResult<T>` metido en una variable
`object`. Comprueba paso a paso que exista `IsValid` de tipo `bool`, que sea `true` y que
exista `Value`.

⚠️ **Lanza `ArgumentException`** si algo falla. Es el único método de la librería que rompe
la promesa de no lanzar:

```csharp
var result = partialResult.IsValid
                 ? partialResult.Value
                 : throw new ArgumentException(partialResult.ErrorsDetails.ToString());
```

💡 **Uso muy restringido**: solo para infraestructura genérica (por ejemplo, un filtro de
ASP.NET Core que recibe el resultado de una acción como `object`). En código de aplicación no
debería aparecer nunca.

---

## `BuildErrorMessage` y el mensaje por defecto

```csharp
public static string BuildErrorMessage(string errorMessage, Exception ex)
    => string.IsNullOrWhiteSpace(errorMessage) ? DEFAULT_EX_ERROR_MESSAGE(ex) : errorMessage;

public static string BuildErrorMessage(Func<Exception, string> messageBuilder, Exception ex)
    => messageBuilder != null ? messageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex);
```

Ambos son **públicos** y determinan el mensaje de todos los métodos `Try*`:

- Si pasas un mensaje o un constructor → se usa el tuyo.
- Si pasas `null` o cadena vacía → se usa `DEFAULT_EX_ERROR_MESSAGE(ex)`.

🔑 **Consecuencia práctica:** si no indicas mensaje, el error que verá el usuario final será
el mensaje técnico de la excepción. **Pasa siempre un `errorMessageBuilder`** en código de
producción.

```csharp
// ⚠️ Sin mensaje: el usuario ve el texto de la excepción (posible fuga de información)
var r = parsear.TryToMlResult(entrada);

// ✅ Con mensaje de dominio: la excepción sigue en Details para el log
var r = parsear.TryToMlResult(entrada, ex => "El formato del archivo no es válido");
```

---

## ⚠️ Particularidades reales del código fuente

**1. `ToMlResultValid` no comprueba `null`.** Un `null` produce un resultado **válido** que
contiene `null`. Usa [`NullToFailed`](../Several/2_NullToFailed.md) si quieres rechazarlo.

**2. Los genéricos de `ToMlResultFail<T>` son obligatorios**: no se pueden inferir.

**3. Las 4 sobrecargas de tupla de `ToMlResultFail` pasan desapercibidas** y son la forma
más concisa de adjuntar `Details`.

**4. `ToMlResultFail<T, TReturn>` devuelve un fallo con mensaje en inglés
(`"Don't change MlResult Fail of valid source."`) si el origen es válido.** Comprueba
`IsValid` antes de llamarlo.

**5. La excepción capturada se guarda en `Details[EX_DESC_KEY]`.** Recupérala con
`GetDetailException()`, nunca accediendo a `Details` a mano.

**6. `TryToMlResultErrors` siempre devuelve `Fail`** y **nunca pierde los errores
originales**, incluso si la acción lanza (los amplía con `AppendExErrorDetail`).

**7. Los despachos internos usan `switch` sobre `object` con reflexión de tipos de
delegado.** Si el tipo no encaja en ninguna rama, se lanza
`ArgumentException($"The type {source.GetType()} is not a valid type")`. Con las sobrecargas
públicas esto no puede ocurrir, pero explica los mensajes si algún día lo ves.

**8. `SecureGetValueFromMlResultBoxed` lanza `ArgumentException`.** Es la única excepción
que la librería propaga a propósito.

**9. Hay código comentado y duplicidades cosméticas** (`ToMlTaskResult` es idéntico a
`ToMlResultAsync`; varios métodos tienen cuerpo con bloque en lugar de expresión). No afecta
al comportamiento.

**10. `ToMlTaskResult` existe pero es redundante:**

```csharp
public static async Task<MlResult<TReturn>> ToMlTaskResult<T, TReturn>(this Func<T, Task<TReturn>> sourceAsync, T value)
```
Hace exactamente lo mismo que `ToMlResultAsync`. Prefiere el segundo por coherencia de
nombres.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Meter un valor en el carril | `valor.ToMlResultValid()` |
| Crear un fallo desde un texto | `"...".ToMlResultFail<T>()` |
| Crear un fallo con varios mensajes | `new[] { "...", "..." }.ToMlResultFail<T>()` |
| Crear un fallo con diagnóstico | `("...", detalles).ToMlResultFail<T>()` |
| Envolver código que lanza | `func.TryToMlResult(valor, ex => "...")` |
| Envolver un efecto lateral que lanza | `accion.TryToMlResult(valor, ex => "...")` |
| Ejecutar algo en la rama de fallo sin perder errores | `accion.TryToMlResultErrors<T>(errores, ...)` |
| Propagar errores cambiando el tipo | `errorsDetails.ToMlResultFail<TReturn>()` |
| Pasar al mundo `object` | `.ToMlResultObject()` |
| Volver del mundo `object` con seguridad | `.FromMlResultObject<T>()` |
| Rechazar un `null` | [`NullToFailed`](../Several/2_NullToFailed.md) |
| Recuperar la excepción de un fallo | `GetDetailException()` |

---

## Ejemplos Prácticos

### Ejemplo 1: envolver una biblioteca legada que lanza

```csharp
public class ImportadorService
{
    public MlResult<IEnumerable<Pedido>> Importar(string ruta)
    {
        Func<string, string> leer = File.ReadAllText;

        return leer.TryToMlResult(ruta, ex => ex switch
                   {
                       FileNotFoundException     => $"El archivo '{ruta}' no existe",
                       UnauthorizedAccessException => $"Sin permisos para leer '{ruta}'",
                       _                         => $"No se pudo leer el archivo '{ruta}'"
                   })
                   .Bind(contenido =>
                   {
                       Func<string, List<Pedido>> deserializar =
                           txt => JsonSerializer.Deserialize<List<Pedido>>(txt)!;

                       return deserializar.TryToMlResult(contenido,
                                  ex => "El contenido del archivo no tiene el formato esperado");
                   })
                   .Bind(lista => lista.EmptyToFailed("El archivo no contiene pedidos")!)
                   .ExecSelfIfFailWithException(ex => _log.LogError(ex, "Importación fallida"));
    }
}
```

Fíjate en el patrón: cada `TryToMlResult` aporta un mensaje **de dominio**, mientras la
excepción técnica queda en `Details` para el log.

### Ejemplo 2: fábricas de error con detalles

```csharp
public static class ErroresPedido
{
    public static MlResult<T> NoEncontrado<T>(int id)
        => ($"El pedido {id} no existe", new Dictionary<string, object>
           {
               ["PedidoId"]     = id,
               ["NoEncontrado"] = true,
               ["Regla"]        = "PED-404"
           }).ToMlResultFail<T>();

    public static MlResult<T> EstadoInvalido<T>(int id, string estadoActual, string esperado)
        => ($"El pedido {id} está en estado '{estadoActual}' y se esperaba '{esperado}'",
            new Dictionary<string, object>
            {
                ["PedidoId"]     = id,
                ["EstadoActual"] = estadoActual,
                ["Regla"]        = "PED-409"
            }).ToMlResultFail<T>();

    public static MlResult<T> ValidacionMultiple<T>(IEnumerable<string> mensajes)
        => mensajes.ToMlResultFail<T>();
}

// Uso
public MlResult<Pedido> Obtener(int id)
    => _repo.Buscar(id) is { } p
           ? p.ToMlResultValid()
           : ErroresPedido.NoEncontrado<Pedido>(id);
```

### Ejemplo 3: efecto lateral seguro con `Action`

```csharp
public MlResult<Documento> Publicar(Documento doc)
{
    Action<Documento> subir = d => _almacen.Subir(d.Ruta, d.Contenido);

    return subir.TryToMlResult(doc, ex => ex is IOException
                                              ? "Error de red al subir el documento"
                                              : "No se pudo publicar el documento")
                .Map(d => d with { Publicado = true, FechaPublicacion = DateTime.UtcNow })
                .ExecSelfIfFailWithException(ex => _log.LogWarning(ex, "Publicación fallida de {Id}", doc.Id));
}
```

### Ejemplo 4: middleware genérico con `object`

```csharp
// Un filtro de ASP.NET Core que no conoce el tipo concreto del resultado
public class MlResultFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult { Value: { } valor }) return;

        // Solo si de verdad es un MlResult<T>; si no, se deja pasar
        try
        {
            var interno = valor.SecureGetValueFromMlResultBoxed();
            context.Result = new OkObjectResult(interno);
        }
        catch (ArgumentException)
        {
            // No era un MlResult válido: no tocamos la respuesta
        }
    }
}
```

⚠️ Este es el único escenario legítimo de `SecureGetValueFromMlResultBoxed`, y obsérvese que
requiere `try/catch` porque **sí lanza**.

### Ejemplo 5: qué no hacer

```csharp
// ❌ Sin mensaje: el usuario final ve el texto técnico de la excepción
var r = parsear.TryToMlResult(entrada);

// ✅ Mensaje de dominio; la excepción sigue en Details
var r = parsear.TryToMlResult(entrada, ex => "El formato de la entrada no es válido");


// ❌ Suponer que ToMlResultValid rechaza el null
var r = clienteNulo.ToMlResultValid();      // Valid(null)

// ✅
var r = clienteNulo.NullToFailed("El cliente es obligatorio");


// ❌ ToMlResultFail<T, TReturn> sobre un resultado válido
var r = validacionValida.ToMlResultFail<Cliente, PedidoDto>();
//      → Fail("Don't change MlResult Fail of valid source.")  ¡mensaje inútil!

// ✅ Comprueba antes, o usa Match
var r = validacion.Match(valid: c   => Procesar(c),
                         fail : err => err.ToMlResultFail<PedidoDto>());


// ❌ Leer la excepción accediendo a Details a mano
var ex = (Exception)resultado.ErrorsDetails.Details["Ex"];   // la clave real es EX_DESC_KEY = "Ex"

// ✅ Usa el accesor oficial
resultado.ExecSelfIfFailWithException(ex => _log.LogError(ex, "…"));


// ❌ SecureGetValueFromMlResultBoxed en código de aplicación
var valor = (Pedido)resultado.SecureGetValueFromMlResultBoxed();

// ✅ Usa Match, que es tipado y no lanza
var dto = resultado.Match(valid: p => p.ToDto(), fail: _ => null);
```

---

## Mejores Prácticas

1. **Pasa siempre un `errorMessageBuilder`** a los métodos `Try*`: si no, el usuario verá el
   mensaje técnico de la excepción.
2. **Aprovecha las sobrecargas de tupla** de `ToMlResultFail` para adjuntar `Details` sin
   ceremonia.
3. **Centraliza las fábricas de error** en una clase estática por agregado
   (`ErroresPedido.NoEncontrado<T>(id)`): mensajes consistentes y códigos de regla en un
   único sitio.
4. **Recuerda que `ToMlResultValid` no filtra `null`.**
5. **Comprueba `IsValid` antes de usar `ToMlResultFail<T, TReturn>`**, o prefiere `Match`.
6. **Recupera las excepciones con los operadores `*WithException`**, no accediendo a
   `Details` directamente.
7. **`FromMlResultObject<T>` en lugar de castear**: devuelve un fallo descriptivo si el tipo
   no coincide, en vez de lanzar.
8. **Restringe `SecureGetValueFromMlResultBoxed` a infraestructura** y envuélvelo en
   `try/catch`: es el único método de la librería que lanza.
9. **Prefiere `ToMlResultAsync` a `ToMlTaskResult`**: son equivalentes, pero el primero sigue
   la convención de nombres.
10. **No dupliques la maquinaria `Try*`**: si necesitas capturar excepciones dentro del
    carril, usa directamente `TryMap`, `TryBind` o `TryExecSelf`, que ya la usan por dentro.

---

## Resumen

- `MlResultTransformations` reúne las **fábricas y conversiones** del `MlResult`: es la base
  sobre la que se construyen todos los demás operadores.
- **`ToMlResultValid<T>`** mete un valor en el carril aprovechando la conversión implícita.
  ⚠️ **No comprueba `null`.**
- **`ToMlResultFail<T>`** tiene **14 sobrecargas** (`MlErrorsDetails`, `MlError`, `string`,
  listas, arrays, `IEnumerable` y **4 formas de tupla con `Details`**), todas con gemela
  `*Async`. El genérico `<T>` es obligatorio.
- **`TryToMlResult`** envuelve delegados y acciones que lanzan; guarda la excepción en
  `Details[EX_DESC_KEY]` y construye el mensaje con `BuildErrorMessage`.
- **`TryToMlResultErrors`** siempre devuelve `Fail` y **nunca pierde los errores
  originales**: si la acción lanza, los amplía con `AppendExErrorDetail`.
- ⚠️ **`ToMlResultFail<T, TReturn>`** devuelve un fallo con el mensaje
  `"Don't change MlResult Fail of valid source."` si el origen es válido.
- **`ToMlResultObject` / `FromMlResultObject`** permiten ir y volver del mundo `object`; el
  segundo comprueba el tipo y falla de forma descriptiva en lugar de lanzar.
- ⚠️ **`SecureGetValueFromMlResultBoxed` lanza `ArgumentException`**: es el único método de
  la librería que lo hace. Solo para infraestructura.
- **`BuildErrorMessage` es público** y decide el mensaje de todos los `Try*`; sin
  `errorMessageBuilder` se usa el texto de la excepción.

---

## Ver también

- [`MlResult`](../Types/MlResult.md) — el tipo central y sus conversiones implícitas
- [`MlResultErrors`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails`, `AppendExErrorDetail`
- [`Extensions`](../Extensions/Extensions.md) — `ToAsync` y otras utilidades transversales
- [`NullToFailed`](../Several/2_NullToFailed.md) — rechazar valores nulos al entrar
- [`TryMap`](../Map/1_Map.md) y [`TryBind`](../Bind/3_Bind.md) — captura de excepciones dentro del carril
- [`MapIfFailWithException`](../Map/6_MapIfFailWithException.md) — recuperar la excepción de `Details`
- [`ExecSelf`](../ExecSelf/1_ExecSelf.md) — efectos laterales seguros
- [`Match`](../Match/1_Match.md) — salir del carril de forma tipada