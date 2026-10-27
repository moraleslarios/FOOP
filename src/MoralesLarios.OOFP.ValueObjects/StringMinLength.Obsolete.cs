// Puentes [Obsolete] de la v2.0.0: nombres antiguos que reenvian al nombre nuevo. Se eliminan en la v3.0.0.
namespace MoralesLarios.OOFP.ValueObjects;
public partial class StringMinLength
{
    [global::System.Obsolete("Renamed to FromStringLength. Will be removed in 3.0.0.", false), global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    public static StringMinLength FromStringLenght(string value, int lenght) => FromStringLength(value, lenght);
}
