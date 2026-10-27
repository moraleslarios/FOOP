namespace MoralesLarios.OOFP.Core.Tests.Unit.Helpers;

public class EnsureFpAsyncTests
{

    #region ThatAsync con fuente asíncrona

    [Fact]
    public async Task ThatAsync_withAsyncSourceAndPredicateOk_return_Valid()
    {
        var result = await EnsureFp.ThatAsync(Task.FromResult(10), x => x > 5, "error");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be(10);
    }

    [Fact]
    public async Task ThatAsync_withAsyncSourceAndPredicateKo_return_Fail()
    {
        var result = await EnsureFp.ThatAsync(Task.FromResult(1), x => x > 5, "error");

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.First().Message.Should().Be("error");
    }

    [Fact]
    public async Task ThatAsync_withNullTask_return_Fail()
    {
        var result = await EnsureFp.ThatAsync<string>(null!, x => x is not null, "error");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withAsyncSourceAndErrorsDetails_return_Fail()
    {
        var errorsDetails = MlErrorsDetails.FromErrorMessage("detalle");

        var result = await EnsureFp.ThatAsync(Task.FromResult(1), x => x > 5, errorsDetails);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.First().Message.Should().Be("detalle");
    }

    [Fact]
    public async Task ThatAsync_withAsyncSourceAndCondition_return_Valid()
    {
        var result = await EnsureFp.ThatAsync(Task.FromResult("ok"), true, "error");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be("ok");
    }

    #endregion

    #region ThatAsync con predicado asíncrono

    [Fact]
    public async Task ThatAsync_withAsyncPredicateOk_return_Valid()
    {
        var result = await EnsureFp.ThatAsync(10, x => Task.FromResult(x > 5), "error");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withAsyncPredicateKo_return_Fail()
    {
        var result = await EnsureFp.ThatAsync(1, x => Task.FromResult(x > 5), "error");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withNullAsyncPredicate_return_Fail()
    {
        var result = await EnsureFp.ThatAsync(10, (Func<int, Task<bool>>)null!, "error");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withAsyncSourceAndAsyncPredicate_return_Valid()
    {
        var result = await EnsureFp.ThatAsync(Task.FromResult(10), x => Task.FromResult(x > 5), "error");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withCancellableAsyncPredicate_return_Valid()
    {
        var result = await EnsureFp.ThatAsync(10, (x, ct) => Task.FromResult(x > 5), "error", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ThatAsync_withNullCancellableAsyncPredicate_return_Fail()
    {
        var result = await EnsureFp.ThatAsync(10, (Func<int, CancellationToken, Task<bool>>)null!, "error");

        result.IsFailure.Should().BeTrue();
    }

    #endregion

    #region ThatArgAsync

    [Fact]
    public async Task ThatArgAsync_withAsyncPredicateKo_addParamNameDetail()
    {
        var quantity = 1;

        var result = await EnsureFp.ThatArgAsync(quantity, x => Task.FromResult(x > 5));

        result.IsFailure.Should().BeTrue();

        var details = result.GetFailureDetailsOrThrow();

        details.Details[PARAM_NAME_KEY].Should().Be("quantity");
        details.Details[ValueMetadataKey].Should().Be(1);
    }

    [Fact]
    public async Task ThatArgAsync_withAsyncSource_return_Valid()
    {
        var result = await EnsureFp.ThatArgAsync(Task.FromResult(10), x => x > 5);

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be(10);
    }

    #endregion

    #region TryThatAsync

    [Fact]
    public async Task TryThatAsync_withThrowingAsyncPredicate_captureException()
    {
        var myValue = 15;

        var result = await EnsureFp.TryThatAsync(myValue, x => throw new InvalidOperationException("boom"), "error");

        result.IsFailure.Should().BeTrue();

        var details = result.GetFailureDetailsOrThrow();

        details.Details.Should().ContainKey(ExceptionMetadataKey);
        details.Details[PARAM_NAME_KEY].Should().Be("myValue");
    }

    [Fact]
    public async Task TryThatAsync_withoutException_return_Valid()
    {
        var result = await EnsureFp.TryThatAsync(10, x => Task.FromResult(x > 5), "error");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task TryThatAsync_withErrorMessageBuilder_useExceptionMessage()
    {
        var result = await EnsureFp.TryThatAsync<int>(5,
                                                     x => throw new InvalidOperationException("boom"),
                                                     ex => $"Fallo controlado: {ex.Message}");

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Errors.First().Message.Should().Be("Fallo controlado: boom");
    }

    [Fact]
    public async Task TryThatAsync_withErrorsDetailsAndException_captureException()
    {
        var errorsDetails = MlErrorsDetails.FromErrorMessage("detalle");

        var result = await EnsureFp.TryThatAsync<int>(5, x => throw new InvalidOperationException("boom"), errorsDetails);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Details.Should().ContainKey(ExceptionMetadataKey);
    }

    #endregion

    #region Guardias clásicas con fuente asíncrona

    [Fact]
    public async Task NotNullAsync_withAsyncSourceOk_return_Valid()
    {
        var result = await EnsureFp.NotNullAsync(Task.FromResult("texto"), "error");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task NotNullAsync_withAsyncSourceNull_return_Fail()
    {
        var result = await EnsureFp.NotNullAsync(Task.FromResult<string>(null!), "error");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task NotNullArgAsync_addParamNameDetail()
    {
        var customerAsync = Task.FromResult<string>(null!);

        var result = await EnsureFp.NotNullArgAsync(customerAsync);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Details[PARAM_NAME_KEY].Should().Be("customerAsync");
    }

    [Fact]
    public async Task NotEmptyAsync_withAsyncSourceOk_return_Valid()
    {
        var result = await EnsureFp.NotEmptyAsync(Task.FromResult<IEnumerable<int>>(new[] { 1, 2 }), "error");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task NotEmptyArgAsync_withEmptyCollection_return_Fail()
    {
        var itemsAsync = Task.FromResult<IEnumerable<int>>(Array.Empty<int>());

        var result = await EnsureFp.NotEmptyArgAsync(itemsAsync);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Details[PARAM_NAME_KEY].Should().Be("itemsAsync");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task NotNullEmptyOrWhitespaceAsync_withEmptyValues_return_Fail(string? value)
    {
        var result = await EnsureFp.NotNullEmptyOrWhitespaceAsync(Task.FromResult(value!), "error");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task NotNullEmptyOrWhitespaceArgAsync_addParamNameDetail()
    {
        var nameAsync = Task.FromResult("  ");

        var result = await EnsureFp.NotNullEmptyOrWhitespaceArgAsync(nameAsync);

        result.IsFailure.Should().BeTrue();
        result.GetFailureDetailsOrThrow().Details[PARAM_NAME_KEY].Should().Be("nameAsync");
    }

    [Fact]
    public async Task NotNullValueAsync_withValue_return_unwrappedValue()
    {
        var result = await EnsureFp.NotNullValueAsync(Task.FromResult<int?>(7), "error");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().Should().Be(7);
    }

    [Fact]
    public async Task NotNullValueAsync_withNull_return_Fail()
    {
        var result = await EnsureFp.NotNullValueAsync(Task.FromResult<int?>(null), "error");

        result.IsFailure.Should().BeTrue();
    }

    #endregion

}
