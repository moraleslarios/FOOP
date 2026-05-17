namespace MoralesLarios.OOFP.WebControllers.Controllers;

[ApiController]
public class SimpleMlComplexPkControllerBase<TEntity, TDto>(IGenServiceFp<TEntity, TDto> _genServiceFp,
                                                            Func<TEntity, object[]>      _pkFields) : ControllerBase
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



    /// <summary>
    /// Obtiene una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas en formato específico según el tipo.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad encontrada o 404 si no existe.</returns>
    /// <remarks>
    /// **Formato de valores:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// - Para strings: valores literales separados por comas. Ej: `value1,value2`
    /// </remarks>
    [HttpGet("id-str/{ids}", Name = $"[controller]_[action]")]
    public virtual async Task<IActionResult> GetByIdAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} isn't null")
                                    .TryMapAsync( _ => InternalGetPkValues(ids), ex => InternalGetPkValuesErrorMessage(ex, ids))
                                    .MatchAsync(
                                                    fail       : _    => MlActionResults.NotFound(detail: $"Row with ids: {ids} not found."),
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

    /// <summary>
    /// Actualiza una entidad existente por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="dto">Los datos actualizados de la entidad.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>La entidad actualizada o errores si falla.</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpPut("{ids}")]
    public virtual async Task<IActionResult> PutAsync([FromRoute][PkParameter] string            ids, 
                                                                 [FromBody]    TDto              dto,
                                                                               CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} isn't null")
                                    .TryMapAsync( _    => InternalGetPkValues(ids), ex => InternalGetPkValuesErrorMessage(ex, ids))
                                    .BindAsync  (idObj => _genServiceFp.UpdateProblemDetailsAsync(dto                 : dto,
                                                                                                  notFoundErrorDetails: MlProblemsDetails.NotFoundError(),
                                                                                                  ct                  : ct, 
                                                                                                  pk                  : idObj))
                                    .ToPutPdActionResultAsync();
        return result;
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromBody] TDto dto, CancellationToken ct = default!)
    {
        var result = await _genServiceFp.UpdateAsync(dto, ct: ct)
                                        .ToPutPdActionResultAsync();
        return result;
    }

    /// <summary>
    /// Elimina una entidad por su clave primaria.
    /// </summary>
    /// <param name="ids">Valores de la clave primaria separados por comas.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Resultado de la eliminación (200 OK o error).</returns>
    /// <remarks>
    /// **Formato de valores en ids:**
    /// - Para claves compuestas numéricas: separadas por comas. Ej: `1,2,3`
    /// - Para DateTime: usar ISO 8601 `yyyy-MM-ddTHH:mm:ss.fff`. Ej: `2026-05-16T07:34:29.239`
    /// </remarks>
    [HttpDelete("{ids}")]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][PkParameter] string ids, CancellationToken ct = default!)
    {
        var result = await EnsureFp.NotNullAsync(ids, $"{nameof(ids)} isn't null")
                                    .TryMapAsync( _    => InternalGetPkValues(ids), ex => InternalGetPkValuesErrorMessage(ex, ids))
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




    private object[] InternalGetPkValues(string ids) => GetPkValues(ids.Split(','), _pkFields);
    private string InternalGetPkValuesErrorMessage(Exception ex, string ids) => $"{ids} not be extract values of pkFields. Ids string array not converted to pkFields. ex: {ex.Message}";



    protected object[] GetPkValues(string[] values, Func<TEntity, object[]> pkFields)
    {
        var sample = Activator.CreateInstance<TEntity>();   // requiere new() o reflexión
        var sampleP = pkFields(sample);                       // todos serán default/null
        if (values.Length != sampleP.Length) throw new ArgumentException($"The number of provided values ({values.Length}) does not match the number of primary key fields ({sampleP.Length}).", nameof(values));

        // ⚠️ Esto sólo funciona si los miembros devueltos son value types con default no-null.
        // Si la PK es int / long / Guid → ok. Si es string → sampleP[i] será null y NO sabrás el tipo.
        var result = new object[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var t = sampleP[i]?.GetType() ?? typeof(string);
            result[i] = Convert.ChangeType(values[i], t, CultureInfo.InvariantCulture);
        }
        return result;
    }

}


