$ErrorActionPreference = 'Continue'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $src
$out = & dotnet test MoralesLarios.OOFP.sln --nologo -v q 2>&1
Pop-Location
$log = Join-Path $PSScriptRoot '_tests.log'
[System.IO.File]::WriteAllLines($log, ($out | ForEach-Object { [string]$_ }))
$res = $out | Where-Object { $_ -match 'Correctas!|Con error!|Superado:|Omitido:|Total:|error CS|Error\b' } | ForEach-Object { ([string]$_).Trim() }
$sum = Join-Path $PSScriptRoot '_tests_sum.log'
[System.IO.File]::WriteAllLines($sum, ($res | ForEach-Object { [string]$_ }))
Write-Output ('lineas resumen = ' + $res.Count)
