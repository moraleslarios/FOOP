// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebControllers.Controllers;

[ApiController]
public class SimpleMlControllerBase<TEntity, TDto, TPk>(IGenServiceFp<TEntity, TDto> _genServiceFp) : ControllerBase
    where TEntity : class
    where TDto    : class
{

    [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.AllAsync(ct: ct)
                                        .ToGetPdActionResultAsync();
        return result;
    }


    [HttpGet("id-str/{id}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")

                                    .MatchAsync(
                                                    fail      : _     => MlActionResults.NotFound(detail: $"Path {id} not exists or is diferent type to PK of '{typeof(TPk).Name}' was not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemsDetails.NotFoundError(), 
                                                                                                                    ct                  : ct, 
                                                                                                                    pk                  : idObj)
                                                                            .ToGetPdActionResultAsync()
                                                );
        return result;
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.CreateAsync(dto, ct: ct)
                                        .ToPostActionResultAsync();
        return result;
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> PutAsync(string id, [FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.UpdateProblemDetailsAsync(dto                 : dto,
                                                                                                  notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToRepoPutActionResultAsync(this);
        return result;
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToPutPdActionResultAsync();
        return result;
    }


    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.DeleteProblemDetailsAsync(notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToDeletePdActionResultAsync();
        return result;
    }

    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.DeleteAsync(dto, ct: ct)
                                        .ToDeletePdActionResultAsync();
        return result;
    }

}

