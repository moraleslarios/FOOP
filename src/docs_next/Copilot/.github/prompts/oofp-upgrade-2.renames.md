# Anexo de renombrados — MoralesLarios.OOFP 1.x → 2.0.0

> Fichero de datos del prompt [`/oofp-upgrade-2`](./oofp-upgrade-2.prompt.md).
> **No es documentación para leer de corrido**: es la tabla de reemplazos que el agente debe aplicar.

## Cómo leer las tablas

| Columna | Significado |
| --- | --- |
| **Antes** | Identificador en la versión 1.x |
| **Después** | Identificador en la 2.0.0 |
| **🔗** | `sí` = existe puente `[Obsolete]` (compila con aviso `CS0618`). `NO` = **rompe la compilación** |

Sufijo `[Async]` ⇒ aplica también a la variante asíncrona (p. ej. `Combine` → `CombineAll` y
`CombineAsync` → `CombineAllAsync`). **Reemplaza siempre primero la variante `…Async`.**

Los métodos con prefijo `Try…` siguen la misma regla: `TryBoolToResult` → `TryFailIfFalse`,
`TryNullToFailed` → `TryFailIfNull`, `TryEmptyToFailed` → `TryFailIfEmpty`, `TryProjection` → `TryTraverse`, etc.

---

## 1. `MoralesLarios.OOFP.Core`

### 1.1 Tipos y records

| Antes | Después | 🔗 |
| --- | --- | --- |
| `MlErrorsDetails` | `MlErrorDetails` | NO |
| `MlErrorsDetailsActions` | `MlErrorDetailsActions` | NO |
| `MlResultActions` | `MlResultExtensions` | NO |
| `MlResultActionsBind` | `MlResultBindExtensions` | NO |
| `MlResultActionsErrorsDetails` | `MlResultErrorDetailsExtensions` | NO |
| `MlResultActionsExecSelf` | `MlResultExecSelfExtensions` | NO |
| `MlResultActionsMap` | `MlResultMapExtensions` | NO |
| `MlResultActionsMatch` | `MlResultMatchExtensions` | NO |
| `MlResultActionsSeveral` | `MlResultSeveralExtensions` | NO |
| `MlResultBucles` | `MlResultCollections` | NO |
| `MlResultTransformations` | `MlResultConversions` | NO |
| `ProyectSplitFailInfo` | `TraverseSplitFailure` | NO |
| `ProyectSplitValidInfo` | `TraverseSplitSuccess` | NO |

