# MlResultActionsBindAlways - Operaciones de Ejecución Incondicional

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindAlways Simples](#métodos-bindalways-simples)
4. [Métodos BindAlways Condicionales](#métodos-bindalways-condicionales)
5. [Métodos TryBindAlways - Captura de Excepciones](#métodos-trybindalways---captura-de-excepciones)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBindAlways` contiene las operaciones de **binding incondicional** para `MlResult<T>`. A diferencia de las operaciones `Bind` tradicionales que solo se ejecutan cuando el resultado es exitoso, los métodos `BindAlways` se ejecutan **independientemente del estado** del `MlResult`, lo que los hace ideales para operaciones de limpieza, logging, auditoría y transformaciones que deben ocurrir sin importar el estado del resultado.

### Propósito Principal

- **Ejecución Incondicional**: Las funciones se ejecutan siempre, sin importar el estado del `MlResult`
- **Logging y Auditoría**: Operaciones que deben registrarse independientemente del éxito o fallo
- **Limpieza de Recursos**: Operaciones de cleanup que deben ejecutarse siempre
- **Transformaciones Finales**: Conversiones o procesamientos que no dependen del estado previo
- **Manejo Diferenciado**: Ejecutar diferentes lógicas según el estado (válido/fallido)

---

## Análisis de la Clase

### Estructura y Filosofía

Los métodos `BindAlways` rompen el patrón tradicional de Railway-Oriented Programming donde los errores se propagan automáticamente. En su lugar, implementan un patrón de **ejecución garantizada**:

```
Resultado Exitoso → Función Always → Nuevo Resultado
      ↓                    ↓              ↓
Resultado Fallido  → Función Always → Nuevo Resultado
```

### Características Principales

1. **Ejecución Garantizada**: Las funciones siempre se ejecutan
2. **Dos Variantes**: Simple (misma función) y Condicional (funciones diferentes según estado)
3. **Ignorancia del Estado Original**: El resultado de la función reemplaza completamente al original
4. **Soporte Completo Asíncrono**: Todas las combinaciones de operaciones síncronas y asíncronas

---

## Métodos BindAlways Simples

### `BindAlways<T, TReturn>()`

**Propósito**: Ejecuta una función que devuelve `MlResult<TReturn>` independientemente del estado del resultado origen

```csharp
public static MlResult<TReturn> BindAlways<T, TReturn>(this MlResult<T> source, 
                                                       Func<MlResult<TReturn>> funcAlways)
```

**Parámetros**:
- `source`: El resultado origen (su estado es ignorado)
- `funcAlways`: Función que se ejecuta siempre y devuelve el nuevo resultado

**Comportamiento**:
- Ignora completamente el estado y valor de `source`
- Ejecuta `funcAlways()` y retorna su resultado
- El resultado final depende únicamente de `funcAlways`

**Ejemplo Básico**:
```csharp
var successResult = MlResult<int>.Valid(42);
var failResult = MlResult<int>.Fail("Error original");

// Ambos casos ejecutan la misma función
var finalSuccess = successResult.BindAlways(() => MlResult<string>.Valid("Siempre ejecutado"));
var finalFail = failResult.BindAlways(() => MlResult<string>.Valid("Siempre ejecutado"));

// Ambos finalSuccess y finalFail contienen "Siempre ejecutado"
```

### Versiones Asíncronas del BindAlways Simple

#### `BindAlwaysAsync<T, TReturn>()` - Conversión a Asíncrono
```csharp
public static Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this MlResult<T> source, 
                                                                  Func<MlResult<TReturn>> funcAlways)
```

**Comportamiento**: Ejecuta `BindAlways` y envuelve el resultado en una `Task`

#### `BindAlwaysAsync<T, TReturn>()` - Función Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this MlResult<T> source, 
                                                                        Func<Task<MlResult<TReturn>>> funcAlwaysAsync)
```

**Comportamiento**: Ejecuta `await funcAlwaysAsync()` independientemente del estado de `source`

#### `BindAlwaysAsync<T, TReturn>()` - Fuente Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindAlwaysAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync, 
                                                                        Func<Task<MlResult<TReturn>>> funcAlwaysAsync)
```

**Comportamiento**: Espera `sourceAsync` (pero ignora su resultado) y ejecuta `funcAlwaysAsync`

---

## Métodos BindAlways Condicionales

### `BindAlways<T, TResult>()` - Ejecución Condicional

**Propósito**: Ejecuta diferentes funciones según el estado del `MlResult`, pero siempre ejecuta una de ellas

```csharp
public static MlResult<TResult> BindAlways<T, TResult>(this MlResult<T> source,
                                                       Func<T, MlResult<TResult>> funcValidAlways,
                                                       Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `funcValidAlways`: Función que se ejecuta si `source` es válido (recibe el valor)
- `funcFailAlways`: Función que se ejecuta si `source` es fallido (recibe los errores)

**Comportamiento**:
- Si `source` es válido: Ejecuta `funcValidAlways(value)` 
- Si `source` es fallido: Ejecuta `funcFailAlways(errorDetails)`
- Siempre ejecuta exactamente una de las dos funciones

**Ejemplo Básico**:
```csharp
var validResult = MlResult<int>.Valid(42);
var failedResult = MlResult<int>.Fail("Error original");

var processedValid = validResult.BindAlways(
    validValue => MlResult<string>.Valid($"Procesado valor: {validValue}"),
    errorDetails => MlResult<string>.Valid($"Manejado error: {errorDetails.FirstErrorMessage}")
);
// processedValid contiene "Procesado valor: 42"

var processedFailed = failedResult.BindAlways(
    validValue => MlResult<string>.Valid($"Procesado valor: {validValue}"),
    errorDetails => MlResult<string>.Valid($"Manejado error: {errorDetails.FirstErrorMessage}")
);
// processedFailed contiene "Manejado error: Error original"
```

### Versiones Asíncronas del BindAlways Condicional

#### Todas las Combinaciones de Funciones Asíncronas
```csharp
// Ambas funciones asíncronas
public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(
    this MlResult<T> source,
    Func<T, Task<MlResult<TResult>>> funcValidAlwaysAsync,
    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync)

// Solo función de éxito asíncrona
public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TResult>>> funcValidAlwaysAsync,
    Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)

// Solo función de fallo asíncrona
public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TResult>> funcValidAlways,
    Func<MlErrorsDetails, Task<MlResult<TResult>>> funcFailAlwaysAsync)

