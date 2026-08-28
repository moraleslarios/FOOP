# `MlResultActionsSeveral` (`Types/MlResultActionsSeveral.cs`)

Utilidades de **entrada y composición**: convierten valores del mundo imperativo (`null`, colecciones
vacías, `bool`) en `MlResult<T>`, y combinan varios resultados en uno solo.

Son las operaciones que normalmente están **al principio** de una tubería (para entrar en el mundo
funcional) o **en un cruce** de la misma (para juntar ramas independientes).

---

## Métodos de la clase

| Método | Sobrecargas | Descripción |
| --- | --- | --- |
| `EmptyToFailed` | 3 | Si la colección es `null` o está vacía, devuelve `Fail`; si no, devuelve la colección como válida. |
| `EmptyToFailedAsync` | 4 | Versiones asíncronas. |
| `NullToFailed` | 4 | Si el valor es `null`, devuelve `Fail`; si no, lo envuelve como válido. |
| `NullToFailedAsync` | 8 | Versiones asíncronas. |
| `BoolToResult` | 8 | Convierte una condición booleana en `MlResult<T>`: `true` → válido, `false` → `Fail` con el mensaje indicado. |
| `BoolToResultAsync` | 16 | Versiones asíncronas. |
| `Combine` | 21 | Combina varios `MlResult` independientes en un único resultado con **tupla** de valores; si alguno falla, acumula los errores. |
| `CombineAsync` | 28 | Versiones asíncronas de `Combine`. |
| `Do` | 1 | Ejecuta una función arbitraria dentro de la tubería (punto de extensión genérico). |
| `DoAsync` | 2 | Versión asíncrona de `Do`. |

> ⚠️ **Corrección respecto a versiones anteriores de esta documentación:** los métodos
> `CreateCompleteMlResult` y `FusionErrosIfExists` **no pertenecen a esta clase**.
> - `CreateCompleteMlResult` está en [`MlResultActions`](./MlResultActions.md).
> - `FusionErrosIfExists` y `FusionFailErros` están en [`MlResultBucles`](./MlResultBucles.md).

---

## Ejemplos

### `NullToFailed`: la frontera con código que devuelve `null`

Es el conversor natural cuando integras APIs, ORMs o librerías que señalan "no encontrado" con `null`:

```csharp
Cliente? clienteOrm = _contexto.Clientes.FirstOrDefault(c => c.Id == id);

MlResult<Cliente> cliente = clienteOrm
    .NullToFailed($"No existe ningún cliente con Id {id}");

// A partir de aquí ya estás en la tubería y puedes encadenar con total seguridad.
MlResult<ClienteDto> dto = cliente.Map(c => new ClienteDto(c.Id, c.Nombre));
```

Muy útil justo detrás de una operación asíncrona:

```csharp
MlResult<Cliente> cliente = await _repo.BuscarPorEmailAsync(email)
    .NullToFailedAsync($"No hay ningún cliente registrado con el email {email}");
```

### `EmptyToFailed`: exigir que una colección traiga datos

```csharp
MlResult<IEnumerable<Linea>> lineas = pedido.Lineas
    .EmptyToFailed("El pedido debe contener al menos una línea");

MlResult<decimal> total = lineas.Map(ls => ls.Sum(l => l.Importe));
```

Cubre a la vez los dos casos peligrosos: `null` y colección vacía.

### `BoolToResult`: convertir una regla en resultado

```csharp
// A partir de una condición, obtienes un MlResult con el valor que quieras transportar.
MlResult<Usuario> autorizado = usuario.TienePermiso(Permisos.Facturar)
    .BoolToResult(usuario, "El usuario no tiene permiso para facturar");

// Encadenando varias reglas:
MlResult<Usuario> validado = usuario.EstaActivo BoolToResult(usuario, "Usuario inactivo")
    .Bind(u => u.EmailConfirmado BoolToResult(u, "Email sin confirmar"))
    .Bind(u => (!u.Bloqueado)  .BoolToResult(u, "Usuario bloqueado"));
```

### `Combine`: unir ramas independientes

Cuando necesitas varios datos que se obtienen por separado y **ninguno depende del otro**, `Combine`
los junta en una tupla y **acumula los errores de todos los que fallen**, en lugar de cortar en el
primero:

```csharp
MlResult<Cliente>   cliente   = ObtenerCliente(clienteId);
MlResult<Tarifa>    tarifa    = ObtenerTarifa(tarifaId);
MlResult<Impuestos> impuestos = ObtenerImpuestos(pais);

MlResult<(Cliente cliente, Tarifa tarifa, Impuestos impuestos)> datos =
    cliente.Combine(tarifa, impuestos);

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
    .CombineAsync(tTarifa, tImpuestos)
    .MapAsync(d => Presupuesto.Crear(d.Item1, d.Item2, d.Item3));
```

> Diferencia clave con `Bind`: `Bind` es **secuencial y cortocircuitante** (el paso 2 necesita el
> resultado del paso 1); `Combine` es **paralelizable y acumulativo** (los pasos son independientes).

### `Do`: punto de extensión genérico

`Do` permite intercalar una función propia sin romper la fluidez de la cadena. Úsalo cuando ninguna
de las familias estándar (`Map`, `Bind`, `ExecSelf`) encaja de forma natural:

```csharp
MlResult<Informe> informe = ObtenerDatos(rango)
    .Do(datos => _generador.Componer(datos));

MlResult<Informe> informeAsync = await ObtenerDatosAsync(rango)
    .DoAsync(datos => _generador.ComponerAsync(datos));
```

---

## Cuándo usar cada método

| Situación | Método |
| --- | --- |
| Un valor puede ser `null` | `NullToFailed` |
| Una colección puede ser `null` o vacía | `EmptyToFailed` |
| Tienes un `bool` que representa una regla | `BoolToResult` |
| Necesitas varios resultados independientes y todos sus errores | `Combine` |
| Un texto obligatorio, un objeto no nulo, o una condición arbitraria al inicio | [`EnsureFp`](../EnsureFp/EnsureFp.md) |
| Procesar una colección elemento a elemento | [`MlResultBucles`](./MlResultBucles.md) |

---

## Documentación detallada por concepto

- [1. `EmptyToFailed`](../Several/1_EmptyToFailed.md)
- [2. `NullToFailed`](../Several/2_NullToFailed.md)
- [3. `BoolToResult`](../Several/3_BoolToResult.md)
- [4. `Combine`](../Several/4_Combine.md)

## Ver también

- [`MlResultActions`](./MlResultActions.md) — `CreateCompleteMlResult`, `SecureValidValue`, etc.
- [`MlResultBucles`](./MlResultBucles.md) — `Projection*`, `FusionFailErros`, `FusionErrosIfExists`.
- [`MlResultTransformations`](./MlResultTransformations.md) — conversiones `ToMlResult*` / `TryToMlResult*`.
