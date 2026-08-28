# EmptyToFailed — Convertir una colección vacía en un fallo

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [Un detalle importante: no es un operador del carril](#un-detalle-importante-no-es-un-operador-del-carril)
4. [Firmas reales e implementación](#firmas-reales-e-implementación)
5. [Las tres formas de expresar el error](#las-tres-formas-de-expresar-el-error)
6. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
9. [Ejemplos Prácticos](#ejemplos-prácticos)
10. [Mejores Prácticas](#mejores-prácticas)
11. [Resumen](#resumen)
12. [Ver también](#ver-también)

---

## Introducción

`EmptyToFailed` es una **puerta de entrada al carril**: recibe una colección normal
(`IEnumerable<T>`) y devuelve un `MlResult<IEnumerable<T>>` que será:

- **válido** si la colección tiene al menos un elemento, o
- **fallido** con el error que tú indiques si la colección es `null` o está vacía.

```csharp
// ❌ Estilo imperativo: la comprobación se repite en cada capa
var clientes = _repo.BuscarPorProvincia("Málaga");
if (clientes is null || !clientes.Any())
    return NotFound("No hay clientes en esa provincia");

// ✅ Con EmptyToFailed: la comprobación entra en el carril y se encadena
return _repo.BuscarPorProvincia("Málaga")
            .EmptyToFailed("No hay clientes en esa provincia")
            .Map(cs => cs.Select(c => c.ToDto()))
            .Match(valid: dtos => Ok(dtos),
                   fail : err  => NotFound(err.ToErrorsMessages()));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema que resuelve

En la mayoría de las aplicaciones, "la consulta no devolvió nada" **no es un error técnico**:
es una respuesta perfectamente normal del almacenamiento. Pero muchas veces sí es un
**error de negocio**: si no hay tarifas activas, no puedes facturar; si no hay líneas de
pedido, no puedes cerrar el pedido.

El código imperativo resuelve esto con un `if` que se repite en todas las capas y que,
además, se olvida con facilidad. `EmptyToFailed` convierte ese `if` en un eslabón de la
tubería:

| Entrada | Salida |
|---------|--------|
| `null` | `MlResult` **Fail** con tu error |
| Colección con 0 elementos | `MlResult` **Fail** con tu error |
| Colección con 1 o más elementos | `MlResult` **Valid** con la misma colección |

Fíjate en que la colección devuelta es **la misma instancia** que entró: `EmptyToFailed`
no copia, no materializa a lista y no transforma nada.

---

## Un detalle importante: no es un operador del carril

🔑 A diferencia de `Map`, `Bind` o `Match`, **`EmptyToFailed` no es una extensión de
`MlResult<T>`**: es una extensión de `IEnumerable<T>`.

```csharp
// ✅ Correcto: se aplica sobre la colección "desnuda"
IEnumerable<Factura> facturas = _repo.Pendientes();
MlResult<IEnumerable<Factura>> resultado = facturas.EmptyToFailed("Sin facturas pendientes")!;

// ❌ NO compila: el origen ya es un MlResult, no un IEnumerable
MlResult<IEnumerable<Factura>> yaEnCarril = ObtenerFacturas();
yaEnCarril.EmptyToFailed("Sin facturas pendientes");   // error de compilación

// ✅ Si ya estás en el carril, entra con Bind
yaEnCarril.Bind(fs => fs.EmptyToFailed("Sin facturas pendientes")!);
```

Este matiz es la causa de la mayoría de las confusiones con este método: piensa en él
como en `NullToFailed` o `BoolToResult`, es decir, como en un **constructor de
`MlResult` a partir de un dato del mundo exterior**.

---

## Firmas reales e implementación

```csharp
public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items,
                                                              MlError        error)
    => (items != null && items.Any())
            ? items.ToMlResultValid()
            : error.ToMlResultFail<IEnumerable<T>>();

public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T>  items,
                                                              MlErrorsDetails errorsDetails)
    => (items != null && items.Any())
            ? items.ToMlResultValid()
            : errorsDetails.ToMlResultFail<IEnumerable<T>>();

public static MlResult<IEnumerable<T>>? EmptyToFailed<T>(this IEnumerable<T> items,
                                                              string         messageError)
    => EmptyToFailed(items, MlError.FromErrorMessage(messageError));
```

Puntos que conviene retener:

| Detalle | Consecuencia práctica |
|---------|----------------------|
| La comprobación es `items != null && items.Any()` | Cubre el `null` **y** el vacío en una sola llamada; no necesitas `NullToFailed` antes |
| El valor válido es `items`, tal cual | No hay copia ni materialización: la pereza de LINQ se conserva |
| La sobrecarga de `string` delega en la de `MlError` | Un único punto de verdad: no hay divergencias de comportamiento entre sobrecargas |
| No existe `TryEmptyToFailed` | No hace falta: el método no invoca ningún delegado tuyo, así que no puede lanzar excepciones de usuario |

---

## Las tres formas de expresar el error

Todas las sobrecargas se diferencian **solo** en cómo describes el fallo:

```csharp
// 1) string  → el caso habitual, mensaje literal
var r1 = lineas.EmptyToFailed("El pedido no tiene líneas");

// 2) MlError → cuando reutilizas un catálogo de errores
public static class ErroresPedido
{
    public static readonly MlError SinLineas = MlError.FromErrorMessage("El pedido no tiene líneas");
}
var r2 = lineas.EmptyToFailed(ErroresPedido.SinLineas);

// 3) MlErrorsDetails → cuando quieres varios errores y/o detalles de diagnóstico
var detalles = MlErrorsDetails.FromErrorMessageDetails(
                    "El pedido no tiene líneas",
                    new Dictionary<string, object> { ["PedidoId"] = pedidoId,
                                                     ["Origen"]   = "CierrePedidoService" });
var r3 = lineas.EmptyToFailed(detalles);
```

La tercera forma es la más valiosa en servicios reales: los `Details` viajan con el error
por toda la tubería y puedes recuperarlos al final con `ToDetailsDescription()` o
`GetDetail<T>("PedidoId")`.

---

## ⚠️ Particularidades reales del código fuente

**1. El tipo de retorno está anotado como nullable (`MlResult<IEnumerable<T>>?`)**
aunque el método **nunca devuelve `null`**. Es una anotación heredada del diseño inicial.
En proyectos con *nullable reference types* activados esto provoca avisos, y por eso
verás `!` en varias llamadas internas de la propia librería:

```csharp
// La librería misma se ve obligada a usar '!'
MlResult<IEnumerable<T>> seguro = items.EmptyToFailed(error)!;
```

Recomendación: encapsula la llamada en un método propio que devuelva el tipo no nullable,
o añade el `!` de forma sistemática.

**2. Se enumera la secuencia para comprobar `Any()`.**
Con una `List<T>` esto es gratis. Con un `IEnumerable<T>` diferido (un `yield`, una
consulta LINQ, un `IQueryable`) **se dispara la enumeración** y, si la fuente no es
reenumerable, puedes perder el primer elemento o pagar dos veces la consulta:

```csharp
// ⚠️ Riesgo: la consulta se ejecuta en Any() y otra vez al recorrer el resultado
var r = _db.Clientes.Where(c => c.Activo)          // IQueryable diferido
                    .EmptyToFailed("Sin clientes activos");

// ✅ Materializa antes de comprobar
var r = _db.Clientes.Where(c => c.Activo)
                    .ToList()
                    .EmptyToFailed("Sin clientes activos");
```

**3. La sobrecarga `EmptyToFailedAsync<T>(this IEnumerable<T>, string)` no es realmente
asíncrona.** Se limita a envolver el resultado con `.ToAsync()` (`Task.FromResult`).
Existe solo para poder encadenar sin romper la cadena `await`; no aporta paralelismo.

---

## Variantes asíncronas

Hay dos grupos, según **de dónde venga la colección**:

| Origen | Error como… | Firma | Naturaleza |
|--------|-------------|-------|-----------|
| `IEnumerable<T>` | `MlError` | `EmptyToFailedAsync<T>(this IEnumerable<T>, MlError)` | Envoltura (`ToAsync()`) |
| `IEnumerable<T>` | `MlErrorsDetails` | `EmptyToFailedAsync<T>(this IEnumerable<T>, MlErrorsDetails)` | Envoltura |
| `IEnumerable<T>` | `string` | `EmptyToFailedAsync<T>(this IEnumerable<T>, string)` | Envoltura |
| `Task<IEnumerable<T>>` | `MlError` | `EmptyToFailedAsync<T>(this Task<IEnumerable<T>>, MlError)` | **Espera el origen** |
| `Task<IEnumerable<T>>` | `MlErrorsDetails` | `EmptyToFailedAsync<T>(this Task<IEnumerable<T>>, MlErrorsDetails)` | **Espera el origen** |
| `Task<IEnumerable<T>>` | `string` | `EmptyToFailedAsync<T>(this Task<IEnumerable<T>>, string)` | **Espera el origen** |

Las tres últimas son las verdaderamente útiles, porque te permiten enlazar directamente
con un repositorio asíncrono sin `await` intermedio:

```csharp
// Sin variante asíncrona: hay que romper la expresión
var pedidos = await _repo.PendientesAsync(clienteId);
var resultado = pedidos.EmptyToFailed("Sin pedidos pendientes")!;

// Con variante asíncrona: una sola expresión encadenable
var resultado = await _repo.PendientesAsync(clienteId)
                           .EmptyToFailedAsync("Sin pedidos pendientes")!;
```

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Fallar si la colección viene vacía o `null` | `EmptyToFailed(...)` |
| Fallar si un objeto individual es `null` | [`NullToFailed`](2_NullToFailed.md) |
| Fallar según una condición booleana arbitraria | [`BoolToResult`](3_BoolToResult.md) |
| Validar el contenido, no la cantidad | [`MapEnsure`](../Map/2_MapEnsure.md) |
| Comprobar el vacío cuando ya estoy en el carril | `.Bind(items => items.EmptyToFailed("...")!)` |
| Comprobar el vacío sin salir del carril y sin cambiar el tipo | `.MapEnsure(items => items.Any(), "...")` |
| Partir de una colección que llega en un `Task` | `EmptyToFailedAsync` (sobrecargas de `Task<IEnumerable<T>>`) |

💡 **`EmptyToFailed` frente a `MapEnsure`**: si la colección viene de fuera del carril,
usa `EmptyToFailed` (es un constructor). Si ya está dentro de un `MlResult`, prefiere
`MapEnsure(items => items.Any(), "...")`, que es más idiomático y no obliga a `Bind` ni a `!`.

---

## Ejemplos Prácticos

### Ejemplo 1: cierre de pedido — la colección vacía es un error de negocio

```csharp
public class CierrePedidoService
{
    private readonly IPedidoRepository _repo;
    private readonly ITarifaRepository _tarifas;

    public async Task<MlResult<PedidoCerrado>> CerrarAsync(int pedidoId)
        => await EnsureFp.That(pedidoId, pedidoId > 0, "El identificador de pedido debe ser positivo")
                         .BindAsync(id => _repo.ObtenerLineasAsync(id)
                                               .EmptyToFailedAsync(
                                                   MlErrorsDetails.FromErrorMessageDetails(
                                                       "No se puede cerrar un pedido sin líneas",
                                                       new Dictionary<string, object> { ["PedidoId"] = id }))!)
                         .BindAsync(async lineas =>
                         {
                             var tarifas = await _tarifas.ActivasAsync();
                             return tarifas.EmptyToFailed("No hay tarifas activas: no se puede valorar el pedido")!
                                           .Map(ts => (Lineas: lineas, Tarifas: ts));
                         })
                         .MapAsync(par => new PedidoCerrado(pedidoId,
                                                            Valorar(par.Lineas, par.Tarifas),
                                                            DateTime.UtcNow).ToAsync());
}
```

Dos comprobaciones de vacío, dos motivos de negocio distintos y ninguna sentencia `if`.

### Ejemplo 2: controlador web — distinguir 404 de 400

```csharp
[HttpGet("provincias/{provincia}/clientes")]
public async Task<IActionResult> Buscar(string provincia)
    => await EnsureFp.NotNullEmptyOrWhitespace(provincia, "Debe indicar una provincia")
                     .BindAsync(p => _repo.BuscarPorProvinciaAsync(p)
                                          .EmptyToFailedAsync(
                                              MlErrorsDetails.FromErrorMessageDetails(
                                                  "No hay clientes en la provincia indicada",
                                                  new Dictionary<string, object> { ["NoEncontrado"] = true }))!)
                     .MapAsync(cs => cs.Select(c => c.ToDto()).ToAsync())
                     .MatchAsync(
                          valid: dtos => Ok(dtos).ToAsync<IActionResult>(),
                          fail : err  => (err.HasKeyDetails("NoEncontrado")
                                              ? NotFound(err.ToErrorsMessages())
                                              : BadRequest(err.ToErrorsMessages())).ToAsync<IActionResult>());
```

Marcar el error con un detalle (`NoEncontrado`) permite decidir el código HTTP **al final**
de la tubería, sin propagar excepciones ni tipos de error especiales.

### Ejemplo 3: importación por lotes — vacío tolerado frente a vacío intolerable

```csharp
public async Task<MlResult<InformeImportacion>> ImportarAsync(string ruta)
{
    // El fichero DEBE traer filas: si no, es un error
    var filas = await LeerCsvAsync(ruta);

    return await filas.EmptyToFailed($"El fichero '{ruta}' no contiene filas de datos")!
                      .BindAsync(fs => fs.ProjectionAsync(ValidarFilaAsync))     // acumula errores por fila
                      .BindAsync(validas =>
                      {
                          // Aquí el vacío SÍ se tolera: puede que todas las filas fueran duplicados
                          var nuevas = validas.Where(v => !_cache.Existe(v.Codigo)).ToList();
                          return nuevas.Any()
                                     ? _repo.InsertarLoteAsync(nuevas)
                                     : new InformeImportacion(0, validas.Count()).ToMlResultValidAsync();
                      });
}
```

Regla de oro: usa `EmptyToFailed` **solo** cuando el vacío impida continuar. Si el vacío
es un resultado legítimo, no lo conviertas en fallo.

### Ejemplo 4: reutilizar un catálogo de errores

```csharp
public static class ErroresCatalogo
{
    public static readonly MlErrorsDetails SinTarifas =
        MlErrorsDetails.FromErrorMessageDetails("No hay tarifas activas",
            new Dictionary<string, object> { ["Codigo"] = "TAR-001", ["Severidad"] = "Alta" });

    public static readonly MlErrorsDetails SinAlmacenes =
        MlErrorsDetails.FromErrorMessageDetails("No hay almacenes operativos",
            new Dictionary<string, object> { ["Codigo"] = "ALM-004", ["Severidad"] = "Critica" });
}

// Uso: mensajes consistentes en toda la aplicación
var tarifas   = (await _tarifas.ActivasAsync()).EmptyToFailed(ErroresCatalogo.SinTarifas)!;
var almacenes = (await _almacenes.OperativosAsync()).EmptyToFailed(ErroresCatalogo.SinAlmacenes)!;

// Y al reportar, los detalles vienen incluidos
tarifas.ExecSelfIfFail(err => _log.LogWarning("{Desc}", err.ToDetailsDescription()));
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ Enumerar dos veces una fuente diferida
var r = ConsultaCostosaDiferida()
            .EmptyToFailed("Sin datos");     // Any() ejecuta la consulta…
var lista = r.Match(valid: x => x.ToList(), fail: _ => new List<T>());  // …y aquí otra vez

// ✅ Materializar una sola vez
var datos = ConsultaCostosaDiferida().ToList();
var r     = datos.EmptyToFailed("Sin datos")!;


// ❌ Comprobar el null "por si acaso" antes de EmptyToFailed
var r = (items ?? Enumerable.Empty<Cliente>()).EmptyToFailed("Sin clientes");

// ✅ EmptyToFailed ya cubre el null
var r = items.EmptyToFailed("Sin clientes")!;


// ❌ Acceder al valor directamente
// var clientes = resultado.Value;

// ✅ Salir del carril con Match
var clientes = resultado.Match(valid: cs => cs, fail: _ => Enumerable.Empty<Cliente>());
```

---

## Mejores Prácticas

1. **Usa `EmptyToFailed` solo cuando el vacío bloquee el proceso.** Una lista vacía
   devuelta a un listado de UI es una respuesta válida, no un error.
2. **Materializa las fuentes diferidas** (`ToList()`, `ToArray()`) antes de llamarlo, para
   evitar enumerar dos veces la consulta.
3. **Prefiere la sobrecarga de `MlErrorsDetails`** en servicios: los `Details` permiten
   decidir el código HTTP o la política de reintento al final de la tubería.
4. **Centraliza los errores frecuentes** en una clase estática de catálogo, como en el
   ejemplo 4, para mantener mensajes y códigos consistentes.
5. **Si ya estás en el carril, usa `MapEnsure(items => items.Any(), "...")`** en lugar de
   `Bind(... EmptyToFailed ...)`: es más corto y no necesita el operador `!`.
6. **Añade `!` de forma sistemática** o encapsula la llamada, para convivir con la
   anotación nullable del tipo de retorno.
7. **Usa las sobrecargas de `Task<IEnumerable<T>>`** para enlazar con repositorios
   asíncronos sin romper la expresión.
8. **Escribe mensajes que expliquen la consecuencia**, no solo el hecho: en vez de
   "lista vacía", escribe "no se puede cerrar un pedido sin líneas".
9. **No lo combines con `NullToFailed`**: la comprobación de `null` ya está incluida.

---

## Resumen

- `EmptyToFailed` transforma un `IEnumerable<T>` en `MlResult<IEnumerable<T>>`: **Fail** si
  es `null` o está vacío, **Valid** con la misma colección en caso contrario.
- Es un **constructor de `MlResult`**, extensión de `IEnumerable<T>`, no un operador del
  carril: para usarlo dentro de una tubería necesitas `Bind`.
- **3 sobrecargas síncronas** (`MlError`, `MlErrorsDetails`, `string`) y **6 asíncronas**
  (las mismas tres sobre `IEnumerable<T>`, que son meras envolturas, y sobre
  `Task<IEnumerable<T>>`, que sí esperan el origen).
- **No existe `TryEmptyToFailed`**: el método no invoca delegados de usuario.
- ⚠️ El tipo de retorno está anotado como nullable aunque nunca devuelve `null`.
- ⚠️ Llama a `Any()`, con el consiguiente riesgo de doble enumeración en fuentes diferidas.
- Alternativa idiomática cuando ya estás en el carril: `MapEnsure(items => items.Any(), "...")`.

---

## Ver también

- [`NullToFailed`](2_NullToFailed.md) — el equivalente para objetos individuales
- [`BoolToResult`](3_BoolToResult.md) — construir un `MlResult` a partir de una condición
- [`Combine`](4_Combine.md) — fusionar varios `MlResult`
- [`MapEnsure`](../Map/2_MapEnsure.md) — validar sin salir del carril
- [`Bind`](../Bind/3_Bind.md) — encadenar operaciones que devuelven `MlResult`
- [`Bucles y proyecciones`](../Bucle/Bucles.md) — recorrer colecciones dentro del carril
- [`MlResultErrors`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y sus fábricas
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — validaciones de entrada al carril