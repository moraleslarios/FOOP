$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
function Load([string]$rel) {
    $p = Join-Path $src $rel
    $b = [System.IO.File]::ReadAllBytes($p)
    return @{ Path = $p; Text = [System.IO.File]::ReadAllText($p); Bom = ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) }
}
function Save($f) { [System.IO.File]::WriteAllText($f.Path, $f.Text, (New-Object System.Text.UTF8Encoding($f.Bom))) }
function ReplaceNth($f, [string]$literal, [int]$nth, [string]$new) {
    $ms = [regex]::Matches($f.Text, [regex]::Escape($literal))
    if ($ms.Count -lt $nth) { throw "No hay ocurrencia $nth de $literal" }
    $m = $ms[$nth - 1]
    $f.Text = $f.Text.Substring(0, $m.Index) + $new + $f.Text.Substring($m.Index + $m.Length)
}

$t = Load 'MoralesLarios.OOFP.Http.Tests.Unit\RegisterServicesTests.cs'
if ([regex]::Matches($t.Text, 'AddRestClient_uses_custom_key_when_configured').Count -ne 2) { throw 'uses_custom: se esperaban 2' }
if ([regex]::Matches($t.Text, 'AddRestClient_throws_when_key_is_null').Count -ne 2) { throw 'throws: se esperaban 2' }
ReplaceNth $t 'AddRestClient_throws_when_key_is_null' 2 'AddCompositeKeyRestClient_duplex_throws_when_key_is_null'
ReplaceNth $t 'AddRestClient_throws_when_key_is_null' 1 'AddCompositeKeyRestClient_throws_when_key_is_null'
ReplaceNth $t 'AddRestClient_uses_custom_key_when_configured' 2 'AddCompositeKeyRestClient_uses_custom_key_when_configured'
Save $t

$e = Load 'MoralesLarios.OOFP.Core\Types\Errors\MlErrorDetailsExtensions.cs'
if ($e.Text -notmatch 'The MlErrorsDetails does not have details\.') { throw 'mensaje no encontrado' }
$e.Text = $e.Text.Replace('The MlErrorsDetails does not have details.', 'The MlErrorDetails does not have metadata.')
Save $e

$i = Load 'MoralesLarios.OOFP.ValueObjects\Id.cs'
$i.Text = [regex]::Replace($i.Text, '\bMlErrorsDetails\b', 'MlErrorDetails')
Save $i
Write-Output 'OK'
