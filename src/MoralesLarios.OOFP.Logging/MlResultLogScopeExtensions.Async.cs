// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

public static partial class MlResultLogScopeExtensions
{

    #region WithLogScopeAsync · plantilla y argumentos

    /// <summary>
    /// Abre un ámbito de registro descrito por una <b>plantilla</b> y sus argumentos, evalúa dentro
    /// de ese ámbito un tramo <b>asíncrono</b> de la cadena y lo cierra al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre · <c>"pedido {PedidoId}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="sectionAsync"/> es nulo.</returns>
    /// <remarks>
    /// <para>
    /// El ámbito se abre antes de <b>invocar</b> el tramo y se cierra en un <c>finally</c> después de
    /// <b>esperarlo</b>, de modo que cubre toda la continuación del tramo, no sólo su parte síncrona.
    /// </para>
    /// <para>
    /// La promesa es la que fija la decisión <c>D-36</c> opción <b>(b)</b>: el ámbito cubre lo que la
    /// librería <b>arranca</b> dentro del tramo. Lo que ya estuviese en marcha antes de abrirlo queda
    /// fuera.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var resultado = await ObtenerCliente(17)
    ///                         .WithLogScopeAsync(logger,
    ///                                            "cliente {ClienteId}", [17],
    ///                                            sectionAsync: r => r.BindAsync(FacturarAsync));
    /// </code>
    /// </example>
    public static Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this MlResult<TSource>                                source,
                                                                                  ILogger                                          logger,
                                                                                  [StringSyntax("MessageTemplate")]
                                                                                  string                                           scopeTemplate,
                                                                                  object?[]                                        scopeArgs,
                                                                                  Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync)
    {
        if (sectionAsync is null) return NullSectionMessage.ToMlResultFail<TResult>().ToAsync();

        if (logger is null || scopeTemplate is null) return sectionAsync(source);

        return RunInScopeAsync(source, logger.BeginScope(scopeTemplate, scopeArgs ?? []), sectionAsync);
    }

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por una <b>plantilla</b> y
    /// sus argumentos, evalúa dentro de ese ámbito un tramo <b>asíncrono</b> de la cadena y lo cierra
    /// al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="sectionAsync"/> es nulo.</returns>
    /// <remarks>
    /// <para>
    /// ⚠️ <b>Particularidad real, medida y no supuesta.</b> <paramref name="sourceAsync"/> se espera
    /// <b>antes</b> de abrir el ámbito, porque una tarea que ya está en marcha no se puede meter
    /// dentro de un contexto que todavía no existe. En consecuencia, las trazas que emita la tarea de
    /// origen <b>no llevan</b> las propiedades del ámbito. Es la decisión <c>D-36</c> opción
    /// <b>(b)</b>: se promete ámbito para lo que la librería arranca, no para lo que recibe ya
    /// iniciado.
    /// </para>
    /// <para>
    /// Si se quiere que la operación de origen también quede dentro del ámbito, hay que envolverla
    /// <b>dentro</b> del tramo, no pasarla como origen.
    /// </para>
    /// </remarks>
    public static async Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                          sourceAsync,
                                                                                        ILogger                                          logger,
                                                                                        [StringSyntax("MessageTemplate")]
                                                                                        string                                           scopeTemplate,
                                                                                        object?[]                                        scopeArgs,
                                                                                        Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync)
        => await (await sourceAsync).WithLogScopeAsync(logger, scopeTemplate, scopeArgs, sectionAsync);

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por una <b>plantilla</b> y
    /// sus argumentos, evalúa dentro de ese ámbito un tramo <b>síncrono</b> de la cadena y lo cierra
    /// al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// Misma particularidad que la sobrecarga de tramo asíncrono: <paramref name="sourceAsync"/> se
    /// espera <b>antes</b> de abrir el ámbito, por la decisión <c>D-36</c> opción <b>(b)</b>.
    /// </remarks>
    public static async Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                    sourceAsync,
                                                                                        ILogger                                    logger,
                                                                                        [StringSyntax("MessageTemplate")]
                                                                                        string                                     scopeTemplate,
                                                                                        object?[]                                  scopeArgs,
                                                                                        Func<MlResult<TSource>, MlResult<TResult>> section)
        => (await sourceAsync).WithLogScope(logger, scopeTemplate, scopeArgs, section);

    #endregion

    #region WithLogScopeAsync · diccionario de propiedades

    /// <summary>
    /// Abre un ámbito de registro descrito por un <b>diccionario</b> de propiedades, evalúa dentro de
    /// ese ámbito un tramo <b>asíncrono</b> de la cadena y lo cierra al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador sin reinterpretarlas. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="sectionAsync"/> es nulo.</returns>
    /// <remarks>
    /// El ámbito se abre antes de <b>invocar</b> el tramo y se cierra en un <c>finally</c> después de
    /// <b>esperarlo</b>, por lo que cubre también su continuación.
    /// </remarks>
    public static Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this MlResult<TSource>                                source,
                                                                                  ILogger                                          logger,
                                                                                  IEnumerable<KeyValuePair<string, object?>>       scopeProperties,
                                                                                  Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync)
    {
        if (sectionAsync is null) return NullSectionMessage.ToMlResultFail<TResult>().ToAsync();

        if (logger is null || scopeProperties is null) return sectionAsync(source);

        return RunInScopeAsync(source, logger.BeginScope(scopeProperties), sectionAsync);
    }

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por un <b>diccionario</b>
    /// de propiedades, evalúa dentro de ese ámbito un tramo <b>asíncrono</b> de la cadena y lo cierra
    /// al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="sectionAsync"/> es nulo.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito, por la decisión
    /// <c>D-36</c> opción <b>(b)</b>: sus trazas quedan <b>fuera</b> del ámbito.
    /// </remarks>
    public static async Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                          sourceAsync,
                                                                                        ILogger                                          logger,
                                                                                        IEnumerable<KeyValuePair<string, object?>>       scopeProperties,
                                                                                        Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync)
        => await (await sourceAsync).WithLogScopeAsync(logger, scopeProperties, sectionAsync);

    /// <summary>
    /// Espera el resultado de origen, abre un ámbito de registro descrito por un <b>diccionario</b>
    /// de propiedades, evalúa dentro de ese ámbito un tramo <b>síncrono</b> de la cadena y lo cierra
    /// al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="sourceAsync">Tarea que produce el resultado de origen.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>Tarea que produce el resultado del tramo, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// ⚠️ <paramref name="sourceAsync"/> se espera <b>antes</b> de abrir el ámbito, por la decisión
    /// <c>D-36</c> opción <b>(b)</b>: sus trazas quedan <b>fuera</b> del ámbito.
    /// </remarks>
    public static async Task<MlResult<TResult>> WithLogScopeAsync<TSource, TResult>(this Task<MlResult<TSource>>                    sourceAsync,
                                                                                        ILogger                                    logger,
                                                                                        IEnumerable<KeyValuePair<string, object?>> scopeProperties,
                                                                                        Func<MlResult<TSource>, MlResult<TResult>> section)
        => (await sourceAsync).WithLogScope(logger, scopeProperties, section);

    #endregion

    #region Núcleo compartido asíncrono

    /// <summary>
    /// Núcleo común de las sobrecargas asíncronas de <c>WithLogScopeAsync</c>: evalúa el tramo con el
    /// ámbito ya abierto, <b>espera</b> su continuación y cierra el ámbito en un <c>finally</c>.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="scope">Ámbito ya abierto, que se cierra al salir. Puede ser nulo si el registrador no devuelve ninguno.</param>
    /// <param name="sectionAsync">Tramo asíncrono de la cadena que se evalúa dentro del ámbito.</param>
    /// <returns>Tarea que produce el resultado del tramo.</returns>
    /// <remarks>
    /// El <c>await</c> está <b>dentro</b> del <c>try</c>, que es lo que hace que el ámbito siga
    /// abierto durante toda la continuación del tramo. La excepción que lo atraviese <b>se propaga</b>,
    /// por la decisión <c>D-34</c>, pero el <c>finally</c> garantiza que el ámbito se cierra.
    /// </remarks>
    private static async Task<MlResult<TResult>> RunInScopeAsync<TSource, TResult>(MlResult<TSource>                                source,
                                                                                   IDisposable?                                     scope,
                                                                                   Func<MlResult<TSource>, Task<MlResult<TResult>>> sectionAsync)
    {
        try
        {
            return await sectionAsync(source);
        }
        finally
        {
            scope?.Dispose();
        }
    }

    #endregion

}
