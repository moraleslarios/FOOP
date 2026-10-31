// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de la familia <c>LogMlResult*IfFail*Template</c>, el registro estructurado de la rama de
/// fallo que introduce la <b>US-35</b> de la epica 08 de la version 2.3.0.
/// </summary>
/// <remarks>
/// <para>
/// Lo que se fija aqui es la decision <c>D-32</c>: en la rama de fallo los argumentos de la
/// plantilla se pueden aportar de dos maneras, y las dos conviven. O los da el llamante con
/// <c>params object?[] args</c>, o se derivan del <c>MlErrorDetails</c> con un
/// <c>Func&lt;MlErrorDetails, object?[]&gt;</c>.
/// </para>
/// <para>
/// Lo segundo que se fija es que los tres nombres de campo que
/// <see cref="MlResultLogErrorFields"/> deriva son <b>contrato</b>. Si alguien los renombra, estas
/// pruebas se rompen, que es exactamente lo que se busca: el compilador del consumidor no avisaria
/// de nada y seria una ruptura silenciosa.
/// </para>
/// <para>
/// Se usa <see cref="StructuredLoggerSpy"/>, y no <c>LoggerSpy</c>, porque el segundo aplana el
/// <c>state</c> a texto y perderia precisamente los campos que hay que comprobar.
/// </para>
/// </remarks>
public class MlResultLogExtensionsTemplateFailTests
{

    #region Fuentes de datos

    /// <summary>Plantilla de referencia con un solo marcador con nombre.</summary>
    private const string Plantilla = "El pedido {PedidoId} no se ha podido procesar";

    /// <summary>Niveles de severidad que la libreria expone como ficheros de nivel.</summary>
    private static readonly LogLevel[] AllLevels =
    [
        LogLevel.Trace, LogLevel.Debug, LogLevel.Information,
        LogLevel.Warning, LogLevel.Error, LogLevel.Critical
    ];

    /// <summary>Alimenta las pruebas parametrizadas por nivel.</summary>
    public static IEnumerable<object[]> Niveles() => AllLevels.Select(level => new object[] { level });

    /// <summary>Resultado valido de referencia.</summary>
    private static MlResult<string> Valido() => "ok".ToMlResultValid();

    /// <summary>Resultado fallido cuyo detalle de error no lleva ni valor ni excepcion.</summary>
    private static MlResult<string> Fallido()
        => MlErrorDetails.FromErrorMessage("fallo-pelado").ToMlResultFail<string>();

    /// <summary>Resultado fallido cuyo detalle de error lleva un valor en los metadatos.</summary>
    private static MlResult<string> FallidoConValor()
        => MlErrorDetails.FromErrorMessageWithValue<string>("fallo-con-valor", "valor-perdido");

    /// <summary>Resultado fallido cuyo detalle de error lleva una excepcion en los metadatos.</summary>
    private static MlResult<string> FallidoConExcepcion()
        => MlErrorDetails.FromErrorMessageWithException("fallo-con-excepcion", new InvalidOperationException("boom"));

    /// <summary>Firmas publicas y estaticas de la clase de extensiones.</summary>
    private static MethodInfo[] FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    #endregion


    #region El test que demuestra la historia

    /// <summary>
    /// Comprueba el caso de uso que motiva la historia: un fallo emite campos estructurados que un
    /// <i>sink</i> puede indexar, sin tener que analizar el texto del mensaje.
    /// </summary>
    [Fact]
    public void LaRamaDeFallo_EmiteCamposIndexablesSinAnalizarTexto()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = Fallido().LogMlResultErrorIfFailTemplate(spy,
                                                                MlResultLogErrorFields.DefaultTemplate,
                                                                MlResultLogErrorFields.From);

