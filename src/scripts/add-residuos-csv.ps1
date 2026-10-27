$csv = 'c:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\Cambios Realizados\_registro.csv'
$before = (Get-Content $csv).Count
$rows = @(
 'fase11d-residuos;1;Parametro;MoralesLarios.OOFP.ValueObjects;MoralesLarios.OOFP.ValueObjects.IntBetween (operador implicito de tupla);minLenght;minLength;0;1;Elemento de tupla del operador implicito'
 'fase11d-residuos;1;Parametro;MoralesLarios.OOFP.ValueObjects;MoralesLarios.OOFP.ValueObjects.IntBetween (operador implicito de tupla);maxLenght;maxLength;0;1;Elemento de tupla del operador implicito'
 'fase11d-residuos;1;Parametro;MoralesLarios.OOFP.ValueObjects;MoralesLarios.OOFP.ValueObjects.StringLengthBetween (operador implicito de tupla);minLenght;minLength;0;1;Elemento de tupla del operador implicito'
 'fase11d-residuos;1;Parametro;MoralesLarios.OOFP.ValueObjects;MoralesLarios.OOFP.ValueObjects.StringLengthBetween (operador implicito de tupla);maxLenght;maxLength;0;1;Elemento de tupla del operador implicito'
 'fase11d-residuos;1;Variable;MoralesLarios.OOFP.ValueObjects.Tests.Unit;MoralesLarios.OOFP.ValueObjects.Tests.Unit.IntLessThanTests;minLenght;minLength;0;4;Variables locales de test'
 'fase11d-residuos;7;Parametro;MoralesLarios.OOFP.AspNetCore.Controllers;MoralesLarios.OOFP.AspNetCore.Controllers.Controllers.MlCompositeKeyCrudControllerBase;_pkFields;_keySelector;0;10;Es una funcion selectora, no una lista de campos'
 'fase11d-residuos;7;Parametro;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching.Controllers.MlCachedCompositeKeyCrudControllerBase;_pkFields;_keySelector;0;4;Es una funcion selectora, no una lista de campos'
 'fase11d-residuos;8;Campo;MoralesLarios.OOFP.AspNetCore.Results;MoralesLarios.OOFP.AspNetCore.Results.Helpers.MlResultActionResultExtensionsLegacy;notFoundKeys;NotFoundDetailKeys;0;2;Campo estatico pasado a readonly y PascalCase'
 'fase11d-residuos;10;Literal;MoralesLarios.OOFP.Http;MoralesLarios.OOFP.Http.HttpApiInvoker.GetPagedAsync;"GetPaginationAsync<...>";"GetPagedAsync<...>";0;1;Literal de traza de log desincronizado con el nombre del metodo'
)
Add-Content -Path $csv -Value $rows -Encoding UTF8
$after = (Get-Content $csv).Count
Write-Output "ANTES=$before DESPUES=$after DELTA=$($after-$before)"
