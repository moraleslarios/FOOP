namespace MoralesLarios.OOFP.Http;

internal static class RestClientPathEncoding
{
    // ASP.NET Core decodes one layer while binding route values; the controller decodes the remaining layer.
    internal static string EscapeSimpleId(string id)
        => Uri.EscapeDataString(Uri.EscapeDataString(id));

    internal static string EscapeCompositeKey(string componentEscapedKey)
        => Uri.EscapeDataString(componentEscapedKey);
}