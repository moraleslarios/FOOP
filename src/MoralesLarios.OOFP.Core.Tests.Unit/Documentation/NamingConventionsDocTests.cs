// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Xml.Linq;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Documentation;

/// <summary>
/// Pruebas de los convenios de nomenclatura documentados en US-16: la semántica propia de la
/// familia <c>Try*</c> y la del sufijo <c>Async</c>.
/// </summary>
public class NamingConventionsDocTests
{

    private const string XmlDocFileName = "MoralesLarios.OOFP.Core.xml";

    private static readonly string[] FamiliesWithTryMembers =
    [
        "T:MoralesLarios.OOFP.Helpers.EnsureFp",
        "T:MoralesLarios.OOFP.Types.MlResultBindExtensions",
        "T:MoralesLarios.OOFP.Types.MlResultConversions",
        "T:MoralesLarios.OOFP.Types.MlResultExecSelfExtensions",
        "T:MoralesLarios.OOFP.Types.MlResultMapExtensions",
        "T:MoralesLarios.OOFP.Types.MlResultMatchExtensions"
    ];



    #region Sufijo Async sin asincronia



    [Fact]
    public void BindAsync_SinAsincronia_DevuelveTareaCompletada()
    {
        MlResult<int> source = 1;

        var task = source.BindAsync(x => MlResult<int>.Valid(x + 1));

        task.IsCompleted.Should().BeTrue();
    }



    [Fact]
    public void MapAsync_SinAsincronia_DevuelveTareaCompletada()
    {
        MlResult<int> source = 1;

        var task = source.MapAsync(x => x + 1);

        task.IsCompleted.Should().BeTrue();
    }



    [Fact]
    public void ValidAsync_SinAsincronia_DevuelveTareaCompletada()
    {
        var task = MlResult<int>.ValidAsync(1);

        task.IsCompleted.Should().BeTrue();
    }



    #endregion



    #region Remarks de la familia Try



    [Fact]
    public void Documentacion_FamiliaTry_TieneRemarks()
    {
        var members = LoadXmlDocMembers();

        foreach (var familyName in FamiliesWithTryMembers)
        {
            var remarks = members.TryGetValue(familyName, out var member)
                              ? member.Element("remarks")?.Value
                              : null;

            remarks.Should().NotBeNullOrWhiteSpace($"la familia {familyName} debe documentar la semántica propia de Try*");
            remarks!.Should().Contain("Try",      $"el <remarks> de {familyName} debe mencionar la familia Try*");
            remarks!.Should().Contain("try/catch", $"el <remarks> de {familyName} debe explicar que la excepción se convierte en fallo");
            remarks!.Should().Contain("BCL",      $"el <remarks> de {familyName} debe contrastar la semántica con la de la BCL");
        }
    }



    [Fact]
    public void Documentacion_SufijoAsync_SeExplicaEnCadaFamilia()
    {
        var members = LoadXmlDocMembers();

        foreach (var familyName in FamiliesWithTryMembers)
        {
            var remarks = members.TryGetValue(familyName, out var member)
                              ? member.Element("remarks")?.Value
                              : null;

            remarks.Should().NotBeNullOrWhiteSpace();
            remarks!.Should().Contain("Async",     $"el <remarks> de {familyName} debe explicar el sufijo Async");
            remarks!.Should().Contain("completada", $"el <remarks> de {familyName} debe aclarar que la tarea ya está completada");
        }
    }



    [Fact]
    public void Documentacion_TryGetValue_Y_TryGetErrors_EstanDocumentados()
    {
        var members = LoadXmlDocMembers();

        members.Should().ContainKey("M:MoralesLarios.OOFP.Types.MlResult`1.TryGetValue(`0@)");
        members.Should().ContainKey("M:MoralesLarios.OOFP.Types.MlResult`1.TryGetErrors(MoralesLarios.OOFP.Types.Errors.MlErrorDetails@)");
    }



    #endregion



    #region Helpers



    private static Dictionary<string, XElement> LoadXmlDocMembers()
    {
        var path = Path.Combine(AppContext.BaseDirectory, XmlDocFileName);

        File.Exists(path).Should().BeTrue($"el fichero de documentación XML '{XmlDocFileName}' debe generarse y copiarse al directorio de salida");

        return XDocument.Load(path)
                        .Descendants("member")
                        .Where (x => x.Attribute("name") is not null)
                        .GroupBy(x => x.Attribute("name")!.Value)
                        .ToDictionary(g => g.Key, g => g.First());
    }



    #endregion



}
