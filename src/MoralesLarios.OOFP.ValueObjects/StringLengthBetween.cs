// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>StringLengthBetween</c>.
/// </summary>
public partial class StringLengthBetween : ValueObject<NotEmptyString>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="StringLengthBetween"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    protected StringLengthBetween(NotEmptyString value, int minLength, int maxLength) : base(value)
    {
        if ( ! IsValid(value, minLength, maxLength)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, minLength, maxLength));
    }


    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value, int minLength, int maxLength) => $"{value} must be between {minLength} and {maxLength}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value, int minLength, int maxLength) => value.Length >= minLength && value.Length <= maxLength;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns>Valor de tipo <c>StringLengthBetween</c> obtenido por la operación.</returns>
    public static StringLengthBetween CreateFromLength(string value, int minLength, int maxLength) => new StringLengthBetween(value, minLength, maxLength);

    /// <summary>
    /// Ejecuta <c>FromLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<StringLengthBetween> FromLength(string value, int minLength, int maxLength, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                            .Map ( _ => new StringLengthBetween(value, minLength, maxLength));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string         (StringLengthBetween valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>StringLengthBetween</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>StringLengthBetween</c> obtenido por la operación.</returns>
    public static implicit operator StringLengthBetween((string value, int minLength, int maxLength) tupleValues) => new StringLengthBetween(tupleValues.value, 
                                                                                                                                             tupleValues.minLength, 
                                                                                                                                             tupleValues.maxLength);
}

