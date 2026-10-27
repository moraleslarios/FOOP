// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringLengthBetween : ValueObject<NotEmptyString>
{
    protected StringLengthBetween(NotEmptyString value, int minLength, int maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, minLength, maxLength));
    }


    public static string FormatErrorMessage(string value, int minLength, int maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(string value, int minLength, int maxLength) => value.Length >= minLength && value.Length <= maxLength;

    public static StringLengthBetween CreateFromLength(string value, int minLength, int maxLength) => new StringLengthBetween(value, minLength, maxLength);

    public static MlResult<StringLengthBetween> FromLength(string value, int minLength, int maxLength, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                            .Map ( _ => new StringLengthBetween(value, minLength, maxLength));

    public static implicit operator string         (StringLengthBetween valueObject) => valueObject.Value;
    public static implicit operator StringLengthBetween((string value, int minLenght, int maxLenght) tupleValues) => new StringLengthBetween(tupleValues.value, 
                                                                                                                                             tupleValues.minLenght, 
                                                                                                                                             tupleValues.maxLenght);
}

