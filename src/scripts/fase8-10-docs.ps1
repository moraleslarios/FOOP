$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
$skip = '^(UpdateProblemDetailsAsync|AllAsync|validMessageBuilder|_pkFields|ConvertDateTime|AddWebControllers)$'
$pairs = Get-Content (Join-Path $src 'Cambios Realizados\_registro.csv') -Encoding UTF8 |
    Where-Object { $_ -like 'fase8-10-niveles7-9;*' } |
    ForEach-Object { $c = $_.Split(';'); if ($c[2] -in 'Tipo', 'Interfaz', 'Metodo', 'Constante', 'Propiedad' -and $c[5] -notmatch $skip) { [pscustomobject]@{ Old = $c[5]; New = $c[6] } } } |
    Group-Object Old | ForEach-Object { $_.Group[0] } |
    Sort-Object { $_.Old.Length } -Descending
foreach ($p in $pairs) { & $r -Include $md -Pattern ('\b' + [regex]::Escape($p.Old) + '\b') -Replacement $p.New }

$app  = '^\\(?!MoralesLarios\.OOFP\.(Application|AspNetCore))'
& $r -Include $md -ExcludePath $app -Pattern '\bUpdateProblemDetailsAsync\b' -Replacement 'UpdateAsync'
& $r -Include $md -ExcludePath $app -Pattern '(?<=\.|`)AllAsync\b(?!\s*\(\s*\w+\s*=>)' -Replacement 'GetPagedAsync'
& $r -Include $md -Pattern '\bvalidMessageBuilder\b' -Replacement 'successMessageFactory'
& $r -Include $md -Pattern '\b_pkFields\b'           -Replacement '_keySelector'
& $r -Include $md -Pattern '\bConvertDateTime\b'     -Replacement 'ParseDateTimeInvariant'
& $r -Include $md -Pattern '\bAddWebControllers\b(?!Cache)' -Replacement 'AddMlControllers'
& $r -Include $md -Pattern '\bAddWebControllersCache\b'     -Replacement 'AddMlControllersOutputCaching'
& $r -Include $md -Pattern '\bGenServiceFp\b'        -Replacement 'CrudServiceFp'
& $r -Include $md -Pattern '\bIGenServiceFp\b'       -Replacement 'ICrudServiceFp'
& $r -Include $md -Pattern '\bAdd(Singleton|Transient)GenServices(Duplex)?FpWithoutReposGeneral\b' -Replacement 'Add$1CrudServices$2FpWithoutRepositories'
