# `EnsureFp` — Tipos `Nullable<T>`: validar y desenvolver a la vez

> Archivo fuente: `Helpers/EnsureFp.Types.cs` (región *Nullables*).

## Índice

- [El problema: `T?` contamina toda la cadena](#el-problema-t-contamina-toda-la-cadena)
- [1. `NotNullValue`](#1-notnullvalue)
- [2. `NotNullValueArg`](#2-notnullvaluearg)
- [3. `NotNullValueThat`: desenvolver y validar en un solo paso](#3-notnullvaluethat-desenvolver-y-validar-en-un-solo-paso)
- [4. `NotNullValueAsync`](#4-notnullvalueasync)
- [5. Por qué `NotNull` no sirve para `Nullable<T>`](#5-por-qué-notnull-no-sirve-para-nullablet)
- [6. Ejemplos completos](#6-ejemplos-completos)
- [7. Mejores prácticas](#7-mejores-prácticas)
- [Ver también](#ver-también)

---

## El problema: `T?` contamina toda la cadena

Los DTO de entrada declaran los tipos valor como `Nullable<T>` para poder distinguir «no enviado» de
«enviado con valor cero». Eso es correcto en el borde del sistema, pero si el `T?` se propaga hacia
dentro aparecen los `.Value`, los `!` y las advertencias del compilador:

```csharp
// ❌ El T? se arrastra y aparecen los .Value defensivos
public MlResult<decimal> Calcular(decimal? importe, int? unidades)
{
    if (importe is null)  return MlResult<decimal>.Fail("Importe obligatorio.");
    if (unidades is null) return MlResult<decimal>.Fail("Unidades obligatorias.");

    return importe.Value / unidades.Value;   // .Value por todas partes
}
```

Las reglas de este bloque hacen las dos cosas de golpe: **validan que hay valor y devuelven el valor
desenvuelto**, de modo que el resto de la cadena trabaja con `T`, no con `T?`.

```csharp
// ✅ La cadena trabaja con decimal e int, no con decimal? e int?
public MlResult<decimal> Calcular(decimal? importe, int? unidades) =>
    NotNullValueArg(importe)
        .Bind(i => NotNullValueArg(unidades)
                       .Bind(u => NotZeroArg(u))
                       .Map(u => i / u));
```

Todas las reglas del bloque tienen la restricción `where T : struct` y devuelven `MlResult<T>`
(el tipo **desenvuelto**).

---

## 1. `NotNullValue`

```csharp
public static MlResult<T> NotNullValue<T>(T? value, string errorMessage)           where T : struct;
public static MlResult<T> NotNullValue<T>(T? value, MlErrorsDetails errorsDetails) where T : struct;
```

```csharp
int?      edad   = null;
decimal?  precio = 19.99m;

MlResult<int>     r1 = NotNullValue(edad,   "La edad es obligatoria.");   // ❌ Fail
MlResult<decimal> r2 = NotNullValue(precio, "El precio es obligatorio."); // ✅ Valid(19.99)
```

Fíjate en el tipo de retorno: `MlResult<int>`, no `MlResult<int?>`. Ahí está todo el valor del método.

Funciona con cualquier `struct`, no solo con numéricos:

```csharp
DateTime?  fecha   = null;
Guid?      id      = Guid.NewGuid();
TimeSpan?  duracion = TimeSpan.FromMinutes(30);
EstadoPedido? estado = EstadoPedido.Confirmado;

var r1 = NotNullValue(fecha,    "La fecha es obligatoria.");    // MlResult<DateTime>
var r2 = NotNullValue(id,       "El id es obligatorio.");       // MlResult<Guid>
var r3 = NotNullValue(duracion, "La duración es obligatoria."); // MlResult<TimeSpan>
var r4 = NotNullValue(estado,   "El estado es obligatorio.");   // MlResult<EstadoPedido>
```

---

## 2. `NotNullValueArg`

```csharp
public static MlResult<T> NotNullValueArg<T>(T? value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct;
```

Genera el mensaje automáticamente a partir de la **expresión escrita en la llamada** y añade a los
detalles la clave `ParamName`:

```csharp
var r = NotNullValueArg(dto.FechaEfecto);
// Mensaje: "'dto.FechaEfecto' no puede ser null."
// Details: ParamName = "dto.FechaEfecto"
```

Es la variante recomendada en la capa de entrada: cero texto que mantener y diagnóstico exacto.
Detalles de la convención en
[la convención de las tres variantes](./1_EnsureFpCore.md#la-convención-de-las-tres-variantes).

---

## 3. `NotNullValueThat`: desenvolver y validar en un solo paso

```csharp
public static MlResult<T> NotNullValueThat<T>(T? value, Func<T, bool> predicate, string errorMessage)
    where T : struct;
```

Combina las dos comprobaciones que casi siempre van juntas:

1. ¿hay valor?
2. ¿el valor cumple la regla de negocio?

```csharp
// ❌ Dos pasos y un Bind intermedio
var r1 = NotNullValue(edad, "La edad es obligatoria.")
             .Bind(e => InRange(e, 18, 120, "Edad fuera de rango."));

// ✅ Un solo paso
var r2 = NotNullValueThat(edad, e => e is >= 18 and <= 120,
                          "La edad es obligatoria y debe estar entre 18 y 120.");
```

El predicado recibe el valor **ya desenvuelto** (`T`, no `T?`), así que no hay `.Value` ni
comparaciones con `null` dentro de la lambda:

```csharp
var r1 = NotNullValueThat(fechaEfecto, f => f > DateTime.UtcNow,
                          "La fecha de efecto es obligatoria y debe ser futura.");

var r2 = NotNullValueThat(importe, i => i > 0m,
                          "El importe es obligatorio y debe ser positivo.");

var r3 = NotNullValueThat(clienteId, g => g != Guid.Empty,
                          "El identificador de cliente no es válido.");
```

**Contrapartida:** el mensaje es único para los dos motivos de fallo. Si necesitas distinguir «no
enviado» de «valor incorrecto» (por ejemplo para responder con códigos distintos), usa los dos pasos
encadenados. Un predicado `null` hace que la regla falle, igual que en el resto de `EnsureFp`.

---

## 4. `NotNullValueAsync`

```csharp
public static async Task<MlResult<T>> NotNullValueAsync<T>(Task<T?> valueAsync, string errorMessage)
    where T : struct;
```

Versión para fuentes asíncronas: espera la tarea de forma protegida (mediante el helper privado
`SecureAwait`, que trata una tarea `null` como `default!` en lugar de lanzar excepción) y aplica la
misma regla.

```csharp
public async Task<MlResult<decimal>> ObtenerSaldoAsync(Guid clienteId) =>
    await NotNullValueAsync(repositorio.BuscarSaldoAsync(clienteId),
                            "El cliente no tiene saldo registrado.");
```

Es el patrón idiomático para los `FirstOrDefaultAsync()` de Entity Framework que devuelven
`Nullable<T>`: convierte «no encontrado» en un fallo del carril sin `if` ni excepciones.

Más sobrecargas asíncronas en [8. Variantes asíncronas](./8_EnsureFpAsync.md).

---

## 5. Por qué `NotNull` no sirve para `Nullable<T>`

Podría parecer que `NotNull(edad, "…")` resuelve el caso, pero no:

```csharp
int? edad = 5;

MlResult<int?> a = NotNull(edad, "…");         // MlResult<int?>  → el T? sigue ahí
MlResult<int>  b = NotNullValue(edad, "…");    // MlResult<int>   → desenvuelto ✅
```

`NotNull<T>` es genérico sin restricción y no puede desenvolver: infiere `T = int?` y devuelve
`MlResult<int?>`. Toda la cadena posterior seguiría trabajando con nulos y necesitando `.Value`.

Resumen de qué usar en cada caso:

| Entrada | Regla correcta | Salida |
|---|---|---|
| `T?` con `T : struct` (`int?`, `Guid?`, `DateTime?`) | `NotNullValue` / `NotNullValueArg` | `MlResult<T>` |
| Tipo referencia (`string`, `Cliente`) | [`NotNull`](./1_EnsureFpCore.md) / `NotNullArg` | `MlResult<T>` |
| `string` que además no puede estar vacío | [`NotNullEmptyOrWhitespace`](./3_EnsureFpStrings.md) | `MlResult<string>` |
| `Guid?` que además no puede ser `Guid.Empty` | [`NotNullNotEmptyGuid`](./6_EnsureFpTypes.md#1-guid-notemptyguid-y-notnullnotemptyguid) | `MlResult<Guid>` |
| Colección que no puede ser nula ni vacía | [`NotEmptyCollection`](./5_EnsureFpCollections.md) | `MlResult<TCollection>` |

---

## 6. Ejemplos completos

### 6.1. DTO de entrada con campos opcionales en el contrato

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public record CrearReservaDto(Guid? SalaId, DateTime? Inicio, int? Asistentes, decimal? Presupuesto);

public MlResult<Reserva> Crear(CrearReservaDto dto) =>
    All(dto,
        d => NotNullNotEmptyGuid(d.SalaId, "La sala es obligatoria.").Map(_ => d),
        d => NotNullValueThat(d.Inicio, i => i > DateTime.UtcNow,
                              "La fecha de inicio es obligatoria y debe ser futura.").Map(_ => d),
        d => NotNullValueThat(d.Asistentes, a => a is >= 1 and <= 50,
                              "Los asistentes son obligatorios y deben estar entre 1 y 50.").Map(_ => d),
        d => NotNullValueThat(d.Presupuesto, p => p >= 0m,
                              "El presupuesto es obligatorio y no puede ser negativo.").Map(_ => d))
    .Map(d => new Reserva(d.SalaId!.Value, d.Inicio!.Value, d.Asistentes!.Value, d.Presupuesto!.Value));
```

Los `!.Value` finales son seguros: en ese punto las reglas ya han garantizado que hay valor.

### 6.2. Distinguir «no enviado» de «valor incorrecto»

```csharp
public MlResult<int> ValidarEdad(int? edad) =>
    NotNullValueArg(edad)                                    // 400: campo obligatorio
        .Bind(e => InRangeArg(e, 18, 120));                  // 422: valor fuera de rango
```

Los dos fallos llevan mensajes y detalles distintos (`ParamName`, `Value`, `Expected`), lo que
permite responder con códigos HTTP diferentes.

### 6.3. Lectura desde base de datos

```csharp
public async Task<MlResult<Factura>> ObtenerAsync(int? facturaId) =>
    await NotNullValueThat(facturaId, id => id > 0,
                           "El identificador de factura es obligatorio y debe ser positivo.")
              .BindAsync(id => repositorio.BuscarAsync(id));
```

### 6.4. Parámetros opcionales de configuración con valor por defecto

```csharp
// Si no viene, se aplica el valor por defecto; si viene, se valida.
public MlResult<int> ResolverTimeout(int? timeoutSegundos) =>
    timeoutSegundos is null
        ? MlResult<int>.Valid(30)
        : InRangeArg(timeoutSegundos.Value, 1, 300);
```

Aquí `NotNullValue` **no** es la regla adecuada: la ausencia es legítima. Documentar esta distinción
evita convertir campos opcionales en obligatorios por inercia.

---

## 7. Mejores prácticas

1. **Desenvuelve en el borde del sistema.** El `T?` debe morir en el controlador o en el servicio de
   aplicación; hacia dentro se trabaja con `T`.
2. **`NotNullValue`, no `NotNull`, para `Nullable<T>`.** `NotNull` no desenvuelve.
3. **Usa `NotNullValueThat` cuando el mensaje pueda ser único** para ambos motivos de fallo; encadena
   dos reglas cuando necesites distinguirlos.
4. **`NotNullValueArg` en la capa de entrada**: mensaje y `ParamName` automáticos, cero texto que
   mantener.
5. **No conviertas en obligatorio lo que es opcional.** Si la ausencia tiene un valor por defecto
   legítimo, resuélvelo con `??` o con un `if` explícito, no con `NotNullValue`.
6. **Para `Guid?` usa
   [`NotNullNotEmptyGuid`](./6_EnsureFpTypes.md#1-guid-notemptyguid-y-notnullnotemptyguid)**: cubre
   `null` y `Guid.Empty` en una sola regla.
7. **Agrupa con [`All`](./2_EnsureFpAggregation.md)** para devolver todos los campos que faltan de
   una vez, en lugar de uno por petición.
8. **En consultas asíncronas usa `NotNullValueAsync`** para convertir «no encontrado» en un fallo del
   carril sin excepciones.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [6. Tipos concretos: `Guid`, enumerados, fechas…](./6_EnsureFpTypes.md)
- [8. Variantes asíncronas](./8_EnsureFpAsync.md)
- [`NullToFailed`](../Several/2_NullToFailed.md) — alternativa por extensión
