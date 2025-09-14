using MoralesLarios.OOFP.Types;
using MoralesLarios.OOFP.Validation.FluentValidations.Helpers;

namespace MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit;
public class ExtensionsTests
{


    [Fact]
    public void ValidateWithDataannotations_objectValid_return_valid()
    {
        User source = new("user", DateTime.Now, "password", "password");

        MlResult<User> result = source.ValidateWitHFluentValidations<User, UserValidator>();

        result.IsValid.Should().BeTrue();
    }



    [Fact]
    public void ValidateWithDataannotations_objectFail_return_fail()
    {
        User source = new("", DateTime.Now, "", "password");

        MlResult<User> result = source.ValidateWitHFluentValidations<User, UserValidator>();

        result.IsFail.Should().BeTrue();
    }








}