// Ambas funciones síncronas desde fuente asíncrona
public static async Task<MlResult<TResult>> BindAlwaysAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TResult>> funcValidAlways,
    Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways)
```

---

## Métodos TryBindAlways - Captura de Excepciones

### TryBindAlways Simple

#### `TryBindAlways<T, TReturn>()` - Versión Segura Simple
```csharp
public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T> source,
                                                          Func<MlResult<TReturn>> funcAlways,
                                                          Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryBindAlways<T, TReturn>(this MlResult<T> source,
                                                          Func<MlResult<TReturn>> funcAlways,
                                                          string errorMessage = null!)
```

**Comportamiento**: 
- Ejecuta `funcAlways` independientemente del estado de `source`
- Si `funcAlways` lanza una excepción, la captura y retorna un `MlResult` fallido
- Convierte excepciones en errores controlados

**Ejemplo**:
```csharp
var result = MlResult<int>.Valid(42);
var safeResult = result.TryBindAlways(
    () => throw new InvalidOperationException("Error simulado"),
    ex => $"Error capturado en always: {ex.Message}"
);
// safeResult será un MlResult fallido con el mensaje personalizado
```

### TryBindAlways Condicional

#### `TryBindAlways<T, TResult>()` - Versión Segura Condicional
```csharp
public static MlResult<TResult> TryBindAlways<T, TResult>(this MlResult<T> source, 
                                                          Func<T, MlResult<TResult>> funcValidAlways,
                                                          Func<MlErrorsDetails, MlResult<TResult>> funcFailAlways,
                                                          Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Ejecuta la función apropiada según el estado de `source`
- Captura excepciones de ambas funciones (`funcValidAlways` y `funcFailAlways`)
- Convierte cualquier excepción en un `MlResult` fallido

**Ejemplo**:
```csharp
var validResult = MlResult<int>.Valid(42);
var safeResult = validResult.TryBindAlways(
    validValue => throw new ArgumentException("Error en función válida"),
    errorDetails => MlResult<string>.Valid("Manejado correctamente"),
    ex => $"Excepción en función válida: {ex.Message}"
);
// safeResult será un MlResult fallido con mensaje de la excepción
```

### Versiones Asíncronas de TryBindAlways

Cada variante de `TryBindAlways` tiene sus correspondientes versiones asíncronas con todas las combinaciones posibles:

- `TryBindAlwaysAsync` para funciones simples
- `TryBindAlwaysAsync` para funciones condicionales
- Soporte para fuentes asíncronas (`Task<MlResult<T>>`)
- Soporte para funciones asíncronas (`Func<Task<MlResult<T>>>`)

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Fuente | Función(es) | Método |
|--------|-------------|---------|
| `MlResult<T>` | `() → MlResult<U>` | `BindAlways` |
| `MlResult<T>` | `() → MlResult<U>` | `BindAlwaysAsync` (conversión) |
| `MlResult<T>` | `() → Task<MlResult<U>>` | `BindAlwaysAsync` |
| `Task<MlResult<T>>` | `() → Task<MlResult<U>>` | `BindAlwaysAsync` |
| `Task<MlResult<T>>` | `() → MlResult<U>` | `BindAlwaysAsync` |

Para BindAlways condicional, cada combinación se multiplica por 4 (2 funciones × 2 posibles estados síncronos/asíncronos).

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Auditoría y Logging

```csharp
public class AuditableUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IMetricsCollector _metricsCollector;
    
    public AuditableUserService(
        IUserRepository userRepository,
        IAuditLogger auditLogger,
        IMetricsCollector metricsCollector)
    {
        _userRepository = userRepository;
        _auditLogger = auditLogger;
        _metricsCollector = metricsCollector;
    }
    
    public async Task<MlResult<User>> GetUserWithAuditAsync(int userId, string requestedBy)
    {
        var startTime = DateTime.UtcNow;
        
        return await _userRepository.GetUserAsync(userId)
            .BindAlwaysAsync(async result => await LogUserAccessAttemptAsync(userId, requestedBy, result))
            .BindAlwaysAsync(async result => await RecordPerformanceMetricsAsync(startTime, "GetUser", result))
            .BindAlways(
                validUser => 
                {
                    _auditLogger.LogSuccess($"User {userId} successfully retrieved by {requestedBy}");
                    return MlResult<User>.Valid(validUser);
                },
                errorDetails => 
                {
                    _auditLogger.LogFailure($"Failed to retrieve user {userId} for {requestedBy}: {errorDetails.FirstErrorMessage}");
                    return MlResult<User>.Fail(errorDetails.AllErrors);
                }
            );
    }
    
    public async Task<MlResult<User>> UpdateUserWithAuditAsync(int userId, UserUpdateRequest updateRequest, string updatedBy)
    {
        var startTime = DateTime.UtcNow;
        var originalUserResult = await _userRepository.GetUserAsync(userId);
        
        return await originalUserResult
            .Bind(originalUser => ValidateUpdateRequest(updateRequest, originalUser))
            .BindAsync(async validatedRequest => await _userRepository.UpdateUserAsync(userId, validatedRequest))
            .BindAlwaysAsync(async updateResult => await LogUpdateAttemptAsync(userId, updateRequest, updatedBy, updateResult))
            .BindAlwaysAsync(async updateResult => await RecordPerformanceMetricsAsync(startTime, "UpdateUser", updateResult))
            .TryBindAlwaysAsync(async updateResult => await NotifyUserChangeAsync(userId, updateResult), 
                               "Notification failed but update was processed")
            .BindAlways(
                updatedUser => 
                {
                    _auditLogger.LogSuccess($"User {userId} successfully updated by {updatedBy}");
                    return MlResult<User>.Valid(updatedUser);
                },
                errorDetails => 
                {
                    _auditLogger.LogFailure($"Failed to update user {userId} by {updatedBy}: {errorDetails.FirstErrorMessage}");
                    return MlResult<User>.Fail(errorDetails.AllErrors);
                }
            );
    }
    
    private async Task<MlResult<MlResult<User>>> LogUserAccessAttemptAsync(int userId, string requestedBy, MlResult<User> result)
    {
        try
        {
            var logEntry = new AuditLogEntry
            {
                UserId = userId,
                Action = "GetUser",
                RequestedBy = requestedBy,
                Timestamp = DateTime.UtcNow,
                Success = result.IsValid,
                ErrorMessage = result.IsValid ? null : result.ErrorsDetails.FirstErrorMessage
            };
            
            await _auditLogger.LogAsync(logEntry);
            return MlResult<MlResult<User>>.Valid(result);
        }
        catch (Exception ex)
        {
            // El logging falló, pero no queremos que esto afecte el resultado principal
            _auditLogger.LogError($"Failed to log user access attempt: {ex.Message}");
            return MlResult<MlResult<User>>.Valid(result);
        }
    }
    
    private async Task<MlResult<MlResult<User>>> RecordPerformanceMetricsAsync(DateTime startTime, string operation, MlResult<User> result)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            var metrics = new PerformanceMetrics
            {
                Operation = operation,
                Duration = duration,
                Success = result.IsValid,
                Timestamp = DateTime.UtcNow
            };
            
            await _metricsCollector.RecordAsync(metrics);
            return MlResult<MlResult<User>>.Valid(result);
        }
        catch (Exception ex)
        {
            // Las métricas fallaron, pero no afectan el resultado principal
            _auditLogger.LogError($"Failed to record performance metrics: {ex.Message}");
            return MlResult<MlResult<User>>.Valid(result);
        }
    }
    
    private async Task<MlResult<MlResult<User>>> LogUpdateAttemptAsync(int userId, UserUpdateRequest request, string updatedBy, MlResult<User> result)
    {
        try
        {
            var logEntry = new AuditLogEntry
            {
                UserId = userId,
                Action = "UpdateUser",
                RequestedBy = updatedBy,
                Timestamp = DateTime.UtcNow,
                Success = result.IsValid,
                Details = JsonSerializer.Serialize(request),
                ErrorMessage = result.IsValid ? null : result.ErrorsDetails.FirstErrorMessage
            };
            
            await _auditLogger.LogAsync(logEntry);
            return MlResult<MlResult<User>>.Valid(result);
        }
        catch (Exception ex)
        {
            _auditLogger.LogError($"Failed to log update attempt: {ex.Message}");
            return MlResult<MlResult<User>>.Valid(result);
        }
    }
    
    private async Task<MlResult<MlResult<User>>> NotifyUserChangeAsync(int userId, MlResult<User> updateResult)
    {
        if (!updateResult.IsValid)
        {
            // No notificar si la actualización falló
            return MlResult<MlResult<User>>.Valid(updateResult);
        }
        
        try
        {
            await _notificationService.NotifyUserUpdatedAsync(userId, updateResult.Value);
            return MlResult<MlResult<User>>.Valid(updateResult);
        }
        catch (Exception ex)
        {
            // La notificación falló, pero la actualización fue exitosa
            _auditLogger.LogWarning($"User {userId} updated successfully but notification failed: {ex.Message}");
            return MlResult<MlResult<User>>.Valid(updateResult);
        }
    }
    
    private MlResult<UserUpdateRequest> ValidateUpdateRequest(UserUpdateRequest request, User originalUser)
    {
        if (request == null)
            return MlResult<UserUpdateRequest>.Fail("Update request cannot be null");
            
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Name))
            return MlResult<UserUpdateRequest>.Fail("At least one field must be updated");
            
        if (!string.IsNullOrWhiteSpace(request.Email) && !IsValidEmail(request.Email))
            return MlResult<UserUpdateRequest>.Fail("Invalid email format");
            
        return MlResult<UserUpdateRequest>.Valid(request);
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

// Clases de apoyo
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserUpdateRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class AuditLogEntry
{
    public int UserId { get; set; }
    public string Action { get; set; }
    public string RequestedBy { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Success { get; set; }
    public string Details { get; set; }
    public string ErrorMessage { get; set; }
}

public class PerformanceMetrics
{
    public string Operation { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
}

// Interfaces
public interface IUserRepository
{
    Task<MlResult<User>> GetUserAsync(int userId);
    Task<MlResult<User>> UpdateUserAsync(int userId, UserUpdateRequest updateRequest);
}

public interface IAuditLogger
{
    Task LogAsync(AuditLogEntry entry);
    void LogSuccess(string message);
    void LogFailure(string message);
    void LogError(string message);
    void LogWarning(string message);
}

public interface IMetricsCollector
{
    Task RecordAsync(PerformanceMetrics metrics);
}

public interface INotificationService
{
    Task NotifyUserUpdatedAsync(int userId, User updatedUser);
}
```

### Ejemplo 2: Sistema de Limpieza de Recursos

```csharp
public class ResourceManagementService
{
    private readonly IFileService _fileService;
    private readonly IDatabaseService _databaseService;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;
    
    public ResourceManagementService(
        IFileService fileService,
        IDatabaseService databaseService,
        ICacheService cacheService,
        ILogger logger)
    {
        _fileService = fileService;
        _databaseService = databaseService;
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<MlResult<ProcessingResult>> ProcessDocumentWithCleanupAsync(DocumentRequest request)
    {
        var tempFiles = new List<string>();
        var databaseTransactions = new List<string>();
        var cacheKeys = new List<string>();
        
        return await ValidateDocumentRequest(request)
            .BindAsync(async validRequest => await CreateTemporaryFilesAsync(validRequest, tempFiles))
            .BindAsync(async tempResult => await ProcessDocumentAsync(tempResult, databaseTransactions, cacheKeys))
            .BindAsync(async processedResult => await FinalizeProcessingAsync(processedResult))
            .BindAlwaysAsync(async finalResult => await CleanupResourcesAsync(tempFiles, databaseTransactions, cacheKeys, finalResult))
            .BindAlways(
                cleanupResult => 
                {
                    _logger.LogInformation($"Document processing completed successfully with cleanup");
                    return cleanupResult;
                },
                errorDetails => 
                {
                    _logger.LogError($"Document processing failed but cleanup was attempted: {errorDetails.FirstErrorMessage}");
                    return MlResult<ProcessingResult>.Fail(errorDetails.AllErrors);
                }
            );
    }
    
    public async Task<MlResult<BackupResult>> CreateBackupWithCleanupAsync(BackupRequest request)
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var lockAcquired = false;
        var connectionOpened = false;
        
        return await ValidateBackupRequest(request)
            .TryBindAsync(async validRequest => await AcquireBackupLockAsync(validRequest), 
                         "Failed to acquire backup lock")
            .DoIf(result => result.IsValid, _ => { lockAcquired = true; return _; })
            .TryBindAsync(async lockResult => await OpenDatabaseConnectionAsync(lockResult),
                         "Failed to open database connection")
            .DoIf(result => result.IsValid, _ => { connectionOpened = true; return _; })
            .TryBindAsync(async dbResult => await CreateTemporaryDirectoryAsync(dbResult, tempDirectory),
                         "Failed to create temporary directory")
            .TryBindAsync(async tempDirResult => await PerformBackupAsync(tempDirResult, tempDirectory),
                         "Backup operation failed")
            .TryBindAsync(async backupResult => await CompressBackupAsync(backupResult, tempDirectory),
                         "Backup compression failed")
            .BindAlwaysAsync(async finalResult => await CleanupBackupResourcesAsync(
                tempDirectory, lockAcquired, connectionOpened, finalResult))
            .BindAlways(
                successfulCleanup => 
                {
                    _logger.LogInformation("Backup completed successfully with full cleanup");
                    return successfulCleanup;
                },
                cleanupErrors => 
                {
                    _logger.LogWarning($"Backup process completed but cleanup had issues: {cleanupErrors.FirstErrorMessage}");
                    return MlResult<BackupResult>.Fail(cleanupErrors.AllErrors);
                }
            );
    }
    
    private async Task<MlResult<ProcessingResult>> CleanupResourcesAsync(
        List<string> tempFiles, 
        List<string> transactions, 
        List<string> cacheKeys, 
        MlResult<ProcessingResult> result)
    {
        var cleanupErrors = new List<string>();
        
        // Limpiar archivos temporales
        foreach (var tempFile in tempFiles)
        {
            try
            {
                if (File.Exists(tempFile))
                {
                    await _fileService.DeleteFileAsync(tempFile);
                    _logger.LogDebug($"Deleted temporary file: {tempFile}");
                }
            }
            catch (Exception ex)
            {
                var error = $"Failed to delete temporary file {tempFile}: {ex.Message}";
                cleanupErrors.Add(error);
                _logger.LogWarning(error);
            }
        }
        
        // Limpiar transacciones de base de datos si el procesamiento falló
        if (!result.IsValid)
        {
            foreach (var transactionId in transactions)
            {
                try
                {
                    await _databaseService.RollbackTransactionAsync(transactionId);
                    _logger.LogDebug($"Rolled back transaction: {transactionId}");
                }
                catch (Exception ex)
                {
                    var error = $"Failed to rollback transaction {transactionId}: {ex.Message}";
                    cleanupErrors.Add(error);
                    _logger.LogWarning(error);
                }
            }
        }
        
        // Limpiar cache
        foreach (var cacheKey in cacheKeys)
        {
            try
            {
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogDebug($"Removed cache key: {cacheKey}");
            }
            catch (Exception ex)
            {
                var error = $"Failed to remove cache key {cacheKey}: {ex.Message}";
                cleanupErrors.Add(error);
                _logger.LogWarning(error);
            }
        }
        
        // Si el resultado original era exitoso pero hubo errores de limpieza
        if (result.IsValid && cleanupErrors.Any())
        {
            _logger.LogWarning($"Processing succeeded but cleanup had {cleanupErrors.Count} errors");
            // Retornar el resultado original exitoso con advertencia
            return result;
        }
        
        // Si el resultado original era fallido
        if (!result.IsValid)
        {
            if (cleanupErrors.Any())
            {
                var allErrors = result.ErrorsDetails.AllErrors.Concat(cleanupErrors).ToArray();
                return MlResult<ProcessingResult>.Fail(allErrors);
            }
            return result;
        }
        
        // Todo exitoso
        return result;
    }
    
    private async Task<MlResult<BackupResult>> CleanupBackupResourcesAsync(
        string tempDirectory, 
        bool lockAcquired, 
        bool connectionOpened, 
        MlResult<BackupResult> result)
    {
        var cleanupErrors = new List<string>();
        
        // Limpiar directorio temporal
        try
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, recursive: true);
                _logger.LogDebug($"Deleted temporary directory: {tempDirectory}");
            }
        }
        catch (Exception ex)
        {
            var error = $"Failed to delete temporary directory {tempDirectory}: {ex.Message}";
            cleanupErrors.Add(error);
            _logger.LogWarning(error);
        }
        
        // Cerrar conexión de base de datos
        if (connectionOpened)
        {
            try
            {
                await _databaseService.CloseConnectionAsync();
                _logger.LogDebug("Database connection closed");
            }
            catch (Exception ex)
            {
                var error = $"Failed to close database connection: {ex.Message}";
                cleanupErrors.Add(error);
                _logger.LogWarning(error);
            }
        }
        
        // Liberar lock
        if (lockAcquired)
        {
            try
            {
                await _databaseService.ReleaseLockAsync();
                _logger.LogDebug("Backup lock released");
            }
            catch (Exception ex)
            {
                var error = $"Failed to release backup lock: {ex.Message}";
                cleanupErrors.Add(error);
                _logger.LogError(error); // Lock es crítico
            }
        }
        
        // Manejar errores de limpieza similar al método anterior
        if (result.IsValid && cleanupErrors.Any())
        {
            _logger.LogWarning($"Backup succeeded but cleanup had {cleanupErrors.Count} errors");
            return result;
        }
        
        if (!result.IsValid)
        {
            if (cleanupErrors.Any())
            {
                var allErrors = result.ErrorsDetails.AllErrors.Concat(cleanupErrors).ToArray();
                return MlResult<BackupResult>.Fail(allErrors);
            }
            return result;
        }
        
        return result;
    }
    
    // Métodos auxiliares de implementación...
    private MlResult<DocumentRequest> ValidateDocumentRequest(DocumentRequest request)
    {
        if (request == null)
            return MlResult<DocumentRequest>.Fail("Document request cannot be null");
        if (string.IsNullOrWhiteSpace(request.FilePath))
            return MlResult<DocumentRequest>.Fail("File path is required");
        if (!File.Exists(request.FilePath))
            return MlResult<DocumentRequest>.Fail($"File not found: {request.FilePath}");
        
        return MlResult<DocumentRequest>.Valid(request);
    }
    
    private async Task<MlResult<TempFileResult>> CreateTemporaryFilesAsync(DocumentRequest request, List<string> tempFiles)
    {
        try
        {
            var tempFile1 = Path.GetTempFileName();
            var tempFile2 = Path.GetTempFileName();
            
            tempFiles.Add(tempFile1);
            tempFiles.Add(tempFile2);
            
            var result = new TempFileResult
            {
                OriginalRequest = request,
                TempFile1 = tempFile1,
                TempFile2 = tempFile2
            };
            
            return MlResult<TempFileResult>.Valid(result);
        }
        catch (Exception ex)
        {
            return MlResult<TempFileResult>.Fail($"Failed to create temporary files: {ex.Message}");
        }
    }
    
    // Más métodos auxiliares...
}

// Clases de apoyo
public class DocumentRequest
{
    public string FilePath { get; set; }
    public string OutputFormat { get; set; }
    public Dictionary<string, object> Options { get; set; }
}

public class ProcessingResult
{
    public string OutputPath { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public int PagesProcessed { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class BackupRequest
{
    public string DatabaseName { get; set; }
    public string OutputPath { get; set; }
    public bool CompressBackup { get; set; }
    public BackupType Type { get; set; }
}

public class BackupResult
{
    public string BackupPath { get; set; }
    public long BackupSize { get; set; }
    public TimeSpan Duration { get; set; }
    public BackupType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TempFileResult
{
    public DocumentRequest OriginalRequest { get; set; }
    public string TempFile1 { get; set; }
    public string TempFile2 { get; set; }
}

public enum BackupType
{
    Full,
    Incremental,
    Differential
}

// Interfaces de servicios
public interface IFileService
{
    Task DeleteFileAsync(string filePath);
}

public interface IDatabaseService
{
    Task RollbackTransactionAsync(string transactionId);
    Task<MlResult<object>> AcquireBackupLockAsync(BackupRequest request);
    Task<MlResult<object>> OpenDatabaseConnectionAsync(object lockResult);
    Task CloseConnectionAsync();
    Task ReleaseLockAsync();
}

public interface ICacheService
{
    Task RemoveAsync(string key);
}

public interface ILogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
}
```

### Ejemplo 3: Transformaciones de Resultado con Logging

```csharp
public class ResultTransformationService
{
    private readonly ILogger _logger;
    private readonly IMetricsService _metricsService;
    
    public ResultTransformationService(ILogger logger, IMetricsService metricsService)
    {
        _logger = logger;
        _metricsService = metricsService;
    }
    
    public async Task<MlResult<ApiResponse<T>>> TransformToApiResponseAsync<T>(
        MlResult<T> serviceResult, 
        string operationName, 
        string correlationId)
    {
        var startTime = DateTime.UtcNow;
        
        return await serviceResult
            .ToAsync()
            .BindAlwaysAsync(async result => await LogOperationResultAsync(operationName, correlationId, result))
            .BindAlwaysAsync(async result => await RecordMetricsAsync(operationName, startTime, result))
            .BindAlways(
                successValue => CreateSuccessApiResponse(successValue, correlationId),
                errorDetails => CreateErrorApiResponse<T>(errorDetails, correlationId)
            );
    }
    
    public MlResult<UserViewModel> TransformUserWithAudit(MlResult<User> userResult, string requestedBy)
    {
        return userResult.BindAlways(
            validUser => 
            {
                _logger.LogInformation($"User {validUser.Id} accessed by {requestedBy}");
                var viewModel = new UserViewModel
                {
                    Id = validUser.Id,
                    Name = validUser.Name,
                    Email = MaskEmail(validUser.Email),
                    LastAccessed = DateTime.UtcNow,
                    AccessedBy = requestedBy
                };
                return MlResult<UserViewModel>.Valid(viewModel);
            },
            errorDetails => 
            {
                _logger.LogWarning($"Failed user access attempt by {requestedBy}: {errorDetails.FirstErrorMessage}");
                var anonymousViewModel = new UserViewModel
                {
                    Id = 0,
                    Name = "Anonymous",
                    Email = "***@***.***",
                    LastAccessed = DateTime.UtcNow,
                    AccessedBy = requestedBy,
                    AccessDenied = true,
                    DenialReason = errorDetails.FirstErrorMessage
                };
                return MlResult<UserViewModel>.Valid(anonymousViewModel);
            }
        );
    }
    
    public MlResult<ResultSummary> SummarizeResults<T>(IEnumerable<MlResult<T>> results, string batchId)
    {
        var resultsList = results.ToList();
        var successCount = 0;
        var failureCount = 0;
        var allErrors = new List<string>();
        
        return resultsList
            .Aggregate(
                MlResult<object>.Valid(new object()),
                (acc, current) => acc.BindAlways(() => 
                {
                    if (current.IsValid)
                    {
                        successCount++;
                        _logger.LogDebug($"Batch {batchId}: Item {successCount + failureCount} succeeded");
                    }
                    else
                    {
                        failureCount++;
                        allErrors.AddRange(current.ErrorsDetails.AllErrors);
                        _logger.LogDebug($"Batch {batchId}: Item {successCount + failureCount} failed: {current.ErrorsDetails.FirstErrorMessage}");
                    }
                    return MlResult<object>.Valid(new object());
                })
            )
            .BindAlways(() => 
            {
                var summary = new ResultSummary
                {
                    BatchId = batchId,
                    TotalItems = resultsList.Count,
                    SuccessCount = successCount,
                    FailureCount = failureCount,
                    SuccessRate = resultsList.Count > 0 ? (double)successCount / resultsList.Count : 0,
                    AllErrors = allErrors,
                    ProcessedAt = DateTime.UtcNow
                };
                
                _logger.LogInformation($"Batch {batchId} summary: {successCount}/{resultsList.Count} succeeded ({summary.SuccessRate:P})");
                return MlResult<ResultSummary>.Valid(summary);
            });
    }
    
    private async Task<MlResult<MlResult<T>>> LogOperationResultAsync<T>(
        string operationName, 
        string correlationId, 
        MlResult<T> result)
    {
        try
        {
            var logMessage = result.IsValid 
                ? $"Operation '{operationName}' succeeded [CorrelationId: {correlationId}]"
                : $"Operation '{operationName}' failed [CorrelationId: {correlationId}]: {result.ErrorsDetails.FirstErrorMessage}";
                
            if (result.IsValid)
                _logger.LogInformation(logMessage);
            else
                _logger.LogWarning(logMessage);
                
            return MlResult<MlResult<T>>.Valid(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to log operation result: {ex.Message}");
            return MlResult<MlResult<T>>.Valid(result);
        }
    }
    
    private async Task<MlResult<MlResult<T>>> RecordMetricsAsync<T>(
        string operationName, 
        DateTime startTime, 
        MlResult<T> result)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            var metrics = new OperationMetrics
            {
                OperationName = operationName,
                Duration = duration,
                Success = result.IsValid,
                Timestamp = DateTime.UtcNow
            };
            
            await _metricsService.RecordAsync(metrics);
            return MlResult<MlResult<T>>.Valid(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to record metrics: {ex.Message}");
            return MlResult<MlResult<T>>.Valid(result);
        }
    }
    
    private MlResult<ApiResponse<T>> CreateSuccessApiResponse<T>(T value, string correlationId)
    {
        var response = new ApiResponse<T>
        {
            Success = true,
            Data = value,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Errors = null
        };
        
        return MlResult<ApiResponse<T>>.Valid(response);
    }
    
    private MlResult<ApiResponse<T>> CreateErrorApiResponse<T>(MlErrorsDetails errorDetails, string correlationId)
    {
        var response = new ApiResponse<T>
        {
            Success = false,
            Data = default(T),
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow,
            Errors = errorDetails.AllErrors
        };
        
        return MlResult<ApiResponse<T>>.Valid(response);
    }
    
    private string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            return "***@***.***";
            
        var parts = email.Split('@');
        var localPart = parts[0].Length > 2 ? $"{parts[0][0]}***{parts[0][^1]}" : "***";
        var domainPart = parts[1].Length > 4 ? $"***{parts[1].Substring(parts[1].Length - 4)}" : "***";
        
        return $"{localPart}@{domainPart}";
    }
}

// Clases de apoyo
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
    public string[] Errors { get; set; }
}

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime LastAccessed { get; set; }
    public string AccessedBy { get; set; }
    public bool AccessDenied { get; set; }
    public string DenialReason { get; set; }
}

public class ResultSummary
{
    public string BatchId { get; set; }
    public int TotalItems { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public double SuccessRate { get; set; }
    public List<string> AllErrors { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class OperationMetrics
{
    public string OperationName { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
}

public interface IMetricsService
{
    Task RecordAsync(OperationMetrics metrics);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar BindAlways vs Bind

```csharp
// ✅ Correcto: Usar BindAlways para operaciones que siempre deben ejecutarse
var result = ProcessData(input)
    .BindAlways(() => LogProcessingAttempt())  // Logging siempre
    .BindAlways(() => CleanupTempFiles())      // Limpieza siempre
    .Bind(data => ValidateResult(data));       // Validación solo si hay datos

// ✅ Correcto: Usar BindAlways condicional para manejar éxito y fallo
var result = CallExternalService()
    .BindAlways(
        success => CreateSuccessNotification(success),
        failure => CreateFailureNotification(failure)
    );

// ❌ Incorrecto: Usar BindAlways cuando se necesita propagación de errores
var result = ValidateInput(input)
    .BindAlways(() => ProcessInput())  // Se ejecutará incluso con input inválido
    .Bind(processed => SaveResult(processed));
```

### 2. Gestión de Recursos con BindAlways

```csharp
// ✅ Correcto: Cleanup garantizado con BindAlways
public async Task<MlResult<ProcessResult>> ProcessWithResourcesAsync(string filePath)
{
    var resources = new DisposableResourceCollection();
    
    return await ValidateFile(filePath)
        .BindAsync(async validPath => await ProcessFileAsync(validPath, resources))
        .BindAlwaysAsync(async result => await CleanupResourcesAsync(resources, result))
        .BindAlways(
            cleanedResult => 
            {
                LogSuccess("Processing completed with cleanup");
                return cleanedResult;
            },
            errorDetails => 
            {
                LogError($"Processing failed but cleanup attempted: {errorDetails.FirstErrorMessage}");
                return MlResult<ProcessResult>.Fail(errorDetails.AllErrors);
            }
        );
}

// ✅ Correcto: Usar TryBindAlways para cleanup que puede fallar
private async Task<MlResult<T>> CleanupResourcesAsync<T>(DisposableResourceCollection resources, MlResult<T> result)
{
    return await result
        .ToAsync()
        .TryBindAlwaysAsync(async () => 
        {
            await resources.DisposeAllAsync();
            return result;
        }, "Resource cleanup failed");
}
```

### 3. Logging y Auditoría

```csharp
// ✅ Correcto: Logging estructurado con BindAlways
public async Task<MlResult<Order>> ProcessOrderAsync(OrderRequest request)
{
    var correlationId = Guid.NewGuid().ToString();
    var startTime = DateTime.UtcNow;
    
    return await ValidateOrder(request)
        .BindAsync(async validOrder => await ProcessPaymentAsync(validOrder))
        .BindAsync(async paidOrder => await CreateOrderAsync(paidOrder))
        .BindAlwaysAsync(async orderResult => await LogOrderProcessingAsync(correlationId, startTime, orderResult))
        .BindAlways(
            successOrder => 
            {
                LogAuditEvent("OrderProcessed", correlationId, true, successOrder.Id);
                return MlResult<Order>.Valid(successOrder);
            },
            errorDetails => 
            {
                LogAuditEvent("OrderProcessingFailed", correlationId, false, errorDetails.FirstErrorMessage);
                return MlResult<Order>.Fail(errorDetails.AllErrors);
            }
        );
}

// ✅ Correcto: Logging que no afecta el resultado principal
private async Task<MlResult<MlResult<Order>>> LogOrderProcessingAsync(
    string correlationId, 
    DateTime startTime, 
    MlResult<Order> result)
{
    try
    {
        var duration = DateTime.UtcNow - startTime;
        var logEntry = new ProcessingLogEntry
        {
            CorrelationId = correlationId,
            Duration = duration,
            Success = result.IsValid,
            Message = result.IsValid ? "Order processed successfully" : result.ErrorsDetails.FirstErrorMessage
        };
        
        await _logger.LogAsync(logEntry);
        return MlResult<MlResult<Order>>.Valid(result);
    }
    catch (Exception ex)
    {
        // Logging falló, pero no afectar el resultado principal
        _fallbackLogger.LogError($"Failed to log order processing: {ex.Message}");
        return MlResult<MlResult<Order>>.Valid(result);
    }
}
```

### 4. Transformaciones de Resultado

```csharp
// ✅ Correcto: Transformar siempre a formato de API
public MlResult<ApiResponse<T>> ToApiResponse<T>(MlResult<T> serviceResult, string correlationId)
{
    return serviceResult.BindAlways(
        successValue => CreateApiSuccessResponse(successValue, correlationId),
        errorDetails => CreateApiErrorResponse<T>(errorDetails, correlationId)
    );
}

// ✅ Correcto: Enriquecimiento de datos independiente del estado
public async Task<MlResult<EnrichedUser>> EnrichUserDataAsync(MlResult<User> userResult)
{
    return await userResult
        .BindAlwaysAsync(async result => await AddTimestampAsync(result))
        .BindAlwaysAsync(async timestampedResult => await AddCorrelationIdAsync(timestampedResult))
        .BindAlways(
            validUser => CreateEnrichedUser(validUser, true),
            errorDetails => CreateEnrichedUser(null, false, errorDetails.FirstErrorMessage)
        );
}

// ❌ Incorrecto: No usar BindAlways para operaciones dependientes del estado
var result = GetUser(id)
    .BindAlways(() => UpdateLastAccess(id))  // Se ejecuta incluso si GetUser falló
    .Bind(user => ProcessUser(user));
```

### 5. Manejo de Excepciones

```csharp
// ✅ Correcto: TryBindAlways para operaciones que pueden fallar
public MlResult<CleanupResult> ProcessWithSafeCleanup<T>(MlResult<T> result)
{
    return result.TryBindAlways(
        () => PerformCleanupOperations(),
        ex => $"Cleanup failed: {ex.Message}"
    );
}

// ✅ Correcto: TryBindAlways condicional con manejo diferenciado
public MlResult<AuditResult> AuditOperationSafely<T>(MlResult<T> operationResult)
{
    return operationResult.TryBindAlways(
        successValue => AuditSuccess(successValue),
        errorDetails => AuditFailure(errorDetails),
        ex => $"Audit operation failed: {ex.GetType().Name} - {ex.Message}"
    );
}

// ❌ Incorrecto: No capturar excepciones en operaciones críticas
var result = ProcessData()
    .BindAlways(() => CriticalCleanup());  // Si CriticalCleanup lanza excepción, se pierde
```

---

## Consideraciones de Rendimiento

### Ejecución Incondicional

- Los métodos `BindAlways` **siempre** ejecutan la función, incluso con resultados fallidos
- No hay optimizaciones de cortocircuito como en `Bind`
- Considerar el costo de ejecución en operaciones costosas

### Captura de Excepciones

- `TryBindAlways` tiene overhead adicional por manejo de excepciones
- Usar solo cuando las funciones pueden lanzar excepciones
- El costo de conversión de excepción a `MlResult` es mínimo

### Operaciones de Limpieza

- Las operaciones de cleanup son típicamente I/O intensivas
- Considerar el uso de `ConfigureAwait(false)` en contextos de librería
- Evaluar si el cleanup debe ser síncrono o asíncrono

---

## Resumen

La clase `MlResultActionsBindAlways` proporciona operaciones de **ejecución incondicional**:

- **`BindAlways` Simple**: Ejecuta la misma función independientemente del estado
- **`BindAlways` Condicional**: Ejecuta diferentes funciones según el estado, pero siempre ejecuta una
- **`BindAlwaysAsync`**: Soporte completo para operaciones asíncronas
- **`TryBindAlways`**: Versiones seguras que capturan excepciones

Estas operaciones son esenciales para:
- **Logging y auditoría** que debe ocurrir siempre
- **Limpieza de recursos** que debe ejecutarse independientemente del estado
- **Transformaciones finales** que no dependen del éxito de operaciones previas
- **Manejo diferenciado** de resultados válidos y fallidos

La clave está en usar `BindAlways` cuando necesites **garantizar la ejecución** de una operación, y `Bind` cuando necesites **propagar errores** automáticamente.