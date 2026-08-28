# `MlResult` y `MlResult<T>` (`Types/MlResult.cs`)

`MlResult<T>` es el tipo raíz de toda la librería. Representa el resultado de una operación que **puede haber ido bien o mal**, sin usar excepciones como flujo de control:

- **Válido**: transporta un valor de tipo `T`.
- **Fallido**: transporta un [`MlErrorsDetails`](./MlResultErrors.md) con la lista de errores y un diccionario de detalles (excepción original, valor de entrada, metadatos...).

Se declara como `partial record`, por lo que la comparación e impresión son estructurales.

---

## Miembros de instancia

| Miembro | Tipo | Descripción |
|---|---|---|
| `IsValid` | `bool` | `true` si el resultado es válido. Es `{ get; init; }`. |
| `IsFail` | `bool` | Azúcar de `!IsValid`. |
| `Value` | `T` | **`internal protected`**. No accesible desde código consumidor. |
| `ErrorsDetails` | `MlErrorsDetails` | **`internal protected`**. No accesible desde código consumidor. |
| `ToString()` | `string` | Si es válido, `Value?.ToString()` (o `"Not right value"` si es `null`); si es fallido, la descripción de los errores. |

> ⚠️ **`Value` y `ErrorsDetails` son `internal protected` a propósito.** Nunca los leas directamente: usa [`Match`](./MlResultActionsMatch.md) para materializar el resultado, o `SecureValidValue` ([`MlResultActions`](./MlResultActions.md)) si ya has verificado que es válido.

---

## Creación de resultados válidos

```csharp
// Factoría explícita
MlResult<int> a = MlResult<int>.Valid(42);

// Conversión implícita desde el valor (la más habitual)
MlResult<int> b = 42;

// Versión asíncrona: devuelve Task<MlResult<int>> ya completada
Task<MlResult<int>> c = MlResult<int>.ValidAsync(42);

// Helper estático genérico
MlResult<Customer> d = MlResult.Valid(customer);
```

## Creación de resultados fallidos

Existen **10 sobrecargas** de `Fail` (y las mismas en `FailAsync`), pensadas para cubrir desde el caso más simple hasta el fallo con metadatos enriquecidos:

```csharp
// 1) Un mensaje
MlResult<Customer> f1 = MlResult<Customer>.Fail("El cliente no existe");

// 2) Varios mensajes / errores
MlResult<Customer> f2 = MlResult<Customer>.Fail(new MlError("Nombre obligatorio"),
                                                new MlError("Email obligatorio"));

// 3) Una colección de errores
IEnumerable<MlError> errores = new[] { "Nombre obligatorio".ToMlError(), "Email obligatorio".ToMlError() };
MlResult<Customer> f3 = MlResult<Customer>.Fail(errores);

// 4) Un MlErrorsDetails ya construido (errores + Details)
MlResult<Customer> f4 = MlResult<Customer>.Fail(MlErrorsDetails.FromErrorMessage("No encontrado"));

// 5) Mensaje + un detalle suelto (clave/valor)
MlResult<Customer> f5 = MlResult<Customer>.Fail("Cliente no encontrado", "CustomerId", customerId);

// 6) Mensaje + diccionario de detalles
MlResult<Customer> f6 = MlResult<Customer>.Fail("Cliente no encontrado",
                                                new Dictionary<string, object>
                                                {
                                                    ["CustomerId"] = customerId,
                                                    ["Origen"]     = "CustomerRepository"
                                                });

// Versión asíncrona equivalente para cualquiera de ellas
Task<MlResult<Customer>> f7 = MlResult<Customer>.FailAsync("El cliente no existe");
```

### Conversiones implícitas hacia `Fail`

Cualquiera de estos tipos se convierte implícitamente en un `MlResult<T>` fallido, lo que permite escribir `return` directos:

