// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Validation.DataAnnotations;

/// <summary>
/// Representa <c>DataAnnotationsValidator</c>.
/// </summary>
public static class DataAnnotationsValidator
{

    /// <summary>
    /// Válida el valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Validate<T>(T source) => ValidateSource(source);


    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ValidateAsync<T>(T source) => ValidateSource(source).ToAsync();

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ValidateAsync<T>(Task<T> sourceAsync)
    {
        var source = await sourceAsync;
        return await ValidateSource(source).ToAsync();
    }


    /// <summary>
    /// Válida el valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> Validate<T>(IEnumerable<T> source) => ValidateSource(source);

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(IEnumerable<T> source) => ValidateSource(source).ToAsync();

    /// <summary>
    /// Válida el valor. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> ValidateAsync<T>(Task<IEnumerable<T>> sourceAsync)
    {
        var source = await sourceAsync;
        return await ValidateSource(source).ToAsync();
    }


    private static MlResult<T> ValidateSource<T>(T source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} must not be null")
                                .Bind(_ => source.ValidateWithDataAnnotations());

        return result;
    }

    private static MlResult<IEnumerable<T>> ValidateSource<T>(IEnumerable<T> source)
    {
        var result = EnsureFp.NotNull(source, $"{nameof(source)} must not be null")
                                .Bind(_ => EnsureFp.NotEmpty(source, $"{nameof(source)} must not be empty"))
                                .Bind(_ => source.ValidateWithDataAnnotations());
        return result;
    }
}
