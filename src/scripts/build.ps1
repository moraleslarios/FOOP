$ErrorActionPreference = 'Continue'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Push-Location $src
$out = & dotnet build MoralesLarios.OOFP.sln -v q --nologo 2>&1
Pop-Location
$log = Join-Path $PSScriptRoot '_build.log'
[System.IO.File]::WriteAllLines($log, ($out | ForEach-Object { [string]$_ }))
$errs = $out | Where-Object { $_ -match ': error ' } | ForEach-Object { [string]$_ } | Sort-Object -Unique
$warns = $out | Where-Object { $_ -match ': warning ' } | ForEach-Object { [string]$_ } | Sort-Object -Unique
$sum = Join-Path $PSScriptRoot '_build_sum.log'
$lines = @()
$lines += 'ERRORES UNICOS = ' + $errs.Count
$lines += $errs
$lines += 'ADVERTENCIAS UNICAS = ' + $warns.Count
$lines += $warns
[System.IO.File]::WriteAllLines($sum, ($lines | ForEach-Object { [string]$_ }))
Write-Output ('errores=' + $errs.Count + ' advertencias=' + $warns.Count)
