// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de regresion de la paridad entre las familias <c>LogMlResultIfFailWithException</c> y
/// <c>LogMlResultIfFailWithoutException</c> (historia US-14 de la epica 03).
/// </summary>
/// <remarks>
/// <para>
/// Estas pruebas existen para que el defecto <c>F1</c> corregido por la <b>US-11</b> no pueda
/// reaparecer. <c>F1</c> consistia en que una de las nueve sobrecargas de
/// <c>LogMlResultIfFailWithExceptionAsync</c> delegaba en
/// <c>LogMlResultIfFailWithValueAsync</c>, de modo que evaluaba el metadato <c>Value</c> en lugar
/// del metadato <c>Ex</c>. La delegacion cruzada no rompia la compilacion y ninguna prueba la
/// detectaba, porque no existia ninguna que comparase el comportamiento observable de las tres
/// sobrecargas de una misma familia.
/// </para>
/// <para>
/// El invariante que se fija aqui es doble:
/// </para>
/// <list type="number">
///   <item>
///     Ante un detalle de error <b>con</b> excepcion, <c>IfFailWithException</c> registra traza y
///     <c>IfFailWithoutException</c> no registra nada. Ante un detalle <b>sin</b> excepcion ocurre
///     exactamente lo contrario.
///   </item>
///   <item>
///     Las tres formas de invocar cada familia (sobre <c>MlResult&lt;T&gt;</c>, sobre
///     <c>Task&lt;MlResult&lt;T&gt;&gt;</c> y con mensaje fijo) producen el mismo resultado
///     observable. Si alguien vuelve a cruzar una delegacion, esta comparacion se rompe.
///   </item>
/// </list>
/// </remarks>
public class MlResultLogExtensionsParidadIfFailWithExceptionTests
{
    /// <summary>Mensaje de referencia que registran todas las variantes bajo prueba.</summary>
    private const string Mensaje = "paridad";

    /// <summary>Nivel de severidad comun a todas las variantes bajo prueba.</summary>
    private const LogLevel Nivel = LogLevel.Error;

    /// <summary>Resultado fallido cuyo detalle de error lleva una excepcion en el metadato <c>Ex</c>.</summary>
    private static MlResult<string> ConExcepcion()
        => MlErrorDetails.FromErrorMessageWithException("con-excepcion", new InvalidOperationException("boom"));

    /// <summary>Resultado fallido cuyo detalle de error no lleva ninguna excepcion.</summary>
    private static MlResult<string> SinExcepcion()
        => MlErrorDetails.FromErrorMessage("sin-excepcion").ToMlResultFail<string>();


    /// <summary>
    /// Comprueba que, ante un detalle de error <b>con</b> excepcion,
    /// <c>IfFailWithException</c> registra traza y <c>IfFailWithoutException</c> no registra nada.
    /// </summary>
    [Fact]
    public void ConExcepcion_SoloRegistraLaFamiliaWithException_RegresionUS11()
    {
        var spyWith    = new LoggerSpy();
        var spyWithout = new LoggerSpy();

        ConExcepcion().LogMlResultIfFailWithException(spyWith, Nivel, _ => Mensaje);
        ConExcepcion().LogMlResultIfFailWithoutException(spyWithout, Nivel, _ => Mensaje);

        spyWith.Entries.Should().HaveCount(1, "el detalle de error lleva el metadato Ex");
        spyWith.Last.Message.Should().Be(Mensaje);
        spyWith.Last.LogLevel.Should().Be(Nivel);

        spyWithout.Entries.Should().BeEmpty("IfFailWithoutException es la rama complementaria");
    }

