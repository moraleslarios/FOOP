// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types.Errors;

/// <summary>
/// Pruebas del modelo de metadatos de error introducido en la version 2.1.1: catalogo de claves
/// reservadas, ranuras de excepcion, fusion sin perdida de informacion, accesores de lectura no
/// destructivos e independencia del diccionario entre instancias.
/// </summary>
public class MlErrorDetailsMetadataTests
{

    #region Utilidades


    private static MlErrorDetails DetallesDe<T>(MlResult<T> source)
        => source.Match(valid: _ => new MlErrorDetails(),
                        fail : x => x);


    private static T ValorDe<T>(MlResult<T> source)
        => source.Match(valid: x => x,
                        fail : _ => default!);


    #endregion


    #region Claves de excepcion


    [Fact]
    public void AppendExDetails_ConClaveExpected_GuardaEnEx()
    {
        var excepcion = new InvalidOperationException("boom");
        var origen    = new Dictionary<string, object> { { MlErrorDetails.MetadataKeys.Expected, "algo" } };

        var resultado = origen.AppendExDetails(excepcion);

        resultado.Should().ContainKey(MlErrorDetails.MetadataKeys.Exception);
        resultado[MlErrorDetails.MetadataKeys.Exception].Should().BeSameAs(excepcion);
        resultado[MlErrorDetails.MetadataKeys.Expected].Should().Be("algo");
    }


    [Fact]
    public void GetException_TrasClaveExpected_RecuperaLaExcepcion()
    {
        var excepcion = new InvalidOperationException("boom");
        var origen    = new Dictionary<string, object> { { MlErrorDetails.MetadataKeys.Expected, "algo" } };

        var detalles = MlErrorDetails.FromErrorMessageDetails("error", origen.AppendExDetails(excepcion));

        detalles.TryGetException(out var obtenida).Should().BeTrue();
        obtenida.Should().BeSameAs(excepcion);
        detalles.GetExceptions().Should().ContainSingle().Which.Should().BeSameAs(excepcion);
    }


    [Theory]
    [InlineData("Expected")]
    [InlineData("ExchangeRate")]
    [InlineData("Extra")]
    [InlineData("ExA1")]
    public void IsExceptionKey_ClavesDeUsuarioConPrefijoEx_DevuelveFalse(string clave)
        => MlErrorDetails.IsExceptionKey(clave).Should().BeFalse();


    [Theory]
    [InlineData("Ex")]
    [InlineData("Ex2")]
    [InlineData("Ex17")]
    public void IsExceptionKey_ClavesCanonicas_DevuelveTrue(string clave)
        => MlErrorDetails.IsExceptionKey(clave).Should().BeTrue();


    [Fact]
    public void NextExceptionKey_SinClaves_DevuelveEx()
        => MlErrorDetails.NextExceptionKey(Array.Empty<string>()).Should().Be("Ex");


    [Fact]
    public void NextExceptionKey_ConEx_DevuelveEx2()
        => MlErrorDetails.NextExceptionKey(new[] { "Ex" }).Should().Be("Ex2");


    [Fact]
    public void NextExceptionKey_ConHueco_RellenaElHueco()
        => MlErrorDetails.NextExceptionKey(new[] { "Ex", "Ex3" }).Should().Be("Ex2");


    #endregion


    #region Fusion de metadatos


    [Fact]
    public void Merge_NoRenumeraLasClavesDeLaParteprincipal()
    {
        var primera = new InvalidOperationException("uno");
        var segunda = new InvalidOperationException("dos");

        var izquierda = MlErrorDetails.FromErrorMessageWithException("izq", primera);
        var derecha   = MlErrorDetails.FromErrorMessageWithException("der", segunda);

        MlErrorDetails fusion = izquierda.Merge(derecha);

        fusion.Metadata[MlErrorDetails.MetadataKeys.Exception].Should().BeSameAs(primera);
        fusion.Metadata["Ex2"].Should().BeSameAs(segunda);
    }


