$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$csv = Join-Path $src 'Cambios Realizados\_registro.csv'
$dst = Join-Path $src 'Cambios Realizados\Renombrados-v2.0.0.md'

$lineas = [System.IO.File]::ReadAllLines($csv, [System.Text.Encoding]::UTF8)

# Simbolos no ASCII
$ac = [string][char]0x00E1   # a acentuada
$ec = [string][char]0x00E9   # e acentuada
$ic = [string][char]0x00ED   # i acentuada
$oc = [string][char]0x00F3   # o acentuada
$uc = [string][char]0x00FA   # u acentuada
$nt = [string][char]0x00F1   # enye
$ab = [string][char]0x00BF   # apertura interrogacion
$fl = [string][char]0x2192   # flecha
$ok = [string][char]0x2705   # check verde
$wr = [string][char]0x26A0 + [string][char]0xFE0F  # aviso
$iMay = [string][char]0x00CD   # I acentuada mayuscula

# Agrupacion: lote -> orden y titulo
$ordenLotes = @(
 'fase1-nivel0',
 'fase2-nivel1-erratas',
 'fase3a-nivel2-mlresult',
 'fase3b-nivel2-errordetails',
 'fase3c-nivel2-familias',
 'fase4a-nivel3-factorias',
 'fase4b-nivel3-valueobjects',
 'fase4c-nivel3-patrones',
 'fase5-6-niveles4-5',
 'fase7-nivel6-efcore',
 'fase7b-nivel6-registro',
 'fase8-10-niveles7-9',
 'fase11-nivel10-http',
 'fase11b-tipos-genericos-hermanos',
 'fase11c-format-composite-key',
 'fase11d-residuos',
 'fase13-docs',
 'fase13-genericos',
 'fase14-residuos'
)

$tituloLote = @{}
$tituloLote['fase1-nivel0'] = 'Nivel 0 ' + $fl + ' Soluci' + $oc + 'n, proyectos y carpetas'
$tituloLote['fase2-nivel1-erratas'] = 'Nivel 1 ' + $fl + ' Erratas ortogr' + $ac + 'ficas'
$tituloLote['fase3a-nivel2-mlresult'] = 'Nivel 2a ' + $fl + ' Estado y acceso de `MlResult`'
$tituloLote['fase3b-nivel2-errordetails'] = 'Nivel 2b ' + $fl + ' `MlErrorsDetails` ' + $fl + ' `MlErrorDetails`'
$tituloLote['fase3c-nivel2-familias'] = 'Nivel 2c ' + $fl + ' Familias de operadores del n' + $uc + 'cleo'
$tituloLote['fase4a-nivel3-factorias'] = 'Nivel 3a ' + $fl + ' Factor' + $ic + 'as de los Value Objects'
$tituloLote['fase4b-nivel3-valueobjects'] = 'Nivel 3b ' + $fl + ' Tipos de los Value Objects'
$tituloLote['fase4c-nivel3-patrones'] = 'Nivel 3c ' + $fl + ' Patrones de los Value Objects'
$tituloLote['fase5-6-niveles4-5'] = 'Niveles 4 y 5 ' + $fl + ' Validation, Shared, Configuration, FileSystem y Logging'
$tituloLote['fase7-nivel6-efcore'] = 'Nivel 6 ' + $fl + ' Repositorios de EntityFrameworkCore'
$tituloLote['fase7b-nivel6-registro'] = 'Nivel 6b ' + $fl + ' Registro de servicios de EntityFrameworkCore'
$tituloLote['fase8-10-niveles7-9'] = 'Niveles 7, 8 y 9 ' + $fl + ' Application, AspNetCore.Results y AspNetCore.Controllers'
$tituloLote['fase11-nivel10-http'] = 'Nivel 10 ' + $fl + ' Clientes REST e invocador HTTP'
$tituloLote['fase11b-tipos-genericos-hermanos'] = 'Nivel 10b ' + $fl + ' Par' + $ac + 'metros de tipo gen' + $ec + 'rico'
$tituloLote['fase11c-format-composite-key'] = 'Nivel 10c ' + $fl + ' Formato de claves compuestas'
$tituloLote['fase11d-residuos'] = 'Residuos detectados por el inventario'
$tituloLote['fase13-docs'] = 'Fase 13 ' + $fl + ' Documentaci' + $oc + 'n (`README.md` y `docs/`)'
$tituloLote['fase13-genericos'] = 'Fase 13b ' + $fl + ' Unificaci' + $oc + 'n del par' + $ac + 'metro gen' + $ec + 'rico `TReturn` en `TResult`'
$tituloLote['fase14-residuos'] = 'Fase 14 ' + $fl + ' Residuos de los sufijos `...Manager` y `...Info` en `Http` y `AspNetCore.Results`'

