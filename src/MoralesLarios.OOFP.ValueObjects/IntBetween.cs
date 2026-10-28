// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>IntBetween</c>.
/// </summary>
public partial class IntBetween : ValueObject<int>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="IntBetween"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    protected IntBetween(int value, int minLength, int maxLength) : base(value)
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
    public static string FormatErrorMessage(int value, int minLength, int maxLength) => $"{value} must be between {minLength} and {maxLength}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(int value, int minLength, int maxLength) => value > minLength && value < maxLength;


    /// <summary>
    /// Crea la instancia solicitada de <c>FromIntLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <returns>Valor de tipo <c>IntBetween</c> obtenido por la operación.</returns>
    public static IntBetween CreateFromIntLength(int value, int minLength, int maxLength) => new IntBetween(value, minLength, maxLength);

    /// <summary>
    /// Ejecuta <c>FromIntLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="minLength">Valor de <c>minLength</c>.</param>
    /// <param name="maxLength">Valor de <c>maxLength</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntBetween> FromIntLength(int value, int minLength, int maxLength, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, minLength, maxLength), errorsDetails ?? FormatErrorMessage(value, minLength, maxLength)))
                    .Map ( _ => new IntBetween(value, minLength, maxLength));

    /// <summary>
    /// Realiza la conversión implícita a <c>int</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>int</c> obtenido por la operación.</returns>
    public static implicit operator int (IntBetween valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>IntBetween</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>IntBetween</c> obtenido por la operación.</returns>
    public static implicit operator IntBetween ((int value, int minLength, int maxLength) tupleValues) => new IntBetween(tupleValues.value, 
                                                                                                                         tupleValues.minLength, 
                                                                                                                         tupleValues.maxLength);



}
