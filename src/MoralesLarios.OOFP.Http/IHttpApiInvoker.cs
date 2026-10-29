using MoralesLarios.OOFP.Http.Requests;

namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Contrato de <c>IHttpApiInvoker</c>.
/// </summary>
public interface IHttpApiInvoker
{
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
    Task<MlResult<TResult>> PostAndReadAsync<T, TResult>(ServiceKey                        clientName,
                                                     T                          itemBody,
                                                     string                     url     = null!, // es un parámetro opcional ya que el post de add, normalmente lleva ya la url completa en el BaseAddress. Esto solo se rellenaría en caso de que no hubiera BaseAddress del cliente registrado
                                                     Dictionary<string, string> headers = null!,
                                                     CancellationToken          ct      = default);
    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST de <c>AndReadPaged</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de la petición.</typeparam>
    /// <typeparam name="TItem">Tipo de los elementos de la colección.</typeparam>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<PagedResult<TItem>>> PostAndReadPagedAsync<TRequest, TItem>(PagedApiCallOptions<TRequest> parameters, CancellationToken ct = default);
    /// <summary>
    /// Crea la instancia solicitada de <c>HttpClient</c>.
    /// </summary>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    MlResult<HttpClient> CreateHttpClient(ServiceKey clientName);
    /// <summary>
    /// Obtiene el elemento solicitado. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<T>> GetAsync<T>(ServiceKey clientName, string url = "", Dictionary<string, string> headers = null, CancellationToken ct = default);
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
    Task<MlResult<T>> PostAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
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
    Task<MlResult<EmptyValue>> PutAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
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
    Task<MlResult<EmptyValue>> DeleteAsync<T>(ServiceKey clientName, T itemBody, string url = null!, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="clientName">Nombre del elemento.</param>
    /// <param name="url">Dirección a la que se dirige la llamada.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> DeleteByIdAsync<T>(ServiceKey clientName, NotEmptyString url, Dictionary<string, string> headers = null!, CancellationToken ct = default);
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
    Task<MlResult<TResponse>> PostAsync<T, TResponse>(ServiceKey clientName, T itemBody, string url = null, Dictionary<string, string> headers = null, CancellationToken ct = default);
}