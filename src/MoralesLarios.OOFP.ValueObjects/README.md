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
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) (núcleo) | `MlResult<T>`, `MlErrorsDetails`, [`EnsureFp`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md), `Bind`, `Map` |
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
| `From…(valor)` | El VO directamente | **Lanza excepción** (`ArgumentNullException` / `ArgumentOutOfRangeException`) | Constantes del código, datos que ya sabes válidos, tests |
| `By…(valor)` | `MlResult<VO>` | **Devuelve un fallo**, no lanza | Entradas externas: API, formularios, ficheros, base de datos |
| Conversión implícita | El VO directamente | **Lanza excepción** (llama al constructor) | Literales en código propio |

```csharp
// From… → lanza si falla. Úsalo con literales que controlas.
Mail soporte = Mail.FromString("soporte@empresa.com");

// By… → nunca lanza. Úsalo con datos que vienen de fuera.
MlResult<Mail> resultado = Mail.ByString(entradaDelUsuario);

resultado.Match(
    valid: mail    => EnviarCorreo(mail),
    fail : errores => MostrarError(errores.ToErrorsMessages()));

// Conversión implícita → azúcar sintáctico, pero lanza igual que From…
Mail directo = "otro@empresa.com";
```

> ⚠️ **Regla práctica**: **todo dato que cruce el borde de la aplicación se construye con `By…`**. Reserva `From…` y las conversiones implícitas para valores literales escritos por ti.

### `By…` no siempre se llama igual

El nombre del método `By…` depende del tipo primitivo de origen. Merece la pena tenerlo a mano:

| VO | Creación segura | Creación directa |
|---|---|---|
| `NotEmptyString`, `Name`, `Key`, `Mail`, `Endpoint` | `ByString(value, errorsDetails?)` | `FromString(value)` |
| `RegexValue` | `ByRegex(value, pattern, errorsDetails?)` | `FromRegex(value, pattern)` |
| `StringMinLength`, `StringMaxLength` | `ByStringLength(value, length, errorsDetails?)` | `FromStringLenght(value, lenght)` |
| `StringBetweenLength` | `ByStringLength(value, min, max, errorsDetails?)` | `FromStringLenght(value, min, max)` |
| `IntMoreThan`, `IntLessThan` | `ByIntLength(value, lenght, errorsDetails?)` | `FromIntLenght(value, lenght)` |
| `IntBetween` | `ByIntLength(value, min, max, errorsDetails?)` | `FromIntLenght(value, min, max)` |
| `IntNotNegative`, `Age`, `IdLite` | `ByInt(value, errorsDetails?)` | `FromInt(value)` |
| `DecimalMoreThan`, `DecimalLessThan` | `ByDecimalLength(value, length, errorsDetails?)` | `FromDecimalLength(value, length)` |
| `DecimalBetween` | `ByDecimalLength(value, min, max, errorsDetails?)` | `FromDecimalLength(value, min, max)` |
| `DecimalNotNegative` | `ByDecimal(value, errorsDetails?)` | `FromDecimal(value)` |
| `DoubleMoreThan`, `DoubleLessThan` | `ByDoubleLength(value, length, errorsDetails?)` | `FromDoubleLength(value, length)` |
| `DoubleBetween` | `ByDoubleLength(value, min, max, errorsDetails?)` | `FromDoubleLength(value, min, max)` |
| `DoubleNotNegative` | `ByDouble(value, errorsDetails?)` | `FromDouble(value)` |
| `Id` | `Bydouble(value, errorsDetails?)` | `Fromdouble(value)` |
| `StringAsInt`, `StringAsLong`, … | `ByString(value)` *(sin `errorsDetails`)* | `FromString(value)` |
| `Empty`, `Void` | — | `Create()` |

> ⚠️ Ojo con los nombres: `Id` usa **`Bydouble` / `Fromdouble`** en minúscula (así está en el código fuente), y `FromIntLenght` / `FromStringLenght` llevan la errata *"Lenght"*. Son los nombres reales, hay que escribirlos tal cual.

### Personalizar el mensaje de error

Casi todos los `By…` aceptan un `MlErrorsDetails` opcional que **sustituye** al mensaje por defecto (que está en inglés):

```csharp
// Mensaje por defecto: "usuario@ is not a valid mail"
var r1 = Mail.ByString("usuario@");

// Mensaje propio, en español y con contexto de negocio
var r2 = Mail.ByString("usuario@", "El correo del destinatario no tiene un formato válido");
```

