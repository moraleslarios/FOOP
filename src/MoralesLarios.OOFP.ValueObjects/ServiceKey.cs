// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>ServiceKey</c>.
/// </summary>
public partial class ServiceKey : StringMinLength
{
    /// <summary>
    /// Obtiene el valor de <c>MinLength</c>.
    /// </summary>
    public static int MinLength => 1;

    private ServiceKey(string value) : base(value, MinLength)
    {
    }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => StringMinLength.FormatErrorMessage(value, MinLength);
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="length">Valor de <c>length</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static new bool IsValid(string value, int length) => value.Length >= length;

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    public static ServiceKey CreateFromString(string value) => new ServiceKey(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ServiceKey> FromString(string value, MlErrorDetails errorsDetails = null!)
        => StringMinLength.FromLength(value, MinLength, errorsDetails)
                            .Map(_ => new ServiceKey(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(ServiceKey   valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>ServiceKey</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>ServiceKey</c> obtenido por la operación.</returns>
    public static implicit operator ServiceKey   (string value      ) => new ServiceKey(value);
}

