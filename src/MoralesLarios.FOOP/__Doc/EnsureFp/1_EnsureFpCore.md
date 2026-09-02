# `EnsureFp` — Núcleo: `That`, `TryThat` y guardas `*Arg`

> Archivo fuente: `Helpers/EnsureFp.Core.cs` (más las guardas originales de `Helpers/EnsureFp.cs`).

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [La convención de las tres variantes](#la-convención-de-las-tres-variantes)
- [1. `That` con mensajes perezosos](#1-that-con-mensajes-perezosos)
- [2. `That` con predicados perezosos](#2-that-con-predicados-perezosos)
- [3. `TryThat`: predicados que pueden lanzar](#3-trythat-predicados-que-pueden-lanzar)
- [4. Guardas con mensaje automático (`*Arg`)](#4-guardas-con-mensaje-automático-arg)
- [5. Guardas clásicas heredadas](#5-guardas-clásicas-heredadas)
- [6. Semántica defensiva y helpers privados](#6-semántica-defensiva-y-helpers-privados)
- [7. Claves de detalle que se rellenan](#7-claves-de-detalle-que-se-rellenan)
- [8. Ejemplos completos](#8-ejemplos-completos)
- [9. Mejores prácticas](#9-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

El bloque `Core` es el corazón de `EnsureFp`. Contiene la primitiva `That` en todas sus formas
y los tres mecanismos transversales que el resto de familias reutiliza:

1. **Mensajes perezosos** (`Func<string>`, `Func<MlErrorsDetails>`, `Func<T,string>`): el mensaje
   se construye **solo si la validación falla**, evitando interpolaciones costosas en el camino feliz.
2. **Predicados perezosos** (`Func<T,bool>`): la condición se evalúa dentro del método, lo que
   permite componer sin evaluar de antemano y sin riesgo de `NullReferenceException`.
3. **`TryThat`**: si el predicado lanza una excepción, en lugar de propagarla se convierte en un
   `MlResult` fallido con la excepción guardada en `Details`.
4. **Guardas `*Arg`**: usan `[CallerArgumentExpression]` para generar el mensaje automáticamente
   a partir del nombre del argumento, eliminando el mensaje repetido en cada línea.

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;   // recomendado: elimina el prefijo EnsureFp.

var result = NotNullArg(cliente)                                   // "'cliente' no puede ser nulo."
    .Bind(c => NotNullEmptyOrWhitespaceArg(c.Email))               // "'c.Email' ..."
    .Bind(_ => ThatArg(cliente, c => c.Edad >= 18));               // "'cliente' no cumple la condición."
```

---

## La convención de las tres variantes

Prácticamente **toda** regla de `EnsureFp` existe en tres formas. Es la convención más
importante para orientarse en la API:

| Variante | Firma del error | Cuándo usarla |
|---|---|---|
| `Regla(valor, string mensaje)` | mensaje literal | quieres un texto de negocio concreto |
| `Regla(valor, MlErrorsDetails error)` | error enriquecido | necesitas código de error, `Details` propios, varios errores |
| `ReglaArg(valor)` | ninguno: se genera | validación de argumento; el nombre de la variable es suficiente |

Las variantes `*Arg` añaden además dos entradas al diccionario `Details` del error:
`ParamName` y `Value`, útiles para depurar y para construir respuestas HTTP.

---

## 1. `That` con mensajes perezosos

```csharp
public static MlResult<T> That<T>(T value, bool condition, Func<string> errorMessageBuilder);
public static MlResult<T> That<T>(T value, bool condition, Func<MlErrorsDetails> errorsDetailsBuilder);
```

El delegado **solo se invoca cuando `condition` es `false`**. Esto importa cuando el mensaje es caro:

```csharp
// ❌ La interpolación (y el ToString de la colección) se ejecuta SIEMPRE.
var r1 = That(pedido, pedido.Total > 0,
              $"El pedido {pedido.Referencia} con líneas [{string.Join(", ", pedido.Lineas)}] no es válido.");

// ✅ La interpolación se ejecuta SOLO si falla.
var r2 = That(pedido, pedido.Total > 0,
              () => $"El pedido {pedido.Referencia} con líneas [{string.Join(", ", pedido.Lineas)}] no es válido.");
```

Con `MlErrorsDetails` perezoso puedes construir un error completo (con detalles de diagnóstico)
solo en la rama de fallo:

```csharp
var result = That(factura, factura.Importe <= limite,
                  () => MlErrorsDetails.FromErrorMessage("Importe por encima del límite autorizado.")
                                       .AddDetail("Limite",  limite)
                                       .AddDetail("Importe", factura.Importe));
```

---

## 2. `That` con predicados perezosos

```csharp
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, string errorMessage);
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, MlErrorsDetails errorsDetails);
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<string> errorMessageBuilder);
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<MlErrorsDetails> errorsDetailsBuilder);
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<T, string> errorMessageBuilder);
public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<T, MlErrorsDetails> errorsDetailsBuilder);
```

La versión con predicado tiene tres ventajas sobre la versión con `bool`:

1. **Composición**: la regla se puede guardar en una variable y reutilizarse.
2. **Encadenamiento fluido**: dentro de un `Bind` el predicado recibe el valor sin necesidad de
   capturar variables externas.
3. **Mensaje dependiente del valor**: con `Func<T,string>` el mensaje se construye a partir del
   propio valor validado.

```csharp
// Mensaje construido a partir del valor validado.
var result = That(importe,
                  i => i > 0 && i <= saldo,
                  i => $"El importe {i:N2} € no es válido (saldo disponible: {saldo:N2} €).");
```

```csharp
// Regla reutilizable.
static readonly Func<Pedido, bool> EsFacturable = p => p.Lineas.Any() && p.Total > 0;

var r = That(pedido, EsFacturable, "El pedido no es facturable.");
```

> ⚠️ **Predicado `null` ⇒ fallo.** Un predicado nulo se trata como condición `false`, nunca como
> «validación superada». Esto es deliberado: es mejor un fallo visible que una guarda silenciada.

---

## 3. `TryThat`: predicados que pueden lanzar

```csharp
public static MlResult<T> TryThat<T>(T value,
                                     Func<T, bool> predicate,
                                     string errorMessage,
                                     [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<T> TryThat<T>(T value,
                                     Func<T, bool> predicate,
                                     MlErrorsDetails errorsDetails,
                                     [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<T> TryThat<T>(T value,
                                     Func<T, bool> predicate,
                                     Func<Exception, string> errorMessageBuilder,
                                     [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Es el equivalente, en el mundo de las guardas, de `TryBind` / `TryMap`. Si el predicado lanza,
el resultado es un fallo con la excepción anexada en `Details["Ex"]` (mediante
`AppendExDetailsToMlDetails`), de modo que la cadena nunca se rompe con un `throw`.

```csharp
// Un predicado que puede lanzar: parseo, acceso a un campo que puede no existir, regex sin timeout…
var result = TryThat(json,
                     j => JsonDocument.Parse(j).RootElement.GetProperty("id").GetInt32() > 0,
                     "El JSON no contiene un identificador válido.");

// Si Parse lanza:
//   result.IsFail == true
//   result.SecureFailErrorsDetails().GetDetailException()   → la JsonException original
```

La tercera sobrecarga permite construir el mensaje **a partir de la excepción**:

```csharp
var result = TryThat(ruta,
                     File.Exists,
                     ex => $"No se pudo comprobar la ruta: {ex.GetType().Name} – {ex.Message}");
```

> **Diferencia clave respecto a `That`:** `That` no captura excepciones. Si el predicado puede
> lanzar, usa `TryThat`; si no puede, usa `That` (más barato y más explícito).

---

## 4. Guardas con mensaje automático (`*Arg`)

```csharp
public static MlResult<T>      NotNullArg<T>(T value,                       [CallerArgumentExpression(nameof(value))] string? paramName = null);
public static MlResult<IEnumerable<T>> NotEmptyArg<T>(IEnumerable<T> value, [CallerArgumentExpression(nameof(value))] string? paramName = null);
public static MlResult<string> NotNullEmptyOrWhitespaceArg(string value,    [CallerArgumentExpression(nameof(value))] string? paramName = null);
public static MlResult<T>      ThatArg<T>(T value, bool condition,          [CallerArgumentExpression(nameof(value))] string? paramName = null);
public static MlResult<T>      ThatArg<T>(T value, Func<T, bool> predicate, [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

El compilador rellena `paramName` con **el texto literal de la expresión** que has pasado. No hay
que escribirlo ni mantenerlo: si renombras la variable, el mensaje se actualiza solo.

```csharp
public MlResult<Cliente> Crear(string nombre, string email, int edad) =>
    NotNullEmptyOrWhitespaceArg(nombre)              // "'nombre' no puede ser nulo, vacío ni contener solo espacios en blanco."
        .Bind(_ => NotNullEmptyOrWhitespaceArg(email))
        .Bind(_ => ThatArg(edad, edad >= 18))        // "'edad' no cumple la condición requerida."
        .Map(_ => new Cliente(nombre, email, edad));
```

Cuando `paramName` no se puede inferir (por ejemplo, si lo pasas explícitamente como `null`), se
usa el nombre por defecto `"value"`.

**Cuándo usar `*Arg` y cuándo no:**

| Situación | Recomendación |
|---|---|
| Validar argumentos de un método interno / de infraestructura | ✅ `*Arg` |
| Regla de negocio que el usuario final va a leer | ❌ usa la variante con `string` |
| API pública cuyos mensajes están traducidos | ❌ usa `MlErrorsDetails` |
| Prototipar rápido | ✅ `*Arg` |

---

## 5. Guardas clásicas heredadas

Siguen disponibles y sin cambios (`Helpers/EnsureFp.cs`):

```csharp
public static MlResult<T>              NotNull<T>(T value, string errorMessage);
public static MlResult<T>              NotNull<T>(T value, MlErrorsDetails errorsDetails);
public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, string errorMessage);
public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails);
public static MlResult<string>         NotNullEmptyOrWhitespace(string value, string errorMessage);
public static MlResult<string>         NotNullEmptyOrWhitespace(string value, MlErrorsDetails errorsDetails);
public static MlResult<T>              That<T>(T value, bool condition, string errorMessage);
public static MlResult<T>              That<T>(T value, bool condition, MlErrorsDetails errorsDetails);
```

> Para cadenas existe además `NotNullOrEmpty` (que **sí** acepta `"   "`) en el bloque de
> [strings](./3_EnsureFpStrings.md), y para colecciones existe `NotEmptyCollection<TCollection,T>`
> (que **preserva el tipo concreto**) en el bloque de [colecciones](./5_EnsureFpCollections.md).

---

## 6. Semántica defensiva y helpers privados

`EnsureFp` nunca lanza por su cuenta. Estas son las decisiones de diseño, todas verificadas por
pruebas unitarias:

| Entrada anómala | Comportamiento |
|---|---|
| `value` es `null` en una regla que necesita el valor | **fallo** (nunca `NullReferenceException`) |
| `predicate` es `null` | se evalúa como `false` ⇒ **fallo** |
| `errorMessage` es `null` o vacío | se usa el mensaje por defecto de `EnsureFpMessages` |
| Colección de validadores vacía o `null` (agregación) | **válido**: no hay nada que incumplir |
| `Task` de entrada `null` (async) | se resuelve a `default!` y la regla decide |

Helpers privados compartidos por todas las familias (no forman parte de la API pública, pero
conocerlos explica el comportamiento):

| Helper | Responsabilidad |
|---|---|
| `EvaluatePredicate<T>(value, predicate)` | `predicate is not null && predicate(value)` |
| `BuildMessage(Func<string>)` | Invoca el constructor de mensaje protegiéndose de `null` |
| `BuildMessage<T>(value, Func<T,string>)` | Igual, pasando el valor validado |
| `BuildMessage(Func<Exception,string>, paramName)` | Mensaje a partir de la excepción capturada |
| `BuildGuard<T>(value, condition, message, paramName)` | Guarda `*Arg`: añade `ParamName` y `Value` |
| `BuildExceptionFail<T>(paramName, ex, builder)` | Fallo de `TryThat`: añade `ParamName` y `Details["Ex"]` |
| `BuildRule<T>(value, condition, message, paramName, params (string Key, object Value)[] extra)` | Motor común de **todas** las reglas especializadas: permite añadir detalles como `Expected` o `FailedIndexes` |

---

## 7. Claves de detalle que se rellenan

Todas las constantes viven en `Helpers/Constants.cs` y están disponibles por `global using static`:

| Constante | Valor | Quién la rellena |
|---|---|---|
| `PARAM_NAME_KEY` | `"ParamName"` | todas las variantes `*Arg` y `TryThat` |
| `VALUE_KEY` | `"Value"` | las variantes `*Arg` |
| `EXPECTED_KEY` | `"Expected"` | reglas numéricas `*Arg` (límite/rango esperado) |
| `FAILED_INDEXES_KEY` | `"FailedIndexes"` | `AllMatch`, `NoneMatch`, `AnyMatch` |
| `EX_DESC_KEY` | `"Ex"` | `TryThat*` al capturar la excepción |

Lectura desde el consumidor:

```csharp
result.Match(
    valid: v  => Ok(v),
    fail:  e  => BadRequest(new
    {
        mensaje   = e.ToErrorsMessages(),
        parametro = e.GetDetailValue<string>(),      // consulta el diccionario Details
        detalle   = e.ToDetailsDescription()
    }));
```

> ⚠️ `MlErrorsDetails` **solo expone** `Errors` y `Details`. No existen `AllErrors`,
> `FirstErrorMessage` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()` o `ToDetailsDescription()`.

---

## 8. Ejemplos completos

### 8.1. Guardas de argumentos en un servicio de aplicación

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public MlResult<PedidoDto> Confirmar(int pedidoId, string usuario, decimal importe) =>
    ThatArg(pedidoId, pedidoId > 0)
        .Bind(_ => NotNullEmptyOrWhitespaceArg(usuario))
        .Bind(_ => ThatArg(importe, i => i > 0m))
        .Bind(_ => _repo.Get(pedidoId))
        .Bind(p  => That(p, p.Estado == Estado.Pendiente,
                         () => $"El pedido {p.Referencia} ya está en estado {p.Estado}."))
        .Map(p   => p.ToDto());
```

### 8.2. Predicado peligroso encapsulado

```csharp
public MlResult<string> ValidarPlantilla(string plantilla) =>
    NotNullEmptyOrWhitespaceArg(plantilla)
        .Bind(p => TryThat(p,
                           t => Regex.IsMatch(t, @"^\{\{[A-Za-z_]+\}\}$", RegexOptions.None, TimeSpan.FromSeconds(1)),
                           ex => $"La plantilla no se pudo evaluar: {ex.Message}"));
```

### 8.3. Mensaje enriquecido solo cuando falla

```csharp
public MlResult<Transferencia> Autorizar(Transferencia t, decimal limiteDiario) =>
    That(t,
         x => x.Importe + _acumulado(x.Cuenta) <= limiteDiario,
         () => MlErrorsDetails.FromErrorMessage("Límite diario superado.")
                              .AddDetail("Cuenta",       t.Cuenta)
                              .AddDetail("LimiteDiario", limiteDiario)
                              .AddDetail("Acumulado",    _acumulado(t.Cuenta)));
```

---

## 9. Mejores prácticas

1. **`using static MoralesLarios.OOFP.Helpers.EnsureFp;`** al principio del archivo: las cadenas de
   validación se leen mucho mejor sin el prefijo repetido.
2. **Valida cuanto antes.** Las guardas son la primera línea del método, antes de tocar repositorios.
3. **Prefiere el predicado al `bool`** cuando la condición dependa del valor: evita evaluar dos veces
   y elimina el riesgo de desreferenciar un `null`.
4. **Usa mensajes perezosos** siempre que el mensaje contenga interpolaciones, `string.Join`,
   serializaciones o consultas.
5. **`*Arg` para argumentos, mensaje explícito para negocio.** El usuario final no debería leer
   nunca `'dto.Cliente.Email'`.
6. **`TryThat` solo donde realmente puede lanzarse una excepción.** Envolverlo todo oculta qué
   partes son peligrosas.
7. **No mezcles guardas con lógica.** Si un `That` necesita más de tres líneas, extrae un predicado
   con nombre.
8. **Para varias reglas del mismo objeto, no encadenes `Bind`**: usa la familia de
   [agregación](./2_EnsureFpAggregation.md) para ver **todos** los errores a la vez.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [8. Variantes asíncronas](./8_EnsureFpAsync.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
- [`Bind`](../Bind/3_Bind.md) · [`Map`](../Map/1_Map.md) · [`MapEnsure`](../Map/2_MapEnsure.md)
- [`NullToFailed`](../Several/2_NullToFailed.md) · [`EmptyToFailed`](../Several/1_EmptyToFailed.md) ·
  [`BoolToResult`](../Several/3_BoolToResult.md)
