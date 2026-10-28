// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

/// <summary>
/// Representa <c>DirectoryPath</c>.
/// </summary>
public partial class DirectoryPath : RegexValue
{
    /// <summary>
    /// Constante con search pattern.
    /// </summary>
    public const string SearchPattern = @"^(?:[a-zA-Z]:[\\\/]|\\\\[^\\\/]+[\\\/][^\\\/]+[\\\/]?|\.{1,2}[\\\/])?(?:[^<>:""/\\|?*\x00-\x1F]+[\\\/])*[^<>:""/\\|?*\x00-\x1F]*$";

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DirectoryPath"/>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    protected DirectoryPath(NotEmptyString value) : base(value, SearchPattern) { }

    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string FormatErrorMessage(string value) => $"{value} is not a valid directory path";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public static bool IsValid(string value) => RegexValue.IsValid(value, SearchPattern);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromPath</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DirectoryPath</c> obtenido por la operación.</returns>
    public static DirectoryPath CreateFromPath(string value) => new DirectoryPath(value);

    /// <summary>
    /// Ejecuta <c>FromPath</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<DirectoryPath> FromPath(string value, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(value)
                            .Bind( _ => EnsureFp.That(value, IsValid(value), errorsDetails ?? FormatErrorMessage(value)))
                            .Map ( _ => new DirectoryPath(value));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="valueObject">Valor de <c>valueObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string     (DirectoryPath valueObject) => valueObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>DirectoryPath</c>.
    /// </summary>
    /// <param name="value">Valor sobre el que se opera.</param>
    /// <returns>Valor de tipo <c>DirectoryPath</c> obtenido por la operación.</returns>
    public static implicit operator DirectoryPath(string      value)       => new DirectoryPath(value);
}

