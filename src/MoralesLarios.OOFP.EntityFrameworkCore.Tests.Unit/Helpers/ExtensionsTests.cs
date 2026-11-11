namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit.Helpers;

public class ExtensionsTests
{
    [Fact]
    public void FormatCompositeKey_SingleValue_ReturnsValueAsText()
    {
        var result = new object[] { 17 }.FormatCompositeKey();

        result.Should().Be("17");
    }

    [Fact]
    public void FormatCompositeKey_CompositeValues_PreservesValueOrder()
    {
        var result = new object[] { 4, "vino" }.FormatCompositeKey();

        result.Should().Be("(4, vino)");
    }

    [Fact]
    public void PrivateOrderBy_Ascending_OrdersBySelectedValue()
    {
        var result = CreateRows().AsQueryable()
            .PrivateOrderBy(SortDirection.Ascending, row => row.SortValue);

        result.Select(row => row.SortValue).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void PrivateOrderBy_Descending_OrdersBySelectedValue()
    {
        var result = CreateRows().AsQueryable()
            .PrivateOrderBy(SortDirection.Descending, row => row.SortValue);

        result.Select(row => row.SortValue).Should().Equal(3, 2, 1);
    }

    [Fact]
    public void PrivateOrderBy_NoSelector_ReturnsOriginalQuery()
    {
        var source = CreateRows().AsQueryable();

        var result = source.PrivateOrderBy(orderByField: null!);

        result.Should().BeSameAs(source);
    }

    private static TestEntity[] CreateRows()
        => [new() { SortValue = 3 }, new() { SortValue = 1 }, new() { SortValue = 2 }];
}