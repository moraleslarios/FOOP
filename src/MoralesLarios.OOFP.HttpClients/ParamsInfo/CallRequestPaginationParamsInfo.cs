namespace MoralesLarios.OOFP.HttpClients.ParamsInfo;





public record CallRequestPaginationParamsInfo(string            Url,
                                              Key               HttpClientFactoryKey,
                                              IntNotNegative    PageNumber,
                                              IntNotNegative    PageSize,
                                              CancellationToken CancellationToken = default)
    : CallRequestParamsInfo(Url, HttpClientFactoryKey, CancellationToken)
{
    public static implicit operator CallRequestPaginationParamsInfo((string            url,
                                                                     Key               httpClientFactoryKey,
                                                                     IntNotNegative    pageNumber,
                                                                     IntNotNegative    pageSize,
                                                                     CancellationToken cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.pageNumber, value.pageSize, value.cancellationToken);


}




public record CallRequestPaginationParamsInfo<TRequest>(                     string            Url,
                                                                             Key               HttpClientFactoryKey,
                                                        [property: Required] TRequest          RequestBody,
                                                                             IntNotNegative    PageNumber,
                                                                             IntNotNegative    PageSize,
                                                                             CancellationToken CancellationToken = default)
    : CallRequestParamsInfo<TRequest>(Url, HttpClientFactoryKey, RequestBody, CancellationToken)
{
    public static implicit operator CallRequestPaginationParamsInfo<TRequest>((string            url,
                                                                               Key               httpClientFactoryKey,
                                                                               TRequest          requestBody,
                                                                               IntNotNegative    pageNumber,
                                                                               IntNotNegative    pageSize,
                                                                               CancellationToken cancellationToken) value)
        => new(value.url, value.httpClientFactoryKey, value.requestBody, value.pageNumber, value.pageSize, value.cancellationToken);


}
