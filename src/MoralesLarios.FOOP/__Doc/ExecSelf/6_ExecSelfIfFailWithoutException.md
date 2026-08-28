# ExecSelfIfFailWithoutException — Efectos secundarios solo ante fallos de negocio

## Índice
1. [Introducción](#introducción)
2. [Cuándo un fallo «no tiene excepción»](#cuándo-un-fallo-no-tiene-excepción)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [Particularidad real del código fuente](#particularidad-real-del-código-fuente)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con las demás variantes](#comparación-con-las-demás-variantes)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

`ExecSelfIfFailWithoutException` es el **complemento exacto** de
[`ExecSelfIfFailWithException`](./5_ExecSelfIfFailWithException.md): ejecuta una acción solo cuando el
resultado es fallido **y no hay ninguna excepción** guardada en `Details["Ex"]`.

Dicho de otro modo: se activa ante los **fallos de negocio**, esos que no son averías sino reglas que
no se cumplen.

```csharp
resultado.ExecSelfIfFailWithoutException(errores =>
    _log.LogWarning("Petición rechazada por reglas de negocio: {E}",
                    errores.ToErrorsDescription()));
```

Usados juntos, los dos métodos parten el conjunto de fallos en dos mitades disjuntas y completas, sin
un solo `if`:

```csharp
resultado
    .ExecSelfIfFailWithException(   (errores, ex) => _log.LogError(ex, "Avería técnica"))
    .ExecSelfIfFailWithoutException( errores      => _log.LogWarning("Regla incumplida: {E}",
                                                                    errores.ToErrorsDescription()));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## Cuándo un fallo «no tiene excepción»

Un `MlResult<T>` fallido llega sin excepción cuando se ha construido a partir de mensajes, no de un
`catch`. Los casos típicos:

| Origen del fallo | ¿Tiene `Details["Ex"]`? |
| --- | :---: |
| `"El importe no puede ser negativo".ToMlResultFail<Pedido>()` | No |
| `MlResult.Fail<T>("mensaje")` | No |
| `EnsureFp.That(valor, condicion, "mensaje")` | No |
| `EnsureFp.NotNull(valor, "mensaje")` | No |
| `MapEnsure(x => x.Importe > 0, "mensaje")` | No |
| `BoolToResult(condicion, "mensaje")` | No |
| Fusión de errores de validación (`FusionErrosIfExists`) | No |
| `TryBind` / `TryMap` / `TryMatch` que capturan una excepción | **Sí** |
| `MlResult.Fail<T>("mensaje", exception)` | **Sí** |

Por eso este método es el sitio natural para todo lo que sea «el usuario o el flujo se ha equivocado»:
avisos, métricas de negocio, mensajes de dominio.

---

## Firmas reales

```csharp
// Síncrono
public static MlResult<T> ExecSelfIfFailWithoutException<T>(
        this MlResult<T>         source,
        Action<MlErrorsDetails>  actionFailWithoutException)
```

Fíjate en que el delegado recibe **solo** `MlErrorsDetails`: no hay nada más que pasar, precisamente
porque no hay excepción.

**Comportamiento**:

| Estado de `source` | `Details["Ex"]` | ¿Se ejecuta la acción? | Resultado |
| --- | --- | :---: | --- |
| Válido | — | No | El mismo, válido |
| Fallido | **No** existe | Sí | El mismo, fallido |
| Fallido | Existe | No | El mismo, fallido |

---

## Variantes asíncronas

| Fuente | Delegado | Método |
| --- | --- | --- |
| `MlResult<T>` | `Action<MlErrorsDetails>` | `ExecSelfIfFailWithoutException` |
| `MlResult<T>` | `Func<MlErrorsDetails, Task>` | `ExecSelfIfFailWithoutExceptionAsync` |
| `Task<MlResult<T>>` | `Action<MlErrorsDetails>` | `ExecSelfIfFailWithoutExceptionAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, Task>` | `ExecSelfIfFailWithoutExceptionAsync` |

```csharp
await ValidarSolicitudAsync(solicitud)
    .BindAsync(s => TramitarAsync(s))
    .ExecSelfIfFailWithoutExceptionAsync(async errores =>
        await _notificaciones.AvisarUsuarioAsync(solicitud.Email, errores.ToErrorsMessages()));
```

---

## Particularidad real del código fuente

| Esperarías | Realidad |
| --- | --- |
| `TryExecSelfIfFailWithoutException` | **No existe**, ni síncrono ni asíncrono. |

Esta familia solo tiene las variantes «normales»: `ExecSelfIfFailWithoutException` (1 sobrecarga
síncrona) y `ExecSelfIfFailWithoutExceptionAsync` (4 sobrecargas).

Si tu efecto secundario **puede lanzar** y quieres capturarlo, tienes dos alternativas:

```csharp
// Opción A: protege el delegado a mano.
.ExecSelfIfFailWithoutException(errores =>
{
    try   { _cola.Publicar(new SolicitudRechazada(errores.ToErrorsMessages())); }
    catch (Exception ex) { _log.LogError(ex, "No se pudo publicar el rechazo"); }
});

// Opción B: usa TryExecSelfFail y filtra dentro con GetDetailException.
.TryExecSelfFail(
    errores =>
    {
        var esNegocio = errores.GetDetailException().Match(valid: _ => false, fail: _ => true);
        if (esNegocio) _cola.Publicar(new SolicitudRechazada(errores.ToErrorsMessages()));
    },
    ex => $"No se pudo publicar el rechazo: {ex.Message}");
```

---

## Ejemplos Prácticos

### Ejemplo 1: Avisar al usuario solo de lo que puede corregir

```csharp
public class ServicioSolicitudes
{
    private readonly ILogger<ServicioSolicitudes> _log;
    private readonly INotificaciones _notificaciones;
    private readonly IAlertas        _alertas;

    public Task<MlResult<Solicitud>> TramitarAsync(SolicitudDto dto)
        => ValidarDatosAsync(dto)
            .BindAsync(s => ComprobarRequisitosAsync(s))
            .BindAsync(s => RegistrarAsync(s))

            // Fallo de negocio: el usuario puede arreglarlo → se lo contamos.
            .ExecSelfIfFailWithoutExceptionAsync(async errores =>
            {
                _log.LogInformation("Solicitud de {Email} rechazada: {E}",
                                    dto.Email, errores.ToErrorsDescription());

                await _notificaciones.EnviarAsync(dto.Email,
                    asunto : "No hemos podido tramitar tu solicitud",
                    cuerpo : ComponerCuerpo(errores.ToErrorsMessages()));
            })

            // Fallo técnico: el usuario no puede hacer nada → avisamos al equipo.
            .ExecSelfIfFailWithExceptionAsync(async (errores, ex) =>
            {
                _log.LogError(ex, "Error técnico tramitando la solicitud de {Email}", dto.Email);
                await _alertas.EnviarAsync($"Tramitación caída: {ex.Message}");
            });

    private static string ComponerCuerpo(IEnumerable<string> motivos)
        => "Revisa los siguientes puntos:\n" +
           string.Join("\n", motivos.Select(m => $" • {m}"));
}
```

Reparto de responsabilidades perfecto: **al usuario, lo accionable; al equipo, lo averiado.**

### Ejemplo 2: Métricas de negocio sin contaminar con errores de infraestructura

```csharp
public MlResult<Descuento> AplicarCupon(Carrito carrito, string cupon)
    => BuscarCupon(cupon)
        .Bind(c => MapEnsure(c, x => x.Vigente,        "El cupón ha caducado"))
        .Bind(c => MapEnsure(c, x => !x.Usado,         "El cupón ya se ha utilizado"))
        .Bind(c => MapEnsure(c, x => carrito.Total >= x.ImporteMinimo,
                                                      "El carrito no alcanza el importe mínimo"))
        .Bind(c => Calcular(carrito, c))

        // Contamos por qué se rechazan los cupones. Un SqlException no es un motivo de negocio
        // y quedaría fuera de esta métrica automáticamente.
        .ExecSelfIfFailWithoutException(errores =>
        {
            foreach (var motivo in errores.ToErrorsMessages())
                _metricas.Incrementar("cupones.rechazados", ("motivo", Normalizar(motivo)));
        });

private static string Normalizar(string mensaje) => mensaje switch
{
    var m when m.Contains("caducado", StringComparison.OrdinalIgnoreCase) => "caducado",
    var m when m.Contains("utilizado", StringComparison.OrdinalIgnoreCase) => "ya_usado",
    var m when m.Contains("mínimo",   StringComparison.OrdinalIgnoreCase) => "importe_minimo",
    _                                                                     => "otro"
};
```

### Ejemplo 3: Validación acumulada y respuesta al cliente

```csharp
public async Task<IActionResult> Crear(ClienteDto dto)
{
    var resultado = await ValidarAsync(dto)
        .BindAsync(c => GuardarAsync(c));

    return await resultado
        // Un solo sitio donde se registran los rechazos de formulario.
        .ExecSelfIfFailWithoutExceptionAsync(errores =>
        {
            _log.LogInformation("Alta rechazada ({N} problemas): {E}",
                                errores.Errors.Count(), errores.ToErrorsDescription());
            return Task.CompletedTask;
        })

        .MatchAsync(
            valid: c       => Task.FromResult<IActionResult>(CreatedAtAction(nameof(Obtener),
                                                                            new { id = c.Id }, c)),
            fail:  errores => Task.FromResult<IActionResult>(
                                  BadRequest(new { errores = errores.ToErrorsMessages() })));
}

private static MlResult<Cliente> ValidarAsync(ClienteDto dto)
    => new[]
       {
           EnsureFp.NotNullEmptyOrWhitespace(dto.Nombre, "El nombre es obligatorio").ToResult(),
           EnsureFp.That(dto.Email, e => e.Contains('@'), "El email no es válido").ToResult(),
           EnsureFp.That(dto.Edad,  e => e >= 18,         "Debes ser mayor de edad").ToResult()
       }
       .FusionErrosIfExists()                 // Acumula TODOS los mensajes, sin excepciones
       .Map(_ => Cliente.Desde(dto));
```

> 💡 `FusionErrosIfExists` produce fallos **sin excepción**, así que todos los errores de validación
> caen limpiamente en esta rama. Ver [`../Types/MlResultBucles.md`](../Types/MlResultBucles.md).

---

## Mejores Prácticas

### 1. Emparéjalo siempre con `ExecSelfIfFailWithException`

Por separado son útiles; juntos son un *router* completo de fallos sin ramificaciones manuales.

### 2. Nivel de log adecuado

Un fallo de negocio no es un `Error`. Usa `LogInformation` o `LogWarning`, y reserva `LogError` para
la rama con excepción. Así los cuadros de mando dejan de mentir.

### 3. Es el sitio ideal para mensajes orientados al usuario

Los mensajes de negocio son legibles y accionables; las excepciones no. Aprovéchalo para redactar la
respuesta al cliente.

### 4. Recuerda que no hay variante `Try*`

Si el efecto puede lanzar, protégelo con `try/catch` dentro del delegado o usa `TryExecSelfFail`
filtrando con `GetDetailException()`.

### 5. Sigue sin recuperar el resultado

Para recuperar un fallo de negocio, usa
[`MapIfFailWithoutException`](../Map/7_MapIfFailWithoutException.md) o
[`BindIfFailWithoutException`](../Bind/9_BindIfFailWithoutException.md).

---

## Comparación con las demás variantes

| Método | Se ejecuta si… | El delegado recibe | ¿Existe `Try*`? |
| --- | --- | --- | :---: |
| `ExecSelfIfValid` | Es válido | `T` | Sí |
| `ExecSelfIfFail` | Es fallido (siempre) | `MlErrorsDetails` | Sí (`TryExecSelfFail`) |
| `ExecSelfIfFailWithValue` | Fallido y hay `Details["Value"]` | `MlErrorsDetails`, `TValue` | Sí |
| `ExecSelfIfFailWithException` | Fallido y hay `Details["Ex"]` | `MlErrorsDetails`, `Exception` | Sí |
| **`ExecSelfIfFailWithoutException`** | **Fallido y sin `Details["Ex"]`** | **`MlErrorsDetails`** | **No** |
| `MapIfFailWithoutException` | Fallido y sin excepción | `MlErrorsDetails` | — (transforma) |
| `BindIfFailWithoutException` | Fallido y sin excepción | `MlErrorsDetails` | — (transforma) |

---

## Resumen

- `ExecSelfIfFailWithoutException` ejecuta una acción **solo ante fallos de negocio**: resultado
  fallido **sin** excepción en `Details["Ex"]`.
- El delegado recibe únicamente `MlErrorsDetails`.
- Es el complemento disjunto de `ExecSelfIfFailWithException`; juntos cubren todos los fallos.
- Los fallos sin excepción los generan `ToMlResultFail`, `MlResult.Fail(mensaje)`, `EnsureFp.*`,
  `MapEnsure`, `BoolToResult` y `FusionErrosIfExists`.
- ⚠️ **No existe `TryExecSelfIfFailWithoutException`**: solo hay 1 sobrecarga síncrona y 4 asíncronas.
- No recupera el resultado: para eso están `MapIfFailWithoutException` y `BindIfFailWithoutException`.

## Ver también

- [`1_ExecSelf.md`](./1_ExecSelf.md) — visión general de la familia.
- [`2_ExecSelfIfValid.md`](./2_ExecSelfIfValid.md) — efectos en la rama válida.
- [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md) — efectos ante cualquier fallo.
- [`4_ExecSelfIfFailWithValue.md`](./4_ExecSelfIfFailWithValue.md) — el valor adjunto al fallo.
- [`5_ExecSelfIfFailWithException.md`](./5_ExecSelfIfFailWithException.md) — el complemento exacto de este documento.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y las claves convencionales.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia y recuento de sobrecargas.
- [`../EnsureFp/EnsureFp.md`](../EnsureFp/EnsureFp.md) — generar fallos de negocio sin excepciones.
- [`../Bind/9_BindIfFailWithoutException.md`](../Bind/9_BindIfFailWithoutException.md) y [`../Map/7_MapIfFailWithoutException.md`](../Map/7_MapIfFailWithoutException.md) — cuando sí quieres recuperar.