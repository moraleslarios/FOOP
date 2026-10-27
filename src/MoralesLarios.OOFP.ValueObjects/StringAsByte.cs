// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsByte : StringAsNumeric<byte>
{
    protected StringAsByte(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = byte.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid byte";
    public static bool IsValid(string value) => byte.TryParse(value, out _);

    public static StringAsByte CreateFromString(string value) => new StringAsByte(value);

    public static MlResult<StringAsByte> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsByte(value));

    public static implicit operator byte(StringAsByte ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsByte(string value) => new StringAsByte(value);

    public static implicit operator string(StringAsByte valueObject) => valueObject.Value.ToString()!;
}

