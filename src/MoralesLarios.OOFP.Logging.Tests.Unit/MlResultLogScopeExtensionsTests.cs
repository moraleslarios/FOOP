// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de <see cref="MlResultLogScopeExtensions"/>: el ámbito mínimo sobre un tramo síncrono de
/// una cadena de <see cref="MlResult{T}"/>.
/// </summary>
/// <remarks>
/// Cubren los criterios de aceptación de la <b>US-42</b> de la épica 9 de la versión 2.4.0, y se
/// apoyan en el espía estructurado que la US-41 enseñó a ver los ámbitos.
/// </remarks>
public class MlResultLogScopeExtensionsTests
{

    #region Forma de la API · plantilla y argumentos

    [Fact]
    public void WithLogScope_ConPlantilla_AbreUnAmbitoConLaPlantillaSinInterpolar()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia, "pedido {PedidoId}", [17], section: r => r);

        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.OriginalFormat.Should().Be("pedido {PedidoId}");
        espia.LastScope.Property("PedidoId").Should().Be(17);
    }

    [Fact]
    public void WithLogScope_ConPlantilla_LasTrazasDelTramoCaenDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia,
                             "pedido {PedidoId}", [17],
                             section: r => r.LogMlResultInformationTemplate(espia, "tramo en curso"));

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
        espia.Entries[0].ScopeProperty("PedidoId").Should().Be(17);
    }

    [Fact]
    public void WithLogScope_ConPlantilla_DevuelveElResultadoDelTramo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = 3.ToMlResultValid()
                         .WithLogScope(espia, "calculo {Id}", [1], section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
    }

    [Fact]
    public void WithLogScope_ConPlantilla_ArgumentosNulos_NoRevienta()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .WithLogScope(espia, "sin argumentos", null!, section: r => r);

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    #endregion

    #region Forma de la API · diccionario de propiedades

    [Fact]
    public void WithLogScope_ConDiccionario_PasaLasPropiedadesTalCual()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        "valor".ToMlResultValid()
               .WithLogScope(espia, propiedades, section: r => r);

        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.State.Should().BeSameAs(propiedades);
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
    }

    [Fact]
    public void WithLogScope_ConDiccionario_LasTrazasDelTramoVenLaPropiedad()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        "valor".ToMlResultValid()
               .WithLogScope(espia,
                             propiedades,
                             section: r => r.LogMlResultInformationTemplate(espia, "tramo en curso"));

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].HasScopeProperty("CorrelacionId").Should().BeTrue();
        espia.Entries[0].ScopeProperty("CorrelacionId").Should().Be("abc-123");
    }

    #endregion

    #region El ámbito se cierra siempre

    [Fact]
    public void WithLogScope_TramoCorrecto_CierraElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia, "pedido {PedidoId}", [17], section: r => r);

        espia.LeakedScopes.Should().BeEmpty();
        espia.ActiveScopeCount.Should().Be(0);
        espia.LastScope.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public void WithLogScope_TramoQueLanza_CierraElAmbitoYPropagaLaExcepcion()
    {
        var espia = new StructuredLoggerSpy();

        var accion = () => "valor".ToMlResultValid()
                                  .WithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        accion.Should().Throw<InvalidOperationException>().WithMessage("boom");

        espia.LeakedScopes.Should().BeEmpty();
        espia.ActiveScopeCount.Should().Be(0);
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void WithLogScope_ConDiccionario_TramoQueLanza_CierraElAmbito()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var accion = () => "valor".ToMlResultValid()
                                  .WithLogScope(espia,
                                                propiedades,
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        accion.Should().Throw<InvalidOperationException>();

        espia.LeakedScopes.Should().BeEmpty();
        espia.ActiveScopeCount.Should().Be(0);
    }

    #endregion

    #region Orden de apertura y de cierre

    [Fact]
    public void WithLogScope_AbreElAmbitoAntesDelTramoYLoCierraDespues()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia,
                             "pedido {PedidoId}", [17],
                             section: r => r.LogMlResultInformationTemplate(espia, "primera")
                                            .LogMlResultInformationTemplate(espia, "ultima"));

        espia.Entries.Should().HaveCount(2);
        espia.Entries.Should().OnlyContain(x => x.IsInsideScope);
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    [Fact]
    public void WithLogScope_LasTrazasDeFueraDelTramo_NoVenElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia, "pedido {PedidoId}", [17], section: r => r)
               .LogMlResultInformationTemplate(espia, "despues del ambito");

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeFalse();
    }

    [Fact]
    public void WithLogScope_Anidado_CadaTrazaVeLosDosAmbitos()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia,
                             "externo {Externo}", [1],
                             section: r => r.WithLogScope(espia,
                                                          "interno {Interno}", [2],
                                                          section: s => s.LogMlResultInformationTemplate(espia, "anidada")));

        espia.Scopes.Should().HaveCount(2);
        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].Scopes.Should().HaveCount(2);
        espia.Entries[0].ScopeProperty("Externo").Should().Be(1);
        espia.Entries[0].ScopeProperty("Interno").Should().Be(2);
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Transparencia del ámbito

    [Fact]
    public void WithLogScope_TramoQueNoRegistraNada_NoEmiteNingunaTraza()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .WithLogScope(espia, "pedido {PedidoId}", [17], section: r => r);

        espia.Entries.Should().BeEmpty();
        espia.Scopes.Should().HaveCount(1);
    }

    [Fact]
    public void WithLogScope_ResultadoFallido_NoAlteraElFalloYLoEntregaAlTramo()
    {
        var espia  = new StructuredLoggerSpy();
        var fallido = MlErrorDetails.FromErrorMessage("fallo-de-origen").ToMlResultFail<string>();

        var resultado = fallido.WithLogScope(espia,
                                             "pedido {PedidoId}", [17],
                                             section: r => r.LogMlResultInformationTemplate(espia, "tramo con fallo"));

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain("fallo-de-origen");
        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
    }

    #endregion

    #region Guardas de nulos

    [Fact]
    public void WithLogScope_ConPlantilla_LoggerNulo_EvaluaElTramoSinAmbito()
    {
        var resultado = 3.ToMlResultValid()
                         .WithLogScope(null!, "pedido {PedidoId}", [17], section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
    }

    [Fact]
    public void WithLogScope_ConDiccionario_LoggerNulo_EvaluaElTramoSinAmbito()
    {
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = 3.ToMlResultValid()
                         .WithLogScope(null!, propiedades, section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
    }

    [Fact]
    public void WithLogScope_PlantillaNula_EvaluaElTramoSinAbrirAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = 3.ToMlResultValid()
                         .WithLogScope(espia, null!, [17], section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void WithLogScope_PropiedadesNulas_EvaluaElTramoSinAbrirAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = 3.ToMlResultValid()
                         .WithLogScope(espia, (IEnumerable<KeyValuePair<string, object?>>)null!, section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void WithLogScope_ConPlantilla_TramoNulo_DevuelveResultadoFallido()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .WithLogScope<string, string>(espia, "pedido {PedidoId}", [17], section: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void WithLogScope_ConDiccionario_TramoNulo_DevuelveResultadoFallido()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = "valor".ToMlResultValid()
                               .WithLogScope<string, string>(espia, propiedades, section: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    #endregion

}