> 💡 `MlErrorsDetails` tiene conversión implícita desde `string`, por eso puedes pasar el texto directamente sin construir nada.

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

- `Equals` exige **el mismo tipo exacto** (`obj.GetType() != GetType()` → `false`). Un `Key` nunca será igual a un `Name`, aunque el texto coincida.
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
│   └── StringBetweenLength
├── ValueObject<int>
│   ├── IntMoreThan → IntNotNegative, Age, IdLite
│   ├── IntLessThan
│   └── IntBetween
├── ValueObject<decimal>
│   ├── DecimalMoreThan → DecimalNotNegative
│   ├── DecimalLessThan
│   └── DecimalBetween
├── ValueObject<double>
│   ├── DoubleMoreThan → DoubleNotNegative, Id
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
| `BuildErrorMessage(...)` | `static string` | Mensaje de error por defecto (en inglés). |
| `From…(...)` | `static VO` | Construcción directa. **Lanza** si es inválido. |
| `By…(..., MlErrorsDetails? )` | `static MlResult<VO>` | Construcción funcional. **No lanza.** |
| `implicit operator TPrimitivo(VO)` | conversión | Sacar el valor: `string texto = miMail;` |
| `implicit operator VO(TPrimitivo)` | conversión | Meter el valor: `Mail m = "a@b.com";` (lanza si es inválido) |

Y la implementación de `By…` es siempre una tubería funcional del núcleo:

```csharp
// Caso simple (numéricos): arranca de MlResult.Empty()
public static MlResult<IntNotNegative> ByInt(int value, MlErrorsDetails errorsDetails = null!)
    => MlResult.Empty()
               .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
               .Map (_ => new IntNotNegative(value));

// Caso compuesto (texto): valida primero el VO padre y luego la regla propia
public static MlResult<Mail> ByString(string value, MlErrorsDetails errorsDetails = null!)
    => NotEmptyString.ByString(value)
                     .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
                     .Map (_ => new Mail(value));
```

> 💡 Fíjate en el `Bind` del segundo caso: si el texto viene vacío, **el error es "cannot be null or empty"**, no "is not a valid mail". El cortocircuito de [`Bind`](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) hace que el mensaje sea siempre el de la primera regla incumplida, que es el más informativo.

---

## Catálogo completo por familias

### Texto

| Tipo | Regla | Notas |
|---|---|---|
| `NotEmptyString` | `! string.IsNullOrWhiteSpace(value)` | La base de toda la familia de texto |
| `StringMinLength` | `value.Length >= length` | Mínimo **inclusivo** |
| `StringMaxLength` | `value.Length < length` | Máximo **exclusivo** ⚠️ |
| `StringBetweenLength` | `Length > min && Length < max` | **Ambos extremos exclusivos** ⚠️ |
| `Key` | `StringMinLength` con `MinLenght = 1` | Claves de caché, identificadores textuales |
| `Name` | `StringMinLength` con `MinLenght = 3` | Nombres de persona, etiquetas |
| `RegexValue` | `Regex.IsMatch(value, pattern)` | Expone `Pattern`; base de `Mail` y `Endpoint` |
| `Mail` | Patrón `^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$` | Constante pública `Mail.EndpointPattern` |
| `Endpoint` | Patrón `^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$` | **Solo `https`**, host + puerto opcional, **sin ruta** |

### Enteros

| Tipo | Regla | Notas |
|---|---|---|
| `IntMoreThan` | `value > length` | Estricto |
| `IntLessThan` | `value < length` | Estricto |
| `IntBetween` | `value > min && value < max` | Ambos exclusivos |
| `IntNotNegative` | `value >= limit` (con `limit = 0`) | El único de la familia `NotNegative` que funciona bien ⚠️ |
| `Age` | `value > -1` y además el ctor rechaza `< 0` | Edad, cero permitido |
| `IdLite` | `value > 0` | Identificador entero, cero no permitido |

### Decimales y dobles

