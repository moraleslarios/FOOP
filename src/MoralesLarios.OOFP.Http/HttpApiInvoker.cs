// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Text.Json;

namespace MoralesLarios.OOFP.Http;

public class HttpApiInvoker(IHttpClientFactory                _httpClientFactory,
                                      ILogger<HttpApiInvoker> _logger) : IHttpApiInvoker
{

    public virtual async Task<MlResult<T>> GetAsync<T>(ServiceKey                        clientName,
                                                       string                     url     = "", // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                       Dictionary<string, string> headers = null!,
                                                       CancellationToken          ct      = default)
    {
        var result = await _logger.LogMlResultInformationAsync($"Consultando datos a la url {url}")
                                    .BindAsync( _             => new HttpRequestMessage(HttpMethod.Get, url).TrySetHeadersAsync(headers ?? []))
                                    .BindAsync(requestMessage => InternalHttpActionAsync<HttpRequestMessage, T>(clientName, requestMessage, url, ct));
        return result!;
    }

    public virtual async Task<MlResult<T>> GetAsync<T>(ApiCallOptions parameters, CancellationToken ct = default)
        => await GetAsync<T>(parameters.HttpClientFactoryKey, parameters.Url, parameters.Headers!, ct);




    public virtual async Task<MlResult<T>> GetPagedAsync<T>(PagedApiCallOptions parameters, CancellationToken ct = default)
    {
        var result = await DataAnnotationsValidator.ValidateAsync(parameters)
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


    public virtual async Task<MlResult<TResult>> PostAndReadAsync<T, TResult>(ServiceKey                        clientName,
                                                                          T                          itemBody,
                                                                          string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                                          Dictionary<string, string> headers = null!,
                                                                          CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Post, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.TrySetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionPostAsync<TResult>(clientName,
                                                                              requestMessage, 
                                                                              url, 
                                                                              ct));





    public virtual async Task<MlResult<PagedResult<TItem>>> PostAndReadPagedAsync<TRequest, TItem>(PagedApiCallOptions<TRequest> parameters, CancellationToken ct = default!)
    {
        var result = await DataAnnotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(parameters.HttpClientFactoryKey))
                                    .ExecSelfIfValidAsync(client => _logger.LogInformation($"Consultando datos paginados a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, parameters.Url ?? string.Empty)}. PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}"))
                                            .LogMlResultInformationAsync(_logger, $"Añadiendo información a las cabeceras del httpClient: PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}")
                                    .BindAsync   (client => client.AddPaginationHeadersAsync(parameters.PageNumber, parameters.PageSize))
                                    .TryBindAsync(client => SendAndReadAsync<TRequest, PagedResult<TItem>>(parameters, client, nameof(PostAndReadPagedAsync)));
        return result!;
    }



    public virtual async Task<MlResult<T>> PostAsync<T>(ServiceKey                        clientName,
                                                        T                          itemBody,
                                                        string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                        Dictionary<string, string> headers = null!,
                                                        CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Post, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.TrySetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionPostAsync<T>(clientName,
                                                                              requestMessage, 
                                                                              url, 
                                                                              ct));

    public virtual async Task<MlResult<TResponse>> PostAsync<T, TResponse>(ServiceKey                        clientName,
                                                           T                          itemBody,
                                                           string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                           Dictionary<string, string> headers = null!,
                                                           CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Post, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.TrySetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionPostAsync<TResponse>(clientName,
                                                                              requestMessage, 
                                                                              url, 
                                                                              ct));


    public virtual async Task<MlResult<EmptyValue>> PutAsync<T>(ServiceKey                        clientName,
                                                           T                          itemBody,
                                                           string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                           Dictionary<string, string> headers = null!,
                                                           CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Put, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.TrySetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionEmptyAsync<T>(clientName,
                                                                               "Put",
                                                                               requestMessage, 
                                                                               url, 
                                                                               ct));


    public virtual async Task<MlResult<EmptyValue>> DeleteAsync<T>(ServiceKey                        clientName,
                                                              T                          itemBody,
                                                              string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                              Dictionary<string, string> headers = null!,
                                                              CancellationToken          ct      = default)
        => await EnsureFp.NotNullAsync(itemBody, $"{nameof(itemBody)} no puede ser nulo")
                            .TryMapAsync( _             => JsonSerializer.Serialize(itemBody))
                            .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Delete, url)
                                                                      {
                                                                          Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                      }.TrySetHeadersAsync(headers ?? []))
                            .BindAsync  (requestMessage => ActionEmptyAsync<T>(clientName,
                                                                               "Delete",
                                                                               requestMessage, 
                                                                               url, 
                                                                               ct));


    public virtual async Task<MlResult<EmptyValue>> DeleteByIdAsync<T>(ServiceKey                        clientName,
                                                                  NotEmptyString             url,
                                                                  Dictionary<string, string> headers = null!,
                                                                  CancellationToken          ct      = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .BindAsync           ( _       => new HttpRequestMessage(HttpMethod.Delete, url).TrySetHeadersAsync(headers ?? []))
                                    .TryBindAsync        (request  => _httpClientFactory.CreateClient(clientName)
                                                                                            .CombineAll(request))
                                    .ExecSelfIfValidAsync(tuple    => _logger.LogMlResultInformationAsync($"Haciendo Delete a la url {Path.Combine(tuple.Item1.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (tuple    => tuple.Item1.SendAsync(tuple.Item2, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         ( _       => EmptyValue.Create())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(DeleteByIdAsync)}<{typeof(T).Name}");

        return result!;
    }




    public MlResult<HttpClient> CreateHttpClient(ServiceKey clientName)
    {
        var result = EnsureFp.NotNull(clientName, $"{nameof(clientName)} no puede ser nulo")
                                .TryMap( _ => _httpClientFactory.CreateClient(clientName));
        return result;
    }






    #region protected methods




    protected virtual async Task<MlResult<TResponse?>> InternalHttpActionAsync<TRequest, TResponse>(ServiceKey                clientName, 
                                                                                                    HttpRequestMessage requestMessage, 
                                                                                                    string             url = null!,
                                                                                                    CancellationToken  ct = default!)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(clientName))
                                    .ExecSelfIfValidAsync(client => InternalLogUrl(_logger,
                                                                                    client.BaseAddress?.ToString()!,
                                                                                    url))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    //.ExecSelfIfValidAsync(actionValid: response => response.Dispose())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(InternalHttpActionAsync)}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }


    protected virtual async Task<MlResult<TResponse?>> SendAndReadAsync<TRequest, TResponse>(ApiCallOptions<TRequest> parameters, HttpClient client, MemberName methodName)
    {
        var result = await EnsureFp.NotNullAsync(parameters.RequestBody, $"{nameof(parameters.RequestBody)} no puede ser nulo")
                                    .TryMapAsync(_ => JsonSerializer.Serialize(parameters.RequestBody))



                                    .TryMapAsync( _       => client.PostAsJsonAsync(parameters.Url, parameters.RequestBody, parameters.CancellationToken))
                                    .BindAsync  (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: parameters.CancellationToken))
                                            .MyMethodFinalLogAsync(_logger, $"{methodName}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }


    protected virtual async Task<MlResult<T>> ActionPostAsync<T>(ServiceKey                clientName,
                                                                 HttpRequestMessage requestMessage,
                                                                 string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                                 CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(clientName))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo post a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         (response => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(PostAsync)}<{typeof(T).Name}");

        return result!;
    }

    protected virtual async Task<MlResult<EmptyValue>> ActionEmptyAsync<T>(ServiceKey                clientName,
                                                                      string             action,
                                                                      HttpRequestMessage requestMessage,
                                                                      string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress en el HttpClientFactoryKey
                                                                      CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(clientName))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo {action} a la url {Path.Combine(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>())
                                    .MapAsync            ( _       => EmptyValue.Create().ToAsync())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(ActionEmptyAsync)}<{typeof(T).Name}");

        return result!;
    }

    protected virtual void InternalLogUrl(ILogger<HttpApiInvoker> logger,
                                          string                            baseAddress, 
                                          string                            url)
        => logger.LogInformation($"Consultando datos a la url {BuildRequestUri(baseAddress, url)}");

    protected virtual string BuildRequestUri(string baseAddress, 
                                            string url)
        => Path.Combine(baseAddress ?? string.Empty, url ?? string.Empty);

    protected virtual void InternalLogUrlPagination(ILogger<HttpApiInvoker> logger,
                                                    string                            baseAddress, 
                                                    string                            url,
                                                    IntNonNegative                    pageNumber,
                                                    IntNonNegative                    pageSize)
        => logger.LogInformation($"Consultando datos paginados a la url {BuildRequestUri(baseAddress, url)}. PageNumber {pageNumber}, PageSize {pageSize}");


    protected virtual Task<MlResult<HttpClient>> InternalAddPagInfoAsync(ILogger<HttpApiInvoker> logger,
                                                                         HttpClient                        client,
                                                                         IntNonNegative                    pageNumber,
                                                                         IntNonNegative                    pageSize)
        => logger.LogMlResultInformationAsync($"Añadiendo información a las cabeceras del httpClient: PageNumber {pageNumber}, PageSize {pageSize}")
                                .BindAsync(_ => client.AddPaginationHeadersAsync(pageNumber, pageSize));


    #endregion


    #region private methods






    #endregion

}



