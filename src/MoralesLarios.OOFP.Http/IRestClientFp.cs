namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Contrato de <c>IRestClientFp</c>.
/// </summary>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
public interface IRestClientFp<TDto>
{
    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> DeleteAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ApiInvoker</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    IHttpApiInvoker GetApiInvoker();
    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TDto>> PostAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> PutAsync(TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString idStr, TDto itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}



/// <summary>
/// Contrato de <c>IRestClientFp</c>.
/// </summary>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
public interface IRestClientFp<TRequest, TResponse>
{
    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> DeleteAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> DeleteByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> GetByIdAsync(NotEmptyString idStr, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>ApiInvoker</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>IHttpApiInvoker</c> obtenido por la operación.</returns>
    IHttpApiInvoker GetApiInvoker();
    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<TResponse>> PostAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> PutAsync(TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="idStr">Valor de <c>idStr</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    Task<MlResult<EmptyValue>> PutByIdAsync(NotEmptyString idStr, TRequest itemBody, Dictionary<string, string> headers = null!, CancellationToken ct = default);
}