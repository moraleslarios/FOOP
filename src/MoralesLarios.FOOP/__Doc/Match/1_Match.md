# MlResult Match - Pattern Matching Funcional

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos Match Básicos](#métodos-match-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryMatch - Captura de Excepciones](#métodos-trymatch---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Bind y Map](#comparación-con-bind-y-map)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

Los métodos `Match` implementan el patrón de **pattern matching funcional** sobre `MlResult<T>`, permitiendo transformar el resultado en un tipo completamente diferente (`TReturn`) con lógica específica para casos de éxito y fallo.

### Propósito Principal

- **Transformación Condicional**: Convertir `MlResult<T>` a cualquier tipo `TReturn`
- **Manejo Bifurcado**: Lógica diferente para éxito vs fallo
- **Finalización de Cadenas**: Terminar pipelines de `MlResult` extrayendo valores
- **Mapeo a Tipos de Respuesta**: Convertir a DTOs, responses HTTP, etc.

---

## Análisis de los Métodos

### Filosofía del Pattern Matching

```
MlResult<T> → Match(validFunc, failFunc) → TReturn
   ↓                     ↓                    ↓
Éxito → validFunc(value) → TReturn
Fallo → failFunc(errorsDetails) → TReturn
```

### Características Principales

1. **Transformación Total**: Siempre produce un `TReturn`, nunca `MlResult`
2. **Bifurcación Funcional**: Dos funciones para dos caminos
3. **Finalización**: Extrae valores del contexto `MlResult`
4. **Soporte Asíncrono Completo**: Todas las combinaciones async/sync
5. **Versiones Seguras**: TryMatch captura excepciones

---

## Métodos Match Básicos

> ⚠️ **Nota sobre `MlErrorsDetails`.** Este tipo expone **solo** dos propiedades: `Errors`
> (`IEnumerable<MlError>`) y `Details` (`Dictionary<string, object>`). **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception` ni `HasValue`. Para obtener los mensajes usa los métodos de
> extensión reales:
>
> | Necesitas | Usa |
> | --- | --- |
> | Array/lista de mensajes | `errores.ToErrorsMessages()` |
> | Un único texto con todos los errores | `errores.ToErrorsDescription()` |
> | El primer mensaje | `errores.Errors.First().Message` |
> | La excepción capturada por un `Try*` | `errores.GetDetailException()` → `MlResult<Exception>` |
> | Un dato adjunto | `errores.GetDetailValue<T>()` / `errores.GetDetail<T>("clave")` |

### `Match<T, TReturn>()`

**Propósito**: transformar un `MlResult<T>` en un `TReturn` con lógica específica para éxito y fallo.

```csharp
public static TReturn Match<T, TReturn>(this MlResult<T>               source,
                                        Func<T, TReturn>               valid,
                                        Func<MlErrorsDetails, TReturn> fail)
```

**Comportamiento**:
- Si `source` es válido: ejecuta `valid(value)` y devuelve su resultado.
- Si `source` es fallido: ejecuta `fail(errorsDetails)` y devuelve su resultado.
- El valor devuelto es un `TReturn` **crudo**: aquí *sales* del mundo `MlResult`. Por eso `Match` es la
  operación natural del **borde** de la aplicación (controladores, handlers, `Main`).

**Ejemplo Básico**:
```csharp
MlResult<Usuario> usuario = ObtenerUsuario(usuarioId);

RespuestaApi<Usuario> respuesta = usuario.Match(
    valid: u        => new RespuestaApi<Usuario> { Correcto = true,  Datos = u },
    fail : errores  => new RespuestaApi<Usuario> { Correcto = false,
                                                  Errores  = errores.ToErrorsMessages() });
```

**Ejemplo con distinto tipo de retorno** (decidir un código HTTP a partir de los errores):
```csharp
MlResult<Pago> resultado = ProcesarPago(datosPago);

int codigo = resultado.Match(
    valid: pago    => StatusCodes.Status200OK,
    fail : errores => errores.ToErrorsDescription().Contains("fraude", StringComparison.OrdinalIgnoreCase)
                        ? StatusCodes.Status403Forbidden
                        : StatusCodes.Status400BadRequest);
```

**Ejemplo idiomático: extraer el valor con un valor por defecto**

`Match` es también la forma correcta (y única segura) de «sacar» el valor, ya que `MlResult<T>.Value`
es `internal protected` a propósito:

```csharp
// ❌ No compila fuera de la librería, y no debe hacerse:
// var config = resultado.Value;

// ✅ Idiomático: si falla, usamos la configuración por defecto.
Configuracion config = resultado.Match(
    valid: c => c,
    fail : _ => Configuracion.PorDefecto);
```

**Ejemplo: enrutar según el tipo de excepción capturada**

```csharp
IActionResult respuesta = resultado.Match(
    valid: dto     => Ok(dto),
    fail : errores => errores.GetDetailException().Match(
                          // Hubo excepción: distinguimos por su tipo.
                          valid: ex => ex switch
                          {
                              TimeoutException      => StatusCode(504, "El proveedor no responde"),
                              HttpRequestException  => StatusCode(502, "Error del proveedor"),
                              UnauthorizedAccessException => Forbid(),
                              _                     => StatusCode(500, ex.Message)
                          },
                          // No hubo excepción: es un error de negocio o validación.
                          fail : _  => BadRequest(errores.ToErrorsMessages())));
```

---

## Variantes Asíncronas

### `MatchAsync<T, TReturn>()` - Ambas Funciones Asíncronas

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)
```

**Ejemplo**:
```csharp
var result = await GetOrderAsync(orderId);
var notification = await result.MatchAsync(
    validAsync: async order => await _emailService.SendOrderConfirmationAsync(order),
    failAsync: async errors => await _emailService.SendErrorNotificationAsync(errors)
);
```

### `MatchAsync<T, TReturn>()` - Solo Función de Fallo Asíncrona

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> valid,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)
```

### `MatchAsync<T, TReturn>()` - Solo Función de Éxito Asíncrona

```csharp
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this MlResult<T> source,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, TReturn> fail)
```

### Variantes con Fuente Asíncrona

```csharp
// Task<MlResult<T>> con funciones asíncronas
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task<TReturn>> validAsync,
    Func<MlErrorsDetails, Task<TReturn>> failAsync)

