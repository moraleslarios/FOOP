// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.Http.Helpers;

public static class MlHttpRequestExtensions
{
    public static MlResult<HttpClient> SetHeaderInfo(this HttpClient client, MemberName headerKey, string headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid(_ => client.DefaultRequestHeaders.Add(headerKey, headerValue))
                                .Map(_ => client);
        return result;
    }

    public static Task<MlResult<HttpClient>> SetHeaderInfoAsync(this HttpClient client, MemberName headerKey, string headerValue)
        => client.SetHeaderInfo(headerKey, headerValue).ToAsync();


    public static MlResult<HttpClient> SetHeaderInfoAsInt(this HttpClient client, MemberName headerKey, int headerValue)
    {
        var result = EnsureFp.NotNull(client, $"{nameof(client)} cannot be null if we want to set information in the header. ")
                                .TryExecSelfIfValid( _ => client.DefaultRequestHeaders.Add(headerKey, headerValue.ToString()))
                                .Map(_ => client);
        return result;
    }

    public static Task<MlResult<HttpClient>> SetHeaderInfoAsIntAsync(this HttpClient client, MemberName headerKey, int headerValue)
        => client.SetHeaderInfoAsInt(headerKey, headerValue).ToAsync();

    public static MlResult<HttpClient> SetHeaderPageNumber(this HttpClient client, IntNonNegative pageNumber)
        => SetHeaderInfoAsInt(client, "X-Page-Number", (int)pageNumber);

    public static Task<MlResult<HttpClient>> SetHeaderPageNumberAsync(this HttpClient client, IntNonNegative pageNumber)
        => client.SetHeaderPageNumber(pageNumber).ToAsync();

    public static MlResult<HttpClient> SetHeaderPageSize(this HttpClient client, IntNonNegative pageSize)
        => SetHeaderInfoAsInt(client, "X-Page-Size", (int)pageSize);

    public static Task<MlResult<HttpClient>> SetHeaderPageSizeAsync(this HttpClient client, IntNonNegative pageSize)
        => client.SetHeaderPageSize(pageSize).ToAsync();

    public static MlResult<HttpClient> SetHeaderPageInfo(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => SetHeaderPageNumber(client, pageNumber)
            .Bind( _ => SetHeaderPageSize(client, pageSize));

    public static Task<MlResult<HttpClient>> SetHeaderPageInfoAsync(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize)
        => client.SetHeaderPageInfo(pageNumber, pageSize).ToAsync();




    public static MlResult<HttpRequestMessage> SetHeaderInfo(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
    {
        var result =                                      EnsureFp.NotNull(request, $"{nameof(headerKey)} cannot be null if we want to set information in the header. ")
                                .Bind              ( _ => EnsureFp.NotNull(headerValue, $"{nameof(headerValue)} cannot be null if we want to set information in the header. "))
                                .TryExecSelfIfValid( _ => request.Headers.Add(headerKey, headerValue))
                                .Map               ( _ => request);
        return result;
    }

    public static Task<MlResult<HttpRequestMessage>> SetHeaderInfoAsync(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue)
        => request.SetHeaderInfo(headerKey, headerValue).ToAsync();

    public static MlResult<HttpRequestMessage> SetHeaders(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues)
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

    public static Task<MlResult<HttpRequestMessage>> SetHeadersAsync(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues)
        => request.SetHeaders(headerKeyValues).ToAsync();

}

