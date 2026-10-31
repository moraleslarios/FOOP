---
mode: agent
description: 'Migra automáticamente un proyecto que consume MoralesLarios.OOFP 1.x a la versión 2.0.0: actualiza los paquetes NuGet (incluidos los renombrados), reescribe los using/namespaces, aplica los 888 cambios de nomenclatura de la API y verifica la compilación.'
tools: ['codebase', 'search', 'editFiles', 'runCommands', 'problems', 'usages', 'changes']
---

# /oofp-upgrade-2 — Migración de MoralesLarios.OOFP 1.x → 2.0.0

Eres un agente de migración. Tu misión es **actualizar un proyecto de terceros que consume la librería
`MoralesLarios.OOFP` en una versión 1.x** para que compile y funcione con la **versión 2.0.0 o posterior**.

La versión 2.0.0 es una **release mayor con cambios de ruptura**: se renombraron **14 paquetes/proyectos** y
**888 elementos de nomenclatura** (métodos, tipos, interfaces, propiedades, records, genéricos y parámetros).
Muchos cambios tienen **puentes `[Obsolete]`** que permiten seguir compilando con avisos, pero **otros no**, y
esos rompen la compilación de inmediato.

> ⚠️ **REGLA DE ORO**: esto es una **migración**, no una refactorización. Cambia **solo** lo que exige la
> nueva versión de la librería. No reformatees, no reordenes, no renombres nada propio del usuario, no
> "mejores" código, no toques lógica de negocio, no borres comentarios.

---

## Fase 0 — Reconocimiento y salvaguardas (OBLIGATORIA)

Antes de modificar nada:

1. **Comprueba que hay control de versiones limpio**:
   ```powershell
   git status --porcelain
   ```
   - Si hay cambios sin confirmar, **detente** e informa al usuario:
     *"Tienes cambios sin confirmar. Confirma o guarda tu trabajo antes de migrar, para poder revertir si algo sale mal."*
   - Si **no** hay repositorio git, advierte al usuario y **pídele confirmación explícita** antes de seguir.

2. **Inventaría las referencias actuales a la librería**. Busca en todos los `.csproj`, `.props`, `.fsproj`,
   `packages.config` y `Directory.Packages.props`:
   ```
   MoralesLarios.OOFP
   ```
   Anota, para cada proyecto: qué paquetes de la familia referencia y con qué versión.

3. **Captura la línea base de compilación** (para poder comparar al final):
   ```powershell
   dotnet build --nologo
   ```
   Si el proyecto ya **no** compila antes de empezar, dilo y pregunta si se sigue adelante.

4. **Detecta la versión de partida**. Si alguna referencia ya es `>= 2.0.0`, informa de que la migración
   podría ser parcial o innecesaria y pide confirmación.

5. **Crea una rama de trabajo** (si hay git):
   ```powershell
   git switch -c chore/oofp-upgrade-2
   ```

---

## Fase 1 — Actualizar los paquetes NuGet

### 1.1 Paquetes RENOMBRADOS (hay que desinstalar el antiguo e instalar el nuevo)

En la 2.0.0 **14 paquetes cambiaron de identificador**. El paquete antiguo **no** recibe la 2.0.0: hay que
**quitarlo** y **añadir el nuevo**.

