[CmdletBinding()]
param(
    [string]$RootPath
)

if ([string]::IsNullOrWhiteSpace($RootPath)) {
    $RootPath = Join-Path (Split-Path -Parent $PSCommandPath) '..'
}

$root = (Resolve-Path -LiteralPath $RootPath).Path
$projectFiles = @(Get-ChildItem -LiteralPath $root -Filter '*.csproj' -File -Recurse |
    Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$projectReferences = @{}
$violations = [System.Collections.Generic.List[string]]::new()

foreach ($projectFile in $projectFiles) {
    try {
        [xml]$projectXml = [System.IO.File]::ReadAllText($projectFile.FullName)
    }
    catch {
        $violations.Add("No se pudo leer XML de proyecto: $($projectFile.FullName)")
        continue
    }

    $references = @(
        foreach ($reference in $projectXml.SelectNodes("//*[local-name()='ProjectReference']")) {
            $include = $reference.GetAttribute('Include')
            if ($include) {
                [System.IO.Path]::GetFileNameWithoutExtension($include)
            }
        }
    )
    $projectReferences[$projectFile.BaseName] = $references
}

foreach ($requiredProject in @('MoralesLarios.OOFP.Core', 'MoralesLarios.OOFP.Shared')) {
    if (-not $projectReferences.ContainsKey($requiredProject)) {
        $violations.Add("No se encontro el proyecto requerido: $requiredProject")
    }
}

$coreReferences = @($projectReferences['MoralesLarios.OOFP.Core'])
$forbiddenCoreReferences = @($coreReferences | Where-Object {
    $_ -eq 'MoralesLarios.OOFP.Logging' -or
    $_ -eq 'MoralesLarios.OOFP.Http' -or
    $_ -like 'MoralesLarios.OOFP.AspNetCore.*'
})
foreach ($reference in $forbiddenCoreReferences) {
    $violations.Add("Core no puede depender de $reference.")
}

foreach ($reference in @($projectReferences['MoralesLarios.OOFP.Shared'])) {
    $violations.Add("Shared no puede depender del proyecto $reference.")
}

$validationProjects = @($projectReferences.Keys | Where-Object { $_ -like 'MoralesLarios.OOFP.Validation*' })
foreach ($project in $validationProjects) {
    if (@($projectReferences[$project]) -contains 'MoralesLarios.OOFP.Http') {
        $violations.Add("$project no puede depender de MoralesLarios.OOFP.Http.")
    }
}

if ($violations.Count -gt 0) {
    $violations | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "Arquitectura de proyectos valida ($($projectFiles.Count) proyectos inspeccionados)."