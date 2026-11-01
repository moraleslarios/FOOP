// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de la familia <c>TryWithLogScope</c> de <see cref="MlResultLogScopeExtensions"/>: el ámbito
/// que <b>convierte en fallo</b> la excepción del tramo en lugar de propagarla.
/// </summary>
/// <remarks>
/// <para>
/// Cubren los criterios de aceptación de la <b>US-44</b> de la épica 9 de la versión 2.4.0, que fija la
/// decisión <c>D-34</c> con la opción <b>(c)</b> · <c>WithLogScope</c> propaga, <c>TryWithLogScope</c>
/// captura · y la decisión <c>D-35</c> con la opción <b>(a)</b> · el ámbito es transparente y no emite
/// traza propia.
/// </para>
/// <para>
/// El cierre del ámbito ante una excepción en la variante que <b>propaga</b>, y el anidamiento de dos
/// ámbitos, ya están probados en <c>MlResultLogScopeExtensionsTests</c> con
/// <c>WithLogScope_TramoQueLanza_CierraElAmbitoYPropagaLaExcepcion</c> y
/// <c>WithLogScope_Anidado_CadaTrazaVeLosDosAmbitos</c>; aquí se repiten sólo sobre la variante que
/// captura, porque su camino de cierre es otro.
/// </para>
/// <para>
/// Igual que en las pruebas asíncronas de la US-43, las aserciones sobre tramos asíncronos no usan
/// <c>ActiveScopeCount</c>: la pila del espía viaja en un <c>AsyncLocal</c> y lo que se escribe en una
/// continuación no vuelve al contexto de ejecución de la prueba. Se comprueba el cierre con
/// <c>LeakedScopes</c>, <c>IsClosed</c> y <c>DisposeCallCount</c>.
/// </para>
/// </remarks>
public class MlResultLogScopeExtensionsTryTests
{

    #region La excepcion se convierte en fallo · plantilla

