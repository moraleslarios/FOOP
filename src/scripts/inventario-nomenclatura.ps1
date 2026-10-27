$ErrorActionPreference = 'Stop'

$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$out  = Join-Path $env:TEMP 'inventario-nomenclatura.txt'

# Nivel|Termino|Regex  (regex vacio => \bTermino\b)
$defs = @'
1|FusionFailErros|
1|FusionErrosIfExists|
1|ChangeReturnResultAlwais|\bChangeReturnResultAlwais
1|TryChangeReturnResultAlwais|\bTryChangeReturnResultAlwais
1|TryMapIAsyncf|
1|MlErrorsDetails|
1|FromStringLenght|
1|FromIntLenght|
1|MinLenght|
1|MaxLenght|
1|Lenght (cualquier)|Lenght
1|Bydouble|
1|Fromdouble|
1|ExistDirectory|
1|AddScopedtGenServicesFpWithoutReposGeneral|
1|AddScopedtGenServicesDuplexFpWithoutReposGeneral|
1|Scopedt (cualquier)|Scopedt
1|TEnumrableResponse|
1|BaseAdress|BaseAdress
1|IfFaild (cualquier)|Faild
1|IfFailDiferentTypes|Diferent
1|"soported"|soported
1|"diferent type"|diferent type
1|"no be null"|no be null
1|"no be empty"|no be empty
1|"isn't null"|isn't null
2|IsValid|
2|IsFail|
2|SecureValidValue|
2|ErrorsDetails (miembro)|\.ErrorsDetails\b
2|.Details|\.Details\b
2|ToErrorsMessages|
2|ToErrorsDescription|
2|ToDetailsDescription|
2|HasKeyDetails|
2|HasValueDetails|
2|HasExceptionDetails|
2|GetDetailValue|\bGetDetailValue
2|GetDetailException|\bGetDetailException
2|AddDetail|
2|EX_DESC_KEY|
2|VALUE_KEY|
2|MlResultBucles|
2|Projection|\bProjection
2|MlResultTransformations|
2|MlResultActionsMap|
2|EmptyToFailed|\bEmptyToFailed
2|NullToFailed|\bNullToFailed
2|BoolToResult|\bBoolToResult
2|Do (metodo)|\bDo(Async)?\s*[<(]
2|Combine|\bCombine(Async)?\s*[<(]
3|class Key|(class|record|struct)\s+Key\b
3|class Name|(class|record|struct)\s+Name\b
3|class Empty|(class|record|struct)\s+Empty\b
3|limit|\blimit\b
3|NotNegative (cualquier)|NotNegative
3|RangeEnumValueObject|
3|StringBetweenLength|
3|ByString|
3|ById / By* factorias|\.By[A-Z]\w*\s*\(
3|ExistsFile|
3|MlFile|
3|MlDirectory|
3|EndpointPattern|
4|Dataannotations|Dataannotations
4|FluentValidations|FluentValidations
4|DataannotationsValidator|
4|MlValidableFp|
4|ValidateObject|
4|IValidatorFp|
4|BuildErrorMessage|
5|Internals|OOFP\.Internals
5|PaginationInfo|
5|PaginationResultInfo|
5|ProblemDetailsInfo|
5|IMlConfigManager|
5|MlFileHelper|
5|MlFileUtils|
5|ReadAllTextFp|
5|GeneralExtensionLoggers|\bGeneralExtensionLoggers
5|LogMlResultFinalAsync|
6|EFRepoBase|
6|EFRepoBaseFp|
6|EFRepo (cualquier)|\bI?EFRepo
6|EFRepoReaderFp|
6|EFRepoWriterFp|
6|EFRepoUpdaterFp|
6|EFRepoDeleterFp|
6|EFRepoReaderPaginationFp|
6|IEFRepoWriterFp|
6|ResolveRepoFp|
6|AddSingletonOOFPRepos|\bAddSingletonOOFPRepos
6|AddScopedOOFPRepos|\bAddScopedOOFPRepos
6|enum OrderBy|enum\s+OrderBy\b
6|OrderBy.Asc/Desc|OrderBy\.(Asc|Desc)\b
6|TryGetInternalData|
6|TryLast|\bTryLast
6|_pkFields|
6|JfCatasDbContext|
7|IGenServiceFp|
7|GenServiceFp|
7|GenServiceDuplexFp|
7|MlProblemsDetails|
7|UpdateProblemDetailsAsync|
7|BuildNotFoundPkError|
7|AllAsync|
7|validMessageBuilder|
7|AddScopedtGenServices|\bAddScopedtGenServices
8|MlResultWebExtensions|
8|MlResultWebExtensionsPlus|
8|ToPostPdActionResult|\bToPostPdActionResult
8|ToSimpleRepoPostActionResult|\bToSimpleRepoPostActionResult
8|MlActionResults|
8|ToProblemsDetailsInfo|
8|ContieneCombinacion|
8|notFoundKeys|
8|"X-Page-Number"|X-Page-Number
8|"X-Page-Size"|X-Page-Size
8|MlRequestWebExtensions|
9|SimpleMlControllerBase|
9|SimpleMlComplexPkControllerBase|
9|Duplex (cualquier)|Duplex
9|GetPkValues|
9|GetPkValuesString|
9|ConverterTo|
9|ConvertDateTime|
9|GetPkValuesErrorMessage|
9|AddWebControllers|
9|PkParameterAttribute|\bPkParameter(Attribute)?\b
9|"id-str"|id-str
9|BypassHeader|
9|"X-Bypass-Cache"|X-Bypass-Cache
9|PerControllerOutputCachePolicy|
10|IGenClientFp|
10|GenClientFp|
10|IGenComplexClientFp|
10|GenComplexClientFp|
10|GenClientDuplexFp / Gen*Duplex*|\bI?Gen\w*Duplex\w*
10|IHttpClientFactoryManager|
10|GetHttpClientFactoryKey|
10|PostGetAsync|
10|PostGetPaginationAsync|
10|GetPaginationAsync|
10|InternalGetUrl|
10|InternalPostGetAsync|
10|SetHeaderInfo|
10|SetHeaderPageNumber|
10|SetHeaderPageSize|
10|ToResponseErrorsDescription|
10|SetHeaders|
10|CallRequestParamsInfo|
10|CallRequestPaginationParamsInfo|
10|AddGenClientFp|
10|AddGenClientComplexFp|
10|AddGenClientDuplexComplexFp|
10|httpClientFactoryKey|
10|configureHttpClientKey|
'@

$csFiles = Get-ChildItem -Path $root -Recurse -File -Include *.cs |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs)\\' }

$mdFiles = Get-ChildItem -Path $root -Recurse -File -Include *.md |
    Where-Object { $_.FullName -notmatch '\\(bin|obj|\.git|\.vs|Temas Pendientes|Cambios Realizados)\\' }

$cache = @{}
foreach ($f in $csFiles + $mdFiles) { $cache[$f.FullName] = [System.IO.File]::ReadAllText($f.FullName) }

function Get-Project([string]$full) {
    $rel = $full.Substring($root.Length + 1)
    $top = $rel.Split('\')[0]
    return ($top -replace '^MoralesLarios\.(OOFP|FOOP)\.?', '')
}

$rows = foreach ($line in ($defs -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $p = $line.Split([char[]]'|', 3)
    $lvl = $p[0]; $term = $p[1]; $rx = $p[2]
    if ([string]::IsNullOrEmpty($rx)) { $rx = '\b' + [regex]::Escape($term) + '\b' }

    $code = 0; $tests = 0; $docs = 0
    $projs = New-Object System.Collections.Generic.SortedSet[string]
    $nFiles = 0

    foreach ($f in $csFiles) {
        $n = [regex]::Matches($cache[$f.FullName], $rx).Count
        if ($n -eq 0) { continue }
        $nFiles++
        $proj = Get-Project $f.FullName
        [void]$projs.Add($proj)
        if ($proj -match 'Tests?($|\.)') { $tests += $n } else { $code += $n }
    }
    foreach ($f in $mdFiles) {
        $docs += [regex]::Matches($cache[$f.FullName], $rx).Count
    }

    [pscustomobject]@{
        N     = $lvl
        Term  = $term
        Code  = $code
        Tests = $tests
        Docs  = $docs
        Files = $nFiles
        Proj  = ($projs -join ',')
    }
}

$sb = New-Object System.Text.StringBuilder
foreach ($r in $rows) {
    [void]$sb.AppendLine(('{0,2} | {1,-48} | {2,5} | {3,5} | {4,4} | {5,3} | {6}' -f $r.N, $r.Term, $r.Code, $r.Tests, $r.Docs, $r.Files, $r.Proj))
}

[void]$sb.AppendLine('')
[void]$sb.AppendLine('=== Rutas Nivel 0 ===')
$paths = @(
    'MoralesLarios.FOOP',
    'MoralesLarios.FOOP\MoralesLarios.FOOP.sln',
    'MoralesLarios.OOFP - copia.sln',
    'MoralesLarios.FOOP\__Doc',
    'MoralesLarios.FOOP\__Doc\Bucle',
    'MoralesLarios.FOOP\__Doc\Types',
    'MoralesLarios.FOOP\__Doc\PendingTasks.txt',
    '..\docs',
    'docs',
    'MoralesLarios.OOFP.EFCore\OopRepos',
    'MoralesLarios.OOFP.EFCore\Repos',
    'MoralesLarios.OOFP.EFCore\Helpers\Constants.cs',
    'MoralesLarios.OOFP.HttpClients\ParamsInfo',
    'MoralesLarios.OOFP.WebServices\Services',
    'MoralesLarios.OOFP.WebServices\Services\GenService.cs',
    'MoralesLarios.OOFP.Internals\Info',
    'MoralesLarios.OOFP.WebControllers.Cache',
    'MoralesLarios.OOFP.Extensions.Loggers.Console.Tests',
    'MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit',
    'MoralesLarios.OOFP.ValueObjects.IO.Test.Unit'
)
foreach ($p in $paths) {
    $full = Join-Path $root $p
    $ok = Test-Path -LiteralPath $full
    $extra = ''
    if ($ok -and (Get-Item -LiteralPath $full).PSIsContainer) {
        $extra = ' (' + @(Get-ChildItem -LiteralPath $full -Recurse -File -ErrorAction SilentlyContinue | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }).Count + ' ficheros)'
    } elseif ($ok) {
        $extra = ' (' + (Get-Item -LiteralPath $full).Length + ' bytes)'
    }
    [void]$sb.AppendLine(('{0,-6} {1}{2}' -f $ok, $p, $extra))
}

[void]$sb.AppendLine('')
[void]$sb.AppendLine('=== csproj ===')
Get-ChildItem -Path $root -Recurse -File -Filter *.csproj |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    ForEach-Object { [void]$sb.AppendLine($_.FullName.Substring($root.Length + 1)) }

[System.IO.File]::WriteAllText($out, $sb.ToString(), (New-Object System.Text.UTF8Encoding($true)))
Write-Output ('OK -> ' + $out + ' (' + $rows.Count + ' terminos)')
