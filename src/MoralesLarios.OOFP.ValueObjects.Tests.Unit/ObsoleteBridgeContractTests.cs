namespace MoralesLarios.OOFP.ValueObjects.Tests.Unit;

public class ObsoleteBridgeContractTests
{
    [Fact]
    public void LengthBridges_ExposeCorrectParameterNames()
    {
        AssertParameterNames(typeof(StringLengthBetween), "FromStringLenght", "value", "minLength", "maxLength");
        AssertParameterNames(typeof(StringMaxLength), "FromStringLenght", "value", "length");
        AssertParameterNames(typeof(StringMinLength), "FromStringLenght", "value", "length");
        AssertParameterNames(typeof(IntBetween), "FromIntLenght", "value", "minLength", "maxLength");
        AssertParameterNames(typeof(IntLessThan), "FromIntLenght", "value", "length");
        AssertParameterNames(typeof(IntMoreThan), "FromIntLenght", "value", "length");
    }

    [Fact]
    public void StringLengthBridges_RecommendExistingCreateFactory()
    {
        AssertObsoleteMessage(typeof(StringLengthBetween), "FromStringLenght");
        AssertObsoleteMessage(typeof(StringMaxLength), "FromStringLenght");
        AssertObsoleteMessage(typeof(StringMinLength), "FromStringLenght");
    }

    private static void AssertParameterNames(Type declaringType, string methodName, params string[] expected)
    {
        var method = declaringType.GetMethod(methodName);

        Assert.NotNull(method);
        Assert.Equal(expected, method.GetParameters().Select(parameter => parameter.Name));
    }

    private static void AssertObsoleteMessage(Type declaringType, string methodName)
    {
        var method = declaringType.GetMethod(methodName);

        Assert.NotNull(method);
        var obsolete = Attribute.GetCustomAttribute(method, typeof(ObsoleteAttribute)) as ObsoleteAttribute;

        Assert.NotNull(obsolete);
        Assert.Contains("Renamed to CreateFromLength", obsolete.Message);
    }
}