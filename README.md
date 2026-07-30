# MoralesLarios.FOOP / MoralesLarios.OOFP

**MoralesLarios.FOOP** es una plataforma modular de librerÌas .NET construida sobre una base funcional com˙n.  
Su centro tÈcnico es **`MoralesLarios.OOFP`**, el n˙cleo sobre el que se apoyan el resto de proyectos: validaciÛn, persistencia, servicios de aplicaciÛn, controladores web, cachÈ, clientes HTTP, logging, IO, value objects y utilidades de infraestructura.

La propuesta del ecosistema es ofrecer una forma homogÈnea de trabajar con:

El objetivo del ecosistema es ofrecer una forma homog√©nea de trabajar con:

- `MlResult<T>` como contenedor de √©xito/error
- composici√≥n funcional con `Bind`, `Map`, `Match` y `ExecSelf`
- manejo explÌcito de errores sin usar excepciones como flujo principal de control
- integraci√≥n natural con ASP.NET Core, EF Core y DI
- tipos seguros mediante value objects y validaciÛn dedicada
- documentaci√≥n t√©cnica extensa y enlazada por m√≥dulos

---

## Visi√≥n general

Esta soluci√≥n est√° pensada para proyectos que quieran combinar:

- n√∫cleo funcional
- validaci√≥n de dominio
- persistencia segura
- exposici√≥n web limpia
- consumo HTTP tipado
- cach√© por controlador
- logging funcional
- configuraci√≥n e IO seguras

El resultado es una arquitectura en capas, consistente y reutilizable, donde cada proyecto aporta una pieza concreta del ecosistema.

---

## C√≥mo navegar esta soluci√≥n

### N˙cleo OOFP

- [DocumentaciÛn tÈcnica completa de `MoralesLarios.OOFP`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md)
- [DocumentaciÛn por tipos](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/README.md)
- [Tipos y modelo base `MlResult`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResult.md)
- [Operaciones `Bind`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsBind.md)
- [Operaciones `Map`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsMap.md)
- [Operaciones `Match`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones `ExecSelf`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones `Several`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultBucles.md)
- [Cambio de tipo de retorno](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultChangeReturnResult.md)
- [Modelo de errores](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultErrors.md)

### Documentaci√≥n por concepto dentro de `__Doc`

