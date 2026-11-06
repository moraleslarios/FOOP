// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.EntityFrameworkCore.Helpers;
/// <summary>
/// Compatibility extensions retained for obsolete Entity Framework Core names.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Formats the supplied primary-key values.
    /// </summary>
    /// <remarks>
    /// This obsolete member is retained for compatibility. Use the replacement named by the
    /// <see cref="ObsoleteAttribute"/> message.
    /// </remarks>
    /// <param name="source">The primary-key values to format.</param>
    /// <returns>A string representation of the values.</returns>
    [global::System.Obsolete("Renamed to FormatCompositeKey. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static string GetPkValues(this object[] source) => FormatCompositeKey(source);
}
