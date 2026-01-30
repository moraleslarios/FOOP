namespace MoralesLarios.OOFP.HttpClients.ParamsInfo;


public record CallRequestParamsInfo(string                      Url,
                                    Key                         HttpClientFactoryKey,
                                    Dictionary<string, string>? Headers           = null!,
                                    CancellationToken           CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo((NotEmptyString              url,
                                                           Key                         httpClientFactoryKey,
                                                           Dictionary<string, string>? headers,
                                                           CancellationToken           cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.headers,  value.cancellationToken);
}




public record CallRequestParamsInfo<TRequest>(                     string                      Url,
                                                                   Key                         HttpClientFactoryKey,
                                              [property: Required] TRequest                    RequestBody,
                                                                   Dictionary<string, string>? Headers           = null!,
                                                                   CancellationToken           CancellationToken = default
)
{
    public static implicit operator CallRequestParamsInfo<TRequest>((string                      url,
                                                                     Key                         httpClientFactoryKey,
                                                                     TRequest                    requestBody,
                                                                     Dictionary<string, string>? headers,
                                                                     CancellationToken           cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.requestBody, value.headers, value.cancellationToken);


}
