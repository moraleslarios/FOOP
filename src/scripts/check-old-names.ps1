$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$excluir = @('Temas Pendientes', 'MEJORAS_PENDIENTES.md', 'CONSEJOS_NOMENCLATURA.md', 'Cambios Realizados', '.github', 'node_modules', 'bin', 'obj')

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
 'AddSingletonGenServicesFp'
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
