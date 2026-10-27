# `MlResultSeveralExtensions` (`Types/MlResultSeveralExtensions.cs`)

Utilidades de **entrada y composición**: convierten valores del mundo imperativo (`null`, colecciones
vacías, `bool`) en `MlResult<T>`, y combinan varios resultados en uno solo.

Son las operaciones que normalmente están **al principio** de una tubería (para entrar en el mundo
funcional) o **en un cruce** de la misma (para juntar ramas independientes).

---

## Métodos de la clase

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `FailIfEmpty` | 3 | Si la colección es `null` o está vacía, devuelve `Fail`; si no, devuelve la colección como válida. |
| `FailIfEmptyAsync` | 4 | Versiones asíncronas. |
| `FailIfNull` | 4 | Si el valor es `null`, devuelve `Fail`; si no, lo envuelve como válido. |
| `FailIfNullAsync` | 8 | Versiones asíncronas. |
| `FailIfFalse` | 8 | Convierte una condición booleana en `MlResult<T>`: `true` → válido, `false` → `Fail` con el mensaje indicado. |
| `FailIfFalseAsync` | 16 | Versiones asíncronas. |
| `CombineAll` | 21 | Combina varios `MlResult` independientes en un único resultado con **tupla** de valores; si alguno falla, acumula los errores. |
| `CombineAllAsync` | 28 | Versiones asíncronas de `CombineAll`. |
| `Do` | 1 | Ejecuta una función arbitraria dentro de la tubería (punto de extensión genérico). |
| `DoAsync` | 2 | Versión asíncrona de `Do`. |

> ⚠️ **Corrección respecto a versiones anteriores de esta documentación:** los métodos
> `CreateCompleteMlResult` y `MergeErrorsIfExists` **no pertenecen a esta clase**.
> - `CreateCompleteMlResult` está en [`MlResultExtensions`](MlResultExtensions.md).
> - `MergeErrorsIfExists` y `MergeFailureErrors` están en [`MlResultCollections`](MlResultCollections.md).

---

## Ejemplos

### `FailIfNull`: la frontera con código que devuelve `null`

Es el conversor natural cuando integras APIs, ORMs o librerías que señalan "no encontrado" con `null`:

```csharp
Cliente? clienteOrm = _contexto.Clientes.FirstOrDefault(c => c.Id == id);

MlResult<Cliente> cliente = clienteOrm
    .FailIfNull($"No existe ningún cliente con Id {id}");

// A partir de aquí ya estás en la tubería y puedes encadenar con total seguridad.
MlResult<ClienteDto> dto = cliente.Map(c => new ClienteDto(c.Id, c.Nombre));
```

Muy útil justo detrás de una operación asíncrona:

```csharp
MlResult<Cliente> cliente = await _repo.BuscarPorEmailAsync(email)
    .FailIfNullAsync($"No hay ningún cliente registrado con el email {email}");
```

### `FailIfEmpty`: exigir que una colección traiga datos

```csharp
MlResult<IEnumerable<Linea>> lineas = pedido.Lineas
    .FailIfEmpty("El pedido debe contener al menos una línea");

MlResult<decimal> total = lineas.Map(ls => ls.Sum(l => l.Importe));
```

Cubre a la vez los dos casos peligrosos: `null` y colección vacía.

### `FailIfFalse`: convertir una regla en resultado

```csharp
// A partir de una condición, obtienes un MlResult con el valor que quieras transportar.
MlResult<Usuario> autorizado = usuario.TienePermiso(Permisos.Facturar)
    .FailIfFalse(usuario, "El usuario no tiene permiso para facturar");

// Encadenando varias reglas:
MlResult<Usuario> validado = usuario.EstaActivo FailIfFalse(usuario, "Usuario inactivo")
    .Bind(u => u.EmailConfirmado FailIfFalse(u, "Email sin confirmar"))
    .Bind(u => (!u.Bloqueado)  .FailIfFalse(u, "Usuario bloqueado"));
```

### `CombineAll`: unir ramas independientes

Cuando necesitas varios datos que se obtienen por separado y **ninguno depende del otro**, `CombineAll`
los junta en una tupla y **acumula los errores de todos los que fallen**, en lugar de cortar en el
primero:

```csharp
MlResult<Cliente>   cliente   = ObtenerCliente(clienteId);
MlResult<Tarifa>    tarifa    = ObtenerTarifa(tarifaId);
MlResult<Impuestos> impuestos = ObtenerImpuestos(pais);

MlResult<(Cliente cliente, Tarifa tarifa, Impuestos impuestos)> datos =
    cliente.CombineAll(tarifa, impuestos);

MlResult<Presupuesto> presupuesto = datos
    .Map(d => Presupuesto.Crear(d.cliente, d.tarifa, d.impuestos));

// Si fallan cliente e impuestos, el resultado contiene AMBOS mensajes de error,
// lo que permite mostrar al usuario todos los problemas de una sola vez.
```

Versión asíncrona, resolviendo las tres consultas en paralelo antes de combinar:

```csharp
var tCliente   = ObtenerClienteAsync(clienteId);
var tTarifa    = ObtenerTarifaAsync(tarifaId);
var tImpuestos = ObtenerImpuestosAsync(pais);

MlResult<Presupuesto> presupuesto = await tCliente
    .CombineAllAsync(tTarifa, tImpuestos)
    .MapAsync(d => Presupuesto.Crear(d.Item1, d.Item2, d.Item3));
```

> Diferencia clave con `Bind`: `Bind` es **secuencial y cortocircuitante** (el paso 2 necesita el
> resultado del paso 1); `CombineAll` es **paralelizable y acumulativo** (los pasos son independientes).

### `Do`: punto de extensión genérico

`Do` permite intercalar una función propia sin romper la fluidez de la cadena. Úsalo cuando ninguna
de las familias estándar (`Map`, `Bind`, `ExecSelf`) encaja de forma natural:

```csharp
MlResult<Informe> informe = ObtenerDatos(rango)
    .Pipe(datos => _generador.Componer(datos));

MlResult<Informe> informeAsync = await ObtenerDatosAsync(rango)
    .PipeAsync(datos => _generador.ComponerAsync(datos));
```

---

## Cuándo usar cada método

| Situación | Método |
| --- | --- |
| Un valor puede ser `null` | `FailIfNull` |
| Una colección puede ser `null` o vacía | `FailIfEmpty` |
| Tienes un `bool` que representa una regla | `FailIfFalse` |
| Necesitas varios resultados independientes y todos sus errores | `CombineAll` |
| Un texto obligatorio, un objeto no nulo, o una condición arbitraria al inicio | [`EnsureFp`](../EnsureFp/EnsureFp.md) |
| Procesar una colección elemento a elemento | [`MlResultCollections`](MlResultCollections.md) |

---

## Documentación detallada por concepto

- [1. `FailIfEmpty`](../Several/1_EmptyToFailed.md)
- [2. `FailIfNull`](../Several/2_FailIfNull.md)
- [3. `FailIfFalse`](../Several/3_BoolToResult.md)
- [4. `CombineAll`](../Several/4_Combine.md)

## Ver también

- [`MlResultExtensions`](MlResultExtensions.md) — `CreateCompleteMlResult`, `GetValueOrThrow`, etc.
- [`MlResultCollections`](MlResultCollections.md) — `Traverse*`, `MergeFailureErrors`, `MergeErrorsIfExists`.
- [`MlResultConversions`](MlResultConversions.md) — conversiones `ToMlResult*` / `TryToMlResult*`.
