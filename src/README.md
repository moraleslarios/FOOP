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
error viaja como **dato estructurado** (`MlErrorDetails`) hasta el borde de la aplicación.

Sobre esa base común se construyen capas independientes: value objects validados, validación
declarativa, IO seguro, repositorios EF Core, servicios de aplicación, traducción a
`IActionResult`, controladores REST genéricos, caché y clientes HTTP tipados. Todas hablan el
mismo idioma, con las mismas convenciones de nombres (`Bind*`, `Map*`, `Match*`, `ExecSelf*`,
`Try*`, `*Async`), de modo que aprender una familia sirve para todas.

---

## 2. Por dónde empezar

| Si quieres… | Ve a… |
|---|---|
| Entender la filosofía y el mapa completo | [`docs/1_Intro.md`](docs/1_Intro.md) |
| Aprender el núcleo con ejemplos y patrones | [`MoralesLarios.OOFP.Core/README.md`](MoralesLarios.OOFP.Core/README.md) |
| Validar precondiciones al entrar en un método | [`docs/EnsureFp/EnsureFp.md`](docs/EnsureFp/EnsureFp.md) |
| Encadenar operaciones que devuelven `MlResult` | [`docs/Bind/3_Bind.md`](docs/Bind/3_Bind.md) |
| Transformar el valor de un resultado válido | [`docs/Map/1_Map.md`](docs/Map/1_Map.md) |
| Salir del carril y devolver un valor concreto | [`docs/Match/1_Match.md`](docs/Match/1_Match.md) |
| Ver la referencia archivo a archivo del núcleo | [`docs/core-types/README.md`](docs/core-types/README.md) |
| Saber qué hay que arreglar y en qué orden | [`Temas Pendientes/README.md`](./Temas%20Pendientes/README.md) |

---

## 3. Proyectos de producción

Todos ellos tienen su propio `README.md` con documentación detallada.

### Núcleo

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.FOOP**](MoralesLarios.OOFP.Core/README.md) *(assembly `MoralesLarios.OOFP`)* | Corazón del ecosistema: `MlResult<T>`, `MlErrorDetails`, `EnsureFp` y las familias de extensiones `Bind`, `Map`, `Match`, `ExecSelf`, `Several`, `Transformations` y `Bucles`. **Es el único proyecto que hay que conocer de verdad.** |
| [**MoralesLarios.OOFP.Shared**](MoralesLarios.OOFP.Shared/README.md) | Tipos compartidos de bajo nivel y soporte de paginación. |
| [**MoralesLarios.OOFP.Shared**](./MoralesLarios.OOFP.Shared/README.md) | Constantes compartidas entre proyectos, **sin ninguna dependencia**. Evita que dos capas se pongan de acuerdo mediante literales duplicados. |

### Dominio, semántica y validación

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.ValueObjects**](./MoralesLarios.OOFP.ValueObjects/README.md) | Value objects tipados y autovalidados que hacen imposibles los estados inválidos. |
| [**MoralesLarios.OOFP.ValueObjects.FileSystem**](MoralesLarios.OOFP.ValueObjects.FileSystem/README.md) | Value objects específicos de rutas, ficheros y directorios. |
| [**MoralesLarios.OOFP.Validation**](./MoralesLarios.OOFP.Validation/README.md) | Base común de la validación funcional: contratos y extensiones sobre `MlResult<T>`. |
| [**MoralesLarios.OOFP.Validation.DataAnnotations**](./MoralesLarios.OOFP.Validation.DataAnnotations/README.md) | Implementación con `System.ComponentModel.DataAnnotations`. |
| [**MoralesLarios.OOFP.Validation.FluentValidation**](MoralesLarios.OOFP.Validation.FluentValidation/README.md) | Implementación con FluentValidation, con paridad de características respecto a la anterior. |

### Infraestructura y utilidades

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.FileSystem**](MoralesLarios.OOFP.FileSystem/README.md) | Operaciones de ficheros y directorios que devuelven `MlResult` en lugar de lanzar excepciones. |
| [**MoralesLarios.OOFP.Configuration**](MoralesLarios.OOFP.Configuration/README.md) | Lectura segura de configuración (`IConfiguration`) con resultados explícitos. |
| [**MoralesLarios.OOFP.Logging**](MoralesLarios.OOFP.Logging/README.md) | Registro de trazas encadenable sobre `MlResult<T>` sin romper el carril. |

### Persistencia

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.EntityFrameworkCore**](MoralesLarios.OOFP.EntityFrameworkCore/README.md) | Repositorios genéricos sobre EF Core en dos sabores: funcional (`MlResult`) y orientado a objetos. |
| [**MoralesLarios.OOFP.EntityFrameworkCore.Sample**](MoralesLarios.OOFP.EntityFrameworkCore.Sample/README.md) | Piezas de integración entre los repositorios EF Core y la capa Web API. |

