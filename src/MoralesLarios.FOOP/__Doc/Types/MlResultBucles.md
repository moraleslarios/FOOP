# `MlResultBucles` (`Types/MlResultBucles.cs`)

Operaciones funcionales sobre colecciones con semántica `MlResult`. Permiten transformar `IEnumerable<T>` en `MlResult<IEnumerable<TResult>>`, aplicando una función por elemento y centralizando la gestión de errores.

## Qué aporta

- Proyecciones (`Projection*`) con política **todo o nada**: si cualquier elemento falla, el resultado global falla.
- Sobrecargas con índice (`Func<T, int, ...>`) para generar mensajes de error contextualizados, numerar filas o validar posiciones.
- Variante *while* (`ProjectionWhile*`) para detener el procesamiento en el primer error.
- Versiones asíncronas secuenciales (`ProjectionAsync*`) para fuentes y transformaciones síncronas o asíncronas.
- Versiones paralelas (`ProjectionParallelAsync*`) para lanzar todas las transformaciones asíncronas con `Task.WhenAll`.
- Métodos de fusión (`FusionFailErros*`, `FusionErrosIfExists*`) para combinar errores de varios `MlResult<T>`.

## Familias de métodos

| Familia | Procesamiento | Transformación | Índice | Comportamiento ante errores |
|---|---:|---|---:|---|
| `Projection` | Secuencial síncrono | `Func<T, MlResult<TResult>>` | No | Procesa todos los elementos y fusiona todos los errores. |
| `Projection` | Secuencial síncrono | `Func<T, int, MlResult<TResult>>` | Sí | Procesa todos los elementos y fusiona todos los errores. |
| `ProjectionAsync` | Secuencial asíncrono | Fuente `IEnumerable<T>` o `Task<IEnumerable<T>>`; transformación síncrona o asíncrona | Sí/No | Procesa todos los elementos y fusiona todos los errores. |
| `ProjectionWhile` | Secuencial síncrono | `Func<T, MlResult<TResult>>` o `Func<T, int, MlResult<TResult>>` | Sí/No | Se detiene en el primer error. |
| `ProjectionWhileAsync` | Secuencial asíncrono | Fuente `IEnumerable<T>` o `Task<IEnumerable<T>>`; transformación síncrona o asíncrona | Sí/No | Se detiene en el primer error. |
| `ProjectionParallelAsync` | Paralelo asíncrono | `Func<T, Task<MlResult<TResult>>>` o `Func<T, int, Task<MlResult<TResult>>>` | Sí/No | Lanza todas las tareas y fusiona todos los errores. No es *fail-fast*. |
| `ProjectionSplit` | Secuencial síncrono | `Func<T, MlResult<TResult>>` | No | **No falla nunca**: separa aciertos y fallos en dos colecciones. |
| `ProjectionSplitAsync` | Secuencial asíncrono | Fuente `IEnumerable<T>` o `Task<IEnumerable<T>>`; transformación síncrona o asíncrona | No | **No falla nunca**: separa aciertos y fallos en dos colecciones. |

## `Projection`

`Projection` aplica una transformación a todos los elementos de una colección. Si todas las transformaciones devuelven `MlResult<TResult>.Valid(...)`, devuelve `MlResult<IEnumerable<TResult>>.Valid(...)`. Si una o varias transformaciones fallan, devuelve un `Fail` con los errores fusionados.

```csharp
MlResult<IEnumerable<ProductDto>> result = products.Projection(product =>
	product.Price > 0
		? MlResult<ProductDto>.Valid(new ProductDto(product.Id, product.Name, product.Price))
		: MlResult<ProductDto>.Fail($"El producto {product.Id} no tiene precio válido"));
```

### Sobrecarga con índice

La sobrecarga `Func<T, int, MlResult<TResult>>` entrega el índice de cada elemento empezando en `0`. Es útil cuando el origen representa filas, líneas o posiciones y se quiere incluir esa información en el resultado o en el error.

```csharp
MlResult<IEnumerable<ImportLine>> result = lines.Projection((line, index) =>
	string.IsNullOrWhiteSpace(line)
		? MlResult<ImportLine>.Fail($"La línea {index + 1} está vacía")
		: MlResult<ImportLine>.Valid(new ImportLine(index + 1, line)));
```

## `ProjectionAsync`

`ProjectionAsync` cubre las combinaciones habituales entre fuente síncrona/asíncrona y transformación síncrona/asíncrona:

```csharp
Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this IEnumerable<T> source,
	Func<T, MlResult<TResult>> transform)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this IEnumerable<T> source,
	Func<T, int, MlResult<TResult>> transform)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this IEnumerable<T> source,
	Func<T, Task<MlResult<TResult>>> transformAsync)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this IEnumerable<T> source,
	Func<T, int, Task<MlResult<TResult>>> transformAsync)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this Task<IEnumerable<T>> sourceAsync,
	Func<T, MlResult<TResult>> transform)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this Task<IEnumerable<T>> sourceAsync,
	Func<T, int, MlResult<TResult>> transform)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this Task<IEnumerable<T>> sourceAsync,
	Func<T, Task<MlResult<TResult>>> transformAsync)

Task<MlResult<IEnumerable<TResult>>> ProjectionAsync<T, TResult>(
	this Task<IEnumerable<T>> sourceAsync,
	Func<T, int, Task<MlResult<TResult>>> transformAsync)
```

