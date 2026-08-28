# BindSaveValueInDetailsIfFaildFuncResult — Guardar la entrada cuando la función falla

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [Firma real e implementación](#firma-real-e-implementación)
4. [Cómo se guarda el valor: la clave `Value`](#cómo-se-guarda-el-valor-la-clave-value)
5. [⚠️ Particularidad real del código fuente: la mutación](#️-particularidad-real-del-código-fuente-la-mutación)
6. [Cómo recuperar el valor guardado](#cómo-recuperar-el-valor-guardado)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [`TryBindSaveValueInDetailsIfFaildFuncResult`](#trybindsavevalueindetailsiffaildfuncresult)
9. [Comparación con `AddValueIfFail` manual](#comparación-con-addvalueiffail-manual)
10. [Ejemplos Prácticos](#ejemplos-prácticos)
11. [Mejores Prácticas](#mejores-prácticas)
12. [Resumen](#resumen)
13. [Ver también](#ver-también)

---

## Introducción

El nombre es largo y algo enrevesado (incluye incluso una errata de origen: *Faild* en vez de *Failed*), pero la idea es muy sencilla y muy útil:

> **Ejecuta la función si el resultado es válido y, si esa función falla, guarda automáticamente el valor de entrada dentro de los detalles del error.**

Es un `Bind` normal con una red de seguridad: cuando algo se rompe, no pierdes el dato que provocó la rotura.

```csharp
// ❌ Bind normal: si Facturar falla, el pedido se ha perdido.
//    El error dice "importe inválido"… ¿de qué pedido?
MlResult<Factura> r = ObtenerPedido(id)
                        .Bind(pedido => Facturar(pedido));

// ✅ El pedido queda guardado en los detalles del fallo
MlResult<Factura> r = ObtenerPedido(id)
                        .BindSaveValueInDetailsIfFaildFuncResult(pedido => Facturar(pedido));
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`; para llegar al valor guardado, `GetDetailValue<T>()`.

---

## El problema que resuelve

En una tubería *railway*, el valor que entra en un paso **desaparece** en cuanto ese paso falla: el fallo solo lleva mensajes de error. Eso convierte muchos diagnósticos en un ejercicio de adivinación.

```
ObtenerPedido(4711)  ──►  Pedido{4711, 12 líneas, 830 €}
                            │
                            ▼  Facturar(pedido)  ✗ "El IVA no está configurado"
                            │
                            ▼
                     Fail{ "El IVA no está configurado" }      ← ¿y el pedido?
```

Con este método, el fallo lleva el pedido dentro:

```
                     Fail{ Errors : "El IVA no está configurado",
                           Details: { "Value": Pedido{4711, …} } }
```

Y eso te permite dos cosas que antes eran imposibles:

1. **Registrar un diagnóstico completo** sin volver a leer nada de la base de datos.
2. **Reintentar o compensar** con el mismo dato, usando la familia [`BindIfFailWithValue`](7_BindIfFailWithValue.md), que precisamente necesita que el valor esté ahí.

---

## Firma real e implementación

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
                                        this MlResult<T>                source,
                                             Func<T, MlResult<TReturn>> func)
    => source.Match
    (
        fail : errorDetails => errorDetails,
        valid: value =>
        {
            var result = func(value);

            if (result.IsFail) result.AddValueDetailIfFail(value!);

            return result;
        }
    );
```

Se lee de un tirón:

| Estado de `source` | Resultado de `func(value)` | Qué devuelve |
|---|---|---|
| Fallido | *no se ejecuta* | El fallo original, **sin añadir nada** |
| Válido | Válido | El resultado válido de `func` |
| Válido | Fallido | El fallo de `func`, **con `value` en `Details["Value"]`** |

> 📌 Observa la primera fila: si el resultado **ya venía fallido**, este método no aporta nada respecto a un `Bind` normal. Solo actúa cuando el fallo lo produce `func`.

---

## Cómo se guarda el valor: la clave `Value`

La cadena de llamadas es la siguiente:

```csharp
// MlResultActions.cs
public static MlResult<T> AddValueDetailIfFail<T>(this MlResult<T> source, object errorValue)
    => source.AddMlErrorDetailIfFail(VALUE_KEY, errorValue);

public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, string errorKey, object errorValue)
    => source.Match(
            fail : errorsDetails => errorsDetails.AddDetail(errorKey, errorValue),
            valid: _            => source);
```

Donde `VALUE_KEY` es la constante `"Value"` definida en `Helpers/Constants.cs`. Es exactamente la misma clave que usan `AddValueIfFail` y `GetDetailValue<T>()`, así que las tres piezas encajan sin configuración.

```csharp
// Constants.cs
public const string VALUE_KEY = "Value";
public const string EX_DESC_KEY = "Ex";     // la clave que usa la familia WithException
```

---

## ⚠️ Particularidad real del código fuente: la mutación

Aquí hay un detalle de implementación que conviene conocer, porque explica un comportamiento sorprendente. Mira otra vez esta línea:

```csharp
if (result.IsFail) result.AddValueDetailIfFail(value!);   // ⚠️ el retorno se descarta

return result;                                            // …y aun así funciona
```

`AddValueDetailIfFail` **devuelve** un `MlResult<TReturn>` nuevo, pero aquí ese retorno se ignora y se devuelve `result`. ¿Por qué funciona entonces? Porque `AddDetail` no crea un diccionario nuevo, **muta el existente**:

```csharp
public static MlErrorsDetails AddDetail<T>(this MlErrorsDetails source, string key, T value)
{
    source.Details.Add(key, value!);        // ⚠️ mutación in-place del diccionario

    var result = (source.Errors, source.Details);

    return result;
}
```

Como `result` y el valor devuelto comparten el mismo `Dictionary<string, object>`, el detalle aparece en ambos. Dos consecuencias prácticas:

1. **Funciona, pero por efecto colateral.** No es un patrón que debas imitar en tu propio código: si escribes tus propios ayudantes, reasigna siempre el retorno.

2. **`Dictionary.Add` lanza si la clave ya existe.** Si el fallo que devuelve `func` ya trae un detalle `"Value"` (por ejemplo, porque internamente ya usó `AddValueIfFail` o otro `BindSaveValueInDetailsIfFaildFuncResult`), la llamada lanzará `ArgumentException`. **No apiles dos de estos métodos sobre el mismo fallo.**

```csharp
// ❌ Riesgo de ArgumentException: dos capas intentan escribir Details["Value"]
ObtenerPedido(id)
    .BindSaveValueInDetailsIfFaildFuncResult(p => FacturarConGuardado(p));  // y este ya guarda dentro

// ✅ Un solo punto de guardado, en la capa que va a consumir el valor
ObtenerPedido(id)
    .BindSaveValueInDetailsIfFaildFuncResult(p => Facturar(p));
```

---

## Cómo recuperar el valor guardado

Tienes dos vías, según lo que quieras hacer.

**Vía 1 — leerlo para diagnosticar**, con `GetDetailValue<T>()`:

```csharp
resultado.ExecSelfIfFail(errores =>
    errores.GetDetailValue<Pedido>()
           .Match(
               valid: pedido => _log.LogError("Fallo facturando el pedido {Id} de {Importe} €: {E}",
                                              pedido.Id, pedido.Importe, errores.ToErrorsMessages()),
               fail : _      => _log.LogError("Fallo facturando (sin dato de entrada): {E}",
                                              errores.ToErrorsMessages())));
```

**Vía 2 — usarlo para recuperarse**, con la familia `BindIfFailWithValue`, que ya sabe extraerlo:

```csharp
MlResult<Factura> r = ObtenerPedido(id)
    .BindSaveValueInDetailsIfFaildFuncResult(pedido => FacturarEnLinea(pedido))
    .BindIfFailWithValue<Pedido, Factura>(pedido => EncolarFacturacionDiferida(pedido));
```

Este es el **encaje natural** de los dos métodos: uno guarda el valor al fallar, el otro lo recoge para intentar un plan B. Sin el primero, el segundo no encontraría nada.

---

## Variantes asíncronas

| Origen | Delegado | Método |
|---|---|---|
| `MlResult<T>` | `Func<T, Task<MlResult<TReturn>>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `Func<T, Task<MlResult<TReturn>>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| `Task<MlResult<T>>` | `Func<T, MlResult<TReturn>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |

La versión con origen síncrono y delegado asíncrono es la única con cuerpo propio; las otras dos delegan en ella (la tercera adaptando el delegado con `func.ToFuncTask()`).

```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
                                        this MlResult<T>                      source,
                                             Func<T, Task<MlResult<TReturn>>> funcAsync)
    => await source.MatchAsync
            (
                failAsync : errorsDetails => MlResult<TReturn>.FailAsync(errorsDetails),
                validAsync: async value =>
                {
                    var result = await funcAsync(value);

                    if (result.IsFail) result.AddValueDetailIfFail(value!);

                    return result;
                }
            );
```

> 💡 El nombre del archivo de documentación termina en `Async` por motivos históricos, pero **la versión síncrona existe y es la principal**. No necesitas trabajar en asíncrono para usar esta función.

---

## `TryBindSaveValueInDetailsIfFaildFuncResult`

Como el resto de la biblioteca, hay variantes `Try*` que capturan las excepciones de `func`:

```csharp
public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
                                        this MlResult<T>                source,
                                             Func<T, MlResult<TReturn>> func,
                                             Func<Exception, string>    errorMessageBuilder);

public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
                                        this MlResult<T>                source,
                                             Func<T, MlResult<TReturn>> func,
                                             string                     errorMessage = null!);
```

La combinación es especialmente potente: el fallo resultante lleva **las dos cosas**, la excepción en `Details["Ex"]` y el valor de entrada en `Details["Value"]`.

```csharp
var r = ObtenerPedido(id)
            .TryBindSaveValueInDetailsIfFaildFuncResult(
                pedido => _pasarela.Cobrar(pedido),
                ex     => $"Error de pasarela: {ex.Message}");

// Y ahora el diagnóstico es completo:
r.ExecSelfIfFail(errores =>
{
    var pedido    = errores.GetDetailValue<Pedido>();
    var excepcion = errores.GetDetailException();

    _log.LogError("Cobro fallido | Pedido: {P} | Excepción: {E} | Errores: {Er}",
                  pedido.Match(valid: p => p.Id.ToString(), fail: _ => "desconocido"),
                  excepcion.Match(valid: e => e.GetType().Name, fail: _ => "ninguna"),
                  errores.ToErrorsMessages());
});
```

Hay `TryBindSaveValueInDetailsIfFaildFuncResultAsync` con 8 sobrecargas, combinando origen y delegado síncrono/asíncrono con las dos formas de construir el mensaje de error.

---

## Comparación con `AddValueIfFail` manual

Puedes conseguir el mismo efecto a mano, y es útil entender la diferencia:

```csharp
// A) Con este método: el valor guardado es el de ENTRADA del paso
ObtenerPedido(id)
    .BindSaveValueInDetailsIfFaildFuncResult(pedido => Facturar(pedido));
//   → Details["Value"] = el Pedido

// B) Con AddValueIfFail: guardas lo que tú decidas, cuando tú decidas
ObtenerPedido(id)
    .Bind(pedido => Facturar(pedido).AddValueIfFail(pedido));
//   → equivalente, pero explícito

// C) Con AddValueIfFail sobre otro dato
ObtenerPedido(id)
    .Bind(pedido => Facturar(pedido).AddValueIfFail(new { pedido.Id, Intento = 2 }));
//   → guardas un contexto enriquecido en lugar del valor crudo
```

| | `BindSaveValueInDetailsIfFaildFuncResult` | `Bind` + `AddValueIfFail` |
|---|---|---|
| Qué guarda | Siempre el valor de entrada | Lo que tú le pases |
| Verbosidad | Menor | Mayor, pero explícita |
| Riesgo de clave duplicada | Sí (usa `Dictionary.Add`) | El mismo |
| Cuándo elegirla | El caso estándar: guardar la entrada tal cual | Cuando quieras guardar un contexto distinto o enriquecido |

---

## Ejemplos Prácticos

### Ejemplo 1: Importación con fila de rechazos

Al importar un fichero, cada línea que falla debe acabar en un informe de rechazos **con la línea original**, no solo con el mensaje.

```csharp
public record Rechazo(int NumeroLinea, string Contenido, IEnumerable<string> Motivos);

public MlResult<InformeImportacion> Importar(IEnumerable<LineaFichero> lineas)
{
    var rechazos = new List<Rechazo>();
    var altas    = new List<Cliente>();

    foreach (var linea in lineas)
    {
        linea.ToMlResultValid()
             .TryBindSaveValueInDetailsIfFaildFuncResult(
                 l  => CrearCliente(l),
                 ex => $"Excepción procesando la línea {linea.Numero}: {ex.Message}")

             .Match(
                 valid: cliente => altas.Add(cliente),
                 fail : errores =>
                 {
                     // El valor de entrada está garantizado en los detalles
                     var original = errores.GetDetailValue<LineaFichero>()
                                           .Match(valid: l => l, fail: _ => linea);

                     rechazos.Add(new Rechazo(original.Numero,
                                              original.Texto,
                                              errores.ToErrorsMessages()));
                     return 0;
                 });
    }

    return new InformeImportacion(altas, rechazos);
}
```

### Ejemplo 2: Plan B con el mismo dato (el encaje con `BindIfFailWithValue`)

```csharp
public async Task<MlResult<Confirmacion>> EnviarNotificacionAsync(NotificacionDto dto)
    => await ValidarNotificacionAsync(dto)

        // 1) Se intenta el canal preferente y, si falla, se guarda el DTO
        .BindSaveValueInDetailsIfFaildFuncResultAsync(n => _push.EnviarAsync(n))

        // 2) El plan B recibe el DTO gracias al paso anterior
        .BindIfFailWithValueAsync<NotificacionDto, Confirmacion>(
            async n => await _email.EnviarAsync(n))

        // 3) Y si también falla el correo, se registra con todo el contexto
        .ExecSelfIfFailAsync(async errores =>
            await _auditoria.RegistrarAsync(
                asunto : "Notificación no entregada por ningún canal",
                detalle: errores.ToDetailsDescription()));
```

Sin el paso 1, el paso 2 no encontraría el valor y no se ejecutaría: `BindIfFailWithValue` depende de que `Details["Value"]` exista.

### Ejemplo 3: Diagnóstico de una llamada externa

```csharp
public MlResult<RespuestaScoring> ConsultarScoring(SolicitudCredito solicitud)
    => ValidarSolicitud(solicitud)
        .TryBindSaveValueInDetailsIfFaildFuncResult(
            s  => _clienteHttp.PostScoring(s),
            ex => $"El servicio de scoring no respondió correctamente: {ex.Message}")

        .ExecSelfIfFail(errores =>
        {
            var esTecnico = errores.GetDetailException().IsValid;

            _log.Log(esTecnico ? LogLevel.Error : LogLevel.Warning,
                     "Scoring fallido | Solicitud: {S} | Tipo: {T} | {E}",
                     errores.GetDetailValue<SolicitudCredito>()
                            .Match(valid: s => $"{s.Nif}/{s.Importe}€", fail: _ => "n/d"),
                     esTecnico ? "técnico" : "negocio",
                     errores.ToErrorsMessages());
        });
```

### Ejemplo 4: Cuándo **no** usarlo

```csharp
// ❌ Innecesario: el valor de entrada es un int que ya tienes en la variable id
ObtenerCliente(id)
    .BindSaveValueInDetailsIfFaildFuncResult(c => ...);   // guarda un Cliente entero por nada

// ❌ Peligroso: guardar entidades enormes o con datos sensibles en los detalles del error,
//    que probablemente acabarán en un log
ObtenerExpediente(id)
    .BindSaveValueInDetailsIfFaildFuncResult(e => Procesar(e));   // ¿va el DNI al log?

// ✅ Guarda solo lo que necesites para diagnosticar o reintentar
ObtenerExpediente(id)
    .Bind(e => Procesar(e).AddValueIfFail(new { e.Id, e.Estado, e.Version }));
```

---

## Mejores Prácticas

1. **Úsalo justo antes del paso que puede fallar de forma opaca.** Su valor está en los pasos cuyo mensaje de error no basta para saber qué ocurrió.

2. **No lo apiles.** Como `AddDetail` usa `Dictionary.Add`, dos guardados sobre el mismo fallo lanzan `ArgumentException`. Un único punto de guardado por rama de fallo.

3. **Piensa en el par con `BindIfFailWithValue`.** Si guardas el valor, es normalmente porque alguien lo va a recoger. Si nadie lo recoge y solo quieres registrar, un `ExecSelfIfFail` con el dato en la clausura puede ser más simple.

4. **Cuidado con los datos sensibles y los objetos grandes.** Lo que entra en `Details` suele terminar en un log. Prefiere `AddValueIfFail` con una proyección reducida (`new { Id, Estado }`).

5. **Prefiere la variante `Try*` para infraestructura.** Así el fallo lleva la excepción **y** el valor, que es el diagnóstico ideal.

6. **Recuerda que no hace nada si el fallo venía de antes.** Solo captura los fallos que produce `func`.

7. **No imites su patrón de mutación.** Funciona por el efecto colateral de `AddDetail`; en tu código, reasigna siempre el retorno.

---

## Resumen

- `BindSaveValueInDetailsIfFaildFuncResult` es un `Bind` que, **si la función falla**, guarda el valor de entrada en `Details["Value"]`.
- Si el resultado **ya venía fallido**, se comporta como un `Bind` normal: no añade nada.
- La clave usada es la constante `VALUE_KEY` (`"Value"`), la misma que leen `GetDetailValue<T>()` y `BindIfFailWithValue`.
- ⚠️ Internamente funciona por **mutación** del diccionario de detalles, y `Dictionary.Add` **lanza si la clave ya existe**: no apiles dos guardados sobre el mismo fallo.
- Tiene 3 sobrecargas asíncronas y 4 + 8 variantes `Try*` que además dejan la excepción en `Details["Ex"]`.
- Su compañero natural es [`BindIfFailWithValue`](7_BindIfFailWithValue.md), que recoge el valor guardado para intentar un plan B.
- El nombre contiene una errata de origen (*Faild*) que se mantiene por compatibilidad.

---

## Ver también

- [`3_Bind.md`](3_Bind.md) — el `Bind` del que este método es una variante.
- [`7_BindIfFailWithValue.md`](7_BindIfFailWithValue.md) — recoger el valor guardado para recuperarse.
- [`8_BindIfFailWithException.md`](8_BindIfFailWithException.md) — recuperarse según la excepción de `Details["Ex"]`.
- [`10_BindAlways.md`](10_BindAlways.md) — el punto de convergencia del *pipeline*.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `AddValueIfFail`, `GetDetailValue<T>`, `GetDetailException`.
- [`../Types/MlResultActions.md`](../Types/MlResultActions.md) — `AddValueDetailIfFail`, `AddMlErrorDetailIfFail`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — la estructura real de `MlErrorsDetails`.
- [`../ExecSelf/4_ExecSelfIfFailWithValue.md`](../ExecSelf/4_ExecSelfIfFailWithValue.md) — observar el valor guardado sin alterar el resultado.