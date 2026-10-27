$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$p = Join-Path $src 'MoralesLarios.OOFP.AspNetCore.Controllers\Attributes\CompositeKeyParameterAttribute.cs'
$enc1252 = [System.Text.Encoding]::GetEncoding(1252)
$t = [System.IO.File]::ReadAllText($p, $enc1252)
$linesBefore = ($t -split "`n").Count

$pairs = @(
    @('public string Description { get; set; }', 'public string? Description { get; set; }'),
    @('public CompositeKeyParameterAttribute(string description = null!)', 'public CompositeKeyParameterAttribute(string? description = null)')
)
foreach ($pair in $pairs) {
    $n = [regex]::Matches($t, [regex]::Escape($pair[0])).Count
    if ($n -ne 1) { throw ('COINCIDENCIAS=' + $n + ' para ' + $pair[0]) }
    $t = $t.Replace($pair[0], $pair[1])
}

$linesAfter = ($t -split "`n").Count
if ($linesBefore -ne $linesAfter) { throw 'CAMBIO DE LINEAS' }
[System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($true)))
Write-Output 'OK C14 + encoding reparado'
Write-Output ([regex]::Matches($t, 'Descripci\u00f3n').Count)
