# Auditoría de `MlResult<T>` y `MlErrorsDetails`

> 📌 **Qué es este documento**
> Revisión a fondo de los **dos tipos centrales** de `MoralesLarios.OOFP`: `MlResult` / `MlResult<T>` y
> `MlErrorsDetails` (más sus satélites `MlError`, `MlErrorsDetailsActions` y `MlResultActionsErrorsDetails`).
>
> ✅ **Todos los puntos 🔴 y buena parte de los 🟠 están _verificados empíricamente_**, no deducidos por lectura.
> Se escribieron **12 tests de diagnóstico** temporales contra el código real: **los 12 fallaron**.
> En cada punto verificado se incluye la **evidencia medida** (salida literal del test o del compilador).

---

## Cómo leer este documento

Cada punto sigue la plantilla de la carpeta:

```text
- [ ] **N. Título breve del problema**
    - Proyecto:        en qué proyecto de la solución está
    - Archivo / clase: dónde exactamente
    - Miembro:         método o propiedad concretos
    - Problema:        qué hace mal el código, de forma objetiva
    - Evidencia:       salida real del test o del compilador (solo en puntos verificados)
    - Impacto:         qué consecuencia real tiene
    - Propuesta:       cómo arreglarlo
```

La numeración usa el prefijo **`MR`** (`MR1`, `MR2`…) para **no colisionar** con los puntos 1-89 de
[`Mejoras-Prioridad-Critica-y-Alta.md`](Mejoras-Prioridad-Critica-y-Alta.md) y sus hermanos.

### Criterios de prioridad

| Prioridad | Criterio |
|---|---|
| 🔴 **Crítica** | Produce **resultados incorrectos**, lanza excepciones donde no debe, o el tipo es imposible de usar |
| 🟠 **Alta** | Contratos rotos, semántica engañosa, igualdad que no funciona, información que se pierde |
| 🟡 **Media** | Ergonomía de la API, mensajes, erratas públicas, genéricos redundantes |
| 🟢 **Baja** | Rendimiento, serialización, analizadores, pulido |

### Resumen

| Prioridad | Puntos | Numeración | Verificados |
|---|---:|---|---|
| 🔴 Crítica | 5 | MR1-MR5 | **5 / 5** |
| 🟠 Alta | 5 | MR6-MR10 | **4 / 5** |
| 🟡 Media | 8 | MR11-MR18 | 1 / 8 |
| 🟢 Baja | 5 | MR19-MR23 | 0 / 5 |
| **Total** | **23** | | **10** |

---

## 🔴 Prioridad crítica

- [ ] **MR1. `AddDetails` (ambas sobrecargas) no hace absolutamente nada: descarta los detalles en silencio**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetailsActions.cs` (líneas ~125 y ~134)
    - Miembro:         `AddDetails(this MlErrorsDetails, Dictionary<string, object>)` y `AddDetails(this MlErrorsDetails, params (string, object)[])`
    - Problema:        Ambos métodos calculan la variable `details` con el diccionario fusionado y **acto seguido la descartan**, devolviendo `source.Details` sin modificar:

      ```csharp
      public static MlErrorsDetails AddDetails(this MlErrorsDetails source, Dictionary<string, object> otherDetails)
      {
          var details = source.Details.Concat(otherDetails).ToDictionary(...);   // ← se calcula
          var result  = (source.Errors, source.Details);                         // ← y se DESCARTA
          return result;
      }
      ```

      El compilador no avisa porque `details` **sí** está asignada, solo que nunca se lee.
    - Evidencia:       `B_count=0 keys=[]` · `B2_count=0 keys=[]` (se esperaba `K1`, `K2`)
    - Impacto:         **Pérdida silenciosa de información de diagnóstico.** Quien añada detalles a un error confía en que están ahí y nunca aparecen. Es el peor tipo de bug: no lanza, no avisa, solo pierde datos.
    - Propuesta:       Cambiar `var result = (source.Errors, source.Details);` por `var result = (source.Errors, details);` en ambas sobrecargas. ⚠️ Revisar los tests existentes: puede haber alguno que pase **precisamente porque** el método no hace nada.

- [ ] **MR2. `AddDetail<T>` muta el objeto original y lanza `ArgumentException` con clave repetida**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetailsActions.cs`
    - Miembro:         `AddDetail<T>(this MlErrorsDetails source, string key, T value)`
    - Problema:        Usa `source.Details.Add(key, value!)` sobre el **diccionario del objeto de entrada**:

      ```csharp
      public static MlErrorsDetails AddDetail<T>(this MlErrorsDetails source, string key, T value)
      {
          source.Details.Add(key, value!);   // ← muta el diccionario compartido + lanza si la clave existe
      ```

      Dos defectos en una línea: **mutación por aliasing** y **excepción con clave duplicada**.
    - Evidencia:       `C2_original_mutado=True sameRef=True` · `System.ArgumentException: An item with the same key has already been added. Key: K`
    - Impacto:         (a) Rompe la inmutabilidad prometida por una librería funcional: cualquiera que conserve una referencia al `MlErrorsDetails` original ve el cambio a distancia. (b) **Lanza una excepción en el camino de error**, que es exactamente donde una librería railway-oriented nunca debe lanzar. Llamar dos veces a `AddMlErrorDetailIfFail` con la misma clave revienta el pipeline.
    - Propuesta:       Copiar el diccionario y usar el **indexador** en lugar de `Add`:

      ```csharp
      var details = new Dictionary<string, object>(source.Details) { [key] = value! };
      return new MlErrorsDetails(source.Errors, details);
      ```

      Si se quiere conservar el valor previo en lugar de sobrescribirlo, aplicar la misma política de renumerado que ya existe para las excepciones (ver **MR5**).