| Paquete 1.x (ANTIGUO — quitar) | Paquete 2.0.0 (NUEVO — añadir) |
| --- | --- |
| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` |
| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` |
| `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` |
| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` |
| `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` |
| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` |
| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` |
| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` |
| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` |
| `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` |
| `MoralesLarios.OOFP.Validation.FluentValidations` | `MoralesLarios.OOFP.Validation.FluentValidation` |
| `MoralesLarios.OOFP.Validation.Dataannotations` | `MoralesLarios.OOFP.Validation.DataAnnotations` |
| `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` *(fusionado)* |
| `MoralesLarios.OOFP` *(paquete raíz)* | `MoralesLarios.OOFP.Core` |

> ℹ️ `MoralesLarios.OOFP.Validation.Dataannotations` → `...DataAnnotations` solo corrige la **capitalización**
> de la `A`. En NuGet el id es case-insensitive, pero el **namespace de C# NO lo es**: hay que corregirlo igual.

**Comandos**, por cada proyecto afectado:
```powershell
dotnet remove <PROYECTO.csproj> package <PAQUETE_ANTIGUO>
dotnet add    <PROYECTO.csproj> package <PAQUETE_NUEVO> --version 2.0.0
```

### 1.2 Paquetes que conservan el nombre (solo subir versión)

`MoralesLarios.OOFP.Core`, `MoralesLarios.OOFP.Shared`, `MoralesLarios.OOFP.Validation`,
`MoralesLarios.OOFP.ValueObjects`.

```powershell
dotnet add <PROYECTO.csproj> package <PAQUETE> --version 2.0.0
```

### 1.3 Reglas de actualización

- Usa **`--version 2.0.0`** como mínimo. Si el usuario pide "la última", resuelve la última estable con
  `dotnet package search <id> --exact-match` (o `dotnet list package --outdated`) y **nunca** instales una
  *prerelease* salvo petición explícita.
- Si el repo usa **Central Package Management** (`Directory.Packages.props` con `<PackageVersion>`),
  edita **ahí** las versiones y deja los `<PackageReference>` sin atributo `Version`.
- Si el repo usa `packages.config` (proyectos antiguos), migra con el gestor de NuGet; avisa al usuario
  porque `dotnet add package` no funciona ahí.
- **Target framework**: la 2.0.0 requiere **`net8.0` o superior**. Si algún proyecto apunta a `net6.0`/`net7.0`
  o a `netstandard2.x`, **detente** y avisa: *"El proyecto X apunta a `<TFM>`; MoralesLarios.OOFP 2.0.0
  requiere net8.0 o superior. Actualiza el TFM antes de continuar."*
- Tras esta fase, ejecuta `dotnet restore`. **No compiles todavía** (habrá cientos de errores; es lo esperado).

---

## Fase 2 — Reescribir `using` y namespaces

Aplica estos reemplazos en **todos** los `.cs`, `.cshtml`, `.razor`, `_Imports.razor`, `GlobalUsings.cs` y
`<Using Include="..."/>` de los `.csproj`.

> ⚠️ **ORDEN CRÍTICO**: aplica siempre **el prefijo más largo primero**. Si no, `MoralesLarios.OOFP.WebControllers`
> se transformaría parcialmente al procesar `MoralesLarios.OOFP.WebControllers.Cache`.
>
> **Técnica recomendada**: hazlo en **dos pasadas con tokens**. Primera pasada, antiguo → token único
> (`@@NS07@@`); segunda pasada, token → nuevo. Así se evita que un reemplazo pise a otro.

| # | Namespace 1.x | Namespace 2.0.0 |
| -- | --- | --- |
| 1 | `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` |
| 2 | `MoralesLarios.OOFP.Validation.FluentValidations` | `MoralesLarios.OOFP.Validation.FluentValidation` |
| 3 | `MoralesLarios.OOFP.Validation.Dataannotations` | `MoralesLarios.OOFP.Validation.DataAnnotations` |
| 4 | `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` |
| 5 | `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` |
| 6 | `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` |
| 7 | `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` |
| 8 | `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` |
| 9 | `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` |
| 10 | `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` |
| 11 | `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` |
| 12 | `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` |
| 13 | `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` |

### 2.1 Namespaces internos que también se movieron

| 1.x | 2.0.0 |
| --- | --- |
| `MoralesLarios.OOFP.HttpClients.ParamsInfo` | `MoralesLarios.OOFP.Http.Requests` |
| `MoralesLarios.OOFP.Internals.Info` | `MoralesLarios.OOFP.Shared.Pagination` |
| `MoralesLarios.OOFP.EFCore.Repos` | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` |
| `MoralesLarios.OOFP.EFCore.OopRepos` | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` |

### 2.2 Namespaces de la 2.0.0 (referencia completa, para resolver `using` que falten)

```
MoralesLarios.OOFP.Types                          (MlResult<T>, MlError, …)
MoralesLarios.OOFP.Types.Errors                   (MlErrorDetails, …)
MoralesLarios.OOFP.Helpers                        MoralesLarios.OOFP.Helpers.Extensions
MoralesLarios.OOFP.Application                    .Services   .Helpers
MoralesLarios.OOFP.AspNetCore.Controllers         .Controllers  .Attributes  .Helpers
MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching   .Controllers  .Policies
MoralesLarios.OOFP.AspNetCore.Results             .Data  .Helpers
MoralesLarios.OOFP.Configuration                  .Config
MoralesLarios.OOFP.EntityFrameworkCore            .Repositories  .Repositories.Imperative  .Enums  .Helpers
MoralesLarios.OOFP.FileSystem
MoralesLarios.OOFP.Http                           .Requests  .Helpers
MoralesLarios.OOFP.Logging
MoralesLarios.OOFP.Shared.Pagination              MoralesLarios.OOFP.Shared.Web
MoralesLarios.OOFP.Validation
MoralesLarios.OOFP.Validation.DataAnnotations     .Helpers
MoralesLarios.OOFP.Validation.FluentValidation    .Helpers
MoralesLarios.OOFP.ValueObjects                   MoralesLarios.OOFP.ValueObjects.FileSystem
```

---

## Fase 3 — Renombrar la API

**Lee el fichero anexo [`oofp-upgrade-2.renames.md`](./oofp-upgrade-2.renames.md)**: contiene el mapa completo
de renombrados organizado por paquete, con una columna que indica si el cambio tiene puente `[Obsolete]`.

### 3.1 Reglas de aplicación (LEER ANTES DE TOCAR CÓDIGO)

1. **Ordena los reemplazos de más largo a más corto.** Es imprescindible. Ejemplos de solapamiento real:
   - `GetHeaderInfoAsIntNotNegative` **antes** que `GetHeaderInfo`.
   - `GetHeaderPaginationInfo` y `GetHeaderPageInfo` **antes** que `GetHeaderInfo`.
   - `ProjectionSplitPlusAsync` → `ProjectionSplitAsync` → `ProjectionSplit` → `Projection`.
   - `CombineAsync` **antes** que `Combine`.
   - `BoolToResultAsync` **antes** que `BoolToResult`.
   - `EFRepoReaderPaginationFp` → … → `EFRepoBase` → `EFRepo`.
   - `FromStringLength` / `FromIntLength` / `FromDecimalLength` **antes** que `FromString` / `FromInt` / `FromDecimal`.

2. **Usa la técnica de dos pasadas con tokens** cuando un mismo identificador aparezca en los dos lados del
   mapa (caso típico de los value objects, donde `FromString` → `CreateFromString` **y** `ByString` → `FromString`):
   - Pasada A: `antiguo` → `@@T07@@`
   - Pasada B: `@@T07@@` → `nuevo`
   - Si usas tokens numerados, **procesa los de 2 dígitos antes que los de 1 dígito** (`@@T10@@` antes que `@@T1@@`).

3. **Respeta los límites de palabra.** Usa `\b` en las expresiones regulares. Nunca reemplaces subcadenas
   dentro de identificadores ajenos: `Name` → `MemberName` **no** debe convertir `UserName` en `UserMemberName`,
   ni `Key` → `ServiceKey` debe tocar `ApiKey` o `PrimaryKey`.

4. **Cambia solo los símbolos de la librería.** Si el usuario tiene una clase propia llamada `Name`, `Key`,
   `Empty`, `OrderBy` o `Combine`, **no la toques**. Antes de reemplazar un identificador genérico,
   verifica con "buscar referencias"/"ir a definición" que proviene de un namespace `MoralesLarios.OOFP.*`.
   Ante la duda, **pregunta al usuario**.

5. **Trabaja por tandas y compila entre tandas**. Orden recomendado:
   `Core` → `Shared` → `ValueObjects` → `ValueObjects.FileSystem` → `Validation*` → `FileSystem` →
   `Configuration` → `EntityFrameworkCore` → `Http` → `Logging` → `Application` → `AspNetCore.Results` →
   `AspNetCore.Controllers` → `AspNetCore.Controllers.OutputCaching`.

6. **No toques las cadenas de texto literales ni los comentarios** salvo que contengan nombres de API que
   dejarían de ser ciertos (p. ej. un mensaje de log que cite `NullToFailed`).

### 3.2 Cambios con SEMÁNTICA distinta (NO son un simple renombrado) ⚠️

Estos exigen revisión manual. **No los apliques a ciegas.**

#### a) Factorías de value objects: se invirtió el significado de `From…`

En la 1.x la convención era ambigua. En la 2.0.0 queda así:

| Prefijo 2.0.0 | Devuelve | Si el valor es inválido |
| --- | --- | --- |
| `CreateFrom…` | el **tipo directo** (`NotEmptyString`, `Age`, …) | **lanza excepción** |
| `From…` | **`MlResult<T>`** | devuelve un *fail*, no lanza |

Mapa de migración:

| Llamada 1.x | Llamada 2.0.0 | ¿Cambia el tipo devuelto? |
| --- | --- | --- |
| `X.FromString(v)` | `X.CreateFromString(v)` | No — sigue devolviendo `X` y lanzando |
| `X.ByString(v)` | `X.FromString(v)` | No — sigue devolviendo `MlResult<X>` |
| `X.FromStringLength(v, n)` | `X.CreateFromLength(v, n)` | No |
| `X.ByStringLength(v, n)` | `X.FromLength(v, n)` | No |
| `X.FromInt(v)` / `ByInt(v)` | `X.CreateFromInt(v)` / `X.FromInt(v)` | No |
| `X.FromIntLength(v,n)` / `ByIntLength(v,n)` | `X.CreateFromIntLength(v,n)` / `X.FromIntLength(v,n)` | No |
| `X.FromDecimal…` / `ByDecimal…` | `X.CreateFromDecimal…` / `X.FromDecimal…` | No |
| `X.FromDouble…` / `ByDouble…` | `X.CreateFromDouble…` / `X.FromDouble…` | No |
| `X.FromRegex(v,p)` / `ByRegex(v,p)` | `X.CreateFromRegex(v,p)` / `X.FromRegex(v,p)` | No |
| `X.FromEnum(v)` | `X.CreateFromEnum(v)` | No |
| `MlFile.FromString(p)` | `FilePath.CreateFromPath(p)` | No |
| `MlFile.ByString(p)` | `FilePath.FromPath(p)` | No |

> 🚨 **PELIGRO**: aplicar `FromString` → `CreateFromString` **y** `ByString` → `FromString` en una sola pasada
> produce `CreateFromString` → `CreateCreateFromString` o convierte los `ByString` en `CreateFromString`.
> **Usa obligatoriamente la técnica de dos pasadas con tokens.**
>
> Además, `FromIntLenght` y `FromStringLenght` (con la errata *Lenght*) existían en la 1.x:
> `FromIntLenght` → `CreateFromIntLength`, `FromStringLenght` → `CreateFromLength`.

También aparte:
- `Id.FromDouble(v)` → directa (lanza). `Id.ByDouble(v)` → segura (`MlResult<Id>`).
- `Empty` → `EmptyValue`, con factorías `EmptyValue.Create()` / `EmptyValue.CreateAsync()`.
- `Void.Create()` sin cambios.

#### b) Propiedades de `MlResult<T>`

| 1.x | 2.0.0 | Nota |
| --- | --- | --- |
| `res.IsValid` | `res.IsSuccess` | Mismo significado |
| `res.IsFail` | `res.IsFailure` | Mismo significado |
| `res.Value` | `res.SuccessValue` | **Sin puente.** Rompe la compilación |
| `res.ErrorsDetails` | `res.FailureDetails` | **Sin puente** |

⚠️ Si el código usa **reflexión** o `nameof` sobre esos nombres (`p.Name == "IsValid"`, `"Value"`),
hay que actualizarlo también: usa `nameof(MlResult<object>.IsSuccess)` y `nameof(MlResult<object>.SuccessValue)`.
Busca específicamente cadenas `"IsValid"`, `"IsFail"`, `"Value"`, `"ErrorsDetails"`.

#### c) `MlSelfValidatable<T>` pasó de clase abstracta a interfaz

```csharp
// 1.x
public class MyDto : MlValidableFp<MyDto> { … }          // o MlSelfValidatable<MyDto>
// 2.0.0
public class MyDto : IMlSelfValidatable<MyDto> { … }
```
Si el tipo heredaba miembros de la clase base, **ya no los hereda**: hay que implementarlos o reubicar la lógica.

#### d) `ToResponseErrorsDescription` desapareció en su versión síncrona

En la 1.x había una sobrecarga síncrona que internamente hacía `.Result` (riesgo de *deadlock*). En la 2.0.0
**solo existe la asíncrona**:
```csharp
// 1.x
var d = response.ToResponseErrorsDescription();
// 2.0.0
var d = await response.ReadErrorDescriptionAsync();
```
El método que la invoca debe pasar a `async`.

#### e) Parsing de fechas en `AspNetCore.Controllers`

`ConvertDateTime` → `ParseDateTimeInvariant`. Además, **cambió el comportamiento**: antes usaba
`CultureInfo.CurrentCulture` y ahora `CultureInfo.InvariantCulture`. Si dependías de la cultura del servidor
para parsear claves compuestas en la ruta, **revisa los formatos de fecha de tus URLs**.

#### f) Cabeceras de paginación

Los literales `"X-Page-Number"` y `"X-Page-Size"` ahora están centralizados:
```csharp
MlPaginationHeaders.PageNumber   // en lugar de "X-Page-Number"
MlPaginationHeaders.PageSize     // en lugar de "X-Page-Size"
```
Reemplaza los literales dispersos por las constantes.

#### g) Genéricos renombrados

`TReturn` → `TResult` y, en `Http`, `K` → `TResponse`, `TEnumrableResponse` → `TItem`. Solo afecta si
**implementas** interfaces/clases base de la librería o declaras *overrides*; una llamada normal no se entera.
No renombres los parámetros genéricos propios del usuario.

#### h) Contratos endurecidos (`abstract`, nulabilidad)

Varias clases base pasaron de `public class` a **`public abstract class`**
(`MlCrudControllerBase`, `MlCompositeKeyCrudControllerBase`, `MlCachedCrudControllerBase`,
`MlCachedCompositeKeyCrudControllerBase`). Si instanciabas una de ellas directamente, ya no se puede: hay que derivar.

Los constructores de `Age`, `Id` e `IdLite` pasaron de `public` a **`protected`**: usa las factorías
(`CreateFromInt`, `FromInt`, …) en lugar de `new Age(30)`.

`MinAllowedValue` de `DecimalNonNegative`/`DoubleNonNegative` pasó de propiedad estática mutable a
**`private const`**: ya no se puede leer ni asignar desde fuera.

#### i) Registro en el contenedor de dependencias (`IServiceCollection`)

| 1.x | 2.0.0 |
| --- | --- |
| `AddWebControllers()` | `AddMlControllers()` |
| `AddWebControllersCache()` | `AddMlControllersOutputCaching()` |
| `AddHttpClientsFp()` | `AddMlHttp()` |
| `AddMlUtilitiesConfig()` | `AddMlConfigurationReader()` |
| `AddGenClientFp<…>()` | `AddRestClient<…>()` |
| `AddGenClientComplexFp<…>()` / `AddGenClientDuplexComplexFp<…>()` | `AddCompositeKeyRestClient<…>()` |
| `AddScopedOOFPRepos()` | `AddOofpRepositoriesScoped()` |
| `AddSingletonOOFPRepos()` | `AddOofpRepositoriesSingleton()` |
| `AddTransientOOFPRepos()` | `AddOofpRepositoriesTransient()` |
| `AddOofpRepositoriesScoped()` *(si venías de una 1.x tardía)* | `AddOofpRepositories()` |
| `AddScopedtGenServicesFpWithoutReposGeneral()` | `AddScopedCrudServicesFpWithoutRepositories()` |
| `AddSingletonGenServicesFpWithoutReposGeneral()` | `AddSingletonCrudServicesFpWithoutRepositories()` |
| `AddTransientGenServicesFpWithoutReposGeneral()` | `AddTransientCrudServicesFpWithoutRepositories()` |
| `AddScopedtGenServicesDuplexFpWithoutReposGeneral()` | `AddScopedCrudServicesDuplexFpWithoutRepositories()` |
| `AddSingletonGenServicesDuplexFpWithoutReposGeneral()` | `AddSingletonCrudServicesDuplexFpWithoutRepositories()` |
| `AddTransientGenServicesDuplexFpWithoutReposGeneral()` | `AddTransientCrudServicesDuplexFpWithoutRepositories()` |

> ⚠️ Ojo con `AddOofpRepositoriesScoped`: en la 2.0.0 **existe**, pero el método que antes se llamaba así
> pasó a llamarse `AddOofpRepositories`. Revisa cuál quieres: `AddOofpRepositories()` registra con el *lifetime*
> por defecto; `AddOofpRepositoriesScoped()` es el antiguo `AddScopedOOFPRepos()`.

### 3.3 Cambios de COMPORTAMIENTO por corrección de bugs (auditoría 2.0.0) ⚠️⚠️

Además de los renombrados, la 2.0.0 **corrige defectos de la 1.x**. Esas correcciones son deliberadamente
**rupturas de comportamiento**: el código del consumidor sigue compilando en casi todos los casos, pero
**la salida observable cambia**. Si tienes tests que fijan la salida antigua (la incorrecta), fallarán.

> 🧭 Cómo leer la tabla: **«Rompe compilación»** = el consumidor tiene que tocar código o `.csproj`.
> **«Rompe contrato»** = compila igual, pero cambia lo que ve el cliente HTTP, la base de datos o el log.

| # | Qué cambió | 1.x (antes) | 2.0.0 (ahora) | Tipo de ruptura |
| -- | --- | --- | --- | --- |
| 17 | Cuerpo del error HTTP en el mensaje de fallo | se incrustaba el body **completo** | se **trunca a 2.048 caracteres** y se puede desactivar | Contrato (mensajes de log/fallo) |
| 27 | Composición de la URL en `Http` | `Path.Combine` → `api\users\3` en Windows | `Uri` → `api/users/3` siempre | Contrato (URL emitida) |
| 28 | Formato de las claves compuestas en la ruta | cultura **del hilo**, tipos limitados | `InvariantCulture` + `DateTimeOffset`/`TimeSpan`/`Guid`/`bool`/`IFormattable`, `null` → `""` | Contrato (URL emitida) |
| 29 | Cabeceras de paginación | en `HttpClient.DefaultRequestHeaders` | en el `HttpRequestMessage` de cada llamada | Contrato (ya no «se pegan» al cliente) |
| 31 | `Location` de los `201 Created` | `https://www.netalpunto.net` / `"NotUri"` | `/` (`WebErrorDetailsKeys.DefaultCreatedLocation`) | Contrato (cabecera `Location`) |
| 32 | `type` de `ProblemDetails` | `https://www.puntonetalpunto.net/` | `about:blank` (`WebErrorDetailsKeys.DefaultProblemType`) | Contrato (cuerpo RFC 9457) |
| 34 | Referencia a MVC en `AspNetCore.Results` y `AspNetCore.Controllers` | `PackageReference Microsoft.AspNetCore.Mvc.Core` 2.x | `FrameworkReference Microsoft.AspNetCore.App` | **Compilación** (transitividad) |
| 35 | `TryRemoveRangeAsync` | guarda sincrónica, `Task` sin `await` | guarda asíncrona esperada | Contrato (ahora sí corta con `null`) |
| 36 | `TotalCount` de la paginación sincrónica | contaba la tabla **completa**, ignorando el filtro | cuenta **con el filtro aplicado** | Contrato (nº de páginas) |
| 37 | Paginación imperativa | `OrderBy` **después** de `Skip`/`Take` | `OrderBy` **antes** de `Skip`/`Take` | Contrato (contenido de cada página) |
| 38 | Constantes y literales de mensaje del núcleo (**2.1.1**) | nombres `MAYUSCULAS`, `const`, mensajes con erratas | `PascalCase`, `static readonly`, mensajes corregidos | **Compilación** (`CS0618`) + Contrato (texto de los mensajes) + **binario** |
| 39 | El tipo `ErrorMessage` queda retirado (**2.1.1**) | `public record ErrorMessage(string Message)` público y vivo | `[Obsolete]` + `[EditorBrowsable(Never)]`; se usa `MlError`; se **elimina** en 3.0.0 | **Compilación** (`CS0618` en 2.1.1 → `CS0246` en 3.0.0) |
| 40 | Contenedor de metadatos de `MlErrorDetails` (**2.1.1**) | 7 defectos funcionales: claves de excepción mal discriminadas, `Merge` que lanza, `AddMetadataRange` NO-OP, lectores que mutan, diccionario compartido | claves `Ex`/`Ex2`/`Ex3` densas, `Merge` que sufija, lectores no destructivos, metadatos **inmutables**, igualdad **estructural** | ⚠️ **Contrato · 9 de 12 cambios son SILENCIOSOS** |
| 41 | `TryGetValue` / `TryGetErrors` y los convenios `Try*` / `Async` (**2.1.1**) | no existían; los dos convenios no estaban documentados | dos miembros nuevos con semántica BCL (`bool` + `out`) y los convenios documentados en el `README.md` del núcleo | ✅ **Ninguna** · aditivo y documental |
| 42 | Troceado de la clase `Extensions` del núcleo (**2.1.1**) | 11 métodos de extensión en una única clase `Extensions` | repartidos en `NullableExtensions`, `ExceptionMetadataExtensions`, `MutationExtensions` y `TaskLiftExtensions`; `Extensions` queda como puentes `[Obsolete]` | **Compilación** (`CS0618`) **solo** si se invocaban en forma **estática** |
| 43 | El miembro `_` de `MlResult` queda retirado (**2.1.1**) | `MlResult._` era la implementación y `Discard` su alias | `Discard` es la **implementación**; `_` queda como puente `[Obsolete]` y se **elimina** en 3.0.0 | **Compilación** (`CS0618` en 2.1.1 → `CS0117` en 3.0.0) |
| 44 | `LogMlResultIfFailWithExceptionAsync` sobre `Task<MlResult<T>>` (**2.2.0**) | delegaba en `LogMlResultIfFailWithValueAsync` y **nunca** registraba la excepción | delega en la sobrecarga correcta de su propia familia y **sí** registra la excepción | ⚠️ **Contrato · SILENCIOSA** (aparecen trazas donde antes no había ninguna) |
| 45 | Parámetro `validBuilMessage` de la familia `LogMlResult*` (**2.2.0**) | 107 parámetros públicos con la errata `Buil` | `validBuildMessage` | **Compilación** (`CS1739`) **solo** si usabas **argumentos con nombre** |
| 46 | Parámetro `faildMessage` de la familia `LogMlResult*` (**2.2.0**) | 84 parámetros públicos con la errata `faild` | `failBuildMessage` | **Compilación** (`CS1739`) **solo** si usabas **argumentos con nombre** |
| 47 | `MyMethodFinalLog[Async]` (**2.2.0**) | nombre sin relación con la familia `LogMlResult*` | `LogActionResult[Async]`; el antiguo queda como puente `[Obsolete]` y se **elimina** en 3.0.0 | **Compilación** (`CS0618` en 2.2.0 → `CS1061` en 3.0.0) |
| 48 | Clase estática `Extensions` de `MoralesLarios.OOFP.Logging` (**2.2.0**) | `public static class Extensions` | `MlResultLogActionExtensions`; `Extensions` queda como puente `[Obsolete]` | **Compilación** (`CS0618`) **solo** si se invocaba en forma **estática** |
| 49 | Idioma y texto de los mensajes que construye la librería (**2.2.0**) | mezcla de castellano e inglés y un `Error` duplicado en `MyMethodFinalLog` | **inglés** en todos los mensajes propios y prefijo `Error when <acción>: ` sin duplicar | ⚠️ **Contrato · SILENCIOSA** (cambia el texto escrito en el log) |
| 50 | La familia `LogMlResult*` consulta `IsEnabled` antes de construir el mensaje (**2.2.0**) | el constructor de mensaje se **invocaba siempre**, incluso con el nivel de log apagado | el constructor **no se invoca** si `logger.IsEnabled(logLevel)` devuelve `false` | ⚠️ **Contrato · SILENCIOSA** (los constructores con efectos colaterales dejan de ejecutarse) |
| 51 | Sobrecargas de `LogMlResult*` con `EventId` (**2.2.0**) | no existían | **35 firmas nuevas** que aceptan un `EventId` y lo trasladan al `ILogger` | ✅ **Ninguna** · aditivo |
| 52 | Sobrecargas de `LogMlResult*` con `ILogger<TCategory>` (**2.2.0**) | había que convertir el registrador tipado a `ILogger` | **21 firmas nuevas** que aceptan directamente el `ILogger<T>` del contenedor | ✅ **Ninguna** · aditivo |
| 53 | Familia `LogMlResult*Template` con **plantilla de mensaje** (**2.3.0**) | no existía: el mensaje se interpolaba antes de llegar al registrador y los datos se perdían dentro de la cadena | **35 firmas nuevas** que aceptan `string messageTemplate` + `params object?[] args` y delegan en `ILogger.Log` **sin interpolar**, de modo que los marcadores viajan como **campos estructurados** | ✅ **Ninguna** · aditivo |
| 54 | Plantillas en la **rama de fallo** y clase `MlResultLogErrorFields` (**2.3.0**) | la plantilla del punto 53 solo cubría la rama incondicional; el fallo seguía emitiendo un `string` plano | **172 firmas nuevas** en las cuatro familias `IfFail`, `IfFailWithValue`, `IfFailWithException` e `IfFailWithoutException`, más el tipo público `MlResultLogErrorFields`, que **fija los nombres de campo** `{ErrorMessage}`, `{ErrorCount}` y `{ExceptionType}` | ✅ **Ninguna** · aditivo, pero los **tres nombres de campo pasan a ser contrato** |
| 55 | Cruce del eje `EventId` con las plantillas de registro estructurado (**2.3.0**) | había que elegir: o `EventId` (punto 51) o plantilla con campos (punto 53), nunca las dos en la misma llamada | **35 firmas nuevas** en `MlResultLogExtensions.TemplateEventId.cs` que aceptan `EventId` **y** plantilla: 5 en la familia genérica `LogMlResultTemplate` y 5 en cada uno de los seis niveles | ✅ **Ninguna** · aditivo; corrige la nota del punto 51 que decía que los ejes «no se combinan» |
| 56 | Los **ámbitos de registro** (`ILogger.BeginScope`) siguen sin existir, y es **deliberado** (**2.3.0**) | el `README.md` decía solamente «no hay `scopes` ni `BeginScope`», sin decir si era un olvido o una decisión | el recuento de `BeginScope` en la librería es **`0`** y queda **documentado y justificado**: los ámbitos se han desgajado a la **épica 9 de la versión 2.4.0** porque plantean tres preguntas de diseño propias | ✅ **Ninguna** · no hay código nuevo; es **documentación de semántica no obvia** |
| 57 | Los parámetros de plantilla se anotan con `[StringSyntax("MessageTemplate")]` (**2.3.0**) | el parámetro `messageTemplate` era un `string` cualquiera: el editor no lo coloreaba y los analizadores no lo reconocían como plantilla de registro | **241 parámetros anotados** en los nueve ficheros de plantilla, de modo que el editor y los analizadores del consumidor tratan la cadena como lo que es; `CA2254 = 0` medido en una sonda con analizadores estrictos | ✅ **Ninguna** · anotación declarativa; no cambia ninguna firma ni ningún comportamiento |

