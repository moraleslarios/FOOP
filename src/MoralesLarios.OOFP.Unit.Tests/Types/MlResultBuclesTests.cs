// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

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


    [Fact]
    public async Task ProjectionAsync_sourceAsync_syncFunc_When_All_completeFuncTransforms_OK_return_valid()
    {
        Task<IEnumerable<TestType>> sourceAsync = ((IEnumerable<TestType>)new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(2, "Name2", DateTime.Now)
        }).ToAsync();

        MlResult<IEnumerable<TestType>> result = await sourceAsync.ProjectionAsync<TestType, TestType>(x => x with { Date = DateTime.MinValue });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ProjectionAsync_sourceAsync_syncFunc_When_completeFuncTransformGenerateError_return_Fail()
    {
        Task<IEnumerable<TestType>> sourceAsync = ((IEnumerable<TestType>)new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(0, "Name2", DateTime.Now)
        }).ToAsync();

        MlResult<IEnumerable<TestType>> result = await sourceAsync.ProjectionAsync<TestType, TestType>(x => x.Id == 0
                                                                                        ? $"Error {x.Name}".ToMlResultFail<TestType>()
                                                                                        : (x with { Date = DateTime.MinValue }));

        MlResult<IEnumerable<TestType>> expected = MlResult<IEnumerable<TestType>>.Fail("Error Name2");

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public async Task ProjectionWhileAsync_sourceAsync_syncFunc_When_2_completeFuncTransformGenerateError_return_Fail_with_first_error()
    {
        Task<IEnumerable<TestType>> sourceAsync = ((IEnumerable<TestType>)new List<TestType>
        {
            new TestType(1, "Name1", DateTime.Now),
            new TestType(0, "Name2", DateTime.Now),
            new TestType(0, "Name3", DateTime.Now)
        }).ToAsync();

        MlResult<IEnumerable<TestType>> result = await sourceAsync.ProjectionWhileAsync(x => x.Id == 0
                                                                                              ? $"Error {x.Name}".ToMlResultFail<TestType>()
                                                                                              : (x with { Date = DateTime.MinValue }));

        MlResult<IEnumerable<TestType>> expected = MlResult<IEnumerable<TestType>>.Fail("Error Name2");

        result.ToString().Should().Be(expected.ToString());
    }





    [Fact]
    public void ProjectionSplit_when_completeFuncTransform_isNull_return_fail()
    {
        List<string> colec = ["uno", "dos", "tres"];

        Func<string, MlResult<int>> completeFuncTransform = null!;

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = colec.ProjectionSplit(completeFuncTransform);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ProjectionSplit_when_sourceContainsElmentsNull_thisElements_NotEvaluated_return_valid_withoutthisElements()
    {
        List<string> colec = ["uno", null!, "tres"];

        Func<string, MlResult<int>> completeFuncTransform = x => x.Length.ToMlResultValid<int>();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = colec.ProjectionSplit(completeFuncTransform);

        result.IsValid.Should().BeTrue();

        result.SecureValidValue().valids.Count.Should().Be(2);
    }

    [Fact]
    public void ProjectionSplit_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements()
    {
        List<string> colec = ["uno", "dos", "tres"];

        Func<string, MlResult<int>> completeFuncTransform = x => x.Length.ToMlResultValid<int>();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = colec.ProjectionSplit(completeFuncTransform);

        result.IsValid.Should().BeTrue();

        result.SecureValidValue().valids.Count.Should().Be(3);
        result.SecureValidValue().fails .Count.Should().Be(0);
    }

    [Fact]
    public void ProjectionSplit_when_allSourceValues_completeFuncTransform_areFail_return_fail_with_allElements()
    {
        List<string> colec = ["uno", "dos", "tres"];

        Func<string, MlResult<int>> completeFuncTransform = x => $"Error {x}".ToMlResultFail<int>();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = colec.ProjectionSplit(completeFuncTransform);

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(0);
        result.SecureValidValue().fails .Count.Should().Be(3);
    }

    [Fact]
    public async Task ProjectionSplitAsync_source_when_allSourceValues_completeFuncTransform_areValid_return_valid_with_allElements()
    {
        List<string> colec = ["uno", "dos", "tres"];

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colec.ProjectionSplitAsync(x => x.Length.ToMlResultValid<int>());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(3);
        result.SecureValidValue().fails.Count.Should().Be(0);
        result.SecureValidValue().valids["uno"].Should().Be(3);
        result.SecureValidValue().valids["dos"].Should().Be(3);
        result.SecureValidValue().valids["tres"].Should().Be(4);
    }

    [Fact]
    public async Task ProjectionSplitAsync_source_when_allSourceValues_completeFuncTransform_areFail_return_valid_with_allFails()
    {
        List<string> colec = ["uno", "dos", "tres"];

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colec.ProjectionSplitAsync(x => $"Error {x}".ToMlResultFail<int>());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(0);
        result.SecureValidValue().fails.Count.Should().Be(3);
        result.SecureValidValue().fails["uno"].ToString().Should().Contain("Error uno");
        result.SecureValidValue().fails["dos"].ToString().Should().Contain("Error dos");
        result.SecureValidValue().fails["tres"].ToString().Should().Contain("Error tres");
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_syncTransform_when_allSourceValues_areValid_return_valid_with_allElements()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", "dos", "tres"]).ToAsync();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(x => x.Length.ToMlResultValid<int>());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(3);
        result.SecureValidValue().fails.Count.Should().Be(0);
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areValid_return_valid_with_allElements()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", "dos", "tres"]).ToAsync();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(x => Task.FromResult(x.Length.ToMlResultValid<int>()));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(3);
        result.SecureValidValue().fails.Count.Should().Be(0);
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_asyncTransform_when_allSourceValues_areFail_return_valid_with_allFails()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", "dos", "tres"]).ToAsync();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(x => Task.FromResult($"Error {x}".ToMlResultFail<int>()));

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(0);
        result.SecureValidValue().fails.Count.Should().Be(3);
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_when_sourceContainsElementsNull_theseElements_NotEvaluated_return_valid_withoutThoseElements()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", null!, "tres"]).ToAsync();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(x => x.Length.ToMlResultValid<int>());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(2);
        result.SecureValidValue().fails.Count.Should().Be(0);
        result.SecureValidValue().valids["uno"].Should().Be(3);
        result.SecureValidValue().valids["tres"].Should().Be(4);
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_syncTransform_when_mixedValidAndFail_return_valid_with_splitValues()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", "dos", "tres", "cuatro"]).ToAsync();

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(x => x == "dos" || x == "cuatro"
            ? $"Error {x}".ToMlResultFail<int>()
            : x.Length.ToMlResultValid<int>());

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().valids.Count.Should().Be(2);
        result.SecureValidValue().fails.Count.Should().Be(2);
        result.SecureValidValue().valids["uno"].Should().Be(3);
        result.SecureValidValue().valids["tres"].Should().Be(4);
        result.SecureValidValue().fails["dos"].ToString().Should().Contain("Error dos");
        result.SecureValidValue().fails["cuatro"].ToString().Should().Contain("Error cuatro");
    }

    [Fact]
    public async Task ProjectionSplitAsync_sourceAsync_asyncTransform_when_transformIsNull_return_fail()
    {
        Task<IEnumerable<string>> colecAsync = ((IEnumerable<string>)["uno", "dos", "tres"]).ToAsync();

        Func<string, Task<MlResult<int>>> completeFuncTransformAsync = null!;

        MlResult<(Dictionary<string, int> valids, Dictionary<string, MlErrorsDetails> fails)> result = await colecAsync.ProjectionSplitAsync(completeFuncTransformAsync);

        result.IsFail.Should().BeTrue();
    }


    #region FusionFailErros


    // Comprobación de la incidencia reportada: "FusionFailErros pierde errores por un return que falta".
    // Este test PASA: la fusión de 3 fallos devuelve los 3 mensajes. El return final SÍ existe.
    [Fact]
    public void FusionFailErros_when_source_has_3_fails_return_fail_with_the_3_errorMessages()
    {
        IEnumerable<MlResult<int>> source =
        [
            "Error 1".ToMlResultFail<int>(),
            "Error 2".ToMlResultFail<int>(),
            "Error 3".ToMlResultFail<int>()
        ];

        MlResult<IEnumerable<int>> result = source.FusionFailErros();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails()
              .ToErrorsMessages()
              .Should().BeEquivalentTo(new[] { "Error 1", "Error 2", "Error 3" });
    }


    // Sólo se fusionan los elementos en fallo, los válidos se ignoran.
    [Fact]
    public void FusionFailErros_when_source_has_validsAndFails_return_fail_onlyWith_failsErrorMessages()
    {
        IEnumerable<MlResult<int>> source =
        [
            1.ToMlResultValid(),
            "Error 2".ToMlResultFail<int>(),
            3.ToMlResultValid(),
            "Error 4".ToMlResultFail<int>()
        ];

        MlResult<IEnumerable<int>> result = source.FusionFailErros();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails()
              .ToErrorsMessages()
              .Should().BeEquivalentTo(new[] { "Error 2", "Error 4" });
    }


    // Defecto ya corregido: la guarda
    //     if ( ! partialResult.Any()) return MlResult<IEnumerable<T>>.Fail("No elements found in failed state to merge");
    // ahora SÍ devuelve el fallo, por lo que no se llega a partialResult.First()
    // y no se lanza InvalidOperationException.
    [Fact]
    public void FusionFailErros_when_source_hasNoFails_notThrows_and_return_fail()
    {
        IEnumerable<MlResult<int>> source = [1.ToMlResultValid(), 2.ToMlResultValid()];

        Action act = () => source.FusionFailErros();

        act.Should().NotThrow();
        source.FusionFailErros().IsFail.Should().BeTrue();
    }


    [Fact]
    public void FusionFailErros_when_source_isEmpty_notThrows_and_return_fail_with_noElementsMessage()
    {
        IEnumerable<MlResult<int>> source = [];

        Action act = () => source.FusionFailErros();

        act.Should().NotThrow();

        MlResult<IEnumerable<int>> result = source.FusionFailErros();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails()
              .ToErrorsMessages()
              .Should().Contain("No elements found in failed state to merge");
    }


    // Comportamiento correcto tras el arreglo del return que faltaba.
    [Fact]
    public void FusionFailErros_when_source_hasNoFails_should_return_fail_with_noElementsMessage()
    {
        IEnumerable<MlResult<int>> source = [1.ToMlResultValid(), 2.ToMlResultValid()];

        MlResult<IEnumerable<int>> result = source.FusionFailErros();

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails()
              .ToErrorsMessages()
              .Should().Contain("No elements found in failed state to merge");
    }


    #endregion FusionFailErros

}

public record TestType(int Id, string Name, DateTime Date);

public record TestType2(int Id, string Name, DateTime Date, string Desc = null!);









