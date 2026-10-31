// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas del hallazgo <c>F1</c>, ya corregido por la historia US-11 de la epica 03.
/// </summary>
/// <remarks>
/// <para>
/// <b>Defecto F1 (historico).</b> En <c>MlResultLogExtensions.cs</c> la sobrecarga
/// <c>LogMlResultIfFailWithExceptionAsync&lt;T&gt;(this Task&lt;MlResult&lt;T&gt;&gt;, ILogger,
/// LogLevel, Func&lt;MlErrorDetails, Exception, string&gt;)</c> delegaba por error en
/// <c>LogMlResultIfFailWithValueAsync</c> en lugar de en
/// <c>LogMlResultIfFailWithExceptionAsync</c>. La consecuencia observable era que la condicion
/// evaluada era la presencia del metadato <c>Value</c> y no la del metadato <c>Ex</c>, de forma que
/// un resultado fallido que llevaba excepcion pero no valor NO registraba traza, y un resultado
/// fallido cuyo metadato <c>Value</c> era una excepcion SI la registraba.
/// </para>
/// <para>
/// Estas pruebas nacieron en la epica 01 (US-03) fijando el comportamiento defectuoso, con nombres
/// que lo denunciaban. La epica 03 (US-11) ha corregido el defecto y, por tanto, ha invertido aqui
/// las expectativas: es la prueba documental de la ruptura silenciosa que introduce la version
/// 2.2.0.
/// </para>
/// </remarks>
public class MlResultLogExtensionsF1Tests
{
    /// <summary>Mensaje de referencia que construye el delegado bajo prueba.</summary>
    private const string Prefijo = "f1";

    /// <summary>Resultado fallido que lleva una excepcion en los metadatos, pero ningun valor.</summary>
    private static MlResult<string> FallidoSoloConExcepcion()
        => MlErrorDetails.FromErrorMessageWithException("fallo-con-excepcion", new InvalidOperationException("kaboom"));

    /// <summary>Resultado fallido que lleva un valor en los metadatos, pero ninguna excepcion.</summary>
    private static MlResult<string> FallidoSoloConValor()
        => MlErrorDetails.FromErrorMessageWithValue<string>("fallo-con-valor", "valor-perdido");

    /// <summary>
    /// Resultado fallido cuyo metadato <c>Value</c> contiene una excepcion. Era el unico caso en el
    /// que la sobrecarga afectada por <c>F1</c> llegaba a registrar traza, porque al delegar en
    /// <c>LogMlResultIfFailWithValueAsync</c> con un <c>Func&lt;MlErrorDetails, Exception, string&gt;</c>
    /// resolvia a la sobrecarga <c>&lt;T, TValue&gt;</c> con <c>TValue = Exception</c> y exigia, por
    /// tanto, un metadato <c>Value</c> de tipo <see cref="Exception"/>.
    /// </summary>
    private static MlResult<string> FallidoConValorDeTipoExcepcion()
        => MlErrorDetails.FromErrorMessageWithValue<Exception>("fallo-con-valor-excepcion", new InvalidOperationException("boom"));


    /// <summary>
    /// Comprueba la correccion de <c>F1</c> (US-11): la sobrecarga asincrona de
    /// <c>IfFailWithException</c> que recibe una tarea y un constructor de mensaje con excepcion
    /// evalua el metadato <c>Ex</c>, por lo que con un fallo que lleva excepcion SI registra traza.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithExceptionAsync_SobreTarea_RegistraLaExcepcion_US11()
    {
        var spy = new LoggerSpy();

        await Task.FromResult(FallidoSoloConExcepcion())
                  .LogMlResultIfFailWithExceptionAsync(
                        spy,
                        LogLevel.Error,
                        (errorDetails, exception) => $"{Prefijo}:{exception.Message}");

        spy.Entries.Should().HaveCount(
            1,
            "US-11: la sobrecarga ya delega en LogMlResultIfFailWithExceptionAsync, asi que exige el "
          + "metadato Ex. Antes de la correccion (defecto F1) exigia el metadato Value y no "
          + "registraba nada.");

        spy.Last.LogLevel.Should().Be(LogLevel.Error);
        spy.Last.Message.Should().Be($"{Prefijo}:kaboom");
    }

