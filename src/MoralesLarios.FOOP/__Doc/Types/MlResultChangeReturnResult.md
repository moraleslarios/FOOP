# `MlResultChangeReturnResult` (`Types/MlResultChangeReturnResult.cs`)

Familia de extensiones para **cambiar el tipo de retorno** de un `MlResult<T>` sin necesidad de
transformar el valor que transporta.

Es el complemento de `Map` y `Bind` para un caso muy concreto y muy frecuente:

> *"Ya he hecho lo que tenía que hacer con este resultado. Ahora quiero devolver **otra cosa**, y el
> valor anterior ya no me interesa; solo me interesa **su estado** (válido o fallido)."*

```csharp
// ❌ Con Map hay que inventar un lambda que ignora la entrada.
MlResult<Confirmacion> r = guardado.Map(_ => new Confirmacion("OK"));

// ✅ Con ChangeReturnResult la intención queda explícita.
MlResult<Confirmacion> r = guardado.ChangeReturnResult(new Confirmacion("OK"));
```

> ⚠️ **Ojo con la ortografía del código fuente:** las familias "siempre" están escritas como
> **`Alwais`**, no `Always`: `ChangeReturnResultAlwaisValid`, `ChangeReturnResultAlwaisFail`. Se
> documentan tal cual para que los encuentres al escribir código.

---

## Familias de métodos

| Familia | Sobrecargas | Actúa cuando | Resultado |
| --- | --- | --- | --- |
| `ChangeReturnResult` | 4 | Siempre | Conserva el estado; solo cambia el tipo `T` → `TReturn`. |
| `ChangeReturnResultAsync` | 5 | Siempre | Versiones asíncronas. |
| `ChangeReturnResultAlwaisValid` | 1 | Siempre | Devuelve **válido** sea cual sea el estado de origen. |
| `ChangeReturnResultAlwaisValidAsync` | 2 | Siempre | Versiones asíncronas. |
| `ChangeReturnResultAlwaisFail` | 4 | Siempre | Devuelve **fallido** sea cual sea el estado de origen. |
| `ChangeReturnResultAlwaisFailAsync` | 8 | Siempre | Versiones asíncronas. |
| `ChangeReturnResultIfValid` | 1 | Solo si es válido | Cambia el tipo; si era fallido, propaga el fallo. |
| `ChangeReturnResultIfValidAsync` | 2 | Solo si es válido | Versiones asíncronas. |
| `ChangeReturnResultIfValidToFail` | 4 | Solo si es válido | **Convierte un éxito en fallo** (reglas de negocio invertidas). |
| `ChangeReturnResultIfValidToFailAsync` | 8 | Solo si es válido | Versiones asíncronas. |
| `ChangeReturnResultIfFailToValid` | 1 | Solo si es fallido | **Recupera** de un fallo devolviendo un valor válido. |
| `ChangeReturnResultIfFailToValidAsync` | 2 | Solo si es fallido | Versiones asíncronas. |

---

## `ChangeReturnResult`: cambiar el tipo conservando el estado

El caso base. El nuevo valor **solo se usa si el origen era válido**; si era fallido, los errores se
propagan intactos.

```csharp
MlResult<int> filasAfectadas = _repo.Actualizar(cliente);

// No nos interesa el número de filas, sino confirmar la operación.
MlResult<string> mensaje = filasAfectadas
    .ChangeReturnResult("Cliente actualizado correctamente");

// Si Actualizar falló, `mensaje` es Fail con los errores originales.
```

Las 4 sobrecargas te permiten indicar el nuevo valor como valor directo, como `Func<TReturn>`, o como
`MlResult<TReturn>` ya construido:

```csharp
// a) Valor directo.
resultado.ChangeReturnResult(Unit.Value);

// b) Fábrica perezosa: solo se evalúa si el origen es válido.
resultado.ChangeReturnResult(() => new Recibo(DateTime.UtcNow, Guid.NewGuid()));

// c) Otro MlResult ya calculado.
resultado.ChangeReturnResult(otroResultado);
```

Versión asíncrona en una tubería real:

```csharp
public Task<MlResult<AcuseRecibo>> RegistrarAsync(Evento evento)
    => _repo.InsertarAsync(evento)                                   // Task<MlResult<long>>
            .ChangeReturnResultAsync(new AcuseRecibo(evento.Id, "REGISTRADO"));
```

---

## `ChangeReturnResultIfValid`: solo en la rama válida

Semánticamente igual que `ChangeReturnResult`, pero con el nombre que hace explícito que la rama
fallida no se toca. Úsalo cuando la legibilidad importe más que la brevedad:

```csharp
MlResult<HttpStatusCode> estado = await _servicio.PublicarAsync(mensaje)
    .ChangeReturnResultIfValidAsync(HttpStatusCode.Accepted);
```

---

## `ChangeReturnResultIfValidToFail`: invertir la lógica

Convierte un éxito en fallo. Suena raro hasta que aparece el caso de uso: **comprobaciones de
existencia negativas**.

```csharp
// Queremos dar de alta un usuario, y que exista ya es un ERROR.
public MlResult<Usuario> Alta(string email)
    => _repo.BuscarPorEmail(email)                       // MlResult<Usuario>: válido si EXISTE
            .ChangeReturnResultIfValidToFail<Usuario, Usuario>(
                $"Ya existe un usuario con el email {email}")
            // Aquí llegamos solo si NO existía.
            .ChangeReturnResultIfFailToValid(new Usuario(email))
            .Bind(_repo.Insertar);
```

