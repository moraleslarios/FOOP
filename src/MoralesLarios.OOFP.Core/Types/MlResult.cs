namespace MoralesLarios.OOFP.Types;


/// <summary>
/// Punto de entrada no genérico con las fábricas estáticas para crear instancias de <see cref="MlResult{T}"/>.
/// </summary>
public partial record MlResult
{
    /// <summary>
    /// Resultado válido sin valor relevante. Equivalente funcional a <c>Unit</c>.
    /// </summary>
    public static MlResult Discard  => new(default!);

    /// <summary>
    /// Crea un resultado válido que transporta el valor indicado.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="t">Valor de <c>t</c>.</param>
    /// <returns>Resultado válido con el valor obtenido.</returns>
    public static MlResult<T> Valid<T>(T t) => new(t);

    /// <summary>
    /// Crea un resultado fallido con los errores indicados.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(params MlError[]            errors)        => new(errors);
    /// <summary>
    /// Crea un resultado fallido a partir de los detalles de error indicados.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errorsDetails">Detalles del error asociados al fallo.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(       MlErrorDetails      errorsDetails) => new(errorsDetails);
    /// <summary>
    /// Crea un resultado fallido con los errores indicados.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(       IEnumerable<MlError> errors       ) => new(errors);

    /// <summary>
    /// Crea un resultado fallido con el error indicado.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(MlError              error                                         ) => new(error);
    /// <summary>
    /// Crea un resultado fallido con los errores y el diccionario de detalles indicados.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Diccionario con información adicional del error.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => new(errors, details);
    /// <summary>
    /// Crea un resultado fallido con el error y el diccionario de detalles indicados.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Diccionario con información adicional del error.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(MlError              error , Dictionary<string, object> details    ) => new(error, details);
    /// <summary>
    /// Crea un resultado fallido con los errores y un único par clave/valor de detalle.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="detailsKey">Clave del detalle adicional.</param>
    /// <param name="detailsValue">Valor del detalle adicional.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => new(errors, new Dictionary<string, object> { { detailsKey , detailsValue  } });
    /// <summary>
    /// Crea un resultado fallido con el error y un único par clave/valor de detalle.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="detailsKey">Clave del detalle adicional.</param>
    /// <param name="detailsValue">Valor del detalle adicional.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(MlError              error , string detailsKey, object detailsValue) => new(error , new Dictionary<string, object> { { detailsKey , detailsValue  } });
    /// <summary>
    /// Crea un resultado fallido con los errores y una tupla con el detalle adicional.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Tupla con la clave y el valor del detalle adicional.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(IEnumerable<MlError> errors, (string key, object value) details    ) => new(errors, new Dictionary<string, object> { { details.key, details.value } });
    /// <summary>
    /// Crea un resultado fallido con el error y una tupla con el detalle adicional.
    /// </summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Tupla con la clave y el valor del detalle adicional.</param>
    /// <returns>Resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail<T>(MlError              error , (string key, object value) details    ) => new(error , new Dictionary<string, object> { { details.key, details.value } });



    /// <summary>
    /// Crea un resultado válido vacío, útil como punto de partida de una cadena funcional.
    /// </summary>
    /// <returns>Resultado válido que transporta un valor <see cref="object"/> sin significado.</returns>
    public static MlResult<object> Empty() => new object();

    /// <summary>
    /// Crea de forma asíncrona un resultado válido vacío, útil como punto de partida de una cadena funcional.
    /// </summary>
    /// <returns>
    /// Tarea con el resultado válido que transporta un valor <see cref="object"/> sin significado.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<object>> EmptyAsync() => Empty().ToAsync();


}




/// <summary>
/// Resultado de una operación que puede ser válido, y entonces transporta un valor de tipo <typeparamref name="T"/>, o fallido, y entonces transporta los detalles del error. Es el tipo central de la biblioteca.
/// </summary>
/// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
public partial record MlResult<T>
{
    /// <summary>
    /// Obtiene o establece el valor de <c>SuccessValue</c>.
    /// </summary>
    internal protected T               SuccessValue         { get; init; }
    /// <summary>
    /// Obtiene o establece el valor de <c>FailureDetails</c>.
    /// </summary>
    internal protected MlErrorDetails FailureDetails { get; init; }

    /// <summary>
    /// Indica si success.
    /// </summary>
    public bool IsSuccess { get; init; }
    /// <summary>
    /// Indica si failure.
    /// </summary>
    public bool IsFailure => ! IsSuccess;


    /// <summary>
    /// Obtiene el valor de <c>_</c>.
    /// </summary>
    public static MlResult<T> _  => new(default(T)!);
    /// <summary>
    /// Obtiene el valor de <c>Discard</c>.
    /// </summary>
    public static MlResult<T> Discard => _ ;


    internal MlResult(T                    t                                                        ) => (SuccessValue, FailureDetails, IsSuccess) = (t       , new MlErrorDetails()                                    , true );
    internal MlResult(MlErrorDetails                                         errorsDetails         ) => (SuccessValue, FailureDetails, IsSuccess) = (default!, errorsDetails                                            , false);
    internal MlResult(IEnumerable<MlError> errors, Dictionary<string, object> details        = null!) => (SuccessValue, FailureDetails, IsSuccess) = (default!, new MlErrorDetails(errors, details)                     , false);
    internal MlResult(MlError              error , Dictionary<string, object> details        = null!) => (SuccessValue, FailureDetails, IsSuccess) = (default!, new MlErrorDetails(new List<MlError> { error }, details), false);



