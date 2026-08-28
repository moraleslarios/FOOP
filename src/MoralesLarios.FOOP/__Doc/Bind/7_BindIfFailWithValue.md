# BindIfFailWithValue — Recuperarse usando el valor que provocó el fallo

## Índice
1. [Introducción](#introducción)
2. [El requisito previo: `AddValueIfFail`](#el-requisito-previo-addvalueiffail)
3. [Las dos formas de `BindIfFailWithValue`](#las-dos-formas-de-bindiffailwithvalue)
4. [Firmas reales](#firmas-reales)
5. [Qué pasa si el valor no está](#qué-pasa-si-el-valor-no-está)
6. [Variantes asíncronas](#variantes-asíncronas)
7. [`TryBindIfFailWithValue`](#trybindiffailwithvalue)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

Cuando un paso falla, el valor con el que trabajaba **se pierde**: `MlResult<T>` fallido solo contiene
errores. Pero muchas estrategias de recuperación necesitan justamente ese dato: *reintentar con el
mismo pedido*, *guardar la línea rechazada*, *aplicar una corrección al valor original*.

`BindIfFailWithValue` resuelve eso: en la rama de fallo, **rescata el valor guardado en los detalles del
error** y lo pasa a tu función de recuperación.

```csharp
// ❌ BindIfFail solo te da los errores: no sabes QUÉ pedido falló.
.BindIfFail(errores => /* ...y ahora, ¿con qué reintento? */);

// ✅ BindIfFailWithValue te devuelve el valor original.
.BindIfFailWithValue(pedidoOriginal => ReintentarConCorreccion(pedidoOriginal));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## El requisito previo: `AddValueIfFail`

🔑 **Este es el punto que más confusión genera.** `BindIfFailWithValue` **no adivina** el valor: lo lee
de `Details["Value"]` (la constante `VALUE_KEY`). Si nadie lo guardó ahí antes, no hay nada que
recuperar.

El encargado de guardarlo es [`AddValueIfFail`](../Types/MlResultActionsErrorsDetails.md):

```csharp
public static MlResult<T> AddValueIfFail<T, TValue>(this MlResult<T> source, TValue value);
```

Flujo completo:

```csharp
var resultado = ObtenerPedido(id)          // MlResult<Pedido> válido
    .AddValueIfFail(dtoOriginal)           // ① Si falla más adelante, el DTO viajará en Details["Value"]
    .Bind(p => ValidarStock(p))            // ② Aquí falla → los detalles ya llevan el DTO
    .BindIfFailWithValue<PedidoDto>(dto => // ③ Recuperamos el DTO y lo usamos
        RegistrarPedidoPendiente(dto));
```

📌 Sin el paso ① el `GetDetailValue<T>()` interno fallará, y `BindIfFailWithValue` devolverá un fallo
indicando que no hay valor en los detalles. **No es un error silencioso, pero tampoco es lo que
esperabas.**

💡 Alternativa habitual: muchas operaciones de la librería (por ejemplo las que fallan tras haber
tenido un valor) ya lo rellenan mediante `MlErrorsDetails.FromErrorMessageWithValue(mensaje, valor)`.
Si tú construyes el fallo a mano, usa esa factoría:

```csharp
return MlResult<Registro>.Fail(
    MlErrorsDetails.FromErrorMessageWithValue("La línea no cumple el formato", lineaCsv));
```

---

## Las dos formas de `BindIfFailWithValue`

| Forma | Firma | Si el resultado es **válido** |
| --- | --- | --- |
| **A — Solo recuperación** | `BindIfFailWithValue<T>(funcValue)` | Devuelve el valor tal cual |
| **B — Ambos caminos** | `BindIfFailWithValue<T, TValue, TReturn>(funcValid, funcFail)` | Ejecuta `funcValid` |

En la forma **A** el valor recuperado es del **mismo tipo `T`** que el resultado. En la forma **B**
puedes recuperar un tipo distinto (`TValue`) y devolver un tercero (`TReturn`), lo cual es lo habitual:
falla un `MlResult<Registro>` pero lo que guardaste es la `LineaCsv` de entrada.

```csharp
// Forma A: el valor guardado es del mismo tipo.
MlResult<Pedido> r = procesarPedido(p)
    .BindIfFailWithValue(pedido => ReintentarSimplificado(pedido));

// Forma B: entra Registro, el valor guardado es LineaCsv, sale Resultado.
MlResult<Resultado> r2 = importarRegistro(linea)
    .BindIfFailWithValue<Registro, LineaCsv, Resultado>(
        funcValid: reg    => Resultado.Ok(reg),
        funcFail : lineaOriginal => Resultado.Rechazada(lineaOriginal));
```

---

## Firmas reales

### Forma A

```csharp
public static MlResult<T> BindIfFailWithValue<T>(this MlResult<T>          source,
                                                      Func<T, MlResult<T>> funcValue)
    => source.Match(
        fail : errorsDetails => errorsDetails.GetDetailValue<T>().Bind(funcValue),
        valid: value         => value);
```

### Forma B

```csharp
public static MlResult<TReturn> BindIfFailWithValue<T, TValue, TReturn>(this MlResult<T>                    source,
                                                                             Func<T     , MlResult<TReturn>> funcValid,
                                                                             Func<TValue, MlResult<TReturn>> funcFail)
    => source.Match(
        fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Bind(value => funcFail(value)),
        valid: value         => funcValid(value));
```

Fíjate en el patrón: **`GetDetailValue<TValue>()` devuelve un `MlResult<TValue>`**, y sobre él se aplica
`.Bind(...)`. Es decir, la recuperación se encadena de forma segura: si no hay valor, `Bind` no ejecuta
tu función.

---

## Qué pasa si el valor no está

| Situación | Resultado |
| --- | --- |
| Resultado válido | Se ignora todo lo demás: forma A devuelve el valor, forma B ejecuta `funcValid` |
| Fallido **con** `Details["Value"]` del tipo esperado | Se ejecuta tu función de recuperación con ese valor |
| Fallido **sin** `Details["Value"]` | `GetDetailValue<TValue>()` falla → **tu función NO se ejecuta** y se devuelve ese fallo |
| Fallido con `Details["Value"]` de **otro tipo** | Igual que el caso anterior: el casting falla y tu función no se ejecuta |

⚠️ Este último punto es la trampa clásica: si guardas un `PedidoDto` y luego recuperas
`BindIfFailWithValue<Pedido>`, no funcionará. **El tipo debe coincidir exactamente.**

Para diagnosticarlo puedes inspeccionar los detalles antes:

```csharp
resultado.ExecSelfIfFail(errores =>
    _log.LogDebug("¿Hay valor en los detalles? {Detalle}", errores.ToDetailsDescription()));
```

---

## Variantes asíncronas

`BindIfFailWithValueAsync` aporta **12 sobrecargas**:

| Eje | Opciones |
| --- | --- |
| Fuente | `MlResult<T>` · `Task<MlResult<T>>` |
| Delegado | síncrono · asíncrono |
| Forma | A (solo `funcValue`) · B (`funcValid` + `funcFail`) |

```csharp
public Task<MlResult<Envio>> ProcesarAsync(EnvioDto dto)
    => CrearEnvioAsync(dto)
        .AddValueIfFailAsync(dto)
        .BindIfFailWithValueAsync<Envio, EnvioDto, Envio>(
            funcValidAsync: e   => ConfirmarAsync(e),
            funcFailAsync : d   => EncolarParaRevisionManualAsync(d));
```

La versión asíncrona usa `GetDetailValueAsync<TValue>()` internamente, con la misma semántica.

---

## `TryBindIfFailWithValue`

Si la recuperación puede lanzar excepciones (guardar en base de datos, escribir un fichero de
rechazados, llamar a otra API), usa la variante `Try*`, que captura la excepción y la guarda en
`Details["Ex"]`:

```csharp
public static MlResult<T> TryBindIfFailWithValue<T>(this MlResult<T>          source,
                                                        Func<T, MlResult<T>> funcValue,
                                                        Func<Exception, string> errorMessageBuilder);
```

Sobrecargas: **`TryBindIfFailWithValue` (4)** y **`TryBindIfFailWithValueAsync` (24)**, con las
variantes de mensaje fijo (`string exceptionAditionalMessage`) y de constructor de mensaje
(`Func<Exception, string>`).

---

## Ejemplos Prácticos

### Ejemplo 1: Importar un CSV registrando las líneas rechazadas

El caso canónico: cuando una línea no se puede convertir, quieres guardarla **tal cual llegó** para que
el usuario la corrija.

```csharp
public record LineaCsv(int Numero, string Contenido);
public record Registro(int Id, string Nombre, decimal Importe);
public record LineaRechazada(int Numero, string Contenido, IEnumerable<string> Motivos);

public class ImportadorCsv
{
    public MlResult<ResumenImportacion> Importar(IEnumerable<LineaCsv> lineas)
    {
        var resultados = lineas
            .Select(linea => ProcesarLinea(linea))
            .ToList();

        return new ResumenImportacion(
            Importadas: resultados.Count(r => r.IsValid),
            Rechazadas: resultados.Count(r => r.IsFail));
    }

    private MlResult<Registro> ProcesarLinea(LineaCsv linea)
        => Parsear(linea)

            // ① Guardamos la línea original: si algo falla después, viajará en los detalles.
            .AddValueIfFail(linea)

            .Bind(r => ValidarImporte(r))
            .Bind(r => _repo.Guardar(r))

            // ② Recuperamos la línea original para archivarla como rechazada.
            .BindIfFailWithValue<Registro, LineaCsv, Registro>(
                funcValid: registro => registro,
                funcFail : original =>
                {
                    _rechazos.Archivar(new LineaRechazada(
                        original.Numero, original.Contenido, ["No superó la validación"]));

                    // Seguimos considerándolo un fallo: la línea NO se importó.
                    return MlResult<Registro>.Fail($"Línea {original.Numero} rechazada");
                });
}
```

**Clave:** `AddValueIfFail(linea)` justo después del parseo es lo que hace posible el paso ②.

### Ejemplo 2: Reintentar con una corrección automática

```csharp
public MlResult<Direccion> Normalizar(DireccionDto dto)
    => ValidarConCallejero(dto)
        .AddValueIfFail(dto)

        // Si el callejero rechaza la dirección, probamos una versión "limpiada".
        .BindIfFailWithValue<Direccion, DireccionDto, Direccion>(
            funcValid: d        => d,
            funcFail : original => ValidarConCallejero(Limpiar(original)));

private static DireccionDto Limpiar(DireccionDto d) => d with
{
    Calle       = d.Calle.Trim().Replace("  ", " "),
    CodigoPostal = d.CodigoPostal.PadLeft(5, '0')
};
```

Un solo reintento, con una transformación conocida, sin duplicar la llamada al callejero en dos ramas de
un `if`.

### Ejemplo 3: Cola de reintentos asíncrona

```csharp
public async Task<MlResult<Notificacion>> EnviarAsync(NotificacionDto dto)
    => await ConstruirAsync(dto)
        .AddValueIfFailAsync(dto)
        .BindAsync(n => _proveedor.EnviarAsync(n))

        .TryBindIfFailWithValueAsync<Notificacion, NotificacionDto, Notificacion>(
            funcValidAsync: n => n.ToMlResultValidAsync(),

            // El DTO original se encola para reintentar más tarde.
            funcFailAsync : async original =>
            {
                await _cola.EncolarAsync(original, reintentarEn: TimeSpan.FromMinutes(15));
                return MlResult<Notificacion>.Fail(
                    $"Notificación a {original.Destinatario} encolada para reintento");
            },

            ex => $"No se pudo encolar la notificación: {ex.Message}")

        .ExecSelfIfFailAsync(errores =>
        {
            _log.LogWarning("Envío fallido: {Detalle}", errores.ToErrorsDescription());
            return Task.CompletedTask;
        });
```

Se usa `TryBindIfFailWithValueAsync` porque **encolar también puede fallar**, y en ese caso no queremos
una excepción sin controlar.

### Ejemplo 4: Diagnóstico — cuando olvidas `AddValueIfFail`

```csharp
// ❌ MAL: nadie guardó el valor, la recuperación nunca se ejecuta.
var malo = ObtenerCliente(id)
    .Bind(c => ValidarCredito(c))
    .BindIfFailWithValue(c => AplicarLimiteReducido(c));
// → Fallo: "no existe valor en los detalles" (AplicarLimiteReducido NO se llamó)

// ✅ BIEN: el valor se guarda antes del paso que puede fallar.
var bueno = ObtenerCliente(id)
    .AddValueIfFail(default(Cliente)!)   // o el DTO/entrada que quieras conservar
    .Bind(c => ValidarCredito(c))
    .BindIfFailWithValue(c => AplicarLimiteReducido(c));
```

Y si dudas de si el valor está o no, compruébalo explícitamente:

```csharp
resultado.ExecSelfIfFail(errores =>
    errores.GetDetailValue<Cliente>()
           .Match(valid: c => _log.LogInformation("Valor recuperado: {Id}", c.Id),
                  fail : _ => _log.LogWarning("Falta AddValueIfFail en la tubería")));
```

---

## Mejores Prácticas

### 1. `AddValueIfFail` va **antes** del paso que puede fallar

Colócalo lo más cerca posible del origen del dato que quieres conservar. Si lo pones al final, ya no
habrá nada que guardar.

### 2. El tipo tiene que coincidir exactamente

`AddValueIfFail(dto)` guarda un `PedidoDto`; entonces debes recuperar con
`BindIfFailWithValue<..., PedidoDto, ...>`. Un tipo distinto (incluso una clase base) hará que la
recuperación no se ejecute.

### 3. Guarda la **entrada**, no un estado intermedio

Lo útil para reintentar o archivar es casi siempre el DTO/línea/mensaje original, no un objeto de
dominio a medio construir.

### 4. Recuperar no obliga a devolver éxito

Es perfectamente válido (y frecuente) que `funcFail` archive el valor y **siga devolviendo un fallo**,
como en el ejemplo 1. La recuperación aquí significa «tengo el dato para actuar», no «finjo que salió
bien».

### 5. Usa `TryBindIfFailWithValue` si la recuperación toca I/O

Archivar rechazados, encolar reintentos o escribir logs persistentes pueden lanzar. No dejes que una
excepción en el camino de error tumbe la aplicación.

### 6. No abuses: `Details` no es un contenedor de estado

Guardar un valor para recuperarlo es un patrón puntual de recuperación. Si necesitas llevar varios datos
a lo largo de la tubería, usa una **tupla** o un objeto de contexto en el camino válido.

---

## Resumen

- `BindIfFailWithValue` actúa **solo en el camino fallido** y recupera de `Details["Value"]` el valor que
  provocó (o precedió a) el fallo.
- 🔑 **Requiere que alguien haya guardado ese valor antes**, normalmente con `AddValueIfFail` o con la
  factoría `MlErrorsDetails.FromErrorMessageWithValue`.
- Dos formas: **A** (`funcValue`, mismo tipo) y **B** (`funcValid` + `funcFail`, con `TValue` y `TReturn`
  independientes).
- Internamente hace `errorsDetails.GetDetailValue<TValue>().Bind(...)`: si el valor no está o el tipo no
  coincide, **tu función no se ejecuta** y se devuelve ese fallo.
- Sobrecargas: `BindIfFailWithValue` (2), `Async` (12), `TryBindIfFailWithValue` (4), `Try...Async` (24).
- Casos típicos: archivar líneas rechazadas, reintentar con corrección, encolar para revisión manual.

## Ver también

- [`6_BindIfFail.md`](./6_BindIfFail.md) — recuperación sin necesitar el valor original.
- [`8_BindIfFailWithException.md`](./8_BindIfFailWithException.md) — recuperación solo ante excepciones.
- [`9_BindIfFailWithoutException.md`](./9_BindIfFailWithoutException.md) — recuperación solo ante fallos de negocio.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `AddValueIfFail`, `GetDetailValue<T>` y el resto del acceso a detalles.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — la clave `VALUE_KEY` y las factorías de `MlErrorsDetails`.
- [`../ExecSelf/4_ExecSelfIfFailWithValue.md`](../ExecSelf/4_ExecSelfIfFailWithValue.md) — la versión que solo observa, sin transformar.
- [`../Map/5_MapIfFailWithValue.md`](../Map/5_MapIfFailWithValue.md) — la versión con función que no puede fallar.
- [`../Types/MlResultActionsBind.md`](../Types/MlResultActionsBind.md) — referencia con todas las sobrecargas.