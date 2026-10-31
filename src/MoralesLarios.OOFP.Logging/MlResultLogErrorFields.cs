// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;

/// <summary>
/// Contrato público de los campos estructurados que la librería sabe derivar de un
/// <see cref="MlErrorDetails"/> para la rama de fallo de las familias <c>…Template</c>.
/// </summary>
/// <remarks>
/// <para>
/// Materializa la decisión <c>D-32</c> de la épica 8 de la versión 2.3.0: en la rama de fallo los
/// argumentos de la plantilla se pueden aportar de dos maneras, y las dos conviven.
/// </para>
/// <list type="number">
/// <item>
/// <description>
/// <b>Los aporta el llamante</b>, con la sobrecarga que recibe <c>params object?[] args</c>. La
/// librería no mira el fallo: se limita a pasar la plantilla y los argumentos al registrador.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Se derivan del fallo</b>, con la sobrecarga que recibe un
/// <c>Func&lt;MlErrorDetails, object?[]&gt;</c>. El constructor sólo se ejecuta si el nivel está
/// habilitado, igual que los constructores de mensaje del resto de la API.
/// </description>
/// </item>
/// </list>
/// <para>
/// ⚠️ <b>Los tres nombres de campo de <see cref="DefaultTemplate"/> son contrato.</b> Un
/// <i>sink</i> que indexe por <c>ErrorMessage</c>, <c>ErrorCount</c> o <c>ExceptionType</c>
/// dejaría de encontrar los datos si se renombrasen, y el compilador del consumidor no avisaría de
/// nada. Por eso cambiarlos en el futuro sería una <b>ruptura silenciosa</b> y exigiría su punto
/// propio en el prompt de migración.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Rama de fallo con los campos canónicos, en una línea.
/// MlResult&lt;int&gt; resultado = origen.LogMlResultErrorIfFailTemplate(logger,
///                                                                 MlResultLogErrorFields.DefaultTemplate,
///                                                                 MlResultLogErrorFields.From);
///
/// // Rama de fallo con campos propios del dominio.
/// MlResult&lt;int&gt; otro = origen.LogMlResultErrorIfFailTemplate(logger,
///                                                           "El pedido {PedidoId} ha fallado: {Motivo}",
///                                                           errors =&gt; [pedidoId, errors.GetErrorsSummary()]);
/// </code>
/// </example>
public static class MlResultLogErrorFields
{

    /// <summary>
    /// Nombre del campo estructurado que transporta el resumen de los mensajes de error.
    /// </summary>
    public const string ErrorMessageField = "ErrorMessage";

    /// <summary>
    /// Nombre del campo estructurado que transporta el número de errores del detalle.
    /// </summary>
    public const string ErrorCountField = "ErrorCount";

    /// <summary>
    /// Nombre del campo estructurado que transporta el nombre del tipo de la excepción asociada al
    /// fallo, o <see cref="NoExceptionValue"/> si el fallo no lleva excepción.
    /// </summary>
    public const string ExceptionTypeField = "ExceptionType";

    /// <summary>
    /// Valor que se publica en el campo <see cref="ExceptionTypeField"/> cuando el detalle del
    /// error no lleva ninguna excepción en sus metadatos.
    /// </summary>
    /// <remarks>
    /// Se publica un valor explícito, y no nulo, para que el campo exista siempre en el
    /// <i>sink</i> y las consultas por ausencia de excepción sean posibles.
    /// </remarks>
    public const string NoExceptionValue = "(ninguna)";

    /// <summary>
    /// Plantilla canónica que consume, <b>en este orden</b>, los tres campos que
    /// <see cref="From(MlErrorDetails)"/> produce.
    /// </summary>
    /// <remarks>
    /// Los marcadores de <c>Microsoft.Extensions.Logging</c> se rellenan <b>por posición</b>, no
    /// por nombre. Si se usa una plantilla propia con <see cref="From(MlErrorDetails)"/> hay que
    /// respetar el orden <c>ErrorMessage</c> · <c>ErrorCount</c> · <c>ExceptionType</c>.
    /// </remarks>
    public const string DefaultTemplate = "El resultado ha fallado: {ErrorMessage} · errores: {ErrorCount} · excepción: {ExceptionType}";

    /// <summary>
    /// Deriva del detalle del error los tres argumentos que consume
    /// <see cref="DefaultTemplate"/>, en el orden que esa plantilla espera.
    /// </summary>
    /// <param name="errorsDetails">Detalle del error del que se derivan los campos. Puede ser nulo.</param>
    /// <returns>
    /// Los valores de <c>ErrorMessage</c>, <c>ErrorCount</c> y <c>ExceptionType</c>, en ese orden.
    /// </returns>
    /// <remarks>
    /// Este método no lanza excepciones: un detalle nulo, sin errores o sin excepción produce
    /// valores neutros, nunca un fallo al registrar.
    /// </remarks>
    public static object?[] From(MlErrorDetails errorsDetails)
        => [ ErrorMessageOf(errorsDetails), ErrorCountOf(errorsDetails), ExceptionTypeOf(errorsDetails) ];

    /// <summary>
    /// Devuelve el resumen de los mensajes de error del detalle indicado.
    /// </summary>
    /// <param name="errorsDetails">Detalle del error. Puede ser nulo.</param>
    /// <returns>El resumen de los errores, o cadena vacía si no hay ninguno.</returns>
    public static string ErrorMessageOf(MlErrorDetails errorsDetails)
        => errorsDetails is null ? string.Empty : errorsDetails.GetErrorsSummary() ?? string.Empty;

    /// <summary>
    /// Devuelve el número de errores del detalle indicado.
    /// </summary>
    /// <param name="errorsDetails">Detalle del error. Puede ser nulo.</param>
    /// <returns>El número de errores, o cero si no hay ninguno.</returns>
    public static int ErrorCountOf(MlErrorDetails errorsDetails)
        => errorsDetails?.Errors?.Count() ?? 0;

    /// <summary>
    /// Devuelve el nombre del tipo de la excepción asociada al detalle indicado.
    /// </summary>
    /// <param name="errorsDetails">Detalle del error. Puede ser nulo.</param>
    /// <returns>
    /// El nombre del tipo de la excepción, o <see cref="NoExceptionValue"/> si el detalle no lleva
    /// excepción en sus metadatos.
    /// </returns>
    public static string ExceptionTypeOf(MlErrorDetails errorsDetails)
        => errorsDetails is not null && errorsDetails.GetException().TryGetValue(out var exception) && exception is not null
                ? exception.GetType().Name
                : NoExceptionValue;

}
