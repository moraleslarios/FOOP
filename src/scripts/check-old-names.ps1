param([switch]$IncludeCs)

$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$excluir = @('Temas Pendientes', 'Cambios Realizados', '.github', 'node_modules', 'bin', 'obj')

$viejos = @(
 'NullToFailed','EmptyToFailed','BoolToResult','ProjectionWhile','ProjectionSplit',
 'IGenServiceFp','MlProblemsDetails','SimpleMlControllerBase','SimpleMlComplexPkControllerBase',
 'PkParameterAttribute','PerControllerOutputCachePolicy','SimpleMlCacheControllerBase',
 'SimpleMlComplexCacheControllerBase','MlActionResults','MlResultWebExtensionsPlus',
 'MlRequestWebExtensions','IMlConfigManager','IEfRepoReaderFp','IEfRepoAdderFp',
 'IEfRepoUpdaterFp','IEfRepoDeleterFp','IEfRepoWriterFp','IEfRepoFp','IEfPaginatorFp',
 'IWrapperIO','IHttpClientFactoryManager','GenClientFp','GenComplexClientFp',
 'CallRequestParamsInfo','LogMlResultFinal','PaginationInfo','PaginationResultInfo',
 'MlErrorsDetails','ChangeReturnResultAlwaisValid','ChangeReturnResultAlwaisFail',
 'TryMapIAsyncf','SecureValidValue','SecureFailErrorsDetails','FusionFailErros',
 'FusionErrosIfExists','VerifyedEnumerableResultData','MoralesLarios.OOFP.WebApi',
 'MoralesLarios.OOFP.WebControllers','MoralesLarios.OOFP.WebServices',
 'MoralesLarios.OOFP.EFCore','MoralesLarios.OOFP.ValueObjects.IO',
 'MoralesLarios.OOFP.Internals','AddScopedtGenServicesFp','AddTransientGenServicesFp',
 'AddSingletonGenServicesFp',
 'GetIHttpClientFactoryManager','HttpClientFactoryKey','httpClientFactoryManager',
 'GetHeaderInfo','GetHeaderPageInfo','GetHeaderPaginationInfo','GetHeaderPageNumber',
 'GetHeaderPageSize','ResolveHttpClientFactoryKey'
)

