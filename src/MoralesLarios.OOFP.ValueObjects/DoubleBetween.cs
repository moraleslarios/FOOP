// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class DoubleBetween : ValueObject<double>
{
    protected DoubleBetween(double value, double minLength, double maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, minLength, maxLength));
    }

    public static string FormatErrorMessage(double value, double minLength, double maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(double value, double minLength, double maxLength) => value > minLength && value < maxLength;

    public static DoubleBetween CreateFromDoubleLength(double value, double minLength, double maxLength) => new DoubleBetween(value, minLength, maxLength);

    public static MlResult<DoubleBetween> FromDoubleLength(double value, double minLength, double maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind(_ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                   .Map(_ => new DoubleBetween(value, minLength, maxLength));

    public static implicit operator double(DoubleBetween valueObject) => valueObject.Value;
    public static implicit operator DoubleBetween((double value, double minLength, double maxLength) tupleValues)
        => new DoubleBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}

