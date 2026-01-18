using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Unit.Tests.Types;

public class MlResultActionsBindTests
{



    #region BindSaveValueIfInvalidFuncResult



    [Fact]
    public void BindSaveValueIfInvalidFuncResult_validSource_withFailFuncResult_AddValueToFailResult()
    {

        MlResult<int> partialResult = 0;

        MlResult<string> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethod);

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { "Value", 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindSaveValueIfInvalidFuncResult_validSource_withValidFuncResult_NotAddValueToFailResult()
    {

        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethod);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void BindSaveValueIfInvalidFuncResult_failSource_returnSource()
    {

        MlResult<int> partialResult = "Fail".ToMlResultFail<int>();

        MlResult<string> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethod);

        result.Should().BeEquivalentTo(partialResult);
    }


    [Fact]
    public async Task BindSaveValueIfInvalidFuncResultAsync_validSource_withFailFuncResult_AddValueToFailResult()
    {

        MlResult<int> partialResult = 0;

        MlResult<string> result = await partialResult.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { "Value", 0 } });

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task  BindSaveValueIfInvalidFuncResultAsync_validSource_withValidFuncResult_NotAddValueToFailResult()
    {

        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        MlResult<string> expected = "Not OK";

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task BindSaveValueIfInvalidFuncResultAsync_failSource_returnSource()
    {

        MlResult<int> partialResult = "Fail".ToMlResultFail<int>();

        MlResult<string> result = await partialResult.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        result.Should().BeEquivalentTo(partialResult);
    }


    [Fact]
    public async Task BindSaveValueIfInvalidFuncResultAsync_sourceAsync_validSource_withFailFuncResult_AddValueToFailResult()
    {

        Task<MlResult<int>> partialResultAsync = MlResult.Valid(0).ToAsync();

        MlResult<string> result = await partialResultAsync.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { "Value", 0 } });

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task  BindSaveValueIfInvalidFuncResultAsync_sourceAsync_validSource_withValidFuncResult_NotAddValueToFailResult()
    {

        Task<MlResult<int>> partialResultAsync = MlResult.Valid(1).ToAsync();

        MlResult<string> result = await partialResultAsync.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        MlResult<string> expected = "Not OK";

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task BindSaveValueIfInvalidFuncResultAsync_sourceAsync_failSource_returnSource()
    {

        Task<MlResult<int>> partialResultResultAsync = "Fail".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultResultAsync.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAsync);

        result.Should().BeEquivalentTo(await partialResultResultAsync);
    }

    //[Fact]
    //public async Task TryBindSaveValueIfInvalidFuncResultAsync_sourceAsync_failSource_returnSource()
    //{

    //    Task<MlResult<int>> partialResultResultAsync = "Fail".ToMlResultFailAsync<int>();

    //    MlResult<string> result = await partialResultResultAsync.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAIntThrowExceptionAsync);

    //    result.Should().BeEquivalentTo(await partialResultResultAsync);
    //}

    [Fact]
    public void TryBindSaveValueIfInvalidFuncResult_source_failSource_returnSource()
    {

        MlResult<int> partialResultResult = "Fail".ToMlResultFail<int>();

        MlResult<string> result = partialResultResult.TryBindSaveValueInDetailsIfFaildFuncResult(BindTestMethodStringThrowException, _ => "Error forced !!!!");

        result.Should().BeEquivalentTo(partialResultResult);
    }

    [Fact]
    public void TryBindSaveValueIfInvalidFuncResult_sourceValid_failSource_returnFailWithValue()
    {

        MlResult<int> partialResultResult = 0;

        MlResult<string> result = partialResultResult.TryBindSaveValueInDetailsIfFaildFuncResult(BindTestMethodStringThrowException, _ => "Error forced !!!!");

        result.IsFail.Should().BeTrue();
    }


    #endregion



    #region BindMulti


    [Fact]
    public void BindMulti_simple_when_sourceIsFail_and_allFuncs_valid_return_valid()
    {
        MlResult<int> partialResult = "error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.BindMulti<int, string>(returnFunc: x => "hecho !!!",
                                                                        x => "Bieenn",
                                                                        x => "Biieenn 2",
                                                                        x => "Bien 3333");

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public void BindMulti_simple_when_allFuncs_valid_return_valid()
    {
        MlResult<int> partialResult = 12;

        MlResult<string> result = partialResult.BindMulti<int, string>(returnFunc: x => "hecho !!!",
                                                                        x => "Bieenn",
                                                                        x => "Biieenn 2",
                                                                        x => "Bien 3333");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void BindMulti_simple_when_anyFuncs_fail_return_fail()
    {
        MlResult<int> partialResult = 12;

        MlResult<string> result = partialResult.BindMulti<int, string>(returnFunc: x => "hecho !!!",
                                                                        x => "Bieenn",
                                                                        x => "Biieenn 2",
                                                                        x => "error".ToMlResultFail<string>());

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void BindMulti_when_AllFuncs_OK_return_validExpected()
    {
        MlResult<int> partialResult = 12;

        MlResult<string> result = partialResult.BindMulti<int, string>(returnFunc: (value, results) => $"hecho {string.Join(",", results)} !!!",
                                                                        x => "Bieenn",
                                                                        x => "Biieenn 2",
                                                                        x => "Bien 3333");

        MlResult<string> expected = "hecho Bieenn,Biieenn 2,Bien 3333 !!!";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void BindMulti_complex_AllFuncs_OK_return_validExpected()
    {
        MlResult<int> partialResult = 12;

        MlResult<string> result = partialResult.BindMulti<int, string, DateTime>(returnFunc: (value, results) => $"hecho {string.Join(",", results)} !!!",
                                                                        x => new DateTime(1, 1, 1),
                                                                        x => new DateTime(2, 2, 2),
                                                                        x => new DateTime(3, 3, 3));

        MlResult<string> expected = "hecho 01/01/0001 0:00:00,02/02/0002 0:00:00,03/03/0003 0:00:00 !!!";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindMultiAsync_complex_AllFuncs_OK_return_validExpected()
    {
        Task<MlResult<int>> partialResult = 12.ToMlResultValidAsync();

        MlResult<string> result = await partialResult.BindMultiAsync<int, string, DateTime>(returnFuncAsync: (value, results) => $"hecho {string.Join(",", results)} !!!".ToMlResultValidAsync(),
                                                                                            x => new DateTime(1, 1, 1).ToMlResultValidAsync(),
                                                                                            x => new DateTime(2, 2, 2).ToMlResultValidAsync(),
                                                                                            x => new DateTime(3, 3, 3).ToMlResultValidAsync());

        MlResult<string> expected = "hecho 01/01/0001 0:00:00,02/02/0002 0:00:00,03/03/0003 0:00:00 !!!";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }




    #endregion



    #region BindIfFail



    [Fact]
    public void BindIfFail_validSource_returnSource()
    {

        MlResult<int> partialResult = 0;

        MlResult<int> result = partialResult.BindIfFail(x => -1);

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public void BindIfFail_failSource_Ok()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.BindIfFail(x => -1);

        MlResult<int> expected = -1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async Task BindIfFailAsync_validSource_returnSource()
    {

        MlResult<int> partialResult = 0;

        MlResult<int> result = await partialResult.BindIfFailAsync(x => (-1).ToMlResultValidAsync());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public async Task BindIfFailAsync_failSource_Ok()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.BindIfFailAsync(x => (-1).ToMlResultValidAsync());

        MlResult<int> expected = -1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task BindIfFailAsync_sourceAsync_validSource_returnSource()
    {

        Task<MlResult<int>> partialResultAsync = 0.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.BindIfFailAsync(x => (-1).ToMlResultValidAsync());

        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public async Task BindIfFailAsync_sourceAsync_failSource_Ok()
    {

        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailAsync(x => (-1).ToMlResultValidAsync());

        MlResult<int> expected = -1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void BindIfFail_diferentReturnValue_failSource_ExecuteFuncFail()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.BindIfFail<int, string>(funcValid: x             => x.ToString(),
                                                                        funcFail : errorsDetails => errorsDetails.Errors.First().ToString());

        MlResult<string> expected = "Error";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void BindIfFail_diferentReturnValue_validSource_ExecuteFuncValid()
    {

        MlResult<int> partialResult = -1;

        MlResult<string> result = partialResult.BindIfFail<int, string>(funcValid: x             => x.ToString(),
                                                                        funcFail : errorsDetails => errorsDetails.Errors.First().ToString());

        MlResult<string> expected = "-1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    #endregion



    #region BindIfFailWithValue


    [Fact]
    public void BindIfFailWithPreviousValue_validSource_firsFuncReturnFail_returnSource()
    {

        MlResult<int> partialResult = 0;

        MlResult<int> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethodInt)
                                                .BindIfFailWithValue(x => x * 2);

        MlResult<int> expected = 0;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindIfFailWithPreviousValue_validSource_firsFuncReturnValid_returnSource()
    {

        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethodInt)
                                                .BindIfFailWithValue(x => x * 2);

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindIfFailWithPreviousValue_faildInitialSource_returnFail()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethodInt)
                                                .BindIfFailWithValue(x => x * 2);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void BindIfFailWithPreviousValue_faildInitialSource_returnFail_with2Erros()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.BindSaveValueInDetailsIfFaildFuncResult(BindTestMethodInt)
                                                .BindIfFailWithValue(x => x * 2);

        int errorsCount = 0;

        result.Match(
                       valid: x => errorsCount = 0,
                        fail: x =>
                        {
                            errorsCount = x.Errors.Count();

                            return result;
                        }
                     );;

        errorsCount.Should().Be(2);
    }



    [Fact]
    public async Task BindIfFailWithPreviousValueAsync_validSource_firsFuncReturnFail_returnSource()
    {

        MlResult<int> partialResult = 0;

        MlResult<int> result = await partialResult.BindSaveValueInDetailsIfFaildFuncResultAsync(BindTestMethodAIntsync)
                                                     .BindIfFailWithValueAsync(x => (x + 1).ToMlResultValidAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task BindExIfFailWithValueAsync_validSource_firsFuncReturnFailExcepcion_returnFail()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.BindAsync(BindTestMethodIntThrowExceptionAsync)
                                                            .AddValueDetailIfFailAsync(0)
                                                    .TryBindIfFailWithValueAsync(BindTestMethodIntThrowExceptionAsync , x => "Sin error");

        MlResult<int> expected = 0;

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task BindExIfFailWithValueAsync_validSource_firsFuncReturnFailExcepcion_returnFail_withExDetails()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.BindAsync(BindTestMethodIntThrowExceptionAsync)
                                                            .AddValueDetailIfFailAsync(0)
                                                    .TryBindIfFailWithValueAsync(BindTestMethodIntThrowExceptionAsync , x => "Sin error");

        bool hasDetails = result.Match(
                                            valid: x             => false,
                                            fail : errorsDetails => errorsDetails.HasExceptionDetails()
                                        );

        hasDetails.Should().BeTrue();
    }


    #endregion



    #region TryBindIfFail


    [Fact]
    public void TryBindIfFail_failSource_funcThrowException_returnFail()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryBindIfFail(errorsDetails => BindTestMethodIntThrowException(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public void TryBindIfFail_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryBindIfFail(errorsDetails => BindTestMethodIntThrowException(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public void TryBindIfFail_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryBindIfFail(errorsDetails => BindTestMethodIntThrowException(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }




    

    [Fact]
    public async Task TryBindIfFailAsync_failSource_funcThrowException_returnFail()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public async Task TryBindIfFailAsync_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindIfFailAsync_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }




    
    

    [Fact]
    public async Task TryBindIfFailAsync_sourceAsync_failSource_funcThrowException_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public async Task TryBindIfFailAsync_sourceAsync_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindIfFailAsync_sourceAsync_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryBindIfFailAsync(errorsDetails => BindTestMethodIntThrowExceptionAsync(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }






    #endregion



    #region BindIfFailWithValue


    [Fact]
    public void BindIfFailWithValue_Multi_validFuncPrevious_printValidValue()
    {
        MlResult<int> partialResult = 1;

        
        MlResult<string> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .AddValueDetailIfFail(DateTime.Now))
                                                .BindIfFailWithValue<string, DateTime, string>(funcValid: stringPrevious => stringPrevious,
                                                                                               funcFail : valueDateTime  => valueDateTime.ToString());

        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void BindIfFailWithValue_Multi_failFuncPrevious_printDetailsValue()
    {
        MlResult<int> partialResult = 0;

        
        MlResult<string> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .AddValueDetailIfFail(new DateTime(1, 1, 1))
                                                .BindIfFailWithValue<string, DateTime, string>(funcValid: stringPrevious => stringPrevious,
                                                                                               funcFail : valueDateTime  => valueDateTime.ToString()));

        MlResult<string> expected = new DateTime(1, 1, 1).ToString() ;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryBindIfFailWithValue_Multi_funcValidThrowException_return_fail()
    {
        MlResult<int> partialResult = 1;

        
        MlResult<decimal> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .AddValueDetailIfFail(DateTime.Now))
                                                .TryBindIfFailWithValue<string, DateTime, decimal>(funcValid          : stringPrevious => decimal.Parse(stringPrevious) / 0,
                                                                                                  funcFail           : valueDateTime  => Convert.ToDecimal(valueDateTime.Year),
                                                                                                  errorMessageBuilder: errorsDetails => "Without message !!!");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindIfFailWithValue_Multi_funcValidThrowException_returnFail_has_exDetails()
    {
        MlResult<int> partialResult = 1;

        
        MlResult<decimal> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .AddValueDetailIfFail(DateTime.Now))
                                                .TryBindIfFailWithValue<string, DateTime, decimal>(funcValid          : stringPrevious => decimal.Parse(stringPrevious) / 0,
                                                                                                  funcFail           : valueDateTime  => Convert.ToDecimal(valueDateTime.Year),
                                                                                                  errorMessageBuilder: errorsDetails => "Without message !!!");

        MlResult<decimal> mlResultEx = result.MapIfFail(errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY) ? 1m : 0m);

        MlResult<decimal> expected = 1m;

        mlResultEx.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryBindIfFailWithValue_Multi_funcFailThrowException_returnFail_has_exDetails()
    {
        MlResult<int> partialResult = 0;

        
        MlResult<decimal> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .AddValueDetailIfFail(DateTime.Now))
                                                .TryBindIfFailWithValue<string, DateTime, decimal>(funcValid          : stringPrevious => 99m,
                                                                                                  funcFail           : valueDateTime  => Convert.ToDecimal(valueDateTime.Year / 0),
                                                                                                  errorMessageBuilder: errorsDetails => "Without message !!!");

        MlResult<decimal> mlResultEx = result.MapIfFail(errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY) ? 1m : 0m);

        MlResult<decimal> expected = 1m;

        mlResultEx.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async void TryBindIfFailWithValueAsync_Multi_funcValidThrowException_returnFail_has_exDetails()
    {
        MlResult<int> partialResult = 1;

        
        MlResult<decimal> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                .AddValueDetailIfFailAsync(DateTime.Now))
                                                .TryBindIfFailWithValueAsync<string, DateTime, decimal>(funcValidAsync          : stringPrevious => (decimal.Parse(stringPrevious) / 0)    .ToMlResultValidAsync(),
                                                                                                       funcFailAsync           : valueDateTime  => (Convert.ToDecimal(valueDateTime.Year)).ToMlResultValidAsync(),
                                                                                                       errorMessageBuilder: errorsDetails => "Without message !!!");

        MlResult<decimal> mlResultEx = result.MapIfFail(errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY) ? 1m : 0m);

        MlResult<decimal> expected = 1m;

        mlResultEx.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async void TryBindIfFailWithValueAsync_Multi_funcFailThrowException_returnFail_has_exDetails()
    {
        MlResult<int> partialResult = 0;

        
        MlResult<decimal> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                .AddValueDetailIfFailAsync(DateTime.Now))
                                                .TryBindIfFailWithValueAsync<string, DateTime, decimal>(funcValidAsync          : stringPrevious => 99m.ToMlResultValidAsync(),
                                                                                                       funcFailAsync           : valueDateTime  => Convert.ToDecimal(valueDateTime.Year / 0).ToMlResultValidAsync(),
                                                                                                       errorMessageBuilder: errorsDetails => "Without message !!!");

        MlResult<decimal> mlResultEx = result.MapIfFail(errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY) ? 1m : 0m);

        MlResult<decimal> expected = 1m;

        mlResultEx.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    #endregion



    #region BindIfFailWithException



    [Fact]
    public void BindIfFailWithException_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.BindIfFailWithException(ex => (ex != null! ? 1 : 0).ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void BindIfFailWithException_sourceFailWithDetailsException_provideException()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithException(ex => (ex != null! ? 1 : 0).ToMlResultValid());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void BindIfFailWithException_sourceFailWithoutDetailsException_returnSource()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithException(ex => (ex != null! ? 1 : 0).ToMlResultValid());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceFailWithDetailsException_provideException()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = await partialResult.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceFailWithoutDetailsException_returnSource()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = await partialResult.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }


    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceAsync_sourceValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceAsync_sourceFailWithDetailsException_provideException()
    {
        Task<MlResult<int>> partialResultAsync =  ("miError",
                                                        new Dictionary<string, object>
                                                        {
                                                            { EX_DESC_KEY, new Exception("miException") },
                                                            { "key2", "value2" }
                                                        }
                                                    ).ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindIfFailWithExceptionAsync_sourceAsync_sourceFailWithoutDetailsException_returnSource()
    {
        Task<MlResult<int>> partialResultAsync =  ("miError",
                                                        new Dictionary<string, object>
                                                        {
                                                            { "key2", "value2" }
                                                        }
                                                    ).ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToMlResultValidAsync());

        result.ToString().Should().BeEquivalentTo(partialResultAsync.Result.ToString());
    }



    [Fact]
    public async Task TryBindIfFailWithExceptionAsync_sourceAsync_generalTests()
    {
        int myNumber = 2;

        Task<MlResult<int>> partialResultAsync =  ("miError",
                                                        new Dictionary<string, object>
                                                        {
                                                            { EX_DESC_KEY, new Exception("miException") },
                                                            { "key2", "value2" }
                                                        }
                                                    ).ToMlResultFailAsync<int>();

        var partialResult = await partialResultAsync;

        MlResult<int> result = await partialResultAsync.TryBindIfFailWithExceptionAsync(ex => (myNumber / 0).ToMlResultValidAsync());

        //MlResult<int> expected = await partialResultAsync;

        //result.ToString().Should().BeEquivalentTo(expected.ToString());
    }






    //[Fact]
    //public void BindIfFailWithException_withDetailsException_provideException()
    //{
    //    MlResult<int> partialResult =  ("miError",
    //                                        new Dictionary<string, object>
    //                                        {
    //                                            { EX_DESC_KEY, new Exception("miException") },
    //                                            { "key2", "value2" }
    //                                        }
    //                                    );

    //    MlResult<bool> result = partialResult.BindIfFailWithException(ex => (ex != null).ToMlResultValid()));

    //    result.IsValid.Should().BeTrue();
    //}



    [Fact]
    public void BindIfFailWithException_multi_validSource_returnValidFunc()
    {
        MlResult<int> partialResult = 1;

        MlResult<DateTime> result = partialResult.BindIfFailWithException<int, DateTime>(funcValid: x  => new DateTime(x, x, x),
                                                                                         funcFail : ex => new DateTime(2, 2, 2));

        MlResult<DateTime> expected = new DateTime(1, 1, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindIfFailWithException_multi_failSource_withExceptionDetails_returnFAilFunc()
    {
        MlResult<int> partialResult =  ("miError",
                                                new Dictionary<string, object>
                                                {
                                                    { EX_DESC_KEY, new Exception("miException") },
                                                    { "key2", "value2" }
                                                }
                                            );

        MlResult<DateTime> result = partialResult.BindIfFailWithException<int, DateTime>(funcValid: x  => new DateTime(x, x, x),
                                                                                         funcFail : ex => new DateTime(2, 2, 2));

        MlResult<DateTime> expected = new DateTime(2, 2, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindIfFailWithException_multi_failSource_withoutExceptionDetails_returnFAilFunc()
    {
        MlResult<int> partialResult =  "InitialError".ToMlResultFail<int>();

        MlResult<DateTime> result = partialResult.BindIfFailWithException<int, DateTime>(funcValid: x  => new DateTime(x, x, x),
                                                                                         funcFail : ex => new DateTime(2, 2, 2));

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }








    [Fact]
    public void BindIfFailWithException_genericException_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.BindIfFailWithException<int, InvalidOperationException>(ex => (ex != null! ? 1 : 0).ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void BindIfFailWithException_genericException_sourceValid_invalidExceptionType_NotExecuteFuncException()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new InvalidOperationException("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithException<int, ApplicationException>(ex => 1.ToMlResultValid());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public void BindIfFailWithException_genericException_sourceValid_validExceptionType_ExecuteFuncException()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new InvalidOperationException("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithException<int, InvalidOperationException>(ex => 1.ToMlResultValid());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void BindIfFailWithException_genericException_specificExceptionType_notExecuteExceptionBase()
    {
        MlResult<int> partialResult = ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { EX_DESC_KEY, new InvalidOperationException("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithException<int, InvalidOperationException>(ex => 1.ToMlResultValid())
                                            .BindIfFailWithException<int, Exception                >(ex => 2.ToMlResultValid());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }










    #endregion


    #region BindIfFailWithoutException


    [Fact]
    public void BindIfFailWithoutException_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.BindIfFailWithoutException( _ => 1.ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void BindIfFailWithoutException_sourceFailWithDetailsException_returnSource()
    {
        MlResult<int> partialResult =  ("miError",
                                                       new Dictionary<string, object>
                                                       {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = partialResult.BindIfFailWithoutException( _ => 1.ToMlResultValid());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public void BindIfFailWithoutException_sourceFailWithoutDetailsException_returnValid()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = partialResult.BindIfFailWithoutException( _ => 1.ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void BindIfFailWithoutException_sourceFailWithoutDetailsException_returnFuncParameterResult()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = partialResult.BindIfFailWithoutException( _ => 2.ToMlResultValid());

        MlResult<int> expected = 2;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }




    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceFailWithDetailsException_returnSource()
    {
        MlResult<int> partialResult = ("miError",
                                                                  new Dictionary<string, object>
                                                                  {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                         );

        MlResult<int> result = await partialResult.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }



    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceFailWithoutDetailsException_returnValid()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceFailWithoutDetailsException_returnFuncParameterResult()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.BindIfFailWithoutExceptionAsync(_ => 2.ToMlResultValidAsync());

        MlResult<int> expected = 2;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceAsync_sourceValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceAsync_sourceFailWithDetailsException_returnSource()
    {
        Task<MlResult<int>> partialResultAsync =  ("miError",
                                                                   new Dictionary<string, object>
                                                                   {
                                                            { EX_DESC_KEY, new Exception("miException") },
                                                            { "key2", "value2" }
                                                        }
                                                                                                                      ).ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        MlResult<int> expected = await partialResultAsync;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceAsync_sourceFailWithoutDetailsException_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = "InitialError".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailWithoutExceptionAsync(_ => 1.ToMlResultValidAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_sourceAsync_sourceFailWithoutDetailsException_returnFuncParameterResult()
    {
        Task<MlResult<int>> partialResultAsync = "InitialError".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.BindIfFailWithoutExceptionAsync(_ => 2.ToMlResultValidAsync());

        MlResult<int> expected = 2;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryBindIfFailWithoutException_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => 1.ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryBindIfFailWithoutException_sourceFailWithDetailsException_returnSource()
    {
        MlResult<int> partialResult =  ("miError",
                                                                  new Dictionary<string, object>
                                                                  {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                         );

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => 1.ToMlResultValid());

        result.ToString().Should().BeEquivalentTo(partialResult.ToString());
    }

    [Fact]
    public void TryBindIfFailWithoutException_sourceFailWithoutDetailsException_returnValid()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => 1.ToMlResultValid());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryBindIfFailWithoutException_sourceFailWithoutDetailsException_returnFuncParameterResult()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => 2.ToMlResultValid());

        MlResult<int> expected = 2;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindIfFailWithoutException_sourceFailWithoutDetailsException_funcThrowException_returnFail()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        int initialValue = 2;

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => (initialValue / 0).ToMlResultValid());

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindIfFailWithoutException_sourceFailWithoutDetailsException_funcThrowException_returnFailWithExDetail()
    {
        MlResult<int> partialResult = "InitialError".ToMlResultFail<int>();

        int initialValue = 2;

        MlResult<int> result = partialResult.TryBindIfFailWithoutException(_ => (initialValue / 0).ToMlResultValid());

        bool hasExError = result.Match(
                                            valid: _             => false,
                                            fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                        );

        hasExError.Should().BeTrue();
    }




    [Fact]
    public void BindIfFailWithoutException_Multi_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.BindIfFailWithoutException<int, string>( funcValid: _ => 1.ToString().ToMlResultValid(),
                                                                                         funcFail:  _ => "done !!!".ToMlResultValid());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void BindIfFailWithoutException_Multi_sourceValid_returnFuncValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.BindIfFailWithoutException<int, string>( funcValid: _ => 1.ToString().ToMlResultValid(),
                                                                                         funcFail:  _ => "done !!!".ToMlResultValid());
        MlResult<string> expected = "1";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void BindIfFailWithoutException_Multi_sourceFail_returnFuncFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.BindIfFailWithoutException<int, string>( funcValid: _ => 1.ToString().ToMlResultValid(),
                                                                                         funcFail:  _ => "done !!!".ToMlResultValid());
        MlResult<string> expected = "done !!!";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void BindIfFailWithoutException_Multi_sourceFail_returnValid()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.BindIfFailWithoutException<int, string>( funcValid: _ => 1.ToString().ToMlResultValid(),
                                                                                                    funcFail:  _ => "done !!!".ToMlResultValid());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                    funcFailAsync :  _ => "done !!!".ToMlResultValidAsync());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceValid_returnFuncValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                    funcFailAsync :  _ => "done !!!".ToMlResultValidAsync());
        MlResult<string> expected = "1";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceFail_returnFuncFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = await partialResult.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                    funcFailAsync :  _ => "done !!!".ToMlResultValidAsync());
        MlResult<string> expected = "done !!!";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceFail_returnValid()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = await partialResult.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                    funcFailAsync :  _ => "done !!!".ToMlResultValidAsync());
        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceAsync_sourceValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<string> result = await partialResultAsync.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                         funcFailAsync : _ => "done !!!".ToMlResultValidAsync());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceAsync_sourceValid_returnFuncValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<string> result = await partialResultAsync.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                         funcFailAsync : _ => "done !!!".ToMlResultValidAsync());
        MlResult<string> expected = "1";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task BindIfFailWithoutExceptionAsync_Multi_sourceAsync_sourceFail_returnFuncFail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultAsync.BindIfFailWithoutExceptionAsync<int, string>( funcValidAsync: _ => 1.ToString().ToMlResultValidAsync(),
                                                                                                          funcFailAsync : _ => "done !!!".ToMlResultValidAsync());
        MlResult<string> expected = "done !!!";
        
        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    #endregion



    #region TryBindBuild

    [Fact]
    public void TryBindBuild_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1,
                                                                              x_name => "Name",
                                                                              x_date => date);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuild_sourceFail_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1,
                                                                              x_name => "Name",
                                                                              x_date => date);
        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public void TryBindBuild_sourceValid_allSimpleParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1,
                                                                              x_name => "Name",
                                                                              x_date => date);

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuild_sourceValid_allResultParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1     .ToMlResultValid(),
                                                                              x_name => "Name".ToMlResultValid(),
                                                                              x_date => date  .ToMlResultValid());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuild_sourceValid_mixResultParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1,
                                                                              x_name => "Name".ToMlResultValid(),
                                                                              x_date => date  .ToMlResultValid());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuild_sourceValid_allResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                              x_name => "Error".ToMlResultFail<string>().ToMlResultObject(),
                                                                              x_date => date.ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuild_sourceValid_allResultParams_oneFuncFail_returnFail_with2Errors()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                              x_name => "Error_Name".ToMlResultFail<string>()  .ToMlResultObject(),
                                                                              x_date => "Error_Date".ToMlResultFail<DateTime>().ToMlResultObject());
        var errors = result.SecureFailErrorsDetails().Errors.Count();

        errors.Should().Be(2);
    }

    [Fact]
    public void TryBindBuild_sourceValid_minusResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                              x_name => "Error".ToMlResultFail<string>().ToMlResultObject());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuild_sourceValid_mayorResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuild<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                              x_name => "Error".ToMlResultFail<string>().ToMlResultObject(),
                                                                              x_date => date.ToMlResultValid(),
                                                                              x_date => date.ToMlResultValid());

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindBuildAsync_sourceValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1     .ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_name => "Name".ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_date => date  .ToMlResultValidAsync().ToMlResultObjectAsync());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1     .ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_name => "Name".ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_date => date  .ToMlResultValidAsync().ToMlResultObjectAsync());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceValid_allSimpleParams_allFuncValids_returnValidData()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1     .ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_name => "Name".ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_date => date  .ToMlResultValidAsync().ToMlResultObjectAsync());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryBindBuildAsync_sourceValid_allResultParams_allFuncValids_returnValidData()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1     .ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_name => "Name".ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_date => date  .ToMlResultValidAsync().ToMlResultObjectAsync());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceValid_mixResultParams_allFuncValids_returnValidData()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1     .ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_name => "Name".ToMlResultValidAsync().ToMlResultObjectAsync(),
                                                                                              x_date => date  .ToMlResultValidAsync().ToMlResultObjectAsync());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceValid_allResultParams_oneFuncFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1      .ToMlResultValidAsync()       .ToMlResultObjectAsync(),
                                                                                              x_name => "Error".ToMlResultFailAsync<string>().ToMlResultObjectAsync(),
                                                                                              x_date => date   .ToMlResultValidAsync()       .ToMlResultObjectAsync());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceValid_minusResultParams_oneFuncFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1      .ToMlResultValidAsync()       .ToMlResultObjectAsync(),
                                                                                              x_name => "Error".ToMlResultFailAsync<string>().ToMlResultObjectAsync());

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildAsync_sourceValid_mayorResultParams_oneFuncFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1      .ToMlResultValidAsync()       .ToMlResultObjectAsync(),
                                                                                              x_name => "Error".ToMlResultFailAsync<string>().ToMlResultObjectAsync(),
                                                                                              x_date => date   .ToMlResultValidAsync()       .ToMlResultObjectAsync(),
                                                                                              x_date => date   .ToMlResultValidAsync()       .ToMlResultObjectAsync());
        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindBuildAsync_sourceValid_paramsMlResultAndNotMlResul_allParamsValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1                            .ToMlResultObjectAsync(),
                                                                                              x_name => BindTestMethodAsync(x_name)  .ToMlResultObjectAsync(),
                                                                                              x_date => date.  ToMlResultValidAsync().ToMlResultObjectAsync());
        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task TryBindBuildAsync_sourceValid_paramsMlResultAndNotMlResult_paramsValidAndNotValid_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 0.ToMlResultValidAsync();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = await partialResultAsync.TryBindBuildAsync<int, TestType>(x_id   => 1.ToMlResultObjectAsync(),
                                                                                              x_name => BindTestMethodAsync(x_name).ToMlResultObjectAsync(), //Fail
                                                                                              x_date => date.ToMlResultValidAsync().ToMlResultObjectAsync());
        result.IsFail.Should().BeTrue();
    }




    #endregion


    #region TryBindBuildTuple_Tuple


    [Fact]
    public void TryBindBuildTuple_Tuple_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.TryBindBuildTuple(x_id   => 1     .ToMlResultValid(),
                                                                    x_name => "Name".ToMlResultValid());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildTuple_Tuple_sourceFail_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = partialResult.TryBindBuildTuple(x_id => 1.ToMlResultValid(),
                                                                    x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildTuple_Tuple_sourceValid_withAnyFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.TryBindBuildTuple(x_id   => "Error_force".ToMlResultFail<int>(),
                                                                    x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildTuple_Tuple_sourceValid_returnValidData()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.TryBindBuildTuple(x_id   => 1.ToMlResultValid(),
                                                                    x_name => "Name".ToMlResultValid());

        MlResult<(int, string)> expected = (1, "Name");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    [Fact]
    public async Task TryBindBuildTupleAsync_Tuple_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.TryBindBuildTupleAsync(x_id   => 1     .ToMlResultValid(),
                                                                               x_name => "Name".ToMlResultValid());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_Tuple_sourceFail_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = await partialResult.TryBindBuildTupleAsync(x_id => 1.ToMlResultValid(),
                                                                               x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_Tuple_sourceValid_withAnyFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.TryBindBuildTupleAsync(x_id   => "Error_force".ToMlResultFail<int>(),
                                                                               x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_Tuple_sourceValid_returnValidData()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValid(),
                                                                               x_name => "Name".ToMlResultValid());

        MlResult<(int, string)> expected = (1, "Name");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValid_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValid(),
                                                                                    x_name => "Name".ToMlResultValid());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValid(),
                                                                                    x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValid_withAnyFuncFail_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => "Error_force".ToMlResultFail<int>(),
                                                                                    x_name => "Name".ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValid_returnValidData()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValid(),
                                                                                    x_name => "Name".ToMlResultValid());

        MlResult<(int, string)> expected = (1, "Name");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }







    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValidAsync_returnValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValidAsync(),
                                                                                    x_name => "Name".ToMlResultValidAsync());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceFailAsync_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValidAsync(),
                                                                                    x_name => "Name".ToMlResultValidAsync());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValid_withAnyFuncFailAsync_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => "Error_force".ToMlResultFailAsync<int>(),
                                                                                    x_name => "Name".ToMlResultValidAsync());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task TryBindBuildTupleAsync_sourceAsync_Tuple_sourceValidAsync_returnValidData()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.TryBindBuildTupleAsync(x_id   => 1.ToMlResultValidAsync(),
                                                                                    x_name => "Name".ToMlResultValidAsync());

        MlResult<(int, string)> expected = (1, "Name");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void TryBindBuildTuple_8_parameters_Tuple_sourceValid_returnValidData()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string, string, string, string, string, string, decimal)> result = partialResult.TryBindBuildTuple(x_id      => 1.ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_name    => "Name".ToMlResultValid(),
                                                                                                                     x_decimal => 9m.ToMlResultValid<decimal>());

        MlResult<(int, string, string, string, string, string, string, decimal)> expected = (1, "Name", "Name", "Name", "Name", "Name", "Name", 9m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    #endregion







    #region TryBindBuildWhile


    [Fact]
    public void TryBindBuildWhile_sourceValid_returnValid()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1,
                                                                                x_name => "Name",
                                                                                x_date => date);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildWhile_sourceFail_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1,
                                                                                x_name => "Name",
                                                                                x_date => date);
        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public void TryBindBuildWhile_sourceValid_allSimpleParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1,
                                                                                x_name => "Name",
                                                                                x_date => date);

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_allResultParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1     .ToMlResultValid(),
                                                                                x_name => "Name".ToMlResultValid(),
                                                                                x_date => date  .ToMlResultValid());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_mixResultParams_allFuncValids_returnValidData()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1,
                                                                                x_name => "Name".ToMlResultValid(),
                                                                                x_date => date  .ToMlResultValid());

        MlResult<TestType> expected = new TestType(1, "Name", date);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_allResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                                x_name => "Error".ToMlResultFail<string>().ToMlResultObject(),
                                                                                x_date => date.ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_minusResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                                x_name => "Error".ToMlResultFail<string>().ToMlResultObject());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_mayorResultParams_oneFuncFail_returnFail()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                                x_name => "Error".ToMlResultFail<string>().ToMlResultObject(),
                                                                                x_date => date.ToMlResultValid(),
                                                                                x_date => date.ToMlResultValid());
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryBindBuildWhile_sourceValid_allResultParams_oneFuncFail_returnFail_with2Errors()
    {
        MlResult<int> partialResult = 1;

        var date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        MlResult<TestType> result = partialResult.TryBindBuildWhile<int, TestType>(x_id   => 1.ToMlResultValid(),
                                                                                x_name => "Error_Name".ToMlResultFail<string>().ToMlResultObject(),
                                                                                x_date => "Error_Date".ToMlResultFail<DateTime>().ToMlResultObject());
        var errors = result.SecureFailErrorsDetails().Errors.Count();

        errors.Should().Be(1);
    }

    #endregion







    private MlResult<string> BindTestMethod(int x)
        => x != 0 ? x.ToString()  : "Not OK".ToMlResultFail<string>();

    private Task<MlResult<string>> BindTestMethodAsync(int x)
        => x != 0 ? x.ToString().ToMlResultValidAsync() : "Not OK".ToMlResultFailAsync<string>();

    private MlResult<int> BindTestMethodInt(int x)
        => x != 0 ? x  : "Not OK".ToMlResultFail<int>();

    private Task<MlResult<int>> BindTestMethodAIntsync(int x)
        => BindTestMethodInt(x).ToAsync();


    private MlResult<int> BindTestMethodIntThrowException(int x)
        => x != 0 ? x : throw new Exception("x not valid");  

    private Task<MlResult<int>> BindTestMethodIntThrowExceptionAsync(int x)
        => BindTestMethodIntThrowException(x).ToAsync();

    private MlResult<string> BindTestMethodStringThrowException(int x)
        => x != 0 ? "Good !!!" : throw new Exception("x not valid");  

    private Task<MlResult<string>> BindTestMethodStringThrowExceptionAsync(int x)
        => BindTestMethodStringThrowException(x).ToAsync();
}




