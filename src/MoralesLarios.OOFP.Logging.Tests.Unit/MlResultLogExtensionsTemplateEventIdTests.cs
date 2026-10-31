// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Tests del cruce entre la plantilla de registro estructurado y el <see cref="EventId"/>, el alcance que
/// fijó la decisión <c>D-28</c> opción (b) de la épica 08 de la versión 2.3.0 (US-36).
/// </summary>
public class MlResultLogExtensionsTemplateEventIdTests
{

    #region Fuentes de datos

    private const string Plantilla = "el pedido {PedidoId} ha cambiado a {Estado}";

    private static readonly EventId Evento = new(1001, "PedidoCambiado");

    private static readonly LogLevel[] AllLevels =
    [
        LogLevel.Trace, LogLevel.Debug, LogLevel.Information,
        LogLevel.Warning, LogLevel.Error, LogLevel.Critical
    ];

    public static IEnumerable<object[]> Niveles() => AllLevels.Select(x => new object[] { x });

    private static MlResult<string> Valido() => "pedido-17".ToMlResultValid();

    private static MlResult<string> Fallido() => MlErrorDetails.FromErrorMessage("no se ha podido procesar")
                                                               .ToMlResultFail<string>();

    private static IEnumerable<MethodInfo> FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    #endregion


    #region El test que demuestra la historia

    /// <summary>
    /// La historia pedía no tener que elegir entre el evento y los campos: una sola llamada debe
    /// entregar el <see cref="EventId"/>, la plantilla intacta y cada marcador como campo propio.
    /// </summary>
    [Fact]
    public void LogMlResultInformationTemplate_ConEventId_EntregaElEventoLaPlantillaYLosCampos()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = Valido().LogMlResultInformationTemplate(spy, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue("la operación de registro no altera el resultado");

        spy.Entries.Should().HaveCount(1);
        spy.Last.LogLevel.Should().Be(LogLevel.Information);
        spy.Last.EventId.Should().Be(Evento);
        spy.Last.OriginalFormat.Should().Be(Plantilla, "la plantilla no se interpola");
        spy.Last.Field("PedidoId").Should().Be(17);
        spy.Last.Field("Estado").Should().Be("Enviado");
    }

    #endregion


    #region El eje EventId en la familia generica

    [Theory]
    [MemberData(nameof(Niveles))]
    public void LogMlResultTemplate_SobreElRegistrador_AceptaElEventIdEnTodosLosNiveles(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        var resultado = spy.LogMlResultTemplate(level, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(level);
        spy.Last.EventId.Should().Be(Evento);
        spy.Last.Field("PedidoId").Should().Be(17);
    }

    [Theory]
    [MemberData(nameof(Niveles))]
    public void LogMlResultTemplate_SobreElResultado_AceptaElEventIdEnTodosLosNiveles(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        var resultado = Valido().LogMlResultTemplate(spy, level, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(level);
        spy.Last.EventId.Should().Be(Evento);
    }

    [Fact]
    public void LogMlResultTemplate_ConEventId_SeEmiteTambienCuandoElResultadoEsFallido()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = Fallido().LogMlResultTemplate(spy, LogLevel.Error, Evento, Plantilla, 17, "Rechazado");

        resultado.IsFailure.Should().BeTrue("el resultado de origen se devuelve intacto");
        spy.Entries.Should().HaveCount(1, "la familia incondicional registra siempre");
        spy.Last.EventId.Should().Be(Evento);
    }

    #endregion


    #region El eje EventId en los seis niveles

    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLaPlantillaConEventIdSobreElRegistrador(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        OperacionDeNivelSobreRegistrador(level)(spy);

        spy.Last.LogLevel.Should().Be(level);
        spy.Last.EventId.Should().Be(Evento);
        spy.Last.OriginalFormat.Should().Be(Plantilla);
    }

    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLaPlantillaConEventIdSobreElResultado(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        var resultado = OperacionDeNivelSobreResultado(level)(Valido(), spy);

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(level);
        spy.Last.EventId.Should().Be(Evento);
    }

    #endregion


    #region Politicas de nulos, pereza y niveles

    [Fact]
    public void LogMlResultTemplate_ConEventId_NoRegistraNadaSiLaPlantillaEsNula()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = Valido().LogMlResultTemplate(spy, LogLevel.Information, Evento, null!, 17);

        resultado.IsSuccess.Should().BeTrue();
        spy.Entries.Should().BeEmpty("una plantilla nula no genera traza");
    }

    [Fact]
    public void LogMlResultTemplate_ConEventId_NoLanzaSiElRegistradorEsNulo()
    {
        var operacion = () => Valido().LogMlResultTemplate(null!, LogLevel.Information, Evento, Plantilla, 17);

        operacion.Should().NotThrow("un registrador nulo se tolera, igual que en el resto de la superficie");
    }

