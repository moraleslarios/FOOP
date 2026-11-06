# MoralesLarios.OOFP.EntityFrameworkCore — Repositorios sobre EF Core en el raíl funcional

Capa de repositorios genéricos sobre **Entity Framework Core 8** que convierte el acceso a datos en operaciones que devuelven [`MlResult<T>`](../docs/core-types/MlResult.md): con captura de excepciones y el "no encontrado" tratado como un `Fail` con mensaje propio.

Expone nueve interfaces funcionales por entidad, todas sobre el mismo `DbContext` del ámbito. `IEfOrderedReadRepository<T>` añade la consulta limitada de último elemento.

---

## Índice

1. [¿Qué problema resuelve?](#qué-problema-resuelve)
2. [Instalación y dependencias](#instalación-y-dependencias)
3. [Estructura del proyecto](#estructura-del-proyecto)
4. [Composición de los repositorios](#composición-de-los-repositorios)
5. [Jerarquía de interfaces](#jerarquía-de-interfaces)
6. [Registro en el contenedor](#registro-en-el-contenedor)
7. [`IEfReadRepository<T>` — lectura](#iefreadrepositoryt--lectura)
8. [`IEfCreateRepository<T>` — inserción](#iefcreaterepositoryt--inserción)
9. [`IEfUpdateRepository<T>` — modificación](#iefupdaterepositoryt--modificación)
10. [`IEfDeleteRepository<T>` — borrado](#iefdeleterepositoryt--borrado)
11. [`IEfWriteRepository<T>` y `IEfRepository<T>` — agregados](#iefwriterepositoryt-y-iefrepositoryt--agregados)
12. [Paginación: `IEfPaginator<T>`](#paginación-iefpaginatort)
13. [Mensajes de error personalizados](#mensajes-de-error-personalizados)
14. [Helpers y `SortDirection`](#helpers-y-sortdirection)
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

✅ **Con `IEfRepository<T>`:**

```csharp
public Task<MlResult<Vino>> RenombrarAsync(int id, string nombre, CancellationToken ct)
    => _repo.TryFindAsync($"El vino {id} no existe", ct, id)
            .MapAsync (v => { v.Nombre = nombre; return v; })
            .BindAsync(v => _repo.TryUpdateAsync(v, ct, v.Id));
```

| Problema | Cómo lo resuelve |
|---|---|
| `Find` devuelve `null` | `FailIfNull` lo convierte en `Fail` con mensaje |
| Cualquier operación lanza | Todo va dentro de `TryMap` / `TryBind` |
| Argumentos nulos o vacíos | `EnsureFp.NotNull` / `EnsureFp.NotEmpty` antes de tocar la BD |
| No se compone | El `MlResult` encadena con `Map`, `Bind`, `Match`… |

---

## Instalación y dependencias

| Dependencia | Versión | Para qué |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.3 | `DbContext`, `DbSet<T>`, `ToListAsync` |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 8.0.0 | `IServiceCollection` y resolución de servicios |
| [`MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md) | — | `MlResult<T>`, `EnsureFp`, `TryMap`, `FailIfNull` |
| [`MoralesLarios.OOFP.Shared`](../MoralesLarios.OOFP.Shared/README.md) | — | `PageRequest`, `PagedResult<T>` |
| [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) | — | `IntNonNegative` (conversiones de paginación) |

Destino: **`net8.0`**. Versión del paquete: **2.0.0**.

`Microsoft.Extensions.DependencyInjection.Abstractions` se declara directamente porque `AddOofpRepositories` expone `IServiceCollection` en su API pública.

```csharp
using MoralesLarios.OOFP.EntityFrameworkCore;              // AddOofpRepositories
using MoralesLarios.OOFP.EntityFrameworkCore.Repositories;        // IEfRepository<T>, EfCrudRepository<T, TContext>
using MoralesLarios.OOFP.EntityFrameworkCore.Enums;        // SortDirection
using MoralesLarios.OOFP.Shared.Pagination;      // PageRequest, PagedResult<T>
```

---

## Estructura del proyecto

```
MoralesLarios.OOFP.EntityFrameworkCore/
├── Repositories/                   ← API funcional pública (devuelve MlResult)
│   ├── EfRepository.cs             → base: GetContext()
│   ├── IEfReadRepository.cs / EfReadRepository.cs
│   ├── IEfCreateRepository.cs / EfCreateRepository.cs
│   ├── IEfUpdateRepository.cs / EfUpdateRepository.cs
│   ├── IEfDeleteRepository.cs / EfDeleteRepository.cs
│   ├── IEfWriteRepository.cs / EfWriteRepository.cs
│   ├── IEfRepository.cs / EfCrudRepository.cs
│   ├── IEfPaginator.cs
│   ├── IEfPagedReadRepository.cs / EfPagedReadRepository.cs
│   └── IEfPagedRepository.cs / EfPagedRepository.cs
├── Helpers/
│   └── Extensions.cs                  → FormatCompositeKey, PrivateOrderBy
├── Enums/SortDirection.cs                    → Ascending, Descending
├── RegisterServices.cs                 → AddOofpRepositories (siempre Scoped)
└── GlobalUsings.cs
```

---

## Composición de los repositorios

Los agregados `EfCrudRepository<T, TContext>` y `EfPagedRepository<T, TContext>` crean sus colaboradores mediante factorías `protected virtual`. Las factorías reciben el mismo `TContext` que el agregado, por lo que todas las operaciones del ámbito comparten el mismo `ChangeTracker` y unidad de trabajo. Las nueve interfaces públicas devuelven `MlResult` o `Task<MlResult<…>>`.

---

## Jerarquía de interfaces

```
IEfReadRepository<T>       lectura: Find, First, Last, GetData, All
IEfCreateRepository<T>        Add, AddRange
IEfUpdateRepository<T>      Update, UpdateRange
IEfDeleteRepository<T>      Remove, RemoveRange
IEfPaginator<T>        AllPagination, GetDataPagination

IEfWriteRepository<T>            : Adder + Updater + Deleter
IEfRepository<T>                  : Reader + Writer                    ← 🟢 el más usado
IEfPagedReadRepository<T>  : Reader + Paginator
IEfPagedRepository<T>        : Repo + Paginator                   ← 🟢 el más completo
```

| Necesito… | Interfaz |
|---|---|
| Solo leer | `IEfReadRepository<T>` |
| Solo insertar | `IEfCreateRepository<T>` |
| Solo modificar | `IEfUpdateRepository<T>` |
| Solo borrar | `IEfDeleteRepository<T>` |
| Escribir (las tres) | `IEfWriteRepository<T>` |
| CRUD completo | `IEfRepository<T>` ✅ |
| Leer + paginar | `IEfPagedReadRepository<T>` |
| CRUD + paginar | `IEfPagedRepository<T>` ✅ |

> 💡 **Segrega por intención**: si un servicio solo consulta, inyecta `IEfReadRepository<T>`. El compilador impedirá que escriba por accidente.

---

## Registro en el contenedor

Un solo método registra las **nueve interfaces funcionales** para una entidad y su contexto:

```csharp
builder.Services.AddDbContext<TestDbContext>(o => o.UseSqlServer(cs));

builder.Services.AddOofpRepositories<Vino, TestDbContext>();
builder.Services.AddOofpRepositories<Cata, TestDbContext>();   // una llamada por entidad
```

**Un solo método y un solo ciclo de vida.** Todos los repositorios se registran como `Scoped`:

| Método | Ciclo de vida |
|---|---|
| `AddOofpRepositories<T, TContext>()` | `Scoped` |

> ✅ **`Scoped` es el único ciclo de vida admitido.** Cada repositorio y agregado recibe por constructor el `DbContext` registrado para el ámbito. Las factorías de los agregados pasan esa misma instancia a sus colaboradores; no se construyen proveedores secundarios ni se usa un localizador estático.

> 💡 **Varios `DbContext`:** inyecta `IMlUnitOfWork<TContext>` para seleccionar explícitamente la unidad de trabajo del contexto. La interfaz no genérica `IMlUnitOfWork` es una alias de conveniencia para aplicaciones con un único contexto registrado.

---

## `IEfReadRepository<T>` — lectura

```csharp
MlResult<T>       TryFind      (params object[] keyValues);
MlResult<T>       TryFind      (MlErrorDetails notFoundErrorDetails, params object[] keyValues);
Task<MlResult<T>> TryFindAsync (CancellationToken token = default, params object[] keyValues);
Task<MlResult<T>> TryFindAsync (MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);

MlResult<T>       TryFirst     (Expression<Func<T, bool>> filter);
MlResult<T>       TryFirst     (Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<T>       TryGetLast      (Expression<Func<T, bool>> filter);
MlResult<T>       TryGetLast      (Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails);
Task<MlResult<T>> TryGetLastAsync (Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryGetLastAsync (Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<IEnumerable<T>>       TryGetData     (Expression<Func<T, bool>> filter);
Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAll         ();
Task<MlResult<IEnumerable<T>>> TryAllAsync    (CancellationToken token = default);
```

| Método | Devuelve `Fail` cuando | Mensaje por defecto |
|---|---|---|
| `TryFind` | `pk` nulo, vacío, o no existe la fila | `"{valores} values not found data in {Entidad}"` |
| `TryFirst` | `filter` nulo o sin coincidencias | `"The query did not return any elements"` |
| `TryGetLast` | `filter` nulo o sin coincidencias | `"The query did not return any elements"` |
| `TryGetData` | `filter` nulo, clave de entrada inválida aguas arriba o error de consulta | `MlResult.Fail`; la consulta síncrona se materializa dentro del método |
| `TryAll` | Error al ejecutar la consulta | `MlResult.Fail`; la consulta síncrona se materializa dentro del método |

Implementación de `TryFind` (muestra el patrón completo):

```csharp
public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    => EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
               .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
               .TryMap (x => GetContext().Set<T>().Find(keyValues)!)
               .TryBind(x => x.FailIfNull(notFoundErrorDetails));   // 🔑 null → Fail
```

**Claves para PK compuestas**: el `params object[]` acepta varias claves en el orden declarado en `HasKey`.

```csharp
// PK simple
await repo.TryFindAsync(ct, 42);

// PK compuesta: HasKey(x => new { x.IdVino, x.IdCata, x.IdUsuario })
await repo.TryFindAsync(ct, idVino, idCata, idUsuario);
```

> 💡 **No hay `TryFirstOrDefault` ni `TryLastOrDefault`, y no hacen falta**: `TryFirst`/`TryGetLast` ya devuelven `Fail` en lugar de lanzar, así que cubren el caso seguro.

> 💡 **`TryAll` y `TryGetData` usan `AsNoTracking()` y materializan sus resultados**: son de solo lectura, no cargan el *change tracker* y los errores de ejecución quedan dentro de `MlResult`. La variante síncrona ejecuta la consulta antes de retornar; la variante async usa `ToListAsync(token)`. Para conjuntos grandes, prefiere las operaciones paginadas. `TryFind`, en cambio, **sí hace seguimiento** (usa `Find`), lo que es correcto porque suele preceder a una modificación.

---

## `IEfCreateRepository<T>` — inserción

```csharp
MlResult<T>                    TryAdd          (T item);
Task<MlResult<T>>              TryAddAsync     (T item, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAddRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

Valida que el argumento no sea nulo y añade la entidad al `ChangeTracker`. **No confirma la base de datos**: después de una o varias escrituras, usa `UnitOfWork.TryCommit[Async]` o `TryExecuteInTransaction[Async]`.

> 💡 `TryAddRange` añade varias entidades al mismo contexto; `UnitOfWork.TryCommit[Async]` las confirma en una sola llamada a `SaveChanges`.

---

## `IEfUpdateRepository<T>` — modificación

```csharp
MlResult<T>                    TryUpdate          (T item);
MlResult<T>                    TryUpdate          (T item, params object[] keyValues);
MlResult<T>                    TryUpdate          (T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues);
Task<MlResult<T>>              TryUpdateAsync     (T item, CancellationToken token = default);
Task<MlResult<T>>              TryUpdateAsync     (T item, CancellationToken token = default, params object[] keyValues);
Task<MlResult<T>>              TryUpdateAsync     (T item, MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
MlResult<IEnumerable<T>>       TryUpdateRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

**Dos comportamientos distintos según pases `keyValues` o no:**
```csharp
public MlResult<T> TryUpdate(T item, MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    => EnsureFp.NotNull(item, "The entity item to update cannot be null")
       .Bind(_ => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
       .TryMap(_ => GetContext().Set<T>().Find(keyValues))
       .TryBind(found => found.FailIfNull(notFoundErrorDetails))
       .TryMap(found =>
       {
           if (!ReferenceEquals(found, item))
               GetContext().Entry(found).State = EntityState.Detached;
           GetContext().Update(item);
           return item;
       });
```

> ✅ **Usa la sobrecarga con `keyValues`** si quieres un `Fail` claro cuando la fila no existe. Los errores de persistencia se informan al llamar a `UnitOfWork.TryCommit[Async]`.

> ✅ **La entidad encontrada sirve para comprobar que la fila existe.** Si `item` es otra instancia, el repositorio desvincula la instancia encontrada antes de actualizar `item`, evitando el conflicto de seguimiento de EF Core.

---

## `IEfDeleteRepository<T>` — borrado

```csharp
MlResult<T>                    TryRemove          (T item);
MlResult<T>                    TryRemove          (params object[] keyValues);
MlResult<T>                    TryRemove          (MlErrorDetails notFoundErrorDetails, params object[] keyValues);
Task<MlResult<T>>              TryRemoveAsync     (T item, CancellationToken token = default);
Task<MlResult<T>>              TryRemoveAsync     (CancellationToken token = default, params object[] keyValues);
Task<MlResult<T>>              TryRemoveAsync     (MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues);
MlResult<IEnumerable<T>>       TryRemoveRange     (IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

> ✅ **Las sobrecargas con clave** hacen `Find` y borran la entidad encontrada, no una instancia externa.

```csharp
.TryMap (pk => GetContext().Set<T>().Find(keyValues))
.TryBind(bdItem => bdItem.FailIfNull(notFoundErrorDetails))
.TryMap (bdItem =>
{
    GetContext().Set<T>().Remove(bdItem);
    return bdItem;
});
```

> ⚠️ **`TryRemove(params object[] keyValues)` puede confundirse con `TryRemove(T item)`** si tu entidad es un `object[]`, caso raro pero posible. Y ojo: `TryRemove(MlErrorDetails, params object[])` frente a `TryRemove(params object[])` se resuelven por el primer argumento, así que un `string` como primera clave podría convertirse implícitamente en `MlErrorDetails`. **Con PK de tipo `string`, usa siempre la sobrecarga explícita con mensaje.**

---

## `IEfWriteRepository<T>` y `IEfRepository<T>` — agregados

```csharp
public interface IEfWriteRepository<T> : IEfCreateRepository<T>, IEfUpdateRepository<T>, IEfDeleteRepository<T> { }

public interface IEfRepository<T> : IEfReadRepository<T>, IEfWriteRepository<T> { }
```

No añaden métodos: solo combinan. Las clases `EfWriteRepository` y `EfCrudRepository` son **delegadores puros**, resuelven los repos especializados y reenvían cada llamada.

Las interfaces agregadas solo expresan composición; sus implementaciones delegan en colaboradores creados con factorías protegidas y comparten el mismo `DbContext`.

---

## Paginación: `IEfPaginator<T>`

```csharp
MlResult<PagedResult<T>>       TryAllPagination         (PageRequest paginationInfo,
                                                                  SortDirection orderBy = SortDirection.Ascending,
                                                                  Expression<Func<T, object>>? orderByField = null);
Task<MlResult<PagedResult<T>>> TryAllPaginationAsync    (…, CancellationToken ct = default!);
MlResult<PagedResult<T>>       TryGetDataPagination     (…, Expression<Func<T, bool>>? filter = null);
Task<MlResult<PagedResult<T>>> TryGetDataPaginationAsync(…, CancellationToken ct = default!);
```

### Los tipos de paginación

```csharp
public record PageRequest(int PageNumber, int PageSize)
{
    private const int MaxPageSize = 1000;

    public int PageNumber { get; init; } = Math.Max  (1, PageNumber);       // 🔑 mínimo 1
    public int PageSize   { get; init; } = Math.Clamp(PageSize, 1, 1000);   // 🔑 entre 1 y 1000
}

public record PagedResult<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount)
    : PageRequest(PageNumber, PageSize);
```

> 💡 **`PageRequest` se autocorrige**: `PageNumber = 0` pasa a `1`, `PageSize = 5000` se recorta a `1000`. **Nunca falla por valores fuera de rango**, los normaliza en silencio.

> 💡 **Conversión implícita desde tupla**, muy cómoda:
> ```csharp
> await repo.TryAllPaginationAsync((2, 25), SortDirection.Descending, x => x.FechaEntrada, ct);
> ```

### Ordenación

```csharp
public static IQueryable<T> PrivateOrderBy<T>(this IQueryable<T> source,
                                              SortDirection orderBy = SortDirection.Ascending,
                                              Expression<Func<T, object>> orderByField = null!)
    => orderByField is null ? source
                            : orderBy == SortDirection.Ascending ? source.OrderBy          (orderByField)
                                                           : source.OrderByDescending(orderByField);
```

La paginación ordena por la clave primaria EF cuando no se proporciona selector. Si se proporciona, agrega todas las propiedades de la clave primaria como desempate, conservando el sentido de `SortDirection`. Esto estabiliza páginas consecutivas con claves simples o compuestas. Las entidades sin clave primaria EF devuelven un resultado fallido: no se puede prometer un orden total. La estabilización puede cambiar qué filas aparecen en páginas existentes; los selectores que ya eran únicos mantienen su orden.

> 💡 **El campo es `Expression<Func<T, object>>`**, así que al ordenar por un tipo valor (`int`, `DateTime`) se genera un *boxing* en la expresión. EF Core lo traduce correctamente, pero es la razón de que no puedas encadenar varios criterios (no hay `ThenBy`).

---

## Mensajes de error personalizados

Todos los métodos de "búsqueda" tienen una sobrecarga con `MlErrorDetails`, que acepta conversión implícita desde `string`:

```csharp
// Mensaje por defecto: "42 values not found data in Vino"
await repo.TryFindAsync(ct, 42);

// Mensaje propio (string → MlErrorDetails implícito)
await repo.TryFindAsync($"No existe el vino con id {42}", ct, 42);

// Con detalles adicionales
await repo.TryFindAsync(MlErrorDetails.FromErrorMessageWithValue(42, "Vino inexistente"), ct, 42);
```

Los mensajes por defecto usan `FormatCompositeKey`:

```csharp
new object[] { 10 }.FormatCompositeKey();        // "10"
new object[] { 10, "A" }.FormatCompositeKey();   // "(10, A)"
```

> 💡 **Personaliza el mensaje si va a llegar al usuario final**: los mensajes por defecto están en inglés e incluyen el nombre del tipo de la entidad, lo cual **filtra detalles internos** en una API pública.

---

## Helpers y `SortDirection`

```csharp
public enum SortDirection { Ascending, Descending }
```

| Helper | Firma | Uso |
|---|---|---|
| `FormatCompositeKey` | `this object[] → string` | Formatea las claves para los mensajes |
| `PrivateOrderBy` | `this IQueryable<T> → IQueryable<T>` | Ordenación condicional |

`Helpers/Extensions.cs` contiene los auxiliares de formato de claves y ordenación usados por los repositorios.

---

## ⚠️ Particularidades reales del código fuente

### 1. ✅ El `DbContext` se comparte dentro del ámbito

`AddOofpRepositories<T, TContext>()` registra nueve interfaces funcionales como `Scoped`. Los agregados crean sus colaboradores con el mismo contexto que reciben por constructor. Consulta [Registro en el contenedor](#registro-en-el-contenedor).

### 2. ✅ El repositorio **no** es dueño del `DbContext` (corregido en la 2.0.0)

En la 1.x, `EfRepository` implementaba `IDisposable` y su `Dispose()` liberaba el `DbContext` inyectado, que es propiedad del contenedor:

```csharp
// 1.x — ⚠️❗ liberaba un contexto que no le pertenecía
public class EfRepository(DbContext dbContext) : IGetContextable, IDisposable
{
    public DbContext GetContext() => dbContext;

    public virtual void Dispose() => GetContext()?.Dispose();
}
```

Desde la 2.0.0 el repositorio **no implementa `IDisposable`**:

```csharp
// 2.0.0 — el contenedor libera el DbContext al cerrar el ámbito
public class EfRepository(DbContext dbContext) : IGetContextable
{
    public DbContext GetContext() => dbContext;
}
```

| Escenario | Qué ocurre |
|---|---|
| Repositorio `Scoped`, contexto `Scoped` | ✅ Repositorio y contexto viven el **mismo** ámbito: todos los repositorios de la petición comparten contexto, y el contenedor libera el contexto al cerrar el ámbito |
| `using (var repo = …)` explícito | ✅ Ya **no compila**: el repositorio no es `IDisposable` |

> ✅ **Esto es lo que hace seguro el ciclo de vida `Scoped`**: el repositorio no es el propietario del `DbContext`, así que no debe liberarlo. Al no ser `IDisposable`, el contenedor no invoca ningún `Dispose()` del repositorio que destruyese el contexto compartido del ámbito; el contexto lo libera quien lo registró (`AddDbContext`) cuando el ámbito termina.

### 3. Unidad de trabajo y transacciones

```csharp
// Registrar cambios y confirmarlos explícitamente.
await repo.TryAddAsync(vino, ct);
var commit = await repo.UnitOfWork.TryCommitAsync(ct);
```

Los 16 métodos síncronos y asíncronos de `TryAdd`, `TryUpdate` y `TryRemove` preparan cambios en el contexto; no ejecutan `SaveChanges`. El agregado `IEfRepository<T>` expone `IMlUnitOfWork` mediante `UnitOfWork`. Si inyectas una interfaz especializada, puedes resolver `IMlUnitOfWork<TContext>` del mismo ámbito.

```csharp
// Un fallo en cualquiera de las escrituras revierte ambas.
var resultado = await repoVinos.UnitOfWork.TryExecuteInTransactionAsync(async token =>
{
    var vinoResult = await repoVinos.TryAddAsync(vino, token);
    if (vinoResult.IsFailure)
        return vinoResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

    var cataResult = await repoCatas.TryAddAsync(cata, token);
    if (cataResult.IsFailure)
        return cataResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

    return vinoResult;
}, ct);
```

`TryExecuteInTransactionAsync` confirma una vez si el callback devuelve un resultado válido y revierte
si devuelve un fallo, lanza una excepción o se cancela. Las transacciones existentes se reutilizan y
no se confirman desde el callback anidado; el propietario de la transacción externa decide cómo
finalizarla.

El aviso de cambios pendientes al cerrar el ámbito está desactivado por defecto. Para localizar
commits olvidados durante el desarrollo, actívalo y configura el nivel `Warning` en el proveedor de
logs:

```csharp
services.Configure<MlUnitOfWorkDiagnostics>(options =>
    options.WarnOnUncommittedChanges = true);
```

### 4. ❗ `TryGetLast` trae **todos** los registros filtrados a memoria

```csharp
public T LastOrDefault(Expression<Func<T, bool>> filter)
    => GetContext().Set<T>()
                        .AsNoTracking()
                        .Where(filter)
                        .AsEnumerable()      // ⚠️❗ ejecuta el SELECT completo
                        .LastOrDefault()!;
```

**`AsEnumerable()` materializa toda la consulta** y luego toma el último elemento en memoria. En una tabla con un millón de filas coincidentes, se transfieren **todas**.

> ❗ **Evita `TryGetLast`/`TryGetLastAsync` en tablas grandes.** Esos métodos preservan su comportamiento previo y materializan todas las coincidencias. Usa la interfaz aditiva `IEfOrderedReadRepository<T>`: ordena en el servidor, desempata por clave primaria y limita la consulta a una fila.
> ```csharp
> var orderedRepo = provider.GetRequiredService<IEfOrderedReadRepository<Vino>>();
> var ultimo = await orderedRepo.TryGetLastByAsync(
>     x => x.Bodega == "X", x => x.FechaEntrada, SortDirection.Ascending, ct);
> ```
> “Último” es el máximo del selector con `Ascending` y el mínimo con `Descending`; la clave primaria deshace empates en el mismo sentido. La API requiere una clave primaria EF.

### 5. ✅ La paginación ordena antes de aplicar `Skip` y `Take`

```csharp
GetContext().Set<T>()
    .AsNoTracking()
    .Where(effectiveFilter)
    .OrderByStableForPagination(GetContext(), orderBy, orderByField)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

La consulta aplica primero el filtro y el orden total, y después pagina. Si `filter` es `null`, se trata como `x => true`; `orderByField` también es opcional y la clave primaria define el orden por defecto. Un selector no único se desempata por todas las propiedades de la clave EF.

### 6. ✅ `TotalCount` respeta el filtro en ambas variantes

```csharp
var effectiveFilter = filter ?? (x => true);
var count = GetContext().Set<T>().Where(effectiveFilter).Count();
```

| Versión | `TotalCount` |
|---|---|
| `TryGetDataPagination` (sync) | ✅ Total de filas que cumplen el filtro |
| `TryGetDataPaginationAsync` | ✅ Total de filas que cumplen el filtro |

El filtro nulo se normaliza a `x => true`; `TryAllPagination[Async]` incluye todas las filas.

### 7. ✅ `TryUpdate(item, keyValues)` evita el conflicto de seguimiento

Al pasar una clave, el repositorio busca la entidad. Si la instancia encontrada es distinta de `item`, la desvincula antes de llamar a `Update(item)`.

```csharp
// La instancia hallada se desvincula antes de actualizar esta entidad.
var vinoModificado = new Vino { Id = 42, Nombre = "Nuevo" };
await repo.TryUpdateAsync(vinoModificado, ct, 42);   // TryFind trae la original, luego Update(la nueva)
```

> ✅ También puedes recuperar, modificar y actualizar **la misma instancia**:
> ```csharp
> await repo.TryFindAsync($"El vino {id} no existe", ct, id)
>           .MapAsync (v => { v.Nombre = nombre; return v; })
>           .BindAsync(v => repo.TryUpdateAsync(v, ct));      // ⬅ sin pk: no repite el Find
> ```

### 8. ✅ `TryRemoveRangeAsync` usa guardas y persistencia asíncronas

```csharp
var result = await EnsureFp.NotNullAsync(items, "The entity items to remove cannot be null")
    .TryMapAsync(async _ =>
    {
        GetContext().Set<T>().RemoveRange(items);
        await GetContext().SaveChangesAsync(token);
        return items;
    });
```

La operación valida `items` de forma asíncrona y espera a `SaveChangesAsync`.

### 9. `EfPagedReadRepository` consulta el contexto directamente

El repositorio construye la consulta directamente sobre el `DbContext` recibido:

```csharp
.TryMap(filter => GetContext().Set<T>()
                              .AsNoTracking()
                              …)
```

La consulta opera sobre el `DbContext` y aplica el filtro, la ordenación y la paginación en ese orden.

### 10. ⚠️ `IntNonNegative` en `PageRequest` con anotaciones incorrectas

```csharp
public record PageRequest([property: Range(0, int.MinValue)] int PageNumber,   // ⚠️ Range(0, -2147483648)
                             [property: Range(0, int.MinValue)] int PageSize)
```

El máximo del rango es **`int.MinValue`**, menor que el mínimo. La anotación es inválida, pero **no tiene efecto** porque la normalización real la hacen `Math.Max` y `Math.Clamp`, y nadie valida ese atributo. (Reside en [`Shared`](../MoralesLarios.OOFP.Shared/README.md).)

### 11. `IGetContextable` expone el contexto del repositorio

`IGetContextable` está en `MoralesLarios.OOFP.EntityFrameworkCore.Repositories` y permite obtener el `DbContext` inyectado. Úsalo para consultas no cubiertas por las interfaces, como `Include`, proyecciones o transacciones; no dispongas el contexto desde el repositorio.

### 12. `typeName` se almacena como campo estático de solo lectura

```csharp
private static readonly string typeName = typeof(T).Name;
```

El nombre se calcula una vez por tipo de entidad.

---

## ⚠️ Lo que NO incluye

> ✅ La API de unidad de trabajo está disponible como `repo.UnitOfWork`. `TryCommit[Async]` confirma
> cambios de una escritura directa; `TryExecuteInTransaction[Async]` coordina commit y rollback.

> ⚠️ **No hay `Include` / carga de relaciones.** No puedes traer entidades relacionadas: `TryGetData` no acepta `Expression<Func<IQueryable<T>, IIncludableQueryable<T, object>>>`. Necesitarás `GetContext()` para consultas con `Include`.

> ⚠️ **No hay proyecciones.** Todo devuelve `T` completo; no hay `Select` a un DTO, así que siempre se traen todas las columnas.

> ⚠️ **No hay `Count`, `Any`, `Sum` ni agregados**. Solo se calcula el `TotalCount` internamente en la paginación.

> ⚠️ **No hay `ThenBy`**: la ordenación admite **un solo campo**.

> ⚠️ **No hay `ExecuteUpdate` / `ExecuteDelete`** (las operaciones por lotes de EF Core 7+). Los borrados por PK cargan la entidad primero.

> ⚠️ **No hay soporte para SQL crudo** (`FromSqlRaw`) ni procedimientos almacenados.

> ⚠️ **No hay control de concurrencia optimista**: `DbUpdateConcurrencyException` se convierte en un `Fail` genérico sin distinguirla de otros errores.

> ⚠️ **No hay `IQueryable` expuesto**: no puedes construir consultas compuestas sin bajar a `GetContext()`.

> ⚠️ **No hay tests unitarios en el propio proyecto.** Existen `MoralesLarios.OOFP.EntityFrameworkCore.TestSupport` y `…Integration.Tests` como proyectos separados.

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

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
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

builder.Services.AddDbContext<TestDbContext>(o => o.UseSqlServer(cs));

// 🔑 Scoped, y ANTES de registrar los repos concretos
builder.Services.AddOofpRepositories<Vino,                 TestDbContext>();
builder.Services.AddOofpRepositories<VinosCatasPuntuacion, TestDbContext>();

// Repos concretos propios (opcional)
builder.Services.AddScoped<IVinosRepo, VinosRepo>();
```

### Ejemplo 2 — Repositorio propio con interfaz específica

```csharp
public interface IVinosRepo : IEfPagedRepository<Vino> { }

public class VinosRepo(TestDbContext db)
    : EfPagedRepository<Vino, TestDbContext>(db), IVinosRepo { }
```

> 💡 **Este es el patrón recomendado**: una interfaz por entidad, para que la inyección sea explícita y puedas añadir métodos propios más adelante.

### Ejemplo 3 — Consulta con PK simple y compuesta

```csharp
public class ConsultasService(IVinosRepo _vinos, IEfReadRepository<VinosCatasPuntuacion> _puntos)
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
public Task<MlResult<PagedResult<Vino>>> PaginaAsync(
        int pagina, int tamanyo, string? bodega, CancellationToken ct)
    => _vinos.TryGetDataPaginationAsync(
                 paginationInfo: (pagina, tamanyo),                       // 🔑 tupla implícita
                 orderBy:        SortDirection.Descending,
                 orderByField:   x => x.FechaEntrada,                     // 🔑 SIEMPRE ordena
                 filter:         x => bodega == null || x.Bodega == bodega,  // 🔑 nunca null
                 ct:             ct);
```

> 💡 **Dos claves**: pasa `orderByField` para que el orden de las páginas sea determinista; si `filter` es `null`, se interpreta como `x => true`. Ambas variantes, síncrona y asíncrona, calculan `TotalCount` con el filtro efectivo.

### Ejemplo 6 — Composición con validación y logging

```csharp
using MoralesLarios.OOFP.Logging;
using MoralesLarios.OOFP.Validation.FluentValidation;

public class AltaVinosService(IVinosRepo _repo, IValidator<Vino> _validator, ILogger<AltaVinosService> _logger)
{
    public Task<MlResult<Vino>> CrearAsync(Vino vino, CancellationToken ct)
        => _validator.MlValidate(vino)
                     .BindAsync(v => _repo.TryAddAsync(v, ct))
                     .LogMlResultInformationIfValidAsync(_logger, v => $"Vino {v.Id} creado")
                     .LogMlResultErrorIfFailAsync       (_logger, e => $"Error al crear vino: {e.GetErrorsSummary()}");
}
```

### Ejemplo 7 — Transacción con la unidad de trabajo

```csharp
public Task<MlResult<Vino>> CrearConCataAsync(Vino vino, Cata cata, CancellationToken ct)
    => _vinos.UnitOfWork.TryExecuteInTransactionAsync(async token =>
    {
        var vinoResult = await _vinos.TryAddAsync(vino, token);
        if (vinoResult.IsFailure)
            return vinoResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

        var cataResult = await _catas.TryAddAsync(cata, token);
        if (cataResult.IsFailure)
            return cataResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

        return vinoResult;
    }, ct);
```

`_vinos` y `_catas` deben compartir el mismo `TContext` y ámbito. El helper confirma ambos cambios
una vez si el resultado es válido y revierte si alguna operación falla.

### Ejemplo 8 — ❌ Qué no hacer / ✅ qué hacer

**Registro del contexto, repositorios y contrato propio:**

```csharp
services.AddDbContext<TestDbContext>(options => options.UseSqlServer(connectionString));
services.AddOofpRepositories<Vino, TestDbContext>();
services.AddScoped<IVinosRepo, VinosRepo>();
```

**Paginación sin ordenación:**

```csharp
// ❌ MAL: paginar sin ordenar → páginas con contenido arbitrario
await repo.TryAllPaginationAsync((1, 20), ct: ct);

// ✅ BIEN: ordena y filtra explícitamente
await repo.TryGetDataPaginationAsync((1, 20), SortDirection.Ascending, x => x.Id, x => true, ct);
```

**Obtener el último registro:**

```csharp
// ❌ MAL: TryGetLast en una tabla grande → trae todas las filas a memoria
await repo.TryGetLastAsync(x => x.Bodega == "Rioja", ct);

// ✅ BIEN: seleccionar una sola fila según un orden explícito
var orderedRepo = provider.GetRequiredService<IEfOrderedReadRepository<Vino>>();
await orderedRepo.TryGetLastByAsync(x => x.Bodega == "Rioja", x => x.FechaEntrada,
                                    SortDirection.Ascending, ct);
```

**El `TotalCount` con filtro (ambas variantes lo respetan):**

```csharp
var paginaSync = repo.TryGetDataPagination((1, 20), SortDirection.Ascending, x => x.Id,
                                           x => x.Año == 2020);
var paginaAsync = await repo.TryGetDataPaginationAsync((1, 20), SortDirection.Ascending, x => x.Id,
                                                       x => x.Año == 2020, ct);
```

**Modificación de entidades:**

```csharp
// El repositorio desvincula la instancia hallada antes de actualizar la instancia nueva.
await repo.TryUpdateAsync(new Vino { Id = 42, Nombre = "X" }, ct, 42);

// ✅ BIEN: recuperar, modificar y actualizar la misma instancia
await repo.TryFindAsync(ct, 42).MapAsync (v => { v.Nombre = "X"; return v; })
                               .BindAsync(v => repo.TryUpdateAsync(v, ct));
```

**Atomicidad entre operaciones:**

```csharp
// ✅ BIEN: el helper confirma o revierte el conjunto de escrituras
var resultado = await repoVinos.UnitOfWork.TryExecuteInTransactionAsync(async token =>
{
    var vinoResult = await repoVinos.TryAddAsync(vino, token);
    if (vinoResult.IsFailure)
        return vinoResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

    var cataResult = await repoCatas.TryAddAsync(cata, token);
    if (cataResult.IsFailure)
        return cataResult.GetFailureDetailsOrThrow().ToMlResultFail<Vino>();

    return vinoResult;
}, ct);
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|---|---|
| CRUD completo de una entidad | `IEfRepository<T>` |
| CRUD + paginación | `IEfPagedRepository<T>` ✅ el más completo |
| Solo consultas | `IEfReadRepository<T>` |
| Solo consultas + paginación | `IEfPagedReadRepository<T>` |
| Buscar por PK con mensaje propio | `TryFindAsync("mensaje", ct, pk)` |
| Buscar por condición | `TryFirstAsync(x => …, ct)` |
| Listar todo | `TryAllAsync(ct)` (usa `AsNoTracking`) |
| Listar filtrado | `TryGetDataAsync(x => …, ct)` |
| Página de resultados | `TryGetDataPaginationAsync(…)` ✅ con `orderByField` y `filter` |
| Insertar varios de golpe | `TryAddRangeAsync(items, ct)` |
| Borrar comprobando existencia | `TryRemoveAsync("mensaje", ct, pk)` |
| El último registro | `IEfOrderedReadRepository<T>.TryGetLastBy[Async]` ✅ ordena y limita a una fila |
| Confirmar escritura directa | `await repo.UnitOfWork.TryCommitAsync(ct)` |
| Varias escrituras atómicas | `await repo.UnitOfWork.TryExecuteInTransactionAsync(...)` |
| `Include`, proyecciones, agregados | ❌ No disponible: baja a `GetContext()` |
| Registrar en DI | `AddOofpRepositories<T, TContext>()` ✅ |

---

## Mejores prácticas

1. **Registra siempre con `AddOofpRepositories`**: registra las nueve interfaces funcionales de la entidad con ciclo de vida `Scoped`, el mismo que el del `DbContext`.
2. **Llama a `AddOofpRepositories` al principio** de la configuración de servicios, antes de cualquier registro que dependa de repositorios.
3. **No intentes liberar un repositorio a mano**: desde la 2.0.0 no implementa `IDisposable`, porque el `DbContext` lo gestiona el contenedor.
4. **Usa las versiones asíncronas** en aplicaciones web para no bloquear el hilo durante el acceso a datos.
5. **La paginación es estable por defecto**: usa la clave primaria EF como orden o desempate; una entidad sin clave produce un fallo. El filtro nulo se interpreta como `x => true`.
6. **Para una sola fila ordenada, usa `IEfOrderedReadRepository<T>.TryGetLastBy[Async]`**; `TryGetLast[Async]` conserva la lectura completa por compatibilidad.
7. **Para modificar, recupera y actualiza la misma instancia** (`TryFind` → modificar → `TryUpdate(v)` sin `pk`).
8. **Personaliza los `MlErrorDetails`** si el mensaje va a llegar al usuario: los de serie están en inglés e incluyen el nombre del tipo.
9. **Segrega las interfaces**: inyecta `IEfReadRepository<T>` donde solo consultes.
10. **Declara una interfaz por entidad** (`IVinosRepo : IEfPagedRepository<Vino>`) para poder ampliarla sin romper llamadas.
11. **Si necesitas atomicidad entre repositorios, usa `TryExecuteInTransactionAsync`** desde el UoW que comparten.
12. **Confirma explícitamente las escrituras directas** con `UnitOfWork.TryCommit[Async]`; los servicios `CrudServiceFp` ya confirman tras cada operación.
13. **Para `Include`, proyecciones o agregados, baja a `GetContext()`**: la API no los cubre y forzarla es peor que usar EF Core directamente.
14. **Combina con [`Validation`](../MoralesLarios.OOFP.Validation/README.md)** para validar la entidad antes de persistirla, y con [`MoralesLarios.OOFP.Logging`](../MoralesLarios.OOFP.Logging/README.md) para trazar.
15. **Los repositorios del mismo ámbito comparten el *change tracker***: sus agregados y colaboradores usan el `DbContext` `Scoped` inyectado.

---

## Resumen

- Repositorios genéricos sobre **EF Core 8** que devuelven `MlResult<T>`: el "no encontrado" es un `Fail` con mensaje personalizable y los argumentos se validan antes de tocar la base de datos.
- **Nueve interfaces componibles** por entidad, incluidas `IEfOrderedReadRepository<T>`. Los métodos de las clases repositorio son `virtual`: puedes heredar y sobrescribir.
- Registro: `AddOofpRepositories<T, TContext>()` registra las nueve interfaces funcionales con ciclo de vida **`Scoped`**.
- Los agregados y sus colaboradores comparten el mismo `DbContext` y `ChangeTracker` dentro del ámbito.
- ✅ **El repositorio no es dueño del `DbContext`**: desde la 2.0.0 no implementa `IDisposable`; el contenedor libera el contexto al cerrar el ámbito.
- ✅ **Unidad de trabajo explícita**: las escrituras preparan cambios; `TryCommit[Async]` los confirma y `TryExecuteInTransaction[Async]` coordina commit/rollback entre operaciones.
- ⚠️ **Límites concretos**: `TryGetLast` materializa toda la consulta; paginar requiere una clave primaria EF para garantizar el orden total.
- ⚠️ **No cubre** `Include`, proyecciones, agregados, `ThenBy`, SQL crudo ni operaciones por lotes: para eso, `GetContext()`.

---

## Ver también

### Navegación general

- [README de la solución completa](../README.md)
- [README del núcleo `MoralesLarios.OOFP`](../MoralesLarios.OOFP.Core/README.md)
- [Introducción general al núcleo funcional](../docs/1_Intro.md)

### Proyectos relacionados

- [`MoralesLarios.OOFP.Shared`](../MoralesLarios.OOFP.Shared/README.md) — 🔑 `PageRequest` y `PagedResult<T>`
- [`MoralesLarios.OOFP.ValueObjects`](../MoralesLarios.OOFP.ValueObjects/README.md) — `IntNonNegative` y las conversiones de paginación
- [`MoralesLarios.OOFP.Validation`](../MoralesLarios.OOFP.Validation/README.md) — validar la entidad antes de persistirla
- [`MoralesLarios.OOFP.Validation.FluentValidation`](../MoralesLarios.OOFP.Validation.FluentValidation/README.md) — validación con FluentValidation
- [`MoralesLarios.OOFP.Logging`](../MoralesLarios.OOFP.Logging/README.md) — trazar los fallos de acceso a datos
- [`MoralesLarios.OOFP.Application`](../MoralesLarios.OOFP.Application/README.md) — capa de servicio sobre estos repositorios
- [`MoralesLarios.OOFP.AspNetCore.Results`](../MoralesLarios.OOFP.AspNetCore.Results/README.md) — convertir el `MlResult` en respuesta HTTP

### Documentación del núcleo útil aquí

- [`MlResult<T>` — el contenedor de éxito/error](../docs/core-types/MlResult.md)
- [`MlErrorDetails` — mensajes y detalles del error](../docs/core-types/MlResultErrors.md)
- [`TryMap` y `TryBind` — capturar excepciones en el raíl](../docs/Map/1_Map.md) 🔑 la base del proyecto
- [`EnsureFp` — validaciones de guarda](../docs/EnsureFp/EnsureFp.md) 🔑 `NotNull`, `NotEmpty`
- [`Bind` — encadenar operaciones que pueden fallar](../docs/Bind/3_Bind.md)
- [`Map` — transformación con cortocircuito](../docs/Map/1_Map.md)
- [`Match` — resolver el `MlResult` en un valor](../docs/Match/1_Match.md)
- [Métodos asíncronos en el raíl](../docs/1_Intro.md#sufijos-de-asincronía)