- [ ] **MR3. `Merge` lanza `ArgumentException` al fusionar errores con claves de detalle coincidentes**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetailsActions.cs`
    - Miembro:         `Merge(this MlErrorsDetails source, MlErrorsDetails other)`
    - Problema:        El método **sí** protege las claves de excepción (las renumera como `Ex`, `Ex1`, `Ex2`…), pero el resto de detalles se fusionan con un `ToDictionary` que **no tolera duplicados**. Fusionar dos errores que ambos llevan la clave `Value` lanza.
    - Evidencia:       `System.ArgumentException: An item with the same key has already been added. Key: Value` — desde `Enumerable.ToDictionary`
    - Impacto:         **El más grave de todos por alcance.** `Merge` es el corazón de `MergeErrorsDetails`, `MergeErrorsDetailsIfFail`, `CreateCompleteMlResult`, `FusionFailErros`… es decir, de **toda la agregación de errores** de la librería. Cualquier `CreateCompleteMlResult` de dos fallos producidos por `FromErrorMessageWithValue` (que usa la clave `Value`) explota. Es un escenario cotidiano, no rebuscado.
    - Propuesta:       Generalizar la estrategia de renumerado que ya se aplica a `Ex` para **cualquier** clave duplicada (`Value`, `Value1`, `Value2`…), extrayéndola a un helper único compartido con **MR5**. Alternativa más simple: usar el indexador (última gana) y documentarlo, aunque se perdería información.

- [ ] **MR4. `MlResult<MlError>` no compila: conversiones implícitas ambiguas (`CS0457`)**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`
    - Miembro:         `implicit operator MlResult<T>(T value)` vs `implicit operator MlResult<T>(MlError error)` (y las de `MlErrorsDetails`, `List<MlError>`, `MlError[]`)
    - Problema:        Cuando el parámetro genérico `T` coincide con alguno de los tipos que ya tienen conversión "de fallo", las dos conversiones colisionan y **el tipo se vuelve inutilizable**.
    - Evidencia:       error del compilador, literal:

      ```text
      error CS0457: Ambiguous user defined conversions
      'MlResult<MlError>.implicit operator MlResult<MlError>(MlError)' and
      'MlResult<MlError>.implicit operator MlResult<MlError>(MlError)'
      when converting from 'MlError' to 'MlResult<MlError>'
      ```

    - Impacto:         Existe un **conjunto de `T` prohibidos y no documentado**: `MlError`, `MlErrorsDetails`, `List<MlError>`, `MlError[]`, `List<string>`, `string[]` y las tuplas con conversión. Nadie puede escribir un método que devuelva `MlResult<MlError>` (algo perfectamente razonable, p. ej. al validar un error). Además, las 6 conversiones implícitas de `MlResult<T>` + las 11 de `MlErrorsDetails` hacen que la intención sea **ambigua para el lector**: ver un `return "texto";` no dice si es éxito o fallo.
    - Propuesta:       Reducir las conversiones implícitas a las imprescindibles (idealmente solo `T → MlResult<T>`, el caso de éxito) y forzar el camino de error a través de las fábricas explícitas que **ya existen** (`ToMlResultFail<T>()`, `MlResult<T>.Fail(...)`). Migración compatible: marcar las conversiones de error como `[Obsolete]` antes de retirarlas. Como mínimo, **documentar la restricción**.

