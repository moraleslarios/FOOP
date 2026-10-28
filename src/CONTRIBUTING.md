# Guía de contribución — MoralesLarios.OOFP

Gracias por contribuir. Este documento recoge las **convenciones de nomenclatura y estilo** que la
biblioteca fija a partir de la versión **2.0.0**, además del flujo de trabajo de ramas y commits.

Todas las reglas de nomenclatura se aplicaron de forma masiva en la 2.0.0; el detalle completo de
qué se renombró está en [`Cambios Realizados/Renombrados-v2.0.0.md`](./Cambios%20Realizados/Renombrados-v2.0.0.md).
El objetivo de esta guía es que **no se vuelvan a introducir** los patrones que allí se eliminaron.

---

## Índice

1. [Idioma](#idioma)
2. [Nomenclatura](#nomenclatura)
   - [Sufijos con significado fijo](#sufijos-con-significado-fijo)
   - [Sufijos y prefijos prohibidos](#sufijos-y-prefijos-prohibidos)
   - [Parámetros de tipo genérico](#parámetros-de-tipo-genérico)
   - [Booleanos](#booleanos)
   - [Factorías estáticas](#factorías-estáticas)
   - [Constantes, campos y colecciones](#constantes-campos-y-colecciones)
   - [Nombres de proyecto y de carpeta](#nombres-de-proyecto-y-de-carpeta)
3. [Diseño de API pública](#diseño-de-api-pública)
4. [Cambios que rompen compatibilidad](#cambios-que-rompen-compatibilidad)
5. [Ramas, commits y versionado](#ramas-commits-y-versionado)
6. [Compilar y ejecutar los tests](#compilar-y-ejecutar-los-tests)
7. [Documentación](#documentación)

---

## Idioma

| Dónde | Idioma |
|---|---|
| Identificadores (tipos, miembros, parámetros, variables) | **Inglés** |
| Mensajes de error y cadenas literales de la biblioteca | **Inglés** |
| Comentarios XML (`///`) | **Inglés** |
| Ficheros `.md` de documentación | **Español** |
| Comentarios internos `//` | Indistinto, preferible inglés |

> No se admiten identificadores mixtos (`ContieneCombinacion`, `MlResultBucles`, `Catas`).
> Todos los que existían se eliminaron en la 2.0.0.

---

## Nomenclatura

### Sufijos con significado fijo

| Sufijo | Significado exacto | Correcto | A evitar |
|---|---|---|---|
| `…Async` | Devuelve `Task` / `ValueTask`. **Siempre** que lo devuelva, y solo entonces. | `TryFindAsync` | `AllAsync` que en realidad es sincrónico |
| `Try…` | No lanza: el fallo viaja dentro del `MlResult`. | `TryMap`, `TryBind` | `Try…` que lanza en una comprobación de `null` |
| `…Fp` | Variante funcional cuando **coexiste** con una imperativa que lanza. | `EnsureFp` | `…Fp` donde no hay variante imperativa |
| `I…` | Interfaz. | `IRestClientFp` | — |
| `…Base` | Clase base `abstract`. Si no es `abstract`, el sufijo sobra. | `MlCrudControllerBase` | una base instanciable |
| `…Extensions` | Clase `static` que contiene **solo** métodos de extensión. | `MlResultActionResultExtensions` | `…Helpers` con extensiones dentro |
| `…Options` | Datos de configuración de una operación, sin lógica. | `ApiCallOptions` | `CallRequestParamsInfo` |
| `…Request` / `…Result` | Entrada / salida de una operación. | `PageRequest`, `PagedResult<T>` | `…Info` para lo mismo |
| `…Repository` | Acceso a persistencia. | `IEfReadRepository<T>` | `…RepoFp`, `…Repo` |

### Sufijos y prefijos prohibidos

| Prohibido | Motivo | Qué usar en su lugar |
|---|---|---|
| `…Info` | No aporta información: **todo** es información. | El sustantivo real (`…Options`, `…Result`, `…Descriptor`) |
| `…Plus` | Solo significa «el nombre bueno estaba ocupado». | Resolver el conflicto de nombres |
| `…Helper` / `…Utils` / `…Manager` | Cajón de sastre: atrae código sin dueño. | Nombrar la responsabilidad concreta (`IHttpApiInvoker`) |
| `Gen…` | Abreviatura ambigua; la genericidad ya la marcan los `<T>`. | El dominio (`Crud…`, `Rest…`) |
| `Simple…` / `Complex…` | Juicios de valor relativos y sin criterio. | El criterio real (`CompositeKey…`) |
| `…Duplex…` | Significa otra cosa en comunicaciones. | La aridad genérica `<TRequest, TResponse>` |
| `Oop…` | Toda la biblioteca es orientada a objetos. | El comportamiento (`Throwing…`) |
| `…Internals` como nombre de proyecto | No es un ámbito, es un modificador (`internal`). | `Shared` |
| `SCREAMING_SNAKE_CASE` | Estilo C/C++, no .NET. | `PascalCase` |

### Parámetros de tipo genérico

| Rol | Nombre | Nunca |
|---|---|---|
| Entidad de dominio / persistencia | `TEntity` | `T`, `TE` |
| DTO de ida y vuelta | `TDto` | `T` |
| DTO de entrada | `TRequest` | `T`, `TIn` |
| DTO de salida | `TResponse` | `K`, `TOut`, `TEnumrableResponse` |
| Clave (simple o compuesta) | `TKey` | `TPk`, `K` |
| Valor de retorno de una proyección | `TResult` | `TReturn`, `TRet` |
| Elemento de una colección | `TItem` | `TEnumerable` |

> **`TResult`, no `TReturn`.** El plan de nomenclatura exigía elegir **uno de los dos** y no
> mantener ambos. Se escoge `TResult` porque es la convención del propio BCL
> (`Func<T, TResult>`, `Task<TResult>`, `Select<TSource, TResult>`): quien llega de LINQ o de
> `System.Linq.Expressions` lo reconoce sin esfuerzo. La unificación se aplicó en la 2.0.0 sobre
> todo el núcleo (2 942 ocurrencias en 9 ficheros) y **no rompe compatibilidad**: el nombre de un
> parámetro de tipo no forma parte de la firma, ni binaria ni de código fuente.

### Booleanos

- `Is…` para **estado**: `IsSuccess`, `IsFailure`, `IsValid`.
- `Has…` para **posesión**: `HasExceptionDetails`.
- `Can…` para **capacidad**: `CanRetry`.
- Nunca un sustantivo o adjetivo suelto (`Valid`, `Error`).

### Factorías estáticas

Convención única para todo el repositorio:

| Verbo | Devuelve | Comportamiento ante entrada inválida |
|---|---|---|
| `From…` | `MlResult<T>` | **Segura**: devuelve un fallo |
| `CreateFrom…` | `T` | **Lanza** excepción |

```csharp
MlResult<Mail> r  = Mail.FromString("a@b.com");     // segura
Mail            m = Mail.CreateFromString("a@b.com"); // lanza si no es válido
```

- El sufijo indica el **tipo de entrada**: `FromString`, `FromInt`, `FromDecimal`, `FromRegex`,
  `FromLength`, `FromIntLength`…
- **No** se admiten `By…` ni `Build…` para lo mismo.
- Si el constructor puede fallar, debe ser `protected` y obligar a pasar por la factoría.

### Constantes, campos y colecciones

- **Constantes:** `PascalCase` (`ExceptionDetailKey`), nunca `EX_DESC_KEY`.
- **Campos privados:** `_camelCase`.
- **Prohibido** `public static` mutable: un límite de validación es `private const`.
- **Colecciones en plural** (`Errors`, `Details`). Un plural en un tipo que **no** es colección es
  una pista falsa: `MlErrorDetails`, no `MlErrorsDetails`; `MlProblemDetails`, no `MlProblemsDetails`.

### Nombres de proyecto y de carpeta

- Prefijo obligatorio `MoralesLarios.OOFP.`.
- El segmento siguiente nombra la **tecnología o la capa**, no una abreviatura:
  `AspNetCore.Results`, `EntityFrameworkCore`, `Http`, `FileSystem`, `Logging`, `Configuration`.
- Proyectos de test: `<Proyecto>.Tests.Unit`, `<Proyecto>.Tests.Integration`.
- Proyectos de ejemplo: `<Proyecto>.Sample`.
- La **carpeta** del proyecto se llama igual que el `.csproj`.

---

## Diseño de API pública

- Las clases base de controlador y de servicio son `abstract`: ASP.NET no debe descubrirlas como
  controladores reales.
- Las abstracciones se declaran como **interfaz**, no como clase abstracta, salvo que haya estado
  compartido: el consumidor solo tiene una clase base disponible (`IMlSelfValidatable<T>`).
- Los diccionarios y colecciones expuestos son de **solo lectura** (`IReadOnlyDictionary<,>`,
  `IReadOnlyList<>`).
- **Nunca** `.Result` ni `.GetAwaiter().GetResult()` en la biblioteca: la firma se propaga como
  `async` hasta el borde.
- Las operaciones que pueden fallar devuelven `MlResult` / `MlResult<T>`; las excepciones se
  reservan para errores de programación.

---

## Cambios que rompen compatibilidad

Cualquier renombrado o cambio de firma de un miembro **público** requiere un puente:

```csharp
// Fichero: MiTipo.Obsolete.cs
[global::System.Obsolete("Renamed to NuevoNombre. Will be removed in 3.0.0.", false),
 global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public static MlResult<T> NombreAntiguo(...) => NuevoNombre(...);
```

Reglas:

1. El puente vive en un fichero `*.Obsolete.cs` separado, junto al tipo original.
2. El mensaje está **en inglés** y nombra el sustituto y la versión de retirada.
3. `EditorBrowsableState.Never` para que no aparezca en IntelliSense.
4. Se registra el cambio como una fila nueva en
   [`Cambios Realizados/_registro.csv`](./Cambios%20Realizados/_registro.csv).
5. Los puentes se eliminan en la siguiente versión **mayor**.

> Cuando el cambio afecta a un **tipo** (no a un miembro), no siempre es posible un puente.
> En ese caso se documenta como ruptura y se refleja en la tabla de contratos del entregable.

### El registro de cambios

`Cambios Realizados/_registro.csv` es la fuente de verdad del entregable de renombrados.
Formato (separador `;`):

```
Lote;Nivel;Tipo;Proyecto;Contenedor;Antes;Despues;Puentes;Referencias;Nota
```

> ⚠️ **Ningún campo puede contener `;`**: el separador es puro, sin escapado de comillas.
> Usa `,` o `.` dentro del texto.

El documento final se **regenera**, nunca se edita a mano:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\generar-cambios-realizados.ps1
```

---

## Ramas, commits y versionado

- Rama por tema: `feature/…`, `fix/…`, `release/…`.
- **Conventional Commits**, con el *scope* igual al proyecto afectado y el cuerpo en español:

```
refactor(core)!: renombrar MlErrorsDetails a MlErrorDetails

El plural en un tipo que no es una colección era una pista falsa.
Se añaden puentes [Obsolete] en MlErrorDetails.Obsolete.cs.
```

- El `!` marca el cambio que rompe compatibilidad.
- Commits **atómicos**: un nivel o un proyecto por commit.
- **SemVer**: los renombrados con puente suben la **mayor** (el consumidor recibe `CS0618`);
  la retirada de los puentes también.

---

## Compilar y ejecutar los tests

```powershell
dotnet build MoralesLarios.OOFP.sln
dotnet test  MoralesLarios.OOFP.sln
```

Criterio de aceptación de cualquier PR:

| Comprobación | Exigencia |
|---|---|
| Compilación | **0 errores** |
| Tests unitarios | **todos en verde** |
| Tests de integración de `Http` | los 6 que requieren un servidor HTTP vivo pueden fallar en local |
| Enlaces y anclas de los `.md` | **0 rotos** |

Los scripts auxiliares de verificación están en `scripts/`:

| Script | Qué hace |
|---|---|
| `build.ps1` | Compila y resume errores y advertencias |
| `test.ps1` | Ejecuta la suite y resume el resultado por proyecto |
| `check-md-links2.ps1` | Verifica los enlaces relativos entre `.md` |
| `check-md-anchors4.ps1` | Verifica las anclas internas de los `.md` |
| `check-md-encoding.ps1` | Detecta problemas de codificación en los `.md` |
| `generar-cambios-realizados.ps1` | Regenera el entregable de renombrados desde el CSV |

> Los `.ps1` del repositorio se escriben en **ASCII puro**: PowerShell 5.1 interpreta un fichero
> UTF-8 sin BOM como Windows-1252 y corrompe los acentos. Los caracteres no ASCII se construyen
> con `[string][char]0xNNNN`.

---

## Documentación

- Cada proyecto tiene su `README.md`.
- La documentación extensa vive en `docs/`, en español.
- Si un renombrado afecta a un **nombre de fichero** de `docs/`, se renombra con `git mv` y se
  actualizan todos los enlaces que apuntaban a él.
- Las carpetas `Temas Pendientes/` y los ficheros `MEJORAS_PENDIENTES.md` y
  `CONSEJOS_NOMENCLATURA.md` son **documentos históricos**: reflejan el estado previo a un
  cambio y **no** se actualizan con la nomenclatura nueva.
