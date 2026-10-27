// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using System.Runtime.CompilerServices;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Validaciones de <see cref="EnsureFp"/> para tipos numéricos y comparables:
/// comparaciones, rangos y signos.
/// </summary>
public static partial class EnsureFp
{

    #region GreaterThan

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> GreaterThan<T>(T value, T limit, string errorMessage)
        where T : IComparable<T>
        => That(value, Compare(value, limit) > 0, errorMessage);

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> GreaterThan<T>(T value, T limit, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, Compare(value, limit) > 0, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que <paramref name="limit"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> GreaterThanArg<T>(T value,
                                                T limit,
                                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value,
                     Compare(value, limit) > 0,
                     EnsureFpMessages.GreaterThan(paramName, limit),
                     paramName,
                     (EXPECTED_KEY, limit!));

    #endregion

    #region GreaterOrEqual

    /// <summary>
    /// Comprueba que el valor es mayor o igual que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> GreaterOrEqual<T>(T value, T limit, string errorMessage)
        where T : IComparable<T>
        => That(value, Compare(value, limit) >= 0, errorMessage);

    /// <summary>
    /// Comprueba que el valor es mayor o igual que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> GreaterOrEqual<T>(T value, T limit, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, Compare(value, limit) >= 0, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es mayor o igual que <paramref name="limit"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> GreaterOrEqualArg<T>(T value,
                                                   T limit,
                                                   [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value,
                     Compare(value, limit) >= 0,
                     EnsureFpMessages.GreaterOrEqual(paramName, limit),
                     paramName,
                     (EXPECTED_KEY, limit!));

    #endregion

    #region LessThan

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> LessThan<T>(T value, T limit, string errorMessage)
        where T : IComparable<T>
        => That(value, Compare(value, limit) < 0, errorMessage);

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> LessThan<T>(T value, T limit, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, Compare(value, limit) < 0, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que <paramref name="limit"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> LessThanArg<T>(T value,
                                             T limit,
                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value,
                     Compare(value, limit) < 0,
                     EnsureFpMessages.LessThan(paramName, limit),
                     paramName,
                     (EXPECTED_KEY, limit!));

    #endregion

    #region LessOrEqual

    /// <summary>
    /// Comprueba que el valor es menor o igual que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> LessOrEqual<T>(T value, T limit, string errorMessage)
        where T : IComparable<T>
        => That(value, Compare(value, limit) <= 0, errorMessage);

    /// <summary>
    /// Comprueba que el valor es menor o igual que <paramref name="limit"/>.
    /// </summary>
    public static MlResult<T> LessOrEqual<T>(T value, T limit, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, Compare(value, limit) <= 0, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es menor o igual que <paramref name="limit"/>,
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> LessOrEqualArg<T>(T value,
                                                T limit,
                                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value,
                     Compare(value, limit) <= 0,
                     EnsureFpMessages.LessOrEqual(paramName, limit),
                     paramName,
                     (EXPECTED_KEY, limit!));

    #endregion

    #region InRange / OutOfRange

    /// <summary>
    /// Comprueba que el valor está entre <paramref name="min"/> y <paramref name="max"/>, ambos incluidos.
    /// </summary>
    public static MlResult<T> InRange<T>(T value, T min, T max, string errorMessage)
        where T : IComparable<T>
        => That(value, IsInRange(value, min, max), errorMessage);

    /// <summary>
    /// Comprueba que el valor está entre <paramref name="min"/> y <paramref name="max"/>, ambos incluidos.
    /// </summary>
    public static MlResult<T> InRange<T>(T value, T min, T max, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, IsInRange(value, min, max), errorsDetails);

    /// <summary>
    /// Comprueba el rango generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> InRangeArg<T>(T value,
                                            T min,
                                            T max,
                                            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value, IsInRange(value, min, max), EnsureFpMessages.InRange(paramName, min, max), paramName);

    /// <summary>
    /// Comprueba que el valor NO está entre <paramref name="min"/> y <paramref name="max"/>, ambos incluidos.
    /// </summary>
    public static MlResult<T> OutOfRange<T>(T value, T min, T max, string errorMessage)
        where T : IComparable<T>
        => That(value, value is not null && ! IsInRange(value, min, max), errorMessage);

    /// <summary>
    /// Comprueba que el valor NO está entre <paramref name="min"/> y <paramref name="max"/>, ambos incluidos.
    /// </summary>
    public static MlResult<T> OutOfRange<T>(T value, T min, T max, MlErrorsDetails errorsDetails)
        where T : IComparable<T>
        => That(value, value is not null && ! IsInRange(value, min, max), errorsDetails);

    /// <summary>
    /// Comprueba que el valor está fuera del rango generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> OutOfRangeArg<T>(T value,
                                               T min,
                                               T max,
                                               [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : IComparable<T>
        => BuildRule(value,
                     value is not null && ! IsInRange(value, min, max),
                     EnsureFpMessages.OutOfRange(paramName, min, max),
                     paramName);

    #endregion

    #region Signos

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que cero.
    /// </summary>
    public static MlResult<T> Positive<T>(T value, string errorMessage)
        where T : INumber<T>
        => That(value, value is not null && value > T.Zero, errorMessage);

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que cero.
    /// </summary>
    public static MlResult<T> Positive<T>(T value, MlErrorsDetails errorsDetails)
        where T : INumber<T>
        => That(value, value is not null && value > T.Zero, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es estrictamente mayor que cero generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> PositiveArg<T>(T value,
                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumber<T>
        => BuildRule(value, value is not null && value > T.Zero, EnsureFpMessages.Positive(paramName), paramName);

    /// <summary>
    /// Comprueba que el valor no es negativo (cero incluido).
    /// </summary>
    public static MlResult<T> NotNegative<T>(T value, string errorMessage)
        where T : INumber<T>
        => That(value, value is not null && value >= T.Zero, errorMessage);

    /// <summary>
    /// Comprueba que el valor no es negativo (cero incluido).
    /// </summary>
    public static MlResult<T> NotNegative<T>(T value, MlErrorsDetails errorsDetails)
        where T : INumber<T>
        => That(value, value is not null && value >= T.Zero, errorsDetails);

    /// <summary>
    /// Comprueba que el valor no es negativo generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> NotNegativeArg<T>(T value,
                                                [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumber<T>
        => BuildRule(value, value is not null && value >= T.Zero, EnsureFpMessages.NotNegative(paramName), paramName);

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que cero.
    /// </summary>
    public static MlResult<T> Negative<T>(T value, string errorMessage)
        where T : INumber<T>
        => That(value, value is not null && value < T.Zero, errorMessage);

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que cero.
    /// </summary>
    public static MlResult<T> Negative<T>(T value, MlErrorsDetails errorsDetails)
        where T : INumber<T>
        => That(value, value is not null && value < T.Zero, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es estrictamente menor que cero generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> NegativeArg<T>(T value,
                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumber<T>
        => BuildRule(value, value is not null && value < T.Zero, EnsureFpMessages.Negative(paramName), paramName);

    /// <summary>
    /// Comprueba que el valor es distinto de cero.
    /// </summary>
    public static MlResult<T> NotZero<T>(T value, string errorMessage)
        where T : INumber<T>
        => That(value, value is not null && value != T.Zero, errorMessage);

    /// <summary>
    /// Comprueba que el valor es distinto de cero.
    /// </summary>
    public static MlResult<T> NotZero<T>(T value, MlErrorsDetails errorsDetails)
        where T : INumber<T>
        => That(value, value is not null && value != T.Zero, errorsDetails);

    /// <summary>
    /// Comprueba que el valor es distinto de cero generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> NotZeroArg<T>(T value,
                                            [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : INumber<T>
        => BuildRule(value, value is not null && value != T.Zero, EnsureFpMessages.NotZero(paramName), paramName);

    #endregion

    #region Helpers privados

    private static int Compare<T>(T value, T other)
        where T : IComparable<T>
        => value is null ? -1 : value.CompareTo(other);

    private static bool IsInRange<T>(T value, T min, T max)
        where T : IComparable<T>
        => value is not null && value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;

    #endregion

}
