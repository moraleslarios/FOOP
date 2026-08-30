```markdown
# ➡️ Documento movido y dividido en tres

> ⚠️ **Este archivo está obsoleto y su contenido está repetido.** El inventario de mejoras vive ahora
> en la carpeta **`Temas Pendientes`**, separado por niveles de prioridad y sin duplicados:
>
> | Prioridad | Puntos | Documento |
> |---|---|---|
> | 🔴 crítica + 🟠 alta | 1-37 | [`Temas Pendientes/Mejoras-Prioridad-Critica-y-Alta.md`](Temas%20Pendientes/Mejoras-Prioridad-Critica-y-Alta.md) |
> | 🟡 media | 38-63 | [`Temas Pendientes/Mejoras-Prioridad-Media.md`](Temas%20Pendientes/Mejoras-Prioridad-Media.md) |
> | 🟢 baja | 64-89 | [`Temas Pendientes/Mejoras-Prioridad-Baja.md`](Temas%20Pendientes/Mejoras-Prioridad-Baja.md) |
>
> Índice de la carpeta: 🗂️ [`Temas Pendientes/README.md`](Temas%20Pendientes/README.md).
>
> **Este archivo puede eliminarse.** Todo lo que sigue se conserva únicamente como histórico.
> clase** y el **método** donde está el problema, qué consecuencia tiene y una propuesta concreta
> de arreglo. La idea es que puedas ir marcando casillas poco a poco sin tener que volver a
> investigar el código.

> ℹ️ Los renombrados de clases, métodos y propiedades **no** están aquí: viven en
> [`CONSEJOS_NOMENCLATURA.md`](CONSEJOS_NOMENCLATURA.md). Este documento trata de **comportamiento**
> (bugs, seguridad, rendimiento, corrección de API y deuda técnica).

---

## Índice

1. [Cómo usar este documento](#cómo-usar-este-documento)
2. [Resumen por prioridad](#resumen-por-prioridad)
3. [Resumen por proyecto](#resumen-por-proyecto)
4. [🔴 Prioridad crítica — bugs que producen resultados incorrectos](#-prioridad-crítica--bugs-que-producen-resultados-incorrectos)
5. [🟠 Prioridad alta — seguridad, fiabilidad y contratos rotos](#-prioridad-alta--seguridad-fiabilidad-y-contratos-rotos)
6. [🟡 Prioridad media — rendimiento, diseño y API pública](#-prioridad-media--rendimiento-diseño-y-api-pública)
7. [🟢 Prioridad baja — limpieza, coherencia y documentación](#-prioridad-baja--limpieza-coherencia-y-documentación)
8. [Plan de trabajo sugerido](#plan-de-trabajo-sugerido)

---

## Cómo usar este documento

Cada ficha tiene siempre la misma forma:

```text
- [ ] **N. Título del problema**
      - **Proyecto:** nombre del .csproj
      - **Archivo / clase:** ruta relativa y tipo afectado
      - **Miembro:** método, propiedad o línea concreta
      - **Problema:** qué hace hoy
      - **Impacto:** qué consecuencia tiene para quien usa la biblioteca
      - **Propuesta:** cómo arreglarlo
