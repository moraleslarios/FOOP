$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src

$pairs = @(
    @('MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit', 'MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit'),
    @('MoralesLarios.OOFP.Validation.Dataannotations',            'MoralesLarios.OOFP.Validation.DataAnnotations')
)

function Git([string[]]$a) {
    $out = & git.exe @a 2>&1
    if ($LASTEXITCODE -ne 0) { throw ('git ' + ($a -join ' ') + ' fallo: ' + ($out -join ' ')) }
}

# Reglas de contenido de la Fase 1 que afectan a estos ficheros (todas idempotentes en .cs/.csproj/.json).
$rules = @(
    @('MoralesLarios\.OOFP\.Validation\.Dataannotations\.Tests\.Unit(?![\w])', 'MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit'),
    @('MoralesLarios\.OOFP\.Validation\.Dataannotations(?![\w])',             'MoralesLarios.OOFP.Validation.DataAnnotations'),
    @('MoralesLarios\.FOOP\\MoralesLarios\.OOFP\.csproj',                      'MoralesLarios.OOFP.Core\MoralesLarios.OOFP.Core.csproj'),
    @('MoralesLarios\.OOFP\.Unit\.Tests(?![\w])',                              'MoralesLarios.OOFP.Core.Tests.Unit'),
    @('MoralesLarios\.OOFP\.Internals(?![\w])',                                'MoralesLarios.OOFP.Shared'),
    @('MoralesLarios\.OOFP\.HttpClients(?![\w])',                              'MoralesLarios.OOFP.Http')
)

foreach ($p in $pairs) {
    $old = $p[0]; $new = $p[1]; $tmp = $old + '.__tmp'
    Git @('checkout', 'HEAD', '--', $old)
    foreach ($f in @(& git.exe ls-files -- $old)) {
        $target = $tmp + $f.Substring($old.Length)
        $dir = Split-Path $target -Parent
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
        Git @('mv', $f, $target)
    }
    foreach ($f in @(& git.exe ls-files -- $tmp)) {
        $target = $new + $f.Substring($tmp.Length)
        $target = $target -replace [regex]::Escape($old + '.csproj') + '$', ($new + '.csproj')
        $dir = Split-Path $target -Parent
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
        Git @('mv', $f, $target)
    }
    if (Test-Path -LiteralPath $tmp) { Remove-Item -LiteralPath $tmp -Recurse -Force }
}

# Renombrar la carpeta en disco si Windows conservo la grafia antigua.
foreach ($p in $pairs) {
    $d = Get-ChildItem -Directory | Where-Object { $_.Name -ieq $p[1] }
    if ($d -and $d.Name -cne $p[1]) {
        try { Rename-Item -LiteralPath $d.FullName ($p[1] + '.__x'); Rename-Item -LiteralPath (Join-Path $src ($p[1] + '.__x')) $p[1] }
        catch { Write-Output "  Aviso: no se pudo cambiar la grafia de la carpeta $($d.Name) (bloqueada). Git ya tiene la ruta correcta." }
    }
}

$utf8Bom = New-Object System.Text.UTF8Encoding($true); $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
foreach ($p in $pairs) {
    foreach ($f in @(& git.exe ls-files -- $p[1])) {
        if ($f -notmatch '\.(cs|csproj|json|md)$') { continue }
        $full  = Join-Path $src $f
        $bytes = [System.IO.File]::ReadAllBytes($full)
        $bom   = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        $t = [System.IO.File]::ReadAllText($full); $orig = $t
        foreach ($r in $rules) { $t = [regex]::Replace($t, $r[0], $r[1]) }
        if ($t -ne $orig) {
            $enc = $utf8NoBom; if ($bom) { $enc = $utf8Bom }
            [System.IO.File]::WriteAllText($full, $t, $enc)
            Write-Output "  contenido actualizado: $f"
        }
    }
}
Git @('add', '-A', '--', '.')
Write-Output 'Recuperacion completada.'
