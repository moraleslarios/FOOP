// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Http;

public partial class RestClientFp<TDto>(ILogger<RestClientFp<TDto>> _logger,
                               IHttpApiInvoker  _apiInvoker,
                               ServiceKey                        _clientName) : IRestClientFp<TDto>
{ 

    public IHttpApiInvoker GetApiInvoker() => _apiInvoker;
    public ServiceKey                       GetClientName()      => _clientName;


    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<IEnumerable<TDto>>(_clientName, string.Empty, headers, ct);

    public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<TDto>(_clientName, $"id-str/{idStr}", headers, ct);

    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PostAsync(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr, 
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, idStr, headers, ct);


    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.DeleteAsync(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _apiInvoker.DeleteByIdAsync<TDto>(_clientName, $"{idStr}", headers, ct);



}


/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/






public partial class RestClientFp<TRequest, TResponse>(ILogger<RestClientFp<TRequest, TResponse>> _logger,
                                              IHttpApiInvoker                 _apiInvoker,
                                              ServiceKey                                       _clientName) : IRestClientFp<TRequest, TResponse>
{ 

    public IHttpApiInvoker GetApiInvoker() => _apiInvoker;
    public ServiceKey                       GetClientName()      => _clientName;


    public virtual Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<IEnumerable<TResponse>>(_clientName, string.Empty, headers, ct);

    public virtual Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<TResponse>(_clientName, $"id-str/{idStr}", headers, ct);

    public virtual Task<MlResult<TResponse>> PostAsync(TRequest                   itemBody, 
                                                       Dictionary<string, string> headers = null!, 
                                                       CancellationToken          ct      = default)
        => _apiInvoker.PostAsync<TRequest, TResponse>(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutAsync(TRequest                   itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr,
                                                      TRequest                   itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, idStr, headers, ct);


    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TRequest                   itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.DeleteAsync(_clientName, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _apiInvoker.DeleteByIdAsync<TResponse>(_clientName, $"{idStr}", headers, ct);



}