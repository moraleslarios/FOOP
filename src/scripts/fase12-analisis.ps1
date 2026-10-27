$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
function Show([string]$title, $items) { Write-Output "=== $title"; $items | ForEach-Object { '  ' + $_ } }
$cs = Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' }

Show 'C1 literales "Ex" en codigo y tests' ($cs | Select-String -Pattern '"Ex"' | ForEach-Object { '{0}:{1} {2}' -f $_.Filename, $_.LineNumber, $_.Line.Trim() } | Select-Object -First 8)
Show 'C2 constructores MlErrorDetails' ($cs | Where-Object Name -eq 'MlErrorDetails.cs' | Select-String -Pattern 'public MlErrorDetails\(|implicit operator' | ForEach-Object { ($_.Line.Trim() -replace '\s+', ' ') })
Show 'C3 MinAllowedValue' ($cs | Select-String -Pattern 'MinAllowedValue \{' | ForEach-Object { '{0}: {1}' -f $_.Filename, $_.Line.Trim() })
Show 'C4 constructores publicos en ValueObjects' ($cs | Where-Object { $_.FullName -match 'ValueObjects' -and $_.FullName -notmatch 'Tests' } | Select-String -Pattern '^\s*public \w+\(' | ForEach-Object { '{0}: {1}' -f $_.Filename, ($_.Line.Trim() -replace '\s+', ' ') })
Show 'C5 MlSelfValidatable' ($cs | Where-Object Name -eq 'MlSelfValidatable.cs' | ForEach-Object { Get-Content $_.FullName } | Where-Object { $_ -match '\w' -and $_ -notmatch '^//' })
Show 'C10 controladores base instanciados o heredados' ($cs | Select-String -Pattern '(new|:)\s*Ml(Cached)?(CompositeKey)?CrudControllerBase' | ForEach-Object { '{0}:{1} {2}' -f $_.Filename, $_.LineNumber, ($_.Line.Trim() -replace '\s+', ' ').Substring(0, [Math]::Min(110, ($_.Line.Trim() -replace '\s+', ' ').Length)) })
Show 'C10 declaraciones' ($cs | Select-String -Pattern 'public (abstract )?class Ml(Cached)?(CompositeKey)?CrudControllerBase' | ForEach-Object { '{0}: {1}' -f $_.Filename, ($_.Line.Trim() -replace '\s+', ' ').Substring(0, [Math]::Min(90, ($_.Line.Trim() -replace '\s+', ' ').Length)) })
Show 'C11 AddMlControllers' ($cs | Where-Object { $_.FullName -match 'AspNetCore\.Controllers\\RegisterServices\.cs$' } | ForEach-Object { Get-Content $_.FullName } | Where-Object { $_ -match '\w' -and $_ -notmatch '^//' })
Show 'C12 ParseDateTimeInvariant' ($cs | Select-String -Pattern 'ParseDateTimeInvariant\(string' -Context 0,8 | ForEach-Object { $_.Line.Trim(); $_.Context.PostContext | ForEach-Object { '    ' + $_.Trim() } })
Show 'C14 Description' ($cs | Select-String -Pattern 'Description.*null!' | Where-Object { $_.Filename -like 'CompositeKey*' } | ForEach-Object { '{0}: {1}' -f $_.Filename, $_.Line.Trim() })
Show 'C16 ToResponseErrorsDescription' ($cs | Select-String -Pattern 'ToResponseErrorsDescription' | ForEach-Object { '{0}:{1} {2}' -f $_.Filename, $_.LineNumber, ($_.Line.Trim() -replace '\s+', ' ').Substring(0, [Math]::Min(120, ($_.Line.Trim() -replace '\s+', ' ').Length)) })