Aunque el método sea asíncrono, estas sobrecargas procesan los elementos de forma secuencial. Si se necesita ejecutar todas las operaciones simultáneamente, se debe usar `ProjectionParallelAsync`.

```csharp
MlResult<IEnumerable<CustomerDto>> result = await customerIds.ProjectionAsync(async (customerId, index) =>
{
	Customer? customer = await repository.FindAsync(customerId);

	return customer is null
		? MlResult<CustomerDto>.Fail($"No existe el cliente de la posición {index + 1}: {customerId}")
		: MlResult<CustomerDto>.Valid(new CustomerDto(customer.Id, customer.Name));
});
```

## `ProjectionWhile`

`ProjectionWhile` aplica la transformación de forma secuencial y se detiene cuando encuentra el primer `MlResult` fallido. Es la opción adecuada cuando no tiene sentido continuar después del primer error o cuando los elementos dependen del procesamiento anterior.

```csharp
MlResult<IEnumerable<StepResult>> result = steps.ProjectionWhile((step, index) =>
{
	MlResult<StepResult> stepResult = ExecuteStep(step);

	return stepResult.IsFail
		? MlResult<StepResult>.Fail($"Falló el paso {index + 1}: {step.Name}")
		: stepResult;
});
```

Diferencia principal frente a `Projection`:

- `Projection`: evalúa todos los elementos y fusiona todos los errores encontrados.
- `ProjectionWhile`: evalúa hasta el primer error y devuelve ese fallo fusionado.

## `ProjectionWhileAsync`

`ProjectionWhileAsync` mantiene el comportamiento *fail-fast* de `ProjectionWhile`, pero permite fuentes y transformaciones asíncronas.

```csharp
MlResult<IEnumerable<ProcessedFile>> result = await files.ProjectionWhileAsync(async (file, index) =>
{
	bool exists = await storage.ExistsAsync(file.Path);

	if (!exists)
	{
		return MlResult<ProcessedFile>.Fail($"Archivo no encontrado en la posición {index + 1}: {file.Path}");
	}

	ProcessedFile processed = await processor.ProcessAsync(file);
	return MlResult<ProcessedFile>.Valid(processed);
});
```

## `ProjectionParallelAsync`

`ProjectionParallelAsync` está pensada para operaciones asíncronas independientes. Construye todas las tareas, espera a que terminen con `Task.WhenAll` y después fusiona los resultados.

```csharp
MlResult<IEnumerable<EnrichedOrder>> result = await orders.ProjectionParallelAsync(async (order, index) =>
{
	Customer customer = await customerService.GetAsync(order.CustomerId);
	ShippingInfo shipping = await shippingService.GetAsync(order.Id);

	return shipping.IsValid
		? MlResult<EnrichedOrder>.Valid(new EnrichedOrder(index + 1, order, customer, shipping))
		: MlResult<EnrichedOrder>.Fail($"Pedido {order.Id} inválido en posición {index + 1}");
});
```

Aspectos importantes:

- No se detiene en el primer error: todas las tareas se lanzan antes de comprobar los resultados.
- El orden de los valores resultantes sigue el orden de la colección original cuando se proyectan los resultados de las tareas creadas por `Select`.
- Conviene usarla solo cuando las operaciones sean independientes y el servicio externo pueda soportar la concurrencia.
- Si se necesita limitar concurrencia, se recomienda aplicar una estrategia externa de particionado o control antes de llamar a este método.

## `ProjectionSplit` y `ProjectionSplitAsync`

A diferencia del resto de las familias, `ProjectionSplit` **nunca devuelve un `Fail` global**. En lugar
de aplicar la política *todo o nada*, **separa** la colección en dos: los elementos que se
transformaron correctamente y los que fallaron.

Es la herramienta adecuada para **procesos por lotes tolerantes a errores**: importaciones,
migraciones, sincronizaciones nocturnas… donde quieres procesar todo lo que se pueda y generar un
informe con el resto.

```csharp
var (correctos, fallidos) = lineasCsv.ProjectionSplit(linea => ParsearCliente(linea));

// `correctos` contiene los ClienteDto que se pudieron construir.
// `fallidos`  contiene los MlErrorsDetails de los que no.

_repo.InsertarLote(correctos);

foreach (var errores in fallidos)
    _informe.AñadirLineaRechazada(errores.ToErrorsDescription());

_log.LogInformation("Importación: {Ok} correctas, {Ko} rechazadas",
                    correctos.Count(), fallidos.Count());
```

Versión asíncrona, con transformación que llama a un servicio externo:

```csharp
var (sincronizados, rechazados) = await pedidos.ProjectionSplitAsync(
    async pedido => await _erp.SincronizarAsync(pedido));

if (rechazados.Any())
    await _alertas.NotificarAsync($"{rechazados.Count()} pedidos no se sincronizaron");
```

### `ProjectionSplit` frente a las demás familias