### 1.2 Propiedades y constantes de `MlResult<T>` / `MlErrorDetails`

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IsValid` | `IsSuccess` | sí |
| `IsFail` | `IsFailure` | sí |
| `Value` | `SuccessValue` | **NO** |
| `ErrorsDetails` | `FailureDetails` | **NO** |
| `Details` | `Metadata` | sí |
| `EX_DESC_KEY` | `ExceptionMetadataKey` | sí |
| `VALUE_KEY` | `ValueMetadataKey` | sí |

### 1.2.a Constantes de `Constants` normalizadas en la **2.1.1**

> ⚠️ Estos renombrados pertenecen a la **2.1.1**, no a la 2.0.0. Los puentes `[Obsolete]`
> se eliminan en la **3.0.0**. Ver el punto **38** de la sección 3.3 del prompt: además del
> nombre, **dos de ellos cambian de valor** y el compilador **no** lo detecta.

| Antes (2.0.0) | Después (2.1.1) | 🔗 |
| --- | --- | --- |
| `Constants.DEFAULT_ERROR_MESSAGE` | `Constants.DefaultErrorMessage` | sí |
| `Constants.NOT_FOUND_KEY` | `Constants.NotFoundKey` | sí |
| `Constants.PARAM_NAME_KEY` | `Constants.ParameterNameKey` | sí |
| `Constants.FAILED_INDEXES_KEY` | `Constants.FailedIndexesKey` | sí |
| `Constants.EXPECTED_KEY` | `Constants.ExpectedKey` | sí |
| `Constants.DEFAULT_EX_ERROR_MESSAGE(ex)` | `EnsureFpMessages.DefaultExceptionError(ex)` | **NO** (cambia de clase) |

### 1.2.b Métodos de extensión reubicados en la **2.1.1**

> ⚠️ Estos **no son renombrados**: el método conserva su nombre y su firma, y **cambia de clase
> contenedora**. Los puentes `[Obsolete]` se eliminan en la **3.0.0**. Ver el punto **42** de la
> sección 3.3 del prompt.
>
> ✅ **Si los invocas en forma de extensión (`valor.ToNullable()`), no tienes que cambiar nada**: el
> espacio de nombres `MoralesLarios.OOFP.Helpers.Extensions` **no cambia** y la resolución de
> métodos de extensión no depende de la clase. Solo se rompe la invocación en **forma estática**.

| Antes (2.0.0) · forma estática | Después (2.1.1) · forma estática | 🔗 |
| --- | --- | --- |
| `Extensions.ToNullable(x)` | `NullableExtensions.ToNullable(x)` | **NO** (cambia de clase) |
| `Extensions.AppendExDetails(d, ex)` | `ExceptionMetadataExtensions.AppendExDetails(d, ex)` | **NO** (cambia de clase) |
| `Extensions.With(x, …)` | `MutationExtensions.With(x, …)` | **NO** (cambia de clase) |
| `Extensions.WithAsync(x, …)` ×2 | `MutationExtensions.WithAsync(x, …)` | **NO** (cambia de clase) |
| `Extensions.VoidToAsync(x, a)` | `TaskLiftExtensions.VoidToAsync(x, a)` | **NO** (cambia de clase) |
| `Extensions.ToFuncTask(f)` ×5 | `TaskLiftExtensions.ToFuncTask(f)` | **NO** (cambia de clase) |

### 1.2.c Tipos retirados en la **2.1.1**

> ⚠️ `ErrorMessage` **no es un renombrado mecánico**: es un tipo que desaparece y cuyo cometido
> asume `MlError`. El puente `[Obsolete]` se elimina en la **3.0.0**. Ver el punto **39** de la
> sección 3.3 del prompt.
>
> 🔎 **Cuidado con los falsos positivos**: `ErrorMessage` es también el nombre de **propiedades** y de
> **parámetros** que **no** cambian (`.ErrorMessage`, `string errorMessage`). Busca siempre con
> límite de palabra y **descarta las coincidencias precedidas por un punto**.

| Antes (2.0.0) | Después (2.1.1) | 🔗 |
| --- | --- | --- |
| `new ErrorMessage(texto)` | `MlError.FromErrorMessage(texto)` | **NO** (cambia de tipo) |
| `MoralesLarios.OOFP.Types.Errors.ErrorMessage` | `MoralesLarios.OOFP.Types.Errors.MlError` | **NO** (cambia de tipo) |
| `ErrorMessage.Message` | `MlError.Message` | sí *(la propiedad conserva el nombre)* |

### 1.3 Métodos

| Antes | Después | 🔗 |
| --- | --- | --- |
| `AddDetail` | `AddMetadataOrThrow` | sí |
| `AddDetails` | `AddMetadataRange` | sí |
| `AddDetailValue` | `AddValueMetadata` | sí |
| `BindSaveValueInDetailsIfFaildFuncResult[Async]` | `BindSaveValueInDetailsIfFailedFuncResult[Async]` | sí |
| `BoolToResult[Async]` | `FailIfFalse[Async]` | sí |
| `BuildErrorMessage` | `FormatErrorMessage` | sí |
| `ChangeReturnResultAlwaisFail[Async]` | `ChangeReturnResultAlwaysFail[Async]` | sí |
| `ChangeReturnResultAlwaisValid[Async]` | `ChangeReturnResultAlwaysValid[Async]` | sí |
| `Combine[Async]` | `CombineAll[Async]` | sí |
| `Do[Async]` | `Pipe[Async]` | sí |
| `EmptyToFailed[Async]` | `FailIfEmpty[Async]` | sí |
| `NullToFailed[Async]` | `FailIfNull[Async]` | sí |
| `FusionErrosIfExists[Async]` | `MergeErrorsIfExists[Async]` | sí |
| `FusionFailErros[Async]` | `MergeFailureErrors[Async]` | sí |
| `GetDetailException[Async]` | `GetException[Async]` | sí |
| `GetDetailValue[Async]` | `GetMetadataValue[Async]` | sí |
| `HasExceptionDetails[Async]` | `ContainsException[Async]` | sí |
| `HasKeyDetails` | `ContainsMetadata` | sí |
| `HasValueDetails[Async]` | `ContainsValueMetadata[Async]` | sí |
| `MergeErrorsDetailsIfFailDiferentTypes[Async]` | `MergeErrorsDetailsIfFailDifferentTypes[Async]` | sí |
| `Projection[Async]` | `Traverse[Async]` | sí |
| `ProjectionParallelAsync` | `TraverseParallelAsync` | sí |
| `ProjectionSplit[Async]` | `TraverseSplit[Async]` | sí |
| `ProjectionSplitPlus[Async]` | `TraverseSplitWithKeys[Async]` | **NO** |
| `ProjectionWhile[Async]` | `TraverseWhile[Async]` | sí |
| `SecureFailErrorsDetails[Async]` | `GetFailureDetailsOrThrow[Async]` | sí |
| `SecureValidValue[Async]` | `GetValueOrThrow[Async]` | sí |
| `ToDetailsDescription` | `GetMetadataSummary` | sí |
| `ToErrorsDescription` | `GetErrorsSummary` | sí |
| `ToErrorsMessages` | `GetErrorMessages` | sí |
| `TryBindSaveValueInDetailsIfFaildFuncResult[Async]` | `TryBindSaveValueInDetailsIfFailedFuncResult[Async]` | sí |
| `TryMapIAsyncf` | `TryMapIfAsync` | **NO** |

### 1.4 Genérico

| Antes | Después | 🔗 |
| --- | --- | --- |
| `TReturn` | `TResult` | **NO** (solo afecta a *overrides* e implementaciones) |

---

## 2. `MoralesLarios.OOFP.Shared` *(antes `…Internals`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `PaginationInfo` | `PageRequest` | **NO** |
| `PaginationResultInfo` | `PagedResult` | **NO** |
| `PaginationResultInfoExtensions` | `PagedResultExtensions` | **NO** |
| literal `"X-Page-Number"` | `MlPaginationHeaders.PageNumber` | **NO** |
| literal `"X-Page-Size"` | `MlPaginationHeaders.PageSize` | **NO** |

---

## 3. `MoralesLarios.OOFP.ValueObjects`

### 3.1 Tipos

| Antes | Después | 🔗 |
| --- | --- | --- |
| `DecimalNotNegative` | `DecimalNonNegative` | NO |
| `DoubleNotNegative` | `DoubleNonNegative` | NO |
| `IntNotNegative` | `IntNonNegative` | NO |
| `Empty` | `EmptyValue` | NO |
| `Key` | `ServiceKey` | NO |
| `Name` | `MemberName` | NO |
| `RangeEnumValueObject` | `EnumRangeValueObject` | NO |
| `StringBetweenLength` | `StringLengthBetween` | NO |

> ⚠️ `Empty`, `Key` y `Name` son nombres muy comunes. **Verifica el namespace de origen antes de reemplazar.**

### 3.2 Factorías — ⚠️ **cambio de semántica, leer la sección 3.2.a del prompt**

| Antes (lanza) | Después (lanza) | Antes (seguro) | Después (seguro) |
| --- | --- | --- | --- |
| `FromString` | `CreateFromString` | `ByString` | `FromString` |
| `FromStringLength` / `FromStringLenght` | `CreateFromLength` | `ByStringLength` | `FromLength` |
| `FromRegex` | `CreateFromRegex` | `ByRegex` | `FromRegex` |
| `FromInt` | `CreateFromInt` | `ByInt` | `FromInt` |
| `FromIntLength` / `FromIntLenght` | `CreateFromIntLength` | `ByIntLength` | `FromIntLength` |
| `FromDecimal` | `CreateFromDecimal` | `ByDecimal` | `FromDecimal` |
| `FromDecimalLength` | `CreateFromDecimalLength` | `ByDecimalLength` | `FromDecimalLength` |
| `FromDouble` | `CreateFromDouble` | `ByDouble` | `FromDouble` |
| `FromDoubleLength` | `CreateFromDoubleLength` | `ByDoubleLength` | `FromDoubleLength` |
| `FromEnum` | `CreateFromEnum` | — | — |

**Qué factoría usa cada tipo (2.0.0):**

| Tipos | Directa (lanza) | Segura (`MlResult<T>`) |
| --- | --- | --- |
| `NotEmptyString`, `MemberName`, `ServiceKey`, `Mail`, `Endpoint`, `StringAsInt` | `CreateFromString` | `FromString` |
| `RegexValue` | `CreateFromRegex` | `FromRegex` |
| `StringMinLength`, `StringMaxLength`, `StringLengthBetween` | `CreateFromLength` | `FromLength` |
| `IntMoreThan`, `IntLessThan`, `IntBetween` | `CreateFromIntLength` | `FromIntLength` |
| `IntNonNegative`, `Age`, `IdLite` | `CreateFromInt` | `FromInt` |
| `DecimalMoreThan`, `DecimalLessThan`, `DecimalBetween` | `CreateFromDecimalLength` | `FromDecimalLength` |
| `DecimalNonNegative` | `CreateFromDecimal` | `FromDecimal` |
| `DoubleMoreThan`, `DoubleLessThan`, `DoubleBetween` | `CreateFromDoubleLength` | `FromDoubleLength` |
| `DoubleNonNegative` | `CreateFromDouble` | `FromDouble` |
| `Id` | `FromDouble` | `ByDouble` |
| `EnumRangeValueObject` | `CreateFromEnum` | `FromEnum` |
| `EmptyValue` | `Create` / `CreateAsync` | — |
| `Void` | `Create` | — |

### 3.3 Otros miembros

| Antes | Después | 🔗 |
| --- | --- | --- |
| `BuildErrorMessage` | `FormatErrorMessage` | NO *(34 ocurrencias)* |
| `EndpointPattern` *(constante)* | `ValidationPattern` | NO |
| `limit` *(propiedad)* | `MinAllowedValue` | NO |
| `MinLenght` | `MinLength` | NO |
| parámetros `lenght`, `maxLenght`, `minLenght` | `length`, `maxLength`, `minLength` | NO |
| `public Age(int)`, `public Id(double)`, `public IdLite(int)` | ahora **`protected`** → usa factorías | NO |
| `public static decimal/double MinAllowedValue { get; private set; }` | ahora **`private const`** → inaccesible | NO |

---

## 4. `MoralesLarios.OOFP.ValueObjects.FileSystem` *(antes `…ValueObjects.IO`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `MlFile` | `FilePath` | NO |
| `MlDirectory` | `DirectoryPath` | NO |
| `ExistsFile` | `ExistingFilePath` | NO |
| `ExistDirectory` | `ExistingDirectoryPath` | NO |
| `FromString` | `CreateFromPath` | NO |
| `ByString` | `FromPath` | NO |
| `BuildErrorMessage` | `FormatErrorMessage` | NO |
| `EndpointPattern` *(constante)* | `SearchPattern` | NO |

---

## 5. `MoralesLarios.OOFP.FileSystem` *(antes `…IO`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IWrapperIO` | `IMlFileSystem` | NO |
| `WrapperIO` | `MlFileSystem` | NO |
| `IWrapperIOExtendido` | `IMlFileSystemExtendido` | NO |
| `WrapperIOExtendido` | `MlFileSystemExtendido` | NO |

