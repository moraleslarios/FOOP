namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

/// <summary>
/// Tests que demuestran los arreglos aplicados sobre MlResultExecSelfExtensions.
/// </summary>
public class MlResultExecSelfFixesTests
{

    #region Helpers


    private static MlResult<int> FailWithoutException(string message = "miError")
        => message.ToMlResultFail<int>();

    private static MlResult<int> FailWithException(string message = "miError")
        => (message,
                new Dictionary<string, object>
                {
                    { Constants.ExceptionMetadataKey, new Exception("miException") },
                    { "key2", "value2" }
                }
            );


    #endregion


    #region ExecSelfIfFailWithoutExceptionAsync - paridad con la version sincrona


    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_sourceFailWithoutException_ExecuteActionFail()
    {
        var partialResult = FailWithoutException();

        var internalResult = 0;

        var result = await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            internalResult += 1;

            await Task.CompletedTask;
        });

        internalResult.Should().Be(1);
        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_sourceFailWithException_NotExecuteActionFail()
    {
        var partialResult = FailWithException();

        var internalResult = 0;

        var result = await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            internalResult += 1;

            await Task.CompletedTask;
        });

        internalResult.Should().Be(0);
        result.IsFailure.Should().BeTrue();
    }


    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_sourceValid_NotExecuteActionFail()
    {
        MlResult<int> partialResult = 1;

        var internalResult = 0;

        var result = await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            internalResult += 1;

            await Task.CompletedTask;
        });

        internalResult.Should().Be(0);
        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be(1);
    }


    /// <summary>
    /// Arreglo principal: la version asincrona recibia el 'errorsResult' interno,
    /// que contiene un error extra ("The key Ex does not exist in the details"),
    /// en lugar del 'errorsDetails' original que recibe la version sincrona.
    /// </summary>
    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_ShouldReceiveSameErrorsDetailsThanSyncVersion()
    {
        var partialResult = FailWithoutException();

        MlErrorDetails syncErrors  = null!;
        MlErrorDetails asyncErrors = null!;

        partialResult.ExecSelfIfFailWithoutException(errorsDetails => syncErrors = errorsDetails);

        await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            asyncErrors = errorsDetails;

            await Task.CompletedTask;
        });

        syncErrors .Should().NotBeNull();
        asyncErrors.Should().NotBeNull();

        asyncErrors.Errors.Count().Should().Be(syncErrors.Errors.Count());

        asyncErrors.Errors.Select(x => x.Message)
                          .Should()
                          .BeEquivalentTo(syncErrors.Errors.Select(x => x.Message));
    }


    /// <summary>
    /// La accion no debe recibir el error sintetico de clave inexistente.
    /// </summary>
    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_ShouldNotReceiveMissingKeySyntheticError()
    {
        var partialResult = FailWithoutException();

        MlErrorDetails asyncErrors = null!;

        await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            asyncErrors = errorsDetails;

            await Task.CompletedTask;
        });

        asyncErrors.Should().NotBeNull();

        asyncErrors.Errors.Should().HaveCount(1);

        asyncErrors.Errors.Select(x => x.Message)
                          .Should()
                          .NotContain(x => x.Contains(Constants.ExceptionMetadataKey) && x.Contains("does not exist"));

        asyncErrors.Errors.First().Message.Should().Be("miError");
    }


    [Fact]
    public async Task ExecSelfIfFailWithoutExceptionAsync_TaskSource_ShouldReceiveOriginalErrorsDetails()
    {
        var partialResult = FailWithoutException().ToAsync();

        MlErrorDetails asyncErrors = null!;

        var result = await partialResult.ExecSelfIfFailWithoutExceptionAsync(async errorsDetails =>
        {
            asyncErrors = errorsDetails;

            await Task.CompletedTask;
        });

        result.IsFailure.Should().BeTrue();

        asyncErrors.Should().NotBeNull();
        asyncErrors.Errors.Should().HaveCount(1);
        asyncErrors.Errors.First().Message.Should().Be("miError");
    }


    #endregion

}
