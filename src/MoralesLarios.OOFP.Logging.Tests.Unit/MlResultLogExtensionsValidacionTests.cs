// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de la validacion de argumentos de las extensiones de registro (historia US-12 de la
/// epica 03).
/// </summary>
/// <remarks>
/// <para>
/// La politica adoptada es la <b>opcion (c) de la decision D-06</b>: cuando el <c>logger</c> es
/// nulo no se registra nada y se devuelve el resultado de origen intacto, sin lanzar excepcion y
/// sin convertir el resultado valido en fallido. Es la unica opcion que <b>no</b> introduce
/// ninguna ruptura para los consumidores y la mas coherente con el estilo ferroviario de la
/// libreria: una traza que no se puede escribir no es un error del dominio.
/// </para>
/// <para>
/// La <b>decision D-12</b> se resuelve con la opcion (a), la misma politica que <c>D-06</c>: si el
/// constructor de mensaje necesario es nulo tampoco se registra nada y se devuelve el origen
/// intacto. Las dos unicas sobrecargas que no pueden devolver el origen, porque su valor de retorno
/// es <c>MlResult&lt;ILogger&gt;</c>, devuelven un resultado fallido.
/// </para>
/// </remarks>
public class MlResultLogExtensionsValidacionTests
{
    /// <summary>Nivel de severidad comun a todas las pruebas.</summary>
    private const LogLevel Nivel = LogLevel.Information;

    /// <summary>Registrador nulo con el que se invocan todas las sobrecargas bajo prueba.</summary>
    private static readonly ILogger Nulo = null!;


    /// <summary>
    /// Comprueba que la sobrecarga de entrada sobre <see cref="ILogger"/> con registrador nulo
    /// devuelve un resultado fallido en lugar de lanzar <see cref="NullReferenceException"/>.
    /// </summary>
    [Fact]
    public void LogMlResult_SobreLoggerNulo_DevuelveResultadoFallido()
    {
        var resultado = Nulo.LogMlResult(Nivel, "mensaje");

        resultado.IsFailure.Should().BeTrue("D-06 (c): no hay ILogger que devolver, asi que se falla");
        Descripcion(resultado).Should().Contain("logger");
    }

    /// <summary>
    /// Comprueba que la version asincrona de la sobrecarga de entrada sobre <see cref="ILogger"/>
    /// con registrador nulo devuelve un resultado fallido.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task LogMlResultAsync_SobreLoggerNulo_DevuelveResultadoFallido()
    {
        var resultado = await Nulo.LogMlResultAsync(Nivel, "mensaje");

        resultado.IsFailure.Should().BeTrue("D-06 (c): no hay ILogger que devolver, asi que se falla");
        Descripcion(resultado).Should().Contain("logger");
    }

    /// <summary>
    /// Comprueba que la sobrecarga con mensaje fijo sobre <c>MlResult&lt;T&gt;</c> y registrador
    /// nulo devuelve el resultado de origen intacto.
    /// </summary>
    [Fact]
    public void LogMlResult_ConMensajeFijoYLoggerNulo_DevuelveElOrigenIntacto()
    {
        var resultado = "valor".ToMlResultValid().LogMlResult(Nulo, Nivel, "mensaje");

        Valor(resultado).Should().Be("valor", "D-06 (c): no se registra, pero el origen no se altera");
    }

