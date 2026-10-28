// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Globalization;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Helpers;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Ejecuta <c>ConverterTo</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="property">Valor de <c>property</c>.</param>
    /// <returns>Valor de tipo <c>object</c> obtenido por la operación.</returns>
    [global::System.Obsolete("Renamed to ConvertToType. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object ConverterTo(this string value, Type property) => ConvertToType(value, property);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>PkValuesErrorMessage</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <param name="ex">Excepción capturada.</param>
    /// <param name="ids">Identificadores de las entidades.</param>
    /// <returns>Representación textual obtenida.</returns>
    [global::System.Obsolete("Renamed to BuildKeyParseErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetPkValuesErrorMessage(this Exception ex, string ids) => BuildKeyParseErrorMessage(ex, ids);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>PkValues</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
    /// <param name="ids">Identificadores de las entidades.</param>
    /// <param name="pkFields">Función a aplicar.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    [global::System.Obsolete("Renamed to ParseKeyValues. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object[] GetPkValues<TEntity>(this string ids, Func<TEntity, object[]> pkFields)
        where TEntity : class => ParseKeyValues<TEntity>(ids, pkFields);
    /// <summary>
    /// Obtiene el elemento solicitado de <c>PkValues</c>.
    /// </summary>
    /// <remarks>
    /// Miembro obsoleto conservado por compatibilidad. Use la alternativa indicada en el atributo
    /// <c>Obsolete</c>.
    /// </remarks>
    /// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
    /// <param name="values">Valores sobre los que se opera.</param>
    /// <param name="pkFields">Función a aplicar.</param>
    /// <returns>Colección con los elementos obtenidos.</returns>
    [global::System.Obsolete("Renamed to ParseKeyValues. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object[] GetPkValues<TEntity>(string[] values, Func<TEntity, object[]> pkFields)
        where TEntity : class => ParseKeyValues<TEntity>(values, pkFields);
}