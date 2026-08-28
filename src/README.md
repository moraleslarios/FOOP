# MoralesLarios.FOOP / MoralesLarios.OOFP

**MoralesLarios.FOOP** es una plataforma modular de librerías .NET construida sobre una base funcional común.  
Su centro técnico es **`MoralesLarios.OOFP`**, el núcleo sobre el que se apoyan el resto de proyectos: validación, persistencia, servicios de aplicación, controladores web, caché, clientes HTTP, logging, IO, value objects y utilidades de infraestructura.

La propuesta de la solución es ofrecer un ecosistema coherente para trabajar con:

- `MlResult<T>` como contenedor de éxito y error
- composición funcional con `Bind`, `Map`, `Match` y `ExecSelf`
- manejo explícito de errores sin usar excepciones como flujo principal
- integración natural con ASP.NET Core, EF Core y DI
- tipos seguros mediante value objects y validación dedicada
- documentación técnica extensa y enlazada por módulos

---

## Visión general

Esta solución está pensada para proyectos que quieran combinar:

- **núcleo funcional**
- **validación de dominio**
- **persistencia segura**
- **exposición web limpia**
- **consumo HTTP tipado**
- **caché por controlador**
- **logging funcional**
- **configuración e IO seguras**

El resultado es una arquitectura en capas, consistente y reusable, donde cada proyecto aporta una pieza concreta del ecosistema.

---

## Cómo empezar a navegar la solución

### 🚀 Punto de entrada recomendado

📘 **[Introducción general y índice completo de la documentación](./MoralesLarios.FOOP/__Doc/1_Intro.md)**