#### 17 — El cuerpo del error HTTP se trunca

`ReadErrorDescriptionAsync` ya no incrusta cuerpos de error sin límite (antes un *stack trace* o un HTML de
error de 2 MB acababa íntegro en el log y en el mensaje de fallo).

```csharp
// Opción A: subir el límite en una llamada concreta
var d = await response.ReadErrorDescriptionAsync(maxBodyLength: 32_768);

// Opción B: desactivar globalmente la inclusión del cuerpo (Program.cs, arranque)
MlResponseWebExtensions.IncludeResponseBodyInErrors = false;   // útil si el body puede traer datos personales
```

**Qué revisar en tu código:** tests o alertas que hagan `Assert.Contains("<texto del body>", error)` sobre
respuestas de error largas, y cualquier parseo del mensaje de fallo. Si truncas, aparece el sufijo
`… [truncado: N caracteres omitidos]`.

#### 27 y 28 — Las URL que se generan son distintas

```
1.x (Windows) :  https://api/v1\clientes\3,14        ← '\' y coma decimal de es-ES
2.0.0         :  https://api/v1/clientes/3.14        ← '/' y punto decimal invariante
```

**Qué revisar en tu código:**
- Rutas del servidor que hicieran *matching* sobre el formato antiguo (p. ej. un `[Route]` que esperase
  `3,14`). Con la 2.0.0 hay que aceptar el formato invariante.
- Si tu `url` empezaba por `/` contando con que `Path.Combine` **descartase** la `BaseAddress`, ahora ya no se
  descarta: la URL se compone correctamente contra la base. **Cambia tus rutas relativas.**
- Si pasas una URL **absoluta** en el parámetro `url`, ahora gana sobre la `BaseAddress` (antes el resultado
  era basura).
- Tests que comparen la URL literal.

#### 29 — Las cabeceras de paginación ya no se quedan pegadas al `HttpClient`

Si tu código hacía esto, **deja de funcionar**:

```csharp
// 1.x — PATRÓN YA NO VÁLIDO
client.SetHeaderPageNumber(1);            // escribía en DefaultRequestHeaders…
var r = await client.GetAsync(url);       // …y la siguiente llamada la heredaba
```

En la 2.0.0 hay sobrecargas sobre `HttpRequestMessage`:

```csharp
using var request = new HttpRequestMessage(HttpMethod.Get, url);
request.AddPaginationHeaders(pageNumber, pageSize);     // o AddPageNumberHeader / AddPageSizeHeader
var response = await client.SendAsync(request, ct);
```

Las sobrecargas sobre `HttpClient` **siguen existiendo** para no romper la compilación, pero están
**desaconsejadas**: filtran cabeceras entre peticiones y no son seguras entre hilos. Migra a las de
`HttpRequestMessage`.

#### 31 y 32 — `Location` y `type` ya no apuntan al dominio del autor

Esto es un **cambio en la respuesta HTTP pública de tu API**:

```diff
- HTTP/1.1 201 Created
- Location: https://www.netalpunto.net
+ HTTP/1.1 201 Created
+ Location: /
```
```diff
  Content-Type: application/problem+json
- { "type": "https://www.puntonetalpunto.net/", "title": "...", "status": 400 }
+ { "type": "about:blank", "title": "...", "status": 400 }
```

**Qué revisar en tu código:**
- **Tests de API** (integración, contract tests, snapshots de Swagger/Pact) que fijen `type` o `Location`.
  Hay que actualizar el valor esperado a `about:blank` y `/`.
- **Clientes** que decidan en función de `problem.type`. Si tu API necesita un `type` propio y estable,
  **pásalo explícitamente**; no te apoyes en el valor por defecto.
- **`Location` correcto**: `/` es un relleno válido, no una URI útil. Usa las sobrecargas que reciben `Uri`
  (`ToPostActionResultAsync(uri)`) o `CreatedAtAction`/`CreatedAtRoute`. Si no tienes URI del recurso creado,
  considera devolver `200 OK` en lugar de un `201` con `Location` inventado.

#### 34 — MVC llega por el framework compartido, no por el paquete 2.x

`MoralesLarios.OOFP.AspNetCore.Results` y `MoralesLarios.OOFP.AspNetCore.Controllers` ya **no** arrastran
`Microsoft.AspNetCore.Mvc.Core` (estaba fuera de soporte desde 2018 y provocaba conflictos de versión).

**Esto rompe la compilación** si tu proyecto consumía tipos de MVC (`IActionResult`, `ControllerBase`,
`ProblemDetails`) **por transitividad** y su `.csproj` usa `Microsoft.NET.Sdk` (librería de clases normal).
Arréglalo de una de estas dos formas:

```xml
<!-- Opción A (recomendada si es un proyecto web) -->
<Project Sdk="Microsoft.NET.Sdk.Web">
```
```xml
<!-- Opción B (librería de clases que necesita tipos de ASP.NET Core) -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
```

> ❌ **No** vuelvas a añadir `<PackageReference Include="Microsoft.AspNetCore.Mvc.Core" Version="2.*" />`.
> Es la causa original del problema.

#### 35, 36 y 37 — La paginación de EF Core devuelve datos distintos (correctos)

| Síntoma 1.x | Causa | Efecto de la corrección |
| --- | --- | --- |
| El total de páginas no cuadraba con el filtro | el `Count` ignoraba el predicado | `TotalCount` **baja** al valor correcto; se muestran menos páginas |
| Al pasar de página se repetían o faltaban filas | `OrderBy` iba después de `Skip`/`Take` | cada página devuelve **otras filas** (las correctas) |
| `TryRemoveRangeAsync(null)` podía seguir | guarda sincrónica mal encadenada | ahora **corta** y devuelve un fallo |

**Qué revisar en tu código:** tests que fijen `TotalCount`, el número de páginas o el contenido exacto de una
página; capturas de pantalla o *snapshots* de listados paginados; y cualquier sitio donde se pasara `null`
al borrado por lotes contando con que no fallara.

> 📌 Si la 1.x **no** aplicaba orden y tú tampoco lo indicas, la 2.0.0 sigue sin poder inventarlo: **indica
> siempre una ordenación estable** (idealmente por clave primaria) en las consultas paginadas. Sin `ORDER BY`,
> SQL Server no garantiza el reparto en páginas.

---

#### 38 — Constantes y literales de mensaje del núcleo (2.1.1) ⚠️

La **2.1.1** normaliza `MoralesLarios.OOFP.Helpers.Constants`: los nombres en
`MAYUSCULAS_CON_GUION_BAJO` pasan a `PascalCase`, los miembros se declaran de forma homogénea como
`public static readonly string` y el único método que había sale de la clase.

| Elemento | 2.0.0 | 2.1.1 / 3.0.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| `Constants.DEFAULT_ERROR_MESSAGE` (nombre) | propiedad | `[Obsolete]` → `Constants.DefaultErrorMessage` | ✅ `CS0618` |
| `DEFAULT_ERROR_MESSAGE` (**valor**) | `…view 'Ex(s) details exceptions.` | `…view the 'Ex' details exceptions.` | ❌ **NO** |
| `DEFAULT_EX_ERROR_MESSAGE` (**valor**) | `…{ex.Message}.More info in Ex Details.` | `…{ex.Message}. More info in 'Ex' details.` | ❌ **NO** |
| `NOT_FOUND_KEY`, `PARAM_NAME_KEY`, `FAILED_INDEXES_KEY`, `EXPECTED_KEY` | `public const string` | `[Obsolete]` propiedad → campo `PascalCase` | ✅ fuente · ❌ **binario** |
| `ExceptionMetadataKey` / `ValueMetadataKey` | propiedad | campo `static readonly` | ✅ fuente · ❌ **binario** |
| `DEFAULT_EX_ERROR_MESSAGE(ex)` (ubicación) | `Constants` | `EnsureFpMessages.DefaultExceptionError(ex)` | ✅ `CS0618` |
| `MlResult<T>.ToString()` con valor `null` | `"Not right value"` | `"<null>"` | ❌ **NO** |

**Qué revisar en tu código:**
- **Tests y aserciones que comparen literalmente** los mensajes por defecto o el `ToString()` de un
  `MlResult<T>` válido con valor `null`: el compilador **no** avisa, los tests fallarán en ejecución.
- Si consumías las cuatro claves como `const`, **recompila**: eran constantes y su valor quedaba
  horneado en tu ensamblado. El cambio es compatible en código fuente, pero **no en binario**.
- Sustituye los nombres antiguos por los `PascalCase` de la tabla (ver el anexo de renombrados).
  Los puentes `[Obsolete]` se eliminan en la **3.0.0**.

> 🆕 `EnsureFpMessages` pasa de `internal` a **`public`**, pero solo expone los cuatro miembros nuevos
> (`DefaultExceptionError`, `NoValueRepresentation`, `ErrorsSectionHeader`, `MetadataSectionHeader`).
> El resto de la clase sigue siendo `internal`.
>
> `EnsureFpMessages.DefaultExceptionError(null)` lanza `ArgumentNullException` (antes, a través de
> `Constants.DEFAULT_EX_ERROR_MESSAGE(null)`, era un `NullReferenceException`).

#### 39 — El tipo `ErrorMessage` queda retirado (2.1.1 → 3.0.0) ⚠️

`MoralesLarios.OOFP.Types.Errors.ErrorMessage` era un `record` de un solo campo (`Message`) sin
cometido propio dentro del modelo de error: el tipo canónico es **`MlError`**. En la **2.1.1** se
marca como puente y en la **3.0.0** el fichero desaparece.

| Elemento | 2.0.0 | 2.1.1 | 3.0.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- | --- |
| `ErrorMessage` (tipo) | `public record` visible | `[Obsolete]` + `[EditorBrowsable(Never)]` | **eliminado** | ✅ `CS0618` en 2.1.1 · `CS0246` en 3.0.0 |
| Firma del constructor | `ErrorMessage(string Message)` | **idéntica** | — | — |

La firma **no cambia** en la 2.1.1, así que el puente es de uso transparente: el código que lo
utilizase sigue compilando, solo emite `CS0618`.

**Qué revisar en tu código:**
- Sustituye `new ErrorMessage(texto)` por `MlError.FromErrorMessage(texto)` (o por el `string`
  desnudo, si solo necesitabas el texto). `MlError` admite conversión implícita desde `string`.
- Si tenías el tipo en una **firma pública propia**, cambiarla es una ruptura para *tus* consumidores:
  planifica el cambio antes de la 3.0.0.
- El tipo ya no aparece en IntelliSense por `[EditorBrowsable(Never)]`; si lo buscas y no lo ves,
  es intencionado.

> ⚠️ Ojo con los falsos positivos al buscar: `ErrorMessage` es también el nombre de una **propiedad**
> de `System.ComponentModel.DataAnnotations.ValidationResult` y de `FluentValidation.ValidationFailure`.
> Esas propiedades **no tienen nada que ver** con este tipo y no hay que tocarlas. Busca con límites
> de palabra y distinguiendo mayúsculas, y descarta las apariciones precedidas por un punto.

---

#### 40 — El contenedor de metadatos de `MlErrorDetails` (2.1.1) ⚠️⚠️ CAMBIOS SILENCIOSOS

> ⚠️ **Este es el punto más peligroso de toda la guía.** La 2.1.1 corrige **siete defectos
> funcionales** del diccionario `Metadata` de `MlErrorDetails`. **Nueve de los doce cambios no los
> detecta el compilador**: tu código compila igual y se comporta distinto. Si tienes tests que fijan
> el comportamiento antiguo (el incorrecto), fallarán en ejecución.

| Elemento | 2.0.0 | 2.1.1 | 3.0.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- | --- |
| Clave asignada a una excepción cuando ya hay una clave que **empieza** por `Ex` (`"Expected"`, `"ExecutionTime"`, `"ExternalId"`) | `"Ex2"` | **`"Ex"`** | ranura tipada | ❌ **NO** |
| Esquema de numeración al fusionar excepciones | `Ex`, `Ex1`, `Ex2` | **`Ex`, `Ex2`, `Ex3`** (denso, sin `Ex1`) | ranura tipada | ❌ **NO** |
| `Merge` con una clave **no** de excepción repetida | lanza `ArgumentException` | **sufija la entrante** (`ParamName`, `ParamName2`) | ídem | ❌ **NO** |
| Las **dos** sobrecargas de `AddMetadataRange` | **NO-OP** (descartaban lo recibido) | **funcionan** | `[Obsolete]` → `WithMetadata` | ❌ **NO** |
| `ToMlErrorsDetails` de la tupla `(MlError, Dictionary<string, object>)` | **perdía** los detalles | **los propaga** | ídem | ❌ **NO** |
| `GetDetail<T>` / `GetMetadataValue<T>` / `GetException*` con clave ausente | **mutaban** el objeto consultado (le añadían el error) | **no lo mutan**, son idempotentes | ídem | ❌ **NO** |
| `Errors` | `IEnumerable<MlError>` perezoso | **materializado** (mismo tipo público) | `IReadOnlyList<MlError>` | ❌ 2.1.1 · ✅ 3.0.0 (`CS0266`) |
| Igualdad de `MlErrorDetails` (`Equals`, `GetHashCode`, `Contains`, claves de diccionario) | **por referencia** | **estructural** (`IEquatable<MlErrorDetails>`) | ídem | ❌ **NO** |
| Retorno de los mutadores (`AddValueDetailIfFail`, `AddMetadataRange`, `AddError*`…) | se podía **descartar**: mutaban el diccionario compartido | **hay que asignarlo**; devuelven instancia nueva | ídem | ❌ **NO** |
| Diccionario de metadatos entre instancias | **compartido** por referencia | **copiado** siempre | ídem | ❌ **NO** |
| `Metadata` (tipo de la propiedad) | `Dictionary<string, object>` | igual + nueva `MetadataView` | **`IReadOnlyDictionary`** | ✅ 3.0.0 (`CS0266` / `CS1061`) |
| `AddMetadataOrThrow`, `AddMetadataRange`, `AddValueMetadata` | mutadores sin aviso | mutadores + `<remarks>` que remite a `WithMetadata` | **`[Obsolete]`** | ✅ 3.0.0 (`CS0618`) |

