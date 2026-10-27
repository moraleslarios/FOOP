// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringBetweenLength : ValueObject<NotEmptyString>
{
    protected StringBetweenLength(NotEmptyString value, int minLength, int maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, minLength, maxLength));
    }


    public static string BuildErrorMessage(string value, int minLength, int maxLength) => $"{value} must be between {minLength} and {maxLength}";
    public static bool IsValid(string value, int minLength, int maxLength) => value.Length >= minLength && value.Length <= maxLength;

    public static StringBetweenLength FromStringLength(string value, int minLength, int maxLength) => new StringBetweenLength(value, minLength, maxLength);

    public static MlResult<StringBetweenLength> ByStringLength(string value, int minLength, int maxLength, MlErrorsDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? BuildErrorMessage(value, minLength, maxLength)))
                            .Map ( _ => new StringBetweenLength(value, minLength, maxLength));

    public static implicit operator string         (StringBetweenLength valueObject) => valueObject.Value;
    public static implicit operator StringBetweenLength((string value, int minLenght, int maxLenght) tupleValues) => new StringBetweenLength(tupleValues.value, 
                                                                                                                                             tupleValues.minLenght, 
                                                                                                                                             tupleValues.maxLenght);
}