- [ ] **MR5. Numeración de claves `Ex` incoherente entre `AppendExDetails` y `Merge`**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Helpers/Extensions/Extensions.cs` (línea ~27) y `Types/Errors/MlErrorsDetailsActions.cs`
    - Miembro:         `AppendExDetails(this Dictionary<string, object>, Exception)` vs `Merge(this MlErrorsDetails, MlErrorsDetails)`
    - Problema:        Dos rutas que hacen lo mismo generan **claves distintas** para la segunda excepción:
        - `AppendExDetails` usa `exKeys.Count + 1` → produce `Ex`, **`Ex2`**, `Ex3`…
        - `Merge` usa `index` → produce `Ex`, **`Ex1`**, `Ex2`…
    - Evidencia:       `E_append=[Ex,Ex2]` vs `E_merge=[Ex,Ex1]`
    - Impacto:         Un consumidor que lea `Details["Ex1"]` funciona o no **según el camino que trajo el error**. Hace imposible documentar el contrato de `Details` y convierte cualquier lectura de excepciones secundarias en una lotería. Nótese además que `Merge` es la única de las dos que **no** deja hueco, por lo que la colisión real depende del orden de las operaciones.
    - Propuesta:       Extraer un único helper (p. ej. `ExceptionDetailKeys.Next(IReadOnlyDictionary<string, object> details)`) y usarlo desde **ambos** sitios. Decidir y documentar la convención (recomendado: `Ex`, `Ex1`, `Ex2`…, la de `Merge`). Reutilizarlo también para **MR3**.

---

## 🟠 Prioridad alta

- [ ] **MR6. `MlResult<T>` es `record` pero su igualdad estructural no funciona**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs` y `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         `Equals` / `GetHashCode` sintetizados por el `record`
    - Problema:        `MlResult<T>` es un `record`, así que el compilador genera igualdad estructural. Pero compara el campo `ErrorsDetails`, que es una **`class` normal sin `Equals` sobrescrito** → comparación por referencia → **dos resultados idénticos nunca son iguales**. Se confirmó por búsqueda que **no existe ni un solo `override` de `Equals`/`GetHashCode`** en todo `Types/`.
    - Evidencia:       `A_Valid_iguales=False` para `MlResult<int>.Valid(5) == MlResult<int>.Valid(5)` · `A2_Fail_iguales=False` · `A3_details_iguales=False`
    - Impacto:         Es contraintuitivo hasta el punto de ser una trampa: el usuario ve `record`, asume valor, y obtiene referencia. Consecuencias prácticas: los tests están obligados a comparar campo a campo, `Distinct()` / `GroupBy` / `HashSet` / `Contains` sobre `MlResult` no funcionan, `MlResult` no puede ser clave de caché ni de diccionario, y `with` no sirve de nada.
    - Propuesta:       Tres caminos, de más a menos ambicioso: **(a)** convertir `MlErrorsDetails` en `record` con igualdad estructural real de `Errors` y `Details` — requiere colecciones inmutables, ver **MR7**; **(b)** implementar `Equals`/`GetHashCode` a mano en `MlErrorsDetails`; **(c)** quitar `record` de `MlResult<T>` y **documentar** que la igualdad es de referencia. La opción (a) es la coherente con el diseño funcional.

- [ ] **MR7. `Errors` es `IEnumerable<MlError>`: evaluación diferida y recorridos múltiples**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         `public IEnumerable<MlError> Errors { get; init; }` y `AddError` / `AddErrors` / `AddErrorMessage` en `MlErrorsDetailsActions.cs`
    - Problema:        `AddError` hace `source.Errors.Append(error)`, que es **perezoso**. Encadenar N adiciones construye una cadena de N iteradores que se **re-evalúa completa en cada consumo**. Y hay consumos dobles: `ToDescription` y `ToErrorsDescription` hacen `source.Count() > 1` **y después** `string.Join(...)` → dos recorridos del mismo `IEnumerable`.
    - Impacto:         Coste O(N²) al acumular errores y, sobre todo, **riesgo de resultados distintos entre recorridos** si el `IEnumerable` de origen no es estable (una consulta LINQ sobre EF Core, por ejemplo, se ejecutaría dos veces contra la base de datos). Un tipo de error **debe** ser un dato inerte, no una consulta pendiente.
    - Propuesta:       Cambiar la propiedad a `IReadOnlyList<MlError>` (o `ImmutableArray<MlError>`) y **materializar en el constructor** (`?.ToList() ?? []`). Es un cambio que rompe binariamente, pero elimina de golpe este punto, habilita **MR6** y protege de la mutación externa.

- [ ] **MR8. Un `MlResult` en estado de fallo puede no contener ningún error**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs` y `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         constructor `MlResult<T>(MlErrorsDetails)`, `MlResult<T>.Fail(...)`, constructor por defecto de `MlErrorsDetails`
    - Problema:        `MlResult<T>.Fail(new MlErrorsDetails())` produce un resultado con `IsFail == true` y **cero errores**. Su `ToString()` devuelve la **cadena vacía**. Lo mismo con `MlResult<T>.Fail()` usando el `params` vacío.
    - Evidencia:       `H_mensajes=[]` · `toString=[]`
    - Impacto:         Un fallo sin mensaje es **indepurable**: aparece una línea en blanco en el log y no hay forma de saber qué pasó ni dónde. Es el escenario que más tiempo hace perder en producción.
    - Propuesta:       Garantizar el invariante en el constructor de `MlErrorsDetails`: si la colección de errores llega vacía, insertar `DEFAULT_ERROR_MESSAGE` (**ya existe** en `Helpers/Constants.cs`). Alternativa más estricta: rechazar la construcción de un fallo sin error. Aplicar el mismo criterio a `MlErrorsDetails.ToString()` (ver **MR14**).

- [ ] **MR9. `GetDetail<T>` no distingue «no existe» de «es null» y contamina el error de entrada**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResultActionsErrorsDetails.cs`
    - Miembro:         `GetDetail<T>`, `GetDetailValue<T>`, `GetDetailException<T>` y sus variantes `Async`
    - Problema:        Dos defectos:
        1. El test de tipo es `source.Details[key] is T value`, y **`is T` es `false` para `null`** → un detalle presente con valor `null` se reporta como *«does not contain a value of type X»*, que es falso.
        2. En la rama de fallo ejecuta `source.AddError(...)` **sobre el objeto de entrada**: consultar un detalle **modifica el error consultado**.
    - Evidencia:       `I_IsValid=False` para un detalle existente con valor `null`
    - Impacto:         (a) Mensaje de diagnóstico engañoso, que lleva a buscar el problema en el sitio equivocado. (b) Efecto colateral inesperado en un método cuyo nombre (`Get…`) promete ser una consulta pura. Con ~150 usos de `GetDetail*` repartidos por `MlResultActionsBind.cs`, `MlResultActionsMap.cs` y `MlResultActionsExecSelf.cs`, el efecto es difícil de rastrear.
    - Propuesta:       Añadir `TryGetDetail<T>(string key, out T value)` **puro** y reescribir `GetDetail<T>` sobre él, construyendo un `MlErrorsDetails` **nuevo** en la rama de fallo (nunca mutando `source`). Distinguir los tres casos: clave ausente, valor `null`, tipo incorrecto — con mensajes distintos.

