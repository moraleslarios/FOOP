# `EnsureFp` — Tipos concretos: `Guid`, enumerados, fechas, `Uri`, email y rutas

> Archivo fuente: `Helpers/EnsureFp.Types.cs`.

## Índice

- [Qué aporta este bloque](#qué-aporta-este-bloque)
- [1. `Guid`: `NotEmptyGuid` y `NotNullNotEmptyGuid`](#1-guid-notemptyguid-y-notnullnotemptyguid)
- [2. Enumerados: `IsDefined`](#2-enumerados-isdefined)
- [3. Fechas: `InFuture` e `InPast`](#3-fechas-infuture-e-inpast)
- [4. `NotDefault`: el valor por defecto como valor inválido](#4-notdefault-el-valor-por-defecto-como-valor-inválido)
- [5. `Uri`: `IsAbsoluteUri` e `IsValidUri`](#5-uri-isabsoluteuri-e-isvaliduri)
- [6. `IsValidEmail`](#6-isvalidemail)
- [7. Sistema de ficheros: `FileExists` y `DirectoryExists`](#7-sistema-de-ficheros-fileexists-y-directoryexists)
- [8. Ejemplos completos](#8-ejemplos-completos)
- [9. Mejores prácticas](#9-mejores-prácticas)
- [Ver también](#ver-también)

---

## Qué aporta este bloque

Hay un grupo de tipos de .NET cuyas validaciones se repiten en todos los proyectos y que siempre se
escriben mal de la misma forma:

| Comprobación habitual | Escrita a mano | Problema |
|---|---|---|
| `Guid` no vacío | `if (id == Guid.Empty) throw …` | `Guid.Empty` es un valor válido de tipo, pero nunca un identificador real |
| Enumerado válido | `if (!Enum.IsDefined(typeof(E), v)) throw …` | verboso, con `typeof` y *boxing* |
| Fecha futura | `if (d <= DateTime.Now) throw …` | mezcla `Now` y `UtcNow` según el `Kind` sin darse cuenta |
| URL válida | `new Uri(s)` | lanza excepción; hay que envolver en `try/catch` |
| Email válido | expresión regular casera | ninguna regex cubre bien el RFC |
| Fichero existente | `if (!File.Exists(p)) throw …` | rompe la cadena funcional |

Este bloque las expresa como reglas `MlResult<T>` con las tres variantes habituales (`string`,
`MlErrorsDetails` y `*Arg`).

---

## 1. `Guid`: `NotEmptyGuid` y `NotNullNotEmptyGuid`

```csharp
public static MlResult<Guid> NotEmptyGuid(Guid value, string errorMessage);
public static MlResult<Guid> NotEmptyGuid(Guid value, MlErrorsDetails errorsDetails);
public static MlResult<Guid> NotEmptyGuidArg(Guid value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<Guid> NotNullNotEmptyGuid(Guid? value, string errorMessage);
public static MlResult<Guid> NotNullNotEmptyGuid(Guid? value, MlErrorsDetails errorsDetails);
public static MlResult<Guid> NotNullNotEmptyGuidArg(Guid? value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

`Guid.Empty` (`00000000-0000-0000-0000-000000000000`) es el fallo más silencioso de una API: pasa
todas las validaciones de tipo y llega a la base de datos como una clave que no existe.

```csharp
var r1 = NotEmptyGuid(clienteId, "El identificador de cliente no es válido.");
var r2 = NotEmptyGuidArg(clienteId);   // "'clienteId' no puede ser Guid.Empty."
```

**`NotNullNotEmptyGuid` cubre las dos comprobaciones a la vez** para un `Guid?` y devuelve el valor
ya **desenvuelto** como `MlResult<Guid>`:

```csharp
public MlResult<Cliente> Obtener(Guid? id) =>
    NotNullNotEmptyGuidArg(id)          // MlResult<Guid>: ni null ni Guid.Empty
        .Bind(g => repositorio.Get(g));
```

Sin esa sobrecarga habría que escribir `NotNullValue(id).Bind(NotEmptyGuidArg)`, que funciona pero
pierde el nombre del parámetro original.

---

## 2. Enumerados: `IsDefined`

```csharp
public static MlResult<TEnum> IsDefined<TEnum>(TEnum value, string errorMessage)           where TEnum : struct, Enum;
public static MlResult<TEnum> IsDefined<TEnum>(TEnum value, MlErrorsDetails errorsDetails) where TEnum : struct, Enum;
public static MlResult<TEnum> IsDefinedArg<TEnum>(TEnum value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null) where TEnum : struct, Enum;
```

En C# un enumerado **no** garantiza que su valor esté declarado: `(EstadoPedido)99` compila y se
asigna sin error. Es un problema real cuando el valor llega de una deserialización JSON o de una
columna de base de datos.

```csharp
public enum EstadoPedido { Borrador = 0, Confirmado = 1, Enviado = 2, Entregado = 3 }

var recibido = (EstadoPedido)99;

var r1 = IsDefined(recibido, "El estado del pedido no es válido.");     // ❌ Fail
var r2 = IsDefinedArg(recibido);
// "'recibido' no es un valor definido de EstadoPedido (actual: 99)."
```

El mensaje automático lo genera `EnsureFpMessages.IsDefinedEnum(paramName, Type, object?)`, que
incluye el **nombre del tipo enumerado** y el valor recibido: el diagnóstico no deja dudas.

> ⚠️ Con enumerados marcados con `[Flags]`, `Enum.IsDefined` rechaza las combinaciones (por ejemplo
> `Leer | Escribir`) porque no son un miembro declarado. Para *flags* valida con una máscara mediante
> [`That`](./1_EnsureFpCore.md):
> ```csharp
> var todos = Permisos.Leer | Permisos.Escribir | Permisos.Borrar;
> var r = That(permisos, p => (p & ~todos) == 0, "Combinación de permisos no válida.");
> ```

---

## 3. Fechas: `InFuture` e `InPast`

Hay sobrecargas para los tres tipos de fecha modernos de .NET:

| Tipo | Referencia temporal usada |
|---|---|
| `DateTime` | `DateTime.UtcNow` si `value.Kind == DateTimeKind.Utc`; en otro caso `DateTime.Now` |
| `DateTimeOffset` | siempre `DateTimeOffset.UtcNow` |
| `DateOnly` | `DateOnly.FromDateTime(DateTime.Today)` |

```csharp
public static MlResult<DateTime>       InFuture(DateTime value, string errorMessage);
public static MlResult<DateTimeOffset> InFuture(DateTimeOffset value, string errorMessage);
public static MlResult<DateOnly>       InFuture(DateOnly value, string errorMessage);
// … con MlErrorsDetails y con las variantes InFutureArg / InPastArg para los tres tipos.
```

La elección de referencia para `DateTime` la resuelve el helper privado `NowFor(DateTime)`. Es la
parte más importante del bloque: **respeta el `Kind` del valor recibido** en lugar de imponer una
zona horaria, evitando el error clásico de comparar una fecha UTC contra la hora local (o al revés)
y obtener desviaciones de horas.

```csharp
var r1 = InFuture(fechaEntrega, "La fecha de entrega debe ser futura.");
var r2 = InPast(fechaNacimiento, "La fecha de nacimiento debe ser pasada.");
var r3 = InFutureArg(fechaCaducidad);   // "'fechaCaducidad' debe ser una fecha futura (actual: …)."
```

`DateOnly` compara **solo el día**: `InFuture(DateOnly.FromDateTime(DateTime.Today))` falla, porque
hoy no es futuro. Si la semántica que quieres es «hoy o posterior», usa
[`GreaterOrEqual`](./4_EnsureFpNumbers.md#1-comparaciones-greaterthan-lessthan-y-variantes):

```csharp
var r = GreaterOrEqual(fecha, DateOnly.FromDateTime(DateTime.Today),
                       "La fecha no puede ser anterior a hoy.");
```

> Para intervalos entre dos fechas usa
> [`InRange`](./4_EnsureFpNumbers.md#2-rangos-inrange-y-outofrange): `DateTime`, `DateTimeOffset` y
> `DateOnly` implementan `IComparable<T>`.

---

## 4. `NotDefault`: el valor por defecto como valor inválido

```csharp
public static MlResult<T> NotDefault<T>(T value, string errorMessage);
public static MlResult<T> NotDefault<T>(T value, MlErrorsDetails errorsDetails);
public static MlResult<T> NotDefaultArg<T>(T value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Generaliza `NotEmptyGuid` a cualquier tipo usando `EqualityComparer<T>.Default`:

| `T` | `default(T)` que se rechaza |
|---|---|
| `int`, `long`, `decimal` | `0` |
| `Guid` | `Guid.Empty` |
| `DateTime` | `01/01/0001 00:00:00` |
| `bool` | `false` |
| tipos referencia | `null` |
| `struct` propio | la instancia con todos sus campos por defecto |

```csharp
var r1 = NotDefault(fechaAlta, "La fecha de alta es obligatoria.");   // rechaza DateTime.MinValue
var r2 = NotDefault(clienteId, "El identificador es obligatorio.");   // rechaza Guid.Empty
var r3 = NotDefaultArg(codigo);                                       // mensaje automático
```

Es especialmente útil con estructuras deserializadas, donde un campo ausente en el JSON no produce
`null` sino el valor por defecto del tipo.

> ⚠️ `NotDefault(0)` falla, y eso es lo que quieres para un identificador; pero **no** lo uses para
> validar importes o contadores donde el cero es legítimo. Ahí la regla correcta es
> [`NotNegative`](./4_EnsureFpNumbers.md#3-signo-y-cero-positive-negative-notnegative-notzero).

---

## 5. `Uri`: `IsAbsoluteUri` e `IsValidUri`

```csharp
public static MlResult<Uri> IsAbsoluteUri(Uri value, string errorMessage);
public static MlResult<Uri> IsAbsoluteUri(Uri value, MlErrorsDetails errorsDetails);
public static MlResult<Uri> IsAbsoluteUriArg(Uri value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<Uri> IsValidUri(string value, string errorMessage,           UriKind uriKind = UriKind.Absolute);
public static MlResult<Uri> IsValidUri(string value, MlErrorsDetails errorsDetails, UriKind uriKind = UriKind.Absolute);
public static MlResult<Uri> IsValidUriArg(string value, UriKind uriKind = UriKind.Absolute,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

Dos escenarios distintos:

- **`IsAbsoluteUri`** recibe un `Uri` ya construido y comprueba que sea absoluto (con esquema y
  autoridad), no relativo.
- **`IsValidUri`** recibe una **cadena** y **devuelve el `Uri` construido**: valida y convierte en un
  solo paso, sin `try/catch` y sin excepciones.

```csharp
// De cadena a Uri, con validación:
MlResult<Uri> r1 = IsValidUri(configuracion.EndpointUrl,
                              "La URL del endpoint no es válida.");

// Aceptando también rutas relativas:
MlResult<Uri> r2 = IsValidUri(ruta, "La ruta no es válida.", UriKind.RelativeOrAbsolute);

// Mensaje automático:
MlResult<Uri> r3 = IsValidUriArg(configuracion.EndpointUrl);
```

Que `IsValidUri` devuelva `MlResult<Uri>` en lugar de `MlResult<string>` es deliberado: encaja
directamente en la cadena de llamada.

```csharp
public MlResult<HttpResponseMessage> Llamar(string url) =>
    IsValidUriArg(url)
        .BindAsync(uri => cliente.GetAsync(uri).ToMlResultValidAsync())
        .Result;
```

> `Uri.TryCreate` con `UriKind.Absolute` acepta esquemas poco habituales (`file:`, `ftp:`,
> `javascript:`). Si necesitas restringir el esquema, añade una regla:
> ```csharp
> var r = IsValidUriArg(url)
>             .Bind(u => IsOneOf(u.Scheme, new[] { "http", "https" },
>                                "Solo se admiten URLs http o https.",
>                                StringComparer.OrdinalIgnoreCase)
>                            .Map(_ => u));
> ```

---

## 6. `IsValidEmail`

```csharp
public static MlResult<string> IsValidEmail(string value, string errorMessage);
public static MlResult<string> IsValidEmail(string value, MlErrorsDetails errorsDetails);
public static MlResult<string> IsValidEmailArg(string value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

La validación **no usa expresiones regulares**. El helper privado `IsEmail(string)` aplica tres
comprobaciones:

1. `MailAddress.TryCreate(value, out var direccion)` — el analizador oficial de .NET, el mismo que
   usa `System.Net.Mail`.
2. `direccion.Address == value` — evita que se acepten formas «amigables» como
   `Nombre <a@b.com>` cuando lo que se esperaba era una dirección pura.
3. `direccion.Host.Contains('.')` — descarta dominios sin punto (`usuario@localhost`), que son
   técnicamente válidos pero casi nunca lo que un formulario espera.

```csharp
IsValidEmail("ana@example.com",       "…");   // ✅ Valid
IsValidEmail("ana@localhost",         "…");   // ❌ Fail (host sin punto)
IsValidEmail("Ana <ana@example.com>", "…");   // ❌ Fail (no es una dirección pura)
IsValidEmail("ana@@example.com",      "…");   // ❌ Fail
IsValidEmail(null!,                   "…");   // ❌ Fail (nunca excepción)
```

Este enfoque es más fiable y más rápido que cualquier regex casera, y no tiene riesgo de
*catastrophic backtracking*.

---

## 7. Sistema de ficheros: `FileExists` y `DirectoryExists`

```csharp
public static MlResult<string> FileExists(string value, string errorMessage);
public static MlResult<string> FileExists(string value, MlErrorsDetails errorsDetails);
public static MlResult<string> FileExistsArg(string value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);

public static MlResult<string> DirectoryExists(string value, string errorMessage);
public static MlResult<string> DirectoryExists(string value, MlErrorsDetails errorsDetails);
public static MlResult<string> DirectoryExistsArg(string value,
    [CallerArgumentExpression(nameof(value))] string? paramName = null);
```

```csharp
public MlResult<string> LeerConfiguracion(string ruta) =>
    FileExistsArg(ruta)
        .TryMap(File.ReadAllText, ex => $"No se pudo leer '{ruta}': {ex.Message}");
```

Dos advertencias importantes:

1. **Son comprobaciones con efecto de entrada/salida.** Consultan el disco, por lo que no son puras
   y no deben usarse en bucles calientes.
2. **Sufren la condición de carrera clásica**: entre el `Exists` y la lectura, el fichero puede
   desaparecer. `FileExists` mejora el *mensaje de error*, no elimina la necesidad de proteger la
   operación real. Combínalas siempre con `TryThat`/`TryMap` o con los tipos de
   `MoralesLarios.OOFP.ValueObjects.IO`.

> Si trabajas mucho con rutas, considera los objetos de valor de
> `MoralesLarios.OOFP.ValueObjects.IO` (`FilePath`, `DirectoryPath`): validan en la construcción y
> hacen imposible propagar una ruta inválida por el sistema.

---

## 8. Ejemplos completos

### 8.1. Alta de un recurso desde una petición HTTP

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public record AltaSuscripcionDto(Guid? ClienteId, string Email, string UrlWebhook,
                                 EstadoSuscripcion Estado, DateTime FechaInicio);

public MlResult<Suscripcion> Crear(AltaSuscripcionDto dto) =>
    All(dto,
        d => NotNullNotEmptyGuid(d.ClienteId, "El cliente es obligatorio.").Map(_ => d),
        d => IsValidEmail(d.Email, "El correo electrónico no es válido.").Map(_ => d),
        d => IsValidUri(d.UrlWebhook, "La URL del webhook no es válida.").Map(_ => d),
        d => IsDefined(d.Estado, "El estado de la suscripción no es válido.").Map(_ => d),
        d => InFuture(d.FechaInicio, "La fecha de inicio debe ser futura.").Map(_ => d))
    .Map(d => new Suscripcion(d.ClienteId!.Value, d.Email, new Uri(d.UrlWebhook),
                              d.Estado, d.FechaInicio));
```

Un fallo devuelve **todos** los problemas a la vez (gracias a
[`All`](./2_EnsureFpAggregation.md#1-all-ejecuta-todas-las-reglas)):

```
"El cliente es obligatorio."
"El correo electrónico no es válido."
"La fecha de inicio debe ser futura."
```

### 8.2. Arranque de un proceso por lotes

```csharp
public MlResult<Configuracion> ValidarEntorno(Configuracion cfg) =>
    All(cfg,
        c => DirectoryExists(c.CarpetaEntrada,  "La carpeta de entrada no existe.").Map(_ => c),
        c => DirectoryExists(c.CarpetaSalida,   "La carpeta de salida no existe.").Map(_ => c),
        c => FileExists(c.PlantillaInforme,     "La plantilla de informe no existe.").Map(_ => c),
        c => IsValidUri(c.EndpointApi,          "El endpoint de la API no es válido.").Map(_ => c),
        c => NotDefault(c.EjecucionId,          "El identificador de ejecución es obligatorio.").Map(_ => c));
```

### 8.3. Validación de un evento de dominio deserializado

```csharp
public MlResult<PedidoConfirmado> Validar(PedidoConfirmado evento) =>
    NotDefaultArg(evento.PedidoId)                               // Guid.Empty tras deserializar
        .Bind(_ => IsDefinedArg(evento.Origen))                  // enum fuera de rango en el JSON
        .Bind(_ => InPastArg(evento.OcurridoEn))                 // no puede ocurrir en el futuro
        .Map(_ => evento);
```

---

## 9. Mejores prácticas

1. **Usa `NotNullNotEmptyGuid` para los `Guid?` que llegan de fuera.** Una sola regla cubre `null` y
   `Guid.Empty` y devuelve el valor desenvuelto.
2. **Valida `IsDefined` en todos los enumerados que provengan de JSON, formularios o base de datos.**
   El sistema de tipos no lo hace por ti.
3. **No uses `IsDefined` con `[Flags]`**; valida con máscara mediante `That`.
4. **Respeta el `Kind` de tus `DateTime`.** Si trabajas en UTC, construye siempre con
   `DateTime.UtcNow` / `DateTimeKind.Utc` para que `InFuture`/`InPast` comparen contra la referencia
   correcta. Mejor aún: usa `DateTimeOffset`.
5. **`InFuture` con `DateOnly` excluye hoy.** Si «hoy vale», usa `GreaterOrEqual`.
6. **Prefiere `IsValidUri` a `new Uri(...)`**: valida y construye sin excepciones, y devuelve
   directamente el `Uri`.
7. **Restringe el esquema de las URLs** con `IsOneOf` si aceptas entradas de usuario.
8. **No escribas expresiones regulares de email.** `IsValidEmail` ya usa el analizador de .NET.
9. **`FileExists`/`DirectoryExists` mejoran el mensaje, no eliminan la carrera.** Envuelve la lectura
   real con `TryThat`/`TryMap`.
10. **Reserva `NotDefault` para identificadores y fechas obligatorias**, no para importes donde el
    cero es válido.

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [2. Agregación de reglas](./2_EnsureFpAggregation.md)
- [3. Cadenas de texto](./3_EnsureFpStrings.md)
- [4. Números y rangos](./4_EnsureFpNumbers.md)
- [7. Tipos `Nullable<T>`](./7_EnsureFpNullables.md)
- [9. Mensajes y claves de detalle](./9_EnsureFpMessages.md)
- [`NullToFailed`](../Several/2_NullToFailed.md) — alternativa por extensión para nulos
