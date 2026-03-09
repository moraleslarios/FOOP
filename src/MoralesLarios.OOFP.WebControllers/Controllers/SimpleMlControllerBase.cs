namespace MoralesLarios.OOFP.WebControllers.Controllers;

[ApiController]
public class SimpleMlControllerBase<TEntity, TDto, TPk>(IGenServiceFp<TEntity, TDto> _genServiceFp) : ControllerBase
    where TEntity : class
    where TDto    : class
{

    [HttpGet]
    public virtual async Task<IActionResult> GetAll(CancellationToken ct = default!)
    {
        var result = await _genServiceFp.AllAsync(ct: ct)
                                        .ToRepoGetActionResultAsync(this);
        return result;
    }


    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetById(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")

                                    .MatchAsync(
                                                    fail: _ => MlActionResults.NotFound(detail: $"Path {id} not exists or is diferent type to PK of '{typeof(TPk).Name}' was not found."),
                                                    validAsync: idObj => _genServiceFp.FindByIdAsync(ct: ct, pk: idObj)
                                                                            .ToRepoGetActionResultAsync(this)
                                                );


                                    //.BindAsync  (idObj => _genServiceFp.FindByIdAsync(ct: ct, pk: idObj))
                                    //.ToRepoGetActionResultAsync(this);
        
        //var data = new ObjectResult(new ProblemDetails
        //{
        //    Status = StatusCodes.Status404NotFound,
        //    Title = "Resource not found",
        //    Detail = $"Entity with id '{id}' was not found.",
        //    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        //});
            
        return result;
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.CreateAsync(dto, ct: ct)
                                        .ToSimpleRepoPostActionResultAsync(this);
        return result;
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(string id, [FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.UpdateAsync(dto, ct: ct, pk: idObj))
                                    .ToRepoPutActionResultAsync(this);
        return result;
    }

    [HttpPut]
    public virtual async Task<IActionResult> Put([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToRepoPutActionResultAsync(this);
        return result;
    }


    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.DeleteAsync(ct: ct, pk: idObj))
                                    .ToRepoDeleteActionResultAsync(this);
        return result;
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.DeleteAsync(dto, ct: ct)
                                        .ToRepoDeleteActionResultAsync(this);
        return result;
    }

}
