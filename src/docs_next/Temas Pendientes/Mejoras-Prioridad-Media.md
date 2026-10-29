# Mejoras pendientes · 🟡 Prioridad media

> 📌 **Qué es este documento**
> Continuación de [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) (que contiene las prioridades 🔴 **crítica** y 🟠 **alta**).
> Aquí están los puntos de **prioridad media**: no producen datos incorrectos ni riesgos de seguridad,
> pero **se pagan cuando el volumen crece** o hacen que la API sea difícil de usar correctamente.
> La numeración continúa la del documento principal, que termina en el punto **37**.

> ℹ️ Los renombrados están en [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md). Las mejoras de ingeniería, empaquetado y
> diseño global están en [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md)
> y [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md).

> ⚠️ **Nomenclatura obsoleta.** Este documento se redactó **antes del renombrado 2.0.0**: los nombres de
> proyecto, clase y miembro que cita **ya no existen**. La tabla de traducción y la verificación
> punto por punto contra el código real están en
> [`Plan-Ejecucion-Prioridad-Media.md`](Plan-Ejecucion-Prioridad-Media.md).

> ✅ **Estado tras la auditoría (commit `9ffd376`):** 2 resueltos (55, 62) · 5 parciales (41, 47, 51, 60, 63) · 19 pendientes.

---

## Índice

