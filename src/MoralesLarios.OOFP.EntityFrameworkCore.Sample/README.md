# MoralesLarios.OOFP.EntityFrameworkCore.Sample

Proyecto de integraci�n entre `MoralesLarios.OOFP.EntityFrameworkCore` y capa Web API.

> Estado actual: **base/skeleton** (m�nimo).

## Qu� contiene

- `Managers/IApplicationManager<TEntity, TDto>`
- `Managers/ApplicationManager<TEntity, TDto>`

Actualmente el contrato e implementaci�n est�n vac�os y listos para extender con l�gica de aplicaci�n espec�fica (orquestaci�n de repos EF + mapping DTO + respuestas web).

## Dependencias

- `MoralesLarios.OOFP.EntityFrameworkCore`
- `MoralesLarios.OOFP.AspNetCore.Results`

## Uso recomendado hoy

Si necesitas funcionalidad real, utiliza directamente:

- `MoralesLarios.OOFP.Application` (`ICrudServiceFp<,>`) para l�gica CRUD funcional.
- `MoralesLarios.OOFP.AspNetCore.Controllers` / `MoralesLarios.OOFP.AspNetCore.Results` para exponer endpoints.

## Ejemplo de extensi�n

```csharp
public interface IApplicationManager<TEntity, TDto>
{
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(CancellationToken ct = default);
}

public class ApplicationManager<TEntity, TDto>(ICrudServiceFp<TEntity, TDto> service)
    : IApplicationManager<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    public Task<MlResult<IEnumerable<TDto>>> GetAllAsync(CancellationToken ct = default)
        => service.AllAsync(ct: ct);
}
```
