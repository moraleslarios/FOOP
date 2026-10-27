// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using MoralesLarios.OOFP.Types.Errors;
using MoralesLarios.OOFP.AspNetCore.Results.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit.Helpers;

public class MlErrorDetailsExtensionsTests
{


    [Fact]
    public void GetProblemDetails_Has_not_details_return_fail()
    {
        MlErrorDetails source = "error without details";

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public void GetProblemDetails_Has_not_problemsDetailsKey_return_fail()
    {
        MlErrorDetails source = ("error without problems details", new Dictionary<string, object>());

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void GetProblemDetails_Has_problemsDetailsKey_return_valid()
    {
        MlErrorDetails source = ("Bad request",
                      new Dictionary<string, object>
                      {
                          { "ProblemsDetails", new
                              {
                                    Status     = 400,
                                    Title      = "Bad request",
                                    Detail     = "Invalid syntax or validation error.",
                                    Type       = "https://www.puntonetalpunto.net/",
                                    Errors     = new Dictionary<string, object>(),
                                    StatusCode = 400
                              }
                          }
                      });

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetProblemDetails_Has_problemsDetailsKey_return_coorectData()
    {
        MlErrorDetails source = ("Bad request",
                      new Dictionary<string, object>
                      {
                          { "ProblemsDetails", new
                              {
                                    Status     = 400,
                                    Title      = "Bad request",
                                    Detail     = "Invalid syntax or validation error.",
                                    Type       = "https://www.puntonetalpunto.net/",
                                    Errors     = new Dictionary<string, object>(),
                                    StatusCode = 400
                              }
                          }
                      });

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        MlResult<ProblemDetailsInfo> expected = new ProblemDetailsInfo
        (
            Status    : 400,
            Title     : "Bad request",
            Detail    : "Invalid syntax or validation error.",
            Type      : "https://www.puntonetalpunto.net/",
            Errors    : new Dictionary<string, object>(),
            StatusCode: 400
        );

        result.ToString().Should().Be(expected.ToString());
    }





}

