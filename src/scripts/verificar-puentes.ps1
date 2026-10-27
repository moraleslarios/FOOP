$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$bad = 0; $total = 0; $files = 0
foreach ($f in Get-ChildItem $src -Recurse -Filter *.Obsolete.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }) {
    $files++
    $t = [System.IO.File]::ReadAllText($f.FullName)
    $rx = [regex]'Obsolete\("Renamed to (?<new>[\w.]+)\. Will be removed in 3\.0\.0\."[^\]]*\]\s*(public|internal)[^=;{(]*?\s(?<old>\w+)\s*(<[^>(]*>)?\s*(\(|=>|\{|=)'
    foreach ($m in $rx.Matches($t)) {
        $total++
        if ($m.Groups['old'].Value -ceq $m.Groups['new'].Value) { $bad++; Write-Output ("PUENTE ROTO {0}: {1}" -f $f.Name, $m.Groups['old'].Value) }
    }
    $attrs = [regex]::Matches($t, 'Renamed to ').Count
    $decl  = $rx.Matches($t).Count
    if ($attrs -ne $decl) { Write-Output ("REVISAR {0}: {1} atributos y {2} declaraciones reconocidas" -f $f.Name, $attrs, $decl) }
}
Write-Output ("Ficheros de puentes: {0}; puentes: {1}; rotos: {2}" -f $files, $total, $bad)
