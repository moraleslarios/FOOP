// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Pruebas de las variantes <c>Async</c> de <see cref="MlResultLogScopeExtensions"/>: qué trazas
/// quedan <b>realmente</b> dentro del ámbito cuando la tubería es asíncrona.
/// </summary>
/// <remarks>
/// <para>
/// Cubren los criterios de aceptación de la <b>US-43</b> de la épica 9 de la versión 2.4.0, y muy en
/// particular los dos casos que la historia exige medir y no suponer: la tarea de origen <b>ya
/// iniciada</b> y dos tuberías <b>concurrentes</b>.
/// </para>
/// <para>
/// ⚠️ Estas pruebas <b>no</b> comprueban <c>ActiveScopeCount</c>. El espía apila los ámbitos en un
/// <see cref="AsyncLocal{T}"/>, y el <c>Dispose</c> de un tramo asíncrono se ejecuta en un contexto
/// de ejecución <b>reanudado</b>, cuya escritura no vuelve al contexto del método de prueba. Lo que
/// sí es fiable, y es lo que se comprueba, es el estado del propio registro del ámbito ·
/// <c>LeakedScopes</c>, <c>IsClosed</c> y <c>DisposeCallCount</c> ·.
/// </para>
/// </remarks>
public class MlResultLogScopeExtensionsAsyncTests
{

    #region Forma de la API · plantilla y argumentos