---

## 6. `MoralesLarios.OOFP.Validation` y derivados

| Antes | Después | 🔗 |
| --- | --- | --- |
| `MlValidableFp<T>` *(abstract class)* | `IMlSelfValidatable<T>` *(**interfaz**)* | **NO** ⚠️ cambia la herencia |
| `MlSelfValidatable<T>` *(abstract class)* | `IMlSelfValidatable<T>` *(**interfaz**)* | **NO** ⚠️ |
| `FluentValidationsValidator` | `FluentValidationValidator` | NO |
| `ValidateWithFluentValidations[Async]` | `ValidateWithFluentValidation[Async]` | sí |
| `ValidateObject` | `ValidateDataAnnotations` | sí |

---

## 7. `MoralesLarios.OOFP.Configuration` *(antes `…Utilities`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IMlConfigManager` | `IMlConfigurationReader` | NO |
| `MlConfigManager` | `MlConfigurationReader` | NO |
| `AddMlUtilitiesConfig` | `AddMlConfigurationReader` | sí |

---

## 8. `MoralesLarios.OOFP.EntityFrameworkCore` *(antes `…EFCore`)*

### 8.1 Repositorios funcionales (`…Fp` → sin sufijo)

| Antes | Después | 🔗 |
| --- | --- | --- |
| `EFRepoBaseFp` | `EfRepository` | NO |
| `EFRepoFp` | `EfCrudRepository` | NO |
| `EFRepoAdderFp` | `EfCreateRepository` | NO |
| `EFRepoDeleterFp` | `EfDeleteRepository` | NO |
| `EFRepoReaderFp` | `EfReadRepository` | NO |
| `EFRepoUpdaterFp` | `EfUpdateRepository` | NO |
| `EFRepoWriterFp` | `EfWriteRepository` | NO |
| `EFRepoPaginationFp` | `EfPagedRepository` | NO |
| `EFRepoReaderPaginationFp` | `EfPagedReadRepository` | NO |

