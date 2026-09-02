// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Validaciones de <see cref="EnsureFp"/> para colecciones: contenido, cardinalidad,
/// duplicados, nulos y predicados sobre los elementos.
/// <para>
/// A diferencia de <c>NotEmpty</c>, las sobrecargas de esta familia preservan el tipo
/// concreto de la colección (<c>List&lt;T&gt;</c>, <c>T[]</c>, ...) en el resultado.
/// </para>
/// </summary>
public static partial class EnsureFp
{

    #region NotEmptyCollection (preserva el tipo concreto)

    /// <summary>
    /// Comprueba que la colección no es nula ni vacía, devolviendo el tipo concreto de la colección.
    /// </summary>
    public static MlResult<TCollection> NotEmptyCollection<TCollection, T>(TCollection value, string errorMessage)
        where TCollection : IEnumerable<T>
        => That(value, value is not null && value.Any(), errorMessage);

    /// <summary>
    /// Comprueba que la colección no es nula ni vacía, devolviendo el tipo concreto de la colección.
    /// </summary>
    public static MlResult<TCollection> NotEmptyCollection<TCollection, T>(TCollection value, MlErrorsDetails errorsDetails)
        where TCollection : IEnumerable<T>
        => That(value, value is not null && value.Any(), errorsDetails);

    /// <summary>
    /// Comprueba que la colección no es nula ni vacía generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<TCollection> NotEmptyCollectionArg<TCollection, T>(TCollection value,
                                                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TCollection : IEnumerable<T>
        => BuildRule(value, value is not null && value.Any(), EnsureFpMessages.NotEmpty(paramName), paramName);

    #endregion

    #region Cardinalidad

    /// <summary>
    /// Comprueba que la colección contiene exactamente <paramref name="expectedCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountExactly<T>(IEnumerable<T> value, int expectedCount, string errorMessage)
        => CountRule(value, expectedCount, expectedCount, errorMessage);

    /// <summary>
    /// Comprueba que la colección contiene exactamente <paramref name="expectedCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountExactly<T>(IEnumerable<T> value, int expectedCount, MlErrorsDetails errorsDetails)
        => CountRule(value, expectedCount, expectedCount, errorsDetails);

