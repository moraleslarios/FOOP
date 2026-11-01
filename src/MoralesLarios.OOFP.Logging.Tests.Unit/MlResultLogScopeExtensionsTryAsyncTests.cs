// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de las variantes <c>Async</c> de la familia <c>TryWithLogScope</c> de
/// <see cref="MlResultLogScopeExtensions"/>.
/// </summary>
/// <remarks>
/// <para>
/// Cubren los criterios de aceptación de la <b>US-44</b> de la épica 9 de la versión 2.4.0 sobre los
/// tres cruces receptor/tramo de la convención asíncrona, y dejan constancia de la <b>asimetría</b>
/// que impone <c>D-36</c> con la opción <b>(b)</b>: la tarea de origen se espera <b>antes</b> de abrir
/// el ámbito y, por tanto, <b>fuera</b> de la captura, de modo que su excepción <b>propaga</b> en vez
/// de convertirse en fallo.
/// </para>
/// <para>
/// ⚠️ Igual que en las pruebas de la US-43, aquí <b>no</b> se comprueba <c>ActiveScopeCount</c>: la
/// pila del espía viaja en un <see cref="AsyncLocal{T}"/> y el <c>Dispose</c> de un tramo asíncrono se
/// ejecuta en un contexto de ejecución reanudado, cuya escritura no vuelve al del método de prueba. Se
/// comprueba el cierre con <c>LeakedScopes</c>, <c>IsClosed</c> y <c>DisposeCallCount</c>.
/// </para>
/// </remarks>
public class MlResultLogScopeExtensionsTryAsyncTests
{

    #region Receptor sincrono y tramo asincrono · plantilla

    [Fact]
    public async Task TryWithLogScopeAsync_TramoQueLanza_DevuelveFalloYCierraElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.IsClosed.Should().BeTrue();
        espia.LastScope.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public async Task TryWithLogScopeAsync_TramoQueLanza_ElFalloArrastraLaExcepcionOriginal()
    {
        var espia   = new StructuredLoggerSpy();
        var lanzada = new InvalidOperationException("boom");

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw lanzada);

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.GetException().TryGetValue(out var recuperada).Should().BeTrue();
        recuperada.Should().BeSameAs(lanzada);
    }

    [Fact]
    public async Task TryWithLogScopeAsync_TramoQueLanza_ConConstructorDeMensaje_UsaEseMensaje()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"),
                                                           errorMessageBuilder: ex => $"el pedido 17 ha fallado: {ex.Message}");

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain("el pedido 17 ha fallado: boom");
    }

    [Fact]
    public async Task TryWithLogScopeAsync_TramoCorrecto_LasTrazasDelTramoCaenDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await 3.ToMlResultValid()
                               .TryWithLogScopeAsync(espia,
                                                     "calculo {Id}", [1],
                                                     sectionAsync: r => r.Map(x => x * 2)
                                                                         .LogMlResultInformationTemplate(espia, "tramo en curso")
                                                                         .ToAsync());

        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
        espia.Entries[0].ScopeProperty("Id").Should().Be(1);
    }

    #endregion

    #region Receptor sincrono y tramo asincrono · diccionario

    [Fact]
    public async Task TryWithLogScopeAsync_ConDiccionario_TramoQueLanza_DevuelveFalloYCierraElAmbito()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           propiedades,
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
    }

    #endregion

    #region Receptor asincrono y tramo asincrono

    [Fact]
    public async Task TryWithLogScopeAsync_ReceptorAsincrono_TramoQueLanza_DevuelveFalloYCierraElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .ToAsync()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task TryWithLogScopeAsync_ReceptorAsincronoConDiccionario_TramoQueLanza_DevuelveFallo()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .ToAsync()
                                     .TryWithLogScopeAsync(espia,
                                                           propiedades,
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Receptor asincrono y tramo sincrono

    [Fact]
    public async Task TryWithLogScopeAsync_ReceptorAsincronoTramoSincrono_TramoQueLanza_DevuelveFallo()
    {
        var espia   = new StructuredLoggerSpy();
        var lanzada = new InvalidOperationException("boom");

        var resultado = await "valor".ToMlResultValid()
                                     .ToAsync()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           section: MlResult<string> (_) => throw lanzada);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.GetException().TryGetValue(out var recuperada).Should().BeTrue();
        recuperada.Should().BeSameAs(lanzada);
        espia.LeakedScopes.Should().BeEmpty();
    }

    [Fact]
    public async Task TryWithLogScopeAsync_ReceptorAsincronoTramoSincronoConDiccionario_TramoQueLanza_DevuelveFallo()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .ToAsync()
                                     .TryWithLogScopeAsync(espia,
                                                           propiedades,
                                                           section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Asimetria documentada de la tarea de origen

    [Fact]
    public async Task TryWithLogScopeAsync_ReceptorAsincronoQueLanza_PropagaLaExcepcionYNoAbreAmbito()
    {
        var espia  = new StructuredLoggerSpy();
        var origen = Task.FromException<MlResult<string>>(new InvalidOperationException("origen"));

        var accion = async () => await origen.TryWithLogScopeAsync(espia,
                                                                   "pedido {PedidoId}", [17],
                                                                   sectionAsync: r => r.ToAsync());

        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("origen");
        espia.Scopes.Should().BeEmpty();
    }

    #endregion

    #region Un resultado fallido no es una excepcion

    [Fact]
    public async Task TryWithLogScopeAsync_TramoQueDevuelveFallo_DevuelveEseMismoFalloSinTocarlo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           "pedido {PedidoId}", [17],
                                                           sectionAsync: _ => MlErrorDetails.FromErrorMessage("fallo-del-tramo")
                                                                                            .ToMlResultFail<string>()
                                                                                            .ToAsync());

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain("fallo-del-tramo");
        errores!.GetException().IsFailure.Should().BeTrue();
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Ambitos anidados

    [Fact]
    public async Task TryWithLogScopeAsync_Anidado_TramoInternoQueLanza_CierraLosDosAmbitos()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           "externo {Externo}", [1],
                                                           sectionAsync: r => r.TryWithLogScopeAsync(espia,
                                                                                                     "interno {Interno}", [2],
                                                                                                     sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom")));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(2);
        espia.Scopes.Should().OnlyContain(x => x.IsClosed);
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Guardas de nulos

    [Fact]
    public async Task TryWithLogScopeAsync_TramoNulo_DevuelveResultadoFallido()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync<string, string>(espia,
                                                                           "pedido {PedidoId}", [17],
                                                                           sectionAsync: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public async Task TryWithLogScopeAsync_ConDiccionario_TramoNulo_DevuelveResultadoFallido()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync<string, string>(espia, propiedades, sectionAsync: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public async Task TryWithLogScopeAsync_PlantillaNula_EvaluaElTramoSinAbrirAmbitoYCaptura()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .TryWithLogScopeAsync(espia,
                                                           null!, [17],
                                                           sectionAsync: async Task<MlResult<string>> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    #endregion

}
