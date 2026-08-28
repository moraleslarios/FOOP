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

## 1. El tipo central: `MlResult<T>`

`MlResult<T>` representa dos estados posibles:

- `IsValid == true`: el flujo tiene un valor correcto disponible.
- `IsValid == false`: el flujo ha fallado y su detalle se encuentra en `ErrorsDetails`.

### Factories principales

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Types.Errors;

var ok = MlResult<int>.Valid(42);
var fail1 = MlResult<string>.Fail("No hay datos");
var fail2 = MlResult<int>.Fail(MlErrorsDetails.FromErrorMessage("La validación ha fallado"));

var empty = MlResult.Empty();
var discard = MlResult.Discard;
```

### Propiedades y conversiones implícitas

```csharp
MlResult<int> r1 = 42;
MlResult<int> r2 = MlResult<int>.Fail("error");
MlResult<int> r3 = "valor inválido";

Console.WriteLine(r1.IsValid); // True
Console.WriteLine(r2.IsFail);  // True
```

La librería ofrece conversiones implícitas para:

- `T` a `MlResult<T>`
- `MlError` a `MlResult<T>`
- `MlErrorsDetails` a `MlResult<T>`
- `IEnumerable<MlError>` a `MlResult<T>`

### Ejemplos de uso reales

```csharp
var result = MlResult<string>.Valid("Hola");

var text = result.Match(
    valid: x => $"OK: {x}",
    fail: errors => $"ERR: {errors}"
);

Console.WriteLine(text);
```

```csharp
var failure = MlResult<int>.Fail("El valor no es válido");

var message = failure.Match(
    valid: x => $"Valor: {x}",
    fail: errors => errors.ToString()
);
```

### `ToString()`

```csharp
var result = MlResult<int>.Fail("fallo 1");
Console.WriteLine(result);
```

Devuelve el valor correcto si es válido, o el texto del error si está en fallo.

---

## 2. `MlError` y `MlErrorsDetails`

### `MlError`

`MlError` es la pieza básica del error funcional. Normaliza el mensaje y ofrece una conversión implícita desde `string`.

```csharp
using MoralesLarios.OOFP.Types.Errors;

var error = MlError.FromErrorMessage("Usuario no encontrado");
var also = "Nombre requerido";

Console.WriteLine(error.Message);
Console.WriteLine(also.ToMlError());
```

`MlErrorExtensions` añade:

- `ToMlError()`
- `ToMlErrors()`

```csharp
IEnumerable<MlError> errors = "error 1".ToMlErrors();
```

### `MlErrorsDetails`

`MlErrorsDetails` contiene una colección de errores y un diccionario con detalles adicionales.

```csharp
var errors = MlErrorsDetails.FromErrorMessage("Fallo de validación");
var withValue = MlErrorsDetails.FromErrorMessageWithValue("Id no válido", 42);
var withDetail = MlErrorsDetails.FromErrorDetails("Fallo inesperado", "Exception", new InvalidOperationException("boom"));
```

### Factory methods reales

```csharp
var errors1 = MlErrorsDetails.FromEnumerableStrings(new[] { "e1", "e2" });
var errors2 = MlErrorsDetails.FromError(MlError.FromErrorMessage("error"));
var errors3 = MlErrorsDetails.FromErrorMessageDetails("mensaje", new Dictionary<string, object> { ["key"] = 123 });
```

### Conversión implícita

```csharp
MlErrorsDetails d1 = "mensaje simple";
MlErrorsDetails d2 = new[] { "a", "b" };
MlErrorsDetails d3 = MlError.FromErrorMessage("x");
```

### Formateo

```csharp
var error = MlErrorsDetails.FromErrorMessageWithValue("El registro no existe", 7);
Console.WriteLine(error.ToString());
```

---

## 3. `EnsureFp`: validaciones funcionales

`EnsureFp` es la capa de precondiciones del núcleo. Devuelve `MlResult<T>` y puede usarse antes de ejecutar lógica de negocio.

### `NotNull`

```csharp
string? nombre = null;