    /// <summary>
    /// Ejecuta <c>Valid</c>.
    /// </summary>
    /// <param name="t">Valor de <c>t</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Valid(T t) => new(t);

    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(params MlError[]            errors       ) => new(errors);
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(       IEnumerable<MlError> errors       ) => new(errors);
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(       MlErrorDetails      errorsDetails) => new(errorsDetails);
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(       MlError              error        ) => new(error);

    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => new(errors, details);
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(MlError              error , Dictionary<string, object> details    ) => new(error , details);
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="detailsKey">Clave identificativa.</param>
    /// <param name="detailsValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => new(errors, new Dictionary<string, object> { { detailsKey, detailsValue } });
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="detailsKey">Clave identificativa.</param>
    /// <param name="detailsValue">Valor sobre el que se opera.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(MlError              error , string detailsKey, object detailsValue) => new(error , new Dictionary<string, object> { { detailsKey, detailsValue } });
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(IEnumerable<MlError> errors, (string key, object value) details    ) => new(errors, new Dictionary<string, object> { { details.key, details.value } });
    /// <summary>
    /// Ejecuta <c>Fail</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static MlResult<T> Fail(MlError              error , (string key, object value) details    ) => new(error , new Dictionary<string, object> { { details.key, details.value } });


    /// <summary>
    /// Ejecuta <c>Valid</c>. Versión asíncrona.
    /// </summary>
    /// <param name="t">Valor de <c>t</c>.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> ValidAsync(T t) => MlResult<T>.Valid(t).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(params MlError[]            errors       ) => MlResult<T>.Fail(errors       ).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(       IEnumerable<MlError> errors       ) => MlResult<T>.Fail(errors       ).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(       MlErrorDetails      errorsDetails) => MlResult<T>.Fail(errorsDetails).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(       MlError              error        ) => MlResult<T>.Fail(error        ).ToAsync();

    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, Dictionary<string, object> details    ) => MlResult<T>.Fail(errors, details).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(MlError              error , Dictionary<string, object> details    ) => MlResult<T>.Fail(error , details).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="detailsKey">Clave identificativa.</param>
    /// <param name="detailsValue">Valor sobre el que se opera.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, string detailsKey, object detailsValue) => MlResult<T>.Fail(errors, new Dictionary<string, object> { { detailsKey, detailsValue } }  ).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="detailsKey">Clave identificativa.</param>
    /// <param name="detailsValue">Valor sobre el que se opera.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(MlError              error , string detailsKey, object detailsValue) => MlResult<T>.Fail(error , new Dictionary<string, object> { { detailsKey, detailsValue } }  ).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(IEnumerable<MlError> errors, (string key, object value) details    ) => MlResult<T>.Fail(errors, new Dictionary<string, object> { { details.key, details.value } }).ToAsync();
    /// <summary>
    /// Ejecuta <c>Fail</c>. Versión asíncrona.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <param name="details">Información adicional asociada.</param>
    /// <returns>
    /// Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.
    /// <para>
    /// La tarea devuelta <b>ya está completada</b>: este método no realiza ninguna operación
    /// asíncrona. El sufijo <c>Async</c> existe para permitir iniciar una cadena que continúa con
    /// combinadores asíncronos. No admite cancelación.
    /// </para>
    /// </returns>
    public static Task<MlResult<T>> FailAsync(MlError              error , (string key, object value) details    ) => MlResult<T>.Fail(error , new Dictionary<string, object> { { details.key, details.value } }).ToAsync();



    

    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="t">Valor de <c>t</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>(T t) => Valid(t);

    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="errorsDetails">Detalles del error, con su mensaje y metadatos asociados.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>(MlErrorDetails errorsDetails) => Fail(errorsDetails);

    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>(List<MlError> errors) => Fail(errors.AsEnumerable());
    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="errors">Colección de errores que describen el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>(MlError[]     errors) => Fail(errors);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="error">Error que describe el fallo.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>(MlError       error ) => Fail(error);


    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>((IEnumerable<MlError> errors, Dictionary<string, object> details)     parameters) => Fail(parameters.errors, parameters.details);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>((MlError              error , Dictionary<string, object> details)     parameters) => Fail(parameters.error , parameters.details);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>((IEnumerable<MlError> errors, string detailsKey, object detailsValue) parameters) => Fail(parameters.errors, parameters.detailsKey, parameters.detailsValue);
    /// <summary>
    /// Realiza la conversión implícita a <c>MlResult&lt;T&gt;</c>.
    /// </summary>
    /// <param name="parameters">Valor de <c>parameters</c>.</param>
    /// <returns>Resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public static implicit operator MlResult<T>((MlError              error , string detailsKey, object detailsValue) parameters) => Fail(parameters.error , parameters.detailsKey, parameters.detailsValue);


    /// <inheritdoc/>
    public override string ToString() => this.Match(
                                                        fail : errors => errors.ToString(),
                                                        valid: value  => value?.ToString() ?? EnsureFpMessages.NoValueRepresentation
                                                   );





}