- [Rendimiento y acceso a datos](#rendimiento-y-acceso-a-datos) — puntos 38-44
- [Diseño de API y contratos](#diseño-de-api-y-contratos) — puntos 45-52
- [Comportamiento y coherencia funcional](#comportamiento-y-coherencia-funcional) — puntos 53-63

---

## Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo / clase:** `Repos/EFRepoReaderFp`
    - **Miembro:** `TryLast`, `TryLastAsync` y variantes
    - ⬜ **PENDIENTE** · hoy `EntityFrameworkCore/Repositories.Imperative/ThrowingEfReadRepository.cs` → `TryGetLast`/`TryGetLastAsync`: L44 `.AsEnumerable().LastOrDefault()`, L47 `(await …ToListAsync()).LastOrDefault()`.
    - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al cliente y se selecciona el último en memoria.
    - **Impacto:** con una tabla grande son millones de filas por la red y un pico de memoria que puede tumbar el proceso, cuando la base de datos lo resolvía con un `TOP 1`.
    - **Propuesta:** ordenar de forma descendente en la consulta y usar `FirstOrDefaultAsync` sobre `IQueryable`. Exigir orden explícito: «el último» no significa nada sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
    - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
    - ⬜ **PENDIENTE** · hoy `EntityFrameworkCore/Repositories/EfUpdateRepository.cs`: L63 `TryFind(pk)` (rastrea) + L65 `Update(item)` (otra instancia).
    - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update` con la instancia recibida por parámetro, que es otro objeto con la misma clave.
    - **Impacto:** `InvalidOperationException` («another instance with the same key value is already being tracked») en el escenario más habitual: actualizar a partir de un DTO.
    - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: un `SaveChanges` por operación**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo / clase:** todos los repositorios (`Repos/*Fp`, `OopRepos/*`)
    - ⬜ **PENDIENTE** · **12 llamadas** a `SaveChanges`/`SaveChangesAsync` en `ThrowingEf{Create,Delete,Update}Repository.cs`; no existe `IMlUnitOfWork`. ⚠️ **El punto más caro del documento** (~1 semana): candidato a épico propio en 2.1.0.
    - **Problema:** cada método de escritura llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
    - **Impacto:** imposible componer varias escrituras en una transacción; si la segunda falla, la primera ya está confirmada. Rompe la consistencia en cualquier caso de uso con más de una entidad.
    - **Propuesta:** separar `Add`/`Update`/`Remove` (que solo marcan) de un `IMlUnitOfWork.CommitAsync()` explícito, o exponer un `TryExecuteInTransactionAsync(...)` que agrupe operaciones y confirme una sola vez.

- [ ] **41. `AllAsync` sin paginación ni límite**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp`
    - **Miembro:** `AllAsync`
    - 🟨 **PARCIAL** · hoy `MoralesLarios.OOFP.Application/Services/CrudServiceFp.cs` → `GetPagedAsync`. Renombrado pero **sigue sin paginar** (L33 y L328 llaman a `TryAllAsync`). `PageRequest` (`MaxPageSize = 1000`) y `EfPagedReadRepository.TryAllPagination` ya existen: solo falta conectarlos.
    - **Problema:** devuelve la tabla completa sin tope de filas, y está expuesto a través de los controladores base.
    - **Impacto:** un `GET` sobre una tabla grande consume memoria del servidor y ancho de banda; es además un vector de denegación de servicio trivial.
    - **Propuesta:** ofrecer solo la variante paginada en la API pública, con `MaxPageSize` configurable, o exigir un filtro obligatorio. Si `AllAsync` debe permanecer, marcarlo como avanzado y documentar el riesgo.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp`, `GenComplexClientFp` y los helpers internos de llamada
    - ⬜ **PENDIENTE** · hoy `Http/HttpApiInvoker.cs`: **9 peticiones** sin `using` (L31, 89, 139, 166, 193, 221, 247, 486, 527) y **7 respuestas** sin liberar; el `response.Dispose()` de L310 está **comentado**.
    - **Problema:** se crean mensajes de petición y se reciben respuestas sin `using`, dejando el contenido sin liberar.
    - **Impacto:** retención de *streams* y *buffers* hasta que actúe el recolector; bajo carga se traduce en consumo de memoria creciente y sockets ocupados más tiempo del necesario.
    - **Propuesta:** `using var request = …;` y `using var response = await client.SendAsync(...)` en todas las rutas, incluidas las de error.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Helpers/Extensions`
    - **Miembro:** `GetPkValues<TEntity>(string[], Func<TEntity, object[]>)`
    - ⬜ **PENDIENTE** · hoy `AspNetCore.Controllers/Helpers/Extensions.cs` L143: `var sample = Activator.CreateInstance<TEntity>();` (miembro renombrado a `ParseKeyValues<TEntity>`).
    - **Problema:** para averiguar los tipos de la clave primaria se **instancia una entidad vacía** por reflexión en cada petición, lo que además obliga a que `TEntity` tenga constructor público sin parámetros y asume `string` cuando el valor resulta `null`.
    - **Impacto:** coste de reflexión en la ruta caliente, restricción artificial en las entidades y conversiones silenciosamente incorrectas.
    - **Propuesta:** obtener los tipos del **modelo de EF Core** (`IEntityType.FindPrimaryKey()`) o de metadatos declarados una sola vez y cacheados; nunca instanciar la entidad para inspeccionarla.

- [ ] **44. Resultado de `JsonSerializer.Serialize` descartado**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** helper interno de peticiones con cuerpo
    - **Miembro:** `InternalPostGetAsync`
    - ⬜ **PENDIENTE** · hoy `Http/HttpApiInvoker.cs` → `SendAndReadAsync`: L329 serializa y **descarta** el resultado; L333 vuelve a serializar con `PostAsJsonAsync`.
    - **Problema:** se llama a `JsonSerializer.Serialize(...)` y **no se usa el valor devuelto**; el cuerpo se construye después por otra vía.
    - **Impacto:** trabajo de serialización duplicado en cada llamada y una línea que hace pensar que configura algo cuando no hace nada.
    - **Propuesta:** eliminar la llamada muerta o usar su resultado para construir el `StringContent`, con las `JsonSerializerOptions` inyectadas.

---

## Diseño de API y contratos

- [ ] **45. No se pueden configurar las `JsonSerializerOptions`**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp`, `GenComplexClientFp` y helpers de serialización
    - ⬜ **PENDIENTE** · hoy `Http/Requests/ApiCallOptions.cs` (L11) y `PagedApiCallOptions.cs` (L16): ninguno expone `JsonSerializerOptions`; `HttpApiInvoker` L138 y L311 usan los valores por defecto.
    - **Problema:** se usan las opciones por defecto de `System.Text.Json`, sin posibilidad de indicar política de nombres, convertidores, `enum` como texto o `DateTime` con formato.
    - **Impacto:** si el servidor usa `camelCase` o convertidores propios, el cliente falla y no hay forma de arreglarlo sin modificar la biblioteca.
    - **Propuesta:** `JsonSerializerOptions` en las opciones del cliente (`MlHttpClientOptions`), con valores por defecto sensatos y una única instancia reutilizada.

- [ ] **46. El código de estado HTTP se pierde como texto**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `Helpers/MlResponseWebExtensions`
    - ⬜ **PENDIENTE** · hoy `Http/Helpers/MlResponseWebExtensions.cs` L70: `Código de error: {(int)response.StatusCode}` embebido en la cadena, no en `Details`.
    - **Problema:** cuando la respuesta no es correcta se construye un mensaje de error con el estado **embebido en la cadena**, sin conservarlo como dato.
    - **Impacto:** quien consume el cliente no puede distinguir un 404 de un 409 o de un 503 sin analizar texto, y por tanto no puede reintentar ni tratar los casos de forma diferenciada.
    - **Propuesta:** añadir el `HttpStatusCode` y la URL a los `Details` con claves constantes, o un código de error tipado. El texto queda solo para el humano.

- [ ] **47. Métodos públicos que no están en las interfaces**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp<…>` frente a `IGenClientFp<…>`
    - **Miembro:** `GetHttpClientFactoryKey()`, `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(...)`
    - 🟨 **PARCIAL** · `GetApiInvoker()` **ya está** en `IRestClientFp` (L44, L116). Faltan `GetClientName()` y los 2 `[Obsolete]`; `GetAsync<T>`/`GetPagedAsync<T>` no están en `IHttpApiInvoker`. **No hay `PublicApiAnalyzers`.**
    - **Problema:** existen en la clase pero no en la interfaz.
    - **Impacto:** quien programa contra la interfaz (lo correcto, y lo que exige la inyección de dependencias) no puede usarlos ni sustituirlos en pruebas.
    - **Propuesta:** decidir para cada uno: subirlo a la interfaz o hacerlo `internal`/`protected`. Congelar el contrato con `PublicApiAnalyzers` para que no vuelva a divergir.

- [ ] **48. Asimetrías de nombres y de parámetros entre familias equivalentes**
    - **Proyectos:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebControllers`
    - ⬜ **PENDIENTE** · hoy `Http/RestClientFp.cs` L51 `$"id-str/{idStr}"` (GET) vs L89 `idStr` crudo (PUT); `MlCrudControllerBase.cs` L81 `dto:` vs L224 `dtoRequest:`.
    - **Impacto:** el usuario aprende una familia y la otra se comporta distinto; cada diferencia es un bug latente que solo aparece en tiempo de ejecución.
    - **Propuesta:** tabla de equivalencias simple ↔ dúplex, unificar nombres y orden de parámetros, y una prueba paramétrica que recorra ambas familias verificando la URL generada.

- [ ] **49. Rutas incoherentes entre verbos del mismo recurso**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** todas las bases de controlador
    - ⬜ **PENDIENTE** · hoy `AspNetCore.Controllers`: `[HttpGet("id-str/{id}")]` (L39, L182) frente a `[HttpPut("{id}")]` (L76, L219) y `[HttpDelete("{id}")]` (L110, L253); igual en la base de clave compuesta.
    - **Impacto:** el recurso no tiene una identidad única; complica el cliente, las cachés y la documentación OpenAPI.
    - **Propuesta:** un solo patrón para todos los verbos del recurso. Si se necesita distinguir clave simple de compuesta, hacerlo con restricciones de ruta, no con segmentos distintos por verbo.

- [ ] **50. `PUT` y `DELETE` con la clave en `[FromBody]`**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** sobrecargas de `PutAsync` y `DeleteAsync` de las bases
    - ⬜ **PENDIENTE (con matiz)** · las sobrecargas de L96, 129, 239 y 272 reciben el **DTO completo** en `[FromBody]`, no un parámetro de clave suelto; el riesgo del `DELETE` con cuerpo se mantiene igual.
    - **Problema:** existen sobrecargas que reciben los valores de clave en el cuerpo de la petición.
    - **Impacto:** muchos *proxies*, CDN y clientes **descartan el cuerpo de un `DELETE`**; la llamada falla de forma intermitente y difícil de diagnosticar.
    - **Propuesta:** clave siempre en la ruta (compuesta con separador documentado y codificado); si el cuerpo es imprescindible, usar `POST` sobre un subrecurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** las 4 bases (`SimpleMl…ControllerBase`, variantes dúplex y de clave compuesta)
    - 🟨 **PARCIAL** · las 4 bases **ya son `abstract`** (resuelto de rebote por el renombrado). Siguen con `[ApiController]` en la base y ninguna declara `[Route]` (correcto: lo pone la derivada). Falta el test de que no aparecen en el catálogo de endpoints.
    - **Problema:** son clases concretas con `[ApiController]` pero sin `[Route]`.
    - **Impacto:** ASP.NET Core puede descubrirlas como controladores reales sin ruta, con errores de arranque o rutas duplicadas; y nada impide instanciarlas directamente.
    - **Propuesta:** declararlas `abstract`, sin `[ApiController]` en la base (que lo ponga el derivado) y con `[Route("[controller]")]` en el derivado. Añadir un test que verifique que las bases no aparecen en el catálogo de endpoints.

- [ ] **52. Sin `[ProducesResponseType]` en ningún método base**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** las 4 bases de controlador
    - ⬜ **PENDIENTE** · **0 `[ProducesResponseType]` y 0 `[Produces]`** en todo `AspNetCore.Controllers`; las acciones devuelven `Task<IActionResult>`.
    - **Problema:** los métodos devuelven `IActionResult` sin declarar los códigos ni los tipos posibles.
    - **Impacto:** OpenAPI genera un contrato vacío; los clientes generados automáticamente no compilan o devuelven `object`, y la documentación no refleja los 400/404/409 que sí se producen.
    - **Propuesta:** `[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TDto))]` y equivalentes para 201, 204, 400, 404 y 500, más `[Produces("application/json")]`.

---

## Comportamiento y coherencia funcional

- [ ] **53. `DeleteAsync(TDto, …)` usa `MapAsync` en lugar de `TryMapAsync`**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp`
    - ⬜ **PENDIENTE** · hoy `Application/Services/CrudServiceFp.cs` L271 y L568: `.MapAsync(_ => dto.Adapt<TEntity>())`. Las firmas (L264, L561) admiten `initialMessage`/`failMessageBuilder` pero **no** factoría de mensaje de éxito. Recuento: **16 `MapAsync` vs 6 `TryMapAsync`**.
    - **Problema:** la conversión de DTO a entidad se hace con la variante que **no captura excepciones**, y además no admite `validMessageBuilder` como sus hermanas.
    - **Impacto:** un DTO con datos no convertibles lanza excepción en lugar de devolver un fallo controlado; se rompe el modelo *railway* precisamente en la operación destructiva.
    - **Propuesta:** usar `TryMapAsync` con constructor de mensaje, igual que el resto de operaciones del servicio.

- [ ] **54. Mensajes del servicio dúplex con el tipo equivocado**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp` dúplex
    - ⬜ **PENDIENTE** · hoy `CrudServiceFp.cs`: L438 y L521 usan `typeof(TEntity).Name`, mientras L539, L547, L548 y L584 describen lo mismo con `typeof(TResponse).Name`.
    - **Impacto:** el mensaje de error nombra un tipo que el consumidor no conoce (la entidad interna) o el equivocado, dificultando el diagnóstico.
    - **Propuesta:** criterio único: **hacia el cliente, el nombre del DTO/`TResponse`; en los logs internos, el de la entidad**.

- [x] **55. Nombres de cabecera de paginación como literales duplicados**
    - **Proyectos:** `MoralesLarios.OOFP.HttpClients` y `MoralesLarios.OOFP.WebApi`
    - **Archivo / clase:** `MlHttpRequestExtensions` y `MlRequestWebExtensions`
    - ✅ **RESUELTO** · constantes en `MoralesLarios.OOFP.Shared/Web/MlPaginationHeaders.cs` (L11 `PageNumber`, L15 `PageSize`), consumidas por `Http` (L72) y `AspNetCore.Results` (L68). Los literales restantes están solo en tests.
    - **Problema:** `"X-Page-Number"` y `"X-Page-Size"` están escritos como literales en ambos lados del protocolo.
    - **Impacto:** si se corrige un nombre en un proyecto y no en el otro, cliente y servidor dejan de entenderse **sin ningún error de compilación**.
    - **Propuesta:** constantes en un proyecto de abstracciones compartido del que dependan ambos, y una prueba de contrato que recorra el ciclo completo.

- [ ] **56. `MlValidableFp<T>` no garantiza que `T` sea el tipo que deriva**
    - **Proyecto:** `MoralesLarios.OOFP.Validation`
    - **Archivo / clase:** `MlValidableFp<T>`
    - ⬜ **PENDIENTE** · hoy `IMlSelfValidatable<T>` (L14) y `MlSelfValidatable<T>` (`MlSelfValidatable.Obsolete.cs` L11): ambos `where T : class`, **sin restricción de tipo propio**.
    - **Problema:** es una clase abstracta genérica sin restricción `where T : MlValidableFp<T>`, de modo que se puede derivar indicando otro tipo.
    - **Impacto:** las factorías y comparaciones devuelven el tipo equivocado y el error aparece lejos de su causa.
    - **Propuesta:** restricción de tipo propio (`where TSelf : IMlValidable<TSelf>`) y, en .NET 8, miembros `static abstract` para las factorías.

- [ ] **57. `ValidationContext` construido sin `IServiceProvider`**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** `DataannotationsValidator`
    - ⬜ **PENDIENTE** · hoy `Validation.DataAnnotations/Helpers/Extensions.cs` L22: `new ValidationContext(source, null, null)` (la clase es `DataAnnotationsValidator`).
    - **Problema:** `new ValidationContext(source, null, null)`.
    - **Impacto:** ningún `ValidationAttribute` puede resolver servicios (`GetService`), lo que excluye validaciones que necesiten consultar la base de datos o configuración.
    - **Propuesta:** aceptar un `IServiceProvider` opcional y pasarlo al contexto, junto con el diccionario de elementos.

- [ ] **58. Asincronía simulada en la validación**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** `DataannotationsValidator`
    - **Miembro:** `ValidateAsync<T>` y derivados
    - ⬜ **PENDIENTE** · `DataAnnotationsValidator.cs` L28 y L57: `ValidateSource(source).ToAsync()`. *Matiz:* las sobrecargas con entrada `Task` **sí** hacen `await` (L38, L67).
    - **Problema:** son la versión sincrónica envuelta en `Task.FromResult` mediante `ToAsync()`.
    - **Impacto:** el sufijo `Async` promete algo que no ocurre; quien lo usa asume que no bloquea el hilo y no es cierto en las rutas que sí hagan E/S en el futuro.
    - **Propuesta:** o hacerlo realmente asíncrono, o documentar de forma explícita que existe solo por simetría de composición. Analizador que impida crear nuevos `*Async` sin `await`.

- [ ] **59. Se pierden los `MemberNames` de la validación**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** conversión de `ValidationResult` a `MlError`
    - ⬜ **PENDIENTE** · hoy `Validation.DataAnnotations/Helpers/Extensions.cs` L39 (`Select(x => x.ErrorMessage)` descarta `MemberNames`) y L40 (`errors!`). `Core/Types/Errors/MlError.cs` L17 solo declara `Message`.
    - **Problema:** solo se conserva el mensaje; además se usa `errors!` para forzar el desempaquetado de una colección que puede ser nula.
    - **Impacto:** el cliente no sabe **qué campo** ha fallado, que es justo lo que necesita un formulario; y el `!` puede acabar en `NullReferenceException`.
    - **Propuesta:** añadir `MemberName`/`Target` a `MlError` y comprobar la colección antes de recorrerla.

- [ ] **60. `GetPkValues(this string ids, …)` parte la cadena de forma ingenua**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Helpers/Extensions`
    - 🟨 **PARCIAL** · hoy `ParseKeyValues`. L119 sigue con `ids.Split(',')` sin decodificar, **pero L145 ya valida el número de componentes** (`if (values.Length != sampleP.Length) throw`).
    - **Problema:** `ids.Split(',')` sin decodificar, sin admitir escapes y sin validar el número de valores frente a la clave real.
    - **Impacto:** cualquier valor de clave que contenga una coma (o esté codificado en la URL) se parte mal y produce un error confuso.
    - **Propuesta:** separador documentado y codificado (`Uri.EscapeDataString`), validación del número de componentes y mensaje de error que indique el formato esperado.

- [ ] **61. `PkParameterAttribute` es inerte**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Attributes/PkParameterAttribute`
    - ⬜ **PENDIENTE** · hoy `CompositeKeyParameterAttribute` (L12). Se aplica en **12 sitios** y nada lo lee: sin `IOperationFilter`/`ISchemaFilter`/`IDocumentFilter`.
    - **Problema:** el atributo existe y se aplica, pero **nada lo lee**: no hay `IOperationFilter` ni `ISchemaFilter` que lo procese.
    - **Impacto:** documentación OpenAPI que el autor cree publicada y que no se genera; falsa sensación de contrato.
    - **Propuesta:** implementar el filtro de Swashbuckle/OpenAPI que lo traduzca a parámetros documentados, o eliminar el atributo.

- [x] **62. Falta un helper de registro para `IGenComplexClientFp<>`**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `RegisterServices`
    - ✅ **RESUELTO** · `Http/RegisterServices.cs` L67 expone `AddCompositeKeyRestClient<TService,TImplementation,TDto>` y L83 registra también `IRestClientFp<TDto>`. Los 4 helpers cubren simple y dúplex.
    - **Problema:** hay helpers para las variantes simple y dúplex compleja, pero no para el cliente complejo simple.
    - **Propuesta:** completar la simetría de los métodos de registro y cubrirlos con un test que construya el `ServiceProvider` con validación de ámbitos.

- [ ] **63. `MlHttpRequestExtensions` con defectos acumulados**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `MlHttpRequestExtensions`
    - 🟨 **PARCIAL** · `Http/Helpers/MlHttpRequestExtensions.cs`: `nameof` equivocado en L134 y L160 ⬜; `TrySetHeaders` no valida la colección antes del `foreach` (L163) ⬜; `Async` simulados vía `ToAsync()` (L180) ⬜; **mensajes en inglés ya correctos** ✅.
    - **Problema:** un `nameof` apunta al parámetro equivocado en el mensaje de error; `SetHeaders` no comprueba nulos antes de recorrer la colección; todos los `*Async` son asincronía simulada; los mensajes están en inglés incorrecto.
    - **Propuesta:** guardas explícitas, `nameof` correcto, mensajes revisados y eliminación del sufijo `Async` donde no aplique.

---

## Ver también

- [`Plan-Ejecucion-Prioridad-Media.md`](Plan-Ejecucion-Prioridad-Media.md) — **auditoría punto por punto contra el código real y plan de ejecución**.
- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — prioridades 🔴 crítica y 🟠 alta.
- [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — prioridad 🟢 baja.
- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) ·
  [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md)
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — propuesta de renombrado.
- [`README.md`](README.md) — índice de la carpeta.
