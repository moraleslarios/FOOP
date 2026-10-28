// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>DecimalNonNegative</c>.
/// </summary>
public partial class DecimalNonNegative : DecimalMoreThan
{
    private const decimal MinAllowedValue = 0m;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DecimalNonNegative"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected DecimalNonNegative(decimal value) : base(value, MinAllowedValue - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(decimal value) => $"{value} must be More than {MinAllowedValue}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(decimal value) => value >= MinAllowedValue;


    /// <summary>
    /// Crea la instancia solicitada de <c>FromDecimal</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DecimalNonNegative</c> obtenido por la operación.</returns>
    public static DecimalNonNegative CreateFromDecimal(decimal value) => new DecimalNonNegative(value);
    /// <summary>
    /// Ejecuta <c>FromDecimal</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DecimalNonNegative> FromDecimal(decimal value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new DecimalNonNegative(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>decimal</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>decimal</c> obtenido por la operación.</returns>
    public static implicit operator decimal            (DecimalNonNegative valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DecimalNonNegative</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DecimalNonNegative</c> obtenido por la operación.</returns>
    public static implicit operator DecimalNonNegative (decimal            value      ) => new DecimalNonNegative(value);

}

