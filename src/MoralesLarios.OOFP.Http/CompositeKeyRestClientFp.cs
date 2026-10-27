namespace MoralesLarios.OOFP.Http;

public class CompositeKeyRestClientFp<TDto>(ILogger<CompositeKeyRestClientFp<TDto>> _logger,
                                      IRestClientFp<TDto>                _genClientFp) : ICompositeKeyRestClientFp<TDto>
{


    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!,
                                                                 CancellationToken          ct      = default)
        => _genClientFp.GetAllAsync(headers, ct);


    public virtual async Task<MlResult<TDto>> GetByIdAsync(       Dictionary<string, string> headers = null!,
                                                                  CancellationToken          ct      = default,
                                                           params object[]                   keyValues)
        => await NotEmptyString.FromString(FormatCompositeKey(keyValues))
                                     .BindAsync(pkValuesStr => _genClientFp.GetByIdAsync(pkValuesStr, headers, ct));


        public virtual async Task<MlResult<TDto>> GetByIdAsync(object[]                   keyValues,
                                                               Dictionary<string, string> headers = null!,
                                                               CancellationToken          ct      = default)
        => await GetByIdAsync(headers, ct, keyValues);


    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PostAsync(itemBody, headers, ct);


    public virtual Task<MlResult<EmptyValue>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PutAsync(itemBody, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(       TDto                       itemBody,
                                                             Dictionary<string, string> headers = null!, 
                                                             CancellationToken          ct      = default,
                                                      params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                         .BindAsync(pkValuesStr => _genClientFp.PutByIdAsync(pkValuesStr, itemBody, headers, ct));

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(object[]                   keyValues,
                                                      TDto                       itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => PutByIdAsync(itemBody, headers, ct, keyValues);

    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _genClientFp.DeleteAsync(itemBody, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(       Dictionary<string, string> headers = null!, 
                                                                CancellationToken          ct      = default,
                                                         params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                            .BindAsync(pkValuesStr => _genClientFp.DeleteByIdAsync(pkValuesStr, headers, ct));

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(object[]                   keyValues,
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => DeleteByIdAsync(headers, ct, keyValues);


    protected virtual string FormatCompositeKey(object[] pkValues)
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


/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/



public class CompositeKeyRestClientFp<TRequest, TResponse>(ILogger<CompositeKeyRestClientFp<TRequest, TResponse>> _logger,
                                                     IRestClientFp<TRequest, TResponse>                _genClientFp) : ICompositeKeyRestClientFp<TRequest, TResponse>
{


    public virtual Task<MlResult<IEnumerable<TResponse>>> GetAllAsync(Dictionary<string, string> headers = null!,
                                                                      CancellationToken          ct      = default)
        => _genClientFp.GetAllAsync(headers, ct);


    public virtual async Task<MlResult<TResponse>> GetByIdAsync(       Dictionary<string, string> headers = null!,
                                                                       CancellationToken          ct      = default,
                                                                params object[]                   keyValues)
        => await NotEmptyString.FromString(FormatCompositeKey(keyValues))
                                     .BindAsync(pkValuesStr => _genClientFp.GetByIdAsync(pkValuesStr, headers, ct));


        public virtual async Task<MlResult<TResponse>> GetByIdAsync(object[]                   keyValues,
                                                                    Dictionary<string, string> headers = null!,
                                                                    CancellationToken          ct      = default)
        => await GetByIdAsync(headers, ct, keyValues);


    public virtual Task<MlResult<TResponse>> PostAsync(TRequest                   itemBody, 
                                                       Dictionary<string, string> headers = null!, 
                                                       CancellationToken          ct      = default)
        => _genClientFp.PostAsync(itemBody, headers, ct);


    public virtual Task<MlResult<EmptyValue>> PutAsync(TRequest                   itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _genClientFp.PutAsync(itemBody, headers, ct);

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(       TRequest                   itemBody,
                                                             Dictionary<string, string> headers = null!, 
                                                             CancellationToken          ct      = default,
                                                      params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                         .BindAsync(pkValuesStr => _genClientFp.PutByIdAsync(pkValuesStr, itemBody, headers, ct));

    public virtual Task<MlResult<EmptyValue>> PutByIdAsync(object[]                   keyValues,
                                                      TRequest                   itemBody,
                                                      Dictionary<string, string> headers = null!, 
                                                      CancellationToken          ct      = default)
        => PutByIdAsync(itemBody, headers, ct, keyValues);

    public virtual Task<MlResult<EmptyValue>> DeleteAsync(TRequest                   itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _genClientFp.DeleteAsync(itemBody, headers, ct);

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(       Dictionary<string, string> headers = null!, 
                                                                CancellationToken          ct      = default,
                                                         params object[]                   keyValues)
        => NotEmptyString.FromString(FormatCompositeKey(keyValues))
                            .BindAsync(pkValuesStr => _genClientFp.DeleteByIdAsync(pkValuesStr, headers, ct));

    public virtual Task<MlResult<EmptyValue>> DeleteByIdAsync(object[]                   keyValues,
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => DeleteByIdAsync(headers, ct, keyValues);


    protected virtual string FormatCompositeKey(object[] pkValues)
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


