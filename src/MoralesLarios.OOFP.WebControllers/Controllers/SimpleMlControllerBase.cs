namespace MoralesLarios.OOFP.WebControllers.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public virtual async Task<IActionResult> GetAll(string id, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(id, $"{nameof(id)} isn't null")
                                    .TryMapAsync( _    => id.ConverterTo(typeof(TPk)), ex => $"{nameof(id)} can't be converted to {typeof(TPk).Name}. ex: {ex.Message}")
                                    .BindAsync  (idObj => _genServiceFp.FindByIdAsync(ct: ct, pk: idObj))
                                    .ToRepoGetActionResultAsync(this);
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
