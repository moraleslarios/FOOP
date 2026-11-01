// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Xunit.Abstractions;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Medición 3 del apéndice de la épica 9 de la versión 2.4.0, aplazada expresamente a la US-45:
/// cuánto cuesta abrir un ámbito de registro cuando el nivel está <b>apagado</b> y nadie escucha.
/// </summary>
/// <remarks>
/// <para>
/// Replica el arnés que la épica 6 montó para la US-31 —<c>Stopwatch</c> más
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/>, con 20.000 iteraciones y dos vueltas de
/// calentamiento— para que las dos mediciones sean comparables. La línea base de la épica 6 es de
/// <b>240,0 bytes</b> por llamada con el nivel apagado.
/// </para>
/// <para>
/// La pregunta que decide es si <c>WithLogScope</c> debería abrir el ámbito <b>de forma
/// condicional</b>, consultando antes al registrador. La respuesta está en el tercer test: un ámbito
/// <b>no tiene nivel</b>, de modo que no hay ningún <c>IsEnabled</c> al que preguntar.
/// </para>
/// </remarks>
public class MlResultLogScopeExtensionsAsignacionesTests
{

    private readonly ITestOutputHelper salida;

    public MlResultLogScopeExtensionsAsignacionesTests(ITestOutputHelper salida) => this.salida = salida;

    #region Fuentes de datos

    private const int Iteraciones   = 20_000;
    private const int Calentamiento =  1_000;

    private static MlResult<string> Valido() => "valor-ok".ToMlResultValid();

    private static MlResult<string> SinAmbito(Func<MlResult<string>, MlResult<string>> tramo)
        => tramo(Valido());

    private static MlResult<string> ConPlantilla(ILogger logger, Func<MlResult<string>, MlResult<string>> tramo)
        => Valido().WithLogScope(logger, "pedido {PedidoId}", [77], tramo);

    private static MlResult<string> ConDiccionario(ILogger logger, Func<MlResult<string>, MlResult<string>> tramo)
        => Valido().WithLogScope(logger, new Dictionary<string, object?> { ["PedidoId"] = 77 }, tramo);

    private static (double bytesPorLlamada, long milisegundos) Medir(Action accion, int iteraciones)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var bytesAntes = GC.GetAllocatedBytesForCurrentThread();
        var reloj      = Stopwatch.StartNew();

        for (var i = 0; i < iteraciones; i++) accion();

        reloj.Stop();

        var bytes = (GC.GetAllocatedBytesForCurrentThread() - bytesAntes) / (double)iteraciones;

