// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

public class MlResultTests
{



    [Fact]
    public void ToString_IsValid_OK()
    {
        MlResult<int> data = 1;

        var result = data.ToString();

        var expected = "1";

        result.Should().Be(expected);
    }


    [Fact]
    public void Fail_WithoutErrors_UsesDefaultError()
    {
        var results = new[]
        {
            MlResult<int>.Fail(new MlErrorDetails()),
            MlResult<int>.Fail(Array.Empty<MlError>()),
            MlResult<int>.Fail()
        };

        foreach (var result in results)
        {
            result.IsFailure.Should().BeTrue();
            result.GetFailureDetailsOrThrow().GetErrorMessages()
                .Should().ContainSingle().Which.Should().Be("The MlResult failed without error details.");
        }
    }


    [Fact]
    public async Task FailAsync_WithoutErrors_UsesDefaultError()
    {
        var result = await MlResult<int>.FailAsync(new MlErrorDetails());

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().GetErrorMessages()
            .Should().ContainSingle().Which.Should().Be("The MlResult failed without error details.");
    }


    [Fact]
    public void MlErrorDetails_ToString_WithoutErrorsOrMetadata_ReturnsDefaultMessage()
        => new MlErrorDetails().ToString().Should().Be("The MlResult failed without error details.");



    [Fact]
    public void ToString_IsFail_withErrors_withDetails_OK()
    {
        MlResult<int> data = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" }, new Dictionary<string, object>
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

        var result = data.ToString();

        var expected = @"""MlError:
     miError
     miError2
     miError3
     miError4
     miError5
Details:
     key1: value1
     key2: value2
     key3: value3
     key4: value4
     key5: value5
     key6: value6
     key7: value7
     key8: value8
     key9: value9
     key10: value10""";


        result.Should().NotBeEquivalentTo(expected);
    }



    [Fact]
    public void ToString_IsFail_withErrors_OK()
    {
        MlResult<int> data = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" });

        var result = data.ToString();

        var expected = @"""MlError:
     miError
     miError2
     miError3
     miError4
     miError5""";


        result.Should().NotBeEquivalentTo(expected);
    }



}