var result = EnsureFp.NotNull(nombre, "El nombre no puede ser nulo");
```

- Si `nombre` es `null`, devuelve `fail`.
- Si no, devuelve un `MlResult<string?>` válido.

### `NotEmpty`

```csharp
var result = EnsureFp.NotEmpty(new[] { 1, 2, 3 }, "La colección no puede estar vacía");
```

### `NotNullEmptyOrWhitespace`

```csharp
var result = EnsureFp.NotNullEmptyOrWhitespace("   ", "El texto es obligatorio");
```

### `That`

```csharp
var result = EnsureFp.That(10, 10 > 0, "Debe ser positivo");
```

`That` evalúa una condición booleana y devuelve `Valid(value)` o `Fail(error)`.

### `ThatAsync`

```csharp
var result = await EnsureFp.ThatAsync(25, 25 > 0, "Debe ser positivo");
```

### `NotNullAsync`, `NotEmptyAsync`, `NotNullEmptyOrWhitespaceAsync`

```csharp
var result = await EnsureFp.NotNullAsync("pepe", "El nombre es obligatorio");
var emptyOk = await EnsureFp.NotEmptyAsync(new[] { 1 }, "Debe haber elementos");
var textOk = await EnsureFp.NotNullEmptyOrWhitespaceAsync("Luis", "Texto obligatorio");
```

---

## 4. `Bind`: encadenar operaciones

`Bind` ejecuta la siguiente transformación solo si el origen es válido. Si falla, se propaga el error.

### `Bind`

```csharp
var result = MlResult<int>.Valid(5)
    .Bind(x => MlResult<int>.Valid(x * 2));

var text = result.Match(
    valid: x => $"Resultado: {x}",
    fail: e => $"Error: {e}"
);
```

### `BindAsync`

```csharp
var result = await MlResult<int>.Valid(10)
    .BindAsync(async x =>
    {
        await Task.Delay(20);
        return MlResult<int>.Valid(x + 5);
    });
```

### `TryBind`

```csharp
var result = MlResult<int>.Valid(12)
    .TryBind(
        x => throw new InvalidOperationException("falla simulada"),
        ex => $"Fallo al calcular: {ex.Message}"
    );
```

Si la lambda lanza, la excepción se captura y se convierte en `MlResult` erróneo.

### `BindMulti`

```csharp
var result = MlResult<int>.Valid(10)
    .BindMulti(
        x => MlResult<string>.Valid($"valor = {x}"),
        x => MlResult<string>.Valid($"doble = {x * 2}"),
        x => MlResult<string>.Valid($"triple = {x * 3}")
    );
```

La versión multi ejecuta varias validaciones y fusiona errores si alguna falla.

---

## 5. `Map`: transformar el valor válido

`Map` transforma el valor solo si el resultado actual es válido.

### `Map`

```csharp
var result = MlResult<string>.Valid("juan")
    .Map(x => x.ToUpperInvariant());
```

### `MapAsync`

```csharp
var result = await MlResult<string>.Valid("madrid")
    .MapAsync(async x =>
    {
        await Task.Delay(10);
        return x.ToUpperInvariant();
    });
```

### `TryMap`

```csharp
var result = MlResult<string>.Valid("hola")
    .TryMap(x => int.Parse(x), ex => $"No se pudo parsear: {ex.Message}");
```

### `MapEnsure`

```csharp
var result = MlResult<int>.Valid(5)
    .MapEnsure(x => x > 0, "El valor debe ser positivo");
```

Si falla la condición, se convierte a un `fail` con detalles.

---

## 6. `Match`: decidir por rama válida o errónea

`Match` es la operación más habitual para cerrar el flujo y decidir qué hacer en cada estado.

### `Match`

```csharp
var result = MlResult<int>.Valid(7);

var output = result.Match(
    valid: x => $"El valor es {x}",
    fail: errors => $"Hay error: {errors}"
);
```

### `MatchAsync`

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

### `TryMatch`

```csharp
var result = MlResult<int>.Valid(5)
    .TryMatch(
        valid: x => x.ToString(),
        fail: e => $"Error: {e}",
        errorMessageBuilder: ex => $"Excepción: {ex.Message}"
    );
