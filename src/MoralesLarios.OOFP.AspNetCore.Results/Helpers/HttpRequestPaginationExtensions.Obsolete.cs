// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;

/// <summary>
/// Métodos de extension de <c>HttpRequestPagination</c>.
/// </summary>
public static partial class HttpRequestPaginationExtensions
{

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<NotEmptyString> GetHeaderInfo(this HttpRequest request, MemberName headerKey)
        => request.GetHeader(headerKey);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<NotEmptyString>> GetHeaderInfoAsync(this HttpRequest request, MemberName headerKey)
        => request.GetHeaderAsync(headerKey);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderInfoAsIntNotNegative</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetIntHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntNonNegative> GetHeaderInfoAsIntNotNegative(this HttpRequest request, MemberName headerKey)
        => request.GetIntHeader(headerKey);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderInfoAsIntNotNegative</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetIntHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IntNonNegative>> GetHeaderInfoAsIntNotNegativeAsync(this HttpRequest request, MemberName headerKey)
        => request.GetIntHeaderAsync(headerKey);

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageNumber</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageNumberHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntNonNegative> GetHeaderPageNumber(this HttpRequest request)
        => request.GetPageNumberHeader();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageNumber</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageNumberHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IntNonNegative>> GetHeaderPageNumberAsync(this HttpRequest request)
        => request.GetPageNumberHeaderAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageSize</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageSizeHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<IntNonNegative> GetHeaderPageSize(this HttpRequest request)
        => request.GetPageSizeHeader();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageSize</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageSizeHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<IntNonNegative>> GetHeaderPageSizeAsync(this HttpRequest request)
        => request.GetPageSizeHeaderAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPaginationHeaders. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> GetHeaderPageInfo(this HttpRequest request)
        => request.GetPaginationHeaders();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPageInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPaginationHeadersAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)>> GetHeaderPageInfoAsync(this HttpRequest request)
        => request.GetPaginationHeadersAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPaginationInfo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageRequest. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<PageRequest> GetHeaderPaginationInfo(this HttpRequest request)
        => request.GetPageRequest();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>HeaderPaginationInfo</c>. Versión asíncrona.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    [global::System.Obsolete("Renamed to GetPageRequestAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<PageRequest>> GetHeaderPaginationInfoAsync(this HttpRequest request)
        => request.GetPageRequestAsync();

}