| Situación | Familia |
| --- | --- |
| Un solo elemento inválido invalida todo el lote (p. ej. líneas de una factura) | `Projection` |
| Quieres abortar cuanto antes para no gastar recursos | `ProjectionWhile` |
| Quieres procesar todo lo posible y **reportar** lo que falló | `ProjectionSplit` |

> 💡 Si dentro de la parte fallida necesitas saber **qué elemento original** produjo cada error,
> guárdalo en los detalles con `AddValueIfFail` dentro de la propia transformación y recupéralo después
> con `GetDetailValue<T>`. Consulta
> [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md).

## Fusión de errores

Cuando ya tienes una colección de `MlResult<T>` **ya calculada** (por ejemplo, porque los resultados
vienen de varios servicios distintos), estos métodos la convierten en un único `MlResult<IEnumerable<T>>`.
Son los mismos que usan internamente las proyecciones para consolidar sus errores.

| Método | Sobrecargas | Comportamiento |
| --- | --- | --- |
| `FusionFailErros<T>(this IEnumerable<MlResult<T>>)` | 1 | Fusiona los errores de los elementos fallidos. Asume que **hay** al menos un fallo. |
| `FusionFailErrosAsync<T>(...)` | 2 | Versiones asíncronas (fuente `Task<...>` y/o resultado asíncrono). |
| `FusionErrosIfExists<T>(this IEnumerable<MlResult<T>>)` | 1 | Si hay errores, los fusiona; si no, devuelve `Valid` con todos los valores. **Es el que usarás casi siempre.** |
| `FusionErrosIfExistsAsync<T>(...)` | 2 | Versiones asíncronas. |
| `VerifiedEnumerableResultData<T>(this IEnumerable<MlResult<T>>)` | 1 | Comprueba la colección y devuelve todos los valores solo si ninguno falla. |

```csharp
IEnumerable<MlResult<int>> partialResults = [
	MlResult<int>.Valid(10),
	MlResult<int>.Fail("Valor inválido A"),
	MlResult<int>.Fail("Valor inválido B")
];

// Fail con los dos mensajes de error fusionados.
MlResult<IEnumerable<int>> result = partialResults.FusionErrosIfExists();
```

Caso de uso realista: consolidar validaciones independientes que ya se han ejecutado.

```csharp
public MlResult<IEnumerable<Regla>> ValidarReglas(Configuracion config)
{
    IEnumerable<MlResult<Regla>> validaciones =
    [
        ValidarLimiteCredito(config),
        ValidarHorarioApertura(config),
        ValidarMonedaBase(config),
        ValidarPoliticaDescuentos(config)
    ];

    // El usuario ve TODOS los problemas de configuración de una sola vez,
    // no solo el primero.
    return validaciones.FusionErrosIfExists();
}
```

Y su equivalente asíncrono, cuando las validaciones consultan servicios:

```csharp
IEnumerable<Task<MlResult<Regla>>> tareas = reglas.Select(_validador.ComprobarAsync);

MlResult<IEnumerable<Regla>> resultado = await Task.WhenAll(tareas)
                                                   .FusionErrosIfExistsAsync();
```

### `FusionFailErros` frente a `FusionErrosIfExists`

- `FusionErrosIfExists` es **seguro en cualquier caso**: si no hay fallos devuelve `Valid`.
- `FusionFailErros` está pensado para la rama en la que **ya sabes** que hay fallos (por ejemplo, dentro
  de un `if (resultados.Any(r => r.IsFail))`). Es lo que usan las proyecciones internamente.
- `VerifiedEnumerableResultData` expresa la misma idea desde la perspectiva de la **verificación**: "dame
  los datos solo si la colección entera es correcta".

En código de aplicación, empieza siempre por `FusionErrosIfExists`.

## Cuándo usar cada método

| Necesidad | Método recomendado |
|---|---|
| Transformar todos los elementos y conocer todos los errores | `Projection` |
| Transformar todos los elementos incluyendo posición/fila | `Projection` con `Func<T, int, ...>` |
| Usar `await` por elemento de forma secuencial | `ProjectionAsync` |
| Detener en el primer error | `ProjectionWhile` |
| Detener en el primer error con operaciones asíncronas | `ProjectionWhileAsync` |
| Ejecutar transformaciones asíncronas independientes en paralelo | `ProjectionParallelAsync` |
| Procesar todo lo posible y reportar los elementos rechazados | `ProjectionSplit` / `ProjectionSplitAsync` |
| Fusionar una colección ya calculada de `MlResult<T>` | `FusionErrosIfExists` o `VerifiedEnumerableResultData` |

## Enlace de detalle

- [Guía de bucles](../Bucle/Bucles.md)

## Ver también

- [`MlResult<T>`](./MlResult.md) — el tipo base.
- [`MlResultActionsSeveral`](./MlResultActionsSeveral.md) — `Combine`, para varios resultados de **tipos distintos**.
- [`MlResultActionsErrorsDetails`](./MlResultActionsErrorsDetails.md) — recuperar el elemento original que falló.
- [`MlResultActionsBind`](./MlResultActionsBind.md) — `TryBindBuild`, para construir un objeto acumulando errores.
