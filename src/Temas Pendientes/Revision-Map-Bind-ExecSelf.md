# Revisión de `MlResultActionsMap`, `MlResultActionsBind` y `MlResultActionsExecSelf`

> 📌 **Qué es este documento**
> Cierre de la revisión exhaustiva de los tres ficheros de métodos extensores más grandes del núcleo
> (`MoralesLarios.OOFP`). Recoge **lo que se arregló** y, sobre todo, **lo que se decidió NO tocar y por qué**.
>
> Todo lo que queda abierto aquí es **source-breaking** para quien ya consume la biblioteca, así que
> requiere una decisión explícita del autor (y, probablemente, un salto de versión mayor).

---

## Índice

1. [Estado actual](#estado-actual)
2. [Ya cerrado](#ya-cerrado)
3. [🔴 R1 — Erratas en nombres públicos (no renombradas)](#-r1--erratas-en-nombres-públicos-no-renombradas)
4. [🟡 R2 — `TException` no se puede inferir](#-r2--texception-no-se-puede-inferir)
5. [🟡 R3 — Ambigüedad entre `string` y `Func<Exception, string>`](#-r3--ambigüedad-entre-string-y-funcexception-string)
6. [🟢 R4 — Restos menores](#-r4--restos-menores)
7. [Patrón de bug a vigilar en el futuro](#patrón-de-bug-a-vigilar-en-el-futuro)

---

## Estado actual

| Métrica | Antes | Después |
|---|---:|---:|
| Recursiones infinitas (`StackOverflowException`) | **~40** | **0** |
| Warnings de compilación de la librería | 27 | **4** |
| Tests de `MoralesLarios.OOFP.Unit.Tests` | 775 ✅ | **781 ✅** |

Los 4 warnings que quedan son `CS8604` en `Helpers/EnsureFp.Strings.cs` (líneas 264, 294, 324, 345) y son
**anteriores** a esta revisión: no tienen relación con `Map`/`Bind`/`ExecSelf`.

---

## Ya cerrado

- [x] **Recursiones infinitas.** Unas 40 sobrecargas cuyo `source` era `Task<MlResult<T>>` se llamaban a
      sí mismas porque la resolución de sobrecarga volvía a elegir el mismo método. Provocaban un
      `StackOverflowException` **no capturable** y **el compilador no avisaba**.
- [x] **`ExecSelfIfFailWithoutExceptionAsync<T>`** pasaba a la acción un `MlErrorsDetails` distinto al de
      la versión sincrónica: incluía un error sintético *"The key Ex does not exist in the details"*.
      Ahora ambas versiones se comportan igual. Cubierto por
      `MlResultActionsExecSelfFixesTests.cs`.
- [x] **`TryMap` / `TryBind` y familias `…IfFailWithValue`** usaban `source.Value` en lugar del `value`
      ya extraído por el `Match`, lo que reevaluaba el resultado innecesariamente.
- [x] **Restricciones genéricas ausentes**: `where TException : Exception` añadido donde faltaba.
- [x] **`BindExIfFailWithoutExceptionAsync` → `TryBindIfFailWithoutExceptionAsync`** (el prefijo `Ex` era
      una errata; el resto de la familia usa `Try`).
- [x] **Nullable espurio en `MlResultActionsSeveral.cs`**: las 12 sobrecargas `CombineAsync` con `source`
      de tipo tupla (aridades 3 a 8) declaraban `Task<MlResult<(...)>?>` cuando el `Combine` sincrónico al
      que delegan devuelve un `MlResult` **no** nullable. Eso generaba 23 `CS8619`, casi todos en los
      consumidores de `MlResultActionsBind.cs`. Al quitar el `?` desaparecieron los 23 warnings y también
      sobraron 4 `.ToAsync()!` que solo estaban para silenciarlos.
- [x] **Erratas internas** (parámetros y comentarios, sin impacto para el consumidor):
      `exceptionAditionalMessage` → `exceptionAdditionalMessage` (159), `funcValidAsyc` → `funcValidAsync`,
      `prevous` → `previous`, `TryMapIAsyncf` → `TryMapIfAsync`, `funcFailAlwaiys` → `funcFailAlways`.
- [x] **Código muerto**: bloques comentados que duplicaban métodos ya implementados, y una nota
      *"Está mal, hay que cambiar"* cuyo arreglo ya estaba aplicado.
- [x] **`string errorMessage = null!` + `_ => errorMessage!`**: homogeneizado en las ~30 sobrecargas que
      lo tenían a medias.

---

## 🔴 R1 — Erratas en nombres públicos (no renombradas)

- [ ] **R1. Tres erratas en identificadores públicos de `MlResult`**
    - **Proyecto:** `MoralesLarios.OOFP`
    - **Archivo / clase:** `Types/MlResultActionsBind.cs`, `Types/MlResultActionsMap.cs`,
      `Types/MlResultActionsErrorsDetails.cs`, `Types/MlResultBucles.cs`
    - **Miembro:**

      | Nombre actual | Nombre correcto | Ocurrencias |
      |---|---|---:|
      | `BindSaveValueInDetailsIfFaild…` | `BindSaveValueInDetailsIfFailed…` | 26 en librería + 17 en tests |
      | `MergeErrorsDetailsIfFailDiferentTypes` | `MergeErrorsDetailsIfFailDifferentTypes` | 24 en librería + 4 en tests |
      | `FusionFailErros` | `FusionFailErrors` | 6 en `Bind` + 30 en `MlResultBucles` + 15 en tests |

    - **Problema:** son **nombres de métodos públicos** mal escritos (`Faild`, `Diferent`, `Erros`).
      Quien lea el IntelliSense los ve tal cual.
    - **Impacto:** renombrarlos **rompe la compilación de cualquier consumidor**. No es un cambio
      cosmético: es un cambio de API.
    - **Por qué NO se ha hecho ahora:**
        1. Es `source-breaking`: exige un salto de versión mayor y una nota de migración.
        2. `FusionFailErros` está mayoritariamente en `MlResultBucles.cs`, que en este momento es
           **trabajo en curso del autor**; tocarlo generaría conflictos.
    - **Propuesta (migración sin romper a nadie):**
        1. Añadir el método con el **nombre correcto** delegando en el actual (una línea, `=>`).
        2. Marcar el nombre viejo con
           `[Obsolete("Usa BindSaveValueInDetailsIfFailed…. Este nombre se eliminará en la v3.", false)]`.
        3. Mantener ambos durante una versión mayor completa.
        4. Eliminar el viejo en la siguiente mayor.
    - **Nota:** este punto se solapa con
      [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md); consolidar allí antes de ejecutarlo.

---

## 🟡 R2 — `TException` no se puede inferir

- [ ] **R2. Los parámetros genéricos `TException` no aparecen en la firma, así que nunca se infieren**
    - **Proyecto:** `MoralesLarios.OOFP`
    - **Archivo / clase:** `Types/MlResultActionsBind.cs`, `Types/MlResultActionsMap.cs`,
      `Types/MlResultActionsExecSelf.cs`
    - **Miembro:** todas las familias `…IfFailWithException<T, TReturn, TException>` y
      `…IfFailWithoutException<T, TReturn, TException>`
    - **Problema:** `TException` solo se usa **dentro del cuerpo** (en
      `errorsDetails.GetDetailException<TException>()`), nunca en un parámetro. C# no puede inferirlo, así
      que el consumidor está **obligado** a escribir los tres argumentos genéricos:

      ```csharp
      // no compila: CS0411
      result.TryBindIfFailWithoutException(funcValid, funcFail);

      // única forma válida hoy
      result.TryBindIfFailWithoutException<int, string, InvalidOperationException>(funcValid, funcFail);
      ```

    - **Impacto:** la API es incómoda y los mensajes de error (`CS0411 - no se pueden inferir los
      argumentos de tipo`) no explican qué falta. En las pruebas de esta revisión hubo que anotar el tipo
      explícitamente en todos los casos.
    - **Propuesta:** ninguna opción es gratis, hay que elegir:
        1. **Sobrecarga con `Exception` por defecto** — añadir una variante `<T, TReturn>` que llame a
           `<T, TReturn, Exception>`. **No rompe nada** y cubre el caso mayoritario. *Recomendada.*
        2. **Parámetro testigo** — `Func<TException, …>` en la firma para que se infiera. Es más limpio
           conceptualmente pero cambia la firma.
        3. **Dejarlo y documentarlo** — añadir `<remarks>` en el XML doc indicando que `TException` debe
           especificarse siempre.

---

## 🟡 R3 — Ambigüedad entre `string` y `Func<Exception, string>`

- [ ] **R3. Un `null` literal como último argumento es ambiguo**
    - **Proyecto:** `MoralesLarios.OOFP`
    - **Archivo / clase:** `Types/MlResultActionsBind.cs`, `Types/MlResultActionsMap.cs`,
      `Types/MlResultActionsExecSelf.cs`
    - **Miembro:** todos los pares de sobrecargas `Try*` que existen dos veces, una con
      `Func<Exception, string> errorMessageBuilder` y otra con `string errorMessage = null!`
    - **Problema:** al pasar `null` explícitamente el compilador no puede decidir cuál de las dos
      sobrecargas aplicar (`CS0121`), porque `null` es convertible a ambas:

      ```csharp
      result.TryBind(func, null); // CS0121: la llamada es ambigua
      ```

    - **Impacto:** confunde a quien viene de una API con un solo parámetro de mensaje. En la práctica se
      resuelve escribiendo `(string)null!` o `(Func<Exception, string>)null!`, que es ruido.
    - **Propuesta:** también hay que elegir, y las dos primeras son `source-breaking`:
        1. **Quitar el valor por defecto** de la sobrecarga `string` (obliga a pasarlo siempre, elimina la
           ambigüedad). Rompe a quien hoy omite el argumento.
        2. **Unificar** en un solo parámetro `Func<Exception, string>` y ofrecer un helper
           `ErrorMessage.Fixed("texto")`. Es el diseño más limpio a largo plazo, pero es el cambio más
           grande.
        3. **Documentarlo** en el XML doc de las sobrecargas afectadas indicando que, si se quiere pasar
           `null`, hay que castear. *Es lo que se ha hecho de momento: nada.*

---

## 🟢 R4 — Restos menores

- [ ] **R4a. `MapAlways<T, TResult>` (versión de dos funciones) no valida `null`**
    - **Archivo:** `Types/MlResultActionsMap.cs`, región `MapAlways`
    - **Problema:** el resto de la familia comprueba los delegados; esta sobrecarga no.
    - **Impacto:** `NullReferenceException` en lugar de un `MlResult` en fallo.

- [ ] **R4b. Asimetría de sobrecargas en `TryMapAlways<T, TReturn>`**
    - **Archivo:** `Types/MlResultActionsMap.cs`, región `MapAlways`
    - **Problema:** faltan combinaciones sync/async que sí existen en las familias hermanas.
    - **Impacto:** el consumidor tiene que envolver a mano con `.ToAsync()` o `.ToFuncTask()`.

- [ ] **R4c. `GetDetailValue<T>().BindAsync(...)` debería ser `GetDetailValueAsync<TValue>()`**
    - **Archivo:** `Types/MlResultActionsBind.cs`, región `BindIfFailWithValue`
    - **Problema:** se usa la versión sincrónica y luego se encadena `BindAsync`, en lugar de la variante
      asíncrona que ya existe.
    - **Impacto:** solo estilo y coherencia; el resultado es correcto.

- [ ] **R4d. Indentación inconsistente**
    - **Archivos:** `Types/MlResultActionsMap.cs` (~1927 en columna 0, ~2705 con 2 espacios),
      `Types/MlResultActionsBind.cs` (~3835, ~3877, ~3960 y el bloque de métodos privados de la región
      `TryBindBuildTuple`)
    - **Problema:** rompe la alineación en columnas que sigue el resto del fichero.
    - **Impacto:** puramente visual. **Ojo:** cualquier arreglo debe preservar la alineación en columnas
      de los parámetros, que es la convención del proyecto.

---

## Patrón de bug a vigilar en el futuro

Es la causa raíz de las ~40 recursiones y **el compilador no lo detecta**. Merece un test o un analizador.

Una sobrecarga cuyo `source` es `Task<MlResult<T>>` y que reenvía a otro método **con el mismo nombre**
debe cambiar **algo** en la llamada, o la resolución de sobrecarga volverá a elegirse a sí misma:

```csharp
// ❌ recursión infinita: mismos tipos de argumento, mismo nombre → se elige a sí mismo
public static async Task<MlResult<TReturn>> XxxAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                     Func<T, TReturn>   funcValid)
    => await sourceAsync.XxxAsync(funcValid);

// ✅ desreferenciando el Task: ahora el source es MlResult<T> y aplica la sobrecarga sincrónica
    => (await sourceAsync).Xxx(funcValid);
```

Las tres únicas salidas válidas son:

| Situación | Arreglo |
|---|---|
| Existe la versión sincrónica con los mismos delegados | `=> (await sourceAsync).Xxx(...)` |
| El delegado es sincrónico y hace falta el asíncrono | `=> await sourceAsync.XxxAsync(func.ToFuncTask(), ...)` |
| El parámetro es `string` y la otra sobrecarga pide `Func<Exception, string>` | `=> (await sourceAsync).TryXxx(..., _ => errorMessage!)` |

Y un segundo patrón, más sutil: si la sobrecarga **no observa** `sourceAsync` (por ejemplo en la familia
`…Always`, donde el resultado previo no se usa), hay que consumirlo de todas formas para no dejar la tarea
sin await:

```csharp
// ❌ la tarea nunca se espera: excepciones perdidas
    => await funcAlwaysAsync();

// ✅
    { _ = await sourceAsync; return await funcAlwaysAsync(); }
```