- [Intro general](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md)
- [Bind](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Bind/3_Bind.md)
- [Map](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Map/1_Map.md)
- [Match](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Match/1_Match.md)
- [ExecSelf](https://github.com/moraleslarios/FOOP/blob/main/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](https://github.com/moraleslarios/FOOP/blob/main/__Doc/EnsureFp/EnsureFp.md)
- [Extensions](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Extensions/Extensions.md)
- [Transformations](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Transformations/Transformations.md)
- [Bucles](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Bucle/Bucles.md)

---

## Qu√© aporta la librer√≠a principal `MoralesLarios.OOFP`

`MoralesLarios.OOFP` define el lenguaje com√∫n del ecosistema. Su idea principal es que la aplicaci√≥n trabaje con resultados expl√≠citos, en lugar de encadenar excepciones como mecanismo primario de control.

### Principios de dise√±o

- **Railway-Oriented Programming**: cada operaci√≥n puede continuar por la v√≠a de √©xito o desviarse a la v√≠a de error.
- **Composici√≥n funcional**: los m√©todos se encadenan de forma fluida y predecible.
- **Errores como datos**: el error no se oculta; se transporta, se inspecciona y se transforma.
- **Asincron√≠a segura**: existe soporte coherente para versiones `Async` y variantes que capturan excepciones.
- **Extensibilidad**: el sistema est√° construido por familias de extensiones y tipos reutilizables.

### Convenciones de nombres

El proyecto sigue una convenci√≥n uniforme:

- `Bind*`: encadena operaciones que ya devuelven `MlResult`
- `Map*`: transforma valores puros dentro de un resultado v√°lido
- `Match*`: ramifica seg√∫n `valid` o `fail`
- `ExecSelf*`: ejecuta efectos secundarios y devuelve el mismo resultado
- `Try*`: captura excepciones y las convierte en fallos funcionales
- `*Async`: versi√≥n as√≠ncrona

Esa convenci√≥n se repite en todo el ecosistema para que el comportamiento sea predecible.

### Ideas clave del n√∫cleo

- `MlResult<T>` es el tipo base de √©xito/error.
- `MlErrorsDetails` transporta el detalle estructurado del error.
- `EnsureFp` aporta precondiciones funcionales.
- Las extensiones de `Types` cubren composici√≥n, transformaci√≥n, coincidencia y cambio de forma del resultado.

---

## Mapa del ecosistema

### Resumen de proyectos principales

| Proyecto | PropÛsito | DocumentaciÛn |
|---|---|---|
| `MoralesLarios.OOFP` | N˙cleo funcional de la soluciÛn | [Intro](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md) ∑ [Tipos](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/README.md) |
| `MoralesLarios.OOFP.ValueObjects` | Value objects tipados y validados | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects/README.md) |
| `MoralesLarios.OOFP.ValueObjects.IO` | Value objects para rutas y sistema de archivos | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects.IO/README.md) |
| `MoralesLarios.OOFP.Validation` | Base de validaciÛn funcional | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation/README.md) |
| `MoralesLarios.OOFP.Validation.Dataannotations` | ValidaciÛn con DataAnnotations | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| `MoralesLarios.OOFP.Validation.FluentValidations` | ValidaciÛn con FluentValidation | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| `MoralesLarios.OOFP.Internals` | Tipos internos compartidos y paginaciÛn | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md) |
| `MoralesLarios.OOFP.Extensions.Loggers` | Logging funcional sobre `MlResult<T>` | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md) |
| `MoralesLarios.OOFP.Utilities` | Lectura segura de configuraciÛn | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Utilities/README.md) |
| `MoralesLarios.OOFP.IO` | IO seguro sobre ficheros y directorios | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md) |
| `MoralesLarios.OOFP.EFCore` | Repositorios funcionales y OOP sobre EF Core | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md) |
| `MoralesLarios.OOFP.WebServices` | Servicios de aplicaciÛn funcionales | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md) |
| `MoralesLarios.OOFP.WebApi` | Puente entre `MlResult<T>` e `IActionResult` | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md) |
| `MoralesLarios.OOFP.WebControllers` | Controladores REST genÈricos | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md) |
| `MoralesLarios.OOFP.WebControllers.Cache` | Controladores REST con cachÈ por controlador | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers.Cache/README.md) |
| `MoralesLarios.OOFP.HttpClients` | Clientes HTTP tipados y funcionales | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md) |
| `MoralesLarios.OOFP.EFCore.WebApi` | Base de integraciÛn entre EF Core y Web API | [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md) |

### Proyectos de pruebas y verificaci√≥n

La soluciÛn incluye distintos proyectos de pruebas e integraciÛn que sirven como validaciÛn viva del ecosistema:

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

Aunque toda la soluciÛn tiene valor por sÌ misma, **`MoralesLarios.OOFP` es el fundamento com˙n**.

### Qu√© resuelve

- abstrae el patr√≥n `Result`
- unifica el tratamiento de errores
- permite composici√≥n funcional sin p√©rdida de contexto
- proporciona la base para logging, validaci√≥n, persistencia y web

### Qu√© encontrar√°s en su documentaci√≥n t√©cnica

- [IntroducciÛn general](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md)
- [GuÌa por tipos](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/README.md)
- [Detalles de `MlResult<T>`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResult.md)
- [Operaciones de `Bind`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsBind.md)
- [Operaciones de `Map`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsMap.md)
- [Operaciones de `Match`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones de `ExecSelf`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones de `Several`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultActionsSeveral.md)
- [Transformaciones](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultBucles.md)
- [Cambio de retorno](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultChangeReturnResult.md)
- [Errores y detalles](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResultErrors.md)

