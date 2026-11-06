# Épica · Paginación determinista y asíncrona en EntityFrameworkCore

> **Versión destino** · `2.0.0`, pendiente de autorización del propietario
> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Origen** · [análisis 04 · nulabilidad](../../../../Mejoras/EntityFrameworkCore/04-Nulabilidad-Genericos-y-Contratos.md) y [06 · rendimiento](../../../../Mejoras/EntityFrameworkCore/06-Rendimiento.md)
> **Estado** · ✅ Completada · `US-01…04` implementadas; orden por clave EF y compatibilidad NRT verificados
> **Esfuerzo estimado** · L (confianza baja; supuesto: resolver orden por clave EF, modificar contrato público y probar empates)
> **Riesgo** · 🔴 muy alto · orden sin estabilidad y anotaciones públicas

> ⚠️ **Ruptura silenciosa registrada** · el orden total puede cambiar las filas de páginas existentes; el punto 76 del prompt describe el impacto y la migración.

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Hallazgos colaterales](#2-hallazgos-colaterales)
3. [Alcance](#3-alcance)
4. [Historias de usuario](#4-historias-de-usuario)
5. [Orden de ejecución](#5-orden-de-ejecución)
6. [Decisiones cerradas](#6-decisiones-cerradas)
7. [Definición de terminado](#7-definición-de-terminado)
8. [Apéndice · paginación actual y destino](#8-apéndice--paginación-actual-y-destino)

## 1. Contexto y problema

El contador del método asíncrono de paginación usa `Count()` síncrono después de `ToListAsync(ct)`. El selector de ordenación es opcional, y `PrivateOrderBy` devuelve la consulta sin `OrderBy` cuando es nulo; un selector no único tampoco desempata páginas. Las seis apariciones `null!` de `IEfPaginator<T>` ocultan que selector y filtro pueden faltar.

| Hallazgo | Evidencia |
|---|---|
| #6 · contrato NRT opcional | `Repositories/IEfPaginator.cs:24,34,44,55`; implementación en `Helpers/Extensions.cs:38-44` y `EfPagedReadRepository.cs:137-139,173-176` |
| #10 · conteo síncrono en ruta asíncrona | `Repositories/EfPagedReadRepository.cs:182-189` |
| #11 · `Skip/Take` sin orden total | `Repositories/EfPagedReadRepository.cs:145-149,182-187`; `Helpers/Extensions.cs:38-44` |

## 2. Hallazgos colaterales

No se añaden hallazgos fuera de #6, #10 y #11 del análisis. La selección de orden estable debe considerar filtros y total de filas, pero no se infieren necesidades de índices ni keyset pagination sin evidencia de carga.

## 3. Alcance

**Dentro**: decisión de orden total; `CountAsync` secuencial con el mismo filtro y token; anotaciones NRT de parámetros que sigan siendo opcionales; regresiones de páginas, empates, filtro y conteo.

**Fuera**: migrar de páginas numeradas a keyset/cursor, cambiar `PageRequest`, crear índices o cambiar modelo/datos de una aplicación consumidora.

## 4. Historias de usuario

### US-01 · Decidir el orden total de la paginación · prioridad crítica · S

**Título de issue**: Definir cómo ordenar y desempatar las páginas de repositorio

**Como / quiero / para**: Como consumidor de paginación, quiero que la selección de elementos de cada página siga un orden estable, para evitar omisiones o duplicados entre páginas.

**Contexto**: `orderByField` puede ser nulo en `IEfPaginator<T>:24,34,44,55`; `PrivateOrderBy` no ordena si falta selector (`Helpers/Extensions.cs:41-44`).

**Decisión**: (b). El selector sigue siendo opcional; se usa la clave primaria EF como orden por defecto y como desempate para claves simples o compuestas. Si la entidad no tiene clave primaria, la operación falla en lugar de prometer estabilidad.

**Criterios de aceptación**
- [x] La decisión cubre selector nulo, empates, clave compuesta y entidades keyless.
- [x] `SortDirection` se aplica también a cada propiedad del desempate.
- [x] Se define que las páginas existentes pueden contener filas distintas.
- [x] La decisión se registra en el punto 76 del prompt antes del cambio.

**Pruebas**: diseñar datos con empates, valores nulos si el proveedor los admite y más de una página.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: la opción elegida puede alterar silenciosamente filas por página; requiere registro en seis sitios del prompt, número por asignar. Si se exige un selector no nulo, evaluar ruptura de fuente con consumidores NRT.

**Metadatos propuestos**: EFCore · prioridad crítica · S, confianza baja · tramo 1 · etiquetas por confirmar.

### US-02 · Ejecutar el conteo sincrónico con la ruta asíncrona · prioridad media · S

**Título de issue**: Cambiar el total de la página a `CountAsync`

**Como / quiero / para**: Como consumidor asíncrono, quiero que el conteo también use I/O asíncrono y el token de cancelación, para no bloquear el hilo durante la consulta de total.

**Contexto**: `EfPagedReadRepository.cs:182-189` llama `ToListAsync(ct)` y luego `Count()`.

**Alcance**: reemplazar el conteo de la variante async por `CountAsync(ct)` secuencial; conservar filtro, total y ausencia de tracking.

**Criterios de aceptación**
- [x] El total async usa `CountAsync(ct)` con el mismo `effectiveFilter`.
- [x] La operación se espera antes de iniciar otro uso del mismo `DbContext`.
- [x] Las variantes sincrónicas mantienen `Count()`.
- [x] Se conserva el mismo total para conjunto vacío, filtro parcial y múltiples páginas.

**Pruebas**: conteo filtrado/unfiltrado, cancelación, conjunto vacío, filtro y cantidad conocidos.

> **Progreso parcial** · `TryGetInternalDataAsync` ahora espera `CountAsync(ct)` usando el filtro existente; `TryGetDataPaginationAsync_OrdenaYRespetaTotalCount` comprueba total filtrado en dos páginas, `TryGetDataPaginationAsync_SinCoincidencias_DevuelveTotalCeroYPaginaVacia` cubre el conjunto vacío, y `TryGetDataPaginationAsync_TokenCancelado_DevuelveFallo` cubre cancelación de la ruta.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: Aditiva; se espera el mismo resultado. Sin cambio de firma.

**Metadatos propuestos**: EFCore · prioridad media · S, confianza media · tramo 1 · etiquetas por confirmar.

### US-03 · Aplicar una ordenación determinista antes de paginar · prioridad crítica · M

**Título de issue**: Garantizar orden total y estable antes de `Skip/Take`

**Como / quiero / para**: Como consumidor de páginas consecutivas, quiero que filas con claves de orden iguales no cambien arbitrariamente de página, para poder recorrer el resultado completo sin duplicados ni omisiones.

**Contexto**: `EfPagedReadRepository.cs:145-149,182-187` aplica `Skip/Take` después de un selector que puede ser nulo o no único.

**Alcance**: aplicar US-01 en los métodos síncrono y asíncrono; mantener `Where` antes de paginación, `TotalCount` filtrado y sort direction.

**Criterios de aceptación**
- [x] El query aplica el orden decidido antes de `Skip` y `Take`.
- [x] Cuando el criterio principal empata, el desempate sigue la regla aprobada.
- [x] Selector ausente recibe orden por clave primaria; una entidad sin clave produce `Fail`.
- [x] La página conserva `PageSize` y el total filtrado, salvo la última página.
- [x] La documentación describe la garantía y el efecto sobre páginas existentes.

**Pruebas**: varias páginas con valores de orden duplicados; orden ascendente y descendente; filtro; página vacía/fuera de rango; comparación de contenido exacto.

**Dependencias**: bloqueada por US-01.

**Compatibilidad/migración**: ruptura silenciosa si cambia la membresía de páginas; registrar búsqueda y detección en los seis sitios del prompt, número por asignar.

**Metadatos propuestos**: EFCore · prioridad crítica · M, confianza baja · tramo 2 · etiquetas por confirmar.

### US-04 · Reflejar en NRT la nulabilidad real de filtros/selectores · prioridad media · S

**Título de issue**: Anotar como opcionales los selectores y filtros de paginación

**Como / quiero / para**: Como consumidor con NRT, quiero que las firmas correspondan al comportamiento real, para no recibir una promesa de no nulidad falsa.

**Contexto**: `IEfPaginator<T>:24,34,44,55` usa `null!`; el helper trata un selector nulo como ausencia.

**Alcance**: anotar únicamente parámetros que se decida conservar opcionales; sincronizar interfaces e implementaciones y comprobar todos los consumidores relevantes.

**Criterios de aceptación**
- [x] Filtro y selector opcionales quedan anotados con `?` en el contrato y sus implementaciones.
- [x] Interfaces y clases implementadoras mantienen anotaciones compatibles.
- [x] Las llamadas consumidoras con ambos argumentos `null` compilan con `CS8625` tratado como error; no se producen avisos.

**Pruebas**: compilación de consumidores con selector/filtro omitido y proporcionado; tests de comportamiento de US-03.

**Dependencias**: bloqueada por US-01 y US-03.

**Compatibilidad/migración**: los parámetros permanecen opcionales y ahora se anotan como anulables, eliminando el aviso falso al pasar `null`; no cambia la firma binaria. El cambio de contenido de página está registrado en el punto 76.

**Metadatos propuestos**: EFCore · prioridad media · S, confianza baja · tramo 2 · etiquetas por confirmar.

## 5. Orden de ejecución

US-01 fija el contrato. US-02 puede avanzar en paralelo. US-03 implementa el contrato y US-04 se cierra con la firma final; el orden del último par puede ajustarse según si el diseño obliga a un selector no nulo.

## 6. Decisiones cerradas

- D-01 · **Clave primaria EF por defecto y desempate**; se aplican todas sus propiedades para claves compuestas.
- D-02 · **El selector permanece opcional**; no se añade sobrecarga ni se introduce una ruptura de fuente.
- D-03 · **Contrato probado con SQLite y EF Core 8**. No se presupone orden de valores `null` específico de otros proveedores; para ampliar soporte, validar el proveedor consumidor.

## 7. Definición de terminado

- [x] Todas las lecturas paginadas aplican un orden total antes de `Skip/Take` según D-01.
- [x] La variante async usa `CountAsync` y propaga cancelación.
- [x] Firmas NRT, documentación y regresiones concuerdan con la decisión.
- [x] La diferencia de contenido de página está registrada en el punto 76.
- [x] Build y suite de integración Release verificados; clave compuesta y entidad keyless cubiertas.

## 8. Apéndice · paginación actual y destino

```csharp
// Actual: puede no haber ORDER BY; el conteo es I/O síncrono.
var page = query.Where(filter)
	.PrivateOrderBy(direction, orderByField)
	.Skip(offset)
	.Take(pageSize)
	.ToListAsync(token);
var total = query.Where(filter).Count();

// Destino: orden total decidido en D-01 y conteo asíncrono secuencial.
var total = await query.Where(filter).CountAsync(token);
```

## Ver también

- [Índice de épicas EFCore](README.md)
- [Análisis de nulabilidad](../../../../Mejoras/EntityFrameworkCore/04-Nulabilidad-Genericos-y-Contratos.md)
- [Análisis de rendimiento](../../../../Mejoras/EntityFrameworkCore/06-Rendimiento.md)
- [Prompt de migración](../../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md)
