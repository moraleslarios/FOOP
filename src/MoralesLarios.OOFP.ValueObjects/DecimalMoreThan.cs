// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DecimalMoreThan</c>.
/// </summary>
public partial class DecimalMoreThan : ValueObject<decimal>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DecimalMoreThan"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    protected DecimalMoreThan(decimal value, decimal length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(decimal value, decimal length) => $"{value} must be more than {length}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(decimal value, decimal length) => value > length;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromDecimalLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Valor de tipo <c>DecimalMoreThan</c> obtenido por la operación.</returns>
    public static DecimalMoreThan CreateFromDecimalLength(decimal value, decimal length) => new DecimalMoreThan(value, length);
    /// <summary>
    /// Ejecuta <c>FromDecimalLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DecimalMoreThan> FromDecimalLength(decimal value, decimal length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DecimalMoreThan(value, length));

    /// <summary>
    /// Realiza la conversión implícita a <c>decimal</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>decimal</c> obtenido por la operación.</returns>
    public static implicit operator decimal(DecimalMoreThan valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DecimalMoreThan</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>DecimalMoreThan</c> obtenido por la operación.</returns>
    public static implicit operator DecimalMoreThan((decimal value, decimal length) tupleValues) => new DecimalMoreThan(tupleValues.value, tupleValues.length);
}

