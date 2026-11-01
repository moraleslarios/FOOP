// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Collections.Immutable;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

/// <summary>
/// Red de seguridad de la épica 9 (US-41): demuestra que el <see cref="StructuredLoggerSpy"/> ve
/// los ámbitos de registro, con sus propiedades, su anidamiento y su cierre.
/// </summary>
/// <remarks>
/// <para>
/// Hasta la 2.3.0 el espía devolvía un <c>NullScope</c>, así que <b>ninguna</b> prueba podía afirmar
/// nada sobre los ámbitos. Sin esta clase, las historias US-42 a US-45 no son verificables.
/// </para>
/// <para>
/// Los dos últimos tests son las <b>mediciones 1 y 2</b> del apéndice de la épica: la primera decide
/// <c>D-33</c> (si el <c>using</c> manual basta) y la segunda decide <c>D-36</c> (qué sobrevive al
/// <c>await</c>). Son mediciones, no afirmaciones de diseño: fijan el comportamiento <b>medido</b>
/// del estándar para que la decisión no se tome por intuición.
/// </para>
/// </remarks>
public class StructuredLoggerSpyScopeTests
{
    #region BeginScope · captura de los ámbitos

    [Fact]
    public void BeginScope_RegistraElAmbitoConSusPropiedades()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope(new Dictionary<string, object?> { ["Pedido"] = 77 }.ToList()))
        {
            spy.LogInformation("dentro");
        }

        spy.Scopes          .Should().HaveCount(1);
        spy.LastScope.Depth .Should().Be(0);
        spy.LastScope.HasProperty("Pedido").Should().BeTrue();
        spy.LastScope.Property   ("Pedido").Should().Be(77);
        spy.LastScope.PropertyNames        .Should().ContainSingle().Which.Should().Be("Pedido");
    }

    [Fact]
    public void BeginScope_ConPlantilla_PublicaElOriginalFormat()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77)) spy.LogInformation("dentro");

        spy.LastScope.OriginalFormat     .Should().Be("Pedido {Pedido}");
        spy.LastScope.Property("Pedido") .Should().Be(77);
    }

    [Fact]
    public void BeginScope_ConEstadoNoEnumerable_DejaLasPropiedadesANulo()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("un estado suelto")) spy.LogInformation("dentro");

        spy.LastScope.State             .Should().Be("un estado suelto");
        spy.LastScope.Properties        .Should().BeNull();
        spy.LastScope.PropertyNames     .Should().BeEmpty();
        spy.LastScope.HasProperty("Nada").Should().BeFalse();
    }

    #endregion

    #region Las trazas saben dentro de qué ámbito cayeron

    [Fact]
    public void Log_DentroDeUnAmbito_AdjuntaElAmbitoALaTraza()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77)) spy.LogInformation("dentro");

        spy.Last.IsInsideScope                 .Should().BeTrue();
        spy.Last.Scopes                        .Should().HaveCount(1);
        spy.Last.HasScopeProperty("Pedido")    .Should().BeTrue();
        spy.Last.ScopeProperty   ("Pedido")    .Should().Be(77);
    }

    [Fact]
    public void Log_FueraDelAmbito_NoAdjuntaNingunAmbito()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77)) spy.LogInformation("dentro");

        spy.LogInformation("fuera");

        spy.Entries                             .Should().HaveCount(2);
        spy.Entries[0].IsInsideScope            .Should().BeTrue();
        spy.Entries[1].IsInsideScope            .Should().BeFalse();
        spy.Entries[1].HasScopeProperty("Pedido").Should().BeFalse();
    }

    [Fact]
    public void Log_ConAmbitosAnidados_RecibeLosDosEnOrdenDeFueraADentro()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Correlacion {Correlacion}", "abc"))
        using (spy.BeginScope("Pedido {Pedido}", 77))
        {
            spy.LogInformation("dentro de los dos");
        }

        spy.Last.Scopes                          .Should().HaveCount(2);
        spy.Last.Scopes[0].Property("Correlacion").Should().Be("abc");
        spy.Last.Scopes[1].Property("Pedido")     .Should().Be(77);
        spy.Last.ScopeProperty("Correlacion")     .Should().Be("abc");
        spy.Last.ScopeProperty("Pedido")          .Should().Be(77);
    }

    [Fact]
    public void ScopeProperty_ConNombreRepetido_GanaElAmbitoMasInterno()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Origen {Origen}", "externo"))
        using (spy.BeginScope("Origen {Origen}", "interno"))
        {
            spy.LogInformation("dentro");
        }

        spy.Last.ScopeProperty("Origen").Should().Be("interno");
    }

    #endregion

    #region Detección del ámbito sin cerrar

    [Fact]
    public void BeginScope_SinCerrar_SeDetectaComoAmbitoFiltrado()
    {
        var spy = new StructuredLoggerSpy();

        // A propósito SIN using: es el fallo que la opción (b) de D-33 haría posible.
        spy.BeginScope("Pedido {Pedido}", 77);

        spy.LogInformation("contaminada");

        spy.LeakedScopes             .Should().HaveCount(1);
        spy.LastScope.IsClosed       .Should().BeFalse();
        spy.ActiveScopeCount         .Should().Be(1);
        spy.Last.IsInsideScope       .Should().BeTrue();
    }

    [Fact]
    public void BeginScope_ConUsing_NoDejaAmbitosFiltrados()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77)) spy.LogInformation("dentro");

        spy.LeakedScopes       .Should().BeEmpty();
        spy.LastScope.IsClosed .Should().BeTrue();
        spy.LastScope.ClosedAt .Should().NotBeNull();
        spy.ActiveScopeCount   .Should().Be(0);
    }

    [Fact]
    public void Dispose_DosVeces_SeContabilizaYNoDesapilaDosVeces()
    {
        var spy    = new StructuredLoggerSpy();
        var ambito = spy.BeginScope("Pedido {Pedido}", 77)!;

        ambito.Dispose();
        var cierre = spy.LastScope.ClosedAt;
        ambito.Dispose();

        spy.LastScope.DisposeCallCount.Should().Be(2);
        spy.LastScope.ClosedAt        .Should().Be(cierre, "el primer cierre es el que cuenta");
        spy.ActiveScopeCount          .Should().Be(0);
    }

    [Fact]
    public void Clear_ReiniciaTambienLosAmbitos()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77)) spy.LogInformation("dentro");

        spy.Clear();

        spy.Scopes          .Should().BeEmpty();
        spy.LeakedScopes    .Should().BeEmpty();
        spy.Entries         .Should().BeEmpty();
        spy.ActiveScopeCount.Should().Be(0);
    }

    #endregion

    #region Compatibilidad con la API previa del espía

    [Fact]
    public void LasTrazasSinAmbito_ConservanLaApiDeCamposDeLa230()
    {
        var spy = new StructuredLoggerSpy();

        spy.LogMlResultInformationTemplate("Pedido {Pedido} a las {Hora}", 77, "nueve");

        spy.Last.OriginalFormat        .Should().Be("Pedido {Pedido} a las {Hora}");
        spy.Last.HasField("Pedido")    .Should().BeTrue();
        spy.Last.Field   ("Pedido")    .Should().Be(77);
        spy.Last.Scopes                .Should().BeEmpty();
        spy.Last.IsInsideScope         .Should().BeFalse();
    }

    #endregion

    #region Medición 1 del apéndice · el using manual, a mano (decide D-33)

    [Fact]
    public void Medicion1_ElUsingManualFunciona_PeroRompeElEstiloDeExpresion()
    {
        var spy = new StructuredLoggerSpy();

        var origen = "42".ToMlResultValid();

        // Así hay que escribirlo HOY, con el using del estándar: la cadena se parte en dos
        // sentencias y hace falta una variable intermedia para sacar el resultado del bloque.
        MlResult<string> resultado;

        using (spy.BeginScope("Pedido {Pedido}", 77))
        {
            resultado = origen.LogMlResultInformationTemplate(spy, "Paso 1 de {Pedido}", 77)
                              .LogMlResultInformationTemplate(spy, "Paso 2 de {Pedido}", 77);
        }

        // La medición: el using SÍ funciona, las dos trazas comparten contexto.
        resultado.IsSuccess                        .Should().BeTrue();
        spy.Entries                                .Should().HaveCount(2);
        spy.Entries.Should().OnlyContain(x => x.IsInsideScope);
        spy.Entries.Should().OnlyContain(x => Equals(x.ScopeProperty("Pedido"), 77));
        spy.LeakedScopes                           .Should().BeEmpty();
    }

    #endregion

    #region Medición 2 del apéndice · qué sobrevive al await (decide D-36)

    [Fact]
    public async Task Medicion2_LoQueSeIniciaDentroDelAmbito_LoHereda()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77))
        {
            await Task.Run(async () =>
            {
                await Task.Yield();
                spy.LogInformation("iniciada dentro");
            });
        }

        // El AsyncLocal fluye hacia DENTRO de lo que se inicia dentro del ámbito.
        spy.Last.IsInsideScope           .Should().BeTrue();
        spy.Last.ScopeProperty("Pedido") .Should().Be(77);
    }

    [Fact]
    public async Task Medicion2_UnTaskYaIniciadoAntesDelAmbito_NoLoHereda()
    {
        var spy      = new StructuredLoggerSpy();
        var arrancar = new TaskCompletionSource();

        // El Task se INICIA fuera del ámbito: es el caso peligroso de la sobrecarga Async, que
        // recibe un Task<MlResult<T>> que el consumidor ya ha puesto en marcha.
        var tarea = Task.Run(async () =>
        {
            await arrancar.Task;
            spy.LogInformation("iniciada fuera, ejecutada dentro");
        });

        using (spy.BeginScope("Pedido {Pedido}", 77))
        {
            arrancar.SetResult();
            await tarea;
        }

        // La medición que decide D-36: NO lo hereda. El ámbito no viaja a un flujo ya arrancado.
        spy.Last.IsInsideScope            .Should().BeFalse();
        spy.Last.HasScopeProperty("Pedido").Should().BeFalse();
    }

    [Fact]
    public async Task Medicion2_DosTuberiasConcurrentes_NoSeContaminanElAmbito()
    {
        var spy = new StructuredLoggerSpy();

        async Task Tuberia(string correlacion)
        {
            using (spy.BeginScope("Correlacion {Correlacion}", correlacion))
            {
                await Task.Yield();
                spy.LogInformation("traza de {Correlacion}", correlacion);
            }
        }

        await Task.WhenAll(Tuberia("A"), Tuberia("B"));

        spy.Entries.Should().HaveCount(2);

        foreach (var entrada in spy.Entries)
        {
            entrada.Scopes               .Should().HaveCount(1, "cada tubería ve sólo su ámbito");
            entrada.ScopeProperty("Correlacion").Should().Be(entrada.Field("Correlacion"));
        }

        spy.LeakedScopes.Should().BeEmpty();
    }

    [Fact]
    public void Medicion2_LaPilaDeAmbitosEsInmutable_YNoSeCompartePorReferencia()
    {
        var spy = new StructuredLoggerSpy();

        using (spy.BeginScope("Pedido {Pedido}", 77))
        {
            spy.LogInformation("primera");

            using (spy.BeginScope("Linea {Linea}", 3)) spy.LogInformation("segunda");

            spy.LogInformation("tercera");
        }

        // Si la pila se compartiese por referencia, las tres trazas verían dos ámbitos.
        spy.Entries[0].Scopes.Should().HaveCount(1);
        spy.Entries[1].Scopes.Should().HaveCount(2);
        spy.Entries[2].Scopes.Should().HaveCount(1);

        ImmutableStack<int>.Empty.Should().BeEmpty();
    }

    #endregion
}
