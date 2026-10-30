# 06 · Rendimiento

> **Proyecto** · `MoralesLarios.OOFP.Core`
> **Versión destino** · 2.1.1 (sólo el inventario documental) · 3.0.0 (los cambios de tipo y de firma)
> **Origen** · análisis profundo del núcleo, posterior al commit `0a6f8fc`
> **Estado** · inventario cerrado en la US-16 de la [Épica 03](../../Next_Versions/2.1.1/Core/Epica-03-Miembros-Publicos-y-Firmas.md) · resto pendiente de aprobación
> **Esfuerzo total** · XL
> **Riesgo** · 🟡 medio el inventario · 🔴 muy alto el cambio de `record` a `readonly struct`

---

## Índice

1. [Contexto](#1-contexto)
2. [Inventario · métodos `*Async` que no son asíncronos](#2-inventario--métodos-async-que-no-son-asíncronos)
   - [2.1. Criterio de detección](#21-criterio-de-detección)
   - [2.2. Resultado por fichero](#22-resultado-por-fichero)
   - [2.3. Casos de referencia](#23-casos-de-referencia)
   - [2.4. Implicaciones de rendimiento](#24-implicaciones-de-rendimiento)
   - [2.5. Acciones aplicadas en la 2.1.1](#25-acciones-aplicadas-en-la-211)
   - [2.6. Acciones aplazadas](#26-acciones-aplazadas)
3. [Hallazgos pendientes de análisis](#3-hallazgos-pendientes-de-análisis)
4. [Ver también](#4-ver-también)

---

## 1. Contexto

El núcleo está construido sobre `MlResult<T>`, un `record` de referencia, y sobre **1.826 métodos
de extensión** de los cuales **3.436 firmas** devuelven `Task<...>`. Esa combinación tiene un coste
medible: una asignación en el montón por cada paso de la cadena y una máquina de estados `async`
por cada combinador que realmente espera.

Pero no todas las firmas `Task<...>` cuestan lo mismo. Una parte sustancial **no realiza ninguna
operación asíncrona**: envuelve un resultado ya calculado en una `Task` completada. Este documento
empieza cuantificando exactamente ese subconjunto, porque es el único hallazgo de rendimiento que
se ha cerrado documentalmente en la versión 2.1.1 (US-16).

---

## 2. Inventario · métodos `*Async` que no son asíncronos

### 2.1. Criterio de detección

Se considera que un método `*Async` **no es asíncrono** cuando cumple las tres condiciones:

1. Su firma es `public static Task<...>` **sin** el modificador `async`.
2. Su nombre termina en `Async`.
3. Su cuerpo (las 8 primeras líneas tras la firma) contiene `ToAsync()`, `Task.FromResult` o
   `Task.CompletedTask`.

`ToAsync()` es la extensión del propio núcleo (`Helpers/Extensions/ParallelExtensions.cs`) que no
es más que un `Task.FromResult`, de modo que las tres marcas son equivalentes: el resultado está
disponible antes de construir la tarea.

El recuento se ha obtenido con un script determinista sobre los 40 ficheros `.cs` del proyecto
(excluyendo `bin/` y `obj/`). **No es una estimación.**

### 2.2. Resultado por fichero

**Total: 248 sobrecargas**, repartidas en 14 ficheros.

| Fichero | Nº | Métodos (nº de sobrecargas afectadas) |
|---------|---:|---------------------------------------|
| `Types/MlResultBindExtensions.cs` | 81 | `BindAlwaysAsync` (x2), `BindAsync` (x1), `BindIfAsync` (x2), `BindIfFailAsync` (x2), `BindIfFailWithExceptionAsync` (x4), `BindIfFailWithExceptionErrorAsync` (x4), `BindIfFailWithoutExceptionAsync` (x4), `BindIfFailWithValueAsync` (x2), `BindMultiAsync` (x3), `BindSaveValueInDetailsIfFailedFuncResultAsync` (x1), `TryBindAlwaysAsync` (x3), `TryBindAsync` (x2), `TryBindBuildTupleAsync` (x13), `TryBindIfAsync` (x4), `TryBindIfFailAsync` (x4), `TryBindIfFailWithExceptionAsync` (x8), `TryBindIfFailWithExceptionErrorAsync` (x8), `TryBindIfFailWithoutExceptionAsync` (x8), `TryBindIfFailWithValueAsync` (x4), `TryBindSaveValueInDetailsIfFailedFuncResultAsync` (x2) |
| `Types/MlResultMapExtensions.cs` | 66 | `MapAlwaysAsync` (x2), `MapAsync` (x1), `MapEnsureAsync` (x3), `MapIfAsync` (x2), `MapIfFailAsync` (x2), `MapIfFailWithExceptionAsync` (x4), `MapIfFailWithExceptionErrorAsync` (x4), `MapIfFailWithoutExceptionAsync` (x4), `MapIfFailWithValueAsync` (x2), `TryMapAlwaysAsync` (x4), `TryMapAsync` (x2), `TryMapIfAsync` (x4), `TryMapIfFailAsync` (x4), `TryMapIfFailWithExceptionAsync` (x8), `TryMapIfFailWithExceptionErrorAsync` (x8), `TryMapIfFailWithoutExceptionAsync` (x8), `TryMapIfFailWithValueAsync` (x4) |
| `Types/MlResultSeveralExtensions.cs` | 22 | `CombineAllAsync` (x14), `FailIfEmptyAsync` (x3), `FailIfFalseAsync` (x5) |
| `Types/MlResultConversions.cs` | 19 | `FromMlResultObjectAsync` (x1), `SecureGetValueFromMlResultBoxedAsync` (x1), `ToMlResultFailAsync` (x14), `ToMlResultObjectAsync` (x2), `ToMlResultValidAsync` (x1) |
| `Types/MlResultExecSelfExtensions.cs` | 17 | `ExecSelfAsync` (x2), `ExecSelfIfFailAsync` (x1), `ExecSelfIfFailWithExceptionAsync` (x1), `ExecSelfIfFailWithoutExceptionAsync` (x1), `ExecSelfIfFailWithValueAsync` (x1), `ExecSelfIfValidAsync` (x1), `TryExecSelfAsync` (x2), `TryExecSelfIfFailAsync` (x2), `TryExecSelfIfFailWithExceptionAsync` (x2), `TryExecSelfIfFailWithValueAsync` (x2), `TryExecSelfIfValidAsync` (x2) |
| `Types/MlResult.cs` | 12 | `EmptyAsync` (x1), `FailAsync` (x10), `ValidAsync` (x1) |
| `Types/MlResultExtensions.cs` | 8 | `AddMlErrorDetailIfFailAsync` (x1), `AddValueDetailIfFailAsync` (x1), `CompleteWithDataValueAsync` (x1), `CompleteWithDataValueIfValidAsync` (x1), `CompleteWithDetailsValueIfFailAsync` (x1), `CreateCompleteMlResultAsync` (x2), `GetFailureDetailsOrThrowAsync` (x1) |
| `Types/MlResultCollections.cs` | 8 | `MergeErrorsIfExistsAsync` (x1), `MergeFailureErrorsAsync` (x1), `TraverseAsync` (x2), `TraverseSplitAsync` (x1), `TraverseSplitWithKeysAsync` (x1), `TraverseWhileAsync` (x2) |
| `Types/MlResultMatchExtensions.cs` | 5 | `MatchAsync` (x2), `TryMatchAsync` (x3) |
| `Types/Errors/MlErrorDetailsActions.cs` | 3 | `ContainsExceptionAsync` (x1), `ContainsValueMetadataAsync` (x1), `MergeErrorsDetailsAsync` (x1) |
| `Types/MlResultErrorDetailsExtensions.cs` | 3 | `GetExceptionAsync` (x2), `GetMetadataValueAsync` (x1) |
| `Helpers/EnsureFp.cs` | 2 | `ThatAsync` (x2) |
| `Helpers/Extensions/ParallelExtensions.cs` | 1 | `ToAsync` (x1) |
| `Helpers/Extensions/Extensions.cs` | 1 | `WithAsync` (x1) |
| **Total** | **248** | |

Puesto en contexto de la superficie total:

| Métrica | Valor |
|---------|------:|
| Firmas `*Async` del núcleo | 3.436 |
| De ellas, **sin asincronía real** | **248** |
| Porcentaje | **7,2 %** |

Los dos ficheros gigantes concentran **147 de las 248** (59 %), lo cual es coherente con que
también concentren el 55,9 % de las líneas del proyecto.

### 2.3. Casos de referencia

Los dos casos citados explícitamente en la US-16 son el patrón canónico:

```csharp
// Types/MlResultBindExtensions.cs — BindAsync
public static Task<MlResult<TResult>> BindAsync<T, TResult>(this MlResult<T>                source,
                                                                 Func<T, MlResult<TResult>> func)
    => source.Bind(func).ToAsync();

// Types/MlResultBindExtensions.cs — TryBindAsync
public static Task<MlResult<TResult>> TryBindAsync<T, TResult>(this MlResult<T>                source,
                                                                    Func<T, MlResult<TResult>> func,
                                                                    string                     exceptionAdditionalMessage = null!)
    => source.TryBind(func, exceptionAdditionalMessage).ToAsync();
```

En ambos el origen es un `MlResult<T>` **sincrónico** y el delegado devuelve un `MlResult<TResult>`
**sincrónico**. No hay nada que esperar: el `ToAsync()` final es un `Task.FromResult`.

Compárese con el hermano que **sí** es asíncrono:

```csharp
public static async Task<MlResult<TResult>> BindAsync<T, TResult>(this Task<MlResult<T>>          sourceAsync,
                                                                       Func<T, MlResult<TResult>> func)
{
    var source = await sourceAsync;   // <- aquí sí hay espera real
    return source.Bind(func);
}
```

La diferencia la marca el **origen** (`MlResult<T>` frente a `Task<MlResult<T>>`) y el **delegado**
(`Func<..., MlResult<...>>` frente a `Func<..., Task<MlResult<...>>>`).

### 2.4. Implicaciones de rendimiento

| Aspecto | Impacto |
|---------|---------|
| **Asignación** | `Task.FromResult` asigna un `Task<T>` en el montón salvo para los valores en caché (`bool`, `int` pequeños, `null`). Con `MlResult<T>` **siempre** asigna. |
| **Máquina de estados** | No se genera (no hay `async`), de modo que el coste se limita al objeto `Task`. Es el caso «barato» de los dos. |
| **`await` en el consumidor** | El `await` de una tarea ya completada se resuelve de forma sincrónica, sin cambio de contexto ni salto al `ThreadPool`. El coste es el del `AsyncStateMachine` del **consumidor**, no de la librería. |
| **Cancelación** | Inexistente. No aceptan `CancellationToken` y no habría dónde observarlo: el trabajo ya está hecho cuando se construye la tarea. |
| **`ConfigureAwait`** | Irrelevante en estas sobrecargas: al estar la tarea completada no hay captura de `SynchronizationContext`. Sí es relevante en las 3.188 restantes (ver el documento `02-Interoperabilidad-BCL.md`, pendiente de redacción). |

Conclusión: **no son un problema de rendimiento grave**, sino de **expectativas**. El consumidor
que ve `BindAsync` asume E/S, cancelación y posible cambio de hilo, y ninguna de las tres cosas
ocurre. De ahí que la acción de la 2.1.1 sea documental y no estructural.

### 2.5. Acciones aplicadas en la 2.1.1

Cerradas en la **US-16** de la Épica 03:

1. **`<returns>` explícito en las 248 sobrecargas.** Todas incorporan ahora un `<para>` dentro de
   su `<returns>` con el texto:

   > La tarea devuelta **ya está completada**: este método no realiza ninguna operación asíncrona.
   > El sufijo `Async` existe para permitir iniciar una cadena que continúa con combinadores
   > asíncronos. No admite cancelación.

   Al estar en el XML de documentación, IntelliSense lo muestra en el editor del consumidor sin
   que éste tenga que leer este documento.

2. **`<remarks>` de familia a nivel de clase** en las 6 clases que exponen variantes `Try*`
   (`EnsureFp`, `MlResultBindExtensions`, `MlResultConversions`, `MlResultExecSelfExtensions`,
   `MlResultMapExtensions`, `MlResultMatchExtensions`), que explica además la semántica divergente
   del prefijo `Try*` respecto a la BCL.

3. **Sección «Convenios de nomenclatura»** en el
   [`README.md` del núcleo](../../../MoralesLarios.OOFP.Core/README.md#15-convenios-de-nomenclatura),
   con las dos semánticas y la tabla de señales para distinguir una sobrecarga realmente asíncrona
   de una que no lo es.

4. **`TryGetValue` / `TryGetErrors`** en `Types/MlResult.TryGet.cs`, los dos únicos miembros del
   núcleo con la semántica `Try*` clásica de la BCL (decisión **D-13**), como vía de escape para el
   consumidor que esperaba ese contrato.

### 2.6. Acciones aplazadas

| Acción | Versión | Motivo del aplazamiento |
|--------|---------|-------------------------|
| Devolver `ValueTask<MlResult<T>>` en lugar de `Task<MlResult<T>>` en estas 248 sobrecargas | 3.0.0 | Cambia la firma pública → rompe la compilación de los 29 proyectos consumidores y de todo el código externo. Requiere análisis de si `ValueTask` se puede esperar más de una vez en las cadenas actuales. |
| Eliminar las sobrecargas y obligar a un `.ToAsync()` explícito en el punto de entrada de la cadena | 3.0.0 | Reduciría la superficie pública en 248 miembros, pero rompe toda cadena existente que las use. |
| Cachear la `Task` completada para los resultados sin valor (`Discard`) | 3.0.0 | Depende de que `Discard` sea ya el miembro canónico (US-12) y de la medición con BenchmarkDotNet. |
| Aceptar `CancellationToken` por coherencia de firma | — | **Descartado.** Sería un parámetro inobservable: el trabajo ya está hecho. Documentar que no hay cancelación es más honesto que aceptar un token y ignorarlo. |

---

## 3. Hallazgos pendientes de análisis

El resto de los hallazgos de rendimiento recogidos en el
[README de mejoras](README.md#4-índice-de-documentos) siguen **pendientes de desarrollo
documental** y no forman parte del alcance de la 2.1.1:

| # | Hallazgo | Riesgo | Versión |
|--:|----------|:------:|---------|
| 1 | `MlResult<T>` es `record` (heap): una asignación por paso de cadena → evaluar `readonly struct` | 🔴 | 3.0.0 |
| 2 | Camino feliz sin asignaciones: evitar construir `MlErrorDetails` cuando el resultado es válido | 🟡 | 3.0.0 |
| 3 | Cierres (*closures*) implícitos en los delegados de los combinadores | 🟢 | 3.0.0 |
| 4 | `[MethodImpl(MethodImplOptions.AggressiveInlining)]` en los combinadores triviales | 🟢 | 2.2.0 |
| 5 | `ArrayPool<T>` en las operaciones de colección (`Traverse*`, `CombineAll`) | 🟡 | 3.0.0 |
| 6 | Línea base con **BenchmarkDotNet** antes de cualquier cambio estructural | 🟢 | 2.2.0 |
| 7 | `[DebuggerDisplay]` en `MlResult<T>`, `MlError`, `MlErrorDetails` (coste cero, ganancia de DX) | 🟢 | 2.2.0 |
| 8 | Evaluación perezosa de `MlErrorDetails.Errors` (`IEnumerable` que se puede recorrer dos veces) | 🟠 | 3.0.0 |

> ⚠️ **Regla de oro** · ninguno de los hallazgos 1, 2, 5 y 8 debe abordarse sin la línea base de
> BenchmarkDotNet del hallazgo 6. El coste real del `record` frente al `struct` depende del tamaño
> de `T` y del número de pasos de la cadena, y sin medición es especulación.

---

## 4. Ver también

- [README de mejoras del núcleo](README.md) — índice de los 8 documentos y matriz de prioridad.
- [Épica 03 · Miembros públicos y firmas](../../Next_Versions/2.1.1/Core/Epica-03-Miembros-Publicos-y-Firmas.md)
  — US-16, que cierra la parte documental.
- [`README.md` del núcleo · §15 Convenios de nomenclatura](../../../MoralesLarios.OOFP.Core/README.md#15-convenios-de-nomenclatura)
  — la documentación de cara al consumidor.