    [Fact]
    public void AppendExDetails_TrasMerge_NoColisiona()
    {
        var primera = new InvalidOperationException("uno");
        var segunda = new InvalidOperationException("dos");
        var tercera = new InvalidOperationException("tres");

        MlErrorDetails fusion = MlErrorDetails.FromErrorMessageWithException("izq", primera)
                                              .Merge(MlErrorDetails.FromErrorMessageWithException("der", segunda));

        var conTercera = fusion.Metadata.AppendExDetails(tercera);

        conTercera["Ex3"].Should().BeSameAs(tercera);
        conTercera.Should().HaveCount(3);
    }


    [Fact]
    public void Merge_ConClaveParamNameEnAmbos_NoLanza()
    {
        var izquierda = MlErrorDetails.FromErrorDetails("izq", MlErrorDetails.MetadataKeys.ParameterName, "a");
        var derecha   = MlErrorDetails.FromErrorDetails("der", MlErrorDetails.MetadataKeys.ParameterName, "b");

        var fusionar = () => izquierda.Merge(derecha);

        fusionar.Should().NotThrow();
    }


    [Fact]
    public void Merge_ConClaveDuplicada_PreservaLosDosValores()
    {
        var izquierda = MlErrorDetails.FromErrorDetails("izq", MlErrorDetails.MetadataKeys.ParameterName, "a");
        var derecha   = MlErrorDetails.FromErrorDetails("der", MlErrorDetails.MetadataKeys.ParameterName, "b");

        MlErrorDetails fusion = izquierda.Merge(derecha);

        fusion.Metadata[MlErrorDetails.MetadataKeys.ParameterName].Should().Be("a");
        fusion.Metadata[$"{MlErrorDetails.MetadataKeys.ParameterName}2"].Should().Be("b");
    }


    [Fact]
    public void MergeErrorsDetailsIfFail_DosGuardiasEnsureFp_Funciona()
    {
        var primera = EnsureFp.NotNull<string>(null!, "El primero es nulo");
        var segunda = EnsureFp.NotNull<string>(null!, "El segundo es nulo");

        var fusionar = () => primera.MergeErrorsDetailsIfFail(segunda);

        fusionar.Should().NotThrow();

        var resultado = primera.MergeErrorsDetailsIfFail(segunda);

        resultado.IsFailure.Should().BeTrue();
        DetallesDe(resultado).Errors.Should().HaveCount(2);
    }


    [Fact]
    public void Merge_DeColeccion_ConservaTodasLasClaves()
    {
        var origen = MlErrorDetails.FromErrorDetails("base", "k0", 0);

        var otros = Enumerable.Range(1, 3)
                              .Select(x => MlErrorDetails.FromErrorDetails($"e{x}", $"k{x}", x))
                              .ToList();

        MlErrorDetails fusion = origen.Merge(otros);

        fusion.Errors.Should().HaveCount(4);
        fusion.Metadata.Should().HaveCount(4);

        for (var i = 0; i < 4; i++) fusion.Metadata[$"k{i}"].Should().Be(i);
    }


    [Fact]
    public void Merge_DeColeccionGrande_NoEsCuadratico()
    {
        var origen = MlErrorDetails.FromErrorMessage("base");

        var otros = Enumerable.Range(1, 1_000)
                              .Select(x => MlErrorDetails.FromErrorDetails($"e{x}", $"k{x}", x))
                              .ToList();

        MlErrorDetails fusion = origen.Merge(otros);

        fusion.Errors.Should().HaveCount(1_001);
        fusion.Metadata.Should().HaveCount(1_000);
    }


    #endregion


    #region Incorporacion de metadatos


    [Fact]
    public void AddMetadataRange_AgregaLosMetadatos()
    {
        var origen = MlErrorDetails.FromErrorMessage("error");

        MlErrorDetails resultado = origen.AddMetadataRange(("a", (object)1), ("b", (object)2));

        resultado.Metadata.Should().HaveCount(2);
        resultado.Metadata["a"].Should().Be(1);
        resultado.Metadata["b"].Should().Be(2);
        origen.Metadata.Should().BeEmpty();
    }


    [Fact]
    public void ToMlErrorsDetails_DeTuplaConDetalles_PropagaLosDetalles()
    {
        var detalles = new Dictionary<string, object> { { "a", 1 } };

        var resultado = (MlError.FromErrorMessage("error"), detalles).ToMlErrorsDetails();

        resultado.Metadata.Should().HaveCount(1);
        resultado.Metadata["a"].Should().Be(1);
    }


