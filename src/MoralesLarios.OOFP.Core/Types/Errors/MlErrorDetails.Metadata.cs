// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.Types.Errors;

/// <summary>
/// Contrato de metadatos de <see cref="MlErrorDetails"/>: catálogo de claves reservadas, accesores de
/// lectura sin efectos secundarios y operaciones de incorporación que no modifican la instancia de partida.
/// </summary>
/// <remarks>
/// <para>
/// <b>Comparador de claves.</b> Los metadatos se indexan con <see cref="StringComparer.Ordinal"/>
/// (decisión D-16 de la épica 4 de la versión 2.1.1): la comparación distingue mayúsculas y minúsculas,
/// por lo que <c>"value"</c> y <c>"Value"</c> son claves distintas.
/// </para>
/// <para>
/// <b>Claves reservadas.</b> Las claves que el núcleo y las capas oficiales escriben están enumeradas en
/// <see cref="MetadataKeys"/>. Además, el prefijo <c>Ex</c> seguido de cero o más dígitos
/// (patrón <c>^Ex\d*$</c>) está reservado para las excepciones acumuladas: <c>Ex</c>, <c>Ex2</c>,
/// <c>Ex3</c>… Una clave propia que encaje en ese patrón será interpretada como excepción.
/// Claves como <c>"Expected"</c>, <c>"ExecutionTime"</c> o <c>"ExternalId"</c> <b>no</b> encajan y son
/// seguras.
/// </para>
/// <para>
/// <b>Política de colisión.</b> Al fusionar dos conjuntos de metadatos
/// (<see cref="MlErrorDetailsActions.Merge(MlErrorDetails, MlErrorDetails)"/>) nunca se pierde
/// información ni se lanza una excepción: la clave entrante que ya existe se sufija con el primer
/// índice libre (<c>ParamName</c>, <c>ParamName2</c>, <c>ParamName3</c>…). Las claves de excepción
/// siguen su propia secuencia reservada (decisión D-15).
/// </para>
/// <para>
/// <b>Inmutabilidad.</b> <see cref="MlErrorDetails.Metadata"/> expone el diccionario concreto por
/// compatibilidad binaria; la vía recomendada de lectura es <see cref="MetadataView"/> y la de
/// escritura, <see cref="WithMetadata(string, object)"/>, que devuelve una instancia nueva con una
/// copia de los metadatos. Ninguna instancia comparte el diccionario con otra.
/// </para>
/// </remarks>
public partial class MlErrorDetails : IEquatable<MlErrorDetails>
{

    #region Catalogo de claves reservadas

    /// <summary>
    /// Claves de metadatos reservadas por el núcleo y por las capas oficiales.
    /// </summary>
    /// <remarks>
    /// El prefijo <c>Ex</c> seguido de dígitos está reservado para las excepciones acumuladas:
    /// no deben usarse claves que encajen en el patrón <c>^Ex\d*$</c>.
    /// </remarks>
    public static class MetadataKeys
    {
        /// <summary>
        /// Excepción capturada. Tipo del valor: <see cref="System.Exception"/>.
        /// La escribe la familia <c>Try*</c> del núcleo.
        /// </summary>
        public const string Exception = "Ex";

        /// <summary>
        /// Valor de entrada que provocó el fallo. Tipo del valor: cualquiera.
        /// La escribe <c>AddValueMetadata</c> y la familia <c>AddValueIfFail</c> del núcleo.
        /// </summary>
        public const string Value = "Value";

        /// <summary>
        /// Nombre del parámetro que incumplió la guardia. Tipo del valor: <see cref="string"/>.
        /// La escribe <c>EnsureFp</c> en el núcleo.
        /// </summary>
        public const string ParameterName = "ParamName";

        /// <summary>
        /// Valor esperado por la guardia. Tipo del valor: cualquiera.
        /// La escribe <c>EnsureFp</c> en el núcleo.
        /// </summary>
        public const string Expected = "Expected";

        /// <summary>
        /// Índices de los elementos que fallaron en una validación de colección.
        /// Tipo del valor: colección de <see cref="int"/>. La escribe <c>EnsureFp</c> en el núcleo.
        /// </summary>
        public const string FailedIndexes = "FailedIndexes";

