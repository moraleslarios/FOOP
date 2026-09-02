# `EnsureFp` — Agregación de reglas: `All`, `AllOrFirst`, `Any`

> Archivo fuente: `Helpers/EnsureFp.Aggregation.cs`.

## Índice

- [El problema: encadenar `Bind` oculta errores](#el-problema-encadenar-bind-oculta-errores)
- [Inventario de la API](#inventario-de-la-api)
- [1. `All`: acumula todos los errores](#1-all-acumula-todos-los-errores)
- [2. `AllResults`: reglas ya evaluadas](#2-allresults-reglas-ya-evaluadas)
- [3. `AllOrFirst`: corta en el primer fallo](#3-allorfirst-corta-en-el-primer-fallo)
- [4. `Any`: basta con que una regla se cumpla](#4-any-basta-con-que-una-regla-se-cumpla)
- [5. Variantes asíncronas](#5-variantes-asíncronas)
- [6. Semántica de fusión de errores](#6-semántica-de-fusión-de-errores)
- [7. Tabla de decisión](#7-tabla-de-decisión)
- [8. Ejemplos completos](#8-ejemplos-completos)
- [9. Mejores prácticas](#9-mejores-prácticas)
- [Ver también](#ver-también)

---

## El problema: encadenar `Bind` oculta errores

Cuando se validan varios campos de un DTO con `Bind`, el flujo se corta en el **primer** fallo.
El usuario corrige ese campo, reenvía el formulario y descubre el segundo error. Y así
sucesivamente.

```csharp
// ❌ Solo verás el primer error: el usuario tendrá que hacer 3 viajes.
var r = NotNullEmptyOrWhitespace(dto.Nombre, "El nombre es obligatorio.")
    .Bind(_ => NotNullEmptyOrWhitespace(dto.Email, "El email es obligatorio."))
    .Bind(_ => That(dto, dto.Edad >= 18, "Debes ser mayor de edad."));
```

```csharp
// ✅ Verás TODOS los errores de golpe.
var r = All(dto,
            d => NotNullEmptyOrWhitespace(d.Nombre, "El nombre es obligatorio.").Map(_ => d),
            d => NotNullEmptyOrWhitespace(d.Email,  "El email es obligatorio.").Map(_ => d),
            d => That(d, d.Edad >= 18, "Debes ser mayor de edad."));
```

Esta familia resuelve exactamente eso: **componer varias reglas sobre el mismo valor** con tres
estrategias distintas (todas, primera, alguna).

---

## Inventario de la API

```csharp
// Todas las reglas se ejecutan; se fusionan los errores de las que fallen.
public static MlResult<T> All<T>(T value, params Func<T, MlResult<T>>[] validators);
public static MlResult<T> All<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators);

// Reglas YA evaluadas (resultados, no delegados).
public static MlResult<T> AllResults<T>(T value, params MlResult<T>[] results);

// Fail-fast: se detiene en la primera regla que falle.
public static MlResult<T> AllOrFirst<T>(T value, params Func<T, MlResult<T>>[] validators);
public static MlResult<T> AllOrFirst<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators);

// Válido si al menos una regla se cumple; si ninguna, se fusionan todos los errores.
public static MlResult<T> Any<T>(T value, params Func<T, MlResult<T>>[] validators);
public static MlResult<T> Any<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators);

// Variantes asíncronas.
public static Task<MlResult<T>> AllAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
public static Task<MlResult<T>> AllOrFirstAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
public static Task<MlResult<T>> AnyAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
```

> ℹ️ **¿Por qué `AllResults` y no `All`?** Una sobrecarga `params MlResult<T>[]` junto a
> `params Func<T,MlResult<T>>[]` provoca ambigüedad de resolución (`CS0121`) en muchas llamadas.
> El nombre distinto es deliberado y hace explícito que los resultados **ya se han evaluado**.

---

## 1. `All`: acumula todos los errores

Ejecuta **todos** los validadores, aunque alguno falle, y fusiona los errores de los que fallen
en un único `MlErrorsDetails`.

```csharp
public MlResult<ClienteDto> Validar(ClienteDto dto) =>
    All(dto,
        d => NotNullEmptyOrWhitespace(d.Nombre, "El nombre es obligatorio.").Map(_ => d),
        d => IsValidEmail(d.Email, "El email no tiene un formato válido.").Map(_ => d),
        d => InRange(d.Edad, 18, 120, "La edad debe estar entre 18 y 120.").Map(_ => d),
        d => NotEmptyGuid(d.TenantId, "El tenant es obligatorio.").Map(_ => d));
```

Si fallan tres de las cuatro reglas, el resultado contiene los tres mensajes:

```csharp
var errores = resultado.SecureFailErrorsDetails().ToErrorsMessages();
// "El nombre es obligatorio.", "El email no tiene un formato válido.", "El tenant es obligatorio."
```

**Patrón `.Map(_ => d)`:** cada validador debe devolver `MlResult<T>` del **mismo** tipo `T`. Como
las reglas especializadas devuelven el tipo del campo (`MlResult<string>`, `MlResult<int>`…), se
reproyecta al objeto completo con `.Map(_ => d)`. Es el idioma habitual de esta familia.

**Coste:** `All` ejecuta todas las reglas siempre. Si alguna es costosa (I/O, consulta a base de
datos), colócala en un `AllOrFirst` posterior o usa `Bind` para separar las fases.

---

## 2. `AllResults`: reglas ya evaluadas

Cuando ya tienes los `MlResult` calculados (por ejemplo porque vienen de métodos distintos), usa
`AllResults`:

```csharp
var rNombre = NotNullEmptyOrWhitespace(dto.Nombre, "Nombre obligatorio.").Map(_ => dto);
var rEmail  = IsValidEmail(dto.Email, "Email no válido.").Map(_ => dto);
var rEdad   = InRange(dto.Edad, 18, 120, "Edad fuera de rango.").Map(_ => dto);

var resultado = AllResults(dto, rNombre, rEmail, rEdad);
```

Aquí **todas** las reglas se han evaluado ya (no hay pereza posible), así que úsalo solo con
comprobaciones baratas o cuyos resultados necesites por separado de todas formas.

---

## 3. `AllOrFirst`: corta en el primer fallo

Misma semántica de composición que `All`, pero **fail-fast**: en cuanto una regla falla, se
devuelve su error y no se evalúan las siguientes.

```csharp
public MlResult<Pedido> Procesar(Pedido pedido) =>
    AllOrFirst(pedido,
        p => NotNull(p, "El pedido es obligatorio."),
        p => NotEmpty(p.Lineas, "El pedido no tiene líneas.").Map(_ => p),
        p => That(p, ExisteEnAlmacen(p), "Alguna línea no tiene stock."));   // ← caro: no se ejecuta si falla antes
```

Úsalo cuando:

- Las reglas están **ordenadas por dependencia** (no tiene sentido comprobar las líneas si el
  pedido es `null`).
- Alguna regla es **costosa** y no quieres pagarla si ya sabes que la petición es inválida.

> Es equivalente a encadenar `Bind`, pero mantiene la lista de reglas visible y homogénea, lo que
> facilita añadirlas, quitarlas o generarlas dinámicamente.

---

## 4. `Any`: basta con que una regla se cumpla

Valida el valor si **al menos una** de las reglas se cumple. Si **ninguna** se cumple, fusiona
todos los errores para explicar todas las alternativas rechazadas.

```csharp
// Un identificador válido puede ser un DNI, un NIE o un pasaporte.
public MlResult<string> ValidarDocumento(string doc) =>
    Any(doc,
        d => Matches(d, @"^\d{8}[A-Z]$",        "No es un DNI válido."),
        d => Matches(d, @"^[XYZ]\d{7}[A-Z]$",   "No es un NIE válido."),
        d => Matches(d, @"^[A-Z]{2}\d{6}$",     "No es un pasaporte válido."));
```

Si `doc` no encaja con ninguno de los tres patrones, el error contiene los tres mensajes, lo que
es exactamente la información que el usuario necesita.

> ⚠️ `Any` **evalúa las reglas hasta encontrar una válida**; el resto no se ejecuta. Si necesitas
> saber cuáles concretamente se cumplen, usa `All` y analiza el resultado.

---

## 5. Variantes asíncronas

```csharp
public static Task<MlResult<T>> AllAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
public static Task<MlResult<T>> AllOrFirstAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
public static Task<MlResult<T>> AnyAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators);
```

Los validadores se ejecutan **secuencialmente** (con `await` en cada uno), no en paralelo. Esto es
deliberado: la mayoría de las reglas asíncronas tocan el mismo `DbContext` o el mismo
`HttpClient`, y el paralelismo produciría errores de concurrencia.

```csharp
public Task<MlResult<UsuarioDto>> ValidarAsync(UsuarioDto dto) =>
    AllAsync(dto,
        async d => (await _repo.ExisteEmailAsync(d.Email))
                       ? MlResult<UsuarioDto>.Fail("El email ya está registrado.")
                       : MlResult<UsuarioDto>.Valid(d),
        async d => (await _repo.ExisteAliasAsync(d.Alias))
                       ? MlResult<UsuarioDto>.Fail("El alias ya está en uso.")
                       : MlResult<UsuarioDto>.Valid(d));
```

Si necesitas paralelismo real sobre una colección, usa
[`ProjectionParallelAsync`](../Bucle/Bucles.md) en lugar de esta familia.

---

## 6. Semántica de fusión de errores

La lógica es la misma para `All`, `AllResults` y `Any` (rama de fallo total):

| Nº de reglas fallidas | Resultado |
|---|---|
| 0 | `Valid(value)` |
| 1 | ese mismo `MlErrorsDetails`, **sin envolver** |
| 2 o más | `failsDetails[0].Merge(failsDetails.Skip(1))` |

Consecuencias prácticas:

- Con un único fallo, el error conserva **exactamente** su forma original (mensajes y `Details`),
  así que las claves como `ParamName` o `Expected` siguen siendo legibles.
- Con varios fallos, `Merge` concatena las listas de errores y fusiona los diccionarios `Details`.
  Si dos errores usan la **misma clave** de detalle, prevalece el primero.

**Casos límite verificados por pruebas:**

| Entrada | Resultado |
|---|---|
| `validators` es `null` | `Valid(value)` |
| `validators` está vacío | `Valid(value)` |
| Algún validador individual es `null` | se **omite** (no cuenta como fallo) |
| Todos los validadores son `null` | `Valid(value)` |

La razón: «no hay reglas» no es lo mismo que «las reglas no se cumplen». Un conjunto vacío de
restricciones se satisface trivialmente.

---

## 7. Tabla de decisión

| Necesito… | Usa |
|---|---|
| Mostrar al usuario **todos** los campos mal rellenados | `All` |
| Componer resultados que ya he calculado | `AllResults` |
| Cortar antes de una regla costosa o dependiente | `AllOrFirst` |
| Aceptar el valor si encaja con **alguno** de varios formatos | `Any` |
| Lo mismo, con reglas que consultan base de datos | `AllAsync` / `AllOrFirstAsync` / `AnyAsync` |
| Validar **cada elemento** de una colección | [`Bucles`](../Bucle/Bucles.md) (`Projection*`) |
| Validar reglas encadenadas donde cada paso transforma el valor | [`Bind`](../Bind/3_Bind.md) |

> ℹ️ Comparación con [`BindMulti`](../Bind/4_BindMulti.md): `BindMulti` acumula errores de varias
> funciones que pueden devolver **tipos distintos** dentro de una cadena ya iniciada. `All` opera
> sobre **un mismo valor y un mismo tipo** y es el punto de entrada natural desde un DTO.
> Y a diferencia de [`Combine`](../Several/4_Combine.md), esta familia **sí** acumula errores.

---

## 8. Ejemplos completos

### 8.1. Validación completa de un DTO de alta

```csharp
using static MoralesLarios.OOFP.Helpers.EnsureFp;

public record AltaClienteDto(string Nombre, string Email, string Telefono, int Edad, Guid TenantId);

public MlResult<AltaClienteDto> Validar(AltaClienteDto dto) =>
    NotNullArg(dto)
        .Bind(d => All(d,
            x => LengthBetween(x.Nombre, 3, 100, "El nombre debe tener entre 3 y 100 caracteres.").Map(_ => x),
            x => IsValidEmail(x.Email,   "El email no tiene un formato válido.").Map(_ => x),
            x => Matches(x.Telefono, @"^\+?\d{9,15}$", "El teléfono no tiene un formato válido.").Map(_ => x),
            x => InRange(x.Edad, 18, 120, "La edad debe estar entre 18 y 120 años.").Map(_ => x),
            x => NotEmptyGuid(x.TenantId, "El tenant es obligatorio.").Map(_ => x)));
```

### 8.2. Respuesta HTTP con todos los errores de validación

```csharp
[HttpPost]
public IActionResult Post(AltaClienteDto dto) =>
    Validar(dto)
        .Bind(d => _service.Crear(d))
        .Match(
            valid: c => Created($"/clientes/{c.Id}", c),
            fail:  e => BadRequest(new
            {
                titulo  = "Los datos enviados no son válidos.",
                errores = e.Errors.Select(x => x.Message).ToArray()
            }));
```

### 8.3. Reglas dinámicas construidas en tiempo de ejecución

Como existe la sobrecarga con `IEnumerable`, las reglas pueden venir de configuración:

```csharp
var reglas = new List<Func<Documento, MlResult<Documento>>>();

if (config.ExigirFirma)   reglas.Add(d => That(d, d.Firmado, "El documento debe estar firmado."));
if (config.ExigirSello)   reglas.Add(d => That(d, d.Sellado, "El documento debe estar sellado."));
if (config.MaxPaginas > 0) reglas.Add(d => LessOrEqual(d.Paginas, config.MaxPaginas,
                                                       $"El documento supera las {config.MaxPaginas} páginas."));

var resultado = All(documento, reglas);   // si la lista queda vacía → Valid(documento)
```

### 8.4. Mezcla de estrategias por fases

```csharp
public async Task<MlResult<Pedido>> ProcesarAsync(Pedido pedido) =>
    // Fase 1: reglas baratas de formato, todas a la vez.
    await All(pedido,
              p => NotEmpty(p.Lineas, "El pedido no tiene líneas.").Map(_ => p),
              p => Positive(p.Total,  "El total debe ser positivo.").Map(_ => p),
              p => NotEmptyGuid(p.ClienteId, "Falta el cliente.").Map(_ => p))
        // Fase 2: reglas caras, en cascada y solo si la fase 1 pasó.
        .BindAsync(p => AllOrFirstAsync(p,
              async x => await _clientes.ExisteAsync(x.ClienteId)
                             ? MlResult<Pedido>.Valid(x)
                             : MlResult<Pedido>.Fail("El cliente no existe."),
              async x => await _stock.HayStockAsync(x.Lineas)
                             ? MlResult<Pedido>.Valid(x)
                             : MlResult<Pedido>.Fail("No hay stock suficiente.")));
```

---

## 9. Mejores prácticas

1. **`All` para formularios, `AllOrFirst` para pipelines.** El usuario quiere ver todos sus
   errores; una tubería de proceso quiere fallar rápido.
2. **Ordena `AllOrFirst` de barato a caro.** El objetivo del fail-fast es no pagar lo caro.
3. **No metas I/O en `All`.** Se ejecutan todas las reglas: multiplicarías las consultas.
4. **Recuerda `.Map(_ => d)`** para reproyectar al tipo del contenedor. Si te resulta repetitivo,
   crea helpers privados con nombre (`ReglaNombre`, `ReglaEmail`…).
5. **Un mensaje por regla, claro y accionable.** Con varios errores fusionados, cada mensaje debe
   entenderse por sí solo.
6. **Con una sola regla no uses `All`**: llama directamente a la regla.
7. **Las variantes asíncronas son secuenciales.** No las uses para paralelizar; para eso está
   [`ProjectionParallelAsync`](../Bucle/Bucles.md).
8. **`Any` necesita mensajes que expliquen alternativas**, no que afirmen un único fallo
   («No es un DNI válido» + «No es un NIE válido» se lee bien; «Dato incorrecto» ×3 no).

---

## Ver también

- [Índice de `EnsureFp`](./EnsureFp.md)
- [1. Núcleo: `That`, `TryThat`, `*Arg`](./1_EnsureFpCore.md)
- [8. Variantes asíncronas](./8_EnsureFpAsync.md)
- [`BindMulti`](../Bind/4_BindMulti.md) — acumular errores con tipos distintos
- [`Combine`](../Several/4_Combine.md) — ⚠️ **no** acumula errores
- [`Bucles`](../Bucle/Bucles.md) — validar elemento a elemento
- [Modelo de errores](../Types/MlResultErrors.md) · [`Details`](../Types/MlResultActionsErrorsDetails.md)
