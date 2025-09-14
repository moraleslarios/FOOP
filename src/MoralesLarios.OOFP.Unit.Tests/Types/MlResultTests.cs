using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Unit.Tests.Types;

public class MlResultTests
{



    [Fact]
    public void ToString_IsValid_OK()
    {
        MlResult<int> data = 1;

        var result = data.ToString();

        var expected = "1";

        result.Should().Be(expected);
    }



    [Fact]
    public void ToString_IsFail_withErrors_withDetails_OK()
    {
        MlResult<int> data = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" }, new Dictionary<string, object>
                                         {
                                                        { "key1", "value1" },
                                                        { "key2", "value2" },
                                                        { "key3", "value3" },
                                                        { "key4", "value4" },
                                                        { "key5", "value5" },
                                                        { "key6", "value6" },
                                                        { "key7", "value7" },
                                                        { "key8", "value8" },
                                                        { "key9", "value9" },
                                                        { "key10", "value10" }
                                                    }
                                          );

        var result = data.ToString();

        var expected = @"""MlError:
     miError
     miError2
     miError3
     miError4
     miError5
Details:
     key1: value1
     key2: value2
     key3: value3
     key4: value4
     key5: value5
     key6: value6
     key7: value7
     key8: value8
     key9: value9
     key10: value10""";


        result.Should().NotBeEquivalentTo(expected);
    }



    [Fact]
    public void ToString_IsFail_withErrors_OK()
    {
        MlResult<int> data = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" });

        var result = data.ToString();

        var expected = @"""MlError:
     miError
     miError2
     miError3
     miError4
     miError5""";


        result.Should().NotBeEquivalentTo(expected);
    }



}
