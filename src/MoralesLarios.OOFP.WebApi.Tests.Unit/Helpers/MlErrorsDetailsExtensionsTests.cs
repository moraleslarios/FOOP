using Microsoft.AspNetCore.Mvc;
using MoralesLarios.OOFP.Types.Errors;
using MoralesLarios.OOFP.WebApi.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoralesLarios.OOFP.WebApi.Tests.Unit.Helpers;

public class MlErrorsDetailsExtensionsTests
{


    [Fact]
    public void GetProblemDetails_Has_not_details_return_fail()
    {
        MlErrorsDetails source = "error without details";

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void GetProblemDetails_Has_not_problemsDetailsKey_return_fail()
    {
        MlErrorsDetails source = ("error without problems details", new Dictionary<string, object>());

        MlResult<ProblemDetailsInfo> result = source.GetProblemDetails();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetProblemDetails_Has_problemsDetailsKey_return_valid()
    {
        MlErrorsDetails source = ("Bad request",
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

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void GetProblemDetails_Has_problemsDetailsKey_return_coorectData()
    {
        MlErrorsDetails source = ("Bad request",
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