```

---

## 7. `ExecSelf`: ejecutar efectos secundarios sin perder el flujo

`ExecSelf` ejecuta una acción y devuelve el mismo `MlResult` original. Es perfecto para logging, trazas o métricas.

### `ExecSelf`

```csharp
var result = MlResult<int>.Valid(10)
    .ExecSelf(
        x => Console.WriteLine($"Procesado: {x}"),
        e => Console.WriteLine($"Error: {e}")
    );
```

### `ExecSelfAsync`

```csharp
var result = await MlResult<string>.Valid("abc")
    .ExecSelfAsync(
        async x => await File.WriteAllTextAsync("out.txt", x),
        async e => await Console.Out.WriteLineAsync(e.ToString())
    );
```

### `TryExecSelf`

```csharp
var result = MlResult<int>.Valid(3)
    .TryExecSelf(
        x => throw new InvalidOperationException("boom"),
        e => Console.WriteLine($"Error: {e}"),
        ex => $"Se produjo: {ex.Message}"
    );
```

---

## 8. Utilidades de `MlResultActions`

La clase `MlResultActions` añade helpers para extender el resultado con detalles y datos extra.

### `AddMlErrorDetailIfFail` / `AddValueDetailIfFail`

```csharp
var result = MlResult<int>.Fail("No válido")
    .AddValueDetailIfFail(42);
```

Esto añade información contextual al detalle del error si el resultado está en estado `fail`.

### `CompleteWithDataValueIfValid`

```csharp
var result = MlResult<int>.Valid(5)
    .CompleteWithDataValueIfValid(x => x * 2);
```

### `CompleteWithDetailsValueIfFail`

```csharp
var result = MlResult<string>.Fail("error")
    .CompleteWithDetailsValueIfFail("contexto");
```

### `SecureValidValue` / `SecureFailErrorsDetails`

```csharp
var ok = MlResult<int>.Valid(99);
Console.WriteLine(ok.SecureValidValue());

var bad = MlResult<int>.Fail("error");
Console.WriteLine(bad.SecureFailErrorsDetails());
```

Este patrón lanza excepción si el flujo no está en el estado esperado, lo que ayuda a proteger acceso inseguro a datos.

### `CreateCompleteMlResult`

```csharp
var r1 = MlResult<int>.Valid(10);
var r2 = MlResult<string>.Valid("x");

var merged = r1.CreateCompleteMlResult(r2);
```

Devuelve un resultado con ambos valores cuando ambos son válidos; si cualquiera falla, devuelve errores fusionados.

---

## 9. `MlResultActionsSeveral`

Esta clase ofrece atajos para transformar entradas nulas, vacías o condicionales en `MlResult`.

### `NullToFailed`

```csharp
string? name = null;
var result = name.NullToFailed("El nombre es obligatiorio");
```

### `EmptyToFailed`

```csharp
var items = Enumerable.Empty<int>();
var result = items.EmptyToFailed("La colección está vacía");
```

### `BoolToResult`

```csharp
var result = 10.BoolToResult(10 > 0, "La condición no se cumple");
```

### `BoolToResult` sobre `bool`

```csharp
var result = true.BoolToResult("La condición no es válida");
```

---

## 10. `MlResultTransformations`

La clase `MlResultTransformations` convierte funciones o acciones normales a flujos `MlResult` sin romper el patrón.

### `ToMlResult`

```csharp
Func<int, int> square = x => x * x;
var result = square.ToMlResult(6);
```

### `TryToMlResult`

```csharp
Func<int, int> failFunc = x => int.Parse("oops");
var result = failFunc.TryToMlResult(1, ex => $"Error: {ex.Message}");
```

### `ToMlResultAsync`

```csharp
Func<int, Task<int>> op = async x =>
{
    await Task.Delay(20);
    return x + 1;
};

var result = await op.ToMlResultAsync(5);
```

### `TryToMlResultAsync`

```csharp
Func<int, Task<int>> bad = async _ =>
{
    await Task.Delay(10);
    throw new InvalidOperationException("bad");
};

