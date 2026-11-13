using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MoralesLarios.OOFP.Types;

namespace MoralesLarios.OOFP.Core.Tests.Unit.Types;

public class MlResultCompilationTests
{
    [Fact]
    public void MlResultOfMlError_ImplicitConversionReportsAmbiguity()
    {
        const string source = """
            using MoralesLarios.OOFP.Types;
            using MoralesLarios.OOFP.Types.Errors;

            static class Probe
            {
                static MlResult<MlError> Convert(MlError error) => error;
            }
            """;

        var errors = CompileErrors(source);

        errors.Should().NotBeEmpty();
        errors.Should().OnlyContain(diagnostic => diagnostic.Id == "CS0457");
    }

    [Fact]
    public void TryMap_WithNullErrorMessageReportsAmbiguity()
    {
        const string source = """
            using MoralesLarios.OOFP.Types;

            static class Probe
            {
                static MlResult<int> Map(MlResult<int> source)
                    => source.TryMap(value => value + 1, null);
            }
            """;

        var errors = CompileErrors(source);

        errors.Should().ContainSingle(diagnostic => diagnostic.Id == "CS0121");
    }

    private static Diagnostic[] CompileErrors(string source)
    {
        var platformAssemblies = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = platformAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(MlResult<>).Assembly.Location));
        var compilation = CSharpCompilation.Create(
            assemblyName: "MlResultCompilationProbe",
            syntaxTrees: [CSharpSyntaxTree.ParseText(source)],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        return compilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ToArray();
    }
}