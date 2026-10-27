// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsLong : StringAsNumeric<long>
{
    protected StringAsLong(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = long.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid long";
    public static bool IsValid(string value) => long.TryParse(value, out _);

    public static StringAsLong CreateFromString(string value) => new StringAsLong(value);

    public static MlResult<StringAsLong> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsLong(value));

    public static implicit operator long(StringAsLong ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsLong(string value) => new StringAsLong(value);

    public static implicit operator string(StringAsLong valueObject) => valueObject.Value.ToString()!;
}

