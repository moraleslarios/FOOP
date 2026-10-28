$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Slug($t) {
    $s = $t
    $s = [regex]::Replace($s, '\[([^\]]*)\]\([^)]*\)', '$1')
    $s = $s -replace '`', ''
    $s = $s -replace '\*', ''
    $s = $s -replace '_', ''
    $s = $s.ToLowerInvariant()
    $s = [regex]::Replace($s, "[^\p{L}\p{Nd} \-]", '')
    $s = $s -replace ' ', '-'
    return $s
}

$rels = @(
  'MoralesLarios.OOFP.AspNetCore.Controllers\README.md',
  'MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching\README.md',
  'MoralesLarios.OOFP.Http\README.md',
  'MoralesLarios.OOFP.Configuration\README.md',
  'MoralesLarios.OOFP.EntityFrameworkCore\README.md'
)

$out = New-Object System.Collections.Generic.List[string]
foreach ($rel in $rels) {
    $p = Join-Path $src $rel
    $out.Add('=== ' + $rel)
    $ls = [System.IO.File]::ReadAllLines($p, [System.Text.Encoding]::UTF8)
    $enCodigo = $false
    for ($i = 0; $i -lt $ls.Count; $i++) {
        $l = $ls[$i]
        if ($l -match '^\s*```') { $enCodigo = -not $enCodigo; continue }
        if ($enCodigo) { continue }
        if ($l -match '^(#{1,3})\s+(.*)$') {
            $out.Add('L' + ($i + 1) + ' :: ' + $Matches[2].TrimEnd() + ' :: ' + (Slug $Matches[2].TrimEnd()))
        }
    }
}

$p2 = Join-Path $PSScriptRoot '_headings.log'
[System.IO.File]::WriteAllLines($p2, $out, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('total = ' + $out.Count)
