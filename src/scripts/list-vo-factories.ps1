$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$dir = Join-Path $src 'MoralesLarios.OOFP.ValueObjects'

$tipos = @('NotEmptyString','MemberName','ServiceKey','Mail','Endpoint','RegexValue',
           'StringMinLength','StringMaxLength','StringLengthBetween',
           'IntMoreThan','IntLessThan','IntBetween','IntNonNegative','Age','IdLite',
           'DecimalMoreThan','DecimalLessThan','DecimalBetween','DecimalNonNegative',
           'DoubleMoreThan','DoubleLessThan','DoubleBetween','DoubleNonNegative',
           'Id','StringAsInt','EmptyValue','Void')

foreach ($t in $tipos) {
    $p = Join-Path $dir ($t + '.cs')
    if (-not (Test-Path -LiteralPath $p)) { Write-Output ('-- ' + $t + ' : NO EXISTE'); continue }
    $txt = [System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8)
    $ms = [regex]::Matches($txt, 'public\s+static\s+([A-Za-z0-9_<>\.]+)\s+([A-Za-z0-9_]+)\s*\(')
    $l = @()
    foreach ($m in $ms) { $l += ($m.Groups[2].Value + ':' + $m.Groups[1].Value) }
    Write-Output ('-- ' + $t + ' : ' + ($l -join ' | '))
}
