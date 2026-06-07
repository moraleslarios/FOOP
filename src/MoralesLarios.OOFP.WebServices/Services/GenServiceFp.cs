// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.WebServices.Helpers;

namespace MoralesLarios.OOFP.WebServices.Services;

public class GenServiceFp<TEntity, TDto>(IEFRepoFp<TEntity>                   _repo,
                                         ILogger<GenServiceFp<TEntity, TDto>> _logger) : IGenServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    public virtual Task<MlResult<IEnumerable<TDto>>> AllAsync(CancellationToken               ct                  = default!,
                                                              string                          initialMessage      = null!,
                                                              Func<IEnumerable<TDto>, string> validMessageBuilder = null!,
                                                              Func<MlErrorsDetails  , string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync( _     => _repo.TryAllAsync(ct))
                            .MapAsync (bdData => bdData.Adapt<IEnumerable<TDto>>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null  ? $"Found {x.Count()} of table {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null  ? $"An error occurred while querying the {typeof(TDto).Name} table. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }

    public virtual Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                                       string                        initialMessage      = null!,
                                                       Func<TDto, string>            validMessageBuilder = null!,
                                                       Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                       params object[]               pk)
        => FindByIdProblemsDetailsAsync(ct                  : ct,
                         initialMessage      : initialMessage,
                         notFoundErrorDetails: typeof(TDto).Name.BuildNotFoundPkError(pk),
                         validMessageBuilder : validMessageBuilder,
                         failMessageBuilder  : failMessageBuilder,
                         pk                  : pk);

    public virtual Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync(MlErrorsDetails               notFoundErrorDetails,
                                                                      CancellationToken             ct                   = default!,
                                                                      string                        initialMessage       = null!,
                                                                      Func<TDto, string>            validMessageBuilder  = null!,
                                                                      Func<MlErrorsDetails, string> failMessageBuilder   = null!,
                                                                      params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying data from the {typeof(TDto).Name} table by Id ({pk.GetPkValues()})")
                            .BindAsync( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
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



    public virtual Task<MlResult<TDto>> CreateAsync(TDto                          dto,
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


    public virtual Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                                    CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<TDto, string>            validMessageBuilder = null!,
                                                    Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                    params object[] pk)
        => UpdateProblemDetailsAsync(dto, typeof(TDto).Name.BuildNotFoundPkError(pk), ct, initialMessage, validMessageBuilder, failMessageBuilder, pk);


    public virtual Task<MlResult<TDto>> UpdateProblemDetailsAsync(TDto                          dto,
                                                                  MlErrorsDetails               notFoundErrorDetails,
                                                                  CancellationToken             ct                  = default!,
                                                                  string                        initialMessage      = null!,
                                                                  Func<TDto, string>            validMessageBuilder = null!,
                                                                  Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                                  params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      pk                  : pk))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(item                : bdData,
                                                                        notFoundErrorDetails: notFoundErrorDetails,
                                                                        token               : ct,
                                                                        pk                  : pk))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was updated successfully in the table corresponding to dto {typeof(TDto).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }





    public virtual Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
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


    public virtual Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                    params object[]               pk)
        => DeleteProblemDetailsAsync(typeof(TDto).Name.BuildNotFoundPkError(pk), ct, initialMessage, failMessageBuilder, pk);


    public virtual Task<MlResult<TDto>> DeleteProblemDetailsAsync(       MlErrorsDetails               notFoundErrorDetails,
                                                                         CancellationToken             ct                  = default!,
                                                                         string                        initialMessage      = null!,
                                                                         Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                                  params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      pk                  : pk))
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
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



    //private MlErrorsDetails BuildNotFoundPkError(string tableName, params object[] pk)
    //    => MlErrorsDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({pk.GetPkValues()})",
    //                                               new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({pk.GetPkValues()})" });
}






/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/



