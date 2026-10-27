// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Núcleo de <see cref="EnsureFp"/>: mensajes perezosos, predicados perezosos,
/// guardias con mensaje automático y validaciones protegidas frente a excepciones.
/// </summary>
public static partial class EnsureFp
{

    #region That con mensajes perezosos

    /// <summary>
    /// Evalúa la condición y, sólo si falla, construye el mensaje de error.
    /// </summary>
    public static MlResult<T> That<T>(T value, bool condition, Func<string> errorMessageBuilder)
        => condition
                ? MlResult<T>.Valid(value)
                : MlResult<T>.Fail(BuildMessage(errorMessageBuilder));

    /// <summary>
    /// Evalúa la condición y, sólo si falla, construye los detalles de error.
    /// </summary>
    public static MlResult<T> That<T>(T value, bool condition, Func<MlErrorsDetails> errorsDetailsBuilder)
        => condition
                ? MlResult<T>.Valid(value)
                : (errorsDetailsBuilder is null
                        ? MlResult<T>.Fail(EnsureFpMessages.NotValid(null))
                        : errorsDetailsBuilder().ToMlResultFail<T>());

    #endregion

    #region That con predicados perezosos

    /// <summary>
    /// Evalúa el predicado sobre el valor. Un predicado nulo se considera fallo.
    /// </summary>
    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, string errorMessage)
        => That(value, EvaluatePredicate(value, predicate), errorMessage);

    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, MlErrorsDetails errorsDetails)
        => That(value, EvaluatePredicate(value, predicate), errorsDetails);

    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<string> errorMessageBuilder)
        => That(value, EvaluatePredicate(value, predicate), errorMessageBuilder);

    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<MlErrorsDetails> errorsDetailsBuilder)
        => That(value, EvaluatePredicate(value, predicate), errorsDetailsBuilder);

    /// <summary>
    /// Evalúa el predicado sobre el valor y construye el mensaje de error a partir del propio valor.
    /// </summary>
    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<T, string> errorMessageBuilder)
        => That(value, EvaluatePredicate(value, predicate), () => BuildMessage(value, errorMessageBuilder));

    /// <summary>
    /// Evalúa el predicado sobre el valor y construye los detalles de error a partir del propio valor.
    /// </summary>
    public static MlResult<T> That<T>(T value, Func<T, bool> predicate, Func<T, MlErrorsDetails> errorsDetailsBuilder)
        => That(value, EvaluatePredicate(value, predicate), () => errorsDetailsBuilder is null
                                                                        ? MlErrorsDetails.FromErrorMessage(EnsureFpMessages.NotValid(null))
                                                                        : errorsDetailsBuilder(value));

    #endregion

    #region TryThat: predicados que pueden lanzar excepciones

    /// <summary>
    /// Ejecuta el predicado capturando cualquier excepción y convirtiéndola en un <c>MlResult</c> fallido.
    /// </summary>
    public static MlResult<T> TryThat<T>(T                        value,
                                         Func<T, bool>            predicate,
                                         string                   errorMessage,
                                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => TryThat(value, predicate, _ => errorMessage, paramName);

    /// <summary>
    /// Ejecuta el predicado capturando cualquier excepción y convirtiéndola en un <c>MlResult</c> fallido.
    /// </summary>
    public static MlResult<T> TryThat<T>(T                        value,
                                         Func<T, bool>            predicate,
                                         MlErrorsDetails          errorsDetails,
                                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (predicate is null) return MlResult<T>.Fail(EnsureFpMessages.NotValid(paramName));

        try
        {
            return That(value, predicate(value), errorsDetails);
        }
        catch (Exception ex)
        {
            return BuildExceptionFail<T>(paramName, ex);
        }
    }

    /// <summary>
    /// Ejecuta el predicado capturando cualquier excepción, permitiendo construir el mensaje a partir de ella.
    /// </summary>
    public static MlResult<T> TryThat<T>(T                        value,
                                         Func<T, bool>            predicate,
                                         Func<Exception, string>  errorMessageBuilder,
                                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (predicate is null) return MlResult<T>.Fail(EnsureFpMessages.NotValid(paramName));

        try
        {
            return That(value, predicate(value), () => BuildMessage(errorMessageBuilder, paramName));
        }
        catch (Exception ex)
        {
            return BuildExceptionFail<T>(paramName, ex, errorMessageBuilder);
        }
    }

    #endregion

    #region Guardias con mensaje automático

    /// <summary>
    /// Comprueba que el valor no es nulo generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static MlResult<T> NotNullArg<T>(T value,
                                            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildGuard(value, value is not null, EnsureFpMessages.NotNull(paramName), paramName);

    /// <summary>
    /// Comprueba que la colección no es nula ni vacía generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static MlResult<IEnumerable<T>> NotEmptyArg<T>(IEnumerable<T> value,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildGuard(value, value is not null && value.Any(), EnsureFpMessages.NotEmpty(paramName), paramName);

    /// <summary>
    /// Comprueba que la cadena no es nula, vacía ni sólo espacios, generando el mensaje con el nombre del argumento.
    /// </summary>
    public static MlResult<string> NotNullEmptyOrWhitespaceArg(string value,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildGuard(value, ! string.IsNullOrWhiteSpace(value), EnsureFpMessages.NotNullEmptyOrWhitespace(paramName), paramName);

    /// <summary>
    /// Comprueba la condición generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static MlResult<T> ThatArg<T>(T value,
                                         bool condition,
                                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildGuard(value, condition, EnsureFpMessages.NotValid(paramName), paramName);

    /// <summary>
    /// Comprueba el predicado generando el mensaje de error con el nombre del argumento.
    /// </summary>
    public static MlResult<T> ThatArg<T>(T value,
                                         Func<T, bool> predicate,
                                         [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildGuard(value, EvaluatePredicate(value, predicate), EnsureFpMessages.NotValid(paramName), paramName);

    #endregion

    #region Helpers privados

    private static bool EvaluatePredicate<T>(T value, Func<T, bool> predicate)
        => predicate is not null && predicate(value);

    private static string BuildMessage(Func<string> errorMessageBuilder)
        => errorMessageBuilder is null
                ? EnsureFpMessages.NotValid(null)
                : errorMessageBuilder() ?? EnsureFpMessages.NotValid(null);

    private static string BuildMessage<T>(T value, Func<T, string> errorMessageBuilder)
        => errorMessageBuilder is null
                ? EnsureFpMessages.NotValid(null)
                : errorMessageBuilder(value) ?? EnsureFpMessages.NotValid(null);

    private static string BuildMessage(Func<Exception, string> errorMessageBuilder, string? paramName)
        => errorMessageBuilder is null
                ? EnsureFpMessages.NotValid(paramName)
                : errorMessageBuilder(null!) ?? EnsureFpMessages.NotValid(paramName);

    private static MlResult<T> BuildGuard<T>(T value, bool condition, string errorMessage, string? paramName)
        => condition
                ? MlResult<T>.Valid(value)
                : MlErrorsDetails.FromErrorMessage(errorMessage)
                                 .AddDetail(PARAM_NAME_KEY, EnsureFpMessages.SafeName(paramName))
                                 .AddDetail<object>(VALUE_KEY, value!)
                                 .ToMlResultFail<T>();

    private static MlResult<T> BuildExceptionFail<T>(string?                 paramName,
                                                     Exception               ex,
                                                     Func<Exception, string> errorMessageBuilder = null!)
    {
        var message = errorMessageBuilder is null
                            ? EnsureFpMessages.PredicateException(paramName, ex)
                            : errorMessageBuilder(ex) ?? EnsureFpMessages.PredicateException(paramName, ex);

        return MlErrorsDetails.FromErrorMessage(message)
                              .AddDetail(PARAM_NAME_KEY, EnsureFpMessages.SafeName(paramName))
                              .AppendExDetailsToMlDetails(ex)
                              .ToMlResultFail<T>();
    }

    /// <summary>
    /// Construye el resultado de una regla concreta, enriqueciendo los detalles del error
    /// con el nombre del parámetro, el valor evaluado y los detalles adicionales indicados.
    /// </summary>
    private static MlResult<T> BuildRule<T>(T                              value,
                                            bool                           condition,
                                            string                         errorMessage,
                                            string?                        paramName,
                                            params (string Key, object Value)[] extraDetails)
    {
        if (condition) return MlResult<T>.Valid(value);

        var errorsDetails = MlErrorsDetails.FromErrorMessage(errorMessage)
                                           .AddDetail(PARAM_NAME_KEY, EnsureFpMessages.SafeName(paramName))
                                           .AddDetail<object>(VALUE_KEY, value!);

        foreach (var (key, detailValue) in extraDetails ?? Array.Empty<(string, object)>())
            errorsDetails = errorsDetails.AddDetail(key, detailValue);

        return errorsDetails.ToMlResultFail<T>();
    }

    #endregion

}
