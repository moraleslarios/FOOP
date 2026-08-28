# BindIfFailWithoutException — Recuperarse solo de los fallos que NO son excepciones

## Índice

1. [Introducción](#introducción)
2. [Cuándo un fallo «no tiene excepción»](#cuándo-un-fallo-no-tiene-excepción)
3. [Firma real](#firma-real)
4. [Tabla de comportamiento](#tabla-de-comportamiento)
5. [El par complementario: `WithException` y `WithoutException`](#el-par-complementario-withexception-y-withoutexception)
6. [Variantes asíncronas](#variantes-asíncronas)
7. [`TryBindIfFailWithoutException`](#trybindiffailwithoutexception)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

No todos los fallos son iguales. Una validación que rechaza un importe negativo y un `SqlException` por caída de la base de datos llegan al mismo sitio —un `MlResult<T>` en estado *fail*— pero merecen tratamientos opuestos:

- El **fallo de negocio** es esperado, forma parte del dominio, y muchas veces se puede corregir o sustituir por un valor alternativo.
- El **fallo técnico** es una anomalía: intentar «arreglarlo» aplicando reglas de negocio suele empeorar las cosas y ocultar el problema real.

`BindIfFailWithoutException` te permite escribir una recuperación que **solo se aplica a los fallos de negocio**, dejando pasar intactos los fallos técnicos.

```csharp
// ❌ BindIfFail recupera TODO, incluido el fallo de infraestructura
var perfil = ObtenerPerfil(id)
                .BindIfFail(_ => Perfil.Anonimo.ToMlResultValid());
// Si la BBDD estaba caída, el usuario recibe un perfil anónimo
// y nadie se enterará nunca de la incidencia.

// ✅ Solo se aplica el valor por defecto a los fallos de negocio
var perfil = ObtenerPerfil(id)
                .BindIfFailWithoutException(_ => Perfil.Anonimo.ToMlResultValid());
// Un SqlException sigue siendo un fallo: se propaga y se puede traducir a un 500.
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`; para llegar al valor original, `GetDetailValue<T>()`.

---

## Cuándo un fallo «no tiene excepción»

El criterio es puramente mecánico: **un fallo «no tiene excepción» cuando su diccionario `Details` no contiene la clave convencional `"Ex"`** (`EX_DESC_KEY`). Esa clave la rellenan automáticamente los métodos de la familia `Try*` cuando capturan una excepción.

| Cómo se creó el fallo | ¿Lleva `Details["Ex"]`? | ¿Se ejecuta tu función? |
|---|---|---|
| `"El importe debe ser positivo".ToMlResultFail<T>()` | No | ✅ Sí |
| `MapEnsure(x => x > 0, "…")` | No | ✅ Sí |
| `EnsureFp.NotNull(dto, "…")` / `EnsureFp.That(...)` | No | ✅ Sí |
| `NullToFailed(...)` / `EmptyToFailed(...)` / `BoolToResult(...)` | No | ✅ Sí |
| `BindMulti` fusionando errores de validación | No | ✅ Sí |
| `TryBind` / `TryMap` / `TryMatch`… que capturó una excepción | Sí | ❌ No |
| `MlErrorsDetails.FromErrorMessageWithException(msg, ex)` | Sí | ❌ No |
| Un fallo de negocio **fusionado** con otro que sí traía excepción | Sí | ❌ No |

> 📌 La última fila es la trampa habitual: `MergeErrorsDetailsIfFail` y `FusionErrosIfExists` combinan los diccionarios `Details`, así que basta con que **uno** de los fallos fusionados trajera una excepción para que el resultado se considere «técnico».

---

## Firma real

Solo hay **una forma** (no existe la variante de dos caminos que sí tienen `BindIfFail` o `BindIfFailWithException`):

```csharp
public static MlResult<T> BindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                             Func<MlErrorsDetails, MlResult<T>> func)
    => source.Match(
            fail : errorsDetails => errorsDetails.GetDetailException().Match(
                                            fail : func,                       // ← NO hay excepción: recupera
                                            valid: _ => errorsDetails),        // ← SÍ hay excepción: propaga el fallo
            valid: x             => x);
```

🔑 Fíjate en la inversión respecto a `BindIfFailWithException`: aquí tu función se coloca en la rama **`fail`** del `Match` interior, es decir, se ejecuta precisamente cuando `GetDetailException()` **no** encuentra nada. Es el mismo mecanismo leído al revés.

Tu función recibe el `MlErrorsDetails` completo (no una excepción, obviamente: no hay ninguna), así que tienes acceso a los mensajes y al resto de claves de `Details`.

---

## Tabla de comportamiento

| Estado de entrada | `Details["Ex"]` | Resultado |
|---|---|---|
| Válido | — | El valor se devuelve intacto; `func` no se ejecuta |
| Fallo | ausente | Se ejecuta `func(errorsDetails)`: puede volver al camino válido o devolver otro fallo |
| Fallo | presente | El fallo se devuelve intacto; `func` **no** se ejecuta |

Como `func` devuelve un `MlResult<T>`, la recuperación es opcional: puedes usarla también para **transformar** el fallo de negocio en otro fallo de negocio más claro sin volver al camino válido.

---

## El par complementario: `WithException` y `WithoutException`

Juntas, las dos familias parten el conjunto de fallos en dos mitades disjuntas y exhaustivas. Eso permite escribir una política completa sin ningún `if`:

```csharp
var resultado = ObtenerTarifa(id)

    // Mitad 1: fallos técnicos → reintento / caché
    .TryBindIfFailWithException(_  => _cache.Leer(id),
                                ex => $"La caché falló: {ex.Message}")

    // Mitad 2: fallos de negocio → tarifa por defecto
    .BindIfFailWithoutException(_ => Tarifa.Estandar.ToMlResultValid());
```

| Método | Se ejecuta si… | Recibe |
|---|---|---|
| `BindIfFailWithException` | hay excepción | `Exception` |
| `BindIfFailWithExceptionError` | hay excepción | `MlErrorsDetails` |
| `BindIfFailWithoutException` | **no** hay excepción | `MlErrorsDetails` |
| `BindIfFail` | siempre que haya fallo | `MlErrorsDetails` |

> 💡 Si vas a encadenar las dos mitades, pon primero la de excepciones. Da igual el orden por corrección (los conjuntos son disjuntos), pero se lee mejor «primero lo excepcional, luego lo ordinario».

---

## Variantes asíncronas

| Origen | Delegado | Firma |
|---|---|---|
| `MlResult<T>` | asíncrono | `BindIfFailWithoutExceptionAsync<T>(Func<MlErrorsDetails, Task<MlResult<T>>>)` |
| `Task<MlResult<T>>` | asíncrono | idem sobre origen asíncrono |
| `Task<MlResult<T>>` | síncrono | se adapta internamente con `func.ToFuncTask()` |

```csharp
public Task<MlResult<Perfil>> ObtenerAsync(int id)
    => _repo.ObtenerAsync(id)                                   // Task<MlResult<Perfil>>
            .BindIfFailWithoutExceptionAsync(async errores =>
            {
                _log.LogInformation("Perfil {Id} no encontrado ({Motivo}); se usa el anónimo",
                                    id, errores.ToErrorsMessages());
                return await Perfil.AnonimoAsync();
            });
```

> 📝 La sobrecarga asíncrona usa internamente `GetDetailExceptionAsync()`, la versión `Task` del extractor. El comportamiento es idéntico al síncrono.

---

## `TryBindIfFailWithoutException`

Como en el resto de la librería, la variante `Try*` protege tu propia función de recuperación por si lanza:

```csharp
public static MlResult<T> TryBindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                                Func<MlErrorsDetails, MlResult<T>> func,
                                                                Func<Exception, string>            errorMessageBuilder);

public static MlResult<T> TryBindIfFailWithoutException<T>(this MlResult<T>                        source,
                                                                Func<MlErrorsDetails, MlResult<T>> func,
                                                                string                             errorMessage = null!);
```

| Método | Síncronas | Asíncronas |
|---|---|---|
| `BindIfFailWithoutException` | 1 | 3 |
| `TryBindIfFailWithoutException` | 2 | 4 |

> 🔑 Hay una consecuencia elegante: si tu recuperación de negocio lanza y `TryBindIfFailWithoutException` la captura, el fallo resultante **ya lleva** `Details["Ex"]`. Es decir, deja de ser un fallo de negocio y pasa a ser técnico, con lo que un `BindIfFailWithoutException` posterior no volverá a intentarlo. El mecanismo evita bucles de recuperación por sí solo.

---

## Ejemplos Prácticos

### Ejemplo 1: Política completa de dos mitades

```csharp
public class ServicioTarifas
{
    private readonly ITarifasRepo  _repo;
    private readonly ITarifasCache _cache;
    private readonly ILogger       _log;

    public MlResult<Tarifa> Obtener(int clienteId)
        => clienteId.ToMlResultValid()
             .MapEnsure(x => x > 0, "El identificador de cliente debe ser positivo")

             // Puede lanzar → si lanza, el fallo llevará Details["Ex"]
             .TryBind(x => _repo.Obtener(x),
                      ex => $"No se pudo leer la tarifa del repositorio: {ex.Message}")

             // MITAD TÉCNICA: caída de infraestructura → caché, y se registra
             .TryBindIfFailWithException(
                    ex => { _log.LogError(ex, "Repositorio de tarifas no disponible");
                            return _cache.Leer(clienteId); },
                    ex => $"La caché de tarifas tampoco respondió: {ex.Message}")

             // MITAD DE NEGOCIO: cliente sin tarifa asignada → tarifa estándar
             .BindIfFailWithoutException(errores =>
                    {
                        _log.LogInformation("Cliente {Id} sin tarifa propia ({Motivo}); se aplica la estándar",
                                            clienteId, errores.ToErrorsMessages());
                        return Tarifa.Estandar.ToMlResultValid();
                    });
}
```

Resultado: la validación del identificador y la ausencia de tarifa se resuelven con la tarifa estándar; una caída del repositorio se registra como error y, si la caché también falla, el resultado sigue siendo un fallo que la capa web traducirá a un 5xx.

### Ejemplo 2: Enriquecer los mensajes de negocio sin recuperar

`func` puede devolver otro fallo. Aquí lo usamos para convertir un mensaje técnico en un mensaje apto para el usuario final, **solo** cuando el fallo es de negocio.

```csharp
public MlResult<Reserva> Reservar(ReservaDto dto)
    => ValidarReserva(dto)
         .TryBind(r => _motor.Confirmar(r),
                  ex => $"El motor de reservas devolvió un error: {ex.Message}")

         // Traduce a lenguaje de usuario, pero NO toca los errores técnicos
         .BindIfFailWithoutException(errores =>
                MlErrorsDetails.FromErrorMessageDetails(
                        "No hemos podido completar tu reserva. Revisa las fechas y el número de personas.",
                        errores.Details)                      // conserva el contexto original
                    .AddErrorsMessages(errores.ToErrorsMessages())   // y también los mensajes internos
                    .ToMlResultFail<Reserva>());
```

Un fallo técnico llega a la capa superior con su excepción intacta para el log; un fallo de negocio llega con un mensaje comprensible **y** con los mensajes originales conservados para diagnóstico.

### Ejemplo 3: Importación de fichero con corrección automática

```csharp
public record LineaCsv(int Numero, string Contenido);
public record Registro(int Numero, string Nombre, decimal Importe);

public MlResult<Registro> ProcesarLinea(LineaCsv linea)
    => linea.ToMlResultValid()
            .AddValueIfFail(linea)                       // para poder reintentar con el original
            .TryBind(l => Parsear(l),                    // puede lanzar FormatException
                     ex => $"Línea {linea.Numero} ilegible: {ex.Message}")

            // Solo las líneas que fallaron por REGLAS (no por formato) se intentan corregir
            .BindIfFailWithValue(l => Parsear(Normalizar(l)))

            // Y si sigue fallando por negocio, se marca como pendiente de revisión manual
            .BindIfFailWithoutException(errores =>
                    errores.AddErrorMessage($"Línea {linea.Numero} marcada para revisión manual")
                           .ToMlResultFail<Registro>());

private static LineaCsv Normalizar(LineaCsv l)
    => l with { Contenido = l.Contenido.Trim().Replace(';', ',') };
```

### Ejemplo 4: Traducción a códigos HTTP en la capa web

La partición técnico/negocio encaja de forma natural con la separación 5xx/4xx.

```csharp
[HttpPost]
public async Task<IActionResult> Crear([FromBody] PedidoDto dto)
{
    var resultado = await _servicio.CrearAsync(dto)

        // Último intento de negocio: completar datos opcionales ausentes
        .BindIfFailWithoutExceptionAsync(async errores =>
                errores.ToErrorsMessages().Any(m => m.Contains("dirección",
                                                               StringComparison.OrdinalIgnoreCase))
                    ? await _servicio.CrearAsync(dto with { Direccion = await DireccionPorDefectoAsync(dto.ClienteId) })
                    : errores.ToMlResultFail<Pedido>());

    return resultado.Match(
        valid: pedido  => CreatedAtAction(nameof(Obtener), new { id = pedido.Id }, pedido),
        fail : errores => errores.GetDetailException()
                                 .Match(
                                    // Había excepción ⇒ fallo técnico
                                    valid: ex => { _log.LogError(ex, "Error creando el pedido");
                                                   return StatusCode(500, "Error interno"); },
                                    // No había excepción ⇒ fallo de negocio
                                    fail : _  => (IActionResult)BadRequest(errores.ToErrorsMessages())));
}
```

---

## Mejores Prácticas

1. **Usa `BindIfFailWithoutException` para los valores por defecto.** Aplicar un valor por defecto cuando la causa real era una caída de infraestructura es una de las formas más eficaces de ocultar un incidente en producción.

2. **Pon primero la mitad técnica.** `TryBindIfFailWithException(...)` seguido de `BindIfFailWithoutException(...)` cubre todos los fallos sin solaparse y se lee como una política explícita.

3. **Cuidado con las fusiones de errores.** `MergeErrorsDetailsIfFail` y `FusionErrosIfExists` mezclan los `Details`: un solo fallo con excepción convierte todo el conjunto en «técnico». Si necesitas conservar la clasificación, decide antes de fusionar.

4. **Aprovecha que `func` puede devolver un fallo.** No estás obligado a recuperar: es el punto perfecto para traducir mensajes internos a mensajes de usuario sin tocar los errores técnicos.

5. **No lo uses para logging.** Si solo quieres observar, usa [`ExecSelfIfFailWithoutException`](../ExecSelf/6_ExecSelfIfFailWithoutException.md): deja claro que no alteras el flujo.

6. **Usa `Try*` si la recuperación toca infraestructura.** Y recuerda el efecto colateral útil: si captura una excepción, el fallo pasa a ser técnico y ningún `BindIfFailWithoutException` posterior lo reintentará.

7. **No hay variante de dos caminos.** Si necesitas transformar también el caso válido, encadena un `Map`/`Bind` después, o usa `Match` al final del *pipeline*.

---

## Resumen

- `BindIfFailWithoutException` ejecuta tu función **solo si el fallo NO lleva excepción** en `Details["Ex"]`, es decir, solo para los fallos de negocio.
- Es el complemento exacto de `BindIfFailWithException`: entre ambos cubren todos los fallos posibles sin solaparse.
- Tiene **una sola forma**: `BindIfFailWithoutException<T>(Func<MlErrorsDetails, MlResult<T>>)`. Recibe los detalles completos.
- Si hay excepción, **el fallo se devuelve intacto**; si el resultado era válido, el valor se devuelve intacto.
- `func` puede recuperar (volver al camino válido) o simplemente **reescribir** el fallo de negocio con un mensaje más claro.
- Las variantes `Try*` protegen la recuperación; al capturar, marcan el fallo como técnico y evitan reintentos en cascada.
- Su uso canónico es la partición **negocio ⇒ 4xx** / **técnico ⇒ 5xx** en la capa de presentación.

---

## Ver también

- [`8_BindIfFailWithException.md`](8_BindIfFailWithException.md) — la mitad complementaria: actuar solo si hay excepción.
- [`6_BindIfFail.md`](6_BindIfFail.md) — recuperación sin distinguir la causa del fallo.
- [`7_BindIfFailWithValue.md`](7_BindIfFailWithValue.md) — recuperación usando el valor que provocó el fallo.
- [`3_Bind.md`](3_Bind.md) — encadenamiento básico y `TryBind`, origen habitual de `Details["Ex"]`.
- [`../ExecSelf/6_ExecSelfIfFailWithoutException.md`](../ExecSelf/6_ExecSelfIfFailWithoutException.md) — el mismo filtro para efectos laterales.
- [`../Map/7_MapIfFailWithoutException.md`](../Map/7_MapIfFailWithoutException.md) — cuando la recuperación no puede fallar.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailException`, `MergeErrorsDetailsIfFail`, `AddValueIfFail`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — claves convencionales `Ex`, `Ex2`, `Value` y factorías de `MlErrorsDetails`.