<#
.SYNOPSIS
    Verifica el prompt de migracion 1.x -> 2.x (oofp-upgrade-2.prompt.md) y que los commits que
    rompen compatibilidad lo hayan actualizado.

.DESCRIPTION
    Comprobaciones sobre el prompt (siempre):
      ERROR  Cada fila de la tabla de la seccion 3.3 tiene su subseccion '#### N'.
      ERROR  Cada subseccion '#### N' tiene su fila en la tabla.
      ERROR  Ningun numero se repite en la tabla.
      ERROR  Cada numero esta en la lista del informe final (Fase 6) y en el apendice de versiones.
      AVISO  Cada numero con ruptura tiene su barrido de busqueda en la Fase 5 (los puntos marcados
             'SIN RUPTURAS' quedan exentos).
      ERROR  El anexo de renombrados existe y no esta vacio.

    Comprobaciones sobre el historial (solo con -BaseRef):
      ERROR  Un commit con '!' en el tipo o con 'BREAKING CHANGE' en el cuerpo que NO toca el prompt.
      AVISO  Un commit que toca un puente '*.Obsolete.cs' o codigo de produccion (feat/fix/refactor/perf)
             y NO toca el prompt: hay que decidir si rompe algo.

.PARAMETER BaseRef
    Commit, rama o etiqueta desde la que auditar (se revisa BaseRef..HEAD). En una PR:
    origin/<rama-destino>. Para una auditoria retroactiva: la etiqueta o el commit de la 1.x.

.PARAMETER SoloAvisos
    Convierte los errores en avisos (codigo de salida 0). Util para una primera auditoria.

.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1

.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-migration-prompt.ps1 -BaseRef origin/main
#>
param(
    [string]$BaseRef = '',
    [switch]$SoloAvisos
)

$ErrorActionPreference = 'Stop'

$repo       = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$promptRel  = 'src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.prompt.md'
$renamesRel = 'src/docs_next/Copilot/.github/prompts/oofp-upgrade-2.renames.md'
$promptPath = Join-Path $repo $promptRel
$renamePath = Join-Path $repo $renamesRel

$errores = New-Object System.Collections.Generic.List[string]
$avisos  = New-Object System.Collections.Generic.List[string]

function Add-Error($mensaje)  { [void]$script:errores.Add($mensaje) }
function Add-Aviso($mensaje)  { [void]$script:avisos.Add($mensaje) }
function Tiene-Numero($texto, $n) { [regex]::IsMatch($texto, "(?<![0-9.])$n(?![0-9])") }

# ---------------------------------------------------------------- prompt
if (-not (Test-Path $promptPath)) { Write-Host "ERROR: no existe $promptRel"; exit 1 }

$lineas = [System.IO.File]::ReadAllLines($promptPath, [System.Text.Encoding]::UTF8)

function Indice-De($patron, $desde = 0) {
    for ($i = $desde; $i -lt $lineas.Count; $i++) { if ($lineas[$i] -match $patron) { return $i } }
    return -1
}

$ini33 = Indice-De '^### 3\.3 '
$fase5 = Indice-De '^## Fase 5'
$fase6 = Indice-De '^## Fase 6'
$final = Indice-De '^## Recordatorios finales'
$apend = Indice-De '^## Ap'

if (($ini33, $fase5, $fase6, $final, $apend) -contains -1) {
    Write-Host 'ERROR: no se encuentran las secciones 3.3, Fase 5, Fase 6, Recordatorios finales o Apendice en el prompt.'
    exit 1
}

$primeraSub = Indice-De '^#### ' ($ini33 + 1)

$filas = @{}
$duplicadas = @()
for ($i = $ini33; $i -lt $primeraSub; $i++) {
    if ($lineas[$i] -match '^\|\s*(\d+)\s*\|') {
        $n = [int]$Matches[1]
        if ($filas.ContainsKey($n)) { $duplicadas += $n } else { $filas[$n] = $i + 1 }
    }
}

$subsecciones = @{}
for ($i = $primeraSub; $i -lt $fase5; $i++) {
    if ($lineas[$i] -match '^#### (\d+(?:[, y]+\d+)*)\s') {
        foreach ($n in ($Matches[1] -split '[^0-9]+' | Where-Object { $_ })) { $subsecciones[[int]$n] = $lineas[$i] }
    }
}

