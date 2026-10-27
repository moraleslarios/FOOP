using MoralesLarios.OOFP.Application.Services;

namespace MoralesLarios.OOFP.Http.Tests.Integration.Clients;

public interface IPruebaDuplexClient : IRestClientFp<PruebaRequestDto, PruebaResponseDto>
{
}