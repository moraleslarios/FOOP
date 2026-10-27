$ErrorActionPreference = 'Stop'
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
$vo = 'ValueObjects|FileSystem'
& $r -Include $md -Pattern '\.IsValid\b(?!\s*\()'                 -Replacement '.IsSuccess'
& $r -Include $md -Pattern '\.IsFail\b(?!\s*\()'                  -Replacement '.IsFailure'
& $r -Include $md -Pattern '(?<![\w.])IsValid(?![\w(])'           -Replacement 'IsSuccess' -ExcludePath $vo
& $r -Include $md -Pattern '(?<![\w.])IsFail(?![\w(])'            -Replacement 'IsFailure' -ExcludePath $vo
& $r -Include $md -Pattern '\bSecureValidValue'                   -Replacement 'GetValueOrThrow'
& $r -Include $md -Pattern '\bSecureFailErrorsDetails'            -Replacement 'GetFailureDetailsOrThrow'
& $r -Include $md -Pattern '\bEX_DESC_KEY\b'                      -Replacement 'ExceptionMetadataKey'
& $r -Include $md -Pattern '\bVALUE_KEY\b'                        -Replacement 'ValueMetadataKey'
& $r -Include $md -Pattern '\.ErrorsDetails\b'                    -Replacement '.FailureDetails'
