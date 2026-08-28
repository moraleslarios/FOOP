# MoralesLarios.OOFP — Documentación por tipos (`Types`)

Esta sección documenta la librería **por clase/archivo**, y complementa la documentación por conceptos (`../Bind`, `../Map`, `../Match`, `../ExecSelf`, `../Several`, `../Bucle`, `../Transformations`, `../EnsureFp`, `../Extensions`).
- Si buscas **"qué métodos existen en un archivo concreto"** → usa esta sección.
- Si buscas **"cómo se usa una familia de métodos en profundidad"** → usa la documentación por conceptos.

---

## Índice de tipos documentados

| Documento | Archivo de código | Qué resuelve |
|---|---|---|
| [`MlResult` y `MlResult<T>`](./MlResult.md) | `Types/MlResult.cs` | El tipo raíz: éxito con valor o fallo con errores. |
| [Modelo de errores](./MlResultErrors.md) | `Types/Errors/*.cs` | `MlError`, `MlErrorsDetails`, `MlErrorsDetailsActions`, `ErrorMessage`. |
| [`MlResultActionsBind`](./MlResultActionsBind.md) | `Types/MlResultActionsBind.cs` | Encadenar operaciones que devuelven `MlResult<T>` (mónada). |
| [`MlResultActionsMap`](./MlResultActionsMap.md) | `Types/MlResultActionsMap.cs` | Transformar el valor sin salir del raíl (functor). |
| [`MlResultActionsMatch`](./MlResultActionsMatch.md) | `Types/MlResultActionsMatch.cs` | Salir del raíl y materializar un valor final. |
| [`MlResultActionsExecSelf`](./MlResultActionsExecSelf.md) | `Types/MlResultActionsExecSelf.cs` | Efectos laterales (log, auditoría) preservando el resultado. |
| [`MlResultActionsSeveral`](./MlResultActionsSeveral.md) | `Types/MlResultActionsSeveral.cs` | `EmptyToFailed`, `NullToFailed`, `BoolToResult`, `Combine`, `Do`. |
| [`MlResultActions`](./MlResultActions.md) | `Types/MlResultActions.cs` | Enriquecer detalles, acumular datos y accesos "seguros" al valor. |
| [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) | `Types/MlResultActionsErrorsDetails.cs` | Leer/fusionar el diccionario `Details` de un fallo. |
| [`MlResultBucles`](./MlResultBucles.md) | `Types/MlResultBucles.cs` | Proyecciones sobre colecciones (`Projection*`) y fusión de errores. |
| [`MlResultChangeReturnResult`](./MlResultChangeReturnResult.md) | `Types/MlResultChangeReturnResult.cs` | Cambiar el tipo/estado devuelto conservando el contexto de error. |
| [`MlResultTransformations`](./MlResultTransformations.md) | `Types/MlResultTransformations.cs` | Puentes entre el mundo imperativo (excepciones) y `MlResult`. |

Helpers relacionados que viven fuera de `Types`:

- [`EnsureFp`](../EnsureFp/EnsureFp.md) → `Helpers/EnsureFp.cs`: precondiciones (`That`, `NotNull`, `NotEmpty`, ...).
- [Extensiones generales](../Extensions/Extensions.md) → `Helpers/Extensions/*.cs`: `ToAsync`, `With`, `ToFuncTask`, `ValidateObject`.

---

## Convención común de nombres

Casi todas las familias siguen el mismo esquema de composición de nombre:

```text
[Try] Operación [Contexto] [Async]
```

| Pieza | Significado | Ejemplo |
|---|---|---|
| *Operación* | La acción base. | `Bind`, `Map`, `Match`, `ExecSelf` |
| `Try` (prefijo) | Envuelve la ejecución en `try/catch`: la excepción se convierte en `Fail` y se guarda en `Details["Ex"]`. | `TryBind`, `TryMap` |
| `Async` (sufijo) | Acepta y/o devuelve `Task<...>`. | `BindAsync`, `MapAsync` |
| `If` | Se ejecuta condicionalmente según un predicado. | `BindIf`, `MapIf` |
| `IfFail` | Solo se ejecuta cuando el resultado es `Fail` (recuperación). | `BindIfFail`, `MapIfFail` |
| `IfFailWithValue` | Solo si es `Fail` **y** el fallo transporta un valor en `Details["Value"]`. | `MapIfFailWithValue` |
| `IfFailWithException` | Solo si es `Fail` **y** el fallo transporta una excepción en `Details["Ex"]`. | `BindIfFailWithException` |
| `IfFailWithoutException` | Solo si es `Fail` **y** el fallo **no** transporta excepción (error de negocio). | `ExecSelfIfFailWithoutException` |

### Por qué hay tantos overloads

Cada familia multiplica sus sobrecargas por tres ejes independientes:

1. **Origen**: `MlResult<T>` o `Task<MlResult<T>>`.
2. **Delegado**: síncrono (`Func<T, ...>`) o asíncrono (`Func<T, Task<...>>`).
3. **Mensaje de error** (solo en variantes `Try*`): `string` fijo o `Func<Exception, string>` para construirlo a partir de la excepción capturada.

Esto significa que **no necesitas adaptar tu código a la librería**: existe la sobrecarga que encaja con lo que ya tienes y el compilador la resuelve. En los documentos siguientes se muestran las combinaciones representativas, no las cientos de firmas literales.

---

## Regla de oro: no accedas al valor directamente

En `MlResult<T>`, tanto `Value` como `ErrorsDetails` son `internal protected`. Es intencional: **la forma soportada de salir del raíl es `Match`** (o `SecureValidValue` cuando ya has garantizado la validez).

