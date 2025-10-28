namespace MoralesLarios.OOFP.ValueObjects.IO;

public class ExistDirectory : NotEmptyString
{
    protected ExistDirectory(string directoryStr) : base(directoryStr)
    {
        if (!IsValid(directoryStr)) throw new ArgumentNullException(nameof(directoryStr), BuildErrorMessage(directoryStr));
    }


    public new static string BuildErrorMessage(string directoryStr) => $"{directoryStr} not exists";
    public new static bool IsValid(string directoryStr) => Directory.Exists(directoryStr);

    public new static ExistDirectory FromString(string directoryStr) => new ExistDirectory(directoryStr);

    public new static MlResult<ExistDirectory> ByString(string directoryStr, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(directoryStr)
                            .Bind(_ => EnsureFp.That(directoryStr, IsValid(directoryStr), errorsDetails ?? BuildErrorMessage(directoryStr))
                            .Map(_ => new ExistDirectory(directoryStr)));

    public static implicit operator string        (ExistDirectory directoryStrObject) => directoryStrObject.Value;
    public static implicit operator ExistDirectory(string         directoryStr      ) => new ExistDirectory(directoryStr);
}