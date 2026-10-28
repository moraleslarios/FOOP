// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;

/// <summary>
/// Métodos de extension de <c>HttpRequestPagination</c>.
/// </summary>
public static partial class HttpRequestPaginationExtensions
{

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Header</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<NotEmptyString> GetHeader(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => NotEmptyString.FromString(headerValue));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>Header</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<NotEmptyString>> GetHeaderAsync(this HttpRequest request, MemberName headerKey)
        => request.GetHeader(headerKey).ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>IntHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntNonNegative> GetIntHeader(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => int.TryParse(headerValue, out var intValue)
                                                        ? MlResult<IntNonNegative>.Valid(intValue)
                                                        : $"Header '{headerKey}' value '{headerValue}' is not a valid integer.".ToMlResultFail<IntNonNegative>());
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>IntHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <param name="headerKey">Clave identificativa.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IntNonNegative>> GetIntHeaderAsync(this HttpRequest request, MemberName headerKey)
        => request.GetIntHeader(headerKey).ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageNumberHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntNonNegative> GetPageNumberHeader(this HttpRequest request)
    {
        MemberName headerKey = global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber;

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetIntHeader(request, headerKey));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageNumberHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IntNonNegative>> GetPageNumberHeaderAsync(this HttpRequest request)
        => request.GetPageNumberHeader().ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageSizeHeader</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntNonNegative> GetPageSizeHeader(this HttpRequest request)
    {
        MemberName headerKey = global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize;

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetIntHeader(request, headerKey));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageSizeHeader</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<IntNonNegative>> GetPageSizeHeaderAsync(this HttpRequest request)
        => request.GetPageSizeHeader().ToAsync();

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PaginationHeaders</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> GetPaginationHeaders(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _             => GetPageNumberHeader(request)
                                                            .CreateCompleteMlResult(GetPageSizeHeader(request)))
                                .Map (paginationInfo => (PageNumber: paginationInfo.Item1, PageSize: paginationInfo.Item2));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PaginationHeaders</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)>> GetPaginationHeadersAsync(this HttpRequest request)
        => request.GetPaginationHeaders().ToAsync();


    private static MlResult<string> InternalReadHeaderRequest(this HttpRequest request, MemberName headerKey)
    {
        var exists = request.Headers.TryGetValue(headerKey, out var headerValue);

        var result = exists
            ? headerValue.ToString().ToMlResultValid()
            : $"Header '{headerKey}' not found.".ToMlResultFail<string>();

        return result;
    }



    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageRequest</c>.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<PageRequest> GetPageRequest(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetPageNumberHeader(request)
                                                            .CreateCompleteMlResult(GetPageSizeHeader(request)))
                                .Map(paginationInfo => new PageRequest(paginationInfo.Item1, paginationInfo.Item2));
        return result;
    }

    /// <summary>
    /// Obtiene el elemento solicitado de <c>PageRequest</c>. Versión asíncrona.
    /// </summary>
    /// <param name="request">Petición que se envia.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static Task<MlResult<PageRequest>> GetPageRequestAsync(this HttpRequest request)
        => request.GetPageRequest().ToAsync();





}