```

### Criterio de prioridad

| Nivel | Significado | Cuándo atacarlo |
|---|---|---|
| 🔴 **Crítico** | El código **devuelve un resultado incorrecto** o un tipo público es imposible de usar. Silencioso: compila y no avisa. | Ya. Cada uno es un parche independiente. |
| 🟠 **Alto** | Riesgo de **seguridad**, de fuga de datos, de bloqueo, o un contrato público roto (códigos HTTP erróneos, dependencias mal registradas). | En la próxima versión menor. |
| 🟡 **Medio** | **Rendimiento** o decisiones de diseño que se pagan al crecer, y asimetrías de API que confunden. | Cuando toques ese proyecto. |
| 🟢 **Bajo** | Limpieza, mensajes, ficheros muertos, documentación. | En cualquier hueco; ideal para un primer PR. |

> 💡 **Sugerencia de método.** Antes de arreglar un 🔴, escribe la prueba que lo demuestra. Muchos
> de estos fallos existen precisamente porque no había test: si arreglas sin test, el siguiente
> refactor lo reintroduce.

---

## Resumen por prioridad

| Prioridad | Nº de puntos | Naturaleza dominante |
|---|---|---|
| 🔴 Crítico | 16 | Bugs de lógica y tipos inconstruibles |
| 🟠 Alto | 21 | Seguridad, DI, códigos HTTP, culturas |
| 🟡 Medio | 26 | Rendimiento en EF Core, diseño de API |
| 🟢 Bajo | 24 | Ficheros muertos, mensajes, coherencia |
| **Total** | **87** | |

---

## Resumen por proyecto

| Proyecto | 🔴 | 🟠 | 🟡 | 🟢 | Punto más urgente |
|---|---|---|---|---|---|
| `MoralesLarios.OOFP` (núcleo) | 2 | 0 | 1 | 3 | `FusionFailErros` sin `return` |
| `MoralesLarios.OOFP.ValueObjects` | 4 | 1 | 2 | 2 | `DecimalNotNegative` inconstruible |
| `MoralesLarios.OOFP.ValueObjects.IO` | 2 | 1 | 1 | 1 | Paréntesis mal anidados en `ExistsFile` |
| `…Validation.Dataannotations` | 2 | 1 | 1 | 2 | Colisión de `ValidateObject` (CS0121) |
| `MoralesLarios.OOFP.EFCore` | 1 | 4 | 6 | 3 | `DbContext` liberado por el repositorio |
| `MoralesLarios.OOFP.WebServices` | 1 | 3 | 3 | 3 | 500 en lugar de 404 |
| `MoralesLarios.OOFP.WebApi` | 1 | 3 | 2 | 2 | `Created` incluso al fallar |
| `MoralesLarios.OOFP.WebControllers` | 1 | 5 | 6 | 5 | Culturas en el parseo de fechas |
| `MoralesLarios.OOFP.HttpClients` | 2 | 3 | 4 | 3 | Volcado del cuerpo de respuesta en el error |
| `MoralesLarios.OOFP.Internals` | 0 | 0 | 0 | 1 | `[Range(0, int.MinValue)]` |

---

## 🔴 Prioridad crítica — bugs que producen resultados incorrectos

Todo lo de esta sección **compila sin avisos** y devuelve valores erróneos o hace imposible usar un
tipo público. Es el bloque que hay que cerrar antes de publicar cualquier versión nueva.

- [ ] **1. `FusionFailErros` no devuelve el resultado fusionado**
      - **Proyecto:** `MoralesLarios.OOFP`
      - **Archivo / clase:** `Types/MlResultBucles.cs` → `MlResultBucles`
      - **Miembro:** `FusionFailErros`, ~línea 707
      - **Problema:** el método construye el `MlResult` con los errores fusionados pero **le falta la
        sentencia `return`** en una de las ramas, por lo que se devuelve el valor por defecto en
        lugar de la fusión.
      - **Impacto:** al combinar varios resultados fallidos, **se pierden errores**. Quien depende de
        `FusionFailErros` para agregar validaciones recibe un resultado incompleto y no hay ninguna
        excepción que lo delate.
      - **Propuesta:** añadir el `return` que falta y cubrirlo con un test que fusione 3 fallos y
        compruebe que el resultado contiene los 3 mensajes.

- [ ] **2. `MapAlwaysAsync` no espera la tarea de origen**
      - **Proyecto:** `MoralesLarios.OOFP`
      - **Archivo / clase:** `Types/MlResultActionsMap.cs` → `MlResultActionsMap`
      - **Miembro:** `MapAlwaysAsync<T, TReturn>(Task<MlResult<T>> sourceAsync, Func<Task<TReturn>> …)`, ~línea 3115
      - **Problema:** la sobrecarga **nunca hace `await sourceAsync`**: ejecuta la función de
        transformación sin esperar a que la tarea de origen termine.
      - **Impacto:** *race condition*. El resultado puede calcularse antes de que el origen haya
        terminado, y si el origen lanza, la excepción queda como *unobserved task exception*. En
        cargas bajas parece funcionar; en producción falla de forma intermitente.
      - **Propuesta:** `var source = await sourceAsync;` al principio y usar `source` en el resto del
        cuerpo. Revisar las sobrecargas hermanas de `MapAlways*` buscando el mismo patrón.

- [ ] **3. `DecimalNotNegative` y `DoubleNotNegative` son imposibles de construir**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `DecimalNotNegative`, `DoubleNotNegative`
      - **Miembro:** `IsValid(value)` frente al constructor de la clase base
      - **Problema:** el `IsValid` de la clase derivada implementa `value < 0` (es decir: «es válido si
        es negativo», justo lo contrario de lo que dice el nombre), mientras que el constructor base
        valida `value > 0`. Ambas condiciones **no se pueden satisfacer a la vez**.
      - **Impacto:** **el tipo público no se puede instanciar con ningún valor**. Cualquiera que lo
        use recibe siempre un fallo, incluido el `0` que el nombre promete aceptar.
      - **Propuesta:** `IsValid(value) => value >= 0` y alinear la validación del constructor base
        para que use el mismo predicado. Test con `-1` (falla), `0` (válido) y `1` (válido).

- [ ] **4. `IntNotNegative.limit` es un campo público estático mutable**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `IntNotNegative`
      - **Miembro:** `public static int limit;`
      - **Problema:** el límite de validación es un **campo público, estático y no `readonly`**, sin
        inicializar (vale `0`).
      - **Impacto:** cualquier código del proceso puede cambiar el límite y alterar la validación
        **de toda la aplicación**, incluidas otras peticiones concurrentes. Es un estado global
        mutable en un objeto de valor, que por definición debe ser inmutable.
      - **Propuesta:** convertirlo en `private const int Limit = 0;` o, si de verdad debe ser
        configurable, pasarlo por constructor a una instancia.

- [ ] **5. `StringBetweenLength` intercambia mínimo y máximo en el mensaje de error**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `StringBetweenLength`
      - **Miembro:** `BuildErrorMessage`
      - **Problema:** el mensaje se compone usando el máximo donde debería ir el mínimo y al revés.
      - **Impacto:** el usuario final lee *«la longitud debe estar entre 50 y 3»*. Un mensaje de
        validación invertido hace perder tiempo depurando en el lado del consumidor.
      - **Propuesta:** corregir el orden de los argumentos y añadir un test que compare el mensaje
        completo, no solo que exista error.

- [ ] **6. `Key.IsValid` y `Name.IsValid` ignoran el parámetro `length`**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `Key`, `Name`
      - **Miembro:** `IsValid(string value, int length)`
      - **Problema:** el parámetro `length` se recibe pero **no se usa** en la comprobación.
      - **Impacto:** la restricción de longitud **no se aplica nunca**: se aceptan valores más largos
        de lo declarado y, si detrás hay una columna `nvarchar(n)`, el fallo aparece más tarde como
        un error de base de datos difícil de rastrear.
      - **Propuesta:** incluir `value.Length <= length` en el predicado. Añadir además el punto 7.

- [ ] **7. `Key` y `Name` tienen constructor público que salta la validación**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `Key`, `Name`
      - **Miembro:** constructores públicos
      - **Problema:** además de las fábricas que devuelven `MlResult`, existe un constructor público
        que permite crear el objeto de valor **sin pasar por `IsValid`**.
      - **Impacto:** rompe la garantía central de un *value object*: «si existe la instancia, el
        valor es válido». A partir de ahí, ninguna capa puede confiar en el tipo.
      - **Propuesta:** hacer los constructores `private` (o `protected`) y dejar solo las fábricas
        como punto de entrada. Es un cambio de API pública: requiere versión mayor.

- [x] **8. `RangeEnumValueObject` está entero comentado**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects`
      - **Archivo / clase:** `RangeEnumValueObject.cs`
      - **Miembro:** todo el fichero
      - **Problema:** el contenido del fichero está **comentado en su totalidad**.
      - **Impacto:** una funcionalidad anunciada (validar que un valor pertenece a un `enum`) no
        existe. Quien la busque por el nombre del fichero creerá que está disponible.
      - **Propuesta:** decidir: implementarlo y cubrirlo con tests, o eliminar el fichero. No dejarlo
        en el estado intermedio.

