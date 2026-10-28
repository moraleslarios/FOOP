// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
/// <summary>
/// Métodos de extension de <c></c>.
/// </summary>
public static class Extensions
{

    /// <summary>
    /// Ejecuta <c>MyMethodFinalLog</c>.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado de origen sobre el que se aplica la operación.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Valor de <c>methodActionDesc</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> MyMethodFinalLog<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      methodActionDesc)
        => source.LogCompletedResult(logger,
                                   validBuildMessage: item   => $"{methodActionDesc} done correctly.",
                                   failBuildMessage : errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");

    /// <summary>
    /// Ejecuta <c>MyMethodFinalLog</c>. Versión asíncrona.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="methodActionDesc">Valor de <c>methodActionDesc</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static async Task<MlResult<T>> MyMethodFinalLogAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                  ILogger     logger,
                                                                  string      methodActionDesc)
        => await (await sourceAsync).LogCompletedResultAsync(logger,
                                                           validBuildMessage: item => $"{methodActionDesc} done correctly.",
                                                           failBuildMessage: errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");




}

