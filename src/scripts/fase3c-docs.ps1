$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
$pairs = @(
    @('MlResultActionsErrorsDetails', 'MlResultErrorDetailsExtensions'),
    @('MlResultActionsExecSelf',      'MlResultExecSelfExtensions'),
    @('MlResultActionsSeveral',       'MlResultSeveralExtensions'),
    @('MlResultActionsMatch',         'MlResultMatchExtensions'),
    @('MlResultActionsBind',          'MlResultBindExtensions'),
    @('MlResultActionsMap',           'MlResultMapExtensions'),
    @('MlResultActions',              'MlResultExtensions'),
    @('MlResultBucles',               'MlResultCollections'),
    @('MlResultTransformations',      'MlResultConversions'),
    @('ProyectSplitValidInfo',        'TraverseSplitSuccess'),
    @('ProyectSplitFailInfo',         'TraverseSplitFailure')
)
foreach ($p in $pairs) { & $r -Include $md -Pattern ('\b' + $p[0] + '(?!\w)') -Replacement $p[1] }
& $r -Include $md -Pattern '\bProjectionSplitPlus'     -Replacement 'TraverseSplitWithKeys'
& $r -Include $md -Pattern '\bProjection(?=(Split|While|Parallel|Async)?\b)' -Replacement 'Traverse'
& $r -Include $md -Pattern '\bEmptyToFailed'           -Replacement 'FailIfEmpty'
& $r -Include $md -Pattern '\bNullToFailed'            -Replacement 'FailIfNull'
& $r -Include $md -Pattern '\bBoolToResult'            -Replacement 'FailIfFalse'
& $r -Include $md -Pattern '\bCombine(?=(Async)?\s*[<(`])' -Replacement 'CombineAll'
& $r -Include $md -Pattern '(?<=`)Combine(Async)?(?=`)'    -Replacement 'CombineAll$1'
& $r -Include $md -Pattern '\.Do(Async)?\s*(?=[<(])'       -Replacement '.Pipe$1'

$base = (Get-Location).Path.Length + 1
$fileMap = @(@('MlResultActionsErrorsDetails', 'MlResultErrorDetailsExtensions'), @('MlResultActions', 'MlResultExtensions'), @('NullToFailed', 'FailIfNull'), @('Bucles', 'Collections'))
foreach ($f in Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\(bin|obj|Temas Pendientes|Cambios Realizados)\\' }) {
    $name = $f.Name
    foreach ($m in $fileMap) { $name = $name -replace $m[0], $m[1] }
    if ($name -ne $f.Name) {
        & git.exe mv $f.FullName.Substring($base) (Join-Path $f.DirectoryName $name).Substring($base)
        Write-Output "git mv $($f.Name) -> $name"
        # Enlaces que apuntan al fichero renombrado.
        & $r -Include $md -Pattern ('(?<=[(/])' + [regex]::Escape($f.Name) + '(?=[)#])') -Replacement $name
    }
}
