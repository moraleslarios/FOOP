namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class EnumRangeValueObjectTests
{
    private enum TestStatus
    {
        None,
        Active,
        Disabled
    }

    [Fact]
    public void EnumRangeValueObject_ByString_should_accept_valid_enum_value()
    {
        var result = EnumRangeValueObject<TestStatus>.FromString("Active");

        result.IsSuccess.Should().BeTrue();
        result.GetValueOrThrow().GetEnumValue().Should().Be(TestStatus.Active);
    }

    [Fact]
    public void EnumRangeValueObject_ByString_should_reject_invalid_enum_value()
    {
        var result = EnumRangeValueObject<TestStatus>.FromString("Unknown");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void EnumRangeValueObject_FromEnum_should_convert_back_to_enum()
    {
        var value = EnumRangeValueObject<TestStatus>.CreateFromEnum(TestStatus.Disabled);

        value.GetEnumValue().Should().Be(TestStatus.Disabled);
    }
}