$textoFase5    = ($lineas[$fase5..($fase6 - 1)])  -join "`n"
$textoFase6    = ($lineas[$fase6..($final - 1)])  -join "`n"
$textoApendice = ($lineas[$apend..($lineas.Count - 1)]) -join "`n"

foreach ($n in $duplicadas) { Add-Error "Punto $n repetido en la tabla de la seccion 3.3." }

foreach ($n in ($filas.Keys | Sort-Object)) {
    if (-not $subsecciones.ContainsKey($n)) { Add-Error "Punto $n : tiene fila en la tabla (linea $($filas[$n])) pero no subseccion '#### $n'." }
    if (-not (Tiene-Numero $textoFase6 $n))    { Add-Error "Punto $n : no aparece en el informe final (Fase 6)." }
    if (-not (Tiene-Numero $textoApendice $n)) { Add-Error "Punto $n : no aparece en el apendice de versiones." }

    $sinRuptura = $subsecciones.ContainsKey($n) -and ($subsecciones[$n] -match 'SIN RUPTURAS')
    if (-not $sinRuptura -and -not (Tiene-Numero $textoFase5 $n)) {
        Add-Aviso "Punto $n : no aparece por numero en el barrido de la Fase 5 (revisa que sus patrones esten por contenido)."
    }
}
foreach ($n in ($subsecciones.Keys | Sort-Object)) {
    if (-not $filas.ContainsKey($n)) { Add-Error "Punto $n : tiene subseccion '#### $n' pero no fila en la tabla de la seccion 3.3." }
}

if (-not (Test-Path $renamePath) -or (Get-Item $renamePath).Length -eq 0) { Add-Error "Falta o esta vacio el anexo $renamesRel." }

# ------------------------------------------------------------- historial
$revisados = 0
if ($BaseRef) {
    $hashes = @(git -C $repo log --no-merges --format='%H' "$BaseRef..HEAD")
    foreach ($h in $hashes) {
        $revisados++
        $corto   = $h.Substring(0, 7)
        $asunto  = (git -C $repo log -1 --format=%s $h)
        $cuerpo  = (git -C $repo log -1 --format=%b $h) -join "`n"
        $ficheros = @(git -C $repo diff-tree --no-commit-id --name-only -r $h)
        $tocaPrompt = $ficheros -contains $promptRel

        $rompe = ($asunto -match '^[a-z]+(\([^)]*\))?!:') -or ($cuerpo -match 'BREAKING CHANGE')
        $tocaCodigo = @($ficheros | Where-Object { $_ -match '^src/.+\.cs$' -and $_ -notmatch '(Tests?\.|\.Sample/|TestSupport/)' }).Count -gt 0
        $tocaPuente = @($ficheros | Where-Object { $_ -match '\.Obsolete\.cs$' }).Count -gt 0
        $tipoCodigo = $asunto -match '^(feat|fix|refactor|perf)(\(|!|:)'

        if ($rompe -and -not $tocaPrompt) {
            Add-Error "Commit $corto marca ruptura y NO actualiza el prompt de migracion: $asunto"
        }
        elseif (-not $tocaPrompt -and $tocaPuente) {
            Add-Aviso "Commit $corto toca un puente [Obsolete] y no el prompt: $asunto"
        }
        elseif (-not $tocaPrompt -and $tocaCodigo -and $tipoCodigo) {
            Add-Aviso "Commit $corto cambia codigo de produccion y no toca el prompt (decide si rompe algo): $asunto"
        }
    }
}

# ---------------------------------------------------------------- salida
Write-Host ''
Write-Host "Prompt: $promptRel ($($lineas.Count) lineas)"
Write-Host "Puntos en la tabla 3.3: $($filas.Count) | subsecciones: $($subsecciones.Count)"
if ($BaseRef) { Write-Host "Commits revisados ($BaseRef..HEAD): $revisados" }

foreach ($a in $avisos)  { Write-Host "AVISO: $a" }
foreach ($e in $errores) { Write-Host "ERROR: $e" }

Write-Host ''
Write-Host "Resultado: $($errores.Count) errores, $($avisos.Count) avisos."

if ($errores.Count -gt 0 -and -not $SoloAvisos) { exit 1 }
exit 0
