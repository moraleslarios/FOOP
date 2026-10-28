// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DoubleBetween</c>.
/// </summary>
public partial class DoubleBetween : ValueObject<double>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DoubleBetween"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    protected DoubleBetween(double value, double minLength, double maxLength) : base(value)
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
    public static string FormatErrorMessage(double value, double minLength, double maxLength) => $"{value} must be between {minLength} and {maxLength}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(double value, double minLength, double maxLength) => value > minLength && value < maxLength;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns>Valor de tipo <c>DoubleBetween</c> obtenido por la operación.</returns>
    public static DoubleBetween CreateFromDoubleLength(double value, double minLength, double maxLength) => new DoubleBetween(value, minLength, maxLength);

    /// <summary>
    /// Ejecuta <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DoubleBetween> FromDoubleLength(double value, double minLength, double maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind(_ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                   .Map(_ => new DoubleBetween(value, minLength, maxLength));

    /// <summary>
    /// Realiza la conversión implícita a <c>double</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>double</c> obtenido por la operación.</returns>
    public static implicit operator double(DoubleBetween valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DoubleBetween</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>DoubleBetween</c> obtenido por la operación.</returns>
    public static implicit operator DoubleBetween((double value, double minLength, double maxLength) tupleValues)
        => new DoubleBetween(tupleValues.value, tupleValues.minLength, tupleValues.maxLength);
}

