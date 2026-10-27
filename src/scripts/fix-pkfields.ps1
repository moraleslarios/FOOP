$src = 'c:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src'
$files = Get-ChildItem -Path $src -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' }
$total = 0
foreach ($f in $files) {
  $txt = [System.IO.File]::ReadAllText($f.FullName)
  $n = [regex]::Matches($txt, '\b_pkFields\b').Count
  if ($n -gt 0) {
    $linesBefore = ($txt -split "`n").Count
    $new = [regex]::Replace($txt, '\b_pkFields\b', '_keySelector')
    $linesAfter = ($new -split "`n").Count
    if ($linesBefore -ne $linesAfter) { Write-Output "ABORT lineas $($f.FullName)"; exit 1 }
    [System.IO.File]::WriteAllText($f.FullName, $new, (New-Object System.Text.UTF8Encoding($true)))
    Write-Output ("{0,-70} {1}" -f $f.FullName.Substring($src.Length+1), $n)
    $total += $n
  }
}
Write-Output "TOTAL=$total"
