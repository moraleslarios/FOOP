# 02 · Auditoría de los documentos complementarios

> **Comprobado contra:** commit `dab05b4` · 2026-11-12 · auditoría estática
> **Leyenda:** ✅ aplicado · 🟨 parcial (destino distinto, o solo existe el puente `[Obsolete]`) · ⬜ pendiente
> **Ruptura:** **C** = de compilación · **S** = silenciosa · **A** = aditiva · **—** = ninguna

## Índice

1. [Análisis de nomenclatura de `ValueObjects` (N1-N21)](#1-análisis-de-nomenclatura-de-valueobjects-n1-n21)
2. [`Consejos-Nomenclatura.md` y `Plan-Renombrados-2.0.0.md`](#2-consejos-nomenclaturamd-y-plan-renombrados-200md)
3. [Consejos de mejora de `MlResult` (MR1-MR23)](#3-consejos-de-mejora-de-mlresult-mr1-mr23)
4. [Revisión Map / Bind / ExecSelf](#4-revisión-map--bind--execself)
5. [`PendingTasks-legacy.md`](#5-pendingtasks-legacymd)
6. [Profesionalización · ingeniería y calidad](#6-profesionalización--ingeniería-y-calidad)
7. [Profesionalización · diseño de API y producto](#7-profesionalización--diseño-de-api-y-producto)

---

## 1. Análisis de nomenclatura de `ValueObjects` (N1-N21)

**Resultado:** 1 resuelto · 10 parciales · 10 pendientes. Destino de todo lo abierto: [G4](../../Pendientes/general_old_version/Epica-G4-ValueObjects-Nomenclatura-y-Diseno.md).

| N | Propuesta | Estado | Evidencia | Qué falta | Ruptura |
|---:|---|:---:|---|---|:---:|
| 1 | `Lenght` → `Length` | 🟨 | `StringLengthBetween.Obsolete.cs:20` | Parámetros ya corregidos; el nombre antiguo sobrevive como puente | C |
| 2 | `BuildErrorMessIdLite` → `BuildErrorMessage` | ⬜ | `IdLite.cs:27` | El destino no existe | C |
| 3 | `Id.Fromdouble` / `Id.Bydouble` | ✅ | `Cambios Realizados/Renombrados-v2.0.0.md:274` | `Id.cs` estaba fuera de la compilación y se eliminó | — |
| 4 | `Mail.EndpointPattern` → `EmailPattern` | 🟨 | `Mail.cs:15` | Se usa `ValidationPattern`; `EndpointPattern` queda obsoleto | C |
| 5 | Mensaje de `Id` «greater than 0» | ⬜ | `IdLite.cs:27` («can be mayor 0») | Corregir el texto | S |
| 6 | Unificar `FOOP` / `OOFP` | 🟨 | `MoralesLarios.OOFP.sln` | Todo normalizado salvo el directorio raíz `FOOP` | — |
| 7 | `Void` + `EmptyValue` → `Unit` | 🟨 | `Void.cs:10`, `EmptyValue.cs:10` | `Empty` pasó a `EmptyValue`; `Void` sigue y `Unit` no existe | C |
| 8 | `IdLite` → `Int32Id`; `Id` → `Int64Id` | 🟨 | `IdLite.cs:10` | `IdLite` sigue; `Id` ya no existe, replantear `Int64Id` | C / A |
| 9 | `Mail` → `EmailAddress` | ⬜ | `Mail.cs:10` | No existe | C |
| 10 | `Endpoint` → `HttpsUrl` | ⬜ | `Endpoint.cs:10` | No existe; el patrón acepta host HTTPS y puerto, sin ruta | C |
| 11 | `Key` → `EntityKey`; `Name` → `DisplayName` | 🟨 | `ServiceKey.cs:10`, `MemberName.cs:10` | Se eligieron destinos más específicos | C si se vuelven a cambiar |
| 12 | `RangeEnumValueObject<T>` → `EnumName<T>` | 🟨 | `EnumRangeValueObject.cs:11` | Se invirtió el orden, no el concepto | C |
| 13 | Tipos de longitud de cadena | 🟨 | `StringLengthBetween.cs:10`, `StringMaxLength.cs:10` | `StringBetweenLength` cambió; `StringMinLength`/`StringMaxLength` no | C |
| 14 | `MoreThan` / `Between` / `NotNegative` | 🟨 | `IntMoreThan.cs:10`, `IntNonNegative.cs:10` | `NotNegative`→`NonNegative` hecho; `MoreThan`/`Between` siguen | C |
| 15 | `NotEmptyString` → `NonBlankString` | ⬜ | `NotEmptyString.Obsolete.cs:18` | No existe | C |
| 16 | `RegexValue` → `RegexString` | ⬜ | `RegexValue.cs:12` | No existe | C |
| 17 | Familia `StringAs*` → `NumericString<T>` | ⬜ | `StringAsInt.cs:12`, `StringAsNumeric.cs:11` | Base y 11 derivados intactos; **44 `CS0108` deducidos, sin build que lo confirme** | C |
| 18 | Familias numéricas → genéricos `INumber<T>` | ⬜ | `IntMoreThan.cs:10` | Siguen las clases por tipo | C |
| 19 | `ValueObject<TValue>` → abstracta `SingleValueObject<TValue>` | ⬜ | `ValueObject.cs:96` | Sigue pública y concreta | C |
| 20 | Unificar factorías (`From`/`TryCreate`/`IsValid`) | 🟨 | `StringAsInt.cs:44`, `StringAsInt.Obsolete.cs:18` | `CreateFrom…` lanza y `From…` devuelve `MlResult`; quedan puentes `By…` y `Void.Create()` | C / S |
| 21 | `length` → `exclusiveMinimum`/`min`/`max` | ⬜ | `IntBetween.cs:38`, `IntMoreThan.cs:35` | Parámetros públicos sin renombrar | C (argumentos con nombre) |

**Ya hecho que figura pendiente:** la estrategia de factorías, los renombrados de proyectos, `PageRequest`/`PagedResult`,
los destinos de `Key`/`Name`, `EnumRangeValueObject` y los tipos de FileSystem.

**Rehacer o descartar:** N8 respecto a `Id` (no hay API publicada que migrar); reconciliar `Unit` con `EmptyValue`/`Void`;
elegir destino único para `Mail`/`Endpoint`; N17 exige antes una **medición** de los `CS0108` reales.

---

## 2. `Consejos-Nomenclatura.md` y `Plan-Renombrados-2.0.0.md`

El renombrado se ejecutó (inventario: **904 cambios**, 885 de nomenclatura y 19 de contrato; el plan preveía 16 de contrato).
Los dos documentos **no coinciden entre sí**: `Consejos-Nomenclatura.md` (L699) da todos los niveles por terminados;
`Plan-Renombrados-2.0.0.md` (L150) conserva cuatro criterios de cierre sin marcar.

| Bloque | Estado | Matiz |
|---|:---:|---|
| C0 · solución, proyectos y carpetas | 🟨 | Falta el directorio raíz; `PendingTasks` no consta convertido en *issues* |
| C1 · erratas | 🟨 | Persisten en puentes; el mensaje de `IdLite` sigue mal |
| C2 · núcleo (`MlResult`, `MlErrorDetails`, constantes) | 🟨 | Destinos aplicados; puentes vivos |
| C3 · `ValueObjects` | 🟨 | Varios destinos difieren de la propuesta (ver §1) |
| C4 · `Validation` | 🟨 | `ValidateAsync` continúa; la propuesta citaba `MlSelfValidatable` |
| C5 · Shared, FileSystem, Configuration, Logging | ✅ / 🟨 | Algunas propuestas de miembros no coinciden con el destino registrado |
| C6-C10 · EFCore, Application, Results, Controllers, Http | ✅ / 🟨 | Implementación amplia; persisten puentes |
| P0-P13 · fases del plan | ✅ / 🟨 | CSV, línea base y `MlRename` existen; no consta verificación final |
| Criterio A · build sin `CS0618` interno | ⬜ | Sin comprobar |
| Criterio B · tests iguales a la línea base | ⬜ | Sin comprobar contra la línea base original |
| Criterio C · inventario sin nombres antiguos | ⬜ | Los antiguos viven en `*.Obsolete.cs` |
| Criterio D · docs sin nombres antiguos | ⬜ | Sin barrido completo |

Destino: [G5](../../Pendientes/general_old_version/Epica-G5-Cierre-Renombrado-y-Puentes.md).

---

## 3. Consejos de mejora de `MlResult` (MR1-MR23)

**Resultado de la auditoría original:** 7 ✅ · 10 🟨 · 6 ⬜. Seguimiento posterior: [G3 completada](../Core/Epica-G3-Nucleo-MlResult-Robustez.md); el resto se mantiene en el [backlog](../../Pendientes/general_old_version/04-Backlog-3.0-y-Descartados.md).

| MR | Asunto | Estado | Evidencia | Qué falta | Ruptura |
|---:|---|:---:|---|---|:---:|
| 1 | `AddDetails` | ✅ | `AddMetadataRange` en `MlErrorDetailsActions.cs:327` | Actualizar el documento | — |
| 2 | `AddDetail` | 🟨 | `AddMetadataOrThrow:305` copia y lanza; `WithMetadata:324` sobrescribe | Declarar la política recomendada | S |
| 3 | Colisiones en `Merge` | ✅ | `MlErrorDetailsActions.cs:85`; tests de colisión | — | — |
| 4 | Conversiones implícitas ambiguas (`MlResult<MlError>`) | ⬜ | `MlResult.cs:412` | Test de compilación y política | C |
| 5 | Secuencia `Ex` | ✅ | `MlErrorDetails.Metadata.cs:178`; tests `NextExceptionKey_*` | — | — |
| 6 | Igualdad estructural | ✅ | `MlErrorDetails.Metadata.cs:370` | — | — |
| 7 | `Errors` diferido | 🟨 | `MlErrorDetails.cs:47` materializa | La propiedad sigue como `IEnumerable` | C |
| 8 | Fallo sin errores | ⬜ | `MlResult.cs:35`; `MlErrorDetails.cs:47` | Definir y probar el invariante de fallo no vacío | S |
| 9 | `GetDetail<T>` y `null` | 🟨 | `MlResultErrorDetailsExtensions.cs:28`; `TryGetMetadata:255` | Un `null` no se distingue de clave ausente o tipo incorrecto | S |
| 10 | Detalles mutables | 🟨 | `MetadataView`, `TryGetException`… | `Metadata` sigue siendo un `Dictionary` público | C |
| 11 | Acceso seguro al resultado | 🟨 | `MlResult.TryGet.cs:40` | `ValueOrDefault` y `Deconstruct` no existen | — |
| 12 | `InvalidProgramException` | ⬜ | `MlResultExtensions.cs:360` | Excepción adecuada que transporte los detalles | S |
| 13 | `ToString()` con valor `null` | ✅ | `ConstantsTests.cs:159` | — | — |
| 14 | `MlErrorDetails.ToString()` vacío | ⬜ | `MlErrorDetails.cs` | Representación explícita o impedir ese estado | S |
| 15 | Erratas públicas | ✅ | `*.Obsolete.cs` | — | — |
| 16 | Genéricos de `MergeErrorsDetails` | ⬜ | `MlErrorDetailsActions.cs:150` | Valorar simplificación con puente | C |
| 17 | `class` frente a `record` | 🟨 | `MlErrorDetails.cs:47` | Revisar nulabilidad y sellado | C |
| 18 | `Discard` con referencia nula | 🟨 | `MlResult.cs:12` delega en `_` y `default(T)!` | Tipo `Unit` o documentar | S |
| 19 | Asignaciones por resultado válido | 🟨 | `MlErrorDetails.Metadata.cs` | **Medir antes de optimizar** | — |
| 20 | Serialización | 🟨 | Adaptación a `ProblemDetails` en ASP.NET Core | Contrato JSON seguro, sin excepciones en bruto | — |
| 21 | Retorno descartado | ⬜ | 0 resultados de `MustUse…` | Analizador, solo si compensa | — |
| 22 | Mensajes internos | 🟨 | `MlResultExtensions.cs:360` con literal inglés | Centralizar | S |
| 23 | Nulos y código muerto en `Merge` | ✅ | `MlErrorDetailsActions.cs:85` | — | — |

---

## 4. Revisión Map / Bind / ExecSelf

**Resultado:** 12 ✅ · 3 🟨 · 1 ⬜ (el documento declara 10 marcadas y 6 abiertas; de esas seis, tres ya están resueltas).

| Asunto | Estado | Evidencia |
|---|:---:|---|
| Recursión infinita en `MapAlwaysAsync` | ✅ | `MlResultBindMapFixesTests.cs:423` |
| Paridad `ExecSelfIfFailWithoutExceptionAsync` | ✅ | `MlResultExecSelfFixesTests.cs:28` |
| Valor extraído en `TryMap`/`TryBind`/`…WithValue` | ✅ | `MlResultBindMapFixesTests.cs` |
| Restricciones `TException` | ✅ | `MlResultBindExtensions.cs:3967` |
| `BindEx…` → `TryBind…` | ✅ | `oofp-upgrade-2.renames.md:145` |
| Nulabilidad de `CombineAsync`, erratas internas, código muerto, `null!` en mensajes | ✅ | registrados en las épicas Core |
| R1 · erratas públicas | ✅ | puentes `*.Obsolete.cs` |
| R4b · paridad `TryMapAlways` · R4c · getter asíncrono en Bind | ✅ | `MlResultMapExtensions.cs:7912`, `:2337` |
| R2 · inferencia de `TException` | 🟨 | falta un test que omita los genéricos |
| R4a · `MapAlways` valida el retorno pero no el delegado | 🟨 | `MlResultMapExtensions.cs:7609` |
| R4d · indentación | 🟨 | las rutas originales quedaron obsoletas al trocear las clases |
| R3 · `null` ambiguo entre `string` y `Func<Exception,string>` | ⬜ | sobrecargas de las familias `Try*` |

Destino: [G3 completada](../Core/Epica-G3-Nucleo-MlResult-Robustez.md).

---

## 5. `PendingTasks-legacy.md`

**Resultado:** 14 ✅ · 7 🟨 · 3 ⬜. Es una lista de ideas con nombres 1.x; la mayor parte **ya existe** con otro nombre.

| Idea | Estado | Hoy |
|---|:---:|---|
| Métodos `Ex` de `BindIfFail`, `MapIfFail`, `…SaveValue…` | ✅ | `TryBindIfFail`, `TryMapIfFail`, `TryBindSaveValueInDetailsIfFailed` |
| `HasException`, `HasValue`, `ReplaceValue` | ✅ | `ContainsException`/`TryGetException`, `ContainsValueMetadata`/`AddValueMetadata`, `WithMetadata` |
| `*IfFailWithValue`, `*IfException`, `*IfFailWithoutException` | ✅ | familias `Bind`/`Map`/`ExecSelf` con esos sufijos (la nomenclatura vigente es `…IfFailWithException`) |
| `BindIf`, `MapIf`, `ExecSelfIf` | ✅ | existen |
| `BoolToMlResult` | ✅ | `FailIfFalse` |
| Trazas `IfException`/`IfFailWithoutException` | ✅ | `LogMlResultIfFailWithException` y `…WithoutException` |
| `ExecSelf` unificado | ✅ | `MlResultExecSelfExtensions.cs:42` |
| Sobrecarga con `MlErrorDetails` en `EnsureFp` | ✅ | `EnsureFp.That/ThatAsync` |
| FluentValidation → `MlResult` | ✅ | `ValidateWithFluentValidation` |
| `LogMlFinish` con tuplas y *builders* | 🟨 | `LogMlResult` con tuplas ya existe; un *helper* sería redundante |
| Registrar al crear el `MlResult` | 🟨 | composición con `.LogMlResult…`; no acoplar `Core` a `Logging` |
| `AddResult` para `Bind` y tuplas | 🟨 | existen `CompleteWith*`; precisar el caso no cubierto |
| Métodos asíncronos de `MlErrorDetailsActions` | 🟨 | no volver asíncrono lo puro |
| `AddValue`, `AddValueIfFail`, `AddValueIfValid` | 🟨 | falta `AddValueIfValid` y su semántica |
| Unificar `CompleteWithDetailsValueIfFailAsync` | 🟨 | solo si hay duplicación concreta |
| `ChangeValueReturn` / `MakeMap` | 🟨 | existe `ChangeReturnResult`; los nombres no |
| `BindSelf` | ⬜ | se compone con `Bind` + `Map` |
| `CompleteWith*` → `TransformData` | ⬜ | renombrado sin ganancia semántica |
| `MakeBind` | ⬜ | diferencia con `Bind` sin definir |

Destino de las ideas abiertas: [backlog y descartes](../../Pendientes/general_old_version/04-Backlog-3.0-y-Descartados.md).

---

## 6. Profesionalización · ingeniería y calidad

**Resultado:** 3 ✅ · 7 🟨 · 7 ⬜. Destino: [G1](../../Pendientes/general_old_version/Epica-G1-Higiene-Repositorio-CI-y-Publicacion.md) y [G6](../../Pendientes/general_old_version/Epica-G6-Pruebas-Calidad-y-Rendimiento.md).

| Tarea | Estado | Evidencia | Falta |
|---|:---:|---|---|
| Una sola solución, sin `* - copia.sln` | ✅ | solo `MoralesLarios.OOFP.sln` | — |
| `.gitignore` y `bin/obj` fuera del índice | ✅ | `git ls-files` → **0** ficheros `bin/obj` versionados | — |
| Alinear dependencias con el *framework* | ✅ | proyectos en `net11.0`, paquetes `11.0.0`, `FrameworkReference` | actualizar la guía |
| Marca y reorganización `src/` + `tests/` | 🟨 | pruebas `*.Tests.Unit` bajo `src/`; raíz `FOOP` | decisión de marca (G5) |
| `.editorconfig`, `.gitattributes`, UTF-8 | 🟨 | `.editorconfig` con 4 diagnósticos; **sin** `.gitattributes` | charset y EOL |
| `Directory.Build.props` y *warnings* | 🟨 | solo `EnforceCodeStyleInBuild`; sin `TreatWarningsAsErrors` | centralizar |
| Leyes algebraicas y paridad sync/async | 🟨 | pruebas puntuales; sin FsCheck/CsCheck | suite generativa (G6) |
| Cobertura con umbral | 🟨 | `coverlet.collector` en varios proyectos | umbral e informe |
| Versión por etiquetas y SemVer | 🟨 | `<Version>2.0.0</Version>` en cada `.csproj` | política y automatización |
| Seguridad y gobernanza | 🟨 | `src/CONTRIBUTING.md` y plantilla de PR | `SECURITY.md`, código de conducta, plantillas de *issue* |
| `PendingTasks.txt` a *issues*; `lutconfig` | ⬜ | `src/MoralesLarios.OOFP.lutconfig` sigue en `src/` | decidir |
| `Directory.Packages.props`, NuGet Audit, Dependabot | ⬜ | no existen | centralizar |
| Analizadores y `BannedSymbols.txt` | ⬜ | ninguno referenciado | selección gradual |
| `PublicApiAnalyzers` | ⬜ | sin `PublicAPI.*.txt` | baseline |
| Integración reproducible en CI | ⬜ | `appsettings.test.json` con instancias locales y credenciales | G1-US-01, G6-US-04 |
| `LICENSE`, `CHANGELOG`, metadatos NuGet, SourceLink | ⬜ | ninguno existe | G1-US-02 |
| `ci.yml`, CodeQL, publicación | ⬜ | único *workflow*: `prompt-migracion.yml` | G1-US-04 |

**Omisiones del documento original:** `global.json` (fijar SDK 11 y política de *roll-forward*), CI de compilación y tests
para `net11.0`, credenciales de prueba versionadas, avisos `NU1510` (referencias redundantes que el SDK 11 ya incluye
en 7 proyectos) y el criterio de qué hacer con la política de soporte de los paquetes antes de publicar.

---

## 7. Profesionalización · diseño de API y producto

**Resultado:** 0 ✅ · 12 🟨 · 8 ⬜. Es un documento **estratégico**: casi nada es una tarea acotada de 2.0.0.
Lo que sí lo es se absorbe en G1/G6; el resto pasa al [backlog 3.0](../../Pendientes/general_old_version/04-Backlog-3.0-y-Descartados.md).

| Línea de trabajo | Estado | Matiz verificado |
|---|:---:|---|
| Núcleo mínimo e IntelliSense avanzado | ⬜ | Hay muchos `EditorBrowsable(Never)` en puentes; no hay categoría «avanzado» |
| Generar sobrecargas / reducir combinatoria | ⬜ | Sin generador; la superficie sigue extensa |
| Dividir archivos grandes | 🟨 | `MlResultMapExtensions.cs` supera las 7.900 líneas |
| Sincronía: `.Result`, `ConfigureAwait(false)` | 🟨 | `.Result` en `MlResultCollections.cs`; sin `ConfigureAwait(false)` en `Core` |
| *Fake async*, `ValueTask`, `IAsyncEnumerable` | 🟨 | Sin `ValueTask` ni `IAsyncEnumerable` |
| `CancellationToken` en la E/S | 🟨 | **Ya existe**: 115 declaraciones `public async Task…CancellationToken` en 13 ficheros; el documento afirma lo contrario |
| Errores tipados `Code/Target/Severity` | ⬜ | `MlError` solo tiene `Message`; VAL añade `Target` |
| Claves de detalle únicas | 🟨 | `WebErrorDetailsKeys` existe |
| Internacionalización | ⬜ | Sin `.resx` ni proveedor; MSG US-01 propone la infraestructura |
| Rendimiento (benchmarks, asignaciones) | ⬜ | Sin BenchmarkDotNet |
| Interoperabilidad (`Deconstruct`, `SelectMany`, `IParsable`) | ⬜ | Sin implementación |
| Arquitectura y pruebas de arquitectura | 🟨 | Existe `Shared`; sin NetArchTest |
| DI, `IOptions`, validación al arranque | 🟨 | `IOptions<MlUnitOfWorkDiagnostics>`; sin `ValidateOnStart` general |
| Opciones neutras (`about:blank`, límites, *timeout*) | ⬜ | Sin opciones generales |
| Observabilidad (`ActivitySource`, `Meter`, `[LoggerMessage]`) | 🟨 | `EventId` y plantillas sí; sin telemetría |
| *Value objects* y validación comunes | 🟨 | Sin base única con `static abstract` |
| Capa web: *framework* y rutas | 🟨 | `FrameworkReference` hecho; `DELETE` con cuerpo sigue (REST US-05) |
| `ProblemDetails` moderno, Minimal API, ETag | ⬜ | Sin `IExceptionHandler`, `IEndpointFilter` ni ETag |
| Documentación, ejemplos, plantilla, DocFX | 🟨 | `Sample` de EFCore y Logging; sin DocFX ni plantilla |
| Gobernanza y hoja de ruta | 🟨 | Ver §6 |
