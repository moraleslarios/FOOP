// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsULong : StringAsNumeric<ulong>
{
    protected StringAsULong(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = ulong.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid ulong";
    public static bool IsValid(string value) => ulong.TryParse(value, out _);

    public static StringAsULong CreateFromString(string value) => new StringAsULong(value);

    public static MlResult<StringAsULong> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsULong(value));

    public static implicit operator ulong(StringAsULong ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsULong(string value) => new StringAsULong(value);

    public static implicit operator string(StringAsULong valueObject) => valueObject.Value.ToString()!;
}

