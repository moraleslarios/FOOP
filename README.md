# MoralesLarios.FOOP / MoralesLarios.OOFP

**MoralesLarios.FOOP** es una plataforma modular de librerías .NET construida sobre una base funcional común.
Su centro técnico es **`MoralesLarios.OOFP`**, el núcleo sobre el que se apoyan el resto de proyectos: validación, persistencia, servicios de aplicación, controladores web, caché, clientes HTTP, logging, IO, value objects y utilidades de infraestructura.

El objetivo del ecosistema es ofrecer una forma homogénea de trabajar con:

- `MlResult<T>` como contenedor de éxito/error
- composición funcional con `Bind`, `Map`, `Match` y `ExecSelf`
- manejo explícito de errores sin usar excepciones como flujo principal de control
- integración natural con ASP.NET Core, EF Core y DI
- tipos seguros mediante value objects y validación dedicada
- documentación técnica extensa y enlazada por módulos

---

## Visión general

Esta solución está pensada para proyectos que quieran combinar:

- núcleo funcional
- validación de dominio
- persistencia segura
- exposición web limpia
- consumo HTTP tipado
- caché por controlador
- logging funcional
- configuración e IO seguras

El resultado es una arquitectura en capas, consistente y reutilizable, donde cada proyecto aporta una pieza concreta del ecosistema.

---

## Cómo navegar esta solución

### 🚀 Punto de entrada recomendado

Si es tu primera vez, sigue este orden:

