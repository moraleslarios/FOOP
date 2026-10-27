// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Application.Helpers;

namespace MoralesLarios.OOFP.Application.Services;

public class CrudServiceFp<TEntity, TDto>(IEfRepository<TEntity>                   _repo,
                                         ILogger<CrudServiceFp<TEntity, TDto>> _logger) : ICrudServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    public virtual Task<MlResult<IEnumerable<TDto>>> GetPagedAsync(CancellationToken               ct                  = default!,
                                                              string                          initialMessage      = null!,
                                                              Func<IEnumerable<TDto>, string> successMessageFactory = null!,
                                                              Func<MlErrorDetails  , string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync( _     => _repo.TryAllAsync(ct))
                            .MapAsync (bdData => bdData.Adapt<IEnumerable<TDto>>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null  ? $"Found {x.Count()} of table {typeof(TDto).Name}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null  ? $"An error occurred while querying the {typeof(TDto).Name} table. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }

    public virtual Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                                       string                        initialMessage      = null!,
                                                       Func<TDto, string>            successMessageFactory = null!,
                                                       Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                       params object[]               pk)
        => FindByIdProblemsDetailsAsync(ct                  : ct,
                         initialMessage      : initialMessage,
                         notFoundErrorDetails: typeof(TDto).Name.BuildEntityNotFoundError(pk),
                         successMessageFactory : successMessageFactory,
                         failMessageBuilder  : failMessageBuilder,
                         pk                  : pk);

    public virtual Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync(MlErrorDetails               notFoundErrorDetails,
                                                                      CancellationToken             ct                   = default!,
                                                                      string                        initialMessage       = null!,
                                                                      Func<TDto, string>            successMessageFactory  = null!,
                                                                      Func<MlErrorDetails, string> failMessageBuilder   = null!,
                                                                      params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying data from the {typeof(TDto).Name} table by Id ({pk.FormatCompositeKey()})")
                            .BindAsync( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                    token               : ct,
                                                                    keyValues                  : pk))
                            .MapAsync (vinoBd => vinoBd?.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => x is not null
                                                                                    ? (successMessageFactory is null
                                                                                            ? $"The query to the {typeof(TDto).Name} table for the pk {pk.FormatCompositeKey()} completed successfully"
                                                                                            : successMessageFactory(x))
                                                                                    : $"No data found in the {typeof(TDto).Name} table for the pk {pk.FormatCompositeKey()}",
                                                   failBuildMessage : errors => failMessageBuilder is null
                                                                                    ? $"An error occurred while querying the {typeof(TDto).Name} table for the pk {pk.FormatCompositeKey()}. Error: {errors.ToString()}"
                                                                                    : failMessageBuilder(errors));
        return result;
    }



    public virtual Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                                    CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<TDto, string>            successMessageFactory = null!,
                                                    Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Creating a new record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null ? $"The record was created successfully in the table corresponding to dto {typeof(TDto).Name}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while creating a new record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                                    CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<TDto, string>            successMessageFactory = null!,
                                                    Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                    params object[] pk)
        => UpdateAsync(dto, typeof(TDto).Name.BuildEntityNotFoundError(pk), ct, initialMessage, successMessageFactory, failMessageBuilder, pk);


    public virtual Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                                                  MlErrorDetails               notFoundErrorDetails,
                                                                  CancellationToken             ct                  = default!,
                                                                  string                        initialMessage      = null!,
                                                                  Func<TDto, string>            successMessageFactory = null!,
                                                                  Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                                  params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      keyValues                  : pk))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(item                : bdData,
                                                                        notFoundErrorDetails: notFoundErrorDetails,
                                                                        token               : ct,
                                                                        keyValues                  : pk))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null ? $"The record was updated successfully in the table corresponding to dto {typeof(TDto).Name}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }





    public virtual Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                                    CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<TDto, string>            successMessageFactory = null!,
                                                    Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .TryMapAsync( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null ? $"The record was updated successfully in the table corresponding to dto {typeof(TDto).Name}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                    params object[]               pk)
        => DeleteProblemDetailsAsync(typeof(TDto).Name.BuildEntityNotFoundError(pk), ct, initialMessage, failMessageBuilder, pk);


    public virtual Task<MlResult<TDto>> DeleteProblemDetailsAsync(       MlErrorDetails               notFoundErrorDetails,
                                                                         CancellationToken             ct                  = default!,
                                                                         string                        initialMessage      = null!,
                                                                         Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                                  params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      keyValues                  : pk))
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
                                                    CancellationToken             ct                  = default!,
                                                    string                        initialMessage      = null!,
                                                    Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TDto).Name}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dto, $"{nameof(dto)} can't be null"))
                            .MapAsync   ( _     => dto.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TDto>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TDto).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TDto).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    //private MlErrorDetails BuildEntityNotFoundError(string tableName, params object[] keyValues)
    //    => MlErrorDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({keyValues.FormatCompositeKey()})",
    //                                               new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({keyValues.FormatCompositeKey()})" });
}






