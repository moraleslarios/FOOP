# MapEnsure — Validar el valor sin cambiarlo

## Índice

1. [Introducción](#introducción)
2. [La idea: un peaje en la tubería](#la-idea-un-peaje-en-la-tubería)
3. [Firmas reales e implementación](#firmas-reales-e-implementación)
4. [Las cuatro formas de expresar el error](#las-cuatro-formas-de-expresar-el-error)
5. [Encadenar validaciones: cortocircuito](#encadenar-validaciones-cortocircuito)
6. [`MapEnsure` frente a `EnsureFp` y `BindIf`](#mapensure-frente-a-ensurefp-y-bindif)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [⚠️ No existe `TryMapEnsure`](#️-no-existe-trymapensure)
9. [Ejemplos Prácticos](#ejemplos-prácticos)
10. [Mejores Prácticas](#mejores-prácticas)
11. [Resumen](#resumen)
12. [Ver también](#ver-también)

---

## Introducción

`MapEnsure` es la operación de **validación en línea**:

> **Si el resultado es válido y el valor cumple el predicado, lo deja pasar tal cual. Si no lo cumple, lo convierte en fallo con el error que tú indiques.**

Es la traducción funcional del típico `if (!condición) throw ...`, pero sin excepciones y sin romper la cadena.

```csharp
// ❌ Estilo imperativo: rompe el flujo con excepciones
var pedido = ObtenerPedido(id);
if (pedido.Lineas.Count == 0) throw new InvalidOperationException("Pedido sin líneas");
if (pedido.Total <= 0)        throw new InvalidOperationException("Importe no válido");

// ✅ Estilo railway: el fallo es un valor más
MlResult<Pedido> r = ObtenerPedido(id)
                        .MapEnsure(p => p.Lineas.Count > 0, "El pedido no tiene líneas")
                        .MapEnsure(p => p.Total > 0,        "El importe del pedido no es válido");
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`.

---

## La idea: un peaje en la tubería

A diferencia de `Map`, **`MapEnsure` no transforma nada**: el tipo de entrada y el de salida son el mismo `T`. Solo decide si el valor sigue avanzando o si el carril cambia a fallo.

```
                     ┌── predicado true  ──►  MlResult<T> válido (mismo valor)
MlResult<T> válido ──┤
                     └── predicado false ──►  MlResult<T> fallido (tu error)

MlResult<T> fallido ─────────────────────►  el mismo fallo (predicado NO se evalúa)
```

Esa firma `MlResult<T> → MlResult<T>` es lo que permite apilar tantas validaciones como quieras sin cambiar de tipo.

---

## Firmas reales e implementación

El método real, del que dependen todos los demás, es el que recibe un **constructor** de `MlErrorsDetails`:

```csharp
public static MlResult<T> MapEnsure<T>(this MlResult<T>              source,
                                            Func<T, bool>            ensureFunc,
                                            Func<T, MlErrorsDetails> errorDetailsResultBuilder)
{
    var result = source.Match(
                                 valid: x      => ensureFunc(x) ? x : errorDetailsResultBuilder(x),
                                 fail : errors => errors.ToMlResultFail<T>()
                           );

    return result;
}
```

Observa cómo el operador ternario devuelve **dos tipos distintos** (`T` y `MlErrorsDetails`) y ambos acaban siendo un `MlResult<T>` gracias a las conversiones implícitas de la biblioteca. Es un buen ejemplo de lo compacto que resulta el tipo.

Las otras tres sobrecargas son azúcar sintáctico sobre esta:

```csharp
// 1) Detalles de error fijos
public static MlResult<T> MapEnsure<T>(this MlResult<T> source, Func<T, bool> ensureFunc,
                                            MlErrorsDetails errorDetailsResult)
    => source.MapEnsure(ensureFunc, _ => errorDetailsResult);

// 2) Mensaje de texto fijo — la más usada
public static MlResult<T> MapEnsure<T>(this MlResult<T> source, Func<T, bool> ensureFunc,
                                            string errorMessageResult)
    => source.MapEnsure(ensureFunc, errorMessageResult.ToMlErrorsDetails());

// 3) Mensaje construido a partir del valor
public static MlResult<T> MapEnsure<T>(this MlResult<T> source, Func<T, bool> ensureFunc,
                                            Func<T, string> errorMessageResultBuilder)
    => source.MapEnsure(ensureFunc, x => errorMessageResultBuilder(x).ToMlErrorsDetails());
```

| Estado de entrada | `ensureFunc(x)` | Resultado |
|---|---|---|
| Válido | `true` | El **mismo** valor, válido |
| Válido | `false` | Fallido con el error que hayas construido |
| Fallido | **No se evalúa** | El mismo fallo, intacto |

---

## Las cuatro formas de expresar el error

Elegir bien la sobrecarga marca la diferencia entre un mensaje inútil y un mensaje que resuelve la incidencia.

| Sobrecarga | Cuándo usarla | Ejemplo |
|---|---|---|
| `string` | Regla fija, sin datos variables | `.MapEnsure(p => p.Total > 0, "El importe debe ser positivo")` |
| `Func<T, string>` | Quieres incluir el valor real en el mensaje | `.MapEnsure(p => p.Total > 0, p => $"Importe no válido: {p.Total:C}")` |
| `MlErrorsDetails` | Error compartido y reutilizable en varios sitios | `.MapEnsure(p => p.Total > 0, ErroresPedido.ImporteNoValido)` |
| `Func<T, MlErrorsDetails>` | Necesitas adjuntar **detalles** además del mensaje | ver abajo |

La cuarta es la más potente, porque te permite dejar el valor conflictivo en los detalles para que lo recojan después `GetDetailValue<T>()` o [`BindIfFailWithValue`](../Bind/7_BindIfFailWithValue.md):

```csharp
.MapEnsure(p => p.Total > 0,
           p => MlErrorsDetails.FromErrorMessageWithValue(
                    $"El importe del pedido {p.Numero} no es válido", p))
```

Y también permite acumular varios mensajes en un único fallo:

```csharp
.MapEnsure(p => p.EsCoherente,
           p => new[] { $"El pedido {p.Numero} es incoherente",
                        $"Suma de líneas: {p.SumaLineas:C}",
                        $"Total declarado: {p.Total:C}" }.ToMlErrorsDetails())
```

---

## Encadenar validaciones: cortocircuito

Al apilar `MapEnsure`, el comportamiento es de **cortocircuito**: en cuanto una falla, las siguientes ya no evalúan su predicado.

```csharp
var r = solicitud.ToMlResultValid()
            .MapEnsure(s => s.Nif is not null,        "Falta el NIF")
            .MapEnsure(s => s.Nif!.Length == 9,       "El NIF debe tener 9 caracteres")   // seguro: el anterior ya garantizó no-null
            .MapEnsure(s => char.IsDigit(s.Nif![0]),  "El NIF debe empezar por dígito");
```

Ese orden no es casual: **las validaciones estructurales van primero**, y las que dependen de ellas después. Es lo que permite el `!` de la segunda línea sin riesgo.

> 🔎 **¿Y si quiero todos los errores a la vez?** El cortocircuito devuelve solo el primero. Para acumular todos los mensajes en un único fallo, usa [`BindMulti`](../Bind/4_BindMulti.md), que ejecuta todas las validaciones y fusiona sus errores.

```csharp
// Cortocircuito: devuelve el PRIMER error
solicitud.ToMlResultValid()
         .MapEnsure(s => s.Nif    is not null, "Falta el NIF")
         .MapEnsure(s => s.Nombre is not null, "Falta el nombre");

// Acumulación: devuelve TODOS los errores
solicitud.ToMlResultValid()
         .BindMulti(s => s.Nif    is not null ? s.ToMlResultValid() : "Falta el NIF".ToMlResultFail<Solicitud>(),
                    s => s.Nombre is not null ? s.ToMlResultValid() : "Falta el nombre".ToMlResultFail<Solicitud>())
         (s => s.ToMlResultValid());
```

---

## `MapEnsure` frente a `EnsureFp` y `BindIf`

Tres herramientas parecidas con propósitos distintos:

| Herramienta | Punto de partida | Qué hace |
|---|---|---|
[`EnsureFp.That`](../EnsureFp/EnsureFp.md) | Un valor **desnudo** | **Entra** en el mundo `MlResult` validando |
| `MapEnsure` | Un `MlResult<T>` | Valida **dentro** de la tubería, sin cambiar el tipo |
| [`BindIf`](../Bind/5_BindIf.md) | Un `MlResult<T>` | **Bifurca** hacia una u otra operación según la condición |

```csharp
// EnsureFp: el punto de entrada de la tubería
EnsureFp.NotNullEmptyOrWhitespace(nif, "El NIF es obligatorio")   // string → MlResult<string>

    // MapEnsure: validaciones sucesivas dentro de la tubería
    .MapEnsure(n => n.Length == 9, n => $"NIF con longitud incorrecta: {n.Length}")

    // BindIf: elegir camino, no validar
    .BindIf(n => n.StartsWith('X'),
            n => BuscarExtranjero(n),
            n => BuscarNacional(n));
```

> 📌 La distinción práctica: **`MapEnsure` responde «¿es válido?»; `BindIf` responde «¿por dónde sigo?»**.

---

## Variantes asíncronas

`MapEnsureAsync` tiene 8 sobrecargas: las 4 formas de error × 2 tipos de origen.

| Origen | Formas de error disponibles |
|---|---|
| `MlResult<T>` | `MlErrorsDetails`, `Func<T, MlErrorsDetails>`, `string`, `Func<T, string>` |
| `Task<MlResult<T>>` | `MlErrorsDetails`, `Func<T, MlErrorsDetails>`, `string`, `Func<T, string>` |

```csharp
public static async Task<MlResult<T>> MapEnsureAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                             Func<T, bool>     ensureFunc,
                                                             string            errorMessageResult)
    => await (await sourceAsync).MapEnsureAsync(ensureFunc, errorMessageResult);
```

> ⚠️ **El predicado es siempre síncrono.** No hay ninguna sobrecarga con `Func<T, Task<bool>>`. Si tu comprobación necesita ir a la base de datos o a un servicio, `MapEnsure` no es la herramienta: usa [`BindAsync`](../Bind/3_Bind.md) devolviendo el valor o el fallo.

```csharp
// ❌ No existe: MapEnsureAsync con predicado asíncrono
.MapEnsureAsync(async n => await _repo.ExisteAsync(n), "No existe")

// ✅ Con Bind, que sí admite operaciones asíncronas
.BindAsync(async n => await _repo.ExisteAsync(n)
                          ? n.ToMlResultValid()
                          : $"El NIF {n} no está registrado".ToMlResultFail<string>());
```

---

## ⚠️ No existe `TryMapEnsure`

En la clase real **no hay variantes `Try*` de `MapEnsure`**. La razón de diseño es clara: un predicado de validación debe ser una expresión booleana pura y no debe lanzar. Si tu predicado puede lanzar, el problema está en el predicado.

```csharp
// ❌ Predicado que puede lanzar: la excepción escapa de la tubería
.MapEnsure(p => int.Parse(p.Codigo) > 100, "Código demasiado bajo")

// ✅ Convierte primero con TryMap, valida después
.TryMap(p => p with { CodigoNumerico = int.Parse(p.Codigo) },
        ex => $"El código no es numérico: {ex.Message}")
.MapEnsure(p => p.CodigoNumerico > 100, "Código demasiado bajo")
```

> 💡 En el código fuente hay además un método `MapEquals` **comentado** (nunca se publicó). No lo busques: no forma parte de la API.

---

## Ejemplos Prácticos

### Ejemplo 1: Validación completa de un alta de cliente

```csharp
public MlResult<Cliente> ValidarAlta(AltaClienteDto dto)
    => EnsureFp.NotNull(dto, "La solicitud de alta es obligatoria")

        // Estructurales primero
        .MapEnsure(d => !string.IsNullOrWhiteSpace(d.RazonSocial),
                        "La razón social es obligatoria")
        .MapEnsure(d => !string.IsNullOrWhiteSpace(d.Nif),
                        "El NIF es obligatorio")

        // Formato después, ya con la garantía de no-null
        .MapEnsure(d => d.RazonSocial!.Length <= 120,
                   d => $"La razón social no puede superar 120 caracteres (tiene {d.RazonSocial!.Length})")
        .MapEnsure(d => d.Nif!.Length == 9,
                   d => $"El NIF '{d.Nif}' no tiene 9 caracteres")

        // Reglas de negocio al final, con detalles enriquecidos
        .MapEnsure(d => d.LimiteCredito >= 0,
                   d => MlErrorsDetails.FromErrorMessageWithValue(
                            $"El límite de crédito no puede ser negativo ({d.LimiteCredito:C})", d))
        .MapEnsure(d => d.Email is null || d.Email.Contains('@'),
                   d => $"El correo '{d.Email}' no tiene un formato válido")

        // Y solo entonces se proyecta
        .Map(d => new Cliente(d.RazonSocial!, d.Nif!, d.LimiteCredito, d.Email));
```

### Ejemplo 2: Validar el resultado de una operación externa

`MapEnsure` es igual de útil **después** de una llamada, para comprobar que lo que has recibido es usable.

```csharp
public async Task<MlResult<Cotizacion>> ObtenerCotizacionAsync(string divisa)
    => await _mercado.ConsultarAsync(divisa)                                  // Task<MlResult<Cotizacion>>

        // El servicio respondió, pero ¿la respuesta sirve?
        .MapEnsureAsync(c => c.Valor > 0,
                        c => $"Cotización no válida para {divisa}: {c.Valor}")

        .MapEnsureAsync(c => c.Fecha >= DateTime.UtcNow.AddMinutes(-15),
                        c => $"Cotización obsoleta para {divisa} (fecha {c.Fecha:HH:mm:ss} UTC)")

        .MapEnsureAsync(c => c.Divisa.Equals(divisa, StringComparison.OrdinalIgnoreCase),
                        c => $"El mercado devolvió la divisa {c.Divisa} en vez de {divisa}")

        .AddMlErrorDetailIfFailAsync("[Mercado] La cotización recibida no es utilizable");
```

Este uso —validar la **salida**, no solo la entrada— es una de las mejores defensas contra integraciones poco fiables.

### Ejemplo 3: Errores reutilizables en un catálogo

Cuando las mismas reglas se validan en varios sitios, centraliza los errores con la sobrecarga de `MlErrorsDetails`.

```csharp
public static class ErroresFactura
{
    public static readonly MlErrorsDetails SinLineas =
        MlErrorsDetails.FromErrorMessage("La factura debe tener al menos una línea");

    public static readonly MlErrorsDetails YaContabilizada =
        MlErrorsDetails.FromErrorMessage("La factura ya está contabilizada y no admite cambios");

    public static MlErrorsDetails DescuadreDe(Factura f)
        => new[] { $"La factura {f.Numero} está descuadrada",
                   $"Base + IVA: {f.Base + f.Iva:C}",
                   $"Total declarado: {f.Total:C}" }.ToMlErrorsDetails();
}

public MlResult<Factura> ValidarParaContabilizar(Factura f)
    => f.ToMlResultValid()
        .MapEnsure(x => x.Lineas.Any(),                    ErroresFactura.SinLineas)
        .MapEnsure(x => !x.Contabilizada,                  ErroresFactura.YaContabilizada)
        .MapEnsure(x => x.Base + x.Iva == x.Total,         ErroresFactura.DescuadreDe);
```

### Ejemplo 4: En un controlador, traduciendo el fallo a HTTP

```csharp
[HttpPost("pedidos/{id:int}/confirmar")]
public async Task<IActionResult> ConfirmarAsync(int id)
    => await EnsureFp.That(id, id > 0, "El identificador debe ser positivo")

        .BindAsync(pedidoId => _repo.ObtenerAsync(pedidoId))

        .MapEnsureAsync(p => p.Estado == EstadoPedido.Borrador,
                        p => $"Solo se confirman pedidos en borrador (estado actual: {p.Estado})")
        .MapEnsureAsync(p => p.Lineas.Any(),
                             "No se puede confirmar un pedido sin líneas")
        .MapEnsureAsync(p => p.Lineas.All(l => l.Cantidad > 0),
                        p => $"El pedido tiene {p.Lineas.Count(l => l.Cantidad <= 0)} línea(s) con cantidad no válida")

        .BindAsync(p => _servicio.ConfirmarAsync(p))

        .MatchAsync(
            valid: confirmado => Ok(new { confirmado.Numero, confirmado.FechaConfirmacion }),
            fail : errores    => errores.GetDetailException()
                                        .Match(
                                            valid: _ => StatusCode(500, new { error = "Error interno" }),
                                            fail : _ => BadRequest(new { errores = errores.ToErrorsMessages() })));
```

---

## Mejores Prácticas

1. **Un `MapEnsure` por regla.** Es la clave para que el mensaje de error identifique exactamente qué ha fallado. Evita predicados con `&&` encadenados.

```csharp
// ❌ Un solo error para tres reglas distintas
.MapEnsure(p => p.Total > 0 && p.Lineas.Any() && p.Cliente is not null, "Pedido no válido")

// ✅ Cada regla con su mensaje
.MapEnsure(p => p.Total > 0,        "El importe debe ser positivo")
.MapEnsure(p => p.Lineas.Any(),     "El pedido no tiene líneas")
.MapEnsure(p => p.Cliente is not null, "El pedido no tiene cliente")
```

2. **Ordena de lo estructural a lo semántico.** Comprueba primero que existe, después su formato y por último las reglas de negocio. El cortocircuito hace que las últimas puedan asumir lo anterior.

3. **Usa `Func<T, string>` cuando el dato ayude a diagnosticar.** «El NIF no es válido» dice mucho menos que «El NIF 'A1234' tiene 5 caracteres, se esperaban 9».

4. **Mantén el predicado puro, síncrono y sin excepciones.** No hay `TryMapEnsure` ni predicados asíncronos por diseño.

5. **Valida también las respuestas ajenas.** `MapEnsure` después de una llamada externa te protege de datos incoherentes.

6. **Si necesitas todos los errores, no uses `MapEnsure`.** Cambia a [`BindMulti`](../Bind/4_BindMulti.md), que acumula.

7. **Centraliza los errores repetidos** en un catálogo estático con la sobrecarga de `MlErrorsDetails`.

8. **No confundas validar con bifurcar.** Si el «fallo» es en realidad un camino alternativo legítimo, lo que quieres es [`BindIf`](../Bind/5_BindIf.md).

---

## Resumen

- `MapEnsure` valida el valor **sin transformarlo**: la firma es `MlResult<T> → MlResult<T>`.
- Si el predicado se cumple, el valor pasa intacto; si no, se convierte en el fallo que tú construyas.
- Si el resultado **ya venía fallido**, el predicado **no se evalúa**.
- Hay **4 sobrecargas síncronas** (`MlErrorsDetails`, `Func<T, MlErrorsDetails>`, `string`, `Func<T, string>`) y **8 asíncronas** (las 4 × 2 tipos de origen).
- El método base es el que recibe `Func<T, MlErrorsDetails>`; los demás delegan en él.
- El encadenamiento es de **cortocircuito**: devuelve el primer error. Para acumular, usa [`BindMulti`](../Bind/4_BindMulti.md).
- **No existe `TryMapEnsure`** ni predicados asíncronos: el predicado debe ser puro y síncrono.
- En el fuente hay un `MapEquals` comentado que **no forma parte de la API**.

---

## Ver también

- [`1_Map.md`](1_Map.md) — transformar el valor.
- [`3_MapIf.md`](3_MapIf.md) — transformar solo si se cumple una condición.
- [`4_MapIfFail.md`](4_MapIfFail.md) — recuperarse de un fallo.
- [`../Bind/5_BindIf.md`](../Bind/5_BindIf.md) — bifurcar según una condición.
- [`../Bind/4_BindMulti.md`](../Bind/4_BindMulti.md) — acumular todos los errores de validación.
- [`../EnsureFp/EnsureFp.md`](../EnsureFp/EnsureFp.md) — entrar en el mundo `MlResult` validando.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — construir `MlErrorsDetails` con detalles.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la clase.