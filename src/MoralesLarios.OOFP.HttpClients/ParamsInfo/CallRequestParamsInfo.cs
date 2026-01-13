namespace MoralesLarios.OOFP.HttpClients.ParamsInfo;


public record CallRequestParamsInfo(string            Url,
                                    Key               HttpClientFactoryKey,
                                    CancellationToken CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo((NotEmptyString    url,
                                                           Key               httpClientFactoryKey,
                                                           CancellationToken cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.cancellationToken);
}




public record CallRequestParamsInfo<TRequest>(                     string            Url,
                                                                   Key               HttpClientFactoryKey,
                                              [property: Required] TRequest          RequestBody,
                                                                   CancellationToken CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo<TRequest>((string            url,
                                                                     Key               httpClientFactoryKey,
                                                                     TRequest          requestBody,
                                                                     CancellationToken cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.requestBody, value.cancellationToken);


}
