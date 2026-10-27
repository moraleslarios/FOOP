// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringMinLength : ValueObject<NotEmptyString>
{
    protected StringMinLength(NotEmptyString value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }


    public static string FormatErrorMessage(string value, int length) => $"{value} cannot be less than {length} characters";
    public static bool IsValid(string value, int length) => value.Length >= length;

    public static StringMinLength CreateFromLength(string value, int length) => new StringMinLength(value, length);

    public static MlResult<StringMinLength> FromLength(string value, int length, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                            .Map ( _ => new StringMinLength(value, length));

    public static implicit operator string         (StringMinLength valueObject) => valueObject.Value;
    public static implicit operator StringMinLength((string value, int length) tupleValues) => new StringMinLength(tupleValues.value, tupleValues.length);
}

