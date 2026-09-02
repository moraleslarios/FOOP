# MoralesLarios.OOFP - Documentación Técnica Completa

## Tabla de Contenidos

1. [Introducción](#introducción)
2. [Arquitectura y Filosofía](#arquitectura-y-filosofía)
3. [Estructura del Proyecto](#estructura-del-proyecto)
4. [Tipos Fundamentales](#tipos-fundamentales)
5. [Sistema de Convención de Nombres](#sistema-de-convención-de-nombres)
6. [Análisis Detallado de Métodos](#análisis-detallado-de-métodos)
7. [Gestión de Errores](#gestión-de-errores)
8. [Extensiones y Utilidades](#extensiones-y-utilidades)
9. [Patrones de Uso](#patrones-de-uso)
10. [Ejemplos Prácticos](#ejemplos-prácticos)
11. [Índice completo de la documentación](#índice-completo-de-la-documentación)

---

## Introducción

**MoralesLarios.OOFP** (Object-Oriented Functional Programming) es una librería .NET 8 / .NET 9 diseñada para implementar patrones de programación funcional en C#, con un enfoque especial en el manejo robusto de resultados y errores. La librería proporciona una abstracción tipo `Result<T>` llamada `MlResult<T>` que encapsula tanto valores exitosos como estados de error, permitiendo la composición funcional de operaciones complejas.

### Objetivos Principales

- **Eliminación de excepciones como flujo de control**: Las operaciones devuelven resultados explícitos en lugar de lanzar excepciones
- **Composición funcional**: Permite encadenar operaciones de forma fluida y segura
- **Manejo explícito de errores**: Los errores son ciudadanos de primera clase con información detallada
- **Asincronía segura**: Soporte completo para operaciones asíncronas con `Task<MlResult<T>>`
- **Flexibilidad de recuperación**: Múltiples estrategias para manejar y recuperarse de errores

---

## Arquitectura y Filosofía

### Principios de Diseño

1. **Railway-Oriented Programming**: Implementa el patrón de "vías de tren" donde las operaciones pueden seguir la vía del éxito o la vía del error
2. **Monadic Composition**: `MlResult<T>` actúa como una mónada, permitiendo composición funcional
3. **Explicit Error Handling**: Los errores no se ocultan, se gestionan explícitamente
4. **Type Safety**: El sistema de tipos garantiza que los errores se manejen apropiadamente

### Flujo de Operaciones

```
Operación 1 (Éxito) → Operación 2 (Éxito) → Operación 3 (Éxito) → Resultado Final
      ↓                     ↓                     ↓
    Error 1               Error 2               Error 3
      ↓                     ↓                     ↓
  Manejo/Recovery       Manejo/Recovery       Manejo/Recovery
```

---

## Estructura del Proyecto

### Organización de Directorios

```
MoralesLarios.OOFP/
├── GlobalUsings.cs                    # Usings globales del proyecto
├── MoralesLarios.OOFP.csproj         # Configuración del proyecto
├── Helpers/                          # Utilidades y extensiones
│   ├── Constants.cs                  # Constantes del proyecto
│   ├── EnsureFp.cs                   # Guardas clásicas (That, NotNull, NotEmpty, NotNullEmptyOrWhitespace)
│   ├── EnsureFp.Core.cs              # Predicados y mensajes perezosos, TryThat y variantes …Arg
│   ├── EnsureFp.Aggregation.cs       # All, AllResults, AllOrFirst, Any: acumulación de errores
│   ├── EnsureFp.Strings.cs           # Longitudes, expresiones regulares, prefijos/sufijos, conjuntos
│   ├── EnsureFp.Numbers.cs           # Comparaciones, rangos y signo (IComparable / INumber)
│   ├── EnsureFp.Collections.cs       # Cardinalidad, duplicados, nulos y predicados por elemento
│   ├── EnsureFp.Types.cs             # Guid, enum, fechas, Uri, email, rutas y Nullable<T>
│   ├── EnsureFp.Async.cs             # Fuentes Task<T> y predicados asíncronos
│   ├── EnsureFpMessages.cs           # Plantillas de los mensajes automáticos
│   └── Extensions/                   # Extensiones generales
│       ├── Extensions.cs             # Extensiones base
│       └── ParallelExtensions.cs     # Extensiones paralelas
├── Types/                            # Tipos principales
│   ├── MlResult.cs                   # Tipo resultado principal
│   ├── MlResultActions.cs            # Extensiones transversales (enriquecer, completar, acceso seguro)
│   ├── MlResultActionsBind.cs        # Operaciones Bind (+ TryBindBuild)
│   ├── MlResultActionsErrorsDetails.cs # Lectura/escritura/fusión de los Details del error
│   ├── MlResultActionsExecSelf.cs    # Operaciones ExecSelf (efectos secundarios)
│   ├── MlResultActionsMap.cs         # Operaciones Map
│   ├── MlResultActionsMatch.cs       # Operaciones Match (salida del MlResult)
│   ├── MlResultActionsSeveral.cs     # Combine, NullToFailed, BoolToResult, EmptyToFailed, Do
│   ├── MlResultBucles.cs             # Proyecciones sobre colecciones
│   ├── MlResultChangeReturnResult.cs # Cambio del tipo de retorno
│   ├── MlResultTransformations.cs    # Frontera con el código imperativo (ToMlResult*, TryToMlResult*)
│   └── Errors/                       # Gestión de errores
│       ├── ErrorMessage.cs           # record ErrorMessage(string Message)
│       ├── MlError.cs                # Error base
│       ├── MlErrorsDetails.cs        # Detalles de error
│       └── MlErrorsDetailsActions.cs # Acciones sobre errores
└── __Doc/                            # Documentación
    ├── 1_Intro.md                    # Este documento
    ├── Types/                        # Referencia por archivo fuente
    ├── Bind/                         # Detalle de la familia Bind
    ├── Map/                          # Detalle de la familia Map
    ├── Match/                        # Detalle de la familia Match
    ├── ExecSelf/                     # Detalle de la familia ExecSelf
    ├── Several/                      # Combine, NullToFailed, BoolToResult, EmptyToFailed
    ├── Bucle/                        # Proyecciones sobre colecciones
    ├── EnsureFp/                     # Precondiciones funcionales (10 documentos)
    ├── Extensions/                   # Extensiones auxiliares
    ├── Transformations/              # Conversiones desde/hacia MlResult
    └── PendingTasks.txt              # Tareas pendientes
```

### Modularidad y Separación de Responsabilidades

Cada archivo tiene una responsabilidad específica:

- **MlResult.cs**: Define la estructura base del tipo resultado
- **MlResultActions*.cs**: Cada archivo implementa un conjunto específico de operaciones
- **Errors/**: Manejo completo del sistema de errores
- **Helpers/**: Utilidades transversales

---

## Tipos Fundamentales

### MlResult<T>

El tipo principal que encapsula un resultado que puede ser:
- **Válido**: Contiene un valor de tipo `T`
- **Fallido**: Contiene información detallada del error

```csharp
public partial record MlResult<T>
{
    internal protected T               Value         { get; init; }
    internal protected MlErrorsDetails ErrorsDetails { get; init; }

    public bool IsValid { get; init; }
    public bool IsFail  => ! IsValid;
}
```

Tres detalles importantes:

1. **Es un `record`, no una `class`**: inmutable y con igualdad por valor.
2. **Las propiedades de estado son `IsValid` e `IsFail`** (no `IsSuccess` / `IsFailure`).
3. **`Value` y `ErrorsDetails` son `internal protected`**: no son accesibles desde tu código. Es
   deliberado y es la clave de la librería: te obliga a pasar por
   [`Match`](./Types/MlResultActionsMatch.md), `Map` o `Bind`, de modo que nunca puedas leer un valor
   que no existe.

```csharp
MlResult<Cliente> resultado = ObtenerCliente(id);

// ❌ No compila: Value es internal protected.
// var c = resultado.Value;

// ✅ La forma correcta de salir del MlResult.
IActionResult respuesta = resultado.Match(
    valid: cliente => Ok(cliente),
    fail : errores => NotFound(errores.ToErrorsMessages()));
```

`MlResult` (sin genérico) aporta las fábricas **estáticas** (`MlResult.Valid<T>`, `MlResult.Fail<T>`,
`MlResult.Empty()`), y `MlResult<T>` añade `Valid` / `Fail` / `*Async` y numerosos **operadores de
conversión implícita** desde `T`, `string`, `MlError`, `MlError[]` y `MlErrorsDetails`.

Detalle completo en [`Types/MlResult.md`](./Types/MlResult.md).

### MlErrorsDetails

Encapsula la información completa de un fallo. Su diseño es minimalista a propósito: **solo dos
propiedades**.

```csharp
public class MlErrorsDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
{
    public IEnumerable<MlError>       Errors  { get; }
    public Dictionary<string, object> Details { get; }
}
```

- `Errors`: la lista de mensajes de error (puede haber varios: validaciones acumuladas).
- `Details`: diccionario abierto donde la librería y tu código guardan **contexto adicional**.

No existen `Exception`, `Value`, `HasException` ni `HasValue`: esa información vive en `Details` bajo
**claves convencionales** definidas en `Helpers/Constants.cs`:

| Clave | Constante | Contenido |
| --- | --- | --- |
| `"Ex"` | `EX_DESC_KEY` | Excepción capturada por cualquier método `Try*`. Si ya había una, se numeran `Ex2`, `Ex3`… |
| `"Value"` | `VALUE_KEY` | Valor de entrada que provocó el fallo. |

Se leen de forma **tipada y segura** con
[`MlResultActionsErrorsDetails`](./Types/MlResultActionsErrorsDetails.md):

```csharp
MlResult<Exception> ex     = errores.GetDetailException();          // clave "Ex"
MlResult<PedidoDto> origen = errores.GetDetailValue<PedidoDto>();   // clave "Value"
MlResult<string>    divisa = errores.GetDetail<string>("Divisa");   // clave propia
```

Devuelven `MlResult<T>`, **no el valor crudo**: si la clave no existe o el tipo no coincide obtienes un
`Fail` descriptivo en lugar de una excepción.

Detalle completo en [`Types/MlResultErrors.md`](./Types/MlResultErrors.md).

### MlError

Representa un error individual. También es mínimo: **un mensaje y nada más**.

```csharp
public record MlError
{
    public string Message { get; init; }

    public static MlError FromErrorMessage(string message);

    public static implicit operator MlError(string message);
}
```

No hay `Code` ni `Metadata`: la categorización se resuelve con el **tipo de la excepción** guardada en
`Details["Ex"]`, y los metadatos con el resto de claves de `Details`. Gracias a la conversión implícita,
casi nunca escribirás `MlError` a mano:

```csharp
MlError   error  = "El nombre es obligatorio";
MlError[] varios = ["Falta el nombre", "Falta el email"];

MlResult<Cliente> r = "El cliente no existe".ToMlResultFail<Cliente>();
```

Si el mensaje llega vacío, el constructor lo sustituye por `DEFAULT_ERROR_MESSAGE`, de modo que un
error nunca queda sin descripción.

---

## Sistema de Convención de Nombres

La librería tiene **miles de sobrecargas**, pero no hay que memorizarlas: todos los nombres se
construyen con la misma fórmula.

### Estructura General

```
[Try] + Prefijo + [Contexto] + [Async]
```

| Pieza | Obligatoria | Significado |
| --- | :---: | --- |
| `Try` | No | Envuelve el delegado en un `try/catch`. La excepción se guarda en `Details["Ex"]`. |
| **Prefijo** | Sí | La operación: `Bind`, `Map`, `ExecSelf`, `Match`, `Projection`, `ChangeReturnResult`… |
| **Contexto** | No | Cuándo/cómo se ejecuta: `If`, `IfFail`, `IfValid`, `IfFailWithValue`, `IfFailWithException`, `IfFailWithoutException`, `Always`, `Ensure`, `While`, `Multi`… |
| `Async` | No | Existe una variante asíncrona (fuente `Task<...>` y/o delegado asíncrono). |

Ejemplos leídos con la fórmula:

| Método | Lectura |
| --- | --- |
| `Bind` | Encadena una operación que devuelve `MlResult`. |
| `TryBind` | Igual, capturando excepciones. |
| `BindIfFailAsync` | Encadena **solo si el resultado venía fallido**, de forma asíncrona. |
| `TryMapIfFailWithExceptionAsync` | Transforma en caso de fallo, recibiendo la excepción original, con `try/catch` y `await`. |
| `ExecSelfIfValid` | Efecto secundario solo si el resultado es válido; **no cambia el resultado**. |

### Prefijos Principales

#### **Bind** — Composición y encadenamiento

El delegado **devuelve `MlResult<TReturn>`**. Se usa cuando el siguiente paso puede fallar.

```csharp
MlResult<Factura> resultado = ValidarPedido(pedido)      // MlResult<Pedido>
                                  .Bind(CalcularTotales)  // MlResult<PedidoValorado>
                                  .Bind(EmitirFactura);   // MlResult<Factura>
```

Detalle: [`Types/MlResultActionsBind.md`](./Types/MlResultActionsBind.md).

#### **Map** — Transformación de valores

El delegado **devuelve un valor normal**, no un `MlResult`. Se usa para conversiones que no pueden
fallar (o que, si fallan, lo hacen lanzando y entonces usarás `TryMap`).

```csharp
MlResult<ClienteDto> dto = ObtenerCliente(id).Map(c => c.ToDto());
```

| ¿El delegado puede fallar? | Usa |
| --- | --- |
| Sí, y devuelve `MlResult<T>` | `Bind` |
| No, es una conversión pura | `Map` |
| Puede lanzar una excepción | `TryMap` / `TryBind` |

Detalle: [`Types/MlResultActionsMap.md`](./Types/MlResultActionsMap.md).

#### **ExecSelf** — Efectos secundarios

Ejecuta una acción (log, métrica, auditoría, evento) y **devuelve el resultado original intacto**. Es
la forma de instrumentar una tubería sin romperla.

```csharp
await ConfirmarPedidoAsync(pedido)
          .ExecSelfIfValidAsync(p  => _log.LogInformation("Pedido {Id} confirmado", p.Id))
          .ExecSelfIfFailAsync (er => _log.LogWarning("Fallo: {E}", er.ToErrorsDescription()));
```

Detalle: [`Types/MlResultActionsExecSelf.md`](./Types/MlResultActionsExecSelf.md).

#### **Match** — Salida del `MlResult`

Es el **único punto de salida**: obliga a tratar los dos estados y devuelve un valor normal.

```csharp
return resultado.Match(
    valid: dto     => Ok(dto),
    fail : errores => BadRequest(errores.ToErrorsMessages()));
```

Detalle: [`Types/MlResultActionsMatch.md`](./Types/MlResultActionsMatch.md).

### Modificador `Try`

#### **Try[Operación]**

Cualquier operación con prefijo `Try` ejecuta el delegado dentro de un `try/catch`. Si se lanza una
excepción, el resultado pasa a `Fail` y la excepción queda **guardada** en `Details["Ex"]`, disponible
después con `GetDetailException()`.

Todas las variantes `Try*` aceptan el mensaje de error de dos formas:

```csharp
// a) Mensaje fijo
.TryBind(GuardarEnDisco, "No se pudo guardar el fichero")

// b) Mensaje construido a partir de la excepción
.TryBind(GuardarEnDisco, ex => $"No se pudo guardar: {ex.Message}")
```

Si no indicas mensaje, se usa `DEFAULT_EX_ERROR_MESSAGE(ex)`.

```csharp
MlResult<Configuracion> config = rutaFichero
        .ToMlResultValid()
        .TryMap(File.ReadAllText,                  ex => $"No se pudo leer: {ex.Message}")
        .TryMap(JsonSerializer.Deserialize<Configuracion>!, ex => $"JSON inválido: {ex.Message}");
```

### Contextos de Ejecución

| Contexto | Se ejecuta cuando… | Qué recibe el delegado |
| --- | --- | --- |
| *(ninguno)* | El resultado es **válido** | El valor `T` |
| `IfValid` | El resultado es válido | El valor `T` |
| `If` | Se cumple un **predicado** sobre el valor | El valor `T` |
| `IfFail` | El resultado es **fallido** | `MlErrorsDetails` |
| `IfFailWithValue` | Es fallido **y** hay un valor en `Details["Value"]` | El valor original |
| `IfFailWithException` | Es fallido **y** hay una excepción en `Details["Ex"]` | La `Exception` |
| `IfFailWithoutException` | Es fallido **y no** hay excepción (fallo de negocio/validación) | `MlErrorsDetails` |
| `Always` | **Siempre**, sea válido o fallido | Nada, o los dos delegados |
| `Ensure` | Solo en `Map`: valida el valor con un predicado | El valor `T` |

Los contextos `IfFailWith*` son la razón por la que merece la pena guardar contexto en `Details`:
permiten **distinguir la causa del fallo** sin `if` anidados.

```csharp
await ProcesarPagoAsync(pago)
          // Reintento solo si falló por un problema técnico.
          .BindIfFailWithExceptionAsync(ex => ex is TimeoutException or HttpRequestException
                                                  ? ReintentarAsync(pago)
                                                  : ex.ToMlResultFailAsync<Recibo>())
          // Los fallos de validación NO se reintentan: se devuelven al usuario.
          .MapIfFailWithoutExceptionAsync(er => Recibo.Rechazado(er));
```

### Sufijos de Asincronía

#### **[Operación]Async**

El sufijo `Async` cubre **tres ejes independientes**, y de ahí sale el número de sobrecargas:

| Eje | Variantes |
| --- | --- |
| Origen | `MlResult<T>` o `Task<MlResult<T>>` |
| Delegado | síncrono (`Func<T, MlResult<R>>`) o asíncrono (`Func<T, Task<MlResult<R>>>`) |
| Mensaje de error (`Try*`) | ninguno, `string`, o `Func<Exception, string>` |

Esto permite escribir tuberías mixtas sin `await` intermedios ni variables temporales:

```csharp
public Task<MlResult<PedidoDto>> ConfirmarAsync(Guid id) =>
    _repo.ObtenerAsync(id)                          // Task<MlResult<Pedido>>
         .BindAsync(ValidarEstado)                  // delegado SÍNCRONO
         .BindAsync(_stock.ReservarAsync)           // delegado ASÍNCRONO
         .TryBindAsync(_pagos.CobrarAsync, ex => $"Cobro fallido: {ex.Message}")
         .MapAsync(p => p.ToDto());
```

> 💡 **Regla práctica**: si *algo* en la tubería es asíncrono, usa la variante `Async` en todos los
> pasos siguientes; el compilador elegirá la sobrecarga correcta según tu delegado.

---

## Análisis Detallado de Métodos

Esta sección es un **mapa de navegación**. El detalle exhaustivo de cada familia (con el número real de
sobrecargas de cada método) está en [`__Doc/Types/`](./Types/README.md).

| Archivo fuente | Contenido | Referencia |
| --- | --- | --- |
| `Types/MlResult.cs` | El tipo, fábricas y conversiones implícitas | [`MlResult.md`](./Types/MlResult.md) |
| `Types/MlResultActionsBind.cs` | `Bind`, `BindMulti`, `BindIf`, `BindIfFail*`, `BindAlways`, `TryBindBuild*` | [`MlResultActionsBind.md`](./Types/MlResultActionsBind.md) |
| `Types/MlResultActionsMap.cs` | `Map`, `MapEnsure`, `MapIf`, `MapIfFail*`, `MapAlways` | [`MlResultActionsMap.md`](./Types/MlResultActionsMap.md) |
| `Types/MlResultActionsMatch.cs` | `Match`, `TryMatch` y las sobrecargas «todo en uno» | [`MlResultActionsMatch.md`](./Types/MlResultActionsMatch.md) |
| `Types/MlResultActionsExecSelf.cs` | `ExecSelf*`: efectos secundarios sin alterar el resultado | [`MlResultActionsExecSelf.md`](./Types/MlResultActionsExecSelf.md) |
| `Types/MlResultActionsSeveral.cs` | `Combine`, `NullToFailed`, `EmptyToFailed`, `BoolToResult`, `Do` | [`MlResultActionsSeveral.md`](./Types/MlResultActionsSeveral.md) |
| `Types/MlResultBucles.cs` | `Projection*`, `ProjectionSplit*`, `Fusion*` | [`MlResultBucles.md`](./Types/MlResultBucles.md) |
| `Types/MlResultTransformations.cs` | `ToMlResult*`, `TryToMlResult*`, boxing | [`MlResultTransformations.md`](./Types/MlResultTransformations.md) |
| `Types/MlResultChangeReturnResult.cs` | Cambiar el tipo de retorno conservando el estado | [`MlResultChangeReturnResult.md`](./Types/MlResultChangeReturnResult.md) |
| `Types/MlResultActions.cs` | Enriquecer errores, transportar datos, acceso seguro | [`MlResultActions.md`](./Types/MlResultActions.md) |
| `Types/MlResultActionsErrorsDetails.cs` | Leer, escribir y fusionar los `Details` del error | [`MlResultActionsErrorsDetails.md`](./Types/MlResultActionsErrorsDetails.md) |
| `Types/Errors/*.cs` | `MlError`, `MlErrorsDetails` y sus acciones | [`MlResultErrors.md`](./Types/MlResultErrors.md) |

### Cómo elegir la operación adecuada

| Lo que quieres hacer | Operación |
| --- | --- |
| Encadenar un paso que **puede fallar** | `Bind` |
| Transformar el valor con una función **que no falla** | `Map` |
| Validar el valor con un predicado | `MapEnsure` |
| Ejecutar un `if` dentro de la tubería | `BindIf` / `MapIf` |
| Recuperarte de un fallo con un valor por defecto | `MapIfFail` |
| Recuperarte de un fallo con otra operación que puede fallar | `BindIfFail` |
| Reaccionar distinto según **la excepción** | `BindIfFailWithException` |
| Reaccionar solo a fallos **de negocio** (sin excepción) | `BindIfFailWithoutException` |
| Registrar/auditar sin alterar el flujo | `ExecSelf*` |
| Ejecutar código **siempre** (limpieza, caché) | `BindAlways` / `MapAlways` |
| Combinar resultados de **tipos distintos** | `Combine` |
| Construir un objeto acumulando **todos** los errores | `TryBindBuild` |
| Detenerte en el primer error al construir | `TryBindBuildWhile` |
| Recorrer una colección (todo o nada) | `Projection` |
| Recorrer una colección tolerando fallos | `ProjectionSplit` |
| Salir del `MlResult` | `Match` |

---

## Gestión de Errores

### Un error no es solo un texto

En esta librería un fallo transporta tres cosas:

1. **Uno o varios mensajes** (`Errors`), porque las validaciones se acumulan.
2. **La excepción original**, si la hubo, en `Details["Ex"]`.
3. **El contexto que tú decidas añadir**, en el resto de claves de `Details`.

```csharp
MlResult<Recibo> resultado = await ProcesarPagoAsync(pago);

// El error incluye: mensaje + excepción + el DTO que lo provocó.
```

### Enriquecer el error con contexto

```csharp
MlResult<Pedido> resultado = ValidarPedido(dto)
        .AddValueDetailIfFail(dto)                       // guarda el DTO en Details["Value"]
        .AddMlErrorDetailIfFail("Validación de alta");   // añade un mensaje extra
```

Con esto, más adelante puedes recuperar el DTO y, por ejemplo, encolarlo para reintento:

```csharp
resultado.Match(
    valid: p       => Ok(p),
    fail : errores => errores.GetDetailValue<PedidoDto>()
                             .Match(valid: origen => { _cola.Encolar(origen); return Accepted(); },
                                    fail : _      => BadRequest(errores.ToErrorsMessages())));
```

### Acumular errores en lugar de cortocircuitar

`Bind` **cortocircuita**: en cuanto algo falla, el resto no se ejecuta. Cuando quieras mostrar al
usuario *todos* los problemas de una vez, tienes tres herramientas:

| Herramienta | Cuándo |
| --- | --- |
| `Combine` | Varios `MlResult` de **tipos distintos** ya calculados. |
| `FusionErrosIfExists` | Una **colección** de `MlResult<T>` ya calculada. |
| `TryBindBuild` | Construir un objeto ejecutando **todas** las funciones de sus campos. |

```csharp
// El usuario ve los tres errores a la vez, no solo el primero.
MlResult<Alta> alta = dto.ToMlResultValid()
                         .TryBindBuild<AltaDto, Alta>(
                              d => ValidarNombre(d.Nombre).ToMlResultObject(),
                              d => ValidarEmail (d.Email ).ToMlResultObject(),
                              d => ValidarEdad  (d.Edad  ).ToMlResultObject());
```

### Traducir errores a la frontera HTTP

```csharp
public async Task<IActionResult> Post(PedidoDto dto)
{
    var resultado = await _servicio.CrearAsync(dto);

    return await resultado.MatchAsync(
        validAsync: async p => { await _bus.PublicarAsync(p); return Created($"/pedidos/{p.Id}", p); },
        failAsync : er => Task.FromResult(Traducir(er)));
}

private IActionResult Traducir(MlErrorsDetails errores) =>
    errores.GetDetailException()
           .Match(
               valid: ex => ex switch
                            {
                                TimeoutException        => StatusCode(504, errores.ToErrorsMessages()),
                                HttpRequestException    => StatusCode(502, errores.ToErrorsMessages()),
                                UnauthorizedAccessException => Forbid(),
                                _                       => StatusCode(500, errores.ToErrorsMessages())
                            },
               // Sin excepción ⇒ es un fallo de negocio ⇒ 400.
               fail : _  => BadRequest(errores.ToErrorsMessages()));
```

Detalle completo en [`Types/MlResultErrors.md`](./Types/MlResultErrors.md) y
[`Types/MlResultActionsErrorsDetails.md`](./Types/MlResultActionsErrorsDetails.md).

---

## Extensiones y Utilidades

### `EnsureFp` — precondiciones funcionales

Métodos **estáticos** (no de extensión) que devuelven `MlResult<T>`. Sustituyen a las guard clauses con
`throw`: en lugar de interrumpir el flujo con una excepción, colocan el valor en el carril válido o
devuelven un fallo enriquecido con el que se puede seguir componiendo.

La primitiva sigue siendo `That`; el resto son reglas de uso frecuente construidas sobre ella.

```csharp
public MlResult<Pedido> Validar(Pedido pedido) =>
    EnsureFp.NotNullArg(pedido)
            .Bind(p => EnsureFp.NotNullEmptyOrWhitespaceArg(p.Referencia).Map(_ => p))
            .Bind(p => EnsureFp.NotEmptyCollectionArg<List<Linea>, Linea>(p.Lineas).Map(_ => p))
            .Bind(p => EnsureFp.PositiveArg(p.Total).Map(_ => p));
```

#### Las tres variantes de cada regla

Toda regla existe en tres formas, y esa simetría es la clave de la familia:

| Variante | Firma | Cuándo usarla |
| --- | --- | --- |
| Con mensaje | `MaxLength(valor, 50, "El nombre es demasiado largo")` | El mensaje se muestra al usuario o al cliente de la API. |
| Con detalle | `MaxLength(valor, 50, misErrorsDetails)` | Necesitas adjuntar `Details` propios (código de error, contexto…). |
| Con sufijo `…Arg` | `MaxLengthArg(valor, 50)` | Validación interna: el mensaje y el nombre del parámetro se generan solos. |

Las variantes `…Arg` usan `[CallerArgumentExpression]`, así que el compilador captura la expresión que
escribiste y `EnsureFpMessages` compone un texto homogéneo. Además añaden a `Details` las claves
`ParamName` y `Value` (y `Expected` en las reglas numéricas), lo que hace los fallos trazables sin
esfuerzo.

#### Las ocho familias

| Familia | Archivo fuente | Ejemplos representativos |
| --- | --- | --- |
| Núcleo | `EnsureFp.cs`, `EnsureFp.Core.cs` | `That`, `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace`, `TryThat`, `ThatArg` |
| Agregación | `EnsureFp.Aggregation.cs` | `All`, `AllResults`, `AllOrFirst`, `Any` (+ sus versiones `…Async`) |
| Cadenas | `EnsureFp.Strings.cs` | `MaxLength`, `MinLength`, `LengthBetween`, `Matches`, `StartsWith`, `ContainsText`, `IsOneOf` |
| Números | `EnsureFp.Numbers.cs` | `GreaterThan`, `LessOrEqual`, `InRange`, `OutOfRange`, `Positive`, `NotZero` |
| Colecciones | `EnsureFp.Collections.cs` | `NotEmptyCollection`, `CountBetween`, `AllMatch`, `NoDuplicates`, `NoNullItems`, `ContainsItem` |
| Tipos concretos | `EnsureFp.Types.cs` | `NotEmptyGuid`, `IsDefined`, `InFuture`, `IsValidUri`, `IsValidEmail`, `FileExists` |
| `Nullable<T>` | `EnsureFp.Types.cs` | `NotNullValue`, `NotNullValueThat` (devuelven el `T` ya desenvuelto) |
| Asíncronas | `EnsureFp.Async.cs` | `ThatAsync`, `TryThatAsync`, `NotNullAsync`, `NotNullValueAsync` |

Los mensajes automáticos viven centralizados en `EnsureFpMessages.cs`, de modo que un cambio de
redacción es un cambio en un único sitio.

#### Acumular errores en lugar de detenerse

`That` y sus derivados son *fail-fast*. Cuando lo que quieres es devolver **todos** los problemas de
una vez —el caso típico de un formulario o del cuerpo de una petición— la respuesta es `All`:

```csharp
MlResult<ClienteDto> validado =
    EnsureFp.All(dto,
        d => EnsureFp.NotNullEmptyOrWhitespaceArg(d.Nombre).Map(_ => d),
        d => EnsureFp.IsValidEmailArg(d.Email).Map(_ => d),
        d => EnsureFp.InRangeArg(d.Edad, 18, 120).Map(_ => d));
```

`All` ejecuta **todos** los validadores y fusiona los `MlErrorsDetails` resultantes.
`AllOrFirst` conserva la semántica *fail-fast*, y `Any` basta con que una regla pase.

#### Predicados que pueden lanzar

Si el predicado consulta un recurso que puede fallar, `TryThat` captura la excepción y la incorpora al
error en vez de propagarla:

```csharp
MlResult<string> ruta = EnsureFp.TryThat(candidato,
                                        p => File.Exists(p),
                                        ex => $"No se pudo comprobar la ruta: {ex.Message}");
```

Documentación completa —más de 90 reglas, con su semántica ante `null`, sus claves de `Details` y sus
ejemplos— en [`EnsureFp/EnsureFp.md`](./EnsureFp/EnsureFp.md), que además indexa las nueve páginas de
detalle de la familia.

### `Helpers/Extensions` — utilidades transversales

| Extensión | Para qué |
| --- | --- |
| `ToAsync<T>()` | Convierte cualquier valor en `Task<T>`. Imprescindible al mezclar mundos. |
| `With<T>(params Action<T>[])` | Configura un objeto en una expresión (útil dentro de `Map`). |
| `WithAsync<T>(...)` | Igual, para `Task<T>` o con acciones asíncronas. |
| `VoidToAsync<T>(Action<T>)` | Adapta una `Action<T>` a una firma asíncrona. |
| `ToFuncTask(...)` | Convierte funciones síncronas en funciones que devuelven `Task` (5 sobrecargas). |
| `ValidateObject()` | Ejecuta las DataAnnotations y devuelve los `ValidationResult`. |
| `ToNullable<T>()` | `T` → `T?` para tipos valor. |
| `AppendExDetails(Exception)` | Añade una excepción al diccionario de detalles, numerándola (`Ex`, `Ex2`, …). |

```csharp
MlResult<Pedido> pedido = nuevo.ToMlResultValid()
                               .Map(p => p.With(x => x.Fecha  = _reloj.Ahora,
                                                x => x.Estado = Estado.Borrador));
```

Detalle en [`Extensions/Extensions.md`](./Extensions/Extensions.md).

### `Constants` — valores por defecto

| Constante | Valor / uso |
| --- | --- |
| `DEFAULT_ERROR_MESSAGE` | Mensaje usado cuando un `MlError` se crea sin texto. |
| `EX_DESC_KEY` | `"Ex"`: clave de la excepción en `Details`. |
| `VALUE_KEY` | `"Value"`: clave del valor de entrada en `Details`. |
| `DEFAULT_EX_ERROR_MESSAGE(ex)` | Mensaje por defecto de las operaciones `Try*`. |

---

## Patrones de Uso

### 1. Entrar en la tubería

```csharp
MlResult<string> a = texto.ToMlResultValid();               // valor válido
MlResult<Cliente> b = "No encontrado".ToMlResultFail<Cliente>();  // fallo
MlResult<Cliente> c = cliente.NullToFailed("Cliente nulo"); // null ⇒ Fail
MlResult<Config>  d = ((Func<Config>)Cargar).TryToMlResult("No se pudo cargar"); // código que lanza
```

### 2. Validar antes de actuar

```csharp
MlResult<Pedido> resultado = EnsureFp.NotNull(dto, "Datos obligatorios")
                                     .Bind(Validar)
                                     .Bind(Normalizar);
```

### 3. Instrumentar sin ensuciar

```csharp
await resultado.ExecSelfIfValidAsync(p  => _metricas.Incr("pedido.ok"))
               .ExecSelfIfFailAsync (er => _log.LogWarning("{E}", er.ToErrorsDescription()));
```

### 4. Recuperación por capas

```csharp
MlResult<Tarifa> tarifa = await _api.ObtenerAsync(zona)
        .BindIfFailWithExceptionAsync(ex => ex is TimeoutException
                                                ? _cache.ObtenerAsync(zona)
                                                : ex.ToMlResultFailAsync<Tarifa>())
        .MapIfFailWithoutExceptionAsync(_ => Tarifa.PorDefecto);
```

### 5. Salir una sola vez, al final

```csharp
return tarifa.Match(valid: t => Ok(t), fail: er => BadRequest(er.ToErrorsMessages()));
```

### Antipatrones a evitar

| ❌ Antipatrón | ✅ Alternativa |
| --- | --- |
| Comprobar `IsFail` con `if` en cada paso | Encadenar con `Bind` / `Map` |
| Intentar leer el valor directamente | `Match`, o `SecureValidValue()` en infraestructura |
| Lanzar excepciones para el flujo de negocio | Devolver `Fail` con un mensaje claro |
| Envolver toda la tubería en un `try/catch` | Usar las variantes `Try*` en el paso concreto que puede lanzar |
| Devolver `null` | Devolver `MlResult<T>` o usar `NullToFailed` |

---

## Ejemplos Prácticos

### Ejemplo completo: alta de un cliente

```csharp
public Task<MlResult<ClienteDto>> AltaAsync(AltaClienteDto dto) =>
    EnsureFp.NotNullAsync(dto, "Los datos del cliente son obligatorios")
        // 1. Validaciones acumuladas: el usuario ve todos los errores a la vez.
        .BindAsync(d => Validar(d))
        // 2. Regla de negocio: el email no puede existir ya.
        .BindAsync(d => _repo.ExisteEmailAsync(d.Email)
                             .BindAsync(existe => existe
                                                      ? $"El email {d.Email} ya está registrado"
                                                            .ToMlResultFailAsync<AltaClienteDto>()
                                                      : d.ToMlResultValidAsync()))
        // 3. Construcción del dominio.
        .MapAsync(d => new Cliente(d.Nombre, d.Email, _reloj.Ahora))
        // 4. Persistencia: puede lanzar ⇒ TryBindAsync.
        .TryBindAsync(c => _repo.InsertarAsync(c),
                      ex => $"No se pudo guardar el cliente: {ex.Message}")
        // 5. Contexto para diagnóstico si algo salió mal.
        .AddValueDetailIfFailAsync(dto)
        // 6. Efectos secundarios, sin alterar el resultado.
        .ExecSelfIfValidAsync(c  => _log.LogInformation("Cliente {Id} creado", c.Id))
        .ExecSelfIfFailAsync (er => _log.LogWarning("Alta rechazada: {E}",
                                                   er.ToErrorsDescription()))
        // 7. Salida del dominio.
        .MapAsync(c => c.ToDto());

private MlResult<AltaClienteDto> Validar(AltaClienteDto d)
{
    IEnumerable<MlResult<string>> validaciones =
    [
        EnsureFp.NotNullEmptyOrWhitespace(d.Nombre, "El nombre es obligatorio"),
        EnsureFp.NotNullEmptyOrWhitespace(d.Email,  "El email es obligatorio"),
        EnsureFp.That(d.Email, d.Email?.Contains('@') == true, "El email no tiene formato válido")
    ];

    return validaciones.FusionErrosIfExists().Map(_ => d);
}
```

Y el controlador, que es la **única** capa que sale del `MlResult`:

```csharp
[HttpPost]
public async Task<IActionResult> Post(AltaClienteDto dto)
{
    var resultado = await _servicio.AltaAsync(dto);

    return resultado.Match(
        valid: cliente => Created($"/clientes/{cliente.Id}", cliente),
        fail : errores => errores.GetDetailException()
                                 .Match(valid: _ => StatusCode(500, errores.ToErrorsMessages()),
                                        fail : _ => BadRequest(errores.ToErrorsMessages())));
}
```

### Ejemplo: importación de un fichero tolerante a errores

```csharp
public async Task<InformeImportacion> ImportarAsync(Stream csv)
{
    var lineas = await LeerLineasAsync(csv);

    var (importados, rechazados) = await lineas.ProjectionSplitAsync(
        async linea => await ParsearAsync(linea)
                                .BindAsync(_repo.InsertarAsync));

    return new InformeImportacion(
        Correctos : importados.Count(),
        Rechazados: rechazados.Select(e => e.ToErrorsDescription()).ToList());
}
```

---

## Índice completo de la documentación

Toda la documentación de `MoralesLarios.OOFP` vive en `__Doc/`. Este es el mapa completo.

### Referencia por archivo de código (`__Doc/Types/`)

Un documento por cada archivo fuente, con **todas** las sobrecargas reales.

| Documento | Archivo fuente | Contenido |
|-----------|----------------|-----------|
| [Índice de tipos](./Types/README.md) | — | Portada de la referencia por tipos |
| [`MlResult.md`](./Types/MlResult.md) | `Types/MlResult.cs` | El tipo raíz, fábricas y conversiones implícitas |
| [`MlResultErrors.md`](./Types/MlResultErrors.md) | `Types/Errors/*.cs` | `MlError`, `MlErrorsDetails`, `ErrorMessage` |
| [`MlResultActions.md`](./Types/MlResultActions.md) | `Types/MlResultActions.cs` | Enriquecer errores, transportar datos, acceso seguro |
| [`MlResultActionsBind.md`](./Types/MlResultActionsBind.md) | `Types/MlResultActionsBind.cs` | `Bind`, `BindMulti`, `BindIf`, `BindIfFail*`, `BindAlways`, `TryBindBuild*` |
| [`MlResultActionsMap.md`](./Types/MlResultActionsMap.md) | `Types/MlResultActionsMap.cs` | `Map`, `MapEnsure`, `MapIf`, `MapIfFail*`, `MapAlways` |
| [`MlResultActionsMatch.md`](./Types/MlResultActionsMatch.md) | `Types/MlResultActionsMatch.cs` | `Match`, `TryMatch` y las sobrecargas «todo en uno» |
| [`MlResultActionsExecSelf.md`](./Types/MlResultActionsExecSelf.md) | `Types/MlResultActionsExecSelf.cs` | `ExecSelf*`: efectos laterales sin alterar el resultado |
| [`MlResultActionsSeveral.md`](./Types/MlResultActionsSeveral.md) | `Types/MlResultActionsSeveral.cs` | `EmptyToFailed`, `NullToFailed`, `BoolToResult`, `Combine`, `Do` |
| [`MlResultActionsErrorsDetails.md`](./Types/MlResultActionsErrorsDetails.md) | `Types/MlResultActionsErrorsDetails.cs` | Leer, escribir y fusionar los `Details` del error |
| [`MlResultBucles.md`](./Types/MlResultBucles.md) | `Types/MlResultBucles.cs` | `Projection*`, `ProjectionSplit*`, `Fusion*` |
| [`MlResultTransformations.md`](./Types/MlResultTransformations.md) | `Types/MlResultTransformations.cs` | `ToMlResult*`, `TryToMlResult*`, boxing |
| [`MlResultChangeReturnResult.md`](./Types/MlResultChangeReturnResult.md) | `Types/MlResultChangeReturnResult.cs` | Cambiar el tipo de retorno conservando el estado |

### Guías por familia de operadores

#### `Bind` — encadenar operaciones que devuelven `MlResult`

| # | Documento | Tema |
|---|-----------|------|
| 2 | [`2_MlResultActions.md`](./Bind/2_MlResultActions.md) | Utilidades base y acceso seguro al valor |
| 3 | [`3_Bind.md`](./Bind/3_Bind.md) | ⭐ `Bind` y `TryBind`: el operador fundamental |
| 4 | [`4_BindMulti.md`](./Bind/4_BindMulti.md) | `BindMulti`: elegir rama según condiciones |
| 5 | [`5_BindIf.md`](./Bind/5_BindIf.md) | `BindIf`: ejecutar solo si se cumple un predicado |
| 6 | [`6_BindIfFail.md`](./Bind/6_BindIfFail.md) | `BindIfFail`: recuperación desde el fallo |
| 7 | [`7_BindIfFailWithValue.md`](./Bind/7_BindIfFailWithValue.md) | Recuperar usando el valor original guardado en `Details` |
| 8 | [`8_BindIfFailWithException.md`](./Bind/8_BindIfFailWithException.md) | Recuperar en función de la excepción capturada |
| 9 | [`9_BindIfFailWithoutException.md`](./Bind/9_BindIfFailWithoutException.md) | Distinguir fallos de negocio de fallos técnicos |
| 10 | [`10_BindAlways.md`](./Bind/10_BindAlways.md) | `BindAlways`: ejecutar en ambas ramas |
| 11 | [`11_BindSaveValueInDetailsIfFaildFuncResultAsync.md`](./Bind/11_BindSaveValueInDetailsIfFaildFuncResultAsync.md) | Guardar el valor de entrada en `Details` al fallar |

#### `Map` — transformar el valor sin salir del carril

| # | Documento | Tema |
|---|-----------|------|
| 1 | [`1_Map.md`](./Map/1_Map.md) | ⭐ `Map` y `TryMap`: transformación pura |
| 2 | [`2_MapEnsure.md`](./Map/2_MapEnsure.md) | `MapEnsure`: validar y transformar |
| 3 | [`3_MapIf.md`](./Map/3_MapIf.md) | `MapIf`: transformación condicional |
| 4 | [`4_MapIfFail.md`](./Map/4_MapIfFail.md) | `MapIfFail`: valor de reserva ante fallo |
| 5 | [`5_MapIfFailWithValue.md`](./Map/5_MapIfFailWithValue.md) | Reserva usando el valor original (`VALUE_KEY`) |
| 6 | [`6_MapIfFailWithException.md`](./Map/6_MapIfFailWithException.md) | Reserva según la excepción (`EX_DESC_KEY`) |
| 7 | [`7_MapIfFailWithoutException.md`](./Map/7_MapIfFailWithoutException.md) | Reserva solo para fallos sin excepción |
| 8 | [`8_MapAlways.md`](./Map/8_MapAlways.md) | `MapAlways`: transformar ambas ramas a un tipo común |

#### `Match` — salir del carril

| # | Documento | Tema |
|---|-----------|------|
| 1 | [`1_Match.md`](./Match/1_Match.md) | ⭐ `Match` y `TryMatch`: materializar el resultado final |
| 2 | [`2_MatchAll.md`](./Match/2_MatchAll.md) | Sobrecargas «todo en uno» y patrones de salida |

#### `ExecSelf` — efectos laterales sin alterar el resultado

| # | Documento | Tema |
|---|-----------|------|
| 1 | [`1_ExecSelf.md`](./ExecSelf/1_ExecSelf.md) | ⭐ `ExecSelf` y `TryExecSelf`: instrumentar la tubería |
| 2 | [`2_ExecSelfIfValid.md`](./ExecSelf/2_ExecSelfIfValid.md) | Solo en la rama válida |
| 3 | [`3_ExecSelfIfFail.md`](./ExecSelf/3_ExecSelfIfFail.md) | Solo en la rama de fallo |
| 4 | [`4_ExecSelfIfFailWithValue.md`](./ExecSelf/4_ExecSelfIfFailWithValue.md) | Al fallar, usando el valor original |
| 5 | [`5_ExecSelfIfFailWithException.md`](./ExecSelf/5_ExecSelfIfFailWithException.md) | Al fallar, con la excepción capturada |
| 6 | [`6_ExecSelfIfFailWithoutException.md`](./ExecSelf/6_ExecSelfIfFailWithoutException.md) | Al fallar sin excepción (error de negocio) |

#### `Several` — puentes desde el mundo imperativo

| # | Documento | Tema |
|---|-----------|------|
| 1 | [`1_EmptyToFailed.md`](./Several/1_EmptyToFailed.md) | Rechazar colecciones vacías |
| 2 | [`2_NullToFailed.md`](./Several/2_NullToFailed.md) | Convertir `null` en fallo explícito |
| 3 | [`3_BoolToResult.md`](./Several/3_BoolToResult.md) | Convertir un `bool` en `MlResult` |
| 4 | [`4_Combine.md`](./Several/4_Combine.md) | `Combine` y `Do` ⚠️ (**no** acumula errores) |

#### `EnsureFp` — precondiciones antes de entrar al carril

| # | Documento | Tema |
|---|-----------|------|
| — | [`EnsureFp.md`](./EnsureFp/EnsureFp.md) | ⭐ Índice de la familia: convenciones, tabla de decisión y mapa de páginas |
| 1 | [`1_EnsureFpCore.md`](./EnsureFp/1_EnsureFpCore.md) | `That`, `NotNull`, `NotEmpty`, `NotNullEmptyOrWhitespace`, `TryThat` y variantes `…Arg` |
| 2 | [`2_EnsureFpAggregation.md`](./EnsureFp/2_EnsureFpAggregation.md) | `All`, `AllResults`, `AllOrFirst`, `Any`: acumular todos los errores |
| 3 | [`3_EnsureFpStrings.md`](./EnsureFp/3_EnsureFpStrings.md) | Longitudes, expresiones regulares, prefijos/sufijos, subcadenas y conjuntos |
| 4 | [`4_EnsureFpNumbers.md`](./EnsureFp/4_EnsureFpNumbers.md) | Comparaciones, rangos y signo sobre `IComparable<T>` e `INumber<T>` |
| 5 | [`5_EnsureFpCollections.md`](./EnsureFp/5_EnsureFpCollections.md) | Cardinalidad, duplicados, elementos nulos y predicados por elemento |
| 6 | [`6_EnsureFpTypes.md`](./EnsureFp/6_EnsureFpTypes.md) | `Guid`, `enum`, fechas, `Uri`, email y rutas del sistema de archivos |
| 7 | [`7_EnsureFpNullables.md`](./EnsureFp/7_EnsureFpNullables.md) | `Nullable<T>`: desenvolver el valor validando a la vez |
| 8 | [`8_EnsureFpAsync.md`](./EnsureFp/8_EnsureFpAsync.md) | Fuentes `Task<T>`, predicados asíncronos y `CancellationToken` |
| 9 | [`9_EnsureFpMessages.md`](./EnsureFp/9_EnsureFpMessages.md) | `EnsureFpMessages`: las plantillas de los mensajes automáticos |

### Utilidades y transformaciones

| Documento | Tema |
|-----------|------|
| [`EnsureFp/EnsureFp.md`](./EnsureFp/EnsureFp.md) | Índice de las más de 90 precondiciones y sus nueve páginas de detalle |
| [`Transformations/Transformations.md`](./Transformations/Transformations.md) | `ToMlResultValid`, `ToMlResultFail`, `TryToMlResult*`, boxing |
| [`Extensions/Extensions.md`](./Extensions/Extensions.md) | `ToAsync`, `With`, `ToFuncTask`, `AppendExDetails`, `Constants` |
| [`Bucle/Bucles.md`](./Bucle/Bucles.md) | `Projection`, `ProjectionWhile`, `ProjectionParallelAsync`, `ProjectionSplit` |

### Rutas de lectura recomendadas

**Si es tu primer contacto con la librería:**

1. Este documento (sobre todo [Tipos Fundamentales](#tipos-fundamentales) y
   [Convención de Nombres](#sistema-de-convención-de-nombres))
2. [`Types/MlResult.md`](./Types/MlResult.md) — el tipo central
3. [`Types/MlResultErrors.md`](./Types/MlResultErrors.md) — cómo se transporta el error
4. [`Bind/3_Bind.md`](./Bind/3_Bind.md) y [`Map/1_Map.md`](./Map/1_Map.md) — los dos operadores básicos
5. [`Match/1_Match.md`](./Match/1_Match.md) — cómo salir del carril

**Si ya sabes ROP y quieres empezar a escribir código:**

1. [`EnsureFp/EnsureFp.md`](./EnsureFp/EnsureFp.md) — entrar al carril validando (y [`2_EnsureFpAggregation.md`](./EnsureFp/2_EnsureFpAggregation.md) si necesitas acumular errores)
2. [`Transformations/Transformations.md`](./Transformations/Transformations.md) — envolver código que lanza
3. [`Bind/3_Bind.md`](./Bind/3_Bind.md) → [`Map/1_Map.md`](./Map/1_Map.md) → [`Match/1_Match.md`](./Match/1_Match.md)
4. [`ExecSelf/1_ExecSelf.md`](./ExecSelf/1_ExecSelf.md) — logging sin romper la cadena

**Si necesitas manejar colecciones:**

1. [`Bucle/Bucles.md`](./Bucle/Bucles.md) — las cuatro estrategias de proyección
2. [`Several/1_EmptyToFailed.md`](./Several/1_EmptyToFailed.md) — rechazar lo vacío
3. [`Several/4_Combine.md`](./Several/4_Combine.md) — ⚠️ ojo: `Combine` **no** acumula errores

**Si estás depurando un fallo y quieres recuperarte:**

1. [`Types/MlResultErrors.md`](./Types/MlResultErrors.md) — leer `Errors` y `Details`
2. [`Bind/6_BindIfFail.md`](./Bind/6_BindIfFail.md) — recuperación con lógica
3. [`Map/4_MapIfFail.md`](./Map/4_MapIfFail.md) — valor de reserva
4. [`Bind/8_BindIfFailWithException.md`](./Bind/8_BindIfFailWithException.md) — según la excepción
5. [`Bind/9_BindIfFailWithoutException.md`](./Bind/9_BindIfFailWithoutException.md) — negocio vs. técnico

---

## Volver arriba

- [README del proyecto `MoralesLarios.OOFP`](../README.md)
- [README general de la solución](../../README.md)
