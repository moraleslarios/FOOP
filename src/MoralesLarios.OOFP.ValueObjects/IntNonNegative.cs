// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>IntNonNegative</c>.
/// </summary>
public partial class IntNonNegative : IntMoreThan
{
    private const int Limit = 0;

    private IntNonNegative(int value) : base(value, Limit - 1)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(int value) => $"{value} must be More than {Limit}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(int value) => value >= Limit;


    /// <summary>
    /// Crea la instancia solicitada de <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>IntNonNegative</c> obtenido por la operación.</returns>
    public static IntNonNegative CreateFromInt(int value) => new IntNonNegative(value);
    /// <summary>
    /// Ejecuta <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntNonNegative> FromInt(int value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new IntNonNegative(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>int</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>int</c> obtenido por la operación.</returns>
    public static implicit operator int            (IntNonNegative valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>IntNonNegative</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>IntNonNegative</c> obtenido por la operación.</returns>
    public static implicit operator IntNonNegative (int            value      ) => new IntNonNegative(value);

}

