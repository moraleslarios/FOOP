// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

public static partial class MlResultLogScopeExtensions
{

    #region TryWithLogScope · plantilla y argumentos

    /// <summary>
    /// Abre un ámbito de registro descrito por una <b>plantilla</b> y sus argumentos, evalúa dentro de
    /// ese ámbito un tramo de la cadena, lo cierra al salir y <b>convierte en resultado fallido</b>
    /// cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeTemplate">Plantilla que describe el ámbito, con marcadores por nombre · <c>"pedido {PedidoId}"</c> ·. Debe ser una constante: los valores viajan en <paramref name="scopeArgs"/>. Si es nula, el tramo se evalúa sin ámbito.</param>
    /// <param name="scopeArgs">Valores que rellenan los marcadores de la plantilla, <b>por posición</b>. Si es nulo se trata como una lista vacía.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada. Si es nulo se usa el mensaje por omisión del núcleo.</param>
    /// <returns>El resultado del tramo, un resultado fallido con la excepción en los metadatos si el tramo lanza, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// <para>
    /// Es la variante que fija la decisión <c>D-34</c>: <c>WithLogScope</c> <b>propaga</b> la excepción y
    /// <c>TryWithLogScope</c> la <b>convierte</b> en un resultado fallido cuyo
    /// <see cref="MlErrorDetails"/> arrastra la excepción, recuperable con <c>GetException()</c>. El
    /// prefijo <c>Try</c> avisa de la captura, igual que en el núcleo.
    /// </para>
    /// <para>
    /// El ámbito se cierra en un <c>finally</c>, de modo que la captura de la excepción y el cierre del
    /// ámbito son independientes: un tramo que lanza no deja ámbitos abiertos.
    /// </para>
    /// <para>
    /// Un tramo que devuelve un resultado <b>fallido</b> —sin lanzar— no activa la captura: el fallo se
    /// devuelve tal cual, sin tocar sus metadatos, porque un fallo no es una excepción.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var resultado = ObtenerCliente(17)
    ///                     .TryWithLogScope(logger,
    ///                                      "cliente {ClienteId}", [17],
    ///                                      section: r => r.Map(Facturar));
    ///
    /// resultado.ToErrorsDetailsDescription();           // el mensaje del error
    /// // y la excepción original sigue ahí:
    /// // errorsDetails.GetException()
    /// </code>
    /// </example>
    public static MlResult<TResult> TryWithLogScope<TSource, TResult>(this MlResult<TSource>                         source,
                                                                          ILogger                                    logger,
                                                                          [StringSyntax("MessageTemplate")]
                                                                          string                                     scopeTemplate,
                                                                          object?[]                                  scopeArgs,
                                                                          Func<MlResult<TSource>, MlResult<TResult>> section,
                                                                          Func<Exception, string>?                   errorMessageBuilder = null)
    {
        if (section is null) return NullSectionMessage.ToMlResultFail<TResult>();

        if (logger is null || scopeTemplate is null) return RunInTryScope(source, null, section, errorMessageBuilder);

        return RunInTryScope(source, logger.BeginScope(scopeTemplate, scopeArgs ?? []), section, errorMessageBuilder);
    }

    #endregion

    #region TryWithLogScope · diccionario de propiedades

