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
   probable es un cambio semántico de la sección 3.2 (fechas con cultura invariante, factorías, nulabilidad).
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
4. **Diff**:
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
5. **Resultado del build**: errores y avisos, antes y después.
6. **Resultado de los tests**: pasados/fallados, antes y después.
7. **Pendientes / intervención manual**: todo `// TODO: OOFP 2.0 — revisar`, cualquier `CS0618` que quede,
   y cualquier decisión que requiera al usuario.
8. **Siguientes pasos sugeridos**: confirmar el commit, revisar el diff, ejecutar la aplicación.

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

> 📚 Referencia completa de los 888 cambios: `Cambios Realizados/Renombrados-v2.0.0.md` en el repositorio
> de MoralesLarios.OOFP. Convenciones de nomenclatura de la 2.0.0: `CONTRIBUTING.md`.
