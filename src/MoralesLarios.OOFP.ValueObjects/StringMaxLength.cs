// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringMaxLength : ValueObject<NotEmptyString>
{
    protected StringMaxLength(NotEmptyString value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), BuildErrorMessage(value, length));
    }


    public static string BuildErrorMessage(string value, int length) => $"{value} cannot be longer than {length} characters";
    public static bool IsValid(string value, int length) => value.Length < length;

    public static StringMaxLength FromStringLength(string value, int length) => new StringMaxLength(value, length);

    public static MlResult<StringMaxLength> ByStringLength(string value, int length, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.ByString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? BuildErrorMessage(value, length)))
                            .Map ( _ => new StringMaxLength(value, length));

    public static implicit operator string         (StringMaxLength valueObject) => valueObject.Value;
    public static implicit operator StringMaxLength((string value, int length) tupleValues) => new StringMaxLength(tupleValues.value, tupleValues.length);
}

