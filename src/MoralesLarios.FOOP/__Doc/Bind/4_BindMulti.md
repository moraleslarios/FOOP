# BindMulti — Acumular errores de varias operaciones

## Índice
1. [Introducción](#introducción)
2. [Cómo funciona realmente](#cómo-funciona-realmente)
3. [Las tres formas de `BindMulti`](#las-tres-formas-de-bindmulti)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [`BindMulti` frente a `Bind` encadenado](#bindmulti-frente-a-bind-encadenado)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Resumen](#resumen)
9. [Ver también](#ver-también)

---

## Introducción

`Bind` encadenado **corta en el primer fallo**. Eso es lo correcto casi siempre… salvo cuando validas
la entrada de un usuario: si un formulario tiene tres campos mal, decírselo de uno en uno es una mala
experiencia.

`BindMulti` resuelve exactamente eso: ejecuta **todas** las funciones que le pasas sobre el mismo
valor, y si alguna falla **fusiona todos los errores** en un único `MlResult` fallido.

```csharp
// ❌ Bind encadenado: el usuario solo se enterará del primer problema.
var r = ValidarNombre(dto)
    .Bind(d => ValidarEmail(d))          // No se ejecuta si el nombre falla
    .Bind(d => ValidarTelefono(d));      // Ni esto

// ✅ BindMulti: el usuario ve TODO lo que está mal de una vez.
var r = dto.ToMlResultValid()
    .BindMulti(d => Registrar(d),        // returnFunc: solo si todas las validaciones pasan
               d => ValidarNombre(d),
               d => ValidarEmail(d),
               d => ValidarTelefono(d));
```

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `Exception`, `HasException`, `AllErrors`,
> `FirstErrorMessage` ni `HasValue`. Usa `ToErrorsMessages()`, `ToErrorsDescription()`,
> `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## Cómo funciona realmente

Esta es la implementación literal de la sobrecarga más simple:

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn>(this   MlResult<T>                  source,
                                                             Func<T, MlResult<TReturn>>   returnFunc,
                                                      params Func<T, MlResult<TReturn>>[] funcs)
    => source.Match(
        fail : errors => errors,
        valid: value  => value.ToMlResultValid()
                              .Map(x => funcs.Select(func => func(value)).ToList())
                              .Bind(resultData => resultData.Any(x => x.IsFail)
                                                     ? resultData.FusionFailErros().SecureFailErrorsDetails()
                                                     : returnFunc(value)));
```

Los tres pasos, en orden:

1. **Si `source` es fallido**, no se ejecuta nada y los errores se propagan tal cual.
2. **Si es válido**, se ejecutan **todas** las funciones de `funcs` sobre el mismo `value`
   (`funcs.Select(func => func(value)).ToList()` — el `ToList()` fuerza la evaluación completa).
3. Si **alguna** falló, se llama a [`FusionFailErros()`](../Types/MlResultBucles.md) para reunir todos
   los errores en uno solo. Si **ninguna** falló, se ejecuta `returnFunc(value)`.

| Estado de entrada | Resultado de `funcs` | ¿Se ejecuta `returnFunc`? | Resultado final |
| --- | --- | :---: | --- |
| Fallido | (no se ejecutan) | No | El fallo original |
| Válido | Todas válidas | **Sí** | Lo que devuelva `returnFunc` |
| Válido | Alguna fallida | No | **Todos** los errores fusionados |

📌 Consecuencia importante: **`funcs` se ejecutan siempre todas**, incluso las posteriores a la que
falla. Si tienen efectos secundarios o son costosas, tenlo en cuenta.

---

## Las tres formas de `BindMulti`

### Forma 1: `returnFunc` recibe solo el valor

La más sencilla. Las funciones de `funcs` actúan como puras validaciones: solo importa si fallan o no,
sus valores se descartan.

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn>(this   MlResult<T>                  source,
                                                             Func<T, MlResult<TReturn>>   returnFunc,
                                                      params Func<T, MlResult<TReturn>>[] funcs)
```

### Forma 2: `returnFunc` recibe el valor **y los resultados** de `funcs`

Cuando sí te interesa lo que produjeron las funciones intermedias:

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn>(this   MlResult<T>                  source,
                                                             Func<T,
                                                                  IEnumerable<TReturn>,
                                                                  MlResult<TReturn>>      returnFunc,
                                                      params Func<T, MlResult<TReturn>>[] funcs)
```

### Forma 3: tipo intermedio distinto (`TFuncColec`)

La más general: las funciones producen un tipo (`TFuncColec`) diferente del resultado final
(`TReturn`).

```csharp
public static MlResult<TReturn> BindMulti<T, TReturn, TFuncColec>(this   MlResult<T>                     source,
                                                                         Func<T,
                                                                              IEnumerable<TFuncColec>,
                                                                              MlResult<TReturn>>         returnFunc,
                                                                  params Func<T, MlResult<TFuncColec>>[] funcs)
```

Internamente hace lo mismo, pero pasa a `returnFunc` únicamente los valores válidos:

```csharp
returnFunc(value, resultsData.Where(x => x.IsValid).Select(y => y.SecureValidValue()))
```

📌 Las formas 1 y 2 son en realidad atajos que delegan en la forma 3 con `TFuncColec = TReturn`.

---

## Variantes asíncronas

`BindMultiAsync` ofrece **18 sobrecargas**, que combinan:

| Eje | Opciones |
| --- | --- |
| Fuente | `MlResult<T>` · `Task<MlResult<T>>` |
| `returnFunc` | síncrono · asíncrono |
| `funcs` | síncronas (`Func<T, MlResult<X>>[]`) · asíncronas (`Func<T, Task<MlResult<X>>>[]`) |

```csharp
public async Task<MlResult<Solicitud>> TramitarAsync(SolicitudDto dto)
    => await dto.ToMlResultValid().ToAsync()
            .BindMultiAsync(
                d => RegistrarAsync(d),                  // returnFunc asíncrono
                d => ValidarTitularAsync(d),             // ↓ funcs asíncronas: se lanzan todas
                d => ValidarDomicilioAsync(d),
                d => ValidarSolvenciaAsync(d));
```

---

## `BindMulti` frente a `Bind` encadenado

| Aspecto | `Bind` encadenado | `BindMulti` |
| --- | --- | --- |
| Al primer fallo | **Corta** (cortocircuito) | Sigue ejecutando el resto |
| Errores devueltos | Solo el primero | **Todos**, fusionados |
| Coste si algo falla | Mínimo | Se pagan todas las funciones |
| Caso de uso típico | Flujo de negocio secuencial | Validación de un formulario o DTO |
| Dependencias entre pasos | Sí (cada paso usa el anterior) | **No** (todas parten del mismo valor) |

🔑 **Regla:** si los pasos **dependen** unos de otros, `Bind`. Si son **independientes** y quieres
reportarlos todos, `BindMulti`.

---

## Ejemplos Prácticos

### Ejemplo 1: Validación completa de un alta de cliente

```csharp
public class ServicioClientes
{
    public MlResult<Cliente> Registrar(ClienteDto dto)
        => dto.ToMlResultValid()
            .BindMulti(
                // Solo se ejecuta si las cuatro validaciones pasan.
                d => Persistir(d),

                // Se ejecutan TODAS: el usuario verá todos los errores a la vez.
                d => EnsureFp.NotNullEmptyOrWhitespace(d.Nombre, "El nombre es obligatorio")
                             .Map(_ => (Cliente)null!),
                d => ValidarEmail(d.Email),
                d => ValidarNif(d.Nif),
                d => ValidarEdad(d.FechaNacimiento));

    private static MlResult<Cliente> ValidarEmail(string email)
        => email.Contains('@')
            ? MlResult<Cliente>.Valid(null!)
            : $"El email '{email}' no tiene un formato válido";

    private static MlResult<Cliente> ValidarNif(string nif)
        => Nif.EsValido(nif)
            ? MlResult<Cliente>.Valid(null!)
            : $"El NIF '{nif}' no es correcto";

    private static MlResult<Cliente> ValidarEdad(DateOnly nacimiento)
        => nacimiento.AddYears(18) <= DateOnly.FromDateTime(DateTime.Today)
            ? MlResult<Cliente>.Valid(null!)
            : "El titular debe ser mayor de edad";

    private MlResult<Cliente> Persistir(ClienteDto dto) => _repo.Guardar(Cliente.Desde(dto));
}
```

En el controlador, el resultado se traduce a un `400` con la lista completa de problemas:

```csharp
[HttpPost("clientes")]
public IActionResult Crear(ClienteDto dto)
    => _servicio.Registrar(dto)
        .Match<Cliente, IActionResult>(
            valid: c       => CreatedAtAction(nameof(Obtener), new { id = c.Id }, c),
            fail:  errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
```

Respuesta típica con tres campos mal:

```json
{
  "errores": [
    "El email 'pepe#correo' no tiene un formato válido",
    "El NIF 'X1234' no es correcto",
    "El titular debe ser mayor de edad"
  ]
}
```

### Ejemplo 2: Aprovechar los resultados intermedios (forma 2)

Aquí las funciones **sí producen valor**, y `returnFunc` los recibe para componer el resultado final:

```csharp
public MlResult<InformeRiesgo> EvaluarRiesgo(Solicitud solicitud)
    => solicitud.ToMlResultValid()
        .BindMulti<Solicitud, Puntuacion>(
            // returnFunc recibe la solicitud Y las puntuaciones calculadas.
            (s, puntuaciones) => new Puntuacion("Global", puntuaciones.Sum(p => p.Valor) / 3),

            s => ConsultarScoringInterno(s),
            s => ConsultarCirbe(s),
            s => ConsultarHistorialImpagos(s))
        .Map(global => new InformeRiesgo(solicitud.Id, global));
```

Si alguna de las tres consultas falla, obtienes **todos** los motivos y no se calcula la media.

### Ejemplo 3: Comprobaciones previas asíncronas antes de una operación crítica

```csharp
public async Task<MlResult<Reserva>> ReservarAsync(ReservaDto dto)
    => await dto.ToMlResultValid().ToAsync()
            .BindMultiAsync(
                d => ConfirmarReservaAsync(d),

                d => ComprobarDisponibilidadAsync(d.SalaId, d.Franja),
                d => ComprobarAforoAsync(d.SalaId, d.Asistentes),
                d => ComprobarPermisosAsync(d.UsuarioId, d.SalaId))

            .AddMlErrorDetailIfFailAsync($"[Reservas] Sala {dto.SalaId}, franja {dto.Franja}")

            .ExecSelfIfFailAsync(errores =>
            {
                _log.LogWarning("Reserva rechazada por {N} motivo(s):\n{Detalle}",
                                errores.Errors.Count(), errores.ToErrorsDescription());
                return Task.CompletedTask;
            });
```

Un único log contiene **todos** los motivos del rechazo, lo que ahorra idas y venidas al soporte.

### Ejemplo 4: Cuándo **no** usar `BindMulti`

```csharp
// ❌ MAL: los pasos dependen unos de otros. Ejecutar Facturar sin haber
//    validado el stock es un error, y BindMulti ejecuta todo siempre.
pedido.ToMlResultValid()
    .BindMulti(p => Enviar(p),
               p => ReservarStock(p),      // Se ejecuta aunque el pago falle
               p => CobrarPago(p));        // Se ejecuta aunque el stock falle

// ✅ BIEN: dependencia secuencial → Bind encadenado, con cortocircuito.
pedido.ToMlResultValid()
    .Bind(p => ReservarStock(p))
    .Bind(p => CobrarPago(p))
    .Bind(p => Enviar(p));
```

---

## Mejores Prácticas

### 1. Solo para comprobaciones independientes y sin efectos secundarios

`BindMulti` ejecuta **todas** las funciones. Si alguna escribe en base de datos, cobra o envía un
correo, no la pongas ahí: usa `Bind` encadenado.

### 2. Ideal en la frontera de entrada

Valida el DTO completo con `BindMulti` en cuanto entra, devuelve todos los errores al cliente, y de ahí
hacia dentro usa `Bind` para el flujo de negocio.

### 3. Cuidado con el coste

Si las comprobaciones son consultas remotas caras, valora si el beneficio de reportarlas todas
compensa. Las variantes asíncronas ayudan, pero recuerda que se ejecutan todas sin excepción.

### 4. Usa la forma 3 cuando los tipos no coinciden

Forzar `MlResult<Cliente>.Valid(null!)` en un validador que en realidad no produce un cliente es un
olor a código. Con `BindMulti<T, TReturn, TFuncColec>` puedes usar el tipo que de verdad corresponda,
por ejemplo `MlResult<Unit>` o `MlResult<string>`.

### 5. Añade contexto al conjunto, no a cada función

Un solo `AddMlErrorDetailIfFail` después del `BindMulti` etiqueta el bloque entero de validaciones sin
ensuciar cada validador.

---

## Resumen

- `BindMulti` ejecuta **todas** las funciones que le pasas sobre el mismo valor y **acumula** sus
  errores con `FusionFailErros()`.
- `returnFunc` solo se ejecuta **si ninguna** de las funciones falló.
- Si `source` ya venía fallido, no se ejecuta absolutamente nada.
- Tres formas: `returnFunc` con solo el valor, con el valor **y** los resultados, o con un tipo
  intermedio distinto (`TFuncColec`).
- 3 sobrecargas síncronas y **18** asíncronas (`BindMultiAsync`).
- **Pasos dependientes → `Bind`. Comprobaciones independientes que quieres reportar todas →
  `BindMulti`.**

## Ver también

- [`3_Bind.md`](./3_Bind.md) — el encadenamiento con cortocircuito.
- [`5_BindIf.md`](./5_BindIf.md) — ejecución condicional de un único paso.
- [`../Types/MlResultBucles.md`](../Types/MlResultBucles.md) — `FusionFailErros` y `FusionErrosIfExists`, la fusión de errores en detalle.
- [`../Several/4_Combine.md`](../Several/4_Combine.md) — combinar varios resultados en una tupla.
- [`../Types/MlResultActionsBind.md`](../Types/MlResultActionsBind.md) — referencia con todas las sobrecargas.
- [`../EnsureFp/EnsureFp.md`](../EnsureFp/EnsureFp.md) — construir validaciones concisas.
- [`../Match/1_Match.md`](../Match/1_Match.md) — traducir el resultado acumulado a una respuesta HTTP.