var result = await bad.TryToMlResultAsync(2, ex => $"Error: {ex.Message}");
```

### `TryToMlResultErrors`

```csharp
var result = ((Action<MlErrorsDetails>)(e => Console.WriteLine(e))).TryToMlResultErrors<int>(
    MlErrorsDetails.FromErrorMessage("fallo"),
    ex => $"Error: {ex.Message}"
);
```

---

## 11. `MlResultBucles`: proyección y fusión con colecciones

`MlResultBucles` hace un trabajo muy útil con `IEnumerable<T>` cuando queremos transformar cada elemento y convertirlo en un resultado funcional.

### `Projection`

```csharp
var numbers = new[] { 1, 2, 3 };

var result = numbers.Projection(x =>
    x > 0
        ? MlResult<int>.Valid(x * 10)
        : MlResult<int>.Fail("negativo"));
```

Si cualquiera de los elementos falla, se fusionan todos los errores en una sola respuesta.

### `ProjectionWhile`

```csharp
var result = numbers.ProjectionWhile(x =>
    x < 3 ? MlResult<int>.Valid(x) : MlResult<int>.Fail("detener"));
```

Se para cuando aparece el primer fallo.

### `ProjectionParallelAsync`

```csharp
var result = await numbers.ProjectionParallelAsync(async x =>
{
    await Task.Delay(10);
    return MlResult<int>.Valid(x + 1);
});
```

### `ProjectionSplit`

```csharp
var result = new[] { 1, 2, 3, -1 }
    .ProjectionSplit(x => x > 0 ? MlResult<int>.Valid(x) : MlResult<int>.Fail("negativo"));
```

Devuelve dos diccionarios:

- `valids`
- `fails`

### `FusionFailErros` / `FusionErrosIfExists`

```csharp
var failures = new[]
{
    MlResult<int>.Fail("e1"),
    MlResult<int>.Fail("e2")
};

var merged = failures.FusionFailErros();
```

Funde todos los errores en un único detalle para que el consumidor reciba un único `fail` completo.

---

## 12. Extensiones genéricas de ayuda

La clase `Extensions` añade helpers útiles para validación, composición y adaptación de delegados.

### `ValidateObject`

```csharp
public class Person
{
    [MinLength(2)]
    public string Name { get; set; } = string.Empty;
}

var person = new Person { Name = "A" };
var results = person.ValidateObject();
```

### `ToNullable`

```csharp
int value = 42;
int? nullable = value.ToNullable();
```

### `AppendExDetails`

```csharp
var dict = new Dictionary<string, object> { ["key"] = "value" };
var extended = dict.AppendExDetails(new InvalidOperationException("boom"));
```

### `With` / `WithAsync`

```csharp
var person = new Person().With(
    x => x.Name = "Ana",
    x => x.Name = x.Name.Trim()
);
```

```csharp
var updated = await Task.FromResult(new Person())
    .WithAsync(x => x.Name = "Luis");
```

### `VoidToAsync`

```csharp
var task = "hola".VoidToAsync(x => Console.WriteLine(x));
```

### `ToFuncTask`

```csharp
Func<int, string> f = x => $"n = {x}";
Func<int, Task<string>> g = f.ToFuncTask();
```

También existe sobre `Action` y `Action<MlErrorsDetails>` para adapatar delegados a `Task`.

---

## 13. Patrón recomendado

El flujo típico en la librería es:

```csharp
using MoralesLarios.OOFP.Helpers;
using MoralesLarios.OOFP.Types;

var age = 18;

var result = EnsureFp.That(age, age >= 18, "Debes ser mayor de edad")
    .Map(x => x + 1)
    .Bind(x => MlResult<int>.Valid(x * 2));

