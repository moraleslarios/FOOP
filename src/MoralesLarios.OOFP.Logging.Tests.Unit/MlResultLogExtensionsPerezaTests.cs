// Tests de la epica 06 (US-28 y US-31) de la version 2.2.0: evaluacion perezosa del
// constructor de mensaje y coste del registro con el nivel apagado.

using System.Diagnostics;
using Xunit.Abstractions;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogExtensionsPerezaTests
{

    private readonly ITestOutputHelper salida;

    public MlResultLogExtensionsPerezaTests(ITestOutputHelper salida) => this.salida = salida;

    #region Fuentes de datos

    private const string Mensaje = "mensaje-perezoso";

    private static MlResult<string> Valido()  => "valor-ok".ToMlResultValid();

    private static MlResult<string> Fallido() => "fallo-controlado".ToMlResultFail<string>();

    #endregion


    #region US-28 · el constructor de mensaje no se invoca con el nivel apagado

    [Fact]
    public void LogMlResult_NoInvocaElConstructorValido_CuandoElNivelEstaDeshabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.Error };
        var invocaciones = 0;

        var resultado = Valido().LogMlResult(spy,
                                             LogLevel.Information,
                                             validBuildMessage: _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(0, "con el nivel apagado no debe construirse el mensaje");
        spy.Entries.Should().BeEmpty();
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResult_NoInvocaElConstructorFallido_CuandoElNivelEstaDeshabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.Critical };
        var invocaciones = 0;

        var resultado = Fallido().LogMlResult(spy,
                                              LogLevel.Warning,
                                              failBuildMessage: _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(0, "con el nivel apagado no debe construirse el mensaje");
        spy.Entries.Should().BeEmpty();
        resultado.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void LogMlResult_InvocaElConstructorValidoUnaSolaVez_CuandoElNivelEstaHabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.Trace };
        var invocaciones = 0;

        var resultado = Valido().LogMlResult(spy,
                                             LogLevel.Information,
                                             validBuildMessage: _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(1, "el mensaje debe construirse exactamente una vez");
        spy.Entries.Should().HaveCount(1);
        spy.Last.Message.Should().Be(Mensaje);
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResult_InvocaElConstructorFallidoUnaSolaVez_CuandoElNivelEstaHabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.Trace };
        var invocaciones = 0;

        var resultado = Fallido().LogMlResult(spy,
                                              LogLevel.Error,
                                              failBuildMessage: _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(1, "el mensaje debe construirse exactamente una vez");
        spy.Entries.Should().HaveCount(1);
        spy.Last.Message.Should().Be(Mensaje);
        resultado.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void LogMlResult_ConsultaIsEnabled_AntesDeConstruirElMensaje()
    {
        var spy = new LoggerSpy { MinimumLevel = LogLevel.Error };
        spy.Clear();

        Valido().LogMlResult(spy, LogLevel.Debug, validBuildMessage: _ => Mensaje);

        spy.IsEnabledCallCount.Should().BeGreaterThan(0, "la guarda debe preguntar al registrador");
    }

    [Fact]
    public void LogMlResult_DevuelveElMismoResultado_ConElNivelEncendidoOApagado()
    {
        var encendido = new LoggerSpy { MinimumLevel = LogLevel.Trace };
        var apagado   = new LoggerSpy { MinimumLevel = LogLevel.None  };

        var conRegistro = Valido().LogMlResult(encendido, LogLevel.Information, validBuildMessage: _ => Mensaje);
        var sinRegistro = Valido().LogMlResult(apagado  , LogLevel.Information, validBuildMessage: _ => Mensaje);

        conRegistro.Match(valid: x => x, fail: _ => "KO")
                   .Should().Be(sinRegistro.Match(valid: x => x, fail: _ => "KO"));
    }

    [Fact]
    public void LogMlResultIfFail_NoInvocaElConstructor_CuandoElNivelEstaDeshabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.None };
        var invocaciones = 0;

        var resultado = Fallido().LogMlResultIfFail(spy,
                                                    LogLevel.Error,
                                                    _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(0);
        spy.Entries.Should().BeEmpty();
        resultado.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void LogMlResultIfValid_NoInvocaElConstructor_CuandoElNivelEstaDeshabilitado()
    {
        var spy          = new LoggerSpy { MinimumLevel = LogLevel.None };
        var invocaciones = 0;

        var resultado = Valido().LogMlResultIfValid(spy,
                                                    LogLevel.Information,
                                                    _ => { invocaciones++; return Mensaje; });

        invocaciones.Should().Be(0);
        spy.Entries.Should().BeEmpty();
        resultado.Match(valid: x => x, fail: _ => "KO").Should().Be("valor-ok");
    }

    #endregion


    #region US-31 · coste del registro con el nivel apagado

    [Fact]
    public void ElRegistroConElNivelApagado_CuestaMuchoMenosQueConElNivelEncendido()
    {
        const int iteraciones = 20_000;

        var apagado   = new LoggerSpy { MinimumLevel = LogLevel.None  };
        var encendido = new LoggerSpy { MinimumLevel = LogLevel.Trace };

        // Calentamiento, para que el JIT no contamine la medicion.
        Medir(apagado  , 1_000);
        Medir(encendido, 1_000);
        encendido.Clear();

        var conNivelApagado   = Medir(apagado  , iteraciones);
        var conNivelEncendido = Medir(encendido, iteraciones);

        apagado.Entries.Should().BeEmpty("con el nivel apagado no debe registrarse nada");
        encendido.Entries.Should().HaveCount(iteraciones);

        salida.WriteLine($"MEDICION US-31 | iteraciones = {iteraciones}");
        salida.WriteLine($"MEDICION US-31 | nivel apagado   : {conNivelApagado.bytesPorLlamada:F1} bytes/llamada, {conNivelApagado.milisegundos} ms");
        salida.WriteLine($"MEDICION US-31 | nivel encendido : {conNivelEncendido.bytesPorLlamada:F1} bytes/llamada, {conNivelEncendido.milisegundos} ms");

        conNivelApagado.bytesPorLlamada.Should().BeLessThan(
            conNivelEncendido.bytesPorLlamada / 2,
            "la evaluacion perezosa debe ahorrar al menos la mitad de las asignaciones; apagado = {0:F1} bytes en {1} ms, encendido = {2:F1} bytes en {3} ms",
            conNivelApagado.bytesPorLlamada,
            conNivelApagado.milisegundos,
            conNivelEncendido.bytesPorLlamada,
            conNivelEncendido.milisegundos);
    }

    private static (double bytesPorLlamada, long milisegundos) Medir(LoggerSpy spy, int iteraciones)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var bytesAntes = GC.GetAllocatedBytesForCurrentThread();
        var reloj      = Stopwatch.StartNew();

        for (var i = 0; i < iteraciones; i++)
            Valido().LogMlResult(spy, LogLevel.Information, validBuildMessage: x => $"valor {x} con relleno {Guid.NewGuid()}");

        reloj.Stop();

        var bytes = (GC.GetAllocatedBytesForCurrentThread() - bytesAntes) / (double)iteraciones;

        return (bytes, reloj.ElapsedMilliseconds);
    }

    #endregion

}
