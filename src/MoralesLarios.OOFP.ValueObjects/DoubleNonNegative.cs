// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DoubleNonNegative : DoubleMoreThan
{
    private const double MinAllowedValue = 0;

    protected DoubleNonNegative(double value) : base(value, MinAllowedValue - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    public static string FormatErrorMessage(double value) => $"{value} must be More than {MinAllowedValue}";
    public static bool IsValid(double value) => value >= MinAllowedValue;


    public static DoubleNonNegative CreateFromDouble(double value) => new DoubleNonNegative(value);
    public static MlResult<DoubleNonNegative> FromDouble(double value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new DoubleNonNegative(value));

    public static implicit operator double            (DoubleNonNegative valueObject) => valueObject.Value;
    public static implicit operator DoubleNonNegative (double            value      ) => new DoubleNonNegative(value);

}
