// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>Endpoint</c>.
/// </summary>
public partial class Endpoint : RegexValue
{
    /// <summary>
    /// Constante con validation pattern.
    /// </summary>
    public const string ValidationPattern = @"^https://[A-Za-z0-9.-]+(?::[1-9][0-9]{0,4})?$"; // host + optional port

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="Endpoint"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected Endpoint(NotEmptyString value) : base(value, ValidationPattern) { }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{value} is not a valid endpoint";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool   IsValid(string value)          => RegexValue.IsValid(value, ValidationPattern);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>Endpoint</c> obtenido por la operación.</returns>
    public static Endpoint CreateFromString(string value) => new Endpoint(value);

    /// <summary>
    /// Ejecuta <c>FromString</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<Endpoint> FromString(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind(_ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map (_ => new Endpoint(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string  (Endpoint valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>Endpoint</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>Endpoint</c> obtenido por la operación.</returns>
    public static implicit operator Endpoint(string   value      ) => new Endpoint(value);
}
        
