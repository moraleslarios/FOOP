

namespace MoralesLarios.OOFP.Unit.Tests.Helpers;
public class EnsureFpTests
{




    [Fact]
    public void NotNull_nullParameter_return_Fail()
    {
        int? data = null;

        var result = EnsureFp.NotNull(data, "data");

        result.IsValid.Should().BeFalse();
    }


    [Fact]
    public async Task NotNullAsync_nullParameter_return_Fail()
    {
        int? data = null;

        var result = await EnsureFp.NotNullAsync(data, "data");

        result.IsValid.Should().BeFalse();
    }


}
