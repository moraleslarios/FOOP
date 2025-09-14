namespace MoralesLarios.OOFP.EFCore.Infrastructure.Tests.Repos;

public class VinosCatasPuntuacionesRepo(JfCatasDbContext dbContext) : EFRepoFp<VinosCatasPuntuacion, JfCatasDbContext>(dbContext), IVinosCatasPuntuacionesRepo
{
}