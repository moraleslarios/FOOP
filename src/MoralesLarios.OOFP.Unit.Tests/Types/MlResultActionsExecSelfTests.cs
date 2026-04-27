// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0


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





    #region ExecSelfIfFailWithException_sourceFailWithouException


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




    
    #region TryExecSelfIfFailWithException

    [Fact]
    public void TryExecSelfIfFailWithException_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => internalResult += 1, "miError");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceFailWithouException_returnFail()
    {
        MlResult<int> partialResult = "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => internalResult += 1, "miErrorControlado");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceFailWithouException_notExecuteActionFailException()
    {
        MlResult<int> partialResult = "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => internalResult += 1, "miErrorControlado");

        internalResult.Should().Be(0);
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceFailWithouException_returnFail_with_2_errors()
    {
        MlResult<int> partialResult = "miError".ToMlResultFail<int>();

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => internalResult += 1, "miErrorControlado");

        var eerorsResult = result.Match(
                                            _    => 0,
                                            fail => fail.Errors.Count()
                                         );

        eerorsResult.Should().Be(2);
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceWithException_executeActionFailException()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                      );

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => internalResult += 1, "miErrorControlado");

        internalResult.Should().Be(1);
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceWithException_actionThrowException_withStringOverload_returnFailWithExceptionDetails()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                      );

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => throw new DivideByZeroException(), "miErrorControlado");

        bool hasException = result.Match(
                                            fail : errorDetails => errorDetails.HasExceptionDetails(),
                                            valid: _            => false
                                        );

        hasException.Should().BeTrue();
    }

    [Fact]
    public void TryExecSelfIfFailWithException_sourceWithException_actionThrowException_withBuilderOverload_returnFailWithExceptionDetails()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                      );

        var internalResult = 0;

        MlResult<int> result = partialResult.TryExecSelfIfFailWithException(exception => throw new DivideByZeroException(),
                                                                             ex => $"Error controlado: {ex.Message}");

        bool hasException = result.Match(
                                            fail : errorDetails => errorDetails.HasExceptionDetails(),
                                            valid: _            => false
                                        );

        hasException.Should().BeTrue();
    }

    #endregion




    #region ExecSelfIf

    [Fact]
    public void ExecSelfIf_sourceValid_conditionTrue_executeActionTrue_and_returnValid()
    {
        MlResult<int> source = 3;

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = source.ExecSelfIf(x => x > 0,
                                                 x => actionTrueCount += x,
                                                 x => actionFalseCount += x);

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(3);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public void ExecSelfIf_sourceValid_conditionFalse_executeActionFalse_and_returnValid()
    {
        MlResult<int> source = 3;

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = source.ExecSelfIf(x => x < 0,
                                                 x => actionTrueCount += x,
                                                 x => actionFalseCount += x);

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(3);
    }

    [Fact]
    public void ExecSelfIf_sourceFail_notExecuteActions_and_returnFail()
    {
        MlResult<int> source = "miError".ToMlResultFail<int>();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = source.ExecSelfIf(x => x > 0,
                                                 x => actionTrueCount += x,
                                                 x => actionFalseCount += x);

        result.IsFail.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecSelfIfAsync_sourceValid_overloadAllFunc_executeActionTrue()
    {
        MlResult<int> source = 5;

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await source.ExecSelfIfAsync(x => x > 0,
                                                            async x => { actionTrueCount += x; await Task.CompletedTask; },
                                                            async x => { actionFalseCount += x; await Task.CompletedTask; });

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(5);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecSelfIfAsync_sourceTask_overloadAllFunc_executeActionFalse()
    {
        Task<MlResult<int>> sourceAsync = 5.ToMlResultValidAsync();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await sourceAsync.ExecSelfIfAsync(x => x < 0,
                                                                 async x => { actionTrueCount += x; await Task.CompletedTask; },
                                                                 async x => { actionFalseCount += x; await Task.CompletedTask; });

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(5);
    }

    [Fact]
    public async Task ExecSelfIfAsync_sourceTask_actionTrueSync_actionFalseAsync_executeActionTrue()
    {
        Task<MlResult<int>> sourceAsync = 2.ToMlResultValidAsync();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await sourceAsync.ExecSelfIfAsync(x => x > 0,
                                                                 x => actionTrueCount += x,
                                                                 async x => { actionFalseCount += x; await Task.CompletedTask; });

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(2);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecSelfIfAsync_sourceTask_actionTrueAsync_actionFalseSync_executeActionFalse()
    {
        Task<MlResult<int>> sourceAsync = 2.ToMlResultValidAsync();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await sourceAsync.ExecSelfIfAsync(x => x < 0,
                                                                 async x => { actionTrueCount += x; await Task.CompletedTask; },
                                                                 x => actionFalseCount += x);

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecSelfIfAsync_sourceTask_overloadAllAction_executeActionTrue()
    {
        Task<MlResult<int>> sourceAsync = 1.ToMlResultValidAsync();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await sourceAsync.ExecSelfIfAsync(x => x > 0,
                                                                 x => actionTrueCount += x,
                                                                 x => actionFalseCount += x);

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(1);
        actionFalseCount.Should().Be(0);
    }


    [Fact]
    public void TryExecSelfIf_sourceValid_conditionTrue_actionThrows_withStringOverload_returnFail()
    {
        MlResult<int> source = 1;

        MlResult<int> result = source.TryExecSelfIf<int>(x => x > 0,
                                                          _ => throw new Exception("boom"),
                                                          _ => { },
                                                          _ => "error controlado");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryExecSelfIf_sourceValid_conditionFalse_actionThrows_withBuilderOverload_returnFail()
    {
        MlResult<int> source = 1;

        MlResult<int> result = source.TryExecSelfIf<int>(x => x < 0,
                                                          _ => { },
                                                          _ => throw new Exception("boom"),
                                                          ex => $"error: {ex.Message}");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryExecSelfIf_sourceFail_notExecuteActions_and_returnFail()
    {
        MlResult<int> source = "miError".ToMlResultFail<int>();

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = source.TryExecSelfIf<int>(x => x > 0,
                                                         x => actionTrueCount += x,
                                                         x => actionFalseCount += x,
                                                         _ => "error controlado");

        result.IsFail.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public async Task TryExecSelfIfAsync_source_overloadFuncTaskAction_withString_executeTruePath()
    {
        MlResult<int> source = 4;

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await source.TryExecSelfIfAsync<int>(x => x > 0,
                                                                    async x => { actionTrueCount += x; await Task.CompletedTask; },
                                                                    x => actionFalseCount += x,
                                                                    _ => "error controlado");

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(4);
        actionFalseCount.Should().Be(0);
    }

    [Fact]
    public async Task TryExecSelfIfAsync_source_overloadFuncTaskFuncTask_withBuilder_executeFalsePath()
    {
        MlResult<int> source = 4;

        var actionTrueCount = 0;
        var actionFalseCount = 0;

        MlResult<int> result = await source.TryExecSelfIfAsync<int>(x => x < 0,
                                                                    async x => { actionTrueCount += x; await Task.CompletedTask; },
                                                                    async x => { actionFalseCount += x; await Task.CompletedTask; },
                                                                    ex => ex.Message);

        result.IsValid.Should().BeTrue();
        actionTrueCount.Should().Be(0);
        actionFalseCount.Should().Be(4);
    }

    [Fact]
    public async Task TryExecSelfIfAsync_source_overloadActionFuncTask_withBuilder_actionThrows_returnFail()
    {
        MlResult<int> source = 4;

        MlResult<int> result = await source.TryExecSelfIfAsync<int>(x => x > 0,
                                                                   _ => throw new Exception("boom"),
                                                                   async _ => await Task.CompletedTask,
                                                                   ex => $"error: {ex.Message}");

        result.IsFail.Should().BeTrue();
    }

    #endregion


}

