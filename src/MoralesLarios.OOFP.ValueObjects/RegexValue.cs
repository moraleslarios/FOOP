// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;

namespace MoralesLarios.OOFP.ValueObjects;

/// <summary>
/// Representa <c>RegexValue</c>.
/// </summary>
public partial class RegexValue : ValueObject<NotEmptyString>
{
    private readonly string _pattern;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="RegexValue"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    protected RegexValue(NotEmptyString value, string pattern) : base(value)
    {
        if ( ! IsValid(value, pattern)) throw new ArgumentNullException(nameof(value), FormatErrorMessage(value, pattern));

        _pattern = pattern;
    }

    /// <summary>
    /// Obtiene el valor de <c>Pattern</c>.
    /// </summary>
    public string Pattern => _pattern;

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value, string pattern)
        => $"{value} does not match regex pattern '{pattern}'";

    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value, string pattern) => Regex.IsMatch(value, pattern);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromRegex</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    /// <returns>Valor de tipo <c>RegexValue</c> obtenido por la operación.</returns>
    public static RegexValue CreateFromRegex(string value, string pattern) => new RegexValue(NotEmptyString.CreateFromString(value), pattern);

    /// <summary>
    /// Ejecuta <c>FromRegex</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="pattern">Valor de <c>pattern</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<RegexValue> FromRegex(string value, string pattern, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
            .Bind( _ => EnsureFp.That(value, IsValid(value, pattern), errorsDetails ?? FormatErrorMessage(value, pattern)))
            .Map ( _ => new RegexValue(value, pattern));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string(RegexValue valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>RegexValue</c>.
    /// </summary>
    /// <param name="tupleValues">Valor de <c>tupleValues</c>.</param>
    /// <returns>Valor de tipo <c>RegexValue</c> obtenido por la operación.</returns>
    public static implicit operator RegexValue((string value, string pattern) tupleValues)
        => new RegexValue(tupleValues.value, tupleValues.pattern);
}

