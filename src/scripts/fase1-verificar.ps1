$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$pairs = @(
    @('MoralesLarios.FOOP/__Doc/Types/', 'docs/core-types/'),
    @('MoralesLarios.FOOP/__Doc/Bucle/', 'docs/loops/'),
    @('MoralesLarios.FOOP/__Doc/PendingTasks.txt', 'Temas Pendientes/PendingTasks-legacy.md'),
    @('MoralesLarios.FOOP/__Doc/', 'docs/'),
    @('MoralesLarios.FOOP/MoralesLarios.OOFP.csproj', 'MoralesLarios.OOFP.Core/MoralesLarios.OOFP.Core.csproj'),
    @('MoralesLarios.FOOP/', 'MoralesLarios.OOFP.Core/'),
    @('MoralesLarios.OOFP.EFCore/OopRepos/', 'MoralesLarios.OOFP.EntityFrameworkCore/Repositories.Imperative/'),
    @('MoralesLarios.OOFP.EFCore/Repos/', 'MoralesLarios.OOFP.EntityFrameworkCore/Repositories/'),
    @('MoralesLarios.OOFP.HttpClients/ParamsInfo/', 'MoralesLarios.OOFP.Http/Requests/'),
    @('MoralesLarios.OOFP.Internals/Info/', 'MoralesLarios.OOFP.Shared/Pagination/'),
    @('MoralesLarios.OOFP.Internals/GlobalUsings.cs', 'MoralesLarios.OOFP.Shared/GlobalUsings.cs')
)
$projects = @(
    'MoralesLarios.OOFP.ValueObjects.IO.Test.Unit|MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit',
    'MoralesLarios.OOFP.ValueObjects.IO|MoralesLarios.OOFP.ValueObjects.FileSystem',
    'MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit|MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit',
    'MoralesLarios.OOFP.Validation.Dataannotations|MoralesLarios.OOFP.Validation.DataAnnotations',
    'MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit|MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit',
    'MoralesLarios.OOFP.Validation.FluentValidations|MoralesLarios.OOFP.Validation.FluentValidation',
    'MoralesLarios.OOFP.WebControllers.Cache|MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching',
    'MoralesLarios.OOFP.WebControllers|MoralesLarios.OOFP.AspNetCore.Controllers',
    'MoralesLarios.OOFP.WebApi.Tests.Unit|MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit',
    'MoralesLarios.OOFP.WebApi|MoralesLarios.OOFP.AspNetCore.Results',
    'MoralesLarios.OOFP.WebServices.Tests.Unit|MoralesLarios.OOFP.Application.Tests.Unit',
    'MoralesLarios.OOFP.WebServices|MoralesLarios.OOFP.Application',
    'MoralesLarios.OOFP.HttpClients.Tests.Integration|MoralesLarios.OOFP.Http.Tests.Integration',
    'MoralesLarios.OOFP.HttpClients.Tests.Unit|MoralesLarios.OOFP.Http.Tests.Unit',
    'MoralesLarios.OOFP.HttpClients|MoralesLarios.OOFP.Http',
    'MoralesLarios.OOFP.EFCore.Infrastructure.Tests|MoralesLarios.OOFP.EntityFrameworkCore.TestSupport',
    'MoralesLarios.OOFP.EFCore.Integration.Tests|MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration',
    'MoralesLarios.OOFP.EFCore.WebApi|MoralesLarios.OOFP.EntityFrameworkCore.Sample',
    'MoralesLarios.OOFP.EFCore|MoralesLarios.OOFP.EntityFrameworkCore',
    'MoralesLarios.OOFP.Extensions.Loggers.Console.Tests|MoralesLarios.OOFP.Logging.Sample',
    'MoralesLarios.OOFP.Extensions.Loggers|MoralesLarios.OOFP.Logging',
    'MoralesLarios.OOFP.Utilities.Tests.Unit|MoralesLarios.OOFP.Configuration.Tests.Unit',
    'MoralesLarios.OOFP.Utilities|MoralesLarios.OOFP.Configuration',
    'MoralesLarios.OOFP.IO|MoralesLarios.OOFP.FileSystem',
    'MoralesLarios.OOFP.Unit.Tests|MoralesLarios.OOFP.Core.Tests.Unit'
)
foreach ($p in $projects) {
    $o, $n = $p.Split('|')
    $pairs += ,@("$o/$o.csproj", "$n/$n.csproj")
    $pairs += ,@("$o/", "$n/")
}
$intencionados = 'MoralesLarios\.FOOP\.sln$|copia\.sln$|ValueObjects\.IO\.2\.Tests\.Unit/|EFCore/Helpers/Constants\.cs$|Services/GenService\.cs$|OOFP\.Internals/(README\.md|MoralesLarios\.OOFP\.Internals\.csproj)$'

$missing = @()
$all = @(& git.exe ls-tree -r --name-only HEAD)
if ($all.Count -lt 100) { throw "ls-tree devolvio solo $($all.Count) rutas" }
foreach ($rel in $all) {
    if ($rel -match '^(scripts|Cambios Realizados)/') { continue }
    if ($rel -match $intencionados) { continue }
    $new = $rel
    foreach ($pp in $pairs) {
        if ($rel.StartsWith($pp[0], [StringComparison]::Ordinal)) { $new = $pp[1] + $rel.Substring($pp[0].Length); break }
    }
    $full = Join-Path (Get-Location) $new
    $dir  = Split-Path $full -Parent
    $ok = $false
    if (Test-Path -LiteralPath $dir) {
        $leaf = Split-Path $full -Leaf
        $ok = @(Get-ChildItem -LiteralPath $dir -File | Where-Object { $_.Name -ceq $leaf }).Count -eq 1
    }
    if (-not $ok) { $missing += "$rel  ->  $new" }
}
Write-Output ("Ficheros que faltan: {0}" -f $missing.Count)
$missing
