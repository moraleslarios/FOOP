# `EnsureFp` — Mensajes automáticos y claves de detalle

> Archivos fuente: `Helpers/EnsureFpMessages.cs` y `Helpers/Constants.cs`.

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [1. `EnsureFpMessages`: un único lugar para el texto](#1-ensurefpmessages-un-único-lugar-para-el-texto)
- [2. El patrón `Rule` y los helpers de formato](#2-el-patrón-rule-y-los-helpers-de-formato)
- [3. Catálogo completo de mensajes](#3-catálogo-completo-de-mensajes)
- [4. Claves de detalle](#4-claves-de-detalle)
- [5. Cómo leer los detalles de un fallo](#5-cómo-leer-los-detalles-de-un-fallo)
- [6. Uso desde un controlador web](#6-uso-desde-un-controlador-web)
- [7. Cómo se construyen los fallos internamente](#7-cómo-se-construyen-los-fallos-internamente)
- [8. Mejores prácticas](#8-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

Las variantes `*Arg` de `EnsureFp` generan el mensaje de error por ti. Ese texto **no está esparcido
por los ocho ficheros de reglas**: vive centralizado en la clase interna `EnsureFpMessages`.

Esa decisión tiene tres consecuencias prácticas:

1. **Consistencia**: todos los mensajes siguen la misma plantilla, así que la salida de la API es
   homogénea sin necesidad de revisar cada regla.
2. **Mantenibilidad**: cambiar la redacción de una familia completa es editar un fichero.
3. **Testabilidad**: los tests unitarios comprueban el texto en un solo punto.

```csharp
internal static class EnsureFpMessages   // ⚠️ internal: no forma parte de la API pública
```

Al ser `internal`, **no puedes invocarla desde tu código**. Se documenta aquí para que sepas
exactamente qué mensaje esperar en cada fallo y puedas escribir tests o traducciones sobre él.

---

## 1. `EnsureFpMessages`: un único lugar para el texto

Los miembros base:

```csharp
public const string DEFAULT_PARAM_NAME = "value";

public static string SafeName(string? paramName);   // paramName ?? DEFAULT_PARAM_NAME

public static string NotNull(string? paramName);
public static string NotEmpty(string? paramName);
public static string NotNullEmptyOrWhitespace(string? paramName);
public static string NotValid(string? paramName);
public static string PredicateException(string? paramName);
```

`SafeName` garantiza que **nunca aparece un nombre vacío** en un mensaje: si el
`[CallerArgumentExpression]` no pudo capturar la expresión (por ejemplo en una llamada por
reflexión), se usa el literal `"value"`.

| Método | Texto producido (con `paramName = "pedido"`) |
|---|---|
| `NotNull` | `'pedido' no puede ser null.` |
| `NotEmpty` | `'pedido' no puede estar vacío.` |
| `NotNullEmptyOrWhitespace` | `'pedido' no puede ser null, vacío ni contener solo espacios.` |
| `NotValid` | `'pedido' no cumple la condición requerida.` |
| `PredicateException` | `Se produjo una excepción al evaluar la condición sobre 'pedido'.` |

---

## 2. El patrón `Rule` y los helpers de formato

Todos los mensajes especializados se construyen con el mismo molde:

```csharp
public static string Rule(string? paramName, string requirement)
    => $"'{SafeName(paramName)}' {requirement}.";
```

De ahí que **todos** los mensajes tengan la misma forma: `'nombre' + requisito + punto`. Es lo que
hace que la salida sea predecible y fácil de parsear o traducir.

Dos helpers privados completan el formato:

| Helper | Función |
|---|---|
| `Actual(int?)` | añade el sufijo ` (actual: N)` cuando hay un recuento o longitud real que mostrar |
| `Render(object?)` | representa un valor en el mensaje de forma segura, incluido `null` |

El resultado es que los mensajes no se limitan a decir qué se esperaba: **dicen también qué se
recibió**.

```
'nombre' debe tener como máximo 10 caracteres (actual: 27).
'lineas' debe contener al menos 1 elemento (actual: 0).
'edad' debe estar entre 18 y 120 (actual: 15).
```

Ese «actual» es la diferencia entre un mensaje que obliga a depurar y uno que resuelve la incidencia
en la primera lectura.

---

## 3. Catálogo completo de mensajes

### 3.1. Cadenas de texto

Corresponden a las reglas de [3. Cadenas de texto](./3_EnsureFpStrings.md).

| Miembro | Requisito expresado |
|---|---|
| `MaxLength` | debe tener como máximo *N* caracteres (actual: *M*) |
| `MinLength` | debe tener al menos *N* caracteres (actual: *M*) |
| `LengthBetween` | debe tener entre *min* y *max* caracteres (actual: *M*) |
| `LengthExactly` | debe tener exactamente *N* caracteres (actual: *M*) |
| `Matches` | debe cumplir el patrón *patrón* |
| `NotMatches` | no debe cumplir el patrón *patrón* |
| `StartsWith` | debe comenzar por *prefijo* |
| `EndsWith` | debe terminar por *sufijo* |
| `Contains` | debe contener *subcadena* |
| `NotContains` | no debe contener *subcadena* |
| `IsOneOf` | debe ser uno de los valores permitidos |

### 3.2. Números y comparables

Corresponden a [4. Números y rangos](./4_EnsureFpNumbers.md).

| Miembro | Requisito expresado |
|---|---|
| `GreaterThan` | debe ser mayor que *límite* |
| `GreaterOrEqual` | debe ser mayor o igual que *límite* |
| `LessThan` | debe ser menor que *límite* |
| `LessOrEqual` | debe ser menor o igual que *límite* |
| `InRange` | debe estar entre *min* y *max* |
| `OutOfRange` | no debe estar entre *min* y *max* |
| `Positive` | debe ser positivo |
| `NotNegative` | no puede ser negativo |
| `Negative` | debe ser negativo |
| `NotZero` | no puede ser cero |

### 3.3. Colecciones

Corresponden a [5. Colecciones](./5_EnsureFpCollections.md).

| Miembro | Requisito expresado |
|---|---|
| `CountExactly` | debe contener exactamente *N* elementos (actual: *M*) |
| `CountAtLeast` | debe contener al menos *N* elementos (actual: *M*) |
| `CountAtMost` | debe contener como máximo *N* elementos (actual: *M*) |
| `CountBetween` | debe contener entre *min* y *max* elementos (actual: *M*) |
| `AllMatch` | todos sus elementos deben cumplir la condición |
| `NoneMatch` | ninguno de sus elementos debe cumplir la condición |
| `AnyMatch` | al menos uno de sus elementos debe cumplir la condición |
| `NoDuplicates` | no puede contener elementos duplicados |
| `NoNullItems` | no puede contener elementos null |
| `ContainsItem` | debe contener el elemento *valor* |

### 3.4. Tipos concretos

Corresponden a [6. Tipos concretos](./6_EnsureFpTypes.md).

| Miembro | Requisito expresado |
|---|---|
| `NotEmptyGuid` | no puede ser `Guid.Empty` |
| `IsDefinedEnum(paramName, Type, object?)` | no es un valor definido de *TipoEnum* (actual: *valor*) |
| `InFuture` | debe ser una fecha futura (actual: *fecha*) |
| `InPast` | debe ser una fecha pasada (actual: *fecha*) |
| `NotDefault` | no puede ser el valor por defecto de su tipo |
| `IsAbsoluteUri` | debe ser una URI absoluta |
| `IsValidUri` | no es una URI válida |
| `IsValidEmail` | no es una dirección de correo válida |
| `FileExists` | no corresponde a un fichero existente |
| `DirectoryExists` | no corresponde a un directorio existente |

`IsDefinedEnum` es el único que recibe el `Type`: el mensaje incluye el **nombre del enumerado**, lo
que resulta decisivo cuando el valor llega de un JSON externo.

---

## 4. Claves de detalle

Además del mensaje, los fallos de `EnsureFp` adjuntan información estructurada en el diccionario
`Details` del `MlErrorsDetails`. Las claves están declaradas como constantes en
`Helpers/Constants.cs`, que es un `global using static` del proyecto: **puedes usarlas directamente
por su nombre**.

```csharp
public const string EX_DESC_KEY        = "Ex";
public const string VALUE_KEY          = "Value";
public const string NOT_FOUND_KEY      = "NotFound";
public const string PARAM_NAME_KEY     = "ParamName";
public const string FAILED_INDEXES_KEY = "FailedIndexes";
public const string EXPECTED_KEY       = "Expected";
```

| Constante | Clave | Contenido | La añaden |
|---|---|---|---|
| `PARAM_NAME_KEY` | `ParamName` | nombre o expresión del parámetro que falló | todas las variantes `*Arg` |
| `VALUE_KEY` | `Value` | el valor recibido | todas las variantes `*Arg` |
| `EXPECTED_KEY` | `Expected` | el límite, rango o conjunto esperado | reglas de números, longitudes y cardinalidad |
| `FAILED_INDEXES_KEY` | `FailedIndexes` | `IEnumerable<int>` con las posiciones que incumplen | `AllMatch`, `NoneMatch`, `NoDuplicates`, `NoNullItems` |
| `EX_DESC_KEY` | `Ex` | la excepción capturada | `TryThat` y `TryThatAsync` |
| `NOT_FOUND_KEY` | `NotFound` | marca semántica de «recurso inexistente» | usada por la capa web, no por `EnsureFp` |

Otras constantes útiles del mismo fichero:

```csharp
public const  string DEFAULT_ERROR_MESSAGE = …;
public static string DEFAULT_EX_ERROR_MESSAGE(Exception ex) => …;
```

---

## 5. Cómo leer los detalles de un fallo

Un aviso importante: **`MlErrorsDetails` solo expone dos propiedades públicas**, `Errors` y
`Details`. No hay una propiedad `Message`. Para leer la información se usan los métodos de extensión
de `MlErrorsDetailsActions`:

| Método | Devuelve |
|---|---|
| `ToErrorsMessages()` | todos los mensajes concatenados |
| `ToErrorsDescription()` | descripción legible del conjunto de errores |
| `Errors.First().Message` | el primer mensaje, si solo esperas uno |
| `GetDetailValue<T>()` | el valor guardado en `Details`, tipado |
| `GetDetailException()` | la excepción guardada bajo la clave `Ex` |
| `ToDetailsDescription()` | volcado legible de todo el diccionario de detalles |

```csharp
var resultado = MaxLengthArg(nombre, 10);

if (resultado.IsFail)
{
    var d = resultado.SecureFailErrorsDetails();

    var mensaje  = d.ToErrorsMessages();
    // "'nombre' debe tener como máximo 10 caracteres (actual: 27)."

    var param    = d.Details[PARAM_NAME_KEY];      // "nombre"
    var valor    = d.Details[VALUE_KEY];           // "Un nombre demasiado largo..."
    var esperado = d.Details[EXPECTED_KEY];        // 10

    var volcado  = d.ToDetailsDescription();       // todo el diccionario, para el log
}
```

Acceso seguro cuando no sabes si la clave existe:

```csharp
if (d.Details.TryGetValue(FAILED_INDEXES_KEY, out var raw) && raw is IEnumerable<int> indices)
    logger.LogWarning("Posiciones inválidas: {Indices}", string.Join(", ", indices));
```

---

## 6. Uso desde un controlador web

Las claves de detalle permiten **decidir el código HTTP sin analizar el texto del mensaje**:

```csharp
[HttpPost]
public async Task<IActionResult> Crear(CrearPedidoDto dto, CancellationToken ct) =>
    (await servicio.CrearAsync(dto, ct))
        .Match(
            valid: p => CreatedAtAction(nameof(Obtener), new { id = p.Id }, p),
            fail:  e => Responder(e));

private IActionResult Responder(MlErrorsDetails e)
{
    // 1. Fallo técnico: el predicado lanzó una excepción → 503
    if (e.Details.ContainsKey(EX_DESC_KEY))
    {
        logger.LogError(e.GetDetailException(), "Fallo técnico validando la petición");
        return StatusCode(StatusCodes.Status503ServiceUnavailable,
                          "Servicio temporalmente no disponible.");
    }

    // 2. Recurso inexistente → 404
    if (e.Details.ContainsKey(NOT_FOUND_KEY))
        return NotFound(e.ToErrorsMessages());

    // 3. Validación de entrada → 400 con el nombre del campo
    var problema = new ValidationProblemDetails
    {
        Title  = "La petición contiene datos no válidos.",
        Detail = e.ToErrorsMessages()
    };

    if (e.Details.TryGetValue(PARAM_NAME_KEY, out var campo) && campo is string nombre)
        problema.Errors[nombre] = e.Errors.Select(x => x.Message).ToArray();

    return BadRequest(problema);
}
```

Y con las reglas de colección, el detalle `FailedIndexes` permite señalar **filas concretas**:

```csharp
fail: e =>
{
    var indices = e.GetDetailValue<IEnumerable<int>>();
    var filas   = indices is null ? "" : string.Join(", ", indices.Select(i => i + 1));
    return BadRequest($"{e.ToErrorsMessages()} Filas afectadas: {filas}");
}
```

> En los proyectos `MoralesLarios.OOFP.WebApi` y `MoralesLarios.OOFP.WebServices` esta traducción ya
> está resuelta mediante `MlProblemsDetails` y la clave compartida
> `MoralesLarios.OOFP.Shared.Web.WebErrorDetailsKeys.ProblemsDetails`.

---

## 7. Cómo se construyen los fallos internamente

Tres helpers privados de `EnsureFp.Core.cs` son los responsables de que todos los fallos tengan la
misma forma. Conocerlos ayuda a interpretar el contenido de `Details`:

| Helper | Detalles que añade |
|---|---|
| `BuildGuard<T>(value, condition, errorMessage, paramName)` | `ParamName` + `Value` |
| `BuildRule<T>(value, condition, errorMessage, paramName, params (string Key, object Value)[] extraDetails)` | `ParamName` + `Value` + los pares extra (`Expected`, `FailedIndexes`…) |
| `BuildExceptionFail<T>(paramName, ex, errorMessageBuilder)` | `ParamName` + los detalles de la excepción vía `AppendExDetailsToMlDetails(ex)` |

`BuildRule` es el que usan todas las familias nuevas (cadenas, números, colecciones, tipos): de ahí
que sus fallos siempre lleven `ParamName`, `Value` y, cuando tiene sentido, `Expected`.

Los helpers de construcción de mensaje son:

```csharp
BuildMessage(Func<string> messageBuilder);                       // mensaje perezoso
BuildMessage<T>(T value, Func<T, string> messageBuilder);        // mensaje a partir del valor
BuildMessage(Func<Exception, string> builder, string? paramName); // mensaje a partir de la excepción
```

Los tres toleran un constructor `null` y recurren al texto por defecto de `EnsureFpMessages`, de modo
que **nunca se produce una `NullReferenceException` al generar un mensaje**.

---

## 8. Mejores prácticas

1. **Prefiere las variantes `*Arg`** cuando el mensaje automático sea suficiente: es texto que no hay
   que escribir, mantener ni traducir dos veces.
2. **Escribe mensajes propios cuando el destinatario es el usuario final.** `'dto.Email' no es una
   dirección de correo válida.` es perfecto para un log y mejorable para un formulario.
3. **Nunca analices el texto del mensaje para tomar decisiones.** Usa las claves de `Details`
   (`ParamName`, `Expected`, `Ex`, `FailedIndexes`).
4. **Usa las constantes, no los literales.** `Details[PARAM_NAME_KEY]`, no `Details["ParamName"]`.
5. **Registra `ToDetailsDescription()` en el log** y devuelve `ToErrorsMessages()` al cliente: todo el
   contexto queda en el servidor y el cliente recibe solo lo necesario.
6. **Rescata la excepción con `GetDetailException()`** en los fallos de `TryThat`/`TryThatAsync` para
   no perder la traza.
7. **Si necesitas internacionalización**, no toques `EnsureFpMessages`: usa las sobrecargas con
   `string` o `MlErrorsDetails` y pasa el texto ya traducido.
8. **Comprueba `ParamName` en los tests** además del mensaje: es más estable frente a cambios de
   redacción.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [3. Cadenas de texto](./3_EnsureFpStrings.md)
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [5. Colecciones](./5_EnsureFpCollections.md)
- [6. Tipos concretos](./6_EnsureFpTypes.md)
- [8. Variantes asíncronas](./8_EnsureFpAsync.md)
- [`MlResultErrors`: anatomía de `MlErrorsDetails`](../Types/MlResultErrors.md)
- [`MlResultActionsErrorsDetails`: métodos sobre los detalles](../Types/MlResultActionsErrorsDetails.md)
