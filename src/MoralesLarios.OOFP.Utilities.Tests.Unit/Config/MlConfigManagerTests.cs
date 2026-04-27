// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0



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



    [Fact]
    public void ReadAppSettingKey_withErrorsDetails_WhenKeyExists_return_valid()
    {
        string appSettingKey = "SimpleKey";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_withErrorsDetails_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "SimpleKey";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        MlResult<string> expected = "SimpleValue";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ReadAppSettingKey_withErrorsDetails_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorsDetails_WhenKeyExists_return_valid()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorsDetails_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        MlResult<string> expected = "ComplexValue1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorsDetails_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists:ComplexKey";
        MoralesLarios.OOFP.Types.Errors.MlErrorsDetails errorsDetails = MoralesLarios.OOFP.Types.Errors.MlErrorsDetails.FromErrorMessage("Error personalizado");

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorsDetails);

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void ReadAppSettingKey_withErrorMessage_WhenKeyExists_return_valid()
    {
        string appSettingKey = "SimpleKey";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_withErrorMessage_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "SimpleKey";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        MlResult<string> expected = "SimpleValue";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ReadAppSettingKey_withErrorMessage_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorMessage_WhenKeyExists_return_valid()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorMessage_WhenKeyExists_return_validValue()
    {
        string appSettingKey = "ComplexKey:ComplexKey1";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        MlResult<string> expected = "ComplexValue1";

        result.ToString().Should().Be(expected.ToString());
    }

    [Fact]
    public void ReadAppSettingKey_complexKey_withErrorMessage_WhenKeyNotExists_return_fail()
    {
        string appSettingKey = "NotExists:ComplexKey";
        string errorMessage = "Error personalizado";

        MlResult<string> result = _sut.ReadAppSettingKey<string>(appSettingKey, errorMessage);

        result.IsFail.Should().BeTrue();
    }
}

