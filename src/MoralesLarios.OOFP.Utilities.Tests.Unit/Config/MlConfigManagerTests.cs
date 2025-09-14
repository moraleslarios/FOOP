

namespace MoralesLarios.OOFP.Utilities.Tests.Unit.Config;
public class MlConfigManagerTests(IMlConfigManager sut)
{
    private readonly IMlConfigManager _sut = sut;


    [Fact]
    public void ReadAppSettingKey_WhenKeyExists_return_valid()
    {
        string appSettingKey = "SimpleKey";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "SimpleKey";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);

        MlResult<string> expected = "SimpleValue";

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void ReadAppSettingKey_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists";
        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);
        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_WhenKeyExists_return_valid()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);

        MlResult<string> expected = "ComplexValue1";

        result.ToString().Should().Be(expected.ToString());
    }


    [Fact]
    public void ReadAppSettingKey_complexKey_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists:ComplexKey";
        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey);
        result.IsFail.Should().BeTrue();
    }



}
