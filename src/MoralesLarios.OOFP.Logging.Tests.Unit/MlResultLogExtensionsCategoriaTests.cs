// Tests de la epica 06 (US-30) de la version 2.2.0: sobrecargas de registro que aceptan un
// registrador tipado por categoria, para usar directamente el que inyecta el contenedor.

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogExtensionsCategoriaTests
{

    #region Fuentes de datos

    /// <summary>
    /// Registrador tipado por categoria que delega en un <see cref="LoggerSpy"/> para poder inspeccionar las trazas.
    /// </summary>
    private sealed class LoggerSpyDeCategoria<TCategory> : ILogger<TCategory>
    {
        public LoggerSpy Interno { get; } = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => Interno.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => Interno.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Interno.Log(logLevel, eventId, state, exception, formatter);
    }

    private sealed class CategoriaDePrueba { }

    private static MlResult<string> Valido()  => "valor-ok".ToMlResultValid();
    private static MlResult<string> Fallido() => "fallo-controlado".ToMlResultFail<string>();

    private static IEnumerable<MethodInfo> FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    #endregion


    #region El registrador tipado recibe las trazas

    [Fact]
    public void LogMlResult_ConRegistradorTipado_RegistraElCasoValido()
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();

        Valido().LogMlResult(logger, LogLevel.Information, validBuildMessage: x => $"valido {x}");

        logger.Interno.Last.Message .Should().Be("valido valor-ok");
        logger.Interno.Last.LogLevel.Should().Be(LogLevel.Information);
    }

    [Fact]
    public void LogMlResult_ConRegistradorTipado_RegistraElCasoFallido()
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();

        Fallido().LogMlResult(logger, LogLevel.Error, failBuildMessage: errors => errors.GetErrorsSummary());

        logger.Interno.Entries  .Should().HaveCount(1);
        logger.Interno.Last.Message.Should().Contain("fallo-controlado");
    }

    [Fact]
    public void LogMlResult_ConRegistradorTipado_DevuelveElResultadoDeOrigenSinAlterar()
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();

        var resultado = Valido().LogMlResult(logger, LogLevel.Information, validBuildMessage: x => $"valido {x}");

        resultado.IsSuccess.Should().BeTrue();
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResult_ConRegistradorTipado_NoInvocaElConstructor_CuandoElNivelEstaDeshabilitado()
    {
        var logger   = new LoggerSpyDeCategoria<CategoriaDePrueba>();
        var llamadas = 0;

        logger.Interno.MinimumLevel = LogLevel.None;

        Valido().LogMlResult(logger, LogLevel.Information, validBuildMessage: x => { llamadas++; return x; });

        llamadas              .Should().Be(0);
        logger.Interno.Entries.Should().BeEmpty();
    }

    [Fact]
    public async Task LogMlResultAsync_ConRegistradorTipado_RegistraDesdeUnaTarea()
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();

        var resultado = await Task.FromResult(Valido())
                                  .LogMlResultAsync(logger, LogLevel.Debug, validBuildMessage: x => $"valido {x}");

        logger.Interno.Last.Message .Should().Be("valido valor-ok");
        logger.Interno.Last.LogLevel.Should().Be(LogLevel.Debug);
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    #endregion


    #region Las seis familias de nivel aceptan el registrador tipado

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void CadaFamiliaDeNivel_AceptaElRegistradorTipadoYUsaSuPropioNivel(LogLevel nivel)
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();

        var _ = nivel switch
        {
            LogLevel.Trace       => Valido().LogMlResultTrace      (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Debug       => Valido().LogMlResultDebug      (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Information => Valido().LogMlResultInformation(logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Warning     => Valido().LogMlResultWarning    (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Error       => Valido().LogMlResultError      (logger, validBuildMessage: x => $"valido {x}"),
            _                    => Valido().LogMlResultCritical   (logger, validBuildMessage: x => $"valido {x}")
        };

        logger.Interno.Last.LogLevel.Should().Be(nivel);
        logger.Interno.Last.Message .Should().Be("valido valor-ok");
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public async Task CadaFamiliaDeNivel_EnSuVersionAsincrona_AceptaElRegistradorTipado(LogLevel nivel)
    {
        var logger = new LoggerSpyDeCategoria<CategoriaDePrueba>();
        var task   = Task.FromResult(Valido());

        var _ = nivel switch
        {
            LogLevel.Trace       => await task.LogMlResultTraceAsync      (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Debug       => await task.LogMlResultDebugAsync      (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Information => await task.LogMlResultInformationAsync(logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Warning     => await task.LogMlResultWarningAsync    (logger, validBuildMessage: x => $"valido {x}"),
            LogLevel.Error       => await task.LogMlResultErrorAsync      (logger, validBuildMessage: x => $"valido {x}"),
            _                    => await task.LogMlResultCriticalAsync   (logger, validBuildMessage: x => $"valido {x}")
        };

        logger.Interno.Last.LogLevel.Should().Be(nivel);
    }

    #endregion


    #region Convivencia con las sobrecargas anteriores

    [Fact]
    public void ElRegistradorSinCategoria_SigueResolviendoLasSobrecargasAnteriores()
    {
        ILogger logger = new LoggerSpy();

        var resultado = Valido().LogMlResultInformation(logger, validBuildMessage: x => $"valido {x}");

        ((LoggerSpy)logger).Last.Message.Should().Be("valido valor-ok");
        resultado.IsSuccess             .Should().BeTrue();
    }

    [Fact]
    public void ElMismoRegistrador_ProduceLaMismaTraza_PorLaViaTipadaYPorLaViaSinCategoria()
    {
        var tipado      = new LoggerSpyDeCategoria<CategoriaDePrueba>();
        var sinCategoria = new LoggerSpy();

        Valido().LogMlResultInformation(tipado                 , validBuildMessage: x => $"valido {x}");
        Valido().LogMlResultInformation((ILogger)sinCategoria   , validBuildMessage: x => $"valido {x}");

        tipado.Interno.Last.Message.Should().Be(sinCategoria.Last.Message);
        tipado.Interno.Last.LogLevel.Should().Be(sinCategoria.Last.LogLevel);
    }

    [Fact]
    public void LasSobrecargasConCategoria_ExistenParaLasSieteFamilias()
    {
        var familias = new[] { "LogMlResult", "LogMlResultTrace", "LogMlResultDebug", "LogMlResultInformation",
                               "LogMlResultWarning", "LogMlResultError", "LogMlResultCritical" };

        var conCategoria = FirmasPublicas().Where(m => m.GetParameters()
                                                        .Any(p => p.ParameterType.IsGenericType
                                                               && p.ParameterType.GetGenericTypeDefinition() == typeof(ILogger<>)))
                                           .ToList();

        conCategoria.Should().NotBeEmpty();

        foreach (var familia in familias)
            conCategoria.Any(m => m.Name == familia || m.Name == $"{familia}Async")
                        .Should().BeTrue($"la familia {familia} debe ofrecer sobrecargas con registrador tipado");
    }

    #endregion

}