1. **[Introducción general al núcleo `MoralesLarios.OOFP`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md)** — filosofía, arquitectura y convención de nombres.
2. **[README técnico del proyecto núcleo](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/README.md)** — catálogo completo de la API con ejemplos.
3. **[Índice de documentación por tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/README.md)** — un documento por archivo de código.
4. **[README de la solución completa](https://github.com/moraleslarios/FOOP/blob/main/src/README.md)** — visión de todas las capas y proyectos.

> 💡 La documentación técnica del núcleo vive en `src/MoralesLarios.FOOP/__Doc/` y está escrita en español, con ejemplos ejecutables y secciones de "qué no hacer".

### Núcleo OOFP — referencia por archivo de código

| Documento | Contenido |
|---|---|
| [`MlResult`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResult.md) | El tipo raíz, fábricas y conversiones implícitas |
| [Modelo de errores](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md) | `MlError`, `MlErrorsDetails` y sus extensiones |
| [`MlResultActions`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActions.md) | Enriquecer errores, transportar datos, acceso seguro |
| [Operaciones `Bind`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsBind.md) | Todas las sobrecargas de `Bind*` |
| [Operaciones `Map`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsMap.md) | Todas las sobrecargas de `Map*` |
| [Operaciones `Match`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsMatch.md) | Todas las sobrecargas de `Match*` |
| [Operaciones `ExecSelf`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsExecSelf.md) | Todas las sobrecargas de `ExecSelf*` |
| [Operaciones `Several`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsSeveral.md) | `EmptyToFailed`, `NullToFailed`, `BoolToResult`, `Combine`, `Do` |
| [Detalles del error](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsErrorsDetails.md) | Leer y fusionar el diccionario `Details` |
| [Bucles funcionales](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultBucles.md) | `Projection*`, `ProjectionSplit*`, `Fusion*` |
| [Transformaciones](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultTransformations.md) | `ToMlResult*`, `TryToMlResult*`, boxing |
| [Cambio de tipo de retorno](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultChangeReturnResult.md) | Familia `ChangeReturnResult*` |

### Núcleo OOFP — guías por concepto

| Familia | Documentos |
|---|---|
| **Introducción** | [Intro general](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md) |
| **`Match`** | [`Match`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Match/1_Match.md) · [`MatchAll`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Match/2_MatchAll.md) |
| **`Bind`** | [`Bind`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) y el resto de la carpeta `Bind/` |
| **`Map`** | [`Map`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Map/1_Map.md) y el resto de la carpeta `Map/` |
| **`ExecSelf`** | [`ExecSelf`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/ExecSelf/1_ExecSelf.md) y el resto de la carpeta `ExecSelf/` |
| **`Several`** | [`EmptyToFailed`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Several/1_EmptyToFailed.md) · [`NullToFailed`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Several/2_NullToFailed.md) · [`BoolToResult`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Several/3_BoolToResult.md) · [`Combine`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Several/4_Combine.md) |
| **Precondiciones** | [`EnsureFp` (índice)](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) · [Núcleo](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/1_EnsureFpCore.md) · [Agregación](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/2_EnsureFpAggregation.md) · [Cadenas](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/3_EnsureFpStrings.md) · [Números](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/4_EnsureFpNumbers.md) · [Colecciones](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/5_EnsureFpCollections.md) · [Tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/6_EnsureFpTypes.md) · [Nullables](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/7_EnsureFpNullables.md) · [Asíncronas](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/8_EnsureFpAsync.md) · [Mensajes](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/9_EnsureFpMessages.md) |
| **Utilidades** | [Extensions](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Extensions/Extensions.md) · [Transformations](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Transformations/Transformations.md) · [Bucles](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) |

---

## Qué aporta la librería principal `MoralesLarios.OOFP`

`MoralesLarios.OOFP` define el lenguaje común del ecosistema. Su idea principal es que la aplicación trabaje con resultados explícitos, en lugar de encadenar excepciones como mecanismo primario de control.

### Principios de diseño

- **Railway-Oriented Programming**: cada operación puede continuar por la vía de éxito o desviarse a la vía de error.
- **Composición funcional**: los métodos se encadenan de forma fluida y predecible.
- **Errores como datos**: el error no se oculta; se transporta, se inspecciona y se transforma.
- **Asincronía segura**: existe soporte coherente para versiones `Async` y variantes que capturan excepciones.
- **Extensibilidad**: el sistema está construido por familias de extensiones y tipos reutilizables.

### Convenciones de nombres

El proyecto sigue una convención uniforme:

- `Bind*`: encadena operaciones que ya devuelven `MlResult`
- `Map*`: transforma valores puros dentro de un resultado válido
- `Match*`: ramifica según `valid` o `fail`
- `ExecSelf*`: ejecuta efectos secundarios y devuelve el mismo resultado
- `Try*`: captura excepciones y las convierte en fallos funcionales
- `*Async`: versión asíncrona

Esa convención se repite en todo el ecosistema para que el comportamiento sea predecible.

### Ideas clave del núcleo

- `MlResult<T>` es el tipo base de éxito/error. Se consulta con `IsValid` / `IsFail`.
- `MlErrorsDetails` transporta el detalle estructurado del error: la lista `Errors` y el diccionario `Details`.
- `EnsureFp` aporta más de 90 precondiciones funcionales agrupadas en ocho familias (núcleo, agregación, cadenas, números, colecciones, tipos concretos, `Nullable<T>` y asíncronas). Cada regla existe en tres variantes: con mensaje `string`, con `MlErrorsDetails` completo y con sufijo `…Arg`, que deduce el nombre del parámetro con `[CallerArgumentExpression]` y genera el mensaje automáticamente. Además, `All` acumula y devuelve todos los errores de validación de una vez, y `TryThat` captura las excepciones que lance el predicado.
- Las extensiones de `Types` cubren composición, transformación, coincidencia y cambio de forma del resultado.

> ⚠️ **Regla de oro**: el valor interno (`Value`) y el detalle de errores (`ErrorsDetails`) son `internal protected`.
> Desde código consumidor **no se accede directamente**: se usa `Match(valid: …, fail: …)` o `SecureValidValue()`.

---

## Mapa del ecosistema

### Resumen de proyectos principales

| Proyecto | Propósito | Documentación |
|---|---|---|
| `MoralesLarios.OOFP` | Núcleo funcional de la solución | [Intro](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md) · [Tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/README.md) · [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/README.md) |
| `MoralesLarios.OOFP.ValueObjects` | Value objects tipados y validados | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects/README.md) |
| `MoralesLarios.OOFP.ValueObjects.IO` | Value objects para rutas y sistema de archivos | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects.IO/README.md) |
| `MoralesLarios.OOFP.Validation` | Base de validación funcional | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation/README.md) |
| `MoralesLarios.OOFP.Validation.Dataannotations` | Validación con DataAnnotations | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| `MoralesLarios.OOFP.Validation.FluentValidations` | Validación con FluentValidation | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| `MoralesLarios.OOFP.Shared` | Constantes compartidas entre proyectos (sin dependencias) | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Shared/README.md) |
| `MoralesLarios.OOFP.Internals` | Tipos internos compartidos y paginación | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md) |
| `MoralesLarios.OOFP.Extensions.Loggers` | Logging funcional sobre `MlResult<T>` | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md) |
| `MoralesLarios.OOFP.Utilities` | Lectura segura de configuración | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Utilities/README.md) |
| `MoralesLarios.OOFP.IO` | IO seguro sobre ficheros y directorios | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md) |
| `MoralesLarios.OOFP.EFCore` | Repositorios funcionales y OOP sobre EF Core | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md) |
| `MoralesLarios.OOFP.WebServices` | Servicios de aplicación funcionales | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md) |
| `MoralesLarios.OOFP.WebApi` | Puente entre `MlResult<T>` e `IActionResult` | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md) |
| `MoralesLarios.OOFP.WebControllers` | Controladores REST genéricos | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md) |
| `MoralesLarios.OOFP.WebControllers.Cache` | Controladores REST con caché por controlador | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers.Cache/README.md) |
| `MoralesLarios.OOFP.HttpClients` | Clientes HTTP tipados y funcionales | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md) |
| `MoralesLarios.OOFP.EFCore.WebApi` | Base de integración entre EF Core y Web API | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md) |