- [x] **9. `ExistsFile.ByString` y `ExistDirectory.ByString` tienen los paréntesis mal anidados**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects.IO`
      - **Archivo / clase:** `ExistsFile`, `ExistDirectory`
      - **Miembro:** `ByString`
      - **Problema:** el `.Map(…)` queda **dentro** del `.Bind(…)` en lugar de encadenado después,
        por un paréntesis colocado en el sitio equivocado.
      - **Impacto:** la composición no es la esperada: la transformación se aplica en un punto
        distinto de la cadena, de forma que el resultado o el error propagado no corresponden a lo
        documentado. Compila sin avisos porque los tipos encajan por casualidad.
      - **Propuesta:** reescribir la cadena con una expresión por línea (una por operador) para que
        el anidamiento sea visible, y añadir tests con ruta existente, ruta inexistente y ruta nula.

- [x] **10. Los constructores de `ExistsFile` y `ExistDirectory` lanzan `ArgumentNullException`**
      - **Proyecto:** `MoralesLarios.OOFP.ValueObjects.IO`
      - **Archivo / clase:** `ExistsFile`, `ExistDirectory`
      - **Miembro:** constructores
      - **Problema:** en una biblioteca cuyo contrato es «los fallos viajan en `MlResult`», estos
        constructores **lanzan excepciones**.
      - **Impacto:** rompe el modelo de errores en el punto más sensible (E/S de disco, donde el
        fallo es lo habitual). El consumidor que compone con `Bind`/`Map` no espera un `throw`.
      - **Propuesta:** constructores privados sin validación y toda la comprobación en las fábricas
        que devuelven `MlResult`. Si hay que mantener el `throw`, documentarlo explícitamente.

- [ ] **11. Las sobrecargas de `DataannotationsValidator.ValidateAsync<T>` se saltan las guardas**
      - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
      - **Archivo / clase:** `DataannotationsValidator`
      - **Miembro:** sobrecargas de `ValidateAsync<T>`
      - **Problema:** la versión sincrónica comprueba `NotNull`/`NotEmpty` antes de validar, pero las
        sobrecargas asíncronas **no lo hacen** y pasan la referencia directamente al validador.
      - **Impacto:** **`NullReferenceException`** en lugar de un `MlResult` fallido. Es el peor caso
        posible: la ruta asíncrona (la que se usa en web) es la insegura.
      - **Propuesta:** extraer las guardas a un método privado y llamarlo desde **todas** las
        sobrecargas. Test con `null` en cada una de ellas.

- [ ] **12. `ValidateObject` colisiona con la extensión del núcleo (CS0121)**
      - **Proyecto:** `MoralesLarios.OOFP.Validation.Dataannotations`
      - **Archivo / clase:** `Helpers/Extensions` → `ValidateObject(this object source)`
      - **Miembro:** el método de extensión completo
      - **Problema:** existe un método de extensión con **la misma firma** en el núcleo
        (`MoralesLarios.OOFP.Helpers.Extensions`). Si un fichero tiene los dos `using`, el compilador
        emite **CS0121: la llamada es ambigua**.
      - **Impacto:** el consumidor **no puede compilar** usando los dos paquetes a la vez sin
        cualificar el nombre completo. Es un bloqueo de adopción.
      - **Propuesta:** renombrar el de este proyecto (por ejemplo `ValidateDataAnnotations`) o
        acotar el genérico para que no coincida la firma. Marcar el antiguo `[Obsolete]`.

- [ ] **13. La clave `httpClientFactoryKey` se queda siempre en `null`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `RegisterServices`
      - **Miembro:** `AddGenClientFp`, `AddGenClientComplexFp`, `AddGenClientDuplexComplexFp`
      - **Problema:** los tres métodos declaran `Key httpClientFactoryKey = null!;` y a continuación
        invocan `configureHttpClientKey(httpClientFactoryKey)`. Como `Key` es un tipo de referencia y
        el delegado recibe **una copia de la referencia**, la asignación que haga el delegado **no
        vuelve** a la variable local: la clave sigue siendo `null`.
      - **Impacto:** el cliente se registra con clave nula, así que en tiempo de ejecución se resuelve
        el `HttpClient` equivocado (o el por defecto, sin `BaseAddress`). El síntoma aparece lejos de
        la causa: peticiones a la URL incorrecta.
      - **Propuesta:** cambiar la firma para que el delegado **devuelva** la clave
        (`Func<Key> configureClientName`) o para que reciba un objeto de opciones mutable. Añadir una
        validación que falle rápido si la clave resultante es nula o vacía.

- [ ] **14. `AddGenClientComplexFp` registra el servicio sin la clave**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `RegisterServices`
      - **Miembro:** `AddGenClientComplexFp`
      - **Problema:** a diferencia de sus hermanos, este método registra `TService` **sin asociarlo a
        la clave** del `HttpClient` nombrado.
      - **Impacto:** el cliente de clave compuesta acaba usando el `HttpClient` por defecto. Si en la
        aplicación hay varios *backends* registrados con nombre, **las peticiones se van al servidor
        equivocado**, con el riesgo de enviar datos a un destino no previsto.
      - **Propuesta:** homogeneizar el registro de los tres métodos extrayendo el cuerpo común a un
        método privado, de forma que el registro con clave sea imposible de olvidar.

- [ ] **15. `ToSimpleRepoPostActionResult` devuelve `Created` incluso cuando el resultado falla**
      - **Proyecto:** `MoralesLarios.OOFP.WebApi`
      - **Archivo / clase:** `Helpers/MlResultWebExtensions`
      - **Miembro:** `ToSimpleRepoPostActionResult`
      - **Problema:** el método no distingue el estado del `MlResult`: construye un
        `Created(...)` en ambas ramas.
      - **Impacto:** **el cliente recibe `201 Created` cuando la operación ha fallado.** Es el bug más
        grave de la capa web: el consumidor da por guardada una entidad que no existe y no reintenta.
        Además rompe cualquier reintento idempotente que se apoye en el código de estado.
      - **Propuesta:** usar el `Match` de `MlResult` para devolver `Created` solo en la rama válida y
        `ProblemDetails` en la rama fallida. Test de integración que compruebe el código de estado
        con un repositorio que devuelva fallo.

- [ ] **16. `BuildNotFoundPkError` usa una clave de detalle equivocada y convierte 404 en 500**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `BuildNotFoundPkError`
      - **Problema:** añade el detalle con la clave `"NotFound"`, pero la capa web
        (`MlErrorsDetailsExtensions.ToProblemsDetailsInfo`) busca la clave `"ProblemsDetails"` para
        decidir el código HTTP. Al no encontrarla, cae en el camino por defecto.
      - **Impacto:** **una entidad inexistente devuelve `500 Internal Server Error` en lugar de
        `404 Not Found`.** Consecuencias en cascada: los reintentos automáticos de los clientes se
        disparan, las alarmas de monitorización se llenan de falsos 5xx y el consumidor no puede
        distinguir «no existe» de «el servidor está roto».
      - **Propuesta:** unificar las claves de detalle en **constantes públicas compartidas** (en el
        proyecto `Internals`/`Shared`) y usarlas en los dos lados. Añadir un test que recorra el
        camino completo servicio → controlador y verifique el `404`.

> ⚠️ **Nota transversal sobre los puntos 15 y 16.** Los dos comparten la misma raíz: **el contrato
> entre `WebServices` y `WebApi` se apoya en cadenas literales** repetidas en dos proyectos. Mientras
> siga siendo así, cualquier arreglo puntual se volverá a romper. La solución de fondo es un tipo o
> un conjunto de constantes compartidas que represente ese contrato.

---

## 🟠 Prioridad alta — seguridad, fiabilidad y contratos rotos

Nada de esta sección devuelve datos incorrectos, pero cada punto puede provocar una **fuga de
información**, un **bloqueo**, un **fallo intermitente** o un comportamiento distinto según el
servidor donde se despliegue.

### Seguridad y privacidad

- [ ] **17. `ToResponseErrorsDescription` volca el cuerpo completo de la respuesta en el error**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions`
      - **Miembro:** `ToResponseErrorsDescription`
      - **Problema:** cuando la llamada HTTP falla, el método lee el cuerpo completo de la respuesta
        remota y lo incorpora **literalmente** al mensaje de error del `MlResult`.
      - **Impacto:** **fuga de datos sensibles.** Ese mensaje termina en los logs y, a través de
        `ProblemDetails`, muchas veces en la respuesta al cliente final. Si el servicio remoto
        devuelve un *stack trace*, una cadena de conexión, un token o datos personales de otro
        usuario, todo eso queda registrado.
      - **Propuesta:** guardar el cuerpo **solo en `Details`** (no en `Message`), truncarlo a un
        máximo configurable, y hacerlo dependiente de un flag de diagnóstico que esté **apagado por
        defecto**. En producción, registrar únicamente el código de estado y la URL sin *query string*.