**API nueva (aditiva) que deberías empezar a usar:**

```csharp
// Lectura: no lanza y no muta
if (details.TryGetMetadata<int>("Expected", out var expected))      { /* … */ }
if (details.TryGetException(out var ex))                            { /* … */ }
foreach (var e in details.GetExceptions())                          { /* todas, en orden */ }
if (details.HasMetadata(MlErrorDetails.MetadataKeys.ParameterName)) { /* … */ }

// Escritura: siempre devuelve una instancia NUEVA
var conMas = details.WithMetadata("Reintentos", 3);                       // ⚠️ asigna el retorno
var conVarias = details.WithMetadata(new Dictionary<string, object> { ["A"] = 1 });

// Catálogo de claves bien conocidas: deja de escribirlas a mano
MlErrorDetails.MetadataKeys.Exception;        // "Ex"
MlErrorDetails.MetadataKeys.Value;            // "Value"
MlErrorDetails.MetadataKeys.ParameterName;    // "ParamName"
MlErrorDetails.MetadataKeys.Expected;         // "Expected"
MlErrorDetails.MetadataKeys.FailedIndexes;    // "FailedIndexes"
MlErrorDetails.MetadataKeys.NotFound;         // "NotFound"
MlErrorDetails.MetadataKeys.ProblemsDetails;  // "ProblemsDetails" (capa web)

// Lectura de solo lectura del diccionario completo
IReadOnlyDictionary<string, object> vista = details.MetadataView;
```

**Qué revisar en tu código:**

1. **Claves de excepción leídas a mano.** Si en algún sitio haces `Metadata["Ex2"]`,
   `Metadata.Keys.Where(k => k.StartsWith("Ex"))` o `Metadata.ContainsKey("Ex1")`, **sustitúyelo**
   por `GetExceptions()` o `TryGetException(out …)`. El esquema `Ex1` ya no existe y `"Ex2"` solo
   aparece cuando hay **dos** excepciones de verdad.
2. ⚠️ **Mutadores cuyo retorno descartabas.** Busca líneas de la forma
   `detalles.AddValueDetailIfFail(x);` o `detalles.AddMetadataRange(d);` **sin** asignación: antes
   «funcionaban» por efecto colateral y ahora **no hacen nada visible**. Tienen que quedar como
   `detalles = detalles.AddValueDetailIfFail(x);`. Este fue exactamente el fallo que destapó la
   corrección dentro del propio núcleo, y el compilador **no lo señala**.
3. **Metadatos que esperabas compartir.** Si guardabas una referencia al `Dictionary` de un
   `MlErrorDetails` y lo mutabas luego esperando que el cambio se viese en el objeto, ya no ocurre.
   Reconstruye con `WithMetadata`.
4. **`try/catch` alrededor de `Merge`.** Si capturabas `ArgumentException` por claves duplicadas,
   ese `catch` queda muerto: `Merge` ya no lanza, sufija la clave entrante.
5. **Comparaciones de `MlErrorDetails`.** `Equals` y `GetHashCode` pasan a ser **estructurales**. Dos
   instancias distintas con el mismo contenido ahora son iguales y colapsan en un `HashSet` o como
   clave de un `Dictionary`. Si dependías de la identidad por referencia, usa
   `ReferenceEquals`. Los operadores `==` / `!=` **no** se han sobrecargado, así que siguen
   comparando referencias.
6. **Tests que esperaban la mutación del lector.** Si algún test afirmaba que tras un
   `GetDetail<T>("ausente")` el origen tenía un error más, hay que corregirlo: el origen no cambia.
7. **Rendimiento/orden de `Errors`.** El contenido y el orden no cambian; solo deja de reevaluarse en
   cada enumeración. Si medías el número de evaluaciones de una proyección, el test cambia.

> 🧭 **Comparador de claves**: es `StringComparer.Ordinal`, ahora **explícito**. El comportamiento es
> el mismo que en 2.0.0 (`"Value"` y `"value"` siguen siendo claves distintas), pero ya está
> documentado y no depende del valor por defecto del `Dictionary`.
>
> 🧭 **Prefijo reservado**: el patrón `^Ex\d*$` queda **reservado** al núcleo. No uses `"Ex"`, `"Ex7"`
> ni similares como claves propias; cualquier otra clave que *empiece* por `Ex` (`"ExportedAt"`,
> `"Expected"`) es perfectamente válida y ya **no** se confunde con una excepción.

---

#### 41 — `TryGetValue` / `TryGetErrors` y los convenios `Try*` / `Async` (2.1.1) ✅ SIN RUPTURAS

> ✅ **Este punto no rompe nada.** Es **puramente aditivo** y documental. No hay que cambiar ni una
> línea de código para migrar. Se incluye aquí porque aclara dos convenios de nomenclatura que son
> fuente habitual de errores por analogía con la BCL.

| Elemento | 2.0.0 | 2.1.1 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| `MlResult<T>.TryGetValue(out T value)` | no existe | **nuevo** · `bool` + `out`, semántica BCL | — (aditivo) |
| `MlResult<T>.TryGetErrors(out MlErrorDetails errorsDetails)` | no existe | **nuevo** · `bool` + `out`, semántica BCL | — (aditivo) |
| Familias `Try*` (`TryBind`, `TryMap`, `TryMatch`, `TryExecSelf`, `TryConvert`, `EnsureFp.Try*`) | sin documentar | `<remarks>` por familia que contrasta su semántica con la de la BCL | — (solo documentación) |
| 248 sobrecargas `*Async` sin asincronía real | sin documentar | `<returns>` que declara que la tarea **ya está completada** | — (solo documentación) |

**Los dos convenios, en una frase cada uno:**

1. **`Try*` de OOFP ≠ `Try*` de la BCL.** En la BCL, `Try*` significa *«devuelvo `bool` y el
   resultado por `out`, y no lanzo»*. En OOFP significa *«envuelvo la ejecución en `try/catch` y
   convierto la excepción en un fallo de `MlResult`»*: devuelve un `MlResult<T>`, no un `bool`, y no
   usa parámetros `out`. Son más de mil métodos y **no se renombran** (decisión D-04).
2. **`Async` de OOFP no garantiza asincronía real.** 248 de las 3.436 firmas que devuelven
   `Task<...>` (un 7,2 %) no realizan ninguna operación asíncrona: devuelven
   `Task.FromResult(...)`. El sufijo existe para poder **iniciar** una cadena que continúa con
   combinadores asíncronos. Ninguna de ellas admite cancelación.

**API nueva (aditiva):**

```csharp
// Semántica BCL para los dos casos donde realmente aplica
if (resultado.TryGetValue(out var valor))          { /* camino feliz */ }
if (resultado.TryGetErrors(out var errorsDetails)) { /* camino de fallo */ }
```

`TryGetValue` y `TryGetErrors` son **complementarios y exhaustivos**: para un mismo `MlResult<T>`
exactamente uno de los dos devuelve `true`. Son los **únicos** miembros de la biblioteca con
semántica `Try*` de la BCL.

**Qué revisar en tu código:**

1. **Nada es obligatorio.** Tu código sigue compilando y comportándose igual.
2. **Opcional · desenvolver un resultado.** Si usabas `Match`, `IsValid` + acceso a la propiedad o un
   `if` sobre el estado solo para extraer el valor, `TryGetValue` / `TryGetErrors` expresan lo mismo
   de forma más corta e idiomática para quien viene del ecosistema .NET.
3. **Opcional · `ConfigureAwait` y cancelación.** Si añadías `ConfigureAwait(false)` o esperabas
   poder cancelar una de las 248 sobrecargas sincrónicas, ahora está documentado que no sirve de
   nada: la tarea ya está completada cuando el método retorna.

> 🧭 **Dónde está documentado**: sección «Convenios de nomenclatura» del `README.md` del núcleo
> (`src/MoralesLarios.OOFP.Core/README.md`, §15) y el inventario completo de las 248 sobrecargas en
> `src/docs_next/Mejoras/Core/06-Rendimiento.md`.
>
> 🧭 **Aplazado a 3.0.0 (no se hace en 2.1.1)**: pasar esas 248 firmas de `Task<T>` a `ValueTask<T>`
> y excluir `OperationCanceledException` del `catch` de los `Try*`. Ambas son rupturas y están
> remitidas a sus documentos de mejora.

#### 42 — La clase `Extensions` del núcleo se ha troceado (2.1.1) ⚠️ SOLO SI LA USABAS EN FORMA ESTÁTICA

> ✅ **Si invocabas estos métodos como métodos de extensión (`valor.ToNullable()`), no tienes que
> cambiar nada.** El espacio de nombres **no cambia** y la resolución de métodos de extensión busca
> en **todas** las clases estáticas del espacio de nombres, no en una concreta.
>
> ⚠️ **Si los invocabas en forma estática (`Extensions.ToNullable(valor)`), obtendrás `CS0618`.**

La clase cajón de sastre `MoralesLarios.OOFP.Helpers.Extensions.Extensions` tenía 11 métodos de
extensión sin relación entre sí. En la 2.1.1 se reparten en cuatro clases con nombre propio:

| Clase nueva | Métodos | Qué familia cubre |
| --- | --- | --- |
| `NullableExtensions` | `ToNullable<T>` | elevación a `Nullable<T>` |
| `ExceptionMetadataExtensions` | `AppendExDetails` | volcado de excepciones a metadatos de error |
| `MutationExtensions` | `With<T>`, `WithAsync<T>` ×2 | mutación encadenable de objetos |
| `TaskLiftExtensions` | `VoidToAsync<T>`, `ToFuncTask` ×5 | elevación de delegados y acciones a `Task` |

**Espacio de nombres (sin cambios):** `MoralesLarios.OOFP.Helpers.Extensions`. Está en el
`global using` del núcleo, así que sigue cubriendo las cuatro clases nuevas.

```csharp
// ✅ Sigue compilando sin tocar nada — forma de extensión
int?                 n = 42.ToNullable();
var                  c = cliente.With(x => x.Nombre = "Ana");
Func<int, Task<int>> f = ((Func<int, int>)(x => x + 1)).ToFuncTask();

// ⚠️ Genera CS0618 (advertencia, NO error) — forma estática
int? n2 = Extensions.ToNullable(42);

// ✅ Migración: usa la clase nueva, o pásalo a forma de extensión
int? n3 = NullableExtensions.ToNullable(42);
```

**Los 11 puentes** viven en `Helpers/Extensions/Extensions.Obsolete.cs`, están marcados con
`[Obsolete("Moved to <Clase>. Will be removed in 3.0.0.", false)]` (advertencia, no error) y con
`[EditorBrowsable(EditorBrowsableState.Never)]` para que no ensucien IntelliSense. **Desaparecen en
la 3.0.0.**

> ⚠️ **Nota sobre `With<T>` / `WithAsync<T>`:** su nombre sugiere una copia al estilo `with` de los
> `record`, pero **mutan la instancia recibida** y devuelven **la misma referencia**. Esto **no
> cambia** en la 2.1.1 (decisión D-06), pero ahora está documentado de forma explícita en el
> `<remarks>` de `MutationExtensions`. Si tu código asumía semántica de copia, tenías un defecto
> latente.

**Qué revisar en tu código:**

1. Busca invocaciones en forma estática: `Extensions.ToNullable(`, `Extensions.With(`,
   `Extensions.WithAsync(`, `Extensions.AppendExDetails(`, `Extensions.VoidToAsync(`,
   `Extensions.ToFuncTask(`. Sustitúyelas por la clase nueva o por la forma de extensión.
2. Si tratas las advertencias como errores (`TreatWarningsAsErrors`), esas llamadas **romperán la
   compilación**. Migra o añade `#pragma warning disable CS0618` de forma temporal y localizada.
3. Revisa los usos de `With<T>` que asumieran que no se muta el origen.

#### 43 — El miembro `MlResult._` queda retirado en favor de `Discard` (2.1.1 → 3.0.0)

`MlResult` ofrecía **dos** formas de obtener «resultado válido sin valor relevante»: `MlResult._`
(la implementación) y `MlResult.Discard` (un alias que se limitaba a reenviar a `_`). Un identificador
de un solo carácter es, además, el **patrón de descarte** del propio lenguaje (`_ => ...`,
`out _`), lo que hacía el código ilegible y los barridos de búsqueda poco fiables. En la **2.1.1** se
**invierte la relación**: `Discard` pasa a ser la implementación y `_` queda como puente.

| Elemento | 2.0.0 | 2.1.1 | 3.0.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- | --- |
| `MlResult.Discard` | alias: `=> _` | **implementación**: `=> new(default!)` | implementación | — (mismo valor) |
| `MlResult._` | implementación | `[Obsolete]` + `[EditorBrowsable(Never)]`, reenvía a `Discard` | **eliminado** | ✅ `CS0618` en 2.1.1 · `CS0117` en 3.0.0 |

El **valor devuelto es idéntico** y `MlResult` es un `record` con igualdad estructural, así que
`MlResult._ == MlResult.Discard`. **No hay ninguna ruptura de comportamiento**: la ruptura es
exclusivamente de nombre y la señala el compilador.

**Qué revisar en tu código:**

- Sustituye `MlResult._` por `MlResult.Discard`. Es un reemplazo **mecánico y seguro**.
- El miembro ya no aparece en IntelliSense por `[EditorBrowsable(Never)]`; si lo buscas y no lo ves,
  es intencionado.
- Si tratas las advertencias como errores (`TreatWarningsAsErrors`), `CS0618` **romperá la
  compilación**: migra, no silencies.

> ⚠️ **Cuidado con los falsos positivos al buscar.** `_` es el patrón de descarte de C# y aparece
> constantemente en código que usa esta librería (`.Bind(_ => ...)`, `.Map(_ => ...)`,
> `out _`, `_ => default`). **Solo** hay que cambiar las apariciones **cualificadas por el tipo**:
> `MlResult._`. Busca `MlResult\s*\.\s*_\b`, nunca `_` a secas.

> 🧭 **`Empty()` y `EmptyAsync()` NO se tocan en la 2.1.1.** Aunque también representan «resultado
> válido sin valor útil», devuelven `MlResult<object>` (genérico), **no `MlResult`**, así que **no son
> alias de `Discard`** y la sustitución no es mecánica: sobre el `MlResult` no genérico no hay
> combinadores `Bind`/`Map`, por lo que `MlResult.Empty().Bind(...)` **no** se puede reescribir como
> `MlResult.Discard.Bind(...)`. Su retirada se aplaza a la **3.0.0**, cuando exista un tipo `Unit`
> (decisión D-12). Si hoy empiezas cadenas con `MlResult.Empty()`, **sigue siendo correcto**.

---

#### 44 — `LogMlResultIfFailWithExceptionAsync` sobre una `Task` ya registra la excepción (2.2.0)