| Origen | Ejemplo |
|---|---|
| `MlError` | `return new MlError("Email inválido");` |
| `MlError[]` | `return new MlError[] { "A", "B" };` |
| `List<MlError>` | `return listaDeErrores;` |
| `MlErrorsDetails` | `return MlErrorsDetails.FromErrorMessage("KO");` |
| Tuplas de error + detalles | `return (errores, detalles);` |

```csharp
public static MlResult<Email> Create(string value)
{
    if (string.IsNullOrWhiteSpace(value)) return new MlError("El email es obligatorio");   // → Fail
    if (!value.Contains('@'))             return new MlError($"Email inválido: {value}");  // → Fail

    return new Email(value);                                                               // → Valid
}
```

---

## Miembros estáticos especiales

| Miembro | Significado |
|---|---|
| `MlResult<T>._` | Instancia "vacía"/descartable de `MlResult<T>`. Útil como valor centinela. |
| `MlResult<T>.Discard` | Alias legible de `_`. Se usa cuando el valor concreto no importa y solo interesa el estado. |
| `MlResult.Empty()` | Devuelve un `MlResult<object>` válido "sin contenido significativo". Ideal para operaciones tipo comando. |
| `MlResult.EmptyAsync()` | Igual que `Empty()` pero como `Task<MlResult<object>>`. |
| `MlResult._` | Equivalente descartable sobre el tipo no genérico. |

```csharp
// Operación de tipo comando: no devuelve datos, solo éxito/fallo
public async Task<MlResult<object>> DeleteAsync(int id)
    => await repository.ExistsAsync(id)
                       .BindAsync(async exists => exists
                            ? await repository.DeleteAsync(id).BindAsync(_ => MlResult.EmptyAsync())
                            : MlResult<object>.Fail($"No existe el registro {id}"));
```

---

## Cómo se consume un `MlResult<T>`

El flujo típico es: **construir → encadenar → materializar**.

```csharp
public async Task<IActionResult> GetCustomer(int id)
    => await EnsureFp.That(id, id > 0, "El identificador debe ser mayor que cero")   // precondición
                     .BindAsync(validId => repository.FindAsync(validId))            // acceso a datos
                     .BindAsync(customer => customer.NullToFailed($"No existe el cliente {id}"))
                     .MapAsync(customer => mapper.Map<CustomerDto>(customer))        // proyección a DTO
                     .ExecSelfIfFailAsync(errors => logger.LogWarning(errors.ToErrorsDescription()))
                     .MatchAsync(                                                    // salida al mundo real
                         valid: dto    => Ok(dto),
                         fail : errors => NotFound(errors.ToErrorsDescription()));
```

Puntos clave del ejemplo:

1. `EnsureFp.That` entra en el raíl validando una precondición.
2. `Bind` encadena operaciones que **también** devuelven `MlResult`.
3. `Map` transforma el valor con una función que **no** devuelve `MlResult`.
4. `ExecSelfIfFail` registra el error **sin alterar** el resultado.
5. `Match` es el único punto donde se abandona el raíl.

---

## Relación con el resto de la librería

| Necesitas... | Usa |
|---|---|
| Encadenar operaciones que devuelven `MlResult` | [`Bind`](./MlResultActionsBind.md) |
| Transformar el valor con una función normal | [`Map`](./MlResultActionsMap.md) |
| Obtener un valor final (HTTP response, DTO, string) | [`Match`](./MlResultActionsMatch.md) |
| Loguear/auditar sin romper la cadena | [`ExecSelf`](./MlResultActionsExecSelf.md) |
| Validar nulos, vacíos, booleanos o combinar resultados | [`Several`](./MlResultActionsSeveral.md) |
| Recorrer colecciones | [`MlResultBucles`](./MlResultBucles.md) |
| Envolver código que lanza excepciones | [`MlResultTransformations`](./MlResultTransformations.md) |
| Leer la excepción o el valor guardado en un fallo | [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) |
