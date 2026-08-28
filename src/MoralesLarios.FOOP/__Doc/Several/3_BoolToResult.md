# BoolToResult — Convertir una condición en un resultado

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [Las dos familias: `BoolToResult<T>` y `BoolToResult`](#las-dos-familias-booltoresultt-y-booltoresult)
4. [Familia 1: `BoolToResult<T>` — validar un valor con una condición](#familia-1-booltoresultt--validar-un-valor-con-una-condición)
5. [Familia 2: `BoolToResult` — el `bool` como sujeto](#familia-2-booltoresult--el-bool-como-sujeto)
6. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
7. [Las cuatro formas de expresar el error](#las-cuatro-formas-de-expresar-el-error)
8. [Variantes asíncronas](#variantes-asíncronas)
9. [`BoolToResult` frente a `EnsureFp.That` y `MapEnsure`](#booltoresult-frente-a-ensurefpthat-y-mapensure)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`BoolToResult` es la puerta de entrada al carril más general de la librería: convierte
**cualquier condición booleana** en un `MlResult`. Si la condición es `true`, el resultado
es válido; si es `false`, el resultado falla con el error que indiques.

```csharp
// ❌ Estilo imperativo: guardas dispersas, mensajes que se pierden
if (pedido.Estado != EstadoPedido.Borrador)
    return BadRequest("Solo se pueden modificar pedidos en borrador");
if (pedido.Lineas.Count > 200)
    return BadRequest("Un pedido no puede tener más de 200 líneas");

// ✅ Con BoolToResult: cada guarda es un eslabón del carril
return pedido.BoolToResult(pedido.Estado == EstadoPedido.Borrador,
                           "Solo se pueden modificar pedidos en borrador")
             .MapEnsure(p => p.Lineas.Count <= 200,
                        "Un pedido no puede tener más de 200 líneas")
             .Match(valid: p   => Ok(p.ToDto()),
                    fail : err => BadRequest(err.ToErrorsMessages()));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema que resuelve

`EmptyToFailed` cubre "la colección está vacía" y `NullToFailed` cubre "el objeto es
`null`". Pero la mayoría de las reglas de negocio no encajan en ninguno de los dos moldes:
*"el pedido debe estar en borrador"*, *"el usuario debe tener permiso"*, *"la fecha no
puede ser futura"*, *"el saldo debe cubrir el importe"*.

`BoolToResult` es la generalización: **tú expresas la condición, la librería construye el
`MlResult`**.

| Condición | Salida |
|-----------|--------|
| `true` | `MlResult` **Valid** con el valor original |
| `false` | `MlResult` **Fail** con tu error |

---

## Las dos familias: `BoolToResult<T>` y `BoolToResult`

Ambas viven en la misma región del código fuente, pero se usan de forma distinta:

| Familia | Firma | Qué lleva el resultado válido | Cuándo usarla |
|---------|-------|-------------------------------|---------------|
| **1** | `BoolToResult<T>(this T source, bool condition, error)` | El **valor** `source` | Quieres validar un objeto y seguir trabajando con él |
| **2** | `BoolToResult(this bool source, error)` | El propio `true` (`MlResult<bool>`) | La condición **es** el sujeto: una comprobación aislada |

```csharp
// Familia 1: el pedido sigue viajando por el carril
MlResult<Pedido> r1 = pedido.BoolToResult(pedido.EsEditable, "El pedido no es editable");
r1.Map(p => p.Total);          // ✅ tengo el pedido

// Familia 2: solo me interesa saber si la comprobación pasó
MlResult<bool> r2 = usuario.TienePermiso("Pedidos.Editar")
                           .BoolToResult("No tiene permiso para editar pedidos");
r2.Bind(_ => EditarPedido(pedido));   // el valor es siempre 'true', no aporta información
```

💡 **Regla práctica:** usa la familia 1 casi siempre. La familia 2 solo cuando la
comprobación no tenga un "sujeto" natural que quieras conservar (por ejemplo, un chequeo
de permisos o de configuración global).

---

## Familia 1: `BoolToResult<T>` — validar un valor con una condición

```csharp
// BASE
public static MlResult<T> BoolToResult<T>(this T               source,
                                                bool            condition,
                                                MlErrorsDetails errorsDetails)
    => condition ? source.ToMlResultValid() : errorsDetails.ToMlResultFail<T>();

public static MlResult<T> BoolToResult<T>(this T source, bool condition, MlError error)
    => source BoolToResult(condition, MlErrorsDetails.FromError(error));

public static MlResult<T> BoolToResult<T>(this T source, bool condition, string errorMessage)
    => source BoolToResult(condition, MlError.FromErrorMessage(errorMessage));

public static MlResult<T> BoolToResult<T>(this T                   source,
                                                bool                condition,
                                                IEnumerable<string> errorsMessage)
    => source BoolToResult(condition, MlErrorsDetails.FromEnumerableStrings(errorsMessage));
```

Puntos importantes:

| Detalle | Consecuencia práctica |
|---------|----------------------|
| `condition` es un **`bool`**, no un `Func<T,bool>` | Se evalúa **antes** de la llamada: no hay evaluación diferida ni cortocircuito |
| El valor válido es `source` tal cual | No se transforma ni se copia |
| No comprueba `null` | Un `source` `null` con `condition == true` produce un `MlResult` válido que contiene `null` |
| Todas las sobrecargas delegan en la de `MlErrorsDetails` | Comportamiento uniforme |
| No existe `TryBoolToResult` | No hay delegado de usuario que pueda lanzar |

---

## Familia 2: `BoolToResult` — el `bool` como sujeto

```csharp
// BASE
public static MlResult<bool> BoolToResult(this bool            source,
                                               MlErrorsDetails errorsDetails)
    => source ? source.ToMlResultValid() : errorsDetails.ToMlResultFail<bool>();

public static MlResult<bool> BoolToResult(this bool source, MlError error)
    => source BoolToResult(MlErrorsDetails.FromError(error));

public static MlResult<bool> BoolToResult(this bool source, string errorMessage)
    => source BoolToResult(MlError.FromErrorMessage(errorMessage));

public static MlResult<bool> BoolToResult(this bool                source,
                                               IEnumerable<string> errorsMessage)
    => source BoolToResult(MlErrorsDetails.FromEnumerableStrings(errorsMessage));
```

Aquí el `bool` cumple **dos papeles a la vez**: es la condición y es el valor. Por eso el
resultado válido siempre contiene `true` — nunca `false`, porque en ese caso el resultado
sería `Fail`.

```csharp
// Uso natural: comprobaciones de guarda al principio de un proceso
public MlResult<Recibo> Emitir(Factura factura, Usuario usuario)
    => usuario.TieneRol("Facturacion").BoolToResult("Se requiere el rol 'Facturacion'")
              .Bind(_ => _config.EmisionHabilitada.BoolToResult("La emisión está deshabilitada"))
              .Bind(_ => factura.BoolToResult(factura.EstaCerrada, "La factura debe estar cerrada"))
              .Map(f => new Recibo(f));
```

🔑 Fíjate en el patrón: la familia 2 se consume con `Bind(_ => ...)`, descartando el valor
`true` porque no aporta nada.

---

## ⚠️ Particularidades reales del código fuente

**1. La condición **no** es diferida: se evalúa siempre.**
Al ser un `bool` y no un `Func<T,bool>`, el argumento se calcula antes de entrar al método,
así que **el cortocircuito del carril no existe** para la condición:

```csharp
// ⚠️ ConsultaCostosa() se ejecuta AUNQUE el resultado anterior ya haya fallado
var r = resultadoPrevio.Bind(x => x.BoolToResult(ConsultaCostosa(x), "..."));
//      ↑ aquí sí hay cortocircuito porque Bind no invoca el lambda si hay fallo

// ⚠️ Pero en una llamada suelta, la condición se evalúa siempre:
var r = valor.BoolToResult(ConsultaCostosa(valor), "...");

// ✅ Si la condición es costosa y ya estás en el carril, usa MapEnsure (predicado diferido)
var r = resultadoPrevio.MapEnsure(x => ConsultaCostosa(x), "...");
```

**2. El mensaje de error también se evalúa siempre.**
No hay sobrecarga con `Func<string>`, así que una interpolación costosa se paga aunque la
condición sea `true`. En la práctica es irrelevante con mensajes normales.

**3. No comprueba `null`.**
`BoolToResult` valida **solo** tu condición:

```csharp
Cliente? c = null;
var r = c.BoolToResult(true, "...");   // ✅ Valid… ¡pero contiene null!

// ✅ Combina las dos comprobaciones
var r = c.NullToFailed("El cliente es obligatorio")
         .MapEnsure(x => x.Activo, "El cliente debe estar activo");
```

**4. En la familia 2, el resultado válido nunca es `false`.**
`MlResult<bool>` es un tipo poco informativo: si es válido, el valor es `true`; si es
`false`, el resultado es `Fail`. Consúmelo con `Bind(_ => ...)`, no leas el valor.

**5. La mayoría de las sobrecargas `*Async` no son realmente asíncronas.**
Las que reciben `this T source` / `this bool source` se limitan a envolver el resultado con
`.ToAsync()` (`Task.FromResult`). Solo las que reciben `Task<T>` / `Task<bool>` esperan el
origen. Además, ninguna acepta un predicado asíncrono: **la condición nunca puede ser un
`Task<bool>` sin `await` previo**.

```csharp
// ❌ No existe una sobrecarga con Func<T, Task<bool>>
// var r = cliente.BoolToResult(await _repo.EstaBloqueadoAsync(id), "...");   ← el await es tuyo

// ✅ Resuelve la condición antes
var bloqueado = await _repo.EstaBloqueadoAsync(id);
var r = cliente.BoolToResult(!bloqueado, "El cliente está bloqueado");

// ✅ O usa Bind con un lambda asíncrono
var r = await clienteResult.BindAsync(async c => (await _repo.EstaBloqueadoAsync(c.Id))
                                                    ? "El cliente está bloqueado".ToMlResultFail<Cliente>()
                                                    : c.ToMlResultValid());
```

---

## Las cuatro formas de expresar el error

Idénticas en las dos familias:

```csharp
// 1) string
var r1 = pedido.BoolToResult(pedido.EsEditable, "El pedido no es editable");

// 2) MlError (catálogo reutilizable)
var r2 = pedido.BoolToResult(pedido.EsEditable, ErroresPedido.NoEditable);

// 3) IEnumerable<string> (mensajes para el usuario final)
var r3 = pedido.BoolToResult(pedido.EsEditable, new[]
{
    "El pedido no se puede modificar",
    $"Estado actual: {pedido.Estado}",
    "Solo los pedidos en borrador son editables"
});

// 4) MlErrorsDetails (mensaje + diagnóstico)
var r4 = pedido.BoolToResult(pedido.EsEditable,
             MlErrorsDetails.FromErrorMessageDetails(
                 "El pedido no es editable",
                 new Dictionary<string, object> { ["PedidoId"] = pedido.Id,
                                                  ["Estado"]   = pedido.Estado.ToString(),
                                                  ["Regla"]    = "PED-013" }));
```

---

## Variantes asíncronas

### Familia 1 (`BoolToResult<T>`)

| Origen | Error como… | Naturaleza |
|--------|-------------|-----------|
| `T` | `MlError` / `MlErrorsDetails` / `string` / `IEnumerable<string>` | Envoltura (`ToAsync()`) |
| `Task<T>` | `MlError` / `MlErrorsDetails` / `string` / `IEnumerable<string>` | **Espera el origen** |

### Familia 2 (`BoolToResult`)

| Origen | Error como… | Naturaleza |
|--------|-------------|-----------|
| `bool` | las cuatro formas | Envoltura |
| `Task<bool>` | las cuatro formas | **Espera el origen** |

En total **8 sobrecargas síncronas** (4 + 4) y **16 asíncronas** (8 + 8).

La variante más útil es la de `Task<bool>` con familia 2, porque permite encadenar
comprobaciones asíncronas de forma directa:

```csharp
// El repositorio devuelve Task<bool>
var r = await _repo.ExisteAsync(nif)
                   .BoolToResultAsync($"No existe ningún cliente con NIF {nif}");
```

---

## `BoolToResult` frente a `EnsureFp.That` y `MapEnsure`

Los tres expresan "esta condición debe cumplirse", pero con papeles distintos:

| Herramienta | Tipo | Condición | Cuándo usarla |
|-------------|------|-----------|---------------|
| `BoolToResult` | Extensión de `T` | `bool` (ya evaluado) | El dato viene de fuera del carril |
| `EnsureFp.That(x, cond, msg)` | Método **estático** | `bool` (ya evaluado) | Validar parámetros al principio de un método |
| [`MapEnsure`](../Map/2_MapEnsure.md) | Extensión de `MlResult<T>` | `Func<T,bool>` (**diferido**) | Ya estás en el carril |

```csharp
// Entrada al método → EnsureFp
public MlResult<Pedido> Cerrar(int pedidoId)
    => EnsureFp.That(pedidoId, pedidoId > 0, "El identificador debe ser positivo")

// Dato externo con condición → BoolToResult
       .Bind(id => _repo.Obtener(id).NullToFailed("Pedido no encontrado"))

// Ya en el carril → MapEnsure (predicado diferido, no se evalúa si hay fallo)
       .MapEnsure(p => p.Estado == EstadoPedido.Borrador, "Solo se cierran pedidos en borrador")
       .MapEnsure(p => p.Lineas.Any(),                    "El pedido no tiene líneas");
```

🔑 **La diferencia crucial** es la evaluación: `MapEnsure` recibe un `Func<T,bool>` y **no
lo ejecuta si el resultado ya venía fallido**; `BoolToResult` recibe un `bool` ya calculado
y por tanto no puede cortocircuitar nada. Con condiciones costosas, prefiere `MapEnsure`.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Validar un objeto externo con una condición y conservarlo | `objeto.BoolToResult(cond, "...")` |
| Comprobar un permiso o interruptor global | `flag.BoolToResult("...")` (familia 2) |
| Lo mismo partiendo de un `Task<bool>` | `.BoolToResultAsync("...")` |
| Validar cuando ya estoy en el carril | [`MapEnsure`](../Map/2_MapEnsure.md) |
| Validar argumentos al entrar en un método | `EnsureFp.That(x, cond, "...")` |
| Fallar si algo es `null` | [`NullToFailed`](2_NullToFailed.md) |
| Fallar si una colección viene vacía | [`EmptyToFailed`](1_EmptyToFailed.md) |
| Elegir entre dos ramas según la condición | [`MapIf`](../Map/3_MapIf.md) o [`BindIf`](../Bind/5_BindIf.md) |
| Acumular **todos** los errores, no cortocircuitar | [`Combine`](4_Combine.md) |

---

## Ejemplos Prácticos

### Ejemplo 1: guardas de negocio con la familia 1

```csharp
public class TransferenciaService
{
    public MlResult<Transferencia> Preparar(Cuenta origen, Cuenta destino, decimal importe)
        => origen.NullToFailed("La cuenta de origen es obligatoria")
                 .Bind(o => o.BoolToResult(o.Activa,
                                MlErrorsDetails.FromErrorMessageDetails(
                                    "La cuenta de origen no está activa",
                                    new Dictionary<string, object> { ["Iban"] = o.Iban, ["Regla"] = "TRF-001" })))
                 .Bind(o => o.BoolToResult(o.Saldo >= importe, new[]
                       {
                           "Saldo insuficiente para realizar la transferencia",
                           $"Saldo disponible: {o.Saldo:C}",
                           $"Importe solicitado: {importe:C}"
                       })
                       .Map(_ => o))
                 .Bind(o => destino.NullToFailed("La cuenta de destino es obligatoria")
                                   .Bind(d => d.BoolToResult(d.Activa, "La cuenta de destino no está activa"))
                                   .Bind(d => d.BoolToResult(d.Iban != o.Iban,
                                                             "El origen y el destino no pueden coincidir"))
                                   .Map(d => new Transferencia(o, d, importe)));
}
```

### Ejemplo 2: comprobaciones de permisos con la familia 2

```csharp
public class DocumentoService
{
    public async Task<MlResult<Documento>> DescargarAsync(int docId, Usuario usuario)
        => await usuario.EstaAutenticado
                        .BoolToResult("Debe iniciar sesión para descargar documentos")
                        .BindAsync(_ => _permisos.PuedeLeerAsync(usuario.Id, docId)
                                                 .BoolToResultAsync(
                                                     MlErrorsDetails.FromErrorMessageDetails(
                                                         "No tiene permiso para acceder a este documento",
                                                         new Dictionary<string, object> { ["Prohibido"] = true })))
                        .BindAsync(_ => _repo.ObtenerAsync(docId)
                                             .NullToFailedAsync("El documento no existe"))
                        .MapAsync(d => d.ConMarcaDeAgua(usuario.Nombre).ToAsync())
                        .ExecSelfIfFailAsync(err => _auditoria.RegistrarAsync(usuario.Id, docId,
                                                                              err.ToErrorsDescription()));
}
```

El detalle `Prohibido` permite responder 403 en el controlador, distinguiéndolo del 404
del documento inexistente.

### Ejemplo 3: validación de ventanas temporales

```csharp
public MlResult<Reserva> Validar(Reserva reserva, ConfiguracionReservas config)
{
    var ahora     = DateTime.UtcNow;
    var antelacion = reserva.Inicio - ahora;

    return reserva.BoolToResult(reserva.Inicio > ahora,
                                "La fecha de inicio debe ser futura")
                  .Bind(r => r.BoolToResult(antelacion >= config.AntelacionMinima,
                                            $"Debe reservar con al menos {config.AntelacionMinima.TotalHours:0} horas de antelación"))
                  .Bind(r => r.BoolToResult(antelacion <= config.AntelacionMaxima,
                                            $"No se puede reservar con más de {config.AntelacionMaxima.TotalDays:0} días de antelación"))
                  .Bind(r => r.BoolToResult(r.Fin > r.Inicio,
                                            "La fecha de fin debe ser posterior a la de inicio"))
                  .Bind(r => r.BoolToResult((r.Fin - r.Inicio) <= config.DuracionMaxima,
                                            $"La duración máxima es de {config.DuracionMaxima.TotalHours:0} horas"));
}
```

Nota: aquí todas las condiciones son cálculos baratos, así que la evaluación no diferida no
supone problema. Si alguna implicara una consulta, convendría usar `MapEnsure`.

### Ejemplo 4: comprobar la configuración al arrancar

```csharp
public static MlResult<AppOpciones> Verificar(AppOpciones opciones)
    => opciones.NullToFailed("No se ha cargado la configuración")
               .Bind(o => o.BoolToResult(!string.IsNullOrWhiteSpace(o.CadenaConexion),
                                         "Falta la cadena de conexión"))
               .Bind(o => o.BoolToResult(Directory.Exists(o.RutaTemporal),
                                         $"La ruta temporal '{o.RutaTemporal}' no existe"))
               .Bind(o => o.BoolToResult(o.TiempoEsperaSegundos is > 0 and <= 300,
                                         "El tiempo de espera debe estar entre 1 y 300 segundos"))
               .ExecSelfIfFail(err => Console.Error.WriteLine(err.ToErrorsDescription()));
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ Condición costosa evaluada aunque el carril ya haya fallado
var r = previo.Bind(x => x.BoolToResult(_repo.CuentaRegistros(x.Id) > 0, "Sin registros"));
//      (aquí Bind sí protege, pero es más claro y directo con MapEnsure)

// ✅ MapEnsure: predicado diferido e idiomático
var r = previo.MapEnsure(x => _repo.CuentaRegistros(x.Id) > 0, "Sin registros");


// ❌ Suponer que BoolToResult comprueba el null
Cliente? c = null;
var r = c.BoolToResult(true, "...");   // Valid… ¡conteniendo null!

// ✅ Encadena NullToFailed primero
var r = c.NullToFailed("El cliente es obligatorio")
         .MapEnsure(x => x.Activo, "El cliente debe estar activo");


// ❌ Leer el valor de un MlResult<bool> de la familia 2
// var paso = resultado.Value;   // siempre true si es válido: no informa de nada

// ✅ Descarta el valor con Bind
resultado.Bind(_ => ContinuarProceso());


// ❌ Cadena de guardas donde cada Bind repite el valor a mano
var r = pedido.BoolToResult(c1, "...").Bind(p => p.BoolToResult(c2, "...").Map(_ => p));


// ✅ Con MapEnsure el valor se conserva solo
var r = pedido.ToMlResultValid()
              .MapEnsure(p => c1(p), "...")
              .MapEnsure(p => c2(p), "...");
```

---

## Mejores Prácticas

1. **Usa la familia 1 (`BoolToResult<T>`) por defecto**: conserva el valor y permite seguir
   encadenando sin trucos.
2. **Reserva la familia 2 (`BoolToResult` sobre `bool`)** para comprobaciones sin sujeto
   natural: permisos, interruptores de configuración, disponibilidad de servicios.
3. **Si ya estás en el carril, prefiere `MapEnsure`**: su predicado es diferido, conserva
   el valor y encadena de forma más limpia.
4. **Recuerda que la condición se evalúa siempre.** No pongas consultas ni cálculos caros
   directamente en el argumento `condition`.
5. **Combínalo con `NullToFailed`**: `BoolToResult` no comprueba el `null`.
6. **Usa la sobrecarga de `MlErrorsDetails`** para incluir el código de regla, los
   identificadores y marcas como `Prohibido` o `NoEncontrado`, y decidir el código HTTP al
   final de la tubería.
7. **Usa la sobrecarga de `IEnumerable<string>`** cuando el usuario necesite ver el valor
   actual, el límite y la acción correctiva.
8. **Resuelve las condiciones asíncronas antes** de llamar (no existe sobrecarga con
   `Task<bool>` como condición) o usa `BindAsync` con un lambda asíncrono.
9. **Si necesitas acumular todos los errores** en lugar de cortocircuitar en el primero,
   usa [`Combine`](4_Combine.md).
10. **Nombra las reglas** (`"PED-013"`, `"TRF-001"`) en los `Details`: facilita el soporte
    y las pruebas.

---

## Resumen

- `BoolToResult` convierte **cualquier condición booleana** en un `MlResult`: **Valid** si
  es `true`, **Fail** con tu error si es `false`.
- Hay **dos familias**: `BoolToResult<T>(source, condition, error)` conserva el valor;
  `BoolToResult(this bool, error)` devuelve `MlResult<bool>` y se consume con `Bind(_ => ...)`.
- **8 sobrecargas síncronas** (4 por familia: `MlErrorsDetails`, `MlError`, `string`,
  `IEnumerable<string>`) y **16 asíncronas** (8 por familia, la mitad simples envolturas y
  la mitad sobre `Task<...>`, que sí esperan el origen).
- El método base de cada familia es el de `MlErrorsDetails`; el resto delega en él.
- ⚠️ La condición es un **`bool` ya evaluado**, no un `Func<T,bool>`: **no hay evaluación
  diferida ni cortocircuito**. Con condiciones costosas, usa
  [`MapEnsure`](../Map/2_MapEnsure.md).
- ⚠️ **No comprueba `null`**: combínalo con [`NullToFailed`](2_NullToFailed.md).
- ⚠️ En la familia 2, el valor válido es siempre `true`: no lo leas.
- **No existe `TryBoolToResult`**: no hay delegado de usuario que pueda lanzar.

---

## Ver también

- [`EmptyToFailed`](1_EmptyToFailed.md) — fallar si una colección viene vacía
- [`NullToFailed`](2_NullToFailed.md) — fallar si un objeto es `null`
- [`Combine`](4_Combine.md) — acumular los errores de varias validaciones
- [`MapEnsure`](../Map/2_MapEnsure.md) — validar con predicado diferido dentro del carril
- [`MapIf`](../Map/3_MapIf.md) y [`BindIf`](../Bind/5_BindIf.md) — bifurcar según una condición
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — `That`, `NotNull`, `NotEmpty` para validar argumentos
- [`MlResultErrors`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y sus fábricas
- [`ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md) — registrar el fallo sin alterar el carril