### 8.2 Repositorios imperativos (lanzan excepciones → prefijo `Throwing`)

| Antes | Después | 🔗 |
| --- | --- | --- |
| `EFRepoBase` | `ThrowingEfRepository` | NO |
| `EFRepo` | `ThrowingEfCrudRepository` | NO |
| `EFRepoAdder` | `ThrowingEfCreateRepository` | NO |
| `EFRepoDeleter` | `ThrowingEfDeleteRepository` | NO |
| `EFRepoReader` | `ThrowingEfReadRepository` | NO |
| `EFRepoUpdater` | `ThrowingEfUpdateRepository` | NO |
| `EFRepoWriter` | `ThrowingEfWriteRepository` | NO |
| `EFRepoPagination` | `ThrowingEfPagedRepository` | NO |
| `EFRepoReaderPagination` | `ThrowingEfPagedReadRepository` | NO |

### 8.3 Interfaces

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IEFRepoFp` | `IEfRepository` | NO |
| `IEFRepoAdderFp` | `IEfCreateRepository` | NO |
| `IEFRepoDeleterFp` | `IEfDeleteRepository` | NO |
| `IEFRepoReaderFp` | `IEfReadRepository` | NO |
| `IEFRepoUpdaterFp` | `IEfUpdateRepository` | NO |
| `IEFRepoWriterFp` | `IEfWriteRepository` | NO |
| `IEFRepoPaginationFp` | `IEfPagedRepository` | NO |
| `IEFRepoReaderPaginationFp` | `IEfPagedReadRepository` | NO |
| `IEFRepo` | `IThrowingEfRepository` | NO |
| `IEFRepoAdder` | `IThrowingEfCreateRepository` | NO |
| `IEFRepoDeleter` | `IThrowingEfDeleteRepository` | NO |
| `IEFRepoReader` | `IThrowingEfReadRepository` | NO |
| `IEFRepoUpdater` | `IThrowingEfUpdateRepository` | NO |
| `IEFRepoWriter` | `IThrowingEfWriteRepository` | NO |
| `IEFRepoPagination` | `IThrowingEfPagedRepository` | NO |
| `IEFRepoReaderPagination` | `IThrowingEfPagedReadRepository` | NO |
| `IEFPaginatorFp` | `IEfPaginator` | NO |

> ⚠️ **ORDEN**: reemplaza siempre de más largo a más corto dentro de este bloque
> (`IEFRepoReaderPaginationFp` → `IEFRepoReaderPagination` → `IEFRepoReaderFp` → `IEFRepoReader` → `IEFRepo`).

### 8.4 Miembros y enums

| Antes | Después | 🔗 |
| --- | --- | --- |
| `OrderBy` *(enum)* | `SortDirection` | NO ⚠️ nombre ambiguo con LINQ |
| `TryGetInternalData` | `TryQueryPage` | NO |
| `TryLast[Async]` | `TryGetLast[Async]` | NO |
| `GetPkValues` | `FormatCompositeKey` | NO |
| `ResolveRepoFp` | `GetRepository` | sí |
| `AddScopedOOFPRepos` | `AddOofpRepositoriesScoped` | sí |
| `AddSingletonOOFPRepos` | `AddOofpRepositoriesSingleton` | sí |
| `AddTransientOOFPRepos` | `AddOofpRepositoriesTransient` | sí |
| `AddOofpRepositoriesScoped` *(1.x tardía)* | `AddOofpRepositories` | **NO** ⚠️ ver nota del prompt |
| parámetro `pk` | `keyValues` | NO |

### 8.5 Namespaces internos

| Antes | Después |
| --- | --- |
| `…EFCore.Repos` | `…EntityFrameworkCore.Repositories` |
| `…EFCore.OopRepos` | `…EntityFrameworkCore.Repositories.Imperative` |
| `…EFCore.Helpers.Constants` | *(eliminado)* |

---

## 9. `MoralesLarios.OOFP.Http` *(antes `…HttpClients`)*

### 9.1 Tipos e interfaces

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IHttpClientFactoryManager` | `IHttpApiInvoker` | NO |
| `HttpClientFactoryManager` | `HttpApiInvoker` | NO |
| `IGenClientFp` | `IRestClientFp` | NO |
| `GenClientFp` | `RestClientFp` | NO |
| `IGenComplexClientFp` | `ICompositeKeyRestClientFp` | NO |
| `GenComplexClientFp` | `CompositeKeyRestClientFp` | NO |
| `CallRequestParamsInfo` | `ApiCallOptions` | NO |
| `CallRequestPaginationParamsInfo` | `PagedApiCallOptions` | NO |

