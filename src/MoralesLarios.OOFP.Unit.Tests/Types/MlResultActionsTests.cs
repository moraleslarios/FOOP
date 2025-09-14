namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultActionsTests
{


    #region MlErrorDetail


    [Fact]
    public void AddMlErrorDetailIfFail_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.AddMlErrorDetailIfFail("key", "value");

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void AddMlErrorDetailIfFail_failSource_addDetails_return()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.AddMlErrorDetailIfFail("key", "value");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { "key", "value" } });

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void AddValueDetailIfFail_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = partialResult.AddValueDetailIfFail("value");

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void AddValueDetailIfFail_failSource_addDetails_return()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.AddValueDetailIfFail("XXXX");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { VALUE_KEY, "XXXX" } });

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task AddMlErrorDetailIfFailAsync_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.AddMlErrorDetailIfFailAsync("key", "value");

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task AddMlErrorDetailIfFailAsync_failSource_addDetails_return()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.AddMlErrorDetailIfFailAsync("key", "value");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { "key", "value" } });

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task AddValueDetailIfFailAsync_validSource_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> result = await partialResult.AddValueDetailIfFailAsync("value");

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task AddValueDetailIfFailAsync_failSource_addDetails_return()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> result = await partialResult.AddValueDetailIfFailAsync("XXXX");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { VALUE_KEY, "XXXX" } });

        result.ToString().Should().Be(expected.ToString());
    }







    [Fact]
    public async Task AddMlErrorDetailIfFailAsync_sourceAsync_validSource_return_source()
    {
        Task<MlResult<int>> partialResultAsync = MlResult.Valid(1).ToAsync();

        MlResult<int> result = await partialResultAsync.AddMlErrorDetailIfFailAsync("key", "value");

        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task AddMlErrorDetailIfFailAsync_sourceAsync_failSource_addDetails_return()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.AddMlErrorDetailIfFailAsync("key", "value");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { "key", "value" } });

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public async Task AddValueDetailIfFailAsync_sourceAsync_validSource_return_source()
    {
        Task<MlResult<int>> partialResultAsync = MlResult.Valid(1).ToAsync();

        MlResult<int> result = await partialResultAsync.AddValueDetailIfFailAsync("value");

        MlResult<int> partialResult = await partialResultAsync;

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public async Task AddValueDetailIfFailAsync_sourceAsync_failSource_addDetails_return()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<int> result = await partialResultAsync.AddValueDetailIfFailAsync("XXXX");

        MlResult<int> expected = ("Error", new Dictionary<string, object> { { VALUE_KEY, "XXXX" } });

        result.ToString().Should().Be(expected.ToString());
    }



    #endregion





    #region CompleteWithValue


    [Fact]
    public void CompleteWithDataValueIfValid_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValueIfValid(x => (initialValue, x)));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CompleteWithDataValueIfValid_validSource_returnValueCompleted()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValueIfValid(x => (initialValue, x)));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void CompleteWithDataValueIfValid_failSource_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValueIfValid(x => (initialValue, x)));
        result.IsFail.Should().BeTrue();
    }           


    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_validSource_returnValueCompleted()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_failSource_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        result.IsFail.Should().BeTrue();
    }   




    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_sourceAsync_validSource_returnIsValid()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                                .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_sourceAsync_validSource_returnValueCompleted()
    {
        Task<MlResult<int>> partialResultAsync = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResultAsync.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                                .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CompleteWithDataValueIfValidAsync_sourceAsync_failSource_returnFail()
    {
        Task<MlResult<int>> partialResultAsync = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, string)> result = await partialResultAsync.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                                .CompleteWithDataValueIfValidAsync(x => (initialValue, x).ToAsync()));
        result.IsFail.Should().BeTrue();
    }   












    [Fact]
    public void CompleteWithDetailsValueIfFail_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDetailsValueIfFail(initialValue));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CompleteWithDetailsValueIfFail_failSource_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDetailsValueIfFail(initialValue));

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void CompleteWithDetailsValueIfFail_validSource_failFuncResult_AddDetailsValue()
    {
        MlResult<int> partialResult = 0;

        MlResult<string> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDetailsValueIfFail(initialValue));

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    
    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_failSource_returnFail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_validSource_failFuncResult_AddDetailsValue()
    {
        MlResult<int> partialResult = 0;

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }




    
    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_sourceAsync_validSource_returnIsValid()
    {
        Task<MlResult<int>> partialResult = 1.ToMlResultValidAsync();

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_sourceAsync_failSource_returnFail()
    {
        Task<MlResult<int>> partialResult = "Error".ToMlResultFailAsync<int>();

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task CompleteWithDetailsValueIfFailAsync_sourceAsync_validSource_failFuncResult_AddDetailsValue()
    {
        Task<MlResult<int>> partialResult = 0.ToMlResultValidAsync();

        MlResult<string> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                    .CompleteWithDetailsValueIfFailAsync(initialValue));

        MlResult<string> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }












    [Fact]
    public void CompleteWithDataValue_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValue(initialValue, x => (initialValue, x)));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CompleteWithDataValue_validSource_returnValueCompleted()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValue(initialValue, x => (initialValue, x)));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void CompleteWithDataValue_validSource_failFuncResult_returnInvalid()
    {
        MlResult<int> partialResult = 0;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValue(initialValue, x => (initialValue, x)));

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public void CompleteWithDataValue_validSource_failFuncResult_AddDetailsValue()
    {
        MlResult<int> partialResult = 0;

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValue(initialValue, x => (initialValue, x)));

        MlResult<(int, string)> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void CompleteWithDataValue_failSource_return_Fail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = partialResult.Bind(initialValue => BindTestMethod(initialValue)
                                                                                .CompleteWithDataValue(initialValue, x => (initialValue, x)));

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void CompleteWithDataValue_failSource_AddValueDetail()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<(int, string)> result = partialResult.CompleteWithDataValue(99, x => (99, "valor relleno"));

        MlResult<string> expected = ("Error", new Dictionary<string, object> { { VALUE_KEY, 99 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CompleteWithDataValueAsync_validSource_returnIsValid()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDataValueAsync_validSource_returnValueCompleted()
    {
        MlResult<int> partialResult = 1;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CompleteWithDataValueAsync_validSource_failFuncResult_returnInvalid()
    {
        MlResult<int> partialResult = 0;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task CompleteWithDataValueAsync_validSource_failFuncResult_AddDetailsValue()
    {
        MlResult<int> partialResult = 0;

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));

        MlResult<(int, string)> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }





    [Fact]
    public async Task CompleteWithDataValueAsync_sourceAsync_validSource_returnIsValid()
    {
        Task<MlResult<int>> partialResult = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CompleteWithDataValueAsync_sourceAsync_validSource_returnValueCompleted()
    {
        Task<MlResult<int>> partialResult = 1.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));
        MlResult<(int, string)> expected = (1, "1");

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CompleteWithDataValueAsync_sourceAsync_validSource_failFuncResult_returnInvalid()
    {
        Task<MlResult<int>> partialResult = 0.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task CompleteWithDataValueAsync_sourceAsync_validSource_failFuncResult_AddDetailsValue()
    {
        Task<MlResult<int>> partialResult = 0.ToMlResultValidAsync();

        MlResult<(int, string)> result = await partialResult.BindAsync(initialValue => BindTestMethodAsync(initialValue)
                                                                                            .CompleteWithDataValueAsync(initialValue, x => (initialValue, x).ToAsync()));

        MlResult<(int, string)> expected = ("Not OK", new Dictionary<string, object> { { VALUE_KEY, 0 } });

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }






    #endregion





    private MlResult<string> BindTestMethod(int x)
        => x != 0 ? x.ToString()  : "Not OK".ToMlResultFail<string>();

    private Task<MlResult<string>> BindTestMethodAsync(int x)
        => x != 0 ? x.ToString().ToMlResultValidAsync() : "Not OK".ToMlResultFailAsync<string>();






    #region CreateCompleteMlResult



    [Fact]
    public void CreateCompleteMlResult_extendMlResult_tooValidParameters_return_valid()
    {
        MlResult<int> source1 = 1;
        MlResult<int> source2 = 2;

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void CreateCompleteMlResult_extendMlResult_source1Valid_source2Fail_return_fail()
    {
        MlResult<int> source1 = 1;
        MlResult<int> source2 = "Error".ToMlResultFail<int>();

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void CreateCompleteMlResult_extendMlResult_fail1Source_source2Valid_return_fail()
    {
        MlResult<int> source1 = "Error".ToMlResultFail<int>();
        MlResult<int> source2 = 2;

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void CreateCompleteMlResult_extendMlResult_tooFailParameters_return_fail()
    {
        MlResult<int> source1 = "Error".ToMlResultFail<int>();
        MlResult<int> source2 = "Error".ToMlResultFail<int>();

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task CreateCompleteMlResultAsync_extendMlResult_tooValidParameters_return_valid()
    {
        MlResult<int> source1 = 1;
        MlResult<int> source2 = 2;

        MlResult<(int, int)> result = await source1.CreateCompleteMlResultAsync(source2);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task CreateCompleteMlResult_extendMlResult_source1ValidAsync_source2Fail_return_fail()
    {
        Task<MlResult<int>> source1Async = 1.ToMlResultValidAsync();
        MlResult<int>       source2      = "Error".ToMlResultFail<int>();

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompleteMlResult_extendMlResult_source1Valid_source2FailAsync_return_fail()
    {
        MlResult<int>       source1      = 1;
        Task<MlResult<int>> source2Async = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, int)> result = await source2Async.CreateCompleteMlResultAsync(source1);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompleteMlResult_extendMlResult_source1Valid_source2FailAsync_return_fail2()
    {
        MlResult<int>       source1      = 1;
        Task<MlResult<int>> source2Async = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, int)> result = await source1.CreateCompleteMlResultAsync(source2Async);

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task CreateCompleteMlResultAsync_extendMlResult_fail1SourceAsync_source2Valid_return_fail()
    {
        Task<MlResult<int>> source1Async = "Error".ToMlResultFailAsync<int>();
        MlResult<int>       source2      = 2;

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompleteMlResultAsync_extendMlResult_fail1Source_source2ValidAsync_return_fail()
    {
        MlResult<int>       source1      = "Error".ToMlResultFail<int>();
        Task<MlResult<int>> source2Async = 2.ToMlResultValidAsync();;

        MlResult<(int, int)> result = await source1.CreateCompleteMlResultAsync(source2Async);

        result.IsFail.Should().BeTrue();
    }



    [Fact]
    public async Task CreateCompleteMlResultAsync_extendMlResult_tooFailParameters_return_fail()
    {
        Task<MlResult<int>> source1Async = "Error".ToMlResultFailAsync<int>();
        Task<MlResult<int>> source2Async = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2Async);

        result.IsFail.Should().BeTrue();
    }







    [Fact]
    public void CreateCompleteMlResult_extendObject_source2Valid_return_valid()
    {
        int           source1 = 1;
        MlResult<int> source2 = 2;

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void CreateCompleteMlResult_extendObject_source2Fail_return_fail()
    {
        int           source1 = 1;
        MlResult<int> source2 = "Error".ToMlResultFail<int>();

        MlResult<(int, int)> result = source1.CreateCompleteMlResult(source2);

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source1Async_source2Valid_return_valid()
    {
        Task<int>     source1Async = 1.ToAsync();
        MlResult<int> source2      = 2;

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source1_source2AsyncValid_return_valid()
    {
        int                 source1      = 1;
        Task<MlResult<int>> source2Async = 2.ToMlResultValidAsync();

        MlResult<(int, int)> result = await source1.CreateCompleteMlResultAsync(source2Async);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source1Async_source2Fail_return_fail()
    {
        Task<int>     source1Async = 1.ToAsync();
        MlResult<int> source2      = "Error".ToMlResultFail<int>();

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2);

        result.IsFail.Should().BeTrue();
    }




    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source2FailAsync_return_fail()
    {
        int                 source1      = 1;
        Task<MlResult<int>> source2Async = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, int)> result = await source1.CreateCompleteMlResultAsync(source2Async);

        result.IsFail.Should().BeTrue();
    }




    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source1Async_source2FValidAsync_return_valid()
    {
        Task<int>           source1Async = 1.ToAsync();
        Task<MlResult<int>> source2Async = 2.ToMlResultValidAsync();

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2Async);

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task CreateCompleteMlResultAsync_extendObject_source1Async_source2FailAsync_return_fail()
    {
        Task<int>           source1Async = 1.ToAsync();
        Task<MlResult<int>> source2Async = "Error".ToMlResultFailAsync<int>();

        MlResult<(int, int)> result = await source1Async.CreateCompleteMlResultAsync(source2Async);

        result.IsFail.Should().BeTrue();
    }





















    #endregion




}