- [ ] **18. `ToResponseErrorsDescription` bloquea con `.Result`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions`
      - **Miembro:** `ToResponseErrorsDescription` (versión sincrónica, que es la que se usa)
      - **Problema:** lee el contenido con `.Result` sobre una tarea de E/S en lugar de `await`.
      - **Impacto:** **riesgo de *deadlock*** en cualquier contexto con `SynchronizationContext`, y
        bloqueo de un hilo del *thread pool* en ASP.NET Core. Bajo carga se traduce en *thread pool
        starvation*: la aplicación entera se vuelve lenta sin que ningún log lo explique.
      - **Propuesta:** convertir el método en `ReadErrorDescriptionAsync` con `await` y propagar el
        `async` a los llamadores. Marcar la versión sincrónica `[Obsolete]`.

- [ ] **19. Los mensajes de error internos llegan tal cual al cliente final**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`, `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `Helpers/Extensions`, clases base de controlador
      - **Miembro:** `GetPkValuesErrorMessage` y los mensajes de validación de las bases
      - **Problema:** los textos de error internos (en inglés y con faltas: `"isn't null"`,
        `"is diferent type"`, `"soported"`) se devuelven en la respuesta HTTP.
      - **Impacto:** doble problema. (1) **Revela detalles de implementación** —nombres de tipo,
        estructura de la clave primaria— a cualquiera que llame a la API con datos malformados.
        (2) Da una imagen de baja calidad y no es traducible.
      - **Propuesta:** separar el mensaje **técnico** (a logs y a `Details`) del mensaje **público**
        (genérico, sin nombres de tipo). Centralizar los textos públicos en un único fichero de
        recursos.

### Inyección de dependencias

- [ ] **20. `ResolveRepoFp<T>` construye un `ServiceProvider` en cada llamada**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `RegisterServices`
      - **Miembro:** `ResolveRepoFp<T>`
      - **Problema:** el método llama a `services.BuildServiceProvider()` **cada vez que se resuelve
        un repositorio**.
      - **Impacto:** cada llamada crea un **contenedor nuevo y paralelo** al de la aplicación. Los
        singletons se duplican, los `scoped` no comparten `DbContext` con el resto de la petición y
        **ninguno de esos contenedores se libera nunca**: fuga de memoria creciente. ASP.NET Core
        emite el aviso ASP0000 precisamente por esto.
      - **Propuesta:** no construir proveedores dentro del registro. Registrar los repositorios como
        `scoped` y resolverlos por constructor o mediante `IServiceProvider` inyectado.

- [ ] **21. Los repositorios liberan el `DbContext` que no les pertenece**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `OopRepos/EFRepoBase`, `Repos/EFRepoBaseFp`
      - **Miembro:** `Dispose()`
      - **Problema:** el `Dispose` del repositorio llama al `Dispose` del `DbContext`, pero ese
        `DbContext` lo ha creado y lo posee **el contenedor de DI**.
      - **Impacto:** `ObjectDisposedException` intermitente. Si en la misma petición hay otro
        repositorio o servicio que usa el mismo `DbContext` *scoped*, el primero que se libere
        **rompe a todos los demás**. El error aparece de forma aleatoria según el orden de liberación.
      - **Propuesta:** regla general: **quien no crea, no libera**. Quitar el `Dispose` del
        `DbContext` del repositorio. Si se quiere permitir la propiedad, añadir un flag `ownsContext`
        que solo sea `true` cuando el repositorio haya creado el contexto (por ejemplo vía
        `IDbContextFactory`.

- [ ] **22. `AddSingletonOOFPRepos` crea una dependencia cautiva**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `RegisterServices`
      - **Miembro:** `AddSingletonOOFPRepos<T, TContext>()`
      - **Problema:** registra como **singleton** repositorios que dependen de un `DbContext`, que es
        *scoped* por diseño.
      - **Impacto:** **dependencia cautiva** (*captive dependency*). El `DbContext` queda atrapado en
        el singleton y vive durante toda la aplicación: acumula entidades rastreadas (fuga de
        memoria), **no es *thread-safe*** (excepciones al usarlo desde peticiones concurrentes) y
        sirve datos obsoletos de su caché de primer nivel.
      - **Propuesta:** eliminar el método o, si se mantiene, que el repositorio reciba un
        `IDbContextFactory<TContext>` y cree un contexto por operación. Documentarlo con un aviso
        muy visible.

- [ ] **23. `_pkFields` no se puede resolver por inyección de dependencias**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** `_pkFields` (`Func<TEntity, object[]>`)
      - **Problema:** el selector de clave se recibe por constructor como un delegado, y **el
        contenedor de DI no sabe construir un `Func<TEntity, object[]>`**.
      - **Impacto:** el controlador **no se puede activar** sin escribir una fábrica manual, lo que
        convierte una clase base «lista para usar» en algo que exige código de infraestructura
        adicional en cada proyecto consumidor.
      - **Propuesta:** mover el selector a un método `protected abstract object[] GetKeyValues(TEntity)`
        que la clase derivada implemente, o a una interfaz registrable
        (`IKeySelector<TEntity>`).

- [ ] **24. `AddWebControllers` no hace nada**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `RegisterServices`
      - **Miembro:** `AddWebControllers`
      - **Problema:** el método existe, es público y su cuerpo está **vacío**.
      - **Impacto:** el consumidor lo llama convencido de haber configurado el proyecto y luego
        obtiene fallos de resolución que no relaciona con esta causa. Un método de registro vacío es
        peor que no tenerlo.
      - **Propuesta:** implementarlo (registrar el selector de claves, los conversores y las
        políticas) o eliminarlo.

### Culturas, formatos y URLs

- [ ] **25. `ConvertDateTime` depende de la cultura del hilo y mezcla formatos**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `ConvertDateTime`
      - **Problema:** usa `Thread.CurrentThread.CurrentCulture` y, además, combina patrones
        incompatibles: `M/d/yyyy` (estilo estadounidense) con `dd/MM/yyyy` (estilo europeo).
      - **Impacto:** **la misma petición se interpreta de forma distinta según el servidor.**
        `03/04/2024` puede ser el 3 de abril o el 4 de marzo dependiendo de la cultura del sistema
        operativo del contenedor. Es el clásico bug que solo aparece en producción y que corrompe
        datos de forma silenciosa.
      - **Propuesta:** usar **siempre `CultureInfo.InvariantCulture`** y aceptar exclusivamente
        formato ISO 8601 (`yyyy-MM-dd`, `O`) en los parámetros de ruta y *query string*. Documentar
        el formato en el `README.md` y en Swagger.

- [ ] **26. `ConverterTo` no recibe `IFormatProvider` y no cubre los tipos habituales**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `ConverterTo(string value, Type type)`
      - **Problema:** convierte sin `IFormatProvider` (misma dependencia de cultura del punto 25) y
        **no soporta** `Guid`, `DateOnly`, `TimeOnly` ni `enum`. Además devuelve `null` cuando la
        entrada es `null` sin distinguirlo de un fallo, y la rama de `DateTime?` no comprueba
        `IsNullOrEmpty`.
      - **Impacto:** los separadores decimales cambian según el servidor (`1.5` frente a `1,5`), y las
        entidades con clave `Guid` —lo más frecuente— **no funcionan** con las clases base de
        controlador.
      - **Propuesta:** firma `MlResult<object?> ConvertToType(string? value, Type target, IFormatProvider provider)`,
        con `InvariantCulture` por defecto y soporte explícito para `Guid`, `DateOnly`, `TimeOnly`,
        `enum` y `decimal`. Devolver un `MlResult` fallido, nunca `null`, cuando la conversión no sea
        posible.

- [ ] **27. `InternalGetUrl` compone URLs con `Path.Combine`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de composición de URL
      - **Miembro:** `InternalGetUrl`
      - **Problema:** usa `Path.Combine`, que es una función de **rutas de sistema de ficheros**, no
        de URLs.
      - **Impacto:** en Windows el separador es `\`, de modo que se generan URLs como
        `api\users\3`. Además `Path.Combine` **descarta todo lo anterior** si un segmento empieza por
        separador, con lo que un segmento mal formado puede apuntar a otra ruta del servidor.
      - **Propuesta:** componer con `Uri`/`UriBuilder` o con concatenación explícita normalizando las
        barras, y aplicar `Uri.EscapeDataString` a cada segmento variable.

- [ ] **28. Los valores de clave primaria no se codifican para URL**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions` → `GetPkValuesString` (duplicado en los dos proyectos)
      - **Miembro:** `GetPkValuesString`
      - **Problema:** concatena los valores usando la **cultura actual** y **sin codificarlos para
        URL**, separándolos por coma. La misma función está copiada literalmente en dos proyectos.
      - **Impacto:** cualquier clave que contenga `/`, `?`, `#`, `&`, un espacio o —muy importante—
        **una coma**, rompe el protocolo de clave compuesta: los segmentos se parten mal y se consulta
        una entidad distinta. Con datos que provienen del usuario, es además un vector de inyección
        en la ruta.
      - **Propuesta:** un único helper en el proyecto compartido, con `InvariantCulture`,
        `Uri.EscapeDataString` por segmento y un separador que no pueda aparecer en los datos (o
        codificado). Añadir la rama de `null` que hoy falta.

