// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.EntityFrameworkCore.Repositories;


/// <summary>
/// Representa <c>EfReadRepository</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TContext">Tipo del contexto de datos.</typeparam>
public class EfReadRepository<T, TContext>(TContext dbContext) : EfRepository(dbContext), IEfReadRepository<T>
    where T        : class
    where TContext : DbContext
{

    internal readonly IThrowingEfReadRepository<T>? _repoReader = RegisterServices.GetRepository<IThrowingEfReadRepository<T>>();

    private string typeName = typeof(T).Name;




    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFind(MlErrorDetails notFoundErrorDetails, params object[] keyValues)
    {
        var result = EnsureFp.NotNull(keyValues, "The object array pk cannot be null")
                                .TryMap (x => EnsureFp.NotEmpty(keyValues, "The object array pk cannot be empty"))
                                .TryMap (x => _repoReader!.Find(keyValues))
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));
        return result;
    }

    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFind(params object[] keyValues)
        => TryFind($"{keyValues.FormatCompositeKey()} values not found data in {typeName}", keyValues);


    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFindAsync(MlErrorDetails notFoundErrorDetails, CancellationToken token = default, params object[] keyValues)
    {
        var result = await EnsureFp.NotNullAsync(keyValues, "The object array pk cannot be null")
                                    .TryMapAsync (x => EnsureFp.NotEmptyAsync(keyValues, "The object array pk cannot be empty"))
                                    .TryMapAsync (x => _repoReader!.FindAsync(token, keyValues))
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));
        return result;
    }

    /// <summary>
    /// Busca el elemento solicitado. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<T>> TryFindAsync(CancellationToken token = default, params object[] keyValues)
        => TryFindAsync($"{keyValues.FormatCompositeKey()} values not found data in {typeName}", token, keyValues);


    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => _repoReader!.FirstOrDefault(filter))
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));
        return result;
    }


    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryFirst(Expression<Func<T, bool>> filter)
        => TryFirst(filter, $"The query did not return any elements");


    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync (x => _repoReader!.FirstOrDefaultAsync(filter, token))
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));
        return result;
    }

    /// <summary>
    /// Obtiene el primer elemento. Captura cualquier excepción producida y la convierte en un resultado
    /// fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<T>> TryFirstAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
        => TryFirstAsync(filter, $"The query did not return any elements", token);



    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap (x => _repoReader!.LastOrDefault(filter))
                                .TryBind(x => x.FailIfNull(notFoundErrorDetails));

        return result;
    }


    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<T> TryGetLast(Expression<Func<T, bool>> filter)
        => TryGetLast(filter, $"The query did not return any elements");


    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="notFoundErrorDetails">Valor de <c>notFoundErrorDetails</c>.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, MlErrorDetails notFoundErrorDetails, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync (x => _repoReader!.LastOrDefaultAsync(filter, token))
                                    .TryBindAsync(x => x.FailIfNullAsync(notFoundErrorDetails));

        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Last</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<T>> TryGetLastAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
        => TryGetLastAsync(filter, $"The query did not return any elements", token);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryGetData(Expression<Func<T, bool>> filter)
    {
        var result = EnsureFp.NotNull(filter, "The filter cannot be null")
                                .TryMap(x => _repoReader!.GetData(filter));

        return result;
    }


    /// <summary>
    /// Obtiene el elemento solicitado de <c>Data</c>. Captura cualquier excepción producida y la convierte
    /// en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="filter">Expresión de filtrado aplicada a la consulta.</param>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryGetDataAsync(Expression<Func<T, bool>> filter, CancellationToken token = default!)
    {
        var result = await EnsureFp.NotNullAsync(filter, "The filter cannot be null")
                                    .TryMapAsync(x => _repoReader!.GetDataAsync(filter, token));

        return result;
    }


    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido.
    /// </summary>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual MlResult<IEnumerable<T>> TryAll()
    {
        var result = MlResult<IEnumerable<T>>._
                            .TryMap(x => _repoReader!.All());

        return result;
    }


    /// <summary>
    /// Indica si todos los elementos cumplen la condición. Captura cualquier excepción producida y la
    /// convierte en un resultado fallido. Versión asíncrona.
    /// </summary>
    /// <param name="token">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<IEnumerable<T>>> TryAllAsync(CancellationToken token = default!)
    {
        var result = await MlResult<IEnumerable<T>>._
                                .TryMapAsync(x => _repoReader!.AllAsync(token: token));

        return result;
    }


}



