using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultChangeReturnResultTests
{




    #region ChangeReturnResult



    [Fact]
    public void ChangeReturnResult_validSource_return_MlResultValidValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.ChangeReturnResult(validValue: "valid", failValue : "invalid");
        MlResult<string> expected = "valid";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ChangeReturnResult_failSource_return_MlResultFailValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.ChangeReturnResult(validValue: "valid", failValue : "invalid");
        MlResult<string> expected = "invalid".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task ChangeReturnResultAsync_validSource_return_MlResultValidValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.ChangeReturnResultAsync(validValue: "valid", failValue : "invalid");
        MlResult<string> expected = "valid";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task ChangeReturnResultAsync_sourceAsync_failSource_return_MlResultFailValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultAsync.ChangeReturnResultAsync(validValue: "valid", failValue : "invalid");
        MlResult<string> expected = "invalid".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }



    #endregion ChangeReturnResult


    #region ChangeReturnResultAlwaisValid





    [Fact]
    public void ChangeReturnResultAlwaisValid_validSource_return_validValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.ChangeReturnResultAlwaisValid(validValue: "valid", failValidValue : "valid2");
        MlResult<string> expected = "valid";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ChangeReturnResultAlwaisValid_failSource_return_failValidValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.ChangeReturnResultAlwaisValid(validValue: "valid", failValidValue : "valid2");
        MlResult<string> expected = "valid2";

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task ChangeReturnResultAlwaisValidAsync_validSource_return_validValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.ChangeReturnResultAlwaisValidAsync(validValue: "valid", failValidValue : "valid2");
        MlResult<string> expected = "valid";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task ChangeReturnResultAlwaisValidAsync_sourceAsync_failSource_return_failValidValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultAsync.ChangeReturnResultAlwaisValidAsync(validValue: "valid", failValidValue : "valid2");
        MlResult<string> expected = "valid2";

        result.ToString().Should().Be(expected.ToString());
    }





    #endregion ChangeReturnResultAlwaisValid


    #region ChangeReturnResultAlwaisFail


    [Fact]
    public void ChangeReturnResultAlwaisFail_validSource_return_validFailValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.ChangeReturnResultAlwaisFail<int, string>(validFailValue: "valid", failValue : "valid2");
        MlResult<string> expected = "valid".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ChangeReturnResultAlwaisFail_failSource_return_failValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.ChangeReturnResultAlwaisFail<int, string>(validFailValue: "valid", failValue : "valid2");
        MlResult<string> expected = "valid2".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task ChangeReturnResultAlwaisFailAsync_validSource_return_validFailValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.ChangeReturnResultAlwaisFailAsync<int, string>(validFailValue: "valid", failValue : "valid2");
        MlResult<string> expected = "valid".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task ChangeReturnResultAlwaisFailAsync_sourceAsync_failSource_return_failValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultAsync.ChangeReturnResultAlwaisFailAsync<int, string>(validFailValue: "valid", failValue : "valid2");
        MlResult<string> expected = "valid2".ToMlResultFail<string>();

        result.ToString().Should().Be(expected.ToString());
    }




    #endregion ChangeReturnResultAlwaisFail


    #region ChangeReturnResultIfValid


    [Fact]
    public void ChangeReturnResultIfValid_validSource_return_returnValidValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.ChangeReturnResultIfValid(returnValidValue: 99);
        MlResult<int> expected = 99;

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void ChangeReturnResultIfValid_failSource_notChangeReturnValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.ChangeReturnResultIfValid(returnValidValue: 99);

        result.ToString().Should().Be(partialResult.ToString());
    }


    #endregion



    #region ChangeReturnResultIfValidToFail


    [Fact]
    public void ChangeReturnResultIfValidToFail_validSource_changeReturnToFail_withReturnFailValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.ChangeReturnResultIfValidToFail(returnFailValue: "fail");

        MlResult<int> expected = "fail".ToMlResultFail<int>();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ChangeReturnResultIfValidToFail_invalidSource_notChangeReturnValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.ChangeReturnResultIfValidToFail(returnFailValue: "fail");

        result.ToString().Should().Be(partialResult.ToString());
    }


    [Fact]
    public async Task ChangeReturnResultIfValidToFailAsync_validSource_changeReturnToFail_withReturnFailValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.ChangeReturnResultIfValidToFailAsync(returnFailValue: "fail");

        MlResult<int> expected = "fail".ToMlResultFail<int>();

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async void ChangeReturnResultIfValidToFailAsync_sourceAsync_invalidSource_notChangeReturnValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.ChangeReturnResultIfValidToFailAsync(returnFailValue: "fail");

        MlResult<int> expected = await partialResultAsync;

        result.ToString().Should().Be(expected.ToString());
    }




    #endregion ChangeReturnResultIfValidToFail



    #region ChangeReturnResultIfFailToValid


    [Fact]
    public void ChangeReturnResultIfFailToValid_validSource_notChangeReturnValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.ChangeReturnResultIfFailToValid(returnValidValue: 99);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void ChangeReturnResultIfFailToValid_failSource_return_returnValidValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.ChangeReturnResultIfFailToValid(returnValidValue: 99);

        MlResult<int> expected = 99;

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task ChangeReturnResultIfFailToValidAsync_validSource_notChangeReturnValue()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.ChangeReturnResultIfFailToValidAsync(returnValidValue: 99);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task ChangeReturnResultIfFailToValidAsync_sourceAsync_failSource_return_returnValidValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.ChangeReturnResultIfFailToValidAsync(returnValidValue: 99);

        MlResult<int> expected = 99;

        result.ToString().Should().Be(expected.ToString()); 
    }



    #endregion ChangeReturnResultIfFailToValid









    [Fact]
    public void TestGeneral()
    {

        MlResult<int> partialResult = 1;

        //MlResult<string> result = partialResult




    }








}