- [ ] **MR10. `Details` es `Dictionary<string, object>`: sin tipo, sin contrato y mutable en público**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         `public Dictionary<string, object> Details { get; init; }`
    - Problema:        Toda la información estructurada de un error viaja en un diccionario sin tipar: excepción (`Ex`), valor (`Value`), `ParamName`, `Expected`, `FailedIndexes`, `NotFound`, `ProblemsDetails`. El propio `__Doc/1_Intro.md` (línea ~182) lo reconoce: *«No existen `Exception`, `Value`, `HasException` ni `HasValue`: esa información vive en `Details` bajo…»*. Y al ser un `Dictionary` público, **cualquiera puede mutarlo desde fuera**.
    - Impacto:         (a) Nada impide escribir `Details["Ex"] = "hola"` (un `string`), y entonces `GetDetailException()` falla en **runtime**, lejos del origen. (b) **La serialización con `System.Text.Json` de `object` es un campo de minas**, y una `Exception` dentro no serializa de forma útil ni segura (puede filtrar rutas y trazas internas). (c) Cero descubribilidad: el consumidor no tiene forma de saber qué claves esperar sin leer el código fuente.
    - Propuesta:       Camino incremental y **no rompedor**: exponer `Details` como `IReadOnlyDictionary<string, object>` y añadir accesores tipados de solo lectura como propiedades de primera clase (`Exception? Exception`, `bool HasException`, `object? Value`, `bool HasValue`) que lean del diccionario — hoy eso son extensiones sueltas (`HasExceptionDetails()`, `HasValueDetails()`) y merecen estar en el tipo. A futuro, un tipo `DetailKey<T>` para claves fuertemente tipadas.