/*********************************************************************************
 * 
 *                                      DUPLEX
 *                      
 * ********************************************************************************/



public class CrudServiceFp<TEntity, TRequest, TResponse>(IEfRepository<TEntity>                                  _repo,
                                                        ILogger<CrudServiceFp<TEntity, TRequest, TResponse>> _logger) : ICrudServiceFp<TEntity, TRequest, TResponse>
    where TEntity   : class
    where TRequest  : class
    where TResponse : class
{


    public virtual Task<MlResult<IEnumerable<TResponse>>> GetPagedAsync(CancellationToken                    ct                  = default!,
                                                                   string                               initialMessage      = null!,
                                                                   Func<IEnumerable<TResponse>, string> successMessageFactory = null!,
                                                                   Func<MlErrorDetails  , string>      failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying all records of the table corresponding to dto {typeof(TResponse).Name}")
                            .BindAsync( _     => _repo.TryAllAsync(ct))
                            .MapAsync (bdData => bdData.Adapt<IEnumerable<TResponse>>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null  ? $"Found {x.Count()} of table {typeof(TResponse).Name}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null  ? $"An error occurred while querying the {typeof(TResponse).Name} table. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    public virtual Task<MlResult<TResponse?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                                            string                        initialMessage      = null!,
                                                            Func<TResponse, string>       successMessageFactory = null!,
                                                            Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                            params object[]               pk)
        => FindByIdProblemsDetailsAsync(ct                  : ct,
                                        initialMessage      : initialMessage,
                                        notFoundErrorDetails: typeof(TResponse).Name.BuildEntityNotFoundError(pk),
                                        successMessageFactory : successMessageFactory,
                                        failMessageBuilder  : failMessageBuilder,
                                        pk                  : pk);

    public virtual Task<MlResult<TResponse?>> FindByIdProblemsDetailsAsync(MlErrorDetails               notFoundErrorDetails,
                                                                           CancellationToken             ct                   = default!,
                                                                           string                        initialMessage       = null!,
                                                                           Func<TResponse, string>       successMessageFactory  = null!,
                                                                           Func<MlErrorDetails, string> failMessageBuilder   = null!,
                                                                           params object[]               pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Querying data from the {typeof(TResponse).Name} table by Id ({pk.FormatCompositeKey()})")
                            .BindAsync( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                    token               : ct,
                                                                    keyValues                  : pk))
                            .MapAsync (vinoBd => vinoBd?.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => x is not null
                                                                                    ? (successMessageFactory is null
                                                                                            ? $"The query to the {typeof(TResponse).Name} table for the pk {pk.FormatCompositeKey()} completed successfully"
                                                                                            : successMessageFactory(x))
                                                                                    : $"No data found in the {typeof(TResponse).Name} table for the pk {pk.FormatCompositeKey()}",
                                                   failBuildMessage : errors => failMessageBuilder is null
                                                                                    ? $"An error occurred while querying the {typeof(TResponse).Name} table for the pk {pk.FormatCompositeKey()}. Error: {errors.ToString()}"
                                                                                    : failMessageBuilder(errors));
        return result;
    }



    public virtual Task<MlResult<TResponse>> CreateAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<TResponse, string>       successMessageFactory = null!,
                                                         Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Creating a new record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryAddAsync(bdData, token: ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger            : _logger,
                                                   validBuildMessage : x      => successMessageFactory is null ? $"The record was created successfully in the table corresponding to {DtosDescType}" : successMessageFactory(x),
                                                   failBuildMessage  : errors => failMessageBuilder  is null ? $"An error occurred while creating a new record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> UpdateAsync(       TRequest                      dtoRequest,
                                                                CancellationToken             ct                  = default!,
                                                                string                        initialMessage      = null!,
                                                                Func<TResponse, string>       successMessageFactory = null!,
                                                                Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                         params object[]                      pk)
        => UpdateAsync(dtoRequest, typeof(TEntity).Name.BuildEntityNotFoundError(pk), ct, initialMessage, successMessageFactory, failMessageBuilder, pk);


    public virtual Task<MlResult<TResponse>> UpdateAsync(       TRequest                      dtoRequest,
                                                                              MlErrorDetails               notFoundErrorDetails,
                                                                              CancellationToken             ct                  = default!,
                                                                              string                        initialMessage      = null!,
                                                                              Func<TResponse, string>       successMessageFactory = null!,
                                                                              Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                                       params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      keyValues                  : pk))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(item                : bdData,
                                                                        notFoundErrorDetails: notFoundErrorDetails,
                                                                        token               : ct,
                                                                        keyValues                  : pk))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null ? $"The record was updated successfully in the table corresponding to {DtosDescType}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }





    public virtual Task<MlResult<TResponse>> UpdateAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<TResponse, string>       successMessageFactory = null!,
                                                         Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Updating a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .TryMapAsync( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryUpdateAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: x      => successMessageFactory is null ? $"The record was updated successfully in the table corresponding to {DtosDescType}" : successMessageFactory(x),
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while updating a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> DeleteAsync(       CancellationToken             ct                  = default!,
                                                                string                        initialMessage      = null!,
                                                                Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                         params object[]                      pk)
        => DeleteProblemDetailsAsync(typeof(TEntity).Name.BuildEntityNotFoundError(pk), ct, initialMessage, failMessageBuilder, pk);


    public virtual Task<MlResult<TResponse>> DeleteProblemDetailsAsync(       MlErrorDetails               notFoundErrorDetails,
                                                                              CancellationToken             ct                  = default!,
                                                                              string                        initialMessage      = null!,
                                                                              Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                                                       params object[]                      pk)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to dto {typeof(TResponse).Name}")
                            .BindAsync  ( _     => EnsureFp.That(pk, pk is not null && pk.Any(), $"{nameof(pk)} can't be null or empty"))
                            .BindAsync  ( _     => _repo.TryFindAsync(notFoundErrorDetails: notFoundErrorDetails,
                                                                      token               : ct,
                                                                      keyValues                  : pk))
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to dto {typeof(TResponse).Name}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to dto {typeof(TResponse).Name}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }


    public virtual Task<MlResult<TResponse>> DeleteAsync(TRequest                      dtoRequest,
                                                         CancellationToken             ct                  = default!,
                                                         string                        initialMessage      = null!,
                                                         Func<MlErrorDetails, string> failMessageBuilder  = null!)
    {
        var result = _logger.LogMlResultInformationAsync(initialMessage ?? $"Deleting a record in the table corresponding to {DtosDescType}")
                            .BindAsync  ( _     => EnsureFp.NotNull(dtoRequest, $"{nameof(dtoRequest)} can't be null"))
                            .MapAsync   ( _     => dtoRequest.Adapt<TEntity>())
                            .BindAsync  (bdData => _repo.TryRemoveAsync(bdData, ct))
                            .MapAsync   (bdData => bdData.Adapt<TResponse>())
                            .LogCompletedResultAsync(logger           : _logger,
                                                   validBuildMessage: _      => $"The record was deleted successfully in the table corresponding to {DtosDescType}",
                                                   failBuildMessage : errors => failMessageBuilder  is null ? $"An error occurred while deleting a record in the table corresponding to {DtosDescType}. Error: {errors.ToString()}" : failMessageBuilder(errors));
        return result;
    }



    //private MlErrorDetails BuildEntityNotFoundError(string tableName, params object[] keyValues)
    //    => MlErrorDetails.FromErrorMessageDetails($"No data found for the {tableName} table by Id ({keyValues.FormatCompositeKey()})",
    //                                               new Dictionary<string, object>() { ["NotFound"] = $"No data found for the {tableName} table by Id ({keyValues.FormatCompositeKey()})" });


    private string DtosDescType => $"dto Request {typeof(TRequest).Name} to dto Response {typeof(TResponse).Name}";

}