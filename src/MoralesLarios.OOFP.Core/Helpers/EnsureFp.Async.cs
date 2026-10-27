// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Simetría asíncrona de <see cref="EnsureFp"/>: permite encadenar validaciones sobre
/// valores que llegan dentro de una <see cref="Task{TResult}"/>, usar predicados asíncronos
/// y propagar un <see cref="CancellationToken"/>.
/// </summary>
public static partial class EnsureFp
{

    #region ThatAsync con fuente asíncrona

    /// <summary>
    /// Espera el valor y comprueba el predicado síncrono sobre él.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, bool> predicate, string errorMessage)
    {
        var value = await SecureAwait(valueAsync);

        return That(value, EvaluatePredicate(value, predicate), errorMessage);
    }

    /// <summary>
    /// Espera el valor y comprueba el predicado síncrono sobre él.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, bool> predicate, MlErrorsDetails errorsDetails)
    {
        var value = await SecureAwait(valueAsync);

        return That(value, EvaluatePredicate(value, predicate), errorsDetails);
    }

    /// <summary>
    /// Espera el valor y comprueba la condición ya evaluada.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, bool condition, string errorMessage)
        => That(await SecureAwait(valueAsync), condition, errorMessage);

    /// <summary>
    /// Espera el valor y comprueba la condición ya evaluada.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, bool condition, MlErrorsDetails errorsDetails)
        => That(await SecureAwait(valueAsync), condition, errorsDetails);

    #endregion

    #region ThatAsync con predicado asíncrono

    /// <summary>
    /// Comprueba un predicado asíncrono sobre el valor.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync, string errorMessage)
        => That(value, await EvaluatePredicateAsync(value, predicateAsync), errorMessage);

    /// <summary>
    /// Comprueba un predicado asíncrono sobre el valor.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(T value, Func<T, Task<bool>> predicateAsync, MlErrorsDetails errorsDetails)
        => That(value, await EvaluatePredicateAsync(value, predicateAsync), errorsDetails);

    /// <summary>
    /// Espera el valor y comprueba un predicado asíncrono sobre él.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, Task<bool>> predicateAsync, string errorMessage)
    {
        var value = await SecureAwait(valueAsync);

        return That(value, await EvaluatePredicateAsync(value, predicateAsync), errorMessage);
    }

    /// <summary>
    /// Espera el valor y comprueba un predicado asíncrono sobre él.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(Task<T> valueAsync, Func<T, Task<bool>> predicateAsync, MlErrorsDetails errorsDetails)
    {
        var value = await SecureAwait(valueAsync);

        return That(value, await EvaluatePredicateAsync(value, predicateAsync), errorsDetails);
    }

