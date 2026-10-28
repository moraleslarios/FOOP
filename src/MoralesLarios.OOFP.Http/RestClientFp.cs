// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Representa <c>RestClientFp</c>.
/// </summary>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
/// <param name="_logger">Registrador utilizado para trazar las llamadas.</param>
/// <param name="_apiInvoker">Invocador HTTP que ejecuta las peticiones.</param>
/// <param name="_clientName">Clave del servicio que identifica el cliente HTTP a utilizar.</param>
public partial class RestClientFp<TDto>(ILogger<RestClientFp<TDto>> _logger,
                               IHttpApiInvoker  _apiInvoker,
                               ServiceKey                        _clientName) : IRestClientFp<TDto>
{ 

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ApiInvoker</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    public IHttpApiInvoker GetApiInvoker() => _apiInvoker;
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ClientName</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    public ServiceKey                       GetClientName()      => _clientName;


    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<IEnumerable<TDto>>(_clientName, string.Empty, headers, ct);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<TDto>(_clientName, $"id-str/{idStr}", headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PostAsync(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr, 
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, idStr, headers, ct);


    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.DeleteAsync(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
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






/// <summary>
/// Representa <c>RestClientFp</c>.
/// </summary>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
/// <param name="_logger">Registrador utilizado para trazar las llamadas.</param>
/// <param name="_apiInvoker">Invocador HTTP que ejecuta las peticiones.</param>
/// <param name="_clientName">Clave del servicio que identifica el cliente HTTP a utilizar.</param>
public partial class RestClientFp<TRequest, TResponse>(ILogger<RestClientFp<TRequest, TResponse>> _logger,
                                              IHttpApiInvoker                 _apiInvoker,
                                              ServiceKey                                       _clientName) : IRestClientFp<TRequest, TResponse>
{ 

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ApiInvoker</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    public IHttpApiInvoker GetApiInvoker() => _apiInvoker;
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ClientName</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    public ServiceKey                       GetClientName()      => _clientName;


    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<IEnumerable<TResponse>>(_clientName, string.Empty, headers, ct);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.GetAsync<TResponse>(_clientName, $"id-str/{idStr}", headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TResponse>> PostAsync(TRequest                   itemBody, 
                                                       Dictionary<string, string> headers = null!, 
                                                       CancellationToken          ct      = default)
        => _apiInvoker.PostAsync<TRequest, TResponse>(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutAsync(TRequest                   itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString             idStr,
                                                      TRequest                   itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => _apiInvoker.PutAsync(_clientName, itemBody, idStr, headers, ct);


    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TRequest                   itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _apiInvoker.DeleteAsync(_clientName, itemBody, string.Empty, headers, ct);

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _apiInvoker.DeleteByIdAsync<TResponse>(_clientName, $"{idStr}", headers, ct);



}