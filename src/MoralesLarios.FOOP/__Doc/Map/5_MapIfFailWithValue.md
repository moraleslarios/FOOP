# MapIfFailWithValue — Recuperarse usando el valor que había antes de fallar

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [El requisito previo: guardar el valor en los detalles](#el-requisito-previo-guardar-el-valor-en-los-detalles)
4. [Las dos formas de `MapIfFailWithValue`](#las-dos-formas-de-mapiffailwithvalue)
5. [Firmas reales e implementación](#firmas-reales-e-implementación)
6. [Qué pasa si el valor no está](#qué-pasa-si-el-valor-no-está)
7. [`MapIfFailWithValue` frente a `MapIfFail` y `BindIfFailWithValue`](#mapiffailwithvalue-frente-a-mapiffail-y-bindiffailwithvalue)
8. [Variantes asíncronas](#variantes-asíncronas)
9. [`TryMapIfFailWithValue` — cuando la recuperación puede lanzar](#trymapiffailwithvalue--cuando-la-recuperación-puede-lanzar)
10. [Ejemplos Prácticos](#ejemplos-prácticos)
11. [Mejores Prácticas](#mejores-prácticas)
12. [Apéndice: `MapDefault` (solo para depurar)](#apéndice-mapdefault-solo-para-depurar)
13. [Resumen](#resumen)
14. [Ver también](#ver-también)

---

## Introducción

`MapIfFail` te permite volver al carril bueno con un valor de reemplazo, pero solo recibe
los errores. Muchas veces eso no es suficiente: para construir un buen valor de reemplazo
necesitas **el dato que estabas procesando cuando la cosa se rompió**.

Ahí entra `MapIfFailWithValue`. Es la variante de `MapIfFail` cuyo delegado de recuperación
no recibe `MlErrorsDetails`, sino **el valor original rescatado de los detalles del error**.

```csharp
// ❌ MapIfFail: solo tengo los errores, no sé qué producto estaba enriqueciendo
var resultado = EnriquecerPrecio(producto)
                    .MapIfFail(errores => Producto.Vacio);        // pierdo el producto

// ✅ MapIfFailWithValue: recupero el producto original y lo degrado con criterio
var resultado = EnriquecerPrecio(producto)
                    .MapIfFailWithValue(p => p with { PrecioFiable = false });
```

> ⚠️ **Sobre `MlErrorsDetails`**
> `MlErrorsDetails` solo expone dos propiedades: `Errors` (una colección de `MlError`) y
> `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`,
> `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer errores usa
> `ToErrorsMessages()`, `ToErrorsDescription()` o `Errors.First().Message`; para leer los
> detalles usa `GetDetailValue<T>()`, `GetDetailException()` o `ToDetailsDescription()`.

---

## El problema que resuelve

En una tubería, cuando un paso falla el valor de entrada **desaparece**: `MlResult<T>` en
estado fallido no lleva un `T`, lleva errores. Ese es precisamente el punto del patrón.

Pero hay casos legítimos en los que quieres el dato de vuelta:

| Caso | Por qué necesitas el valor original |
|------|-------------------------------------|
| Degradar en lugar de abortar | Devolver el objeto tal cual estaba, marcado como incompleto. |
| Construir un mensaje útil | «No se pudo validar el pedido *12345* del cliente *ACME*». |
| Reintentar por otra vía | Volver a intentar la operación con un canal alternativo. |
| Registrar el dato que falló | Auditar exactamente qué entrada provocó el problema. |

La librería resuelve esto guardando el valor **dentro del diccionario `Details`** del error,
bajo la clave `"Value"` (la constante `VALUE_KEY`). `MapIfFailWithValue` lo lee de ahí.

---

## El requisito previo: guardar el valor en los detalles

**Esto es lo más importante de este documento:** `MapIfFailWithValue` no guarda nada, solo
*lee*. Si nadie metió el valor en los detalles antes, no habrá nada que recuperar.

Tienes tres formas de meterlo:

```csharp
// 1) Explícitamente, sobre un resultado que ya falló
var conValor = ValidarPedido(pedido).AddValueDetailIfFail(pedido);

// 2) Con AddValueIfFail (útil encadenado, guarda un valor de otro tipo)
var conValor = ValidarPedido(pedido).AddValueIfFail(pedido);

// 3) Desde el propio error, al crearlo
return MlErrorsDetails.FromErrorMessageWithValue("El pedido no es válido", pedido)
                      .ToMlResultFail<PedidoValidado>();
```

Recuerda cómo se implementa `AddValueDetailIfFail` (verificado en `MlResultActions.cs`):

```csharp
public static MlResult<T> AddMlErrorDetailIfFail<T>(this MlResult<T> source, string errorKey, object errorValue)
    => source.Match(fail : errorsDetails => errorsDetails.AddDetail(errorKey, errorValue),
                    valid: _ => source);

public static MlResult<T> AddValueDetailIfFail<T>(this MlResult<T> source, object errorValue)
    => source.AddMlErrorDetailIfFail(VALUE_KEY, errorValue);
```

> ⚠️ `AddDetail<T>` hace `source.Details.Add(key, value)`, es decir **muta el diccionario** y
> **lanza `ArgumentException` si la clave ya existe**. No llames dos veces a
> `AddValueDetailIfFail` sobre el mismo resultado fallido.

El patrón completo, entonces, es siempre **de dos pasos**:

```csharp
var resultado = ValidarPedido(pedido)              // puede fallar
                    .AddValueDetailIfFail(pedido)  // 1) guardo el pedido en Details["Value"]
                    .MapIfFailWithValue(p => …);   // 2) lo recupero para construir el reemplazo
```

---

## Las dos formas de `MapIfFailWithValue`

| | **Forma A** — recuperación en el mismo tipo | **Forma B** — proyección de las dos ramas |
|---|---|---|
| Genéricos | `<T>` | `<T, TValue, TReturn>` |
| Delegados | `Func<T, T> func` | `Func<T, TReturn> funcValid` + `Func<TValue, TReturn> funcFail` |
| Rama válida | se devuelve el valor tal cual | se transforma con `funcValid` |
| Rama fallida | se lee `Details["Value"]` como `T` y se transforma con `func` | se lee `Details["Value"]` como `TValue` y se transforma con `funcFail` |
| Tipo de salida | `MlResult<T>` | `MlResult<TReturn>` |
| Uso típico | degradar el objeto sin cambiar de forma | traducir a un modelo de vista/DTO cubriendo éxito y fracaso |

La diferencia clave frente a `MapIfFail`: aquí el valor guardado puede ser **de otro tipo**
(`TValue`) que el del resultado (`T`). Eso te permite guardar el DTO de entrada y recuperarlo
aunque la tubería ya estuviera trabajando con una entidad distinta.

---

## Firmas reales e implementación

### Forma A — `MapIfFailWithValue<T>`

```csharp
/// <summary>
/// Execute the function if the source is fail, otherwise return the source.
/// source parameter has a prevous 'Value' execution
/// </summary>
public static MlResult<T> MapIfFailWithValue<T>(this MlResult<T> source,
                                                      Func<T, T> func)
    => source.Match
                    (
                        fail : errorsDetails => errorsDetails.GetDetailValue<T>().Map(func),
                        valid: value         => value
                    );
```

Léelo despacio, porque toda la semántica está en una línea:

1. Si `source` es válido → se devuelve el valor **sin tocarlo**. `func` no se ejecuta.
2. Si `source` es fallido → se pide `GetDetailValue<T>()`:
   - si el valor está y es del tipo `T`, se le aplica `func` y el resultado es **válido**;
   - si no está, el resultado sigue siendo **fallido** (ver la sección siguiente).

Como `func` devuelve un `T` desnudo (no un `MlResult<T>`), la recuperación es **infalible**:
cuando el valor se recupera, la salida es siempre válida.

### Forma B — `MapIfFailWithValue<T, TValue, TReturn>`

```csharp
public static MlResult<TReturn> MapIfFailWithValue<T, TValue, TReturn>(this MlResult<T>            source,
                                                                            Func<T     , TReturn> funcValid,
                                                                            Func<TValue, TReturn> funcFail)
    => source.Match
                    (
                        fail : errorsDetails => errorsDetails.GetDetailValue<TValue>().Map(value => funcFail(value)),
                        valid: value         => funcValid(value)
                    );
```

Aquí las dos ramas convergen en un mismo `TReturn`. Es el equivalente de `Match`, pero con
la rama de fallo alimentada por el valor guardado en lugar de por los errores.

### Tabla de comportamiento

| Estado de `source` | ¿Hay `Details["Value"]` del tipo esperado? | Forma A | Forma B |
|---|---|---|---|
| Válido | irrelevante | devuelve el valor; `func` no se ejecuta | ejecuta `funcValid` |
| Fallido | sí | ejecuta `func` → resultado **válido** | ejecuta `funcFail` → resultado **válido** |
| Fallido | no | resultado **fallido** | resultado **fallido** |

---

## Qué pasa si el valor no está

Es el error más habitual con esta familia: se llama a `MapIfFailWithValue` sin haber guardado
antes el valor, y el resultado sigue fallando… pero **con otro error**.

```csharp
// ❌ Falta el paso de guardado
var r = ValidarPedido(pedido)                          // falla: "Falta el NIF"
            .MapIfFailWithValue(p => p with { Ok = false });
// r sigue fallido. El delegado NUNCA se ejecutó.
```

`GetDetailValue<T>()` devuelve un `MlResult<T>` fallido cuando la clave `"Value"` no existe
en `Details` o cuando el objeto almacenado no es del tipo pedido. Y como la implementación
hace `errorsDetails.GetDetailValue<T>().Map(func)`, ese fallo es el que se propaga:
**el error original queda sustituido por el error de «no hay valor»**.

Estrategias:

```csharp
// ✅ 1) Guarda siempre el valor justo después del paso que puede fallar
var r = ValidarPedido(pedido)
            .AddValueDetailIfFail(pedido)
            .MapIfFailWithValue(p => p with { Ok = false });

// ✅ 2) Si el valor es opcional, conserva el error original fusionándolo
var r = ValidarPedido(pedido)
            .BindIfFail(errores => errores.GetDetailValue<Pedido>()
                                          .Map(p => p with { Ok = false })
                                          .MergeErrorsDetailsIfFail(errores));

// ✅ 3) Si no necesitas el valor, usa MapIfFail y punto
var r = ValidarPedido(pedido).MapIfFail(_ => Pedido.Vacio);
```

> 📌 Diagnóstico rápido: si sospechas que el valor no está, inspecciona
> `errores.ToDetailsDescription()` o `errores.HasValueDetails()`, que te dice si existe la
> clave `"Value"`.

---

## `MapIfFailWithValue` frente a `MapIfFail` y `BindIfFailWithValue`

| Método | Qué recibe el delegado de fallo | Qué devuelve el delegado | ¿Puede volver a fallar la recuperación? |
|---|---|---|---|
| `MapIfFail` | `MlErrorsDetails` | `T` desnudo | No (salvo que lance) |
| `MapIfFailWithValue` | el valor guardado (`T` o `TValue`) | `T`/`TReturn` desnudo | Solo si el valor no estaba |
| `BindIfFailWithValue` | el valor guardado | `MlResult<…>` | Sí, la recuperación decide |

Regla práctica:

- **No necesito el dato** → `MapIfFail`.
- **Necesito el dato y la recuperación no puede fallar** → `MapIfFailWithValue`.
- **Necesito el dato y la recuperación sí puede fallar** → `BindIfFailWithValue`.

---

## Variantes asíncronas

### Forma A

| Sobrecarga | Origen | Delegado |
|---|---|---|
| `MapIfFailWithValueAsync<T>` | `MlResult<T>` | `Func<T, Task<T>>` |
| `MapIfFailWithValueAsync<T>` | `Task<MlResult<T>>` | `Func<T, Task<T>>` |
| `MapIfFailWithValueAsync<T>` | `Task<MlResult<T>>` | `Func<T, T>` |

### Forma B

| Sobrecarga | Origen | Delegados |
|---|---|---|
| `MapIfFailWithValueAsync<T, TValue, TReturn>` | `MlResult<T>` | ambos asíncronos |
| `MapIfFailWithValueAsync<T, TValue, TReturn>` | `Task<MlResult<T>>` | ambos asíncronos |
| `MapIfFailWithValueAsync<T, TValue, TReturn>` | `MlResult<T>` | mezclas síncrono/asíncrono |

En total, la región publica **2 sobrecargas síncronas** de `MapIfFailWithValue` y **7
asíncronas**, más las de `TryMapIfFailWithValue` (4 síncronas y más de veinte asíncronas).

Si te falta una combinación concreta de síncrono/asíncrono, homogeneiza con los helpers de la
librería:

```csharp
// convierte un delegado síncrono en asíncrono
Func<Pedido, Task<Pedido>> degradarAsync = Degradar.ToFuncTask();

// convierte un valor en Task<valor>
await pedido.ToAsync();
```

---

## `TryMapIfFailWithValue` — cuando la recuperación puede lanzar

Si el delegado de recuperación puede lanzar (parseos, accesos a propiedades de terceros,
conversiones…), usa la variante `Try*`: captura la excepción y la convierte en un fallo con
la excepción guardada en `Details["Ex"]`.

```csharp
public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T>             source,
                                                        Func<T, T>              funcValue,
                                                        Func<Exception, string> errorMessageBuilder)
    => source.Match
                    (
                        fail : errorsDetails => errorsDetails.GetDetailValue<T>()
                                                             .Bind(x => funcValue.TryToMlResult(x, errorMessageBuilder)),
                        valid: value         => value
                    );

public static MlResult<T> TryMapIfFailWithValue<T>(this MlResult<T> source,
                                                        Func<T, T>  funcValue,
                                                        string      errorMessage = null!)
    => source.TryMapIfFailWithValue(funcValue, _ => errorMessage!);
```

Y la Forma B protege **las dos ramas**:

```csharp
public static MlResult<TReturn> TryMapIfFailWithValue<T, TValue, TReturn>(this MlResult<T>             source,
                                                                               Func<T     , TReturn>   funcValid,
                                                                               Func<TValue, TReturn>   funcFail,
                                                                               Func<Exception, string> errorMessageBuilder)
    => source.Match
                    (
                        fail : errorsDetails => errorsDetails.GetDetailValue<TValue>()
                                                             .Bind(value => funcFail.TryToMlResult(value, errorMessageBuilder)),
                        valid: value         => funcValid.TryToMlResult(source.Value, errorMessageBuilder)
                    );
```

> ⚠️ Ten en cuenta dos matices reales del código:
> 1. Si el delegado lanza, **los errores originales se pierden**: el resultado final describe
>    la excepción de la recuperación, no el fallo que la provocó. Si necesitas conservar el
>    contexto, registra antes con `ExecSelfIfFail`.
> 2. En la Forma B síncrona, la rama válida invoca `funcValid.TryToMlResult(source.Value, …)`
>    en lugar de usar la variable `value`. El comportamiento es el mismo (en esa rama
>    `source.Value` **es** `value`), pero no te sorprendas al leer el fuente.

---

## Ejemplos Prácticos

### Ejemplo 1: Degradar un producto cuando falla el enriquecimiento de precio

El catálogo debe mostrarse siempre. Si el servicio de precios no responde, mostramos el
producto con el precio marcado como no fiable en lugar de romper la página.

```csharp
public MlResult<ProductoCatalogo> ObtenerProductoParaCatalogo(int productoId)
    => _repo.Obtener(productoId)                                   // MlResult<ProductoCatalogo>
            .Bind(EnriquecerConPrecioActual)                       // puede fallar
            .AddValueDetailIfFail(_repo.Obtener(productoId).Match(  // guardamos el producto base
                                        valid: p  => (object)p,
                                        fail :  _  => new object()))
            .MapIfFailWithValue(p => p with
                                     {
                                         PrecioFiable = false,
                                         Aviso        = "Precio pendiente de actualización"
                                     });
```

Más limpio si el paso que puede fallar guarda el valor él mismo:

```csharp
private MlResult<ProductoCatalogo> EnriquecerConPrecioActual(ProductoCatalogo producto)
    => _precios.Consultar(producto.Sku)
               .Map(precio => producto with { Precio = precio, PrecioFiable = true })
               .AddValueDetailIfFail(producto);      // ← el propio paso deja el valor listo

public MlResult<ProductoCatalogo> ObtenerProductoParaCatalogo(int productoId)
    => _repo.Obtener(productoId)
            .Bind(EnriquecerConPrecioActual)
            .MapIfFailWithValue(p => p with { PrecioFiable = false,
                                              Aviso        = "Precio pendiente de actualización" });
```

### Ejemplo 2: Forma B como traductor a modelo de vista

Queremos devolver siempre una tarjeta de cliente. Si la carga de detalles falla, mostramos
los datos mínimos que venían en la petición.

```csharp
public record ClienteBusquedaDto(string Nif, string NombreAproximado);

public record TarjetaClienteVm(string Nif, string Nombre, string Email, bool Completa);

public MlResult<TarjetaClienteVm> ObtenerTarjeta(ClienteBusquedaDto peticion)
    => _clientes.BuscarPorNif(peticion.Nif)                       // MlResult<Cliente>
                .AddValueDetailIfFail(peticion)                   // guardamos el DTO de entrada
                .MapIfFailWithValue<Cliente, ClienteBusquedaDto, TarjetaClienteVm>
                (
                    funcValid: c   => new TarjetaClienteVm(c.Nif, c.Nombre, c.Email, Completa: true),
                    funcFail : dto => new TarjetaClienteVm(dto.Nif, dto.NombreAproximado, "", Completa: false)
                );
```

Fíjate en que `TValue` (`ClienteBusquedaDto`) **no es** `T` (`Cliente`): eso es exactamente
lo que hace útil a la Forma B.

### Ejemplo 3: Reintento por un canal alternativo con `TryMapIfFailWithValue`

El envío del justificante se intenta por correo; si falla, generamos el PDF localmente. La
generación puede lanzar, así que usamos la variante protegida.

```csharp
public MlResult<Justificante> EmitirJustificante(SolicitudJustificante solicitud)
    => _correo.Enviar(solicitud)                                  // MlResult<Justificante>
              .AddValueDetailIfFail(solicitud)
              .ExecSelfIfFail(e => _log.LogWarning("Correo no disponible: {Detalle}",
                                                   e.ToErrorsDescription()))
              .TryMapIfFailWithValue<SolicitudJustificante, SolicitudJustificante, Justificante>
              (
                  funcValid: j   => j,
                  funcFail : s   => _pdf.GenerarLocal(s),          // puede lanzar IOException
                  errorMessageBuilder: ex => $"No se pudo generar el justificante local: {ex.Message}"
              );
```

Aquí encadenamos tres ideas del patrón: guardar el valor, **registrar antes de recuperar**
(porque la recuperación borra el error original) y proteger la recuperación con `Try*`.

### Ejemplo 4: Qué no hacer

```csharp
// ❌ 1) Llamar a MapIfFailWithValue sin haber guardado el valor
resultado.MapIfFailWithValue(p => p with { Ok = false });
// El delegado no se ejecuta y el error original se sustituye por "no hay valor".

// ❌ 2) Guardar el valor dos veces
resultado.AddValueDetailIfFail(pedido)
         .AddValueDetailIfFail(pedido);   // ArgumentException: la clave "Value" ya existe

// ❌ 3) Pedir un tipo que no coincide con el guardado
resultado.AddValueDetailIfFail(dto)                  // guardamos un DTO
         .MapIfFailWithValue(entidad => entidad);    // pedimos la entidad → falla

// ❌ 4) Usarlo para ocultar errores de programación
resultado.MapIfFailWithValue(x => x);   // "que no falle nunca" no es una estrategia
```

✅ En su lugar:

```csharp
// 1) y 2) un único guardado, en el paso que puede fallar
_precios.Consultar(sku).Map(…).AddValueDetailIfFail(producto);

// 3) tipos explícitos y coherentes
.MapIfFailWithValue<Entidad, Dto, Vm>(funcValid: …, funcFail: …);

// 4) si el fallo es un bug, déjalo propagar; registra y devuelve el error
resultado.ExecSelfIfFail(e => _log.LogError("{Detalle}", e.ToErrorsDescription()));
```

---

## Mejores Prácticas

1. **Guarda el valor en el mismo método que puede fallar.** Así nadie tiene que recordar el
   paso previo desde fuera: el resultado fallido ya viene «cargado».
2. **Un solo `AddValueDetailIfFail` por resultado.** El diccionario `Details` lanza si la
   clave `"Value"` se repite.
3. **Sé explícito con los genéricos en la Forma B** (`<T, TValue, TReturn>`); la inferencia
   con tres parámetros suele fallar y el mensaje del compilador no ayuda.
4. **Registra antes de recuperar.** Una vez recuperas, el error desaparece del resultado:
   `.ExecSelfIfFail(e => _log.LogWarning("{D}", e.ToErrorsDescription()))`.
5. **Marca el valor degradado.** Añade una propiedad (`PrecioFiable`, `Completa`, `Aviso`)
   para que el consumidor sepa que no es un resultado de primera categoría.
6. **Usa `BindIfFailWithValue` si la recuperación puede fallar.** No fuerces un `Map` con un
   delegado que en realidad necesita devolver un `MlResult`.
7. **Usa `TryMapIfFailWithValue` si el delegado puede lanzar**, con un
   `errorMessageBuilder` que diga qué se estaba intentando recuperar.
8. **Colócalo al final de la tubería.** Si recuperas demasiado pronto, los pasos siguientes
   trabajarán sobre un valor degradado sin saberlo.

---

## Apéndice: `MapDefault` (solo para depurar)

Justo antes de esta región, el fuente publica un método que conviene conocer para **no
usarlo**:

```csharp
public static MlResult<T> MapDefault<T>(this object source)
    => "Warning, MapDefault method is only valid tu debug code".ToMlResultFail<T>();

public static async Task<MlResult<T>> MapDefaultAsync<T>(this object source)
    => await (source ?? new object()).MapDefault<T>().ToAsync();
```

`MapDefault` **siempre devuelve un fallo** con ese mensaje de aviso (que incluye el propio
error tipográfico del literal, *«only valid tu debug code»*). Es un marcador de andamiaje
para cortar temporalmente una tubería mientras se depura. **Nunca debe quedar en código de
producción.**

---

## Resumen

- `MapIfFailWithValue` es `MapIfFail` pero el delegado de recuperación recibe **el valor
  original**, no los errores.
- El valor se lee de `Details["Value"]`, así que **hay que haberlo guardado antes** con
  `AddValueDetailIfFail`, `AddValueIfFail` o `MlErrorsDetails.FromErrorMessageWithValue`.
- Hay **dos formas**: la A (`<T>`, misma forma de salida) y la B (`<T, TValue, TReturn>`,
  proyecta las dos ramas a un tipo común y permite que el valor guardado sea de otro tipo).
- Si el valor **no está** o **no es del tipo pedido**, el delegado no se ejecuta y el error
  original se sustituye por el de «no hay valor».
- Como el delegado devuelve un valor desnudo, la recuperación es infalible: si el valor se
  recupera, **la salida es válida**.
- `TryMapIfFailWithValue` protege el delegado con `TryToMlResult`, pero pierde el error
  original: registra antes de recuperar.
- `MapDefault` existe pero es un marcador de depuración que siempre falla.

---

## Ver también

- [`4_MapIfFail.md`](4_MapIfFail.md) — recuperación con los errores en la mano.
- [`6_MapIfFailWithException.md`](6_MapIfFailWithException.md) — recuperación cuando el fallo lleva excepción.
- [`7_MapIfFailWithoutException.md`](7_MapIfFailWithoutException.md) — recuperación cuando el fallo **no** lleva excepción.
- [`1_Map.md`](1_Map.md) — la operación base.
- [`../Bind/7_BindIfFailWithValue.md`](../Bind/7_BindIfFailWithValue.md) — la versión que sí puede volver a fallar.
- [`../ExecSelf/4_ExecSelfIfFailWithValue.md`](../ExecSelf/4_ExecSelfIfFailWithValue.md) — efectos laterales con el valor recuperado.
- [`../Types/MlResultActions.md`](../Types/MlResultActions.md) — `AddValueDetailIfFail` y compañía.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `GetDetailValue<T>`, `AddValueIfFail`.
- [`../Types/MlResultErrors.md`](../Types/MlResultErrors.md) — estructura real de `MlErrorsDetails`.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la familia `Map`.