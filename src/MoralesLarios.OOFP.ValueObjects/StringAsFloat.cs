// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsFloat : StringAsNumeric<float>
{
    protected StringAsFloat(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = float.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid float";
    public static bool IsValid(string value) => float.TryParse(value, out _);

    public static StringAsFloat CreateFromString(string value) => new StringAsFloat(value);

    public static MlResult<StringAsFloat> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsFloat(value));

    public static implicit operator float(StringAsFloat ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsFloat(string value) => new StringAsFloat(value);

    public static implicit operator string(StringAsFloat valueObject) => valueObject.Value.ToString()!;
}

