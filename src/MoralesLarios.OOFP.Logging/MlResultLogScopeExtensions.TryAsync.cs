// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

public static partial class MlResultLogScopeExtensions
{

    #region TryWithLogScopeAsync · plantilla y argumentos

    /// <summary>
    /// Abre un ámbito de registro descrito por una <b>plantilla</b> y sus argumentos, evalúa dentro de
    /// ese ámbito un tramo <b>asíncrono</b> de la cadena, lo cierra al salir y <b>convierte en
    /// resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada. Si es nulo se usa el mensaje por omisión del núcleo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// La captura cubre toda la continuación del tramo, porque el <c>await</c> está <b>dentro</b> del
    /// <c>try</c>. El ámbito se cierra en el <c>finally</c>, por la decisión <c>D-34</c>.
    /// </remarks>
    public static Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this MlResult<TSource>                                source,
                                                                                     ILogger                                          logger,
                                                                                     [StringSyntax("MessageTemplate")]
                                                                                     string                                           scopeTemplate,
                                                                                     object?[]                                        scopeArgs,
                                                                                     Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync,
                                                                                     Func<Exception, string>?                         errorMessageBuilder = null)
    {
        if (sectionAsync is null) return NullSectionMessage.ToMlResultFail<TResult>().ToAsync();

        if (logger is null || scopeTemplate is null) return RunInTryScopeAsync(source, null, sectionAsync, errorMessageBuilder);

        return RunInTryScopeAsync(source, logger.BeginScope(scopeTemplate, scopeArgs ?? []), sectionAsync, errorMessageBuilder);
    }

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por una <b>plantilla</b> y
    /// sus argumentos, evalúa dentro de ese ámbito un tramo <b>asíncrono</b> de la cadena, lo cierra al
    /// salir y <b>convierte en resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito y <b>fuera</b> de la
    /// captura, por la decisión <c>D-36</c> opción <b>(b)</b>: lo que ya estaba en marcha no entra en un
    /// contexto que todavía no existe, y por tanto su excepción <b>se propaga</b> en lugar de
    /// convertirse en fallo.
    /// </remarks>
    public static async Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                          sourceAsync,
                                                                                            ILogger                                          logger,
                                                                                            [StringSyntax("MessageTemplate")]
                                                                                            string                                           scopeTemplate,
                                                                                            object?[]                                        scopeArgs,
                                                                                            Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync,
                                                                                            Func<Exception, string>?                         errorMessageBuilder = null)
        => await (await sourceAsync).TryWithLogScopeAsync(logger, scopeTemplate, scopeArgs, sectionAsync, errorMessageBuilder);

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por una <b>plantilla</b> y
    /// sus argumentos, evalúa dentro de ese ámbito un tramo <b>síncrono</b> de la cadena, lo cierra al
    /// salir y <b>convierte en resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito y <b>fuera</b> de la
    /// captura, por la decisión <c>D-36</c> opción <b>(b)</b>.
    /// </remarks>
    public static async Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                    sourceAsync,
                                                                                            ILogger                                    logger,
                                                                                            [StringSyntax("MessageTemplate")]
                                                                                            string                                     scopeTemplate,
                                                                                            object?[]                                  scopeArgs,
                                                                                            Func<MlResult<TSource>, MlResult<TResult>> section,
                                                                                            Func<Exception, string>?                   errorMessageBuilder = null)
        => (await sourceAsync).TryWithLogScope(logger, scopeTemplate, scopeArgs, section, errorMessageBuilder);

    #endregion

    #region TryWithLogScopeAsync · diccionario de propiedades

    /// <summary>
    /// Abre un ámbito de registro descrito por un <b>diccionario</b> de propiedades, evalúa dentro de
    /// ese ámbito un tramo <b>asíncrono</b> de la cadena, lo cierra al salir y <b>convierte en
    /// resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// La captura cubre toda la continuación del tramo y el ámbito se cierra en el <c>finally</c>, por la
    /// decisión <c>D-34</c>.
    /// </remarks>
    public static Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this MlResult<TSource>                                source,
                                                                                     ILogger                                          logger,
                                                                                     IEnumerable<KeyValuePair<string, object?>>       scopeProperties,
                                                                                     Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync,
                                                                                     Func<Exception, string>?                         errorMessageBuilder = null)
    {
        if (sectionAsync is null) return NullSectionMessage.ToMlResultFail<TResult>().ToAsync();

        if (logger is null || scopeProperties is null) return RunInTryScopeAsync(source, null, sectionAsync, errorMessageBuilder);

        return RunInTryScopeAsync(source, logger.BeginScope(scopeProperties), sectionAsync, errorMessageBuilder);
    }

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por un <b>diccionario</b> de
    /// propiedades, evalúa dentro de ese ámbito un tramo <b>asíncrono</b> de la cadena, lo cierra al
    /// salir y <b>convierte en resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito y <b>fuera</b> de la
    /// captura, por la decisión <c>D-36</c> opción <b>(b)</b>.
    /// </remarks>
    public static async Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                          sourceAsync,
                                                                                            ILogger                                          logger,
                                                                                            IEnumerable<KeyValuePair<string, object?>>       scopeProperties,
                                                                                            Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync,
                                                                                            Func<Exception, string>?                         errorMessageBuilder = null)
        => await (await sourceAsync).TryWithLogScopeAsync(logger, scopeProperties, sectionAsync, errorMessageBuilder);

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por un <b>diccionario</b> de
    /// propiedades, evalúa dentro de ese ámbito un tramo <b>síncrono</b> de la cadena, lo cierra al
    /// salir y <b>convierte en resultado fallido</b> cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito y <b>fuera</b> de la
    /// captura, por la decisión <c>D-36</c> opción <b>(b)</b>.
    /// </remarks>
    public static async Task<MlResult<TResult>> TryWithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                    sourceAsync,
                                                                                            ILogger                                    logger,
                                                                                            IEnumerable<KeyValuePair<string, object?>> scopeProperties,
                                                                                            Func<MlResult<TSource>, MlResult<TResult>> section,
                                                                                            Func<Exception, string>?                   errorMessageBuilder = null)
        => (await sourceAsync).TryWithLogScope(logger, scopeProperties, section, errorMessageBuilder);

    #endregion

    #region Núcleo compartido asíncrono de la captura

    /// <summary>
    /// Núcleo común de las sobrecargas asíncronas de <c>TryWithLogScopeAsync</c>: evalúa el tramo con el
    /// ámbito ya abierto, <b>espera</b> su continuación, convierte en fallo la excepción que la
    /// atraviese y cierra el ámbito en un <c>finally</c>.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="scope">Ámbito ya abierto, que se cierra al salir. Puede ser nulo si no se ha abierto ninguno.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// El <c>await</c> está <b>dentro</b> del <c>try</c>, así que la captura alcanza también a las
    /// excepciones que se lanzan después del primer punto de suspensión, no sólo a la parte síncrona del
    /// tramo.
    /// </remarks>
    private static async Task<MlResult<TResult>> RunInTryScopeAsync<TSource, TResult>(MlResult<TSource>                                source,
                                                                                      IDisposable?                                     scope,
                                                                                      Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync,
                                                                                      Func<Exception, string>?                         errorMessageBuilder)
    {
        try
        {
            return await sectionAsync(source);
        }
        catch (Exception ex)
        {
            return ExceptionToFail<TResult>(ex, errorMessageBuilder);
        }
        finally
        {
            scope?.Dispose();
        }
    }

    #endregion

}
