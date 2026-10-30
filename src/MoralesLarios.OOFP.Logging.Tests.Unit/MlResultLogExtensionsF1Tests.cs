// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas que fijan el comportamiento actual del hallazgo <c>F1</c> antes de corregirlo, de modo
/// que la correccion de la epica 03 (historia US-11) se vea como un cambio de expectativa explicito
/// y no como una regresion silenciosa.
/// </summary>
/// <remarks>
/// <para>
/// <b>Defecto F1.</b> En <c>MlResultLogExtensions.cs</c> la sobrecarga
/// <c>LogMlResultIfFailWithExceptionAsync&lt;T&gt;(this Task&lt;MlResult&lt;T&gt;&gt;, ILogger,
/// LogLevel, Func&lt;MlErrorDetails, Exception, string&gt;)</c> delega por error en
/// <c>LogMlResultIfFailWithValueAsync</c> en lugar de en
/// <c>LogMlResultIfFailWithExceptionAsync</c>. La consecuencia observable es que la condicion que
/// se evalua es la presencia del metadato <c>Value</c> y no la del metadato <c>Ex</c>, de forma que
/// un resultado fallido que lleva excepcion pero no valor NO registra traza, y un resultado fallido
/// que lleva valor pero no excepcion SI la registra.
/// </para>
/// <para>
/// Decision D-08 de la epica 01, opcion (b): en lugar de marcar la prueba como omitida, se afirma
/// el comportamiento actual con un nombre que lo denuncia. Cuando la epica 03 corrija el defecto,
/// estas dos pruebas pasaran a rojo y habra que invertir sus expectativas, que es exactamente la
/// senal que se busca.
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
    /// Resultado fallido cuyo metadato <c>Value</c> contiene una excepcion. Es el unico caso en el
    /// que la sobrecarga afectada por <c>F1</c> llega a registrar traza, porque al delegar en
    /// <c>LogMlResultIfFailWithValueAsync</c> con un <c>Func&lt;MlErrorDetails, Exception, string&gt;</c>
    /// resuelve a la sobrecarga <c>&lt;T, TValue&gt;</c> con <c>TValue = Exception</c> y exige, por
    /// tanto, un metadato <c>Value</c> de tipo <see cref="Exception"/>.
    /// </summary>
    private static MlResult<string> FallidoConValorDeTipoExcepcion()
        => MlErrorDetails.FromErrorMessageWithValue<Exception>("fallo-con-valor-excepcion", new InvalidOperationException("boom"));


    /// <summary>
    /// Fija el defecto <c>F1</c>: la sobrecarga asincrona de <c>IfFailWithException</c> que recibe
    /// una tarea y un constructor de mensaje con excepcion evalua el metadato <c>Value</c>, por lo
    /// que con un fallo que solo lleva excepcion NO registra ninguna traza.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithExceptionAsync_SobreTarea_RegistraElValorEnLugarDeLaExcepcion_DefectoConocidoF1()
    {
        var spy = new LoggerSpy();

        await Task.FromResult(FallidoSoloConExcepcion())
                  .LogMlResultIfFailWithExceptionAsync(
                        spy,
                        LogLevel.Error,
                        (errorDetails, exception) => $"{Prefijo}:{exception.Message}");

        spy.Entries.Should().BeEmpty(
            "F1: la sobrecarga delega en LogMlResultIfFailWithValueAsync, asi que exige el metadato "
          + "Value y no el metadato Ex. Cuando la epica 03 (US-11) lo corrija, esta expectativa debe "
          + "pasar a esperar una traza con el mensaje de la excepcion.");
    }

    /// <summary>
    /// Contrapartida del defecto <c>F1</c>: la sobrecarga afectada solo registra cuando el metadato
    /// <c>Value</c> existe <b>y</b> es de tipo <see cref="Exception"/>, porque al delegar en
    /// <c>LogMlResultIfFailWithValueAsync</c> resuelve a la variante <c>&lt;T, TValue&gt;</c> con
    /// <c>TValue = Exception</c>. Es la prueba directa de que la condicion evaluada es la
    /// equivocada: se mira la clave <c>Value</c> en lugar de la clave <c>Ex</c>.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithExceptionAsync_SobreTarea_ConFalloSinExcepcionPeroConValorExcepcion_SiRegistra_DefectoConocidoF1()
    {
        var spy = new LoggerSpy();

        await Task.FromResult(FallidoConValorDeTipoExcepcion())
                  .LogMlResultIfFailWithExceptionAsync(
                        spy,
                        LogLevel.Error,
                        (errorDetails, exception) => $"{Prefijo}:{exception.Message}");

        spy.Entries.Should().HaveCount(
            1,
            "F1: al delegar en LogMlResultIfFailWithValueAsync basta con que exista el metadato Value "
          + "de tipo Exception, aunque no exista el metadato Ex. Cuando la epica 03 (US-11) lo "
          + "corrija, esta expectativa debe pasar a esperar que NO se registre nada.");

        spy.Last.LogLevel.Should().Be(LogLevel.Error);
        spy.Last.Message.Should().Be($"{Prefijo}:boom");
    }

    /// <summary>
    /// Prueba de contraste: la sobrecarga de <c>IfFailWithValue</c>, que es la que si delega donde
    /// debe, se comporta exactamente igual que la de <c>IfFailWithException</c> ante los mismos
    /// resultados. La coincidencia de comportamiento entre las dos es la evidencia del defecto.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithValueAsync_SobreTarea_SeComportaIgualQueIfFailWithExceptionAsync_EvidenciaDeF1()
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

        spyException.Entries.Should().HaveCount(spyValue.Entries.Count,
            "F1: ambas sobrecargas acaban ejecutando el mismo cuerpo, cuando deberian evaluar "
          + "metadatos distintos (Ex frente a Value).");

        spyException.Entries.Should().HaveCount(1);
    }

    /// <summary>
    /// Comprueba que la sobrecarga sincrona equivalente, que no padece el defecto, si distingue
    /// correctamente entre un fallo con excepcion y un fallo sin ella. Sirve de linea de referencia
    /// del comportamiento correcto que la epica 03 debe conseguir en la version asincrona.
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
