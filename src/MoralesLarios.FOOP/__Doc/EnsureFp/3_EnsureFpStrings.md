# `EnsureFp` — Cadenas de texto

> Archivo fuente: `Helpers/EnsureFp.Strings.cs`.

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [Inventario de la API](#inventario-de-la-api)
- [1. Nulidad y vacío: `NotNullOrEmpty`](#1-nulidad-y-vacío-notnullorempty)
- [2. Longitud](#2-longitud)
- [3. Expresiones regulares: `Matches` / `NotMatches`](#3-expresiones-regulares-matches--notmatches)
- [4. Prefijos, sufijos y contenido](#4-prefijos-sufijos-y-contenido)
- [5. Conjuntos de valores permitidos: `IsOneOf`](#5-conjuntos-de-valores-permitidos-isoneof)
- [6. Semántica de `null` y de las comparaciones](#6-semántica-de-null-y-de-las-comparaciones)
- [7. Ejemplos completos](#7-ejemplos-completos)
- [8. Mejores prácticas](#8-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

Las cadenas son el tipo de dato que más validaciones necesita: longitudes, formatos, listas
cerradas de valores, prefijos… Este bloque cubre las reglas de texto habituales para no tener que
escribirlas a mano en cada proyecto, con dos garantías transversales:

- **Nunca lanzan.** Una entrada `null` produce un fallo, no una `NullReferenceException`.
- **Las expresiones regulares tienen tiempo de espera.** Todas las llamadas a `Regex` internas
  usan `REGEX_DEFAULT_TIMEOUT = TimeSpan.FromSeconds(2)`, lo que protege frente a ataques
  ReDoS con patrones catastróficos.

Cada regla existe en las tres variantes habituales: `string` de mensaje, `MlErrorsDetails`
enriquecido y `*Arg` con mensaje automático vía `[CallerArgumentExpression]`
(ver [la convención de las tres variantes](./1_EnsureFpCore.md#la-convención-de-las-tres-variantes)).

---

## Inventario de la API

| Regla | Comprueba | Variantes |
|---|---|---|
| `NotNullOrEmpty` | no `null` y `Length > 0` (acepta `"   "`) | `string` · `MlErrorsDetails` · `Arg` |
| `MaxLength` | `Length <= max` | `string` · `MlErrorsDetails` · `Arg` |
| `MinLength` | `Length >= min` | `string` · `MlErrorsDetails` · `Arg` |
| `LengthBetween` | `min <= Length <= max` | `string` · `MlErrorsDetails` · `Arg` |
| `LengthExactly` | `Length == n` | `string` · `MlErrorsDetails` · `Arg` |
| `Matches` | encaja con un patrón o un `Regex` | `string` · `MlErrorsDetails` · `Arg` |
| `NotMatches` | **no** encaja con el patrón | `string` · `MlErrorsDetails` · `Arg` |
| `StartsWith` | empieza por un prefijo | `string` · `MlErrorsDetails` · `Arg` |
| `EndsWith` | termina por un sufijo | `string` · `MlErrorsDetails` · `Arg` |
| `ContainsText` | contiene una subcadena | `string` · `MlErrorsDetails` · `Arg` |
| `NotContainsText` | **no** contiene una subcadena | `string` · `MlErrorsDetails` · `Arg` |
| `IsOneOf` | pertenece a un conjunto cerrado | `string` · `MlErrorsDetails` · `Arg` · genérico `<T>` |

> ℹ️ **¿Por qué `ContainsText` y no `Contains`?** `Contains` colisionaría conceptualmente con
> `ContainsItem` de la familia de colecciones y con los métodos de extensión de LINQ. El sufijo
> `Text` hace explícito que la comprobación es sobre una cadena.

---

## 1. Nulidad y vacío: `NotNullOrEmpty`

```csharp
public static MlResult<string> NotNullOrEmpty(string value, string errorMessage);
public static MlResult<string> NotNullOrEmpty(string value, MlErrorsDetails errorsDetails);
public static MlResult<string> NotNullOrEmptyArg(string value, [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Diferencia crítica respecto a `NotNullEmptyOrWhitespace` (del [núcleo](./1_EnsureFpCore.md)):

| Entrada | `NotNullOrEmpty` | `NotNullEmptyOrWhitespace` |
|---|---|---|
| `null` | ❌ fallo | ❌ fallo |
| `""` | ❌ fallo | ❌ fallo |
| `"   "` | ✅ **válido** | ❌ fallo |
| `"texto"` | ✅ válido | ✅ válido |

Usa `NotNullOrEmpty` cuando el espacio **es** contenido significativo (un separador configurado,
un carácter de relleno en un fichero de posiciones fijas). Para entradas de usuario, usa casi
siempre `NotNullEmptyOrWhitespace`.

---

## 2. Longitud

```csharp
public static MlResult<string> MaxLength(string value, int maxLength, string errorMessage);
public static MlResult<string> MaxLength(string value, int maxLength, MlErrorsDetails errorsDetails);
public static MlResult<string> MaxLengthArg(string value, int maxLength, [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<string> MinLength(string value, int minLength, string errorMessage);
// … + MlErrorsDetails + MinLengthArg

public static MlResult<string> LengthBetween(string value, int minLength, int maxLength, string errorMessage);
// … + MlErrorsDetails + LengthBetweenArg

public static MlResult<string> LengthExactly(string value, int length, string errorMessage);
// … + MlErrorsDetails + LengthExactlyArg
```

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

var r1 = MaxLength(descripcion, 500, "La descripción no puede superar los 500 caracteres.");
var r2 = LengthBetween(usuario, 3, 20, "El usuario debe tener entre 3 y 20 caracteres.");
var r3 = LengthExactly(codigoPostal, 5, "El código postal debe tener exactamente 5 dígitos.");

// Con mensaje automático:
var r4 = MinLengthArg(password, 8);   // "'password' debe tener al menos 8 caracteres (actual: 4)."
```

Los mensajes automáticos incluyen la **longitud real** además de la esperada, lo que ahorra un
viaje de ida y vuelta al depurar.

`LengthExactly` es la regla natural para códigos de formato fijo: NIF, IBAN por tramos, códigos
de país ISO, códigos postales, referencias internas…

---

## 3. Expresiones regulares: `Matches` / `NotMatches`

```csharp
public static MlResult<string> Matches(string value, string pattern, string errorMessage);
public static MlResult<string> Matches(string value, string pattern, MlErrorsDetails errorsDetails);
public static MlResult<string> Matches(string value, Regex  regex,   string errorMessage);
public static MlResult<string> Matches(string value, Regex  regex,   MlErrorsDetails errorsDetails);
public static MlResult<string> MatchesArg(string value, string pattern, [CallerArgumentExpression(nameof(value))] string? paramName = null);
public static MlResult<string> MatchesArg(string value, Regex  regex,   [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<string> NotMatches(string value, string pattern, string errorMessage);
public static MlResult<string> NotMatches(string value, string pattern, MlErrorsDetails errorsDetails);
public static MlResult<string> NotMatches(string value, Regex  regex,   string errorMessage);
public static MlResult<string> NotMatchesArg(string value, string pattern, [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

**Dos sobrecargas, dos usos:**

| Sobrecarga | Cuándo |
|---|---|
| `pattern` como `string` | uso puntual; la librería aplica el timeout de 2 s automáticamente |
| `Regex` precompilado | uso repetido o en bucle: compila una vez y reutiliza |

```csharp
// Uso puntual.
var r = Matches(referencia, @"^REF-\d{6}$", "La referencia debe seguir el formato REF-000000.");

// Uso intensivo: compila una vez.
private static readonly Regex ReferenciaRegex =
    new(@"^REF-\d{6}$", RegexOptions.Compiled, TimeSpan.FromSeconds(2));

var r2 = Matches(referencia, ReferenciaRegex, "Referencia con formato incorrecto.");
```

> ⚠️ Si construyes tú el `Regex`, **pon tú el timeout**: la librería solo puede aplicar el suyo a
> los patrones que compila internamente.

`NotMatches` es la regla de las listas negras: rechazar caracteres peligrosos, secuencias
reservadas o patrones prohibidos.

```csharp
var seguro = NotMatches(nombreFichero, @"[<>:""/\\|?*]",
                        "El nombre de fichero contiene caracteres no permitidos.");
```

---

## 4. Prefijos, sufijos y contenido

```csharp
public static MlResult<string> StartsWith(string value, string prefix, string errorMessage,
                                          StringComparison comparisonType = StringComparison.Ordinal);
public static MlResult<string> StartsWith(string value, string prefix, MlErrorsDetails errorsDetails,
                                          StringComparison comparisonType = StringComparison.Ordinal);
public static MlResult<string> StartsWithArg(string value, string prefix,
                                             StringComparison comparisonType = StringComparison.Ordinal,
                                             [CallerArgumentExpression(nameof(value))] string? paramName = null);

// EndsWith / EndsWithArg          → idéntico, con el parámetro `suffix`
// ContainsText / ContainsTextArg  → idéntico, con el parámetro `substring`
// NotContainsText / NotContainsTextArg
```

El parámetro `comparisonType` es **`Ordinal` por defecto**, la opción correcta para identificadores,
códigos, rutas y protocolos: es la comparación más rápida y no depende de la cultura del hilo.

```csharp
// Comparación ordinal (por defecto): "https://" debe ir en minúsculas.
var r1 = StartsWith(url, "https://", "La URL debe usar HTTPS.");

// Comparación insensible a mayúsculas cuando el dato viene de un usuario.
var r2 = StartsWith(codigo, "es-", "El código debe pertenecer a España.",
                    StringComparison.OrdinalIgnoreCase);

// Rechazar contenido no permitido.
var r3 = NotContainsText(comentario, "<script", "El comentario contiene contenido no permitido.",
                         StringComparison.OrdinalIgnoreCase);
```

> ⚠️ Un `prefix`, `suffix` o `substring` **`null` produce fallo**, no éxito. La regla nunca se
> «desactiva» silenciosamente por un parámetro mal informado.

---

## 5. Conjuntos de valores permitidos: `IsOneOf`

Versión específica para cadenas, con `StringComparer` opcional:

```csharp
public static MlResult<string> IsOneOf(string value, IEnumerable<string> allowedValues, string errorMessage,
                                       StringComparer? comparer = null);
public static MlResult<string> IsOneOf(string value, IEnumerable<string> allowedValues, MlErrorsDetails errorsDetails,
                                       StringComparer? comparer = null);
public static MlResult<string> IsOneOfArg(string value, IEnumerable<string> allowedValues,
                                          StringComparer? comparer = null,
                                          [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Y versión genérica para cualquier tipo, con `IEqualityComparer<T>` opcional:

```csharp
public static MlResult<T> IsOneOf<T>(T value, IEnumerable<T> allowedValues, string errorMessage,
                                     IEqualityComparer<T>? comparer = null);
public static MlResult<T> IsOneOfArg<T>(T value, IEnumerable<T> allowedValues,
                                        IEqualityComparer<T>? comparer = null,
                                        [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

```csharp
private static readonly string[] EstadosValidos = ["Pendiente", "Enviado", "Entregado", "Cancelado"];

var r1 = IsOneOf(estado, EstadosValidos, "Estado de pedido no reconocido.");

// Insensible a mayúsculas y minúsculas.
var r2 = IsOneOf(estado, EstadosValidos, "Estado no reconocido.", StringComparer.OrdinalIgnoreCase);

// Versión genérica sobre valores no textuales.
var r3 = IsOneOf(codigoIva, new[] { 0, 4, 10, 21 }, "Tipo de IVA no válido.");
```

> Para validar que un valor de tipo `enum` es uno de los declarados, usa
> [`IsDefined<TEnum>`](./6_EnsureFpTypes.md#enumeraciones-isdefined): es más rápido y no requiere
> mantener una lista aparte.

---

## 6. Semántica de `null` y de las comparaciones

| Situación | Comportamiento |
|---|---|
| `value` es `null` | **fallo** en todas las reglas del bloque |
| `pattern` / `prefix` / `suffix` / `substring` es `null` | **fallo** |
| `allowedValues` es `null` o está vacío | **fallo** (ningún valor puede pertenecer al conjunto vacío) |
| `comparer` es `null` | se usa el comparador por defecto (`StringComparer.Ordinal` / `EqualityComparer<T>.Default`) |
| `Regex` es `null` | **fallo** |
| El `Regex` supera el timeout | **fallo** con la excepción disponible en `Details["Ex"]` si se usó `TryThat` |

Helpers privados que implementan estas reglas: `HasMaxLength`, `HasMinLength`, `HasLengthBetween`,
`IsMatch(string,string)`, `IsMatch(string,Regex)`, `IsInSet`. Todos comprueban la nulidad antes de
tocar el valor.

---

## 7. Ejemplos completos

### 7.1. Validación de credenciales

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public MlResult<Credenciales> Validar(string usuario, string password) =>
    All((usuario, password),
        c => LengthBetween(c.usuario, 3, 20, "El usuario debe tener entre 3 y 20 caracteres.").Map(_ => c),
        c => Matches(c.usuario, @"^[a-z0-9._-]+$",
                     "El usuario solo admite minúsculas, dígitos, punto, guion y guion bajo.").Map(_ => c),
        c => MinLength(c.password, 12, "La contraseña debe tener al menos 12 caracteres.").Map(_ => c),
        c => Matches(c.password, @"[A-Z]",   "La contraseña debe incluir una mayúscula.").Map(_ => c),
        c => Matches(c.password, @"\d",      "La contraseña debe incluir un dígito.").Map(_ => c),
        c => Matches(c.password, @"[^\w\s]", "La contraseña debe incluir un símbolo.").Map(_ => c),
        c => NotContainsText(c.password, c.usuario,
                             "La contraseña no puede contener el nombre de usuario.",
                             StringComparison.OrdinalIgnoreCase).Map(_ => c))
    .Map(c => new Credenciales(c.usuario, c.password));
```

### 7.2. Normalización de una ruta de almacenamiento

```csharp
public MlResult<string> ValidarRutaBlob(string ruta) =>
    NotNullEmptyOrWhitespaceArg(ruta)
        .Bind(r => MaxLength(r, 1024, "La ruta supera el límite de 1024 caracteres."))
        .Bind(r => NotContainsText(r, "..", "La ruta no puede contener saltos de directorio."))
        .Bind(r => NotMatches(r, @"[\x00-\x1F]", "La ruta contiene caracteres de control."))
        .Bind(r => StartsWith(r, "tenant/", "La ruta debe estar dentro del contenedor del tenant."));
```

### 7.3. Cabecera de un fichero de intercambio

```csharp
private static readonly Regex CabeceraRegex =
    new(@"^H\|(?<fecha>\d{8})\|(?<origen>[A-Z]{4})$", RegexOptions.Compiled, TimeSpan.FromSeconds(2));

public MlResult<string> ValidarCabecera(string linea) =>
    NotNullOrEmptyArg(linea)
        .Bind(l => LengthExactly(l, 24, "La cabecera debe ocupar exactamente 24 caracteres."))
        .Bind(l => StartsWith(l, "H|", "La primera línea debe ser una cabecera."))
        .Bind(l => Matches(l, CabeceraRegex, "El formato de la cabecera no es válido."));
```

---

## 8. Mejores prácticas

1. **Elige bien entre `NotNullOrEmpty` y `NotNullEmptyOrWhitespace`.** Para datos de usuario,
   casi siempre el segundo.
2. **Precompila los `Regex` que se usan más de una vez** y ponles siempre un timeout explícito.
3. **`Ordinal` por defecto es correcto.** Cambia a `OrdinalIgnoreCase` solo cuando el dato lo pida;
   evita las variantes con cultura salvo para texto realmente lingüístico.
4. **Valida la longitud antes que el formato.** Un `MaxLength` es O(1) y evita ejecutar un regex
   sobre una entrada gigante.
5. **`NotMatches` para listas negras, `Matches` para listas blancas.** Cuando sea posible, prefiere
   la lista blanca: es más segura.
6. **Guarda los conjuntos de `IsOneOf` en campos `static readonly`** para no asignar un array por
   llamada.
7. **Combina con [`All`](./2_EnsureFpAggregation.md)** para devolver todos los problemas de formato
   de un formulario en una sola respuesta.
8. **Para emails y URLs no uses regex**: usa
   [`IsValidEmail` e `IsValidUri`](./6_EnsureFpTypes.md), basados en los parsers de la BCL.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [5. Colecciones](./5_EnsureFpCollections.md)
- [6. Tipos concretos: Guid, enum, fechas, URI, email, rutas](./6_EnsureFpTypes.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
