// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Métodos de extension de <c>MlResultSeveral</c>.
/// </summary>
public static partial class MlResultSeveralExtensions
{


    #region FailIfEmpty



    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T> items, 
                                                                  MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : error.ToMlResultFail<IEnumerable<T>>();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T>  items, 
                                                                  MlErrorDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IEnumerable<T>>? FailIfEmpty<T>(this IEnumerable<T> items, 
                                                                  string         messageError)
        => FailIfEmpty(items, MlError.FromErrorMessage(messageError));



    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T> items, 
                                                                             MlError        error)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : error.ToMlResultFail<IEnumerable<T>>().ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T>  items, 
                                                                             MlErrorDetails errorsDetails)
        => (items != null && items.Any()) 
                ? items.ToMlResultValid().ToAsync() 
                : errorsDetails.ToMlResultFail<IEnumerable<T>>().ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="items">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this IEnumerable<T> items, 
                                                                             string         messageError)
        => FailIfEmpty(items, MlError.FromErrorMessage(messageError)).ToAsync()!;



    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public async static Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlError              error)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(error);

        return result!;
    }

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public static async Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   MlErrorDetails errorsDetails)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(errorsDetails);

        return result!;
    }

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la colección esta vacia. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="itemsAsync">Elementos sobre los que se opera.</param>
    /// <param name="messageError">Valor de <c>messageError</c>.</param>
    /// <returns>Valor de tipo <c>Task&lt;MlResult&lt;IEnumerable&lt;T&gt;&gt;&gt;?</c> obtenido por la operación.</returns>
    public static async Task<MlResult<IEnumerable<T>>>? FailIfEmptyAsync<T>(this Task<IEnumerable<T>> itemsAsync, 
                                                                                   string               messageError)
    {
        var partialMlResult = await itemsAsync;

        var result = partialMlResult.FailIfEmpty(messageError);

        return result!;
    }









    #endregion FailIfEmpty



    #region FailIfNull


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfNull<T>(this T               source,
                                                   MlErrorDetails errorsDetails)
        => source == null ? errorsDetails.ToMlResultFail<T>() : source.ToMlResultValid();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfNull<T>(this T       source,
                                                   MlError error)
        => source.FailIfNull(MlErrorDetails.FromError(error));


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfNull<T>(this T      source,
                                                   string errorMessage)
        => source.FailIfNull(MlError.FromErrorMessage(errorMessage));



    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfNull<T>(this T                   source,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfNull(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T       source,
                                                                    MlError error)
        => await source.FailIfNullAsync(MlErrorDetails.FromError(error));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T               source,
                                                                    MlErrorDetails errorsDetails)
        => await source.FailIfNull(errorsDetails).ToAsync();


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T      source,
                                                                    string errorMessage)
        => await source.FailIfNull(errorMessage).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this T                   source,
                                                                    IEnumerable<string> errorsMessage)
        => await source.FailIfNullAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T> sourceAsync,
                                                                    string  errorMessage)
        => await (await sourceAsync).FailIfNullAsync(errorMessage);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>       sourceAsync,
                                                                    MlError       error)
        => await (await sourceAsync).FailIfNullAsync(error);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>             sourceAsync,
                                                                    MlErrorDetails    errorsDetails)
        => await (await sourceAsync).FailIfNullAsync(errorsDetails);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando el valor es nulo. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfNullAsync<T>(this Task<T>             sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfNullAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    #endregion



    #region FailIfFalse




    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfFalse<T>(this T               source,
                                                    bool            condition,
                                                    MlErrorDetails errorsDetails)
        => condition ?  source.ToMlResultValid() : errorsDetails.ToMlResultFail<T>();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfFalse<T>(this T       source,
                                                   bool    condition,
                                                   MlError error)
        => source.FailIfFalse(condition, MlErrorDetails.FromError(error));


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfFalse<T>(this T      source,
                                                   bool   condition,
                                                   string errorMessage)
        => source.FailIfFalse(condition, MlError.FromErrorMessage(errorMessage));



    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FailIfFalse<T>(this T                   source,
                                                   bool                condition,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfFalse(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T       source,
                                                                    bool    condition,
                                                                    MlError error)
        => await source.FailIfFalseAsync(condition, MlErrorDetails.FromError(error));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> FailIfFalseAsync<T>(this T               source,
                                                                    bool            condition,
                                                                    MlErrorDetails errorsDetails)
        => source.FailIfFalse(condition, errorsDetails).ToAsync();


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T      source,
                                                                    bool   condition,
                                                                    string errorMessage)
        => await source.FailIfFalseAsync(condition, MlError.FromErrorMessage(errorMessage));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this T                   source,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await source.FailIfFalseAsync(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T> sourceAsync,
                                                                    bool    condition,
                                                                    string  errorMessage)
        => await (await sourceAsync).FailIfFalseAsync(condition, errorMessage);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>       sourceAsync,
                                                                    bool          condition,
                                                                    MlError       error)
        => await (await sourceAsync).FailIfFalseAsync(condition, error);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>             sourceAsync,
                                                                    bool                condition,
                                                                    MlErrorDetails    errorsDetails)
        => await (await sourceAsync).FailIfFalseAsync(condition, errorsDetails);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="condition">Condición que debe cumplirse.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FailIfFalseAsync<T>(this Task<T>             sourceAsync,
                                                                    bool                condition,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfFalseAsync(condition, MlErrorDetails.FromEnumerableStrings(errorsMessage));










    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<bool> FailIfFalse(this bool            source,
                                                   MlErrorDetails errorsDetails)
        => source ? source.ToMlResultValid() : errorsDetails.ToMlResultFail<bool>();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<bool> FailIfFalse(this bool    source,
                                                   MlError error)
        => source.FailIfFalse(MlErrorDetails.FromError(error));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<bool> FailIfFalse(this bool   source,
                                                   string errorMessage)
        => source.FailIfFalse(MlError.FromErrorMessage(errorMessage));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<bool> FailIfFalse(this bool                source,
                                                   IEnumerable<string> errorsMessage)
        => source.FailIfFalse(MlErrorDetails.FromEnumerableStrings(errorsMessage));


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<bool>> FailIfFalseAsync(this bool            source,
                                                              MlErrorDetails errorsDetails)
        => source.FailIfFalse(errorsDetails).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<bool>> FailIfFalseAsync(this bool    source,
                                                              MlError error)
        => source.FailIfFalse(MlErrorDetails.FromError(error)).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<bool>> FailIfFalseAsync(this bool   source,
                                                              string errorMessage)
        => source.FailIfFalse(MlError.FromErrorMessage(errorMessage)).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<bool>> FailIfFalseAsync(this bool                source,
                                                              IEnumerable<string> errorsMessage)
        => source.FailIfFalse(MlErrorDetails.FromEnumerableStrings(errorsMessage)).ToAsync();


    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool>      sourceAsync,
                                                                    MlErrorDetails errorsDetails)
        => await (await sourceAsync).FailIfFalseAsync(errorsDetails);

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool> sourceAsync,
                                                                    MlError    error)
        => await (await sourceAsync).FailIfFalseAsync(MlErrorDetails.FromError(error));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool> sourceAsync,
                                                                    string     errorMessage)
        => await (await sourceAsync).FailIfFalseAsync(MlError.FromErrorMessage(errorMessage));

    /// <summary>
    /// Ejecuta <c>Fail</c> unicamente cuando la condición no se cumple. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<bool>> FailIfFalseAsync(this Task<bool>          sourceAsync,
                                                                    IEnumerable<string> errorsMessage)
        => await (await sourceAsync).FailIfFalseAsync(MlErrorDetails.FromEnumerableStrings(errorsMessage));



    #endregion




    // ********************************************************



    #region Combine


    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2)> CombineAll<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                TResult2           otherValue)
        => source.Match(
                            valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((x, otherValue)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this MlResult<TResult1> source, 
                                                                                                           TResult2           otherValue)
        => source.CombineAll(otherValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="otherValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this Task<MlResult<TResult1>> sourceAsync, 
                                                                                                                 TResult2                 otherValue)
        => await (await sourceAsync).CombineAllAsync(otherValue);





    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3)> CombineAll<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                      (TResult2 value1, TResult3 value2) values)
        => source.Match(
                            valid: x       => MlResult<(TResult1, TResult2, TResult3)>.Valid((x, values.value1, values.value2)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this MlResult<TResult1>                 source, 
                                                                                                                 (TResult2 value1, TResult3 value2) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this Task<MlResult<TResult1>>           sourceAsync, 
                                                                                                                       (TResult2 value1, TResult3 value2) values)
        => await (await sourceAsync).CombineAllAsync(values);





    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source, 
                                                                                                                          (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.Match(
                            valid: x              => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((x, values.value1, values.value2, values.value3)),
                            fail   : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this MlResult<TResult1>                                  source,
                                                                                                                                     (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this Task<MlResult<TResult1>>                            sourceAsync,
                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3) values)
        => await (await sourceAsync).CombineAllAsync(values);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                              (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((x, values.value1, values.value2, values.value3, values.value4)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this MlResult<TResult1>                                                   source,
                                                                                                                                                         (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this Task<MlResult<TResult1>>                                             sourceAsync,
                                                                                                                                                               (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4) values)
        => await (await sourceAsync).CombineAllAsync(values);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this MlResult<TResult1> source,
                                                                                                                                                                             (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                   (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5) values)
        => await (await sourceAsync).CombineAllAsync(values);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this MlResult<TResult1> source,
                                                                                                                                                                                                 (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                       (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6) values)
        => await (await sourceAsync).CombineAllAsync(values);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                  (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.Match(
                            valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((x, values.value1, values.value2, values.value3, values.value4, values.value5, values.value6, values.value7)),
                            fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                       );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this MlResult<TResult1> source,
                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => source.CombineAll(values).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this Task<MlResult<TResult1>> sourceAsync,
                                                                                                                                                                                                                           (TResult2 value1, TResult3 value2, TResult4 value3, TResult5 value4, TResult6 value5, TResult7 value6, TResult8 value7) values)
        => await (await sourceAsync).CombineAllAsync(values);






    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2)> CombineAll<TResult1, TResult2>(this TResult1           source, 
                                                                                                MlResult<TResult2> mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1 value1, TResult2 value2)>.Valid((source, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1 value1, TResult2 value2)>()
                           );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this TResult1           source,
                                                                                                           MlResult<TResult2> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2)>> CombineAllAsync<TResult1, TResult2>(this TResult1                 source,
                                                                                                                 Task<MlResult<TResult2>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)> CombineAll<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source, 
                                                                                                                           MlResult<TResult3>                 mlResultValue)
        => mlResultValue.Match(

                                valid: x            => MlResult<(TResult1, TResult2, TResult3)>.Valid((source.value1, source.value2, x)),
                                fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3)>()
                           );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                      MlResult<TResult3>                 mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3)>> CombineAllAsync<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                                                            Task<MlResult<TResult3>>           mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source, 
                                                                                                                                                       MlResult<TResult4>                                         mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4)>.Valid((source.value1, source.value2, source.value3, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4)>()
                               );

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                  MlResult<TResult4>                                  mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                                                                         Task<MlResult<TResult4>>                            mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source, 
                                                                                                                                                                                 MlResult<TResult5>                                                   mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)>.Valid((source.value1, source.value2, source.value3, source.value4, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5)>()
                               );


    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                             MlResult<TResult5>                                                   mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                                                                                   Task<MlResult<TResult5>>                                             mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                            MlResult<TResult6> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)>()
                               );


    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                        MlResult<TResult6> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                                                                                Task<MlResult<TResult6>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                        MlResult<TResult7> mlResultValue)
        => mlResultValue.Match(
                                    valid: x            => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, x)),
                                    fail : errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)>()
                               );


    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                    MlResult<TResult7> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                                                                                          Task<MlResult<TResult7>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                  MlResult<TResult8> mlResultValue)
        => mlResultValue.Match(
                                    valid: x => MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>.Valid((source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, x)),
                                    fail: errorDetails => errorDetails.ToMlResultFail<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)>()
                               );


    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                               MlResult<TResult8> mlResultValue)
        => source.CombineAll(mlResultValue).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>All</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="mlResultValueAsync">Valor de <c>mlResultValueAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<(TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7, TResult8 value8)>> CombineAllAsync<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                                                                                                     Task<MlResult<TResult8>> mlResultValueAsync)
        => await source.CombineAllAsync(await mlResultValueAsync);




    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2)> CombineAll<TResult1, TResult2>(this TResult1 source,
                                                                                  TResult2 value)
        => (source, value).ToMlResultValid();


    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3)> CombineAll<TResult1, TResult2, TResult3>(this (TResult1 value1, TResult2 value2) source,
                                                                                                      TResult3 newValue)
        => (source.value1, source.value2, newValue).ToMlResultValid();



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4)> CombineAll<TResult1, TResult2, TResult3, TResult4>(this (TResult1 value1, TResult2 value2, TResult3 value3) source,
                                                                                                                          TResult4 newValue)
        => (source.value1, source.value2, source.value3, newValue).ToMlResultValid();



    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4) source,
                                                                                                                                               TResult5 newValue)
        => (source.value1, source.value2, source.value3, source.value4, newValue).ToMlResultValid();

    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5) source,
                                                                                                                                                                  TResult6 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, newValue).ToMlResultValid();

    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6) source,
                                                                                                                                                                                      TResult7 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, newValue).ToMlResultValid();

    /// <summary>
    /// Combina los resultados de <c>All</c>.
    /// </summary>
    /// <typeparam name="TResult1">Tipo genérico TResult1.</typeparam>
    /// <typeparam name="TResult2">Tipo genérico TResult2.</typeparam>
    /// <typeparam name="TResult3">Tipo genérico TResult3.</typeparam>
    /// <typeparam name="TResult4">Tipo genérico TResult4.</typeparam>
    /// <typeparam name="TResult5">Tipo genérico TResult5.</typeparam>
    /// <typeparam name="TResult6">Tipo genérico TResult6.</typeparam>
    /// <typeparam name="TResult7">Tipo genérico TResult7.</typeparam>
    /// <typeparam name="TResult8">Tipo genérico TResult8.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="newValue">Nuevo valor que se asigna.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8)> CombineAll<TResult1, TResult2, TResult3, TResult4, TResult5, TResult6, TResult7, TResult8>(this (TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4, TResult5 value5, TResult6 value6, TResult7 value7) source,
                                                                                                                                                                                                          TResult8 newValue)
        => (source.value1, source.value2, source.value3, source.value4, source.value5, source.value6, source.value7, newValue).ToMlResultValid();



    #endregion




    #region Do


/// <summary>
/// Ejecuta <c>Pipe</c>.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
/// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
/// <param name="action">Acción que se ejecuta como efecto lateral.</param>
/// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> Pipe<T, TResult>(this MlResult<T> source, Func<MlResult<T>, MlResult<TResult>> action)
        => action(source);

/// <summary>
/// Ejecuta <c>Pipe</c>. Versión asíncrona.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
/// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
/// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
/// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> PipeAsync<T, TResult>(this MlResult<T>                                source, 
                                                                         Func<MlResult<T>, Task<MlResult<TResult>>> actionAsync)
        => await actionAsync(source);

/// <summary>
/// Ejecuta <c>Pipe</c>. Versión asíncrona.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
/// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
/// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
/// <param name="actionAsync">Acción asíncrona que se ejecuta como efecto lateral.</param>
/// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> PipeAsync<T, TResult>(this Task<MlResult<T>>                                sourceAsync, 
                                                                         Func<Task<MlResult<T>>, Task<MlResult<TResult>>> actionAsync)
        => await actionAsync(sourceAsync);


    #endregion






}

