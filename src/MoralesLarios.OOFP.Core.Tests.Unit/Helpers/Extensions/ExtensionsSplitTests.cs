// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using LegacyExtensions = MoralesLarios.OOFP.Helpers.Extensions.Extensions;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Helpers.Extensions;

/// <summary>
/// Pruebas de la US-11: el troceado de la antigua clase <c>Extensions</c> en cuatro clases
/// cohesionadas no debe romper ninguna invocación en forma de extensión, y la forma estática
/// debe seguir disponible mediante puentes marcados como obsoletos.
/// </summary>
public class ExtensionsSplitTests
{

    private static readonly Type[] ClasesDeExtension =
    [
        typeof(NullableExtensions),
        typeof(ExceptionMetadataExtensions),
        typeof(MutationExtensions),
        typeof(TaskLiftExtensions)
    ];


    private sealed class Caja
    {
        public int Numero { get; set; }
        public string Texto { get; set; } = string.Empty;
    }


    [Fact]
    public async Task Extensiones_InvocacionComoExtension_SigueFuncionando()
    {
        // ToNullable -> NullableExtensions
        int? nullable = 7.ToNullable();
        nullable.Should().Be(7);

        // AppendExDetails -> ExceptionMetadataExtensions
        var metadatos = new Dictionary<string, object>().AppendExDetails(new InvalidOperationException("boom"));
        metadatos.Should().ContainKey(MlErrorDetails.MetadataKeys.Exception);

        // With / WithAsync -> MutationExtensions
        var caja = new Caja().With(x => x.Numero = 3, x => x.Texto = "tres");
        caja.Numero.Should().Be(3);
        caja.Texto.Should().Be("tres");

        var cajaAsync = await new Caja().WithAsync(x => x.Numero = 4);
        cajaAsync.Numero.Should().Be(4);

        var cajaDesdeTarea = await Task.FromResult(new Caja()).WithAsync(x => x.Numero = 5);
        cajaDesdeTarea.Numero.Should().Be(5);

        // VoidToAsync / ToFuncTask -> TaskLiftExtensions
        var contador = 0;
        await 1.VoidToAsync(x => contador += x);
        contador.Should().Be(1);

        Func<int, int> duplicar = x => x * 2;
        (await duplicar.ToFuncTask()(21)).Should().Be(42);

        Action<int> acumular = x => contador += x;
        await acumular.ToFuncTask()(9);
        contador.Should().Be(10);

        Action simple = () => contador++;
        await simple.ToFuncTask()();
        contador.Should().Be(11);
    }


    [Fact]
    public void Extensions_PuentesEstaticos_EstanObsoletos()
    {
        var puentes = typeof(LegacyExtensions)
                            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        puentes.Should().NotBeEmpty("la clase de compatibilidad debe conservar la forma estatica de los metodos movidos");

        foreach (var puente in puentes)
        {
            var obsoleto = puente.GetCustomAttribute<ObsoleteAttribute>();

            obsoleto.Should().NotBeNull($"el puente '{puente.Name}' debe estar marcado como obsoleto");
            obsoleto!.IsError.Should().BeFalse($"el puente '{puente.Name}' debe avisar, no romper la compilacion");
            obsoleto.Message.Should().Contain("3.0.0", $"el puente '{puente.Name}' debe indicar cuando desaparece");

            puente.IsDefined(typeof(ExtensionAttribute), inherit: false)
                  .Should().BeFalse($"el puente '{puente.Name}' no debe ser metodo de extension, para no duplicar la resolucion");
        }
    }


    [Fact]
    public void ClasesDeExtension_TienenSummary()
    {
        var rutaXml = Path.ChangeExtension(typeof(NullableExtensions).Assembly.Location, ".xml");

        File.Exists(rutaXml).Should().BeTrue($"se esperaba el fichero de documentacion XML en '{rutaXml}'");

        var miembros = XDocument.Load(rutaXml)
                                    .Descendants("member")
                                    .ToDictionary(x => x.Attribute("name")!.Value, x => x);

        foreach (var clase in ClasesDeExtension)
        {
            var clave = $"T:{clase.FullName}";

            miembros.Should().ContainKey(clave, $"la clase '{clase.Name}' debe estar documentada");

            var summary = miembros[clave].Element("summary");

            summary.Should().NotBeNull($"la clase '{clase.Name}' debe tener su propio <summary>");
            summary!.Value.Trim().Should().NotBeEmpty($"el <summary> de '{clase.Name}' no puede estar vacio");
        }
    }

}
