// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;


namespace MoralesLarios.OOFP.Validation.DataAnnotations.Helpers;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{

    /// <summary>
    /// Válida el valor de <c>DataAnnotations</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IEnumerable<ValidationResult> ValidateDataAnnotations(this object source)
    {
        ValidationContext valContext = new ValidationContext(source, null, null);
        var result     = new List<ValidationResult>();
        Validator.TryValidateObject(source, valContext, result, true);
        
        return result;
    } 


    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ValidateWithDataAnnotations<T>(this T source)
    {
        var result = source!.ValidateDataAnnotations().ToMlResultValid()
                                .Map ( valResults => valResults.Select(x => x.ErrorMessage))
                                .Bind( errors     => errors.Any() ? errors!.ToMlResultFail<T>() : source.ToMlResultValid<T>());
        return result;

    }

    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ValidateWithDataAnnotationsAsync<T>(this T source)
        => source!.ValidateWithDataAnnotations().ToAsync();

    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> ValidateWithDataAnnotationsAsync<T>(this Task<T> sourceAsync)
        => await (await sourceAsync).ValidateWithDataAnnotationsAsync();


    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>> ValidateWithDataAnnotations<T>(this IEnumerable<T> source)
    {
        var result = source.Select(x => x.ValidateWithDataAnnotations())
                            .MergeErrorsIfExists();
        return result;
    }

    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IEnumerable<T>>> ValidateWithDataAnnotationsAsync<T>(this IEnumerable<T> source)
        => source.ValidateWithDataAnnotations().ToAsync();

    /// <summary>
    /// Válida el valor de <c>WithDataAnnotations</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<IEnumerable<T>>> ValidateWithDataAnnotationsAsync<T>(this Task<IEnumerable<T>> sourceAsync)
        => await (await sourceAsync).ValidateWithDataAnnotationsAsync();




}
