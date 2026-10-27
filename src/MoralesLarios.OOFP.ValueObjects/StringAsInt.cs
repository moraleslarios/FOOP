// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;



public partial class StringAsInt : StringAsNumeric<int>
{

    protected StringAsInt(string value) : base(value)
    {
        if( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = int.Parse(value);
    }

    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid int";
    public static bool IsValid(string value) => int.TryParse(value, out _);

    public static StringAsInt CreateFromString(string value) => new StringAsInt(value);

    public static MlResult<StringAsInt> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsInt(value));


    public static implicit operator int(StringAsInt ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsInt(string value) => new StringAsInt(value);

    public static implicit operator string(StringAsInt valueObject) => valueObject.Value.ToString()!;

}