    [Fact]
    public void LogMlResultTemplate_ConEventId_NoRegistraNadaSiElNivelEstaApagado()
    {
        var spy = new StructuredLoggerSpy { MinimumLevel = LogLevel.Error };

        Valido().LogMlResultTemplate(spy, LogLevel.Debug, Evento, Plantilla, 17, "Enviado");

        spy.Entries.Should().BeEmpty();
        spy.IsEnabledCallCount.Should().BeGreaterThan(0, "la pereza se consulta antes de construir nada");
    }

    [Fact]
    public void LogMlResultTemplate_ConEventId_AdmiteUnaListaDeArgumentosVacia()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = spy.LogMlResultTemplate(LogLevel.Information, Evento, "sin marcadores");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.OriginalFormat.Should().Be("sin marcadores");
        spy.Last.EventId.Should().Be(Evento);
    }

    #endregion


    #region Variantes asincronas

    [Fact]
    public async Task LogMlResultTemplateAsync_ConEventId_SobreUnResultadoSincrono()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = await Valido().LogMlResultTemplateAsync(spy, LogLevel.Warning, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(LogLevel.Warning);
        spy.Last.EventId.Should().Be(Evento);
    }

    [Fact]
    public async Task LogMlResultTemplateAsync_ConEventId_SobreUnaTarea()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = await Valido().ToAsync().LogMlResultTemplateAsync(spy, LogLevel.Error, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(LogLevel.Error);
        spy.Last.EventId.Should().Be(Evento);
    }

    [Fact]
    public async Task LogMlResultCriticalTemplateAsync_ConEventId_SobreUnaTarea()
    {
        var spy = new StructuredLoggerSpy();

        var resultado = await Valido().ToAsync().LogMlResultCriticalTemplateAsync(spy, Evento, Plantilla, 17, "Enviado");

        resultado.IsSuccess.Should().BeTrue();
        spy.Last.LogLevel.Should().Be(LogLevel.Critical);
        spy.Last.EventId.Should().Be(Evento);
    }

    #endregion


    #region Resolucion de sobrecargas sin nombres de parametro (criterio de la US-36)

    /// <summary>
    /// Criterio de aceptación de la US-36 · las combinaciones nuevas no deben generar ambigüedad. Este test
    /// no comprueba nada en ejecución que no comprueben los anteriores: su valor es que <b>compila</b>,
    /// invocando una llamada de cada forma nueva <b>sin nombres de parámetro</b>.
    /// </summary>
    /// <remarks>
    /// El <see cref="EventId"/> desambigua por sí solo frente a las sobrecargas sin evento, porque el
    /// parámetro siguiente sigue siendo la plantilla <c>string</c>. Lo que <b>sí</b> es ambiguo es pasar un
    /// literal <c>null</c> donde se espera el <c>params object?[]</c>: hay que escribirlo
    /// <c>(object?[])null!</c>, igual que documenta el punto 54 del prompt de migración.
    /// </remarks>
    [Fact]
    public void LasCombinacionesNuevas_SeResuelvenSinNombresDeParametro()
    {
        var spy = new StructuredLoggerSpy();

        spy.LogMlResultTemplate(LogLevel.Information, Evento, Plantilla, 17, "Enviado");
        spy.LogMlResultInformationTemplate(Evento, Plantilla, 17, "Enviado");

        Valido().LogMlResultTemplate(spy, LogLevel.Information, Evento, Plantilla, 17, "Enviado");
        Valido().LogMlResultInformationTemplate(spy, Evento, Plantilla, 17, "Enviado");

        spy.Entries.Should().HaveCount(4, "las cuatro formas sincronas han resuelto y registrado");
    }

    /// <summary>
    /// Criterio de aceptación de la US-36 · con un <see cref="ILogger{TCategory}"/> las llamadas deben
    /// resolver <b>sin casting</b>, aunque el cruce con la categoría quede fuera de alcance por <c>D-28</c>.
    /// </summary>
    [Fact]
    public void LasCombinacionesNuevas_SeResuelvenConUnILoggerDeCategoriaSinCasting()
    {
        var spy = new StructuredLoggerSpy();
        ILogger<MlResultLogExtensionsTemplateEventIdTests> logger = new LoggerDeCategoria(spy);

        logger.LogMlResultInformationTemplate(Evento, Plantilla, 17, "Enviado");
        Valido().LogMlResultInformationTemplate(logger, Evento, Plantilla, 17, "Enviado");

        spy.Entries.Should().HaveCount(2, "ILogger<T> es un ILogger, no hace falta castear");
        spy.Last.EventId.Should().Be(Evento);
    }

    #endregion


    #region Simetria del fichero de cruce

    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLasCincoFirmasDelCruceConEventId(LogLevel level)
    {
        var nombres = FirmasPublicas().Where(x => x.Name == $"LogMlResult{level}Template"
                                              || x.Name == $"LogMlResult{level}TemplateAsync")
                                      .Count(x => x.GetParameters().Any(p => p.ParameterType == typeof(EventId)));

        nombres.Should().Be(5, $"el nivel {level} debe exponer las cinco firmas del cruce plantilla x EventId");
    }

    [Fact]
    public void LaFamiliaGenerica_ExponeLasCincoFirmasDelCruceConEventId()
    {
        var firmas = FirmasPublicas().Where(x => x.Name is "LogMlResultTemplate" or "LogMlResultTemplateAsync")
                                     .Count(x => x.GetParameters().Any(p => p.ParameterType == typeof(EventId)));

        firmas.Should().Be(5, "la familia generica expone las cinco firmas del cruce plantilla x EventId");
    }

    /// <summary>
    /// <c>D-28</c> opción (b) acotó el cruce a la familia incondicional: las familias de la rama de fallo
    /// <b>no</b> reciben el eje <see cref="EventId"/>. Este test fija ese límite para que nadie añada
    /// firmas «por simetría».
    /// </summary>
    [Fact]
    public void LasFamiliasCondicionales_NoRecibenElEjeEventId()
    {
        var firmas = FirmasPublicas().Where(x => x.Name.Contains("IfFail", StringComparison.Ordinal)
                                              && x.Name.Contains("Template", StringComparison.Ordinal))
                                     .Where(x => x.GetParameters().Any(p => p.ParameterType == typeof(EventId)));

        firmas.Should().BeEmpty("D-28 (b) deja las familias condicionales fuera del cruce con EventId");
    }

    #endregion


    #region Utilidades

    private static Action<StructuredLoggerSpy> OperacionDeNivelSobreRegistrador(LogLevel level)
        => level switch
        {
            LogLevel.Trace       => x => x.LogMlResultTraceTemplate      (Evento, Plantilla, 17, "Enviado"),
            LogLevel.Debug       => x => x.LogMlResultDebugTemplate      (Evento, Plantilla, 17, "Enviado"),
            LogLevel.Information => x => x.LogMlResultInformationTemplate(Evento, Plantilla, 17, "Enviado"),
            LogLevel.Warning     => x => x.LogMlResultWarningTemplate    (Evento, Plantilla, 17, "Enviado"),
            LogLevel.Error       => x => x.LogMlResultErrorTemplate      (Evento, Plantilla, 17, "Enviado"),
            LogLevel.Critical    => x => x.LogMlResultCriticalTemplate   (Evento, Plantilla, 17, "Enviado"),
            _                    => throw new ArgumentOutOfRangeException(nameof(level))
        };

    private static Func<MlResult<string>, StructuredLoggerSpy, MlResult<string>> OperacionDeNivelSobreResultado(LogLevel level)
        => level switch
        {
            LogLevel.Trace       => (s, x) => s.LogMlResultTraceTemplate      (x, Evento, Plantilla, 17, "Enviado"),
            LogLevel.Debug       => (s, x) => s.LogMlResultDebugTemplate      (x, Evento, Plantilla, 17, "Enviado"),
            LogLevel.Information => (s, x) => s.LogMlResultInformationTemplate(x, Evento, Plantilla, 17, "Enviado"),
            LogLevel.Warning     => (s, x) => s.LogMlResultWarningTemplate    (x, Evento, Plantilla, 17, "Enviado"),
            LogLevel.Error       => (s, x) => s.LogMlResultErrorTemplate      (x, Evento, Plantilla, 17, "Enviado"),
            LogLevel.Critical    => (s, x) => s.LogMlResultCriticalTemplate   (x, Evento, Plantilla, 17, "Enviado"),
            _                    => throw new ArgumentOutOfRangeException(nameof(level))
        };

    /// <summary>
    /// Envoltorio mínimo que presenta el <see cref="StructuredLoggerSpy"/> como un
    /// <see cref="ILogger{TCategoryName}"/>, para poder probar la resolución de sobrecargas sin casting.
    /// </summary>
    private sealed class LoggerDeCategoria(ILogger interno) : ILogger<MlResultLogExtensionsTemplateEventIdTests>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => interno.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) => interno.IsEnabled(logLevel);

        public void Log<TState>(LogLevel                         logLevel,
                                EventId                          eventId,
                                TState                           state,
                                Exception?                       exception,
                                Func<TState, Exception?, string> formatter)
            => interno.Log(logLevel, eventId, state, exception, formatter);
    }

    #endregion

}
