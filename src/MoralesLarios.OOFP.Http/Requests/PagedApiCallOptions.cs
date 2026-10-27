namespace MoralesLarios.OOFP.Http.Requests;





public record PagedApiCallOptions(string                      Url,
                                              ServiceKey                         HttpClientFactoryKey,
                                              IntNonNegative              PageNumber,
                                              IntNonNegative              PageSize,
                                              Dictionary<string, string>? Headers           = null!,
                                              CancellationToken           CancellationToken = default)
    : ApiCallOptions(Url, HttpClientFactoryKey, Headers,  CancellationToken)
{
    public static implicit operator PagedApiCallOptions((string                      url,
                                                                     ServiceKey                         httpClientFactoryKey,
                                                                     IntNonNegative              pageNumber,
                                                                     IntNonNegative              pageSize,
                                                                     Dictionary<string, string>? Headers,
                                                                     CancellationToken           cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.pageNumber, value.pageSize, value.Headers, value.cancellationToken);


}




public record PagedApiCallOptions<TRequest>(                     string                      Url,
                                                                             ServiceKey                         HttpClientFactoryKey,
                                                        [property: Required] TRequest                    RequestBody,
                                                                             IntNonNegative              PageNumber,
                                                                             IntNonNegative              PageSize,
                                                                             Dictionary<string, string>? Headers           = null!,
                                                                             CancellationToken           CancellationToken = default)
    : ApiCallOptions<TRequest>(Url, HttpClientFactoryKey, RequestBody, Headers, CancellationToken)
{
    public static implicit operator PagedApiCallOptions<TRequest>((string                      url,
                                                                               ServiceKey                         httpClientFactoryKey,
                                                                               TRequest                    requestBody,
                                                                               IntNonNegative              pageNumber,
                                                                               IntNonNegative              pageSize,
                                                                               Dictionary<string, string>? headers,
                                                                               CancellationToken           cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.requestBody, value.pageNumber, value.pageSize, value.headers, value.cancellationToken);


}
