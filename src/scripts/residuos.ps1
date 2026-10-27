$src = 'c:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src'
Set-Location $src
$cs = Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj|scripts)\\' }
$noObs = $cs | Where-Object { $_.Name -notlike '*.Obsolete.cs' }

$terms = @(
 'FusionFailErros','FusionErrosIfExists','ChangeReturnResultAlwais','FromStringLenght','FromIntLenght',
 'MinLenght','AddScopedtGenServicesFpWithoutReposGeneral','AddScopedtGenServicesDuplexFpWithoutReposGeneral',
 'IfFailDiferentTypes','SecureValidValue','ToErrorsMessages','ToErrorsDescription','ToDetailsDescription',
 'HasKeyDetails','HasValueDetails','HasExceptionDetails','GetDetailValue','GetDetailException','AddDetail',
 'EX_DESC_KEY','VALUE_KEY','Projection','EmptyToFailed','NullToFailed','BoolToResult',
 'EndpointPattern','BuildNotFoundPkError','ToPostPdActionResult','ToSimpleRepoPostActionResult',
 'ToProblemsDetailsInfo','notFoundKeys','GetPkValues','ConverterTo','GetPkValuesErrorMessage',
 'AddWebControllers','BypassHeader','GetHttpClientFactoryKey','GetPaginationAsync','SetHeaderInfo',
 'SetHeaderPageNumber','SetHeaderPageSize','ToResponseErrorsDescription','SetHeaders',
 'AddGenClientFp','AddGenClientComplexFp','AddGenClientDuplexComplexFp','ResolveRepoFp',
 'AddSingletonOOFPRepos','AddScopedOOFPRepos','LogMlResultFinalAsync','ValidateObject','IsFail','_pkFields'
)
foreach ($t in $terms) {
  $hits = $noObs | Select-String -Pattern ('\b' + [regex]::Escape($t) + '\b')
  if ($hits) {
    Write-Output "### $t  (fuera de *.Obsolete.cs: $($hits.Count))"
    $hits | Select-Object -First 6 | ForEach-Object {
      $l = ($_.Line.Trim() -replace '\s+',' ')
      if ($l.Length -gt 120) { $l = $l.Substring(0,120) }
      '    {0}:{1}  {2}' -f $_.Filename, $_.LineNumber, $l
    }
  }
}
