$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$log = Join-Path $src 'scripts\_tests.log'
dotnet test MoralesLarios.OOFP.sln --nologo -v q 2>&1 | Out-File -FilePath $log -Encoding utf8
Write-Output "EXIT=$LASTEXITCODE"
Get-Content $log | Select-String -Pattern '^(Superado|Error|Failed|Passed|Fallido)!' | ForEach-Object { $_.Line.Trim() }
Write-Output '--- FALLOS ---'
Get-Content $log | Select-String -Pattern '^\s*(Error de|Failed|X )' | ForEach-Object { $_.Line.Trim() } | Select-Object -First 40
Write-Output '--- ERRORES DE COMPILACION ---'
Get-Content $log | Select-String -Pattern ': error ' | ForEach-Object { $_.Line.Trim() } | Select-Object -First 40
