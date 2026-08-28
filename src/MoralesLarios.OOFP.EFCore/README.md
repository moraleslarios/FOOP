# MoralesLarios.OOFP.EFCore

Capa de repositorios sobre Entity Framework Core 8 para el ecosistema FOOP.

Ofrece dos estilos:

| Estilo | Namespace | Visibilidad | Retorno | Uso recomendado |
|---|---|---|---|---|
| Funcional (`*Fp`) | `MoralesLarios.OOFP.EFCore.Repos` | pública | `MlResult<T>` / `Task<MlResult<T>>` | Sí |
| OOP | `MoralesLarios.OOFP.EFCore.OopRepos` | mayoritariamente `internal` | `T` / `Task<T>` | Motor interno |

> La API pública de este paquete es la funcional (`*Fp`). La capa OOP es el soporte interno que usan los repos funcionales.

## Qué hace

- CRUD completo con semántica funcional.
- Búsqueda por clave primaria simple o compuesta con `params object[] pk`.
- Consultas posicionales seguras: `TryFirst` y `TryLast`.
- Paginación con `PaginationInfo` y `PaginationResultInfo<T>`.
- Registro masivo en DI por entidad/contexto.
- Extensión por herencia: todos los métodos públicos de la capa funcional son `virtual`.

## Dependencias

- `Microsoft.EntityFrameworkCore` 8.0.3
- `net8.0`
- `MoralesLarios.OOFP`
- `MoralesLarios.OOFP.Internals`
- `MoralesLarios.OOFP.ValueObjects`

## Estructura del proyecto

```text
MoralesLarios.OOFP.EFCore/
├── Repos/                          ← API pública funcional
│   ├── EFRepoBaseFp.cs
│   ├── IEFRepoReaderFp.cs / EFRepoReaderFp.cs
│   ├── IEFRepoAdderFp.cs / EFRepoAdderFp.cs
│   ├── IEFRepoUpdaterFp.cs / EFRepoUpdaterFp.cs
│   ├── IEFRepoDeleterFp.cs / EFRepoDeleterFp.cs
│   ├── IEFRepoWriterFp.cs / EFRepoWriterFp.cs
│   ├── IEFRepoFp.cs / EFRepoFp.cs
│   ├── IEFPaginatorFp.cs
│   ├── IEFRepoReaderPaginationFp.cs / EFRepoReaderPaginationFp.cs
│   └── IEFRepoPaginationFp.cs / EFRepoPaginationFp.cs
├── OopRepos/                       ← motor interno
│   ├── EFRepoBase.cs, EFRepo.cs, EFRepoReader.cs
│   ├── EFRepoAdder.cs, EFRepoUpdater.cs, EFRepoDeleter.cs
│   ├── EFRepoPagination.cs, EFRepoReaderPagination.cs
│   └── IGetContextable.cs
├── Helpers/Extensions.cs           ← GetPkValues, PrivateOrderBy
├── Enums/OrderBy.cs                ← Ascending, Descending
├── RegisterServices.cs             ← Add*OOFPRepos<T, TContext>()
├── GlobalUsings.cs
└── README.md
```

## Registro en DI

```csharp
services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));

services.AddTransientOOFPRepos<User, AppDbContext>();
// o AddScopedOOFPRepos<User, AppDbContext>();
// o AddSingletonOOFPRepos<User, AppDbContext>();
```

Esto registra automáticamente para esa entidad tanto el motor OOP interno como la API funcional:

| OOP | Funcional |
|---|---|
| `IEFRepo<T>` | `IEFRepoFp<T>` |
| `IEFRepoReader<T>` | `IEFRepoReaderFp<T>` |
| `IEFRepoWriter<T>` | `IEFRepoWriterFp<T>` |
| `IEFRepoAdder<T>` | `IEFRepoAdderFp<T>` |
| `IEFRepoUpdater<T>` | `IEFRepoUpdaterFp<T>` |
| `IEFRepoDeleter<T>` | `IEFRepoDeleterFp<T>` |
| `IEFRepoPagination<T>` | `IEFRepoPaginationFp<T>` |
| `IEFRepoReaderPagination<T>` | `IEFRepoReaderPaginationFp<T>` |

