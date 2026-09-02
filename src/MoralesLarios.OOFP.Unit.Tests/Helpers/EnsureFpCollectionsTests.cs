namespace MoralesLarios.OOFP.Unit.Tests.Helpers;

public class EnsureFpCollectionsTests
{

    #region NotEmptyCollection

    [Fact]
    public void NotEmptyCollection_preserveConcreteType()
    {
        var source = new List<int> { 1, 2, 3 };

        var result = EnsureFp.NotEmptyCollection<List<int>, int>(source, "error");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().Should().BeSameAs(source);
    }

    [Fact]
    public void NotEmptyCollection_withEmptyCollection_return_Fail()
        => EnsureFp.NotEmptyCollection<List<int>, int>(new List<int>(), "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotEmptyCollection_withNullCollection_return_Fail()
        => EnsureFp.NotEmptyCollection<List<int>, int>(null!, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NotEmptyCollectionArg_addParamNameDetail()
    {
        var items = Array.Empty<string>();

        var result = EnsureFp.NotEmptyCollectionArg<string[], string>(items);

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Details[PARAM_NAME_KEY].Should().Be("items");
    }

    #endregion

    #region Cardinalidad

    [Theory]
    [InlineData(3, 3, true)]
    [InlineData(3, 4, false)]
    public void CountExactly_evaluateCount(int itemsCount, int expectedCount, bool expectedValid)
        => EnsureFp.CountExactly(Enumerable.Range(1, itemsCount), expectedCount, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(3, 2, true)]
    [InlineData(3, 3, true)]
    [InlineData(3, 4, false)]
    public void CountAtLeast_evaluateCount(int itemsCount, int minCount, bool expectedValid)
        => EnsureFp.CountAtLeast(Enumerable.Range(1, itemsCount), minCount, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(3, 4, true)]
    [InlineData(3, 3, true)]
    [InlineData(3, 2, false)]
    public void CountAtMost_evaluateCount(int itemsCount, int maxCount, bool expectedValid)
        => EnsureFp.CountAtMost(Enumerable.Range(1, itemsCount), maxCount, "error").IsValid.Should().Be(expectedValid);

    [Theory]
    [InlineData(3, 2, 5, true)]
    [InlineData(1, 2, 5, false)]
    [InlineData(6, 2, 5, false)]
    public void CountBetween_evaluateCount(int itemsCount, int min, int max, bool expectedValid)
        => EnsureFp.CountBetween(Enumerable.Range(1, itemsCount), min, max, "error").IsValid.Should().Be(expectedValid);

    [Fact]
    public void CountExactlyArg_addExpectedDetail()
    {
        var lines = new[] { "a", "b" };

        var result = EnsureFp.CountExactlyArg(lines, 5);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("lines");
        details.Details[EXPECTED_KEY].Should().Be(5);
    }

    [Fact]
    public void CountAtLeast_withNullCollection_return_Fail()
        => EnsureFp.CountAtLeast<int>(null!, 1, "error").IsFail.Should().BeTrue();

    #endregion

    #region Predicados sobre elementos

    [Fact]
    public void AllMatch_withAllItemsOk_return_Valid()
        => EnsureFp.AllMatch(new[] { 2, 4, 6 }, x => x % 2 == 0, "error").IsValid.Should().BeTrue();

    [Fact]
    public void AllMatch_withFailedItems_return_FailWithIndexes()
    {
        var numbers = new[] { 2, 3, 4, 5 };

        var result = EnsureFp.AllMatchArg(numbers, x => x % 2 == 0);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("numbers");
        details.Details[FAILED_INDEXES_KEY].As<IEnumerable<int>>().Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Fact]
    public void NoneMatch_withNoItemsMatching_return_Valid()
        => EnsureFp.NoneMatch(new[] { 1, 3, 5 }, x => x % 2 == 0, "error").IsValid.Should().BeTrue();

    [Fact]
    public void NoneMatch_withSomeItemMatching_return_Fail()
        => EnsureFp.NoneMatch(new[] { 1, 2, 3 }, x => x % 2 == 0, "error").IsFail.Should().BeTrue();

    [Fact]
    public void AnyMatch_withSomeItemMatching_return_Valid()
        => EnsureFp.AnyMatch(new[] { 1, 2, 3 }, x => x > 2, "error").IsValid.Should().BeTrue();

    [Fact]
    public void AnyMatch_withNoItemMatching_return_Fail()
        => EnsureFp.AnyMatch(new[] { 1, 2, 3 }, x => x > 10, "error").IsFail.Should().BeTrue();

    #endregion

    #region Duplicados, nulos y pertenencia

    [Fact]
    public void NoDuplicates_withUniqueItems_return_Valid()
        => EnsureFp.NoDuplicates(new[] { "a", "b", "c" }, "error").IsValid.Should().BeTrue();

    [Fact]
    public void NoDuplicates_withDuplicatedItems_return_Fail()
        => EnsureFp.NoDuplicates(new[] { "a", "b", "a" }, "error").IsFail.Should().BeTrue();

    [Fact]
    public void NoDuplicates_withIgnoreCaseComparer_return_Fail()
        => EnsureFp.NoDuplicates(new[] { "a", "A" }, "error", StringComparer.OrdinalIgnoreCase).IsFail.Should().BeTrue();

    [Fact]
    public void NoNullItems_withoutNulls_return_Valid()
        => EnsureFp.NoNullItems(new[] { "a", "b" }, "error").IsValid.Should().BeTrue();

    [Fact]
    public void NoNullItemsArg_withNulls_return_FailWithIndexes()
    {
        var names = new[] { "a", null, "c", null };

        var result = EnsureFp.NoNullItemsArg(names);

        result.IsFail.Should().BeTrue();

        var details = result.SecureFailErrorsDetails();

        details.Details[PARAM_NAME_KEY].Should().Be("names");
        details.Details[FAILED_INDEXES_KEY].As<IEnumerable<int>>().Should().BeEquivalentTo(new[] { 1, 3 });
    }

    [Fact]
    public void ContainsItem_withExistingItem_return_Valid()
        => EnsureFp.ContainsItem(new[] { 1, 2, 3 }, 2, "error").IsValid.Should().BeTrue();

    [Fact]
    public void ContainsItem_withMissingItem_return_Fail()
        => EnsureFp.ContainsItem(new[] { 1, 2, 3 }, 9, "error").IsFail.Should().BeTrue();

    [Fact]
    public void ContainsItemArg_buildAutomaticMessage()
    {
        var roles = new[] { "user", "editor" };

        var result = EnsureFp.ContainsItemArg(roles, "admin");

        result.IsFail.Should().BeTrue();
        result.SecureFailErrorsDetails().Errors.First().Message.Should().Contain("admin");
    }

    #endregion

}
