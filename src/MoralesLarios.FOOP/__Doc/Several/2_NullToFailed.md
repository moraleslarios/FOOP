# NullToFailed — Convertir un `null` en un fallo explícito

## Índice

1. [Introducción](#introducción)
2. [El problema que resuelve](#el-problema-que-resuelve)
3. [No es un operador del carril, es una puerta de entrada](#no-es-un-operador-del-carril-es-una-puerta-de-entrada)
4. [Firmas reales e implementación](#firmas-reales-e-implementación)
5. [Las cuatro formas de expresar el error](#las-cuatro-formas-de-expresar-el-error)
6. [⚠️ Particularidades reales del código fuente](#️-particularidades-reales-del-código-fuente)
7. [Variantes asíncronas](#variantes-asíncronas)
8. [`NullToFailed` frente a las alternativas](#nulltofailed-frente-a-las-alternativas)
9. [Tabla de decisión rápida](#tabla-de-decisión-rápida)
10. [Ejemplos Prácticos](#ejemplos-prácticos)
11. [Mejores Prácticas](#mejores-prácticas)
12. [Resumen](#resumen)
13. [Ver también](#ver-también)

---

## Introducción

`NullToFailed` convierte un valor cualquiera en un `MlResult<T>`: **válido** si el valor
no es `null`, **fallido** con el error que indiques si lo es. Es el antídoto contra la
epidemia de `if (x is null) return ...` repartidos por todas las capas.

```csharp
// ❌ Estilo imperativo: la comprobación se repite y se olvida
var cliente = await _repo.ObtenerAsync(id);
if (cliente is null)
    return NotFound($"No existe el cliente {id}");
var tarifa = _tarifas.Buscar(cliente.TarifaId);
if (tarifa is null)
    return Problem("Tarifa no configurada");

// ✅ Con NullToFailed: el null entra en el carril y se encadena
return await _repo.ObtenerAsync(id)
                  .NullToFailedAsync($"No existe el cliente {id}")
                  .BindAsync(c => _tarifas.Buscar(c.TarifaId)
                                          .NullToFailed($"Tarifa {c.TarifaId} no configurada")
                                          .Map(t => (Cliente: c, Tarifa: t)))
                  .MatchAsync(valid: par => Ok(par.ToDto()).ToAsync<IActionResult>(),
                              fail : err => NotFound(err.ToErrorsMessages()).ToAsync<IActionResult>());
```

> ⚠️ **Sobre `MlErrorsDetails`** — solo expone `Errors` y `Details`. **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`, `HasValue` ni `HasException`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailValue<T>()`, `GetDetailException()`, `ToDetailsDescription()`.

---

## El problema que resuelve

Un `null` es información: significa "no lo encontré", "no está configurado", "aún no se
ha calculado". El problema es que **el `null` no lleva consigo el motivo**. Cuando estalla
el `NullReferenceException` tres capas más arriba, ya has perdido el contexto.

`NullToFailed` hace dos cosas a la vez:

1. **Elimina el `null`** del resto de la tubería: a partir de ahí trabajas con un valor
   garantizado.
2. **Adjunta el motivo** en el mismo punto donde se detecta, con el mensaje y los
   detalles que tú decidas.

| Entrada | Salida |
|---------|--------|
| `null` | `MlResult<T>` **Fail** con tu error |
| Cualquier otro valor | `MlResult<T>` **Valid** con ese mismo valor |

---

## No es un operador del carril, es una puerta de entrada

🔑 `NullToFailed` es una extensión de **`T`** (cualquier tipo), no de `MlResult<T>`.
Pertenece a la misma familia de "constructores" que
[`EmptyToFailed`](1_EmptyToFailed.md) y [`BoolToResult`](3_BoolToResult.md).

```csharp
// ✅ Sobre un valor normal
Cliente? c = _repo.Buscar(id);
MlResult<Cliente> resultado = c.NullToFailed("Cliente no encontrado");

// ✅ Sobre el resultado de un método asíncrono
MlResult<Cliente> resultado = await _repo.BuscarAsync(id).NullToFailedAsync("Cliente no encontrado");

// ⚠️ Sobre un MlResult sí compila (T = MlResult<X>), pero NO es lo que quieres:
//    envuelve el resultado en otro resultado
var raro = yaEnCarril.NullToFailed("...");   // MlResult<MlResult<X>>

// ✅ Si ya estás en el carril y quieres comprobar un campo, usa Bind
yaEnCarril.Bind(c => c.Tarifa.NullToFailed("Tarifa no configurada"));
```

---

## Firmas reales e implementación

El método base es el que recibe `MlErrorsDetails`; **todas las demás sobrecargas delegan
en él**, así que el comportamiento es idéntico:

```csharp
// BASE
public static MlResult<T> NullToFailed<T>(this T               source,
                                               MlErrorsDetails errorsDetails)
    => source == null ? errorsDetails.ToMlResultFail<T>() : source.ToMlResultValid();

public static MlResult<T> NullToFailed<T>(this T       source, MlError error)
    => source.NullToFailed(MlErrorsDetails.FromError(error));

public static MlResult<T> NullToFailed<T>(this T source, string errorMessage)
    => source.NullToFailed(MlError.FromErrorMessage(errorMessage));

public static MlResult<T> NullToFailed<T>(this T                   source,
                                               IEnumerable<string> errorsMessage)
    => source.NullToFailed(MlErrorsDetails.FromEnumerableStrings(errorsMessage));
```

Puntos que conviene retener:

| Detalle | Consecuencia práctica |
|---------|----------------------|
| La comprobación es `source == null` (no `is null`) | Para tipos con `operator ==` sobrecargado se usa **ese** operador, no la comparación de referencias |
| El valor válido es `source`, tal cual | No hay copia, transformación ni normalización |
| Todas las sobrecargas delegan en la de `MlErrorsDetails` | Un único punto de verdad; sin divergencias de comportamiento |
| No existe `TryNullToFailed` | No hace falta: el método no invoca ningún delegado tuyo |
| No existe una sobrecarga con `Func<...>` para el mensaje | El mensaje se **evalúa siempre**, aunque el valor no sea `null` (ver particularidades) |

---

## Las cuatro formas de expresar el error

```csharp
// 1) string → el caso más frecuente
var r1 = cliente.NullToFailed($"No existe el cliente {id}");

// 2) MlError → reutilizar un catálogo
var r2 = cliente.NullToFailed(ErroresCliente.NoEncontrado);

// 3) IEnumerable<string> → varios mensajes a la vez
var r3 = configuracion.NullToFailed(new[]
{
    "No se ha encontrado la configuración de facturación",
    "Revise la sección 'Facturacion' de appsettings.json",
    "Contacte con soporte si el problema persiste"
});

// 4) MlErrorsDetails → mensaje + detalles de diagnóstico
var r4 = cliente.NullToFailed(MlErrorsDetails.FromErrorMessageDetails(
             "No existe el cliente solicitado",
             new Dictionary<string, object> { ["ClienteId"]    = id,
                                              ["NoEncontrado"] = true,
                                              ["Capa"]         = "ClienteService" }));
```

La forma 3 es especialmente cómoda para mensajes orientados al usuario final (qué ha
pasado, qué revisar, a quién avisar). La forma 4 es la preferible en servicios, porque los
`Details` viajan con el error y permiten decidir el código HTTP o la política de reintento
al final de la tubería.

---

## ⚠️ Particularidades reales del código fuente

**1. El mensaje se construye siempre, incluso cuando el valor no es `null`.**
No hay sobrecarga con `Func<string>`, así que la interpolación se evalúa antes de la
llamada:

```csharp
// ⚠️ ObtenerDescripcionCostosa() se ejecuta SIEMPRE, aunque cliente no sea null
var r = cliente.NullToFailed($"No existe: {ObtenerDescripcionCostosa(id)}");

// ✅ Si el mensaje es costoso, decide con Match o construye el error aparte
var r = cliente is null
            ? MlResult<Cliente>.Fail($"No existe: {ObtenerDescripcionCostosa(id)}")
            : cliente.ToMlResultValid();
```

En la práctica, con mensajes interpolados normales el coste es irrelevante; el aviso solo
importa si la construcción del mensaje implica consultas o formateos caros.

**2. Se usa `source == null`, no `is null`.**
Si `T` sobrecarga `operator ==` (por ejemplo, un *value object* que compara por valor y
considera "igual a null" un estado vacío), la comprobación puede comportarse de forma
inesperada:

```csharp
public record Codigo(string Valor)
{
    public static bool operator ==(Codigo? a, Codigo? b) => /* lógica personalizada */ ...;
}
// ⚠️ NullToFailed usará esa lógica personalizada, no la comparación de referencias
```

**3. Con tipos de valor no anulables la llamada es inútil.**
`int`, `DateTime`, `decimal`… nunca son `null`, por lo que `NullToFailed` siempre devuelve
`Valid`. Para esos casos usa
[`BoolToResult`](3_BoolToResult.md) o `EnsureFp.That(...)`:

```csharp
// ❌ Nunca falla: un int no puede ser null
var r = cantidad.NullToFailed("La cantidad es obligatoria");

// ✅ Comprueba lo que de verdad te importa
var r = EnsureFp.That(cantidad, cantidad > 0, "La cantidad debe ser positiva");

// ✅ Con Nullable<T> sí tiene sentido, pero recuerda que el resultado es MlResult<int?>
MlResult<int?> r = cantidadOpcional.NullToFailed("La cantidad es obligatoria");
//    …si quieres MlResult<int>, desenvuélvelo después
MlResult<int> valor = r.Map(x => x!.Value);
```

**4. `NullToFailedAsync` sobre un valor síncrono no es realmente asíncrono.**
Las cuatro sobrecargas que reciben `this T source` se limitan a envolver el resultado con
`.ToAsync()` (`Task.FromResult`). Existen para poder encadenar sin romper la cadena
`await`; no aportan paralelismo. Las que reciben `this Task<T> sourceAsync` sí esperan el
origen.

---

## Variantes asíncronas

| Origen | Error como… | Naturaleza |
|--------|-------------|-----------|
| `T` | `MlError` | Envoltura (`ToAsync()`) |
| `T` | `MlErrorsDetails` | Envoltura |
| `T` | `string` | Envoltura |
| `T` | `IEnumerable<string>` | Envoltura |
| `Task<T>` | `MlError` | **Espera el origen** |
| `Task<T>` | `MlErrorsDetails` | **Espera el origen** |
| `Task<T>` | `string` | **Espera el origen** |
| `Task<T>` | `IEnumerable<string>` | **Espera el origen** |

Las cuatro últimas son las verdaderamente útiles: permiten enlazar con un repositorio
asíncrono sin `await` intermedio.

```csharp
// Sin variante asíncrona: hay que romper la expresión
var cliente = await _repo.ObtenerAsync(id);
var resultado = cliente.NullToFailed($"No existe el cliente {id}");

// Con variante asíncrona: una sola expresión encadenable
var resultado = await _repo.ObtenerAsync(id)
                           .NullToFailedAsync($"No existe el cliente {id}")
                           .BindAsync(ValidarAsync)
                           .MapAsync(c => c.ToDto().ToAsync());
```

---

## `NullToFailed` frente a las alternativas

| Herramienta | Cuándo usarla | Diferencia clave |
|-------------|---------------|------------------|
| `NullToFailed` | El dato viene de fuera del carril y puede ser `null` | Constructor: entra al carril |
| `EnsureFp.NotNull(x, "...")` | Validación de argumentos al principio de un método | Método **estático**, no extensión; misma semántica |
| [`EmptyToFailed`](1_EmptyToFailed.md) | El dato es una colección y el vacío también es un problema | Cubre `null` **y** vacío |
| [`BoolToResult`](3_BoolToResult.md) | La condición no es "ser null" | Cualquier predicado |
| [`MapEnsure`](../Map/2_MapEnsure.md) | Ya estás en el carril y quieres validar el valor | Operador del carril, no constructor |
| `??` de C# | Quieres un **valor por defecto**, no un fallo | No genera error; oculta el motivo |

💡 `EnsureFp.NotNull` y `NullToFailed` hacen lo mismo con distinta sintaxis. Convención
recomendada: `EnsureFp.NotNull` para **validar parámetros de entrada** al principio de un
método, y `NullToFailed` para **encadenar** resultados de consultas.

---

## Tabla de decisión rápida

| Necesito… | Uso |
|-----------|-----|
| Fallar si un objeto es `null` | `NullToFailed("...")` |
| Fallar si un objeto es `null`, con varios mensajes | `NullToFailed(new[] { "...", "..." })` |
| Fallar si es `null` y adjuntar diagnóstico | `NullToFailed(MlErrorsDetails.FromErrorMessageDetails(...))` |
| Lo mismo, partiendo de un `Task<T>` | `NullToFailedAsync("...")` |
| Fallar si una colección viene vacía o `null` | [`EmptyToFailed`](1_EmptyToFailed.md) |
| Validar un `int`, `decimal`, `DateTime`… | `EnsureFp.That(x, condición, "...")` |
| Comprobar un campo cuando ya estoy en el carril | `.Bind(c => c.Campo.NullToFailed("..."))` |
| Sustituir el `null` por un valor por defecto (sin fallar) | `valor ?? porDefecto` |

---

## Ejemplos Prácticos

### Ejemplo 1: cadena de búsquedas dependientes

```csharp
public class FacturacionService
{
    private readonly IClienteRepository  _clientes;
    private readonly ITarifaRepository   _tarifas;
    private readonly IDireccionRepository _direcciones;

    public async Task<MlResult<DatosFacturacion>> PrepararAsync(int clienteId)
        => await _clientes.ObtenerAsync(clienteId)
                          .NullToFailedAsync(MlErrorsDetails.FromErrorMessageDetails(
                              "El cliente indicado no existe",
                              new Dictionary<string, object> { ["ClienteId"] = clienteId,
                                                               ["NoEncontrado"] = true,
                                                               ["Capa"]         = "ClienteService" }))
                          .BindAsync(async cliente =>
                          {
                              var tarifa = await _tarifas.ObtenerAsync(cliente.TarifaId);
                              return tarifa.NullToFailed(
                                         $"El cliente {clienteId} apunta a la tarifa {cliente.TarifaId}, que no existe")
                                     .Map(t => (Cliente: cliente, Tarifa: t));
                          })
                          .BindAsync(async par =>
                          {
                              var dir = await _direcciones.FiscalAsync(par.Cliente.Id);
                              return dir.NullToFailed(new[]
                                        {
                                            "El cliente no tiene dirección fiscal registrada",
                                            "Complete la ficha del cliente antes de facturar"
                                        })
                                     .Map(d => new DatosFacturacion(par.Cliente, par.Tarifa, d));
                          });
}
```

Tres `null` posibles, tres mensajes distintos y precisos, y ni un solo `if`. Además, el
primer error incluye `NoEncontrado`, lo que permite responder 404 en lugar de 400.

### Ejemplo 2: configuración obligatoria al arrancar

```csharp
public static MlResult<OpcionesCorreo> LeerOpciones(IConfiguration config)
    => config.GetSection("Correo").Get<OpcionesCorreo>()
             .NullToFailed(new[]
             {
                 "No se ha encontrado la sección 'Correo' en la configuración",
                 "Añada la sección con las claves Host, Puerto y Remitente",
                 "Consulte la documentación de despliegue"
             })
             .MapEnsure(o => !string.IsNullOrWhiteSpace(o.Host), "El host de correo es obligatorio")
             .MapEnsure(o => o.Puerto > 0,                       "El puerto de correo debe ser positivo")
             .MapEnsure(o => !string.IsNullOrWhiteSpace(o.Remitente), "El remitente es obligatorio");
```

Combinación idiomática: `NullToFailed` para entrar al carril y `MapEnsure` para validar
el contenido una vez dentro.

### Ejemplo 3: valor opcional que no debe fallar

```csharp
public async Task<MlResult<PerfilUsuario>> ObtenerPerfilAsync(int usuarioId)
    => await _usuarios.ObtenerAsync(usuarioId)
                      .NullToFailedAsync($"El usuario {usuarioId} no existe")
                      .MapAsync(async u =>
                      {
                          // El avatar es OPCIONAL: aquí NO usamos NullToFailed
                          var avatar = await _avatares.ObtenerAsync(u.Id) ?? Avatar.PorDefecto;

                          // Las preferencias también son opcionales
                          var prefs = await _prefs.ObtenerAsync(u.Id) ?? Preferencias.PorDefecto;

                          return new PerfilUsuario(u, avatar, prefs);
                      });
```

Regla de oro: `NullToFailed` **solo** cuando el `null` impide continuar. Si el `null`
tiene un sustituto razonable, usa `??`.

### Ejemplo 4: comprobar campos anidados dentro del carril

```csharp
public MlResult<Envio> PrepararEnvio(Pedido pedido)
    => pedido.NullToFailed("El pedido es obligatorio")
             .Bind(p => p.Destinatario.NullToFailed("El pedido no tiene destinatario")
                                      .Map(d => (Pedido: p, Destinatario: d)))
             .Bind(par => par.Destinatario.Direccion
                             .NullToFailed(MlErrorsDetails.FromErrorMessageDetails(
                                 "El destinatario no tiene dirección de entrega",
                                 new Dictionary<string, object> { ["PedidoId"]      = par.Pedido.Id,
                                                                  ["DestinatarioId"] = par.Destinatario.Id }))
                             .Map(dir => new Envio(par.Pedido, par.Destinatario, dir)))
             .ExecSelfIfFail(err => _log.LogWarning("Envío no preparado: {Desc}", err.ToErrorsDescription()));
```

### Ejemplo 5: qué no hacer

```csharp
// ❌ NullToFailed sobre un tipo de valor: nunca falla
var r = importe.NullToFailed("El importe es obligatorio");   // importe es decimal

// ✅ Usa una condición real
var r = EnsureFp.That(importe, importe > 0, "El importe debe ser positivo");


// ❌ Mensaje costoso evaluado siempre
var r = cliente.NullToFailed($"No existe: {await DescribirAsync(id)}");

// ✅ Mensaje sencillo + detalles
var r = cliente.NullToFailed(MlErrorsDetails.FromErrorMessageDetails(
            "Cliente no encontrado", new Dictionary<string, object> { ["Id"] = id }));

```

---

## Mejores Prácticas

1. **Aplica `NullToFailed` justo donde aparece el `null`**, no tres capas más arriba: el
   mensaje solo puede ser preciso si conoces el contexto.
2. **Reserva el fallo para los `null` que bloquean el proceso.** Si hay un valor por
   defecto razonable, usa `??`.
3. **No lo uses con tipos de valor no anulables**: nunca fallará. Usa
   `EnsureFp.That` o [`BoolToResult`](3_BoolToResult.md).
4. **Prefiere la sobrecarga de `MlErrorsDetails`** en servicios: los `Details`
   (`NoEncontrado`, identificadores, capa) permiten decidir la respuesta HTTP al final.
5. **Usa la sobrecarga de `IEnumerable<string>`** para mensajes orientados al usuario:
   qué pasó, qué revisar, a quién avisar.
6. **Convención de equipo:** `EnsureFp.NotNull` para validar parámetros de entrada,
   `NullToFailed` para encadenar resultados de consultas.
7. **Usa las sobrecargas de `Task<T>`** para no romper la expresión con `await`
   intermedios.
8. **Combínalo con `MapEnsure`**: `NullToFailed` garantiza que el objeto existe;
   `MapEnsure` valida que su contenido es correcto.
9. **Evita interpolaciones costosas** en el mensaje, ya que se evalúan siempre.
10. **Nunca compruebes el `null` de nuevo dentro de `Map`/`Bind`**: el carril ya lo
    garantiza.

---

## Resumen

- `NullToFailed` transforma cualquier valor en `MlResult<T>`: **Fail** si es `null`,
  **Valid** con el mismo valor en caso contrario.
- Es un **constructor de `MlResult`** (extensión de `T`), no un operador del carril; para
  usarlo dentro de una tubería, encadénalo con `Bind`.
- **4 sobrecargas síncronas** (`MlErrorsDetails`, `MlError`, `string`,
  `IEnumerable<string>`) y **8 asíncronas** (las mismas cuatro sobre `T`, que son meras
  envolturas, y sobre `Task<T>`, que sí esperan el origen).
- El método **base** es el de `MlErrorsDetails`; todos los demás delegan en él.
- **No existe `TryNullToFailed`**: el método no invoca delegados de usuario.
- ⚠️ Usa `source == null` (respeta `operator ==` sobrecargados) y **evalúa el mensaje
  siempre**.
- ⚠️ Con tipos de valor no anulables es una operación inútil: nunca falla.
- Equivalente estático para validar argumentos: `EnsureFp.NotNull(x, "...")`.

---

## Ver también

- [`EmptyToFailed`](1_EmptyToFailed.md) — el equivalente para colecciones
- [`BoolToResult`](3_BoolToResult.md) — construir un `MlResult` desde una condición
- [`Combine`](4_Combine.md) — fusionar varios `MlResult`
- [`EnsureFp`](../EnsureFp/EnsureFp.md) — `NotNull`, `NotEmpty`, `That` y compañía
- [`MapEnsure`](../Map/2_MapEnsure.md) — validar el contenido sin salir del carril
- [`Bind`](../Bind/3_Bind.md) — encadenar operaciones que devuelven `MlResult`
- [`MlResultErrors`](../Types/MlResultErrors.md) — `MlError`, `MlErrorsDetails` y sus fábricas
- [`Transformations`](../Transformations/Transformations.md) — `ToMlResultValid`, `ToMlResultFail`, `ToAsync`