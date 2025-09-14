using MoralesLarios.OOFP.Types.Errors;
using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultActionsMapTests
{


    #region Map


    [Fact]
    public async Task TryMapAsync_withException_return_fail()
    {
        MlResult<int> partialResult = 12;

        decimal divisor = 0;

        MlResult<decimal> result = await partialResult.TryMapAsync(x =>  (x / divisor).ToAsync());

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task TryMapAsync_colec_withException_return_fail()
    {
        MlResult<IEnumerable<int>> partialResult = new List<int>() { 1, 3, 4 }.AsEnumerable().ToMlResultValid();

        decimal divisor = 0;

        MlResult<IEnumerable<decimal>> result = await partialResult.TryMapAsync(x => x.Select(y => (y / divisor)).ToList().AsEnumerable().ToAsync());

        result.IsFail.Should().BeTrue();
    }






    #endregion






    #region MapComp


    [Fact]
    public void MapComp_validSource_compOK_return_compFuncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapEnsure(x => x > 0, string.Empty);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void MapComp_failSource_return_errorDetailsResult()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.MapEnsure(x => x > 0, string.Empty);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void MapComp_validSource_compNoOK_return_errorRsult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapEnsure(x => x > 2, "Not good comparation");

        MlResult<int> expected = "Not good comparation".ToMlResultFail<int>();

        result.ToString().Should().Be(expected.ToString());
    }



    [Fact]
    public async Task MapCompAsync_validSource_compOK_return_compFuncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapEnsureAsync(x => x > 0, string.Empty);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task MapCompAsync_failSource_return_errorDetailsResult()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.MapEnsureAsync(x => x > 0, string.Empty);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task MapCompAsync_validSource_compNoOK_return_errorRsult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapEnsureAsync(x => x > 2, "Not good comparation");

        MlResult<int> expected = "Not good comparation".ToMlResultFail<int>();

        result.ToString().Should().Be(expected.ToString());
    }






    [Fact]
    public async Task MapCompAsync_sourceAsync_validSource_compOK_return_compFuncResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapEnsureAsync(x => x > 0, string.Empty);

        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task MapCompAsync_sourceAsync_failSource_return_errorDetailsResult()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.MapEnsureAsync(x => x > 0, string.Empty);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task MapCompAsync_sourceAsync_validSource_compNoOK_return_errorRsult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapEnsureAsync(x => x > 2, "Not good comparation");

        MlResult<int> expected = "Not good comparation".ToMlResultFail<int>();

        result.ToString().Should().Be(expected.ToString());
    }


    #endregion





    #region MapIfFail



    [Fact]
    public void MapIfFail_validSource_return_isValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapIfFail(x => x.Errors.Count());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void MapIfFail_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapIfFail(x => x.Errors.Count());

        result.ToString().Should().Be(partialResult.ToString());
    }


    [Fact]
    public void MapIfFail_FailSource_return_fail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.MapIfFail(x => x.Errors.Count());
        
        result.IsFail.Should().BeFalse();
    }


    [Fact]
    public void MapIfFail_FailSource_return_resultFuncValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.MapIfFail(x => x.Errors.Count());

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());

    }




    [Fact]
    public async Task MapIfFailAsync_validSource_return_isValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MapIfFailAsync_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        result.ToString().Should().Be(partialResult.ToString());
    }


    [Fact]
    public async Task MapIfFailAsync_FailSource_return_fail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        result.IsFail.Should().BeFalse();
    }


    [Fact]
    public async Task MapIfFailAsync_FailSource_return_resultFuncValue()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }



    [Fact]
    public async Task MapIfFailAsync_sourceAsync_validSource_return_isValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task MapIfFailAsync_sourceAsync_validSource_return_source()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().Be(partialResult.ToString());
    }


    [Fact]
    public async Task MapIfFailAsync_sourceAsync_FailSource_return_fail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        result.IsFail.Should().BeFalse();
    }


    [Fact]
    public async Task MapIfFailAsync_sourceAsync_FailSource_return_resultFuncValue()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.MapIfFailAsync(x => x.Errors.Count().ToAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }



    [Fact]
    public void MapIfFail_diferentReturnValue_failSource_ExecuteFuncFail()
    {

        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.MapIfFail<int, string>(funcValid: x             => x.ToString(),
                                                                        funcFail : errorsDetails => errorsDetails.Errors.First().ToString());

        MlResult<string> expected = "Error";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void MapIfFail_diferentReturnValue_validSource_ExecuteFuncValid()
    {

        MlResult<int> partialResult = -1;

        MlResult<string> result = partialResult.MapIfFail<int, string>(funcValid: x             => x.ToString(),
                                                                        funcFail : errorsDetails => errorsDetails.Errors.First().ToString());

        MlResult<string> expected = "-1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async Task MapIfFailAsync_sourceAsync_validSourceAsync_failSource_return_source()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfFailAsync(x => x.Errors.Count());
        
        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task MapIfFailAsync_diferentReturnValue_validSourceAsync_failSource_ExecuteFuncFail()
    {

        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResultAsync.MapIfFailAsync<int, string>(funcValidAsync: x             => x.ToString().ToAsync(),
                                                                                       funcFail      : errorsDetails => errorsDetails.Errors.First().ToString());

        MlResult<string> expected = "Error";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    #endregion



    #region TryMapIfFail


    [Fact]
    public void TryMapIfFail_failSource_funcThrowException_returnFail()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryMapIfFail(errorsDetails => MapTestMethodAIntThrowException(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public void TryMapIfFail_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryMapIfFail(errorsDetails => MapTestMethodAIntThrowException(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public void TryMapIfFail_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.TryMapIfFail(errorsDetails => MapTestMethodAIntThrowException(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }





    

    [Fact]
    public async Task TryMapIfFailAsync_failSource_funcThrowException_returnFail()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public async Task TryMapIfFailAsync_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task TryMapIfFailAsync_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        MlResult<int> partialResult = "Initial Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }




    
    

    [Fact]
    public async Task TryMapIfFailAsync_sourceAsync_failSource_funcThrowException_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(0), "Other error");

        result.IsFail.Should().BeTrue(); 
    }

    [Fact]
    public async Task TryMapIfFailAsync_sourceAsync_failSource_funcThrowException_returnMlResultWithExDetails()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(0), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task TryMapIfFailAsync_sourceAsync_failSource_funcNotThrowException_returnMlResultWithoutExDetails()
    {
        Task<MlResult<int>> partialResultAsync = "Initial Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.TryMapIfFailAsync(errorsDetails => MapTestMethodAIntThrowExceptionAsync(1), "Other error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeFalse();
    }






    #endregion



    #region MapIfFailWithException



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

        MlResult<int> result = await partialResult.MapIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async Task MapIfFailWithExceptionAsync_sourceFailWithoutDetailsException_returnSource()
    {
        MlResult<int> partialResult =  ("miError",
                                            new Dictionary<string, object>
                                            {
                                                { "key2", "value2" }
                                            }
                                        );

        MlResult<int> result = await partialResult.MapIfFailWithExceptionAsync(ex => (ex != null! ? 1 : 0).ToAsync());

        MlResult<int> expected = (new MlError[] { "miError" , "The key Ex does not exist in the details" },
                                    new Dictionary<string, object>
                                    {
                                        { "key2", "value2" }
                                    }
                                );

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    #endregion


    #region MapIf

    [Fact]
    public void MapIf_diferentTypes_validSource_compTrue_return_funcTrueResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.MapIf(condition : x => x > 0,
                                                       funcTrue : x =>  "1",
                                                       funcFalse: x => "-1");

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void MapIf_diferentTypes_validSource_compFalse_return_funcFalseResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.MapIf(condition : x => x > 1,
                                                       funcTrue : x =>  "1",
                                                       funcFalse: x => "-1");

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void MapIf_validSource_compTrue_return_funcTrueResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapIf(condition : x => x > 0,
                                                   func      : x =>  2);
        MlResult<int> expected = 2;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void MapIf_validSource_compFalse_return_funcFalseResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.MapIf(condition : x => x > 1,
                                                   func      : x =>  2);

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }



    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compTrue_return_funcTrueResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition : x => x > 0,
                                                                 funcTrue : x =>  "1",
                                                                 funcFalse: x => "-1");

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compFalse_return_funcFalseResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition : x => x > 1,
                                                                 funcTrue  : x =>  "1",
                                                                 funcFalse : x => "-1");

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compTrue_return_funcTrueAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition     : x => x > 0,
                                                                 funcTrueAsync : x =>  "1".ToAsync(),
                                                                 funcFalse     : x => "-1");

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compFalse_return_funcFalseAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition      : x => x > 1,
                                                                 funcTrue       : x =>  "1",
                                                                 funcFalseAsync : x => "-1".ToAsync());

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compTrueAsync_return_funcTrueAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition     : x => x > 0,
                                                                 funcTrueAsync : x =>  "1".ToAsync(),
                                                                 funcFalseAsync: x => "-1".ToAsync());

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_diferentTypes_validSource_compFalseAsync_return_funcFalseAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.MapIfAsync(condition      : x => x > 1,
                                                                 funcTrueAsync  : x =>  "1".ToAsync(),
                                                                 funcFalseAsync : x => "-1".ToAsync());

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task MapIfAsync_sourceAsync_diferentTypes_validSource_compTrueAsync_return_funcTrueAsyncResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<string> result = await partialResultAsync.MapIfAsync(condition     : x => x > 0,
                                                                      funcTrueAsync : x =>  "1".ToAsync(),
                                                                      funcFalseAsync: x => "-1".ToAsync());

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_sourceAsync_diferentTypes_validSource_compFalseAsync_return_funcFalseAsyncResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<string> result = await partialResultAsync.MapIfAsync(condition      : x => x > 1,
                                                                      funcTrueAsync  : x =>  "1".ToAsync(),
                                                                      funcFalseAsync : x => "-1".ToAsync());

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }









    [Fact]
    public async Task MapIfAsync_validSource_compTrue_return_funcTrueResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfAsync(condition: x => x > 0,
                                                              func     : x => 2);

        MlResult<int> expected = 2;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_validSource_compFalse_return_funcFalseResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfAsync(condition : x => x > 1,
                                                              func      : x =>  2);

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_sourceAsync_validSource_compTrue_return_funcTrueResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfAsync(condition: x => x > 0,
                                                                   func     : x => 2);

        MlResult<int> expected = 2;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_sourceAsync_validSource_compFalse_return_funcFalseResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfAsync(condition : x => x > 1,
                                                                   func      : x =>  2);

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task MapIfAsync_validSource_compTrue_return_funcTrueAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfAsync(condition: x => x > 0,
                                                              funcAsync: x => 2.ToAsync());

        MlResult<int> expected = 2;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_validSource_compFalse_return_funcFalseAsyncResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.MapIfAsync(condition : x => x > 1,
                                                              funcAsync : x =>  2.ToAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_sourceAsync_validSource_compTrue_return_funcTrueAsyncResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfAsync(condition: x => x > 0,
                                                                   funcAsync: x => 2.ToAsync());

        MlResult<int> expected = 2;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MapIfAsync_sourceAsync_validSource_compFalse_return_funcFalseAsyncResult()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<int> result = await partialResultAsync.MapIfAsync(condition : x => x > 1,
                                                                   funcAsync : x =>  2.ToAsync());

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }



  [Fact]
    public void TryMapIf_diferentTypes_validSource_compTrue_return_funcTrueResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.TryMapIf(condition : x => x > 0,
                                                         funcTrue  : x =>  "1",
                                                         funcFalse : x => "-1");

        MlResult<string> expected = "1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void TryMapIf_diferentTypes_validSource_compFalse_return_funcFalseResult()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.TryMapIf(condition : x => x > 1,
                                                         funcTrue  : x =>  "1",
                                                         funcFalse : x => "-1");

        MlResult<string> expected = "-1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void TryMapIf_diferentTypes_failSource_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.TryMapIf(condition : x => x > 1,
                                                         funcTrue : x =>  "1",
                                                         funcFalse: x => "-1");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryMapIf_diferentTypes_validSource_compTrue_funcTrueResultThrowException_returnFail()
    {
        MlResult<int> partialResult = 1;

        int divisor = 0;

        MlResult<string> result = partialResult.TryMapIf(condition : x => x > 0,
                                                         funcTrue  : x =>  (x / divisor).ToString(),
                                                         funcFalse : x => "-1");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void TryMapIf_diferentTypes_validSource_compTrue_funcFalseResultThrowException_returnFail()
    {
        MlResult<int> partialResult = -1;

        int divisor = 0;

        MlResult<string> result = partialResult.TryMapIf(condition : x => x > 0,
                                                         funcTrue  : x => "1",
                                                         funcFalse : x => (x / divisor).ToString());

        result.IsFail.Should().BeTrue();
    }








    #endregion





    //[Fact]
    //public void TestsVarious()
    //{

    //    MlResult<int> partialResult = 1;

    //    MlResult<string> result = partialResult.Map()





    //}



    private int MapTestMethodAIntThrowException(int x)
        => x != 0 ? x : throw new Exception("x not valid");  

    private Task<int> MapTestMethodAIntThrowExceptionAsync(int x)
        => MapTestMethodAIntThrowException(x).ToAsync();



}