        spy.Entries.Should().HaveCount(1, "el resultado es fallido");
        spy.Last.LogLevel.Should().Be(LogLevel.Error);
        spy.Last.OriginalFormat.Should().Be(MlResultLogErrorFields.DefaultTemplate,
                                            "la plantilla se entrega sin interpolar");
        spy.Last.HasField(MlResultLogErrorFields.ErrorMessageField ).Should().BeTrue();
        spy.Last.HasField(MlResultLogErrorFields.ErrorCountField   ).Should().BeTrue();
        spy.Last.HasField(MlResultLogErrorFields.ExceptionTypeField).Should().BeTrue();
        devuelto.IsFailure.Should().BeTrue("el resultado de origen se devuelve intacto");
    }

    #endregion


    #region Contrato de los nombres y de los valores de los campos derivados

    /// <summary>
    /// Fija los tres nombres de campo canonicos. Si cambian, es una ruptura silenciosa para
    /// cualquier consulta que los indexe.
    /// </summary>
    [Fact]
    public void LosNombresDeCampoCanonicos_SonContrato()
    {
        MlResultLogErrorFields.ErrorMessageField .Should().Be("ErrorMessage");
        MlResultLogErrorFields.ErrorCountField   .Should().Be("ErrorCount");
        MlResultLogErrorFields.ExceptionTypeField.Should().Be("ExceptionType");
        MlResultLogErrorFields.NoExceptionValue  .Should().Be("(ninguna)");
    }

    /// <summary>
    /// Comprueba que la plantilla canonica consume los tres campos, y en el mismo orden en que
    /// <c>From</c> los produce, porque los marcadores se rellenan por posicion.
    /// </summary>
    [Fact]
    public void LaPlantillaCanonica_ConsumeLosTresCamposEnElOrdenDeFrom()
    {
        var plantilla = MlResultLogErrorFields.DefaultTemplate;

        plantilla.IndexOf("{" + MlResultLogErrorFields.ErrorMessageField  + "}").Should().BeGreaterThanOrEqualTo(0);
        plantilla.IndexOf("{" + MlResultLogErrorFields.ErrorCountField    + "}").Should().BeGreaterThan(
            plantilla.IndexOf("{" + MlResultLogErrorFields.ErrorMessageField + "}"));
        plantilla.IndexOf("{" + MlResultLogErrorFields.ExceptionTypeField + "}").Should().BeGreaterThan(
            plantilla.IndexOf("{" + MlResultLogErrorFields.ErrorCountField + "}"));

        MlResultLogErrorFields.From(MlErrorDetails.FromErrorMessage("x")).Should().HaveCount(3);
    }

    /// <summary>
    /// Comprueba que, cuando el detalle de error lleva excepcion, el campo
    /// <c>ExceptionType</c> publica el nombre del tipo de esa excepcion.
    /// </summary>
    [Fact]
    public void ConExcepcion_ElCampoExceptionType_PublicaElNombreDelTipo()
    {
        var spy = new StructuredLoggerSpy();

        FallidoConExcepcion().LogMlResultErrorIfFailTemplate(spy,
                                                             MlResultLogErrorFields.DefaultTemplate,
                                                             MlResultLogErrorFields.From);

        spy.Last.Field(MlResultLogErrorFields.ExceptionTypeField)
                .Should().Be(nameof(InvalidOperationException));
    }

    /// <summary>
    /// Comprueba que, cuando el detalle de error no lleva excepcion, el campo
    /// <c>ExceptionType</c> publica el valor explicito de ausencia y no un nulo.
    /// </summary>
    [Fact]
    public void SinExcepcion_ElCampoExceptionType_PublicaElValorDeAusencia()
    {
        var spy = new StructuredLoggerSpy();

        Fallido().LogMlResultErrorIfFailTemplate(spy,
                                                 MlResultLogErrorFields.DefaultTemplate,
                                                 MlResultLogErrorFields.From);

        spy.Last.Field(MlResultLogErrorFields.ExceptionTypeField)
                .Should().Be(MlResultLogErrorFields.NoExceptionValue);
    }

    /// <summary>
    /// Comprueba que el campo <c>ErrorCount</c> publica el numero de errores del detalle.
    /// </summary>
    [Fact]
    public void ElCampoErrorCount_PublicaElNumeroDeErrores()
    {
        var spy = new StructuredLoggerSpy();

        Fallido().LogMlResultErrorIfFailTemplate(spy,
                                                 MlResultLogErrorFields.DefaultTemplate,
                                                 MlResultLogErrorFields.From);

        spy.Last.Field(MlResultLogErrorFields.ErrorCountField).Should().Be(1);
    }

    /// <summary>
    /// Comprueba que los metodos de derivacion no lanzan ante un detalle nulo, porque registrar no
    /// debe poder tumbar al consumidor.
    /// </summary>
    [Fact]
    public void LaDerivacionDeCampos_NoLanzaConUnDetalleNulo()
    {
        MlResultLogErrorFields.ErrorMessageOf (null!).Should().BeEmpty();
        MlResultLogErrorFields.ErrorCountOf   (null!).Should().Be(0);
        MlResultLogErrorFields.ExceptionTypeOf(null!).Should().Be(MlResultLogErrorFields.NoExceptionValue);
    }

    #endregion


    #region Las dos maneras de aportar los argumentos (decision D-32)

    /// <summary>
    /// Comprueba que la sobrecarga con <c>params</c> pasa al registrador los argumentos que da el
    /// llamante, sin mirar el detalle del error.
    /// </summary>
    [Fact]
    public void ConArgumentosDelLlamante_SePublicaElCampoDelLlamante()
    {
        var spy = new StructuredLoggerSpy();

        Fallido().LogMlResultErrorIfFailTemplate(spy, Plantilla, 4711);

        spy.Entries.Should().HaveCount(1);
        spy.Last.OriginalFormat.Should().Be(Plantilla);
        spy.Last.Field("PedidoId").Should().Be(4711);
    }

    /// <summary>
    /// Comprueba que el constructor de argumentos <b>no</b> se ejecuta cuando el nivel esta
    /// deshabilitado, igual que los constructores de mensaje del resto de la API.
    /// </summary>
    [Fact]
    public void ConNivelDeshabilitado_ElConstructorDeArgumentosNoSeEjecuta()
    {
        var spy       = new StructuredLoggerSpy { MinimumLevel = LogLevel.Critical };
        var invocado  = false;

        Fallido().LogMlResultErrorIfFailTemplate(spy, Plantilla, _ => { invocado = true; return [1]; });

        invocado.Should().BeFalse("la construccion de argumentos es perezosa");
        spy.Entries.Should().BeEmpty();
        spy.IsEnabledCallCount.Should().Be(1, "se consulta IsEnabled una sola vez");
    }

    #endregion


    #region Las cuatro familias y su condicion de registro

    /// <summary>
    /// Comprueba que ninguna de las cuatro familias registra nada ante un resultado valido.
    /// </summary>
    [Fact]
    public void AnteUnResultadoValido_NingunaFamiliaRegistra()
    {
        var spy = new StructuredLoggerSpy();

        Valido().LogMlResultErrorIfFailTemplate                 (spy, Plantilla, 1);
        Valido().LogMlResultErrorIfFailWithValueTemplate        (spy, Plantilla, 1);
        Valido().LogMlResultErrorIfFailWithExceptionTemplate    (spy, Plantilla, 1);
        Valido().LogMlResultErrorIfFailWithoutExceptionTemplate (spy, Plantilla, 1);

        spy.Entries.Should().BeEmpty();
    }

    /// <summary>
    /// Comprueba que <c>IfFailWithValue</c> solo registra cuando el detalle lleva valor.
    /// </summary>
    [Fact]
    public void IfFailWithValueTemplate_SoloRegistraCuandoElDetalleLlevaValor()
    {
        var conValor = new StructuredLoggerSpy();
        var sinValor = new StructuredLoggerSpy();

        FallidoConValor().LogMlResultErrorIfFailWithValueTemplate(conValor, Plantilla, 1);
        Fallido()        .LogMlResultErrorIfFailWithValueTemplate(sinValor, Plantilla, 1);

        conValor.Entries.Should().HaveCount(1);
        sinValor.Entries.Should().BeEmpty();
    }

    /// <summary>
    /// Comprueba la paridad de las dos familias de excepcion: ante un detalle con excepcion
    /// registra <c>IfFailWithException</c>, y ante uno sin excepcion registra
    /// <c>IfFailWithoutException</c>. Es el mismo invariante que la US-14 vigila para la rama
    /// clasica, trasladado a las plantillas.
    /// </summary>
    [Fact]
    public void LasDosFamiliasDeExcepcion_MantienenLaParidadDeLaUS14()
    {
        var conWith    = new StructuredLoggerSpy();
        var conWithout = new StructuredLoggerSpy();
        var sinWith    = new StructuredLoggerSpy();
        var sinWithout = new StructuredLoggerSpy();

        FallidoConExcepcion().LogMlResultErrorIfFailWithExceptionTemplate   (conWith   , Plantilla, 1);
        FallidoConExcepcion().LogMlResultErrorIfFailWithoutExceptionTemplate(conWithout, Plantilla, 1);
        Fallido()            .LogMlResultErrorIfFailWithExceptionTemplate   (sinWith   , Plantilla, 1);
        Fallido()            .LogMlResultErrorIfFailWithoutExceptionTemplate(sinWithout, Plantilla, 1);

        conWith   .Entries.Should().HaveCount(1, "el detalle lleva excepcion");
        conWithout.Entries.Should().BeEmpty   ("el detalle lleva excepcion");
        sinWith   .Entries.Should().BeEmpty   ("el detalle no lleva excepcion");
        sinWithout.Entries.Should().HaveCount(1, "el detalle no lleva excepcion");
    }

    #endregion


    #region Politicas de nulos, pereza y niveles

    /// <summary>
    /// Comprueba que un registrador nulo no registra nada ni lanza, y que el resultado de origen
    /// se devuelve intacto (politica D-06).
    /// </summary>
    [Fact]
    public void ConRegistradorNulo_NoLanzaYDevuelveElOrigen()
    {
        var devuelto = Fallido().LogMlResultErrorIfFailTemplate(null!, Plantilla, 1);

        devuelto.IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// Comprueba que una plantilla nula no registra nada y devuelve el resultado de origen.
    /// </summary>
    [Fact]
    public void ConPlantillaNula_NoRegistraNadaYDevuelveElOrigen()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = Fallido().LogMlResultErrorIfFailTemplate(spy, null!, 1);

        spy.Entries.Should().BeEmpty();
        devuelto.IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// Comprueba que unos argumentos nulos no impiden registrar la plantilla.
    /// </summary>
    /// <remarks>
    /// El <c>null</c> hay que convertirlo a <c>object?[]</c> de forma explicita: con el literal
    /// desnudo la llamada es ambigua entre la sobrecarga de <c>params</c> y la del constructor de
    /// argumentos, porque <c>null</c> es convertible a las dos. Es un matiz de la API, no un
    /// defecto, y queda documentado en el punto 54 del prompt de migracion.
    /// </remarks>
    [Fact]
    public void ConArgumentosNulos_SeRegistraLaPlantilla()
    {
        var spy = new StructuredLoggerSpy();

        Fallido().LogMlResultErrorIfFailTemplate(spy, Plantilla, (object?[])null!);

        spy.Entries.Should().HaveCount(1);
        spy.Last.OriginalFormat.Should().Be(Plantilla);
    }

    /// <summary>
    /// Comprueba que cada fichero de nivel registra con su propio nivel de severidad.
    /// </summary>
    /// <param name="level">Nivel bajo prueba.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_RegistraLaPlantillaDeFalloConSuPropioNivel(LogLevel level)
    {
        var spy = new StructuredLoggerSpy();

        var operacion = OperacionIfFailDe(level);
        operacion(Fallido(), spy, Plantilla);

        spy.Entries.Should().HaveCount(1);
        spy.Last.LogLevel.Should().Be(level);
        spy.Last.OriginalFormat.Should().Be(Plantilla);
    }

    /// <summary>Devuelve la invocacion de <c>IfFailTemplate</c> del nivel indicado.</summary>
    /// <param name="level">Nivel cuya sobrecarga se quiere invocar.</param>
    /// <returns>Delegado que aplica la operacion.</returns>
    private static Func<MlResult<string>, ILogger, string, MlResult<string>> OperacionIfFailDe(LogLevel level)
        => level switch
        {
            LogLevel.Trace       => (s, l, m) => s.LogMlResultTraceIfFailTemplate      (l, m, 1),
            LogLevel.Debug       => (s, l, m) => s.LogMlResultDebugIfFailTemplate      (l, m, 1),
            LogLevel.Information => (s, l, m) => s.LogMlResultInformationIfFailTemplate(l, m, 1),
            LogLevel.Warning     => (s, l, m) => s.LogMlResultWarningIfFailTemplate    (l, m, 1),
            LogLevel.Error       => (s, l, m) => s.LogMlResultErrorIfFailTemplate      (l, m, 1),
            LogLevel.Critical    => (s, l, m) => s.LogMlResultCriticalIfFailTemplate   (l, m, 1),
            _                    => throw new ArgumentOutOfRangeException(nameof(level))
        };

    #endregion


    #region Variantes asincronas

    /// <summary>
    /// Comprueba que la variante asincrona sobre <c>MlResult&lt;T&gt;</c> registra lo mismo que la
    /// sincrona.
    /// </summary>
    [Fact]
    public async Task VarianteAsincronaSobreResultado_RegistraLoMismo()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = await Fallido().LogMlResultErrorIfFailTemplateAsync(spy, Plantilla, 4711);

        spy.Entries.Should().HaveCount(1);
        spy.Last.Field("PedidoId").Should().Be(4711);
        devuelto.IsFailure.Should().BeTrue();
    }

    /// <summary>
    /// Comprueba que la variante asincrona sobre <c>Task&lt;MlResult&lt;T&gt;&gt;</c> registra lo
    /// mismo que la sincrona.
    /// </summary>
    [Fact]
    public async Task VarianteAsincronaSobreTarea_RegistraLoMismo()
    {
        var spy = new StructuredLoggerSpy();

        var devuelto = await Fallido().ToAsync()
                                      .LogMlResultErrorIfFailTemplateAsync(spy,
                                                                           MlResultLogErrorFields.DefaultTemplate,
                                                                           MlResultLogErrorFields.From);

        spy.Entries.Should().HaveCount(1);
        spy.Last.HasField(MlResultLogErrorFields.ErrorMessageField).Should().BeTrue();
        devuelto.IsFailure.Should().BeTrue();
    }

    #endregion


    #region Simetria entre los seis ficheros de nivel

    /// <summary>
    /// Comprueba que los seis niveles exponen las cuatro familias de plantilla de fallo, para que
    /// la paridad que la epica 01 vigila no se rompa por un nivel olvidado.
    /// </summary>
    /// <param name="level">Nivel bajo prueba.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLasCuatroFamiliasDePlantillaDeFallo(LogLevel level)
    {
        var nombres = FirmasPublicas().Select(m => m.Name)
                                      .Where (n => n.StartsWith($"LogMlResult{level}", StringComparison.Ordinal))
                                      .Distinct()
                                      .ToList();

        nombres.Should().Contain($"LogMlResult{level}IfFailTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithValueTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithExceptionTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithoutExceptionTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailTemplateAsync");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithValueTemplateAsync");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithExceptionTemplateAsync");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithoutExceptionTemplateAsync");
    }

    /// <summary>
    /// Comprueba que las cuatro familias nuevas <b>no</b> capturan las llamadas existentes con
    /// mensaje fijo, es decir, que el punto 30 del prompt de migracion sigue a salvo.
    /// </summary>
    [Fact]
    public void LaFamiliaNueva_NoCapturaLasLlamadasExistentes()
    {
        var spy = new StructuredLoggerSpy();

        Fallido().LogMlResultIfFail(spy, LogLevel.Error, "mensaje fijo de siempre");

        spy.Entries.Should().HaveCount(1);
        spy.Last.OriginalFormat.Should().Be("mensaje fijo de siempre",
                                            "la llamada clasica sigue resolviendose a la sobrecarga clasica");
        spy.Last.FieldNames.Should().NotContain("PedidoId");
    }

    #endregion

}