# Recolectar filas
$filas = @{}
$contratos = New-Object System.Collections.Generic.List[string[]]
$total = 0
for ($i = 1; $i -lt $lineas.Count; $i++) {
    $l = $lineas[$i]
    if ([string]::IsNullOrWhiteSpace($l)) { continue }
    $c = $l.Split(';')
    if ($c.Count -lt 7) { continue }
    $total++
    if ([string]$c[0] -eq 'fase12-contratos') { $contratos.Add($c); continue }
    $lote = [string]$c[0]
    if (-not $filas.ContainsKey($lote)) { $filas[$lote] = (New-Object System.Collections.Generic.List[string[]]) }
    $filas[$lote].Add($c)
}

function Esc($t) {
    $s = [string]$t
    if ([string]::IsNullOrWhiteSpace($s)) { return '' }
    $s = $s.Replace('|', '\|')
    return $s.Trim()
}

function Cod($t) {
    $s = Esc $t
    if ($s -eq '') { return '' }
    if ($s -eq '-') { return '-' }
    if ($s.StartsWith('(')) { return '*' + $s + '*' }
    return '`' + $s + '`'
}

$o = New-Object System.Collections.Generic.List[string]
$o.Add('# Renombrados de la versi' + $oc + 'n 2.0.0')
$o.Add('')
$o.Add('> Documento generado autom' + $ac + 'ticamente desde [`_registro.csv`](./_registro.csv) por')
$o.Add('> `scripts/generar-cambios-realizados.ps1`. **No lo edites a mano**: edita el CSV y vuelve a generarlo.')
$o.Add('')
$o.Add('Este documento recoge **todos** los cambios de nomenclatura aplicados a la soluci' + $oc + 'n')
$o.Add('`MoralesLarios.OOFP` en la versi' + $oc + 'n 2.0.0, con su **valor inicial** y su **valor modificado**.')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## ' + $iMay + 'ndice')
$o.Add('')
$o.Add('1. [Resumen](#resumen)')
$o.Add('2. [Gu' + $ic + 'a de migraci' + $oc + 'n r' + $ac + 'pida](#gu' + $ic + 'a-de-migraci' + $oc + 'n-r' + $ac + 'pida)')
$o.Add('3. [Compatibilidad hacia atr' + $ac + 's](#compatibilidad-hacia-atr' + $ac + 's)')
$o.Add('4. [Cambios de nomenclatura por nivel](#cambios-de-nomenclatura-por-nivel)')
$o.Add('5. [Cambios de contrato (Fase 12)](#cambios-de-contrato-fase-12)')
$o.Add('6. [Cambios planificados no aplicados](#cambios-planificados-no-aplicados)')
$o.Add('7. [Validaci' + $oc + 'n](#validaci' + $oc + 'n)')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Resumen')
$o.Add('')
$o.Add('| Concepto | Valor |')
$o.Add('|---|---|')
$o.Add('| Total de cambios registrados | **' + $total + '** |')
$o.Add('| Cambios de nomenclatura | **' + ($total - $contratos.Count) + '** |')
$o.Add('| Cambios de contrato (Fase 12) | **' + $contratos.Count + '** |')
$o.Add('| Proyectos de la soluci' + $oc + 'n | 30 |')
$o.Add('| Commits de la rama `release/2.0.0-nomenclatura` | 10 |')
$o.Add('')
$o.Add('### Cambios por lote')
$o.Add('')
$o.Add('| Lote | Nivel | Cambios |')
$o.Add('|---|---|---|')
foreach ($lote in $ordenLotes) {
    if (-not $filas.ContainsKey($lote)) { continue }
    $f = $filas[$lote]
    $niv = [string]$f[0][1]
    $o.Add('| `' + $lote + '` | ' + $niv + ' | ' + $f.Count + ' |')
}
$o.Add('| `fase12-contratos` | 12 | ' + $contratos.Count + ' |')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Gu' + $ic + 'a de migraci' + $oc + 'n r' + $ac + 'pida')
$o.Add('')
$o.Add('### 1. Paquetes NuGet renombrados')
$o.Add('')
$o.Add('Sustituye primero las referencias de paquete; el resto de los renombrados no compilan hasta que esto este hecho.')
$o.Add('')
$o.Add('| Paquete 1.x | Paquete 2.0.0 |')
$o.Add('|---|---|')
$o.Add('| `MoralesLarios.OOFP.WebApi` | `MoralesLarios.OOFP.AspNetCore.Results` |')
$o.Add('| `MoralesLarios.OOFP.WebControllers` | `MoralesLarios.OOFP.AspNetCore.Controllers` |')
$o.Add('| `MoralesLarios.OOFP.WebControllers.Cache` | `MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching` |')
$o.Add('| `MoralesLarios.OOFP.WebServices` | `MoralesLarios.OOFP.Application` |')
$o.Add('| `MoralesLarios.OOFP.EFCore` | `MoralesLarios.OOFP.EntityFrameworkCore` |')
$o.Add('| `MoralesLarios.OOFP.HttpClients` | `MoralesLarios.OOFP.Http` |')
$o.Add('| `MoralesLarios.OOFP.Utilities` | `MoralesLarios.OOFP.Configuration` |')
$o.Add('| `MoralesLarios.OOFP.IO` | `MoralesLarios.OOFP.FileSystem` |')
$o.Add('| `MoralesLarios.OOFP.ValueObjects.IO` | `MoralesLarios.OOFP.ValueObjects.FileSystem` |')
$o.Add('| `MoralesLarios.OOFP.Extensions.Loggers` | `MoralesLarios.OOFP.Logging` |')
$o.Add('| `MoralesLarios.OOFP.Internals` | `MoralesLarios.OOFP.Shared` |')
$o.Add('')
$o.Add('### 2. Renombrados de alto impacto')
$o.Add('')
$o.Add('Los mas frecuentes en codigo de consumidor. Todos tienen puente `[Obsolete]` salvo los marcados ' + $wr + '.')
$o.Add('')
$o.Add('| Antes | Despu' + $ec + 's |')
$o.Add('|---|---|')
$o.Add('| `MlResult.IsValid` | `MlResult.IsSuccess` |')
$o.Add('| `MlResult.IsFail` | `MlResult.IsFailure` |')
$o.Add('| `MlErrorsDetails` ' + $wr + ' | `MlErrorDetails` |')
$o.Add('| `NullToFailed` | `FailIfNull` |')
$o.Add('| `EmptyToFailed` | `FailIfEmpty` |')
$o.Add('| `BoolToResult` | `FailIfFalse` |')
$o.Add('| `Combine` | `CombineAll` |')
$o.Add('| `Projection` / `ProjectionWhile` / `ProjectionSplit` | `Traverse` / `TraverseWhile` / `TraverseSplit` |')
$o.Add('| `SecureValidValue` | `GetValueOrThrow` |')
$o.Add('| `SecureFailErrorsDetails` | `GetFailureDetailsOrThrow` |')
$o.Add('| `FusionFailErros` | `MergeFailureErrors` |')
$o.Add('| `PaginationInfo` ' + $wr + ' | `PageRequest` |')
$o.Add('| `PaginationResultInfo<T>` ' + $wr + ' | `PagedResult<T>` |')
$o.Add('| `IGenServiceFp<TEntity, TDto>` ' + $wr + ' | `ICrudServiceFp<TEntity, TDto>` |')
$o.Add('| `SimpleMlControllerBase<...>` ' + $wr + ' | `MlCrudControllerBase<...>` |')
$o.Add('| `SimpleMlComplexPkControllerBase<...>` ' + $wr + ' | `MlCompositeKeyCrudControllerBase<...>` |')
$o.Add('| `MlActionResults` ' + $wr + ' | `MlResults` |')
$o.Add('| `IEfRepoFp<T>` ' + $wr + ' | `IEfRepository<T>` |')
$o.Add('| `GenClientFp<TDto>` ' + $wr + ' | `RestClientFp<TDto>` |')
$o.Add('| `GenComplexClientFp<TDto>` ' + $wr + ' | `CompositeKeyRestClientFp<TDto>` |')
$o.Add('| `IHttpClientFactoryManager` ' + $wr + ' | `IHttpApiInvoker` |')
$o.Add('| `IWrapperIO` ' + $wr + ' | `IMlFileSystem` |')
$o.Add('| `MlFile` / `MlDirectory` ' + $wr + ' | `FilePath` / `DirectoryPath` |')
$o.Add('')
$o.Add('### 3. Factor' + $ic + 'as de los Value Objects')
$o.Add('')
$o.Add('Convenci' + $oc + 'n 2.0.0: `From...` devuelve `MlResult<T>` (segura) y `CreateFrom...` devuelve el tipo directo (lanza).')
$o.Add('Adem' + $ac + 's los constructores p' + $uc + 'blicos que lanzaban excepci' + $oc + 'n pasan a `protected` (C4), as' + $ic + ' que **hay que usar la factor' + $ic + 'a**.')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Compatibilidad hacia atr' + $ac + 's')
$o.Add('')
$o.Add('La columna **Puente** indica si el nombre antiguo sigue disponible:')
$o.Add('')
$o.Add('- **S' + $ic + '** ' + $fl + ' existe un miembro marcado con `[Obsolete]` que reenv' + $ic + 'a al nombre nuevo.')
$o.Add('  El c' + $oc + 'digo de la 1.x **compila** con una advertencia `CS0618`.')
$o.Add('- **No** ' + $fl + ' el nombre antiguo ha desaparecido: hay que actualizar el c' + $oc + 'digo que lo usaba.')
$o.Add('')
$o.Add('> ' + $wr + ' **Todos los puentes `[Obsolete]` se eliminan en la versi' + $oc + 'n 3.0.0.**')
$o.Add('> Los ficheros que los contienen siguen el patr' + $oc + 'n `*.Obsolete.cs`.')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Cambios de nomenclatura por nivel')
$o.Add('')

