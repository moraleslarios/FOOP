$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $src
$r = Join-Path $PSScriptRoot 'reemplazar-texto.ps1'
$cs = '*.cs'
# Nombres de #region con nombres antiguos
$pairs = @(
    @('BindSaveValueInDetailsIfFaildFuncResultAsync', 'BindSaveValueInDetailsIfFailedFuncResultAsync'),
    @('ChangeReturnResultAlwaisValid', 'ChangeReturnResultAlwaysValid'),
    @('ChangeReturnResultAlwaisFail',  'ChangeReturnResultAlwaysFail'),
    @('SecureValidValue', 'GetValueOrThrow'),
    @('EmptyToFailed', 'FailIfEmpty'), @('NullToFailed', 'FailIfNull'), @('BoolToResult', 'FailIfFalse'),
    @('FusionFailErros', 'MergeFailureErrors'),
    @('AddDetailValue', 'AddValueMetadata'), @('HasValueDetails', 'ContainsValueMetadata'), @('HasExceptionDetails', 'ContainsException')
)
foreach ($p in $pairs) { & $r -Include $cs -Pattern ('(?<=#(end)?region\s+)' + $p[0] + '\b') -Replacement $p[1] }
# Literales y mensajes
& $r -Include $cs -Pattern 'Cannot obtain the MlErrorsDetails'                       -Replacement 'Cannot obtain the MlErrorDetails'
& $r -Include $cs -Pattern 'Use ValidateWithFluentValidations instead\.'             -Replacement 'Use ValidateWithFluentValidation instead.'
& $r -Include $cs -Pattern 'Use ValidateWithFluentValidationsAsync instead\.'        -Replacement 'Use ValidateWithFluentValidationAsync instead.'
& $r -Include $cs -Pattern 'Add(Scoped|Singleton|Transient)OOFPRepos(?=[^\w(])'       -Replacement 'AddOofpRepositories$1'
& $r -Include $cs -Pattern 'AddOofpRepositoriesScoped'                               -Replacement 'AddOofpRepositories'
