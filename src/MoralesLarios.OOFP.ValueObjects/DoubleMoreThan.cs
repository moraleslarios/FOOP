// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DoubleMoreThan</c>.
/// </summary>
public partial class DoubleMoreThan : ValueObject<double>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DoubleMoreThan"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    protected DoubleMoreThan(double value, double length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(double value, double length) => $"{value} must be more than {length}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(double value, double length) => value > length;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Valor de tipo <c>DoubleMoreThan</c> obtenido por la operación.</returns>
    public static DoubleMoreThan CreateFromDoubleLength(double value, double length) => new DoubleMoreThan(value, length);
    /// <summary>
    /// Ejecuta <c>FromDoubleLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DoubleMoreThan> FromDoubleLength(double value, double length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                   .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                   .Map ( _ => new DoubleMoreThan(value, length));

    /// <summary>
    /// Realiza la conversión implícita a <c>double</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>double</c> obtenido por la operación.</returns>
    public static implicit operator double(DoubleMoreThan valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DoubleMoreThan</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>DoubleMoreThan</c> obtenido por la operación.</returns>
    public static implicit operator DoubleMoreThan((double value, double length) tupleValues) => new DoubleMoreThan(tupleValues.value, tupleValues.length);
}