```csharp
// ❌ No compila fuera de la librería
// var user = userResult.Value;

// ✅ Materializa el resultado decidiendo ambas ramas
IActionResult response = userResult.Match(
    valid: user   => Ok(user),
    fail : errors => BadRequest(errors.ToErrorsDescription()));
```

---

## Guías por concepto (documentación en profundidad)

Cada familia tiene además una guía extensa con ejemplos, particularidades del código fuente y
malas prácticas a evitar. **Si estás aprendiendo, empieza por aquí.**

### Punto de partida

- 📘 [**Introducción general**](../1_Intro.md) — filosofía, convención de nombres e
  [índice completo de los 48 documentos](../1_Intro.md#índice-completo-de-la-documentación)

### `Bind` — encadenar operaciones que devuelven `MlResult`

| Documento | Tema |
|-----------|------|
| [`2_MlResultActions`](../Bind/2_MlResultActions.md) | Utilidades base y acceso seguro al valor |
| [`3_Bind`](../Bind/3_Bind.md) | ⭐ `Bind` y `TryBind`: el operador fundamental |
| [`4_BindMulti`](../Bind/4_BindMulti.md) | Elegir rama según condiciones |
| [`5_BindIf`](../Bind/5_BindIf.md) | Ejecutar solo si se cumple un predicado |
| [`6_BindIfFail`](../Bind/6_BindIfFail.md) | Recuperación desde el fallo |
| [`7_BindIfFailWithValue`](../Bind/7_BindIfFailWithValue.md) | Recuperar con el valor original de `Details` |
| [`8_BindIfFailWithException`](../Bind/8_BindIfFailWithException.md) | Recuperar según la excepción capturada |
| [`9_BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md) | Fallos de negocio vs. fallos técnicos |
| [`10_BindAlways`](../Bind/10_BindAlways.md) | Ejecutar en ambas ramas |
| [`11_BindSaveValueInDetails…`](../Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md) | Guardar la entrada en `Details` al fallar |

### `Map` — transformar el valor sin salir del carril

| Documento | Tema |
|-----------|------|
| [`1_Map`](../Map/1_Map.md) | ⭐ `Map` y `TryMap`: transformación pura |
| [`2_MapEnsure`](../Map/2_MapEnsure.md) | Validar y transformar |
| [`3_MapIf`](../Map/3_MapIf.md) | Transformación condicional |
| [`4_MapIfFail`](../Map/4_MapIfFail.md) | Valor de reserva ante fallo |
| [`5_MapIfFailWithValue`](../Map/5_MapIfFailWithValue.md) | Reserva usando `Details["Value"]` |
| [`6_MapIfFailWithException`](../Map/6_MapIfFailWithException.md) | Reserva según `Details["Ex"]` |
| [`7_MapIfFailWithoutException`](../Map/7_MapIfFailWithoutException.md) | Reserva solo si no hay excepción |
| [`8_MapAlways`](../Map/8_MapAlways.md) | Transformar ambas ramas a un tipo común |

### `Match` — salir del carril

| Documento | Tema |
|-----------|------|
| [`1_Match`](../Match/1_Match.md) | ⭐ `Match` y `TryMatch`: materializar el resultado final |
| [`2_MatchAll`](../Match/2_MatchAll.md) | Sobrecargas «todo en uno» y patrones de salida |

### `ExecSelf` — efectos laterales sin alterar el resultado

| Documento | Tema |
|-----------|------|
| [`1_ExecSelf`](../ExecSelf/1_ExecSelf.md) | ⭐ `ExecSelf` y `TryExecSelf`: instrumentar la tubería |
| [`2_ExecSelfIfValid`](../ExecSelf/2_ExecSelfIfValid.md) | Solo en la rama válida |
| [`3_ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md) | Solo en la rama de fallo |
| [`4_ExecSelfIfFailWithValue`](../ExecSelf/4_ExecSelfIfFailWithValue.md) | Al fallar, con el valor original |
| [`5_ExecSelfIfFailWithException`](../ExecSelf/5_ExecSelfIfFailWithException.md) | Al fallar, con la excepción |
| [`6_ExecSelfIfFailWithoutException`](../ExecSelf/6_ExecSelfIfFailWithoutException.md) | Al fallar sin excepción |

### `Several` — puentes desde el mundo imperativo

| Documento | Tema |
|-----------|------|
| [`1_EmptyToFailed`](../Several/1_EmptyToFailed.md) | Rechazar colecciones vacías |
| [`2_NullToFailed`](../Several/2_NullToFailed.md) | Convertir `null` en fallo explícito |
| [`3_BoolToResult`](../Several/3_BoolToResult.md) | Convertir un `bool` en `MlResult` |
| [`4_Combine`](../Several/4_Combine.md) | `Combine` y `Do` ⚠️ (**no** acumula errores) |

### Utilidades y colecciones

| Documento | Tema |
|-----------|------|
| [`EnsureFp`](../EnsureFp/EnsureFp.md) | Precondiciones: `That`, `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace` |
| [`Transformations`](../Transformations/Transformations.md) | `ToMlResultValid`, `ToMlResultFail`, `TryToMlResult*` |
| [`Extensions`](../Extensions/Extensions.md) | `ToAsync`, `With`, `ToFuncTask`, `AppendExDetails`, `Constants` |
| [`Bucles`](../Bucle/Bucles.md) | `Projection`, `ProjectionWhile`, `ProjectionParallelAsync`, `ProjectionSplit` |

---

## Volver arriba

- 📘 [Introducción general de `MoralesLarios.OOFP`](../1_Intro.md)
- 📘 [README del proyecto `MoralesLarios.OOFP`](../../README.md)
- 📘 [README general de la solución](../../../README.md)