var text = result.Match(
    valid: x => $"OK: {x}",
    fail: errors => $"ERROR: {errors}"
);
```

La secuencia suele ser:

1. `EnsureFp.*` valida la entrada.
2. `Map` transforma el valor válido.
3. `Bind` encadena otra operación funcional.
4. `Match` decide el resultado final.
5. `ExecSelf` puede registrar o emitir efectos secundarios sin romper el flujo.

---

## 14. Cuándo usar cada familia

- `Valid` / `Fail`: construir resultados explícitos.
- `Bind`: encadenar operaciones que ya devuelven `MlResult`.
- `Map`: transformar el valor del resultado cuando todo va bien.
- `Match`: cerrar el flujo y decidir el comportamiento final.
- `ExecSelf`: ejecutar eventos laterales manteniendo el mismo estado.
- `EnsureFp`: validar entrada y condicionantes.
- `MlErrorsDetails`: transportar detalles complejos de error.
- `Projection*`: transformar colecciones de forma segura.

---

## 15. Documentación adicional del núcleo

Estos enlaces complementan la guía principal y permiten profundizar en cada familia de métodos con ejemplos y explicaciones más concretas:

- [Intro general y filosofía técnica](./__Doc/1_Intro.md) — visión general del proyecto, diseño y principios del ecosistema.
- [Documentación por tipos](./__Doc/Types/README.md) — índice por archivo/clase principal del núcleo.
- [Tipos y resultados](./__Doc/Types/MlResult.md) — detalle del modelo básico de `MlResult` y `MlResult<T>`.
- [Bind](./__Doc/Bind/3_Bind.md) — encadenamiento de operaciones con propagación de errores.
- [Map](./__Doc/Map/1_Map.md) — transformaciones sobre valores válidos.
- [Match](./__Doc/Match/1_Match.md) — ramas de decisión según el estado del resultado.
- [ExecSelf](./__Doc/ExecSelf/1_ExecSelf.md) — ejecución de efectos secundarios sin destruir el flujo.
- [Several](./__Doc/Several/1_EmptyToFailed.md) — validación de colecciones vacías y casos de error por contenido.
- [EnsureFp](./__Doc/EnsureFp/EnsureFp.md) — validaciones y precondiciones funcionales.
- [Extensions](./__Doc/Extensions/Extensions.md) — utilidades auxiliares, validación data annotations y modificadores funcionales.
- [Transformations](./__Doc/Transformations/Transformations.md) — conversión de funciones y acciones normales a `MlResult`.
- [Bucles](./__Doc/Bucle/Bucles.md) — proyección, fusión y manejo de errores en colecciones.

### README de cada proyecto

- [MoralesLarios.OOFP.EFCore](./src/MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](./src/MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](./src/MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](./src/MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Utilities](./src/MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](./src/MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](./src/MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](./src/MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](./src/MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](./src/MoralesLarios.OOFP.WebServices/README.md)

---

## 16. Resumen

`MoralesLarios.OOFP` ofrece un estilo funcional muy claro: el flujo de negocio se expresa con `MlResult<T>`, los errores se encapsulan con `MlError` y `MlErrorsDetails`, y la composición se hace con `Bind`, `Map`, `Match`, `ExecSelf` y varias utilidades de extensión.

Este patrón hace que el código sea más explícito, más fácil de probar y más consistente en todo el ecosistema FOOP.

Para la capa de repositorios basada en EF Core, se recomienda consultar el README de la librería `MoralesLarios.OOFP.EFCore`. Allí se documenta el registro del repositorio, la herencia de clases base y el uso funcional real de cada repositorio.

---

## `ExecSelf`: ejecutar efectos secundarios sin perder el resultado

`ExecSelf` ejecuta una acción y devuelve el mismo resultado original. Es ideal para logging, trazas o métricas sin romper el flujo.

### `ExecSelf`

```csharp
var result = MlResult<int>.Valid(10)
    .ExecSelf(
        x => Console.WriteLine($"He procesado {x}"),
        e => Console.WriteLine($"Falló: {e}")
    );
```

Explicación: el resultado original se devuelve, pero también se ejecuta la acción adecuada.

### `ExecSelfAsync`

```csharp
var result = await MlResult<string>.Valid("abc")
    .ExecSelfAsync(
        async x => await File.WriteAllTextAsync("out.txt", x),
        async e => await Console.Out.WriteLineAsync(e.ToString())
    );
