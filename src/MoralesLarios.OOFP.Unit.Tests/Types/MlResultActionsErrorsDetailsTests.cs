namespace MoralesLarios.OOFP.Unit.Tests.Types;

public class MlResultActionsErrorsDetailsTests
{


    [Fact]
    public void GetDetail_keyNotExist_return_fail()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<string> result = errors1.GetDetail<string>("key3");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetDetail_keyExist_return_valid()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<string> result = errors1.GetDetail<string>("key2");

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void GetDetail_keyExist_diferentTypeOfT_return_fail()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", 7m }
                                            }
                                    );

        MlResult<string> result = errors1.GetDetail<string>("key2");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void GetDetailValue_keyExist_return_valid()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<string> result = errors1.GetDetailValue<string>();

        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public void GetDetailValue_keyNotExist_return_fail()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<string> result = errors1.GetDetailValue<string>();

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public async Task Tests()
    {
        MlResult<int> partialResult = await "Error".ToMlResultFailAsync<int>();

        var data = await partialResult.CompleteWithDetailsValueIfFailAsync(69)
                                            .BindIfFailAsync(errorsDetails => errorsDetails
                                                                                    .GetDetailValueAsync<int>());
        data.Should().NotBeNull();
    }


    [Fact]
    public void MergeErrorsDetailsIfFail_sourceValid_secuondaryFal_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<int> secondary = "Error".ToMlResultFail<int>();

        MlResult<int> result = partialResult.MergeErrorsDetailsIfFail(secondary);

        result.ToString().Should().Be(partialResult.ToString());
    }


    [Fact]
    public void MergeErrorsDetailsIfFail_sourceFAil_secondaryValid_return_source()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<int> secondary = 1;

        MlResult<int> result = partialResult.MergeErrorsDetailsIfFail(secondary);

        result.ToString().Should().Be(partialResult.ToString());
    }



    [Fact]
    public void MergeErrorsDetailsIfFailDioferentTypes_sourceValid_secuondaryFal_return_source()
    {
        MlResult<int> partialResult = 1;

        MlResult<string> secondary = "Error".ToMlResultFail<string>();

        MlResult<int> result = partialResult.MergeErrorsDetailsIfFailDiferentTypes(secondary);

        result.ToString().Should().Be(partialResult.ToString());
    }

    [Fact]
    public void MergeErrorsDetailsIfFailDioferentTypes_sourceFAil_secondaryValid_return_source()
    {
        MlResult<int> partialResult = "Error".ToMlResultFail<int>();

        MlResult<string> secondary = "hola";

        MlResult<int> result = partialResult.MergeErrorsDetailsIfFailDiferentTypes(secondary);

        result.ToString().Should().Be(partialResult.ToString());
    }



    [Fact]
    public void AddValueIfFail_sourceValid_return_source()
    {
        MlResult<int> source = 1;

        MlResult<int> result = source.AddValueIfFail<int, int>(69);

        MlResult<int> expected = 1;

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void AddValueIfFail_sourceFail_return_failWithValueAdded()
    {
        MlResult<int> source = "miError".ToMlResultFail<int>();

        MlResult<int> result = source.AddValueIfFail<int, int>(69);

        MlResult<int> expected = ("miError", new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, 69 }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }




    #region MergeErrorsDetails


    [Fact]
    public void MergeErrorsDetails_secondaryValid_return_source()
    {
        MlErrorsDetails source = ("miError", new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<int> secondary = 1;

        MlResult<string> result = source.MergeErrorsDetails<int, string>(secondary);

        MlResult<string> expected = ("miError", new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void MergeErrorsDetails_secondaryfail_return_failMergeErrorsDetails()
    {
        MlErrorsDetails source = ("miError", new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlResult<int> secondary = ("miError2", new Dictionary<string, object> 
                                            { 
                                                { "other", "value1-2" },
                                                { "key2-2", "value2-2" }
                                            }
                                    );

        MlResult<string> result = source.MergeErrorsDetails<int, string>(secondary);

        MlResult<string> expected = (["miError", "miError2"], new Dictionary<string, object> 
                                            { 
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" },
                                                { "other", "value1-2" },
                                                { "key2-2", "value2-2" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }




    #endregion




}
