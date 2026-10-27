// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

public class MlResultSeveralExtensionsTests
{


    [Fact]
    public void CombineAll_2Values_OK()
    {
        MlResult<int> value1 = 8;

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = value1.CombineAll(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_2Values_OK()
    {
        MlResult<int> value1 = 8;

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAllAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_2Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAllAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void CombineAll_3Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = value1.CombineAll(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_3Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_3Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void CombineAll_4Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = value1.CombineAll(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_4Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_4Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void CombineAll_5Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = value1.CombineAll(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_5Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_5Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = await value1.CombineAllAsync(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void CombineAll_reverse_2Values_OK()
    {
        int value1 = 8;

        MlResult<decimal> value2 = 5.5m;

        MlResult<(int, decimal)> result   = value1.CombineAll(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAllAsync_reverse_2Values_OK()
    {
        int value1 = 8;

        MlResult<decimal> value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAllAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_reverse_2Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        decimal value2 = 5.5m;

        MlResult<(decimal, int)> result   = await value2.CombineAllAsync(value1);
        MlResult<(decimal, int)> expected = (5.5m, 8);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void CombineAll_reverse_3Values_OK()
    {
        (int, string) values = (8, "hiii");

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, decimal)> result   = values.CombineAll(valueResult);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAllAsync_reverse_3Values_OK()
    {
        (int, string) values = (8, "hiii");

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, decimal)>  expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_reverse_3Values_sourceAsync_OK()
    {
        (int, string) values = (8, "hiii");

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, decimal)>  expected = (8, "hiii", 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void CombineAll_reverse_4Values_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, decimal)> result   = values.CombineAll(valueResult);
        MlResult<(int, string, int, decimal)> expected = (8, "hiii", 1, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAllAsync_reverse_4Values_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, int, decimal)>  expected = (8, "hiii", 1, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_reverse_4Values_sourceAsync_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, int, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, int, decimal)>  expected = (8, "hiii", 1, 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void CombineAll_reverse_5Values_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, int, decimal)> result   = values.CombineAll(valueResult);
        MlResult<(int, string, int, int, decimal)> expected = (8, "hiii", 1, 2, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAllAsync_reverse_5Values_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, int, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, int, int, decimal)>  expected = (8, "hiii", 1, 2, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAllAsync_reverse_5Values_sourceAsync_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, int, int, decimal)>? result   = await values.CombineAllAsync(valueResult);
        MlResult<(int, string, int, int, decimal)>  expected = (8, "hiii", 1, 2, 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async void FailIfFalse_falseCondition_return_Fail()
    {
        bool condition = 1 > 2;

        MlResult<bool> result = condition.FailIfFalse("Error");

        MlResult<bool> expectred = "Error".ToMlResultFail<bool>();

        result.ToString().Should().BeEquivalentTo(expectred.ToString());
    }

    [Fact]
    public async Task FailIfNullAsync_sourceAsync_withMlError_nullSource_return_Fail()
    {
        Task<string?> sourceAsync = ((string?)null).ToAsync();

        MlResult<string?> result = await sourceAsync.FailIfNullAsync(MlError.FromErrorMessage("Error"));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task FailIfNullAsync_sourceAsync_withErrorsDetails_nonNullSource_return_Valid()
    {
        Task<string?> sourceAsync = ((string?)"value").ToAsync();

        MlResult<string?> result = await sourceAsync.FailIfNullAsync(MlErrorDetails.FromErrorMessage("Error"));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task FailIfFalseAsync_sourceAsync_withMlError_falseCondition_return_Fail()
    {
        Task<int> sourceAsync = 10.ToAsync();

        MlResult<int> result = await sourceAsync.FailIfFalseAsync(condition: false, error: MlError.FromErrorMessage("Error"));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task FailIfFalseAsync_sourceAsync_withErrorsDetails_trueCondition_return_Valid()
    {
        Task<int> sourceAsync = 10.ToAsync();

        MlResult<int> result = await sourceAsync.FailIfFalseAsync(condition: true, errorsDetails: MlErrorDetails.FromErrorMessage("Error"));

        result.IsSuccess.Should().BeTrue();
    }










}

