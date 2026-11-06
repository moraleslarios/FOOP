using System.ComponentModel.DataAnnotations;
using MoralesLarios.OOFP.Shared.Pagination;

namespace MoralesLarios.OOFP.Http.Tests.Unit;

public class PaginationValidationTests
{
    [Fact]
    public void PageRequest_NormalizesValuesWithinItsValidationRanges()
    {
        var pageRequest = new PageRequest(0, PageRequest.MaxPageSize + 1);

        Assert.Equal(1, pageRequest.PageNumber);
        Assert.Equal(PageRequest.MaxPageSize, pageRequest.PageSize);
        Assert.True(IsValid(pageRequest));
    }

    [Fact]
    public void PagedResult_AcceptsAnEmptyResultAndRejectsNegativeTotalCount()
    {
        var emptyResult = new PagedResult<int>([], 1, 25, 0);
        var invalidResult = emptyResult with { TotalCount = -1 };

        Assert.True(IsValid(emptyResult));
        Assert.False(IsValid(invalidResult));
    }

    [Fact]
    public void PaginationPropertiesExposeTheirValidationRanges()
    {
        AssertRange(typeof(PageRequest), nameof(PageRequest.PageNumber), 1, int.MaxValue);
        AssertRange(typeof(PageRequest), nameof(PageRequest.PageSize), 1, PageRequest.MaxPageSize);
        AssertRange(typeof(PagedResult<int>), nameof(PagedResult<int>.TotalCount), 0, int.MaxValue);
    }

    private static bool IsValid(object instance)
    {
        var validationResults = new List<ValidationResult>();
        return Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, validateAllProperties: true);
    }

    private static void AssertRange(Type declaringType, string propertyName, int minimum, int maximum)
    {
        var property = declaringType.GetProperty(propertyName);
        Assert.NotNull(property);

        var range = Attribute.GetCustomAttribute(property, typeof(RangeAttribute)) as RangeAttribute;
        Assert.NotNull(range);
        Assert.Equal(minimum, range.Minimum);
        Assert.Equal(maximum, range.Maximum);
    }
}