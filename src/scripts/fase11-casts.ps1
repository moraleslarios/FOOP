$ErrorActionPreference = 'Stop'
$path = Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'MoralesLarios.OOFP.Http\RegisterServices.cs'
$b = [System.IO.File]::ReadAllBytes($path)
$bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
$t = [System.IO.File]::ReadAllText($path)
$lines = ([regex]::Matches($t, "`n")).Count
function Rep([string]$pattern, [string]$replacement, [int]$expected) {
    $n = [regex]::Matches($script:t, $pattern).Count
    if ($n -ne $expected) { throw "'$pattern': esperadas $expected, hay $n" }
    $script:t = [regex]::Replace($script:t, $pattern, $replacement)
}
Rep '\(Action<HttpClient>\)\(client =>' 'client =>' 3
Rep '(?m)^(\s*)\}\)\);' '$1});' 3
Rep 'ResolveHttpClientFactoryKey\(Func<ServiceKey> configureClient,' 'ResolveHttpClientFactoryKey(Func<ServiceKey> clientNameFactory,' 1
Rep '(?<=var httpClientFactoryKey = )configureClient is not null(\s*)\? configureClient\(\)' 'clientNameFactory is not null$1? clientNameFactory()' 1
if (([regex]::Matches($t, "`n")).Count -ne $lines) { throw 'Cambio de lineas' }
[System.IO.File]::WriteAllText($path, $t, (New-Object System.Text.UTF8Encoding($bom)))
Write-Output 'OK'
