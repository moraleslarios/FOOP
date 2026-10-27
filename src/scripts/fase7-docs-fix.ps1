$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
# Deshacer LINQ: .SortDirection( -> .OrderBy(  y la mencion al bug de LINQ
& $r -Include $md -Pattern '\.SortDirection(\s*\()'            -Replacement '.OrderBy$1'
& $r -Include $md -Pattern 'bug del `SortDirection` tras `Skip`' -Replacement 'bug del `OrderBy` tras `Skip`'
# Pares que se saltaron por deduplicar por longitud
$pairs = Get-Content (Join-Path $src 'Cambios Realizados\_registro.csv') -Encoding UTF8 |
    Where-Object { $_ -like 'fase7-nivel6-efcore;*' } |
    ForEach-Object { $c = $_.Split(';'); if ($c[2] -in 'Tipo', 'Interfaz', 'Metodo') { [pscustomobject]@{ Old = $c[5]; New = $c[6] } } } |
    Group-Object Old | ForEach-Object { $_.Group[0] } |
    Sort-Object { $_.Old.Length } -Descending
foreach ($p in $pairs) { & $r -Include $md -Pattern ('\b' + [regex]::Escape($p.Old) + '\b') -Replacement $p.New }
