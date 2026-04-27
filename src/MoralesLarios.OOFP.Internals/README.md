# MoralesLarios.OOFP.Internals

Tipos internos reutilizables para modelos de paginación y metadatos comunes.

## Qué hace

- Define contratos de información paginada (`PaginationInfo`, `PaginationResultInfo<T>`).
- Incluye validaciones de rango y normalización de valores.

## Dependencias

- `MoralesLarios.OOFP.ValueObjects`

## Clases

### `PaginationInfo`

```csharp
public record PaginationInfo(int PageNumber, int PageSize)
```

Comportamiento:

- `PageNumber` mínimo 1 (`Math.Max(1, PageNumber)`).
- `PageSize` limitado entre 1 y 1000 (`Math.Clamp`).
- Conversión implícita desde `(int pageNumber, int pageSize)` y desde `(IntNotNegative, IntNotNegative)`.

### `PaginationResultInfo<T>`

```csharp
public record PaginationResultInfo<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount)
    : PaginationInfo(PageNumber, PageSize)
```

Incluye `Items` y `TotalCount` además de los datos de página.

## Ejemplos

```csharp
PaginationInfo page = (pageNumber: 0, pageSize: 5000);
// queda normalizado a PageNumber=1, PageSize=1000

PaginationResultInfo<string> result = (new[] { "a", "b" }, 1, 20, 200);
```
