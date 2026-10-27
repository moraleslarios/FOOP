$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$dir = 'MoralesLarios.OOFP.Logging'
$map = @{
    'GeneralExtensionLoggers'            = 'MlResultLogExtensions'
    'GeneralExtensionLoggers.Obsolete'   = 'MlResultLogExtensions.Obsolete'
    'GeneralExtensionLoggersCritical'    = 'MlResultLogExtensions.Critical'
    'GeneralExtensionLoggersDebug'       = 'MlResultLogExtensions.Debug'
    'GeneralExtensionLoggersError'       = 'MlResultLogExtensions.Error'
    'GeneralExtensionLoggersInformation' = 'MlResultLogExtensions.Information'
    'GeneralExtensionLoggersTrace'       = 'MlResultLogExtensions.Trace'
    'GeneralExtensionLoggersWarning'     = 'MlResultLogExtensions.Warning'
}
foreach ($old in $map.Keys) {
    $path = Join-Path $src "$dir\$old.cs"
    if (-not (Test-Path $path)) { continue }
    $b = [System.IO.File]::ReadAllBytes($path)
    $bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
    $t = [System.IO.File]::ReadAllText($path)
    $cls = $old -replace '\.Obsolete$', ''
    $n = [regex]::Matches($t, "public static (partial )?class $cls\b").Count
    if ($n -ne 1) { throw "$old : esperada 1 declaracion, hay $n" }
    $t = [regex]::Replace($t, "public static (partial )?class $cls\b", 'public static partial class MlResultLogExtensions')
    [System.IO.File]::WriteAllText($path, $t, (New-Object System.Text.UTF8Encoding($bom)))
    $tracked = & git.exe ls-files -- "$dir/$old.cs"
    if ($tracked) { & git.exe mv "$dir/$old.cs" "$dir/$($map[$old]).cs" } else { Move-Item $path (Join-Path $src "$dir\$($map[$old]).cs") }
    Write-Output "$old -> $($map[$old]).cs"
}
$r = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
& $r -Pattern '\bGeneralExtensionLoggers(Critical|Debug|Error|Information|Trace|Warning)?\b' -Replacement 'MlResultLogExtensions'
