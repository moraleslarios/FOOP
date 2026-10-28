// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;

/// <summary>
/// Representa <c>MlCachedCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
/// <typeparam name="TPk">Tipo genérico TPk.</typeparam>
public abstract class MlCachedCrudControllerBase<TEntity, TDto, TPk>(ICrudServiceFp<TEntity, TDto> _genServiceFp,
                                                             IOutputCacheStore            _outputCacheStore)
        : MlCrudControllerBase<TEntity, TDto, TPk>(_genServiceFp)
    where TEntity : class
    where TDto    : class
{
    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);

    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default) => await base.GetByIdAsync(id, ct);


    /// <inheritdoc/>
    public override async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync(string id, [FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(id, dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(id, ct);
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
/// Representa <c>MlCachedCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
/// <typeparam name="TPk">Tipo genérico TPk.</typeparam>
public abstract class MlCachedCrudControllerBase<TEntity, TRequest, TResponse, TPk>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp,
                                                                            IOutputCacheStore                           _outputCacheStore)
        : MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>(_genServiceFp)
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{
    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetAllAsync(CancellationToken ct = default!) => await base.GetAllAsync(ct);

    /// <inheritdoc/>
    [MlControllerCache]
    public override async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default) => await base.GetByIdAsync(id, ct);


    /// <inheritdoc/>
    public override async Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PostAsync(dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync(string id, [FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(id, dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        await EvictControllerCacheAsync(ct);

        return await base.PutAsync(dto, ct);
    }

    /// <inheritdoc/>
    public override async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default)
    {
        await EvictControllerCacheAsync(ct);

        return await base.DeleteAsync(id, ct);
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


