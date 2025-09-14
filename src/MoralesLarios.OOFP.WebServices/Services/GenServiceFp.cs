namespace MoralesLarios.OOFP.WebServices.Services;

public class GenServiceFp<TEntity, TDto>(IEFRepoFp<TEntity>                             _repo,
                                                   ILogger<GenServiceFp<TEntity, TDto>> _logger) : IGenServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    public Task<MlResult<IEnumerable<TDto>>> AllAsync(CancellationToken               ct                  = default!,
                                                      string                          initialMessage      = null!,
                                                      Func<IEnumerable<TDto>, string> validMessageBuilder = null!,
                                                      Func<MlErrorsDetails, string>   failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync( _     => _repo.TryAllAsync(ct))
                            .MapAsync (bdData => bdData.Adapt<IEnumerable<TDto>>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"Found {x.Count()} of table {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null  ? $"An error occurred while querying the {typeof(TDto).Name} table. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }

    public Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                               string                        initialMessage      = null!,
                                               Func<TDto, string>            validMessageBuilder = null!,
                                               Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                               params object[] pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying data from the {typeof(TDto).Name} table by Id ({pk.GetPkValues()})")
                            .BindAsync( _     => _repo.TryFindAsync(notFoundErrorDetails: BuildNotFoundPkError(tableName: typeof(TDto).Name, pk: pk),
                                                                    token               : ct,
                                                                    pk                  : pk))
                            .MapAsync (vinoBd => vinoBd?.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => x is not null
                                                                                    ? (validMessageBuilder is null
                                                                                            ? $"The query to the {typeof(TDto).Name} table for the pk {pk.GetPkValues()} completed successfully"
                                                                                            : validMessageBuilder(x))
                                                                                    : $"No data found in the {typeof(TDto).Name} table for the pk {pk.GetPkValues()}",
                                                   failBuildMessage : errors => failMessageBuilder is null
                                                                                    ? $"An error occurred while querying the {typeof(TDto).Name} table for the pk {pk.GetPkValues()}. Error: {errors.ToString()}"
                                                                                    : failMessageBuilder(errors));
        return result;
    }


    public Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                            CancellationToken             ct                  = default!,
                                            string                        initialMessage      = null!,
                                            Func<TDto, string>            validMessageBuilder = null!,
                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Creating a new record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was created successfully in the table corresponding to dto {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while creating a new record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                            CancellationToken             ct                  = default!,
                                            string                        initialMessage      = null!,
                                            Func<TDto, string>            validMessageBuilder = null!,
                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                            params object[] pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: BuildNotFoundPkError(tableName: typeof(TDto).Name, 
                                                                                                             pk       : pk),
                                                                  token               : ct,
                                                                  pk                  : pk))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(bdData,
                                                                        notFoundErrorDetails: BuildNotFoundPkError(tableName: typeof(TDto).Name, pk: pk),
                                                                        token: ct,
                                                                        pk: pk))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was updated successfully in the table corresponding to dto {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }

    public Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                            CancellationToken             ct                  = default!,
                                            string                        initialMessage      = null!,
                                            Func<TDto, string>            validMessageBuilder = null!,
                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was updated successfully in the table corresponding to dto {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    public Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                            string                        initialMessage      = null!,
                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                            params object[] pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: BuildNotFoundPkError(tableName: typeof(TDto).Name, 
                                                                                                                 pk       : pk),
                                                                      token               : ct,
                                                                      pk                  : pk))
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    public Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
                                            CancellationToken             ct                  = default!,
                                            string                        initialMessage      = null!,
                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .MapAsync   ( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    private MlErrorsDetails BuildNotFoundPkError(string tableName, params object[] pk)
        => MlErrorsDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({pk.GetPkValues()})",
                                                   new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({pk.GetPkValues()})" });
}
