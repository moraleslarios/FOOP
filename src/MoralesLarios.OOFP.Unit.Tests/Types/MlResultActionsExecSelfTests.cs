
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultActionsExecSelfTests
{








    #region ExecSelfIfFailWithValue


    [Fact]
    public void ExecSelfIfFailWithValue_whenSourceIfValid_returnSource()
    {
        MlResult<int> partialResult = 1;

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        result.IsValid.Should().BeTrue();
    }



    [Fact]
    public void ExecSelfIfFailWithValue_whenSourceIffail_withoutValueDetails_returnFail()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void ExecSelfIfFailWithValue_whenSourceIffail_withoutValueDetails_returnFail_with_2_errors()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        var eerorsResult = result.Match(
                                            _    => 0,
                                            fail => fail.Errors.Count()
                                         );

        eerorsResult.Should().Be(2);
    }


    [Fact]
    public void ExecSelfIfFailWithValue_whenSourceIffail_withValueDetails_execWithValidValue()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>()
                                                    .AddValueDetailIfFail(3);

        var internalResult = -1;

        MlResult<int> result = partialResult.ExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        internalResult.Should().Be(2);
    }


    [Fact]
    public void TryExecSelfIfFailWithValue_whenSourceIfValid_returnSource()
    {
        MlResult<int> partialResult = 1;

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        result.IsValid.Should().BeTrue();
    }



    [Fact]
    public void TryExecSelfIfFailWithValue_whenSourceIffail_withoutValueDetails_returnFail()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void TryExecSelfIfFailWithValue_whenSourceIffail_withoutValueDetails_returnFail_with_2_errors()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        var eerorsResult = result.Match(
                                            _    => 0,
                                            fail => fail.Errors.Count()
                                         );

        eerorsResult.Should().Be(2);
    }


    [Fact]
    public void TryExecSelfIfFailWithValue_whenSourceIffail_withValueDetails_execWithValidValue()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>()
                                                    .AddValueDetailIfFail(3);

        var internalResult = -1;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithValue<int, int>(value => internalResult += value);

        internalResult.Should().Be(2);
    }



    [Fact]
    public void TryExecSelfIfFailWithValue_whenSourceIffail_withValueDetails_actionFailValueThrowException_returnFailWithExceptionDetails()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>()
                                                    .AddValueDetailIfFail(3);

        var internalResult = -1;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithValue<int, int>(value => internalResult += value / 0);

        bool hasException = result.Match(
                                            fail : errorDetails => errorDetails.HasExceptionDetails(),
                                            valid: _            => false
                                        );

        hasException.Should().BeTrue();
    }





    #endregion





    #region ExecSelfIfFailWithValue


    [Fact]
    public void ExecSelfIfFailWithException_sourceFailWithouException_returnFail()
    {
        MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithException(exception => internalResult += 1);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ExecSelfIfFailWithException_sourceFailWithouException_notExecuteActionFailException()
    {
        MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithException(exception => internalResult += 1);

        internalResult.Should().Be(0);
    }

    [Fact]
    public void ExecSelfIfFailWithException_sourceFailWithouException_returnFailWithNotExceptionDetails()
    {
        MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithException(exception => internalResult += 1);

        bool hasNotExceptionDetail = partialResult.Match(
                                                            fail : errorDetails => errorDetails.Errors.Any(error => error == "The key Ex does not exist in the details" ) ,
                                                            valid: _    => false
                                                        );

        internalResult.Should().Be(0);
    }



    [Fact]
    public void ExecSelfIfFailWithException_sourceWithouException_executeActionFailException()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithException(exception => internalResult += 1);

        internalResult.Should().Be(1);
    }




    #endregion





    #region ExecSelfIfFailWithValue


    //[Fact]
    //public void ExecSelfIfFailWithException_sourceFailWithouException_returnFail()
    //{
    //    MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

    //    var internalResult = 0;

    //    MlResult<int> result = partialResult.ExecSelfIfFailWithException(exception => internalResult += 1);

    //    result.IsFail.Should().BeTrue();
    //}








    #endregion





    #region ExecSelfIfFailWithoutException



    [Fact]
    public void ExecSelfIfFailWithoutException_sourceFailWithouException_returnFail()
    {
        MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void ExecSelfIfFailWithoutException_sourceFailWithouException_ExecuteActionFail()
    {
        MlResult<int> partialResult =  "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        internalResult.Should().Be(1);
    }


    [Fact]
    public void ExecSelfIfFailWithoutException_sourceFailWittException_returnFail()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ExecSelfIfFailWithoutException_sourceFailWittException_NotExecuteActionFail()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        internalResult.Should().Be(0);
    }

    [Fact]
    public void ExecSelfIfFailWithoutException_sourceValid_returnFail()
    {
        MlResult<int> partialResult =  1;

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ExecSelfIfFailWithoutException_sourceValid_NotExecutedActionFail()
    {
        MlResult<int> partialResult =  1;

        var internalResult = 0;

        MlResult<int> result = partialResult.ExecSelfIfFailWithoutException(errorDetails => internalResult += 1);

        internalResult.Should().Be(0);
    }



    #endregion




}
