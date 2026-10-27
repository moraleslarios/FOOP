$ErrorActionPreference = 'Stop'
$src = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$reg = Join-Path $src 'Cambios Realizados\_registro.csv'
$header = 'Lote;Nivel;Tipo;Proyecto;Contenedor;Antes;Despues;Puentes;Referencias;Nota'

$rows = @(
 'Solucion;-;MoralesLarios.FOOP/MoralesLarios.FOOP.sln;(eliminada);Solucion duplicada',
 'Solucion;-;MoralesLarios.OOFP - copia.sln;(eliminada);Copia obsoleta de la solucion',
 'Proyecto;-;MoralesLarios.OOFP (carpeta MoralesLarios.FOOP);MoralesLarios.OOFP.Core;Paquete/assembly. El namespace raiz MoralesLarios.OOFP NO cambia (RootNamespace explicito)',
 'Proyecto;-;MoralesLarios.OOFP.Internals;MoralesLarios.OOFP.Shared (fusionado);Shared ya existia: Info/*.cs, GlobalUsings y README pasan a Shared',
 'Proyecto;-;MoralesLarios.OOFP.Utilities;MoralesLarios.OOFP.Configuration;',
 'Proyecto;-;MoralesLarios.OOFP.IO;MoralesLarios.OOFP.FileSystem;',
 'Proyecto;-;MoralesLarios.OOFP.Validation.Dataannotations;MoralesLarios.OOFP.Validation.DataAnnotations;',
 'Proyecto;-;MoralesLarios.OOFP.Validation.FluentValidations;MoralesLarios.OOFP.Validation.FluentValidation;Referencias a FluentValidation.Results cualificadas con global:: (colision con el namespace propio)',
 'Proyecto;-;MoralesLarios.OOFP.WebApi;MoralesLarios.OOFP.AspNetCore.Results;',
 'Proyecto;-;MoralesLarios.OOFP.WebControllers;MoralesLarios.OOFP.AspNetCore.Controllers;',
 'Proyecto;-;MoralesLarios.OOFP.WebControllers.Cache;MoralesLarios.OOFP.AspNetCore.Controllers.OutputCaching;',
 'Proyecto;-;MoralesLarios.OOFP.WebServices;MoralesLarios.OOFP.Application;',
 'Proyecto;-;MoralesLarios.OOFP.HttpClients;MoralesLarios.OOFP.Http;',
 'Proyecto;-;MoralesLarios.OOFP.EFCore;MoralesLarios.OOFP.EntityFrameworkCore;',
 'Proyecto;-;MoralesLarios.OOFP.Extensions.Loggers;MoralesLarios.OOFP.Logging;',
 'Proyecto;-;MoralesLarios.OOFP.ValueObjects.IO;MoralesLarios.OOFP.ValueObjects.FileSystem;Derivado (coherencia con FileSystem)',
 'Proyecto;-;MoralesLarios.OOFP.EFCore.WebApi;MoralesLarios.OOFP.EntityFrameworkCore.Sample;Derivado: es una aplicacion de ejemplo',
 'Proyecto test;-;MoralesLarios.OOFP.Unit.Tests;MoralesLarios.OOFP.Core.Tests.Unit;',
 'Proyecto test;-;MoralesLarios.OOFP.EFCore.Infrastructure.Tests;MoralesLarios.OOFP.EntityFrameworkCore.TestSupport;No contiene tests: es soporte',
 'Proyecto test;-;MoralesLarios.OOFP.EFCore.Integration.Tests;MoralesLarios.OOFP.EntityFrameworkCore.Tests.Integration;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.ValueObjects.IO.Test.Unit;MoralesLarios.OOFP.ValueObjects.FileSystem.Tests.Unit;',
 'Proyecto test;-;MoralesLarios.OOFP.ValueObjects.IO.2.Tests.Unit;(eliminado);Fuera del .sln, no compilaba y no tenia tests: no habia nada que fusionar',
 'Proyecto;-;MoralesLarios.OOFP.Extensions.Loggers.Console.Tests;MoralesLarios.OOFP.Logging.Sample;Es una app de consola, no un proyecto de tests',
 'Proyecto test;-;MoralesLarios.OOFP.Utilities.Tests.Unit;MoralesLarios.OOFP.Configuration.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.Validation.Dataannotations.Tests.Unit;MoralesLarios.OOFP.Validation.DataAnnotations.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.Validation.FluentValidations.Tests.Unit;MoralesLarios.OOFP.Validation.FluentValidation.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.WebApi.Tests.Unit;MoralesLarios.OOFP.AspNetCore.Results.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.WebServices.Tests.Unit;MoralesLarios.OOFP.Application.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.HttpClients.Tests.Unit;MoralesLarios.OOFP.Http.Tests.Unit;Derivado',
 'Proyecto test;-;MoralesLarios.OOFP.HttpClients.Tests.Integration;MoralesLarios.OOFP.Http.Tests.Integration;Derivado',
 'Carpeta;Core;MoralesLarios.FOOP/__Doc/;docs/;Documentacion del nucleo en src/docs',
 'Carpeta;Core;__Doc/Bucle/;docs/loops/;',
 'Carpeta;Core;__Doc/Types/;docs/core-types/;',
 'Fichero;Core;__Doc/PendingTasks.txt;Temas Pendientes/PendingTasks-legacy.md;Pendiente de convertir en issues de GitHub',
 'Carpeta;EntityFrameworkCore;EFCore/OopRepos/;EntityFrameworkCore/Repositories.Imperative/;Namespace ...EFCore.OopRepos -> ...EntityFrameworkCore.Repositories.Imperative',
 'Carpeta;EntityFrameworkCore;EFCore/Repos/;EntityFrameworkCore/Repositories/;Namespace ...EFCore.Repos -> ...EntityFrameworkCore.Repositories',
 'Carpeta;Http;HttpClients/ParamsInfo/;Http/Requests/;Namespace ...HttpClients.ParamsInfo -> ...Http.Requests',
 'Carpeta;Shared;Internals/Info/;Shared/Pagination/;Namespace ...Internals.Info -> ...Shared.Pagination',
 'Fichero;Application;WebServices/Services/GenService.cs;(eliminado);Fichero vacio (0 bytes)',
 'Fichero;EntityFrameworkCore;EFCore/Helpers/Constants.cs;(eliminado);Clase vacia sin uso'
)

$out = New-Object System.Collections.Generic.List[string]
if (-not (Test-Path $reg)) { $out.Add($header) }
foreach ($r in $rows) {
    $p = $r.Split(';')
    $out.Add(('fase1-nivel0;0;{0};{1};;{2};{3};0;;{4}' -f $p[0], $p[1], $p[2], $p[3], $p[4]))
}
[System.IO.File]::AppendAllLines($reg, $out, (New-Object System.Text.UTF8Encoding($true)))
Write-Output ("{0} filas anadidas a {1}" -f $rows.Count, $reg)
