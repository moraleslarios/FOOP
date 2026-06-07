# MoralesLarios.FOOP / MoralesLarios.OOFP

**MoralesLarios.OOFP** es una solución modular de librerías .NET diseñada para construir software con un enfoque **funcional, consistente y reutilizable**.

La pieza central es la librería **`MoralesLarios.OOFP`**, sobre la que se apoyan el resto de proyectos: validación, persistencia, servicios de aplicación, Web API, controladores REST, caché, clientes HTTP, logging, IO y utilidades de infraestructura.

El objetivo del ecosistema es ofrecer una forma homogénea de trabajar con:

- `MlResult<T>` como contenedor de éxito/error
- composición funcional con `Bind`, `Map`, `Match` y `ExecSelf`
- manejo explícito de errores sin depender de excepciones como flujo de control
- integración natural con ASP.NET Core, EF Core y DI
- tipado fuerte mediante value objects y validaciones dedicadas
- documentación técnica extensa y enlazada por módulos

---

## Cómo navegar esta solución

### Documentación principal del núcleo OOFP

- [Documentación técnica completa de `MoralesLarios.OOFP`](./__Doc/1_Intro.md)
- [Documentación por tipos de `MoralesLarios.OOFP`](./__Doc/Types/README.md)
- [Modelo de resultados y tipos base](./__Doc/Types/MlResult.md)
- [Operaciones `Bind`](./__Doc/Types/MlResultActionsBind.md)
- [Operaciones `Map`](./__Doc/Types/MlResultActionsMap.md)
- [Operaciones `Match`](./__Doc/Types/MlResultActionsMatch.md)
- [Operaciones `ExecSelf`](./__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones `Several`](./__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](./__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](./__Doc/Types/MlResultBucles.md)
- [Cambio de tipo de retorno](./__Doc/Types/MlResultChangeReturnResult.md)
- [Modelo de errores](./__Doc/Types/MlResultErrors.md)

### Documentación por concepto dentro de `__Doc`

- [Intro general](./__Doc/1_Intro.md)
- [Bind](./__Doc/Bind/3_Bind.md)
- [Map](./__Doc/Map/1_Map.md)
- [Match](./__Doc/Match/1_Match.md)
- [ExecSelf](./__Doc/ExecSelf/1_ExecSelf.md)
- [Several](./__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](./__Doc/EnsureFp/EnsureFp.md)
- [Extensions](./__Doc/Extensions/Extensions.md)
- [Transformations](./__Doc/Transformations/Transformations.md)
- [Bucles](./__Doc/Bucle/Bucles.md)

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

- `MlResult<T>` es el tipo base de éxito/error.
- `MlErrorsDetails` transporta el detalle estructurado del error.
- `EnsureFp` aporta precondiciones funcionales.
- Las extensiones de `Types` cubren composición, transformación, coincidencia y cambio de forma del resultado.

---

## Mapa del ecosistema

### Paquetes principales

