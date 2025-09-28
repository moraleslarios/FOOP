namespace MoralesLarios.OOFP.Unit.Tests.Types;

public class MlResultActionsSeveralTests
{


    [Fact]
    public void Combine_2Values_OK()
    {
        MlResult<int> value1 = 8;

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = value1.Combine(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_2Values_OK()
    {
        MlResult<int> value1 = 8;

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_2Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        decimal value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void Combine_3Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = value1.Combine(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_3Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_3Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal) values = ("hiii", 5.5m);

        MlResult<(int, string, decimal)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void Combine_4Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = value1.Combine(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_4Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_4Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal, int) values = ("hiii", 5.5m, 1);

        MlResult<(int, string, decimal, int)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal, int)> expected = (8, "hiii", 5.5m, 1);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public void Combine_5Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = value1.Combine(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_5Values_OK()
    {
        MlResult<int> value1 = 8;

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_5Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        (string, decimal, int, int) values = ("hiii", 5.5m, 1, 2);

        MlResult<(int, string, decimal, int, int)> result   = await value1.CombineAsync(values);
        MlResult<(int, string, decimal, int, int)> expected = (8, "hiii", 5.5m, 1, 2);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void Combine_reverse_2Values_OK()
    {
        int value1 = 8;

        MlResult<decimal> value2 = 5.5m;

        MlResult<(int, decimal)> result   = value1.Combine(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAsync_reverse_2Values_OK()
    {
        int value1 = 8;

        MlResult<decimal> value2 = 5.5m;

        MlResult<(int, decimal)> result   = await value1.CombineAsync(value2);
        MlResult<(int, decimal)> expected = (8, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_reverse_2Values_sourceAsync_OK()
    {
        Task<MlResult<int>> value1 = 8.ToMlResultValidAsync();

        decimal value2 = 5.5m;

        MlResult<(decimal, int)> result   = await value2.CombineAsync(value1);
        MlResult<(decimal, int)> expected = (5.5m, 8);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public void Combine_reverse_3Values_OK()
    {
        (int, string) values = (8, "hiii");

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, decimal)> result   = values.Combine(valueResult);
        MlResult<(int, string, decimal)> expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAsync_reverse_3Values_OK()
    {
        (int, string) values = (8, "hiii");

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, decimal)>  expected = (8, "hiii", 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_reverse_3Values_sourceAsync_OK()
    {
        (int, string) values = (8, "hiii");

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, decimal)>  expected = (8, "hiii", 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void Combine_reverse_4Values_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, decimal)> result   = values.Combine(valueResult);
        MlResult<(int, string, int, decimal)> expected = (8, "hiii", 1, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAsync_reverse_4Values_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, int, decimal)>  expected = (8, "hiii", 1, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_reverse_4Values_sourceAsync_OK()
    {
        (int, string, int) values = (8, "hiii", 1);

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, int, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, int, decimal)>  expected = (8, "hiii", 1, 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public void Combine_reverse_5Values_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, int, decimal)> result   = values.Combine(valueResult);
        MlResult<(int, string, int, int, decimal)> expected = (8, "hiii", 1, 2, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }


    [Fact]
    public async Task CombineAsync_reverse_5Values_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        MlResult<decimal> valueResult = 5.5m;

        MlResult<(int, string, int, int, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, int, int, decimal)>  expected = (8, "hiii", 1, 2, 5.5m);

        result.ToString().Should().BeEquivalentTo(expected.ToString());
    }

    [Fact]
    public async Task CombineAsync_reverse_5Values_sourceAsync_OK()
    {
        (int, string, int, int) values = (8, "hiii", 1, 2);

        Task<MlResult<decimal>> valueResult = 5.5m.ToMlResultValidAsync();

        MlResult<(int, string, int, int, decimal)>? result   = await values.CombineAsync(valueResult);
        MlResult<(int, string, int, int, decimal)>  expected = (8, "hiii", 1, 2, 5.5m);

        result!.ToString().Should().BeEquivalentTo(expected.ToString());
    }



    [Fact]
    public async void BoolToResult_falseCondition_return_Fail()
    {
        bool condition = 1 > 2;

        MlResult<bool> result = condition.BoolToResult("Error");

        MlResult<bool> expectred = "Error".ToMlResultFail<bool>();

        result.ToString().Should().BeEquivalentTo(expectred.ToString());
    }










}