### Proyectos de pruebas y verificación

La solución incluye distintos proyectos de pruebas e integración que sirven como validación viva del ecosistema:

- `MoralesLarios.OOFP.Unit.Tests`
- `MoralesLarios.OOFP.ValueObjects.Tests.Unit`
- `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit`
- `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit`
- `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit`
- `MoralesLarios.OOFP.WebApi.Tests.Unit`
- `MoralesLarios.OOFP.WebServices.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Integration`
- `MoralesLarios.OOFP.EFCore.Infrastructure.Tests`
- `MoralesLarios.OOFP.EFCore.Integration.Tests`
- `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests`

---

## La pieza central: `MoralesLarios.OOFP`

Aunque toda la solución tiene valor por sí misma, **`MoralesLarios.OOFP` es el fundamento común**.

### Qué resuelve

- abstrae el patrón `Result`
- unifica el tratamiento de errores
- permite composición funcional sin pérdida de contexto
- proporciona la base para logging, validación, persistencia y web

### Qué encontrarás en su documentación técnica

- [Introducción general](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md)
- [Guía por tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/README.md)
- [Detalles de `MlResult<T>`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [Operaciones de `Bind`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsBind.md)
- [Operaciones de `Map`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsMap.md)
- [Operaciones de `Match`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones de `ExecSelf`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones de `Several`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultBucles.md)
- [Cambio de retorno](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultChangeReturnResult.md)
- [Errores y detalles](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)

### Por qué es importante

Porque el resto de proyectos reutilizan el mismo estilo y semántica:

- `ValueObjects` usa `MlResult<T>` para crear y validar tipos seguros.
- `Validation` transforma validaciones en resultados funcionales.
- `EFCore` encapsula operaciones de base de datos en resultados.
- `WebServices` expone la lógica de aplicación en la misma semántica.
- `WebApi` convierte esos resultados en respuestas HTTP.
- `HttpClients` consume esas respuestas con la misma filosofía.

---

## Capas de la solución

### Dominio, semántica y tipos seguros

#### `MoralesLarios.OOFP.ValueObjects`
Librería de value objects tipados para evitar el uso de primitivos sin semántica.

Aporta, entre otros:

- `NotEmptyString`
- `Key`
- `Mail`
- `IntNotNegative`
- value objects numéricos y de texto

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects/README.md)