### 9.2 Métodos

| Antes | Después | 🔗 |
| --- | --- | --- |
| `GetIHttpClientFactoryManager` | `GetApiInvoker` | sí |
| `GetHttpClientFactoryKey` | `GetClientName` | sí |
| `ResolveHttpClientFactoryKey` | `ResolveClientName` | NO |
| `GetPaginationAsync` | `GetPagedAsync` | NO |
| `PostGetAsync` | `PostAndReadAsync` | NO |
| `PostGetPaginationAsync` | `PostAndReadPagedAsync` | NO |
| `InternalGetUrl` | `BuildRequestUri` | NO |
| `InternalPostGetAsync` | `SendAndReadAsync` | NO |
| `GetPkValuesString` | `FormatCompositeKey` | NO |
| `ToResponseErrorsDescription` *(síncrono)* | `ReadErrorDescriptionAsync` | **NO** ⚠️ ahora es `async` |
| `ToResponseErrorsDescriptionAsync` | `ReadErrorDescriptionAsync` | sí |
| `SetHeaderInfo[Async]` | `AddHeader[Async]` | sí |
| `SetHeaderInfoAsInt[Async]` | `AddIntHeader[Async]` | sí |
| `SetHeaderPageInfo[Async]` | `AddPaginationHeaders[Async]` | sí |
| `SetHeaderPageNumber[Async]` | `AddPageNumberHeader[Async]` | sí |
| `SetHeaderPageSize[Async]` | `AddPageSizeHeader[Async]` | sí |
| `SetHeaders[Async]` | `TrySetHeaders[Async]` | sí |
| `AddHttpClientsFp` | `AddMlHttp` | sí |
| `AddGenClientFp` | `AddRestClient` | sí |
| `AddGenClientComplexFp` | `AddCompositeKeyRestClient` | sí |
| `AddGenClientDuplexComplexFp` | `AddCompositeKeyRestClient` | sí |

