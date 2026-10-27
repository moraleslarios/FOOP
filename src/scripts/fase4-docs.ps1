$ErrorActionPreference = 'Stop'
Set-Location (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$r  = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$md = '*.md'
$soloVo = '^\\(?!MoralesLarios\.OOFP\.(ValueObjects|FileSystem|Http|Application|AspNetCore|EntityFrameworkCore|Configuration|Validation)).*'

# Factorias de FileSystem (antes de las genericas)
& $r -Include $md -Pattern '\b(ExistsFile|ExistDirectory|MlFile|MlDirectory)\.FromString\b' -Replacement '$1.CreateFromPath'
& $r -Include $md -Pattern '\b(ExistsFile|ExistDirectory|MlFile|MlDirectory)\.ByString\b'   -Replacement '$1.FromPath'
# Factorias de ValueObjects: From* (lanza) -> CreateFrom*, By* (MlResult) -> From*
& $r -Include $md -ExcludePath $soloVo -Pattern '\.FromStringLength\b' -Replacement '.CreateFromLength'
& $r -Include $md -ExcludePath $soloVo -Pattern '\.From(String|Int|Decimal|Double|Regex|Enum|IntLength|DecimalLength|DoubleLength)\b' -Replacement '.CreateFrom$1'
& $r -Include $md -ExcludePath $soloVo -Pattern '`From…`'  -Replacement '`CreateFrom…`'
& $r -Include $md -ExcludePath $soloVo -Pattern '\bFrom…(?=\s*\()' -Replacement 'CreateFrom…'
& $r -Include $md -ExcludePath $soloVo -Pattern '\.ByStringLength\b' -Replacement '.FromLength'
& $r -Include $md -ExcludePath $soloVo -Pattern '\.By(String|Int|Decimal|Double|Regex|IntLength|DecimalLength|DoubleLength)\b' -Replacement '.From$1'
& $r -Include $md -ExcludePath $soloVo -Pattern '`By…'     -Replacement '`From…'
& $r -Include $md -ExcludePath $soloVo -Pattern '\bBy…'     -Replacement 'From…'

# Tipos con nombre generico: solo patrones inequivocos y solo en README de proyectos satelite
& $r -Include $md -ExcludePath $soloVo -Pattern '`Key`'   -Replacement '`ServiceKey`'
& $r -Include $md -ExcludePath $soloVo -Pattern '(?<![\w.])Key\.(?=[A-Z])' -Replacement 'ServiceKey.'
& $r -Include $md -ExcludePath $soloVo -Pattern '<Key>'   -Replacement '<ServiceKey>'
& $r -Include $md -ExcludePath $soloVo -Pattern '(?<![\w.])Key(?= [a-z]\w*\s*[,)=;])' -Replacement 'ServiceKey'
& $r -Include $md -ExcludePath $soloVo -Pattern '`Name`'  -Replacement '`MemberName`'
& $r -Include $md -ExcludePath $soloVo -Pattern '(?<![\w.])Name\.(?=[A-Z])' -Replacement 'MemberName.'
& $r -Include $md -ExcludePath $soloVo -Pattern '`Empty`' -Replacement '`EmptyValue`'
& $r -Include $md -ExcludePath $soloVo -Pattern '(?<![\w.])Empty\.(?=[A-Z])' -Replacement 'EmptyValue.'

# Tipos con nombre inequivoco: en toda la documentacion
& $r -Include $md -Pattern '(?<=\w)NotNegative\b'     -Replacement 'NonNegative'
& $r -Include $md -Pattern '\bRangeEnumValueObject\b' -Replacement 'EnumRangeValueObject'
& $r -Include $md -Pattern '\bStringBetweenLength\b'  -Replacement 'StringLengthBetween'
& $r -Include $md -Pattern '\bExistsFile\b'           -Replacement 'ExistingFilePath'
& $r -Include $md -Pattern '\bExistDirectory\b'       -Replacement 'ExistingDirectoryPath'
& $r -Include $md -Pattern '\bMlFile\b'               -Replacement 'FilePath'
& $r -Include $md -Pattern '\bMlDirectory\b'          -Replacement 'DirectoryPath'
& $r -Include $md -ExcludePath $soloVo -Pattern '\bBuildErrorMessage\b' -Replacement 'FormatErrorMessage'
& $r -Include $md -ExcludePath $soloVo -Pattern '\b(static (int|decimal|double) )limit\b' -Replacement '$1MinAllowedValue'
& $r -Include $md -ExcludePath $soloVo -Pattern '\.limit\b' -Replacement '.MinAllowedValue'

# EndpointPattern: SearchPattern en FilePath/DirectoryPath, ValidationPattern en Endpoint/Mail
foreach ($f in Get-ChildItem -Recurse -File -Filter *.md | Where-Object { $_.FullName -notmatch '\\(bin|obj|Temas Pendientes|Cambios Realizados)\\' }) {
    $t = [System.IO.File]::ReadAllText($f.FullName)
    if ($t -notmatch 'EndpointPattern') { continue }
    $lines = $t -split "`n"
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -notmatch 'EndpointPattern') { continue }
        $new = 'SearchPattern'; if ($lines[$i] -match '\b(Endpoint|Mail)\b' -and $lines[$i] -notmatch '(FilePath|DirectoryPath)') { $new = 'ValidationPattern' }
        $lines[$i] = $lines[$i] -replace '\bEndpointPattern\b', $new
    }
    [System.IO.File]::WriteAllText($f.FullName, ($lines -join "`n"), (New-Object System.Text.UTF8Encoding($true)))
    Write-Output "EndpointPattern actualizado en $($f.Name)"
}