    [Fact]
    public async Task WithLogScopeAsync_ConPlantilla_AbreUnAmbitoConLaPlantillaSinInterpolar()
    {
        var espia = new StructuredLoggerSpy();

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia, "pedido {PedidoId}", [17], sectionAsync: r => r.ToAsync());

        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.OriginalFormat.Should().Be("pedido {PedidoId}");
        espia.LastScope.Property("PedidoId").Should().Be(17);
    }

    [Fact]
    public async Task WithLogScopeAsync_ConPlantilla_LasTrazasDelTramoCaenDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia,
                                        "pedido {PedidoId}", [17],
                                        sectionAsync: r => r.LogMlResultInformationTemplate(espia, "tramo en curso").ToAsync());

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
        espia.Entries[0].ScopeProperty("PedidoId").Should().Be(17);
    }

    [Fact]
    public async Task WithLogScopeAsync_ConPlantilla_DevuelveElResultadoDelTramo()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await 3.ToMlResultValid()
                               .WithLogScopeAsync(espia,
                                                  "calculo {Id}", [1],
                                                  sectionAsync: r => r.Map(x => x * 2).ToAsync());

        resultado.IsSuccess.Should().BeTrue();
        resultado.TryGetValue(out var valor).Should().BeTrue();
        valor.Should().Be(6);
    }

    [Fact]
    public async Task WithLogScopeAsync_ConPlantilla_ArgumentosNulos_NoRevienta()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync(espia, "sin argumentos", null!, sectionAsync: r => r.ToAsync());

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    #endregion

    #region Forma de la API · diccionario de propiedades

    [Fact]
    public async Task WithLogScopeAsync_ConDiccionario_PasaLasPropiedadesTalCual()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia, propiedades, sectionAsync: r => r.ToAsync());

        espia.Scopes.Should().HaveCount(1);
        espia.LastScope.State.Should().BeSameAs(propiedades);
        espia.LastScope.Property("CorrelacionId").Should().Be("abc-123");
    }

    [Fact]
    public async Task WithLogScopeAsync_ConDiccionario_LasTrazasDelTramoVenLaPropiedad()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia,
                                        propiedades,
                                        sectionAsync: r => r.LogMlResultInformationTemplate(espia, "tramo en curso").ToAsync());

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].HasScopeProperty("CorrelacionId").Should().BeTrue();
        espia.Entries[0].ScopeProperty("CorrelacionId").Should().Be("abc-123");
    }

    #endregion

    #region El ámbito cubre también la continuación del tramo

    [Fact]
    public async Task WithLogScopeAsync_TramoConAwait_LaTrazaPosteriorAlAwaitSigueDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia,
                                        "pedido {PedidoId}", [17],
                                        sectionAsync: async r =>
                                        {
                                            await Task.Yield();

                                            return r.LogMlResultInformationTemplate(espia, "despues del await");
                                        });

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].IsInsideScope.Should().BeTrue();
        espia.Entries[0].ScopeProperty("PedidoId").Should().Be(17);
        espia.LastScope.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public async Task WithLogScopeAsync_TramoCorrecto_CierraElAmbito()
    {
        var espia = new StructuredLoggerSpy();

        await "valor".ToMlResultValid()
                     .WithLogScopeAsync(espia,
                                        "pedido {PedidoId}", [17],
                                        sectionAsync: async r =>
                                        {
                                            await Task.Yield();

                                            return r;
                                        });

        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.IsClosed.Should().BeTrue();
        espia.LastScope.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public async Task WithLogScopeAsync_TramoQueLanzaTrasElAwait_CierraElAmbitoYPropagaLaExcepcion()
    {
        var espia = new StructuredLoggerSpy();

        var accion = async () => await "valor".ToMlResultValid()
                                              .WithLogScopeAsync(espia,
                                                                 "pedido {PedidoId}", [17],
                                                                 sectionAsync: async Task<MlResult<string>> (_) =>
                                                                 {
                                                                     await Task.Yield();

                                                                     throw new InvalidOperationException("boom");
                                                                 });

        await accion.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");

        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task WithLogScopeAsync_ConDiccionario_TramoQueLanzaTrasElAwait_CierraElAmbito()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var accion = async () => await "valor".ToMlResultValid()
                                              .WithLogScopeAsync(espia,
                                                                 propiedades,
                                                                 sectionAsync: async Task<MlResult<string>> (_) =>
                                                                 {
                                                                     await Task.Yield();

                                                                     throw new InvalidOperationException("boom");
                                                                 });

        await accion.Should().ThrowAsync<InvalidOperationException>();

        espia.LeakedScopes.Should().BeEmpty();
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    #endregion

    #region La promesa del await · tarea de origen ya iniciada (D-36 opción b)

    /// <summary>
    /// Caso <b>peligroso</b> que exige medir la US-43: una tarea que <b>ya está en marcha</b> se pasa
    /// como origen a la sobrecarga <c>Async</c>.
    /// </summary>
    /// <remarks>
    /// <b>Resultado medido</b> · las trazas que emite la tarea de origen quedan <b>fuera</b> del
    /// ámbito, porque el origen se espera <b>antes</b> de abrirlo. Es exactamente lo que promete la
    /// decisión <c>D-36</c> opción <b>(b)</b>.
    /// </remarks>
    [Fact]
    public async Task WithLogScopeAsync_TareaDeOrigenYaIniciada_SusTrazasQuedanFueraDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        async Task<MlResult<string>> OrigenAsync()
        {
            await Task.Yield();

            return "valor".ToMlResultValid()
                          .LogMlResultInformationTemplate(espia, "traza del origen");
        }

        var origen = OrigenAsync();

        await origen.WithLogScopeAsync(espia,
                                       "pedido {PedidoId}", [17],
                                       sectionAsync: r => r.LogMlResultInformationTemplate(espia, "traza del tramo").ToAsync());

        espia.Entries.Should().HaveCount(2);

        var trazaDelOrigen = espia.Entries.Single(x => x.Message.Contains("traza del origen"));
        var trazaDelTramo  = espia.Entries.Single(x => x.Message.Contains("traza del tramo"));

        trazaDelOrigen.IsInsideScope.Should().BeFalse("el origen se espera antes de abrir el ámbito · D-36 (b)");
        trazaDelOrigen.Scopes.Should().BeEmpty();

        trazaDelTramo.IsInsideScope.Should().BeTrue();
        trazaDelTramo.ScopeProperty("PedidoId").Should().Be(17);
    }

    [Fact]
    public async Task WithLogScopeAsync_TareaDeOrigenYaIniciada_ConDiccionario_SusTrazasQuedanFueraDelAmbito()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        async Task<MlResult<string>> OrigenAsync()
        {
            await Task.Yield();

            return "valor".ToMlResultValid()
                          .LogMlResultInformationTemplate(espia, "traza del origen");
        }

        await OrigenAsync().WithLogScopeAsync(espia,
                                              propiedades,
                                              sectionAsync: r => r.LogMlResultInformationTemplate(espia, "traza del tramo").ToAsync());

        espia.Entries.Single(x => x.Message.Contains("traza del origen")).IsInsideScope.Should().BeFalse();
        espia.Entries.Single(x => x.Message.Contains("traza del tramo" )).HasScopeProperty("CorrelacionId").Should().BeTrue();
    }

    [Fact]
    public async Task WithLogScopeAsync_OrigenAsincronoConTramoSincrono_LaTrazaDelTramoCaeDentroDelAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .ToAsync()
                                     .WithLogScopeAsync(espia,
                                                        "pedido {PedidoId}", [17],
                                                        section: r => r.LogMlResultInformationTemplate(espia, "tramo sincrono"));

        resultado.IsSuccess.Should().BeTrue();
        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].ScopeProperty("PedidoId").Should().Be(17);
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    [Fact]
    public async Task WithLogScopeAsync_OrigenAsincronoConTramoSincrono_ConDiccionario_LaTrazaDelTramoVeLaPropiedad()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        await "valor".ToMlResultValid()
                     .ToAsync()
                     .WithLogScopeAsync(espia,
                                        propiedades,
                                        section: r => r.LogMlResultInformationTemplate(espia, "tramo sincrono"));

        espia.Entries.Should().HaveCount(1);
        espia.Entries[0].ScopeProperty("CorrelacionId").Should().Be("abc-123");
        espia.LastScope.IsClosed.Should().BeTrue();
    }

    #endregion

    #region Concurrencia · dos tuberías no se contaminan el ámbito

    /// <summary>
    /// Dos tuberías que corren <b>a la vez</b> sobre el <b>mismo</b> registrador, entrelazadas a
    /// propósito con dos compuertas, para que cada una esté suspendida mientras la otra registra.
    /// </summary>
    /// <remarks>
    /// Cada tubería se arranca con <see cref="Task.Run(Func{Task})"/>, que es lo que hace un servidor
    /// real con dos peticiones: cada una recibe su propia copia del contexto de ejecución, de modo
    /// que el <see cref="AsyncLocal{T}"/> de los ámbitos no se comparte entre ellas.
    /// </remarks>
    [Fact]
    public async Task WithLogScopeAsync_DosTuberiasConcurrentes_NoSeContaminanElAmbito()
    {
        var espia     = new StructuredLoggerSpy();
        var compuertaA = new TaskCompletionSource();
        var compuertaB = new TaskCompletionSource();

        Task<MlResult<string>> TuberiaA()
            => "a".ToMlResultValid()
                  .WithLogScopeAsync(espia,
                                     "tuberia {Nombre}", ["A"],
                                     sectionAsync: async r =>
                                     {
                                         compuertaA.SetResult();

                                         await compuertaB.Task;

                                         return r.LogMlResultInformationTemplate(espia, "traza de A");
                                     });

        Task<MlResult<string>> TuberiaB()
            => "b".ToMlResultValid()
                  .WithLogScopeAsync(espia,
                                     "tuberia {Nombre}", ["B"],
                                     sectionAsync: async r =>
                                     {
                                         await compuertaA.Task;

                                         var salida = r.LogMlResultInformationTemplate(espia, "traza de B");

                                         compuertaB.SetResult();

                                         return salida;
                                     });

        await Task.WhenAll(Task.Run(TuberiaA), Task.Run(TuberiaB));

        espia.Entries.Should().HaveCount(2);

        var trazaDeA = espia.Entries.Single(x => x.Message.Contains("traza de A"));
        var trazaDeB = espia.Entries.Single(x => x.Message.Contains("traza de B"));

        trazaDeA.Scopes.Should().HaveCount(1, "una tubería no debe ver el ámbito de la otra");
        trazaDeA.ScopeProperty("Nombre").Should().Be("A");

        trazaDeB.Scopes.Should().HaveCount(1, "una tubería no debe ver el ámbito de la otra");
        trazaDeB.ScopeProperty("Nombre").Should().Be("B");

        espia.Scopes.Should().HaveCount(2);
        espia.LeakedScopes.Should().BeEmpty();
    }

    #endregion

    #region Guardas de nulos

    [Fact]
    public async Task WithLogScopeAsync_LoggerNulo_EvaluaElTramoSinAmbito()
    {
        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync(null!, "pedido {PedidoId}", [17], sectionAsync: r => r.ToAsync());

        resultado.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task WithLogScopeAsync_PlantillaNula_EvaluaElTramoSinAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync(espia, null!, [17], sectionAsync: r => r.ToAsync());

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public async Task WithLogScopeAsync_ConDiccionario_LoggerNulo_EvaluaElTramoSinAmbito()
    {
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync(null!, propiedades, sectionAsync: r => r.ToAsync());

        resultado.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task WithLogScopeAsync_PropiedadesNulas_EvaluaElTramoSinAmbito()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync(espia,
                                                        (IEnumerable<KeyValuePair<string, object?>>)null!,
                                                        sectionAsync: r => r.ToAsync());

        resultado.IsSuccess.Should().BeTrue();
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public async Task WithLogScopeAsync_ConPlantilla_TramoNulo_DevuelveResultadoFallido()
    {
        var espia = new StructuredLoggerSpy();

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync<string, string>(espia, "pedido {PedidoId}", [17], sectionAsync: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    [Fact]
    public async Task WithLogScopeAsync_ConDiccionario_TramoNulo_DevuelveResultadoFallido()
    {
        var espia       = new StructuredLoggerSpy();
        var propiedades = new Dictionary<string, object?> { ["CorrelacionId"] = "abc-123" };

        var resultado = await "valor".ToMlResultValid()
                                     .WithLogScopeAsync<string, string>(espia, propiedades, sectionAsync: null!);

        resultado.IsFailure.Should().BeTrue();
        resultado.TryGetErrors(out var errores).Should().BeTrue();
        errores!.Errors.Select(x => x.Message).Should().Contain(MlResultLogScopeExtensions.NullSectionMessage);
        espia.Scopes.Should().BeEmpty();
    }

    #endregion

}
