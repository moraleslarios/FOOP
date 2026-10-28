namespace MoralesLarios.OOFP.Http.Requests;





public record PagedApiCallOptions(string                      Url,
                                              ServiceKey                         ClientName,
                                              IntNonNegative              PageNumber,
                                              IntNonNegative              PageSize,
                                              Dictionary<string, string>? Headers           = null!,
                                              CancellationToken           CancellationToken = default)
    : ApiCallOptions(Url, ClientName, Headers,  CancellationToken)
{
    public static implicit operator PagedApiCallOptions((string                      url,
                                                                     ServiceKey                         clientName,
                                                                     IntNonNegative              pageNumber,
                                                                     IntNonNegative              pageSize,
                                                                     Dictionary<string, string>? Headers,
                                                                     CancellationToken           cancellationToken) value)
        => new(value.url, value.clientName, value.pageNumber, value.pageSize, value.Headers, value.cancellationToken);


}




public record PagedApiCallOptions<TRequest>(                     string                      Url,
                                                                             ServiceKey                         ClientName,
                                                        [property: Required] TRequest                    RequestBody,
                                                                             IntNonNegative              PageNumber,
                                                                             IntNonNegative              PageSize,
                                                                             Dictionary<string, string>? Headers           = null!,
                                                                             CancellationToken           CancellationToken = default)
    : ApiCallOptions<TRequest>(Url, ClientName, RequestBody, Headers, CancellationToken)
{
    public static implicit operator PagedApiCallOptions<TRequest>((string                      url,
                                                                               ServiceKey                         clientName,
                                                                               TRequest                    requestBody,
                                                                               IntNonNegative              pageNumber,
                                                                               IntNonNegative              pageSize,
                                                                               Dictionary<string, string>? headers,
                                                                               CancellationToken           cancellationToken) value)
        => new(value.url, value.clientName, value.requestBody, value.pageNumber, value.pageSize, value.headers, value.cancellationToken);


}