    /// <summary>
    /// Contrapartida de la correccion de <c>F1</c> (US-11): un fallo cuyo metadato <c>Value</c> es
    /// una excepcion pero que no tiene metadato <c>Ex</c> ya NO registra traza, porque la condicion
    /// evaluada es la correcta.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithExceptionAsync_SobreTarea_ConFalloSinExcepcionPeroConValorExcepcion_NoRegistra_US11()
    {
        var spy = new LoggerSpy();

        await Task.FromResult(FallidoConValorDeTipoExcepcion())
                  .LogMlResultIfFailWithExceptionAsync(
                        spy,
                        LogLevel.Error,
                        (errorDetails, exception) => $"{Prefijo}:{exception.Message}");

        spy.Entries.Should().BeEmpty(
            "US-11: la sobrecarga ya no mira el metadato Value. Antes de la correccion (defecto F1) "
          + "bastaba con que existiera un metadato Value de tipo Exception para registrar traza.");
    }

    /// <summary>
    /// Prueba de contraste: tras la correccion de <c>F1</c> (US-11) las sobrecargas de
    /// <c>IfFailWithException</c> y de <c>IfFailWithValue</c> ya NO se comportan igual ante el mismo
    /// resultado. La divergencia es la evidencia de que el defecto esta corregido.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithValueAsync_SobreTarea_YaNoSeComportaIgualQueIfFailWithExceptionAsync_US11()
    {
        var spyException = new LoggerSpy();
        var spyValue     = new LoggerSpy();

        await Task.FromResult(FallidoConValorDeTipoExcepcion())
                  .LogMlResultIfFailWithExceptionAsync(
                        spyException,
                        LogLevel.Warning,
                        (errorDetails, exception) => $"{Prefijo}:excepcion");

        await Task.FromResult(FallidoConValorDeTipoExcepcion())
                  .LogMlResultIfFailWithValueAsync<string, Exception>(
                        spyValue,
                        LogLevel.Warning,
                        (errorDetails, valor) => $"{Prefijo}:valor");

        spyException.Entries.Should().BeEmpty(
            "US-11: IfFailWithException evalua el metadato Ex, que este resultado no tiene");

        spyValue.Entries.Should().HaveCount(
            1,
            "IfFailWithValue evalua el metadato Value, que este resultado si tiene");

        spyValue.Last.Message.Should().Be($"{Prefijo}:valor");
    }

    /// <summary>
    /// Comprueba que la sobrecarga sincrona equivalente, que nunca padecio el defecto, distingue
    /// correctamente entre un fallo con excepcion y un fallo sin ella. Era la linea de referencia
    /// del comportamiento que la US-11 ha trasladado a la version asincrona.
    /// </summary>
    [Fact]
    public void IfFailWithException_Sincrono_DistingueCorrectamenteLaExcepcion_ComportamientoDeReferencia()
    {
        var spyConExcepcion = new LoggerSpy();

        FallidoSoloConExcepcion()
            .LogMlResultIfFailWithException(
                spyConExcepcion,
                LogLevel.Error,
                (errorDetails, exception) => $"{Prefijo}:{exception.Message}");

        spyConExcepcion.Entries.Should().HaveCount(1);
        spyConExcepcion.Last.Message.Should().Be($"{Prefijo}:kaboom");

        var spySinExcepcion = new LoggerSpy();

        FallidoSoloConValor()
            .LogMlResultIfFailWithException(
                spySinExcepcion,
                LogLevel.Error,
                (errorDetails, exception) => $"{Prefijo}:no-deberia-registrarse");

        spySinExcepcion.Entries.Should().BeEmpty(
            "la version sincrona si exige el metadato Ex, que es el comportamiento correcto");
    }
}
