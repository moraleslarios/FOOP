// Tests de la epica 08 (US-34) de la version 2.3.0: familia de sobrecargas con plantilla de
// mensaje, que delega en el registrador sin interpolar para que los marcadores viajen como campos.

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogExtensionsTemplateTests
{

    #region Fuentes de datos

    private const string Plantilla = "hola a las {Hora}";

    private static readonly LogLevel[] AllLevels =
    [
        LogLevel.Trace,
        LogLevel.Debug,
        LogLevel.Information,
        LogLevel.Warning,
        LogLevel.Error,
        LogLevel.Critical
    ];

    public static IEnumerable<object[]> Niveles() => AllLevels.Select(x => new object[] { x });

    private static MlResult<string> Valido()  => "valor-ok".ToMlResultValid();
    private static MlResult<string> Fallido() => "fallo-controlado".ToMlResultFail<string>();

    private static IEnumerable<MethodInfo> FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    #endregion


    #region El test que demuestra la epica: los marcadores viajan como campos

    /// <summary>
    /// Es el test que demuestra la epica: la plantilla no se interpola, y el state que recibe el
    /// registrador expone el marcador {Hora} como un campo consultable con su valor original.
    /// </summary>
    [Fact]
    public void LogMlResultInformationTemplate_PublicaElMarcadorComoCampoEstructurado()
    {
        var spy  = new StructuredLoggerSpy();
        var hora = new DateTime(2024, 5, 17, 9, 30, 0);

        spy.LogMlResultInformationTemplate(Plantilla, hora);

        spy.Last.Fields                            .Should().NotBeNull();
        spy.Last.Fields!                           .Should().BeAssignableTo<IReadOnlyList<KeyValuePair<string, object?>>>();
        spy.Last.HasField("Hora")                  .Should().BeTrue("el marcador {Hora} debe publicarse como campo");
        spy.Last.Field("Hora")                     .Should().Be(hora, "el campo debe conservar el valor original, no su texto");
    }

    /// <summary>
    /// Comprueba que la plantilla llega al registrador SIN sustituir, publicada bajo la clave
    /// {OriginalFormat}, que es lo que permite agrupar trazas por plantilla en el destino.
    /// </summary>
    [Fact]
    public void LogMlResultInformationTemplate_EntregaLaPlantillaSinSustituirEnOriginalFormat()
    {
        var spy = new StructuredLoggerSpy();

        spy.LogMlResultInformationTemplate(Plantilla, new DateTime(2024, 5, 17, 9, 30, 0));

        spy.Last.OriginalFormat.Should().Be(Plantilla, "la plantilla no debe interpolarse antes de llegar al registrador");
        spy.Last.Message       .Should().Contain("hola a las ");
        spy.Last.Message       .Should().NotContain("{Hora}", "el formateador del registrador si debe sustituir el marcador");
    }

    /// <summary>
    /// Comprueba que los marcadores se rellenan POR POSICION, no por nombre, que es el matiz que
    /// mas confunde al consumidor.
    /// </summary>
    [Fact]
    public void LogMlResultInformationTemplate_RellenaLosMarcadoresPorPosicion()
    {
        var spy = new StructuredLoggerSpy();

        spy.LogMlResultInformationTemplate("{Primero} y {Segundo}", "uno", "dos");

        spy.Last.FieldNames  .Should().ContainInOrder("Primero", "Segundo");
        spy.Last.Field("Primero").Should().Be("uno");
        spy.Last.Field("Segundo").Should().Be("dos");
    }

    #endregion


    #region Los seis niveles y el nucleo neutral

    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_RegistraConSuPropioNivel(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        Valido().LogMlResultTemplate(spy, level, Plantilla, DateTime.Now);

        spy.Last.LogLevel           .Should().Be(level);
        spy.Last.HasField("Hora")   .Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLaFamiliaTemplateCompleta(LogLevel level)
    {
        var nombres = FirmasPublicas()
                            .Select(x => x.Name)
                            .Where(x => x.StartsWith($"LogMlResult{level}Template", StringComparison.Ordinal))
                            .ToArray();

        nombres.Should().Contain($"LogMlResult{level}Template");
        nombres.Should().Contain($"LogMlResult{level}TemplateAsync");
    }

    [Fact]
    public void LosSeisNiveles_ExponenElMismoNumeroDeFirmasConPlantilla()
    {
        var recuentos = AllLevels.ToDictionary(
                            level => level,
                            level => FirmasPublicas().Count(x => x.Name.StartsWith($"LogMlResult{level}Template", StringComparison.Ordinal)));

        recuentos.Values.Should().OnlyContain(x => x > 0);

        recuentos.Values.Distinct().Should().HaveCount(
            1,
            "la familia con plantilla debe ser simetrica, y el recuento obtenido fue: {0}",
            string.Join(", ", recuentos.Select(x => $"{x.Key}={x.Value}")));
    }

    #endregion


    #region Politicas: registrador nulo, plantilla nula y cortocircuito por IsEnabled

    /// <summary>
    /// Politica D-06: un registrador nulo devuelve el resultado de origen intacto y no lanza.
    /// </summary>
    [Fact]
    public void LogMlResultTemplate_ConRegistradorNulo_DevuelveElOrigenSinLanzar()
    {
        var origen = Valido();

        var devuelto = origen.LogMlResultInformationTemplate(null!, Plantilla, DateTime.Now);

        devuelto.IsSuccess.Should().BeTrue();
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResultTemplate_ConPlantillaNula_NoRegistraNadaYDevuelveElOrigen()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = Valido().LogMlResultInformationTemplate(spy, null!, DateTime.Now);

        spy.Entries.Should().BeEmpty();
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");
    }

    [Fact]
    public void LogMlResultTemplate_ConElNivelApagado_NoRegistraNada()
    {
        var spy = new StructuredLoggerSpy { MinimumLevel = LogLevel.Error };

        Valido().LogMlResultInformationTemplate(spy, Plantilla, DateTime.Now);

        spy.Entries           .Should().BeEmpty();
        spy.IsEnabledCallCount.Should().Be(1, "debe consultarse IsEnabled una sola vez y cortocircuitar");
    }

    [Fact]
    public void LogMlResultTemplate_ConArgumentosNulos_NoLanza()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = Valido().LogMlResultInformationTemplate(spy, "sin marcadores", null!);

        spy.Last.OriginalFormat.Should().Be("sin marcadores");
        devuelto.IsSuccess     .Should().BeTrue();
    }

    [Fact]
    public void LogMlResultTemplate_ConResultadoFallido_RegistraYDevuelveElFalloIntacto()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = Fallido().LogMlResultErrorTemplate(spy, Plantilla, DateTime.Now);

        spy.Last.LogLevel.Should().Be(LogLevel.Error);
        devuelto.IsFailure.Should().BeTrue();
        devuelto.TryGetErrors(out var errores).Should().BeTrue();
        errores.Errors.Should().ContainSingle(x => x.Message == "fallo-controlado");
    }

    #endregion


    #region Variantes asincronas

    [Fact]
    public async Task LogMlResultTemplateAsync_SobreMlResult_RegistraYDevuelveElOrigen()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = await Valido().LogMlResultInformationTemplateAsync(spy, Plantilla, DateTime.Now);

        spy.Last.HasField("Hora").Should().BeTrue();
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");
    }

    [Fact]
    public async Task LogMlResultTemplateAsync_SobreTarea_RegistraYDevuelveElOrigen()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = await Task.FromResult(Valido()).LogMlResultWarningTemplateAsync(spy, Plantilla, DateTime.Now);

        spy.Last.LogLevel.Should().Be(LogLevel.Warning);
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");
    }

    [Fact]
    public async Task LogMlResultTemplateAsync_DelRegistrador_DevuelveElRegistrador()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = await spy.LogMlResultCriticalTemplateAsync(Plantilla, DateTime.Now);

        spy.Last.LogLevel.Should().Be(LogLevel.Critical);
        devuelto.IsSuccess.Should().BeTrue();
    }

    #endregion


    #region La familia nueva NO captura las llamadas existentes (punto 30 del prompt)

    /// <summary>
    /// Las cinco formas de llamada de la tabla del punto 30 del prompt de migracion, escritas SIN
    /// nombres de parametro. Al llevar la familia nueva el sufijo Template (decision D-26), ninguna
    /// de ellas puede ser capturada por las sobrecargas nuevas: todas siguen resolviendo a las
    /// antiguas. Si una resolucion cambiase, este metodo dejaria de compilar o de pasar.
    /// </summary>
    [Fact]
    public void LasCincoFormasDeLlamadaExistentes_SiguenResolviendoALasSobrecargasAntiguas()
    {
        var spy = new LoggerSpy();

        // 1) ILogger + mensaje literal
        spy.LogMlResultInformation("mensaje-1");
        spy.Last.Message.Should().Be("mensaje-1");

        // 2) MlResult + logger + un unico mensaje literal
        Valido().LogMlResultInformation(spy, "mensaje-2");
        spy.Last.Message.Should().Be("mensaje-2");

        // 3) MlResult + logger + los dos mensajes literales
        Valido().LogMlResultInformation(spy, "valido-3", "fallido-3");
        spy.Last.Message.Should().Be("valido-3");

        // 4) MlResult + logger + el par de constructores de mensaje
        Valido().LogMlResultInformation(spy, (string x) => $"valido-4 {x}", (MlErrorDetails e) => e.GetErrorsSummary());
        spy.Last.Message.Should().Be("valido-4 valor-ok");

        // 5) ILogger + nivel + mensaje literal
        spy.LogMlResult(LogLevel.Information, "mensaje-5");
        spy.Last.Message.Should().Be("mensaje-5");
    }

    /// <summary>
    /// Contraejemplo explicito del riesgo que evita la decision D-26: la sobrecarga de los dos
    /// mensajes literales sigue tratando el segundo string como el mensaje del caso fallido, y NO
    /// como un argumento de plantilla.
    /// </summary>
    [Fact]
    public void LaSobrecargaDeDosMensajes_NoSeConfundeConUnaPlantillaMasArgumento()
    {
        var spy = new LoggerSpy();

        Fallido().LogMlResultInformation(spy, "valido-literal", "fallido-literal");

        spy.Last.Message.Should().Be("fallido-literal");
    }

    #endregion

}
