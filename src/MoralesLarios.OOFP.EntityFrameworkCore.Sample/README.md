# MoralesLarios.OOFP.EntityFrameworkCore.Sample

Ejemplo ejecutable de repositorios EF Core y unidad de trabajo con SQLite en memoria.

## Ejecutar

```powershell
dotnet run --project MoralesLarios.OOFP.EntityFrameworkCore.Sample/MoralesLarios.OOFP.EntityFrameworkCore.Sample.csproj
```

El programa registra dos repositorios `Scoped` sobre el mismo `DbContext`, confirma un pedido y su
detalle con un único `TryExecuteInTransactionAsync` y demuestra que un resultado fallido no deja
filas parciales en la base de datos. No requiere servidor ni configuración externa.

`Managers/IApplicationManager<TEntity, TDto>` y `Managers/ApplicationManager<TEntity, TDto>` se
conservan como contratos vacíos para extensiones del consumidor; el ejemplo transaccional se ejecuta
desde `Program.cs`.