```

Explicación: útil para efectos laterales asíncronos.

### `TryExecSelf`

```csharp
var result = MlResult<int>.Valid(3)
    .TryExecSelf(
        x => throw new InvalidOperationException("boom"),
        e => Console.WriteLine($"Error: {e}"),
        ex => $"Se produjo: {ex.Message}"
    );
```

Explicación: la acción puede lanzar excepción y, aun así, el flujo sigue manejándose con `MlResult`.

---

## `EnsureFp`: validaciones funcionales

`EnsureFp` es la capa de precondiciones del núcleo. Se usa para comprobar condiciones y convertirlas en `MlResult<T>` de manera elegante.

### `NotNull`

```csharp
string? nombre = null;

var result = EnsureFp.NotNull(nombre, "El nombre no puede ser nulo");
```

Explicación: si `nombre` es `null`, devuelve `fail`; si no, devuelve el valor.

### `NotEmpty`

```csharp
var result = EnsureFp.NotEmpty(new[] { 1, 2, 3 }, "La colección no puede estar vacía");
```

Explicación: valida que la colección no sea nula ni vacía.

### `NotNullEmptyOrWhitespace`

```csharp
var result = EnsureFp.NotNullEmptyOrWhitespace("    ", "El texto es obligatorio");
```

Explicación: valida cadenas no nulas, no vacías y sin espacios en blanco.

### `That`

```csharp
var result = EnsureFp.That(10, x => x > 0, "Debe ser positivo");
```

Explicación: acepta una condición arbitraria para decidir si el valor es válido o no.

### `ThatAsync`

```csharp
var result = await EnsureFp.ThatAsync(25, x => x > 0, "Debe ser positivo");
```

Explicación: permite la validación en flujo asíncrono.

---

## `MlErrorsDetails`: errores estructurados

`MlErrorsDetails` es el contenedor del detalle del error. Puede contener:

- una colección de `MlError`
- un diccionario de detalles adicionales

### `FromErrorMessage`

```csharp
var e = MlErrorsDetails.FromErrorMessage("Usuario no encontrado");
```

Explicación: crea un error simple con un texto de mensaje.

### `FromErrorMessageWithValue`

```csharp
var e = MlErrorsDetails.FromErrorMessageWithValue("Id no válido", 42);
```

Explicación: añade información contextual del valor que falló.

### `FromErrorDetails`

```csharp
var e = MlErrorsDetails.FromErrorDetails(MlError.FromErrorMessage("Fallo técnico"), new Dictionary<string, object>
{
    ["key"] = "valor"
});
```

Explicación: incluye una estructura detallada del error.

### Conversión implícita

```csharp
MlErrorsDetails errors = "error 1";
MlErrorsDetails errors2 = new[] { "error 1", "error 2" };
```

Explicación: puedes pasar texto o colecciones de texto directamente a una variable `MlErrorsDetails` sin conversiones manuales.

### `ToString()`

```csharp
var error = MlErrorsDetails.FromErrorMessageWithValue("No existe el registro", 7);
Console.WriteLine(error);
```

Explicación: genera una salida legible con mensajes y detalles asociados.

---

## Patrón de uso recomendado

Este es el flujo típico en toda la solución:

```csharp
using MoralesLarios.OOFP.Helpers;
using MoralesLarios.OOFP.Types;

var age = 18;

var result = EnsureFp.That(age, x => x >= 18, "Debes ser mayor de edad")
    .Map(x => x + 1)
    .Bind(x => MlResult<int>.Valid(x * 2));

