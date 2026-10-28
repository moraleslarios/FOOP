// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>EnumRangeValueObject</c>.
/// </summary>
/// <typeparam name="TEnum">Tipo de la enumeración.</typeparam>
public partial class EnumRangeValueObject<TEnum> : ValueObject<string>
    where TEnum : struct, Enum
{
    private EnumRangeValueObject(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentException(FormatErrorMessage(value), nameof(value));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{value} is not a valid value for enum {typeof(TEnum).Name}";

    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value)
        => !string.IsNullOrWhiteSpace(value)
           && Enum.GetNames<TEnum>().Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Crea la instancia solicitada de <c>FromEnum</c>.
    /// </summary>
    /// <param name="enumValue">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>EnumRangeValueObject&lt;TEnum&gt;</c> obtenido por la operación.</returns>
    public static EnumRangeValueObject<TEnum> CreateFromEnum(TEnum enumValue) => CreateFromString(enumValue.ToString());

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>EnumRangeValueObject&lt;TEnum&gt;</c> obtenido por la operación.</returns>
    public static EnumRangeValueObject<TEnum> CreateFromString(string value) => new(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<EnumRangeValueObject<TEnum>> FromString(string value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
            .Map(_ => new EnumRangeValueObject<TEnum>(value));

    /// <summary>
    /// Obtiene el elemento solicitado de <c>EnumValue</c>.
    /// </summary>
    /// <returns>Valor de tipo <c>TEnum</c> obtenido por la operación.</returns>
    public TEnum GetEnumValue()
    {
        var enumName = Enum.GetNames<TEnum>()
            .FirstOrDefault(x => string.Equals(x, Value, StringComparison.OrdinalIgnoreCase));

        if (enumName is null) throw new ArgumentException(FormatErrorMessage(Value), nameof(Value));

        return Enum.Parse<TEnum>(enumName, ignoreCase: true);
    }

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(EnumRangeValueObject<TEnum> valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>TEnum</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>TEnum</c> obtenido por la operación.</returns>
    public static implicit operator TEnum(EnumRangeValueObject<TEnum> valueObject) => valueObject.GetEnumValue();
    /// <summary>
    /// Realiza la conversión implícita a <c>EnumRangeValueObject&lt;TEnum&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>EnumRangeValueObject&lt;TEnum&gt;</c> obtenido por la operación.</returns>
    public static implicit operator EnumRangeValueObject<TEnum>(string value) => CreateFromString(value);
    /// <summary>
    /// Realiza la conversión implícita a <c>EnumRangeValueObject&lt;TEnum&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>EnumRangeValueObject&lt;TEnum&gt;</c> obtenido por la operación.</returns>
    public static implicit operator EnumRangeValueObject<TEnum>(TEnum value) => CreateFromEnum(value);
}
