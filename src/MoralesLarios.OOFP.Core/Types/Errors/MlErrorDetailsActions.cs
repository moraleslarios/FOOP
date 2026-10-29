// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;

/// <summary>
/// Representa <c>MlErrorDetailsActions</c>.
/// </summary>
public static partial class MlErrorDetailsActions
{

    /// <summary>
    /// Añade el elemento al final de <c>ExDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Valor de tipo <c>Dictionary&lt;string, object&gt;</c> obtenido por la operación.</returns>
    public static Dictionary<string, object> AppendExDetails(this MlErrorDetails source, Exception ex) => source.Metadata.AppendExDetails(ex);



    /// <summary>
    /// Añade el elemento al final de <c>ExDetailsToMlDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AppendExDetailsToMlDetails(this MlErrorDetails source, Exception ex) => (source.Errors,  source.Metadata.AppendExDetails(ex));

    /// <summary>
    /// Añade el elemento al final de <c>ExDetailsToMlDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AppendExDetailsToMlDetails(this MlErrorDetails source, 
                                                                  string          message, 
                                                                  Exception       ex) 
        => (source.Errors.Append(message),  source.Metadata.AppendExDetails(ex));





    /// <summary>
    /// Añade el elemento al final de <c>ExErrorDetail</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AppendExErrorDetail(this MlErrorDetails source, 
                                                           Exception       ex,
                                                           Func<Exception, string> errorMessageBuilder = null!)
        => source.AppendExDetailsToMlDetails(errorMessageBuilder != null ? errorMessageBuilder(ex) : EnsureFpMessages.DefaultExceptionError(ex), ex);

    /// <summary>
    /// Añade el elemento al final de <c>ExErrorDetail</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <param name="message">Mensaje descriptivo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AppendExErrorDetail(this MlErrorDetails source,
                                                           Exception ex,
                                                           string message = null!)
        => source.AppendExDetailsToMlDetails(message != null ? message : EnsureFpMessages.DefaultExceptionError(ex), ex);



    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="other">Segundo resultado que participa en la operación.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails Merge(this MlErrorDetails source, 
                                             MlErrorDetails other)
    {
        var errors = source.Errors.Concat(other.Errors).ToList();

        var sourceDetails = source?.Metadata ?? new Dictionary<string, object>();
        var otherDetails  = other ?.Metadata ?? new Dictionary<string, object>();

        var principalDetailsWithEx = sourceDetails.Where(x => x.Value is Exception)
                                           .Concat(otherDetails.Where(x => x.Value is Exception))
                                           .Select((x, index) => ($"{Constants.ExceptionMetadataKey}{(index == 0 ? string.Empty : index.ToString())}", x.Value))
                                           .ToDictionary(x => x.Item1, x => x.Item2);

        //var printipalDetailsWitoutEx = source.Metadata.Where(x => x.Key != Constants.ExceptionMetadataKey).ToDictionary(x => x.Key, x => x.SuccessValue);
        //var otherDetailsWitoutEx     = other .Metadata.Where(x => x.Key != Constants.ExceptionMetadataKey).ToDictionary(x => x.Key, x => x.SuccessValue);

        var printipalDetailsWitoutEx = sourceDetails.Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);
        var otherDetailsWitoutEx     = otherDetails .Where(x => x.Value is not Exception).ToDictionary(x => x.Key, x => x.Value);

        var details = printipalDetailsWitoutEx.Concat(otherDetailsWitoutEx)
                                              .Concat(principalDetailsWithEx).ToDictionary(x => x.Key, x => x.Value);

        var result = (errors, details);

        return result;
    }



    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails Merge(this MlErrorDetails              source,
                                             IEnumerable<MlErrorDetails> errorsDetails)
    {
        foreach (var errorDetails in errorsDetails)
        {
            source = source.Merge(errorDetails);
        }

        return source;
    }



    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> MergeErrorsDetails<T, TResult>(this MlErrorDetails source,
                                                                        MlResult<T>     secondary)
        => secondary.Match(
                                fail : source.Merge,
                                valid: _ => source
                           );

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> MergeErrorsDetailsAsync<T, TResult>(this MlErrorDetails source,
                                                                                   MlResult<T>     secondary)
        => source.MergeErrorsDetails<T, TResult>(secondary).ToAsync();

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondary">Resultado sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MergeErrorsDetailsAsync<T, TResult>(this Task<MlErrorDetails> sourceAsync,
                                                                                         MlResult<T>           secondary)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TResult>(secondary);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MergeErrorsDetailsAsync<T, TResult>(this MlErrorDetails   source,
                                                                                   Task<MlResult<T>> secondaryAsync)
        => source.MergeErrorsDetails<T, TResult>(await secondaryAsync);

    /// <summary>
    /// Combina los resultados de <c>ErrorsDetails</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="secondaryAsync">Valor de <c>secondaryAsync</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> MergeErrorsDetailsAsync<T, TResult>(this Task<MlErrorDetails> sourceAsync,
                                                                                         Task<MlResult<T>>     secondaryAsync)
        => await (await sourceAsync).MergeErrorsDetailsAsync<T, TResult>(await secondaryAsync);




    /// <summary>
    /// Añade el elemento indicado de <c>Error</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddError(this MlErrorDetails source, MlError error) => (source.Errors.Append(error), source.Metadata);
    /// <summary>
    /// Añade el elemento indicado de <c>ErrorMessage</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddErrorMessage(this MlErrorDetails source, string errorMessage) => (source.Errors.Append(errorMessage), source.Metadata);
    /// <summary>
    /// Añade el elemento indicado de <c>Errors</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddErrors(this MlErrorDetails source, IEnumerable<MlError> errors) 
        => (source.Errors.Concat(errors).ToList(), source.Metadata);

    /// <summary>
    /// Añade el elemento indicado de <c>Errors</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddErrors(this MlErrorDetails source, params MlError[] errors) 
        => (source.Errors.Concat(errors).ToList(), source.Metadata);
    /// <summary>
    /// Añade el elemento indicado de <c>ErrorsMessages</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, IEnumerable<string> errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Metadata);

    /// <summary>
    /// Añade el elemento indicado de <c>ErrorsMessages</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, params string[] errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), source.Metadata);

    /// <summary>
    /// Añade el elemento indicado de <c>MetadataOrThrow</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddMetadataOrThrow<T>(this MlErrorDetails source, string key, T value)    
    {
        source.Metadata.Add(key, value!);

        var result = (source.Errors, source.Metadata);

        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>MetadataRange</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Valor de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, Dictionary<string, object> otherDetails)    
    {
        var details = source.Metadata.Concat(otherDetails).ToDictionary(x => x.Key, x => x.Value);

        var result = (source.Errors, source.Metadata);

        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>MetadataRange</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Colección de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, params (string key, object value)[] otherDetails)
    {
        var details = source.Metadata.Concat(otherDetails.ToDictionary(x => x.key, x => x.value));

        var result = (source.Errors, source.Metadata);

        return result;
    }





    /// <summary>
    /// Añade el elemento indicado de <c>ValueMetadata</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails AddValueMetadata<T>(this MlErrorDetails source, T value)
    {
        if (source.Metadata.ContainsKey(Constants.ValueMetadataKey)) source.Metadata.Remove(Constants.ValueMetadataKey);

        var result = source.AddMetadataOrThrow(Constants.ValueMetadataKey, value!);

        return result;
    }



    /// <summary>
    /// Ejecuta <c>ContainsValueMetadata</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsValueMetadata(this MlErrorDetails source) => source.Metadata.ContainsKey(Constants.ValueMetadataKey);

    /// <summary>
    /// Ejecuta <c>ContainsValueMetadata</c>. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static Task<bool> ContainsValueMetadataAsync(this MlErrorDetails source) => source.ContainsValueMetadata().ToAsync();


    /// <summary>
    /// Ejecuta <c>ContainsException</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsException(this MlErrorDetails source) => source.Metadata.ContainsKey(Constants.ExceptionMetadataKey);

    /// <summary>
    /// Ejecuta <c>ContainsException</c>. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static Task<bool> ContainsExceptionAsync(this MlErrorDetails source) => source.ContainsException().ToAsync();

    /// <summary>
    /// Ejecuta <c>ContainsMetadata</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsMetadata(this MlErrorDetails source, string key) => source.Metadata.ContainsKey(key);



}

