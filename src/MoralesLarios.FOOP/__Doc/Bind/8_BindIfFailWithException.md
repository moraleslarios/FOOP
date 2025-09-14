# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos con Excepción Previa

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Diferencias con BindIfFailWithValue](#diferencias-con-bindiffailwithvalue)
4. [Variantes de BindIfFailWithException](#variantes-de-bindiffailwithexception)
5. [Variantes de TryBindIfFailWithException](#variantes-de-trybindiffailwithexception)
6. [Patrones de Uso](#patrones-de-uso)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindIfFailWithException** de `MlResultActionsBind` proporciona operaciones especializadas para el manejo de errores que tienen acceso a una **excepción previa almacenada** en los detalles del error. Estas operaciones implementan patrones de **recuperación basada en excepciones**, **análisis de errores específicos**, y **manejo contextual de fallos**, permitiendo estrategias de recuperación que pueden analizar y responder al tipo específico de excepción que causó el fallo.

### Propósito Principal

- **Recuperación Específica por Excepción**: Estrategias de recuperación basadas en el tipo de excepción
- **Análisis de Fallos**: Inspección detallada de excepciones para determinar causa raíz
- **Manejo Contextual**: Acceso a información rica de la excepción original
- **Escalado Inteligente**: Decisiones de escalado basadas en tipos de excepción

### Concepto Clave: Excepción en Detalles de Error

El sistema `MlResult` permite almacenar excepciones en los detalles del error cuando ocurre un fallo debido a una excepción. Esto permite que las funciones de recuperación tengan acceso completo a la excepción original, incluyendo stack trace, mensaje, y tipo específico.

---

## Análisis de la Funcionalidad

### Filosofía de BindIfFailWithException

```
MlResult<T> → ¿Es Fallo?
              ├─ Sí → ¿Tiene Excepción Almacenada?
              │       ├─ Sí → Función de Recuperación(Exception) → MlResult<T>
              │       └─ No → Retorna Fallo Original (sin cambios)
              └─ No → Retorna Valor Original (sin cambios)
```

### Comportamiento Crítico: Diferencias con BindIfFailWithValue

**Diferencia Fundamental en Comportamiento:**

1. **BindIfFailWithValue**: 
   - Si recibe un `MlResult` Fail **sin** ValueDetail → Añade un nuevo Error al que viene de la ejecución anterior
   - Siempre intenta procesar, incluso sin valor previo

2. **BindIfFailWithException**: 
   - Si recibe un `MlResult` Fail **sin** ExceptionDetail → Devuelve el `MlResult` Fail **igual que le vino**
   - **NO** ejecuta la función de recuperación si no hay excepción almacenada

### Flujo de Procesamiento

1. **Si el resultado fuente es exitoso**: Retorna el valor sin cambios
2. **Si el resultado fuente es fallido**: 
   - Intenta extraer la excepción almacenada usando `errorsDetails.GetDetailException()`
   - **Si HAY excepción almacenada**: Ejecuta la función de recuperación con esa excepción
   - **Si NO hay excepción almacenada**: Retorna el error original sin modificaciones (comportamiento clave)

### Tipos de Operaciones

1. **BindIfFailWithException Simple**: Recuperación solo cuando hay excepción previa
2. **BindIfFailWithException con Transformación**: Manejo de éxito y fallo con posible cambio de tipo
3. **TryBindIfFailWithException**: Versiones seguras que capturan excepciones en las funciones de recuperación

---

## Diferencias con BindIfFailWithValue

### Tabla Comparativa de Comportamientos

| Escenario | BindIfFailWithValue | BindIfFailWithException |
|-----------|-------------------|------------------------|
| **Resultado Exitoso** | Retorna valor sin cambios | Retorna valor sin cambios |
| **Fallo CON dato almacenado** | Ejecuta función con valor previo | Ejecuta función con excepción |
| **Fallo SIN dato almacenado** | ⚠️ **Añade nuevo error** | ✅ **Retorna fallo original** |
| **Función de recuperación falla** | Propaga nuevo error | Puede mergear errores |

### Ejemplos de Comportamiento Diferencial

```csharp
// Caso 1: Fallo sin datos almacenados
var failWithoutValue = MlResult<string>.Fail("Simple error"); // Sin valor ni excepción

// BindIfFailWithValue - Añade nuevo error
var resultWithValue = failWithoutValue.BindIfFailWithValue(prevValue => 
    MlResult<string>.Valid("recovered")); // Se ejecuta, puede añadir error

// BindIfFailWithException - Retorna original sin cambios
var resultWithException = failWithoutValue.BindIfFailWithException(ex => 
    MlResult<string>.Valid("recovered")); // NO se ejecuta, retorna fallo original

// Caso 2: Fallo con datos almacenados
var failWithException = MlResult<string>.Fail("Error with exception", 
    detailException: new InvalidOperationException("Original error"));

// BindIfFailWithException - Ejecuta función de recuperación
var resultWithStoredException = failWithException.BindIfFailWithException(ex => 
{
    // ex es InvalidOperationException
    return ex switch
    {
        InvalidOperationException => MlResult<string>.Valid("Recovered from InvalidOperation"),
        ArgumentException => MlResult<string>.Valid("Recovered from Argument"),
        _ => MlResult<string>.Fail("Cannot recover from this exception type")
    };
}); // Se ejecuta la función
```

---

## Variantes de BindIfFailWithException

### Variante 1: BindIfFailWithException Simple

**Propósito**: Recuperación usando la excepción previa almacenada en el error

```csharp
public static MlResult<T> BindIfFailWithException<T>(this MlResult<T> source,
                                                     Func<Exception, MlResult<T>> funcException)
```

**Parámetros**:
- `source`: El resultado que puede contener una excepción en caso de fallo
- `funcException`: Función que recibe la excepción y retorna un nuevo resultado

**Comportamiento**:
- Si `source` es exitoso: Retorna el valor sin cambios
- Si `source` es fallido CON excepción: Ejecuta `funcException(storedException)`
- Si `source` es fallido SIN excepción: Retorna el error original **sin cambios**

**Flujo Interno**:
```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(), // Sin excepción: retorna original
        valid: funcException                                           // Con excepción: ejecuta función
    ),
    valid: value => value
)
```

### Variante 2: BindIfFailWithException con Transformación de Tipos

**Propósito**: Manejo completo con acceso a excepción previa y posibilidad de cambiar tipos

```csharp
public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<Exception, MlResult<TReturn>> funcFail)
```

**Parámetros**:
- `source`: El resultado fuente
- `funcValid`: Función para manejar valores exitosos
- `funcFail`: Función para manejar fallos usando excepción previa

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `funcValid(value)`
- Si `source` es fallido CON excepción: Ejecuta `funcFail(storedException)`
- Si `source` es fallido SIN excepción: Retorna error original convertido al nuevo tipo

### Soporte Asíncrono Completo

Ambas variantes incluyen soporte asíncrono completo con todas las combinaciones posibles.

---

## Variantes de TryBindIfFailWithException

### TryBindIfFailWithException Simple

**Propósito**: Versión segura que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T> source,
                                                        Func<Exception, MlResult<T>> funcException,
                                                        Func<Exception, string> errorMessageBuilder)
```

**Comportamiento Especial**:
- Si HAY excepción almacenada: Ejecuta función de forma segura
- Si la función de recuperación falla: Mergea errores con el resultado original usando `MergeErrorsDetailsIfFail`
- Si NO hay excepción almacenada: Retorna el error original sin modificaciones

### TryBindIfFailWithException con Transformación

**Propósito**: Versión segura para ambas funciones (éxito y fallo)

```csharp
public static MlResult<TReturn> TryBindIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<Exception, MlResult<TReturn>> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento Especial**:
- Protege ambas funciones contra excepciones
- Usa `MergeErrorsDetailsIfFailDiferentTypes` para mergear errores entre tipos diferentes
- Mantiene el comportamiento de no ejecutar si no hay excepción almacenada

---

## Patrones de Uso

### Patrón 1: Recuperación Específica por Tipo de Excepción

```csharp
// Estrategias de recuperación basadas en tipo de excepción
var result = await riskyDatabaseOperation
    .BindIfFailWithExceptionAsync(async ex => ex switch
    {
        SqlTimeoutException timeout => await RetryWithLongerTimeout(),
        SqlConnectionException conn => await TryBackupDatabase(),
        SqlDeadlockException deadlock => await RetryWithRandomDelay(),
        _ => MlResult<Data>.Fail($"Cannot recover from {ex.GetType().Name}")
    });
```

### Patrón 2: Análisis de Causa Raíz

```csharp
// Análisis detallado de la excepción para determinar estrategia
var result = await complexOperation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        var rootCause = AnalyzeRootCause(ex);
        var recoveryStrategy = DetermineRecoveryStrategy(rootCause);
        
        return await ExecuteRecoveryStrategy(recoveryStrategy, ex);
    });
```

### Patrón 3: Escalado Inteligente

```csharp
// Escalado basado en severidad de la excepción
var result = await criticalOperation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        var severity = ClassifyExceptionSeverity(ex);
        
        return severity switch
        {
            Severity.Low => await AttemptLocalRecovery(ex),
            Severity.Medium => await EscalateToBackupService(ex),
            Severity.High => await NotifyAdminsAndFail(ex),
            _ => MlResult<ProcessResult>.Fail("Unknown severity level")
        };
    });
```

### Patrón 4: Logging Contextual

```csharp
// Logging detallado usando información de la excepción
var result = await operation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        await LogExceptionWithContext(ex, new
        {
            OperationId = operationId,
            UserId = userId,
            StackTrace = ex.StackTrace,
            InnerException = ex.InnerException?.Message,
            ExceptionData = ex.Data
        });
        
        return MlResult<Data>.Fail(ex.Message); // Propagar después de logging
    });
```

### Patrón 5: Transformación de Excepciones

```csharp
// Transformar excepciones técnicas en errores de negocio
var result = await technicalOperation
    .BindIfFailWithExceptionAsync<TechnicalData, BusinessResult>(
        validAsync: async data => await TransformToBusinessResult(data),
        failAsync: async ex => await TransformExceptionToBusinessError(ex)
    );
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Archivos con Recuperación Específica

```csharp
public class FileProcessingService
{
    private readonly IFileRepository _fileRepository;
    private readonly IBackupStorage _backupStorage;
    private readonly ILogger<FileProcessingService> _logger;

    public async Task<MlResult<ProcessedFile>> ProcessFileAsync(FileRequest request)
    {
        return await ValidateFileRequest(request)
            .BindAsync(validRequest => ReadFileAsync(validRequest.FilePath))
            .BindAsync(fileContent => ParseFileContentAsync(fileContent))
            .BindIfFailWithExceptionAsync(async ex =>
            {
                await LogParsingFailureAsync(request.FilePath, ex);
                return await HandleParsingException(request, ex);
            })
            .BindAsync(parsedContent => ProcessFileContentAsync(parsedContent))
            .BindIfFailWithExceptionAsync<ParsedContent, ProcessedFile>(
                validAsync: async content => await FinalizeProcessingAsync(content),
                failAsync: async ex => await HandleProcessingException(request, ex)
            );
    }

    public async Task<MlResult<ProcessedFile>> ProcessFileSafelyAsync(FileRequest request)
    {
        return await ValidateFileRequest(request)
            .BindAsync(validRequest => ReadFileAsync(validRequest.FilePath))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async ex => await SafeFileRecoveryAsync(request, ex),
                errorMessageBuilder: ex => $"Safe file recovery failed for {request.FilePath}: {ex.Message}"
            )
            .BindAsync(fileContent => ParseFileContentAsync(fileContent))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async ex => await SafeParsingRecoveryAsync(request, ex),
                errorMessage: "Safe parsing recovery failed"
            );
    }

    private async Task<MlResult<FileRequest>> ValidateFileRequest(FileRequest request)
    {
        await Task.Delay(25);

        if (string.IsNullOrEmpty(request.FilePath))
            return MlResult<FileRequest>.Fail("File path is required");

        if (!File.Exists(request.FilePath))
            return MlResult<FileRequest>.Fail("File does not exist");

        return MlResult<FileRequest>.Valid(request);
    }

    private async Task<MlResult<FileContent>> ReadFileAsync(string filePath)
    {
        await Task.Delay(100);

        try
        {
            // Simular diferentes tipos de errores de lectura
            var fileName = Path.GetFileName(filePath);
            
            if (fileName.Contains("locked"))
            {
                var lockEx = new UnauthorizedAccessException($"File {filePath} is locked by another process");
                return MlResult<FileContent>.Fail("File access denied", detailException: lockEx);
            }

            if (fileName.Contains("corrupted"))
            {
                var corruptEx = new InvalidDataException($"File {filePath} appears to be corrupted");
                return MlResult<FileContent>.Fail("File is corrupted", detailException: corruptEx);
            }

            if (fileName.Contains("toobig"))
            {
                var sizeEx = new OutOfMemoryException($"File {filePath} is too large to process");
                return MlResult<FileContent>.Fail("File too large", detailException: sizeEx);
            }

            if (fileName.Contains("network"))
            {
                var networkEx = new HttpRequestException($"Network error accessing file {filePath}");
                return MlResult<FileContent>.Fail("Network error", detailException: networkEx);
            }

            // Simulación de lectura exitosa
            var content = new FileContent
            {
                FilePath = filePath,
                Content = $"Content of {fileName}",
                Size = 1024,
                ReadAt = DateTime.UtcNow,
                Encoding = "UTF-8"
            };

            return MlResult<FileContent>.Valid(content);
        }
        catch (Exception ex)
        {
            return MlResult<FileContent>.Fail($"Unexpected error reading file: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ParsedContent>> ParseFileContentAsync(FileContent fileContent)
    {
        await Task.Delay(150);

        try
        {
            // Simular diferentes tipos de errores de parsing
            if (fileContent.Content.Contains("invalid_json"))
            {
                var jsonEx = new JsonException($"Invalid JSON format in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("JSON parsing failed", detailException: jsonEx);
            }

            if (fileContent.Content.Contains("invalid_xml"))
            {
                var xmlEx = new XmlException($"Invalid XML format in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("XML parsing failed", detailException: xmlEx);
            }

            if (fileContent.Content.Contains("encoding_error"))
            {
                var encodingEx = new DecoderFallbackException($"Encoding error in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("Encoding error", detailException: encodingEx);
            }

            if (fileContent.Size > 10000)
            {
                var memoryEx = new OutOfMemoryException($"File {fileContent.FilePath} too large for parsing");
                return MlResult<ParsedContent>.Fail("Memory limit exceeded", detailException: memoryEx);
            }

            // Parsing exitoso
            var parsedContent = new ParsedContent
            {
                OriginalFile = fileContent,
                ParsedData = $"Parsed: {fileContent.Content}",
                ParsedAt = DateTime.UtcNow,
                Format = DetectFormat(fileContent.Content),
                RecordCount = 10
            };

            return MlResult<ParsedContent>.Valid(parsedContent);
        }
        catch (Exception ex)
        {
            return MlResult<ParsedContent>.Fail($"Unexpected parsing error: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleParsingException(FileRequest request, Exception ex)
    {
        await Task.Delay(200);

        return ex switch
        {
            JsonException jsonEx => await HandleJsonParsingError(request, jsonEx),
            XmlException xmlEx => await HandleXmlParsingError(request, xmlEx),
            DecoderFallbackException encodingEx => await HandleEncodingError(request, encodingEx),
            OutOfMemoryException memoryEx => await HandleMemoryError(request, memoryEx),
            _ => await HandleGenericParsingError(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleJsonParsingError(FileRequest request, JsonException jsonEx)
    {
        await Task.Delay(100);

        // Intentar parsing alternativo para JSON
        try
        {
            // Simular parsing más tolerante
            var recoveredFile = new ProcessedFile
            {
                OriginalPath = request.FilePath,
                Status = ProcessingStatus.RecoveredFromJsonError,
                ProcessedAt = DateTime.UtcNow,
                ErrorRecoveryMethod = "Alternative JSON parser",
                PartialData = "Recovered partial JSON data",
                RecoveryDetails = new RecoveryDetails
                {
                    OriginalException = jsonEx,
                    RecoveryMethod = "Tolerant JSON parsing",
                    SuccessfulRecovery = true
                }
            };

            return MlResult<ProcessedFile>.Valid(recoveredFile);
        }
        catch (Exception recoveryEx)
        {
            return MlResult<ProcessedFile>.Fail(
                $"JSON recovery failed: {recoveryEx.Message}",
                detailException: recoveryEx);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleXmlParsingError(FileRequest request, XmlException xmlEx)
    {
        await Task.Delay(120);

        // Intentar convertir a formato alternativo
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromXmlError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "XML to Text conversion",
            PartialData = "Extracted text from XML",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = xmlEx,
                RecoveryMethod = "Text extraction from XML",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleEncodingError(FileRequest request, DecoderFallbackException encodingEx)
    {
        await Task.Delay(80);

        // Intentar con encoding diferente
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromEncodingError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Alternative encoding detection",
            PartialData = "Re-encoded content",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = encodingEx,
                RecoveryMethod = "UTF-8 fallback encoding",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleMemoryError(FileRequest request, OutOfMemoryException memoryEx)
    {
        await Task.Delay(150);

        // Para errores de memoria, crear procesamiento por chunks
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.ProcessedInChunks,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Chunk-based processing",
            PartialData = "Processed in 5 chunks",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = memoryEx,
                RecoveryMethod = "Streaming chunk processor",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleGenericParsingError(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        // Para errores desconocidos, crear resultado con información de diagnóstico
        var diagnosticFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.FailedWithDiagnostics,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Diagnostic information extraction",
            PartialData = $"Error details: {ex.Message}",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Error analysis and reporting",
                SuccessfulRecovery = false,
                DiagnosticInfo = new Dictionary<string, string>
                {
                    ["ExceptionType"] = ex.GetType().Name,
                    ["StackTrace"] = ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace.Length)) ?? "No stack trace",
                    ["InnerException"] = ex.InnerException?.Message ?? "No inner exception"
                }
            }
        };

        return MlResult<ProcessedFile>.Valid(diagnosticFile);
    }

    private async Task<MlResult<ProcessedContent>> ProcessFileContentAsync(ParsedContent parsedContent)
    {
        await Task.Delay(200);

        try
        {
            // Simular procesamiento que puede fallar
            if (parsedContent.RecordCount > 1000)
            {
                var ex = new InvalidOperationException($"Too many records to process: {parsedContent.RecordCount}");
                return MlResult<ProcessedContent>.Fail("Record limit exceeded", detailException: ex);
            }

            var processedContent = new ProcessedContent
            {
                OriginalParsed = parsedContent,
                ProcessedData = $"Processed: {parsedContent.ParsedData}",
                ProcessedAt = DateTime.UtcNow,
                ProcessingRules = GetProcessingRules(parsedContent.Format),
                QualityScore = 0.95
            };

            return MlResult<ProcessedContent>.Valid(processedContent);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedContent>.Fail($"Processing failed: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleProcessingException(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        return ex switch
        {
            InvalidOperationException invalidOp => await HandleInvalidOperationError(request, invalidOp),
            ArgumentException argEx => await HandleArgumentError(request, argEx),
            _ => await HandleGenericProcessingError(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleInvalidOperationError(FileRequest request, InvalidOperationException ex)
    {
        await Task.Delay(80);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromInvalidOperation,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Reduced scope processing",
            PartialData = "Processed with reduced parameters",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Parameter adjustment and retry",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> SafeFileRecoveryAsync(FileRequest request, Exception ex)
    {
        await Task.Delay(120);

        // Recuperación segura que puede fallar de forma controlada
        if (ex is OutOfMemoryException)
            throw new SystemException($"Cannot perform safe recovery for memory errors on {request.FilePath}");

        return ex switch
        {
            UnauthorizedAccessException _ => await CreateFileAccessRecovery(request),
            InvalidDataException _ => await CreateDataRecovery(request),
            HttpRequestException _ => await CreateNetworkRecovery(request),
            _ => await CreateGenericRecovery(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> SafeParsingRecoveryAsync(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        // Parsing recovery que puede fallar
        if (ex is OutOfMemoryException)
            throw new InvalidOperationException("Cannot perform safe parsing recovery for memory errors");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.SafeRecovery,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Safe parsing recovery",
            PartialData = "Minimal safe processing completed",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Safe minimal processing",
                SuccessfulRecovery = true,
                IsSafeRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    // Métodos auxiliares
    private async Task<MlResult<ProcessedFile>> CreateFileAccessRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "File access recovery", "Backup file used");
    }

    private async Task<MlResult<ProcessedFile>> CreateDataRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Data corruption recovery", "Partial data extracted");
    }

    private async Task<MlResult<ProcessedFile>> CreateNetworkRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Network error recovery", "Cached version used");
    }

    private async Task<MlResult<ProcessedFile>> CreateGenericRecovery(FileRequest request, Exception ex)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Generic error recovery", $"Error logged: {ex.GetType().Name}");
    }

    private MlResult<ProcessedFile> CreateRecoveryFile(FileRequest request, string method, string data)
    {
        var recoveryFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = method,
            PartialData = data,
            RecoveryDetails = new RecoveryDetails
            {
                RecoveryMethod = method,
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveryFile);
    }

    private async Task LogParsingFailureAsync(string filePath, Exception ex)
    {
        await Task.Delay(10);
        _logger.LogError(ex, "Parsing failed for file {FilePath}", filePath);
    }

    private string DetectFormat(string content)
    {
        if (content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("["))
            return "JSON";
        if (content.TrimStart().StartsWith("<"))
            return "XML";
        return "TEXT";
    }

    private List<string> GetProcessingRules(string format)
    {
        return format switch
        {
            "JSON" => new List<string> { "ValidateJsonSchema", "ExtractMetadata", "NormalizeStructure" },
            "XML" => new List<string> { "ValidateXmlSchema", "ExtractAttributes", "ConvertToJson" },
            _ => new List<string> { "DetectEncoding", "ExtractKeywords", "BasicFormatting" }
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleArgumentError(FileRequest request, ArgumentException argEx)
    {
        await Task.Delay(70);
        return CreateRecoveryFile(request, "Argument error recovery", "Default parameters applied");
    }

    private async Task<MlResult<ProcessedFile>> HandleGenericProcessingError(FileRequest request, Exception ex)
    {
        await Task.Delay(70);
        return CreateRecoveryFile(request, "Generic processing recovery", $"Error handled: {ex.Message}");
    }

    private async Task<MlResult<ProcessedFile>> FinalizeProcessingAsync(ParsedContent content)
    {
        await Task.Delay(50);
        var finalFile = new ProcessedFile
        {
            OriginalPath = content.OriginalFile.FilePath,
            Status = ProcessingStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            PartialData = $"Final: {content.ParsedData}"
        };
        return MlResult<ProcessedFile>.Valid(finalFile);
    }
}

// Clases de apoyo para procesamiento de archivos
public enum ProcessingStatus
{
    Completed,
    Failed,
    RecoveredFromError,
    RecoveredFromJsonError,
    RecoveredFromXmlError,
    RecoveredFromEncodingError,
    ProcessedInChunks,
    FailedWithDiagnostics,
    RecoveredFromInvalidOperation,
    SafeRecovery
}

public class FileRequest
{
    public string FilePath { get; set; }
    public string ProcessingType { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class FileContent
{
    public string FilePath { get; set; }
    public string Content { get; set; }
    public long Size { get; set; }
    public DateTime ReadAt { get; set; }
    public string Encoding { get; set; }
}

public class ParsedContent
{
    public FileContent OriginalFile { get; set; }
    public string ParsedData { get; set; }
    public DateTime ParsedAt { get; set; }
    public string Format { get; set; }
    public int RecordCount { get; set; }
}

public class ProcessedContent
{
    public ParsedContent OriginalParsed { get; set; }
    public string ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
    public List<string> ProcessingRules { get; set; }
    public double QualityScore { get; set; }
}

public class ProcessedFile
{
    public string OriginalPath { get; set; }
    public ProcessingStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ErrorRecoveryMethod { get; set; }
    public string PartialData { get; set; }
    public RecoveryDetails RecoveryDetails { get; set; }
}

public class RecoveryDetails
{
    public Exception OriginalException { get; set; }
    public string RecoveryMethod { get; set; }
    public bool SuccessfulRecovery { get; set; }
    public bool IsSafeRecovery { get; set; }
    public Dictionary<string, string> DiagnosticInfo { get; set; }
}

public interface IFileRepository { }
public interface IBackupStorage { }
```

### Ejemplo 2: Sistema de Integración de APIs con Manejo Específico de Errores HTTP

```csharp
public class ApiIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IRetryPolicy _retryPolicy;
    private readonly ICircuitBreaker _circuitBreaker;

    public async Task<MlResult<ApiResponse>> CallExternalApiAsync(ApiRequest request)
    {
        return await ValidateApiRequest(request)
            .BindAsync(validRequest => MakeHttpRequestAsync(validRequest))
            .BindIfFailWithExceptionAsync(async ex =>
            {
                await LogHttpFailureAsync(request, ex);
                return await HandleHttpException(request, ex);
            })
            .BindAsync(httpResponse => ParseApiResponseAsync(httpResponse))
            .BindIfFailWithExceptionAsync<HttpResponseMessage, ApiResponse>(
                validAsync: async response => await ProcessSuccessfulResponseAsync(response),
                failAsync: async ex => await HandleApiParsingException(request, ex)
            );
    }

    private async Task<MlResult<HttpResponseMessage>> MakeHttpRequestAsync(ApiRequest request)
    {
        await Task.Delay(100);

        try
        {
            // Simular diferentes tipos de errores HTTP
            if (request.Endpoint.Contains("timeout"))
            {
                var timeoutEx = new TaskCanceledException("Request timeout", new TimeoutException());
                return MlResult<HttpResponseMessage>.Fail("Request timed out", detailException: timeoutEx);
            }

            if (request.Endpoint.Contains("unauthorized"))
            {
                var authEx = new HttpRequestException("401 Unauthorized", null, HttpStatusCode.Unauthorized);
                return MlResult<HttpResponseMessage>.Fail("Authentication failed", detailException: authEx);
            }

            if (request.Endpoint.Contains("ratelimit"))
            {
                var rateLimitEx = new HttpRequestException("429 Too Many Requests", null, HttpStatusCode.TooManyRequests);
                return MlResult<HttpResponseMessage>.Fail("Rate limit exceeded", detailException: rateLimitEx);
            }

            if (request.Endpoint.Contains("server-error"))
            {
                var serverEx = new HttpRequestException("500 Internal Server Error", null, HttpStatusCode.InternalServerError);
                return MlResult<HttpResponseMessage>.Fail("Server error", detailException: serverEx);
            }

            if (request.Endpoint.Contains("network"))
            {
                var networkEx = new SocketException(10054); // Connection reset by peer
                var httpEx = new HttpRequestException("Network error", networkEx);
                return MlResult<HttpResponseMessage>.Fail("Network failure", detailException: httpEx);
            }

            // Respuesta exitosa simulada
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"data\": \"Response for {request.Endpoint}\", \"status\": \"success\"}}")
            };

            return MlResult<HttpResponseMessage>.Valid(response);
        }
        catch (Exception ex)
        {
            return MlResult<HttpResponseMessage>.Fail($"Unexpected HTTP error: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ApiResponse>> HandleHttpException(ApiRequest request, Exception ex)
    {
        await Task.Delay(150);

        return ex switch
        {
            TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException 
                => await HandleTimeoutException(request, timeoutEx),
            
            HttpRequestException httpEx when httpEx.Data.Contains("StatusCode") 
                => await HandleHttpStatusException(request, httpEx),
            
            HttpRequestException httpEx when httpEx.InnerException is SocketException 
                => await HandleNetworkException(request, httpEx),
            
            SocketException socketEx 
                => await HandleSocketException(request, socketEx),
            
            _ => await HandleGenericHttpException(request, ex)
        };
    }

    private async Task<MlResult<ApiResponse>> HandleTimeoutException(ApiRequest request, TaskCanceledException ex)
    {
        await Task.Delay(100);

        // Para timeouts, intentar con retry automático
        var retryResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "timeout_recovered",
            Data = "Request will be retried automatically",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Automatic retry scheduled",
                RetryScheduled = true,
                NextRetryAt = DateTime.UtcNow.AddMinutes(5)
            }
        };

        return MlResult<ApiResponse>.Valid(retryResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleHttpStatusException(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(120);

        // Extraer status code del mensaje de la excepción
        var statusCode = ExtractStatusCodeFromException(ex);

        return statusCode switch
        {
            HttpStatusCode.Unauthorized => await HandleUnauthorizedError(request, ex),
            HttpStatusCode.TooManyRequests => await HandleRateLimitError(request, ex),
            HttpStatusCode.InternalServerError => await HandleServerError(request, ex),
            HttpStatusCode.BadGateway => await HandleBadGatewayError(request, ex),
            _ => await HandleGenericStatusError(request, ex, statusCode)
        };
    }

    private async Task<MlResult<ApiResponse>> HandleUnauthorizedError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(80);

        // Para errores de autorización, intentar refresh del token
        var authResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "auth_recovery_initiated",
            Data = "Authentication token refresh initiated",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Token refresh and retry",
                RequiresManualIntervention = false,
                TokenRefreshInitiated = true
            }
        };

        return MlResult<ApiResponse>.Valid(authResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleRateLimitError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(90);

        // Para rate limiting, programar retry con backoff
        var rateLimitResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "rate_limit_handled",
            Data = "Request queued for delayed retry",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Exponential backoff retry",
                RetryScheduled = true,
                NextRetryAt = DateTime.UtcNow.AddMinutes(CalculateBackoffMinutes(request.RetryCount)),
                BackoffMultiplier = 2.0
            }
        };

        return MlResult<ApiResponse>.Valid(rateLimitResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleServerError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(110);

        // Para errores del servidor, usar endpoint alternativo si está disponible
        var serverErrorResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "server_error_handled",
            Data = "Switched to backup endpoint",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Backup endpoint failover",
                BackupEndpointUsed = true,
                OriginalEndpoint = request.Endpoint,
                BackupEndpoint = GetBackupEndpoint(request.Endpoint)
            }
        };

        return MlResult<ApiResponse>.Valid(serverErrorResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleNetworkException(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(100);

        var networkErrorResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "network_error_handled",
            Data = "Network connectivity issue - cached response provided",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Cached response fallback",
                CachedResponseUsed = true,
                CacheAge = TimeSpan.FromMinutes(30)
            }
        };

        return MlResult<ApiResponse>.Valid(networkErrorResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleSocketException(ApiRequest request, SocketException ex)
    {
        await Task.Delay(90);

        var socketErrorResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "socket_error_handled",
            Data = "Connection issue - local processing applied",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Local processing fallback",
                LocalProcessingUsed = true,
                SocketErrorCode = ex.ErrorCode
            }
        };

        return MlResult<ApiResponse>.Valid(socketErrorResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleGenericHttpException(ApiRequest request, Exception ex)
    {
        await Task.Delay(80);

        var genericResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "generic_error_handled",
            Data = "Unknown error - diagnostic information collected",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Error analysis and reporting",
                DiagnosticsCollected = true,
                ErrorAnalysis = new Dictionary<string, string>
                {
                    ["ExceptionType"] = ex.GetType().Name,
                    ["Message"] = ex.Message,
                    ["Source"] = ex.Source ?? "Unknown",
                    ["HelpLink"] = ex.HelpLink ?? "Not provided"
                }
            }
        };

        return MlResult<ApiResponse>.Valid(genericResponse);
    }

    // Métodos auxiliares
    private async Task<MlResult<ApiRequest>> ValidateApiRequest(ApiRequest request)
    {
        await Task.Delay(25);
        
        if (string.IsNullOrEmpty(request.Endpoint))
            return MlResult<ApiRequest>.Fail("Endpoint is required");
        
        return MlResult<ApiRequest>.Valid(request);
    }

    private async Task<MlResult<ApiResponse>> ParseApiResponseAsync(HttpResponseMessage response)
    {
        await Task.Delay(50);
        
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(content);
            return MlResult<ApiResponse>.Valid(apiResponse);
        }
        catch (JsonException ex)
        {
            return MlResult<ApiResponse>.Fail("Failed to parse API response", detailException: ex);
        }
    }

    private async Task<MlResult<ApiResponse>> ProcessSuccessfulResponseAsync(HttpResponseMessage response)
    {
        await Task.Delay(30);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = new ApiResponse
        {
            Status = "success",
            Data = content,
            Timestamp = DateTime.UtcNow
        };
        return MlResult<ApiResponse>.Valid(apiResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleApiParsingException(ApiRequest request, Exception ex)
    {
        await Task.Delay(60);
        
        var parsingErrorResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "parsing_error_handled",
            Data = "Response parsing failed - raw response preserved",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Raw response preservation",
                RawResponsePreserved = true
            }
        };

        return MlResult<ApiResponse>.Valid(parsingErrorResponse);
    }

    private async Task LogHttpFailureAsync(ApiRequest request, Exception ex)
    {
        await Task.Delay(10);
        // Log HTTP failure with exception details
    }

    private HttpStatusCode ExtractStatusCodeFromException(HttpRequestException ex)
    {
        if (ex.Data.Contains("StatusCode"))
            return (HttpStatusCode)ex.Data["StatusCode"];
        
        // Analizar mensaje para extraer código de estado
        if (ex.Message.Contains("401")) return HttpStatusCode.Unauthorized;
        if (ex.Message.Contains("429")) return HttpStatusCode.TooManyRequests;
        if (ex.Message.Contains("500")) return HttpStatusCode.InternalServerError;
        if (ex.Message.Contains("502")) return HttpStatusCode.BadGateway;
        
        return HttpStatusCode.InternalServerError;
    }

    private int CalculateBackoffMinutes(int retryCount)
    {
        return Math.Min((int)Math.Pow(2, retryCount), 60); // Max 60 minutes
    }

    private string GetBackupEndpoint(string originalEndpoint)
    {
        return originalEndpoint.Replace("api.primary.com", "api.backup.com");
    }

    private async Task<MlResult<ApiResponse>> HandleBadGatewayError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(85);
        
        var badGatewayResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "bad_gateway_handled",
            Data = "Gateway error - alternative route used",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Alternative gateway route",
                AlternativeRouteUsed = true
            }
        };

        return MlResult<ApiResponse>.Valid(badGatewayResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleGenericStatusError(ApiRequest request, HttpRequestException ex, HttpStatusCode statusCode)
    {
        await Task.Delay(75);
        
        var genericStatusResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = $"status_{(int)statusCode}_handled",
            Data = $"HTTP {statusCode} handled with default strategy",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Default status code handling",
                HttpStatusCode = statusCode
            }
        };

        return MlResult<ApiResponse>.Valid(genericStatusResponse);
    }
}

// Clases de apoyo para integración de APIs
public class ApiRequest
{
    public int Id { get; set; }
    public string Endpoint { get; set; }
    public string Method { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public string Body { get; set; }
    public int RetryCount { get; set; }
}

public class ApiResponse
{
    public int RequestId { get; set; }
    public string Status { get; set; }
    public string Data { get; set; }
    public DateTime Timestamp { get; set; }
    public ApiRecoveryInfo RecoveryInfo { get; set; }
}

public class ApiRecoveryInfo
{
    public Exception OriginalException { get; set; }
    public string RecoveryMethod { get; set; }
    public bool RequiresManualIntervention { get; set; }
    public bool RetryScheduled { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public bool TokenRefreshInitiated { get; set; }
    public double? BackoffMultiplier { get; set; }
    public bool BackupEndpointUsed { get; set; }
    public string OriginalEndpoint { get; set; }
    public string BackupEndpoint { get; set; }
    public bool CachedResponseUsed { get; set; }
    public TimeSpan? CacheAge { get; set; }
    public bool LocalProcessingUsed { get; set; }
    public int? SocketErrorCode { get; set; }
    public bool DiagnosticsCollected { get; set; }
    public Dictionary<string, string> ErrorAnalysis { get; set; }
    public bool RawResponsePreserved { get; set; }
    public bool AlternativeRouteUsed { get; set; }
    public HttpStatusCode? HttpStatusCode { get; set; }
}

public interface IRetryPolicy { }
public interface ICircuitBreaker { }
```

---

## Mejores Prácticas

### 1. Verificación Explícita de Excepción Almacenada

```csharp
// ✅ Correcto: Manejar el caso donde no hay excepción almacenada
public async Task<MlResult<Data>> ProcessWithExceptionHandlingAsync(MlResult<Data> source)
{
    return await source
        .BindIfFailWithExceptionAsync(async ex =>
        {
            // Esta función solo se ejecuta si HAY excepción almacenada
            return await RecoverBasedOnException(ex);
        });
    
    // Si no hay excepción almacenada, el error original se retorna sin cambios
    // Esto es diferente a BindIfFailWithValue que añadiría un error adicional
}

// ✅ Correcto: Documentar el comportamiento esperado
/// <summary>
/// Procesa datos con recuperación basada en excepción.
/// NOTA: La función de recuperación solo se ejecuta si el error contiene una excepción almacenada.
/// Si no hay excepción, el error original se retorna sin modificaciones.
/// </summary>
public async Task<MlResult<ProcessedData>> ProcessWithDocumentedBehaviorAsync(InputData input)
{
    return await ProcessInput(input)
        .BindIfFailWithExceptionAsync(async ex =>
        {
            // Solo se ejecuta si hay excepción en el error
            return await CreateRecoveryFromException(ex);
        });
}
```

### 2. Estrategias de Recuperación por Tipo de Excepción

```csharp
// ✅ Correcto: Estrategias específicas y bien definidas por tipo
public class ExceptionRecoveryStrategies
{
    public async Task<MlResult<T>> CreateRecoveryStrategy<T>(Exception ex)
    {
        return ex switch
        {
            // Errores de red - retry con backoff
            HttpRequestException httpEx when IsNetworkError(httpEx) 
                => await NetworkRecoveryStrategy<T>(httpEx),
            
            // Errores de timeout - retry con timeout mayor
            TimeoutException timeoutEx 
                => await TimeoutRecoveryStrategy<T>(timeoutEx),
            
            // Errores de autorización - refresh credentials
            UnauthorizedAccessException authEx 
                => await AuthRecoveryStrategy<T>(authEx),
            
            // Errores de datos - usar datos por defecto
            InvalidDataException dataEx 
                => await DataRecoveryStrategy<T>(dataEx),
            
            // Errores de memoria - liberar recursos y retry
            OutOfMemoryException memEx 
                => await MemoryRecoveryStrategy<T>(memEx),
            
            // Errores de SQL - usar cache o fuente alternativa
            SqlException sqlEx 
                => await DatabaseRecoveryStrategy<T>(sqlEx),
            
            // Otros errores - logging y fallo controlado
            _ => await GenericRecoveryStrategy<T>(ex)
        };
    }
    
    private async Task<MlResult<T>> NetworkRecoveryStrategy<T>(HttpRequestException ex)
    {
        // Implementación específica para errores de red
        await Task.Delay(CalculateBackoffDelay(ex));
        // ... lógica de retry
        return MlResult<T>.Fail($"Network recovery attempted: {ex.Message}");
    }
    
    private bool IsNetworkError(HttpRequestException ex)
    {
        return ex.Message.Contains("timeout") || 
               ex.Message.Contains("connection") ||
               ex.InnerException is SocketException;
    }
    
    private int CalculateBackoffDelay(Exception ex)
    {
        // Lógica para calcular delay basado en tipo de error
        return 1000; // Simplificado
    }
}

// ✅ Correcto: Uso de estrategias específicas
public async Task<MlResult<ProcessResult>> ProcessWithSpecificStrategiesAsync(ProcessRequest request)
{
    var strategies = new ExceptionRecoveryStrategies();
    
    return await ExecuteProcessing(request)
        .BindIfFailWithExceptionAsync(async ex => await strategies.CreateRecoveryStrategy<ProcessResult>(ex))
        .BindIfFailWithExceptionAsync(async ex =>
        {
            // Segunda capa de recuperación para fallos de la primera
            await LogSecondaryRecoveryAttempt(ex);
            return await CreateFallbackResult(request, ex);
        });
}
```

### 3. Logging Contextual con Información de Excepción

```csharp
// ✅ Correcto: Logging rico con contexto de excepción
public class ContextualExceptionLogger
{
    private readonly ILogger<ContextualExceptionLogger> _logger;
    
    public async Task<MlResult<T>> LogAndRecover<T>(MlResult<T> source, string operationContext)
    {
        return await source
            .BindIfFailWithExceptionAsync(async ex =>
            {
                var logContext = CreateLogContext(ex, operationContext);
                await LogExceptionWithContext(logContext);
                
                return await DetermineRecoveryAction<T>(ex, logContext);
            });
    }
    
    private ExceptionLogContext CreateLogContext(Exception ex, string operationContext)
    {
        return new ExceptionLogContext
        {
            ExceptionType = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            InnerExceptions = ExtractInnerExceptions(ex),
            OperationContext = operationContext,
            Timestamp = DateTime.UtcNow,
            MachineName = Environment.MachineName,
            ProcessId = Environment.ProcessId,
            ThreadId = Thread.CurrentThread.ManagedThreadId,
            CustomData = ExtractCustomData(ex)
        };
    }
    
    private async Task LogExceptionWithContext(ExceptionLogContext context)
    {
        _logger.LogError(
            "Exception in {OperationContext}: {ExceptionType} - {Message}. " +
            "Machine: {MachineName}, Process: {ProcessId}, Thread: {ThreadId}",
            context.OperationContext,
            context.ExceptionType,
            context.Message,
            context.MachineName,
            context.ProcessId,
            context.ThreadId);
        
        // Log detallado para análisis posterior
        _logger.LogDebug("Full exception details: {@ExceptionContext}", context);
    }
    
    private List<string> ExtractInnerExceptions(Exception ex)
    {
        var innerExceptions = new List<string>();
        var current = ex.InnerException;
        
        while (current != null)
        {
            innerExceptions.Add($"{current.GetType().Name}: {current.Message}");
            current = current.InnerException;
        }
        
        return innerExceptions;
    }
    
    private Dictionary<string, object> ExtractCustomData(Exception ex)
    {
        var customData = new Dictionary<string, object>();
        
        foreach (var key in ex.Data.Keys)
        {
            customData[key.ToString()] = ex.Data[key];
        }
        
        return customData;
    }
}

// ✅ Uso del logger contextual
public async Task<MlResult<ProcessedData>> ProcessWithContextualLoggingAsync(InputData input)
{
    var contextualLogger = new ContextualExceptionLogger();
    
    return await ProcessData(input)
        .BindAsync(data => TransformData(data))
        .Then(result => contextualLogger.LogAndRecover(result, $"Data processing for input {input.Id}"))
        .BindAsync(data => ValidateData(data))
        .Then(result => contextualLogger.LogAndRecover(result, $"Data validation for input {input.Id}"));
}
```

### 4. Manejo de Merge de Errores en Try* Variants

```csharp
// ✅ Correcto: Entender el comportamiento de merge en Try variants
public async Task<MlResult<ProcessedData>> ProcessWithTryVariantsAsync(InputData input)
{
    return await ProcessInput(input)
        .TryBindIfFailWithExceptionAsync(
            funcExceptionAsync: async ex =>
            {
                // Si esta función falla, el error se mergea con el original
                return await AttemptRecovery(ex);
            },
            errorMessageBuilder: recoveryEx => $"Recovery failed: {recoveryEx.Message}"
        );
    
    // El resultado final puede contener:
    // 1. Error original (si no hay excepción almacenada)
    // 2. Resultado de recuperación exitosa
    // 3. Error original + error de recuperación (si la recuperación falla)
}

// ✅ Correcto: Documentar el comportamiento de merge
/// <summary>
/// Procesa datos con recuperación segura.
/// Si la recuperación falla, ambos errores (original y de recuperación) 
/// se combinan en el resultado final.
/// </summary>
public async Task<MlResult<Data>> ProcessWithSafeMergeAsync(InputData input)
{
    return await ProcessInput(input)
        .TryBindIfFailWithExceptionAsync(
            funcExceptionAsync: async ex =>
            {
                // Función que puede fallar
                if (ex is OutOfMemoryException)
                    throw new InvalidOperationException("Cannot recover from memory errors");
                
                return await RecoverFromException(ex);
            },
            errorMessage: "Recovery operation failed"
        );
}
```

### 5. Testing de Comportamiento Específico

```csharp
// ✅ Correcto: Testing específico para BindIfFailWithException
[TestFixture]
public class BindIfFailWithExceptionTests
{
    [Test]
    public async Task BindIfFailWithException_WithStoredException_ExecutesRecoveryFunction()
    {
        // Arrange
        var originalException = new InvalidOperationException("Original error");
        var failingResult = MlResult<string>.Fail("Processing failed", detailException: originalException);
        
        // Act
        var result = await failingResult
            .BindIfFailWithExceptionAsync(async ex =>
            {
                // Verificar que recibimos la excepción correcta
                ex.Should().BeOfType<InvalidOperationException>();
                ex.Message.# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos con Excepción Previa

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Diferencias con BindIfFailWithValue](#diferencias-con-bindiffailwithvalue)
4. [Variantes de BindIfFailWithException](#variantes-de-bindiffailwithexception)
5. [Variantes de TryBindIfFailWithException](#variantes-de-trybindiffailwithexception)
6. [Patrones de Uso](#patrones-de-uso)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindIfFailWithException** de `MlResultActionsBind` proporciona operaciones especializadas para el manejo de errores que tienen acceso a una **excepción previa almacenada** en los detalles del error. Estas operaciones implementan patrones de **recuperación basada en excepciones**, **análisis de errores específicos**, y **manejo contextual de fallos**, permitiendo estrategias de recuperación que pueden analizar y responder al tipo específico de excepción que causó el fallo.

### Propósito Principal

- **Recuperación Específica por Excepción**: Estrategias de recuperación basadas en el tipo de excepción
- **Análisis de Fallos**: Inspección detallada de excepciones para determinar causa raíz
- **Manejo Contextual**: Acceso a información rica de la excepción original
- **Escalado Inteligente**: Decisiones de escalado basadas en tipos de excepción

### Concepto Clave: Excepción en Detalles de Error

El sistema `MlResult` permite almacenar excepciones en los detalles del error cuando ocurre un fallo debido a una excepción. Esto permite que las funciones de recuperación tengan acceso completo a la excepción original, incluyendo stack trace, mensaje, y tipo específico.

---

## Análisis de la Funcionalidad

### Filosofía de BindIfFailWithException

```
MlResult<T> → ¿Es Fallo?
              ├─ Sí → ¿Tiene Excepción Almacenada?
              │       ├─ Sí → Función de Recuperación(Exception) → MlResult<T>
              │       └─ No → Retorna Fallo Original (sin cambios)
              └─ No → Retorna Valor Original (sin cambios)
```

### Comportamiento Crítico: Diferencias con BindIfFailWithValue

**Diferencia Fundamental en Comportamiento:**

1. **BindIfFailWithValue**: 
   - Si recibe un `MlResult` Fail **sin** ValueDetail → Añade un nuevo Error al que viene de la ejecución anterior
   - Siempre intenta procesar, incluso sin valor previo

2. **BindIfFailWithException**: 
   - Si recibe un `MlResult` Fail **sin** ExceptionDetail → Devuelve el `MlResult` Fail **igual que le vino**
   - **NO** ejecuta la función de recuperación si no hay excepción almacenada

### Flujo de Procesamiento

1. **Si el resultado fuente es exitoso**: Retorna el valor sin cambios
2. **Si el resultado fuente es fallido**: 
   - Intenta extraer la excepción almacenada usando `errorsDetails.GetDetailException()`
   - **Si HAY excepción almacenada**: Ejecuta la función de recuperación con esa excepción
   - **Si NO hay excepción almacenada**: Retorna el error original sin modificaciones (comportamiento clave)

### Tipos de Operaciones

1. **BindIfFailWithException Simple**: Recuperación solo cuando hay excepción previa
2. **BindIfFailWithException con Transformación**: Manejo de éxito y fallo con posible cambio de tipo
3. **TryBindIfFailWithException**: Versiones seguras que capturan excepciones en las funciones de recuperación

---

## Diferencias con BindIfFailWithValue

### Tabla Comparativa de Comportamientos

| Escenario | BindIfFailWithValue | BindIfFailWithException |
|-----------|-------------------|------------------------|
| **Resultado Exitoso** | Retorna valor sin cambios | Retorna valor sin cambios |
| **Fallo CON dato almacenado** | Ejecuta función con valor previo | Ejecuta función con excepción |
| **Fallo SIN dato almacenado** | ⚠️ **Añade nuevo error** | ✅ **Retorna fallo original** |
| **Función de recuperación falla** | Propaga nuevo error | Puede mergear errores |

### Ejemplos de Comportamiento Diferencial

```csharp
// Caso 1: Fallo sin datos almacenados
var failWithoutValue = MlResult<string>.Fail("Simple error"); // Sin valor ni excepción

// BindIfFailWithValue - Añade nuevo error
var resultWithValue = failWithoutValue.BindIfFailWithValue(prevValue => 
    MlResult<string>.Valid("recovered")); // Se ejecuta, puede añadir error

// BindIfFailWithException - Retorna original sin cambios
var resultWithException = failWithoutValue.BindIfFailWithException(ex => 
    MlResult<string>.Valid("recovered")); // NO se ejecuta, retorna fallo original

// Caso 2: Fallo con datos almacenados
var failWithException = MlResult<string>.Fail("Error with exception", 
    detailException: new InvalidOperationException("Original error"));

// BindIfFailWithException - Ejecuta función de recuperación
var resultWithStoredException = failWithException.BindIfFailWithException(ex => 
{
    // ex es InvalidOperationException
    return ex switch
    {
        InvalidOperationException => MlResult<string>.Valid("Recovered from InvalidOperation"),
        ArgumentException => MlResult<string>.Valid("Recovered from Argument"),
        _ => MlResult<string>.Fail("Cannot recover from this exception type")
    };
}); // Se ejecuta la función
```

---

## Variantes de BindIfFailWithException

### Variante 1: BindIfFailWithException Simple

**Propósito**: Recuperación usando la excepción previa almacenada en el error

```csharp
public static MlResult<T> BindIfFailWithException<T>(this MlResult<T> source,
                                                     Func<Exception, MlResult<T>> funcException)
```

**Parámetros**:
- `source`: El resultado que puede contener una excepción en caso de fallo
- `funcException`: Función que recibe la excepción y retorna un nuevo resultado

**Comportamiento**:
- Si `source` es exitoso: Retorna el valor sin cambios
- Si `source` es fallido CON excepción: Ejecuta `funcException(storedException)`
- Si `source` es fallido SIN excepción: Retorna el error original **sin cambios**

**Flujo Interno**:
```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(), // Sin excepción: retorna original
        valid: funcException                                           // Con excepción: ejecuta función
    ),
    valid: value => value
)
```

### Variante 2: BindIfFailWithException con Transformación de Tipos

**Propósito**: Manejo completo con acceso a excepción previa y posibilidad de cambiar tipos

```csharp
public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<Exception, MlResult<TReturn>> funcFail)
```

**Parámetros**:
- `source`: El resultado fuente
- `funcValid`: Función para manejar valores exitosos
- `funcFail`: Función para manejar fallos usando excepción previa

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `funcValid(value)`
- Si `source` es fallido CON excepción: Ejecuta `funcFail(storedException)`
- Si `source` es fallido SIN excepción: Retorna error original convertido al nuevo tipo

### Soporte Asíncrono Completo

Ambas variantes incluyen soporte asíncrono completo con todas las combinaciones posibles.

---

## Variantes de TryBindIfFailWithException

### TryBindIfFailWithException Simple

**Propósito**: Versión segura que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryBindIfFailWithException<T>(this MlResult<T> source,
                                                        Func<Exception, MlResult<T>> funcException,
                                                        Func<Exception, string> errorMessageBuilder)
```

**Comportamiento Especial**:
- Si HAY excepción almacenada: Ejecuta función de forma segura
- Si la función de recuperación falla: Mergea errores con el resultado original usando `MergeErrorsDetailsIfFail`
- Si NO hay excepción almacenada: Retorna el error original sin modificaciones

### TryBindIfFailWithException con Transformación

**Propósito**: Versión segura para ambas funciones (éxito y fallo)

```csharp
public static MlResult<TReturn> TryBindIfFailWithException<T, TReturn>(
    this MlResult<T> source,
    Func<T, MlResult<TReturn>> funcValid,
    Func<Exception, MlResult<TReturn>> funcFail,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento Especial**:
- Protege ambas funciones contra excepciones
- Usa `MergeErrorsDetailsIfFailDiferentTypes` para mergear errores entre tipos diferentes
- Mantiene el comportamiento de no ejecutar si no hay excepción almacenada

---

## Patrones de Uso

### Patrón 1: Recuperación Específica por Tipo de Excepción

```csharp
// Estrategias de recuperación basadas en tipo de excepción
var result = await riskyDatabaseOperation
    .BindIfFailWithExceptionAsync(async ex => ex switch
    {
        SqlTimeoutException timeout => await RetryWithLongerTimeout(),
        SqlConnectionException conn => await TryBackupDatabase(),
        SqlDeadlockException deadlock => await RetryWithRandomDelay(),
        _ => MlResult<Data>.Fail($"Cannot recover from {ex.GetType().Name}")
    });
```

### Patrón 2: Análisis de Causa Raíz

```csharp
// Análisis detallado de la excepción para determinar estrategia
var result = await complexOperation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        var rootCause = AnalyzeRootCause(ex);
        var recoveryStrategy = DetermineRecoveryStrategy(rootCause);
        
        return await ExecuteRecoveryStrategy(recoveryStrategy, ex);
    });
```

### Patrón 3: Escalado Inteligente

```csharp
// Escalado basado en severidad de la excepción
var result = await criticalOperation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        var severity = ClassifyExceptionSeverity(ex);
        
        return severity switch
        {
            Severity.Low => await AttemptLocalRecovery(ex),
            Severity.Medium => await EscalateToBackupService(ex),
            Severity.High => await NotifyAdminsAndFail(ex),
            _ => MlResult<ProcessResult>.Fail("Unknown severity level")
        };
    });
```

### Patrón 4: Logging Contextual

```csharp
// Logging detallado usando información de la excepción
var result = await operation
    .BindIfFailWithExceptionAsync(async ex =>
    {
        await LogExceptionWithContext(ex, new
        {
            OperationId = operationId,
            UserId = userId,
            StackTrace = ex.StackTrace,
            InnerException = ex.InnerException?.Message,
            ExceptionData = ex.Data
        });
        
        return MlResult<Data>.Fail(ex.Message); // Propagar después de logging
    });
```

### Patrón 5: Transformación de Excepciones

```csharp
// Transformar excepciones técnicas en errores de negocio
var result = await technicalOperation
    .BindIfFailWithExceptionAsync<TechnicalData, BusinessResult>(
        validAsync: async data => await TransformToBusinessResult(data),
        failAsync: async ex => await TransformExceptionToBusinessError(ex)
    );
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Archivos con Recuperación Específica

```csharp
public class FileProcessingService
{
    private readonly IFileRepository _fileRepository;
    private readonly IBackupStorage _backupStorage;
    private readonly ILogger<FileProcessingService> _logger;

    public async Task<MlResult<ProcessedFile>> ProcessFileAsync(FileRequest request)
    {
        return await ValidateFileRequest(request)
            .BindAsync(validRequest => ReadFileAsync(validRequest.FilePath))
            .BindAsync(fileContent => ParseFileContentAsync(fileContent))
            .BindIfFailWithExceptionAsync(async ex =>
            {
                await LogParsingFailureAsync(request.FilePath, ex);
                return await HandleParsingException(request, ex);
            })
            .BindAsync(parsedContent => ProcessFileContentAsync(parsedContent))
            .BindIfFailWithExceptionAsync<ParsedContent, ProcessedFile>(
                validAsync: async content => await FinalizeProcessingAsync(content),
                failAsync: async ex => await HandleProcessingException(request, ex)
            );
    }

    public async Task<MlResult<ProcessedFile>> ProcessFileSafelyAsync(FileRequest request)
    {
        return await ValidateFileRequest(request)
            .BindAsync(validRequest => ReadFileAsync(validRequest.FilePath))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async ex => await SafeFileRecoveryAsync(request, ex),
                errorMessageBuilder: ex => $"Safe file recovery failed for {request.FilePath}: {ex.Message}"
            )
            .BindAsync(fileContent => ParseFileContentAsync(fileContent))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async ex => await SafeParsingRecoveryAsync(request, ex),
                errorMessage: "Safe parsing recovery failed"
            );
    }

    private async Task<MlResult<FileRequest>> ValidateFileRequest(FileRequest request)
    {
        await Task.Delay(25);

        if (string.IsNullOrEmpty(request.FilePath))
            return MlResult<FileRequest>.Fail("File path is required");

        if (!File.Exists(request.FilePath))
            return MlResult<FileRequest>.Fail("File does not exist");

        return MlResult<FileRequest>.Valid(request);
    }

    private async Task<MlResult<FileContent>> ReadFileAsync(string filePath)
    {
        await Task.Delay(100);

        try
        {
            // Simular diferentes tipos de errores de lectura
            var fileName = Path.GetFileName(filePath);
            
            if (fileName.Contains("locked"))
            {
                var lockEx = new UnauthorizedAccessException($"File {filePath} is locked by another process");
                return MlResult<FileContent>.Fail("File access denied", detailException: lockEx);
            }

            if (fileName.Contains("corrupted"))
            {
                var corruptEx = new InvalidDataException($"File {filePath} appears to be corrupted");
                return MlResult<FileContent>.Fail("File is corrupted", detailException: corruptEx);
            }

            if (fileName.Contains("toobig"))
            {
                var sizeEx = new OutOfMemoryException($"File {filePath} is too large to process");
                return MlResult<FileContent>.Fail("File too large", detailException: sizeEx);
            }

            if (fileName.Contains("network"))
            {
                var networkEx = new HttpRequestException($"Network error accessing file {filePath}");
                return MlResult<FileContent>.Fail("Network error", detailException: networkEx);
            }

            // Simulación de lectura exitosa
            var content = new FileContent
            {
                FilePath = filePath,
                Content = $"Content of {fileName}",
                Size = 1024,
                ReadAt = DateTime.UtcNow,
                Encoding = "UTF-8"
            };

            return MlResult<FileContent>.Valid(content);
        }
        catch (Exception ex)
        {
            return MlResult<FileContent>.Fail($"Unexpected error reading file: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ParsedContent>> ParseFileContentAsync(FileContent fileContent)
    {
        await Task.Delay(150);

        try
        {
            // Simular diferentes tipos de errores de parsing
            if (fileContent.Content.Contains("invalid_json"))
            {
                var jsonEx = new JsonException($"Invalid JSON format in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("JSON parsing failed", detailException: jsonEx);
            }

            if (fileContent.Content.Contains("invalid_xml"))
            {
                var xmlEx = new XmlException($"Invalid XML format in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("XML parsing failed", detailException: xmlEx);
            }

            if (fileContent.Content.Contains("encoding_error"))
            {
                var encodingEx = new DecoderFallbackException($"Encoding error in file {fileContent.FilePath}");
                return MlResult<ParsedContent>.Fail("Encoding error", detailException: encodingEx);
            }

            if (fileContent.Size > 10000)
            {
                var memoryEx = new OutOfMemoryException($"File {fileContent.FilePath} too large for parsing");
                return MlResult<ParsedContent>.Fail("Memory limit exceeded", detailException: memoryEx);
            }

            // Parsing exitoso
            var parsedContent = new ParsedContent
            {
                OriginalFile = fileContent,
                ParsedData = $"Parsed: {fileContent.Content}",
                ParsedAt = DateTime.UtcNow,
                Format = DetectFormat(fileContent.Content),
                RecordCount = 10
            };

            return MlResult<ParsedContent>.Valid(parsedContent);
        }
        catch (Exception ex)
        {
            return MlResult<ParsedContent>.Fail($"Unexpected parsing error: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleParsingException(FileRequest request, Exception ex)
    {
        await Task.Delay(200);

        return ex switch
        {
            JsonException jsonEx => await HandleJsonParsingError(request, jsonEx),
            XmlException xmlEx => await HandleXmlParsingError(request, xmlEx),
            DecoderFallbackException encodingEx => await HandleEncodingError(request, encodingEx),
            OutOfMemoryException memoryEx => await HandleMemoryError(request, memoryEx),
            _ => await HandleGenericParsingError(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleJsonParsingError(FileRequest request, JsonException jsonEx)
    {
        await Task.Delay(100);

        // Intentar parsing alternativo para JSON
        try
        {
            // Simular parsing más tolerante
            var recoveredFile = new ProcessedFile
            {
                OriginalPath = request.FilePath,
                Status = ProcessingStatus.RecoveredFromJsonError,
                ProcessedAt = DateTime.UtcNow,
                ErrorRecoveryMethod = "Alternative JSON parser",
                PartialData = "Recovered partial JSON data",
                RecoveryDetails = new RecoveryDetails
                {
                    OriginalException = jsonEx,
                    RecoveryMethod = "Tolerant JSON parsing",
                    SuccessfulRecovery = true
                }
            };

            return MlResult<ProcessedFile>.Valid(recoveredFile);
        }
        catch (Exception recoveryEx)
        {
            return MlResult<ProcessedFile>.Fail(
                $"JSON recovery failed: {recoveryEx.Message}",
                detailException: recoveryEx);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleXmlParsingError(FileRequest request, XmlException xmlEx)
    {
        await Task.Delay(120);

        // Intentar convertir a formato alternativo
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromXmlError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "XML to Text conversion",
            PartialData = "Extracted text from XML",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = xmlEx,
                RecoveryMethod = "Text extraction from XML",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleEncodingError(FileRequest request, DecoderFallbackException encodingEx)
    {
        await Task.Delay(80);

        // Intentar con encoding diferente
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromEncodingError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Alternative encoding detection",
            PartialData = "Re-encoded content",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = encodingEx,
                RecoveryMethod = "UTF-8 fallback encoding",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleMemoryError(FileRequest request, OutOfMemoryException memoryEx)
    {
        await Task.Delay(150);

        // Para errores de memoria, crear procesamiento por chunks
        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.ProcessedInChunks,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Chunk-based processing",
            PartialData = "Processed in 5 chunks",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = memoryEx,
                RecoveryMethod = "Streaming chunk processor",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> HandleGenericParsingError(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        // Para errores desconocidos, crear resultado con información de diagnóstico
        var diagnosticFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.FailedWithDiagnostics,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Diagnostic information extraction",
            PartialData = $"Error details: {ex.Message}",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Error analysis and reporting",
                SuccessfulRecovery = false,
                DiagnosticInfo = new Dictionary<string, string>
                {
                    ["ExceptionType"] = ex.GetType().Name,
                    ["StackTrace"] = ex.StackTrace?.Substring(0, Math.Min(500, ex.StackTrace.Length)) ?? "No stack trace",
                    ["InnerException"] = ex.InnerException?.Message ?? "No inner exception"
                }
            }
        };

        return MlResult<ProcessedFile>.Valid(diagnosticFile);
    }

    private async Task<MlResult<ProcessedContent>> ProcessFileContentAsync(ParsedContent parsedContent)
    {
        await Task.Delay(200);

        try
        {
            // Simular procesamiento que puede fallar
            if (parsedContent.RecordCount > 1000)
            {
                var ex = new InvalidOperationException($"Too many records to process: {parsedContent.RecordCount}");
                return MlResult<ProcessedContent>.Fail("Record limit exceeded", detailException: ex);
            }

            var processedContent = new ProcessedContent
            {
                OriginalParsed = parsedContent,
                ProcessedData = $"Processed: {parsedContent.ParsedData}",
                ProcessedAt = DateTime.UtcNow,
                ProcessingRules = GetProcessingRules(parsedContent.Format),
                QualityScore = 0.95
            };

            return MlResult<ProcessedContent>.Valid(processedContent);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedContent>.Fail($"Processing failed: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> HandleProcessingException(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        return ex switch
        {
            InvalidOperationException invalidOp => await HandleInvalidOperationError(request, invalidOp),
            ArgumentException argEx => await HandleArgumentError(request, argEx),
            _ => await HandleGenericProcessingError(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleInvalidOperationError(FileRequest request, InvalidOperationException ex)
    {
        await Task.Delay(80);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromInvalidOperation,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Reduced scope processing",
            PartialData = "Processed with reduced parameters",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Parameter adjustment and retry",
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> SafeFileRecoveryAsync(FileRequest request, Exception ex)
    {
        await Task.Delay(120);

        // Recuperación segura que puede fallar de forma controlada
        if (ex is OutOfMemoryException)
            throw new SystemException($"Cannot perform safe recovery for memory errors on {request.FilePath}");

        return ex switch
        {
            UnauthorizedAccessException _ => await CreateFileAccessRecovery(request),
            InvalidDataException _ => await CreateDataRecovery(request),
            HttpRequestException _ => await CreateNetworkRecovery(request),
            _ => await CreateGenericRecovery(request, ex)
        };
    }

    private async Task<MlResult<ProcessedFile>> SafeParsingRecoveryAsync(FileRequest request, Exception ex)
    {
        await Task.Delay(100);

        // Parsing recovery que puede fallar
        if (ex is OutOfMemoryException)
            throw new InvalidOperationException("Cannot perform safe parsing recovery for memory errors");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.SafeRecovery,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = "Safe parsing recovery",
            PartialData = "Minimal safe processing completed",
            RecoveryDetails = new RecoveryDetails
            {
                OriginalException = ex,
                RecoveryMethod = "Safe minimal processing",
                SuccessfulRecovery = true,
                IsSafeRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    // Métodos auxiliares
    private async Task<MlResult<ProcessedFile>> CreateFileAccessRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "File access recovery", "Backup file used");
    }

    private async Task<MlResult<ProcessedFile>> CreateDataRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Data corruption recovery", "Partial data extracted");
    }

    private async Task<MlResult<ProcessedFile>> CreateNetworkRecovery(FileRequest request)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Network error recovery", "Cached version used");
    }

    private async Task<MlResult<ProcessedFile>> CreateGenericRecovery(FileRequest request, Exception ex)
    {
        await Task.Delay(60);
        return CreateRecoveryFile(request, "Generic error recovery", $"Error logged: {ex.GetType().Name}");
    }

    private MlResult<ProcessedFile> CreateRecoveryFile(FileRequest request, string method, string data)
    {
        var recoveryFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            Status = ProcessingStatus.RecoveredFromError,
            ProcessedAt = DateTime.UtcNow,
            ErrorRecoveryMethod = method,
            PartialData = data,
            RecoveryDetails = new RecoveryDetails
            {
                RecoveryMethod = method,
                SuccessfulRecovery = true
            }
        };

        return MlResult<ProcessedFile>.Valid(recoveryFile);
    }

    private async Task LogParsingFailureAsync(string filePath, Exception ex)
    {
        await Task.Delay(10);
        _logger.LogError(ex, "Parsing failed for file {FilePath}", filePath);
    }

    private string DetectFormat(string content)
    {
        if (content.TrimStart().StartsWith("{") || content.TrimStart().StartsWith("["))
            return "JSON";
        if (content.TrimStart().StartsWith("<"))
            return "XML";
        return "TEXT";
    }

    private List<string> GetProcessingRules(string format)
    {
        return format switch
        {
            "JSON" => new List<string> { "ValidateJsonSchema", "ExtractMetadata", "NormalizeStructure" },
            "XML" => new List<string> { "ValidateXmlSchema", "ExtractAttributes", "ConvertToJson" },
            _ => new List<string> { "DetectEncoding", "ExtractKeywords", "BasicFormatting" }
        };
    }

    private async Task<MlResult<ProcessedFile>> HandleArgumentError(FileRequest request, ArgumentException argEx)
    {
        await Task.Delay(70);
        return CreateRecoveryFile(request, "Argument error recovery", "Default parameters applied");
    }

    private async Task<MlResult<ProcessedFile>> HandleGenericProcessingError(FileRequest request, Exception ex)
    {
        await Task.Delay(70);
        return CreateRecoveryFile(request, "Generic processing recovery", $"Error handled: {ex.Message}");
    }

    private async Task<MlResult<ProcessedFile>> FinalizeProcessingAsync(ParsedContent content)
    {
        await Task.Delay(50);
        var finalFile = new ProcessedFile
        {
            OriginalPath = content.OriginalFile.FilePath,
            Status = ProcessingStatus.Completed,
            ProcessedAt = DateTime.UtcNow,
            PartialData = $"Final: {content.ParsedData}"
        };
        return MlResult<ProcessedFile>.Valid(finalFile);
    }
}

// Clases de apoyo para procesamiento de archivos
public enum ProcessingStatus
{
    Completed,
    Failed,
    RecoveredFromError,
    RecoveredFromJsonError,
    RecoveredFromXmlError,
    RecoveredFromEncodingError,
    ProcessedInChunks,
    FailedWithDiagnostics,
    RecoveredFromInvalidOperation,
    SafeRecovery
}

public class FileRequest
{
    public string FilePath { get; set; }
    public string ProcessingType { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class FileContent
{
    public string FilePath { get; set; }
    public string Content { get; set; }
    public long Size { get; set; }
    public DateTime ReadAt { get; set; }
    public string Encoding { get; set; }
}

public class ParsedContent
{
    public FileContent OriginalFile { get; set; }
    public string ParsedData { get; set; }
    public DateTime ParsedAt { get; set; }
    public string Format { get; set; }
    public int RecordCount { get; set; }
}

public class ProcessedContent
{
    public ParsedContent OriginalParsed { get; set; }
    public string ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
    public List<string> ProcessingRules { get; set; }
    public double QualityScore { get; set; }
}

public class ProcessedFile
{
    public string OriginalPath { get; set; }
    public ProcessingStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ErrorRecoveryMethod { get; set; }
    public string PartialData { get; set; }
    public RecoveryDetails RecoveryDetails { get; set; }
}

public class RecoveryDetails
{
    public Exception OriginalException { get; set; }
    public string RecoveryMethod { get; set; }
    public bool SuccessfulRecovery { get; set; }
    public bool IsSafeRecovery { get; set; }
    public Dictionary<string, string> DiagnosticInfo { get; set; }
}

public interface IFileRepository { }
public interface IBackupStorage { }
```

### Ejemplo 2: Sistema de Integración de APIs con Manejo Específico de Errores HTTP

```csharp
public class ApiIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IRetryPolicy _retryPolicy;
    private readonly ICircuitBreaker _circuitBreaker;

    public async Task<MlResult<ApiResponse>> CallExternalApiAsync(ApiRequest request)
    {
        return await ValidateApiRequest(request)
            .BindAsync(validRequest => MakeHttpRequestAsync(validRequest))
            .BindIfFailWithExceptionAsync(async ex =>
            {
                await LogHttpFailureAsync(request, ex);
                return await HandleHttpException(request, ex);
            })
            .BindAsync(httpResponse => ParseApiResponseAsync(httpResponse))
            .BindIfFailWithExceptionAsync<HttpResponseMessage, ApiResponse>(
                validAsync: async response => await ProcessSuccessfulResponseAsync(response),
                failAsync: async ex => await HandleApiParsingException(request, ex)
            );
    }

    private async Task<MlResult<HttpResponseMessage>> MakeHttpRequestAsync(ApiRequest request)
    {
        await Task.Delay(100);

        try
        {
            // Simular diferentes tipos de errores HTTP
            if (request.Endpoint.Contains("timeout"))
            {
                var timeoutEx = new TaskCanceledException("Request timeout", new TimeoutException());
                return MlResult<HttpResponseMessage>.Fail("Request timed out", detailException: timeoutEx);
            }

            if (request.Endpoint.Contains("unauthorized"))
            {
                var authEx = new HttpRequestException("401 Unauthorized", null, HttpStatusCode.Unauthorized);
                return MlResult<HttpResponseMessage>.Fail("Authentication failed", detailException: authEx);
            }

            if (request.Endpoint.Contains("ratelimit"))
            {
                var rateLimitEx = new HttpRequestException("429 Too Many Requests", null, HttpStatusCode.TooManyRequests);
                return MlResult<HttpResponseMessage>.Fail("Rate limit exceeded", detailException: rateLimitEx);
            }

            if (request.Endpoint.Contains("server-error"))
            {
                var serverEx = new HttpRequestException("500 Internal Server Error", null, HttpStatusCode.InternalServerError);
                return MlResult<HttpResponseMessage>.Fail("Server error", detailException: serverEx);
            }

            if (request.Endpoint.Contains("network"))
            {
                var networkEx = new SocketException(10054); // Connection reset by peer
                var httpEx = new HttpRequestException("Network error", networkEx);
                return MlResult<HttpResponseMessage>.Fail("Network failure", detailException: httpEx);
            }

            // Respuesta exitosa simulada
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"{{\"data\": \"Response for {request.Endpoint}\", \"status\": \"success\"}}")
            };

            return MlResult<HttpResponseMessage>.Valid(response);
        }
        catch (Exception ex)
        {
            return MlResult<HttpResponseMessage>.Fail($"Unexpected HTTP error: {ex.Message}", detailException: ex);
        }
    }

    private async Task<MlResult<ApiResponse>> HandleHttpException(ApiRequest request, Exception ex)
    {
        await Task.Delay(150);

        return ex switch
        {
            TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException 
                => await HandleTimeoutException(request, timeoutEx),
            
            HttpRequestException httpEx when httpEx.Data.Contains("StatusCode") 
                => await HandleHttpStatusException(request, httpEx),
            
            HttpRequestException httpEx when httpEx.InnerException is SocketException 
                => await HandleNetworkException(request, httpEx),
            
            SocketException socketEx 
                => await HandleSocketException(request, socketEx),
            
            _ => await HandleGenericHttpException(request, ex)
        };
    }

    private async Task<MlResult<ApiResponse>> HandleTimeoutException(ApiRequest request, TaskCanceledException ex)
    {
        await Task.Delay(100);

        // Para timeouts, intentar con retry automático
        var retryResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "timeout_recovered",
            Data = "Request will be retried automatically",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Automatic retry scheduled",
                RetryScheduled = true,
                NextRetryAt = DateTime.UtcNow.AddMinutes(5)
            }
        };

        return MlResult<ApiResponse>.Valid(retryResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleHttpStatusException(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(120);

        // Extraer status code del mensaje de la excepción
        var statusCode = ExtractStatusCodeFromException(ex);

        return statusCode switch
        {
            HttpStatusCode.Unauthorized => await HandleUnauthorizedError(request, ex),
            HttpStatusCode.TooManyRequests => await HandleRateLimitError(request, ex),
            HttpStatusCode.InternalServerError => await HandleServerError(request, ex),
            HttpStatusCode.BadGateway => await HandleBadGatewayError(request, ex),
            _ => await HandleGenericStatusError(request, ex, statusCode)
        };
    }

    private async Task<MlResult<ApiResponse>> HandleUnauthorizedError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(80);

        // Para errores de autorización, intentar refresh del token
        var authResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "auth_recovery_initiated",
            Data = "Authentication token refresh initiated",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Token refresh and retry",
                RequiresManualIntervention = false,
                TokenRefreshInitiated = true
            }
        };

        return MlResult<ApiResponse>.Valid(authResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleRateLimitError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(90);

        // Para rate limiting, programar retry con backoff
        var rateLimitResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "rate_limit_handled",
            Data = "Request queued for delayed retry",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Exponential backoff retry",
                RetryScheduled = true,
                NextRetryAt = DateTime.UtcNow.AddMinutes(CalculateBackoffMinutes(request.RetryCount)),
                BackoffMultiplier = 2.0
            }
        };

        return MlResult<ApiResponse>.Valid(rateLimitResponse);
    }

    private async Task<MlResult<ApiResponse>> HandleServerError(ApiRequest request, HttpRequestException ex)
    {
        await Task.Delay(110);

        // Para errores del servidor, usar endpoint alternativo si está disponible
        var serverErrorResponse = new ApiResponse
        {
            RequestId = request.Id,
            Status = "server_error_handled",
            Data = "Switched to backup endpoint",
            Timestamp = DateTime.UtcNow,
            RecoveryInfo = new ApiRecoveryInfo
            {
                OriginalException = ex,
                RecoveryMethod = "Backup endpoint failove…