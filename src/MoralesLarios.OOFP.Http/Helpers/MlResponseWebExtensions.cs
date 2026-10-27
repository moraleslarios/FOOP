// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Http.Helpers;

public static partial class MlResponseWebExtensions
{

    /// <summary>
    /// Lee el cuerpo de una respuesta HTTP fallida y construye una descripción legible del error.
    /// Si el contenido es JSON, lo reformatea indentado para facilitar su lectura.
    /// </summary>
    /// <returns>Cadena vacía si la respuesta es satisfactoria.</returns>
    public static async Task<string> ReadErrorDescriptionAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return string.Empty;

        string errorContent;

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
            // Si falla la lectura o el parseo, no se puede aportar detalle
            errorContent = "[No se pudo leer el contenido del error]";
        }

        var result = @$"Se ha producido un error en la llamada al servicio.
                    Código de error: {(int)response.StatusCode}
                    Razón: {response.ReasonPhrase}
                    Detalle error: {errorContent}";

        return result;
    }

}