- [ ] **29. Las cabeceras de paginación se mutan en un `HttpClient` del *pool***
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** `SetHeaderInfo`, `SetHeaderPageNumber`, `SetHeaderPageSize`
      - **Problema:** escriben en `client.DefaultRequestHeaders`, y ese `HttpClient` viene de
        `IHttpClientFactory`, que **reutiliza** el manejador y comparte la instancia.
      - **Impacto:** las cabeceras **se filtran entre peticiones**. Una llamada puede heredar el
        `X-Page-Number` de otra, y si alguna vez se añade ahí un token de autorización, **un usuario
        podría recibir los datos de otro**. También provoca `InvalidOperationException` al mutar
        cabeceras desde varios hilos.
      - **Propuesta:** poner las cabeceras en el **`HttpRequestMessage`** de cada llamada, nunca en el
        cliente. Usar `SendAsync(request)` en lugar de los atajos `GetAsync`/`PostAsync`.

### Contratos y fiabilidad

- [ ] **30. La misma clave malformada devuelve 404 en `GET` y 500 en `PUT`/`DELETE`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el error de conversión de la clave se traduce a `404` en la ruta de lectura pero
        se propaga como error no controlado (`500`) en las de escritura.
      - **Impacto:** contrato incoherente. Un cliente que valide por código de estado no puede
        distinguir «petición mal formada» de «servidor caído», y las alarmas de 5xx se disparan por
        errores del cliente. En rigor, ninguno de los dos es correcto: debería ser `400 Bad Request`.
      - **Propuesta:** un único punto de entrada que convierta la clave y devuelva `400` para clave
        malformada y `404` solo cuando la clave es válida pero la entidad no existe.

