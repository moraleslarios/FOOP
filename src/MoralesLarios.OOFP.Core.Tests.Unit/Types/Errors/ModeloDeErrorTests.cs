// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Reflection;
using MoralesLarios.OOFP.Types.Errors;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types.Errors;

/// <summary>
/// Pruebas del modelo de error: comprueban que el tipo heredado <c>ErrorMessage</c> está retirado
/// y que el espacio de nombres sólo expone los tipos canónicos.
/// </summary>
public class ModeloDeErrorTests
{
    [Fact]
    public void ErrorMessage_EstaMarcadoObsoleto()
    {
#pragma warning disable CS0618 // el propósito del test es inspeccionar el tipo obsoleto
        var tipo = typeof(ErrorMessage);
#pragma warning restore CS0618

        var obsoleto = tipo.GetCustomAttribute<ObsoleteAttribute>();

        obsoleto.Should().NotBeNull();
        obsoleto!.Message.Should().Be("Use MlError instead. Will be removed in 3.0.0.");
        obsoleto.IsError.Should().BeFalse();

        var navegable = tipo.GetCustomAttribute<EditorBrowsableAttribute>();

        navegable.Should().NotBeNull();
        navegable!.State.Should().Be(EditorBrowsableState.Never);
    }

    [Fact]
    public void ModeloDeError_TiposPublicos_SonLosEsperados()
    {
        var esperados = new[]
        {
            nameof(MlError),
            nameof(MlErrorExtensions),
            nameof(MlErrorDetails),
            nameof(MlErrorDetailsExtensions),
            nameof(MlErrorDetailsActions)
        };

        var actuales = typeof(MlError).Assembly
                                      .GetExportedTypes()
                                      .Where(t => t.Namespace == typeof(MlError).Namespace)
                                      .Where(t => !t.IsNested)
                                      .Where(t => t.GetCustomAttribute<ObsoleteAttribute>() is null)
                                      .Select(t => t.Name)
                                      .OrderBy(n => n)
                                      .ToArray();

        actuales.Should().BeEquivalentTo(esperados);
    }
}
