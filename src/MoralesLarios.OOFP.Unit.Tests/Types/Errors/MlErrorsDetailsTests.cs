using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Unit.Tests.Types.Errors;
public class MlErrorsDetailsTests
{





    [Fact]
    public void ToDescription_OK()
    {
        MlErrorsDetails expected = (new List<MlError> { "miError", "miError2", "miError3", "miError4", "miError5" }, new Dictionary<string, object>
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

        var result = expected.ToErrorsDetailsDescription();
    }



}
