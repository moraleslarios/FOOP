using Microsoft.EntityFrameworkCore;

namespace MoralesLarios.OOFP.EFCore.OopRepos;

internal class EFRepoBase : IDisposable, IGetContextable
{
    protected internal DbContext internalDbContext { get; set; }


    public EFRepoBase(DbContext dbContext)
    {
        internalDbContext = dbContext;
    }






    public virtual void Dispose()
    {
        internalDbContext?.Dispose();
    }

    public DbContext GetContext() => internalDbContext;
}
