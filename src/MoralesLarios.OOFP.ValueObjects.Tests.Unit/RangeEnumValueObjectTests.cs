namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class RangeEnumValueObjectTests
{
    private enum TestStatus
    {
        None,
        Active,
        Disabled
    }

    [Fact]
    public void RangeEnumValueObject_ByString_should_accept_valid_enum_value()
    {
        var result = RangeEnumValueObject<TestStatus>.ByString("Active");

        result.IsValid.Should().BeTrue();
        result.SecureValidValue().GetEnumValue().Should().Be(TestStatus.Active);
    }

    [Fact]
    public void RangeEnumValueObject_ByString_should_reject_invalid_enum_value()
    {
        var result = RangeEnumValueObject<TestStatus>.ByString("Unknown");

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void RangeEnumValueObject_FromEnum_should_convert_back_to_enum()
    {
        var value = RangeEnumValueObject<TestStatus>.FromEnum(TestStatus.Disabled);

        value.GetEnumValue().Should().Be(TestStatus.Disabled);
    }
}
