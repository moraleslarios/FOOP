# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos Basado en Excepciones

## Índice
1. [Introducción](#introducción)
2. [BindIfFailWithException - Análisis](#bindiffailwithexception---análisis)
3. [BindIfFailWithoutException - Análisis](#bindiffailwithoutexception---análisis)
4. [Diferencias Clave Entre Ambas Operaciones](#diferencias-clave-entre-ambas-operaciones)
5. [Variantes y Sobrecargas](#variantes-y-sobrecargas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

Esta sección presenta dos operaciones especializadas que manejan errores basándose en la **presencia o ausencia de excepciones** en los detalles del error:

- **BindIfFailWithException**: Ejecuta funciones de recuperación **solo cuando** hay una excepción almacenada en el error
- **BindIfFailWithoutException**: Ejecuta funciones de recuperación **solo cuando NO** hay una excepción almacenada en el error

### Propósito Principal

- **Clasificación de Errores**: Distinguir entre errores causados por excepciones vs. errores lógicos de negocio
- **Manejo Especializado**: Aplicar estrategias diferentes según el origen del error
- **Granularidad de Control**: Control fino sobre qué tipo de errores se procesan
- **Separación de Responsabilidades**: Separar el manejo de errores técnicos vs. errores de negocio

---

## BindIfFailWithException - Análisis

### Comportamiento Fundamental

```
MlResult<T> → ¿Es Fallo?
              ├─ No → Retorna Valor Original
              └─ Sí → ¿Tiene Excepción en ErrorDetails?
                      ├─ Sí → Ejecuta Función de Recuperación(Exception)
                      └─ No → Retorna Error Original (sin procesar)
```

### Filosofía de la Operación

**BindIfFailWithException** actúa únicamente sobre errores que **contienen una excepción**. Si el error no tiene una excepción asociada, la operación no se ejecuta y el error se propaga sin cambios.

### Lógica Interna Clave

```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(), // Sin excepción -> propagar error
        valid: funcException // Con excepción -> ejecutar función
    ),
    valid: value => value
)
```

### Variantes Principales

#### Variante 1: Simple - Solo Recuperación de Excepciones
```csharp
public static MlResult<T> BindIfFailWithException<T>(this MlResult<T> source,
                                                     Func<Exception, MlResult<T>> funcException)
```

#### Variante 2: Con Transformación - Manejo Completo
```csharp
public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(this MlResult<T> source,
                                                                    Func<T, MlResult<TReturn>> funcValid,
                                                                    Func<Exception, MlResult<TReturn>> funcFail)
```

---

## BindIfFailWithoutException - Análisis

### Comportamiento Fundamental

```
MlResult<T> → ¿Es Fallo?
              ├─ No → Retorna Valor Original
              └─ Sí → ¿Tiene Excepción en ErrorDetails?
                      ├─ Sí → Retorna Error Original (sin procesar)
                      └─ No → Ejecuta Función de Recuperación(MlErrorsDetails)
```

### Filosofía de la Operación

**BindIfFailWithoutException** actúa únicamente sobre errores que **NO contienen una excepción**. Estos son típicamente errores de validación, lógica de negocio, o condiciones controladas.

### Lógica Interna Clave

```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: func, // Sin excepción -> ejecutar función
        valid: _ => errorsDetails // Con excepción -> propagar error
    ),
    valid: x => x
)
```

### Variantes Principales

#### Variante 1: Simple - Solo Recuperación de Errores sin Excepción
```csharp
public static MlResult<T> BindIfFailWithoutException<T>(this MlResult<T> source,
                                                        Func<MlErrorsDetails, MlResult<T>> func)
```

#### Variante 2: Con Transformación - Manejo Completo
```csharp
public static MlResult<TReturn> BindIfFailWithoutException<T, TReturn>(this MlResult<T> source,
                                                                       Func<T, MlResult<TReturn>> funcValid,
                                                                       Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
```

---

## Diferencias Clave Entre Ambas Operaciones

### Tabla Comparativa

| Aspecto | BindIfFailWithException | BindIfFailWithoutException |
|---------|------------------------|----------------------------|
| **Actúa sobre** | Errores **con** excepción | Errores **sin** excepción |
| **Función recibe** | `Exception` | `MlErrorsDetails` |
| **Si no hay excepción** | Propaga error original | Ejecuta función |
| **Si hay excepción** | Ejecuta función | Propaga error original |
| **Uso típico** | Manejo de errores técnicos | Manejo de errores de negocio |

### Diferencia con BindIfFailWithValue

Según el comentario en el código:

- **BindIfFailWithValue**: Si no hay `ValueDetail`, añade un nuevo error
- **BindIfFailWithException**: Si no hay `ExceptionDetail`, devuelve el error original sin cambios

---

## Variantes y Sobrecargas

### Soporte Asíncrono Completo

Ambas operaciones incluyen:
- **Funciones asíncronas**: `Func<T, Task<MlResult<T>>>`
- **Fuente asíncrona**: `Task<MlResult<T>>`
- **Todas las combinaciones**: Función síncrona con fuente asíncrona, etc.

### Variantes Try*

Ambas operaciones incluyen versiones `Try*` que capturan excepciones:
- `TryBindIfFailWithException`
- `TryBindIfFailWithoutException`

### Métodos de Merge de Errores

En las versiones `Try*` se utilizan métodos especiales para combinar errores:
- `MergeErrorsDetailsIfFail`: Para mismo tipo
- `MergeErrorsDetailsIfFailDiferentTypes`: Para tipos diferentes

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Archivos con Manejo Diferenciado

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _validator;
    private readonly IFileProcessor _processor;
    private readonly IErrorRecoveryService _recoveryService;
    private readonly ILogger<FileProcessingService> _logger;

    public async Task<MlResult<ProcessedFile>> ProcessFileAsync(FileRequest request)
    {
        return await ValidateFileAsync(request)
            .BindAsync(validFile => ReadFileContentAsync(validFile))
            .BindAsync(fileContent => ProcessFileContentAsync(fileContent))
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Manejo de errores técnicos (I/O, red, sistema)
                await LogTechnicalErrorAsync(request.FilePath, exception);
                return await HandleTechnicalErrorAsync(request, exception);
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Manejo de errores de negocio (validación, formato, contenido)
                await LogBusinessErrorAsync(request.FilePath, errorDetails);
                return await HandleBusinessErrorAsync(request, errorDetails);
            });
    }

    public async Task<MlResult<ProcessedFile>> ProcessFileSafelyAsync(FileRequest request)
    {
        return await ValidateFileAsync(request)
            .BindAsync(validFile => ReadFileContentAsync(validFile))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async exception => await SafeHandleTechnicalErrorAsync(request, exception),
                errorMessageBuilder: ex => $"Failed to handle technical error for file {request.FilePath}: {ex.Message}"
            )
            .BindAsync(fileContent => ProcessFileContentAsync(fileContent))
            .TryBindIfFailWithoutExceptionAsync(
                funcAsync: async errorDetails => await SafeHandleBusinessErrorAsync(request, errorDetails),
                errorMessage: "Failed to handle business logic error"
            );
    }

    private async Task<MlResult<ValidatedFile>> ValidateFileAsync(FileRequest request)
    {
        await Task.Delay(50);

        // Validaciones de negocio que no lanzan excepciones
        if (string.IsNullOrEmpty(request.FilePath))
            return MlResult<ValidatedFile>.Fail("File path is required");

        if (!request.FilePath.EndsWith(".txt") && !request.FilePath.EndsWith(".csv"))
            return MlResult<ValidatedFile>.Fail("Unsupported file format");

        if (request.MaxSizeBytes > 0 && request.EstimatedSizeBytes > request.MaxSizeBytes)
            return MlResult<ValidatedFile>.Fail($"File size {request.EstimatedSizeBytes} exceeds maximum {request.MaxSizeBytes}");

        var validatedFile = new ValidatedFile
        {
            Path = request.FilePath,
            Format = Path.GetExtension(request.FilePath),
            MaxSizeBytes = request.MaxSizeBytes,
            ValidatedAt = DateTime.UtcNow
        };

        return MlResult<ValidatedFile>.Valid(validatedFile);
    }

    private async Task<MlResult<FileContent>> ReadFileContentAsync(ValidatedFile validatedFile)
    {
        await Task.Delay(100);

        try
        {
            // Simular operaciones que pueden lanzar excepciones del sistema
            if (validatedFile.Path.Contains("missing"))
                throw new FileNotFoundException($"File not found: {validatedFile.Path}");

            if (validatedFile.Path.Contains("locked"))
                throw new UnauthorizedAccessException($"Access denied to file: {validatedFile.Path}");

            if (validatedFile.Path.Contains("network"))
                throw new IOException($"Network error reading file: {validatedFile.Path}");

            // Simular errores de negocio sin excepción
            if (validatedFile.Path.Contains("empty"))
                return MlResult<FileContent>.Fail("File is empty");

            if (validatedFile.Path.Contains("corrupted"))
                return MlResult<FileContent>.Fail("File content is corrupted");

            var content = new FileContent
            {
                Path = validatedFile.Path,
                Data = $"Content of {validatedFile.Path}",
                Size = 1024,
                ReadAt = DateTime.UtcNow,
                Encoding = "UTF-8"
            };

            return MlResult<FileContent>.Valid(content);
        }
        catch (Exception ex)
        {
            // Capturar excepción y almacenarla en el error
            return MlResult<FileContent>.Fail($"Error reading file: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> ProcessFileContentAsync(FileContent fileContent)
    {
        await Task.Delay(200);

        try
        {
            // Simular procesamiento que puede lanzar excepciones
            if (fileContent.Data.Contains("SYSTEM_ERROR"))
                throw new InvalidOperationException("System error during processing");

            if (fileContent.Data.Contains("MEMORY_ERROR"))
                throw new OutOfMemoryException("Insufficient memory for processing");

            // Simular errores de negocio
            if (fileContent.Data.Contains("INVALID_FORMAT"))
                return MlResult<ProcessedFile>.Fail("Invalid file format detected");

            if (fileContent.Data.Contains("BUSINESS_RULE_VIOLATION"))
                return MlResult<ProcessedFile>.Fail("Business rule validation failed");

            var processedFile = new ProcessedFile
            {
                OriginalPath = fileContent.Path,
                ProcessedData = $"Processed: {fileContent.Data}",
                ProcessedAt = DateTime.UtcNow,
                ProcessingTimeMs = 200,
                Status = ProcessingStatus.Completed
            };

            return MlResult<ProcessedFile>.Valid(processedFile);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedFile>.Fail($"Processing failed: {ex.Message}", ex);
        }
    }

    // Manejo de errores técnicos (con excepción)
    private async Task<MlResult<ProcessedFile>> HandleTechnicalErrorAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(150);

        var recoveryStrategy = exception switch
        {
            FileNotFoundException => await AttemptFileRecoveryAsync(request),
            UnauthorizedAccessException => await AttemptPermissionRecoveryAsync(request),
            IOException => await AttemptNetworkRecoveryAsync(request),
            OutOfMemoryException => await AttemptMemoryRecoveryAsync(request),
            _ => await CreateTechnicalErrorReportAsync(request, exception)
        };

        return recoveryStrategy;
    }

    // Manejo de errores de negocio (sin excepción)
    private async Task<MlResult<ProcessedFile>> HandleBusinessErrorAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(120);

        var errorMessage = errorDetails.GetMessage();

        var recoveryStrategy = errorMessage switch
        {
            var msg when msg.Contains("File path is required") => await CreateEmptyFileProcessingAsync(request),
            var msg when msg.Contains("Unsupported file format") => await AttemptFormatConversionAsync(request),
            var msg when msg.Contains("exceeds maximum") => await AttemptFileSplittingAsync(request),
            var msg when msg.Contains("File is empty") => await CreateDefaultContentAsync(request),
            var msg when msg.Contains("corrupted") => await AttemptContentRecoveryAsync(request),
            var msg when msg.Contains("Invalid file format") => await CreateValidationReportAsync(request, errorDetails),
            var msg when msg.Contains("Business rule violation") => await CreateBusinessExceptionReportAsync(request, errorDetails),
            _ => await CreateGenericBusinessErrorReportAsync(request, errorDetails)
        };

        return recoveryStrategy;
    }

    // Métodos de recuperación técnica
    private async Task<MlResult<ProcessedFile>> AttemptFileRecoveryAsync(FileRequest request)
    {
        await Task.Delay(100);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File recovered from backup",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredFromBackup,
            RecoveryMethod = "FileNotFound_BackupRecovery"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptPermissionRecoveryAsync(FileRequest request)
    {
        await Task.Delay(80);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed with elevated permissions",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredWithElevatedPermissions,
            RecoveryMethod = "Permission_ElevatedAccess"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptNetworkRecoveryAsync(FileRequest request)
    {
        await Task.Delay(200);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed via alternative network path",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredViaAlternativeNetwork,
            RecoveryMethod = "Network_AlternativePath"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptMemoryRecoveryAsync(FileRequest request)
    {
        await Task.Delay(250);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed in chunks due to memory constraints",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredWithMemoryOptimization,
            RecoveryMethod = "Memory_ChunkedProcessing"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateTechnicalErrorReportAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(100);

        var errorReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Technical error report: {exception.GetType().Name}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.TechnicalErrorReported,
            ErrorDetails = exception.Message,
            RecoveryMethod = "TechnicalError_Report"
        };

        return MlResult<ProcessedFile>.Valid(errorReport);
    }

    // Métodos de recuperación de negocio
    private async Task<MlResult<ProcessedFile>> CreateEmptyFileProcessingAsync(FileRequest request)
    {
        await Task.Delay(50);

        var emptyFile = new ProcessedFile
        {
            OriginalPath = "default_empty_file.txt",
            ProcessedData = "Default empty file content",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedWithDefaults,
            RecoveryMethod = "EmptyPath_DefaultFile"
        };

        return MlResult<ProcessedFile>.Valid(emptyFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptFormatConversionAsync(FileRequest request)
    {
        await Task.Delay(150);

        var convertedFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"File converted from unsupported format",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ConvertedFormat,
            RecoveryMethod = "UnsupportedFormat_Conversion"
        };

        return MlResult<ProcessedFile>.Valid(convertedFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptFileSplittingAsync(FileRequest request)
    {
        await Task.Delay(200);

        var splitFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File split into smaller chunks for processing",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedInChunks,
            RecoveryMethod = "SizeLimit_FileSplitting"
        };

        return MlResult<ProcessedFile>.Valid(splitFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateDefaultContentAsync(FileRequest request)
    {
        await Task.Delay(80);

        var defaultFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "Default content generated for empty file",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedWithDefaults,
            RecoveryMethod = "EmptyContent_DefaultGeneration"
        };

        return MlResult<ProcessedFile>.Valid(defaultFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptContentRecoveryAsync(FileRequest request)
    {
        await Task.Delay(180);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "Content recovered using error correction algorithms",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredContent,
            RecoveryMethod = "CorruptedContent_ErrorCorrection"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateValidationReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(60);

        var validationReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Validation report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ValidationReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "ValidationError_Report"
        };

        return MlResult<ProcessedFile>.Valid(validationReport);
    }

    private async Task<MlResult<ProcessedFile>> CreateBusinessExceptionReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(70);

        var businessReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Business rule violation report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.BusinessRuleViolationReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "BusinessRule_ViolationReport"
        };

        return MlResult<ProcessedFile>.Valid(businessReport);
    }

    private async Task<MlResult<ProcessedFile>> CreateGenericBusinessErrorReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(90);

        var genericReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Business error report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.BusinessErrorReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "BusinessError_GenericReport"
        };

        return MlResult<ProcessedFile>.Valid(genericReport);
    }

    // Versiones "Safe" para TryBind
    private async Task<MlResult<ProcessedFile>> SafeHandleTechnicalErrorAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(100);

        // Versión segura que puede fallar de forma controlada
        if (exception is OutOfMemoryException && request.EstimatedSizeBytes > 1_000_000)
            throw new InvalidOperationException("Cannot recover from memory error with large files");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Safe recovery from {exception.GetType().Name}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.SafeRecovery,
            RecoveryMethod = "Safe_TechnicalErrorHandling"
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    private async Task<MlResult<ProcessedFile>> SafeHandleBusinessErrorAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(80);

        // Versión segura que puede fallar de forma controlada
        if (errorDetails.GetMessage().Contains("critical"))
            throw new BusinessException("Cannot safely handle critical business errors");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Safe business error recovery",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.SafeRecovery,
            RecoveryMethod = "Safe_BusinessErrorHandling"
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    // Métodos de logging
    private async Task LogTechnicalErrorAsync(string filePath, Exception exception)
    {
        await Task.Delay(10);
        _logger.LogError(exception, "Technical error processing file {FilePath}", filePath);
    }

    private async Task LogBusinessErrorAsync(string filePath, MlErrorsDetails errorDetails)
    {
        await Task.Delay(10);
        _logger.LogWarning("Business error processing file {FilePath}: {Error}", filePath, errorDetails.GetMessage());
    }
}

// Clases de apoyo para procesamiento de archivos
public enum ProcessingStatus
{
    Completed,
    RecoveredFromBackup,
    RecoveredWithElevatedPermissions,
    RecoveredViaAlternativeNetwork,
    RecoveredWithMemoryOptimization,
    TechnicalErrorReported,
    ProcessedWithDefaults,
    ConvertedFormat,
    ProcessedInChunks,
    RecoveredContent,
    ValidationReported,
    BusinessRuleViolationReported,
    BusinessErrorReported,
    SafeRecovery
}

public class FileRequest
{
    public string FilePath { get; set; }
    public long EstimatedSizeBytes { get; set; }
    public long MaxSizeBytes { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class ValidatedFile
{
    public string Path { get; set; }
    public string Format { get; set; }
    public long MaxSizeBytes { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class FileContent
{
    public string Path { get; set; }
    public string Data { get; set; }
    public long Size { get; set; }
    public DateTime ReadAt { get; set; }
    public string Encoding { get; set; }
}

public class ProcessedFile
{
    public string OriginalPath { get; set; }
    public string ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
    public int ProcessingTimeMs { get; set; }
    public ProcessingStatus Status { get; set; }
    public string RecoveryMethod { get; set; }
    public string ErrorDetails { get; set; }
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

public interface IFileValidator { }
public interface IFileProcessor { }
public interface IErrorRecoveryService { }
```

### Ejemplo 2: Sistema de Autenticación y Autorización con Manejo Diferenciado

```csharp
public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ISecurityLogger _securityLogger;

    public async Task<MlResult<AuthenticationResult>> AuthenticateAsync(LoginRequest request)
    {
        return await ValidateLoginRequestAsync(request)
            .BindAsync(validRequest => FindUserAsync(validRequest.Username))
            .BindAsync(user => ValidatePasswordAsync(user, request.Password))
            .BindAsync(authenticatedUser => GenerateTokenAsync(authenticatedUser))
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Manejo de errores técnicos (BD, red, servicios externos)
                await LogSecurityTechnicalErrorAsync(request.Username, exception);
                return await HandleTechnicalAuthErrorAsync(request, exception);
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Manejo de errores de seguridad/negocio (credenciales, validación)
                await LogSecurityBusinessErrorAsync(request.Username, errorDetails);
                return await HandleSecurityBusinessErrorAsync(request, errorDetails);
            });
    }

    public async Task<MlResult<AuthenticationResult>> AuthenticateSafelyAsync(LoginRequest request)
    {
        return await ValidateLoginRequestAsync(request)
            .BindAsync(validRequest => FindUserAsync(validRequest.Username))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async exception => await SafeHandleDatabaseErrorAsync(request, exception),
                errorMessageBuilder: ex => $"Database recovery failed for user {request.Username}: {ex.Message}"
            )
            .BindAsync(user => ValidatePasswordAsync(user, request.Password))
            .TryBindIfFailWithoutExceptionAsync(
                funcAsync: async errorDetails => await SafeHandleCredentialErrorAsync(request, errorDetails),
                errorMessage: "Safe credential error handling failed"
            );
    }

    private async Task<MlResult<LoginRequest>> ValidateLoginRequestAsync(LoginRequest request)
    {
        await Task.Delay(30);

        // Validaciones de negocio sin excepciones
        if (string.IsNullOrEmpty(request.Username))
            return MlResult<LoginRequest>.Fail("Username is required");

        if (string.IsNullOrEmpty(request.Password))
            return MlResult<LoginRequest>.Fail("Password is required");

        if (request.Username.Length < 3)
            return MlResult<LoginRequest>.Fail("Username must be at least 3 characters");

        if (request.Password.Length < 6)
            return MlResult<LoginRequest>.Fail("Password must be at least 6 characters");

        return MlResult<LoginRequest>.Valid(request);
    }

    private async Task<MlResult<User>> FindUserAsync(string username)
    {
        await Task.Delay(100);

        try
        {
            // Simular operaciones de BD que pueden lanzar excepciones
            if (username.Contains("db_error"))
                throw new SqlException("Database connection failed");

            if (username.Contains("timeout"))
                throw new TimeoutException("Database query timeout");

            if (username.Contains("network"))
                throw new NetworkException("Network connectivity issue");

            // Simular errores de negocio sin excepción
            if (username.Contains("notfound"))
                return MlResult<User>.Fail("User not found");

            if (username.Contains("disabled"))
                return MlResult<User>.Fail("User account is disabled");

            if (username.Contains("suspended"))
                return MlResult<User>.Fail("User account is suspended");

            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = "hashed_password_123",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow.AddDays(-1),
                FailedLoginAttempts = 0
            };

            return MlResult<User>.Valid(user);
        }
        catch (Exception ex)
        {
            return MlResult<User>.Fail($"Database error: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<User>> ValidatePasswordAsync(User user, string password)
    {
        await Task.Delay(80);

        try
        {
            // Simular validación que puede lanzar excepciones técnicas
            if (password.Contains("hash_error"))
                throw new CryptographicException("Password hashing service error");

            if (password.Contains("service_down"))
                throw new ServiceUnavailableException("Password validation service unavailable");

            // Simular errores de negocio sin excepción
            if (password != "correct_password")
                return MlResult<User>.Fail("Invalid password");

            if (user.FailedLoginAttempts >= 5)
                return MlResult<User>.Fail("Account locked due to too many failed attempts");

            if (!user.IsActive)
                return MlResult<User>.Fail("User account is not active");

            // Simular validación exitosa
            user.LastLoginAt = DateTime.UtcNow;
            user.FailedLoginAttempts = 0;

            return MlResult<User>.Valid(user);
        }
        catch (Exception ex)
        {
            return MlResult<User>.Fail($"Password validation error: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<AuthenticationResult>> GenerateTokenAsync(User user)
    {
        await Task.Delay(60);

        try
        {
            // Simular generación de token que puede fallar técnicamente
            if (user.Username.Contains("token_error"))
                throw new SecurityTokenException("Token generation failed");

            if (user.Username.Contains("key_error"))
                throw new SecurityTokenSignatureKeyNotFoundException("Signing key not found");

            var token = new AuthenticationResult
            {
                UserId = user.Id,
                Username = user.Username,
                Token = $"token_{Guid.NewGuid()}",
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IssuedAt = DateTime.UtcNow,
                TokenType = "Bearer"
            };

            return MlResult<AuthenticationResult>.Valid(token);
        }
        catch (Exception ex)
        {
            return MlResult<AuthenticationResult>.Fail($"Token generation error: {ex.Message}", ex);
        }
    }

    // Manejo de errores técnicos (con excepción)
    private async Task<MlResult<AuthenticationResult>> HandleTechnicalAuthErrorAsync(LoginRequest request, Exception exception)
    {
        await Task.Delay(100);

        var recoveryResult = exception switch
        {
            SqlException => await AttemptDatabaseRecoveryAsync(request),
            TimeoutException => await AttemptTimeoutRecoveryAsync(request),
            NetworkException => await AttemptNetworkRecoveryAsync(request),
            CryptographicException => await AttemptCryptoRecoveryAsync(request),
            SecurityTokenException => await AttemptTokenRecoveryAsync(request),
            ServiceUnavailableException => await AttemptServiceRecoveryAsync(request),
            _ => await CreateTechnicalErrorResponseAsync(request, exception)
        };

        return recoveryResult;
    }

    // Manejo de errores de negocio (sin excepción)
    private async Task<MlResult<AuthenticationResult>> HandleSecurityBusinessErrorAsync(LoginRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(80);

        var errorMessage = errorDetails.GetMessage();

        var recoveryResult = errorMessage switch
        {
            var msg when msg.Contains("Username is required") => await CreateGuestSessionAsync(),
            var msg when msg.Contains("Password is required") => await CreateAnonymousSessionAsync(),
            var msg when msg.Contains("User not found") => await CreateUserRegistrationPromptAsync(request),
            var msg when msg.Contains("disabled") => await CreateAccountRecoveryPromptAsync(request),
            var msg when msg.Contains("suspended") => await CreateAppealProcessPromptAsync(request),
            var msg when msg.Contains("Invalid password") => await HandleInvalidPasswordAsync(request),
            var msg when msg.Contains("Account locked") => await CreateAccountUnlockPromptAsync(request),
            var msg when msg.Contains("not active") => await CreateActivationPromptAsync(request),
            _ => await CreateGenericSecurityErrorResponseAsync(request, errorDetails)
        };

        return recoveryResult;
    }

    // Métodos de recuperación técnica
    private async Task<MlResult<AuthenticationResult>> AttemptDatabaseRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(150);

        var fallbackAuth = new AuthenticationResult
        {
            UserId = -1,
            Username = request.Username,
            Token = $"fallback_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30), // Token más corto para fallback
            IssuedAt = DateTime.UtcNow,
            TokenType = "Fallback",
            IsFallbackAuthentication = true,
            RecoveryMethod = "DatabaseFallback"
        };

        return MlResult<AuthenticationResult>.Valid(fallbackAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptTimeoutRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(120);

        var quickAuth = new AuthenticationResult
        {
            UserId = -2,
            Username = request.Username,
            Token = $"quick_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Token muy corto
            IssuedAt = DateTime.UtcNow,
            TokenType = "Quick",
            IsQuickAuthentication = true,
            RecoveryMethod = "TimeoutQuickAuth"
        };

        return MlResult<AuthenticationResult>.Valid(quickAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptNetworkRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(200);

        var offlineAuth = new AuthenticationResult
        {
            UserId = -3,
            Username = request.Username,
            Token = $"offline_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Offline",
            IsOfflineAuthentication = true,
            RecoveryMethod = "NetworkOfflineAuth"
        };

        return MlResult<AuthenticationResult>.Valid(offlineAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptCryptoRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(100);

        var basicAuth = new AuthenticationResult
        {
            UserId = -4,
            Username = request.Username,
            Token = $"basic_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Basic",
            IsBasicAuthentication = true,
            RecoveryMethod = "CryptoBasicAuth"
        };

        return MlResult<AuthenticationResult>.Valid(basicAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptTokenRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(80);

        var simpleAuth = new AuthenticationResult
        {
            UserId = -5,
            Username = request.Username,
            Token = $"simple_token_{DateTime.UtcNow.Ticks}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Simple",
            IsSimpleAuthentication = true,
            RecoveryMethod = "TokenSimpleAuth"
        };

        return MlResult<AuthenticationResult>.Valid(simpleAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptServiceRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(120);

        var emergencyAuth = new AuthenticationResult
        {
            UserId = -6,
            Username = request.Username,
            Token = $"emergency_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(45),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Emergency",
            IsEmergencyAuthentication = true,
            RecoveryMethod = "ServiceEmergencyAuth"
        };

        return MlResult<AuthenticationResult>.Valid(emergencyAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateTechnicalErrorResponseAsync(LoginRequest request, Exception exception)
    {
        await Task.Delay(60);

        var errorAuth = new AuthenticationResult
        {
            UserId = -999,
            Username = request.Username,
            Token = "error_token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Error",
            IsErrorResponse = true,
            ErrorMessage = exception.Message,
            RecoveryMethod = "TechnicalErrorResponse"
        };

        return MlResult<AuthenticationResult>.Valid(errorAuth);
    }

    // Métodos de recuperación de negocio
    private async Task<MlResult<AuthenticationResult>> CreateGuestSessionAsync()
    {
        await Task.Delay(40);

        var guestAuth = new AuthenticationResult
        {
            UserId = 0,
            Username = "guest",
            Token = $"guest_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Guest",
            IsGuestSession = true,
            RecoveryMethod = "NoUsername_GuestSession"
        };

        return MlResult<AuthenticationResult>.Valid(guestAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateAnonymousSessionAsync()
    {
        await Task.Delay(30);

        var anonymousAuth = new AuthenticationResult
        {
            UserId = -10,
            Username = "anonymous",
            Token = $"anonymous_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Anonymous",
            IsAnonymousSession = true,
            RecoveryMethod = "NoPassword_AnonymousSession"
        };

        return MlResult<AuthenticationResult>.Valid(anonymousAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateUserRegistrationPromptAsync(LoginRequest request)
    {
        await Task.Delay(50);

        var registrationPrompt = new AuthenticationResult
        {
            UserId = -20,
            Username = request.Username,
            Token = $"registration_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IssuedAt = DateTime.UtcNow,
            TokenType = "RegistrationPrompt",
            RequiresRegistration = true,
            RecoveryMethod = "UserNotFound_RegistrationPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(registrationPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> CreateAccountRecoveryPromptAsync(LoginRequest request)
    {
        await Task.Delay(60);

        var recoveryPrompt = new AuthenticationResult
        {
            UserId = -30,
            Username = request.Username,
            Token = $"recovery_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(20),
            IssuedAt = DateTime.UtcNow,
            TokenType = "RecoveryPrompt",
            RequiresAccountRecovery = true,
            RecoveryMethod = "AccountDisabled_RecoveryPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(recoveryPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> CreateAppealProcessPromptAsync(LoginRequest request)
    {
        await Task.Delay(70);

        var appealPrompt = new AuthenticationResult
        {
            UserId = -40,
            Username = request.Username,
            Token = $"appeal_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IssuedAt = DateTime.UtcNow,
            TokenType = "AppealPrompt",
            RequiresAppeal = true,
            RecoveryMethod = "AccountSuspended_AppealPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(appealPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> HandleInvalidPasswordAsync(LoginRequest request)
    {
        await Task.Delay(90);

        var passwordResetPrompt = new AuthenticationResult
        {
            UserId = -50,
            Username = request.Username,
            Token = $"password_reset_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(25),
            IssuedAt = DateTime.UtcNow,
            TokenType = "PasswordResetPrompt",
            RequiresPasswordReset = true,
            RecoveryMethod = "InvalidPassword_ResetPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(passwordResetPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> CreateAccountUnlockPromptAsync(LoginRequest request)
    {
        await Task.Delay(80);

        var unlockPrompt = new AuthenticationResult
        {
            UserId = -60,
            Username = request.Username,
            Token = $"unlock_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IssuedAt = DateTime.UtcNow,
            TokenType = "UnlockPrompt",
            RequiresAccountUnlock = true,
            RecoveryMethod = "AccountLocked_UnlockPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(unlockPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> CreateActivationPromptAsync(LoginRequest request)
    {
        await Task.Delay(45);

        var activationPrompt = new AuthenticationResult
        {
            UserId = -70,
            Username = request.Username,
            Token = $"activation_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            TokenType = "ActivationPrompt",
            RequiresActivation = true,
            RecoveryMethod = "AccountInactive_ActivationPrompt"
        };

        return MlResult<AuthenticationResult>.Valid(activationPrompt);
    }

    private async Task<MlResult<AuthenticationResult>> CreateGenericSecurityErrorResponseAsync(LoginRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(55);

        var securityError = new AuthenticationResult
        {
            UserId = -100,
            Username = request.Username,
            Token = $"security_error_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IssuedAt = DateTime.UtcNow,
            TokenType = "SecurityError",
            IsSecurityError = true,
            ErrorMessage = errorDetails.GetMessage(),
            RecoveryMethod = "GenericSecurityError_Response"
        };

        return MlResult<AuthenticationResult>.Valid(securityError);
    }

    // Versiones "Safe" para TryBind
    private async Task<MlResult<AuthenticationResult>> SafeHandleDatabaseErrorAsync(LoginRequest request, Exception exception)
    {
        await Task.Delay(100);

        if (exception is SqlException && request.Username.Contains("critical"))
            throw new SecurityException("Cannot safely handle database error for critical user");

        var safeAuth = new AuthenticationResult
        {
            UserId = -1000,
            Username = request.Username,
            Token = $"safe_db_recovery_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IssuedAt = DateTime.UtcNow,
            TokenType = "SafeDatabaseRecovery",
            IsSafeRecovery = true,
            RecoveryMethod = "Safe_DatabaseErrorHandling"
        };

        return MlResult<AuthenticationResult>.Valid(safeAuth);
    }

    private async Task<MlResult<AuthenticationResult>> SafeHandleCredentialErrorAsync(LoginRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(60);

        if (errorDetails.GetMessage().Contains("security_critical"))
            throw new SecurityException("Cannot safely handle critical security error");

        var safeAuth = new AuthenticationResult
        {
            UserId = -2000,
            Username = request.Username,
            Token = $"safe_credential_recovery_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IssuedAt = DateTime.UtcNow,
            TokenType = "SafeCredentialRecovery",
            IsSafeRecovery = true,
            RecoveryMethod = "Safe_CredentialErrorHandling"
        };

        return MlResult<AuthenticationResult>.Valid(safeAuth);
    }

    // Métodos de logging de seguridad
    private async Task LogSecurityTechnicalErrorAsync(string username, Exception exception)
    {
        await Task.Delay(10);
        await _securityLogger.LogTechnicalErrorAsync(username, exception);
    }

    private async Task LogSecurityBusinessErrorAsync(string username, MlErrorsDetails errorDetails)
    {
        await Task.Delay(10);
        await _securityLogger.LogBusinessErrorAsync(username, errorDetails);
    }
}

// Clases de apoyo para autenticación
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ClientId { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; }
}

public class AuthenticationResult
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    public string TokenType { get; set; }
    public string RecoveryMethod { get; set; }
    public string ErrorMessage { get; set; }
    
    // Flags para diferentes tipos de autenticación
    public bool IsFallbackAuthentication { get; set; }
    public bool IsQuickAuthentication { get; set; }
    public bool IsOfflineAuthentication { get; set; }
    public bool IsBasicAuthentication { get; set; }
    public bool IsSimpleAuthentication { get; set; }
    public bool IsEmergencyAuthentication { get; set; }
    public bool IsErrorResponse { get; set; }
    public bool IsGuestSession { get; set; }
    public bool IsAnonymousSession { get; set; }
    public bool IsSafeRecovery { get; set; }
    public bool IsSecurityError { get; set; }
    
    // Flags para acciones requeridas
    public bool RequiresRegistration { get; set; }
    public bool RequiresAccountRecovery { get; set; }
    public bool RequiresAppeal { get; set; }
    public bool RequiresPasswordReset { get; set; }
    public bool RequiresAccountUnlock { get; set; }
    public bool RequiresActivation { get; set; }
}

// Excepciones personalizadas
public class SqlException : Exception
{
    public SqlException(string message) : base(message) { }
}

public class NetworkException : Exception
{
    public NetworkException(string message) : base(message) { }
}

public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message) { }
}

public class SecurityTokenException : Exception
{
    public SecurityTokenException(string message) : base(message) { }
}

public class SecurityTokenSignatureKeyNotFoundException : Exception
{
    public SecurityTokenSignatureKeyNotFoundException(string message) : base(message) { }
}

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
}

public class CryptographicException : Exception
{
    public CryptographicException(string message) : base(message) { }
}

// Interfaces
public interface IUserRepository { }
public interface ITokenService { }
public interface ISecurityLogger
{
    Task LogTechnicalErrorAsync(string username, Exception exception);
    Task LogBusinessErrorAsync(string username, MlErrorsDetails errorDetails);
}
```

---

## Mejores Prácticas

### 1. Clasificación Correcta de Errores

```csharp
// ✅ Correcto: Separar claramente errores técnicos vs. de negocio
public async Task<MlResult<ProcessedData>> ProcessDataAsync(InputData data)
{
    return await ValidateData(data)
        .BindAsync(validData => TransformData(validData))
        .BindIfFailWithExceptionAsync(async exception =>
        {
            // Solo errores que vienen de excepciones (técnicos)
            await LogTechnicalError(exception);
            return await AttemptTechnicalRecovery(exception);
        })
        .BindIfFailWithoutExceptionAsync(async errorDetails =>
        {
            // Solo errores de validación/negocio (sin excepción)
            await LogBusinessError(errorDetails);
            return await AttemptBusinessRecovery(errorDetails);
        });
}

// ❌ Incorrecto: Usar BindIfFail genérico cuando se puede ser específico
public async Task<MlResult<ProcessedData>> ProcessDataAsync(InputData data)
{
    return await ValidateData(data)
        .BindIfFailAsync(async errors => 
        {
            // No distingue entre errores técnicos y de negocio
            return await GenericRecovery(errors);
        });
}
```

### 2. Manejo Específico por Tipo de Error

```csharp
// ✅ Correcto: Estrategias específicas para cada tipo
public class ErrorHandlingService
{
    public async Task<MlResult<T>> HandleErrorsSpecificallyAsync<T>(MlResult<T> result)
    {
        return await result
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Estrategias para errores técnicos
                return exception switch
                {
                    SqlException sqlEx => await HandleDatabaseError<T>(sqlEx),
                    HttpRequestException httpEx => await HandleNetworkError<T>(httpEx),
                    TimeoutException timeoutEx => await HandleTimeoutError<T>(timeoutEx),
                    OutOfMemoryException memEx => await HandleMemoryError<T>(memEx),
                    _ => await HandleGenericTechnicalError<T>(exception)
                };
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Estrategias para errores de negocio
                var message = errorDetails.GetMessage();
                
                return message switch
                {
                    var msg when msg.Contains("validation") => await HandleValidationError<T>(errorDetails),
                    var msg when msg.Contains("business rule") => await HandleBusinessRuleError<T>(errorDetails),
                    var msg when msg.Contains("permission") => await HandlePermissionError<T>(errorDetails),
                    var msg when msg.Contains("not found") => await HandleNotFoundError<T>(errorDetails),
                    _ => await HandleGenericBusinessError<T>(errorDetails)
                };
            });
    }
}
```

### 3. Logging Diferenciado

```csharp
// ✅ Correcto: Logging específico para cada tipo de error
public class LoggingService
{
    private readonly ILogger<LoggingService> _logger;
    
    public async Task<MlResult<T>> ProcessWithLoggingAsync<T>(MlResult<T> result)
    {
        return await result
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Log técnico con detalles para desarrolladores
                _logger.LogError(exception, 
                    "Technical error: {ExceptionType} - {Message}. Stack: {StackTrace}",
                    exception.GetType().Name,
                    exception.Message,
                    exception.StackTrace);
                
                // Notificar a equipos técnicos
                await NotifyTechnicalTeam(exception);
                
                return await AttemptTechnicalRecovery<T>(exception);
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Log de negocio para análisis y métricas
                _logger.LogWarning(
                    "Business error: {ErrorMessage}. Details: {ErrorDetails}",
                    errorDetails.GetMessage(),
                    errorDetails.GetDetails());
                
                // Métricas de negocio
                await UpdateBusinessErrorMetrics(errorDetails);
                
                return await AttemptBusinessRecovery<T>(errorDetails);
            });
    }
}
```

### 4. Testing Granular

```csharp
// ✅ Correcto: Tests específicos para cada tipo de error
[TestClass]
public class ErrorHandlingTests
{
    [Test]
    public async Task BindIfFailWithException_TechnicalError_UsesAppropriateRecovery()
    {
        // Arrange
        var exception = new SqlException("Database connection failed");
        var failingResult = MlResult<Data>.Fail("DB Error", exception);
        
        // Act
        var result = await failingResult
            .BindIfFailWithExceptionAsync(async ex => await CreateDatabaseFallback());
        
        // Assert
        result.Should().BeSuccessful();
        result.Value.RecoveryType.Should().Be("DatabaseFallback");
    }
    
    [Test]
    public async Task BindIfFailWithoutException_BusinessError_UsesAppropriateRecovery()
    {
        // Arrange
        var failingResult = MlResult<Data>.Fail("Validation failed: Required field missing");
        
        // Act
        var result = await failingResult
            .BindIfFailWithoutExceptionAsync(async errors => await CreateValidationRecovery(errors));
        
        // Assert
        result.Should().BeSuccessful();
        result.Value.RecoveryType.Should().Be("ValidationRecovery");
    }
    
    [Test]
    public async Task BindIfFailWithException_BusinessErrorWithoutException_DoesNotExecute()
    {
        // Arrange
        var businessError = MlResult<Data>.Fail("Business rule violation"); // Sin excepción
        var recoveryExecuted = false;
        
        // Act
        var result = await businessError
            .BindIfFailWithExceptionAsync(async ex =>
            {
                recoveryExecuted = true;
                return await CreateRecovery();
            });
        
        // Assert
        result.Should().BeFailure();
        recoveryExecuted.Should().BeFalse();
    }
    
    [Test]
    public async Task BindIfFailWithoutException_TechnicalErrorWithException_DoesNotExecute()
    {
        // Arrange
        var exception = new IOException("File not found");
        var technicalError = MlResult<Data>.Fail("IO Error", exception);
        var recoveryExecuted = false;
        
        // Act
        var result = await technicalError
            .BindIfFailWithoutExceptionAsync(async errors =>
            {
                recoveryExecuted = true;
                return await CreateRecovery();
            });
        
        // Assert
        result.Should().BeFailure();
        recoveryExecuted.Should().BeFalse();
    }
}
```

### 5. Composición de Ambas Estrategias

```csharp
// ✅ Correcto: Usar ambas operaciones en pipeline completo
public async Task<MlResult<FinalResult>> CompleteProcessingPipelineAsync(InputData input)
{
    return await ValidateInput(input)
        .BindAsync(validInput => ProcessStep1(validInput))
        .BindIfFailWithoutExceptionAsync(async validationErrors =>
        {
            // Manejo de errores de validación
            await LogValidationFailure(validationErrors);
            return await CreateValidationRecovery(input, validationErrors);
        })
        .BindAsync(step1Result => ProcessStep2(step1Result))
        .BindIfFailWithExceptionAsync(async technicalException =>
        {
            // Manejo de errores técnicos en step2
            await LogTechnicalFailure(technicalException);
            return await CreateTechnicalRecovery(input, technicalException);
        })
        .BindAsync(step2Result => ProcessStep3(step2Result))
        .BindIfFailWithoutExceptionAsync(async businessErrors =>
        {
            // Manejo de errores de negocio en step3
            await LogBusinessFailure(businessErrors);
            return await CreateBusinessRecovery(input, businessErrors);
        })
        .BindIfFailWithExceptionAsync(async finalTechnicalException =>
        {
            // Último recurso para errores técnicos
            await LogCriticalTechnicalFailure(finalTechnicalException);
            return await CreateFinalTechnicalRecovery(input, finalTechnicalException);
        });
}
```

### 6. Manejo de Errores en Versiones Try*

```csharp
// ✅ Correcto: Usar Try* cuando las funciones de recuperación pueden fallar
public async Task<MlResult<ProcessedData>> ProcessWithSafeRecoveryAsync(InputData input)
{
    return await ProcessData(input)
        .TryBindIfFailWithExceptionAsync(
            funcExceptionAsync: async exception => await RiskyTechnicalRecovery(exception),
            errorMessageBuilder: ex => $"Technical recovery failed: {ex.Message}"
        )
        .TryBindIfFailWithoutExceptionAsync# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos Basado en Excepciones

## Índice
1. [Introducción](#introducción)
2. [BindIfFailWithException - Análisis](#bindiffailwithexception---análisis)
3. [BindIfFailWithoutException - Análisis](#bindiffailwithoutexception---análisis)
4. [Diferencias Clave Entre Ambas Operaciones](#diferencias-clave-entre-ambas-operaciones)
5. [Variantes y Sobrecargas](#variantes-y-sobrecargas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

Esta sección presenta dos operaciones especializadas que manejan errores basándose en la **presencia o ausencia de excepciones** en los detalles del error:

- **BindIfFailWithException**: Ejecuta funciones de recuperación **solo cuando** hay una excepción almacenada en el error
- **BindIfFailWithoutException**: Ejecuta funciones de recuperación **solo cuando NO** hay una excepción almacenada en el error

### Propósito Principal

- **Clasificación de Errores**: Distinguir entre errores causados por excepciones vs. errores lógicos de negocio
- **Manejo Especializado**: Aplicar estrategias diferentes según el origen del error
- **Granularidad de Control**: Control fino sobre qué tipo de errores se procesan
- **Separación de Responsabilidades**: Separar el manejo de errores técnicos vs. errores de negocio

---

## BindIfFailWithException - Análisis

### Comportamiento Fundamental

```
MlResult<T> → ¿Es Fallo?
              ├─ No → Retorna Valor Original
              └─ Sí → ¿Tiene Excepción en ErrorDetails?
                      ├─ Sí → Ejecuta Función de Recuperación(Exception)
                      └─ No → Retorna Error Original (sin procesar)
```

### Filosofía de la Operación

**BindIfFailWithException** actúa únicamente sobre errores que **contienen una excepción**. Si el error no tiene una excepción asociada, la operación no se ejecuta y el error se propaga sin cambios.

### Lógica Interna Clave

```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: exErrorsDetails => exErrorsDetails.ToMlResultFail<T>(), // Sin excepción -> propagar error
        valid: funcException // Con excepción -> ejecutar función
    ),
    valid: value => value
)
```

### Variantes Principales

#### Variante 1: Simple - Solo Recuperación de Excepciones
```csharp
public static MlResult<T> BindIfFailWithException<T>(this MlResult<T> source,
                                                     Func<Exception, MlResult<T>> funcException)
```

#### Variante 2: Con Transformación - Manejo Completo
```csharp
public static MlResult<TReturn> BindIfFailWithException<T, TReturn>(this MlResult<T> source,
                                                                    Func<T, MlResult<TReturn>> funcValid,
                                                                    Func<Exception, MlResult<TReturn>> funcFail)
```

---

## BindIfFailWithoutException - Análisis

### Comportamiento Fundamental

```
MlResult<T> → ¿Es Fallo?
              ├─ No → Retorna Valor Original
              └─ Sí → ¿Tiene Excepción en ErrorDetails?
                      ├─ Sí → Retorna Error Original (sin procesar)
                      └─ No → Ejecuta Función de Recuperación(MlErrorsDetails)
```

### Filosofía de la Operación

**BindIfFailWithoutException** actúa únicamente sobre errores que **NO contienen una excepción**. Estos son típicamente errores de validación, lógica de negocio, o condiciones controladas.

### Lógica Interna Clave

```csharp
source.Match(
    fail: errorsDetails => errorsDetails.GetDetailException().Match(
        fail: func, // Sin excepción -> ejecutar función
        valid: _ => errorsDetails // Con excepción -> propagar error
    ),
    valid: x => x
)
```

### Variantes Principales

#### Variante 1: Simple - Solo Recuperación de Errores sin Excepción
```csharp
public static MlResult<T> BindIfFailWithoutException<T>(this MlResult<T> source,
                                                        Func<MlErrorsDetails, MlResult<T>> func)
```

#### Variante 2: Con Transformación - Manejo Completo
```csharp
public static MlResult<TReturn> BindIfFailWithoutException<T, TReturn>(this MlResult<T> source,
                                                                       Func<T, MlResult<TReturn>> funcValid,
                                                                       Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
```

---

## Diferencias Clave Entre Ambas Operaciones

### Tabla Comparativa

| Aspecto | BindIfFailWithException | BindIfFailWithoutException |
|---------|------------------------|----------------------------|
| **Actúa sobre** | Errores **con** excepción | Errores **sin** excepción |
| **Función recibe** | `Exception` | `MlErrorsDetails` |
| **Si no hay excepción** | Propaga error original | Ejecuta función |
| **Si hay excepción** | Ejecuta función | Propaga error original |
| **Uso típico** | Manejo de errores técnicos | Manejo de errores de negocio |

### Diferencia con BindIfFailWithValue

Según el comentario en el código:

- **BindIfFailWithValue**: Si no hay `ValueDetail`, añade un nuevo error
- **BindIfFailWithException**: Si no hay `ExceptionDetail`, devuelve el error original sin cambios

---

## Variantes y Sobrecargas

### Soporte Asíncrono Completo

Ambas operaciones incluyen:
- **Funciones asíncronas**: `Func<T, Task<MlResult<T>>>`
- **Fuente asíncrona**: `Task<MlResult<T>>`
- **Todas las combinaciones**: Función síncrona con fuente asíncrona, etc.

### Variantes Try*

Ambas operaciones incluyen versiones `Try*` que capturan excepciones:
- `TryBindIfFailWithException`
- `TryBindIfFailWithoutException`

### Métodos de Merge de Errores

En las versiones `Try*` se utilizan métodos especiales para combinar errores:
- `MergeErrorsDetailsIfFail`: Para mismo tipo
- `MergeErrorsDetailsIfFailDiferentTypes`: Para tipos diferentes

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Archivos con Manejo Diferenciado

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _validator;
    private readonly IFileProcessor _processor;
    private readonly IErrorRecoveryService _recoveryService;
    private readonly ILogger<FileProcessingService> _logger;

    public async Task<MlResult<ProcessedFile>> ProcessFileAsync(FileRequest request)
    {
        return await ValidateFileAsync(request)
            .BindAsync(validFile => ReadFileContentAsync(validFile))
            .BindAsync(fileContent => ProcessFileContentAsync(fileContent))
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Manejo de errores técnicos (I/O, red, sistema)
                await LogTechnicalErrorAsync(request.FilePath, exception);
                return await HandleTechnicalErrorAsync(request, exception);
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Manejo de errores de negocio (validación, formato, contenido)
                await LogBusinessErrorAsync(request.FilePath, errorDetails);
                return await HandleBusinessErrorAsync(request, errorDetails);
            });
    }

    public async Task<MlResult<ProcessedFile>> ProcessFileSafelyAsync(FileRequest request)
    {
        return await ValidateFileAsync(request)
            .BindAsync(validFile => ReadFileContentAsync(validFile))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async exception => await SafeHandleTechnicalErrorAsync(request, exception),
                errorMessageBuilder: ex => $"Failed to handle technical error for file {request.FilePath}: {ex.Message}"
            )
            .BindAsync(fileContent => ProcessFileContentAsync(fileContent))
            .TryBindIfFailWithoutExceptionAsync(
                funcAsync: async errorDetails => await SafeHandleBusinessErrorAsync(request, errorDetails),
                errorMessage: "Failed to handle business logic error"
            );
    }

    private async Task<MlResult<ValidatedFile>> ValidateFileAsync(FileRequest request)
    {
        await Task.Delay(50);

        // Validaciones de negocio que no lanzan excepciones
        if (string.IsNullOrEmpty(request.FilePath))
            return MlResult<ValidatedFile>.Fail("File path is required");

        if (!request.FilePath.EndsWith(".txt") && !request.FilePath.EndsWith(".csv"))
            return MlResult<ValidatedFile>.Fail("Unsupported file format");

        if (request.MaxSizeBytes > 0 && request.EstimatedSizeBytes > request.MaxSizeBytes)
            return MlResult<ValidatedFile>.Fail($"File size {request.EstimatedSizeBytes} exceeds maximum {request.MaxSizeBytes}");

        var validatedFile = new ValidatedFile
        {
            Path = request.FilePath,
            Format = Path.GetExtension(request.FilePath),
            MaxSizeBytes = request.MaxSizeBytes,
            ValidatedAt = DateTime.UtcNow
        };

        return MlResult<ValidatedFile>.Valid(validatedFile);
    }

    private async Task<MlResult<FileContent>> ReadFileContentAsync(ValidatedFile validatedFile)
    {
        await Task.Delay(100);

        try
        {
            // Simular operaciones que pueden lanzar excepciones del sistema
            if (validatedFile.Path.Contains("missing"))
                throw new FileNotFoundException($"File not found: {validatedFile.Path}");

            if (validatedFile.Path.Contains("locked"))
                throw new UnauthorizedAccessException($"Access denied to file: {validatedFile.Path}");

            if (validatedFile.Path.Contains("network"))
                throw new IOException($"Network error reading file: {validatedFile.Path}");

            // Simular errores de negocio sin excepción
            if (validatedFile.Path.Contains("empty"))
                return MlResult<FileContent>.Fail("File is empty");

            if (validatedFile.Path.Contains("corrupted"))
                return MlResult<FileContent>.Fail("File content is corrupted");

            var content = new FileContent
            {
                Path = validatedFile.Path,
                Data = $"Content of {validatedFile.Path}",
                Size = 1024,
                ReadAt = DateTime.UtcNow,
                Encoding = "UTF-8"
            };

            return MlResult<FileContent>.Valid(content);
        }
        catch (Exception ex)
        {
            // Capturar excepción y almacenarla en el error
            return MlResult<FileContent>.Fail($"Error reading file: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<ProcessedFile>> ProcessFileContentAsync(FileContent fileContent)
    {
        await Task.Delay(200);

        try
        {
            // Simular procesamiento que puede lanzar excepciones
            if (fileContent.Data.Contains("SYSTEM_ERROR"))
                throw new InvalidOperationException("System error during processing");

            if (fileContent.Data.Contains("MEMORY_ERROR"))
                throw new OutOfMemoryException("Insufficient memory for processing");

            // Simular errores de negocio
            if (fileContent.Data.Contains("INVALID_FORMAT"))
                return MlResult<ProcessedFile>.Fail("Invalid file format detected");

            if (fileContent.Data.Contains("BUSINESS_RULE_VIOLATION"))
                return MlResult<ProcessedFile>.Fail("Business rule validation failed");

            var processedFile = new ProcessedFile
            {
                OriginalPath = fileContent.Path,
                ProcessedData = $"Processed: {fileContent.Data}",
                ProcessedAt = DateTime.UtcNow,
                ProcessingTimeMs = 200,
                Status = ProcessingStatus.Completed
            };

            return MlResult<ProcessedFile>.Valid(processedFile);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedFile>.Fail($"Processing failed: {ex.Message}", ex);
        }
    }

    // Manejo de errores técnicos (con excepción)
    private async Task<MlResult<ProcessedFile>> HandleTechnicalErrorAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(150);

        var recoveryStrategy = exception switch
        {
            FileNotFoundException => await AttemptFileRecoveryAsync(request),
            UnauthorizedAccessException => await AttemptPermissionRecoveryAsync(request),
            IOException => await AttemptNetworkRecoveryAsync(request),
            OutOfMemoryException => await AttemptMemoryRecoveryAsync(request),
            _ => await CreateTechnicalErrorReportAsync(request, exception)
        };

        return recoveryStrategy;
    }

    // Manejo de errores de negocio (sin excepción)
    private async Task<MlResult<ProcessedFile>> HandleBusinessErrorAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(120);

        var errorMessage = errorDetails.GetMessage();

        var recoveryStrategy = errorMessage switch
        {
            var msg when msg.Contains("File path is required") => await CreateEmptyFileProcessingAsync(request),
            var msg when msg.Contains("Unsupported file format") => await AttemptFormatConversionAsync(request),
            var msg when msg.Contains("exceeds maximum") => await AttemptFileSplittingAsync(request),
            var msg when msg.Contains("File is empty") => await CreateDefaultContentAsync(request),
            var msg when msg.Contains("corrupted") => await AttemptContentRecoveryAsync(request),
            var msg when msg.Contains("Invalid file format") => await CreateValidationReportAsync(request, errorDetails),
            var msg when msg.Contains("Business rule violation") => await CreateBusinessExceptionReportAsync(request, errorDetails),
            _ => await CreateGenericBusinessErrorReportAsync(request, errorDetails)
        };

        return recoveryStrategy;
    }

    // Métodos de recuperación técnica
    private async Task<MlResult<ProcessedFile>> AttemptFileRecoveryAsync(FileRequest request)
    {
        await Task.Delay(100);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File recovered from backup",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredFromBackup,
            RecoveryMethod = "FileNotFound_BackupRecovery"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptPermissionRecoveryAsync(FileRequest request)
    {
        await Task.Delay(80);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed with elevated permissions",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredWithElevatedPermissions,
            RecoveryMethod = "Permission_ElevatedAccess"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptNetworkRecoveryAsync(FileRequest request)
    {
        await Task.Delay(200);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed via alternative network path",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredViaAlternativeNetwork,
            RecoveryMethod = "Network_AlternativePath"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptMemoryRecoveryAsync(FileRequest request)
    {
        await Task.Delay(250);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File processed in chunks due to memory constraints",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredWithMemoryOptimization,
            RecoveryMethod = "Memory_ChunkedProcessing"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateTechnicalErrorReportAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(100);

        var errorReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Technical error report: {exception.GetType().Name}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.TechnicalErrorReported,
            ErrorDetails = exception.Message,
            RecoveryMethod = "TechnicalError_Report"
        };

        return MlResult<ProcessedFile>.Valid(errorReport);
    }

    // Métodos de recuperación de negocio
    private async Task<MlResult<ProcessedFile>> CreateEmptyFileProcessingAsync(FileRequest request)
    {
        await Task.Delay(50);

        var emptyFile = new ProcessedFile
        {
            OriginalPath = "default_empty_file.txt",
            ProcessedData = "Default empty file content",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedWithDefaults,
            RecoveryMethod = "EmptyPath_DefaultFile"
        };

        return MlResult<ProcessedFile>.Valid(emptyFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptFormatConversionAsync(FileRequest request)
    {
        await Task.Delay(150);

        var convertedFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"File converted from unsupported format",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ConvertedFormat,
            RecoveryMethod = "UnsupportedFormat_Conversion"
        };

        return MlResult<ProcessedFile>.Valid(convertedFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptFileSplittingAsync(FileRequest request)
    {
        await Task.Delay(200);

        var splitFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "File split into smaller chunks for processing",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedInChunks,
            RecoveryMethod = "SizeLimit_FileSplitting"
        };

        return MlResult<ProcessedFile>.Valid(splitFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateDefaultContentAsync(FileRequest request)
    {
        await Task.Delay(80);

        var defaultFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "Default content generated for empty file",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ProcessedWithDefaults,
            RecoveryMethod = "EmptyContent_DefaultGeneration"
        };

        return MlResult<ProcessedFile>.Valid(defaultFile);
    }

    private async Task<MlResult<ProcessedFile>> AttemptContentRecoveryAsync(FileRequest request)
    {
        await Task.Delay(180);

        var recoveredFile = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = "Content recovered using error correction algorithms",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.RecoveredContent,
            RecoveryMethod = "CorruptedContent_ErrorCorrection"
        };

        return MlResult<ProcessedFile>.Valid(recoveredFile);
    }

    private async Task<MlResult<ProcessedFile>> CreateValidationReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(60);

        var validationReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Validation report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.ValidationReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "ValidationError_Report"
        };

        return MlResult<ProcessedFile>.Valid(validationReport);
    }

    private async Task<MlResult<ProcessedFile>> CreateBusinessExceptionReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(70);

        var businessReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Business rule violation report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.BusinessRuleViolationReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "BusinessRule_ViolationReport"
        };

        return MlResult<ProcessedFile>.Valid(businessReport);
    }

    private async Task<MlResult<ProcessedFile>> CreateGenericBusinessErrorReportAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(90);

        var genericReport = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Business error report: {errorDetails.GetMessage()}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.BusinessErrorReported,
            ErrorDetails = errorDetails.GetMessage(),
            RecoveryMethod = "BusinessError_GenericReport"
        };

        return MlResult<ProcessedFile>.Valid(genericReport);
    }

    // Versiones "Safe" para TryBind
    private async Task<MlResult<ProcessedFile>> SafeHandleTechnicalErrorAsync(FileRequest request, Exception exception)
    {
        await Task.Delay(100);

        // Versión segura que puede fallar de forma controlada
        if (exception is OutOfMemoryException && request.EstimatedSizeBytes > 1_000_000)
            throw new InvalidOperationException("Cannot recover from memory error with large files");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Safe recovery from {exception.GetType().Name}",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.SafeRecovery,
            RecoveryMethod = "Safe_TechnicalErrorHandling"
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    private async Task<MlResult<ProcessedFile>> SafeHandleBusinessErrorAsync(FileRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(80);

        // Versión segura que puede fallar de forma controlada
        if (errorDetails.GetMessage().Contains("critical"))
            throw new BusinessException("Cannot safely handle critical business errors");

        var safeRecovery = new ProcessedFile
        {
            OriginalPath = request.FilePath,
            ProcessedData = $"Safe business error recovery",
            ProcessedAt = DateTime.UtcNow,
            Status = ProcessingStatus.SafeRecovery,
            RecoveryMethod = "Safe_BusinessErrorHandling"
        };

        return MlResult<ProcessedFile>.Valid(safeRecovery);
    }

    // Métodos de logging
    private async Task LogTechnicalErrorAsync(string filePath, Exception exception)
    {
        await Task.Delay(10);
        _logger.LogError(exception, "Technical error processing file {FilePath}", filePath);
    }

    private async Task LogBusinessErrorAsync(string filePath, MlErrorsDetails errorDetails)
    {
        await Task.Delay(10);
        _logger.LogWarning("Business error processing file {FilePath}: {Error}", filePath, errorDetails.GetMessage());
    }
}

// Clases de apoyo para procesamiento de archivos
public enum ProcessingStatus
{
    Completed,
    RecoveredFromBackup,
    RecoveredWithElevatedPermissions,
    RecoveredViaAlternativeNetwork,
    RecoveredWithMemoryOptimization,
    TechnicalErrorReported,
    ProcessedWithDefaults,
    ConvertedFormat,
    ProcessedInChunks,
    RecoveredContent,
    ValidationReported,
    BusinessRuleViolationReported,
    BusinessErrorReported,
    SafeRecovery
}

public class FileRequest
{
    public string FilePath { get; set; }
    public long EstimatedSizeBytes { get; set; }
    public long MaxSizeBytes { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class ValidatedFile
{
    public string Path { get; set; }
    public string Format { get; set; }
    public long MaxSizeBytes { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class FileContent
{
    public string Path { get; set; }
    public string Data { get; set; }
    public long Size { get; set; }
    public DateTime ReadAt { get; set; }
    public string Encoding { get; set; }
}

public class ProcessedFile
{
    public string OriginalPath { get; set; }
    public string ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
    public int ProcessingTimeMs { get; set; }
    public ProcessingStatus Status { get; set; }
    public string RecoveryMethod { get; set; }
    public string ErrorDetails { get; set; }
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

public interface IFileValidator { }
public interface IFileProcessor { }
public interface IErrorRecoveryService { }
```

### Ejemplo 2: Sistema de Autenticación y Autorización con Manejo Diferenciado

```csharp
public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ISecurityLogger _securityLogger;

    public async Task<MlResult<AuthenticationResult>> AuthenticateAsync(LoginRequest request)
    {
        return await ValidateLoginRequestAsync(request)
            .BindAsync(validRequest => FindUserAsync(validRequest.Username))
            .BindAsync(user => ValidatePasswordAsync(user, request.Password))
            .BindAsync(authenticatedUser => GenerateTokenAsync(authenticatedUser))
            .BindIfFailWithExceptionAsync(async exception =>
            {
                // Manejo de errores técnicos (BD, red, servicios externos)
                await LogSecurityTechnicalErrorAsync(request.Username, exception);
                return await HandleTechnicalAuthErrorAsync(request, exception);
            })
            .BindIfFailWithoutExceptionAsync(async errorDetails =>
            {
                // Manejo de errores de seguridad/negocio (credenciales, validación)
                await LogSecurityBusinessErrorAsync(request.Username, errorDetails);
                return await HandleSecurityBusinessErrorAsync(request, errorDetails);
            });
    }

    public async Task<MlResult<AuthenticationResult>> AuthenticateSafelyAsync(LoginRequest request)
    {
        return await ValidateLoginRequestAsync(request)
            .BindAsync(validRequest => FindUserAsync(validRequest.Username))
            .TryBindIfFailWithExceptionAsync(
                funcExceptionAsync: async exception => await SafeHandleDatabaseErrorAsync(request, exception),
                errorMessageBuilder: ex => $"Database recovery failed for user {request.Username}: {ex.Message}"
            )
            .BindAsync(user => ValidatePasswordAsync(user, request.Password))
            .TryBindIfFailWithoutExceptionAsync(
                funcAsync: async errorDetails => await SafeHandleCredentialErrorAsync(request, errorDetails),
                errorMessage: "Safe credential error handling failed"
            );
    }

    private async Task<MlResult<LoginRequest>> ValidateLoginRequestAsync(LoginRequest request)
    {
        await Task.Delay(30);

        // Validaciones de negocio sin excepciones
        if (string.IsNullOrEmpty(request.Username))
            return MlResult<LoginRequest>.Fail("Username is required");

        if (string.IsNullOrEmpty(request.Password))
            return MlResult<LoginRequest>.Fail("Password is required");

        if (request.Username.Length < 3)
            return MlResult<LoginRequest>.Fail("Username must be at least 3 characters");

        if (request.Password.Length < 6)
            return MlResult<LoginRequest>.Fail("Password must be at least 6 characters");

        return MlResult<LoginRequest>.Valid(request);
    }

    private async Task<MlResult<User>> FindUserAsync(string username)
    {
        await Task.Delay(100);

        try
        {
            // Simular operaciones de BD que pueden lanzar excepciones
            if (username.Contains("db_error"))
                throw new SqlException("Database connection failed");

            if (username.Contains("timeout"))
                throw new TimeoutException("Database query timeout");

            if (username.Contains("network"))
                throw new NetworkException("Network connectivity issue");

            // Simular errores de negocio sin excepción
            if (username.Contains("notfound"))
                return MlResult<User>.Fail("User not found");

            if (username.Contains("disabled"))
                return MlResult<User>.Fail("User account is disabled");

            if (username.Contains("suspended"))
                return MlResult<User>.Fail("User account is suspended");

            var user = new User
            {
                Id = 1,
                Username = username,
                PasswordHash = "hashed_password_123",
                IsActive = true,
                LastLoginAt = DateTime.UtcNow.AddDays(-1),
                FailedLoginAttempts = 0
            };

            return MlResult<User>.Valid(user);
        }
        catch (Exception ex)
        {
            return MlResult<User>.Fail($"Database error: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<User>> ValidatePasswordAsync(User user, string password)
    {
        await Task.Delay(80);

        try
        {
            // Simular validación que puede lanzar excepciones técnicas
            if (password.Contains("hash_error"))
                throw new CryptographicException("Password hashing service error");

            if (password.Contains("service_down"))
                throw new ServiceUnavailableException("Password validation service unavailable");

            // Simular errores de negocio sin excepción
            if (password != "correct_password")
                return MlResult<User>.Fail("Invalid password");

            if (user.FailedLoginAttempts >= 5)
                return MlResult<User>.Fail("Account locked due to too many failed attempts");

            if (!user.IsActive)
                return MlResult<User>.Fail("User account is not active");

            // Simular validación exitosa
            user.LastLoginAt = DateTime.UtcNow;
            user.FailedLoginAttempts = 0;

            return MlResult<User>.Valid(user);
        }
        catch (Exception ex)
        {
            return MlResult<User>.Fail($"Password validation error: {ex.Message}", ex);
        }
    }

    private async Task<MlResult<AuthenticationResult>> GenerateTokenAsync(User user)
    {
        await Task.Delay(60);

        try
        {
            // Simular generación de token que puede fallar técnicamente
            if (user.Username.Contains("token_error"))
                throw new SecurityTokenException("Token generation failed");

            if (user.Username.Contains("key_error"))
                throw new SecurityTokenSignatureKeyNotFoundException("Signing key not found");

            var token = new AuthenticationResult
            {
                UserId = user.Id,
                Username = user.Username,
                Token = $"token_{Guid.NewGuid()}",
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IssuedAt = DateTime.UtcNow,
                TokenType = "Bearer"
            };

            return MlResult<AuthenticationResult>.Valid(token);
        }
        catch (Exception ex)
        {
            return MlResult<AuthenticationResult>.Fail($"Token generation error: {ex.Message}", ex);
        }
    }

    // Manejo de errores técnicos (con excepción)
    private async Task<MlResult<AuthenticationResult>> HandleTechnicalAuthErrorAsync(LoginRequest request, Exception exception)
    {
        await Task.Delay(100);

        var recoveryResult = exception switch
        {
            SqlException => await AttemptDatabaseRecoveryAsync(request),
            TimeoutException => await AttemptTimeoutRecoveryAsync(request),
            NetworkException => await AttemptNetworkRecoveryAsync(request),
            CryptographicException => await AttemptCryptoRecoveryAsync(request),
            SecurityTokenException => await AttemptTokenRecoveryAsync(request),
            ServiceUnavailableException => await AttemptServiceRecoveryAsync(request),
            _ => await CreateTechnicalErrorResponseAsync(request, exception)
        };

        return recoveryResult;
    }

    // Manejo de errores de negocio (sin excepción)
    private async Task<MlResult<AuthenticationResult>> HandleSecurityBusinessErrorAsync(LoginRequest request, MlErrorsDetails errorDetails)
    {
        await Task.Delay(80);

        var errorMessage = errorDetails.GetMessage();

        var recoveryResult = errorMessage switch
        {
            var msg when msg.Contains("Username is required") => await CreateGuestSessionAsync(),
            var msg when msg.Contains("Password is required") => await CreateAnonymousSessionAsync(),
            var msg when msg.Contains("User not found") => await CreateUserRegistrationPromptAsync(request),
            var msg when msg.Contains("disabled") => await CreateAccountRecoveryPromptAsync(request),
            var msg when msg.Contains("suspended") => await CreateAppealProcessPromptAsync(request),
            var msg when msg.Contains("Invalid password") => await HandleInvalidPasswordAsync(request),
            var msg when msg.Contains("Account locked") => await CreateAccountUnlockPromptAsync(request),
            var msg when msg.Contains("not active") => await CreateActivationPromptAsync(request),
            _ => await CreateGenericSecurityErrorResponseAsync(request, errorDetails)
        };

        return recoveryResult;
    }

    // Métodos de recuperación técnica
    private async Task<MlResult<AuthenticationResult>> AttemptDatabaseRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(150);

        var fallbackAuth = new AuthenticationResult
        {
            UserId = -1,
            Username = request.Username,
            Token = $"fallback_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30), // Token más corto para fallback
            IssuedAt = DateTime.UtcNow,
            TokenType = "Fallback",
            IsFallbackAuthentication = true,
            RecoveryMethod = "DatabaseFallback"
        };

        return MlResult<AuthenticationResult>.Valid(fallbackAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptTimeoutRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(120);

        var quickAuth = new AuthenticationResult
        {
            UserId = -2,
            Username = request.Username,
            Token = $"quick_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Token muy corto
            IssuedAt = DateTime.UtcNow,
            TokenType = "Quick",
            IsQuickAuthentication = true,
            RecoveryMethod = "TimeoutQuickAuth"
        };

        return MlResult<AuthenticationResult>.Valid(quickAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptNetworkRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(200);

        var offlineAuth = new AuthenticationResult
        {
            UserId = -3,
            Username = request.Username,
            Token = $"offline_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Offline",
            IsOfflineAuthentication = true,
            RecoveryMethod = "NetworkOfflineAuth"
        };

        return MlResult<AuthenticationResult>.Valid(offlineAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptCryptoRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(100);

        var basicAuth = new AuthenticationResult
        {
            UserId = -4,
            Username = request.Username,
            Token = $"basic_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Basic",
            IsBasicAuthentication = true,
            RecoveryMethod = "CryptoBasicAuth"
        };

        return MlResult<AuthenticationResult>.Valid(basicAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptTokenRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(80);

        var simpleAuth = new AuthenticationResult
        {
            UserId = -5,
            Username = request.Username,
            Token = $"simple_token_{DateTime.UtcNow.Ticks}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Simple",
            IsSimpleAuthentication = true,
            RecoveryMethod = "TokenSimpleAuth"
        };

        return MlResult<AuthenticationResult>.Valid(simpleAuth);
    }

    private async Task<MlResult<AuthenticationResult>> AttemptServiceRecoveryAsync(LoginRequest request)
    {
        await Task.Delay(120);

        var emergencyAuth = new AuthenticationResult
        {
            UserId = -6,
            Username = request.Username,
            Token = $"emergency_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(45),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Emergency",
            IsEmergencyAuthentication = true,
            RecoveryMethod = "ServiceEmergencyAuth"
        };

        return MlResult<AuthenticationResult>.Valid(emergencyAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateTechnicalErrorResponseAsync(LoginRequest request, Exception exception)
    {
        await Task.Delay(60);

        var errorAuth = new AuthenticationResult
        {
            UserId = -999,
            Username = request.Username,
            Token = "error_token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Error",
            IsErrorResponse = true,
            ErrorMessage = exception.Message,
            RecoveryMethod = "TechnicalErrorResponse"
        };

        return MlResult<AuthenticationResult>.Valid(errorAuth);
    }

    // Métodos de recuperación de negocio
    private async Task<MlResult<AuthenticationResult>> CreateGuestSessionAsync()
    {
        await Task.Delay(40);

        var guestAuth = new AuthenticationResult
        {
            UserId = 0,
            Username = "guest",
            Token = $"guest_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Guest",
            IsGuestSession = true,
            RecoveryMethod = "NoUsername_GuestSession"
        };

        return MlResult<AuthenticationResult>.Valid(guestAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateAnonymousSessionAsync()
    {
        await Task.Delay(30);

        var anonymousAuth = new AuthenticationResult
        {
            UserId = -10,
            Username = "anonymous",
            Token = $"anonymous_token_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            TokenType = "Anonymous",
            IsAnonymousSession = true,
            RecoveryMethod = "NoPassword_AnonymousSession"
        };

        return MlResult<AuthenticationResult>.Valid(anonymousAuth);
    }

    private async Task<MlResult<AuthenticationResult>> CreateUserRegistrationPromptAsync(LoginRequest request)
    {
        await Task.Delay(50);

        var registrationPrompt = new AuthenticationResult
        {
            UserId = -20,
            Username = request.Username,
            Token = $"registration_prompt_{Guid.NewGuid()}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IssuedAt = DateTime.UtcNow,
            TokenType = "RegistrationPrompt",
            RequiresRegistration = true,
            RecoveryMethod = "UserNotFound_RegistrationPrompt"
  …