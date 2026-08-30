# Consejos de nomenclatura para MoralesLarios.OOFP

> 📌 **Este documento no cambia ni una línea de código.** Es una propuesta de renombrados,
> ordenada por proyecto, con el nombre actual, el nombre sugerido y el motivo. Sirve como
> hoja de ruta para ir mejorando la legibilidad de la biblioteca poco a poco.

---

## Índice

1. [Cómo leer este documento](#cómo-leer-este-documento)
2. [Principios de nomenclatura que propongo fijar](#principios-de-nomenclatura-que-propongo-fijar)
3. [Nivel 0 — Solución, proyectos y carpetas](#nivel-0--solución-proyectos-y-carpetas)
4. [Nivel 1 — Erratas ortográficas (prioridad máxima)](#nivel-1--erratas-ortográficas-prioridad-máxima)
5. [Nivel 2 — Núcleo: `MlResult` y sus familias](#nivel-2--núcleo-mlresult-y-sus-familias)
6. [Nivel 3 — `ValueObjects` y `ValueObjects.IO`](#nivel-3--valueobjects-y-valueobjectsio)
7. [Nivel 4 — `Validation`, `Dataannotations` y `FluentValidations`](#nivel-4--validation-dataannotations-y-fluentvalidations)
8. [Nivel 5 — `Internals`, `IO`, `Utilities` y `Extensions.Loggers`](#nivel-5--internals-io-utilities-y-extensionsloggers)
9. [Nivel 6 — `EFCore`](#nivel-6--efcore)
10. [Nivel 7 — `WebServices`](#nivel-7--webservices)
11. [Nivel 8 — `WebApi`](#nivel-8--webapi)
12. [Nivel 9 — `WebControllers` y `WebControllers.Cache`](#nivel-9--webcontrollers-y-webcontrollerscache)
13. [Nivel 10 — `HttpClients`](#nivel-10--httpclients)
14. [Guía de estilo resultante](#guía-de-estilo-resultante)
15. [Cómo aplicar los cambios sin romper a nadie](#cómo-aplicar-los-cambios-sin-romper-a-nadie)
16. [Checklist de ejecución](#checklist-de-ejecución)

---

## Cómo leer este documento

Cada sección es una tabla con tres columnas:

| Columna | Significado |
|---|---|
| **Nombre actual** | Exactamente como aparece hoy en el código. |
| **Nombre propuesto** | Mi sugerencia. |
| **Por qué** | El problema concreto que resuelve el renombrado. |

Y cada entrada lleva una etiqueta de impacto:

- 🔴 **Ruptura pública** — es un tipo o miembro `public`; renombrarlo rompe a los consumidores.
  Requiere `[Obsolete]` + versión mayor (ver [migración](#cómo-aplicar-los-cambios-sin-romper-a-nadie)).
- 🟡 **Ruptura interna** — `internal`, `protected` o `private`: se puede renombrar con seguridad.
- 🟢 **Sin ruptura** — carpetas, ficheros, parámetros de tipo genérico, variables locales,
  nombres de parámetros no usados por llamada nombrada.

> 💡 **Regla de oro del documento:** un buen nombre responde a *«¿qué obtengo?»* y *«¿qué pasa si
> falla?»* sin necesidad de abrir la implementación. Si para entender un método hay que leer su
> cuerpo, el nombre es mejorable.

---

## Principios de nomenclatura que propongo fijar

Antes de las tablas, los criterios que he usado para proponer cada nombre:

1. **Ortografía correcta y en un solo idioma técnico (inglés) para el código.**
   Hoy conviven `Erros`/`Errors`, `Lenght`/`Length`, `Alwais`/`Always`, `Adress`/`Address`.
   Una errata en un nombre público es permanente: se copia en cada `using`, cada test y cada blog.
2. **Simetría en los pares.** Si existe `IsValid`, lo natural es `IsInvalid` (o `IsSuccess`/`IsFailure`),
   no `IsFail`. Si existe `ExistsFile`, debe existir `ExistsDirectory`, no `ExistDirectory`.
3. **El nombre dice el resultado, no la mecánica interna.**
   `GetPkValuesString` describe cómo; `FormatCompositeKey` describe qué.
4. **Sufijos con significado fijo y único:**
   `…Async` (devuelve `Task`), `Try…` (captura excepciones y las convierte en `Fail`),
   `…Fp` (variante que devuelve `MlResult`), `…Info` (DTO de parámetros), `I…` (interfaz).
5. **Nada de sufijos vacíos.** `…Plus`, `…Helper`, `…Manager`, `…Utils` o `…Internals` no aportan
   información: cualquier clase podría llamarse así. Se sustituyen por el rol real.
6. **Los tipos genéricos se nombran por su papel:** `TEntity`, `TDto`, `TRequest`, `TResponse`, `TKey`.
   Nunca letras sueltas (`K`) ni nombres con errata (`TEnumrableResponse`).
7. **Los nombres muy cortos y genéricos necesitan contexto.**
   `Key`, `Name` o `Empty` en un espacio de nombres global colisionan mentalmente con `System`.
8. **Plural = colección, singular = elemento.** `Repos` (carpeta) sí; `MlResultBucles` (una clase
   con métodos) no.
9. **Las abreviaturas solo si son universales en el dominio:** `Dto`, `Pk`, `Http`, `Api`, `Sql`, `Id`.
   `Gen`, `Pag`, `Desc`, `Ex` no lo son.
10. **Español solo en la documentación; inglés en el código.** Hoy hay comentarios y mensajes de
    error mezclados; los nombres de miembros deberían quedar todos en inglés.

---

## Nivel 0 — Solución, proyectos y carpetas

Este es el renombrado de mayor impacto visual y el más barato de todos: **no cambia ni una firma**.

### El problema `FOOP` vs `OOFP`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| Carpeta `MoralesLarios.FOOP\` que contiene `MoralesLarios.OOFP.csproj` | `MoralesLarios.OOFP\` | 🟢 La carpeta y el proyecto **dicen cosas distintas** (`FOOP` vs `OOFP`). Quien clona el repo no sabe cuál es el nombre real de la biblioteca. |
| `MoralesLarios.FOOP.sln` (dentro de `MoralesLarios.FOOP\`) | *(eliminar)* | 🟢 Hay **tres** soluciones: `MoralesLarios.OOFP.sln`, `MoralesLarios.OOFP - copia.sln` y esta. Sobran dos. |
| `MoralesLarios.OOFP - copia.sln` | *(eliminar)* | 🟢 «copia» en el nombre de un artefacto versionado es ruido puro; para eso está Git. |
| `MoralesLarios.OOFP` (paquete/assembly del núcleo) | `MoralesLarios.OOFP.Core` | 🔴 El núcleo comparte nombre con la solución entera. Un `…​.Core` explícito deja claro que es *una* pieza y no *el todo*, igual que hace `Microsoft.Extensions.*`. |

> ⚠️ **Si solo vas a hacer un renombrado de este documento, haz este.** Unificar `FOOP`/`OOFP` en
> `OOFP` en carpetas, `.sln`, `.csproj` y namespaces elimina la confusión más costosa de la solución.

### Nombres de proyecto

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` | 🔴 «Internals» promete que **no** es para consumo externo, pero sus tipos son `public` y los usan `EFCore`, `WebApi` y `HttpClients`. `Shared` (o `Contracts`) describe la realidad: tipos compartidos entre capas. |
| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` | 🔴 «Utilities» no dice nada. El proyecto hace **una** cosa: leer `IConfiguration` devolviendo `MlResult`. |
| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` | 🔴 `IO` colisiona con `System.IO` y obliga a cualificar. `FileSystem` es inequívoco y describe el alcance real (ficheros y directorios). |
| `MoralesLarios.OOFP.Validation.Dataannotations` | `MoralesLarios.OOFP.Validation.DataAnnotations` | 🔴 El tipo de .NET es `DataAnnotations`, con `A` mayúscula. Escribirlo mal en el nombre del paquete perjudica hasta la búsqueda en NuGet. |
| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` | 🔴 «WebApi» sugiere *una API de ejemplo*; el proyecto en realidad solo traduce `MlResult` → `IActionResult`/`ProblemDetails`. El nombre propuesto dice el qué y la plataforma. |
| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` | 🔴 Coherencia con el anterior y agrupación por plataforma. |
| `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` | 🔴 «Cache» es ambiguo (¿memoria? ¿distribuida? ¿HTTP?). El proyecto implementa **output caching** de ASP.NET Core. |
| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` | 🔴 «WebServices» arrastra el significado histórico de SOAP/ASMX y además **no depende de la web**: es la capa de aplicación (CRUD + mapeo + trazas). |
| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` | 🔴 Plural innecesario; el proyecto es la integración HTTP completa (fábrica, clientes, cabeceras). |
| `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` | 🔴 Convención del ecosistema (`Microsoft.EntityFrameworkCore.*`). Con `EFCore` a secas se pierde el enganche en las búsquedas. |
| `MoralesLarios.OOFP.EFCore.Infrastructure.Tests` | `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` | 🔴 **No contiene tests**: contiene el `DbContext`, modelos y configuraciones que *usan* los tests. Llamarlo `…Tests` hace que se ejecute (y falle) en los pipelines. |
| `MoralesLarios.OOFP.Unit.Tests` | `MoralesLarios.OOFP.Core.Tests.Unit` | 🟢 El resto de la solución usa el orden `…Tests.Unit` / `…Tests.Integration`. Este es el único que lo invierte. |
| `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit` | `MoralesLarios.OOFP.ValueObjects.IO.Tests.Unit` | 🟢 `Test` en singular rompe el patrón de los otros 10 proyectos de test. |
| `MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit` | *(fusionar con el anterior)* | 🟢 Un `.2` en el nombre de un proyecto es deuda técnica visible: son dos proyectos de test para la misma biblioteca. |
| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` | 🔴 «Extensions.Loggers» describe la técnica (métodos de extensión) en lugar de la capacidad. Además `Extensions` como segmento intermedio se confunde con `Microsoft.Extensions.*`. |
| `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests` | `MoralesLarios.OOFP.Logging.Tests.Unit` | 🟢 «Console» era el proyecto de pruebas manuales; si son tests, el sufijo debe ser el estándar. |

### Carpetas dentro de los proyectos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.FOOP\__Doc\` | `docs\` (en la raíz del repo) | 🟢 El doble guion bajo es una convención privada que ningún generador de documentación reconoce. Además ya existe una carpeta `docs\` **vacía** en la raíz: hay dos sitios para lo mismo. |
| `__Doc\Bucle\` | `docs\loops\` | 🟢 Único nombre de carpeta en español dentro de una jerarquía en inglés. |
| `__Doc\Types\` | `docs\core-types\` | 🟢 «Types» es tan genérico que no orienta: todo en C# es un tipo. |
| `__Doc\PendingTasks.txt` | *(mover a issues de GitHub)* | 🟢 Un TODO en `.txt` dentro de la carpeta de documentación no lo lee nadie y no se puede cerrar. |
| `EFCore\OopRepos\` | `EFCore\Repositories.Imperative\` | 🟢 «Oop» como prefijo es confuso (todo el código es OOP). Lo que distingue esa carpeta es que **lanza excepciones** en lugar de devolver `MlResult`. |
| `EFCore\Repos\` | `EFCore\Repositories\` | 🟢 «Repos» es jerga; el nombre completo cuesta lo mismo de escribir una vez. |
| `HttpClients\ParamsInfo\` | `Http\Requests\` | 🟢 La carpeta contiene los *records* que describen una petición, no «info de params». |
| `WebServices\Services\` (con `GenService.cs` vacío) | *(eliminar la carpeta o el fichero vacío)* | 🟢 Un fichero vacío en una carpeta propia sugiere que falta código. |

---

## Nivel 1 — Erratas ortográficas (prioridad máxima)

Estas son las que arreglaría primero después del Nivel 0. Una errata en un nombre público
**se propaga a todo el código de tus usuarios** y ya no se puede corregir sin romperlos.

### Núcleo (`MoralesLarios.OOFP`)

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `FusionFailErros` | `FusionFailErrors` | 🔴 `Erros` → `Errors`. En `Types/MlResultBucles.cs`. Además este método tiene un bug real (le falta el `return`), así que ya vas a tocarlo. |
| `FusionErrosIfExists` | `FusionErrorsIfExists` | 🔴 Misma errata. Y `Fusion` como verbo no existe en inglés: `MergeErrorsIfExists` sería aún mejor. |
| `ChangeReturnResultAlwais` | `ChangeReturnResultAlways` | 🔴 `Alwais` → `Always`. Afecta a toda la familia (`…AlwaisAsync`, `TryChangeReturnResultAlwais`, etc.), unas cuantas sobrecargas. |
| `TryMapIAsyncf` | `TryMapAsync` | 🔴 En `Types/MlResultActionsMap.cs`. Hay una `I` de más y una `f` final huérfana: parece un `TryMapIf` a medio escribir. El nombre correcto depende de la semántica real del método (si condiciona, `TryMapIfAsync`). |
| `MlErrorsDetails` | `MlErrorDetails` | 🔴 «Los detalles de los errores» es `ErrorDetails` en inglés; el plural en el sustantivo intermedio es un calco del español. |

### `ValueObjects`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `FromStringLenght` | `FromStringLength` | 🔴 `Lenght` → `Length`. Es la errata más repetida de la solución. |
| `FromIntLenght` | `FromIntLength` | 🔴 Ídem. |
| `MinLenght` | `MinLength` | 🔴 Ídem, y aquí duele más porque coincide con `MinLengthAttribute` de .NET escrito bien. |
| `Id.Bydouble` | `Id.ByDouble` | 🔴 PascalCase roto en el sufijo del tipo. |
| `Id.Fromdouble` | `Id.FromDouble` | 🔴 Ídem. |
| `ExistDirectory` | `ExistsDirectory` | 🔴 Su pareja se llama `ExistsFile`. Dos métodos hermanos con conjugación distinta obligan a mirar IntelliSense cada vez. |

### `WebServices`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `AddScopedtGenServicesFpWithoutReposGeneral` | `AddScopedCrudServicesFpWithoutRepositories` | 🔴 `Scopedt` tiene una `t` de más. Y `…General` al final no aporta nada. |
| `AddScopedtGenServicesDuplexFpWithoutReposGeneral` | `AddScopedCrudServicesDuplexFpWithoutRepositories` | 🔴 Misma `t` intrusa. |

### `HttpClients`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `TEnumrableResponse` | `TEnumerableResponse` | 🟢 Falta la `e` de `Enumerable`. Es un parámetro genérico, así que **renombrarlo no rompe nada**: es el arreglo más rentable del documento. |
| `BaseAdress` (en comentarios XML y nombres de parámetro) | `BaseAddress` | 🟢/🔴 `Adress` → `Address`. Aparece ~8 veces. En comentarios es 🟢; si es nombre de parámetro y alguien usa argumentos nombrados, es 🔴. |
| `K` (segundo genérico de `IHttpClientFactoryManager`) | `TResponse` | 🟢 Una letra suelta obliga a leer la firma completa para saber qué representa. |

### Cadenas de mensaje visibles para el usuario final

No son identificadores, pero salen por la API y por los logs, así que cuentan:

| Texto actual | Texto propuesto | Por qué |
|---|---|---|
| `"... is not soported"` (`WebControllers/Helpers/Extensions.cs`) | `"... is not supported"` | 🟢 Errata que llega al cliente HTTP en el cuerpo del error. |
| `"is diferent type"` | `"is of a different type"` | 🟢 Errata + gramática. |
| `"source no be null"` | `"source must not be null"` | 🟢 No es inglés. Aparece en `Validation.Dataannotations`. |
| `"source no be empty"` | `"source must not be empty"` | 🟢 Ídem. |
| `"{x} no be null"` | `"{x} must not be null"` | 🟢 Ídem. |
| `"isn't null"` (mensaje de error cuando **sí** es null) | `"must not be null"` | 🟢 El mensaje actual afirma justo lo contrario de lo que ha pasado. |

---

## Nivel 2 — Núcleo: `MlResult` y sus familias

Aquí están los renombrados de mayor valor pedagógico: son los nombres que un recién llegado
lee en los primeros cinco minutos.

### El tipo `MlResult<T>`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `IsValid` | `IsSuccess` | 🔴 «Válido» es vocabulario de **validación**; `MlResult` no valida, representa el desenlace de una operación. Con `IsSuccess` no hay que explicar que un resultado puede ser «no válido» aunque el dato sea perfecto (p. ej. un timeout). |
| `IsFail` | `IsFailure` | 🔴 `Fail` es verbo, `Failure` es sustantivo; una propiedad booleana pide sustantivo o adjetivo. Y así el par `IsSuccess`/`IsFailure` es simétrico, que es lo que hacen `Result` de FluentResults, `LanguageExt` y `CSharpFunctionalExtensions`. |
| `SecureValidValue()` | `GetValueOrThrow()` | 🔴 «Secure» sugiere seguridad/criptografía. El método hace lo contrario de ser seguro: **lanza** si el resultado es fallido. `GetValueOrThrow` es el nombre que todo el mundo espera y avisa del peligro. |
| `Value` (`internal protected`) | `SuccessValue` | 🟡 Al ser interno el renombrado es gratis, y `Value` a secas obliga a recordar que solo es legible en la rama válida. |
| `ErrorsDetails` (`internal protected`) | `FailureDetails` | 🟡 Coherencia con `IsFailure`, y evita el plural intermedio. |

### `MlError` y `MlErrorsDetails`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MlErrorsDetails` | `MlErrorDetails` | 🔴 Ver Nivel 1. Es el tipo que más aparece en la documentación, así que la errata se ve mucho. |
| `MlErrorsDetails.Errors` | *(mantener)* | ✅ Correcto: es una colección. |
| `MlErrorsDetails.Details` | `Metadata` | 🔴 `Details` dentro de `…Details` es redundante (`errorDetails.Details`). El diccionario contiene metadatos arbitrarios adjuntos al error: `Metadata` lo dice mejor y encaja con `Activity.Tags`/`Exception.Data`. |
| `ToErrorsMessages()` | `GetErrorMessages()` | 🔴 Devuelve una colección, no convierte de un tipo a otro; `To…` se reserva para conversiones (`ToString`, `ToList`). Además `Errors` en plural delante de `Messages` sobra. |
| `ToErrorsDescription()` | `GetErrorsSummary()` | 🔴 Devuelve **una** cadena que concatena todos los mensajes; «description» y «messages» suenan a lo mismo y hoy es imposible adivinar cuál devuelve `string` y cuál `IEnumerable<string>`. |
| `ToDetailsDescription()` | `GetMetadataSummary()` | 🔴 Coherencia con los dos anteriores. |
| `HasKeyDetails(string)` | `ContainsMetadata(string key)` | 🔴 El orden de las palabras está invertido (`KeyDetails` en lugar de `DetailKey`) y `Contains…` es el verbo canónico para preguntar por una clave. |
| `HasValueDetails()` | `ContainsValueMetadata()` | 🔴 Ídem. Hoy se confunde con «tiene algún detalle». |
| `HasExceptionDetails()` | `ContainsException()` | 🔴 Ídem, y así hace pareja con `GetException()`. |
| `GetDetailValue<T>()` | `GetMetadataValue<T>()` | 🔴 Coherencia con `Metadata`. |
| `GetDetailException()` | `GetException()` | 🔴 «Detail» aquí no aporta: solo hay una excepción por resultado. |
| `AddDetail<T>(...)` | `WithMetadata<T>(...)` | 🔴 **Muy importante:** `AddDetail` **muta** el objeto y **lanza** si la clave existe. En una biblioteca funcional, un `Add…` que muta rompe la expectativa del usuario. `WithMetadata` (que devolvería una copia) alinea nombre y semántica; si no se puede cambiar el comportamiento, al menos `AddDetailOrThrow`. |
| `MlError.Message` | *(mantener)* | ✅ Simple y correcto. |

### Constantes

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `EX_DESC_KEY` (`= "Ex"`) | `ExceptionMetadataKey` | 🔴 `SCREAMING_SNAKE_CASE` no es la convención de .NET para `const` públicas (sí lo es en C/C++ y Java). Y `EX_DESC` mezcla dos abreviaturas opacas. |
| El valor `"Ex"` | `"Exception"` | 🔴 La clave viaja a los logs y a los `ProblemDetails`; ahí `"Ex"` no significa nada para quien lee la traza. |
| `VALUE_KEY` (`= "Value"`) | `ValueMetadataKey` | 🔴 Ídem con la convención de nombres. |

### Familias de métodos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MlResultBucles` | `MlResultCollections` | 🔴 **Único nombre de clase en español de todo el núcleo.** Además «bucles» describe el mecanismo (iterar) y no el propósito (operar sobre colecciones de resultados). |
| `Projection` / `ProjectionAsync` | `Traverse` / `TraverseAsync` | 🔴 «Projection» ya significa otra cosa en LINQ (`Select`). La operación real —convertir `IEnumerable<MlResult<T>>` en `MlResult<IEnumerable<T>>`— se llama `Traverse`/`Sequence` en toda la literatura funcional; usar el nombre estándar hace que la documentación externa sirva. |
| `MlResultTransformations` | `MlResultConversions` | 🔴 «Transformation» solapa con `Map`, que también transforma. Esta clase convierte entre `MlResult`, `Nullable`, `Try`, etc. |
| `MlResultActionsMap` (fichero/clase) | `MlResultMapExtensions` | 🟡 «Actions» es engañoso: un `Action` en C# no devuelve valor, y estos métodos sí. El sufijo `…Extensions` es la convención para clases estáticas de extensión. |
| `EmptyToFailed` | `FailIfEmpty` | 🔴 El nombre actual se lee «convierte lo vacío en fallido», pero el método recibe una **colección** y decide. `FailIf…` deja el predicado delante, igual que `EnsureFp.That`. |
| `NullToFailed` | `FailIfNull` | 🔴 Ídem. |
| `BoolToResult` | `ToMlResult` (o `FailIfFalse`) | 🔴 `…ToResult` es correcto salvo que el tipo no se llama `Result`, se llama `MlResult`: el nombre miente ligeramente sobre el tipo de retorno. |
| `Do` | `Tap` | 🔴 `Do` no dice si el valor sigue fluyendo. `Tap` (o el `ExecSelf` que ya usa la propia biblioteca) sí. **Peor aún: hoy conviven `Do` y `ExecSelf` para la misma idea**; hay que quedarse con uno. |
| `Combine` | `Merge` o `CombineAll` | 🔴 Con dos resultados «combine» está bien; con N conviene indicar que agrega **todos** los errores y no cortocircuita. |
| `EnsureFp.That` | *(mantener)* | ✅ Es la primitiva y el nombre es idiomático (igual que en xUnit/Shouldly). |
| `MlResultBucles.FusionFailErros` | `MergeFailureErrors` | 🔴 Ver Nivel 1: además de la errata, `Fusion` no es un verbo inglés. |

### Sufijos de familia que conviene revisar

| Patrón actual | Propuesta | Por qué |
|---|---|---|
| `…IfFailWithException` / `…IfFailWithoutException` | *(mantener)* | ✅ Largos, pero autodescriptivos y simétricos. |
| `…Always` (tras corregir `Alwais`) | *(mantener)* | ✅ El significado —«ejecuta en las dos ramas»— es claro. |
| `Try…` | *(mantener)* | ✅ Convención ya establecida y documentada en `1_Intro.md`. |
| `…Fp` | *(mantener, pero documentar)* | ✅ Solo tiene sentido cuando coexiste con la variante que lanza excepciones (`EFRepoBase` vs `EFRepoBaseFp`). Donde **no** existe pareja, el sufijo es ruido. |

---

## Nivel 3 — `ValueObjects` y `ValueObjects.IO`

Los *value objects* son tipos que aparecen en las firmas del usuario, así que sus nombres pesan
mucho. Y aquí hay dos casos de nombres **peligrosamente genéricos**.

### Tipos con nombre demasiado corto

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `Key` | `ServiceKey` (o `RegistrationKey`) | 🔴 `Key` colisiona mentalmente con `KeyValuePair.Key`, con claves de diccionario y con claves primarias. En esta biblioteca representa la **clave de registro con nombre en el contenedor de DI** (`HttpClientFactory`, repos con clave): el nombre debe decirlo. |
| `Name` | `MemberName` (o `Identifier`) | 🔴 Ídem: `Name` es una de las palabras más colisionadas de .NET. Además su `IsValid` **ignora el parámetro `length`**, lo que refuerza que hoy nadie sabe qué garantiza. |
| `Empty` | `EmptyValue` (o eliminarlo y usar `Option`/`null`) | 🔴 Un tipo público llamado `Empty` no se puede leer en una firma: `Empty x` parece un error de compilación. Y colisiona con `string.Empty`, `Array.Empty<T>()`, `Enumerable.Empty<T>()`. |
| `Id` | *(mantener)* | ✅ Universalmente entendido en el dominio. Sus factorías sí necesitan arreglo (ver Nivel 1). |

### Numéricos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `IntNotNegative.limit` (`public static int limit;`) | `IntNotNegative.MinAllowedValue` (`private const`) | 🔴 Tres problemas en un nombre: es **`public`**, es **`static` mutable** (cualquiera puede cambiar la validación de todas las instancias) y está en **camelCase** siendo público. El nombre propuesto además explica qué límite es. |
| `DecimalNotNegative` / `DoubleNotNegative` | `DecimalNonNegative` / `DoubleNonNegative` | 🔴 En inglés técnico, «no negativo» (≥ 0) es `NonNegative`; `NotNegative` se lee como una negación de acción. Y ojo: **hoy estos dos tipos son inconstruibles** por un choque entre `IsValid` y el constructor base, así que el renombrado viene con arreglo obligatorio. |
| `IntNotNegative`, `LongNotNegative`… | `IntNonNegative`, `LongNonNegative`… | 🔴 Coherencia con lo anterior en toda la familia. |
| `IntPositive` | *(mantener)* | ✅ Correcto y sin ambigüedad. |
| `RangeEnumValueObject` (fichero entero comentado) | `EnumRangeValueObject` *(o eliminar el fichero)* | 🟢 Mientras esté comentado no es API; si se recupera, el orden natural en inglés es `EnumRange…`. Un fichero 100 % comentado es peor que no tenerlo. |

### Cadenas

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `StringBetweenLength` | `StringLengthBetween` | 🔴 El orden actual se lee «cadena entre longitud»; el propuesto, «longitud de cadena entre X e Y». (Y su `BuildErrorMessage` **intercambia min y max**, con lo cual el mensaje miente.) |
| `FromStringLenght` | `FromLength` | 🔴 Además de la errata, `String` es redundante en un tipo que ya se llama `String…`. |
| `MinLenght` / `MaxLenght` | `MinLength` / `MaxLength` | 🔴 Errata; ver Nivel 1. |
| `StringNotEmpty` | *(mantener)* | ✅ Coincide con `string.IsNullOrEmpty`, así que el usuario ya lo entiende. |

### Patrón de factorías

Hoy conviven dos prefijos para lo mismo y el criterio no está documentado:

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `By…` (`Id.ByDouble`, `ExistsFile.ByString`) | `Create…` / `From…` | 🔴 `By…` sugiere «ordenado por» (`OrderBy`, `GroupBy`, `ThenBy`). Como aquí **construye**, lo correcto es `From…` (conversión) o `Create…` (fábrica). |
| `From…` (`FromLength`, `FromDouble`) | *(mantener como único prefijo)* | ✅ Es el patrón de .NET (`DateTime.FromBinary`, `TimeSpan.FromSeconds`). |
| **Convención propuesta** | `From<Tipo>` devuelve `MlResult<T>`; `Create<Tipo>` lanza | 🟢 Con dos prefijos y una regla escrita, el usuario sabe qué esperar sin abrir el código. Hoy la diferencia entre `By…` y `From…` es invisible. |

### `ValueObjects.IO`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `ExistsFile` | `ExistingFilePath` | 🔴 `ExistsFile` parece un **método** que devuelve `bool` (como `File.Exists`), no un tipo que envuelve una ruta. `ExistingFilePath` deja claro que es un dato con una invariante. |
| `ExistDirectory` | `ExistingDirectoryPath` | 🔴 Además de arreglar la conjugación (Nivel 1), mismo razonamiento. |
| `MlFile` | `FilePath` (o `MlFilePath`) | 🔴 `MlFile` sugiere que representa **el fichero** (contenido, handle); en realidad envuelve **la ruta**. La distinción evita bugs. |
| `MlDirectory` | `DirectoryPath` (o `MlDirectoryPath`) | 🔴 Ídem. |
| `MlFile.EndpointPattern` | `FileNamePattern` (o `SearchPattern`) | 🔴 **El peor nombre de la solución:** «endpoint» es vocabulario HTTP y aquí no hay ninguna URL. Es el patrón de búsqueda que se pasa a `Directory.GetFiles`, así que `SearchPattern` es el nombre que ya usa .NET. |
| `MlDirectory.EndpointPattern` | `DirectoryNamePattern` (o `SearchPattern`) | 🔴 Ídem. |
| `ExistsFile.ByString` | `ExistingFilePath.FromPath` | 🔴 «ByString» describe el **tipo del parámetro**, no la intención. Y todo en C# se puede construir «desde un string»: no distingue nada. |
| Constructores públicos que lanzan `ArgumentNullException` | *(hacerlos `private` y exponer solo `From…`)* | 🟡 No es un renombrado, pero es el mismo problema de fondo: hoy el usuario tiene dos caminos (constructor que lanza, factoría que devuelve `MlResult`) y el nombre no le avisa de cuál es el «bueno». |

---

## Nivel 4 — `Validation`, `Dataannotations` y `FluentValidations`

### Nombres de proyecto y namespace

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.Validation.Dataannotations` | `…Validation.DataAnnotations` | 🔴 El nombre oficial es `System.ComponentModel.DataAnnotations`. Escribirlo mal en namespace y paquete cuesta autocompletado y posicionamiento en NuGet. |
| `MoralesLarios.OOFP.Validation.FluentValidations` | `…Validation.FluentValidation` | 🔴 La biblioteca se llama **FluentValidation**, en singular. El plural indica al lector que no se ha mirado el nombre real del paquete. |
| `DataannotationsValidator` | `DataAnnotationsValidator` | 🔴 Ídem dentro de la clase. |

### Abstracciones

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MlValidableFp<T>` | `MlSelfValidatable<T>` | 🔴 Tres problemas: (1) en inglés es `Validatable`, no `Validable`; (2) el sufijo `Fp` no distingue nada porque **no hay** versión no-`Fp`; (3) el nombre no dice que el patrón es CRTP (el tipo se valida a sí mismo). |
| `MlValidableFp<T>` como **`abstract class`** | `IMlSelfValidatable<T>` (interfaz) | 🟡 El nombre `…able` es una promesa de capacidad, y en .NET las capacidades se expresan con interfaces (`IComparable`, `IDisposable`). Al ser clase abstracta consume la única herencia disponible del usuario. |
| `ValidateObject(this object)` en `Validation.Dataannotations.Helpers.Extensions` | `ValidateDataAnnotations(this object)` | 🔴 **Colisiona con el `ValidateObject` del núcleo** y produce `CS0121` (llamada ambigua) en cuanto se importan los dos namespaces. Un nombre específico elimina el error de compilación sin `using` alias. |
| `IValidator` / `IValidatorFp` (si coexisten) | `IMlValidator<T>` | 🔴 Nombre único, genérico y con el prefijo de la biblioteca para no chocar con `FluentValidation.IValidator<T>`, que se importa en el mismo fichero. |

### Métodos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `ValidateAsync<T>` (que internamente es `Task.FromResult`) | `Validate<T>` | 🔴 **Async falso.** Un método con sufijo `Async` que no hace E/S ni cede el hilo engaña al llamador, que añadirá `await` y `ConfigureAwait` sin necesidad. Si se mantiene por compatibilidad de interfaz, hay que documentarlo explícitamente. |
| `Validate` / `ValidateAsync` con y sin guardas | *(unificar nombres o diferenciarlos)* | 🔴 Hoy dos sobrecargas con el mismo nombre se comportan distinto: unas comprueban `null` y otras revientan con `NullReferenceException`. Si el comportamiento difiere, el nombre debe diferir (`ValidateOrThrow` vs `Validate`). |
| `BuildErrorMessage` | `FormatErrorMessage` | 🟡 `Build…` sugiere un *builder* con estado acumulado; aquí solo se formatea una cadena. |

### Conversión de resultados

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `ToMlResult(this ValidationResult)` | `ToMlResult` *(mantener)* | ✅ Nombre correcto. El problema es de comportamiento: pierde `MemberNames`. |
| `errors!` (force-unwrap en el cuerpo) | *(no es nombre, pero anótalo)* | 🟢 El `!` documenta una suposición no verificada; si el nombre de la variable fuera `errorsOrNull` se vería el riesgo al leer. |

---

## Nivel 5 — `Internals`, `IO`, `Utilities` y `Extensions.Loggers`

### `Internals` → `Shared`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` | 🔴 Ver Nivel 0: el nombre promete privacidad que no existe. |
| `Info/PaginationInfo` | `PageRequest` | 🔴 El sufijo `Info` no distingue nada (todo DTO es «info»). `PageRequest` dice que es lo que **pide** el cliente (número y tamaño de página), frente a lo que devuelve el servidor. |
| `Info/PaginationResultInfo` | `PagedResult<T>` | 🔴 Es el nombre de facto en el ecosistema .NET y hace pareja obvia con `PageRequest`. Hoy `PaginationInfo` y `PaginationResultInfo` se diferencian por una palabra en el medio, lo que provoca errores al escoger en IntelliSense. |
| `Info/` (carpeta) | `Pagination/` | 🟢 Una carpeta llamada «Info» no ayuda a encontrar nada. |
| `ProblemDetailsInfo` | `ProblemDetailsData` *(o eliminar)* | 🔴 Coexiste con `MlProblemsDetails` y con el `ProblemDetails` de ASP.NET Core: **tres tipos con el mismo concepto y tres nombres distintos**. Lo ideal es quedarse con uno. |
| `IMlConfigManager` | `IMlConfigurationReader` | 🔴 «Manager» es el ejemplo clásico de sufijo vacío. La interfaz solo **lee** configuración: `Reader` dice qué se puede hacer y qué no. |

### `IO` → `FileSystem`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` | 🔴 Ver Nivel 0. |
| `MlFileHelper` / `MlFileUtils` (sufijos vacíos) | `MlFile` / `MlFileOperations` | 🔴 `Helper` y `Utils` son contenedores de «lo que no supe dónde poner»; un nombre por capacidad obliga a agrupar mejor. |
| `ReadAllTextFp` | `ReadAllText` | 🔴 En un proyecto donde **todo** devuelve `MlResult`, el sufijo `Fp` es ruido en cada llamada. Se reserva solo donde exista la pareja imperativa. |

### `Utilities` → `Configuration`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` | 🔴 Ver Nivel 0. Un proyecto llamado «Utilities» acaba siendo el vertedero de la solución. |
| `GetSection` / `GetValue` sin sufijo de fallo | `TryGetSection` / `TryGetValue` | 🟡 Coherencia con la convención `Try…` ya documentada en `1_Intro.md`. |

### `Extensions.Loggers` → `Logging`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` | 🔴 Ver Nivel 0. |
| `GeneralExtensionLoggers` | `MlResultLogExtensions` | 🔴 «General» + «Extension» + «Loggers»: tres palabras y ninguna dice sobre **qué** se extiende. |
| `GeneralExtensionLoggersCritical`, `…Error`, `…Warning`… | *(un solo fichero `MlResultLogExtensions.<Level>.cs`)* | 🟢 Una clase por nivel de log multiplica los nombres sin añadir información; con ficheros parciales se mantiene la organización sin contaminar el namespace. |
| `LogMlResultFinalAsync` | `LogResultAsync` | 🔴 «Final» no significa nada para el llamador (¿final de qué?). Si marca el último paso de la cadena, `LogCompletedResultAsync` sería explícito. |
| `…Fp` en métodos de log | *(eliminar)* | 🟢 No existe variante no funcional. |

---

## Nivel 6 — `EFCore`

Este es el proyecto con la nomenclatura más inconsistente: convive vocabulario de dos estilos
(imperativo y funcional) sin una regla escrita.

### Las dos familias de repositorios

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| Carpeta `OopRepos/` + clase `EFRepoBase` | `Repositories.Imperative/` + `ThrowingEfRepository` | 🔴 «Oop» no distingue nada (todo el proyecto es OOP). Lo que de verdad diferencia esta familia es que **lanza excepciones**; el nombre debe avisarlo porque es la decisión de diseño más importante al elegir. |
| Carpeta `Repos/` + clase `EFRepoBaseFp` | `Repositories/` + `EfRepository` | 🔴 Al ser la familia principal (la que devuelve `MlResult`), debería tener el nombre **sin sufijo**; el sufijo lo lleva la excepción a la regla, no la regla. |
| `EFRepo…` | `EfRepository…` | 🔴 Dos cosas: (1) `Repo` es jerga —se escribe una vez y se lee mil—; (2) las siglas de más de dos letras van en PascalCase según las *Framework Design Guidelines* (`Ef`, no `EF`), igual que Microsoft escribe `EfCoreOptions` en su propio código de ejemplo. |
| `EFRepoReaderFp` / `EFRepoWriterFp` / `EFRepoUpdaterFp` / `EFRepoDeleterFp` | `EfReadRepository` / `EfCreateRepository` / `EfUpdateRepository` / `EfDeleteRepository` | 🟡 `…er` sobre `…er` (`Reader`, `Writer`, `Updater`, `Deleter`) suena a máquina y `Writer` no deja claro si escribe o inserta. Con el verbo delante se lee la intención. |
| `EFRepoReaderPaginationFp` | `EfPagedReadRepository` | 🟡 «Pagination» al final del nombre parece un módulo aparte; el adjetivo delante indica que es una variante del lector. |
| `IEFRepoWriterFp` (cuerpo entero comentado) | *(eliminar el fichero)* | 🟢 Una interfaz sin miembros y comentada no es API: es deuda visible. |

### Registro en el contenedor

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `ResolveRepoFp<T>` | `GetRepository<T>` | 🔴 «Resolve» es vocabulario interno del contenedor de DI; el llamador solo quiere **obtener** un repositorio. (Y este método construye un `ServiceProvider` nuevo en cada llamada, que es un bug aparte.) |
| `AddSingletonOOFPRepos<T,TContext>()` | `AddOofpRepositoriesSingleton<T,TContext>()` | 🔴 (1) `OOFP` en mayúsculas dentro de un identificador es difícil de leer junto a otras siglas; (2) el patrón de .NET pone el modificador de ciclo de vida **al final** cuando no es `AddScoped`/`AddSingleton` puro; (3) `Repos` → `Repositories`. Ojo: registrar repos con `DbContext` como singleton es una *captive dependency*, así que quizá el método deba desaparecer. |
| `AddScopedOOFPRepos…` | `AddOofpRepositories…` | 🟢 Coherencia. |
| `Helpers/Constants.cs` (clase vacía) | *(eliminar)* | 🟢 Un nombre sin contenido solo genera dudas. |

### Enumeraciones y consultas

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `enum OrderBy` | `SortDirection` | 🔴 `OrderBy` colisiona directamente con `Queryable.OrderBy`: en un fichero con `using System.Linq` el lector no sabe si está viendo el operador o el enum. Y sus valores (`Asc`/`Desc`) describen una **dirección**, no una cláusula. |
| Valores `Asc` / `Desc` | `Ascending` / `Descending` | 🔴 En un enum público las abreviaturas no ahorran nada (el usuario las autocompleta) y `Desc` se confunde con «description». |
| `TryGetInternalData` | `QueryPageAsync` / `BuildPagedQuery` | 🟡 «InternalData» no dice qué devuelve; y siendo `Try…` público debería describir la operación de negocio, no su papel interno. |
| `TryLast` / `TryLastAsync` | `TryGetLast…` | 🟡 `Last` a secas parece la propiedad de una colección; con `Get` se ve que hay una consulta detrás. |
| `TryFind` / `TryFindAsync` | *(mantener)* | ✅ Coincide con `DbSet.Find`, así que el usuario ya sabe que busca por clave primaria. |

### Modelo de datos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `Pk` en nombres de parámetro (`pk`, `pkValues`) | `key` / `keyValues` | 🟡 `Pk` es una abreviatura aceptable en SQL, pero en la firma pública queda `TryUpdate(item, pk)`, donde `pk` puede ser un valor o una colección. `keyValues` lo resuelve. |
| `_pkFields` (`Func<TEntity, object[]>`) | `keySelector` | 🔴 No son «campos»: es una **función selectora**. El nombre actual hace pensar que se puede inyectar una lista de nombres de columna. |
| `JfCatasDbContext` (en el proyecto de tests) | `TestDbContext` | 🟢 Siglas de un proyecto concreto del autor dentro de código de soporte para tests: nadie de fuera sabe qué es «JfCatas». |

---

## Nivel 7 — `WebServices`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` | 🔴 «Web services» significa SOAP/WCF para casi todo el mundo. Este proyecto contiene la **capa de aplicación** (orquesta repositorios y mapea DTOs) y no sabe nada de HTTP. |
| `IGenServiceFp<…>` | `ICrudServiceFp<…>` | 🔴 `Gen` es una abreviatura de «generic» que no se usa en .NET, y «genérico» ya lo indica la sintaxis `<…>`. `Crud` dice exactamente lo que ofrece (alta, baja, modificación, consulta). |
| `GenServiceFp<…>` | `CrudServiceFp<…>` | 🔴 Ídem. |
| `GenServiceDuplexFp<TRequest,TResponse>` | `CrudServiceFp<TRequest,TResponse>` | 🔴 **«Duplex» es vocabulario de comunicaciones** (full-duplex, WCF duplex channels) y aquí solo significa «DTO de entrada distinto del de salida». Con dos parámetros genéricos el propio compilador ya distingue las dos variantes: **el sufijo es innecesario**. Si hiciera falta una palabra, `SplitDto` o `InOut` serían más honestas. |
| `Services/GenService.cs` (fichero vacío) | *(eliminar)* | 🟢 Ver Nivel 0. |
| `MlProblemsDetails` | `MlProblemDetails` | 🔴 El tipo de referencia de RFC 7807 y de ASP.NET Core se llama `ProblemDetails`, en singular en la primera palabra. El plural extra hace que el `using` no case y que el lector dude de si es el mismo concepto. |
| `UpdateProblemDetailsAsync` | `UpdateAsync` (con overload que devuelve `MlProblemDetails`) | 🔴 El nombre mezcla la operación (`Update`) con el **formato de su error** (`ProblemDetails`). El formato del error no debería aparecer en el nombre del caso de uso. |
| `BuildNotFoundPkError` | `BuildEntityNotFoundError` | 🟡 `Pk` en el medio hace pensar que el error es «sobre la clave»; el error es «no existe la entidad». (Y hoy usa la clave `"NotFound"` en lugar de `"ProblemsDetails"`, lo que provoca un 500 en vez de un 404.) |
| `AllAsync` | `GetPagedAsync` | 🔴 `AllAsync` colisiona con `Queryable.AllAsync` (que devuelve `bool`) y promete «todo», lo que es exactamente lo que no debe hacer un endpoint sin paginación. |
| `validMessageBuilder` (parámetro) | `successMessageFactory` | 🟡 Coherencia con `IsSuccess` (Nivel 2) y `Factory` describe mejor un delegado que produce un valor. |
| `AddScopedtGenServices…` | `AddScopedCrudServices…` | 🔴 Ver Nivel 1 (errata `Scopedt`). |

---

## Nivel 8 — `WebApi`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` | 🔴 «WebApi» es tan amplio que se confunde con un proyecto ejecutable (de hecho hay otro que sí lo es: `EFCore.WebApi`). Este solo convierte `MlResult` en `IActionResult`. |
| `MlResultWebExtensions` | `MlResultActionResultExtensions` | 🔴 «Web» no dice a qué se convierte. Además la clase entera está marcada `[Obsolete]` pero se sigue publicando: el nombre debería reflejar que es la versión antigua (`…Legacy`) o desaparecer. |
| `MlResultWebExtensionsPlus` | `MlResultActionResultExtensions` | 🔴 **`Plus` es el peor sufijo posible**: no dice qué añade ni cuándo usarlo frente a la clase sin `Plus`. Es el síntoma de «ya existía una clase con el nombre bueno». Al retirar la obsoleta, este es el nombre que queda libre. |
| `ToPostPdActionResult<T>` | `ToCreatedProblemDetailsResult<T>` | 🔴 `Pd` es una abreviatura inventada de `ProblemDetails` que nadie va a adivinar; y `Post` describe el **verbo HTTP** en lugar del resultado (`201 Created`). |
| `ToSimpleRepoPostActionResult` | `ToCreatedResult` | 🔴 Mezcla tres conceptos: «simple», «repo» y «post». Al llamador le da igual que detrás haya un repositorio. |
| `MlActionResults` | `MlResults` (o `MlTypedResults`) | 🟡 `MlActionResults` es un contenedor de fábricas estáticas equivalente a `Results`/`TypedResults` de ASP.NET Core; usar un nombre paralelo hace obvio su uso. |
| `MlErrorsDetailsExtensions.ToProblemsDetailsInfo` | `ToProblemDetails` | 🔴 Tres arreglos a la vez: plural intermedio (`Errors`→`Error`, `Problems`→`Problem`) y sufijo `Info` vacío. |
| `ContieneCombinacion` (método privado en español) | `ContainsAnyKey` | 🔴 **Único identificador en español del código de producción.** Rompe la regla de «un solo idioma en el código». |
| `notFoundKeys` (comparación por texto) | `NotFoundDetailKeys` (constante pública) | 🟡 Hoy es una lista local de literales; al ser el contrato que decide entre 404 y 500, debería tener nombre público y documentado. |
| `"X-Page-Number"` / `"X-Page-Size"` (literales) | `MlPaginationHeaders.PageNumber` / `.PageSize` | 🔴 **Están duplicados literalmente en `WebApi` y en `HttpClients`.** Un typo en uno de los dos lados rompe la paginación en silencio; una constante compartida (en `Shared`) lo hace imposible. |
| `MlRequestWebExtensions` | `HttpRequestPaginationExtensions` | 🟡 Extiende `HttpRequest` para leer paginación: el nombre puede decirlo. |
| Nombres asíncronos asimétricos en `…Plus` | *(añadir `Async` a todos los que devuelven `Task`)* | 🔴 Hoy hay métodos que devuelven `Task` sin sufijo `Async` y viceversa. Es la incoherencia que más ralentiza el autocompletado. |

---

## Nivel 9 — `WebControllers` y `WebControllers.Cache`

### Clases base de controlador

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` | 🔴 «Web» + «Controllers» es redundante y no indica el framework. |
| `SimpleMlControllerBase<TEntity,TDto>` | `MlCrudControllerBase<TEntity,TDto>` | 🔴 **`Simple` es un juicio de valor, no una descripción.** ¿Simple frente a qué? Lo que realmente distingue esta clase de su hermana es que la clave primaria es **de una sola columna**. |
| `SimpleMlComplexPkControllerBase<TEntity,TDto>` | `MlCompositeKeyCrudControllerBase<TEntity,TDto>` | 🔴 `Simple` + `Complex` en el mismo nombre es contradictorio y desconcertante. Y el término estándar para una PK de varias columnas es **composite key**, no «complex pk». |
| Variantes `…Duplex…` | `…<TRequest,TResponse>` sin sufijo | 🔴 Mismo razonamiento que en `WebServices`: la aridad genérica ya distingue, y «duplex» significa otra cosa. |
| Clases base **no `abstract`** | *(marcarlas `abstract`)* | 🟡 No es nombre, pero `…Base` es una promesa: si se puede instanciar, el nombre miente. |

### Helpers de clave primaria

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `GetPkValues<TEntity>(string[], Func<TEntity,object[]>)` | `ParseKeyValues<TEntity>(…)` | 🔴 `Get…` sugiere lectura de algo existente; el método **parsea** cadenas de la URL y convierte tipos, que es donde puede fallar. Con `Parse…` el llamador espera errores de formato. |
| `GetPkValues(this string ids, …)` | `ParseCompositeKey(this string route)` | 🔴 El parámetro se llama `ids` (plural) pero es **una** cadena con separadores: el nombre confunde sobre el formato esperado. |
| `GetPkValuesString` | `FormatCompositeKey` | 🔴 (1) `…String` describe el tipo de retorno, cosa que ya hace la firma; (2) `Get…` esconde que hay **formateo con cultura** por medio; (3) está **duplicado literalmente** en `WebControllers` y `HttpClients`, y ese es el contrato de serialización de claves compuestas: merece un nombre único en un sitio compartido. |
| `ConverterTo(string, Type)` | `ConvertToType(string value, Type targetType)` | 🔴 `Converter` es un **sustantivo** (una cosa que convierte) usado como nombre de acción; y `To` sin complemento no dice a qué. |
| `ConvertDateTime` | `ParseDateTimeInvariant` | 🔴 El nombre no avisa de lo más importante: hoy usa la **cultura del hilo actual**, así que el mismo texto se interpreta distinto según el servidor. Un nombre con `Invariant` documenta la decisión. |
| `GetPkValuesErrorMessage` | `BuildKeyParseErrorMessage` | 🟡 Coherencia con `ParseKeyValues`. |
| `AddWebControllers` (hoy no hace nada) | *(eliminar o implementar)* | 🟢 Un método de registro vacío hace creer que la configuración está hecha. |

### Atributos y metadatos

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `PkParameterAttribute` | `CompositeKeyParameterAttribute` | 🔴 Coherencia con «composite key». (Además hoy es **inerte**: no hay `IOperationFilter` que lo lea, así que no documenta nada en Swagger.) |
| `PkParameterAttribute.Description` (`= null!`) | `Description` como `string?` o requerido | 🟡 El nombre promete un texto; el valor por defecto es una mentira de nulabilidad. |
| Rutas `id-str/{id}` vs `{id}` | `{id}` en todos los verbos | 🔴 No es un identificador, pero es nomenclatura pública: el mismo recurso con dos formas de URL según el verbo obliga a leer el código del controlador para llamar a la API. |

### `WebControllers.Cache`

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.WebControllers.Cache` | `…AspNetCore.Controllers.OutputCaching` | 🔴 «Cache» a secas no dice si es *output cache*, *response cache*, memoria o distribuida. El nombre del mecanismo de .NET 8 es **output caching**. |
| `BypassHeader` / `"X-Bypass-Cache"` | `CacheBypassHeader` + constante compartida | 🟡 El nombre corto se pierde entre otros «header» y el literal debería vivir junto al resto de cabeceras `X-…`. |
| `PerControllerOutputCachePolicy` | `ControllerScopedOutputCachePolicy` | 🟡 «Per…» se lee como una unidad de medida; «scoped» es el adjetivo que ya usa ASP.NET Core para ámbitos. |

---

## Nivel 10 — `HttpClients`

Es el proyecto más grande y el que más nombres discutibles acumula, porque replica la
nomenclatura de `WebServices` sin que sean el mismo concepto.

### Proyecto y abstracciones

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` | 🔴 El plural sugiere una colección de clientes concretos; el proyecto ofrece **infraestructura** para construirlos. |
| `IGenClientFp<TDto>` | `IRestClientFp<TDto>` | 🔴 `Gen` no se entiende (¿generate? ¿generic? ¿general?) y no aporta: la genericidad ya la marca `<TDto>`. `Rest` dice qué protocolo y qué estilo. |
| `GenClientFp<TDto>` | `RestClientFp<TDto>` | 🔴 Ídem. |
| `IGenComplexClientFp<TDto>` | `ICompositeKeyRestClientFp<TDto>` | 🔴 **`Complex` no significa nada por sí solo.** Lo que hace especial a este cliente es que la entidad remota tiene **clave compuesta**. Además el orden `GenComplexClient` mezcla dos adjetivos: `CompositeKeyRestClient` se lee de corrido. |
| `GenComplexClientFp<TRequest,TResponse>` | `CompositeKeyRestClientFp<TRequest,TResponse>` | 🔴 Ídem. |
| Variantes `…Duplex…` | *(eliminar el sufijo; basta la aridad genérica)* | 🔴 Igual que en `WebServices` y `WebControllers`: hoy el mismo concepto («DTO de entrada ≠ DTO de salida») se llama `Duplex` en tres proyectos y en ninguno significa lo que significa en redes. |
| `IHttpClientFactoryManager` | `IHttpApiInvoker` | 🔴 Dos sufijos vacíos en cadena: «Factory» + «Manager». Y engaña: **no** es una fábrica de `HttpClient` (esa es `IHttpClientFactory` de .NET), sino el componente que **ejecuta** la llamada y deserializa la respuesta. |
| `IHttpClientFactoryManager<T, K>` | `IHttpApiInvoker<TRequest, TResponse>` | 🔴 Ver Nivel 1: `K` no dice nada y `T` sin rol tampoco. |
| `GetHttpClientFactoryKey()` | `GetClientName()` | 🔴 Lo que devuelve es el **nombre del cliente registrado** (`AddHttpClient("nombre")`); «FactoryKey» describe un detalle de implementación. Además hoy **no está en `IGenClientFp<>`**, así que el nombre solo existe en la clase concreta. |

### Métodos de llamada

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `PostGetAsync<T>` | `PostAndReadAsync<T>` | 🔴 `PostGet` se lee como «POST seguido de GET», que es exactamente lo que **no** hace: hace un POST y lee el cuerpo de la respuesta. |
| `PostGetPaginationAsync<TRequest,TEnumrableResponse>` | `PostAndReadPagedAsync<TRequest,TItem>` | 🔴 Errata en el genérico (Nivel 1) + el mismo problema de `PostGet` + `Pagination` (sustantivo) donde va un adjetivo (`Paged`). |
| `GetPaginationAsync<T>` | `GetPagedAsync<T>` | 🔴 Ídem, y coherencia con `PagedResult<T>` del Nivel 5. |
| `GetByIdAsync` / `PutByIdAsync` | *(mantener)* | ✅ Claros y simétricos… salvo que hoy construyen **rutas distintas** (`id-str/{id}` vs `{id}`), lo que hace que nombres simétricos oculten comportamientos asimétricos. |
| `DeleteByIdAsync` | *(mantener)* | ✅ El nombre está bien; el problema es que la versión duplex deserializa `TResponse` y la simplex `TDto` sin que se note en la firma. |
| `InternalGetUrl` | `BuildRequestUri` | 🔴 «Internal» ya lo indica el modificador `private`; y lo importante es que **construye una URI** (hoy con `Path.Combine`, que en Windows mete `\`). |
| `InternalPostGetAsync` | `SendAndReadAsync` | 🟡 Coherencia con el renombrado de `PostGetAsync`. |
| `SetHeaderInfo` / `SetHeaderPageNumber` / `SetHeaderPageSize` | `AddPaginationHeaders(HttpRequestMessage …)` | 🔴 Dos problemas: (1) `…Info` vacío; (2) **`Set…` sobre `client.DefaultRequestHeaders`** modifica un `HttpClient` del pool, afectando a otras peticiones. Un nombre que reciba el `HttpRequestMessage` hace evidente el ámbito correcto. |
| `ToResponseErrorsDescription` | `ReadErrorDescriptionAsync` | 🔴 Tres avisos en un nombre: (1) hoy usa `.Result` (bloqueo, riesgo de *deadlock*) y el nombre no lo delata; (2) hace **E/S**, así que necesita `Async`; (3) `To…` sugiere una conversión pura y barata. |
| `SetHeaders` | `TrySetHeaders` | 🟡 No comprueba `null`; el sufijo `Try…` obligaría a devolver `MlResult` y a documentar el fallo. |

### Records de petición

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| Carpeta `ParamsInfo/` | `Requests/` | 🔴 Ver Nivel 0. |
| `CallRequestParamsInfo` | `ApiCallOptions` | 🔴 **Tres sustantivos genéricos encadenados**: «call» + «request» + «params» + «info» y ninguno concreta. Lo que describe es el conjunto de opciones de una llamada. |
| `CallRequestPaginationParamsInfo` | `PagedApiCallOptions` | 🔴 Ídem; y así queda claro que es una especialización del anterior (hoy los dos *records* **no están relacionados por herencia** aunque el nombre lo sugiera). |
| `CallRequestParamsInfo<T>` vs `CallRequestParamsInfo` | `ApiCallOptions<TBody>` : `ApiCallOptions` | 🔴 Mismo nombre con y sin genérico para tipos **sin relación** es la trampa de nomenclatura más fácil de sufrir: el `using` compila y el comportamiento cambia. |
| `Headers` (`Dictionary<string,string>? Headers = null!`) | `Headers` como `IReadOnlyDictionary<string,string>?` | 🟡 El nombre está bien; `null!` sobre un tipo ya anulable es una contradicción que confunde al lector. |
| `params object[] pk` (tras parámetros opcionales) | `object[] keyValues` | 🔴 `params` detrás de opcionales hace que la llamada se resuelva de forma sorprendente; y `pk` en singular para un array de valores. |

### Registro

| Nombre actual | Nombre propuesto | Por qué |
|---|---|---|
| `AddGenClientFp` | `AddRestClient` | 🔴 Coherencia con `RestClientFp`, y en un método de extensión de DI el sufijo `Fp` no aporta. |
| `AddGenClientComplexFp` | `AddCompositeKeyRestClient` | 🔴 Además el orden actual (`Client` + `Complex`) está invertido respecto al nombre de la clase (`GenComplexClient`): dos nombres para lo mismo. |
| `AddGenClientDuplexComplexFp` | `AddCompositeKeyRestClient<TRequest,TResponse>` | 🔴 «Duplex» + «Complex» + «Fp» en un solo nombre: imposible de recordar y de autocompletar. |
| *(no existe registro para `IGenComplexClientFp<>`)* | `AddCompositeKeyRestClient` debe cubrirlo | 🟢 No es un renombrado, pero el hueco se detecta justo al ordenar los nombres: hay una interfaz pública sin forma de registrarse. |
| `Key httpClientFactoryKey` (parámetro) | `string clientName` | 🔴 Coherencia con `GetClientName()` y con `IHttpClientFactory.CreateClient(string name)`. (El parámetro actual además se inicializa a `null!` y nunca recibe valor: bug real.) |
| `configureHttpClientKey` | `configureClient` | 🟡 El delegado configura el cliente, no la clave. |

---

## Guía de estilo resultante

Si aplicas los renombrados anteriores, la biblioteca queda con estas reglas. Merece la pena
copiarlas a un `.editorconfig` o a un `CONTRIBUTING.md` para que las nuevas incorporaciones no
vuelvan a abrir la puerta a `Gen…`, `…Plus` o `…Info`.

### Sufijos con significado fijo

| Sufijo | Significado exacto | Ejemplo correcto | Ejemplo a evitar |
|---|---|---|---|
| `…Async` | Devuelve `Task`/`ValueTask`. **Siempre** que lo devuelva, sin excepción. | `TryFindAsync` | `AllAsync` que en realidad es sincrónico |
| `Try…` | No lanza: el fallo viaja en el `MlResult`. | `TryMap`, `TryBind` | `Try…` que además lanza en el `null` check |
| `…Fp` | Variante funcional cuando **coexiste** con una imperativa que lanza. | `EnsureFp` | `…Fp` en un proyecto donde no hay variante imperativa |
| `I…` | Interfaz. | `IRestClientFp` | — |
| `…Base` | Clase base `abstract`. Si no es `abstract`, sobra el sufijo. | `MlCrudControllerBase` | base instanciable |
| `…Extensions` | Clase `static` solo con métodos de extensión. | `MlResultActionResultExtensions` | `…Helpers` con extensiones dentro |
| `…Options` | Datos de configuración de una operación, sin lógica. | `ApiCallOptions` | `CallRequestParamsInfo` |
| `…Result` / `…PagedResult` | Salida de una operación. | `PagedResult<T>` | `…Info` para lo mismo |

### Sufijos y prefijos prohibidos

| Prohibido | Motivo | Qué usar |
|---|---|---|
| `…Info` | No dice nada: **todo** es información. | El sustantivo real (`…Options`, `…Result`, `…Descriptor`) |
| `…Plus` | Solo significa «el nombre bueno estaba ocupado». | Resolver el conflicto de nombres |
| `…Helper` / `…Utils` / `…Manager` | Cajón de sastre: atrae código sin dueño. | Nombrar la responsabilidad concreta |
| `Gen…` | Abreviatura ambigua; la genericidad ya la marca `<T>`. | El dominio (`Crud…`, `Rest…`) |
| `Simple…` / `Complex…` | Juicios de valor relativos. | El criterio real (`CompositeKey…`) |
| `…Duplex…` | Significa otra cosa en comunicaciones. | La aridad genérica `<TRequest,TResponse>` |
| `Oop…` | Toda la biblioteca es OOP. | El comportamiento (`Throwing…`) |
| `…Internals` como nombre de proyecto | No es un ámbito, es un `internal`. | `Shared` |

### Parámetros genéricos

| Rol | Nombre | Nunca |
|---|---|---|
| Entidad de dominio / persistencia | `TEntity` | `T`, `TE` |
| DTO de ida y vuelta | `TDto` | `T` |
| DTO de entrada | `TRequest` | `T`, `TIn` |
| DTO de salida | `TResponse` | `K`, `TOut`, `TEnumrableResponse` |
| Clave | `TKey` | `TPk`, `K` |
| Valor de retorno de una proyección | `TResult` / `TReturn` (uno de los dos, no ambos) | `TRet` |
| Elemento de una colección | `TItem` | `TEnumerable` |

### Otras reglas

- **Booleanos:** `Is…` para estado (`IsValid`), `Has…` para posesión (`HasExceptionDetails`),
  `Can…` para capacidad. Nunca un sustantivo suelto (`Valid`).
- **Fábricas estáticas:** un solo verbo para todo el repositorio. Se propone `From…` para
  conversión de un valor (`FromString`, `FromInt`) y `Create…` para composición de varios.
  Hoy conviven `By…`, `From…` y `Build…` para lo mismo.
- **Constantes:** `PascalCase` (`ExceptionDetailKey`), no `SCREAMING_SNAKE_CASE`
  (`EX_DESC_KEY`), que es estilo C/C++.
- **Campos privados:** `_camelCase`. Nunca `public static` mutable (ver `IntNotNegative.limit`).
- **Colecciones:** plural (`Errors`, `Details`). Un plural en un tipo que **no** es colección
  (`MlResultBucles`, `MlProblemsDetails`) es una pista falsa.
- **Un solo idioma en el código:** inglés en identificadores, mensajes y XML docs; español solo
  en la documentación `.md`. Hoy quedan `Bucles`, `ContieneCombinacion` y `Catas` en el código.

---

## Cómo aplicar los cambios sin romper a nadie

La biblioteca está publicada, así que un renombrado masivo en un solo commit obligaría a todos
los consumidores a reescribir su código de golpe. El camino recomendado:

### 1. Un nivel por Pull Request

Los niveles de este documento están ordenados de menos a más invasivos a propósito:

- **Nivel 0** (carpetas y proyectos) y **Nivel 1** (erratas internas) no cambian ninguna firma
  pública que un consumidor esté usando de forma consciente. Empieza por ahí.
- Los niveles 2 a 10 sí tocan API pública: uno por PR, con su propia entrada en el changelog.

### 2. Puentes con `[Obsolete]` para todo lo marcado 🔴

Para cada miembro público renombrado, deja un reenvío durante **una versión mayor completa**:

```csharp
// Nombre nuevo: implementación real.
public static MlResult<TResult> ToCreatedProblemDetailsResult<TResult>(this MlResult<TResult> source)
    => /* … */;

// Nombre antiguo: solo reenvía. Se elimina en la siguiente mayor.
[Obsolete("Renombrado a ToCreatedProblemDetailsResult. Se eliminará en la versión 3.0.", error: false)]
public static MlResult<TResult> ToPostPdActionResult<TResult>(this MlResult<TResult> source)
    => source.ToCreatedProblemDetailsResult();
```

Para tipos y interfaces no hay reenvío posible con `class`, pero sí con `using` global o con una
clase parcial derivada marcada `[Obsolete]`. En la práctica: **los renombrados de tipo van todos
juntos en la misma versión mayor**.

### 3. Versionado

- Nivel 0 y 1 → versión **menor** (`1.0.14` → `1.1.0`).
- Cualquier nivel con 🔴 → versión **mayor** (`1.x` → `2.0.0`), con las erratas antiguas todavía
  presentes y marcadas `[Obsolete]`.
- Eliminación de los `[Obsolete]` → siguiente mayor (`3.0.0`).

### 4. Orden mecánico dentro de cada PR

1. Renombrar con el refactor del IDE (`F2`), **no** con buscar y reemplazar: así se actualizan
   también los `nameof`, los XML docs y las referencias entre proyectos.
2. Añadir los puentes `[Obsolete]`.
3. Ejecutar `dotnet format` para que el estilo no ensucie el diff.
4. Compilar la solución completa: los proyectos de test son el mejor detector de firmas rotas.
5. Ejecutar los proyectos de pruebas (`*.Tests.Unit`, `*.Integration.Tests`).
6. **Actualizar la documentación en el mismo PR**: `README.md` del proyecto tocado y los
   ficheros de `MoralesLarios.FOOP/__Doc/` que mencionen el nombre antiguo. Si la documentación
   se queda atrás, el renombrado es peor que no hacerlo.
7. Anotar el renombrado en el changelog con las dos columnas: antes → después.

### 5. Qué no hacer

- ❌ No renombrar y cambiar comportamiento en el mismo commit: si algo se rompe, no sabrás qué fue.
- ❌ No tocar los nombres de la familia `…WithValue` / `…WithoutValue` en el mismo PR que el resto
  de `Map`/`Bind`: son muchísimas sobrecargas y el diff se vuelve ilegible. Merecen su propio PR.
- ❌ No aprovechar el renombrado para «colar» arreglos de bugs. Esos van en
  [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) y tienen su propia
  prioridad.

---

## Checklist de ejecución

Marca cada nivel cuando esté cerrado (renombrado + `[Obsolete]` + docs actualizadas + tests en verde).

- [ ] **Nivel 0** — Carpeta `MoralesLarios.FOOP\` → `MoralesLarios.OOFP\`, `.sln` duplicados
      eliminados, proyectos y carpetas renombrados, ficheros vacíos borrados.
- [ ] **Nivel 1** — Erratas ortográficas corregidas con `[Obsolete]` en las públicas
      (`TryMapIAsyncf`, `Alwais`, `Lenght`, `Bydouble`, `Scopedt`, `TEnumrableResponse`,
      `BaseAdress`, `soported`, `diferent`, `Bucles`).
- [ ] **Nivel 2** — Núcleo: `IsValid`/`IsFail`, `MlErrorsDetails`, constantes en `PascalCase`,
      sufijos de familia revisados.
- [ ] **Nivel 3** — `ValueObjects` y `ValueObjects.IO`: tipos de nombre corto con contexto,
      fábricas homogéneas, `EndpointPattern` renombrado.
- [ ] **Nivel 4** — `Validation`, `DataAnnotations` (con la A mayúscula) y `FluentValidations`.
- [ ] **Nivel 5** — `Shared`, `FileSystem`, `Configuration` y `Logging`.
- [ ] **Nivel 6** — `EntityFrameworkCore`: familias de repositorio, `SortDirection`,
      `GetRepository`, `keySelector`.
- [ ] **Nivel 7** — `Application`: `ICrudServiceFp`, `MlProblemDetails`, sin `Duplex`.
- [ ] **Nivel 8** — `AspNetCore.Results`: clase `…Plus` retirada, `ToCreatedProblemDetailsResult`,
      cabeceras de paginación como constantes compartidas, `ContainsAnyKey`.
- [ ] **Nivel 9** — `AspNetCore.Controllers`: `MlCrudControllerBase`,
      `MlCompositeKeyCrudControllerBase`, `ParseKeyValues`, `ConvertToType`, rutas homogéneas.
- [ ] **Nivel 10** — `Http`: `IRestClientFp`, `IHttpApiInvoker`, `PostAndReadAsync`,
      `ApiCallOptions`, `clientName`.
- [ ] **Cierre** — Reglas de la [guía de estilo](#guía-de-estilo-resultante) escritas en
      `CONTRIBUTING.md`, `__Doc/` sin referencias a nombres antiguos, y los `[Obsolete]` del
      ciclo anterior eliminados.

---

📌 **Recordatorio final:** este documento es una propuesta de lectura, no un mandato. Si algún
nombre actual está ya muy asentado entre tus consumidores, el coste de romperlo puede superar la
mejora de legibilidad. En ese caso, deja el nombre y **documenta el motivo**: un nombre imperfecto
explicado es mejor que un nombre perfecto que rompe a todo el mundo.

Ver también:

- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — bugs y problemas
  de seguridad ordenados por prioridad.
- [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) — rendimiento, contratos y coherencia.
- [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — limpieza, erratas y documentación.
- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) —
  higiene del repositorio, analizadores, testing y CI/CD.
- [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md) —
  superficie pública, asincronía, observabilidad y hoja de ruta.
- [`README.md`](README.md) — índice de la carpeta y plan de trabajo por bloques.
- [`../README.md`](../README.md) — visión general de la solución.
- [`../MoralesLarios.FOOP/__Doc/1_Intro.md`](../MoralesLarios.FOOP/__Doc/1_Intro.md) —
  convenciones de nomenclatura **actuales** del núcleo (`[Try][Operación][Contexto][Async]`).
