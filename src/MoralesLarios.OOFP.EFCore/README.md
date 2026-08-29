# MoralesLarios.OOFP.EFCore — Repositorios sobre EF Core en el raíl funcional

Capa de repositorios genéricos sobre **Entity Framework Core 8** que convierte el acceso a datos en operaciones que devuelven [`MlResult<T>`](../MoralesLarios.FOOP/__Doc/Types/MlResult.md): sin `try`/`catch`, sin comprobaciones de `null` y con el "no encontrado" tratado como un `Fail` con mensaje propio en lugar de como una excepción.

Es el proyecto **más grande y ambicioso** de la periferia de la solución: 35 ficheros, dos capas paralelas y ocho repositorios componibles por entidad.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Arquitectura: dos capas paralelas](#arquitectura-dos-capas-paralelas)
5. [Jerarquía de interfaces](#jerarquía-de-interfaces)
6. [Registro en el contenedor](#registro-en-el-contenedor)
7. [`IEFRepoReaderFp<T>` — lectura](#iefrepoReaderfpt--lectura)
8. [`IEFRepoAdderFp<T>` — inserción](#iefrepoadderfpt--inserción)
9. [`IEFRepoUpdaterFp<T>` — modificación](#iefrepoupdaterfpt--modificación)
10. [`IEFRepoDeleterFp<T>` — borrado](#iefrepodeleterfpt--borrado)
11. [`IEFRepoWriterFp<T>` y `IEFRepoFp<T>` — agregados](#iefrepowriterfpt-y-iefrepofpt--agregados)
12. [Paginación: `IEFPaginatorFp<T>`](#paginación-iefpaginatorfpt)
13. [Mensajes de error personalizados](#mensajes-de-error-personalizados)
14. [Helpers y `OrderBy`](#helpers-y-orderby)
15. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
16. [⚠️ Lo que NO incluye](#️-lo-que-no-incluye)
17. [Ejemplos prácticos](#ejemplos-prácticos)
18. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
19. [Mejores prácticas](#mejores-prácticas)
20. [Resumen](#resumen)
21. [Ver también](#ver-también)

---

## ¿Qué problema resuelve?

El acceso a datos con EF Core arrastra tres molestias constantes: **`Find` devuelve `null`**, **cualquier operación puede lanzar**, y el resultado **no se compone** con el resto de la lógica.

❌ **Con EF Core directamente:**

```csharp
public async Task<Vino> RenombrarAsync(int id, string nombre, CancellationToken ct)
{
    var vino = await _db.Vinos.FindAsync([id], ct);   // puede devolver null

    if (vino is null)
        throw new NotFoundException($"El vino {id} no existe");   // excepción para flujo normal

    vino.Nombre = nombre;

    try
    {
        await _db.SaveChangesAsync(ct);               // puede lanzar DbUpdateException
    }
    catch (DbUpdateConcurrencyException ex) { /* … */ }
    catch (DbUpdateException ex)            { /* … */ }

    return vino;
}
```

✅ **Con `IEFRepoFp<T>`:**

```csharp
public Task<MlResult<Vino>> RenombrarAsync(int id, string nombre, CancellationToken ct)
    => _repo.TryFindAsync($"El vino {id} no existe", ct, id)
            .MapAsync (v => { v.Nombre = nombre; return v; })
            .BindAsync(v => _repo.TryUpdateAsync(v, ct, v.Id));
```

| Problema | Cómo lo resuelve |
|---|---|
| `Find` devuelve `null` | `NullToFailed` lo convierte en `Fail` con mensaje |
| Cualquier operación lanza | Todo va dentro de `TryMap` / `TryBind` |
| Argumentos nulos o vacíos | `EnsureFp.NotNull` / `EnsureFp.NotEmpty` antes de tocar la BD |
| No se compone | El `MlResult` encadena con `Map`, `Bind`, `Match`… |

---

## Instalación y dependencias

| Dependencia | Versión | Para qué |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.3 | `DbContext`, `DbSet<T>`, `ToListAsync` |
| [`MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md) | — | `MlResult<T>`, `EnsureFp`, `TryMap`, `NullToFailed` |
| [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) | — | `PaginationInfo`, `PaginationResultInfo<T>` |
| [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) | — | `IntNotNegative` (conversiones de paginación) |

Destino: **`net8.0`**. Versión del paquete: **1.0.14**.

> ⚠️ **`Microsoft.Extensions.DependencyInjection` se usa sin declararse** como paquete propio: llega de forma transitiva a través de EF Core. Funciona, pero es una dependencia implícita.

```csharp
using MoralesLarios.OOFP.EFCore;              // AddTransientOOFPRepos, …
using MoralesLarios.OOFP.EFCore.Repos;        // IEFRepoFp<T>, EFRepoFp<T, TContext>
using MoralesLarios.OOFP.EFCore.Enums;        // OrderBy
using MoralesLarios.OOFP.Internals.Info;      // PaginationInfo, PaginationResultInfo<T>
```

---

## Estructura del proyecto

```
MoralesLarios.OOFP.EFCore/
├── Repos/                          ← 🟢 API PÚBLICA funcional (devuelve MlResult)
│   ├── EFRepoBaseFp.cs                 → base: GetContext() + Dispose()
│   ├── IEFRepoReaderFp.cs   / EFRepoReaderFp.cs
│   ├── IEFRepoAdderFp.cs    / EFRepoAdderFp.cs
│   ├── IEFRepoUpdaterFp.cs  / EFRepoUpdaterFp.cs
│   ├── IEFRepoDeleterFp.cs  / EFRepoDeleterFp.cs
│   ├── IEFRepoWriterFp.cs   / EFRepoWriterFp.cs        (adder+updater+deleter)
│   ├── IEFRepoFp.cs         / EFRepoFp.cs              (reader+writer)
│   ├── IEFPaginatorFp.cs                               (solo paginación)
│   ├── IEFRepoReaderPaginationFp.cs / EFRepoReaderPaginationFp.cs
│   └── IEFRepoPaginationFp.cs       / EFRepoPaginationFp.cs
├── OopRepos/                       ← 🔒 MOTOR INTERNO (devuelve T, clases internal)
│   ├── EFRepoBase.cs, EFRepo.cs, EFRepoReader.cs
│   ├── EFRepoAdder.cs, EFRepoUpdater.cs, EFRepoDeleter.cs
│   ├── EFRepoPagination.cs, EFRepoReaderPagination.cs
│   └── IGetContextable.cs
├── Helpers/
│   ├── Extensions.cs                   → GetPkValues, PrivateOrderBy
│   └── Constants.cs                    → ⚠️ vacío
├── Enums/OrderBy.cs                    → Ascending, Descending
├── RegisterServices.cs                 → Add{Transient|Scoped|Singleton}OOFPRepos
└── GlobalUsings.cs
```

---

## Arquitectura: dos capas paralelas

Cada repositorio existe **dos veces**: una versión OOP interna que habla con EF Core, y una versión funcional pública que la envuelve.

```
Tu servicio
     │
     ▼
IEFRepoReaderFp<T>          🟢 pública — devuelve MlResult<T>
  EFRepoReaderFp<T, TContext>
     │  valida con EnsureFp, envuelve en TryMap, NullToFailed
     ▼
IEFRepoReader<T>            🔒 internal — devuelve T (puede ser null, puede lanzar)
  EFRepoReader<T, TContext>
     │
     ▼
DbContext.Set<T>()          EF Core
```

| Capa | Namespace | Visibilidad | Devuelve | Rol |
|---|---|---|---|---|
| **Funcional** (`*Fp`) | `…EFCore.Repos` | **pública** | `MlResult<T>` | 🟢 **La que debes usar** |
| **OOP** | `…EFCore.OopRepos` | `internal` | `T` | 🔒 Motor interno |

Ejemplo del envoltorio, en `EFRepoAdderFp`:

```csharp
public virtual MlResult<T> TryAdd(T item)
    => EnsureFp.NotNull(item, "The entity item to add cannot be null")   // 1️⃣ valida
               .TryMap(x => _internalRepoAdder!.Add(item));             // 2️⃣ delega y captura
```

Y el motor OOP correspondiente:

```csharp
public T Add(T item)
{
    internalDbContext.Set<T>().Add(item);
    internalDbContext.SaveChanges();     // ⚠️ guarda en CADA operación
    return item;
}
```

> ⚠️ **Las clases OOP son `internal`**, así que **no puedes usarlas ni heredarlas** desde tu proyecto. Solo la capa `*Fp` es accesible. Las interfaces `IEFRepoReader<T>` etc. son públicas, pero sus implementaciones no.

> 💡 **Todos los métodos de la capa funcional son `virtual`**: puedes heredar de `EFRepoReaderFp<T, TContext>` y sobrescribir solo lo que necesites.

---

## Jerarquía de interfaces

```
IEFRepoReaderFp<T>       lectura: Find, First, Last, GetData, All
IEFRepoAdderFp<T>        Add, AddRange
IEFRepoUpdaterFp<T>      Update, UpdateRange
IEFRepoDeleterFp<T>      Remove, RemoveRange
IEFPaginatorFp<T>        AllPagination, GetDataPagination

IEFRepoWriterFp<T>            : Adder + Updater + Deleter
IEFRepoFp<T>                  : Reader + Writer                    ← 🟢 el más usado
IEFRepoReaderPaginationFp<T>  : Reader + Paginator
IEFRepoPaginationFp<T>        : Repo + Paginator                   ← 🟢 el más completo
```

| Necesito… | Interfaz |
|---|---|
| Solo leer | `IEFRepoReaderFp<T>` |
| Solo insertar | `IEFRepoAdderFp<T>` |
| Solo modificar | `IEFRepoUpdaterFp<T>` |
| Solo borrar | `IEFRepoDeleterFp<T>` |
| Escribir (las tres) | `IEFRepoWriterFp<T>` |
| CRUD completo | `IEFRepoFp<T>` ✅ |
| Leer + paginar | `IEFRepoReaderPaginationFp<T>` |
| CRUD + paginar | `IEFRepoPaginationFp<T>` ✅ |

> 💡 **Segrega por intención**: si un servicio solo consulta, inyecta `IEFRepoReaderFp<T>`. El compilador impedirá que escriba por accidente.

---

## Registro en el contenedor

Un solo método registra **las 16 interfaces** (8 OOP + 8 funcionales) para una entidad y su contexto:

```csharp
builder.Services.AddDbContext<JfCatasDbContext>(o => o.UseSqlServer(cs));

builder.Services.AddScopedOOFPRepos<Vino, JfCatasDbContext>();
builder.Services.AddScopedOOFPRepos<Cata, JfCatasDbContext>();   // una llamada por entidad
```

Tres variantes según el ciclo de vida:

| Método | Ciclo de vida | Recomendación |
|---|---|---|
| `AddTransientOOFPRepos<T, TContext>()` | `Transient` | ✅ Válido |
| `AddScopedOOFPRepos<T, TContext>()` | `Scoped` | ✅ **Recomendado** (coincide con `AddDbContext`) |
| `AddSingletonOOFPRepos<T, TContext>()` | `Singleton` | ❌ **Ver la advertencia** |

> ❌ **No uses `AddSingletonOOFPRepos` con un `DbContext` registrado con `AddDbContext`** (que es `Scoped`). Es una *captive dependency*: el repositorio `Singleton` retendría para siempre un contexto de un ámbito ya cerrado. `DbContext` **no es seguro entre hilos** y acumularía todo el *change tracker* durante la vida del proceso.

> ⚠️ **El nombre rompe la convención de la solución.** Aquí es `Add…OOFPRepos`; en otros proyectos es `AddMl…`.

### ❗ Cómo resuelven sus dependencias los repositorios: el punto crítico

Los repos `*Fp` **no reciben sus colaboradores por constructor**. Los resuelven mediante un localizador de servicios estático:

```csharp
public static class RegisterServices
{
    private static IServiceCollection? _services;      // ⚠️ estático

    internal static IServiceProvider ServiceProvider
        => _services?.BuildServiceProvider()           // ⚠️❗ construye un provider NUEVO cada vez
           ?? throw new ArgumentNullException("The field Provider is null");

    public static TRepo ResolveRepoFp<TRepo>()
        => ServiceProvider.GetService<TRepo>() ?? throw new ArgumentException(…);
}
```

Y en cada repositorio:

```csharp
public class EFRepoAdderFp<T, TContext>(TContext dbContext) : EFRepoBaseFp(dbContext), IEFRepoAdderFp<T>
{
    internal readonly IEFRepoAdder<T>? _internalRepoAdder =
        RegisterServices.ResolveRepoFp<IEFRepoAdder<T>>();   // ⚠️ en la construcción del objeto
}
```

**Consecuencias que debes conocer:**

| Consecuencia | Detalle |
|---|---|
| ⚠️ **Orden obligatorio** | `Add…OOFPRepos<T, TContext>()` debe llamarse **antes** de que se construya cualquier repositorio |
| ⚠️ **Un contenedor nuevo por resolución** | `BuildServiceProvider()` se ejecuta en **cada** acceso. Los `Singleton` se duplican y cada provider queda sin liberar |
| ⚠️ **Coste en rendimiento** | Construir un `ServiceProvider` es costoso; aquí ocurre varias veces por cada repositorio creado |
| ⚠️ **`DbContext` distinto** | El colaborador interno se resuelve del provider secundario, **no del ámbito actual**: puede no ser el mismo `DbContext` que recibió el repositorio por constructor |
| ⚠️ **Registro tardío invisible** | Los servicios añadidos después de la primera resolución sí aparecen (se re-construye), pero de forma imprevisible |
| ✅ **Error claro si falta** | Si no registraste la entidad, `ArgumentException` con el nombre del tipo y los métodos a usar |
| ⚠️ **La última llamada gana** | `_services` se sobrescribe en cada `Add…OOFPRepos`. Con varias entidades queda la última colección asignada (normalmente la misma instancia, así que suele funcionar) |

> ❗ **Este es el aspecto más delicado del proyecto.** Funciona en aplicaciones normales de ASP.NET Core porque `_services` apunta a la misma `IServiceCollection` que acaba en el contenedor real, pero el patrón de *service locator* con `BuildServiceProvider()` repetido es un antipatrón reconocido. Ten presente que **el `DbContext` que usa el motor interno puede no ser el del ámbito de la petición**, lo que afecta al seguimiento de entidades y a las transacciones.

> 💡 **Mitigación práctica**: registra siempre con `AddScopedOOFPRepos`, haz las llamadas **al principio** de la configuración de servicios, y no dependas de que dos operaciones consecutivas compartan el mismo *change tracker*.

---

## `IEFRepoReaderFp<T>` — lectura

```csharp
MlResult<T>       TryFind      (params object[] pk);
MlResult<T>       TryFind      (MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>> TryFindAsync (CancellationToken token = default, params object[] pk);
Task<MlResult<T>> TryFindAsync (MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);

MlResult<T>       TryFirst     (Expression<Func<T, bool>> filter);
MlResult<T>       TryFirst     (Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<T>       TryLast      (Expression<Func<T, bool>> filter);
MlResult<T>       TryLast      (Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
Task<MlResult<T>> TryLastAsync (Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryLastAsync (Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<IEnumerable<T>>       TryGetData     (Expression<Func<T, bool>> filter);
Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAll         ();
Task<MlResult<IEnumerable<T>>> TryAllAsync    (CancellationToken token = default);
```

| Método | Devuelve `Fail` cuando | Mensaje por defecto |
|---|---|---|
| `TryFind` | `pk` nulo, vacío, o no existe la fila | `"{valores} values not found data in {Entidad}"` |
| `TryFirst` | `filter` nulo o sin coincidencias | `"The query did not return any elements"` |
| `TryLast` | `filter` nulo o sin coincidencias | `"The query did not return any elements"` |
| `TryGetData` | `filter` nulo o error de BD | *(propaga la excepción como error)* |
| `TryAll` | Error de BD | *(propaga la excepción como error)* |

Implementación de `TryFind` (muestra el patrón completo):

```csharp
public virtual MlResult<T> TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk)
    => EnsureFp.NotNull(pk, "The object array pk cannot be null")
               .TryMap (x => EnsureFp.NotEmpty(pk, "The object array pk cannot be empty"))
               .TryMap (x => _repoReader!.Find(pk))
               .TryBind(x => x.NullToFailed(notFoundErrorDetails));   // 🔑 null → Fail
```

**Claves para PK compuestas**: el `params object[]` acepta varias claves en el orden declarado en `HasKey`.

```csharp
// PK simple
await repo.TryFindAsync(ct, 42);

// PK compuesta: HasKey(x => new { x.IdVino, x.IdCata, x.IdUsuario })
await repo.TryFindAsync(ct, idVino, idCata, idUsuario);
```

> 💡 **No hay `TryFirstOrDefault` ni `TryLastOrDefault`, y no hacen falta**: `TryFirst`/`TryLast` ya devuelven `Fail` en lugar de lanzar, así que cubren el caso seguro.

> 💡 **`TryAll` y `TryGetData` usan `AsNoTracking()`** en el motor interno: son de solo lectura y no cargan el *change tracker*. `TryFind`, en cambio, **sí hace seguimiento** (usa `Find`), lo que es correcto porque suele preceder a una modificación.

---

## `IEFRepoAdderFp<T>` — inserción

```csharp
MlResult<T>                    TryAdd          (T item);
Task<MlResult<T>>              TryAddAsync     (T item, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAddRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

Valida que el argumento no sea nulo y delega. **Cada llamada hace `SaveChanges` por su cuenta** (ver [Particularidades](#️-particularidades-reales-del-código-fuente)).

> 💡 **`TryAddRange` es una sola transacción implícita**: inserta todas las entidades y llama a `SaveChanges` una única vez. Prefiérelo a un bucle de `TryAdd`.

---

## `IEFRepoUpdaterFp<T>` — modificación

```csharp
MlResult<T>                    TryUpdate          (T item);
MlResult<T>                    TryUpdate          (T item, params object[] pk);
MlResult<T>                    TryUpdate          (T item, MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>>              TryUpdateAsync     (T item, CancellationToken token = default);
Task<MlResult<T>>              TryUpdateAsync     (T item, CancellationToken token = default, params object[] pk);
Task<MlResult<T>>              TryUpdateAsync     (T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
MlResult<IEnumerable<T>>       TryUpdateRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

**Dos comportamientos distintos según pases `pk` o no:**
```csharp
public MlResult<T> TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk)
    => EnsureFp.NotNull  (item, "The entity item to update cannot be null")
               .Bind  ( _  => EnsureFp.NotEmpty(pk, "The object array pk cannot be empty"))
               .Bind  ( pk => notFoundErrorDetails is null
                                ? _repoReaderFp!.TryFind(pk)
                                : _repoReaderFp!.TryFind(notFoundErrorDetails, pk))
               .TryMap( x  => _internalRepoUpdater!.Update(item));   // ⚠️ actualiza `item`, no `x`
```

> ✅ **Usa la sobrecarga con `pk`** si quieres un `Fail` claro cuando la fila no existe, en lugar de una `DbUpdateConcurrencyException`.

> ⚠️ **La entidad encontrada se descarta**: el `TryFind` sirve solo como comprobación de existencia y luego se actualiza el `item` que pasaste. Como `Find` **hace seguimiento**, la entidad de la base de datos queda en el *change tracker* y actualizar otra instancia con la misma clave puede provocar `InvalidOperationException` ("*The instance of entity type cannot be tracked because another instance with the same key value is already being tracked*"). Ver [Particularidades](#️-particularidades-reales-del-código-fuente).

---

## `IEFRepoDeleterFp<T>` — borrado

```csharp
MlResult<T>                    TryRemove          (T item);
MlResult<T>                    TryRemove          (params object[] pk);
MlResult<T>                    TryRemove          (MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>>              TryRemoveAsync     (T item, CancellationToken token = default);
Task<MlResult<T>>              TryRemoveAsync     (CancellationToken token = default, params object[] pk);
Task<MlResult<T>>              TryRemoveAsync     (MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
MlResult<IEnumerable<T>>       TryRemoveRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

> ✅ **Las sobrecargas con `pk` sí son correctas aquí**: hacen `TryFind`, y **borran la entidad encontrada** (`bdItem`), no una instancia externa. Es el patrón adecuado, a diferencia del *updater*.

```csharp
.Bind  ( pk     => _repoReaderFp!.TryFind(notFoundErrorDetails, pk))
.TryMap( bdItem => _internalRepoDeleter!.Remove(bdItem));   // ✅ borra la entidad de la BD
```

> ⚠️ **`TryRemove(params object[] pk)` puede confundirse con `TryRemove(T item)`** si tu entidad es un `object[]`, caso raro pero posible. Y ojo: `TryRemove(MlErrorsDetails, params object[])` frente a `TryRemove(params object[])` se resuelven por el primer argumento, así que un `string` como primera clave podría convertirse implícitamente en `MlErrorsDetails`. **Con PK de tipo `string`, usa siempre la sobrecarga explícita con mensaje.**

---

## `IEFRepoWriterFp<T>` y `IEFRepoFp<T>` — agregados

```csharp
public interface IEFRepoWriterFp<T> : IEFRepoAdderFp<T>, IEFRepoUpdaterFp<T>, IEFRepoDeleterFp<T> { }

public interface IEFRepoFp<T> : IEFRepoReaderFp<T>, IEFRepoWriterFp<T> { }
```

No añaden métodos: solo combinan. Las clases `EFRepoWriterFp` y `EFRepoFp` son **delegadores puros**, resuelven los repos especializados y reenvían cada llamada.

> ⚠️ **`IEFRepoWriterFp<T>` contiene ~19 líneas de firmas comentadas**, restos de cuando declaraba los métodos antes de heredarlos. Sin efecto funcional, pero es ruido.

---

## Paginación: `IEFPaginatorFp<T>`

```csharp
MlResult<PaginationResultInfo<T>>       TryAllPagination         (PaginationInfo paginationInfo,
                                                                  OrderBy orderBy = OrderBy.Ascending,
                                                                  Expression<Func<T, object>> orderByField = null!);
Task<MlResult<PaginationResultInfo<T>>> TryAllPaginationAsync    (…, CancellationToken ct = default!);
MlResult<PaginationResultInfo<T>>       TryGetDataPagination     (…, Expression<Func<T, bool>> filter = null!);
Task<MlResult<PaginationResultInfo<T>>> TryGetDataPaginationAsync(…, CancellationToken ct = default!);
```

### Los tipos de paginación

```csharp
public record PaginationInfo(int PageNumber, int PageSize)
{
    private const int MaxPageSize = 1000;

    public int PageNumber { get; init; } = Math.Max  (1, PageNumber);       // 🔑 mínimo 1
    public int PageSize   { get; init; } = Math.Clamp(PageSize, 1, 1000);   // 🔑 entre 1 y 1000
}

public record PaginationResultInfo<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount)
    : PaginationInfo(PageNumber, PageSize);
```

> 💡 **`PaginationInfo` se autocorrige**: `PageNumber = 0` pasa a `1`, `PageSize = 5000` se recorta a `1000`. **Nunca falla por valores fuera de rango**, los normaliza en silencio.

> 💡 **Conversión implícita desde tupla**, muy cómoda:
> ```csharp
> await repo.TryAllPaginationAsync((2, 25), OrderBy.Descending, x => x.FechaEntrada, ct);
> ```

### Ordenación

```csharp
public static IQueryable<T> PrivateOrderBy<T>(this IQueryable<T> source,
                                              OrderBy orderBy = OrderBy.Ascending,
                                              Expression<Func<T, object>> orderByField = null!)
    => orderByField is null ? source
                            : orderBy == OrderBy.Ascending ? source.OrderBy          (orderByField)
                                                           : source.OrderByDescending(orderByField);
```

> ⚠️ **Si no pasas `orderByField`, no hay `ORDER BY`** y el resultado de `Skip`/`Take` **no está garantizado**: SQL Server puede devolver las filas en cualquier orden, así que dos páginas consecutivas podrían repetir u omitir registros. **Pasa siempre `orderByField` al paginar.**

> 💡 **El campo es `Expression<Func<T, object>>`**, así que al ordenar por un tipo valor (`int`, `DateTime`) se genera un *boxing* en la expresión. EF Core lo traduce correctamente, pero es la razón de que no puedas encadenar varios criterios (no hay `ThenBy`).

---

## Mensajes de error personalizados

Todos los métodos de "búsqueda" tienen una sobrecarga con `MlErrorsDetails`, que acepta conversión implícita desde `string`:

```csharp
// Mensaje por defecto: "42 values not found data in Vino"
await repo.TryFindAsync(ct, 42);

// Mensaje propio (string → MlErrorsDetails implícito)
await repo.TryFindAsync($"No existe el vino con id {42}", ct, 42);

// Con detalles adicionales
await repo.TryFindAsync(MlErrorsDetails.FromErrorMessageWithValue(42, "Vino inexistente"), ct, 42);
```

Los mensajes por defecto vienen de `GetPkValues`:

```csharp
new object[] { 10 }.GetPkValues();        // "10"
new object[] { 10, "A" }.GetPkValues();   // "(10, A)"
```

> 💡 **Personaliza el mensaje si va a llegar al usuario final**: los mensajes por defecto están en inglés e incluyen el nombre del tipo de la entidad, lo cual **filtra detalles internos** en una API pública.

---

## Helpers y `OrderBy`

```csharp
public enum OrderBy { Ascending, Descending }
```

| Helper | Firma | Uso |
|---|---|---|
| `GetPkValues` | `this object[] → string` | Formatea las claves para los mensajes |
| `PrivateOrderBy` | `this IQueryable<T> → IQueryable<T>` | Ordenación condicional |

> ⚠️ **`Helpers/Constants.cs` está completamente vacío** (una clase `internal static` sin miembros). Residuo sin efecto.

---

## ⚠️ Particularidades reales del código fuente

### 1. ❗ `BuildServiceProvider()` en cada resolución

Ya descrito en [Registro en el contenedor](#-cómo-resuelven-sus-dependencias-los-repositorios-el-punto-crítico). Es el problema estructural más importante: **cada acceso a `RegisterServices.ServiceProvider` construye un contenedor nuevo**, que nunca se libera, y del que se resuelven los colaboradores internos.

### 2. ❗ `Dispose` libera el `DbContext` que gestiona el contenedor

```csharp
public class EFRepoBaseFp(DbContext dbContext) : IGetContextable, IDisposable
{
    public DbContext GetContext() => dbContext;

    public virtual void Dispose() => GetContext()?.Dispose();   // ⚠️❗
}
```

El `DbContext` **lo inyecta el contenedor**, que ya se encarga de liberarlo al cerrar el ámbito. Aquí el repositorio lo libera también.

**Consecuencias:**
```csharp
public class EFRepoBaseFp(DbContext dbContext) : IGetContextable, IDisposable
{
    public DbContext GetContext() => dbContext;

    public virtual void Dispose() => GetContext()?.Dispose();   // ⚠️❗
}
```

| Escenario | Qué ocurre |
|---|---|
| Repositorio `Scoped`, contexto `Scoped` | El contenedor libera ambos al cerrar el ámbito. **El contexto puede quedar dispuesto antes de que otro servicio del mismo ámbito lo use** → `ObjectDisposedException` |
| Repositorio `Transient` | ⚠️ **Peor**: cada repositorio liberado deja inservible el `DbContext` compartido del ámbito |
| `using (var repo = …)` explícito | 💥 Rompe el contexto para el resto de la petición |

> ❗ **Nunca llames a `Dispose()` a mano sobre un repositorio**, ni lo envuelvas en `using`. Deja que el contenedor gestione el ciclo de vida. Y **no registres los repos como `Transient` si comparten el `DbContext` del ámbito** — usa `Scoped`.

### 3. ⚠️ `SaveChanges` en cada operación: no hay unidad de trabajo

```csharp
public T Add(T item)
{
    internalDbContext.Set<T>().Add(item);
    internalDbContext.SaveChanges();     // ⚠️ commit inmediato
    return item;
}
```

**Cada `TryAdd`, `TryUpdate` y `TryRemove` confirma su propio cambio.** No existe `SaveChanges` público ni gestión de transacciones.

```csharp
// ⚠️ NO es atómico: si el segundo falla, el primero YA está confirmado
await repo.TryAddAsync(vino, ct);
await repoCatas.TryAddAsync(cata, ct);
```

> ⚠️ **Para operaciones atómicas necesitas una transacción explícita**, accediendo al contexto con `GetContext()`:
> ```csharp
> await using var tx = repo.GetContext().Database.BeginTransaction();
>
> var resultado = await repo.TryAddAsync(vino, ct)
>                           .BindAsync(_ => repoCatas.TryAddAsync(cata, ct));
>
> await resultado.MatchAsync(valid: async _ => { await tx.CommitAsync(ct);   return resultado; },
>                            fail:  async _ => { await tx.RollbackAsync(ct); return resultado; });
> ```
> ⚠️ Esto solo funciona si **ambos repositorios comparten el mismo `DbContext`**, lo que no está garantizado por el punto 1.

### 4. ❗ `TryLast` trae **todos** los registros filtrados a memoria

```csharp
public T LastOrDefault(Expression<Func<T, bool>> filter)
    => internalDbContext.Set<T>()
                        .AsNoTracking()
                        .Where(filter)
                        .AsEnumerable()      // ⚠️❗ ejecuta el SELECT completo
                        .LastOrDefault()!;
```

**`AsEnumerable()` materializa toda la consulta** y luego toma el último elemento en memoria. En una tabla con un millón de filas coincidentes, se transfieren **todas**.

> ❗ **Evita `TryLast`/`TryLastAsync` en tablas grandes.** SQL Server no soporta `LAST()`, y la alternativa correcta es invertir la ordenación y tomar el primero:
> ```csharp
> // ✅ Solo trae una fila
> var ultimo = await repo.TryGetDataPaginationAsync(
>                             new PaginationInfo(1, 1),
>                             OrderBy.Descending,
>                             x => x.FechaEntrada,
>                             x => x.Bodega == "X",
>                             ct)
>                        .MapAsync(p => p.Items.First());
> ```

### 5. ❗ En la capa OOP de paginación, el `ORDER BY` se aplica **después** de `Skip`/`Take`

```csharp
// EFRepoReaderPagination (capa OOP interna)
public IEnumerable<T> GetDataOrderby<TKey>(int pageNumber, int pageSize,
                                           Expression<Func<T, TKey>> orderBy, …)
    => GetInternalData(pageNumber, pageSize, filter)   // ya aplicó Skip().Take()
           .OrderBy(orderBy)                          // ⚠️❗ ordena DESPUÉS
           .ToList();
```

Esto **ordena solo las filas de la página**, no el conjunto completo: el contenido de cada página es esencialmente arbitrario.

> ✅ **La capa funcional lo hace bien**: `EFRepoReaderPaginationFp.TryGetInternalData` ordena antes de paginar.
> ```csharp
> .Where(filter)
> .PrivateOrderBy(orderBy, orderByField)   // ✅ primero ordena
> .Skip((pageNumber - 1) * pageSize)       // ✅ luego pagina
> .Take(pageSize)
> ```
> Como la capa OOP es `internal`, **no te afecta si usas la API funcional**. Es un motivo más para no intentar acceder a la capa OOP.

### 6. ❗ El `TotalCount` es incoherente entre la versión síncrona y la asíncrona

```csharp
// TryGetInternalData (SÍNCRONO)
.TryMap(items => new { Count = GetContext().Set<T>().Count(),               // ⚠️ SIN filtro
                       Items = items })

// TryGetInternalDataAsync (ASÍNCRONO)
.TryMapAsync(items => new { Count = GetContext().Set<T>().Where(filter!).Count(),   // ✅ CON filtro
                            Items = items })
```

| Versión | `TotalCount` |
|---|---|
| `TryGetDataPagination` (sync) | ⚠️ **Total de la tabla**, ignorando el filtro |
| `TryGetDataPaginationAsync` | ✅ Total de filas que cumplen el filtro |

> ❗ **El `TotalCount` de la versión síncrona con filtro es incorrecto**: si filtras 50 registros de una tabla de 10 000, devuelve `10000`. El número de páginas calculado en la interfaz de usuario sería erróneo. **Usa la versión asíncrona cuando pases filtro.**

> ⚠️ **Y en la versión asíncrona hay un riesgo complementario**: `Where(filter!)` usa el parámetro `filter`, que **es `null` cuando la llamada viene de `TryAllPaginationAsync`**. Eso produce una `ArgumentNullException` que el `TryMapAsync` convierte en `Fail`. Es decir, **`TryAllPaginationAsync` puede devolver `Fail` al calcular el total**. Si necesitas paginar todo, pasa un filtro trivial:
> ```csharp
> // ✅ Evita el null interno
> await repo.TryGetDataPaginationAsync((1, 25), OrderBy.Ascending, x => x.Id, x => true, ct);
> ```

### 7. ⚠️ `TryUpdate(item, pk)` puede provocar un conflicto de seguimiento

Como se explicó en el [*updater*](#iefrepoupdaterfpt--modificación): `TryFind` deja la entidad **con seguimiento** y luego se llama a `Update(item)` con **otra instancia** de la misma clave.

```csharp
// ⚠️ Riesgo: dos instancias con la misma PK en el change tracker
var vinoModificado = new Vino { Id = 42, Nombre = "Nuevo" };
await repo.TryUpdateAsync(vinoModificado, ct, 42);   // TryFind trae la original, luego Update(la nueva)
```

> ✅ **Patrón seguro**: recupera, modifica la instancia recuperada, y actualiza **esa misma**.
> ```csharp
> await repo.TryFindAsync($"El vino {id} no existe", ct, id)
>           .MapAsync (v => { v.Nombre = nombre; return v; })
>           .BindAsync(v => repo.TryUpdateAsync(v, ct));      // ⬅ sin pk: no repite el Find
> ```

### 8. ⚠️ `TryRemoveRangeAsync` no usa la variante asíncrona de `EnsureFp`

```csharp
public virtual Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default)
{
    var result = EnsureFp.NotNull(items, "…")        // ⚠️ NotNull, no NotNullAsync
                         .TryMapAsync(x => _internalRepoDeleter!.RemoveRangeAsync(items, token));
    return result;                                    // ⚠️ sin await
}
```

Funciona (devuelve la `Task` sin esperar), pero **es el único método del proyecto con ese estilo**; el resto usa `await` y `EnsureFp.NotNullAsync`.

### 9. ⚠️ `EFRepoReaderPaginationFp` no usa la capa OOP

A diferencia de todos los demás repos funcionales, este **accede al `DbContext` directamente**:

```csharp
.TryMap(filter => GetContext().Set<T>()      // ⚠️ directo, sin pasar por IEFRepoReaderPagination
                              .AsNoTracking()
                              …)
```

> 💡 **Es en realidad una ventaja**: al no pasar por el motor interno, evita el bug del `OrderBy` tras `Skip` descrito en el punto 5. Pero significa que `EFRepoReaderPagination` (OOP) **queda registrado y sin usar**.

### 10. ⚠️ `IntNotNegative` en `PaginationInfo` con anotaciones incorrectas

```csharp
public record PaginationInfo([property: Range(0, int.MinValue)] int PageNumber,   // ⚠️ Range(0, -2147483648)
                             [property: Range(0, int.MinValue)] int PageSize)
```

El máximo del rango es **`int.MinValue`**, menor que el mínimo. La anotación es inválida, pero **no tiene efecto** porque la normalización real la hacen `Math.Max` y `Math.Clamp`, y nadie valida ese atributo. (Reside en [`Internals`](../MoralesLarios.OOFP.Internals/README.md).)

### 11. Las clases OOP son `internal`, sus interfaces públicas

Puedes **declarar** una dependencia de `IEFRepoReader<T>` pero no implementarla con las clases del paquete ni heredarlas. En la práctica, **la capa OOP es inaccesible**.

### 12. `typeName` se calcula por instancia

```csharp
private string typeName = typeof(T).Name;   // campo de instancia, no static
```

Se recalcula en cada repositorio construido. Detalle menor de rendimiento.

---

## ⚠️ Lo que NO incluye

> ⚠️ **No hay unidad de trabajo ni transacciones.** Cada operación hace su propio `SaveChanges`. No hay `SaveChangesAsync` público, ni `BeginTransaction`, ni patrón *Unit of Work*.

> ⚠️ **No hay `Include` / carga de relaciones.** No puedes traer entidades relacionadas: `TryGetData` no acepta `Expression<Func<IQueryable<T>, IIncludableQueryable<T, object>>>`. Necesitarás `GetContext()` para consultas con `Include`.

> ⚠️ **No hay proyecciones.** Todo devuelve `T` completo; no hay `Select` a un DTO, así que siempre se traen todas las columnas.

> ⚠️ **No hay `Count`, `Any`, `Sum` ni agregados**. Solo se calcula el `TotalCount` internamente en la paginación.

> ⚠️ **No hay `ThenBy`**: la ordenación admite **un solo campo**.

> ⚠️ **No hay `ExecuteUpdate` / `ExecuteDelete`** (las operaciones por lotes de EF Core 7+). Los borrados por PK cargan la entidad primero.

> ⚠️ **No hay soporte para SQL crudo** (`FromSqlRaw`) ni procedimientos almacenados.

> ⚠️ **No hay control de concurrencia optimista**: `DbUpdateConcurrencyException` se convierte en un `Fail` genérico sin distinguirla de otros errores.

> ⚠️ **No hay `IQueryable` expuesto**: no puedes construir consultas compuestas sin bajar a `GetContext()`.

> ⚠️ **No hay tests unitarios en el propio proyecto.** Existen `MoralesLarios.OOFP.EFCore.Infrastructure.Tests` y `…Integration.Tests` como proyectos separados.

---

## Ejemplos prácticos

### El modelo de los ejemplos

```csharp
public class Vino
{
    public int      Id           { get; set; }
    public string   Nombre       { get; set; } = string.Empty;
    public string   Bodega       { get; set; } = string.Empty;
    public int      Anyo         { get; set; }
    public DateTime FechaEntrada { get; set; }
}

public class VinosCatasPuntuacion   // PK compuesta
{
    public int      IdVino     { get; set; }
    public int      IdCata     { get; set; }
    public string   IdUsuario  { get; set; } = string.Empty;
    public decimal  Puntuacion { get; set; }
}

public class JfCatasDbContext(DbContextOptions<JfCatasDbContext> options) : DbContext(options)
{
    public DbSet<Vino>                 Vinos        => Set<Vino>();
    public DbSet<VinosCatasPuntuacion> Puntuaciones => Set<VinosCatasPuntuacion>();

    protected override void OnModelCreating(ModelBuilder mb)
        => mb.Entity<VinosCatasPuntuacion>()
             .HasKey(x => new { x.IdVino, x.IdCata, x.IdUsuario });
}
```

### Ejemplo 1 — Configuración completa

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JfCatasDbContext>(o => o.UseSqlServer(cs));

// 🔑 Scoped, y ANTES de registrar los repos concretos
builder.Services.AddScopedOOFPRepos<Vino,                 JfCatasDbContext>();
builder.Services.AddScopedOOFPRepos<VinosCatasPuntuacion, JfCatasDbContext>();

// Repos concretos propios (opcional)
builder.Services.AddScoped<IVinosRepo, VinosRepo>();
```

### Ejemplo 2 — Repositorio propio con interfaz específica

```csharp
public interface IVinosRepo : IEFRepoPaginationFp<Vino> { }

public class VinosRepo(JfCatasDbContext db)
    : EFRepoPaginationFp<Vino, JfCatasDbContext>(db), IVinosRepo { }
```

> 💡 **Este es el patrón recomendado**: una interfaz por entidad, para que la inyección sea explícita y puedas añadir métodos propios más adelante.

### Ejemplo 3 — Consulta con PK simple y compuesta

```csharp
public class ConsultasService(IVinosRepo _vinos, IEFRepoReaderFp<VinosCatasPuntuacion> _puntos)
{
    public Task<MlResult<Vino>> BuscarVinoAsync(int id, CancellationToken ct)
        => _vinos.TryFindAsync($"No existe el vino con id {id}", ct, id);

    public Task<MlResult<VinosCatasPuntuacion>> BuscarPuntuacionAsync(
            int idVino, int idCata, string idUsuario, CancellationToken ct)
        => _puntos.TryFindAsync("Puntuación no registrada", ct, idVino, idCata, idUsuario);
}
```

### Ejemplo 4 — Modificación segura (el patrón correcto)

```csharp
public Task<MlResult<Vino>> RenombrarAsync(int id, string nombre, CancellationToken ct)
    => _vinos.TryFindAsync($"No existe el vino con id {id}", ct, id)
             .BindEnsureAsync(_ => !string.IsNullOrWhiteSpace(nombre), "El nombre no puede estar vacío")
             .MapAsync       (v => { v.Nombre = nombre; return v; })
             .BindAsync      (v => _vinos.TryUpdateAsync(v, ct));   // ✅ sin pk: evita el doble tracking
```

### Ejemplo 5 — Paginación correcta

```csharp
public Task<MlResult<PaginationResultInfo<Vino>>> PaginaAsync(
        int pagina, int tamanyo, string? bodega, CancellationToken ct)
    => _vinos.TryGetDataPaginationAsync(
                 paginationInfo: (pagina, tamanyo),                       // 🔑 tupla implícita
                 orderBy:        OrderBy.Descending,
                 orderByField:   x => x.FechaEntrada,                     // 🔑 SIEMPRE ordena
                 filter:         x => bodega == null || x.Bodega == bodega,  // 🔑 nunca null
                 ct:             ct);
```

> 💡 **Tres claves**: usa la versión **asíncrona** (el `TotalCount` síncrono ignora el filtro), pasa **siempre** `orderByField` (sin él el orden no está garantizado) y pasa **siempre** un `filter` no nulo (evita el `Where(null)` interno).

### Ejemplo 6 — Composición con validación y logging

```csharp
using MoralesLarios.OOFP.Extensions.Loggers;
using MoralesLarios.OOFP.Validation.FluentValidations;

public class AltaVinosService(IVinosRepo _repo, IValidator<Vino> _validator, ILogger<AltaVinosService> _logger)
{
    public Task<MlResult<Vino>> CrearAsync(Vino vino, CancellationToken ct)
        => _validator.MlValidate(vino)
                     .BindAsync(v => _repo.TryAddAsync(v, ct))
                     .LogMlResultInformationIfValidAsync(_logger, v => $"Vino {v.Id} creado")
                     .LogMlResultErrorIfFailAsync       (_logger, e => $"Error al crear vino: {e.ToErrorsDescription()}");
}
```

### Ejemplo 7 — Transacción explícita cuando necesitas atomicidad

```csharp
public async Task<MlResult<Vino>> CrearConPuntuacionAsync(
        Vino vino, VinosCatasPuntuacion punto, CancellationToken ct)
{
    await using var tx = _vinos.GetContext().Database.BeginTransaction();

    var resultado = await _vinos.TryAddAsync(vino, ct)
                                .BindAsync(v => _puntos.TryAddAsync(punto, ct)
                                                       .MapAsync(_ => v));

    if (resultado.IsValid) await tx.CommitAsync(ct);
    else                   await tx.RollbackAsync(ct);

    return resultado;
}
```

> ⚠️ **Requiere que ambos repositorios compartan el `DbContext`**, lo que depende del ciclo de vida y del localizador de servicios. Verifícalo en tu escenario.

### Ejemplo 8 — ❌ Qué no hacer / ✅ qué hacer

**Ciclo de vida en el registro:**

```csharp
// ❌ MAL: Singleton con un DbContext Scoped → captive dependency
services.AddSingletonOOFPRepos<Vino, JfCatasDbContext>();

// ✅ BIEN
services.AddScopedOOFPRepos<Vino, JfCatasDbContext>();
```

**Liberación del repositorio:**

```csharp
// ❌ MAL: disponer el repo rompe el DbContext del ámbito
using (var repoTemporal = provider.GetRequiredService<IVinosRepo>()) { /* … */ }

// ✅ BIEN: deja que el contenedor lo gestione
var repoInyectado = provider.GetRequiredService<IVinosRepo>();
```

**Paginación sin ordenación:**

```csharp
// ❌ MAL: paginar sin ordenar → páginas con contenido arbitrario
await repo.TryAllPaginationAsync((1, 20), ct: ct);

// ✅ BIEN: ordena y filtra explícitamente
await repo.TryGetDataPaginationAsync((1, 20), OrderBy.Ascending, x => x.Id, x => true, ct);
```

**Obtener el último registro:**

```csharp
// ❌ MAL: TryLast en una tabla grande → trae todas las filas a memoria
await repo.TryLastAsync(x => x.Bodega == "Rioja", ct);

// ✅ BIEN: ordenar descendente y tomar el primero (una sola fila)
await repo.TryGetDataPaginationAsync((1, 1), OrderBy.Descending, x => x.FechaEntrada,
                                     x => x.Bodega == "Rioja", ct)
          .MapAsync(p => p.Items.First());
```

**El `TotalCount` con filtro:**

```csharp
// ❌ MAL: la versión síncrona cuenta TODA la tabla, ignorando el filtro
var paginaMal = repo.TryGetDataPagination((1, 20), OrderBy.Ascending, x => x.Id,
                                          x => x.Anyo == 2020);

// ✅ BIEN: la asíncrona sí aplica el filtro al total
var paginaBien = await repo.TryGetDataPaginationAsync((1, 20), OrderBy.Ascending, x => x.Id,
                                                      x => x.Anyo == 2020, ct);
```

**Modificación de entidades:**

```csharp
// ❌ MAL: Update con pk sobre una instancia nueva → posible conflicto de tracking
await repo.TryUpdateAsync(new Vino { Id = 42, Nombre = "X" }, ct, 42);

// ✅ BIEN: recuperar, modificar y actualizar la misma instancia
await repo.TryFindAsync(ct, 42).MapAsync (v => { v.Nombre = "X"; return v; })
                               .BindAsync(v => repo.TryUpdateAsync(v, ct));
```

**Atomicidad entre operaciones:**

```csharp
// ❌ MAL: cada operación confirma por su cuenta
await repoVinos.TryAddAsync(vino, ct);      // ya confirmado en la BD
await repoCatas.TryAddAsync(cata, ct);      // si falla, el anterior NO se revierte

// ✅ BIEN: transacción explícita (ver ejemplo 7)
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| CRUD completo de una entidad | `IEFRepoFp<T>` |
| CRUD + paginación | `IEFRepoPaginationFp<T>` ✅ el más completo |
| Solo consultas | `IEFRepoReaderFp<T>` |
| Solo consultas + paginación | `IEFRepoReaderPaginationFp<T>` |
| Buscar por PK con mensaje propio | `TryFindAsync("mensaje", ct, pk)` |
| Buscar por condición | `TryFirstAsync(x => …, ct)` |
| Listar todo | `TryAllAsync(ct)` (usa `AsNoTracking`) |
| Listar filtrado | `TryGetDataAsync(x => …, ct)` |
| Página de resultados | `TryGetDataPaginationAsync(…)` ✅ con `orderByField` y `filter` |
| Insertar varios de golpe | `TryAddRangeAsync(items, ct)` |
| Borrar comprobando existencia | `TryRemoveAsync("mensaje", ct, pk)` |
| El último registro | ❌ **No uses `TryLast`**: ordena descendente y toma el primero |
| Atomicidad entre operaciones | Transacción manual con `GetContext().Database.BeginTransaction()` |
| `Include`, proyecciones, agregados | ❌ No disponible: baja a `GetContext()` |
| Registrar en DI | `AddScopedOOFPRepos<T, TContext>()` ✅ |

---

## Mejores prácticas

1. **Registra siempre con `AddScopedOOFPRepos`**, que coincide con el ciclo de vida de `AddDbContext`. Evita `Singleton`.
2. **Llama a `Add…OOFPRepos` al principio** de la configuración de servicios, antes de cualquier registro que dependa de repositorios.
3. **Nunca llames a `Dispose()`** sobre un repositorio ni lo envuelvas en `using`: liberaría el `DbContext` compartido.
4. **Usa siempre las versiones asíncronas** en aplicaciones web, y en paginación además por el problema del `TotalCount`.
5. **Al paginar, pasa siempre `orderByField` y un `filter` no nulo.**
6. **Evita `TryLast`/`TryLastAsync`**: materializa toda la consulta. Ordena descendente y toma el primero.
7. **Para modificar, recupera y actualiza la misma instancia** (`TryFind` → modificar → `TryUpdate(v)` sin `pk`).
8. **Personaliza los `MlErrorsDetails`** si el mensaje va a llegar al usuario: los de serie están en inglés e incluyen el nombre del tipo.
9. **Segrega las interfaces**: inyecta `IEFRepoReaderFp<T>` donde solo consultes.
10. **Declara una interfaz por entidad** (`IVinosRepo : IEFRepoPaginationFp<Vino>`) para poder ampliarla sin romper llamadas.
11. **Si necesitas atomicidad, usa una transacción explícita** con `GetContext().Database.BeginTransaction()`.
12. **Para `Include`, proyecciones o agregados, baja a `GetContext()`**: la API no los cubre y forzarla es peor que usar EF Core directamente.
13. **Combina con [`Validation`](../MoralesLarios.OOFP.Validation/README.md)** para validar la entidad antes de persistirla, y con [`Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) para trazar.
14. **No des por hecho que dos operaciones comparten el *change tracker***: el localizador de servicios puede entregar contextos distintos.

---

## Resumen

- Repositorios genéricos sobre **EF Core 8** que devuelven `MlResult<T>`: el "no encontrado" es un `Fail` con mensaje personalizable, no una excepción, y los argumentos se validan con `EnsureFp` antes de tocar la base de datos.
- **Dos capas paralelas**: la funcional (`*Fp`, pública, `MlResult`) envuelve a la OOP (`internal`, devuelve `T`). **Usa solo la funcional**; la OOP es inaccesible desde fuera.
- **Ocho repositorios componibles** por entidad, desde `IEFRepoAdderFp<T>` hasta `IEFRepoPaginationFp<T>`. Todos los métodos son `virtual`: puedes heredar y sobrescribir.
- Registro: `AddScopedOOFPRepos<T, TContext>()` ✅ registra las 16 interfaces de una entidad. ⚠️ **Evita la variante `Singleton`** (captive dependency con el `DbContext`).
- ❗ **El punto más delicado**: los repos resuelven sus colaboradores con un **localizador de servicios estático que llama a `BuildServiceProvider()` en cada acceso**. Implica coste, providers sin liberar y que el `DbContext` interno pueda no ser el del ámbito actual.
- ❗ **`Dispose` libera el `DbContext` inyectado**: nunca dispongas un repositorio a mano.
- ❗ **Sin unidad de trabajo**: cada operación hace su propio `SaveChanges`. Para atomicidad, transacción explícita con `GetContext().Database.BeginTransaction()`.
- ⚠️ **Tres trampas concretas**: `TryLast` materializa toda la consulta; el `TotalCount` de la paginación **síncrona** ignora el filtro; y `TryUpdate(item, pk)` puede provocar conflicto de seguimiento.
- ⚠️ **No cubre** `Include`, proyecciones, agregados, `ThenBy`, SQL crudo ni operaciones por lotes: para eso, `GetContext()`.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.FOOP/README.md)
- [Introducción general al núcleo funcional](../MoralesLarios.FOOP/__Doc/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.Internals`](../MoralesLarios.OOFP.Internals/README.md) — 🔑 `PaginationInfo` y `PaginationResultInfo<T>`
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — `IntNotNegative` y las conversiones de paginación
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validar la entidad antes de persistirla
- [`MoralesLarios.OOFP.Validation.FluentValidations`](../MoralesLarios.OOFP.Validation.FluentValidations/README.md) — validación con FluentValidation
- [`MoralesLarios.OOFP.Extensions.Loggers`](../MoralesLarios.OOFP.Extensions.Loggers/README.md) — trazar los fallos de acceso a datos
- [`MoralesLarios.OOFP.WebServices`](../MoralesLarios.OOFP.WebServices/README.md) — capa de servicio sobre estos repositorios
- [`MoralesLarios.OOFP.WebApi`](../MoralesLarios.OOFP.WebApi/README.md) — convertir el `MlResult` en respuesta HTTP

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../MoralesLarios.FOOP/__Doc/Types/MlResult.md)
- [`MlErrorsDetails` — mensajes y detalles del error](../MoralesLarios.FOOP/__Doc/Types/MlResultErrors.md)
- [`TryMap` y `TryBind` — capturar excepciones en el raíl](../MoralesLarios.FOOP/__Doc/Map/1_Map.md) 🔑 la base del proyecto
- [`EnsureFp` — validaciones de guarda](../MoralesLarios.FOOP/__Doc/EnsureFp/EnsureFp.md) 🔑 `NotNull`, `NotEmpty`
- [`Bind` — encadenar operaciones que pueden fallar](../MoralesLarios.FOOP/__Doc/Bind/3_Bind.md)
- [`Map` — transformación con cortocircuito](../MoralesLarios.FOOP/__Doc/Map/1_Map.md)
- [`Match` — resolver el `MlResult` en un valor](../MoralesLarios.FOOP/__Doc/Match/1_Match.md)
- [Métodos asíncronos en el raíl](../MoralesLarios.FOOP/__Doc/1_Intro.md#sufijos-de-asincronía)
