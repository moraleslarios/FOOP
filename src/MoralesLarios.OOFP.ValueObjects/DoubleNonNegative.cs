// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DoubleNonNegative</c>.
/// </summary>
public partial class DoubleNonNegative : DoubleMoreThan
{
    private const double MinAllowedValue = 0;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DoubleNonNegative"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected DoubleNonNegative(double value) : base(value, MinAllowedValue - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(double value) => $"{value} must be More than {MinAllowedValue}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(double value) => value >= MinAllowedValue;


    /// <summary>
    /// Crea la instancia solicitada de <c>FromDouble</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DoubleNonNegative</c> obtenido por la operación.</returns>
    public static DoubleNonNegative CreateFromDouble(double value) => new DoubleNonNegative(value);
    /// <summary>
    /// Ejecuta <c>FromDouble</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DoubleNonNegative> FromDouble(double value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new DoubleNonNegative(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>double</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>double</c> obtenido por la operación.</returns>
    public static implicit operator double            (DoubleNonNegative valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DoubleNonNegative</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DoubleNonNegative</c> obtenido por la operación.</returns>
    public static implicit operator DoubleNonNegative (double            value      ) => new DoubleNonNegative(value);

}