| Tipo | Regla | Notas |
|---|---|---|
| `DecimalMoreThan` / `DoubleMoreThan` | `value > length` | Estricto |
| `DecimalLessThan` / `DoubleLessThan` | `value < length` | Estricto |
| `DecimalBetween` / `DoubleBetween` | `value > min && value < max` | Ambos exclusivos |
| `DecimalNotNegative` / `DoubleNotNegative` | `value < limit` (con `limit = 0`) | ⚠️ **La condición está invertida** — ver [particularidades](#️-particularidades-reales-del-código-fuente) |
| `Id` | `value > 0` (sobre `double`) | Métodos `Bydouble` / `Fromdouble` |

### Parseo desde texto

`StringAsNumeric<TValue>` es la base genérica; los concretos son:

`StringAsByte` · `StringAsSByte` · `StringAsShort` · `StringAsUShort` · `StringAsInt` · `StringAsUInt` · `StringAsLong` · `StringAsULong` · `StringAsFloat` · `StringAsDouble` · `StringAsDecimal`

Sirven para **cruzar la frontera texto → número una sola vez y de forma segura**:

```csharp
MlResult<StringAsInt> cantidad = StringAsInt.ByString(Request.Query["cantidad"]);

int valor = cantidad.Match(valid: c => (int)c, fail: _ => 0);
```

Los concretos tienen doble conversión de salida: **al número** (`implicit operator int`) y **al texto** (`implicit operator string`, que hace `Value.ToString()`).

> ⚠️ La entrada se convierte con `int.TryParse` en los tipos concretos, pero `StringAsNumeric<TValue>` genérico usa `Convert.ChangeType`, que **lanza** en vez de devolver `false`. Por eso su `ByString` usa `TryBind` en lugar de `Bind`. Prefiere siempre los tipos concretos (`StringAsInt`) al genérico.

### Especiales

| Tipo | Creación | Para qué |
|---|---|---|
| `Empty` | `Empty.Create()`, `Empty.CreateAsync()` | Marcador de "operación sin valor de retorno" en tuberías `MlResult<Empty>` |
| `Void` | `Void.Create()` | Equivalente a `Empty`, sin variante asíncrona |

Ambos envuelven `string.Empty` y **ignoran cualquier valor que se les pase**. Son señales, no datos.

---

## Conversiones implícitas y explícitas

### Salida (VO → primitivo): siempre segura

```csharp
Mail   mail  = Mail.FromString("a@b.com");
string texto = mail;                          // implícita, nunca falla

Age    edad  = Age.FromInt(30);
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
IntBetween           nota     = (value: 7,        minLenght: 0,  maxLenght: 11);
StringMinLength      titulo   = (value: "Hola",   length: 3);
StringBetweenLength  alias    = (value: "pepe",   minLenght: 2,  maxLenght: 20);
RegexValue           codigo   = (value: "AB-123", pattern: @"^[A-Z]{2}-\d{3}$");
```

> 💡 **Nombra los elementos de la tupla.** Con dos o tres `int` seguidos, invertir el orden es demasiado fácil y el compilador no puede ayudarte.

---

## ⚠️ Particularidades reales del código fuente

Observaciones sobre el código tal y como está hoy. **Conviene conocerlas antes de confiar en un tipo.**

### 1. 🐛 `DecimalNotNegative` y `DoubleNotNegative` tienen la condición invertida

```csharp
// DecimalNotNegative.cs
public static decimal limit { get; private set; } = 0m;
public static bool IsValid(decimal value) => value < limit;   // ← "menor que 0"
```

El nombre promete *"no negativo"*, pero la validación acepta **solo los negativos**. Además, al llamar al constructor base `DecimalMoreThan(value, 0)`, este exige `value > 0`, así que **las dos condiciones son incompatibles**: cualquier valor que pase una fallará la otra.

> ⚠️ **En la práctica, `DecimalNotNegative` y `DoubleNotNegative` no se pueden construir con éxito.** Hasta que se corrijan, usa `DecimalMoreThan.ByDecimalLength(value, -1)` o valida a mano con `EnsureFp.That(value, value >= 0, "…")`.
>
> `IntNotNegative` **sí funciona** correctamente (`value >= limit` con `limit = 0`).

### 2. ⚠️ `IntNotNegative.limit` es un campo público mutable

```csharp
public static int limit;      // sin readonly, sin private set
```

Cualquier código puede escribir `IntNotNegative.limit = 100;` y **cambiar la validación de toda la aplicación en caliente**. No lo hagas. (En `DecimalNotNegative` y `DoubleNotNegative` sí es `{ get; private set; }`.)

### 3. Los límites de longitud son exclusivos (y no siempre simétricos)

| Tipo | Mínimo | Máximo |
|---|---|---|
| `StringMinLength` | **inclusivo** (`>=`) | — |
| `StringMaxLength` | — | **exclusivo** (`<`) |
| `StringBetweenLength` | **exclusivo** (`>`) | **exclusivo** (`<`) |

Es decir: `StringMaxLength.ByStringLength("abcde", 5)` **falla**, porque exige `Length < 5`. Si quieres "hasta 5 caracteres", pasa `6`.

Lo mismo con `IntBetween`: para el rango 1..10 inclusive hay que pedir `(value, 0, 11)`.

### 4. `StringBetweenLength.BuildErrorMessage` invierte los límites en el texto

```csharp
=> $"{value} must be between {maxLenght} and {minLenght}";   // ← max primero
```

El mensaje dirá *"must be between 20 and 2"*. Si el mensaje se muestra al usuario, **pasa tu propio `errorsDetails`**.

### 5. `Key.IsValid` y `Name.IsValid` ignoran el parámetro `length`

```csharp
public static new bool IsValid(string value, int length) => StringMinLength.IsValid(value, MinLenght);
```

Pasan su constante interna (`1` para `Key`, `3` para `Name`) y descartan lo que le pases. Es coherente con el propósito del tipo, pero la firma engaña: **el segundo argumento no tiene efecto**.

### 6. Los constructores de `Key` y `Name` son públicos

A diferencia del resto de la librería (donde el constructor es `protected` para forzar el uso de las fábricas), `Key` y `Name` se pueden instanciar con `new Key("x")`. Sigue siendo válido, pero por coherencia usa `FromString` / `ByString`.

### 7. Los mensajes de error por defecto están en inglés

`"{value} cannot be null or empty"`, `"{value} is not a valid mail"`, `"{value} must be More than 0"`… Si esos textos van a llegar al usuario final, **pasa siempre tu propio `MlErrorsDetails` en español**.

### 8. `RegexValue.Pattern` es una propiedad de instancia

Puedes preguntarle a un `RegexValue` con qué patrón se validó (`miValor.Pattern`), pero **`Mail` y `Endpoint` no lo exponen como estático**; para eso están las constantes `Mail.EndpointPattern` y `Endpoint.EndpointPattern` (nombre heredado, aunque en `Mail` valide correos).

### 9. `Endpoint` es más restrictivo de lo que parece

El patrón `^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$` **exige `https`** y **no admite ruta**:

```csharp
Endpoint.IsValid("https://api.empresa.com");        // ✅
Endpoint.IsValid("https://api.empresa.com:8443");   // ✅
Endpoint.IsValid("http://api.empresa.com");         // ❌ no es https
Endpoint.IsValid("https://api.empresa.com/v1");     // ❌ lleva ruta
Endpoint.IsValid("https://localhost:5001");         // ✅
```

---

## ⚠️ Lo que NO existe

> ⚠️ **`RangeEnumValueObject` no existe.** El fichero `RangeEnumValueObject.cs` está **íntegramente comentado**: el tipo no compila ni se puede usar. Si necesitas validar contra un `enum`, hazlo con `EnsureFp.That(texto, Enum.TryParse<TEnum>(texto, true, out _), "…")`.

> ⚠️ **No existe** una propiedad pública `Value` en los VO. `Value` es `protected`. Para obtener el primitivo usa **la conversión implícita** (`string s = miMail;`) o `ToString()`.

> ⚠️ **No existen** métodos `Validate()` de instancia, ni `TryCreate(out …)`, ni variantes `…Async` de los `By…` (salvo `Empty.CreateAsync()`). Tampoco hay conversión implícita de un VO a `MlResult<VO>`.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Ejemplos prácticos

### Ejemplo 1 — Un modelo de dominio a prueba de balas

```csharp
using MoralesLarios.OOFP.ValueObjects;

public record Usuario(IdLite Id, Name Nombre, Mail Correo, Age Edad);

// Construcción segura: cada VO valida su parte y los errores se acumulan por cortocircuito
public static MlResult<Usuario> CrearUsuario(int id, string nombre, string correo, int edad)
    => IdLite.ByInt(id, "El identificador debe ser mayor que 0")
             .Bind(idVo => Name.ByString(nombre, "El nombre debe tener al menos 3 caracteres")
             .Bind(nomVo => Mail.ByString(correo, "El correo no tiene un formato válido")
             .Bind(mailVo => Age.ByInt(edad, "La edad no puede ser negativa")
             .Map (edadVo => new Usuario(idVo, nomVo, mailVo, edadVo)))));

// Uso
var resultado = CrearUsuario(1, "Ana", "ana@empresa.com", 34);

resultado.Match(
    valid: u       => Console.WriteLine($"Creado: {u.Nombre} <{u.Correo}>, {u.Edad} años"),
    fail : errores => Console.WriteLine($"No se pudo crear: {errores.ToErrorsDescription()}"));
```

> 💡 La interpolación `{u.Nombre}` funciona sin conversión explícita gracias al `ToString()` de `ValueObject<TValue>`.

### Ejemplo 2 — Validar la entrada de una API sin un solo `if`

```csharp
[HttpPost("suscripciones")]
public IActionResult Suscribir([FromBody] SuscripcionRequest request)
    => Mail.ByString(request.Email, "Indica un correo electrónico válido")
           .Bind(mail => Name.ByString(request.Nombre, "El nombre debe tener al menos 3 caracteres")
                             .Map(nombre => (mail, nombre)))
           .Bind(x => _servicio.Suscribir(x.mail, x.nombre))
           .Match(
               valid: _       => Ok(new { mensaje = "Suscripción registrada" }),
               fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
```

### Ejemplo 3 — Convertir texto en número de forma segura

Típico al leer configuración, parámetros de query o ficheros CSV.

```csharp
public static MlResult<int> LeerPuerto(string textoPuerto)
    => StringAsInt.ByString(textoPuerto)
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

cache[Key.FromString("usuarios:activos")] = "…json…";

// Otra instancia distinta, pero igual en valor → encuentra la entrada
bool existe = cache.ContainsKey(Key.FromString("usuarios:activos"));   // true

// Y la igualdad exige el mismo tipo exacto:
Key  k = Key.FromString("hola");
Name n = Name.FromString("hola");
bool iguales = k.Equals(n);      // false, aunque el texto coincida
```

### Ejemplo 5 — Endpoint validado para un cliente HTTP

```csharp
public static MlResult<HttpClient> CrearCliente(string urlBase)
    => Endpoint.ByString(urlBase, $"'{urlBase}' no es un endpoint https válido (host y puerto, sin ruta)")
               .Map(endpoint => new HttpClient { BaseAddress = new Uri(endpoint) });

// La conversión implícita Endpoint → string permite pasarlo directo a Uri.
```

### Ejemplo 6 — ❌ Qué no hacer / ✅ qué hacer

```csharp
// ❌ MAL: usar From… con datos externos → excepción no controlada
var mail = Mail.FromString(Request.Form["email"]);        // 💥 si el usuario escribe cualquier cosa

// ✅ BIEN: By… devuelve MlResult y el error viaja por el canal normal
var mail = Mail.ByString(Request.Form["email"], "Correo no válido");


// ❌ MAL: volver a validar lo que el tipo ya garantiza
void Enviar(Mail destino)
{
    if (! destino.ToString()!.Contains('@')) throw new ArgumentException();   // imposible
}

// ✅ BIEN: confiar en el tipo
void Enviar(Mail destino) { /* … */ }


// ❌ MAL: dejar el mensaje en inglés llegar al usuario final
return BadRequest(Name.ByString(x).Match(valid: _ => "", fail: e => e.ToErrorsMessages()));
//     → "ab cannot be less than 3 characters"

// ✅ BIEN: mensaje propio en español
return BadRequest(Name.ByString(x, "El nombre debe tener al menos 3 caracteres")
                      .Match(valid: _ => "", fail: e => e.ToErrorsMessages()));


// ❌ MAL: asumir que los rangos son inclusivos
var titulo = StringMaxLength.ByStringLength("12345", 5);      // FALLA: exige Length < 5

// ✅ BIEN: recordar que el máximo es exclusivo
var titulo = StringMaxLength.ByStringLength("12345", 6);


// ❌ MAL: contar con DecimalNotNegative / DoubleNotNegative
var importe = DecimalNotNegative.ByDecimal(10m);              // no se puede construir (bug conocido)

// ✅ BIEN: usar el tipo paramétrico o EnsureFp
var importe = DecimalMoreThan.ByDecimalLength(10m, -1m);
// o bien
var importe2 = EnsureFp.That(10m, 10m >= 0m, "El importe no puede ser negativo");


// ❌ MAL: modificar el límite estático global
IntNotNegative.limit = 10;      // cambia la validación de TODA la aplicación

// ✅ BIEN: usar el tipo paramétrico para límites propios
var vo = IntMoreThan.ByIntLength(valor, 9);


// ❌ MAL: buscar RangeEnumValueObject
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
| Nombre de persona / etiqueta legible | `Name` (mínimo 3 caracteres) |
| Clave de caché o identificador textual | `Key` (mínimo 1 carácter) |
| Correo electrónico | `Mail` |
| URL base `https` sin ruta | `Endpoint` |
| Cualquier otro formato de texto | `RegexValue` con tu patrón |
| Longitud mínima / máxima / rango | `StringMinLength` / `StringMaxLength` / `StringBetweenLength` |
| Entero no negativo (cero incluido) | `IntNotNegative` |
| Entero mayor que cero (identificador) | `IdLite` |
| Edad | `Age` |
| Identificador numérico `double` | `Id` |
| Entero con límite propio | `IntMoreThan` / `IntLessThan` / `IntBetween` |
| Importe monetario con límite | `DecimalMoreThan` / `DecimalBetween` ⚠️ (evita `DecimalNotNegative`) |
| Convertir texto de entrada en número | `StringAsInt`, `StringAsDecimal`, … |
| Señalar "sin valor de retorno" en una tubería | `Empty` / `Void` |
| Validar contra un `enum` | `EnsureFp.That(…, Enum.TryParse<T>(…), …)` |

---

## Mejores prácticas

1. **Usa `By…` para todo dato externo y `From…` solo para literales tuyos.** Es la distinción más importante de la librería.
2. **Pasa siempre tu propio `MlErrorsDetails` en español** cuando el mensaje pueda llegar al usuario. Los mensajes por defecto están en inglés y son técnicos.
3. **Pon los value objects en las firmas públicas**, no los primitivos. Ahí es donde el compilador empieza a trabajar para ti.
4. **No revalides dentro del método** lo que el tipo del parámetro ya garantiza.
5. **Recuerda que los máximos y los rangos son exclusivos**; ajusta el límite en uno.
6. **Encadena con `Bind`** cuando una validación dependa de la anterior, y usa el último `Map` para construir el agregado.
7. **Nombra los elementos de las tuplas** en las conversiones paramétricas.
8. **No toques `IntNotNegative.limit`**: es estado global mutable.
9. **Evita `DecimalNotNegative` y `DoubleNotNegative`** hasta que se corrija la condición invertida.
10. **Prefiere `StringAsInt` frente a `StringAsNumeric<int>`**: el concreto usa `TryParse`, el genérico usa `Convert.ChangeType` y lanza.
11. **`IsValid` es tu amigo para consultas**: si solo quieres saber si algo sería válido, no construyas el objeto.
12. **Usa `Empty` / `Void`** en lugar de `MlResult<bool>` para operaciones que no devuelven dato: expresan mejor la intención.

---

## Resumen

`MoralesLarios.OOFP.ValueObjects` traslada la validación **del código al sistema de tipos**:

- **Dos rutas de creación**: `From…` (lanza, para literales) y `By…` (devuelve `MlResult<T>`, para datos externos).
- **Igualdad estructural** heredada de `ValueObject`, con comparación por valores atómicos y exigencia de tipo exacto.
- **Composición por herencia**: los VO de texto con longitud se construyen sobre `NotEmptyString`, así que la ausencia de vacíos está garantizada por el tipo.
- **Familias**: texto (9 tipos), enteros (6), decimales y dobles (9), parseo desde texto (12), especiales (2).
- **Conversiones implícitas** hacia el primitivo (siempre seguras) y desde el primitivo o desde tuplas (pueden lanzar).
- ⚠️ **Puntos a vigilar**: la condición invertida de `DecimalNotNegative`/`DoubleNotNegative`, los máximos exclusivos, el `limit` público mutable de `IntNotNegative`, los mensajes en inglés y `RangeEnumValueObject` (comentado, inexistente).

Esta librería es la base de [`ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) (rutas y ficheros), aporta `IntNotNegative` a [`Internals`](../MoralesLarios.OOFP.Internals/README.md) (paginación) y se combina de forma natural con [`Validation`](../MoralesLarios.OOFP.Validation/README.md).

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.ValueObjects.IO`](../MoralesLarios.OOFP.ValueObjects.IO/README.md) — value objects de rutas y ficheros (`MlFile`, `MlDirectory`)
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validación funcional con `MlValidableFp<T>`
- [`MoralesLarios.OOFP.Validation.Dataannotations`](../MoralesLarios.OOFP.Validation.Dataannotations/README.md) — validación por atributos
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — integración con FluentValidation
- [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) — usa `IntNotNegative` en la paginación

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — errores y detalles](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`EnsureFp` — el motor de validación que usan todos los `By…`](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [`Bind` — encadenar validaciones dependientes](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` — construir el objeto final](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — salir del mundo `MlResult`](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
