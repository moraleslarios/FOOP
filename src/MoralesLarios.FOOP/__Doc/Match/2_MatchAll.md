# MlResult MatchAll - Ejecución Incondicional con Transformación

> ⚠️ **Aviso importante sobre los nombres.** En el código fuente existe una región llamada
> `#region MatchAll` dentro de `Types/MlResultActionsMatch.cs`, pero **no existe ningún método público
> llamado `MatchAll`**. Los métodos de esa región se llaman igualmente
> `Match` / `MatchAsync` / `TryMatch` / `TryMatchAsync`; lo que los distingue es su **firma**: reciben
> un único delegado **sin parámetros** (`Func<TReturn> funcAll`) y devuelven `MlResult<TReturn>`.
> A lo largo de este documento hablamos de «la sobrecarga *match-all*» para referirnos a ellos.

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos MatchAll Básicos](#métodos-matchall-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryMatchAll - Captura de Excepciones](#métodos-trymatchall---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Match y ExecSelf](#comparación-con-match-y-execself)

---

## Introducción

La sobrecarga *match-all* de `Match` proporciona un patrón de **ejecución incondicional con
transformación**: ejecuta una función independientemente del estado del `MlResult<T>` (válido o fallido)
y devuelve un `MlResult<TReturn>` **nuevo**, desligado del estado anterior.

### Propósito Principal

- **Ejecución Independiente del Estado**: la función se ejecuta sin importar si el resultado es válido o fallido.
- **Transformación Consistente**: convierte cualquier entrada en `MlResult<TReturn>`.
- **Operaciones de Finalización**: ideal para *cleanup*, cierre de sesión, generación de informes o telemetría de cierre.
- **Reset de Contexto**: crear un resultado nuevo cuando el estado anterior ya no aporta información útil.

### Cómo distinguirla de la sobrecarga clásica

| Sobrecarga | Firma | Devuelve |
| --- | --- | --- |
| Clásica (dos ramas) | `Match<T, TReturn>(Func<T, TReturn> valid, Func<MlErrorsDetails, TReturn> fail)` | `TReturn` **crudo** (sale del `MlResult`) |
| *match-all* | `Match<T, TReturn>(Func<TReturn> funcAll)` | `MlResult<TReturn>` (**sigue** en la tubería) |

El compilador resuelve una u otra según el número de delegados que le pases, así que no hay ambigüedad
en el código real.

---

## Análisis de los Métodos

### Filosofía de la sobrecarga *match-all*

```
MlResult<T> (cualquier estado) → Match(funcAll) → MlResult<TReturn>
                ↓                      ↓                    ↓
     Válido/Fallido        → funcAll() →       nuevo resultado válido
```

Implementación real (simplificada):

```csharp
public static MlResult<TReturn> Match<T, TReturn>(this MlResult<T> source,
                                                  Func<TReturn>    funcAll)
    => funcAll();
```

Literalmente **ignora `source`**. El valor devuelto se convierte en `MlResult<TReturn>` gracias a la
conversión implícita del tipo.

### Características Principales

1. **Ejecución Incondicional**: se ejecuta siempre, sin importar el estado.
2. **Ignora la Entrada**: no recibe el valor ni los errores del `MlResult<T>` original.
3. **Nuevo Contexto**: genera un `MlResult<TReturn>` completamente nuevo.
4. **Reset de Estado**: los errores previos **se pierden**. Si los necesitas, regístralos antes con
   `ExecSelfIfFail` o consérvalos con `MergeErrorsDetailsIfFail`.
5. **Transformación Total**: cambia tanto el tipo como el contexto del resultado.

> ⚠️ El punto 4 es el riesgo principal: si usas esta sobrecarga en medio de una tubería, **enmascaras
> los fallos anteriores**. Úsala de forma consciente, normalmente al final del flujo.

---

## Métodos MatchAll Básicos

### `Match<T, TReturn>(Func<TReturn> funcAll)`

**Propósito**: ejecutar una función independientemente del estado del resultado y crear un nuevo
`MlResult<TReturn>`.

```csharp
public static MlResult<TReturn> Match<T, TReturn>(this MlResult<T> source,
                                                  Func<TReturn>    funcAll)
```

**Comportamiento**:
- Ignora por completo el estado y el contenido de `source`.
- Ejecuta `funcAll()` incondicionalmente.
- Devuelve un `MlResult<TReturn>` válido con el valor obtenido.

**Ejemplo Básico**:
```csharp
MlResult<Pedido> resultado = ProcesarPedido(dto);   // puede ser válido o fallido

// Cerramos el flujo con un mensaje único, sea cual sea el resultado.
MlResult<string> cierre = resultado.Match(() => "Proceso finalizado");
// Siempre válido: "Proceso finalizado"
```

**Ejemplo más útil: liberar recursos y devolver un acuse**

```csharp
MlResult<Acuse> acuse = await _importador.ImportarAsync(fichero)
        // Dejamos rastro del fallo ANTES de perderlo.
        .ExecSelfIfFailAsync(er => _log.LogWarning("Importación fallida: {E}",
                                                  er.ToErrorsDescription()))
        // Y cerramos siempre igual: el cliente recibe un acuse de recepción.
        .MatchAsync(() => new Acuse(fichero.Nombre, _reloj.Ahora));
```

---

## Variantes Asíncronas

### `MatchAsync` con función asíncrona

```csharp
public static Task<MlResult<TReturn>> MatchAsync<T, TReturn>(
    this MlResult<T>     source,
    Func<Task<TReturn>>  funcAllAsync)
```

**Ejemplo**:
```csharp
MlResult<Datos> resultado = await ProcesarAsync(datos);

MlResult<Notificacion> aviso = await resultado.MatchAsync(
    async () => await _notificaciones.EnviarCierreAsync());
```

### `MatchAsync` con fuente asíncrona

```csharp
// Fuente asíncrona + función asíncrona
public static Task<MlResult<TReturn>> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<Task<TReturn>>    funcAllAsync)

// Fuente asíncrona + función síncrona
public static Task<MlResult<TReturn>> MatchAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<TReturn>          funcAll)
```

**Ejemplo** (sin `await` intermedios, la tubería no se rompe):
```csharp
MlResult<Resumen> resumen = await EjecutarProcesoLargoAsync()
        .MatchAsync(async () => await GenerarResumenAsync());
```

En total hay **cuatro** sobrecargas asíncronas, combinando fuente (síncrona / `Task`) y delegado
(síncrono / asíncrono).

---

## Métodos TryMatchAll - Captura de Excepciones

### `TryMatch<T, TReturn>(Func<TReturn> funcAll, ...)`

```csharp
// Mensaje construido a partir de la excepción
public static MlResult<TReturn> TryMatch<T, TReturn>(
    this MlResult<T>        source,
    Func<TReturn>           funcAll,
    Func<Exception, string> errorMessageBuilder)

// Mensaje fijo
public static MlResult<TReturn> TryMatch<T, TReturn>(
    this MlResult<T> source,
    Func<TReturn>    funcAll,
    string           exceptionAditionalMessage = null!)
```

**Comportamiento**:
- Ejecuta `funcAll()` incondicionalmente.
- Si lanza una excepción, devuelve un `MlResult<TReturn>` fallido **con la excepción guardada** en
  `Details["Ex"]`, recuperable después con `GetDetailException()`.
- Si termina bien, devuelve un `MlResult<TReturn>` válido.

**Detalle de implementación relevante**: internamente conserva los errores previos cuando `source` venía
fallido, porque delega en `funcAll.TryToMlResult(errorDetails, errorMessageBuilder)`. Es decir,
`TryMatch` es **menos destructivo** que `Match` en su variante *match-all*.

**Ejemplo**:
```csharp
MlResult<Datos> resultado = ProcesarDatos(entrada);

MlResult<Informe> informe = resultado.TryMatch(
    () => GenerarInformeComplejo(),                  // puede lanzar
    ex => $"No se pudo generar el informe: {ex.Message}");
```

### Versiones Asíncronas de `TryMatch`

```csharp
// Fuente síncrona + función asíncrona
public static Task<MlResult<TReturn>> TryMatchAsync<T, TReturn>(
    this MlResult<T>        source,
    Func<Task<TReturn>>     funcAllAsync,
    Func<Exception, string> errorMessageBuilder)

// Fuente asíncrona + función asíncrona
public static Task<MlResult<TReturn>> TryMatchAsync<T, TReturn>(
    this Task<MlResult<T>>  sourceAsync,
    Func<Task<TReturn>>     funcAllAsync,
    Func<Exception, string> errorMessageBuilder)
```

Cada una tiene además su gemela con `string exceptionAditionalMessage`, lo que da **cuatro**
sobrecargas asíncronas de `TryMatch` en esta región.

---

## Ejemplos Prácticos

### Ejemplo 1: Auditoría de cierre de una operación

Queremos que **toda** ejecución deje un registro de auditoría, tanto si salió bien como si falló, y que
el método devuelva el identificador del registro creado.

```csharp
public class ServicioPedidos
{
    private readonly IAuditoria _auditoria;
    private readonly ILogger<ServicioPedidos> _log;

    public async Task<MlResult<Guid>> ProcesarConAuditoriaAsync(PedidoDto dto)
        => await ProcesarAsync(dto)

            // 1. Antes de perder el contexto, registramos qué pasó realmente.
            .ExecSelfIfValidAsync(p  => _log.LogInformation("Pedido {Id} procesado", p.Id))
            .ExecSelfIfFailAsync (er => _log.LogWarning("Pedido rechazado: {E}",
                                                       er.ToErrorsDescription()))

            // 2. Sea cual sea el estado, creamos SIEMPRE el registro de auditoría
            //    y devolvemos su Id. TryMatch protege la escritura en base de datos.
            .TryMatchAsync(
                async () => await _auditoria.RegistrarAsync("ProcesarPedido", _reloj.Ahora),
                ex => $"No se pudo escribir la auditoría: {ex.Message}");
}
```

Puntos a observar:

- `ExecSelf*` **no cambia** el resultado: solo observa. Es el sitio correcto para el log.
- La sobrecarga *match-all* de `TryMatch` **descarta** el `Pedido` y devuelve un `Guid`.
- Si la auditoría falla, el resultado final es `Fail` con la excepción en `Details["Ex"]`.

### Ejemplo 2: Informe de finalización de un proceso por lotes

```csharp
public Task<MlResult<InformeLote>> EjecutarLoteAsync(IEnumerable<Factura> facturas) =>
    facturas.ProjectionAsync(_emisor.EmitirAsync)      // Task<MlResult<IEnumerable<Emision>>>

        // Guardamos el detalle del fallo por si el informe lo necesita.
        .ExecSelfIfFailAsync(er => _incidencias.Anotar(er.ToErrorsDescription()))

        // El informe se genera SIEMPRE: si el lote falló, describe el fallo.
        .MatchAsync(async () => await _informes.GenerarCierreLoteAsync(_reloj.Ahora));
```

> 💡 Si necesitas que el informe distinga entre éxito y fallo, **no uses la sobrecarga *match-all***:
> usa la clásica de dos ramas, que sí recibe el valor y los errores.
>
> ```csharp
> InformeLote informe = resultadoLote.Match(
>     valid: emisiones => InformeLote.Correcto(emisiones.Count()),
>     fail : errores   => InformeLote.ConErrores(errores.ToErrorsMessages()));
> ```

### Ejemplo 3: Reinicio de contexto para una operación independiente

Hay flujos en los que el resultado anterior es irrelevante para el siguiente paso: por ejemplo, un
proceso de limpieza que debe ejecutarse aunque la operación principal haya fallado.

```csharp
public async Task<MlResult<int>> SincronizarYLimpiarAsync()
{
    return await _sincronizador.SincronizarAsync()

        // Nos interesa saberlo, pero no debe impedir la limpieza.
        .ExecSelfIfFailAsync(er => _log.LogError("Sincronización fallida: {E}",
                                                er.ToErrorsDescription()))

        // Contexto nuevo: devolvemos el número de ficheros temporales eliminados,
        // con independencia de cómo terminara la sincronización.
        .TryMatchAsync(async () => await _limpieza.BorrarTemporalesAsync(),
                       ex => $"No se pudo limpiar el directorio temporal: {ex.Message}");
}
```

### Ejemplo 4: Conservar los errores previos

Si quieres reiniciar el contexto **pero sin perder** el diagnóstico anterior, combina la sobrecarga
*match-all* con `MergeErrorsDetailsIfFail` o guarda los errores explícitamente:

```csharp
MlResult<Acuse> resultado = await _proceso.EjecutarAsync(dto);

MlResult<Acuse> conHistorial = resultado
        // Guardamos la descripción del fallo en una clave propia...
        .MapIfFail(er => Acuse.Rechazado(er.ToErrorsDescription()))
        // ...y ahora el "reset" ya no destruye información.
        .Match(() => Acuse.Cerrado(_reloj.Ahora));
```

---

## Mejores Prácticas

### 1. Cuándo usar la sobrecarga *match-all*

| Escenario | ¿Es adecuada? | Comentario |
| --- | :---: | --- |
| Cerrar un flujo con un acuse o informe único | ✅ | Es su caso natural. |
| Liberar recursos / limpiar temporales | ✅ | Normalmente con `TryMatch`. |
| Registrar una métrica de finalización | ✅ | Aunque `ExecSelf*` suele bastar. |
| Necesitas el valor o los errores | ❌ | Usa la sobrecarga clásica de dos ramas. |
| En medio de una tubería de negocio | ❌ | Enmascara los fallos anteriores. |
| Solo quieres un efecto secundario | ❌ | Usa `ExecSelf*`, que no altera el resultado. |

### 2. Diferencia con la sobrecarga clásica de `Match`

```csharp
// Clásica: recibe el estado, DEVUELVE UN VALOR CRUDO (sale del MlResult).
IActionResult respuesta = resultado.Match(
    valid: dto     => Ok(dto),
    fail : errores => BadRequest(errores.ToErrorsMessages()))

// match-all: ignora el estado, DEVUELVE UN MlResult (sigue en la tubería).
MlResult<string> cierre = resultado.Match(() => "Finalizado");
```

| Aspecto | Clásica | *match-all* |
| --- | --- | --- |
| Delegados | 2 (`valid`, `fail`) | 1 (`funcAll`, sin parámetros) |
| Acceso al valor / errores | Sí | No |
| Tipo de retorno | `TReturn` | `MlResult<TReturn>` |
| Conserva el estado de fallo | Lo trata explícitamente | **Lo descarta** (`Match`) / lo fusiona (`TryMatch`) |

### 3. Usa `TryMatch` para operaciones con riesgo

Cualquier `funcAll` que toque disco, red o base de datos debe ir con `TryMatch`, nunca con `Match`:

```csharp
// ❌ Si GenerarPdf lanza, la excepción se propaga y rompe la tubería.
var r1 = resultado.Match(() => GenerarPdf(datos));

// ✅ La excepción se convierte en Fail y queda en Details["Ex"].
var r2 = resultado.TryMatch(() => GenerarPdf(datos),
                            ex => $"No se pudo generar el PDF: {ex.Message}");
```

### 4. Registra antes de reiniciar

Como la sobrecarga *match-all* de `Match` descarta los errores, **siempre** encadena antes un
`ExecSelfIfFail` (o `ExecSelfIfFailAsync`) si el diagnóstico te importa.

---

## Comparación con Match y ExecSelf

### Tabla Comparativa

| Operación | ¿Se ejecuta siempre? | ¿Recibe el valor? | ¿Recibe los errores? | Devuelve | Altera el resultado |
| --- | :---: | :---: | :---: | --- | :---: |
| `Match(valid, fail)` | Sí | Sí | Sí | `TReturn` crudo | Sale de la tubería |
| `Match(funcAll)` | Sí | No | No | `MlResult<TReturn>` | Sí (contexto nuevo) |
| `TryMatch(funcAll, …)` | Sí | No | No (los fusiona) | `MlResult<TReturn>` | Sí |
| `MapAlways(funcAlways)` | Sí | No | No | `MlResult<TReturn>` | Sí |
| `BindAlways(funcAlways)` | Sí | No | No | `MlResult<TReturn>` | Sí |
| `ExecSelf(...)` | Sí | Sí | Sí | El **mismo** `MlResult<T>` | **No** |
| `ExecSelfIfValid` | Solo si válido | Sí | — | El mismo | **No** |
| `ExecSelfIfFail` | Solo si fallido | — | Sí | El mismo | **No** |

> 📌 `Match(funcAll)`, `MapAlways(funcAlways)` y `BindAlways(funcAlways)` tienen implementaciones
> prácticamente idénticas (`=> funcAlways();`). Elige el nombre que mejor exprese tu intención: `Match`
> para *cerrar* un flujo, `MapAlways`/`BindAlways` para *continuar* con otro.

### Ejemplo Comparativo

```csharp
MlResult<Pedido> resultado = ProcesarPedido(dto);

// 1. Quiero devolver una respuesta HTTP → clásica.
IActionResult http = resultado.Match(
    valid: p  => Ok(p.ToDto()),
    fail : er => BadRequest(er.ToErrorsMessages()))

// 2. Quiero registrar sin cambiar nada → ExecSelf.
MlResult<Pedido> igual = resultado
        .ExecSelfIfValid(p  => _log.LogInformation("OK {Id}", p.Id))
        .ExecSelfIfFail (er => _log.LogWarning("KO {E}", er.ToErrorsDescription()))

// 3. Quiero cerrar con un acuse común → match-all.
MlResult<Acuse> acuse = resultado.Match(() => new Acuse(_reloj.Ahora));
```

---

## Resumen

- **No hay ningún método llamado `MatchAll`**: son sobrecargas de `Match` / `TryMatch` que reciben un
  `Func<TReturn>` sin parámetros y devuelven `MlResult<TReturn>`.
- Se ejecutan **siempre**, ignorando el estado y el contenido del resultado de origen.
- `Match(funcAll)` **descarta** los errores previos; `TryMatch(funcAll, …)` los **fusiona** con el nuevo
  fallo si lo hubiera.
- Su uso natural es el **cierre de un flujo**: acuses, informes, limpieza y auditoría.
- Si necesitas el valor o los errores, usa la sobrecarga clásica de dos ramas.
- Si solo quieres un efecto secundario, usa `ExecSelf*`.

## Ver también

- [`1_Match.md`](./1_Match.md) — la sobrecarga clásica de dos ramas.
- [`../Types/MlResultActionsMatch.md`](../Types/MlResultActionsMatch.md) — referencia completa del archivo fuente.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — efectos secundarios sin alterar el resultado.
- [`../Map/8_MapAlways.md`](../Map/8_MapAlways.md) y [`../Bind/10_BindAlways.md`](../Bind/10_BindAlways.md) — operaciones incondicionales que continúan la tubería.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — recuperar la excepción guardada por `TryMatch`.