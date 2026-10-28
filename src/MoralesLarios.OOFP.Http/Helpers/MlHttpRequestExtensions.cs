// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Http.Helpers;

/// <summary>
/// Métodos de extension de <c>MlHttpRequest</c>.
/// </summary>
public static partial class MlHttpRequestExtensions
{
    /// <summary>
    /// Añade el elemento indicado de <c>Header</c>.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpClient> AddHeader(this HttpClient client, MemberName headerKey, string headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid(_ => client.DefaultRequestHeaders.Add(headerKey, headerValue))
                                .Map(_ => client);
        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>Header</c>. Versión asíncrona.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpClient>> AddHeaderAsync(this HttpClient client, MemberName headerKey, string headerValue)
        => client.AddHeader(headerKey, headerValue).ToAsync();


    /// <summary>
    /// Añade el elemento indicado de <c>IntHeader</c>.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpClient> AddIntHeader(this HttpClient client, MemberName headerKey, int headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid( _ => client.DefaultRequestHeaders.Add(headerKey, headerValue.ToString()))
                                .Map(_ => client);
        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>IntHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpClient>> AddIntHeaderAsync(this HttpClient client, MemberName headerKey, int headerValue)
        => client.AddIntHeader(headerKey, headerValue).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>PageNumberHeader</c>.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpClient> AddPageNumberHeader(this HttpClient client, IntNonNegative pageNumber)
        => AddIntHeader(client, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber, (int)pageNumber);

    /// <summary>
    /// Añade el elemento indicado de <c>PageNumberHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpClient>> AddPageNumberHeaderAsync(this HttpClient client, IntNonNegative pageNumber)
        => client.AddPageNumberHeader(pageNumber).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>PageSizeHeader</c>.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpClient> AddPageSizeHeader(this HttpClient client, IntNonNegative pageSize)
        => AddIntHeader(client, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize, (int)pageSize);

    /// <summary>
    /// Añade el elemento indicado de <c>PageSizeHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpClient>> AddPageSizeHeaderAsync(this HttpClient client, IntNonNegative pageSize)
        => client.AddPageSizeHeader(pageSize).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>PaginationHeaders</c>.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpClient> AddPaginationHeaders(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => AddPageNumberHeader(client, pageNumber)
            .Bind( _ => AddPageSizeHeader(client, pageSize));

    /// <summary>
    /// Añade el elemento indicado de <c>PaginationHeaders</c>. Versión asíncrona.
    /// </summary>
    /// <param name="client">Cliente utilizado para la llamada.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpClient>> AddPaginationHeadersAsync(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => client.AddPaginationHeaders(pageNumber, pageSize).ToAsync();




    /// <summary>
    /// Añade el elemento indicado de <c>Header</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpRequestMessage> AddHeader(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
    {
        var result =                                      EnsureFp.NotNull(request, $"{nameof(headerKey)} cannot be null if we want to set information in the header. ")
                                .Bind              ( _ => EnsureFp.NotNull(headerValue, $"{nameof(headerValue)} cannot be null if we want to set information in the header. "))
                                .TryExecSelfIfValid( _ => request.Headers.Add(headerKey, headerValue))
                                .Map               ( _ => request);
        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>Header</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> AddHeaderAsync(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
        => request.AddHeader(headerKey, headerValue).ToAsync();

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por set headers.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKeyValues">Valor de <c>headerKeyValues</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpRequestMessage> TrySetHeaders(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(headerKeyValues)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid( _ =>
                                                        {
                                                            foreach (var kvp in headerKeyValues)
                                                            {
                                                                request.Headers.Add(kvp.Key, kvp.Value);
                                                            }
                                                        })
                                .Map                ( _ => request);
        return result;
    }

    /// <summary>
    /// Ejecuta la operación de forma protegida, capturando cualquier excepción y convirtiendola en un
    /// resultado fallido indicada por set headers. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKeyValues">Valor de <c>headerKeyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> TrySetHeadersAsync(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues)
        => request.TrySetHeaders(headerKeyValues).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>IntHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpRequestMessage> AddIntHeader(this HttpRequestMessage request, MemberName headerKey, int headerValue)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid( _ => request.Headers.Add(headerKey, headerValue.ToString(CultureInfo.InvariantCulture)))
                                .Map               ( _ => request);
        return result;
    }

    /// <summary>
    /// Añade el elemento indicado de <c>IntHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <param name="headerValue">Valor sobre el que se opera.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> AddIntHeaderAsync(this HttpRequestMessage request, MemberName headerKey, int headerValue)
        => request.AddIntHeader(headerKey, headerValue).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>PageNumberHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpRequestMessage> AddPageNumberHeader(this HttpRequestMessage request, IntNonNegative pageNumber)
        => AddIntHeader(request, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber, (int)pageNumber);

    /// <summary>
    /// Añade el elemento indicado de <c>PageNumberHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> AddPageNumberHeaderAsync(this HttpRequestMessage request, IntNonNegative pageNumber)
        => request.AddPageNumberHeader(pageNumber).ToAsync();

    /// <summary>
    /// Añade el elemento indicado de <c>PageSizeHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<HttpRequestMessage> AddPageSizeHeader(this HttpRequestMessage request, IntNonNegative pageSize)
        => AddIntHeader(request, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize, (int)pageSize);

    /// <summary>
    /// Añade el elemento indicado de <c>PageSizeHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> AddPageSizeHeaderAsync(this HttpRequestMessage request, IntNonNegative pageSize)
        => request.AddPageSizeHeader(pageSize).ToAsync();

    /// <summary>
    /// Punto 29: las cabeceras de paginación se añaden al <see cref="HttpRequestMessage"/> de cada
    /// llamada, NUNCA a <c>HttpClient.DefaultRequestHeaders</c>. El <c>HttpClient</c> procede de
    /// <c>IHttpClientFactory</c>, que reutiliza y comparte la instancia: mutar sus cabeceras por
    /// defecto las filtra entre peticiones (y entre usuarios) y no es seguro entre hilos.
    /// </summary>
    public static MlResult<HttpRequestMessage> AddPaginationHeaders(this HttpRequestMessage request, IntNonNegative pageNumber, IntNonNegative pageSize)
        => AddPageNumberHeader(request, pageNumber)
            .Bind( _ => AddPageSizeHeader(request, pageSize));

    /// <summary>
    /// Añade el elemento indicado de <c>PaginationHeaders</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="pageNumber">Número de pagina solicitada, comenzando en uno.</param>
    /// <param name="pageSize">Número de elementos por pagina.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<HttpRequestMessage>> AddPaginationHeadersAsync(this HttpRequestMessage request, IntNonNegative pageNumber, IntNonNegative pageSize)
        => request.AddPaginationHeaders(pageNumber, pageSize).ToAsync();
}

