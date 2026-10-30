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
| 40 | Contenedor de metadatos de `MlErrorDetails` (**2.1.1**) | 7 defectos funcionales: claves de excepción mal discriminadas, `Merge` que lanza, `AddMetadataRange` NO-OP, lectores que mutan, diccionario compartido | claves `Ex`/`Ex2`/`Ex3` densas, `Merge` que sufija, lectores no destructivos, metadatos **inmutables**, igualdad **estructural** | ⚠️ **Contrato · 9 de 12 cambios son SILENCIOSOS** |
| 41 | `TryGetValue` / `TryGetErrors` y los convenios `Try*` / `Async` (**2.1.1**) | no existían; los dos convenios no estaban documentados | dos miembros nuevos con semántica BCL (`bool` + `out`) y los convenios documentados en el `README.md` del núcleo | ✅ **Ninguna** · aditivo y documental |

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
   36, 37 y —si migras a la 2.1.1— 38, 39, 40 y 41, indicando, para cada uno, si **afecta** al proyecto, qué se
   cambió y **qué debe validar el usuario** (en especial: URL emitidas, cabecera `Location`, `type` de
   `ProblemDetails`, resultados paginados y el texto de los mensajes de error por defecto). Para el
   punto **40**, enumera **explícitamente** las cuatro verificaciones silenciosas del punto 4 de la
   Fase 5 y su resultado: mutadores con el retorno descartado, claves `Ex` leídas a mano, `catch`
   muertos alrededor de `Merge` e identidad por referencia de `MlErrorDetails`.
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
| 2.1.1 | 38, 39, 40, 41 | Normalización de `Constants`, mensajes corregidos, `EnsureFpMessages` público, `ErrorMessage` retirado, **saneamiento del contenedor de metadatos de `MlErrorDetails`** y `TryGetValue` / `TryGetErrors` (aditivo) |

> 🧭 Si vienes de una 2.0.x y solo quieres subir a la 2.1.1, **sáltate las fases 1 a 3** (los namespaces y la
> nomenclatura no cambian) y aplica únicamente los puntos **38, 39, 40 y 41** de la sección 3.3 más el barrido
> específico del punto 4 de la Fase 5.
>
> ⚠️ Aun así, **no te saltes el punto 40**: es el único de la 2.1.1 cuyos cambios son en su mayoría
> invisibles para el compilador. Un salto 2.0.x → 2.1.1 que compile a la primera y pase los tests puede
> seguir teniendo líneas que han dejado de tener efecto (mutadores con el retorno descartado).
