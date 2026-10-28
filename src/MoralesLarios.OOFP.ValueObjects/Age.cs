// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>Age</c>.
/// </summary>
public partial class Age : IntMoreThan
{
    private static int Limit => -1;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="Age"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected Age(int value) : base(value, Limit)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), FormatErrorMessage(value));
    }
    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(int value) => $"Age cannot be negative.";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(int value) => Age.IsValid(value, Limit);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>Age</c> obtenido por la operación.</returns>
    public static Age CreateFromInt(int value) => new Age(value);
    /// <summary>
    /// Ejecuta <c>FromInt</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<Age> FromInt(int value, MlErrorDetails errorsDetails = null!)
        => MlResult.Empty()
                    .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                    .Map ( _ => new Age(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>int</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Valor de tipo <c>int</c> obtenido por la operación.</returns>
    public static implicit operator int (Age valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>Age</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>Age</c> obtenido por la operación.</returns>
    public static implicit operator Age (int value) => new Age(value);


}

