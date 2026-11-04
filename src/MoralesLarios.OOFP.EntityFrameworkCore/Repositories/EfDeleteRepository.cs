// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Representa <c>EfDeleteRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfDeleteRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfDeleteRepository<T>
    where T        : class
    where TContext : DbContext
{

    private static readonly string typeName = typeof(T).Name;


    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to remove cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<T>().Remove(item);
                                return item;
                            });
        return result;
    }

    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(params object[] keyValues)
        => TryRemove(null!, keyValues);

    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryRemove(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? $"{keyValues.FormatCompositeKey()} values not found data in {typeName}";

        var result = EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty")
                                .TryMap (x       => GetContext().Set<T>().Find(keyValues)!) 
                                .TryBind(bdItem  => bdItem.FailIfNull(errorDetails))
                                .TryMap( bdItem  =>
                                {
                                    GetContext().Set<T>().Remove(bdItem);
                                    return bdItem;
                                });
        return result;
    }


    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryRemoveAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to remove cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().Set<T>().Remove(item);
                                return item;
                            });
        return result;
    }


    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<T>> TryRemoveAsync(CancellationToken token = default, params object[] keyValues)
        => TryRemoveAsync(null!, token, keyValues);

    /// <summary>
    /// Elimina el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryRemoveAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var errorDetails = notFoundErrorDetails ?? $"{keyValues.FormatCompositeKey()} values not found data in {typeName}";

        var result = await EnsureFp.NotEmptyAsync(keyValues, "The object array pk cannot be empty")
                                    .TryMapAsync ( async x      =>
                                    {
                                        var found = await GetContext().Set<T>().FindAsync(keyValues, token);
                                        return found!;
                                    })
                                    .TryBindAsync( bdItem        => bdItem.FailIfNullAsync(errorDetails))
                                    .TryMapAsync( async bdItem  =>
                                    {
                                        GetContext().Set<T>().Remove(bdItem);
                                        return bdItem;
                                    });
        return result;
    }

    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryRemoveRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to remove cannot be null")
                            .TryMap(x =>
                            {
                                GetContext().Set<T>().RemoveRange(items);
                                return items;
                            });
        return result;
    }

    // Punto 35: el resto de metodos *Async del repositorio arrancan la cadena con la guarda
    // asincrona (EnsureFp.NotNullAsync) y esperan el resultado. Aqui se usaba la guarda sincrona
    // y se devolvia la Task sin await, con lo que el metodo no era homogeneo con sus hermanos
    // (ni respecto a la propagacion de excepciones ni al punto de captura de la pila).
    /// <summary>
    /// Elimina el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryRemoveRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to remove cannot be null")
                            .TryMapAsync(async x =>
                            {
                                GetContext().Set<T>().RemoveRange(items);
                                return items;
                            });
        return result;
    }

}