#### `MoralesLarios.OOFP.ValueObjects.IO`
Especialización de value objects para rutas y filesystem.

Aporta:

- `MlFile`
- `MlDirectory`
- `ExistsFile`
- `ExistDirectory`

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects.IO/README.md)

#### `MoralesLarios.OOFP.Validation`
Base de validación funcional con `MlValidableFp<T>`.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation/README.md)

#### `MoralesLarios.OOFP.Validation.Dataannotations`
Extiende la validación funcional con atributos de `DataAnnotations`.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)

#### `MoralesLarios.OOFP.Validation.FluentValidations`
Extiende la validación funcional con `FluentValidation`.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)

---

### Infraestructura común

#### `MoralesLarios.OOFP.Internals`
Tipos internos reutilizables, especialmente para paginación y metadatos compartidos.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md)

#### `MoralesLarios.OOFP.Extensions.Loggers`
Extensiones para registrar trazas sobre `MlResult<T>` sin romper el flujo funcional.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md)

#### `MoralesLarios.OOFP.Utilities`
Lectura segura de configuración y connection strings con `MlResult<T>`.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Utilities/README.md)

#### `MoralesLarios.OOFP.IO`
Wrapper funcional sobre `System.IO` para ficheros y directorios.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md)

---

### Persistencia

#### `MoralesLarios.OOFP.EFCore`
Capa de repositorios EF Core en dos estilos:

- funcional (`*Fp`), devolviendo `MlResult<T>`
- OOP clásico

Soporta:

- CRUD completo
- búsqueda por PK simple o compuesta
- paginación
- consultas posicionales
- registro masivo por DI

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md)

#### `MoralesLarios.OOFP.EFCore.WebApi`
Proyecto de integración entre EF Core y Web API.

Actualmente es una base/skeleton para extender con lógica de aplicación específica.

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md)

---

### Servicios de aplicación

#### `MoralesLarios.OOFP.WebServices`
Capa funcional entre repositorio y web.

Aporta:

- `IGenServiceFp<TEntity, TDto>`
- `IGenServiceFp<TEntity, TRequest, TResponse>`
- `GenServiceFp<TEntity, TDto>`
- `GenServiceFp<TEntity, TRequest, TResponse>`
- `MlProblemsDetails`
- extensiones de registro para ciclo de vida clásico y duplex

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md)

---

### Exposición web

#### `MoralesLarios.OOFP.WebApi`
Puente funcional entre `MlResult<T>` e `IActionResult`.

Aporta:

- `MlActionResults`
- `ExtendedProblemDetails`
- `ProblemDetailsInfo`
- `MlResultWebExtensionsPlus`
- `MlErrorsDetailsExtensions`
- helpers para headers del request

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores genéricos ASP.NET Core para CRUD estándar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar parámetros PK en Swagger/OpenAPI

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensión cacheada de los controladores genéricos.

Aporta:

- caché por controlador
- invalidación automática en escrituras
- vaciado manual
- bypass dinámico
- soporte clásico y duplex
- soporte para PK compuesta

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers.Cache/README.md)

---

### Consumo HTTP

#### `MoralesLarios.OOFP.HttpClients`
Cliente HTTP funcional integrado con `MlResult<T>` y `IHttpClientFactory`.

Aporta:

- clientes tipados con PK simple
- clientes duplex request/response
- clientes para PK compuesta
- manager funcional sobre `IHttpClientFactory`
- helpers de cabeceras y respuestas HTTP

📄 [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)

---

## End-to-end: cómo usar el ecosistema

### Caso típico

1. Modela el dominio con `ValueObjects`.
2. Valida con `Validation`.
3. Persiste con `EFCore`.
4. Expón la lógica con `WebServices`.
5. Publica con `WebControllers` y `WebApi`.
6. Añade `WebControllers.Cache` si necesitas caché.
7. Consume desde otro servicio con `HttpClients`.
8. Registra trazas con `Extensions.Loggers`.
9. Lee configuración con `Utilities`.
10. Usa `IO` y `ValueObjects.IO` para operaciones de sistema de archivos.

