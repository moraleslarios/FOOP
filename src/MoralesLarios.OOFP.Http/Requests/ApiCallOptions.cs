namespace MoralesLarios.OOFP.Http.Requests;


public record ApiCallOptions(string                      Url,
                                    ServiceKey                         ClientName,
                                    Dictionary<string, string>? Headers           = null!,
                                    CancellationToken           CancellationToken = default
)
{
    public static implicit operator ApiCallOptions((NotEmptyString              url,
                                                           ServiceKey                         clientName,
                                                           Dictionary<string, string>? headers,
                                                           CancellationToken           cancellationToken) value) 
        => new (value.url, value.clientName, value.headers,  value.cancellationToken);
}




public record ApiCallOptions<TRequest>(                     string                      Url,
                                                                   ServiceKey                         ClientName,
                                              [property: Required] TRequest                    RequestBody,
                                                                   Dictionary<string, string>? Headers           = null!,
                                                                   CancellationToken           CancellationToken = default
)
{
    public static implicit operator ApiCallOptions<TRequest>((string                      url,
                                                                     ServiceKey                         clientName,
                                                                     TRequest                    requestBody,
                                                                     Dictionary<string, string>? headers,
                                                                     CancellationToken           cancellationToken) value) 
        => new (value.url, value.clientName, value.requestBody, value.headers, value.cancellationToken);


}
