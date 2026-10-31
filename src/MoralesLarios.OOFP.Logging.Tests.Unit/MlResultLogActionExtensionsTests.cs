// Tests de la epica 04: nomenclatura y firmas publicas.
// Cubren MlResultLogActionExtensions (nombre nuevo), los puentes [Obsolete] de la clase Extensions
// y el idioma ingles de los mensajes que construye la libreria.

using MoralesLarios.OOFP.Logging;
using MoralesLarios.OOFP.Logging.Tests.Unit.Support;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogActionExtensionsTests
{

    private const string Accion = "saving the invoice";


    [Fact]
    public void LogActionResult_con_resultado_valido_registra_el_mensaje_de_exito_en_ingles()
    {
        var spy   = new LoggerSpy();
        var valor = 42.ToMlResultValid();

        var resultado = valor.LogActionResult(spy, Accion);

        resultado.IsSuccess.Should().BeTrue();
        spy.Entries.Should().HaveCount(1);
        spy.Last!.Message.Should().Be($"{Accion} done correctly.");
    }


    [Fact]
    public void LogActionResult_con_resultado_fallido_registra_el_mensaje_de_error_en_ingles()
    {
        var spy   = new LoggerSpy();
        var valor = "no se pudo guardar".ToMlResultFail<int>();

        var resultado = valor.LogActionResult(spy, Accion);

        resultado.IsFailure.Should().BeTrue();
        spy.Entries.Should().HaveCount(1);
        spy.Last!.Message.Should().StartWith($"Error when {Accion}: ");
        spy.Last!.Message.Should().Contain("no se pudo guardar");
    }


    [Fact]
    public async Task LogActionResultAsync_con_resultado_valido_registra_el_mensaje_de_exito_en_ingles()
    {
        var spy   = new LoggerSpy();
        var valor = Task.FromResult(42.ToMlResultValid());

        var resultado = await valor.LogActionResultAsync(spy, Accion);

        resultado.IsSuccess.Should().BeTrue();
        spy.Entries.Should().HaveCount(1);
        spy.Last!.Message.Should().Be($"{Accion} done correctly.");
    }


    [Fact]
    public async Task LogActionResultAsync_con_resultado_fallido_registra_el_mensaje_de_error_en_ingles()
    {
        var spy   = new LoggerSpy();
        var valor = Task.FromResult("fallo asincrono".ToMlResultFail<int>());

        var resultado = await valor.LogActionResultAsync(spy, Accion);

        resultado.IsFailure.Should().BeTrue();
        spy.Entries.Should().HaveCount(1);
        spy.Last!.Message.Should().StartWith($"Error when {Accion}: ");
    }


    [Fact]
    public void El_prefijo_de_error_no_duplica_la_palabra_Error()
    {
        var spy   = new LoggerSpy();
        var valor = "detalle".ToMlResultFail<int>();

        valor.LogActionResult(spy, Accion);

        var prefijo     = spy.Last!.Message.Substring(0, spy.Last!.Message.IndexOf(':') + 1);
        var ocurrencias = prefijo.Split("Error").Length - 1;

        prefijo.Should().Be($"Error when {Accion}:");
        ocurrencias.Should().Be(1);
    }


#pragma warning disable CS0618 // se comprueba a proposito el puente obsoleto
    [Fact]
    public void El_puente_obsoleto_MyMethodFinalLog_delega_en_LogActionResult()
    {
        var spyPuente = new LoggerSpy();
        var spyNuevo  = new LoggerSpy();

        Extensions.MyMethodFinalLog(42.ToMlResultValid(), spyPuente, Accion);
        42.ToMlResultValid().LogActionResult(spyNuevo, Accion);

        spyPuente.Messages.Should().BeEquivalentTo(spyNuevo.Messages);
    }


    [Fact]
    public async Task El_puente_obsoleto_MyMethodFinalLogAsync_delega_en_LogActionResultAsync()
    {
        var spyPuente = new LoggerSpy();
        var spyNuevo  = new LoggerSpy();

        await Extensions.MyMethodFinalLogAsync(Task.FromResult(42.ToMlResultValid()), spyPuente, Accion);
        await Task.FromResult(42.ToMlResultValid()).LogActionResultAsync(spyNuevo, Accion);

        spyPuente.Messages.Should().BeEquivalentTo(spyNuevo.Messages);
    }
#pragma warning restore CS0618

}
