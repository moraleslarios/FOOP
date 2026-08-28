# Extensions — Utilidades transversales

## Índice

1. [Introducción](#introducción)
2. [`ToAsync` — la extensión más usada de la librería](#toasync--la-extensión-más-usada-de-la-librería)
3. [`With` / `WithAsync` — mutación fluida](#with--withasync--mutación-fluida)
4. [`ToFuncTask` — adaptar delegados síncronos a asíncronos](#tofunctask--adaptar-delegados-síncronos-a-asíncronos)
5. [`AppendExDetails` — acumular excepciones sin sobrescribir](#appendexdetails--acumular-excepciones-sin-sobrescribir)
6. [`ValidateObject` — DataAnnotations en bruto](#validateobject--dataannotations-en-bruto)
7. [`ToNullable` y `VoidToAsync`](#tonullable-y-voidtoasync)
8. [Las constantes: `Constants`](#las-constantes-constants)
9. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
10. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
11. [Ejemplos Prácticos](#ejemplos-prácticos)
12. [Mejores Prácticas](#mejores-prácticas)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

Esta página documenta las utilidades transversales de la librería, repartidas en tres
archivos del namespace `MoralesLarios.OOFP.Helpers`:

| Archivo | Contenido |
|---------|-----------|
| `Helpers/Extensions/ParallelExtensions.cs` | `ToAsync<T>` |
| `Helpers/Extensions/Extensions.cs` | `With`, `ToFuncTask`, `AppendExDetails`, `ValidateObject`, `ToNullable`, `VoidToAsync` |
| `Helpers/Constants.cs` | `EX_DESC_KEY`, `VALUE_KEY`, mensajes por defecto |

Son piezas pequeñas, pero conocerlas evita mucho código repetitivo: `ToAsync` aparece en casi
todas las tuberías asíncronas y `ToFuncTask` resuelve los problemas de inferencia de tipos
más frecuentes.

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## `ToAsync` — la extensión más usada de la librería

El archivo `ParallelExtensions.cs` contiene **un único método**, y es probablemente el más
invocado de todo el proyecto:

```csharp
public static class ParallelExtensions
{
    public static Task<T> ToAsync<T>(this T value) => Task.FromResult(value);
}
```

Envuelve cualquier valor en un `Task<T>` ya completado. Su razón de ser es **encajar valores
síncronos en cadenas asíncronas** sin escribir `Task.FromResult` a mano:

```csharp
// Dentro de una cadena async, MapAsync espera un Func<T, Task<TResult>>
var r = await ObtenerClienteAsync(id)
                  .MapAsync(c => c.ToDto().ToAsync());     // ← ToAsync ajusta la firma

// Devolver un valor constante desde un método asíncrono sin async/await
public Task<MlResult<int>> Cero() => 0.ToMlResultValid().ToAsync();
```

⚠️ **`ToAsync` no crea concurrencia.** `Task.FromResult` devuelve una tarea **ya
completada**: no hay hilo, ni E/S, ni paralelismo. El nombre del archivo
(`ParallelExtensions`) es engañoso.

```csharp
// ⚠️ Esto NO ejecuta nada en paralelo
var t = CalculoLento().ToAsync();     // CalculoLento() se ejecuta ANTES, de forma síncrona

// ✅ Para paralelismo real necesitas Task.Run o E/S asíncrona de verdad
var t = Task.Run(() => CalculoLento());
```

🔑 **Regla mnemotécnica:** `ToAsync` es un **adaptador de firmas**, no un acelerador.

---

## `With` / `WithAsync` — mutación fluida

```csharp
public static T With<T>(this T source, params Action<T>[] changes)
    where T : class
{
    foreach (var change in changes)
        change(source);

    return source;
}
```

Aplica una lista de acciones al objeto y lo devuelve, permitiendo encadenar modificaciones:

```csharp
var pedido = new Pedido()
                 .With(p => p.ClienteId = 42,
                       p => p.Fecha     = DateTime.UtcNow,
                       p => p.Estado    = EstadoPedido.Borrador);
```

⚠️ **Muy importante: `With` MUTA el objeto original.** No hace ninguna copia. Es lo contrario
de la expresión `with` de C# para `record`, que sí crea un objeto nuevo:

```csharp
var original = new Pedido { Estado = EstadoPedido.Borrador };

var otro = original.With(p => p.Estado = EstadoPedido.Confirmado);

// ⚠️ original y otro son EL MISMO objeto; original.Estado ya es Confirmado
Console.WriteLine(ReferenceEquals(original, otro));   // true
```

💡 **Recomendación:** en un proyecto que apuesta por el estilo funcional, prefiere
`record` + expresión `with` de C#, que preserva la inmutabilidad:

```csharp
// ✅ Inmutable: crea un objeto nuevo
var confirmado = pedido with { Estado = EstadoPedido.Confirmado };

// ⚠️ Mutable: modifica el existente
var confirmado = pedido.With(p => p.Estado = EstadoPedido.Confirmado);
```

`With` es útil sobre todo para **inicializar objetos legados** con setters públicos y muchas
propiedades, o para configurar objetos de infraestructura.

Las variantes asíncronas (`WithAsync`) existen para encadenar sobre un `Task<T>`:

```csharp
var pedido = await ObtenerPedidoAsync(id)
                       .WithAsync(p => p.UltimoAcceso = DateTime.UtcNow);
```

---

## `ToFuncTask` — adaptar delegados síncronos a asíncronos

Cinco sobrecargas que convierten delegados síncronos en su equivalente asíncrono. Resuelven
los errores de inferencia de tipos más molestos de la librería:

```csharp
public static Func<T, Task<TResult>> ToFuncTask<T, TResult>(this Func<T, TResult> func)
    => x => func(x).ToAsync();

public static Func<MlErrorsDetails, Task<TResult>> ToFuncTask<TResult>(this Func<MlErrorsDetails, TResult> func)
    => errorsDetails => func(errorsDetails).ToAsync();

public static Func<T, Task> ToFuncTask<T>(this Action<T> action)
    => x => { action(x); return Task.CompletedTask; };

public static Func<MlErrorsDetails, Task> ToFuncTask(this Action<MlErrorsDetails> action)
    => errorsDetails => { action(errorsDetails); return Task.CompletedTask; };

public static Func<Task> ToFuncTask(this Action action)
    => () => { action(); return Task.CompletedTask; };
```

🔑 **Cuándo lo necesitas:** cuando tienes un método síncrono ya escrito y quieres pasarlo a
un operador `*Async` que espera un delegado asíncrono.

```csharp
// Un logger síncrono ya existente
void Registrar(MlErrorsDetails err) => _log.LogWarning(err.ToErrorsDescription());

// ❌ No encaja: ExecSelfIfFailAsync espera Func<MlErrorsDetails, Task>
// await resultado.ExecSelfIfFailAsync(Registrar);

// ✅ Con ToFuncTask
Action<MlErrorsDetails> accion = Registrar;
await resultado.ExecSelfIfFailAsync(accion.ToFuncTask());

// ✅ Alternativa sin ToFuncTask: lambda con ToAsync
await resultado.ExecSelfIfFailAsync(err => { Registrar(err); return Task.CompletedTask; });
```

⚠️ Las dos sobrecargas específicas de `MlErrorsDetails` existen porque el compilador **no
puede inferir** `T = MlErrorsDetails` cuando la sobrecarga genérica también encaja. Es un
apaño necesario, no un capricho.

---

## `AppendExDetails` — acumular excepciones sin sobrescribir

```csharp
public static Dictionary<string, object> AppendExDetails(this Dictionary<string, object> source, Exception ex)
{
    var exKeys = source.Keys.Where(x => x.StartsWith(EX_DESC_KEY)).ToList();

    var exKey = exKeys.Any() ? $"{EX_DESC_KEY}{exKeys.Count + 1}" : EX_DESC_KEY;

    var result = source.ToDictionary(x => x.Key, x => x.Value);

    result.Add(exKey, ex);

    return result;
}
```

Añade una excepción al diccionario de `Details` **generando una clave nueva** si ya había
otra. Como `EX_DESC_KEY` vale `"Ex"`, las claves resultantes son:

| Excepción | Clave |
|-----------|-------|
| 1.ª | `"Ex"` |
| 2.ª | `"Ex2"` |
| 3.ª | `"Ex3"` |

🔑 **Por qué importa:** es lo que permite que un resultado fallido acumule **varias**
excepciones (la del negocio y la del logger que también falló) sin que una tape a la otra.
Es la base de `AppendExErrorDetail` en `MlErrorsDetails`.

```csharp
// Devuelve un diccionario NUEVO: el original no se modifica
var detalles = new Dictionary<string, object> { ["PedidoId"] = 42 };

var conUna = detalles.AppendExDetails(exNegocio);    // { PedidoId, Ex }
var conDos = conUna.AppendExDetails(exLogger);       // { PedidoId, Ex, Ex2 }
```

⚠️ **Cuidado con la numeración.** El cálculo `$"{EX_DESC_KEY}{exKeys.Count + 1}"` usa el
número de claves existentes, así que si alguien añade manualmente una clave que empiece por
`"Ex"` (por ejemplo `"ExtraInfo"`), la numeración se descuadra. **No uses claves propias que
empiecen por `Ex`.**

```csharp
// ❌ "ExtraInfo" empieza por "Ex": rompe la numeración de excepciones
var detalles = new Dictionary<string, object> { ["ExtraInfo"] = "..." };

// ✅ Usa nombres que no colisionen
var detalles = new Dictionary<string, object> { ["InfoAdicional"] = "..." };
```

---

## `ValidateObject` — DataAnnotations en bruto

```csharp
public static IEnumerable<ValidationResult> ValidateObject(this object source)
{
    var valContext = new ValidationContext(source, null, null);
    var resultado  = new List<ValidationResult>();

    Validator.TryValidateObject(source, valContext, resultado, true);

    return resultado;
}
```

Ejecuta la validación de **DataAnnotations** sobre un objeto y devuelve la lista de
resultados. Devuelve una **colección vacía si todo es válido**.

⚠️ **No devuelve un `MlResult`.** Es una utilidad de bajo nivel; tienes que convertirla tú:

```csharp
// Puente manual hacia el carril
public static MlResult<T> ValidarAnotaciones<T>(T objeto)
{
    var errores = objeto!.ValidateObject().ToList();

    return errores.Any()
               ? errores.Select(e => e.ErrorMessage ?? "Error de validación").ToMlResultFail<T>()
               : objeto.ToMlResultValid();
}
```

💡 Para uso real, mejor el paquete
**`MoralesLarios.OOFP.Validation.Dataannotations`**, que ya integra DataAnnotations con
`MlResult` de forma completa (incluida la información de los miembros afectados).

⚠️ El último parámetro de `TryValidateObject` es `true`, es decir **validación recursiva de
todas las propiedades**. Puede ser costoso en grafos de objetos grandes.

---

## `ToNullable` y `VoidToAsync`

### `ToNullable<T>`

```csharp
public static T? ToNullable<T>(this T source) where T : struct => source;
```

Convierte un value type en su equivalente `Nullable<T>`. Es puramente sintáctico
(el compilador ya hace esta conversión de forma implícita), pero ayuda en expresiones donde
necesitas forzar el tipo:

```csharp
int  edad     = 30;
int? nullable = edad.ToNullable();

// Uso típico: unificar tipos en un operador ternario o en un Map
var r = resultado.Map(x => x.Activo ? x.Edad.ToNullable() : null);
```

### `VoidToAsync<T>`

```csharp
public static Task VoidToAsync<T>(this T source, Action<T> voidAction)
{
    voidAction(source);
    return Task.CompletedTask;
}
```

Ejecuta una acción sobre el valor y devuelve `Task.CompletedTask`.

⚠️ **La acción se ejecuta de forma síncrona, antes de devolver la tarea.** No hay
asincronía real, igual que en `ToAsync`.

```csharp
// Encajar un efecto lateral síncrono en una firma que pide Task
await pedido.VoidToAsync(p => _log.LogInformation("Procesando {Id}", p.Id));
```

💡 En la práctica se usa poco: `ToFuncTask` cubre los mismos casos con mejor encaje en los
operadores de la librería.

---

## Las constantes: `Constants`

```csharp
public static class Constants
{
    public static string DEFAULT_ERROR_MESSAGE { get; }
        = "Without custom error message. For more info, view 'Ex(s) details exceptions.";

    public static string EX_DESC_KEY { get; } = "Ex";
    public static string VALUE_KEY   { get; } = "Value";

    public static string DEFAULT_EX_ERROR_MESSAGE(Exception ex)
        => $"An error occurred while executing the function. Error: {ex.Message}.More info in Ex Details.";
}
```

| Constante | Valor | Para qué sirve |
|-----------|-------|----------------|
| `EX_DESC_KEY` | `"Ex"` | Clave de `Details` donde se guardan las excepciones |
| `VALUE_KEY` | `"Value"` | Clave donde `*WithValue` guarda el valor original |
| `DEFAULT_ERROR_MESSAGE` | (texto) | Mensaje cuando no se indica ninguno |
| `DEFAULT_EX_ERROR_MESSAGE(ex)` | (texto + `ex.Message`) | Mensaje por defecto de los `Try*` |

🔑 **Consecuencias prácticas de conocer estas claves:**

1. **No uses `"Ex"`, `"Ex2"`, `"Value"` ni nada que empiece por `Ex`** como claves propias en
   tus `Details`: colisionan con la maquinaria interna.
2. Los mensajes por defecto están **en inglés** y exponen `ex.Message`. Nunca los muestres a
   un usuario final: pasa siempre un mensaje de dominio a los `Try*`.
3. Para leer estos valores usa los accesores oficiales (`GetDetailException()`,
   `GetDetailValue<T>()`), no las claves literales.

```csharp
// ❌ Depender de la constante literal
var ex = resultado.ErrorsDetails.Details["Ex"];

// ✅ Accesor oficial
resultado.ExecSelfIfFailWithException(ex => _log.LogError(ex, "…"));
```

⚠️ Nótese que `DEFAULT_EX_ERROR_MESSAGE` tiene una **errata en el texto original**
(`"Error: {ex.Message}.More info"`, sin espacio tras el punto). Está así en el código fuente.

---

## ⚠️ Particularidades reales del código fuente

**1. `ParallelExtensions` no paraleliza nada.** Contiene solo `ToAsync`, que es
`Task.FromResult`. El nombre del archivo induce a error.

**2. `With` MUTA el objeto original** y devuelve la misma referencia. No confundir con la
expresión `with` de C# para `record`.

**3. `With` exige `where T : class`.** No funciona con `struct`.

**4. `AppendExDetails` numera contando las claves que empiezan por `"Ex"`.** Cualquier clave
propia con ese prefijo descuadra la numeración.

**5. `AppendExDetails` devuelve un diccionario nuevo** (hace `ToDictionary`): el original no
se modifica. Es el único método de este grupo que respeta la inmutabilidad.

**6. `ValidateObject` ignora el `bool` que devuelve `TryValidateObject`** y se basa solo en
la lista. Funciona, pero significa que no distingue "válido" de "no se pudo validar".

**7. `ValidateObject` valida recursivamente** (último parámetro `true`).

**8. `VoidToAsync` ejecuta la acción de forma síncrona** antes de devolver
`Task.CompletedTask`.

**9. Las dos sobrecargas de `ToFuncTask` para `MlErrorsDetails` son necesarias** por
limitaciones de inferencia del compilador, no redundancia.

**10. Las constantes son propiedades `static get`, no `const`.** Da igual en la práctica,
pero significa que no se pueden usar en atributos ni en `switch` de constantes.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Encajar un valor en una cadena asíncrona | `valor.ToAsync()` |
| Pasar un método síncrono a un operador `*Async` | `accion.ToFuncTask()` |
| Inicializar un objeto legado con muchos setters | `objeto.With(...)` |
| Copia inmutable de un `record` | Expresión `with` de C# (**no** `With`) |
| Añadir una excepción a `Details` sin perder la anterior | `detalles.AppendExDetails(ex)` |
| Validar DataAnnotations | `MoralesLarios.OOFP.Validation.Dataannotations` (mejor que `ValidateObject`) |
| Convertir un value type a nullable | `valor.ToNullable()` |
| Paralelismo real | `Task.Run` / E/S asíncrona (**no** `ToAsync`) |
| Leer la excepción de un fallo | `GetDetailException()` |

---

## Ejemplos Prácticos

### Ejemplo 1: `ToAsync` para cerrar cadenas asíncronas

```csharp
public class TarifaService
{
    public Task<MlResult<TarifaDto>> ObtenerAsync(int id)
        => EnsureFp.ThatAsync(id, id > 0, "El identificador debe ser positivo")
                   .BindAsync(i => _repo.BuscarAsync(i)
                                        .NullToFailedAsync($"No existe la tarifa {i}"))
                   // MapAsync espera Func<T, Task<TResult>>: ToAsync ajusta la firma
                   .MapAsync(t => t.ToDto().ToAsync())
                   .BindAsync(dto => dto.Vigente
                                         ? dto.ToMlResultValidAsync()
                                         : "La tarifa no está vigente".ToMlResultFailAsync<TarifaDto>());
}
```

### Ejemplo 2: `ToFuncTask` para reutilizar métodos síncronos

```csharp
public class AuditoriaService
{
    // Métodos síncronos ya existentes, usados en muchos sitios
    private void RegistrarExito(Pedido p)          => _log.LogInformation("Pedido {Id} OK", p.Id);
    private void RegistrarFallo(MlErrorsDetails e) => _log.LogWarning("Fallo: {D}", e.ToErrorsDescription());

    public async Task<MlResult<PedidoDto>> ProcesarAsync(int id)
    {
        Action<Pedido>          onOk = RegistrarExito;
        Action<MlErrorsDetails> onKo = RegistrarFallo;

        return await ObtenerPedidoAsync(id)
                         .ExecSelfAsync(onOk.ToFuncTask(), onKo.ToFuncTask())
                         .MapAsync(p => p.ToDto().ToAsync());
    }
}
```

### Ejemplo 3: `AppendExDetails` para acumular fallos en cascada

```csharp
public class NotificadorResiliente
{
    public MlResult<Pedido> NotificarConReintento(Pedido pedido)
    {
        var detalles = new Dictionary<string, object> { ["PedidoId"] = pedido.Id };
        var errores  = new List<MlError>();

        foreach (var canal in _canales)
        {
            try
            {
                canal.Enviar(pedido);
                return pedido.ToMlResultValid();          // primer canal que funciona
            }
            catch (Exception ex)
            {
                errores.Add(new MlError($"El canal '{canal.Nombre}' falló"));
                detalles = detalles.AppendExDetails(ex);   // Ex, Ex2, Ex3…
            }
        }

        return (errores.AsEnumerable(), detalles).ToMlResultFail<Pedido>();
    }
}
```

Nótese cómo la tupla `(IEnumerable<MlError>, Dictionary<string,object>)` encaja directamente
con una de las sobrecargas de
[`ToMlResultFail`](../Transformations/Transformations.md#grupo-2-tomlresultfail--14-formas-de-fallar).

### Ejemplo 4: `With` en su terreno legítimo

```csharp
// Configuración de un objeto de infraestructura con muchos setters
var cliente = new HttpClient()
                  .With(c => c.BaseAddress = new Uri(_config.BaseUrl),
                        c => c.Timeout     = TimeSpan.FromSeconds(30),
                        c => c.DefaultRequestHeaders.Add("X-Api-Key", _config.ApiKey));
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ Esperar concurrencia de ToAsync
var t1 = ConsultaLenta1().ToAsync();
var t2 = ConsultaLenta2().ToAsync();
await Task.WhenAll(t1, t2);          // ⚠️ ¡Las consultas ya se ejecutaron en secuencia!

// ✅ Métodos realmente asíncronos
var t1 = ConsultaLenta1Async();
var t2 = ConsultaLenta2Async();
await Task.WhenAll(t1, t2);


// ❌ Usar With esperando inmutabilidad
var confirmado = pedidoOriginal.With(p => p.Estado = Confirmado);
// ⚠️ pedidoOriginal.Estado TAMBIÉN cambió

// ✅ Expresión with de C# sobre un record
var confirmado = pedidoOriginal with { Estado = Confirmado };


// ❌ Claves propias que empiecen por "Ex": rompen la numeración
var detalles = new Dictionary<string, object> { ["ExportadoPor"] = usuario };

// ✅
var detalles = new Dictionary<string, object> { ["UsuarioExportacion"] = usuario };


// ❌ Leer Details con la clave literal
var ex = (Exception)resultado.ErrorsDetails.Details["Ex"];

// ✅ Accesor oficial
resultado.ExecSelfIfFailWithException(ex => _log.LogError(ex, "…"));


// ❌ ValidateObject a pelo para validar entidades de dominio
var errores = dto.ValidateObject();

// ✅ El paquete de integración devuelve MlResult directamente
// (MoralesLarios.OOFP.Validation.Dataannotations)
```

---

## Mejores Prácticas

1. **Usa `ToAsync` solo como adaptador de firmas.** Para concurrencia real, métodos
   asíncronos de verdad o `Task.Run`.
2. **Prefiere `record` + expresión `with` de C#** a `With`. Reserva `With` para objetos
   legados o de infraestructura.
3. **Recuerda que `With` muta**: no lo uses sobre objetos compartidos ni cacheados.
4. **`ToFuncTask` para reutilizar métodos síncronos** en operadores `*Async`, en lugar de
   duplicarlos con `async`.
5. **Nunca uses claves de `Details` que empiecen por `Ex`** ni la clave `Value`: colisionan
   con `EX_DESC_KEY` y `VALUE_KEY`.
6. **Accede a los detalles con los accesores oficiales** (`GetDetailException`,
   `GetDetailValue<T>`), no con claves literales.
7. **Pasa siempre un mensaje de dominio a los `Try*`**: los mensajes por defecto están en
   inglés y exponen `ex.Message`.
8. **Para DataAnnotations, usa el paquete de integración**, no `ValidateObject` directamente.
9. **Cuidado con el coste de `ValidateObject`**: valida recursivamente todo el grafo.
10. **`ToNullable` y `VoidToAsync` son marginales**: si te apoyas mucho en ellos, revisa si
    hay un operador de la librería que exprese mejor tu intención.

---

## Resumen

- **`ToAsync<T>`** (`ParallelExtensions`) es la extensión más usada: `Task.FromResult`.
  ⚠️ **No aporta concurrencia**, solo adapta firmas. El nombre del archivo engaña.
- **`With` / `WithAsync`** aplican acciones en cadena y devuelven el objeto.
  ⚠️ **MUTAN el original** y exigen `where T : class`. No es la expresión `with` de C#.
- **`ToFuncTask`** (5 sobrecargas) convierte `Func`/`Action` síncronos en asíncronos; dos de
  ellas existen específicamente para `MlErrorsDetails` por límites de inferencia.
- **`AppendExDetails`** añade excepciones al diccionario generando claves `Ex`, `Ex2`, `Ex3`…
  Devuelve un diccionario **nuevo**. ⚠️ No uses claves propias que empiecen por `Ex`.
- **`ValidateObject`** ejecuta DataAnnotations recursivamente y devuelve
  `IEnumerable<ValidationResult>`; **no** devuelve `MlResult`. Prefiere el paquete
  `Validation.Dataannotations`.
- **`ToNullable`** y **`VoidToAsync`** son azúcar sintáctico de uso marginal.
- **`Constants`**: `EX_DESC_KEY = "Ex"`, `VALUE_KEY = "Value"`, más los mensajes por defecto
  en inglés que exponen `ex.Message`. **No colisiones con esas claves.**

---

## Ver también

- [`Transformations`](../Transformations/Transformations.md) — `ToMlResultValid`, `ToMlResultFail`, los `Try*`
- [`MlResultErrors`](../Types/MlResultErrors.md) — `AppendExErrorDetail`, `GetDetailException`, `GetDetailValue`
- [`MlResult`](../Types/MlResult.md) — el tipo central
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — guardas de entrada al carril
- [`ExecSelf`](../ExecSelf/1_ExecSelf.md) — efectos laterales, donde `ToFuncTask` es más útil
- [`MapIfFailWithValue`](../Map/5_MapIfFailWithValue.md) — uso de `VALUE_KEY`
- [`MapIfFailWithException`](../Map/6_MapIfFailWithException.md) — uso de `EX_DESC_KEY`
- [`Bucles y proyecciones`](../Bucle/Bucles.md) — recorrido de colecciones