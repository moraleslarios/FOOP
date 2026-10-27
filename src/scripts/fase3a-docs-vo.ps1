$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$targets = @(& git.exe diff --name-only -- '*ValueObjects*/*.md' '*Validation*/*.md' '*FileSystem*/*.md' | ForEach-Object { $_ -replace '^src/', '' })
if ($targets.Count -gt 0) { & git.exe checkout -- $targets }

# Lineas que hablan de otros IsValid (FluentValidation.ValidationResult o metodos de value objects).
$skip = 'ValidationResult|valResult|No existen|(Key|Name|ExistsFile|ExistDirectory|MlFile|MlDirectory|Age|Id|Mail|Endpoint|RegexValue|StringAs\w*|Int\w*|Decimal\w*|Double\w*|String\w*Length)\.IsValid|IsValid\s*\('
$rules = @(
    @('(?<=(\)|\b[a-z_]\w*))\.IsValid\b(?!\s*\()', '.IsSuccess'), @('(?<=(\)|\b[a-z_]\w*))\.IsFail\b(?!\s*\()', '.IsFailure'),
    @('(?<![\w.])IsValid(?!\w)', 'IsSuccess'), @('(?<![\w.])IsFail(?!\w)', 'IsFailure'),
    @('\bSecureValidValue', 'GetValueOrThrow'), @('\bSecureFailErrorsDetails', 'GetFailureDetailsOrThrow'),
    @('\bEX_DESC_KEY\b', 'ExceptionMetadataKey'), @('\bVALUE_KEY\b', 'ValueMetadataKey'), @('\.ErrorsDetails\b', '.FailureDetails')
)
foreach ($rel in $targets) {
    $path  = Join-Path $src $rel
    $bytes = [System.IO.File]::ReadAllBytes($path)
    $bom   = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
    $text  = [System.IO.File]::ReadAllText($path)
    $lines = $text -split "`n"
    $n = 0
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $l = $lines[$i]
        $isVoLine = $l -match $skip
        foreach ($r in $rules) {
            if ($isVoLine -and $r[0] -match 'IsValid|IsFail') { continue }
            if ($rel -match 'ValueObjects' -and $r[0] -match '^\(\?<!') { continue }
            $l = [regex]::Replace($l, $r[0], $r[1])
        }
        if ($l -ne $lines[$i]) { $lines[$i] = $l; $n++ }
    }
    if ($n -gt 0) {
        [System.IO.File]::WriteAllText($path, ($lines -join "`n"), (New-Object System.Text.UTF8Encoding($bom)))
        Write-Output ("{0,3} lineas  {1}" -f $n, $rel)
    }
}
