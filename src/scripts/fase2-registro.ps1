$ErrorActionPreference = 'Stop'
$reg = Join-Path (Resolve-Path (Join-Path $PSScriptRoot '..')).Path 'Cambios Realizados\_registro.csv'
$rows = @(
 'fase2-nivel1-erratas;1;Texto;MoralesLarios.OOFP.AspNetCore.Controllers;Helpers/Extensions.cs;"The type {0} is not soported";"The type {0} is not supported";0;3;Mensaje visible en el cuerpo del error HTTP',
 'fase2-nivel1-erratas;1;Texto;MoralesLarios.OOFP.AspNetCore.Controllers;SimpleMl*ControllerBase;"... or is diferent type to PK of ...";"... or is of a different type than PK of ...";0;3;Mensaje visible (404)',
 'fase2-nivel1-erratas;1;Texto;MoralesLarios.OOFP.Validation.*;*Validator.cs;"{x} no be null";"{x} must not be null";0;6;',
 'fase2-nivel1-erratas;1;Texto;MoralesLarios.OOFP.Validation.*;*Validator.cs;"{x} no be empty";"{x} must not be empty";0;3;',
 'fase2-nivel1-erratas;1;Texto;MoralesLarios.OOFP.AspNetCore.Controllers;SimpleMl*ControllerBase;"{x} isn''t null";"{x} must not be null";0;13;El mensaje anterior afirmaba lo contrario de lo ocurrido',
 'fase2-nivel1-erratas;1;Comentario;MoralesLarios.OOFP.Http;HttpClientFactoryManager;BaseAdress;BaseAddress;0;10;Solo aparecia en comentarios',
 'fase2-nivel1-erratas;1;Metodo;MoralesLarios.OOFP.ValueObjects;Id (Id.cs excluido de la compilacion);Bydouble / Fromdouble;ByDouble / FromDouble;0;2;Codigo muerto: <Compile Remove="Id.cs" />',
 'fase2-nivel1-erratas;1;Documento;docs;Bind;11_BindSaveValueInDetailsIfFaildFuncResultAsync.md;11_BindSaveValueInDetailsIfFailedFuncResultAsync.md;0;0;',
 'fase2-nivel1-erratas;1;Metodo;-;-;TryMapIAsyncf;TryMapIfAsync;0;0;Ya corregido en la revision Map/Bind (commit 1f5f910)'
)
[System.IO.File]::AppendAllLines($reg, [string[]]$rows, (New-Object System.Text.UTF8Encoding($false)))
Write-Output "$($rows.Count) filas anadidas"
