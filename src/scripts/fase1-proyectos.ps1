param([switch]$DryRun, [switch]$SoloMovimientos)
$ErrorActionPreference = 'Stop'

$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$utf8Bom   = New-Object System.Text.UTF8Encoding($true)
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

# ------------------------------------------------------------------ mapas
# Proyectos: orden = especifico antes que generico.
$projects = @(
    @('MoralesLarios.OOFP.ValueObjects.IO.Test.Unit',               'MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit'),
    @('MoralesLarios.OOFP.ValueObjects.IO',                         'MoralesLarios.OOFP.ValueObjects.FileSystem'),
    @('MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit',   'MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit'),
    @('MoralesLarios.OOFP.Validation.Dataannotations',              'MoralesLarios.OOFP.Validation.DataAnnotations'),
    @('MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit', 'MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit'),
    @('MoralesLarios.OOFP.Validation.FluentValidations',            'MoralesLarios.OOFP.Validation.FluentValidation'),
    @('MoralesLarios.OOFP.WebControllers.Cache',                    'MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching'),
    @('MoralesLarios.OOFP.WebControllers',                          'MoralesLarios.OOFP.AspNetCore.Controllers'),
    @('MoralesLarios.OOFP.WebApi.Tests.Unit',                       'MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit'),
    @('MoralesLarios.OOFP.WebApi',                                  'MoralesLarios.OOFP.AspNetCore.Results'),
    @('MoralesLarios.OOFP.WebServices.Tests.Unit',                  'MoralesLarios.OOFP.Application.Tests.Unit'),
    @('MoralesLarios.OOFP.WebServices',                             'MoralesLarios.OOFP.Application'),
    @('MoralesLarios.OOFP.HttpClients.Tests.Integration',           'MoralesLarios.OOFP.Http.Tests.Integration'),
    @('MoralesLarios.OOFP.HttpClients.Tests.Unit',                  'MoralesLarios.OOFP.Http.Tests.Unit'),
    @('MoralesLarios.OOFP.HttpClients',                             'MoralesLarios.OOFP.Http'),
    @('MoralesLarios.OOFP.EFCore.Infrastructure.Tests',             'MoralesLarios.OOFP.EntityFrameworkCore.TestSupport'),
    @('MoralesLarios.OOFP.EFCore.Integration.Tests',                'MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration'),
    @('MoralesLarios.OOFP.EFCore.WebApi',                           'MoralesLarios.OOFP.EntityFrameworkCore.Sample'),
    @('MoralesLarios.OOFP.EFCore',                                  'MoralesLarios.OOFP.EntityFrameworkCore'),
    @('MoralesLarios.OOFP.Extensions.Loggers.Console.Tests',        'MoralesLarios.OOFP.Logging.Sample'),
    @('MoralesLarios.OOFP.Extensions.Loggers',                      'MoralesLarios.OOFP.Logging'),
    @('MoralesLarios.OOFP.Utilities.Tests.Unit',                    'MoralesLarios.OOFP.Configuration.Tests.Unit'),
    @('MoralesLarios.OOFP.Utilities',                               'MoralesLarios.OOFP.Configuration'),
    @('MoralesLarios.OOFP.IO',                                      'MoralesLarios.OOFP.FileSystem'),
    @('MoralesLarios.OOFP.Unit.Tests',                              'MoralesLarios.OOFP.Core.Tests.Unit')
)

