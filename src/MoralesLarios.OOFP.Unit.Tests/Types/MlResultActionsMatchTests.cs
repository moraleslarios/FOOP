namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultActionsMatchTests
{

    /// ********************************************************************************************************
    ///                         Faltarían todos los métoso normales
    /// ********************************************************************************************************

















    [Fact]
    public async Task MatchAsync_sourceAsync_validAndfailSync_sourceValid_OK()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        MlResult<int> result = await source.MatchAsync(valid: x      => 12,
                                                       fail : errors => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task MatchAsync_sourceAsync_validAndfailSync_sourceFail_OK()
    {
        Task<MlResult<int>> source = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await source.MatchAsync(valid: x      => 12,
                                                       fail : errors => 13);

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async Task TryMatchAsync_sourceAsync_validAndfailSync_sourceValid_withoutException_OK()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        MlResult<int> result = await source.TryMatchAsync(valid: x      => 12,
                                                          fail : errors => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_validAndfailSync_sourceFail_withoutException_OK()
    {
        Task<MlResult<int>> source = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await source.TryMatchAsync(valid: x      => 12,
                                                          fail : errors => 13);

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_validAndfailSync_sourceValid_withException_OK()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(valid: x      => 12/divisor,
                                                          fail : errors => 13);

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_validAndfailSync_sourceFail_withException_OK()
    {
        Task<MlResult<int>> source = "Error".ToMlResultFailAsync<int>();

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(valid: x      => 12,
                                                          fail : errors =>  (13 / divisor));

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }









    #region Match




    [Fact]
    public void Match_simple_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = source.Match(() => 12);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void Match_simple_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = source.Match(() => 12);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_simple_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.MatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_simple_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = await source.MatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_sourceAysnc_simple_valid_OK()
    {
        Task<MlResult<int>> sourceAsync = 11.ToMlResultValidAsync();

        MlResult<int> result = await sourceAsync.MatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_sourceAsync_simple_fail_OK()
    {
        Task<MlResult<int>> source = "error".ToMlResultFailAsync<int>();

        MlResult<int> result = await source.MatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void Match_valid_execute_valid()
    {
        MlResult<int> source = 11;

        MlResult<int> result = source.Match(valid: x            => x + 1,
                                              fail : errorDetails => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void Match_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = source.Match(valid: x            => x + 1,
                                              fail : errorDetails => 13);

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task MatchAsync_valid_execute_valid()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.MatchAsync(validAsync: x            => (x + 1).ToAsync(),
                                                         failAsync : errorDetails => 13.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task  MatchAsync_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = await source.MatchAsync(validAsync: x            => (x + 1).ToAsync(),
                                                         failAsync : errorDetails => 13.ToAsync());

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_sourceAsync_valid_execute_valid()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        MlResult<int> result = await source.MatchAsync(validAsync: x            => (x + 1).ToAsync(),
                                                         failAsync : errorDetails => 13.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task  MatchAsync_sourceAsync_fail_OK()
    {
        Task<MlResult<int>> source = "error".ToMlResultFailAsync<int>();

        MlResult<int> result = await source.MatchAsync(validAsync: x            => (x + 1).ToAsync(),
                                                         failAsync : errorDetails => 13.ToAsync());

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }




    //[Fact]
    //public void Match_changeValue_simple_valid_OK()
    //{
    //    MlResult<int> source = 11;

    //    MlResult<string> result = source.MakeMapChangeTypeResult(() => "12");

    //    MlResult<string> expected = "12";

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}

    //[Fact]
    //public void Match_changeValue_simple_fail_OK()
    //{
    //    MlResult<int> source = "error".ToMlResultFail<int>();

    //    MlResult<string> result = source.MakeMapChangeTypeResult(() => "12");

    //    MlResult<string> expected = "12";

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}

    //[Fact]
    //public async Task MakeMapAsyncChangeValueReturn_simple_valid_OK()
    //{
    //    MlResult<int> source = 11;

    //    MlResult<int> result = await source.MakeMapAsync(() => 12.ToAsync());

    //    MlResult<int> expected = 12;

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}

    //[Fact]
    //public async Task MakeMapAsyncChangeValueReturn_simple_fail_OK()
    //{
    //    MlResult<int> source = "error".ToMlResultFail<int>();

    //    MlResult<string> result = await source.MakeMapChangeTypeResultAsync(() => "12".ToAsync());

    //    MlResult<string> expected = "12";

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}

    //[Fact]
    //public async Task MakeMapAsyncChangeValueReturn_sourceAysnc_simple_valid_OK()
    //{
    //    Task<MlResult<int>> sourceAsync = 11.ToMlResultValidAsync();

    //    MlResult<string> result = await sourceAsync.MakeMapChangeTypeResultAsync(() => "12".ToAsync());

    //    MlResult<string> expected = "12";

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}

    //[Fact]
    //public async Task MakeMapAsyncChangeValueReturn_sourceAsync_simple_fail_OK()
    //{
    //    Task<MlResult<int>> sourceAsync = "error".ToMlResultFailAsync<int>();

    //    MlResult<string> result = await sourceAsync.MakeMapChangeTypeResultAsync(() => "12".ToAsync());

    //    MlResult<string> expected = "12";

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}






    [Fact]
    public void Match_compose_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = source.Match(valid: x      => (x + 1).ToString(),
                                                                  fail : errors => errors.Errors.Count().ToString());
        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void Match_compose_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = source.Match(valid: x      => (x + 1).ToString(),
                                                                  fail : errors => errors.Errors.Count().ToString());
        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = await source.MatchAsync(validAsync: x      => (x + 1).ToString().ToAsync(),
                                                                             failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_compose_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = await source.MatchAsync(validAsync: x      => (x + 1).ToString().ToAsync(),
                                                                             failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_sourceAysnc_valid_OK()
    {
        Task<MlResult<int>> sourceAsync = 11.ToMlResultValidAsync();

        MlResult<string> result = await sourceAsync.MatchAsync(validAsync: x      => (x + 1).ToString().ToAsync(),
                                                                                  failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_compose_sourceAsync_fail_OK()
    {
        Task<MlResult<int>> sourceAsync = "error".ToMlResultFailAsync<int>();

        MlResult<string> result = await sourceAsync.MatchAsync(validAsync: x      => (x + 1).ToString().ToAsync(),
                                                                                  failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }









    [Fact]
    public void TryMatch_simple_valid_withoutException_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = source.TryMatch(() => 12);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryMatch_simple_valid_withoException_OK()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<int> result = source.TryMatch(() => 12/divisor, "Test en error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public void TryMatch_simple_fail_withoutException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = source.TryMatch(() => 12);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryMatch_simple_fail_withoException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<int> result = source.TryMatch(() => 12/divisor, "Test en error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task TryMatchAsync_simple_valid_withoutException_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.TryMatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_simple_valid_withoException_OK()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(() => (12/divisor).ToAsync(), "Test en error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_simple_valid_withoutException_OK()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        MlResult<int> result = await source.TryMatchAsync(() => 12.ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_simple_valid_withoException_OK()
    {
        Task<MlResult<int>> source = 11.ToMlResultValidAsync();

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(() => (12/divisor).ToAsync(), "Test en error");

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public void TryMatch_valid_withoutException_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = source.TryMatch(valid: x => x + 1,
                                                 fail : errorsDetails => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryMatch_valid_withException_OK()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<int> result = source.TryMatch(valid: x => (x + 1) / divisor,
                                                 fail : errorsDetails => 13);

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }



    [Fact]
    public void TryMatch_fail_withoutException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = source.TryMatch(valid: x => x + 1,
                                                 fail : errorsDetails => 13);

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void TryMatch_fail_withException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<int> result = source.TryMatch(valid: x => x + 1,
                                                 fail : errorsDetails => 13 / divisor);

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }





    [Fact]
    public async Task TryMatchAsync_valid_withoutException_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.TryMatchAsync(validAsync: x             => (x + 1).ToAsync(),
                                                            failAsync : errorsDetails => 13     .ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_valid_withException_OK()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(validAsync: x             => (x + 1 / divisor).ToAsync(),
                                                            failAsync : errorsDetails => 13               .ToAsync());

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }



    [Fact]
    public async Task TryMatchAsync_fail_withoutException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<int> result = await source.TryMatchAsync(validAsync: x             => (x + 1).ToAsync(),
                                                            failAsync : errorsDetails => 13     .ToAsync());

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_fail_withException_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(validAsync: x             => (x + 1       ).ToAsync(),
                                                            failAsync : errorsDetails => (13 / divisor).ToAsync());

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_valid_withoutException_OK()
    {
        Task<MlResult<int>> sourceAsync = 11.ToMlResultValidAsync();

        MlResult<int> result = await sourceAsync.TryMatchAsync(validAsync: x             => (x + 1).ToAsync(),
                                                                 failAsync : errorsDetails => 13     .ToAsync());

        MlResult<int> expected = 12;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_sourceAsync_valid_withException_OK()
    {
        Task<MlResult<int>> sourceAsync = 11.ToMlResultValidAsync();

        var divisor = 0;

        MlResult<int> result = await sourceAsync.TryMatchAsync(validAsync: x             => (x + 1 / divisor).ToAsync(),
                                                                 failAsync : errorsDetails => 13               .ToAsync());

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }



    [Fact]
    public async Task TryMatchAsync_sourceAsync_fail_withoutException_OK()
    {
        Task<MlResult<int>> sourceAsync = "error".ToMlResultFailAsync<int>();

        MlResult<int> result = await sourceAsync.TryMatchAsync(validAsync: x             => (x + 1).ToAsync(),
                                                                 failAsync : errorsDetails => 13     .ToAsync());

        MlResult<int> expected = 13;

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task TryMatchAsync_sourceAsync_fail_withException_OK()
    {
        Task<MlResult<int>> sourceAsync = "error".ToMlResultFailAsync<int>();

        var divisor = 0;

        MlResult<int> result = await sourceAsync.TryMatchAsync(validAsync: x             => (x + 1       ).ToAsync(),
                                                                 failAsync : errorsDetails => (13 / divisor).ToAsync());

        bool hasErrors = result.Match(
                                        valid: x             => false,
                                        fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                      );     

        hasErrors.Should().BeTrue();
    }





    [Fact]
    public void TryMatch_simple_withoutException_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = source.TryMatch<int, string>(funcAll: () => 12.ToString());

        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryMatch_simple_withException_return_failWithEx()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<string> result = source.TryMatch<int, string>(funcAll: () => (12 / divisor).ToString());

        bool hasErrors = result.Match(
                                valid: x             => false,
                                fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public void TryMatch_simple_withoutException_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = source.TryMatch<int, string>(funcAll: () => 12.ToString());

        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryMatch_simple_withException_fail_failWithEx()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<string> result = source.TryMatch<int, string>(funcAll: () => (12 / divisor).ToString());

        bool hasErrors = result.Match(
                                valid: x             => false,
                                fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );     

        hasErrors.Should().BeTrue();
    }




    [Fact]
    public async Task TryMatchAsync_simple_withoutException_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = await source.TryMatchAsync(funcAllAsync: () => 12.ToString().ToAsync());

        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_simple_withException_return_failWithEx()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<string> result = await source.TryMatchAsync(funcAllAsync: () => (12 / divisor).ToString().ToAsync());

        bool hasErrors = result.Match(
                                valid: x             => false,
                                fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );     

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task TryMatchAsync_simple_withoutException_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = await source.TryMatchAsync(funcAllAsync: () => 12.ToString().ToAsync());

        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_simple_withException_fail_failWithEx()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<string> result = await source.TryMatchAsync(funcAllAsync: () => (12 / divisor).ToString().ToAsync());

        bool hasErrors = result.Match(
                                valid: x             => false,
                                fail : errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );     

        hasErrors.Should().BeTrue();
    }









    [Fact]
    public void TryMatch_withoutException_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = source.TryMatch(valid: x => (x + 1).ToString(),
                                                                    fail : errors => errors.Errors.Count().ToString());
        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryMatch_withException_valid_return_failWithEx()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<string> result = source.TryMatch(valid: x => (x + 1 / divisor).ToString(),
                                                                    fail: errors => errors.Errors.Count().ToString());
        bool hasErrors = result.Match(
                                valid: x => false,
                                fail: errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public void TryMatch_withoutException_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = source.TryMatch(valid: x => (x + 1).ToString(),
                                                                    fail : errors => errors.Errors.Count().ToString());
        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void TryMatch_withException_fail_return_failWithEx()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<string> result = source.TryMatch(valid: x => (x + 1).ToString(),
                                                                    fail: errors => (errors.Errors.Count() / divisor).ToString());
        bool hasErrors = result.Match(
                                valid: x => false,
                                fail: errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );

        hasErrors.Should().BeTrue();
    }




    [Fact]
    public async Task TryMatchAsync_withoutException_valid_OK()
    {
        MlResult<int> source = 11;

        MlResult<string> result = await source.TryMatchAsync(validAsync: x => (x + 1).ToString().ToAsync(),
                                                                                failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "12";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_withException_valid_return_failWithEx()
    {
        MlResult<int> source = 11;

        var divisor = 0;

        MlResult<string> result = await source.TryMatchAsync(validAsync: x => (x + 1 / divisor).ToString().ToAsync(),
                                                                               failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        bool hasErrors = result.Match(
                                valid: x => false,
                                fail: errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task TryMatchAsync_withoutException_fail_OK()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        MlResult<string> result = await source.TryMatchAsync(validAsync: x => (x + 1).ToString().ToAsync(),
                                                                               failAsync : errors => errors.Errors.Count().ToString().ToAsync());
        MlResult<string> expected = "1";

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_withException_fail_return_failWithEx()
    {
        MlResult<int> source = "error".ToMlResultFail<int>();

        var divisor = 0;

        MlResult<string> result = await source.TryMatchAsync(validAsync: x => (x + 1).ToString().ToAsync(),
                                                                               failAsync : errors => (errors.Errors.Count() / divisor).ToString().ToAsync());
        bool hasErrors = result.Match(
                                valid: x => false,
                                fail: errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY)
                                );

        hasErrors.Should().BeTrue();
    }


    [Fact]
    public async Task MatchAsync_sourceSync_validAndfailSync_sourceValid_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.MatchAsync(valid: x => 12,
                                                       fail: _ => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task MatchAsync_sourceSync_validAndfailSync_sourceFail_OK()
    {
        MlResult<int> source = "Error".ToMlResultFail<int>();

        MlResult<int> result = await source.MatchAsync(valid: x => 12,
                                                       fail: _ => 13);

        MlResult<int> expected = 13;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_sourceSync_validAndfailSync_sourceValid_withoutException_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.TryMatchAsync(valid: x => 12,
                                                          fail: _ => 13);

        MlResult<int> expected = 12;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task TryMatchAsync_sourceSync_validAndfailSync_sourceValid_withException_OK()
    {
        MlResult<int> source = 11;
        var divisor = 0;

        MlResult<int> result = await source.TryMatchAsync(valid: _ => 12 / divisor,
                                                          fail: _ => 13);

        bool hasErrors = result.Match(
            valid: _ => false,
            fail: errorsDetails => errorsDetails.Details.ContainsKey(EX_DESC_KEY));

        hasErrors.Should().BeTrue();
    }

    [Fact]
    public async Task MatchAsync_simple_sourceSync_OK()
    {
        MlResult<int> source = 11;

        MlResult<int> result = await source.MatchAsync(() => 12);
        MlResult<int> expected = 12;

        result.ToString().Should().Be(expected.ToString());
    }





    #endregion







}
