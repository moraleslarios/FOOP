namespace MoralesLarios.OOFP.HttpClients.ParamsInfo;


public record CallRequestParamsInfo(string            Url,
                                    Name              HttpClientFactoryKey,
                                    CancellationToken CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo((NotEmptyString    url,
                                                           Name              httpClientFactoryKey,
                                                           CancellationToken cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.cancellationToken);
}




public record CallRequestParamsInfo<TRequest>(                     string            Url,
                                                                   Name              HttpClientFactoryKey,
                                              [property: Required] TRequest          RequestBody,
                                                                   CancellationToken CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo<TRequest>((string            url,
                                                                     Name              httpClientFactoryKey,
                                                                     TRequest          requestBody,
                                                                     CancellationToken cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.requestBody, value.cancellationToken);


}