### Por qu√© es importante

Porque el resto de proyectos reutilizan el mismo estilo y sem·ntica:

- `ValueObjects` usa `MlResult<T>` para crear y validar tipos seguros.
- `Validation` transforma validaciones en resultados funcionales.
- `EFCore` encapsula operaciones de base de datos en resultados.
- `WebServices` expone la lÛgica de aplicaciÛn en la misma sem·ntica.
- `WebApi` convierte esos resultados en respuestas HTTP.
- `HttpClients` consume esas respuestas con la misma filosof√≠a.

---

## Capas de la soluci√≥n

### Dominio, sem·ntica y tipos seguros

#### `MoralesLarios.OOFP.ValueObjects`
Librer√≠a de value objects tipados para evitar el uso de primitivos sin sem√°ntica.

Aporta, entre otros:

- `NotEmptyString`
- `Key`
- `Mail`
- `IntNotNegative`
- value objects num√©ricos y de texto

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects/README.md)

#### `MoralesLarios.OOFP.ValueObjects.IO`
Especializaci√≥n de value objects para rutas y filesystem.

Aporta:

- `MlFile`
- `MlDirectory`
- `ExistsFile`
- `ExistDirectory`

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.ValueObjects.IO/README.md)

#### `MoralesLarios.OOFP.Validation`
Base de validaci√≥n funcional con `MlValidableFp<T>`.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation/README.md)

#### `MoralesLarios.OOFP.Validation.Dataannotations`
Extiende la validaci√≥n funcional con atributos de `DataAnnotations`.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.Dataannotations/README.md)

#### `MoralesLarios.OOFP.Validation.FluentValidations`
Extiende la validaci√≥n funcional con `FluentValidation`.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Validation.FluentValidations/README.md)

---

### Infraestructura com˙n

#### `MoralesLarios.OOFP.Internals`
Tipos internos reutilizables, especialmente para paginaciÛn y metadatos compartidos.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md)

#### `MoralesLarios.OOFP.Extensions.Loggers`
Extensiones para registrar trazas sobre `MlResult<T>` sin romper el flujo funcional.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md)

#### `MoralesLarios.OOFP.Utilities`
Lectura segura de configuraci√≥n y connection strings con `MlResult<T>`.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Utilities/README.md)

#### `MoralesLarios.OOFP.IO`
Wrapper funcional sobre `System.IO` para ficheros y directorios.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md)

---

### Persistencia

#### `MoralesLarios.OOFP.EFCore`
Capa de repositorios EF Core en dos estilos:

- funcional (`*Fp`), devolviendo `MlResult<T>`
- OOP cl√°sico

Soporta:

- CRUD completo
- b√∫squeda por PK simple o compuesta
- paginaci√≥n
- consultas posicionales
- registro masivo por DI

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md)

#### `MoralesLarios.OOFP.EFCore.WebApi`
Proyecto de integraci√≥n entre EF Core y Web API.

Actualmente es una base/skeleton para extender con l√≥gica de aplicaci√≥n espec√≠fica.

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md)

---

### Servicios de aplicaciÛn

#### `MoralesLarios.OOFP.WebServices`
Capa funcional entre repositorio y web.

Aporta:

- `IGenServiceFp<TEntity, TDto>`
- `IGenServiceFp<TEntity, TRequest, TResponse>`
- `GenServiceFp<TEntity, TDto>`
- `GenServiceFp<TEntity, TRequest, TResponse>`
- `MlProblemsDetails`
- extensiones de registro para ciclo de vida cl√°sico y duplex

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md)

---

### ExposiciÛn web

#### `MoralesLarios.OOFP.WebApi`
Puente funcional entre `MlResult<T>` e `IActionResult`.

Aporta:

- `MlActionResults`
- `ExtendedProblemDetails`
- `ProblemDetailsInfo`
- `MlResultWebExtensionsPlus`
- `MlErrorsDetailsExtensions`
- helpers para headers del request

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores gen√©ricos ASP.NET Core para CRUD est√°ndar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar par√°metros PK en Swagger/OpenAPI

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensi√≥n cacheada de los controladores gen√©ricos.

