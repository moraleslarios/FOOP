$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
# Pares sacados del registro de la Fase 7 (tipos), del mas largo al mas corto.
$pairs = Get-Content (Join-Path $src 'Cambios Realizados\_registro.csv') -Encoding UTF8 |
    Where-Object { $_ -like 'fase7-nivel6-efcore;*' } |
    ForEach-Object { $c = $_.Split(';'); if ($c[2] -in 'Tipo', 'Interfaz', 'Enum', 'Metodo') { ,@($c[5], $c[6]) } } |
    Sort-Object { $_[0].Length } -Descending -Unique
foreach ($p in $pairs) { & $r -Include $md -Pattern ('\b' + [regex]::Escape($p[0]) + '\b') -Replacement $p[1] }
& $r -Include $md -Pattern '\bOrderBy\.(Asc|Desc)\b' -Replacement 'SortDirection.$1ending'
& $r -Include $md -Pattern '(?<![.\w])OrderBy(?=\s+\w+\s*[,)=])' -Replacement 'SortDirection' -ExcludePath '^\\(?!MoralesLarios\.OOFP\.EntityFrameworkCore)'
& $r -Include $md -Pattern '`OrderBy`' -Replacement '`SortDirection`' -ExcludePath '^\\(?!MoralesLarios\.OOFP\.EntityFrameworkCore)'
& $r -Include $md -Pattern '\bparams object\[\] pk\b' -Replacement 'params object[] keyValues'
