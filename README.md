# MoralesLarios.FOOP

**MoralesLarios.FOOP** es una plataforma modular de librerías .NET construida sobre una base funcional común.  
Su centro técnico es **`MoralesLarios.OOFP`**, el núcleo sobre el que se apoyan el resto de proyectos: validación, persistencia, servicios de aplicación, controladores web, caché, clientes HTTP, logging, IO, value objects y utilidades de infraestructura.

La propuesta del ecosistema es ofrecer una forma homogénea de trabajar con:

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

### Núcleo OOFP

- [Documentación técnica completa de `MoralesLarios.OOFP`](./src/__Doc/1_Intro.md)
- [Documentación por tipos](./src/__Doc/Types/README.md)
- [Tipos y modelo base `MlResult`](./src/__Doc/Types/MlResult.md)
- [Operaciones `Bind`](./src/__Doc/Types/MlResultActionsBind.md)
- [Operaciones `Map`](./src/__Doc/Types/MlResultActionsMap.md)
- [Operaciones `Match`](./src/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones `ExecSelf`](./src/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones `Several`](./src/__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](./src/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](./src/__Doc/Types/MlResultBucles.md)
- [Cambio de tipo de retorno](./src/__Doc/Types/MlResultChangeReturnResult.md)
- [Modelo de errores](./src/__Doc/Types/MlResultErrors.md)

### Documentación por concepto dentro de `__Doc`

- [Intro general](./src/__Doc/1_Intro.md)
- [Bind](./src/__Doc/Bind/3_Bind.md)
- [Map](./src/__Doc/Map/1_Map.md)
- [Match](./src/__Doc/Match/1_Match.md)
- [ExecSelf](./src/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](./src/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](./src/__Doc/EnsureFp/EnsureFp.md)
- [Extensions](./src/__Doc/Extensions/Extensions.md)
- [Transformations](./src/__Doc/Transformations/Transformations.md)
- [Bucles](./src/__Doc/Bucle/Bucles.md)

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

### Resumen de proyectos principales

| Proyecto | Propósito | Documentación |
|---|---|---|
| `MoralesLarios.OOFP` | Núcleo funcional de la solución | [Intro](./src/__Doc/1_Intro.md) · [Tipos](./src/__Doc/Types/README.md) |
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

- [Introducción general](./src/__Doc/1_Intro.md)
- [Guía por tipos](./src/__Doc/Types/README.md)
- [Detalles de `MlResult<T>`](./src/__Doc/Types/MlResult.md)
- [Operaciones de `Bind`](./src/__Doc/Types/MlResultActionsBind.md)
- [Operaciones de `Map`](./src/__Doc/Types/MlResultActionsMap.md)
- [Operaciones de `Match`](./src/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones de `ExecSelf`](./src/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones de `Several`](./src/__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](./src/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](./src/__Doc/Types/MlResultBucles.md)
- [Cambio de retorno](./src/__Doc/Types/MlResultChangeReturnResult.md)
- [Errores y detalles](./src/__Doc/Types/MlResultErrors.md)

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

📘 [README](./src/MoralesLarios.OOFP.ValueObjects/README.md)

#### `MoralesLarios.OOFP.ValueObjects.IO`
Especialización de value objects para rutas y filesystem.

Aporta:

- `MlFile`
- `MlDirectory`
- `ExistsFile`
- `ExistDirectory`

📘 [README](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md)

#### `MoralesLarios.OOFP.Validation`
Base de validación funcional con `MlValidableFp<T>`.

📘 [README](./src/MoralesLarios.OOFP.Validation/README.md)

#### `MoralesLarios.OOFP.Validation.Dataannotations`
Extiende la validación funcional con atributos de `DataAnnotations`.

📘 [README](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)

#### `MoralesLarios.OOFP.Validation.FluentValidations`
Extiende la validación funcional con `FluentValidation`.

📘 [README](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)

---

### Infraestructura común

#### `MoralesLarios.OOFP.Internals`
Tipos internos reutilizables, especialmente para paginación y metadatos compartidos.

📘 [README](./src/MoralesLarios.OOFP.Internals/README.md)

#### `MoralesLarios.OOFP.Extensions.Loggers`
Extensiones para registrar trazas sobre `MlResult<T>` sin romper el flujo funcional.

📘 [README](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md)

#### `MoralesLarios.OOFP.Utilities`
Lectura segura de configuración y connection strings con `MlResult<T>`.

📘 [README](./src/MoralesLarios.OOFP.Utilities/README.md)

#### `MoralesLarios.OOFP.IO`
Wrapper funcional sobre `System.IO` para ficheros y directorios.

📘 [README](./src/MoralesLarios.OOFP.IO/README.md)

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

📘 [README](./src/MoralesLarios.OOFP.EFCore/README.md)

#### `MoralesLarios.OOFP.EFCore.WebApi`
Proyecto de integración entre EF Core y Web API.

Actualmente es una base/skeleton para extender con lógica de aplicación específica.

📘 [README](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md)

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

📘 [README](./src/MoralesLarios.OOFP.WebServices/README.md)

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

📘 [README](./src/MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores genéricos ASP.NET Core para CRUD estándar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar parámetros PK en Swagger/OpenAPI

📘 [README](./src/MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensión cacheada de los controladores genéricos.

Aporta:

- caché por controlador
- invalidación automática en escrituras
- vaciado manual
- bypass dinámico
- soporte clásico y duplex
- soporte para PK compuesta

📘 [README](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)

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

📘 [README](./src/MoralesLarios.OOFP.HttpClients/README.md)

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

- [Intro general y filosofía técnica](./src/__Doc/1_Intro.md)
- [Documentación por tipos](./src/__Doc/Types/README.md)
- [Tipos y resultados](./src/__Doc/Types/MlResult.md)
- [Bind](./src/__Doc/Bind/3_Bind.md)
- [Map](./src/__Doc/Map/1_Map.md)
- [Match](./src/__Doc/Match/1_Match.md)
- [ExecSelf](./src/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](./src/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](./src/__Doc/EnsureFp/EnsureFp.md)
- [Extensions](./src/__Doc/Extensions/Extensions.md)
- [Transformations](./src/__Doc/Transformations/Transformations.md)
- [Bucles](./src/__Doc/Bucle/Bucles.md)

### README de cada proyecto

- [MoralesLarios.OOFP.EFCore](./src/MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](./src/MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](./src/MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](./src/MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](./src/MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](./src/MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Utilities](./src/MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](./src/MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](./src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](./src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](./src/MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](./src/MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](./src/MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](./src/MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](./src/MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](./src/MoralesLarios.OOFP.WebServices/README.md)

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

1. [Intro general de `MoralesLarios.OOFP`](./src/__Doc/1_Intro.md)
2. [Documentación por tipos](./src/__Doc/Types/README.md)
3. [WebServices](./src/MoralesLarios.OOFP.WebServices/README.md)
4. [WebApi](./src/MoralesLarios.OOFP.WebApi/README.md)
5. [WebControllers](./src/MoralesLarios.OOFP.WebControllers/README.md)
6. [HttpClients](./src/MoralesLarios.OOFP.HttpClients/README.md)

---

## Nota final

Este repositorio no es una única librería aislada, sino una **plataforma modular**. Cada proyecto tiene su propio README y, cuando aplica, su propia documentación técnica enlazada desde `__Doc`.

La documentación raíz pretende ser la puerta de entrada oficial al ecosistema completo.
