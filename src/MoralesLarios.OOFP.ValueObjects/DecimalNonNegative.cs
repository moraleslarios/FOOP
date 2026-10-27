// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DecimalNonNegative : DecimalMoreThan
{
    private const decimal MinAllowedValue = 0m;

    protected DecimalNonNegative(decimal value) : base(value, MinAllowedValue - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    public static string FormatErrorMessage(decimal value) => $"{value} must be More than {MinAllowedValue}";
    public static bool IsValid(decimal value) => value >= MinAllowedValue;


    public static DecimalNonNegative CreateFromDecimal(decimal value) => new DecimalNonNegative(value);
    public static MlResult<DecimalNonNegative> FromDecimal(decimal value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new DecimalNonNegative(value));

    public static implicit operator decimal            (DecimalNonNegative valueObject) => valueObject.Value;
    public static implicit operator DecimalNonNegative (decimal            value      ) => new DecimalNonNegative(value);

}

