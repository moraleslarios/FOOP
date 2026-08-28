# Bucles y proyecciones — Recorrer colecciones dentro del carril

## Índice

1. [Introducción](#introducción)
2. [El problema: `IEnumerable<MlResult<T>>` no sirve de nada](#el-problema-ienumerablemlresultt-no-sirve-de-nada)
3. [Las cuatro estrategias de proyección](#las-cuatro-estrategias-de-proyección)
4. [`Projection` — procesar todo y acumular errores](#projection--procesar-todo-y-acumular-errores)
5. [`ProjectionWhile` — parar en el primer fallo](#projectionwhile--parar-en-el-primer-fallo)
6. [`ProjectionParallelAsync` — concurrencia real](#projectionparallelasync--concurrencia-real)
7. [`ProjectionSplit` — separar aciertos de fallos](#projectionsplit--separar-aciertos-de-fallos)
8. [Los agregadores: `FusionFailErros`, `FusionErrosIfExists`, `VerifiedEnumerableResultData`](#los-agregadores-fusionfailerros-fusionerrosifexists-verifiedenumerableresultdata)
9. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`MlResultBucles` resuelve un problema que aparece en cuanto trabajas con colecciones: **qué
hacer cuando la operación que aplicas a cada elemento puede fallar**.

Es la única familia de la librería que **sí acumula errores** de verdad (recuerda que
[`Combine`](../Several/4_Combine.md) **no** lo hace: cortocircuita).

```csharp
// Validar 500 líneas de un fichero importado y saber TODAS las que están mal
MlResult<IEnumerable<Linea>> r = lineasCrudas.Projection(ValidarLinea);

if (r.IsFail)
    // Contiene los errores de todas las líneas defectuosas, no solo de la primera
    _log.LogWarning(r.ErrorsDetails.ToErrorsDescription());
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema: `IEnumerable<MlResult<T>>` no sirve de nada

Si aplicas `Select` con una función que devuelve `MlResult<T>`, obtienes una colección de
resultados. Eso **no** es útil: no puedes seguir la tubería con ella.

```csharp
// ❌ Tipo inmanejable: ¿está bien? ¿está mal? ¿cuáles fallaron?
IEnumerable<MlResult<Linea>> inutil = crudas.Select(ValidarLinea);

// ✅ Lo que quieres es esto: UN resultado que contiene TODA la colección
MlResult<IEnumerable<Linea>> util = crudas.Projection(ValidarLinea);
```

🔑 La operación clave se llama **"invertir el envoltorio"**: pasar de
`IEnumerable<MlResult<T>>` a `MlResult<IEnumerable<T>>`. Esta familia lo hace de cuatro
maneras distintas según lo que necesites.

```
IEnumerable<T>  ──[ Func<T, MlResult<TResult>> ]──►  MlResult<IEnumerable<TResult>>
```

---

## Las cuatro estrategias de proyección

| Método | Recorre | Ante un fallo | Errores del resultado |
|--------|---------|---------------|----------------------|
| `Projection` | **Todos** los elementos | Sigue procesando | **Todos** los errores fusionados |
| `ProjectionWhile` | Hasta el primer fallo | **Para** (`break`) | Solo el del elemento que falló |
| `ProjectionParallelAsync` | Todos, **en paralelo** | Sigue (ya lanzados) | Todos los errores fusionados |
| `ProjectionSplit` | **Todos** | Sigue | **Nunca falla**: devuelve dos diccionarios |

🔑 **Criterio de elección:**

- **Validar entrada de usuario / importar ficheros** → `Projection` o `ProjectionSplit`
  (quieres informar de todos los problemas de una vez).
- **Pasos dependientes o costosos** → `ProjectionWhile` (no gastes recursos si ya falló).
- **Llamadas a servicios externos independientes** → `ProjectionParallelAsync`.
- **Procesamiento parcial aceptable** (procesa lo bueno, informa de lo malo) →
  `ProjectionSplit`.

Todas las variantes tienen sobrecarga **con índice** (`Func<T, int, MlResult<TResult>>`), muy
útil para mensajes tipo *"error en la línea 42"*.

---

## `Projection` — procesar todo y acumular errores

```csharp
public static MlResult<IEnumerable<TResult>> Projection<T, TResult>(this IEnumerable<T>             source,
                                                                         Func<T, MlResult<TResult>> completeFuncTransform)
{
    var result = source.ToMlResultValid()
                        .Bind(x =>
                        {
                            var partialData = x.Select(completeFuncTransform).ToList();

                            var result = partialData.Any(x => x.IsFail) ?
                                         FusionFailErros(partialData)   :
                                         MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.Value));

                            return result;
                        });
    return result;
}
```

🔑 **Comportamiento:** ejecuta la transformación sobre **todos** los elementos (nótese el
`.ToList()`, que fuerza la evaluación), y solo después decide:

- Si **algún** elemento falló → `FusionFailErros` fusiona **todos** los errores.
- Si todos son válidos → resultado válido con la colección transformada.

```csharp
var lineas = new[] { "10;ABC", "xx;DEF", "30;GHI", "yy;JKL" };

var r = lineas.Projection((linea, i) => ParsearLinea(linea, i));

// r.IsFail == true, y ErrorsDetails contiene los errores de las líneas 2 Y 4
Console.WriteLine(r.ErrorsDetails.ToErrorsDescription());
// → "Línea 2: cantidad ilegible 'xx'" + "Línea 4: cantidad ilegible 'yy'"
```

⚠️ **Coste:** procesa todos los elementos aunque el primero ya haya fallado. Si la
transformación es costosa (consultas, E/S), usa `ProjectionWhile`.

### Sobrecargas

| Firma | Notas |
|-------|-------|
| `Projection(Func<T, MlResult<TResult>>)` | Base |
| `Projection(Func<T, int, MlResult<TResult>>)` | Con índice |
| `ProjectionAsync(...)` desde `IEnumerable<T>` con delegado **síncrono** | Envoltura `.ToAsync()` |
| `ProjectionAsync(...)` desde `Task<IEnumerable<T>>` | Con `await` real |
| `ProjectionAsync(...)` con delegado **asíncrono** | Recorre **en secuencia** con `await` por elemento |

⚠️ **Importante:** `ProjectionAsync` con delegado asíncrono es **secuencial**, no paralelo.
Espera cada elemento antes de pasar al siguiente. Para paralelismo real necesitas
`ProjectionParallelAsync`.

---

## `ProjectionWhile` — parar en el primer fallo

```csharp
public static MlResult<IEnumerable<TResult>> ProjectionWhile<T, TResult>(this IEnumerable<T>             source,
                                                                              Func<T, MlResult<TResult>> completeFuncTransform)
{
    var result = source.ToMlResultValid()
                        .Bind(x =>
                        {
                            List<MlResult<TResult>> partialData = [];

                            foreach (var item in x)
                            {
                                var funcResult = completeFuncTransform(item);
                                partialData.Add(funcResult);
                                if (funcResult.IsFail) break;      // ← corta aquí
                            }

                            var result = partialData.Any(x => x.IsFail) ?
                                         FusionFailErros(partialData) :
                                         MlResult<IEnumerable<TResult>>.Valid(partialData.Select(x => x.Value));

                            return result;
                        });
    return result;
}
```

🔑 **Cortocircuita**: en cuanto un elemento falla, deja de procesar el resto. El resultado
contiene **solo ese error** (porque es el único fallo de la lista).

```csharp
// Procesar pagos: si uno falla, no sigas cobrando
var r = pagos.ProjectionWhile(ProcesarPago);

// Si el pago 3 de 100 falla, los pagos 4..100 NO se intentan
```

⚠️ **Detalle sutil de la sobrecarga con índice:** el `index++` está **después** del `break`:

```csharp
int index = 0;
foreach (var item in x)
{
    var funcResult = completeFuncTransform(item, index);
    partialData.Add(funcResult);
    if (funcResult.IsFail) break;
    index++;                          // ← solo se incrementa si NO falló
}
```

Esto funciona correctamente (el índice es válido en cada llamada), pero significa que el
índice **cuenta elementos procesados con éxito**. En este bucle coincide con la posición real
porque se corta al primer fallo.

💡 **`ProjectionWhile` es el equivalente de colección de `Bind`**: cortocircuita.
**`Projection` es el que acumula**, algo que ningún otro operador de la librería hace.

---

## `ProjectionParallelAsync` — concurrencia real

Este es el único lugar de la librería con **paralelismo auténtico**:

```csharp
public static async Task<MlResult<IEnumerable<TResult>>> ProjectionParallelAsync<T, TResult>(
        this IEnumerable<T>                   source,
             Func<T, Task<MlResult<TResult>>> completeFuncTransformAsync)
{
    var result = await source.ToMlResultValidAsync()
                        .BindAsync(async colec =>
                        {
                            List<Task<MlResult<TResult>>> tasks =
                                colec.Select(item => completeFuncTransformAsync(item)).ToList();

                            await Task.WhenAll(tasks);

                            List<MlResult<TResult>> partialData = tasks.Select(t => t.Result).ToList();

                            var result = partialData.Any(x => x.IsFail) ?
                                         FusionFailErros(partialData) :
                                         await MlResult<IEnumerable<TResult>>.ValidAsync(partialData.Select(x => x.Value));

                            return result;
                        });
    return result;
}
```

🔑 Lanza **todas** las tareas a la vez y espera con `Task.WhenAll`. Acumula todos los errores,
como `Projection`.

```csharp
// Consultar el stock de 50 artículos en un servicio externo, en paralelo
var r = await articulos.ProjectionParallelAsync(a => _stockApi.ConsultarAsync(a.Sku));
```

⚠️ **No hay límite de concurrencia.** Si la colección tiene 10 000 elementos, se lanzan
10 000 tareas simultáneas. Puedes agotar el pool de conexiones o provocar un
*rate limit* en la API. **Trocea tú la colección** si es grande:

```csharp
// ✅ Lotes de 20
var resultados = new List<MlResult<Stock>>();
foreach (var lote in articulos.Chunk(20))
{
    var r = await lote.ProjectionParallelAsync(a => _stockApi.ConsultarAsync(a.Sku));
    // …acumula
}
```

⚠️ **Usa `t.Result` después de `Task.WhenAll`**, no `await t`. Es correcto (las tareas ya
están completadas), pero significa que si alguna tarea **lanza una excepción**, se propaga
envuelta en `AggregateException` en lugar de la excepción original. **Envuelve tus delegados
con `TryToMlResultAsync`** para que las excepciones se conviertan en fallos del carril:

```csharp
// ✅ El delegado no lanza nunca: devuelve MlResult
var r = await articulos.ProjectionParallelAsync(async a =>
{
    Func<string, Task<Stock>> consulta = _stockApi.ConsultarAsync;
    return await consulta.TryToMlResultAsync(a.Sku, ex => $"Error al consultar {a.Sku}");
});
```

⚠️ **Solo existe en versión asíncrona** (obviamente) y **solo con delegado asíncrono**.

---

## `ProjectionSplit` — separar aciertos de fallos

```csharp
public static MlResult<(Dictionary<T, TResult> valids, Dictionary<T, MlErrorsDetails> fails)>
    ProjectionSplit<T, TResult>(this IEnumerable<T> source, Func<T, MlResult<TResult>> completeFuncTransform)
    where T : notnull
{
    var result = EnsureFp.NotNull(completeFuncTransform, "completeFuncTransform cannot be null")
                        .Map(x =>
                        {
                            var partialData = source.Where(z => z is not null)
                                                    .Select(z => (z, completeFuncTransform(z))).ToList();

                            var partialResult = (
                                valids: partialData.Where(x => x.Item2.IsValid).ToDictionary(x => x.Item1, x => x.Item2.SecureValidValue()),
                                fails : partialData.Where(x => x.Item2.IsFail ).ToDictionary(x => x.Item1, x => x.Item2.SecureFailErrorsDetails()));
                            return partialResult;
                        });
    return result;
}
```

🔑 **Es el más útil de los cuatro para procesamiento por lotes**, y el que suele pasar
desapercibido. Devuelve **dos diccionarios**, indexados por el elemento original:

- `valids`: elemento original → resultado transformado.
- `fails`: elemento original → errores.

```csharp
var r = await pedidos.ProjectionSplitAsync(p => ProcesarAsync(p));

var (procesados, rechazados) = r.SecureValidValue();

_log.LogInformation("{Ok} procesados, {Ko} rechazados", procesados.Count, rechazados.Count);

foreach (var (pedido, errores) in rechazados)
    _log.LogWarning("Pedido {Id}: {Errores}", pedido.Id, errores.ToErrorsDescription());

await GuardarAsync(procesados.Values);   // ← guarda solo los buenos
```

### Particularidades clave

⚠️ **Prácticamente nunca falla.** El único `Fail` posible es que el propio delegado sea
`null` (lo comprueba con `EnsureFp.NotNull`). Los fallos de los elementos **no** hacen fallar
el resultado: van al diccionario `fails`.

⚠️ **Descarta silenciosamente los elementos `null`** (`Where(z => z is not null)`). Si tu
colección tiene nulos, desaparecen sin aviso: no están ni en `valids` ni en `fails`.

```csharp
var items = new[] { "a", null, "b" };
var r = items.ProjectionSplit(Procesar);
// valids + fails suman 2, no 3. El null se descartó en silencio.
```

⚠️ **Exige `where T : notnull`** y usa el elemento como **clave de diccionario**. Dos
consecuencias importantes:

1. **Si hay elementos duplicados, `ToDictionary` lanza `ArgumentException`.**
2. La igualdad depende de `Equals`/`GetHashCode` de `T`. Con `record`, la igualdad es
   estructural: **dos elementos con los mismos valores cuentan como duplicados**.

```csharp
// ❌ Con records de igualdad estructural, esto LANZA si hay dos líneas idénticas
public record Linea(string Sku, int Cantidad);
var r = lineas.ProjectionSplit(Validar);   // ⚠️ ArgumentException si hay duplicados

// ✅ Usa una clave única, o desduplica antes
var r = lineas.Select((l, i) => (Indice: i, Linea: l))
              .ProjectionSplit(t => Validar(t.Linea));
```

⚠️ La variante con delegado asíncrono (`ProjectionSplitAsync` con
`Func<T, Task<MlResult<TResult>>>`) recorre **en secuencia** con `foreach` + `await`. No hay
versión paralela de `ProjectionSplit`.

---

## Los agregadores: `FusionFailErros`, `FusionErrosIfExists`, `VerifiedEnumerableResultData`

Estos tres métodos operan directamente sobre `IEnumerable<MlResult<T>>` y son la maquinaria
interna de las proyecciones. También puedes usarlos tú.

### `VerifiedEnumerableResultData` — el más recomendable

```csharp
public static MlResult<IEnumerable<T>> VerifiedEnumerableResultData<T>(this IEnumerable<MlResult<T>> source)
    => source.Any(x => x.IsFail) ?
       FusionFailErros(source)   :
       MlResult<IEnumerable<T>>.Valid(source.Select(x => x.Value));
```

Convierte una colección de resultados en un resultado de colección. **Es el que debes usar**
si ya tienes un `IEnumerable<MlResult<T>>` en la mano:

```csharp
IEnumerable<MlResult<Linea>> resultados = ObtenerDeAlgunSitio();

MlResult<IEnumerable<Linea>> r = resultados.VerifiedEnumerableResultData();
```

⚠️ Enumera `source` **dos veces** (`Any` y `Select`). Materializa con `.ToList()` si es una
consulta diferida.

### `FusionErrosIfExists` — equivalente seguro

```csharp
public static MlResult<IEnumerable<T>> FusionErrosIfExists<T>(this IEnumerable<MlResult<T>> source)
{
    var partialResult = source.Where(x => x.IsFail).ToList();

    if (!partialResult.Any())
        return MlResult<IEnumerable<T>>.Valid(source.Select(x => x.SecureValidValue()));

    MlErrorsDetails result = partialResult.First().ErrorsDetails;
    foreach (var item in partialResult.Skip(1))
        result = result.Merge(item.ErrorsDetails);

    return result;
}
```

Hace lo mismo que `VerifiedEnumerableResultData` pero usando `SecureValidValue()` en lugar de
`.Value`, y **maneja correctamente el caso "ninguno falló"**. Es la opción más robusta de las
tres.

### `FusionFailErros` — ⚠️ tiene un bug: exige que haya fallos

```csharp
public static MlResult<IEnumerable<T>> FusionFailErros<T>(this IEnumerable<MlResult<T>> source)
{
    var partialResult = source.Where(x => x.IsFail).ToList();

    if (!partialResult.Any()) MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");
    //                        ↑ ⚠️ FALTA EL 'return': el resultado se descarta

    MlErrorsDetails result = partialResult.First().ErrorsDetails;   // ⚠️ lanza si está vacío
    // …
}
```

⚠️⚠️ **Aviso importante:** en la comprobación de "no hay fallos" **falta el `return`**. El
`MlResult.Fail(...)` se construye y se descarta, y la ejecución continúa hasta
`partialResult.First()`, que **lanza `InvalidOperationException`** sobre una lista vacía.

🔑 **Consecuencia práctica:** **nunca llames a `FusionFailErros` directamente** salvo que
estés seguro de que hay al menos un fallo. Las proyecciones internas lo hacen bien (siempre
comprueban `Any(x => x.IsFail)` antes), pero tú no tienes por qué acordarte.

```csharp
// ❌ Si todos los resultados son válidos, LANZA InvalidOperationException
var r = resultados.FusionFailErros();

// ✅ Usa cualquiera de estos dos, que sí manejan el caso vacío
var r = resultados.VerifiedEnumerableResultData();
var r = resultados.FusionErrosIfExists();
```

Los tres tienen variantes `*Async` (envolturas `.ToAsync()` y sobrecargas desde
`Task<IEnumerable<MlResult<T>>>`.

💡 Nótese la errata en el nombre: **`FusionFailErros`**, sin la `e` de "Errors". Está así en
la API pública.

---

## ⚠️ Particularidades reales del código fuente

**1. `Projection` es el único operador de la librería que ACUMULA errores.** `Combine`,
`Bind` y `Map` cortocircuitan.

**2. `ProjectionAsync` con delegado asíncrono es SECUENCIAL**, no paralelo. Solo
`ProjectionParallelAsync` paraleliza.

**3. `ProjectionParallelAsync` no limita la concurrencia**: lanza tantas tareas como
elementos. Trocea tú las colecciones grandes.

**4. `ProjectionParallelAsync` usa `t.Result` tras `Task.WhenAll`**: las excepciones llegan
como `AggregateException`. Envuelve los delegados con `TryToMlResultAsync`.

**5. `ProjectionSplit` casi nunca falla**: solo si el delegado es `null`. Los fallos de
elementos van al diccionario `fails`.

**6. `ProjectionSplit` descarta los elementos `null` en silencio** (`Where(z => z is not null)`).

**7. `ProjectionSplit` usa el elemento como clave de diccionario**: ⚠️ **lanza
`ArgumentException` con elementos duplicados** (ojo con los `record`, cuya igualdad es
estructural).

**8. ⚠️ `FusionFailErros` tiene un bug**: falta el `return` en el caso "no hay fallos", así
que **lanza `InvalidOperationException`** si se lo llamas con una colección sin fallos. Usa
`VerifiedEnumerableResultData` o `FusionErrosIfExists`.

**9. `VerifiedEnumerableResultData` enumera la colección dos veces** (`Any` + `Select`).
Materializa las consultas diferidas.

**10. Erratas en los nombres públicos:** `FusionFailErros` y `FusionErrosIfExists` (falta
una `e` en "Errors" en ambos). Están así en la API.

**11. Hay mucho código comentado** en el archivo (versiones antiguas con
`Func<T, MlResult<T>>` en lugar de `Func<T, MlResult<TResult>>`). Todas las versiones activas
son las genéricas de dos tipos.

**12. Todas las variantes tienen sobrecarga con índice** (`Func<T, int, ...>`), incluida la
paralela.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Validar toda una colección e informar de **todos** los errores | `Projection` |
| Parar en el primer fallo (pasos costosos o dependientes) | `ProjectionWhile` |
| Llamadas independientes a servicios externos, en paralelo | `ProjectionParallelAsync` |
| Procesar lo bueno e informar de lo malo | `ProjectionSplit` |
| Mensajes con el número de línea | Sobrecarga con `Func<T, int, ...>` |
| Ya tengo un `IEnumerable<MlResult<T>>` | `VerifiedEnumerableResultData` |
| Lo mismo, con manejo robusto del caso vacío | `FusionErrosIfExists` |
| Fusionar errores de una colección con fallos garantizados | `FusionFailErros` (⚠️ lanza si no hay fallos) |
| Rechazar una colección vacía | [`EmptyToFailed`](../Several/1_EmptyToFailed.md) |

---

## Ejemplos Prácticos

### Ejemplo 1: importar un CSV informando de todas las líneas malas

```csharp
public class ImportadorCsv
{
    public MlResult<IEnumerable<Movimiento>> Importar(IEnumerable<string> lineas)
        => lineas.EmptyToFailed("El fichero no contiene líneas")!
                 // Projection: quiero saber TODAS las líneas defectuosas de una vez
                 .Bind(ls => ls.Projection((linea, i) => ParsearLinea(linea, i + 1)));

    private MlResult<Movimiento> ParsearLinea(string linea, int numeroLinea)
    {
        var campos = linea.Split(';');

        return EnsureFp.That(campos, campos.Length == 4,
                             $"Línea {numeroLinea}: se esperaban 4 campos y hay {campos.Length}")
                       .Bind(c => decimal.TryParse(c[2], out var importe)
                                      ? importe.ToMlResultValid()
                                      : ($"Línea {numeroLinea}: importe ilegible '{c[2]}'",
                                         new Dictionary<string, object> { ["Linea"] = numeroLinea })
                                            .ToMlResultFail<decimal>())
                       .Map(importe => new Movimiento(campos[0], campos[1], importe, campos[3]));
    }
}

// El resultado fallido contiene TODOS los errores:
// "Línea 12: importe ilegible 'x'", "Línea 47: se esperaban 4 campos y hay 3", …
```

### Ejemplo 2: `ProjectionWhile` para una migración transaccional

```csharp
public async Task<MlResult<IEnumerable<Script>>> AplicarMigracionesAsync(IEnumerable<Script> scripts)
    // Los scripts son dependientes: si el 3 falla, aplicar el 4 corrompería la BD
    => await scripts.ProjectionWhileAsync(async s =>
       {
           Func<Script, Task> ejecutar = _db.EjecutarAsync;

           return await ejecutar.TryToMlResultAsync(s,
                      ex => $"Falló la migración '{s.Nombre}': {ex.Message}");
       });
```

### Ejemplo 3: `ProjectionParallelAsync` con troceado y captura de excepciones

```csharp
public class SincronizadorStock
{
    public async Task<MlResult<IEnumerable<Stock>>> SincronizarAsync(IEnumerable<Articulo> articulos)
    {
        var todos = new List<MlResult<Stock>>();

        // Lotes de 20 para no saturar la API externa
        foreach (var lote in articulos.Chunk(20))
        {
            var r = await lote.ProjectionParallelAsync(async a =>
            {
                Func<string, Task<Stock>> consulta = _api.ConsultarStockAsync;

                // Envolvemos para que el delegado NUNCA lance dentro de Task.WhenAll
                return await consulta.TryToMlResultAsync(a.Sku,
                           ex => $"No se pudo consultar el stock de {a.Sku}: {ex.Message}");
            });

            todos.Add(r.Map(items => items));   // acumulamos el resultado del lote
        }

        return todos.SelectMany(r => r.IsValid ? r.SecureValidValue() : [])
                    .ToMlResultValid();
    }
}
```

### Ejemplo 4: `ProjectionSplit` para procesamiento parcial

```csharp
public class ProcesadorNominas
{
    public async Task<MlResult<InformeNomina>> ProcesarMesAsync(IEnumerable<Empleado> empleados)
    {
        // Clave única explícita para evitar el problema de duplicados
        var indexados = empleados.Select((e, i) => (Id: $"{e.Nif}#{i}", Empleado: e)).ToList();

        var r = await indexados.ProjectionSplitAsync(t => CalcularNominaAsync(t.Empleado));

        return r.Map(split =>
        {
            var (correctas, fallidas) = split;

            foreach (var (clave, errores) in fallidas)
                _log.LogWarning("Nómina de {Clave} no calculada: {Errores}",
                                clave.Id, errores.ToErrorsDescription());

            return new InformeNomina(
                Calculadas: correctas.Values.ToList(),
                Incidencias: fallidas.Select(f => new Incidencia(f.Key.Empleado.Nif,
                                                                f.Value.ToErrorsMessages())).ToList());
        });
    }
}
```

Fíjate en que el resultado global **es válido**: el proceso terminó correctamente, aunque
algunos elementos individuales tuvieran incidencias. Esa es la gran ventaja de
`ProjectionSplit`.

### Ejemplo 5: qué no hacer

```csharp
// ❌ Select con función que devuelve MlResult: tipo inmanejable
IEnumerable<MlResult<Linea>> inutil = crudas.Select(Validar);

// ✅
MlResult<IEnumerable<Linea>> util = crudas.Projection(Validar);


// ❌ FusionFailErros sin garantía de fallos: LANZA InvalidOperationException
var r = resultados.FusionFailErros();

// ✅ Manejan bien el caso "todos válidos"
var r = resultados.VerifiedEnumerableResultData();
var r = resultados.FusionErrosIfExists();


// ❌ ProjectionSplit con records duplicados: ArgumentException en ToDictionary
public record Linea(string Sku, int Cantidad);
var r = lineasConDuplicados.ProjectionSplit(Validar);

// ✅ Clave única
var r = lineas.Select((l, i) => (i, l)).ProjectionSplit(t => Validar(t.l));


// ❌ Esperar paralelismo de ProjectionAsync
var r = await items.ProjectionAsync(i => LlamadaLentaAsync(i));   // ⚠️ SECUENCIAL

// ✅
var r = await items.ProjectionParallelAsync(i => LlamadaLentaAsync(i));


// ❌ ProjectionParallelAsync sobre 10.000 elementos: 10.000 tareas simultáneas
var r = await todosLosArticulos.ProjectionParallelAsync(Consultar);

// ✅ Trocea
foreach (var lote in todosLosArticulos.Chunk(25)) { /* … */ }


// ❌ Delegado que lanza dentro de ProjectionParallelAsync: AggregateException
var r = await items.ProjectionParallelAsync(i => _api.ConsultarAsync(i));  // puede lanzar

// ✅ Envuelve con TryToMlResultAsync
var r = await items.ProjectionParallelAsync(async i =>
{
    Func<Item, Task<Dato>> f = _api.ConsultarAsync;
    return await f.TryToMlResultAsync(i, ex => $"Error en {i.Id}");
});


// ❌ Projection sobre una consulta diferida costosa (se enumera varias veces)
var r = _db.Pedidos.Where(p => p.Abierto).Projection(Validar);

// ✅ Materializa
var r = _db.Pedidos.Where(p => p.Abierto).ToList().Projection(Validar);
```

---

## Mejores Prácticas

1. **Elige la estrategia según la intención**: `Projection` para informar de todo,
   `ProjectionWhile` para cortar, `ProjectionParallelAsync` para concurrencia,
   `ProjectionSplit` para procesamiento parcial.
2. **Usa `ProjectionSplit` en procesos por lotes**: es lo que quieres el 90 % de las veces y
   suele pasarse por alto.
3. **Da una clave única explícita a `ProjectionSplit`** (por ejemplo con el índice) para
   evitar `ArgumentException` por duplicados.
4. **Nunca llames a `FusionFailErros` directamente**: usa `VerifiedEnumerableResultData` o
   `FusionErrosIfExists`, que manejan el caso vacío.
5. **Trocea las colecciones antes de `ProjectionParallelAsync`** (`Chunk(20)`, `Chunk(50)`):
   no hay límite de concurrencia interno.
6. **Envuelve los delegados asíncronos con `TryToMlResultAsync`** dentro de las proyecciones
   paralelas, para no lidiar con `AggregateException`.
7. **Aprovecha las sobrecargas con índice** para mensajes con número de línea: es lo que
   convierte un error genérico en un error accionable.
8. **Materializa las consultas diferidas** con `.ToList()` antes de proyectar.
9. **Recuerda que `ProjectionAsync` con delegado asíncrono es secuencial**: si esperabas
   paralelismo, usa la variante `Parallel`.
10. **Cuidado con los `null` en `ProjectionSplit`**: se descartan sin aviso. Fíltralos tú si
    su presencia es un error.
11. **Combina con `EmptyToFailed`** al principio, para distinguir "colección vacía" de
    "colección procesada sin incidencias".

---

## Resumen

- `MlResultBucles` convierte `IEnumerable<MlResult<T>>` en `MlResult<IEnumerable<T>>`, y es
  **la única familia que acumula errores** de verdad.
- **`Projection`** procesa **todos** los elementos y fusiona **todos** los errores. Ideal para
  validación de entrada e importaciones.
- **`ProjectionWhile`** **corta** en el primer fallo (`break`). Ideal para pasos dependientes
  o costosos.
- **`ProjectionParallelAsync`** es el **único paralelismo real** de la librería
  (`Task.WhenAll`). ⚠️ **Sin límite de concurrencia**; usa `t.Result`, así que envuelve los
  delegados con `TryToMlResultAsync`.
- **`ProjectionSplit`** devuelve **dos diccionarios** (`valids`, `fails`) y **casi nunca
  falla**. Es la mejor opción para procesamiento por lotes.
  ⚠️ Descarta los `null` en silencio y **lanza con elementos duplicados** (usa una clave
  única).
- ⚠️ **`ProjectionAsync` con delegado asíncrono es SECUENCIAL**, no paralelo.
- **`VerifiedEnumerableResultData`** y **`FusionErrosIfExists`** son los agregadores seguros.
- ⚠️⚠️ **`FusionFailErros` tiene un bug** (falta un `return`): **lanza
  `InvalidOperationException`** si la colección no tiene fallos. No lo llames directamente.
- Todas las variantes tienen **sobrecarga con índice**, muy útil para mensajes con número de
  línea.

---

## Ver también

- [`EmptyToFailed`](../Several/1_EmptyToFailed.md) — rechazar colecciones vacías antes de proyectar
- [`Combine`](../Several/4_Combine.md) — ⚠️ **no** acumula errores, a diferencia de `Projection`
- [`MlResultErrors`](../Types/MlResultErrors.md) — `Merge`, `ToErrorsDescription`, `ToErrorsMessages`
- [`Transformations`](../Transformations/Transformations.md) — `TryToMlResultAsync`, `SecureValidValue`
- [`Bind`](../Bind/3_Bind.md) — el cortocircuito de un solo valor
- [`Map`](../Map/1_Map.md) — transformación de un solo valor
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — validar cada elemento
- [`Match`](../Match/1_Match.md) — construir la respuesta final