// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DoubleLessThan</c>.
/// </summary>
public partial class DoubleLessThan : ValueObject<double>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DoubleLessThan"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    protected DoubleLessThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(double value, double length) => $"{value} must be less than {length}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(double value, double length) => value < length;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Valor de tipo <c>DoubleLessThan</c> obtenido por la operación.</returns>
    public static DoubleLessThan CreateFromDoubleLength(double value, double length) => new DoubleLessThan(value, length);
    /// <summary>
    /// Ejecuta <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DoubleLessThan> FromDoubleLength(double value, double length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DoubleLessThan(value, length));

    /// <summary>
    /// Realiza la conversión implícita a <c>double</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>double</c> obtenido por la operación.</returns>
    public static implicit operator double(DoubleLessThan valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DoubleLessThan</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>DoubleLessThan</c> obtenido por la operación.</returns>
    public static implicit operator DoubleLessThan((double value, double length) tupleValues) => new DoubleLessThan(tupleValues.value, tupleValues.length);
}