- [ ] **31. `Location` apuntando al dominio del autor en todos los `POST`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`, `MoralesLarios.OOFP.WebApi`
      - **Archivo / clase:** las 4 clases base de controlador; `MlResultWebExtensionsPlus`
      - **Miembro:** `PostAsync`; `ToPostPdActionResult<T>`; `Created("NotUri", new object())`
      - **Problema:** al no recibir un `Uri`, se emite un `201 Created` cuya cabecera `Location` vale
        `"https://www.netalpunto.net"` (dominio del autor de la biblioteca), y en un caso literalmente
        la cadena `"NotUri"`.
      - **Impacto:** **la cabecera `Location` de la API pública de cualquier consumidor apunta a un
        dominio de terceros.** Los clientes que siguen `Location` para leer el recurso creado hacen
        una petición externa; en el mejor caso falla, en el peor filtra información en el `Referer`.
      - **Propuesta:** parámetro obligatorio con el nombre de la ruta y los valores de ruta
        (`CreatedAtAction`/`CreatedAtRoute`). Si no se dispone de URI, devolver `200 OK` en lugar de
        un `201` con `Location` inventado.

- [ ] **32. `type` de `ProblemDetails` con el dominio del autor por defecto**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`, `MoralesLarios.OOFP.WebApi`
      - **Archivo / clase:** `MlProblemsDetails`, `MlActionResults`, `MlResultWebExtensionsPlus`
      - **Miembro:** valor por defecto de `type`
      - **Problema:** el campo `type` se rellena con `"https://www.puntonetalpunto.net/"` y
        `"https://www.netalpunto.net"`, codificado a fuego.
      - **Impacto:** según RFC 7807, `type` es el identificador del tipo de problema y los clientes lo
        usan para decidir. Todas las APIs construidas con la biblioteca publican el **mismo** `type`
        para problemas distintos y **de un dominio ajeno**.
      - **Propuesta:** usar los URN estándar (`about:blank` para los códigos HTTP conocidos) y permitir
        configurar un prefijo propio mediante opciones. Nunca un dominio codificado.

- [ ] **33. `UpdateProblemDetailsAsync` consulta dos veces y descarta la entidad encontrada**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `UpdateProblemDetailsAsync`
      - **Problema:** llama a `TryFindAsync` para comprobar la existencia, **descarta el resultado** y
        vuelve a consultar más adelante.
      - **Impacto:** doble ida a la base de datos por actualización y, sobre todo, una **ventana de
        carrera**: entre las dos consultas otro proceso puede borrar o modificar la entidad. Como
        además se actualiza una instancia distinta de la rastreada, aparecen conflictos de
        seguimiento de EF Core.
      - **Propuesta:** una sola consulta, reutilizar la entidad rastreada y aplicar los cambios sobre
        ella. Considerar concurrencia optimista con `[Timestamp]`/`RowVersion`.

- [ ] **34. `Microsoft.AspNetCore.Mvc.Core` 2.1.0 en un proyecto `net8.0`**
      - **Proyecto:** `MoralesLarios.OOFP.WebApi`
      - **Archivo / clase:** `MoralesLarios.OOFP.WebApi.csproj`
      - **Miembro:** `PackageReference`
      - **Problema:** se referencia el paquete **2.1.0**, de 2018 y fuera de soporte, en un proyecto
        que compila para `net8.0` (donde MVC forma parte del *framework* compartido).
      - **Impacto:** arrastra un ensamblado antiguo con vulnerabilidades conocidas y provoca
        conflictos de versión difíciles de diagnosticar en los consumidores. En un proyecto web
        moderno, esta referencia **no debería existir**.
      - **Propuesta:** eliminar el `PackageReference` y usar
        `<FrameworkReference Include="Microsoft.AspNetCore.App" />`. Revisar el resto de los `.csproj`
        buscando referencias equivalentes.

- [ ] **35. `TryRemoveRangeAsync` usa la guarda sincrónica**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoDeleterFp`
      - **Miembro:** `TryRemoveRangeAsync`
      - **Problema:** dentro de un método asíncrono se usa `EnsureFp.NotNull` (sincrónico) en lugar de
        su equivalente asíncrono, rompiendo la cadena de composición.
      - **Impacto:** el resultado de la guarda no se compone bien con el resto del flujo asíncrono, de
        modo que en el caso de entrada nula el borrado **puede continuar** en lugar de cortarse.
      - **Propuesta:** usar la variante asíncrona y revisar todos los `*Async` del proyecto buscando
        guardas sincrónicas mal encadenadas.

- [ ] **36. `TotalCount` de la paginación sincrónica ignora el filtro**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderPaginationFp`
      - **Miembro:** `TryGetInternalData` (versión sincrónica)
      - **Problema:** el total de registros se calcula sobre la tabla completa, **sin aplicar el
        predicado de filtro** que sí se aplica a la página devuelta.
      - **Impacto:** el número de páginas que se comunica al cliente es incorrecto: la interfaz
        muestra páginas que vienen vacías. La versión asíncrona sí lo hace bien, así que el
        comportamiento **cambia según el método que se llame**.
      - **Propuesta:** contar sobre la misma `IQueryable` filtrada y compartir la construcción de la
        consulta entre las versiones sincrónica y asíncrona para que no puedan divergir.

