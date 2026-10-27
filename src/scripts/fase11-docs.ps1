$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
$skip = '^(pk|K|httpClientFactoryKey|configureHttpClientKey)$'
$pairs = Get-Content (Join-Path $src 'Cambios Realizados\_registro.csv') -Encoding UTF8 |
    Where-Object { $_ -like 'fase11*' } |
    ForEach-Object { $c = $_.Split(';'); if ($c[2] -in 'Tipo', 'Interfaz', 'Record', 'Metodo' -and $c[5] -notmatch $skip) { [pscustomobject]@{ Old = $c[5]; New = $c[6] } } } |
    Group-Object Old | ForEach-Object { $_.Group[0] } |
    Sort-Object { $_.Old.Length } -Descending
foreach ($p in $pairs) { & $r -Include $md -Pattern ('\b' + [regex]::Escape($p.Old) + '\b') -Replacement $p.New }
$http = '^\\(?!MoralesLarios\.OOFP\.Http)'
& $r -Include $md -ExcludePath $http -Pattern '\bhttpClientFactoryKey\b'   -Replacement 'clientName'
& $r -Include $md -ExcludePath $http -Pattern '\bconfigureHttpClientKey\b' -Replacement 'clientNameFactory'
& $r -Include $md -Pattern '\bAddGenClient(Duplex)?ComplexFp\b' -Replacement 'AddCompositeKeyRestClient'
& $r -Include $md -Pattern '\bAddGenClientFp\b'                 -Replacement 'AddRestClient'
& $r -Include $md -Pattern '\bAddHttpClientsFp\b'               -Replacement 'AddMlHttp'
& $r -Include $md -Pattern '\bGetPkValuesString\b'              -Replacement 'FormatCompositeKey'
& $r -Include $md -Pattern '\.GetPkValues\(\)'                  -Replacement '.FormatCompositeKey()'
& $r -Include $md -Pattern '\bAdd(Scoped|Singleton|Transient)OOFPRepos\b' -Replacement 'AddOofpRepositories$1'
& $r -Include $md -Pattern '\bAddOofpRepositoriesScoped\b'      -Replacement 'AddOofpRepositories'
