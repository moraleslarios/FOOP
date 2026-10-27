$ErrorActionPreference = 'Stop'

$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

$terms = @('Faild', 'Diferent', 'FusionFailErros', 'Alwais')

$files = Get-ChildItem -Path $root -Recurse -File -Include *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }

foreach ($term in $terms) {

    Write-Output ''
    Write-Output ('=== ' + $term + ' ===')

    $rx = [regex]::Escape($term)

    foreach ($f in $files) {
        $t = [System.IO.File]::ReadAllText($f.FullName)
        $n = [regex]::Matches($t, $rx).Count
        if ($n -eq 0) { continue }
        $rel = $f.FullName.Substring($root.Length + 1)
        Write-Output ('{0,4}  {1}' -f $n, $rel)
    }
}

Write-Output ''
Write-Output '=== Firmas publicas afectadas (declaraciones) ==='
foreach ($f in $files) {
    $lines = [System.IO.File]::ReadAllLines($f.FullName)
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $l = $lines[$i]
        if ($l -match 'public\s+static' -and $l -match '(Faild|Diferent|FusionFailErros|Alwais)') {
            $rel = $f.FullName.Substring($root.Length + 1)
            Write-Output ('{0}:{1}  {2}' -f $rel, ($i + 1), $l.Trim())
        }
    }
}