### 9.3 Propiedades, campos, parámetros y genéricos

| Antes | Después | 🔗 |
| --- | --- | --- |
| `HttpClientFactoryKey` | `ClientName` | NO |
| `_httpClientFactoryKey` | `_clientName` | NO |
| `_httpClientFactoryManager` | `_apiInvoker` | NO |
| parámetro `httpClientFactoryKey` | `clientName` | NO |
| parámetro `configureHttpClientKey` | `configureClient` | NO |
| parámetro `pk` | `keyValues` | NO |
| genérico `K` | `TResponse` | NO |
| genérico `TEnumrableResponse` | `TItem` | NO |

### 9.4 Namespace interno

`MoralesLarios.OOFP.HttpClients.ParamsInfo` → `MoralesLarios.OOFP.Http.Requests`

---

## 10. `MoralesLarios.OOFP.Logging` *(antes `…Extensions.Loggers`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `LogMlResultFinal[Async]` | `LogCompletedResult[Async]` | sí |

---

## 11. `MoralesLarios.OOFP.Application` *(antes `…WebServices`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `IGenServiceFp` | `ICrudServiceFp` | NO |
| `GenServiceFp` | `CrudServiceFp` | NO |
| `MlProblemsDetails` | `MlProblemDetails` | NO |
| `AllAsync` | `GetPagedAsync` | NO |
| `UpdateProblemDetailsAsync` | `UpdateAsync` | NO |
| `GetPkValues` | `FormatCompositeKey` | NO |
| `BuildNotFoundPkError` | `BuildEntityNotFoundError` | sí |
| `AddScopedtGenServicesFpWithoutReposGeneral` | `AddScopedCrudServicesFpWithoutRepositories` | sí |
| `AddSingletonGenServicesFpWithoutReposGeneral` | `AddSingletonCrudServicesFpWithoutRepositories` | sí |
| `AddTransientGenServicesFpWithoutReposGeneral` | `AddTransientCrudServicesFpWithoutRepositories` | sí |
| `AddScopedtGenServicesDuplexFpWithoutReposGeneral` | `AddScopedCrudServicesDuplexFpWithoutRepositories` | sí |
| `AddSingletonGenServicesDuplexFpWithoutReposGeneral` | `AddSingletonCrudServicesDuplexFpWithoutRepositories` | sí |
| `AddTransientGenServicesDuplexFpWithoutReposGeneral` | `AddTransientCrudServicesDuplexFpWithoutRepositories` | sí |

