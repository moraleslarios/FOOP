namespace MoralesLarios.OOFP.HttpClients.ParamsInfo;





public record CallRequestPaginationParamsInfo(string                      Url,
                                              Key                         HttpClientFactoryKey,
                                              IntNotNegative              PageNumber,
                                              IntNotNegative              PageSize,
                                              Dictionary<string, string>? Headers           = null!,
                                              CancellationToken           CancellationToken = default)
    : CallRequestParamsInfo(Url, HttpClientFactoryKey, Headers,  CancellationToken)
{
    public static implicit operator CallRequestPaginationParamsInfo((string                      url,
                                                                     Key                         httpClientFactoryKey,
                                                                     IntNotNegative              pageNumber,
                                                                     IntNotNegative              pageSize,
                                                                     Dictionary<string, string>? Headers,
                                                                     CancellationToken           cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.pageNumber, value.pageSize, value.Headers, value.cancellationToken);


}




public record CallRequestPaginationParamsInfo<TRequest>(                     string                      Url,
                                                                             Key                         HttpClientFactoryKey,
                                                        [property: Required] TRequest                    RequestBody,
                                                                             IntNotNegative              PageNumber,
                                                                             IntNotNegative              PageSize,
                                                                             Dictionary<string, string>? Headers           = null!,
                                                                             CancellationToken           CancellationToken = default)
    : CallRequestParamsInfo<TRequest>(Url, HttpClientFactoryKey, RequestBody, Headers, CancellationToken)
{
    public static implicit operator CallRequestPaginationParamsInfo<TRequest>((string                      url,
                                                                               Key                         httpClientFactoryKey,
                                                                               TRequest                    requestBody,
                                                                               IntNotNegative              pageNumber,
                                                                               IntNotNegative              pageSize,
                                                                               Dictionary<string, string>? headers,
                                                                               CancellationToken           cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.requestBody, value.pageNumber, value.pageSize, value.headers, value.cancellationToken);


}