### Servicios y exposición web

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.Application**](MoralesLarios.OOFP.Application/README.md) | Servicios de aplicación funcionales: orquestan repositorios y validación devolviendo `MlResult`. |
| [**MoralesLarios.OOFP.AspNetCore.Results**](MoralesLarios.OOFP.AspNetCore.Results/README.md) | Puente entre `MlResult<T>` e `IActionResult`, con traducción a `ProblemDetails` (RFC 7807) y códigos HTTP correctos. |
| [**MoralesLarios.OOFP.AspNetCore.Controllers**](MoralesLarios.OOFP.AspNetCore.Controllers/README.md) | Controladores REST genéricos listos para heredar (CRUD completo). |
| [**MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching**](MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching/README.md) | Los mismos controladores con caché configurable por controlador. |

### Consumo HTTP

| Proyecto | Propósito |
|---|---|
| [**MoralesLarios.OOFP.Http**](MoralesLarios.OOFP.Http/README.md) | Clientes HTTP tipados que convierten respuestas y errores de red en `MlResult<T>`. |

---

## 4. Proyectos de pruebas

La solución incluye **14 proyectos de pruebas** que funcionan como verificación viva del
ecosistema y, en la práctica, como documentación ejecutable de cada comportamiento:

| Proyecto | Cubre |
|---|---|
| `MoralesLarios.OOFP.Core.Tests.Unit` | El núcleo: `MlResult`, `EnsureFp` y todas las familias de extensiones |
| `MoralesLarios.OOFP.ValueObjects.Tests.Unit` | Value objects |
| `MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit` | Value objects de IO |
| `MoralesLarios.OOFP.ValueObjects.FileSystem.2.Tests.Unit` | Value objects de IO (segunda batería) |
| `MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit` | Validación con DataAnnotations |
| `MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit` | Validación con FluentValidation |
| `MoralesLarios.OOFP.Configuration.Tests.Unit` | Lectura de configuración |
| `MoralesLarios.OOFP.Application.Tests.Unit` | Servicios de aplicación |
| `MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit` | Traducción a `IActionResult` y `ProblemDetails` |
| `MoralesLarios.OOFP.Http.Tests.Unit` | Clientes HTTP (unitarias) |
| `MoralesLarios.OOFP.Http.Tests.Integration` | Clientes HTTP (integración) |
| `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` | Repositorios EF Core sobre base en memoria/archivo |
| `MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration` | Repositorios EF Core contra base real |
| `MoralesLarios.OOFP.Logging.Sample` | Logging funcional |

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
   `MlErrorDetails` completo, y con sufijo **`…Arg`**, que usa
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

📚 Documentación completa: [`docs/EnsureFp/EnsureFp.md`](docs/EnsureFp/EnsureFp.md)
(índice) y sus nueve páginas de familia:
[Núcleo](docs/EnsureFp/1_EnsureFpCore.md) ·
[Agregación](docs/EnsureFp/2_EnsureFpAggregation.md) ·
[Cadenas](docs/EnsureFp/3_EnsureFpStrings.md) ·
[Números](docs/EnsureFp/4_EnsureFpNumbers.md) ·
[Colecciones](docs/EnsureFp/5_EnsureFpCollections.md) ·
[Tipos](docs/EnsureFp/6_EnsureFpTypes.md) ·
[Nullables](docs/EnsureFp/7_EnsureFpNullables.md) ·
[Asíncronas](docs/EnsureFp/8_EnsureFpAsync.md) ·
[Mensajes](docs/EnsureFp/9_EnsureFpMessages.md).

### 📦 Nuevo proyecto `MoralesLarios.OOFP.Shared`

Proyecto sin dependencias cuya misión es alojar las **constantes que comparten varios
proyectos**. Nace de un bug real: la clave del diccionario `MlErrorDetails.Metadata` se escribía
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

El núcleo cuenta con **57 documentos** en `docs/`, organizados por familia
funcional. El [índice completo](docs/1_Intro.md#índice-completo-de-la-documentación)
los lista todos con una descripción de cada uno.

---

## 6. Cómo compilar y ejecutar las pruebas

Desde esta carpeta (`src/`):

```powershell
# Restaurar y compilar toda la solución
dotnet build .\MoralesLarios.OOFP.sln

# Ejecutar las pruebas del núcleo
dotnet test .\MoralesLarios.OOFP.Core.Tests.Unit\MoralesLarios.OOFP.Core.Tests.Unit.csproj

# Ejecutar sólo las pruebas de EnsureFp
dotnet test .\MoralesLarios.OOFP.Core.Tests.Unit\MoralesLarios.OOFP.Core.Tests.Unit.csproj --filter "FullyQualifiedName~EnsureFp"
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
- 📘 [README del núcleo `MoralesLarios.OOFP`](MoralesLarios.OOFP.Core/README.md)
- 📘 [Introducción técnica y mapa de la documentación](docs/1_Intro.md)
- 🗂️ [Temas pendientes](./Temas%20Pendientes/README.md)
