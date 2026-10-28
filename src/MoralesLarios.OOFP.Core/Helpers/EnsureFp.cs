// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Guardias funcionales que validan argumentos y estados devolviendo un resultado
/// en lugar de lanzar excepciones.
/// </summary>
public static partial class EnsureFp
{


    /// <summary>
    /// Ejecuta <c>NotNull</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> NotNull<T>(T value, string errorMessage)
        => That(value, value is not null, errorMessage);

    /// <summary>
    /// Ejecuta <c>NotNull</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> NotNull<T>(T value, MlErrorDetails errorsDetails)
        => That(value, value is not null, errorsDetails);


    /// <summary>
    /// Ejecuta <c>NotEmpty</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, string message)
        => That(value, value != null && value.Any(), message);

    /// <summary>
    /// Ejecuta <c>NotEmpty</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> NotEmpty<T>(IEnumerable<T> value, MlErrorDetails errorsDetails)
        => That(value, value != null && value.Any(), errorsDetails);


    /// <summary>
    /// Ejecuta <c>NotNullEmptyOrWhitespace</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<string> NotNullEmptyOrWhitespace(string value, string errorMessage)
         => That(value, ! string.IsNullOrWhiteSpace(value), errorMessage);

    /// <summary>
    /// Ejecuta <c>NotNullEmptyOrWhitespace</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<string> NotNullEmptyOrWhitespace(string value, MlErrorDetails errorsDetails)
        => That(value, !string.IsNullOrWhiteSpace(value), errorsDetails);


    /// <summary>
    /// Ejecuta <c>That</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> That<T>(T value, bool condition, string errorMessage)
        => condition ? MlResult<T>.Valid(value) : MlResult<T>.Fail(errorMessage);


    /// <summary>
    /// Ejecuta <c>That</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> That<T>(T value, bool condition, MlErrorDetails errorsDetails)
        => condition ? MlResult<T>.Valid(value) : errorsDetails.ToMlResultFail<T>();


    //public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage)
    //    => condition ? MlResult<T>.Valid(value).ToAsync() : MlResult<T>.Fail(errorMessage).ToAsync();


    /// <summary>
    /// Ejecuta <c>That</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, string errorMessage)
    {
        var result = condition ? MlResult<T>.Valid(value).ToAsync() : MlResult<T>.Fail(errorMessage).ToAsync();

        return result;
    }



    /// <summary>
    /// Ejecuta <c>That</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ThatAsync<T>(T value, bool condition, MlErrorDetails errorsDetails)
        => condition ? MlResult<T>.Valid(value).ToAsync() : errorsDetails.ToMlResultFail<T>().ToAsync();



    //public static async Task<MlResult<T>> NotNullAsync<T>(T value, string errorMessage)
    //    => await ThatAsync(value, value is not null, errorMessage);

    /// <summary>
    /// Ejecuta <c>NotNull</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> NotNullAsync<T>(T value, string errorMessage)
    {
        var result = await ThatAsync(value, value is not null, errorMessage);

        return result;
    }





    /// <summary>
    /// Ejecuta <c>NotNull</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> NotNullAsync<T>(T value, MlErrorDetails errorsDetails)
        => ThatAsync(value, value is not null, errorsDetails);


    /// <summary>
    /// Ejecuta <c>NotEmpty</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, string message)
        => ThatAsync(value, value != null && value.Any(), message);

    /// <summary>
    /// Ejecuta <c>NotEmpty</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(IEnumerable<T> value, MlErrorDetails errorsDetails)
        => ThatAsync(value, value != null && value.Any(), errorsDetails);


    /// <summary>
    /// Ejecuta <c>NotNullEmptyOrWhitespace</c>. Versión asíncrona.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, string errorMessage)
         => ThatAsync(value, ! string.IsNullOrWhiteSpace(value), errorMessage);

    /// <summary>
    /// Ejecuta <c>NotNullEmptyOrWhitespace</c>. Versión asíncrona.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(string value, MlErrorDetails errorsDetails)
        => ThatAsync(value, !string.IsNullOrWhiteSpace(value), errorsDetails);




}

