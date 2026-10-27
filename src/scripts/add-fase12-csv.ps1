$ErrorActionPreference = 'Stop'
$csv = Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'Cambios Realizados\_registro.csv'
$enc = New-Object System.Text.UTF8Encoding($true)
$txt = [System.IO.File]::ReadAllText($csv, [System.Text.Encoding]::UTF8)
$antes = ($txt -split "`n").Count
Write-Output ('lineas antes = ' + $antes)

$filas = @(
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.ValueObjects;DecimalNonNegative;public static decimal MinAllowedValue { get; private set; };private const decimal MinAllowedValue;1;0;C3: el limite deja de ser estado mutable publico. El puente estatico limit sigue existiendo en DecimalNonNegative.Obsolete.cs.'
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.ValueObjects;DoubleNonNegative;public static double MinAllowedValue { get; private set; };private const double MinAllowedValue;1;0;C3: el limite deja de ser estado mutable publico. El puente estatico limit sigue existiendo en DoubleNonNegative.Obsolete.cs.'
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.ValueObjects;Age;public Age(int value);protected Age(int value);0;0;C4: el constructor lanzaba excepcion. Se usa la factoria FromInt/ByInt.'
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.ValueObjects;Id;public Id(double value);protected Id(double value);0;0;C4: el constructor lanzaba excepcion. Se usa la factoria FromDouble/ByDouble.'
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.ValueObjects;IdLite;public IdLite(int value);protected IdLite(int value);0;0;C4: el constructor lanzaba excepcion. Se usa la factoria FromInt/ByInt.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.Validation;MoralesLarios.OOFP.Validation;abstract class MlSelfValidatable<T>;interface IMlSelfValidatable<T>;1;0;C5: la autovalidacion pasa a interfaz para no consumir la unica clase base del consumidor. La clase abstracta queda como puente Obsolete implementando la interfaz.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;public class MlCrudControllerBase<TEntity, TDto, TPk>;public abstract class MlCrudControllerBase<TEntity, TDto, TPk>;0;0;C10: las clases base de controlador pasan a abstractas para que ASP.NET no las descubra como controladores.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;public class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>;public abstract class MlCrudControllerBase<TEntity, TRequest, TResponse, TPk>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;public class MlCompositeKeyCrudControllerBase<TEntity, TDto>;public abstract class MlCompositeKeyCrudControllerBase<TEntity, TDto>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Controllers;public class MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>;public abstract class MlCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;public class MlCachedCrudControllerBase<TEntity, TDto, TPk>;public abstract class MlCachedCrudControllerBase<TEntity, TDto, TPk>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;public class MlCachedCrudControllerBase<TEntity, TRequest, TResponse, TPk>;public abstract class MlCachedCrudControllerBase<TEntity, TRequest, TResponse, TPk>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;public class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>;public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TDto>;0;0;C10.'
 'fase12-contratos;12;Tipo;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers;public class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>;public abstract class MlCachedCompositeKeyCrudControllerBase<TEntity, TRequest, TResponse>;0;0;C10.'
 'fase12-contratos;12;Metodo;MoralesLarios.OOFP.Http;MoralesLarios.OOFP.Http.Helpers.MlResponseWebExtensions;ToResponseErrorsDescriptionAsync;ReadErrorDescriptionAsync;1;5;C11: el nombre no indicaba que lee el cuerpo de la respuesta.'
 'fase12-contratos;12;Metodo;MoralesLarios.OOFP.Http;MoralesLarios.OOFP.Http.Helpers.MlResponseWebExtensions;ToResponseErrorsDescription (sincrono, usaba .Result);ReadErrorDescriptionAsync (unica version, asincrona);1;5;C11: se elimina la version sincrona que bloqueaba con .Result; las 5 llamadas de HttpApiInvoker pasan a await.'
 'fase12-contratos;12;Metodo;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.RegisterServices;AddMlControllers (no-op sin marcar);AddMlControllers [Obsolete] no-op;1;0;C12: no registraba nada; se marca Obsolete para que el consumidor retire la llamada. El puente AddWebControllers ya no delega.'
 'fase12-contratos;12;Miembro;MoralesLarios.OOFP.AspNetCore.Controllers;CompositeKeyParameterAttribute;public string Description (string description = null!);public string? Description (string? description = null);0;0;C14: anulabilidad correcta en lugar del supresor null!.'
 'fase12-contratos;12;Comportamiento;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Helpers.Extensions;DateTime.TryParse/TryParseExact con CurrentCulture;con CultureInfo.InvariantCulture;0;0;C16: el parseo de claves de URL dependia de la cultura del servidor.'
)

$nuevo = $txt.TrimEnd("`r", "`n") + "`r`n" + ($filas -join "`r`n") + "`r`n"
[System.IO.File]::WriteAllText($csv, $nuevo, $enc)
$txt2 = [System.IO.File]::ReadAllText($csv, [System.Text.Encoding]::UTF8)
Write-Output ('lineas despues = ' + ($txt2 -split "`n").Count)
Write-Output ('filas anadidas = ' + $filas.Count)