- [ ] **37. `OrderBy` aplicado después de `Skip`/`Take`**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `OopRepos/EFRepoReaderPagination`
      - **Miembro:** construcción de la consulta paginada
      - **Problema:** la ordenación se encadena **después** de `Skip` y `Take`.
      - **Impacto:** SQL Server pagina **sin orden determinista** y ordena solo la página ya recortada.
        Resultado: al recorrer las páginas se repiten registros y se pierden otros, sin ningún error.
        Es un fallo de corrección de datos que se percibe como «la lista se comporta raro».
      - **Propuesta:** orden obligatorio **antes** de `Skip`/`Take`, con una ordenación por clave
        primaria como respaldo cuando el consumidor no indique ninguna.

---

## 🟡 Prioridad media — rendimiento, diseño y API pública

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)
      - **Miembro:** cada método de escritura
      - **Problema:** cada operación llama a `SaveChanges`/`SaveChangesAsync` por su cuenta.
      - **Impacto:** **es imposible componer varias escrituras en una transacción.** Si la segunda
        falla, la primera ya está confirmada y los datos quedan a medias. Además, un lote de N
        inserciones son N idas a la base de datos.
      - **Propuesta:** añadir un `IUnitOfWork` con `CommitAsync` y sobrecargas con un flag `autoSave`
        (por defecto `true` para no romper a nadie). Documentar el patrón en el `README.md`.

- [ ] **41. `AllAsync` no admite paginación**
      - **Proyecto:** `MoralesLarios.OOFP.WebServices`
      - **Archivo / clase:** `GenServiceFp`
      - **Miembro:** `AllAsync`
      - **Problema:** devuelve la tabla completa sin límite ni parámetros de paginación.
      - **Impacto:** un `GET` de colección sobre una tabla de producción trae todos los registros:
        tiempos de respuesta enormes, presión de memoria y un vector trivial de denegación de
        servicio. La paginación existe en el repositorio, pero el servicio no la expone.
      - **Propuesta:** añadir una sobrecarga con parámetros de página que devuelva el total. Marcar la
        versión sin límite `[Obsolete]` o imponerle un tope máximo configurable.

- [ ] **42. `HttpRequestMessage` y `HttpResponseMessage` nunca se liberan**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico y sus métodos internos
      - **Miembro:** todos los métodos de llamada
      - **Problema:** los mensajes de petición y respuesta se crean sin `using`.
      - **Impacto:** se retienen los flujos de contenido y los recursos asociados hasta que actúa el
        recolector. Bajo carga sostenida, consumo de memoria creciente y agotamiento de conexiones.
      - **Propuesta:** `using var request = …;` y `using var response = …;` en todos los métodos.

- [ ] **43. `Activator.CreateInstance<TEntity>()` en cada petición**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `Helpers/Extensions`
      - **Miembro:** `GetPkValues<TEntity>(string[] ids, Func<TEntity, object[]> pkFields)`
      - **Problema:** para averiguar los tipos de la clave primaria **instancia la entidad** con
        `Activator.CreateInstance<TEntity>()` en cada petición. Eso exige constructor público sin
        parámetros y, cuando una propiedad vale `null`, se asume silenciosamente que es `string`.
      - **Impacto:** coste de reflexión por petición, restricción artificial sobre las entidades del
        consumidor y conversiones equivocadas: una clave `Guid` se trata como texto.
      - **Propuesta:** obtener los tipos por **metadatos** (los `PropertyInfo` del selector o el modelo
        de EF Core), cachearlos en un `static` por tipo y no instanciar nada.

- [ ] **44. `JsonSerializer.Serialize` cuyo resultado se descarta**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clase interna de llamadas
      - **Miembro:** `InternalPostGetAsync`
      - **Problema:** se serializa el objeto y **el resultado no se asigna a nada**; el cuerpo se
        construye después por otra vía.
      - **Impacto:** trabajo de CPU y asignaciones inútiles en cada llamada, y la duda de si esa línea
        debía usarse (posible cambio de comportamiento pendiente).
      - **Propuesta:** eliminar la línea o usar su resultado, comprobando que la serialización efectiva
        aplique las mismas opciones que el resto del proyecto.

### Diseño de API y contratos

- [ ] **45. No se pueden inyectar `JsonSerializerOptions`**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** clases de cliente genérico
      - **Miembro:** serialización y deserialización
      - **Problema:** las opciones de JSON están fijadas internamente; no hay forma de suministrarlas.
      - **Impacto:** el consumidor **no puede** adaptar el cliente a un servicio remoto con
        convenciones distintas (`snake_case`, formatos de fecha propios, convertidores para tipos de
        dominio). Eso obliga a no usar la biblioteca precisamente en el caso más frecuente:
        integrarse con una API de un tercero.
      - **Propuesta:** aceptar `JsonSerializerOptions` por constructor o por opciones registradas en
        DI, con un valor por defecto sensato (`PropertyNameCaseInsensitive = true`).

- [ ] **46. El código de estado HTTP se pierde como dato estructurado**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `MlResponseWebExtensions` y clientes
      - **Miembro:** construcción del error a partir de la respuesta
      - **Problema:** el código de estado se incrusta en el **texto** del mensaje de error en lugar de
        guardarse en `Details` como valor.
      - **Impacto:** el consumidor no puede decidir por programa (reintentar en `503`, no reintentar
        en `400`, refrescar el token en `401`) sin **analizar cadenas**. Se pierde la ventaja principal
        del modelo de errores con detalles.
      - **Propuesta:** añadir el `HttpStatusCode` a `Details` con una clave constante y ofrecer un
        método de extensión `GetHttpStatusCode()` sobre `MlErrorsDetails`.

