// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;



/// <summary>
/// Conjunto de errores de una operación fallida junto con los metadatos (pares clave/valor) que aportan el contexto adicional del fallo.
/// </summary>
/// <remarks>
/// <para>Contrato de los metadatos:</para>
/// <list type="bullet">
///   <item><description>
///     <b>Comparador de claves</b>: <see cref="StringComparer.Ordinal"/>. Las claves distinguen
///     mayúsculas y minúsculas.
///   </description></item>
///   <item><description>
///     <b>Claves reservadas</b>: las claves bien conocidas están centralizadas en
///     <see cref="MetadataKeys"/> (<c>Ex</c>, <c>Value</c>, <c>ParamName</c>, <c>Expected</c>,
///     <c>FailedIndexes</c>, <c>NotFound</c> y <c>ProblemsDetails</c>).
///   </description></item>
///   <item><description>
///     <b>Prefijo <c>Ex</c> reservado</b>: las excepciones acumuladas ocupan la secuencia
///     <c>Ex</c>, <c>Ex2</c>, <c>Ex3</c>… Sólo esas claves exactas (el patrón <c>^Ex\d*$</c>) se
///     consideran ranuras de excepción, de modo que claves propias como <c>Expected</c> o
///     <c>ExternalId</c> no interfieren.
///   </description></item>
///   <item><description>
///     <b>Política de colisión</b>: al fusionar dos instancias con la misma clave no se pierde ni
///     se sobrescribe nada; la clave entrante se sufija (<c>ParamName</c>, <c>ParamName2</c>…).
///     La fusión nunca lanza.
///   </description></item>
///   <item><description>
///     <b>Independencia</b>: cada instancia posee su propio diccionario. Ninguna operación de la
///     biblioteca comparte el diccionario entre dos instancias.
///   </description></item>
/// </list>
/// <para>
/// La vía de lectura recomendada son <see cref="TryGetMetadata{T}(string, out T)"/>,
/// <see cref="HasMetadata(string)"/>, <see cref="TryGetException(out Exception)"/>,
/// <see cref="GetExceptions"/> y <see cref="MetadataView"/>; la de escritura,
/// <see cref="WithMetadata(string, object)"/>.
/// </para>
/// </remarks>
public partial class MlErrorDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
{
    /// <summary>
    /// Obtiene la colección de errores que describen el fallo. La colección está materializada:
    /// enumerarla varias veces no reejecuta ninguna proyección.
    /// </summary>
    public IEnumerable<MlError>       Errors  { get; init; } = Errors is null ? new List<MlError>() : Errors.ToList();
    /// <summary>
    /// Obtiene los metadatos que aportan el contexto adicional del fallo.
    /// </summary>
    /// <remarks>
    /// La vía de lectura recomendada es <see cref="MetadataView"/> y la de escritura,
    /// <see cref="WithMetadata(string, object)"/>. El diccionario es una copia independiente del que
    /// se pasó al constructor: ninguna instancia comparte sus metadatos con otra.
    /// </remarks>
    public Dictionary<string, object> Metadata { get; init; } = CopyMetadata(Details);

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MlErrorDetails"/>.
    /// </summary>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <param name="details">Información adicional asociada.</param>
    protected MlErrorDetails(IEnumerable<string> errorMessages, 
                              Dictionary<string, object> details) : this()
    {
        Errors  = errorMessages is null ? new List<MlError>() : errorMessages.Select(x => MlError.FromErrorMessage(x)).ToList();
        Metadata = CopyMetadata(details);
    }


    /// <summary>
    /// Ejecuta <c>FromErrorsMessagesDetails</c>.
    /// </summary>
    /// <param name="errorsMessages">Colección de <c>errorsMessages</c>.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorsMessagesDetails  (IEnumerable<string> errorsMessages, Dictionary<string, object> details = null!) => new(errorsMessages, details);
    /// <summary>
    /// Ejecuta <c>FromErrorMessageDetails</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageDetails    (string              errorMessage  , Dictionary<string, object> details = null!) => new(new List<MlError>{ MlError.FromErrorMessage(errorMessage) }, details);
    /// <summary>
    /// Ejecuta <c>FromEnumerableErrors</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromEnumerableErrors       (IEnumerable<MlError> errors                                                   ) => new(errors);
    /// <summary>
    /// Ejecuta <c>FromEnumerableErrors</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromEnumerableErrors       (IEnumerable<string>  errors                                                   ) => new(errors, null!);
    /// <summary>
    /// Ejecuta <c>FromEnumerableErrorsDetails</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromEnumerableErrorsDetails(IEnumerable<MlError> errors       , Dictionary<string, object> details = null!) => new(errors, details);
    /// <summary>
    /// Ejecuta <c>FromEnumerableStrings</c>.
    /// </summary>
    /// <param name="errorsMessages">Colección de <c>errorsMessages</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromEnumerableStrings      (IEnumerable<string> errorsMessages                                            ) => FromErrorsMessagesDetails(errorsMessages);
    /// <summary>
    /// Ejecuta <c>FromError</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromError                  (MlError             error                                                     ) => new(new List<MlError> { error });
    /// <summary>
    /// Ejecuta <c>FromErrorDetails</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorDetails           (MlError             error         , Dictionary<string, object> details = null!) => new(new List<MlError> { error }, details);
    /// <summary>
    /// Ejecuta <c>FromErrorDetails</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="key">Clave identificativa.</param>
    /// <param name="detail">Valor de <c>detail</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorDetails           (string              errorMessage  , string key, object detail                 ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { key, detail} }); 
    /// <summary>
    /// Ejecuta <c>FromErrorMessage</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessage           (string              errorMessage                                              ) => FromErrorMessageDetails(errorMessage);

