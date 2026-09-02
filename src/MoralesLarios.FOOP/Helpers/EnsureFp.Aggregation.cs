// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Agregación de validaciones de <see cref="EnsureFp"/>: combinación de varias reglas sobre un mismo valor.
/// </summary>
public static partial class EnsureFp
{

    #region All: ejecuta todas las reglas y fusiona todos los errores

    /// <summary>
    /// Ejecuta todas las reglas sobre el valor y, si alguna falla, devuelve un <c>MlResult</c> fallido
    /// con los errores y detalles de todas las reglas fallidas fusionados.
    /// </summary>
    public static MlResult<T> All<T>(T value, params Func<T, MlResult<T>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        var failsDetails = new List<MlErrorsDetails>();

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = validator(value);

            if (partialResult.IsFail) failsDetails.Add(partialResult.SecureFailErrorsDetails());
        }

        return BuildAggregationResult(value, failsDetails);
    }

    /// <summary>
    /// Ejecuta todas las reglas sobre el valor y, si alguna falla, devuelve un <c>MlResult</c> fallido
    /// con los errores y detalles de todas las reglas fallidas fusionados.
    /// </summary>
    public static MlResult<T> All<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators)
        => All(value, validators?.ToArray()!);

    /// <summary>
    /// Fusiona los errores de todos los <c>MlResult</c> fallidos, devolviendo el valor original si todos son válidos.
    /// </summary>
    public static MlResult<T> AllResults<T>(T value, params MlResult<T>[] results)
        => BuildAggregationResult(value, (results ?? Array.Empty<MlResult<T>>()).Where(x => x.IsFail)
                                                                               .Select(x => x.SecureFailErrorsDetails())
                                                                               .ToList());

    #endregion

    #region AllOrFirst: cortocircuita en la primera regla fallida

    /// <summary>
    /// Ejecuta las reglas en orden y devuelve el primer fallo encontrado, sin evaluar las restantes.
    /// </summary>
    public static MlResult<T> AllOrFirst<T>(T value, params Func<T, MlResult<T>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = validator(value);

            if (partialResult.IsFail) return partialResult;
        }

        return MlResult<T>.Valid(value);
    }

    /// <summary>
    /// Ejecuta las reglas en orden y devuelve el primer fallo encontrado, sin evaluar las restantes.
    /// </summary>
    public static MlResult<T> AllOrFirst<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators)
        => AllOrFirst(value, validators?.ToArray()!);

    #endregion

    #region Any: basta con que una regla se cumpla

    /// <summary>
    /// Devuelve válido si al menos una de las reglas se cumple. Si ninguna se cumple,
    /// fusiona los errores de todas ellas.
    /// </summary>
    public static MlResult<T> Any<T>(T value, params Func<T, MlResult<T>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        var failsDetails = new List<MlErrorsDetails>();

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = validator(value);

            if (partialResult.IsValid) return MlResult<T>.Valid(value);

            failsDetails.Add(partialResult.SecureFailErrorsDetails());
        }

        return BuildAggregationResult(value, failsDetails);
    }

    /// <summary>
    /// Devuelve válido si al menos una de las reglas se cumple. Si ninguna se cumple,
    /// fusiona los errores de todas ellas.
    /// </summary>
    public static MlResult<T> Any<T>(T value, IEnumerable<Func<T, MlResult<T>>> validators)
        => Any(value, validators?.ToArray()!);

    #endregion

    #region Versiones asíncronas

    /// <summary>
    /// Versión asíncrona de <see cref="All{T}(T, Func{T, MlResult{T}}[])"/>. Ejecuta todas las reglas.
    /// </summary>
    public static async Task<MlResult<T>> AllAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        var failsDetails = new List<MlErrorsDetails>();

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = await validator(value);

            if (partialResult.IsFail) failsDetails.Add(partialResult.SecureFailErrorsDetails());
        }

        return BuildAggregationResult(value, failsDetails);
    }

    /// <summary>
    /// Versión asíncrona de <see cref="AllOrFirst{T}(T, Func{T, MlResult{T}}[])"/>. Cortocircuita en el primer fallo.
    /// </summary>
    public static async Task<MlResult<T>> AllOrFirstAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = await validator(value);

            if (partialResult.IsFail) return partialResult;
        }

        return MlResult<T>.Valid(value);
    }

    /// <summary>
    /// Versión asíncrona de <see cref="Any{T}(T, Func{T, MlResult{T}}[])"/>.
    /// </summary>
    public static async Task<MlResult<T>> AnyAsync<T>(T value, params Func<T, Task<MlResult<T>>>[] validators)
    {
        if (validators is null || validators.Length == 0) return MlResult<T>.Valid(value);

        var failsDetails = new List<MlErrorsDetails>();

        foreach (var validator in validators)
        {
            if (validator is null) continue;

            var partialResult = await validator(value);

            if (partialResult.IsValid) return MlResult<T>.Valid(value);

            failsDetails.Add(partialResult.SecureFailErrorsDetails());
        }

        return BuildAggregationResult(value, failsDetails);
    }

    #endregion

    #region Helpers privados

    private static MlResult<T> BuildAggregationResult<T>(T value, List<MlErrorsDetails> failsDetails)
        => failsDetails.Count switch
        {
            0 => MlResult<T>.Valid(value),
            1 => failsDetails[0].ToMlResultFail<T>(),
            _ => failsDetails[0].Merge(failsDetails.Skip(1)).ToMlResultFail<T>()
        };

    #endregion

}
