// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class StringAsNumeric<TValue> : ValueObject
    where TValue : struct
{

    protected TValue Value;

    protected StringAsNumeric(string value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = (TValue)Convert.ChangeType(value, typeof(TValue))!;
    }



    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }


    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid number";
    public static bool IsValid(string value) => Convert.ChangeType(value, typeof(TValue)) != null;

    public static StringAsNumeric<TValue> CreateFromString(string value) => new StringAsNumeric<TValue>(value);

    public static MlResult<StringAsNumeric<TValue>> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsNumeric<TValue>(value));










    public static implicit operator TValue(StringAsNumeric<TValue> ValueObjectFp) => ValueObjectFp.Value;

    public static explicit operator StringAsNumeric<TValue>(string value) => new StringAsNumeric<TValue>(value);

    public static implicit operator string(StringAsNumeric<TValue> valueObject) => valueObject.Value.ToString()!;



    public override string? ToString() => Value.ToString();


}

