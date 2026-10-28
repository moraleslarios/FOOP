$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$malos = 0
$revisados = 0

$mds = Get-ChildItem -Path $src -Recurse -Filter *.md -File |
       Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules)\\' }

foreach ($f in $mds) {
    $t = [System.IO.File]::ReadAllText($f.FullName, [System.Text.Encoding]::UTF8)
    $ms = [regex]::Matches($t, '\]\(([^)#:]+\.md)(#[^)]*)?\)')
    foreach ($m in $ms) {
        $rel = $m.Groups[1].Value
        if ($rel -match '^https?') { continue }
        $revisados++
        $relDec = [System.Uri]::UnescapeDataString($rel)
        $destino = Join-Path $f.DirectoryName $relDec
        if (-not (Test-Path -LiteralPath $destino)) {
            $malos++
            $corto = $f.FullName.Substring($src.Length + 1)
            Write-Output ('ROTO: ' + $corto + ' -> ' + $rel)
        }
    }
}
Write-Output ('revisados = ' + $revisados + '  rotos = ' + $malos)
