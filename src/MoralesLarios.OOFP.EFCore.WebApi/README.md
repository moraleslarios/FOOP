# MoralesLarios.OOFP.EFCore.WebApi

Proyecto de integración entre `MoralesLarios.OOFP.EFCore` y capa Web API.

> Estado actual: **base/skeleton** (mínimo).

## Qué contiene

- `Managers/IApplicationManager<TEntity, TDto>`
- `Managers/ApplicationManager<TEntity, TDto>`

Actualmente el contrato e implementación están vacíos y listos para extender con lógica de aplicación específica (orquestación de repos EF + mapping DTO + respuestas web).

## Dependencias

- `MoralesLarios.OOFP.EFCore`
- `MoralesLarios.OOFP.WebApi`

## Uso recomendado hoy

Si necesitas funcionalidad real, utiliza directamente:

- `MoralesLarios.OOFP.WebServices` (`IGenServiceFp<,>`) para lógica CRUD funcional.
- `MoralesLarios.OOFP.WebControllers` / `MoralesLarios.OOFP.WebApi` para exponer endpoints.

## Ejemplo de extensión

```csharp
public interface IApplicationManager<TEntity, TDto>
{
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(CancellationToken ct = default);
}

public class ApplicationManager<TEntity, TDto>(IGenServiceFp<TEntity, TDto> service)
    : IApplicationManager<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    public Task<MlResult<IEnumerable<TDto>>> GetAllAsync(CancellationToken ct = default)
        => service.AllAsync(ct: ct);
}
```
