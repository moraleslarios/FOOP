// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Text.Json;

namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Representa <c>HttpApiInvoker</c>.
/// </summary>
public class HttpApiInvoker(IHttpClientFactory                _httpClientFactory,
                                      ILogger<HttpApiInvoker> _logger) : IHttpApiInvoker
{

    /// <summary>
    /// Obtiene el elemento solicitado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> GetAsync<T>(ServiceKey                        clientName,
                                                       string                     url     = "", // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
                                                       Dictionary<string, string> headers = null!,
                                                       CancellationToken          ct      = default)
    {
        var result = await _logger.LogMlResultInformationAsync($"Consultando datos a la url {url}")
                                    .BindAsync( _             => new HttpRequestMessage(HttpMethod.Get, url).TrySetHeadersAsync(headers ?? []))
                                    .BindAsync(requestMessage => InternalHttpActionAsync<HttpRequestMessage, T>(clientName, requestMessage, url, ct));
        return result!;
    }

    /// <summary>
    /// Obtiene el elemento solicitado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> GetAsync<T>(ApiCallOptions parameters, CancellationToken ct = default)
        => await GetAsync<T>(parameters.ClientName, parameters.Url, parameters.Headers!, ct);




    /// <summary>
    /// Obtiene el elemento solicitado de <c>Paged</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> GetPagedAsync<T>(PagedApiCallOptions parameters, CancellationToken ct = default)
    {
        var result = await DataAnnotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(parameters.ClientName))
                                    .ExecSelfIfValidAsync(client => InternalLogUrlPagination(_logger, 
                                                                                             client.BaseAddress?.ToString()!, 
                                                                                             parameters.Url, 
                                                                                             parameters.PageNumber, 
                                                                                             parameters.PageSize))
                                    .TryBindAsync        (client => SendPagedRequestAsync<T>(client, HttpMethod.Get, parameters, ct))
                                            .MyMethodFinalLogAsync(_logger, $"GetPagedAsync<{typeof(T).Name}>");
        return result!;
    }


    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST de <c>AndRead</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResult">Tipo del valor devuelto.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<TResult>> PostAndReadAsync<T, TResult>(ServiceKey                        clientName,
                                                                          T                          itemBody,
                                                                          string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
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





    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST de <c>AndReadPaged</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<PagedResult<TItem>>> PostAndReadPagedAsync<TRequest, TItem>(PagedApiCallOptions<TRequest> parameters, CancellationToken ct = default!)
    {
        var result = await DataAnnotationsValidator.ValidateAsync(parameters)
                                    .TryMapAsync         ( _     => _httpClientFactory.CreateClient(parameters.ClientName))
                                    .ExecSelfIfValidAsync(client => _logger.LogInformation($"Consultando datos paginados a la url {BuildRequestUri(client.BaseAddress?.ToString() ?? string.Empty, parameters.Url ?? string.Empty)}. PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}"))
                                            .LogMlResultInformationAsync(_logger, $"Añadiendo información a las cabeceras de la petición: PageNumber {parameters.PageNumber}, PageSize {parameters.PageSize}")
                                    .TryBindAsync(client => SendAndReadPagedAsync<TRequest, PagedResult<TItem>>(parameters, client, nameof(PostAndReadPagedAsync)));
        return result!;
    }



    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<T>> PostAsync<T>(ServiceKey                        clientName,
                                                        T                          itemBody,
                                                        string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
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

    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<TResponse>> PostAsync<T, TResponse>(ServiceKey                        clientName,
                                                           T                          itemBody,
                                                           string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
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


    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<EmptyValue>> PutAsync<T>(ServiceKey                        clientName,
                                                           T                          itemBody,
                                                           string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
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


    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<EmptyValue>> DeleteAsync<T>(ServiceKey                        clientName,
                                                              T                          itemBody,
                                                              string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
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


    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<EmptyValue>> DeleteByIdAsync<T>(ServiceKey                        clientName,
                                                                  NotEmptyString             url,
                                                                  Dictionary<string, string> headers = null!,
                                                                  CancellationToken          ct      = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .BindAsync           ( _       => new HttpRequestMessage(HttpMethod.Delete, url).TrySetHeadersAsync(headers ?? []))
                                    .TryBindAsync        (request  => _httpClientFactory.CreateClient(clientName)
                                                                                            .CombineAll(request))
                                    .ExecSelfIfValidAsync(tuple    => _logger.LogMlResultInformationAsync($"Haciendo Delete a la url {BuildRequestUri(tuple.Item1.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (tuple    => tuple.Item1.SendAsync(tuple.Item2, ct))
                                    .BindAsync           (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         ( _       => EmptyValue.Create())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(DeleteByIdAsync)}<{typeof(T).Name}");

        return result!;
    }




    /// <summary>
    /// Crea la instancia solicitada de <c>HttpClient</c>.
    /// </summary>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public MlResult<HttpClient> CreateHttpClient(ServiceKey clientName)
    {
        var result = EnsureFp.NotNull(clientName, $"{nameof(clientName)} no puede ser nulo")
                                .TryMap( _ => _httpClientFactory.CreateClient(clientName));
        return result;
    }






    #region protected methods




    /// <summary>
    /// Ejecuta <c>InternalHttpAction</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="requestMessage">Mensaje descriptivo.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
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
                                    .BindAsync           (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    //.ExecSelfIfValidAsync(actionValid: response => response.Dispose())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(InternalHttpActionAsync)}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }


    /// <summary>
    /// Envia la petición de <c>AndRead</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="methodName">Nombre del elemento.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<TResponse?>> SendAndReadAsync<TRequest, TResponse>(ApiCallOptions<TRequest> parameters, HttpClient client, MemberName methodName)
    {
        var result = await EnsureFp.NotNullAsync(parameters.RequestBody, $"{nameof(parameters.RequestBody)} no puede ser nulo")
                                    .TryMapAsync(_ => JsonSerializer.Serialize(parameters.RequestBody))



                                    .TryMapAsync( _       => client.PostAsJsonAsync(parameters.Url, parameters.RequestBody, parameters.CancellationToken))
                                    .BindAsync  (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync(response => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: parameters.CancellationToken))
                                            .MyMethodFinalLogAsync(_logger, $"{methodName}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }


    /// <summary>
    /// Ejecuta <c>ActionPost</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="requestMessage">Mensaje descriptivo.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<T>> ActionPostAsync<T>(ServiceKey                clientName,
                                                                 HttpRequestMessage requestMessage,
                                                                 string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
                                                                 CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(clientName))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo post a la url {BuildRequestUri(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync         (response => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct))
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(PostAsync)}<{typeof(T).Name}");

        return result!;
    }

    /// <summary>
    /// Ejecuta <c>ActionEmpty</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="action">Acción que se ejecuta como efecto lateral.</param>
    /// <param name="requestMessage">Mensaje descriptivo.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<EmptyValue>> ActionEmptyAsync<T>(ServiceKey                clientName,
                                                                      string             action,
                                                                      HttpRequestMessage requestMessage,
                                                                      string             url = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
                                                                      CancellationToken  ct  = default)
    {
        var result = await MlResult.EmptyAsync()
                                    .TryMapAsync         ( _       => _httpClientFactory.CreateClient(clientName))
                                    .ExecSelfIfValidAsync(client   => _logger.LogMlResultInformationAsync($"Haciendo {action} a la url {BuildRequestUri(client.BaseAddress?.ToString() ?? string.Empty, url ?? string.Empty)}"))
                                    .TryMapAsync         (client   => client.SendAsync(requestMessage, ct))
                                    .BindAsync           (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .MapAsync            ( _       => EmptyValue.Create().ToAsync())
                                            .MyMethodFinalLogAsync(_logger, $"{nameof(ActionEmptyAsync)}<{typeof(T).Name}");

        return result!;
    }

    /// <summary>
    /// Ejecuta <c>InternalLogUrl</c>.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="baseAddress">Valor de <c>baseAddress</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    protected virtual void InternalLogUrl(ILogger<HttpApiInvoker> logger,
                                          string                            baseAddress, 
                                          string                            url)
        => logger.LogInformation($"Consultando datos a la url {BuildRequestUri(baseAddress, url)}");

    /// <summary>
    /// Construye la instancia solicitada de <c>RequestUri</c>.
    /// </summary>
    /// <param name="baseAddress">Valor de <c>baseAddress</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <returns>Representación textual obtenida.</returns>
    protected virtual string BuildRequestUri(string baseAddress, 
                                            string url)
    {
        // Punto 27: Path.Combine es para rutas del sistema de ficheros: en Windows introduce '\'
        // y si 'url' empieza por '/' descarta por completo 'baseAddress'. Para URLs hay que usar Uri.
        var safeBase = baseAddress ?? string.Empty;
        var safeUrl  = url         ?? string.Empty;

        if (string.IsNullOrWhiteSpace(safeUrl )) return safeBase;
        if (string.IsNullOrWhiteSpace(safeBase)) return safeUrl;

        if (Uri.TryCreate(safeUrl, UriKind.Absolute, out var absoluteUrl)) return absoluteUrl.ToString();

        if (! Uri.TryCreate(safeBase, UriKind.Absolute, out var baseUri))
        {
            return $"{safeBase.TrimEnd('/')}/{safeUrl.TrimStart('/')}";
        }

        var normalizedBase = baseUri.AbsoluteUri.EndsWith('/') ? baseUri : new Uri($"{baseUri.AbsoluteUri}/");

        var result = Uri.TryCreate(normalizedBase, safeUrl.TrimStart('/'), out var combined)
                        ? combined.ToString()
                        : $"{normalizedBase.AbsoluteUri}{safeUrl.TrimStart('/')}";

        return result;
    }

    /// <summary>
    /// Ejecuta <c>InternalLogUrlPagination</c>.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="baseAddress">Valor de <c>baseAddress</c>.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    protected virtual void InternalLogUrlPagination(ILogger<HttpApiInvoker> logger,
                                                    string                            baseAddress, 
                                                    string                            url,
                                                    IntNonNegative                    pageNumber,
                                                    IntNonNegative                    pageSize)
        => logger.LogInformation($"Consultando datos paginados a la url {BuildRequestUri(baseAddress, url)}. PageNumber {pageNumber}, PageSize {pageSize}");


    /// <summary>
    /// Ejecuta <c>InternalAddPagInfo</c>. Versión asíncrona.
    /// </summary>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual Task<MlResult<HttpClient>> InternalAddPagInfoAsync(ILogger<HttpApiInvoker> logger,
                                                                         HttpClient                        client,
                                                                         IntNonNegative                    pageNumber,
                                                                         IntNonNegative                    pageSize)
        => logger.LogMlResultInformationAsync($"Añadiendo información a las cabeceras del httpClient: PageNumber {pageNumber}, PageSize {pageSize}")
                                .BindAsync(_ => client.AddPaginationHeadersAsync(pageNumber, pageSize));


    /// <summary>
    /// Punto 29: las cabeceras de paginacion se escriben en el <see cref="HttpRequestMessage"/> de
    /// esta llamada concreta, nunca en <c>HttpClient.DefaultRequestHeaders</c>. El cliente procede
    /// de <see cref="IHttpClientFactory"/> y se reutiliza entre peticiones: mutar sus cabeceras por
    /// defecto las filtraria a llamadas ajenas y no es seguro entre hilos.
    /// </summary>
    protected virtual MlResult<HttpRequestMessage> BuildPagedRequest(HttpMethod                  method,
                                                                     string                      url,
                                                                     IntNonNegative              pageNumber,
                                                                     IntNonNegative              pageSize,
                                                                     Dictionary<string, string>? headers = null!)
        => new HttpRequestMessage(method, url).TrySetHeaders(headers ?? [])
                                .Bind(request => request.AddPaginationHeaders(pageNumber, pageSize));


    /// <summary>
    /// Envia la petición de <c>PagedRequest</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="method">Valor de <c>method</c>.</param>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<T>> SendPagedRequestAsync<T>(HttpClient          client,
                                                                       HttpMethod          method,
                                                                       PagedApiCallOptions parameters,
                                                                       CancellationToken   ct = default)
    {
        var result = await BuildPagedRequest(method, parameters.Url, parameters.PageNumber, parameters.PageSize, parameters.Headers).ToAsync()
                                    .TryMapAsync(request        => client.SendAsync(request, ct))
                                    .BindAsync  (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync(response       => response.Content.ReadFromJsonAsync<T>(cancellationToken: ct));
        return result!;
    }


    /// <summary>
    /// Envia la petición de <c>AndReadPaged</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="methodName">Nombre del elemento.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    protected virtual async Task<MlResult<TResponse?>> SendAndReadPagedAsync<TRequest, TResponse>(PagedApiCallOptions<TRequest> parameters, HttpClient client, MemberName methodName)
    {
        var result = await EnsureFp.NotNullAsync(parameters.RequestBody, $"{nameof(parameters.RequestBody)} no puede ser nulo")
                                    .TryMapAsync( _             => JsonSerializer.Serialize(parameters.RequestBody))
                                    .BindAsync  (jsonBody       => new HttpRequestMessage(HttpMethod.Post, parameters.Url)
                                                                             {
                                                                                 Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                                                                             }.TrySetHeadersAsync(parameters.Headers ?? []))
                                    .BindAsync  (request        => request.AddPaginationHeadersAsync(parameters.PageNumber, parameters.PageSize))
                                    .TryMapAsync(request        => client.SendAsync(request, parameters.CancellationToken))
                                    .BindAsync  (async response => response.IsSuccessStatusCode
                                                                        ? response.ToMlResultValid()
                                                                        : (await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>())
                                    .TryMapAsync(response       => response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: parameters.CancellationToken))
                                            .MyMethodFinalLogAsync(_logger, $"{methodName}<{typeof(TRequest).Name}, {typeof(TResponse).Name}>");
        return result;
    }


    #endregion

}



