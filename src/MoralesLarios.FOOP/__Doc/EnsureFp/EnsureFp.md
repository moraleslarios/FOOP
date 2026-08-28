# EnsureFp — Guardas de entrada al carril funcional

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [No es una extensión: es una clase estática](#no-es-una-extensión-es-una-clase-estática)
4. [Inventario completo de la API](#inventario-completo-de-la-api)
5. [`That` — el método base](#that--el-método-base)
6. [Los tres atajos: `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace`](#los-tres-atajos-notnull-notempty-notnullemptyorwhitespace)
7. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
8. [Variantes asíncronas](#variantes-asíncronas)
9. [`EnsureFp` frente a `NullToFailed`, `EmptyToFailed` y `BoolToResult`](#ensurefp-frente-a-nulltofailed-emptytofailed-y-booltoresult)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`EnsureFp` es una clase estática de ayuda que cumple el papel de las **guardas clásicas**
(`ArgumentNullException.ThrowIfNull`, `Guard.Against...`) pero **sin lanzar excepciones**:
en lugar de romper el flujo, devuelve un `MlResult<T>`.

```csharp
// ❌ Guardas imperativas: excepciones que hay que capturar arriba
public Factura Emitir(Pedido pedido, string serie)
{
    ArgumentNullException.ThrowIfNull(pedido);
    if (string.IsNullOrWhiteSpace(serie)) throw new ArgumentException(nameof(serie));
    if (!pedido.Lineas.Any())             throw new InvalidOperationException("Sin líneas");
    // …
}

// ✅ Con EnsureFp: el error es un valor, la tubería sigue siendo funcional
public MlResult<Factura> Emitir(Pedido pedido, string serie)
    => EnsureFp.NotNull(pedido, "El pedido es obligatorio")
               .Bind(p => EnsureFp.NotNullEmptyOrWhitespace(serie, "La serie es obligatoria")
                                  .Map(s => (p, s)))
               .Bind(t => EnsureFp.NotEmpty(t.p.Lineas, "El pedido no tiene líneas")
                                  .Map(_ => t))
               .Map(t => Construir(t.p, t.s));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema que resuelve

Todos los operadores del carril (`Map`, `Bind`, `MapEnsure`, `ExecSelf`…) son extensiones de
`MlResult<T>`: **necesitan que ya estés dentro del carril**. Pero cuando escribes un método
público, los argumentos llegan como valores desnudos de C#.

`EnsureFp` resuelve ese primer paso: **valida un argumento y te deja dentro del carril**.

```
Argumentos de C#  ──[ EnsureFp ]──►  MlResult<T>  ──[ Map / Bind / ... ]──►  MlResult<TResult>
   (mundo OO)                          (carril funcional)
```

---

## No es una extensión: es una clase estática

Este es el detalle que más despista al principio. `EnsureFp` **no** contiene métodos de
extensión, sino métodos estáticos normales:

```csharp
public static class EnsureFp
{
    public static MlResult<T> That<T>(T value, bool condition, string errorMessage) => /* … */;
    public static MlResult<T> NotNull<T>(T value, string errorMessage)              => /* … */;
    // …
}
```

Por tanto siempre se invoca con el nombre de la clase delante:

```csharp
// ✅ Correcto
var r = EnsureFp.NotNull(cliente, "El cliente es obligatorio");

// ❌ No compila: no es un método de extensión
// var r = cliente.NotNull("El cliente es obligatorio");

// ✅ Si quieres sintaxis de extensión, usa los métodos de Several
var r = cliente.NullToFailed("El cliente es obligatorio");
```

💡 **Consejo:** añade `using static MoralesLarios.OOFP.Helpers.EnsureFp;` en los archivos con
muchas validaciones y escribe directamente `NotNull(...)`, `That(...)`.

---

## Inventario completo de la API

La clase tiene exactamente **14 métodos** (7 síncronos + 7 asíncronos), y cada uno viene en
dos sabores según cómo expreses el error (`string` o `MlErrorsDetails`):

| Método | Condición que comprueba | Devuelve |
|--------|------------------------|----------|
| `That<T>(value, condition, error)` | La `condition` que tú indiques | `MlResult<T>` |
| `NotNull<T>(value, error)` | `value is not null` | `MlResult<T>` |
| `NotEmpty<T>(value, error)` | `value != null && value.Any()` | `MlResult<IEnumerable<T>>` |
| `NotNullEmptyOrWhitespace(value, error)` | `!string.IsNullOrWhiteSpace(value)` | `MlResult<string>` |

Más sus cuatro equivalentes asíncronos: `ThatAsync`, `NotNullAsync`, `NotEmptyAsync`,
`NotNullEmptyOrWhitespaceAsync`.

⚠️ **No hay más.** No existen `NotDefault`, `InRange`, `Positive`, `Matches`, `MinLength`
ni ninguna otra guarda especializada. Para el resto de comprobaciones se usa `That`.

⚠️ **Solo dos formas de expresar el error**: `string` y `MlErrorsDetails`. A diferencia de
`BoolToResult` o `NullToFailed`, aquí **no hay sobrecargas para `MlError` ni para
`IEnumerable<string>`**.

```csharp
// ✅ Las dos formas disponibles
EnsureFp.NotNull(cliente, "El cliente es obligatorio");
EnsureFp.NotNull(cliente, MlErrorsDetails.FromErrorMessageDetails(
                              "El cliente es obligatorio",
                              new Dictionary<string, object> { ["Parametro"] = "cliente" }));

// ❌ No existen estas sobrecargas
// EnsureFp.NotNull(cliente, ErroresCliente.Obligatorio);        // MlError
// EnsureFp.NotNull(cliente, new[] { "msg1", "msg2" });          // IEnumerable<string>

// ✅ Convierte a MlErrorsDetails si necesitas esas formas
EnsureFp.NotNull(cliente, MlErrorsDetails.FromError(ErroresCliente.Obligatorio));
EnsureFp.NotNull(cliente, MlErrorsDetails.FromEnumerableStrings(new[] { "msg1", "msg2" }));
```

---

## `That` — el método base

Todos los demás métodos delegan en `That`. Es la guarda genérica:

```csharp
public static MlResult<T> That<T>(T value, bool condition, string errorMessage)
    => condition ? MlResult<T>.Valid(value) : MlResult<T>.Fail(errorMessage);

public static MlResult<T> That<T>(T value, bool condition, MlErrorsDetails errorsDetails)
    => condition ? MlResult<T>.Valid(value) : errorsDetails.ToMlResultFail<T>();
```

Si la condición se cumple, el valor entra en el carril tal cual; si no, el resultado es
fallido con tu error.

```csharp
// Cualquier regla que no tenga atajo se expresa con That
EnsureFp.That(edad,     edad is >= 18 and <= 120,       "La edad debe estar entre 18 y 120");
EnsureFp.That(importe,  importe > 0,                     "El importe debe ser positivo");
EnsureFp.That(nif,      RegexNif.IsMatch(nif),           "El NIF no tiene un formato válido");
EnsureFp.That(fecha,    fecha <= DateTime.UtcNow,        "La fecha no puede ser futura");
EnsureFp.That(pagina,   pagina >= 1,                     "La página debe ser 1 o mayor");
```

⚠️ Como en `BoolToResult`, la `condition` es un **`bool` ya evaluado**, no un delegado: se
calcula antes de entrar al método. Con validaciones de argumentos (siempre baratas) esto no
supone ningún problema.

---

## Los tres atajos: `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace`

### `NotNull<T>`

```csharp
public static MlResult<T> NotNull<T>(T value, string errorMessage)
    => That(value, value is not null, errorMessage);
```

🔑 Usa **`value is not null`**, no `value == null`. Esta diferencia importa: el patrón
`is not null` **ignora cualquier `operator ==` sobrecargado** y comprueba la referencia real.
Es más seguro que la implementación de [`NullToFailed`](../Several/2_NullToFailed.md), que
sí usa `== null` y por tanto puede verse afectada por operadores sobrecargados.

⚠️ El parámetro es `T value` sin restricción `where T : class`, así que puedes pasar un
value type — pero entonces la comprobación es inútil (nunca será `null`) y el compilador
puede avisarte.

### `NotEmpty<T>`

```csharp
public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, string message)
    => That(value, value != null && value.Any(), message);
```

Comprueba **`null` y vacío a la vez** — igual que
[`EmptyToFailed`](../Several/1_EmptyToFailed.md), del que es el gemelo estático.

⚠️ Invoca `.Any()`, que **enumera el primer elemento**. Con una consulta LINQ diferida o un
`IEnumerable` de un solo uso, esto puede tener efectos:

```csharp
// ⚠️ La consulta se ejecuta aquí, y otra vez al recorrerla después
var r = EnsureFp.NotEmpty(_db.Clientes.Where(c => c.Activo), "Sin clientes activos");

// ✅ Materializa antes
var activos = _db.Clientes.Where(c => c.Activo).ToList();
var r = EnsureFp.NotEmpty(activos, "Sin clientes activos");
```

⚠️ El retorno es `MlResult<IEnumerable<T>>`, no `MlResult<List<T>>`: si le pasas una lista,
pierdes el tipo concreto.

### `NotNullEmptyOrWhitespace`

```csharp
public static MlResult<string> NotNullEmptyOrWhitespace(string value, string errorMessage)
     => That(value, !string.IsNullOrWhiteSpace(value), errorMessage);
```

La guarda para cadenas: rechaza `null`, `""` y `"   "`. Es el atajo más usado en la práctica,
porque casi todos los identificadores, códigos y nombres que llegan de fuera son cadenas.

⚠️ **No recorta la cadena.** Si el valor es `"  ABC  "`, pasa la validación y sigue con los
espacios. Haz el `Trim()` tú:

```csharp
var r = EnsureFp.NotNullEmptyOrWhitespace(nif, "El NIF es obligatorio")
                .Map(s => s.Trim().ToUpperInvariant());
```

---

## ⚠️ Particularidades reales del código fuente

**1. Solo `string` y `MlErrorsDetails` como forma de error.** Ya comentado: no hay
sobrecargas para `MlError` ni `IEnumerable<string>`.

**2. Solo 4 validaciones.** `That`, `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace`. Todo
lo demás se construye con `That`.

**3. `NotNull` usa `is not null` (bueno); `NotEmpty` usa `!= null` (menos estricto).**
Inconsistencia real del código, sin consecuencias prácticas en el 99 % de los casos.

**4. Las variantes asíncronas no son realmente asíncronas.** Todas se limitan a envolver el
resultado síncrono con `.ToAsync()` (es decir, `Task.FromResult`). **No aportan
concurrencia**; su única utilidad es encajar en una cadena que ya es asíncrona.

```csharp
// Implementación real: no hay nada que esperar
public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, MlErrorsDetails errorsDetails)
    => condition ? MlResult<T>.Valid(value).ToAsync() : errorsDetails.ToMlResultFail<T>().ToAsync();
```

**5. Ninguna sobrecarga acepta un predicado asíncrono.** No existe
`ThatAsync(value, Func<Task<bool>>, ...)`. Si tu condición requiere una consulta, resuélvela
antes:

```csharp
// ✅ El await es tuyo
var existe = await _repo.ExisteAsync(nif);
var r = EnsureFp.That(nif, !existe, $"Ya existe un cliente con NIF {nif}");
```

**6. Dos métodos tienen cuerpo con bloque en lugar de expresión.** `ThatAsync(string)` y
`NotNullAsync(string)` están escritos con `{ var result = …; return result; }` y hay código
comentado justo encima. Es puramente cosmético: el comportamiento es idéntico al de sus
hermanos.

**7. `NotNullAsync(value, MlErrorsDetails)` no es `async`**: devuelve directamente el `Task`
de `ThatAsync`. Otra asimetría cosmética.

**8. No existe ninguna variante `Try*`.** `EnsureFp` no invoca delegados de usuario, así que
no hay excepciones que capturar.

---

## Variantes asíncronas

| Método | Naturaleza real |
|--------|----------------|
| `ThatAsync(value, condition, string \| MlErrorsDetails)` | Envoltura `ToAsync()` |
| `NotNullAsync(value, string \| MlErrorsDetails)` | Envoltura |
| `NotEmptyAsync(value, string \| MlErrorsDetails)` | Envoltura |
| `NotNullEmptyOrWhitespaceAsync(value, string \| MlErrorsDetails)` | Envoltura |

Su uso natural es **abrir una tubería asíncrona** sin tener que insertar un `.ToAsync()`
manual:

```csharp
public Task<MlResult<ClienteDto>> ObtenerAsync(string nif)
    => EnsureFp.NotNullEmptyOrWhitespaceAsync(nif, "El NIF es obligatorio")
               .BindAsync(n => _repo.BuscarPorNifAsync(n)
                                    .NullToFailedAsync($"No existe cliente con NIF {n}"))
               .MapAsync(c => c.ToDto().ToAsync());
```

---

## `EnsureFp` frente a `NullToFailed`, `EmptyToFailed` y `BoolToResult`

Las cuatro herramientas hacen prácticamente lo mismo; la diferencia es **la sintaxis y el
punto de uso**:

| Herramienta | Forma | Formas de error | Comprobación de `null` |
|-------------|-------|-----------------|------------------------|
| `EnsureFp.NotNull` | Estático | `string`, `MlErrorsDetails` | `is not null` (estricto) |
| [`NullToFailed`](../Several/2_NullToFailed.md) | Extensión | 4 formas | `== null` (respeta `operator==`) |
| `EnsureFp.NotEmpty` | Estático | `string`, `MlErrorsDetails` | `!= null && Any()` |
| [`EmptyToFailed`](../Several/1_EmptyToFailed.md) | Extensión | 3 formas | `!= null && Any()` |
| `EnsureFp.That` | Estático | `string`, `MlErrorsDetails` | — |
| [`BoolToResult`](../Several/3_BoolToResult.md) | Extensión | 4 formas | — |
| [`MapEnsure`](../Map/2_MapEnsure.md) | Extensión de `MlResult<T>` | varias | Predicado **diferido** |

🔑 **Criterio práctico:**

- **Al entrar en un método público**, con argumentos sueltos → `EnsureFp`. El prefijo
  `EnsureFp.` deja visualmente claro que es una guarda de precondición.
- **Ya dentro del carril** → `MapEnsure`, que además tiene predicado diferido.
- **Si prefieres sintaxis fluida desde el primer momento** → los métodos de
  [`Several`](../Several/1_EmptyToFailed.md), que son extensiones.

```csharp
// Estilo A: EnsureFp para la puerta de entrada (guardas explícitas)
public MlResult<Recibo> Emitir(Pedido pedido, string serie)
    => EnsureFp.NotNull(pedido, "El pedido es obligatorio")
               .MapEnsure(p => p.Lineas.Any(), "El pedido no tiene líneas")
               .Map(p => Construir(p, serie));

// Estilo B: todo fluido con las extensiones de Several
public MlResult<Recibo> Emitir(Pedido pedido, string serie)
    => pedido.NullToFailed("El pedido es obligatorio")
             .MapEnsure(p => p.Lineas.Any(), "El pedido no tiene líneas")
             .Map(p => Construir(p, serie));
```

Ambos son correctos. Elige uno **y sé consistente en todo el proyecto**.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Validar un argumento no nulo al entrar en un método | `EnsureFp.NotNull(x, "...")` |
| Validar una cadena obligatoria | `EnsureFp.NotNullEmptyOrWhitespace(s, "...")` |
| Validar que una colección trae elementos | `EnsureFp.NotEmpty(items, "...")` |
| Cualquier otra regla sobre un argumento | `EnsureFp.That(x, condición, "...")` |
| Lo mismo, abriendo una tubería asíncrona | `EnsureFp.*Async(...)` |
| Validar con detalles de diagnóstico | Sobrecarga con `MlErrorsDetails` |
| Usar un `MlError` de catálogo | `MlErrorsDetails.FromError(err)` |
| Validar **ya dentro** del carril | [`MapEnsure`](../Map/2_MapEnsure.md) |
| Sintaxis fluida en lugar de estática | [`NullToFailed`](../Several/2_NullToFailed.md), [`BoolToResult`](../Several/3_BoolToResult.md) |
| Validar con reglas de FluentValidation o DataAnnotations | Paquetes `MoralesLarios.OOFP.Validation.*` |

---

## Ejemplos Prácticos

### Ejemplo 1: guardas de un método de servicio

```csharp
public class PedidoService
{
    public MlResult<Pedido> Crear(int clienteId, string referencia, IEnumerable<LineaDto> lineas)
        => EnsureFp.That(clienteId, clienteId > 0, "El identificador de cliente debe ser positivo")
                   .Bind(_ => EnsureFp.NotNullEmptyOrWhitespace(referencia, "La referencia es obligatoria"))
                   .Map(r => r.Trim().ToUpperInvariant())
                   .Bind(r => EnsureFp.That(r, r.Length <= 20, "La referencia no puede superar 20 caracteres"))
                   .Bind(r => EnsureFp.NotEmpty(lineas, "El pedido debe tener al menos una línea")
                                      .Map(ls => (Referencia: r, Lineas: ls)))
                   .Bind(t => EnsureFp.That(t, t.Lineas.Count() <= 200,
                                            "El pedido no puede tener más de 200 líneas"))
                   .Map(t => new Pedido(clienteId, t.Referencia, t.Lineas.Select(Convertir).ToList()));
}
```

### Ejemplo 2: `using static` para aligerar la sintaxis

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public class ReservaService
{
    public MlResult<Reserva> Reservar(string sala, DateTime inicio, TimeSpan duracion, int asistentes)
        => NotNullEmptyOrWhitespace(sala, "La sala es obligatoria")
              .Bind(s => That(s, _salas.Existe(s), $"La sala '{s}' no existe"))
              .Bind(s => That(s, inicio > DateTime.UtcNow, "La fecha de inicio debe ser futura"))
              .Bind(s => That(s, duracion >= TimeSpan.FromMinutes(15),
                                 "La duración mínima es de 15 minutos"))
              .Bind(s => That(s, duracion <= TimeSpan.FromHours(8),
                                 "La duración máxima es de 8 horas"))
              .Bind(s => That(s, asistentes is > 0 and <= 50,
                                 "El número de asistentes debe estar entre 1 y 50"))
              .Map(s => new Reserva(s, inicio, duracion, asistentes));
}
```

### Ejemplo 3: guardas con detalles para el controlador

```csharp
public class ArticuloService
{
    private static MlErrorsDetails ErrorParametro(string parametro, string mensaje, object? valor = null)
        => MlErrorsDetails.FromErrorMessageDetails(mensaje, new Dictionary<string, object>
        {
            ["Parametro"]     = parametro,
            ["ValorRecibido"] = valor ?? "(null)",
            ["Categoria"]     = "ValidacionEntrada"
        });

    public async Task<MlResult<PaginaDto<ArticuloDto>>> BuscarAsync(string? texto, int pagina, int tamano)
        => await EnsureFp.NotNullEmptyOrWhitespaceAsync(texto!,
                             ErrorParametro(nameof(texto), "El texto de búsqueda es obligatorio", texto))
                         .BindAsync(t => EnsureFp.ThatAsync(t, t.Trim().Length >= 3,
                             ErrorParametro(nameof(texto), "El texto debe tener al menos 3 caracteres", t)))
                         .BindAsync(t => EnsureFp.ThatAsync(t, pagina >= 1,
                             ErrorParametro(nameof(pagina), "La página debe ser 1 o mayor", pagina)))
                         .BindAsync(t => EnsureFp.ThatAsync(t, tamano is > 0 and <= 100,
                             ErrorParametro(nameof(tamano), "El tamaño debe estar entre 1 y 100", tamano)))
                         .BindAsync(t => _repo.BuscarAsync(t.Trim(), pagina, tamano))
                         .MapAsync(p => p.ToDto().ToAsync());
}
```

El controlador puede leer `GetDetailValue<string>("Categoria")` para devolver un 400 con la
lista de parámetros problemáticos.

### Ejemplo 4: condición que requiere una consulta

```csharp
public async Task<MlResult<Cliente>> AltaAsync(AltaDto dto)
{
    // Primero las guardas puramente sintácticas
    var basico = EnsureFp.NotNull(dto, "Los datos de alta son obligatorios")
                         .Bind(d => EnsureFp.NotNullEmptyOrWhitespace(d.Nif, "El NIF es obligatorio")
                                            .Map(_ => d));

    if (!basico.IsValid) return basico.ErrorsDetails.ToMlResultFail<Cliente>();

    // La condición asíncrona se resuelve fuera: EnsureFp no acepta predicados asíncronos
    var yaExiste = await _repo.ExisteNifAsync(dto.Nif);

    return EnsureFp.That(dto, !yaExiste, $"Ya existe un cliente con el NIF {dto.Nif}")
                   .Map(d => new Cliente(d.Nif, d.Nombre));
}
```

💡 Si prefieres no romper la cadena, usa `BindAsync` con un lambda asíncrono y
`BoolToResult` dentro.

### Ejemplo 5: qué no hacer

```csharp
// ❌ Llamarlo como método de extensión: no compila
// var r = cliente.NotNull("El cliente es obligatorio");

// ✅ Prefijo de clase, o usa NullToFailed
var r = EnsureFp.NotNull(cliente, "El cliente es obligatorio");


// ❌ Pasar un MlError: no hay sobrecarga
// EnsureFp.NotNull(cliente, ErroresCliente.Obligatorio);

// ✅ Conviértelo
EnsureFp.NotNull(cliente, MlErrorsDetails.FromError(ErroresCliente.Obligatorio));


// ❌ Esperar concurrencia de las variantes Async
await EnsureFp.NotNullAsync(a, "…");    // no hay E/S: es Task.FromResult

// ✅ Úsalas solo para encajar en una cadena ya asíncrona


// ❌ NotEmpty sobre una consulta diferida (se enumera dos veces)
var r = EnsureFp.NotEmpty(_db.Pedidos.Where(p => p.Abierto), "Sin pedidos abiertos");

// ✅ Materializa primero
var abiertos = _db.Pedidos.Where(p => p.Abierto).ToList();
var r = EnsureFp.NotEmpty(abiertos, "Sin pedidos abiertos");


// ❌ Suponer que NotNullEmptyOrWhitespace recorta la cadena
var r = EnsureFp.NotNullEmptyOrWhitespace(nif, "…");   // "  X  " pasa tal cual

// ✅ Normaliza después
var r = EnsureFp.NotNullEmptyOrWhitespace(nif, "…").Map(s => s.Trim().ToUpperInvariant());


// ❌ Usar EnsureFp dentro del carril, obligando a un Bind ceremonial
var r = pedidoResult.Bind(p => EnsureFp.That(p, p.Lineas.Any(), "Sin líneas"));

// ✅ MapEnsure es más directo y su predicado es diferido
var r = pedidoResult.MapEnsure(p => p.Lineas.Any(), "Sin líneas");
```

---

## Mejores Prácticas

1. **Usa `EnsureFp` en la primera línea de los métodos públicos**: es la puerta de entrada
   natural al carril y el prefijo hace evidente que se trata de precondiciones.
2. **Dentro del carril, cambia a `MapEnsure`**: evita el `Bind` ceremonial y su predicado sí
   es diferido.
3. **Elige un estilo y sé consistente**: o `EnsureFp.*` (estático) o los métodos de
   `Several` (fluidos). Mezclarlos sin criterio confunde.
4. **Recuerda que solo hay `string` y `MlErrorsDetails`**: convierte con
   `MlErrorsDetails.FromError(...)` o `FromEnumerableStrings(...)` si necesitas otras formas.
5. **Incluye el nombre del parámetro en los `Details`** (`["Parametro"] = nameof(nif)`): es
   lo que permite construir respuestas 400 con detalle por campo.
6. **Materializa las colecciones antes de `NotEmpty`** para no enumerar dos veces.
7. **Normaliza las cadenas después de validarlas** (`Trim`, `ToUpperInvariant`): la guarda no
   lo hace.
8. **No esperes concurrencia de las variantes `Async`**: son envolturas. Úsalas solo para
   abrir una cadena asíncrona.
9. **Resuelve las condiciones asíncronas fuera** y pásalas como `bool` a `That`.
10. **Considera `using static ...EnsureFp;`** en las clases con muchas validaciones.
11. **Para validaciones declarativas complejas** (atributos, reglas encadenadas), usa los
    paquetes `MoralesLarios.OOFP.Validation.Dataannotations` o
    `MoralesLarios.OOFP.Validation.FluentValidations`.

---

## Resumen

- `EnsureFp` es una **clase estática** (no extensiones) que convierte argumentos de C# en
  `MlResult<T>` **sin lanzar excepciones**.
- Tiene exactamente **4 validaciones**: `That` (base), `NotNull`, `NotEmpty`,
  `NotNullEmptyOrWhitespace`; cada una con variante `*Async` → **14 métodos** en total
  contando las dos formas de error.
- ⚠️ Solo acepta **`string` y `MlErrorsDetails`** como error. **No hay** sobrecargas para
  `MlError` ni `IEnumerable<string>`.
- `NotNull` usa `is not null` (estricto, ignora `operator==` sobrecargado); `NotEmpty` usa
  `!= null && Any()`.
- ⚠️ Las variantes **`*Async` no son realmente asíncronas**: solo envuelven con `.ToAsync()`.
- ⚠️ **Ninguna acepta un predicado asíncrono**: resuelve la condición antes de llamar.
- ⚠️ `NotEmpty` **enumera** con `.Any()`; materializa las consultas diferidas primero.
- ⚠️ `NotNullEmptyOrWhitespace` **no recorta** la cadena.
- **No existe ninguna variante `Try*`**: no hay delegados de usuario.
- Dentro del carril, prefiere [`MapEnsure`](../Map/2_MapEnsure.md).