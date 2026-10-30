// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers.Extensions;

/// <summary>
/// Métodos de extensión que volcan los datos de una <see cref="Exception"/> al diccionario de
/// metadatos de un error, respetando el esquema de ranuras reservadas
/// <c>Ex</c>, <c>Ex2</c>, <c>Ex3</c>…
/// </summary>
/// <remarks>
/// Esta clase forma parte del troceado de la antigua clase <c>Extensions</c>, que acumulaba
/// familias de métodos sin relación entre sí. El espacio de nombres
/// <c>MoralesLarios.OOFP.Helpers.Extensions</c> <b>no cambia</b>, de modo que la invocación en
/// forma de extensión (<c>metadatos.AppendExDetails(ex)</c>) sigue resolviéndose igual: el
/// compilador busca los métodos de extensión en <b>todas</b> las clases estáticas del espacio de
/// nombres importado, no en una clase concreta.
/// <para>
/// Sólo la invocación en forma <b>estática</b> (<c>Extensions.AppendExDetails(metadatos, ex)</c>)
/// queda afectada; para ella se conservan puentes <c>[Obsolete]</c> en
/// <c>Helpers/Extensions/Extensions.Obsolete.cs</c>, que desaparecerán en la versión 3.0.0.
/// </para>
/// </remarks>
public static class ExceptionMetadataExtensions
{

    /// <summary>
    /// Devuelve una copia del diccionario de metadatos con la excepción añadida en la primera
    /// ranura de excepción libre.
    /// </summary>
    /// <param name="source">Diccionario de metadatos de origen.</param>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Un diccionario <b>nuevo</b> con todas las entradas de origen más la excepción.</returns>
    /// <remarks>
    /// La excepción se almacena en la primera ranura libre de la secuencia reservada
    /// <c>Ex</c>, <c>Ex2</c>, <c>Ex3</c>… Sólo se consideran ocupadas las claves que encajan en el
    /// patrón <c>^Ex\d*$</c>: una clave propia como <c>"Expected"</c> o <c>"ExternalId"</c> no
    /// desplaza la ranura canónica. El diccionario de origen <b>no se modifica</b>.
    /// </remarks>
    public static Dictionary<string, object> AppendExDetails(this Dictionary<string, object> source, Exception ex)
    {
        var result = MlErrorDetails.CopyMetadata(source);

        result[MlErrorDetails.NextExceptionKey(result.Keys)] = ex;

        return result;
    }

}
