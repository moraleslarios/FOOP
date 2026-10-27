param([Parameter(Mandatory)][string]$Pattern, [Parameter(Mandatory)][string]$Replacement, [string]$Include = '*.cs,*.csproj,*.json,*.md', [string]$ExcludePath = '^$')
$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$inc = $Include.Split(',')
$files = Get-ChildItem -Recurse -File -Include $inc |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs|scripts|Temas Pendientes|Cambios Realizados)\\' -and $_.Name -notmatch '^(MEJORAS_PENDIENTES|CONSEJOS_NOMENCLATURA)\.md$' -and $_.FullName.Substring($src.Length) -notmatch $ExcludePath }
$total = 0; $nf = 0
foreach ($f in $files) {
    $b = [System.IO.File]::ReadAllBytes($f.FullName)
    $bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
    $t = [System.IO.File]::ReadAllText($f.FullName)
    $n = [regex]::Matches($t, $Pattern).Count
    if ($n -eq 0) { continue }
    $u = [regex]::Replace($t, $Pattern, $Replacement)
    if (([regex]::Matches($u, "`n")).Count -ne ([regex]::Matches($t, "`n")).Count) { throw "Cambio de lineas en $($f.FullName)" }
    [System.IO.File]::WriteAllText($f.FullName, $u, (New-Object System.Text.UTF8Encoding($bom)))
    $total += $n; $nf++
}
Write-Output ("{0} -> {1}: {2} sustituciones en {3} ficheros" -f $Pattern, $Replacement, $total, $nf)
