

namespace MoralesLarios.OOFP.ValueObjects;

public class NotEmptyString : ValueObject<string>
{
    protected NotEmptyString(string value) : base(value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value));
    }

    public static string BuildErrorMessage(string value) => $"{nameof(value)} cannot be null or empty";
    public static bool IsValid(string value) => ! string.IsNullOrWhiteSpace(value);

    public static NotEmptyString FromString(string value) => new NotEmptyString(value);

    public static MlResult<NotEmptyString> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value))
                    .Map(_ => new NotEmptyString(value));

    public static implicit operator string        (NotEmptyString valueObject) => valueObject.Value;
    public static implicit operator NotEmptyString(string         value      ) => new NotEmptyString(value);
}
