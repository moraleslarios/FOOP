// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public class RangeEnumValueObject<TEnum> : ValueObject<string>
    where TEnum : struct, Enum
{
    private RangeEnumValueObject(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentException(BuildErrorMessage(value), nameof(value));
    }

    public static string BuildErrorMessage(string value) => $"{value} is not a valid value for enum {typeof(TEnum).Name}";

    public static bool IsValid(string value)
        => !string.IsNullOrWhiteSpace(value)
           && Enum.GetNames<TEnum>().Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

    public static RangeEnumValueObject<TEnum> FromEnum(TEnum enumValue) => FromString(enumValue.ToString());

    public static RangeEnumValueObject<TEnum> FromString(string value) => new(value);

    public static MlResult<RangeEnumValueObject<TEnum>> ByString(string value, MlErrorsDetails errorsDetails = null!)
        => MlResult.Empty()
            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? BuildErrorMessage(value)))
            .Map(_ => new RangeEnumValueObject<TEnum>(value));

    public TEnum GetEnumValue()
    {
        var enumName = Enum.GetNames<TEnum>()
            .FirstOrDefault(x => string.Equals(x, Value, StringComparison.OrdinalIgnoreCase));

        if (enumName is null) throw new ArgumentException(BuildErrorMessage(Value), nameof(Value));

        return Enum.Parse<TEnum>(enumName, ignoreCase: true);
    }

    public static implicit operator string(RangeEnumValueObject<TEnum> valueObject) => valueObject.Value;
    public static implicit operator TEnum(RangeEnumValueObject<TEnum> valueObject) => valueObject.GetEnumValue();
    public static implicit operator RangeEnumValueObject<TEnum>(string value) => FromString(value);
    public static implicit operator RangeEnumValueObject<TEnum>(TEnum value) => FromEnum(value);
}
