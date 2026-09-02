# `EnsureFp` — Colecciones

> Archivo fuente: `Helpers/EnsureFp.Collections.cs`.

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [1. `NotEmptyCollection`: conservar el tipo concreto](#1-notemptycollection-conservar-el-tipo-concreto)
- [2. Cardinalidad: `CountExactly`, `CountAtLeast`, `CountAtMost`, `CountBetween`](#2-cardinalidad-countexactly-countatleast-countatmost-countbetween)
- [3. Predicados sobre los elementos: `AllMatch`, `NoneMatch`, `AnyMatch`](#3-predicados-sobre-los-elementos-allmatch-nonematch-anymatch)
- [4. `NoDuplicates` y `NoNullItems`](#4-noduplicates-y-nonullitems)
- [5. `ContainsItem`](#5-containsitem)
- [6. Enumeración única: el helper `Materialize`](#6-enumeración-única-el-helper-materialize)
- [7. Semántica de `null` y de los comparadores](#7-semántica-de-null-y-de-los-comparadores)
- [8. Ejemplos completos](#8-ejemplos-completos)
- [9. Mejores prácticas](#9-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

`NotEmpty` (el método histórico de `EnsureFp`) solo responde a una pregunta: «¿hay algo?». En la
práctica las colecciones necesitan mucho más: cardinalidad exacta o mínima, ausencia de duplicados,
ausencia de nulos, todos los elementos válidos, ninguno prohibido, presencia de un valor concreto.

Este bloque cubre esas comprobaciones con dos garantías importantes:

- **La colección se enumera exactamente una vez**, aunque sea un `IEnumerable<T>` perezoso o el
  resultado de una consulta LINQ diferida.
- **Los fallos informan de las posiciones concretas** que incumplen la regla, mediante la clave de
  detalle `FailedIndexes`.

---

## 1. `NotEmptyCollection`: conservar el tipo concreto

```csharp
public static MlResult<TCollection> NotEmptyCollection<TCollection, T>(TCollection value, string errorMessage)
    where TCollection : IEnumerable<T>;

public static MlResult<TCollection> NotEmptyCollection<TCollection, T>(TCollection value, MlErrorsDetails errorsDetails)
    where TCollection : IEnumerable<T>;

public static MlResult<TCollection> NotEmptyCollectionArg<TCollection, T>(TCollection value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null)
    where TCollection : IEnumerable<T>;
```

El `NotEmpty<T>` clásico devuelve `MlResult<IEnumerable<T>>`, lo que obliga a un `.ToList()` o un
*cast* aguas abajo si trabajabas con una `List<T>` o un array:

```csharp
// ❌ Antes: se pierde el tipo concreto
MlResult<IEnumerable<Pedido>> r = NotEmpty(pedidos, "Sin pedidos.");
// r.Map(p => p.Count) → no compila: IEnumerable<T> no tiene Count

// ✅ Ahora: el tipo se conserva
MlResult<List<Pedido>> r2 = NotEmptyCollection<List<Pedido>, Pedido>(pedidos, "Sin pedidos.");
var total = r2.Map(p => p.Count);   // ✅ compila
```

El precio es indicar los dos parámetros genéricos (el compilador no puede inferir `T` a partir de
`TCollection`). Es un patrón habitual en librerías: se paga verbosidad a cambio de no perder el tipo.

```csharp
var r1 = NotEmptyCollection<List<int>, int>(ids, "La lista de identificadores está vacía.");
var r2 = NotEmptyCollection<int[], int>(codigos, "No se han recibido códigos.");
var r3 = NotEmptyCollectionArg<Dictionary<string, string>, KeyValuePair<string, string>>(cabeceras);
```

> Si no necesitas el tipo concreto, `NotEmpty` sigue siendo perfectamente válido y más breve.

---

## 2. Cardinalidad: `CountExactly`, `CountAtLeast`, `CountAtMost`, `CountBetween`

```csharp
public static MlResult<IEnumerable<T>> CountExactly<T>(IEnumerable<T> value, int expectedCount, string errorMessage);
public static MlResult<IEnumerable<T>> CountAtLeast<T>(IEnumerable<T> value, int minCount, string errorMessage);
public static MlResult<IEnumerable<T>> CountAtMost<T>(IEnumerable<T> value, int maxCount, string errorMessage);
public static MlResult<IEnumerable<T>> CountBetween<T>(IEnumerable<T> value, int minCount, int maxCount, string errorMessage);

// Cada una con su sobrecarga MlErrorsDetails y su variante *Arg.
```

| Regla | Condición | Uso típico |
|---|---|---|
| `CountExactly` | `count == expectedCount` | pares de coordenadas, tuplas fijas, importaciones con formato rígido |
| `CountAtLeast` | `count >= minCount` | «al menos un destinatario», «mínimo dos firmantes» |
| `CountAtMost` | `count <= maxCount` | límites de lote, tamaño máximo de una petición masiva |
| `CountBetween` | `min <= count <= max` | rangos de adjuntos, participantes de una reunión |

```csharp
var r1 = CountAtLeast(destinatarios, 1, "Debe indicarse al menos un destinatario.");
var r2 = CountAtMost(adjuntos, 10, "No se admiten más de 10 adjuntos.");
var r3 = CountBetween(firmantes, 2, 5, "Se requieren entre 2 y 5 firmantes.");
var r4 = CountExactly(coordenadas, 2, "Se esperaban exactamente dos coordenadas.");

// Mensaje automático con el recuento real:
var r5 = CountAtLeastArg(destinatarios, 1);   // "'destinatarios' debe contener al menos 1 elemento (actual: 0)."
```

Todas devuelven `MlResult<IEnumerable<T>>` con la **colección ya materializada**, de modo que
enumerar el resultado no vuelve a ejecutar la consulta original.

> `CountBetween` es inclusivo en ambos extremos, igual que
> [`InRange`](./4_EnsureFpNumbers.md#2-rangos-inrange-y-outofrange).

---

## 3. Predicados sobre los elementos: `AllMatch`, `NoneMatch`, `AnyMatch`

```csharp
public static MlResult<IEnumerable<T>> AllMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage);
public static MlResult<IEnumerable<T>> NoneMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage);
public static MlResult<IEnumerable<T>> AnyMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage);

// Cada una con su sobrecarga MlErrorsDetails y su variante *Arg.
```

| Regla | Válido cuando | Detalle en caso de fallo |
|---|---|---|
| `AllMatch` | **todos** los elementos cumplen el predicado | índices de los que **no** cumplen |
| `NoneMatch` | **ningún** elemento cumple el predicado | índices de los que **sí** cumplen |
| `AnyMatch` | **al menos uno** cumple el predicado | (no aplica: ninguno cumple) |

La diferencia respecto a escribir `lista.All(...)` a mano es el diagnóstico: el fallo incluye la
clave `FailedIndexes` con las **posiciones exactas** que rompen la regla.

```csharp
var resultado = AllMatch(lineas, l => l.Cantidad > 0,
                         "Todas las líneas deben tener cantidad positiva.");

if (resultado.IsFail)
{
    var detalles = resultado.SecureFailErrorsDetails();
    var indices  = detalles.GetDetailValue<IEnumerable<int>>();   // p. ej. [2, 7]
    // → "Las líneas 3 y 8 tienen cantidad cero o negativa."
}
```

Más ejemplos:

```csharp
var r1 = NoneMatch(usuarios, u => u.Bloqueado,
                   "Ningún participante puede estar bloqueado.");

var r2 = AnyMatch(roles, r => r == "Administrador",
                  "Se requiere al menos un administrador.");

var r3 = AllMatchArg(precios, p => p >= 0);   // mensaje y ParamName automáticos
```

> ⚠️ Un predicado `null` hace que la regla **falle** (semántica coherente con
> [`EvaluatePredicate`](./1_EnsureFpCore.md#semántica-defensiva)): nunca se lanza
> `NullReferenceException`.

---

## 4. `NoDuplicates` y `NoNullItems`

```csharp
public static MlResult<IEnumerable<T>> NoDuplicates<T>(IEnumerable<T> value, string errorMessage,
                                                       IEqualityComparer<T>? comparer = null);
public static MlResult<IEnumerable<T>> NoDuplicates<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails,
                                                       IEqualityComparer<T>? comparer = null);
public static MlResult<IEnumerable<T>> NoDuplicatesArg<T>(IEnumerable<T> value,
                                                          IEqualityComparer<T>? comparer = null,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<IEnumerable<T>> NoNullItems<T>(IEnumerable<T> value, string errorMessage);
public static MlResult<IEnumerable<T>> NoNullItems<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails);
public static MlResult<IEnumerable<T>> NoNullItemsArg<T>(IEnumerable<T> value,
                                                         [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

`NoDuplicates` acepta un comparador opcional, lo que permite comparaciones insensibles a mayúsculas
o basadas en una clave de negocio:

```csharp
var r1 = NoDuplicates(codigos, "Hay códigos repetidos.");

var r2 = NoDuplicates(nombres, "Hay nombres repetidos.",
                      StringComparer.OrdinalIgnoreCase);

var r3 = NoDuplicatesArg(referencias, StringComparer.Ordinal);
```

`NoNullItems` protege las colecciones de referencias antes de proyectarlas:

```csharp
public MlResult<IEnumerable<string>> Normalizar(IEnumerable<string> entradas) =>
    NoNullItemsArg(entradas)
        .Map(e => e.Select(x => x.Trim().ToUpperInvariant()));   // seguro: no hay nulos
```

Ambas informan de los índices implicados mediante `FailedIndexes` cuando fallan.

---

## 5. `ContainsItem`

```csharp
public static MlResult<IEnumerable<T>> ContainsItem<T>(IEnumerable<T> value, T item, string errorMessage,
                                                       IEqualityComparer<T>? comparer = null);
public static MlResult<IEnumerable<T>> ContainsItem<T>(IEnumerable<T> value, T item, MlErrorsDetails errorsDetails,
                                                       IEqualityComparer<T>? comparer = null);
public static MlResult<IEnumerable<T>> ContainsItemArg<T>(IEnumerable<T> value, T item,
                                                          IEqualityComparer<T>? comparer = null,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Comprueba que un elemento **obligatorio** esté presente:

```csharp
var r1 = ContainsItem(roles, "Lector", "El conjunto de roles debe incluir 'Lector'.");
var r2 = ContainsItem(cabeceras, "Authorization", "Falta la cabecera de autorización.",
                      StringComparer.OrdinalIgnoreCase);
```

> Para el caso inverso —«el valor debe pertenecer a un conjunto permitido»— la regla correcta es
> [`IsOneOf`](./3_EnsureFpStrings.md#7-conjuntos-permitidos-isoneof): allí lo que se valida es el
> **valor**; aquí lo que se valida es la **colección**.

---

## 6. Enumeración única: el helper `Materialize`

Todas las reglas de este bloque pasan por el helper privado `Materialize<T>`, que:

1. si la entrada ya es `ICollection<T>` (`List<T>`, array, `HashSet<T>`…), la usa tal cual;
2. si es un `IEnumerable<T>` perezoso, lo recorre **una sola vez** y guarda el resultado;
3. devuelve siempre una colección estable que se propaga en el `MlResult` de salida.

Esto importa porque una consulta LINQ diferida puede golpear la base de datos en cada enumeración:

```csharp
var consulta = contexto.Pedidos.Where(p => p.Pendiente);   // IQueryable, aún sin ejecutar

// ✅ Una sola ejecución: la regla materializa y el resultado ya viene materializado
var r = CountAtLeast(consulta, 1, "No hay pedidos pendientes.")
            .Bind(p => AllMatch(p, x => x.Importe > 0, "Hay importes no positivos."));
```

Otros helpers privados: `FailedIndexes<T>` (posiciones que incumplen), `HasNoDuplicates<T>` y
`CountRule<T>` (dos sobrecargas que centralizan la construcción del fallo de cardinalidad).

---

## 7. Semántica de `null` y de los comparadores

| Situación | Comportamiento |
|---|---|
| Colección `null` | **falla** en todas las reglas (nunca `NullReferenceException`) |
| Colección vacía | `CountAtLeast(…, 1)` falla; `AllMatch` y `NoneMatch` **pasan** (semántica lógica clásica de vacuidad); `AnyMatch` falla |
| Predicado `null` | la regla **falla** |
| `comparer` `null` | se usa `EqualityComparer<T>.Default` |
| Elementos `null` dentro de la colección | solo los detecta `NoNullItems`; el resto de reglas los tratan como un valor más |

El punto que más sorprende es el de la colección vacía en `AllMatch`: «todos los elementos de un
conjunto vacío cumplen la condición» es verdad por vacuidad. Si necesitas ambas cosas, encadena:

```csharp
var r = CountAtLeast(lineas, 1, "El pedido debe tener al menos una línea.")
            .Bind(l => AllMatch(l, x => x.Cantidad > 0, "Todas las líneas requieren cantidad positiva."));
```

---

## 8. Ejemplos completos

### 8.1. Validación completa de un pedido

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public MlResult<Pedido> Validar(Pedido pedido) =>
    All(pedido,
        p => CountBetween(p.Lineas, 1, 100, "El pedido debe tener entre 1 y 100 líneas.").Map(_ => p),
        p => AllMatch(p.Lineas, l => l.Cantidad > 0, "Todas las líneas requieren cantidad positiva.").Map(_ => p),
        p => NoDuplicates(p.Lineas.Select(l => l.ProductoId), "Hay productos repetidos en el pedido.").Map(_ => p),
        p => NoNullItems(p.Etiquetas, "Las etiquetas no pueden contener nulos.").Map(_ => p));
```

### 8.2. Importación de un fichero CSV

```csharp
public MlResult<IEnumerable<string[]>> ValidarFilas(IEnumerable<string[]> filas) =>
    CountAtLeastArg(filas, 2)                                              // cabecera + al menos un dato
        .Bind(f => AllMatch(f, c => c.Length == 5,
                            "Todas las filas deben tener exactamente 5 columnas."))
        .Bind(f => NoneMatch(f, c => c.All(string.IsNullOrWhiteSpace),
                             "El fichero contiene filas completamente vacías."));
```

Y el diagnóstico de las filas erróneas:

```csharp
var resultado = ValidarFilas(filas);

resultado.Match(
    valid: f  => Ok($"{f.Count()} filas válidas."),
    fail:  e  =>
    {
        var indices = e.GetDetailValue<IEnumerable<int>>();
        var lineas  = indices is null ? "" : string.Join(", ", indices.Select(i => i + 1));
        return BadRequest($"{e.ToErrorsMessages()} Líneas afectadas: {lineas}");
    });
```

### 8.3. Permisos de un usuario

```csharp
public MlResult<IEnumerable<string>> ValidarPermisos(IEnumerable<string> permisos) =>
    NotEmptyCollectionArg<string[], string>(permisos?.ToArray()!)
        .Bind(p => NoDuplicates(p, "Hay permisos repetidos.", StringComparer.OrdinalIgnoreCase))
        .Bind(p => ContainsItem(p, "Lectura", "Todo usuario debe tener al menos permiso de lectura.",
                                StringComparer.OrdinalIgnoreCase));
```

---

## 9. Mejores prácticas

1. **Usa `NotEmptyCollection<TCollection, T>` cuando necesites el tipo concreto** (`.Count`,
   indexación, `Array`); usa `NotEmpty` si te basta `IEnumerable<T>`.
2. **Trabaja con el resultado, no con la colección original.** Las reglas devuelven la colección ya
   materializada: encadena sobre ella para no volver a enumerar la fuente.
3. **Combina cardinalidad y predicado.** `AllMatch` pasa con colecciones vacías; añade
   `CountAtLeast` si eso no es aceptable.
4. **Aprovecha `FailedIndexes`.** Es lo que convierte «hay líneas inválidas» en «las líneas 3 y 8
   son inválidas».
5. **Pasa comparadores explícitos** en `NoDuplicates`, `ContainsItem` e `IsOneOf` cuando compares
   texto: `StringComparer.OrdinalIgnoreCase` evita falsos negativos.
6. **Valida `NoNullItems` antes de proyectar** una colección de referencias.
7. **Agrupa las reglas con [`All`](./2_EnsureFpAggregation.md)** para devolver todos los problemas
   de la colección en una sola respuesta.
8. **No uses estas reglas dentro de un bucle sobre la misma colección.** Están pensadas para
   evaluarse una vez sobre el conjunto completo.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [3. Cadenas de texto](./3_EnsureFpStrings.md) — `IsOneOf`, longitudes
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
- [Bucles y colecciones con `MlResult`](../Bucle/Bucles.md)