        /// <summary>
        /// Marca de recurso no encontrado. Tipo del valor: cualquiera.
        /// La escribe el consumidor y la consume la capa <c>MoralesLarios.OOFP.AspNetCore.Results</c>
        /// para emitir un <c>404 Not Found</c>.
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Detalles del problema en formato RFC 7807. Tipo del valor:
        /// <c>Microsoft.AspNetCore.Mvc.ProblemDetails</c>. Clave de capa web: la escribe y la consume
        /// <c>MoralesLarios.OOFP.AspNetCore.Results</c>, nunca el núcleo.
        /// </summary>
        public const string ProblemsDetails = "ProblemsDetails";
    }

    #endregion

    #region Comparador y metadatos vacios

    /// <summary>
    /// Comparador con el que se indexan las claves de metadatos. Decisión D-16: comparación ordinal,
    /// sensible a mayúsculas y minúsculas.
    /// </summary>
    internal static StringComparer MetadataKeyComparer => StringComparer.Ordinal;

    /// <summary>
    /// Vista de solo lectura compartida que se devuelve cuando no hay ningún metadato, para no
    /// asignar un diccionario vacío por cada fallo.
    /// </summary>
    internal static readonly IReadOnlyDictionary<string, object> EmptyMetadata =
        new ReadOnlyDictionary<string, object>(new Dictionary<string, object>(0, MetadataKeyComparer));

    private IReadOnlyDictionary<string, object>? metadataView;

    /// <summary>
    /// Obtiene los metadatos del error como una vista de solo lectura.
    /// </summary>
    /// <remarks>
    /// Es la vía de lectura recomendada: no permite mutación y, cuando no hay metadatos, devuelve una
    /// instancia vacía compartida sin asignar memoria. El tipo de
    /// <see cref="MlErrorDetails.Metadata"/> se mantiene como <see cref="Dictionary{TKey, TValue}"/>
    /// por compatibilidad binaria y pasará a <see cref="IReadOnlyDictionary{TKey, TValue}"/> en la
    /// versión 3.0.0.
    /// </remarks>
    public IReadOnlyDictionary<string, object> MetadataView
        => Metadata.Count == 0
               ? EmptyMetadata
               : metadataView ??= new ReadOnlyDictionary<string, object>(Metadata);

    #endregion

    #region Claves de excepcion

    [GeneratedRegex(@"^Ex\d*$", RegexOptions.CultureInvariant)]
    private static partial Regex ExceptionKeyRegex();

    /// <summary>
    /// Indica si la clave indicada es una de las ranuras reservadas para las excepciones acumuladas.
    /// </summary>
    /// <param name="key">Clave de metadatos a comprobar.</param>
    /// <returns>
    /// <see langword="true"/> si la clave encaja en el patrón <c>^Ex\d*$</c>; en caso contrario,
    /// <see langword="false"/>.
    /// </returns>
    internal static bool IsExceptionKey(string? key)
        => key is not null && ExceptionKeyRegex().IsMatch(key);

    /// <summary>
    /// Devuelve el orden de acumulación de una clave de excepción: <c>"Ex"</c> es 1, <c>"Ex2"</c> es 2,
    /// <c>"Ex10"</c> es 10.
    /// </summary>
    /// <param name="key">Clave de metadatos que encaja en el patrón de excepción.</param>
    /// <returns>Posición de la clave en la secuencia de acumulación.</returns>
    internal static int ExceptionKeyOrder(string key)
        => key.Length == MetadataKeys.Exception.Length
               ? 1
               : int.TryParse(key.AsSpan(MetadataKeys.Exception.Length), out var index) ? index : int.MaxValue;

