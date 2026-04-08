// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Extensions.Loggers;
public static class Extensions
{

    public static MlResult<T> MyMethodFinalLog<T>(this MlResult<T> source, 
                                                       ILogger     logger,
                                                       string      methodActionDesc)
        => source.LogMlResultFinal(logger,
                                   validBuildMessage: item   => $"{methodActionDesc} done correctly.",
                                   failBuildMessage : errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");

    public static async Task<MlResult<T>> MyMethodFinalLogAsync<T>(this Task<MlResult<T>> sourceAsync, 
                                                                  ILogger     logger,
                                                                  string      methodActionDesc)
        => await (await sourceAsync).LogMlResultFinalAsync(logger,
                                                           validBuildMessage: item => $"{methodActionDesc} done correctly.",
                                                           failBuildMessage: errors => $"Error when {methodActionDesc} Error: {errors.ToErrorsDetailsDescription()}");




}

