// Tests de la epica 04 (US-15 y US-16): fijan por reflexion los nombres nuevos de los parametros
// constructores de mensaje, de modo que un renombrado accidental vuelva a romper la suite.

using System.Reflection;
using MoralesLarios.OOFP.Logging;

namespace MoralesLarios.OOFP.Logging.Tests.Unit;

public class MlResultLogExtensionsNomenclaturaTests
{

    private static IEnumerable<ParameterInfo> TodosLosParametros()
        => typeof(MlResultLogExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                        .SelectMany(m => m.GetParameters());


    [Fact]
    public void Ningun_parametro_publico_se_llama_validBuilMessage()
    {
        var erratas = TodosLosParametros().Where(p => p.Name == "validBuilMessage").ToList();

        erratas.Should().BeEmpty();
    }


    [Fact]
    public void Ningun_parametro_publico_se_llama_faildMessage()
    {
        var erratas = TodosLosParametros().Where(p => p.Name == "faildMessage").ToList();

        erratas.Should().BeEmpty();
    }


    [Fact]
    public void Existen_parametros_con_los_nombres_nuevos()
    {
        var nombres = TodosLosParametros().Select(p => p.Name).Distinct().ToList();

        nombres.Should().Contain("validBuildMessage");
        nombres.Should().Contain("failBuildMessage");
    }


    [Fact]
    public void Un_mismo_convenio_rige_todos_los_constructores_de_mensaje()
    {
        var sospechosos = TodosLosParametros()
            .Where(p => p.Name is not null
                     && p.Name.Contains("Message", StringComparison.Ordinal)
                     && p.ParameterType.Name.StartsWith("Func", StringComparison.Ordinal))
            .Select(p => p.Name!)
            .Distinct()
            .ToList();

        sospechosos.Should().OnlyContain(n => n == "validBuildMessage" || n == "failBuildMessage");
    }


    #pragma warning disable CS0618 // se inspecciona a proposito la clase obsoleta
    [Fact]
    public void La_clase_antigua_Extensions_esta_marcada_como_obsoleta()
    {
        var atributo = typeof(Extensions).GetCustomAttribute<ObsoleteAttribute>();

        atributo.Should().NotBeNull();
        atributo!.Message.Should().Contain("MlResultLogActionExtensions");
        atributo!.IsError.Should().BeFalse();
    }
#pragma warning restore CS0618

    [Fact]
    public void La_clase_nueva_MlResultLogActionExtensions_no_esta_obsoleta()
    {
        typeof(MlResultLogActionExtensions).GetCustomAttribute<ObsoleteAttribute>().Should().BeNull();
    }

}