    [Fact]
    public void AddError_NoCompartenElDiccionarioDeMetadatos()
    {
        var origen = MlErrorDetails.FromErrorDetails("error", "a", 1);

        MlErrorDetails ampliado = origen.AddErrorMessage("otro");

        ampliado.Metadata.Should().NotBeSameAs(origen.Metadata);

        ampliado.Metadata["b"] = 2;

        origen.Metadata.Should().HaveCount(1);
    }


    [Fact]
    public void WithMetadata_NoModificaLaInstanciaDeOrigen()
    {
        var origen = MlErrorDetails.FromErrorMessage("error");

        var resultado = origen.WithMetadata("a", 1);

        resultado.Metadata.Should().HaveCount(1);
        origen.Metadata.Should().BeEmpty();
    }


    #endregion


    #region Accesores de lectura no destructivos


    [Fact]
    public void GetDetail_ConClaveAusente_NoModificaElOrigen()
    {
        var origen = MlErrorDetails.FromErrorMessage("error");

        var resultado = origen.GetDetail<int>("ausente");

        resultado.IsFailure.Should().BeTrue();
        origen.Errors.Should().HaveCount(1);
        origen.Metadata.Should().BeEmpty();
    }


    [Fact]
    public void GetDetail_LlamadoDosVeces_EsIdempotente()
    {
        var origen = MlErrorDetails.FromErrorMessage("error");

        var primera = origen.GetDetail<int>("ausente");
        var segunda = origen.GetDetail<int>("ausente");

        DetallesDe(primera).Errors.Count().Should().Be(DetallesDe(segunda).Errors.Count());
        origen.Errors.Should().HaveCount(1);
    }


    [Fact]
    public void GetDetail_ConTipoIncorrecto_NoModificaElOrigen()
    {
        var origen = MlErrorDetails.FromErrorDetails("error", "a", "texto");

        var resultado = origen.GetDetail<int>("a");

        resultado.IsFailure.Should().BeTrue();
        origen.Metadata.Should().HaveCount(1);
        origen.Metadata["a"].Should().Be("texto");
    }


    [Fact]
    public void GetDetail_ConClavePresente_DevuelveElValor()
    {
        var origen = MlErrorDetails.FromErrorDetails("error", "a", 7);

        var resultado = origen.GetDetail<int>("a");

        resultado.IsSuccess.Should().BeTrue();
        ValorDe(resultado).Should().Be(7);
    }


    [Fact]
    public void TryGetMetadata_ConClaveNula_DevuelveFalse()
    {
        var origen = MlErrorDetails.FromErrorDetails("error", "a", 7);

        origen.TryGetMetadata<int>(null!, out var valor).Should().BeFalse();
        valor.Should().Be(default);
    }


    [Fact]
    public void MetadataView_SinMetadatos_DevuelveLaInstanciaVacia()
    {
        var primero = MlErrorDetails.FromErrorMessage("uno");
        var segundo = MlErrorDetails.FromErrorMessage("dos");

        primero.MetadataView.Should().BeSameAs(segundo.MetadataView);
        primero.MetadataView.Should().BeEmpty();
    }


    [Fact]
    public void MetadataView_ConMetadatos_EsDeSoloLectura()
    {
        var origen = MlErrorDetails.FromErrorDetails("error", "a", 7);

        origen.MetadataView.Should().HaveCount(1);
        origen.MetadataView.Should().BeSameAs(origen.MetadataView);
    }


    #endregion


    #region Igualdad estructural


    [Fact]
    public void Equals_ConMismosErroresYMetadatos_DevuelveTrue()
    {
        var primero = MlErrorDetails.FromErrorDetails("error", "a", 1);
        var segundo = MlErrorDetails.FromErrorDetails("error", "a", 1);

        primero.Equals(segundo).Should().BeTrue();
        primero.GetHashCode().Should().Be(segundo.GetHashCode());
    }


    [Fact]
    public void Equals_ConMetadatosDistintos_DevuelveFalse()
    {
        var primero = MlErrorDetails.FromErrorDetails("error", "a", 1);
        var segundo = MlErrorDetails.FromErrorDetails("error", "a", 2);

        primero.Equals(segundo).Should().BeFalse();
    }


