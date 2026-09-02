$apiKey = $env:NUGET_API_KEY
$source = "https://api.nuget.org/v3/index.json"

if ([string]::IsNullOrWhiteSpace($apiKey)) {
    throw "Set the NUGET_API_KEY environment variable before running this script."
}

dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.EFCore.WebApi\bin\Release\MoralesLarios.OOFP.EFCore.WebApi.1.0.3.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.HttpClients\bin\Release\MoralesLarios.OOFP.HttpClients.1.0.15.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Internals\bin\Release\MoralesLarios.OOFP.Internals.1.0.3.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.IO\bin\Release\MoralesLarios.OOFP.IO.1.0.1.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Shared\bin\Release\MoralesLarios.OOFP.Shared.1.0.0.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Utilities\bin\Release\MoralesLarios.OOFP.Utilities.1.0.3.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Validation.Dataannotations\bin\Release\MoralesLarios.OOFP.Validation.Dataannotations.1.0.5.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Validation.FluentValidations\bin\Release\MoralesLarios.OOFP.Validation.FluentValidations.1.0.1.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.Validation\bin\Release\MoralesLarios.OOFP.Validation.1.0.1.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.ValueObjects.IO\bin\Release\MoralesLarios.OOFP.ValueObjects.IO.1.0.4.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.ValueObjects\bin\Release\MoralesLarios.OOFP.ValueObjects.1.0.12.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.WebApi\bin\Release\MoralesLarios.OOFP.WebApi.1.0.12.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.WebControllers.Cache\bin\Release\MoralesLarios.OOFP.WebControllers.Cache.1.0.4.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.WebControllers\bin\Release\MoralesLarios.OOFP.WebControllers.1.0.6.nupkg --api-key $apiKey --source $source
dotnet nuget push C:\Git\MoralesLarios\FOOP\MoralesLarios.FOOP\src\MoralesLarios.OOFP.WebServices\bin\Release\MoralesLarios.OOFP.WebServices.1.0.11.nupkg --api-key $apiKey --source $source
