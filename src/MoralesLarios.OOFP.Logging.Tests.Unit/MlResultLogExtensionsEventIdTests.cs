// Tests de la epica 06 (US-29) de la version 2.2.0: sobrecargas de registro que aceptan un
// EventId, para poder filtrar y correlacionar trazas sin depender del texto del mensaje.

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogExtensionsEventIdTests
{

    #region Fuentes de datos

    private static readonly EventId Evento = new(4242, "EventoDePrueba");

    private static MlResult<string> Valido()  => "valor-ok".ToMlResultValid();
    private static MlResult<string> Fallido() => "fallo-controlado".ToMlResultFail<string>();

    private static IEnumerable<MethodInfo> FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    #endregion


    #region El EventId llega al ILogger

    [Fact]
    public void LogMlResult_TrasladaElEventId_AlRegistradorEnElCasoValido()
    {
        var spy = new LoggerSpy();

        Valido().LogMlResult(spy, LogLevel.Information, Evento, validBuildMessage: x => $"valido {x}");

        spy.Last.EventId.Should().Be(Evento);
        spy.Last.EventId.Id.Should().Be(4242);
        spy.Last.EventId.Name.Should().Be("EventoDePrueba");
    }

    [Fact]
    public void LogMlResult_TrasladaElEventId_AlRegistradorEnElCasoFallido()
    {
        var spy = new LoggerSpy();

        Fallido().LogMlResult(spy, LogLevel.Error, Evento, failBuildMessage: errors => errors.GetErrorsSummary());

        spy.Last.EventId.Should().Be(Evento);
        spy.Last.LogLevel.Should().Be(LogLevel.Error);
    }

    [Fact]
    public void LogMlResult_DelRegistrador_TrasladaElEventIdYElMensaje()
    {
        var spy = new LoggerSpy();

        spy.LogMlResult(LogLevel.Warning, Evento, "mensaje-literal");

        spy.Last.EventId .Should().Be(Evento);
        spy.Last.Message .Should().Be("mensaje-literal");
        spy.Last.LogLevel.Should().Be(LogLevel.Warning);
    }

    [Fact]
    public async Task LogMlResultAsync_DelRegistrador_TrasladaElEventIdYElMensaje()
    {
        var spy = new LoggerSpy();

        await spy.LogMlResultAsync(LogLevel.Debug, Evento, "mensaje-literal-asincrono");

        spy.Last.EventId.Should().Be(Evento);
        spy.Last.Message.Should().Be("mensaje-literal-asincrono");
    }

    [Fact]
    public async Task LogMlResultAsync_DeUnaTarea_TrasladaElEventId()
    {
        var spy = new LoggerSpy();

        var resultado = await Task.FromResult(Valido())
                                  .LogMlResultAsync(spy, LogLevel.Information, Evento, validBuildMessage: x => $"valido {x}");

        spy.Last.EventId.Should().Be(Evento);
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    #endregion


    #region Las seis familias de nivel trasladan el EventId

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void CadaFamiliaDeNivel_TrasladaElEventIdConSuPropioNivel(LogLevel nivel)
    {
        var spy = new LoggerSpy();

        var _ = nivel switch
        {
            LogLevel.Trace       => Valido().LogMlResultTrace      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Debug       => Valido().LogMlResultDebug      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Information => Valido().LogMlResultInformation(spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Warning     => Valido().LogMlResultWarning    (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Error       => Valido().LogMlResultError      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            _                    => Valido().LogMlResultCritical   (spy, Evento, validBuildMessage: x => $"valido {x}")
        };

        spy.Last.EventId .Should().Be(Evento);
        spy.Last.LogLevel.Should().Be(nivel);
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void CadaFamiliaDeNivel_DelRegistrador_TrasladaElEventIdConSuPropioNivel(LogLevel nivel)
    {
        var spy = new LoggerSpy();

        var _ = nivel switch
        {
            LogLevel.Trace       => spy.LogMlResultTrace      (Evento, "literal"),
            LogLevel.Debug       => spy.LogMlResultDebug      (Evento, "literal"),
            LogLevel.Information => spy.LogMlResultInformation(Evento, "literal"),
            LogLevel.Warning     => spy.LogMlResultWarning    (Evento, "literal"),
            LogLevel.Error       => spy.LogMlResultError      (Evento, "literal"),
            _                    => spy.LogMlResultCritical   (Evento, "literal")
        };

        spy.Last.EventId .Should().Be(Evento);
        spy.Last.LogLevel.Should().Be(nivel);
        spy.Last.Message .Should().Be("literal");
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public async Task CadaFamiliaDeNivel_EnSuVersionAsincrona_TrasladaElEventId(LogLevel nivel)
    {
        var spy  = new LoggerSpy();
        var task = Task.FromResult(Valido());

        var _ = nivel switch
        {
            LogLevel.Trace       => await task.LogMlResultTraceAsync      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Debug       => await task.LogMlResultDebugAsync      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Information => await task.LogMlResultInformationAsync(spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Warning     => await task.LogMlResultWarningAsync    (spy, Evento, validBuildMessage: x => $"valido {x}"),
            LogLevel.Error       => await task.LogMlResultErrorAsync      (spy, Evento, validBuildMessage: x => $"valido {x}"),
            _                    => await task.LogMlResultCriticalAsync   (spy, Evento, validBuildMessage: x => $"valido {x}")
        };

        spy.Last.EventId .Should().Be(Evento);
        spy.Last.LogLevel.Should().Be(nivel);
    }

    #endregion


    #region Las sobrecargas con EventId respetan el resto del contrato

    [Fact]
    public void LogMlResult_ConEventId_NoInvocaElConstructor_CuandoElNivelEstaDeshabilitado()
    {
        var spy      = new LoggerSpy { MinimumLevel = LogLevel.None };
        var llamadas = 0;

        Valido().LogMlResult(spy, LogLevel.Information, Evento, validBuildMessage: x => { llamadas++; return x; });

        llamadas        .Should().Be(0, "con el nivel apagado el constructor de mensaje no debe invocarse");
        spy.Entries     .Should().BeEmpty();
    }

    [Fact]
    public void LogMlResult_ConEventId_DevuelveElResultadoDeOrigenSinAlterar()
    {
        var spy = new LoggerSpy();

        var resultado = Valido().LogMlResult(spy, LogLevel.Information, Evento, validBuildMessage: x => $"valido {x}");

        resultado.IsSuccess.Should().BeTrue();
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResult_ConEventId_NoRegistraNada_CuandoNoSeIndicaConstructor()
    {
        var spy = new LoggerSpy();

        Valido() .LogMlResult(spy, LogLevel.Information, Evento);
        Fallido().LogMlResult(spy, LogLevel.Information, Evento);

        spy.Entries.Should().BeEmpty();
    }

    [Fact]
    public void LogMlResult_ConEventId_DevuelveFallo_CuandoElRegistradorEsNulo()
    {
        var resultado = ((ILogger)null!).LogMlResult(LogLevel.Information, Evento, "mensaje");

        resultado.IsFailure.Should().BeTrue();
    }

    #endregion


    #region Simetria y convenios de las nuevas firmas

    [Fact]
    public void LasSobrecargasConEventId_ExistenParaLosSeisNiveles()
    {
        var niveles = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" };

        var recuentos = niveles.ToDictionary(nivel => nivel,
                                             nivel => FirmasPublicas().Count(m => m.Name.Contains($"LogMlResult{nivel}")
                                                                               && m.GetParameters().Any(p => p.ParameterType == typeof(EventId))));

        recuentos.Values.Should().AllSatisfy(n => n.Should().BeGreaterThan(0));
        recuentos.Values.Distinct().Should().HaveCount(1, "las seis familias de nivel deben ofrecer las mismas sobrecargas con EventId");
    }

    [Fact]
    public void LasSobrecargasConEventId_NoUsanParametrosOpcionalesParaElEventId()
    {
        var opcionales = FirmasPublicas().SelectMany(m => m.GetParameters())
                                         .Where(p => p.ParameterType == typeof(EventId) && p.IsOptional)
                                         .ToList();

        opcionales.Should().BeEmpty("un EventId opcional crearia ambiguedad con las firmas anteriores");
    }

    [Fact]
    public void LasSobrecargasConEventId_ColocanElEventIdAntesDeLosConstructoresDeMensaje()
    {
        var firmas = FirmasPublicas().Where(m => m.GetParameters().Any(p => p.ParameterType == typeof(EventId))).ToList();

        firmas.Should().NotBeEmpty();

        foreach (var firma in firmas)
        {
            var parametros = firma.GetParameters();
            var posicionEvento = Array.FindIndex(parametros, p => p.ParameterType == typeof(EventId));
            var posicionFunc   = Array.FindIndex(parametros, p => p.ParameterType.Name.StartsWith("Func"));

            if (posicionFunc >= 0)
                posicionEvento.Should().BeLessThan(posicionFunc, $"en {firma.Name} el EventId debe preceder a los constructores de mensaje");
        }
    }

    #endregion

}
