// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class StringLengthBetween
{
    [global::System.Obsolete("Renamed to FromStringLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static StringLengthBetween FromStringLenght(string value, int minLenght, int maxLenght) => CreateFromLength(value, minLenght, maxLenght);
    [global::System.Obsolete("Renamed to FromLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static MlResult<StringLengthBetween> ByStringLength(string value, int minLength, int maxLength, MlErrorDetails errorsDetails = null !) => FromLength(value, minLength, maxLength, errorsDetails);
}