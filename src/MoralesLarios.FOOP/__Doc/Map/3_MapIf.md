# MapIf — Transformar solo cuando se cumple una condición

## Índice

1. [Introducción](#introducción)
2. [Las dos formas de `MapIf`](#las-dos-formas-de-mapif)
3. [Firmas reales e implementación](#firmas-reales-e-implementación)
4. [Por qué la Forma B no necesita `funcFalse`](#por-qué-la-forma-b-no-necesita-funcfalse)
5. [`MapIf` frente a `BindIf` y `MapEnsure`](#mapif-frente-a-bindif-y-mapensure)
6. [Variantes asíncronas](#variantes-asíncronas)
7. [`TryMapIf` — cuando la transformación puede lanzar](#trymapif--cuando-la-transformación-puede-lanzar)
8. [⚠️ Particularidad real del código fuente: `TryMapIAsyncf`](#️-particularidad-real-del-código-fuente-trymapiasyncf)
9. [Ejemplos Prácticos](#ejemplos-prácticos)
10. [Mejores Prácticas](#mejores-prácticas)
11. [Resumen](#resumen)
12. [Ver también](#ver-también)

---

## Introducción

`MapIf` es el `if / else` **dentro** de la tubería, aplicado a **transformaciones puras**:

> **Si el resultado es válido, evalúa la condición sobre el valor y aplica una transformación u otra. Si es fallido, el fallo pasa intacto y la condición ni se evalúa.**

```csharp
// ❌ Sacar el valor de la tubería para poder decidir
var r = ObtenerPrecio(articuloId);
decimal final;
if (r.IsValid)
{
    var p = r.Value;                          // acceso interno, no disponible
    final = p.EsPromocion ? p.Base * 0.8m : p.Base;
}

// ✅ Decidir sin salir de la tubería
MlResult<decimal> r = ObtenerPrecio(articuloId)
                        .MapIf(p => p.EsPromocion,
                               p => p.Base * 0.8m,     // rama true
                               p => p.Base);           // rama false
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`.

---

## Las dos formas de `MapIf`

La región `MapIf` del fuente contiene **dos familias distintas**, y confundirlas es el error más habitual.

| | Forma A — bifurcación | Forma B — transformación opcional |
|---|---|---|
| **Genéricos** | `<T, TReturn>` | `<T>` |
| **Delegados** | `funcTrue` **y** `funcFalse` | solo `func` |
| **Tipo de salida** | Puede cambiar: `MlResult<TReturn>` | **El mismo**: `MlResult<T>` |
| **Si la condición es falsa** | Ejecuta `funcFalse(x)` | Devuelve `x` **sin tocarlo** |
| **Lectura** | «una cosa **o** la otra» | «aplica esto **solo si**…» |

```csharp
// Forma A: dos caminos, el tipo puede cambiar
MlResult<string> etiqueta = pedido.MapIf(p => p.EsUrgente,
                                         p => $"URGENTE-{p.Numero}",
                                         p => p.Numero.ToString());

// Forma B: un solo camino opcional, el tipo se mantiene
MlResult<Pedido> conRecargo = pedido.MapIf(p => p.EsUrgente,
                                           p => p with { Total = p.Total * 1.15m });
```

El propio autor lo dejó escrito como comentario en el fuente, justo antes de la Forma B:

> *«En este grupo, no añado función de parámetro para cuando no se cumple la comprobación, ya que al ser el mismo tipo no sería necesario. Si quisiera hacer algo en caso de no cumplirse, me valdrían las sobrecargas de arriba.»*

---

## Firmas reales e implementación

### Forma A — `<T, TReturn>` con las dos ramas

```csharp
public static MlResult<TReturn> MapIf<T, TReturn>(this MlResult<T>      source,
                                                       Func<T, bool>    condition,
                                                       Func<T, TReturn> funcTrue,
                                                       Func<T, TReturn> funcFalse)
{
    var result = source.Match(
                                 valid: x => condition(x) ? funcTrue(x) : funcFalse(x),
                                 fail :      MlResult<TReturn>.Fail
                           );
    return result;
}
```

### Forma B — `<T>` con una sola rama

```csharp
public static MlResult<T> MapIf<T>(this MlResult<T>   source,
                                        Func<T, bool> condition,
                                        Func<T, T>    func)
{
    var result = source.Match(
                                 valid: x => condition(x) ? func(x) : x,
                                 fail :      MlResult<T>.Fail
                           );
    return result;
}
```

Fíjate en dos detalles del código real:

1. **`fail: MlResult<TReturn>.Fail` es un grupo de métodos**, no una lambda. Se apoya en la sobrecarga que recibe `MlErrorsDetails`, así que **los errores y todos sus detalles se conservan íntegros** al cambiar de tipo genérico.
2. Los delegados devuelven `TReturn`/`T` **desnudo**, no `MlResult<...>`. La conversión implícita del tipo hace el resto. Esto es lo que convierte a `MapIf` en un `Map` condicional y no en un `Bind`.

| Estado de entrada | `condition(x)` | Forma A | Forma B |
|---|---|---|---|
| Válido | `true` | `funcTrue(x)` | `func(x)` |
| Válido | `false` | `funcFalse(x)` | `x` sin cambios |
| Fallido | **No se evalúa** | Mismo fallo | Mismo fallo |

---

## Por qué la Forma B no necesita `funcFalse`

Porque en la Forma B **entrada y salida son el mismo tipo `T`**, y por tanto existe una «rama falsa» obvia y única: *no hacer nada*. En la Forma A eso es imposible: si `T` es `Pedido` y `TReturn` es `string`, no hay forma automática de convertir un `Pedido` en `string` cuando la condición no se cumple, así que la rama falsa es obligatoria.

Consecuencia práctica muy útil: **la Forma B es apilable**, porque conserva el tipo.

```csharp
MlResult<Pedido> ajustado = pedido.ToMlResultValid()
    .MapIf(p => p.EsUrgente,                p => p with { Total = p.Total * 1.15m })
    .MapIf(p => p.Total > 1000,             p => p with { Descuento = 0.05m })
    .MapIf(p => p.Cliente.EsVip,            p => p with { Descuento = p.Descuento + 0.05m })
    .MapIf(p => p.PaisEnvio != "ES",        p => p with { GastosEnvio = 25m });
```

Cada línea es una regla de negocio independiente y legible. **No hay cortocircuito entre ellas**: se evalúan todas (siempre que el resultado siga siendo válido), a diferencia de lo que ocurre con `MapEnsure`.

---

## `MapIf` frente a `BindIf` y `MapEnsure`

Tres operaciones condicionales que responden a preguntas distintas:

| Operación | Pregunta que responde | Devuelve el delegado |
|---|---|---|
| `MapIf` | «¿qué **transformación** aplico?» | Un valor desnudo (`TReturn`) |
| [`BindIf`](../Bind/5_BindIf.md) | «¿qué **operación que puede fallar** ejecuto?» | Un `MlResult<TReturn>` |
| [`MapEnsure`](2_MapEnsure.md) | «¿el valor **es válido**?» | Nada: solo un `bool` |

```csharp
// MapIf: la decisión no puede fallar (cálculo puro)
.MapIf(p => p.EsUrgente, p => p.Total * 1.15m, p => p.Total)

// BindIf: cada rama puede fallar (va a base de datos)
.BindIf(p => p.EsUrgente, p => TramitarUrgente(p), p => TramitarNormal(p))

// MapEnsure: no transforma, solo decide si el valor sigue
.MapEnsure(p => p.Total > 0, "El importe debe ser positivo")
```

> 📌 **Regla de oro**: si tu delegado devuelve `MlResult<...>` y usas `MapIf`, obtendrás un `MlResult<MlResult<...>>` anidado. Eso es la señal de que necesitabas `BindIf`. Lo explica en detalle [`1_Map.md`](1_Map.md).

---

## Variantes asíncronas

`MapIfAsync` tiene **12 sobrecargas**, resultado de combinar el tipo de origen con la asincronía de cada rama.

### Forma A — 8 sobrecargas

| Origen | `funcTrue` | `funcFalse` |
|---|---|---|
| `MlResult<T>` | sync | sync |
| `MlResult<T>` | **async** | sync |
| `MlResult<T>` | sync | **async** |
| `MlResult<T>` | **async** | **async** |
| `Task<MlResult<T>>` | sync | sync |
| `Task<MlResult<T>>` | **async** | sync |
| `Task<MlResult<T>>` | sync | **async** |
| `Task<MlResult<T>>` | **async** | **async** |

### Forma B — 4 sobrecargas

| Origen | `func` |
|---|---|
| `MlResult<T>` | sync |
| `MlResult<T>` | **async** |
| `Task<MlResult<T>>` | sync |
| `Task<MlResult<T>>` | **async** |

Las combinaciones mixtas usan un pequeño truco para homogeneizar tipos: la rama síncrona se envuelve con `.ToAsync()`.

```csharp
public static async Task<MlResult<TReturn>> MapIfAsync<T, TReturn>(this MlResult<T>            source,
                                                                        Func<T, bool>          condition,
                                                                        Func<T, Task<TReturn>> funcTrueAsync,
                                                                        Func<T, TReturn>       funcFalse)
{
    var result = await source.MatchAsync<T, MlResult<TReturn>>(
                                       validAsync: async x => condition(x) ? await funcTrueAsync(x)
                                                                           : await funcFalse(x).ToAsync(),
                                       fail      :            MlResult<TReturn>.Fail
                                  );
    return result;
}
```

> ⚠️ **La condición es siempre síncrona.** No hay ninguna sobrecarga con `Func<T, Task<bool>>`. Si decidir requiere una consulta, resuélvela antes con `BindAsync` y transporta la respuesta en el valor (por ejemplo, en una tupla).

---

## `TryMapIf` — cuando la transformación puede lanzar

`TryMapIf` envuelve **las dos ramas** en captura de excepciones. La clave está en `TryToMlResult`:

```csharp
public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T>             source,
                                                          Func<T, bool>           condition,
                                                          Func<T, TReturn>        funcTrue,
                                                          Func<T, TReturn>        funcFalse,
                                                          Func<Exception, string> errorMessageBuilder)
{
    var result = source.Match(
                                 valid: x => condition(x)
                                                ? funcTrue .TryToMlResult(x, errorMessageBuilder)
                                                : funcFalse.TryToMlResult(x, errorMessageBuilder),
                                 fail :      MlResult<TReturn>.Fail
                           );
    return result;
}

// Sobrecarga con mensaje fijo
public static MlResult<TReturn> TryMapIf<T, TReturn>(this MlResult<T> source, Func<T, bool> condition,
                                                          Func<T, TReturn> funcTrue, Func<T, TReturn> funcFalse,
                                                          string exceptionAditionalMessage = null!)
    => source.TryMapIf(condition, funcTrue, funcFalse, _ => exceptionAditionalMessage!);
```

`TryToMlResult` captura la excepción, construye el mensaje con tu `errorMessageBuilder` y **guarda la excepción original en `Details["Ex"]`**. Eso significa que después puedes recuperarla:

```csharp
var r = dato.ToMlResultValid()
            .TryMapIf(d => d.EsJson,
                      d => JsonSerializer.Deserialize<Config>(d.Contenido)!,
                      d => ConfigParser.DesdeIni(d.Contenido),
                      ex => $"No se pudo interpretar la configuración: {ex.Message}");

r.Match(valid: cfg     => Aplicar(cfg),
        fail : errores => errores.GetDetailException()
                                 .Match(valid: ex => _log.LogError(ex, "Fallo de parseo"),
                                        fail : _  => _log.LogWarning("{Msg}", errores.ToErrorsDescription())));
```

> ⚠️ **La condición no está protegida.** `condition(x)` se evalúa **fuera** del `TryToMlResult`. Si tu predicado lanza, la excepción escapa de la tubería. Mantenlo trivial.

Recuento de sobrecargas `Try`: **Forma A** 2 síncronas y 19 asíncronas; **Forma B** 2 síncronas y 6 asíncronas.

---

## ⚠️ Particularidad real del código fuente: `TryMapIAsyncf`

En la región `MapIf` existen **tres métodos con el nombre mal escrito**: `TryMapIAsyncf` en lugar de `TryMapIfAsync` (líneas 483, 498 y 505 del fuente).

```csharp
public static async Task<MlResult<TReturn>> TryMapIAsyncf<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                          ...
```

Son las sobrecargas para **origen asíncrono** de la Forma A. Funcionan perfectamente, pero:

- **No las encontrarás** escribiendo `TryMapIfAsync` y esperando que IntelliSense las ofrezca para un `Task<MlResult<T>>`.
- Si un día se corrige el nombre, será un **cambio incompatible**.

**Recomendación práctica**: evita depender de ese nombre. Aísla el `await` y llama a la versión síncrona de origen:

```csharp
// ❌ Depende del nombre erróneo
await tareaResultado.TryMapIAsyncf(cond, funcTrue, funcFalse, "mensaje");

// ✅ Estable frente a la corrección del typo
var resultado = await tareaResultado;
var final     = resultado.TryMapIf(cond, funcTrue, funcFalse, "mensaje");
```

---

## Ejemplos Prácticos

### Ejemplo 1: Reglas de precio acumulativas (Forma B apilada)

```csharp
public MlResult<Presupuesto> AplicarTarifas(Presupuesto presupuesto)
    => presupuesto.ToMlResultValid()

        // Cada regla es independiente y solo actúa si procede
        .MapIf(p => p.Cliente.EsVip,
               p => p with { Descuento = p.Descuento + 0.10m })

        .MapIf(p => p.Importe >= 5_000m,
               p => p with { Descuento = p.Descuento + 0.05m })

        .MapIf(p => p.Lineas.Count >= 20,
               p => p with { Descuento = p.Descuento + 0.02m })

        // Techo de descuento: también es una transformación condicional
        .MapIf(p => p.Descuento > 0.20m,
               p => p with { Descuento = 0.20m })

        // Recargo por envío internacional
        .MapIf(p => p.PaisEnvio != "ES",
               p => p with { GastosEnvio = p.GastosEnvio + 30m })

        // Y el cálculo final, que ya no es condicional
        .Map(p => p with { Total = p.Importe * (1 - p.Descuento) + p.GastosEnvio });
```

Compara la legibilidad con el equivalente imperativo: aquí **cada regla ocupa exactamente dos líneas** y se puede añadir, quitar o reordenar sin tocar las demás.

### Ejemplo 2: Enrutar el envío según el destino (Forma A + async)

```csharp
public async Task<MlResult<Envio>> PrepararEnvioAsync(int pedidoId)
    => await _repo.ObtenerPedidoAsync(pedidoId)

        .MapEnsureAsync(p => p.Lineas.Any(), "El pedido no tiene líneas")

        // Forma A asíncrona: dos ramas que devuelven el MISMO tipo de destino
        .MapIfAsync(p => p.PaisEnvio == "ES",
                    async p => await _nacional.CalcularAsync(p),        // rama true  (async)
                    async p => await _internacional.CalcularAsync(p))   // rama false (async)

        .MapEnsureAsync(e => e.Coste >= 0,
                        e => $"Coste de envío no válido: {e.Coste:C}")

        .AddMlErrorDetailIfFailAsync($"[Envíos] Fallo al preparar el envío del pedido {pedidoId}");
```

> 💡 Si `CalcularAsync` devolviera `Task<MlResult<Envio>>` en vez de `Task<Envio>`, esto sería un caso de [`BindIf`](../Bind/5_BindIf.md), **no** de `MapIf`.

### Ejemplo 3: Normalizar entrada del usuario con captura de excepciones

```csharp
public MlResult<DateTime> InterpretarFecha(string entrada)
    => EnsureFp.NotNullEmptyOrWhitespace(entrada, "La fecha es obligatoria")

        // Limpieza opcional (Forma B): solo si viene con espacios sobrantes
        .MapIf(s => s != s.Trim(), s => s.Trim())

        // Dos formatos posibles, ambos pueden lanzar → TryMapIf
        .TryMapIf(s => s.Contains('/'),
                  s => DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                  s => DateTime.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                  ex => $"El texto '{entrada}' no es una fecha válida (formatos admitidos: dd/MM/yyyy o yyyy-MM-dd). Detalle: {ex.Message}")

        // Validación de rango, ya con la fecha construida
        .MapEnsure(f => f <= DateTime.Today,
                   f => $"La fecha {f:dd/MM/yyyy} es futura y no se admite");
```

### Ejemplo 4: Qué no hacer

```csharp
// ❌ 1. Delegado que devuelve MlResult<> con MapIf → anidamiento
MlResult<MlResult<Factura>> mal = pedido.MapIf(p => p.EsUrgente,
                                               p => _repo.FacturarUrgente(p),   // devuelve MlResult<Factura>
                                               p => _repo.FacturarNormal(p));
// ✅ Usa BindIf
MlResult<Factura> bien = pedido.BindIf(p => p.EsUrgente,
                                       p => _repo.FacturarUrgente(p),
                                       p => _repo.FacturarNormal(p));


// ❌ 2. Usar MapIf para validar: la "rama falsa" fabricando un fallo a mano
var mal2 = pedido.MapIf(p => p.Total > 0,
                        p => p,
                        p => throw new InvalidOperationException("Importe no válido"));
// ✅ Eso es exactamente para lo que existe MapEnsure
var bien2 = pedido.MapEnsure(p => p.Total > 0, "El importe debe ser positivo");


// ❌ 3. Condición con efectos secundarios o que puede lanzar
var mal3 = dato.MapIf(d => int.Parse(d.Codigo) > 100, ...);   // la excepción ESCAPA
// ✅ Convierte primero, decide después
var bien3 = dato.TryMap(d => int.Parse(d.Codigo), ex => $"Código no numérico: {ex.Message}")
                .MapIf(c => c > 100, c => c * 2, c => c);


// ❌ 4. Depender del nombre mal escrito TryMapIAsyncf
await tarea.TryMapIAsyncf(cond, fT, fF, "msg");
// ✅ Resuelve el await y usa el nombre estable
var r = await tarea;
var f = r.TryMapIf(cond, fT, fF, "msg");
```

---

## Mejores Prácticas

1. **Elige la forma por el tipo de salida, no por costumbre.** Si el tipo no cambia, la Forma B (`MapIf(condition, func)`) es más corta y más clara: expresa «aplica esto solo si…».

2. **Apila Formas B para modelar reglas de negocio.** Una regla por línea, independiente y reordenable. Recuerda que **se evalúan todas**, no hay cortocircuito.

3. **Mantén la condición pura, síncrona y trivial.** No está protegida por `TryMapIf` y no admite versión asíncrona.

4. **Si el delegado devuelve `MlResult<...>`, cambia a [`BindIf`](../Bind/5_BindIf.md).** El síntoma inequívoco es un tipo anidado `MlResult<MlResult<...>>`.

5. **No uses `MapIf` para validar.** Para eso está [`MapEnsure`](2_MapEnsure.md), que además produce un error con mensaje en lugar de obligarte a lanzar.

6. **Con `TryMapIf`, usa el `Func<Exception, string>`** en lugar del `string` fijo: el mensaje de la excepción es casi siempre la parte más útil del diagnóstico.

7. **Evita `TryMapIAsyncf`** en código nuevo: aísla el `await` y llama a `TryMapIf`.

8. **Si ambas ramas comparten casi todo el código**, probablemente lo que quieres es un `Map` con la condición dentro del cálculo. `MapIf` brilla cuando las ramas son realmente distintas.

---

## Resumen

- `MapIf` aplica una **transformación pura** condicionada al valor, sin salir de la tubería.
- Hay **dos formas**: la **A** (`<T, TReturn>`, con `funcTrue` y `funcFalse`, el tipo puede cambiar) y la **B** (`<T>`, con un solo `func`, el tipo se mantiene y si la condición es falsa el valor pasa intacto).
- La Forma B no necesita `funcFalse` porque «no hacer nada» es una rama falsa válida cuando entrada y salida son del mismo tipo; por eso **es apilable**.
- Si el resultado ya venía **fallido**, la condición **no se evalúa** y el fallo se propaga con todos sus detalles (`fail: MlResult<TReturn>.Fail` es un grupo de métodos).
- Recuento: `MapIf` **2** síncronas y **12** asíncronas; `TryMapIf` **4** síncronas y **19 + 6** asíncronas.
- **La condición nunca es asíncrona** ni está protegida contra excepciones.
- `TryMapIf` protege **las dos ramas** con `TryToMlResult` y deja la excepción en `Details["Ex"]`.
- ⚠️ Tres sobrecargas del fuente se llaman **`TryMapIAsyncf`** (typo por `TryMapIfAsync`): úsalas con cautela o evítalas.
- Si el delegado devuelve `MlResult<...>`, la herramienta correcta es [`BindIf`](../Bind/5_BindIf.md); si solo quieres validar, [`MapEnsure`](2_MapEnsure.md).

---

## Ver también

- [`1_Map.md`](1_Map.md) — transformación incondicional y la regla `Map` vs `Bind`.
- [`2_MapEnsure.md`](2_MapEnsure.md) — validar sin transformar.
- [`4_MapIfFail.md`](4_MapIfFail.md) — transformar en la rama de fallo.
- [`8_MapAlways.md`](8_MapAlways.md) — transformar sea válido o fallido.
- [`../Bind/5_BindIf.md`](../Bind/5_BindIf.md) — bifurcar hacia operaciones que pueden fallar.
- [`../Types/MlResultTransformations.md`](../Types/MlResultTransformations.md) — cómo funciona `TryToMlResult`.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la clase.