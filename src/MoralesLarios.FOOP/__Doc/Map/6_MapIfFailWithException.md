# MapIfFailWithException — Recuperarse solo cuando el fallo trae una excepción

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [Cómo llega la excepción a los detalles](#cómo-llega-la-excepción-a-los-detalles)
4. [La regla de oro: sin excepción no hay recuperación](#la-regla-de-oro-sin-excepción-no-hay-recuperación)
5. [Las cuatro formas de `MapIfFailWithException`](#las-cuatro-formas-de-mapiffailwithexception)
6. [Firmas reales e implementación](#firmas-reales-e-implementación)
7. [Filtrar por tipo de excepción con `TException`](#filtrar-por-tipo-de-excepción-con-texception)
8. [La familia `MapIfFailWithExceptionError`](#la-familia-mapiffailwithexceptionerror)
9. [Variantes asíncronas](#variantes-asíncronas)
10. [`TryMapIfFailWithException` — cuando la recuperación puede lanzar](#trymapiffailwithexception--cuando-la-recuperación-puede-lanzar)
11. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
12. [Ejemplos Prácticos](#ejemplos-prácticos)
13. [Mejores Prácticas](#mejores-prácticas)
14. [Resumen](#resumen)
15. [Ver también](#ver-también)

---

## Introducción

No todos los fallos son iguales. En una tubería `MlResult` conviven dos naturalezas muy
distintas de error:

- **Errores de negocio**: «el NIF no es válido», «el stock es insuficiente». Son esperados,
  los produce tu propio código con `ToMlResultFail` o `EnsureFp`.
- **Errores técnicos**: una `SqlException`, un `HttpRequestException`, un `TimeoutException`.
  Los captura un método `Try*` y quedan guardados como excepción dentro del error.

`MapIfFailWithException` es la herramienta para **reaccionar solo a los segundos**. Su
delegado de recuperación recibe la `Exception` real, y si el fallo no lleva excepción, la
operación **no hace nada**: el fallo sale tal cual entró.

```csharp
// ❌ MapIfFail no distingue: recupera igual un NIF inválido que una caída de red
var r = ConsultarTarifa(sku).MapIfFail(_ => Tarifa.Cacheada(sku));

// ✅ MapIfFailWithException: solo tira de caché si el problema fue técnico
var r = ConsultarTarifa(sku)
            .MapIfFailWithException(ex => Tarifa.Cacheada(sku));
//        un error de negocio ("SKU no catalogado") se propaga intacto
```

> ⚠️ **Sobre `MlErrorsDetails`**
> `MlErrorsDetails` solo expone dos propiedades: `Errors` (una colección de `MlError`) y
> `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer errores usa
> `ToErrorsMessages()`, `ToErrorsDescription()` o `Errors.First().Message`; para obtener la
> excepción usa `GetDetailException()` o `GetDetailException<TException>()`.

---

## El problema que resuelve

Mezclar la gestión de fallos de negocio y de fallos técnicos es una de las causas más
frecuentes de bugs sutiles: un `catch` demasiado ancho convierte un error de validación en un
reintento, o al contrario, una caída de infraestructura se presenta al usuario como si fuera
culpa de sus datos.

`MapIfFailWithException` te da un **filtro semántico** en medio de la tubería:

| Quiero… | Uso |
|---|---|
| Recuperarme de cualquier fallo | `MapIfFail` |
| Recuperarme **solo de fallos técnicos** | `MapIfFailWithException` |
| Recuperarme **solo de fallos de negocio** | `MapIfFailWithoutException` |
| Recuperarme de un tipo concreto de excepción | `MapIfFailWithException<T, TException>` |

Es, en la práctica, la versión funcional y tipada de un `catch (SqlException)` colocado en el
punto exacto de la tubería donde tiene sentido.

---

## Cómo llega la excepción a los detalles

La excepción no aparece por arte de magia: alguien la capturó y la guardó en
`Details["Ex"]` (la constante `EX_DESC_KEY`). Las vías habituales son:

```csharp
// 1) Cualquier método Try* de la librería lo hace por ti
var r = origen.TryMap(x => JsonSerializer.Deserialize<Dto>(x)!,
                      ex => $"JSON inválido: {ex.Message}");
//        si Deserialize lanza → fallo con la excepción en Details["Ex"]

// 2) Explícitamente, al construir el error
return MlErrorsDetails.FromErrorMessageWithException("No se pudo abrir el fichero", ex)
                      .ToMlResultFail<Contenido>();

// 3) Añadiéndola a un error que ya existe
errores.AppendExDetails(ex);
```

Y así se lee:

```csharp
// La excepción concreta, sin filtrar por tipo
MlResult<Exception> exResult = errores.GetDetailException();

// Filtrando por tipo (falla si la guardada no es un TimeoutException)
MlResult<TimeoutException> toResult = errores.GetDetailException<TimeoutException>();

// Comprobación booleana rápida
bool esTecnico = errores.GetDetailException().IsValid;
```

> 📌 `AppendExDetails` numera las excepciones sucesivas como `Ex`, `Ex2`, `Ex3`… si se apilan
> varias. `GetDetailException()` lee la clave `Ex`.

---

## La regla de oro: sin excepción no hay recuperación

El propio autor lo dejó escrito como comentario justo al abrir la región, contrastándolo con
la familia hermana que trabaja con el valor guardado:

```text
En el caso de MapIfFailWithException es diferente al MapIfFailWithValue.

    1.- MapIfFailWithValue: Si recibe un MlResult Fail sin ValueDetail, añadira un nuevo Error
        al que le viene de la ejecución anterior

    2.- MapIfFailWithException: Si recibe un MlResult Fail sin ExceptionDetail, Devolvera el
        MlResult Fail, igual que le vino
```

Esa diferencia es **la característica más importante de esta familia** y la que la hace
segura de usar:

| Estado de entrada | ¿Hay `Details["Ex"]`? | Qué ocurre |
|---|---|---|
| Válido | irrelevante | se devuelve el valor; el delegado no se ejecuta |
| Fallido | **sí** | se ejecuta el delegado con la excepción → resultado **válido** |
| Fallido | **no** | se devuelve **el mismo fallo, con sus errores originales intactos** |

En otras palabras: puedes intercalar `MapIfFailWithException` en cualquier punto sin miedo a
degradar los mensajes de error de negocio. Si no aplica, es transparente.

---

## Las cuatro formas de `MapIfFailWithException`

La región publica cuatro variantes que combinan dos ejes: **si el tipo de salida cambia** y
**si se filtra por tipo de excepción**.

| | Genéricos | Delegados | Salida |
|---|---|---|---|
| **Forma A** | `<T>` | `Func<Exception, T> funcException` | `MlResult<T>` |
| **Forma B** | `<T, TReturn>` | `Func<T, TReturn> funcValid` + `Func<Exception, TReturn> funcFail` | `MlResult<TReturn>` |
| **Forma C** | `<T, TException>` | `Func<TException, T> funcException` | `MlResult<T>` |
| **Forma D** | `<T, TReturn, TException>` | `Func<T, TReturn> funcValid` + `Func<TException, TReturn> funcFail` | `MlResult<TReturn>` |

Las formas **C** y **D** llevan la restricción `where TException : Exception`.

---

## Firmas reales e implementación

### Forma A — recuperación en el mismo tipo

```csharp
/// <summary>
/// Execute the function if the source is fail, otherwise return the source.
/// source parameter has a prevous Exception execution or 'ex' ErrorDetail
/// </summary>
public static MlResult<T> MapIfFailWithException<T>(this MlResult<T>        source,
                                                         Func<Exception, T> funcException)
    => source.Match(
                        fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                    fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                    valid: ex              => funcException(ex).ToMlResultValid()
                                                ),
                        valid: value         => value
                    );
```

Fíjate en el `Match` anidado: el interno decide si hay excepción. En la rama `fail` interna se
devuelve el fallo, y en la `valid` interna se ejecuta la recuperación, que **siempre produce
un resultado válido** porque `funcException` devuelve un `T` desnudo.

### Forma B — las dos ramas convergen en `TReturn`

```csharp
public static MlResult<TReturn> MapIfFailWithException<T, TReturn>(this MlResult<T>              source,
                                                                        Func<T        , TReturn> funcValid,
                                                                        Func<Exception, TReturn> funcFail)
    => source.Match(
                        fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                    fail : _  => errorsDetails.ToMlResultFail<TReturn>(),
                                                    valid: ex => funcFail(ex).ToMlResultValid()
                                                ),
                        valid: x => funcValid(x).ToMlResultValid()
                    );
```

Es un `Match` con la rama de fallo **condicionada a que exista excepción**. Muy útil para
proyectar a un modelo de vista distinguiendo «funcionó», «falló por algo técnico» y «falló por
negocio» (este último caso sale como fallo y lo tratas fuera).

---

## Filtrar por tipo de excepción con `TException`

Las formas C y D cambian `GetDetailException()` por `GetDetailException<TException>()`, que
falla si la excepción guardada **no es** del tipo pedido:

```csharp
public static MlResult<T> MapIfFailWithException<T, TException>(this MlResult<T>         source,
                                                                     Func<TException, T> funcException)
    where TException : Exception
    => source.Match(
                        fail : errorsDetails => errorsDetails.GetDetailException<TException>().Match(
                                                    fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                    valid: ex              => funcException(ex).ToMlResultValid()
                                                ),
                        valid: value         => value
                    );
```

Esto te permite escribir el equivalente a varios `catch` tipados, en cadena y sin excepciones
de control de flujo:

```csharp
var resultado = LeerConfiguracion(ruta)
                    .MapIfFailWithException<Config, FileNotFoundException>(_  => Config.PorDefecto)
                    .MapIfFailWithException<Config, JsonException>        (ex => Config.PorDefecto with
                                                                                 {
                                                                                     Aviso = $"Config corrupta: {ex.Message}"
                                                                                 });
// Una IOException, o un error de negocio, siguen su camino como fallo.
```

> ⚠️ Los genéricos de las formas C y D **casi nunca se infieren**: al indicar `TException`
> tienes que escribir también `T` (y `TReturn` en la forma D). Es el motivo por el que en los
> ejemplos aparecen siempre explícitos.

---

## La familia `MapIfFailWithExceptionError`

Junto a las cuatro formas anteriores, la región publica una **subfamilia paralela** con el
sufijo `Error`. La condición de activación es la misma (que haya excepción), pero el delegado
de fallo recibe **el `MlErrorsDetails` completo** en lugar de la excepción suelta:

```csharp
public static MlResult<T> MapIfFailWithExceptionError<T>(this MlResult<T>              source,
                                                              Func<MlErrorsDetails, T> funcFail)
    => source.Match(
                        fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                    fail : exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(),
                                                    valid: ex              => funcFail(errorsDetails).ToMlResultValid()
                                                ),
                        valid: value         => value
                    );
```

🔑 Observa que la variable `ex` de la rama válida **se descarta**: solo sirve como *guarda*.
El delegado trabaja con `errorsDetails`, de donde puede sacar tanto los mensajes como la
excepción, si la necesita.

Úsala cuando la recuperación necesite **mensajes y excepción a la vez**:

```csharp
var r = SincronizarAsync(lote)
            .MapIfFailWithExceptionError(errores => new ResultadoSync
            {
                Estado   = "Reintentable",
                Mensajes = errores.ToErrorsMessages().ToList(),
                Detalle  = errores.ToDetailsDescription()
            });
```

La subfamilia replica las mismas cuatro formas: `<T>`, `<T, TReturn>`, `<T, TException>` y
`<T, TReturn, TException>`, cada una con sus variantes `Async` y `Try`.

---

## Variantes asíncronas

Todas las formas tienen su `…Async`, combinando origen síncrono/asíncrono y delegados
síncronos/asíncronos:

| Forma | Origen | Delegado(s) |
|---|---|---|
| A | `MlResult<T>` | `Func<Exception, Task<T>>` |
| A | `Task<MlResult<T>>` | `Func<Exception, Task<T>>` |
| A | `Task<MlResult<T>>` | `Func<Exception, T>` |
| B | `MlResult<T>` | ambos asíncronos |
| B | `Task<MlResult<T>>` | ambos asíncronos |
| C | `MlResult<T>` / `Task<MlResult<T>>` | `Func<TException, Task<T>>` y `Func<TException, T>` |
| D | `MlResult<T>` / `Task<MlResult<T>>` | mezclas de síncrono y asíncrono |

Internamente usan `MatchAsync` y `GetDetailExceptionAsync()`:

```csharp
public static async Task<MlResult<T>> MapIfFailWithExceptionAsync<T>(this MlResult<T>              source,
                                                                          Func<Exception, Task<T>> funcExceptionAsync)
    => await source.MatchAsync(
                        failAsync : errorsDetails => errorsDetails.GetDetailExceptionAsync().MatchAsync(
                                        failAsync :       _  =>              errorsDetails.ToMlResultFailAsync<T>(),
                                        validAsync: async ex => await (await funcExceptionAsync(ex)).ToMlResultValidAsync<T>()
                                    ),
                        validAsync: value         => value.ToMlResultValidAsync()
                    );
```

> ⚠️ **Particularidad real del código fuente:** en el fichero hay un bloque de sobrecargas
> **comentadas** de `TryMapIfFailWithExceptionAsync<T, TReturn>` con origen `Task<MlResult<T>>`.
> Si el compilador te dice que no encuentra la combinación exacta que buscas para la Forma B
> asíncrona, no es tu culpa: espera el `Task` con `await` y llama a la versión síncrona.
>
> ```csharp
> // ✅ Alternativa cuando falta la sobrecarga
> var previo = await ObtenerAsync(id);
> var r = previo.TryMapIfFailWithException(funcValid, funcFail, ex => $"…{ex.Message}");
> ```

---

## `TryMapIfFailWithException` — cuando la recuperación puede lanzar

Si tu plan B también puede reventar (leer una caché en disco, llamar a un servicio de
respaldo…), usa la variante protegida:

```csharp
public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>             source,
                                                            Func<Exception, T>      funcException,
                                                            Func<Exception, string> errorMessageBuilder)
    => source.Match(
                        fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                                    fail : _  => errorsDetails.ToMlResultFail<T>(),
                                                    valid: ex => funcException.TryToMlResult(ex, errorMessageBuilder)
                                                                              .MergeErrorsDetailsIfFail(source)
                                                ),
                        valid: value         => value
                    );

public static MlResult<T> TryMapIfFailWithException<T>(this MlResult<T>        source,
                                                            Func<Exception, T> funcException,
                                                            string             errorMessage = null!)
    => source.TryMapIfFailWithException(funcException, _ => errorMessage!);
```

🔑 **Detalle excelente y exclusivo de esta familia:** la llamada a
`.MergeErrorsDetailsIfFail(source)` **fusiona los errores originales** con el error de la
excepción del plan B. Es decir, si la recuperación falla, el resultado conserva **ambas
historias**: por qué falló el intento principal y por qué falló el respaldo.

```csharp
var r = ConsultarTarifaRemota(sku)                       // falla: HttpRequestException
            .TryMapIfFailWithException(_ => LeerTarifaDeDisco(sku),   // falla: IOException
                                       ex => $"La caché en disco tampoco respondió: {ex.Message}");

// r.Match(fail: e => e.ToErrorsDescription(), valid: …) muestra los DOS problemas.
```

Compáralo con `TryMapIfFail`, que en la misma situación **pierde el error original**. Si
necesitas trazabilidad completa de una cadena de respaldos, esta es la familia adecuada.

---

## Tabla de decisión rápida

| Situación | Método |
|---|---|
| Recuperarme de cualquier excepción, mismo tipo | `MapIfFailWithException<T>` |
| Proyectar éxito y excepción a un tipo común | `MapIfFailWithException<T, TReturn>` |
| Reaccionar solo a `SqlException`, `TimeoutException`… | `MapIfFailWithException<T, TException>` |
| Igual que la anterior, cambiando de tipo | `MapIfFailWithException<T, TReturn, TException>` |
| Necesito los mensajes **y** la excepción | `MapIfFailWithExceptionError<…>` |
| La recuperación puede lanzar | `Try…` (conserva el error original vía `MergeErrorsDetailsIfFail`) |
| Quiero recuperarme de fallos **de negocio** | `MapIfFailWithoutException` |
| Solo quiero registrar, no recuperar | `ExecSelfIfFailWithException` |

---

## Ejemplos Prácticos

### Ejemplo 1: Caché de respaldo solo ante fallos de infraestructura

El catálogo consulta tarifas a un servicio externo. Si el servicio se cae, servimos la última
tarifa conocida; pero si el SKU simplemente no existe, eso es un error de negocio y debe
llegar al usuario.

```csharp
public class ServicioTarifas
{
    public MlResult<Tarifa> Obtener(string sku)
        => ValidarSku(sku)                                        // negocio: puede fallar sin excepción
               .Bind(ConsultarServicioRemoto)                     // técnico: Try* interno
               .ExecSelfIfFail(e => _log.LogWarning("Tarifa {Sku}: {Detalle}", sku, e.ToErrorsDescription()))
               .MapIfFailWithException(ex => _cache.UltimaConocida(sku) with
                                             {
                                                 EsFiable    = false,
                                                 MotivoCache = ex.GetType().Name
                                             });

    private MlResult<string> ValidarSku(string sku)
        => EnsureFp.NotNullEmptyOrWhitespace(sku, "El SKU es obligatorio");

    private MlResult<Tarifa> ConsultarServicioRemoto(string sku)
        => EnsureFp.That(sku, _catalogo.Existe(sku), $"El SKU '{sku}' no está catalogado")
                   .TryMap(s => _http.GetTarifa(s),               // aquí nace la excepción
                           ex => $"El servicio de tarifas no respondió: {ex.Message}");
}
```

Comportamiento resultante:

| Entrada | Camino | Salida |
|---|---|---|
| SKU vacío | error de negocio, sin excepción | **fallo**: «El SKU es obligatorio» |
| SKU no catalogado | error de negocio, sin excepción | **fallo**: «El SKU 'X' no está catalogado» |
| Servicio caído | `HttpRequestException` en `Details["Ex"]` | **válido**: tarifa de caché, `EsFiable = false` |

### Ejemplo 2: Distinguir 400 de 503 en un controlador con la Forma B

```csharp
[HttpGet("{pedidoId:int}")]
public async Task<IActionResult> Obtener(int pedidoId)
{
    var respuesta = await _pedidos.ObtenerAsync(pedidoId)
        .MapIfFailWithExceptionAsync<Pedido, IActionResult>(
            funcValidAsync: p  => Task.FromResult<IActionResult>(Ok(PedidoVm.De(p))),
            funcFailAsync : ex => Task.FromResult<IActionResult>(
                                      StatusCode(503, new
                                      {
                                          mensaje = "Servicio temporalmente no disponible",
                                          tipo    = ex.GetType().Name
                                      })));

    // Si el fallo NO traía excepción, respuesta sigue siendo un MlResult fallido:
    return respuesta.Match(valid: accion  => accion,
                           fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

Este es el patrón canónico de la Forma B: la propia operación resuelve los casos «éxito» y
«fallo técnico», y el `Match` final recoge lo único que queda, los **fallos de negocio** → 400.

### Ejemplo 3: Cadena de respaldos con trazabilidad completa

Un documento se busca primero en el almacenamiento en la nube, luego en el disco local y, si
todo falla, se quiere el informe completo de lo ocurrido.

```csharp
public MlResult<Documento> Recuperar(Guid documentoId)
    => DescargarDeNube(documentoId)                                   // puede lanzar StorageException
           .TryMapIfFailWithException<Documento, StorageException>(
                funcException      : _  => LeerDeDiscoLocal(documentoId),   // puede lanzar IOException
                errorMessageBuilder: ex => $"El respaldo local falló: {ex.Message}")
           .ExecSelfIfFail(e => _log.LogError("Documento {Id} irrecuperable. {Detalle}",
                                              documentoId, e.ToErrorsDescription()));
```

Gracias a `MergeErrorsDetailsIfFail`, el log final contiene la `StorageException` original
**y** la `IOException` del respaldo, no solo la última.

### Ejemplo 4: Qué no hacer

```csharp
// ❌ 1) Esperar que recupere errores de negocio
ValidarPedido(dto).MapIfFailWithException(ex => Pedido.Vacio);
// Un fallo de validación no lleva excepción → el delegado NUNCA se ejecuta.

// ❌ 2) Usarlo como catch universal para tapar bugs
resultado.MapIfFailWithException(ex => default!);
// Una NullReferenceException es un bug: hay que verla, no enterrarla.

// ❌ 3) Confiar en la inferencia de genéricos con TException
resultado.MapIfFailWithException<TimeoutException>(ex => …);   // no compila

// ❌ 4) Acceder a .Value para inspeccionar la excepción
if (resultado.IsFail) { var ex = resultado.ErrorsDetails.Details["Ex"]; }
```

✅ En su lugar:

```csharp
// 1) para errores de negocio, la familia complementaria
ValidarPedido(dto).MapIfFailWithoutException(errores => Pedido.Vacio);

// 2) filtra el tipo que de verdad sabes tratar
resultado.MapIfFailWithException<Pedido, TimeoutException>(_ => Pedido.Reintentable);

// 3) genéricos completos y explícitos
resultado.MapIfFailWithException<Pedido, TimeoutException>(ex => …);

// 4) lee la excepción con la API prevista
resultado.ExecSelfIfFail(e => e.GetDetailException()
                               .Match(valid: ex => _log.LogError(ex, "Fallo técnico"),
                                      fail : _  => _log.LogWarning("Fallo de negocio: {M}",
                                                                   e.ToErrorsMessages())));
```

---

## Mejores Prácticas

1. **Reserva esta familia para los fallos técnicos.** Es su razón de ser: si el error lo
   generó tu lógica de negocio, no habrá excepción y la operación será transparente.
2. **Filtra por `TException` siempre que sepas qué tratar.** Recuperarse de «cualquier
   excepción» suele esconder un `catch (Exception)` disfrazado.
3. **Escribe los genéricos completos** en las formas C y D (`<T, TException>`,
   `<T, TReturn, TException>`); la inferencia no funciona.
4. **Registra antes de recuperar** con `ExecSelfIfFail`: una vez recuperado, el resultado es
   válido y nadie sabrá que hubo un incidente.
5. **Marca el valor degradado** (`EsFiable = false`, `MotivoCache`, un aviso) para que el
   consumidor pueda distinguir un dato de primera de uno de respaldo.
6. **Usa `Try…` en cadenas de respaldo** y aprovecha que fusiona los errores: tendrás el
   histórico completo del fallo en un único `ToErrorsDescription()`.
7. **Usa la subfamilia `…Error`** cuando la recuperación necesite los mensajes de negocio
   además de la excepción.
8. **No la uses para bugs.** `NullReferenceException`, `IndexOutOfRangeException` y
   compañía deben propagarse y verse, no reciclarse en un valor por defecto.
9. **Colócala cerca del borde de la tubería**, cuando ya sabes qué respuesta quieres dar; si
   recuperas demasiado pronto, los pasos siguientes trabajarán con datos de respaldo sin
   saberlo.

---

## Resumen

- `MapIfFailWithException` recupera **solo si el fallo trae una excepción** en
  `Details["Ex"]`; si no la trae, devuelve el fallo **intacto** (a diferencia de otras
  familias, no añade ni sustituye errores).
- Hay **cuatro formas**: `<T>`, `<T, TReturn>`, `<T, TException>` y `<T, TReturn, TException>`;
  las dos últimas filtran por tipo de excepción con `where TException : Exception`.
- Existe una **subfamilia `MapIfFailWithExceptionError`** con las mismas cuatro formas, cuyo
  delegado recibe el `MlErrorsDetails` completo en lugar de la excepción; la excepción sigue
  siendo la condición de activación, pero se descarta como parámetro.
- Como el delegado devuelve un valor desnudo, si hay excepción **la salida es siempre válida**.
- `TryMapIfFailWithException` protege la recuperación y, mediante
  `MergeErrorsDetailsIfFail(source)`, **conserva los errores originales**: ideal para cadenas
  de respaldo con trazabilidad.
- Todas las formas tienen variantes asíncronas; algunas sobrecargas de la Forma B asíncrona
  con origen `Task<MlResult<T>>` están comentadas en el fuente: resuelve el `Task` con `await`.
- La familia complementaria es `MapIfFailWithoutException`, que actúa exactamente en el caso
  opuesto.

---

## Ver también

- [`7_MapIfFailWithoutException.md`](7_MapIfFailWithoutException.md) — la operación espejo: recupera solo si **no** hay excepción.
- [`4_MapIfFail.md`](4_MapIfFail.md) — recuperación incondicional con los errores en la mano.
- [`1_Map.md`](1_Map.md) — la operación base de la familia.
- [`8_MapAlways.md`](8_MapAlways.md) — cuando quieres actuar pase lo que pase.
- [`../Bind/8_BindIfFailWithException.md`](../Bind/8_BindIfFailWithException.md) — la versión cuya recuperación sí puede volver a fallar.
- [`../Bind/9_BindIfFailWithoutException.md`](../Bind/9_BindIfFailWithoutException.md) — su espejo en la familia `Bind`.
- [`../ExecSelf/5_ExecSelfIfFailWithException.md`](../ExecSelf/5_ExecSelfIfFailWithException.md) — registrar sin recuperar.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException`, `GetDetailException<TException>`, `MergeErrorsDetailsIfFail`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — estructura real de `MlErrorsDetails` y la clave `Ex`.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la familia `Map`.