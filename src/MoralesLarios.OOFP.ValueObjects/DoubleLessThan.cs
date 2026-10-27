// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DoubleLessThan : ValueObject<double>
{
    protected DoubleLessThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    public static string FormatErrorMessage(double value, double length) => $"{value} must be less than {length}";
    public static bool IsValid(double value, double length) => value < length;

    public static DoubleLessThan CreateFromDoubleLength(double value, double length) => new DoubleLessThan(value, length);
    public static MlResult<DoubleLessThan> FromDoubleLength(double value, double length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DoubleLessThan(value, length));

    public static implicit operator double(DoubleLessThan valueObject) => valueObject.Value;
    public static implicit operator DoubleLessThan((double value, double length) tupleValues) => new DoubleLessThan(tupleValues.value, tupleValues.length);
}

