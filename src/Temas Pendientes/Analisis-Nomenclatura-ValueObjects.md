# Análisis de nomenclatura — `MoralesLarios.OOFP.ValueObjects`

> 📌 **Qué es este documento**
> Revisión **nombre a nombre** de los 30 tipos públicos del proyecto `MoralesLarios.OOFP.ValueObjects`,
> con el problema detectado, el nombre propuesto y alternativas.
> Complementa a [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md), que cubre el resto de la solución.
>
> **Alcance:** solo nomenclatura. Los bugs de comportamiento de estos tipos están en
> [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md).
>
> **Aviso:** todos los renombrados de tipos y miembros públicos son **cambios de ruptura**
> y exigen subir versión **mayor** (SemVer). Ver §12 para la estrategia de migración.

---

## Índice

- [1. Problemas transversales](#1-problemas-transversales)
- [2. Infraestructura base](#2-infraestructura-base)
- [3. Tipos «vacío» (duplicados)](#3-tipos-vacío-duplicados)
- [4. Cadenas — invariantes de longitud](#4-cadenas--invariantes-de-longitud)
- [5. Cadenas — de dominio](#5-cadenas--de-dominio)
- [6. Numéricos — comparaciones](#6-numéricos--comparaciones)
- [7. Numéricos — de dominio](#7-numéricos--de-dominio)
- [8. Cadenas numéricas (`StringAs*`)](#8-cadenas-numéricas-stringas)
- [9. Enumerados](#9-enumerados)
- [10. Convenciones de miembros](#10-convenciones-de-miembros)
- [11. Resumen ejecutivo — top 10](#11-resumen-ejecutivo--top-10)
- [12. Estrategia de migración](#12-estrategia-de-migración)
- [13. Checklist de seguimiento](#13-checklist-de-seguimiento)

---

## 1. Problemas transversales

| # | Problema | Evidencia | Impacto |
|---|---|---|---|
| 1 | **Errata sistemática `Lenght` → `Length`** | `FromIntLenght`, `minLenght`, `maxLenght`, `MinLenght`, `$"...{lenght}..."` | Muy alto: está en la API pública y en mensajes de error visibles al usuario |
| 2 | **`length` usado para umbrales numéricos** | `IntMoreThan.IsValid(int value, int length)`, `IntBetween(value, minLenght, maxLenght)` | Semánticamente falso: no es una longitud, es un *límite / umbral* |
| 3 | **`MoreThan` no es idiomático en .NET** | `IntMoreThan`, `DecimalMoreThan`, `DoubleMoreThan` | La BCL usa `GreaterThan` (`Comparer`, `Expression.GreaterThan`) |
| 4 | **Orden del adjetivo inconsistente** | `NotEmptyString` (adjetivo delante) vs `IntNotNegative` (adjetivo detrás) | Impredecible al autocompletar |
| 5 | **Colisiones con la BCL / frameworks** | `Void`, `Empty`, `Key`, `Name`, `Endpoint`, `Id` | Obliga a `using` calificados; `Endpoint` choca con `Microsoft.AspNetCore.Http.Endpoint` y `System.Net.EndPoint` |
| 6 | **Prefijo `StringAs*`** | 11 tipos + base | `As*` es convención de **método** (`AsSpan`, `AsEnumerable`), no de tipo. Un tipo debe nombrar el *invariante*, no la conversión |
| 7 | **Alias del lenguaje mezclados con nombres CLR** | `StringAsInt`, `StringAsShort`, `StringAsUShort`, `StringAsFloat` | La guía de MS usa el nombre CLR en identificadores (`Int32`, `Int16`, `UInt16`, `Single`) |
| 8 | **Sufijo `ValueObject` redundante** | `RangeEnumValueObject<TEnum>` | Todos los tipos del ensamblado son value objects |
| 9 | **Verbos de factoría inconsistentes** | `From*` / `By*` / `Create()` conviviendo | `By*` no significa nada en .NET; `Create()` solo en `Void` / `Empty` |
| 10 | **Copy-paste con nombres roto** | `IdLite.BuildErrorMessIdLite`, `Mail.EndpointPattern`, `Id.Fromdouble` / `Id.Bydouble` | Ruido evidente en API pública |
| 11 | **`FOOP` vs `OOFP`** | Carpeta `MoralesLarios.FOOP` frente a namespace / `.sln` / `.csproj` `MoralesLarios.OOFP` | Confunde en rutas, búsquedas y documentación. Fijar **uno** |

---

## 2. Infraestructura base

| Actual | Diagnóstico | Propuesta | Alternativas |
|---|---|---|---|
| `ValueObject` | Correcto y estándar (DDD) | **Mantener** | `ValueObjectBase` (si se quiere señalar que es abstracta) |
| `ValueObject<TValue>` | Correcto, pero **no es abstracta**: se puede instanciar sin invariantes | **`SingleValueObject<TValue>`** | `WrappedValue<TValue>`, `ValueWrapper<TValue>` |

> Renombrar el genérico libera el nombre `ValueObject<T>` y describe mejor «VO de un único valor».

---

## 3. Tipos «vacío» (duplicados)

`Void` y `Empty` son **el mismo tipo con dos nombres**: ambos derivan de `ValueObject<string>`,
ambos ignoran el parámetro del constructor y ambos guardan `string.Empty`.

| Actual | Problema | Propuesta | Alternativas |
|---|---|---|---|
| `Void` | Choca con `System.Void`; en C# `void` es «sin retorno», no «un valor vacío» | **`Unit`** (nombre canónico en FP: F#, Rx, LanguageExt) | `Nothing`, `NoValue` |
| `Empty` | Choca con `Enumerable.Empty`, `string.Empty`, `Array.Empty`; además `Empty` es un *adjetivo*, mal nombre de tipo | **Eliminar y fusionar en `Unit`** | Si hay que conservar dos: `Unit` + `NoValue` |

**Recomendación:** un solo tipo `Unit` con `Unit.Value` (singleton) en lugar de `Create()`,
que sugiere falsamente que se crean instancias distintas.

---

## 4. Cadenas — invariantes de longitud

| Actual | Problema | Propuesta | Alternativas |
|---|---|---|---|
| `NotEmptyString` | Orden de adjetivo inconsistente con el resto; además valida `IsNullOrWhiteSpace`, es decir *no en blanco*, no *no vacío* | **`NonBlankString`** | `NonEmptyString`, `RequiredString`, `FilledString` |
| `StringMinLength` | Nombre «de configuración», no de invariante | **`MinLengthString`** | `StringWithMinLength`, `AtLeastLengthString` |
| `StringMaxLength` | Igual. Y valida `< length` (**exclusivo**) mientras `StringMinLength` usa `>=` (**inclusivo**): el nombre no revela la asimetría | **`MaxLengthString`** | `StringWithMaxLength`, `BoundedLengthString` |
| `StringBetweenLength` | Gramaticalmente invertido: «between» cualifica a *length*, no a *string* | **`StringLengthBetween`** | `LengthRangeString`, `StringWithinLengthRange`, `BoundedString` |

> ⚠️ **Nota de diseño ligada al nombre:** si `StringMaxLength` fuese inclusivo como `StringMinLength`,
> los tres nombres serían coherentes entre sí. Hoy el nombre miente sobre el límite.

---

## 5. Cadenas — de dominio

| Actual | Problema | Propuesta | Alternativas |
|---|---|---|---|
| `Key` | Extremadamente genérico; choca con `KeyValuePair.Key`, `IDictionary` y `[Key]` de EF Core. No dice *qué* clave es (min length 1) | **`EntityKey`** | `IdentifierKey`, `KeyString`, `ShortCode`, `Slug` |
| `Name` | Genérico; choca con `MemberInfo.Name`, `Type.Name`, `[Display(Name=)]`. Min length 3 sin explicar por qué | **`DisplayName`** | `EntityName`, `PersonName`, `NameString`, `Label` |
| `RegexValue` | `Value` es ruido (todo es un valor). No dice que sea una *cadena* | **`RegexString`** | `PatternMatchedString`, `MatchingString`, `RegexConstrainedString` |
| `Endpoint` | Colisión grave con ASP.NET Core y `System.Net`. Además el patrón solo acepta `https://host[:puerto]`, **sin ruta**: no es un «endpoint» | **`HttpsUrl`** | `BaseUrl`, `ServiceUrl`, `SecureHostUrl`, `HttpsAuthority` |
| `Mail` | `Mail` es el *mensaje*; lo que valida es la *dirección*. Estándar: `EmailAddress` (cf. `[EmailAddress]`, `MailAddress`) | **`EmailAddress`** | `Email`, `MailAddressValue` |

**Erratas asociadas:** en `Mail` la constante se llama `EndpointPattern` (copy-paste desde `Endpoint`);
debería ser `EmailPattern`. En `Endpoint`, `HttpsUrlPattern`.

---

## 6. Numéricos — comparaciones

El patrón `<Tipo><Comparación>` es aceptable y **agrupa bien por tipo al autocompletar**,
pero usa un verbo no idiomático y un parámetro mal nombrado.

| Actual | Propuesta | Alternativas | Renombrado del parámetro |
|---|---|---|---|
| `IntMoreThan` | **`IntGreaterThan`** | `Int32GreaterThan`, `IntAbove` | `length` → **`exclusiveMinimum`** / `threshold` |
| `IntLessThan` | *(ya correcto)* | `Int32LessThan`, `IntBelow` | `length` → **`exclusiveMaximum`** |
| `IntBetween` | **`IntInRange`** | `Int32Between`, `BoundedInt`, `IntRange` | `minLenght` / `maxLenght` → **`min`** / **`max`** |
| `IntNotNegative` | **`NonNegativeInt`** | `Int32NonNegative`, `ZeroOrGreaterInt` | — |
| `DecimalMoreThan` | **`DecimalGreaterThan`** | `DecimalAbove` | ídem `Int` |
| `DecimalLessThan` | *(ya correcto)* | `DecimalBelow` | ídem |
| `DecimalBetween` | **`DecimalInRange`** | `BoundedDecimal` | ídem |
| `DecimalNotNegative` | **`NonNegativeDecimal`** | `ZeroOrGreaterDecimal` | — |
| `DoubleMoreThan` | **`DoubleGreaterThan`** | `DoubleAbove` | ídem |
| `DoubleLessThan` | *(ya correcto)* | `DoubleBelow` | ídem |
| `DoubleBetween` | **`DoubleInRange`** | `BoundedDouble` | ídem |
| `DoubleNotNegative` | **`NonNegativeDouble`** | `ZeroOrGreaterDouble` | — |

### Alternativa de mayor calado (recomendada a medio plazo)

Colapsar los **12 tipos en 4 genéricos**, ahora que existe `INumber<T>` en .NET 8:

```csharp
public class GreaterThan<T> : SingleValueObject<T> where T : INumber<T> { }
public class LessThan<T>    : SingleValueObject<T> where T : INumber<T> { }
public class InRange<T>     : SingleValueObject<T> where T : INumber<T> { }
public class NonNegative<T> : SingleValueObject<T> where T : INumber<T> { }
```

Uso: `NonNegative<decimal>`, `InRange<int>`.
Elimina duplicación y hace desaparecer la trampa de `base(value, limit - 1)`
(el desplazamiento que hoy hace falta porque la base compara con `>` estricto).

---

## 7. Numéricos — de dominio

| Actual | Problema | Propuesta | Alternativas |
|---|---|---|---|
| `Id` | Es `double`: **un identificador nunca debe ser de coma flotante**; el nombre no advierte del tipo subyacente. Choca con `[Key] Id`, `IIdentifiable.Id` | **`Int64Id`** (cambiando a `long`) | `NumericId`, `EntityId`, `Identifier`, `Id<T>` |
| `IdLite` | **`Lite` no significa nada** (¿menos válido? ¿más rápido?). En realidad es «id de 32 bits» | **`Int32Id`** | `IntId`, `ShortId`, `SmallId` |
| `Age` | Correcto en general, pero ambiguo (¿años? ¿días?) y su `Limit => -1` es un truco | **`AgeInYears`** | `Age` (mantener), `PersonAge`, `YearsOld` |

> ⚠️ El par `Id` / `IdLite` es **el peor caso del proyecto**: los nombres no revelan la diferencia real
> (`double` vs `int`) y sugieren una jerarquía «completo vs reducido» que no existe.
> Alternativa unificada: **`Identifier<T>`** con `Identifier<int>` / `Identifier<long>` / `Identifier<Guid>`.

---

## 8. Cadenas numéricas (`StringAs*`)

La familia con más problemas: **12 tipos**, prefijo no idiomático y **44 warnings `CS0108`**
por ocultación de `IsValid` / `FromString` / `ByString` / `BuildErrorMessage`.

| Actual | Propuesta | Alternativas |
|---|---|---|
| `StringAsNumeric<TValue>` | **`NumericString<T>`** | `ParsableString<T>`, `NumberString<T>`, `StringNumber<T>` |
| `StringAsByte` | **`ByteString`** | `NumericString<byte>` |
| `StringAsSByte` | **`SByteString`** | `NumericString<sbyte>` |
| `StringAsShort` | **`Int16String`** | ~~`ShortString`~~ (¡ambiguo: parece «cadena corta»!) |
| `StringAsUShort` | **`UInt16String`** | `NumericString<ushort>` |
| `StringAsInt` | **`Int32String`** | `IntString` |
| `StringAsUInt` | **`UInt32String`** | `NumericString<uint>` |
| `StringAsLong` | **`Int64String`** | ~~`LongString`~~ (ambiguo: parece «cadena larga») |
| `StringAsULong` | **`UInt64String`** | `NumericString<ulong>` |
| `StringAsFloat` | **`SingleString`** | `FloatString` |
| `StringAsDouble` | **`DoubleString`** | `NumericString<double>` |
| `StringAsDecimal` | **`DecimalString`** | `NumericString<decimal>` |

**Observación clave:** `ShortString` y `LongString` serían **peores** que los nombres actuales
(se leen como longitud de la cadena). Ese es el argumento definitivo para usar los nombres CLR
(`Int16String`, `Int64String`).

**Alternativa recomendada:** eliminar los 11 tipos derivados y quedarse solo con `NumericString<T>`.
Los derivados **no añaden ningún invariante**: únicamente cierran el genérico, y al hacerlo generan
los 44 `CS0108`. Es un problema de *diseño* que se manifiesta como problema de *nomenclatura*.

---

## 9. Enumerados

| Actual | Problema | Propuesta | Alternativas |
|---|---|---|---|
| `RangeEnumValueObject<TEnum>` | Tres problemas a la vez: (a) `Range` es **engañoso** — no hay rango, sino pertenencia al conjunto de nombres; (b) sufijo `ValueObject` **redundante**; (c) tres palabras para un concepto simple | **`EnumName<TEnum>`** (guarda el `string` del nombre) | `EnumValue<TEnum>`, `EnumMember<TEnum>`, `ValidEnumName<TEnum>`, `KnownEnum<TEnum>` |

---

## 10. Convenciones de miembros

Afecta a **todos** los tipos del proyecto.

| Actual | Problema | Propuesta |
|---|---|---|
| `FromInt`, `FromString`, `Fromdouble`, `FromIntLenght` | El sufijo con el tipo es redundante (ya está en la firma), más erratas y *casing* incorrecto | **`From(...)`** o **`Create(...)`** — lanza excepción |
| `ByInt`, `ByString`, `Bydouble`, `ByIntLength` | `By*` no comunica «devuelve `MlResult`» | **`TryCreate(...)`** o **`Validate(...)`** / `CreateResult(...)` |
| `Void.Create()` / `Empty.Create()` | Rompe con el patrón `From*` | `Unit.Value` (propiedad singleton) |
| `IdLite.BuildErrorMessIdLite` | Copy-paste roto | `BuildErrorMessage` |
| `Id.BuildErrorMessage` → `"Id can be mayor 0."` | *Spanglish* («mayor») y gramática incorrecta | `"Id must be greater than 0."` |
| `"cannot be less than {lenght} characters"` | Errata **visible en runtime** | `{length}` |
| `Mail.EndpointPattern` | Nombre heredado de otro tipo | `EmailPattern` |

**Regla propuesta para todo el proyecto:**

| Intención | Nombre | Devuelve |
|---|---|---|
| Crear o fallar | `From(...)` / `Create(...)` | el tipo (lanza excepción) |
| Crear o error funcional | `TryCreate(...)` | `MlResult<T>` |
| Validar sin crear | `IsValid(...)` | `bool` |
| Mensaje de error | `BuildErrorMessage(...)` | `string` |

---

## 11. Resumen ejecutivo — top 10

Ordenado por relación **valor / coste**.

| # | Actual | Propuesto | Motivo |
|---|---|---|---|
| 1 | `*Lenght*` (todos) | `*Length*` | Errata en API pública y en mensajes al usuario |
| 2 | `IdLite` | `Int32Id` | Nombre sin significado |
| 3 | `Id` (`double`) | `Int64Id` (`long`) | Tipo inadecuado + nombre que lo oculta |
| 4 | `RangeEnumValueObject<T>` | `EnumName<T>` | Engañoso + sufijo redundante |
| 5 | `Endpoint` | `HttpsUrl` | Colisión con ASP.NET Core + semántica falsa |
| 6 | `Mail` | `EmailAddress` | Término estándar del sector |
| 7 | `Void` + `Empty` | `Unit` (uno solo) | Duplicado + colisión con la BCL |
| 8 | `StringBetweenLength` | `StringLengthBetween` | Gramática invertida |
| 9 | `*MoreThan` | `*GreaterThan` | Idiomático en .NET |
| 10 | `StringAs*` (12) | `NumericString<T>` | Elimina los 44 `CS0108` |

---

## 12. Estrategia de migración

Todos estos cambios son **rupturas de API pública** (versión mayor). Ruta segura:

1. **Crear** los nuevos nombres como tipos reales, con la implementación definitiva.
2. **Marcar** los antiguos con `[Obsolete("Use XxxNew instead.", error: false)]`, heredando o delegando
   al nuevo para no duplicar lógica.
3. **Publicar** como versión *minor* con avisos de compilación; retirar los antiguos en la siguiente *major*.
4. Las erratas **internas** (nombres de variables locales, textos de mensajes de error) se pueden corregir
   **ya mismo**, salvo los nombres de parámetro, que rompen a quien use argumentos nombrados.

```csharp
// Ejemplo de paso 2
[Obsolete("Use EmailAddress instead. Mail will be removed in v2.0.", error: false)]
public class Mail : EmailAddress { /* ... */ }
```

### Qué se puede hacer sin romper nada

- [ ] Corregir los textos de los mensajes de error (`lenght` → `length`, «mayor» → «greater than»).
- [ ] Renombrar variables y campos **privados**.
- [ ] Renombrar `Mail.EndpointPattern` → `EmailPattern` *(⚠️ es `public const`: es ruptura, pero de bajísimo uso)*.
- [ ] Añadir documentación XML explicando el invariante de cada tipo (compensa nombres ambiguos hasta el rename).

---

## 13. Checklist de seguimiento

### Erratas y limpieza (🟢 baja, sin rediseño)

- [ ] **N1.** Errata `Lenght` → `Length` en todos los identificadores y mensajes.
- [ ] **N2.** `IdLite.BuildErrorMessIdLite` → `BuildErrorMessage`.
- [ ] **N3.** `Id.Fromdouble` / `Id.Bydouble` → *casing* correcto (`FromDouble` / `ByDouble`).
- [ ] **N4.** `Mail.EndpointPattern` → `EmailPattern`.
- [ ] **N5.** Mensaje `"Id can be mayor 0."` → `"Id must be greater than 0."`.
- [ ] **N6.** Unificar `FOOP` / `OOFP` en carpetas, namespaces y proyectos.

### Renombrados de tipos (🟡 media, con `[Obsolete]`)

- [ ] **N7.** `Void` + `Empty` → `Unit` (fusionar los dos).
- [ ] **N8.** `IdLite` → `Int32Id`; `Id` → `Int64Id` (y cambiar `double` por `long`).
- [ ] **N9.** `Mail` → `EmailAddress`.
- [ ] **N10.** `Endpoint` → `HttpsUrl`.
- [ ] **N11.** `Key` → `EntityKey`; `Name` → `DisplayName`.
- [ ] **N12.** `RangeEnumValueObject<T>` → `EnumName<T>`.
- [ ] **N13.** `StringBetweenLength` → `StringLengthBetween`; `StringMinLength` / `StringMaxLength` → `MinLengthString` / `MaxLengthString`.
- [ ] **N14.** `*MoreThan` → `*GreaterThan`; `*Between` → `*InRange`; `*NotNegative` → `NonNegative*`.
- [ ] **N15.** `NotEmptyString` → `NonBlankString`.
- [ ] **N16.** `RegexValue` → `RegexString`.

### Rediseño (🟠 alta, versión 2.0)

- [ ] **N17.** Sustituir los 12 tipos `StringAs*` por `NumericString<T>` (elimina los 44 `CS0108`).
- [ ] **N18.** Sustituir los 12 tipos numéricos por `GreaterThan<T>` / `LessThan<T>` / `InRange<T>` / `NonNegative<T>` con `INumber<T>`.
- [ ] **N19.** `ValueObject<TValue>` → `SingleValueObject<TValue>` y hacerla `abstract`.
- [ ] **N20.** Unificar los verbos de factoría: `From` / `TryCreate` / `IsValid` / `BuildErrorMessage`.
- [ ] **N21.** Renombrar los parámetros `length` de los comparadores a `exclusiveMinimum` / `exclusiveMaximum` / `min` / `max`.
