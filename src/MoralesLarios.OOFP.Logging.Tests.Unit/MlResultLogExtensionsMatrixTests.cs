// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Matriz de pruebas que recorre los seis niveles de severidad (<c>Trace</c>, <c>Debug</c>,
/// <c>Information</c>, <c>Warning</c>, <c>Error</c> y <c>Critical</c>) por las siete regiones
/// funcionales de la API de registro, para fijar el comportamiento observable de
/// <c>MlResultLogExtensions</c> antes de tocar una sola linea de produccion.
/// </summary>
/// <remarks>
/// Alcance acordado en la decision D-05 (opcion b) de la epica 01 de la version 2.2.0: se cubre
/// una pareja valido/fallido por cada combinacion nivel x region, mas los casos frontera, en lugar
/// de las 339 firmas publicas.
/// </remarks>
public class MlResultLogExtensionsMatrixTests
{
    #region Nombres de las regiones funcionales

    /// <summary>Region que registra la traza siempre, con independencia del estado del resultado.</summary>
    private const string Incondicional = "incondicional";

    /// <summary>Region que registra la traza unicamente cuando el resultado es valido.</summary>
    private const string SoloSiValido = "soloSiValido";

    /// <summary>Region que registra la traza unicamente cuando el resultado es fallido.</summary>
    private const string SoloSiFallido = "soloSiFallido";

    /// <summary>Region que registra la traza cuando el resultado es fallido y lleva un valor en los metadatos.</summary>
    private const string SiFallidoConValor = "siFallidoConValor";

    /// <summary>Region que registra la traza cuando el resultado es fallido y lleva una excepcion en los metadatos.</summary>
    private const string SiFallidoConExcepcion = "siFallidoConExcepcion";

    /// <summary>Region que registra la traza cuando el resultado es fallido y NO lleva excepcion en los metadatos.</summary>
    private const string SiFallidoSinExcepcion = "siFallidoSinExcepcion";

    #endregion


    #region Infraestructura de la matriz

    /// <summary>
    /// Operacion de registro de un solo mensaje, comun a las seis primeras regiones funcionales.
    /// </summary>
    /// <param name="source">Resultado de origen sobre el que se aplica la operacion.</param>
    /// <param name="logger">Registrador al que se envian las trazas.</param>
    /// <param name="message">Mensaje descriptivo que se espera registrado.</param>
    /// <returns>El resultado devuelto por la operacion de registro.</returns>
    private delegate MlResult<string> LogOperation(MlResult<string> source, ILogger logger, string message);