// Task<MlResult<T>> con funciones síncronas
public static async Task<TReturn> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, TReturn> valid,
    Func<MlErrorsDetails, TReturn> fail)
```

---

## Métodos TryMatch - Captura de Excepciones

### `TryMatch<T, TResult>()` - Versión Segura

```csharp
public static MlResult<TResult> TryMatch<T, TResult>(
    this MlResult<T> source, 
    Func<T, TResult> valid,
    Func<MlErrorsDetails, TResult> fail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Ejecuta el `Match` normalmente.
- Si **cualquiera** de las dos funciones lanza una excepción, devuelve un `MlResult<TResult>` fallido
  con la excepción guardada en `Details["Ex"]` (recuperable con `GetDetailException()`).
- Convierte así las excepciones en errores manejables dentro de la tubería.

> ⚠️ **Diferencia importante**: `Match` devuelve `TResult` **crudo**, mientras que `TryMatch` devuelve
> `MlResult<TResult>`. Necesita el envoltorio para poder representar el fallo del propio delegado.

**Ejemplo**:
```csharp
MlResult<Datos> resultado = ProcesarDatos(datos);

MlResult<RespuestaDto> respuesta = resultado.TryMatch(
    valid: d       => TransformarADto(d),                            // Puede lanzar excepción
    fail : errores => new RespuestaDto { Mensaje = errores.Errors.First().Message },
    ex => $"Falló la transformación a DTO: {ex.Message}");
```

**Ejemplo con mensaje fijo** (sobrecarga `string exceptionAditionalMessage`):
```csharp
MlResult<byte[]> pdf = informe.TryMatch(
    valid: inf     => SerializarPdf(inf),                            // Puede lanzar
    fail : errores => GenerarPdfDeError(errores.ToErrorsDescription()),
    "No se pudo generar el PDF del informe");
```

Si omites tanto `errorMessageBuilder` como `exceptionAditionalMessage`, se usa el mensaje por defecto
`DEFAULT_EX_ERROR_MESSAGE(ex)` definido en `Helpers/Constants.cs`.

### Versiones Asíncronas de TryMatch

```csharp
// Funciones asíncronas
public static Task<MlResult<TResult>> TryMatchAsync<T, TResult>(
    this MlResult<T> source, 
    Func<T, Task<TResult>> validAsync,
    Func<MlErrorsDetails, Task<TResult>> failAsync,
    Func<Exception, string> errorMessageBuilder)

// Con Task<MlResult<T>>
public static async Task<MlResult<TResult>> TryMatchAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<TResult>> validAsync,
    Func<MlErrorsDetails, Task<TResult>> failAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Constructor de respuestas de API

Un único punto de traducción entre el mundo `MlResult` y el mundo HTTP. Observa que **todo** el acceso a
los errores se hace con la API real (`ToErrorsMessages`, `ToErrorsDescription`, `GetDetailException`).

```csharp
public static class ConstructorRespuestas
{
    /// <summary>
    /// Envuelve cualquier MlResult en un DTO de respuesta uniforme.
    /// </summary>
    public static RespuestaApi<T> Construir<T>(MlResult<T> resultado)
        => resultado.Match(
            valid: datos   => new RespuestaApi<T>
            {
                Correcto   = true,
                Datos      = datos,
                MomentoUtc = DateTime.UtcNow
            },
            fail : errores => new RespuestaApi<T>
            {
                Correcto      = false,
                Errores       = errores.ToErrorsMessages(),      // string[]
                CodigoError   = DeterminarCodigo(errores),
                Diagnostico   = errores.ToErrorsDescription(),   // texto único
                MomentoUtc    = DateTime.UtcNow
            });

    /// <summary>
    /// Variante asíncrona: registra el resultado y devuelve el IActionResult adecuado.
    /// </summary>
    public static Task<IActionResult> ConstruirAsync<T>(Task<MlResult<T>> resultadoAsync)
        => resultadoAsync.MatchAsync(
            validAsync: async datos =>
            {
                await RegistrarExitoAsync(datos);
                return (IActionResult) new OkObjectResult(datos);
            },
            failAsync : async errores =>
            {
                await RegistrarErroresAsync(errores);
                return new ObjectResult(errores.ToErrorsMessages())
                {
                    StatusCode = DeterminarCodigoHttp(errores)
                };
            });

    private static string DeterminarCodigo(MlErrorsDetails errores)
    {
        // Un único texto en minúsculas evita recorrer la colección varias veces.
        var texto = errores.ToErrorsDescription().ToLowerInvariant();

        if (texto.Contains("validación") || texto.Contains("validacion")) return "VALIDATION_ERROR";
        if (texto.Contains("no existe")  || texto.Contains("no encontrado")) return "NOT_FOUND";
        return "GENERAL_ERROR";
    }

    private static int DeterminarCodigoHttp(MlErrorsDetails errores)
        // Primero miramos si hubo excepción: es la señal más fiable.
        => errores.GetDetailException().Match(
            valid: ex => ex switch
            {
                TimeoutException            => StatusCodes.Status504GatewayTimeout,
                HttpRequestException        => StatusCodes.Status502BadGateway,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _                           => StatusCodes.Status500InternalServerError
            },
            // Sin excepción → error de negocio: lo decidimos por el texto.
            fail : _  => DeterminarCodigo(errores) switch
            {
                "NOT_FOUND"        => StatusCodes.Status404NotFound,
                "VALIDATION_ERROR" => StatusCodes.Status400BadRequest,
                _                  => StatusCodes.Status400BadRequest
            });

    private static Task RegistrarExitoAsync<T>(T datos)          => Task.CompletedTask;
    private static Task RegistrarErroresAsync(MlErrorsDetails e) => Task.CompletedTask;
}

public class RespuestaApi<T>
{
    public bool     Correcto    { get; set; }
    public T?       Datos       { get; set; }
    public string[] Errores     { get; set; } = [];
    public string?  CodigoError { get; set; }
    public string?  Diagnostico { get; set; }
    public DateTime MomentoUtc  { get; set; }
}
```

**Uso desde un controlador**, en una sola línea:

```csharp
[HttpGet("{id:guid}")]
public Task<IActionResult> Get(Guid id)
    => ConstructorRespuestas.ConstruirAsync(_servicio.ObtenerAsync(id));
```

### Ejemplo 2: Sistema de notificaciones condicionales

Aquí `TryMatchAsync` cierra una tubería de negocio: la rama válida y la rama de fallo hacen trabajo
asíncrono (enviar correos, SMS, alertas) y ambas devuelven el **mismo** tipo, `ResultadoNotificacion`.
Como el envío puede lanzar excepciones, usamos la variante `Try*`, que las convierte en `Fail` en lugar
de dejarlas escapar.

```csharp
public class ServicioNotificaciones
{
    private readonly IServicioEmail _email;
    private readonly IServicioSms   _sms;
    private readonly ILogger<ServicioNotificaciones> _log;

    public Task<MlResult<ResultadoNotificacion>> ProcesarPedidoAsync(SolicitudPedido solicitud)
        => ValidarPedido(solicitud)
            .BindAsync(async valido  => await ProcesarPagoAsync(valido))
            .BindAsync(async pagado  => await CrearPedidoAsync(pagado))
            .TryMatchAsync(
                validAsync: async pedido  => await NotificarExitoAsync(pedido),
                failAsync : async errores => await NotificarFalloAsync(solicitud, errores),
                ex => $"Fallo al enviar las notificaciones: {ex.Message}");

    // ── Rama válida ────────────────────────────────────────────────────────────
    private async Task<ResultadoNotificacion> NotificarExitoAsync(Pedido pedido)
    {
        var enviadas = new List<string>();

        if (await _email.EnviarConfirmacionAsync(pedido.ClienteId, pedido))
            enviadas.Add($"Email de confirmación al cliente {pedido.ClienteId}");

        if (pedido.EsUrgente && await _sms.EnviarUrgenteAsync(pedido.TelefonoCliente, pedido.Id))
            enviadas.Add($"SMS urgente a {pedido.TelefonoCliente}");

        if (pedido.Importe > 1_000m)
        {
            await _email.EnviarAvisoImporteAltoAsync(pedido.EmailComercial, pedido);
            enviadas.Add("Aviso de importe alto al comercial");
        }

        return new ResultadoNotificacion
        {
            Correcto  = true,
            PedidoId  = pedido.Id,
            Enviadas  = [.. enviadas],
            Mensaje   = $"Pedido {pedido.Id} procesado correctamente"
        };
    }

    // ── Rama de fallo ──────────────────────────────────────────────────────────
    private async Task<ResultadoNotificacion> NotificarFalloAsync(SolicitudPedido solicitud,
                                                                 MlErrorsDetails  errores)
    {
        var enviadas = new List<string>();

        // ToErrorsDescription() da un único texto legible con todos los errores.
        var diagnostico = errores.ToErrorsDescription();

        if (!string.IsNullOrWhiteSpace(solicitud.EmailCliente))
        {
            await _email.EnviarAvisoErrorAsync(solicitud.EmailCliente, diagnostico);
            enviadas.Add($"Aviso de error a {solicitud.EmailCliente}");
        }

        if (EsErrorCritico(errores))
        {
            await _email.EnviarAlertaCriticaAsync("soporte@empresa.com", solicitud, diagnostico);
            enviadas.Add("Alerta crítica a soporte");
        }

        // El log incluye la excepción original si la hubo, gracias a GetDetailException().
        errores.GetDetailException()
               .Match(valid: ex => { _log.LogError(ex, "Pedido rechazado: {D}", diagnostico); return 0; },
                      fail : _  => { _log.LogWarning("Pedido rechazado: {D}", diagnostico);   return 0; });

        return new ResultadoNotificacion
        {
            Correcto     = false,
            CodigoError  = ClasificarError(errores),
            // El primer mensaje se obtiene de la colección real de errores.
            MensajeError = errores.Errors.FirstOrDefault()?.Message ?? diagnostico,
            Enviadas     = [.. enviadas],
            Mensaje      = "El pedido no se pudo procesar; se han enviado los avisos"
        };
    }

    /// <summary>
    /// Un error es crítico si vino de una excepción de infraestructura
    /// o si su descripción menciona un componente crítico.
    /// </summary>
    private static bool EsErrorCritico(MlErrorsDetails errores)
        => errores.GetDetailException().Match(
               valid: ex => ex is TimeoutException or HttpRequestException or InvalidOperationException,
               fail : _  => new[] { "pasarela de pago", "base de datos", "crítico" }
                                .Any(clave => errores.ToErrorsDescription()
                                                     .Contains(clave, StringComparison.OrdinalIgnoreCase)));

    private static string ClasificarError(MlErrorsDetails errores)
    {
        var texto = errores.ToErrorsDescription().ToLowerInvariant();

        if (texto.Contains("validación") || texto.Contains("validacion")) return "VALIDACION";
        if (texto.Contains("pago"))                                        return "PAGO";
        if (texto.Contains("stock"))                                       return "STOCK";
        return "GENERAL";
    }
}

public class ResultadoNotificacion
{
    public bool     Correcto     { get; set; }
    public Guid?    PedidoId     { get; set; }
    public string?  CodigoError  { get; set; }
    public string?  MensajeError { get; set; }
    public string[] Enviadas     { get; set; } = [];
    public string   Mensaje      { get; set; } = string.Empty;
}
```

> 💡 **Detalle clave**: `TryMatchAsync` devuelve `MlResult<ResultadoNotificacion>`, **no**
> `ResultadoNotificacion` a secas. La variante `Try*` necesita envolver el resultado para poder
> representar el fallo del propio delegado. La sobrecarga sin `Try` (`MatchAsync`) sí devuelve el
> valor crudo.

---

## Mejores Prácticas

### 1. Cuándo usar `Match` frente a `Bind` / `Map`

| Quieres… | Usa | Devuelve |
| --- | --- | --- |
| Continuar la tubería con otra operación que puede fallar | `Bind` | `MlResult<TReturn>` |
| Continuar la tubería transformando el valor (no falla) | `Map` | `MlResult<TReturn>` |
| Observar sin cambiar nada (log, métrica) | `ExecSelf*` | El mismo `MlResult<T>` |
| Recuperarte de un fallo con un valor alternativo | `MapIfFail` / `BindIfFail` | `MlResult<T>` |
| **Salir** del mundo `MlResult` con un valor concreto | `Match` | `TReturn` crudo |

Regla práctica: **`Match` va al final**. Si aparece en medio de una tubería, casi siempre lo que
querías era `Bind`, `Map`, `MapIfFail` o `ExecSelf*`.

### 2. Manejo de excepciones

Si **cualquiera** de las dos ramas puede lanzar, usa `TryMatch` / `TryMatchAsync`. Ten en cuenta que
`TryMatch` **cambia el tipo de retorno** a `MlResult<TResult>`, mientras que `Match` devuelve
`TResult` crudo: esa es la única diferencia estructural entre ambos. La excepción capturada queda en
`Details["Ex"]` y se recupera después con `GetDetailException()`.

Hay dos formas de indicar el mensaje de error: `Func<Exception, string> errorMessageBuilder` o
`string exceptionAditionalMessage`. Si no indicas ninguna, se usa el mensaje por defecto
`DEFAULT_EX_ERROR_MESSAGE(ex)` definido en `Helpers/Constants.cs`.

### 3. Funciones asíncronas

Existe una sobrecarga para cada combinación, de modo que **nunca necesitas `await` intermedios** ni
`.Result`:

| Fuente | `valid` | `fail` | Método |
| --- | --- | --- | --- |
| `MlResult<T>` | sync | sync | `Match` |
| `MlResult<T>` | async | async | `MatchAsync` |
| `MlResult<T>` | sync | async | `MatchAsync` |
| `MlResult<T>` | async | sync | `MatchAsync` |
| `Task<MlResult<T>>` | sync | sync | `MatchAsync` |
| `Task<MlResult<T>>` | async | async | `MatchAsync` |
| `Task<MlResult<T>>` | sync | async | `MatchAsync` |
| `Task<MlResult<T>>` | async | sync | `MatchAsync` |

### 4. Ambas ramas deben devolver el mismo tipo

Es un requisito del compilador, pero conviene recordarlo: si las ramas devuelven tipos distintos,
declara explícitamente `TReturn` (por ejemplo `Match<Pedido, IActionResult>`) o convierte ambas a un
tipo común (`IActionResult`, una interfaz propia, etc.).

---

## Comparación con Bind y Map

### Tabla Comparativa

| Operación | Recibe | Devuelve | ¿Sigue en la tubería? | ¿Se ejecuta si hay fallo? |
| --- | --- | --- | :---: | :---: |
| `Bind` | `T` | `MlResult<TReturn>` | Sí | No |
| `Map` | `T` | `MlResult<TReturn>` | Sí | No |
| `MapIfFail` | `MlErrorsDetails` | `MlResult<T>` | Sí | **Solo** si hay fallo |
| `ExecSelf` | `T` o los errores | El mismo `MlResult<T>` | Sí | Según la variante |
| `Match(valid, fail)` | `T` **y** los errores | `TReturn` **crudo** | **No** | Sí (rama `fail`) |
| `TryMatch(valid, fail, …)` | `T` **y** los errores | `MlResult<TResult>` | Sí | Sí (rama `fail`) |
| `Match(funcAll)` | Nada | `MlResult<TReturn>` | Sí | Sí (ignora el estado) |

### Ejemplo Comparativo

La misma necesidad, «obtener un pedido y devolver una respuesta HTTP», resuelta con cada operación:

- Con `Bind`: encadenas otra operación que **también puede fallar** y sigues dentro de `MlResult`.
- Con `Map`: transformas el pedido en un DTO y sigues dentro de `MlResult`.
- Con `MapIfFail`: sustituyes el fallo por un pedido «vacío» y sigues dentro de `MlResult`.
- Con `ExecSelfIfFail`: registras el error en el log y el resultado **no cambia**.
- Con `Match`: decides `Ok(...)` o `BadRequest(...)` y **sales** con un `IActionResult`.

---

## Resumen

- `Match` es el **pattern matching** de la librería: una función para el caso válido y otra para el
  caso fallido, y devuelve un `TReturn` **crudo**.
- Es la operación del **borde** de la aplicación: donde se abandona el mundo `MlResult`.
- Es también la forma correcta de leer el contenido, porque `MlResult<T>.Value` es
  `internal protected` a propósito.
- `TryMatch` hace lo mismo pero captura las excepciones de los delegados y devuelve
  `MlResult<TResult>`, guardando la excepción en `Details["Ex"]`.
- Existen sobrecargas para las **ocho** combinaciones de fuente y delegados síncronos/asíncronos.
- `MlErrorsDetails` solo tiene `Errors` y `Details`: usa `ToErrorsMessages()`,
  `ToErrorsDescription()`, `GetDetailException()` y `GetDetailValue<T>()` para consultarlo.

## Ver también

- [`2_MatchAll.md`](./2_MatchAll.md) — la sobrecarga incondicional (`Func<TReturn>` sin parámetros).
- [`../Types/MlResultActionsMatch.md`](../Types/MlResultActionsMatch.md) — referencia completa del archivo fuente.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — el sistema de errores (`MlError`, `MlErrorsDetails`).
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException`, `GetDetailValue<T>`, `GetDetail<T>`.
- [`../Bind/3_Bind.md`](../Bind/3_Bind.md) — encadenar operaciones que pueden fallar.
- [`../Map/1_Map.md`](../Map/1_Map.md) — transformar el valor sin salir de la tubería.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — efectos secundarios sin alterar el resultado.