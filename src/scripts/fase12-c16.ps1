$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$p = Join-Path $src 'MoralesLarios.OOFP.Http\HttpApiInvoker.cs'
$t = [System.IO.File]::ReadAllText($p, [System.Text.Encoding]::UTF8)
$linesBefore = ($t -split "`n").Count

$old = 'response.ToResponseErrorsDescription().ToMlResultFail<HttpResponseMessage>()'
$new = '(await response.ReadErrorDescriptionAsync()).ToMlResultFail<HttpResponseMessage>()'
$n = [regex]::Matches($t, [regex]::Escape($old)).Count
Write-Output ('usos ToResponseErrorsDescription = ' + $n)
if ($n -ne 5) { throw ('ESPERADOS 5, ENCONTRADOS ' + $n) }
$t = $t.Replace($old, $new)

# los lambdas que ahora contienen await deben declararse async
$oldLambda = '(response => response.IsSuccessStatusCode'
$newLambda = '(async response => response.IsSuccessStatusCode'
$m = [regex]::Matches($t, [regex]::Escape($oldLambda)).Count
Write-Output ('lambdas response => = ' + $m)
if ($m -ne 5) { throw ('ESPERADOS 5 lambdas, ENCONTRADOS ' + $m) }
$t = $t.Replace($oldLambda, $newLambda)

$linesAfter = ($t -split "`n").Count
if ($linesBefore -ne $linesAfter) { throw 'CAMBIO DE LINEAS' }
[System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($true)))
Write-Output 'OK C16'
