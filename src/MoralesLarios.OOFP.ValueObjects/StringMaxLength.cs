// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>StringMaxLength</c>.
/// </summary>
public partial class StringMaxLength : ValueObject<NotEmptyString>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="StringMaxLength"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    protected StringMaxLength(NotEmptyString value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }


    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value, int length) => $"{value} cannot be longer than {length} characters";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value, int length) => value.Length < length;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Valor de tipo <c>StringMaxLength</c> obtenido por la operación.</returns>
    public static StringMaxLength CreateFromLength(string value, int length) => new StringMaxLength(value, length);

    /// <summary>
    /// Ejecuta <c>FromLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<StringMaxLength> FromLength(string value, int length, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                            .Map ( _ => new StringMaxLength(value, length));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string         (StringMaxLength valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>StringMaxLength</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>StringMaxLength</c> obtenido por la operación.</returns>
    public static implicit operator StringMaxLength((string value, int length) tupleValues) => new StringMaxLength(tupleValues.value, tupleValues.length);
}

