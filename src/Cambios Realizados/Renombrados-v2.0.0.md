# Renombrados de la versión 2.0.0

> Documento generado automáticamente desde [`_registro.csv`](./_registro.csv) por
> `scripts/generar-cambios-realizados.ps1`. **No lo edites a mano**: edita el CSV y vuelve a generarlo.

Este documento recoge **todos** los cambios de nomenclatura aplicados a la solución
`MoralesLarios.OOFP` en la versión 2.0.0, con su **valor inicial** y su **valor modificado**.

---

## Índice

1. [Resumen](#resumen)
2. [Guía de migración rápida](#guía-de-migración-rápida)
3. [Compatibilidad hacia atrás](#compatibilidad-hacia-atrás)
4. [Cambios de nomenclatura por nivel](#cambios-de-nomenclatura-por-nivel)
5. [Cambios de contrato (Fase 12)](#cambios-de-contrato-fase-12)
6. [Cambios planificados no aplicados](#cambios-planificados-no-aplicados)
7. [Validación](#validación)

---

## Resumen

| Concepto | Valor |
|---|---|
| Total de cambios registrados | **888** |
| Cambios de nomenclatura | **869** |
| Cambios de contrato (Fase 12) | **19** |
| Proyectos de la solución | 30 |
| Commits de la rama `release/2.0.0-nomenclatura` | 10 |

### Cambios por lote

| Lote | Nivel | Cambios |
|---|---|---|
| `fase1-nivel0` | 0 | 40 |
| `fase2-nivel1-erratas` | 1 | 101 |
| `fase3a-nivel2-mlresult` | 2 | 11 |
| `fase3b-nivel2-errordetails` | 2 | 22 |
| `fase3c-nivel2-familias` | 2 | 108 |
| `fase4a-nivel3-factorias` | 3 | 81 |
| `fase4b-nivel3-valueobjects` | 3 | 88 |
| `fase4c-nivel3-patrones` | 3 | 2 |
| `fase5-6-niveles4-5` | 4 | 43 |
| `fase7-nivel6-efcore` | 6 | 100 |
| `fase7b-nivel6-registro` | 6 | 1 |
| `fase8-10-niveles7-9` | 7 | 105 |
| `fase11-nivel10-http` | 10 | 91 |
| `fase11b-tipos-genericos-hermanos` | 7 | 12 |
| `fase11c-format-composite-key` | 9 | 2 |
| `fase11d-residuos` | 1 | 9 |
| `fase13-docs` | 13 | 19 |
| `fase13-genericos` | 13 | 10 |
| `fase14-residuos` | 14 | 24 |
| `fase12-contratos` | 12 | 19 |

---

## Guía de migración rápida

### 1. Paquetes NuGet renombrados

Sustituye primero las referencias de paquete; el resto de los renombrados no compilan hasta que esto este hecho.

| Paquete 1.x | Paquete 2.0.0 |
|---|---|
| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` |
| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` |
| `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` |
| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` |
| `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` |
| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` |
| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` |
| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` |
| `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` |
| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` |
| `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` |

### 2. Renombrados de alto impacto

Los mas frecuentes en codigo de consumidor. Todos tienen puente `[Obsolete]` salvo los marcados ⚠️.

| Antes | Después |
|---|---|
| `MlResult.IsValid` | `MlResult.IsSuccess` |
| `MlResult.IsFail` | `MlResult.IsFailure` |
| `MlErrorsDetails` ⚠️ | `MlErrorDetails` |
| `NullToFailed` | `FailIfNull` |
| `EmptyToFailed` | `FailIfEmpty` |
| `BoolToResult` | `FailIfFalse` |
| `Combine` | `CombineAll` |
| `Projection` / `ProjectionWhile` / `ProjectionSplit` | `Traverse` / `TraverseWhile` / `TraverseSplit` |
| `SecureValidValue` | `GetValueOrThrow` |
| `SecureFailErrorsDetails` | `GetFailureDetailsOrThrow` |
| `FusionFailErros` | `MergeFailureErrors` |
| `PaginationInfo` ⚠️ | `PageRequest` |
| `PaginationResultInfo<T>` ⚠️ | `PagedResult<T>` |
| `IGenServiceFp<TEntity, TDto>` ⚠️ | `ICrudServiceFp<TEntity, TDto>` |
| `SimpleMlControllerBase<...>` ⚠️ | `MlCrudControllerBase<...>` |
| `SimpleMlComplexPkControllerBase<...>` ⚠️ | `MlCompositeKeyCrudControllerBase<...>` |
| `MlActionResults` ⚠️ | `MlResults` |
| `IEfRepoFp<T>` ⚠️ | `IEfRepository<T>` |
| `GenClientFp<TDto>` ⚠️ | `RestClientFp<TDto>` |
| `GenComplexClientFp<TDto>` ⚠️ | `CompositeKeyRestClientFp<TDto>` |
| `IHttpClientFactoryManager` ⚠️ | `IHttpApiInvoker` |
| `IWrapperIO` ⚠️ | `IMlFileSystem` |
| `MlFile` / `MlDirectory` ⚠️ | `FilePath` / `DirectoryPath` |

### 3. Factorías de los Value Objects

Convención 2.0.0: `From...` devuelve `MlResult<T>` (segura) y `CreateFrom...` devuelve el tipo directo (lanza).
Además los constructores públicos que lanzaban excepción pasan a `protected` (C4), así que **hay que usar la factoría**.

---

## Compatibilidad hacia atrás

La columna **Puente** indica si el nombre antiguo sigue disponible:

- **Sí** → existe un miembro marcado con `[Obsolete]` que reenvía al nombre nuevo.
  El código de la 1.x **compila** con una advertencia `CS0618`.
- **No** → el nombre antiguo ha desaparecido: hay que actualizar el código que lo usaba.

> ⚠️ **Todos los puentes `[Obsolete]` se eliminan en la versión 3.0.0.**
> Los ficheros que los contienen siguen el patrón `*.Obsolete.cs`.

---

## Cambios de nomenclatura por nivel

### Nivel 0 → Solución, proyectos y carpetas

<sub>Lote `fase1-nivel0` → 40 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Solucion |  |  | `MoralesLarios.FOOP/MoralesLarios.FOOP.sln` | *(eliminada)* | No | Solucion duplicada |
| 2 | Solucion |  |  | `MoralesLarios.OOFP - copia.sln` | *(eliminada)* | No | Copia obsoleta de la solucion |
| 3 | Proyecto |  |  | `MoralesLarios.OOFP (carpeta MoralesLarios.FOOP)` | `MoralesLarios.OOFP.Core` | No | Paquete/assembly. El namespace raiz MoralesLarios.OOFP NO cambia (RootNamespace explicito) |
| 4 | Proyecto |  |  | `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared (fusionado)` | No | Shared ya existia: Info/*.cs, GlobalUsings y README pasan a Shared |
| 5 | Proyecto |  |  | `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` | No |  |
| 6 | Proyecto |  |  | `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` | No |  |
| 7 | Proyecto |  |  | `MoralesLarios.OOFP.Validation.Dataannotations` | `MoralesLarios.OOFP.Validation.DataAnnotations` | No |  |
| 8 | Proyecto |  |  | `MoralesLarios.OOFP.Validation.FluentValidations` | `MoralesLarios.OOFP.Validation.FluentValidation` | No | Referencias a FluentValidation.Results cualificadas con global:: (colision con el namespace propio) |
| 9 | Proyecto |  |  | `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` | No |  |
| 10 | Proyecto |  |  | `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` | No |  |
| 11 | Proyecto |  |  | `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` | No |  |
| 12 | Proyecto |  |  | `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` | No |  |
| 13 | Proyecto |  |  | `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` | No |  |
| 14 | Proyecto |  |  | `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` | No |  |
| 15 | Proyecto |  |  | `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` | No |  |
| 16 | Proyecto |  |  | `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` | No | Derivado (coherencia con FileSystem) |
| 17 | Proyecto |  |  | `MoralesLarios.OOFP.EFCore.WebApi` | `MoralesLarios.OOFP.EntityFrameworkCore.Sample` | No | Derivado: es una aplicacion de ejemplo |
| 18 | Proyecto test |  |  | `MoralesLarios.OOFP.Unit.Tests` | `MoralesLarios.OOFP.Core.Tests.Unit` | No |  |
| 19 | Proyecto test |  |  | `MoralesLarios.OOFP.EFCore.Infrastructure.Tests` | `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` | No | No contiene tests: es soporte |
| 20 | Proyecto test |  |  | `MoralesLarios.OOFP.EFCore.Integration.Tests` | `MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration` | No | Derivado |
| 21 | Proyecto test |  |  | `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit` | `MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit` | No |  |
| 22 | Proyecto test |  |  | `MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit` | *(eliminado)* | No | Fuera del .sln, no compilaba y no tenia tests: no habia nada que fusionar |
| 23 | Proyecto |  |  | `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests` | `MoralesLarios.OOFP.Logging.Sample` | No | Es una app de consola, no un proyecto de tests |
| 24 | Proyecto test |  |  | `MoralesLarios.OOFP.Utilities.Tests.Unit` | `MoralesLarios.OOFP.Configuration.Tests.Unit` | No | Derivado |
| 25 | Proyecto test |  |  | `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit` | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit` | No | Derivado |
| 26 | Proyecto test |  |  | `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit` | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit` | No | Derivado |
| 27 | Proyecto test |  |  | `MoralesLarios.OOFP.WebApi.Tests.Unit` | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit` | No | Derivado |
| 28 | Proyecto test |  |  | `MoralesLarios.OOFP.WebServices.Tests.Unit` | `MoralesLarios.OOFP.Application.Tests.Unit` | No | Derivado |
| 29 | Proyecto test |  |  | `MoralesLarios.OOFP.HttpClients.Tests.Unit` | `MoralesLarios.OOFP.Http.Tests.Unit` | No | Derivado |
| 30 | Proyecto test |  |  | `MoralesLarios.OOFP.HttpClients.Tests.Integration` | `MoralesLarios.OOFP.Http.Tests.Integration` | No | Derivado |
| 31 | Carpeta | Core |  | `MoralesLarios.FOOP/__Doc/` | `docs/` | No | Documentacion del nucleo en src/docs |
| 32 | Carpeta | Core |  | `__Doc/Bucle/` | `docs/loops/` | No |  |
| 33 | Carpeta | Core |  | `__Doc/Types/` | `docs/core-types/` | No |  |
| 34 | Fichero | Core |  | `__Doc/PendingTasks.txt` | `Temas Pendientes/PendingTasks-legacy.md` | No | Pendiente de convertir en issues de GitHub |
| 35 | Carpeta | EntityFrameworkCore |  | `EFCore/OopRepos/` | `EntityFrameworkCore/Repositories.Imperative/` | No | Namespace ...EFCore.OopRepos -> ...EntityFrameworkCore.Repositories.Imperative |
| 36 | Carpeta | EntityFrameworkCore |  | `EFCore/Repos/` | `EntityFrameworkCore/Repositories/` | No | Namespace ...EFCore.Repos -> ...EntityFrameworkCore.Repositories |
| 37 | Carpeta | Http |  | `HttpClients/ParamsInfo/` | `Http/Requests/` | No | Namespace ...HttpClients.ParamsInfo -> ...Http.Requests |
| 38 | Carpeta | Shared |  | `Internals/Info/` | `Shared/Pagination/` | No | Namespace ...Internals.Info -> ...Shared.Pagination |
| 39 | Fichero | Application |  | `WebServices/Services/GenService.cs` | *(eliminado)* | No | Fichero vacio (0 bytes) |
| 40 | Fichero | EntityFrameworkCore |  | `EFCore/Helpers/Constants.cs` | *(eliminado)* | No | Clase vacia sin uso |

### Nivel 1 → Erratas ortográficas

<sub>Lote `fase2-nivel1-erratas` → 101 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsBind` | `BindSaveValueInDetailsIfFaildFuncResult` | `BindSaveValueInDetailsIfFailedFuncResult` | Sí |  |
| 2 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsBind` | `BindSaveValueInDetailsIfFaildFuncResultAsync` | `BindSaveValueInDetailsIfFailedFuncResultAsync` | No |  |
| 3 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsBind` | `TryBindSaveValueInDetailsIfFaildFuncResult` | `TryBindSaveValueInDetailsIfFailedFuncResult` | No |  |
| 4 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsBind` | `TryBindSaveValueInDetailsIfFaildFuncResultAsync` | `TryBindSaveValueInDetailsIfFailedFuncResultAsync` | No |  |
| 5 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `MergeErrorsDetailsIfFailDiferentTypes` | `MergeErrorsDetailsIfFailDifferentTypes` | Sí |  |
| 6 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `MergeErrorsDetailsIfFailDiferentTypesAsync` | `MergeErrorsDetailsIfFailDifferentTypesAsync` | No |  |
| 7 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultChangeReturnResult` | `ChangeReturnResultAlwaisValid` | `ChangeReturnResultAlwaysValid` | Sí |  |
| 8 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultChangeReturnResult` | `ChangeReturnResultAlwaisValidAsync` | `ChangeReturnResultAlwaysValidAsync` | No |  |
| 9 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultChangeReturnResult` | `ChangeReturnResultAlwaisFail` | `ChangeReturnResultAlwaysFail` | No |  |
| 10 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultChangeReturnResult` | `ChangeReturnResultAlwaisFailAsync` | `ChangeReturnResultAlwaysFailAsync` | No |  |
| 11 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `FusionFailErros` | `MergeFailureErrors` | Sí | Errata + Fusion no es verbo ingles (nombre final del Nivel 2) |
| 12 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `FusionFailErrosAsync` | `MergeFailureErrorsAsync` | No | Errata + Fusion no es verbo ingles (nombre final del Nivel 2) |
| 13 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `FusionErrosIfExists` | `MergeErrorsIfExists` | Sí | Errata + Fusion no es verbo ingles |
| 14 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `FusionErrosIfExistsAsync` | `MergeErrorsIfExistsAsync` | No | Errata + Fusion no es verbo ingles |
| 15 | member | Core.Tests.Unit | `*` | `Faild` | `Failed` | No | SIN COINCIDENCIAS |
| 16 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultActionsErrorsDetailsTests` | `MergeErrorsDetailsIfFailDiferentTypesAsync_sourceSync_secondaryAsync_return_fail_with_merged_errors` | `MergeErrorsDetailsIfFailDifferentTypesAsync_sourceSync_secondaryAsync_return_fail_with_merged_errors` | No |  |
| 17 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisValid_validSource_return_validValue` | `ChangeReturnResultAlwaysValid_validSource_return_validValue` | No |  |
| 18 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisValid_failSource_return_failValidValue` | `ChangeReturnResultAlwaysValid_failSource_return_failValidValue` | No |  |
| 19 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisValidAsync_validSource_return_validValue` | `ChangeReturnResultAlwaysValidAsync_validSource_return_validValue` | No |  |
| 20 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisValidAsync_sourceAsync_failSource_return_failValidValue` | `ChangeReturnResultAlwaysValidAsync_sourceAsync_failSource_return_failValidValue` | No |  |
| 21 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisFail_validSource_return_validFailValue` | `ChangeReturnResultAlwaysFail_validSource_return_validFailValue` | No |  |
| 22 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisFail_failSource_return_failValue` | `ChangeReturnResultAlwaysFail_failSource_return_failValue` | No |  |
| 23 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisFailAsync_validSource_return_validFailValue` | `ChangeReturnResultAlwaysFailAsync_validSource_return_validFailValue` | No |  |
| 24 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultChangeReturnResultTests` | `ChangeReturnResultAlwaisFailAsync_sourceAsync_failSource_return_failValue` | `ChangeReturnResultAlwaysFailAsync_sourceAsync_failSource_return_failValue` | No |  |
| 25 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultBuclesTests` | `FusionFailErros_when_source_has_3_fails_return_fail_with_the_3_errorMessages` | `MergeFailureErrors_when_source_has_3_fails_return_fail_with_the_3_errorMessages` | No |  |
| 26 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultBuclesTests` | `FusionFailErros_when_source_has_validsAndFails_return_fail_onlyWith_failsErrorMessages` | `MergeFailureErrors_when_source_has_validsAndFails_return_fail_onlyWith_failsErrorMessages` | No |  |
| 27 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultBuclesTests` | `FusionFailErros_when_source_hasNoFails_notThrows_and_return_fail` | `MergeFailureErrors_when_source_hasNoFails_notThrows_and_return_fail` | No |  |
| 28 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultBuclesTests` | `FusionFailErros_when_source_isEmpty_notThrows_and_return_fail_with_noElementsMessage` | `MergeFailureErrors_when_source_isEmpty_notThrows_and_return_fail_with_noElementsMessage` | No |  |
| 29 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultBuclesTests` | `FusionFailErros_when_source_hasNoFails_should_return_fail_with_noElementsMessage` | `MergeFailureErrors_when_source_hasNoFails_should_return_fail_with_noElementsMessage` | No |  |
| 30 | member | Core.Tests.Unit | `*` | `FusionErrosIfExists` | `MergeErrorsIfExists` | No | SIN COINCIDENCIAS |
| 31 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween` | `FromIntLenght` | `FromIntLength` | Sí |  |
| 32 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan` | `FromIntLenght` | `FromIntLength` | Sí |  |
| 33 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan` | `FromIntLenght` | `FromIntLength` | Sí |  |
| 34 | Propiedad | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Key` | `MinLenght` | `MinLength` | Sí |  |
| 35 | Propiedad | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Name` | `MinLenght` | `MinLength` | Sí |  |
| 36 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength` | `FromStringLenght` | `FromStringLength` | Sí |  |
| 37 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength` | `FromStringLenght` | `FromStringLength` | Sí |  |
| 38 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength` | `FromStringLenght` | `FromStringLength` | Sí |  |
| 39 | member | ValueObjects | `*` | `lenght` | `length` | No | SIN COINCIDENCIAS |
| 40 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.IntBetween(int, int, int)` | `minLenght` | `minLength` | No |  |
| 41 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.IntBetween(int, int, int)` | `maxLenght` | `maxLength` | No |  |
| 42 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.BuildErrorMessage(int, int, int)` | `minLenght` | `minLength` | No |  |
| 43 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.BuildErrorMessage(int, int, int)` | `maxLenght` | `maxLength` | No |  |
| 44 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.IsValid(int, int, int)` | `minLenght` | `minLength` | No |  |
| 45 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.IsValid(int, int, int)` | `maxLenght` | `maxLength` | No |  |
| 46 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.FromIntLength(int, int, int)` | `minLenght` | `minLength` | No |  |
| 47 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.FromIntLength(int, int, int)` | `maxLenght` | `maxLength` | No |  |
| 48 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.ByIntLength(int, int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `minLenght` | `minLength` | No |  |
| 49 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween.ByIntLength(int, int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `maxLenght` | `maxLength` | No |  |
| 50 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.StringBetweenLength(MoralesLarios.OOFP.ValueObjects.NotEmptyString, int, int)` | `minLenght` | `minLength` | No |  |
| 51 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.StringBetweenLength(MoralesLarios.OOFP.ValueObjects.NotEmptyString, int, int)` | `maxLenght` | `maxLength` | No |  |
| 52 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.BuildErrorMessage(string, int, int)` | `minLenght` | `minLength` | No |  |
| 53 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.BuildErrorMessage(string, int, int)` | `maxLenght` | `maxLength` | No |  |
| 54 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.IsValid(string, int, int)` | `minLenght` | `minLength` | No |  |
| 55 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.IsValid(string, int, int)` | `maxLenght` | `maxLength` | No |  |
| 56 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.FromStringLength(string, int, int)` | `minLenght` | `minLength` | No |  |
| 57 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.FromStringLength(string, int, int)` | `maxLenght` | `maxLength` | No |  |
| 58 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.ByStringLength(string, int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `minLenght` | `minLength` | No |  |
| 59 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength.ByStringLength(string, int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `maxLenght` | `maxLength` | No |  |
| 60 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan.BuildErrorMessage(int, int)` | `lenght` | `length` | No |  |
| 61 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan.FromIntLength(int, int)` | `lenght` | `length` | No |  |
| 62 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan.ByIntLength(int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `lenght` | `length` | No |  |
| 63 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan.BuildErrorMessage(int, int)` | `lenght` | `length` | No |  |
| 64 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan.FromIntLength(int, int)` | `lenght` | `length` | No |  |
| 65 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan.ByIntLength(int, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `lenght` | `length` | No |  |
| 66 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength.BuildErrorMessage(string, int)` | `lenght` | `length` | No |  |
| 67 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength.FromStringLength(string, int)` | `lenght` | `length` | No |  |
| 68 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength.ByStringLength(string, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `lenght` | `length` | No |  |
| 69 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength.BuildErrorMessage(string, int)` | `lenght` | `length` | No |  |
| 70 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength.FromStringLength(string, int)` | `lenght` | `length` | No |  |
| 71 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength.ByStringLength(string, int, MoralesLarios.OOFP.Types.Errors.MlErrorsDetails)` | `lenght` | `length` | No |  |
| 72 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.AgeTests` | `Age_ValueMoreThanLenght_ReturnsValid` | `Age_ValueMoreThanLength_ReturnsValid` | No |  |
| 73 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.AgeTests` | `Age_ValueMoreThanLenght_ReturnsFail` | `Age_ValueMoreThanLength_ReturnsFail` | No |  |
| 74 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntBetweenTests` | `IntBetween_ValueLessThanMinLenght_ReturnsFail` | `IntBetween_ValueLessThanMinLength_ReturnsFail` | No |  |
| 75 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntBetweenTests` | `IntBetween_ValueMoreThanMaxLenght_ReturnsFail` | `IntBetween_ValueMoreThanMaxLength_ReturnsFail` | No |  |
| 76 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntLessThanTests` | `IntLessThan_ValueLessThanLenght_ReturnsValid` | `IntLessThan_ValueLessThanLength_ReturnsValid` | No |  |
| 77 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntLessThanTests` | `IntLessThan_ValueLessThanLenght_ReturnsFail` | `IntLessThan_ValueLessThanLength_ReturnsFail` | No |  |
| 78 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntMoreThanTests` | `IntMoreThan_ValueMoreThanLenght_ReturnsValid` | `IntMoreThan_ValueMoreThanLength_ReturnsValid` | No |  |
| 79 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntMoreThanTests` | `IntMoreThan_ValueMoreThanLenght_ReturnsFail` | `IntMoreThan_ValueMoreThanLength_ReturnsFail` | No |  |
| 80 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.NameTests` | `Name_ValueLessThanLenght_ReturnsValid` | `Name_ValueLessThanLength_ReturnsValid` | No |  |
| 81 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.NameTests` | `Name_ValueLessThanLenght_ReturnsFail` | `Name_ValueLessThanLength_ReturnsFail` | No |  |
| 82 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringBetweenLengthTests` | `StringBetweenLength_ValueLessThanMinLenght_ReturnsFail` | `StringBetweenLength_ValueLessThanMinLength_ReturnsFail` | No |  |
| 83 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringBetweenLengthTests` | `StringBetweenLength_ValueMoreThanMaxLenght_ReturnsFail` | `StringBetweenLength_ValueMoreThanMaxLength_ReturnsFail` | No |  |
| 84 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringMaxLengthTests` | `StringMaxLength_ValueLessThanLenght_ReturnsValid` | `StringMaxLength_ValueLessThanLength_ReturnsValid` | No |  |
| 85 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringMaxLengthTests` | `StringMaxLength_ValueLessThanLenght_ReturnsFail` | `StringMaxLength_ValueLessThanLength_ReturnsFail` | No |  |
| 86 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringMinLengthTests` | `StringMinLength_ValueLessThanLenght_ReturnsValid` | `StringMinLength_ValueLessThanLength_ReturnsValid` | No |  |
| 87 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringMinLengthTests` | `StringMinLength_ValueLessThanLenght_ReturnsFail` | `StringMinLength_ValueLessThanLength_ReturnsFail` | No |  |
| 88 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddScopedtGenServicesFpWithoutReposGeneral` | `AddScopedCrudServicesFpWithoutRepositories` | Sí |  |
| 89 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddScopedtGenServicesDuplexFpWithoutReposGeneral` | `AddScopedCrudServicesDuplexFpWithoutRepositories` | Sí |  |
| 90 | member | Application.Tests.Unit | `*` | `Scopedt` | `Scoped` | No | SIN COINCIDENCIAS |
| 91 | Generico | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PostGetPaginationAsync<TRequest, TEnumrableResponse>(MoralesLarios.OOFP.Http.Requests.CallRequestPaginationParamsInfo<TRequest>, System.Threading.CancellationToken)` | `TEnumrableResponse` | `TItem` | No |  |
| 92 | Generico | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PostGetPaginationAsync<TRequest, TEnumrableResponse>(MoralesLarios.OOFP.Http.Requests.CallRequestPaginationParamsInfo<TRequest>, System.Threading.CancellationToken)` | `TEnumrableResponse` | `TItem` | No |  |
| 93 | Texto | AspNetCore.Controllers | `Helpers/Extensions.cs` | `"The type {0} is not soported"` | `"The type {0} is not supported"` | No | Mensaje visible en el cuerpo del error HTTP |
| 94 | Texto | AspNetCore.Controllers | `SimpleMl*ControllerBase` | `"... or is diferent type to PK of ..."` | `"... or is of a different type than PK of ..."` | No | Mensaje visible (404) |
| 95 | Texto | Validation.* | `*Validator.cs` | `"{x} no be null"` | `"{x} must not be null"` | No |  |
| 96 | Texto | Validation.* | `*Validator.cs` | `"{x} no be empty"` | `"{x} must not be empty"` | No |  |
| 97 | Texto | AspNetCore.Controllers | `SimpleMl*ControllerBase` | `"{x} isn't null"` | `"{x} must not be null"` | No | El mensaje anterior afirmaba lo contrario de lo ocurrido |
| 98 | Comentario | Http | `HttpClientFactoryManager` | `BaseAdress` | `BaseAddress` | No | Solo aparecia en comentarios |
| 99 | Metodo | ValueObjects | `Id (Id.cs excluido de la compilacion)` | `Bydouble / Fromdouble` | `ByDouble / FromDouble` | No | Codigo muerto: <Compile Remove="Id.cs" /> |
| 100 | Documento | docs | `Bind` | `11_BindSaveValueInDetailsIfFaildFuncResultAsync.md` | `11_BindSaveValueInDetailsIfFailedFuncResultAsync.md` | No |  |
| 101 | Metodo |  | `-` | `TryMapIAsyncf` | `TryMapIfAsync` | No | Ya corregido en la revision Map/Bind (commit 1f5f910) |

### Nivel 2a → Estado y acceso de `MlResult`

<sub>Lote `fase3a-nivel2-mlresult` → 11 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Propiedad | Core | `MoralesLarios.OOFP.Types.MlResult<T>` | `IsValid` | `IsSuccess` | Sí | Valid es vocabulario de validacion, par simetrico IsSuccess/IsFailure |
| 2 | Propiedad | Core | `MoralesLarios.OOFP.Types.MlResult<T>` | `IsFail` | `IsFailure` | Sí |  |
| 3 | Propiedad | Core | `MoralesLarios.OOFP.Types.MlResult<T>` | `Value` | `SuccessValue` | No | internal protected |
| 4 | Propiedad | Core | `MoralesLarios.OOFP.Types.MlResult<T>` | `ErrorsDetails` | `FailureDetails` | No | internal protected |
| 5 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActions` | `SecureValidValue` | `GetValueOrThrow` | Sí | Secure sugeria seguridad, el metodo lanza si el resultado es fallido |
| 6 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActions` | `SecureValidValueAsync` | `GetValueOrThrowAsync` | Sí | Secure sugeria seguridad, el metodo lanza si el resultado es fallido |
| 7 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActions` | `SecureFailErrorsDetails` | `GetFailureDetailsOrThrow` | Sí | Derivado: pareja de GetValueOrThrow |
| 8 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActions` | `SecureFailErrorsDetailsAsync` | `GetFailureDetailsOrThrowAsync` | No | Derivado: pareja de GetValueOrThrow |
| 9 | Propiedad | Core | `MoralesLarios.OOFP.Helpers.Constants` | `EX_DESC_KEY` | `ExceptionMetadataKey` | Sí | El valor "Ex" no cambia aqui (cambio de contrato -> Fase 12) |
| 10 | Propiedad | Core | `MoralesLarios.OOFP.Helpers.Constants` | `VALUE_KEY` | `ValueMetadataKey` | Sí |  |
| 11 | Correccion | Core | `MlResultTransformations.SecureGetValueFromMlResultBoxed` | `p.Name == "IsValid" / "Value"` | `nameof(MlResult<object>.IsSuccess) / nameof(...SuccessValue)` | No | Reflexion por literal: el renombrado la rompia |

### Nivel 2b → `MlErrorsDetails` → `MlErrorDetails`

<sub>Lote `fase3b-nivel2-errordetails` → 22 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Propiedad | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetails` | `Details` | `Metadata` | Sí | errorDetails.Details era redundante, contiene metadatos adjuntos al error |
| 2 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorDetailsExtensions` | `ToErrorsMessages` | `GetErrorMessages` | Sí | To... se reserva para conversiones |
| 3 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorDetailsExtensions` | `ToErrorsDescription` | `GetErrorsSummary` | No |  |
| 4 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorDetailsExtensions` | `ToDetailsDescription` | `GetMetadataSummary` | Sí |  |
| 5 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `HasKeyDetails` | `ContainsMetadata` | Sí |  |
| 6 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `HasValueDetails` | `ContainsValueMetadata` | Sí |  |
| 7 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `HasValueDetailsAsync` | `ContainsValueMetadataAsync` | Sí |  |
| 8 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `HasExceptionDetails` | `ContainsException` | Sí |  |
| 9 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `HasExceptionDetailsAsync` | `ContainsExceptionAsync` | Sí |  |
| 10 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `AddDetail` | `AddMetadataOrThrow` | Sí | Muta y lanza si la clave existe, WithMetadata inmutable queda como cambio de contrato |
| 11 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `AddDetails` | `AddMetadataRange` | No | Derivado. BUG preexistente: descarta la union de metadatos |
| 12 | Metodo | Core | `MoralesLarios.OOFP.Types.Errors.MlErrorsDetailsActions` | `AddDetailValue` | `AddValueMetadata` | Sí | Derivado |
| 13 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `GetDetailValue` | `GetMetadataValue` | Sí |  |
| 14 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `GetDetailValueAsync` | `GetMetadataValueAsync` | No |  |
| 15 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `GetDetailException` | `GetException` | No | Solo hay una excepcion por resultado |
| 16 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsErrorsDetails` | `GetDetailExceptionAsync` | `GetExceptionAsync` | No | Solo hay una excepcion por resultado |
| 17 | Tipo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers` | `MlErrorsDetailsExtensions` | `MlErrorDetailsExtensions` | No | Los detalles de los errores = ErrorDetails en ingles |
| 18 | Tipo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers` | `MlErrorsDetailsExtensionsTests` | `MlErrorDetailsExtensionsTests` | No | Los detalles de los errores = ErrorDetails en ingles |
| 19 | Tipo | Core | `MoralesLarios.OOFP.Types.Errors` | `MlErrorsDetails` | `MlErrorDetails` | No | Los detalles de los errores = ErrorDetails en ingles |
| 20 | Tipo | Core | `MoralesLarios.OOFP.Types.Errors` | `MlErrorsDetailsActions` | `MlErrorDetailsActions` | No | Los detalles de los errores = ErrorDetails en ingles |
| 21 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.Errors` | `MlErrorsDetailsActionsTests` | `MlErrorDetailsActionsTests` | No | Los detalles de los errores = ErrorDetails en ingles |
| 22 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.Errors` | `MlErrorsDetailsTests` | `MlErrorDetailsTests` | No | Los detalles de los errores = ErrorDetails en ingles |

### Nivel 2c → Familias de operadores del núcleo

<sub>Lote `fase3c-nivel2-familias` → 108 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `EmptyToFailed` | `FailIfEmpty` | No | El predicado delante, como EnsureFp.That |
| 2 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `EmptyToFailedAsync` | `FailIfEmptyAsync` | No | El predicado delante, como EnsureFp.That |
| 3 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `NullToFailed` | `FailIfNull` | No |  |
| 4 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `NullToFailedAsync` | `FailIfNullAsync` | No |  |
| 5 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `BoolToResult` | `FailIfFalse` | No |  |
| 6 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `BoolToResultAsync` | `FailIfFalseAsync` | No |  |
| 7 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `Combine` | `CombineAll` | No | Agrega TODOS los errores, no cortocircuita |
| 8 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `CombineAsync` | `CombineAllAsync` | No | Agrega TODOS los errores, no cortocircuita |
| 9 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `Do` | `Pipe` | Sí | No duplica ExecSelf: aplica una funcion al MlResult completo (Pipe) |
| 10 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultActionsSeveral` | `DoAsync` | `PipeAsync` | No | No duplica ExecSelf: aplica una funcion al MlResult completo (Pipe) |
| 11 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionSplitPlus` | `TraverseSplitWithKeys` | No | Nuevo en 2.0 (sin publicar): sin puente. Plus es un sufijo prohibido |
| 12 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionSplitPlusAsync` | `TraverseSplitWithKeysAsync` | No | Nuevo en 2.0 (sin publicar): sin puente. Plus es un sufijo prohibido |
| 13 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `Projection` | `Traverse` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 14 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionAsync` | `TraverseAsync` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 15 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionWhile` | `TraverseWhile` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 16 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionWhileAsync` | `TraverseWhileAsync` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 17 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionParallelAsync` | `TraverseParallelAsync` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 18 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionSplit` | `TraverseSplit` | Sí | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 19 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultBucles` | `ProjectionSplitAsync` | `TraverseSplitAsync` | No | Nombre estandar en programacion funcional (Projection ya significa Select en LINQ) |
| 20 | Record | Core | `MoralesLarios.OOFP.Types` | `ProyectSplitValidInfo` | `TraverseSplitSuccess` | No | Nuevo en 2.0, Proyect no es ingles e Info es sufijo prohibido |
| 21 | Record | Core | `MoralesLarios.OOFP.Types` | `ProyectSplitFailInfo` | `TraverseSplitFailure` | No |  |
| 22 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsBind` | `MlResultBindExtensions` | No | Actions es enganoso (un Action no devuelve valor), sufijo ...Extensions |
| 23 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsErrorsDetails` | `MlResultErrorDetailsExtensions` | No |  |
| 24 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsExecSelf` | `MlResultExecSelfExtensions` | No |  |
| 25 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsMap` | `MlResultMapExtensions` | No |  |
| 26 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsMatch` | `MlResultMatchExtensions` | No |  |
| 27 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActionsSeveral` | `MlResultSeveralExtensions` | No |  |
| 28 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultActions` | `MlResultExtensions` | No |  |
| 29 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultBucles` | `MlResultCollections` | No | Unico nombre de clase en espanol del nucleo |
| 30 | Tipo | Core | `MoralesLarios.OOFP.Types` | `MlResultTransformations` | `MlResultConversions` | No | Transformation solapa con Map |
| 31 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsBindMapFixesTests` | `MlResultBindMapFixesTests` | No |  |
| 32 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsExecSelfFixesTests` | `MlResultExecSelfFixesTests` | No |  |
| 33 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsErrorsDetailsTests` | `MlResultErrorDetailsExtensionsTests` | No |  |
| 34 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsBindTests` | `MlResultBindExtensionsTests` | No |  |
| 35 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsExecSelfTests` | `MlResultExecSelfExtensionsTests` | No |  |
| 36 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsMapTests` | `MlResultMapExtensionsTests` | No |  |
| 37 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsMatchTests` | `MlResultMatchExtensionsTests` | No |  |
| 38 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsSeveralTests` | `MlResultSeveralExtensionsTests` | No |  |
| 39 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultActionsTests` | `MlResultExtensionsTests` | No |  |
| 40 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultBuclesTests` | `MlResultCollectionsTests` | No |  |
| 41 | Tipo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types` | `MlResultTransformationsTests` | `MlResultConversionsTests` | No |  |
| 42 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlus_when_completeFuncTransform_isNull_return_fail` | `TraverseSplitWithKeys_when_completeFuncTransform_isNull_return_fail` | No |  |
| 43 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlus_when_sourceContainsElmentsNull_thisElements_NotEvaluated_return_valid_withoutthisElements` | `TraverseSplitWithKeys_when_sourceContainsElmentsNull_thisElements_NotEvaluated_return_valid_withoutthisElements` | No |  |
| 44 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlus_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | `TraverseSplitWithKeys_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | No |  |
| 45 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlus_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allElements` | `TraverseSplitWithKeys_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allElements` | No |  |
| 46 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_source_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | `TraverseSplitWithKeysAsync_source_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | No |  |
| 47 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_source_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allFails` | `TraverseSplitWithKeysAsync_source_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allFails` | No |  |
| 48 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_syncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | `TraverseSplitWithKeysAsync_sourceAsync_syncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | No |  |
| 49 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_asyncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | `TraverseSplitWithKeysAsync_sourceAsync_asyncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | No |  |
| 50 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_asyncTransform_when_allSourceValues_areFail_return_valid_with_allFails` | `TraverseSplitWithKeysAsync_sourceAsync_asyncTransform_when_allSourceValues_areFail_return_valid_with_allFails` | No |  |
| 51 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_when_sourceContainsElementsNull_theseElements_NotEvaluated_return_valid_withoutThoseElements` | `TraverseSplitWithKeysAsync_sourceAsync_when_sourceContainsElementsNull_theseElements_NotEvaluated_return_valid_withoutThoseElements` | No |  |
| 52 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_syncTransform_when_mixedValidAndFail_return_valid_with_splitValues` | `TraverseSplitWithKeysAsync_sourceAsync_syncTransform_when_mixedValidAndFail_return_valid_with_splitValues` | No |  |
| 53 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitPlusAsync_sourceAsync_asyncTransform_when_transformIsNull_return_fail` | `TraverseSplitWithKeysAsync_sourceAsync_asyncTransform_when_transformIsNull_return_fail` | No |  |
| 54 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat` | `Traverse_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat` | No |  |
| 55 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_When_All_completeFuncTransforms_OK_return_valid` | `Traverse_When_All_completeFuncTransforms_OK_return_valid` | No |  |
| 56 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value` | `Traverse_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value` | No |  |
| 57 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | `Traverse_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | No |  |
| 58 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_differentResult_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat` | `Traverse_differentResult_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat` | No |  |
| 59 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid` | `Traverse_differentResult_When_All_completeFuncTransforms_OK_return_valid` | No |  |
| 60 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value` | `Traverse_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value` | No |  |
| 61 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | `Traverse_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | No |  |
| 62 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionWhile_When_2_completeFuncTransformGenerateError_return_Fail_with_1_errors_Concat` | `TraverseWhile_When_2_completeFuncTransformGenerateError_return_Fail_with_1_errors_Concat` | No |  |
| 63 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionParallelAsync_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | `TraverseParallelAsync_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform` | No |  |
| 64 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionAsync_sourceAsync_syncFunc_When_All_completeFuncTransforms_OK_return_valid` | `TraverseAsync_sourceAsync_syncFunc_When_All_completeFuncTransforms_OK_return_valid` | No |  |
| 65 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionAsync_sourceAsync_syncFunc_When_completeFuncTransformGenerateError_return_Fail` | `TraverseAsync_sourceAsync_syncFunc_When_completeFuncTransformGenerateError_return_Fail` | No |  |
| 66 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionWhileAsync_sourceAsync_syncFunc_When_2_completeFuncTransformGenerateError_return_Fail_with_first_error` | `TraverseWhileAsync_sourceAsync_syncFunc_When_2_completeFuncTransformGenerateError_return_Fail_with_first_error` | No |  |
| 67 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplit_when_completeFuncTransform_isNull_return_fail` | `TraverseSplit_when_completeFuncTransform_isNull_return_fail` | No |  |
| 68 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplit_when_sourceContainsElmentsNull_thisElements_NotEvaluated_return_valid_withoutthisElements` | `TraverseSplit_when_sourceContainsElmentsNull_thisElements_NotEvaluated_return_valid_withoutthisElements` | No |  |
| 69 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplit_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | `TraverseSplit_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | No |  |
| 70 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplit_when_allSourceValues_completeFuncTransform_areFail_return_fail_with_allElements` | `TraverseSplit_when_allSourceValues_completeFuncTransform_areFail_return_fail_with_allElements` | No |  |
| 71 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_source_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | `TraverseSplitAsync_source_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements` | No |  |
| 72 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_source_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allFails` | `TraverseSplitAsync_source_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allFails` | No |  |
| 73 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_syncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | `TraverseSplitAsync_sourceAsync_syncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | No |  |
| 74 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | `TraverseSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areValid_return_valid_with_allElements` | No |  |
| 75 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areFail_return_valid_with_allFails` | `TraverseSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areFail_return_valid_with_allFails` | No |  |
| 76 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_when_sourceContainsElementsNull_theseElements_NotEvaluated_return_valid_withoutThoseElements` | `TraverseSplitAsync_sourceAsync_when_sourceContainsElementsNull_theseElements_NotEvaluated_return_valid_withoutThoseElements` | No |  |
| 77 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_syncTransform_when_mixedValidAndFail_return_valid_with_splitValues` | `TraverseSplitAsync_sourceAsync_syncTransform_when_mixedValidAndFail_return_valid_with_splitValues` | No |  |
| 78 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultCollectionsTests` | `ProjectionSplitAsync_sourceAsync_asyncTransform_when_transformIsNull_return_fail` | `TraverseSplitAsync_sourceAsync_asyncTransform_when_transformIsNull_return_fail` | No |  |
| 79 | member | Core.Tests.Unit | `*` | `^EmptyToFailed` | `FailIfEmpty` | No | SIN COINCIDENCIAS |
| 80 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `NullToFailedAsync_sourceAsync_withMlError_nullSource_return_Fail` | `FailIfNullAsync_sourceAsync_withMlError_nullSource_return_Fail` | No |  |
| 81 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `NullToFailedAsync_sourceAsync_withErrorsDetails_nonNullSource_return_Valid` | `FailIfNullAsync_sourceAsync_withErrorsDetails_nonNullSource_return_Valid` | No |  |
| 82 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `BoolToResult_falseCondition_return_Fail` | `FailIfFalse_falseCondition_return_Fail` | No |  |
| 83 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `BoolToResultAsync_sourceAsync_withMlError_falseCondition_return_Fail` | `FailIfFalseAsync_sourceAsync_withMlError_falseCondition_return_Fail` | No |  |
| 84 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `BoolToResultAsync_sourceAsync_withErrorsDetails_trueCondition_return_Valid` | `FailIfFalseAsync_sourceAsync_withErrorsDetails_trueCondition_return_Valid` | No |  |
| 85 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_2Values_OK` | `CombineAll_2Values_OK` | No |  |
| 86 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_2Values_OK` | `CombineAllAsync_2Values_OK` | No |  |
| 87 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_2Values_sourceAsync_OK` | `CombineAllAsync_2Values_sourceAsync_OK` | No |  |
| 88 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_3Values_OK` | `CombineAll_3Values_OK` | No |  |
| 89 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_3Values_OK` | `CombineAllAsync_3Values_OK` | No |  |
| 90 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_3Values_sourceAsync_OK` | `CombineAllAsync_3Values_sourceAsync_OK` | No |  |
| 91 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_4Values_OK` | `CombineAll_4Values_OK` | No |  |
| 92 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_4Values_OK` | `CombineAllAsync_4Values_OK` | No |  |
| 93 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_4Values_sourceAsync_OK` | `CombineAllAsync_4Values_sourceAsync_OK` | No |  |
| 94 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_5Values_OK` | `CombineAll_5Values_OK` | No |  |
| 95 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_5Values_OK` | `CombineAllAsync_5Values_OK` | No |  |
| 96 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_5Values_sourceAsync_OK` | `CombineAllAsync_5Values_sourceAsync_OK` | No |  |
| 97 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_reverse_2Values_OK` | `CombineAll_reverse_2Values_OK` | No |  |
| 98 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_2Values_OK` | `CombineAllAsync_reverse_2Values_OK` | No |  |
| 99 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_2Values_sourceAsync_OK` | `CombineAllAsync_reverse_2Values_sourceAsync_OK` | No |  |
| 100 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_reverse_3Values_OK` | `CombineAll_reverse_3Values_OK` | No |  |
| 101 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_3Values_OK` | `CombineAllAsync_reverse_3Values_OK` | No |  |
| 102 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_3Values_sourceAsync_OK` | `CombineAllAsync_reverse_3Values_sourceAsync_OK` | No |  |
| 103 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_reverse_4Values_OK` | `CombineAll_reverse_4Values_OK` | No |  |
| 104 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_4Values_OK` | `CombineAllAsync_reverse_4Values_OK` | No |  |
| 105 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_4Values_sourceAsync_OK` | `CombineAllAsync_reverse_4Values_sourceAsync_OK` | No |  |
| 106 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `Combine_reverse_5Values_OK` | `CombineAll_reverse_5Values_OK` | No |  |
| 107 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_5Values_OK` | `CombineAllAsync_reverse_5Values_OK` | No |  |
| 108 | Metodo | Core.Tests.Unit | `MoralesLarios.OOFP.Core.Tests.Unit.Types.MlResultSeveralExtensionsTests` | `CombineAsync_reverse_5Values_sourceAsync_OK` | `CombineAllAsync_reverse_5Values_sourceAsync_OK` | No |  |

### Nivel 3a → Factorías de los Value Objects

<sub>Lote `fase4a-nivel3-factorias` → 81 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength` | `FromStringLength` | `CreateFromLength` | No | Lanza -> Create. Sin puente: su nombre pasa a la variante MlResult. String es redundante en String*Length |
| 2 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength` | `FromStringLength` | `CreateFromLength` | No | Lanza -> Create. Sin puente: su nombre pasa a la variante MlResult. String es redundante en String*Length |
| 3 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength` | `FromStringLength` | `CreateFromLength` | No | Lanza -> Create. Sin puente: su nombre pasa a la variante MlResult. String es redundante en String*Length |
| 4 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Age` | `FromInt` | `CreateFromInt` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 5 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalBetween` | `FromDecimalLength` | `CreateFromDecimalLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 6 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalLessThan` | `FromDecimalLength` | `CreateFromDecimalLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 7 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalMoreThan` | `FromDecimalLength` | `CreateFromDecimalLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 8 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalNotNegative` | `FromDecimal` | `CreateFromDecimal` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 9 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleBetween` | `FromDoubleLength` | `CreateFromDoubleLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 10 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleLessThan` | `FromDoubleLength` | `CreateFromDoubleLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 11 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleMoreThan` | `FromDoubleLength` | `CreateFromDoubleLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 12 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleNotNegative` | `FromDouble` | `CreateFromDouble` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 13 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Endpoint` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 14 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IdLite` | `FromInt` | `CreateFromInt` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 15 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween` | `FromIntLength` | `CreateFromIntLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 16 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan` | `FromIntLength` | `CreateFromIntLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 17 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan` | `FromIntLength` | `CreateFromIntLength` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 18 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntNotNegative` | `FromInt` | `CreateFromInt` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 19 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Key` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 20 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Mail` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 21 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Name` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 22 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.NotEmptyString` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 23 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RangeEnumValueObject<TEnum>` | `FromEnum` | `CreateFromEnum` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 24 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RangeEnumValueObject<TEnum>` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 25 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RegexValue` | `FromRegex` | `CreateFromRegex` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 26 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsByte` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 27 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDecimal` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 28 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDouble` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 29 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsFloat` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 30 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsInt` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 31 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsLong` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 32 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsNumeric<TValue>` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 33 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsSByte` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 34 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsShort` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 35 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUInt` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 36 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsULong` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 37 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUShort` | `FromString` | `CreateFromString` | No | Convencion documentada: Create lanza, From devuelve MlResult. Sin puente posible |
| 38 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistDirectory` | `FromString` | `CreateFromPath` | No | Lanza -> Create. ByString describia el tipo del parametro, no la intencion |
| 39 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistsFile` | `FromString` | `CreateFromPath` | No | Lanza -> Create. ByString describia el tipo del parametro, no la intencion |
| 40 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlDirectory` | `FromString` | `CreateFromPath` | No | Lanza -> Create. ByString describia el tipo del parametro, no la intencion |
| 41 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlFile` | `FromString` | `CreateFromPath` | No | Lanza -> Create. ByString describia el tipo del parametro, no la intencion |
| 42 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength` | `ByStringLength` | `FromLength` | Sí | By sugiere ordenacion (OrderBy), From = conversion que devuelve MlResult |
| 43 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength` | `ByStringLength` | `FromLength` | Sí | By sugiere ordenacion (OrderBy), From = conversion que devuelve MlResult |
| 44 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength` | `ByStringLength` | `FromLength` | Sí | By sugiere ordenacion (OrderBy), From = conversion que devuelve MlResult |
| 45 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Age` | `ByInt` | `FromInt` | Sí |  |
| 46 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalBetween` | `ByDecimalLength` | `FromDecimalLength` | Sí |  |
| 47 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalLessThan` | `ByDecimalLength` | `FromDecimalLength` | Sí |  |
| 48 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalMoreThan` | `ByDecimalLength` | `FromDecimalLength` | Sí |  |
| 49 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalNotNegative` | `ByDecimal` | `FromDecimal` | Sí |  |
| 50 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleBetween` | `ByDoubleLength` | `FromDoubleLength` | Sí |  |
| 51 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleLessThan` | `ByDoubleLength` | `FromDoubleLength` | Sí |  |
| 52 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleMoreThan` | `ByDoubleLength` | `FromDoubleLength` | Sí |  |
| 53 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleNotNegative` | `ByDouble` | `FromDouble` | Sí |  |
| 54 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Endpoint` | `ByString` | `FromString` | Sí |  |
| 55 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IdLite` | `ByInt` | `FromInt` | Sí |  |
| 56 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween` | `ByIntLength` | `FromIntLength` | Sí |  |
| 57 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan` | `ByIntLength` | `FromIntLength` | Sí |  |
| 58 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan` | `ByIntLength` | `FromIntLength` | Sí |  |
| 59 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntNotNegative` | `ByInt` | `FromInt` | Sí |  |
| 60 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Key` | `ByString` | `FromString` | Sí |  |
| 61 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Mail` | `ByString` | `FromString` | Sí |  |
| 62 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Name` | `ByString` | `FromString` | Sí |  |
| 63 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.NotEmptyString` | `ByString` | `FromString` | Sí |  |
| 64 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RangeEnumValueObject<TEnum>` | `ByString` | `FromString` | Sí |  |
| 65 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RegexValue` | `ByRegex` | `FromRegex` | Sí |  |
| 66 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsByte` | `ByString` | `FromString` | Sí |  |
| 67 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDecimal` | `ByString` | `FromString` | Sí |  |
| 68 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDouble` | `ByString` | `FromString` | Sí |  |
| 69 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsFloat` | `ByString` | `FromString` | Sí |  |
| 70 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsInt` | `ByString` | `FromString` | Sí |  |
| 71 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsLong` | `ByString` | `FromString` | Sí |  |
| 72 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsNumeric<TValue>` | `ByString` | `FromString` | Sí |  |
| 73 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsSByte` | `ByString` | `FromString` | Sí |  |
| 74 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsShort` | `ByString` | `FromString` | Sí |  |
| 75 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUInt` | `ByString` | `FromString` | Sí |  |
| 76 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsULong` | `ByString` | `FromString` | Sí |  |
| 77 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUShort` | `ByString` | `FromString` | Sí |  |
| 78 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistDirectory` | `ByString` | `FromPath` | Sí |  |
| 79 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistsFile` | `ByString` | `FromPath` | Sí |  |
| 80 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlDirectory` | `ByString` | `FromPath` | Sí |  |
| 81 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlFile` | `ByString` | `FromPath` | Sí |  |

### Nivel 3b → Tipos de los Value Objects

<sub>Lote `fase4b-nivel3-valueobjects` → 88 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Propiedad | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalNotNegative` | `limit` | `MinAllowedValue` | Sí | camelCase siendo publico. Hacerlo private const es cambio de contrato (Fase 12) |
| 2 | Propiedad | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleNotNegative` | `limit` | `MinAllowedValue` | Sí | camelCase siendo publico. Hacerlo private const es cambio de contrato (Fase 12) |
| 3 | Constante | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlDirectory` | `EndpointPattern` | `SearchPattern` | No | Endpoint es vocabulario HTTP, es el patron de Directory.GetFiles |
| 4 | Constante | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlFile` | `EndpointPattern` | `SearchPattern` | No | Endpoint es vocabulario HTTP, es el patron de Directory.GetFiles |
| 5 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Age` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 6 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalBetween` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 7 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalLessThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 8 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalMoreThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 9 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DecimalNotNegative` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 10 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleBetween` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 11 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleLessThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 12 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleMoreThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 13 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.DoubleNotNegative` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 14 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Endpoint` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 15 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 16 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntLessThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 17 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntMoreThan` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 18 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntNotNegative` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 19 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Key` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 20 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Mail` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 21 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Name` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 22 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.NotEmptyString` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 23 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RangeEnumValueObject<TEnum>` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 24 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.RegexValue` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 25 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsByte` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 26 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDecimal` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 27 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsDouble` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 28 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsFloat` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 29 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsInt` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 30 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsLong` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 31 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsNumeric<TValue>` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 32 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsSByte` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 33 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsShort` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 34 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUInt` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 35 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsULong` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 36 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringAsUShort` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 37 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringBetweenLength` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 38 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMaxLength` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 39 | Metodo | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringMinLength` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena (Build sugiere un builder). Sin puente: 43 tipos con ocultacion de miembros |
| 40 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistDirectory` | `BuildErrorMessage` | `FormatErrorMessage` | No |  |
| 41 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.ExistsFile` | `BuildErrorMessage` | `FormatErrorMessage` | No |  |
| 42 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlDirectory` | `BuildErrorMessage` | `FormatErrorMessage` | No |  |
| 43 | Metodo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem.MlFile` | `BuildErrorMessage` | `FormatErrorMessage` | No |  |
| 44 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `Key` | `ServiceKey` | No | Clave de registro con nombre en DI, Key a secas colisiona con KeyValuePair.Key, claves de diccionario y PK |
| 45 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `Name` | `MemberName` | No | Name es de las palabras mas colisionadas de .NET |
| 46 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `Empty` | `EmptyValue` | No | Empty x se lee como error, colisiona con string.Empty, Array.Empty... |
| 47 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `DecimalNotNegative` | `DecimalNonNegative` | No | No negativo (>= 0) es NonNegative en ingles tecnico |
| 48 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `DoubleNotNegative` | `DoubleNonNegative` | No | No negativo (>= 0) es NonNegative en ingles tecnico |
| 49 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `IntNotNegative` | `IntNonNegative` | No | No negativo (>= 0) es NonNegative en ingles tecnico |
| 50 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `RangeEnumValueObject` | `EnumRangeValueObject` | No |  |
| 51 | Tipo | ValueObjects | `MoralesLarios.OOFP.ValueObjects` | `StringBetweenLength` | `StringLengthBetween` | No | Longitud de cadena entre X e Y |
| 52 | Tipo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem` | `ExistsFile` | `ExistingFilePath` | No | ExistsFile parecia un metodo bool (File.Exists), es un dato con invariante |
| 53 | Tipo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem` | `ExistDirectory` | `ExistingDirectoryPath` | No | Ademas corrige la conjugacion (Exist -> Exists) |
| 54 | Tipo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem` | `MlFile` | `FilePath` | No | Envuelve la ruta, no el fichero |
| 55 | Tipo | ValueObjects.FileSystem | `MoralesLarios.OOFP.ValueObjects.FileSystem` | `MlDirectory` | `DirectoryPath` | No |  |
| 56 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `KeyTests` | `ServiceKeyTests` | No |  |
| 57 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `NameTests` | `MemberNameTests` | No |  |
| 58 | type | ValueObjects.Tests.Unit | `*` | `EmptyTests` | `EmptyValueTests` | No | SIN COINCIDENCIAS |
| 59 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `IntNotNegativeTests` | `IntNonNegativeTests` | No |  |
| 60 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `DecimalNotNegativeTests` | `DecimalNonNegativeTests` | No |  |
| 61 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `DoubleNotNegativeTests` | `DoubleNonNegativeTests` | No |  |
| 62 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `RangeEnumValueObjectTests` | `EnumRangeValueObjectTests` | No |  |
| 63 | Tipo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | `StringBetweenLengthTests` | `StringLengthBetweenTests` | No |  |
| 64 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntNonNegativeTests` | `IntNotNegative_should_not_expose_a_mutable_static_limit_field` | `IntNonNegative_should_not_expose_a_mutable_static_limit_field` | No |  |
| 65 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_should_accept_min_and_max_boundaries` | `StringLengthBetween_should_accept_min_and_max_boundaries` | No |  |
| 66 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_error_message_should_print_min_before_max` | `StringLengthBetween_error_message_should_print_min_before_max` | No |  |
| 67 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_ValueBetweenLimits_ReturnsValid` | `StringLengthBetween_ValueBetweenLimits_ReturnsValid` | No |  |
| 68 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_EmptyValue_ReturnsFail` | `StringLengthBetween_EmptyValue_ReturnsFail` | No |  |
| 69 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_ValueLessThanMinLength_ReturnsFail` | `StringLengthBetween_ValueLessThanMinLength_ReturnsFail` | No |  |
| 70 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.StringLengthBetweenTests` | `StringBetweenLength_ValueMoreThanMaxLength_ReturnsFail` | `StringLengthBetween_ValueMoreThanMaxLength_ReturnsFail` | No |  |
| 71 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.EnumRangeValueObjectTests` | `RangeEnumValueObject_ByString_should_accept_valid_enum_value` | `EnumRangeValueObject_ByString_should_accept_valid_enum_value` | No |  |
| 72 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.EnumRangeValueObjectTests` | `RangeEnumValueObject_ByString_should_reject_invalid_enum_value` | `EnumRangeValueObject_ByString_should_reject_invalid_enum_value` | No |  |
| 73 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.EnumRangeValueObjectTests` | `RangeEnumValueObject_FromEnum_should_convert_back_to_enum` | `EnumRangeValueObject_FromEnum_should_convert_back_to_enum` | No |  |
| 74 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.ServiceKeyTests` | `Key_ValueWithOneChar_ReturnsValid` | `ServiceKey_ValueWithOneChar_ReturnsValid` | No |  |
| 75 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.ServiceKeyTests` | `Key_EmptyValue_ReturnsFail` | `ServiceKey_EmptyValue_ReturnsFail` | No |  |
| 76 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.ServiceKeyTests` | `Key_ValueLongerStillValid` | `ServiceKey_ValueLongerStillValid` | No |  |
| 77 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.ServiceKeyTests` | `Key_IsValid_should_use_the_received_length` | `ServiceKey_IsValid_should_use_the_received_length` | No |  |
| 78 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.ServiceKeyTests` | `Key_should_not_expose_public_constructors` | `ServiceKey_should_not_expose_public_constructors` | No |  |
| 79 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.MemberNameTests` | `Name_ValueLessThanLength_ReturnsValid` | `MemberName_ValueLessThanLength_ReturnsValid` | No |  |
| 80 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.MemberNameTests` | `Name_EmptyValue_ReturnsFail` | `MemberName_EmptyValue_ReturnsFail` | No |  |
| 81 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.MemberNameTests` | `Name_ValueLessThanLength_ReturnsFail` | `MemberName_ValueLessThanLength_ReturnsFail` | No |  |
| 82 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.MemberNameTests` | `Name_IsValid_should_use_the_received_length` | `MemberName_IsValid_should_use_the_received_length` | No |  |
| 83 | Metodo | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.MemberNameTests` | `Name_should_not_expose_public_constructors` | `MemberName_should_not_expose_public_constructors` | No |  |
| 84 | member | ValueObjects.Tests.Unit | `*` | `^Empty_` | `EmptyValue_` | No | SIN COINCIDENCIAS |
| 85 | Tipo | ValueObjects.FileSystem.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit` | `ExistsFileTests` | `ExistingFilePathTests` | No |  |
| 86 | Tipo | ValueObjects.FileSystem.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit` | `ExistDirectoryTests` | `ExistingDirectoryPathTests` | No |  |
| 87 | member | ValueObjects.FileSystem.Tests.Unit | `*` | `^ExistsFile` | `ExistingFilePath` | No | SIN COINCIDENCIAS |
| 88 | member | ValueObjects.FileSystem.Tests.Unit | `*` | `^ExistDirectory` | `ExistingDirectoryPath` | No | SIN COINCIDENCIAS |

### Nivel 3c → Patrones de los Value Objects

<sub>Lote `fase4c-nivel3-patrones` → 2 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Constante | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Endpoint` | `EndpointPattern` | `ValidationPattern` | Sí | Derivado: es la regex de validacion |
| 2 | Constante | ValueObjects | `MoralesLarios.OOFP.ValueObjects.Mail` | `EndpointPattern` | `ValidationPattern` | Sí | Derivado: nombre copiado de Endpoint, es la regex de email |

### Niveles 4 y 5 → Validation, Shared, Configuration, FileSystem y Logging

<sub>Lote `fase5-6-niveles4-5` → 43 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Tipo | Validation | `MoralesLarios.OOFP.Validation` | `MlValidableFp` | `MlSelfValidatable` | No | Validatable (no Validable), Fp sobraba (no hay version no-Fp). Pasar a interfaz = cambio de contrato |
| 2 | Tipo | Validation.DataAnnotations | `MoralesLarios.OOFP.Validation.DataAnnotations` | `DataannotationsValidator` | `DataAnnotationsValidator` | No |  |
| 3 | Metodo | Validation.DataAnnotations | `MoralesLarios.OOFP.Validation.DataAnnotations.Helpers.Extensions` | `ValidateObject` | `ValidateDataAnnotations` | Sí | Colisionaba con ValidateObject del nucleo (CS0121) |
| 4 | Metodo | Validation.DataAnnotations | `MoralesLarios.OOFP.Validation.DataAnnotations.Helpers.Extensions` | `ValidateWithDataannotations` | `ValidateWithDataAnnotations` | No | Derivado: DataAnnotations con A mayuscula |
| 5 | Metodo | Validation.DataAnnotations | `MoralesLarios.OOFP.Validation.DataAnnotations.Helpers.Extensions` | `ValidateWithDataannotationsAsync` | `ValidateWithDataAnnotationsAsync` | No | Derivado: DataAnnotations con A mayuscula |
| 6 | Tipo | Validation.FluentValidation | `MoralesLarios.OOFP.Validation.FluentValidation` | `FluentValidationsValidator` | `FluentValidationValidator` | No | La biblioteca se llama FluentValidation, en singular |
| 7 | Metodo | Validation.FluentValidation | `MoralesLarios.OOFP.Validation.FluentValidation.Helpers.Extensions` | `ValidateWithFluentValidations` | `ValidateWithFluentValidation` | No |  |
| 8 | Metodo | Validation.FluentValidation | `MoralesLarios.OOFP.Validation.FluentValidation.Helpers.Extensions` | `ValidateWithFluentValidationsAsync` | `ValidateWithFluentValidationAsync` | No |  |
| 9 | Metodo | Core | `MoralesLarios.OOFP.Types.MlResultConversions` | `BuildErrorMessage` | `FormatErrorMessage` | No | Solo formatea una cadena |
| 10 | Tipo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit` | `DataannotationsValidatorTests` | `DataAnnotationsValidatorTests` | No |  |
| 11 | type | Validation.FluentValidation.Tests.Unit | `*` | `^FluentValidations` | `FluentValidation` | No | SIN COINCIDENCIAS |
| 12 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotations_objectValid_return_valid` | `ValidateWithDataAnnotations_objectValid_return_valid` | No |  |
| 13 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotations_objectFail_return_fail` | `ValidateWithDataAnnotations_objectFail_return_fail` | No |  |
| 14 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotationsAsync_colec_AllValid_return_valid` | `ValidateWithDataAnnotationsAsync_colec_AllValid_return_valid` | No |  |
| 15 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotationsAsync_colec_AllFail_return_Fail` | `ValidateWithDataAnnotationsAsync_colec_AllFail_return_Fail` | No |  |
| 16 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotationsAsync_colec_AllFail_return_2_errors` | `ValidateWithDataAnnotationsAsync_colec_AllFail_return_2_errors` | No |  |
| 17 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotationsAsync_colec_fail_1_element_return_Fail` | `ValidateWithDataAnnotationsAsync_colec_fail_1_element_return_Fail` | No |  |
| 18 | Metodo | Validation.DataAnnotations.Tests.Unit | `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit.Helpers.ExtensionsTests` | `ValidateWithDataannotationsAsync_colec_fail_1_element_return_1_errors` | `ValidateWithDataAnnotationsAsync_colec_fail_1_element_return_1_errors` | No |  |
| 19 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_objectValid_return_valid` | `ValidateWithFluentValidation_objectValid_return_valid` | No |  |
| 20 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_objectFail_return_fail` | `ValidateWithFluentValidation_objectFail_return_fail` | No |  |
| 21 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_collectionValid_return_valid` | `ValidateWithFluentValidation_collectionValid_return_valid` | No |  |
| 22 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_collectionWithInvalidItem_return_fail` | `ValidateWithFluentValidation_collectionWithInvalidItem_return_fail` | No |  |
| 23 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_collectionWithTwoInvalidItems_returnsTwoErrors` | `ValidateWithFluentValidation_collectionWithTwoInvalidItems_returnsTwoErrors` | No |  |
| 24 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidations_collectionWithOneInvalidItem_returnsOneError` | `ValidateWithFluentValidation_collectionWithOneInvalidItem_returnsOneError` | No |  |
| 25 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidationsAsync_taskObject_return_valid` | `ValidateWithFluentValidationAsync_taskObject_return_valid` | No |  |
| 26 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `ValidateWithFluentValidationsAsync_taskCollection_return_fail` | `ValidateWithFluentValidationAsync_taskCollection_return_fail` | No |  |
| 27 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_nullObject_return_fail` | `FluentValidationValidator_nullObject_return_fail` | No |  |
| 28 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_emptyCollection_return_fail` | `FluentValidationValidator_emptyCollection_return_fail` | No |  |
| 29 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_asyncDirectObject_return_fail` | `FluentValidationValidator_asyncDirectObject_return_fail` | No |  |
| 30 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_asyncDirectCollection_return_valid` | `FluentValidationValidator_asyncDirectCollection_return_valid` | No |  |
| 31 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_asyncObject_return_valid` | `FluentValidationValidator_asyncObject_return_valid` | No |  |
| 32 | Metodo | Validation.FluentValidation.Tests.Unit | `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit.ExtensionsTests` | `FluentValidationsValidator_asyncCollectionWithInvalidItem_return_fail` | `FluentValidationValidator_asyncCollectionWithInvalidItem_return_fail` | No |  |
| 33 | Record | Shared | `MoralesLarios.OOFP.Shared.Pagination` | `PaginationInfo` | `PageRequest` | No | Lo que pide el cliente, Info no distingue nada |
| 34 | Record | Shared | `MoralesLarios.OOFP.Shared.Pagination` | `PaginationResultInfo` | `PagedResult` | No | Nombre de facto en .NET, pareja de PageRequest |
| 35 | Interfaz | Configuration | `MoralesLarios.OOFP.Configuration.Config` | `IMlConfigManager` | `IMlConfigurationReader` | No | Manager es sufijo vacio, la interfaz solo lee |
| 36 | Tipo | Configuration | `MoralesLarios.OOFP.Configuration.Config` | `MlConfigManager` | `MlConfigurationReader` | No | Derivado |
| 37 | Metodo | Configuration | `MoralesLarios.OOFP.Configuration.RegisterServices` | `AddMlUtilitiesConfig` | `AddMlConfigurationReader` | Sí | Derivado: el proyecto ya no se llama Utilities |
| 38 | Interfaz | FileSystem | `MoralesLarios.OOFP.FileSystem` | `IWrapperIO` | `IMlFileSystem` | No | Derivado: IO -> FileSystem, Wrapper es sufijo vacio |
| 39 | Tipo | FileSystem | `MoralesLarios.OOFP.FileSystem` | `WrapperIO` | `MlFileSystem` | No |  |
| 40 | Metodo | Logging | `MoralesLarios.OOFP.Logging.GeneralExtensionLoggers` | `LogMlResultFinal` | `LogCompletedResult` | No | Final no decia nada al llamador |
| 41 | Metodo | Logging | `MoralesLarios.OOFP.Logging.GeneralExtensionLoggers` | `LogMlResultFinalAsync` | `LogCompletedResultAsync` | No | Final no decia nada al llamador |
| 42 | Tipo | Configuration.Tests.Unit | `MoralesLarios.OOFP.Configuration.Tests.Unit.Config` | `MlConfigManagerTests` | `MlConfigurationReaderTests` | No |  |
| 43 | member | Configuration.Tests.Unit | `*` | `ConfigManager` | `ConfigurationReader` | No | SIN COINCIDENCIAS |

### Nivel 6 → Repositorios de EntityFrameworkCore

<sub>Lote `fase7-nivel6-efcore` → 100 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.RegisterServices` | `ResolveRepoFp` | `GetRepository` | Sí | Resolve es vocabulario interno del contenedor |
| 2 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.RegisterServices` | `AddTransientOOFPRepos` | `AddOofpRepositoriesTransient` | Sí | Siglas en PascalCase, ciclo de vida al final, Repos -> Repositories |
| 3 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.RegisterServices` | `AddScopedOOFPRepos` | `AddOofpRepositoriesScoped` | Sí | Siglas en PascalCase, ciclo de vida al final, Repos -> Repositories |
| 4 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.RegisterServices` | `AddSingletonOOFPRepos` | `AddOofpRepositoriesSingleton` | Sí | Siglas en PascalCase, ciclo de vida al final, Repos -> Repositories |
| 5 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>` | `TryLast` | `TryGetLast` | No | Last a secas parece la propiedad de una coleccion |
| 6 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>` | `TryLastAsync` | `TryGetLastAsync` | No | Last a secas parece la propiedad de una coleccion |
| 7 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoReaderPaginationFp<T, TContext>` | `TryGetInternalData` | `TryQueryPage` | No | Privado: describe la operacion, no su papel interno |
| 8 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoDeleterFp<T, TContext>.TryRemove(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 9 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoDeleterFp<T, TContext>.TryRemove(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 10 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoDeleterFp<T, TContext>.TryRemoveAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 11 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoDeleterFp<T, TContext>.TryRemoveAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 12 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryFind(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 13 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryFindAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 14 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryFind(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 15 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryFindAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 16 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryRemove(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 17 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryRemove(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 18 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryRemoveAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 19 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryRemoveAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 20 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryUpdate(T, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 21 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryUpdate(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 22 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryUpdateAsync(T, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 23 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoFp<T, TContext>.TryUpdateAsync(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 24 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryFind(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 25 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryFindAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 26 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryFind(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 27 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryFindAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 28 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryRemove(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 29 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryRemove(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 30 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryRemoveAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 31 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryRemoveAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 32 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryUpdate(T, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 33 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryUpdate(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 34 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryUpdateAsync(T, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 35 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoPaginationFp<T, TContext>.TryUpdateAsync(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 36 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoReaderFp<T, TContext>.TryFind(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 37 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoReaderFp<T, TContext>.TryFind(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 38 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoReaderFp<T, TContext>.TryFindAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 39 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoReaderFp<T, TContext>.TryFindAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 40 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoUpdaterFp<T, TContext>.TryUpdate(T, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 41 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoUpdaterFp<T, TContext>.TryUpdate(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 42 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoUpdaterFp<T, TContext>.TryUpdateAsync(T, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 43 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoUpdaterFp<T, TContext>.TryUpdateAsync(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 44 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryRemove(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 45 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryRemove(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 46 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryRemoveAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 47 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryRemoveAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 48 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryUpdate(T, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 49 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryUpdate(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 50 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryUpdateAsync(T, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 51 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.EFRepoWriterFp<T, TContext>.TryUpdateAsync(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 52 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoDeleterFp<T>.TryRemove(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 53 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoDeleterFp<T>.TryRemove(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 54 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoDeleterFp<T>.TryRemoveAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 55 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoDeleterFp<T>.TryRemoveAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 56 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoReaderFp<T>.TryFind(params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 57 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoReaderFp<T>.TryFindAsync(System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 58 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoReaderFp<T>.TryFind(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 59 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoReaderFp<T>.TryFindAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 60 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoUpdaterFp<T>.TryUpdate(T, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 61 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoUpdaterFp<T>.TryUpdate(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 62 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoUpdaterFp<T>.TryUpdateAsync(T, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 63 | Parametro | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.IEFRepoUpdaterFp<T>.TryUpdateAsync(T, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No | params object[]: pk en singular para un array de valores |
| 64 | Enum | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Enums` | `OrderBy` | `SortDirection` | No | Colisionaba con Queryable.OrderBy, sus valores son una direccion |
| 65 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoBase` | `ThrowingEfRepository` | No | Lo que distingue esta familia es que LANZA excepciones |
| 66 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoBaseFp` | `EfRepository` | No | La familia principal (MlResult) lleva el nombre sin sufijo |
| 67 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoReaderPaginationFp` | `IEfPagedReadRepository` | No |  |
| 68 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoReaderPaginationFp` | `EfPagedReadRepository` | No |  |
| 69 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoReaderFp` | `IEfReadRepository` | No | El verbo delante: se lee la intencion |
| 70 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoReaderFp` | `EfReadRepository` | No |  |
| 71 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoAdderFp` | `IEfCreateRepository` | No |  |
| 72 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoAdderFp` | `EfCreateRepository` | No |  |
| 73 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoUpdaterFp` | `IEfUpdateRepository` | No |  |
| 74 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoUpdaterFp` | `EfUpdateRepository` | No |  |
| 75 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoDeleterFp` | `IEfDeleteRepository` | No |  |
| 76 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoDeleterFp` | `EfDeleteRepository` | No |  |
| 77 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoWriterFp` | `IEfWriteRepository` | No | No esta vacia: agrega Create+Update+Delete. Se renombra en vez de borrarse |
| 78 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoWriterFp` | `EfWriteRepository` | No |  |
| 79 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoPaginationFp` | `IEfPagedRepository` | No |  |
| 80 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoPaginationFp` | `EfPagedRepository` | No |  |
| 81 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFPaginatorFp` | `IEfPaginator` | No |  |
| 82 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `IEFRepoFp` | `IEfRepository` | No |  |
| 83 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` | `EFRepoFp` | `EfCrudRepository` | No | EfRepository ya es la base, esta es la implementacion CRUD completa |
| 84 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoReaderPagination` | `IThrowingEfPagedReadRepository` | No |  |
| 85 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoReaderPagination` | `ThrowingEfPagedReadRepository` | No |  |
| 86 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoReader` | `IThrowingEfReadRepository` | No |  |
| 87 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoReader` | `ThrowingEfReadRepository` | No |  |
| 88 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoAdder` | `IThrowingEfCreateRepository` | No |  |
| 89 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoAdder` | `ThrowingEfCreateRepository` | No |  |
| 90 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoUpdater` | `IThrowingEfUpdateRepository` | No |  |
| 91 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoUpdater` | `ThrowingEfUpdateRepository` | No |  |
| 92 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoDeleter` | `IThrowingEfDeleteRepository` | No |  |
| 93 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoDeleter` | `ThrowingEfDeleteRepository` | No |  |
| 94 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoWriter` | `IThrowingEfWriteRepository` | No |  |
| 95 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoWriter` | `ThrowingEfWriteRepository` | No |  |
| 96 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepoPagination` | `IThrowingEfPagedRepository` | No |  |
| 97 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepoPagination` | `ThrowingEfPagedRepository` | No |  |
| 98 | Interfaz | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `IEFRepo` | `IThrowingEfRepository` | No |  |
| 99 | Tipo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative` | `EFRepo` | `ThrowingEfCrudRepository` | No |  |
| 100 | Tipo | EntityFrameworkCore.TestSupport | `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` | `JfCatasDbContext` | `TestDbContext` | No | Siglas de un proyecto concreto del autor |

### Nivel 6b → Registro de servicios de EntityFrameworkCore

<sub>Lote `fase7b-nivel6-registro` → 1 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.RegisterServices` | `AddOofpRepositoriesScoped` | `AddOofpRepositories` | No | Scoped es el ciclo de vida por defecto: sin sufijo (el puente de AddScopedOOFPRepos se actualiza solo) |

### Niveles 7, 8 y 9 → Application, AspNetCore.Results y AspNetCore.Controllers

<sub>Lote `fase8-10-niveles7-9` → 105 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>` | `UpdateProblemDetailsAsync` | `UpdateAsync` | No | El formato del error no debe aparecer en el nombre del caso de uso |
| 2 | Metodo | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>` | `UpdateProblemDetailsAsync` | `UpdateAsync` | No | El formato del error no debe aparecer en el nombre del caso de uso |
| 3 | Metodo | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>` | `AllAsync` | `GetPagedAsync` | No | Colisionaba con Queryable.AllAsync (bool) y prometia 'todo' |
| 4 | Metodo | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>` | `AllAsync` | `GetPagedAsync` | No | Colisionaba con Queryable.AllAsync (bool) y prometia 'todo' |
| 5 | Metodo | Application | `MoralesLarios.OOFP.Application.Helpers.Extensions` | `BuildNotFoundPkError` | `BuildEntityNotFoundError` | Sí | El error es 'no existe la entidad', no 'sobre la clave' |
| 6 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddTransientGenServicesFpWithoutReposGeneral` | `AddTransientCrudServicesFpWithoutRepositories` | Sí | Derivado: mismo patron que la variante Scoped (Fase 2) |
| 7 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddSingletonGenServicesFpWithoutReposGeneral` | `AddSingletonCrudServicesFpWithoutRepositories` | Sí | Derivado: mismo patron que la variante Scoped (Fase 2) |
| 8 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddTransientGenServicesDuplexFpWithoutReposGeneral` | `AddTransientCrudServicesDuplexFpWithoutRepositories` | Sí | Derivado: mismo patron que la variante Scoped (Fase 2) |
| 9 | Metodo | Application | `MoralesLarios.OOFP.Application.RegisterServices` | `AddSingletonGenServicesDuplexFpWithoutReposGeneral` | `AddSingletonCrudServicesDuplexFpWithoutRepositories` | Sí | Derivado: mismo patron que la variante Scoped (Fase 2) |
| 10 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.GetPagedAsync(System.Threading.CancellationToken, string, System.Func<System.Collections.Generic.IEnumerable<TDto>, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 11 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.FindByIdAsync(System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 12 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.FindByIdProblemsDetailsAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 13 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.CreateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 14 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.UpdateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 15 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.UpdateAsync(TDto, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 16 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TDto>.UpdateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 17 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.GetPagedAsync(System.Threading.CancellationToken, string, System.Func<System.Collections.Generic.IEnumerable<TResponse>, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 18 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.FindByIdAsync(System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 19 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.FindByIdProblemsDetailsAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 20 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.CreateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 21 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 22 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 23 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.GenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 24 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.GetPagedAsync(System.Threading.CancellationToken, string, System.Func<System.Collections.Generic.IEnumerable<TDto>, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 25 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.FindByIdProblemsDetailsAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 26 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.FindByIdAsync(System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 27 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.CreateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 28 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.UpdateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 29 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.UpdateAsync(TDto, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 30 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TDto>.UpdateAsync(TDto, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TDto, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 31 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.GetPagedAsync(System.Threading.CancellationToken, string, System.Func<System.Collections.Generic.IEnumerable<TResponse>, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 32 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.CreateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 33 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.FindByIdAsync(System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 34 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.FindByIdProblemsDetailsAsync(MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 35 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 36 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>)` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 37 | Parametro | Application | `MoralesLarios.OOFP.Application.Services.IGenServiceFp<TEntity, TRequest, TResponse>.UpdateAsync(TRequest, MoralesLarios.OOFP.Types.Errors.MlErrorDetails, System.Threading.CancellationToken, string, System.Func<TResponse, string>, System.Func<MoralesLarios.OOFP.Types.Errors.MlErrorDetails, string>, params object[])` | `validMessageBuilder` | `successMessageFactory` | No | Coherencia con IsSuccess, Factory describe un delegado que produce un valor |
| 38 | Interfaz | Application | `MoralesLarios.OOFP.Application.Services` | `IGenServiceFp` | `ICrudServiceFp` | No | Gen no se usa en .NET, Crud dice lo que ofrece |
| 39 | Tipo | Application | `MoralesLarios.OOFP.Application.Services` | `GenServiceFp` | `CrudServiceFp` | No |  |
| 40 | Tipo | Application | `MoralesLarios.OOFP.Application` | `MlProblemsDetails` | `MlProblemDetails` | No | RFC 7807 / ASP.NET Core: ProblemDetails |
| 41 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPostPdActionResult` | `ToCreatedProblemDetailsResult` | No | Pd era abreviatura inventada, Post describia el verbo, no el resultado (201) |
| 42 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPostPdActionResultAsync` | `ToCreatedProblemDetailsResultAsync` | No | Pd era abreviatura inventada, Post describia el verbo, no el resultado (201) |
| 43 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToGetPdActionResult` | `ToOkProblemDetailsResult` | Sí | Derivado: 200 OK |
| 44 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToGetPdActionResultAsync` | `ToOkProblemDetailsResultAsync` | No | Derivado: 200 OK |
| 45 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPutPdActionResult` | `ToNoContentProblemDetailsResult` | Sí | Derivado: 204 |
| 46 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPutPdActionResultAsync` | `ToNoContentProblemDetailsResultAsync` | No | Derivado: 204 |
| 47 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPatchPdActionResult` | `ToPatchedProblemDetailsResult` | Sí | Derivado: 204 (mismo codigo que Put/Delete, el nombre conserva el verbo para no colisionar) |
| 48 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToPatchPdActionResultAsync` | `ToPatchedProblemDetailsResultAsync` | No | Derivado: 204 (mismo codigo que Put/Delete, el nombre conserva el verbo para no colisionar) |
| 49 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToDeletePdActionResult` | `ToDeletedProblemDetailsResult` | Sí | Derivado |
| 50 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensionsPlus` | `ToDeletePdActionResultAsync` | `ToDeletedProblemDetailsResultAsync` | No | Derivado |
| 51 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensions` | `ToSimpleRepoPostActionResult` | `ToCreatedResult` | Sí | Mezclaba simple, repo y post |
| 52 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensions` | `ToSimpleRepoPostActionResultAsync` | `ToCreatedResultAsync` | No | Mezclaba simple, repo y post |
| 53 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlErrorDetailsExtensions` | `ToProblemsDetailsInfo` | `ToProblemDetails` | Sí |  |
| 54 | Metodo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultWebExtensions` | `ContieneCombinacion` | `ContainsAnyKey` | No | Unico identificador en espanol del codigo de produccion |
| 55 | Tipo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers` | `MlResultWebExtensions` | `MlResultActionResultExtensionsLegacy` | No | Ya era [Obsolete]: se conserva con sufijo Legacy para liberar el nombre |
| 56 | Tipo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers` | `MlResultWebExtensionsPlus` | `MlResultActionResultExtensions` | No | Plus solo significaba 'el nombre bueno estaba ocupado' |
| 57 | Tipo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers` | `MlRequestWebExtensions` | `HttpRequestPaginationExtensions` | No |  |
| 58 | Tipo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results` | `MlActionResults` | `MlResults` | No | Paralelo a Results/TypedResults de ASP.NET Core |
| 59 | Record | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Data` | `ProblemDetailsInfo` | `ProblemDetailsData` | No |  |
| 60 | Tipo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers` | `MlResultWebExtensionsTests` | `MlResultActionResultExtensionsLegacyTests` | No |  |
| 61 | Tipo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers` | `MlResultWebExtensionsPlusTests` | `MlResultActionResultExtensionsTests` | No |  |
| 62 | Tipo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers` | `MlRequestWebExtensionsTests` | `HttpRequestPaginationExtensionsTests` | No |  |
| 63 | member | AspNetCore.Results.Tests.Unit | `*` | `^ToPostPdActionResult` | `ToCreatedProblemDetailsResult` | No | SIN COINCIDENCIAS |
| 64 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResult_Should_Return_Ok_When_MlResult_Is_Valid` | `ToOkProblemDetailsResult_Should_Return_Ok_When_MlResult_Is_Valid` | No |  |
| 65 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResult_When_source_is_fail_without_problemDetailsKey_returns_InternalServerError` | `ToOkProblemDetailsResult_When_source_is_fail_without_problemDetailsKey_returns_InternalServerError` | No |  |
| 66 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResult_WhenServiceBuildsNotFoundPkError_returnsNotFound` | `ToOkProblemDetailsResult_WhenServiceBuildsNotFoundPkError_returnsNotFound` | No |  |
| 67 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResultToRepoGetPdActionResult_When_source_is_fail_with_problemDetailsKey_returns_ProblemDetails` | `ToOkProblemDetailsResultToRepoGetPdActionResult_When_source_is_fail_with_problemDetailsKey_returns_ProblemDetails` | No |  |
| 68 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResultAsync_From_MlResult_Should_Return_Ok_When_Valid` | `ToOkProblemDetailsResultAsync_From_MlResult_Should_Return_Ok_When_Valid` | No |  |
| 69 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToGetPdActionResultAsync_From_Task_Should_Return_Ok_When_Valid` | `ToOkProblemDetailsResultAsync_From_Task_Should_Return_Ok_When_Valid` | No |  |
| 70 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPutPdActionResult_Should_Return_NoContent_When_Valid` | `ToNoContentProblemDetailsResult_Should_Return_NoContent_When_Valid` | No |  |
| 71 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPutPdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails` | `ToNoContentProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails` | No |  |
| 72 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPutPdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | `ToNoContentProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | No |  |
| 73 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPutPdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | `ToNoContentProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | No |  |
| 74 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPutPdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid` | `ToNoContentProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid` | No |  |
| 75 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPatchPdActionResult_Should_Return_NoContent_When_Valid` | `ToPatchedProblemDetailsResult_Should_Return_NoContent_When_Valid` | No |  |
| 76 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPatchPdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails` | `ToPatchedProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails` | No |  |
| 77 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPatchPdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | `ToPatchedProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | No |  |
| 78 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPatchPdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | `ToPatchedProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | No |  |
| 79 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToPatchPdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid` | `ToPatchedProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid` | No |  |
| 80 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToDeletePdActionResult_Should_Return_NoContent_When_Valid` | `ToDeletedProblemDetailsResult_Should_Return_NoContent_When_Valid` | No |  |
| 81 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToDeletePdActionResult_Should_Return_500_When_Fail_Without_ProblemDetails` | `ToDeletedProblemDetailsResult_Should_Return_500_When_Fail_Without_ProblemDetails` | No |  |
| 82 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToDeletePdActionResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | `ToDeletedProblemDetailsResult_Should_Return_ProblemDetails_When_Fail_With_ProblemDetails` | No |  |
| 83 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToDeletePdActionResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | `ToDeletedProblemDetailsResultAsync_From_MlResult_Should_Return_NoContent_When_Valid` | No |  |
| 84 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsTests` | `ToDeletePdActionResultAsync_From_Task_Should_Return_NoContent_When_Valid` | `ToDeletedProblemDetailsResultAsync_From_Task_Should_Return_NoContent_When_Valid` | No |  |
| 85 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsLegacyTests` | `ToSimpleRepoPostActionResult_whenValid_returnsCreated` | `ToCreatedResult_whenValid_returnsCreated` | No |  |
| 86 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsLegacyTests` | `ToSimpleRepoPostActionResult_whenFail_returnsInternalServerError` | `ToCreatedResult_whenFail_returnsInternalServerError` | No |  |
| 87 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsLegacyTests` | `ToSimpleRepoPostActionResult_whenNotFoundFail_returnsNotFound` | `ToCreatedResult_whenNotFoundFail_returnsNotFound` | No |  |
| 88 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsLegacyTests` | `ToSimpleRepoPostActionResultAsync_whenFail_returnsInternalServerError` | `ToCreatedResultAsync_whenFail_returnsInternalServerError` | No |  |
| 89 | Metodo | AspNetCore.Results.Tests.Unit | `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers.MlResultActionResultExtensionsLegacyTests` | `ToSimpleRepoPostActionResultAsync_fromTask_whenFail_returnsInternalServerError` | `ToCreatedResultAsync_fromTask_whenFail_returnsInternalServerError` | No |  |
| 90 | Metodo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Helpers.Extensions` | `ConverterTo` | `ConvertToType` | Sí | Converter es sustantivo, To sin complemento |
| 91 | Metodo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Helpers.Extensions` | `ConvertDateTime` | `ParseDateTimeInvariant` | No | Nombre, usar InvariantCulture es cambio de contrato (aparte) |
| 92 | Metodo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Helpers.Extensions` | `GetPkValuesErrorMessage` | `BuildKeyParseErrorMessage` | Sí |  |
| 93 | Metodo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Helpers.Extensions` | `GetPkValues` | `ParseKeyValues` | No | Parsea cadenas de la URL: el llamador debe esperar errores de formato |
| 94 | member | AspNetCore.Controllers | `*` | `_pkFields` | `_keySelector` | No | SIN COINCIDENCIAS |
| 95 | member | AspNetCore.Controllers.OutputCaching | `*` | `_pkFields` | `_keySelector` | No | SIN COINCIDENCIAS |
| 96 | Tipo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Controllers` | `SimpleMlControllerBase` | `MlCrudControllerBase` | No | Simple es un juicio de valor |
| 97 | Tipo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Controllers` | `SimpleMlComplexPkControllerBase` | `MlCompositeKeyCrudControllerBase` | No | Simple + Complex era contradictorio, el termino estandar es composite key |
| 98 | Tipo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Attributes` | `PkParameterAttribute` | `CompositeKeyParameterAttribute` | No |  |
| 99 | Tipo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers` | `SimpleMlCacheControllerBase` | `MlCachedCrudControllerBase` | No | Derivado |
| 100 | Tipo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers` | `SimpleMlComplexCacheControllerBase` | `MlCachedCompositeKeyCrudControllerBase` | No | Derivado |
| 101 | Tipo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Policies` | `PerControllerOutputCachePolicy` | `ControllerScopedOutputCachePolicy` | No |  |
| 102 | Constante | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Policies.ControllerScopedOutputCachePolicy` | `BypassHeader` | `CacheBypassHeader` | Sí |  |
| 103 | Metodo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.RegisterServices` | `AddWebControllersCache` | `AddMlControllersOutputCaching` | Sí | Derivado: el proyecto ya no se llama WebControllers.Cache |
| 104 | Metodo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.RegisterServices` | `AddWebControllers` | `AddMlControllers` | Sí | Derivado. Hoy no hace nada: eliminarlo es cambio de contrato |
| 105 | Constante | Shared | `MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders` | `"X-Page-Number" / "X-Page-Size" (literales duplicados)` | `MlPaginationHeaders.PageNumber / .PageSize` | No | Constantes compartidas entre AspNetCore.Results y Http |

### Nivel 10 → Clientes REST e invocador HTTP

<sub>Lote `fase11-nivel10-http` → 91 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | Http | `MoralesLarios.OOFP.Http.GenClientFp<TDto>` | `GetHttpClientFactoryKey` | `GetClientName` | Sí | Devuelve el nombre del cliente registrado |
| 2 | Metodo | Http | `MoralesLarios.OOFP.Http.GenClientFp<TRequest, TResponse>` | `GetHttpClientFactoryKey` | `GetClientName` | Sí | Devuelve el nombre del cliente registrado |
| 3 | Metodo | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager` | `PostGetPaginationAsync` | `PostAndReadPagedAsync` | No |  |
| 4 | Metodo | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager` | `PostGetAsync` | `PostAndReadAsync` | No | PostGet se leia como POST seguido de GET |
| 5 | Metodo | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager` | `GetPaginationAsync` | `GetPagedAsync` | No |  |
| 6 | Metodo | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager` | `InternalGetUrl` | `BuildRequestUri` | No | Internal ya lo dice el modificador, construye una URI |
| 7 | Metodo | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager` | `InternalPostGetAsync` | `SendAndReadAsync` | No |  |
| 8 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageInfo` | `AddPaginationHeaders` | Sí | Info vacio. Pasar a HttpRequestMessage es cambio de contrato |
| 9 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageInfoAsync` | `AddPaginationHeadersAsync` | Sí | Info vacio. Pasar a HttpRequestMessage es cambio de contrato |
| 10 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageNumber` | `AddPageNumberHeader` | Sí | Derivado |
| 11 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageNumberAsync` | `AddPageNumberHeaderAsync` | Sí | Derivado |
| 12 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageSize` | `AddPageSizeHeader` | Sí | Derivado |
| 13 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderPageSizeAsync` | `AddPageSizeHeaderAsync` | Sí | Derivado |
| 14 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderInfoAsInt` | `AddIntHeader` | Sí | Derivado |
| 15 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderInfoAsIntAsync` | `AddIntHeaderAsync` | Sí | Derivado |
| 16 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderInfo` | `AddHeader` | No | Derivado |
| 17 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaderInfoAsync` | `AddHeaderAsync` | No | Derivado |
| 18 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeaders` | `TrySetHeaders` | Sí | Ya devuelve MlResult: Try es honesto |
| 19 | Metodo | Http | `MoralesLarios.OOFP.Http.Helpers.MlHttpRequestExtensions` | `SetHeadersAsync` | `TrySetHeadersAsync` | Sí | Ya devuelve MlResult: Try es honesto |
| 20 | Metodo | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>` | `GetPkValuesString` | `FormatCompositeKey` | No | Describe el que, no el como |
| 21 | Metodo | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>` | `GetPkValuesString` | `FormatCompositeKey` | No | Describe el que, no el como |
| 22 | Metodo | Http | `MoralesLarios.OOFP.Http.RegisterServices` | `AddGenClientFp` | `AddRestClient` | Sí |  |
| 23 | Metodo | Http | `MoralesLarios.OOFP.Http.RegisterServices` | `AddGenClientComplexFp` | `AddCompositeKeyRestClient` | Sí |  |
| 24 | Metodo | Http | `MoralesLarios.OOFP.Http.RegisterServices` | `AddGenClientDuplexComplexFp` | `AddCompositeKeyRestClient` | Sí | La aridad generica (4) distingue la variante DTO entrada != salida |
| 25 | Metodo | Http | `MoralesLarios.OOFP.Http.RegisterServices` | `AddHttpClientsFp` | `AddMlHttp` | Sí | Derivado: el proyecto ya no se llama HttpClients, Fp en DI no aporta |
| 26 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.GetAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 27 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PostAndReadAsync<T, TResult>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 28 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PostAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 29 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PostAsync<T, K>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 30 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PutAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 31 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.DeleteAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 32 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.DeleteByIdAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, MoralesLarios.OOFP.ValueObjects.NotEmptyString, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 33 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.CreateHttpClient(MoralesLarios.OOFP.ValueObjects.ServiceKey)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 34 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.InternalHttpActionAsync<TRequest, TResponse>(MoralesLarios.OOFP.ValueObjects.ServiceKey, System.Net.Http.HttpRequestMessage, string, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 35 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.ActionPostAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, System.Net.Http.HttpRequestMessage, string, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 36 | Parametro | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.ActionEmptyAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, string, System.Net.Http.HttpRequestMessage, string, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 37 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PostAndReadAsync<T, TResult>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 38 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.CreateHttpClient(MoralesLarios.OOFP.ValueObjects.ServiceKey)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 39 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.GetAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 40 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PostAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 41 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PutAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 42 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.DeleteAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 43 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.DeleteByIdAsync<T>(MoralesLarios.OOFP.ValueObjects.ServiceKey, MoralesLarios.OOFP.ValueObjects.NotEmptyString, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 44 | Parametro | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PostAsync<T, K>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `httpClientFactoryKey` | `clientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name). El tipo (ServiceKey) no cambia |
| 45 | Parametro | Http | `MoralesLarios.OOFP.Http.RegisterServices.AddRestClient<TService, TImplementation>(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Func<MoralesLarios.OOFP.ValueObjects.ServiceKey>, System.Action<System.Net.Http.HttpClient>)` | `configureHttpClientKey` | `configureClient` | No |  |
| 46 | Parametro | Http | `MoralesLarios.OOFP.Http.RegisterServices.AddCompositeKeyRestClient<TService, TImplementation, TDto>(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Func<MoralesLarios.OOFP.ValueObjects.ServiceKey>, System.Action<System.Net.Http.HttpClient>)` | `configureHttpClientKey` | `configureClient` | No |  |
| 47 | Parametro | Http | `MoralesLarios.OOFP.Http.RegisterServices.AddCompositeKeyRestClient<TService, TImplementation, TRequest, TResponse>(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Func<MoralesLarios.OOFP.ValueObjects.ServiceKey>, System.Action<System.Net.Http.HttpClient>)` | `configureHttpClientKey` | `configureClient` | No |  |
| 48 | Parametro | Http | `MoralesLarios.OOFP.Http.RegisterServices.ResolveHttpClientFactoryKey(System.Func<MoralesLarios.OOFP.ValueObjects.ServiceKey>, string)` | `configureHttpClientKey` | `configureClient` | No |  |
| 49 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.GetByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 50 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.GetByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 51 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.PutByIdAsync(TDto, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 52 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.PutByIdAsync(object[], TDto, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 53 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.DeleteByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 54 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TDto>.DeleteByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 55 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.GetByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 56 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.GetByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 57 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.PutByIdAsync(TRequest, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 58 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.PutByIdAsync(object[], TRequest, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 59 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.DeleteByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 60 | Parametro | Http | `MoralesLarios.OOFP.Http.GenComplexClientFp<TRequest, TResponse>.DeleteByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 61 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.DeleteByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 62 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.DeleteByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 63 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.GetByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 64 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.GetByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 65 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.PutByIdAsync(TDto, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 66 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TDto>.PutByIdAsync(object[], TDto, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 67 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.DeleteByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 68 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.DeleteByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 69 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.GetByIdAsync(System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 70 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.GetByIdAsync(object[], System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 71 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.PutByIdAsync(TRequest, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken, params object[])` | `pk` | `keyValues` | No |  |
| 72 | Parametro | Http | `MoralesLarios.OOFP.Http.IGenComplexClientFp<TRequest, TResponse>.PutByIdAsync(object[], TRequest, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `pk` | `keyValues` | No |  |
| 73 | Generico | Http | `MoralesLarios.OOFP.Http.HttpClientFactoryManager.PostAsync<T, K>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `K` | `TResponse` | No | Una letra suelta no dice su papel |
| 74 | Generico | Http | `MoralesLarios.OOFP.Http.IHttpClientFactoryManager.PostAsync<T, K>(MoralesLarios.OOFP.ValueObjects.ServiceKey, T, string, System.Collections.Generic.Dictionary<string, string>, System.Threading.CancellationToken)` | `K` | `TResponse` | No | Una letra suelta no dice su papel |
| 75 | Interfaz | Http | `MoralesLarios.OOFP.Http` | `IGenComplexClientFp` | `ICompositeKeyRestClientFp` | No | Complex no significaba nada: la entidad tiene clave compuesta |
| 76 | Tipo | Http | `MoralesLarios.OOFP.Http` | `GenComplexClientFp` | `CompositeKeyRestClientFp` | No |  |
| 77 | Interfaz | Http | `MoralesLarios.OOFP.Http` | `IGenClientFp` | `IRestClientFp` | No | Gen no se entiende, Rest dice protocolo y estilo |
| 78 | Tipo | Http | `MoralesLarios.OOFP.Http` | `GenClientFp` | `RestClientFp` | No |  |
| 79 | Interfaz | Http | `MoralesLarios.OOFP.Http` | `IHttpClientFactoryManager` | `IHttpApiInvoker` | No | No es una fabrica de HttpClient: ejecuta la llamada y deserializa |
| 80 | Tipo | Http | `MoralesLarios.OOFP.Http` | `HttpClientFactoryManager` | `HttpApiInvoker` | No |  |
| 81 | Record | Http | `MoralesLarios.OOFP.Http.Requests` | `CallRequestPaginationParamsInfo` | `PagedApiCallOptions` | No |  |
| 82 | Record | Http | `MoralesLarios.OOFP.Http.Requests` | `CallRequestParamsInfo` | `ApiCallOptions` | No | Cuatro sustantivos genericos encadenados |
| 83 | Tipo | Http.Tests.Integration | `MoralesLarios.OOFP.Http.Tests.Integration` | `GenClientFpPruebaComplexTests` | `RestClientFpPruebaComplexTests` | No |  |
| 84 | Tipo | Http.Tests.Integration | `MoralesLarios.OOFP.Http.Tests.Integration` | `GenClientFpPruebasTests` | `RestClientFpPruebasTests` | No |  |
| 85 | Tipo | Http.Tests.Integration | `MoralesLarios.OOFP.Http.Tests.Integration` | `GenClientFpTests` | `RestClientFpTests` | No |  |
| 86 | Tipo | Http.Tests.Integration | `MoralesLarios.OOFP.Http.Tests.Integration` | `GenComplexClientTests` | `CompositeKeyRestClientTests` | No |  |
| 87 | Metodo | Http.Tests.Unit | `MoralesLarios.OOFP.Http.Tests.Unit.RegisterServicesTests` | `AddGenClientFp_uses_default_key_when_not_configured` | `AddRestClient_uses_default_key_when_not_configured` | No |  |
| 88 | Metodo | Http.Tests.Unit | `MoralesLarios.OOFP.Http.Tests.Unit.RegisterServicesTests` | `AddGenClientFp_uses_custom_key_when_configured` | `AddRestClient_uses_custom_key_when_configured` | No |  |
| 89 | Metodo | Http.Tests.Unit | `MoralesLarios.OOFP.Http.Tests.Unit.RegisterServicesTests` | `AddGenClientComplexFp_uses_custom_key_when_configured` | `AddRestClient_uses_custom_key_when_configured` | No |  |
| 90 | Metodo | Http.Tests.Unit | `MoralesLarios.OOFP.Http.Tests.Unit.RegisterServicesTests` | `AddGenClientComplexFp_throws_when_key_is_null` | `AddRestClient_throws_when_key_is_null` | No |  |
| 91 | Metodo | Http.Tests.Unit | `MoralesLarios.OOFP.Http.Tests.Unit.RegisterServicesTests` | `AddGenClientDuplexComplexFp_throws_when_key_is_null` | `AddRestClient_throws_when_key_is_null` | No |  |

### Nivel 10b → Parámetros de tipo genérico

<sub>Lote `fase11b-tipos-genericos-hermanos` → 12 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Interfaz | Application | `MoralesLarios.OOFP.Application.Services` | `IGenServiceFp` | `ICrudServiceFp` | No | Variante <TEntity, TRequest, TResponse> (no se renombro en la Fase 8 por un fallo de la herramienta con la aridad) |
| 2 | Tipo | Application | `MoralesLarios.OOFP.Application.Services` | `GenServiceFp` | `CrudServiceFp` | No | Variante <TEntity, TRequest, TResponse> |
| 3 | Tipo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Controllers` | `SimpleMlControllerBase` | `MlCrudControllerBase` | No | Variante <TEntity, TRequest, TResponse, TPk> |
| 4 | Tipo | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Controllers` | `SimpleMlComplexPkControllerBase` | `MlCompositeKeyCrudControllerBase` | No | Variante <TEntity, TRequest, TResponse> |
| 5 | Tipo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers` | `SimpleMlCacheControllerBase` | `MlCachedCrudControllerBase` | No | Variante <TEntity, TRequest, TResponse, TPk> |
| 6 | Tipo | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers` | `SimpleMlComplexCacheControllerBase` | `MlCachedCompositeKeyCrudControllerBase` | No | Variante <TEntity, TRequest, TResponse> |
| 7 | Interfaz | Http | `MoralesLarios.OOFP.Http` | `IGenComplexClientFp` | `ICompositeKeyRestClientFp` | No | Variante <TRequest, TResponse> |
| 8 | Tipo | Http | `MoralesLarios.OOFP.Http` | `GenComplexClientFp` | `CompositeKeyRestClientFp` | No | Variante <TRequest, TResponse> |
| 9 | Interfaz | Http | `MoralesLarios.OOFP.Http` | `IGenClientFp` | `IRestClientFp` | No | Variante <TRequest, TResponse> |
| 10 | Tipo | Http | `MoralesLarios.OOFP.Http` | `GenClientFp` | `RestClientFp` | No | Variante <TRequest, TResponse> |
| 11 | Record | Http | `MoralesLarios.OOFP.Http.Requests` | `CallRequestPaginationParamsInfo` | `PagedApiCallOptions` | No | Variante <TRequest> |
| 12 | Record | Http | `MoralesLarios.OOFP.Http.Requests` | `CallRequestParamsInfo` | `ApiCallOptions` | No | Variante <TRequest> |

### Nivel 10c → Formato de claves compuestas

<sub>Lote `fase11c-format-composite-key` → 2 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | member | Application | `*` | `GetPkValues` | `FormatCompositeKey` | No | SIN COINCIDENCIAS |
| 2 | Metodo | EntityFrameworkCore | `MoralesLarios.OOFP.EntityFrameworkCore.Helpers.Extensions` | `GetPkValues` | `FormatCompositeKey` | Sí | Idem |

### Residuos detectados por el inventario

<sub>Lote `fase11d-residuos` → 9 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween (operador implicito de tupla)` | `minLenght` | `minLength` | No | Elemento de tupla del operador implicito |
| 2 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.IntBetween (operador implicito de tupla)` | `maxLenght` | `maxLength` | No | Elemento de tupla del operador implicito |
| 3 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringLengthBetween (operador implicito de tupla)` | `minLenght` | `minLength` | No | Elemento de tupla del operador implicito |
| 4 | Parametro | ValueObjects | `MoralesLarios.OOFP.ValueObjects.StringLengthBetween (operador implicito de tupla)` | `maxLenght` | `maxLength` | No | Elemento de tupla del operador implicito |
| 5 | Variable | ValueObjects.Tests.Unit | `MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntLessThanTests` | `minLenght` | `minLength` | No | Variables locales de test |
| 6 | Parametro | AspNetCore.Controllers | `MoralesLarios.OOFP.AspNetCore.Controllers.Controllers.MlCompositeKeyCrudControllerBase` | `_pkFields` | `_keySelector` | No | Es una funcion selectora, no una lista de campos |
| 7 | Parametro | AspNetCore.Controllers.OutputCaching | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers.MlCachedCompositeKeyCrudControllerBase` | `_pkFields` | `_keySelector` | No | Es una funcion selectora, no una lista de campos |
| 8 | Campo | AspNetCore.Results | `MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultActionResultExtensionsLegacy` | `notFoundKeys` | `NotFoundDetailKeys` | No | Campo estatico pasado a readonly y PascalCase |
| 9 | Literal | Http | `MoralesLarios.OOFP.Http.HttpApiInvoker.GetPagedAsync` | `"GetPaginationAsync<...>"` | `"GetPagedAsync<...>"` | No | Literal de traza de log desincronizado con el nombre del metodo |

### Fase 13 → Documentación (`README.md` y `docs/`)

<sub>Lote `fase13-docs` → 19 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Fichero doc |  | `docs/Several` | `1_EmptyToFailed.md` | `1_FailIfEmpty.md` | No | Renombrado con git mv |
| 2 | Fichero doc |  | `docs/Several` | `3_BoolToResult.md` | `3_FailIfFalse.md` | No | Renombrado con git mv |
| 3 | Fichero doc |  | `docs/Several` | `4_Combine.md` | `4_CombineAll.md` | No | Renombrado con git mv |
| 4 | Mencion doc |  | `docs/core-types/MlResultCollections.md` | `ProjectionWhileAsync` | `TraverseWhileAsync` | No | Alineado con el nombre real del API |
| 5 | Mencion doc |  | `docs/core-types/MlResultCollections.md` | `ProjectionSplitAsync` | `TraverseSplitAsync` | No | Alineado con el nombre real del API |
| 6 | Mencion doc |  | `docs/loops/Collections.md` | `ProjectionWhileAsync` | `TraverseWhileAsync` | No |  |
| 7 | Mencion doc |  | `docs/loops/Collections.md` | `ProjectionSplitAsync` | `TraverseSplitAsync` | No |  |
| 8 | Mencion doc |  | `docs/1_Intro.md` | `ProjectionSplitAsync` | `TraverseSplitAsync` | No |  |
| 9 | Mencion doc |  | `MoralesLarios.OOFP.Core/README.md` | `ProjectionWhileAsync` | `TraverseWhileAsync` | No |  |
| 10 | Mencion doc |  | `MoralesLarios.OOFP.Core/README.md` | `ProjectionSplitAsync` | `TraverseSplitAsync` | No |  |
| 11 | Mencion doc |  | `MoralesLarios.OOFP.FileSystem/README.md` | `IWrapperIOExtendido` | `IMlFileSystemExtendido` | No | Ejemplo de extension del contrato |
| 12 | Mencion doc |  | `MoralesLarios.OOFP.FileSystem/README.md` | `WrapperIOExtendido` | `MlFileSystemExtendido` | No | Ejemplo de extension del contrato |
| 13 | Mencion doc |  | `MoralesLarios.OOFP.Shared/README.md` | `PaginationResultInfoExtensions` | `PagedResultExtensions` | No | Ejemplo de extensiones sobre PagedResult<T> |
| 14 | Mencion doc |  | `docs/Several/1_FailIfEmpty.md` | `TryEmptyToFailed` | `TryFailIfEmpty` | No | Nota "no existe Try..." |
| 15 | Mencion doc |  | `docs/Several/2_FailIfNull.md` | `TryNullToFailed` | `TryFailIfNull` | No | Nota "no existe Try..." |
| 16 | Mencion doc |  | `docs/Several/3_FailIfFalse.md` | `TryBoolToResult` | `TryFailIfFalse` | No | Nota "no existe Try..." |
| 17 | Ancla doc |  | `18 ficheros .md` | `anclas con nombres de la 1.x` | `anclas con nombres de la 2.0.0` | No | 59 anclas internas corregidas: de 57 rotas a 0 |
| 18 | Codificacion |  | `AspNetCore.Controllers.OutputCaching/README.md` | `98 caracteres U+FFFD (mojibake preexistente)` | `texto correcto UTF-8` | No | Dano preexistente de commits de abril-junio |
| 19 | Codificacion |  | `EntityFrameworkCore.Sample/README.md` | `12 caracteres U+FFFD (mojibake preexistente)` | `texto correcto UTF-8` | No | Dano preexistente |

### Fase 13b → Unificación del parámetro genérico `TReturn` en `TResult`

<sub>Lote `fase13-genericos` → 10 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Generico | Core | `MlResultBindExtensions` | `TReturn` | `TResult` | No | Unificacion D-10. El plan exigia uno de los dos. Se elige TResult por coherencia con Func(T, TResult) del BCL. El nombre de un parametro de tipo no forma parte de la firma: no hay ruptura binaria ni de codigo fuente |
| 2 | Generico | Core | `MlResultMapExtensions` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 3 | Generico | Core | `MlResultMatchExtensions` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 4 | Generico | Core | `MlResultChangeReturnResult` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 5 | Generico | Core | `MlResultConversions` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 6 | Generico | Core | `MlResultExtensions` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 7 | Generico | Core | `MlResultBindExtensions.Obsolete` | `TReturn` | `TResult` | No | Unificacion D-10. Puentes Obsolete |
| 8 | Generico | Core | `MlResultChangeReturnResult.Obsolete` | `TReturn` | `TResult` | No | Unificacion D-10. Puentes Obsolete |
| 9 | Generico | Core | `MlErrorDetailsActions` | `TReturn` | `TResult` | No | Unificacion D-10 |
| 10 | Mencion doc |  | `CONTRIBUTING.md` | *(inexistente)* | `CONTRIBUTING.md` | No | Paso 13.1. Guia de estilo de nomenclatura de la 2.0.0 |

### Fase 14 → Residuos de los sufijos `...Manager` y `...Info` en `Http` y `AspNetCore.Results`

<sub>Lote `fase14-residuos` → 24 cambios</sub>

| # | Tipo | Proyecto | Contenedor | Antes | Después | Puente | Nota |
|---:|---|---|---|---|---|:-:|---|
| 1 | Metodo | Http | `IRestClientFp(TDto)` | `GetIHttpClientFactoryManager` | `GetApiInvoker` | No | Sufijo Manager prohibido. Devuelve el IHttpApiInvoker subyacente |
| 2 | Metodo | Http | `IRestClientFp(TRequest, TResponse)` | `GetIHttpClientFactoryManager` | `GetApiInvoker` | No | Sufijo Manager prohibido. Devuelve el IHttpApiInvoker subyacente |
| 3 | Metodo | Http | `RestClientFp(TDto)` | `GetIHttpClientFactoryManager` | `GetApiInvoker` | Sí | Sufijo Manager prohibido. Puente Obsolete en RestClientFp.Obsolete.cs |
| 4 | Metodo | Http | `RestClientFp(TRequest, TResponse)` | `GetIHttpClientFactoryManager` | `GetApiInvoker` | Sí | Sufijo Manager prohibido. Puente Obsolete en RestClientFp.Obsolete.cs |
| 5 | Metodo | Http | `RestClientFp(TDto)` | `GetHttpClientFactoryKey` | `GetClientName` | Sí | Puente Obsolete anadido tambien a la clase de un generico |
| 6 | Propiedad | Http | `ApiCallOptions` | `HttpClientFactoryKey` | `ClientName` | No | Coherente con GetClientName e IHttpClientFactory.CreateClient(name) |
| 7 | Propiedad | Http | `PagedApiCallOptions` | `HttpClientFactoryKey` | `ClientName` | No | Coherente con ApiCallOptions.ClientName |
| 8 | Campo | Http | `RestClientFp(TDto)` | `_httpClientFactoryManager` | `_apiInvoker` | No | Campo del constructor primario. Coherente con IHttpApiInvoker |
| 9 | Campo | Http | `RestClientFp(TDto)` | `_httpClientFactoryKey` | `_clientName` | No | Campo del constructor primario. Coherente con GetClientName |
| 10 | Campo | Http | `HttpApiInvoker` | `_httpClientFactoryManager` | `_apiInvoker` | No | Campo interno |
| 11 | Metodo | Http | `RegisterServices` | `ResolveHttpClientFactoryKey` | `ResolveClientName` | No | Helper interno de registro. Coherente con ClientName |
| 12 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderInfo` | `GetHeader` | Sí | Sufijo Info prohibido. Simetrico de AddHeader |
| 13 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderInfoAsync` | `GetHeaderAsync` | Sí | Sufijo Info prohibido. Simetrico de AddHeader |
| 14 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderInfoAsIntNotNegative` | `GetIntHeader` | Sí | Sufijo Info prohibido. Simetrico de AddIntHeader |
| 15 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderInfoAsIntNotNegativeAsync` | `GetIntHeaderAsync` | Sí | Sufijo Info prohibido. Simetrico de AddIntHeader |
| 16 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageNumber` | `GetPageNumberHeader` | Sí | Simetrico de AddPageNumberHeader |
| 17 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageNumberAsync` | `GetPageNumberHeaderAsync` | Sí | Simetrico de AddPageNumberHeader |
| 18 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageSize` | `GetPageSizeHeader` | Sí | Simetrico de AddPageSizeHeader |
| 19 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageSizeAsync` | `GetPageSizeHeaderAsync` | Sí | Simetrico de AddPageSizeHeader |
| 20 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageInfo` | `GetPaginationHeaders` | Sí | Sufijo Info prohibido. Simetrico de AddPaginationHeaders |
| 21 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPageInfoAsync` | `GetPaginationHeadersAsync` | Sí | Sufijo Info prohibido. Simetrico de AddPaginationHeaders |
| 22 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPaginationInfo` | `GetPageRequest` | Sí | Sufijo Info prohibido. Devuelve un PageRequest |
| 23 | Metodo | AspNetCore.Results | `HttpRequestPaginationExtensions` | `GetHeaderPaginationInfoAsync` | `GetPageRequestAsync` | Sí | Sufijo Info prohibido. Devuelve un PageRequest |
| 24 | Fichero | AspNetCore.Results | `Helpers` | *(inexistente)* | `HttpRequestPaginationExtensions.Obsolete.cs` | No | Nuevo fichero con los 12 puentes Obsolete de la clase |

---

## Cambios de contrato (Fase 12)

Estos cambios **no son renombrados**: modifican la forma del API público
(visibilidad, mutabilidad, anulabilidad, asincronía o comportamiento).
Se listan aparte porque su **impacto** en el código consumidor es distinto.

| # | Proyecto | Elemento | Antes | Después | Impacto |
|---:|---|---|---|---|---|
| 1 | ValueObjects | `DecimalNonNegative` | `public static decimal MinAllowedValue { get, private set }` | `private const decimal MinAllowedValue` | **Puente `[Obsolete]`**: C3: el limite deja de ser estado mutable publico. El puente estatico limit sigue existiendo en DecimalNonNegative.Obsolete.cs. |
| 2 | ValueObjects | `DoubleNonNegative` | `public static double MinAllowedValue { get, private set }` | `private const double MinAllowedValue` | **Puente `[Obsolete]`**: C3: el limite deja de ser estado mutable publico. El puente estatico limit sigue existiendo en DoubleNonNegative.Obsolete.cs. |
| 3 | ValueObjects | `Age` | `public Age(int value)` | `protected Age(int value)` | **Ruptura**: C4: el constructor lanzaba excepcion. Se usa la factoria FromInt/ByInt. |
| 4 | ValueObjects | `Id` | `public Id(double value)` | `protected Id(double value)` | **Ruptura**: C4: el constructor lanzaba excepcion. Se usa la factoria FromDouble/ByDouble. |
| 5 | ValueObjects | `IdLite` | `public IdLite(int value)` | `protected IdLite(int value)` | **Ruptura**: C4: el constructor lanzaba excepcion. Se usa la factoria FromInt/ByInt. |
| 6 | Validation | `Validation` | `abstract class MlSelfValidatable<T>` | `interface IMlSelfValidatable<T>` | **Puente `[Obsolete]`**: C5: la autovalidacion pasa a interfaz para no consumir la unica clase base del consumidor. La clase abstracta queda como puente Obsolete implementando la interfaz. |
| 7 | AspNetCore.Controllers | `AspNetCore.Controllers.Controllers` | `public class MlCrudControllerBase<TEntity, TDto, TPk>` | `public abstract class MlCrudControllerBase<TEntity, TDto, TPk>` | **Ruptura**: C10: las clases base de controlador pasan a abstractas para que ASP.NET no las descubra como controladores. |
| 8 | AspNetCore.Controllers | `AspNetCore.Controllers.Controllers` | `public class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>` | `public abstract class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>` | **Ruptura**: C10. |
| 9 | AspNetCore.Controllers | `AspNetCore.Controllers.Controllers` | `public class MlCompositeKeyCrudControllerBase<TEntity, TDto>` | `public abstract class MlCompositeKeyCrudControllerBase<TEntity, TDto>` | **Ruptura**: C10. |
| 10 | AspNetCore.Controllers | `AspNetCore.Controllers.Controllers` | `public class MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>` | `public abstract class MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>` | **Ruptura**: C10. |
| 11 | AspNetCore.Controllers.OutputCaching | `AspNetCore.Controllers.OutputCaching.Controllers` | `public class MlCachedCrudControllerBase<TEntity, TDto, TPk>` | `public abstract class MlCachedCrudControllerBase<TEntity, TDto, TPk>` | **Ruptura**: C10. |
| 12 | AspNetCore.Controllers.OutputCaching | `AspNetCore.Controllers.OutputCaching.Controllers` | `public class MlCachedCrudControllerBase<TEntity, TRequest, TResponse, TPk>` | `public abstract class MlCachedCrudControllerBase<TEntity, TRequest, TResponse, TPk>` | **Ruptura**: C10. |
| 13 | AspNetCore.Controllers.OutputCaching | `AspNetCore.Controllers.OutputCaching.Controllers` | `public class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>` | `public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>` | **Ruptura**: C10. |
| 14 | AspNetCore.Controllers.OutputCaching | `AspNetCore.Controllers.OutputCaching.Controllers` | `public class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>` | `public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>` | **Ruptura**: C10. |
| 15 | Http | `Http.Helpers.MlResponseWebExtensions` | `ToResponseErrorsDescriptionAsync` | `ReadErrorDescriptionAsync` | **Puente `[Obsolete]`**: C11: el nombre no indicaba que lee el cuerpo de la respuesta. |
| 16 | Http | `Http.Helpers.MlResponseWebExtensions` | `ToResponseErrorsDescription (sincrono, usaba .Result)` | `ReadErrorDescriptionAsync (unica version, asincrona)` | **Puente `[Obsolete]`**: C11: se elimina la version sincrona que bloqueaba con .Result. Las 5 llamadas de HttpApiInvoker pasan a await. |
| 17 | AspNetCore.Controllers | `AspNetCore.Controllers.RegisterServices` | `AddMlControllers (no-op sin marcar)` | `AddMlControllers [Obsolete] no-op` | **Puente `[Obsolete]`**: C12: no registraba nada. Se marca Obsolete para que el consumidor retire la llamada. El puente AddWebControllers ya no delega. |
| 18 | AspNetCore.Controllers | `CompositeKeyParameterAttribute` | `public string Description (string description = null!)` | `public string? Description (string? description = null)` | **Ruptura**: C14: anulabilidad correcta en lugar del supresor null!. |
| 19 | AspNetCore.Controllers | `AspNetCore.Controllers.Helpers.Extensions` | `DateTime.TryParse/TryParseExact con CurrentCulture` | `con CultureInfo.InvariantCulture` | **Ruptura**: C16: el parseo de claves de URL dependia de la cultura del servidor. |

---

## Cambios planificados no aplicados

**No queda ningún renombrado planificado sin aplicar**: los once niveles del plan
(Nivel 0 a Nivel 10) y la fase de cierre se ejecutaron por completo.

Se dejó **deliberadamente fuera** del alcance de la 2.0.0 lo siguiente:

| Elemento | Motivo |
|---|---|
| Documentos de `Temas Pendientes/`, `CONSEJOS_NOMENCLATURA.md` y `MEJORAS_PENDIENTES.md` | Son documentos **históricos** de análisis: deben conservar la nomenclatura anterior para que el razonamiento siga siendo legible. |
| Retirada de los puentes `[Obsolete]` | Se mantienen durante toda la rama 2.x. Su eliminación está planificada para la **3.0.0**. |
| Carpetas de proyectos antiguos vacías | Se eliminaron del árbol de trabajo: eran restos de los `git mv` del Nivel 0 y no contenían ningún fichero versionado. |
| Los 6 tests de integración de `Http.Tests.Integration` | Fallan **desde antes** de este trabajo porque exigen un servidor HTTP vivo. No se tocaron: son un asunto de infraestructura de pruebas, no de nomenclatura. |

Las decisiones abiertas (`D-1` a `D-11`) y el detalle de lo que queda para la
siguiente versión están documentados en
[`Temas Pendientes/Plan-Renombrados-2.0.0.md`](../Temas%20Pendientes/Plan-Renombrados-2.0.0.md).

---

## Validación

Estado tras aplicar **todos** los cambios de este documento:

| Comprobación | Resultado |
|---|---|
| `dotnet build MoralesLarios.OOFP.sln` | ✅ **0 errores** |
| Tests unitarios | ✅ **1 027 / 1 027** |
| Tests de integración (`Http.Tests.Integration`) | 27 / 33 → 6 fallos **preexistentes** (requieren un servidor HTTP vivo) |
| Enlaces relativos entre `.md` | ✅ 1 444 revisados, **0 rotos** |
| Anclas internas de los `.md` | ✅ 826 revisadas, **0 rotas** (eran 57 rotas) |
| Codificación de los `.md` | ✅ UTF-8 válido, **0 caracteres de reemplazo** |

> Los 6 fallos de integración coinciden **exactamente** con la línea base previa
> registrada en [`_linea-base.txt`](./_linea-base.txt), por lo que **no** son regresiones.

