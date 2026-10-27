$ErrorActionPreference = 'Stop'
$path = Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'MoralesLarios.OOFP.Http\RegisterServices.cs'
$b = [System.IO.File]::ReadAllBytes($path)
$bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
$t = [System.IO.File]::ReadAllText($path)
$lines = ([regex]::Matches($t, "`n")).Count

# El parametro Func<ServiceKey> renombrado (antes configureHttpClientKey) produce la clave: clientNameFactory.
$p1 = 'Func<ServiceKey>(\s+)configureClient(\s*)='
$n1 = [regex]::Matches($t, $p1).Count
if ($n1 -ne 3) { throw "Esperadas 3 declaraciones Func<ServiceKey> configureClient, hay $n1" }
$t = [regex]::Replace($t, $p1, 'Func<ServiceKey>$1clientNameFactory$2=')

$p2 = 'ResolveHttpClientFactoryKey\(configureClient,'
$n2 = [regex]::Matches($t, $p2).Count
if ($n2 -ne 3) { throw "Esperadas 3 llamadas ResolveHttpClientFactoryKey(configureClient, hay $n2" }
$t = [regex]::Replace($t, $p2, 'ResolveHttpClientFactoryKey(clientNameFactory,')

if (([regex]::Matches($t, "`n")).Count -ne $lines) { throw 'Cambio de lineas' }
[System.IO.File]::WriteAllText($path, $t, (New-Object System.Text.UTF8Encoding($bom)))
Write-Output 'OK'
