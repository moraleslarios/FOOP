using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Unit.Tests.Types.Errors;
public class MlErrorsDetailsActionsTests
{




    [Fact]
    public void Merge_OK()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                                    { 
                                                        { "key1", "value1" },
                                                        { "key2", "value2" }
                                                    }
                                   );

        MlErrorsDetails other = ("miError2", new Dictionary<string, object> 
                                                    { 
                                                        { "key3", "value3" },
                                                        { "key4", "value4" }
                                                    }
                                   );

        MlErrorsDetails result = errors1.Merge(other);

        MlErrorsDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" },
                                                        { "key3", "value3" },
                                                        { "key4", "value4" }
                                                    }
                                          );

        result.Should().BeEquivalentTo(expected);

    }


    [Fact]
    public void Merge_Range_OK()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        IEnumerable<MlErrorsDetails> mlErrorsDetails = new List<MlErrorsDetails>
        {
            ("miError2", new Dictionary<string, object>
                                            {
                                                { "key3", "value3" },
                                                { "key4", "value4" }
                                            }
            ),
            ("miError3", new Dictionary<string, object>
                                            {
                                                { "key5", "value5" },
                                                { "key6", "value6" }
                                            }
            ),
            ("miError4", new Dictionary<string, object>
                                            {
                                                { "key7", "value7" },
                                                { "key8", "value8" }
                                            }
            ),
            ("miError5", new Dictionary<string, object>
                                            {
                                                { "key9", "value9" },
                                                { "key10", "value10" }
                                            }
            )

        };

        MlErrorsDetails result = errors1.Merge(mlErrorsDetails);

        MlErrorsDetails expected = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" },
                                                        { "key3", "value3" },
                                                        { "key4", "value4" },
                                                        { "key5", "value5" },
                                                        { "key6", "value6" },
                                                        { "key7", "value7" },
                                                        { "key8", "value8" },
                                                        { "key9", "value9" },
                                                        { "key10", "value10" }
                                                    }
                                          );

        result.Should().BeEquivalentTo(expected);
    }



    //[Fact]
    //public void Merge_withExceptions_OK()
    //{
    //    MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
    //                                        { 
    //                                            { EX_DESC_KEY      , new ArgumentNullException("Excepcion 1") },
    //                                            { $"{EX_DESC_KEY}2", new ArgumentNullException("Excepcion 2") }
    //                                        }
    //                                );

    //    IEnumerable<MlErrorsDetails> mlErrorsDetails = new List<MlErrorsDetails>
    //    {
    //        ("miError2", new Dictionary<string, object>
    //                                        {
    //                                            { EX_DESC_KEY      , new ArgumentNullException("Excepcion 3") },
    //                                            { $"{EX_DESC_KEY}2", new ArgumentNullException("Excepcion 4") }
    //                                        }
    //        )

    //    };

    //    MlErrorsDetails result = errors1.Merge(mlErrorsDetails);

    //    MlErrorsDetails expected = (new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
    //                                     {
    //                                            { EX_DESC_KEY      , new ArgumentNullException("Excepcion 1") },
    //                                            { $"{EX_DESC_KEY}2", new ArgumentNullException("Excepcion 2") },
    //                                            { $"{EX_DESC_KEY}3", new ArgumentNullException("Excepcion 3") },
    //                                            { $"{EX_DESC_KEY}4", new ArgumentNullException("Excepcion 4") }
    //                                                }
    //                                      );

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}






    [Fact]
    public void AddError_Ok()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorsDetails result = errors1.AddError("miError2");

        MlErrorsDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" }
                                                    }
                                          );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AddErrorMessage_Ok()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorsDetails result = errors1.AddErrorMessage("miError2");

        MlErrorsDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" }
                                                    }
                                          );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AddErrors_Ok()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorsDetails result = errors1.AddErrors("miError2", "miError3");

        MlErrorsDetails expected = ( new List<MlError> { "miError", "miError2", "miError3" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" }
                                                    }
                                          );

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AddDetail_Ok()
    {
        MlErrorsDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorsDetails result = errors1.AddDetail("key3", "value3");

        MlErrorsDetails expected = ( new List<MlError> { "miError" }, new Dictionary<string, object>
        {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" },
                                                        { "key3", "value3" }
                                                    }
                                                 );

        result.Should().BeEquivalentTo(expected);

    }



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




    #region AddDetailValue


    [Fact]
    public void AddDetailValue_keyExist_return_valid()
    {
        MlErrorsDetails errors1 = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlErrorsDetails result = errors1.AddDetailValue("value3");

        MlErrorsDetails expected = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { VALUE_KEY, "value3" },
                                                { "key2", "value2" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void AddDetailValue_keyNotExist_return_valid()
    {
        MlErrorsDetails errors1 = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { "keyXXX", "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlErrorsDetails result = errors1.AddDetailValue("value3");

        MlErrorsDetails expected = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { "keyXXX", "value1" },
                                                { "key2", "value2" },
                                                { VALUE_KEY, "value3" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }






    #endregion



    #region HasValueDetails


    [Fact]
    public void HasValueDetails_withDetailsValue_return_true()
    {
        MlErrorsDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = errors1.HasValueDetails();

        result.Should().BeTrue();
    }


    [Fact]
    public void HasValueDetails_withoutDetailsValue_return_false()
    {
        MlErrorsDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = errors1.HasValueDetails();

        result.Should().BeFalse();
    }



    [Fact]
    public async Task HasValueDetailsAsync_withDetailsValue_return_true()
    {
        MlErrorsDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { VALUE_KEY, "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = await errors1.HasValueDetailsAsync();

        result.Should().BeTrue();
    }


    [Fact]
    public async Task HasValueDetailsAsync_withoutDetailsValue_return_false()
    {
        MlErrorsDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = await errors1.HasValueDetailsAsync();

        result.Should().BeFalse();
    }




    #endregion



    #region HasExceptionDetails


    [Fact]
    public void HasExceptionDetails_withExceptionDetails_return_true()
    {
        MlErrorsDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                          {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                                                           );

        bool result = errors1.HasExceptionDetails();

        result.Should().BeTrue();
    }


    [Fact]
    public void HasExceptionDetails_withoutExceptionDetails_return_false()
    {
        MlErrorsDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                                     {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                                                                                                                                               );

        bool result = errors1.HasExceptionDetails();

        result.Should().BeFalse();
    }


    [Fact]
    public async Task HasExceptionDetailsAsync_withExceptionDetails_return_true()
    {
        MlErrorsDetails errors1 = ("miError",
                                                          new Dictionary<string, object>
                                                          {
                                                { EX_DESC_KEY, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                                                           );

        bool result = await errors1.HasExceptionDetailsAsync();

        result.Should().BeTrue();
    }


    [Fact]
    public async Task HasExceptionDetailsAsync_withoutExceptionDetails_return_false()
    {
        MlErrorsDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                                     {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                                                                                                                                               );

        bool result = await errors1.HasExceptionDetailsAsync();

        result.Should().BeFalse();
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



    #endregion





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
