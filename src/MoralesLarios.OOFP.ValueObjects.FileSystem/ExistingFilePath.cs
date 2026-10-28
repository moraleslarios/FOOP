// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

/// <summary>
/// Representa <c>ExistingFilePath</c>.
/// </summary>
public partial class ExistingFilePath : NotEmptyString
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ExistingFilePath"/>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    protected ExistingFilePath(string pathStr) : base(pathStr)
    {
        if ( ! IsValid(pathStr)) throw new FileNotFoundException(FormatErrorMessage(pathStr));
    } 


    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public new static string FormatErrorMessage(string pathStr) => $"{pathStr} not exists";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public new static bool IsValid(string pathStr) => File.Exists(pathStr);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromPath</c>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <returns>Valor de tipo <c>ExistingFilePath</c> obtenido por la operación.</returns>
    public static ExistingFilePath CreateFromPath(string pathStr) => new ExistingFilePath(pathStr);

    /// <summary>
    /// Ejecuta <c>FromPath</c>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ExistingFilePath> FromPath(string pathStr, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(pathStr)
                            .Bind( _ => EnsureFp.That(pathStr, IsValid(pathStr), errorsDetails ?? FormatErrorMessage(pathStr)))
                            .Map ( _ => new ExistingFilePath(pathStr));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="pathStrObject">Valor de <c>pathStrObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string    (ExistingFilePath pathStrObject) => pathStrObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>ExistingFilePath</c>.
    /// </summary>
    /// <param name="pathStr">Valor de <c>pathStr</c>.</param>
    /// <returns>Valor de tipo <c>ExistingFilePath</c> obtenido por la operación.</returns>
    public static implicit operator ExistingFilePath(string     pathStr      ) => new ExistingFilePath(pathStr);
}

