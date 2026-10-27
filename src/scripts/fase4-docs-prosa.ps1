$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$e = [char]0x2026
foreach ($rel in 'MoralesLarios.OOFP.ValueObjects\README.md', 'MoralesLarios.OOFP.ValueObjects.FileSystem\README.md') {
    $p = Join-Path $src $rel
    $b = [System.IO.File]::ReadAllBytes($p)
    $bom = $b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF
    $t = [System.IO.File]::ReadAllText($p)
    $marker = [string][char]0xE000
    # From -> CreateFrom primero (marcado para que la regla By -> From no lo vuelva a tocar)
    $t = [regex]::Replace($t, "(?<![\w])From$e", "Create${marker}From$e")
    $t = [regex]::Replace($t, "(?<![\w])By$e", "From$e")
    $t = $t.Replace("Create${marker}From", 'CreateFrom')
    [System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($bom)))
    Write-Output "actualizado $rel"
}
