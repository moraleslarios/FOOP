# MoralesLarios.OOFP.IO

Wrapper funcional para operaciones básicas de entrada/salida (`IO`) sobre ficheros y directorios.

## Qué hace

- Encapsula operaciones de `System.IO` devolviendo `MlResult<T>`.
- Usa `MoralesLarios.OOFP.ValueObjects.IO` para validar rutas y existencia.
- Ofrece registro DI sencillo con `AddOOFPIO()`.

## Dependencias

- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `MoralesLarios.OOFP`
- `MoralesLarios.OOFP.ValueObjects.IO`

## Estructura

- `IWrapperIO.cs`
- `WrapperIO.cs`
- `RegisterServices.cs`

## `IWrapperIO`

Métodos:

- `EnumerateFiles(string directoryStr)` -> `MlResult<IEnumerable<ExistsFile>>`
- `ReadAllText(string filePathStr)` -> `MlResult<string>`
- `ReadAllLines(string filePathStr)` -> `MlResult<IEnumerable<string>>`

## `WrapperIO`

Implementación:

- `EnumerateFiles`: valida directorio con `ExistDirectory.ByString`, lista ficheros y los transforma a `ExistsFile`.
- `ReadAllText`: valida fichero con `ExistsFile.ByString` y ejecuta `File.ReadAllText`.
- `ReadAllLines`: valida fichero y ejecuta `File.ReadLines`.

Ejemplo:

```csharp
var text = _wrapperIO.ReadAllText(@"C:\tmp\input.txt");

var files = _wrapperIO.EnumerateFiles(@"C:\tmp\");
```

## Registro DI

```csharp
services.AddOOFPIO();
```

Registra `IWrapperIO` como `Singleton`.
