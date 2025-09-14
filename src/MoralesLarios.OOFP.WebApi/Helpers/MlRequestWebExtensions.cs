namespace MoralesLarios.OOFP.WebApi.Helpers;

public static class MlRequestWebExtensions
{

    public static MlResult<NotEmptyString> GetHeaderInfo(this HttpRequest request, Name headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => NotEmptyString.ByString(headerValue));
        return result;
    }

    public static Task<MlResult<NotEmptyString>> GetHeaderInfoAsync(this HttpRequest request, Name headerKey)
        => request.GetHeaderInfo(headerKey).ToAsync();

    public static MlResult<IntNotNegative> GetHeaderInfoAsIntNotNegative(this HttpRequest request, Name headerKey)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _          => InternalReadHeaderRequest(request, headerKey))
                                .Bind(headerValue => int.TryParse(headerValue, out var intValue)
                                                        ? MlResult<IntNotNegative>.Valid(intValue)
                                                        : $"Header '{headerKey}' value '{headerValue}' is not a valid integer.".ToMlResultFail<IntNotNegative>());
        return result;
    }

    public static Task<MlResult<IntNotNegative>> GetHeaderInfoAsIntNotNegativeAsync(this HttpRequest request, Name headerKey)
        => request.GetHeaderInfoAsIntNotNegative(headerKey).ToAsync();

    public static MlResult<IntNotNegative> GetHeaderPageNumber(this HttpRequest request)
    {
        Name headerKey = "X-Page-Number";

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetHeaderInfoAsIntNotNegative(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNotNegative>> GetHeaderPageNumberAsync(this HttpRequest request)
        => request.GetHeaderPageNumber().ToAsync();

    public static MlResult<IntNotNegative> GetHeaderPageSize(this HttpRequest request)
    {
        Name headerKey = "X-Page-Size";

        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind(_ => GetHeaderInfoAsIntNotNegative(request, headerKey));
        return result;
    }

    public static Task<MlResult<IntNotNegative>> GetHeaderPageSizeAsync(this HttpRequest request)
        => request.GetHeaderPageSize().ToAsync();

    public static MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)> GetHeaderPageInfo(this HttpRequest request)
    {
        var result = EnsureFp.NotNull(request, $"{nameof(request)} cannot be null if we want to get information from the header. ")
                                .Bind( _             => GetHeaderPageNumber(request)
                                                            .CreateCompleteMlResult(GetHeaderPageSize(request)))
                                .Map (paginationInfo => (PageNumber: paginationInfo.Item1, PageSize: paginationInfo.Item2));
        return result;
    }

    public static Task<MlResult<(IntNotNegative PageNumber, IntNotNegative PageSize)>> GetHeaderPageInfoAsync(this HttpRequest request)
        => request.GetHeaderPageInfo().ToAsync();


    private static MlResult<string> InternalReadHeaderRequest(this HttpRequest request, Name headerKey)
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