    /// <summary>
    /// Abre un ámbito de registro descrito por un <b>diccionario</b> de propiedades, evalúa dentro de
    /// ese ámbito un tramo de la cadena, lo cierra al salir y <b>convierte en resultado fallido</b>
    /// cualquier excepción que el tramo lance.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="logger">Registrador en el que se abre el ámbito. Si es nulo, el tramo se evalúa igualmente, pero sin ámbito.</param>
    /// <param name="scopeProperties">Propiedades del ámbito, que se pasan <b>tal cual</b> al registrador sin reinterpretarlas. Si es nulo, el tramo se evalúa sin ámbito.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito. No puede ser nulo.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada. Si es nulo se usa el mensaje por omisión del núcleo.</param>
    /// <returns>El resultado del tramo, un resultado fallido con la excepción en los metadatos si el tramo lanza, o un resultado fallido si <paramref name="section"/> es nulo.</returns>
    /// <remarks>
    /// Misma semántica que la sobrecarga de plantilla: cierre en <c>finally</c> y excepción convertida
    /// en fallo con la excepción en los metadatos, por la decisión <c>D-34</c>.
    /// </remarks>
    public static MlResult<TResult> TryWithLogScope<TSource, TResult>(this MlResult<TSource>                         source,
                                                                          ILogger                                    logger,
                                                                          IEnumerable<KeyValuePair<string, object?>> scopeProperties,
                                                                          Func<MlResult<TSource>, MlResult<TResult>> section,
                                                                          Func<Exception, string>?                   errorMessageBuilder = null)
    {
        if (section is null) return NullSectionMessage.ToMlResultFail<TResult>();

        if (logger is null || scopeProperties is null) return RunInTryScope(source, null, section, errorMessageBuilder);

        return RunInTryScope(source, logger.BeginScope(scopeProperties), section, errorMessageBuilder);
    }

    #endregion

    #region Núcleo compartido de la captura

    /// <summary>
    /// Núcleo común de las sobrecargas síncronas de <c>TryWithLogScope</c>: evalúa el tramo con el
    /// ámbito ya abierto, convierte en fallo la excepción que lo atraviese y cierra el ámbito en un
    /// <c>finally</c>.
    /// </summary>
    /// <typeparam name="TSource">Tipo del valor transportado por el resultado de origen.</typeparam>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado que produce el tramo.</typeparam>
    /// <param name="source">Resultado de origen que se entrega al tramo.</param>
    /// <param name="scope">Ámbito ya abierto, que se cierra al salir. Puede ser nulo si no se ha abierto ninguno.</param>
    /// <param name="section">Tramo de la cadena que se evalúa dentro del ámbito.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error a partir de la excepción capturada.</param>
    /// <returns>El resultado del tramo, o un resultado fallido con la excepción en los metadatos.</returns>
    /// <remarks>
    /// El <c>catch</c> y el <c>finally</c> conviven: el ámbito se cierra tanto si el tramo termina bien
    /// como si lanza, y el orden de ejecución —primero el <c>catch</c>, después el <c>finally</c>— es
    /// irrelevante aquí porque el fallo no se registra, sólo se devuelve.
    /// </remarks>
    private static MlResult<TResult> RunInTryScope<TSource, TResult>(MlResult<TSource>                         source,
                                                                     IDisposable?                              scope,
                                                                     Func<MlResult<TSource>, MlResult<TResult>> section,
                                                                     Func<Exception, string>?                  errorMessageBuilder)
    {
        try
        {
            return section(source);
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

    /// <summary>
    /// Convierte una excepción capturada en un resultado fallido que arrastra la propia excepción en
    /// los metadatos del error, con la misma clave que usa el núcleo.
    /// </summary>
    /// <typeparam name="TResult">Tipo del valor transportado por el resultado fallido.</typeparam>
    /// <param name="ex">Excepción capturada.</param>
    /// <param name="errorMessageBuilder">Constructor del mensaje de error. Si es nulo se usa el mensaje por omisión del núcleo.</param>
    /// <returns>Resultado fallido cuya excepción se recupera con <c>GetException()</c>.</returns>
    /// <remarks>
    /// Delega en <see cref="MlErrorDetails.FromErrorMessageWithException(string, Exception)"/>, que es
    /// el mismo camino que recorre la familia <c>Try*</c> del núcleo, para que el consumidor recupere la
    /// excepción exactamente igual en los dos sitios.
    /// </remarks>
    internal static MlResult<TResult> ExceptionToFail<TResult>(Exception ex, Func<Exception, string>? errorMessageBuilder)
        => MlErrorDetails.FromErrorMessageWithException(errorMessageBuilder is null
                                                            ? MoralesLarios.OOFP.Helpers.EnsureFpMessages.DefaultExceptionError(ex)
                                                            : errorMessageBuilder(ex),
                                                        ex)
                         .ToMlResultFail<TResult>();

    #endregion

}
