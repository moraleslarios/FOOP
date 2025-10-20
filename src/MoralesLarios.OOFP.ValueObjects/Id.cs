namespace MoralesLarios.OOFP.ValueObjects;

public class Id : DoubleMoreThan
{
	private static double Limit => 0;

	public Id(double value) : base(value, Limit)
	{
		if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), BuildErrorMessage(value));
	}
	public static string BuildErrorMessage(double value) => $"Id can be mayor 0.";
	public static bool IsValid(double value) => Id.IsValid(value, Limit);

	public static Id Fromdouble(double value) => new Id(value);
	public static MlResult<Id> Bydouble(double value, MlErrorsDetails errorsDetails = null!)
		=> MlResult.Empty()
					.Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
					.Map ( _ => new Id(value));

	public static implicit operator double(Id     valueObject) => valueObject.Value;
	public static implicit operator		Id(double value		 ) => new Id(value);


}