    /// <summary>
    /// Calcula la siguiente clave libre de la secuencia reservada de excepciones.
    /// </summary>
    /// <param name="existingKeys">Claves ya presentes en los metadatos.</param>
    /// <returns>
    /// <c>"Ex"</c> si la ranura inicial está libre; en caso contrario, la primera clave
    /// <c>"Ex2"</c>, <c>"Ex3"</c>… que no esté ocupada. La secuencia resultante es siempre densa.
    /// </returns>
    /// <remarks>
    /// Es el <b>único</b> generador de claves de excepción del núcleo. Busca el primer hueco libre y
    /// no el siguiente al número de claves existentes, de modo que la función es idempotente frente a
    /// estados heredados inconsistentes.
    /// </remarks>
    internal static string NextExceptionKey(IEnumerable<string> existingKeys)
    {
        var used = existingKeys.Where(IsExceptionKey).ToHashSet(MetadataKeyComparer);

        if ( ! used.Contains(MetadataKeys.Exception)) return MetadataKeys.Exception;

        for (var index = 2; ; index++)
        {
            var candidate = $"{MetadataKeys.Exception}{index}";

            if ( ! used.Contains(candidate)) return candidate;
        }
    }

    #endregion

    #region Fusion de metadatos

    /// <summary>
    /// Incorpora al diccionario de destino todas las entradas de origen aplicando la política de
    /// colisión de la decisión D-15: ninguna entrada se pierde y ninguna llamada lanza.
    /// </summary>
    /// <param name="target">Diccionario de destino, que se modifica en el sitio.</param>
    /// <param name="source">Entradas a incorporar.</param>
    internal static void MergeMetadataInto(Dictionary<string, object>                 target,
                                           IEnumerable<KeyValuePair<string, object>>? source)
    {
        if (source is null) return;

        foreach (var entry in source)
        {
            if ( ! target.ContainsKey(entry.Key))
            {
                target[entry.Key] = entry.Value;
                continue;
            }

            if (IsExceptionKey(entry.Key))
            {
                target[NextExceptionKey(target.Keys)] = entry.Value;
                continue;
            }

            var index = 2;

            while (target.ContainsKey($"{entry.Key}{index}")) index++;

            target[$"{entry.Key}{index}"] = entry.Value;
        }
    }

    /// <summary>
    /// Crea un diccionario de metadatos nuevo con el comparador del núcleo y el contenido indicado.
    /// </summary>
    /// <param name="source">Entradas iniciales, o <see langword="null"/> para un diccionario vacío.</param>
    /// <returns>Diccionario nuevo e independiente del de origen.</returns>
    internal static Dictionary<string, object> CopyMetadata(IDictionary<string, object>? source)
        => source is null
               ? new Dictionary<string, object>(MetadataKeyComparer)
               : new Dictionary<string, object>(source, MetadataKeyComparer);

    #endregion

    #region Accesores de lectura

    /// <summary>
    /// Intenta obtener el metadato asociado a la clave indicada con la semántica de
    /// <see cref="IDictionary{TKey, TValue}.TryGetValue"/>.
    /// </summary>
    /// <typeparam name="T">Tipo esperado del valor.</typeparam>
    /// <param name="key">Clave identificativa del metadato.</param>
    /// <param name="value">Valor obtenido, o el valor predeterminado de <typeparamref name="T"/>.</param>
    /// <returns>
    /// <see langword="true"/> si la clave existe y su valor es de tipo <typeparamref name="T"/>; en
    /// caso contrario, <see langword="false"/>.
    /// </returns>
    /// <remarks>No lanza ninguna excepción y no modifica la instancia.</remarks>
    public bool TryGetMetadata<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        if (key is not null && Metadata.TryGetValue(key, out var raw) && raw is T typed)
        {
            value = typed;
            return true;
        }

        value = default;

