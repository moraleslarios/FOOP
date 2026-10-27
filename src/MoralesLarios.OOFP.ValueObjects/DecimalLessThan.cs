// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DecimalLessThan : ValueObject<decimal>
{
    protected DecimalLessThan(decimal value, decimal length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    public static string FormatErrorMessage(decimal value, decimal length) => $"{value} must be less than {length}";
    public static bool IsValid(decimal value, decimal length) => value < length;

    public static DecimalLessThan CreateFromDecimalLength(decimal value, decimal length) => new DecimalLessThan(value, length);
    public static MlResult<DecimalLessThan> FromDecimalLength(decimal value, decimal length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DecimalLessThan(value, length));

    public static implicit operator decimal(DecimalLessThan valueObject) => valueObject.Value;
    public static implicit operator DecimalLessThan((decimal value, decimal length) tupleValues) => new DecimalLessThan(tupleValues.value, tupleValues.length);
}

