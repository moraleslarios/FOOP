# `EnsureFp` — Números, comparables y rangos

> Archivo fuente: `Helpers/EnsureFp.Numbers.cs`.

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [Dos familias, dos restricciones genéricas](#dos-familias-dos-restricciones-genéricas)
- [1. Comparaciones: `GreaterThan`, `LessThan` y variantes](#1-comparaciones-greaterthan-lessthan-y-variantes)
- [2. Rangos: `InRange` y `OutOfRange`](#2-rangos-inrange-y-outofrange)
- [3. Signo y cero: `Positive`, `Negative`, `NotNegative`, `NotZero`](#3-signo-y-cero-positive-negative-notnegative-notzero)
- [4. La clave `Expected` en los detalles del error](#4-la-clave-expected-en-los-detalles-del-error)
- [5. Semántica de `null` y de la comparación](#5-semántica-de-null-y-de-la-comparación)
- [6. Ejemplos completos](#6-ejemplos-completos)
- [7. Mejores prácticas](#7-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

Las comprobaciones numéricas son el segundo grupo más frecuente después de las cadenas: importes
positivos, cantidades mínimas, porcentajes entre 0 y 100, identificadores mayores que cero,
descuentos que no pueden ser negativos…

Este bloque las expresa una sola vez y de forma genérica, sin duplicar código por tipo (`int`,
`long`, `decimal`, `double`, `short`, `byte`, `BigInteger`…) y sin conversiones ni *boxing*.

Como el resto de `EnsureFp`, cada regla tiene sus tres variantes: mensaje `string`,
`MlErrorsDetails` y `*Arg` con mensaje automático
(ver [la convención de las tres variantes](./1_EnsureFpCore.md#la-convención-de-las-tres-variantes)).

---

## Dos familias, dos restricciones genéricas

| Restricción | Reglas | Tipos que abarca |
|---|---|---|
| `where T : IComparable<T>` | `GreaterThan`, `GreaterOrEqual`, `LessThan`, `LessOrEqual`, `InRange`, `OutOfRange` | todos los numéricos **y** `DateTime`, `DateOnly`, `TimeSpan`, `string`, `Version`, tipos propios comparables |
| `where T : INumber<T>` | `Positive`, `NotNegative`, `Negative`, `NotZero` | solo numéricos (interfaz genérica matemática de .NET 7+) |

La distinción no es un detalle: **`InRange` funciona con fechas**, porque una fecha es comparable,
mientras que `Positive` no tiene sentido para una fecha y el compilador lo impide.

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

var r1 = InRange(edad, 18, 120, "Edad fuera de rango.");                      // int
var r2 = InRange(importe, 0.01m, 10_000m, "Importe fuera de rango.");          // decimal
var r3 = InRange(fecha, inicio, fin, "La fecha está fuera del periodo.");       // DateTime ✅
var r4 = Positive(importe, "El importe debe ser positivo.");                    // decimal
// var r5 = Positive(fecha, "...");   // ❌ no compila: DateTime no es INumber<T>
```

---

## 1. Comparaciones: `GreaterThan`, `LessThan` y variantes

```csharp
public static MlResult<T> GreaterThan<T>(T value, T limit, string errorMessage)          where T : IComparable<T>;
public static MlResult<T> GreaterThan<T>(T value, T limit, MlErrorsDetails errorsDetails) where T : IComparable<T>;
public static MlResult<T> GreaterThanArg<T>(T value, T limit,
                                            [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>;

// GreaterOrEqual / GreaterOrEqualArg   →  value >= limit
// LessThan       / LessThanArg         →  value <  limit
// LessOrEqual    / LessOrEqualArg      →  value <= limit
```

| Regla | Condición | Uso típico |
|---|---|---|
| `GreaterThan` | `value > limit` | identificadores (`> 0`), contadores estrictos |
| `GreaterOrEqual` | `value >= limit` | cantidades mínimas, edad mínima |
| `LessThan` | `value < limit` | límites exclusivos (`< 100`) |
| `LessOrEqual` | `value <= limit` | topes, cupos, capacidad máxima |

```csharp
var r1 = GreaterThan(pedidoId, 0, "El identificador de pedido debe ser positivo.");
var r2 = GreaterOrEqual(cantidad, 1, "Debe pedirse al menos una unidad.");
var r3 = LessOrEqual(descuento, 0.5m, "El descuento no puede superar el 50 %.");

// Con mensaje automático (incluye valor real y esperado):
var r4 = GreaterThanArg(pedidoId, 0);   // "'pedidoId' debe ser mayor que 0 (actual: -3)."
```

**Elige `GreaterOrEqual` frente a `GreaterThan` con cuidado.** Es el origen más habitual de los
errores «por uno» (*off-by-one*): «al menos 1 unidad» es `GreaterOrEqual(cantidad, 1)`, no
`GreaterThan(cantidad, 1)`.

---

## 2. Rangos: `InRange` y `OutOfRange`

```csharp
public static MlResult<T> InRange<T>(T value, T min, T max, string errorMessage)           where T : IComparable<T>;
public static MlResult<T> InRange<T>(T value, T min, T max, MlErrorsDetails errorsDetails) where T : IComparable<T>;
public static MlResult<T> InRangeArg<T>(T value, T min, T max,
                                        [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>;

// OutOfRange / OutOfRangeArg  →  la negación exacta de InRange
```

**`InRange` es inclusivo en ambos extremos**: la condición es `min <= value && value <= max`. Es la
semántica que espera el 90 % de las validaciones de negocio («entre 18 y 120 años» incluye 18 y 120).

```csharp
var r1 = InRange(porcentaje, 0, 100, "El porcentaje debe estar entre 0 y 100.");
var r2 = InRange(mes, 1, 12, "El mes debe estar entre 1 y 12.");
var r3 = InRange(fechaEfecto, DateTime.Today, DateTime.Today.AddYears(1),
                 "La fecha de efecto debe caer dentro del próximo año.");
```

`OutOfRange` sirve para **excluir** una franja concreta:

```csharp
// Los códigos internos reservados van del 900 al 999.
var r = OutOfRange(codigo, 900, 999, "Ese rango de códigos está reservado para uso interno.");
```

> ⚠️ Si `min > max` el rango es vacío y **cualquier** valor falla en `InRange`. La librería no
> corrige el orden de los extremos: un rango invertido es un error del llamante y conviene que se
> manifieste, no que se silencie.

Para rangos **exclusivos** combina las comparaciones simples:

```csharp
// 0 < valor < 1  (exclusivo en ambos extremos)
var r = GreaterThan(valor, 0m, "Debe ser mayor que 0.")
            .Bind(v => LessThan(v, 1m, "Debe ser menor que 1."));
```

---

## 3. Signo y cero: `Positive`, `Negative`, `NotNegative`, `NotZero`

```csharp
public static MlResult<T> Positive<T>(T value, string errorMessage)    where T : INumber<T>;   // value > 0
public static MlResult<T> NotNegative<T>(T value, string errorMessage) where T : INumber<T>;   // value >= 0
public static MlResult<T> Negative<T>(T value, string errorMessage)    where T : INumber<T>;   // value < 0
public static MlResult<T> NotZero<T>(T value, string errorMessage)     where T : INumber<T>;   // value != 0

// Cada una con su sobrecarga MlErrorsDetails y su variante *Arg.
```

| Regla | Condición | ¿Acepta 0? | Uso típico |
|---|---|---|---|
| `Positive` | `> 0` | ❌ | importes de cobro, cantidades, identificadores |
| `NotNegative` | `>= 0` | ✅ | saldos, stock, descuentos, contadores |
| `Negative` | `< 0` | ❌ | abonos, ajustes contables negativos |
| `NotZero` | `!= 0` | ❌ | divisores, factores de conversión |

```csharp
var r1 = Positive(importe, "El importe a cobrar debe ser mayor que cero.");
var r2 = NotNegative(stock, "El stock no puede ser negativo.");
var r3 = NotZero(divisor, "El divisor no puede ser cero.");

// Mensaje automático:
var r4 = PositiveArg(importe);   // "'importe' debe ser positivo (actual: -12,50)."
```

**`NotZero` antes de dividir** es el patrón que evita la excepción y mantiene la cadena:

```csharp
public MlResult<decimal> CalcularMedia(decimal total, int elementos) =>
    NotZeroArg(elementos)
        .Map(n => total / n);
```

---

## 4. La clave `Expected` en los detalles del error

Las variantes `*Arg` de este bloque añaden al diccionario `Details` la constante
`EXPECTED_KEY` (`"Expected"`) con el límite o el rango esperado, además de `ParamName` y `Value`
que añaden todas las guardas `*Arg`.

```csharp
var resultado = InRangeArg(edad, 18, 120);

if (resultado.IsFail)
{
    var detalles = resultado.SecureFailErrorsDetails();

    detalles.ToErrorsMessages();        // "'edad' debe estar entre 18 y 120 (actual: 15)."
    detalles.ToDetailsDescription();    // ParamName = edad, Value = 15, Expected = 18..120
}
```

Esto permite construir respuestas HTTP estructuradas sin analizar el texto del mensaje:

```csharp
fail: e => BadRequest(new ProblemDetails
{
    Title  = "Parámetro fuera de rango",
    Detail = e.ToErrorsMessages(),
    Extensions = { ["detalles"] = e.Details }   // incluye ParamName, Value y Expected
});
```

Ver el catálogo completo en [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md).

---

## 5. Semántica de `null` y de la comparación

Aunque `T` puede ser un tipo referencia comparable (`string`, `Version`), el helper privado
`Compare<T>` protege la comparación:

| Situación | Comportamiento |
|---|---|
| `value` es `null` | `Compare` devuelve `-1`: el valor se considera **menor que todo** |
| `limit`, `min` o `max` es `null` | se compara con la semántica de `IComparable<T>` del tipo |
| Tipos valor (`int`, `decimal`…) | no puede haber `null`; comparación directa |
| `Nullable<T>` (`int?`, `decimal?`) | **no** encaja en estas restricciones: desenvuélvelo primero con [`NotNullValue`](./7_EnsureFpNullables.md) |

Consecuencia práctica: con `value == null`, `GreaterThan` y `GreaterOrEqual` **fallan**, `LessThan`
y `LessOrEqual` **pasan**, e `InRange` **falla**. Si trabajas con tipos referencia comparables,
valida la nulidad antes:

```csharp
var r = NotNullArg(version)
            .Bind(v => GreaterOrEqual(v, new Version(2, 0), "Se requiere la versión 2.0 o superior."));
```

Para `Nullable<T>`, el camino correcto es desenvolver primero:

```csharp
public MlResult<decimal> Validar(decimal? importe) =>
    NotNullValueArg(importe)                    // MlResult<decimal> (ya desenvuelto)
        .Bind(i => Positive(i, "El importe debe ser positivo."));
```

Helpers privados del bloque: `Compare<T>` (comparación defensiva) e `IsInRange<T>`
(`Compare(value, min) >= 0 && Compare(value, max) <= 0`).

---

## 6. Ejemplos completos

### 6.1. Línea de pedido

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public record LineaPedido(int ProductoId, int Cantidad, decimal PrecioUnitario, decimal Descuento);

public MlResult<LineaPedido> Validar(LineaPedido l) =>
    All(l,
        x => GreaterThan(x.ProductoId, 0, "El producto no es válido.").Map(_ => x),
        x => InRange(x.Cantidad, 1, 999, "La cantidad debe estar entre 1 y 999.").Map(_ => x),
        x => Positive(x.PrecioUnitario, "El precio unitario debe ser mayor que cero.").Map(_ => x),
        x => InRange(x.Descuento, 0m, 1m, "El descuento debe expresarse entre 0 y 1.").Map(_ => x));
```

### 6.2. Paginación de una consulta

```csharp
public MlResult<(int Pagina, int Tamano)> ValidarPaginacion(int pagina, int tamano) =>
    GreaterOrEqualArg(pagina, 1)                          // "'pagina' debe ser mayor o igual que 1 (actual: 0)."
        .Bind(_ => InRangeArg(tamano, 1, 200))            // "'tamano' debe estar entre 1 y 200 (actual: 5000)."
        .Map(_ => (pagina, tamano));
```

### 6.3. Rango de fechas de un informe

```csharp
public MlResult<(DateTime Desde, DateTime Hasta)> ValidarPeriodo(DateTime desde, DateTime hasta) =>
    LessOrEqual(desde, hasta, "La fecha inicial no puede ser posterior a la final.")
        .Bind(_ => InRange(desde, new DateTime(2020, 1, 1), DateTime.Today,
                           "El periodo debe empezar entre 2020 y hoy."))
        .Bind(_ => LessOrEqual((hasta - desde).Days, 366,
                               "El periodo no puede superar un año."))
        .Map(_ => (desde, hasta));
```

### 6.4. Cálculo protegido

```csharp
public MlResult<decimal> PrecioMedio(decimal importeTotal, int unidades) =>
    NotNegativeArg(importeTotal)
        .Bind(_ => PositiveArg(unidades))
        .Map(u => importeTotal / u);
```

---

## 7. Mejores prácticas

1. **Repasa los extremos.** `InRange` es **inclusivo**; para rangos exclusivos combina
   `GreaterThan` + `LessThan`.
2. **No inviertas `min` y `max`.** Un rango invertido rechaza todo y es difícil de diagnosticar.
3. **`Positive` para importes de cobro, `NotNegative` para saldos y stock.** Es la distinción que
   más errores de negocio evita.
4. **`NotZero` antes de cualquier división**, incluidas las medias y los porcentajes.
5. **Desenvuelve los `Nullable<T>` antes de comparar** con
   [`NotNullValue`](./7_EnsureFpNullables.md); no intentes forzar la restricción genérica.
6. **Usa `*Arg` en la capa de entrada** (controladores, servicios de aplicación) para obtener
   automáticamente `ParamName`, `Value` y `Expected` en los detalles.
7. **Mantén los límites en constantes con nombre** (`MaxUnidadesPorLinea`, `TamanoPaginaMaximo`) en
   lugar de literales dispersos por el código.
8. **Compón con [`All`](./2_EnsureFpAggregation.md)** cuando valides varios campos numéricos del
   mismo DTO: el usuario verá todos los rangos incumplidos a la vez.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [3. Cadenas de texto](./3_EnsureFpStrings.md)
- [5. Colecciones](./5_EnsureFpCollections.md) — comprobaciones de cardinalidad
- [7. Tipos `Nullable<T>`](./7_EnsureFpNullables.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
