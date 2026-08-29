# MoralesLarios.OOFP.Validation.Dataannotations — DataAnnotations sin excepciones

Puente entre los atributos clásicos de `System.ComponentModel.DataAnnotations` (`[Required]`, `[Range]`, `[EmailAddress]`, `[StringLength]`…) y el modelo funcional de la solución: en lugar de rellenar un `ModelState` o lanzar una `ValidationException`, **devuelve un [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)** listo para encadenar.

Permite reutilizar las anotaciones que ya tienen tus DTOs — sin reescribir reglas — y validarlas **fuera de ASP.NET Core**: en servicios, en trabajos en segundo plano, en importadores de ficheros o en tests.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [`DataannotationsValidator` — la fachada estática](#dataannotationsvalidator--la-fachada-estática)
5. [`Helpers.Extensions` — los métodos de extensión](#helpersextensions--los-métodos-de-extensión)
6. [Cómo se convierten los errores](#cómo-se-convierten-los-errores)
7. [Objetos individuales vs. colecciones](#objetos-individuales-vs-colecciones)
8. [Combinación con `MlValidableFp<T>`](#combinación-con-mlvalidablefpt)
9. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
10. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
11. [Ejemplos prácticos](#ejemplos-prácticos)
12. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
13. [Mejores prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

`Validator.TryValidateObject` funciona, pero su API es incómoda: hay que crear un `ValidationContext`, preparar una `List<ValidationResult>` por referencia, acordarse del cuarto parámetro `validateAllProperties: true` y luego traducir el resultado a lo que use tu aplicación.

❌ **Con la API cruda de .NET:**

```csharp
var contexto   = new ValidationContext(dto, null, null);
var resultados = new List<ValidationResult>();

if (! Validator.TryValidateObject(dto, contexto, resultados, true))   // ⚠️ el 'true' se olvida siempre
{
    var mensajes = resultados.Select(r => r.ErrorMessage).ToList();
    return BadRequest(mensajes);          // y aquí se rompe la cadena
}

var creado = _servicio.Crear(dto);        // ¿y si esto también puede fallar?
return Ok(creado);
```

✅ **Con este proyecto:**

```csharp
return DataannotationsValidator.Validate(dto)
    .Bind(v => _servicio.Crear(v))
    .Match(valid: creado  => Ok(creado),
           fail : errores => BadRequest(errores.ToErrorsMessages()));
```

> 💡 **La ganancia no es escribir menos líneas**, es que la validación por atributos **entra en el mismo raíl** que el resto del caso de uso: se encadena con `Bind`, se acumula con el resto de errores y se traduce a HTTP en un único `Match`.

Y de paso: `validateAllProperties: true` ya viene puesto, así que **no se te olvidará** (es el error más común al usar `TryValidateObject` a mano, porque sin él solo se evalúa `[Required]`).

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) | Proyecto base (y, transitivamente, el núcleo `MoralesLarios.OOFP`) |
| `System.ComponentModel.DataAnnotations` | Parte del framework: atributos y `Validator` |

```csharp
using MoralesLarios.OOFP.Validation.Dataannotations;          // DataannotationsValidator
using MoralesLarios.OOFP.Validation.Dataannotations.Helpers;  // métodos de extensión
using System.ComponentModel.DataAnnotations;                  // los atributos en tus DTOs
```

No requiere registro en el contenedor de dependencias: **todo es estático**.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Validation.Dataannotations/
├── DataannotationsValidator.cs        → fachada estática (6 sobrecargas de Validate/ValidateAsync)
└── Helpers/
    └── Extensions.cs                  → 7 métodos de extensión (el motor real)
```

Solo hay **dos ficheros de código** y una relación muy simple: `DataannotationsValidator` es una fachada que añade comprobaciones previas y delega en los métodos de extensión.

```
DataannotationsValidator.Validate(x)
   └─ EnsureFp.NotNull(x, …)
        └─ x.ValidateWithDataannotations()
             └─ x.ValidateObject()
                  └─ Validator.TryValidateObject(x, ctx, resultados, true)
```

---

## `DataannotationsValidator` — la fachada estática

```csharp
public static class DataannotationsValidator
{
    public static MlResult<T>                    Validate     <T>(T source);
    public static Task<MlResult<T>>              ValidateAsync<T>(T source);
    public static Task<MlResult<T>>              ValidateAsync<T>(Task<T> sourceAsync);

    public static MlResult<IEnumerable<T>>       Validate     <T>(IEnumerable<T> source);
    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(IEnumerable<T> source);
    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(Task<IEnumerable<T>> sourceAsync);
}
```

| Sobrecarga | Comprobaciones previas | Comportamiento |
|---|---|---|
| `Validate<T>(T)` | `EnsureFp.NotNull` | Valida un objeto; falla si es `null` |
| `Validate<T>(IEnumerable<T>)` | `NotNull` + `NotEmpty` | Valida todos los elementos y **fusiona** los errores |
| `ValidateAsync<T>(T)` | **ninguna** | Envuelve el resultado síncrono con `.ToAsync()` |
| `ValidateAsync<T>(IEnumerable<T>)` | **ninguna** | Ídem, para colecciones |
| `ValidateAsync<T>(Task<T>)` | **ninguna** | Espera la tarea y valida el resultado |
| `ValidateAsync<T>(Task<IEnumerable<T>>)` | **ninguna** | Ídem, para colecciones |

Implementación real de las dos sobrecargas síncronas:

```csharp
public static MlResult<T> Validate<T>(T source)
    => EnsureFp.NotNull(source, $"{nameof(source)} no be null")
         .Bind(_ => source.ValidateWithDataannotations());

public static MlResult<IEnumerable<T>> Validate<T>(IEnumerable<T> source)
    => EnsureFp.NotNull (source, $"{nameof(source)} no be null")
         .Bind(_ => EnsureFp.NotEmpty(source, $"{nameof(source)} no be empty"))
         .Bind(_ => source.ValidateWithDataannotations());
```

> ⚠️ **Una colección vacía se considera error** (`"source no be empty"`). Si en tu caso una lista vacía es legítima, **no uses esta sobrecarga**: llama directamente a la extensión `source.ValidateWithDataannotations()`.

---

## `Helpers.Extensions` — los métodos de extensión

Es donde está el trabajo real. Namespace: `MoralesLarios.OOFP.Validation.Dataannotations.Helpers`.

```csharp
public static class Extensions
{
    // 1. El motor: envuelve Validator.TryValidateObject
    public static IEnumerable<ValidationResult> ValidateObject(this object source)
    {
        ValidationContext valContext = new ValidationContext(source, null, null);
        var result = new List<ValidationResult>();
        Validator.TryValidateObject(source, valContext, result, true);   // 🔑 validateAllProperties: true
        return result;
    }

    // 2. Objeto individual → MlResult<T>
    public static MlResult<T> ValidateWithDataannotations<T>(this T source)
        => source!.ValidateObject().ToMlResultValid()
                  .Map (valResults => valResults.Select(x => x.ErrorMessage))
                  .Bind(errors     => errors.Any() ? errors!.ToMlResultFail<T>()
                                                   : source.ToMlResultValid<T>());

    public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this T source);
    public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this Task<T> sourceAsync);

    // 3. Colección → MlResult<IEnumerable<T>>, acumulando errores de TODOS los elementos
    public static MlResult<IEnumerable<T>> ValidateWithDataannotations<T>(this IEnumerable<T> source)
        => source.Select(x => x.ValidateWithDataannotations())
                 .FusionErrosIfExists();

    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this IEnumerable<T> source);
    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataannotationsAsync<T>(this Task<IEnumerable<T>> sourceAsync);
}
```

| Método | Devuelve | Notas |
|---|---|---|
| `ValidateObject(this object)` | `IEnumerable<ValidationResult>` | Vacío = válido. **No** devuelve `bool` |
| `ValidateWithDataannotations<T>(this T)` | `MlResult<T>` | Válido ⇒ devuelve **el propio objeto** |
| `ValidateWithDataannotations<T>(this IEnumerable<T>)` | `MlResult<IEnumerable<T>>` | Acumula errores de todos los elementos |
| `…Async` (×4) | `Task<…>` | Solo envuelven; **no** hay validación asíncrona real |

> 💡 **`ValidateObject` es útil por sí solo** cuando quieres los `ValidationResult` completos (con `MemberNames`, para pintar errores junto a cada campo del formulario). `ValidateWithDataannotations` se queda solo con `ErrorMessage`.

---

## Cómo se convierten los errores

La conversión es directa y merece entenderla, porque determina qué verás en el `Fail`:

```csharp
.Map (valResults => valResults.Select(x => x.ErrorMessage))   // ValidationResult → string?
.Bind(errors     => errors.Any() ? errors!.ToMlResultFail<T>()
                                 : source.ToMlResultValid<T>())
```

1. Cada `ValidationResult` se reduce a **su `ErrorMessage`**.
2. Si hay al menos uno, se construye un `Fail` con **todos** los mensajes (un `MlError` por mensaje).
3. Si no hay ninguno, se devuelve el objeto original como `Valid`.

Por tanto, dentro de un mismo objeto **todos los atributos se evalúan y todos los errores llegan**: no hay cortocircuito.

```csharp
var resultado = new CrearUsuarioDto { Email = "sin-arroba", Nombre = "" }
                    .ValidateWithDataannotations();

resultado.Match(
    valid: dto     => "ok",
    fail : errores => string.Join(" | ", errores.ToErrorsMessages()));
// → "El campo Email no es una dirección de correo válida | El campo Nombre es obligatorio"
```

> ⚠️ **Se pierde el `MemberNames`.** El `MlError` solo guarda el `Message`, así que **no sabrás a qué propiedad corresponde cada error** salvo que el propio mensaje lo diga. Si necesitas asociar error ↔ campo, personaliza los mensajes con el nombre del campo o usa `ValidateObject` directamente.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Objetos individuales vs. colecciones

| | Objeto individual | Colección |
|---|---|---|
| Firma | `MlResult<T>` | `MlResult<IEnumerable<T>>` |
| Errores dentro de un objeto | Todos, acumulados | Todos, acumulados |
| Errores entre objetos | — | Todos, fusionados con [`FusionErrosIfExists`](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) |
| Vacío / `null` | `null` ⇒ `Fail` (solo en `DataannotationsValidator`) | `null` o vacío ⇒ `Fail` (solo en `DataannotationsValidator`) |

`FusionErrosIfExists` recorre la secuencia de `MlResult<T>`, y:

- si **ninguno** falla, devuelve `Valid` con la colección de valores;
- si **alguno** falla, devuelve `Fail` con **todos** los errores de **todos** los elementos fusionados en un único `MlErrorsDetails`.

```csharp
var filas = new[]
{
    new FilaImportacion { Codigo = "A1", Cantidad = 5  },   // ok
    new FilaImportacion { Codigo = "",   Cantidad = 3  },   // falta código
    new FilaImportacion { Codigo = "C3", Cantidad = -1 },   // cantidad negativa
};

filas.ValidateWithDataannotations()
     .Match(valid: ok      => $"{ok.Count()} filas válidas",
            fail : errores => $"{errores.Errors.Count()} errores: {errores.ToErrorsDescription()}");
// → "2 errores: …"
```

> ⚠️ **No sabrás en qué fila estaba cada error.** El mensaje del atributo no lleva índice. Para importaciones grandes, valida elemento a elemento en un bucle y añade el índice con `AddMlErrorDetailIfFail` (ver [Ejemplo 4](#ejemplo-4--saber-qué-fila-falló-en-una-importación)).

---

## Combinación con `MlValidableFp<T>`

Este proyecto **no obliga** a heredar de [`MlValidableFp<T>`](../MoralesLarios.OOFP.Validation/README.md) — funciona con cualquier clase. Pero la combinación de ambos es el patrón más potente: **atributos para lo declarativo, código para lo que los atributos no saben expresar**.

```csharp
using MoralesLarios.OOFP.Validation;
using MoralesLarios.OOFP.Validation.Dataannotations.Helpers;
using System.ComponentModel.DataAnnotations;

public class ReservaRequest : MlValidableFp<ReservaRequest>
{
    [Required(ErrorMessage = "El nombre del titular es obligatorio")]
    [StringLength(80, ErrorMessage = "El titular no puede superar los 80 caracteres")]
    public string Titular { get; init; } = string.Empty;

    [Required]
    [EmailAddress(ErrorMessage = "El email del titular no es válido")]
    public string Email { get; init; } = string.Empty;

    [Range(1, 10, ErrorMessage = "El número de personas debe estar entre 1 y 10")]
    public int Personas { get; init; }

    public DateTime Entrada { get; init; }
    public DateTime Salida  { get; init; }

    public override MlResult<ReservaRequest> Validate()
        => this.ValidateWithDataannotations()                        // 1. lo declarativo
               .Bind(r => EnsureFp.That(r, r.Salida > r.Entrada,     // 2. lo que los atributos no pueden
                                        "La fecha de salida debe ser posterior a la de entrada"))
               .Bind(r => EnsureFp.That(r, (r.Salida - r.Entrada).TotalDays <= 30,
                                        "La reserva no puede superar los 30 días"));
}
```

> 💡 **Reparto natural**: los atributos cubren *obligatoriedad, formato y rango de un campo aislado*; el código de `Validate()` cubre *relaciones entre campos*. Los atributos **no pueden** comparar dos propiedades entre sí (salvo `[Compare]`, limitado a igualdad).

---

## ⚠️ Particularidades reales del código fuente

### 1. Las sobrecargas `Async` no comprueban `null` ni vacío

```csharp
// SÍ comprueba
public static MlResult<T> Validate<T>(T source)
    => EnsureFp.NotNull(source, …).Bind(_ => source.ValidateWithDataannotations());

// NO comprueba: va directo a la extensión
public static Task<MlResult<T>> ValidateAsync<T>(T source)
    => source.ValidateWithDataannotations().ToAsync();
```

Consecuencia: `DataannotationsValidator.ValidateAsync<T>(null)` **lanza `NullReferenceException`** en lugar de devolver un `Fail`, porque la extensión hace `source!.ValidateObject()` con el operador `!` (perdón del compilador, no comprobación).

**Recomendación:** si el valor puede ser nulo, usa la versión síncrona `Validate` y envuélvela tú, o encadena `EnsureFp.NotNull` antes:

```csharp
await EnsureFp.NotNull(dto, "El DTO no puede ser nulo")
              .BindAsync(async d => await DataannotationsValidator.ValidateAsync(d));
```

### 2. Los métodos `Async` no son asíncronos de verdad

Todos terminan en `.ToAsync()`, que es `Task.FromResult(...)`. **No hay ninguna operación de E/S**: existen únicamente para que encajen en cadenas `async` sin romperlas. No aportan paralelismo ni evitan bloqueos.

```csharp
public static Task<MlResult<T>> ValidateWithDataannotationsAsync<T>(this T source)
    => source!.ValidateWithDataannotations().ToAsync();       // Task.FromResult
```

### 3. `ValidateObject` colisiona con el homónimo del núcleo

El núcleo `MoralesLarios.OOFP` ya define `ValidateObject(this object)` en `MoralesLarios.OOFP.Helpers.Extensions`. Si importas **los dos namespaces** en el mismo fichero, el compilador dará **error de ambigüedad** (`CS0121`).

**Solución:** invócalo con nombre completo cuando haya conflicto, o no importes uno de los dos namespaces en ese fichero:

```csharp
var resultados = MoralesLarios.OOFP.Validation.Dataannotations.Helpers.Extensions.ValidateObject(dto);
```

### 4. El resultado booleano de `TryValidateObject` se descarta

```csharp
Validator.TryValidateObject(source, valContext, result, true);   // sin 'if', sin asignar
return result;
```

Es correcto en la práctica — la lista `result` vacía equivale a `true` — pero significa que **el criterio de "válido" es "la lista está vacía"**, no el booleano.

### 5. `ValidationContext` se construye sin `IServiceProvider` ni `items`

```csharp
new ValidationContext(source, null, null)
```

Consecuencia importante: los atributos personalizados que necesiten resolver servicios dentro de `IsValid(value, validationContext)` **recibirán `validationContext.GetService(...) == null`**. Los `ValidationAttribute` con dependencias inyectadas no funcionarán aquí (sí funcionarían en ASP.NET Core MVC, que sí pasa el proveedor).

### 6. Mensajes internos en inglés defectuoso

Los mensajes de las comprobaciones previas son `"source no be null"` y `"source no be empty"`. **Nunca los muestres al usuario final**: cuando `Validate` recibe un nulo, traduce el error antes de devolverlo, por ejemplo con `MergeErrorsDetailsIfFail` o comprobando el nulo tú mismo con un mensaje propio.

### 7. `ErrorMessage` puede ser `null`

`ValidationResult.ErrorMessage` es `string?`. El código usa `errors!` para silenciar la advertencia. Con un atributo mal configurado (sin `ErrorMessage` y sin recurso de localización) podría llegar un `null` a la lista de errores. En la práctica los atributos del framework siempre generan mensaje, pero **si escribes atributos propios, devuelve siempre un `ValidationResult` con mensaje**.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No valida objetos anidados.** `Validator.TryValidateObject`, incluso con `validateAllProperties: true`, **no** recorre en profundidad las propiedades complejas ni los elementos de una colección interna. Si tu DTO contiene otro DTO, tienes que validarlo explícitamente (ver [Ejemplo 3](#ejemplo-3--validar-objetos-anidados-explícitamente)).

> ⚠️ **No hay caché de metadatos** más allá de la que hace internamente `Validator`. Para validar cientos de miles de objetos en bucle, este enfoque basado en reflexión no es el más rápido: considera [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) o comprobaciones a mano con `EnsureFp`.

> ⚠️ **No integra con `ModelState`.** Es un mecanismo paralelo: si además dejas activada la validación automática de MVC, el DTO se validará **dos veces** y el `400` lo devolverá MVC antes de llegar a tu código.

> ⚠️ **No existen** `IsValidDataannotations`, `TryValidate`, `ValidateAndThrow`, ni ninguna sobrecarga que acepte `ValidationContext`, `IServiceProvider`, `items` o `validateAllProperties`. El comportamiento está fijado en el código.

> ⚠️ **No hay validación asíncrona real** ni soporte para atributos asíncronos (que tampoco existen en DataAnnotations).

---

## Ejemplos prácticos

### Ejemplo 1 — Validar un DTO en un servicio y encadenar

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation.Dataannotations;
using System.ComponentModel.DataAnnotations;

public class CrearProductoDto
{
    [Required(ErrorMessage = "El código del producto es obligatorio")]
    [StringLength(10, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 10 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Range(0.01, 999_999, ErrorMessage = "El precio debe ser mayor que 0")]
    public decimal Precio { get; set; }
}

public class ServicioProductos
{
    public Task<MlResult<Producto>> Crear(CrearProductoDto dto)
        => DataannotationsValidator.Validate(dto)
               .BindAsync(async v => await _repo.AddAsync(Mapear(v)));
}
```

### Ejemplo 2 — Traducir el error interno de `null` a un mensaje propio

```csharp
public MlResult<CrearProductoDto> ValidarSeguro(CrearProductoDto? dto)
    => EnsureFp.NotNull(dto, "No se ha recibido el cuerpo de la petición")
         .Bind(d => d.ValidateWithDataannotations());
```

Así el usuario nunca ve el `"source no be null"` interno.

### Ejemplo 3 — Validar objetos anidados explícitamente

```csharp
public class PedidoDto
{
    [Required] public string ClienteId { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección de envío es obligatoria")]
    public DireccionDto? Envio { get; set; }

    public List<LineaDto> Lineas { get; set; } = new();
}

public static MlResult<PedidoDto> ValidarPedido(PedidoDto dto)
    => dto.ValidateWithDataannotations()                     // solo el nivel superficial
          .Bind(p => p.Envio!.ValidateWithDataannotations()  // el anidado, a mano
                            .Map(_ => p))
          .Bind(p => p.Lineas.ValidateWithDataannotations()  // la colección interna, a mano
                            .Map(_ => p));
```

> ⚠️ El `[Required]` sobre `Envio` detecta que sea `null`, **pero no valida sus atributos internos**. Por eso el segundo `Bind` es imprescindible.

### Ejemplo 4 — Saber qué fila falló en una importación

`FusionErrosIfExists` pierde el índice, así que enriquecemos cada error con su posición:

```csharp
public static MlResult<IEnumerable<FilaCsv>> ValidarFichero(List<FilaCsv> filas)
    => filas.Select((fila, i) => fila.ValidateWithDataannotations()
                                     .AddMlErrorDetailIfFail("fila", i + 2))   // +2: cabecera + base 1
            .FusionErrosIfExists();

// Y al consumirlo
resultado.Match(
    valid: ok      => Ok($"{ok.Count()} filas importadas"),
    fail : errores => BadRequest(new
    {
        mensajes = errores.ToErrorsMessages(),
        detalles = errores.ToDetailsDescription()
    }));
```

> 💡 Alternativa más explícita: recorre con un `foreach`, valida fila a fila y construye tú los mensajes con el prefijo `$"Fila {i}: …"`. Pierdes la fusión automática, pero ganas mensajes autoexplicativos.

### Ejemplo 5 — Validar el resultado de una llamada asíncrona

```csharp
// El DTO llega de un cliente HTTP y hay que validarlo antes de procesarlo
public Task<MlResult<TarifaDto>> ObtenerTarifaValidada(int id)
    => DataannotationsValidator.ValidateAsync(_cliente.GetTarifaAsync(id));
    //                                        ^^^^^^^^^^^^^^^^^^^^^^^^^^ Task<TarifaDto>
```

> ⚠️ Recuerda la particularidad 1: si `GetTarifaAsync` puede devolver `null`, esta sobrecarga **lanzará**. En ese caso:

```csharp
public async Task<MlResult<TarifaDto>> ObtenerTarifaValidada(int id)
    => await EnsureFp.NotNull(await _cliente.GetTarifaAsync(id), $"No existe la tarifa {id}")
                     .BindAsync(async t => await t.ValidateWithDataannotationsAsync());
```

### Ejemplo 6 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: importar los dos namespaces con ValidateObject → CS0121 ambiguo
using MoralesLarios.OOFP.Helpers.Extensions;
using MoralesLarios.OOFP.Validation.Dataannotations.Helpers;
var r = dto.ValidateObject();          // 💥 error de compilación

// ✅ BIEN: usa el método de alto nivel, que no colisiona
var r = dto.ValidateWithDataannotations();


// ❌ MAL: pasar null a la sobrecarga async
await DataannotationsValidator.ValidateAsync<Dto>(null!);     // 💥 NullReferenceException

// ✅ BIEN
await EnsureFp.NotNull(dto, "DTO obligatorio").BindAsync(d => d.ValidateWithDataannotationsAsync());
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Validar un DTO con atributos, con control de `null` | `DataannotationsValidator.Validate(dto)` |
| Validar un DTO sin comprobar `null` | `dto.ValidateWithDataannotations()` |
| Validar una lista exigiendo que no esté vacía | `DataannotationsValidator.Validate(lista)` |
| Validar una lista que puede estar vacía | `lista.ValidateWithDataannotations()` |
| Validar lo que devuelve una `Task` | `DataannotationsValidator.ValidateAsync(tarea)` |
| Obtener los `ValidationResult` completos (con `MemberNames`) | `dto.ValidateObject()` |
| Saber a qué elemento pertenece cada error | `Select((x,i) => …AddMlErrorDetailIfFail("fila", i))` + `FusionErrosIfExists()` |
| Reglas entre varios campos | `MlValidableFp<T>.Validate()` + `EnsureFp.That` |
| Reglas complejas, condicionales o con dependencias | [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| Reglas reutilizables por tipo de dato | [`ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) |

---

## Mejores prácticas

1. **Usa `DataannotationsValidator.Validate` en los bordes** (entrada de un servicio, lectura de un fichero) y las extensiones en el interior, donde ya sabes que el objeto no es nulo.
2. **Personaliza siempre `ErrorMessage`** e incluye el nombre del campo en el texto: el `MemberNames` se pierde en la conversión a `MlError`.
3. **Escribe los mensajes en el idioma del usuario final**, no en el del código.
4. **Nunca dejes escapar `"source no be null"` / `"source no be empty"`**: comprueba tú los nulos con tus propios mensajes.
5. **Valida los objetos anidados explícitamente**: el motor no baja de nivel.
6. **No importes a la vez** `MoralesLarios.OOFP.Helpers.Extensions` y `…Dataannotations.Helpers` en el mismo fichero si vas a usar `ValidateObject`.
7. **No confíes en las sobrecargas `Async` para valores que puedan ser nulos**: no comprueban nada.
8. **Combina atributos con `MlValidableFp<T>`**: declarativo para el campo, código para las relaciones entre campos.
9. **No dupliques la validación con la automática de MVC**: elige un mecanismo por proyecto y sé coherente.
10. **Añade contexto con `AddMlErrorDetailIfFail`** (índice de fila, identificador, fichero) cuando el error deba diagnosticarse en logs.
11. **No uses atributos personalizados con servicios inyectados**: aquí el `ValidationContext` no tiene `IServiceProvider`.
12. **Para volúmenes grandes, mide**: la validación por reflexión es cómoda pero no es la opción más rápida.

---

## Resumen

- Adapta `System.ComponentModel.DataAnnotations` al modelo funcional: los atributos siguen igual, pero el resultado es un **`MlResult<T>` encadenable**.
- Dos piezas: **`DataannotationsValidator`** (fachada estática, 6 sobrecargas, comprueba `null`/vacío en las síncronas) y **`Helpers.Extensions`** (7 métodos de extensión, el motor real).
- `validateAllProperties: true` viene fijado: **todos los atributos se evalúan** y **todos los errores se devuelven** juntos, sin cortocircuito.
- Para colecciones, `FusionErrosIfExists` **acumula** los errores de todos los elementos (pero pierde el índice: añádelo con `AddMlErrorDetailIfFail`).
- ⚠️ Las sobrecargas `Async` **no comprueban `null`** y **no son asíncronas de verdad**; `ValidateObject` **colisiona** con el homónimo del núcleo; **no valida objetos anidados**; el `ValidationContext` **no tiene `IServiceProvider`**.
- Encaja perfectamente con [`MlValidableFp<T>`](../MoralesLarios.OOFP.Validation/README.md): atributos para lo declarativo, `Validate()` para las reglas entre campos.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — el contrato base `MlValidableFp<T>`
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — alternativa con reglas fluidas
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — reglas encapsuladas en tipos
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — servicios genéricos donde encadenar la validación
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — conversión de errores a respuestas HTTP

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores y detalles](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — comprobaciones que devuelven `MlResult`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` — transformar el valor válido](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — convertir el resultado en respuesta](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [`FusionErrosIfExists` y bucles funcionales](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)
