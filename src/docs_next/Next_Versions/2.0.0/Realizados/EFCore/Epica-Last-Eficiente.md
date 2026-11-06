# Épica · Selección eficiente del último resultado

> **Versión destino** · `2.0.0`, pendiente de autorización del propietario
> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Origen** · [análisis 06 · rendimiento](../../../../Mejoras/EntityFrameworkCore/06-Rendimiento.md)
> **Estado** · ✅ Completada · `US-01…02` implementadas y verificadas con SQLite
> **Esfuerzo estimado** · M (confianza baja; supuesto: añadir operación con orden explícito y verificar traducción en proveedor de test)
> **Riesgo** · 🟠 alto · “último” requiere una semántica de orden que la API actual no declara

## Índice

1. [Contexto y problema](#1-contexto-y-problema)
2. [Hallazgos colaterales](#2-hallazgos-colaterales)
3. [Alcance](#3-alcance)
4. [Historias de usuario](#4-historias-de-usuario)
5. [Orden de ejecución](#5-orden-de-ejecución)
6. [Decisiones cerradas](#6-decisiones-cerradas)
7. [Definición de terminado](#7-definición-de-terminado)
8. [Apéndice · patrón actual y destino](#8-apéndice--patrón-actual-y-destino)

## 1. Contexto y problema

`TryGetLast` no recibe selector de orden y carga todas las coincidencias (`AsEnumerable().LastOrDefault()`); `TryGetLastAsync` ejecuta `ToListAsync(token)` antes de devolver un único elemento. El orden de una tabla sin `OrderBy` no está garantizado, por lo que cambiarlo a `First/Last` en SQL sin definir criterio sería un cambio especulativo de semántica.

| Hallazgo | Evidencia |
|---|---|
| #9 · materializa filas innecesarias | `Repositories/EfReadRepository.cs:135-165`; [análisis 06](../../../../Mejoras/EntityFrameworkCore/06-Rendimiento.md) |
| Método de integración actual | `Tests.Integration/Repos/RepoVinorFpTests.cs:293-311` comprueba una coincidencia, pero no cardinalidad ni orden de múltiples filas |

## 2. Hallazgos colaterales

El mismo patrón existe en las variantes síncrona y asíncrona; ambas forman parte de US-02. No se presume que “último” tenga un orden útil hasta cerrar D-01.

## 3. Alcance

**Dentro**: decidir el significado de “último”; agregar una consulta ordenada con límite de una fila; incluir token async y error `Fail` si no existe; probar que la consulta no materializa todas las coincidencias.

**Fuera**: alterar silenciosamente los métodos existentes sin orden, generalizar búsqueda/paginación, o afirmar un porcentaje de rendimiento sin medir SQL y carga real.

## 4. Historias de usuario

### US-01 · Definir el orden observable de “último” · prioridad alta · S

**Título de issue**: Acordar un selector de orden para recuperar el último registro

**Como / quiero / para**: Como consumidor de repositorios, quiero definir qué significa “último” para una entidad, para que el resultado sea estable y pueda traducirse a una consulta limitada en servidor.

**Contexto**: `EfReadRepository.cs:135-165` usa la enumeración que entregue el proveedor sin `OrderBy`.

**Alcance**: decidir si se añade una operación con selector obligatorio (recomendado) y si el selector debe ser único o desempatar con clave EF. Mantener el contrato existente mientras no se apruebe una sustitución.

**Criterios de aceptación**
- [x] `Ascending` elige el máximo del selector y `Descending` el mínimo.
- [x] La clave EF deshace empates, incluidas todas las propiedades de claves compuestas.
- [x] `TryGetLast[Async]` permanece intacto; la nueva interfaz se añade sin romper llamadas existentes.

**Pruebas**: caso con varias filas y orden no correlacionado con el orden de inserción; caso con empate; sin coincidencias.

**Dependencias**: Sin dependencias.

**Compatibilidad/migración**: Decisión bloqueante. Mantener los métodos actuales y añadir una API nueva es aditivo; reinterpretarlos altera comportamiento silenciosamente.

**Metadatos propuestos**: EFCore · prioridad alta · S, confianza baja · tramo 1 · etiquetas por confirmar.

### US-02 · Obtener una sola fila en la base de datos · prioridad media · M

**Título de issue**: Añadir lectura de último elemento limitada por orden en EF Core

**Como / quiero / para**: Como consumidor que necesita el extremo más reciente/alto de un conjunto, quiero que EF Core ordene y limite la consulta a una fila, para evitar cargar todas las coincidencias en memoria.

**Contexto**: las rutas actuales materializan toda la colección en `EfReadRepository.cs:138,165`.

**Alcance**: agregar las variantes síncrona y asíncrona decididas en US-01; aplicar filtro, `AsNoTracking` si corresponde y orden traducible; no alterar `TryGetLast` actual sin autorización expresa.

**Criterios de aceptación**
- [x] El selector se aplica en SQL y la consulta retorna como máximo una entidad (`LIMIT 1` verificado).
- [x] La variante asíncrona usa EF async y propaga `CancellationToken`.
- [x] Sin coincidencias o sin clave primaria EF se devuelve un resultado `Fail`.
- [x] La API nueva tiene XML docs y aparece en el README del proyecto.

**Pruebas**: selección exacta entre varias filas; empates según D-01; filtro; ausencia; query interceptor/log confirma una lectura limitada, no lista completa.

**Dependencias**: bloqueada por US-01.

**Compatibilidad/migración**: aditiva si se incorpora como operación nueva. Si se cambia el resultado antiguo, registrar la ruptura de comportamiento en los seis sitios del prompt; número por asignar.

**Metadatos propuestos**: EFCore · prioridad media · M, confianza baja · tramo 1 · etiquetas por confirmar.

## 5. Orden de ejecución

US-01 es el único bloqueo. Con su decisión cerrada, US-02 se implementa y valida con el proveedor de producción o una configuración que confirme traducción SQL.

## 6. Decisiones cerradas

- D-01 · **Añadir `TryGetLastBy[Async]` con selector obligatorio** y mantener intactos los métodos anteriores.
- D-02 · **Desempatar por todas las propiedades de la clave primaria**, en el mismo sentido que `SortDirection`.
- D-03 · **SQLite con EF Core 8** valida traducción y `LIMIT 1`; no se afirma un porcentaje de rendimiento ni se extrapola a otros proveedores.

## 7. Definición de terminado

- [x] “Último” tiene un orden documentado y probado, sin depender del orden físico de la tabla.
- [x] SQLite muestra una consulta limitada a una fila en variantes sync y async; no se afirman speedups no medidos.
- [x] Los métodos antiguos conservan su semántica.
- [x] Hay cobertura de filtro, empate, clave compuesta, ausencia y cancelación; migración aditiva registrada en el punto 75.
- [x] Build y suite de integración Release verificados.

## 8. Apéndice · patrón actual y destino

```csharp
// Actual: materializa todas las filas antes de seleccionar una.
var items = await query.AsNoTracking().Where(filter).ToListAsync(token);
var last = items.LastOrDefault();

// API implementada: orden explícito, desempate por clave EF y límite de una fila.
var result = await orderedRepository.TryGetLastByAsync(filter, orderByField,
													   SortDirection.Ascending, token);
```

El fragmento destino depende de la decisión sobre desempates y no sustituye la API existente sin aprobación.

## Ver también

- [Índice de épicas EFCore](README.md)
- [Análisis de rendimiento](../../../../Mejoras/EntityFrameworkCore/06-Rendimiento.md)
- [Prompt de migración](../../../../Copilot/.github/prompts/oofp-upgrade-2.prompt.md)
