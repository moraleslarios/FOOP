namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Repos;

public class VinosCatasRepo(JfCatasDbContext dbContext) : EFRepoFp<VinosCata, JfCatasDbContext>(dbContext), IVinosCatasRepo
{
}