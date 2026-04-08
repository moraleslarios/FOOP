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
    [OutputCache(PolicyName = "PerControllerTag")]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);

    [OutputCache(PolicyName = "PerControllerTag")]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default) => await base.GetByIdAsync(id, ct);


    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default)
    {
        await _outputCacheStore.EvictByTagAsync(PerControllerOutputCachePolicy.GetControllerTag(HttpContext), ct);

        return await base.PostAsync(dto, ct);
    }
}
