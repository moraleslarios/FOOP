// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types.Errors;
public class MlErrorDetailsActionsTests
{




    [Fact]
    public void Merge_OK()
    {
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                                    { 
                                                        { "key1", "value1" },
                                                        { "key2", "value2" }
                                                    }
                                   );

        MlErrorDetails other = ("miError2", new Dictionary<string, object> 
                                                    { 
                                                        { "key3", "value3" },
                                                        { "key4", "value4" }
                                                    }
                                   );

        MlErrorDetails result = errors1.Merge(other);

        MlErrorDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
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
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        IEnumerable<MlErrorDetails> mlErrorsDetails = new List<MlErrorDetails>
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

        MlErrorDetails result = errors1.Merge(mlErrorsDetails);

        MlErrorDetails expected = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" }, new Dictionary<string, object>
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
    //    MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
    //                                        { 
    //                                            { Constants.ExceptionMetadataKey      , new ArgumentNullException("Excepcion 1") },
    //                                            { $"{Constants.ExceptionMetadataKey}2", new ArgumentNullException("Excepcion 2") }
    //                                        }
    //                                );

    //    IEnumerable<MlErrorDetails> mlErrorsDetails = new List<MlErrorDetails>
    //    {
    //        ("miError2", new Dictionary<string, object>
    //                                        {
    //                                            { Constants.ExceptionMetadataKey      , new ArgumentNullException("Excepcion 3") },
    //                                            { $"{Constants.ExceptionMetadataKey}2", new ArgumentNullException("Excepcion 4") }
    //                                        }
    //        )

    //    };

    //    MlErrorDetails result = errors1.Merge(mlErrorsDetails);

    //    MlErrorDetails expected = (new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
    //                                     {
    //                                            { Constants.ExceptionMetadataKey      , new ArgumentNullException("Excepcion 1") },
    //                                            { $"{Constants.ExceptionMetadataKey}2", new ArgumentNullException("Excepcion 2") },
    //                                            { $"{Constants.ExceptionMetadataKey}3", new ArgumentNullException("Excepcion 3") },
    //                                            { $"{Constants.ExceptionMetadataKey}4", new ArgumentNullException("Excepcion 4") }
    //                                                }
    //                                      );

    //    result.ToString().Should().BeEquivalentTo(expected.ToString());
    //}






    [Fact]
    public void AddError_Ok()
    {
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorDetails result = errors1.AddError("miError2");

        MlErrorDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
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
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorDetails result = errors1.AddErrorMessage("miError2");

        MlErrorDetails expected = ( new List<MlError> { "miError", "miError2" }, new Dictionary<string, object>
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
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorDetails result = errors1.AddErrors("miError2", "miError3");

        MlErrorDetails expected = ( new List<MlError> { "miError", "miError2", "miError3" }, new Dictionary<string, object>
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
        MlErrorDetails errors1 = ("miError", new Dictionary<string, object> 
                                            { 
                                                { "key1", "value1" },
                                                { "key2", "value2" }
                                            }
                            );

        MlErrorDetails result = errors1.AddMetadataOrThrow("key3", "value3");

        MlErrorDetails expected = ( new List<MlError> { "miError" }, new Dictionary<string, object>
        {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" },
                                                        { "key3", "value3" }
                                                    }
                                                 );

        result.Should().BeEquivalentTo(expected);

    }







    #region AddValueMetadata


    [Fact]
    public void AddDetailValue_keyExist_return_valid()
    {
        MlErrorDetails errors1 = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { Constants.ValueMetadataKey, "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlErrorDetails result = errors1.AddValueMetadata("value3");

        MlErrorDetails expected = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { Constants.ValueMetadataKey, "value3" },
                                                { "key2", "value2" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void AddDetailValue_keyNotExist_return_valid()
    {
        MlErrorDetails errors1 = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { "keyXXX", "value1" },
                                                { "key2", "value2" }
                                            }
                                    );

        MlErrorDetails result = errors1.AddValueMetadata("value3");

        MlErrorDetails expected = ("miError", 
                                    new Dictionary<string, object>
                                            {
                                                { "keyXXX", "value1" },
                                                { "key2", "value2" },
                                                { Constants.ValueMetadataKey, "value3" }
                                            }
                                    );

        result.ToString().Should().Be(expected.ToString());
    }






    #endregion



    #region ContainsValueMetadata


    [Fact]
    public void HasValueDetails_withDetailsValue_return_true()
    {
        MlErrorDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { Constants.ValueMetadataKey, "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = errors1.ContainsValueMetadata();

        result.Should().BeTrue();
    }


    [Fact]
    public void HasValueDetails_withoutDetailsValue_return_false()
    {
        MlErrorDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = errors1.ContainsValueMetadata();

        result.Should().BeFalse();
    }



    [Fact]
    public async Task HasValueDetailsAsync_withDetailsValue_return_true()
    {
        MlErrorDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { Constants.ValueMetadataKey, "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = await errors1.ContainsValueMetadataAsync();

        result.Should().BeTrue();
    }


    [Fact]
    public async Task HasValueDetailsAsync_withoutDetailsValue_return_false()
    {
        MlErrorDetails errors1 = ("miError",
                                               new Dictionary<string, object>
                                               {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                  );

        bool result = await errors1.ContainsValueMetadataAsync();

        result.Should().BeFalse();
    }




    #endregion



    #region ContainsException


    [Fact]
    public void HasExceptionDetails_withExceptionDetails_return_true()
    {
        MlErrorDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                          {
                                                { Constants.ExceptionMetadataKey, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                                                           );

        bool result = errors1.ContainsException();

        result.Should().BeTrue();
    }


    [Fact]
    public void HasExceptionDetails_withoutExceptionDetails_return_false()
    {
        MlErrorDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                                     {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                                                                                                                                               );

        bool result = errors1.ContainsException();

        result.Should().BeFalse();
    }


    [Fact]
    public async Task HasExceptionDetailsAsync_withExceptionDetails_return_true()
    {
        MlErrorDetails errors1 = ("miError",
                                                          new Dictionary<string, object>
                                                          {
                                                { Constants.ExceptionMetadataKey, new Exception("miException") },
                                                { "key2", "value2" }
                                            }
                                                                                                                                           );

        bool result = await errors1.ContainsExceptionAsync();

        result.Should().BeTrue();
    }


    [Fact]
    public async Task HasExceptionDetailsAsync_withoutExceptionDetails_return_false()
    {
        MlErrorDetails errors1 = ("miError",
                                            new Dictionary<string, object>
                                                                     {
                                                { "Key1", "value1" },
                                                { "key2", "value2" }
                                            }
                                                                                                                                                                                                               );

        bool result = await errors1.ContainsExceptionAsync();

        result.Should().BeFalse();
    }




    #endregion















}