    [Fact]
    public void TryWithLogScope_TramoQueLanza_DevuelveFalloYNoPropagaLaExcepcion()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TryWithLogScope_TramoQueLanza_CierraElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espia,
                                "pedido {PedidoId}", [17],
                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        espia.Scopes.Should().HaveCount(1);
        espia.LeakedScopes.Should().BeEmpty();
        espia.ActiveScopeCount.Should().Be(0);
        espia.LastScope.IsClosed.Should().BeTrue();
        espia.LastScope.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public void TryWithLogScope_TramoQueLanza_ElFalloArrastraLaExcepcionOriginal()
    {
        var espia     = new StructuredLoggerSpy();
        var lanzada   = new InvalidOperationException("boom");

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw lanzada);

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.GetException().TryGetValue(out var recuperada).Should().BeTrue();
        recuperada.Should().BeSameAs(lanzada);
    }

    [Fact]
    public void TryWithLogScope_TramoQueLanza_SinConstructorDeMensaje_UsaElMensajeDelNucleo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MoralesLarios.OOFP.Helpers.EnsureFpMessages.DefaultExceptionError(new InvalidOperationException("boom")));
    }

    [Fact]
    public void TryWithLogScope_TramoQueLanza_ConConstructorDeMensaje_UsaEseMensaje()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"),
                                                errorMessageBuilder: ex => $"el pedido 17 ha fallado: {ex.Message}");

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain("el pedido 17 ha fallado: boom");
    }

    #endregion

    #region La excepcion se convierte en fallo · diccionario

    [Fact]
    public void TryWithLogScope_ConDiccionario_TramoQueLanza_DevuelveFalloYCierraElAmbito()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                propiedades,
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
    }

    [Fact]
    public void TryWithLogScope_ConDiccionario_TramoQueLanza_ElFalloArrastraLaExcepcionOriginal()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };
        var lanzada     = new InvalidOperationException("boom");

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia, propiedades, section: MlResult<string> (_) => throw lanzada);

        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.GetException().TryGetValue(out var recuperada).Should().BeTrue();
        recuperada.Should().BeSameAs(lanzada);
    }

    #endregion

    #region El tramo correcto se comporta igual que en WithLogScope

    [Fact]
    public void TryWithLogScope_TramoCorrecto_DevuelveElResultadoDelTramo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = 3.ToMlResultValid()
                         .TryWithLogScope(espia, "calculo {Id}", [1], section: r => r.Map(x => x * 2));

        resultado.IsSuccess.Should().BeTrue();
        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
    }

    [Fact]
    public void TryWithLogScope_TramoCorrecto_LasTrazasDelTramoCaenDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espia,
                                "pedido {PedidoId}", [17],
                                section: r => r.LogMlResultInformationTemplate(espia, "tramo en curso"));

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
        espia.Entries[0].ScopeProperty("PedidoId").Should().Be(17);
    }

    [Fact]
    public void TryWithLogScope_TramoQueNoRegistraNada_NoEmiteNingunaTraza()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espia, "pedido {PedidoId}", [17], section: r => r);

        espia.Entries.Should().BeEmpty();
        espia.Scopes.Should().HaveCount(1);
    }

    [Fact]
    public void TryWithLogScope_TramoQueLanza_NoEmiteNingunaTraza()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espia,
                                "pedido {PedidoId}", [17],
                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        espia.Entries.Should().BeEmpty();
    }

    #endregion

    #region Un resultado fallido no es una excepcion

    [Fact]
    public void TryWithLogScope_TramoQueDevuelveFallo_DevuelveEseMismoFalloSinTocarlo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "pedido {PedidoId}", [17],
                                                section: _ => MlErrorDetails.FromErrorMessage("fallo-del-tramo").ToMlResultFail<string>());

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain("fallo-del-tramo");
        errores!.GetException().IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TryWithLogScope_TramoQueDevuelveFallo_CierraElAmbitoYRegistraLoMismoQueElCasoValido()
    {
        var espiaValido  = new StructuredLoggerSpy();
        var espiaFallido = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espiaValido,
                                "pedido {PedidoId}", [17],
                                section: r => r.LogMlResultInformationTemplate(espiaValido, "tramo en curso"));

        MlErrorDetails.FromErrorMessage("fallo-de-origen")
                      .ToMlResultFail<string>()
                      .TryWithLogScope(espiaFallido,
                                       "pedido {PedidoId}", [17],
                                       section: r => r.LogMlResultInformationTemplate(espiaFallido, "tramo en curso"));

        espiaFallido.Scopes.Should().HaveCount(espiaValido.Scopes.Count);
        espiaFallido.Entries.Should().HaveCount(espiaValido.Entries.Count);
        espiaFallido.LeakedScopes.Should().BeEmpty();
        espiaFallido.LastScope.IsClosed.Should().BeTrue();
        espiaFallido.LastScope.OriginalFormat.Should().Be(espiaValido.LastScope.OriginalFormat);
        espiaFallido.Entries[0].IsInsideScope.Should().BeTrue();
    }

    #endregion

    #region Ambitos anidados

    [Fact]
    public void TryWithLogScope_Anidado_CadaTrazaVeLosDosAmbitos()
    {
        var espia = new StructuredLoggerSpy();

        "valor".ToMlResultValid()
               .TryWithLogScope(espia,
                                "externo {Externo}", [1],
                                section: r => r.TryWithLogScope(espia,
                                                                "interno {Interno}", [2],
                                                                section: s => s.LogMlResultInformationTemplate(espia, "anidada")));

        espia.Scopes.Should().HaveCount(2);
        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].Scopes.Should().HaveCount(2);
        espia.Entries[0].ScopeProperty("Externo").Should().Be(1);
        espia.Entries[0].ScopeProperty("Interno").Should().Be(2);
        espia.LeakedScopes.Should().BeEmpty();
    }

    [Fact]
    public void TryWithLogScope_Anidado_TramoInternoQueLanza_CierraLosDosAmbitos()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                "externo {Externo}", [1],
                                                section: r => r.TryWithLogScope(espia,
                                                                                "interno {Interno}", [2],
                                                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom")));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().HaveCount(2);
        espia.LeakedScopes.Should().BeEmpty();
        espia.ActiveScopeCount.Should().Be(0);
        espia.Scopes.Should().OnlyContain(x => x.IsClosed);
    }

    #endregion

    #region Guardas de nulos

    [Fact]
    public void TryWithLogScope_ConPlantilla_TramoNulo_DevuelveResultadoFallido()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope<string, string>(espia, "pedido {PedidoId}", [17], section: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void TryWithLogScope_ConDiccionario_TramoNulo_DevuelveResultadoFallido()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope<string, string>(espia, propiedades, section: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void TryWithLogScope_LoggerNulo_CapturaLaExcepcionIgualmente()
    {
        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(null!,
                                                "pedido {PedidoId}", [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.GetException().IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void TryWithLogScope_PlantillaNula_EvaluaElTramoSinAbrirAmbitoYCaptura()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                null!, [17],
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public void TryWithLogScope_PropiedadesNulas_EvaluaElTramoSinAbrirAmbitoYCaptura()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = "valor".ToMlResultValid()
                               .TryWithLogScope(espia,
                                                (IEnumerable<KeyValuePair<string, object?>>)null!,
                                                section: MlResult<string> (_) => throw new InvalidOperationException("boom"));

        resultado.IsFailure.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    #endregion

}
