namespace MoralesLarios.OOFP.HttpClients;

public class GenClientFp<TDto>(ILogger<GenClientFp<TDto>> _logger,
                               IHttpClientFactoryManager  _httpClientFactoryManager,
                               Key                        _httpClientFactoryKey) : IGenClientFp<TDto>
{ 

    public virtual Task<MlResult<IEnumerable<TDto>>> GetAllAsync(Dictionary<string, string> headers = null!, 
                                                                 CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<IEnumerable<TDto>>(_httpClientFactoryKey, string.Empty, headers, ct);

    public virtual Task<MlResult<TDto>> GetByIdAsync(NotEmptyString             idStr,
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.GetAsync<TDto>(_httpClientFactoryKey, $"id-str/{idStr}", headers, ct);

    public virtual Task<MlResult<TDto>> PostAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _httpClientFactoryManager.PostAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<Empty>> PutAsync(TDto                       itemBody, 
                                                  Dictionary<string, string> headers = null!, 
                                                  CancellationToken          ct      = default)
        => _httpClientFactoryManager.PutAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);


    public virtual Task<MlResult<Empty>> DeleteAsync(TDto                       itemBody, 
                                                     Dictionary<string, string> headers = null!, 
                                                     CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteAsync(_httpClientFactoryKey, itemBody, string.Empty, headers, ct);

    public virtual Task<MlResult<Empty>> DeleteByIdAsync(NotEmptyString             idStr, 
                                                         Dictionary<string, string> headers = null!, 
                                                         CancellationToken          ct      = default)
        => _httpClientFactoryManager.DeleteByIdAsync<TDto>(_httpClientFactoryKey, $"{idStr}", headers, ct);



}
