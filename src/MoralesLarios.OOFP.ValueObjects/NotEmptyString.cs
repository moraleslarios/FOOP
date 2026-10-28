// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>NotEmptyString</c>.
/// </summary>
public partial class NotEmptyString : ValueObject<string>
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="NotEmptyString"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected NotEmptyString(string value) : base(value)
    {
        if ( ! IsValid(value)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value));
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{nameof(value)} cannot be null or empty";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value) => ! string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>NotEmptyString</c> obtenido por la operación.</returns>
    public static NotEmptyString CreateFromString(string value) => new NotEmptyString(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<NotEmptyString> FromString(string value, MlErrorDetails errorsDetails = null!)
        => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value))
                    .Map(_ => new NotEmptyString(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string        (NotEmptyString valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>NotEmptyString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>NotEmptyString</c> obtenido por la operación.</returns>
    public static implicit operator NotEmptyString(string         value      ) => new NotEmptyString(value);
}

