using System.Threading.Tasks;

namespace MoralesLarios.OOFP.Unit.Tests.Types;
public class MlResultBuclesTests
{


    [Fact]
    public void Projection_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(0, "Name2", DateTime.Now),
            new TestType(0, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                                   $"Error {x.Name}".ToMlResultFail<TestType>() : 
                                                                                   ( x with { Date = DateTime.Now.AddYears(-1) } ));

        MlResult<IEnumerable<TestType>> expected = MlResult<IEnumerable<TestType>>.Fail("Error Name2", "Error Name3");

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void Projection_When_All_completeFuncTransforms_OK_return_valid()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                                $"Error {x.Name}".ToMlResultFail<TestType>() : 
                                                                                ( x with { Date = DateTime.Now.AddYears(-1) } ));

        MlResult<IEnumerable<TestType>> expected = MlResult<IEnumerable<TestType>>.Fail("Error Name2", "Error Name3");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Projection_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                                $"Error {x.Name}".ToMlResultFail<TestType>() : 
                                                                                ( x with { Date = DateTime.Now.AddYears(-1) } ));

        var resultValue = result.Match(x => x,  x => new List<TestType>());

        resultValue.Count().Should().Be(3);
    }

    [Fact]
    public void Projection_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                                $"Error {x.Name}".ToMlResultFail<TestType>() : 
                                                                                ( x with { Date = DateTime.MinValue } ));

        var resultValue = result.Match(x => x,  x => new List<TestType>());

        resultValue.All(x => x.Date == DateTime.MinValue).Should().BeTrue();
    }

    [Fact]
    public void Projection_differentResult_When_2_completeFuncTransformGenerateError_return_Fail_with_2_errors_Concat()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(0, "Name2", DateTime.Now),
            new TestType(0, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType2>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                           $"Error {x.Name}".ToMlResultFail<TestType2>() : 
                                                                           ( new TestType2(x.Id, x.Name, DateTime.Now.AddYears(-1) )));

        MlResult<IEnumerable<TestType2>> expected = MlResult<IEnumerable<TestType2>>.Fail("Error Name2", "Error Name3");

        result.Should().BeEquivalentTo(expected);
    }




    [Fact]
    public void Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType2>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                           $"Error {x.Name}".ToMlResultFail<TestType2>() : 
                                                                           ( new TestType2(x.Id, x.Name, DateTime.Now.AddYears(-1) )));

        MlResult<IEnumerable<TestType2>> expected = MlResult<IEnumerable<TestType2>>.Fail("Error Name2", "Error Name3");

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_in_Value()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType2>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                          $"Error {x.Name}".ToMlResultFail<TestType2>() : 
                                                                          ( new TestType2(x.Id, x.Name, DateTime.Now.AddYears(-1) )));

        var resultValue = result.Match(x => x,  x => new List<TestType2>());

        resultValue.Count().Should().Be(3);
    }

    [Fact]
    public void Projection_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType2>> result = IEnumerable.Projection(x => x.Id == 0 ? 
                                                                           $"Error {x.Name}".ToMlResultFail<TestType2>() : 
                                                                           ( new TestType2(x.Id, x.Name, DateTime.MinValue )));

        var resultValue = result.Match(x => x,  x => new List<TestType2>());

        resultValue.All(x => x.Date == DateTime.MinValue).Should().BeTrue();
    }

    [Fact]
    public void ProjectionWhile_When_2_completeFuncTransformGenerateError_return_Fail_with_1_errors_Concat()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(0, "Name2", DateTime.Now),
            new TestType(0, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType>> result = IEnumerable.ProjectionWhile(x => x.Id == 0 ?
                                                                                         $"Error {x.Name}".ToMlResultFail<TestType>() :
                                                                                         (x with { Date = DateTime.Now.AddYears(-1) }));

        MlResult<IEnumerable<TestType>> expected = MlResult<IEnumerable<TestType>>.Fail("Error Name2");

        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public async Task ProjectionParallelAsync_differentResult_When_All_completeFuncTransforms_OK_return_valid_with_allElements_whit_correctTransform()
    {
        var IEnumerable = new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now),
            new TestType(3, "Name3", DateTime.Now)
        };

        MlResult<IEnumerable<TestType2>> result = await IEnumerable.ProjectionParallelAsync(x => x.Id == 0 ?
                                                                                              ($"Error {x.Name}".ToMlResultFailAsync<TestType2>()) :
                                                                                              (new TestType2(x.Id, x.Name, DateTime.MinValue).ToMlResultValidAsync()));

        var resultValue = result.Match(x => x, x => new List<TestType2>());

        resultValue.All(x => x.Date == DateTime.MinValue).Should().BeTrue();
    }

}



public record TestType(int Id, string Name, DateTime Date);

public record TestType2(int Id, string Name, DateTime Date, string Desc = null!);
