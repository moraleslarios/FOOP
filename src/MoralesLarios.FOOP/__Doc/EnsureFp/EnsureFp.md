# EnsureFp — Guardas de entrada al carril funcional

> Documento índice de la familia. Archivos fuente: `Helpers/EnsureFp.cs`,
> `EnsureFp.Core.cs`, `EnsureFp.Aggregation.cs`, `EnsureFp.Strings.cs`, `EnsureFp.Numbers.cs`,
> `EnsureFp.Collections.cs`, `EnsureFp.Types.cs`, `EnsureFp.Async.cs`, `EnsureFpMessages.cs`.

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [No es una extensión: es una clase estática](#no-es-una-extensión-es-una-clase-estática)
4. [Mapa de la familia: las nueve páginas](#mapa-de-la-familia-las-nueve-páginas)
5. [Las tres variantes de cada regla](#las-tres-variantes-de-cada-regla)
6. [Panorámica de la API por familias](#panorámica-de-la-api-por-familias)
7. [`EnsureFp` frente a `NullToFailed`, `EmptyToFailed` y `BoolToResult`](#ensurefp-frente-a-nulltofailed-emptytofailed-y-booltoresult)
8. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
9. [Ejemplos prácticos](#ejemplos-prácticos)
10. [Mejores prácticas](#mejores-prácticas)
11. [Resumen](#resumen)
12. [Ver también](#ver-también)

---

## Introducción

`EnsureFp` es la clase estática de validación de la librería. Cumple el papel de las **guardas
clásicas** (`ArgumentNullException.ThrowIfNull`, `Guard.Against…`) pero **sin lanzar excepciones**:
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

// ✅ Con EnsureFp: el error es un valor y todas las reglas se acumulan
public MlResult<Factura> Emitir(Pedido pedido, string serie)
    => EnsureFp.All(pedido,
                    p => EnsureFp.NotNullArg(p),
                    p => EnsureFp.CountAtLeastArg(p.Lineas, 1).Map(_ => p),
                    p => EnsureFp.NotNullEmptyOrWhitespaceArg(serie).Map(_ => p))
               .Map(p => Construir(p, serie));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`,
> `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`,
> `ToDetailsDescription()`.

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
extensión, sino métodos estáticos normales, repartidos en varios ficheros de una misma
`static partial class`:

```csharp
namespace MoralesLarios.OOFP.Helpers;

public static partial class EnsureFp   // Core, Aggregation, Strings, Numbers, Collections, Types, Async
{
    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, string errorMessage) => /* … */;
    public static MlResult<T> NotNullArg<T>(T value, [CallerArgumentExpression(nameof(value))] string? paramName = null) => /* … */;
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
muchas validaciones y escribe directamente `NotNullArg(...)`, `MaxLength(...)`, `InRange(...)`.

---

## Mapa de la familia: las nueve páginas

La API es amplia, así que está documentada por familias. Cada página incluye el inventario
completo de firmas, la semántica defensiva, los detalles emitidos y ejemplos.

| # | Página | Contenido |
|---|--------|-----------|
| 1 | [`1_EnsureFpCore.md`](./1_EnsureFpCore.md) | `That` (condición, predicado y mensajes perezosos), `TryThat`, guardas `*Arg` con `[CallerArgumentExpression]`, `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace` |
| 2 | [`2_EnsureFpAggregation.md`](./2_EnsureFpAggregation.md) | `All`, `AllResults`, `AllOrFirst`, `Any` y sus versiones asíncronas: acumular **todos** los errores en lugar de detenerse en el primero |
| 3 | [`3_EnsureFpStrings.md`](./3_EnsureFpStrings.md) | Longitudes, `Matches`/`NotMatches` con timeout, `StartsWith`, `EndsWith`, `ContainsText`, `IsOneOf` |
| 4 | [`4_EnsureFpNumbers.md`](./4_EnsureFpNumbers.md) | Comparaciones (`IComparable<T>`), rangos, signo y cero (`INumber<T>`) |
| 5 | [`5_EnsureFpCollections.md`](./5_EnsureFpCollections.md) | `NotEmptyCollection` conservando el tipo, cardinalidad, `AllMatch`/`NoneMatch`/`AnyMatch` con índices fallidos, duplicados, nulos, `ContainsItem` |
| 6 | [`6_EnsureFpTypes.md`](./6_EnsureFpTypes.md) | `Guid`, enumerados, fechas (`InFuture`/`InPast`), `NotDefault`, `Uri`, email, ficheros y directorios |
| 7 | [`7_EnsureFpNullables.md`](./7_EnsureFpNullables.md) | `NotNullValue` y `NotNullValueThat`: validar un `T?` y **desenvolverlo** a `MlResult<T>` |
| 8 | [`8_EnsureFpAsync.md`](./8_EnsureFpAsync.md) | Fuentes `Task<T>`, predicados `Func<T, Task<bool>>`, `CancellationToken`, `TryThatAsync` |
| 9 | [`9_EnsureFpMessages.md`](./9_EnsureFpMessages.md) | `EnsureFpMessages` y las claves de detalle (`ParamName`, `Value`, `Expected`, `FailedIndexes`, `Ex`) |

---

## Las tres variantes de cada regla

Casi todas las reglas de `EnsureFp` se ofrecen en **tres formas**. Reconocer el patrón te ahorra
consultar la firma:

| Forma | Firma típica | Cuándo usarla |
|-------|--------------|---------------|
| **Mensaje** | `MaxLength(value, 10, "El nombre es demasiado largo")` | el texto va dirigido al usuario final |
| **Detalles** | `MaxLength(value, 10, MlErrorsDetails.FromErrorMessageDetails(…))` | necesitas adjuntar diagnóstico propio |
| **`…Arg`** | `MaxLengthArg(value, 10)` | validación de argumentos: el mensaje y el nombre del parámetro se generan solos |

Las variantes `…Arg` usan `[CallerArgumentExpression]`, así que capturan **la expresión escrita en
la llamada** y la publican en `Details[PARAM_NAME_KEY]`:

```csharp
var r = EnsureFp.MaxLengthArg(dto.Nombre, 10);

// Mensaje: "'dto.Nombre' debe tener como máximo 10 caracteres (actual: 27)."
// Details: { ParamName = "dto.Nombre", Value = "…", Expected = 10 }
```

Detalle completo en [1. Núcleo](./1_EnsureFpCore.md#la-convención-de-las-tres-variantes) y en
[9. Mensajes y claves de detalle](./9_EnsureFpMessages.md).

---

## Panorámica de la API por familias

Resumen de un vistazo. El inventario exhaustivo de sobrecargas está en cada página.

### Núcleo — [detalle](./1_EnsureFpCore.md)

| Método | Comprueba |
|--------|-----------|
| `That<T>(value, bool \| Func<T,bool>, error)` | cualquier condición; el error puede ser perezoso (`Func<string>`, `Func<T,string>`) |
| `TryThat<T>(value, predicate, error)` | igual, pero **captura la excepción** del predicado y la guarda en `Details["Ex"]` |
| `NotNull<T>` · `NotNullArg<T>` | `value is not null` |
| `NotEmpty<T>` · `NotEmptyArg<T>` | colección no nula y con elementos |
| `NotNullEmptyOrWhitespace` · `…Arg` | cadena no nula, no vacía y no solo espacios |
| `ThatArg<T>` | `That` con mensaje y nombre de parámetro automáticos |

### Agregación — [detalle](./2_EnsureFpAggregation.md)

| Método | Semántica |
|--------|-----------|
| `All<T>` | ejecuta **todas** las reglas y **fusiona** todos los errores |
| `AllResults<T>` | igual, partiendo de `MlResult<T>` ya calculados |
| `AllOrFirst<T>` | *fail-fast*: se detiene en la primera regla que falla |
| `Any<T>` | válido si al menos una regla pasa |
| `AllAsync` · `AllOrFirstAsync` · `AnyAsync` | equivalentes con reglas `Func<T, Task<MlResult<T>>>` |

### Cadenas — [detalle](./3_EnsureFpStrings.md)

`NotNullOrEmpty` · `MaxLength` · `MinLength` · `LengthBetween` · `LengthExactly` · `Matches` ·
`NotMatches` · `StartsWith` · `EndsWith` · `ContainsText` · `NotContainsText` · `IsOneOf`
(y su genérico `IsOneOf<T>`).

### Números y comparables — [detalle](./4_EnsureFpNumbers.md)

`GreaterThan` · `GreaterOrEqual` · `LessThan` · `LessOrEqual` · `InRange` · `OutOfRange`
(`where T : IComparable<T>`); `Positive` · `NotNegative` · `Negative` · `NotZero`
(`where T : INumber<T>`).

### Colecciones — [detalle](./5_EnsureFpCollections.md)

`NotEmptyCollection<TCollection,T>` · `CountExactly` · `CountAtLeast` · `CountAtMost` ·
`CountBetween` · `AllMatch` · `NoneMatch` · `AnyMatch` · `NoDuplicates` · `NoNullItems` ·
`ContainsItem`.

### Tipos concretos y nullables — [detalle 6](./6_EnsureFpTypes.md) · [detalle 7](./7_EnsureFpNullables.md)

`NotEmptyGuid` · `NotNullNotEmptyGuid` · `IsDefined<TEnum>` · `InFuture` · `InPast`
(`DateTime`, `DateTimeOffset`, `DateOnly`) · `NotDefault<T>` · `IsAbsoluteUri` · `IsValidUri` ·
`IsValidEmail` · `FileExists` · `DirectoryExists` · `NotNullValue<T>` · `NotNullValueThat<T>`.

### Asíncronas — [detalle](./8_EnsureFpAsync.md)

`ThatAsync` (fuente `Task<T>`, predicado `Func<T,Task<bool>>`, con `CancellationToken`) ·
`ThatArgAsync` · `TryThatAsync` · `NotNullAsync` · `NotEmptyAsync` ·
`NotNullEmptyOrWhitespaceAsync` · `NotNullValueAsync` y sus variantes `…ArgAsync`.

> 🔑 A diferencia de las primeras versiones de la librería, **sí hay sobrecargas realmente
> asíncronas**: aceptan `Task<T>` como fuente y `Func<T, Task<bool>>` como predicado, con soporte
> de `CancellationToken`. Los envoltorios históricos de `EnsureFp.cs` (simples `.ToAsync()`) se
> mantienen por compatibilidad.

---

## `EnsureFp` frente a `NullToFailed`, `EmptyToFailed` y `BoolToResult`

Las herramientas se solapan en los casos simples; la diferencia es **la sintaxis y el punto de
uso**:

| Herramienta | Forma | Formas de error | Comprobación de `null` |
|-------------|-------|-----------------|------------------------|
| `EnsureFp.NotNull` | Estático | `string`, `MlErrorsDetails`, `…Arg` | `is not null` (estricto) |
| [`NullToFailed`](../Several/2_NullToFailed.md) | Extensión | 4 formas | `== null` (respeta `operator==`) |
| `EnsureFp.NotEmpty` | Estático | `string`, `MlErrorsDetails`, `…Arg` | `!= null && Any()` |
| [`EmptyToFailed`](../Several/1_EmptyToFailed.md) | Extensión | 3 formas | `!= null && Any()` |
| `EnsureFp.That` | Estático | `string`, `MlErrorsDetails`, perezosas, `…Arg` | — |
| [`BoolToResult`](../Several/3_BoolToResult.md) | Extensión | 4 formas | — |
| [`MapEnsure`](../Map/2_MapEnsure.md) | Extensión de `MlResult<T>` | varias | Predicado **diferido** |

🔑 **Criterio práctico:**

- **Al entrar en un método público**, con argumentos sueltos → `EnsureFp`, preferentemente en su
  variante `…Arg`. El prefijo deja visualmente claro que es una guarda de precondición.
- **Cuando quieres informar de todos los errores a la vez** → `EnsureFp.All`.
- **Ya dentro del carril** → `MapEnsure`.
- **Si prefieres sintaxis fluida desde el primer momento** → los métodos de
  [`Several`](../Several/1_EmptyToFailed.md), que son extensiones.

```csharp
// Estilo A: EnsureFp para la puerta de entrada (guardas explícitas)
public MlResult<Recibo> Emitir(Pedido pedido, string serie)
    => EnsureFp.NotNullArg(pedido)
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
| Validar un argumento no nulo al entrar en un método | `NotNullArg(x)` |
| Validar una cadena obligatoria | `NotNullEmptyOrWhitespaceArg(s)` |
| Limitar la longitud de una cadena | `MaxLengthArg(s, 50)`, `LengthBetweenArg(s, 3, 50)` |
| Validar un formato con expresión regular | `MatchesArg(s, patron)` |
| Comprobar que un valor está en un conjunto | `IsOneOfArg(s, permitidos)` |
| Validar un rango numérico | `InRangeArg(edad, 18, 120)` |
| Exigir un importe positivo | `PositiveArg(importe)` |
| Validar que una colección trae elementos | `NotEmptyCollectionArg<List<Linea>, Linea>(lineas)` |
| Exigir un número de elementos | `CountBetweenArg(lineas, 1, 200)` |
| Validar todos los elementos y saber **cuáles** fallan | `AllMatchArg(lineas, l => l.Cantidad > 0)` → `Details["FailedIndexes"]` |
| Rechazar duplicados | `NoDuplicatesArg(codigos)` |
| Validar un `Guid` de ruta | `NotEmptyGuidArg(id)` |
| Validar un enumerado que llega de un JSON | `IsDefinedArg(estado)` |
| Validar y **desenvolver** un `int?` | `NotNullValueArg(dto.Edad)` → `MlResult<int>` |
| Cualquier otra regla sobre un argumento | `ThatArg(x, condición)` o `That(x, p => …, "…")` |
| Que la condición pueda lanzar excepción | `TryThat(x, p => …, "…")` |
| Condición que necesita E/S o base de datos | `ThatAsync(x, async v => await …, "…", ct)` |
| Informar de **todos** los errores de una vez | `All(dto, reglas…)` |
| Detenerse en el primer error | `AllOrFirst(dto, reglas…)` |
| Aceptar si cumple al menos una alternativa | `Any(dto, reglas…)` |
| Validar **ya dentro** del carril | [`MapEnsure`](../Map/2_MapEnsure.md) |
| Sintaxis fluida en lugar de estática | [`NullToFailed`](../Several/2_NullToFailed.md), [`BoolToResult`](../Several/3_BoolToResult.md) |
| Validar con reglas de FluentValidation o DataAnnotations | Paquetes `MoralesLarios.OOFP.Validation.*` |

---

## Ejemplos prácticos

### Ejemplo 1: guardas de un método de servicio, con acumulación de errores

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public class PedidoService
{
    public MlResult<Pedido> Crear(CrearPedidoDto dto)
        => NotNullArg(dto)
               .Bind(d => All(d,
                              x => PositiveArg(x.ClienteId).Map(_ => x),
                              x => NotNullEmptyOrWhitespaceArg(x.Referencia).Map(_ => x),
                              x => LengthBetweenArg(x.Referencia, 3, 20).Map(_ => x),
                              x => MatchesArg(x.Referencia, @"^[A-Z0-9\-]+$").Map(_ => x),
                              x => CountBetweenArg(x.Lineas, 1, 200).Map(_ => x),
                              x => AllMatchArg(x.Lineas, l => l.Cantidad > 0).Map(_ => x),
                              x => NoDuplicatesArg(x.Lineas.Select(l => l.Sku)).Map(_ => x)))
               .Map(d => new Pedido(d.ClienteId,
                                    d.Referencia.Trim().ToUpperInvariant(),
                                    d.Lineas.Select(Convertir).ToList()));
}
```

El cliente recibe **todos** los problemas en una sola respuesta, no uno por petición.

### Ejemplo 2: reglas de negocio con `That` y predicado diferido

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public MlResult<Reserva> Reservar(string sala, DateTime inicio, TimeSpan duracion, int asistentes)
    => NotNullEmptyOrWhitespaceArg(sala)
          .Bind(s => That(s, x => _salas.Existe(x), x => $"La sala '{x}' no existe"))
          .Bind(s => That(s, _ => inicio > DateTime.UtcNow, "La fecha de inicio debe ser futura"))
          .Bind(s => InRange(duracion, TimeSpan.FromMinutes(15), TimeSpan.FromHours(8),
                             "La duración debe estar entre 15 minutos y 8 horas").Map(_ => s))
          .Bind(s => InRangeArg(asistentes, 1, 50).Map(_ => s))
          .Map(s => new Reserva(s, inicio, duracion, asistentes));
```

El predicado de `That` **es diferido**: solo se evalúa si el valor llega al carril válido.

### Ejemplo 3: condición asíncrona, sin romper la cadena

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public Task<MlResult<Cliente>> AltaAsync(AltaDto dto, CancellationToken ct)
    => NotNullArg(dto)
          .Bind(d => IsValidEmailArg(d.Email).Map(_ => d))
          .Bind(d => MatchesArg(d.Nif, RegexNif).Map(_ => d))
          .BindAsync(d => ThatAsync(d,
                                    async (x, token) => !await _repo.ExisteNifAsync(x.Nif, token),
                                    $"Ya existe un cliente con el NIF {d.Nif}",
                                    ct))
          .MapAsync(d => new Cliente(d.Nif, d.Nombre).ToAsync());
```

Ya **no hace falta** resolver la consulta fuera de la cadena: hay sobrecargas con predicado
asíncrono y `CancellationToken`. Ver [8. Variantes asíncronas](./8_EnsureFpAsync.md).

### Ejemplo 4: predicado que puede lanzar excepción

```csharp
// El parseo puede fallar con una entrada corrupta: TryThat lo convierte en fallo, no en crash
var r = EnsureFp.TryThat(cadenaJson,
                         s => JsonSerializer.Deserialize<Config>(s)!.Version >= 3,
                         ex => $"No se pudo interpretar la configuración: {ex.Message}");

if (r.IsFail)
    logger.LogError(r.SecureFailErrorsDetails().GetDetailException(), "Configuración inválida");
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ Llamarlo como método de extensión: no compila
// var r = cliente.NotNull("El cliente es obligatorio");

// ✅ Prefijo de clase, using static, o usa NullToFailed
var r = EnsureFp.NotNull(cliente, "El cliente es obligatorio");


// ❌ Pasar un MlError: no hay sobrecarga
// EnsureFp.NotNull(cliente, ErroresCliente.Obligatorio);

// ✅ Conviértelo
EnsureFp.NotNull(cliente, MlErrorsDetails.FromError(ErroresCliente.Obligatorio));


// ❌ Encadenar Bind cuando quieres informar de todos los errores: solo verás el primero
var r1 = NotNullEmptyOrWhitespaceArg(dto.Nombre)
             .Bind(_ => IsValidEmailArg(dto.Email))
             .Bind(_ => PositiveArg(dto.Edad));

// ✅ All los acumula
var r2 = All(dto,
             x => NotNullEmptyOrWhitespaceArg(x.Nombre).Map(_ => x),
             x => IsValidEmailArg(x.Email).Map(_ => x),
             x => PositiveArg(x.Edad).Map(_ => x));


// ❌ NotEmpty pierde el tipo concreto de la colección
MlResult<IEnumerable<Linea>> a = EnsureFp.NotEmpty(lineas, "Sin líneas");

// ✅ NotEmptyCollection lo conserva
MlResult<List<Linea>> b = EnsureFp.NotEmptyCollection<List<Linea>, Linea>(lineas, "Sin líneas");


// ❌ Suponer que NotNullEmptyOrWhitespace recorta la cadena
var c = EnsureFp.NotNullEmptyOrWhitespace(nif, "…");   // "  X  " pasa tal cual

// ✅ Normaliza después
var d = EnsureFp.NotNullEmptyOrWhitespace(nif, "…").Map(s => s.Trim().ToUpperInvariant());


// ❌ Escribir a mano un mensaje que ya existe
EnsureFp.That(dto.Edad, dto.Edad is >= 18 and <= 120, "La edad debe estar entre 18 y 120");

// ✅ La regla especializada aporta el mensaje, el nombre y el detalle Expected
EnsureFp.InRangeArg(dto.Edad, 18, 120);
```

---

## Mejores prácticas

1. **Usa `EnsureFp` en la primera línea de los métodos públicos**: es la puerta de entrada natural
   al carril y el prefijo hace evidente que se trata de precondiciones.
2. **Prefiere las variantes `…Arg` para validar argumentos**: mensaje, nombre del parámetro y
   detalles se generan solos, y son consistentes en toda la solución.
3. **Prefiere la regla especializada al `That` genérico**: `InRangeArg` aporta mensaje con el valor
   real y el detalle `Expected`; `That` no.
4. **Usa `All` cuando el destinatario es un formulario o una API pública** y `AllOrFirst` cuando
   validar cuesta caro o las reglas dependen unas de otras.
5. **Usa `TryThat` siempre que el predicado invoque código que pueda lanzar** (parseo,
   deserialización, expresiones regulares complejas).
6. **Dentro del carril, cambia a `MapEnsure`**: evita el `Bind` ceremonial.
7. **Elige un estilo y sé consistente**: o `EnsureFp.*` (estático, con `using static`) o los
   métodos de `Several` (fluidos).
8. **No analices el texto del mensaje**: decide con las claves de `Details`
   (`ParamName`, `Expected`, `FailedIndexes`, `Ex`). Ver
   [9. Mensajes y claves](./9_EnsureFpMessages.md).
9. **Prefiere `NotEmptyCollection` a `NotEmpty`** para no perder el tipo concreto de la colección.
10. **Materializa las consultas diferidas** antes de validarlas si vas a recorrerlas después.
11. **Normaliza las cadenas después de validarlas** (`Trim`, `ToUpperInvariant`): las guardas no lo
    hacen.
12. **Propaga el `CancellationToken`** en las validaciones asíncronas que consulten recursos
    externos.
13. **Para validaciones declarativas complejas** (atributos, reglas encadenadas), usa los paquetes
    `MoralesLarios.OOFP.Validation.Dataannotations` o
    `MoralesLarios.OOFP.Validation.FluentValidations`.

---

## Resumen

- `EnsureFp` es una **clase estática parcial** (no extensiones) repartida en ocho ficheros de
  reglas más `EnsureFpMessages`, que convierte argumentos de C# en `MlResult<T>` **sin lanzar
  excepciones**.
- Cubre **ocho familias**: núcleo, agregación, cadenas, números, colecciones, tipos concretos,
  nullables y asíncronas.
- Cada regla se ofrece en **tres variantes**: con `string`, con `MlErrorsDetails` y `…Arg` con
  mensaje y nombre de parámetro automáticos vía `[CallerArgumentExpression]`.
- `That` acepta **condición ya evaluada o predicado diferido**, y mensajes perezosos
  (`Func<string>`, `Func<T,string>`).
- `TryThat` **captura la excepción** del predicado y la publica en `Details["Ex"]`.
- `All` **acumula todos los errores**; `AllOrFirst` es *fail-fast*; `Any` acepta alternativas.
- Las variantes asíncronas admiten **fuentes `Task<T>`, predicados asíncronos y
  `CancellationToken`**.
- Todos los fallos llevan `Details["ParamName"]` y `Details["Value"]`; muchos añaden
  `Expected` o `FailedIndexes`.
- Dentro del carril, prefiere [`MapEnsure`](../Map/2_MapEnsure.md).

---

## Ver también

**Páginas de la familia**

- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [3. Cadenas de texto](./3_EnsureFpStrings.md)
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [5. Colecciones](./5_EnsureFpCollections.md)
- [6. Tipos concretos](./6_EnsureFpTypes.md)
- [7. Tipos `Nullable<T>`](./7_EnsureFpNullables.md)
- [8. Variantes asíncronas](./8_EnsureFpAsync.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)

**Documentación relacionada**

- [Introducción general a la librería](../1_Intro.md)
- [`MapEnsure` — validar dentro del carril](../Map/2_MapEnsure.md)
- [`EmptyToFailed`](../Several/1_EmptyToFailed.md) · [`NullToFailed`](../Several/2_NullToFailed.md) · [`BoolToResult`](../Several/3_BoolToResult.md) · [`Combine`](../Several/4_Combine.md)
- [`Bind` — encadenar operaciones que pueden fallar](../Bind/3_Bind.md)
- [`MlResultErrors` — anatomía de `MlErrorsDetails`](../Types/MlResultErrors.md)