# Rutas antiguas -> nuevas (para reescribir enlaces de los .md). Especifico primero.
$pathMap = New-Object System.Collections.ArrayList
function Add-PathMap([string]$old, [string]$new) { [void]$pathMap.Add(@((Join-Path $src $old), (Join-Path $src $new))) }
Add-PathMap 'MoralesLarios.FOOP\__Doc\Types'               'docs\core-types'
Add-PathMap 'MoralesLarios.FOOP\__Doc\Bucle'               'docs\loops'
Add-PathMap 'MoralesLarios.FOOP\__Doc\PendingTasks.txt'    'Temas Pendientes\PendingTasks-legacy.md'
Add-PathMap 'MoralesLarios.FOOP\__Doc'                     'docs'
Add-PathMap 'MoralesLarios.FOOP\MoralesLarios.OOFP.csproj' 'MoralesLarios.OOFP.Core\MoralesLarios.OOFP.Core.csproj'
Add-PathMap 'MoralesLarios.FOOP'                           'MoralesLarios.OOFP.Core'
Add-PathMap 'MoralesLarios.OOFP.EFCore\OopRepos'           'MoralesLarios.OOFP.EntityFrameworkCore\Repositories.Imperative'
Add-PathMap 'MoralesLarios.OOFP.EFCore\Repos'              'MoralesLarios.OOFP.EntityFrameworkCore\Repositories'
Add-PathMap 'MoralesLarios.OOFP.HttpClients\ParamsInfo'    'MoralesLarios.OOFP.Http\Requests'
Add-PathMap 'MoralesLarios.OOFP.Internals\Info'            'MoralesLarios.OOFP.Shared\Pagination'
Add-PathMap 'MoralesLarios.OOFP.Internals\MoralesLarios.OOFP.Internals.csproj' 'MoralesLarios.OOFP.Shared\MoralesLarios.OOFP.Shared.csproj'
Add-PathMap 'MoralesLarios.OOFP.Internals'                 'MoralesLarios.OOFP.Shared'
foreach ($p in $projects) {
    Add-PathMap ($p[0] + '\' + $p[0] + '.csproj') ($p[1] + '\' + $p[1] + '.csproj')
    Add-PathMap $p[0] $p[1]
}

function Map-Path([string]$abs) {
    foreach ($m in $pathMap) {
        $old = $m[0]; $new = $m[1]
        if ($abs.Equals($old, [StringComparison]::OrdinalIgnoreCase)) { return $new }
        if ($abs.StartsWith($old + '\', [StringComparison]::OrdinalIgnoreCase)) { return $new + $abs.Substring($old.Length) }
    }
    return $abs
}

# Sustituciones de texto (namespaces, nombres de proyecto, rutas). Se aplican en orden.
$textRules = New-Object System.Collections.ArrayList
function Add-Rule([string]$pattern, [string]$replacement) { [void]$textRules.Add(@($pattern, $replacement)) }
Add-Rule 'MoralesLarios\.FOOP\\MoralesLarios\.OOFP\.csproj' 'MoralesLarios.OOFP.Core\MoralesLarios.OOFP.Core.csproj'
Add-Rule 'MoralesLarios\.FOOP/MoralesLarios\.OOFP\.csproj'  'MoralesLarios.OOFP.Core/MoralesLarios.OOFP.Core.csproj'
Add-Rule '"MoralesLarios\.OOFP", "MoralesLarios\.OOFP\.Core\\' '"MoralesLarios.OOFP.Core", "MoralesLarios.OOFP.Core\'
Add-Rule 'MoralesLarios\.OOFP\.Internals\.Info(?![\w])' 'MoralesLarios.OOFP.Shared.Pagination'
Add-Rule 'MoralesLarios\.OOFP\.Internals(?![\w])'      'MoralesLarios.OOFP.Shared'
foreach ($p in $projects) { Add-Rule ([regex]::Escape($p[0]) + '(?![\w])') $p[1] }
Add-Rule 'MoralesLarios\.OOFP\.EntityFrameworkCore\.OopRepos(?![\w])' 'MoralesLarios.OOFP.EntityFrameworkCore.Repositories.Imperative'
Add-Rule 'MoralesLarios\.OOFP\.EntityFrameworkCore\.Repos(?![\w])'    'MoralesLarios.OOFP.EntityFrameworkCore.Repositories'
Add-Rule 'MoralesLarios\.OOFP\.Http\.ParamsInfo(?![\w])'              'MoralesLarios.OOFP.Http.Requests'

# Solo en prosa de .md (los enlaces ya se reescriben aparte). No toca URLs (precedidas de '/').
$mdRules = New-Object System.Collections.ArrayList
[void]$mdRules.Add(@('(?<![/\w.])MoralesLarios\.FOOP[/\\]__Doc[/\\]Types', 'docs/core-types'))
[void]$mdRules.Add(@('(?<![/\w.])MoralesLarios\.FOOP[/\\]__Doc[/\\]Bucle', 'docs/loops'))
[void]$mdRules.Add(@('(?<![/\w.])MoralesLarios\.FOOP[/\\]__Doc', 'docs'))
[void]$mdRules.Add(@('(?<![/\w.])__Doc[/\\]Types', 'docs/core-types'))
[void]$mdRules.Add(@('(?<![/\w.])__Doc[/\\]Bucle', 'docs/loops'))
[void]$mdRules.Add(@('(?<![/\w.])__Doc(?![\w])', 'docs'))
[void]$mdRules.Add(@('(?<![/\w.\\])MoralesLarios\.FOOP(?=[/\\])(?![/\\]src)', 'MoralesLarios.OOFP.Core'))

# ------------------------------------------------------------------ ficheros a tratar
function Test-Excluded([string]$full) {
    return ($full -match '\\(bin|obj|\.git|\.vs|scripts|Temas Pendientes|Cambios Realizados)\\') -or
           ($full -match '\\(MEJORAS_PENDIENTES|CONSEJOS_NOMENCLATURA)\.md$')
}

$textFiles = Get-ChildItem -Recurse -File -Include *.cs,*.csproj,*.sln,*.json,*.md,*.http,*.props,*.targets,*.lutconfig |
    Where-Object { -not (Test-Excluded $_.FullName) -and $_.Name -ne 'MoralesLarios.OOFP - copia.sln' -and $_.Name -ne 'MoralesLarios.FOOP.sln' -and $_.FullName -notmatch 'ValueObjects\.IO\.2\.Tests\.Unit' }

# Los .md de Temas Pendientes y la raiz: solo se les reescriben ENLACES, no el texto.
$linkOnlyFiles = Get-ChildItem -Recurse -File -Filter *.md |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' -and (Test-Excluded $_.FullName) -and $_.FullName -notmatch '\\scripts\\' }

function Read-Text([string]$path) {
    $bytes  = [System.IO.File]::ReadAllBytes($path)
    $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
    return @{ Text = [System.IO.File]::ReadAllText($path); Bom = $hasBom }
}

function Write-Text([string]$path, [string]$text, [bool]$bom) {
    if ($DryRun) { return }
    $enc = $utf8NoBom; if ($bom) { $enc = $utf8Bom }
    [System.IO.File]::WriteAllText($path, $text, $enc)
}

function Count-Lines([string]$t) { return ([regex]::Matches($t, "`n")).Count }

function Rewrite-Links([string]$text, [string]$fileOld) {
    $dirOld = Split-Path $fileOld -Parent
    $dirNew = Split-Path (Map-Path $fileOld) -Parent
    $rx = [regex]'\]\((?<t>[^)\s]+)(?<title>\s+"[^"]*")?\)'
    return $rx.Replace($text, {
        param($m)
        $t = $m.Groups['t'].Value
        if ($t -match '^(https?:|mailto:|#|[a-z]+://)') { return $m.Value }
        $anchor = ''
        $hash = $t.IndexOf('#')
        if ($hash -ge 0) { $anchor = $t.Substring($hash); $t = $t.Substring(0, $hash) }
        if ($t -eq '') { return $m.Value }
        $decoded = [Uri]::UnescapeDataString($t).Replace('/', '\')
        $trailing = $decoded.EndsWith('\')
        try { $absOld = [System.IO.Path]::GetFullPath((Join-Path $dirOld $decoded.TrimEnd('\'))) } catch { return $m.Value }
        $absNew = Map-Path $absOld
        if ($absNew -eq $absOld -and $dirNew -eq $dirOld) { return $m.Value }
        $from = New-Object Uri(($dirNew.TrimEnd('\') + '\'))
        $to   = New-Object Uri($absNew)
        $rel  = [Uri]::UnescapeDataString($from.MakeRelativeUri($to).ToString())
        if ($rel -eq '') { $rel = '.' }
        if ($trailing) { $rel = $rel + '/' }
        $rel = $rel.Replace(' ', '%20')
        return '](' + $rel + $anchor + $m.Groups['title'].Value + ')'
    })
}

$changed = 0; $report = @{}
function Note([string]$k) { if ($report.ContainsKey($k)) { $report[$k]++ } else { $report[$k] = 1 } }

if (-not $SoloMovimientos) {

# ------------------------------------------------------------------ 1. contenido
foreach ($f in $textFiles) {
    $r = Read-Text $f.FullName
    $t = $r.Text
    if ($f.Extension -eq '.sln') {
        $t = [regex]::Replace($t, '(?m)^Project\([^\r\n]*"MoralesLarios\.OOFP\.Internals"[^\r\n]*\r?\nEndProject\r?\n', '')
        $t = [regex]::Replace($t, '(?m)^[^\r\n]*\{DA8DCC51-825A-4346-9E26-C17E2B4DC15B\}[^\r\n]*\r?\n', '')
        Note 'sln  entrada Internals eliminada'
    }
    $before = Count-Lines $t

    if ($f.Extension -eq '.md') { $t = Rewrite-Links $t $f.FullName }

    foreach ($rule in $textRules) {
        $n = [regex]::Matches($t, $rule[0]).Count
        if ($n -gt 0) { $t = [regex]::Replace($t, $rule[0], $rule[1]); Note ('txt  ' + $rule[1]) }
    }
    if ($f.Extension -eq '.md') {
        foreach ($rule in $mdRules) {
            if ([regex]::IsMatch($t, $rule[0])) { $t = [regex]::Replace($t, $rule[0], $rule[1]); Note ('md   ' + $rule[1]) }
        }
    }

    if ($t -ne $r.Text) {
        if ((Count-Lines $t) -ne $before) { throw "Cambio de numero de lineas en $($f.FullName)" }
        Write-Text $f.FullName $t $r.Bom
        $changed++
    }
}

foreach ($f in $linkOnlyFiles) {
    $r = Read-Text $f.FullName
    $t = Rewrite-Links $r.Text $f.FullName
    if ($t -ne $r.Text) {
        if ((Count-Lines $t) -ne (Count-Lines $r.Text)) { throw "Cambio de numero de lineas en $($f.FullName)" }
        Write-Text $f.FullName $t $r.Bom
        $changed++; Note 'enlaces en docs historicos'
    }
}

Write-Output ("Ficheros con contenido modificado: {0}" -f $changed)
$report.GetEnumerator() | Sort-Object Name | ForEach-Object { "  {0,4}  {1}" -f $_.Value, $_.Name }

if ($DryRun) { return }

# ------------------------------------------------------------------ 2. csproj: duplicados y Core
foreach ($f in Get-ChildItem -Recurse -File -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }) {
    $r = Read-Text $f.FullName
    $seen = @{}
    $lines = $r.Text -split "`r?`n"
    $out = foreach ($l in $lines) {
        if ($l -match '<ProjectReference Include="([^"]+)"') {
            $k = $Matches[1].ToLowerInvariant()
            if ($seen.ContainsKey($k)) { continue }
            $seen[$k] = 1
        }
        $l
    }
    $t = ($out -join "`r`n")
    if ($t -ne $r.Text) { Write-Text $f.FullName $t $r.Bom; Write-Output "  ProjectReference duplicado eliminado en $($f.Name)" }
}

foreach ($f in Get-ChildItem -Recurse -File -Filter GlobalUsings.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }) {
    $r = Read-Text $f.FullName
    $seen = @{}
    $lines = $r.Text -split "`r?`n"
    $out = foreach ($l in $lines) {
        $k = $l.Trim()
        if ($k -like 'global using *') { if ($seen.ContainsKey($k)) { continue }; $seen[$k] = 1 }
        $l
    }
    $t = ($out -join "`r`n")
    if ($t -ne $r.Text) { Write-Text $f.FullName $t $r.Bom; Write-Output "  global using duplicado eliminado en $($f.FullName.Substring($src.Length + 1))" }
}

$core = Join-Path $src 'MoralesLarios.FOOP\MoralesLarios.OOFP.csproj'
$r = Read-Text $core
if ($r.Text -notmatch '<RootNamespace>') {
    $t = $r.Text -replace '(\s*)<Version>2\.0\.0</Version>', ('$1<Version>2.0.0</Version>' + '$1<RootNamespace>MoralesLarios.OOFP</RootNamespace>')
    Write-Text $core $t $r.Bom
}

$shared = Join-Path $src 'MoralesLarios.OOFP.Shared\MoralesLarios.OOFP.Shared.csproj'
$r = Read-Text $shared
if ($r.Text -notmatch 'ValueObjects\.csproj') {
    $t = $r.Text -replace '</Project>', ("  <ItemGroup>`r`n    <ProjectReference Include=`"..\MoralesLarios.OOFP.ValueObjects\MoralesLarios.OOFP.ValueObjects.csproj`" />`r`n  </ItemGroup>`r`n`r`n</Project>")
    Write-Text $shared $t $r.Bom
}

}

# ------------------------------------------------------------------ 3. borrados y movimientos
function Git([string[]]$a) {
    & git.exe @a 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { throw ('git ' + ($a -join ' ') + ' fallo') }
}

function GitRm([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) { return }
    $tracked = & git.exe ls-files -- $path
    if ($tracked) { Git @('rm', '-q', '-r', '-f', $path) }
    if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Recurse -Force }
}

function GitMv([string]$from, [string]$to) {
    if (-not (Test-Path -LiteralPath $from)) { return }
    if (-not (Test-Path -LiteralPath $from -PathType Container)) {
        $dir = Split-Path $to -Parent
        if ($dir -and -not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
        Git @('mv', $from, $to)
        return
    }
    # Directorio: fichero a fichero (C# Dev Kit bloquea el renombrado de carpetas de proyecto).
    $prefix = ($from.TrimEnd('/', '\') -replace '\\', '/') + '/'
    foreach ($f in @(& git.exe ls-files -- $from)) {
        $rel = ($f -replace '^src/', '')
        if (-not $rel.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) { $rel = $f }
        $target = ($to.TrimEnd('/', '\') -replace '\\', '/') + '/' + $rel.Substring($prefix.Length)
        $dir = Split-Path $target -Parent
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
        Git @('mv', $rel, $target)
    }
    try { Remove-Item -LiteralPath $from -Recurse -Force -ErrorAction Stop } catch { Write-Output "  (resto no versionado sin borrar: $from)" }
}

Get-ChildItem -Recurse -Directory -Include bin,obj | Where-Object { $_.FullName -notmatch '\\scripts\\' } | Remove-Item -Recurse -Force

GitRm 'MoralesLarios.OOFP - copia.sln'
GitRm 'MoralesLarios.FOOP/MoralesLarios.FOOP.sln'
GitRm 'MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit'
GitRm 'MoralesLarios.OOFP.EFCore/Helpers/Constants.cs'
GitRm 'MoralesLarios.OOFP.WebServices/Services/GenService.cs'

# Internals -> Shared
New-Item -ItemType Directory -Force 'MoralesLarios.OOFP.Shared\Pagination' | Out-Null
GitMv 'MoralesLarios.OOFP.Internals/Info/PaginationInfo.cs'       'MoralesLarios.OOFP.Shared/Pagination/PaginationInfo.cs'
GitMv 'MoralesLarios.OOFP.Internals/Info/PaginationResultInfo.cs' 'MoralesLarios.OOFP.Shared/Pagination/PaginationResultInfo.cs'
GitMv 'MoralesLarios.OOFP.Internals/GlobalUsings.cs'              'MoralesLarios.OOFP.Shared/GlobalUsings.cs'
if (Test-Path 'MoralesLarios.OOFP.Internals\README.md') {
    $sharedReadme = Read-Text 'MoralesLarios.OOFP.Shared\README.md'
    $internalsReadme = Read-Text 'MoralesLarios.OOFP.Internals\README.md'
    Write-Text (Join-Path $src 'MoralesLarios.OOFP.Shared\README.md') ($sharedReadme.Text.TrimEnd() + "`r`n`r`n---`r`n`r`n<!-- Contenido fusionado desde el antiguo proyecto MoralesLarios.OOFP.Internals (v2.0.0) -->`r`n`r`n" + $internalsReadme.Text) $sharedReadme.Bom
}
GitRm 'MoralesLarios.OOFP.Internals'

# Carpetas internas
GitMv 'MoralesLarios.OOFP.EFCore/OopRepos'        'MoralesLarios.OOFP.EFCore/Repositories.Imperative'
GitMv 'MoralesLarios.OOFP.EFCore/Repos'           'MoralesLarios.OOFP.EFCore/Repositories'
GitMv 'MoralesLarios.OOFP.HttpClients/ParamsInfo' 'MoralesLarios.OOFP.HttpClients/Requests'

# Documentacion
GitMv 'MoralesLarios.FOOP/__Doc/PendingTasks.txt' 'Temas Pendientes/PendingTasks-legacy.md'
GitMv 'MoralesLarios.FOOP/__Doc' 'docs'
GitMv 'docs/Types' 'docs/core-types'
GitMv 'docs/Bucle' 'docs/loops'

# Proyectos
GitMv 'MoralesLarios.FOOP' 'MoralesLarios.OOFP.Core'
GitMv 'MoralesLarios.OOFP.Core/MoralesLarios.OOFP.csproj' 'MoralesLarios.OOFP.Core/MoralesLarios.OOFP.Core.csproj'
foreach ($p in $projects) {
    GitMv $p[0] $p[1]
    GitMv ($p[1] + '/' + $p[0] + '.csproj') ($p[1] + '/' + $p[1] + '.csproj')
}

Write-Output 'Fase 1 aplicada.'
