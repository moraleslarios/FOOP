$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

# Slug de GitHub (algoritmo real): se renderiza el markdown, se pasa a
# minusculas, se elimina todo lo que NO sea \p{Word} (letras, marcas,
# digitos, subrayado), guion o espacio, y los espacios pasan a guion.
# IMPORTANTE: U+FE0F (variation selector) es categoria Mn -> SE CONSERVA.
#             U+26A0 (simbolo) -> se elimina.
# NO se recortan los guiones de los extremos.
function Slug($t) {
    $s = $t
    $s = [regex]::Replace($s, '!\[[^\]]*\]\([^)]*\)', '')      # imagenes
    $s = [regex]::Replace($s, '\[([^\]]*)\]\([^)]*\)', '$1')   # [txt](url) -> txt
    $s = $s -replace '`', ''
    $s = $s -replace '\*', ''
    $s = $s -replace '~', ''
    $s = $s.ToLowerInvariant()
    $s = [regex]::Replace($s, "[^\p{L}\p{M}\p{Nd}_ \-]", '')
    $s = $s -replace ' ', '-'
    return $s
}

function Sim($a, $b) {
    if ($a -eq $b) { return 1.0 }
    if ($a.Length -lt 2 -or $b.Length -lt 2) { return 0.0 }
    $sa = New-Object System.Collections.Generic.HashSet[string]
    for ($i = 0; $i -lt $a.Length - 1; $i++) { [void]$sa.Add($a.Substring($i, 2)) }
    $sb = New-Object System.Collections.Generic.HashSet[string]
    for ($i = 0; $i -lt $b.Length - 1; $i++) { [void]$sb.Add($b.Substring($i, 2)) }
    $inter = 0
    foreach ($x in $sa) { if ($sb.Contains($x)) { $inter++ } }
    $uni = $sa.Count + $sb.Count - $inter
    if ($uni -eq 0) { return 0.0 }
    return [double]$inter / $uni
}

# Documentos historicos de planificacion: no se auditan.
$excluir = @('MEJORAS_PENDIENTES.md', 'CONSEJOS_NOMENCLATURA.md')

$mds = Get-ChildItem -Path $src -Recurse -Filter *.md -File |
       Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules)\\' } |
       Where-Object { $_.FullName -notmatch '\\Temas Pendientes\\' } |
       Where-Object { $excluir -notcontains $_.Name }

$out = New-Object System.Collections.Generic.List[string]
$rev = 0

foreach ($f in $mds) {
    $txt = [System.IO.File]::ReadAllText($f.FullName, [System.Text.Encoding]::UTF8)
    $lineas = $txt -split "`r?`n"

    $anclas = New-Object System.Collections.Generic.List[string]
    $enCodigo = $false
    foreach ($l in $lineas) {
        if ($l -match '^\s*```') { $enCodigo = -not $enCodigo; continue }
        if ($enCodigo) { continue }
        if ($l -match '^(#{1,6})\s+(.*)$') { $anclas.Add((Slug $Matches[2].TrimEnd())) }
        foreach ($m in [regex]::Matches($l, '<a\s+(?:id|name)="([^"]+)"')) {
            $anclas.Add($m.Groups[1].Value.ToLowerInvariant())
        }
    }
    $set = New-Object System.Collections.Generic.HashSet[string]
    foreach ($a in $anclas) { [void]$set.Add($a) }

    $rel = $f.FullName.Substring($src.Length + 1)
    foreach ($m in [regex]::Matches($txt, '\]\(#([^)]+)\)')) {
        $rev++
        $a = [System.Uri]::UnescapeDataString($m.Groups[1].Value).ToLowerInvariant()
        if ($set.Contains($a)) { continue }
        $best = ''
        $bs = 0.0
        foreach ($c in $anclas) {
            $s = Sim $a $c
            if ($s -gt $bs) { $bs = $s; $best = $c }
        }
        $out.Add('ROTO|' + $rel + '|' + $a + '|' + ([math]::Round($bs, 2)) + '|' + $best)
    }
}

$p = Join-Path $PSScriptRoot '_anchors4.log'
[System.IO.File]::WriteAllLines($p, $out, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('revisados = ' + $rev + '  rotos = ' + $out.Count)
