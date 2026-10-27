$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
function Edit([string]$rel, [string]$pattern, [string]$replacement, [int]$expected) {
    $path = Join-Path $src $rel
    $b = [System.IO.File]::ReadAllBytes($path)
    $bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
    $t = [System.IO.File]::ReadAllText($path)
    $n = [regex]::Matches($t, $pattern).Count
    if ($n -ne $expected) { throw "$rel : esperadas $expected coincidencias de '$pattern', hay $n" }
    [System.IO.File]::WriteAllText($path, [regex]::Replace($t, $pattern, $replacement), (New-Object System.Text.UTF8Encoding($bom)))
}
foreach ($rel in 'MoralesLarios.OOFP.AspNetCore.Results\Helpers\HttpRequestPaginationExtensions.cs', 'MoralesLarios.OOFP.Http\Helpers\MlHttpRequestExtensions.cs') {
    Edit $rel '"X-Page-Number"' 'global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageNumber' 1
    Edit $rel '"X-Page-Size"'   'global::MoralesLarios.OOFP.Shared.Web.MlPaginationHeaders.PageSize'   1
}
Write-Output 'OK'
