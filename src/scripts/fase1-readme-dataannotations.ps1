$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$path = Join-Path $src 'MoralesLarios.OOFP.Validation.DataAnnotations\README.md'
$b = [System.IO.File]::ReadAllBytes($path)
$bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
$t = [System.IO.File]::ReadAllText($path)
$lines = ([regex]::Matches($t, "`n")).Count

$rules = @(
    @('\.\./MoralesLarios\.FOOP/__Doc/Types/', '../docs/core-types/'),
    @('\.\./MoralesLarios\.FOOP/__Doc/Bucle/', '../docs/loops/'),
    @('\.\./MoralesLarios\.FOOP/__Doc/',       '../docs/'),
    @('\.\./MoralesLarios\.FOOP/',             '../MoralesLarios.OOFP.Core/'),
    @('(?<![/\w.])MoralesLarios\.FOOP[/\\]__Doc', 'docs'),
    @('(?<![/\w.])__Doc(?![\w])', 'docs'),
    @('MoralesLarios\.OOFP\.Validation\.FluentValidations(?![\w])', 'MoralesLarios.OOFP.Validation.FluentValidation'),
    @('MoralesLarios\.OOFP\.WebControllers\.Cache(?![\w])', 'MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching'),
    @('MoralesLarios\.OOFP\.WebControllers(?![\w])', 'MoralesLarios.OOFP.AspNetCore.Controllers'),
    @('MoralesLarios\.OOFP\.WebApi(?![\w])', 'MoralesLarios.OOFP.AspNetCore.Results'),
    @('MoralesLarios\.OOFP\.WebServices(?![\w])', 'MoralesLarios.OOFP.Application'),
    @('MoralesLarios\.OOFP\.EFCore(?![\w])', 'MoralesLarios.OOFP.EntityFrameworkCore'),
    @('MoralesLarios\.OOFP\.Extensions\.Loggers(?![\w])', 'MoralesLarios.OOFP.Logging'),
    @('MoralesLarios\.OOFP\.Utilities(?![\w])', 'MoralesLarios.OOFP.Configuration'),
    @('MoralesLarios\.OOFP\.ValueObjects\.IO(?![\w])', 'MoralesLarios.OOFP.ValueObjects.FileSystem'),
    @('MoralesLarios\.OOFP\.IO(?![\w])', 'MoralesLarios.OOFP.FileSystem')
)
foreach ($r in $rules) { $t = [regex]::Replace($t, $r[0], $r[1]) }
if (([regex]::Matches($t, "`n")).Count -ne $lines) { throw 'Cambio de lineas' }
[System.IO.File]::WriteAllText($path, $t, (New-Object System.Text.UTF8Encoding($bom)))
Write-Output 'README de DataAnnotations actualizado.'