La familia `LogMlResultIfFailWithException*` del proyecto `MoralesLarios.OOFP.Logging` registra una traza
**solo cuando el detalle de error lleva una excepción** en sus metadatos. De sus seis sobrecargas, la que
recibe un `Task<MlResult<T>>` y un constructor de mensaje `Func<MlErrorDetails, Exception, string>`
delegaba, por una **llamada cruzada**, en `LogMlResultIfFailWithValueAsync`. La resolución de sobrecargas
resolvía ese destino con `TValue = Exception`, de modo que el método solo registraba algo si la clave de
metadatos `Value` contenía una excepción — algo que **ninguna factoría de `MlErrorDetails` produce**. El
resultado práctico: **esa sobrecarga no registraba nunca nada**.

| Elemento | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| `LogMlResultIfFailWithExceptionAsync(this Task<MlResult<T>>, ILogger, LogLevel, Func<MlErrorDetails, Exception, string>)` | delegaba en `LogMlResultIfFailWithValueAsync` → **0 trazas** | delega en `LogMlResultIfFailWithExceptionAsync` → **1 traza** cuando hay excepción | ❌ **No** · la firma, el tipo de retorno y el valor devuelto son idénticos |
| Las otras 5 sobrecargas de la familia | correctas | **sin cambios** | — |

**Por qué es silenciosa.** La firma no cambia, el `MlResult<T>` devuelto es el mismo y el código del
consumidor compila sin tocar una línea. Lo único que cambia es la **salida observable del log**: donde
antes no se escribía nada, ahora se escribe la traza que siempre debió escribirse.

**Qué revisar en tu código:**

- Busca las llamadas a `LogMlResultIfFailWithExceptionAsync` cuyo receptor sea una expresión de tipo
  `Task<MlResult<T>>` (es decir, **sin `await` delante**) y que pasen un constructor de mensaje de dos
  parámetros. Son las afectadas.
- Revisa los **tests que afirman ausencia de traza** (`Entries.Should().BeEmpty()`, contadores a `0`,
  `Verify(..., Times.Never())` sobre `ILogger.Log`) alrededor de esas llamadas: si fijaban el
  comportamiento defectuoso, **ahora fallarán** y hay que invertir la aserción.
- Revisa los **volúmenes de log y las alertas** basadas en recuento de errores: un servicio que usara esa
  sobrecarga en un camino caliente puede empezar a emitir trazas de nivel `Error` que antes no emitía.
- Si tu intención real era registrar el **valor** y no la excepción, cambia explícitamente la llamada a
  `LogMlResultIfFailWithValueAsync<T, TValue>` con los genéricos **expresos**: sin ellos el compilador
  no puede inferir `TValue` y emite `CS1593`.

**Cómo detectarla:**

```
LogMlResultIfFailWithExceptionAsync
```

y, sobre cada coincidencia, comprobar si el receptor es una `Task<MlResult<T>>`. Complementariamente, en
la propia librería el barrido que garantiza que el defecto no ha vuelto es:

```
LogMlResultIfFailWithValueAsync
```

dentro de los cuerpos de los métodos `…IfFailWithException*` → debe dar **0 ocurrencias**.

> 🧭 **Lo que NO rompe en la 2.2.0.** La validación de argumentos nulos de esta misma versión
> (`logger` o constructor de mensaje nulos) se resolvió con la política «**no registrar y devolver el
> `source` intacto**», de modo que **no altera** ningún comportamiento previo: antes de la 2.2.0 esos
> casos lanzaban `NullReferenceException`, y lanzar menos excepciones no es una ruptura. Tampoco rompe el
> renombrado del parámetro de lambda `errorDetals` → `errorDetails`, porque es un **nombre local**, no un
> parámetro público.

#### 45 — El parámetro `validBuilMessage` pasa a llamarse `validBuildMessage` (2.2.0)

Toda la familia `LogMlResult*` del proyecto `MoralesLarios.OOFP.Logging` recibía el constructor del
mensaje del caso válido en un parámetro llamado `validBuilMessage`, con una **errata** heredada desde la
primera versión (`Buil` por `Build`). En la 2.2.0 pasa a llamarse `validBuildMessage` en los **107**
parámetros públicos afectados, repartidos por los siete ficheros de la familia.

| Elemento | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| Llamada con argumento **posicional** | `src.LogCompletedResult(log, v => ..., f => ...)` | **idéntica** | — · no hay nada que cambiar |
| Llamada con argumento **con nombre** | `validBuilMessage: v => ...` | `validBuildMessage: v => ...` | ✅ **Sí** · `CS1739` |

**Por qué no hay puente.** C# no admite alias de parámetro: no existe atributo ni sobrecarga que acepte
los dos nombres a la vez sin duplicar las 107 firmas. La ruptura se asume en la 2.2.0 porque el uso de
argumentos con nombre en estas llamadas es minoritario y, cuando ocurre, el compilador la señala con
`CS1739` en la línea exacta.

**Cómo detectarla:**

```
validBuilMessage:
```

Las coincidencias **sin** los dos puntos son declaraciones de la propia librería y no aparecen en código
de consumidor.

#### 46 — El parámetro `faildMessage` pasa a llamarse `failBuildMessage` (2.2.0)

La misma familia recibía el constructor del mensaje del caso fallido en un parámetro llamado
`faildMessage` (errata de `fail`). En la 2.2.0 pasa a llamarse `failBuildMessage` en los **84**
parámetros públicos afectados.

| Elemento | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| Llamada con argumento **posicional** | `src.LogCompletedResult(log, v => ..., f => ...)` | **idéntica** | — · no hay nada que cambiar |
| Llamada con argumento **con nombre** | `faildMessage: f => ...` | `failBuildMessage: f => ...` | ✅ **Sí** · `CS1739` |

⚠️ **No lo confundas con `failMessage`**, el parámetro `string` que lleva el mensaje **fijo** del caso
fallido en las sobrecargas que no reciben una lambda: ése **no cambia**. El convenio final de la 2.2.0
queda así:

| Parámetro | Tipo | Cuándo |
| --- | --- | --- |
| `validBuildMessage` | `Func<T, string>` | constructor del mensaje del caso válido |
| `failBuildMessage` | `Func<MlErrorDetails, string>` | constructor del mensaje del caso fallido |
| `failBuildMessage` | `Func<MlErrorDetails, Exception, string>` | ídem, con la excepción |
| `failMessage` | `string` | mensaje **fijo** del caso fallido · ✅ **no se toca** |

**Cómo detectarla:**

```
faildMessage:
```

#### 47 — `MyMethodFinalLog[Async]` pasa a llamarse `LogActionResult[Async]` (2.2.0)

El par de extensiones que registra el desenlace de una acción completa se llamaba `MyMethodFinalLog` y
`MyMethodFinalLogAsync`, nombres que no seguían el convenio `LogXxx` del resto del proyecto y que
hablaban del **método que llama** en lugar de lo que se registra. En la 2.2.0 pasan a `LogActionResult`
y `LogActionResultAsync`.

| Elemento | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| `source.MyMethodFinalLog(logger, desc)` | nombre vivo | puente `[Obsolete]` + `[EditorBrowsable(Never)]` | ✅ **Sí** · `CS0618` en 2.2.0, `CS1061` en 3.0.0 |
| `sourceAsync.MyMethodFinalLogAsync(logger, desc)` | nombre vivo | ídem | ✅ **Sí** |
| `source.LogActionResult(logger, desc)` | — | nombre final | — |

El puente delega literalmente en el nombre nuevo, así que el comportamiento es el mismo salvo el texto
del mensaje (punto 49). **Se elimina en la 3.0.0.**

**Cómo detectarla:**

```
MyMethodFinalLog
```

#### 48 — La clase `Extensions` de `MoralesLarios.OOFP.Logging` pasa a `MlResultLogActionExtensions` (2.2.0)

Igual que ocurrió en el núcleo con el punto 42, el proyecto de logging tenía una clase estática
llamada simplemente `Extensions`, un nombre que no dice nada y que colisiona con cualquier otra
`Extensions` importada. En la 2.2.0 el contenido vive en `MlResultLogActionExtensions` y `Extensions`
queda como **tipo puente** marcado `[Obsolete]` y `[EditorBrowsable(Never)]`.

| Forma de uso | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| Como **método de extensión** (`source.MyMethodFinalLog(...)`) | resuelve en `Extensions` | resuelve en `MlResultLogActionExtensions` | — · el nombre de la clase es irrelevante |
| Invocación **estática** (`Extensions.MyMethodFinalLog(source, ...)`) | compila | compila con `CS0618` | ✅ **Sí** |
| `typeof(Extensions)` o reflexión sobre el tipo | compila | compila con `CS0618` | ✅ **Sí** |

**Cómo detectarla:**

```
Extensions.MyMethodFinalLog|Logging.Extensions|typeof(Extensions)
```

#### 49 — Los mensajes que construye la librería se unifican en inglés (2.2.0)

`MoralesLarios.OOFP.Logging` es, casi en su totalidad, una capa que **transporta** el texto que le pasa el
consumidor; pero los dos miembros del punto 47 **construyen** ellos mismos el mensaje. Esos textos
mezclaban convenios y, además, el del caso fallido repetía la palabra `Error`. En la 2.2.0 se fija
**inglés** como idioma de todo mensaje que emita la librería y se corrige la duplicidad.

| Caso | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| Válido | `"{desc} done correctly."` | **sin cambios** | — |
| Fallido | `"Error when {desc} Error: {detalle}"` | `"Error when {desc}: {detalle}"` | 🔗 **No** · solo cambia el `string` |

**Por qué es silenciosa.** Las firmas no cambian y el `MlResult<T>` devuelto es el mismo: lo único que
cambia es el **texto escrito en el log**.

**Qué revisar en tu código:**

- Tests que comparen literalmente el mensaje registrado (`Should().Be("Error when ... Error: ...")`,
  *snapshots*, `Verify(l => l.Log(..., It.Is<string>(m => m == ...)))`).
- Reglas de agregación, alertas y búsquedas guardadas de tu plataforma de logs que filtren por el texto
  antiguo con el `Error` duplicado.
- Mensajes propios **en castellano** que tuvieras encadenados a los de la librería: la política de la
  2.2.0 es que la librería emite en inglés y el consumidor decide el idioma de **sus** textos.

**Cómo detectarla:**

```
done correctly|Error when
```

> 🧭 **Lo que NO rompe de los puntos 45 a 49.** Las llamadas con argumentos **posicionales** — la forma
> habitual — compilan sin tocar una sola línea. Los métodos de extensión invocados **como extensión**
> tampoco se enteran del renombrado de la clase. Y la paridad de la familia `LogMlResult*`
> (47 miembros genéricos más 17 por cada nivel de log) se mantiene **intacta**: los puntos 45 a 49 no
> añaden ni retiran ningún miembro de esa familia. Los puntos **51 y 52 sí añaden** miembros, pero
> siempre en **todos** los niveles a la vez, de modo que la simetría se conserva.

---

#### 50 — La familia `LogMlResult*` consulta `IsEnabled` antes de construir el mensaje (2.2.0)

Hasta la 2.1.1, los métodos de `LogMlResult*` que reciben un **constructor de mensaje**
(`validBuildMessage` / `failBuildMessage`) lo **invocaban siempre**, y solo después entregaban el
`string` resultante al `ILogger`, que lo descartaba si el nivel estaba apagado. Es decir: se pagaba el
coste completo de interpolar, serializar y concatenar un mensaje que nadie iba a leer.

En la 2.2.0 se consulta `logger.IsEnabled(logLevel)` **antes** de construir el mensaje.

| Caso | 2.1.1 | 2.2.0 | ¿Lo detecta el compilador? |
| --- | --- | --- | --- |
| Nivel **encendido** | se construye el mensaje y se registra | **sin cambios** | — |
| Nivel **apagado** | **se construye** el mensaje y se descarta | **no se construye** | 🔗 **No** · misma firma, mismo retorno |

**Por qué es silenciosa.** Las firmas no cambian y el `MlResult<T>` devuelto es exactamente el mismo.
Lo único que cambia es que, con el nivel apagado, **tu `Func` ya no se ejecuta**.

**Qué revisar en tu código:**

- Constructores de mensaje con **efectos colaterales**: si tu `validBuildMessage` o tu
  `failBuildMessage` incrementa un contador, escribe en una caché, envía una métrica, consume un
  enumerable perezoso o hace una llamada a base de datos, **ese efecto desaparece** cuando el nivel
  está apagado. Muévelo fuera del constructor de mensaje.
- Tests que cuenten invocaciones del constructor de mensaje con un `LogLevel` que el `ILogger`
  simulado no tiene habilitado: antes contaban 1 y ahora cuentan 0.
- `ILogger` simulados con `Mock<ILogger>` **sin configurar `IsEnabled`**: `Moq` devuelve `false` por
  defecto para `bool`, de modo que esos dobles **dejan de registrar**. Añade
  `mock.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);`.

**Cómo detectarla:**

```
validBuildMessage|failBuildMessage|Mock<ILogger|IsEnabled
```

> 🧭 La ganancia medida en el repositorio de la librería, con 20.000 iteraciones en `Release`, es de
> un **69,7 %** menos de asignaciones y un **93 %** menos de tiempo cuando el nivel está apagado.

---

#### 51 — Sobrecargas de `LogMlResult*` con `EventId` (2.2.0) ✅ SIN RUPTURAS

La 2.2.0 añade **35 firmas públicas nuevas** que aceptan un `EventId` y lo trasladan al `ILogger`,
para poder correlacionar trazas sin meter el identificador dentro del texto del mensaje.

```csharp
// 2.1.1 — el identificador tenía que ir en el texto
resultado.LogMlResultInformation(logger, validBuildMessage: x => $"[4242] Pedido {x.Id} localizado");

// 2.2.0 — el identificador viaja como EventId
resultado.LogMlResultInformation(logger, new EventId(4242, "PedidoLocalizado"),
                                 validBuildMessage: x => $"Pedido {x.Id} localizado");
```

**Por qué no rompe nada.** El `EventId` es un parámetro **obligatorio** colocado **antes** de los
constructores de mensaje opcionales, de modo que ninguna llamada existente puede resolverse por
error contra la sobrecarga nueva. Las firmas antiguas siguen vivas y sin cambios.

**Qué hacer:** nada. Es opcional. Si quieres aprovecharlo, revisa tus mensajes que llevan el
identificador incrustado en el texto:

```
LogMlResult.*\[\d+\]
```

---

#### 52 — Sobrecargas de `LogMlResult*` con `ILogger<TCategory>` (2.2.0) ✅ SIN RUPTURAS

La 2.2.0 añade **21 firmas públicas nuevas** que aceptan directamente el `ILogger<T>` que inyecta el
contenedor de dependencias, sin obligar a convertirlo a `ILogger`.

```csharp
// 2.1.1 — había que convertir
resultado.LogMlResultInformation((ILogger)_logger, validBuildMessage: x => $"{x}");

// 2.2.0 — el ILogger<PedidosService> se acepta tal cual
resultado.LogMlResultInformation(_logger, validBuildMessage: x => $"{x}");
```