Ese documento contiene el
[mapa de los 48 documentos técnicos](./MoralesLarios.FOOP/__Doc/1_Intro.md#índice-completo-de-la-documentación)
del núcleo, con rutas de lectura según tu nivel y tu objetivo.

### Núcleo OOFP — referencia por archivo de código

| Documento | Contenido |
|-----------|-----------|
| [README del proyecto `MoralesLarios.OOFP`](./MoralesLarios.FOOP/README.md) | Guía extensa del núcleo con ejemplos |
| [Índice de la referencia por tipos](./MoralesLarios.FOOP/__Doc/Types/README.md) | Portada de `__Doc/Types/` |
| [`MlResult`](./MoralesLarios.FOOP/__Doc/Types/MlResult.md) | El tipo raíz, fábricas y conversiones implícitas |
| [Modelo de errores](./MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md) | `MlError`, `MlErrorsDetails`, `ErrorMessage` |
| [`MlResultActions`](./MoralesLarios.FOOP/__Doc/Types/MlResultActions.md) | Enriquecer errores, transportar datos, acceso seguro |
| [Operaciones `Bind`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsBind.md) | Todas las sobrecargas de `Bind*` |
| [Operaciones `Map`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsMap.md) | Todas las sobrecargas de `Map*` |
| [Operaciones `Match`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsMatch.md) | Todas las sobrecargas de `Match*` |
| [Operaciones `ExecSelf`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsExecSelf.md) | Todas las sobrecargas de `ExecSelf*` |
| [Operaciones `Several`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsSeveral.md) | `EmptyToFailed`, `NullToFailed`, `BoolToResult`, `Combine`, `Do` |
| [Detalles del error](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsErrorsDetails.md) | Leer y fusionar el diccionario `Details` |
| [Bucles funcionales](./MoralesLarios.FOOP/__Doc/Types/MlResultBucles.md) | `Projection*`, `ProjectionSplit*`, `Fusion*` |
| [Transformaciones](./MoralesLarios.FOOP/__Doc/Types/MlResultTransformations.md) | `ToMlResult*`, `TryToMlResult*`, boxing |
| [Cambio de tipo de retorno](./MoralesLarios.FOOP/__Doc/Types/MlResultChangeReturnResult.md) | Conservar el estado al cambiar de tipo |

### Núcleo OOFP — guías por concepto

**`Bind` — encadenar operaciones que devuelven `MlResult`**

- [`3_Bind`](./MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) ⭐ ·
  [`2_MlResultActions`](./MoralesLarios.FOOP/__Doc/Bind/2_MlResultActions.md) ·
  [`4_BindMulti`](./MoralesLarios.FOOP/__Doc/Bind/4_BindMulti.md) ·
  [`5_BindIf`](./MoralesLarios.FOOP/__Doc/Bind/5_BindIf.md)
- Recuperación: [`6_BindIfFail`](./MoralesLarios.FOOP/__Doc/Bind/6_BindIfFail.md) ·
  [`7_BindIfFailWithValue`](./MoralesLarios.FOOP/__Doc/Bind/7_BindIfFailWithValue.md) ·
  [`8_BindIfFailWithException`](./MoralesLarios.FOOP/__Doc/Bind/8_BindIfFailWithException.md) ·
  [`9_BindIfFailWithoutException`](./MoralesLarios.FOOP/__Doc/Bind/9_BindIfFailWithoutException.md)
- [`10_BindAlways`](./MoralesLarios.FOOP/__Doc/Bind/10_BindAlways.md) ·
  [`11_BindSaveValueInDetails…`](./MoralesLarios.FOOP/__Doc/Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md)

**`Map` — transformar el valor sin salir del carril**

- [`1_Map`](./MoralesLarios.FOOP/__Doc/Map/1_Map.md) ⭐ ·
  [`2_MapEnsure`](./MoralesLarios.FOOP/__Doc/Map/2_MapEnsure.md) ·
  [`3_MapIf`](./MoralesLarios.FOOP/__Doc/Map/3_MapIf.md)
- Reserva ante fallo: [`4_MapIfFail`](./MoralesLarios.FOOP/__Doc/Map/4_MapIfFail.md) ·
  [`5_MapIfFailWithValue`](./MoralesLarios.FOOP/__Doc/Map/5_MapIfFailWithValue.md) ·
  [`6_MapIfFailWithException`](./MoralesLarios.FOOP/__Doc/Map/6_MapIfFailWithException.md) ·
  [`7_MapIfFailWithoutException`](./MoralesLarios.FOOP/__Doc/Map/7_MapIfFailWithoutException.md)
- [`8_MapAlways`](./MoralesLarios.FOOP/__Doc/Map/8_MapAlways.md)

**`Match` — salir del carril**

- [`1_Match`](./MoralesLarios.FOOP/__Doc/Match/1_Match.md) ⭐ ·
  [`2_MatchAll`](./MoralesLarios.FOOP/__Doc/Match/2_MatchAll.md)

**`ExecSelf` — efectos laterales sin alterar el resultado**

- [`1_ExecSelf`](./MoralesLarios.FOOP/__Doc/ExecSelf/1_ExecSelf.md) ⭐ ·
  [`2_ExecSelfIfValid`](./MoralesLarios.FOOP/__Doc/ExecSelf/2_ExecSelfIfValid.md) ·
  [`3_ExecSelfIfFail`](./MoralesLarios.FOOP/__Doc/ExecSelf/3_ExecSelfIfFail.md)
- [`4_ExecSelfIfFailWithValue`](./MoralesLarios.FOOP/__Doc/ExecSelf/4_ExecSelfIfFailWithValue.md) ·
  [`5_ExecSelfIfFailWithException`](./MoralesLarios.FOOP/__Doc/ExecSelf/5_ExecSelfIfFailWithException.md) ·
  [`6_ExecSelfIfFailWithoutException`](./MoralesLarios.FOOP/__Doc/ExecSelf/6_ExecSelfIfFailWithoutException.md)

**`Several` — puentes desde el mundo imperativo**

- [`1_EmptyToFailed`](./MoralesLarios.FOOP/__Doc/Several/1_EmptyToFailed.md) ·
  [`2_NullToFailed`](./MoralesLarios.FOOP/__Doc/Several/2_NullToFailed.md) ·
  [`3_BoolToResult`](./MoralesLarios.FOOP/__Doc/Several/3_BoolToResult.md) ·
  [`4_Combine`](./MoralesLarios.FOOP/__Doc/Several/4_Combine.md) ⚠️ (**no** acumula errores)

**Utilidades y colecciones**

- [`EnsureFp`](./MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) — precondiciones funcionales
- [`Transformations`](./MoralesLarios.FOOP/__Doc/Transformations/Transformations.md) — entrar al carril desde código que lanza
- [`Extensions`](./MoralesLarios.FOOP/__Doc/Extensions/Extensions.md) — `ToAsync`, `With`, `ToFuncTask`, `Constants`
- [`Bucles`](./MoralesLarios.FOOP/__Doc/Bucle/Bucles.md) — proyecciones sobre colecciones

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
| `MoralesLarios.OOFP` | Núcleo funcional de la solución | [README](./MoralesLarios.FOOP/README.md) · [Intro](./MoralesLarios.FOOP/__Doc/1_Intro.md) · [Tipos](./MoralesLarios.FOOP/__Doc/Types/README.md) |
| `MoralesLarios.OOFP.ValueObjects` | Value objects tipados y validados | [README](./MoralesLarios.OOFP.ValueObjects/README.md) |
| `MoralesLarios.OOFP.ValueObjects.IO` | Value objects para rutas y sistema de archivos | [README](./MoralesLarios.OOFP.ValueObjects.IO/README.md) |
| `MoralesLarios.OOFP.Validation` | Base de validación funcional | [README](./MoralesLarios.OOFP.Validation/README.md) |
| `MoralesLarios.OOFP.Validation.Dataannotations` | Validación con DataAnnotations | [README](./MoralesLarios.OOFP.Validation.Dataannotations/README.md) |
| `MoralesLarios.OOFP.Validation.FluentValidations` | Validación con FluentValidation | [README](./MoralesLarios.OOFP.Validation.FluentValidations/README.md) |
| `MoralesLarios.OOFP.Internals` | Tipos internos compartidos y paginación | [README](./MoralesLarios.OOFP.Internals/README.md) |
| `MoralesLarios.OOFP.Extensions.Loggers` | Logging funcional sobre `MlResult<T>` | [README](./MoralesLarios.OOFP.Extensions.Loggers/README.md) |
| `MoralesLarios.OOFP.Utilities` | Lectura segura de configuración | [README](./MoralesLarios.OOFP.Utilities/README.md) |
| `MoralesLarios.OOFP.IO` | IO seguro sobre ficheros y directorios | [README](./MoralesLarios.OOFP.IO/README.md) |
| `MoralesLarios.OOFP.EFCore` | Repositorios funcionales y OOP sobre EF Core | [README](./MoralesLarios.OOFP.EFCore/README.md) |
| `MoralesLarios.OOFP.WebServices` | Servicios de aplicación funcionales | [README](./MoralesLarios.OOFP.WebServices/README.md) |
| `MoralesLarios.OOFP.WebApi` | Puente entre `MlResult<T>` e `IActionResult` | [README](./MoralesLarios.OOFP.WebApi/README.md) |
| `MoralesLarios.OOFP.WebControllers` | Controladores REST genéricos | [README](./MoralesLarios.OOFP.WebControllers/README.md) |
| `MoralesLarios.OOFP.WebControllers.Cache` | Controladores REST con caché por controlador | [README](./MoralesLarios.OOFP.WebControllers.Cache/README.md) |
| `MoralesLarios.OOFP.HttpClients` | Clientes HTTP tipados y funcionales | [README](./MoralesLarios.OOFP.HttpClients/README.md) |
| `MoralesLarios.OOFP.EFCore.WebApi` | Base de integración entre EF Core y Web API | [README](./MoralesLarios.OOFP.EFCore.WebApi/README.md) |

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

- 📘 [**Introducción general con el índice completo**](./MoralesLarios.FOOP/__Doc/1_Intro.md) — el punto de partida
- 📘 [README extenso del núcleo](./MoralesLarios.FOOP/README.md) — guía con ejemplos
- [Guía por tipos](./MoralesLarios.FOOP/__Doc/Types/README.md)
- [Detalles de `MlResult<T>`](./MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [Errores y detalles](./MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [Operaciones de `Bind`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsBind.md)
- [Operaciones de `Map`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsMap.md)
- [Operaciones de `Match`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsMatch.md)
- [Operaciones de `ExecSelf`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsExecSelf.md)
- [Operaciones de `Several`](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsSeveral.md)
- [Utilidades de `MlResultActions`](./MoralesLarios.FOOP/__Doc/Types/MlResultActions.md)
- [Detalles del error](./MoralesLarios.FOOP/__Doc/Types/MlResultActionsErrorsDetails.md)
- [Transformaciones](./MoralesLarios.FOOP/__Doc/Types/MlResultTransformations.md)
- [Bucles funcionales](./MoralesLarios.FOOP/__Doc/Types/MlResultBucles.md)
- [Cambio de retorno](./MoralesLarios.FOOP/__Doc/Types/MlResultChangeReturnResult.md)

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

📘 [README](./MoralesLarios.OOFP.ValueObjects/README.md)

#### `MoralesLarios.OOFP.ValueObjects.IO`
Especialización de value objects para rutas y filesystem.

Aporta:

- `MlFile`
- `MlDirectory`
- `ExistsFile`
- `ExistDirectory`

📘 [README](./MoralesLarios.OOFP.ValueObjects.IO/README.md)

#### `MoralesLarios.OOFP.Validation`
Base de validación funcional con `MlValidableFp<T>`.

📘 [README](./MoralesLarios.OOFP.Validation/README.md)

#### `MoralesLarios.OOFP.Validation.Dataannotations`
Extiende la validación funcional con atributos de `DataAnnotations`.

📘 [README](./MoralesLarios.OOFP.Validation.Dataannotations/README.md)

#### `MoralesLarios.OOFP.Validation.FluentValidations`
Extiende la validación funcional con `FluentValidation`.

📘 [README](./MoralesLarios.OOFP.Validation.FluentValidations/README.md)

---

### Infraestructura común

#### `MoralesLarios.OOFP.Internals`
Tipos internos reutilizables, especialmente para paginación y metadatos compartidos.

📘 [README](./MoralesLarios.OOFP.Internals/README.md)

#### `MoralesLarios.OOFP.Extensions.Loggers`
Extensiones para registrar trazas sobre `MlResult<T>` sin romper el flujo funcional.

📘 [README](./MoralesLarios.OOFP.Extensions.Loggers/README.md)

#### `MoralesLarios.OOFP.Utilities`
Lectura segura de configuración y connection strings con `MlResult<T>`.

📘 [README](./MoralesLarios.OOFP.Utilities/README.md)

#### `MoralesLarios.OOFP.IO`
Wrapper funcional sobre `System.IO` para ficheros y directorios.

📘 [README](./MoralesLarios.OOFP.IO/README.md)

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

📘 [README](./MoralesLarios.OOFP.EFCore/README.md)

#### `MoralesLarios.OOFP.EFCore.WebApi`
Proyecto de integración entre EF Core y Web API.

Actualmente es una base/skeleton para extender con lógica de aplicación específica.

📘 [README](./MoralesLarios.OOFP.EFCore.WebApi/README.md)

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

📘 [README](./MoralesLarios.OOFP.WebServices/README.md)

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

📘 [README](./MoralesLarios.OOFP.WebApi/README.md)

#### `MoralesLarios.OOFP.WebControllers`
Controladores genéricos ASP.NET Core para CRUD estándar.

Aporta:

- soporte para PK simple
- soporte duplex request/response
- soporte para PK compuesta
- soporte duplex con PK compuesta
- atributo para documentar parámetros PK en Swagger/OpenAPI

📘 [README](./MoralesLarios.OOFP.WebControllers/README.md)

#### `MoralesLarios.OOFP.WebControllers.Cache`
Extensión cacheada de los controladores genéricos.

Aporta:

- caché por controlador
- invalidación automática en escrituras
- vaciado manual
- bypass dinámico
- soporte clásico y duplex
- soporte para PK compuesta

📘 [README](./MoralesLarios.OOFP.WebControllers.Cache/README.md)

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

📘 [README](./MoralesLarios.OOFP.HttpClients/README.md)

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

- 📘 [**Intro general, filosofía técnica e índice completo**](./MoralesLarios.FOOP/__Doc/1_Intro.md)
- 📘 [README extenso del núcleo](./MoralesLarios.FOOP/README.md)
- [Documentación por tipos](./MoralesLarios.FOOP/__Doc/Types/README.md)
- [Tipos y resultados](./MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [Bind](./MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [Map](./MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [Match](./MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [ExecSelf](./MoralesLarios.FOOP/__Doc/ExecSelf/1_ExecSelf.md)
- [Several](./MoralesLarios.FOOP/__Doc/Several/1_EmptyToFailed.md)
- [EnsureFp](./MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
- [Extensions](./MoralesLarios.FOOP/__Doc/Extensions/Extensions.md)
- [Transformations](./MoralesLarios.FOOP/__Doc/Transformations/Transformations.md)
- [Bucles](./MoralesLarios.FOOP/__Doc/Bucle/Bucles.md)

> El listado completo y detallado de los 48 documentos está en
> [«Cómo empezar a navegar la solución»](#cómo-empezar-a-navegar-la-solución)
> y en el [índice maestro](./MoralesLarios.FOOP/__Doc/1_Intro.md#índice-completo-de-la-documentación).

### README de cada proyecto

- [MoralesLarios.OOFP (núcleo)](./MoralesLarios.FOOP/README.md)
- [MoralesLarios.OOFP.EFCore](./MoralesLarios.OOFP.EFCore/README.md)
- [MoralesLarios.OOFP.EFCore.WebApi](./MoralesLarios.OOFP.EFCore.WebApi/README.md)
- [MoralesLarios.OOFP.Extensions.Loggers](./MoralesLarios.OOFP.Extensions.Loggers/README.md)
- [MoralesLarios.OOFP.HttpClients](./MoralesLarios.OOFP.HttpClients/README.md)
- [MoralesLarios.OOFP.IO](./MoralesLarios.OOFP.IO/README.md)
- [MoralesLarios.OOFP.Internals](./MoralesLarios.OOFP.Internals/README.md)
- [MoralesLarios.OOFP.Utilities](./MoralesLarios.OOFP.Utilities/README.md)
- [MoralesLarios.OOFP.Validation](./MoralesLarios.OOFP.Validation/README.md)
- [MoralesLarios.OOFP.Validation.Dataannotations](./MoralesLarios.OOFP.Validation.Dataannotations/README.md)
- [MoralesLarios.OOFP.Validation.FluentValidations](./MoralesLarios.OOFP.Validation.FluentValidations/README.md)
- [MoralesLarios.OOFP.ValueObjects](./MoralesLarios.OOFP.ValueObjects/README.md)
- [MoralesLarios.OOFP.ValueObjects.IO](./MoralesLarios.OOFP.ValueObjects.IO/README.md)
- [MoralesLarios.OOFP.WebApi](./MoralesLarios.OOFP.WebApi/README.md)
- [MoralesLarios.OOFP.WebControllers](./MoralesLarios.OOFP.WebControllers/README.md)
- [MoralesLarios.OOFP.WebControllers.Cache](./MoralesLarios.OOFP.WebControllers.Cache/README.md)
- [MoralesLarios.OOFP.WebServices](./MoralesLarios.OOFP.WebServices/README.md)

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

1. 📘 [Intro general de `MoralesLarios.OOFP`](./MoralesLarios.FOOP/__Doc/1_Intro.md) — **empieza aquí**
2. [README extenso del núcleo](./MoralesLarios.FOOP/README.md)
3. [Documentación por tipos](./MoralesLarios.FOOP/__Doc/Types/README.md)
4. [WebServices](./MoralesLarios.OOFP.WebServices/README.md)
5. [WebApi](./MoralesLarios.OOFP.WebApi/README.md)
6. [WebControllers](./MoralesLarios.OOFP.WebControllers/README.md)
7. [HttpClients](./MoralesLarios.OOFP.HttpClients/README.md)

---

## Nota final

Este repositorio no es una única librería aislada, sino una **plataforma modular**. Cada proyecto tiene su propio README y, cuando aplica, su propia documentación técnica enlazada desde `__Doc`.

La documentación raíz pretende ser la puerta de entrada oficial al ecosistema completo.
