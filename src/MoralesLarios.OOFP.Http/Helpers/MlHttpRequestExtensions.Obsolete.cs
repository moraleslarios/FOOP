// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.Http.Helpers;
public static partial class MlHttpRequestExtensions
{
    [global::System.Obsolete("Renamed to AddPaginationHeaders. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageInfo(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize) => AddPaginationHeaders(client, pageNumber, pageSize);
    [global::System.Obsolete("Renamed to AddPaginationHeadersAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageInfoAsync(this HttpClient client, IntNonNegative pageNumber, IntNonNegative pageSize) => AddPaginationHeadersAsync(client, pageNumber, pageSize);
    [global::System.Obsolete("Renamed to AddPageNumberHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageNumber(this HttpClient client, IntNonNegative pageNumber) => AddPageNumberHeader(client, pageNumber);
    [global::System.Obsolete("Renamed to AddPageNumberHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageNumberAsync(this HttpClient client, IntNonNegative pageNumber) => AddPageNumberHeaderAsync(client, pageNumber);
    [global::System.Obsolete("Renamed to AddPageSizeHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderPageSize(this HttpClient client, IntNonNegative pageSize) => AddPageSizeHeader(client, pageSize);
    [global::System.Obsolete("Renamed to AddPageSizeHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderPageSizeAsync(this HttpClient client, IntNonNegative pageSize) => AddPageSizeHeaderAsync(client, pageSize);
    [global::System.Obsolete("Renamed to AddIntHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderInfoAsInt(this HttpClient client, MemberName headerKey, int headerValue) => AddIntHeader(client, headerKey, headerValue);
    [global::System.Obsolete("Renamed to AddIntHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderInfoAsIntAsync(this HttpClient client, MemberName headerKey, int headerValue) => AddIntHeaderAsync(client, headerKey, headerValue);
    [global::System.Obsolete("Renamed to AddHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpClient> SetHeaderInfo(this HttpClient client, MemberName headerKey, string headerValue) => AddHeader(client, headerKey, headerValue);
    [global::System.Obsolete("Renamed to AddHeader. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpRequestMessage> SetHeaderInfo(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue) => AddHeader(request, headerKey, headerValue);
    [global::System.Obsolete("Renamed to AddHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpClient>> SetHeaderInfoAsync(this HttpClient client, MemberName headerKey, string headerValue) => AddHeaderAsync(client, headerKey, headerValue);
    [global::System.Obsolete("Renamed to AddHeaderAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpRequestMessage>> SetHeaderInfoAsync(this HttpRequestMessage request, MemberName headerKey, NotEmptyString headerValue) => AddHeaderAsync(request, headerKey, headerValue);
    [global::System.Obsolete("Renamed to TrySetHeaders. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<HttpRequestMessage> SetHeaders(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues) => TrySetHeaders(request, headerKeyValues);
    [global::System.Obsolete("Renamed to TrySetHeadersAsync. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static Task<MlResult<HttpRequestMessage>> SetHeadersAsync(this HttpRequestMessage request, Dictionary<string, string> headerKeyValues) => TrySetHeadersAsync(request, headerKeyValues);
}