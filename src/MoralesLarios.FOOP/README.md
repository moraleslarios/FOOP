# MoralesLarios.OOFP / MoralesLarios.FOOP

`MoralesLarios.OOFP` es el núcleo funcional del ecosistema. Su objetivo es centralizar la validación, el control de errores y el flujo de negocio con un patrón consistente basado en `MlResult<T>` y composición funcional.

La librería se usa de base por capas posteriores como `ValueObjects`, `Validation`, `EFCore`, `WebServices`, `WebApi`, clientes HTTP y utilidades. La idea principal es evitar que los errores se conviertan en un mecanismo de control principal, y en su lugar transportar el estado explícito del flujo con resultados y detalles estructurados.

---

## Qué aporta esta librería

- `MlResult<T>` para éxito/error con valor explícito.
- Validaciones funcionales con `EnsureFp`.
- `Bind`, `Map`, `Match`, `ExecSelf` y variantes `Try*` / `Async`.
- `MlError` y `MlErrorsDetails` para errores detallados.
- Extensiones para colecciones, acciones y conversiones funcionales.
- Soporte natural de asincronía con `Task<MlResult<T>>`.

---

## Estructura del núcleo

```text
MoralesLarios.FOOP/
├── Types/
│   ├── MlResult.cs
│   ├── MlResultActions.cs
│   ├── MlResultActionsBind.cs
│   ├── MlResultActionsMap.cs
│   ├── MlResultActionsMatch.cs
│   ├── MlResultActionsExecSelf.cs
│   ├── MlResultActionsSeveral.cs
│   ├── MlResultTransformations.cs
│   ├── MlResultBucles.cs
│   └── Errors/
│       ├── MlError.cs
│       └── MlErrorsDetails.cs
├── Helpers/
│   ├── EnsureFp.cs
│   └── Extensions/
│       └── Extensions.cs
├── GlobalUsings.cs
├── README.md
└── __Doc/
```

---

## Índice de contenidos

