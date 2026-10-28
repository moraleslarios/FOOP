// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers;
/// <summary>
/// Representa <c>Constants</c>.
/// </summary>
public static partial class Constants
{
    /// <summary>
    /// Obtiene el valor de <c>DEFAULT_ERROR_MESSAGE</c>.
    /// </summary>
    public static string DEFAULT_ERROR_MESSAGE { get; } = "Without custom error message. For more info, view 'Ex(s) details exceptions.";
    /// <summary>
    /// Obtiene el valor de <c>ExceptionMetadataKey</c>.
    /// </summary>
    public static string ExceptionMetadataKey           { get; } = "Ex";
    /// <summary>
    /// Obtiene el valor de <c>ValueMetadataKey</c>.
    /// </summary>
    public static string ValueMetadataKey             { get; } = "Value";
    /// <summary>
    /// Constante con not found key.
    /// </summary>
    public const string NOT_FOUND_KEY = "NotFound";
    /// <summary>
    /// Constante con param name key.
    /// </summary>
    public const string PARAM_NAME_KEY = "ParamName";

    /// <summary>
    /// Constante con failed indexes key.
    /// </summary>
    public const string FAILED_INDEXES_KEY = "FailedIndexes";

    /// <summary>
    /// Constante con expected key.
    /// </summary>
    public const string EXPECTED_KEY = "Expected";



    /// <summary>
    /// Ejecuta <c>DEFAULT_EX_ERROR_MESSAGE</c>.
    /// </summary>
    /// <param name="ex">Excepción capturada.</param>
    /// <returns>Representación textual obtenida.</returns>
    public static string DEFAULT_EX_ERROR_MESSAGE(Exception ex) => $"An error occurred while executing the function. Error: {ex.Message}.More info in Ex Details.";

}