| Proyecto | Rol en la solución | Documentación |
|---|---|---|
| `MoralesLarios.OOFP` | Núcleo funcional de toda la solución | [__Doc/1_Intro.md](./__Doc/1_Intro.md) · [__Doc/Types](./__Doc/Types/README.md) |
| `MoralesLarios.OOFP.ValueObjects` | Value objects tipados y validados | [README](./src/MoralesLarios.OOFP.ValueObjects/README.md) |
| `MoralesLarios.OOFP.ValueObjects.IO` | Value objects para rutas y sistema de archivos | [README](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md) |
| `MoralesLarios.OOFP.Validation` | Base de validación funcional | [README](./src/MoralesLarios.OOFP.Validation/README.md) |
| `MoralesLarios.OOFP.Validation.Dataannotations` | Validación con DataAnnotations | [README](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| `MoralesLarios.OOFP.Validation.FluentValidations` | Validación con FluentValidation | [README](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| `MoralesLarios.OOFP.Internals` | Tipos internos compartidos y paginación | [README](./src/MoralesLarios.OOFP.Internals/README.md) |
| `MoralesLarios.OOFP.Extensions.Loggers` | Logging funcional sobre `MlResult<T>` | [README](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md) |
| `MoralesLarios.OOFP.Utilities` | Lectura segura de configuración | [README](./src/MoralesLarios.OOFP.Utilities/README.md) |
| `MoralesLarios.OOFP.IO` | IO seguro sobre ficheros y directorios | [README](./src/MoralesLarios.OOFP.IO/README.md) |
| `MoralesLarios.OOFP.EFCore` | Repositorios funcionales y OOP sobre EF Core | [README](./src/MoralesLarios.OOFP.EFCore/README.md) |
| `MoralesLarios.OOFP.WebServices` | Servicios de aplicación funcionales | [README](./src/MoralesLarios.OOFP.WebServices/README.md) |
| `MoralesLarios.OOFP.WebApi` | Puente entre `MlResult<T>` e `IActionResult` | [README](./src/MoralesLarios.OOFP.WebApi/README.md) |
| `MoralesLarios.OOFP.WebControllers` | Controladores REST genéricos | [README](./src/MoralesLarios.OOFP.WebControllers/README.md) |
| `MoralesLarios.OOFP.WebControllers.Cache` | Controladores REST con caché por controlador | [README](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md) |
| `MoralesLarios.OOFP.HttpClients` | Clientes HTTP tipados y funcionales | [README](./src/MoralesLarios.OOFP.HttpClients/README.md) |
| `MoralesLarios.OOFP.EFCore.WebApi` | Base de integración entre EF Core y Web API | [README](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md) |

### Proyectos de pruebas y verificación

Estos proyectos validan la solución desde distintos ángulos y sirven como referencia de uso real:

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

Aunque toda la solución tiene valor por sí misma, `MoralesLarios.OOFP` es el fundamento común.

### Qué resuelve

- abstrae el patrón `Result`
- unifica tratamiento de errores
- permite composición funcional sin pérdida de contexto
- proporciona la base para logging, validación, persistencia y web

### Qué encontrarás en su documentación técnica

- [Introducción general](./__Doc/1_Intro.md)
- [Guía por tipos](./__Doc/Types/README.md)
- [Detalles de `MlResult<T>`](./__Doc/Types/MlResult.md)
- [Operaciones de `Bind`](./__Doc/Types/MlResultActionsBind.md)
- [Operaciones de `Map`](./__Doc/Types/MlResultActionsMap.md)
- [Operaciones de `Match`](./__Doc/Types/MlResultActionsMatch.md)
- [Operaciones de `ExecSelf`](./__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones de `Several`](./__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](./__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](./__Doc/Types/MlResultBucles.md)
- [Cambio de retorno](./__Doc/Types/MlResultChangeReturnResult.md)
- [Errores y detalles](./__Doc/Types/MlResultErrors.md)

### Por qué es importante

Porque el resto de librerías reutilizan exactamente el mismo estilo:

- `ValueObjects` usa `MlResult<T>` para crear y validar tipos seguros.
- `Validation` transforma validaciones en resultados funcionales.
- `EFCore` encapsula operaciones de base de datos en resultados.
- `WebServices` expone la lógica de aplicación en la misma semántica funcional.
- `WebApi` convierte esos resultados en respuestas HTTP.
- `HttpClients` consume esas respuestas con la misma filosofía.

---

## Capas de la solución

### 1. Dominio, semántica y tipos seguros

#### `MoralesLarios.OOFP.ValueObjects`
Librería de value objects tipados para evitar el uso de primitivos sin semántica.

Aporta, entre otros:

- `NotEmptyString`
- `Key`
- `Mail`
- `IntNotNegative`
- value objects numéricos y de texto

📘 [README del proyecto](./src/MoralesLarios.OOFP.ValueObjects/README.md)

#### `MoralesLarios.OOFP.ValueObjects.IO`
Especialización de value objects para rutas y filesystem.

Aporta:

- `MlFile`
- `MlDirectory`
- `ExistsFile`
- `ExistDirectory`

📘 [README del proyecto](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md)

#### `MoralesLarios.OOFP.Validation`
Base de validación funcional con `MlValidableFp<T>`.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Validation/README.md)

#### `MoralesLarios.OOFP.Validation.Dataannotations`
Extiende la validación funcional con atributos de `DataAnnotations`.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)

