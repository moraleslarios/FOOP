// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>IntMoreThan</c>.
/// </summary>
public partial class IntMoreThan : ValueObject<int>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="IntMoreThan"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    protected IntMoreThan(int value, int length) : base(value)
    {
        if ( ! IsValid(value, length)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, length));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(int value, int length) => $"{value} must be More than {length}";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(int value, int length) => value > length;


    /// <summary>
    /// Crea la instancia solicitada de <c>FromIntLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns>Valor de tipo <c>IntMoreThan</c> obtenido por la operación.</returns>
    public static IntMoreThan CreateFromIntLength(int value, int length) => new IntMoreThan(value, length);
    /// <summary>
    /// Ejecuta <c>FromIntLength</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<IntMoreThan> FromIntLength(int value, int length, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value, length), errorsDetails ?? FormatErrorMessage(value, length)))
                    .Map ( _ => new IntMoreThan(value, length));

    /// <summary>
    /// Realiza la conversión implícita a <c>int</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>int</c> obtenido por la operación.</returns>
    public static implicit operator int (IntMoreThan valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>IntMoreThan</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>IntMoreThan</c> obtenido por la operación.</returns>
    public static implicit operator IntMoreThan ((int value, int length) tupleValues) => new IntMoreThan(tupleValues.value, tupleValues.length);



}