> ℹ️ La clase `GenService` (versión no funcional) **se eliminó**. Si la usabas, migra a `CrudServiceFp`.

---

## 12. `MoralesLarios.OOFP.AspNetCore.Results` *(antes `…WebApi`)*

### 12.1 Tipos y records

| Antes | Después | 🔗 |
| --- | --- | --- |
| `MlActionResults` | `MlResults` | NO |
| `MlErrorsDetailsExtensions` | `MlErrorDetailsExtensions` | NO |
| `MlRequestWebExtensions` | `HttpRequestPaginationExtensions` | NO |
| `MlResultWebExtensionsPlus` | `MlResultActionResultExtensions` | NO |
| `MlResultWebExtensions` | `MlResultActionResultExtensionsLegacy` | NO |
| `ProblemDetailsInfo` | `ProblemDetailsData` | NO |

### 12.2 Métodos de lectura de cabeceras

| Antes | Después | 🔗 |
| --- | --- | --- |
| `GetHeaderInfo[Async]` | `GetHeader[Async]` | sí |
| `GetHeaderInfoAsIntNotNegative[Async]` | `GetIntHeader[Async]` | sí |
| `GetHeaderPageInfo[Async]` | `GetPaginationHeaders[Async]` | sí |
| `GetHeaderPageNumber[Async]` | `GetPageNumberHeader[Async]` | sí |
| `GetHeaderPageSize[Async]` | `GetPageSizeHeader[Async]` | sí |
| `GetHeaderPaginationInfo[Async]` | `GetPageRequest[Async]` | sí |

> ⚠️ **ORDEN CRÍTICO**: `GetHeaderInfoAsIntNotNegative` → `GetHeaderPaginationInfo` →
> `GetHeaderPageNumber` → `GetHeaderPageSize` → `GetHeaderPageInfo` → `GetHeaderInfo`.

### 12.3 Métodos de conversión a `IActionResult`