### Importante

Los repos funcionales no resuelven sus dependencias por constructor; lo hacen desde `RegisterServices.ResolveRepoFp<TRepo>()` usando el `IServiceCollection` capturado por la llamada de registro.

Eso implica:

1. Debes registrar la entidad antes de crear instancias de repositorios derivados.
2. La llamada `Add*OOFPRepos<T, TContext>()` debe hacerse antes que el uso del repo concreto.
3. Si omites el registro, la resolución lanza `ArgumentException` con un mensaje claro.

---

## API funcional

### `IEFRepoReaderFp<T>`

```csharp
MlResult<T>       TryFind(params object[] pk);
MlResult<T>       TryFind(MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] pk);
Task<MlResult<T>> TryFindAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);

MlResult<T>       TryFirst(Expression<Func<T, bool>> filter);
MlResult<T>       TryFirst(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<T>       TryLast(Expression<Func<T, bool>> filter);
MlResult<T>       TryLast(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails);
Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
Task<MlResult<T>> TryLastAsync(Expression<Func<T, bool>> filter, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default);

MlResult<IEnumerable<T>>       TryGetData(Expression<Func<T, bool>> filter);
Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAll();
Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default);
```

No existe `TryFirstOrDefault` ni `TryLastOrDefault` en la interfaz pública; los métodos `TryFirst` y `TryLast` ya tienen la semántica más segura: devuelven `fail` si no hay resultados, sin lanzar excepción.

### `IEFRepoAdderFp<T>`

