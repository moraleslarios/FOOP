$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
foreach ($n in 'Bind', 'ExecSelf', 'Map', 'Match', 'Several') {
    $f = Get-ChildItem -Recurse -File -Filter "MlResultExtensions$n.md" | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | Select-Object -First 1
    if (-not $f) { continue }
    $base = (Get-Location).Path.Length + 1
    & git.exe mv $f.FullName.Substring($base) (Join-Path $f.DirectoryName "MlResult${n}Extensions.md").Substring($base)
    Write-Output "git mv $($f.Name) -> MlResult${n}Extensions.md"
}
foreach ($f in Get-ChildItem -Recurse -File -Filter '*Transformations*.md' | Where-Object { $_.FullName -notmatch '\\(bin|obj|Temas Pendientes)\\' -and $_.Name -match '^MlResultTransformations' }) {
    $base = (Get-Location).Path.Length + 1
    $new  = $f.Name -replace 'MlResultTransformations', 'MlResultConversions'
    & git.exe mv $f.FullName.Substring($base) (Join-Path $f.DirectoryName $new).Substring($base)
    Write-Output "git mv $($f.Name) -> $new"
}