public class GenServiceFp<TEntity, TRequest, TResponse>(IEFRepoFp<TEntity>                                  _repo,
                                                        ILogger<GenServiceFp<TEntity, TRequest, TResponse>> _logger) : IGenServiceFp<TEntity, TRequest, TResponse>
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{


    public virtual Task<MlResult<IEnumerable<TResponse>>> AllAsync(CancellationToken                    ct                  = default!,
                                                                   string                               initialMessage      = null!,
                                                                   Func<IEnumerable<TResponse>, string> validMessageBuilder = null!,
                                                                   Func<MlErrorsDetails  , string>      failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of the table corresponding to dto {typeof(TResponse).Name}")
                            .BindAsync( _     => _repo.TryAllAsync(ct))
                            .MapAsync (bdData => bdData.Adapt<IEnumerable<TResponse>>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null  ? $"Found {x.Count()} of table {typeof(TResponse).Name}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null  ? $"An error occurred while querying the {typeof(TResponse).Name} table. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    public virtual Task<MlResult<TResponse?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                                            string                        initialMessage      = null!,
                                                            Func<TResponse, string>       validMessageBuilder = null!,
                                                            Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                            params object[]               pk)
        => FindByIdProblemsDetailsAsync(ct                  : ct,
                                        initialMessage      : initialMessage,
                                        notFoundErrorDetails: typeof(TResponse).Name.BuildNotFoundPkError(pk),
                                        validMessageBuilder : validMessageBuilder,
                                        failMessageBuilder  : failMessageBuilder,
                                        pk                  : pk);

    public virtual Task<MlResult<TResponse?>> FindByIdProblemsDetailsAsync(MlErrorsDetails               notFoundErrorDetails,
                                                                           CancellationToken             ct                   = default!,
                                                                           string                        initialMessage       = null!,
                                                                           Func<TResponse, string>       validMessageBuilder  = null!,
                                                                           Func<MlErrorsDetails, string> failMessageBuilder   = null!,
                                                                           params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying data from the {typeof(TResponse).Name} table by Id ({pk.GetPkValues()})")
                            .BindAsync( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                    token               : ct,
                                                                    pk                  : pk))
                            .MapAsync (vinoBd => vinoBd?.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => x is not null
                                                                                    ? (validMessageBuilder is null
                                                                                            ? $"The query to the {typeof(TResponse).Name} table for the pk {pk.GetPkValues()} completed successfully"
                                                                                            : validMessageBuilder(x))
                                                                                    : $"No data found in the {typeof(TResponse).Name} table for the pk {pk.GetPkValues()}",
                                                   failBuildMessage : errors => failMessageBuilder is null
                                                                                    ? $"An error occurred while querying the {typeof(TResponse).Name} table for the pk {pk.GetPkValues()}. Error: {errors.ToString()}"
                                                                                    : failMessageBuilder(errors));
        return result;
    }



    public virtual Task<MlResult<TResponse>> CreateAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<TResponse, string>       validMessageBuilder = null!,
                                                         Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Creating a new record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger            : _logger,
                                                   validBuildMessage : x      => validMessageBuilder is null ? $"The record was created successfully in the table corresponding to {DtosDescType}" : validMessageBuilder(x),
                                                   failBuildMessage  : errors => failMessageBuilder  is null ? $"An error occurred while creating a new record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> UpdateAsync(       TRequest                      dtoRequest,
                                                                CancellationToken             ct                  = default!,
                                                                string                        initialMessage      = null!,
                                                                Func<TResponse, string>       validMessageBuilder = null!,
                                                                Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                         params object[]                      pk)
        => UpdateProblemDetailsAsync(dtoRequest, typeof(TEntity).Name.BuildNotFoundPkError(pk), ct, initialMessage, validMessageBuilder, failMessageBuilder, pk);


    public virtual Task<MlResult<TResponse>> UpdateProblemDetailsAsync(       TRequest                      dtoRequest,
                                                                              MlErrorsDetails               notFoundErrorDetails,
                                                                              CancellationToken             ct                  = default!,
                                                                              string                        initialMessage      = null!,
                                                                              Func<TResponse, string>       validMessageBuilder = null!,
                                                                              Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                                       params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      pk                  : pk))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(item                : bdData,
                                                                        notFoundErrorDetails: notFoundErrorDetails,
                                                                        token               : ct,
                                                                        pk                  : pk))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was updated successfully in the table corresponding to {DtosDescType}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }





    public virtual Task<MlResult<TResponse>> UpdateAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<TResponse, string>       validMessageBuilder = null!,
                                                         Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: x      => validMessageBuilder is null ? $"The record was updated successfully in the table corresponding to {DtosDescType}" : validMessageBuilder(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> DeleteAsync(       CancellationToken             ct                  = default!,
                                                                string                        initialMessage      = null!,
                                                                Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                         params object[]                      pk)
        => DeleteProblemDetailsAsync(typeof(TEntity).Name.BuildNotFoundPkError(pk), ct, initialMessage, failMessageBuilder, pk);


    public virtual Task<MlResult<TResponse>> DeleteProblemDetailsAsync(       MlErrorsDetails               notFoundErrorDetails,
                                                                              CancellationToken             ct                  = default!,
                                                                              string                        initialMessage      = null!,
                                                                              Func<MlErrorsDetails, string> failMessageBuilder  = null!,
                                                                       params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TResponse).Name}")
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      pk                  : pk))
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TResponse).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TResponse).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> DeleteAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<MlErrorsDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .MapAsync   ( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogMlResultFinalAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to {DtosDescType}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    //private MlErrorsDetails BuildNotFoundPkError(string tableName, params object[] pk)
    //    => MlErrorsDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({pk.GetPkValues()})",
    //                                               new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({pk.GetPkValues()})" });


    private string DtosDescType => $"dto Request {typeof(TRequest).Name} to dto Response {typeof(TResponse).Name}";

}