var texto = result.Match(
    valid: x => $"OK: {x}",
    fail: e => $"ERROR: {e}"
);
```

Explicación:

1. `EnsureFp.That` valida la precondición.
2. `Map` transforma el valor si la validación pasó.
3. `Bind` ejecuta otra operación funcional.
4. `Match` decide la salida final según si el resultado fue válido o no.

---

## Caso real: flujo con validación y consulta

```csharp
public static async Task<string> ProcesarUsuarioAsync(string? nombre)
{
    var result = await EnsureFp.NotNullEmptyOrWhitespaceAsync(nombre, "El nombre es obligatorio")
        .BindAsync(value => MlResult<string>.Valid(value.Trim()))
        .MapAsync(value => value.ToUpperInvariant())
        .MatchAsync(
            validAsync: async x =>
            {
                await Task.Delay(10);
                return $"Usuario procesado: {x}";
            },
            failAsync: async e =>
            {
                await Task.Delay(10);
                return $"Falló: {e}";
            }
        );

    return result;
}
```

Explicación: este patrón es el más habitual en la librería. Todo el flujo se mantiene en `MlResult`, sin necesidad de lanzar excepciones para controlar errores.

---

## Cuándo usar cada familia de métodos

- `Valid` / `Fail`: construir resultados explícitos.
- `Bind`: encadenar operaciones que ya devuelven `MlResult`.
- `Map`: transformar el valor de un resultado válido.
- `Match`: decidir el comportamiento final según el estado.
- `ExecSelf`: ejecutar efectos secundarios sin perder el flujo original.
- `EnsureFp`: validar entradas antes de seguir con la lógica.
- `MlErrorsDetails`: transportar detalles de error.

---

## En resumen

`MoralesLarios.OOFP` presenta un estilo funcional muy claro: todos los caminos de negocio pasan por `MlResult<T>`, y cada operación puede ser encadenada sin romper la semántica de éxito/error. El resultado es un código más predecible, más fácil de testear y más uniforme en todo el ecosistema FOOP.

La documentación técnica detallada de cada tipo y cada operación adicional se encuentra en la carpeta `__Doc` del proyecto.

---

### 4. Servicios de aplicaci�n

#### `MoralesLarios.OOFP.WebServices`
Capa funcional entre repositorio y web.

Aporta:

- `IGenServiceFp<TEntity, TDto>`
- `IGenServiceFp<TEntity, TRequest, TResponse>`
- `GenServiceFp<TEntity, TDto>`
- `GenServiceFp<TEntity, TRequest, TResponse>`
- `MlProblemsDetails`
- extensiones de registro para ciclo de vida cl�sico y duplex

?? [README del proyecto](./src/MoralesLarios.OOFP.WebServices/README.md)

---

### 5. Exposici�n web

#### `MoralesLarios.OOFP.WebApi`
Puente funcional entre `MlResult<T>` e `IActionResult`.

Aporta:

- `MlActionResults`
- `ExtendedProblemDetails`
- `ProblemDetailsInfo`
- `MlResultWebExtensionsPlus`
- `MlErrorsDetailsExtensions`
- helpers para headers del request

?? [README del proyecto](./src/MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores gen�ricos ASP.NET Core para CRUD est�ndar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar par�metros PK en Swagger/OpenAPI

?? [README del proyecto](./src/MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensi�n cacheada de los controladores gen�ricos.

Aporta:

- cach� por controlador
- invalidaci�n autom�tica en escrituras
- vaciado manual
- bypass din�mico
- soporte cl�sico y duplex
- soporte para PK compuesta

?? [README del proyecto](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)

---

### 6. Consumo HTTP

#### `MoralesLarios.OOFP.HttpClients`
Cliente HTTP funcional integrado con `MlResult<T>` y `IHttpClientFactory`.

Aporta:

- clientes tipados con PK simple
- clientes duplex request/response
- clientes para PK compuesta
- manager funcional sobre `IHttpClientFactory`
- helpers de cabeceras y respuestas HTTP

?? [README del proyecto](./src/MoralesLarios.OOFP.HttpClients/README.md)

---

## End-to-end: c�mo se usa este ecosistema

### Caso cl�sico de API

1. Modela el dominio con `ValueObjects`.
2. Valida con `Validation`.
3. Persiste con `EFCore`.
4. Exp�n l�gica con `WebServices`.
5. Publica con `WebControllers` y `WebApi`.
6. A�ade `WebControllers.Cache` si necesitas cach�.
7. Consume desde otro servicio con `HttpClients`.
8. Registra trazas con `Extensions.Loggers`.
9. Lee configuraci�n con `Utilities`.
10. Usa `IO` y `ValueObjects.IO` para operaciones de sistema de archivos.

### Ejemplo conceptual

```csharp
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
services.AddTransient(typeof(IEFRepoFp<>), typeof(EFRepoFp<>));
services.AddTransientGenServicesFpWithoutReposGeneral();
services.AddControllers();
```

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> service)
    : SimpleMlControllerBase<User, UserDto, int>(service) { }
```

