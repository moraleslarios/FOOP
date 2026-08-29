# MoralesLarios.OOFP.Validation.FluentValidations — FluentValidation en el raíl funcional

Puente entre [FluentValidation](https://docs.fluentvalidation.net/) y el modelo funcional de la solución: ejecuta un `AbstractValidator<T>` y traduce su `ValidationResult` a un **[`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)** encadenable.

Es el proyecto adecuado cuando las reglas son demasiado ricas para los atributos de DataAnnotations: condicionales (`When`), reglas por colección (`RuleForEach`), validadores anidados (`SetValidator`), mensajes con interpolación o severidades. Todo eso se escribe en el validador de FluentValidation **como siempre**, y aquí solo se convierte el resultado.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Los dos únicos métodos](#los-dos-únicos-métodos)
5. [Cómo funciona la conversión, paso a paso](#cómo-funciona-la-conversión-paso-a-paso)
6. [Restricciones genéricas: qué exigen y por qué](#restricciones-genéricas-qué-exigen-y-por-qué)
7. [Combinación con `MlValidableFp<T>`](#combinación-con-mlvalidablefpt)
8. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
9. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
10. [Ejemplos prácticos](#ejemplos-prácticos)
11. [Comparativa con los otros mecanismos de validación](#comparativa-con-los-otros-mecanismos-de-validación)
12. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
13. [Mejores prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

FluentValidation devuelve un `ValidationResult` con `IsValid` y `Errors`. Es cómodo, pero **rompe la cadena**: hay que salir del flujo, mirar un booleano y decidir.

❌ **Con FluentValidation a pelo:**

```csharp
var validador = new CrearUsuarioValidator();
var resultado = validador.Validate(dto);

if (! resultado.IsValid)
    return BadRequest(resultado.Errors.Select(e => e.ErrorMessage));

var creado = await _repo.AddAsync(dto);      // ¿y si esto falla? otro if...
if (creado is null) return StatusCode(500);
return Ok(creado);
```

✅ **Con este proyecto:**

```csharp
return await dto.ValidateWitHFluentValidations<CrearUsuarioDto, CrearUsuarioValidator>()
                .BindAsync(v => _repo.AddAsync(v))
                .MatchAsync(valid: creado  => Ok(creado),
                            fail : errores => BadRequest(errores.ToErrorsMessages()));
```

> 💡 **La ventaja real**: la validación deja de ser un `if` que interrumpe y pasa a ser **un eslabón más** de la misma cadena que la persistencia, el mapeo y la respuesta HTTP. Un único `Match` al final decide qué se devuelve.

Además, **no hay que instanciar el validador**: el propio método lo crea por reflexión a partir del parámetro genérico.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| `FluentValidation` | `AbstractValidator<T>`, `ValidationResult`, `RuleFor`… |
| [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) | Proyecto base (y, transitivamente, el núcleo `MoralesLarios.OOFP`) |

```csharp
using FluentValidation;                                          // para escribir el validador
using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;   // 🔑 los métodos de extensión
```

> ⚠️ **El namespace de las extensiones acaba en `.Helpers`.** Si importas solo `MoralesLarios.OOFP.Validation.FluentValidations`, el método de extensión **no aparecerá** en IntelliSense. Es el error más frecuente al usar este proyecto.

No requiere registro en el contenedor de dependencias: **todo es estático** y los validadores se instancian por reflexión.

---

## Estructura del proyecto

```
MoralesLarios.OOFP.Validation.FluentValidations/
├── GlobalUsings.cs
└── Helpers/
    └── Extensions.cs      → los 2 únicos métodos públicos del proyecto
```

Es, junto con [`Validation`](../MoralesLarios.OOFP.Validation/README.md), uno de los proyectos más pequeños de la solución: **una sola clase estática con dos métodos**. Toda la potencia viene de FluentValidation; aquí solo está la traducción.

---

## Los dos únicos métodos

```csharp
namespace MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

public static class Extensions
{
    public static MlResult<T> ValidateWitHFluentValidations<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new();

    public static Task<MlResult<T>> ValidateWitHFluentValidationsAsync<T, TValidator>(this T source)
        where T          : class
        where TValidator : AbstractValidator<T>, new();
}
```

| Método | Devuelve | Notas |
|---|---|---|
| `ValidateWitHFluentValidations<T, TValidator>()` | `MlResult<T>` | Válido ⇒ devuelve **el propio objeto** intacto |
| `ValidateWitHFluentValidationsAsync<T, TValidator>()` | `Task<MlResult<T>>` | Solo envuelve con `.ToAsync()`; **no** es asíncrono real |

> ⚠️ **El nombre tiene una errata en el código fuente: `ValidateWitHFluentValidations`** — con **`H` mayúscula** en `WitH` y sin la `h` de `With`. No es un error de esta documentación: es literalmente así en `Helpers/Extensions.cs`. Si lo escribes bien (`ValidateWithFluentValidations`) **no compilará**.

Uso mínimo:

```csharp
MlResult<CrearUsuarioDto> resultado =
    dto.ValidateWitHFluentValidations<CrearUsuarioDto, CrearUsuarioValidator>();
```

> 💡 **Hay que indicar los dos genéricos** aunque `T` parezca inferible: al ser `TValidator` no inferible, C# obliga a escribir la lista completa de argumentos de tipo.

---

## Cómo funciona la conversión, paso a paso

Esta es la implementación real, y merece leerla porque explica todos los comportamientos:

```csharp
public static MlResult<T> ValidateWitHFluentValidations<T, TValidator>(this T source)
    where T          : class
    where TValidator : AbstractValidator<T>, new()
    => MlResult.Empty()
         .TryMap(_          => Activator.CreateInstance<TValidator>(),
                              $"Problems with automatic create instance of {typeof(TValidator).Name}")
         .TryMap(validator  => validator.Validate(source))
         .Map   (valResults => valResults.Errors.Select(x => x.ErrorMessage))
         .Bind  (errors     => errors.Any() ? errors.ToMlResultFail<T>()
                                            : source.ToMlResultValid<T>());
```

| Paso | Qué hace | Si falla |
|---|---|---|
| `MlResult.Empty()` | Semilla `Valid<object>` para arrancar la cadena | — |
| `TryMap` + `Activator.CreateInstance<TValidator>()` | **Instancia el validador por reflexión** | `Fail` con `"Problems with automatic create instance of X"` y la excepción en `Details` |
| `TryMap` + `validator.Validate(source)` | Ejecuta todas las reglas | `Fail` con el mensaje por defecto y la excepción en `Details` |
| `Map` + `Errors.Select(ErrorMessage)` | Se queda **solo con los mensajes** | — |
| `Bind` | ¿Hay mensajes? ⇒ `Fail`. ¿Ninguno? ⇒ `Valid` con `source` | — |

Consecuencias importantes de este diseño:

1. **Todas las reglas se evalúan**: no hay cortocircuito. Si el DTO tiene 5 errores, el `Fail` traerá los 5 mensajes (uno por `MlError`), respetando la `CascadeMode` que hayas configurado en el validador.
2. **Se pierde todo lo que no sea `ErrorMessage`**: `PropertyName`, `AttemptedValue`, `ErrorCode`, `Severity` y `CustomState` **desaparecen**. Si necesitas asociar error ↔ propiedad, **incluye el nombre del campo en el mensaje**.
3. **Los errores de infraestructura y los de negocio llegan por el mismo canal**: un fallo al construir el validador produce un `Fail` igual que una regla incumplida. Se distinguen porque el primero **lleva excepción en `Details`** (ver [Ejemplo 5](#ejemplo-5--distinguir-error-de-validación-de-error-técnico)).
4. `Severity.Warning` / `Severity.Info` **también se convierten en errores**: `ValidationResult.Errors` los incluye, y aquí no se filtran por severidad.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Restricciones genéricas: qué exigen y por qué

```csharp
where T          : class
where TValidator : AbstractValidator<T>, new()
```

| Restricción | Implicación práctica |
|---|---|
| `T : class` | **No sirve para `struct`, `record struct` ni tipos primitivos**. Sí para `class` y `record` (de referencia) |
| `TValidator : AbstractValidator<T>` | El validador debe ser exactamente para `T`, no para una clase base |
| `TValidator : new()` | 🔑 **El validador debe tener constructor público sin parámetros** |

La última es la más restrictiva: **un validador con dependencias inyectadas no se puede usar aquí**.

```csharp
// ✅ Válido aquí
public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioDto>
{
    public CrearUsuarioValidator() { RuleFor(x => x.Email).NotEmpty().EmailAddress(); }
}

// ❌ NO compila con estas extensiones: no cumple new()
public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioDto>
{
    public CrearUsuarioValidator(IUsuariosRepo repo) { … }   // 💥 CS0310
}
```

Si necesitas dependencias, resuelve el validador tú mismo desde el contenedor y convierte el resultado a mano (ver [Ejemplo 6](#ejemplo-6--validador-con-dependencias-inyectadas)).

---

## Combinación con `MlValidableFp<T>`

El patrón recomendado: el DTO hereda de [`MlValidableFp<T>`](../MoralesLarios.OOFP.Validation/README.md) y su `Validate()` delega en el validador de FluentValidation. Así el resto de la aplicación **no necesita saber qué motor de validación se usa**.

```csharp
using FluentValidation;
using MoralesLarios.OOFP.Validation;
using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

public class CrearPedidoDto : MlValidableFp<CrearPedidoDto>
{
    public string       ClienteId { get; init; } = string.Empty;
    public List<Linea>  Lineas    { get; init; } = new();
    public decimal      Descuento { get; init; }

    public override MlResult<CrearPedidoDto> Validate()
        => this.ValidateWitHFluentValidations<CrearPedidoDto, CrearPedidoValidator>();
}

public class CrearPedidoValidator : AbstractValidator<CrearPedidoDto>
{
    public CrearPedidoValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty().WithMessage("El identificador de cliente es obligatorio");

        RuleFor(x => x.Lineas).NotEmpty().WithMessage("El pedido debe tener al menos una línea");

        RuleForEach(x => x.Lineas).ChildRules(l =>
        {
            l.RuleFor(x => x.Cantidad).GreaterThan(0)
                                      .WithMessage("La cantidad de cada línea debe ser mayor que 0");
        });

        // Regla condicional: los atributos de DataAnnotations no pueden expresar esto
        When(x => x.Descuento > 0, () =>
            RuleFor(x => x.Descuento).LessThanOrEqualTo(50)
                                     .WithMessage("El descuento no puede superar el 50%"));
    }
}
```

Y el consumidor solo ve el contrato:

```csharp
public Task<MlResult<Pedido>> Crear(CrearPedidoDto dto)
    => dto.Validate()                               // ← no sabe que hay FluentValidation detrás
          .BindAsync(v => _repo.AddAsync(Mapear(v)));
```

> 💡 **Ventaja de este acoplamiento indirecto**: si mañana cambias FluentValidation por DataAnnotations o por comprobaciones a mano con `EnsureFp`, solo tocas el cuerpo de `Validate()`.

---

## ⚠️ Particularidades reales del código fuente

### 1. El nombre del método está mal escrito

`ValidateWitHFluentValidations` (y su versión `Async`). La `H` mayúscula está en el código, así que **es el nombre válido**. Si lo corriges en tu llamada, no compila.

### 2. Se crea un validador nuevo en **cada** llamada

```csharp
.TryMap(_ => Activator.CreateInstance<TValidator>(), …)
```

No hay caché ni reutilización de instancias. En un bucle sobre 100 000 elementos se construyen 100 000 validadores por reflexión. **Para volúmenes grandes, instancia el validador una vez** y llama a `Validate` directamente, convirtiendo tú el resultado.

### 3. `Activator.CreateInstance<T>()` es más lento que `new T()`

Con la restricción `new()` presente, el compilador **podría** haber usado `new TValidator()` (que se compila a una llamada directa). Usar `Activator` añade sobrecarga de reflexión innecesaria. Es un detalle de implementación, pero explica el punto anterior.

### 4. El segundo `TryMap` no lleva mensaje de error personalizado

```csharp
.TryMap(validator => validator.Validate(source))    // sin mensaje
```

Si una regla lanza una excepción (por ejemplo, un `Must(...)` que accede a una propiedad nula), el `Fail` llevará el **mensaje por defecto** del núcleo, no algo descriptivo. La excepción sí queda en `Details`: recupérala con `GetDetailException()`.

### 5. `source` no se comprueba contra `null`

No hay `EnsureFp.NotNull`. Con `source == null`:

- `Activator.CreateInstance` funciona;
- `validator.Validate(null)` **lanza `ArgumentNullException`** dentro de FluentValidation;
- ese fallo lo captura el `TryMap`, así que **obtendrás un `Fail`, no una excepción propagada** — pero con un mensaje poco claro.

**Recomendación:** comprueba el nulo explícitamente con tu propio mensaje:

```csharp
EnsureFp.NotNull(dto, "No se ha recibido el cuerpo de la petición")
        .Bind(d => d.ValidateWitHFluentValidations<CrearUsuarioDto, CrearUsuarioValidator>());
```

### 6. El método `Async` no es asíncrono de verdad

```csharp
=> source.ValidateWitHFluentValidations<T, TValidator>().ToAsync();
```

`ToAsync()` es `Task.FromResult(...)`. **No** llama a `ValidateAsync` de FluentValidation, así que **los validadores asíncronos (`MustAsync`, `CustomAsync`) no se ejecutarán como tal**: FluentValidation lanzará `AsyncValidatorInvokedSynchronouslyException` al detectar una regla asíncrona en una llamada síncrona. Esa excepción la atrapa el `TryMap` y verás un `Fail` confuso.

> ⚠️ **Si tu validador tiene reglas asíncronas, no uses estas extensiones.** Llama tú a `await validador.ValidateAsync(dto)` y convierte el resultado (ver [Ejemplo 7](#ejemplo-7--validador-con-reglas-asíncronas)).

### 7. Los mensajes internos están en inglés defectuoso

`"Problems with automatic create instance of X"`. **Nunca lo muestres al usuario final**: es un error de programación (validador sin constructor accesible), no de datos. Fíltralo comprobando si hay excepción en los detalles.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No soporta validación asíncrona real.** No existe ninguna sobrecarga que llame a `ValidateAsync` de FluentValidation.

> ⚠️ **No soporta validadores con dependencias**: la restricción `new()` lo impide.

> ⚠️ **No soporta `RuleSet`s.** No hay parámetro para `options => options.IncludeRuleSets(...)`, ni acceso a `ValidationContext<T>`, ni posibilidad de pasar `RootContextData`.

> ⚠️ **No filtra por `Severity`.** Los avisos (`Warning`) y los informativos (`Info`) se convierten en errores igual que los `Error`.

> ⚠️ **No conserva `PropertyName` ni `ErrorCode`.** Solo sobrevive `ErrorMessage`. Si el front necesita saber el campo, ponlo en el texto o construye tú la traducción.

> ⚠️ **No integra con `ModelState` ni con la validación automática de ASP.NET Core.** Es un mecanismo paralelo: si además registras los validadores en MVC, el DTO se validará dos veces.

> ⚠️ **No existen** `ValidateWithFluentValidations` (sin la errata), `TryValidateFluent`, `IsValidFluent`, ni sobrecargas que acepten una instancia de validador, un `IValidator<T>` o un `IServiceProvider`.

---

## Ejemplos prácticos

### Ejemplo 1 — Validar y encadenar en un servicio

```csharp
using FluentValidation;
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

public class CrearProductoDto
{
    public string  Codigo { get; init; } = string.Empty;
    public decimal Precio { get; init; }
}

public class CrearProductoValidator : AbstractValidator<CrearProductoDto>
{
    public CrearProductoValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().WithMessage("El código es obligatorio")
                              .Length(3, 10).WithMessage("El código debe tener entre 3 y 10 caracteres");

        RuleFor(x => x.Precio).GreaterThan(0).WithMessage("El precio debe ser mayor que 0");
    }
}

public class ServicioProductos
{
    public Task<MlResult<Producto>> Crear(CrearProductoDto dto)
        => dto.ValidateWitHFluentValidations<CrearProductoDto, CrearProductoValidator>()
              .BindAsync(v => _repo.AddAsync(Mapear(v)));
}
```

### Ejemplo 2 — Incluir el nombre del campo en el mensaje

Como `PropertyName` se pierde, el mensaje debe ser autoexplicativo:

```csharp
// ❌ El front recibe "No puede estar vacío" y no sabe de qué campo
RuleFor(x => x.Email).NotEmpty().WithMessage("No puede estar vacío");

// ✅ El mensaje se entiende solo
RuleFor(x => x.Email).NotEmpty().WithMessage("El correo electrónico es obligatorio");

// ✅ Alternativa con el nombre de la propiedad interpolado por FluentValidation
RuleFor(x => x.Email).NotEmpty().WithMessage("El campo {PropertyName} es obligatorio");
```

### Ejemplo 3 — Validar una colección acumulando todos los errores

Las extensiones solo trabajan con un objeto. Para una colección, combina con [`FusionErrosIfExists`](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md):

```csharp
public static MlResult<IEnumerable<FilaCsv>> ValidarFichero(List<FilaCsv> filas)
    => filas.Select((fila, i) =>
                fila.ValidateWitHFluentValidations<FilaCsv, FilaCsvValidator>()
                    .AddMlErrorDetailIfFail("fila", i + 2))     // +2: cabecera + base 1
            .FusionErrosIfExists();
```

> 💡 `AddMlErrorDetailIfFail` guarda el índice en `Details`, así los logs sí saben qué fila falló aunque el mensaje no lo diga. Recupéralo con `ToDetailsDescription()`.

### Ejemplo 4 — Comprobar el nulo con un mensaje propio

```csharp
public MlResult<CrearProductoDto> ValidarSeguro(CrearProductoDto? dto)
    => EnsureFp.NotNull(dto, "No se ha recibido el cuerpo de la petición")
         .Bind(d => d.ValidateWitHFluentValidations<CrearProductoDto, CrearProductoValidator>());
```

### Ejemplo 5 — Distinguir error de validación de error técnico

Los fallos técnicos (validador no instanciable, excepción dentro de una regla) **llevan excepción en `Details`**; los de negocio, no:

```csharp
var resultado = dto.ValidateWitHFluentValidations<CrearProductoDto, CrearProductoValidator>();

return resultado.Match(
    valid: v       => Ok(v),
    fail : errores => errores.GetDetailException().Match(
                          valid: ex => { _logger.LogError(ex, "Fallo técnico al validar");
                                         return StatusCode(500, "Error interno de validación"); },
                          fail : _  => BadRequest(errores.ToErrorsMessages())));
```

> 💡 `GetDetailException()` devuelve `MlResult<Exception>`: `IsValid` significa "hay excepción registrada", es decir, **el fallo es técnico y no debe mostrarse al usuario**.

### Ejemplo 6 — Validador con dependencias inyectadas

Las extensiones no valen (`new()`), pero la conversión es de tres líneas:

```csharp
public class UsuarioUnicoValidator : AbstractValidator<CrearUsuarioDto>
{
    public UsuarioUnicoValidator(IUsuariosRepo repo)
    {
        RuleFor(x => x.Email).NotEmpty()
            .Must(email => ! repo.ExisteEmail(email))
            .WithMessage("Ya existe un usuario con ese correo electrónico");
    }
}

public static MlResult<T> AMlResult<T>(this ValidationResult valResult, T source)
    => valResult.IsValid
           ? source.ToMlResultValid()
           : valResult.Errors.Select(e => e.ErrorMessage).ToMlResultFail<T>();

// Uso: el validador viene del contenedor
public MlResult<CrearUsuarioDto> Validar(CrearUsuarioDto dto)
    => _validador.Validate(dto).AMlResult(dto);
```

### Ejemplo 7 — Validador con reglas asíncronas

```csharp
public class PedidoValidator : AbstractValidator<PedidoDto>
{
    public PedidoValidator(IStockService stock)
    {
        RuleFor(x => x.ProductoId)
            .MustAsync(async (id, ct) => await stock.HayExistenciasAsync(id))
            .WithMessage("No hay existencias del producto solicitado");
    }
}

// ⚠️ ValidateWitHFluentValidationsAsync NO sirve aquí: lanzaría
//    AsyncValidatorInvokedSynchronouslyException (capturada como Fail confuso).

public async Task<MlResult<PedidoDto>> Validar(PedidoDto dto)
    => (await _validador.ValidateAsync(dto)).AMlResult(dto);   // extensión del Ejemplo 6
```

### Ejemplo 8 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: escribir el nombre "bien" → no existe
dto.ValidateWithFluentValidations<Dto, DtoValidator>();     // 💥 no compila

// ✅ BIEN: la errata es parte del nombre
dto.ValidateWitHFluentValidations<Dto, DtoValidator>();

```

### Ejemplo 9 — Validador con reglas asíncronas y dependencias

```csharp
public class PedidoValidator : AbstractValidator<PedidoDto>
{
    public PedidoValidator(IStockService stock)
    {
        RuleFor(x => x.ProductoId)
            .MustAsync(async (id, ct) => await stock.HayExistenciasAsync(id))
            .WithMessage("No hay existencias del producto solicitado");
    }
}

public async Task<MlResult<PedidoDto>> Validar(PedidoDto dto)
    => (await _validador.ValidateAsync(dto)).AMlResult(dto);
```

### Ejemplo 10 — Validador con reglas asíncronas y dependencias

```csharp
public class PedidoValidator : AbstractValidator<PedidoDto>
{
    public PedidoValidator(IStockService stock)
    {
        RuleFor(x => x.ProductoId)
            .MustAsync(async (id, ct) => await stock.HayExistenciasAsync(id))
            .WithMessage("No hay existencias del producto solicitado");
    }
}

public async Task<MlResult<PedidoDto>> Validar(PedidoDto dto)
    => (await _validador.ValidateAsync(dto)).AMlResult(dto);
```

---

## Comparativa con los otros mecanismos de validación

| | [`Validation`](../MoralesLarios.OOFP.Validation/README.md) | [`.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) | **`.FluentValidations`** | [`ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) |
|---|---|---|---|---|
| Dónde se escriben las reglas | En `Validate()`, a mano | En atributos, sobre la propiedad | En un `AbstractValidator<T>` aparte | Dentro del propio tipo |
| Reglas entre varios campos | ✅ | ❌ (salvo `[Compare]`) | ✅ (`When`, `Must`) | ❌ |
| Reglas sobre colecciones internas | ✅ manual | ❌ | ✅ (`RuleForEach`) | ❌ |
| Objetos anidados | ✅ manual | ❌ | ✅ (`SetValidator`) | ✅ por composición |
| Dependencias inyectadas | ✅ | ❌ | ✅ *pero no con estas extensiones* | ❌ |
| Reglas asíncronas | ✅ manual | ❌ | ✅ *pero no con estas extensiones* | ❌ |
| Acumula todos los errores | Con `BindMulti` | ✅ siempre | ✅ siempre | ❌ (uno por VO) |
| Dependencia externa | Ninguna | Ninguna (framework) | `FluentValidation` | Ninguna |
| Coste por validación | El de tu código | Reflexión | Reflexión + instancia nueva | Mínimo |

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Reglas ricas (condicionales, colecciones, anidados) sin dependencias | `dto.ValidateWitHFluentValidations<T, TValidator>()` |
| Lo mismo dentro de una cadena `async` | `…Async<T, TValidator>()` |
| Que el consumidor no sepa qué motor uso | `MlValidableFp<T>.Validate()` delegando en la extensión |
| Comprobar antes que el objeto no es nulo | `EnsureFp.NotNull(dto, "…").Bind(d => d.ValidateWitH…)` |
| Validar una lista acumulando errores | `Select(...).AddMlErrorDetailIfFail("fila", i)` + `FusionErrosIfExists()` |
| Validador con dependencias del contenedor | Resolverlo por DI + conversión manual (Ejemplo 6) |
| Reglas asíncronas (`MustAsync`) | `await validador.ValidateAsync(dto)` + conversión manual (Ejemplo 7) |
| Validar cientos de miles de objetos | Un validador reutilizado + conversión manual |
| Reglas simples de formato/obligatoriedad | [`.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| Reglas inherentes a un tipo de dato | [`ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) |

---

## Mejores prácticas

1. **Recuerda la errata del nombre**: `ValidateWitH…`, con `H` mayúscula.
2. **Importa el namespace `.Helpers`**, no el raíz.
3. **Incluye el nombre del campo en cada `WithMessage`**: el `PropertyName` se pierde en la conversión.
4. **Escribe los mensajes en el idioma del usuario final** y personalízalos todos: los de FluentValidation por defecto están en inglés.
5. **Un validador por DTO, en su propio fichero**, con el sufijo `Validator`.
6. **Delega desde `MlValidableFp<T>.Validate()`** para no acoplar el resto del código a FluentValidation.
7. **Comprueba los nulos tú mismo** con `EnsureFp.NotNull` y un mensaje propio.
8. **Separa error técnico de error de negocio** con `GetDetailException()` antes de responder al cliente.
9. **No uses estas extensiones con reglas asíncronas o validadores con dependencias**: convierte el `ValidationResult` a mano.
10. **Para volúmenes grandes, reutiliza la instancia del validador**: aquí se crea una nueva en cada llamada.
11. **No dupliques la validación con la automática de MVC**: elige un mecanismo por proyecto.
12. **Añade contexto con `AddMlErrorDetailIfFail`** (índice, identificador, fichero) cuando el error deba diagnosticarse en logs.
13. **Ten en cuenta que `Severity.Warning` cuenta como error**: si necesitas avisos no bloqueantes, no los modeles con FluentValidation aquí.

---

## Resumen

- Traduce el `ValidationResult` de FluentValidation a un **`MlResult<T>` encadenable**, sin `if` ni excepciones.
- **Dos únicos métodos**, en `MoralesLarios.OOFP.Validation.FluentValidations.Helpers.Extensions`:
  `ValidateWitHFluentValidations<T, TValidator>()` y su variante `Async`.
- El validador se **instancia por reflexión** en cada llamada: de ahí las restricciones `T : class` y `TValidator : AbstractValidator<T>, new()`.
- **Todos los errores llegan juntos** (uno por `MlError`), pero **solo sobrevive `ErrorMessage`**: pon el nombre del campo en el texto.
- ⚠️ Erratas y límites reales: el nombre lleva **`WitH`**; el método `Async` **no es asíncrono**; **no soporta reglas asíncronas, `RuleSet`s ni validadores con dependencias**; **no comprueba `null`**; **no filtra por severidad**.
- Los fallos técnicos se distinguen de los de negocio porque **llevan excepción en `Details`** (`GetDetailException()`).
- Combínalo con [`MlValidableFp<T>`](../MoralesLarios.OOFP.Validation/README.md) para que el consumidor no dependa del motor de validación.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — el contrato base `MlValidableFp<T>`
- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — alternativa con atributos
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — reglas encapsuladas en tipos
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — servicios genéricos donde encadenar la validación
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — conversión de errores a respuestas HTTP

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores y detalles](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — comprobaciones que devuelven `MlResult`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` y `TryMap` — transformar el valor válido](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — convertir el resultado en respuesta](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [`FusionErrosIfExists` y bucles funcionales](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)
