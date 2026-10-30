// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;

/// <summary>
/// Operaciones sobre <see cref="MlErrorDetails"/>: incorporación de los datos de una excepción como metadatos, fusión de los errores de varios resultados y propagación de esos errores a resultados de otro tipo.
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
    /// <remarks>
    /// La fusión nunca pierde información ni lanza: los errores se concatenan en orden y, cuando una
    /// clave de metadatos existe en los dos conjuntos, la entrante se sufija con el primer índice
    /// libre (<c>ParamName</c>, <c>ParamName2</c>…). Las excepciones conservan su secuencia reservada
    /// <c>Ex</c>, <c>Ex2</c>, <c>Ex3</c>… sin renumerar las ya presentes en <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails Merge(this MlErrorDetails source, 
                                             MlErrorDetails other)
    {
        var errors = (source?.Errors ?? Enumerable.Empty<MlError>())
                        .Concat(other?.Errors ?? Enumerable.Empty<MlError>())
                        .ToList();

        var details = MlErrorDetails.CopyMetadata(null);

        MlErrorDetails.MergeMetadataInto(details, source?.Metadata);
        MlErrorDetails.MergeMetadataInto(details, other ?.Metadata);

        var result = (errors, details);

        return result;
    }



    /// <summary>
    /// Combina los resultados.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Acumula en una sola pasada: el coste es lineal en el número total de errores y metadatos, no
    /// cuadrático en el número de elementos de <paramref name="errorsDetails"/>.
    /// </remarks>
    public static MlErrorDetails Merge(this MlErrorDetails              source,
                                             IEnumerable<MlErrorDetails> errorsDetails)
    {
        var errors = new List<MlError>(source?.Errors ?? Enumerable.Empty<MlError>());

        var details = MlErrorDetails.CopyMetadata(null);

        MlErrorDetails.MergeMetadataInto(details, source?.Metadata);

        if (errorsDetails is not null)
        {
            foreach (var errorDetails in errorsDetails)
            {
                if (errorDetails is null) continue;

                errors.AddRange(errorDetails.Errors);

                MlErrorDetails.MergeMetadataInto(details, errorDetails.Metadata);
            }
        }

        var result = (errors, details);

        return result;
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
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
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
    /// Devuelve unos detalles de error nuevos con el error indicado añadido a la colección existente, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddError(this MlErrorDetails source, MlError error) 
        => (source.Errors.Append(error).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));
    /// <summary>
    /// Devuelve unos detalles de error nuevos con un error construido a partir del mensaje indicado, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddErrorMessage(this MlErrorDetails source, string errorMessage) 
        => (source.Errors.Append(errorMessage).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));
    /// <summary>
    /// Devuelve unos detalles de error nuevos con la colección de errores indicada concatenada a la existente, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddErrors(this MlErrorDetails source, IEnumerable<MlError> errors) 
        => (source.Errors.Concat(errors).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));

    /// <summary>
    /// Devuelve unos detalles de error nuevos con los errores indicados concatenados a la colección existente, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddErrors(this MlErrorDetails source, params MlError[] errors) 
        => (source.Errors.Concat(errors).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));
    /// <summary>
    /// Devuelve unos detalles de error nuevos con los errores construidos a partir de la colección de mensajes indicada, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, IEnumerable<string> errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));

    /// <summary>
    /// Devuelve unos detalles de error nuevos con los errores construidos a partir de los mensajes indicados, conservando los metadatos.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Los metadatos se copian: la instancia devuelta no comparte el diccionario con
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddErrorsMessages(this MlErrorDetails source, params string[] errorMessages) 
        => (source.Errors.Concat(errorMessages.ToMlErrors()).ToList(), MlErrorDetails.CopyMetadata(source.Metadata));

    /// <summary>
    /// Añade el par clave-valor indicado al diccionario de metadatos del error; lanza una excepción si la clave ya existe.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Devuelve una instancia nueva y <b>no</b> modifica <paramref name="source"/>. Si la clave ya
    /// existe lanza <see cref="ArgumentException"/>; use
    /// <see cref="MlErrorDetails.WithMetadata(string, object)"/> cuando quiera sobrescribir sin
    /// riesgo de excepción.
    /// </remarks>
    /// <exception cref="ArgumentException">La clave ya existe en los metadatos de <paramref name="source"/>.</exception>
    public static MlErrorDetails AddMetadataOrThrow<T>(this MlErrorDetails source, string key, T value)    
    {
        var details = MlErrorDetails.CopyMetadata(source.Metadata);

        details.Add(key, value!);

        var result = (source.Errors, details);

        return result;
    }

    /// <summary>
    /// Incorpora a los metadatos del error todas las entradas del diccionario indicado.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Valor de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Las claves que ya existan se sobrescriben con el valor entrante. Para conservar los dos
    /// valores use <see cref="Merge(MlErrorDetails, MlErrorDetails)"/>. No modifica
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, Dictionary<string, object> otherDetails)    
        => source.WithMetadata(otherDetails);

    /// <summary>
    /// Incorpora a los metadatos del error todas las parejas clave-valor indicadas.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="otherDetails">Colección de <c>otherDetails</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>
    /// Las claves que ya existan se sobrescriben con el valor entrante. Para conservar los dos
    /// valores use <see cref="Merge(MlErrorDetails, MlErrorDetails)"/>. No modifica
    /// <paramref name="source"/>.
    /// </remarks>
    public static MlErrorDetails AddMetadataRange(this MlErrorDetails source, params (string key, object value)[] otherDetails)
        => source.WithMetadata(otherDetails?.Select(x => new KeyValuePair<string, object>(x.key, x.value)));





    /// <summary>
    /// Registra el valor indicado en los metadatos del error, bajo la clave estándar de valor, sustituyendo el que hubiera.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    /// <remarks>Devuelve una instancia nueva y no modifica <paramref name="source"/>.</remarks>
    public static MlErrorDetails AddValueMetadata<T>(this MlErrorDetails source, T value)
        => source.WithMetadata(MlErrorDetails.MetadataKeys.Value, value!);

    /// <summary>
    /// Indica si los metadatos del error contienen la clave estándar de valor.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsValueMetadata(this MlErrorDetails source) => source.HasMetadata(MlErrorDetails.MetadataKeys.Value);

    /// <summary>
    /// Indica si los metadatos del error contienen la clave estándar de valor. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>
    /// Tarea que produce <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<bool> ContainsValueMetadataAsync(this MlErrorDetails source) => source.ContainsValueMetadata().ToAsync();

    /// <summary>
    /// Indica si los metadatos del error contienen la clave canónica de excepción.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsException(this MlErrorDetails source) => source.HasMetadata(MlErrorDetails.MetadataKeys.Exception);

    /// <summary>
    /// Indica si los metadatos del error contienen la clave canónica de excepción. Versión asíncrona.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>
    /// Tarea que produce <c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<bool> ContainsExceptionAsync(this MlErrorDetails source) => source.ContainsException().ToAsync();

    /// <summary>
    /// Indica si los metadatos del error contienen la clave indicada.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool ContainsMetadata(this MlErrorDetails source, string key) => source.HasMetadata(key);
}

