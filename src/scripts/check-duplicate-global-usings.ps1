$ErrorActionPreference = 'Stop'
$sourceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$pattern = [regex]'(?m)^[ \t]*global[ \t]+using[ \t]+(?<directive>[^;]+);'
$duplicates = New-Object System.Collections.Generic.List[string]

foreach ($project in Get-ChildItem -LiteralPath $sourceRoot -Recurse -Filter '*.csproj' -File) {
    $seen = [System.Collections.Generic.Dictionary[string, System.Collections.Generic.List[string]]]::new([System.StringComparer]::Ordinal)
    $sources = Get-ChildItem -LiteralPath $project.DirectoryName -Recurse -Filter '*.cs' -File |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }

    foreach ($source in $sources) {
        $text = [System.IO.File]::ReadAllText($source.FullName)
        foreach ($match in $pattern.Matches($text)) {
            $directive = ($match.Groups['directive'].Value -replace '\s+', ' ').Trim()
            if (-not $seen.ContainsKey($directive)) {
                $seen.Add($directive, [System.Collections.Generic.List[string]]::new())
            }
            $seen[$directive].Add($source.FullName)
        }
    }

    foreach ($entry in $seen.GetEnumerator()) {
        if ($entry.Value.Count -gt 1) {
            $duplicates.Add("$($project.BaseName): $($entry.Key) ($($entry.Value.Count) declarations)")
        }
    }
}

if ($duplicates.Count -gt 0) {
    $duplicates | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Output 'No duplicate global using directives found.'
