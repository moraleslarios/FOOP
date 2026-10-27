$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
& $r -Include $md -Pattern 'MlErrorsDetails'            -Replacement 'MlErrorDetails'
& $r -Include $md -Pattern '\.Details\b'                -Replacement '.Metadata'
& $r -Include $md -Pattern '\bToErrorsMessages\b'       -Replacement 'GetErrorMessages'
& $r -Include $md -Pattern '\bToErrorsDescription\b'    -Replacement 'GetErrorsSummary'
& $r -Include $md -Pattern '\bToDetailsDescription\b'   -Replacement 'GetMetadataSummary'
& $r -Include $md -Pattern '\bHasKeyDetails\b'          -Replacement 'ContainsMetadata'
& $r -Include $md -Pattern '\bHasValueDetails'          -Replacement 'ContainsValueMetadata'
& $r -Include $md -Pattern '\bHasExceptionDetails'      -Replacement 'ContainsException'
& $r -Include $md -Pattern '\bAddDetailValue\b'         -Replacement 'AddValueMetadata'
& $r -Include $md -Pattern '\bAddDetails\b'             -Replacement 'AddMetadataRange'
& $r -Include $md -Pattern '\bAddDetail\b'              -Replacement 'AddMetadataOrThrow'
& $r -Include $md -Pattern '\bGetDetailValue'           -Replacement 'GetMetadataValue'
& $r -Include $md -Pattern '\bGetDetailException'       -Replacement 'GetException'

foreach ($f in Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\(bin|obj|Temas Pendientes|Cambios Realizados)\\' -and $_.Name -match 'MlErrorsDetails' }) {
    $base = (Get-Location).Path.Length + 1
    $new  = Join-Path $f.DirectoryName ($f.Name -replace 'MlErrorsDetails', 'MlErrorDetails')
    & git.exe mv $f.FullName.Substring($base) $new.Substring($base)
    Write-Output "git mv $($f.Name) -> $(Split-Path $new -Leaf)"
}
