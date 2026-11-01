// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

/// <summary>
/// Extensiones que abren un <b>ámbito de registro</b> sobre un tramo de una cadena de
/// <see cref="MlResult{T}"/>, de modo que todas las trazas que se emitan dentro de ese tramo
/// compartan el mismo contexto.
/// </summary>
/// <remarks>
/// <para>
/// Materializa las decisiones <c>D-33</c> a <c>D-38</c> de la épica 9 de la versión 2.4.0:
/// </para>
/// <list type="bullet">
///   <item><description>
///     <c>D-33</c> · el ámbito <b>envuelve un tramo</b> que se recibe como función, en lugar de
///     obligar al consumidor a partir la cadena en dos sentencias con un <c>using</c> a mano.
///   </description></item>
///   <item><description>
///     <c>D-34</c> · el ámbito se cierra en un <c>finally</c> y la excepción que atraviese el tramo
///     <b>se propaga</b>, igual que en el resto de la librería.
///   </description></item>
///   <item><description>
///     <c>D-35</c> · el ámbito es <b>transparente</b>: no emite ninguna traza propia, sólo añade
///     propiedades a lo que el tramo registre. Un ámbito sobre un tramo que no registra nada no
///     produce <b>ninguna</b> traza.
///   </description></item>
///   <item><description>
///     <c>D-37</c> · las propiedades del ámbito se pueden aportar como <b>plantilla y argumentos</b>
///     —la forma principal— o como un <b>diccionario</b>, que se pasa tal cual al registrador.
///   </description></item>
///   <item><description>
///     <c>D-38</c> · el ámbito <b>no se cruza</b> con los seis niveles, con el <c>EventId</c> ni con
///     <c>ILogger&lt;TCategory&gt;</c>, porque el ámbito no es un registrador y no emite traza propia.
///   </description></item>
/// </list>
/// </remarks>
public static partial class MlResultLogScopeExtensions
{

    /// <summary>
    /// Mensaje de error que se devuelve cuando el tramo que se pretende envolver es nulo.
    /// </summary>
    /// <remarks>
    /// A diferencia de un registrador nulo, que es una no-operación porque el registro nunca altera
    /// el flujo, un tramo nulo <b>es</b> el flujo: no hay nada que evaluar ni ningún resultado de
    /// origen del tipo de salida que devolver, de modo que la cadena pasa a fallida.
    /// </remarks>
    internal const string NullSectionMessage = "El parámetro 'section' no puede ser nulo.";

    #region WithLogScope · plantilla y argumentos

    /// <summary>
    /// Abre un ámbito de registro descrito por una <b>plantilla</b> y sus argumentos, evalúa el
    /// tramo indicado dentro de ese ámbito y lo cierra al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre · <c>"pedido {PedidoId}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>El resultado que produce el tramo, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// <para>
    /// El ámbito se abre <b>antes</b> de evaluar el tramo y se cierra <b>después</b> de su última
    /// traza, en un <c>finally</c>, de modo que una excepción que atraviese el tramo no deja el
    /// ámbito abierto. La excepción <b>se propaga</b>, por la decisión <c>D-34</c>.
    /// </para>
    /// <para>
    /// El ámbito es transparente: no emite traza alguna por sí mismo. Si el tramo no registra nada,
    /// no se verá nada en el destino.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .WithLogScope(logger,
    ///                                   "cliente {ClienteId}", [17],
    ///                                   section: r => r.LogMlResultInformationTemplate(logger, "cliente consultado")
    ///                                                  .Bind(Facturar));
    /// </code>
    /// </example>
    public static MlResult<TResult> WithLogScope<TSource, TResult>(this MlResult<TSource>                          source,
                                                                       ILogger                                    logger,
                                                                       [StringSyntax("MessageTemplate")]
                                                                       string                                     scopeTemplate,
                                                                       object?[]                                   scopeArgs,
                                                                       Func<MlResult<TSource>, MlResult<TResult>> section)
    {
        if (section is null) return NullSectionMessage.ToMlResultFail<TResult>();

        if (logger is null || scopeTemplate is null) return section(source);

        return RunInScope(source, logger.BeginScope(scopeTemplate, scopeArgs ?? []), section);
    }

    #endregion

    #region WithLogScope · diccionario de propiedades

    /// <summary>
    /// Abre un ámbito de registro descrito por un <b>diccionario</b> de propiedades, evalúa el tramo
    /// indicado dentro de ese ámbito y lo cierra al salir.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador sin reinterpretarlas. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <returns>El resultado que produce el tramo, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// Esta sobrecarga existe para quien compone el contexto en <b>tiempo de ejecución</b> —por
    /// ejemplo a partir de una cabecera HTTP—, donde no hay una plantilla constante que escribir.
    /// El resto del contrato es idéntico al de la sobrecarga de plantilla.
    /// </remarks>
    /// <example>
    /// <code>
    /// var propiedades = new Dictionary&lt;string, object?&gt; { ["CorrelacionId"] = correlacion };
    ///
    /// var resultado = ObtenerCliente(17)
    ///                     .WithLogScope(logger, propiedades, section: r => r.Bind(Facturar));
    /// </code>
    /// </example>
    public static MlResult<TResult> WithLogScope<TSource, TResult>(this MlResult<TSource>                          source,
                                                                       ILogger                                    logger,
                                                                       IEnumerable<KeyValuePair<string, object?>>  scopeProperties,
                                                                       Func<MlResult<TSource>, MlResult<TResult>> section)
    {
        if (section is null) return NullSectionMessage.ToMlResultFail<TResult>();

        if (logger is null || scopeProperties is null) return section(source);

        return RunInScope(source, logger.BeginScope(scopeProperties), section);
    }

    #endregion

    #region Núcleo compartido

    /// <summary>
    /// Núcleo común de las sobrecargas de <c>WithLogScope</c>: evalúa el tramo con el ámbito ya
    /// abierto y cierra ese ámbito en un <c>finally</c>.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="scope">Ámbito ya abierto, que se cierra al salir. Puede ser nulo si el registrador no devuelve ninguno.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito.</param>
    /// <returns>El resultado que produce el tramo.</returns>
    /// <remarks>
    /// La excepción que atraviese el tramo <b>se propaga</b>, por la decisión <c>D-34</c>, pero el
    /// <c>finally</c> garantiza que el ámbito no se queda abierto.
    /// </remarks>
    private static MlResult<TResult> RunInScope<TSource, TResult>(MlResult<TSource>                          source,
                                                                  IDisposable?                               scope,
                                                                  Func<MlResult<TSource>, MlResult<TResult>> section)
    {
        try
        {
            return section(source);
        }
        finally
        {
            scope?.Dispose();
        }
    }

    #endregion

}
