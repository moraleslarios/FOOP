$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$rx = [regex]'\]\((?<t>[^)\s]+)(\s+"[^"]*")?\)'
$broken = @(); $checked = 0
$md = Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }
foreach ($f in $md) {
    $text = [System.IO.File]::ReadAllText($f.FullName)
    foreach ($m in $rx.Matches($text)) {
        $t = $m.Groups['t'].Value
        if ($t -match '^(https?:|mailto:|#|[a-z]+://)') { continue }
        $h = $t.IndexOf('#'); if ($h -ge 0) { $t = $t.Substring(0, $h) }
        if ($t -eq '') { continue }
        $checked++
        $p = [System.IO.Path]::GetFullPath((Join-Path $f.DirectoryName ([Uri]::UnescapeDataString($t).Replace('/', '\'))))
        if (-not (Test-Path -LiteralPath $p)) { $broken += ('{0} -> {1}' -f $f.FullName.Substring($src.Length + 1), $m.Groups['t'].Value) }
    }
}
Write-Output ("Enlaces relativos comprobados: {0}; rotos: {1}" -f $checked, $broken.Count)
$broken | Select-Object -First 40
