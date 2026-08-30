# Mejoras pendientes · 🟡 Prioridad media

> 📌 **Qué es este documento**
> Continuación de [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) (que contiene las prioridades 🔴 **crítica** y 🟠 **alta**).
> Aquí están los puntos de **prioridad media**: no producen datos incorrectos ni riesgos de seguridad,
> pero **se pagan cuando el volumen crece** o hacen que la API sea difícil de usar correctamente.
> La numeración continúa la del documento principal, que termina en el punto **37**.

> ℹ️ Los renombrados están en [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md). Las mejoras de ingeniería, empaquetado y
> diseño global están en [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md)
> y [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md).

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
    - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al cliente y se selecciona el último en memoria.
    - **Impacto:** con una tabla grande son millones de filas por la red y un pico de memoria que puede tumbar el proceso, cuando la base de datos lo resolvía con un `TOP 1`.
    - **Propuesta:** ordenar de forma descendente en la consulta y usar `FirstOrDefaultAsync` sobre `IQueryable`. Exigir orden explícito: «el último» no significa nada sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
    - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
    - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update` con la instancia recibida por parámetro, que es otro objeto con la misma clave.
    - **Impacto:** `InvalidOperationException` («another instance with the same key value is already being tracked») en el escenario más habitual: actualizar a partir de un DTO.
    - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: un `SaveChanges` por operación**
    - **Proyecto:** `MoralesLarios.OOFP.EFCore`
    - **Archivo / clase:** todos los repositorios (`Repos/*Fp`, `OopRepos/*`)
    - **Problema:** cada método de escritura llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
    - **Impacto:** imposible componer varias escrituras en una transacción; si la segunda falla, la primera ya está confirmada. Rompe la consistencia en cualquier caso de uso con más de una entidad.
    - **Propuesta:** separar `Add`/`Update`/`Remove` (que solo marcan) de un `IMlUnitOfWork.CommitAsync()` explícito, o exponer un `TryExecuteInTransactionAsync(...)` que agrupe operaciones y confirme una sola vez.

- [ ] **41. `AllAsync` sin paginación ni límite**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp`
    - **Miembro:** `AllAsync`
    - **Problema:** devuelve la tabla completa sin tope de filas, y está expuesto a través de los controladores base.
    - **Impacto:** un `GET` sobre una tabla grande consume memoria del servidor y ancho de banda; es además un vector de denegación de servicio trivial.
    - **Propuesta:** ofrecer solo la variante paginada en la API pública, con `MaxPageSize` configurable, o exigir un filtro obligatorio. Si `AllAsync` debe permanecer, marcarlo como avanzado y documentar el riesgo.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp`, `GenComplexClientFp` y los helpers internos de llamada
    - **Problema:** se crean mensajes de petición y se reciben respuestas sin `using`, dejando el contenido sin liberar.
    - **Impacto:** retención de *streams* y *buffers* hasta que actúe el recolector; bajo carga se traduce en consumo de memoria creciente y sockets ocupados más tiempo del necesario.
    - **Propuesta:** `using var request = …;` y `using var response = await client.SendAsync(...)` en todas las rutas, incluidas las de error.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Helpers/Extensions`
    - **Miembro:** `GetPkValues<TEntity>(string[], Func<TEntity, object[]>)`
    - **Problema:** para averiguar los tipos de la clave primaria se **instancia una entidad vacía** por reflexión en cada petición, lo que además obliga a que `TEntity` tenga constructor público sin parámetros y asume `string` cuando el valor resulta `null`.
    - **Impacto:** coste de reflexión en la ruta caliente, restricción artificial en las entidades y conversiones silenciosamente incorrectas.
    - **Propuesta:** obtener los tipos del **modelo de EF Core** (`IEntityType.FindPrimaryKey()`) o de metadatos declarados una sola vez y cacheados; nunca instanciar la entidad para inspeccionarla.

- [ ] **44. Resultado de `JsonSerializer.Serialize` descartado**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** helper interno de peticiones con cuerpo
    - **Miembro:** `InternalPostGetAsync`
    - **Problema:** se llama a `JsonSerializer.Serialize(...)` y **no se usa el valor devuelto**; el cuerpo se construye después por otra vía.
    - **Impacto:** trabajo de serialización duplicado en cada llamada y una línea que hace pensar que configura algo cuando no hace nada.
    - **Propuesta:** eliminar la llamada muerta o usar su resultado para construir el `StringContent`, con las `JsonSerializerOptions` inyectadas.

---

## Diseño de API y contratos

- [ ] **45. No se pueden configurar las `JsonSerializerOptions`**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp`, `GenComplexClientFp` y helpers de serialización
    - **Problema:** se usan las opciones por defecto de `System.Text.Json`, sin posibilidad de indicar política de nombres, convertidores, `enum` como texto o `DateTime` con formato.
    - **Impacto:** si el servidor usa `camelCase` o convertidores propios, el cliente falla y no hay forma de arreglarlo sin modificar la biblioteca.
    - **Propuesta:** `JsonSerializerOptions` en las opciones del cliente (`MlHttpClientOptions`), con valores por defecto sensatos y una única instancia reutilizada.

- [ ] **46. El código de estado HTTP se pierde como texto**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `Helpers/MlResponseWebExtensions`
    - **Problema:** cuando la respuesta no es correcta se construye un mensaje de error con el estado **embebido en la cadena**, sin conservarlo como dato.
    - **Impacto:** quien consume el cliente no puede distinguir un 404 de un 409 o de un 503 sin analizar texto, y por tanto no puede reintentar ni tratar los casos de forma diferenciada.
    - **Propuesta:** añadir el `HttpStatusCode` y la URL a los `Details` con claves constantes, o un código de error tipado. El texto queda solo para el humano.

- [ ] **47. Métodos públicos que no están en las interfaces**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `GenClientFp<…>` frente a `IGenClientFp<…>`
    - **Miembro:** `GetHttpClientFactoryKey()`, `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(...)`
    - **Problema:** existen en la clase pero no en la interfaz.
    - **Impacto:** quien programa contra la interfaz (lo correcto, y lo que exige la inyección de dependencias) no puede usarlos ni sustituirlos en pruebas.
    - **Propuesta:** decidir para cada uno: subirlo a la interfaz o hacerlo `internal`/`protected`. Congelar el contrato con `PublicApiAnalyzers` para que no vuelva a divergir.

- [ ] **48. Asimetrías de nombres y de parámetros entre familias equivalentes**
    - **Proyectos:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebControllers`
    - **Ejemplos:** `GetByIdAsync` envía `id-str/{idStr}` mientras `PutByIdAsync` envía el `idStr` en crudo; el `DeleteByIdAsync` dúplex usa `TResponse` donde el simple usa `TDto`; el `PutAsync` dúplex usa el argumento nombrado `dtoRequest:` y el simple `dto:`.
    - **Impacto:** el usuario aprende una familia y la otra se comporta distinto; cada diferencia es un bug latente que solo aparece en tiempo de ejecución.
    - **Propuesta:** tabla de equivalencias simple ↔ dúplex, unificar nombres y orden de parámetros, y una prueba paramétrica que recorra ambas familias verificando la URL generada.

- [ ] **49. Rutas incoherentes entre verbos del mismo recurso**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** todas las bases de controlador
    - **Problema:** `GET` publica `id-str/{id}` mientras `PUT` y `DELETE` usan `{id}`.
    - **Impacto:** el recurso no tiene una identidad única; complica el cliente, las cachés y la documentación OpenAPI.
    - **Propuesta:** un solo patrón para todos los verbos del recurso. Si se necesita distinguir clave simple de compuesta, hacerlo con restricciones de ruta, no con segmentos distintos por verbo.

- [ ] **50. `PUT` y `DELETE` con la clave en `[FromBody]`**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** sobrecargas de `PutAsync` y `DeleteAsync` de las bases
    - **Problema:** existen sobrecargas que reciben los valores de clave en el cuerpo de la petición.
    - **Impacto:** muchos *proxies*, CDN y clientes **descartan el cuerpo de un `DELETE`**; la llamada falla de forma intermitente y difícil de diagnosticar.
    - **Propuesta:** clave siempre en la ruta (compuesta con separador documentado y codificado); si el cuerpo es imprescindible, usar `POST` sobre un subrecurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** las 4 bases (`SimpleMl…ControllerBase`, variantes dúplex y de clave compuesta)
    - **Problema:** son clases concretas con `[ApiController]` pero sin `[Route]`.
    - **Impacto:** ASP.NET Core puede descubrirlas como controladores reales sin ruta, con errores de arranque o rutas duplicadas; y nada impide instanciarlas directamente.
    - **Propuesta:** declararlas `abstract`, sin `[ApiController]` en la base (que lo ponga el derivado) y con `[Route("[controller]")]` en el derivado. Añadir un test que verifique que las bases no aparecen en el catálogo de endpoints.

- [ ] **52. Sin `[ProducesResponseType]` en ningún método base**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** las 4 bases de controlador
    - **Problema:** los métodos devuelven `IActionResult` sin declarar los códigos ni los tipos posibles.
    - **Impacto:** OpenAPI genera un contrato vacío; los clientes generados automáticamente no compilan o devuelven `object`, y la documentación no refleja los 400/404/409 que sí se producen.
    - **Propuesta:** `[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TDto))]` y equivalentes para 201, 204, 400, 404 y 500, más `[Produces("application/json")]`.

---

## Comportamiento y coherencia funcional

- [ ] **53. `DeleteAsync(TDto, …)` usa `MapAsync` en lugar de `TryMapAsync`**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp`
    - **Problema:** la conversión de DTO a entidad se hace con la variante que **no captura excepciones**, y además no admite `validMessageBuilder` como sus hermanas.
    - **Impacto:** un DTO con datos no convertibles lanza excepción en lugar de devolver un fallo controlado; se rompe el modelo *railway* precisamente en la operación destructiva.
    - **Propuesta:** usar `TryMapAsync` con constructor de mensaje, igual que el resto de operaciones del servicio.

- [ ] **54. Mensajes del servicio dúplex con el tipo equivocado**
    - **Proyecto:** `MoralesLarios.OOFP.WebServices`
    - **Archivo / clase:** `GenServiceFp` dúplex
    - **Problema:** unos mensajes usan `typeof(TEntity).Name` y otros `typeof(TResponse).Name` para describir la misma operación.
    - **Impacto:** el mensaje de error nombra un tipo que el consumidor no conoce (la entidad interna) o el equivocado, dificultando el diagnóstico.
    - **Propuesta:** criterio único: **hacia el cliente, el nombre del DTO/`TResponse`; en los logs internos, el de la entidad**.

- [ ] **55. Nombres de cabecera de paginación como literales duplicados**
    - **Proyectos:** `MoralesLarios.OOFP.HttpClients` y `MoralesLarios.OOFP.WebApi`
    - **Archivo / clase:** `MlHttpRequestExtensions` y `MlRequestWebExtensions`
    - **Problema:** `"X-Page-Number"` y `"X-Page-Size"` están escritos como literales en ambos lados del protocolo.
    - **Impacto:** si se corrige un nombre en un proyecto y no en el otro, cliente y servidor dejan de entenderse **sin ningún error de compilación**.
    - **Propuesta:** constantes en un proyecto de abstracciones compartido del que dependan ambos, y una prueba de contrato que recorra el ciclo completo.

- [ ] **56. `MlValidableFp<T>` no garantiza que `T` sea el tipo que deriva**
    - **Proyecto:** `MoralesLarios.OOFP.Validation`
    - **Archivo / clase:** `MlValidableFp<T>`
    - **Problema:** es una clase abstracta genérica sin restricción `where T : MlValidableFp<T>`, de modo que se puede derivar indicando otro tipo.
    - **Impacto:** las factorías y comparaciones devuelven el tipo equivocado y el error aparece lejos de su causa.
    - **Propuesta:** restricción de tipo propio (`where TSelf : IMlValidable<TSelf>`) y, en .NET 8, miembros `static abstract` para las factorías.

- [ ] **57. `ValidationContext` construido sin `IServiceProvider`**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** `DataannotationsValidator`
    - **Problema:** `new ValidationContext(source, null, null)`.
    - **Impacto:** ningún `ValidationAttribute` puede resolver servicios (`GetService`), lo que excluye validaciones que necesiten consultar la base de datos o configuración.
    - **Propuesta:** aceptar un `IServiceProvider` opcional y pasarlo al contexto, junto con el diccionario de elementos.

- [ ] **58. Asincronía simulada en la validación**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** `DataannotationsValidator`
    - **Miembro:** `ValidateAsync<T>` y derivados
    - **Problema:** son la versión sincrónica envuelta en `Task.FromResult` mediante `ToAsync()`.
    - **Impacto:** el sufijo `Async` promete algo que no ocurre; quien lo usa asume que no bloquea el hilo y no es cierto en las rutas que sí hagan E/S en el futuro.
    - **Propuesta:** o hacerlo realmente asíncrono, o documentar de forma explícita que existe solo por simetría de composición. Analizador que impida crear nuevos `*Async` sin `await`.

- [ ] **59. Se pierden los `MemberNames` de la validación**
    - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
    - **Archivo / clase:** conversión de `ValidationResult` a `MlError`
    - **Problema:** solo se conserva el mensaje; además se usa `errors!` para forzar el desempaquetado de una colección que puede ser nula.
    - **Impacto:** el cliente no sabe **qué campo** ha fallado, que es justo lo que necesita un formulario; y el `!` puede acabar en `NullReferenceException`.
    - **Propuesta:** añadir `MemberName`/`Target` a `MlError` y comprobar la colección antes de recorrerla.

- [ ] **60. `GetPkValues(this string ids, …)` parte la cadena de forma ingenua**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Helpers/Extensions`
    - **Problema:** `ids.Split(',')` sin decodificar, sin admitir escapes y sin validar el número de valores frente a la clave real.
    - **Impacto:** cualquier valor de clave que contenga una coma (o esté codificado en la URL) se parte mal y produce un error confuso.
    - **Propuesta:** separador documentado y codificado (`Uri.EscapeDataString`), validación del número de componentes y mensaje de error que indique el formato esperado.

- [ ] **61. `PkParameterAttribute` es inerte**
    - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
    - **Archivo / clase:** `Attributes/PkParameterAttribute`
    - **Problema:** el atributo existe y se aplica, pero **nada lo lee**: no hay `IOperationFilter` ni `ISchemaFilter` que lo procese.
    - **Impacto:** documentación OpenAPI que el autor cree publicada y que no se genera; falsa sensación de contrato.
    - **Propuesta:** implementar el filtro de Swashbuckle/OpenAPI que lo traduzca a parámetros documentados, o eliminar el atributo.

- [ ] **62. Falta un helper de registro para `IGenComplexClientFp<>`**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `RegisterServices`
    - **Problema:** hay helpers para las variantes simple y dúplex compleja, pero no para el cliente complejo simple.
    - **Impacto:** el usuario tiene que escribir a mano el registro con `AddHttpClient` y la clave, replicando lógica interna que puede cambiar.
    - **Propuesta:** completar la simetría de los métodos de registro y cubrirlos con un test que construya el `ServiceProvider` con validación de ámbitos.

- [ ] **63. `MlHttpRequestExtensions` con defectos acumulados**
    - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
    - **Archivo / clase:** `MlHttpRequestExtensions`
    - **Problema:** un `nameof` apunta al parámetro equivocado en el mensaje de error; `SetHeaders` no comprueba nulos antes de recorrer la colección; todos los `*Async` son asincronía simulada; los mensajes están en inglés incorrecto.
    - **Impacto:** mensajes que señalan el parámetro erróneo (diagnóstico más lento) y posible `NullReferenceException` al pasar una colección nula.
    - **Propuesta:** guardas explícitas, `nameof` correcto, mensajes revisados y eliminación del sufijo `Async` donde no aplique.

---

## Ver también

- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — prioridades 🔴 crítica y 🟠 alta.
- [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — prioridad 🟢 baja.
- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) ·
  [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md)
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — propuesta de renombrado.
- [`README.md`](README.md) — índice de la carpeta.
