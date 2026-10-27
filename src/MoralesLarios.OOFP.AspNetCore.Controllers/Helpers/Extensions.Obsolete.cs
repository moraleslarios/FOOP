// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
using System.Globalization;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Helpers;
public static partial class Extensions
{
    [global::System.Obsolete("Renamed to ConvertToType. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object ConverterTo(this string value, Type property) => ConvertToType(value, property);
    [global::System.Obsolete("Renamed to BuildKeyParseErrorMessage. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetPkValuesErrorMessage(this Exception ex, string ids) => BuildKeyParseErrorMessage(ex, ids);
    [global::System.Obsolete("Renamed to ParseKeyValues. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object[] GetPkValues<TEntity>(this string ids, Func<TEntity, object[]> pkFields)
        where TEntity : class => ParseKeyValues<TEntity>(ids, pkFields);
    [global::System.Obsolete("Renamed to ParseKeyValues. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static object[] GetPkValues<TEntity>(string[] values, Func<TEntity, object[]> pkFields)
        where TEntity : class => ParseKeyValues<TEntity>(values, pkFields);
}