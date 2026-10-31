// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

/// <summary>
/// Guardas internas de la política de argumentos nulos de las extensiones de registro.
/// </summary>
/// <remarks>
/// Materializa las decisiones <c>D-06</c> y <c>D-12</c> de la épica 3 de la versión 2.2.0:
/// un fallo de registro nunca altera el flujo que está intentando registrar. Cuando el
/// registrador o el constructor de mensaje son nulos, la operación se convierte en una
/// no-operación y se devuelve el resultado de origen intacto.
/// </remarks>
internal static class MlResultLogGuards
{

    /// <summary>
    /// Mensaje de error que se devuelve cuando el registrador es nulo y no hay resultado de
    /// origen que preservar.
    /// </summary>
    internal const string NullLoggerMessage = "El parámetro 'logger' no puede ser nulo.";

    /// <summary>
    /// Construye el resultado fallido que sustituye a la traza cuando el registrador es nulo.
    /// </summary>
    /// <returns>Resultado fallido con el mensaje <see cref="NullLoggerMessage"/>.</returns>
    internal static MlResult<ILogger> NullLoggerFail() => NullLoggerMessage.ToMlResultFail<ILogger>();

    /// <summary>
    /// Construye el resultado fallido que sustituye a la traza cuando el registrador es nulo.
    /// Versión asíncrona.
    /// </summary>
    /// <returns>Tarea que produce el resultado fallido con el mensaje <see cref="NullLoggerMessage"/>.</returns>
    internal static Task<MlResult<ILogger>> NullLoggerFailAsync() => NullLoggerFail().ToAsync();

    /// <summary>
    /// Invoca un constructor de mensaje de un argumento, devolviendo nulo cuando el propio
    /// constructor es nulo, cuando el registrador es nulo o cuando el nivel indicado está
    /// deshabilitado en el registrador.
    /// </summary>
    /// <typeparam name="TArg">Tipo del argumento que recibe el constructor de mensaje.</typeparam>
    /// <param name="logger">Registrador al que se preguntará si el nivel está habilitado.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registraría la traza.</param>
    /// <param name="builder">Constructor de mensaje, que puede ser nulo.</param>
    /// <param name="arg">Argumento que se pasa al constructor de mensaje.</param>
    /// <returns>El mensaje construido, o nulo si no hay nada que registrar.</returns>
    /// <remarks>
    /// Materializa la decisión <c>D-19</c> de la épica 6 de la versión 2.2.0, resuelta con la
    /// opción <c>(c)</c>: evaluar siempre el constructor de mensaje contradice el contrato de
    /// <c>Microsoft.Extensions.Logging</c>, de modo que se corrige como defecto. El efecto es una
    /// <b>ruptura silenciosa</b>: los efectos colaterales que el consumidor tuviese dentro del
    /// constructor de mensaje dejan de producirse cuando el nivel está apagado.
    /// </remarks>
    internal static string Build<TArg>(ILogger            logger,
                                       LogLevel           logLevel,
                                       Func<TArg, string> builder,
                                       TArg               arg)
        => builder is null || ! IsEnabled(logger, logLevel) ? null! : builder(arg);

    /// <summary>
    /// Invoca un constructor de mensaje de dos argumentos, devolviendo nulo cuando el propio
    /// constructor es nulo, cuando el registrador es nulo o cuando el nivel indicado está
    /// deshabilitado en el registrador.
    /// </summary>
    /// <typeparam name="TArg1">Tipo del primer argumento que recibe el constructor de mensaje.</typeparam>
    /// <typeparam name="TArg2">Tipo del segundo argumento que recibe el constructor de mensaje.</typeparam>
    /// <param name="logger">Registrador al que se preguntará si el nivel está habilitado.</param>
    /// <param name="logLevel">Nivel de severidad con el que se registraría la traza.</param>
    /// <param name="builder">Constructor de mensaje, que puede ser nulo.</param>
    /// <param name="arg1">Primer argumento que se pasa al constructor de mensaje.</param>
    /// <param name="arg2">Segundo argumento que se pasa al constructor de mensaje.</param>
    /// <returns>El mensaje construido, o nulo si no hay nada que registrar.</returns>
    /// <remarks>
    /// Materializa la decisión <c>D-19</c> de la épica 6 de la versión 2.2.0, resuelta con la
    /// opción <c>(c)</c>.
    /// </remarks>
    internal static string Build<TArg1, TArg2>(ILogger                    logger,
                                               LogLevel                   logLevel,
                                               Func<TArg1, TArg2, string> builder,
                                               TArg1                      arg1,
                                               TArg2                      arg2)
        => builder is null || ! IsEnabled(logger, logLevel) ? null! : builder(arg1, arg2);

    /// <summary>
    /// Indica si merece la pena construir el mensaje para el nivel indicado.
    /// </summary>
    /// <param name="logger">Registrador al que se pregunta, que puede ser nulo.</param>
    /// <param name="logLevel">Nivel de severidad que se consulta.</param>
    /// <returns><c>true</c> si el registrador existe y acepta el nivel; <c>false</c> en caso contrario.</returns>
    internal static bool IsEnabled(ILogger logger, LogLevel logLevel)
        => logger is not null && logger.IsEnabled(logLevel);

}
