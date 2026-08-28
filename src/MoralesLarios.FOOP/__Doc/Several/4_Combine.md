# Combine — Acumular valores en una tupla a lo largo del carril

## Índice

1. [Introducción](#introducción)
2. [⚠️ Lo primero: `Combine` NO acumula errores](#️-lo-primero-combine-no-acumula-errores)
3. [El problema que resuelve: el infierno de los `Bind` anidados](#el-problema-que-resuelve-el-infierno-de-los-bind-anidados)
4. [Las tres familias de `Combine`](#las-tres-familias-de-combine)
5. [Familia A: `MlResult<T>` + valor(es) sueltos](#familia-a-mlresultt--valores-sueltos)
6. [Familia B: valor/tupla + `MlResult<T>`](#familia-b-valortupla--mlresultt)
7. [Familia C: valor/tupla + valor (sin `MlResult`)](#familia-c-valortupla--valor-sin-mlresult)
8. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
9. [Variantes asíncronas](#variantes-asíncronas)
10. [Apéndice: `Do`, el operador de escape](#apéndice-do-el-operador-de-escape)
11. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
12. [Ejemplos Prácticos](#ejemplos-prácticos)
13. [Mejores Prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## Introducción

`Combine` sirve para **arrastrar varios valores a la vez por el carril**, agrupándolos en
una tupla de C#. Es la respuesta al problema clásico de la programación funcional: cuando
el paso 5 de una tubería necesita el resultado del paso 1, del 2 y del 4.

```csharp
// ❌ Bind anidados para conservar valores anteriores: ilegible
return ObtenerCliente(id)
        .Bind(cliente => ObtenerTarifa(cliente.TarifaId)
            .Bind(tarifa => ObtenerDescuento(cliente.Id)
                .Bind(descuento => ObtenerImpuestos(cliente.Pais)
                    .Map(impuestos => Calcular(cliente, tarifa, descuento, impuestos)))));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## ⚠️ Lo primero: `Combine` NO acumula errores

Es el malentendido más frecuente con este método. En otras librerías funcionales, un
`Combine`/`Apply`/`Zip` recoge **todos** los errores de todos los operandos. **Aquí no.**

```csharp
// Implementación real, sin adornos
public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(
        this MlResult<TResult1> source, TResult2 otherValue)
    => source.Match(
           valid: x            => MlResult<(TResult1, TResult2)>.Valid((x, otherValue)),
           fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2)>()
       );
```

Lo que hace es **exactamente**:

| Situación | Resultado |
|-----------|-----------|
| El `MlResult` operando es válido | `Valid` con la tupla ampliada |
| El `MlResult` operando es fallido | `Fail` con **los errores de ese operando y nada más** |

🔑 **`Combine` es un cortocircuito, como `Bind`.** Solo hay un `MlResult` en juego en cada
llamada; el otro operando es un valor normal. Por tanto no hay nada que "acumular".

```csharp
// ❌ Expectativa equivocada: esperar los dos errores
var a = MlResult<int>.Fail("Error A");
var r = a.Combine(otroValor);            // solo contiene "Error A" — correcto, solo hay uno

// ⚠️ Y aquí, aunque haya dos MlResult, tampoco se acumulan:
var b = MlResult<string>.Fail("Error B");
var r2 = a.Combine(b);                   // ⚠️ NO compila como esperas: 'b' entra como TResult2
                                         //    → MlResult<(int, MlResult<string>)>
```

**Si necesitas acumular errores de varias validaciones independientes**, la librería ofrece
otras herramientas:

- Recoger los errores de cada validación y fusionarlos con `MlErrorsDetails.FromErrorsDetails(...)`
- Los métodos de la carpeta [`Bucle`](../Bucle/Bucles.md), que acumulan errores por elemento
- `MergeErrorsDetailsIfFail(...)` para arrastrar los errores previos al añadir uno nuevo

---

## El problema que resuelve: el infierno de los `Bind` anidados

En una tubería lineal, cada paso solo ve el resultado del paso anterior. Cuando un paso
tardío necesita datos de varios pasos previos, tienes tres opciones:

1. **Anidar `Bind`** — el "callback hell" funcional: la indentación crece sin control.
2. **Crear un DTO intermedio** por cada combinación de datos — mucho código ceremonial.
3. **Usar `Combine`** — arrastrar una tupla que crece paso a paso.

`Combine` es la opción pragmática: no necesitas declarar tipos y la tubería se mantiene
plana.

```csharp
// La tupla crece: 1 → 2 → 3 → 4 elementos
MlResult<Cliente>                                    paso1 = ObtenerCliente(id);
MlResult<(Cliente, Tarifa)>                          paso2 = paso1.Combine(tarifa);
MlResult<(Cliente, Tarifa, Descuento)>               paso3 = paso2.Combine(descuento);
MlResult<(Cliente, Tarifa, Descuento, Impuestos)>    paso4 = paso3.Combine(impuestos);
```

---

## Las tres familias de `Combine`

El código fuente define tres grupos distintos, según **qué lado lleva el `MlResult`**:

| Familia | Firma característica | Qué hace | Puede fallar |
|---------|---------------------|----------|--------------|
| **A** | `MlResult<T1>.Combine(valorOTupla)` | Añade 1 a 7 valores sueltos al resultado | Sí, si `source` falla |
| **B** | `valorOTupla.Combine(MlResult<Tn>)` | Añade el valor de un `MlResult` a una tupla existente | Sí, si el `MlResult` falla |
| **C** | `valorOTupla.Combine(valor)` | Construye la tupla y la marca válida | **Nunca falla** |

Las tres llegan hasta **8 elementos** en la tupla.

```csharp
// A: parto de un MlResult y añado valores ya disponibles
MlResult<(Cliente, Tarifa)> a = clienteResult.Combine(tarifaYaObtenida);

// B: parto de una tupla de valores y añado un MlResult
MlResult<(Cliente, Tarifa)> b = cliente.Combine(tarifaResult);

// C: solo agrupo valores, sin ningún MlResult implicado
MlResult<(Cliente, Tarifa)> c = cliente.Combine(tarifa);   // siempre Valid
```

---

## Familia A: `MlResult<T>` + valor(es) sueltos

Es la más usada. El primer operando es el `MlResult` que viaja por el carril; el segundo es
un valor (o una tupla de valores) que ya tienes a mano.

```csharp
// 2 elementos: el segundo operando es un valor suelto
public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(
        this MlResult<TResult1> source, TResult2 otherValue)

// 3 a 8 elementos: el segundo operando es una TUPLA de valores
public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(
        this MlResult<TResult1> source, (TResult2 value1, TResult3 value2) values)
```

🔑 **Detalle crucial:** a partir de 3 elementos, el segundo argumento es **una tupla**, no
una lista de argumentos. Se añaden **todos de golpe** al valor del `MlResult`:

```csharp
// ✅ Añade DOS valores de una vez → tupla de 3
MlResult<(Cliente, Tarifa, Descuento)> r = clienteResult.Combine((tarifa, descuento));

// ✅ Equivalente encadenando (nótese: aquí entra en juego la familia B/C)
MlResult<(Cliente, Tarifa, Descuento)> r2 = clienteResult
        .Combine(tarifa)                      // familia A, 2 elementos
        .Bind(t => t.Combine(descuento));     // familia C sobre la tupla
```

**Nombres de los campos:** solo la sobrecarga de 2 elementos declara nombres
(`value1`, `value2`). De 3 a 8, el tipo de retorno es una tupla **sin nombres**, así que
accedes con `Item1`, `Item2`, `Item3`…

```csharp
// 2 elementos: nombres disponibles
clienteResult.Combine(tarifa).Map(t => $"{t.value1.Nombre} — {t.value2.Codigo}");

// 3+ elementos: solo Item1, Item2, Item3…
clienteResult.Combine((tarifa, descuento))
             .Map(t => $"{t.Item1.Nombre} — {t.Item2.Codigo} — {t.Item3.Porcentaje}");

// 💡 Truco: deconstruye para dar nombres legibles
clienteResult.Combine((tarifa, descuento))
             .Map(t =>
             {
                 var (cliente, tar, desc) = t;
                 return $"{cliente.Nombre} — {tar.Codigo} — {desc.Porcentaje}";
             });
```

---

## Familia B: valor/tupla + `MlResult<T>`

Aquí el `MlResult` es el **segundo** operando. Sirve para el caso inverso: ya tienes una
tupla de valores válidos y quieres añadirle el resultado de una operación que puede fallar.

```csharp
// valor + MlResult → tupla de 2
public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(
        this TResult1 source, MlResult<TResult2> mlResultValue)
    => mlResultValue.Match(
           valid: x            => MlResult<(TResult1, TResult2)>.Valid((source, x)),
           fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2)>()
       );

// tupla de 2 + MlResult → tupla de 3  (y así hasta 8)
public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)> Combine<TResult1, TResult2, TResult3>(
        this (TResult1 value1, TResult2 value2) source, MlResult<TResult3> mlResultValue)
```

Uso típico dentro de un `Bind`, cuando el valor que falta viene de una consulta:

```csharp
var resultado = clienteResult
        .Bind(cliente => cliente.Combine(ObtenerTarifa(cliente.TarifaId)))       // (Cliente, Tarifa)
        .Bind(t       => t.Combine(ObtenerDescuento(t.value1.Id)))               // (Cliente, Tarifa, Descuento)
        .Map(t => Calcular(t.Item1, t.Item2, t.Item3));
```

🔑 En esta familia **los campos sí llevan nombres** (`value1`, `value2`, `value3`…) en todas
las aridades, a diferencia de la familia A. Es una asimetría real del código fuente.

---

## Familia C: valor/tupla + valor (sin `MlResult`)

Estas sobrecargas **nunca fallan**: se limitan a construir la tupla y envolverla en un
resultado válido.

```csharp
public static MlResult<(TResult1, TResult2)> Combine<TResult1, TResult2>(
        this TResult1 source, TResult2 value)
    => (source, value).ToMlResultValid();

public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(
        this (TResult1 value1, TResult2 value2) source, TResult3 newValue)
    => (source.value1, source.value2, newValue).ToMlResultValid();
// … hasta 8 elementos
```

Sirven para **ampliar una tupla dentro de un `Bind`** sin ceremonia:

```csharp
// Dentro del carril: la tupla crece de 2 a 3 elementos
var r = paresResult.Bind(t => t.Combine(nuevoValor));   // familia C, siempre Valid
```

⚠️ Estas sobrecargas devuelven tuplas **sin nombres** (`Item1`, `Item2`…), igual que la
familia A de 3+ elementos.

---

## ⚠️ Particularidades reales del código fuente

**1. `Combine` cortocircuita, no acumula.** Ya explicado arriba: es el punto más
importante.

**2. Los nombres de los campos de la tupla son inconsistentes entre familias.**

| Familia | Aridad | ¿Campos con nombre? |
|---------|--------|--------------------|
| A | 2 | ✅ `value1`, `value2` |
| A | 3–8 | ❌ solo `Item1`, `Item2`, … |
| B | 2–8 | ✅ `value1`, `value2`, … |
| C | 2–8 | ❌ solo `Item1`, `Item2`, … |

Recomendación: **deconstruye siempre** (`var (a, b, c) = t;`) para no depender de esta
asimetría y ganar legibilidad.

**3. Algunas sobrecargas asíncronas de la familia B devuelven tipos nullable.**
Concretamente las que parten de una tupla:
`Task<MlResult<(...)>?>`. Igual que en `EmptyToFailed`, **nunca devuelven `null`**; es una
anotación heredada. La propia librería usa `.ToAsync()!` internamente.

**4. No existe `TryCombine`.** `Combine` no invoca delegados de usuario, así que no hay
excepciones que capturar.

**5. Los valores del segundo operando se evalúan siempre.**
Como son valores y no delegados, se calculan **antes** de la llamada, incluso si el
`MlResult` ya venía fallido:

```csharp
// ⚠️ ConsultaCostosa() se ejecuta aunque clienteResult sea Fail
var r = clienteResult.Combine(ConsultaCostosa());

// ✅ Si el valor es costoso, obtenlo dentro de un Bind (que sí cortocircuita)
var r = clienteResult.Bind(c => c.Combine(ConsultaCostosa()));
```

Esta es la razón principal para preferir el patrón `Bind(x => x.Combine(...))` (familias B
y C) frente a la familia A cuando el segundo operando implique trabajo real.

**6. El límite es 8 elementos.** Si necesitas más, es señal de que deberías introducir un
tipo propio (`record`) en lugar de seguir ampliando la tupla.

---

## Variantes asíncronas

Cada sobrecarga síncrona tiene dos hermanas asíncronas:

| Patrón | Naturaleza |
|--------|-----------|
| `CombineAsync(this MlResult<T> source, valor)` | Envoltura: `source.Combine(valor).ToAsync()` |
| `CombineAsync(this Task<MlResult<T>> sourceAsync, valor)` | **Espera el origen** |
| `CombineAsync(this T source, MlResult<Tn>)` | Envoltura |
| `CombineAsync(this T source, Task<MlResult<Tn>>)` | **Espera el `MlResult`** |

```csharp
// Encadenamiento asíncrono completo
var resultado = await ObtenerClienteAsync(id)
                      .CombineAsync(tarifaYaObtenida)                 // Task<MlResult<...>> + valor
                      .BindAsync(async t => await t.CombineAsync(ObtenerDescuentoAsync(t.value1.Id)))
                      .MapAsync(t => Calcular(t.Item1, t.Item2, t.Item3).ToAsync());
```

⚠️ La familia C **no tiene variantes asíncronas**: al no poder fallar ni esperar nada, no
tendría sentido.

---

## Apéndice: `Do`, el operador de escape

En el mismo archivo del código fuente, junto a `Combine`, vive un método pequeño pero
llamativo:

```csharp
public static MlResult<TResult> Do<T, TResult>(this MlResult<T> source,
                                                    Func<MlResult<T>, MlResult<TResult>> action)
    => action(source);

public static async Task<MlResult<TResult>> DoAsync<T, TResult>(this MlResult<T> source,
                                                    Func<MlResult<T>, Task<MlResult<TResult>>> actionAsync)
    => await actionAsync(source);

public static async Task<MlResult<TResult>> DoAsync<T, TResult>(this Task<MlResult<T>> sourceAsync,
                                                    Func<Task<MlResult<T>>, Task<MlResult<TResult>>> actionAsync)
    => await actionAsync(sourceAsync);
```

`Do` **invoca tu delegado pasándole el `MlResult` completo**, sin comprobar si es válido o
fallido. No usa `Match`, no cortocircuita, no envuelve nada:

| Método | Recibe el delegado | ¿Cortocircuita? |
|--------|-------------------|-----------------|
| `Map` | El **valor** `T` | ✅ Sí |
| `Bind` | El **valor** `T` | ✅ Sí |
| `Match` | Valor **o** errores, según la rama | ✅ Sí (elige rama) |
| **`Do`** | El **`MlResult<T>` entero** | ❌ **No** |

Es un **operador de escape** para insertar en la tubería una función que necesita ver el
resultado completo, típicamente porque va a decidir por sí misma cómo tratar el fallo:

```csharp
// Insertar una política propia en medio de la tubería
var r = ObtenerCliente(id)
            .Do(res => _politicaReintentos.Aplicar(res))     // ve el MlResult completo
            .Map(c => c.ToDto());

// ⚠️ La tercera sobrecarga pasa el Task SIN esperar: tu delegado decide cuándo hacer await
var r = await ObtenerClienteAsync(id)
                  .DoAsync(async tarea => await _politica.AplicarAsync(tarea));
```

💡 **Cuándo usarlo:** casi nunca. Si tu intención es reaccionar al fallo, usa
[`MapIfFail`](../Map/4_MapIfFail.md) o [`BindIfFail`](../Bind/6_BindIfFail.md); si es
observar sin alterar, [`ExecSelf`](../ExecSelf/1_ExecSelf.md). `Do` solo se justifica para
integrar funciones externas cuya firma ya trabaja con `MlResult<T>`.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Arrastrar 2–8 valores por el carril | `Combine` |
| Añadir un valor ya disponible a un `MlResult` | Familia A: `resultado.Combine(valor)` |
| Añadir el resultado de una consulta a una tupla | Familia B: `.Bind(t => t.Combine(consulta()))` |
| Ampliar una tupla con un valor seguro | Familia C: `.Bind(t => t.Combine(valor))` |
| Evitar que el segundo operando se evalúe si hay fallo | Envuelve en `Bind` (familias B/C) |
| **Acumular** los errores de varias validaciones | [`Bucles`](../Bucle/Bucles.md), `MlErrorsDetails.FromErrorsDetails` |
| Más de 8 valores | Define un `record` propio |
| Transformar el valor | [`Map`](../Map/1_Map.md) |
| Encadenar una operación que puede fallar | [`Bind`](../Bind/3_Bind.md) |
| Ver el `MlResult` completo dentro de la tubería | `Do` (raramente necesario) |

---

## Ejemplos Prácticos

### Ejemplo 1: cálculo de precio con cuatro dependencias

```csharp
public class CalculadoraPrecio
{
    public async Task<MlResult<PrecioFinal>> CalcularAsync(int clienteId, int articuloId, int cantidad)
        => await EnsureFp.That(cantidad, cantidad > 0, "La cantidad debe ser positiva")
                         .BindAsync(_ => _clientes.ObtenerAsync(clienteId)
                                                  .NullToFailedAsync($"Cliente {clienteId} no encontrado"))
                         // (Cliente, Articulo)
                         .BindAsync(async cliente => cliente.Combine(
                                        await _articulos.ObtenerAsync(articuloId)
                                                        .NullToFailedAsync($"Artículo {articuloId} no encontrado")))
                         // (Cliente, Articulo, Tarifa)
                         .BindAsync(async t => await t.CombineAsync(_tarifas.VigenteAsync(t.value1.TarifaId)))
                         // (Cliente, Articulo, Tarifa, Impuesto)
                         .BindAsync(async t => await t.CombineAsync(_impuestos.ParaPaisAsync(t.value1.Pais)))
                         .MapAsync(t =>
                         {
                             var (cliente, articulo, tarifa, impuesto) = t;
                             var baseImponible = tarifa.PrecioDe(articulo) * cantidad;
                             var conDescuento  = baseImponible * (1 - cliente.Descuento);
                             return new PrecioFinal(conDescuento, conDescuento * impuesto.Tipo).ToAsync();
                         });
}
```

Fíjate en el patrón: cada paso usa `Bind(... Combine ...)` para que las consultas **no se
ejecuten si algo ya falló**, y se deconstruye la tupla al final para dar nombres legibles.

### Ejemplo 2: informe con datos de varias fuentes

```csharp
public async Task<MlResult<Informe>> GenerarAsync(int ejercicio, string departamento)
{
    // Lanzamos las consultas en paralelo y las esperamos antes de combinar
    var tVentas    = _ventas.DelEjercicioAsync(ejercicio, departamento);
    var tGastos    = _gastos.DelEjercicioAsync(ejercicio, departamento);
    var tPlantilla = _rrhh.PlantillaMediaAsync(ejercicio, departamento);

    await Task.WhenAll(tVentas, tGastos, tPlantilla);

    // Familia A con tupla: los tres valores ya están resueltos
    return (await tVentas)
                .Combine((await tGastos, await tPlantilla))          // (Ventas, Gastos, Plantilla)
                .MapEnsure(t => t.Item1.Any(), "No hay ventas registradas en el ejercicio")
                .Map(t =>
                {
                    var (ventas, gastos, plantilla) = t;
                    return new Informe(ejercicio, departamento,
                                       ventas.Sum(v => v.Importe),
                                       gastos.Sum(g => g.Importe),
                                       plantilla);
                })
                .ExecSelfIfFail(err => _log.LogWarning("Informe no generado: {Desc}",
                                                       err.ToErrorsDescription()));
}
```

Aquí sí conviene la familia A: las tres consultas se han lanzado en paralelo a propósito,
así que su evaluación anticipada es deseada.

### Ejemplo 3: validación de un formulario complejo

```csharp
public MlResult<AltaCliente> Validar(AltaClienteDto dto)
    => dto.NullToFailed("Los datos de alta son obligatorios")
          .Bind(d => ValidarNif(d.Nif)
                        .Combine(d))                                 // (Nif, AltaClienteDto)
          .Bind(t => ValidarEmail(t.value2.Email)
                        .Map(email => (t.value1, email, t.value2)))  // (Nif, Email, Dto)
          .Bind(t => t.Item3.Direccion.NullToFailed("La dirección es obligatoria")
                        .Map(dir => (t.Item1, t.Item2, dir)))        // (Nif, Email, Direccion)
          .Map(t =>
          {
              var (nif, email, direccion) = t;
              return new AltaCliente(nif, email, direccion);
          });
```

Nota: al mezclar familias, los nombres de campo cambian (`value2` en un paso, `Item3` en el
siguiente). Es un buen recordatorio de por qué conviene deconstruir.

### Ejemplo 4: `Do` para integrar una política externa

```csharp
// Una función legada que ya trabaja con MlResult y decide su propia estrategia
private MlResult<Pedido> AplicarPoliticaLegada(MlResult<Pedido> resultado)
    => resultado.Match(
           valid: p   => p.Total > 10_000 ? MarcarParaRevision(p) : p.ToMlResultValid(),
           fail : err => err.HasKeyDetails("Transitorio")
                             ? ReintentarUnaVez()
                             : err.ToMlResultFail<Pedido>());

// Do la inserta en la tubería sin adaptadores
public MlResult<PedidoDto> Procesar(int id)
    => ObtenerPedido(id)
            .Do(AplicarPoliticaLegada)
            .Map(p => p.ToDto());
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ Esperar que Combine acumule errores de dos validaciones
var r = ValidarNif(nif).Combine(ValidarEmail(email));
//      ⚠️ El segundo operando entra como VALOR: MlResult<(Nif, MlResult<Email>)>

// ✅ Encadena y decide, o acumula explícitamente
var r = ValidarNif(nif).Bind(n => ValidarEmail(email).Map(e => (Nif: n, Email: e)));

// ❌ Consulta costosa evaluada aunque el carril ya haya fallado
var r = clienteResult.Combine(ConsultaCostosa());

// ✅ Dentro de Bind: solo se ejecuta si el carril sigue válido
var r = clienteResult.Bind(c => c.Combine(ConsultaCostosa()));

// ❌ Tuplas de 8 elementos con acceso por Item7, Item8…
var r = a.Combine((b, c, d, e, f, g, h)).Map(t => Calcular(t.Item1, t.Item2, /* … */ t.Item8));

// ✅ Define un tipo con nombres
record ContextoCalculo(Cliente Cliente, Tarifa Tarifa, /* … */);
var r = a.Bind(cliente => ConstruirContexto(cliente)).Map(Calcular);

// ❌ Usar Do para observar el resultado
var r = resultado.Do(res => { _log.LogInformation("{R}", res); return res; });

// ✅ ExecSelf está hecho para eso
var r = resultado.ExecSelf(v => _log.LogInformation("OK {V}", v),
                           e => _log.LogWarning("KO {E}", e.ToErrorsDescription()));
```

---

## Mejores Prácticas

1. **No esperes acumulación de errores.** `Combine` cortocircuita como `Bind`. Para
   acumular, usa los métodos de [`Bucle`](../Bucle/Bucles.md) o fusiona
   `MlErrorsDetails` a mano.
2. **Prefiere el patrón `Bind(t => t.Combine(...))`** cuando el valor añadido implique una
   consulta o un cálculo: así se respeta el cortocircuito.
3. **Deconstruye siempre la tupla** (`var (a, b, c) = t;`) al consumirla: evita depender de
   `Item1`/`value1` y documenta el código.
4. **Limita la tupla a 3 o 4 elementos.** A partir de ahí, un `record` propio es más
   legible y más fácil de mantener.
5. **Nunca superes los 8 elementos**: es el límite físico de las sobrecargas.
6. **Usa la familia A con valores ya resueltos** (por ejemplo, tras un `Task.WhenAll`),
   donde la evaluación anticipada es intencionada.
7. **Añade `!` en las sobrecargas asíncronas de la familia B** si trabajas con *nullable
   reference types* activados.
8. **Reserva `Do` para integrar código legado** que ya opera con `MlResult<T>`. Para
   observar usa `ExecSelf`; para recuperar, `MapIfFail`/`BindIfFail`.
9. **Nombra las variables intermedias** cuando la tubería sea larga: `MlResult<(Cliente,
   Tarifa)> conTarifa = ...` es más claro que una cadena de veinte líneas.

---

## Resumen

- `Combine` agrupa de **2 a 8 valores en una tupla** que viaja por el carril, evitando
  anidar `Bind`.
- ⚠️ **No acumula errores**: cortocircuita como `Bind`. En cada llamada solo interviene un
  `MlResult`.
- Hay **tres familias**: A (`MlResult` + valores), B (valor/tupla + `MlResult`) y
  C (valor/tupla + valor, que **nunca falla**).
- ⚠️ Los **nombres de los campos son inconsistentes**: `value1/value2…` en la familia B y en
  la A de 2 elementos; solo `Item1/Item2…` en la A de 3+ y en toda la C. **Deconstruye.**
- ⚠️ El segundo operando **se evalúa siempre**, incluso con el carril fallido: envuelve en
  `Bind` si es costoso.
- ⚠️ Algunas sobrecargas asíncronas de la familia B están anotadas como nullable sin
  devolver nunca `null`.
- **No existe `TryCombine`**: no hay delegados de usuario.
- **Apéndice `Do`**: invoca tu delegado con el `MlResult` **completo**, sin `Match` y sin
  cortocircuito. Operador de escape para integrar código externo; raramente necesario.

---

## Ver también

- [`EmptyToFailed`](1_EmptyToFailed.md) — fallar si una colección viene vacía
- [`NullToFailed`](2_NullToFailed.md) — fallar si un objeto es `null`
- [`BoolToResult`](3_BoolToResult.md) — construir un `MlResult` desde una condición
- [`Bind`](../Bind/3_Bind.md) — encadenar operaciones que pueden fallar
- [`Map`](../Map/1_Map.md) — transformar el valor del carril
- [`Bucles y proyecciones`](../Bucle/Bucles.md) — recorrer colecciones acumulando errores
- [`ExecSelf`](../ExecSelf/1_ExecSelf.md) — observar sin alterar el carril
- [`MlResultErrors`](../Types/MlResultErrors.md) — `MlErrorsDetails` y la fusión de errores
- [`Transformations`](../Transformations/Transformations.md) — `ToMlResultValid`, `ToAsync`