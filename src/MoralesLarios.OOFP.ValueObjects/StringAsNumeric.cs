// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>StringAsNumeric</c>.
/// </summary>
/// <typeparam name="TValue">Tipo del valor.</typeparam>
public partial class StringAsNumeric<TValue> : ValueObject
    where TValue : struct
{

    /// <summary>
    /// Valor de <c>Value</c>.
    /// </summary>
    protected TValue Value;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="StringAsNumeric{TValue}"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected StringAsNumeric(string value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));

        Value = (TValue)Convert.ChangeType(value, typeof(TValue))!;
    }



    /// <inheritdoc/>
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }


    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{nameof(value)} should be a valid number";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value) => Convert.ChangeType(value, typeof(TValue)) != null;

    /// <summary>
    /// Crea la instancia solicitada de from string.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsNumeric&lt;TValue&gt;</c> obtenido por la operación.</returns>
    public static StringAsNumeric<TValue> CreateFromString(string value) => new StringAsNumeric<TValue>(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<StringAsNumeric<TValue>> FromString(string value)
        => MlResult.Empty()
                    .TryBind(_ => EnsureFp.That(value, IsValid(value), FormatErrorMessage(value)),
                                            $"{FormatErrorMessage(value)}")
                    .Map(_ => new StringAsNumeric<TValue>(value));










    /// <summary>
    /// Realiza la conversión implícita a <c>TValue</c>.
    /// </summary>
    /// <param name="ValueObjectFp">Valor de <c>ValueObjectFp</c>.</param>
    /// <returns>Valor de tipo <c>TValue</c> obtenido por la operación.</returns>
    public static implicit operator TValue(StringAsNumeric<TValue> ValueObjectFp) => ValueObjectFp.Value;

    /// <summary>
    /// Realiza la conversión explícita a <c>StringAsNumeric&lt;TValue&gt;</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>StringAsNumeric&lt;TValue&gt;</c> obtenido por la operación.</returns>
    public static explicit operator StringAsNumeric<TValue>(string value) => new StringAsNumeric<TValue>(value);

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(StringAsNumeric<TValue> valueObject) => valueObject.Value.ToString()!;



    /// <inheritdoc/>
    public override string? ToString() => Value.ToString();


}