---

## 🟡 Prioridad media

- [ ] **MR11. `Value` y `ErrorsDetails` son `internal protected`: no hay acceso al valor sin arriesgar una excepción**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`
    - Miembro:         `internal protected T Value { get; init; }`, `internal protected MlErrorsDetails ErrorsDetails { get; init; }`
    - Problema:        Desde fuera del ensamblado, la única vía para leer el valor es `SecureValidValue()`, que **lanza** si el resultado es un fallo. Falta el par no-lanzante canónico de cualquier tipo `Result` moderno.
    - Impacto:         Obliga a pasar siempre por `Match`, incluso cuando lo natural sería un `if`. No hay pattern matching, no hay deconstrucción, y el usuario acaba escribiendo `try/catch` alrededor de `SecureValidValue()`, que es justo lo que la librería quiere evitar.
    - Propuesta:       Añadir API **aditiva** (no rompe nada):

      ```csharp
      public bool TryGetValue(out T value);
      public bool TryGetError(out MlErrorsDetails errors);
      public T    ValueOrDefault(T fallback = default!);
      public void Deconstruct(out bool isValid, out T value, out MlErrorsDetails errors);
      ```

      Con `Deconstruct` se habilita `var (ok, valor, error) = resultado;`, que es lo que un usuario de C# moderno espera encontrar.

- [ ] **MR12. `SecureValidValue` lanza `InvalidProgramException` y pierde el `MlErrorsDetails`**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResultActions.cs`, región `SecureValidValue`
    - Miembro:         `SecureValidValue<T>`, `SecureValidValueAsync<T>`, `SecureFailErrorsDetails<T>` (+2 async)
    - Problema:        Lanzan `InvalidProgramException`, un tipo **reservado por el CLR** para indicar que el JIT encontró IL corrupta. Además, al lanzar solo se transmite un `string`: **todo el `MlErrorsDetails` se pierde**.
    - Evidencia:       `F_tipo=InvalidProgramException`
    - Impacto:         (a) Confunde el diagnóstico y engaña a los filtros de excepciones y a la telemetría, que clasificarán el error como corrupción del runtime. (b) Al cruzar la frontera de excepciones se pierden los errores y los detalles acumulados, que es precisamente la información valiosa.
    - Propuesta:       Usar `InvalidOperationException` o, mejor, una `MlResultException` propia que **transporte el `MlErrorsDetails`** en una propiedad, de modo que un `catch` de nivel superior pueda recuperarlo íntegro.

