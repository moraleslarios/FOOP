# MoralesLarios.OOFP.ValueObjects — tipos con significado en vez de primitivos

Librería de **value objects** (objetos de valor) que sustituyen los primitivos anónimos (`string`, `int`, `decimal`, `double`) por tipos que **llevan su propia validación dentro**.

La idea es sencilla y muy potente: si un método recibe un `Mail`, ya no hace falta comprobar si el texto tiene arroba. **Si el objeto existe, es válido.** Y si no se puede construir, el error viaja como `MlResult<T>` por el mismo canal que el resto de la aplicación.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Las dos formas de crear un value object](#las-dos-formas-de-crear-un-value-object)
4. [Clases base: `ValueObject` y `ValueObject<TValue>`](#clases-base-valueobject-y-valueobjecttvalue)
5. [El patrón común de todos los VO](#el-patrón-común-de-todos-los-vo)
6. [Catálogo completo por familias](#catálogo-completo-por-familias)
7. [Conversiones implícitas y explícitas](#conversiones-implícitas-y-explícitas)
8. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
9. [⚠️ Lo que NO existe](#️-lo-que-no-existe)
10. [Ejemplos prácticos](#ejemplos-prácticos)
11. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
12. [Mejores prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

El problema clásico es la **obsesión por los primitivos**: todo es `string` o `int`, así que el compilador no puede ayudarte y la validación se dispersa por toda la aplicación.

❌ **Sin value objects:**

```csharp
public void RegistrarUsuario(string nombre, string email, int edad)
{
    // La validación se repite en CADA punto de entrada
    if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException(nameof(nombre));
    if (nombre.Length < 3)                 throw new ArgumentException(nameof(nombre));
    if (! Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$")) throw new ArgumentException(nameof(email));
    if (edad < 0)                          throw new ArgumentException(nameof(edad));
    // ...
}

// Y nada impide esto, que compila perfectamente:
RegistrarUsuario(email, nombre, edad);   // 💥 parámetros invertidos, sin aviso
```

✅ **Con value objects:**

```csharp
public void RegistrarUsuario(Name nombre, Mail email, Age edad)
{
    // Cero validaciones: si los objetos existen, son válidos.
}

// Y ahora esto NO compila: el compilador detecta el intercambio.
RegistrarUsuario(email, nombre, edad);   // ❌ error de compilación
```

> 💡 **La idea de fondo**: mover la validación **del método al tipo**. Se valida una sola vez, en la construcción, y a partir de ahí el tipo es la garantía. Esto se conoce como *"hacer que los estados inválidos sean imposibles de representar"*.

---

## Instalación y dependencias

| Dependencia | Para qué |
|---|---|
| [`MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md) (núcleo) | `MlResult<T>`, `MlErrorDetails`, [`EnsureFp`](../docs/EnsureFp/EnsureFp.md), `Bind`, `Map` |
| `System.Text.RegularExpressions` | Usado por `RegexValue` y sus derivados |

```csharp
using MoralesLarios.OOFP.ValueObjects;
```

**No necesita registro en el contenedor de dependencias.** Es una librería de tipos de dominio, sin servicios ni configuración.

---

## Las dos formas de crear un value object

Este es el concepto **más importante** de la librería. Cada VO ofrece dos rutas de creación, con semánticas opuestas:

| Ruta | Devuelve | Si el valor es inválido | Cuándo usarla |
|---|---|---|---|
| `CreateFrom…(valor)` | El VO directamente | **Lanza excepción** (`ArgumentNullException` / `ArgumentOutOfRangeException`) | Constantes del código, datos que ya sabes válidos, tests |
| `From…(valor)` | `MlResult<VO>` | **Devuelve un fallo**, no lanza | Entradas externas: API, formularios, ficheros, base de datos |
| Conversión implícita | El VO directamente | **Lanza excepción** (llama al constructor) | Literales en código propio |

```csharp
// CreateFrom… → lanza si falla. Úsalo con literales que controlas.
Mail soporte = Mail.CreateFromString("soporte@empresa.com");

// From… → nunca lanza. Úsalo con datos que vienen de fuera.
MlResult<Mail> resultado = Mail.FromString(entradaDelUsuario);

resultado.Match(
    valid: mail    => EnviarCorreo(mail),
    fail : errores => MostrarError(errores.GetErrorMessages()));

// Conversión implícita → azúcar sintáctico, pero lanza igual que CreateFrom…
Mail directo = "otro@empresa.com";
```

> ⚠️ **Regla práctica**: **todo dato que cruce el borde de la aplicación se construye con `From…`**. Reserva `CreateFrom…` y las conversiones implícitas para valores literales escritos por ti.

### `From…` no siempre se llama igual

El nombre del método `From…` depende del tipo primitivo de origen. Merece la pena tenerlo a mano:

| VO | Creación segura (`MlResult<T>`) | Creación directa (lanza) |
|---|---|---|
| `NotEmptyString`, `MemberName`, `ServiceKey`, `Mail`, `Endpoint` | `FromString(value, errorsDetails?)` | `CreateFromString(value)` |
| `RegexValue` | `FromRegex(value, pattern, errorsDetails?)` | `CreateFromRegex(value, pattern)` |
| `StringMinLength`, `StringMaxLength` | `FromLength(value, length, errorsDetails?)` | `CreateFromLength(value, length)` |
| `StringLengthBetween` | `FromLength(value, min, max, errorsDetails?)` | `CreateFromLength(value, min, max)` |
| `IntMoreThan`, `IntLessThan` | `FromIntLength(value, length, errorsDetails?)` | `CreateFromIntLength(value, length)` |
| `IntBetween` | `FromIntLength(value, min, max, errorsDetails?)` | `CreateFromIntLength(value, min, max)` |
| `IntNonNegative`, `Age`, `IdLite` | `FromInt(value, errorsDetails?)` | `CreateFromInt(value)` |
| `DecimalMoreThan`, `DecimalLessThan` | `FromDecimalLength(value, length, errorsDetails?)` | `CreateFromDecimalLength(value, length)` |
| `DecimalBetween` | `FromDecimalLength(value, min, max, errorsDetails?)` | `CreateFromDecimalLength(value, min, max)` |
| `DecimalNonNegative` | `FromDecimal(value, errorsDetails?)` | `CreateFromDecimal(value)` |
| `DoubleMoreThan`, `DoubleLessThan` | `FromDoubleLength(value, length, errorsDetails?)` | `CreateFromDoubleLength(value, length)` |
| `DoubleBetween` | `FromDoubleLength(value, min, max, errorsDetails?)` | `CreateFromDoubleLength(value, min, max)` |
| `DoubleNonNegative` | `FromDouble(value, errorsDetails?)` | `CreateFromDouble(value)` |
| `Id` | `ByDouble(value, errorsDetails?)` | `FromDouble(value)` |
| `StringAsInt`, `StringAsLong`, … | `FromString(value)` *(sin `errorsDetails`)* | `CreateFromString(value)` |
| `EmptyValue`, `Void` | — | `Create()` |

> ✅ **Resuelto en 2.0.0:** en la 1.x las factorías seguras se llamaban `By…` (`ByString`, `ByIntLength`,
> `ByStringLength`…) y varias llevaban la errata *"Lenght"* (`FromIntLenght`, `FromStringLenght`).
> Desde la 2.0.0 el patrón es uniforme: **`From…` devuelve `MlResult<T>`** y **`CreateFrom…` lanza**.
> Los nombres antiguos se conservan como puentes `[Obsolete]` y **se eliminan en la 3.0.0**.

> ⚠️ **Única excepción pendiente:** `Id` sigue invertido (`FromDouble` es la directa y `ByDouble` la
> segura). Se normalizará en una versión futura.

### Personalizar el mensaje de error

Casi todos los `From…` aceptan un `MlErrorDetails` opcional que **sustituye** al mensaje por defecto (que está en inglés):

```csharp
// Mensaje por defecto: "usuario@ is not a valid mail"
var r1 = Mail.FromString("usuario@");

// Mensaje propio, en español y con contexto de negocio
var r2 = Mail.FromString("usuario@", "El correo del destinatario no tiene un formato válido");
```

> 💡 `MlErrorDetails` tiene conversión implícita desde `string`, por eso puedes pasar el texto directamente sin construir nada.

---

## Clases base: `ValueObject` y `ValueObject<TValue>`

### `ValueObject` (abstracta)

Aporta **igualdad estructural**: dos value objects son iguales si sus valores atómicos son iguales, sin importar que sean instancias distintas.

```csharp
public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right);
    protected static bool NotEqualOperator(ValueObject left, ValueObject right);

    protected abstract IEnumerable<object> GetAtomicValues();   // ← lo que define la identidad

    public override bool Equals(object? obj);
    public override int  GetHashCode();

    public static bool operator ==(ValueObject left, ValueObject right);
    public static bool operator !=(ValueObject left, ValueObject right);

    public ValueObject GetCopy();      // copia superficial (MemberwiseClone)
}
```

Detalles de comportamiento reales:

- `Equals` exige **el mismo tipo exacto** (`obj.GetType() != GetType()` → `false`). Un `ServiceKey` nunca será igual a un `MemberName`, aunque el texto coincida.
- Compara los valores atómicos **en orden**, y exige que ambas secuencias tengan la misma longitud.
- `GetHashCode` combina los hashes con XOR.
- `GetCopy()` devuelve un `ValueObject` (tipo base), no el tipo concreto: tendrás que castear.

### `ValueObject<TValue>`

Base para VOs de **un solo valor**, que es el caso de prácticamente toda la librería.

```csharp
public class ValueObject<TValue> : ValueObject
{
    protected TValue Value;

    protected ValueObject(TValue value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        Value = value;
    }

    protected override IEnumerable<object> GetAtomicValues() { yield return Value!; }

    public static implicit operator TValue(ValueObject<TValue> ValueObjectFp) => ValueObjectFp.Value;
    public static explicit operator ValueObject<TValue>(TValue value)         => new ValueObject<TValue>(value);

    public override string? ToString() => Value?.ToString();
}
```

Puntos clave:

- **`Value` es `protected`**: desde fuera no se lee directamente. Se accede mediante la **conversión implícita al primitivo**, que cada VO concreto redefine.
- El constructor base **rechaza `null`** siempre, en todos los VO de la librería.
- `ToString()` delega en el valor interno, así que los VO se interpolan de forma natural: `$"Correo: {mail}"`.

### Jerarquía real de tipos

```
ValueObject (abstracta)
├── ValueObject<string>
│   ├── NotEmptyString
│   ├── Empty
│   └── Void
├── ValueObject<NotEmptyString>          ← ¡ojo: el valor interno es otro VO!
│   ├── RegexValue
│   │   ├── Mail
│   │   └── Endpoint
│   ├── StringMinLength  → Key, Name
│   ├── StringMaxLength
│   └── StringLengthBetween
├── ValueObject<int>
│   ├── IntMoreThan → IntNonNegative, Age, IdLite
│   ├── IntLessThan
│   └── IntBetween
├── ValueObject<decimal>
│   ├── DecimalMoreThan → DecimalNonNegative
│   ├── DecimalLessThan
│   └── DecimalBetween
├── ValueObject<double>
│   ├── DoubleMoreThan → DoubleNonNegative, Id
│   ├── DoubleLessThan
│   └── DoubleBetween
└── StringAsNumeric<TValue>   (hereda de ValueObject, no de ValueObject<T>)
    └── StringAsByte, StringAsSByte, StringAsShort, StringAsUShort,
        StringAsInt, StringAsUInt, StringAsLong, StringAsULong,
        StringAsFloat, StringAsDouble, StringAsDecimal
```

> 🔑 **Detalle elegante**: los VO de texto con longitud heredan de `ValueObject<NotEmptyString>`, no de `ValueObject<string>`. Eso significa que **la comprobación de "no vacío" está garantizada por el sistema de tipos** antes de mirar la longitud. La composición de validaciones es estructural, no una lista de `if`.

---

## El patrón común de todos los VO

Conocido el patrón, ya sabes usar los 30 tipos. Cada VO expone:

| Miembro | Tipo | Qué hace |
|---|---|---|
| `IsValid(...)` | `static bool` | Predicado puro de validación. **Útil para preguntar sin construir.** |
| `FormatErrorMessage(...)` | `static string` | Mensaje de error por defecto (en inglés). |
| `CreateFrom…(...)` | `static VO` | Construcción directa. **Lanza** si es inválido. |
| `From…(..., MlErrorDetails? )` | `static MlResult<VO>` | Construcción funcional. **No lanza.** |
| `implicit operator TPrimitivo(VO)` | conversión | Sacar el valor: `string texto = miMail;` |
| `implicit operator VO(TPrimitivo)` | conversión | Meter el valor: `Mail m = "a@b.com";` (lanza si es inválido) |

Y la implementación de `From…` es siempre una tubería funcional del núcleo:

```csharp
// Caso simple (numéricos): arranca de MlResult.Empty()
public static MlResult<IntNonNegative> FromInt(int value, MlErrorDetails errorsDetails = null!)
    => MlResult.Empty()
               .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
               .Map (_ => new IntNonNegative(value));

// Caso compuesto (texto): valida primero el VO padre y luego la regla propia
public static MlResult<Mail> FromString(string value, MlErrorDetails errorsDetails = null!)
    => NotEmptyString.FromString(value)
                     .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                     .Map (_ => new Mail(value));
```

> 💡 Fíjate en el `Bind` del segundo caso: si el texto viene vacío, **el error es "cannot be null or empty"**, no "is not a valid mail". El cortocircuito de [`Bind`](../docs/Bind/3_Bind.md) hace que el mensaje sea siempre el de la primera regla incumplida, que es el más informativo.

---

## Catálogo completo por familias

### Texto

| Tipo | Regla | Notas |
|---|---|---|
| `NotEmptyString` | `! string.IsNullOrWhiteSpace(value)` | La base de toda la familia de texto |
| `StringMinLength` | `value.Length >= length` | Mínimo **inclusivo** |
| `StringMaxLength` | `value.Length < length` | Máximo **exclusivo** ⚠️ |
| `StringLengthBetween` | `Length > min && Length < max` | **Ambos extremos exclusivos** ⚠️ |
| `ServiceKey` | `StringMinLength` con `MinLength = 1` | Claves de caché, identificadores textuales |
| `MemberName` | `StringMinLength` con `MinLength = 3` | Nombres de persona, etiquetas |
| `RegexValue` | `Regex.IsMatch(value, pattern)` | Expone `Pattern`; base de `Mail` y `Endpoint` |
| `Mail` | Patrón `^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$` | Constante pública `Mail.ValidationPattern` |
| `Endpoint` | Patrón `^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$` | **Solo `https`**, host + puerto opcional, **sin ruta** |

### Enteros

| Tipo | Regla | Notas |
|---|---|---|
| `IntMoreThan` | `value > length` | Estricto |
| `IntLessThan` | `value < length` | Estricto |
| `IntBetween` | `value > min && value < max` | Ambos exclusivos |
| `IntNonNegative` | `value >= limit` (con `limit = 0`) | El único de la familia `NotNegative` que funciona bien ⚠️ |
| `Age` | `value > -1` y además el ctor rechaza `< 0` | Edad, cero permitido |
| `IdLite` | `value > 0` | Identificador entero, cero no permitido |

### Decimales y dobles

| Tipo | Regla | Notas |
|---|---|---|
| `DecimalMoreThan` / `DoubleMoreThan` | `value > length` | Estricto |
| `DecimalLessThan` / `DoubleLessThan` | `value < length` | Estricto |
| `DecimalBetween` / `DoubleBetween` | `value > min && value < max` | Ambos exclusivos |
| `DecimalNonNegative` / `DoubleNonNegative` | `value < limit` (con `limit = 0`) | ⚠️ **La condición está invertida** — ver [particularidades](#️-particularidades-reales-del-código-fuente) |
| `Id` | `value > 0` (sobre `double`) | Métodos `ByDouble` (segura) / `FromDouble` (directa) |

### Parseo desde texto

`StringAsNumeric<TValue>` es la base genérica; los concretos son:

`StringAsByte` · `StringAsSByte` · `StringAsShort` · `StringAsUShort` · `StringAsInt` · `StringAsUInt` · `StringAsLong` · `StringAsULong` · `StringAsFloat` · `StringAsDouble` · `StringAsDecimal`

Sirven para **cruzar la frontera texto → número una sola vez y de forma segura**:

```csharp
MlResult<StringAsInt> cantidad = StringAsInt.FromString(Request.Query["cantidad"]);

int valor = cantidad.Match(valid: c => (int)c, fail: _ => 0);
```

Los concretos tienen doble conversión de salida: **al número** (`implicit operator int`) y **al texto** (`implicit operator string`, que hace `Value.ToString()`).

> ⚠️ La entrada se convierte con `int.TryParse` en los tipos concretos, pero `StringAsNumeric<TValue>` genérico usa `Convert.ChangeType`, que **lanza** en vez de devolver `false`. Por eso su `FromString` usa `TryBind` en lugar de `Bind`. Prefiere siempre los tipos concretos (`StringAsInt`) al genérico.

### Especiales

| Tipo | Creación | Para qué |
|---|---|---|
| `EmptyValue` | `EmptyValue.Create()`, `EmptyValue.CreateAsync()` | Marcador de "operación sin valor de retorno" en tuberías `MlResult<Empty>` |
| `Void` | `Void.Create()` | Equivalente a `EmptyValue`, sin variante asíncrona |

Ambos envuelven `string.Empty` y **ignoran cualquier valor que se les pase**. Son señales, no datos.

---

## Conversiones implícitas y explícitas

### Salida (VO → primitivo): siempre segura

```csharp
Mail   mail  = Mail.CreateFromString("a@b.com");
string texto = mail;                          // implícita, nunca falla

Age    edad  = Age.CreateFromInt(30);
int    n     = edad;                          // implícita, nunca falla
```

### Entrada (primitivo → VO): puede lanzar

```csharp
Mail ok  = "a@b.com";        // ✅ funciona
Mail mal = "esto-no-es-mail"; // 💥 ArgumentNullException en tiempo de ejecución
```

### VO que necesitan varios datos: conversión desde tupla

Los tipos paramétricos (`MoreThan`, `LessThan`, `Between`, `MinLength`…) no pueden convertirse desde un primitivo suelto, porque les falta el límite. Usan **tuplas**:

```csharp
IntMoreThan          positivo = (value: 5,        length: 0);
IntBetween           nota     = (value: 7,        minLength: 0,  maxLength: 11);
StringMinLength      titulo   = (value: "Hola",   length: 3);
StringLengthBetween  alias    = (value: "pepe",   minLength: 2,  maxLength: 20);
RegexValue           codigo   = (value: "AB-123", pattern: @"^[A-Z]{2}-\d{3}$");
```

> 💡 **Nombra los elementos de la tupla.** Con dos o tres `int` seguidos, invertir el orden es demasiado fácil y el compilador no puede ayudarte.

---

## ⚠️ Particularidades reales del código fuente

Observaciones sobre el código tal y como está hoy. **Conviene conocerlas antes de confiar en un tipo.**

### 1. ✅ `DecimalNonNegative` y `DoubleNonNegative` ya funcionan (corregido en 2.0.0)

```csharp
// DecimalNonNegative.cs
private const decimal MinAllowedValue = 0m;
public static bool IsValid(decimal value) => value >= MinAllowedValue;   // ← "cero o mayor"
```

En versiones anteriores la condición estaba **invertida** (`value < limit`, es decir, *solo negativos*) y además chocaba con el constructor base `DecimalMoreThan(value, 0)`, que exige `value > 0`: las dos condiciones eran incompatibles y **el tipo no se podía construir**.

> ✅ **Corregido en 2.0.0.** La validación es `value >= 0` y el constructor base recibe `MinAllowedValue - 1`, igual que `IntNonNegative`. Ya puedes usar `DecimalNonNegative.FromDecimal(importe)` y `DoubleNonNegative.FromDouble(valor)` con normalidad.

### 2. ✅ Los límites ya no son estado global mutable (corregido en 2.0.0)

```csharp
private const int     Limit           = 0;    // IntNonNegative
private const decimal MinAllowedValue = 0m;   // DecimalNonNegative
private const double  MinAllowedValue = 0;    // DoubleNonNegative
```

Antes eran `public static` (en `IntNonNegative`, incluso un campo sin `readonly`), así que cualquier código podía escribir `IntNonNegative.MinAllowedValue = 100;` y **cambiar la validación de toda la aplicación en caliente**.

> ✅ **Corregido en 2.0.0.** Los tres límites son `private const`: están fijados en compilación y no hay forma de alterarlos. Si necesitas un umbral propio, usa el tipo paramétrico (`IntMoreThan`, `DecimalMoreThan`, `DoubleMoreThan`).
>
> Los puentes `DecimalNonNegative.limit` y `DoubleNonNegative.limit` se conservan marcados como `[Obsolete]` y se eliminarán en la 3.0.0.

### 3. ⚠️ Los constructores públicos han pasado a `protected` (2.0.0)

`Age`, `Id` e `IdLite` tenían constructor `public` que lanzaba `ArgumentOutOfRangeException`. Como eso contradice el propio planteamiento de la librería (los errores viajan en `MlResult`, no como excepciones), ahora son `protected`.

```csharp
// ❌ Ya no compila
var edad = new Age(30);

// ✅ Vía de construcción correcta
MlResult<Age> edad  = Age.FromInt(30);        // datos externos → MlResult
Age           edad2 = Age.CreateFromInt(30);  // literal propio → lanza si es inválido
Age           edad3 = 30;                     // operador implícito (puede lanzar)
```

---

## ⚠️ Lo que NO existe

> ⚠️ **`EnumRangeValueObject` no existe.** El fichero `EnumRangeValueObject.cs` está **íntegramente comentado**: el tipo no compila ni se puede usar. Si necesitas validar contra un `enum`, hazlo con `EnsureFp.That(texto, Enum.TryParse<TEnum>(texto, true, out _), "…")`.

> ⚠️ **No existe** una propiedad pública `Value` en los VO. `Value` es `protected`. Para obtener el primitivo usa **la conversión implícita** (`string s = miMail;`) o `ToString()`.

> ⚠️ **No existen** métodos `Validate()` de instancia, ni `TryCreate(out …)`, ni variantes `…Async` de los `From…` (salvo `EmptyValue.CreateAsync()`). Tampoco hay conversión implícita de un VO a `MlResult<VO>`.

> ⚠️ **Sobre `MlErrorDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `GetErrorMessages()`, `GetErrorsSummary()`, `Errors.First().Message`, `GetMetadataValue<T>()`, `GetException()`, `GetMetadataSummary()`.

---

## Ejemplos prácticos

### Ejemplo 1 — Un modelo de dominio a prueba de balas

```csharp
using MoralesLarios.OOFP.ValueObjects;

public record Usuario(IdLite Id, Name Nombre, Mail Correo, Age Edad);

// Construcción segura: cada VO valida su parte y los errores se acumulan por cortocircuito
public static MlResult<Usuario> CrearUsuario(int id, string nombre, string correo, int edad)
    => IdLite.FromInt(id, "El identificador debe ser mayor que 0")
             .Bind(idVo => MemberName.FromString(nombre, "El nombre debe tener al menos 3 caracteres")
             .Bind(nomVo => Mail.FromString(correo, "El correo no tiene un formato válido")
             .Bind(mailVo => Age.FromInt(edad, "La edad no puede ser negativa")
             .Map (edadVo => new Usuario(idVo, nomVo, mailVo, edadVo)))));

// Uso
var resultado = CrearUsuario(1, "Ana", "ana@empresa.com", 34);

resultado.Match(
    valid: u       => Console.WriteLine($"Creado: {u.Nombre} <{u.Correo}>, {u.Edad} años"),
    fail : errores => Console.WriteLine($"No se pudo crear: {errores.GetErrorsSummary()}"));
```

> 💡 La interpolación `{u.Nombre}` funciona sin conversión explícita gracias al `ToString()` de `ValueObject<TValue>`.

### Ejemplo 2 — Validar la entrada de una API sin un solo `if`

```csharp
[HttpPost("suscripciones")]
public IActionResult Suscribir([FromBody] SuscripcionRequest request)
    => Mail.FromString(request.Email, "Indica un correo electrónico válido")
           .Bind(mail => MemberName.FromString(request.Nombre, "El nombre debe tener al menos 3 caracteres")
                             .Map(nombre => (mail, nombre)))
           .Bind(x => _servicio.Suscribir(x.mail, x.nombre))
           .Match(
               valid: _       => Ok(new { mensaje = "Suscripción registrada" }),
               fail : errores => BadRequest(new { errores = errores.GetErrorMessages() }));
```

### Ejemplo 3 — Convertir texto en número de forma segura

Típico al leer configuración, parámetros de query o ficheros CSV.

```csharp
public static MlResult<int> LeerPuerto(string textoPuerto)
    => StringAsInt.FromString(textoPuerto)
                  .Map (vo     => (int)vo)
                  .Bind(puerto => EnsureFp.That(puerto,
                                               puerto is > 0 and < 65_536,
                                               $"El puerto {puerto} está fuera del rango válido (1-65535)"));

// "8080"   → válido, 8080
// "ocho"   → fallo: "value should be a valid int"
// "70000"  → fallo: "El puerto 70000 está fuera del rango válido (1-65535)"
```

### Ejemplo 4 — Value objects como clave de caché y de comparación

La igualdad estructural hace que los VO funcionen perfectamente como claves de diccionario.

```csharp
var cache = new Dictionary<Key, string>();

cache[ServiceKey.CreateFromString("usuarios:activos")] = "…json…";

// Otra instancia distinta, pero igual en valor → encuentra la entrada
bool existe = cache.ContainsKey(ServiceKey.CreateFromString("usuarios:activos"));   // true

// Y la igualdad exige el mismo tipo exacto:
Key  k = ServiceKey.CreateFromString("hola");
Name n = MemberName.CreateFromString("hola");
bool iguales = k.Equals(n);      // false, aunque el texto coincida
```

### Ejemplo 5 — Endpoint validado para un cliente HTTP

```csharp
public static MlResult<HttpClient> CrearCliente(string urlBase)
    => Endpoint.FromString(urlBase, $"'{urlBase}' no es un endpoint https válido (host y puerto, sin ruta)")
               .Map(endpoint => new HttpClient { BaseAddress = new Uri(endpoint) });

// La conversión implícita Endpoint → string permite pasarlo directo a Uri.
```

### Ejemplo 6 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: usar CreateFrom… con datos externos → excepción no controlada
var mail = Mail.CreateFromString(Request.Form["email"]);        // 💥 si el usuario escribe cualquier cosa

// ✅ BIEN: From… devuelve MlResult y el error viaja por el canal normal
var mail = Mail.FromString(Request.Form["email"], "Correo no válido");


// ❌ MAL: volver a validar lo que el tipo ya garantiza
void Enviar(Mail destino)
{
    if (! destino.ToString()!.Contains('@')) throw new ArgumentException();   // imposible
}

// ✅ BIEN: confiar en el tipo
void Enviar(Mail destino) { /* … */ }


// ❌ MAL: dejar el mensaje en inglés llegar al usuario final
return BadRequest(MemberName.FromString(x).Match(valid: _ => "", fail: e => e.GetErrorMessages()));
//     → "ab cannot be less than 3 characters"

// ✅ BIEN: mensaje propio en español
return BadRequest(MemberName.FromString(x, "El nombre debe tener al menos 3 caracteres")
                      .Match(valid: _ => "", fail: e => e.GetErrorMessages()));


// ❌ MAL: asumir que los rangos son inclusivos
var titulo = StringMaxLength.FromLength("12345", 5);      // FALLA: exige Length < 5

// ✅ BIEN: recordar que el máximo es exclusivo
var titulo = StringMaxLength.FromLength("12345", 6);


// ❌ MAL: construir un value object con new
var edad = new Age(30);                   // no compila: el constructor es protected

// ✅ BIEN: usar la factoría
MlResult<Age> edad  = Age.FromInt(30, "La edad no puede ser negativa");
Age           edad2 = Age.CreateFromInt(30);   // solo para literales propios


// ❌ MAL: intentar cambiar el umbral de un tipo NonNegative
IntNonNegative.MinAllowedValue = 10;      // no compila: es private const

// ✅ BIEN: usar el tipo paramétrico para límites propios
var vo = IntMoreThan.FromIntLength(valor, 9);

// ❌ MAL: buscar EnumRangeValueObject
// (el fichero está completamente comentado: el tipo no existe)

// ✅ BIEN
var estado = EnsureFp.That(texto, Enum.TryParse<EstadoPedido>(texto, true, out _),
                           $"'{texto}' no es un estado de pedido válido");
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| Texto obligatorio, sin más reglas | `NotEmptyString` |
| Nombre de persona / etiqueta legible | `MemberName` (mínimo 3 caracteres) |
| Clave de caché o identificador textual | `ServiceKey` (mínimo 1 carácter) |
| Correo electrónico | `Mail` |
| URL base `https` sin ruta | `Endpoint` |
| Cualquier otro formato de texto | `RegexValue` con tu patrón |
| Longitud mínima / máxima / rango | `StringMinLength` / `StringMaxLength` / `StringLengthBetween` |
| Entero no negativo (cero incluido) | `IntNonNegative` |
| Entero mayor que cero (identificador) | `IdLite` |
| Edad | `Age` |
| Identificador numérico `double` | `Id` |
| Entero con límite propio | `IntMoreThan` / `IntLessThan` / `IntBetween` |
| Importe monetario con límite | `DecimalMoreThan` / `DecimalBetween` |
| Importe monetario no negativo (cero incluido) | `DecimalNonNegative` / `DoubleNonNegative` |
| Convertir texto de entrada en número | `StringAsInt`, `StringAsDecimal`, … |
| Señalar "sin valor de retorno" en una tubería | `EmptyValue` / `Void` |
| Validar contra un `enum` | `EnsureFp.That(…, Enum.TryParse<T>(…), …)` |

---

## Mejores prácticas

1. **Usa `From…` para todo dato externo y `CreateFrom…` solo para literales tuyos.** Es la distinción más importante de la librería.
2. **Pasa siempre tu propio `MlErrorDetails` en español** cuando el mensaje pueda llegar al usuario. Los mensajes por defecto están en inglés y son técnicos.
3. **Pon los value objects en las firmas públicas**, no los primitivos. Ahí es donde el compilador empieza a trabajar para ti.
4. **No revalides dentro del método** lo que el tipo del parámetro ya garantiza.
5. **Recuerda que los máximos y los rangos son exclusivos**; ajusta el límite en uno.
6. **Encadena con `Bind`** cuando una validación dependa de la anterior, y usa el último `Map` para construir el agregado.
7. **Nombra los elementos de las tuplas** en las conversiones paramétricas.
8. **No construyas los VO con `new`**: los constructores son `protected` o `private` y lanzan. Usa `From…` o `CreateFrom…`.
9. **Si necesitas un umbral distinto de cero**, usa el tipo paramétrico (`IntMoreThan`, `DecimalMoreThan`, `DoubleMoreThan`): los límites de los tipos `NonNegative` son `private const` y no se pueden cambiar.
10. **Prefiere `StringAsInt` frente a `StringAsNumeric<int>`**: el concreto usa `TryParse`, el genérico usa `Convert.ChangeType` y lanza.
11. **`IsValid` es tu amigo para consultas**: si solo quieres saber si algo sería válido, no construyas el objeto.
12. **Usa `EmptyValue` / `Void`** en lugar de `MlResult<bool>` para operaciones que no devuelven dato: expresan mejor la intención.

---

## Resumen

`MoralesLarios.OOFP.ValueObjects` traslada la validación **del código al sistema de tipos**:

- **Dos rutas de creación**: `CreateFrom…` (lanza, para literales) y `From…` (devuelve `MlResult<T>`, para datos externos).
- **Igualdad estructural** heredada de `ValueObject`, con comparación por valores atómicos y exigencia de tipo exacto.
- **Composición por herencia**: los VO de texto con longitud se construyen sobre `NotEmptyString`, así que la ausencia de vacíos está garantizada por el tipo.
- **Familias**: texto (9 tipos), enteros (6), decimales y dobles (9), parseo desde texto (12), especiales (2).
- **Conversiones implícitas** hacia el primitivo (siempre seguras) y desde el primitivo o desde tuplas (pueden lanzar).
- ✅ **Corregido en 2.0.0**: la condición invertida de `DecimalNonNegative`/`DoubleNonNegative` (ya se pueden construir) y los límites públicos mutables (ahora `private const`). Los constructores de `Age`, `Id` e `IdLite` han pasado a `protected`.
- ⚠️ **Puntos a vigilar**: los máximos y rangos son exclusivos, los mensajes por defecto están en inglés y `EnumRangeValueObject` sigue comentado (inexistente).

Esta librería es la base de [`ValueObjects.FileSystem`](../MoralesLarios.OOFP.ValueObjects.FileSystem/README.md) (rutas y ficheros), aporta `IntNonNegative` a [`Shared`](../MoralesLarios.OOFP.Shared/README.md) (paginación) y se combina de forma natural con [`Validation`](../MoralesLarios.OOFP.Validation/README.md).

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md)
- [Introducción general al núcleo funcional](../docs/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects.FileSystem`](../MoralesLarios.OOFP.ValueObjects.FileSystem/README.md) — value objects de rutas y ficheros (`FilePath`, `DirectoryPath`)
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación funcional con `IMlSelfValidatable<T>`
- [`MoralesLarios.OOFP.Validation.DataAnnotations`](../MoralesLarios.OOFP.Validation.DataAnnotations/README.md) — validación por atributos
- [`MoralesLarios.OOFP.Validation.FluentValidation`](../MoralesLarios.OOFP.Validation.FluentValidation/README.md) — integración con FluentValidation
- [`MoralesLarios.OOFP.Shared`](../MoralesLarios.OOFP.Shared/README.md) — usa `IntNonNegative` en la paginación

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../docs/core-types/MlResult.md)
- [`MlErrorDetails` — errores y detalles](../docs/core-types/MlResultErrors.md)
- [`EnsureFp` — el motor de validación que usan todos los `From…`](../docs/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar validaciones dependientes](../docs/Bind/3_Bind.md)
- [`Map` — construir el objeto final](../docs/Map/1_Map.md)
- [`Match` — salir del mundo `MlResult`](../docs/Match/1_Match.md)
