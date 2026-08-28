# BindIfFail — Recuperarse de un fallo

## Índice
1. [Introducción](#introducción)
2. [Las dos formas de `BindIfFail`](#las-dos-formas-de-bindiffail)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [`TryBindIfFail` — cuando la recuperación puede lanzar](#trybindiffail--cuando-la-recuperación-puede-lanzar)
6. [Recuperar, transformar o simplemente informar](#recuperar-transformar-o-simplemente-informar)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

Todos los `Bind` de la librería trabajan sobre el **camino válido** y dejan pasar los fallos intactos.
`BindIfFail` es el espejo: actúa **solo si el resultado es fallido** y le da una oportunidad de
**volver al camino válido**.

Es la herramienta de los *fallbacks*: caché caída → base de datos, servicio principal caído → servicio
secundario, configuración ausente → valores por defecto.

```csharp
// ❌ Con try/catch anidados el fallback ensucia todo.
Tarifa tarifa;
try
{
    tarifa = _cache.ObtenerTarifa(id);
}
catch
{
    try   { tarifa = _bd.ObtenerTarifa(id); }
    catch { tarifa = Tarifa.PorDefecto; }
}

// ✅ Con BindIfFail la cascada de alternativas es una tubería lineal.
var tarifa = ObtenerDeCache(id)
    .BindIfFail(_ => ObtenerDeBaseDeDatos(id))
    .BindIfFail(_ => MlResult<Tarifa>.Valid(Tarifa.PorDefecto));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## Las dos formas de `BindIfFail`

| Forma | Firma | Qué hace si el resultado es **válido** |
| --- | --- | --- |
| **A — Solo recuperación** | `BindIfFail<T>(func)` | **Nada**: devuelve el valor tal cual |
| **B — Ambos caminos** | `BindIfFail<T, TReturn>(funcValid, funcFail)` | Ejecuta `funcValid` |

La forma **A** conserva el tipo (`MlResult<T>` → `MlResult<T>`), porque debe poder devolver el valor
original sin tocarlo. La forma **B** permite cambiar de tipo, ya que ambos caminos están cubiertos.

```csharp
// Forma A: intentar otra cosa solo si ha fallado.
MlResult<Tarifa> t = ObtenerDeCache(id)
    .BindIfFail(errores => ObtenerDeBaseDeDatos(id));

// Forma B: los dos caminos convergen en un tipo nuevo.
MlResult<RespuestaApi> r = ObtenerCliente(id)
    .BindIfFail(funcValid: c       => RespuestaApi.Ok(c),
                funcFail : errores => RespuestaApi.Error(errores.ToErrorsMessages()));
```

📌 La forma B es exactamente [`Match`](../Match/1_Match.md) pero devolviendo `MlResult<TReturn>` en las
dos ramas. Si una de tus ramas no puede fallar, `Match` suele leerse mejor.

---

## Firmas reales

### Forma A

```csharp
public static MlResult<T> BindIfFail<T>(this MlResult<T>                        source,
                                             Func<MlErrorsDetails, MlResult<T>> func)
    => source.Match(
        fail : func,
        valid: value => value);          // ← conversión implícita T → MlResult<T>
```

### Forma B

```csharp
public static MlResult<TReturn> BindIfFail<T, TReturn>(this MlResult<T>                              source,
                                                            Func<T              , MlResult<TReturn>> funcValid,
                                                            Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
    => source.Match(
        fail : funcFail,
        valid: funcValid);
```

| Estado de entrada | Forma A | Forma B |
| --- | --- | --- |
| Válido | Devuelve el valor sin cambios | `funcValid(valor)` |
| Fallido | `func(errorsDetails)` | `funcFail(errorsDetails)` |

🔑 `func`/`funcFail` reciben el **`MlErrorsDetails` completo**, no un simple mensaje. Eso te permite
decidir la estrategia de recuperación en función de *qué* falló: leer la excepción con
`GetDetailException()`, recuperar el valor original con `GetDetailValue<T>()` o inspeccionar los
mensajes.

📌 A diferencia de `Bind`, `BindIfFail` **puede recuperar** un fallo… y también **puede empeorarlo**:
si `func` devuelve otro fallo, ese es el que se propaga (el original se pierde, salvo que lo fusiones
explícitamente con `MergeErrorsDetailsIfFail`).

---

## Variantes asíncronas

`BindIfFailAsync` suma **12 sobrecargas**, combinando:

| Eje | Opciones |
| --- | --- |
| Fuente | `MlResult<T>` · `Task<MlResult<T>>` |
| Delegado de recuperación | síncrono · asíncrono |
| Forma | A (solo `func`) · B (`funcValid` + `funcFail`) |

```csharp
public Task<MlResult<Cotizacion>> ObtenerCotizacionAsync(string simbolo)
    => ConsultarProveedorPrincipalAsync(simbolo)
        .BindIfFailAsync(errores => ConsultarProveedorRespaldoAsync(simbolo))
        .BindIfFailAsync(errores => LeerUltimoCierreAsync(simbolo));
```

Internamente todas se apoyan en `MatchAsync`, y las que reciben un delegado síncrono lo adaptan con
`func.ToFuncTask()`.

---

## `TryBindIfFail` — cuando la recuperación puede lanzar

El *fallback* casi siempre es I/O: otra base de datos, un fichero, otra API. Si puede lanzar
excepciones, usa `TryBindIfFail`, que las convierte en un fallo y guarda la excepción en
`Details["Ex"]`.

```csharp
public static MlResult<T> TryBindIfFail<T>(this MlResult<T>                        source,
                                                Func<MlErrorsDetails, MlResult<T>> func,
                                                Func<Exception, string>            errorMessageBuilder);

// Sobrecarga equivalente con mensaje fijo:
public static MlResult<T> TryBindIfFail<T>(this MlResult<T>                        source,
                                                Func<MlErrorsDetails, MlResult<T>> func,
                                                string                             exceptionAditionalMessage);
```

Sobrecargas disponibles: **`TryBindIfFail` (4)** y **`TryBindIfFailAsync` (24)**, cubriendo también la
forma B con `funcValid`/`funcFail`.

```csharp
var configuracion = LeerDeVariablesDeEntorno()
    .TryBindIfFail(
        errores => LeerDeFicheroLocal("appsettings.local.json"),   // puede lanzar IOException
        ex      => $"El fallback a fichero local también falló: {ex.Message}");
```

| Método | Excepción en el *fallback* | Cuándo usarlo |
| --- | --- | --- |
| `BindIfFail` | **Se propaga** y rompe la tubería | El *fallback* es código propio sin riesgo |
| `TryBindIfFail` | Se convierte en `MlResult` fallido | El *fallback* hace I/O o usa terceros |

---

## Recuperar, transformar o simplemente informar

Es fácil confundir las herramientas que «actúan sobre el fallo». Esta tabla las separa:

| Lo que quieres hacer | Herramienta | ¿Puede volver al camino válido? |
| --- | --- | --- |
| Intentar una alternativa que también puede fallar | **`BindIfFail`** | **Sí** |
| Devolver un valor por defecto (no puede fallar) | [`MapIfFail`](../Map/4_MapIfFail.md) | Sí |
| Solo registrar/notificar el fallo, sin cambiarlo | [`ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md) | No |
| Añadir contexto al mensaje de error | `AddMlErrorDetailIfFail` | No |
| Salir de `MlResult` hacia un tipo cualquiera | [`Match`](../Match/1_Match.md) | — |
| Recuperarse usando el **valor original** que falló | [`7_BindIfFailWithValue.md`](./7_BindIfFailWithValue.md) | Sí |
| Recuperarse solo si hubo **excepción** | [`8_BindIfFailWithException.md`](./8_BindIfFailWithException.md) | Sí |
| Recuperarse solo si **no** hubo excepción | [`9_BindIfFailWithoutException.md`](./9_BindIfFailWithoutException.md) | Sí |

💡 Regla rápida: si tu *fallback* **no puede fallar**, `MapIfFail` es más simple. Si puede fallar,
`BindIfFail`.

---

## Ejemplos Prácticos

### Ejemplo 1: Cascada de orígenes de datos

```csharp
public class ServicioTarifas
{
    public MlResult<Tarifa> Obtener(string codigo)
        => LeerDeCache(codigo)

            // 1er fallback: la base de datos (puede fallar por red).
            .BindIfFail(errores =>
            {
                _log.LogDebug("Caché no disponible para {Codigo}: {Motivo}",
                              codigo, errores.Errors.First().Message);
                return LeerDeBaseDeDatos(codigo);
            })

            // 2º fallback: el fichero de tarifas embebido (puede lanzar).
            .TryBindIfFail(errores => LeerDeFicheroEmbebido(codigo),
                           ex => $"Ningún origen pudo servir la tarifa '{codigo}': {ex.Message}")

            .AddMlErrorDetailIfFail($"[Tarifas] Código solicitado: {codigo}");
}
```

Cada `BindIfFail` solo se ejecuta si el anterior falló. Si la caché responde, **no se toca la base de
datos ni el fichero**.

### Ejemplo 2: No perder el error original al fusionar

Por defecto, si el *fallback* también falla te quedas solo con **su** error. Cuando quieras conservar
ambos, fusiónalos:

```csharp
public MlResult<Documento> Cargar(string id)
    => LeerDelRepositorioPrimario(id)
        .BindIfFail(erroresPrimario =>
            LeerDelRepositorioSecundario(id)
                // Si el secundario también falla, añade los errores del primario.
                .MergeErrorsDetailsIfFail(erroresPrimario));
```

Resultado: un único fallo que contiene el motivo de **los dos** repositorios, lo que hace el diagnóstico
muchísimo más rápido.

### Ejemplo 3: Convertir cualquier resultado en una respuesta HTTP (forma B)

```csharp
[HttpGet("pedidos/{id}")]
public async Task<IActionResult> Obtener(int id)
{
    var respuesta = await _servicio.ObtenerAsync(id)
        .BindIfFailAsync(
            funcValidAsync: async pedido  => await EnriquecerConEnvioAsync(pedido),
            funcFailAsync : async errores => await RegistrarYConstruirErrorAsync(errores));

    return respuesta.Match<RespuestaPedido, IActionResult>(
        valid: r       => Ok(r),
        fail : errores => StatusCode(500, errores.ToErrorsMessages()));
}

private async Task<MlResult<RespuestaPedido>> RegistrarYConstruirErrorAsync(MlErrorsDetails errores)
{
    await _auditoria.RegistrarAsync(errores.ToErrorsDescription());

    // Un pedido inexistente no es un error del sistema: devolvemos una respuesta "vacía" válida.
    return errores.Errors.Any(e => e.Message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase))
                ? RespuestaPedido.NoEncontrado(errores.ToErrorsMessages())
                : MlResult<RespuestaPedido>.Fail(errores);
}
```

Observa el patrón: la forma B permite **decidir caso por caso** si el fallo se recupera (respuesta
«no encontrado» válida) o se mantiene (error real del sistema).

### Ejemplo 4: Reintento con estrategia según el tipo de fallo

`func` recibe los detalles completos, así que puedes reintentar **solo** ante fallos técnicos:

```csharp
public async Task<MlResult<Pago>> CobrarAsync(Pago pago)
    => await IntentarCobroAsync(pago)
        .BindIfFailAsync(async errores =>
            // ¿Hubo excepción? → fallo técnico transitorio → merece un reintento.
            await errores.GetDetailException()
                .MatchAsync(
                    validAsync: async ex =>
                    {
                        _log.LogWarning(ex, "Cobro {Id}: fallo técnico, reintentando", pago.Id);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        return await IntentarCobroAsync(pago);
                    },
                    // Sin excepción → rechazo de negocio (fondos insuficientes): no reintentar.
                    failAsync: _ => MlResult<Pago>.FailAsync(errores)));
```

**Clave:** distinguir fallo técnico de rechazo de negocio mediante `GetDetailException()` evita
reintentos inútiles y cargos duplicados.

---

## Mejores Prácticas

### 1. Un *fallback* debe ser realmente equivalente

`BindIfFail` es para orígenes alternativos del **mismo** dato. Si la alternativa devuelve algo distinto
en significado, no la esconds tras un *fallback*: hazla explícita en el flujo.

### 2. Registra por qué se activó el *fallback*

Un *fallback* silencioso oculta averías: el sistema «funciona» mientras la caché lleva días caída.
Aprovecha que recibes `MlErrorsDetails` para dejar traza, o añade un `ExecSelfIfFail` antes.

### 3. Fusiona los errores si el *fallback* también falla

Sin `MergeErrorsDetailsIfFail` pierdes el motivo del intento original y solo verás el último. Ver
ejemplo 2.

### 4. No reintentes fallos de negocio

Reintentar «saldo insuficiente» es inútil; reintentar un *timeout* tiene sentido. Distingue con
`GetDetailException()` o usa directamente
[`BindIfFailWithException`](./8_BindIfFailWithException.md) y
[`BindIfFailWithoutException`](./9_BindIfFailWithoutException.md), que ya filtran por ti.

### 5. Si el *fallback* no puede fallar, usa `MapIfFail`

```csharp
// ❌ Envolver a mano un valor que nunca falla.
.BindIfFail(_ => MlResult<Tarifa>.Valid(Tarifa.PorDefecto))

// ✅ Más directo y más claro.
.MapIfFail(_ => Tarifa.PorDefecto)
```

### 6. Limita la cascada

Tres *fallbacks* encadenados ya son un olor a diseño: probablemente necesitas una estrategia explícita
(patrón *chain of responsibility* o una lista de proveedores recorrida con `Projection`).

---

## Resumen

- `BindIfFail` es el espejo de `Bind`: actúa **solo sobre el camino fallido** y puede devolverlo al
  camino válido.
- **Forma A** (`func`): mismo tipo, no toca el valor si es válido. **Forma B** (`funcValid` +
  `funcFail`): cubre ambos caminos y permite cambiar de tipo.
- El delegado recibe el `MlErrorsDetails` **completo**, lo que permite decidir la estrategia según el
  motivo del fallo.
- Si el *fallback* también falla, su error **sustituye** al original; usa `MergeErrorsDetailsIfFail`
  para conservar los dos.
- Sobrecargas: `BindIfFail` (2), `BindIfFailAsync` (12), `TryBindIfFail` (4), `TryBindIfFailAsync` (24).
- **Fallback que puede fallar → `BindIfFail`. Valor por defecto seguro → `MapIfFail`. Solo observar →
  `ExecSelfIfFail`.**

## Ver también

- [`3_Bind.md`](./3_Bind.md) — el encadenamiento sobre el camino válido.
- [`5_BindIf.md`](./5_BindIf.md) — bifurcación condicional sobre el valor.
- [`7_BindIfFailWithValue.md`](./7_BindIfFailWithValue.md) — recuperación usando el valor que provocó el fallo.
- [`8_BindIfFailWithException.md`](./8_BindIfFailWithException.md) — recuperación solo ante excepciones.
- [`9_BindIfFailWithoutException.md`](./9_BindIfFailWithoutException.md) — recuperación solo ante fallos de negocio.
- [`10_BindAlways.md`](./10_BindAlways.md) — ejecutar algo con independencia del estado.
- [`../Map/4_MapIfFail.md`](../Map/4_MapIfFail.md) — valor por defecto sin posibilidad de fallo.
- [`../ExecSelf/3_ExecSelfIfFail.md`](../ExecSelf/3_ExecSelfIfFail.md) — observar el fallo sin alterarlo.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException`, `MergeErrorsDetailsIfFail` y compañía.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir de `MlResult` hacia cualquier tipo.