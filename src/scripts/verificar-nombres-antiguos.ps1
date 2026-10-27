$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
# Homonimos legitimos que no son el simbolo renombrado (Name, Value, string.Empty, LINQ OrderBy, etc.)
$homonimos = '^(Key|Name|Empty|OrderBy|Value|Details|Do|DoAsync|Combine|CombineAsync|IsValid|AllAsync|limit|pk|lenght|BuildErrorMessage|FromString|FromInt|FromDecimal|FromDouble|FromRegex|FromEnum|ValidateObject|ErrorsDetails|ConfigureHttpClientKey)$'
$rows = Get-Content (Join-Path $src 'Cambios Realizados\_registro.csv') -Encoding UTF8 | Select-Object -Skip 1 |
    ForEach-Object { $c = $_.Split(';'); if ($c.Count -gt 6 -and $c[2] -in 'Tipo', 'Interfaz', 'Record', 'Enum', 'Metodo', 'Propiedad', 'Constante') { [pscustomobject]@{ Lote = $c[0]; Tipo = $c[2]; Old = $c[5]; New = $c[6] } } } |
    Where-Object { $_.Old -notmatch $homonimos -and $_.Old -match '^\w+$' } |
    Group-Object Old | ForEach-Object { $_.Group[0] }
$files = Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' -and $_.Name -notlike '*.Obsolete.cs' }
$texts = @{}; foreach ($f in $files) { $texts[$f.FullName] = [System.IO.File]::ReadAllText($f.FullName) }
$found = 0
foreach ($r in $rows) {
    $rx = '(?<![\w"])' + [regex]::Escape($r.Old) + '(?![\w"])'
    $hits = foreach ($k in $texts.Keys) { $n = [regex]::Matches($texts[$k], $rx).Count; if ($n) { '{0}({1})' -f (Split-Path $k -Leaf), $n } }
    if ($hits) { $found++; '{0,-10} {1,-40} -> {2,-40} {3}' -f $r.Tipo, $r.Old, $r.New, ($hits -join ' ') }
}
"Nombres antiguos aun presentes en el codigo (fuera de *.Obsolete.cs): $found de $(@($rows).Count) comprobados"