| Antes | Después | 🔗 |
| --- | --- | --- |
| `ToGetPdActionResult[Async]` | `ToOkProblemDetailsResult[Async]` | sí |
| `ToPostPdActionResult[Async]` | `ToCreatedProblemDetailsResult[Async]` | sí |
| `ToPutPdActionResult[Async]` | `ToNoContentProblemDetailsResult[Async]` | sí |
| `ToPatchPdActionResult[Async]` | `ToPatchedProblemDetailsResult[Async]` | sí |
| `ToDeletePdActionResult[Async]` | `ToDeletedProblemDetailsResult[Async]` | sí |
| `ToSimpleRepoPostActionResult[Async]` | `ToCreatedResult[Async]` | sí |
| `ToProblemsDetailsInfo` | `ToProblemDetails` | sí |
| `ContieneCombinacion` | `ContainsAnyKey` | NO |

---

## 13. `MoralesLarios.OOFP.AspNetCore.Controllers` *(antes `…WebControllers`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `SimpleMlControllerBase` | `MlCrudControllerBase` *(ahora `abstract`)* | NO |
| `SimpleMlComplexPkControllerBase` | `MlCompositeKeyCrudControllerBase` *(ahora `abstract`)* | NO |
| `PkParameterAttribute` | `CompositeKeyParameterAttribute` | NO |
| `ConvertDateTime` | `ParseDateTimeInvariant` | **NO** ⚠️ ahora usa `InvariantCulture` |
| `_pkFields` | `_keySelector` | NO |
| `AddWebControllers` | `AddMlControllers` | sí |
| `ConverterTo` | `ConvertToType` | sí |
| `GetPkValues` | `ParseKeyValues` | sí |
| `GetPkValuesErrorMessage` | `BuildKeyParseErrorMessage` | sí |
| `public string Description` + `(string description = null!)` | `public string? Description` + `(string? description = null)` | NO |

### Mensajes de error que cambiaron (si los comparabas en tests)

| Antes | Después |
| --- | --- |
| `"... or is diferent type to PK of ..."` | `"... or is of a different type than PK of ..."` |
| `"{x} isn't null"` | `"{x} must not be null"` |
| `"The type {0} is not soported"` | `"The type {0} is not supported"` |
| `"{x} no be empty"` *(Validation)* | `"{x} must not be empty"` |
| `"{x} no be null"` *(Validation)* | `"{x} must not be null"` |

---

## 14. `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` *(antes `…WebControllers.Cache`)*

| Antes | Después | 🔗 |
| --- | --- | --- |
| `SimpleMlCacheControllerBase` | `MlCachedCrudControllerBase` *(ahora `abstract`)* | NO |
| `SimpleMlComplexCacheControllerBase` | `MlCachedCompositeKeyCrudControllerBase` *(ahora `abstract`)* | NO |
| `PerControllerOutputCachePolicy` | `ControllerScopedOutputCachePolicy` | NO |
| `_pkFields` | `_keySelector` | NO |
| `AddWebControllersCache` | `AddMlControllersOutputCaching` | sí |
| `BypassHeader` | `CacheBypassHeader` | sí |

---

## 15. Documentación renombrada (por si enlazas a los docs del repo)

| Antes | Después |
| --- | --- |
| `1_EmptyToFailed.md` | `1_FailIfEmpty.md` |
| `3_BoolToResult.md` | `3_FailIfFalse.md` |
| `4_Combine.md` | `4_CombineAll.md` |
| `11_BindSaveValueInDetailsIfFaildFuncResultAsync.md` | `11_BindSaveValueInDetailsIfFailedFuncResultAsync.md` |
| `__Doc/Bucle/` | `docs/loops/` |
| `__Doc/Types/` | `docs/core-types/` |
| `__Doc/` | `docs/` |

---

## 16. Resumen de riesgo

| Categoría | Nº aprox. | Efecto si no se migra |
| --- | --- | --- |
| Paquetes renombrados | 14 | `NU1101` / `CS0246` — no restaura |
| Namespaces renombrados | 13 + 4 internos | `CS0234` / `CS0246` |
| Renombrados **sin puente** | **231** | **Error de compilación** |
| Renombrados **con puente** | **187** | Aviso `CS0618` — se eliminan en 3.0.0 |
| Cambios semánticos | 9 bloques | **Compila pero se comporta distinto** ⚠️ |
| Textos / mensajes | 8 | Tests de aserción sobre cadenas |

**Total documentado: 888 cambios** en 14 fases. El registro completo está en
`Cambios Realizados/Renombrados-v2.0.0.md` del repositorio de la librería.