**Por qué no rompe nada.** Para un `ILogger<T>` la sobrecarga nueva gana por **conversión de
identidad**, y para un `ILogger` a secas sigue ganando la antigua. En ambos casos el
comportamiento observable es idéntico: la nueva **delega** en la antigua.

**Qué hacer:** nada. Si quieres limpiar código, busca las conversiones que ya no hacen falta:

```
\(ILogger\)
```

> ⚠️ Las dos dimensiones **no se combinan entre sí**: no hay sobrecargas que acepten a la vez un
> `ILogger<TCategory>` y un `EventId`, ni se han añadido a las familias `IfValid`, `IfFail`,
> `IfFailWithValue`, `IfFailWithException` ni `IfFailWithoutException`. Solo a `LogMlResult` y a las
> seis familias de nivel (`LogMlResultTrace` … `LogMlResultCritical`).
>
> 🆕 **Matiz de la 2.3.0.** Eso sigue siendo cierto para `ILogger<TCategory>`, pero el eje de
> `EventId` **sí** se cruza con las plantillas de registro estructurado desde la 2.3.0: ver el
> **punto 55**. El cruce alcanza la familia genérica `LogMlResultTemplate` y las seis de nivel, y
> **no** las familias condicionales.

---

#### 53 — Familia `LogMlResult*Template` con plantilla de mensaje (2.3.0) ✅ SIN RUPTURAS

La 2.3.0 añade **35 firmas públicas nuevas** —5 del núcleo neutral y 5 por cada uno de los seis
niveles— que aceptan una **plantilla de mensaje** y sus argumentos, y los pasan **tal cual** a
`ILogger.Log`. La plantilla **no se interpola**: los marcadores llegan al registrador como **campos
estructurados**, que es lo que permite consultarlos y agruparlos en el destino (Seq, Application
Insights, Elastic…).

```csharp
// 2.2.0 — el dato se diluía dentro de la cadena
resultado.LogMlResultInformation(logger, $"hola a las {DateTime.Now}");
// el log recibe un único string: "hola a las 17/05/2024 9:30:00"

// 2.3.0 — el dato viaja como campo
resultado.LogMlResultInformationTemplate(logger, "hola a las {Hora}", DateTime.Now);
// el log recibe:  {OriginalFormat} = "hola a las {Hora}"   y   Hora = <DateTime>
```

Las firmas nuevas, por familia:

| Familia | Firmas | Forma |
| --- | --- | --- |
| Núcleo neutral | 5 | `LogMlResultTemplate[Async](… LogLevel logLevel, string messageTemplate, params object?[] args)` sobre `ILogger`, `MlResult<T>` y `Task<MlResult<T>>` |
| Por nivel (×6) | 30 | `LogMlResult<Nivel>Template[Async](… string messageTemplate, params object?[] args)`, con `<Nivel>` = `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical` |

**Por qué no rompe nada.** La familia nueva lleva el **sufijo `Template`**, así que es un conjunto de
nombres **disjunto** del existente: ninguna llamada escrita contra la 2.2.0 puede resolverse a una
sobrecarga nueva, ni siquiera las que pasan dos `string` sin nombre de parámetro (ver el punto 30).
Tampoco se ha modificado **ninguna** firma anterior, ni añadiéndole un parámetro opcional.

**Comportamiento.** Es coherente con el resto de la familia `LogMlResult*`:

- consulta `IsEnabled(logLevel)` **antes** de cualquier trabajo (punto 50);
- un `logger` nulo **no lanza**: no registra y devuelve el resultado de origen intacto;
- una `messageTemplate` nula **no registra nada** y devuelve el resultado de origen intacto;
- devuelve siempre el `MlResult<T>` de entrada sin tocarlo, igual que el resto de la familia.

**Qué hacer:** nada obligatorio. Si quieres aprovecharlo, busca las interpolaciones que diluyen datos
dentro del mensaje y pásalas a plantilla:

```
LogMlResult\w*\(\s*\w+\s*,\s*\$"
LogMlResult\w*\(\s*\$"
```

Y, para el **punto 54**, localiza los registros de la rama de fallo que todavía arman el texto a mano,
porque son los candidatos directos a pasar a plantilla con campos indexables:

```
LogMlResultIfFail\w*\(
LogMlResult(Trace|Debug|Information|Warning|Error|Critical)IfFail\w*\(
"ErrorMessage"|"ErrorCount"|"ExceptionType"
```

Y, para el **punto 55**, localiza las llamadas que usan el eje de `EventId` y las que usan plantilla,
porque son exactamente las que ahora pueden **unirse en una sola**:

```
LogMlResult\w*Template\w*\(
new EventId\(
```

Cruza las dos listas: donde una traza con `EventId` conviva con otra que arma el texto a mano para el
mismo hecho, puedes dejar **una sola** llamada añadiendo el sufijo `Template` y pasando el `EventId`
entre el `logger`/`logLevel` y la plantilla. La acción es **opcional**: nada de lo que ya está escrito
deja de compilar ni cambia de comportamiento.

Y, para el **punto 56**, busca si el consumidor ha intentado suplir por su cuenta los ámbitos que la
librería no tiene:

```
BeginScope
CorrelationId|correlationId
```

Si aparece un `using _logger.BeginScope(...)` envolviendo una tubería de `MlResult`, **déjalo tal
cual**: es la forma correcta hoy, y es exactamente lo que la épica 9 de la 2.4.0 se propone convertir
en API. Lo único que hay que hacer es **no** prometerle al consumidor un `WithLogScope` que todavía no
existe. Si en su lugar ve que el consumidor arrastra un `CorrelationId` a mano dentro del texto del
mensaje, puedes sugerirle que lo publique como **campo** con la plantilla del punto 53
(`"... {CorrelationId} ..."`), que es la mejora disponible sin esperar a la 2.4.0.

Cada coincidencia de las dos primeras búsquedas **sigue compilando y funcionando igual**; la acción es
opcional. Si decides migrarla, usa `MlResultLogErrorFields.DefaultTemplate` y
`MlResultLogErrorFields.From` para no inventar nombres de campo propios. La tercera búsqueda localiza
nombres de campo escritos como **literal**: sustitúyelos por las constantes
`MlResultLogErrorFields.ErrorMessageField`, `…ErrorCountField` y `…ExceptionTypeField`, porque son
contrato y un renombrado futuro no lo detectaría el compilador.

> ⚠️ **Dos matices de los marcadores.** Primero, se rellenan **por posición**, no por nombre: el
> primer argumento va al primer marcador aunque el nombre no cuadre. Segundo, el nombre del marcador
> **es la clave del campo** en el destino, así que conviene escribirlo en `PascalCase` (`{Hora}`,
> `{PedidoId}`) y mantenerlo estable: cambiarlo rompe las consultas guardadas del destino, y eso el
> compilador no lo detecta.
>
> ⚠️ La plantilla debe ser una **constante**, nunca una cadena interpolada: si escribes
> `$"hola a las {hora}"` vuelves a perder el campo y, además, el analizador `CA2254` lo señala.

---

#### 54 — Plantillas en la rama de fallo y contrato de campos de error (2.3.0) ✅ SIN RUPTURAS

El punto 53 solo cubría la rama **incondicional**. La 2.3.0 completa el trabajo y lleva la plantilla a
las cuatro familias que solo registran **cuando el resultado es fallido**, que son justamente las que
interesan para alertar:

| Familia | Registra cuando… |
| --- | --- |
| `LogMlResult*IfFailTemplate` | el resultado es fallido |
| `LogMlResult*IfFailWithValueTemplate` | es fallido **y** el detalle de error lleva un valor en los metadatos |
| `LogMlResult*IfFailWithExceptionTemplate` | es fallido **y** el detalle de error lleva una excepción |
| `LogMlResult*IfFailWithoutExceptionTemplate` | es fallido **y** el detalle de error **no** lleva excepción |

Son **172 firmas públicas nuevas**: 24 en el núcleo neutral (4 familias × 6 sobrecargas) y 24 por cada
uno de los seis ficheros de nivel. El recuento total de la clase pasa de **433** a **605**.

**De dónde salen los argumentos.** Hay **dos maneras, y conviven** (decisión `D-32` de la épica 08):

```csharp
// (1) los aporta el llamante, igual que en el punto 53
resultado.LogMlResultErrorIfFailTemplate(logger, "El pedido {PedidoId} ha fallado", pedidoId);

// (2) se derivan del MlErrorDetails con un constructor perezoso
resultado.LogMlResultErrorIfFailTemplate(logger,
                                         MlResultLogErrorFields.DefaultTemplate,
                                         MlResultLogErrorFields.From);
```

La segunda forma recibe un `Func<MlErrorDetails, object?[]>` que **no se invoca** si el nivel está
apagado, igual que los constructores de mensaje del punto 50.

**Los nombres de campo son contrato.** El tipo público nuevo `MlResultLogErrorFields` los fija:

| Constante | Valor | Qué publica |
| --- | --- | --- |
| `ErrorMessageField` | `ErrorMessage` | el resumen de errores del `MlErrorDetails` |
| `ErrorCountField` | `ErrorCount` | el número de errores |
| `ExceptionTypeField` | `ExceptionType` | el **nombre del tipo** de la excepción, o `NoExceptionValue` |
| `NoExceptionValue` | `(ninguna)` | valor explícito de ausencia, nunca `null` |
| `DefaultTemplate` | — | plantilla canónica que consume los tres campos **en el orden de `From`** |

> ⚠️ **Riesgo de ruptura silenciosa futura.** A partir de la 2.3.0 estos tres nombres son parte del
> contrato público: si una versión posterior los renombrase, las consultas guardadas del destino
> dejarían de encontrar nada y **el compilador no avisaría**. Por eso están expuestos como constantes:
> referencia `MlResultLogErrorFields.ErrorMessageField` en lugar de escribir `"ErrorMessage"` a mano.

**Por qué no rompe nada.** El sufijo `Template` mantiene el conjunto de nombres **disjunto** del
existente, así que ninguna llamada de la 2.2.0 puede resolverse a una sobrecarga nueva (ver el punto
30). No se ha modificado ninguna firma anterior.

> ⚠️ **Un matiz de resolución de sobrecargas.** Como cada familia tiene una sobrecarga con
> `params object?[]` y otra con `Func<MlErrorDetails, object?[]>`, pasar el literal `null` desnudo es
> **ambiguo** (`CS0121`), porque es convertible a las dos. Si necesitas pasar nulo, conviértelo:
> `(object?[])null!`. Es un error de compilación, no una ruptura silenciosa.

**Comportamiento.** Idéntico al del punto 53: `IsEnabled` antes de cualquier trabajo, `logger` nulo y
`messageTemplate` nula no registran ni lanzan, y el `MlResult<T>` de entrada se devuelve intacto. Un
resultado **válido** no registra nada en ninguna de las cuatro familias.

**Qué hacer:** nada obligatorio. Si ya registras fallos con mensaje fijo y quieres poder alertar por
campo, pásalos a plantilla.

---

#### 56 — Los ámbitos de registro siguen a cero, y es deliberado (2.3.0) ✅ SIN RUPTURAS

Este punto **no cambia ni una línea de código**. Está aquí porque la ausencia de una capacidad, cuando
no se explica, se interpreta como un descuido, y el consumidor acaba esperando que aparezca sola en la
siguiente versión o, peor, construyendo su diseño sobre la suposición de que llegará.

`ILogger.BeginScope` es el mecanismo del estándar para que **varias** trazas compartan contexto: todo
lo que se registre dentro del ámbito lleva adosadas sus propiedades, y el destino de trazas puede
agruparlas. En `MoralesLarios.OOFP.Logging` el recuento de `BeginScope` es **`0`**, y lo sigue siendo
después de las 242 firmas nuevas de los puntos 53, 54 y 55.

Lo que cambia es que ahora está **documentado con su motivo**. Una tubería como ésta emite tres trazas
que pertenecen a la misma operación lógica y que **nada** relaciona entre sí:

```csharp
origen.Bind(Validar)
      .LogMlResultInformationTemplate(_logger, "validado {PedidoId}", id)
      .Bind(Guardar)
      .LogMlResultInformationTemplate(_logger, "guardado {PedidoId}", id)
      .LogMlResultErrorIfFailTemplate(_logger, MlResultLogErrorFields.DefaultTemplate,
                                      MlResultLogErrorFields.From);
```

**Qué tienes que hacer como consumidor.** Nada obligatorio. Si necesitas correlacionar trazas hoy,
tienes dos vías, las dos disponibles ya:

1. **Abrir el ámbito tú mismo**, alrededor de la tubería. Es la forma correcta y no choca con nada de
   la librería:

   ```csharp
   using (_logger.BeginScope("{Operacion} de {PedidoId}", "AltaPedido", id))
   {
       var resultado = origen.Bind(Validar).Bind(Guardar)
                             .LogMlResultInformationTemplate(_logger, "procesado {PedidoId}", id);
   }
   ```

2. **Publicar la clave de correlación como campo** con la plantilla del punto 53, de modo que el
   destino pueda agrupar por consulta aunque no haya ámbito:
   `"... {CorrelationId} ..."` con el valor en los argumentos.

**Por qué no entró con los puntos 53 a 55.** Porque un ámbito es un `IDisposable` y una tubería de
métodos de extensión no tiene dónde ponerlo: cada eslabón recibe y devuelve un `MlResult<T>`, y no hay
ningún sitio natural donde cerrar el `using`. Eso abre tres preguntas que no se podían resolver de
paso, y que son las que la épica 9 tiene que contestar antes de publicar una sola firma:

- **qué pasa si el tramo envuelto lanza una excepción** —si el ámbito se cierra, si la excepción se
  convierte en `MlResult` fallido, o si se deja propagar;
- **cómo se compone con `Bind`** —si el ámbito cubre un tramo que el consumidor delimita o toda la
  tubería, y qué se registra al cerrarlo;
- **si el ámbito debe sobrevivir al `await`** —porque la variante asíncrona es precisamente donde un
  ámbito mal diseñado se rompe en silencio.

**Cómo detectarlo.** No hace falta: el compilador no interviene y el comportamiento no cambia. El
único síntoma es documental, y es el que este punto corrige.

**Dónde seguirlo.** En la épica 9 de la versión **2.4.0** y en su plan: seis historias (US-41 … US-46),
seis decisiones abiertas (`D-33` … `D-38`) y cuatro mediciones que hay que hacer **antes** de decidir.
Si esas mediciones demuestran que escribir el `using` a mano no estorba lo suficiente, la decisión
`D-33` se cerrará descartando la API y los ámbitos seguirán siendo responsabilidad del consumidor.

---

#### 55 — El eje `EventId` se cruza con las plantillas de registro estructurado (2.3.0) ✅ SIN RUPTURAS

Hasta la 2.3.0 el consumidor tenía que **elegir**. O usaba el eje de `EventId` del punto 51, y entonces
emitía un `string` plano; o usaba la plantilla del punto 53, y entonces renunciaba al `EventId` con el
que filtra y agrupa en su destino de trazas. La nota del punto 51 decía expresamente que los ejes «no
se combinan», y era verdad.

Este punto **añade el cruce** que faltaba, en un fichero propio,
`MlResultLogExtensions.TemplateEventId.cs`, con **35 firmas nuevas**:

| Familia | Firmas | Formas |
|---------|-------:|--------|
| `LogMlResultTemplate` (genérica, con `LogLevel` expreso) | 5 | sobre `ILogger`, sobre `MlResult<T>` y sobre `Task<MlResult<T>>`, en sus variantes síncrona y `Async` |
| `LogMlResultTraceTemplate` … `LogMlResultCriticalTemplate` | 30 | las mismas 5 formas por cada uno de los seis niveles |

Antes de la 2.3.0 había que partir la llamada en dos, y la traza con campos perdía el evento:

```csharp
// antes · o el EventId, o los campos
source.LogMlResultInformation(_logger, new EventId(1001, "PedidoCreado"), $"el pedido {id} se ha creado");
```

Ahora las dos cosas viajan juntas, y los marcadores siguen llegando como **campos indexables**:

```csharp
// ahora · el EventId y los campos, en la misma llamada
source.LogMlResultInformationTemplate(_logger, new EventId(1001, "PedidoCreado"),
                                      "el pedido {PedidoId} se ha creado", pedido.Id);
```

**Alcance exacto, y por qué es exacto.** El cruce llega hasta donde llegaba el eje de `EventId`, y no
más allá:

- ✅ familia genérica `LogMlResultTemplate` y las **seis** de nivel;
- ❌ **no** las familias condicionales de plantilla (`LogMlResultIfFailTemplate` y las otras tres del
  punto 54): el eje de `EventId` nunca llegó a las familias condicionales y seguir el patrón habría
  multiplicado la superficie pública sin demanda que lo justifique;
- ❌ **no** el cruce con `ILogger<TCategory>`: ese eje se mantiene aislado, igual que en la 2.2.0.

**Comportamiento.** Idéntico al del punto 53, parámetro a parámetro: `IsEnabled` se consulta **antes**
de cualquier trabajo, un `logger` nulo o una `messageTemplate` nula **no registran ni lanzan**, y el
`MlResult<T>` de entrada se devuelve **intacto**, válido o fallido. La única diferencia es que la
llamada subyacente pasa a ser `logger.Log(logLevel, eventId, messageTemplate, args)`.

**Por qué no rompe nada.** Las 35 firmas son **nuevas**, no se ha modificado ninguna anterior, y el
`EventId` va en **posición propia** entre el `logger`/`logLevel` y la plantilla, así que una llamada de
la 2.3.0 sin `EventId` **no puede** resolverse a la sobrecarga nueva. Se verificó compilando una llamada
de cada forma **sin nombres de parámetro** y con un `ILogger<TCategory>` **sin *casting***: el build en
Release da **0 errores**, sin un solo `CS0121`.

**Qué hacer:** nada. Si ya usabas `EventId` y habías renunciado a la plantilla —o al contrario—, ahora
puedes unir las dos en una sola llamada añadiendo el sufijo `Template`.

---

#### 57 — Los parámetros de plantilla se anotan con `[StringSyntax]` (2.3.0) ✅ SIN RUPTURAS

Las **241** apariciones del parámetro `messageTemplate` de las familias de plantilla (puntos 53, 54 y
55) quedan anotadas con `[StringSyntax("MessageTemplate")]`. Es una anotación **puramente
declarativa**: no cambia ni una firma, ni un tipo de retorno, ni un comportamiento.

| Fichero | Parámetros anotados |
| --- | ---: |
| `MlResultLogExtensions.Template.cs` | 6 |
| `MlResultLogExtensions.TemplateEventId.cs` | 36 |
| `MlResultLogExtensions.TemplateFail.cs` | 25 |
| Los seis ficheros de nivel (`Trace` … `Critical`) | 29 cada uno · **174** |
| **Total** | **241** |

**Qué gana el consumidor.** Su editor y sus analizadores pasan a reconocer la cadena como lo que es,
una plantilla de registro: coloreado de los marcadores, aviso si el recuento de marcadores no cuadra
con el de argumentos y aviso si la plantilla llega **interpolada** en lugar de constante. Es la pieza
que faltaba para que el matiz documentado en los puntos 53 y 54 —«la plantilla debe ser una
constante»— lo vigile la herramienta y no la memoria de quien escribe la llamada.

**La medición de `CA2254`.** Se compiló un proyecto **sonda** fuera de la solución, con
`EnableNETAnalyzers`, `AnalysisLevel=latest-all`, `AnalysisMode=All` y `TreatWarningsAsErrors=true`,
que ejercita las cinco formas de la familia: sobre un `ILogger`, sobre un resultado válido, en la rama
de fallo, con `EventId` y en variante `Async`. Resultado: **`CA2254 = 0`**, build con `EXIT = 0` y
**cero diagnósticos**.

Ese `0` **no es un falso negativo**: la sonda incluye, a propósito, una llamada **de control** directa
a `ILogger.Log`, y esa sí emite `CA1848` y `CA1873`, lo que demuestra que los analizadores estaban
activos. `CA2254` solo se dispara sobre las llamadas directas al registrador, y esas viven **dentro**
de la librería, tras un `#pragma warning disable CA2254` acotado al núcleo privado que recibe la
plantilla del consumidor en tiempo de ejecución.

> ⚠️ **Matiz que sí te puede aparecer.** Con analizadores estrictos, llamar a esta familia puede
> seguir emitiendo **`CA1848`** («usar los delegados de `LoggerMessage`»). Es **esperado** y se puede
> silenciar sin riesgo: satisfacerlo exigiría plantillas conocidas en tiempo de compilación, que es
> justo lo que esta API no puede imponer, porque la plantilla la aporta el consumidor en la llamada.
> No existe —ni está previsto— ningún `LoggerMessage.Define` en la librería.

**Qué hacer:** nada. Si ya tenías `CA2254` activado y silenciado a mano alrededor de estas llamadas,
puedes **retirar la supresión**: ya no hace falta.

---

## Fase 4 — Compilar y resolver iterativamente

1. ```powershell
   dotnet build --nologo
   ```
2. **Resuelve primero los errores `CS0246` (tipo no encontrado) y `CS0234`** (no existe en el namespace):
   casi siempre es un `using` olvidado de la Fase 2 o un tipo que cambió de nombre en la Fase 3.
3. Luego los **`CS0117`/`CS1061`** (el miembro no existe): renombrado de método o propiedad pendiente.
4. Repite build + corrección hasta `0 errores`. **Si tras 5 iteraciones siguen apareciendo errores nuevos,
   detente y pide instrucciones al usuario** en lugar de seguir probando a ciegas.
5. **No silencies errores** con `#pragma warning disable`, `dynamic`, casts forzados ni `!`. Si no sabes cómo
   migrar algo, déjalo marcado con `// TODO: OOFP 2.0 — revisar` y repórtalo.

### 4.1 Avisos de los puentes `[Obsolete]`

La 2.0.0 conserva **187 puentes `[Obsolete]`** para que el código 1.x compile con avisos. Si el build termina
con `0 errores` pero con **`CS0618`** (*"'X' is obsolete"*), significa que **aún quedan llamadas al nombre antiguo**.

```powershell
dotnet build --nologo /p:WarningLevel=4
```

**Resuélvelos todos**: los puentes están marcados *"Will be removed in 3.0.0"* y
`[EditorBrowsable(Never)]`, así que no aparecen en IntelliSense. Dejarlos es deuda técnica que romperá en la 3.0.0.

> 💡 Si el usuario quiere una migración **gradual**, puede quedarse aquí: con los paquetes nuevos y los
> namespaces corregidos, los puentes permiten compilar. Pregúntale si prefiere
> **(a)** migración completa a nombres 2.0.0 o **(b)** mínima (solo paquetes + namespaces, aceptando `CS0618`).
> **Recomienda (a).**

---

## Fase 5 — Verificación

1. **Compilación limpia**:
   ```powershell
   dotnet build --nologo
   ```
   Objetivo: `0 Error(s)` y `0` avisos `CS0618`/`CS0619`.
2. **Tests**:
   ```powershell
   dotnet test --nologo
   ```
   Compara con la línea base de la Fase 0. Si algún test que pasaba ahora falla, **investígalo**: lo más
   probable es un cambio semántico de la sección 3.2 (fechas con cultura invariante, factorías, nulabilidad)
   o un cambio de comportamiento de la sección 3.3 (URL, cabeceras de paginación, `Location`, `type` de
   `ProblemDetails`, `TotalCount` y orden de las consultas paginadas).
3. **Barrido final de nombres antiguos**. Busca en todo el código que no quede ningún nombre 1.x:
   ```
   MoralesLarios.OOFP.WebApi|MoralesLarios.OOFP.WebControllers|MoralesLarios.OOFP.WebServices
   MoralesLarios.OOFP.HttpClients|MoralesLarios.OOFP.EFCore|MoralesLarios.OOFP.Internals
   MoralesLarios.OOFP.Utilities|MoralesLarios.OOFP.Extensions.Loggers|MoralesLarios.OOFP.ValueObjects.IO
   NullToFailed|EmptyToFailed|BoolToResult|SecureValidValue|FusionFailErros|FusionErrosIfExists
   MlErrorsDetails|MlResultActions|MlProblemsDetails|MlActionResults|MlResultWebExtensions
   IsValid|IsFail|ErrorsDetails|Projection|ProjectionSplit|ProjectionWhile
   IGenServiceFp|GenServiceFp|SimpleMlControllerBase|SimpleMlComplexPkControllerBase
   SimpleMlCacheControllerBase|PkParameterAttribute|PerControllerOutputCachePolicy
   IGenClientFp|GenClientFp|GenComplexClientFp|IHttpClientFactoryManager|HttpClientFactoryManager
   CallRequestParamsInfo|CallRequestPaginationParamsInfo|GetHttpClientFactoryKey
   PaginationInfo|PaginationResultInfo|IWrapperIO|WrapperIO|IMlConfigManager|MlConfigManager
   IEFRepo|EFRepo|IEFPaginatorFp|MlFile|MlDirectory|ExistsFile|ExistDirectory
   MlValidableFp|FluentValidationsValidator|IntNotNegative|DecimalNotNegative|DoubleNotNegative
   RangeEnumValueObject|StringBetweenLength|LogMlResultFinal|ChangeReturnResultAlwais
   ```
   ⚠️ Algunos (`IsValid`, `Value`, `Name`, `Key`, `Empty`, `OrderBy`, `Combine`, `Do`) son palabras muy comunes:
   revisa cada coincidencia a mano y **descarta las que no provengan de la librería**.
