# MlResultActions — Enriquecimiento, acceso seguro y composición

## Índice
1. [Introducción](#introducción)
2. [Mapa de la clase](#mapa-de-la-clase)
3. [Enriquecer errores: `AddMlErrorDetailIfFail` y `AddValueDetailIfFail`](#enriquecer-errores-addmlerrordetailiffail-y-addvaluedetailiffail)
4. [Completar con datos: la familia `CompleteWithData*`](#completar-con-datos-la-familia-completewithdata)
5. [Acceso seguro: `SecureValidValue` y `SecureFailErrorsDetails`](#acceso-seguro-securevalidvalue-y-securefailerrorsdetails)
6. [Composición cruda: `CreateCompleteMlResult`](#composición-cruda-createcompletemlresult)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Resumen](#resumen)
10. [Ver también](#ver-también)

---

## Introducción

`MlResultActions` (fichero `Types/MlResultActions.cs`) es la clase «de infraestructura» del tipo
`MlResult<T>`. No transforma valores como [`Map`](../Map/1_Map.md) ni encadena operaciones como
[`Bind`](./3_Bind.md): su trabajo es **enriquecer el resultado con contexto**, **acceder a su interior
de forma segura** y **componer resultados a mano** cuando hace falta.

Existe por una razón de diseño muy concreta:

```csharp
public partial record MlResult<T>
{
    internal protected T                Value         { get; init; }
    internal protected MlErrorsDetails  ErrorsDetails { get; init; }
}
```

`Value` y `ErrorsDetails` son **`internal protected` a propósito**: desde fuera de la librería no
puedes leerlos directamente, y eso es una virtud, no una limitación. Te obliga a pasar por
[`Match`](../Match/1_Match.md) o por los métodos de esta clase, que nunca lanzan
`NullReferenceException` ni devuelven un valor «basura».

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## Mapa de la clase

| Grupo | Métodos | Sobrecargas | Para qué sirve |
| --- | --- | :---: | --- |
| **Enriquecer errores** | `AddMlErrorDetailIfFail` | 1 | Añade un mensaje de error extra si ya está fallido |
| | `AddMlErrorDetailIfFailAsync` | 2 | Idem, en cadenas asíncronas |
| | `AddValueDetailIfFail` | 1 | Guarda un valor arbitrario en `Details` si está fallido |
| | `AddValueDetailIfFailAsync` | 2 | Idem, en cadenas asíncronas |
| **Completar con datos** | `CompleteWithDataValueIfValid` | 1 | Guarda el valor válido en `Details` para usarlo después |
| | `CompleteWithDataValueIfValidAsync` | 4 | Idem, en cadenas asíncronas |
| | `CompleteWithDetailsValueIfFail` | 1 | Añade detalles solo a la rama fallida |
| | `CompleteWithDetailsValueIfFailAsync` | 1 | Idem, asíncrono |
| | `CompleteWithDataValue` | 1 | Guarda el valor sea válido o fallido |
| | `CompleteWithDataValueAsync` | 4 | Idem, asíncrono |
| **Acceso seguro** | `SecureValidValue` | 1 | Devuelve el valor o `default` sin lanzar |
| | `SecureValidValueAsync` | 1 | Idem, asíncrono |
| | `SecureFailErrorsDetails` | 1 | Devuelve los errores o unos vacíos, sin lanzar |
| | `SecureFailErrorsDetailsAsync` | 2 | Idem, asíncrono |
| **Composición** | `CreateCompleteMlResult` | 3 | Construye un `MlResult<T>` a partir de sus piezas |
| | `CreateCompleteMlResultAsync` | 8 | Idem, asíncrono |

📌 Casi todos comparten un patrón: **si el estado no coincide con lo que el método espera, devuelven
el resultado intacto**. Son seguros de encadenar en cualquier punto de una tubería.

---

## Enriquecer errores: `AddMlErrorDetailIfFail` y `AddValueDetailIfFail`

### `AddMlErrorDetailIfFail`

Añade un mensaje de error adicional **solo si el resultado ya es fallido**. Sirve para aportar
contexto de la capa en la que estás sin perder el error original.

```csharp
public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, string errorDetail)
```

```csharp
var resultado = _repositorio.ObtenerCliente(id)
    .AddMlErrorDetailIfFail($"Fallo al recuperar el cliente {id} en el alta de pedido");

// Si el repositorio falló con "Conexión rechazada", ahora los errores son:
//   1. "Conexión rechazada"
//   2. "Fallo al recuperar el cliente 42 en el alta de pedido"
```

Esto genera **trazas apilables**: cada capa añade su capa de contexto y al final tienes la historia
completa del fallo sin haber escrito un solo `try/catch`.

### `AddValueDetailIfFail`

Guarda un valor arbitrario en `Details` **solo si el resultado es fallido**. Es la forma de conservar
el dato de entrada que provocó el problema.

```csharp
public static MlResult<T> AddValueDetailIfFail<T, TValue>(this MlResult<T> source, TValue value)
```

```csharp
var resultado = ProcesarLinea(linea)
    .AddValueDetailIfFail(linea);          // Guarda la línea original en Details["Value"]

resultado.ExecSelfIfFailWithValue<Registro, LineaCsv>((errores, lineaMala) =>
    _log.LogWarning("Línea {N} descartada: {E}", lineaMala.Numero, errores.ToErrorsDescription()));
```

> 💡 Es el hermano de [`AddValueIfFail`](../Types/MlResultActionsErrorsDetails.md) y el requisito
> previo de [`ExecSelfIfFailWithValue`](../ExecSelf/4_ExecSelfIfFailWithValue.md),
> [`BindIfFailWithValue`](./7_BindIfFailWithValue.md) y
> [`MapIfFailWithValue`](../Map/5_MapIfFailWithValue.md).

### Variantes asíncronas

```csharp
await _repositorio.ObtenerClienteAsync(id)
    .AddMlErrorDetailIfFailAsync($"Contexto: alta de pedido para el cliente {id}")
    .AddValueDetailIfFailAsync(id);
```

---

## Completar con datos: la familia `CompleteWithData*`

Estos tres métodos guardan información en `Details` para que esté disponible **más adelante** en la
tubería. Se diferencian únicamente en *cuándo* actúan:

| Método | Actúa si el resultado es… | Qué guarda |
| --- | --- | --- |
| `CompleteWithDataValueIfValid` | **Válido** | El valor actual, en `Details` |
| `CompleteWithDetailsValueIfFail` | **Fallido** | Detalles adicionales que tú aportas |
| `CompleteWithDataValue` | **Cualquiera** | El valor, sin condiciones |

```csharp
// Guardamos el pedido original antes de una transformación con pérdida de información,
// para poder reconstruir el contexto si algo falla más abajo.
var resultado = ObtenerPedido(id)
    .CompleteWithDataValueIfValid()
    .Bind(p => CalcularTotales(p))          // Aquí ya trabajamos con un Totales, no con el Pedido
    .Bind(t => Facturar(t))
    .ExecSelfIfFail(errores =>
        errores.GetDetailValue<Pedido>().Match(
            valid: pedido => _log.LogError("Falló la facturación del pedido {Id}", pedido.Id),
            fail:  _      => _log.LogError("Falló la facturación (sin contexto de pedido)")));
```

```csharp
// Añadimos metadatos de diagnóstico solo cuando hay fallo.
var resultado = EjecutarConsulta(sql)
    .CompleteWithDetailsValueIfFail(new Dictionary<string, object>
    {
        ["Sql"]        = sql,
        ["Servidor"]   = _config.Servidor,
        ["Momento"]    = DateTime.UtcNow
    });
```

---

## Acceso seguro: `SecureValidValue` y `SecureFailErrorsDetails`

Son las **puertas de escape controladas**. Nunca lanzan excepciones, sea cual sea el estado.

```csharp
public static T               SecureValidValue<T>       (this MlResult<T> source);
public static MlErrorsDetails SecureFailErrorsDetails<T>(this MlResult<T> source);
```

| Método | Si el resultado es válido | Si el resultado es fallido |
| --- | --- | --- |
| `SecureValidValue` | Devuelve el valor | Devuelve `default(T)` (`null` en referencias, `0` en `int`…) |
| `SecureFailErrorsDetails` | Devuelve un `MlErrorsDetails` vacío | Devuelve los errores reales |

```csharp
// Caso legítimo: interoperar con un API que espera el valor pelado y acepta null.
var cliente = ObtenerCliente(id).SecureValidValue();
if (cliente is null) return NotFound();

// Caso legítimo: volcar los errores en un log sin ramificar.
_log.LogDebug("Errores acumulados: {E}",
              resultado.SecureFailErrorsDetails().ToErrorsDescription());
```

⚠️ **Cuidado con `SecureValidValue`:** te devuelve `default(T)` en la rama fallida, lo que significa
que **pierdes por completo el motivo del fallo**. Úsalo solo cuando de verdad no te importe, y prefiere
`Match` en todo lo demás:

```csharp
// ❌ Pierde información y te obliga a comprobar null.
var total = CalcularTotal(pedido).SecureValidValue();

// ✅ Explícito, sin nulos y con valor por defecto intencionado.
var total = CalcularTotal(pedido).Match(valid: t => t, fail: _ => 0m);
```

---

## Composición cruda: `CreateCompleteMlResult`

Construye un `MlResult<T>` a partir de sus piezas (valor, errores y detalles) en una sola llamada.
Es la herramienta de más bajo nivel de la clase y la necesitarás sobre todo al **escribir tus propios
combinadores** o al **traducir desde otra librería**.

```csharp
// Adaptador desde un resultado de una librería de terceros.
public static MlResult<T> DesdeTercero<T>(ResultadoExterno<T> externo)
    => externo.Ok
        ? MlResult<T>.Valid(externo.Data)
        : MlResultActions.CreateCompleteMlResult<T>(
              value         : default!,
              errorsDetails : MlErrorsDetails.FromErrorsMessagesDetails(
                                  externo.Mensajes,
                                  new Dictionary<string, object>
                                  {
                                      ["CodigoExterno"] = externo.Codigo,
                                      ["Origen"]        = "ApiTercero"
                                  }));
```

En el 95 % de los casos no lo necesitas: para crear resultados usa
`MlResult.Valid`, `MlResult.Fail`, las conversiones implícitas o
[`ToMlResultFail`](../Transformations/Transformations.md).

---

## Ejemplos Prácticos

### Ejemplo 1: Tubería con contexto acumulado por capas

```csharp
public class GestorPedidos
{
    public async Task<MlResult<Factura>> AplicarDescuentoAsync(int pedidoId, string cupon)
        => await ObtenerPedidoAsync(pedidoId)
                .AddMlErrorDetailIfFailAsync($"[Pedidos] No se pudo recuperar el pedido {pedidoId}")

                // Conservamos el pedido: lo vamos a necesitar si falla el cálculo.
                .CompleteWithDataValueIfValidAsync()

                .BindAsync(p => ValidarCuponAsync(p, cupon))
                .AddMlErrorDetailIfFailAsync($"[Descuentos] El cupón '{cupon}' no es aplicable")

                .BindAsync(p => EmitirFacturaAsync(p))
                .AddMlErrorDetailIfFailAsync("[Facturación] La emisión de la factura ha fallado")

                // Un único punto de observabilidad, con la historia completa del fallo.
                .ExecSelfIfFailAsync(errores =>
                {
                    var contexto = errores.GetDetailValue<Pedido>()
                                          .Match(valid: p => $"pedido {p.Id}, total {p.Total:C}",
                                                 fail:  _ => "sin contexto de pedido");

                    _log.LogError("Descuento no aplicado ({Contexto}). Traza:\n{Traza}",
                                  contexto, errores.ToErrorsDescription());
                    return Task.CompletedTask;
                });
}
```

El log resultante contiene la cadena entera de causas:

```
[Pedidos] No se pudo recuperar el pedido 42
  ← Timeout al conectar con la base de datos
```

### Ejemplo 2: Importación masiva conservando la fila que falló

```csharp
public MlResult<ResumenImportacion> Importar(IEnumerable<LineaCsv> lineas)
{
    var aceptadas = new List<Registro>();
    var rechazadas = new List<LineaRechazada>();

    foreach (var linea in lineas)
    {
        ProcesarLinea(linea)
            .AddValueDetailIfFail(linea)               // Guardamos la línea original
            .ExecSelfIfValid(r => aceptadas.Add(r))
            .ExecSelfIfFailWithValue<Registro, LineaCsv>((errores, mala) =>
                rechazadas.Add(new LineaRechazada(mala.Numero,
                                                  mala.Contenido,
                                                  errores.ToErrorsMessages())));
    }

    return new ResumenImportacion(aceptadas, rechazadas);
}

public record LineaCsv(int Numero, string Contenido);
public record LineaRechazada(int Numero, string Contenido, IEnumerable<string> Motivos);
public record ResumenImportacion(IReadOnlyList<Registro> Aceptadas,
                                 IReadOnlyList<LineaRechazada> Rechazadas);
```

### Ejemplo 3: Frontera con ASP.NET Core

```csharp
[HttpPost("pedidos/{id:int}/confirmar")]
public async Task<IActionResult> Confirmar(int id)
{
    var resultado = await _gestor.ConfirmarAsync(id)
        .CompleteWithDetailsValueIfFailAsync(new Dictionary<string, object>
        {
            ["TraceId"]  = HttpContext.TraceIdentifier,
            ["Usuario"]  = User.Identity?.Name ?? "anónimo"
        });

    return resultado.Match<Confirmacion, IActionResult>(
        valid: c       => Ok(c),
        fail:  errores => StatusCode(500, new
        {
            errores = errores.ToErrorsMessages(),
            traza   = errores.ToDetailsDescription()      // Incluye TraceId y Usuario
        }));
}
```

---

## Mejores Prácticas

### 1. `AddMlErrorDetailIfFail` una vez por capa, no una por línea

Un mensaje de contexto por frontera arquitectónica (repositorio, servicio, controlador) produce trazas
legibles. Diez mensajes seguidos producen ruido.

### 2. `CompleteWithDataValueIfValid` justo antes de perder el dato

Colócalo inmediatamente antes de la transformación que descarta la información que querrás en el log.

### 3. `SecureValidValue` solo en las fronteras

Dentro del dominio, `Match`. En el borde (interoperar con un API que exige el valor pelado), `Secure*`.
Y si necesitas un valor por defecto explícito, `Match(valid: x => x, fail: _ => defecto)` es más claro.

### 4. `CreateCompleteMlResult` es para adaptadores

Si lo estás usando en lógica de negocio, casi siempre hay un método más expresivo:
`MlResult.Fail`, `ToMlResultFail`, `EnsureFp.That`…

### 5. Recuerda que estos métodos no cortocircuitan

Son transparentes: devuelven el resultado intacto cuando el estado no les corresponde. Puedes
encadenarlos con total libertad sin alterar la semántica de la tubería.

---

## Resumen

- `MlResultActions` aporta **contexto, acceso seguro y composición**, no transformación.
- `Value` y `ErrorsDetails` son `internal protected` **por diseño**: el acceso pasa por `Match` o por
  los métodos `Secure*`.
- `AddMlErrorDetailIfFail` apila mensajes de contexto capa a capa; `AddValueDetailIfFail` conserva el
  dato culpable en `Details["Value"]`.
- La familia `CompleteWithData*` guarda información para usarla más abajo en la tubería.
- `SecureValidValue` devuelve `default(T)` en la rama fallida: cómodo, pero pierde el motivo del error.
- `CreateCompleteMlResult` es la construcción de bajo nivel, ideal para adaptadores.

## Ver también

- [`3_Bind.md`](./3_Bind.md) — encadenar operaciones que devuelven `MlResult`.
- [`../Types/MlResult.md`](../Types/MlResult.md) — el tipo y sus miembros reales.
- [`../Types/MlResultActions.md`](../Types/MlResultActions.md) — referencia completa de esta clase.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetail`, `AddValueIfFail`, `GetDetailException`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y las claves convencionales.
- [`../Match/1_Match.md`](../Match/1_Match.md) — la forma idiomática de salir de `MlResult<T>`.
- [`../ExecSelf/4_ExecSelfIfFailWithValue.md`](../ExecSelf/4_ExecSelfIfFailWithValue.md) — consumir el valor guardado en el fallo.
- [`../Transformations/Transformations.md`](../Transformations/Transformations.md) — crear resultados desde valores y mensajes.