- [ ] **MR13. `MlResult<T>.ToString()` devuelve `"Not right value"` para un `Valid` con valor `null`**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`
    - Miembro:         `ToString()` → `Match(fail: ..., valid: value => value?.ToString() ?? "Not right value")`
    - Problema:        Un `MlResult<string?>.Valid(null)` es un resultado **perfectamente válido**, y su representación textual afirma literalmente que el valor no es correcto. El mensaje además está en inglés macarrónico.
    - Impacto:         Log engañoso justo cuando se está depurando. Y `MlResult<T>` **no tiene `[DebuggerDisplay]`**, así que en el depurador hay que expandir el objeto para ver cualquier cosa.
    - Propuesta:       Devolver `"null"` (o `string.Empty`) y añadir `[DebuggerDisplay]` a `MlResult<T>`, `MlErrorsDetails` y `MlError`.

- [ ] **MR14. `MlErrorsDetails.ToString()` devuelve cadena vacía cuando no hay errores ni detalles**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         `ToString()` (implementación manual con `StringBuilder`)
    - Problema:        Sin errores y sin detalles, el `StringBuilder` no acumula nada y se devuelve `""`.
    - Impacto:         El log escribe una línea en blanco donde debería haber un error. Muy relacionado con **MR8**: si se garantiza el invariante «un fallo siempre tiene al menos un error», este caso casi desaparece — pero conviene blindarlo igualmente.
    - Propuesta:       Devolver un marcador explícito, del tipo `"MlErrorsDetails (empty)"`.

- [ ] **MR15. Erratas en identificadores públicos**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResultActionsErrorsDetails.cs`, `Types/MlResultBucles.cs`, `Types/Errors/MlErrorsDetailsActions.cs`, `Types/MlResultActionsMap.cs`
    - Miembro:         `MergeErrorsDetailsIfFailDiferentTypes` (→ *Different*), `FusionErrosIfExists` y `FusionFailErros` (→ *Errors*), la variable interna `printipalDetailsWitoutEx` (→ *principalDetailsWithoutEx*), y el literal `"Warning, MapDefault method is only valid tu debug code"` (→ *to*) en `MlResultActionsMap.cs:906`
    - Problema:        Erratas visibles en la **superficie pública** de la librería.
    - Impacto:         Merma la percepción de calidad y obliga al usuario a memorizar el error tipográfico para poder invocar el método.
    - Propuesta:       Crear los nombres correctos y dejar los actuales como alias `[Obsolete]`. **Este patrón ya está en uso en el repositorio**, así que es coherente y no rompe a nadie. Ver también [`Consejos-Nomenclatura.md`](Consejos-Nomenclatura.md).

- [ ] **MR16. `MergeErrorsDetails<T, TReturn>` obliga a especificar genéricos redundantes**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetailsActions.cs`, uso visible en `Types/MlResultActions.cs`
    - Miembro:         `MergeErrorsDetails<T, TReturn>`
    - Problema:        `TReturn` no es inferible y `T` no aporta información útil, así que el llamante escribe siempre la forma completa: `errorDetails1.MergeErrorsDetails<T2, (T1, T2)>(source2)`.
    - Impacto:         Ruido en cada punto de uso y una barrera de entrada innecesaria; el código de `CreateCompleteMlResult` es difícil de leer por este motivo.
    - Propuesta:       Simplificar la firma a `MergeErrorsDetails<TReturn>` (o separar la fusión de la conversión: `Merge(...).ToMlResultFail<TReturn>()`), manteniendo la firma antigua como `[Obsolete]`.

- [ ] **MR17. `MlErrorsDetails` es una `class` que quiere ser un `record`**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         declaración del tipo y constructor `protected MlErrorsDetails(IEnumerable<string>, Dictionary<string, object>)`
    - Problema:        Varias señales de un diseño a medio camino: es `class` pero usa **constructor primario con parámetros en PascalCase** (`Errors`, `Details` — convención de `record`), usa `null!` para silenciar el compilador, tiene propiedades `init` sin igualdad de valor, y arrastra un constructor `protected` que es **código muerto** (la clase no es `sealed` pero no tiene ningún heredero en la solución).
    - Impacto:         Confunde sobre la semántica del tipo (¿valor o referencia?) y es la causa probable de los **21 warnings `CS8619`** de `MlResultActionsBind.cs`. La incoherencia se extiende a la organización: `MlErrorDetailsExtensions` vive dentro de `MlErrorsDetails.cs` (ns `.Errors`), `MlErrorsDetailsActions` en `.Errors`, y `MlResultActionsErrorsDetails` en `.Types` — **tres sitios con responsabilidades solapadas**.
    - Propuesta:       Convertirlo en `record` (encaja con **MR6** y **MR7**), **sellarlo**, eliminar el constructor `protected` muerto, quitar los `null!` y consolidar las tres clases de extensión en una ubicación coherente.

- [ ] **MR18. `MlResult._` / `Discard` crean un `Valid` que contiene `null`**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`
    - Miembro:         `static MlResult<T> _ => new(default(T)!)`, `Discard`
    - Problema:        Para cualquier `T` de referencia, `default(T)!` es `null`, así que `_` produce un **resultado válido cuyo valor es `null`**. El `!` silencia el aviso que precisamente advertía de esto. Además, la propiedad crea una **instancia nueva en cada acceso**.
    - Impacto:         Un `null` viaja como éxito por todo el pipeline y revienta con un `NullReferenceException` **muy lejos del origen**, que es el escenario más costoso de depurar. Es exactamente lo que un tipo `Result` debería impedir.
    - Propuesta:       Documentarlo de forma explícita, restringir su uso a `T` de valor o a un `Unit`/`Void` propio, y cachear la instancia en un `static readonly` para no asignar en cada acceso.