```csharp
builder.Services.AddHttpClientsFp();
builder.Services.AddGenClientFp<IUsersClient, UsersClient>(
    configureClient: c => c.BaseAddress = new Uri("https://api.example.com/api/users/"));
```

---

## Proyectos de pruebas

La soluci�n tambi�n incluye una capa de validaci�n mediante proyectos de prueba unitarios e integraci�n:

- `MoralesLarios.OOFP.Unit.Tests`
- `MoralesLarios.OOFP.ValueObjects.Tests.Unit`
- `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit`
- `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit`
- `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit`
- `MoralesLarios.OOFP.WebApi.Tests.Unit`
- `MoralesLarios.OOFP.WebServices.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Integration`
- `MoralesLarios.OOFP.EFCore.Infrastructure.Tests`
- `MoralesLarios.OOFP.EFCore.Integration.Tests`
- `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests`

Estos proyectos sirven para verificar contratos, ejemplos reales de uso y escenarios de integraci�n entre capas.

---

## Documentaci�n adicional

### Documentaci�n ra�z del n�cleo OOFP

- [Intro general y filosof�a t�cnica](./__Doc/1_Intro.md)
- [Documentaci�n por tipos](./__Doc/Types/README.md)
- [Tipos y resultados](./__Doc/Types/MlResult.md)
- [Bind](./__Doc/Bind/3_Bind.md)
- [Map](./__Doc/Map/1_Map.md)
- [Match](./__Doc/Match/1_Match.md)
- [ExecSelf](./__Doc/ExecSelf/1_ExecSelf.md)
- [Several](./__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](./__Doc/EnsureFp/EnsureFp.md)
- [Extensions](./__Doc/Extensions/Extensions.md)
- [Transformations](./__Doc/Transformations/Transformations.md)
- [Bucles](./__Doc/Bucle/Bucles.md)

### README de cada proyecto

- [MoralesLarios.OOFP.EFCore](./src/MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](./src/MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](./src/MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](./src/MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Utilities](./src/MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](./src/MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](./src/MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](./src/MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](./src/MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](./src/MoralesLarios.OOFP.WebServices/README.md)

---

## Resumen ejecutivo

Si tuviera que describir la soluci�n en una sola frase, ser�a esta:

> **MoralesLarios.FOOP es un ecosistema .NET funcional para construir dominios, servicios, APIs y clientes con una sem�ntica com�n basada en `MlResult<T>`.**

Y si tuviera que destacar una sola pieza, esa ser�a:

> **`MoralesLarios.OOFP` es el n�cleo fundacional; el resto de proyectos ampl�an su valor hacia validaci�n, persistencia, web, cach�, HTTP, IO y configuraci�n.**

---

## Compatibilidad

La soluci�n est� organizada para proyectos objetivo de:

- `.NET 9`
- `.NET 8`

---

## Licencia y estilo de trabajo

La soluci�n est� pensada para ser usada como base de aplicaciones reales y para crecer por capas, manteniendo una misma forma de trabajo en todo el stack.

Si buscas una entrada r�pida para entender la librer�a, empieza por:

1. [Intro general de `MoralesLarios.OOFP`](./__Doc/1_Intro.md)
2. [README del proyecto principal](./src/MoralesLarios.OOFP.WebServices/README.md)
3. [WebApi](./src/MoralesLarios.OOFP.WebApi/README.md)
4. [WebControllers](./src/MoralesLarios.OOFP.WebControllers/README.md)
5. [HttpClients](./src/MoralesLarios.OOFP.HttpClients/README.md)

---

## Nota final

Este repositorio no es una �nica librer�a aislada, sino una **plataforma modular**. Cada proyecto tiene su propio README y, cuando aplica, su propia documentaci�n t�cnica enlazada desde `__Doc`.

La documentaci�n ra�z pretende ser la puerta de entrada oficial al ecosistema completo.
