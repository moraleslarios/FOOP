// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;

[ApiController]
public class MlCrudControllerBase<TEntity, TDto, TPk>(ICrudServiceFp<TEntity, TDto> _genServiceFp) : ControllerBase
    where TEntity : class
    where TDto    : class
{

    [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.GetPagedAsync(ct: ct)
                                        .ToOkProblemDetailsResultAsync();
        return result;
    }


    [HttpGet("id-str/{id}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")

                                    .MatchAsync(
                                                    fail      : _     => MlResults.NotFound(detail: $"Path {id} not exists or is of a different type than PK of '{typeof(TPk).Name}' was not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(), 
                                                                                                                    ct                  : ct, 
                                                                                                                    pk                  : idObj)
                                                                            .ToOkProblemDetailsResultAsync()
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
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.UpdateAsync(dto                 : dto,
                                                                                                  notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToNoContentProblemDetailsResultAsync();
        return result;
    }


    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.DeleteProblemDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToDeletedProblemDetailsResultAsync();
        return result;
    }

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



[ApiController]
public class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp) : ControllerBase
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{

     [HttpGet]
    public virtual async Task<IActionResult> GetAllAsync(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.GetPagedAsync(ct: ct)
                                        .ToOkProblemDetailsResultAsync();
        return result;
    }


    [HttpGet("id-str/{id}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")

                                    .MatchAsync(
                                                    fail      : _     => MlResults.NotFound(detail: $"Path {id} not exists or is of a different type than PK of '{typeof(TPk).Name}' was not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdProblemsDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(), 
                                                                                                                    ct                  : ct, 
                                                                                                                    pk                  : idObj)
                                                                            .ToOkProblemDetailsResultAsync()
                                                );
        return result;
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.CreateAsync(dto, ct: ct)
                                        .ToPostActionResultAsync();
        return result;
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> PutAsync(string id, [FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.UpdateAsync(dtoRequest          : dto,
                                                                                                  notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToNoContentProblemDetailsResultAsync();
        return result;
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToNoContentProblemDetailsResultAsync();
        return result;
    }


    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteAsync(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} must not be null")
                                    .TryMapAsync( _    => id.ConvertToType(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.DeleteProblemDetailsAsync(notFoundErrorDetails: MlProblemDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToDeletedProblemDetailsResultAsync();
        return result;
    }

    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromBody] TRequest dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.DeleteAsync(dto, ct: ct)
                                        .ToDeletedProblemDetailsResultAsync();
        return result;
    }



}







