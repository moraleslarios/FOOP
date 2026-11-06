# 04 · Nulabilidad, genéricos y contratos

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · 2.x o versión mayor, según el efecto de los nuevos avisos en consumidores
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🟠 alto por tratarse de una biblioteca pública con NRT habilitado

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

El proyecto habilita `<Nullable>enable</Nullable>`, pero los parámetros de ordenación y filtro de la paginación son opcionales mediante `null!` y se exponen como tipos no anulables. El código de `PrivateOrderBy` trata expresamente un selector nulo como «no ordenar», y los métodos normalizan un filtro nulo a `x => true`.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Repositories/IEfPaginator.cs` | 24, 34, 44, 55 | Contrato NRT no refleja los valores opcionales |
| 01 | `Helpers/Extensions.cs` | 38-44 | Confirma el tratamiento de selector nulo |
| 01 | `Repositories/EfPagedReadRepository.cs` | 137-139, 173-176 | Confirma el tratamiento de filtro nulo |

### Punto 01 — La firma declara no nulos valores que el contrato admite como nulos

Las cuatro operaciones públicas permiten omitir el selector y, en el caso filtrado, la expresión de filtro; la interfaz lo expresa con valores por defecto `null!`, no con anotaciones `?`. Hay **seis** apariciones `null!` en las cuatro firmas de `IEfPaginator<T>`: una en cada selector de `TryAllPagination`, una en su variante asíncrona, y selector más filtro en cada variante filtrada.

```csharp
// Repositories/IEfPaginator.cs:24,44
MlResult<PagedResult<T>> TryAllPagination(
    PageRequest paginationInfo,
    SortDirection orderBy = SortDirection.Ascending,
    Expression<Func<T, object>> orderByField = null!); // ← opcional en ejecución, no anulable en NRT
```

La implementación acepta esa ausencia (`PrivateOrderBy` retorna `source` si no hay selector) y sustituye un filtro ausente por una expresión verdadera. Los consumidores con NRT reciben una promesa de no nulidad que no coincide con la semántica real.

> **Propuesta** · Anotar como anulables los parámetros realmente opcionales y mantener no anulables los requeridos. Antes de cambiar la API, compilar un consumidor de referencia con `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` y decidir la estrategia de compatibilidad. La anotación de referencia puede introducir nuevos avisos para consumidores aunque no altere el IL; clasificar como **cambio de contrato fuente** y coordinarlo según la política de versiones.

## Alcance

Solo se señala la nulabilidad observable en la interfaz de paginación. Las 26 apariciones `null!` y 21 `default!` medidas en el proyecto completo no se consideran automáticamente defectos: varias son de implementación o de tipos valor como `CancellationToken` y requieren evaluación contextual.

## Historias de usuario

- Como consumidor con NRT habilitado, quiero que la firma indique qué filtros y selectores puedo omitir.
- Como mantenedor, quiero evitar supresiones que hagan parecer obligatorio un valor que la implementación acepta como ausente.

## Orden de ejecución

1. Confirmar con consumidores si el filtro y la ordenación nulos son parte intencional de la API.
2. Decidir el mecanismo de versión y avisos para anotar los parámetros opcionales.
3. Actualizar interfaces e implementaciones y compilar proyectos consumidores representativos.

## Decisiones abiertas

- D-01 · ¿Se conserva el comportamiento de paginar sin filtro y sin selector, o se exige siempre un orden explícito?
- D-02 · ¿La publicación de anotaciones NRT actualizadas se hace en versión menor con avisos previstos o se reserva a una mayor?

## Definición de terminado

- La nulabilidad de la interfaz coincide con la semántica de la implementación.
- Las anotaciones de interfaces e implementaciones son consistentes.
- Se registra cualquier aviso nuevo que pueda afectar consumidores con advertencias como errores.

## Apéndice · código actual y propuesto

```csharp
// Actual
Expression<Func<T, object>> orderByField = null!;
Expression<Func<T, bool>> filter = null!;

// Propuesto, solo si se ratifica que ambos argumentos son opcionales
Expression<Func<T, object>>? orderByField = null;
Expression<Func<T, bool>>? filter = null;
```

## Ver también

- [06 · Rendimiento](06-Rendimiento.md) · consecuencia de permitir páginas sin orden estable.
- [README del análisis](README.md).
