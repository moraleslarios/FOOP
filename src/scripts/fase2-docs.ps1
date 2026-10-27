$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$r = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
& $r -Include $md -Pattern 'IfFaild'                                          -Replacement 'IfFailed'
& $r -Include $md -Pattern 'IfFailDiferentTypes'                              -Replacement 'IfFailDifferentTypes'
& $r -Include $md -Pattern 'ChangeReturnResultAlwais'                         -Replacement 'ChangeReturnResultAlways'
& $r -Include $md -Pattern '\bFusionFailErros'                                -Replacement 'MergeFailureErrors'
& $r -Include $md -Pattern '\bFusionErrosIfExists'                            -Replacement 'MergeErrorsIfExists'
& $r -Include $md -Pattern '\bFromStringLenght\b'                             -Replacement 'FromStringLength'
& $r -Include $md -Pattern '\bFromIntLenght\b'                                -Replacement 'FromIntLength'
& $r -Include $md -Pattern '\bMinLenght\b'                                    -Replacement 'MinLength'
& $r -Include $md -Pattern '\b(min|max)Lenght\b'                              -Replacement '$1Length'
& $r -Include $md -Pattern '\blenght\b'                                       -Replacement 'length'
& $r -Include $md -Pattern '\bAddScopedtGenServicesFpWithoutReposGeneral\b'   -Replacement 'AddScopedCrudServicesFpWithoutRepositories'
& $r -Include $md -Pattern '\bAddScopedtGenServicesDuplexFpWithoutReposGeneral\b' -Replacement 'AddScopedCrudServicesDuplexFpWithoutRepositories'
& $r -Include $md -Pattern '\bTEnumrableResponse\b'                           -Replacement 'TItem'

foreach ($f in Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\(bin|obj|Temas Pendientes|Cambios Realizados)\\' -and $_.Name -match 'Faild' }) {
    $rel = $f.FullName.Substring((Get-Location).Path.Length + 1)
    $new = Join-Path $f.DirectoryName ($f.Name -replace 'Faild', 'Failed')
    & git.exe mv $rel $new.Substring((Get-Location).Path.Length + 1)
    Write-Output "git mv $($f.Name) -> $(Split-Path $new -Leaf)"
}

# Enlaces desde los documentos historicos a ficheros renombrados.
foreach ($f in Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -match '\\Temas Pendientes\\' }) {
    $t = [System.IO.File]::ReadAllText($f.FullName)
    $u = [regex]::Replace($t, '(\]\([^)]*?)IfFaild([^)]*\))', '$1IfFailed$2')
    if ($u -ne $t) { [System.IO.File]::WriteAllText($f.FullName, $u, (New-Object System.Text.UTF8Encoding($true))); Write-Output "enlaces actualizados en $($f.Name)" }
}
