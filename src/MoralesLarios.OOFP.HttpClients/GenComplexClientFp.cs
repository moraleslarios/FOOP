namespace MoralesLarios.OOFP.HttpClients;

public class GenComplexClientFp<TDto>(ILogger<GenComplexClientFp<TDto>> _logger,
                                      IGenClientFp<TDto>                _genClientFp) : IGenComplexClientFp<TDto>
{


    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!,
                                                                 CancellationToken          ct      = default)
        => _genClientFp.GetAllAsync(headers, ct);


    public virtual async Task<MlResult<TDto>> GetByIdAsync(       Dictionary<string, string> headers = null!,
                                                                  CancellationToken          ct      = default,
                                                           params object[]                   pk)
        => await NotEmptyString.ByString(GetPkValuesString(pk))
                                     .BindAsync(pkValuesStr => _genClientFp.GetByIdAsync(pkValuesStr, headers, ct));


        public virtual async Task<MlResult<TDto>> GetByIdAsync(object[]                   pk,
                                                               Dictionary<string, string> headers = null!,
                                                               CancellationToken          ct      = default)
        => await GetByIdAsync(headers, ct, pk);


    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PostAsync(itemBody, headers, ct);


    public virtual Task<MlResult<Empty>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PutAsync(itemBody, headers, ct);

    public virtual Task<MlResult<Empty>> PutByIdAsync(       TDto                       itemBody,
                                                             Dictionary<string, string> headers = null!, 
                                                             CancellationToken          ct      = default,
                                                      params object[]                   pk)
        => NotEmptyString.ByString(GetPkValuesString(pk))
                         .BindAsync(pkValuesStr => _genClientFp.PutByIdAsync(pkValuesStr, itemBody, headers, ct));

    public virtual Task<MlResult<Empty>> PutByIdAsync(object[]                   pk,
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => PutByIdAsync(itemBody, headers, ct, pk);

    public virtual Task<MlResult<Empty>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _genClientFp.DeleteAsync(itemBody, headers, ct);

    public virtual Task<MlResult<Empty>> DeleteByIdAsync(       Dictionary<string, string> headers = null!, 
                                                                CancellationToken          ct      = default,
                                                         params object[]                   pk)
        => NotEmptyString.ByString(GetPkValuesString(pk))
                            .BindAsync(pkValuesStr => _genClientFp.DeleteByIdAsync(pkValuesStr, headers, ct));

    public virtual Task<MlResult<Empty>> DeleteByIdAsync(object[]                   pk,
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => DeleteByIdAsync(headers, ct, pk);


    protected virtual string GetPkValuesString(object[] pkValues)
    {
        return string.Join(",", pkValues.Select(v => v switch
        {
            DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ss.fff"),
            DateOnly d  => d.ToString("yyyy-MM-dd"),
            TimeOnly t  => t.ToString("HH:mm:ss.fff"),
            _           => v.ToString()
        }));
    }

}
