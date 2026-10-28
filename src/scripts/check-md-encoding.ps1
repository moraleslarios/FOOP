$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$out = New-Object System.Collections.Generic.List[string]

$mds = Get-ChildItem -Path $src -Recurse -Filter *.md -File |
       Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules)\\' }

foreach ($f in $mds) {
    $b = [System.IO.File]::ReadAllBytes($f.FullName)
    $bom = ''
    if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { $bom = 'BOM' }
    # detectar si es UTF-8 valido
    $enc = New-Object System.Text.UTF8Encoding($false, $true)
    $ok = 'UTF8'
    try { [void]$enc.GetString($b) } catch { $ok = 'NO-UTF8' }
    if ($ok -eq 'NO-UTF8' -or $bom -eq '') {
        $out.Add($ok + '|' + $bom + '|' + $f.FullName.Substring($src.Length + 1))
    }
}

$p = Join-Path $PSScriptRoot '_encodings.log'
[System.IO.File]::WriteAllLines($p, $out, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('sospechosos = ' + $out.Count + ' de ' + $mds.Count)