- [ ] **47. Métodos públicos ausentes de las interfaces**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`
      - **Archivo / clase:** `IGenClientFp<>`, `IHttpClientFactoryManager`
      - **Miembro:** `GetAsync<T>(CallRequestParamsInfo)`, `GetPaginationAsync<T>(…)`,
        `GetHttpClientFactoryKey()`
      - **Problema:** son públicos en la implementación pero **no están declarados en la interfaz**.
      - **Impacto:** quien programe contra la abstracción (lo correcto, y lo necesario para poder
        hacer *mocks* en tests) **no tiene acceso a parte de la funcionalidad**, y se ve obligado a
        acoplarse al tipo concreto.
      - **Propuesta:** subir esos miembros a la interfaz. Si alguno no debe formar parte del contrato,
        hacerlo `internal` o `protected`.

- [ ] **48. Asimetrías de nombres y parámetros entre familias equivalentes**
      - **Proyecto:** `MoralesLarios.OOFP.HttpClients`, `MoralesLarios.OOFP.WebApi`,
        `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** `GenClientFp` y su variante *duplex*; `MlResultWebExtensionsPlus`; clases
        base de controlador
      - **Miembro:** `PutByIdAsync` frente a `GetByIdAsync` (uno envía `idStr` suelto y el otro
        `id-str/{idStr}`); `DeleteByIdAsync` (la variante *duplex* usa `<TResponse>` y la simple
        `<TDto>`); los `PutAsync` simples usan el argumento nombrado `dto:` y los *duplex*
        `dtoRequest:`
      - **Problema:** métodos que deberían ser simétricos difieren en la ruta, en el genérico o en el
        nombre del parámetro.
      - **Impacto:** el cliente y el servidor **no se entienden** en las rutas afectadas (`PUT` por
        identificador falla), y el consumidor no puede razonar por analogía: cada método hay que
        comprobarlo en el código fuente.
      - **Propuesta:** un único helper que construya la ruta por identificador y usarlo desde todos
        los métodos, en cliente y servidor. Tests de contrato que recorran cliente → controlador.

- [ ] **49. Rutas incoherentes entre verbos: `id-str/{id}` frente a `{id}`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** atributos de ruta de `GetAsync`, `PutAsync`, `DeleteAsync`
      - **Problema:** el `GET` expone `id-str/{id}` mientras que `PUT` y `DELETE` usan `{id}`.
      - **Impacto:** la API resultante no es REST reconocible: el mismo recurso tiene dos direcciones
        según el verbo. Cualquier cliente generado a partir de la especificación OpenAPI queda
        confuso, y quien escriba el cliente a mano se equivoca.
      - **Propuesta:** una sola forma de ruta por recurso (`{id}` para clave simple y un patrón
        explícito documentado para clave compuesta) aplicada a todos los verbos.

- [ ] **50. `PUT` y `DELETE` con sobrecargas que reciben cuerpo**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** clases base de controlador
      - **Miembro:** sobrecargas de `PutAsync` y `DeleteAsync` con `[FromBody]`
      - **Problema:** se ofrecen variantes que esperan el cuerpo de la petición en `DELETE`.
      - **Impacto:** muchos *proxies*, CDN y balanceadores **descartan el cuerpo de un `DELETE`**, y
        varias bibliotecas cliente no permiten enviarlo. El endpoint funciona en local y falla al
        desplegar detrás de una pasarela.
      - **Propuesta:** en `DELETE`, la clave va en la ruta o en la *query string*. Si de verdad hace
        falta enviar datos, usar `POST` sobre un sub-recurso de acción.

- [ ] **51. Las clases base de controlador no son `abstract` ni declaran `[Route]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** declaración de tipo y atributos
      - **Problema:** llevan `[ApiController]` pero **no** `[Route]`, y **no son `abstract`**.
      - **Impacto:** MVC puede descubrir la propia clase base como controlador activable, exponiendo
        endpoints genéricos no previstos; y sin `[Route]` el enrutado depende de convenciones que la
        derivada debe recordar declarar.
      - **Propuesta:** marcar las bases `abstract` y documentar que la derivada debe aportar
        `[Route("[controller]")]`, o incluirlo en la base.

- [ ] **52. Las clases base no declaran `[ProducesResponseType]`**
      - **Proyecto:** `MoralesLarios.OOFP.WebControllers`
      - **Archivo / clase:** las 4 clases base
      - **Miembro:** todos los métodos de acción
      - **Problema:** ninguna acción documenta los códigos de respuesta posibles.
      - **Impacto:** la especificación OpenAPI generada solo declara `200`, de modo que los clientes
        generados automáticamente **no contemplan** `201`, `204`, `400`, `404` ni el esquema de
        `ProblemDetails`. Toda la riqueza del modelo de errores se pierde en el contrato publicado.
      - **Propuesta:** añadir los `[ProducesResponseType]` correspondientes en cada acción, incluido
        `[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]`.

---

## 🟢 Prioridad baja — limpieza, coherencia y documentación

Aquí no hay datos incorrectos ni riesgos de seguridad, pero sí decisiones que **se pagan cuando el
volumen crece** o que hacen la API difícil de usar correctamente.

### Rendimiento y acceso a datos

- [ ] **38. `TryLast*` materializa toda la consulta en memoria**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoReaderFp`
      - **Miembro:** `TryLast`, `TryLastAsync` y variantes
      - **Problema:** para obtener el último elemento se trae **la colección filtrada completa** al
        cliente y se selecciona el último en memoria.
      - **Impacto:** con una tabla grande esto son millones de filas por la red y un pico de memoria
        que puede tumbar el proceso, cuando la base de datos podía resolverlo con un `TOP 1`.
      - **Propuesta:** ordenar de forma descendente en la consulta y usar
        `FirstOrDefaultAsync`/`LastOrDefaultAsync` sobre `IQueryable`. Exigir una ordenación explícita:
        «el último» no tiene sentido sin orden definido.

- [ ] **39. `TryUpdate(item, pk)` provoca conflictos de seguimiento de EF Core**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** `Repos/EFRepoUpdaterFp`
      - **Miembro:** `TryUpdate(TEntity item, object[] pk)`
      - **Problema:** hace `Find` (que **rastrea** la entidad encontrada) y después llama a `Update`
        con la instancia **recibida por parámetro**, que es otro objeto con la misma clave.
      - **Impacto:** `InvalidOperationException`: «another instance with the same key value is already
        being tracked». Falla en el escenario más habitual: actualizar a partir de un DTO.
      - **Propuesta:** `Find` + `SetValues(item)` sobre la entidad rastreada, o `AsNoTracking` para
        comprobar existencia y luego `Attach` + `Update`. Una sola estrategia en todo el proyecto.

- [ ] **40. No hay unidad de trabajo: `SaveChanges` por operación**
      - **Proyecto:** `MoralesLarios.OOFP.EFCore`
      - **Archivo / clase:** todos los repositorios de escritura (`EFRepoWriterFp`, `EFRepoUpdaterFp`,
        `EFRepoDeleterFp` y sus equivalentes OOP)