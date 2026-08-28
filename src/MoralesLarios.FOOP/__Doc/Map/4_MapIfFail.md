# MapIfFail — Volver al carril bueno con un valor de reemplazo

## Índice

1. [Introducción](#introducción)
2. [Las dos formas de `MapIfFail`](#las-dos-formas-de-mapiffail)
3. [Firmas reales e implementación](#firmas-reales-e-implementación)
4. [`MapIfFail` frente a `BindIfFail` y `Match`](#mapiffail-frente-a-bindiffail-y-match)
5. [El riesgo de tragarse los errores](#el-riesgo-de-tragarse-los-errores)
6. [Variantes asíncronas](#variantes-asíncronas)
7. [`TryMapIfFail` — cuando la recuperación puede lanzar](#trymapiffail--cuando-la-recuperación-puede-lanzar)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

`MapIfFail` es la operación de **recuperación garantizada**:

> **Si el resultado es fallido, produce un valor de reemplazo a partir de los errores. Si es válido, lo deja pasar.**

La palabra clave es *garantizada*: el delegado devuelve un valor **desnudo** (`T`), no un `MlResult<T>`. Por tanto **no puede fallar**, y el resultado de `MapIfFail` es siempre válido si el delegado no lanza.

```csharp
// ❌ Estilo imperativo con try/catch y variable mutable
Configuracion cfg;
try { cfg = LeerConfiguracion(ruta); }
catch { cfg = Configuracion.PorDefecto; }

// ✅ Estilo railway: el valor por defecto es una transformación del fallo
MlResult<Configuracion> cfg = LeerConfiguracion(ruta)
                                  .MapIfFail(errores => Configuracion.PorDefecto);
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`.

---

## Las dos formas de `MapIfFail`

Como en tantas familias de la biblioteca, hay **dos formas** con propósitos distintos.

| | Forma A — solo recuperación | Forma B — punto de convergencia |
|---|---|---|
| **Genéricos** | `<T>` | `<T, TReturn>` |
| **Delegados** | `Func<MlErrorsDetails, T> func` | `funcValid` **y** `funcFail` |
| **Tipo de salida** | El mismo: `MlResult<T>` | Puede cambiar: `MlResult<TReturn>` |
| **Rama válida** | El valor pasa **intacto** | Se transforma con `funcValid` |
| **Lectura** | «si falla, usa esto» | «unifica los dos carriles en un tipo común» |

```csharp
// Forma A: rescatar y seguir con el mismo tipo
MlResult<decimal> saldo = ConsultarSaldo(cuenta)
                              .MapIfFail(errores => 0m);

// Forma B: convertir ambos carriles en un único tipo de salida
MlResult<string> mensaje = ConsultarSaldo(cuenta)
                               .MapIfFail(funcValid: s        => $"Saldo disponible: {s:C}",
                                          funcFail : errores  => $"No se pudo consultar: {errores.ToErrorsDescription()}");
```

---

## Firmas reales e implementación

### Forma A — `<T>`, solo recuperación

```csharp
public static MlResult<T> MapIfFail<T>(this MlResult<T>              source,
                                            Func<MlErrorsDetails, T> func)
    => source.Match
                    (
                        fail : func,
                        valid: value => value
                    );
```

Es literalmente un `Match` en el que la rama válida es la identidad. El delegado recibe **el `MlErrorsDetails` completo**, así que puedes usar los errores para decidir el valor de reemplazo.

### Forma B — `<T, TReturn>`, punto de convergencia

```csharp
public static MlResult<TReturn> MapIfFail<T, TReturn>(this MlResult<T>                    source,
                                                           Func<T,               TReturn> funcValid,
                                                           Func<MlErrorsDetails, TReturn> funcFail)
    => source.Match
                    (
                        fail : funcFail,
                        valid: funcValid
                    );
```

| Estado de entrada | Forma A | Forma B |
|---|---|---|
| Válido | El mismo valor, intacto | `funcValid(valor)` |
| Fallido | `func(errorsDetails)` → **resultado válido** | `funcFail(errorsDetails)` → **resultado válido** |

> 📌 En ambas formas, **el resultado siempre sale válido cuando la entrada era fallida**. Eso es lo que distingue a `MapIfFail` de todo lo demás: es un punto de no retorno para los errores.

---

## `MapIfFail` frente a `BindIfFail` y `Match`

Tres formas de tratar la rama de fallo, con niveles de compromiso muy distintos:

| Operación | Devuelve el delegado | ¿Puede seguir fallando? | Sale del mundo `MlResult` |
|---|---|---|---|
| `MapIfFail` | Un valor desnudo (`T`) | **No** | No |
| [`BindIfFail`](../Bind/6_BindIfFail.md) | Un `MlResult<T>` | **Sí** | No |
| [`Match`](../Match/1_Match.md) | Cualquier tipo `TReturn` crudo | — | **Sí** |

```csharp
// MapIfFail: la recuperación es infalible (valor en memoria)
.MapIfFail(errores => Tarifa.Estandar)

// BindIfFail: el plan B también puede fallar (va a otro proveedor)
.BindIfFail(errores => _proveedorSecundario.Consultar(articulo))

// Match: ya no quiero un MlResult, quiero una respuesta HTTP
.Match(valid: t       => Ok(t),
       fail : errores => BadRequest(errores.ToErrorsMessages()))
```

> 🔑 **Criterio de elección**: si el plan B puede fallar, usa `BindIfFail`. Si es un valor seguro (una constante, un objeto por defecto, un cálculo local), usa `MapIfFail`. Y si ya no vas a seguir la tubería, usa `Match`.

La Forma B de `MapIfFail` y `Match` se parecen mucho, pero hay una diferencia esencial:

```csharp
// Match devuelve TReturn CRUDO: sales de la tubería
string texto = resultado.Match(valid: s => $"{s:C}", fail: e => "no disponible");

// MapIfFail Forma B devuelve MlResult<TReturn>: sigues dentro
MlResult<string> envuelto = resultado.MapIfFail(s => $"{s:C}", e => "no disponible");
// ...y por eso puedes continuar:
                                     .MapEnsure(t => t.Length < 50, "Texto demasiado largo");
```

---

## El riesgo de tragarse los errores

`MapIfFail` es la operación **más peligrosa** de la familia `Map`, porque **hace desaparecer los errores sin dejar rastro**. Un `MapIfFail` mal colocado convierte un fallo de infraestructura en un silencioso «todo bien».

```csharp
// ❌ Peligrosísimo: si la base de datos está caída, el usuario ve saldo 0
var saldo = _repo.ConsultarSaldo(cuenta).MapIfFail(_ => 0m);
```

Ese `0m` es indistinguible de un saldo real de cero. Dos medidas obligatorias:

**1. Registra antes de tragarte el error.** Usa [`ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md), que observa sin modificar:

```csharp
var saldo = _repo.ConsultarSaldo(cuenta)
                 .ExecSelfIfFail(errores => _log.LogWarning("Saldo no disponible para {Cuenta}: {Detalle}",
                                                            cuenta, errores.ToErrorsDescription()))
                 .MapIfFail(_ => 0m);
```

**2. Haz visible que el valor es de reemplazo.** No devuelvas un dato indistinguible del real:

```csharp
// ✅ El consumidor sabe que es un valor degradado
var saldo = _repo.ConsultarSaldo(cuenta)
                 .ExecSelfIfFail(e => _log.LogWarning("{Detalle}", e.ToErrorsDescription()))
                 .MapIfFail(errores => new SaldoInfo(Importe: 0m,
                                                     EsFiable: false,
                                                     Motivo: errores.ToErrorsDescription()));
```

**3. Distingue error técnico de error de negocio.** Un fallo de negocio puede tener un valor por defecto razonable; una excepción de infraestructura normalmente no debe silenciarse:

```csharp
.MapIfFail(errores => errores.GetDetailException().IsValid
                          ? throw new InvalidOperationException("Fallo técnico no recuperable")  // ⚠️ ver TryMapIfFail
                          : Tarifa.Estandar);
```

> 💡 Para ese último caso hay una herramienta mucho mejor: [`MapIfFailWithoutException`](7_MapIfFailWithoutException.md), que **solo** recupera cuando el fallo **no** lleva excepción asociada.

---

## Variantes asíncronas

`MapIfFailAsync` tiene **12 sobrecargas** en total: 4 para la Forma A y 8 para la Forma B.

### Forma A — 4 sobrecargas

| Origen | `func` |
|---|---|
| `MlResult<T>` | sync |
| `MlResult<T>` | **async** (`Func<MlErrorsDetails, Task<T>>`) |
| `Task<MlResult<T>>` | sync |
| `Task<MlResult<T>>` | **async** |

### Forma B — 8 sobrecargas

Combinaciones de origen (`MlResult<T>` / `Task<MlResult<T>>`) con la asincronía de `funcValid` y `funcFail`. Las mixtas se homogeneizan con `ToFuncTask()`, un helper de la biblioteca que convierte un `Func<A, B>` en un `Func<A, Task<B>>`:

```csharp
public static async Task<MlResult<TReturn>> MapIfFailAsync<T, TReturn>(this Task<MlResult<T>>              sourceAsync,
                                                                            Func<T,               TReturn> funcValid,
                                                                            Func<MlErrorsDetails, Task<TReturn>> funcFailAsync)
    => await (await sourceAsync).MapIfFailAsync(funcValid.ToFuncTask(), funcFailAsync);
```

> 📌 Recuerda la convención de la biblioteca: **el sufijo `Async` se refiere al origen y/o al delegado**, y el compilador resuelve la sobrecarga correcta. No tienes que memorizar los nombres, solo escribir lo natural.

---

## `TryMapIfFail` — cuando la recuperación puede lanzar

Aunque el delegado de `MapIfFail` *debería* ser infalible, a veces el valor por defecto se obtiene de algún sitio que puede reventar (deserializar un fichero de configuración de respaldo, leer una variable de entorno, etc.). Para eso está `TryMapIfFail`.

### Forma A

```csharp
public static MlResult<T> TryMapIfFail<T>(this MlResult<T>              source,
                                               Func<MlErrorsDetails, T> func,
                                               Func<Exception, string>  errorMessageBuilder)
    => source.Match
                    (
                        fail : errorDetails => func.TryToMlResult(errorDetails, errorMessageBuilder),
                        valid: value => value
                    );

// Sobrecarga con mensaje fijo
public static MlResult<T> TryMapIfFail<T>(this MlResult<T> source, Func<MlErrorsDetails, T> func,
                                               string errorMessage = null!)
    => source.TryMapIfFail(func, _ => errorMessage!);
```

### Forma B

```csharp
public static MlResult<TReturn> TryMapIfFail<T, TReturn>(this MlResult<T>                    source,
                                                              Func<T,               TReturn> funcValid,
                                                              Func<MlErrorsDetails, TReturn> funcFail,
                                                              Func<Exception, string>        errorMessageBuilder)
    => source.Match
                    (
                        fail : errorDetails => funcFail .TryToMlResult(errorDetails, errorMessageBuilder),
                        valid: x            => funcValid.TryToMlResult(x           , errorMessageBuilder)
                    );
```

Fíjate en que en la Forma B **las dos ramas están protegidas**.

⚠️ **Aviso importante sobre la pérdida de contexto**: si la recuperación lanza, el resultado es un **fallo nuevo** construido con tu mensaje y con la excepción en `Details["Ex"]`. **Los errores originales que motivaron la recuperación se pierden.** Si te importa conservarlos, regístralos antes:

```csharp
var cfg = LeerConfiguracion(ruta)
              .ExecSelfIfFail(e => _log.LogWarning("Config principal no disponible: {D}", e.ToErrorsDescription()))
              .TryMapIfFail(_ => JsonSerializer.Deserialize<Configuracion>(File.ReadAllText(rutaRespaldo))!,
                            ex => $"Tampoco se pudo leer la configuración de respaldo '{rutaRespaldo}': {ex.Message}");
```

Recuento: `TryMapIfFail` **4** sobrecargas síncronas (2 por forma: `string` y `Func<Exception, string>`) y **24** asíncronas.

---

## Ejemplos Prácticos

### Ejemplo 1: Valor por defecto con registro y degradación visible

```csharp
public async Task<MlResult<PreferenciasUsuario>> ObtenerPreferenciasAsync(int usuarioId)
    => await _repo.ObtenerPreferenciasAsync(usuarioId)

        // 1. Observar SIN silenciar: el fallo queda registrado
        .ExecSelfIfFailAsync(errores => _log.LogWarning(
                "Preferencias no disponibles para el usuario {UsuarioId}. Se usarán las de por defecto. Detalle: {Detalle}",
                usuarioId, errores.ToErrorsDescription()))

        // 2. Recuperar con un valor que DECLARA que es de reemplazo
        .MapIfFailAsync(errores => PreferenciasUsuario.PorDefecto with
                                   {
                                       EsPersonalizada = false,
                                       MotivoFallback  = errores.Errors.First().Message
                                   });
```

### Ejemplo 2: Forma B como traductor a modelo de vista

La Forma B es ideal justo antes de devolver datos a la capa de presentación, cuando **quieres un único tipo** que represente tanto el éxito como el fallo.

```csharp
public async Task<MlResult<TarjetaSaldoVm>> ObtenerTarjetaAsync(string iban)
    => await _banco.ConsultarSaldoAsync(iban)

        .MapIfFailAsync(
            // Rama válida: proyección normal
            funcValid: saldo => new TarjetaSaldoVm(
                                    Importe    = saldo.Importe.ToString("C"),
                                    Actualizado = saldo.Fecha.ToString("g"),
                                    Estado      = EstadoTarjeta.Correcto,
                                    Aviso       = null),

            // Rama fallida: la MISMA tarjeta, en modo degradado
            funcFail : errores => new TarjetaSaldoVm(
                                    Importe    = "—",
                                    Actualizado = "no disponible",
                                    Estado      = errores.GetDetailException().IsValid
                                                        ? EstadoTarjeta.ErrorTecnico
                                                        : EstadoTarjeta.SinDatos,
                                    Aviso       = errores.ToErrorsDescription()));
```

La interfaz recibe siempre un `TarjetaSaldoVm` y sabe pintar los tres estados. **Ningún `if` en la vista.**

### Ejemplo 3: Cadena de respaldos con `TryMapIfFail`

```csharp
public MlResult<CadenaConexion> ResolverConexion(string entorno)
    => // 1º intento: fichero de configuración del entorno
       LeerDesdeFichero($"appsettings.{entorno}.json")

        // 2º intento: variable de entorno (puede lanzar si el formato es inválido)
        .ExecSelfIfFail(e => _log.LogInformation("Sin fichero de configuración: {D}", e.ToErrorsDescription()))
        .TryMapIfFail(_ => CadenaConexion.Parsear(
                               Environment.GetEnvironmentVariable("DB_CONN")
                                   ?? throw new InvalidOperationException("La variable DB_CONN no está definida")),
                      ex => $"No se pudo resolver la conexión para el entorno '{entorno}': {ex.Message}")

        // 3º intento: solo en desarrollo, la conexión local. Aquí sí es infalible.
        .MapIfFail(errores => entorno == "Development"
                                  ? CadenaConexion.LocalDb
                                  : throw new InvalidOperationException(
                                        $"Configuración de base de datos ausente en '{entorno}'. {errores.ToErrorsDescription()}"));
```

> 💡 En el último paso se lanza a propósito: hay fallos de los que **no se debe** recuperar. Cuando la ausencia de configuración es un error de despliegue, lo correcto es que la aplicación no arranque.

### Ejemplo 4: Qué no hacer

```csharp
// ❌ 1. Recuperar sin dejar rastro: los errores se evaporan
var total = CalcularTotal(pedido).MapIfFail(_ => 0m);
// ✅ Observa primero
var total2 = CalcularTotal(pedido)
                 .ExecSelfIfFail(e => _log.LogError("Cálculo fallido: {D}", e.ToErrorsDescription()))
                 .MapIfFail(_ => 0m);


// ❌ 2. Usar MapIfFail cuando el plan B puede fallar → anidamiento
MlResult<MlResult<Tarifa>> mal = _principal.Consultar(art)
                                    .MapIfFail(_ => _secundario.Consultar(art));  // devuelve MlResult<Tarifa>
// ✅ Eso es BindIfFail
MlResult<Tarifa> bien = _principal.Consultar(art)
                            .BindIfFail(_ => _secundario.Consultar(art));


// ❌ 3. MapIfFail al principio de la tubería: neutraliza TODAS las validaciones posteriores
//    porque a partir de ahí nunca hay fallo que propagar hacia atrás... pero tampoco contexto.
var mal3 = _repo.Obtener(id).MapIfFail(_ => Pedido.Vacio)
                            .MapEnsure(p => p.Lineas.Any(), "Sin líneas");   // ahora falla por el valor FALSO
// ✅ Coloca la recuperación al FINAL, cuando ya has validado
var bien3 = _repo.Obtener(id).MapEnsure(p => p.Lineas.Any(), "Sin líneas")
                             .ExecSelfIfFail(e => _log.LogWarning("{D}", e.ToErrorsDescription()))
                             .MapIfFail(_ => Pedido.Vacio);


// ❌ 4. Perder el error original al usar TryMapIfFail sin registrarlo antes
var mal4 = Principal().TryMapIfFail(_ => Respaldo(), "El respaldo también falló");
//    Si el respaldo lanza, el error de Principal() ya no está en ninguna parte.
// ✅ Registra o fusiona antes
var bien4 = Principal().ExecSelfIfFail(e => _log.LogWarning("{D}", e.ToErrorsDescription()))
                       .TryMapIfFail(_ => Respaldo(), ex => $"El respaldo también falló: {ex.Message}");
```

---

## Mejores Prácticas

1. **Nunca uses `MapIfFail` sin observar antes el fallo.** Combínalo siempre con [`ExecSelfIfFail`](../ExecSelf/3_ExecSelfIfFail.md) o con un `AddMlErrorDetailIfFail` previo. Un error silenciado es un error que depurarás dos semanas después.

2. **Colócalo al final de la tubería.** `MapIfFail` cierra la rama de error; todo lo que pongas después trabajará ya con el valor de reemplazo.

3. **Si el plan B puede fallar, usa [`BindIfFail`](../Bind/6_BindIfFail.md).** El síntoma de haberte equivocado es el tipo anidado `MlResult<MlResult<T>>`.

4. **Haz que el valor de reemplazo sea distinguible del real.** Añade una bandera (`EsFiable`, `EsPersonalizada`) o un motivo. Un `0` mudo es una bomba de relojería.

5. **Usa la Forma B como traductor final a modelo de vista.** Unifica los dos carriles en un tipo que la presentación sabe pintar, sin `if` en la vista.

6. **Aprovecha el `MlErrorsDetails` que recibes.** No lo ignores con `_ =>`: puede alimentar el motivo, decidir entre varios valores por defecto o distinguir error técnico de error de negocio con `GetDetailException().IsValid`.

7. **Para «recuperar solo si no fue una excepción», usa [`MapIfFailWithoutException`](7_MapIfFailWithoutException.md)**, no un `if` dentro del delegado.

8. **Con `TryMapIfFail`, recuerda que los errores originales se pierden** si la recuperación lanza. Regístralos antes o considera fusionar con `MergeErrorsDetailsIfFail`.

---

## Resumen

- `MapIfFail` **recupera un fallo con un valor de reemplazo infalible**: el delegado devuelve `T` desnudo, no `MlResult<T>`.
- Hay **dos formas**: la **A** (`<T>`, solo recuperación, la rama válida pasa intacta) y la **B** (`<T, TReturn>`, con `funcValid` y `funcFail`, punto de convergencia que puede cambiar el tipo).
- Ambas son un `Match` con conversión implícita: **si la entrada era fallida, la salida es válida**. Los errores desaparecen.
- Recuento: `MapIfFail` **2** síncronas y **12** asíncronas; `TryMapIfFail` **4** síncronas y **24** asíncronas.
- `TryMapIfFail` protege la recuperación con `TryToMlResult` (excepción a `Details["Ex"]`), pero **pierde los errores originales** si el delegado lanza.
- La Forma B se diferencia de [`Match`](../Match/1_Match.md) en que **devuelve `MlResult<TReturn>`** y permite seguir la tubería, mientras que `Match` devuelve el tipo crudo y sale de ella.
- Es la operación más peligrosa de la familia: **combínala siempre con `ExecSelfIfFail`** y haz visible que el valor es degradado.
- Si el plan B puede fallar → [`BindIfFail`](../Bind/6_BindIfFail.md). Si solo debe recuperarse ante fallos de negocio → [`MapIfFailWithoutException`](7_MapIfFailWithoutException.md).

---

## Ver también

- [`1_Map.md`](1_Map.md) — la regla `Map` frente a `Bind`.
- [`3_MapIf.md`](3_MapIf.md) — transformar según una condición sobre el valor.
- [`5_MapIfFailWithValue.md`](5_MapIfFailWithValue.md) — recuperar usando el valor guardado en los detalles.
- [`7_MapIfFailWithoutException.md`](7_MapIfFailWithoutException.md) — recuperar solo ante fallos sin excepción.
- [`8_MapAlways.md`](8_MapAlways.md) — transformar sea válido o fallido.
- [`../Bind/6_BindIfFail.md`](../Bind/6_BindIfFail.md) — recuperación que también puede fallar.
- [`../ExecSelf/3_ExecSelfIfFail.md`](../ExecSelf/3_ExecSelfIfFail.md) — observar el fallo sin modificarlo.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir del mundo `MlResult`.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la clase.