#### `MoralesLarios.OOFP.Validation.FluentValidations`
Extiende la validación funcional con `FluentValidation`.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)

---

### 2. Infraestructura común

#### `MoralesLarios.OOFP.Internals`
Tipos internos reutilizables, especialmente para paginación y metadatos comunes.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Internals/README.md)

#### `MoralesLarios.OOFP.Extensions.Loggers`
Extensiones para registrar trazas sobre `MlResult<T>` sin romper el flujo funcional.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md)

#### `MoralesLarios.OOFP.Utilities`
Lectura segura de configuración y connection strings con `MlResult<T>`.

📘 [README del proyecto](./src/MoralesLarios.OOFP.Utilities/README.md)

#### `MoralesLarios.OOFP.IO`
Wrapper funcional sobre `System.IO` para ficheros y directorios.

📘 [README del proyecto](./src/MoralesLarios.OOFP.IO/README.md)

---

### 3. Persistencia

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

📘 [README del proyecto](./src/MoralesLarios.OOFP.EFCore/README.md)

#### `MoralesLarios.OOFP.EFCore.WebApi`
Proyecto de integración entre EF Core y Web API.

Actualmente es una base/skeleton para extender con lógica de aplicación específica.

📘 [README del proyecto](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md)

---

### 4. Servicios de aplicación

#### `MoralesLarios.OOFP.WebServices`
Capa funcional entre repositorio y web.

Aporta:

- `IGenServiceFp<TEntity, TDto>`
- `IGenServiceFp<TEntity, TRequest, TResponse>`
- `GenServiceFp<TEntity, TDto>`
- `GenServiceFp<TEntity, TRequest, TResponse>`
- `MlProblemsDetails`
- extensiones de registro para ciclo de vida clásico y duplex

📘 [README del proyecto](./src/MoralesLarios.OOFP.WebServices/README.md)

---

### 5. Exposición web

#### `MoralesLarios.OOFP.WebApi`
Puente funcional entre `MlResult<T>` e `IActionResult`.

Aporta:

- `MlActionResults`
- `ExtendedProblemDetails`
- `ProblemDetailsInfo`
- `MlResultWebExtensionsPlus`
- `MlErrorsDetailsExtensions`
- helpers para headers del request

📘 [README del proyecto](./src/MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores genéricos ASP.NET Core para CRUD estándar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar parámetros PK en Swagger/OpenAPI

📘 [README del proyecto](./src/MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensión cacheada de los controladores genéricos.

Aporta:

- caché por controlador
- invalidación automática en escrituras
- vaciado manual
- bypass dinámico
- soporte clásico y duplex
- soporte para PK compuesta

📘 [README del proyecto](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)

---

### 6. Consumo HTTP

#### `MoralesLarios.OOFP.HttpClients`
Cliente HTTP funcional integrado con `MlResult<T>` y `IHttpClientFactory`.

Aporta:

- clientes tipados con PK simple
- clientes duplex request/response
- clientes para PK compuesta
- manager funcional sobre `IHttpClientFactory`
- helpers de cabeceras y respuestas HTTP

📘 [README del proyecto](./src/MoralesLarios.OOFP.HttpClients/README.md)

---

## End-to-end: cómo se usa este ecosistema

### Caso clásico de API

1. Modela el dominio con `ValueObjects`.
2. Valida con `Validation`.
3. Persiste con `EFCore`.
4. Expón lógica con `WebServices`.
5. Publica con `WebControllers` y `WebApi`.
6. Añade `WebControllers.Cache` si necesitas caché.
7. Consume desde otro servicio con `HttpClients`.
8. Registra trazas con `Extensions.Loggers`.
9. Lee configuración con `Utilities`.
10. Usa `IO` y `ValueObjects.IO` para operaciones de sistema de archivos.

### Ejemplo conceptual
