// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extensión que registran, en una sola llamada, el desenlace completo de la acción
/// de un método: una traza de éxito con el valor obtenido o una traza de error con los detalles
/// del fallo.
/// </summary>
/// <remarks>
/// Todos los mensajes que esta clase construye se emiten en <b>inglés</b>, conforme a la política
/// de idioma fijada en la versión 2.2.0 para el texto que genera la propia librería.
/// </remarks>
public static class MlResultLogActionExtensions
{

    /// <summary>
    /// Registra el desenlace completo de la acción de un método sobre el resultado recibido.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Descripción de la acción del método que se está registrando.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> LogActionResult<T>(this MlResult<T> source,
                                                      ILogger     logger,
                                                      string      methodActionDesc)
        => source.LogCompletedResult(logger,
                                   validBuildMessage: item   => $"{methodActionDesc} done correctly.",
                                   failBuildMessage : errors => $"Error when {methodActionDesc}: {errors.ToErrorsDetailsDescription()}");

    /// <summary>
    /// Registra el desenlace completo de la acción de un método sobre el resultado recibido. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Descripción de la acción del método que se está registrando.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> LogActionResultAsync<T>(this Task<MlResult<T>> sourceAsync,
                                                                       ILogger           logger,
                                                                       string            methodActionDesc)
        => await (await sourceAsync).LogCompletedResultAsync(logger,
                                                           validBuildMessage: item   => $"{methodActionDesc} done correctly.",
                                                           failBuildMessage : errors => $"Error when {methodActionDesc}: {errors.ToErrorsDetailsDescription()}");

}