Otro caso típico: reglas de bloqueo.

```csharp
MlResult<Pedido> validado = _repo.BuscarBloqueo(pedidoId)
    .ChangeReturnResultIfValidToFail<Bloqueo, Pedido>(
        b => $"El pedido está bloqueado por {b.Usuario} desde {b.Desde:g}");
```

Las 4 sobrecargas permiten dar el error como `string`, `MlError`, colección de mensajes o
`MlErrorsDetails`, y también como `Func<T, ...>` para incluir datos del valor válido en el mensaje
(como en el ejemplo del bloqueo).

---

## `ChangeReturnResultIfFailToValid`: recuperación

El simétrico exacto: un fallo se convierte en un valor válido. Es la forma más concisa de expresar un
**valor por defecto ante error**.

```csharp
// Si no hay preferencias guardadas, usamos las de fábrica.
MlResult<Preferencias> prefs = _repo.ObtenerPreferencias(usuarioId)
    .ChangeReturnResultIfFailToValid(Preferencias.PorDefecto);
```

| Alternativa | Diferencia |
| --- | --- |
| `ChangeReturnResultIfFailToValid` | Valor fijo o fábrica sin parámetros; ignora los errores. |
| [`MapIfFail`](./MlResultActionsMap.md) | Recibe el `MlErrorsDetails` y puede usarlo para decidir el valor. |
| [`BindIfFail`](./MlResultActionsBind.md) | Igual, pero el reemplazo puede volver a fallar. |

Usa `ChangeReturnResultIfFailToValid` cuando el motivo del fallo **da igual**; usa `MapIfFail` o
`BindIfFail` cuando sí importa.

---

## `ChangeReturnResultAlwaisValid` y `ChangeReturnResultAlwaisFail`

Descartan por completo el estado de origen.

### `AlwaisValid`

```csharp
// Operación "best effort": la limpieza de caché nunca debe romper la petición.
MlResult<Unit> resultado = _cache.Invalidar(clave)
    .ChangeReturnResultAlwaisValid(Unit.Value);
```

### `AlwaisFail`

```csharp
// Un endpoint deshabilitado temporalmente: hagas lo que hagas, falla.
public MlResult<Informe> GenerarInforme(Filtro filtro)
    => Validar(filtro)
           .ChangeReturnResultAlwaisFail<Filtro, Informe>(
               "La generación de informes está deshabilitada por mantenimiento");
```

Es útil también en **pruebas** y en *feature flags*, donde quieres forzar una rama sin tocar la lógica.

> 💡 Si además de cambiar el tipo necesitas **ejecutar** algo en ambas ramas, mira
> [`BindAlways` / `MapAlways`](./MlResultActionsMap.md) o
> [la región `MatchAll` de `Match`](./MlResultActionsMatch.md), que evalúan una función en cualquier
> estado.

---

## Tabla de decisión rápida

| Quiero… | Método |
| --- | --- |
| Cambiar el tipo, conservando el estado | `ChangeReturnResult` / `ChangeReturnResultIfValid` |
| Transformar el valor (me interesa el original) | [`Map`](./MlResultActionsMap.md) |
| Encadenar otra operación que puede fallar | [`Bind`](./MlResultActionsBind.md) |
| Que un éxito se convierta en error | `ChangeReturnResultIfValidToFail` |
| Un valor por defecto si falla, sin mirar el error | `ChangeReturnResultIfFailToValid` |
| Un valor por defecto **calculado a partir** del error | [`MapIfFail`](./MlResultActionsMap.md) |
| Ignorar el estado y devolver siempre válido / fallido | `ChangeReturnResultAlwaisValid` / `...AlwaisFail` |
| Salir de `MlResult` con un valor concreto | [`Match`](./MlResultActionsMatch.md) |

---

## Ejemplo completo

```csharp
public async Task<IActionResult> RegistrarUsuarioAsync(RegistroDto dto)
{
    return await ValidarDto(dto)
        // 1) Que el email ya exista es un error de negocio.
        .BindAsync(d => _repo.BuscarPorEmailAsync(d.Email)
                             .ChangeReturnResultIfValidToFailAsync<Usuario, RegistroDto>(
                                 $"El email {d.Email} ya está registrado")
                             .ChangeReturnResultIfFailToValidAsync(d))
        // 2) Alta real.
        .BindAsync(d => _repo.InsertarAsync(new Usuario(d.Email, d.Nombre)))
        // 3) El id numérico no interesa fuera: cambiamos el tipo de retorno.
        .ChangeReturnResultAsync(new RegistroConfirmado(dto.Email, DateTime.UtcNow))
        // 4) El envío del correo de bienvenida no debe tumbar el registro.
        .ExecSelfIfValidAsync(async c => await _correo.EnviarBienvenidaAsync(c.Email))
        .MatchAsync(valid: c       => Created($"/usuarios/{c.Email}", c),
                    fail : errores => BadRequest(new { errores = errores.ToErrorsMessages() }));
}
```

---

## Ver también

- [`MlResultActionsMap`](./MlResultActionsMap.md) — transformar el valor.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — encadenar operaciones que pueden fallar.
- [`MlResultActionsMatch`](./MlResultActionsMatch.md) — salir del `MlResult`.
- [`MlResultTransformations`](./MlResultTransformations.md) — entrar en el `MlResult`.
