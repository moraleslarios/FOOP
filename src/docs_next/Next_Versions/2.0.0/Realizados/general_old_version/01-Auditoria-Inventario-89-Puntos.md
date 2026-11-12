# 01 · Auditoría del inventario de 89 puntos

> **Fuente:** [`Mejoras-Prioridad-Critica-y-Alta.md`](../../Realizados/Temas%20Pendientes/Mejoras-Prioridad-Critica-y-Alta.md) (1-37) · [`Mejoras-Prioridad-Media.md`](../../Realizados/Temas%20Pendientes/Mejoras-Prioridad-Media.md) (38-63) · [`Mejoras-Prioridad-Baja.md`](../../Realizados/Temas%20Pendientes/Mejoras-Prioridad-Baja.md) (64-89)
> **Comprobado contra:** commit `dab05b4` · 2026-11-12 · auditoría estática (código y tests existentes)
> **Leyenda:** ✅ el defecto ya no existe · 🟨 parcialmente cerrado · ⬜ sigue presente
> **Destino:** `Hecho` = nada que planificar · `REST`, `SER`, `SRV`, `VAL`, `ERR`, `MSG`, `COH` = épica pendiente ya existente · `G1`…`G6` = épica nueva de esta carpeta

## Índice

1. [Resumen](#1-resumen)
2. [Tabla punto a punto](#2-tabla-punto-a-punto)
3. [Correcciones al inventario original](#3-correcciones-al-inventario-original)
4. [Equivalencias de nombres](#4-equivalencias-de-nombres)

---

## 1. Resumen

| Bloque | ✅ | 🟨 | ⬜ | Observación |
|---|---:|---:|---:|---|
| 1-16 · Crítica | 14 | 2 | 0 | Los dos parciales son decisiones de diseño de `ValueObjects` |
| 17-37 · Alta | 9 | 7 | 5 | Los 12 abiertos se concentran en `Http`, controladores y `CrudServiceFp` |
| 38-63 · Media | 4 | 5 | 17 | Cubiertos por épicas existentes **sin ningún criterio marcado** |
| 64-89 · Baja | 15 | 10 | 1 | La limpieza de código muerto y buena parte de las erratas están hechas |

Reparto de lo no resuelto (47 puntos):

| Destino | Puntos |
|---|---|
| Épicas ya existentes | 33 (REST 9 · SER 6 · SRV 3 · VAL 4 · ERR 2 · MSG 4 · COH 5; el 82 se reparte entre COH y SRV y se cuenta una vez) |
| **G2** (nueva) | 17, 18, 19, 23, 25, 26, 27, 29, 30, 31, 33, 38 (12) |
| **G4** (nueva) | 6, 10 (2) |

La retirada de los puentes `[Obsolete]` de INV-72 e INV-73 sigue en ERR hasta que **G5-US-02** decida su política.

> «INV-nn» = punto de este inventario. «MIG-nn» = punto del prompt de migración (numeración distinta).

---

## 2. Tabla punto a punto

### 2.1 Crítica · 1-16

| INV | Título | Estado | Evidencia actual | Destino |
|---:|---|:---:|---|---|
| 1 | Fusión de errores sin `return` | ✅ | `Core/Types/MlResultCollections.cs:900`; test `MlResultCollectionsTests.cs:603` | Hecho |
| 2 | `MapAlwaysAsync` no esperaba el origen | ✅ | `MlResultMapExtensions.cs:7655`; el test solo cubre la no recursión | Hecho · test de orden en G6 |
| 3 | Value objects «no negativos» rechazaban 0 | ✅ | `DecimalNonNegative.cs:34`; test `NotNegativeDecimalDoubleTests.cs:15` | Hecho |
| 4 | Límite mutable de `IntNonNegative` | ✅ | `IntNonNegative.cs:12` (`const` privado) | Hecho |
| 5 | Mensaje mínimo/máximo invertido | ✅ | `StringLengthBetween.cs:31`; test `StringLengthBetweenTests.cs:22` | Hecho |
| 6 | `length` ignorado en `Key`/`Name` | 🟨 | `ServiceKey.cs:33` usa `length` como mínimo; **no hay máximo** (el test confirma que una cadena larga es válida) | G4-US-07 |
| 7 | Constructores públicos de `Key`/`Name` | ✅ | `ServiceKey.cs:17` privado; test `ServiceKeyTests.cs:47` | Hecho |
| 8 | Rango de enum comentado | ✅ | `EnumRangeValueObject.cs:11` activo; tests presentes | Hecho |
| 9 | Encadenamiento de rutas existentes | ✅ | `ExistingFilePath.cs:51` (`Map` tras `Bind`); tests de fichero y directorio | Hecho |
| 10 | Excepciones al crear rutas existentes | 🟨 | `ExistingFilePath.cs:18` sigue lanzando en la vía directa; `FromPath` devuelve `MlResult` | G4-US-07 |
| 11 | Guardas de `ValidateAsync` | ✅ | `DataAnnotationsValidator.cs:28`; test `DataAnnotationsValidatorTests.cs:10` | Hecho |
| 12 | Colisión de `ValidateObject` | ✅ | solo queda el alias obsoleto `Extensions.Obsolete.cs:20` | Hecho |
| 13 | Clave HTTP nula | ✅ | `Http/RegisterServices.cs:125`; test `RegisterServicesTests.cs:51` | Hecho |
| 14 | Servicio compuesto sin cliente nombrado | ✅ | `Http/RegisterServices.cs:119`; el test comprueba la clave, no una petición real | Hecho · prueba extremo a extremo en G6 |
| 15 | `Created` devuelto en fallo | ✅ | `MlResultActionResultExtensionsLegacy.cs:257`; test `…LegacyTests.cs:57` | Hecho |
| 16 | 404 convertido en 500 | ✅ | `Application/Helpers/Extensions.cs:17`; test `MlResultActionResultExtensionsTests.cs:40` | Hecho |

### 2.2 Alta · 17-37

| INV | Título | Estado | Evidencia actual | Destino |
|---:|---|:---:|---|---|
| 17 | Cuerpo remoto en los mensajes de error | 🟨 | `MlResponseWebExtensions.cs:18,25`: truncado a 2.048, pero **incluirlo sigue activado por defecto** | G2-US-08 |
| 18 | Bloqueo (`.Result`) al leer el error HTTP | 🟨 | `MlResponseWebExtensions.cs:33` ya es asíncrono; el puente síncrono (`…Obsolete.cs:31`) bloquea | G2-US-08 · G5-US-02 |
| 19 | Mensajes internos expuestos por los controladores | ⬜ | `MlCrudControllerBase.cs:43,47` devuelve texto de excepción e identificadores | G2-US-02 |
| 20 | `BuildServiceProvider` durante el registro | ✅ | `EntityFrameworkCore/RegisterServices.cs:26`; test de integración `RegisterServicesIntegrationTests.cs:53` | Hecho |
| 21 | El repositorio liberaba un contexto ajeno | ✅ | `EfRepository` ya no es `IDisposable`; test `EfRepositoryTests.cs:12` | Hecho (MIG-63) |
| 22 | Registro `Singleton` cautivo | ✅ | solo `Scoped`; test `RegisterServicesTests.cs:16` | Hecho (MIG-62) |
| 23 | Selector de clave no resoluble por DI | ⬜ | `MlCompositeKeyCrudControllerBase.cs:10` exige `Func<TEntity, object[]>` en el constructor | G2-US-03 (enlazada con REST US-01) |
| 24 | `AddWebControllers` vacío | ✅ | `AspNetCore.Controllers/RegisterServices.cs:16` + puente que avisa de que es *no-op* | Hecho |
| 25 | Parseo de fecha no ISO | 🟨 | `Controllers/Helpers/Extensions.cs:84,97`: cultura invariante, pero admite varios formatos ambiguos | G2-US-04 |
| 26 | Conversión de identificadores incompleta | ⬜ | `Extensions.cs:22,34`: faltan `Guid`, `DateOnly`, `TimeOnly`, `enum`; numéricos con cultura actual; puede devolver `null` | G2-US-04 |
| 27 | Composición de URL | 🟨 | `HttpApiInvoker.cs:419` usa `Uri`, **sin escapar** cada segmento variable | G2-US-05 |
| 28 | Clave compuesta en la URL | 🟨 | `CompositeKeyRestClientFp.cs:146` formatea en invariante; `Extensions.cs:119` hace `Split(',')` sin decodificar | REST US-02 |
| 29 | Cabeceras de paginación pegadas al cliente | 🟨 | `HttpApiInvoker.cs:477` ya usa el mensaje; `MlHttpRequestExtensions.cs:24` aún muta `DefaultRequestHeaders` | G2-US-06 |
| 30 | Clave inválida produce estados incoherentes | ⬜ | `MlCrudControllerBase.cs:43,85`: GET→404, PUT/DELETE→500 | G2-US-01 |
| 31 | `Location` del `201 Created` | 🟨 | `MlResultActionResultExtensions.cs:83`: sustituido por `/`, pero el POST base no apunta al recurso (`MlCrudControllerBase.cs:61`) | G2-US-07 |
| 32 | `ProblemDetails.type` | ✅ | `WebErrorDetailsKeys.cs:14` (`about:blank`) | Hecho (MIG-32) · test en G6 |
| 33 | Doble consulta al actualizar | ⬜ | `CrudServiceFp.cs:167` busca y `EfUpdateRepository.cs:135` vuelve a hacer `FindAsync` | G2-US-09 |
| 34 | Referencia MVC 2.x | ✅ | `FrameworkReference Microsoft.AspNetCore.App` en Results y Controllers | Hecho (MIG-34) |
| 35 | Guarda de borrado asíncrono | ✅ | `EfDeleteRepository.cs:154`; test `EfDeleteRepositoryTests.cs:24` | Hecho |
| 36 | Conteo paginado ignoraba el filtro | ✅ | `EfPagedReadRepository.cs:120` | Hecho · test síncrono **con filtro** en G6 |
| 37 | `OrderBy` después de `Skip/Take` | ✅ | `EfPagedReadRepository.cs:114,116`; tests de paginación | Hecho |

### 2.3 Media · 38-63

| INV | Título | Estado | Evidencia actual | Destino |
|---:|---|:---:|---|---|
| 38 | `TryGetLast` materializa todo | 🟨 | `EfReadRepository.cs:105,123` conserva los métodos antiguos; existe `TryGetLastBy` limitado a una fila | G2-US-10 |
| 39 | Conflicto de seguimiento en `TryUpdate` | ✅ | `EfUpdateRepository.cs:78`; test de integración `…UpdateRepositoryIntegrationTests.cs:33` | Hecho |
| 40 | Sin unidad de trabajo | ✅ | `IMlUnitOfWork.cs:25`; épica archivada | Hecho (MIG-71, 72) |
| 41 | `GetPagedAsync` no pagina | ⬜ | `CrudServiceFp.cs:27,33` llama a `TryAllAsync` | SRV |
| 42 | Peticiones y respuestas HTTP sin liberar | ⬜ | `HttpApiInvoker.cs:31`; `Dispose` comentado en `:310` | SER US-06 |
| 43 | `Activator.CreateInstance<TEntity>()` | ⬜ | `Controllers/Helpers/Extensions.cs:143` | REST US-01 |
| 44 | Serialización descartada | ⬜ | `HttpApiInvoker.cs:329,333` | SER US-03 |
| 45 | Sin `JsonSerializerOptions` | ⬜ | `ApiCallOptions.cs:11`, `PagedApiCallOptions.cs:16` | SER US-01, US-02 |
| 46 | Estado HTTP solo en el texto | ⬜ | `MlResponseWebExtensions.cs:70` | SER US-04, US-05 |
| 47 | Clase e interfaz divergen | 🟨 | `RestClientFp.cs:26` ya tiene `GetClientName`; `IRestClientFp.cs:44` no | SER US-07, US-08 |
| 48 | Rutas y nombres asimétricos | ⬜ | `RestClientFp.cs:51,89` | REST US-03, US-04 |
| 49 | Ruta distinta por verbo | ⬜ | `MlCrudControllerBase.cs:39,76` | REST US-03 |
| 50 | `DELETE` con cuerpo | ⬜ | `MlCrudControllerBase.cs:129` | REST US-05 |
| 51 | Bases de controlador publicables | 🟨 | `MlCrudControllerBase.cs:14` ya `abstract`; falta el test de catálogo de endpoints | REST US-08 |
| 52 | OpenAPI sin respuestas declaradas | ⬜ | `Task<IActionResult>` y 0 `[ProducesResponseType]` | REST US-06 |
| 53 | `MapAsync` en borrados | ⬜ | `CrudServiceFp.cs:275,577` | SRV |
| 54 | Mensaje con `TEntity` en vez de `TResponse` | ⬜ | `CrudServiceFp.cs:444,529` | SRV |
| 55 | Literales de cabecera duplicados | ✅ | `Shared/Web/MlPaginationHeaders.cs:11` | Hecho · test de contrato en G6 |
| 56 | Falta restricción CRTP | ⬜ | `IMlSelfValidatable.cs:14` solo `where T : class` | VAL |
| 57 | `ValidationContext` sin servicios | ⬜ | `DataAnnotations/Helpers/Extensions.cs:22` | VAL |
| 58 | Asincronía simulada | ⬜ | `DataAnnotationsValidator.cs:28,57` (`ToAsync()`) | VAL |
| 59 | Se descartan los `MemberNames` | ⬜ | `Extensions.cs:39`; `MlError.cs:16` no tiene `Target` | VAL |
| 60 | Clave compuesta partida de forma ingenua | 🟨 | `Extensions.cs:119` `Split(',')`; ya se valida el número de componentes | REST US-02 |
| 61 | Atributo de clave que nadie lee | ⬜ | `CompositeKeyParameterAttribute.cs:12`, 12 usos | REST US-07 |
| 62 | Helper de registro de clave compuesta | ✅ | `Http/RegisterServices.cs:67,83`; test `RegisterServicesTests.cs:67` | Hecho |
| 63 | Defectos de `MlHttpRequestExtensions` | 🟨 | líneas 134, 160, 180; el test `MlHttpRequestExtensionsTests.cs:28` está **vacío** | SER US-09 · test en G6 |

### 2.4 Baja · 64-89

| INV | Título | Estado | Evidencia actual | Destino |
|---:|---|:---:|---|---|
| 64 | Interfaz con el cuerpo comentado | ✅ | `IEfRepository.cs:11` limpio (el inventario citaba el fichero equivocado) | Hecho |
| 65 | `Constants.cs` vacía | ✅ | `Core/Helpers/Constants.cs:28` con contenido | Hecho |
| 66 | `GenService.cs` vacío | ✅ | sustituido por `CrudServiceFp.cs` | Hecho |
| 67 | Rango enum comentado | ✅ | ver INV-8 | Hecho |
| 68 | Código comentado del controlador | ✅ | `MlCompositeKeyCrudControllerBase.cs` limpio | Hecho |
| 69 | Región privada vacía | ✅ | eliminada de `HttpApiInvoker.cs` | Hecho |
| 70 | `ILogger` sin uso en clientes REST | ✅ | `RestClientFp.cs:12` sin logger | Hecho (MIG-78) |
| 71 | `TryMapIAsyncf` | ✅ | hoy `TryMapIfAsync` (`MlResultMapExtensions.cs:852`) | Hecho |
| 72 | `ChangeReturnResultAlwais*` | 🟨 | sigue como puente `[Obsolete]` (`…ChangeReturnResult.Obsolete.cs:22`) | G5-US-02 |
| 73 | Erratas `Lenght` | 🟨 | parámetros y avisos corregidos; métodos erróneos viven como puentes | G5-US-02 |
| 74 | `Id.Bydouble` / `Fromdouble` | ✅ | `Id.cs` estaba excluido de la compilación y se eliminó | Hecho |
| 75 | Registro `Scopedt` | ✅ | `Application/RegisterServices.cs:25` | Hecho |
| 76 | `TEnumrable` / `BaseAdress` | ✅ | `IHttpApiInvoker.cs:39` usa `TItem` | Hecho |
| 77 | Mensajes de validación | 🟨 | `FluentValidation/Helpers/Extensions.cs:38` ya corregido; falta infraestructura común | MSG US-01 |
| 78 | Mensajes de controladores | 🟨 | `Controllers/Helpers/Extensions.cs:129`; queda el alias antiguo | MSG US-03 |
| 79 | Castellano en la capa web | 🟨 | `HttpApiInvoker.cs:30`, `CompositeKeyParameterAttribute.cs:25` | MSG US-04, US-05, US-06 |
| 80 | Logs genéricos | 🟨 | `CrudServiceFp.cs:32` interpola; sin plantilla ni `EventId` | MSG US-07 |
| 81 | Rangos imposibles | ✅ | `PageRequest.cs:18`; test `PaginationValidationTests.cs:9` | Hecho (MIG-80) |
| 82 | Records y contratos HTTP | 🟨 | `ApiCallOptions.cs:13` con `null!`; el problema de `params` está en `ICrudServiceFp.cs:45` | COH · SRV |
| 83 | Genéricos de log sin cerrar | ✅ | `HttpApiInvoker.cs:256` | Hecho (MIG-81) |
| 84 | Nulabilidad de `IHttpApiInvoker` | 🟨 | `IHttpApiInvoker.cs:50` mezcla `null!`, `null` y `""` | COH |
| 85 | Comentario `PostGetAsync` | ✅ | `IHttpApiInvoker.cs:26` | Hecho |
| 86 | Nombre `SearchPattern` | 🟨 | `FilePath.cs:15`, `DirectoryPath.cs:15`: es una expresión regular de validación | COH |
| 87 | XML y estado HTTP incoherentes | ⬜ | `MlCompositeKeyCrudControllerBase.cs:119` promete 200 donde se devuelve 204 | COH |
| 88 | `global using` duplicados | ✅ | sin duplicados literales | Hecho · guarda automática en G1 |
| 89 | `README.md` de proyectos | 🟨 | 13 proyectos actuales sin `README.md` | COH |

---

## 3. Correcciones al inventario original

Errores del propio inventario que conviene no arrastrar:

| INV | Error o desactualización | Corrección |
|---:|---|---|
| 6 | Dice que se ignora `length`; el código lo usa como mínimo y **no** define máximo | La decisión pendiente es *si* debe haber máximo |
| 8, 67 | Cita `RangeEnumValueObject` | Hoy es `EnumRangeValueObject` |
| 10 | Está marcado `[x]` pero la vía directa sigue lanzando | Reabierto como 🟨 |
| 12 | Atribuye a `Core` una extensión `ValidateObject` | No existe en el `Core` actual |
| 59 | `errors!` no prueba que la colección pueda ser nula | Exigir evidencia concreta antes de añadir guardas |
| 64 | Cita `IEfWriteRepository.cs` con 22 firmas | Era `IEfRepository.cs` con 26 |
| 71 | Propone `TryMapAsync` | El nombre vigente es `TryMapIfAsync` |
| 73 | Recomienda `FromStringLength` | Debe recomendarse `CreateFromLength` |
| 78 | Propone `;` como separador | El parser vigente usa `,` |
| 82 | Atribuye `params object[] pk` a los *records* HTTP | Pertenece a `ICrudServiceFp.cs:45` |
| 86 | Habla de `EndpointPattern` | Ya se llama `SearchPattern` en FileSystem; en `Mail` es `ValidationPattern` |
| 89 | «Faltan README en dos proyectos» | Esos proyectos ya no existen; faltan en 13 actuales |
| 17, 18 | Siguen sin marcar | Parcialmente mitigados (truncado, flujo asíncrono) |
| 35-37 | Siguen sin marcar | Resueltos y con prueba |
| — | Las casillas de los tres documentos siguen sin reflejar nada | Se sustituyen por la columna «Estado» de esta auditoría |

**Documentación contradictoria dentro de las épicas archivadas:** `Epica-Simplificacion-Repositorios-EFCore.md`
(L684) registra la retirada de `BuildServiceProvider`, mientras `Epica-Unidad-de-Trabajo-y-Transacciones.md`
(L59) afirma que se conserva. El código confirma la retirada (INV-20 ✅); conviene corregir la segunda (G5-US-03).

**Épicas pendientes desactualizadas frente al código:**

- *Contrato REST* figura «⬜ No iniciada», pero las cuatro bases ya son `abstract` (INV-51) y ya se valida el número de componentes de la clave (INV-60).
- *Serialización y errores HTTP* afirma que falta `GetClientName` en la clase; ya existe en `RestClientFp.cs:26` y solo falta en la interfaz.
- *Servicio y paginación*: varias líneas citadas para INV-53 y INV-54 se han desplazado (hoy `CrudServiceFp.cs:275,577` y `:444,529`).
- *Erratas* describe puentes `Scopedt` en `Application/RegisterServices.Obsolete.cs`, fichero que ya no existe.
- *Coherencia* describe rangos imposibles como defecto vivo; el código, sus tests y su propio *checklist* los dan por corregidos.
- *Mensajes* presenta el inglés de validación como defecto vivo; ya está corregido.

---

## 4. Equivalencias de nombres

### Proyectos

| En el inventario | Hoy |
|---|---|
| `OOFP.EFCore` | `OOFP.EntityFrameworkCore` |
| `OOFP.WebServices` | `OOFP.Application` |
| `OOFP.HttpClients` | `OOFP.Http` |
| `OOFP.WebControllers` / `.Cache` | `OOFP.AspNetCore.Controllers` / `.Controllers.OutputCaching` |
| `OOFP.WebApi` | `OOFP.AspNetCore.Results` |
| `OOFP.Validation.Dataannotations` / `.FluentValidations` | `OOFP.Validation.DataAnnotations` / `.FluentValidation` |
| `OOFP.ValueObjects.IO` · `OOFP.IO` | `OOFP.ValueObjects.FileSystem` · `OOFP.FileSystem` |
| `OOFP.Internals` | `OOFP.Shared` |
| `OOFP.Utilities` · `OOFP.Extensions.Loggers` | `OOFP.Configuration` · `OOFP.Logging` |
| paquete raíz `OOFP` | `OOFP.Core` |

### Tipos y miembros citados con frecuencia

| Antiguo | Hoy |
|---|---|
| `MlErrorsDetails` | `MlErrorDetails` |
| `GenServiceFp` · `AllAsync` | `CrudServiceFp` · `GetPagedAsync` (⚠️ pese a no paginar, INV-41) |
| `GenClientFp` · `GenComplexClientFp` | `RestClientFp` · `CompositeKeyRestClientFp` |
| `MlValidableFp<T>` | `IMlSelfValidatable<T>` |
| `PkParameterAttribute` | `CompositeKeyParameterAttribute` |
| `GetPkValues<TEntity>` | `ParseKeyValues<TEntity>` |
| `TryLast` / `TryLastAsync` | `TryGetLast` / `TryGetLastAsync` |
| `RangeEnumValueObject` | `EnumRangeValueObject` |
| `Empty` (value object) | `EmptyValue` |

Inventario completo de renombrados: [`oofp-upgrade-2.renames.md`](../../../../Copilot/.github/prompts/oofp-upgrade-2.renames.md).
