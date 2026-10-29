# Plan de renombrados v2.0.0 — aplicación completa de `Consejos-Nomenclatura.md`

> 🛑 **Este documento no cambia ni una línea de código.** Es el plan para revisar y aprobar
> antes de ejecutar. Las decisiones pendientes están marcadas con **❓D-n** y resumidas en la
> [§ 5](#5-decisiones-pendientes).

**Fecha:** 2026-10-27 · **Estado:** pendiente de aprobación
**Documento fuente (único):** [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — Niveles 0 a 10,
guía de estilo, estrategia de migración y checklist.
**Entregable final:** `Cambios Realizados/Renombrados-v2.0.0.md` con la tabla *versión 1.x → versión 2.0.0*
de **todos** los cambios aplicados (ver [§ 4](#4-entregable-final-cambios-realizados)).

---

## Índice

1. [Inventario medido](#1-inventario-medido)
2. [Estrategia y herramientas](#2-estrategia-y-herramientas)
3. [Fases de ejecución](#3-fases-de-ejecución) (Fase 0 a Fase 13)
4. [Entregable final: `Cambios Realizados`](#4-entregable-final-cambios-realizados)
5. [Decisiones pendientes](#5-decisiones-pendientes)
6. [Riesgos](#6-riesgos)

---

## 1. Inventario medido

Todas las cifras de este plan salen de `scripts/inventario-nomenclatura.ps1` (157 términos), no de
estimaciones. La foto completa del estado actual está guardada en
`scripts/inventario-nomenclatura.antes.txt` y servirá de columna «antes» en el entregable final.

**Columnas:** `Cód.` = ocurrencias en proyectos de código · `Tests` = en proyectos de test ·
`Docs` = en `.md` de la solución (README y `__Doc/`, **excluyendo** `Temas Pendientes/`).

### 1.1 Hallazgos que cambian la propuesta original

Al cruzar el documento con el código aparecen diez cosas que el documento no podía saber y que
el plan resuelve explícitamente:

| # | Hallazgo | Consecuencia en el plan |
|---|---|---|
| H1 | **Ya existe un proyecto `MoralesLarios.OOFP.Shared`** (1 fichero: `Web/WebErrorDetailsKeys.cs`), referenciado por `WebApi` y `WebServices`. | `Internals → Shared` **no es un renombrado sino una fusión**: se mueven los 2 ficheros de `Internals/Info/` a `Shared/Pagination/` y se elimina `Internals`. |
| H2 | El documento propone **dos nombres sucesivos** para el mismo símbolo en niveles distintos: `FromStringLenght` (N1 → `FromStringLength`, N3 → `FromLength`), `ExistDirectory` (N1 → `ExistsDirectory`, N3 → `ExistingDirectoryPath`), `FusionFailErros` (N1 → `FusionFailErrors`, N2 → `MergeFailureErrors`). | Se aplica **directamente el nombre final**. Renombrar dos veces duplica el trabajo y deja un nombre intermedio que nunca se publica. |
| H3 | `Types/MlResultBucles.cs` (**trabajo en curso del autor**, fuera de límites) contiene `MlResultBucles`, `FusionFailErros` (29 de 39) y `Projection` (19+10). | Todos esos renombrados quedan **🔒 bloqueados** hasta tener permiso (❓D-1). |
| H4 | `IsValid` existe en `MlResult<T>` **y** como método de validación en `ValueObjects`; `AllAsync` existe en `WebServices` **y** en `EnsureFp.Aggregation` (agregado booleano); `Combine` cuenta `Path.Combine`; `limit` cuenta `Limit` privados de `Age`/`Id`/`IdLite`. | Buscar y reemplazar **renombraría lo que no debe**. Los símbolos se renombran con un renombrador **semántico** (Roslyn), igual que el `F2` que exige el propio documento. |
| H5 | `BuildErrorMessage` (N4, «Validation») vive en realidad en `ValueObjects` y `ValueObjects.IO` (140 + 3, 43 ficheros). | Se ejecuta en la fase de `ValueObjects`, no en la de `Validation`. |
| H6 | `MlFileHelper`, `MlFileUtils`, `ReadAllTextFp`, `IValidatorFp`, `GenServiceDuplexFp`, `GenClientDuplexFp`, `MaxLenght`, `TryChangeReturnResultAlwais` y los valores `OrderBy.Asc/Desc` → **0 ocurrencias**. `TryMapIAsyncf` → **0** (ya corregido). | Se marcan como «no aplica / ya hecho». No generan trabajo. |
| H7 | `MlResultWebExtensions` (obsoleta) y `MlResultWebExtensionsPlus` tienen **el mismo nombre destino** (`MlResultActionResultExtensions`). | Primero se **elimina** la clase ya `[Obsolete]` (era obsoleta en 1.x: eliminarla en una mayor es lo correcto) y después se renombra la `…Plus`. |
| H8 | `MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit` **no está en `MoralesLarios.OOFP.sln`**, y tiene cambios tuyos sin commitear. | Se fusiona con `ValueObjects.IO.Test.Unit` **después** de que hagas commit de esos cambios. |
| H9 | No existe `namespace MoralesLarios.FOOP` en el código: la discrepancia FOOP/OOFP es **solo de carpeta y `.sln`**. | La Fase 1 del núcleo es barata en código: rutas, `.sln` y `ProjectReference`. |
| H10 | `..\docs\` (raíz del repo, **fuera del workspace `src`**) existe y está vacía. | Mover `__Doc` allí saca la documentación del workspace actual (❓D-9). |

### 1.2 Recuento por nivel

Las tablas detalladas, fila a fila, están en la [§ 3](#3-fases-de-ejecución) dentro de cada fase.

| Nivel | Ámbito | Cambios | Ocurrencias código + tests | Docs a actualizar |
|---|---|---|---|---|
| 0 | Solución, proyectos, carpetas | 2 `.sln` borrados · 25 proyectos · 8 carpetas · 2 ficheros vacíos | `ProjectReference`, `.sln`, namespaces | todos los README |
| 1 | Erratas | 19 | ≈ 230 (sin `MlErrorsDetails`) | ≈ 200 |
| 2 | Núcleo `MlResult` | 30 | ≈ 3 300 (`MlErrorsDetails` 1 272, `IsValid` 415, `IsFail` 341) | ≈ 2 600 |
| 3 | `ValueObjects` / `.IO` | 18 | ≈ 700 | ≈ 450 |
| 4 | `Validation.*` | 7 | ≈ 120 | ≈ 300 |
| 5 | `Shared`/`FileSystem`/`Configuration`/`Logging` | 9 | ≈ 90 | ≈ 210 |
| 6 | `EntityFrameworkCore` | 16 | ≈ 330 | ≈ 470 |
| 7 | `Application` | 9 | ≈ 140 | ≈ 280 |
| 8 | `AspNetCore.Results` | 11 | ≈ 60 | ≈ 150 |
| 9 | `AspNetCore.Controllers[.OutputCaching]` | 14 | ≈ 130 | ≈ 280 |
| 10 | `Http` | 24 | ≈ 240 | ≈ 400 |
| — | Cambios de contrato asociados (no son renombrados) | 16 | — | — |

---

## 2. Estrategia y herramientas

### 2.1 Reglas que impone el documento (§ «Cómo aplicar los cambios sin romper a nadie»)

1. **Un nivel por bloque de commits**, empezando por los Niveles 0 y 1.
2. **Renombrar con refactor semántico (`F2`), nunca con buscar y reemplazar**, para que se actualicen
   `nameof`, `cref` de XML docs y referencias entre proyectos.
3. **Puente `[Obsolete]` para todo miembro 🔴** durante toda la v2; se eliminan en la 3.0.0.
4. **Los tipos e interfaces no admiten puente**: todos sus renombrados van juntos en esta mayor.
5. **No renombrar y cambiar comportamiento en el mismo commit.** No colar arreglos de bugs.
6. **La familia `…WithValue`/`…WithoutValue` va en su propio commit**, separada del resto de `Map`/`Bind`.
7. **La documentación se actualiza en el mismo bloque** que el renombrado.
8. **Changelog con columnas antes → después** (es exactamente el entregable de la § 4).

### 2.2 Herramienta 1 — Renombrador semántico Roslyn (`scripts/tools/MlRename/`)

Como el `F2` del IDE no se puede automatizar desde aquí y buscar-y-reemplazar está prohibido por
el documento (y además falla por H4), propongo una pequeña consola .NET **fuera de la solución**
que hace exactamente lo mismo que `F2`, pero por lotes y de forma reproducible:

| Pieza | Qué hace |
|---|---|
| `Microsoft.Build.Locator` + `MSBuildWorkspace` | Abre `MoralesLarios.OOFP.sln` con el modelo semántico completo. |
| `Renamer.RenameSymbolAsync` | Renombra **el símbolo**, no el texto: `IsValid` de `MlResult<T>` sí, `IsValid` de `ValueObject` no. Opciones: `RenameOverloads = true`, `RenameInComments = true`, `RenameInStrings = false`, `RenameFile = true` (renombra el `.cs` cuando coincide con el tipo). |
| Entrada `scripts/tools/MlRename/renames.<fase>.json` | Una entrada por cambio: `{ "nivel", "simbolo" (ID de documentación: `T:`, `M:`, `P:`, `F:`, `N:`), "nuevo", "puente": true/false }`. Revisable antes de ejecutar. |
| **Generador de puentes** | Para cada miembro 🔴 con `"puente": true`, captura la firma **antes** del renombrado y emite un reenvío en un fichero aparte `*.Obsolete.cs` (ver § 2.3). |
| **Registro** `Cambios Realizados/_registro.csv` | Cada cambio aplicado añade una fila: nivel, tipo, proyecto, antes, después, puente, nº de referencias actualizadas, commit. **Es la fuente del entregable final.** |
| Modo `--dry-run` | Lista referencias afectadas por símbolo sin escribir nada. Se ejecuta y se revisa antes de cada fase. |

Para lo que **no** es un símbolo C# se usan scripts PowerShell deterministas con las guardas ya
conocidas (recuento exacto de coincidencias esperadas, recuento de líneas, UTF-8 con BOM):

| Qué | Herramienta |
|---|---|
| Carpetas, `.csproj`, `.sln`, `ProjectReference` | `git mv` + script PowerShell (conserva los GUID del `.sln`) |
| Cadenas literales (mensajes de error, cabeceras HTTP, rutas) | script PowerShell con regex anclada **dentro de comillas** |
| Documentación `.md` (README, `__Doc/`) y nombres de fichero `.md` | script PowerShell con la tabla del `_registro.csv` como entrada |

### 2.3 Forma de los puentes `[Obsolete]`

```csharp
// MlResultActionsBind.Obsolete.cs  (fichero nuevo; se borra entero en la 3.0.0)
public static partial class MlResultActionsBind
{
    [Obsolete("Renamed to BindSaveValueInDetailsIfFailedFuncResult. Will be removed in 3.0.0.", error: false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(this MlResult<T>          source,
                                                                                         Func<T, MlResult<TReturn>> func)
        => source.BindSaveValueInDetailsIfFailedFuncResult(func);
}
```

- **`[EditorBrowsable(Never)]`** oculta el nombre antiguo del IntelliSense: el código del consumidor
  sigue compilando (con *warning* CS0618), pero nadie nuevo lo descubre. Resuelve el principal
  inconveniente del puente (que la errata siga a la vista).
- **Un fichero `*.Obsolete.cs` por clase**: en la 3.0.0 se eliminan los puentes borrando ficheros, sin
  tocar el código real. Requiere marcar las clases estáticas como `partial` (no rompe nada).
- **Mensaje en inglés** (principio 10 del documento: un solo idioma en el código). ❓D-4
- **Sin puente:** tipos, interfaces, namespaces, proyectos, parámetros genéricos, nombres de
  parámetro y miembros `internal`/`private` (como dice el documento).

### 2.4 Protocolo por paso (no se avanza si algo falla)

```text
1. MlRename --dry-run   → revisar la lista de referencias afectadas
2. MlRename             → aplicar  (o script PowerShell con guardas)
3. git diff --stat      → ¿solo los ficheros esperados?
4. dotnet build MoralesLarios.OOFP.sln -v q --nologo   → 0 errores
5. dotnet test  (proyectos afectados)                   → mismo resultado que la línea base
6. commit  "v2 Nn: <antes> → <después>"
```

Si el *diff* contiene algo no previsto → `git checkout -- .` y se reintenta con un alcance más estrecho.

### 2.5 Criterios de aceptación globales

- [ ] `dotnet build MoralesLarios.OOFP.sln` → **0 errores**; warnings: los 4 CS8604 conocidos de
      `Helpers/EnsureFp.Strings.cs` y **ningún CS0618** (nadie dentro de la solución usa un puente).
- [ ] Tests: **mismo resultado que la línea base** proyecto a proyecto (núcleo: 781 en verde). Se
      compara el **número** de tests, no solo que estén verdes, para detectar tests perdidos.
- [ ] `scripts/inventario-nomenclatura.ps1` → **0** para cada nombre antiguo, salvo en ficheros
      `*.Obsolete.cs` y en `Temas Pendientes/` / `Cambios Realizados/`.
- [ ] Ningún `.md` de la solución (fuera de esas dos carpetas) cita un nombre antiguo.

---

## 3. Fases de ejecución

**Leyenda:** 🔴 ruptura pública · 🟡 interna · 🟢 sin ruptura · 🌉 lleva puente `[Obsolete]` ·
🔒 bloqueado por `MlResultBucles.cs` · ⚙️ cambio de contrato (va a la Fase 12) · ⏭ no aplica (0 ocurrencias).

### Fase 0 — Preparación

| Paso | Acción |
|---|---|
| 0.1 | **Tú:** commit de tus cambios pendientes (`ValueObjects.IO.2.Tests.Unit.csproj`). |
| 0.2 | Rama `release/2.0.0-nomenclatura`. |
| 0.3 | **Línea base:** `dotnet build` de la solución + `dotnet test` de **cada** proyecto de test; guardar en `Cambios Realizados/_linea-base.txt` (nº de tests por proyecto y resultado). Los de integración (`EFCore.Integration.Tests`, `HttpClients.Tests.Integration`) pueden necesitar infraestructura: se anota su estado real y se compara contra él. |
| 0.4 | Crear `scripts/tools/MlRename/` (§ 2.2), probarlo con un `--dry-run` sobre `IsValid` y comprobar que **excluye** los `IsValid` de `ValueObjects`. |
| 0.5 | Crear `Cambios Realizados/` con `_registro.csv` vacío (cabecera). |
| 0.6 | Marcar `partial` las clases estáticas que van a recibir puentes (commit independiente, 🟢). |

### Fase 1 — Nivel 0: solución, proyectos y carpetas

**Orden interno:** borrados → núcleo → proyectos satélite → proyectos de test → carpetas internas.
Cada proyecto: `git mv` de carpeta y `.csproj` → `AssemblyName`/`PackageId`/`RootNamespace` →
`.sln` (conservando GUID) → `ProjectReference` → namespaces (MlRename, `N:`) → `InternalsVisibleTo`
→ README. Un commit por proyecto.

#### 1.a Soluciones

| Antes | Después | |
|---|---|---|
| `MoralesLarios.FOOP/MoralesLarios.FOOP.sln` | *(eliminado)* | 🟢 |
| `MoralesLarios.OOFP - copia.sln` | *(eliminado)* | 🟢 |

#### 1.b Proyectos de código

| Antes | Después | |
|---|---|---|
| carpeta `MoralesLarios.FOOP/` + `MoralesLarios.OOFP.csproj` | `MoralesLarios.OOFP.Core/` + `MoralesLarios.OOFP.Core.csproj` | 🔴 ❓D-2 |
| `MoralesLarios.OOFP.Internals` | **fusionado en** `MoralesLarios.OOFP.Shared` (H1) | 🔴 |
| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` | 🔴 |
| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` | 🔴 |
| `MoralesLarios.OOFP.Validation.Dataannotations` | `MoralesLarios.OOFP.Validation.DataAnnotations` | 🔴 |
| `MoralesLarios.OOFP.Validation.FluentValidations` | `MoralesLarios.OOFP.Validation.FluentValidation` *(N4)* | 🔴 |
| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` | 🔴 |
| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` | 🔴 |
| `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` | 🔴 |
| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` | 🔴 |
| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` | 🔴 |
| `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` | 🔴 |
| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` | 🔴 |
| `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` *(derivado)* | 🔴 ❓D-3 |
| `MoralesLarios.OOFP.EFCore.WebApi` *(app de ejemplo)* | `MoralesLarios.OOFP.EntityFrameworkCore.Sample` *(derivado)* | 🟢 ❓D-3 |

#### 1.c Proyectos de test

| Antes | Después | |
|---|---|---|
| `MoralesLarios.OOFP.Unit.Tests` | `MoralesLarios.OOFP.Core.Tests.Unit` | 🟢 |
| `MoralesLarios.OOFP.EFCore.Infrastructure.Tests` | `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` | 🟢 |
| `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit` | `MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit` | 🟢 |
| `MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit` | **fusionado** en el anterior (H8) | 🟢 |
| `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests` | `MoralesLarios.OOFP.Logging.Tests.Unit` | 🟢 |
| `MoralesLarios.OOFP.EFCore.Integration.Tests` | `MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.Utilities.Tests.Unit` | `MoralesLarios.OOFP.Configuration.Tests.Unit` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit` | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit` | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.WebApi.Tests.Unit` | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.WebServices.Tests.Unit` | `MoralesLarios.OOFP.Application.Tests.Unit` *(derivado)* | 🟢 |
| `MoralesLarios.OOFP.HttpClients.Tests.Unit` / `.Tests.Integration` | `MoralesLarios.OOFP.Http.Tests.Unit` / `.Tests.Integration` *(derivado)* | 🟢 |

*(derivado)* = el documento no lo nombra, pero se sigue de renombrar el proyecto que prueban.

#### 1.d Carpetas y ficheros internos

| Antes | Después | |
|---|---|---|
| `MoralesLarios.FOOP/__Doc/` (58 ficheros) | `docs/` ❓D-9 | 🟢 |
| `__Doc/Bucle/` | `docs/loops/` | 🟢 |
| `__Doc/Types/` | `docs/core-types/` | 🟢 |
| `__Doc/PendingTasks.txt` (8 KB) | `Temas Pendientes/PendingTasks-legacy.md` (crear las *issues* de GitHub lo haces tú) | 🟢 |
| `EFCore/OopRepos/` (18 ficheros) | `EntityFrameworkCore/Repositories.Imperative/` | 🟢 |
| `EFCore/Repos/` (18 ficheros) | `EntityFrameworkCore/Repositories/` | 🟢 |
| `EFCore/Helpers/Constants.cs` (clase vacía, N6) | *(eliminado)* | 🟢 |
| `HttpClients/ParamsInfo/` | `Http/Requests/` | 🟢 |
| `Internals/Info/` | `Shared/Pagination/` | 🟢 |
| `WebServices/Services/GenService.cs` (0 bytes) | *(eliminado)* | 🟢 |

> ℹ️ Mover carpetas de código **cambia los namespaces** si siguen la ruta (`…EFCore.OopRepos`).
> Se actualizan con MlRename (`N:`) en el mismo commit.

### Fase 2 — Nivel 1: erratas

Se aplica el **nombre final** (H2). Lo que en N2/N3 vuelve a renombrarse se indica en la columna «Final en».

| Antes | Después | Cód. | Tests | Docs | Proyecto | |
|---|---|---|---|---|---|---|
| `…IfFaild…` (`BindSaveValueInDetailsIfFaildFuncResult[Async]`, `TryBind…`; 15 sobrecargas) | `…IfFailed…` | 26 | 17 | 45 | Core | 🔴🌉 |
| `MergeErrorsDetailsIfFailDiferentTypes[Async]` (5 sobrecargas) | `MergeErrorsDetailsIfFailDifferentTypes[Async]` | 24 | 4 | 4 | Core | 🔴🌉 |
| `ChangeReturnResultAlwaisValid/Fail[Async]` (16 sobrecargas) | `ChangeReturnResultAlwaysValid/Fail[Async]` | 28 | 20 | 11 | Core | 🔴🌉 |
| `FusionErrosIfExists` | `MergeErrorsIfExists` ❓D-5 | 4 | 0 | 57 | Core, Validation.* | 🔴🌉 |
| `FusionFailErros[Async]` | `MergeFailureErrors[Async]` (final N2) | 29 | 10 | 36 | Core | 🔴🌉🔒 |
| `MlErrorsDetails` | `MlErrorDetails` → **se hace en Fase 3** (es N2 y es el cambio más grande) | — | — | — | — | — |
| `FromStringLenght` | `FromLength` (final N3) | 3 | 0 | 3 | ValueObjects | 🔴🌉 |
| `FromIntLenght` | `FromIntLength` | 3 | 0 | 3 | ValueObjects | 🔴🌉 |
| `MinLenght` (y resto de `Lenght`: 74 + 24 en 16 ficheros) | `MinLength` / `…Length` | 74 | 24 | 16 | ValueObjects | 🔴🌉 |
| `MaxLenght` | — | 0 | 0 | 0 | — | ⏭ |
| `Id.Bydouble` | `Id.FromDouble` (final N3: `By…` → `From…`) | 1 | 0 | 3 | ValueObjects | 🔴🌉 |
| `Id.Fromdouble` | `Id.FromDouble` ⚠️ choca con el anterior → ❓D-6 | 1 | 0 | 3 | ValueObjects | 🔴🌉 |
| `ExistDirectory` | `ExistingDirectoryPath` (final N3; tipo) | 10 | 20 | 41 | FileSystem, ValueObjects.IO | 🔴 |
| `AddScopedtGenServicesFpWithoutReposGeneral` | `AddScopedCrudServicesFpWithoutRepositories` | 1 | 1 | 10 | Application | 🔴🌉 |
| `AddScopedtGenServicesDuplexFpWithoutReposGeneral` | `AddScopedCrudServicesFpWithoutRepositories<TRequest,TResponse>` (sin `Duplex`, N7) | 1 | 0 | 2 | Application | 🔴🌉 |
| `TEnumrableResponse` (genérico) | `TItem` (final N10) | 5 | 0 | 6 | Http | 🟢 |
| `BaseAdress` | `BaseAddress` | 9 | 0 | 1 | Http | 🟢/🔴 |
| `K` (genérico de `IHttpClientFactoryManager`) | `TResponse` | — | — | — | Http | 🟢 |
| `TryMapIAsyncf` | `TryMapIfAsync` | 0 | 0 | 10 | — | ✅ ya hecho (solo docs) |

**Textos visibles por el usuario** (script PowerShell, solo dentro de literales):

| Antes | Después | Cód. | Proyecto |
|---|---|---|---|
| `"… is not soported"` | `"… is not supported"` | 2 | AspNetCore.Controllers |
| `"is diferent type"` | `"is of a different type"` | 2 | AspNetCore.Controllers |
| `"source no be null"` / `"{x} no be null"` | `"… must not be null"` | 4 | Validation.* |
| `"source no be empty"` | `"source must not be empty"` | 2 | Validation.* |
| `"isn't null"` (cuando sí es null) | `"must not be null"` | 12 | AspNetCore.Controllers |

> ⚠️ Cambiar textos de error puede romper tests o consumidores que comparen mensajes. Se buscan
> los literales también en tests antes de aplicar.

### Fase 3 — Nivel 2: núcleo `MlResult`

Es la fase más grande (≈ 3 300 referencias). Se divide en **commits independientes por familia**,
y la familia `…WithValue`/`…WithoutValue` va en su propio commit (regla 6).

#### 3.a Tipo `MlResult<T>`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `IsValid` (solo el de `MlResult<T>`, H4) | `IsSuccess` | 149 | 266 | 118 | 🔴🌉 |
| `IsFail` | `IsFailure` | 59 | 282 | 56 | 🔴🌉 |
| `SecureValidValue()` | `GetValueOrThrow()` | 19 | 104 | 40 | 🔴🌉 |
| `Value` (`internal protected`) | `SuccessValue` | — | — | — | 🟡 |
| `ErrorsDetails` (`internal protected`) | `FailureDetails` | 18 | 0 | 15 | 🟡 |

#### 3.b `MlErrorsDetails` y miembros

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `MlErrorsDetails` (tipo; 88 ficheros, 11 proyectos) | `MlErrorDetails` | 1 169 | 103 | 671 | 🔴 |
| `MlErrorsDetailsActions` / `MlErrorsDetailsExtensions` (+ ficheros y tests) | `MlErrorDetailsActions` / `MlErrorDetailsExtensions` | incl. | incl. | incl. | 🔴 |
| `.Details` | `.Metadata` | 30 | 83 | 27 | 🔴🌉 |
| `ToErrorsMessages()` | `GetErrorMessages()` | 1 | 4 | 169 | 🔴🌉 |
| `ToErrorsDescription()` | `GetErrorsSummary()` | 9 | 0 | 205 | 🔴🌉 |
| `ToDetailsDescription()` | `GetMetadataSummary()` | 1 | 0 | 41 | 🔴🌉 |
| `HasKeyDetails(string)` | `ContainsMetadata(string key)` | 3 | 0 | 5 | 🔴🌉 |
| `HasValueDetails()` | `ContainsValueMetadata()` | 2 | 3 | 2 | 🔴🌉 |
| `HasExceptionDetails()` | `ContainsException()` | 3 | 7 | 7 | 🔴🌉 |
| `GetDetailValue<T>()` (+`Async`) | `GetMetadataValue<T>()` | 29 | 7 | 92 | 🔴🌉 |
| `GetDetailException()` (+`Async`) | `GetException()` | 116 | 4 | 170 | 🔴🌉 |
| `AddDetail<T>(…)` | `AddMetadataOrThrow<T>(…)` (renombrado puro; `WithMetadata` inmutable → ⚙️ Fase 12) ❓D-7 | 9 | 1 | 20 | 🔴🌉 |

#### 3.c Constantes (`Helpers/Constants.cs`, hoy `public static string X { get; }`)

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `EX_DESC_KEY` | `ExceptionMetadataKey` | 24 | 78 | 34 | 🔴🌉 |
| `VALUE_KEY` | `ValueMetadataKey` | 15 | 28 | 28 | 🔴🌉 |
| valor `"Ex"` | `"Exception"` | — | — | — | ⚙️ Fase 12 (cambia datos en logs/ProblemDetails) |

#### 3.d Familias y clases

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `MlResultBucles` | `MlResultCollections` | 1 | 0 | 45 | 🔴🔒 |
| `Projection` / `ProjectionAsync` | `Traverse` / `TraverseAsync` | 60 | 74 | 234 | 🔴🌉🔒 |
| `MlResultTransformations` | `MlResultConversions` | 4 | 2 | 30 | 🔴 |
| `MlResultActionsMap` | `MlResultMapExtensions` ❓D-8 | 1 | 1 | 54 | 🟡 |
| `EmptyToFailed` (+`Async`) | `FailIfEmpty` | 16 | 0 | 87 | 🔴🌉 |
| `NullToFailed` (+`Async`) | `FailIfNull` | 30 | 4 | 131 | 🔴🌉 |
| `BoolToResult` (+`Async`) | `FailIfFalse` ❓D-5 | 47 | 6 | 122 | 🔴🌉 |
| `Do` (3, `MlResultActionsSeveral.cs`) | *(eliminado, puente → `ExecSelf`)* ❓D-5 | 3 | 0 | 9 | 🔴🌉 |
| `Combine` (+`Async`) | `CombineAll` ❓D-5 | 147 | 24 | 53 | 🔴🌉 |
| `EnsureFp.That`, `…IfFailWithException`, `…Always`, `Try…`, `…Fp` | *(se mantienen)* | — | — | — | ✅ |

### Fase 4 — Nivel 3: `ValueObjects` y `ValueObjects.FileSystem`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `Key` | `ServiceKey` ❓D-5 | — | — | — | 🔴 |
| `Name` | `MemberName` ❓D-5 | — | — | — | 🔴 |
| `Empty` | `EmptyValue` ❓D-5 | — | — | — | 🔴 |
| `IntNotNegative` / `DecimalNotNegative` / `DoubleNotNegative` / `LongNotNegative`… | `…NonNegative` | 71 | 36 | 95 | 🔴 |
| `…NotNegative.limit` | `MinAllowedValue` (renombrado; `private const` → ⚙️ Fase 12) | — | — | 28 | 🔴🌉 |
| `RangeEnumValueObject` (fichero comentado) | *(eliminado)* | 10 | 3 | 6 | 🟢 |
| `StringBetweenLength` | `StringLengthBetween` | 9 | 6 | 9 | 🔴 |
| Factorías `By…` (`ByString`, `ById`…) | `From…` (`FromString`, …) | 42 | 51 | 70 | 🔴🌉 |
| `BuildErrorMessage` (H5; 43 ficheros) | `FormatErrorMessage` | 140 | 3 | 24 | 🟡 |
| `ExistsFile` | `ExistingFilePath` | 14 | 26 | 90 | 🔴 |
| `ExistDirectory` | `ExistingDirectoryPath` | 10 | 20 | 41 | 🔴 |
| `ExistsFile.ByString` | `ExistingFilePath.FromPath` | incl. | incl. | incl. | 🔴🌉 |
| `MlFile` | `FilePath` ❓D-5 | 9 | 0 | 41 | 🔴 |
| `MlDirectory` | `DirectoryPath` ❓D-5 | 9 | 0 | 32 | 🔴 |
| `MlFile.EndpointPattern` / `MlDirectory.EndpointPattern` | `SearchPattern` | 12 | 0 | 12 | 🔴🌉 |
| Constructores públicos que lanzan | `private` + solo `From…` | — | — | — | ⚙️ Fase 12 |

### Fase 5 — Nivel 4: `Validation.*`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| namespace `…Validation.Dataannotations` | `…Validation.DataAnnotations` *(Fase 1)* | 21 | 26 | 116 | 🔴 |
| namespace `…Validation.FluentValidations` | `…Validation.FluentValidation` *(Fase 1)* | 23 | 36 | 71 | 🔴 |
| `DataannotationsValidator` | `DataAnnotationsValidator` | 3 | 4 | 25 | 🔴 |
| `MlValidableFp<T>` | `MlSelfValidatable<T>` (renombrado; → interfaz `IMlSelfValidatable<T>` en ⚙️ Fase 12) | 1 | 0 | 41 | 🔴 |
| `ValidateObject` (`Validation.Dataannotations`) | `ValidateDataAnnotations` (arregla CS0121) | 2 | 0 | 39 | 🔴🌉 |
| `ValidateAsync` (async falso) | `Validate` | — | — | — | 🔴🌉 |
| `IValidator` / `IValidatorFp` | — | 0 | 0 | 0 | ⏭ |

### Fase 6 — Nivel 5: `Shared`, `FileSystem`, `Configuration`, `Logging`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `PaginationInfo` | `PageRequest` | 21 | 0 | 62 | 🔴 |
| `PaginationResultInfo` | `PagedResult<T>` | 22 | 0 | 48 | 🔴 |
| `ProblemDetailsInfo` | `ProblemDetailsData` ❓D-5 | 6 | 6 | 19 | 🔴 |
| `IMlConfigManager` | `IMlConfigurationReader` | 3 | 2 | 17 | 🔴 |
| `GetSection` / `GetValue` (Configuration) | `TryGetSection` / `TryGetValue` | — | — | — | 🟡🌉 |
| `GeneralExtensionLoggers` (+ `Critical/Error/Warning/…`) | `MlResultLogExtensions` (ficheros parciales `.<Level>.cs`) | 7 | 0 | 20 | 🔴 |
| `LogMlResultFinalAsync` | `LogCompletedResultAsync` ❓D-5 | 19 | 0 | 14 | 🔴🌉 |
| `…Fp` en métodos de log | *(sin sufijo)* | — | — | — | 🔴🌉 |
| `MlFileHelper` / `MlFileUtils` / `ReadAllTextFp` | — | 0 | 0 | 0 | ⏭ |

### Fase 7 — Nivel 6: `EntityFrameworkCore`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `EFRepoBase` | `ThrowingEfRepository` | 9 | 0 | 2 | 🔴 |
| `EFRepoBaseFp` | `EfRepository` | 8 | 0 | 5 | 🔴 |
| `EFRepo…` (resto de la familia: 218 + 8 en 44 ficheros) | `EfRepository…` | 218 | 8 | 217 | 🔴 |
| `EFRepoReaderFp` / `WriterFp` / `UpdaterFp` / `DeleterFp` | `EfReadRepository` / `EfCreateRepository` / `EfUpdateRepository` / `EfDeleteRepository` | 17 | 0 | 108 | 🔴 |
| `EFRepoReaderPaginationFp` | `EfPagedReadRepository` | 4 | 0 | 4 | 🔴 |
| `IEFRepoWriterFp` (comentado) | *(eliminado)* | 10 | 0 | 8 | 🟢 |
| `ResolveRepoFp<T>` | `GetRepository<T>` | 14 | 0 | 4 | 🔴🌉 |
| `AddSingletonOOFPRepos…` | `AddOofpRepositoriesSingleton…` | 2 | 0 | 5 | 🔴🌉 |
| `AddScopedOOFPRepos…` | `AddOofpRepositories…` | 2 | 0 | 18 | 🔴🌉 |
| `enum OrderBy` | `SortDirection` | 1 | 0 | 1 | 🔴 |
| `Asc` / `Desc` | `Ascending` / `Descending` | — | — | — | 🔴 |
| `TryGetInternalData` | `QueryPageAsync` | 3 | 0 | 3 | 🟡🌉 |
| `TryLast` / `TryLastAsync` | `TryGetLast` / `TryGetLastAsync` | 26 | 0 | 76 | 🔴🌉 |
| `pk` / `pkValues` (parámetros) | `key` / `keyValues` | — | — | — | 🟡 |
| `_pkFields` (en Controllers) | `keySelector` | 14 | 0 | 16 | 🔴 |
| `JfCatasDbContext` | `TestDbContext` | 0 | 16 | 12 | 🟢 |

### Fase 8 — Nivel 7: `Application`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `IGenServiceFp<…>` | `ICrudServiceFp<…>` | 18 | 0 | 64 | 🔴 |
| `GenServiceFp<…>` | `CrudServiceFp<…>` | 10 | 0 | 30 | 🔴 |
| `…Duplex…` (registro) | *(sin sufijo; aridad genérica)* | 3 | — | — | 🔴🌉 |
| `MlProblemsDetails` | `MlProblemDetails` | 14 | 0 | 76 | 🔴 |
| `UpdateProblemDetailsAsync` | `UpdateAsync` (sobrecarga) | 10 | 0 | 11 | 🔴🌉 |
| `BuildNotFoundPkError` | `BuildEntityNotFoundError` | 9 | 1 | 15 | 🟡 |
| `AllAsync` (solo `Application`/`Controllers`; **no** el de `EnsureFp`, H4) | `GetPagedAsync` | ≈ 8 | — | — | 🔴🌉 |
| `validMessageBuilder` | `successMessageFactory` | 54 | 0 | 10 | 🟡 |
| `Services/GenService.cs` | *(eliminado en Fase 1)* | — | — | — | 🟢 |

### Fase 9 — Nivel 8: `AspNetCore.Results`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `MlResultWebExtensions` (ya `[Obsolete]` en 1.x) | *(eliminada, H7)* | 1 | 0 | 9 | 🔴 |
| `MlResultWebExtensionsPlus` | `MlResultActionResultExtensions` | 1 | 0 | 36 | 🔴 |
| `ToPostPdActionResult` | `ToCreatedProblemDetailsResult` | 8 | 4 | 11 | 🔴🌉 |
| `ToSimpleRepoPostActionResult` | `ToCreatedResult` | 5 | 10 | 7 | 🔴🌉 |
| `MlActionResults` | `MlResults` ❓D-5 | 12 | 0 | 38 | 🔴 |
| `ToProblemsDetailsInfo` | `ToProblemDetails` | 2 | 0 | 6 | 🔴🌉 |
| `ContieneCombinacion` | `ContainsAnyKey` | 2 | 0 | 1 | 🟡 |
| `notFoundKeys` | `NotFoundDetailKeys` (constante pública) | 2 | 0 | 2 | 🟡 |
| `"X-Page-Number"` / `"X-Page-Size"` (duplicados) | `MlPaginationHeaders.PageNumber` / `.PageSize` en `Shared` | 4 | 10 | 34 | 🔴 |
| `MlRequestWebExtensions` | `HttpRequestPaginationExtensions` | 1 | 0 | 10 | 🟡 |
| Métodos que devuelven `Task` sin `Async` | `…Async` | — | — | — | 🔴🌉 |

### Fase 10 — Nivel 9: `AspNetCore.Controllers` y `.OutputCaching`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `SimpleMlControllerBase<,>` | `MlCrudControllerBase<,>` | 4 | 0 | 29 | 🔴 |
| `SimpleMlComplexPkControllerBase<,>` | `MlCompositeKeyCrudControllerBase<,>` | 4 | 0 | 23 | 🔴 |
| `GetPkValues<TEntity>(…)` | `ParseKeyValues<TEntity>(…)` | 29 | 2 | 41 | 🔴🌉 |
| `GetPkValues(this string ids, …)` | `ParseCompositeKey(this string route)` | incl. | incl. | incl. | 🔴🌉 |
| `GetPkValuesString` (duplicado en Controllers y Http) | `FormatCompositeKey` (en `Shared`) | 8 | 0 | 6 | 🔴🌉 |
| `ConverterTo` | `ConvertToType` | 7 | 0 | 14 | 🔴🌉 |
| `ConvertDateTime` | `ParseDateTimeInvariant` (nombre; usar `InvariantCulture` → ⚙️ Fase 12) | 3 | 0 | 6 | 🔴🌉 |
| `GetPkValuesErrorMessage` | `BuildKeyParseErrorMessage` | 7 | 0 | 4 | 🟡 |
| `PkParameterAttribute` | `CompositeKeyParameterAttribute` | 15 | 0 | 21 | 🔴 |
| `BypassHeader` / `"X-Bypass-Cache"` | `CacheBypassHeader` (constante compartida) | 3 | 2 | 11 | 🟡 |
| `PerControllerOutputCachePolicy` | `ControllerScopedOutputCachePolicy` | 12 | 0 | 11 | 🟡 |
| `AddWebControllers` (vacío) · clases `…Base` no abstractas · rutas `id-str/{id}` | — | — | — | — | ⚙️ Fase 12 |

### Fase 11 — Nivel 10: `Http`

| Antes | Después | Cód. | Tests | Docs | |
|---|---|---|---|---|---|
| `IGenClientFp<TDto>` / `GenClientFp<TDto>` | `IRestClientFp<TDto>` / `RestClientFp<TDto>` | 14 | 11 | 90 | 🔴 |
| `IGenComplexClientFp<TDto>` / `GenComplexClientFp<,>` | `ICompositeKeyRestClientFp<TDto>` / `CompositeKeyRestClientFp<,>` | 8 | 2 | 24 | 🔴 |
| `…Duplex…` | *(sin sufijo)* | 1 | 15 | 12 | 🔴 |
| `IHttpClientFactoryManager<T, K>` | `IHttpApiInvoker<TRequest, TResponse>` | 9 | 6 | 44 | 🔴 |
| `GetHttpClientFactoryKey()` | `GetClientName()` | 2 | 0 | 24 | 🔴🌉 |
| `PostGetAsync` | `PostAndReadAsync` | 3 | 3 | 9 | 🔴🌉 |
| `PostGetPaginationAsync<TRequest,TEnumrableResponse>` | `PostAndReadPagedAsync<TRequest,TItem>` | 3 | 1 | 10 | 🔴🌉 |
| `GetPaginationAsync` | `GetPagedAsync` | 2 | 0 | 29 | 🔴🌉 |
| `InternalGetUrl` | `BuildRequestUri` | 3 | 0 | 2 | 🟡 |
| `InternalPostGetAsync` | `SendAndReadAsync` | 2 | 0 | 20 | 🟡 |
| `SetHeaderInfo` / `SetHeaderPageNumber` / `SetHeaderPageSize` | `AddPaginationHeaders` (renombre; pasar a `HttpRequestMessage` → ⚙️ Fase 12) | 10 | 0 | 12 | 🟡 |
| `ToResponseErrorsDescription` | `ReadErrorDescriptionAsync` (quitar `.Result` → ⚙️ Fase 12) | 6 | 0 | 9 | 🟡 |
| `SetHeaders` | `TrySetHeaders` (devolver `MlResult` → ⚙️ Fase 12) | 2 | 0 | 3 | 🟡 |
| `CallRequestParamsInfo` / `<T>` | `ApiCallOptions` / `ApiCallOptions<TBody>` | 9 | 0 | 33 | 🔴 |
| `CallRequestPaginationParamsInfo` | `PagedApiCallOptions` | 7 | 1 | 12 | 🔴 |
| `params object[] pk` | `object[] keyValues` | — | — | — | 🔴 |
| `AddGenClientFp` | `AddRestClient` | 1 | 4 | 10 | 🔴🌉 |
| `AddGenClientComplexFp` | `AddCompositeKeyRestClient` | 1 | 3 | 9 | 🔴🌉 |
| `AddGenClientDuplexComplexFp` | `AddCompositeKeyRestClient<TRequest,TResponse>` | 1 | 2 | 4 | 🔴🌉 |
| `httpClientFactoryKey` (parámetro `Key`) | `clientName` (`string`) | 55 | 8 | 20 | 🔴 |
| `configureHttpClientKey` | `configureClient` | 9 | 0 | 9 | 🟡 |

### Fase 12 — Cambios de contrato que el documento asocia a los renombrados

El documento los menciona junto a los nombres, pero **cambian comportamiento**, así que por su
propia regla 5 van **después** y **cada uno en su commit**, con test que lo demuestre:

| # | Cambio | Origen |
|---|---|---|
| C1 | Valor de `ExceptionMetadataKey`: `"Ex"` → `"Exception"` | N2 |
| C2 | `AddMetadataOrThrow` → `WithMetadata` inmutable (devuelve copia) ❓D-7 | N2 |
| C3 | `…NonNegative.limit` → `private const MinAllowedValue` (hoy `public static` mutable) | N3 |
| C4 | Constructores de value objects que lanzan → `private`, solo factorías `From…` | N3 |
| C5 | `MlSelfValidatable<T>` (clase abstracta) → `IMlSelfValidatable<T>` (interfaz) | N4 |
| C6 | `Validate`/`ValidateAsync`: unificar guardas de `null` o separar `ValidateOrThrow` | N4 |
| C7 | Eliminar `ProblemDetailsInfo` si `MlProblemDetails` lo cubre (tres tipos para un concepto) | N5 |
| C8 | `GeneralExtensionLoggers*` (una clase por nivel) → una clase parcial | N5 |
| C9 | `AddOofpRepositoriesSingleton` → eliminar (*captive dependency*) | N6 |
| C10 | Clases `…ControllerBase` → `abstract` | N9 |
| C11 | `AddWebControllers` vacío → eliminar | N9 |
| C12 | `ParseDateTimeInvariant` → usar `CultureInfo.InvariantCulture` | N9 |
| C13 | Rutas `id-str/{id}` → `{id}` en todos los verbos (**rompe el contrato HTTP**) | N9 |
| C14 | `PkParameterAttribute.Description` → `string?` | N9 |
| C15 | `AddPaginationHeaders` sobre `HttpRequestMessage`, no sobre `DefaultRequestHeaders` | N10 |
| C16 | `ReadErrorDescriptionAsync` sin `.Result`; `TrySetHeaders` devuelve `MlResult`; `Headers` → `IReadOnlyDictionary?` | N10 |

> Los **bugs** que el documento menciona de pasada (`FusionFailErros` sin `return`, `DecimalNotNegative`
> inconstruible, `StringBetweenLength` con min/max invertidos, `ResolveRepoFp` que crea un
> `ServiceProvider` por llamada, `BuildNotFoundPkError` que da 500, `Path.Combine` en URIs,
> `httpClientFactoryKey = null!`…) **no** entran aquí: pertenecen a
> `Mejoras-Prioridad-Critica-y-Alta.md` y se harán en su bloque.

### Fase 13 — Cierre

| Paso | Acción |
|---|---|
| 13.1 | Guía de estilo del documento → `CONTRIBUTING.md` (sufijos fijos, prohibidos, genéricos por rol, booleanos, factorías `From…`/`Create…`, constantes `PascalCase`, `_camelCase`, inglés en código). |
| 13.2 | `TReturn` vs `TResult`: el documento pide «uno de los dos, no ambos». Renombrado 🟢 (genéricos) ❓D-10. |
| 13.3 | Actualizar todos los `.md` de la solución (README de cada proyecto, `docs/`), incluidos los **nombres de fichero** que contienen nombres antiguos (p. ej. `11_BindSaveValueInDetailsIfFaildFuncResultAsync.md`). |
| 13.4 | Marcar los 11 niveles + cierre en el *checklist* de `Consejos-Nomenclatura.md`; marcar R1 cerrado en `Revision-Map-Bind-ExecSelf.md`. |
| 13.5 | Re-ejecutar `inventario-nomenclatura.ps1` → guardar como `scripts/inventario-nomenclatura.despues.txt`. |
| 13.6 | **Generar `Cambios Realizados/Renombrados-v2.0.0.md`** (§ 4). |
| 13.7 | Suite completa + comparación con la línea base. |

---

## 4. Entregable final: `Cambios Realizados`

```text
src/
└── Cambios Realizados/
    ├── Renombrados-v2.0.0.md      ← el documento que pides
    ├── _registro.csv              ← fuente de datos (una fila por cambio, la escribe MlRename y los scripts)
    └── _linea-base.txt            ← resultado de build/tests antes de empezar
```

`Renombrados-v2.0.0.md` **se genera a partir de `_registro.csv`** con
`scripts/generar-cambios-realizados.ps1`, así que la tabla refleja lo que **realmente** se aplicó, no
lo que se planeó. Estructura:

1. **Resumen**: nº de cambios por nivel y por tipo, nº de puentes `[Obsolete]`, proyectos afectados.
2. **Guía de migración rápida** para consumidores (paquetes NuGet renombrados primero).
3. **Una tabla por nivel**, con este formato:

| # | Nivel | Tipo | Versión 1.x (antes) | Versión 2.0.0 (después) | Proyecto | Ruptura | Puente `[Obsolete]` |
|---|---|---|---|---|---|---|---|
| 1 | 0 | Proyecto | `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` | — | 🔴 | — |
| 2 | 1 | Método | `BindSaveValueInDetailsIfFaildFuncResult` | `BindSaveValueInDetailsIfFailedFuncResult` | Core | 🔴 | ✅ hasta 3.0.0 |
| 3 | 2 | Propiedad | `MlResult<T>.IsValid` | `MlResult<T>.IsSuccess` | Core | 🔴 | ✅ hasta 3.0.0 |
| 4 | 2 | Tipo | `MlErrorsDetails` | `MlErrorDetails` | Core | 🔴 | — (tipo) |
| 5 | 1 | Texto | `"… is not soported"` | `"… is not supported"` | AspNetCore.Controllers | 🟢 | — |
| … | | | | | | | |

4. **Cambios de contrato (Fase 12)** en tabla aparte con columnas *antes / después / impacto*.
5. **No aplicados**, con motivo (⏭ inexistentes, 🔒 bloqueados si D-1 = no).

---

## 5. Decisiones pendientes

Con estas respuestas ejecuto sin más preguntas. Si estás de acuerdo con todas las recomendaciones,
basta con «OK a todo».

| # | Decisión | Opciones | Recomendación |
|---|---|---|---|
| **D-1** | `Types/MlResultBucles.cs` (WIP): `MlResultBucles`, `FusionFailErros`, `Projection` | (a) me das permiso tras hacer commit de tu trabajo · (b) se quedan fuera y se listan como «no aplicados» | **(a)**: si no, la v2 sale con los nombres viejos y su renombrado obligaría a otra mayor. |
| **D-2** | Núcleo `MoralesLarios.OOFP` → `MoralesLarios.OOFP.Core`: ¿también el **namespace**? | (a) solo carpeta, `.csproj`, `AssemblyName`, `PackageId` · (b) también namespace `MoralesLarios.OOFP.Core.*` | **(a)**: el namespace raíz `MoralesLarios.OOFP` es el de toda la familia; cambiarlo rompería **cada fichero** de cada consumidor sin beneficio. |
| **D-3** | Renombrados *derivados* que el documento no nombra (`ValueObjects.IO` → `.FileSystem`, `EFCore.WebApi` → `.Sample`, proyectos de test) | (a) aplicarlos · (b) solo los que nombra el documento | **(a)**: sin ellos quedan nombres incoherentes (`ValueObjects.IO` junto a `FileSystem`). |
| **D-4** | Idioma de los mensajes `[Obsolete]` | inglés · español (como el ejemplo del doc) | **Inglés** (principio 10 del propio documento). |
| **D-5** | Donde el documento da alternativas | `MergeErrorsIfExists`, `FailIfFalse`, eliminar `Do` (→ `ExecSelf`), `CombineAll`, `ServiceKey`, `MemberName`, `EmptyValue`, `FilePath`/`DirectoryPath`, `ProblemDetailsData`, `LogCompletedResultAsync`, `MlResults` | **Las indicadas** (la primera opción o la más explícita de cada par). |
| **D-6** | `Id.Bydouble` y `Id.Fromdouble` convergen en `Id.FromDouble` | ¿hacen lo mismo? → fusionar · ¿difieren? → `FromDouble` + `CreateFromDouble` (convención `From`=MlResult, `Create`=lanza) | Lo verifico en el código antes de la Fase 2 y te lo indico. |
| **D-7** | `AddDetail` | (a) renombre puro `AddMetadataOrThrow` + `WithMetadata` inmutable en Fase 12 · (b) directamente `WithMetadata` | **(a)**: respeta la regla «no renombrar y cambiar comportamiento a la vez». |
| **D-8** | `MlResultActionsMap` → `MlResultMapExtensions`: el doc solo nombra `Map` | (a) solo `Map` · (b) toda la familia `MlResultActions*` → `MlResult*Extensions` | **(b)**: renombrar solo una de seis clases hermanas crea la incoherencia que el documento combate. |
| **D-9** | `__Doc/` → `docs/` | (a) `..\docs\` raíz del repo (fuera del workspace actual) · (b) `src\docs\` | **(a)** es lo que dice el documento; **(b)** si quieres seguir viéndola desde este workspace. |
| **D-10** | `TReturn`/`TResult` → uno solo | (a) unificar a `TResult` en esta versión · (b) dejarlo para otra | **(b)**: es 🟢 (no rompe nada) y añade miles de líneas de *diff* que ocultarían los cambios reales. |
| **D-11** | Puentes `[Obsolete]` | (a) como dice el documento, con `[EditorBrowsable(Never)]` · (b) ruptura limpia sin puentes | **(a)**: es la estrategia del documento, y `EditorBrowsable` evita que las erratas sigan en IntelliSense. |

---

## 6. Riesgos

| Riesgo | Mitigación |
|---|---|
| Renombrar símbolos homónimos que no tocan (`IsValid` de `ValueObjects`, `AllAsync` de `EnsureFp`, `Path.Combine`) | Renombrador semántico Roslyn + `--dry-run` revisado (H4). |
| Corrupción silenciosa en los ficheros grandes de `Types/` (ya ocurrió con edición asistida) | Ninguna edición manual; solo MlRename/scripts + `git diff --stat` + build tras cada paso. |
| Los puentes generados no compilan (genéricos con restricciones, `params`, valores por defecto) | Se generan desde la sintaxis original y se compilan en el mismo paso; si falla un puente, se escribe a mano solo ese. |
| Cambios de ruta de proyecto rompen `ProjectReference`/`.sln` | Un proyecto por commit + build de la solución completa tras cada uno. |
| Conflicto con tu trabajo en `MlResultBucles.cs` | D-1. |
| Tests de integración que ya fallaban en la línea base | Se compara contra la línea base, no contra «todo verde». |
| Un *diff* tan grande que no se pueda revisar | Un commit por fila (o familia) de cada tabla; el `_registro.csv` enlaza cada cambio con su commit. |
| Consumidores que comparen mensajes de error o la clave `"Ex"` | Ambos van en commits propios y salen en la sección «Cambios de contrato» del entregable. |

---

### Anexo — Scripts de apoyo

| Script | Estado | Uso |
|---|---|---|
| `scripts/inventario-nomenclatura.ps1` | creado | Mide los 157 términos (código / tests / docs) + rutas de Nivel 0. Verificación «antes/después». |
| `scripts/inventario-nomenclatura.antes.txt` | creado | Foto del estado actual. |
| `scripts/inventario-ubicaciones.ps1` | creado | Localiza fichero a fichero los términos ambiguos (H4, H5). |
| `scripts/contar-renombrados.ps1`, `scripts/detalle-renombrados.ps1` | creados | Versiones previas, más reducidas. Se pueden borrar. |
| `scripts/tools/MlRename/` | **por crear** (Fase 0) | Renombrador semántico + generador de puentes + registro. |
| `scripts/generar-cambios-realizados.ps1` | **por crear** (Fase 13) | `_registro.csv` → `Cambios Realizados/Renombrados-v2.0.0.md`. |
