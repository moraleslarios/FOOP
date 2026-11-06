# 03 · Modelo de error

> **Proyecto** · `MoralesLarios.OOFP.EntityFrameworkCore`
> **Versión destino** · versión mayor por decidir para cambios de ejecución diferida
> **Origen** · análisis del commit `2d07c33`, rama `release/2.0.0-nomenclatura`
> **Estado** · propuesta, pendiente de decisión
> **Esfuerzo total** · sin estimar
> **Riesgo** · 🔴 muy alto: excepciones de consulta pueden escapar del resultado funcional

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

Las interfaces describen operaciones que capturan excepciones y devuelven `MlResult`, pero dos rutas síncronas entregan secuencias diferidas. La creación del `IQueryable` no ejecuta SQL; el fallo de proveedor ocurre cuando el consumidor enumera el resultado, fuera del `TryMap` que construye el `MlResult`.

| Punto # | Fichero | Líneas | Naturaleza |
|---:|---|---:|---|
| 01 | `Repositories/EfReadRepository.cs` | 188-207, 218-236 | Ruptura de contrato funcional en la frontera de enumeración |
| 02 | `Repositories/EfReadRepository.cs`; `Helpers/Extensions.cs` | 32-33, 46, 74; 20-23 | Entrada nula puede lanzar antes de la guarda |

### Punto 01 — Las excepciones de enumeración escapan de `MlResult`

`TryGetData` devuelve un `IQueryable<T>` convertido a `IEnumerable<T>` sin ejecutarlo. `TryAll` devuelve el `DbSet` de forma igualmente diferida. En cambio, sus equivalentes asíncronos ejecutan `ToListAsync` antes de devolver. Por tanto, las rutas síncrona y asíncrona no ofrecen el mismo límite de captura.

```csharp
// Repositories/EfReadRepository.cs:188-191
public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter)
    => EnsureFp.NotNull(filter, "The filter cannot be null")
       .TryMap(x => (IEnumerable<T>)GetContext().Set<T>().AsNoTracking().Where(filter)); // ← la consulta no se ejecuta aquí
```

La documentación de `IEfReadRepository<T>` promete convertir excepciones de la operación en fallo; sin embargo, errores de conexión, traducción o lectura producidos durante `foreach` ocurren después del retorno. El consumidor puede recibir una respuesta `Success` y luego una excepción directa.

> **Propuesta** · Decidir un contrato único: materializar las lecturas síncronas dentro de la cadena protegida, o exponer explícitamente una consulta diferida cuyo consumidor sea dueño de la enumeración y de sus excepciones. La primera alternativa cambia el momento de ejecución, memoria y consistencia observables; clasificar como **ruptura de comportamiento silenciosa** y registrar en el prompt de migración antes de aplicarla. Añadir pruebas que provoquen el error al enumerar, no solo al construir la consulta.

### Punto 02 — El formato de claves se evalúa antes de la validación

La sobrecarga por defecto interpola `FormatCompositeKey` antes de entrar en la sobrecarga que valida `keyValues`. Si el consumidor pasa explícitamente un arreglo `null`, `source.Count()` falla y la excepción escapa de `TryFind`/`TryFindAsync` antes de `EnsureFp.NotNull`.

```csharp
// Repositories/EfReadRepository.cs:45-46
public virtual MlResult<T> TryFind(params object[] keyValues)
    => TryFind($"{keyValues.FormatCompositeKey()} values not found data in {typeName}", keyValues); // ← formatea antes de validar
```

`FormatCompositeKey` accede a `source.Count()` en `Helpers/Extensions.cs:20-23` y no acepta un origen anulable. El mismo patrón de construcción del mensaje aparece en las rutas de eliminación y actualización por clave (`EfDeleteRepository.cs:56,109`; `EfUpdateRepository.cs:62,129`).

> **Propuesta** · Validar `keyValues` antes de construir el mensaje y definir el tratamiento de claves individuales nulas. Las operaciones `Try*` deben devolver un fallo para la entrada inválida, no lanzar durante el formato del mensaje. Clasificación: **ruptura de comportamiento** al cambiar la respuesta de excepción a `MlResult.Fail`; conservar la respuesta anterior solo si se decide explícitamente por compatibilidad.

## Alcance

Este análisis cubre la evaluación y captura de errores en las operaciones de lectura y claves de entidad. No recomienda capturar errores que ocurran después de entregar deliberadamente una consulta diferida.

## Historias de usuario

- Como consumidor, quiero saber si una lectura devuelve datos materializados o una consulta diferida y quién captura sus errores.
- Como consumidor, quiero que una operación `Try*` convierta claves inválidas en `MlResult.Fail` de forma consistente.

## Orden de ejecución

1. Decidir el contrato de evaluación diferida para `TryGetData` y `TryAll`.
2. Corregir el orden de validación y formato de claves.
3. Añadir pruebas de error de enumeración y de arreglos/claves nulos.
4. Actualizar documentación de API y registro de migración si el comportamiento cambia.

## Decisiones abiertas

- D-01 · ¿Las lecturas síncronas deben materializar para cumplir la promesa de captura de excepciones?
- D-02 · ¿Se permite un valor `null` dentro de una clave compuesta y, si sí, cómo se representa en el mensaje?

## Definición de terminado

- Los límites donde se ejecuta SQL y donde se capturan errores están documentados y probados.
- Las sobrecargas de lectura síncronas y asíncronas tienen un contrato deliberadamente coherente o explícitamente distinto.
- Las entradas inválidas no provocan una excepción durante la construcción de mensajes antes de las guardas.
- Toda diferencia observable queda registrada como ruptura de comportamiento.

## Apéndice · código actual y propuesto

```csharp
// Actual: la excepción de enumeración queda fuera del TryMap.
var result = EnsureFp.NotNull(filter, message)
    .TryMap(_ => (IEnumerable<T>)dbContext.Set<T>().Where(filter));

// Alternativa a evaluar: materializar dentro del límite funcional.
var result = EnsureFp.NotNull(filter, message)
    .TryMap(_ => (IEnumerable<T>)dbContext.Set<T>().Where(filter).ToList());
```

## Ver también

- [06 · Rendimiento](06-Rendimiento.md) · materialización de `TryGetLast`.
- [08 · Calidad, pruebas y documentación](08-Calidad-Pruebas-y-Documentacion.md) · alcance actual de las aserciones.
- [README del análisis](README.md).
