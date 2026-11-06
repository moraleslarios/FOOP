# 06 · Rendimiento

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x para `CountAsync`; versión por decidir para orden paginado
> **Origen** · análisis estático del commit `2d07c33`; no incluye planes ni mediciones de carga
> **Estado** · propuesta, pendiente de validación con proveedor y carga reales
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟠 alto en páginas sin orden; 🟡 medio en las optimizaciones

---

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Alcance](#alcance)
3. [Historias de usuario](#historias-de-usuario)
4. [Orden de ejecución](#orden-de-ejecución)
5. [Decisiones abiertas](#decisiones-abiertas)
6. [Definición de terminado](#definición-de-terminado)
7. [Apéndice · código actual y propuesto](#apéndice--código-actual-y-propuesto)
8. [Ver también](#ver-también)

---

## 1. Contexto y problema

El análisis de fuente muestra que `TryGetLast` ejecuta la consulta completa y que el conteo del método de paginación asíncrono es síncrono. También muestra que ordenar es opcional, de modo que la consulta puede aplicar `Skip/Take` sin orden explícito. No se midieron latencias, SQL ni planes; la prioridad refleja la cardinalidad potencial y el riesgo de resultados paginados no repetibles, no una mejora porcentual estimada.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Repositories/EfReadRepository.cs` | 135-165 | Lee todas las filas coincidentes para devolver la última |
| 02 | `Repositories/EfPagedReadRepository.cs` | 173-189 | I/O síncrono dentro de método asíncrono |
| 03 | `Repositories/IEfPaginator.cs`; `Helpers/Extensions.cs`; `Repositories/EfPagedReadRepository.cs` | 24, 34, 44, 55; 38-44; 145-149, 182-187 | `Skip/Take` permite consultas sin orden estable |

### Punto 01 — `TryGetLast` materializa todos los resultados

La implementación síncrona convierte la consulta en `IEnumerable` y usa `LastOrDefault`; la asíncrona carga toda la lista antes de tomar el último elemento. La cantidad de filas transferidas y entidades materializadas crece con todas las coincidencias, aunque solo se devuelve una.

```csharp
// Repositories/EfReadRepository.cs:162-165
.TryMapAsync(async x =>
    (await GetContext().Set<T>().AsNoTracking().Where(filter).ToListAsync(token))
        .LastOrDefault()!); // ← materializa todas las coincidencias
```

> **Propuesta** · Añadir una operación que reciba una expresión de ordenación explícita y traduzca la selección del extremo a SQL (`OrderBy`/`ThenBy` inverso y `FirstOrDefaultAsync`, por ejemplo), limitando el resultado a una fila. No sustituir el método actual hasta definir qué significa “último”: sin orden explícito, el orden de filas no está garantizado. Validar SQL y cantidad de filas transferidas con el proveedor de producción. Clasificación: **aditiva** si se añade una API de orden explícito; el reemplazo de la semántica antigua puede ser de comportamiento.

### Punto 02 — Conteo síncrono dentro de paginación asíncrona

`TryGetInternalDataAsync` obtiene los elementos con `ToListAsync(ct)` pero calcula `Count` con `Count()`. Esto bloquea en la operación de base de datos síncrona dentro del flujo asíncrono y no pasa el token a esa consulta. La búsqueda encontró **0** llamadas `CountAsync` en los 28 ficheros de producción.

```csharp
// Repositories/EfPagedReadRepository.cs:182-189
.ToListAsync(ct))
.TryMapAsync(items => new {
    Count = GetContext().Set<T>().Where(effectiveFilter).Count(), // ← I/O síncrono
    Items = items
});
```

> **Propuesta** · Cambiar la ruta a `CountAsync(ct)` dentro de la composición asíncrona y mantener exactamente el mismo filtro y total. Comprobar que la ejecución no inicia dos operaciones concurrentes sobre el mismo `DbContext`; la consulta de conteo debe esperarse de forma secuencial. Clasificación: **aditiva**, sin cambio previsto en el resultado.

### Punto 03 — Paginación sin orden total y estable

`orderByField` es opcional. `PrivateOrderBy` devuelve la fuente sin ordenar cuando es `null`, y después se aplican `Skip` y `Take`. Aun cuando hay selector, si no es único, los elementos empatados no tienen un desempate definido. La selección de filas puede variar entre páginas o ejecuciones.

```csharp
// Helpers/Extensions.cs:38-44
=> orderByField is null
       ? source // ← sin ORDER BY
       : orderBy == SortDirection.Ascending
           ? source.OrderBy(orderByField)
           : source.OrderByDescending(orderByField);

// Repositories/EfPagedReadRepository.cs:145-149
.PrivateOrderBy(orderBy, orderByField)
.Skip((pageNumber - 1) * pageSize)
.Take(pageSize);
```

> **Propuesta** · Definir una ordenación determinista con clave única como desempate, o exigir un selector que ya proporcione un orden total. Para consultas frecuentes con páginas profundas, evaluar paginación por cursor, sin reemplazar el acceso por número de página sin aprobación: son contratos distintos. Clasificación: **ruptura de comportamiento silenciosa** si cambia qué filas aparecen en una página; registrar antes/después y cómo detectar duplicados u omisiones.

## Alcance

Revisión estática de consultas y sus límites de materialización. No se hicieron benchmarks, inspección de SQL generado, análisis de planes ni mediciones de producción; las propuestas de rendimiento deben validarse con esos datos.

## Historias de usuario

- Como consumidor, quiero que `TryGetLast` solicite solo la fila requerida cuando especifico el orden.
- Como consumidor asíncrono, quiero que las dos consultas de paginación respeten asincronía y cancelación.
- Como consumidor paginado, quiero que páginas sucesivas no omitan ni dupliquen filas por falta de un desempate estable.

## Orden de ejecución

1. Definir contrato de orden para `TryGetLast` y paginación.
2. Implementar el conteo asíncrono manteniendo filtro y secuencialidad.
3. Medir filas/SQL y latencia con el proveedor objetivo.
4. Documentar como ruptura cualquier cambio en composición de páginas.

## Decisiones abiertas

- D-01 · ¿Qué clave ordenable puede usarse de desempate para entidades con claves simples o compuestas?
- D-02 · ¿Se necesita acceso a páginas arbitrarias o bastaría paginación por cursor?
- D-03 · ¿Qué proveedor y distribución de datos representan la carga real para validar `TryGetLast`?

## Definición de terminado

- No se ejecuta I/O síncrono en la ruta asíncrona de paginación.
- Las consultas de extremo tienen un orden explícito y limitan filas en servidor.
- El resultado paginado tiene orden total y pruebas para empates y páginas consecutivas.
- Cualquier cambio de filas por página se registra en el prompt de migración.
- No se atribuye una mejora cuantitativa sin medición.

## Apéndice · código actual y propuesto

```csharp
// Actual
Count = query.Where(filter).Count();

// Propuesto en el camino asíncrono
Count = await query.Where(filter).CountAsync(cancellationToken);
```

## Ver también

- [03 · Modelo de error](03-Modelo-de-Error.md) · ejecución diferida de lecturas.
- [04 · Nulabilidad y contratos](04-Nulabilidad-Genericos-y-Contratos.md) · selector opcional.
- [README del análisis](README.md).
