// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Validaciones de <see cref="EnsureFp"/> especializadas en cadenas de texto:
/// longitudes, expresiones regulares, prefijos/sufijos, contenido y listas de valores permitidos.
/// </summary>
public static partial class EnsureFp
{

    #region Constantes internas

    /// <summary>
    /// Tiempo máximo de evaluación de las expresiones regulares, para evitar ataques de tipo ReDoS.
    /// </summary>
    private static readonly TimeSpan REGEX_DEFAULT_TIMEOUT = TimeSpan.FromSeconds(2);

    #endregion

    #region NotNullOrEmpty

    /// <summary>
    /// Comprueba que la cadena no es nula ni vacía (los espacios en blanco sí se admiten).
    /// </summary>
    public static MlResult<string> NotNullOrEmpty(string value, string errorMessage)
        => That(value, ! string.IsNullOrEmpty(value), errorMessage);

    /// <summary>
    /// Comprueba que la cadena no es nula ni vacía (los espacios en blanco sí se admiten).
    /// </summary>
    public static MlResult<string> NotNullOrEmpty(string value, MlErrorsDetails errorsDetails)
        => That(value, ! string.IsNullOrEmpty(value), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena no es nula ni vacía generando el mensaje con el nombre del argumento.
    /// </summary>
    public static MlResult<string> NotNullOrEmptyArg(string value,
                                                     [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, ! string.IsNullOrEmpty(value), EnsureFpMessages.NotEmpty(paramName), paramName);

    #endregion

    #region MaxLength

    /// <summary>
    /// Comprueba que la longitud de la cadena no supera <paramref name="maxLength"/>.
    /// Una cadena nula se considera fallo.
    /// </summary>
    public static MlResult<string> MaxLength(string value, int maxLength, string errorMessage)
        => That(value, HasMaxLength(value, maxLength), errorMessage);

    /// <summary>
    /// Comprueba que la longitud de la cadena no supera <paramref name="maxLength"/>.
    /// </summary>
    public static MlResult<string> MaxLength(string value, int maxLength, MlErrorsDetails errorsDetails)
        => That(value, HasMaxLength(value, maxLength), errorsDetails);

    /// <summary>
    /// Comprueba que la longitud de la cadena no supera <paramref name="maxLength"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> MaxLengthArg(string value,
                                                int maxLength,
                                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     HasMaxLength(value, maxLength),
                     EnsureFpMessages.MaxLength(paramName, maxLength, value?.Length),
                     paramName,
                     (EXPECTED_KEY, maxLength));

    #endregion

    #region MinLength

    /// <summary>
    /// Comprueba que la longitud de la cadena alcanza <paramref name="minLength"/>.
    /// Una cadena nula se considera fallo.
    /// </summary>
    public static MlResult<string> MinLength(string value, int minLength, string errorMessage)
        => That(value, HasMinLength(value, minLength), errorMessage);

    /// <summary>
    /// Comprueba que la longitud de la cadena alcanza <paramref name="minLength"/>.
    /// </summary>
    public static MlResult<string> MinLength(string value, int minLength, MlErrorsDetails errorsDetails)
        => That(value, HasMinLength(value, minLength), errorsDetails);

    /// <summary>
    /// Comprueba que la longitud de la cadena alcanza <paramref name="minLength"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> MinLengthArg(string value,
                                                int minLength,
                                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     HasMinLength(value, minLength),
                     EnsureFpMessages.MinLength(paramName, minLength, value?.Length),
                     paramName,
                     (EXPECTED_KEY, minLength));

    #endregion

    #region LengthBetween

    /// <summary>
    /// Comprueba que la longitud de la cadena está entre <paramref name="minLength"/> y <paramref name="maxLength"/>, ambos incluidos.
    /// </summary>
    public static MlResult<string> LengthBetween(string value, int minLength, int maxLength, string errorMessage)
        => That(value, HasLengthBetween(value, minLength, maxLength), errorMessage);

    /// <summary>
    /// Comprueba que la longitud de la cadena está entre <paramref name="minLength"/> y <paramref name="maxLength"/>, ambos incluidos.
    /// </summary>
    public static MlResult<string> LengthBetween(string value, int minLength, int maxLength, MlErrorsDetails errorsDetails)
        => That(value, HasLengthBetween(value, minLength, maxLength), errorsDetails);

    /// <summary>
    /// Comprueba el rango de longitud generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> LengthBetweenArg(string value,
                                                    int minLength,
                                                    int maxLength,
                                                    [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     HasLengthBetween(value, minLength, maxLength),
                     EnsureFpMessages.LengthBetween(paramName, minLength, maxLength, value?.Length),
                     paramName);

    #endregion

    #region LengthExactly

    /// <summary>
    /// Comprueba que la cadena tiene exactamente <paramref name="length"/> caracteres.
    /// </summary>
    public static MlResult<string> LengthExactly(string value, int length, string errorMessage)
        => That(value, value is not null && value.Length == length, errorMessage);

