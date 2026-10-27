// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Http;

public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>> _logger,
                               IHttpClientFactoryManager  _httpClientFactoryManager,
                               ServiceKey                        _httpClientFactoryKey) : IGenClientFp<TDto>
{ 

    public IHttpClientFactoryManager GetIHttpClientFactoryManager() => _httpClientFactoryManager;
    public ServiceKey                       GetHttpClientFactoryKey()      => _httpClientFactoryKey;


    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<IEnumerable<TDto>>(_httpClientFactoryKey, string.Empty, headers, ct);

    public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<TDto>(_httpClientFactoryKey, $"id-str/{idStr}", headers, ct);

    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _httpClientFactoryManager.PostAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _httpClientFactoryManager.PutAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr, 
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _httpClientFactoryManager.PutAsync(_httpClientFactoryKey, itemBody, idStr, headers, ct);


    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteByIdAsync<TDto>(_httpClientFactoryKey, $"{idStr}", headers, ct);



}


/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/






public class GenClientFp<TRequest, TResponse>(ILogger<GenClientFp<TRequest, TResponse>> _logger,
                                              IHttpClientFactoryManager                 _httpClientFactoryManager,
                                              ServiceKey                                       _httpClientFactoryKey) : IGenClientFp<TRequest, TResponse>
{ 

    public IHttpClientFactoryManager GetIHttpClientFactoryManager() => _httpClientFactoryManager;
    public ServiceKey                       GetHttpClientFactoryKey()      => _httpClientFactoryKey;


    public virtual Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<IEnumerable<TResponse>>(_httpClientFactoryKey, string.Empty, headers, ct);

    public virtual Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<TResponse>(_httpClientFactoryKey, $"id-str/{idStr}", headers, ct);

    public virtual Task<MlResult<TResponse>> PostAsync(TRequest                   itemBody, 
                                                       Dictionary<string, string> headers = null!, 
                                                       CancellationToken          ct      = default)
        => _httpClientFactoryManager.PostAsync<TRequest, TResponse>(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutAsync(TRequest                   itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _httpClientFactoryManager.PutAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr,
                                                      TRequest                   itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _httpClientFactoryManager.PutAsync(_httpClientFactoryKey, itemBody, idStr, headers, ct);


    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TRequest                   itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteByIdAsync<TResponse>(_httpClientFactoryKey, $"{idStr}", headers, ct);



}