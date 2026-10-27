// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsSByte : StringAsNumeric<sbyte>
{
    protected StringAsSByte(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = sbyte.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid sbyte";
    public static bool IsValid(string value) => sbyte.TryParse(value, out _);

    public static StringAsSByte CreateFromString(string value) => new StringAsSByte(value);

    public static MlResult<StringAsSByte> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsSByte(value));

    public static implicit operator sbyte(StringAsSByte ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsSByte(string value) => new StringAsSByte(value);

    public static implicit operator string(StringAsSByte valueObject) => valueObject.Value.ToString()!;
}