        return false;
    }

    /// <summary>
    /// Intenta obtener la primera excepción acumulada en los metadatos.
    /// </summary>
    /// <param name="exception">Excepción obtenida, o <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> si hay una excepción en la ranura canónica <c>"Ex"</c>; en caso
    /// contrario, <see langword="false"/>.
    /// </returns>
    /// <remarks>No lanza ninguna excepción y no modifica la instancia.</remarks>
    public bool TryGetException([MaybeNullWhen(false)] out Exception exception)
        => TryGetMetadata(MetadataKeys.Exception, out exception);

    /// <summary>
    /// Obtiene todas las excepciones acumuladas en los metadatos, en orden de acumulación.
    /// </summary>
    /// <returns>
    /// Secuencia con las excepciones de las ranuras reservadas <c>"Ex"</c>, <c>"Ex2"</c>, <c>"Ex3"</c>…,
    /// vacía si no hay ninguna.
    /// </returns>
    /// <remarks>
    /// Es la vía recomendada de recuperación: libera al consumidor de conocer el esquema de claves, que
    /// es un detalle interno del núcleo. No lanza ninguna excepción y no modifica la instancia.
    /// </remarks>
    public IEnumerable<Exception> GetExceptions()
        => Metadata.Where(x => IsExceptionKey(x.Key))
                   .OrderBy(x => ExceptionKeyOrder(x.Key))
                   .Select(x => x.Value)
                   .OfType<Exception>();

    /// <summary>
    /// Indica si existe un metadato con la clave indicada.
    /// </summary>
    /// <param name="key">Clave identificativa del metadato.</param>
    /// <returns>
    /// <see langword="true"/> si la clave existe; en caso contrario, <see langword="false"/>.
    /// </returns>
    /// <remarks>No lanza ninguna excepción y no modifica la instancia.</remarks>
    public bool HasMetadata(string key) => key is not null && Metadata.ContainsKey(key);

    #endregion

    #region Incorporacion inmutable

    /// <summary>
    /// Devuelve unos detalles de error nuevos con el metadato indicado incorporado, sin modificar la
    /// instancia de partida.
    /// </summary>
    /// <param name="key">Clave identificativa del metadato.</param>
    /// <param name="value">Valor a asociar a la clave.</param>
    /// <returns>Instancia nueva con una copia independiente de los metadatos.</returns>
    /// <remarks>
    /// Si la clave ya existe, su valor se sobrescribe: a diferencia de
    /// <see cref="MlErrorDetailsActions.AddMetadataOrThrow{T}(MlErrorDetails, string, T)"/>, este
    /// método nunca lanza.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> es <see langword="null"/>.</exception>
    public MlErrorDetails WithMetadata(string key, object value)
    {
        ArgumentNullException.ThrowIfNull(key);

        var copy = CopyMetadata(Metadata);

        copy[key] = value;

        return new MlErrorDetails(Errors, copy);
    }

    /// <summary>
    /// Devuelve unos detalles de error nuevos con todos los metadatos indicados incorporados, sin
    /// modificar la instancia de partida.
    /// </summary>
    /// <param name="metadata">Parejas clave-valor a incorporar.</param>
    /// <returns>Instancia nueva con una copia independiente de los metadatos.</returns>
    /// <remarks>
    /// Las claves que ya existan se sobrescriben con el valor entrante. Para conservar los dos valores
    /// use <see cref="MlErrorDetailsActions.Merge(MlErrorDetails, MlErrorDetails)"/>, que aplica la
    /// política de colisión de la decisión D-15.
    /// </remarks>
    public MlErrorDetails WithMetadata(IEnumerable<KeyValuePair<string, object>>? metadata)
    {
        var copy = CopyMetadata(Metadata);

        if (metadata is not null)
        {
            foreach (var entry in metadata) copy[entry.Key] = entry.Value;
        }

        return new MlErrorDetails(Errors, copy);
    }

    #endregion

    #region Igualdad estructural

    /// <summary>
    /// Indica si estos detalles de error tienen el mismo contenido que los indicados.
    /// </summary>
    /// <param name="other">Detalles de error con los que comparar.</param>
    /// <returns>
    /// <see langword="true"/> si ambos tienen los mismos errores en el mismo orden y los mismos
    /// metadatos; en caso contrario, <see langword="false"/>.
    /// </returns>
    public bool Equals(MlErrorDetails? other)
    {
        if (other is null)                return false;
        if (ReferenceEquals(this, other)) return true;

        if ( ! Errors.SequenceEqual(other.Errors)) return false;

        if (Metadata.Count != other.Metadata.Count) return false;

        foreach (var entry in Metadata)
        {
            if ( ! other.Metadata.TryGetValue(entry.Key, out var otherValue)) return false;

            if ( ! Equals(entry.Value, otherValue)) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as MlErrorDetails);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = new();

        foreach (var error in Errors) hash.Add(error);

        hash.Add(Metadata.Count);

        foreach (var key in Metadata.Keys.OrderBy(x => x, MetadataKeyComparer)) hash.Add(key, MetadataKeyComparer);

        return hash.ToHashCode();
    }

    #endregion

}