foreach ($lote in $ordenLotes) {
    if (-not $filas.ContainsKey($lote)) { continue }
    $f = $filas[$lote]
    $t = [string]$tituloLote[$lote]
    if ([string]::IsNullOrWhiteSpace($t)) { $t = $lote }
    $o.Add('### ' + $t)
    $o.Add('')
    $o.Add('<sub>Lote `' + $lote + '` ' + $fl + ' ' + $f.Count + ' cambios</sub>')
    $o.Add('')
    $o.Add('| # | Tipo | Proyecto | Contenedor | Antes | Despu' + $ec + 's | Puente | Nota |')
    $o.Add('|---:|---|---|---|---|---|:-:|---|')
    $k = 0
    foreach ($c in $f) {
        $k++
        $tipo = Esc $c[2]
        $proy = Esc $c[3]
        if ($proy -eq '-' -or $proy -eq '') { $proy = '' } else { $proy = $proy.Replace('MoralesLarios.OOFP.', '') }
        $cont = Esc $c[4]
        if ($cont -ne '') { $cont = '`' + $cont + '`' }
        $antes = Cod $c[5]
        $desp  = Cod $c[6]
        $pu = ''
        if ($c.Count -gt 7) { if ([string]$c[7] -eq '1') { $pu = 'S' + $ic } else { $pu = 'No' } }
        $nota = ''
        if ($c.Count -gt 9) { $nota = Esc $c[9] }
        $o.Add('| ' + $k + ' | ' + $tipo + ' | ' + $proy + ' | ' + $cont + ' | ' + $antes + ' | ' + $desp + ' | ' + $pu + ' | ' + $nota + ' |')
    }
    $o.Add('')
}

