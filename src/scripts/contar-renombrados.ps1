$ErrorActionPreference = 'Stop'

$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$terms = @(
    'Faild', 'Diferent', 'FusionFailErros', 'FusionErrosIfExists', 'Alwais',
    'TryMapIAsyncf', 'MlErrorsDetails', 'Lenght', 'Bydouble', 'Fromdouble',
    'ExistDirectory', 'Scopedt', 'TEnumrableResponse', 'BaseAdress',
    'soported', 'MlResultBucles', 'IsValid', 'IsFail', 'SecureValidValue',
    'EX_DESC_KEY', 'VALUE_KEY', 'ContieneCombinacion', 'EndpointPattern',
    'MlProblemsDetails', 'ToErrorsMessages', 'ToErrorsDescription',
    'HasKeyDetails', 'GetDetailValue', 'GetDetailException', 'AddDetail'
)

$files = Get-ChildItem -Path $root -Recurse -File -Include *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }

$cache = @{}
foreach ($f in $files) { $cache[$f.FullName] = [System.IO.File]::ReadAllText($f.FullName) }

$rows = @()

foreach ($term in $terms) {

    $rx    = [regex]::Escape($term)
    $lib   = 0
    $tests = 0
    $projs = @()

    foreach ($kv in $cache.GetEnumerator()) {

        $n = [regex]::Matches($kv.Value, $rx).Count
        if ($n -eq 0) { continue }

        $rel  = $kv.Key.Substring($root.Length + 1)
        $proj = ($rel -split '\\')[0]
        $projs += $proj

        if ($proj -match 'Tests?($|\.)') { $tests += $n } else { $lib += $n }
    }

    $rows += [pscustomobject]@{
        Termino   = $term
        Codigo    = $lib
        Tests     = $tests
        Ficheros  = ($projs | Sort-Object -Unique).Count
        Proyectos = (($projs | Sort-Object -Unique) -replace '^MoralesLarios\.(OOFP\.)?', '') -join ','
    }
}

$rows | Sort-Object -Property Codigo -Descending |
    Format-Table -AutoSize -Property Termino, Codigo, Tests, Proyectos
