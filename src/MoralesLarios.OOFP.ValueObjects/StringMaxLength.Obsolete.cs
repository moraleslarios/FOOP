// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class StringMaxLength
{
    [global::System.Obsolete("Renamed to FromStringLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static StringMaxLength FromStringLenght(string value, int lenght) => CreateFromLength(value, lenght);
    [global::System.Obsolete("Renamed to FromLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<StringMaxLength> ByStringLength(string value, int length, MlErrorDetails errorsDetails = null !) => FromLength(value, length, errorsDetails);
}