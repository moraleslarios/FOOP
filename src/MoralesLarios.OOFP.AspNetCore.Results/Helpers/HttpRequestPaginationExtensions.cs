// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;

public static partial class HttpRequestPaginationExtensions
{

    public static MlResult<NotEmptyString> GetHeader(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => NotEmptyString.FromString(headerValue));
        return result;
    }

    public static Task<MlResult<NotEmptyString>> GetHeaderAsync(this HttpRequest request, MemberName headerKey)
        => request.GetHeader(headerKey).ToAsync();

    public static MlResult<IntNonNegative> GetIntHeader(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => int.TryParse(headerValue, out var intValue)
                                                        ? MlResult<IntNonNegative>.Valid(intValue)
                                                        : $"Header '{headerKey}' value '{headerValue}' is not a valid integer.".ToMlResultFail<IntNonNegative>());
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetIntHeaderAsync(this HttpRequest request, MemberName headerKey)
        => request.GetIntHeader(headerKey).ToAsync();

    public static MlResult<IntNonNegative> GetPageNumberHeader(this HttpRequest request)
    {
        MemberName headerKey = global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber;

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetIntHeader(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetPageNumberHeaderAsync(this HttpRequest request)
        => request.GetPageNumberHeader().ToAsync();

    public static MlResult<IntNonNegative> GetPageSizeHeader(this HttpRequest request)
    {
        MemberName headerKey = global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize;

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetIntHeader(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetPageSizeHeaderAsync(this HttpRequest request)
        => request.GetPageSizeHeader().ToAsync();

    public static MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> GetPaginationHeaders(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _             => GetPageNumberHeader(request)
                                                            .CreateCompleteMlResult(GetPageSizeHeader(request)))
                                .Map (paginationInfo => (PageNumber: paginationInfo.Item1, PageSize: paginationInfo.Item2));
        return result;
    }

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



    public static MlResult<PageRequest> GetPageRequest(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetPageNumberHeader(request)
                                                            .CreateCompleteMlResult(GetPageSizeHeader(request)))
                                .Map(paginationInfo => new PageRequest(paginationInfo.Item1, paginationInfo.Item2));
        return result;
    }

    public static Task<MlResult<PageRequest>> GetPageRequestAsync(this HttpRequest request)
        => request.GetPageRequest().ToAsync();





}

