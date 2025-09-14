namespace MoralesLarios.OOFP.HttpClients.Helpers;

public static class MlResponseWebExtensions
{

    public static string ToResponseErrorsDescription(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return string.Empty;
        
        var result = @$"Se ha producido un error en la llamada al servicio. Código de error: {(int)response.StatusCode}.{Environment.NewLine} 
                        Razón: {response.ReasonPhrase}{Environment.NewLine}
                        Detalle error: {response.Content.ReadAsStringAsync().Result}";

        return result;
    }


}
