param(
    [string]$ReportsPath = 'TestResults',
    [string]$SummaryPath = $env:GITHUB_STEP_SUMMARY,
    [string]$BaselinePath,
    [switch]$FailOnDecrease
)

$ErrorActionPreference = 'Stop'
$reports = @(Get-ChildItem -LiteralPath $ReportsPath -Recurse -Filter 'coverage.cobertura.xml' -File)
if ($reports.Count -eq 0) {
    throw 'No Cobertura reports found.'
}

$packages = @{}
foreach ($report in $reports) {
    [xml]$coverage = Get-Content -LiteralPath $report.FullName -Raw
    foreach ($package in $coverage.SelectNodes("//*[local-name()='package']")) {
        $name = $package.GetAttribute('name')
        if (-not $packages.ContainsKey($name)) {
            $packages[$name] = @{}
        }

        foreach ($class in $package.SelectNodes("./*[local-name()='classes']/*[local-name()='class']")) {
            $file = $class.GetAttribute('filename')
            if ($file -match '(^|[\\/])obj([\\/]|$)') {
                continue
            }

            foreach ($line in $class.SelectNodes(".//*[local-name()='line']")) {
                $key = "$file|$($line.GetAttribute('number'))"
                $hit = [int]$line.GetAttribute('hits') -gt 0
                if (-not $packages[$name].ContainsKey($key) -or $hit) {
                    $packages[$name][$key] = $hit
                }
            }
        }
    }
}

$baseline = @{}
if ($BaselinePath) {
    $baselineData = Get-Content -LiteralPath $BaselinePath -Raw | ConvertFrom-Json
    foreach ($project in $baselineData.projects) {
        if ($project.linesValid -le 0) {
            throw "Invalid coverage baseline for $($project.name)."
        }
        $baseline[$project.name] = 100 * $project.linesCovered / $project.linesValid
    }
}

$summary = [System.Collections.Generic.List[string]]::new()
$summary.Add('## Cobertura de codigo')
$summary.Add('')
$summary.Add('| Proyecto | Lineas cubiertas | Cobertura | Minimo |')
$summary.Add('|---|---:|---:|---:|')
$failures = [System.Collections.Generic.List[string]]::new()
foreach ($name in ($packages.Keys | Sort-Object)) {
    $lines = @($packages[$name].Values)
    $valid = $lines.Count
    $covered = @($lines | Where-Object { $_ }).Count
    $percentage = if ($valid -gt 0) { [math]::Round(100 * $covered / $valid, 1) } else { 0 }
    $minimum = if ($baseline.ContainsKey($name)) { $baseline[$name] } else { $null }
    $minimumText = if ($null -ne $minimum) { '{0:N1}%' -f $minimum } else { 'n/a' }
    $summary.Add("| $name | $covered/$valid | $percentage% | $minimumText |")

    if ($FailOnDecrease -and $valid -gt 0 -and $null -eq $minimum) {
        $failures.Add("No coverage baseline for $name.")
    }
    elseif ($FailOnDecrease -and $null -ne $minimum -and (100 * $covered / $valid) -lt $minimum) {
        $failures.Add("Coverage for $name fell from $('{0:N2}' -f $minimum)% to $('{0:N2}' -f (100 * $covered / $valid))%.")
    }
}

if ($SummaryPath) {
    Add-Content -LiteralPath $SummaryPath -Value ($summary -join [Environment]::NewLine)
}
$summary
if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}
