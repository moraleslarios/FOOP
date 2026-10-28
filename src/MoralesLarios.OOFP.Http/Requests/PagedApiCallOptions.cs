namespace MoralesLarios.OOFP.Http.Requests;





/// <summary>
/// Opciones de configuración de <c>PagedApiCall</c>.
/// </summary>
/// <param name="Url">Valor de <c>Url</c>.</param>
/// <param name="ClientName">Nombre del elemento.</param>
/// <param name="PageNumber">Valor de <c>PageNumber</c>.</param>
/// <param name="PageSize">Valor de <c>PageSize</c>.</param>
/// <param name="Headers">Valor de <c>Headers</c>.</param>
/// <param name="CancellationToken">Token para cancelar la operación.</param>
public record PagedApiCallOptions(string                      Url,
                                              ServiceKey                         ClientName,
                                              IntNonNegative              PageNumber,
                                              IntNonNegative              PageSize,
                                              Dictionary<string, string>? Headers           = null!,
                                              CancellationToken           CancellationToken = default)
    : ApiCallOptions(Url, ClientName, Headers,  CancellationToken)
{
    /// <summary>
    /// Realiza la conversión implícita a <c>PagedApiCallOptions</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>PagedApiCallOptions</c> obtenido por la operación.</returns>
    public static implicit operator PagedApiCallOptions((string                      url,
                                                                     ServiceKey                         clientName,
                                                                     IntNonNegative              pageNumber,
                                                                     IntNonNegative              pageSize,
                                                                     Dictionary<string, string>? Headers,
                                                                     CancellationToken           cancellationToken) value)
        => new(value.url, value.clientName, value.pageNumber, value.pageSize, value.Headers, value.cancellationToken);


}




/// <summary>
/// Opciones de configuración de <c>PagedApiCall</c>.
/// </summary>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <param name="Url">Valor de <c>Url</c>.</param>
/// <param name="ClientName">Nombre del elemento.</param>
/// <param name="RequestBody">Valor de <c>RequestBody</c>.</param>
/// <param name="PageNumber">Valor de <c>PageNumber</c>.</param>
/// <param name="PageSize">Valor de <c>PageSize</c>.</param>
/// <param name="Headers">Valor de <c>Headers</c>.</param>
/// <param name="CancellationToken">Token para cancelar la operación.</param>
public record PagedApiCallOptions<TRequest>(                     string                      Url,
                                                                             ServiceKey                         ClientName,
                                                        [property: Required] TRequest                    RequestBody,
                                                                             IntNonNegative              PageNumber,
                                                                             IntNonNegative              PageSize,
                                                                             Dictionary<string, string>? Headers           = null!,
                                                                             CancellationToken           CancellationToken = default)
    : ApiCallOptions<TRequest>(Url, ClientName, RequestBody, Headers, CancellationToken)
{
    /// <summary>
    /// Realiza la conversión implícita a <c>PagedApiCallOptions&lt;TRequest&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>PagedApiCallOptions&lt;TRequest&gt;</c> obtenido por la operación.</returns>
    public static implicit operator PagedApiCallOptions<TRequest>((string                      url,
                                                                               ServiceKey                         clientName,
                                                                               TRequest                    requestBody,
                                                                               IntNonNegative              pageNumber,
                                                                               IntNonNegative              pageSize,
                                                                               Dictionary<string, string>? headers,
                                                                               CancellationToken           cancellationToken) value)
        => new(value.url, value.clientName, value.requestBody, value.pageNumber, value.pageSize, value.headers, value.cancellationToken);


}
