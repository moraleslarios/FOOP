namespace MoralesLarios.OOFP.ValueObjects.IO;

public class ExistsFile : NotEmptyString
{
    protected ExistsFile(string pathStr) : base(pathStr)
    {
        if ( ! IsValid(pathStr)) throw new ArgumentNullException(nameof(pathStr), BuildErrorMessage(pathStr));
    } 


    public new static string BuildErrorMessage(string pathStr) => $"{pathStr} not exists";
    public new static bool IsValid(string pathStr) => File.Exists(pathStr);

    public new static ExistsFile FromString(string pathStr) => new ExistsFile(pathStr);

    public new static MlResult<ExistsFile> ByString(string pathStr, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(pathStr)
                            .Bind( _ => EnsureFp.That(pathStr, IsValid(pathStr), errorsDetails ?? BuildErrorMessage(pathStr))
                            .Map ( _ => new ExistsFile(pathStr)));

    public static implicit operator string    (ExistsFile pathStrObject) => pathStrObject.Value;
    public static implicit operator ExistsFile(string     pathStr      ) => new ExistsFile(pathStr);
}
