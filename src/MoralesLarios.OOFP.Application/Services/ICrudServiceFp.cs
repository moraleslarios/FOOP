// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


namespace MoralesLarios.OOFP.Application.Services;

public interface ICrudServiceFp<TEntity, TDto>
    where TEntity  : class
    where TDto     : class
{
    Task<MlResult<IEnumerable<TDto>>> GetPagedAsync(CancellationToken               ct                  = default!,
                                               string                          initialMessage      = null!,
                                               Func<IEnumerable<TDto>, string> successMessageFactory = null!,
                                               Func<MlErrorDetails, string>   failMessageBuilder  = null!);

    Task<MlResult<TDto?>> FindByIdProblemsDetailsAsync(MlErrorDetails               notFoundErrorDetails,
                                                       CancellationToken             ct                   = default, 
                                                       string                        initialMessage       = null!, 
                                                       Func<TDto, string>            successMessageFactory  = null!, 
                                                       Func<MlErrorDetails, string> failMessageBuilder   = null!, 
                                                       params object[] pk);


    Task<MlResult<TDto?>> FindByIdAsync(CancellationToken             ct                  = default!,
                                        string                        initialMessage      = null!,
                                        Func<TDto, string>            successMessageFactory = null!,
                                        Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                        params object[] pk);

    Task<MlResult<TDto>> CreateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);

    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    Task<MlResult<TDto>> DeleteAsync(CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!,
                                     params object[] pk);

    Task<MlResult<TDto>> UpdateAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<TDto, string>            successMessageFactory = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);

    Task<MlResult<TDto>> DeleteAsync(TDto                          dto,
                                     CancellationToken             ct                  = default!,
                                     string                        initialMessage      = null!,
                                     Func<MlErrorDetails, string> failMessageBuilder  = null!);
    Task<MlResult<TDto>> UpdateAsync(TDto dto, MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TDto, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TDto>> DeleteProblemDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
}


public interface ICrudServiceFp<TEntity, TRequest, TResponse>
    where TEntity : class
    where TRequest : class
    where TResponse : class
{
    Task<MlResult<IEnumerable<TResponse>>> GetPagedAsync(CancellationToken ct = default, string initialMessage = null, Func<IEnumerable<TResponse>, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    Task<MlResult<TResponse>> CreateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    Task<MlResult<TResponse>> DeleteAsync(CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TResponse>> DeleteAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    Task<MlResult<TResponse>> DeleteProblemDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TResponse?>> FindByIdAsync(CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TResponse?>> FindByIdProblemsDetailsAsync(MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null);
    Task<MlResult<TResponse>> UpdateAsync(TRequest dtoRequest, MlErrorDetails notFoundErrorDetails, CancellationToken ct = default, string initialMessage = null, Func<TResponse, string> successMessageFactory = null, Func<MlErrorDetails, string> failMessageBuilder = null, params object[] pk);
}
