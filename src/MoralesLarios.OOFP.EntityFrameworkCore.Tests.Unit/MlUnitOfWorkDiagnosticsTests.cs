namespace MoralesLarios.OOFP.EntityFrameworkCore.Tests.Unit;

public class MlUnitOfWorkDiagnosticsTests
{
    [Fact]
    public void WarnOnUncommittedChanges_DefaultsToFalse()
    {
        var diagnostics = new MlUnitOfWorkDiagnostics();

        diagnostics.WarnOnUncommittedChanges.Should().BeFalse();
    }

    [Fact]
    public void WarnOnUncommittedChanges_CanBeEnabled()
    {
        var diagnostics = new MlUnitOfWorkDiagnostics
        {
            WarnOnUncommittedChanges = true
        };

        diagnostics.WarnOnUncommittedChanges.Should().BeTrue();
    }
}