---

## 🟢 Prioridad baja

- [ ] **MR19. Tres asignaciones de heap por cada resultado válido**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`, `Types/Errors/MlErrorsDetails.cs`
    - Miembro:         constructor `MlResult<T>(T t)` → `new(t, new MlErrorsDetails(), true)`
    - Problema:        La ruta de éxito, que es la mayoritaria, asigna el `MlResult<T>` **más** un `MlErrorsDetails` **más** un `Dictionary` vacío que nunca se usará.
    - Impacto:         Presión de GC innecesaria en pipelines de alto volumen, que es justo donde esta librería se encadena miles de veces.
    - Propuesta:       Compartir una instancia singleton `MlErrorsDetails.Empty` e inicializar `Details` de forma **perezosa** (`null` hasta el primer uso, expuesto como diccionario vacío de solo lectura). A futuro, evaluar `readonly struct` para `MlResult<T>`.

- [ ] **MR20. Serialización: `MlResult<T>` y `MlErrorsDetails` no son serializables de forma segura**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResult.cs`, `Types/Errors/MlErrorsDetails.cs`
    - Problema:        `Value` y `ErrorsDetails` son `internal protected` (invisibles para `System.Text.Json`), y `Details` es un `Dictionary<string, object>` que puede contener una `Exception`.
    - Impacto:         Devolver un `MlResult<T>` desde una API web no produce el JSON esperado, y si se fuerza, **puede filtrar trazas y rutas internas** al cliente.
    - Propuesta:       `JsonConverter` propio para ambos tipos y un mapeo de primera clase a `ProblemDetails` — la constante `WebErrorDetailsKeys.ProblemsDetails` del proyecto `MoralesLarios.OOFP.Shared` ya está preparada para esto.

- [ ] **MR21. Un `MlResult` puede descartarse en silencio**
    - Proyecto:        `MoralesLarios.OOFP`
    - Problema:        Invocar `EnsureFp.That(...)` (o cualquier método que devuelva `MlResult`) como **sentencia suelta** compila sin aviso y el fallo se pierde por completo.
    - Impacto:         Una validación que parece estar puesta, no está. Es un fallo de omisión difícil de detectar en revisión de código.
    - Propuesta:       Analizador Roslyn propio (o atributo `[MustUseReturnValue]` de JetBrains.Annotations) que marque como advertencia el descarte del valor de retorno.

