$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$out  = Join-Path $env:TEMP 'inventario-ubicaciones.txt'
$sb = New-Object System.Text.StringBuilder

$terms = @(
    '\bProjection(Async)?\b',
    '\bDo(Async)?\s*[<(]',
    '(?<!Path\.)\bCombine(Async)?\s*[<(]',
    '\bEmptyToFailed',
    '\bNullToFailed',
    '\bBoolToResult',
    '\bMlResultTransformations\b',
    'abstract\s+\w+\s+BuildErrorMessage',
    'static\s+int\s+limit',
    '\bValidateObject\b',
    '\bAllAsync\b',
    'Duplex',
    '\bTryLast',
    '\bAddDetail\b',
    'namespace\s+MoralesLarios\.FOOP',
    'namespace\s+MoralesLarios\.OOFP\.Internals',
    'namespace\s+MoralesLarios\.OOFP\.Shared',
    'MlErrorsDetails\.Errors\b',
    'class\s+MlErrorsDetails',
    'EX_DESC_KEY\s*=',
    'VALUE_KEY\s*='
)

$files = Get-ChildItem -Path $root -Recurse -File -Include *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }
$cache = @{}
foreach ($f in $files) { $cache[$f.FullName] = [System.IO.File]::ReadAllText($f.FullName) }

foreach ($rx in $terms) {
    [void]$sb.AppendLine('=== ' + $rx + ' ===')
    foreach ($f in $files) {
        $n = [regex]::Matches($cache[$f.FullName], $rx).Count
        if ($n -gt 0) { [void]$sb.AppendLine(('{0,4}  {1}' -f $n, $f.FullName.Substring($root.Length + 1))) }
    }
}

[void]$sb.AppendLine('=== Contenido de MoralesLarios.OOFP.Shared ===')
Get-ChildItem -LiteralPath (Join-Path $root 'MoralesLarios.OOFP.Shared') -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object { [void]$sb.AppendLine($_.FullName.Substring($root.Length + 1) + '  ' + $_.Length) }

[void]$sb.AppendLine('=== Contenido de MoralesLarios.OOFP.Internals ===')
Get-ChildItem -LiteralPath (Join-Path $root 'MoralesLarios.OOFP.Internals') -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object { [void]$sb.AppendLine($_.FullName.Substring($root.Length + 1) + '  ' + $_.Length) }

[void]$sb.AppendLine('=== Proyectos en MoralesLarios.OOFP.sln ===')
Select-String -Path (Join-Path $root 'MoralesLarios.OOFP.sln') -Pattern '^Project\(' |
    ForEach-Object { [void]$sb.AppendLine($_.Line) }

[void]$sb.AppendLine('=== ProjectReference a Shared ===')
Get-ChildItem -Path $root -Recurse -File -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object {
        $t = [System.IO.File]::ReadAllText($_.FullName)
        if ($t -match 'OOFP\.Shared') { [void]$sb.AppendLine('Shared    <- ' + $_.Name) }
        if ($t -match 'OOFP\.Internals') { [void]$sb.AppendLine('Internals <- ' + $_.Name) }
    }

[System.IO.File]::WriteAllText($out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('OK -> ' + $out)
