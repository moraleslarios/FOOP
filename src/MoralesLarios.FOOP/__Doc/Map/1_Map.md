# Map — Transformar el valor sin salir del carril

## Índice

1. [Introducción](#introducción)
2. [`Map` frente a `Bind`: la regla de oro](#map-frente-a-bind-la-regla-de-oro)
3. [Firma real e implementación](#firma-real-e-implementación)
4. [El error clásico: `MlResult<MlResult<T>>`](#el-error-clásico-mlresultmlresultt)
5. [`TryMap` — cuando la transformación puede lanzar](#trymap--cuando-la-transformación-puede-lanzar)
6. [Variantes asíncronas](#variantes-asíncronas)
7. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)
10. [Resumen](#resumen)
11. [Ver también](#ver-también)

---

## Introducción

`Map` es la operación más simple y más usada de toda la biblioteca:

> **Si el resultado es válido, transforma el valor. Si es fallido, no hace nada y propaga el fallo.**

La diferencia con [`Bind`](../Bind/3_Bind.md) está en el tipo del delegado, y es la única cosa que hay que aprender:

| | Delegado | Se usa cuando… |
|---|---|---|
| `Map` | `Func<T, TReturn>` | La transformación **no puede fallar** |
| `Bind` | `Func<T, MlResult<TReturn>>` | La transformación **puede fallar** |

```csharp
// Map: formatear un importe no puede fallar
MlResult<string> texto = ObtenerFactura(id)
                            .Map(f => $"{f.Numero} — {f.Total:C}");

// Bind: cobrar sí puede fallar
MlResult<Recibo> recibo = ObtenerFactura(id)
                            .Bind(f => Cobrar(f));
```

> ⚠️ **Sobre `MlErrorsDetails`**: `MlErrorsDetails` solo expone dos propiedades públicas: `Errors` (la colección de `MlError`) y `Details` (un `Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Para leer los mensajes usa `ToErrorsMessages()` o `ToErrorsDescription()`; para llegar a la excepción usa `GetDetailException()`.

---

## `Map` frente a `Bind`: la regla de oro

Piénsalo así: `Map` transforma **lo que hay dentro de la caja**; `Bind` sustituye **la caja entera**.

```
Map:    MlResult<Pedido> ──[ Pedido → decimal ]──►  MlResult<decimal>
                             (no falla)

Bind:   MlResult<Pedido> ──[ Pedido → MlResult<Factura> ]──►  MlResult<Factura>
                             (puede fallar)                    (aplanado, no anidado)
```

La regla práctica es mecánica:

> **Si tu lambda ya devuelve un `MlResult<...>`, usa `Bind`. Si devuelve un valor normal, usa `Map`.**

---

## Firma real e implementación

```csharp
public static MlResult<TReturn> Map<T, TReturn>(this MlResult<T>      source,
                                                     Func<T, TReturn> func)
    => source.Match
    (
        fail : MlResult<TReturn>.Fail,
        valid: value => func(value)
    );
```

Dos observaciones sobre este cuerpo:

1. **La rama de fallo cambia el tipo genérico pero conserva los errores intactos.** `MlResult<TReturn>.Fail` se pasa como *grupo de métodos*: recibe los `MlErrorsDetails` originales y los reempaqueta en el nuevo tipo. Ni un mensaje ni un detalle se pierden.

2. **La rama válida se apoya en la conversión implícita.** `func(value)` devuelve un `TReturn` desnudo, que se convierte automáticamente en `MlResult<TReturn>` válido gracias al operador implícito definido en `MlResult<T>`.

| Estado de entrada | `func` | Resultado |
|---|---|---|
| Válido | Se ejecuta | Válido con el valor transformado |
| Fallido | **No se ejecuta** | El mismo fallo, con el tipo genérico cambiado |

---

## El error clásico: `MlResult<MlResult<T>>`

Es el tropiezo número uno de quien empieza. Si usas `Map` con una función que devuelve `MlResult`, el compilador no se queja: te devuelve un resultado anidado que no sirve para nada.

```csharp
// ❌ MAL: el tipo resultante es MlResult<MlResult<Factura>>
MlResult<MlResult<Factura>> mal = ObtenerPedido(id)
                                     .Map(p => Facturar(p));      // Facturar devuelve MlResult<Factura>

// A partir de aquí todo se vuelve incómodo: el fallo de Facturar queda "escondido"
// dentro de un resultado VÁLIDO, y las comprobaciones de IsValid mienten.

// ✅ BIEN: Bind aplana
MlResult<Factura> bien = ObtenerPedido(id)
                            .Bind(p => Facturar(p));
```

> 🔎 **Síntoma para detectarlo**: si ves un `IsValid` que dice `true` pero el resultado "no funciona", o si necesitas escribir `.Value.Value`, casi seguro has usado `Map` donde tocaba `Bind`.

---

## `TryMap` — cuando la transformación puede lanzar

Una transformación puede no devolver `MlResult` y aun así romperse: un `int.Parse`, un acceso a un índice, una división. Para eso está `TryMap`, que envuelve la llamada en un `try/catch` y convierte la excepción en un fallo.

```csharp
public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T>             source,
                                                        Func<T, TReturn>        func,
                                                        Func<Exception, string> errorMessageBuilder)
    => source.Match
    (
        fail : MlResult<TReturn>.Fail,
        valid: value => func.TryToMlResult(source.Value, errorMessageBuilder)
    );

// Sobrecarga cómoda con mensaje fijo
public static MlResult<TReturn> TryMap<T, TReturn>(this MlResult<T>      source,
                                                        Func<T, TReturn> func,
                                                        string           exceptionAditionalMessage = null!)
    => TryMap(source, func, _ => exceptionAditionalMessage!);
```

El trabajo real lo hace `TryToMlResult`, que además de capturar la excepción **la guarda en `Details["Ex"]`**. Eso significa que después puedes distinguir un fallo técnico de un fallo de negocio:

```csharp
var r = LeerConfiguracion()
            .TryMap(texto => int.Parse(texto["Timeout"]),
                    ex => $"El timeout configurado no es un número válido: {ex.Message}");

// El fallo lleva la excepción dentro y las familias WithException pueden actuar sobre ella
r.ExecSelfIfFailWithException(ex => _log.LogError(ex, "Configuración inválida"));
```

Las dos formas de construir el mensaje aparecen en toda la biblioteca:

| Parámetro | Cuándo usarlo |
|---|---|
| `string exceptionAditionalMessage` | Mensaje fijo, no dependes del texto de la excepción |
| `Func<Exception, string> errorMessageBuilder` | Quieres incluir datos de la excepción en el mensaje |

---

## Variantes asíncronas

`MapAsync` tiene 4 sobrecargas que cubren las combinaciones de origen y delegado:

| Origen | Delegado | Método |
|---|---|---|
| `MlResult<T>` | `Func<T, TReturn>` | `MapAsync` (envuelve con `ToAsync()`) |
| `MlResult<T>` | `Func<T, Task<TReturn>>` | `MapAsync` |
| `Task<MlResult<T>>` | `Func<T, Task<TReturn>>` | `MapAsync` |
| `Task<MlResult<T>>` | `Func<T, TReturn>` | `MapAsync` |

Y `TryMapAsync` tiene 8, porque cada combinación se duplica según cómo se construya el mensaje de error (`string` o `Func<Exception, string>`).

Gracias a la sobrecarga con origen `Task<MlResult<T>>`, **puedes encadenar sin `await` intermedios**:

```csharp
public Task<MlResult<ClienteDto>> ObtenerDtoAsync(int id)
    => ObtenerClienteAsync(id)                          // Task<MlResult<Cliente>>
        .MapAsync(c => new ClienteDto(c.Id, c.Nombre))   // sin await
        .MapAsync(async dto => dto with                  // delegado asíncrono
        {
            Avatar = await _avatares.ObtenerUrlAsync(dto.Id)
        });
```

> 💡 **Regla de nomenclatura**: en cuanto un eslabón de la cadena es asíncrono, todos los siguientes usan el sufijo `Async`, aunque su delegado sea síncrono. El sufijo se refiere al **origen**, no al delegado.

---

## Tabla de decisión rápida

| Necesito… | Método |
|---|---|
| Transformar el valor; no puede fallar | `Map` |
| Transformar el valor; puede lanzar excepción | `TryMap` |
| Transformar el valor; devuelve `MlResult` | [`Bind`](../Bind/3_Bind.md) |
| Comprobar una condición sobre el valor | [`MapEnsure`](2_MapEnsure.md) |
| Transformar solo si se cumple una condición | [`MapIf`](3_MapIf.md) |
| Transformar el fallo en un valor | [`MapIfFail`](4_MapIfFail.md) |
| Mirar el valor sin transformarlo (log, métrica) | [`ExecSelf`](../ExecSelf/1_ExecSelf.md) |
| Salir del `MlResult` con un valor final | [`Match`](../Match/1_Match.md) |

---

## Ejemplos Prácticos

### Ejemplo 1: Proyección a DTO en una capa de aplicación

El caso más habitual: la tubería obtiene entidades y `Map` las convierte en lo que la API expone.

```csharp
public async Task<MlResult<PedidoResumenDto>> ObtenerResumenAsync(int pedidoId)
    => await EnsureFp.That(pedidoId, pedidoId > 0, "El identificador de pedido debe ser positivo")

        // Bind: la lectura puede fallar (no existe, sin permisos…)
        .BindAsync(id => _repo.ObtenerPedidoAsync(id))

        // Map: proyectar a DTO nunca falla
        .MapAsync(pedido => new PedidoResumenDto(
                                Numero   : pedido.Numero,
                                Cliente  : pedido.Cliente.RazonSocial,
                                Lineas   : pedido.Lineas.Count,
                                Total    : pedido.Lineas.Sum(l => l.Cantidad * l.Precio),
                                Estado   : pedido.Estado.ToString()))

        // Map: enriquecer el DTO tampoco falla
        .MapAsync(dto => dto with { TotalFormateado = dto.Total.ToString("C", _cultura) })

        .AddMlErrorDetailIfFailAsync("[Aplicación] No se pudo construir el resumen del pedido");
```

### Ejemplo 2: `TryMap` en el borde del sistema (parseo)

Todo lo que entra desde fuera —ficheros, cabeceras HTTP, variables de entorno— es material para `TryMap`.

```csharp
public MlResult<ParametrosLote> LeerParametros(IDictionary<string, string> crudos)
    => crudos.ToMlResultValid()

        .TryMap(d => new ParametrosLote(
                        FechaProceso : DateTime.ParseExact(d["fecha"], "yyyyMMdd", CultureInfo.InvariantCulture),
                        TamanoPagina : int.Parse(d["pagina"]),
                        Reintentos   : int.Parse(d["reintentos"])),
                ex => $"Los parámetros del lote no son válidos ({ex.GetType().Name}): {ex.Message}")

        // MapEnsure para las reglas de negocio, que no lanzan
        .MapEnsure(p => p.TamanoPagina is > 0 and <= 1000,
                        "El tamaño de página debe estar entre 1 y 1000")
        .MapEnsure(p => p.Reintentos <= 5,
                        "No se admiten más de 5 reintentos");
```

### Ejemplo 3: Cadena mixta `Map` / `Bind` bien tipada

```csharp
public async Task<MlResult<CertificadoDto>> EmitirCertificadoAsync(SolicitudDto dto)
    => await ValidarSolicitudAsync(dto)                                  // MlResult<Solicitud>

        .BindAsync(s => _registro.BuscarTitularAsync(s.Nif)              // puede fallar → Bind
                                 .Map(titular => (Solicitud: s, Titular: titular)))

        .MapAsync(par => new BorradorCertificado(                        // no falla → Map
                             Titular  : par.Titular.NombreCompleto,
                             Concepto : par.Solicitud.Concepto,
                             Fecha    : DateTime.UtcNow))

        .TryMapAsync(async b => await _pdf.RenderizarAsync(b),           // puede lanzar → TryMap
                     ex => $"Error generando el PDF del certificado: {ex.Message}")

        .MapAsync(pdf => new CertificadoDto(pdf.Id, pdf.Bytes.Length));  // no falla → Map
```

Fíjate en el patrón `Bind(... .Map(x => (A: …, B: …)))`: es la forma idiomática de **arrastrar dos valores** por la tubería sin variables externas.

### Ejemplo 4: Lo que **no** debes hacer

```csharp
// ❌ Map con una función que devuelve MlResult → anidamiento
resultado.Map(p => Facturar(p));

// ❌ Map con una función que puede lanzar → la excepción escapa de la tubería
resultado.Map(t => int.Parse(t));

// ❌ Map con efectos secundarios y sin transformación real
resultado.Map(p => { _log.LogInformation("{P}", p); return p; });

// ✅ Cada caso con su herramienta
resultado.Bind(p => Facturar(p));
resultado.TryMap(t => int.Parse(t), ex => $"Valor no numérico: {ex.Message}");
resultado.ExecSelf(p => _log.LogInformation("{P}", p));
```

---

## Mejores Prácticas

1. **Mantén `Map` puro.** Si la lambda escribe en un log, en disco o en una variable externa, lo que quieres es [`ExecSelf`](../ExecSelf/1_ExecSelf.md).

2. **Ante la duda, mira el tipo de retorno de tu lambda.** ¿Devuelve `MlResult<...>`? → `Bind`. ¿Un valor normal? → `Map`. ¿Puede lanzar? → `TryMap`.

3. **Usa `TryMap` en todas las fronteras.** Parseos, deserializaciones, reflexión, cálculos con división: cualquier cosa que pueda lanzar debe entrar por `TryMap`, no por `Map`.

4. **Un `Map` por concepto.** Es más legible una cadena de tres `Map` con nombres claros que un único `Map` con una lambda de veinte líneas.

5. **No accedas a `.Value`.** `Map` existe precisamente para no tener que hacerlo. Si necesitas salir del `MlResult`, hazlo con `Match` al final.

6. **Elige la sobrecarga `Func<Exception, string>` cuando el mensaje deba incluir el motivo real.** El mensaje fijo es cómodo, pero pierde información diagnóstica útil.

7. **No te preocupes por los `await` intermedios.** Las sobrecargas con origen `Task<MlResult<T>>` permiten cadenas limpias con un solo `await` al principio.

---

## Resumen

- `Map` transforma el valor de un resultado válido y propaga los fallos sin tocarlos.
- Implementación real: `source.Match(fail: MlResult<TReturn>.Fail, valid: value => func(value))`.
- La rama de fallo **cambia el tipo genérico pero conserva errores y detalles intactos**.
- Usa `Map` para funciones que **no fallan**; usa [`Bind`](../Bind/3_Bind.md) para las que devuelven `MlResult`.
- El anidamiento `MlResult<MlResult<T>>` es la señal inequívoca de haber usado `Map` en lugar de `Bind`.
- `TryMap` captura la excepción, la convierte en fallo y **la guarda en `Details["Ex"]`**, con 2 sobrecargas síncronas y 8 asíncronas.
- `MapAsync` tiene 4 sobrecargas que cubren origen y delegado síncrono/asíncrono.

---

## Ver también

- [`2_MapEnsure.md`](2_MapEnsure.md) — validar condiciones sobre el valor.
- [`3_MapIf.md`](3_MapIf.md) — transformar solo si se cumple una condición.
- [`4_MapIfFail.md`](4_MapIfFail.md) — transformar el fallo en un valor.
- [`8_MapAlways.md`](8_MapAlways.md) — transformar siempre, sea válido o fallido.
- [`../Bind/3_Bind.md`](../Bind/3_Bind.md) — la operación hermana para funciones que fallan.
- [`../Match/1_Match.md`](../Match/1_Match.md) — salir del `MlResult`.
- [`../ExecSelf/1_ExecSelf.md`](../ExecSelf/1_ExecSelf.md) — efectos secundarios sin transformar.
- [`../Types/MlResultActionsMap.md`](../Types/MlResultActionsMap.md) — inventario completo de la clase.