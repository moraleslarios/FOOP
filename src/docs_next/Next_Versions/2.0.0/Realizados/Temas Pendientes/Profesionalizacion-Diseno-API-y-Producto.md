# Profesionalización (2 de 2): diseño de API, asincronía y producto

> 📌 **Qué es este documento**
> Consejos sobre **cómo está diseñada la biblioteca** y cómo se percibe desde fuera: forma de la API,
> asincronía, cancelación, errores, rendimiento, observabilidad, arquitectura por capas, documentación
> y comunidad. Ningún cambio de código: decisiones de diseño argumentadas.

> ℹ️ **Documentos hermanos**
> - [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md) — repositorio,
>   compilación, analizadores, pruebas, NuGet y CI/CD.
> - [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) y
>   [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — defectos concretos del código.
> - [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — defectos críticos y de alto impacto.
> - [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — renombrados propuestos.

---

## Índice

1. [Tamaño de la superficie pública: el problema nº 1 de diseño](#1-tamaño-de-la-superficie-pública-el-problema-nº-1-de-diseño)
2. [Asincronía: reglas no negociables en una biblioteca](#2-asincronía-reglas-no-negociables-en-una-biblioteca)
3. [Cancelación](#3-cancelación)
4. [Modelo de errores: de cadenas a datos](#4-modelo-de-errores-de-cadenas-a-datos)
5. [Internacionalización de los mensajes](#5-internacionalización-de-los-mensajes)
6. [`MlResult<T>` y rendimiento](#6-mlresultt-y-rendimiento)
7. [Interoperabilidad con .NET moderno](#7-interoperabilidad-con-net-moderno)
8. [Arquitectura por capas y dependencias](#8-arquitectura-por-capas-y-dependencias)
9. [Inyección de dependencias y ciclos de vida](#9-inyección-de-dependencias-y-ciclos-de-vida)
10. [Configuración: de literales a opciones](#10-configuración-de-literales-a-opciones)
11. [Observabilidad](#11-observabilidad)
12. [Value objects y validación](#12-value-objects-y-validación)
13. [Capa web: ASP.NET Core como es hoy](#13-capa-web-aspnet-core-como-es-hoy)
14. [Documentación como producto](#14-documentación-como-producto)
15. [Ergonomía para quien la usa por primera vez](#15-ergonomía-para-quien-la-usa-por-primera-vez)
16. [Comunidad y gobernanza](#16-comunidad-y-gobernanza)
17. [Hoja de ruta sugerida](#17-hoja-de-ruta-sugerida)

---

## 1. Tamaño de la superficie pública: el problema nº 1 de diseño

`MlResultActionsMap.cs` supera las 3 000 líneas y las familias `Map*`, `Bind*`, `ExecSelf*`,
`ChangeReturnResult*` y `Match*` suman **cientos de sobrecargas** por la combinatoria de:

```text
[Try] × [operación] × [WithValue | WithoutValue | WithException | WithoutException | IfFail | IfValid | Always] × [Async]
```

Esto genera tres costes reales:

1. **IntelliSense inservible.** Al escribir `.Map` aparecen decenas de candidatas y el usuario no sabe
   cuál es «la normal».
2. **Mantenimiento multiplicado.** Cada corrección hay que replicarla en N sobrecargas, y así nacen
   las asimetrías ya detectadas (nombres que no coinciden entre la variante simple y la asíncrona, o
   entre la simple y la dúplex).
3. **Bugs que se esconden.** El `MapAlwaysAsync` que no espera la tarea pasó desapercibido justamente
   porque es una entre cientos.

### Propuestas, de menor a mayor ambición

| Nivel | Propuesta | Coste / beneficio |
|---|---|---|
| Mínimo | **Documentar el núcleo mínimo**: `Bind`, `Map`, `Match`, `EnsureFp.That`, `ExecSelf` y sus `Async`. Todo lo demás, «avanzado» | Muy bajo coste, mejora inmediata la adopción |
| Bajo | `[EditorBrowsable(EditorBrowsableState.Advanced)]` en las sobrecargas exóticas | IntelliSense limpio sin romper nada |
| Medio | **Generar** las sobrecargas con un *source generator* o plantillas T4 desde una especificación única | Elimina las asimetrías por construcción; un bug se corrige una vez |
| Medio | Dividir los archivos gigantes por familia (`Map.Core.cs`, `Map.IfFail.cs`, `Map.Async.cs`) con `partial` | Revisiones legibles, *diffs* pequeños |
| Alto | Sustituir combinaciones por **parámetros**: en lugar de `MapIfFailWithException`, un `MapIfFail(handler)` donde el `handler` recibe el `MlErrorsDetails` completo | Reduce la API un orden de magnitud, pero es *breaking* |
| Alto | Unificar `Try*` mediante un único operador `Attempt(...)` que capture excepciones, en vez de duplicar cada método | Halva la superficie |

> 💡 La regla práctica: **si dos métodos se diferencian solo en qué parte del error reciben, deberían
> ser un único método con un parámetro.** El nombre no debería codificar la firma.

---

## 2. Asincronía: reglas no negociables en una biblioteca

Este es el apartado con más riesgo de la biblioteca, porque los fallos de asincronía no se ven en
pruebas pequeñas y **aparecen en producción bajo carga**.

| Regla | Estado actual | Qué hacer |
|---|---|---|
| **Nunca `.Result` ni `.Wait()`** | El método que se usa para describir errores de respuesta HTTP es la variante sincrónica que hace `.Result` | Eliminar las variantes sincrónicas que envuelven E/S; que la ruta síncrona no exista es mejor que documentar que no se use |
| **`ConfigureAwait(false)` en toda la biblioteca** | No aplicado de forma sistemática | Activarlo y hacerlo obligatorio con `Meziantou.Analyzer` |
| **Nada de *fake async*** | `ValidateAsync` es `Task.FromResult`; varios `*Async` de HTTP no esperan nada | O el método es realmente asíncrono, o no debe llamarse `Async`. Si existe por simetría, documentarlo explícitamente |
| **`ValueTask` en rutas calientes** | Todo es `Task<MlResult<T>>` | Considerar `ValueTask<MlResult<T>>` donde el resultado suele estar disponible de inmediato (validaciones, cachés) |
| **`IAsyncEnumerable<T>`** | Ausente | Para los `All*` y las proyecciones sobre colecciones grandes, permite consumo en flujo sin materializar |
| **Paridad sync/async garantizada** | Se rompe con facilidad | Prueba paramétrica que ejecute cada par y compare el resultado (ver documento hermano) |
| **`Task` nunca `null`** | Hay firmas con `Task<...> = null!` | Un `Task` nulo es un `NullReferenceException` diferido; usar sobrecarga en lugar de valor por defecto |

Recomendación adicional: **una sola convención de nombres para la asincronía**. Hoy hay
`TryMapIAsyncf`, `ChangeReturnResultAlwais*` y variantes donde el sufijo `Async` está en distinta
posición. El sufijo debe ir **siempre al final**, sin excepciones.

---

## 3. Cancelación

**No hay `CancellationToken` en ninguna firma pública.** En 2026, una biblioteca que hace E/S
(EF Core, `HttpClient`) sin cancelación es descartada en cualquier revisión de arquitectura: una
petición web abortada por el cliente sigue consumiendo la base de datos hasta terminar.

**Propuesta:**

- Añadir `CancellationToken cancellationToken = default` como **último parámetro** de todos los
  `*Async` que hagan E/S: repositorios, servicios, clientes HTTP.
- Propagarlo a `ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`, `SendAsync`, etc.
- Traducir `OperationCanceledException` a un `MlResult` de fallo **solo si el usuario lo pide**; por
  defecto debe propagarse, porque es control de flujo, no un error de dominio.
- Documentar el criterio con un ejemplo en el `README.md` del núcleo.
- En los controladores base, tomar el token de `HttpContext.RequestAborted`.

Como es un cambio en la superficie pública, encaja perfectamente en una versión **2.0**.

---

## 4. Modelo de errores: de cadenas a datos

El diseño de `MlErrorsDetails` es acertado (colección de errores + diccionario de detalles), pero su
uso actual convierte información estructurada en texto y luego intenta recuperarla con comparaciones
de cadenas. Los síntomas ya detectados lo demuestran: una clave `"NotFound"` frente a
`"ProblemsDetails"` que provoca un 500 en lugar de un 404, y una detección de «no encontrado» por
coincidencia de subcadenas.

**Propuestas:**

1. **Códigos de error tipados.** Un `enum` o `record struct MlErrorCode(string Value)` con un catálogo
   (`NotFound`, `Validation`, `Conflict`, `Unauthorized`, `Transient`, `Unexpected`). La traducción a
   código HTTP se hace con ese dato, nunca con el texto.
2. **Constantes públicas para todas las claves de detalle.** Hoy hay literales `"Ex"`, `"Value"`,
   `"NotFound"`, `"ProblemsDetails"`, `"X-Page-Number"` y `"X-Page-Size"` repartidos y duplicados entre
   proyectos. Una única clase de constantes compartida evita el desajuste silencioso.
3. **`AddDetail` no debe mutar ni lanzar.** Hoy modifica el diccionario y lanza si la clave existe:
   dos comportamientos sorprendentes en un tipo que se presenta como inmutable. Propuesta: `WithDetail`
   que devuelva una copia, y `WithDetailOrReplace` explícito.
4. **`MlError` con más que `Message`.** Añadir `Code`, `Target`/`MemberName` y `Severity`. Hoy se
   pierden los `MemberNames` de las validaciones al convertirlas a `MlError`, y eso es exactamente lo
   que necesita un formulario para marcar el campo erróneo.
5. **Nunca reflexión para el contrato.** La construcción de `ProblemDetails` a partir de reflexión sobre
   los detalles es frágil: cualquier renombrado la rompe en tiempo de ejecución, sin aviso del
   compilador.
6. **Separar excepción de error de dominio.** Que una excepción viaje dentro de los detalles está bien
   para diagnóstico, pero no debe salir nunca hacia el cliente; hoy hay rutas donde el cuerpo completo
   de una respuesta HTTP acaba en el mensaje de error.

---

## 5. Internacionalización de los mensajes

Los mensajes por defecto están **en inglés y con faltas** (`"source no be null"`, `"is diferent
type"`, `"soported"`, `"BaseAdress"`), y algunos textos codificados están **en español**. El resultado
es una biblioteca que no se puede usar en un producto internacional sin reescribir los mensajes.

**Propuesta:**

- Todos los mensajes por defecto a **archivos de recursos** (`.resx`) con `IStringLocalizer` o, si se
  quiere evitar la dependencia, un `IMlMessageProvider` inyectable.
- Idioma neutro por defecto: **inglés correcto**, revisado.
- Cada mensaje con **plantilla y parámetros nombrados** (`"{Member} must not be null"`), no
  concatenación.
- Convención clara: **el código y los mensajes de la API en inglés; la documentación en español** (que
  es lo que ya se hace y funciona bien).
- Permitir al consumidor sustituir cualquier mensaje sin heredar clases.

---

## 6. `MlResult<T>` y rendimiento

| Aspecto | Recomendación |
|---|---|
| `readonly struct` | Si `MlResult<T>` es hoy `class`, evaluar `readonly struct` para evitar asignaciones en cadenas largas. Medir antes con BenchmarkDotNet: si `T` es grande, la copia puede ser peor |
| Camino de éxito sin asignaciones | Evitar crear `MlErrorsDetails`, listas o diccionarios vacíos cuando el resultado es válido; usar instancias singleton/`Empty` |
| `MlErrorsDetails` inmutable | `ImmutableArray<MlError>` y `FrozenDictionary` para los detalles; hoy el diccionario es mutable |
| Cadenas | `ToErrorsDescription` y compañía con `StringBuilder` o `string.Join`, y `IFormatProvider` explícito |
| *Delegates* | Cuidado con las clausuras que capturan variables en cada llamada dentro de bucles; los `Projection*` de `MlResultBucles` son candidatos a revisión |
| `[MethodImpl(AggressiveInlining)]` | En los accesores triviales (`IsValid`, `IsFail`) |
| Colecciones | Preferir `IReadOnlyList<T>` en la API pública y `Span`/`Memory` donde tenga sentido |
| Medición | **Antes que cualquier optimización**, un proyecto `benchmarks/` con escenarios reales: cadena de 5 `Bind`, 1 000 validaciones, paginación de 10 000 filas |

> ⚠️ Ninguna de estas optimizaciones debe hacerse «por si acaso». La secuencia correcta es:
> BenchmarkDotNet → identificar el 5 % que importa → optimizar solo eso → volver a medir.

---

## 7. Interoperabilidad con .NET moderno

Piezas que hoy faltan y que un consumidor da por supuestas:

- **`Result` y patrones de C# moderno**: `Deconstruct` para `var (ok, value, errors) = result;` y
  soporte de `is` / *pattern matching* con propiedades.
- **Operadores implícitos** desde `T` y desde `MlError` para escribir `return value;` sin ceremonia
  (con cuidado: los operadores implícitos ambiguos ya han causado un problema detectado).
- **`LINQ query syntax`**: implementar `Select`, `SelectMany` y `Where` con las firmas que espera el
  compilador permite escribir `from a in ... from b in ...`, que es la forma más legible de encadenar.
- **`System.Text.Json`**: convertidores para `MlResult<T>` y para los *value objects*, más
  `JsonSerializerOptions` **inyectable** (hoy se usan las opciones por defecto sin posibilidad de
  configurarlas).
- **Serialización de *value objects***: convertidores para EF Core (`ValueConverter`), para
  `System.Text.Json` y para el enlace de modelos de ASP.NET Core (`TypeConverter`), de modo que
  `Key`, `Name` o `ExistsFile` puedan usarse directamente en entidades y DTOs.
- **`IParsable<T>` y `ISpanParsable<T>`** en los *value objects*: los hace utilizables en rutas y
  *query strings* sin código adicional.
- **Interfaces genéricas estáticas** (`static abstract`) para las factorías `From*` / `By*`, que hoy
  son métodos estáticos sin contrato común.
- **Analizador propio**: un pequeño analizador que avise cuando un `MlResult` devuelto **se descarta**
  sin comprobar. Es el error nº 1 de todos los usuarios de tipos *result*; `[MustUseReturnValue]` o un
  diagnóstico propio lo evitan.

---

## 8. Arquitectura por capas y dependencias

La separación en proyectos es buena, pero hay dos problemas de dependencia:

1. **Duplicación entre capas.** El mismo código aparece copiado en más de un sitio: la construcción de
   la cadena de valores de clave primaria existe en `WebControllers` y en `HttpClients`; el catálogo de
   `ProblemDetails` está duplicado entre `WebServices` y `WebApi`; las cabeceras de paginación son
   literales repetidos en dos proyectos. Cuando se corrige uno y no el otro, cliente y servidor dejan
   de entenderse.
   **Propuesta:** un proyecto `MoralesLarios.OOFP.Web.Abstractions` con el **contrato compartido**
   (nombres de cabeceras, forma del `ProblemDetails`, protocolo de claves compuestas) del que dependan
   `WebApi`, `WebControllers`, `WebServices` y `HttpClients`.
2. **`Internals` público.** Un proyecto llamado `Internals` que se publica como paquete es una
   contradicción. Si es infraestructura compartida, merece un nombre honesto (`Abstractions`,
   `Primitives`); si es realmente interno, `InternalsVisibleTo` y no publicarlo.

Otras recomendaciones:

- **Test de arquitectura** con `NetArchTest` o `ArchUnitNET`: «el núcleo no referencia ASP.NET Core»,
  «`ValueObjects` no referencia EF Core», «nada en `src/` referencia `Newtonsoft.Json`». Se ejecuta en
  CI y protege el diseño mejor que cualquier documento.
- **Diagrama de dependencias** en el `README.md` raíz (Mermaid), para que se vea de un vistazo qué
  arrastra cada paquete.
- Que el núcleo **no dependa de nada** fuera de la BCL. Es su mayor argumento de venta.

---

## 9. Inyección de dependencias y ciclos de vida

Los defectos ya identificados aquí (construcción de un `ServiceProvider` en cada resolución, un
`DbContext` del contenedor liberado por el repositorio, un *singleton* que captura servicios *scoped*,
un `AddWebControllers` que no hace nada, delegados no resolubles) tienen una **causa común**: el
registro se hace con genéricos abiertos y factorías manuales en lugar de con las capacidades del
contenedor.

**Propuestas de diseño:**

- **Patrón `Add…` + `IOptions`**: un único punto de entrada por proyecto
  (`AddMlOofpEfCore(options => …)`), con `options` validado por `ValidateOnStart()`.
- **Nunca `BuildServiceProvider()`** dentro de un registro; usar `IServiceProvider` inyectado o
  factorías con `ActivatorUtilities`.
- **No implementar `IDisposable`** en un servicio que recibe sus dependencias por inyección: el
  contenedor es el dueño del ciclo de vida.
- **Registros con clave** (`AddKeyedScoped`, disponible desde .NET 8) para el escenario de varias
  configuraciones del mismo servicio, en lugar de la gestión manual de claves que hoy falla.
- **Un solo ciclo de vida documentado por tipo** y una tabla en el `README.md` de cada proyecto:
  *tipo → ciclo de vida → dependencias → seguridad ante hilos*.
- **Comprobación de configuración en el arranque**: si falta una `BaseAddress` o una cadena de
  conexión, la aplicación debe fallar al arrancar, no en la primera petición.
- **Métodos de registro cubiertos por pruebas**: un test que construya el `ServiceProvider` con
  `ValidateScopes = true` y `ValidateOnBuild = true` habría detectado tres de los defectos actuales.

---

## 10. Configuración: de literales a opciones

Hoy hay decisiones de producto **incrustadas en el código**: un dominio concreto del autor como valor
por defecto de `Location` y del campo `type` de `ProblemDetails`, nombres de cabecera como literales,
formatos de fecha fijos, tamaños de página sin límite y rutas construidas a mano.

**Propuesta:** una clase de opciones por proyecto, con valores por defecto **neutros**:

```text
MlWebOptions          → BaseProblemTypeUri, PageNumberHeader, PageSizeHeader,
                        MaxPageSize, DefaultPageSize, IncludeExceptionDetails (false)
MlEfCoreOptions       → DefaultPageSize, MaxPageSize, EnableUnitOfWork, TrackingBehavior
MlHttpClientOptions   → BaseAddress, JsonSerializerOptions, DefaultHeaders, Timeout, RetryPolicy
MlValidationOptions   → MessageProvider, StopOnFirstError
```

Con `IValidateOptions<T>` y `ValidateOnStart()`. El valor por defecto de un URI de tipo de problema
debe ser **`about:blank`** (lo que dice el RFC 9457), nunca un dominio de terceros.

---

## 11. Observabilidad

Hay `ILogger<>` **inyectado y nunca usado** en cuatro clases de cliente HTTP, y un proyecto entero de
extensiones de log. La oportunidad es grande y el coste bajo.

- **`LoggerMessage` con *source generator*** (`[LoggerMessage(EventId = …, Level = …, Message = "…")]`):
  cero asignaciones, `EventId` estables y mensajes con parámetros nombrados. Es la forma correcta en
  2026, y sustituye a los `logger.LogInformation($"…")` interpolados.
- **`EventId` catalogados** en una clase de constantes, documentados en el `README.md` de logging.
- **`ActivitySource` y métricas** (`Meter`) para trazas distribuidas compatibles con OpenTelemetry: una
  actividad por operación de repositorio y por llamada HTTP, con el resultado (`valid`/`fail`) como
  etiqueta. Esto convierte la biblioteca en «observable por defecto», que es un argumento de adopción
  muy fuerte.
- **Nunca registrar datos sensibles**: revisar que ningún mensaje volcado incluya cuerpos de respuesta
  completos, cabeceras de autorización ni cadenas de conexión.
- **Redacción configurable** del contenido de los errores según el entorno: detalle en desarrollo,
  mensaje genérico en producción.
- **Correlación**: propagar `TraceId` en los `MlErrorsDetails` para que un error devuelto al cliente
  pueda cruzarse con los logs del servidor.

---

## 12. Value objects y validación

El conjunto de *value objects* es una de las partes más útiles de la biblioteca, y también la que más
se beneficiaría de un rediseño de base, porque los defectos detectados (tipos imposibles de construir,
un campo estático público mutable, parámetros de longitud ignorados, constructores públicos que se
saltan la validación, mensajes con mínimo y máximo intercambiados) son todos **variantes del mismo
patrón mal aplicado**.

**Propuestas:**

- **Una única clase base** que garantice por construcción que no se puede crear una instancia inválida:
  constructor `protected`, factoría estática `Create` que devuelva `MlResult<T>` y ningún camino
  alternativo. Si el constructor público existe, tarde o temprano alguien lo usará.
- **`static abstract` en la base genérica** (`IMlValueObject<TSelf, TValue>`) para que el propio
  compilador exija que el parámetro genérico sea el tipo que deriva. Hoy nada lo impide.
- **Nunca estado estático mutable.** Los límites son parte del tipo, no una variable global.
- **Igualdad y orden coherentes**: `record`/`readonly record struct`, `IComparable<T>`,
  `IEquatable<T>`, `GetHashCode` alineado y `ToString()` con `IFormatProvider`.
- **Un solo motor de validación en la fachada.** Hoy conviven `EnsureFp`, DataAnnotations y
  FluentValidation con puentes distintos; conviene una interfaz única (`IMlValidator<T>`) y que los
  tres sean implementaciones intercambiables.
- **Conservar `MemberNames`** al traducir resultados de validación, y no forzar el desempaquetado de
  colecciones que pueden ser nulas.
- **`ValidationContext` con `IServiceProvider`** para permitir validaciones que resuelvan servicios.
- **Distinguir «validar» de «asegurar»**: `Validate` devuelve todos los errores; `Ensure` corta en el
  primero. Hoy no está claro cuál hace qué.
- **Colisión de extensiones**: dos métodos de extensión `ValidateObject` sobre `object` en dos
  *namespaces* que se importan juntos producen error de compilación en el código del consumidor. Los
  métodos de extensión sobre `object` deberían evitarse por completo.

---

## 13. Capa web: ASP.NET Core como es hoy

Los controladores base y las extensiones de resultado están escritos con el estilo de ASP.NET Core 2.x.
Actualizarlos a 8.x mejora la biblioteca a todos los niveles:

| Tema | Recomendación |
|---|---|
| Referencias | `FrameworkReference` a `Microsoft.AspNetCore.App`, no `PackageReference` a `Mvc.Core` 2.1.0 |
| `ProblemDetails` | Usar `IProblemDetailsService` y `ProblemDetails` del framework, alineado con **RFC 9457**; `type` por defecto `about:blank` |
| Excepciones | `IExceptionHandler` (.NET 8) en lugar de comprobaciones dispersas |
| Filtros | `IEndpointFilter` y filtros de resultado en lugar de helpers de conversión repetidos |
| Rutas | Coherentes entre verbos: si `GET` usa un patrón, `PUT` y `DELETE` deben usar el mismo |
| Verbos | `DELETE` **sin cuerpo**: la clave va en la ruta; muchos intermediarios descartan el cuerpo de un `DELETE` |
| `Location` | Generado con `LinkGenerator`/`Url.Action`, nunca un dominio codificado |
| `[ProducesResponseType]` | En todos los métodos base, más `[Produces("application/json")]` |
| Bases de controlador | `abstract`, con `[Route]` propio y sin código comentado |
| OpenAPI | Que `PkParameterAttribute` funcione de verdad mediante `IOperationFilter`, o eliminarlo |
| Minimal APIs | Ofrecer también `MapMlCrud<TEntity, TDto>()`: hoy es la forma en que mucha gente escribe APIs, y da acceso a un público que no usará controladores |
| Validación de entrada | Límite máximo de tamaño de página y validación de los parámetros de paginación |
| Códigos HTTP | Derivados del **código de error tipado**, nunca de comparar textos |
| Idempotencia y concurrencia | `ETag`/`If-Match` en `PUT` y `DELETE`; hoy hay una doble consulta con carrera entre lectura y escritura |

---

## 14. Documentación como producto

La documentación en `__Doc` es extensa y de calidad; lo que falta es **distribución**.

- **Sitio publicado** con DocFX o Docusaurus en GitHub Pages, con búsqueda. Un `.md` en un repositorio
  no se encuentra en Google; un sitio sí.
- **API reference generada** desde los comentarios XML (`GenerateDocumentationFile` ya propuesto).
- **`<example>` en los XML doc** de los métodos del núcleo mínimo: es lo que ve el usuario en el
  *tooltip*, y hoy IntelliSense es la primera documentación que consulta cualquiera.
- **Ejemplos compilables**: una carpeta `samples/` con proyectos que **se compilen en CI**. Un ejemplo
  que no compila es peor que no tener ejemplo. Idealmente, extraer los fragmentos de la documentación
  desde código real para que nunca queden obsoletos.
- **Guía de migración** por versión mayor, y una sección «errores frecuentes» (descartar un
  `MlResult`, mezclar `Map` con `Bind`, usar la variante sincrónica de una operación de E/S).
- **ADRs** (`docs/adr/`) con las decisiones de diseño y su motivo: por qué `MlResult` y no
  excepciones, por qué `IsValid`/`IsFail`, por qué esta jerarquía de errores. Es lo que evita que la
  discusión se repita cada seis meses.
- **Comparativa honesta** con `LanguageExt`, `CSharpFunctionalExtensions`, `FluentResults` y
  `OneOf`: en qué casos esta biblioteca es mejor elección. Genera más confianza que cualquier
  argumento de venta.

---

## 15. Ergonomía para quien la usa por primera vez

Prueba mental: alguien encuentra el paquete y tiene **cinco minutos**. Hoy ese recorrido es difícil.

- **`README.md` con un ejemplo de 15 líneas en los primeros 30 segundos**: instalar, un `Bind`, un
  `Match`, resultado. Todo lo demás, después.
- **Núcleo mínimo señalizado**: seis métodos que resuelven el 90 % de los casos, marcados como tal en
  la documentación y en IntelliSense.
- **Plantilla `dotnet new`** (`dotnet new mloofp-api`) con una API funcional completa: es la vía más
  rápida de adopción y demuestra el estilo previsto.
- **Errores de compilación amables**: mensajes de excepción y de validación que digan *qué hacer*, no
  solo qué ha fallado.
- **Analizador que avise de resultados descartados** (ver §7).
- **Snippets** para Visual Studio y VS Code.
- **Una sola forma de hacer cada cosa** en la documentación. Si hay tres maneras de validar, el
  lector se bloquea; hay que recomendar una y mencionar las otras al final.

---

## 16. Comunidad y gobernanza

- `LICENSE` explícito en la raíz (**hoy falta**): sin licencia, ninguna empresa puede usar el paquete.
- `CONTRIBUTING.md` con el flujo de trabajo, el estilo y cómo ejecutar las pruebas.
- `CODE_OF_CONDUCT.md` y `SECURITY.md`.
- Plantillas de *issue* y de *pull request*, con etiquetas (`bug`, `breaking`, `good first issue`).
- **Hoja de ruta pública** y los inventarios de mejoras convertidos progresivamente en *issues* con hitos.
- **Insignias** en el `README.md`: NuGet, compilación, cobertura, licencia, framework de destino.
- **Discusiones** de GitHub habilitadas para preguntas, en lugar de *issues*.
- Declarar el **nivel de soporte** y la política de versiones: da tranquilidad a quien evalúa adoptarla.

---

## 17. Hoja de ruta sugerida

Cuatro fases, pensadas para que cada una entregue valor por sí sola.

### Fase 1 — Credibilidad (semanas 1-2, sin cambios de comportamiento)

`LICENSE`, limpieza del repositorio, `Directory.Build.props`, `Directory.Packages.props`, analizadores
en modo advertencia, `.editorconfig`, CI básica en Linux y Windows. **Resultado:** el repositorio ya
parece profesional y la CI empieza a proteger.

### Fase 2 — Corrección (semanas 3-6, versión 1.x de parches)

Todos los defectos 🔴 y los 🟠 de seguridad e inyección de dependencias, cada uno con su prueba.
Pruebas de leyes y de paridad sync/async. Cobertura publicada. **Resultado:** la biblioteca es fiable.

### Fase 3 — Diseño (versión 2.0, con cambios *breaking* documentados)

`CancellationToken` en toda la API asíncrona, códigos de error tipados, opciones configurables en
lugar de literales, capa web modernizada a ASP.NET Core 8, *value objects* rediseñados con factoría
única, mensajes en recursos. Guía de migración y `CHANGELOG.md`. **Resultado:** la API es defendible en
una revisión de arquitectura.

### Fase 4 — Adopción (continuo)

Reducción de la superficie pública (generación de sobrecargas), sitio de documentación publicado,
`samples/` en CI, plantilla `dotnet new`, analizador de resultados descartados, observabilidad con
OpenTelemetry, `benchmarks/` y optimización guiada por medición. **Resultado:** la biblioteca se
encuentra, se entiende y se elige.

---

## Ver también

- [`Profesionalizacion-Ingenieria-y-Calidad.md`](Profesionalizacion-Ingenieria-y-Calidad.md)
- [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) ·
  [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md)
- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — defectos críticos y de alto impacto.
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — renombrados propuestos.
