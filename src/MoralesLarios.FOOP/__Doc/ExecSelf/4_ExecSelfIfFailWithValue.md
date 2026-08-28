# ExecSelfIfFailWithValue — Efectos secundarios con el valor que provocó el fallo

## Índice
1. [Introducción](#introducción)
2. [El requisito previo: `AddValueIfFail`](#el-requisito-previo-addvalueiffail)
3. [Firmas reales](#firmas-reales)
4. [Variantes asíncronas](#variantes-asíncronas)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con las demás variantes](#comparación-con-las-demás-variantes)
8. [Resumen](#resumen)
9. [Ver también](#ver-también)

---

## Introducción

Cuando una operación falla, muchas veces el mensaje de error no basta: quieres saber **con qué datos
de entrada** falló. `ExecSelfIfFailWithValue` resuelve exactamente eso: ejecuta una acción solo si el
resultado es fallido **y además hay un valor guardado en los detalles del error**, pasándote ese
valor ya tipado.

```csharp
resultado.ExecSelfIfFailWithValue<Resultado, LineaCsv>((errores, linea) =>
    _log.LogWarning("La línea {N} no se pudo importar: {E}",
                    linea.Numero, errores.ToErrorsDescription()));
```

El valor viaja en `MlErrorsDetails.Details` bajo la clave convencional `"Value"`
(`Constants.VALUE_KEY`). Si esa clave **no** existe o el tipo no coincide, la acción **no se
ejecuta** y el resultado se propaga igual: no hay excepciones ni sorpresas.

> ⚠️ **Nota sobre `MlErrorsDetails`.** Solo expone `Errors` (`IEnumerable<MlError>`) y `Details`
> (`Dictionary<string, object>`). **No existen** `AllErrors`, `FirstErrorMessage`, `Exception`,
> `HasValue` ni `HasException`. Consulta los errores con `ToErrorsMessages()`,
> `ToErrorsDescription()`, `Errors.First().Message`, `GetDetailException()` o `GetDetailValue<T>()`.

---

## El requisito previo: `AddValueIfFail`

`ExecSelfIfFailWithValue` **no adivina** el valor: alguien tiene que haberlo guardado antes. La
forma habitual es `AddValueIfFail`, de `MlResultActionsErrorsDetails`:

```csharp
public MlResult<Registro> ImportarLinea(LineaCsv linea)
    => ValidarLinea(linea)
        .Bind(l => Transformar(l))
        .Bind(r => Guardar(r))

        // Si algo de lo anterior falló, adjuntamos la línea original al error.
        .AddValueIfFail(linea)

        // Y ahora ya podemos usarla en el efecto secundario.
        .ExecSelfIfFailWithValue<Registro, LineaCsv>((errores, l) =>
            _rechazos.Add(new Rechazo(l.Numero, l.Texto, errores.ToErrorsDescription())));
```

**Cadena completa del mecanismo:**

| Paso | Método | Qué hace |
| --- | --- | --- |
| 1 | `AddValueIfFail(valor)` | Guarda `valor` en `Details["Value"]` si el resultado es fallido |
| 2 | `ExecSelfIfFailWithValue<T, TValue>(action)` | Ejecuta la acción con ese valor recuperado y tipado |
| — | `GetDetailValue<TValue>()` | Alternativa manual: devuelve `MlResult<TValue>` |

Otras formas de que la clave `"Value"` acabe rellena:

- `MlResult.Fail<T>(mensaje, valor)` y `MlErrorsDetails.FromErrorMessageWithValue(mensaje, valor)`.
- `AddValueDetailIfFail` y `CompleteWithDataValueIfValid` de `MlResultActions`.
- Las familias `BindIfFailWithValue` y `MapIfFailWithValue`, que ya asumen ese contrato.

---

## Firmas reales

```csharp
// Síncrono
public static MlResult<T> ExecSelfIfFailWithValue<T, TValue>(
        this MlResult<T>                 source,
        Action<MlErrorsDetails, TValue>  actionFailWithValue)

// Con captura de excepciones
public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(
        this MlResult<T>                 source,
        Action<MlErrorsDetails, TValue>  actionFailWithValue,
        Func<Exception, string>          errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithValue<T, TValue>(
        this MlResult<T>                 source,
        Action<MlErrorsDetails, TValue>  actionFailWithValue,
        string                           exceptionAditionalMessage = null!)
```

Fíjate en que hay **dos** parámetros genéricos:

- `T` — el tipo del `MlResult` de la tubería.
- `TValue` — el tipo del valor adjunto al error.

Normalmente hay que indicarlos de forma explícita, porque `TValue` no se puede inferir del
argumento:

```csharp
// ✅ Explícito
.ExecSelfIfFailWithValue<Registro, LineaCsv>((errores, linea) => ...)

// ⚠️ Con lambda tipada también compila, pero es menos legible
.ExecSelfIfFailWithValue((MlErrorsDetails errores, LineaCsv linea) => ...)
```

**Comportamiento**:

| Estado de `source` | `Details["Value"]` | ¿Se ejecuta la acción? | Resultado |
| --- | --- | :---: | --- |
| Válido | — | No | El mismo, válido |
| Fallido | Existe y es `TValue` | Sí | El mismo, fallido |
| Fallido | No existe | No | El mismo, fallido |
| Fallido | Existe pero de otro tipo | No | El mismo, fallido |

---

## Variantes asíncronas

Las cuatro combinaciones de fuente y delegado:

| Fuente | Delegado | Método |
| --- | --- | --- |
| `MlResult<T>` | `Action<MlErrorsDetails, TValue>` | `ExecSelfIfFailWithValue` |
| `MlResult<T>` | `Func<MlErrorsDetails, TValue, Task>` | `ExecSelfIfFailWithValueAsync` |
| `Task<MlResult<T>>` | `Action<MlErrorsDetails, TValue>` | `ExecSelfIfFailWithValueAsync` |
| `Task<MlResult<T>>` | `Func<MlErrorsDetails, TValue, Task>` | `ExecSelfIfFailWithValueAsync` |

Y sus equivalentes seguros `TryExecSelfIfFailWithValueAsync` (8 sobrecargas: 4 combinaciones × 2
formas de mensaje de error).

```csharp
await ProcesarPagoAsync(peticion)
    .AddValueIfFailAsync(peticion)
    .ExecSelfIfFailWithValueAsync<Recibo, PeticionPago>(async (errores, p) =>
        await _colaReintentos.EncolarAsync(p, errores.ToErrorsDescription()));
```

---

## Ejemplos Prácticos

### Ejemplo 1: Importación CSV con informe de rechazos

```csharp
public class ImportadorCsv
{
    private readonly List<LineaRechazada> _rechazos = new();

    public MlResult<ResumenImportacion> Importar(IEnumerable<LineaCsv> lineas)
    {
        var correctos = new List<Registro>();

        foreach (var linea in lineas)
        {
            ProcesarLinea(linea)
                .ExecSelfIfValid(r => correctos.Add(r))
                .ExecSelfIfFailWithValue<Registro, LineaCsv>((errores, l) =>
                    _rechazos.Add(new LineaRechazada(
                        Numero  : l.Numero,
                        Original: l.Texto,
                        Motivos : errores.ToErrorsMessages().ToList())));
        }

        return new ResumenImportacion(correctos.Count, _rechazos.Count, _rechazos);
    }

    private MlResult<Registro> ProcesarLinea(LineaCsv linea)
        => ValidarFormato(linea)
            .Bind(l => Convertir(l))
            .Bind(r => Persistir(r))
            .AddValueIfFail(linea);          // ← imprescindible para el paso siguiente
}

public record LineaCsv(int Numero, string Texto);
public record LineaRechazada(int Numero, string Original, List<string> Motivos);
public record ResumenImportacion(int Correctos, int Rechazados, IReadOnlyList<LineaRechazada> Detalle);
```

Ninguna línea mala interrumpe el bucle y el informe final sabe **qué** línea falló y **por qué**.

> 💡 Si el bucle lo estás escribiendo a mano, mira `ProjectionSplit` en
> [`../Types/MlResultBucles.md`](../Types/MlResultBucles.md): separa correctos y fallidos por ti.

### Ejemplo 2: Cola de reintentos con la petición original

```csharp
public Task<MlResult<Recibo>> CobrarAsync(PeticionCobro peticion)
    => ValidarAsync(peticion)
        .BindAsync(p => AutorizarAsync(p))
        .BindAsync(a => CapturarAsync(a))

        // Guardamos la petición completa para poder reintentarla tal cual.
        .AddValueIfFailAsync(peticion)

        .ExecSelfIfFailWithValueAsync<Recibo, PeticionCobro>(async (errores, p) =>
        {
            // Solo reintentamos los fallos técnicos, no los rechazos del banco.
            var esTecnico = errores.GetDetailException()
                                   .Match(valid: _ => true, fail: _ => false);

            if (esTecnico)
                await _colaReintentos.EncolarAsync(p, TimeSpan.FromMinutes(5));
            else
                _log.LogInformation("Cobro rechazado para {Ref}: {E}",
                                    p.Referencia, errores.ToErrorsDescription());
        });
```

### Ejemplo 3: Auditoría de intentos fallidos (efecto que puede lanzar)

```csharp
public Task<MlResult<Usuario>> AutenticarAsync(Credenciales credenciales)
    => BuscarUsuarioAsync(credenciales.Email)
        .BindAsync(u => ComprobarPasswordAsync(u, credenciales.Password))
        .BindAsync(u => ComprobarBloqueoAsync(u))

        // Nunca guardamos la contraseña: solo el email y el origen.
        .AddValueIfFailAsync(new IntentoAcceso(credenciales.Email, credenciales.Ip))

        // Escribir en la BD de seguridad puede fallar y queremos enterarnos.
        .TryExecSelfIfFailWithValueAsync<Usuario, IntentoAcceso>(
            async (errores, intento) =>
            {
                await _seguridad.RegistrarIntentoFallidoAsync(intento.Email, intento.Ip);

                var fallidos = await _seguridad.ContarFallidosRecientesAsync(intento.Email);
                if (fallidos >= 5)
                    await _seguridad.BloquearAsync(intento.Email, TimeSpan.FromMinutes(15));
            },
            ex => $"No se pudo registrar el intento de acceso fallido: {ex.Message}");

public record IntentoAcceso(string Email, string Ip);
```

### Ejemplo 4: Alternativa manual con `GetDetailValue<T>`

Si solo necesitas el valor en un punto concreto, puedes leerlo directamente:

```csharp
resultado.ExecSelfIfFail(errores =>
{
    var descripcion = errores.GetDetailValue<LineaCsv>()
        .Match(valid: l  => $"línea {l.Numero}: «{l.Texto}»",
               fail:  _  => "(sin línea asociada)");

    _log.LogWarning("Importación fallida en {Contexto}: {E}",
                    descripcion, errores.ToErrorsDescription());
});
```

| Enfoque | Cuándo usarlo |
| --- | --- |
| `ExecSelfIfFailWithValue<T, TValue>` | El efecto **solo tiene sentido** si hay valor adjunto |
| `ExecSelfIfFail` + `GetDetailValue<T>()` | El efecto se ejecuta siempre y el valor es **opcional** |

---

## Mejores Prácticas

### 1. `AddValueIfFail` antes, `ExecSelfIfFailWithValue` después

Sin el primero, el segundo nunca se ejecuta y el silencio es total (no hay error, simplemente no
pasa nada). Es el fallo más habitual con esta familia.

```csharp
// ❌ La acción nunca se ejecuta: nadie guardó el valor.
Procesar(linea).ExecSelfIfFailWithValue<Registro, LineaCsv>((e, l) => ...);

// ✅
Procesar(linea).AddValueIfFail(linea).ExecSelfIfFailWithValue<Registro, LineaCsv>((e, l) => ...);
```

### 2. Adjunta el dato mínimo necesario

`Details` viaja con el error por toda la tubería. No metas entidades enormes ni grafos completos, y
**nunca** datos sensibles (contraseñas, tarjetas, tokens).

### 3. Respeta el tipo exacto

La recuperación es por tipo. Si guardas `LineaCsv` y pides `object` o una clase base, la acción no se
ejecuta. Mantén el mismo tipo en `AddValueIfFail` y en `TValue`.

### 4. Sigue sin recuperar el resultado

Como el resto de `ExecSelf*`, esto **observa**. Si quieres reconstruir un valor a partir del dato
adjunto, usa `MapIfFailWithValue` o `BindIfFailWithValue`.

---

## Comparación con las demás variantes

| Método | Se ejecuta si… | El delegado recibe | ¿Cambia el resultado? |
| --- | --- | --- | :---: |
| `ExecSelfIfFail` | Es fallido | `MlErrorsDetails` | No |
| **`ExecSelfIfFailWithValue`** | **Fallido y hay `Details["Value"]` de tipo `TValue`** | **`MlErrorsDetails`, `TValue`** | **No** |
| `ExecSelfIfFailWithException` | Fallido y hay `Details["Ex"]` | `MlErrorsDetails`, `Exception` | No |
| `ExecSelfIfFailWithoutException` | Fallido y **sin** excepción | `MlErrorsDetails` | No |
| `MapIfFailWithValue` | Fallido y hay valor adjunto | `MlErrorsDetails`, `TValue` | **Sí**: devuelve un valor |
| `BindIfFailWithValue` | Fallido y hay valor adjunto | `MlErrorsDetails`, `TValue` | **Sí**: devuelve otro `MlResult` |

---

## Resumen

- `ExecSelfIfFailWithValue` ejecuta una acción solo si el resultado es fallido **y** hay un valor del
  tipo esperado en `Details["Value"]`.
- Ese valor lo pone normalmente `AddValueIfFail`, y sin él la acción se omite en silencio.
- La clave convencional es `"Value"` (`Constants.VALUE_KEY`).
- Requiere **dos** genéricos explícitos: `<T, TValue>`.
- Existen las cuatro combinaciones asíncronas y las variantes `Try*`, que convierten en fallo las
  excepciones del delegado.
- No recupera el resultado: para eso están `MapIfFailWithValue` y `BindIfFailWithValue`.

## Ver también

- [`1_ExecSelf.md`](./1_ExecSelf.md) — visión general de la familia.
- [`2_ExecSelfIfValid.md`](./2_ExecSelfIfValid.md) — efectos en la rama válida.
- [`3_ExecSelfIfFail.md`](./3_ExecSelfIfFail.md) — efectos en la rama fallida.
- [`5_ExecSelfIfFailWithException.md`](./5_ExecSelfIfFailWithException.md) — el equivalente con la excepción.
- [`6_ExecSelfIfFailWithoutException.md`](./6_ExecSelfIfFailWithoutException.md) — solo fallos de negocio.
- [`../Types/MlResultActionsErrorsDetails.md`](../Types/MlResultActionsErrorsDetails.md) — `AddValueIfFail`, `GetDetailValue<T>`, `GetDetail<T>`.
- [`../Types/MlResultActionsExecSelf.md`](../Types/MlResultActionsExecSelf.md) — referencia y recuento de sobrecargas.
- [`../Bind/7_BindIfFailWithValue.md`](../Bind/7_BindIfFailWithValue.md) y [`../Map/5_MapIfFailWithValue.md`](../Map/5_MapIfFailWithValue.md) — cuando sí quieres recuperar.
- [`../Types/MlResultBucles.md`](../Types/MlResultBucles.md) — `ProjectionSplit` para separar correctos y fallidos.