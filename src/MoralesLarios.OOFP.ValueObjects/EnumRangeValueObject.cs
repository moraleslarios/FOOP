// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

public partial class EnumRangeValueObject<TEnum> : ValueObject<string>
    where TEnum : struct, Enum
{
    private EnumRangeValueObject(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentException(FormatErrorMessage(value), nameof(value));
    }

    public static string FormatErrorMessage(string value) => $"{value} is not a valid value for enum {typeof(TEnum).Name}";

    public static bool IsValid(string value)
        => !string.IsNullOrWhiteSpace(value)
           && Enum.GetNames<TEnum>().Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

    public static EnumRangeValueObject<TEnum> CreateFromEnum(TEnum enumValue) => CreateFromString(enumValue.ToString());

    public static EnumRangeValueObject<TEnum> CreateFromString(string value) => new(value);

    public static MlResult<EnumRangeValueObject<TEnum>> FromString(string value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
            .Map(_ => new EnumRangeValueObject<TEnum>(value));

    public TEnum GetEnumValue()
    {
        var enumName = Enum.GetNames<TEnum>()
            .FirstOrDefault(x => string.Equals(x, Value, StringComparison.OrdinalIgnoreCase));

        if (enumName is null) throw new ArgumentException(FormatErrorMessage(Value), nameof(Value));

        return Enum.Parse<TEnum>(enumName, ignoreCase: true);
    }

    public static implicit operator string(EnumRangeValueObject<TEnum> valueObject) => valueObject.Value;
    public static implicit operator TEnum(EnumRangeValueObject<TEnum> valueObject) => valueObject.GetEnumValue();
    public static implicit operator EnumRangeValueObject<TEnum>(string value) => CreateFromString(value);
    public static implicit operator EnumRangeValueObject<TEnum>(TEnum value) => CreateFromEnum(value);
}
