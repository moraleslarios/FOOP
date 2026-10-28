// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>StringAsFloat</c>.
/// </summary>
public partial class StringAsFloat : StringAsNumeric<float>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="StringAsFloat"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected StringAsFloat(string value) : base(value)
    {
        if (!IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = float.Parse(value);
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid float";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value) => float.TryParse(value, out _);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsFloat</c> obtenido por la operación.</returns>
    public static StringAsFloat CreateFromString(string value) => new StringAsFloat(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<StringAsFloat> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsFloat(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>float</c>.
    /// </summary>
    /// <param name="ValueObjectFp">Valor de <c>ValueObjectFp</c>.</param>
    /// <returns>Valor de tipo <c>float</c> obtenido por la operación.</returns>
    public static implicit operator float(StringAsFloat ValueObjectFp) => ValueObjectFp.Value;

    /// <summary>
    /// Realiza la conversión explícita a <c>StringAsFloat</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsFloat</c> obtenido por la operación.</returns>
    public static explicit operator StringAsFloat(string value) => new StringAsFloat(value);

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(StringAsFloat valueObject) => valueObject.Value.ToString()!;
}

