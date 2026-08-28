# MapAlways — Producir un resultado con independencia del estado

## Índice

1. [Introducción](#introducción)
2. [Las dos formas de `MapAlways`](#las-dos-formas-de-mapalways)
3. [Forma A: `Func<TReturn>` — descartar por completo el estado anterior](#forma-a-functreturn--descartar-por-completo-el-estado-anterior)
4. [⚠️ Particularidad real del código fuente: la sobrecarga que no espera al origen](#️-particularidad-real-del-código-fuente-la-sobrecarga-que-no-espera-al-origen)
5. [Forma B: `funcValidAlways` + `funcFailAlways` — convergencia de ramas](#forma-b-funcvalidalways--funcfailalways--convergencia-de-ramas)
6. [`MapAlways` frente a `Match`](#mapalways-frente-a-match)
7. [Métodos con implementación idéntica en toda la librería](#métodos-con-implementación-idéntica-en-toda-la-librería)
8. [`TryMapAlways` — cuando el delegado puede lanzar](#trymapalways--cuando-el-delegado-puede-lanzar)
9. [Variantes asíncronas](#variantes-asíncronas)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`MapAlways` es la operación que **cierra** una tubería produciendo un valor **sin importar** si el `MlResult` de entrada era válido o fallido. Es el equivalente al bloque `finally` de un `try/catch`, pero devolviendo un valor en lugar de ejecutando efectos.

Tiene **dos formas muy distintas** que conviene no confundir:

```csharp
// Forma A: ignora completamente el estado anterior
MlResult<Informe> r1 = tuberia.MapAlways(() => GenerarInformeFinal());

// Forma B: cada rama produce su propio resultado, ambas hacia el mismo tipo
MlResult<RespuestaDto> r2 = tuberia.MapAlways(
                                funcValidAlways: pedido  => RespuestaDto.Ok(pedido.Id),
                                funcFailAlways : errores => RespuestaDto.Error(errores.ToErrorsMessages()));
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## Las dos formas de `MapAlways`

| | Forma A | Forma B |
|---|---|---|
| Firma del delegado | `Func<TReturn>` (sin parámetros) | `Func<T, TResult>` + `Func<MlErrorsDetails, TResult>` |
| ¿Accede al valor válido? | ❌ No | ✅ Sí (`funcValidAlways`) |
| ¿Accede a los errores? | ❌ No | ✅ Sí (`funcFailAlways`) |
| ¿Conserva los errores previos? | ❌ No, se descartan | ❌ No, pero los recibes para transformarlos |
| Resultado | Siempre `Valid(funcAlways())` | `Valid` de la rama correspondiente |
| Uso típico | valor constante, recarga total, cierre de bloque | convergencia a un DTO de respuesta |

🔑 **Las dos formas siempre devuelven un `MlResult` válido** (salvo que el delegado lance en las variantes `Try`). `MapAlways` es, por definición, el punto donde el carril de error **desaparece**.

---

## Forma A: `Func<TReturn>` — descartar por completo el estado anterior

El código fuente es literalmente esta línea:

```csharp
public static MlResult<TReturn> MapAlways<T, TReturn>(this MlResult<T>   source,
                                                           Func<TReturn> funcAlways)
    => funcAlways();
```

Es importante entender lo que **no** hace:

- **No** llama a `source.Match(...)`.
- **No** consulta `source.IsValid`.
- **No** conserva ni un solo error ni detalle del origen.
- El parámetro `source` existe **únicamente** para que el método pueda escribirse como extensión y encajar en una cadena fluida.

```csharp
var resultado = ValidarPedido(dto)          // pongamos que falla con 3 errores
                    .BindAsync(...)          // no se ejecuta
                    .MapAlways(() => "listo"); // → Valid("listo"): los 3 errores se han perdido
```

⚠️ **Esto es intencionado, pero peligroso.** Si lo usas en medio de una tubería, silencias todos los fallos anteriores sin dejar rastro. Usa Forma A solo cuando:

1. Estés al final de la tubería y ya hayas registrado los errores (por ejemplo con un `ExecSelfIfFail` previo), **o**
2. El valor que produces sea realmente independiente del resultado anterior (recargar un estado completo, devolver un acuse de recibo, etc.).

```csharp
// ✅ Patrón correcto: registrar primero, descartar después
var acuse = await ProcesarLoteAsync(lote)
                    .ExecSelfIfFailAsync(err => _log.LogErrorAsync(err.ToErrorsDescription()))
                    .MapAlwaysAsync(() => _repo.RecargarEstadoLoteAsync(lote.Id));
```

---

## ⚠️ Particularidad real del código fuente: la sobrecarga que no espera al origen

Dos de las sobrecargas asíncronas de la Forma A tienen una implementación que conviene conocer:

```csharp
// ✅ Esta espera el origen antes de invocar el delegado
public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                            Func<TReturn>     funcAlways)
    => (await sourceAsync).MapAlways(funcAlways);

// ⚠️ Esta NO espera el origen: ejecuta el delegado directamente
public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(this Task<MlResult<T>>   sourceAsync,
                                                                            Func<Task<TReturn>> funcAlwaysAsync)
    => await funcAlwaysAsync();       // sourceAsync nunca se hace await
```

Consecuencias prácticas de la segunda sobrecarga:

- La tarea de origen **no se espera**: el delegado puede empezar antes de que la operación previa haya terminado, o incluso ejecutarse en paralelo con ella.
- Si `sourceAsync` lanza una excepción, esa excepción queda **sin observar** (`UnobservedTaskException`).
- El orden de ejecución no está garantizado.

> 💡 **Recomendación práctica:** si el origen es asíncrono y necesitas garantizar que ha terminado antes de producir el valor final, **haz el `await` explícito** antes de llamar a `MapAlways`:
>
> ```csharp
> // ✅ Orden garantizado
> var previo = await ProcesarAsync(dto);
> var final  = await previo.MapAlwaysAsync(() => GenerarAcuseAsync());
>
> // ✅ Alternativa: usa la sobrecarga con delegado síncrono, que sí espera el origen
> var final2 = await ProcesarAsync(dto).MapAlwaysAsync(() => GenerarAcuse());
> ```

Este comportamiento **no** afecta a la Forma B ni a las variantes `TryMapAlways`, que sí usan `Match`/`MatchAsync` sobre el origen y por tanto lo esperan siempre.

---

## Forma B: `funcValidAlways` + `funcFailAlways` — convergencia de ramas

Esta es la forma útil en el 90 % de los casos reales. El propio autor la documenta así en el código fuente:

> *"Ejecutará la acción, utilizando cada una de las funciones, según el estado del MlResult. Necesita 2 funciones: 1.- Función que se ejecutará si el MlResult es válido (facilita el value valido por parámetros) 2.- Función que se ejecutará si el MlResult es fallido (facilita el errorDetails por parámetros)"*

```csharp
public static MlResult<TResult> MapAlways<T, TResult>(this MlResult<T>                    source,
                                                           Func<T              , TResult> funcValidAlways,
                                                           Func<MlErrorsDetails, TResult> funcFailAlways)
    => source.Match(valid: funcValidAlways,
                    fail : funcFailAlways);
```

Ambas ramas producen un `TResult`, que se envuelve como `MlResult<TResult>` válido mediante conversión implícita. El resultado **nunca es un fallo**: has traducido el error a un valor legítimo de tu dominio.

```csharp
public async Task<ResumenImportacion> ImportarAsync(Stream fichero)
    => (await LeerFicheroAsync(fichero)
                .BindAsync(ValidarFilasAsync)
                .BindAsync(GuardarAsync)
                .MapAlwaysAsync(
                    funcValidAlwaysAsync: r   => ResumenImportacion.Correcta(r.Filas).ToAsync(),
                    funcFailAlwaysAsync : err => ResumenImportacion.Fallida(err.ToErrorsMessages()).ToAsync()))
        .Match(valid: x => x, fail: _ => ResumenImportacion.Desconocida);
```

---

## `MapAlways` frente a `Match`

La Forma B y `Match` hacen esencialmente lo mismo. La diferencia está en el tipo de retorno:

| | `Match` | `MapAlways` (Forma B) |
|---|---|---|
| Devuelve | `TReturn` **desnudo** | `MlResult<TResult>` |
| ¿Sigues en el carril? | ❌ No, sales del `MlResult` | ✅ Sí, puedes seguir encadenando |
| Uso típico | último paso: convertir a `IActionResult`, a un valor, etc. | paso intermedio: normalizar y continuar |

```csharp
// Match: salgo del carril, obtengo un IActionResult
IActionResult respuesta = resultado.Match(
                              valid: c => Ok(c),
                              fail : e => BadRequest(e.ToErrorsMessages()));

// MapAlways: sigo en el carril y puedo encadenar más pasos
MlResult<Informe> informe = resultado.MapAlways(
                                funcValidAlways: c => Informe.DeCliente(c),
                                funcFailAlways : e => Informe.DeError(e.ToErrorsDescription()))
                                     .Map(i => i.ConMarcaTemporal(DateTime.UtcNow))
                                     .MapEnsure(i => i.EsPublicable, "El informe no es publicable");
```

📌 Regla práctica: si después de unificar las ramas todavía te queda trabajo dentro del carril, usa `MapAlways`; si es el punto final de salida, usa `Match`.

---

## Métodos con implementación idéntica en toda la librería

Un dato que ayuda a entender el diseño: la Forma A de `MapAlways` es **exactamente el mismo cuerpo** que otras dos APIs de la librería:

```csharp
MapAlways<T, TReturn>(source, Func<TReturn> funcAlways)  => funcAlways();
BindAlways<T, TReturn>(source, Func<TReturn> funcAlways) => funcAlways();   // ../Bind/10_BindAlways.md
Match<T, TReturn>(source, Func<TReturn> funcAll)         => funcAll();      // ../Match/2_MatchAll.md
```

Las tres ignoran por completo el `source`. Existen como nombres distintos por **legibilidad de intención** dentro de una cadena, no por diferencia de comportamiento. Elige el nombre que mejor comunique lo que estás haciendo en ese punto:

- `MapAlways` → "produzco un valor final, pase lo que pase".
- `BindAlways` → "invoco otra operación que devuelve `MlResult`, pase lo que pase".
- `Match(funcAll)` → "colapso el resultado a un valor único".

---

## `TryMapAlways` — cuando el delegado puede lanzar

A diferencia de la Forma A base, `TryMapAlways` **sí consulta el estado del origen**, porque en la rama fallida pasa los `errorDetails` al mecanismo de captura:

```csharp
public static MlResult<TReturn> TryMapAlways<T, TReturn>(this MlResult<T>             source,
                                                              Func<TReturn>           funcAlways,
                                                              Func<Exception, string> errorMessageBuilder)
    => source.Match(valid: _            => funcAlways.TryToMlResult(errorMessageBuilder),
                    fail : errorDetails => funcAlways.TryToMlResult(errorDetails, errorMessageBuilder));
```

🔑 Detalle valioso: en la rama fallida se invoca la sobrecarga `TryToMlResult(errorDetails, ...)`, que **arrastra los errores originales**. Es decir, si el origen venía en fallo **y** el delegado lanza, el `MlResult` resultante contiene **ambos** conjuntos de errores. Si el delegado no lanza, se devuelve su valor como válido y los errores previos se descartan.

La Forma B protegida hace lo análogo con los dos delegados:

```csharp
public static MlResult<TResult> TryMapAlways<T, TResult>(this MlResult<T>                    source,
                                                              Func<T              , TResult> funcValidAlways,
                                                              Func<MlErrorsDetails, TResult> funcFailAlways,
                                                              Func<Exception, string>        errorMessageBuilder)
    => source.Match(valid: value        => funcValidAlways.TryToMlResult(value       , errorMessageBuilder),
                    fail : errorDetails => funcFailAlways .TryToMlResult(errorDetails, errorMessageBuilder));
```

Cada variante tiene dos formas de indicar el mensaje de error: un `string` fijo o un `Func<Exception, string>` que puede inspeccionar la excepción capturada.

```csharp
var resumen = resultadoProceso.TryMapAlways(
                  funcValidAlways: r   => ConstruirResumen(r),          // puede lanzar
                  funcFailAlways : err => ConstruirResumenError(err),   // puede lanzar
                  errorMessageBuilder: ex => $"No se pudo construir el resumen: {ex.Message}");
```

---

## Variantes asíncronas

Ambas formas cubren las combinaciones habituales de origen y delegados:

| Forma | Origen | Delegado(s) | Método |
|---|---|---|---|
| A | `MlResult<T>` | `Func<Task<TReturn>>` | `MapAlwaysAsync` |
| A | `Task<MlResult<T>>` | `Func<Task<TReturn>>` | `MapAlwaysAsync` ⚠️ *(no espera el origen)* |
| A | `Task<MlResult<T>>` | `Func<TReturn>` | `MapAlwaysAsync` |
| B | `MlResult<T>` | ambos asíncronos | `MapAlwaysAsync` |
| B | `Task<MlResult<T>>` | ambos asíncronos | `MapAlwaysAsync` |
| B | `Task<MlResult<T>>` | válido sincrónico + fallido asíncrono | `MapAlwaysAsync` |
| B | `Task<MlResult<T>>` | válido asíncrono + fallido sincrónico | `MapAlwaysAsync` |
| B | `Task<MlResult<T>>` | ambos sincrónicos | `MapAlwaysAsync` |

📌 Las combinaciones mixtas se resuelven internamente homogeneizando el delegado síncrono con `funcValidAlways.ToFuncTask()`. Puedes usar la misma técnica en tu código, o envolver un valor con `.ToAsync()`.

Existe la batería equivalente de `TryMapAlwaysAsync` para las dos formas, con las dos maneras de expresar el mensaje de error.

---

## Tabla de decisión rápida

| Necesito… | Método |
|---|---|
| Un valor **constante o independiente** al final de la tubería | `MapAlways(Func<TReturn>)` (Forma A) |
| **Unificar** éxito y fallo en un tipo común y **seguir encadenando** | `MapAlways(funcValid, funcFail)` (Forma B) |
| Unificar éxito y fallo y **salir** del `MlResult` | [`Match`](../Match/1_Match.md) |
| Lo mismo pero el delegado devuelve `MlResult` | [`BindAlways`](../Bind/10_BindAlways.md) |
| Recuperarme **solo** de los fallos | [`MapIfFail`](4_MapIfFail.md) |
| Recuperarme solo de los fallos **técnicos** | [`MapIfFailWithException`](6_MapIfFailWithException.md) |
| Recuperarme solo de los fallos **de negocio** | [`MapIfFailWithoutException`](7_MapIfFailWithoutException.md) |
| Ejecutar un **efecto** siempre, sin cambiar el resultado | [`ExecSelf`](../ExecSelf/1_ExecSelf.md) |
| Proteger un delegado que puede lanzar | `TryMapAlways` |

---

## Ejemplos Prácticos

### Ejemplo 1: Respuesta HTTP uniforme sin salir del carril

```csharp
public async Task<IActionResult> Crear(CrearPedidoDto dto)
{
    var respuesta = await ValidarAsync(dto)
                            .BindAsync(_servicio.CrearAsync)
                            .ExecSelfIfFailAsync(err => _log.LogWarningAsync(err.ToErrorsDescription()))
                            .MapAlwaysAsync(
                                funcValidAlwaysAsync: p   => ApiRespuesta<int>.Ok(p.Id).ToAsync(),
                                funcFailAlwaysAsync : err => ApiRespuesta<int>
                                                                .Error(err.ToErrorsMessages())
                                                                .ToAsync())
                            // Seguimos en el carril: añadimos metadatos comunes
                            .MapAsync(r => (r with { Version = "v2", Marca = DateTime.UtcNow }).ToAsync());

    return respuesta.Match(valid: r => Ok(r), fail: _ => StatusCode(500));
}
```

La ventaja de `MapAlways` frente a `Match` aquí es que el enriquecimiento con `Version` y `Marca` se escribe **una sola vez**, después de la convergencia, en lugar de duplicarlo en las dos ramas.

### Ejemplo 2: Auditoría que se escribe pase lo que pase

```csharp
public async Task<MlResult<RegistroAuditoria>> EjecutarConAuditoriaAsync(string operacion, Func<Task<MlResult<Unit>>> accion)
{
    var inicio    = DateTime.UtcNow;
    var resultado = await accion();

    return await resultado.MapAlwaysAsync(
        funcValidAlwaysAsync: async _ =>
        {
            var reg = RegistroAuditoria.Correcto(operacion, inicio, DateTime.UtcNow);
            await _auditoria.GuardarAsync(reg);
            return reg;
        },
        funcFailAlwaysAsync: async err =>
        {
            var esTecnico = err.GetDetailException().IsValid;   // ¿había excepción en Details["Ex"]?
            var reg = RegistroAuditoria.Fallido(operacion, inicio, DateTime.UtcNow,
                                                err.ToErrorsDescription(),
                                                esTecnico ? "TECNICO" : "NEGOCIO");
            await _auditoria.GuardarAsync(reg);
            return reg;
        });
}
```

Fíjate en `err.GetDetailException().IsValid` como forma idiomática de preguntar "¿este fallo es técnico?".

### Ejemplo 3: Recarga total de estado (Forma A bien usada)

```csharp
// Tras intentar aplicar una tanda de cambios, devolvemos el estado real del agregado,
// sea cual sea el resultado de la tanda: la fuente de verdad es la base de datos.
public async Task<MlResult<EstadoCarrito>> AplicarCambiosAsync(Guid carritoId, IEnumerable<Cambio> cambios)
    => await AplicarUnoAUnoAsync(carritoId, cambios)
                 .ExecSelfIfFailAsync(err => _log.LogWarningAsync(
                     "Algunos cambios del carrito {Id} no se aplicaron: {D}", carritoId, err.ToErrorsDescription()))
                 // El delegado es síncrono en su firma de entrada → esta sobrecarga SÍ espera el origen
                 .MapAlwaysAsync(() => _repo.LeerEstadoAsync(carritoId));
```

⚠️ Nótese el comentario: se ha elegido deliberadamente el punto de la cadena y se ha registrado el fallo **antes** de descartarlo.

### Ejemplo 4: Semáforo de salud de un servicio

```csharp
public async Task<Semaforo> ComprobarSaludAsync()
{
    var comprobaciones = await ComprobarBdAsync()
                                 .BindAsync(_ => ComprobarColaAsync())
                                 .BindAsync(_ => ComprobarCacheAsync());

    var semaforo = await comprobaciones.MapAlwaysAsync(
        funcValidAlwaysAsync: _   => Semaforo.Verde.ToAsync(),
        funcFailAlwaysAsync : err => (err.GetDetailException().IsValid
                                          ? Semaforo.Rojo(err.ToErrorsMessages())      // avería
                                          : Semaforo.Ambar(err.ToErrorsMessages()))    // degradación de negocio
                                     .ToAsync());

    return semaforo.Match(valid: s => s, fail: _ => Semaforo.Desconocido);
}
```

### Ejemplo 5: Qué **no** hacer

```csharp
// ❌ MAL: MapAlways en medio de la tubería borra los errores silenciosamente
var r1 = ValidarPedido(dto)
             .MapAlways(() => dto)          // los errores de validación desaparecen sin log
             .Bind(GuardarPedido);          // ¡guardamos un pedido inválido!

// ❌ MAL: usar la Forma A cuando en realidad necesitas los datos
var r2 = ObtenerCliente(id)
             .MapAlways(() => "OK");        // se pierde el cliente y también el motivo del fallo

// ❌ MAL: sobrecarga asíncrona que no espera el origen cuando el orden importa
var r3 = await GuardarEnBdAsync(entidad)
                   .MapAlwaysAsync(() => LeerDeBdAsync(entidad.Id));   // puede leer ANTES de guardar

// ✅ BIEN: registrar antes de descartar, y garantizar el orden con un await explícito
var guardado = await GuardarEnBdAsync(entidad);
var r4 = await guardado
                 .ExecSelfIfFailAsync(err => _log.LogErrorAsync(err.ToErrorsDescription()))
                 .MapAlwaysAsync(() => LeerDeBd(entidad.Id));   // delegado síncrono → espera el origen

// ✅ BIEN: si necesitas los datos de cada rama, usa la Forma B
var r5 = ObtenerCliente(id)
             .MapAlways(funcValidAlways: c   => FichaDto.De(c),
                        funcFailAlways : err => FichaDto.NoDisponible(err.ToErrorsMessages()));
```

---

## Mejores Prácticas

1. **Usa la Forma B casi siempre.** La Forma A tira a la basura toda la información del resultado anterior; la Forma B te obliga a decidir qué hacer con cada rama, que es lo que normalmente quieres.

2. **Registra los errores antes de descartarlos.** Un `ExecSelfIfFail` inmediatamente antes de un `MapAlways` de Forma A es el patrón que evita fallos invisibles.

3. **Coloca `MapAlways` al final de la tubería**, no en el medio. En el medio convierte un fallo en éxito y las operaciones posteriores trabajarán sobre datos que nunca se validaron.

4. **Cuidado con `MapAlwaysAsync(Task<MlResult<T>>, Func<Task<TReturn>>)`**: no espera al origen. Si el orden importa, haz `await` explícito del origen o usa la sobrecarga con delegado síncrono.

5. **Elige entre `MapAlways` y `Match` según si sigues en el carril.** `MapAlways` devuelve `MlResult<TResult>` (encadenable); `Match` devuelve el valor desnudo (salida definitiva).

6. **Si el delegado devuelve un `MlResult`, usa `BindAlways`, no `MapAlways`.** De lo contrario obtendrás el anidamiento `MlResult<MlResult<T>>`.

7. **Protege con `TryMapAlways` los delegados frágiles** (serialización, E/S, cálculos con datos parciales). Recuerda que en la rama fallida los errores originales **se arrastran** si el delegado lanza.

8. **Distingue fallo técnico de fallo de negocio dentro de `funcFailAlways`** con `err.GetDetailException().IsValid`. Así una sola llamada a `MapAlways` puede producir respuestas diferenciadas (503 vs. 400, rojo vs. ámbar).

9. **No uses `MapAlways` para efectos secundarios.** Si solo quieres registrar o notificar sin cambiar el resultado, la herramienta correcta es [`ExecSelf`](../ExecSelf/1_ExecSelf.md).

---

## Resumen

- `MapAlways` produce un valor **con independencia** del estado del `MlResult` de entrada; es el punto donde el carril de error se cierra.
- Tiene **dos formas**: A `Func<TReturn>` (ignora por completo el origen: su cuerpo es literalmente `=> funcAlways();`) y B `(funcValidAlways, funcFailAlways)` (delega en `source.Match` y da acceso al valor y a los errores).
- La Forma A **descarta todos los errores sin dejar rastro**: regístralos antes con `ExecSelfIfFail`.
- ⚠️ La sobrecarga `MapAlwaysAsync(Task<MlResult<T>>, Func<Task<TReturn>>)` **no hace `await` del origen**; si el orden importa, espera el origen explícitamente o usa la sobrecarga con delegado síncrono.
- La Forma A comparte cuerpo exacto con [`BindAlways(funcAlways)`](../Bind/10_BindAlways.md) y con [`Match(funcAll)`](../Match/2_MatchAll.md): son alias por legibilidad.
- Frente a `Match`, `MapAlways` devuelve `MlResult<TResult>` y por tanto **permite seguir encadenando**.
- `TryMapAlways` protege los delegados con `TryToMlResult` y, en la rama fallida, **arrastra los errores originales** si el delegado lanza.
- Ambas formas tienen la batería completa de variantes asíncronas, incluidas combinaciones mixtas resueltas con `ToFuncTask()`.

---

## Ver también

- [`7_MapIfFailWithoutException.md`](7_MapIfFailWithoutException.md) — recuperación selectiva de fallos de negocio.
- [`6_MapIfFailWithException.md`](6_MapIfFailWithException.md) — recuperación selectiva de fallos técnicos.
- [`4_MapIfFail.md`](4_MapIfFail.md) — recuperación de cualquier fallo, conservando el tipo.
- [`1_Map.md`](1_Map.md) — la operación base de transformación.
- [`../Bind/10_BindAlways.md`](../Bind/10_BindAlways.md) — la versión `Bind`, cuando el delegado devuelve `MlResult`.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir del carril con un valor desnudo.
- [`../Match/2_MatchAll.md`](../Match/2_MatchAll.md) — la sobrecarga `Match(Func<TReturn>)`, gemela de la Forma A.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — efectos secundarios en ambas ramas sin alterar el resultado.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la familia `Map`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — estructura real de `MlError` y `MlErrorsDetails`.