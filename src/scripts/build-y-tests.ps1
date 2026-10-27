param([switch]$Tests)
$ErrorActionPreference = 'Continue'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
& git.exe checkout -- MoralesLarios.OOFP.sln 2>$null

$out = & dotnet build MoralesLarios.OOFP.sln -v q --nologo 2>&1 | ForEach-Object { $_.ToString() }
$errors   = $out | Where-Object { $_ -match ': error ' } | ForEach-Object { ($_ -replace '^.*?\\src\\', '' -replace '\s*\[[^\]]+\]\s*$', '') } | Sort-Object -Unique
$warnings = $out | Where-Object { $_ -match ': warning ' } | ForEach-Object { if ($_ -match ': warning (\w+)') { $Matches[1] } } | Group-Object | ForEach-Object { '{0} x{1}' -f $_.Name, $_.Count }
$summary  = $out | Where-Object { $_ -match '^\s*\d+ (Warning|Error)\(s\)' } | ForEach-Object { $_.Trim() }

Write-Output ('BUILD: ' + ($summary -join ' | '))
if ($warnings) { Write-Output ('  warnings (unicos): ' + ($warnings -join ', ')) }
$errors | Select-Object -First 30 | ForEach-Object { if ($_.Length -gt 240) { '  ' + $_.Substring(0, 240) } else { '  ' + $_ } }
if ($errors.Count -gt 30) { Write-Output ("  ... y {0} errores mas" -f ($errors.Count - 30)) }

if ($Tests -and -not $errors) {
    $t = & dotnet test MoralesLarios.OOFP.sln --nologo --no-build 2>&1 | ForEach-Object { $_.ToString() }
    $t | Where-Object { $_ -match '^\s*(Passed|Failed)!' } | ForEach-Object { '  ' + (($_.Trim()) -replace 'Duration: [^-]+- ', '' -replace 'Skipped:\s+0, ', '') }
}
& git.exe checkout -- MoralesLarios.OOFP.sln 2>$null
