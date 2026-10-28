// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Http.Helpers;

/// <summary>
/// Métodos de extension de <c>MlResponseWeb</c>.
/// </summary>
public static partial class MlResponseWebExtensions
{

    /// <summary>
    /// Numero maximo de caracteres del cuerpo de la respuesta que se incluyen en la descripcion
    /// del error. Evita volcar respuestas enormes (o datos sensibles completos) en logs y en
    /// mensajes que puedan llegar al consumidor.
    /// </summary>
    public const int DefaultMaxErrorBodyLength = 2_048;

    /// <summary>
    /// Punto 17: permite desactivar por completo la inclusion del cuerpo de la respuesta en la
    /// descripcion del error. Util en entornos donde el cuerpo puede contener informacion
    /// sensible (trazas de servidor, datos personales, etc.).
    /// </summary>
    public static bool IncludeResponseBodyInErrors { get; set; } = true;

    /// <summary>
    /// Lee el cuerpo de una respuesta HTTP fallida y construye una descripción legible del error.
    /// Si el contenido es JSON, lo reformatea indentado para facilitar su lectura.
    /// El cuerpo se trunca a <paramref name="maxBodyLength"/> caracteres.
    /// </summary>
    /// <returns>Cadena vacía si la respuesta es satisfactoria.</returns>
    public static async Task<string> ReadErrorDescriptionAsync(this HttpResponseMessage response, 
                                                                   int                  maxBodyLength = DefaultMaxErrorBodyLength)
    {
        if (response.IsSuccessStatusCode) return string.Empty;

        string errorContent;

        if (! IncludeResponseBodyInErrors)
        {
            errorContent = "[Contenido del error omitido por configuración]";
        }
        else
        {
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

                errorContent = Truncate(errorContent, maxBodyLength);
            }
            catch
            {
                // Si falla la lectura o el parseo, no se puede aportar detalle
                errorContent = "[No se pudo leer el contenido del error]";
            }
        }

        var result = @$"Se ha producido un error en la llamada al servicio.
                    Código de error: {(int)response.StatusCode}
                    Razón: {response.ReasonPhrase}
                    Detalle error: {errorContent}";

        return result;
    }

    private static string Truncate(string source, int maxLength)
    {
        if (maxLength <= 0                     ) return string.Empty;
        if (string.IsNullOrEmpty(source)       ) return source;
        if (source.Length <= maxLength         ) return source;

        var result = $"{source[..maxLength]}… [truncado: {source.Length - maxLength} caracteres omitidos]";

        return result;
    }

}
