// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Types.Errors;



/// <summary>
/// Representa <c>MlErrorDetails</c>.
/// </summary>
public partial class MlErrorDetails(IEnumerable<MlError>       Errors  = null!,
                             Dictionary<string, object> Details = null!)
{
    /// <summary>
    /// Obtiene o establece el valor de <c>Errors</c>.
    /// </summary>
    public IEnumerable<MlError>       Errors  { get; init; } = Errors  ?? new List<MlError>();
    /// <summary>
    /// Obtiene o establece el valor de <c>Metadata</c>.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = Details ?? new();

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="MlErrorDetails"/>.
    /// </summary>
    /// <param name="errorMessages">Mensajes descriptivos de los errores.</param>
    /// <param name="details">Información adicional asociada.</param>
    protected MlErrorDetails(IEnumerable<string> errorMessages, 
                              Dictionary<string, object> details) : this()
    {
        Errors  = errorMessages.Select(x => MlError.FromErrorMessage(x));
        Metadata = details ?? new Dictionary<string, object>();
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
    public static MlErrorDetails FromErrorMessageWithException(string errorMessage, Exception exception) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ExceptionMetadataKey, exception} });
    /// <summary>
    /// Ejecuta <c>FromErrorMessageWithValue</c>.
    /// </summary>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageWithValue    (string errorMessage, object    value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ValueMetadataKey , value     } });
    /// <summary>
    /// Ejecuta <c>FromErrorMessageWithValue</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorMessage">Mensaje descriptivo del error.</param>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>MlErrorDetails</c> obtenido por la operación.</returns>
    public static MlErrorDetails FromErrorMessageWithValue<T> (string errorMessage, T         value    ) => FromErrorMessageDetails(errorMessage, new Dictionary<string, object>() { { ValueMetadataKey , value!    } });


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
    public override string ToString()
    {
        StringBuilder sb = new();

        if (Errors.Any())
        {
            sb.AppendLine("MlError:");
            foreach (var error in Errors)
            {
                sb.AppendLine($"     {error.Message}");
            }
        }

        if (Metadata.Any())
        {
            sb.AppendLine("Details:");
            foreach (var detail in Metadata)
            {
                sb.AppendLine($"     {detail.Key}: {detail.Value}");
            }
        }

        var result = sb.ToString().TrimEnd();

        return result;
    }


}


/// <summary>
/// Métodos de extension de <c>MlErrorDetails</c>.
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
    public static MlErrorDetails ToMlErrorsDetails(this (MlError error, Dictionary<string, object> details) source) => new(source.error.ToMlErrors());
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