        return (bytes, reloj.ElapsedMilliseconds);
    }

    #endregion


    #region Medición 3 del apéndice · coste del ámbito con el nivel apagado

    /// <summary>
    /// Mide el sobrecoste en asignaciones que añade el ámbito sobre el mismo tramo sin envolver,
    /// con el nivel apagado y con un destino que no materializa ámbitos.
    /// </summary>
    [Fact]
    public void Medicion3_ElAmbitoConElNivelApagado_CuestaUnosPocosBytesSobreElTramoSuelto()
    {
        var spy = new LoggerSpy { MinimumLevel = LogLevel.None };

        Func<MlResult<string>, MlResult<string>> tramo = r => r.LogMlResultInformationTemplate(spy, "paso {PedidoId}", 77);

        Action sinAmbito      = () => SinAmbito(tramo);
        Action conPlantilla   = () => ConPlantilla(spy, tramo);
        Action conDiccionario = () => ConDiccionario(spy, tramo);

        // Calentamiento, para que el JIT no contamine la medicion.
        Medir(sinAmbito     , Calentamiento);
        Medir(conPlantilla  , Calentamiento);
        Medir(conDiccionario, Calentamiento);

        var suelto      = Medir(sinAmbito     , Iteraciones);
        var plantilla   = Medir(conPlantilla  , Iteraciones);
        var diccionario = Medir(conDiccionario, Iteraciones);

        var sobrecostePlantilla   = plantilla  .bytesPorLlamada - suelto.bytesPorLlamada;
        var sobrecosteDiccionario = diccionario.bytesPorLlamada - suelto.bytesPorLlamada;

        spy.Entries.Should().BeEmpty("con el nivel apagado no debe registrarse nada");

        salida.WriteLine($"MEDICION 3 | iteraciones = {Iteraciones}");
        salida.WriteLine($"MEDICION 3 | tramo suelto          : {suelto     .bytesPorLlamada:F1} bytes/llamada, {suelto     .milisegundos} ms");
        salida.WriteLine($"MEDICION 3 | ambito con plantilla  : {plantilla  .bytesPorLlamada:F1} bytes/llamada, {plantilla  .milisegundos} ms");
        salida.WriteLine($"MEDICION 3 | ambito con diccionario: {diccionario.bytesPorLlamada:F1} bytes/llamada, {diccionario.milisegundos} ms");
        salida.WriteLine($"MEDICION 3 | sobrecoste plantilla   = {sobrecostePlantilla:F1} bytes/llamada");
        salida.WriteLine($"MEDICION 3 | sobrecoste diccionario = {sobrecosteDiccionario:F1} bytes/llamada");

        suelto.bytesPorLlamada.Should().BeLessThan(
            320,
            "el tramo suelto debe seguir pegado a la linea base de 240,0 bytes de la epica 6; medido = {0:F1} bytes",
            suelto.bytesPorLlamada);

        sobrecostePlantilla.Should().BeLessThan(
            160,
            "el ambito con plantilla solo debe anadir el array de argumentos y el boxeo de sus valores; medido = {0:F1} bytes sobre {1:F1} bytes del tramo suelto",
            sobrecostePlantilla,
            suelto.bytesPorLlamada);

        sobrecosteDiccionario.Should().BeLessThan(
            400,
            "el ambito con diccionario solo debe anadir el propio diccionario; medido = {0:F1} bytes sobre {1:F1} bytes del tramo suelto",
            sobrecosteDiccionario,
            suelto.bytesPorLlamada);
    }

    /// <summary>
    /// Comprueba que, con un destino que materializa los ámbitos de verdad, el coste añadido es del
    /// destino y no de la librería, y que en 2.000 vueltas no se queda ningún ámbito abierto.
    /// </summary>
    [Fact]
    public void Medicion3_ConUnDestinoQueImplementaAmbitosDeVerdad_ElCosteEsDelDestinoYNoHayFugas()
    {
        const int iteraciones = 2_000;

        var spy = new StructuredLoggerSpy { MinimumLevel = LogLevel.None };

        Func<MlResult<string>, MlResult<string>> tramo = r => r.LogMlResultInformationTemplate(spy, "paso {PedidoId}", 77);

        Action conPlantilla = () => ConPlantilla(spy, tramo);

        var conAmbito = Medir(conPlantilla, iteraciones);

        salida.WriteLine($"MEDICION 3 | destino con ambitos reales: {conAmbito.bytesPorLlamada:F1} bytes/llamada, {conAmbito.milisegundos} ms, {iteraciones} iteraciones");

        spy.Entries         .Should().BeEmpty("con el nivel apagado no debe registrarse nada");
        spy.Scopes          .Should().HaveCount(iteraciones, "cada vuelta abre su propio ambito");
        spy.LeakedScopes    .Should().BeEmpty("el finally de RunInScope cierra todos los ambitos");
        spy.ActiveScopeCount.Should().Be(0, "la pila de ambitos debe quedar vacia");
    }

    /// <summary>
    /// Deja constancia de por qué el ámbito <b>no</b> se abre de forma condicional: no existe ningún
    /// <c>IsEnabled</c> al que preguntar, porque un ámbito no tiene nivel.
    /// </summary>
    [Fact]
    public void Medicion3_ElAmbitoNoConsultaIsEnabled_PorqueUnAmbitoNoTieneNivel()
    {
        var spy = new StructuredLoggerSpy { MinimumLevel = LogLevel.None };

        var resultado = Valido().WithLogScope(spy, "pedido {PedidoId}", [77], section: r => r);

        resultado.IsSuccess        .Should().BeTrue();
        spy.IsEnabledCallCount     .Should().Be(0, "un ambito no tiene nivel, asi que no hay nada que consultar");
        spy.Scopes                 .Should().HaveCount(1, "el ambito se abre aunque el nivel este apagado");
        spy.LastScope.IsClosed     .Should().BeTrue();
        spy.Entries                .Should().BeEmpty("el ambito es transparente · D-35 (a)");
    }

    #endregion

}
