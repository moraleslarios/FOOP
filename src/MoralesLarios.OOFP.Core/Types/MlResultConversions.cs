// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


using System.Reflection;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Types;
/// <summary>
/// Representa <c>MlResultConversions</c>.
/// </summary>
public static partial class MlResultConversions
{


    /// <summary>
    /// Ejecuta <c>ToMlResult</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ToMlResult<T, TResult>(this Func<T, TResult> source, 
                                                                T                value)
    {
        TResult partialResult = source(value);

        MlResult<TResult> result = partialResult;

        return result;
    }

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryToMlResult<T, TResult>(this Func<T, TResult> source, 
                                                                   T                value,
                                                                   string           exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TResult>(value, exceptionAdditionalMessage);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryToMlResult<T, TResult>(this Func<T, TResult>         source, 
                                                                   T                        value,
                                                                   Func<Exception, string>  errorMessageBuilder)
        => source.TryToInternalMlResult<T, TResult>(value, errorMessageBuilder);


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryToMlResult<T, TResult>(this Func<T, MlResult<TResult>> source, 
                                                                   T                          value,
                                                                   string                     exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TResult>(value, exceptionAdditionalMessage);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> TryToMlResult<T, TResult>(this Func<T, MlResult<TResult>> source, 
                                                                   T                          value,
                                                                   Func<Exception, string>    errorMessageBuilder)
        => source.TryToInternalMlResult<T, TResult>(value, errorMessageBuilder);



    /// <summary>
    /// Ejecuta <c>ToMlResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ToMlResultAsync<T, TResult>(this Func<T, Task<TResult>> sourceAsync, 
                                                                                 T                      value)
    {
        TResult partialResult = await sourceAsync(value);

        MlResult<TResult> result = partialResult;

        return result;
    }



    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryToMlResultAsync<T, TResult>(this Func<T, Task<TResult>> sourceAsync, 
                                                                                    T                      value,
                                                                                    string                 exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TResult>(value, exceptionAdditionalMessage);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryToMlResultAsync<T, TResult>(this Func<T, Task<TResult>>  sourceAsync, 
                                                                                    T                       value,
                                                                                    Func<Exception, string> errorMessageBuilder)
        => await sourceAsync.TryToInternalMlResultAsync<T, TResult>(value, errorMessageBuilder);


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="exceptionAdditionalMessage">Mensaje descriptivo.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryToMlResultAsync<T, TResult>(this Func<T, Task<MlResult<TResult>>> sourceAsync, 
                                                                                    T                                value,
                                                                                    string                           exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TResult>(value, exceptionAdditionalMessage);


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> TryToMlResultAsync<T, TResult>(this Func<T, Task<MlResult<TResult>>> sourceAsync, 
                                                                                    T                                value,
                                                                                    Func<Exception, string>          errorMessageBuilder)
        => await sourceAsync.TryToInternalMlResultAsync<T, TResult>(value, errorMessageBuilder);






    //public static MlResult<T> TryToMlResult<T>(this Func<T>                 source, 
    //                                                Func<Exception, string> errorMessageBuilder = null!)
    //{

    //    /// ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

    //    try
    //    {
    //        var result = source();

    //        return result.ToMlResultValid();
    //    }
    //    catch (Exception ex)
    //    {
    //        string message = FormatErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
    //                                               Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

    //        return errorDetails.ToMlResultFail<T>();
    //    }
    //}


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Func<T>                 source, 
                                                    Func<Exception, string> errorMessageBuilder = null!)
        => source.TryToInternalMlResult<T>(errorMessageBuilder);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Func<MlResult<T>>       source, 
                                                    Func<Exception, string> errorMessageBuilder = null!)
        => source.TryToInternalMlResult<T>(errorMessageBuilder);


    private static MlResult<T> TryToInternalMlResult<T>(this object                  source,
                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<T>           mySource => mySource(),
                Func<MlResult<T>> mySource => mySource(),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };

        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            result = errorDetails.ToMlResultFail<T>();
        }

        return result;
    }




    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Action<T>               source, 
                                                    T                       value,
                                                    Func<Exception, string> messageBuilder = null!)
    {
        // ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            source(value);

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }





    //public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync, 
    //                                                                 Func<Exception, string> errorMessageBuilder = null!)
    //{
    //    try
    //    {
    //        var result = await sourceAsync();

    //        return result.ToMlResultValid();
    //    }
    //    catch (Exception ex)
    //    {
    //        string message = FormatErrorMessage(errorMessageBuilder, ex);

    //        var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
    //                                    Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

    //        return errorDetails.ToMlResultFail<T>();
    //    }
    //}


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync, 
                                                                     Func<Exception, string> errorMessageBuilder = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T>(errorMessageBuilder);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<MlResult<T>>> sourceAsync, 
                                                                     Func<Exception, string> errorMessageBuilder = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T>(errorMessageBuilder);

    private static async Task<MlResult<T>> TryToInternalMlResultAsync<T>(this object                  source,
                                                                              Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<Task<T>>           mySource => await mySource(),
                Func<Task<MlResult<T>>> mySource => await mySource(),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(errorMessageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            result = errorDetails.ToMlResultFail<T>();
        }

        return result;
    }






    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="returnValue">Valor sobre el que se opera.</param>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<T, Task>           sourceAsync,
                                                                     T                       returnValue,
                                                                     Func<Exception, string> messageBuilder = null!)
    {
        try
        {
            await sourceAsync(returnValue);

            return returnValue.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                        Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result errors.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResultErrors<T>(this Action<MlErrorDetails> source,
                                                          MlErrorDetails         errorsDetails,
                                                          Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            source(errorsDetails);

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }



    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result errors. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultErrorsAsync<T>(this Func<MlErrorDetails, Task> sourceAsync,
                                                                           MlErrorDetails             errorsDetails,
                                                                           Func<Exception, string>     errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            await sourceAsync(errorsDetails);

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    /// <summary>
    /// Ejecuta <c>ToMlTaskResult</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ToMlTaskResult<T, TResult>(this Func<T, Task<TResult>> sourceAsync,
                                                                                T value)
    {
        TResult partialResult = await sourceAsync(value);

        MlResult<TResult> result = partialResult;

        return result;
    }










    //private static MlResult<TResult> ToInternalMlResultEx<T, TResult>(this object source, 
    //                                                                       T      value,
    //                                                                       string exceptionAdditionalMessage = null!)
    //    => source.ToInternalMlResultEx<T, TResult>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);

    //private static MlResult<TResult> ToInternalMlResultEx<T, TResult>(this object                  source, 
    //                                                                       T                       value,
    //                                                                       Func<Exception, string> errorMessageBuilder)
    //{
    //    MlResult<TResult> result = default!;

    //    try
    //    {
    //        result = source switch
    //        {
    //            Func<T, TResult>           mySource => mySource.ToMlResult(value),
    //            Func<T, MlResult<TResult>> mySource => mySource(value),
    //            _                                   => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
    //        };
    //    }
    //    catch (Exception ex)
    //    {
    //        string msg = FormatErrorMessage(errorMessageBuilder, ex);

    //        result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
    //                                     Metadata: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

    //    }

    //    return result;
    //}




    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorDetails,T> source,
                                                    MlErrorDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Func<MlErrorDetails, MlResult<T>> source,
                                                    MlErrorDetails                    errorsDetails,
                                                    Func<Exception, string>            errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorDetails,Task<T>> sourceAsync,
                                                               MlErrorDetails               errorsDetails,
                                                               Func<Exception, string>       errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<MlErrorDetails, Task<MlResult<T>>> sourceAsync,
                                                                     MlErrorDetails                          errorsDetails,
                                                                     Func<Exception, string>                  errorMessageBuilder)
        => await TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Func<T>                 source,
                                                    MlErrorDetails         errorsDetails,
                                                    Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResult<T>(source, errorsDetails, errorMessageBuilder);

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task<T>>           sourceAsync,
                                                               MlErrorDetails         errorsDetails,
                                                               Func<Exception, string> errorMessageBuilder)
        => TryToInternalMlResultAsync<T>(sourceAsync, errorsDetails, errorMessageBuilder);



    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResult<T>(this Action                  source, 
                                                    T                       value,
                                                    Func<Exception, string> messageBuilder = null!)
    {
        // ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            source();

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result errors.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> TryToMlResultErrors<T>(this Action                  source,
                                                          MlErrorDetails         errorsDetails,
                                                          Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            source();

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultAsync<T>(this Func<Task>              sourceAsync, 
                                                                     T                       value,
                                                                     Func<Exception, string> messageBuilder = null!)
    {
        // ********** Cuando ees una MlResult en Fail y con errores, no se añaden los nuevos errores

        try
        {
            await sourceAsync();

            return value.ToMlResultValid();
        }
        catch (Exception ex)
        {
            string message = FormatErrorMessage(messageBuilder, ex);

            var errorDetails = new MlErrorDetails(Errors : new List<MlError> { new MlError(message) },
                                                   Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });

            return errorDetails.ToMlResultFail<T>();
        }
    }



    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por to ml result errors. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <param name="errorMessageBuilder">Función a aplicar.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> TryToMlResultErrorsAsync<T>(this Func<Task>              sourceAsync,
                                                                           MlErrorDetails         errorsDetails,
                                                                           Func<Exception, string> errorMessageBuilder = null!)
    {
        MlResult<T> result = default!;

        try
        {
            await sourceAsync();

            result = errorsDetails.ToMlResultFail<T>();
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }





    private static MlResult<T> TryToInternalMlResult<T>(this object                  source,
                                                             MlErrorDetails         errorsDetails,
                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<T>                            mySource => mySource(),
                Func<MlErrorDetails, T>           mySource => mySource(errorsDetails),
                Func<MlErrorDetails, MlResult<T>> mySource => mySource(errorsDetails),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }


    private static async Task<MlResult<T>> TryToInternalMlResultAsync<T>(this object                  source,
                                                                              MlErrorDetails         errorsDetails,
                                                                              Func<Exception, string> errorMessageBuilder)
    {
        MlResult<T> result = default!;

        try
        {
            result = source switch
            {
                Func<Task<T>>                            mySource => await mySource(),
                Func<MlErrorDetails, Task<T>>           mySource => await mySource(errorsDetails),
                Func<MlErrorDetails, Task<MlResult<T>>> mySource => await mySource(errorsDetails),
                _ => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            result = errorsDetails.AppendExErrorDetail(ex, errorMessageBuilder);
        }

        return result;
    }













        

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string errorMessage, Exception ex)
        => string.IsNullOrWhiteSpace(errorMessage) ? DEFAULT_EX_ERROR_MESSAGE(ex) : errorMessage;

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="messageBuilder">Función a aplicar.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(Func<Exception, string> messageBuilder, Exception ex)
        => messageBuilder != null ? messageBuilder(ex) : DEFAULT_EX_ERROR_MESSAGE(ex);



    /// <summary>
    /// Ejecuta <c>ToMlResultValid</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultValid<T>(this T                    source) => source;
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this MlErrorDetails      source) => source;
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this MlError              source) => source;
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this List<MlError>        source) => source;
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this List<string>         source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this MlError[]            source) => source;
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this string[]             source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<MlError> source) => new MlErrorDetails(source);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this IEnumerable<string>  source) => MlErrorDetails.FromErrorsMessagesDetails(source);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorDetails(source.Item1, source.Item2);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorDetails.FromErrorsMessagesDetails(source.Item1, source.Item2);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorDetails(new List<MlError> { source.Item1 }, source.Item2);
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> ToMlResultFail <T>(this (string              , Dictionary<string, object>) source) => MlErrorDetails.FromErrorMessageDetails(source.Item1, source.Item2);

    /// <summary>
    /// Ejecuta <c>ToMlResultValid</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultValidAsync<T>(this T                    source) => source.ToMlResultValid().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlErrorDetails      source) => source.ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError              source) => source.ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string               source) => MlError.FromErrorMessage(source).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<MlError>        source) => source.ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this List<string>         source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this MlError[]            source) => source.ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this string[]             source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<MlError> source) => new MlErrorDetails(source).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this IEnumerable<string>  source) => MlErrorDetails.FromErrorsMessagesDetails(source).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<MlError>, Dictionary<string, object>) source) => new MlErrorDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (IEnumerable<string> , Dictionary<string, object>) source) => MlErrorDetails.FromErrorsMessagesDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (MlError             , Dictionary<string, object>) source) => new MlErrorDetails(new List<MlError> { source.Item1 }, source.Item2).ToMlResultFail<T>().ToAsync();
    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> ToMlResultFailAsync <T>(this (string              , Dictionary<string, object>) source) => MlErrorDetails.FromErrorMessageDetails(source.Item1, source.Item2).ToMlResultFail<T>().ToAsync();




    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<TResult> ToMlResultFail<T, TResult>(this MlResult<T> source)
        => source.Match(
                            fail: errorDetails => errorDetails,
                            valid: _           => MlResult<TResult>.Fail("Don't change MlResult Fail of valid source.")
                        );

    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<TResult>> ToMlResultFailAsync<T, TResult>(this MlResult<T> source)
        => source.ToMlResultFail<T, TResult>().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToMlResultFail</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<TResult>> ToMlResultFailAsync<T, TResult>(this Task<MlResult<T>> sourceAsync)
        => await (await sourceAsync).ToMlResultFailAsync<T, TResult>();



    /// <summary>
    /// Ejecuta <c>SecureGetValueFromMlResultBoxed</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>object</c> obtenido por la operación.</returns>
    public static object SecureGetValueFromMlResultBoxed(this object source)
    {
        var properties = source.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        var partialResult = MlResult.Empty()
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == nameof(MlResult<object>.IsSuccess) && p.PropertyType == typeof(bool)))
                                        .MapEnsure(isValidProperty => isValidProperty is not null,
                                                    _              => $"The encapsulated MlResult object does not have a property '{nameof(MlResult<object>.IsSuccess)}' of type bool. The type isn't MlResult.")
                                        .Map      (isValidProperty => isValidProperty!.GetValue(source))
                                        .MapEnsure(isValidObj      => isValidObj is bool,
                                                    _              => $"The '{nameof(MlResult<object>.IsSuccess)}' property is not of type bool")
                                        .MapEnsure(isValid         => (bool)isValid!,
                                                    _              => $"The encapsulated MlResult object in obj, from which you want to extract the '{nameof(MlResult<object>.SuccessValue)}' property, is in a Fail state, so this information cannot be retrieved.")
                                        .Map      ( _              => properties.FirstOrDefault(p => p.Name == nameof(MlResult<object>.SuccessValue)))
                                        .MapEnsure(valueProperty   => valueProperty is not null,
                                                    _              => $"The source object does not have a property '{nameof(MlResult<object>.SuccessValue)}'")
                                        .Map      ( valueProperty  => valueProperty!.GetValue(source));

        var result = partialResult.IsSuccess ? partialResult.SuccessValue : throw new ArgumentException(partialResult.FailureDetails.ToString());

        return result!;
    }

    /// <summary>
    /// Ejecuta <c>SecureGetValueFromMlResultBoxed</c>. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>object</c> obtenido por la operación.</returns>
    public static Task<object> SecureGetValueFromMlResultBoxedAsync(this object source)
        => source.SecureGetValueFromMlResultBoxed().ToAsync();

    /// <summary>
    /// Ejecuta <c>SecureGetValueFromMlResultBoxed</c>. Versión asíncrona.
    /// </summary>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el valor de tipo <c>object</c> obtenido por la operación.</returns>
    public static async Task<object> SecureGetValueFromMlResultBoxedAsync(this Task<object> sourceAsync)
        => await (await sourceAsync).SecureGetValueFromMlResultBoxedAsync();


    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<object> ToMlResultObject<T>(this MlResult<T> source)
    {
        var result = source.Match(
                            fail : errorDetails => errorDetails    .ToMlResultFail <object>(),
                            valid: value        => ((object)value!).ToMlResultValid()
                        );
        return result;
    }
    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<object>> ToMlResultObjectAsync<T>(this MlResult<T> source)
        => source.ToMlResultObject().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<object>> ToMlResultObjectAsync<T>(this Task<MlResult<T>> sourceAsync)
        => await (await sourceAsync).ToMlResultObjectAsync();


    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<object> ToMlResultObject<T>(this T source) => ((object)source!).ToMlResultValid();

    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<object>> ToMlResultObjectAsync<T>(this T source) => source.ToMlResultObject().ToAsync();

    /// <summary>
    /// Ejecuta <c>ToMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<object>> ToMlResultObjectAsync<T>(this Task<T> sourceAsync) => await (await sourceAsync).ToMlResultObjectAsync();


    /// <summary>
    /// Ejecuta <c>FromMlResultObject</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> FromMlResultObject<T>(this MlResult<object> source)
    {
        var result = source.Match(
                                    fail : errorDetails => errorDetails.ToMlResultFail<T>(),
                                    valid: value        => (value is T tValue) ? tValue.ToMlResultValid() : MlResult<T>.Fail($"The value '{value}' of type '{value?.GetType()}' cannot be cast to the requested type '{typeof(T)}'.")
                                );
        return result;
    }

    /// <summary>
    /// Ejecuta <c>FromMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<T>> FromMlResultObjectAsync<T>(this MlResult<object> source)
        => source.FromMlResultObject<T>().ToAsync();

    /// <summary>
    /// Ejecuta <c>FromMlResultObject</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> FromMlResultObjectAsync<T>(this Task<MlResult<object>> sourceAsync)
        => await (await sourceAsync).FromMlResultObjectAsync<T>();









    private static MlResult<TResult> TryToInternalMlResult<T, TResult>(this object source, 
                                                                            T      value,
                                                                            string exceptionAdditionalMessage = null!)
        => source.TryToInternalMlResult<T, TResult>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);

    private static MlResult<TResult> TryToInternalMlResult<T, TResult>(this object                  source, 
                                                                            T                       value,
                                                                            Func<Exception, string> errorMessageBuilder)
    {
        MlResult<TResult> result = default!;

        try
        {
            result = source switch
            {
                Func<T, TResult>           mySource => mySource.ToMlResult(value),
                Func<T, MlResult<TResult>> mySource => mySource(value),
                _                                   => throw new ArgumentException($"The type {source.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {
            string msg = FormatErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });
            
        }

        return result;
    }






    private static async Task<MlResult<TResult>> TryToInternalMlResultAsync<T, TResult>(this object sourceAsync, 
                                                                                             T      value,
                                                                                             string exceptionAdditionalMessage = null!)
        => await sourceAsync.TryToInternalMlResultAsync<T, TResult>(value, ex => exceptionAdditionalMessage ?? DEFAULT_ERROR_MESSAGE);


    private static async Task<MlResult<TResult>> TryToInternalMlResultAsync<T, TResult>(this object                  sourceAsync, 
                                                                                             T                       value,
                                                                                             Func<Exception, string> errorMessageBuilder)
    {
        MlResult<TResult> result = default!;

        try
        {
            result = sourceAsync switch
            {
                Func<T, Task<TResult>>           mySource => await mySource.ToMlResultAsync(value),
                Func<T, Task<MlResult<TResult>>> mySource => await mySource(value),
                _                                         => throw new ArgumentException($"The type {sourceAsync.GetType()} is not a valid type")
            };
        }
        catch (Exception ex)
        {


            string msg = FormatErrorMessage(errorMessageBuilder, ex);

            result = new MlErrorDetails(Errors : new List<MlError> { new MlError(msg) },
                                         Details: new Dictionary<string, object> { { ExceptionMetadataKey, ex } });
            
        }

        return result;
    }






}

