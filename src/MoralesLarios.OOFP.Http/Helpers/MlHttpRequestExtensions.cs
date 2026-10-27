// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Http.Helpers;

public static partial class MlHttpRequestExtensions
{
    public static MlResult<HttpClient> AddHeader(this HttpClient client, MemberName headerKey, string headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid(_ => client.DefaultRequestHeaders.Add(headerKey, headerValue))
                                .Map(_ => client);
        return result;
    }

    public static Task<MlResult<HttpClient>> AddHeaderAsync(this HttpClient client, MemberName headerKey, string headerValue)
        => client.AddHeader(headerKey, headerValue).ToAsync();


    public static MlResult<HttpClient> AddIntHeader(this HttpClient client, MemberName headerKey, int headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid( _ => client.DefaultRequestHeaders.Add(headerKey, headerValue.ToString()))
                                .Map(_ => client);
        return result;
    }

    public static Task<MlResult<HttpClient>> AddIntHeaderAsync(this HttpClient client, MemberName headerKey, int headerValue)
        => client.AddIntHeader(headerKey, headerValue).ToAsync();

    public static MlResult<HttpClient> AddPageNumberHeader(this HttpClient client, IntNonNegative pageNumber)
        => AddIntHeader(client, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber, (int)pageNumber);

    public static Task<MlResult<HttpClient>> AddPageNumberHeaderAsync(this HttpClient client, IntNonNegative pageNumber)
        => client.AddPageNumberHeader(pageNumber).ToAsync();

    public static MlResult<HttpClient> AddPageSizeHeader(this HttpClient client, IntNonNegative pageSize)
        => AddIntHeader(client, global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize, (int)pageSize);

    public static Task<MlResult<HttpClient>> AddPageSizeHeaderAsync(this HttpClient client, IntNonNegative pageSize)
        => client.AddPageSizeHeader(pageSize).ToAsync();

    public static MlResult<HttpClient> AddPaginationHeaders(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => AddPageNumberHeader(client, pageNumber)
            .Bind( _ => AddPageSizeHeader(client, pageSize));

    public static Task<MlResult<HttpClient>> AddPaginationHeadersAsync(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => client.AddPaginationHeaders(pageNumber, pageSize).ToAsync();




    public static MlResult<HttpRequestMessage> AddHeader(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
    {
        var result =                                      EnsureFp.NotNull(request, $"{nameof(headerKey)} cannot be null if we want to set information in the header. ")
                                .Bind              ( _ => EnsureFp.NotNull(headerValue, $"{nameof(headerValue)} cannot be null if we want to set information in the header. "))
                                .TryExecSelfIfValid( _ => request.Headers.Add(headerKey, headerValue))
                                .Map               ( _ => request);
        return result;
    }

    public static Task<MlResult<HttpRequestMessage>> AddHeaderAsync(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
        => request.AddHeader(headerKey, headerValue).ToAsync();

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

    public static Task<MlResult<HttpRequestMessage>> TrySetHeadersAsync(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues)
        => request.TrySetHeaders(headerKeyValues).ToAsync();

}

