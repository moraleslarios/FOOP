// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DecimalBetween : ValueObject<decimal>
{
    protected DecimalBetween(decimal value, decimal minLength, decimal maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, minLength, maxLength));
    }

    public static string FormatErrorMessage(decimal value, decimal minLength, decimal maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(decimal value, decimal minLength, decimal maxLength) => value > minLength && value < maxLength;

    public static DecimalBetween CreateFromDecimalLength(decimal value, decimal minLength, decimal maxLength) => new DecimalBetween(value, minLength, maxLength);

    public static MlResult<DecimalBetween> FromDecimalLength(decimal value, decimal minLength, decimal maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                   .Map ( _ => new DecimalBetween(value, minLength, maxLength));

    public static implicit operator decimal(DecimalBetween valueObject) => valueObject.Value;
    public static implicit operator DecimalBetween((decimal value, decimal minLength, decimal maxLength) tupleValues)
        => new DecimalBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}

