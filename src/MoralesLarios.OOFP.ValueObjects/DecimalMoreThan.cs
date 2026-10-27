// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DecimalMoreThan : ValueObject<decimal>
{
    protected DecimalMoreThan(decimal value, decimal length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    public static string FormatErrorMessage(decimal value, decimal length) => $"{value} must be more than {length}";
    public static bool IsValid(decimal value, decimal length) => value > length;

    public static DecimalMoreThan CreateFromDecimalLength(decimal value, decimal length) => new DecimalMoreThan(value, length);
    public static MlResult<DecimalMoreThan> FromDecimalLength(decimal value, decimal length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DecimalMoreThan(value, length));

    public static implicit operator decimal(DecimalMoreThan valueObject) => valueObject.Value;
    public static implicit operator DecimalMoreThan((decimal value, decimal length) tupleValues) => new DecimalMoreThan(tupleValues.value, tupleValues.length);
}

