// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class IntBetween : ValueObject<int>
{
    protected IntBetween(int value, int minLength, int maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, minLength, maxLength));
    }

    public static string FormatErrorMessage(int value, int minLength, int maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(int value, int minLength, int maxLength) => value > minLength && value < maxLength;


    public static IntBetween CreateFromIntLength(int value, int minLength, int maxLength) => new IntBetween(value, minLength, maxLength);

    public static MlResult<IntBetween> FromIntLength(int value, int minLength, int maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                    .Map ( _ => new IntBetween(value, minLength, maxLength));

    public static implicit operator int (IntBetween valueObject) => valueObject.Value;
    public static implicit operator IntBetween ((int value, int minLength, int maxLength) tupleValues) => new IntBetween(tupleValues.value, 
                                                                                                                         tupleValues.minLength, 
                                                                                                                         tupleValues.maxLength);



}