    /// <summary>
    /// Comprueba el caso simetrico: ante un detalle de error <b>sin</b> excepcion,
    /// <c>IfFailWithoutException</c> registra traza y <c>IfFailWithException</c> no registra nada.
    /// </summary>
    [Fact]
    public void SinExcepcion_SoloRegistraLaFamiliaWithoutException_RegresionUS11()
    {
        var spyWith    = new LoggerSpy();
        var spyWithout = new LoggerSpy();

        SinExcepcion().LogMlResultIfFailWithException(spyWith, Nivel, _ => Mensaje);
        SinExcepcion().LogMlResultIfFailWithoutException(spyWithout, Nivel, _ => Mensaje);

        spyWith.Entries.Should().BeEmpty("el detalle de error no lleva el metadato Ex");

        spyWithout.Entries.Should().HaveCount(1, "IfFailWithoutException es la rama complementaria");
        spyWithout.Last.Message.Should().Be(Mensaje);
        spyWithout.Last.LogLevel.Should().Be(Nivel);
    }

    /// <summary>
    /// Comprueba que las seis variantes de <c>IfFailWithException</c> que reciben un constructor de
    /// mensaje <c>Func&lt;MlErrorDetails, string&gt;</c> o un mensaje fijo producen el mismo
    /// resultado observable, tanto sobre <c>MlResult&lt;T&gt;</c> como sobre
    /// <c>Task&lt;MlResult&lt;T&gt;&gt;</c>. Es la prueba que habria detectado el defecto
    /// <c>F1</c> de la US-11.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithException_LasSeisVariantesSeComportanIgual_RegresionUS11()
    {
        await ComprobarParidadIfFailWithException(ConExcepcion, esperado: 1);
        await ComprobarParidadIfFailWithException(SinExcepcion, esperado: 0);
    }

    /// <summary>
    /// Comprueba que las seis variantes de <c>IfFailWithException</c> registran el mismo numero de
    /// trazas para un mismo resultado de origen.
    /// </summary>
    /// <param name="origen">Fabrica del resultado fallido de origen.</param>
    /// <param name="esperado">Numero de trazas que debe registrar cada variante.</param>
    /// <returns>Tarea que representa la comprobacion.</returns>
    private static async Task ComprobarParidadIfFailWithException(Func<MlResult<string>> origen, int esperado)
    {
        var registros = new List<int>
        {
            Contar(spy => origen().LogMlResultIfFailWithException(spy, Nivel, _ => Mensaje)),
            Contar(spy => origen().LogMlResultIfFailWithException(spy, Nivel, Mensaje)),
            await ContarAsync(spy => origen().LogMlResultIfFailWithExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => origen().LogMlResultIfFailWithExceptionAsync(spy, Nivel, Mensaje)),
            await ContarAsync(spy => Task.FromResult(origen()).LogMlResultIfFailWithExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => Task.FromResult(origen()).LogMlResultIfFailWithExceptionAsync(spy, Nivel, Mensaje))
        };

        registros.Should().AllBeEquivalentTo(
            esperado,
            "las seis variantes de IfFailWithException deben delegar en el mismo cuerpo; si una se "
          + "cruza con otra familia, como ocurria con el defecto F1 corregido por la US-11, esta "
          + "comparacion se rompe");
    }

