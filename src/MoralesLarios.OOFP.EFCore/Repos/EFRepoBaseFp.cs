namespace MoralesLarios.OOFP.EFCore.Repos;
public class EFRepoBaseFp(DbContext dbContext) : IGetContextable , IDisposable
{
    public DbContext GetContext() => dbContext;


    public virtual void Dispose()
    {
        GetContext()?.Dispose();
    }


}
