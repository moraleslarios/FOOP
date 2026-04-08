// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.WebApi.Helpers;

public static class MlErrorsDetailsExtensions
{

    private const string ProblemsDetails = nameof(ProblemsDetails);


    public static MlResult<ProblemDetailsInfo> GetProblemDetails(this MlErrorsDetails source)
    {
        var result = MlResult.Empty()
                                .MapEnsure( _  => source.HasKeyDetails(ProblemsDetails),
                                            _  => "The MlErrorsDetails does not have details.")
                                .Map      ( _  => source.Details[ProblemsDetails])
                                .MapEnsure(obj => obj is not null, 
                                            _  => "The details ProblemsDetails key has a null object.")
                                .Bind     (obj => obj.ToProblemsDetailsInfo() );
        return result;
    }


    public static MlResult<ProblemDetailsInfo> ToProblemsDetailsInfo(this object obj)
    {
        var result = MlResult.Empty()
                            .MapEnsure(_ => HasRequiredProblemDetailsProperties(obj), 
                                        _ => "The object does not have all required properties (Status, Title, Detail, Type, Errors, StatusCode).")
                            .Map(_ => ExtractProblemDetailsInfo(obj));
        return result;
    }

    private static bool HasRequiredProblemDetailsProperties(object obj)
    {
        var type = obj.GetType();
        var requiredProperties = new[] { "Status", "Title", "Detail", "Type", "Errors", "StatusCode" };
        
        return requiredProperties.All(propName => type.GetProperty(propName) != null);
    }

    private static ProblemDetailsInfo ExtractProblemDetailsInfo(object obj)
    {
        var type = obj.GetType();
        
        var status     = (int?)                       type.GetProperty("Status"    )?.GetValue(obj) ?? 500;
        var title      = (string?)                    type.GetProperty("Title"     )?.GetValue(obj) ?? "Error";
        var detail     = (string?)                    type.GetProperty("Detail"    )?.GetValue(obj) ?? string.Empty;
        var typeValue  = (string?)                    type.GetProperty("Type"      )?.GetValue(obj) ?? string.Empty;
        var errors     = (Dictionary<string, object>?)type.GetProperty("Errors"    )?.GetValue(obj) ?? new Dictionary<string, object>();
        var statusCode = (int?)                       type.GetProperty("StatusCode")?.GetValue(obj) ?? 500;
        
        return new ProblemDetailsInfo(
            Status    : status,
            Title     : title,
            Detail    : detail,
            Type      : typeValue,
            Errors    : errors,
            StatusCode: statusCode
        );
    }



}

