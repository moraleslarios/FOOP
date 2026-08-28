# BindIf — Bifurcar el flujo según una condición

## Índice
1. [Introducción](#introducción)
2. [Las dos formas de `BindIf`](#las-dos-formas-de-bindif)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [`TryBindIf` — cuando la rama puede lanzar](#trybindif--cuando-la-rama-puede-lanzar)
6. [`BindIf` frente a otras alternativas](#bindif-frente-a-otras-alternativas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

En medio de una tubería suele aparecer una bifurcación: *si el cliente es VIP aplica una tarifa, si no,
otra*. Escribirlo con `Bind` y un `if` dentro rompe la lectura de la cadena:

```csharp
// ❌ El if dentro del lambda ensucia la tubería.
var resultado = ObtenerCliente(id)
    .Bind(c =>
    {
        if (c.EsVip) return AplicarTarifaVip(c);
        return AplicarTarifaEstandar(c);
    })
    .Bind(c => Facturar(c));

// ✅ BindIf expresa la bifurcación como un eslabón más.
var resultado = ObtenerCliente(id)
    .BindIf(c => c.EsVip,
            funcTrue : c => AplicarTarifaVip(c),
            funcFalse: c => AplicarTarifaEstandar(c))
    .Bind(c => Facturar(c));
```

`BindIf` **no evalúa la condición si el resultado ya venía fallido**: como todos los `Bind`, el fallo
cortocircuita y se propaga intacto.

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## Las dos formas de `BindIf`

La región `BindIf` del código fuente contiene **dos firmas distintas** que se llaman igual. Distinguirlas
es la clave para usar la familia bien.

| Forma | Firma | Qué hace si la condición es `false` |
| --- | --- | --- |
| **A — Bifurcación completa** | `BindIf<T, TReturn>(condition, funcTrue, funcFalse)` | Ejecuta `funcFalse` |
| **B — Paso opcional** | `BindIf<T>(condition, func)` | **No hace nada**: devuelve el valor tal cual |

La forma **B** solo existe cuando el tipo de entrada y de salida coinciden (`MlResult<T>` → `MlResult<T>`),
porque necesita poder devolver el valor original sin transformarlo.

```csharp
// Forma A: dos caminos, ambos obligatorios.
MlResult<Factura> f = pedido.BindIf(p => p.EsInternacional,
                                    p => FacturarConAduana(p),
                                    p => FacturarNacional(p));

// Forma B: un paso que solo se aplica a veces.
MlResult<Pedido> p2 = pedido.BindIf(p => p.Importe > 1000,
                                    p => RequerirAutorizacion(p));
// Si el importe es <= 1000, p2 es exactamente pedido, sin tocar nada.
```

---

## Firmas reales

### Forma A

```csharp
public static MlResult<TReturn> BindIf<T, TReturn>(this MlResult<T>                source,
                                                        Func<T, bool>              condition,
                                                        Func<T, MlResult<TReturn>> funcTrue,
                                                        Func<T, MlResult<TReturn>> funcFalse)
    => source.Match(
        valid: x => condition(x) ? funcTrue(x) : funcFalse(x),
        fail :      MlResult<TReturn>.Fail);
```

### Forma B

```csharp
public static MlResult<T> BindIf<T>(this MlResult<T>          source,
                                         Func<T, bool>        condition,
                                         Func<T, MlResult<T>> func)
    => source.Match(
        valid: x => condition(x) ? func(x) : x,          // ← 'x' se convierte implícitamente en MlResult<T>
        fail :      MlResult<T>.Fail);
```

Fíjate en el `: x` de la forma B: aprovecha la **conversión implícita** de `T` a `MlResult<T>` para
devolver el valor sin envolverlo a mano.

| Estado de entrada | `condition` | Forma A | Forma B |
| --- | --- | --- | --- |
| Fallido | **no se evalúa** | Propaga el fallo | Propaga el fallo |
| Válido | `true` | `funcTrue(x)` | `func(x)` |
| Válido | `false` | `funcFalse(x)` | `x` sin cambios |

📌 La condición es un `Func<T, bool>` **puro y síncrono**: no puede fallar ni devolver `MlResult`. Si tu
condición necesita consultar un servicio o puede fallar, resuélvela antes con `Bind`/`Map` y guárdala en
el propio valor (o en una tupla).

---

## Variantes asíncronas

`BindIfAsync` cubre **11 sobrecargas** combinando estos ejes:

| Eje | Opciones |
| --- | --- |
| Fuente | `MlResult<T>` · `Task<MlResult<T>>` |
| `funcTrue` | síncrona · asíncrona |
| `funcFalse` | síncrona · asíncrona |

```csharp
// Fuente asíncrona + ambas ramas asíncronas.
public Task<MlResult<Envio>> PrepararAsync(Pedido pedido)
    => ObtenerPedidoAsync(pedido.Id)
        .BindIfAsync(p => p.EsUrgente,
                     funcTrueAsync : p => ReservarMensajeriaExpressAsync(p),
                     funcFalseAsync: p => ReservarMensajeriaEstandarAsync(p));
```

💡 En cuanto un solo paso sea asíncrono, usa `BindIfAsync`/`BindAsync` en **todos** los eslabones
posteriores; mezclar `.Result` o `await` intermedios dentro de los lambdas rompe la composición.

---

## `TryBindIf` — cuando la rama puede lanzar

Si `funcTrue` o `funcFalse` invocan código que puede lanzar excepciones (I/O, deserialización, un ORM),
usa `TryBindIf`: envuelve la ejecución y convierte la excepción en un fallo, guardándola en
`Details["Ex"]`.

```csharp
public static MlResult<TReturn> TryBindIf<T, TReturn>(this MlResult<T>                source,
                                                           Func<T, bool>              condition,
                                                           Func<T, MlResult<TReturn>> funcTrue,
                                                           Func<T, MlResult<TReturn>> funcFalse,
                                                           Func<Exception, string>    errorMessageBuilder)
    => source.Match(
        valid: x => condition(x)
                        ? funcTrue .TryToMlResult(x, errorMessageBuilder)
                        : funcFalse.TryToMlResult(x, errorMessageBuilder),
        fail :      MlResult<TReturn>.Fail);
```

Existe también la sobrecarga con un `string exceptionAditionalMessage` en lugar del constructor de
mensaje. En total: **3 `TryBindIf` síncronos** y **24 `TryBindIfAsync`**.

```csharp
var resultado = documento
    .TryBindIf(d => d.Formato == Formato.Xml,
               d => ParsearXml(d.Contenido),          // puede lanzar XmlException
               d => ParsearJson(d.Contenido),         // puede lanzar JsonException
               ex => $"No se pudo parsear el documento {documento.Id}: {ex.Message}");

// Recuperar la excepción original más adelante:
resultado.ExecSelfIfFail(errores =>
    errores.GetDetailException()
           .ExecSelfIfValid(ex => _log.LogError(ex, "Fallo de parseo")));
```

| Método | Excepción en la rama | Cuándo usarlo |
| --- | --- | --- |
| `BindIf` | **Se propaga** y rompe la tubería | Las ramas son código propio validado |
| `TryBindIf` | Se convierte en `MlResult` fallido | Las ramas hacen I/O o usan librerías de terceros |

---

## `BindIf` frente a otras alternativas

| Necesidad | Herramienta |
| --- | --- |
| Dos caminos, cada uno produce el resultado | **`BindIf`** (forma A) |
| Un paso extra que solo se aplica a veces | **`BindIf`** (forma B) |
| Bifurcar pero las ramas devuelven un valor plano, no `MlResult` | [`MapIf`](../Map/3_MapIf.md) |
| Convertir una condición en éxito/fallo | [`MapEnsure`](../Map/2_MapEnsure.md) o `EnsureFp.That` |
| Ejecutar varias comprobaciones y acumular errores | [`BindMulti`](./4_BindMulti.md) |
| Reaccionar según el estado (válido/fallido), no según el valor | [`Match`](../Match/1_Match.md) |
| Solo un efecto secundario condicional, sin cambiar el valor | [`ExecSelfIf`](../ExecSelf/1_ExecSelf.md) |

🔑 `BindIf` bifurca por el **contenido** del valor. `Match` bifurca por el **estado** del resultado. No
son intercambiables.

---

## Ejemplos Prácticos

### Ejemplo 1: Tarifa según el tipo de cliente (forma A)

```csharp
public class ServicioFacturacion
{
    public MlResult<Factura> Emitir(int clienteId, Carrito carrito)
        => ObtenerCliente(clienteId)
            .Bind(c => ValidarCarrito(carrito).Map(_ => c))

            .BindIf(c => c.EsVip,
                    funcTrue : c => CalcularConDescuentoVip(c, carrito),
                    funcFalse: c => CalcularTarifaEstandar(c, carrito))

            .BindIf(f => f.Total > 3000m,
                    f => RequerirAprobacionFinanciera(f))     // Forma B: paso opcional

            .AddMlErrorDetailIfFail($"[Facturación] Cliente {clienteId}");

    private MlResult<Factura> CalcularConDescuentoVip(Cliente c, Carrito carrito)
        => _tarifas.ObtenerVip(c.Segmento)
            .Map(tarifa => Factura.Crear(c, carrito, tarifa, descuento: 0.15m));

    private MlResult<Factura> CalcularTarifaEstandar(Cliente c, Carrito carrito)
        => _tarifas.ObtenerEstandar()
            .Map(tarifa => Factura.Crear(c, carrito, tarifa, descuento: 0m));
}
```

Observa la combinación: primero una bifurcación real (forma A), después un paso que **solo** se aplica a
importes altos (forma B). La tubería sigue leyéndose de arriba abajo sin un solo `if`.

### Ejemplo 2: Paso opcional idempotente (forma B)

La forma B brilla cuando el paso es *enriquecer si hace falta*:

```csharp
public MlResult<Pedido> Normalizar(Pedido pedido)
    => pedido.ToMlResultValid()
        // Solo si falta la dirección de facturación, la copiamos de la de envío.
        .BindIf(p => p.DireccionFacturacion is null,
                p => p with { DireccionFacturacion = p.DireccionEnvio })

        // Solo si no tiene divisa, aplicamos la del país.
        .BindIf(p => string.IsNullOrWhiteSpace(p.Divisa),
                p => ObtenerDivisaDelPais(p.DireccionEnvio.Pais)
                        .Map(divisa => p with { Divisa = divisa }))

        // Solo si el pedido viene de la web, calculamos gastos de envío.
        .BindIf(p => p.Canal == Canal.Web,
                p => CalcularGastosEnvio(p));
```

Tres pasos condicionales encadenados, todos opcionales, sin ramas anidadas. Fíjate en que el segundo
`BindIf` **puede fallar** (`ObtenerDivisaDelPais`): si lo hace, los dos siguientes ya no se ejecutan.

### Ejemplo 3: Bifurcación asíncrona con lectura de datos externa

```csharp
public async Task<MlResult<Liquidacion>> LiquidarAsync(int contratoId)
    => await ObtenerContratoAsync(contratoId)

        // La condición debe ser síncrona: primero traemos el dato que necesitamos
        // y lo llevamos en una tupla.
        .BindAsync(c => ObtenerSaldoAsync(c.CuentaId).Map(s => (Contrato: c, Saldo: s)))

        .BindIfAsync(x => x.Saldo >= x.Contrato.ImportePendiente,
                     funcTrueAsync : x => LiquidarTotalAsync(x.Contrato),
                     funcFalseAsync: x => GenerarPlanDePagosAsync(x.Contrato, x.Saldo))

        .ExecSelfIfFailAsync(errores =>
        {
            _log.LogWarning("Liquidación {Id} fallida: {Detalle}",
                            contratoId, errores.ToErrorsDescription());
            return Task.CompletedTask;
        });
```

**Patrón clave:** cuando la condición depende de un dato que hay que consultar, tráelo primero con
`BindAsync` y pásalo en una tupla. Así `condition` sigue siendo un `Func<T, bool>` puro.

### Ejemplo 4: Elegir el parser según el formato (con `TryBindIf`)

```csharp
public MlResult<IReadOnlyList<Movimiento>> ImportarExtracto(Fichero fichero)
    => EnsureFp.NotNull(fichero, "El fichero es obligatorio")
        .Bind(f => EnsureFp.That(f, f.Bytes.Length > 0, "El fichero está vacío"))

        .TryBindIf(f => f.Nombre.EndsWith(".csv", StringComparison.OrdinalIgnoreCase),
                   funcTrue : f => ParsearCsv(f.Bytes),        // CsvHelper puede lanzar
                   funcFalse: f => ParsearNorma43(f.Bytes),    // El parser propio puede lanzar
                   ex => $"Error al leer '{fichero.Nombre}': {ex.Message}")

        .Bind(movs => EnsureFp.NotEmpty(movs, "El extracto no contiene movimientos"))
        .Map(movs => (IReadOnlyList<Movimiento>)movs.ToList());
```

Y en el controlador se distingue el fallo técnico del de negocio:

```csharp
[HttpPost("extractos")]
public IActionResult Importar(IFormFile fichero)
    => _servicio.ImportarExtracto(Fichero.Desde(fichero))
        .Match<IReadOnlyList<Movimiento>, IActionResult>(
            valid: movs    => Ok(new { importados = movs.Count }),
            fail : errores => errores.GetDetailException()
                                     .Match(valid: _ => StatusCode(500, errores.ToErrorsMessages()),
                                            fail : _ => BadRequest(errores.ToErrorsMessages())));
```

---

## Mejores Prácticas

### 1. Mantén la condición pura y baratísima

`condition` se ejecuta dentro de la tubería y no puede fallar. Nada de I/O, nada de excepciones. Si
necesitas un dato remoto, tráelo antes con `Bind`/`Map` y llévalo en una tupla (ver ejemplo 3).

### 2. Elige la forma correcta

Si la rama `false` no tiene nada que hacer, **no escribas `funcFalse: c => c`**: usa la forma B de un
solo delegado. Es más corta y comunica mejor la intención de «paso opcional».

```csharp
// ❌ Ruido innecesario.
.BindIf(p => p.NecesitaRevision, p => Revisar(p), p => MlResult<Pedido>.Valid(p))

// ✅ Forma B.
.BindIf(p => p.NecesitaRevision, p => Revisar(p))
```

### 3. Nombra los parámetros en bifurcaciones largas

Con dos lambdas seguidas es fácil invertirlas por accidente. Usar `funcTrue:` y `funcFalse:` de forma
explícita convierte un error silencioso en código autoexplicativo.

### 4. No anides `BindIf` dentro de `BindIf`

Dos niveles ya son ilegibles. Si tienes más de dos caminos, extrae un método `Selector` que devuelva la
estrategia, o usa una expresión `switch` sobre un enumerado y aplica `Bind` una sola vez.

### 5. `TryBindIf` solo donde de verdad hay riesgo

Envolver todo en `Try*` oculta bugs propios convirtiéndolos en fallos «de negocio». Resérvalo para
llamadas a librerías externas, ficheros, red y bases de datos.

---

## Resumen

- `BindIf` bifurca la tubería según una condición **sobre el valor**, sin romper la cadena.
- Tiene **dos formas**: bifurcación completa (`funcTrue` + `funcFalse`, con cambio de tipo permitido) y
  paso opcional (`func` único, mismo tipo, devuelve el valor intacto si la condición es `false`).
- Si el resultado ya venía fallido, **la condición no se evalúa** y el fallo se propaga.
- `condition` es un `Func<T, bool>` puro y síncrono; los datos externos se traen antes.
- Sobrecargas: `BindIf` (2), `BindIfAsync` (11), `TryBindIf` (3), `TryBindIfAsync` (24).
- `TryBindIf` captura las excepciones de las ramas y las guarda en `Details["Ex"]`, recuperables con
  `GetDetailException()`.

## Ver también

- [`3_Bind.md`](./3_Bind.md) — el encadenamiento base sin condición.
- [`4_BindMulti.md`](./4_BindMulti.md) — ejecutar varias comprobaciones y acumular sus errores.
- [`6_BindIfFail.md`](./6_BindIfFail.md) — la bifurcación complementaria: actuar sobre el camino fallido.
- [`../Map/3_MapIf.md`](../Map/3_MapIf.md) — la versión para ramas que devuelven un valor plano.
- [`../Map/2_MapEnsure.md`](../Map/2_MapEnsure.md) — convertir una condición en éxito o fallo.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — `ExecSelfIf`, para efectos condicionales.
- [`../Match/1_Match.md`](../Match/1_Match.md) — bifurcar por estado en lugar de por valor.
- [`../EnsureFp/EnsureFp.md`](../EnsureFp/EnsureFp.md) — construir las guardas previas.
- [`../Types/MlResultActionsBind.md`](../Types/MlResultActionsBind.md) — referencia con todas las sobrecargas.