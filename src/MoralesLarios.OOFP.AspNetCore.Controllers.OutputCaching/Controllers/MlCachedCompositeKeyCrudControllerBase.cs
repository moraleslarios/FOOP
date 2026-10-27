using MoralesLarios.OOFP.AspNetCore.Controllers.Attributes;

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;

public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>(ICrudServiceFp<TEntity, TDto> _genServiceFp,
                                                               Func<TEntity, object[]>      _keySelector,
                                                               IOutputCacheStore            _outputCacheStore)
        : MlCompositeKeyCrudControllerBase<TEntity, TDto>(_genServiceFp, _keySelector)
     where TEntity : class
     where TDto    : class
{
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);


    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
        => await base.GetByIdAsync(ids, ct);


    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }




    public override async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                  [FromBody]    TDto              dto,
                                                                                CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(ids, dto, ct);
    }

    public override async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }


    public override async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(ids, ct);
    }


    public override async Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(dto, ct);
    }


        [HttpGet("clear-cache/now")]
    public virtual async Task EvictControllerCacheAsync(CancellationToken ct = default)
        => await _outputCacheStore.EvictByTagAsync(ControllerScopedOutputCachePolicy.GetControllerTag(HttpContext), ct);
}



/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/




public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                              Func<TEntity, object[]>                     _keySelector,
                                                                              IOutputCacheStore                           _outputCacheStore)
        : MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>(_genServiceFp, _keySelector)
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);


    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
        => await base.GetByIdAsync(ids, ct);


    public override async Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }




    public override async Task<IActionResult> PutAsync([FromRoute][CompositeKeyParameter] string            ids, 
                                                                  [FromBody]    TRequest          dto,
                                                                                CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(ids, dto, ct);
    }

    public override async Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }


    public override async Task<IActionResult> DeleteAsync([FromRoute][CompositeKeyParameter] string ids, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(ids, ct);
    }


    public override async Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(dto, ct);
    }


        [HttpGet("clear-cache/now")]
    public virtual async Task EvictControllerCacheAsync(CancellationToken ct = default)
        => await _outputCacheStore.EvictByTagAsync(ControllerScopedOutputCachePolicy.GetControllerTag(HttpContext), ct);
}