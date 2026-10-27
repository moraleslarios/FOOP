$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$files = @(
    'MoralesLarios.OOFP.AspNetCore.Controllers\Controllers\MlCrudControllerBase.cs',
    'MoralesLarios.OOFP.AspNetCore.Controllers\Controllers\MlCompositeKeyCrudControllerBase.cs',
    'MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching\Controllers\MlCachedCrudControllerBase.cs',
    'MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching\Controllers\MlCachedCompositeKeyCrudControllerBase.cs'
)
$pattern = 'public class (Ml(?:Cached)?(?:CompositeKey)?CrudControllerBase<)'
$total = 0
foreach ($f in $files) {
    $p = Join-Path $src $f
    $t = [System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8)
    $linesBefore = ($t -split "`n").Count
    $n = [regex]::Matches($t, $pattern).Count
    if ($n -eq 0) { Write-Output ('SIN COINCIDENCIAS ' + $f); continue }
    $t2 = [regex]::Replace($t, $pattern, 'public abstract class $1')
    $linesAfter = ($t2 -split "`n").Count
    if ($linesBefore -ne $linesAfter) { throw ('CAMBIO DE LINEAS en ' + $f) }
    [System.IO.File]::WriteAllText($p, $t2, (New-Object System.Text.UTF8Encoding($true)))
    Write-Output ('{0} {1}' -f $f, $n)
    $total += $n
}
Write-Output ('TOTAL=' + $total)
