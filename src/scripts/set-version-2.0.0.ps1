$ErrorActionPreference = 'Stop'

$newVersion = '2.0.0'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$tags = @('Version', 'VersionPrefix', 'PackageVersion', 'AssemblyVersion', 'FileVersion')

$projects = Get-ChildItem -Path $root -Recurse -Filter *.csproj -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$report = @()

foreach ($p in $projects) {

    $bytes  = [System.IO.File]::ReadAllBytes($p.FullName)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $text   = [System.IO.File]::ReadAllText($p.FullName)

    $linesBefore = ($text -split "`n").Count
    $changes     = @()
    $newText     = $text

    foreach ($tag in $tags) {
        $pattern = "<$tag>([^<]*)</$tag>"
        $matches = [regex]::Matches($newText, $pattern)
        if ($matches.Count -eq 0) { continue }

        foreach ($m in $matches) {
            $old = $m.Groups[1].Value
            if ($old -ne $newVersion) { $changes += "$tag`: $old -> $newVersion" }
        }
        $newText = [regex]::Replace($newText, $pattern, "<$tag>$newVersion</$tag>")
    }

    if ($changes.Count -eq 0) {
        $report += [pscustomobject]@{ Project = $p.Name; Result = 'sin cambios' }
        continue
    }

    $linesAfter = ($newText -split "`n").Count
    if ($linesAfter -ne $linesBefore) {
        throw "ABORTADO: el recuento de lineas cambio en $($p.FullName) ($linesBefore -> $linesAfter)"
    }

    $encoding = New-Object System.Text.UTF8Encoding($hasBom)
    [System.IO.File]::WriteAllText($p.FullName, $newText, $encoding)

    $report += [pscustomobject]@{ Project = $p.Name; Result = ($changes -join ' ; ') }
}

$report | Format-Table -AutoSize -Wrap
