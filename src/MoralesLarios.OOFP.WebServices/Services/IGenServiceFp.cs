
namespace MoralesLarios.OOFP.WebServices.Services;

public interface IGenServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    Task<MlResult<IEnumerable<TDto>>> AllAsync(CancellationToken               ct                  = default!,
                                               string                          initialMessage      = null!,
                                               Func<IEnumerable<TDto>, string> validMessageBuilder = null!,
                                               Func<MlErrorsDetails, string>   failMessageBuilder  = null!);

    Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                        string                        initialMessage      = null!,
                                        Func<TDto, string>            validMessageBuilder = null!,
                                        Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                        params object[] pk);

    Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            validMessageBuilder = null!,
                                     Func<MlErrorsDetails, string> failMessageBuilder  = null!);

    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            validMessageBuilder = null!,
                                     Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            validMessageBuilder = null!,
                                     Func<MlErrorsDetails, string> failMessageBuilder  = null!);

    Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorsDetails, string> failMessageBuilder  = null!);
}