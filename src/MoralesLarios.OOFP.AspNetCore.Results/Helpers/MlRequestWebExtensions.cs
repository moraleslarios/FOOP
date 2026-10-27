// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;

public static class MlRequestWebExtensions
{

    public static MlResult<NotEmptyString> GetHeaderInfo(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => NotEmptyString.FromString(headerValue));
        return result;
    }

    public static Task<MlResult<NotEmptyString>> GetHeaderInfoAsync(this HttpRequest request, MemberName headerKey)
        => request.GetHeaderInfo(headerKey).ToAsync();

    public static MlResult<IntNonNegative> GetHeaderInfoAsIntNotNegative(this HttpRequest request, MemberName headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => int.TryParse(headerValue, out var intValue)
                                                        ? MlResult<IntNonNegative>.Valid(intValue)
                                                        : $"Header '{headerKey}' value '{headerValue}' is not a valid integer.".ToMlResultFail<IntNonNegative>());
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetHeaderInfoAsIntNotNegativeAsync(this HttpRequest request, MemberName headerKey)
        => request.GetHeaderInfoAsIntNotNegative(headerKey).ToAsync();

    public static MlResult<IntNonNegative> GetHeaderPageNumber(this HttpRequest request)
    {
        MemberName headerKey = "X-Page-Number";

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetHeaderInfoAsIntNotNegative(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetHeaderPageNumberAsync(this HttpRequest request)
        => request.GetHeaderPageNumber().ToAsync();

    public static MlResult<IntNonNegative> GetHeaderPageSize(this HttpRequest request)
    {
        MemberName headerKey = "X-Page-Size";

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetHeaderInfoAsIntNotNegative(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNonNegative>> GetHeaderPageSizeAsync(this HttpRequest request)
        => request.GetHeaderPageSize().ToAsync();

    public static MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)> GetHeaderPageInfo(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _             => GetHeaderPageNumber(request)
                                                            .CreateCompleteMlResult(GetHeaderPageSize(request)))
                                .Map (paginationInfo => (PageNumber: paginationInfo.Item1, PageSize: paginationInfo.Item2));
        return result;
    }

    public static Task<MlResult<(IntNonNegative PageNumber, IntNonNegative PageSize)>> GetHeaderPageInfoAsync(this HttpRequest request)
        => request.GetHeaderPageInfo().ToAsync();


    private static MlResult<string> InternalReadHeaderRequest(this HttpRequest request, MemberName headerKey)
    {
        var exists = request.Headers.TryGetValue(headerKey, out var headerValue);

        var result = exists
            ? headerValue.ToString().ToMlResultValid()
            : $"Header '{headerKey}' not found.".ToMlResultFail<string>();

        return result;
    }



    public static MlResult<PaginationInfo> GetHeaderPaginationInfo(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetHeaderPageNumber(request)
                                                            .CreateCompleteMlResult(GetHeaderPageSize(request)))
                                .Map(paginationInfo => new PaginationInfo(paginationInfo.Item1, paginationInfo.Item2));
        return result;
    }

    public static Task<MlResult<PaginationInfo>> GetHeaderPaginationInfoAsync(this HttpRequest request)
        => request.GetHeaderPaginationInfo().ToAsync();





}

