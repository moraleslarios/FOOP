namespace MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;

/// <summary>
/// Representa <c>MlCompositeKeyCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
[ApiController]
public abstract class MlCompositeKeyCrudControllerBase<TEntity, TDto>(ICrudServiceFp<TEntity, TDto> _genServiceFp,
                                                            Func<TEntity, object[]>      _keySelector) : ControllerBase
        where TEntity : class
        where TDto    : class
{


    /// <summary>
    /// Obtiene todos los elementos. Versión asíncrona.
    /// </summary>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.GetPagedAsync(ct: ct)
                                        .ToOkProblemDetailsResultAsync();
        return result;
    }



    /// <summary>
    /// Obtiene una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas en formato específico según el tipo.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad encontrada o 404 si no existe.</returns>
    /// <remarks>
    /// **Formato de valores:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// - Para strings: valores literales separados por comas. Ej: `value1,value2`
    /// </remarks>
    [HttpGet("id-str/{ids}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _ => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .MatchAsync(
                                                    fail       : _    => MlResults.NotFound(detail: $"Row with ids: {ids} not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                                    ct                  : ct,
                                                                                                                    pk                  : idObj)
                                                                            .ToOkProblemDetailsResultAsync()
                                                );
        return result;
    }


    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.CreateAsync(dto, ct: ct)
                                        .ToPostActionResultAsync();
        return result;
    }

    /// <summary>
    /// Actualiza una entidad existente por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="dto">Los datos actualizados de la entidad.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad actualizada o errores si falla.</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpPut("{ids}")]
    public virtual async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                 [FromBody]    TDto              dto,
                                                                               CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _    => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .BindAsync  (idObj => _genServiceFp.UpdateAsync(dto                 : dto,
                                                                                                  notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Elimina una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Resultado de la eliminación (200 OK o error).</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpDelete("{ids}")]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _    => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .BindAsync  (idObj => _genServiceFp.DeleteProblemDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToDeletedProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.DeleteAsync(dto, ct: ct)
                                        .ToDeletedProblemDetailsResultAsync();
        return result;
    }


}



/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/




/// <summary>
/// Representa <c>MlCompositeKeyCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
[ApiController]
public abstract class MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                           Func<TEntity, object[]>                     _keySelector) : ControllerBase
        where TEntity   : class
        where TRequest  : class
        where TResponse : class
{

        /// <summary>
        /// Obtiene todos los elementos. Versión asíncrona.
        /// </summary>
        /// <param name="ct">Token para cancelar la operación.</param>
        /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
        [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.GetPagedAsync(ct: ct)
                                        .ToOkProblemDetailsResultAsync();
        return result;
    }



    /// <summary>
    /// Obtiene una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas en formato específico según el tipo.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad encontrada o 404 si no existe.</returns>
    /// <remarks>
    /// **Formato de valores:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// - Para strings: valores literales separados por comas. Ej: `value1,value2`
    /// </remarks>
    [HttpGet("id-str/{ids}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _ => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .MatchAsync(
                                                    fail       : _    => MlResults.NotFound(detail: $"Row with ids: {ids} not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                                    ct                  : ct,
                                                                                                                    pk                  : idObj)
                                                                            .ToOkProblemDetailsResultAsync()
                                                );
        return result;
    }


    /// <summary>
    /// Envia la petición mediante el verbo HTTP POST. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.CreateAsync(dto, ct: ct)
                                        .ToPostActionResultAsync();
        return result;
    }

    /// <summary>
    /// Actualiza una entidad existente por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="dto">Los datos actualizados de la entidad.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad actualizada o errores si falla.</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpPut("{ids}")]
    public virtual async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                 [FromBody]    TRequest          dto,
                                                                               CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _    => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .BindAsync  (idObj => _genServiceFp.UpdateAsync(dtoRequest          : dto,
                                                                                                  notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Elimina una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Resultado de la eliminación (200 OK o error).</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpDelete("{ids}")]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} must not be null")
                                    .TryMapAsync( _    => ids.ParseKeyValues(_keySelector), ex => ex.BuildKeyParseErrorMessage(ids))
                                    .BindAsync  (idObj => _genServiceFp.DeleteProblemDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToDeletedProblemDetailsResultAsync();
        return result;
    }

    /// <summary>
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.DeleteAsync(dto, ct: ct)
                                        .ToDeletedProblemDetailsResultAsync();
        return result;
    }


}