    /// <summary>
    /// Tabla de despacho nivel x region. Es necesaria porque el nivel de severidad forma parte del
    /// nombre del metodo (<c>LogMlResultError</c>, <c>LogMlResultWarning</c>, ...) y por tanto no se
    /// puede seleccionar con un parametro de un <c>[Theory]</c>.
    /// </summary>
    private static readonly Dictionary<(LogLevel Level, string Region), LogOperation> Operations = new()
    {
        [(LogLevel.Trace      , Incondicional)] = (s, l, m) => s.LogMlResultTrace      (l, m),
        [(LogLevel.Debug      , Incondicional)] = (s, l, m) => s.LogMlResultDebug      (l, m),
        [(LogLevel.Information, Incondicional)] = (s, l, m) => s.LogMlResultInformation(l, m),
        [(LogLevel.Warning    , Incondicional)] = (s, l, m) => s.LogMlResultWarning    (l, m),
        [(LogLevel.Error      , Incondicional)] = (s, l, m) => s.LogMlResultError      (l, m),
        [(LogLevel.Critical   , Incondicional)] = (s, l, m) => s.LogMlResultCritical   (l, m),

        [(LogLevel.Trace      , SoloSiValido)] = (s, l, m) => s.LogMlResultTraceIfValid      (l, m),
        [(LogLevel.Debug      , SoloSiValido)] = (s, l, m) => s.LogMlResultDebugIfValid      (l, m),
        [(LogLevel.Information, SoloSiValido)] = (s, l, m) => s.LogMlResultInformationIfValid(l, m),
        [(LogLevel.Warning    , SoloSiValido)] = (s, l, m) => s.LogMlResultWarningIfValid    (l, m),
        [(LogLevel.Error      , SoloSiValido)] = (s, l, m) => s.LogMlResultErrorIfValid      (l, m),
        [(LogLevel.Critical   , SoloSiValido)] = (s, l, m) => s.LogMlResultCriticalIfValid   (l, m),

        [(LogLevel.Trace      , SoloSiFallido)] = (s, l, m) => s.LogMlResultTraceIfFail      (l, m),
        [(LogLevel.Debug      , SoloSiFallido)] = (s, l, m) => s.LogMlResultDebugIfFail      (l, m),
        [(LogLevel.Information, SoloSiFallido)] = (s, l, m) => s.LogMlResultInformationIfFail(l, m),
        [(LogLevel.Warning    , SoloSiFallido)] = (s, l, m) => s.LogMlResultWarningIfFail    (l, m),
        [(LogLevel.Error      , SoloSiFallido)] = (s, l, m) => s.LogMlResultErrorIfFail      (l, m),
        [(LogLevel.Critical   , SoloSiFallido)] = (s, l, m) => s.LogMlResultCriticalIfFail   (l, m),

        [(LogLevel.Trace      , SiFallidoConValor)] = (s, l, m) => s.LogMlResultTraceIfFailWithValue      (l, m),
        [(LogLevel.Debug      , SiFallidoConValor)] = (s, l, m) => s.LogMlResultDebugIfFailWithValue      (l, m),
        [(LogLevel.Information, SiFallidoConValor)] = (s, l, m) => s.LogMlResultInformationIfFailWithValue(l, m),
        [(LogLevel.Warning    , SiFallidoConValor)] = (s, l, m) => s.LogMlResultWarningIfFailWithValue    (l, m),
        [(LogLevel.Error      , SiFallidoConValor)] = (s, l, m) => s.LogMlResultErrorIfFailWithValue      (l, m),
        [(LogLevel.Critical   , SiFallidoConValor)] = (s, l, m) => s.LogMlResultCriticalIfFailWithValue   (l, m),

        [(LogLevel.Trace      , SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultTraceIfFailWithException      (l, m),
        [(LogLevel.Debug      , SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultDebugIfFailWithException      (l, m),
        [(LogLevel.Information, SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultInformationIfFailWithException(l, m),
        [(LogLevel.Warning    , SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultWarningIfFailWithException    (l, m),
        [(LogLevel.Error      , SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultErrorIfFailWithException      (l, m),
        [(LogLevel.Critical   , SiFallidoConExcepcion)] = (s, l, m) => s.LogMlResultCriticalIfFailWithException   (l, m),

        [(LogLevel.Trace      , SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultTraceIfFailWithoutException      (l, m),
        [(LogLevel.Debug      , SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultDebugIfFailWithoutException      (l, m),
        [(LogLevel.Information, SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultInformationIfFailWithoutException(l, m),
        [(LogLevel.Warning    , SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultWarningIfFailWithoutException    (l, m),
        [(LogLevel.Error      , SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultErrorIfFailWithoutException      (l, m),
        [(LogLevel.Critical   , SiFallidoSinExcepcion)] = (s, l, m) => s.LogMlResultCriticalIfFailWithoutException   (l, m),
    };

    /// <summary>
    /// Tabla de despacho de la septima region: la sobrecarga completa, que recibe el mensaje de la
    /// rama valida y el de la rama fallida a la vez.
    /// </summary>
    private static readonly Dictionary<LogLevel, Func<MlResult<string>, ILogger, string, string, MlResult<string>>> CompleteOperations = new()
    {
        [LogLevel.Trace      ] = (s, l, v, f) => s.LogMlResultTrace      (l, v, f),
        [LogLevel.Debug      ] = (s, l, v, f) => s.LogMlResultDebug      (l, v, f),
        [LogLevel.Information] = (s, l, v, f) => s.LogMlResultInformation(l, v, f),
        [LogLevel.Warning    ] = (s, l, v, f) => s.LogMlResultWarning    (l, v, f),
        [LogLevel.Error      ] = (s, l, v, f) => s.LogMlResultError      (l, v, f),
        [LogLevel.Critical   ] = (s, l, v, f) => s.LogMlResultCritical   (l, v, f),
    };

    /// <summary>Los seis niveles de severidad que la libreria expone con un fichero propio.</summary>
    private static readonly LogLevel[] AllLevels =
    [
        LogLevel.Trace, LogLevel.Debug, LogLevel.Information,
        LogLevel.Warning, LogLevel.Error, LogLevel.Critical
    ];

    /// <summary>Las siete regiones funcionales, en el mismo orden que el apendice 7.1 de la epica 01.</summary>
    private static readonly string[] AllRegions =
    [
        Incondicional, SoloSiValido, SoloSiFallido,
        SiFallidoConValor, SiFallidoConExcepcion, SiFallidoSinExcepcion
    ];

    /// <summary>Mensaje de referencia que se espera registrado.</summary>
    private const string Mensaje = "mensaje-de-la-matriz";

    /// <summary>Resultado valido de referencia.</summary>
    private static MlResult<string> Valido() => "valor-ok".ToMlResultValid();

    /// <summary>Resultado fallido de referencia, sin metadatos.</summary>
    private static MlResult<string> FallidoPelado() => MlErrorDetails.FromErrorMessage("fallo-pelado");

    /// <summary>Resultado fallido de referencia con un valor en los metadatos.</summary>
    private static MlResult<string> FallidoConValor()
        => MlErrorDetails.FromErrorMessageWithValue<string>("fallo-con-valor", "valor-perdido");

    /// <summary>Resultado fallido de referencia con una excepcion en los metadatos.</summary>
    private static MlResult<string> FallidoConExcepcion()
        => MlErrorDetails.FromErrorMessageWithException("fallo-con-excepcion", new InvalidOperationException("boom"));

    /// <summary>Producto cartesiano de los seis niveles por las seis regiones de un solo mensaje.</summary>
    public static TheoryData<LogLevel, string> NivelPorRegion()
    {
        var data = new TheoryData<LogLevel, string>();

        foreach (var level in AllLevels)
            foreach (var region in AllRegions)
                data.Add(level, region);

        return data;
    }

    /// <summary>Los seis niveles de severidad, uno por caso.</summary>
    public static TheoryData<LogLevel> Niveles()
    {
        var data = new TheoryData<LogLevel>();

        foreach (var level in AllLevels) data.Add(level);

        return data;
    }

    #endregion


    #region Nivel registrado

    /// <summary>
    /// Comprueba que, para las treinta y seis combinaciones nivel x region, cuando la region
    /// decide registrar la traza lo hace exactamente con el nivel que figura en el nombre del metodo.
    /// </summary>
    /// <param name="level">Nivel de severidad esperado.</param>
    /// <param name="region">Region funcional bajo prueba.</param>
    [Theory]
    [MemberData(nameof(NivelPorRegion))]
    public void CadaRegion_CuandoRegistra_UsaExactamenteElNivelDeSuNombre(LogLevel level, string region)
    {
        var spy   = new LoggerSpy();
        var fuente = FuenteQueRegistra(region);

        Operations[(level, region)](fuente, spy, Mensaje);

        spy.Entries.Should().HaveCount(1, "la region {0} debe registrar una unica traza", region);
        spy.Last.LogLevel.Should().Be(level);
        spy.Last.Message.Should().Be(Mensaje);
    }

    /// <summary>
    /// Comprueba que, para las treinta y seis combinaciones nivel x region, cuando el estado del
    /// resultado no cumple la condicion de la region no se registra ninguna traza.
    /// </summary>
    /// <param name="level">Nivel de severidad de la operacion.</param>
    /// <param name="region">Region funcional bajo prueba.</param>
    [Theory]
    [MemberData(nameof(NivelPorRegion))]
    public void CadaRegion_CuandoNoSeCumpleSuCondicion_NoRegistraNada(LogLevel level, string region)
    {
        if (region == Incondicional) return;   // la region incondicional registra siempre, por definicion

        var spy   = new LoggerSpy();
        var fuente = FuenteQueNoRegistra(region);

        Operations[(level, region)](fuente, spy, Mensaje);

        spy.Entries.Should().BeEmpty("la region {0} no debe registrar nada cuando no se cumple su condicion", region);
    }

    /// <summary>
    /// Comprueba que la region incondicional registra la traza tanto para un resultado valido
    /// como para un resultado fallido.
    /// </summary>
    /// <param name="level">Nivel de severidad esperado.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void RegionIncondicional_RegistraTantoSiEsValidoComoSiEsFallido(LogLevel level)
    {
        var spyValido = new LoggerSpy();
        Operations[(level, Incondicional)](Valido(), spyValido, Mensaje);

        var spyFallido = new LoggerSpy();
        Operations[(level, Incondicional)](FallidoPelado(), spyFallido, Mensaje);

        spyValido .Entries.Should().HaveCount(1);
        spyFallido.Entries.Should().HaveCount(1);
        spyValido .Last.LogLevel.Should().Be(level);
        spyFallido.Last.LogLevel.Should().Be(level);
    }

    /// <summary>
    /// Comprueba la septima region: la sobrecarga completa elige el mensaje de la rama valida o el
    /// de la rama fallida segun el estado del resultado, y en ambos casos usa el nivel de su nombre.
    /// </summary>
    /// <param name="level">Nivel de severidad esperado.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void RegionCompleta_EligeElMensajeSegunElEstadoYMantieneElNivel(LogLevel level)
    {
        var spyValido = new LoggerSpy();
        CompleteOperations[level](Valido(), spyValido, "mensaje-valido", "mensaje-fallido");

        var spyFallido = new LoggerSpy();
        CompleteOperations[level](FallidoPelado(), spyFallido, "mensaje-valido", "mensaje-fallido");

        spyValido .Entries.Should().HaveCount(1);
        spyValido .Last.Message .Should().Be("mensaje-valido");
        spyValido .Last.LogLevel.Should().Be(level);

        spyFallido.Entries.Should().HaveCount(1);
        spyFallido.Last.Message .Should().Be("mensaje-fallido");
        spyFallido.Last.LogLevel.Should().Be(level);
    }

    #endregion


    #region El resultado de origen se devuelve inalterado

    /// <summary>
    /// Comprueba que las regiones que no consultan los metadatos devuelven el resultado de origen
    /// tal cual, sin alterar ni el valor ni los errores.
    /// </summary>
    /// <param name="level">Nivel de severidad de la operacion.</param>
    /// <param name="region">Region funcional bajo prueba.</param>
    [Theory]
    [MemberData(nameof(NivelPorRegion))]
    public void RegionesSinMetadatos_DevuelvenElResultadoDeOrigenInalterado(LogLevel level, string region)
    {
        if (region is SiFallidoConValor or SiFallidoConExcepcion) return;   // estas consultan los metadatos

        var spy = new LoggerSpy();

        var valido   = Valido();
        var devuelto = Operations[(level, region)](valido, spy, Mensaje);

        devuelto.IsSuccess.Should().BeTrue();
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");

        spy.Clear();

        var fallido         = FallidoPelado();
        var devueltoFallido = Operations[(level, region)](fallido, spy, Mensaje);

        devueltoFallido.IsSuccess.Should().BeFalse();
        devueltoFallido.TryGetErrors(out var errores).Should().BeTrue();
        errores.Errors.Should().ContainSingle(x => x.Message == "fallo-pelado");
    }

    /// <summary>
    /// Comprueba que las regiones que consultan los metadatos devuelven el resultado de origen
    /// inalterado cuando el metadato que buscan esta presente.
    /// </summary>
    /// <param name="level">Nivel de severidad de la operacion.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void RegionesConMetadatos_ConElMetadatoPresente_DevuelvenElResultadoDeOrigenInalterado(LogLevel level)
    {
        var spyValor = new LoggerSpy();
        var conValor = Operations[(level, SiFallidoConValor)](FallidoConValor(), spyValor, Mensaje);

        conValor.IsSuccess.Should().BeFalse();
        conValor.TryGetErrors(out var erroresValor).Should().BeTrue();
        erroresValor.Errors.Should().ContainSingle(x => x.Message == "fallo-con-valor");

        var spyExcepcion = new LoggerSpy();
        var conExcepcion = Operations[(level, SiFallidoConExcepcion)](FallidoConExcepcion(), spyExcepcion, Mensaje);

        conExcepcion.IsSuccess.Should().BeFalse();
        conExcepcion.TryGetErrors(out var erroresExcepcion).Should().BeTrue();
        erroresExcepcion.Errors.Should().ContainSingle(x => x.Message == "fallo-con-excepcion");
    }

    /// <summary>
    /// Comprueba que, para un resultado valido, ninguna de las seis regiones condicionales altera
    /// el valor transportado.
    /// </summary>
    /// <param name="level">Nivel de severidad de la operacion.</param>
    /// <param name="region">Region funcional bajo prueba.</param>
    [Theory]
    [MemberData(nameof(NivelPorRegion))]
    public void ConUnResultadoValido_NingunaRegionAlteraElValor(LogLevel level, string region)
    {
        var spy = new LoggerSpy();

        var devuelto = Operations[(level, region)](Valido(), spy, Mensaje);

        devuelto.IsSuccess.Should().BeTrue();
        devuelto.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be("valor-ok");
    }

    #endregion


    #region Simetria entre los seis ficheros de nivel

    /// <summary>
    /// Comprueba el invariante de simetria de la API: los seis ficheros de nivel exponen el mismo
    /// numero de firmas publicas. La comparacion se hace entre los seis recuentos, nunca contra un
    /// literal, para que el invariante siga siendo valido cuando se anadan sobrecargas a los seis.
    /// </summary>
    /// <remarks>
    /// Las plantillas de la rama de fallo (epica 08, US-35) anadieron 24 firmas a cada nivel, de
    /// 51 a 75, repartidas en cuatro familias nuevas. El invariante se mantiene porque el generador
    /// inyecta exactamente el mismo bloque en los seis ficheros.
    /// </remarks>
    [Fact]
    public void LosSeisNiveles_ExponenElMismoNumeroDeFirmasPublicas()
    {
        var recuentos = AllLevels.ToDictionary(level => level, CuentaFirmasDelNivel);

        recuentos.Values.Should().OnlyContain(x => x > 0, "cada nivel debe exponer al menos una firma");

        recuentos.Values.Distinct().Should().HaveCount(
            1,
            "los seis ficheros de nivel deben ser simetricos, y el recuento obtenido fue: {0}",
            string.Join(", ", recuentos.Select(x => $"{x.Key}={x.Value}")));
    }

    /// <summary>
    /// Comprueba que cada uno de los seis niveles expone las once regiones funcionales, de forma
    /// que ninguna region quede huerfana en un nivel concreto. Las cuatro ultimas son las
    /// plantillas de la rama de fallo que anadio la US-35 de la epica 08.
    /// </summary>
    /// <param name="level">Nivel de severidad bajo prueba.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeLasOnceRegionesFuncionales(LogLevel level)
    {
        var nombres = FirmasPublicas()
                            .Select(x => x.Name)
                            .Where(x => x.StartsWith($"LogMlResult{level}", StringComparison.Ordinal))
                            .ToArray();

        nombres.Should().Contain($"LogMlResult{level}");
        nombres.Should().Contain($"LogMlResult{level}IfValid");
        nombres.Should().Contain($"LogMlResult{level}IfFail");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithValue");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithException");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithoutException");
        nombres.Should().Contain($"LogMlResult{level}Async");

        nombres.Should().Contain($"LogMlResult{level}Template");
        nombres.Should().Contain($"LogMlResult{level}IfFailTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithValueTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithExceptionTemplate");
        nombres.Should().Contain($"LogMlResult{level}IfFailWithoutExceptionTemplate");
    }

    /// <summary>
    /// Fija el recuento de firmas publicas por nivel que la epica 01 vigila, para que cualquier
    /// anadido futuro obligue a actualizar este test de forma consciente y a registrar el cambio en
    /// el prompt de migracion.
    /// </summary>
    /// <remarks>
    /// El recuento se obtiene por reflexion sobre el prefijo <c>LogMlResult&lt;Nivel&gt;</c>, asi que
    /// agrega las firmas del fichero de nivel y las de los ficheros transversales de
    /// <c>EventId</c> y de categoria. Paso de 60 a 84 con las plantillas de la rama de fallo (US-35)
    /// y de 84 a 89 con el cruce plantilla x <c>EventId</c> (US-36).
    /// </remarks>
    /// <param name="level">Nivel de severidad bajo prueba.</param>
    [Theory]
    [MemberData(nameof(Niveles))]
    public void CadaNivel_ExponeElRecuentoDeFirmasEsperado(LogLevel level)
        => CuentaFirmasDelNivel(level).Should().Be(
                89,
                "el recuento por nivel quedo fijado en 89 firmas tras cruzar la plantilla con EventId (US-36)");
    #endregion


    #region Ayudas privadas

    /// <summary>
    /// Devuelve el resultado de origen que hace que la region indicada decida registrar la traza.
    /// </summary>
    /// <param name="region">Region funcional bajo prueba.</param>
    /// <returns>Resultado de origen apropiado para la region.</returns>
    private static MlResult<string> FuenteQueRegistra(string region) => region switch
    {
        Incondicional         => Valido(),
        SoloSiValido          => Valido(),
        SoloSiFallido         => FallidoPelado(),
        SiFallidoConValor     => FallidoConValor(),
        SiFallidoConExcepcion => FallidoConExcepcion(),
        SiFallidoSinExcepcion => FallidoPelado(),
        _                     => throw new ArgumentOutOfRangeException(nameof(region), region, "Region no contemplada.")
    };

    /// <summary>
    /// Devuelve el resultado de origen que hace que la region indicada decida NO registrar la traza.
    /// </summary>
    /// <param name="region">Region funcional bajo prueba.</param>
    /// <returns>Resultado de origen apropiado para la region.</returns>
    private static MlResult<string> FuenteQueNoRegistra(string region) => region switch
    {
        SoloSiValido          => FallidoPelado(),
        SoloSiFallido         => Valido(),
        SiFallidoConValor     => Valido(),
        SiFallidoConExcepcion => Valido(),
        SiFallidoSinExcepcion => FallidoConExcepcion(),
        _                     => throw new ArgumentOutOfRangeException(nameof(region), region, "Region no contemplada.")
    };

    /// <summary>Devuelve todas las firmas publicas estaticas de la clase de extensiones de registro.</summary>
    /// <returns>Coleccion de metodos publicos estaticos.</returns>
    private static IEnumerable<MethodInfo> FirmasPublicas()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static);

    /// <summary>Cuenta las firmas publicas que pertenecen al nivel indicado.</summary>
    /// <param name="level">Nivel de severidad.</param>
    /// <returns>Numero de firmas publicas del nivel.</returns>
    private static int CuentaFirmasDelNivel(LogLevel level)
        => FirmasPublicas().Count(x => x.Name.StartsWith($"LogMlResult{level}", StringComparison.Ordinal));

    #endregion
}
