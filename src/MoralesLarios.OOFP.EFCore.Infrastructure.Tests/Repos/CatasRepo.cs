namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Repos;

public class CatasRepo(JfCatasDbContext dbContext) : EFRepoFp<Cata, JfCatasDbContext>(dbContext), ICatasRepo
{
}