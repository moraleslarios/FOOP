namespace MoralesLarios.OOFP.Http.Requests;


public record ApiCallOptions(string                      Url,
                                    ServiceKey                         HttpClientFactoryKey,
                                    Dictionary<string, string>? Headers           = null!,
                                    CancellationToken           CancellationToken = default
)
{
    public static implicit operator ApiCallOptions((NotEmptyString              url,
                                                           ServiceKey                         httpClientFactoryKey,
                                                           Dictionary<string, string>? headers,
                                                           CancellationToken           cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.headers,  value.cancellationToken);
}




public record ApiCallOptions<TRequest>(                     string                      Url,
                                                                   ServiceKey                         HttpClientFactoryKey,
                                              [property: Required] TRequest                    RequestBody,
                                                                   Dictionary<string, string>? Headers           = null!,
                                                                   CancellationToken           CancellationToken = default
)
{
    public static implicit operator ApiCallOptions<TRequest>((string                      url,
                                                                     ServiceKey                         httpClientFactoryKey,
                                                                     TRequest                    requestBody,
                                                                     Dictionary<string, string>? headers,
                                                                     CancellationToken           cancellationToken) value) 
        => new (value.url, value.httpClientFactoryKey, value.requestBody, value.headers, value.cancellationToken);


}
