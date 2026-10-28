// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Http.Helpers;
/// <summary>
/// Métodos de extension de <c>MlHttpRequest</c>.
/// </summary>
public static partial class MlHttpRequestExtensions
{
    /// <summary>
    /// Ejecuta <c>SetHeaderPageInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPaginationHeaders. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageInfo(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize) => AddPaginationHeaders(client, pageNumber, pageSize);
    /// <summary>
    /// Ejecuta <c>SetHeaderPageInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPaginationHeadersAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageInfoAsync(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize) => AddPaginationHeadersAsync(client, pageNumber, pageSize);
    /// <summary>
    /// Ejecuta <c>SetHeaderPageNumber</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPageNumberHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageNumber(this HttpClient client, IntNonNegative pageNumber) => AddPageNumberHeader(client, pageNumber);
    /// <summary>
    /// Ejecuta <c>SetHeaderPageNumber</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPageNumberHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageNumberAsync(this HttpClient client, IntNonNegative pageNumber) => AddPageNumberHeaderAsync(client, pageNumber);
    /// <summary>
    /// Ejecuta <c>SetHeaderPageSize</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPageSizeHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageSize(this HttpClient client, IntNonNegative pageSize) => AddPageSizeHeader(client, pageSize);
    /// <summary>
    /// Ejecuta <c>SetHeaderPageSize</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddPageSizeHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageSizeAsync(this HttpClient client, IntNonNegative pageSize) => AddPageSizeHeaderAsync(client, pageSize);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfoAsInt</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddIntHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderInfoAsInt(this HttpClient client, MemberName headerKey, int headerValue) => AddIntHeader(client, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfoAsInt</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddIntHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderInfoAsIntAsync(this HttpClient client, MemberName headerKey, int headerValue) => AddIntHeaderAsync(client, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderInfo(this HttpClient client, MemberName headerKey, string headerValue) => AddHeader(client, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpRequestMessage> SetHeaderInfo(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue) => AddHeader(request, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderInfoAsync(this HttpClient client, MemberName headerKey, string headerValue) => AddHeaderAsync(client, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaderInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to AddHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpRequestMessage>> SetHeaderInfoAsync(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue) => AddHeaderAsync(request, headerKey, headerValue);
    /// <summary>
    /// Ejecuta <c>SetHeaders</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKeyValues">Valor de <c>headerKeyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TrySetHeaders. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpRequestMessage> SetHeaders(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues) => TrySetHeaders(request, headerKeyValues);
    /// <summary>
    /// Ejecuta <c>SetHeaders</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKeyValues">Valor de <c>headerKeyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to TrySetHeadersAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpRequestMessage>> SetHeadersAsync(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues) => TrySetHeadersAsync(request, headerKeyValues);
}