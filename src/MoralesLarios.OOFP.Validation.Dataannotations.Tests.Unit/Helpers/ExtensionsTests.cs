using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit.Helpers;
public class ExtensionsTests
{


    [Fact]
    public void ValidateWithDataannotations_objectValid_return_valid()
    {
        User source = new("user", DateTime.Now, "password", "password");

        MlResult<User> result = source.ValidateWithDataannotations();

        result.IsValid.Should().BeTrue();
    }



    [Fact]
    public void ValidateWithDataannotations_objectFail_return_fail()
    {
        User source = new("", DateTime.Now, "", "password");

        MlResult<User> result = source.ValidateWithDataannotations();

        result.IsFail.Should().BeTrue();
    }


    [Fact]
    public void ValidateWithDataannotationsAsync_colec_AllValid_return_valid()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataannotations();

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataannotationsAsync_colec_AllFail_return_Fail()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password_diferewt2")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataannotations();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataannotationsAsync_colec_AllFail_return_2_errors()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password_diferewt2")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataannotations();

        var resultErrors = result.SecureFailErrorsDetails().Errors.Count();

        resultErrors.Should().Be(2);
    }

    [Fact]
    public void ValidateWithDataannotationsAsync_colec_fail_1_element_return_Fail()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataannotations();

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void ValidateWithDataannotationsAsync_colec_fail_1_element_return_1_errors()
    {
        IEnumerable<User> source = new List<User>
        {
            new("user", DateTime.Now, "password", "password_diferewt1"),
            new("user2", DateTime.Now, "password", "password")
        };

        MlResult<IEnumerable<User>> result = source.ValidateWithDataannotations();

        var resultErrors = result.SecureFailErrorsDetails().Errors.Count();

        resultErrors.Should().Be(1);
    }
}