    /// <summary>
    /// Comprueba la cardinalidad exacta generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountExactlyArg<T>(IEnumerable<T> value,
                                                              int expectedCount,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && materialized.Count == expectedCount,
                                         EnsureFpMessages.CountExactly(paramName, expectedCount, materialized?.Count),
                                         paramName,
                                         (EXPECTED_KEY, expectedCount));
    }

    /// <summary>
    /// Comprueba que la colección contiene al menos <paramref name="minCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtLeast<T>(IEnumerable<T> value, int minCount, string errorMessage)
        => CountRule(value, minCount, int.MaxValue, errorMessage);

    /// <summary>
    /// Comprueba que la colección contiene al menos <paramref name="minCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtLeast<T>(IEnumerable<T> value, int minCount, MlErrorsDetails errorsDetails)
        => CountRule(value, minCount, int.MaxValue, errorsDetails);

    /// <summary>
    /// Comprueba la cardinalidad mínima generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtLeastArg<T>(IEnumerable<T> value,
                                                              int minCount,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && materialized.Count >= minCount,
                                         EnsureFpMessages.CountAtLeast(paramName, minCount, materialized?.Count),
                                         paramName,
                                         (EXPECTED_KEY, minCount));
    }

    /// <summary>
    /// Comprueba que la colección contiene como máximo <paramref name="maxCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtMost<T>(IEnumerable<T> value, int maxCount, string errorMessage)
        => CountRule(value, 0, maxCount, errorMessage);

    /// <summary>
    /// Comprueba que la colección contiene como máximo <paramref name="maxCount"/> elementos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtMost<T>(IEnumerable<T> value, int maxCount, MlErrorsDetails errorsDetails)
        => CountRule(value, 0, maxCount, errorsDetails);

    /// <summary>
    /// Comprueba la cardinalidad máxima generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountAtMostArg<T>(IEnumerable<T> value,
                                                             int maxCount,
                                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && materialized.Count <= maxCount,
                                         EnsureFpMessages.CountAtMost(paramName, maxCount, materialized?.Count),
                                         paramName,
                                         (EXPECTED_KEY, maxCount));
    }

    /// <summary>
    /// Comprueba que el número de elementos está entre <paramref name="minCount"/> y <paramref name="maxCount"/>, ambos incluidos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountBetween<T>(IEnumerable<T> value, int minCount, int maxCount, string errorMessage)
        => CountRule(value, minCount, maxCount, errorMessage);

    /// <summary>
    /// Comprueba que el número de elementos está entre <paramref name="minCount"/> y <paramref name="maxCount"/>, ambos incluidos.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountBetween<T>(IEnumerable<T> value, int minCount, int maxCount, MlErrorsDetails errorsDetails)
        => CountRule(value, minCount, maxCount, errorsDetails);

    /// <summary>
    /// Comprueba el rango de cardinalidad generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> CountBetweenArg<T>(IEnumerable<T> value,
                                                              int minCount,
                                                              int maxCount,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && materialized.Count >= minCount && materialized.Count <= maxCount,
                                         EnsureFpMessages.CountBetween(paramName, minCount, maxCount, materialized?.Count),
                                         paramName);
    }

    #endregion

    #region Predicados sobre los elementos

    /// <summary>
    /// Comprueba que todos los elementos cumplen el predicado. La colección se materializa una única vez.
    /// </summary>
    public static MlResult<IEnumerable<T>> AllMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && materialized.All(predicate), errorMessage);
    }

    /// <summary>
    /// Comprueba que todos los elementos cumplen el predicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> AllMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, MlErrorsDetails errorsDetails)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && materialized.All(predicate), errorsDetails);
    }

    /// <summary>
    /// Comprueba que todos los elementos cumplen el predicado, informando en los detalles
    /// de los índices que no lo cumplen.
    /// </summary>
    public static MlResult<IEnumerable<T>> AllMatchArg<T>(IEnumerable<T> value,
                                                          Func<T, bool> predicate,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        if (materialized is null || predicate is null)
            return BuildRule<IEnumerable<T>>(materialized!, false, EnsureFpMessages.AllMatch(paramName, null), paramName);

        var failedIndexes = FailedIndexes(materialized, x => ! predicate(x));

        return BuildRule<IEnumerable<T>>(materialized,
                                         failedIndexes.Count == 0,
                                         EnsureFpMessages.AllMatch(paramName, failedIndexes),
                                         paramName,
                                         (FAILED_INDEXES_KEY, failedIndexes));
    }

    /// <summary>
    /// Comprueba que ningún elemento cumple el predicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoneMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && ! materialized.Any(predicate), errorMessage);
    }

    /// <summary>
    /// Comprueba que ningún elemento cumple el predicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoneMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, MlErrorsDetails errorsDetails)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && ! materialized.Any(predicate), errorsDetails);
    }

    /// <summary>
    /// Comprueba que ningún elemento cumple el predicado, informando en los detalles
    /// de los índices que sí lo cumplen.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoneMatchArg<T>(IEnumerable<T> value,
                                                           Func<T, bool> predicate,
                                                           [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        if (materialized is null || predicate is null)
            return BuildRule<IEnumerable<T>>(materialized!, false, EnsureFpMessages.NoneMatch(paramName, null), paramName);

        var matchedIndexes = FailedIndexes(materialized, predicate);

        return BuildRule<IEnumerable<T>>(materialized,
                                         matchedIndexes.Count == 0,
                                         EnsureFpMessages.NoneMatch(paramName, matchedIndexes),
                                         paramName,
                                         (FAILED_INDEXES_KEY, matchedIndexes));
    }

    /// <summary>
    /// Comprueba que al menos un elemento cumple el predicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> AnyMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, string errorMessage)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && materialized.Any(predicate), errorMessage);
    }

    /// <summary>
    /// Comprueba que al menos un elemento cumple el predicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> AnyMatch<T>(IEnumerable<T> value, Func<T, bool> predicate, MlErrorsDetails errorsDetails)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && predicate is not null && materialized.Any(predicate), errorsDetails);
    }

    /// <summary>
    /// Comprueba que al menos un elemento cumple el predicado generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> AnyMatchArg<T>(IEnumerable<T> value,
                                                          Func<T, bool> predicate,
                                                          [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && predicate is not null && materialized.Any(predicate),
                                         EnsureFpMessages.AnyMatch(paramName),
                                         paramName);
    }

    #endregion

    #region Duplicados, nulos y pertenencia

    /// <summary>
    /// Comprueba que la colección no contiene elementos duplicados.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoDuplicates<T>(IEnumerable<T> value, string errorMessage, IEqualityComparer<T>? comparer = null)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, HasNoDuplicates(materialized, comparer), errorMessage);
    }

    /// <summary>
    /// Comprueba que la colección no contiene elementos duplicados.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoDuplicates<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails, IEqualityComparer<T>? comparer = null)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, HasNoDuplicates(materialized, comparer), errorsDetails);
    }

    /// <summary>
    /// Comprueba que la colección no contiene elementos duplicados generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoDuplicatesArg<T>(IEnumerable<T> value,
                                                              IEqualityComparer<T>? comparer = null,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         HasNoDuplicates(materialized, comparer),
                                         EnsureFpMessages.NoDuplicates(paramName),
                                         paramName);
    }

    /// <summary>
    /// Comprueba que la colección no contiene elementos nulos.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoNullItems<T>(IEnumerable<T> value, string errorMessage)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && ! materialized.Any(x => x is null), errorMessage);
    }

    /// <summary>
    /// Comprueba que la colección no contiene elementos nulos.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoNullItems<T>(IEnumerable<T> value, MlErrorsDetails errorsDetails)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && ! materialized.Any(x => x is null), errorsDetails);
    }

    /// <summary>
    /// Comprueba que la colección no contiene elementos nulos, informando de los índices nulos en los detalles.
    /// </summary>
    public static MlResult<IEnumerable<T>> NoNullItemsArg<T>(IEnumerable<T> value,
                                                             [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        if (materialized is null)
            return BuildRule<IEnumerable<T>>(materialized!, false, EnsureFpMessages.NotNull(paramName), paramName);

        var nullIndexes = FailedIndexes(materialized, x => x is null);

        return BuildRule<IEnumerable<T>>(materialized,
                                         nullIndexes.Count == 0,
                                         EnsureFpMessages.NoNullItems(paramName, nullIndexes),
                                         paramName,
                                         (FAILED_INDEXES_KEY, nullIndexes));
    }

    /// <summary>
    /// Comprueba que la colección contiene el elemento indicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> ContainsItem<T>(IEnumerable<T> value, T item, string errorMessage, IEqualityComparer<T>? comparer = null)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && materialized.Contains(item, comparer), errorMessage);
    }

    /// <summary>
    /// Comprueba que la colección contiene el elemento indicado.
    /// </summary>
    public static MlResult<IEnumerable<T>> ContainsItem<T>(IEnumerable<T> value, T item, MlErrorsDetails errorsDetails, IEqualityComparer<T>? comparer = null)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!, materialized is not null && materialized.Contains(item, comparer), errorsDetails);
    }

    /// <summary>
    /// Comprueba que la colección contiene el elemento indicado generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<IEnumerable<T>> ContainsItemArg<T>(IEnumerable<T> value,
                                                              T item,
                                                              IEqualityComparer<T>? comparer = null,
                                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        var materialized = Materialize(value);

        return BuildRule<IEnumerable<T>>(materialized!,
                                         materialized is not null && materialized.Contains(item, comparer),
                                         EnsureFpMessages.ContainsItem(paramName, item),
                                         paramName);
    }

    #endregion

    #region Helpers privados

    private static List<T>? Materialize<T>(IEnumerable<T> value)
        => value is null
                ? null
                : value as List<T> ?? value.ToList();

    private static List<int> FailedIndexes<T>(List<T> source, Func<T, bool> failCondition)
        => source.Select((item, index) => (item, index))
                 .Where(x => failCondition(x.item))
                 .Select(x => x.index)
                 .ToList();

    private static bool HasNoDuplicates<T>(List<T>? source, IEqualityComparer<T>? comparer)
        => source is not null && source.Distinct(comparer).Count() == source.Count;

    private static MlResult<IEnumerable<T>> CountRule<T>(IEnumerable<T> value, int minCount, int maxCount, string errorMessage)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!,
                                    materialized is not null && materialized.Count >= minCount && materialized.Count <= maxCount,
                                    errorMessage);
    }

    private static MlResult<IEnumerable<T>> CountRule<T>(IEnumerable<T> value, int minCount, int maxCount, MlErrorsDetails errorsDetails)
    {
        var materialized = Materialize(value);

        return That<IEnumerable<T>>(materialized!,
                                    materialized is not null && materialized.Count >= minCount && materialized.Count <= maxCount,
                                    errorsDetails);
    }

    #endregion

}
