// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;

/// <summary>
/// Representa <c>EfCreateRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfCreateRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfCreateRepository<T>
    where T        : class
    where TContext : DbContext
{

    internal readonly IThrowingEfCreateRepository<T>? _internalRepoAdder = RegisterServices.GetRepository<IThrowingEfCreateRepository<T>>();



    /// <summary>
    /// Añade el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryAdd(T item)
    {
        var result = EnsureFp.NotNull(item, "The entity item to add cannot be null")
                            .TryMap(x => _internalRepoAdder!.Add(item));

        return result;
    }

    /// <summary>
    /// Añade el elemento indicado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="item">Elemento sobre el que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryAddAsync(T item, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(item, "The entity item to add cannot be null")
                            .TryMapAsync(x => _internalRepoAdder!.AddAsync(item, token));

        return result;
    }


    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte en
    /// un resultado fallido.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryAddRange(IEnumerable<T> items)
    {
        var result = EnsureFp.NotNull(items, "The entity items to addRange cannot be null")
                            .TryMap(x => _internalRepoAdder!.AddRange(items));

        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>Range</c>. Captura cualquier excepción producida y la convierte en
    /// un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryAddRangeAsync(IEnumerable<T> items, CancellationToken token = default)
    {
        var result = await EnsureFp.NotNullAsync(items, "The entity items to addRange cannot be null")
                            .TryMapAsync(x => _internalRepoAdder!.AddRangeAsync(items, token));

        return result;
    }



}