    #endregion


    #region Clave estandar de valor


    [Fact]
    public void BindSaveValueInDetailsIfFailedFuncResult_ConFuncionFallida_GuardaElValor()
    {
        MlResult<int> origen = 7;

        var resultado = origen.BindSaveValueInDetailsIfFailedFuncResult<int, string>(_ => "fallo".ToMlResultFail<string>());

        resultado.IsFailure.Should().BeTrue();
        ValorDe(DetallesDe(resultado).GetMetadataValue<int>()).Should().Be(7);
    }


    [Fact]
    public async Task BindSaveValueInDetailsIfFailedFuncResultAsync_ConFuncionFallida_GuardaElValor()
    {
        MlResult<int> origen = 7;

        var resultado = await origen.BindSaveValueInDetailsIfFailedFuncResultAsync<int, string>(_ => "fallo".ToMlResultFailAsync<string>());

        resultado.IsFailure.Should().BeTrue();
        ValorDe(DetallesDe(resultado).GetMetadataValue<int>()).Should().Be(7);
    }


    #endregion


    #region Materializacion de Errors


    [Fact]
    public void Errors_EnumeradoDosVeces_NoReejecutaLaProyeccion()
    {
        var veces   = 0;
        var mensajes = ContarEnumeraciones(new[] { "a", "b" }, () => veces++);

        var detalles = MlErrorDetails.FromEnumerableStrings(mensajes);

        _ = detalles.Errors.Count();
        _ = detalles.Errors.Count();

        veces.Should().Be(1);
    }


    [Fact]
    public void AddError_EnCadena_NoAcumulaIteradoresPerezosos()
    {
        var detalles = MlErrorDetails.FromErrorMessage("inicial");

        for (var i = 0; i < 100; i++) detalles = detalles.AddErrorMessage($"error {i}");

        detalles.Errors.Should().HaveCount(101);
        detalles.Errors.Should().BeOfType<List<MlError>>();
    }


    [Fact]
    public void ToString_EnumeraLosErroresUnaSolaVez()
    {
        var veces   = 0;
        var mensajes = ContarEnumeraciones(new[] { "a", "b" }, () => veces++);

        var detalles = MlErrorDetails.FromEnumerableStrings(mensajes);
        veces = 0;

        _ = detalles.ToString();

        veces.Should().Be(0);
    }


    [Fact]
    public void MlResult_DeFalloConMismoContenido_TienenLosMismosDetalles()
    {
        MlResult<int> primero = MlResult<int>.Fail(MlErrorDetails.FromErrorDetails("error", "a", 1));
        MlResult<int> segundo = MlResult<int>.Fail(MlErrorDetails.FromErrorDetails("error", "a", 1));

        DetallesDe(primero).Equals(DetallesDe(segundo)).Should().BeTrue();
    }


    #endregion


    #region Catalogo de claves


    [Fact]
    public void MetadataKeys_NingunaClaveDelCatalogoChocaConElPatronDeExcepcion()
    {
        var claves = new[]
        {
            MlErrorDetails.MetadataKeys.Value,
            MlErrorDetails.MetadataKeys.ParameterName,
            MlErrorDetails.MetadataKeys.Expected,
            MlErrorDetails.MetadataKeys.FailedIndexes,
            MlErrorDetails.MetadataKeys.NotFound,
            MlErrorDetails.MetadataKeys.ProblemsDetails,
        };

        claves.Should().OnlyContain(x => ! MlErrorDetails.IsExceptionKey(x));
        MlErrorDetails.IsExceptionKey(MlErrorDetails.MetadataKeys.Exception).Should().BeTrue();
    }


    [Fact]
    public void Constants_ReenvianALasClavesDelCatalogo()
    {
        Constants.ValueMetadataKey.Should().Be(MlErrorDetails.MetadataKeys.Value);
        Constants.ExceptionMetadataKey.Should().Be(MlErrorDetails.MetadataKeys.Exception);
    }


    #endregion


    #region Utilidades de enumeracion


    private static IEnumerable<string> ContarEnumeraciones(IEnumerable<string> source, Action onEnumerate)
    {
        onEnumerate();
        foreach (var item in source) yield return item;
    }


    #endregion

}
