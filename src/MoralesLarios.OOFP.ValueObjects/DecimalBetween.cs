// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DecimalBetween</c>.
/// </summary>
public partial class DecimalBetween : ValueObject<decimal>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DecimalBetween"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    protected DecimalBetween(decimal value, decimal minLength, decimal maxLength) : base(value)
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
    public static string FormatErrorMessage(decimal value, decimal minLength, decimal maxLength) => $"{value} must be between {minLength} and {maxLength}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(decimal value, decimal minLength, decimal maxLength) => value > minLength && value < maxLength;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromDecimalLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns>Valor de tipo <c>DecimalBetween</c> obtenido por la operación.</returns>
    public static DecimalBetween CreateFromDecimalLength(decimal value, decimal minLength, decimal maxLength) => new DecimalBetween(value, minLength, maxLength);

    /// <summary>
    /// Ejecuta <c>FromDecimalLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DecimalBetween> FromDecimalLength(decimal value, decimal minLength, decimal maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                   .Map ( _ => new DecimalBetween(value, minLength, maxLength));

    /// <summary>
    /// Realiza la conversión implícita a <c>decimal</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>decimal</c> obtenido por la operación.</returns>
    public static implicit operator decimal(DecimalBetween valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DecimalBetween</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>DecimalBetween</c> obtenido por la operación.</returns>
    public static implicit operator DecimalBetween((decimal value, decimal minLength, decimal maxLength) tupleValues)
        => new DecimalBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}