    /// <summary>
    /// Comprueba un predicado asíncrono cancelable sobre el valor.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(T value,
                                                       Func<T, CancellationToken, Task<bool>> predicateAsync,
                                                       string errorMessage,
                                                       CancellationToken cancellationToken = default)
    {
        if (predicateAsync is null) return That(value, false, errorMessage);

        var condition = await predicateAsync(value, cancellationToken);

        return That(value, condition, errorMessage);
    }

    /// <summary>
    /// Comprueba un predicado asíncrono cancelable sobre el valor.
    /// </summary>
    public static async Task<MlResult<T>> ThatAsync<T>(T value,
                                                       Func<T, CancellationToken, Task<bool>> predicateAsync,
                                                       MlErrorsDetails errorsDetails,
                                                       CancellationToken cancellationToken = default)
    {
        if (predicateAsync is null) return That(value, false, errorsDetails);

        var condition = await predicateAsync(value, cancellationToken);

        return That(value, condition, errorsDetails);
    }

    #endregion

    #region ThatArgAsync (mensaje automático)

    /// <summary>
    /// Comprueba un predicado asíncrono generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static async Task<MlResult<T>> ThatArgAsync<T>(T value,
                                                          Func<T, Task<bool>> predicateAsync,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     await EvaluatePredicateAsync(value, predicateAsync),
                     EnsureFpMessages.NotValid(paramName),
                     paramName);

    /// <summary>
    /// Espera el valor y comprueba un predicado generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static async Task<MlResult<T>> ThatArgAsync<T>(Task<T> valueAsync,
                                                          Func<T, bool> predicate,
                                                          [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null)
    {
        var value = await SecureAwait(valueAsync);

        return BuildRule(value, EvaluatePredicate(value, predicate), EnsureFpMessages.NotValid(paramName), paramName);
    }

    #endregion

    #region TryThatAsync

    /// <summary>
    /// Ejecuta un predicado asíncrono capturando cualquier excepción y convirtiéndola en un fallo enriquecido.
    /// </summary>
    public static async Task<MlResult<T>> TryThatAsync<T>(T value,
                                                          Func<T, Task<bool>> predicateAsync,
                                                          string errorMessage,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        try
        {
            var condition = await EvaluatePredicateAsync(value, predicateAsync);

            return BuildGuard(value, condition, errorMessage, paramName);
        }
        catch (Exception ex)
        {
            return BuildExceptionFail<T>(paramName, ex);
        }
    }

    /// <summary>
    /// Ejecuta un predicado asíncrono capturando cualquier excepción y convirtiéndola en un fallo enriquecido.
    /// </summary>
    public static async Task<MlResult<T>> TryThatAsync<T>(T value,
                                                          Func<T, Task<bool>> predicateAsync,
                                                          MlErrorsDetails errorsDetails,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        try
        {
            var condition = await EvaluatePredicateAsync(value, predicateAsync);

            return That(value, condition, errorsDetails);
        }
        catch (Exception ex)
        {
            return BuildExceptionFail<T>(paramName, ex);
        }
    }

    /// <summary>
    /// Ejecuta un predicado asíncrono capturando la excepción y construyendo el mensaje a partir de ella.
    /// </summary>
    public static async Task<MlResult<T>> TryThatAsync<T>(T value,
                                                          Func<T, Task<bool>> predicateAsync,
                                                          Func<Exception, string> errorMessageBuilder,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        try
        {
            var condition = await EvaluatePredicateAsync(value, predicateAsync);

            return BuildGuard(value, condition, EnsureFpMessages.NotValid(paramName), paramName);
        }
        catch (Exception ex)
        {
            return BuildExceptionFail<T>(paramName, ex, errorMessageBuilder);
        }
    }

    #endregion

    #region Guardias clásicas con fuente asíncrona

    /// <summary>
    /// Espera el valor y comprueba que no es nulo.
    /// </summary>
    public static async Task<MlResult<T>> NotNullAsync<T>(Task<T> valueAsync, string errorMessage)
        => NotNull(await SecureAwait(valueAsync), errorMessage);

    /// <summary>
    /// Espera el valor y comprueba que no es nulo.
    /// </summary>
    public static async Task<MlResult<T>> NotNullAsync<T>(Task<T> valueAsync, MlErrorsDetails errorsDetails)
        => NotNull(await SecureAwait(valueAsync), errorsDetails);

    /// <summary>
    /// Espera el valor y comprueba que no es nulo generando el mensaje de error automáticamente.
    /// </summary>
    public static async Task<MlResult<T>> NotNullArgAsync<T>(Task<T> valueAsync,
                                                             [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null)
    {
        var value = await SecureAwait(valueAsync);

        return BuildRule(value, value is not null, EnsureFpMessages.NotNull(paramName), paramName);
    }

    /// <summary>
    /// Espera la colección y comprueba que no es nula ni vacía.
    /// </summary>
    public static async Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(Task<IEnumerable<T>> valueAsync, string errorMessage)
        => NotEmpty(await SecureAwait(valueAsync), errorMessage);

    /// <summary>
    /// Espera la colección y comprueba que no es nula ni vacía.
    /// </summary>
    public static async Task<MlResult<IEnumerable<T>>> NotEmptyAsync<T>(Task<IEnumerable<T>> valueAsync, MlErrorsDetails errorsDetails)
        => NotEmpty(await SecureAwait(valueAsync), errorsDetails);

    /// <summary>
    /// Espera la colección y comprueba que no es nula ni vacía generando el mensaje de error automáticamente.
    /// </summary>
    public static async Task<MlResult<IEnumerable<T>>> NotEmptyArgAsync<T>(Task<IEnumerable<T>> valueAsync,
                                                                           [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null)
    {
        var value = await SecureAwait(valueAsync);

        return BuildRule(value, value is not null && value.Any(), EnsureFpMessages.NotEmpty(paramName), paramName);
    }

    /// <summary>
    /// Espera la cadena y comprueba que no es nula, vacía ni compuesta solo por espacios en blanco.
    /// </summary>
    public static async Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(Task<string> valueAsync, string errorMessage)
        => NotNullEmptyOrWhitespace(await SecureAwait(valueAsync), errorMessage);

    /// <summary>
    /// Espera la cadena y comprueba que no es nula, vacía ni compuesta solo por espacios en blanco.
    /// </summary>
    public static async Task<MlResult<string>> NotNullEmptyOrWhitespaceAsync(Task<string> valueAsync, MlErrorsDetails errorsDetails)
        => NotNullEmptyOrWhitespace(await SecureAwait(valueAsync), errorsDetails);

    /// <summary>
    /// Espera la cadena y comprueba que tiene contenido, generando el mensaje de error automáticamente.
    /// </summary>
    public static async Task<MlResult<string>> NotNullEmptyOrWhitespaceArgAsync(Task<string> valueAsync,
                                                                                [CallerArgumentExpression(nameof(valueAsync))] string? paramName = null)
    {
        var value = await SecureAwait(valueAsync);

        return BuildRule(value,
                         ! string.IsNullOrWhiteSpace(value),
                         EnsureFpMessages.NotNullEmptyOrWhitespace(paramName),
                         paramName);
    }

    /// <summary>
    /// Espera el nullable de tipo valor y devuelve el valor desenvuelto si tiene contenido.
    /// </summary>
    public static async Task<MlResult<T>> NotNullValueAsync<T>(Task<T?> valueAsync, string errorMessage)
        where T : struct
        => NotNullValue(valueAsync is null ? null : await valueAsync, errorMessage);

    #endregion

    #region Helpers privados

    private static async Task<T> SecureAwait<T>(Task<T> valueAsync)
        => valueAsync is null
                ? default!
                : await valueAsync;

    private static async Task<bool> EvaluatePredicateAsync<T>(T value, Func<T, Task<bool>> predicateAsync)
    {
        if (predicateAsync is null) return false;

        var predicateResult = predicateAsync(value);

        return predicateResult is null
                    ? false
                    : await predicateResult;
    }

    #endregion

}