    /// <summary>
    /// Comprueba que la cadena tiene exactamente <paramref name="length"/> caracteres.
    /// </summary>
    public static MlResult<string> LengthExactly(string value, int length, MlErrorsDetails errorsDetails)
        => That(value, value is not null && value.Length == length, errorsDetails);

    /// <summary>
    /// Comprueba la longitud exacta generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> LengthExactlyArg(string value,
                                                    int length,
                                                    [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     value is not null && value.Length == length,
                     EnsureFpMessages.LengthExactly(paramName, length, value?.Length),
                     paramName,
                     (EXPECTED_KEY, length));

    #endregion

    #region Matches / NotMatches

    /// <summary>
    /// Comprueba que la cadena cumple el patrón indicado. La evaluación tiene un timeout de seguridad frente a ReDoS.
    /// </summary>
    public static MlResult<string> Matches(string value, string pattern, string errorMessage)
        => That(value, IsMatch(value, pattern), errorMessage);

    /// <summary>
    /// Comprueba que la cadena cumple el patrón indicado.
    /// </summary>
    public static MlResult<string> Matches(string value, string pattern, MlErrorsDetails errorsDetails)
        => That(value, IsMatch(value, pattern), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena cumple la expresión regular precompilada indicada (opción recomendada por rendimiento).
    /// </summary>
    public static MlResult<string> Matches(string value, Regex regex, string errorMessage)
        => That(value, IsMatch(value, regex), errorMessage);

    /// <summary>
    /// Comprueba que la cadena cumple la expresión regular precompilada indicada.
    /// </summary>
    public static MlResult<string> Matches(string value, Regex regex, MlErrorsDetails errorsDetails)
        => That(value, IsMatch(value, regex), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena cumple el patrón indicado generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> MatchesArg(string value,
                                              string pattern,
                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, IsMatch(value, pattern), EnsureFpMessages.Matches(paramName, pattern), paramName);

    /// <summary>
    /// Comprueba que la cadena cumple la expresión regular indicada generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> MatchesArg(string value,
                                              Regex regex,
                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, IsMatch(value, regex), EnsureFpMessages.Matches(paramName, regex?.ToString() ?? "null"), paramName);

    /// <summary>
    /// Comprueba que la cadena NO cumple el patrón indicado.
    /// </summary>
    public static MlResult<string> NotMatches(string value, string pattern, string errorMessage)
        => That(value, ! IsMatch(value, pattern), errorMessage);

    /// <summary>
    /// Comprueba que la cadena NO cumple el patrón indicado.
    /// </summary>
    public static MlResult<string> NotMatches(string value, string pattern, MlErrorsDetails errorsDetails)
        => That(value, ! IsMatch(value, pattern), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena NO cumple la expresión regular indicada.
    /// </summary>
    public static MlResult<string> NotMatches(string value, Regex regex, string errorMessage)
        => That(value, ! IsMatch(value, regex), errorMessage);

    /// <summary>
    /// Comprueba que la cadena NO cumple el patrón indicado generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> NotMatchesArg(string value,
                                                 string pattern,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, ! IsMatch(value, pattern), EnsureFpMessages.NotMatches(paramName, pattern), paramName);

    #endregion

    #region StartsWith / EndsWith / Contains

    /// <summary>
    /// Comprueba que la cadena comienza por <paramref name="prefix"/>.
    /// </summary>
    public static MlResult<string> StartsWith(string value,
                                              string prefix,
                                              string errorMessage,
                                              StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && prefix is not null && value.StartsWith(prefix, comparisonType), errorMessage);

    /// <summary>
    /// Comprueba que la cadena comienza por <paramref name="prefix"/>.
    /// </summary>
    public static MlResult<string> StartsWith(string value,
                                              string prefix,
                                              MlErrorsDetails errorsDetails,
                                              StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && prefix is not null && value.StartsWith(prefix, comparisonType), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena comienza por <paramref name="prefix"/> generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> StartsWithArg(string value,
                                                 string prefix,
                                                 StringComparison comparisonType = StringComparison.Ordinal,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     value is not null && prefix is not null && value.StartsWith(prefix, comparisonType),
                     EnsureFpMessages.StartsWith(paramName, prefix),
                     paramName);

    /// <summary>
    /// Comprueba que la cadena termina en <paramref name="suffix"/>.
    /// </summary>
    public static MlResult<string> EndsWith(string value,
                                            string suffix,
                                            string errorMessage,
                                            StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && suffix is not null && value.EndsWith(suffix, comparisonType), errorMessage);

    /// <summary>
    /// Comprueba que la cadena termina en <paramref name="suffix"/>.
    /// </summary>
    public static MlResult<string> EndsWith(string value,
                                            string suffix,
                                            MlErrorsDetails errorsDetails,
                                            StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && suffix is not null && value.EndsWith(suffix, comparisonType), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena termina en <paramref name="suffix"/> generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> EndsWithArg(string value,
                                               string suffix,
                                               StringComparison comparisonType = StringComparison.Ordinal,
                                               [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     value is not null && suffix is not null && value.EndsWith(suffix, comparisonType),
                     EnsureFpMessages.EndsWith(paramName, suffix),
                     paramName);

    /// <summary>
    /// Comprueba que la cadena contiene <paramref name="substring"/>.
    /// </summary>
    public static MlResult<string> ContainsText(string value,
                                                string substring,
                                                string errorMessage,
                                                StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && substring is not null && value.Contains(substring, comparisonType), errorMessage);

    /// <summary>
    /// Comprueba que la cadena contiene <paramref name="substring"/>.
    /// </summary>
    public static MlResult<string> ContainsText(string value,
                                                string substring,
                                                MlErrorsDetails errorsDetails,
                                                StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && substring is not null && value.Contains(substring, comparisonType), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena contiene <paramref name="substring"/> generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> ContainsTextArg(string value,
                                                   string substring,
                                                   StringComparison comparisonType = StringComparison.Ordinal,
                                                   [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     value is not null && substring is not null && value.Contains(substring, comparisonType),
                     EnsureFpMessages.Contains(paramName, substring),
                     paramName);

    /// <summary>
    /// Comprueba que la cadena NO contiene <paramref name="substring"/>.
    /// </summary>
    public static MlResult<string> NotContainsText(string value,
                                                   string substring,
                                                   string errorMessage,
                                                   StringComparison comparisonType = StringComparison.Ordinal)
        => That(value, value is not null && substring is not null && ! value.Contains(substring, comparisonType), errorMessage);

    /// <summary>
    /// Comprueba que la cadena NO contiene <paramref name="substring"/> generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> NotContainsTextArg(string value,
                                                      string substring,
                                                      StringComparison comparisonType = StringComparison.Ordinal,
                                                      [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     value is not null && substring is not null && ! value.Contains(substring, comparisonType),
                     EnsureFpMessages.NotContains(paramName, substring),
                     paramName);

    #endregion

    #region IsOneOf

    /// <summary>
    /// Comprueba que la cadena está dentro del conjunto de valores permitidos.
    /// </summary>
    public static MlResult<string> IsOneOf(string value,
                                           IEnumerable<string> allowedValues,
                                           string errorMessage,
                                           StringComparer? comparer = null)
        => That(value, IsInSet(value, allowedValues, comparer), errorMessage);

    /// <summary>
    /// Comprueba que la cadena está dentro del conjunto de valores permitidos.
    /// </summary>
    public static MlResult<string> IsOneOf(string value,
                                           IEnumerable<string> allowedValues,
                                           MlErrorsDetails errorsDetails,
                                           StringComparer? comparer = null)
        => That(value, IsInSet(value, allowedValues, comparer), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena está dentro del conjunto de valores permitidos,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> IsOneOfArg(string value,
                                              IEnumerable<string> allowedValues,
                                              StringComparer? comparer = null,
                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     IsInSet(value, allowedValues, comparer),
                     EnsureFpMessages.IsOneOf(paramName, allowedValues),
                     paramName);

    /// <summary>
    /// Comprueba que el valor está dentro del conjunto de valores permitidos (versión genérica).
    /// </summary>
    public static MlResult<T> IsOneOf<T>(T value,
                                         IEnumerable<T> allowedValues,
                                         string errorMessage,
                                         IEqualityComparer<T>? comparer = null)
        => That(value, allowedValues is not null && allowedValues.Contains(value, comparer), errorMessage);

    /// <summary>
    /// Comprueba que el valor está dentro del conjunto de valores permitidos (versión genérica).
    /// </summary>
    public static MlResult<T> IsOneOfArg<T>(T value,
                                            IEnumerable<T> allowedValues,
                                            IEqualityComparer<T>? comparer = null,
                                            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     allowedValues is not null && allowedValues.Contains(value, comparer),
                     EnsureFpMessages.IsOneOf(paramName, allowedValues?.Select(x => x?.ToString() ?? "null")),
                     paramName);

    #endregion

    #region Helpers privados

    private static bool HasMaxLength(string value, int maxLength)
        => value is not null && value.Length <= maxLength;

    private static bool HasMinLength(string value, int minLength)
        => value is not null && value.Length >= minLength;

    private static bool HasLengthBetween(string value, int minLength, int maxLength)
        => value is not null && value.Length >= minLength && value.Length <= maxLength;

    private static bool IsMatch(string value, string pattern)
        => value is not null && pattern is not null && Regex.IsMatch(value, pattern, RegexOptions.None, REGEX_DEFAULT_TIMEOUT);

    private static bool IsMatch(string value, Regex regex)
        => value is not null && regex is not null && regex.IsMatch(value);

    private static bool IsInSet(string value, IEnumerable<string> allowedValues, StringComparer? comparer)
        => allowedValues is not null && allowedValues.Contains(value, comparer ?? StringComparer.Ordinal);

    #endregion

}
