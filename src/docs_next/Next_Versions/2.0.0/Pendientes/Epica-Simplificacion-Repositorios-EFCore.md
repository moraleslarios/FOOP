# Épica · Simplificación de la capa de repositorios de `EntityFrameworkCore`

> **Versión destino:** 2.0.0
> **Proyecto afectado:** `MoralesLarios.OOFP.EntityFrameworkCore` (en exclusiva)
> **Origen:** auditoría específica de la capa `Repositories` solicitada por el propietario
> **Estado:** 🔄 Parcialmente realizada · el eje del ciclo de vida ✅ completado (`Transient` único); el desacople de los imperativos, pendiente
> **Esfuerzo:** XL (~4-6 jornadas: ~1 para la red de seguridad, ~2-3 para desacoplar, ~1 para el registro y el `README`)
> **Riesgo:** 🔴 muy alto · retira 18 ficheros, 149 métodos y 4 miembros públicos de registro
> **Objetivo del propietario:** conservar **solo** los repositorios funcionales (los que devuelven
> `MlResult`), desechar los imperativos y dejar el registro en el contenedor reducido a algo sencillo.

---

> ## ⚠️ Actualización · el eje del ciclo de vida ya está resuelto (`Transient` único)
>
> Parte de esta épica **ya se ha ejecutado**, por decisión expresa del propietario, dentro de la
> simplificación general de la librería: **`Transient` es el único ciclo de vida de toda la solución** y
> el ciclo de vida **deja de ser un eje de la API pública**.
>
> **Lo que ya está hecho** (registrado como puntos **62**, **63** y **64** de
> [`oofp-upgrade-2.prompt.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) §3.3):
>
> - ✅ **D11 resuelto** · `AddOofpRepositoriesTransient`, `AddOofpRepositoriesScoped` y
>   `AddOofpRepositoriesSingleton` **se han retirado**. Queda un **único**
>   `AddOofpRepositories<T, TContext>()` que registra las 16 interfaces con `AddTransient`.
> - ✅ **D10 resuelto por eliminación** · ya no existe forma de registrar un repositorio como
>   `Singleton`, así que la *captive dependency* sobre el `DbContext` **es inalcanzable**.
> - ✅ **D9 resuelto** · `EfRepository` y `ThrowingEfRepository` **ya no implementan `IDisposable`**
>   ni llaman a `GetContext().Dispose()`. Un repositorio no es dueño del `DbContext` que le inyectan.
>   Esto era **obligatorio** con `Transient`: el contenedor rastrea las instancias `IDisposable` y las
>   dispone al cerrar el ámbito, con lo que el repositorio habría destruido un `DbContext` ajeno.
> - ✅ En la capa de aplicación, los 6 métodos
>   `Add{Transient|Scoped|Singleton}CrudServices[Duplex]FpWithoutRepositories` se han reducido a
>   `AddCrudServicesFpWithoutRepositories()` y `AddCrudServicesDuplexFpWithoutRepositories()`.
> - ✅ `AddOOFPIO()` registra `IMlFileSystem` como `Transient` (antes `Singleton`).
> - ✅ **9 puentes `[Obsolete]`** del eje del ciclo de vida **retirados**. El único superviviente en
>   `RegisterServices.Obsolete.cs` es `ResolveRepoFp` → `GetRepository`.
> - ✅ `TestSupport/RegisterServices.cs` actualizado a `AddOofpRepositories`.
>
> **Decisiones que esto cierra:**
>
> - **D-03 · CERRADA** · la pregunta «¿debe `AddOofpRepositories` rechazar `ServiceLifetime.Singleton`?»
>   **desaparece**: el parámetro `lifetime` de la §4.4 **no se añade**. No hay nada que rechazar porque
>   no hay nada que elegir. Si el anfitrión necesita de verdad un `Singleton`, registra él su propia
>   envoltura e inyecta `IServiceScopeFactory` para abrir un ámbito por operación.
> - La firma propuesta en la **§4.4** queda **superada**: léela sin el parámetro
>   `ServiceLifetime lifetime` y con `services.AddTransient<…>()` en lugar de `services.Add(new
>   ServiceDescriptor(…, lifetime))`.
>
> **Excepción documentada** · el único `AddSingleton` que sobrevive en la solución está en
> `MoralesLarios.OOFP.Logging.Sample\Program.cs` L22:
> `AddSingleton<ILoggerFactory>(… new SerilogLoggerFactory(serilogLogger, dispose: false))`. Es
> **infraestructura de Serilog**, que *debe* ser singleton por contrato de `ILoggerFactory`, está en un
> proyecto de muestra y **no forma parte de la API pública**. Queda **fuera** del alcance de la
> unificación.
>
> **Lo que sigue pendiente de esta épica** · todo el resto: el desacoplamiento de la familia imperativa
> (US-03, US-04, US-05), la retirada de `BuildServiceProvider()` (D2) y la red de seguridad (US-01).

---

---

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Radiografía del código actual](#2-radiografía-del-código-actual)
3. [Debilidades detectadas](#3-debilidades-detectadas)
4. [Arquitectura de destino](#4-arquitectura-de-destino)
5. [Alcance](#5-alcance)
6. [Historias de usuario](#6-historias-de-usuario)
7. [Orden de ejecución](#7-orden-de-ejecución)
8. [Rupturas y registro obligatorio](#8-rupturas-y-registro-obligatorio)
9. [Decisiones abiertas](#9-decisiones-abiertas)
10. [Definición de terminado](#10-definición-de-terminado)
11. [Apéndice · código actual y propuesto](#11-apéndice--código-actual-y-propuesto)

---

## 1. Contexto y problema

El proyecto mantiene **dos familias paralelas de repositorios** sobre la misma funcionalidad:

| Familia | Carpeta | Ficheros | Métodos | Devuelve |
|---|---|---:|---:|---|
| Funcional (**se conserva**) | `Repositories/` | 18 | **178** | `MlResult<T>` |
| Imperativa (**se desecha**) | `Repositories.Imperative/` | 18 | **149** | la entidad, o lanza excepción |

El objetivo parece a primera vista una tarea de borrado. **No lo es**, y ahí está el riesgo real de
esta épica: aunque **ningún proyecto de la solución consume la familia imperativa desde fuera**, la
familia funcional **sí la consume por dentro**, y lo hace a través de un **localizador de servicios
estático**. Borrar la carpeta sin más deja la capa funcional sin implementación.

```mermaid
graph LR
    C["Consumidor<br/>IEfRepository&lt;T&gt;"] --> FP["EfCrudRepository<br/>(familia FP)"]
    FP -.->|"RegisterServices.GetRepository&lt;T&gt;()<br/>13 llamadas en 7 ficheros"| RS["RegisterServices<br/>static IServiceCollection _services<br/>BuildServiceProvider()"]
    RS -.-> IMP["ThrowingEf*<br/>(familia imperativa)"]
    IMP --> EF["DbContext.Set&lt;T&gt;()"]
```

La cadena correcta debería ser `Consumidor → FP → DbSet<T>`. Hoy hay **dos saltos artificiales** en
medio, y uno de ellos construye un contenedor de dependencias nuevo en cada acceso.

---

## 2. Radiografía del código actual

### 2.1 Estructura del proyecto

```text
MoralesLarios.OOFP.EntityFrameworkCore/
├── Enums/SortDirection.cs                    (solo lo usa la familia FP)
├── GlobalUsings.cs                           (18 entradas; la 8.ª importa el namespace imperativo)
├── Helpers/Extensions.cs                     (FormatCompositeKey, PrivateOrderBy — solo FP)
├── Helpers/Extensions.Obsolete.cs            (puente GetPkValues, 0 llamadas internas)
├── RegisterServices.cs                       (~150 líneas, 3 métodos casi idénticos)
├── RegisterServices.Obsolete.cs              (4 puentes [Obsolete])
├── README.md                                 (1037 líneas, desactualizado)
├── Repositories/                             (18 ficheros — familia FP, SE CONSERVA)
└── Repositories.Imperative/                  (18 ficheros — familia imperativa, SE DESECHA)
```

### 2.2 El acoplamiento crítico · 13 llamadas en 7 ficheros

`RegisterServices` expone un localizador de servicios estático:

```csharp
// RegisterServices.cs
private static IServiceCollection? _services;

internal static IServiceProvider ServiceProvider
    => _services?.BuildServiceProvider() ?? throw new ArgumentNullException("The field Provider is null");

public static TRepo GetRepository<TRepo>()
    => ServiceProvider.GetService<TRepo>() ?? throw new ArgumentException($"{typeof(TRepo).Name} is not registered …");
```

Los repositorios funcionales lo invocan **en la inicialización de campo**, de modo que la resolución
ocurre al construir el objeto, fuera de todo control del contenedor:

| Fichero FP | Llamadas | Campos | Resuelve |
|---|---:|---|---|
| `EfCreateRepository.cs` | 1 | `_internalRepoAdder` | **imperativo** |
| `EfReadRepository.cs` | 1 | `_repoReader` | **imperativo** |
| `EfDeleteRepository.cs` | 2 | `_internalRepoDeleter`, `_repoReaderFp` | imperativo + FP |
| `EfUpdateRepository.cs` | 2 | `_internalRepoUpdater`, `_repoReaderFp` | imperativo + FP |
| `EfCrudRepository.cs` | 2 | `_repoReaderFp`, `_repoWriterFp` | FP |
| `EfWriteRepository.cs` | 3 | `_repoAdderFp`, `_repoDeleterFp`, `_repoUpdaterFp` | FP |
| `EfPagedRepository.cs` | 2 | `_repoReaderPaginatorFp`, `_repoWriterFp` | FP |

**4 llamadas resuelven la familia imperativa** y **9 resuelven la propia familia funcional**. Las dos
mitades hay que desmontarlas: las primeras porque su destino desaparece, las segundas porque el
mecanismo es inaceptable aunque el destino se conserve.

> `ResolveRepoFp` (el puente `[Obsolete]`) tiene **0 usos internos**.

### 2.3 Lo que hace realmente cada capa

La capa imperativa es una envoltura **finísima** sobre `DbSet<T>`; la funcional es una envoltura
sobre la imperativa que añade la validación y el raíl de error:

```csharp
// Repositories.Imperative/ThrowingEfReadRepository.cs — el trabajo real
public T Find(params object[] keys) => internalDbContext.Set<T>().Find(keys)!;

// Repositories/EfReadRepository.cs — la envoltura funcional
public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    => EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
               .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
               .TryMap (x => _repoReader!.Find(keyValues))      // ← único punto de contacto con EF
               .TryBind(x => x.FailIfNull(notFoundErrorDetails));
```

**Este es el dato que hace viable la épica:** la capa imperativa no aporta lógica, solo indirección.
Sustituir `_repoReader!.Find(keyValues)` por `GetContext().Set<T>().Find(keyValues)` conserva el
comportamiento **exacto** y elimina la dependencia.

### 2.4 Visibilidad real de la familia imperativa

De sus 149 métodos, **solo 13 son públicos**, repartidos en 4 interfaces:

| Tipo | Visibilidad | Métodos públicos |
|---|---|---:|
| `IGetContextable` | `public` | 1 |
| `IThrowingEfCreateRepository<T>` | `public` ← incoherente | 4 |
| `IThrowingEfUpdateRepository<T>` | `public` ← incoherente | 4 |
| `IThrowingEfDeleteRepository<T>` | `public` ← incoherente | 4 |
| Las otras 5 interfaces y las 9 clases | `internal` | 0 |

Tres interfaces son `public` y sus cinco hermanas `internal`, sin ningún criterio que lo justifique:
es una **fuga de visibilidad**, no un contrato deliberado.

> ⚠️ **`IGetContextable` es el detalle que se escapa.** Es `public`, vive en el namespace
> `…Repositories.Imperative`, pero **la implementa la clase base funcional `EfRepository`**. Borrar
> la carpeta sin rescatarlo antes rompe la compilación de la capa que queremos conservar.

### 2.5 Consumidores externos dentro de la solución

Cuatro proyectos referencian el `.csproj`:

| Proyecto | Qué usa | ¿Usa lo imperativo? |
|---|---|---|
| `OOFP.Application` | `IEfRepository<TEntity>` en `Services/CrudServiceFp.cs` (L14, L306) | **No** |
| `…EntityFrameworkCore.TestSupport` | 4 interfaces heredan de `IEfRepository<T>`; 4 clases derivan de `EfCrudRepository<T, TestDbContext>`; `AddOofpRepositories` para 4 entidades (✅ ya renombrado desde `AddOofpRepositoriesTransient`) | **No** |
| `…EntityFrameworkCore.Tests.Integration` | recibe `IVinosRepo`; **las 4 llamadas están comentadas y no hay ningún `[Fact]`** | **No** |
| `…EntityFrameworkCore.Sample` | referencia el `.csproj` pero no usa ningún tipo de repositorio | **No** |

**Cero referencias a `IThrowingEf*` o `ThrowingEf*` en toda la solución.** El borrado no rompe a
ningún consumidor interno. Y, como el paquete `2.0.0` **no está publicado en NuGet**, tampoco hay
consumidores externos reales que dependan de las 3 interfaces filtradas.

### 2.6 Red de seguridad disponible: ninguna

**0 tests activos** ejercitan los repositorios, ni funcionales ni imperativos. El único fichero
relacionado, `…Tests.Integration/Repos/RepoVinorFpTests.cs`, tiene 4 escenarios **comentados** y
ningún atributo de test. `OOFP.Application.Tests.Unit` no referencia `CrudServiceFp` ni
`IEfRepository`.

> Esto convierte la red de seguridad en la **primera** historia de usuario, no en la última.

---

## 3. Debilidades detectadas

Agrupadas por naturaleza, con las cifras verificadas. Las marcadas con ⛔ son las que **no deben
repetirse** en el diseño de destino.

### 3.1 Resolución de dependencias

| # | Debilidad | Cifra | Consecuencia |
|---|---|---|---|
| D1 ⛔ | **Localizador de servicios estático** | 13 llamadas en 7 ficheros | El repositorio decide sus dependencias; no se puede sustituir ni simular en un test |
| D2 ⛔ | **`BuildServiceProvider()` en una propiedad** | 1 llamada, invocada desde los 13 puntos | Construye un **contenedor nuevo en cada acceso**: grafos duplicados, singletons falsos, fugas de memoria y un `DbContext` **distinto** del inyectado al repositorio |
| D3 ⛔ | **Estado estático mutable** | `private static IServiceCollection? _services` | No es seguro entre hilos, el último registro gana y los tests en paralelo se pisan |
| D4 | **Resolución en inicializador de campo** | 13 campos | La resolución ocurre al construir, no al usar; un registro posterior no se ve |

### 3.2 Arquitectura

| # | Debilidad | Cifra | Consecuencia |
|---|---|---|---|
| D5 ⛔ | **Doble capa para la misma funcionalidad** | 178 + 149 = 327 métodos | Todo cambio hay que hacerlo dos veces; la capa imperativa solo aporta indirección |
| D6 ⛔ | **Agregados por localizador en vez de por herencia o composición** | 9 de las 13 llamadas | `EfCrudRepository` y `EfWriteRepository` podrían delegar sin tocar el contenedor |
| D7 | **Fuga de visibilidad** | 3 interfaces `public` frente a 5 `internal` | Superficie pública involuntaria |
| D8 | **`IGetContextable` mal ubicado** | `public`, en el namespace imperativo, implementado por la base **FP** | Acopla la capa que se conserva a la carpeta que se borra |
| D9 ✅ **RESUELTA** | **`IDisposable` sobre un `DbContext` inyectado** | `EfRepository.Dispose()` y `ThrowingEfRepository.Dispose()` hacían `GetContext()?.Dispose()` | Con `AddScoped` el contenedor **también** lo disponía → doble `Dispose` y `ObjectDisposedException`. **Ya corregido**: ambas clases dejan de implementar `IDisposable` (punto **63**) |
| D10 ✅ **RESUELTA** | **`AddOofpRepositoriesSingleton`** | 1 método público | Un repositorio singleton capturaba un `DbContext` scoped: *captive dependency* de manual. **Ya corregido por eliminación**: el método se ha retirado (punto **62**) |

### 3.3 Registro en el contenedor

| # | Debilidad | Cifra | Consecuencia |
|---|---|---|---|
| D11 ✅ **RESUELTA** | **3 métodos casi idénticos** | `…Transient`, `…Scoped`, `…Singleton` | La única diferencia era el tiempo de vida. **Ya corregido**: se ha unificado en un solo `AddOofpRepositories` que registra siempre `Transient`, sin parámetro de ciclo de vida (punto **62**) |
| D12 | **16 servicios registrados por llamada** | 8 `IThrowingEf*` + 8 `IEf*` | La mitad sobra en cuanto desaparezca la familia imperativa |
| D13 | **Nombres de interfaz y clase que no concuerdan** | `IEfRepository<T>` → `EfCrudRepository<T,TContext>` | Dificulta leer el registro; además coexisten `EfRepository` (base no genérica) e `IEfRepository<T>` |
| D14 | **Formato roto** | alineación de espacios desalineada tras los renombrados | Ruido en cada diff |

### 3.4 Calidad del código

| # | Debilidad | Cifra |
|---|---|---|
| D15 | **Código comentado en una interfaz pública** | **26 líneas** de firmas en `IEfRepository<T>` (L14-39) |
| D16 | **Duplicación literal de firmas** | 6 firmas paginadas repetidas entre `IThrowingEfPagedReadRepository<T>` e `IThrowingEfPagedRepository<T>` |
| D17 | **`virtual` incoherente** | 137 declaraciones (135 públicas + 2 `protected`); pero `EfUpdateRepository` tiene **2 métodos públicos no virtuales**, contradiciendo al `README` |
| D18 | **`where` ausentes** | `IThrowingEfRepository<T>` e `IThrowingEfPagedRepository<T>` no declaran `where T : class` |
| D19 | **Campo de instancia innecesario** | `private string typeName = typeof(T).Name;` en `EfReadRepository` se recalcula por instancia en vez de ser `static readonly` |
| D20 | **Global using de la capa que se borra** | `GlobalUsings.cs`, línea 8 |
| D21 | **5 atributos `[Obsolete]`** | `GetPkValues`, `ResolveRepoFp`, `AddTransientOOFPRepos`, `AddScopedOOFPRepos`, `AddSingletonOOFPRepos` (todos `error: false`) |
| D22 | **Sin `InternalsVisibleTo`** | los tipos `internal` no son testeables |
| D23 | **`README` desactualizado** | 1037 líneas: describe carpetas `Repos/` y `OopRepos/` y un `Helpers/Constants.cs` **inexistentes**, y documenta la versión `1.0.14` cuando el `.csproj` dice `2.0.0` |

### 3.5 Errata detectada en documentación ya existente

La épica [`Epica-Limpieza-Codigo-Muerto.md`](Epica-Limpieza-Codigo-Muerto.md) sitúa el **punto 64**
(«22 declaraciones comentadas») en `Repositories/IEfWriteRepository.cs`, líneas 13-34. **La
ubicación real es `Repositories/IEfRepository.cs`, líneas 14-39, y son 26 líneas.**
`IEfWriteRepository.cs` tiene 14 líneas en total y su cuerpo está vacío. Hay que corregir esa épica
o absorber el punto 64 en la US-07 de esta.

---

## 4. Arquitectura de destino

### 4.1 Principio rector

> Un repositorio funcional recibe su `DbContext` por constructor, habla **directamente** con
> `DbSet<T>` dentro del raíl `Try*`, y **no conoce el contenedor de dependencias**.

### 4.2 Estructura resultante

```text
MoralesLarios.OOFP.EntityFrameworkCore/
├── Enums/SortDirection.cs                    (sin cambios)
├── GlobalUsings.cs                           (17 entradas: se retira el namespace imperativo)
├── Helpers/Extensions.cs                     (sin cambios)
├── RegisterServices.cs                       (~40 líneas, 1 método + sobrecargas)
├── Repositories/
│   ├── IGetContextable.cs                    ← RESCATADO del namespace imperativo
│   └── … los 18 ficheros FP, ya sin localizador
└── (Repositories.Imperative/ ELIMINADA)
```

Balance: **−18 ficheros**, **−149 métodos**, **−110 líneas** en `RegisterServices`, **−13 llamadas**
al localizador, **−1 `BuildServiceProvider()`**, **−1 campo estático mutable**.

### 4.3 Cómo se resuelven los agregados sin localizador

Es la decisión de diseño central. Tres opciones; la recomendada es la **B**.

**Opción A · herencia en cadena.** `EfReadRepository` → `EfWriteRepository` → `EfCrudRepository` →
`EfPagedRepository`. Cero duplicación, pero `EfWriteRepository` heredaría los 16 métodos de lectura,
que su interfaz no declara: semántica sucia.

**Opción B · composición en el constructor (recomendada).** El agregado instancia sus colaboradores
con el mismo `DbContext`, mediante métodos factoría `protected virtual` que preservan la
extensibilidad:

```csharp
public class EfCrudRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfRepository<T>
    where T        : class
    where TContext : DbContext
{
    private readonly IEfReadRepository<T>  _reader = CreateReader(dbContext);
    private readonly IEfWriteRepository<T> _writer = CreateWriter(dbContext);

    /// <summary>Crea el colaborador de lectura. Sobrescribible para sustituirlo en pruebas.</summary>
    protected static virtual IEfReadRepository<T> CreateReader(TContext context) => new EfReadRepository<T, TContext>(context);
    …
}
```

Ventajas: no toca las firmas públicas, no toca el contenedor, el `DbContext` es **siempre el mismo
objeto** (hoy no lo es), los cuerpos actuales (`_repoReaderFp.TryFind(keyValues)`) **se conservan
tal cual** cambiando solo el origen del campo, e interfaces y jerarquía quedan intactas.

**Opción C · núcleo estático interno.** Un `internal static class EfOps<T>` con la lógica de EF, y
todos los repositorios como envolturas `Try*`. Elimina toda duplicación real, pero obliga a
reescribir los 178 cuerpos: más alcance y más riesgo para el mismo beneficio funcional.

> Decisión **D-01**.

### 4.4 El nuevo `RegisterServices`

> ✅ **Ya implementado**, aunque con dos desviaciones respecto al diseño original de esta sección:
> **no hay parámetro `lifetime`** (el ciclo de vida ya no es un eje de la API: siempre `Transient`) y
> **se siguen registrando las 16 interfaces**, porque el desacople de los repositorios imperativos
> (US-03/04/05) todavía no se ha hecho. Registrado como punto **62** de
> [`oofp-upgrade-2.prompt.md`](../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md) §3.3.

Un **único** punto de entrada, sin parámetros y con un **único ciclo de vida**:

```csharp
public static IServiceCollection AddOofpRepositories<T, TContext>(this IServiceCollection services)
    where T        : class
    where TContext : DbContext
{
    // 8 interfaces imperativas (IThrowingEf*Repository<T>)  -> ThrowingEf*Repository<T, TContext>
    // 8 interfaces funcionales  (IEf*Repository<T>)         -> Ef*Repository<T, TContext>
    services.AddTransient<IEfReadRepository<T>,      EfReadRepository<T, TContext>>();
    services.AddTransient<IEfCreateRepository<T>,    EfCreateRepository<T, TContext>>();
    services.AddTransient<IEfUpdateRepository<T>,    EfUpdateRepository<T, TContext>>();
    services.AddTransient<IEfDeleteRepository<T>,    EfDeleteRepository<T, TContext>>();
    services.AddTransient<IEfWriteRepository<T>,     EfWriteRepository<T, TContext>>();
    services.AddTransient<IEfRepository<T>,          EfCrudRepository<T, TContext>>();
    services.AddTransient<IEfPagedReadRepository<T>, EfPagedReadRepository<T, TContext>>();
    services.AddTransient<IEfPagedRepository<T>,     EfPagedRepository<T, TContext>>();
    // … y las 8 imperativas equivalentes

    return services;
}
```

**Ya han desaparecido**: `AddOofpRepositoriesTransient`, `AddOofpRepositoriesScoped`,
`AddOofpRepositoriesSingleton` y 3 de los 4 puentes `[Obsolete]` de `RegisterServices.Obsolete.cs`.

**Se conservan todavía** (su retirada sigue siendo parte de esta épica y de la de unidad de trabajo):
`_services`, `ServiceProvider`, `BuildServiceProvider()`, `GetRepository<TRepo>()` y su único puente
`[Obsolete]`, `ResolveRepoFp`.

> ✅ **D10 resuelto y D-03 sin efecto.** `AddOofpRepositoriesSingleton` no se ha sustituido por
> `ServiceLifetime.Singleton`: se ha **eliminado**, y con ella la posibilidad de elegir. Al no existir
> ningún parámetro `lifetime`, la pregunta de si el método debe rechazar `Singleton` en tiempo de
> ejecución (**D-03**) deja de tener sentido. El `DbContext` lo sigue registrando el anfitrión con
> `AddDbContext`, normalmente `Scoped`, y los repositorios `Transient` lo comparten dentro del ámbito.
---

## 5. Alcance

### 5.1 Dentro

- Eliminar íntegra la carpeta `Repositories.Imperative/` (18 ficheros, 149 métodos).
- Desacoplar los 13 puntos de resolución de la familia funcional.
- Rescatar `IGetContextable` al namespace de la familia que se conserva.
- Reescribir `RegisterServices.cs` y eliminar `RegisterServices.Obsolete.cs`.
- Corregir `Dispose`, el `virtual` incoherente, `typeName`, las 26 líneas comentadas y
  `GlobalUsings.cs`.
- Crear la red de seguridad de tests que hoy no existe.
- Actualizar el `README.md` del proyecto y el prompt de migración.

### 5.2 Fuera

- **Cualquier otro proyecto de la solución**, salvo los ajustes mínimos e inevitables en
  `TestSupport` y `Tests.Integration` para que compilen y para alojar los tests nuevos.
- La **unidad de trabajo y las transacciones**: es la
  [épica 5](Epica-Unidad-de-Trabajo-y-Transacciones.md) (punto 40) y no se solapa con esta.
- **Subir la versión del paquete.** Todo queda en `2.0.0`; cualquier cambio de versión se consulta
  antes con el propietario.

---

## 6. Historias de usuario

### US-01 · Red de seguridad antes de tocar nada · 🟢 bajo · M

Sin tests, el desacoplamiento es indemostrable.

- [ ] Reactivar `…Tests.Integration/Repos/RepoVinorFpTests.cs`: descomentar los 4 escenarios
      (`TryAllAsync`, `TryAddAsync`, `TryUpdateAsync`, `TryRemoveAsync`) y dotarlos de `[Fact]`.
- [ ] Añadir cobertura de **cada una de las 8 interfaces funcionales** sobre `TestDbContext`,
      verificando camino válido **y** camino fallido (`IsFailure` con sus `MlErrorDetails`).
- [ ] Cubrir expresamente los comportamientos que el desacoplamiento podría alterar: `TryFind` con
      clave compuesta, `TryGetLast`, el `ORDER BY` de la paginación y el `TotalCount`.
- [ ] Los tests se escriben **contra la API pública** y **sin** `RegisterServices.GetRepository`.
- [ ] Build y suite completos en verde; el total de tests **sube** respecto a la línea base.

> No se usa `IsValid`/`IsFail` (obsoletos): la definición de terminado exige `CS0618 = 0`.

### US-02 · Rescatar `IGetContextable` · 🟠 alto · XS

- [ ] Mover `IGetContextable.cs` a `Repositories/`, con namespace
      `MoralesLarios.OOFP.EntityFrameworkCore.Repositories`.
- [ ] `EfRepository` compila sin el global using del namespace imperativo.
- [ ] Decidir si sigue siendo `public` → decisión **D-02**.
- [ ] Cambio de namespace registrado como ruptura de compilación en los 6 sitios, y el renombrado en
      `oofp-upgrade-2.renames.md`.

### US-03 · Desacoplar los repositorios hoja de la familia imperativa · 🔴 muy alto · L

Afecta a `EfCreateRepository`, `EfReadRepository`, `EfUpdateRepository` y `EfDeleteRepository`.

- [ ] Retirar los 4 campos `_internalRepoAdder`, `_repoReader`, `_internalRepoUpdater` y
      `_internalRepoDeleter`.
- [ ] Sustituir cada llamada `_repoXxx!.Metodo(...)` por la operación equivalente sobre
      `GetContext().Set<T>()`, **respetando literalmente** la semántica actual: `AsNoTracking()` en
      las lecturas, `Where(filter).AsEnumerable().Last()` en `TryGetLast`, y el mismo tratamiento de
      `null!`.
- [ ] Los envoltorios `EnsureFp` / `TryMap` / `TryBind` y los mensajes de error **no cambian**.
- [ ] El operador `!` desaparece de esas llamadas: ya no hay campo anulable.
- [ ] Los tests de la US-01 siguen en verde **sin modificarse**.

### US-04 · Desacoplar los agregados de la familia funcional · 🔴 muy alto · M

Afecta a `EfCrudRepository`, `EfWriteRepository` y `EfPagedRepository` (las 9 llamadas FP→FP).

- [ ] Aplicar la opción elegida en **D-01** (recomendada: composición con factorías
      `protected virtual`).
- [ ] El constructor primario `(TContext dbContext)` **se conserva**: `TestSupport` deriva de
      `EfCrudRepository<T, TestDbContext>` y no debe tocarse.
- [ ] Todos los colaboradores comparten **la misma instancia** de `DbContext` que el agregado.
- [ ] `RegisterServices.GetRepository` queda con **0 usos** en todo el proyecto.

### US-05 · Eliminar la carpeta `Repositories.Imperative/` · 🔴 muy alto · S

- [ ] Borrar los 18 ficheros con `git rm` (no con borrado manual).
- [ ] Retirar la línea 8 de `GlobalUsings.cs`.
- [ ] Comprobar que `Helpers/` y `Enums/` siguen compilando (su uso era exclusivamente funcional).
- [ ] Build `--no-incremental` con **0 errores** y sin aumento de avisos.
- [ ] Las 3 interfaces públicas filtradas (`IThrowingEfCreateRepository`,
      `IThrowingEfUpdateRepository`, `IThrowingEfDeleteRepository`) registradas como ruptura de
      compilación.

### US-06 · Simplificar el registro en el contenedor · 🔴 muy alto · M

- [ ] Reescribir `RegisterServices.cs` según §4.4: **1 método**, **8 registros**, sin estado
      estático y sin `BuildServiceProvider()`.
- [ ] Eliminar `RegisterServices.Obsolete.cs` y sus 4 puentes.
- [x] 
      para 4 entidades.
- [ ] Corregir la alineación del fichero (D14).
- [ ] Documentar en el `README` por qué no se debe usar `ServiceLifetime.Singleton`.
- [x] 
      `AddOofpRepositoriesSingleton` y la firma de `AddOofpRepositories` registradas en los 6 sitios.

### US-07 · Limpieza de la capa conservada · 🟢 bajo · S

- [ ] Retirar las **26 líneas** comentadas de `IEfRepository<T>` (L14-39) y corregir la errata del
      punto 64 en `Epica-Limpieza-Codigo-Muerto.md` (§3.5).
- [ ] Homogeneizar `virtual`: los 2 métodos públicos no virtuales de `EfUpdateRepository` pasan a
      serlo, o el `README` deja de afirmar que todos lo son → decisión **D-04**.
- [ ] `typeName` pasa a `private static readonly`.
- [ ] Decidir el destino de `Extensions.GetPkValues` (`[Obsolete]` con 0 llamadas) → **D-05**.
- [ ] Evaluar si hace falta `InternalsVisibleTo` tras el borrado (D22): previsiblemente **no**,
      porque dejan de existir tipos `internal` relevantes.

### US-08 · Corregir el `Dispose` · 🟠 alto · XS

- [ ] `EfRepository` **deja de disponer** el `DbContext` que recibe: quien lo crea, lo dispone.
- [ ] Decidir si `EfRepository` sigue implementando `IDisposable` → decisión **D-06**.
- [ ] Añadir un test que construya un repositorio bajo `AddScoped`, cierre el ámbito y reutilice el
      contexto sin `ObjectDisposedException`.
- [ ] **Ruptura silenciosa**: registrada con sus patrones `grep` y su forma de detección.

### US-09 · Actualizar el `README.md` del proyecto · 🟢 bajo · M

Sobre las 1037 líneas y 50 encabezados actuales:

- [ ] Eliminar el H2 «Arquitectura: dos capas paralelas» y el H3 «Cómo resuelven sus dependencias los
      repositorios: el punto crítico», que dejan de describir la realidad.
- [ ] Corregir «Estructura del proyecto»: hoy menciona `Repos/`, `OopRepos/` y
      `Helpers/Constants.cs`, que **no existen**.
- [ ] Corregir la versión documentada: `1.0.14` → `2.0.0`.
- [ ] Revisar los 12 apartados de «Particularidades reales del código fuente»: el **1**
      (`BuildServiceProvider`), el **2** (`Dispose`) y el **11** (visibilidad OOP) quedan obsoletos.
- [ ] Reescribir «Registro en el contenedor» con el método único.
- [ ] `scripts\check-md-anchors4.ps1` en verde: **0 anclas rotas**.

### US-10 · Registro de rupturas y puerta de control · 🟠 alto · S

- [x] ✅ Cada ruptura de §8 presente en los **6 sitios** de `oofp-upgrade-2.prompt.md`.
- [x] ✅ Renombrados y cambios de namespace en `oofp-upgrade-2.renames.md`.
- [x] ✅ Numeración continuada **desde el 62** sin huecos ni reutilizaciones (puntos **62**, **63** y **64**).
- [x] ✅ Build `--no-incremental` y suite completa en verde; `CS1591 = CS0618 = 0` (0 errores, 236 avisos, 1606 pruebas).
- [x] ✅ Commit con Conventional Commits en castellano, con `!`, creado con `git commit -F` desde un
      fichero UTF-8 sin BOM (`9d84ec2`).

---

## 7. Orden de ejecución

El orden **no es negociable**: la familia funcional depende de la imperativa, así que el borrado va
necesariamente al final del desacoplamiento.

```mermaid
graph LR
    U1["US-01<br/>Red de seguridad<br/>M · 🟢"] --> U2["US-02<br/>IGetContextable<br/>XS · 🟠"]
    U2 --> U3["US-03<br/>Desacoplar hojas<br/>L · 🔴"]
    U3 --> U4["US-04<br/>Desacoplar agregados<br/>M · 🔴"]
    U4 --> U5["US-05<br/>Borrar lo imperativo<br/>S · 🔴"]
    U5 --> U6["US-06<br/>Registro simple<br/>M · 🔴"]
    U6 --> U7["US-07<br/>Limpieza<br/>S · 🟢"]
    U7 --> U8["US-08<br/>Dispose<br/>XS · 🟠"]
    U8 --> U9["US-09<br/>README<br/>M · 🟢"]
    U9 --> U10["US-10<br/>Rupturas<br/>S · 🟠"]
```

| Hito | Historias | Entregable verificable |
|---|---|---|
| **H1 · Protección** | US-01 | La suite cubre las 8 interfaces funcionales |
| **H2 · Desacoplamiento** | US-02 … US-04 | `GetRepository` con **0 usos**; tests en verde sin tocarlos |
| **H3 · Borrado** | US-05 | `Repositories.Imperative/` no existe; build en verde |
| **H4 · Registro** | US-06 | `RegisterServices.cs` ≈ 40 líneas, 1 método, 8 registros |
| **H5 · Pulido** | US-07 … US-09 | Sin código comentado; `README` veraz; anclas en verde |
| **H6 · Contrato** | US-10 | Las rupturas en los 6 sitios; commit conforme |

> **H2 es el tramo crítico.** Hasta que `GetRepository` tenga 0 usos no se puede borrar nada, y es
> donde la red de seguridad de H1 se gana su coste.

---

## 8. Rupturas y registro obligatorio

> ⚠️ Cada punto necesita sus **6 sitios**: tabla §3.3, subsección `#### N`, barrido de la Fase 5
> (punto 4), lista de la Fase 6 (punto 5), tabla del apéndice de versiones y, si hay renombrado o
> cambio de namespace, `oofp-upgrade-2.renames.md`.

**Último `#### N` usado en el prompt: 61 ⇒ esta épica arranca en el 62.** La asignación definitiva se
confirma en el momento de registrar cada punto.

| Nº previsto | Ruptura | Tipo | US |
|---:|---|---|---|
| 62 | `IGetContextable` cambia de `…Repositories.Imperative` a `…Repositories` | compilación + renombrado de namespace | US-02 |
| 63 | Desaparecen `IThrowingEfCreateRepository<T>`, `IThrowingEfUpdateRepository<T>` e `IThrowingEfDeleteRepository<T>` | compilación | US-05 |
| 64 | Desaparece `RegisterServices.GetRepository<TRepo>()` (y su puente `ResolveRepoFp`) | compilación | US-06 |
| **62** ✅ | Desaparecen `AddOofpRepositoriesTransient`, `AddOofpRepositoriesScoped` y `AddOofpRepositoriesSingleton` (y sus 3 puentes), y los 6 `Add*CrudServices[Duplex]FpWithoutRepositories`; queda un método por cosa registrable, siempre `Transient` | compilación + ⚠️ silenciosa (`Scoped`→`Transient`) | US-06 · **ya registrada** |
| **63** ✅ | `EfRepository` y `ThrowingEfRepository` dejan de implementar `IDisposable` | compilación | US-08 · **ya registrada** |
| **64** ✅ | `AddOOFPIO()` registra `IMlFileSystem` como `Transient` en vez de `Singleton` | ⚠️ silenciosa | US-06 · **ya registrada** |
| 66 | `AddOofpRepositories<T, TContext>` gana el parámetro opcional `ServiceLifetime lifetime` | **binaria** (compila, no enlaza) | US-06 |
| 67 | El contenedor ya **no** registra las 8 interfaces `IThrowingEf*` | compilación para quien las resolviera | US-06 |
| 68 | `EfRepository.Dispose()` **deja de disponer** el `DbContext` inyectado | **silenciosa** ⚠️ | US-08 |
| 69 | Los agregados comparten la misma instancia de `DbContext` que el agregado (antes resolvían una propia) | **silenciosa** ⚠️ | US-04 |

Las dos rupturas **silenciosas** (68 y 69) son las peligrosas: el consumidor compila igual y el
comportamiento cambia. La 68 **arregla** un doble `Dispose`, pero quien se apoyase en que el
repositorio cerraba el contexto verá contextos abiertos; la 69 **arregla** el seguimiento de
entidades, pero quien dependiera de tener grafos de cambios separados verá un único
`ChangeTracker`. Ambas necesitan su apartado «cómo detectarla» y sus patrones `grep`.

> Si la decisión **D-02** deja `IGetContextable` como `internal`, el punto 62 pasa de cambio de
> namespace a **retirada de un tipo público**: hay que reflejarlo.

---

## 9. Decisiones abiertas

| Id | Decisión | Opciones | Recomendación |
|---|---|---|---|
| **D-01** | Cómo resuelven sus colaboradores los agregados | A herencia en cadena · **B composición con factorías** · C núcleo estático interno | **B**: no toca firmas ni jerarquía, y conserva los cuerpos actuales |
| **D-02** | ¿`IGetContextable` sigue siendo `public`? | mantener `public` en `…Repositories` · pasar a `internal` · eliminar y dejar solo `EfRepository.GetContext()` | **Mantener `public`**: `GetContext()` ya es público en `EfRepository` y es el punto de extensión natural |
| **D-03** ✅ **CERRADA · sin efecto** | ¿Debe `AddOofpRepositories` rechazar `ServiceLifetime.Singleton`? | aceptarlo y documentar el riesgo · lanzar `ArgumentException` · avisar por log | **La pregunta desaparece**: no se añade ningún parámetro `lifetime`, así que no hay nada que rechazar. El método registra siempre `Transient` (punto **62**) |
| **D-04** | `virtual` en toda la superficie funcional | hacer virtuales los 2 métodos de `EfUpdateRepository` · corregir el `README` | **Hacerlos virtuales**: coherencia con los otros 135 |
| **D-05** 🔄 **PARCIAL** | Destino de los 5 `[Obsolete]` | retirarlos ahora con el resto de la poda · mantenerlos hasta la 3.0.0 como anuncia su comentario | **Retirar los de `RegisterServices`** (su destino desaparece) y **mantener `GetPkValues`**. ✅ Ya retirados 3 de los 4 (`AddTransientOOFPRepos`, `AddScopedOOFPRepos`, `AddSingletonOOFPRepos`); sobrevive `ResolveRepoFp` porque `GetRepository<TRepo>()` aún existe |
| **D-06** | ¿`EfRepository` sigue implementando `IDisposable`? | sí, con `Dispose` vacío · **no**, retirarlo | **Retirarlo**: un `Dispose` vacío invita al mal uso; pero es ruptura de compilación para quien haga `using` |
| **D-07** | ¿Sobrecarga que registre todas las entidades de un `DbContext`? | añadir `AddOofpRepositories<TContext>()` recorriendo el modelo · no añadirla | **No añadirla ahora**: cabe en una épica posterior y amplía el alcance |

---

## 10. Definición de terminado

- [ ] `Repositories.Imperative/` **no existe**; 18 ficheros y 149 métodos retirados.
- [ ] `RegisterServices.GetRepository` y `BuildServiceProvider()` con **0 apariciones** en el
      proyecto.
- [ ] **0 campos estáticos mutables** en `RegisterServices`.
- [ ] `RegisterServices.cs` ≈ 40 líneas, **1 método público** y **8 registros**;
      `RegisterServices.Obsolete.cs` eliminado.
- [ ] `GlobalUsings.cs` con 17 entradas, sin el namespace imperativo.
- [ ] **0 líneas comentadas** en las interfaces públicas del proyecto.
- [ ] Los tests de la US-01 cubren las 8 interfaces funcionales y **pasan sin modificarse** tras el
      desacoplamiento.
- [ ] `dotnet build MoralesLarios.OOFP.sln -c Release --no-incremental`: **EXITCODE=0, 0 errores**,
      `CS1591 = 0`, `CS0618 = 0`, y avisos **no superiores** a la línea base (236).
- [ ] `dotnet test MoralesLarios.OOFP.sln -c Release --no-build`: **0 con error, 0 omitidos**, total
      **superior** a 1606.
- [ ] `scripts\check-md-anchors4.ps1`: **0 anclas rotas**.
- [ ] `README.md` del proyecto sin referencias a la capa imperativa, a `Repos/`, a `OopRepos/`, a
      `Helpers/Constants.cs` ni a la versión `1.0.14`.
- [ ] Los puntos **62-69** registrados en los **6 sitios** de `oofp-upgrade-2.prompt.md`.
- [ ] Renombrados y cambios de namespace en `oofp-upgrade-2.renames.md`.
- [ ] Errata del punto 64 corregida en `Epica-Limpieza-Codigo-Muerto.md`.
- [ ] Todos los criterios de aceptación de las US-01…US-10 marcados.
- [ ] Commit `refactor(efcore)!: …` creado con `git commit -F`; **sin `git push` sin confirmación**.
- [ ] **La versión del paquete sigue en `2.0.0`**, salvo decisión expresa del propietario.

---

## 11. Apéndice · código actual y propuesto

### 11.1 Un repositorio hoja · antes y después

```csharp
// ───────── ANTES · Repositories/EfReadRepository.cs
public class EfReadRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfReadRepository<T>
    where T : class  where TContext : DbContext
{
    internal readonly IThrowingEfReadRepository<T>? _repoReader
        = RegisterServices.GetRepository<IThrowingEfReadRepository<T>>();   // ← localizador estático

    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
        => EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                   .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
                   .TryMap (x => _repoReader!.Find(keyValues))              // ← salto a lo imperativo
                   .TryBind(x => x.FailIfNull(notFoundErrorDetails));
}

// ───────── DESPUÉS
public class EfReadRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfReadRepository<T>
    where T : class  where TContext : DbContext
{
    private static readonly string typeName = typeof(T).Name;               // ← ya no es de instancia

    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
        => EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                   .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
                   .TryMap (x => GetContext().Set<T>().Find(keyValues)!)    // ← EF directo, misma semántica
                   .TryBind(x => x.FailIfNull(notFoundErrorDetails));
}
```

El raíl, los mensajes y el tipo de retorno son **idénticos**. Solo cambia el origen del dato.

### 11.2 `Dispose` · antes y después

```csharp
// ───────── ANTES · dispone un DbContext que no es suyo
public class EfRepository(DbContext dbContext) : IGetContextable, IDisposable
{
    public DbContext GetContext() => dbContext;
    public virtual void Dispose() => GetContext()?.Dispose();   // ⚠️ el contenedor también lo dispone
}

// ───────── DESPUÉS (según D-06, opción recomendada)
public class EfRepository(DbContext dbContext) : IGetContextable
{
    /// <summary>Obtiene el contexto de datos recibido por constructor.</summary>
    /// <remarks>El ciclo de vida del contexto pertenece a quien lo crea (normalmente el contenedor
    /// de dependencias). El repositorio no lo libera.</remarks>
    public DbContext GetContext() => dbContext;
}
```

### 11.3 El registro · antes y después

```csharp
// ───────── ANTES · 3 métodos × 16 registros = 48 líneas de registro + estado estático
public static IServiceCollection AddOofpRepositories<T, TContext>(this IServiceCollection services)
    where T : class  where TContext : DbContext
{
    _services = services;                                      // ⚠️ estado estático mutable
    services.AddScoped<IThrowingEfReadRepository  <T>, ThrowingEfReadRepository  <T, TContext>>();
    services.AddScoped<IThrowingEfCreateRepository<T>, ThrowingEfCreateRepository<T, TContext>>();
    // … 6 registros imperativos más …
    services.AddScoped<IEfReadRepository<T>, EfReadRepository<T, TContext>>();
    // … 7 registros funcionales más …
    return services;
}
// ✅ AddOofpRepositoriesTransient / …Scoped / …Singleton ya se han retirado (punto 62)
// + AddOofpRepositoriesSingleton (idéntico con AddSingleton)  ⚠️ captive dependency

// ───────── DESPUÉS · 1 método × 8 registros, sin estado
services.AddOofpRepositories<Vino, BodegaContext>();                              // Scoped
services.AddOofpRepositories<Cata, BodegaContext>(ServiceLifetime.Transient);      // Transient
```

### 11.4 Cómo reconocer el problema para no repetirlo

Las tres cifras que resumen esta épica, y que conviene vigilar en cualquier proyecto futuro:

| Síntoma | Cómo detectarlo |
|---|---|
| Localizador de servicios | `grep` de `GetService` o `GetRequiredService` **fuera** de la composición raíz |
| Contenedor construido en caliente | `grep` de `BuildServiceProvider` — debe aparecer **0 veces** en una librería |
| Estado estático de registro | `grep` de `static IServiceCollection` o `static IServiceProvider` |

---

## Ver también

- [`README.md` del proyecto](../../../../MoralesLarios.OOFP.EntityFrameworkCore/README.md) · documentación de la capa afectada
- [Épica · Unidad de trabajo y transacciones](Epica-Unidad-de-Trabajo-y-Transacciones.md) · punto 40, sobre el mismo proyecto
- [Épica · Limpieza de código muerto](Epica-Limpieza-Codigo-Muerto.md) · punto 64, con la errata descrita en §3.5
- [Índice de pendientes](README.md)
