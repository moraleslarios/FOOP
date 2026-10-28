using MoralesLarios.OOFP.AspNetCore.Controllers.Attributes;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;

/// <summary>
/// Representa <c>MlCachedCompositeKeyCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>(ICrudServiceFp<TEntity, TDto> _genServiceFp,
                                                               Func<TEntity, object[]>      _keySelector,
                                                               IOutputCacheStore            _outputCacheStore)
        : MlCompositeKeyCrudControllerBase<TEntity, TDto>(_genServiceFp, _keySelector)
     where TEntity : class
     where TDto    : class
{
    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);


    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
        => await base.GetByIdAsync(ids, ct);


    /// <inheritdoc/>
    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }




    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                  [FromBody]    TDto              dto,
                                                                                CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(ids, dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(ids, ct);
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(dto, ct);
    }


        /// <summary>
        /// Ejecuta <c>EvictControllerCache</c>. Versión asíncrona.
        /// </summary>
        /// <param name="ct">Token para cancelar la operación.</param>
        /// <returns>Tarea que representa la operación asíncrona.</returns>
        [HttpGet("clear-cache/now")]
    public virtual async Task EvictControllerCacheAsync(CancellationToken ct = default)
        => await _outputCacheStore.EvictByTagAsync(ControllerScopedOutputCachePolicy.GetControllerTag(HttpContext), ct);
}



/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/




/// <summary>
/// Representa <c>MlCachedCompositeKeyCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                              Func<TEntity, object[]>                     _keySelector,
                                                                              IOutputCacheStore                           _outputCacheStore)
        : MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>(_genServiceFp, _keySelector)
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{
    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);


    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
        => await base.GetByIdAsync(ids, ct);


    /// <inheritdoc/>
    public override async Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }




    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                  [FromBody]    TRequest          dto,
                                                                                CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(ids, dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(ids, ct);
    }


    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(dto, ct);
    }


        /// <summary>
        /// Ejecuta <c>EvictControllerCache</c>. Versión asíncrona.
        /// </summary>
        /// <param name="ct">Token para cancelar la operación.</param>
        /// <returns>Tarea que representa la operación asíncrona.</returns>
        [HttpGet("clear-cache/now")]
    public virtual async Task EvictControllerCacheAsync(CancellationToken ct = default)
        => await _outputCacheStore.EvictByTagAsync(ControllerScopedOutputCachePolicy.GetControllerTag(HttpContext), ct);
}