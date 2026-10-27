$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
function Edit([string]$path, [string]$pattern, [string]$replacement, [int]$expected) {
    $b = [System.IO.File]::ReadAllBytes($path)
    $bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
    $t = [System.IO.File]::ReadAllText($path)
    $n = [regex]::Matches($t, $pattern).Count
    if ($n -ne $expected) { throw "$path : esperadas $expected coincidencias, hay $n" }
    [System.IO.File]::WriteAllText($path, [regex]::Replace($t, $pattern, $replacement), (New-Object System.Text.UTF8Encoding($bom)))
}
foreach ($f in Get-ChildItem (Join-Path $src 'MoralesLarios.OOFP.ValueObjects') -Filter 'StringAs*.Obsolete.cs') {
    if ($f.Name -eq 'StringAsNumeric.Obsolete.cs') { continue }
    Edit $f.FullName 'public static (MlResult<StringAs\w+> ByString)' 'public new static $1' 1
}
foreach ($n in 'ExistsFile', 'ExistDirectory') {
    Edit (Join-Path $src "MoralesLarios.OOFP.ValueObjects.FileSystem\$n.cs") 'public new static (\w+|MlResult<\w+>) (CreateFromPath|FromPath)\(' 'public static $1 $2(' 2
}
Write-Output 'OK'
