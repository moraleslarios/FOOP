# MoralesLarios.FOOP — Código fuente de la solución

Esta carpeta contiene **todos los proyectos** de la solución `MoralesLarios.OOFP.sln`: el núcleo
funcional, las librerías satélite que lo extienden y los proyectos de pruebas que lo verifican.

> 📘 Si vienes de fuera, empieza por el [**README general del repositorio**](../README.md), que
> explica la visión, la arquitectura y las rutas de lectura recomendadas.

---

## Índice

1. [Qué es esta solución](#1-qué-es-esta-solución)
2. [Por dónde empezar](#2-por-dónde-empezar)
3. [Proyectos de producción](#3-proyectos-de-producción)
4. [Proyectos de pruebas](#4-proyectos-de-pruebas)
5. [Novedades destacadas](#5-novedades-destacadas)
6. [Cómo compilar y ejecutar las pruebas](#6-cómo-compilar-y-ejecutar-las-pruebas)
7. [Temas pendientes: mejoras, nomenclatura y profesionalización](#-temas-pendientes-mejoras-nomenclatura-y-profesionalización)

---

## 1. Qué es esta solución

**MoralesLarios.FOOP** es un ecosistema .NET que aplica *Railway-Oriented Programming* de forma
sistemática: en lugar de propagar excepciones como mecanismo primario de control de flujo, cada
operación devuelve un **`MlResult<T>`** que representa explícitamente el éxito o el fallo, y el
error viaja como **dato estructurado** (`MlErrorsDetails`) hasta el borde de la aplicación.

Sobre esa base común se construyen capas independientes: value objects validados, validación
declarativa, IO seguro, repositorios EF Core, servicios de aplicación, traducción a
`IActionResult`, controladores REST genéricos, caché y clientes HTTP tipados. Todas hablan el
mismo idioma, con las mismas convenciones de nombres (`Bind*`, `Map*`, `Match*`, `ExecSelf*`,
`Try*`, `*Async`), de modo que aprender una familia sirve para todas.

---

## 2. Por dónde empezar

| Si quieres… | Ve a… |
|---|---|
| Entender la filosofía y el mapa completo | [`MoralesLarios.FOOP/__Doc/1_Intro.md`](./MoralesLarios.FOOP/__Doc/1_Intro.md) |
| Aprender el núcleo con ejemplos y patrones | [`MoralesLarios.FOOP/README.md`](./MoralesLarios.FOOP/README.md) |
| Validar precondiciones al entrar en un método | [`__Doc/EnsureFp/EnsureFp.md`](./MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) |
| Encadenar operaciones que devuelven `MlResult` | [`__Doc/Bind/3_Bind.md`](./MoralesLarios.FOOP/__Doc/Bind/3_Bind.md) |
| Transformar el valor de un resultado válido | [`__Doc/Map/1_Map.md`](./MoralesLarios.FOOP/__Doc/Map/1_Map.md) |
| Salir del carril y devolver un valor concreto | [`__Doc/Match/1_Match.md`](./MoralesLarios.FOOP/__Doc/Match/1_Match.md) |
| Ver la referencia archivo a archivo del núcleo | [`__Doc/Types/README.md`](./MoralesLarios.FOOP/__Doc/Types/README.md) |
| Saber qué hay que arreglar y en qué orden | [`Temas Pendientes/README.md`](./Temas%20Pendientes/README.md) |

---

## 3. Proyectos de producción

Todos ellos tienen su propio `README.md` con documentación detallada.

### Núcleo

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.FOOP**](./MoralesLarios.FOOP/README.md) *(assembly `MoralesLarios.OOFP`)* | Corazón del ecosistema: `MlResult<T>`, `MlErrorsDetails`, `EnsureFp` y las familias de extensiones `Bind`, `Map`, `Match`, `ExecSelf`, `Several`, `Transformations` y `Bucles`. **Es el único proyecto que hay que conocer de verdad.** |
| [**MoralesLarios.OOFP.Internals**](./MoralesLarios.OOFP.Internals/README.md) | Tipos compartidos de bajo nivel y soporte de paginación. |
| [**MoralesLarios.OOFP.Shared**](./MoralesLarios.OOFP.Shared/README.md) | Constantes compartidas entre proyectos, **sin ninguna dependencia**. Evita que dos capas se pongan de acuerdo mediante literales duplicados. |

### Dominio, semántica y validación

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.ValueObjects**](./MoralesLarios.OOFP.ValueObjects/README.md) | Value objects tipados y autovalidados que hacen imposibles los estados inválidos. |
| [**MoralesLarios.OOFP.ValueObjects.IO**](./MoralesLarios.OOFP.ValueObjects.IO/README.md) | Value objects específicos de rutas, ficheros y directorios. |
| [**MoralesLarios.OOFP.Validation**](./MoralesLarios.OOFP.Validation/README.md) | Base común de la validación funcional: contratos y extensiones sobre `MlResult<T>`. |
| [**MoralesLarios.OOFP.Validation.Dataannotations**](./MoralesLarios.OOFP.Validation.Dataannotations/README.md) | Implementación con `System.ComponentModel.DataAnnotations`. |
| [**MoralesLarios.OOFP.Validation.FluentValidations**](./MoralesLarios.OOFP.Validation.FluentValidations/README.md) | Implementación con FluentValidation, con paridad de características respecto a la anterior. |

### Infraestructura y utilidades

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.IO**](./MoralesLarios.OOFP.IO/README.md) | Operaciones de ficheros y directorios que devuelven `MlResult` en lugar de lanzar excepciones. |
| [**MoralesLarios.OOFP.Utilities**](./MoralesLarios.OOFP.Utilities/README.md) | Lectura segura de configuración (`IConfiguration`) con resultados explícitos. |
| [**MoralesLarios.OOFP.Extensions.Loggers**](./MoralesLarios.OOFP.Extensions.Loggers/README.md) | Registro de trazas encadenable sobre `MlResult<T>` sin romper el carril. |

### Persistencia

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.EFCore**](./MoralesLarios.OOFP.EFCore/README.md) | Repositorios genéricos sobre EF Core en dos sabores: funcional (`MlResult`) y orientado a objetos. |
| [**MoralesLarios.OOFP.EFCore.WebApi**](./MoralesLarios.OOFP.EFCore.WebApi/README.md) | Piezas de integración entre los repositorios EF Core y la capa Web API. |

### Servicios y exposición web

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.WebServices**](./MoralesLarios.OOFP.WebServices/README.md) | Servicios de aplicación funcionales: orquestan repositorios y validación devolviendo `MlResult`. |
| [**MoralesLarios.OOFP.WebApi**](./MoralesLarios.OOFP.WebApi/README.md) | Puente entre `MlResult<T>` e `IActionResult`, con traducción a `ProblemDetails` (RFC 7807) y códigos HTTP correctos. |
| [**MoralesLarios.OOFP.WebControllers**](./MoralesLarios.OOFP.WebControllers/README.md) | Controladores REST genéricos listos para heredar (CRUD completo). |
| [**MoralesLarios.OOFP.WebControllers.Cache**](./MoralesLarios.OOFP.WebControllers.Cache/README.md) | Los mismos controladores con caché configurable por controlador. |

### Consumo HTTP

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.HttpClients**](./MoralesLarios.OOFP.HttpClients/README.md) | Clientes HTTP tipados que convierten respuestas y errores de red en `MlResult<T>`. |

---

## 4. Proyectos de pruebas

La solución incluye **14 proyectos de pruebas** que funcionan como verificación viva del
ecosistema y, en la práctica, como documentación ejecutable de cada comportamiento:

| Proyecto | Cubre |
|---|---|
| `MoralesLarios.OOFP.Unit.Tests` | El núcleo: `MlResult`, `EnsureFp` y todas las familias de extensiones |
| `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | Value objects |
| `MoralesLarios.OOFP.ValueObjects.IO.Test.Unit` | Value objects de IO |
| `MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit` | Value objects de IO (segunda batería) |
| `MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit` | Validación con DataAnnotations |
| `MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit` | Validación con FluentValidation |
| `MoralesLarios.OOFP.Utilities.Tests.Unit` | Lectura de configuración |
| `MoralesLarios.OOFP.WebServices.Tests.Unit` | Servicios de aplicación |
| `MoralesLarios.OOFP.WebApi.Tests.Unit` | Traducción a `IActionResult` y `ProblemDetails` |
| `MoralesLarios.OOFP.HttpClients.Tests.Unit` | Clientes HTTP (unitarias) |
| `MoralesLarios.OOFP.HttpClients.Tests.Integration` | Clientes HTTP (integración) |
| `MoralesLarios.OOFP.EFCore.Infrastructure.Tests` | Repositorios EF Core sobre base en memoria/archivo |
| `MoralesLarios.OOFP.EFCore.Integration.Tests` | Repositorios EF Core contra base real |
| `MoralesLarios.OOFP.Extensions.Loggers.Console.Tests` | Logging funcional |

> Estos proyectos de pruebas son, hoy, los únicos
> proyectos que **todavía no tienen `README.md`** propio (recogido como punto 89 del inventario de mejoras).

---

## 5. Novedades destacadas

### 🛡️ `EnsureFp`: de 4 guardas a una biblioteca de precondiciones

`EnsureFp` era un puñado de comprobaciones básicas (`NotNull`, `NotEmpty`,
`NotNullEmptyOrWhitespace`, `That`). Ahora es una **`static partial class` repartida en ocho
ficheros** que ofrece **más de 90 reglas** listas para usar, sin dejar de ser retrocompatible.

Las tres claves del diseño:

1. **Tres variantes por regla.** Cada comprobación existe con mensaje `string`, con
   `MlErrorsDetails` completo, y con sufijo **`…Arg`**, que usa
   `[CallerArgumentExpression]` para deducir el nombre del parámetro y **construir el mensaje
   por ti**:

   ```csharp
   // Sin escribir un solo mensaje de error:
   EnsureFp.NotNullEmptyOrWhitespaceArg(nombre)      // "'nombre' no puede ser nulo, vacío…"
   EnsureFp.GreaterOrEqualArg(cantidad, 1)           // "'cantidad' debe ser mayor o igual que 1…"
   EnsureFp.InRangeArg(edad, 18, 120)
   ```

2. **Agregación de errores.** `All` ejecuta *todas* las reglas y fusiona sus detalles, en lugar
   de detenerse en la primera. También hay `AllOrFirst` (fail-fast), `AllResults` y `Any`:

   ```csharp
   var result = EnsureFp.All(dto,
       d => EnsureFp.NotNullEmptyOrWhitespaceArg(d.Email),
       d => EnsureFp.IsValidEmailArg(d.Email),
       d => EnsureFp.InRangeArg(d.Edad, 18, 120));
   // Si fallan las tres, el resultado informa de las tres.
   ```

3. **Captura de excepciones y asincronía real.** `TryThat` convierte en fallo funcional
   cualquier excepción del predicado, y `EnsureFp.Async` añade sobrecargas que aceptan
   **`Task<T>` como fuente**, **predicados `Func<T, Task<bool>>`** y `CancellationToken`.

Las ocho familias:

| Familia | Ejemplos de reglas |
|---|---|
| **Núcleo** | `That` con predicados y mensajes perezosos, `TryThat`, guardas `…Arg` |
| **Agregación** | `All`, `AllOrFirst`, `AllResults`, `Any` (+ variantes async) |
| **Cadenas** | `MaxLength`, `MinLength`, `LengthBetween`, `Matches`, `StartsWith`, `ContainsText`, `IsOneOf` |
| **Números** | `GreaterThan`, `LessOrEqual`, `InRange`, `OutOfRange`, `Positive`, `NotNegative`, `NotZero` |
| **Colecciones** | `CountExactly`, `CountBetween`, `AllMatch`, `NoneMatch`, `NoDuplicates`, `NoNullItems`, `ContainsItem` |
| **Tipos concretos** | `NotEmptyGuid`, `IsDefined` (enums), `InFuture`/`InPast`, `NotDefault`, `IsValidUri`, `IsValidEmail`, `FileExists`, `DirectoryExists` |
| **`Nullable<T>`** | `NotNullValue`, `NotNullValueThat` — desenvuelven `T?` a `MlResult<T>` |
| **Asíncronas** | `ThatAsync`, `TryThatAsync`, `NotNullAsync`, `NotEmptyAsync`, `NotNullValueAsync` |

📚 Documentación completa: [`__Doc/EnsureFp/EnsureFp.md`](./MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md)
(índice) y sus nueve páginas de familia:
[Núcleo](./MoralesLarios.FOOP/__Doc/EnsureFp/1_EnsureFpCore.md) ·
[Agregación](./MoralesLarios.FOOP/__Doc/EnsureFp/2_EnsureFpAggregation.md) ·
[Cadenas](./MoralesLarios.FOOP/__Doc/EnsureFp/3_EnsureFpStrings.md) ·
[Números](./MoralesLarios.FOOP/__Doc/EnsureFp/4_EnsureFpNumbers.md) ·
[Colecciones](./MoralesLarios.FOOP/__Doc/EnsureFp/5_EnsureFpCollections.md) ·
[Tipos](./MoralesLarios.FOOP/__Doc/EnsureFp/6_EnsureFpTypes.md) ·
[Nullables](./MoralesLarios.FOOP/__Doc/EnsureFp/7_EnsureFpNullables.md) ·
[Asíncronas](./MoralesLarios.FOOP/__Doc/EnsureFp/8_EnsureFpAsync.md) ·
[Mensajes](./MoralesLarios.FOOP/__Doc/EnsureFp/9_EnsureFpMessages.md).

### 📦 Nuevo proyecto `MoralesLarios.OOFP.Shared`

Proyecto sin dependencias cuya misión es alojar las **constantes que comparten varios
proyectos**. Nace de un bug real: la clave del diccionario `MlErrorsDetails.Details` se escribía
como literal en dos capas distintas, y el desacuerdo entre ambas **degradaba silenciosamente un
404 en un 500**. Con `WebErrorDetailsKeys.ProblemsDetails` hay una sola fuente de verdad.

Detalles y reglas de uso en su [README](./MoralesLarios.OOFP.Shared/README.md).

### 🐛 Correcciones funcionales

- **`ToSimpleRepoPostActionResult`** — se auditó su comportamiento y se añadieron pruebas de
  regresión que fijan el contrato de los códigos HTTP devueltos.
- **`BuildNotFoundPkError`** — ya usa la clave compartida, de modo que el `ProblemDetails`
  construido en la capa de servicios llega íntegro a la capa web y el **404 se preserva**.
- **`ExistsFile` / `ExistDirectory`** — corregido el anidamiento de condiciones y el mensaje de
  error, con pruebas que lo demuestran.
- **Validación con FluentValidation** — igualada en características a la variante de
  DataAnnotations, con su batería de pruebas equivalente.

### 📚 Documentación

El núcleo cuenta con **57 documentos** en `MoralesLarios.FOOP/__Doc/`, organizados por familia
funcional. El [índice completo](./MoralesLarios.FOOP/__Doc/1_Intro.md#índice-completo-de-la-documentación)
los lista todos con una descripción de cada uno.

---

## 6. Cómo compilar y ejecutar las pruebas

Desde esta carpeta (`src/`):

```powershell
# Restaurar y compilar toda la solución
dotnet build .\MoralesLarios.OOFP.sln

# Ejecutar las pruebas del núcleo
dotnet test .\MoralesLarios.OOFP.Unit.Tests\MoralesLarios.OOFP.Unit.Tests.csproj

# Ejecutar sólo las pruebas de EnsureFp
dotnet test .\MoralesLarios.OOFP.Unit.Tests\MoralesLarios.OOFP.Unit.Tests.csproj --filter "FullyQualifiedName~EnsureFp"
```

> ⚠️ Los proyectos de **integración** (`*.Integration.Tests`) pueden requerir recursos externos
> (base de datos, endpoints HTTP). Revisa el `appsettings.test.json` correspondiente antes de
> ejecutarlos.

---

## 🗂️ Temas pendientes: mejoras, nomenclatura y profesionalización

La carpeta [**`Temas Pendientes`**](./Temas%20Pendientes/README.md) reúne los documentos de trabajo
sobre el estado de la solución: qué hay que arreglar, en qué orden y qué cambiaría para
profesionalizar la biblioteca. Ninguno de ellos modifica código: son inventarios y guías de decisión.

| Documento | Contenido |
|---|---|
| [🗂️ **Índice de la carpeta**](./Temas%20Pendientes/README.md) | Resumen global de los 89 puntos, resumen por proyecto, plan de trabajo por bloques y orden de lectura recomendado. |
| [🔴🟠 Mejoras de prioridad crítica y alta](./Temas%20Pendientes/Mejoras-Prioridad-Critica-y-Alta.md) | Puntos 1-37: bugs que producen resultados incorrectos, seguridad, inyección de dependencias, culturas y contratos rotos. |
| [🟡 Mejoras de prioridad media](./Temas%20Pendientes/Mejoras-Prioridad-Media.md) | Puntos 38-63: rendimiento y acceso a datos, diseño de API y coherencia funcional. |
| [🟢 Mejoras de prioridad baja](./Temas%20Pendientes/Mejoras-Prioridad-Baja.md) | Puntos 64-89: código muerto, erratas en identificadores públicos, mensajes al usuario y documentación. |
| [🔤 Consejos de nomenclatura](./Temas%20Pendientes/Consejos-Nomenclatura.md) | Propuesta de renombrado en 10 niveles (solución, proyectos, carpetas, tipos, métodos y propiedades), con tablas «nombre actual → nombre propuesto» y una estrategia de migración con `[Obsolete]`. |
| [🏗️ Profesionalización (1/2): ingeniería y calidad](./Temas%20Pendientes/Profesionalizacion-Ingenieria-y-Calidad.md) | Higiene del repositorio, `Directory.Build.props`, gestión centralizada de paquetes, analizadores, nulabilidad, estrategia de pruebas, metadatos NuGet, SemVer, CI/CD y seguridad de la cadena de suministro. |
| [🎨 Profesionalización (2/2): diseño de API y producto](./Temas%20Pendientes/Profesionalizacion-Diseno-API-y-Producto.md) | Superficie pública, asincronía y cancelación, modelo de errores tipado, i18n, rendimiento de `MlResult<T>`, arquitectura por capas, observabilidad, documentación como producto, comunidad y hoja de ruta en 4 fases. |

**Por dónde empezar:** el [índice de la carpeta](./Temas%20Pendientes/README.md) → los puntos 🔴 críticos
→ la fase 1 de la hoja de ruta (credibilidad del repositorio, sin cambios de comportamiento).

---

## Compatibilidad

- `.NET 9`
- `.NET 8`

---

## Ver también

- 📘 [README general del repositorio](../README.md)
- 📘 [README del núcleo `MoralesLarios.OOFP`](./MoralesLarios.FOOP/README.md)
- 📘 [Introducción técnica y mapa de la documentación](./MoralesLarios.FOOP/__Doc/1_Intro.md)
- 🗂️ [Temas pendientes](./Temas%20Pendientes/README.md)
