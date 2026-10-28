// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;

/// <summary>
/// Representa <c>MlCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TDto">Tipo del objeto de transferencia de datos.</typeparam>
/// <typeparam name="TPk">Tipo genérico TPk.</typeparam>
[ApiController]
public abstract class MlCrudControllerBase<TEntity, TDto, TPk>(ICrudServiceFp<TEntity, TDto> _genServiceFp) : ControllerBase
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
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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
/// Representa <c>MlCrudControllerBase</c>.
/// </summary>
/// <typeparam name="TEntity">Tipo de la entidad de datos.</typeparam>
/// <typeparam name="TRequest">Tipo de la petición.</typeparam>
/// <typeparam name="TResponse">Tipo de la respuesta.</typeparam>
/// <typeparam name="TPk">Tipo genérico TPk.</typeparam>
[ApiController]
public abstract class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>(ICrudServiceFp<TEntity, TRequest, TResponse> _genServiceFp) : ControllerBase
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
    /// Obtiene el elemento solicitado de <c>ById</c>. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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
    /// Envia la petición mediante el verbo HTTP PUT. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="dto">Objeto de transferencia de datos.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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
    /// Elimina el elemento indicado. Versión asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <param name="ct">Token para cancelar la operación.</param>
    /// <returns>Tarea que produce el valor de tipo <c>IActionResult</c> obtenido por la operación.</returns>
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