    /// <summary>
    /// Comprueba que las tres sobrecargas de <c>IfFailWithException</c> que reciben un constructor
    /// de mensaje <c>Func&lt;MlErrorDetails, Exception, string&gt;</c> producen el mismo resultado
    /// observable. La tercera de ellas, la que recibe una tarea, es exactamente la que padecia el
    /// defecto <c>F1</c> corregido por la US-11.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithException_LasTresSobrecargasConExcepcionEnElMensajeSeComportanIgual_RegresionUS11()
    {
        var conSinc  = Contar(spy => ConExcepcion().LogMlResultIfFailWithException(spy, Nivel, (_, ex) => ex.Message));
        var conAsinc = await ContarAsync(spy => ConExcepcion().LogMlResultIfFailWithExceptionAsync(spy, Nivel, (_, ex) => ex.Message));
        var conTarea = await ContarAsync(spy => Task.FromResult(ConExcepcion()).LogMlResultIfFailWithExceptionAsync(spy, Nivel, (_, ex) => ex.Message));

        conSinc.Should().Be(1);
        conAsinc.Should().Be(1, "la version asincrona sobre MlResult debe registrar igual que la sincrona");
        conTarea.Should().Be(1, "US-11: la version sobre Task delegaba antes en IfFailWithValueAsync y no registraba nada");

        var sinSinc  = Contar(spy => SinExcepcion().LogMlResultIfFailWithException(spy, Nivel, (_, ex) => ex.Message));
        var sinAsinc = await ContarAsync(spy => SinExcepcion().LogMlResultIfFailWithExceptionAsync(spy, Nivel, (_, ex) => ex.Message));
        var sinTarea = await ContarAsync(spy => Task.FromResult(SinExcepcion()).LogMlResultIfFailWithExceptionAsync(spy, Nivel, (_, ex) => ex.Message));

        sinSinc.Should().Be(0);
        sinAsinc.Should().Be(0);
        sinTarea.Should().Be(0, "US-11: sin metadato Ex no debe registrarse nada en ninguna de las tres");
    }

    /// <summary>
    /// Comprueba que las seis variantes de <c>IfFailWithoutException</c> se comportan igual entre
    /// si, de forma complementaria a la familia <c>IfFailWithException</c>.
    /// </summary>
    /// <returns>Tarea que representa la ejecucion de la prueba.</returns>
    [Fact]
    public async Task IfFailWithoutException_LasSeisVariantesSeComportanIgual_RegresionUS11()
    {
        var registros = new List<int>
        {
            Contar(spy => SinExcepcion().LogMlResultIfFailWithoutException(spy, Nivel, _ => Mensaje)),
            Contar(spy => SinExcepcion().LogMlResultIfFailWithoutException(spy, Nivel, Mensaje)),
            await ContarAsync(spy => SinExcepcion().LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => SinExcepcion().LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, Mensaje)),
            await ContarAsync(spy => Task.FromResult(SinExcepcion()).LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => Task.FromResult(SinExcepcion()).LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, Mensaje))
        };

        registros.Should().AllBeEquivalentTo(1, "sin metadato Ex las seis variantes deben registrar");

        var conExcepcion = new List<int>
        {
            Contar(spy => ConExcepcion().LogMlResultIfFailWithoutException(spy, Nivel, _ => Mensaje)),
            Contar(spy => ConExcepcion().LogMlResultIfFailWithoutException(spy, Nivel, Mensaje)),
            await ContarAsync(spy => ConExcepcion().LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => ConExcepcion().LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, Mensaje)),
            await ContarAsync(spy => Task.FromResult(ConExcepcion()).LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, _ => Mensaje)),
            await ContarAsync(spy => Task.FromResult(ConExcepcion()).LogMlResultIfFailWithoutExceptionAsync(spy, Nivel, Mensaje))
        };

        conExcepcion.Should().AllBeEquivalentTo(0, "con metadato Ex ninguna de las seis debe registrar");
    }


    /// <summary>Ejecuta una operacion sincrona sobre un espia nuevo y devuelve el numero de trazas registradas.</summary>
    /// <param name="operacion">Operacion de registro bajo prueba.</param>
    /// <returns>Numero de trazas registradas por la operacion.</returns>
    private static int Contar(Action<LoggerSpy> operacion)
    {
        var spy = new LoggerSpy();

        operacion(spy);

        return spy.Entries.Count;
    }

    /// <summary>Ejecuta una operacion asincrona sobre un espia nuevo y devuelve el numero de trazas registradas.</summary>
    /// <param name="operacion">Operacion de registro bajo prueba.</param>
    /// <returns>Tarea que produce el numero de trazas registradas por la operacion.</returns>
    private static async Task<int> ContarAsync(Func<LoggerSpy, Task<MlResult<string>>> operacion)
    {
        var spy = new LoggerSpy();

        await operacion(spy);

        return spy.Entries.Count;
    }
}