4. **Revisión de los cambios de comportamiento (sección 3.3)**. Busca y revisa a mano:
   ```
   PackageReference Include="Microsoft.AspNetCore.Mvc.Core"
   SetHeaderPageNumber|SetHeaderPageSize|AddPageNumberHeader|AddPageSizeHeader
   DefaultRequestHeaders
   netalpunto|puntonetalpunto
   TotalCount|ToResponseErrorsDescription|ReadErrorDescription
   ```
   Y, si actualizas a la **2.1.1** o posterior, busca además (punto 38):
   ```
   DEFAULT_ERROR_MESSAGE|DEFAULT_EX_ERROR_MESSAGE|NOT_FOUND_KEY|PARAM_NAME_KEY
   FAILED_INDEXES_KEY|EXPECTED_KEY|EX_DESC_KEY|VALUE_KEY
   Ex(s) details exceptions|More info in Ex Details|Not right value
   using static MoralesLarios.OOFP.Helpers.Constants
   new ErrorMessage(|Types.Errors.ErrorMessage|ErrorMessage(
   ```
   Y, para el punto 40 (metadatos de `MlErrorDetails`), busca además:
   ```
   "Ex1"|"Ex2"|'Ex1'|'Ex2'|StartsWith("Ex")|StartsWith('Ex')
   Metadata\[|\.Details\[|Metadata\.ContainsKey|Metadata\.TryGetValue|Metadata\.Add\(
   AddMetadataRange|AddMetadataOrThrow|AddValueMetadata|AddValueDetailIfFail
   \.Merge\(|GetDetail<|GetMetadataValue<|GetException
   ```
   Y, para el punto 44 (`LogMlResultIfFailWithExceptionAsync` sobre una `Task`), busca además:
   ```
   LogMlResultIfFailWithExceptionAsync
   ```
   ⚠️ Revisa **solo** las coincidencias cuyo receptor sea una expresión de tipo
   `Task<MlResult<T>>` (sin `await` delante) con un constructor de mensaje de **dos** parámetros:
   esa sobrecarga **no registraba nada** antes de la 2.2.0 y ahora sí registra.
   Y, para los puntos 45 a 49 (nomenclatura y mensajes de `MoralesLarios.OOFP.Logging`), busca además:
   ```
   validBuilMessage:|faildMessage:
   MyMethodFinalLog
   Extensions.MyMethodFinalLog|Logging.Extensions|typeof(Extensions)
   done correctly|Error when
   ```
   ⚠️ Los dos primeros solo rompen la compilación si los pasabas como **argumento con nombre**
   (con los dos puntos detrás): un argumento **posicional** compila sin tocar nada. El último barrido
   localiza los tests, los *snapshots* y los filtros de log que fijaban el **texto** de los mensajes que
   construye la librería, que ahora están **en inglés** y sin el `Error` duplicado.
   Y, para el punto 50 (`IsEnabled` antes de construir el mensaje), busca además:
   ```
   validBuildMessage|failBuildMessage|Mock<ILogger|IsEnabled
   ```
   ⚠️ Revisa **cada** constructor de mensaje buscando **efectos colaterales** (contadores,
   métricas, escrituras en caché, consumo de enumerables perezosos): con el nivel de log apagado
   **ya no se ejecutan**. Y revisa los `Mock<ILogger>` que no configuran `IsEnabled`: `Moq` devuelve
   `false` por defecto y esos dobles dejan de registrar.
   Y, para los puntos 51 y 52 (sobrecargas con `EventId` y con `ILogger<TCategory>`), busca además
   — **solo para limpiar**, porque ninguno de los dos rompe nada:
   ```
   LogMlResult.*\[\d+\]
   \(ILogger\)
   ```
   Y, para el punto 53 (familia `LogMlResult*Template`), busca además — **solo para limpiar**,
   porque es aditivo:
   ```
   LogMlResult\w*\(\s*\w+\s*,\s*\$"
   LogMlResult\w*\(\s*\$"
   ```
   Cada coincidencia es un mensaje **interpolado**: el dato acaba diluido dentro de la cadena y el
   destino no lo puede consultar como campo. Pásalo a `LogMlResult*Template` con la plantilla como
   **constante** y los datos en `args`. ⚠️ Y comprueba que la plantilla resultante **no** sea una
   cadena interpolada: el analizador `CA2254` lo señala, pero solo si está activado.
   Y, para el punto 57 (anotación `[StringSyntax]` de las plantillas), busca además — **solo para
   limpiar**, porque es aditivo:
   ```
   pragma warning disable CA2254|pragma warning restore CA2254
   NoWarn.*CA2254|dotnet_diagnostic\.CA2254
   ```
   Cada coincidencia es una supresión de `CA2254` que el consumidor escribió para poder usar estas
   llamadas. Con la anotación de la 2.3.0 **ya no es necesaria**: retírala y vuelve a compilar; si el
   aviso reaparece, es que la plantilla está **interpolada** y hay que pasarla a constante.
   ⚠️ **No** retires las supresiones de `CA1848`: ese aviso sigue siendo esperado al llamar a esta
   familia, y silenciarlo es correcto.
   Y, para el punto 43 (`MlResult._` → `MlResult.Discard`), busca además:
   ```
   MlResult\s*\.\s*_\b
   ```
   ⚠️ Busca **solo** esa forma cualificada. `_` a secas es el patrón de descarte de C# y produciría
   cientos de falsos positivos (`.Bind(_ => ...)`, `out _`): esos **no** se tocan.
   - Si aparece `PackageReference … Microsoft.AspNetCore.Mvc.Core`, **quítalo** y pon
     `<FrameworkReference Include="Microsoft.AspNetCore.App" />` (o cambia a `Microsoft.NET.Sdk.Web`).
   - Si se añaden cabeceras de paginación sobre `HttpClient`, migra a las sobrecargas de
     `HttpRequestMessage`.
   - Si hay tests o *snapshots* que fijen `Location`, `type`, URL compuestas o `TotalCount`, actualiza el
     valor esperado al comportamiento correcto de la 2.0.0.
   - **2.1.1**: sustituye los nombres en `MAYÚSCULAS` de `Constants` por sus gemelos `PascalCase`, cambia
     `Constants.DEFAULT_EX_ERROR_MESSAGE(ex)` por `EnsureFpMessages.DefaultExceptionError(ex)` y actualiza
     las aserciones que comparen literalmente los mensajes por defecto o el `ToString()` de un
     `MlResult<T>` con valor `null` (ahora `"<null>"`). Si declarabas
     `global using static MoralesLarios.OOFP.Helpers.Constants;`, sigue funcionando, pero conviene
     cualificar los usos con `Constants.` para que `CS0618` sea legible.
   - **2.1.1**: si usabas el tipo `ErrorMessage`, cámbialo por `MlError` (punto 39). Busca con límite de
     palabra y **descarta las coincidencias precedidas por un punto** (`.ErrorMessage`): esas son la
     propiedad homónima de `ValidationResult` o de `ValidationFailure` y no hay que tocarlas.
   - ⚠️ **2.1.1 (punto 40, el más delicado)**: por **cada** coincidencia del barrido de metadatos,
     verifica una a una estas cuatro cosas, porque **el compilador no avisa de ninguna**:
     1. que **ningún** mutador (`Add*`, `With*`) se llame **descartando el retorno** —esa línea pasa a
        no tener efecto—;
     2. que no se lean claves `"Ex1"` ni se discriminen excepciones con `StartsWith("Ex")` → usa
        `GetExceptions()` / `TryGetException`;
     3. que no haya un `catch (ArgumentException)` alrededor de un `Merge` esperando claves duplicadas;
     4. que no se dependa de la **identidad por referencia** de un `MlErrorDetails` (ahora `Equals` y
        `GetHashCode` son estructurales) ni del **diccionario compartido** entre instancias.
     Sustituye las claves escritas a mano por `MlErrorDetails.MetadataKeys.*` y la lectura cruda del
     diccionario por `TryGetMetadata<T>` / `MetadataView`.
   - **2.1.1**: si usabas `MlResult._`, cámbialo por `MlResult.Discard` (punto 43). Es un reemplazo
     mecánico: el valor devuelto es el mismo. **No** toques los `_` que son patrón de descarte del
     lenguaje. `MlResult.Empty()` y `MlResult.EmptyAsync()` **siguen siendo válidos** en la 2.1.1.
   - **2.2.0**: si llamabas a `LogMlResultIfFailWithExceptionAsync` sobre una `Task<MlResult<T>>`
     (punto 44), **no hay nada que cambiar en el código**, pero sí hay que revisar los tests que
     afirmaban **ausencia** de traza y los umbrales de alerta basados en recuento de errores: esa
     sobrecarga no registraba nada y ahora registra la excepción.
   - **2.2.0**: si pasabas `validBuilMessage:` o `faildMessage:` como **argumento con nombre**
     (puntos 45 y 46), renombra los dos a `validBuildMessage:` y `failBuildMessage:`. No hay puente
     posible: C# no admite alias de parámetro, pero el compilador señala cada caso con `CS1739`. **No**
     toques `failMessage`, el parámetro `string` del mensaje fijo.
   - **2.2.0**: si usabas `MyMethodFinalLog[Async]` o la clase `Extensions` de
     `MoralesLarios.OOFP.Logging` en forma estática (puntos 47 y 48), cámbialos por
     `LogActionResult[Async]` y `MlResultLogActionExtensions`. Los puentes `[Obsolete]` siguen
     funcionando en la 2.2.0, pero **desaparecen en la 3.0.0**.
   - **2.2.0**: si tus tests, tus *snapshots* o tus alertas comparaban el **texto** de los mensajes que
     construye la librería (punto 49), actualiza el valor esperado: ahora están **en inglés** y el del
     caso fallido ya no repite la palabra `Error`.
5. **Diff**:
   ```powershell
   git diff --stat
   ```
   Revisa que no haya ficheros tocados sin motivo.

---

## Fase 6 — Informe final (OBLIGATORIO)

Entrega al usuario, **en español**, un informe con:

1. **Paquetes**: tabla de antiguo → nuevo → versión instalada, por proyecto.
2. **Ficheros modificados**: número total y desglose por proyecto.
3. **Cambios de nomenclatura aplicados**: número total y los 10 más frecuentes.
4. **Cambios semánticos revisados** (sección 3.2): qué se encontró y qué se hizo en cada caso.
5. **Cambios de comportamiento revisados** (sección 3.3): tabla con los puntos 17, 27, 28, 29, 31, 32, 34, 35,
   36, 37, —si migras a la 2.1.1— 38, 39, 40, 41, 42 y 43 y —si migras a la 2.2.0— 44, 45, 46, 47, 48 y 49, indicando, para cada uno, si **afecta** al proyecto, qué se
   cambió y **qué debe validar el usuario** (en especial: URL emitidas, cabecera `Location`, `type` de
   `ProblemDetails`, resultados paginados y el texto de los mensajes de error por defecto). Para el
   punto **40**, enumera **explícitamente** las cuatro verificaciones silenciosas del punto 4 de la
   Fase 5 y su resultado: mutadores con el retorno descartado, claves `Ex` leídas a mano, `catch`
   muertos alrededor de `Merge` e identidad por referencia de `MlErrorDetails`. Para el punto
   **44**, indica expresamente cuántas llamadas a `LogMlResultIfFailWithExceptionAsync` sobre una
   `Task<MlResult<T>>` se han encontrado y qué tests que afirmaban ausencia de traza se han
   tenido que invertir. Para el punto **50**, indica cuántos constructores de mensaje con **efectos
   colaterales** se han encontrado y qué se ha hecho con cada uno, cuántos `Mock<ILogger>` han
   necesitado `Setup(l => l.IsEnabled(...)).Returns(true)` y cuántos tests que contaban
   invocaciones del constructor con el nivel apagado se han tenido que ajustar. Para los puntos
   **51 y 52**, basta con hacer constar que son **aditivos** y, si se han aprovechado, cuántas
   llamadas se han migrado a `EventId` y cuántas conversiones `(ILogger)` se han podido eliminar.
   Para el punto **53**, haz constar que es **aditivo** y, si se ha aprovechado, cuántos mensajes
   interpolados se han pasado a `LogMlResult*Template` y cuántos marcadores quedan escritos dentro
   de una cadena interpolada (que no publican campo y disparan `CA2254`). Para el punto **54**,
   indica cuántos registros de la **rama de fallo** se han pasado a plantilla, cuántos usan la forma
   con `Func<MlErrorDetails, object?[]>` frente a la de `params`, y cuántos literales
   `"ErrorMessage"`, `"ErrorCount"` o `"ExceptionType"` se han sustituido por las constantes de
   `MlResultLogErrorFields`. Para el punto **55**, haz constar que es **aditivo** y, si se ha
   aprovechado, cuántas parejas de llamadas (una con `EventId`, otra con el texto armado a mano) se
   han unificado en una sola llamada `LogMlResult*Template` con `EventId`, y deja escrito que el
   cruce con `ILogger<TCategory>` **sigue sin existir**, para que nadie lo busque. Para el punto
   **56**, deja constancia explícita de que la librería **no** tiene ámbitos de registro, de cuántos
   `using logger.BeginScope(...)` escritos a mano se han encontrado en el proyecto —que **no** hay que
   tocar— y de cuántas claves de correlación viajaban dentro del texto del mensaje y se han podido
   publicar como campo aprovechando el punto 53. Para el punto **57**, indica cuántas supresiones de
   `CA2254` se han podido **retirar** del proyecto y confirma que el build sigue sin emitir ese aviso;
   si queda algún `CA1848`, dilo expresamente y aclara que es **esperado** y que no hay nada que
   migrar.
6. **Resultado del build**: errores y avisos, antes y después.
7. **Resultado de los tests**: pasados/fallados, antes y después.
8. **Pendientes / intervención manual**: todo `// TODO: OOFP 2.0 — revisar`, cualquier `CS0618` que quede,
   y cualquier decisión que requiera al usuario.
9. **Siguientes pasos sugeridos**: confirmar el commit, revisar el diff, ejecutar la aplicación.

No afirmes que la migración está completa si queda **cualquier** error de compilación, test roto nuevo,
`CS0618` sin resolver o `TODO` pendiente. En ese caso, dilo explícitamente y enumera lo que falta.

---

## Recordatorios finales

- ✅ Pregunta antes de reemplazar identificadores ambiguos (`Name`, `Key`, `Value`, `Empty`, `Combine`, `Do`, `OrderBy`).
- ✅ Dos pasadas con tokens siempre que haya solapamiento entre el lado antiguo y el nuevo.
- ✅ Más largo primero, con límites de palabra.
- ✅ Compila entre tandas; no acumules 500 cambios sin verificar.
- ❌ No reformatees, no reordenes `using`, no "limpies" código.
- ❌ No toques `Program.cs`, `appsettings*.json` ni ficheros de configuración salvo que contengan nombres de la librería.
- ❌ No instales *prereleases* sin permiso.
- ❌ No des la migración por buena sin compilar.
- ⚠️ **Avisa SIEMPRE al usuario de los cambios de comportamiento de la sección 3.3**, aunque el build y los
  tests queden en verde: cambian la respuesta HTTP pública de su API y el contenido de sus listados paginados.

> 📚 Referencia completa de los 888 cambios: `Cambios Realizados/Renombrados-v2.0.0.md` en el repositorio
> de MoralesLarios.OOFP. Convenciones de nomenclatura de la 2.0.0: `CONTRIBUTING.md`.

---

## Apéndice — Versiones posteriores a la 2.0.0

Este prompt cubre el salto **1.x → 2.0.0** y, además, acumula las rupturas introducidas en cada versión
posterior. Consulta la sección 3.3 para el detalle de cada punto.

| Versión | Puntos de la 3.3 | Resumen de la ruptura |
| --- | --- | --- |
| 2.0.0 | 17, 27, 28, 29, 31, 32, 34, 35, 36, 37 | Renombrado masivo de la API + correcciones de comportamiento |
| 2.1.1 | 38, 39, 40, 41, 42, 43 | Normalización de `Constants`, mensajes corregidos, `EnsureFpMessages` público, `ErrorMessage` retirado, **saneamiento del contenedor de metadatos de `MlErrorDetails`**, `TryGetValue` / `TryGetErrors` (aditivo) y **troceado de la clase `Extensions`**, además de la retirada de `MlResult._` en favor de `Discard` |
| 2.3.0 | 53, 54, 55, 56, 57 | **242 firmas nuevas aditivas** y la **anotación `[StringSyntax]` de los 241 parámetros de plantilla** (punto 57, con `CA2254 = 0` medido en una sonda con analizadores estrictos): la familia `LogMlResult*Template`, que acepta una **plantilla de mensaje** y sus argumentos y los entrega a `ILogger.Log` **sin interpolar**, de modo que los marcadores viajan como **campos estructurados** en lugar de diluirse en la cadena (35 firmas), y su extensión a la **rama de fallo** en las cuatro familias `IfFail*` (172 firmas), con el tipo nuevo `MlResultLogErrorFields`, que **fija como contrato** los nombres de campo `ErrorMessage`, `ErrorCount` y `ExceptionType`. ✅ **Sin rupturas**: el sufijo `Template` deja el conjunto de nombres disjunto del anterior y no se ha modificado ninguna firma previa. Además, el **cruce del eje `EventId` con la plantilla** (35 firmas), que deja sin efecto la nota de la 2.2.0 según la cual los ejes no se combinaban: ahora `EventId` × plantilla **sí**, `ILogger<TCategory>` **no** |
| 2.2.0 | 44, 45, 46, 47, 48, 49, 50, 51, 52 | Corrección de la **llamada cruzada** de `LogMlResultIfFailWithExceptionAsync` sobre `Task<MlResult<T>>` (esa sobrecarga **no registraba nada**), **normalización de la nomenclatura pública** de `MoralesLarios.OOFP.Logging` (`validBuilMessage` → `validBuildMessage`, `faildMessage` → `failBuildMessage`, `MyMethodFinalLog[Async]` → `LogActionResult[Async]`, clase `Extensions` → `MlResultLogActionExtensions`) y **unificación en inglés** del idioma de los mensajes que emite la librería. Además, **evaluación perezosa** del constructor de mensaje (`IsEnabled` antes de construir) y **56 firmas nuevas aditivas**: 35 con `EventId` y 21 con `ILogger<TCategory>`. ⚠️ **Tres rupturas silenciosas (44, 49 y 50)** |

> 🧭 Si vienes de una 2.0.x y solo quieres subir a la 2.1.1, **sáltate las fases 1 a 3** (los namespaces y la
> nomenclatura no cambian) y aplica únicamente los puntos **38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56 y 57** de la sección 3.3 más el barrido
> específico del punto 4 de la Fase 5.
>
> 🧭 El salto **2.2.0 → 2.3.0** no exige nada: sus cinco puntos, el **53**, el **54**, el **55**, el
> **56** y el **57**, son aditivos. Léelos solo si quieres aprovechar el registro estructurado; el
> **54** interesa especialmente si montas alertas sobre la rama de fallo, porque fija los nombres de
> campo como contrato, y el **55** si ya filtrabas por `EventId` y habías tenido que renunciar a los
> campos. El **56** no pide ninguna acción: solo deja escrito que los **ámbitos de registro** siguen
> a cero a propósito y cómo correlacionar trazas mientras tanto. Y el **57** solo te interesa si
> tienes analizadores estrictos: puedes **retirar** las supresiones de `CA2254` que hubieras escrito
> alrededor de estas llamadas, aunque los avisos `CA1848` seguirán ahí y son esperados.
>
> ⚠️ Aun así, **no te saltes el punto 40**: es el único de la 2.1.1 cuyos cambios son en su mayoría
> invisibles para el compilador. Un salto 2.0.x → 2.1.1 que compile a la primera y pase los tests puede
> seguir teniendo líneas que han dejado de tener efecto (mutadores con el retorno descartado).