$o.Add('---')
$o.Add('')
$o.Add('## Cambios de contrato (Fase 12)')
$o.Add('')
$o.Add('Estos cambios **no son renombrados**: modifican la forma del API p' + $uc + 'blico')
$o.Add('(visibilidad, mutabilidad, anulabilidad, asincron' + $ic + 'a o comportamiento).')
$o.Add('Se listan aparte porque su **impacto** en el c' + $oc + 'digo consumidor es distinto.')
$o.Add('')
$o.Add('| # | Proyecto | Elemento | Antes | Despu' + $ec + 's | Impacto |')
$o.Add('|---:|---|---|---|---|---|')
$k = 0
foreach ($c in $contratos) {
    $k++
    $proy = (Esc $c[3]).Replace('MoralesLarios.OOFP.', '')
    $cont = Esc $c[4]
    $cont = $cont.Replace('MoralesLarios.OOFP.', '')
    if ($cont -ne '') { $cont = '`' + $cont + '`' }
    $antes = Cod $c[5]
    $desp  = Cod $c[6]
    $pu = ''
    if ($c.Count -gt 7) { if ([string]$c[7] -eq '1') { $pu = '**Puente `[Obsolete]`**: ' } else { $pu = '**Ruptura**: ' } }
    $nota = ''
    if ($c.Count -gt 9) { $nota = Esc $c[9] }
    $o.Add('| ' + $k + ' | ' + $proy + ' | ' + $cont + ' | ' + $antes + ' | ' + $desp + ' | ' + $pu + $nota + ' |')
}
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Cambios planificados no aplicados')
$o.Add('')
$o.Add('**No queda ning' + $uc + 'n renombrado planificado sin aplicar**: los once niveles del plan')
$o.Add('(Nivel 0 a Nivel 10) y la fase de cierre se ejecutaron por completo.')
$o.Add('')
$o.Add('Se dej' + $oc + ' **deliberadamente fuera** del alcance de la 2.0.0 lo siguiente:')
$o.Add('')
$o.Add('| Elemento | Motivo |')
$o.Add('|---|---|')
$o.Add('| Documentos de `Temas Pendientes/` | Son documentos **hist' + $oc + 'ricos** de an' + $ac + 'lisis: deben conservar la nomenclatura anterior para que el razonamiento siga siendo legible. |')
$o.Add('| Retirada de los puentes `[Obsolete]` | Se mantienen durante toda la rama 2.x. Su eliminaci' + $oc + 'n est' + $ac + ' planificada para la **3.0.0**. |')
$o.Add('| Carpetas de proyectos antiguos vac' + $ic + 'as | Se eliminaron del ' + $ac + 'rbol de trabajo: eran restos de los `git mv` del Nivel 0 y no conten' + $ic + 'an ning' + $uc + 'n fichero versionado. |')
$o.Add('| Los 6 tests de integraci' + $oc + 'n de `Http.Tests.Integration` | Fallan **desde antes** de este trabajo porque exigen un servidor HTTP vivo. No se tocaron: son un asunto de infraestructura de pruebas, no de nomenclatura. |')
$o.Add('')
$o.Add('Las decisiones abiertas (`D-1` a `D-11`) y el detalle de lo que queda para la')
$o.Add('siguiente versi' + $oc + 'n est' + $ac + 'n documentados en')
$o.Add('[`Temas Pendientes/Plan-Renombrados-2.0.0.md`](../Temas%20Pendientes/Plan-Renombrados-2.0.0.md).')
$o.Add('')
$o.Add('---')
$o.Add('')
$o.Add('## Validaci' + $oc + 'n')
$o.Add('')
$o.Add('Estado tras aplicar **todos** los cambios de este documento:')
$o.Add('')
$o.Add('| Comprobaci' + $oc + 'n | Resultado |')
$o.Add('|---|---|')
$o.Add('| `dotnet build MoralesLarios.OOFP.sln` | ' + $ok + ' **0 errores** |')
$o.Add('| Tests unitarios | ' + $ok + ' **1 027 / 1 027** |')
$o.Add('| Tests de integraci' + $oc + 'n (`Http.Tests.Integration`) | 27 / 33 ' + $fl + ' 6 fallos **preexistentes** (requieren un servidor HTTP vivo) |')
$o.Add('| Enlaces relativos entre `.md` | ' + $ok + ' 1 444 revisados, **0 rotos** |')
$o.Add('| Anclas internas de los `.md` | ' + $ok + ' 826 revisadas, **0 rotas** (eran 57 rotas) |')
$o.Add('| Codificaci' + $oc + 'n de los `.md` | ' + $ok + ' UTF-8 v' + $ac + 'lido, **0 caracteres de reemplazo** |')
$o.Add('')
$o.Add('> Los 6 fallos de integraci' + $oc + 'n coinciden **exactamente** con la l' + $ic + 'nea base previa')
$o.Add('> registrada en [`_linea-base.txt`](./_linea-base.txt), por lo que **no** son regresiones.')
$o.Add('')

[System.IO.File]::WriteAllLines($dst, $o, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('lineas escritas = ' + $o.Count + '  cambios = ' + $total)
