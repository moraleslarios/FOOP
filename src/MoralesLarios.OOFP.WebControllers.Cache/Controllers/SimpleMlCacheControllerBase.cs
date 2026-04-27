// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using MoralesLarios.OOFP.WebControllers.Cache.Policies;
using MoralesLarios.OOFP.WebControllers.Controllers;

namespace MoralesLarios.OOFP.WebControllers.Cache.Controllers;

public class SimpleMlCacheControllerBase<TEntity, TDto, TPk>(IGenServiceFp<TEntity, TDto> _genServiceFp,
                                                             IOutputCacheStore            _outputCacheStore)
        : SimpleMlControllerBase<TEntity, TDto, TPk>(_genServiceFp)
    where TEntity : class
    where TDto    : class
{
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);

    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default) => await base.GetByIdAsync(id, ct);


    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }

    public override async Task<IActionResult> PutAsync(string id, [FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(id, dto, ct);
    }

    public override async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }

    public override async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(id, ct);
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