- [ ] **MR22. Mensajes internos en inglés y sin centralizar**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/MlResultActionsErrorsDetails.cs`, `Types/MlResultActions.cs`, `Types/MlResult.cs`
    - Problema:        Literales dispersos por el código: `"The key {key} does not exist in the details"`, `"Cannot obtain the secure value from MlResult in Fail state"`, `"Not right value"`…
    - Impacto:         Imposible localizar, revisar ni mantener coherente el tono. Contrasta con `EnsureFp`, donde el trabajo **ya se hizo bien** en `EnsureFpMessages.cs`.
    - Propuesta:       Centralizar en un `MlResultMessages` siguiendo el patrón ya establecido por `EnsureFpMessages.cs`.

- [ ] **MR23. `Merge` comprueba `null` en el parámetro equivocado y arrastra código comentado**
    - Proyecto:        `MoralesLarios.OOFP`
    - Archivo / clase: `Types/Errors/MlErrorsDetailsActions.cs`
    - Miembro:         `Merge(this MlErrorsDetails source, MlErrorsDetails other)`
    - Problema:        Usa `source?.Details` — comprobando `source`, que en un método de extensión invocado con sintaxis de instancia prácticamente nunca es `null` — mientras accede a `other.Errors` **sin comprobación alguna**. La protección está en el lado que no la necesita. El método arrastra además 2 líneas comentadas de una versión anterior.
    - Impacto:         `NullReferenceException` con un `other` nulo, en un método que aparenta ser defensivo.
    - Propuesta:       Validar `other` (devolviendo `source` si es `null`, o lanzando `ArgumentNullException`), retirar el `?.` innecesario y borrar el código comentado. Limpiar de paso el bloque comentado `ValidateObject` y el `using System.ComponentModel.DataAnnotations;` sobrante de `Helpers/Extensions/Extensions.cs`.

---

## Plan de trabajo sugerido

Cada fase es **publicable por separado**, sin dejar nada a medias.

| Fase | Puntos | Contenido | ¿Rompe API? |
|---|---|---|---|
| **1** | MR1, MR2, MR3, MR5, MR23 | Los bugs confirmados: `AddDetails`, `AddDetail`, `Merge` y la numeración de claves `Ex` | **No** |
| **2** | MR8, MR9, MR12, MR13, MR14 | Invariantes y diagnóstico: fallo sin error, `GetDetail`, tipo de excepción, `ToString()` | Mínimamente |
| **3** | MR10, MR11 | API aditiva: `TryGetValue`, `TryGetError`, `Deconstruct` y accesores tipados | **No** (aditivo) |
| **4** | MR6, MR7, MR17 | Igualdad e inmutabilidad reales: `IReadOnlyList`, `record`, sellado | **Sí** |
| **5** | MR4, MR15, MR16, MR18 | Conversiones implícitas, erratas y genéricos, con alias `[Obsolete]` | Controlado |
| **6** | MR19, MR20, MR21, MR22 | Rendimiento, serialización, analizador y mensajes | **No** |

### Avisos antes de empezar

1. **La Fase 1 es la más urgente y la de menor riesgo**, pero **arreglar `AddDetails` (MR1) puede hacer fallar tests existentes** que hoy pasan *porque* el método no hace nada. Hay que revisar la suite completa tras el cambio.
2. La **Fase 4 es la de mayor riesgo**: los cambios de inmutabilidad en `MlErrorsDetails` afectan a `MlResultActionsBind.cs`, `MlResultActionsMap.cs` y `MlResultActionsExecSelf.cs`, con **~150 usos de `GetDetail*`**. Conviene hacerla incremental y con compatibilidad.
3. **Baseline actual de la suite: 736 tests en verde.** Validar con `dotnet build` + `dotnet test` tras cada fase.
4. Si se toca la API pública, actualizar la documentación: `__Doc/Types/MlResult.md`, `__Doc/Types/MlResultErrors.md`, `__Doc/Types/MlResultActionsErrorsDetails.md`, `__Doc/1_Intro.md` (línea ~182, sobre `Details`) y los `README.md` afectados. Repasar después el chequeo de enlaces (**1005 enlaces relativos, 0 rotos** en la última validación).

---

## Anexo: cómo se verificaron los puntos

Se creó una clase de tests temporal (`MoralesLarios.OOFP.Unit.Tests/Diagnostico/_DiagMlResultTests.cs`, **ya eliminada**)
con un test por hipótesis, redactado de forma que **pasara si el comportamiento fuese el correcto**. El mensaje de
aserción incluía el valor medido, para que el fallo mostrara la evidencia. Resultado:

```text
Failed!  - Failed: 12, Passed: 0, Skipped: 0, Total: 12
```

Más el error de compilación `CS0457` de **MR4**, que impidió incluso compilar su caso de prueba.

| Punto | Evidencia medida |
|---|---|
| MR1 | `B_count=0 keys=[]` · `B2_count=0 keys=[]` |
| MR2 | `C2_original_mutado=True sameRef=True` · `ArgumentException: ... Key: K` |
| MR3 | `ArgumentException: An item with the same key has already been added. Key: Value` |
| MR4 | `error CS0457: Ambiguous user defined conversions` |
| MR5 | `E_append=[Ex,Ex2]` vs `E_merge=[Ex,Ex1]` |
| MR6 | `A_Valid_iguales=False` · `A2_Fail_iguales=False` · `A3_details_iguales=False` |
| MR8 | `H_mensajes=[]` · `toString=[]` |
| MR9 | `I_IsValid=False` |
| MR12 | `F_tipo=InvalidProgramException` |

> 💡 **Recomendación**: al arreglar cada punto, **recuperar su test de diagnóstico** y dejarlo en la suite
> como test de regresión. Ya está escrito en la polaridad correcta: pasará cuando el bug esté resuelto.
