using MoralesLarios.OOFP.WebControllers.Attributes;

namespace MoralesLarios.OOFP.WebControllers.Cache.Controllers;

public class SimpleMlComplexCacheControllerBase<TEntity, TDto>(IGenServiceFp<TEntity, TDto> _genServiceFp,
                                                               Func<TEntity, object[]>      _pkFields,
                                                               IOutputCacheStore            _outputCacheStore)
        : SimpleMlComplexPkControllerBase<TEntity, TDto>(_genServiceFp, _pkFields)
     where TEntity : class
     where TDto    : class
{
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);


    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!)
        => await base.GetByIdAsync(ids, ct);


    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }




    public override async Task<IActionResult> PutAsync([FromRoute][PkParameter] string            ids, 
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


    public override async Task<IActionResult> DeleteAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!)
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
        => await _outputCacheStore.EvictByTagAsync(PerControllerOutputCachePolicy.GetControllerTag(HttpContext), ct);
}
