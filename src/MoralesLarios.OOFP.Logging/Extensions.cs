// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging;
public static class Extensions
{

    public static MlResult<T> MyMethodFinalLog<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      methodActionDesc)
        => source.LogCompletedResult(logger,
                                   validBuildMessage: item   => $"{methodActionDesc} done correctly.",
                                   failBuildMessage : errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");

    public static async Task<MlResult<T>> MyMethodFinalLogAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                  ILogger     logger,
                                                                  string      methodActionDesc)
        => await (await sourceAsync).LogCompletedResultAsync(logger,
                                                           validBuildMessage: item => $"{methodActionDesc} done correctly.",
                                                           failBuildMessage: errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");




}

