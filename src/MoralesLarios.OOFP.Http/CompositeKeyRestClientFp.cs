namespace MoralesLarios.OOFP.Http;

/// <summary>
/// Representa <c>CompositeKeyRestClientFp</c>.
/// </summary>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
public class CompositeKeyRestClientFp<TDto>(ILogger<CompositeKeyRestClientFp<TDto>> _logger,
                                      IRestClientFp<TDto>                _genClientFp) : ICompositeKeyRestClientFp<TDto>
{


    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!,
                                                                 CancellationToken          ct      = default)
        => _genClientFp.GetAllAsync(headers, ct);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<TDto>> GetByIdAsync(       Dictionary<string, string> headers = null!,
                                                                  CancellationToken          ct      = default,
                                                           params object[]                   keyValues)
        => await NotEmptyString.FromString(FormatCompositeKey(keyValues))
                                     .BindAsync(pkValuesStr => _genClientFp.GetByIdAsync(pkValuesStr, headers, ct));


        /// <summary>
        /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
        /// </summary>
        /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
        /// <param name="headers">Cabeceras que acompañan a la petición.</param>
        /// <param name="ct">Token para cancelar la operación.</param>
        /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
        public virtual async Task<MlResult<TDto>> GetByIdAsync(object[]                   keyValues,
                                                               Dictionary<string, string> headers = null!,
                                                               CancellationToken          ct      = default)
        => await GetByIdAsync(headers, ct, keyValues);


    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PostAsync(itemBody, headers, ct);


    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PutAsync(itemBody, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(       TDto                       itemBody,
                                                             Dictionary<string, string> headers = null!, 
                                                             CancellationToken          ct      = default,
                                                      params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                         .BindAsync(pkValuesStr => _genClientFp.PutByIdAsync(pkValuesStr, itemBody, headers, ct));

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(object[]                   keyValues,
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => PutByIdAsync(itemBody, headers, ct, keyValues);

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _genClientFp.DeleteAsync(itemBody, headers, ct);

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(       Dictionary<string, string> headers = null!, 
                                                                CancellationToken          ct      = default,
                                                         params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                            .BindAsync(pkValuesStr => _genClientFp.DeleteByIdAsync(pkValuesStr, headers, ct));

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(object[]                   keyValues,
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => DeleteByIdAsync(headers, ct, keyValues);


    /// <summary>
    /// Ejecuta <c>FormatCompositeKey</c>.
    /// </summary>
    /// <param name="pkValues">Colección de <c>pkValues</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    protected virtual string FormatCompositeKey(object[] pkValues)
    {
        var result = string.Join(",", (pkValues ?? Array.Empty<object>()).Select(FormatCompositeKeyValue));

        return result;
    }

    /// <summary>
    /// Punto 28: todos los valores de una clave compuesta se formatean con
    /// <see cref="CultureInfo.InvariantCulture"/>. Con la cultura del hilo, un decimal en es-ES se
    /// serializaria como "3,14" y la coma romperia el separador de la propia clave compuesta.
    /// </summary>
    protected virtual string FormatCompositeKeyValue(object value)
        => value switch
        {
            null              => string.Empty,
            DateTime       dt => dt.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
            DateOnly       d  => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly       t  => t.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture),
            TimeSpan       ts => ts.ToString("c", CultureInfo.InvariantCulture),
            Guid           g  => g.ToString("D"),
            bool           b  => b ? "true" : "false",
            IFormattable   f  => f.ToString(null, CultureInfo.InvariantCulture),
            _                 => value.ToString() ?? string.Empty
        };

}


/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/



/// <summary>
/// Representa <c>CompositeKeyRestClientFp</c>.
/// </summary>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
public class CompositeKeyRestClientFp<TRequest, TResponse>(ILogger<CompositeKeyRestClientFp<TRequest, TResponse>> _logger,
                                                     IRestClientFp<TRequest, TResponse>                _genClientFp) : ICompositeKeyRestClientFp<TRequest, TResponse>
{


    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!,
                                                                      CancellationToken          ct      = default)
        => _genClientFp.GetAllAsync(headers, ct);


    /// <summary>
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual async Task<MlResult<TResponse>> GetByIdAsync(       Dictionary<string, string> headers = null!,
                                                                       CancellationToken          ct      = default,
                                                                params object[]                   keyValues)
        => await NotEmptyString.FromString(FormatCompositeKey(keyValues))
                                     .BindAsync(pkValuesStr => _genClientFp.GetByIdAsync(pkValuesStr, headers, ct));


        /// <summary>
        /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
        /// </summary>
        /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
        /// <param name="headers">Cabeceras que acompañan a la petición.</param>
        /// <param name="ct">Token para cancelar la operación.</param>
        /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
        public virtual async Task<MlResult<TResponse>> GetByIdAsync(object[]                   keyValues,
                                                                    Dictionary<string, string> headers = null!,
                                                                    CancellationToken          ct      = default)
        => await GetByIdAsync(headers, ct, keyValues);


    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<TResponse>> PostAsync(TRequest                   itemBody, 
                                                       Dictionary<string, string> headers = null!, 
                                                       CancellationToken          ct      = default)
        => _genClientFp.PostAsync(itemBody, headers, ct);


    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutAsync(TRequest                   itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PutAsync(itemBody, headers, ct);

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(       TRequest                   itemBody,
                                                             Dictionary<string, string> headers = null!, 
                                                             CancellationToken          ct      = default,
                                                      params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                         .BindAsync(pkValuesStr => _genClientFp.PutByIdAsync(pkValuesStr, itemBody, headers, ct));

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(object[]                   keyValues,
                                                      TRequest                   itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => PutByIdAsync(itemBody, headers, ct, keyValues);

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="itemBody">Valor de <c>itemBody</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TRequest                   itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _genClientFp.DeleteAsync(itemBody, headers, ct);

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(       Dictionary<string, string> headers = null!, 
                                                                CancellationToken          ct      = default,
                                                         params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                            .BindAsync(pkValuesStr => _genClientFp.DeleteByIdAsync(pkValuesStr, headers, ct));

    /// <summary>
    /// Elimina el elemento indicado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="keyValues">Colección de <c>keyValues</c>.</param>
    /// <param name="headers">Cabeceras que acompañan a la petición.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el resultado válido con el valor obtenido, o resultado fallido con los detalles del error.</returns>
    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(object[]                   keyValues,
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => DeleteByIdAsync(headers, ct, keyValues);


    /// <summary>
    /// Ejecuta <c>FormatCompositeKey</c>.
    /// </summary>
    /// <param name="pkValues">Colección de <c>pkValues</c>.</param>
    /// <returns>Representación textual obtenida.</returns>
    protected virtual string FormatCompositeKey(object[] pkValues)
    {
        var result = string.Join(",", (pkValues ?? Array.Empty<object>()).Select(FormatCompositeKeyValue));

        return result;
    }

    /// <summary>
    /// Punto 28: todos los valores de una clave compuesta se formatean con
    /// <see cref="CultureInfo.InvariantCulture"/>. Con la cultura del hilo, un decimal en es-ES se
    /// serializaria como "3,14" y la coma romperia el separador de la propia clave compuesta.
    /// </summary>
    protected virtual string FormatCompositeKeyValue(object value)
        => value switch
        {
            null              => string.Empty,
            DateTime       dt => dt.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture),
            DateOnly       d  => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly       t  => t.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture),
            TimeSpan       ts => ts.ToString("c", CultureInfo.InvariantCulture),
            Guid           g  => g.ToString("D"),
            bool           b  => b ? "true" : "false",
            IFormattable   f  => f.ToString(null, CultureInfo.InvariantCulture),
            _                 => value.ToString() ?? string.Empty
        };

}


