// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DoubleMoreThan : ValueObject<double>
{
    protected DoubleMoreThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    public static string FormatErrorMessage(double value, double length) => $"{value} must be more than {length}";
    public static bool IsValid(double value, double length) => value > length;

    public static DoubleMoreThan CreateFromDoubleLength(double value, double length) => new DoubleMoreThan(value, length);
    public static MlResult<DoubleMoreThan> FromDoubleLength(double value, double length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DoubleMoreThan(value, length));

    public static implicit operator double(DoubleMoreThan valueObject) => valueObject.Value;
    public static implicit operator DoubleMoreThan((double value, double length) tupleValues) => new DoubleMoreThan(tupleValues.value, tupleValues.length);
}