1. [El tipo central: `MlResult<T>`](#1-el-tipo-central-mlresultt)
2. [`MlError` y `MlErrorsDetails`](#2-mlerror-y-mlerrorsdetails)
3. [`EnsureFp`: validaciones funcionales](#3-ensurefp-validaciones-funcionales)
4. [`Bind`: encadenar operaciones](#4-bind-encadenar-operaciones)
5. [`Map`: transformar el valor válido](#5-map-transformar-el-valor-válido)
6. [`Match`: decidir por rama válida o errónea](#6-match-decidir-por-rama-válida-o-errónea)
7. [`ExecSelf`: ejecutar efectos secundarios sin perder el flujo](#7-execself-ejecutar-efectos-secundarios-sin-perder-el-flujo)
8. [Utilidades de `MlResultActions`](#8-utilidades-de-mlresultactions)
9. [`MlResultActionsSeveral`](#9-mlresultactionsseveral)
10. [`MlResultTransformations`](#10-mlresulttransformations)
11. [`MlResultBucles`: proyección y fusión con colecciones](#11-mlresultbucles-proyección-y-fusión-con-colecciones)
12. [Extensiones genéricas de ayuda](#12-extensiones-genéricas-de-ayuda)
13. [Patrón recomendado](#13-patrón-recomendado)
14. [Cuándo usar cada familia](#14-cuándo-usar-cada-familia)
15. [Documentación adicional del núcleo](#15-documentación-adicional-del-núcleo)
16. [Resumen](#16-resumen)

---

## 1. El tipo central: `MlResult<T>`

`MlResult<T>` es el punto de entrada del diseño funcional de la librería: la práctica totalidad de operaciones, validaciones y transformaciones del ecosistema se apoyan en el mismo contrato de éxito o fallo. Entender bien este tipo es entender el 80 % de la librería, porque el resto de familias de métodos (`Bind`, `Map`, `Match`, `ExecSelf`, `EnsureFp`, `Projection*`…) no son más que formas distintas de operar sobre él.

### La idea de fondo: el error como dato, no como excepción

En el modelo tradicional de .NET, cuando algo va mal se lanza una excepción y el control salta a un `catch` situado en otro punto del programa. Eso tiene dos problemas: el flujo se vuelve difícil de seguir y la firma del método miente (un `Task<User>` en realidad puede devolver "usuario" o "explosión").

`MlResult<T>` invierte ese planteamiento: **el error deja de ser un salto de control y se convierte en un dato más que viaja por la cadena**. La firma se vuelve honesta (`MlResult<User>` significa literalmente "un usuario o el motivo de por qué no lo hay") y el flujo se lee de arriba abajo, sin saltos.

```csharp
// ❌ Estilo tradicional: la firma no dice nada de los posibles fallos.
public User GetUser(int id)
{
    if (id <= 0) throw new ArgumentException("Id no válido");
    var user = _repo.Find(id);
    if (user is null) throw new NotFoundException($"No existe el usuario {id}");
    return user;
}

// ✅ Estilo funcional: la firma declara que esto puede fallar y el fallo es un valor.
public MlResult<User> GetUser(int id)
    => EnsureFp.That(id, id > 0, "Id no válido")
               .Bind(validId => _repo.Find(validId).NullToFailed($"No existe el usuario {validId}"));
```

En el segundo caso no hay `try/catch`, no hay excepciones de negocio y quien llama al método está **obligado por el propio tipo** a decidir qué hacer si falla.

### Los dos estados posibles

`MlResult<T>` representa dos estados y sólo dos:

| Estado | Propiedades | Significado |
|--------|-------------|-------------|
| Válido | `IsValid == true`, `IsFail == false` | El flujo tiene un valor correcto disponible en `Value`. |
| Fallido | `IsValid == false`, `IsFail == true` | El flujo ha fallado y el motivo está en `ErrorsDetails`. |

Estos dos estados son mutuamente excluyentes: nunca hay un resultado "medio válido". Esto es precisamente lo que permite que todas las operaciones posteriores sean predecibles: cada método sabe exactamente qué hacer en cada una de las dos ramas.

### Factories principales

Estas fábricas permiten crear el resultado en los estados más habituales. Es importante conocerlas porque casi todos los métodos del núcleo devuelven o esperan un `MlResult<T>` en lugar de un valor simple.

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

// 1) Resultado válido: envuelve un valor correcto.
var ok = MlResult<int>.Valid(42);

// 2) Resultado fallido a partir de un mensaje de texto simple.
var fail1 = MlResult<string>.Fail("No hay datos");

// 3) Resultado fallido a partir de un detalle de error estructurado.
var fail2 = MlResult<int>.Fail(MlErrorsDetails.FromErrorMessage("La validación ha fallado"));

// 4) Resultado sin valor asociado: se usa cuando la operación sólo indica éxito/fracaso
//    (por ejemplo, un borrado) y no hay nada que devolver.
var empty   = MlResult.Empty();
var discard = MlResult.Discard;
```

**Cuándo usar cada una:**
- `Valid(value)` → la operación ha ido bien y hay un dato que propagar.
- `Fail("mensaje")` → atajo cómodo cuando basta con un texto explicativo.
- `Fail(MlErrorsDetails)` → cuando quieres adjuntar varios errores a la vez o detalles extra (el valor que falló, una excepción, un código de negocio…).
- `MlResult.Empty()` / `MlResult.Discard` → operaciones tipo comando, donde el "éxito" no lleva payload.

### Propiedades y conversiones implícitas

La conversión implícita hace que el código sea mucho más natural: un valor simple se convierte automáticamente en un resultado válido, y un error viaja dentro del mismo tipo sin necesidad de envolverlo manualmente cada vez. En la práctica, esto permite escribir `return valor;` o `return "mensaje de error";` dentro de un método que devuelve `MlResult<T>`.

```csharp
MlResult<int> r1 = 42;                            // int            → resultado válido
MlResult<int> r2 = MlResult<int>.Fail("error");   // construcción explícita
MlResult<int> r3 = "valor inválido";              // string          → resultado fallido

Console.WriteLine(r1.IsValid); // True
Console.WriteLine(r2.IsFail);  // True
Console.WriteLine(r3.IsFail);  // True
```

La librería ofrece conversiones implícitas para:

| Desde | Hacia | Resultado |
|-------|-------|-----------|
| `T` | `MlResult<T>` | Resultado **válido** con ese valor. |
| `MlError` | `MlResult<T>` | Resultado **fallido** con ese error. |
| `MlErrorsDetails` | `MlResult<T>` | Resultado **fallido** con ese detalle completo. |
| `IEnumerable<MlError>` | `MlResult<T>` | Resultado **fallido** con varios errores agrupados. |

Gracias a esto, un método de negocio queda muy limpio:

```csharp
public MlResult<decimal> AplicarDescuento(decimal precio, decimal porcentaje)
{
    if (precio    <= 0) return "El precio debe ser mayor que cero.";   // → fail implícito
    if (porcentaje < 0 || porcentaje > 100) return "El porcentaje debe estar entre 0 y 100.";

    return precio - (precio * porcentaje / 100);                       // → valid implícito
}
```

### Ejemplos de uso reales

La forma correcta de "abrir" un resultado es `Match`, porque obliga a tratar ambas ramas y garantiza que nunca se lee un valor que no existe.

```csharp
var result = MlResult<string>.Valid("Hola");

var text = result.Match(
    valid: x      => $"OK: {x}",
    fail:  errors => $"ERR: {errors}"
);

Console.WriteLine(text); // OK: Hola
```

```csharp
var failure = MlResult<int>.Fail("El valor no es válido");

var message = failure.Match(
    valid: x      => $"Valor: {x}",
    fail:  errors => errors.ToString()
);

Console.WriteLine(message); // El valor no es válido
```

Un ejemplo algo más completo, encadenando validación, transformación y cierre, que resume el estilo de trabajo de toda la librería:

```csharp
// Entrada procedente de fuera (formulario, API, fichero…).
string? entrada = " 250 ";

var resultado = EnsureFp.NotNullEmptyOrWhitespace(entrada, "Debes indicar un importe.")
    .Map(texto  => texto.Trim())                                       // " 250 " → "250"
    .TryMap(texto => decimal.Parse(texto),                             // "250"   → 250m
            ex    => $"El importe no tiene un formato numérico válido: {ex.Message}")
    .MapEnsure(importe => importe > 0, "El importe debe ser positivo.")
    .Map(importe => importe * 1.21m);                                  // IVA incluido

var salida = resultado.Match(
    valid: total  => $"Total con IVA: {total:N2} €",
    fail:  errors => $"No se pudo calcular el total. Motivo: {errors}"
);

Console.WriteLine(salida); // Total con IVA: 302,50 €
```

Fíjate en que **no hay ni un solo `if` de comprobación de errores ni un `try/catch`**: cada eslabón de la cadena decide por sí mismo si continuar o cortocircuitar, y el error original se conserva intacto hasta el `Match` final.

### `ToString()`

`ToString()` está sobrescrito para facilitar el diagnóstico rápido y el logging: devuelve la representación del valor si el resultado es válido, o el texto del error si está en fallo.

```csharp
var ok   = MlResult<int>.Valid(15);
var fail = MlResult<int>.Fail("fallo 1");

Console.WriteLine(ok);   // 15
Console.WriteLine(fail); // fallo 1
```

Es muy práctico para volcar el estado en un log sin tener que hacer un `Match` sólo para imprimir.

---

## 2. `MlError` y `MlErrorsDetails`

Estos dos tipos representan la parte de error del flujo funcional. La distinción es sencilla pero importante:

- **`MlError`** = *un* error individual (un mensaje).
- **`MlErrorsDetails`** = *el paquete completo de error* que viaja dentro de un `MlResult` fallido: una colección de `MlError` **más** un diccionario de detalles adicionales.

Esa separación es la que permite que la información no se pierda a lo largo de la cadena: se pueden acumular varios errores de validación y, además, adjuntar contexto técnico (el valor que falló, la excepción original, el identificador de la entidad, etc.) sin mezclarlo con el mensaje que verá el usuario final.

### `MlError`

`MlError` es la pieza básica del error funcional. Normaliza el mensaje y ofrece conversión implícita desde `string`, de modo que casi nunca hace falta construirlo a mano.

```csharp
using MoralesLarios.OOFP.Types.Errors;

var error = MlError.FromErrorMessage("Usuario no encontrado");
var also  = "Nombre requerido";              // simple string

Console.WriteLine(error.Message);            // Usuario no encontrado
Console.WriteLine(also.ToMlError());         // Nombre requerido
```

`MlErrorExtensions` añade dos atajos de conversión que se usan constantemente:

| Extensión | Sobre | Devuelve | Para qué sirve |
|-----------|-------|----------|----------------|
| `ToMlError()` | `string` | `MlError` | Convertir un mensaje suelto en un error tipado. |
| `ToMlErrors()` | `string` / `IEnumerable<string>` | `IEnumerable<MlError>` | Preparar una lista de errores para agruparlos en un único fallo. |

```csharp
IEnumerable<MlError> errors = "error 1".ToMlErrors();

// Caso típico: convertir los mensajes de una validación en un único resultado fallido.
var mensajes = new[] { "El nombre es obligatorio.", "La edad debe ser positiva." };
MlResult<Person> resultado = mensajes.ToMlErrors();   // conversión implícita a fail
```

### `MlErrorsDetails`

`MlErrorsDetails` contiene una colección de errores y un diccionario con detalles adicionales. Es lo que realmente se transporta dentro de un `MlResult` fallido.

```csharp
// 1) Error simple: sólo un mensaje.
var errors = MlErrorsDetails.FromErrorMessage("Fallo de validación");

// 2) Error + el valor que lo provocó (queda registrado en los detalles).
var withValue = MlErrorsDetails.FromErrorMessageWithValue("Id no válido", 42);

// 3) Error + detalle técnico arbitrario (aquí, la excepción original).
var withDetail = MlErrorsDetails.FromErrorDetails(
    "Fallo inesperado",
    "Exception",
    new InvalidOperationException("boom"));
```

La diferencia práctica entre las tres es **cuánta información de diagnóstico sobrevive**: la primera sólo dice *qué* pasó; la segunda añade *con qué dato* pasó; la tercera añade *el detalle técnico* para poder depurarlo después sin reproducir el caso.

### Factory methods reales

```csharp
// A partir de varios mensajes de texto: ideal para validaciones acumuladas.
var errors1 = MlErrorsDetails.FromEnumerableStrings(new[] { "e1", "e2" });

// A partir de un MlError ya construido.
var errors2 = MlErrorsDetails.FromError(MlError.FromErrorMessage("error"));

// A partir de un mensaje más un diccionario de detalles a medida.
var errors3 = MlErrorsDetails.FromErrorMessageDetails(
    "mensaje",
    new Dictionary<string, object> { ["key"] = 123 });
```

Un caso real de uso: recoger todas las reglas incumplidas de una entidad y devolverlas juntas en una sola respuesta, en lugar de ir informando de una en una.

```csharp
public MlResult<Person> Validar(Person person)
{
    var errores = new List<string>();

    if (string.IsNullOrWhiteSpace(person.Name)) errores.Add("El nombre es obligatorio.");
    if (person.Name.Length is 1)                errores.Add("El nombre debe tener al menos 2 caracteres.");
    if (person.Age < 0)                         errores.Add("La edad no puede ser negativa.");

    return errores.Any()
        ? MlResult<Person>.Fail(MlErrorsDetails.FromEnumerableStrings(errores))
        : MlResult<Person>.Valid(person);
}
```

### Conversión implícita

Igual que con `MlResult<T>`, las conversiones implícitas evitan ruido sintáctico:

```csharp
MlErrorsDetails d1 = "mensaje simple";                 // desde string
MlErrorsDetails d2 = new[] { "a", "b" };               // desde colección de strings
MlErrorsDetails d3 = MlError.FromErrorMessage("x");    // desde MlError
```

Esto es lo que permite escribir `return "mensaje de error";` en un método que devuelve `MlResult<T>`: el `string` se convierte en `MlErrorsDetails` y éste, a su vez, en un resultado fallido.

### Formateo

`ToString()` genera una salida legible que incluye los mensajes y los detalles asociados, lo que resulta muy útil tanto en logs como en respuestas de diagnóstico.

```csharp
var error = MlErrorsDetails.FromErrorMessageWithValue("El registro no existe", 7);
Console.WriteLine(error.ToString());
// Muestra el mensaje junto al detalle del valor (7) que provocó el fallo.
```

---

## 3. `EnsureFp`: validaciones funcionales

`EnsureFp` es la capa de precondiciones del núcleo. Se usa **al principio** de la lógica de negocio para comprobar argumentos, valores nulos, colecciones vacías o condiciones de dominio, y convertir cualquier fallo en un `MlResult<T>` consistente. Su objetivo es sustituir el clásico bloque de guardas con excepciones por una expresión que ya devuelve el flujo funcional.

```csharp
// ❌ Guardas tradicionales: rompen el flujo con excepciones.
if (nombre is null)                 throw new ArgumentNullException(nameof(nombre));
if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre vacío");
if (edad < 18)                      throw new ArgumentException("Menor de edad");

// ✅ Con EnsureFp: el fallo es un valor y la cadena continúa de forma natural.
var result = EnsureFp.NotNullEmptyOrWhitespace(nombre, "El nombre es obligatorio.")
                     .Bind(n => EnsureFp.That(n, edad >= 18, "Debes ser mayor de edad."));
```

Todos los métodos comparten la misma forma: **reciben el valor a validar y el error a devolver si la validación no se cumple**, y devuelven `MlResult<T>` con el propio valor si todo va bien. Además, cada método admite tanto un `string` como un `MlErrorsDetails` como segundo argumento, por si necesitas un error enriquecido en lugar de un simple mensaje.

### `NotNull`

La validación más básica: comprueba si el valor entrante es `null` antes de continuar. Si la comprobación falla, el flujo se corta inmediatamente con un error claro y estructurado; si no, el valor sigue viajando por la cadena.

```csharp
string? nombre = null;

var result = EnsureFp.NotNull(nombre, "El nombre no puede ser nulo");
```

- Si `nombre` es `null`, devuelve `fail` con el mensaje indicado.
- Si no, devuelve un `MlResult<string?>` válido que contiene el mismo `nombre`.

Es el sustituto directo de `ArgumentNullException` cuando la ausencia de valor es una **situación de negocio esperable** (por ejemplo, un campo que el usuario no ha rellenado) y no un error de programación.

### `NotEmpty`

Valida que una colección no sea `null` **ni** esté vacía. Es muy útil antes de procesar lotes, porque evita ejecutar toda la maquinaria de un proceso masivo para descubrir al final que no había nada que procesar.

```csharp
var result = EnsureFp.NotEmpty(new[] { 1, 2, 3 }, "La colección no puede estar vacía");
```

```csharp
// Caso real: no tiene sentido facturar un pedido sin líneas.
var facturable = EnsureFp.NotEmpty(pedido.Lineas, "El pedido no contiene líneas.")
                         .Map(lineas => lineas.Sum(l => l.Importe));
```

### `NotNullEmptyOrWhitespace`

Valida cadenas de texto en las tres situaciones problemáticas de golpe: `null`, cadena vacía (`""`) y cadena compuesta sólo por espacios (`"   "`). Es la validación por defecto para cualquier texto que llegue desde el exterior, porque `" "` suele ser tan inválido como `null` pero se cuela en un simple `!= null`.

```csharp
var result = EnsureFp.NotNullEmptyOrWhitespace("   ", "El texto es obligatorio");
// → fail: los espacios en blanco no cuentan como contenido válido.
```

### `That`

`That` es la validación de propósito general: evalúa una **condición booleana** y devuelve `Valid(value)` si se cumple o `Fail(error)` si no. Aquí es donde encajan las reglas de negocio que no son simples comprobaciones de nulidad.

```csharp
var result = EnsureFp.That(10, 10 > 0, "Debe ser positivo");
```

Importante: la condición se recibe ya **evaluada** (es un `bool`, no una lambda), así que puedes construirla con cualquier expresión, incluso combinando varias comprobaciones:

```csharp
var edad = 17;

var mayorDeEdad = EnsureFp.That(edad, edad >= 18, "Debes ser mayor de edad para continuar.");

// Reglas compuestas en una sola guarda.
var importe = 1500m;
var saldo   = 900m;

var pagoValido = EnsureFp.That(importe,
                              importe > 0 && importe <= saldo,
                              $"El importe {importe:N2} € supera el saldo disponible ({saldo:N2} €).");
```

### `ThatAsync`

Versión asíncrona de `That`, pensada para encajar en cadenas `async` sin romper el `await`. Devuelve `Task<MlResult<T>>`, de modo que puede seguir componiéndose con `BindAsync`, `MapAsync` o `MatchAsync`.

```csharp
var result = await EnsureFp.ThatAsync(25, 25 > 0, "Debe ser positivo");
```

Su utilidad real es servir de **primer eslabón de una cadena asíncrona**, evitando tener que mezclar métodos síncronos y asíncronos:

```csharp
var resultado = await EnsureFp.ThatAsync(pedidoId, pedidoId > 0, "Identificador de pedido no válido.")
    .BindAsync(id => _repo.GetPedidoAsync(id))
    .MapAsync(pedido => pedido.ToDto());
```

### `NotNullAsync`, `NotEmptyAsync`, `NotNullEmptyOrWhitespaceAsync`

Son las contrapartidas asíncronas de las tres validaciones anteriores. Su comportamiento es idéntico, pero devuelven `Task<MlResult<T>>` para poder iniciar o continuar un flujo `async` sin fricción.

```csharp
var result  = await EnsureFp.NotNullAsync("pepe", "El nombre es obligatorio");
var emptyOk = await EnsureFp.NotEmptyAsync(new[] { 1 }, "Debe haber elementos");
var textOk  = await EnsureFp.NotNullEmptyOrWhitespaceAsync("Luis", "Texto obligatorio");
```

**Resumen de la familia `EnsureFp`:**

| Método | Valida | Úsalo cuando… |
|--------|--------|---------------|
| `NotNull` | valor `!= null` | el dato puede no venir informado. |
| `NotEmpty` | colección con al menos un elemento | vas a procesar un lote o agregar valores. |
| `NotNullEmptyOrWhitespace` | texto con contenido real | el dato es una cadena de entrada de usuario. |
| `That` | cualquier condición booleana | expresas una regla de negocio. |
| `*Async` | lo mismo, en flujo asíncrono | la cadena posterior es `async`. |

---

## 4. `Bind`: encadenar operaciones

`Bind` es la operación de composición más importante del modelo funcional del núcleo: **ejecuta el siguiente paso sólo si el resultado anterior fue válido**. Si no lo fue, el error se propaga tal cual, sin necesidad de comprobarlo manualmente en cada punto. Esto es lo que se conoce como *railway oriented programming*: el flujo circula por la vía del éxito y, en el momento en que algo falla, se desvía a la vía del error y ya no vuelve.

La regla para elegir entre `Bind` y `Map` es simple y conviene memorizarla:

| Si tu función devuelve… | Usa |
|-------------------------|-----|
| `MlResult<TOut>` (puede fallar) | **`Bind`** |
| `TOut` (no puede fallar) | **`Map`** |

Usar `Map` con una función que devuelve `MlResult` produciría un `MlResult<MlResult<T>>` anidado; `Bind` es precisamente lo que evita ese anidamiento aplanando el resultado.

### `Bind`

En este caso se parte de un valor válido y se encadena una operación que también devuelve `MlResult`. El flujo se sigue ejecutando sólo si todo va bien; si aparece un fallo intermedio, no se pierde el estado del error.

```csharp
var result = MlResult<int>.Valid(5)
    .Bind(x => MlResult<int>.Valid(x * 2));

var text = result.Match(
    valid: x => $"Resultado: {x}",   // Resultado: 10
    fail:  e => $"Error: {e}"
);
```

Y así se ve el cortocircuito en acción: en cuanto un eslabón falla, los siguientes **no llegan a ejecutarse**.

```csharp
var result = MlResult<int>.Valid(5)
    .Bind(x => MlResult<int>.Fail("Se ha roto aquí."))   // ← corta el flujo
    .Bind(x => MlResult<int>.Valid(x * 100))             // ← nunca se ejecuta
    .Bind(x => MlResult<int>.Valid(x + 1));              // ← nunca se ejecuta

Console.WriteLine(result); // Se ha roto aquí.
```

Un caso realista, donde cada paso depende del anterior y cualquiera puede fallar:

```csharp
public MlResult<FacturaDto> EmitirFactura(int pedidoId)
    => EnsureFp.That(pedidoId, pedidoId > 0, "Identificador de pedido no válido.")
        .Bind(id      => BuscarPedido(id))                  // MlResult<Pedido>
        .Bind(pedido  => ValidarPedido(pedido))             // MlResult<Pedido>
        .Bind(pedido  => CalcularImportes(pedido))          // MlResult<Factura>
        .Bind(factura => GuardarFactura(factura))           // MlResult<Factura>
        .Map(factura  => factura.ToDto());                  // Factura → DTO (no falla)
```

Se lee como una receta de arriba abajo. Si el pedido no existe, el error de "pedido no encontrado" llega intacto al final sin que ninguno de los pasos posteriores se ejecute.

### `BindAsync`

Versión asíncrona de `Bind`. Encadena una función que devuelve `Task<MlResult<TOut>>` y mantiene exactamente la misma semántica de cortocircuito, con la ventaja de que **no ejecuta la tarea asíncrona si el resultado previo ya venía fallido** (ahorrando así llamadas innecesarias a base de datos o a servicios externos).

```csharp
var result = await MlResult<int>.Valid(10)
    .BindAsync(async x =>
    {
        await Task.Delay(20);
        return MlResult<int>.Valid(x + 5);
    });
```

```csharp
// Caso real: sólo se consulta la base de datos si el identificador es válido.
var dto = await EnsureFp.ThatAsync(id, id > 0, "Id no válido.")
    .BindAsync(async validId => await _repo.GetByIdAsync(validId))
    .BindAsync(async user    => await _permisos.ComprobarAccesoAsync(user))
    .MapAsync(user => user.ToDto());
```

### `TryBind`

`TryBind` es `Bind` **con red de seguridad**: si la lambda lanza una excepción, ésta se captura y se convierte en un `MlResult` fallido con el mensaje que tú construyas a partir de la excepción. Es la forma de meter código que no controlas (parseos, librerías de terceros, llamadas a sistemas externos) dentro del flujo funcional sin que una excepción se escape y rompa la cadena.

```csharp
var result = MlResult<int>.Valid(12)
    .TryBind(
        x  => throw new InvalidOperationException("falla simulada"),
        ex => $"Fallo al calcular: {ex.Message}"
    );
```

Si la lambda lanza, la excepción se captura y se convierte en `MlResult` erróneo, en lugar de propagarse hacia arriba.

```csharp
// Caso real: la deserialización de un JSON externo puede lanzar y no queremos que tumbe el proceso.
var config = MlResult<string>.Valid(jsonRecibido)
    .TryBind(
        json => JsonSerializer.Deserialize<Configuracion>(json)
                    .NullToFailed("El JSON no contenía una configuración válida."),
        ex   => $"El JSON recibido no se pudo interpretar: {ex.Message}"
    );
```

**Regla práctica:** usa `Bind` cuando controlas todo el código de la lambda y sabes que sólo devuelve fallos como valor; usa `TryBind` en cuanto haya algo que pueda lanzar.

### `BindMulti`

`BindMulti` ejecuta **varias operaciones sobre el mismo valor de entrada** y fusiona los errores si alguna de ellas falla. La diferencia clave con encadenar varios `Bind` es que aquí las operaciones son *independientes entre sí*: todas parten del mismo valor y no se cortocircuitan unas a otras, así que puedes informar de **todos** los problemas a la vez en lugar de sólo del primero.

```csharp
var result = MlResult<int>.Valid(10)
    .BindMulti(
        x => MlResult<string>.Valid($"valor = {x}"),
        x => MlResult<string>.Valid($"doble = {x * 2}"),
        x => MlResult<string>.Valid($"triple = {x * 3}")
    );
```

La versión multi ejecuta varias validaciones y fusiona errores si alguna falla.

```csharp
// Caso real: validar un formulario completo y devolver TODOS los errores de golpe,
// que es lo que espera un usuario rellenando una pantalla.
var validado = MlResult<Registro>.Valid(registro)
    .BindMulti(
        r => EnsureFp.NotNullEmptyOrWhitespace(r.Email,  "El email es obligatorio."),
        r => EnsureFp.That(r.Edad, r.Edad >= 18,          "Debes ser mayor de edad."),
        r => EnsureFp.That(r.Password, r.Password.Length >= 8,
                           "La contraseña debe tener al menos 8 caracteres.")
    );

// Si fallan las tres, el resultado contiene los tres mensajes agrupados.
```

**Cuándo elegir cada uno de los dos enfoques:**
```
- Varios `Bind` en cadena** → cuando los pasos **dependen** unos de otros: no puedes calcular los importes de un pedido sin haberlo localizado antes. El flujo se corta en el primer problema y se informa del **primer** error.
- **Un único `BindMulti`** → cuando las comprobaciones son **independientes** entre sí y todas parten del mismo dato. No se cortocircuitan y se informa de **todos** los errores a la vez, algo ideal para validar formularios.
```

---

## 5. `Map`: transformar el valor válido

`Map` no cambia el estado del flujo; **sólo transforma el valor cuando el resultado está en OK**. Es ideal para normalizar datos, calcular propiedades derivadas o preparar el valor final para la capa de salida. Si el resultado venía fallido, la lambda de transformación simplemente no se ejecuta y el error pasa de largo.

Piensa en `Map` como una tubería de conversión de tipos dentro de la vía del éxito: `MlResult<A>` → `MlResult<B>`, sin posibilidad de que la propia conversión introduzca un fallo nuevo.

### `Map`

Este ejemplo convierte un texto a mayúsculas sin tocar el flujo del error. Si el valor original era válido, la transformación se aplica; si era fallido, se conserva el fallo sin ejecutar la lógica de conversión.

```csharp
var result = MlResult<string>.Valid("juan")
    .Map(x => x.ToUpperInvariant());   // → válido con "JUAN"

var propagado = MlResult<string>.Fail("no hay nombre")
    .Map(x => x.ToUpperInvariant());   // → sigue fallido; la lambda NO se ejecuta
```

El uso más frecuente en una aplicación real es la conversión de entidad a DTO al final del flujo, y la normalización de datos al principio:

```csharp
var dto = BuscarUsuario(id)                        // MlResult<User>
    .Map(user => new UserDto                       // User → UserDto (nunca falla)
    {
        Id       = user.Id,
        Nombre   = user.Nombre.Trim(),
        Email    = user.Email.ToLowerInvariant(),
        EsActivo = user.FechaBaja is null
    });
```

### `MapAsync`

Versión asíncrona de `Map`: la transformación devuelve un `Task<TOut>`. Igual que `BindAsync`, **no ejecuta la tarea si el resultado ya venía fallido**, lo que evita trabajo inútil.

```csharp
var result = await MlResult<string>.Valid("madrid")
    .MapAsync(async x =>
    {
        await Task.Delay(10);
        return x.ToUpperInvariant();
    });
```

```csharp
// Caso real: enriquecer un resultado válido con datos de un servicio externo
// cuya llamada no se considera un punto de fallo del negocio.
var informe = await ObtenerVentasAsync(mes)
    .MapAsync(async ventas => await _formateador.GenerarPdfAsync(ventas));
```

### `TryMap`

`TryMap` es la variante protegida de `Map`: aplica la transformación y, si ésta lanza una excepción, la convierte en un `fail` con el mensaje que construyas. Es imprescindible para conversiones que pueden explotar, como parseos, castings o accesos a estructuras que quizá no tengan la forma esperada.

```csharp
var result = MlResult<string>.Valid("hola")
    .TryMap(x => int.Parse(x), ex => $"No se pudo parsear: {ex.Message}");
// "hola" no es un número → fail con el mensaje construido, sin excepción propagada.
```

```csharp
// Caso real: convertir texto de entrada a tipos de dominio de forma segura.
var fecha = MlResult<string>.Valid(textoFecha)
    .TryMap(t  => DateTime.ParseExact(t, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            ex => $"La fecha '{textoFecha}' no tiene el formato esperado (yyyy-MM-dd).");
```

### `MapEnsure`

`MapEnsure` combina transformación y validación: comprueba una condición sobre el valor válido y, si no se cumple, convierte el resultado en un `fail` con detalles. Es la forma de intercalar una regla de negocio **en medio** de una cadena, sin tener que salir a `EnsureFp` ni escribir un `Bind` sólo para validar.

```csharp
var result = MlResult<int>.Valid(5)
    .MapEnsure(x => x > 0, "El valor debe ser positivo");
```

Si falla la condición, se convierte a un `fail` con detalles; si se cumple, el valor continúa intacto.

```csharp
// Caso real: comprobaciones intermedias sin romper la fluidez de la cadena.
var total = ObtenerCarrito(clienteId)
    .MapEnsure(c => c.Lineas.Any(),        "El carrito está vacío.")
    .MapEnsure(c => c.Lineas.Count <= 50,  "No se admiten más de 50 líneas por pedido.")
    .Map(c => c.Lineas.Sum(l => l.Importe))
    .MapEnsure(importe => importe <= 10_000m, "El importe supera el límite autorizado.");
```

**Resumen de la familia:**
```
| Método | Transformación | Si la lambda falla | Uso típico |
|--------|----------------|--------------------|------------|
| `Map` | `T → TOut` | no puede fallar | mapear a DTO, normalizar. |
| `MapAsync` | `T → Task<TOut>` | no puede fallar | enriquecer con datos externos. |
| `TryMap` | `T → TOut` | excepción → `fail` | parseos y conversiones. |
| `MapEnsure` | `T → T` + condición | condición falsa → `fail` | reglas intermedias. |
```

---

## 6. `Match`: decidir por rama válida o errónea

`Match` es el **punto de cierre** del flujo funcional: recibe el resultado final y decide el comportamiento según su estado. Es la forma idiomática de salir del mundo `MlResult<T>` y volver al mundo normal (un `string`, un `IActionResult`, un código de salida…).

Su gran virtud es que **obliga a tratar las dos ramas**: no es posible olvidarse del caso de error, porque el compilador exige ambas lambdas. Esto elimina de raíz la clase de bugs en que un fallo se ignora silenciosamente.

Regla de oro: **usa `Bind`/`Map` mientras estés operando y `Match` sólo una vez, al final**. Si aparecen varios `Match` intermedios en una cadena, casi siempre es señal de que debería haber un `Bind`.

### `Match`

Aquí se cierra el resultado y se decide el texto final según si el estado es válido o erróneo. En una API real, esta rama suele responder con un DTO, un mensaje, un `ProblemDetails` o un código de negocio concreto.

```csharp
var result = MlResult<int>.Valid(7);

var output = result.Match(
    valid: x      => $"El valor es {x}",
    fail:  errors => $"Hay error: {errors}"
);
```

El caso más habitual en una aplicación web es convertir el resultado en una respuesta HTTP:

```csharp
[HttpGet("{id:int}")]
public IActionResult Get(int id)
    => _servicio.ObtenerUsuario(id)
        .Match<IActionResult>(
            valid: dto    => Ok(dto),
            fail:  errors => BadRequest(new { errores = errors.ToString() })
        );
```

Y también sirve para decidir un flujo de programa completo, no sólo un valor:

```csharp
var codigoSalida = ProcesarFichero(ruta).Match(
    valid: resumen => { Console.WriteLine($"Procesadas {resumen.Filas} filas."); return 0; },
    fail:  errors  => { Console.Error.WriteLine($"Error: {errors}");             return 1; }
);
```

### `MatchAsync`

Versión asíncrona de `Match`, para cuando el cierre del flujo requiere trabajo asíncrono en una o ambas ramas (guardar en base de datos, enviar una notificación, escribir en un log remoto…). Sólo se ejecuta la rama correspondiente al estado real del resultado.

```csharp
var result = await MlResult<string>.Valid("ok")
    .MatchAsync(
        validAsync: async x =>
        {
            await Task.Delay(10);
            return $"OK:{x}";
        },
        failAsync: async errors =>
        {
            await Task.Delay(10);
            return $"ERR:{errors}";
        }
    );
```

```csharp
// Caso real: confirmar o compensar una operación según el resultado.
var mensaje = await ProcesarPagoAsync(pago)
    .MatchAsync(
        validAsync: async recibo => { await _mailer.EnviarReciboAsync(recibo); return "Pago confirmado."; },
        failAsync:  async errors => { await _log.RegistrarFalloAsync(errors);  return "Pago rechazado.";  }
    );
```

### `TryMatch`

`TryMatch` protege el propio cierre del flujo: si la lambda de la rama válida (o la de error) lanza una excepción, ésta se captura y se transforma en un resultado fallido mediante `errorMessageBuilder`, en lugar de propagarse. Es útil cuando la acción final es delicada —serializar, escribir en disco, invocar un sistema externo— y no quieres que un fallo en ese último paso deje el proceso sin control.

```csharp
var result = MlResult<int>.Valid(5)
    .TryMatch(
        valid:               x  => x.ToString(),
        fail:                e  => $"Error: {e}",
        errorMessageBuilder: ex => $"Excepción: {ex.Message}"
    );
```

**Resumen de la familia:**
```
| Método | Ramas | Úsalo cuando… |
|--------|-------|---------------|
| `Match` | síncronas | cierras el flujo y devuelves un valor. |
| `MatchAsync` | asíncronas | el cierre implica I/O o llamadas externas. |
| `TryMatch` | síncronas protegidas | la acción de cierre puede lanzar excepción. |
```

---

## 7. `ExecSelf`: ejecutar efectos secundarios sin perder el flujo

`ExecSelf` sirve para ejecutar **efectos secundarios** —registrar trazas, emitir métricas, notificar, auditar— sin destruir ni alterar el resultado que ya se está propagando. Es una de las piezas más útiles cuando se quiere añadir observabilidad a un flujo sin complicar el camino principal.

La diferencia con `Map` es fundamental: `Map` **transforma** el valor y devuelve otro distinto; `ExecSelf` **no toca nada** y devuelve exactamente el mismo `MlResult` que recibió. Por eso se puede intercalar en cualquier punto de una cadena sin cambiar su tipo ni su semántica:

```csharp
var result = ObtenerPedido(id)
    .ExecSelf(p => _log.Info($"Pedido {p.Id} localizado."),      // no altera el flujo
              e => _log.Warn($"No se encontró el pedido: {e}"))
    .Bind(p => Facturar(p))                                       // la cadena sigue igual
    .ExecSelf(f => _metricas.Incrementar("facturas.emitidas"),
              e => _metricas.Incrementar("facturas.fallidas"));
```

### `ExecSelf`

Ejecuta una acción u otra según el estado y devuelve el resultado original intacto. Recibe dos delegados: uno para la rama válida (con el valor) y otro para la rama de error (con el detalle).

```csharp
var result = MlResult<int>.Valid(10)
    .ExecSelf(
        x => Console.WriteLine($"Procesado: {x}"),
        e => Console.WriteLine($"Error: {e}")
    );

// Imprime "Procesado: 10" y result sigue siendo el válido con 10.
```

Es el punto ideal para trazas de auditoría, porque el registro queda **junto al paso al que se refiere** y no en un bloque aparte:

```csharp
var resultado = EnsureFp.NotNullEmptyOrWhitespace(usuario, "Usuario obligatorio.")
    .ExecSelf(u => _log.Debug($"Intento de acceso de '{u}'."),
              e => _log.Warn ($"Intento de acceso sin usuario: {e}"))
    .Bind(u => Autenticar(u))
    .ExecSelf(sesion => _auditoria.Registrar(sesion.UsuarioId, "LOGIN_OK"),
              errors => _auditoria.Registrar(null,             "LOGIN_KO"));
```

### `ExecSelfAsync`

Versión asíncrona: los efectos secundarios pueden ser `async` (escribir un fichero, publicar un evento, llamar a un servicio de telemetría). Igual que en la versión síncrona, el resultado devuelto es el original, sin modificar.

```csharp
var result = await MlResult<string>.Valid("abc")
    .ExecSelfAsync(
        async x => await File.WriteAllTextAsync("out.txt", x),
        async e => await Console.Out.WriteLineAsync(e.ToString())
    );
```

```csharp
// Caso real: publicar un evento de integración sin condicionar el flujo principal.
var pedido = await GuardarPedidoAsync(nuevo)
    .ExecSelfAsync(
        async p => await _bus.PublicarAsync(new PedidoCreado(p.Id)),
        async e => await _log.ErrorAsync($"No se pudo guardar el pedido: {e}")
    );
```

### `TryExecSelf`

`TryExecSelf` protege el propio efecto secundario. Es un detalle importante: **un fallo al registrar una traza no debería tumbar una operación de negocio que ya había ido bien**. Con `TryExecSelf`, si la acción lanza una excepción, ésta se captura y se convierte en un `fail` controlado mediante el constructor de mensaje, en lugar de propagarse fuera de la cadena.

```csharp
var result = MlResult<int>.Valid(3)
    .TryExecSelf(
        x  => throw new InvalidOperationException("boom"),
        e  => Console.WriteLine($"Error: {e}"),
        ex => $"Se produjo: {ex.Message}"
    );
```

```csharp
// Caso real: el sistema de logging externo puede estar caído; queremos enterarnos,
// pero de forma controlada y sin excepciones sueltas.
var resultado = ProcesarLote(lote)
    .TryExecSelf(
        r  => _telemetriaRemota.Enviar(r.Resumen),   // puede lanzar por red
        e  => _log.Warn(e.ToString()),
        ex => $"No se pudo enviar la telemetría: {ex.Message}"
    );
```

**Resumen de la familia:**
```
| Método | Efecto | Devuelve | Úsalo cuando… |
|--------|--------|----------|---------------|
| `ExecSelf` | acción síncrona | el mismo resultado | logging y trazas simples. |
| `ExecSelfAsync` | acción asíncrona | el mismo resultado | eventos, ficheros, telemetría. |
| `TryExecSelf` | acción protegida | el mismo resultado o `fail` | el efecto puede lanzar excepción. |
```

---

## 8. Utilidades de `MlResultActions`

La clase `MlResultActions` añade helpers para **enriquecer** el resultado: añadir detalles al error, completar el valor válido, acceder de forma segura al contenido o combinar varios resultados en uno. Son piezas de apoyo que evitan escribir código repetitivo alrededor de `MlResult<T>`.

### `AddMlErrorDetailIfFail` / `AddValueDetailIfFail`

Estos métodos añaden información contextual **sólo si el resultado está en estado `fail`**. Si el resultado es válido, no hacen nada y lo devuelven intacto. Su utilidad es enorme a la hora de depurar: permiten ir acumulando pistas a medida que el error asciende por las capas, sin cambiar el mensaje original que verá el usuario.

```csharp
var result = MlResult<int>.Fail("No válido")
    .AddValueDetailIfFail(42);
```

Esto añade información contextual al detalle del error si el resultado está en estado `fail`: en el ejemplo, queda registrado que el valor implicado era `42`.

- `AddValueDetailIfFail(valor)` → adjunta **el dato** que provocó el problema.
- `AddMlErrorDetailIfFail(detalle)` → adjunta **un detalle de error adicional** (otro mensaje, un código, información técnica).

```csharp
// Caso real: cada capa añade su contexto sin reescribir el error original.
public MlResult<Pedido> BuscarPedido(int id)
    => _repo.Find(id)
            .NullToFailed("El pedido no existe.")
            .AddValueDetailIfFail(id)                                  // ¿qué id se buscó?
            .AddMlErrorDetailIfFail($"Consulta ejecutada por {_usuarioActual}.");
```

Cuando el error llega al log o a la respuesta de diagnóstico, contiene el mensaje de negocio **y** todo el rastro acumulado, sin necesidad de reproducir el caso.

### `CompleteWithDataValueIfValid`

Actúa de forma simétrica a los anteriores: **sólo hace algo si el resultado es válido**, completando o derivando el valor. Si el resultado venía fallido, la función no se ejecuta y el error se propaga.

```csharp
var result = MlResult<int>.Valid(5)
    .CompleteWithDataValueIfValid(x => x * 2);
```

Se usa cuando quieres añadir datos calculados al resultado válido sin romper la cadena y sin necesidad de plantear un `Bind` completo.

### `CompleteWithDetailsValueIfFail`

Complementa el detalle del error con información adicional cuando el resultado está en fallo. Es la contrapartida del método anterior: uno enriquece la vía del éxito, el otro la vía del error.

```csharp
var result = MlResult<string>.Fail("error")
    .CompleteWithDetailsValueIfFail("contexto");
```

Muy útil justo antes de devolver el resultado a la capa superior, para dejar constancia del contexto de ejecución (nombre del método, parámetros de entrada, identificador de correlación…).

### `SecureValidValue` / `SecureFailErrorsDetails`

Permiten **acceder directamente** al contenido del resultado, pero de forma protegida: si el resultado no está en el estado esperado, lanzan excepción en lugar de devolver un valor basura o `null`.

```csharp
var ok = MlResult<int>.Valid(99);
Console.WriteLine(ok.SecureValidValue());        // 99

var bad = MlResult<int>.Fail("error");
Console.WriteLine(bad.SecureFailErrorsDetails()); // error
```

Este patrón lanza excepción si el flujo no está en el estado esperado, lo que ayuda a proteger el acceso inseguro a datos: convierte un posible bug silencioso en un fallo inmediato y visible.

- `SecureValidValue()` → devuelve el valor; **lanza** si el resultado está en `fail`.
- `SecureFailErrorsDetails()` → devuelve el detalle del error; **lanza** si el resultado es válido.

⚠️ **Cuándo usarlos:** principalmente en **tests** y en puntos donde ya has comprobado el estado (`if (result.IsValid) …`). En código de producción, la vía recomendada es siempre `Match`, porque no puede lanzar y obliga a tratar ambas ramas.

### `CreateCompleteMlResult`

Combina dos resultados en uno solo. Devuelve un resultado con **ambos valores** cuando los dos son válidos; si cualquiera de ellos falla, devuelve los **errores fusionados**.

```csharp
var r1 = MlResult<int>.Valid(10);
var r2 = MlResult<string>.Valid("x");

var merged = r1.CreateCompleteMlResult(r2);
```

Devuelve un resultado con ambos valores cuando ambos son válidos; si cualquiera falla, devuelve errores fusionados.

```csharp
// Caso real: una operación necesita dos datos que se obtienen por separado,
// y queremos saber de golpe si falta alguno de los dos (no sólo el primero).
var cliente   = BuscarCliente(clienteId);      // MlResult<Cliente>
var direccion = BuscarDireccion(direccionId);  // MlResult<Direccion>

var envio = cliente.CreateCompleteMlResult(direccion)
    .Map(par => new Envio(par.Item1, par.Item2));
// Si faltan cliente y dirección, el error contiene ambos motivos.
```

**Resumen de la familia:**
```
| Método | Actúa si… | Para qué sirve |
|--------|-----------|----------------|
| `AddValueDetailIfFail` | fallo | adjuntar el dato que falló. |
| `AddMlErrorDetailIfFail` | fallo | adjuntar un detalle de error extra. |
| `CompleteWithDataValueIfValid` | válido | derivar/completar el valor. |
| `CompleteWithDetailsValueIfFail` | fallo | añadir contexto al detalle. |
| `SecureValidValue` | válido (o lanza) | acceso directo controlado al valor. |
| `SecureFailErrorsDetails` | fallo (o lanza) | acceso directo controlado al error. |
| `CreateCompleteMlResult` | ambos | combinar dos resultados y fusionar errores. |
```

---

## 9. `MlResultActionsSeveral`

Esta clase ofrece atajos para transformar **entradas del mundo imperativo** (valores nulos, colecciones vacías, condiciones booleanas) en `MlResult`. Es la puerta de entrada más cómoda al flujo funcional cuando trabajas con APIs, ORM o librerías que devuelven `null` o `bool` en lugar de resultados.

Son métodos de extensión, así que se leen de forma muy natural: se aplican directamente sobre el valor de origen.

### `NullToFailed`

Convierte un valor potencialmente nulo en un `MlResult`: si es `null`, resultado fallido con el mensaje indicado; si no, resultado válido con el valor. Es el método más usado del núcleo, porque casi todos los accesos a datos pueden devolver "nada".

```csharp
string? name = null;
var result = name.NullToFailed("El nombre es obligatiorio");
```

```csharp
// Caso real: un FirstOrDefault se convierte en un flujo funcional en una sola línea.
var usuario = _context.Users.FirstOrDefault(u => u.Email == email)
                            .NullToFailed($"No existe ningún usuario con el email '{email}'.");

// Y encadena de forma inmediata:
var dto = usuario.Map(u => u.ToDto());
```

Diferencia con `EnsureFp.NotNull`: son equivalentes en comportamiento, pero `NullToFailed` es una **extensión** (se aplica al final de una expresión, ideal para encadenar) mientras que `EnsureFp.NotNull` es una **llamada estática** (ideal para abrir un método a modo de guarda).

### `EmptyToFailed`

Convierte una colección vacía (o nula) en un resultado fallido. Evita ejecutar procesos de agregación sobre conjuntos sin elementos, que es una fuente clásica de resultados engañosos (medias de cero elementos, `Max()` que lanza, etc.).

```csharp
var items = Enumerable.Empty<int>();
var result = items.EmptyToFailed("La colección está vacía");
```

```csharp
// Caso real: calcular estadísticas sólo si hay datos que analizar.
var resumen = _repo.GetVentas(mes)
    .EmptyToFailed($"No hay ventas registradas en el mes {mes}.")
    .Map(ventas => new Resumen(
        Total:  ventas.Sum(v => v.Importe),
        Media:  ventas.Average(v => v.Importe),
        Maximo: ventas.Max(v => v.Importe)));
```

### `BoolToResult`

Convierte una condición booleana en un resultado, **conservando el valor original** cuando la condición se cumple. Es el equivalente en forma de extensión a `EnsureFp.That`.

```csharp
var result = 10 BoolToResult(10 > 0, "La condición no se cumple");
// → válido con 10, porque la condición es verdadera.
```

```csharp
// Caso real: comprobar una regla sobre un objeto sin salir de la expresión.
var pedidoEditable = pedido.BoolToResult(
    pedido.Estado == EstadoPedido.Borrador,
    $"El pedido {pedido.Id} no se puede modificar porque está en estado {pedido.Estado}.");
```

### `BoolToResult` sobre `bool`

Sobrecarga aplicada directamente sobre un `bool`: si es `true`, resultado válido; si es `false`, resultado fallido con el mensaje. Se usa cuando el valor que te interesa **es** la propia condición, típicamente al envolver métodos que devuelven `bool` para indicar éxito.

```csharp
var result = true BoolToResult("La condición no es válida");
```

```csharp
// Caso real: adaptar una API antigua que informa del éxito con un bool.
var borrado = _legacyRepo.Delete(id)
                         .BoolToResult($"No se pudo eliminar el registro {id}.");

var mensaje = borrado.Match(
    valid: _      => "Registro eliminado correctamente.",
    fail:  errors => errors.ToString());
```

**Resumen de la familia:**
```
| Extensión | Se aplica sobre | Falla cuando… |
|-----------|-----------------|---------------|
| `NullToFailed` | cualquier referencia/nullable | el valor es `null`. |
| `EmptyToFailed` | `IEnumerable<T>` | la colección es nula o vacía. |
| `BoolToResult` (valor + condición) | cualquier valor | la condición es `false`. |
| `BoolToResult` (sobre `bool`) | `bool` | el propio valor es `false`. |
```

Estos cuatro atajos son, en la práctica, **el puente entre el código heredado y el flujo funcional**: permiten adoptar `MlResult<T>` de forma incremental sin reescribir las capas inferiores.

---

## 10. `MlResultTransformations`

La clase `MlResultTransformations` convierte **funciones y acciones normales** en flujos `MlResult` sin romper el patrón. Su valor está en la interoperabilidad: te permite reutilizar delegados, métodos existentes o lógica de terceros dentro de una cadena funcional, sin tener que reescribirlos para que devuelvan `MlResult`.

En todos los casos, la mecánica es la misma: se invoca la función con el argumento indicado y el retorno se envuelve como resultado válido; en las variantes `Try*`, cualquier excepción se captura y se transforma en `fail`.

### `ToMlResult`

Ejecuta una `Func<TIn, TOut>` y envuelve el retorno en un resultado válido. Se usa cuando sabes que la función **no puede fallar** y sólo necesitas incorporarla al flujo.

```csharp
Func<int, int> square = x => x * x;
var result = square.ToMlResult(6);   // → válido con 36
```

```csharp
// Caso real: reutilizar una regla de cálculo ya existente dentro de la cadena.
Func<Pedido, decimal> calcularIva = p => p.BaseImponible * 0.21m;

var iva = calcularIva.ToMlResult(pedido)
    .MapEnsure(v => v >= 0, "El IVA calculado no puede ser negativo.");
```

### `TryToMlResult`

Igual que el anterior, pero **protegido**: si la función lanza una excepción, se captura y se convierte en un resultado fallido con el mensaje que construyas a partir de la excepción. Es la forma segura de invocar código que no controlas.

```csharp
Func<int, int> failFunc = x => int.Parse("oops");
var result = failFunc.TryToMlResult(1, ex => $"Error: {ex.Message}");
// → fail: "Error: The input string 'oops' was not in a correct format."
```

```csharp
// Caso real: envolver una librería de terceros que lanza excepciones.
Func<string, Documento> parsear = texto => _libreriaExterna.Parse(texto);

var documento = parsear.TryToMlResult(contenido,
    ex => $"El documento no se pudo interpretar: {ex.Message}");
```

### `ToMlResultAsync`

Versión asíncrona de `ToMlResult`: ejecuta una `Func<TIn, Task<TOut>>` y envuelve el resultado. Permite incorporar operaciones `async` existentes (llamadas HTTP, consultas a base de datos) al flujo funcional sin adaptarlas.

```csharp
Func<int, Task<int>> op = async x =>
{
    await Task.Delay(20);
    return x + 1;
};

var result = await op.ToMlResultAsync(5);   // → válido con 6
```

### `TryToMlResultAsync`

La combinación más habitual en la práctica: **asíncrono y protegido**. Toda llamada a un sistema externo puede fallar por red, timeout o permisos, y este método garantiza que ese fallo entre en el flujo como un `fail` bien descrito en lugar de como una excepción.

```csharp
Func<int, Task<int>> bad = async _ =>
{
    await Task.Delay(10);
    throw new InvalidOperationException("bad");
};

var result = await bad.TryToMlResultAsync(2, ex => $"Error: {ex.Message}");
// → fail: "Error: bad"
```

```csharp
// Caso real: consumir una API externa sin propagar excepciones de red.
Func<int, Task<Cotizacion>> consultar = id => _clienteHttp.GetCotizacionAsync(id);

var cotizacion = await consultar.TryToMlResultAsync(productoId,
    ex => $"No se pudo consultar la cotización del producto {productoId}: {ex.Message}");
```

### `TryToMlResultErrors`

Variante para adaptar **acciones que trabajan sobre el detalle de error** (`Action<MlErrorsDetails>`) al flujo funcional. Es la pieza que permite incorporar manejadores de error existentes —notificadores, formateadores, escritores de log— protegiéndolos frente a excepciones propias.

```csharp
var result = ((Action<MlErrorsDetails>)(e => Console.WriteLine(e))).TryToMlResultErrors<int>(
    MlErrorsDetails.FromErrorMessage("fallo"),
    ex => $"Error: {ex.Message}"
);
```

En otras palabras: ejecuta el manejador con el detalle de error indicado y, si el propio manejador revienta, ese segundo fallo también queda encapsulado en un `MlResult` en lugar de escaparse.

**Resumen de la familia `MlResultTransformations`:**

| Método | Delegado de entrada | Protegido | Asíncrono |
|--------|--------------------|-----------|-----------|
| `ToMlResult` | `Func<TIn, TOut>` | ❌ | ❌ |
| `TryToMlResult` | `Func<TIn, TOut>` | ✅ | ❌ |
| `ToMlResultAsync` | `Func<TIn, Task<TOut>>` | ❌ | ✅ |
| `TryToMlResultAsync` | `Func<TIn, Task<TOut>>` | ✅ | ✅ |
| `TryToMlResultErrors` | `Action<MlErrorsDetails>` | ✅ | ❌ |

Todas ellas convierten un delegado o método existente en un `MlResult`, de modo que el código heredado se incorpora al flujo funcional sin reescribirlo.

---

## 11. `MlResultBucles`: proyección y fusión con colecciones

Esta es, probablemente, la familia que más código repetitivo elimina de una aplicación real. Todo proceso por lotes —importar un fichero, validar una lista de líneas, enriquecer un catálogo llamando a un servicio externo— se enfrenta siempre a las mismas preguntas: *¿qué hago cuando un elemento falla?*, *¿sigo o me paro?*, *¿cómo devuelvo todos los errores juntos?*, *¿cómo separo lo que ha ido bien de lo que ha ido mal?*

`MlResultBucles` responde a esas cuatro preguntas con cuatro métodos distintos. La clave para usarla bien es entender que **cada uno implementa una política de error diferente** y que elegir el correcto es una decisión de negocio, no técnica.

### El problema que resuelve

Así se escribe un proceso por lotes de la forma tradicional:

```csharp
// ❌ Bucle imperativo: 20 líneas de fontanería y una decisión de error enterrada dentro.
var resultados = new List<Cliente>();
var errores    = new List<string>();

foreach (var fila in filas)
{
    try
    {
        var cliente = ConvertirFila(fila);          // puede lanzar
        if (cliente.Email is null)
        {
            errores.Add($"Fila {fila.Numero}: email obligatorio.");
            continue;                               // ¿continuamos o paramos? aquí se decide
        }
        resultados.Add(cliente);
    }
    catch (Exception ex)
    {
        errores.Add($"Fila {fila.Numero}: {ex.Message}");
    }
}

if (errores.Any()) return BadRequest(errores);
return Ok(resultados);
```

Y así con `MlResultBucles`:

```csharp
// ✅ La política de error la elige el método; el cuerpo sólo describe la transformación de UN elemento.
var resultado = filas.Projection(fila => ConvertirFila(fila));   // MlResult<IEnumerable<Cliente>>
```

La transformación se escribe **para un solo elemento** (`T → MlResult<TResult>`) y el método se encarga de recorrer, acumular, fusionar errores y decidir el resultado global. Esto tiene una ventaja añadida importante: la función de transformación es una unidad pequeña, reutilizable y trivialmente testeable por separado.

### Las cuatro políticas de error

| Método | Recorrido | Ante un fallo | Resultado |
|--------|-----------|---------------|-----------|
| `Projection` | recorre **todos** los elementos | continúa y **acumula** | `MlResult<IEnumerable<TResult>>`: válido con todos los valores, o fallido con **todos** los errores fusionados. |
| `ProjectionWhile` | recorre **hasta el primer fallo** | **se detiene** | `MlResult<IEnumerable<TResult>>`: válido con todos los valores, o fallido con el **primer** error. |
| `ProjectionParallelAsync` | lanza **todas** las tareas en paralelo | continúa y **acumula** | `Task<MlResult<IEnumerable<TResult>>>` con la misma semántica que `Projection`. |
| `ProjectionSplit` | recorre **todos** los elementos | ni corta ni falla | `MlResult<(Dictionary valids, Dictionary fails)>`: **siempre válido**, con lo bueno y lo malo separado. |

Regla mental rápida:

- ¿Quiero **informar de todo** lo que está mal y no guardar nada si algo falla? → `Projection`.
- ¿El proceso es **secuencial y dependiente** y no tiene sentido seguir tras un fallo? → `ProjectionWhile`.
- ¿Cada elemento implica **I/O independiente** (HTTP, base de datos) y quiero paralelismo? → `ProjectionParallelAsync`.
- ¿Quiero un proceso **tolerante**, que guarde lo válido y reporte lo inválido? → `ProjectionSplit`.

Todos ellos admiten además una sobrecarga con **índice** (`Func<T, int, MlResult<TResult>>`), muy útil para mensajes de error del tipo "línea 27 del fichero".

### `Projection`

Aplica la transformación a **todos** los elementos y fusiona los errores de los que hayan fallado. Es una operación *todo o nada*: si un solo elemento es inválido, el resultado global es fallido, pero **con la lista completa de motivos**, no sólo con el primero.

Ése es exactamente el comportamiento que se espera al validar un fichero o un formulario complejo: el usuario quiere ver de una vez todo lo que tiene que corregir.

```csharp
var numeros = new[] { 1, 2, 3 };

var result = numeros.Projection(x => MlResult<int>.Valid(x * 10));
// → válido con [10, 20, 30]
```

**Caso real: importación de un CSV de clientes.**

```csharp
public MlResult<IEnumerable<Cliente>> ImportarClientes(IEnumerable<FilaCsv> filas)
    => filas.Projection((fila, indice) =>
        EnsureFp.NotNullEmptyOrWhitespace(fila.Nombre, $"Línea {indice + 1}: el nombre es obligatorio.")
            .Bind(_ => EnsureFp.NotNullEmptyOrWhitespace(fila.Email, $"Línea {indice + 1}: el email es obligatorio.))
            .MapEnsure(_ => fila.Email.Contains('@'),  $"Línea {indice + 1}: el email '{fila.Email}' no es válido.")
            .Map(_ => new Cliente(fila.Nombre.Trim(), fila.Email.ToLowerInvariant())));

// Uso:
var importacion = ImportarClientes(filas);

var respuesta = importacion.Match(
    valid: clientes => $"Importados {clientes.Count()} clientes correctamente.",
    fail:  errores  => $"No se ha importado nada. Corrige estos errores:\n{errores}"
);

// Si las líneas 3 y 7 están mal, el error contiene AMBOS mensajes:
//   Línea 3: el email es obligatorio.
//   Línea 7: el email 'pepe.com' no es válido.
```

Observa el detalle importante: **no se importa nada** si hay un solo error. Eso es lo correcto cuando el fichero representa una unidad de trabajo (una remesa, un asiento contable) y una importación parcial dejaría los datos incoherentes.

### `ProjectionWhile`

Recorre la colección **hasta el primer fallo** y se detiene ahí. Los elementos posteriores ni se evalúan.

Se usa cuando **el orden importa y los pasos dependen entre sí**, o cuando seguir procesando después de un error sería inútil o incluso peligroso. También es la opción eficiente cuando cada elemento cuesta mucho (una llamada externa por elemento) y no tiene sentido gastar el resto de llamadas si ya sabemos que el proceso va a fallar.

```csharp
var numeros = new[] { 1, 2, 3, 4 };

var result = numeros.ProjectionWhile(x => x < 3
    ? MlResult<int>.Valid(x)
    : MlResult<int>.Fail($"El valor {x} no es válido."));

// → fail con "El valor 3 no es válido."
//   El elemento 4 NUNCA se evalúa.
```

**Caso real: aplicación de movimientos contables sobre un saldo.**

Aquí cada movimiento parte del saldo resultante del anterior, así que en cuanto uno es inválido el resto carece de sentido:

```csharp
decimal saldo = 1_000m;

var result = movimientos.ProjectionWhile((mov, indice) =>
    EnsureFp.That(mov, saldo + mov.Importe >= 0,
                  $"Movimiento {indice + 1} ({mov.Concepto}): saldo insuficiente. " +
                  $"Saldo actual {saldo:N2} €, importe {mov.Importe:N2} €.")
        .Map(m =>
        {
            saldo += m.Importe;                       // el estado avanza paso a paso
            return new Apunte(m.Concepto, m.Importe, saldo);
        }));

var mensaje = result.Match(
    valid: apuntes => $"Aplicados {apuntes.Count()} movimientos. Saldo final: {saldo:N2} €",
    fail:  errores => $"Proceso detenido: {errores}"
);
```

Con `Projection` este mismo código sería incorrecto: seguiría aplicando movimientos sobre un saldo que ya sabemos inconsistente y generaría una cascada de errores derivados carentes de valor.

### `ProjectionParallelAsync`

Lanza la transformación de **todos** los elementos a la vez (`Task.WhenAll`) y espera a que terminen todas. La política de error es la de `Projection`: acumula y fusiona.

Es la herramienta indicada cuando el coste de cada elemento es **espera de I/O** —una llamada HTTP, una consulta a base de datos, una lectura de fichero— porque el tiempo total pasa de ser la *suma* de todas las llamadas a ser aproximadamente el de **la más lenta**.

```csharp
var ids = new[] { 1, 2, 3 };

var result = await ids.ProjectionParallelAsync(async id =>
{
    await Task.Delay(50);                       // simula una llamada externa
    return MlResult<int>.Valid(id * 2);
});

// → válido con [2, 4, 6] en ~50 ms en lugar de ~150 ms.
```

**Caso real: enriquecer un catálogo con precios de un servicio externo.**

```csharp
public async Task<MlResult<IEnumerable<ProductoConPrecio>>> EnriquecerAsync(IEnumerable<Producto> productos)
    => await productos.ProjectionParallelAsync(async producto =>
        await ObtenerPrecioAsync(producto.Sku)                        // Task<MlResult<decimal>>
            .MapEnsureAsync(precio => precio > 0,
                            $"El precio recibido para el SKU {producto.Sku} no es válido.")
            .MapAsync(precio => new ProductoConPrecio(producto, precio)));

// 200 productos → 200 llamadas concurrentes en lugar de 200 llamadas secuenciales.
// Si 3 SKU no tienen precio, el resultado es fail con los 3 motivos identificados por SKU.
```

⚠️ **Precauciones al usar la versión paralela:**
```
- La lambda debe ser **segura frente a concurrencia**: no mutes variables compartidas (como el `saldo` del ejemplo anterior) ni uses un `DbContext` de EF Core desde varias tareas a la vez.
- El **orden de ejecución no está garantizado** (el orden del resultado sí se corresponde con el de las tareas lanzadas).
- Si cada elemento consume un recurso limitado (conexiones, cuota de una API), lanzar cientos de tareas de golpe puede ser contraproducente; en ese caso es preferible `ProjectionAsync` secuencial o trocear la colección en bloques.
```

### `ProjectionSplit`

Devuelve **dos diccionarios**: uno con los elementos que han ido bien (`valids`, clave = elemento original, valor = resultado) y otro con los que han fallado (`fails`, clave = elemento original, valor = su `MlErrorsDetails`).

Su particularidad más importante es que **el resultado global es válido incluso si hay elementos fallidos**: el fallo de un elemento no es un fallo del proceso, sino un dato del informe final. Por eso es la opción natural para procesos **tolerantes a errores parciales**, donde interesa avanzar con lo que se puede y dar cuenta del resto.

```csharp
var numeros = new[] { 1, 2, 3, 4 };

var result = numeros.ProjectionSplit(x => x % 2 == 0
    ? MlResult<int>.Valid(x)
    : MlResult<int>.Fail($"{x} es impar."));

var split = result.SecureValidValue();

Console.WriteLine(split.valids.Count);   // 2  → los pares 2 y 4
Console.WriteLine(split.fails.Count);    // 2  → los impares 1 y 3
```

**Caso real: sincronización nocturna tolerante a fallos.**

Un proceso batch que se ejecuta de madrugada no debe abortarse porque 5 registros de 10.000 vengan mal: debe importar los 9.995 correctos y dejar un informe de los 5 rechazados para revisión.

```csharp
public async Task<InformeSincronizacion> SincronizarAsync(IEnumerable<RegistroExterno> registros)
{
    var resultado = registros.ProjectionSplit(registro =>
        ValidarRegistro(registro)                  // MlResult<RegistroExterno>
            .Map(r => r.ToEntidad())
            .AddValueDetailIfFail(registro.Id));   // deja constancia de QUÉ registro falló

    var split = resultado.SecureValidValue();

    // 1) Persistimos únicamente lo válido.
    await _repo.GuardarAsync(split.valids.Values);

    // 2) Informamos con detalle de lo rechazado, registro a registro.
    foreach (var (registro, errores) in split.fails)
    {
        _log.Warn($"Registro {registro.Id} rechazado: {errores}");
    }

    return new InformeSincronizacion(
        Importados: split.valids.Count,
        Rechazados: split.fails.Count,
        Detalle:    split.fails.ToDictionary(f => f.Key.Id, f => f.Value.ToString()));
}

// Salida típica: "Importados: 9995, Rechazados: 5"
```

Fíjate en que la clave del diccionario es **el elemento original**, no un índice: eso permite saber exactamente *qué* entrada ha fallado y por qué, sin tener que volver a cruzar posiciones con la colección de origen.

> Requisito: el tipo `T` de la colección debe ser `notnull`, porque se usa como clave del diccionario. Los elementos nulos de la colección de origen se descartan.

### `FusionFailErros` y `FusionErrosIfExists`

Estos dos métodos trabajan sobre una colección que **ya es** `IEnumerable<MlResult<T>>` —es decir, cuando tú mismo has generado los resultados y sólo necesitas consolidarlos en uno. Son las piezas que usan internamente los métodos `Projection*`, pero resultan muy útiles por separado.

| Método | Si hay fallos | Si no hay fallos |
|--------|---------------|------------------|
| `FusionFailErros` | devuelve `fail` con **todos** los errores fusionados | (se usa sabiendo que hay fallos) |
| `FusionErrosIfExists` | devuelve `fail` con **todos** los errores fusionados | devuelve `valid` con **todos** los valores |

`FusionErrosIfExists` es el que se usa habitualmente en código de aplicación, porque cubre los dos casos:

```csharp
var results = new[]
{
    MlResult<int>.Valid(1),
    MlResult<int>.Fail("error 1"),
    MlResult<int>.Fail("error 2")
};

var merged = results.FusionErrosIfExists();
// → fail con "error 1" y "error 2" fusionados en un único MlErrorsDetails.
```

**Caso real: validación por reglas independientes.**

Cuando las reglas de negocio están modeladas como funciones sueltas, se aplican todas y se consolidan al final. Así el usuario recibe el informe completo en una sola respuesta:

```csharp
public MlResult<Pedido> ValidarPedido(Pedido pedido)
{
    var validaciones = new[]
    {
        EnsureFp.NotEmpty(pedido.Lineas,              "El pedido debe tener al menos una línea."),
        EnsureFp.That(pedido, pedido.ClienteId > 0,   "El pedido debe tener un cliente asignado."),
        EnsureFp.That(pedido, pedido.Total > 0,       "El total del pedido debe ser positivo."),
        EnsureFp.That(pedido, pedido.Fecha <= DateTime.Today,
                                                      "La fecha del pedido no puede ser futura.")
    }
    .Select(r => r.Map(_ => pedido));                 // homogeneizamos el tipo a MlResult<Pedido>

    return validaciones.FusionErrosIfExists()         // MlResult<IEnumerable<Pedido>>
                       .Map(_ => pedido);             // volvemos al pedido original
}

// Si falla el cliente y el total, el error contiene los dos mensajes, no sólo el primero.
```

Y una nota práctica sobre la elección entre familias: cuando las validaciones **se aplican al mismo objeto**, `BindMulti` (sección 4) suele ser más directo; `FusionErrosIfExists` brilla cuando los resultados **provienen de sitios distintos** (varias consultas, varios servicios, un bucle propio) y hay que consolidarlos.

### Guía rápida de decisión

```csharp
// Todo o nada, informando de todos los errores  → Projection
var a = filas.Projection(ValidarFila);

// Secuencial y dependiente, parar en el primer error → ProjectionWhile
var b = movimientos.ProjectionWhile(Aplicar);

// I/O independiente por elemento, en paralelo → ProjectionParallelAsync
var c = await skus.ProjectionParallelAsync(ConsultarPrecioAsync);

// Tolerante: guarda lo bueno, informa de lo malo → ProjectionSplit
var d = registros.ProjectionSplit(Convertir);

// Consolidar resultados que ya tengo → FusionErrosIfExists
var e = resultadosSueltos.FusionErrosIfExists();
```

Todas las variantes disponen de su versión `*Async` (`ProjectionAsync`, `ProjectionWhileAsync`, `ProjectionSplitAsync`, `FusionFailErrosAsync`, `FusionErrosIfExistsAsync`) y aceptan tanto una colección síncrona como un `Task<IEnumerable<T>>` de entrada, de modo que encajan sin fricción en cualquier punto de una cadena asíncrona.

---

## 12. Extensiones genéricas de ayuda

`MoralesLarios.OOFP.Helpers.Extensions` agrupa pequeñas utilidades transversales que no pertenecen conceptualmente a `MlResult<T>` pero que aparecen constantemente al escribir código funcional: validar un objeto con DataAnnotations, adaptar delegados síncronos a asíncronos, enriquecer un diccionario de detalles con una excepción o inicializar un objeto de forma fluida.

### `ValidateObject`

Ejecuta la validación de **DataAnnotations** sobre cualquier objeto y devuelve la lista de `ValidationResult` obtenidos. No lanza excepciones: si no hay errores, devuelve una colección vacía.

```csharp
using System.ComponentModel.DataAnnotations;

public class Person
{
    [Required, MinLength(2)]
    public string Name { get; set; } = default!;
}

var errors = new Person { Name = "A" }.ValidateObject();

foreach (var e in errors)
    Console.WriteLine(e.ErrorMessage);
// The field Name must be a string or array type with a minimum length of '2'.
```

Su verdadera utilidad aparece al combinarla con `MlResult`, convirtiendo la validación declarativa por atributos en un resultado funcional en tres líneas:

```csharp
public MlResult<T> Validar<T>(T objeto) where T : notnull
{
    var errores = objeto.ValidateObject()
                        .Select(v => v.ErrorMessage ?? "Error de validación desconocido.")
                        .ToList();

    return errores.Any()
        ? MlResult<T>.Fail(MlErrorsDetails.FromEnumerableStrings(errores))
        : MlResult<T>.Valid(objeto);
}

// Uso: los atributos definen las reglas y el flujo funcional las transporta.
var resultado = Validar(new Person { Name = "A" })
    .Bind(persona => Guardar(persona));
```

> El proyecto `MoralesLarios.OOFP.Validation.Dataannotations` industrializa precisamente este patrón; esta extensión es la pieza de bajo nivel sobre la que se apoya.

### `ToNullable`

Convierte un tipo por valor (`struct`) en su equivalente `Nullable<T>`. Es un método de conveniencia que evita casts explícitos y ayuda a que las expresiones genéricas encajen sin ruido sintáctico.

```csharp
int  numero    = 5;
int? anulable  = numero.ToNullable();   // int → int?

Console.WriteLine(anulable.HasValue);   // True
```

Resulta especialmente cómodo al construir objetos cuyas propiedades son opcionales o al alimentar métodos que esperan tipos anulables sin tener que declarar variables intermedias.

### `AppendExDetails`

Añade una excepción al diccionario de detalles de error **sin sobrescribir las que ya estuvieran registradas**: si la clave de excepción ya existe, genera una nueva numerada (`ExDescription`, `ExDescription2`, `ExDescription3`…). Además, devuelve una **copia** del diccionario, por lo que no muta el original.

```csharp
var details = new Dictionary<string, object>();

details = details.AppendExDetails(new InvalidOperationException("primera"));
details = details.AppendExDetails(new TimeoutException("segunda"));

// El diccionario conserva AMBAS excepciones, cada una con su propia clave.
```

Es la mecánica que permite que un error acumule el rastro completo cuando atraviesa varias capas y cada una aporta su excepción, en lugar de quedarse sólo con la última —que suele ser la menos informativa.

### `With` y `WithAsync`

Aplican una serie de acciones de configuración sobre un objeto y **devuelven el propio objeto**, lo que permite inicializar y encadenar en una sola expresión.

```csharp
var person = new Person().With(
    p => p.Name = "Luis",
    p => p.Age  = 30
);

Console.WriteLine($"{person.Name} - {person.Age}");   // Luis - 30
```

`WithAsync` es la variante para flujos asíncronos y acepta tanto un objeto como un `Task<T>` de entrada, de forma que se puede intercalar en medio de una cadena `await`:

```csharp
var pedido = await _repo.GetPedidoAsync(id)
    .WithAsync(p => p.FechaRevision = DateTime.UtcNow,
               p => p.Revisado      = true);
```

⚠️ **Importante:** `With` **muta el objeto recibido** (no crea una copia). Es perfecto para configurar instancias recién creadas o entidades que estás a punto de guardar, pero no lo uses como si fuera un `with` de records: para copias inmutables emplea la expresión `with` propia de C#.

### `VoidToAsync`

Adapta una `Action<T>` para que pueda usarse donde se espera un delegado asíncrono: ejecuta la acción y devuelve una `Task` ya completada.

```csharp
await 5.VoidToAsync(x => Console.WriteLine($"Valor: {x}"));   // Valor: 5
```

Sirve para evitar el molesto `async x => { Hacer(x); await Task.CompletedTask; }` cuando una sobrecarga asíncrona es la única disponible y tu efecto secundario es, en realidad, síncrono.

### `ToFuncTask`

Convierte delegados síncronos en su equivalente asíncrono. Existen varias sobrecargas que cubren los casos habituales del núcleo:

| Sobrecarga | Convierte | En |
|------------|-----------|-----|
| `ToFuncTask<T, TResult>` | `Func<T, TResult>` | `Func<T, Task<TResult>>` |
| `ToFuncTask<TResult>` | `Func<MlErrorsDetails, TResult>` | `Func<MlErrorsDetails, Task<TResult>>` |
| `ToFuncTask<T>` | `Action<T>` | `Func<T, Task>` |
| `ToFuncTask` | `Action<MlErrorsDetails>` | `Func<MlErrorsDetails, Task>` |
| `ToFuncTask` | `Action` | `Func<Task>` |

```csharp
Func<int, string> f      = x => $"valor {x}";
Func<int, Task<string>> g = f.ToFuncTask();

Console.WriteLine(await g(7));   // valor 7
```

Su utilidad práctica es **reutilizar lógica síncrona ya existente dentro de cadenas asíncronas** sin duplicarla ni envolverla a mano en cada punto de uso:

```csharp
Func<Pedido, PedidoDto> mapear = p => p.ToDto();       // función síncrona ya escrita

var dto = await ObtenerPedidoAsync(id)
    .MapAsync(mapear.ToFuncTask());                    // encaja en la cadena async
```

**Resumen de la familia:**
```
| Extensión | Se aplica sobre | Para qué sirve |
|-----------|-----------------|----------------|
| `ValidateObject` | cualquier objeto | validar con DataAnnotations sin excepciones. |
| `ToNullable` | `struct` | obtener el equivalente `Nullable<T>`. |
| `AppendExDetails` | `Dictionary<string, object>` | acumular excepciones en los detalles del error. |
| `With` / `WithAsync` | cualquier clase | configurar e inicializar de forma fluida. |
| `VoidToAsync` | cualquier valor + `Action<T>` | ejecutar un efecto síncrono donde se espera `Task`. |
| `ToFuncTask` | `Func<>` / `Action<>` | adaptar delegados síncronos a firmas asíncronas. |
```

---

## 13. Patrón recomendado

El estilo de trabajo que propone la librería sigue siempre el mismo orden. No es una imposición arbitraria: cada fase tiene una responsabilidad clara y respetar la secuencia es lo que hace que el código resulte legible sin comentarios.

```text
1. Validar        → EnsureFp / MapEnsure / NullToFailed      (¿puedo empezar?)
2. Transformar    → Map / TryMap                              (adapto los datos)
3. Encadenar      → Bind / BindAsync / TryBind                (pasos que pueden fallar)
4. Observar       → ExecSelf / ExecSelfAsync                   (logs, métricas, eventos)
5. Cerrar         → Match / MatchAsync                         (una sola vez, al final)
```

Un ejemplo mínimo que ilustra la mecánica:

```csharp
var result = EnsureFp.That(10, 10 > 0, "Debe ser positivo")
    .Map(x  => x + 1)
    .Bind(x => MlResult<int>.Valid(x * 2))
    .Match(
        valid: x => $"OK: {x}",          // OK: 22
        fail:  e => $"ERROR: {e}"
    );
```

Y el mismo patrón aplicado a un caso completo de aplicación, donde se aprecia por qué el orden importa:

```csharp
public async Task<IActionResult> CrearPedido(CrearPedidoRequest request)
    => await EnsureFp.NotNull(request, "La petición no puede estar vacía.")           // 1. validar

        .BindMulti(                                                                   // 1. validar (todo junto)
            r => EnsureFp.That(r.ClienteId, r.ClienteId > 0, "Cliente no válido."),
            r => EnsureFp.NotEmpty(r.Lineas,                 "El pedido no tiene líneas."))

        .Map(r => r.ToDominio())                                                      // 2. transformar

        .BindAsync(async pedido => await _clientes.ExisteAsync(pedido.ClienteId)       // 3. encadenar
                                        .MapAsync(_ => pedido))
        .BindAsync(async pedido => await _stock.ReservarAsync(pedido.Lineas)           // 3. encadenar
                                        .MapAsync(_ => pedido))
        .BindAsync(async pedido => await _repo.GuardarAsync(pedido))                   // 3. encadenar

        .ExecSelfAsync(                                                                // 4. observar
            async pedido  => await _bus.PublicarAsync(new PedidoCreado(pedido.Id)),
            async errores => await _log.WarnAsync($"Pedido rechazado: {errores}"))

        .MatchAsync(                                                                   // 5. cerrar
            validAsync: pedido  => Task.FromResult<IActionResult>(Ok(pedido.ToDto())),
            failAsync:  errores => Task.FromResult<IActionResult>(
                                       BadRequest(new { errores = errores.ToString() })));
```

Cinco reglas prácticas que se derivan de este patrón:

1. **Valida cuanto antes.** Cuanto más arriba se detecte un dato inválido, menos trabajo inútil se ejecuta y más claro es el mensaje de error.
2. **Un solo `Match`, y al final.** Varios `Match` intermedios son la señal más habitual de que falta un `Bind`.
3. **`Try*` sólo donde puede lanzarse una excepción.** Envolver todo en `Try*` oculta qué partes son realmente peligrosas.
4. **Los efectos secundarios, en `ExecSelf`.** Nunca dentro de un `Map`: `Map` debe ser una transformación pura y predecible.
5. **Enriquece el error, no lo sustituyas.** Usa `AddValueDetailIfFail` / `AddMlErrorDetailIfFail` para añadir contexto conservando el mensaje original.

---

## 14. Cuándo usar cada familia

Esta tabla resume, en una sola vista, la decisión que hay que tomar en cada punto de una cadena funcional:

| Necesidad | Familia recomendada | Método típico |
|-----------|---------------------|---------------|
| Comprobar precondiciones al entrar en un método | `EnsureFp` | `NotNull`, `NotEmpty`, `That` |
| Convertir un `null` o una colección vacía en fallo | `MlResultActionsSeveral` | `NullToFailed`, `EmptyToFailed` |
| Adaptar una API que devuelve `bool` | `MlResultActionsSeveral` | `BoolToResult` |
| Encadenar un paso que **puede fallar** | `Bind` | `Bind`, `BindAsync`, `TryBind` |
| Transformar un valor que **no puede fallar** | `Map` | `Map`, `MapAsync` |
| Parsear o convertir con riesgo de excepción | `Map` protegido | `TryMap` |
| Aplicar una regla de negocio en medio de la cadena | `Map` | `MapEnsure` |
| Validar varias reglas del mismo objeto y ver **todos** los errores | `Bind` | `BindMulti` |
| Registrar trazas, métricas o eventos | `ExecSelf` | `ExecSelf`, `ExecSelfAsync`, `TryExecSelf` |
| Añadir contexto de diagnóstico al error | `MlResultActions` | `AddValueDetailIfFail` |
| Combinar dos resultados independientes | `MlResultActions` | `CreateCompleteMlResult` |
| Procesar una colección (todo o nada) | `MlResultBucles` | `Projection` |
| Procesar una colección parando al primer fallo | `MlResultBucles` | `ProjectionWhile` |
| Procesar una colección con I/O en paralelo | `MlResultBucles` | `ProjectionParallelAsync` |
| Procesar una colección tolerante a fallos parciales | `MlResultBucles` | `ProjectionSplit` |
| Consolidar resultados ya obtenidos | `MlResultBucles` | `FusionErrosIfExists` |
| Reutilizar delegados o métodos existentes | `MlResultTransformations` | `ToMlResult`, `TryToMlResultAsync` |
| Cerrar el flujo y devolver una respuesta | `Match` | `Match`, `MatchAsync` |

Y los tres errores más frecuentes al empezar:

- **Usar `Map` donde toca `Bind`** → aparece un `MlResult<MlResult<T>>`. Si tu lambda devuelve `MlResult`, usa `Bind`.
- **Leer `Value` directamente** → si el resultado está en fallo, `Value` no es fiable. Usa `Match` (o `SecureValidValue()` sólo en tests).
- **Hacer `Match` en medio de la cadena** para volver a construir otro `MlResult` después → sustitúyelo por un `Bind`.

---

## 15. Documentación adicional

### Punto de entrada: el índice maestro

📘 **[Introducción general y índice completo de la documentación](./__Doc/1_Intro.md)** —
filosofía, convención de nombres y el
[mapa de los 48 documentos de `__Doc/`](./__Doc/1_Intro.md#índice-completo-de-la-documentación).

### Referencia por archivo de código (`__Doc/Types/`)

| Documento | Contenido |
|-----------|-----------|
| [Índice de tipos](./__Doc/Types/README.md) | Portada de la referencia por archivo |
| [`MlResult`](./__Doc/Types/MlResult.md) | El tipo raíz, fábricas y conversiones implícitas |
| [Modelo de errores](./__Doc/Types/MlResultErrors.md) | `MlError`, `MlErrorsDetails`, `ErrorMessage` |
| [`MlResultActions`](./__Doc/Types/MlResultActions.md) | Enriquecer errores, transportar datos, acceso seguro |
| [`MlResultActionsBind`](./__Doc/Types/MlResultActionsBind.md) | Todas las sobrecargas de `Bind*` |
| [`MlResultActionsMap`](./__Doc/Types/MlResultActionsMap.md) | Todas las sobrecargas de `Map*` |
| [`MlResultActionsMatch`](./__Doc/Types/MlResultActionsMatch.md) | Todas las sobrecargas de `Match*` |
| [`MlResultActionsExecSelf`](./__Doc/Types/MlResultActionsExecSelf.md) | Todas las sobrecargas de `ExecSelf*` |
| [`MlResultActionsSeveral`](./__Doc/Types/MlResultActionsSeveral.md) | `EmptyToFailed`, `NullToFailed`, `BoolToResult`, `Combine`, `Do` |
| [`MlResultActionsErrorsDetails`](./__Doc/Types/MlResultActionsErrorsDetails.md) | Leer y fusionar el diccionario `Details` |
| [`MlResultBucles`](./__Doc/Types/MlResultBucles.md) | `Projection*`, `ProjectionSplit*`, `Fusion*` |
| [`MlResultTransformations`](./__Doc/Types/MlResultTransformations.md) | `ToMlResult*`, `TryToMlResult*`, boxing |
| [`MlResultChangeReturnResult`](./__Doc/Types/MlResultChangeReturnResult.md) | Cambiar el tipo de retorno conservando el estado |

### Guías por concepto

**`Bind` — encadenar operaciones que devuelven `MlResult`**

- [`3_Bind`](./__Doc/Bind/3_Bind.md) ⭐ · [`2_MlResultActions`](./__Doc/Bind/2_MlResultActions.md) ·
  [`4_BindMulti`](./__Doc/Bind/4_BindMulti.md) · [`5_BindIf`](./__Doc/Bind/5_BindIf.md)
- Recuperación: [`6_BindIfFail`](./__Doc/Bind/6_BindIfFail.md) ·
  [`7_BindIfFailWithValue`](./__Doc/Bind/7_BindIfFailWithValue.md) ·
  [`8_BindIfFailWithException`](./__Doc/Bind/8_BindIfFailWithException.md) ·
  [`9_BindIfFailWithoutException`](./__Doc/Bind/9_BindIfFailWithoutException.md)
- [`10_BindAlways`](./__Doc/Bind/10_BindAlways.md) ·
  [`11_BindSaveValueInDetails…`](./__Doc/Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md)

**`Map` — transformar el valor sin salir del carril**

- [`1_Map`](./__Doc/Map/1_Map.md) ⭐ · [`2_MapEnsure`](./__Doc/Map/2_MapEnsure.md) ·
  [`3_MapIf`](./__Doc/Map/3_MapIf.md)
- Reserva ante fallo: [`4_MapIfFail`](./__Doc/Map/4_MapIfFail.md) ·
  [`5_MapIfFailWithValue`](./__Doc/Map/5_MapIfFailWithValue.md) ·
  [`6_MapIfFailWithException`](./__Doc/Map/6_MapIfFailWithException.md) ·
  [`7_MapIfFailWithoutException`](./__Doc/Map/7_MapIfFailWithoutException.md)
- [`8_MapAlways`](./__Doc/Map/8_MapAlways.md)

**`Match` — salir del carril**

- [`1_Match`](./__Doc/Match/1_Match.md) ⭐ · [`2_MatchAll`](./__Doc/Match/2_MatchAll.md)

**`ExecSelf` — efectos laterales sin alterar el resultado**

- [`1_ExecSelf`](./__Doc/ExecSelf/1_ExecSelf.md) ⭐ ·
  [`2_ExecSelfIfValid`](./__Doc/ExecSelf/2_ExecSelfIfValid.md) ·
  [`3_ExecSelfIfFail`](./__Doc/ExecSelf/3_ExecSelfIfFail.md)
- [`4_ExecSelfIfFailWithValue`](./__Doc/ExecSelf/4_ExecSelfIfFailWithValue.md) ·
  [`5_ExecSelfIfFailWithException`](./__Doc/ExecSelf/5_ExecSelfIfFailWithException.md) ·
  [`6_ExecSelfIfFailWithoutException`](./__Doc/ExecSelf/6_ExecSelfIfFailWithoutException.md)

**`Several` — puentes desde el mundo imperativo**

- [`1_EmptyToFailed`](./__Doc/Several/1_EmptyToFailed.md) ·
  [`2_NullToFailed`](./__Doc/Several/2_NullToFailed.md) ·
  [`3_BoolToResult`](./__Doc/Several/3_BoolToResult.md) ·
  [`4_Combine`](./__Doc/Several/4_Combine.md) ⚠️ (**no** acumula errores)

**Utilidades y colecciones**

- [`EnsureFp`](./__Doc/EnsureFp/EnsureFp.md) — precondiciones funcionales
- [`Transformations`](./__Doc/Transformations/Transformations.md) — entrar al carril desde código que lanza
- [`Extensions`](./__Doc/Extensions/Extensions.md) — `ToAsync`, `With`, `ToFuncTask`, `Constants`
- [`Bucles`](./__Doc/Bucle/Bucles.md) — proyecciones sobre colecciones

### Documentación de los proyectos del ecosistema

- [README general de la solución](../README.md)
- [MoralesLarios.OOFP.EFCore](../MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](../MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](../MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](../MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](../MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](../MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Utilities](../MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](../MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](../MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](../MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](../MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](../MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](../MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](../MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](../MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](../MoralesLarios.OOFP.WebServices/README.md)

---

## 16. Resumen

`MoralesLarios.OOFP` es el núcleo fundacional del ecosistema **MoralesLarios.FOOP**. Su idea
central cabe en una frase:

> **El error es un dato que viaja por la tubería, no una excepción que la interrumpe.**

### Las cinco piezas que hay que conocer

| Pieza | Para qué sirve | Documentación |
|-------|----------------|---------------|
| `MlResult<T>` | Contenedor de éxito con valor o fallo con errores | [`MlResult.md`](./__Doc/Types/MlResult.md) |
| `MlErrorsDetails` | Transporta los errores y el diccionario `Details` | [`MlResultErrors.md`](./__Doc/Types/MlResultErrors.md) |
| `EnsureFp` | Entrar al carril validando precondiciones | [`EnsureFp.md`](./__Doc/EnsureFp/EnsureFp.md) |
| `Bind` / `Map` | Avanzar por el carril (con y sin `MlResult` de vuelta) | [`3_Bind.md`](./__Doc/Bind/3_Bind.md) · [`1_Map.md`](./__Doc/Map/1_Map.md) |
| `Match` | Salir del carril y materializar la respuesta | [`1_Match.md`](./__Doc/Match/1_Match.md) |

### La convención de nombres, en una línea

```text
[Try] Operación [If | IfFail | IfFailWithValue | IfFailWithException | IfFailWithoutException] [Always] [Async]
```

- `Try*` → captura la excepción y la guarda en `Details["Ex"]`
- `*Async` → acepta y/o devuelve `Task<...>`
- `*IfFail*` → solo se ejecuta en la rama de fallo (recuperación)
- `*Always` → se ejecuta en ambas ramas

### Errores frecuentes que conviene recordar

1. **No leas `Value` directamente**: usa `Match` (o `SecureValidValue()` solo cuando ya has
   garantizado la validez).
2. **`Combine` no acumula errores**: cortocircuita. Si necesitas acumular, usa
   [`Projection`](./__Doc/Bucle/Bucles.md).
3. **`MlErrorsDetails` solo expone `Errors` y `Details`**: no existen `AllErrors`,
   `FirstErrorMessage` ni `HasException`.
4. **`ProjectionAsync` con delegado asíncrono es secuencial**: para paralelismo usa
   `ProjectionParallelAsync`.
5. **No hagas `Match` en medio de la cadena** para reconstruir otro `MlResult`: eso es un
   `Bind`.

### Por dónde seguir

- 📘 [Introducción general con el índice completo de la documentación](./__Doc/1_Intro.md)
- 📘 [Referencia archivo a archivo (`__Doc/Types/`)](./__Doc/Types/README.md)
- 📘 [README general de la solución](../README.md)

---

## Compatibilidad

- `.NET 9`
- `.NET 8`