    /// <summary>
    /// Comprueba que la version asincrona sobre <c>MlResult&lt;T&gt;</c> con registrador nulo
    /// devuelve el resultado de origen intacto.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task LogMlResultAsync_SobreMlResultConLoggerNulo_DevuelveElOrigenIntacto()
    {
        var resultado = await "valor".ToMlResultValid().LogMlResultAsync(Nulo, Nivel, "mensaje");

        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la version asincrona sobre <c>Task&lt;MlResult&lt;T&gt;&gt;</c> con
    /// registrador nulo devuelve el resultado de origen intacto.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task LogMlResultAsync_SobreTareaConLoggerNulo_DevuelveElOrigenIntacto()
    {
        var resultado = await Task.FromResult("valor".ToMlResultValid()).LogMlResultAsync(Nulo, Nivel, "mensaje");

        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la sobrecarga con constructores de mensaje y registrador nulo devuelve el
    /// resultado de origen intacto, incluso cuando los constructores si estan informados.
    /// </summary>
    [Fact]
    public void LogMlResult_ConConstructoresYLoggerNulo_DevuelveElOrigenIntacto()
    {
        var resultado = "valor".ToMlResultValid()
                               .LogMlResult(Nulo,
                                            Nivel,
                                            (Func<string, string>)(x => $"valido:{x}"),
                                            (Func<MlErrorDetails, string>)(e => $"fallo:{e.GetErrorsSummary()}"));

        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la sobrecarga con constructores de mensaje nulos no registra nada ni lanza
    /// excepcion, que es la politica de la decision D-12 (a).
    /// </summary>
    [Fact]
    public void LogMlResult_ConConstructoresNulos_NoRegistraNada()
    {
        var spy = new LoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .LogMlResult(spy,
                                            Nivel,
                                            (Func<string, string>)null!,
                                            (Func<MlErrorDetails, string>)null!);

        spy.Entries.Should().BeEmpty("D-12 (a): sin constructor de mensaje no hay nada que registrar");
        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la sobrecarga de tuplas con un constructor de mensaje nulo no lanza excepcion
    /// y devuelve el resultado de origen intacto.
    /// </summary>
    [Fact]
    public void LogMlResult_ConTuplasYConstructorNulo_NoLanzaYDevuelveElOrigen()
    {
        var spy = new LoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .LogMlResult(spy,
                                            (Nivel, (Func<string, string>)null!),
                                            (LogLevel.Error, (Func<MlErrorDetails, string>)null!));

        spy.Entries.Should().BeEmpty("D-12 (a): el constructor nulo no debe propagar excepcion");
        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la sobrecarga de tuplas con registrador nulo devuelve el resultado de origen
    /// intacto.
    /// </summary>
    [Fact]
    public void LogMlResult_ConTuplasYLoggerNulo_DevuelveElOrigenIntacto()
    {
        var resultado = "valor".ToMlResultValid()
                               .LogMlResult(Nulo,
                                            (Nivel, (Func<string, string>)(x => x)),
                                            (LogLevel.Error, (Func<MlErrorDetails, string>)(e => e.GetErrorsSummary())));

        Valor(resultado).Should().Be("valor");
    }

    /// <summary>
    /// Comprueba que la familia <c>IfFail</c> con constructor de mensaje nulo sobre un resultado
    /// fallido no lanza excepcion y no registra nada.
    /// </summary>
    [Fact]
    public void LogMlResultIfFail_ConConstructorNulo_NoLanzaNiRegistra()
    {
        var spy = new LoggerSpy();

        var resultado = "fallo".ToMlResultFail<string>()
                               .LogMlResultIfFail(spy, Nivel, (Func<MlErrorDetails, string>)null!);

        spy.Entries.Should().BeEmpty("D-12 (a): sin constructor de mensaje no hay nada que registrar");
        resultado.IsFailure.Should().BeTrue("el origen fallido se devuelve intacto");
    }

    /// <summary>
    /// Comprueba que la familia <c>IfFail</c> con registrador nulo sobre un resultado fallido no
    /// lanza excepcion y devuelve el origen intacto.
    /// </summary>
    [Fact]
    public void LogMlResultIfFail_ConLoggerNulo_NoLanzaYDevuelveElOrigen()
    {
        var resultado = "fallo".ToMlResultFail<string>()
                               .LogMlResultIfFail(Nulo, Nivel, (Func<MlErrorDetails, string>)(e => e.GetErrorsSummary()));

        resultado.IsFailure.Should().BeTrue("D-06 (c): el origen fallido se devuelve intacto");
    }


    /// <summary>Extrae el valor de un resultado valido, o una marca si el resultado es fallido.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado del que se extrae el valor.</param>
    /// <returns>Representacion textual del valor, o <c>"[fallido]"</c> si el resultado es fallido.</returns>
    private static string Valor<T>(MlResult<T> source)
        => source.Match(valid: x => $"{x}", fail: _ => "[fallido]");

    /// <summary>Extrae la descripcion de los errores de un resultado fallido.</summary>
    /// <typeparam name="T">Tipo del valor transportado por el resultado.</typeparam>
    /// <param name="source">Resultado del que se extraen los errores.</param>
    /// <returns>Descripcion de los errores, o <c>"[valido]"</c> si el resultado es valido.</returns>
    private static string Descripcion<T>(MlResult<T> source)
        => source.Match(valid: _ => "[valido]", fail: e => e.ToErrorsDetailsDescription());
}
