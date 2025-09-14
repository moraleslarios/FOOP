using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Validation;
public abstract class MlValidableFp<T>
    where T : class
{


    public abstract MlResult<T> Validate();





}
