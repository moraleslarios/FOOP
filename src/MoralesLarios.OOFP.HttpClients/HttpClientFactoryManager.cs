
using System.Runtime.InteropServices.Marshalling;
using System.Text.Json;

namespace MoralesLarios.OOFP.HttpClients;

public class HttpClientFactoryManager(IHttpClientFactory                _httpClientFactory,
                                      ILogger<HttpClientFactoryManager> _logger) : IHttpClientFactoryManager
{
    //public async Task<MlResult<T>> GetAsync<T>(Key  httpClientFactoryKey,
    //                                           string            url = "", // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
    //                                           CancellationToken ct = default)
    //{
    //    var result = await _logger.LogMlResultInformationAsync($"Consultando datos a la url {url}")
    //                                .TryMapAsync( _      => _httpClientFactory.CreateClient(httpClientFactoryKey))
    //                                .TryMapAsync( client => client.GetFromJsonAsync<T>(url, ct))
    //                                        .MyMethodFinalLogAsync(_logger, $"GetAsync<{typeof(T).Name}>");
    //    return result!;
    //}

    public async Task<MlResult<T>> GetAsync<T>(Key                        httpClientFactoryKey,
                                               string                     url     = "", // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                               Dictionary<string, string> headers = null!,
                                               CancellationToken          ct      = default)
    {
        var result = await _logger.LogMlResultInformationAsync($"Consultando datos a la url {url}")
                                    .BindAsync( _             => new HttpRequestMessage(HttpMethod.Get, url).SetHeadersAsync(headers ?? []))
                                    .BindAsync(requestMessage => InternalHttpActionAsync<HttpRequestMessage, T>(httpClientFactoryKey, requestMessage, url, ct));
        return result!;
    }

    public async Task<MlResult<T>> GetAsync<T>(CallRequestParamsInfo parameters, CancellationToken ct = default)
        => await GetAsync<T>(parameters.HttpClientFactoryKey, parameters.Url, parameters.Headers!, ct);




    public async Task<MlResult<T>> GetPaginationAsync<T>(CallRequestPaginationParamsInfo parameters, CancellationToken ct = default)
    {
        var result = await DataannotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(parameters.HttpClientFactoryKey))
                                    .ExecSelfIfValidAsync(client => InternalLogUrlPagination(_logger, 
                                                                                             client.BaseAddress?.ToString()!, 
                                                                                             parameters.Url, 
                                                                                             parameters.PageNumber, 
                                                                                             parameters.PageSize))
                                    .BindAsync           (client => InternalAddPagInfoAsync (_logger, 
                                                                                             client, 
                                                                                             parameters.PageNumber, 
                                                                                             parameters.PageSize))
                                    .TryMapAsync         (client => client.GetFromJsonAsync<T>(parameters.Url, ct))
                                            .MyMethodFinalLogAsync(_logger, $"GetPaginationAsync<{typeof(T).Name}>");
        return result!;
    }



    public async Task<MlResult<TResponse>> PostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters)
    {
        var result = await DataannotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync         ( _      => _httpClientFactory.CreateClient(parameters.HttpClientFactoryKey))
                                    .ExecSelfIfValidAsync( client => InternalLogUrl(_logger,
                                                                                    client.BaseAddress?.ToString()!,
                                                                                    parameters.Url))
                                    .TryBindAsync        ( client => InternalPostGetAsync<TRequest, TResponse>(parameters, client, nameof(PostGetAsync)));
        return result!;
    }

    public async Task<MlResult<PaginationResultInfo<TEnumrableResponse>>> PostGetPaginationAsync<TRequest, TEnumrableResponse>(CallRequestPaginationParamsInfo<TRequest> parameters, CancellationToken ct = default!)
    {
        var result = await DataannotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync ( _      => _httpClientFactory.CreateClient(parameters.HttpClientFactoryKey))
                                            .LogMlResultInformationAsync(_logger, "fdasñlkajsñlajdñ")

                                       

                                    .ExecSelfIfValidAsync(client => _logger.LogInformation($"Consultando datos paginados a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, parameters.Url ?? string.Empty)}. PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}"))
                                            .LogMlResultInformationAsync(_logger, $"Añadiendo información a las cabeceras del httpClient: PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}")
                                    .BindAsync   (client => client.SetHeaderPageInfoAsync(parameters.PageNumber, parameters.PageSize))
                                    .TryBindAsync(client => InternalPostGetAsync<TRequest, PaginationResultInfo<TEnumrableResponse>>(parameters, client, nameof(PostGetPaginationAsync)));
        return result!;
    }




    //public async Task<MlResult<T>> PostAsync<T>(Key               httpClientFactoryKey,
    //                                            T                 itemBody,
    //                                            string            url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
    //                                            CancellationToken ct  = default)
    //    => await ActionAsync(httpClientFactoryKey, 
    //                         itemBody, 
    //                         "Post", 
    //                         client => client.PostAsJsonAsync(url, itemBody, ct), 
    //                         url, 
    //                         ct);

    public async Task<MlResult<T>> PostAsync<T>(Key                        httpClientFactoryKey,
                                                T                          itemBody,
                                                string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                Dictionary<string, string> headers = null!,
                                                CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Post, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.SetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionPostAsync<T>(httpClientFactoryKey,
                                                                              requestMessage, 
                                                                              url, 
                                                                              ct));

    //public async Task<MlResult<Empty>> PutAsync<T>(Key               httpClientFactoryKey,
    //                                               T                 itemBody,
    //                                               string            url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
    //                                               CancellationToken ct  = default)
    //{
    //    var result = await MlResult.EmptyAsync()
    //                                .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
    //                                .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo Put a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
    //                                .TryMapAsync         (client   => client.PutAsJsonAsync(url, itemBody, ct))
    //                                .BindAsync           (response => response.IsSuccessStatusCode
    //                                                                    ? response.ToMlResultValid()
    //                                                                    : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
    //                                .TryMapAsync         ( _       => Empty.Create())
    //                                        .MyMethodFinalLogAsync(_logger, $"{nameof(PutAsync)}<{typeof(T).Name}");
    //    return result!;
    //}


    public async Task<MlResult<Empty>> PutAsync<T>(Key                        httpClientFactoryKey,
                                                   T                          itemBody,
                                                   string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                   Dictionary<string, string> headers = null!,
                                                   CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Put, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.SetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionEmptyAsync<T>(httpClientFactoryKey,
                                                                               "Put",
                                                                               requestMessage, 
                                                                               url, 
                                                                               ct));





    //public async Task<MlResult<Empty>> DeleteAsync<T>(Key               httpClientFactoryKey,
    //                                                  T                 itemBody,
    //                                                  string            url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
    //                                                  CancellationToken ct  = default)
    //{
    //    var result = await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
    //                        .TryMapAsync         ( _       => new {
    //                                                                    Client = _httpClientFactory.CreateClient(httpClientFactoryKey),
    //                                                                    HttpRequestMessage =  new HttpRequestMessage(HttpMethod.Delete, url) { Content = JsonContent.Create(itemBody) }
    //                                                                })
    //                        .ExecSelfIfValidAsync(data     => _logger.LogMlResultInformationAsync($"Haciendo Delete a la url {Path.Combine(data.Client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
    //                        .TryMapAsync         (data     => data.Client.SendAsync(data.HttpRequestMessage, ct))
    //                        .BindAsync           (response => response.IsSuccessStatusCode
    //                                                            ? response.ToMlResultValid()
    //                                                            : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
    //                        .TryMapAsync         ( _       => Empty.Create())
    //                                .MyMethodFinalLogAsync(_logger, $"{nameof(DeleteAsync)}<{typeof(T).Name}");

    //    return result!;
    //}


    public async Task<MlResult<Empty>> DeleteAsync<T>(Key                        httpClientFactoryKey,
                                                      T                          itemBody,
                                                      string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                      Dictionary<string, string> headers = null!,
                                                      CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Delete, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.SetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionEmptyAsync<T>(httpClientFactoryKey,
                                                                               "Delete",
                                                                               requestMessage, 
                                                                               url, 
                                                                               ct));



    //public async Task<MlResult<Empty>> DeleteByIdAsync<T>(Key               httpClientFactoryKey,
    //                                                      NotEmptyString    url,
    //                                                      CancellationToken ct = default)
    //{
    //    var result = await MlResult.EmptyAsync()
    //                                .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
    //                                .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo Delete a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
    //                                .TryMapAsync         (client   => client.DeleteAsync(url, ct))
    //                                .BindAsync           (response => response.IsSuccessStatusCode
    //                                                                    ? response.ToMlResultValid()
    //                                                                    : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
    //                                .TryMapAsync         ( _       => Empty.Create())
    //                                        .MyMethodFinalLogAsync(_logger, $"{nameof(DeleteByIdAsync)}<{typeof(T).Name}");

    //    return result!;
    //}


    public async Task<MlResult<Empty>> DeleteByIdAsync<T>(Key                        httpClientFactoryKey,
                                                          NotEmptyString             url,
                                                          Dictionary<string, string> headers = null!,
                                                          CancellationToken          ct      = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .BindAsync           ( _       => new HttpRequestMessage(HttpMethod.Delete, url).SetHeadersAsync(headers ?? []))
                                    .TryBindAsync        (request  => _httpClientFactory.CreateClient(httpClientFactoryKey)
                                                                                            .Combine(request))
                                    .ExecSelfIfValidAsync(tuple    => _logger.LogMlResultInformationAsync($"Haciendo Delete a la url {Path.Combine(tuple.Item1.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (tuple    => tuple.Item1.SendAsync(tuple.Item2, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         ( _       => Empty.Create())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(DeleteByIdAsync)}<{typeof(T).Name}");

        return result!;
    }




    public MlResult<HttpClient> CreateHttpClient(Key httpClientFactoryKey)
    {
        var result = EnsureFp.NotNull(httpClientFactoryKey, $"{nameof(httpClientFactoryKey)} no puede ser nulo")
                                .TryMap( _ => _httpClientFactory.CreateClient(httpClientFactoryKey));
        return result;
    }






    #region protected methods




    protected virtual async Task<MlResult<TResponse?>> InternalHttpActionAsync<TRequest, TResponse>(Key                httpClientFactoryKey, 
                                                                                                    HttpRequestMessage requestMessage, 
                                                                                                    string             url = null!,
                                                                                                    CancellationToken  ct = default!)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(httpClientFactoryKey))
                                    .ExecSelfIfValidAsync(client => InternalLogUrl(_logger,
                                                                                    client.BaseAddress?.ToString()!,
                                                                                    url))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    //.ExecSelfIfValidAsync(actionValid: response => response.Dispose())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(InternalPostGetAsync)}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }

    protected virtual async Task<MlResult<TResponse?>> InternalPostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters, HttpRequestMessage requestMessage)
        => await InternalHttpActionAsync<TRequest, TResponse>(parameters.HttpClientFactoryKey, requestMessage, parameters.Url, parameters.CancellationToken);






    protected virtual async Task<MlResult<TResponse?>> InternalPostGetAsync<TRequest, TResponse>(CallRequestParamsInfo<TRequest> parameters, HttpClient client, Name methodName)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync( _       => client.PostAsJsonAsync(parameters.Url, parameters.RequestBody, parameters.CancellationToken))
                                    .BindAsync  (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: parameters.CancellationToken))
                                            .MyMethodFinalLogAsync(_logger, $"{methodName}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }

    //protected virtual async Task<MlResult<T>> ActionAsync<T>(Key               httpClientFactoryKey,
    //                                                         T                 itemBody,
    //                                                         string            action,
    //                                                         Func<HttpClient, Task<HttpResponseMessage>> actionFunc,
    //                                                         string            url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
    //                                                         CancellationToken ct  = default)
    //{
    //    var result = await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
    //                                .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
    //                                .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo {action} a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
    //                                .TryMapAsync         (client   => actionFunc(client))
    //                                .BindAsync           (response => response.IsSuccessStatusCode
    //                                                                    ? response.ToMlResultValid()
    //                                                                    : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
    //                                .TryMapAsync(response => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct))
    //                                        .MyMethodFinalLogAsync(_logger, $"{nameof(PostAsync)}<{typeof(T).Name}");

    //    return result!;
    //}

    protected virtual async Task<MlResult<T>> ActionPostAsync<T>(Key                httpClientFactoryKey,
                                                                 HttpRequestMessage requestMessage,
                                                                 string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                                 CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo post a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         (response => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(PostAsync)}<{typeof(T).Name}");

        return result!;
    }

    protected virtual async Task<MlResult<Empty>> ActionEmptyAsync<T>(Key                httpClientFactoryKey,
                                                                      string             action,
                                                                      HttpRequestMessage requestMessage,
                                                                      string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAdress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                                      CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(httpClientFactoryKey))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo {action} a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .MapAsync            ( _       => Empty.Create().ToAsync())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(ActionEmptyAsync)}<{typeof(T).Name}");

        return result!;
    }

    protected virtual void InternalLogUrl(ILogger<HttpClientFactoryManager> logger,
                                          string                            baseAddress, 
                                          string                            url)
        => logger.LogInformation($"Consultando datos a la url {InternalGetUrl(baseAddress, url)}");

    protected virtual string InternalGetUrl(string baseAddress, 
                                            string url)
        => Path.Combine(baseAddress ?? string.Empty, url ?? string.Empty);

    protected virtual void InternalLogUrlPagination(ILogger<HttpClientFactoryManager> logger,
                                                    string                            baseAddress, 
                                                    string                            url,
                                                    IntNotNegative                    pageNumber,
                                                    IntNotNegative                    pageSize)
        => logger.LogInformation($"Consultando datos paginados a la url {InternalGetUrl(baseAddress, url)}. PageNumber {pageNumber}, PageSize {pageSize}");


    protected virtual Task<MlResult<HttpClient>> InternalAddPagInfoAsync(ILogger<HttpClientFactoryManager> logger,
                                                                         HttpClient                        client,
                                                                         IntNotNegative                    pageNumber,
                                                                         IntNotNegative                    pageSize)
        => logger.LogMlResultInformationAsync($"Añadiendo información a las cabeceras del httpClient: PageNumber {pageNumber}, PageSize {pageSize}")
                                .BindAsync(_ => client.SetHeaderPageInfoAsync(pageNumber, pageSize));


    #endregion


    #region private methods






    #endregion

}


