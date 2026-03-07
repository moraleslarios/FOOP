namespace MoralesLarios.OOFP.HttpClients.Helpers;

public static class MlResponseWebExtensions
{

    //public static string ToResponseErrorsDescriptionAsync(this HttpResponseMessage response)
    //{
    //    if (response.IsSuccessStatusCode) return string.Empty;
        
    //    var result = @$"Se ha producido un error en la llamada al servicio. Código de error: {(int)response.StatusCode}.{Environment.NewLine} 
    //                    Razón: {response.ReasonPhrase}{Environment.NewLine}
    //                    Detalle error: {response.Content.ReadAsStringAsync().Result}";

    //    return result;
    //}


    public static async Task<string> ToResponseErrorsDescriptionAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return string.Empty;

        string errorContent = string.Empty;

        try
        {
            errorContent = await response.Content.ReadAsStringAsync();

            // Intenta deserializar si es JSON para un formato más legible
            if (!string.IsNullOrWhiteSpace(errorContent) && errorContent.TrimStart().StartsWith('{'))
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(errorContent);
                errorContent = System.Text.Json.JsonSerializer.Serialize(
                    jsonDoc.RootElement,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                );
            }
        }
        catch
        {
            // Si falla la lectura o parse, devuelve lo que sea
            errorContent = "[No se pudo leer el contenido del error]";
        }

        var result = @$"Se ha producido un error en la llamada al servicio.
                    Código de error: {(int)response.StatusCode}
                    Razón: {response.ReasonPhrase}
                    Detalle error: {errorContent}";

        return result;
    }


    public static string ToResponseErrorsDescription(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return string.Empty;
        string errorContent = string.Empty;
        try
        {
            errorContent = response.Content.ReadAsStringAsync().Result;
            // Intenta deserializar si es JSON para un formato más legible
            if (!string.IsNullOrWhiteSpace(errorContent) && errorContent.TrimStart().StartsWith('{'))
            {
                using var jsonDoc = System.Text.Json.JsonDocument.Parse(errorContent);
                errorContent = System.Text.Json.JsonSerializer.Serialize(
                    jsonDoc.RootElement,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                );
            }
        }
        catch
        {
            // Si falla la lectura o parse, devuelve lo que sea
            errorContent = "[No se pudo leer el contenido del error]";
        }
        var result = @$"Se ha producido un error en la llamada al servicio.
                    Código de error: {(int)response.StatusCode}
                    Razón: {response.ReasonPhrase}
                    Detalle error: {errorContent}";
        return result;
    }


}
