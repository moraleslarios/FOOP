// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.ValueObjects.FileSystem;

/// <summary>
/// Representa <c>ExistingDirectoryPath</c>.
/// </summary>
public partial class ExistingDirectoryPath : NotEmptyString
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ExistingDirectoryPath"/>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    protected ExistingDirectoryPath(string directoryStr) : base(directoryStr)
    {
        if (!IsValid(directoryStr)) throw new DirectoryNotFoundException(FormatErrorMessage(directoryStr));
    }


    /// <summary>
    /// Ejecuta <c>FormatErrorMessage</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public new static string FormatErrorMessage(string directoryStr) => $"{directoryStr} not exists";
    /// <summary>
    /// Ejecuta <c>IsValid</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns><c>true</c> si la condición se cumple; en caso contrario, <c>false</c>.</returns>
    public new static bool IsValid(string directoryStr) => Directory.Exists(directoryStr);

    /// <summary>
    /// Crea la instancia solicitada de <c>FromPath</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns>Valor de tipo <c>ExistingDirectoryPath</c> obtenido por la operación.</returns>
    public static ExistingDirectoryPath CreateFromPath(string directoryStr) => new ExistingDirectoryPath(directoryStr);

    /// <summary>
    /// Ejecuta <c>FromPath</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<ExistingDirectoryPath> FromPath(string directoryStr, MlErrorDetails errorsDetails = null!)
        => NotEmptyString.FromString(directoryStr)
                            .Bind( _ => EnsureFp.That(directoryStr, IsValid(directoryStr), errorsDetails ?? FormatErrorMessage(directoryStr)))
                            .Map ( _ => new ExistingDirectoryPath(directoryStr));

    /// <summary>
    /// Realiza la conversión implícita a <c>string</c>.
    /// </summary>
    /// <param name="directoryStrObject">Valor de <c>directoryStrObject</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static implicit operator string        (ExistingDirectoryPath directoryStrObject) => directoryStrObject.Value;
    /// <summary>
    /// Realiza la conversión implícita a <c>ExistingDirectoryPath</c>.
    /// </summary>
    /// <param name="directoryStr">Valor de <c>directoryStr</c>.</param>
    /// <returns>Valor de tipo <c>ExistingDirectoryPath</c> obtenido por la operación.</returns>
    public static implicit operator ExistingDirectoryPath(string         directoryStr      ) => new ExistingDirectoryPath(directoryStr);
}