$out = New-Object System.Collections.Generic.List[string]
$ficheros = Get-ChildItem -Path $src -Filter '*.md' -Recurse -File
$tot = 0
foreach ($f in $ficheros) {
    $rel = $f.FullName.Substring($src.Length + 1)
    $salta = $false
    foreach ($e in $excluir) { if ($rel.StartsWith($e)) { $salta = $true } }
    if ($salta) { continue }
    $lineas = [System.IO.File]::ReadAllLines($f.FullName, [System.Text.Encoding]::UTF8)
    for ($i = 0; $i -lt $lineas.Count; $i++) {
        foreach ($v in $viejos) {
            if ($lineas[$i].Contains($v)) {
                $out.Add($rel + '|L' + ($i + 1) + '|' + $v + '|' + $lineas[$i].Trim())
                $tot++
                break
            }
        }
    }
}
$p = Join-Path $PSScriptRoot '_oldnames.log'
[System.IO.File]::WriteAllLines($p, $out, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('coincidencias = ' + $tot)

if ($IncludeCs) {
    $tokens = @('Alwais', 'Lenght', 'lenght', 'Faild', 'faild', 'Diferent', 'diferent', 'Erros', 'Scopedt', 'Adress', 'Enumrable', 'IAsyncf', 'soported', 'Bydouble', 'Fromdouble')
    $whitelist = @{
        'MoralesLarios.OOFP.Core\Types\MlResultBindExtensions.Obsolete.cs|Faild' = 15
        'MoralesLarios.OOFP.Core\Types\MlResultChangeReturnResult.Obsolete.cs|Alwais' = 15
        'MoralesLarios.OOFP.Core\Types\MlResultCollections.Obsolete.cs|Erros' = 6
        'MoralesLarios.OOFP.Core\Types\MlResultErrorDetailsExtensions.Obsolete.cs|Diferent' = 5
        'MoralesLarios.OOFP.ValueObjects\IntBetween.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\IntLessThan.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\IntMoreThan.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\MemberName.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\ServiceKey.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\StringLengthBetween.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\StringMaxLength.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects\StringMinLength.Obsolete.cs|Lenght' = 1
        'MoralesLarios.OOFP.ValueObjects.Tests.Unit\ObsoleteBridgeContractTests.cs|Lenght' = 9
        'MoralesLarios.OOFP.Logging.Tests.Unit\MlResultLogExtensionsNomenclaturaTests.cs|faild' = 2
    }
    $falsePositives = @{
        'MoralesLarios.OOFP.Core\Types\MlResultBindExtensions.cs|3443|diferent' = $true
        'MoralesLarios.OOFP.Core\Types\MlResultMapExtensions.cs|2872|diferent' = $true
    }
    $approved = @{}
    $findings = New-Object System.Collections.Generic.List[string]
    $csFiles = Get-ChildItem -Path $src -Filter '*.cs' -Recurse -File

    foreach ($file in $csFiles) {
        $relative = $file.FullName.Substring($src.Length + 1)
        if ($relative -match '(^|\\)(bin|obj|\.git)(\\|$)' -or $relative.EndsWith('.g.cs')) { continue }
        $content = [System.IO.File]::ReadAllText($file.FullName)
        $lineas = [System.IO.File]::ReadAllLines($file.FullName)

        foreach ($token in $tokens) {
            $matches = [regex]::Matches($content, [regex]::Escape($token))
            foreach ($match in $matches) {
                $lineNumber = [regex]::Matches($content.Substring(0, $match.Index), "`n").Count + 1
                $line = $lineas[$lineNumber - 1]
                $key = $relative + '|' + $token
                if ($whitelist.ContainsKey($key) -and (!$approved.ContainsKey($key) -or $approved[$key] -lt $whitelist[$key])) {
                    if (!$approved.ContainsKey($key)) { $approved[$key] = 0 }
                    $approved[$key]++
                    continue
                }
                if ($falsePositives.ContainsKey($relative + '|' + $lineNumber + '|' + $token)) { continue }

                if ($line -match '^\s*///') {
                    $classification = 'comentario XML'
                } elseif ($line.Contains([char]34) -or $line.Contains([char]39)) {
                    $classification = 'literal de cadena'
                } elseif ($line -match ('\b(?:string|int|long|double|float|decimal|bool|object)\s+' + [regex]::Escape($token) + '\w*\b')) {
                    $classification = 'parametro'
                } elseif ($relative -match '\.Tests\.(Unit|Integration)\\' -and $line -match '^\s*public\s+(?:async\s+)?(?:void|Task)\s+') {
                    $classification = 'nombre de test'
                } else {
                    $classification = 'tipo/miembro'
                }
                $findings.Add($relative + '|L' + $lineNumber + '|' + $token + '|' + $classification + '|' + $line.Trim())
            }
        }
    }

    foreach ($key in $whitelist.Keys) {
        $actual = if ($approved.ContainsKey($key)) { $approved[$key] } else { 0 }
        if ($actual -ne $whitelist[$key]) { $findings.Add($key + '|esperadas=' + $whitelist[$key] + '|encontradas=' + $actual) }
    }

    $csLog = Join-Path $PSScriptRoot '_oldnames-cs.log'
    [System.IO.File]::WriteAllLines($csLog, $findings, (New-Object System.Text.UTF8Encoding($true)))
    Write-Output ('hallazgos C# fuera de la lista blanca = ' + $findings.Count)
    if ($findings.Count -gt 0) {
        Get-Content $csLog
        exit 1
    }
}
