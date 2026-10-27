// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.AspNetCore.Results.Helpers;

public static partial class MlErrorDetailsExtensions
{
    public static MlResult<ProblemDetailsData> GetProblemDetails(this MlErrorDetails source)
    {
        var result = MlResult.Empty()
                                .MapEnsure( _  => source.ContainsMetadata(ProblemsDetails),
                                            _  => "The MlErrorDetails does not have metadata.")
                                .Map      ( _  => source.Metadata[ProblemsDetails])
                                .MapEnsure(obj => obj is not null, 
                                            _  => "The details ProblemsDetails key has a null object.")
                                .Bind     (obj => obj.ToProblemDetails() );
        return result;
    }


    public static MlResult<ProblemDetailsData> ToProblemDetails(this object obj)
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

    private static ProblemDetailsData ExtractProblemDetailsInfo(object obj)
    {
        var type = obj.GetType();
        
        var status     = (int?)                       type.GetProperty("Status"    )?.GetValue(obj) ?? 500;
        var title      = (string?)                    type.GetProperty("Title"     )?.GetValue(obj) ?? "Error";
        var detail     = (string?)                    type.GetProperty("Detail"    )?.GetValue(obj) ?? string.Empty;
        var typeValue  = (string?)                    type.GetProperty("Type"      )?.GetValue(obj) ?? string.Empty;
        var errors     = (Dictionary<string, object>?)type.GetProperty("Errors"    )?.GetValue(obj) ?? new Dictionary<string, object>();
        var statusCode = (int?)                       type.GetProperty("StatusCode")?.GetValue(obj) ?? 500;
        
        return new ProblemDetailsData(
            Status    : status,
            Title     : title,
            Detail    : detail,
            Type      : typeValue,
            Errors    : errors,
            StatusCode: statusCode
        );
    }



}