### Ejemplo conceptual

```csharp
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
services.AddTransient(typeof(IEFRepoFp<>), typeof(EFRepoFp<>));
services.AddTransientGenServicesFpWithoutReposGeneral();
services.AddControllers();
```

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController(IGenServiceFp<User, UserDto> service)
    : SimpleMlControllerBase<User, UserDto, int>(service) { }
```

```csharp
builder.Services.AddHttpClientsFp();
builder.Services.AddGenClientFp<IUsersClient, UsersClient>(
    configureClient: c => c.BaseAddress = new Uri("https://api.example.com/api/users/"));
```

---

## Proyectos de pruebas

La solución también incluye proyectos de pruebas unitarias e integración que funcionan como verificación viva del comportamiento:

- `MoralesLarios.OOFP.Unit.Tests`
- `MoralesLarios.OOFP.ValueObjects.Tests.Unit`
- `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit`
- `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit`
- `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit`
- `MoralesLarios.OOFP.WebApi.Tests.Unit`
- `MoralesLarios.OOFP.WebServices.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Unit`
- `MoralesLarios.OOFP.HttpClients.Tests.Integration`
- `MoralesLarios.OOFP.EFCore.Infrastructure.Tests`
- `MoralesLarios.OOFP.EFCore.Integration.Tests`
- `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests`

Estos proyectos sirven para verificar contratos, ejemplos reales de uso y escenarios de integración entre capas.

---

## Documentación adicional

### Documentación raíz del núcleo OOFP

- [Intro general y filosofía técnica](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md)
- [Documentación por tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/README.md)
- [Tipos y resultados](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [Bind](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [Map](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [Match](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [ExecSelf](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp (índice de la familia)](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) — y sus nueve páginas: [Núcleo](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/1_EnsureFpCore.md), [Agregación](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/2_EnsureFpAggregation.md), [Cadenas](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/3_EnsureFpStrings.md), [Números](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/4_EnsureFpNumbers.md), [Colecciones](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/5_EnsureFpCollections.md), [Tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/6_EnsureFpTypes.md), [Nullables](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/7_EnsureFpNullables.md), [Asíncronas](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/8_EnsureFpAsync.md), [Mensajes](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/EnsureFp/9_EnsureFpMessages.md)
- [Extensions](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Extensions/Extensions.md)
- [Transformations](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Transformations/Transformations.md)
- [Bucles](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)

### README de cada proyecto

- [MoralesLarios.OOFP (núcleo)](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/README.md)
- [MoralesLarios.OOFP.EFCore](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Shared](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Shared/README.md)
- [MoralesLarios.OOFP.Utilities](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md)

---

## Resumen ejecutivo

Si tuviera que describir la solución en una sola frase, sería esta:

> **MoralesLarios.FOOP es un ecosistema .NET funcional para construir dominios, servicios, APIs y clientes con una semántica común basada en `MlResult<T>`.**

Y si tuviera que destacar una sola pieza, esa sería:

> **`MoralesLarios.OOFP` es el núcleo fundacional; el resto de proyectos amplían su valor hacia validación, persistencia, web, caché, HTTP, IO y configuración.**

---

## Compatibilidad

La solución está organizada para proyectos objetivo de:

- `.NET 9`
- `.NET 8`

---

## Licencia y estilo de trabajo

La solución está pensada para crecer por capas, manteniendo una misma forma de trabajo en todo el stack.

Si buscas una entrada rápida para entender la librería, empieza por:

1. [Intro general de `MoralesLarios.OOFP`](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/1_Intro.md)
2. [Documentación por tipos](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.FOOP/__Doc/Types/README.md)
3. [WebServices](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md)
4. [WebApi](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md)
5. [WebControllers](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md)
6. [HttpClients](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)

---

## Nota final

Este repositorio no es una única librería aislada, sino una **plataforma modular**. Cada proyecto tiene su propio README y, cuando aplica, su propia documentación técnica enlazada desde `src/MoralesLarios.FOOP/__Doc`.

La documentación raíz pretende ser la puerta de entrada oficial al ecosistema completo.
