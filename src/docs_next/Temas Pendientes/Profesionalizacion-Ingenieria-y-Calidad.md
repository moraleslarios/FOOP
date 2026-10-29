# Profesionalización (1 de 2): ingeniería, calidad y distribución

> 📌 **Qué es este documento**
> Consejos para llevar `MoralesLarios.OOFP` de «biblioteca personal que funciona» a **biblioteca
> profesional publicable**, centrados en *cómo se construye, se prueba, se empaqueta y se distribuye*.
> No contiene ningún cambio de código: es una guía de decisiones.

> ℹ️ **Qué NO entra aquí**
> - Los **defectos concretos del código** están en [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md),
>   [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) y en
>   [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) (crítica y alta).
> - Los **cambios de nombres** están en [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md).
> - El **diseño de la API, la asincronía, el rendimiento y el producto** están en
>   [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md).

---

## Índice

1. [Higiene del repositorio y de la solución](#1-higiene-del-repositorio-y-de-la-solución)
2. [`Directory.Build.props`: propiedades comunes](#2-directorybuildprops-propiedades-comunes)
3. [Gestión central de paquetes y coherencia de versiones](#3-gestión-central-de-paquetes-y-coherencia-de-versiones)
4. [Analizadores, estilo y reglas que detectan los bugs por sí solas](#4-analizadores-estilo-y-reglas-que-detectan-los-bugs-por-sí-solas)
5. [`nullable` y contratos de nulidad](#5-nullable-y-contratos-de-nulidad)
6. [Estrategia de pruebas](#6-estrategia-de-pruebas)
7. [Contrato de API pública](#7-contrato-de-api-pública)
8. [Empaquetado NuGet](#8-empaquetado-nuget)
9. [Versionado y gestión de cambios](#9-versionado-y-gestión-de-cambios)
10. [CI/CD con GitHub Actions](#10-cicd-con-github-actions)
11. [Seguridad de la cadena de suministro](#11-seguridad-de-la-cadena-de-suministro)
12. [Checklist de profesionalización](#12-checklist-de-profesionalización)

---

## 1. Higiene del repositorio y de la solución

Es lo primero que ve alguien que se plantea usar la biblioteca, y hoy transmite «proyecto en obras».

| Situación actual | Por qué importa | Propuesta |
|---|---|---|
| `MoralesLarios.OOFP - copia.sln` en la raíz | Un archivo llamado «copia» es la señal más rápida de proyecto no mantenido | Eliminarlo del control de versiones |
| Dos soluciones (`src/MoralesLarios.OOFP.sln` y `src/MoralesLarios.FOOP/MoralesLarios.FOOP.sln`) | Nadie sabe cuál es la buena; la CI puede compilar la equivocada | Una sola solución en la raíz de `src` |
| La carpeta se llama `MoralesLarios.FOOP` pero el `.csproj` es `MoralesLarios.OOFP.csproj` | Dos marcas para lo mismo (FOOP / OOFP) confunden en rutas, *namespaces* y NuGet | Elegir **una** y renombrar carpeta, proyecto, `RootNamespace` y `AssemblyName` |
| Documentación en `MoralesLarios.FOOP/__Doc/` | El doble subrayado es un artificio para que ordene primero; no es convención .NET | Mover a `docs/` (la carpeta `docs/` de la raíz **está vacía**) |
| `PendingTasks.txt` dentro de `__Doc` | Las tareas en un `.txt` no se priorizan, no se asignan y no se cierran | Convertir en *issues* de GitHub con etiquetas |
| `MoralesLarios.OOFP.lutconfig` en la raíz | Configuración de herramienta local suelta en la raíz | Mover a `build/` o `.config/`, o ignorarla |
| `bin/` y `obj/` aparecen en el árbol | Si están versionados, cada *pull* genera conflictos binarios | Revisar `.gitignore` con la plantilla oficial de .NET y purgar del historial si procede |
| Archivos vacíos o íntegramente comentados: `Services/GenService.cs`, `Repos/IEFRepoWriterFp.cs`, `EFCore/Helpers/Constants.cs`, `RangeEnumValueObject.cs` | Ruido que hace dudar de qué está terminado | Borrarlos (el historial de Git ya guarda el código) o completarlos |
| `Attributes/PkParameterAttribute.cs` no está en UTF-8 | Produce mojibake en revisiones y en cualquier editor moderno | `.editorconfig` con `charset = utf-8` y `.gitattributes` con `*.cs text eol=lf working-tree-encoding=UTF-8` |
| `GlobalUsings.cs` con `using` duplicados | Advertencias y confusión sobre qué está realmente disponible | Depurar y mantener uno por proyecto |

### Nombres de los proyectos de prueba

Hoy conviven **seis** convenciones distintas:

```text
MoralesLarios.OOFP.Unit.Tests                        ← Xxx.Unit.Tests
MoralesLarios.OOFP.Utilities.Tests.Unit              ← Xxx.Tests.Unit
MoralesLarios.OOFP.ValueObjects.IO.Test.Unit         ← Xxx.Test.Unit  (singular)
MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit      ← ".2" numerado
MoralesLarios.OOFP.HttpClients.Tests.Integration     ← Xxx.Tests.Integration
MoralesLarios.OOFP.EFCore.Integration.Tests          ← Xxx.Integration.Tests
MoralesLarios.OOFP.EFCore.Infrastructure.Tests       ← tercera categoría sin equivalente
```

**Propuesta:** una única convención `<ProyectoBajoPrueba>.Tests.<Unit|Integration>`, fusionar
`ValueObjects.IO.2.Tests.Unit` con su hermano y decidir si `Infrastructure.Tests` es realmente
integración. Además, separar carpetas `src/` y `tests/` en la solución, que es lo que espera
cualquiera que llegue de fuera.

> 💡 Faltan `README.md` en `MoralesLarios.OOFP.EFCore.WebApi` y `MoralesLarios.OOFP.WebControllers.Cache`;
> son los dos únicos proyectos sin documentación propia.

---

## 2. `Directory.Build.props`: propiedades comunes

Hoy cada `.csproj` repite (y a veces contradice) las mismas propiedades. Un único
`Directory.Build.props` en la raíz de `src` centraliza las decisiones y garantiza que **ningún
proyecto nuevo se quede fuera**.

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>

    <!-- Calidad: las advertencias son errores desde el primer día del proyecto nuevo -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-recommended</AnalysisLevel>

    <!-- Documentación y reproducibilidad -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <Deterministic>true</Deterministic>
    <EnablePackageValidation>true</EnablePackageValidation>

    <!-- Metadatos comunes del paquete -->
    <Authors>Alberto Morales Larios</Authors>
    <Company>MoralesLarios</Company>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/MoralesLarios/…</PackageProjectUrl>
    <RepositoryType>git</RepositoryType>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>
</Project>
```

Y un `Directory.Build.props` distinto dentro de `tests/` que desactive el empaquetado
(`<IsPackable>false</IsPackable>`) y active `CollectCoverage`.

> ⚠️ **`TreatWarningsAsErrors` de golpe generará cientos de errores.** El camino realista es
> activarlo y añadir `<NoWarn>` con la lista concreta que hoy incumple el código (por ejemplo
> `CS1591` de documentación ausente), e ir vaciando esa lista poco a poco. Así la deuda queda
> **acotada y visible** en lugar de crecer.

---

## 3. Gestión central de paquetes y coherencia de versiones

### El problema hoy

| Proyecto | Dependencia relevante | Versión |
|---|---|---|
| `HttpClients` | `Microsoft.Extensions.Http` | **9.0.6** |
| `WebServices` | `Microsoft.Extensions.Configuration.Abstractions` | **9.0.6** |
| `EFCore` | `Microsoft.EntityFrameworkCore` | **8.0.3** |
| `WebApi` | `Microsoft.AspNetCore.Mvc.Core` | **2.1.0** ⚠️ |
| `WebControllers` | `Microsoft.AspNetCore.Mvc.Core` | 2.3.9 |

Mezclar la familia 9.x con la 8.x en la misma solución arrastra a los consumidores un conflicto de
versiones que ellos no han pedido; y `Mvc.Core` 2.1.0 sobre `net8.0` es directamente una referencia
que no debería existir (está recogido como defecto de prioridad alta).

Además, la **versión de cada paquete** ha divergido: `1.0.14` (EFCore, HttpClients), `1.0.11`
(WebApi), `1.0.10` (WebServices), `1.0.5` (WebControllers). Nadie sabe qué combinación está probada
junta.

### Propuesta

1. **`Directory.Packages.props`** con `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`
   y todos los `<PackageVersion>` en un solo sitio.
2. Alinear toda la familia `Microsoft.Extensions.*` y `EntityFrameworkCore` a la **misma banda** que
   el `TargetFramework` (8.x para `net8.0`).
3. Sustituir los `PackageReference` de ASP.NET Core por
   `<FrameworkReference Include="Microsoft.AspNetCore.App" />`.
4. Decidir la política de versiones: **una versión única para toda la familia** (más simple de
   comunicar y de probar) o versionado independiente **documentado** con una matriz de
   compatibilidad en el `README.md`.
5. Activar `<NuGetAudit>true</NuGetAudit>` y `<NuGetAuditMode>all</NuGetAuditMode>` para que la
   compilación avise de vulnerabilidades conocidas.
6. **Renovate o Dependabot** para las actualizaciones, con agrupación por familia.

---

## 4. Analizadores, estilo y reglas que detectan los bugs por sí solas

Este es, con diferencia, **el mayor retorno de inversión** de toda la lista: varios de los defectos
críticos y altos ya detectados los habría cazado un analizador **en la primera compilación**.

| Analizador | Qué aporta aquí concretamente |
|---|---|
| `Microsoft.VisualStudio.Threading.Analyzers` | Detecta el `.Result` bloqueante y el `async` sin `await`; habría marcado el `MapAlwaysAsync` que no espera la tarea |
| `Meziantou.Analyzer` | Exige `ConfigureAwait(false)` en biblioteca, `CancellationToken` propagado, `StringComparison` explícito, `IFormatProvider` explícito (los bugs de cultura) |
| `StyleCop.Analyzers` | Coherencia de estilo, orden de miembros, documentación obligatoria |
| `Roslynator.Analyzers` | Simplificaciones y detección de código muerto / retornos ausentes |
| `SonarAnalyzer.CSharp` | Detecta el `return` que falta en `FusionFailErros` y el parámetro `length` que se ignora en `Key`/`Name` |
| `Microsoft.CodeAnalysis.BannedApiAnalyzers` | Prohibir explícitamente `DateTime.Now`, `Thread.CurrentThread.CurrentCulture`, `BuildServiceProvider()`, `Task.Result`, `Path.Combine` en proyectos web |
| `Microsoft.CodeAnalysis.PublicApiAnalyzers` | Congela la superficie pública: ningún cambio *breaking* pasa sin verse en el *diff* |

Un `BannedSymbols.txt` como este habría impedido tres defectos:

```text
M:Microsoft.Extensions.DependencyInjection.ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(Microsoft.Extensions.DependencyInjection.IServiceCollection);Usar IServiceProvider inyectado
P:System.Threading.Thread.CurrentThread;Usar CultureInfo.InvariantCulture explícita
P:System.Threading.Tasks.Task`1.Result;Usar await
```

Y un `.editorconfig` en la raíz con `charset`, `end_of_line`, `indent_style`, orden de `using`,
severidades por regla y `dotnet_diagnostic.IDE0055.severity = warning` para el formato.

---

## 5. `nullable` y contratos de nulidad

El código usa `= null!` y `null!` para silenciar al compilador (por ejemplo en
`IHttpClientFactoryManager`, en `CallRequest*ParamsInfo` o en `PkParameterAttribute.Description`).
Ese patrón **apaga precisamente la comprobación que evita los `NullReferenceException`** y, de hecho,
uno de los bugs críticos (`httpClientFactoryKey` que siempre vale `null`) nace de un `= null!`.

**Propuesta:**

- `<Nullable>enable</Nullable>` en todos los proyectos y **prohibir `null!`** por convención (regla
  de revisión), salvo en interoperabilidad justificada con comentario.
- Parámetros obligatorios: `ArgumentNullException.ThrowIfNull(x)` en la API imperativa, o la guarda
  `EnsureFp.NotNull` en la funcional — pero **una sola** de las dos por capa, no ambas mezcladas.
- Anotar los helpers con `[NotNullWhen(true)]`, `[MemberNotNullWhen]` y `[return: NotNullIfNotNull]`
  para que el análisis de flujo funcione en el código del consumidor.
- En `record`s de parámetros, si un miembro puede faltar, declararlo `T?` con valor por defecto real,
  nunca `T = null!`.

---

## 6. Estrategia de pruebas

### 6.1 La inversión de mayor valor: leyes algebraicas

Para una biblioteca de programación funcional, las pruebas más valiosas no son las de ejemplos, sino
las **basadas en propiedades** que verifican que `MlResult<T>` se comporta como un mónada. Con
`FsCheck` o `CsCheck` bastan unas decenas de líneas para cubrir miles de casos:

| Ley | Enunciado | Qué protege |
|---|---|---|
| Identidad izquierda | `Valid(x).Bind(f)` ≡ `f(x)` | Que `Bind` no altere el valor de entrada |
| Identidad derecha | `m.Bind(Valid)` ≡ `m` | Que no se pierdan errores ni detalles |
| Asociatividad | `m.Bind(f).Bind(g)` ≡ `m.Bind(x => f(x).Bind(g))` | Que encadenar de dos formas dé lo mismo |
| Functor | `m.Map(f).Map(g)` ≡ `m.Map(x => g(f(x)))` | Coherencia de la familia `Map` |
| Cortocircuito | Si `m.IsFail`, `f` **no** se ejecuta | El invariante central del *railway* |
| Paridad sync/async | `m.Bind(f)` ≡ `await m.BindAsync(f.ToAsync())` | Habría detectado el `MapAlwaysAsync` roto |

Esa última fila es clave: **una prueba paramétrica que compare cada método sincrónico con su gemelo
asíncrono** habría detectado por sí sola el bug de la tarea no esperada, y detectará los siguientes.

### 6.2 Pirámide y herramientas

| Nivel | Qué cubre | Herramientas |
|---|---|---|
| Unitario | Núcleo, `EnsureFp`, *value objects*, validadores, helpers | xUnit + FluentAssertions + NSubstitute |
| Propiedades | Leyes y paridad sync/async | FsCheck / CsCheck |
| Snapshot | JSON de `ProblemDetails`, mensajes de error | Verify |
| Integración | EF Core y HTTP reales | **Testcontainers** (SQL Server en Docker) y `WebApplicationFactory` |
| Contrato | Cliente ↔ controlador de la misma familia | Test que llame al `GenClientFp` contra el controlador base alojado |
| Mutación | Calidad real de las aserciones | Stryker.NET |
| Rendimiento | Rutas calientes | BenchmarkDotNet |

### 6.3 Cambios concretos recomendados

- **`App_Data` con base de datos física** → Testcontainers o, como mínimo, SQLite en memoria. Una
  base de datos versionada en el repositorio hace las pruebas dependientes de la máquina.
- **`appsettings.test.json`**: verificar que no contiene credenciales reales; usar
  *user secrets* o variables de entorno.
- **Cobertura** con `coverlet.collector`, umbral mínimo en CI (empezar en el valor actual y subirlo
  un punto por *sprint*, nunca bajarlo) y publicación del informe en el *pull request*.
- **Un test por defecto corregido**: cada elemento marcado en los documentos de mejoras debe cerrarse
  con la prueba que lo demuestra, no solo con el arreglo.
- **Los `*.Tests.Integration` deben poder ejecutarse en CI**; si hoy requieren una instancia local,
  están efectivamente sin ejecutar.

---

## 7. Contrato de API pública

Una biblioteca con esta superficie (miles de sobrecargas entre `Map*`, `Bind*`, `ExecSelf*`…)
necesita que **cualquier cambio en la API pública sea visible y deliberado**.

- `Microsoft.CodeAnalysis.PublicApiAnalyzers` con `PublicAPI.Shipped.txt` / `PublicAPI.Unshipped.txt`
  por proyecto: añadir un método público obliga a añadir una línea al fichero, y quitarlo produce
  error de compilación. El *diff* del `PublicAPI` es la mejor revisión de un cambio *breaking*.
- **`internal` y `sealed` por defecto**: hoy hay clases públicas que son detalle de implementación
  (helpers internos de composición de URL, constantes) y clases base no sella­das ni `abstract`.
- **Política de `[Obsolete]`**: mensaje con la alternativa, `DiagnosticId` propio y versión de
  retirada. Hoy se publica una clase entera marcada `[Obsolete]` y aun así referenciada por otros
  helpers, que es lo peor de los dos mundos.
- Marcar con `[EditorBrowsable(EditorBrowsableState.Advanced)]` las sobrecargas exóticas para que
  IntelliSense muestre primero las cinco o seis que se usan el 95 % de las veces.

---

## 8. Empaquetado NuGet

| Metadato | Estado deseable |
|---|---|
| `PackageId` | Coherente con la marca elegida (FOOP u OOFP), sin mezclar |
| `Description` | Dos frases claras: qué resuelve y para quién. Hoy conviene revisar que no sea genérica |
| `PackageTags` | `functional;railway-oriented;result;error-handling;efcore;aspnetcore` |
| `PackageReadmeFile` | El `README.md` del proyecto incluido en el paquete: es lo que se ve en nuget.org |
| `PackageIcon` | Un icono propio (128×128 PNG) |
| `PackageLicenseExpression` | `MIT` (o la que corresponda); **hoy falta `LICENSE` en el repositorio** |
| `RepositoryUrl` + SourceLink | `Microsoft.SourceLink.GitHub` para depurar dentro de la biblioteca |
| Símbolos | `.snupkg` publicado junto al paquete |
| `PackageReleaseNotes` | Generadas desde el `CHANGELOG.md` |
| `EnablePackageValidation` | Con `PackageValidationBaselineVersion` apuntando a la última publicada: detecta rupturas binarias antes de publicar |

También conviene un **paquete «meta»** (`MoralesLarios.OOFP.All`) que agrupe los proyectos para quien
quiera todo, sin obligar a los demás a arrastrar EF Core o ASP.NET Core.

---

## 9. Versionado y gestión de cambios

- **SemVer estricto** y por escrito: qué se considera *breaking* (incluye añadir un miembro a una
  interfaz pública, que hoy se plantea en varios puntos pendientes).
- **`MinVer` o `Nerdbank.GitVersioning`**: la versión sale de las etiquetas de Git, no de un número
  escrito a mano en cada `.csproj` (que es cómo se llega a `1.0.5` / `1.0.10` / `1.0.14` divergentes).
- **`CHANGELOG.md`** en formato *Keep a Changelog*, con secciones `Added` / `Changed` / `Fixed` /
  `Deprecated` / `Removed`. Los bugs críticos ya identificados merecen una entrada `Fixed` explícita,
  porque cambian el comportamiento observable.
- **Multi-*targeting***: el núcleo (`MlResult`, `EnsureFp`, *value objects*) no necesita ASP.NET Core;
  publicarlo también para `netstandard2.1` o `net9.0` multiplica su alcance sin coste de diseño.
- **Rama de mantenimiento** por versión mayor y política de soporte declarada.

---

## 10. CI/CD con GitHub Actions

La carpeta `.github` ya existe; conviene completarla con cuatro flujos:

| Flujo | Disparador | Contenido |
|---|---|---|
| `ci.yml` | *push* y *pull request* | `restore` → `build -warnaserror` → `test` con cobertura → publicación del informe. Matriz `ubuntu-latest` + `windows-latest` (imprescindible: varios bugs son **específicos de Windows**, como el `Path.Combine` en URLs y la cultura del hilo) |
| `codeql.yml` | *push* y programado | Análisis de seguridad de GitHub |
| `release.yml` | etiqueta `v*` | `pack` → validación → `nuget push` con `--skip-duplicate` y atestación de procedencia |
| `dependabot.yml` | programado | Actualización de NuGet y de las propias *actions* |

Complementos: protección de la rama principal con comprobaciones obligatorias, plantilla de *pull
request* con la casilla «he añadido la prueba que demuestra el arreglo», plantillas de *issue* para
error y para propuesta, y `ContinuousIntegrationBuild=true` en los paquetes publicados para que las
compilaciones sean reproducibles.

---

## 11. Seguridad de la cadena de suministro

- **`NuGetAudit`** activado y con fallo de compilación ante vulnerabilidades altas.
- **Firma de paquetes** y publicación con atestación de procedencia (`--source` + *provenance* de
  GitHub Actions).
- **SBOM** generado en la publicación.
- **Sin secretos en el repositorio**: revisar `appsettings.test.json` y cualquier cadena de conexión;
  añadir un escaneo de secretos (GitHub *secret scanning* o `gitleaks`) al flujo de CI.
- **`SECURITY.md`** con la política de comunicación de vulnerabilidades y los canales privados.
- Revisar los puntos ya identificados con implicación de seguridad: volcado del cuerpo de las
  respuestas HTTP en los mensajes de error, cabeceras mutadas en un `HttpClient` compartido y
  `Location` apuntando a un dominio de terceros.

---

## 12. Checklist de profesionalización

Marca cada casilla cuando esté hecha; el orden es el recomendado.

- [ ] Eliminar `- copia.sln`, unificar en una sola solución y separar `src/` de `tests/`
- [ ] Decidir la marca definitiva (FOOP u OOFP) y aplicarla a carpetas, proyectos y paquetes
- [ ] Crear `Directory.Build.props` y `Directory.Packages.props`
- [ ] Alinear todas las dependencias a la banda de `net8.0` y quitar `Mvc.Core` 2.1.0
- [ ] Añadir `.editorconfig`, `.gitattributes` y normalizar a UTF-8
- [ ] Incorporar los analizadores y `BannedSymbols.txt`
- [ ] Activar `TreatWarningsAsErrors` con `NoWarn` acotado y ponerse a vaciarlo
- [ ] Añadir `LICENSE`, `CHANGELOG.md`, `CONTRIBUTING.md`, `SECURITY.md`
- [ ] Escribir las pruebas de leyes y la paridad sync/async del núcleo
- [ ] Migrar las pruebas de integración a Testcontainers y ejecutarlas en CI
- [ ] Configurar `ci.yml`, `codeql.yml`, `release.yml` y `dependabot.yml`
- [ ] Congelar la API pública con `PublicApiAnalyzers`
- [ ] Completar los metadatos NuGet, SourceLink, símbolos y `EnablePackageValidation`
- [ ] Pasar el versionado a etiquetas de Git con `MinVer`
- [ ] Unificar la nomenclatura de los proyectos de prueba

---

## Ver también

- [`Profesionalizacion-Diseno-API-y-Producto.md`](Profesionalizacion-Diseno-API-y-Producto.md) — diseño
  de la API, asincronía, rendimiento, observabilidad y producto.
- [`Mejoras-Prioridad-Media.md`](Mejoras-Prioridad-Media.md) y
  [`Mejoras-Prioridad-Baja.md`](Mejoras-Prioridad-Baja.md) — defectos concretos del código.
- [`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) — defectos de prioridad crítica y alta.
- [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md) — propuesta de renombrado de clases, métodos y propiedades.
- [`README.md`](README.md) — índice de la carpeta.
