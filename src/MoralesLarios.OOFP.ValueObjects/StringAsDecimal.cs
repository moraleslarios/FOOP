// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsDecimal : StringAsNumeric<decimal>
{
    protected StringAsDecimal(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = decimal.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid decimal";
    public static bool IsValid(string value) => decimal.TryParse(value, out _);

    public static StringAsDecimal CreateFromString(string value) => new StringAsDecimal(value);

    public static MlResult<StringAsDecimal> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsDecimal(value));

    public static implicit operator decimal(StringAsDecimal ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsDecimal(string value) => new StringAsDecimal(value);

    public static implicit operator string(StringAsDecimal valueObject) => valueObject.Value.ToString()!;
}

