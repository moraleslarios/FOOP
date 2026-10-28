namespace MoralesLarios.OOFP.Http.Requests;


/// <summary>
/// Opciones de configuración de <c>ApiCall</c>.
/// </summary>
/// <param name="Url">Valor de <c>Url</c>.</param>
/// <param name="ClientName">Nombre del elemento.</param>
/// <param name="Headers">Valor de <c>Headers</c>.</param>
/// <param name="CancellationToken">Token para cancelar la operación.</param>
public record ApiCallOptions(string                      Url,
                                    ServiceKey                         ClientName,
                                    Dictionary<string, string>? Headers           = null!,
                                    CancellationToken           CancellationToken = default
)
{
    /// <summary>
    /// Realiza la conversión implícita a <c>ApiCallOptions</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>ApiCallOptions</c> obtenido por la operación.</returns>
    public static implicit operator ApiCallOptions((NotEmptyString              url,
                                                           ServiceKey                         clientName,
                                                           Dictionary<string, string>? headers,
                                                           CancellationToken           cancellationToken) value) 
        => new (value.url, value.clientName, value.headers,  value.cancellationToken);
}




/// <summary>
/// Opciones de configuración de <c>ApiCall</c>.
/// </summary>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <param name="Url">Valor de <c>Url</c>.</param>
/// <param name="ClientName">Nombre del elemento.</param>
/// <param name="RequestBody">Valor de <c>RequestBody</c>.</param>
/// <param name="Headers">Valor de <c>Headers</c>.</param>
/// <param name="CancellationToken">Token para cancelar la operación.</param>
public record ApiCallOptions<TRequest>(                     string                      Url,
                                                                   ServiceKey                         ClientName,
                                              [property: Required] TRequest                    RequestBody,
                                                                   Dictionary<string, string>? Headers           = null!,
                                                                   CancellationToken           CancellationToken = default
)
{
    /// <summary>
    /// Realiza la conversión implícita a <c>ApiCallOptions&lt;TRequest&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>ApiCallOptions&lt;TRequest&gt;</c> obtenido por la operación.</returns>
    public static implicit operator ApiCallOptions<TRequest>((string                      url,
                                                                     ServiceKey                         clientName,
                                                                     TRequest                    requestBody,
                                                                     Dictionary<string, string>? headers,
                                                                     CancellationToken           cancellationToken) value) 
        => new (value.url, value.clientName, value.requestBody, value.headers, value.cancellationToken);


}