```csharp
MlResult<T>                    TryAdd(T item);
Task<MlResult<T>>              TryAddAsync(T item, CancellationToken token = default);
MlResult<IEnumerable<T>>       TryAddRange(IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

### `IEFRepoUpdaterFp<T>`

```csharp
MlResult<T>                    TryUpdate(T item);
MlResult<T>                    TryUpdate(T item, params object[] pk);
MlResult<T>                    TryUpdate(T item, MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>>              TryUpdateAsync(T item, CancellationToken token = default);
Task<MlResult<T>>              TryUpdateAsync(T item, CancellationToken token = default, params object[] pk);
Task<MlResult<T>>              TryUpdateAsync(T item, MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
MlResult<IEnumerable<T>>       TryUpdateRange(IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryUpdateRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

### `IEFRepoDeleterFp<T>`

```csharp
MlResult<T>                    TryRemove(T item);
MlResult<T>                    TryRemove(params object[] pk);
MlResult<T>                    TryRemove(MlErrorsDetails notFoundErrorDetails, params object[] pk);
Task<MlResult<T>>              TryRemoveAsync(T item, CancellationToken token = default);
Task<MlResult<T>>              TryRemoveAsync(CancellationToken token = default, params object[] pk);
Task<MlResult<T>>              TryRemoveAsync(MlErrorsDetails notFoundErrorDetails, CancellationToken token = default, params object[] pk);
MlResult<IEnumerable<T>>       TryRemoveRange(IEnumerable<T> items);
Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default);
```

### `IEFRepoWriterFp<T>`

```csharp
public interface IEFRepoWriterFp<T> : IEFRepoAdderFp<T>, IEFRepoUpdaterFp<T>, IEFRepoDeleterFp<T>
```

### `IEFRepoFp<T>`

```csharp
public interface IEFRepoFp<T> : IEFRepoReaderFp<T>, IEFRepoWriterFp<T>
```

### `IEFPaginatorFp<T>`

```csharp
MlResult<PaginationResultInfo<T>>       TryAllPagination(PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!);
Task<MlResult<PaginationResultInfo<T>>> TryAllPaginationAsync(PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, CancellationToken ct = default);

MlResult<PaginationResultInfo<T>>       TryGetDataPagination(PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!);
Task<MlResult<PaginationResultInfo<T>>> TryGetDataPaginationAsync(PaginationInfo paginationInfo, OrderBy orderBy = OrderBy.Ascending, Expression<Func<T, object>> orderByField = null!, Expression<Func<T, bool>> filter = null!, CancellationToken ct = default);
```

### `IEFRepoReaderPaginationFp<T>`

```csharp
public interface IEFRepoReaderPaginationFp<T> : IEFRepoReaderFp<T>, IEFPaginatorFp<T>
```

### `IEFRepoPaginationFp<T>`

```csharp
public interface IEFRepoPaginationFp<T> : IEFRepoFp<T>, IEFPaginatorFp<T>
```

---

## `EFRepoBaseFp` y la clase base para heredar

```csharp
public class EFRepoBaseFp(DbContext dbContext) : IGetContextable, IDisposable
{
    public DbContext GetContext() => dbContext;
    public virtual void Dispose() => GetContext()?.Dispose();
}
```

La base te da el contexto y, si necesitas, puedes tocar la capa de EF Core directamente para casos avanzados.

---

## `MlErrorsDetails` y manejo de errores

Los métodos `Try*` no suelen lanzarte errores; devuelven `MlResult` en estado `fail` con un `MlErrorsDetails`.

`MlErrorsDetails` admite conversiones implícitas desde `string`, `string[]`, `List<string>`, `MlError`, `MlError[]` y más, por lo que puedes hacer esto sin esfuerzo:

```csharp
await repo.TryFindAsync("No existe el vino solicitado", ct, id);
await repo.TryFindAsync(MlErrorsDetails.FromErrorMessage($"El vino {id} no existe"), ct, id);
```

Ejemplos útiles:

```csharp
var error = MlErrorsDetails.FromErrorMessage("Elemento no encontrado");
var error2 = MlErrorsDetails.FromErrorMessageWithValue<int>(42, "No existe el valor");
```

---

## `PaginationInfo`, `PaginationResultInfo<T>` y `OrderBy`

```csharp
public enum OrderBy { Ascending, Descending }
```

```csharp
public record PaginationInfo(int PageNumber, int PageSize);
public record PaginationResultInfo<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount)
    : PaginationInfo(PageNumber, PageSize);
```

`PaginationInfo` normaliza los valores:

- `PageNumber = Math.Max(1, PageNumber)`
- `PageSize = Math.Clamp(PageSize, 1, 1000)`

También puedes pasar tuplas directamente:

```csharp
var result = await repo.TryAllPaginationAsync((2, 25), OrderBy.Descending, x => x.FechaEntrada, ct);
```

---

## Ejemplos de uso desde 0 (uno por cada repositorio)

### Base del ejemplo

```csharp
public class Vino
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Bodega { get; set; } = string.Empty;
    public int Año { get; set; }
    public DateTime FechaEntrada { get; set; }
}

public class VinosCatasPuntuacion
{
    public int IdVino { get; set; }
    public int IdCata { get; set; }
    public string IdUsuario { get; set; } = string.Empty;
    public decimal Puntuacion { get; set; }
    public DateTime Fecha { get; set; }
}
```

```csharp
public class JfCatasDbContext : DbContext
{
    public DbSet<Vino> Vinos => Set<Vino>();
    public DbSet<VinosCatasPuntuacion> VinosCatasPuntuaciones => Set<VinosCatasPuntuacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<VinosCatasPuntuacion>()
            .HasKey(x => new { x.IdVino, x.IdCata, x.IdUsuario });
    }
}
```

### Patrón general

```csharp
services.AddDbContext<JfCatasDbContext>(o => o.UseSqlite(connectionString));
services.AddTransientOOFPRepos<Vino, JfCatasDbContext>();
services.AddTransient<IVinosRepo, VinosRepo>();
```

> Las entidades deben registrarse con `Add*OOFPRepos<T, TContext>()` antes de resolver repos concretos derivados.

### 1) `EFRepoReaderFp<T, TContext>`

```csharp
public interface IVinosReaderRepo : IEFRepoReaderFp<Vino> { }

public class VinosReaderRepo(JfCatasDbContext dbContext)
    : EFRepoReaderFp<Vino, JfCatasDbContext>(dbContext), IVinosReaderRepo { }
```

Registro:

```csharp
services.AddTransient<IVinosReaderRepo, VinosReaderRepo>();
```

Uso:

```csharp
public class VinosQueryService(IVinosReaderRepo repo)
{
    public Task<MlResult<Vino>> BuscarAsync(int id, CancellationToken ct)
        => repo.TryFindAsync(ct, id);

    public Task<MlResult<IEnumerable<Vino>>> TodosAsync(CancellationToken ct)
        => repo.TryAllAsync(ct);
}
```

### 2) `EFRepoAdderFp<T, TContext>`

```csharp
public interface IVinosAdderRepo : IEFRepoAdderFp<Vino> { }

public class VinosAdderRepo(JfCatasDbContext dbContext)
    : EFRepoAdderFp<Vino, JfCatasDbContext>(dbContext), IVinosAdderRepo { }
```

Uso:

```csharp
public class AltaVinosService(IVinosAdderRepo repo)
{
    public Task<MlResult<Vino>> InsertarAsync(Vino vino, CancellationToken ct)
        => repo.TryAddAsync(vino, ct);
}
```

### 3) `EFRepoUpdaterFp<T, TContext>`

```csharp
public interface IVinosUpdaterRepo : IEFRepoUpdaterFp<Vino> { }

public class VinosUpdaterRepo(JfCatasDbContext dbContext)
    : EFRepoUpdaterFp<Vino, JfCatasDbContext>(dbContext), IVinosUpdaterRepo { }
```

Uso:

```csharp
public class ModificarVinosService(IVinosUpdaterRepo repo)
{
    public Task<MlResult<Vino>> ActualizarAsync(Vino vino, CancellationToken ct)
        => repo.TryUpdateAsync(vino, ct, vino.Id);
}
```

### 4) `EFRepoDeleterFp<T, TContext>`

```csharp
public interface IVinosDeleterRepo : IEFRepoDeleterFp<Vino> { }

public class VinosDeleterRepo(JfCatasDbContext dbContext)
    : EFRepoDeleterFp<Vino, JfCatasDbContext>(dbContext), IVinosDeleterRepo { }
```

Uso:

```csharp
public class BorrarVinosService(IVinosDeleterRepo repo)
{
    public Task<MlResult<Vino>> BorrarAsync(int id, CancellationToken ct)
        => repo.TryRemoveAsync(MlErrorsDetails.FromErrorMessage($"El vino {id} no existe"), ct, id);
}
```

### 5) `EFRepoWriterFp<T, TContext>`

```csharp
public interface IVinosWriterRepo : IEFRepoWriterFp<Vino> { }

public class VinosWriterRepo(JfCatasDbContext dbContext)
    : EFRepoWriterFp<Vino, JfCatasDbContext>(dbContext), IVinosWriterRepo { }
```

Uso:

```csharp
public class VinosCommandService(IVinosWriterRepo repo)
{
    public Task<MlResult<Vino>> CrearAsync(Vino vino, CancellationToken ct)
        => repo.TryAddAsync(vino, ct);

    public Task<MlResult<Vino>> BorrarAsync(int id, CancellationToken ct)
        => repo.TryRemoveAsync(ct, id);
}
```

### 6) `EFRepoFp<T, TContext>`

```csharp
public interface IVinosRepo : IEFRepoFp<Vino> { }

public class VinosRepo(JfCatasDbContext dbContext)
    : EFRepoFp<Vino, JfCatasDbContext>(dbContext), IVinosRepo { }
```

Uso:

```csharp
public class VinosService(IVinosRepo repo)
{
    public Task<MlResult<Vino>> BuscarAsync(int id, CancellationToken ct)
        => repo.TryFindAsync(ct, id);

    public Task<MlResult<Vino>> GuardarAsync(Vino vino, CancellationToken ct)
        => repo.TryAddAsync(vino, ct);

    public Task<MlResult<Vino>> RenombrarAsync(int id, string nombre, CancellationToken ct)
        => repo.TryFindAsync(ct, id)
               .BindAsync(v => { v.Nombre = nombre; return repo.TryUpdateAsync(v, ct, v.Id); });
}
```

Ejemplo con PK compuesta:

```csharp
public interface IVinosCatasPuntuacionesRepo : IEFRepoFp<VinosCatasPuntuacion> { }

public class VinosCatasPuntuacionesRepo(JfCatasDbContext dbContext)
    : EFRepoFp<VinosCatasPuntuacion, JfCatasDbContext>(dbContext), IVinosCatasPuntuacionesRepo { }
```

Uso:

```csharp
public class PuntuacionesService(IVinosCatasPuntuacionesRepo repo)
{
    public Task<MlResult<VinosCatasPuntuacion>> BuscarAsync(int idVino, int idCata, string idUsuario, CancellationToken ct)
        => repo.TryFindAsync(ct, idVino, idCata, idUsuario);
}
```

### 7) `EFRepoReaderPaginationFp<T, TContext>`

```csharp
public interface IVinosReaderPaginationRepo : IEFRepoReaderPaginationFp<Vino> { }

public class VinosReaderPaginationRepo(JfCatasDbContext dbContext)
    : EFRepoReaderPaginationFp<Vino, JfCatasDbContext>(dbContext), IVinosReaderPaginationRepo { }
```

Uso:

```csharp
public class VinosGridService(IVinosReaderPaginationRepo repo)
{
    public Task<MlResult<PaginationResultInfo<Vino>>> PaginaAsync(int page, int size, CancellationToken ct)
        => repo.TryAllPaginationAsync(new PaginationInfo(page, size), OrderBy.Descending, x => x.FechaEntrada, ct);
}
```

### 8) `EFRepoPaginationFp<T, TContext>`

```csharp
public interface IVinosFullRepo : IEFRepoPaginationFp<Vino> { }

public class VinosFullRepo(JfCatasDbContext dbContext)
    : EFRepoPaginationFp<Vino, JfCatasDbContext>(dbContext), IVinosFullRepo { }
```

Uso:

```csharp
public class VinosFullService(IVinosFullRepo repo)
{
    public Task<MlResult<PaginationResultInfo<Vino>>> PaginaAsync(int page, int size, CancellationToken ct)
        => repo.TryAllPaginationAsync((page, size), OrderBy.Ascending, x => x.Id, ct);

    public Task<MlResult<Vino>> CrearAsync(Vino vino, CancellationToken ct)
        => repo.TryAddAsync(vino, ct);
}
```

---

## Helpers

### `GetPkValues`

```csharp
new object[] { 10, "A" }.GetPkValues(); // "(10, A)"
```

### `PrivateOrderBy`

```csharp
var query = dbContext.Vinos.AsQueryable();
var ordered = query.PrivateOrderBy(OrderBy.Descending, x => x.FechaEntrada);
```

---

## Notas finales

- El estilo funcional es el recomendado y el realmente usable desde fuera del ensamblado.
- La capa OOP es el motor interno de la biblioteca y tiene muchos tipos `internal`.
- Para integrarlo con web services o API, encaja muy bien con `MoralesLarios.OOFP.WebServices` y `MoralesLarios.OOFP.WebApi`.
- Si necesitas sobreescribir comportamientos concretos, puedes heredar de las clases `*Fp` y anular solo los métodos que te interesen.