    /// <summary>
    /// Ejecuta <c>FromErrorMessageWithException</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="exception">Excepción capturada.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageWithException(string errorMessage, Exception exception) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { Constants.ExceptionMetadataKey, exception} });
    /// <summary>
    /// Ejecuta <c>FromErrorMessageWithValue</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageWithValue    (string errorMessage, object    value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { Constants.ValueMetadataKey , value     } });
    /// <summary>
    /// Ejecuta <c>FromErrorMessageWithValue</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageWithValue<T> (string errorMessage, T         value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { Constants.ValueMetadataKey , value!    } });


    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(List<MlError> errors        ) => new(errors);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(MlError[]     errors        ) => new(errors);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorsMessages">Colección de <c>errorsMessages</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(List<string>  errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorsMessages">Colección de <c>errorsMessages</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(string[]      errorsMessages) => FromErrorsMessagesDetails(errorsMessages);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(MlError       error         ) => new(new List<MlError> { error });
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails(string        errorMessage  ) => FromErrorMessageDetails(errorMessage);

    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails((IEnumerable<MlError>, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails((IEnumerable<string >, Dictionary<string, object>) errorDetails) => new(errorDetails.Item1, errorDetails.Item2);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails((MlError             , Dictionary<string, object>) errorDetails) => new(new List<MlError> { errorDetails.Item1 }, errorDetails.Item2);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails((string              , Dictionary<string, object>) errorDetails) => FromErrorMessageDetails(errorDetails.Item1, errorDetails.Item2);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlErrorDetails</c>.
    /// </summary>
    /// <param name="errorDetail">Valor de <c>errorDetail</c>.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static implicit operator MlErrorDetails((string              , string, object)             errorDetail ) => FromErrorMessageDetails(errorDetail.Item1, new Dictionary<string, object>() { { errorDetail.Item2, errorDetail.Item3} });



    /// <inheritdoc/>
    /// <remarks>
    /// Recorre los errores y los metadatos una sola vez: la cabecera de cada sección se escribe al
    /// encontrar su primer elemento, de modo que no se enumera la colección para comprobar si está
    /// vacía.
    /// </remarks>
    public override string ToString()
    {
        StringBuilder sb = new();

        var hasErrors = false;

        foreach (var error in Errors)
        {
            if ( ! hasErrors)
            {
                sb.AppendLine(EnsureFpMessages.ErrorsSectionHeader);
                hasErrors = true;
            }

            sb.AppendLine($"     {error.Message}");
        }

        var hasMetadata = false;

        foreach (var detail in Metadata)
        {
            if ( ! hasMetadata)
            {
                sb.AppendLine(EnsureFpMessages.MetadataSectionHeader);
                hasMetadata = true;
            }

            sb.AppendLine($"     {detail.Key}: {detail.Value}");
        }

        var result = sb.ToString().TrimEnd();

        return result;
    }


}


/// <summary>
/// Métodos de extensión para crear y transformar instancias de <see cref="MlErrorDetails"/>.
/// </summary>
public static partial class MlErrorDetailsExtensions
{


    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this IEnumerable<MlError> errors) => new(errors);
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this (IEnumerable<MlError> errors, Dictionary<string, object> details) source) => new(source.errors, source.details);
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this MlError error) => new(error.ToMlErrors());
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this (MlError error, Dictionary<string, object> details) source) => new(source.error.ToMlErrors(), source.details);
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this IEnumerable<string> errorMessages) => new(errorMessages.ToMlErrors());
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this (IEnumerable<string> errorsMessages, Dictionary<string, object> details) source) => new(source.errorsMessages.ToMlErrors(), source.details);
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this string errorMessage) => new(errorMessage.ToMlErrors());
    /// <summary>
    /// Ejecuta <c>ToMlErrorsDetails</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails ToMlErrorsDetails(this (string errorMessage, Dictionary<string, object> details) source) => new(source.errorMessage.ToMlErrors(), source.details);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>ErrorMessages</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    public static IEnumerable<string> GetErrorMessages(this MlErrorDetails source) => source.Errors.Select(x => x.Message);



    /// <summary>
    /// Ejecuta <c>ToDescription</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string ToDescription(this IEnumerable<string> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ErrorsSummary</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string GetErrorsSummary(this IEnumerable<MlError> source) 
        => source.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ErrorsSummary</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string GetErrorsSummary(this MlErrorDetails source) 
        => source.Errors.Count() > 1 
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, source.Errors)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, source.Errors);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>MetadataSummary</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string GetMetadataSummary(this MlErrorDetails source)
    {
        var errorData = source.Metadata.Select(x => $"{x.Key} : {x.Value}");
        return errorData.Count() > 1
                ? $"{Environment.NewLine}{string.Join(Environment.NewLine, errorData)}{Environment.NewLine}"
                : string.Join(Environment.NewLine, errorData);
    }

    /// <summary>
    /// Ejecuta <c>ToErrorsDetailsDescription</c>.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string ToErrorsDetailsDescription(this MlErrorDetails source) => source.ToString();






}
