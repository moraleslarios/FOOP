// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>StringAsLong</c>.
/// </summary>
public partial class StringAsLong : StringAsNumeric<long>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="StringAsLong"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected StringAsLong(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = long.Parse(value);
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid long";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value) => long.TryParse(value, out _);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsLong</c> obtenido por la operación.</returns>
    public static StringAsLong CreateFromString(string value) => new StringAsLong(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<StringAsLong> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsLong(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>long</c>.
    /// </summary>
    /// <param name="ValueObjectFp">Valor de <c>ValueObjectFp</c>.</param>
    /// <returns>Valor de tipo <c>long</c> obtenido por la operación.</returns>
    public static implicit operator long(StringAsLong ValueObjectFp) => ValueObjectFp.Value;

    /// <summary>
    /// Realiza la conversión explícita a <c>StringAsLong</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsLong</c> obtenido por la operación.</returns>
    public static explicit operator StringAsLong(string value) => new StringAsLong(value);

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(StringAsLong valueObject) => valueObject.Value.ToString()!;
}

