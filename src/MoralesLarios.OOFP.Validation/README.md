# MoralesLarios.OOFP.Validation — el contrato de los objetos auto-validables

Proyecto **mínimo y deliberadamente vacío de implementación**: contiene un único tipo, `MlValidableFp<T>`, que define el contrato común *"este objeto sabe validarse a sí mismo y devuelve un [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)"*.

Su valor no está en el código que trae (son cuatro líneas), sino en **unificar la forma de validar en toda la solución**: da igual si por debajo usas `if`s a mano, DataAnnotations o FluentValidation — desde fuera, todo objeto validable se comporta igual y encadena igual.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [`MlValidableFp<T>` — la clase base completa](#mlvalidablefpt--la-clase-base-completa)
4. [Las tres reglas del contrato](#las-tres-reglas-del-contrato)
5. [Cómo implementar `Validate()`](#cómo-implementar-validate)
6. [Las tres estrategias de acumulación de errores](#las-tres-estrategias-de-acumulación-de-errores)
7. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
8. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
9. [Ejemplos prácticos](#ejemplos-prácticos)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Mejores prácticas](#mejores-prácticas)
12. [Resumen](#resumen)
13. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

En una aplicación grande la validación acaba dispersa: unos DTOs se validan en el controlador, otros en el servicio, otros en un helper estático, y cada uno **devuelve algo distinto** (`bool`, `string`, lista de errores, excepción…). El resultado es que **no se pueden encadenar** ni tratar de forma uniforme.

❌ **Sin un contrato común:**
```csharp
// Cada validación tiene una forma diferente de fallar
if (! ValidadorUsuario.EsValido(dto, out var errores))  return BadRequest(errores);
var r = ValidadorDireccion.Validar(dto.Direccion);       // devuelve string? con el error
if (r is not null) return BadRequest(r);
try { ValidadorPago.Comprobar(dto.Pago); }               // esta lanza
catch (ValidationException ex) { return BadRequest(ex.Message); }
```

✅ **Con `MlValidableFp<T>`:**
```csharp
// Todas las validaciones devuelven MlResult<T>, así que se encadenan igual
public IActionResult Crear(CrearUsuarioRequest dto)
    => dto.Validate()
          .Bind(v => _servicio.Crear(v))
          .Match(valid: creado  => Ok(creado),
                 fail : errores => BadRequest(errores.ToErrorsMessages()));
```

> 💡 **La idea de fondo**: la validación deja de ser un *paso previo con su propio protocolo de error* y se convierte en **el primer eslabón del mismo raíl** que el resto del caso de uso. `Validate()` devuelve lo mismo que devuelve el repositorio, que el servicio y que el cliente HTTP: un `MlResult<T>`.

Además, al ser el objeto quien se valida, **la regla vive junto al dato**: no hay que buscar en qué validador está la restricción de un campo.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) | `MlResult<T>`, `MlErrorsDetails` y todos los operadores |

Es la única dependencia. **Este proyecto no referencia DataAnnotations ni FluentValidation**: son las librerías satélite las que dependen de él, nunca al contrario.

```csharp
using MoralesLarios.OOFP.Validation;
using MoralesLarios.OOFP.Types;          // MlResult<T>
```

No requiere registro en el contenedor de dependencias.

---

## `MlValidableFp<T>` — la clase base completa

Este es **el proyecto entero**, literalmente:

```csharp
namespace MoralesLarios.OOFP.Validation;

public abstract class MlValidableFp<T>
    where T : class
{
    public abstract MlResult<T> Validate();
}
```

| Elemento | Detalle |
|---|---|
| Tipo | `abstract class` (no interfaz) → obliga a **herencia**, se consume una sola posición de clase base |
| Restricción | `where T : class` → **no admite `struct`**, `record struct` ni tipos primitivos |
| Único miembro | `public abstract MlResult<T> Validate()` — sin parámetros, sin `virtual`, sin implementación por defecto |
| Retorno | `MlResult<T>` — en caso válido se espera **el propio objeto**; en caso inválido, los errores |

El patrón de uso es **autorreferencial** (*self-referential generic*): la clase se pasa a sí misma como parámetro genérico.

```csharp
public class CrearUsuarioRequest : MlValidableFp<CrearUsuarioRequest>
//                                              ^^^^^^^^^^^^^^^^^^^ el propio tipo
```

> ⚠️ **El compilador no obliga a que `T` sea la clase que hereda.** Nada impide escribir `class A : MlValidableFp<B>`. Es una convención, no una garantía: respétala siempre para que `Validate()` devuelva el tipo que el llamante espera.

---

## Las tres reglas del contrato

Aunque el compilador solo exige la firma, el contrato **semántico** que espera el resto de la solución tiene tres reglas:

### 1. Si es válido, devuelve `this`

```csharp
// ✅ Devuelve el propio objeto: permite seguir la cadena con el dato ya validado
return this.ToMlResultValid();
```

Así el llamante puede hacer `dto.Validate().Bind(v => Guardar(v))` y trabajar con `v` sabiendo que ya pasó la validación.

### 2. `Validate()` es **pura**: no muta, no llama a base de datos, no lanza

```csharp
// ❌ MAL: efectos secundarios dentro de Validate
public override MlResult<Pedido> Validate()
{
    Total = Lineas.Sum(l => l.Importe);                     // muta el objeto
    if (! _repo.ExisteCliente(ClienteId)) return "…";       // I/O oculto y lento
    return this.ToMlResultValid();
}

// ✅ BIEN: solo comprueba lo que el objeto sabe de sí mismo
public override MlResult<Pedido> Validate()
    => EnsureFp.That(this, Lineas.Any(),        "El pedido debe tener al menos una línea")
       .Bind(p => EnsureFp.That(p, ClienteId > 0, "El pedido debe tener cliente"));
```

Las validaciones que necesitan la base de datos (unicidad, existencia de claves ajenas) **no son responsabilidad de `Validate()`**: van en el servicio, encadenadas después.

### 3. Es **idempotente**: llamarla dos veces da el mismo resultado

Esto se cumple automáticamente si respetas la regla 2.

---

## Cómo implementar `Validate()`

Hay tres estilos, de menos a más recomendable según el número de reglas.

### Estilo 1 — Ternario, para una sola regla

```csharp
public class Etiqueta : MlValidableFp<Etiqueta>
{
    public string Texto { get; init; } = string.Empty;

    public override MlResult<Etiqueta> Validate()
        => string.IsNullOrWhiteSpace(Texto)
               ? "El texto de la etiqueta es obligatorio".ToMlResultFail<Etiqueta>()
               : this.ToMlResultValid();
}
```

### Estilo 2 — `EnsureFp` + `Bind`, cortocircuitado (para en el primer error)

```csharp
public class CrearUsuarioRequest : MlValidableFp<CrearUsuarioRequest>
{
    public string Email  { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public int    Edad   { get; init; }

    public override MlResult<CrearUsuarioRequest> Validate()
        => EnsureFp.NotNullEmptyOrWhitespace(Email, "El email es obligatorio")
             .Bind(_ => EnsureFp.That(Email, Email.Contains('@'), "El email no tiene un formato válido"))
             .Bind(_ => EnsureFp.NotNullEmptyOrWhitespace(Nombre, "El nombre es obligatorio"))
             .Bind(_ => EnsureFp.That(Edad, Edad is >= 18 and <= 120, "La edad debe estar entre 18 y 120"))
             .Map (_ => this);
}
```

> 💡 Fíjate en el `.Map(_ => this)` final: las comprobaciones intermedias devuelven el *campo*, y el `Map` final reconvierte al objeto completo para cumplir la regla 1.

### Estilo 3 — Value objects, delegando la validación a los tipos

```csharp
using MoralesLarios.OOFP.ValueObjects;

public class CrearUsuarioRequest : MlValidableFp<CrearUsuarioRequest>
{
    public string Email  { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public int    Edad   { get; init; }

    public override MlResult<CrearUsuarioRequest> Validate()
        => Mail.ByString(Email,  "El email no tiene un formato válido")
          .Bind(_ => Name.ByString(Nombre, "El nombre debe tener al menos 3 caracteres"))
          .Bind(_ => Age .ByInt   (Edad,   "La edad no es válida"))
          .Map (_ => this);
}
```

> 💡 **Es el estilo más limpio** cuando ya usas [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md): la regla se define una vez en el value object y se reutiliza en todos los DTOs.

---

## Las tres estrategias de acumulación de errores

Es la decisión más importante al implementar `Validate()`: **¿paras en el primer error o los devuelves todos?**

| Estrategia | Operador | Comportamiento | Cuándo usarla |
|---|---|---|---|
| **Cortocircuito** | `Bind` | Para en el **primer** error | Validaciones dependientes; procesos internos |
| **Acumulación** | `BindMulti` | Ejecuta **todas** y fusiona los errores | Formularios de usuario, APIs públicas |
| **Secuencial con parada** | `TryBindBuildWhile` | Construye acumulando hasta el primer fallo | Composición de varios valores |

### Cortocircuito con `Bind`

```csharp
// El usuario ve UN error: "El email es obligatorio"
public override MlResult<Registro> Validate()
    => EnsureFp.NotNullEmptyOrWhitespace(Email,  "El email es obligatorio")
         .Bind(_ => EnsureFp.NotNullEmptyOrWhitespace(Clave, "La clave es obligatoria"))
         .Map (_ => this);
```

Tiene sentido cuando las reglas **dependen** unas de otras: no valides el formato del email si el email está vacío.

### Acumulación con `BindMulti`

```csharp
// El usuario ve TODOS los errores de golpe
public override MlResult<Registro> Validate()
    => this.ToMlResultValid()
           .BindMulti(r => EnsureFp.NotNullEmptyOrWhitespace(r.Email,  "El email es obligatorio").Map(_ => r),
                      r => EnsureFp.NotNullEmptyOrWhitespace(r.Clave,  "La clave es obligatoria").Map(_ => r),
                      r => EnsureFp.That(r, r.Clave.Length >= 8,       "La clave debe tener 8 caracteres o más"),
                      r => EnsureFp.That(r, r.AceptaTerminos,          "Debes aceptar los términos"));
```

> ⚠️ **`BindMulti` ejecuta *todos* los delegados**, incluso si el primero falla. Asegúrate de que ninguno pueda lanzar sobre un dato nulo: si `Clave` puede ser `null`, `r.Clave.Length` explota. En esos casos combina: `BindMulti` para las reglas independientes y `Bind` para las dependientes.

Más detalle en la documentación de [`Bind` y `BindMulti`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md).

---

## ⚠️ Particularidades reales del código fuente

### 1. Es una clase abstracta, no una interfaz

```csharp
public abstract class MlValidableFp<T> where T : class
```

Consecuencias prácticas:

- **Consume la única posición de clase base.** Un DTO que ya herede de otra clase (por ejemplo una base con auditoría) **no puede** heredar de `MlValidableFp<T>`.
- **No sirve para `record struct` ni `struct`** (por `where T : class`). Los `record` de referencia sí valen, pero recuerda que un `record` que hereda de una clase abstracta pierde parte de la comodidad sintáctica.
- **No hay `IMlValidableFp<T>`.** Si necesitas validar tipos que ya tienen clase base, la alternativa es declarar tu propio método `Validate()` con la misma firma y documentarlo, o usar una interfaz propia; el resto de la solución no exige el tipo base, solo el patrón.

### 2. `Validate()` no tiene implementación por defecto

Es `abstract`, no `virtual`: **cada clase derivada está obligada a escribirla**. No existe un comportamiento heredado del tipo *"si no defines nada, es válido"*.

### 3. El proyecto no valida nada por sí mismo

No hay motor de reglas, ni atributos, ni reflexión, ni caché. Todo el trabajo real lo hace el `Validate()` que escribas tú, o las librerías satélite:

- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — atributos `[Required]`, `[Range]`, `[EmailAddress]`…
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — reglas con `AbstractValidator<T>`

### 4. Nada garantiza que se llame a `Validate()`

No hay filtro, interceptor ni middleware automático. **Si no invocas `Validate()`, el objeto entra sin validar.** Elige un punto fijo (normalmente la primera línea del método del servicio, o del controlador) y aplícalo por convención en todo el proyecto.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay `ValidateAsync()`.** El contrato es síncrono a propósito: `Validate()` solo debe mirar el propio objeto, y eso nunca necesita `await`. Las validaciones asíncronas (unicidad en base de datos, llamadas a otro servicio) van **después**, encadenadas con `BindAsync` en el servicio.

> ⚠️ **No hay validación en cascada automática.** Si tu DTO contiene otros `MlValidableFp<>`, tienes que llamarlos explícitamente (ver [Ejemplo 3](#ejemplo-3--validación-en-cascada-de-objetos-anidados)).

> ⚠️ **No existen** `IsValid`, `Errors`, `TryValidate`, `ValidateAndThrow`, `Validator`, ni atributos de ningún tipo. El único miembro es `Validate()`.

> ⚠️ **No hay integración automática con ASP.NET Core.** El `ModelState` y los `ValidationAttribute` de MVC siguen funcionando por su cuenta; `MlValidableFp<T>` es un mecanismo paralelo y explícito.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Ejemplos prácticos

### Ejemplo 1 — DTO completo con acumulación de errores para un formulario

```csharp
using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation;
using MoralesLarios.OOFP.Helpers;

public class CrearClienteRequest : MlValidableFp<CrearClienteRequest>
{
    public string  Nombre    { get; init; } = string.Empty;
    public string  Email     { get; init; } = string.Empty;
    public string? Telefono  { get; init; }
    public decimal LimiteCredito { get; init; }

    public override MlResult<CrearClienteRequest> Validate()
        => this.ToMlResultValid()
               .BindMulti(
                   r => EnsureFp.NotNullEmptyOrWhitespace(r.Nombre, "El nombre del cliente es obligatorio").Map(_ => r),
                   r => EnsureFp.That(r, r.Nombre.Length <= 100, "El nombre no puede superar los 100 caracteres"),
                   r => EnsureFp.That(r, r.Email.Contains('@'),  "El email no tiene un formato válido"),
                   r => EnsureFp.That(r, r.LimiteCredito >= 0,   "El límite de crédito no puede ser negativo"),
                   r => EnsureFp.That(r, r.Telefono is null || r.Telefono.Length >= 9,
                                         "El teléfono debe tener al menos 9 dígitos"));
}
```

Consumo desde un controlador:

```csharp
[HttpPost]
public IActionResult Crear(CrearClienteRequest dto)
    => dto.Validate()
          .Bind(v => _servicio.Crear(v))
          .Match(valid: creado  => CreatedAtAction(nameof(Obtener), new { creado.Id }, creado),
                 fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
```

### Ejemplo 2 — Separar validación de formato (síncrona) de validación de negocio (asíncrona)

Este es **el patrón recomendado** para el reparto de responsabilidades:

```csharp
public class ServicioClientes
{
    public Task<MlResult<Cliente>> Crear(CrearClienteRequest dto)
        => dto.Validate()                                                  // 1. formato: síncrono, en el DTO
              .BindAsync(async v => await NoExisteEmail(v))                // 2. negocio: asíncrono, en el servicio
              .BindAsync(async v => await _repo.AddAsync(Mapear(v)));      // 3. persistencia

    private async Task<MlResult<CrearClienteRequest>> NoExisteEmail(CrearClienteRequest dto)
        => await _repo.AnyAsync(c => c.Email == dto.Email)
               ? $"Ya existe un cliente con el email '{dto.Email}'".ToMlResultFail<CrearClienteRequest>()
               : dto.ToMlResultValid();
}
```

> 💡 **Regla de reparto**: si la regla se puede comprobar **sin salir del objeto**, va en `Validate()`. Si necesita base de datos, hora del sistema, configuración o otro servicio, va en el servicio.

### Ejemplo 3 — Validación en cascada de objetos anidados

No es automática: hay que orquestarla. Para una colección, `Projection` valida todos los elementos y fusiona los errores.

```csharp
public class LineaPedido : MlValidableFp<LineaPedido>
{
    public int     ProductoId { get; init; }
    public int     Cantidad   { get; init; }

    public override MlResult<LineaPedido> Validate()
        => EnsureFp.That(this, ProductoId > 0, "La línea debe referenciar un producto")
             .Bind(l => EnsureFp.That(l, l.Cantidad > 0, $"La cantidad del producto {l.ProductoId} debe ser mayor que 0"));
}

public class Pedido : MlValidableFp<Pedido>
{
    public int                    ClienteId { get; init; }
    public List<LineaPedido>      Lineas    { get; init; } = new();
    public DireccionEnvio?        Envio     { get; init; }

    public override MlResult<Pedido> Validate()
        => EnsureFp.That(this, ClienteId > 0, "El pedido debe tener un cliente")
             .Bind(p => EnsureFp.NotEmpty(p.Lineas, "El pedido debe tener al menos una línea"))
             .Bind(_ => Lineas.Projection(l => l.Validate()))        // valida TODAS las líneas
             .Bind(_ => Envio is null
                            ? "La dirección de envío es obligatoria".ToMlResultFail<DireccionEnvio>()
                            : Envio.Validate())                       // valida el objeto anidado
             .Map (_ => this);
}
```

> 💡 [`Projection`](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) recorre la colección, valida cada elemento y **fusiona todos los errores** en un único `MlErrorsDetails`: el usuario ve de golpe qué líneas están mal.

### Ejemplo 4 — Añadir contexto al error para diagnóstico

```csharp
public override MlResult<ImportarFilaRequest> Validate()
    => EnsureFp.That(this, Importe > 0, "El importe debe ser positivo")
         .Map(_ => this)
         .AddMlErrorDetailIfFail("numeroFila", NumeroFila)      // detalle extra solo si falla
         .AddMlErrorDetailIfFail("fichero",    NombreFichero);

// Al consumirlo
resultado.Match(
    valid: fila    => Procesar(fila),
    fail : errores => _logger.LogWarning("Fila inválida: {Msg} | {Detalles}",
                                         errores.Errors.First().Message,
                                         errores.ToDetailsDescription()));
```

### Ejemplo 5 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: lanzar excepciones desde Validate()
public override MlResult<Dto> Validate()
{
    if (Edad < 0) throw new ArgumentException("Edad inválida");     // rompe el modelo funcional
    return this.ToMlResultValid();
}

// ✅ BIEN: devolver el fallo como valor
public override MlResult<Dto> Validate()
    => EnsureFp.That(this, Edad >= 0, "La edad no puede ser negativa");


// ❌ MAL: devolver un objeto nuevo o distinto de this
public override MlResult<Dto> Validate() => new Dto { Edad = 0 }.ToMlResultValid();

// ✅ BIEN: devolver this
public override MlResult<Dto> Validate() => this.ToMlResultValid();


// ❌ MAL: consultar la base de datos dentro de Validate()
public override MlResult<Dto> Validate()
    => _repo.Existe(Email) ? "Email duplicado".ToMlResultFail<Dto>() : this.ToMlResultValid();

// ✅ BIEN: eso va en el servicio, encadenado después
// dto.Validate().BindAsync(v => _repo.NoExisteEmailAsync(v))


// ❌ MAL: ignorar el resultado de Validate()
dto.Validate();                    // el valor de retorno se descarta: no valida nada
Guardar(dto);

// ✅ BIEN: encadenar sobre el resultado
dto.Validate().Bind(Guardar);


// ❌ MAL: parámetro genérico que no es la propia clase
public class Pedido : MlValidableFp<Cliente> { … }     // compila, pero rompe la convención

// ✅ BIEN
public class Pedido : MlValidableFp<Pedido> { … }


// ❌ MAL: BindMulti con reglas que dependen de una anterior
this.ToMlResultValid().BindMulti(
    r => EnsureFp.NotNull(r.Clave, "Clave obligatoria").Map(_ => r),
    r => EnsureFp.That(r, r.Clave.Length >= 8, "Clave corta"));   // 💥 NRE si Clave es null

// ✅ BIEN: Bind para las dependientes, BindMulti para las independientes
EnsureFp.NotNull(Clave, "Clave obligatoria")
        .Bind(_ => this.ToMlResultValid()
                       .BindMulti(r => EnsureFp.That(r, r.Clave.Length >= 8, "Clave corta"),
                                  r => EnsureFp.That(r, r.AceptaTerminos,    "Acepta los términos")));
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Un contrato uniforme de validación | Heredar de `MlValidableFp<T>` |
| Parar en el primer error | `Bind` entre comprobaciones |
| Devolver todos los errores de un formulario | `BindMulti` |
| Comprobar una condición suelta | `EnsureFp.That(obj, condicion, "mensaje")` |
| Comprobar no nulo / no vacío | `EnsureFp.NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace` |
| Validar cada elemento de una colección | `coleccion.Projection(x => x.Validate())` |
| Validar con atributos `[Required]`, `[Range]`… | [`Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| Validar con reglas `AbstractValidator<T>` | [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| Reglas reutilizables por tipo de dato | [`ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) |
| Validaciones que consultan la BD | En el servicio, con `BindAsync` |

---

## Mejores prácticas

1. **Usa siempre el patrón autorreferencial**: `class X : MlValidableFp<X>`.
2. **Devuelve `this` en el caso válido**, para que la cadena siga con el objeto validado.
3. **Mantén `Validate()` pura**: sin I/O, sin mutación, sin excepciones, sin dependencias inyectadas.
4. **Reparte responsabilidades**: formato y coherencia interna en `Validate()`; unicidad y reglas de negocio con estado externo, en el servicio.
5. **Elige la estrategia de errores según el consumidor**: `BindMulti` para humanos, `Bind` para procesos internos.
6. **Con `BindMulti`, asegúrate de que ninguna regla pueda lanzar** sobre datos nulos.
7. **Escribe los mensajes en el idioma del usuario final** y de forma accionable: *"El email no tiene un formato válido"*, no *"Email inválido"*.
8. **Extrae las reglas repetidas a value objects** en lugar de duplicarlas en cada DTO.
9. **Llama a `Validate()` en un punto fijo y documentado** (primera línea del método del servicio es la opción más segura): nada lo hace automáticamente.
10. **Añade contexto con `AddMlErrorDetailIfFail`** cuando el error deba diagnosticarse en logs (nº de fila, identificador, fichero…).
11. **No dupliques la validación** con los `ValidationAttribute` de MVC: decide un mecanismo y sé coherente.
12. **Prueba `Validate()` con test unitarios puros**: no necesita mocks ni base de datos, es el tipo de código más fácil y rentable de testear.

---

## Resumen

- `MoralesLarios.OOFP.Validation` contiene **un único tipo**: `public abstract class MlValidableFp<T> where T : class`, con **un único miembro**: `public abstract MlResult<T> Validate()`.
- Su función es **normalizar la validación**: todo objeto validable falla igual y encadena igual que el resto de la solución.
- Se usa con el patrón autorreferencial `class X : MlValidableFp<X>` y devolviendo `this` cuando es válido.
- **Cortocircuito con `Bind`**, **acumulación con `BindMulti`**: la elección depende de si el error lo lee un humano o un proceso.
- `Validate()` debe ser **pura y síncrona**; las reglas que necesitan la base de datos se encadenan después con `BindAsync`.
- Es la base de [`Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) y [`Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md), que aportan los motores de reglas.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — validación con atributos
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — validación con FluentValidation
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — reglas reutilizables encapsuladas en tipos
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — servicios genéricos donde encadenar la validación
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — conversión de `MlErrorsDetails` a respuestas HTTP

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores y detalles](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — comprobaciones que devuelven `MlResult`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Bind` y `BindMulti` — cortocircuito y acumulación](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` — transformar el valor válido](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — convertir el resultado en respuesta](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [`Projection` — validar colecciones enteras](../MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)