Aporta:

- cach√© por controlador
- invalidaci√≥n autom√°tica en escrituras
- vaciado manual
- bypass din√°mico
- soporte cl√°sico y duplex
- soporte para PK compuesta

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers.Cache/README.md)

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

?? [README](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)

---

## End-to-end: cÛmo usar el ecosistema

### Caso tÌpico

1. Modela el dominio con `ValueObjects`.
2. Valida con `Validation`.
3. Persiste con `EFCore`.
4. ExpÛn la lÛgica con `WebServices`.
5. Publica con `WebControllers` y `WebApi`.
6. A√±ade `WebControllers.Cache` si necesitas cach√©.
7. Consume desde otro servicio con `HttpClients`.
8. Registra trazas con `Extensions.Loggers`.
9. Lee configuraci√≥n con `Utilities`.
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

La soluciÛn tambiÈn incluye proyectos de pruebas unitarias e integraciÛn que funcionan como verificaciÛn viva del comportamiento:

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

Estos proyectos sirven para verificar contratos, ejemplos reales de uso y escenarios de integraci√≥n entre capas.

---

## Documentaci√≥n adicional

### Documentaci√≥n ra√≠z del n√∫cleo OOFP

- [Intro general y filosofÌa tÈcnica](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md)
- [DocumentaciÛn por tipos](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/README.md)
- [Tipos y resultados](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/MlResult.md)
- [Bind](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Bind/3_Bind.md)
- [Map](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Map/1_Map.md)
- [Match](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Match/1_Match.md)
- [ExecSelf](https://github.com/moraleslarios/FOOP/blob/main/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](https://github.com/moraleslarios/FOOP/blob/main/__Doc/EnsureFp/EnsureFp.md)
- [Extensions](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Extensions/Extensions.md)
- [Transformations](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Transformations/Transformations.md)
- [Bucles](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Bucle/Bucles.md)

### README de cada proyecto

- [MoralesLarios.OOFP.EFCore](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.Internals/README.md)
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

Si tuviera que describir la soluci√≥n en una sola frase, ser√≠a esta:

> **MoralesLarios.FOOP es un ecosistema .NET funcional para construir dominios, servicios, APIs y clientes con una sem√°ntica com√∫n basada en `MlResult<T>`.**

Y si tuviera que destacar una sola pieza, esa ser√≠a:

> **`MoralesLarios.OOFP` es el n√∫cleo fundacional; el resto de proyectos ampl√≠an su valor hacia validaci√≥n, persistencia, web, cach√©, HTTP, IO y configuraci√≥n.**

---

## Compatibilidad

La soluci√≥n est√° organizada para proyectos objetivo de:

- `.NET 9`
- `.NET 8`

---

## Licencia y estilo de trabajo

La soluciÛn est· pensada para crecer por capas, manteniendo una misma forma de trabajo en todo el stack.

Si buscas una entrada r√°pida para entender la librer√≠a, empieza por:

1. [Intro general de `MoralesLarios.OOFP`](https://github.com/moraleslarios/FOOP/blob/main/__Doc/1_Intro.md)
2. [DocumentaciÛn por tipos](https://github.com/moraleslarios/FOOP/blob/main/__Doc/Types/README.md)
3. [WebServices](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebServices/README.md)
4. [WebApi](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebApi/README.md)
5. [WebControllers](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.WebControllers/README.md)
6. [HttpClients](https://github.com/moraleslarios/FOOP/blob/main/src/MoralesLarios.OOFP.HttpClients/README.md)

---

## Nota final

Este repositorio no es una √∫nica librer√≠a aislada, sino una **plataforma modular**. Cada proyecto tiene su propio README y, cuando aplica, su propia documentaci√≥n t√©cnica enlazada desde `__Doc`.

La documentaci√≥n ra√≠z pretende ser la puerta de entrada oficial al ecosistema completo.
