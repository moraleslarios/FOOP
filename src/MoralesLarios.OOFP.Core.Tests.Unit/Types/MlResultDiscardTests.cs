// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

/// <summary>
/// US-12 · `Discard` es la implementación canónica de «resultado válido sin valor relevante»
/// y `_` queda únicamente como puente [Obsolete] que reenvía a `Discard`.
/// </summary>
public class MlResultDiscardTests
{

    [Fact]
    public void MlResult_Discard_EsValido()
    {
        var result = MlResult.Discard;

        result.Should().NotBeNull();
        result.Should().BeOfType<MlResult>();
    }



    [Fact]
    public void MlResult_Guion_DelegaEnDiscard()
    {
#pragma warning disable CS0618 // El puente se comprueba a propósito.
        var guion = MlResult._;
#pragma warning restore CS0618

        var discard = MlResult.Discard;

        guion.Should().Be(discard);
    }



    [Fact]
    public void MlResult_Guion_EstaObsoleto()
    {
        var member = typeof(MlResult).GetProperty("_", BindingFlags.Public | BindingFlags.Static);

        member.Should().NotBeNull();

        var obsolete = member!.GetCustomAttribute<ObsoleteAttribute>();

        obsolete.Should().NotBeNull();
        obsolete!.Message.Should().Be("Renamed to Discard. Will be removed in 3.0.0.");
        obsolete.IsError.Should().BeFalse();
    }



    /// <summary>
    /// D-03 / D-12 · `Empty()` devuelve <c>MlResult&lt;object&gt;</c>, no <c>MlResult</c>, así que no es
    /// un alias de `Discard`. Su retirada se aplaza a la 3.0.0, cuando exista un tipo `Unit`. Este test
    /// fija la decisión para que nadie los marque obsoletos en la 2.1.x por descuido.
    /// </summary>
    [Fact]
    public void MlResult_Empty_NoEstaObsoletoEnLa211()
    {
        var empty      = typeof(MlResult).GetMethod(nameof(MlResult.Empty)     , BindingFlags.Public | BindingFlags.Static);
        var emptyAsync = typeof(MlResult).GetMethod(nameof(MlResult.EmptyAsync), BindingFlags.Public | BindingFlags.Static);

        empty     .Should().NotBeNull();
        emptyAsync.Should().NotBeNull();

        empty     !.GetCustomAttribute<ObsoleteAttribute>().Should().BeNull();
        emptyAsync!.GetCustomAttribute<ObsoleteAttribute>().Should().BeNull();
    }

}
