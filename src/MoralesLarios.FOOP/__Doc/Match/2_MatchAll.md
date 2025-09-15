# MlResult MatchAll - Ejecución Incondicional con Transformación

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos MatchAll Básicos](#métodos-matchall-básicos)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryMatchAll - Captura de Excepciones](#métodos-trymatchall---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Match y ExecSelf](#comparación-con-match-y-execself)

---

## Introducción

Los métodos `MatchAll` proporcionan un patrón de **ejecución incondicional con transformación**, ejecutando una función independientemente del estado del `MlResult<T>` (éxito o fallo) y transformando el resultado a un nuevo tipo `TReturn` envuelto en `MlResult<TReturn>`.

### Propósito Principal

- **Ejecución Independiente del Estado**: La función se ejecuta sin importar si el resultado es válido o fallido
- **Transformación Consistente**: Convierte cualquier entrada en `MlResult<TReturn>`
- **Operaciones de Finalización**: Ideal para cleanup, logging general, o generación de reportes
- **Reset de Contexto**: Crear nuevos resultados independientes del estado anterior

---

## Análisis de los Métodos

### Filosofía de MatchAll

```
MlResult<T> (cualquier estado) → MatchAll(func) → MlResult<TReturn>
                ↓                      ↓                    ↓
     Válido/Fallido → func() → MlResult<TReturn>.Valid(result)
```

### Características Principales

1. **Ejecución Incondicional**: Se ejecuta siempre, sin importar el estado
2. **Ignora Entrada**: No recibe el valor ni los errores del `MlResult<T>` original
3. **Nuevo Contexto**: Genera un `MlResult<TReturn>` completamente nuevo
4. **Reset de Estado**: El resultado anterior no afecta el nuevo resultado
5. **Transformación Total**: Cambia tanto el tipo como el contexto de resultado

---

## Métodos MatchAll Básicos

### `MatchAll<T, TReturn>()`

**Propósito**: Ejecutar una función independientemente del estado del resultado y crear un nuevo `MlResult<TReturn>`

```csharp
public static MlResult<TReturn> MatchAll<T, TReturn>(this MlResult<T> source, 
                                                     Func<TReturn> funcAll)
```

**Comportamiento**:
- Ignora completamente el estado y contenido de `source`
- Ejecuta `funcAll()` incondicionalmente
- Retorna `MlResult<TReturn>.Valid(funcAll())`

**Ejemplo Básico**:
```csharp
var anyResult = ProcessData(data); // Puede ser válido o fallido
var newResult = anyResult.MatchAll(() => "Proceso completado");
// Siempre retorna MlResult<string>.Valid("Proceso completado")
```

**Ejemplo con Generación de Reportes**:
```csharp
var processResult = ExecuteComplexOperation();
var report = processResult.MatchAll(() => new ProcessReport
{
    Timestamp = DateTime.UtcNow,
    ProcessId = Guid.NewGuid(),
    Status = "Completed",
    Message = "Operation finished, check logs for details"
});
// Siempre genera un reporte válido
```

---

## Variantes Asíncronas

### `MatchAllAsync<T, TReturn>()` - Función Asíncrona

```csharp
public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<Task<TReturn>> funcAllAsync)
```

**Ejemplo**:
```csharp
var result = await ProcessDataAsync(data);
var notification = await result.MatchAllAsync(async () => 
    await _notificationService.SendCompletionNotificationAsync());
```

### `MatchAllAsync<T, TReturn>()` - Fuente Asíncrona

```csharp
// Con función asíncrona
public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<Task<TReturn>> funcAllAsync)

// Con función síncrona
public static async Task<MlResult<TReturn>> MatchAllAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<TReturn> funcAll)
```

**Ejemplo**:
```csharp
var summary = await ProcessLongRunningTaskAsync()
    .MatchAllAsync(async () => await GenerateCompletionSummaryAsync());
```

---

## Métodos TryMatchAll - Captura de Excepciones

### `TryMatchAll<T, TReturn>()` - Versión Segura

```csharp
public static MlResult<TReturn> TryMatchAll<T, TReturn>(
    this MlResult<T> source, 
    Func<TReturn> funcAll,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: 
- Ejecuta `funcAll()` incondicionalmente
- Si `funcAll()` lanza excepción, captura y retorna `MlResult<TReturn>` fallido
- Si `funcAll()` ejecuta correctamente, retorna `MlResult<TReturn>` válido

**Ejemplo**:
```csharp
var result = ProcessData(data);
var safeReport = result.TryMatchAll(
    () => GenerateComplexReport(), // Puede lanzar excepción
    ex => $"Report generation failed: {ex.Message}"
);
```

### Versiones Asíncronas de TryMatchAll

```csharp
// Función asíncrona
public static Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<Task<TReturn>> funcAllAsync,
    Func<Exception, string> errorMessageBuilder)

// Con Task<MlResult<T>>
public static async Task<MlResult<TReturn>> TryMatchAllAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<Task<TReturn>> funcAllAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Auditoría y Logging

```csharp
public class AuditService
{
    private readonly ILogger _logger;
    private readonly IAuditRepository _auditRepo;
    private readonly IMetricsCollector _metrics;
    
    public async Task<MlResult<AuditRecord>> ProcessWithAuditAsync<T>(
        Task<MlResult<T>> operationAsync, 
        string operationName, 
        string userId)
    {
        var startTime = DateTime.UtcNow;
        var correlationId = Guid.NewGuid().ToString();
        
        return await operationAsync
            .TryMatchAllAsync(async () =>
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                
                // Crear registro de auditoría independiente del resultado
                var auditRecord = new AuditRecord
                {
                    CorrelationId = correlationId,
                    OperationName = operationName,
                    UserId = userId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Duration = duration,
                    Timestamp = DateTime.UtcNow
                };
                
                // Guardar en base de datos
                await _auditRepo.SaveAuditRecordAsync(auditRecord);
                
                // Registrar métricas
                await _metrics.RecordOperationDurationAsync(operationName, duration);
                await _metrics.IncrementOperationCountAsync(operationName);
                
                // Log general
                await _logger.LogInformationAsync(
                    $"Operation {operationName} completed for user {userId} in {duration.TotalMilliseconds}ms");
                
                return auditRecord;
                
            }, ex => $"Audit processing failed: {ex.Message}");
    }
    
    // Ejemplo de uso
    public async Task<ApiResponse<UserData>> GetUserWithAuditAsync(int userId)
    {
        var userResult = GetUserDataAsync(userId);
        
        var auditResult = await ProcessWithAuditAsync(
            userResult, 
            "GetUserData", 
            userId.ToString());
        
        var originalResult = await userResult;
        
        return originalResult.Match(
            valid: userData => new ApiResponse<UserData>
            {
                Success = true,
                Data = userData,
                AuditId = auditResult.IsValid ? auditResult.Value.CorrelationId : null
            },
            fail: errors => new ApiResponse<UserData>
            {
                Success = false,
                Errors = errors.AllErrors,
                AuditId = auditResult.IsValid ? auditResult.Value.CorrelationId : null
            }
        );
    }
}

public class AuditRecord
{
    public string CorrelationId { get; set; }
    public string OperationName { get; set; }
    public string UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string[] Errors { get; set; }
    public string AuditId { get; set; }
}
```

### Ejemplo 2: Generación de Reportes de Finalización

```csharp
public class ProcessingReportService
{
    private readonly IReportGenerator _reportGenerator;
    private readonly INotificationService _notificationService;
    
    public async Task<MlResult<ProcessingReport>> ProcessBatchWithReportAsync(
        List<BatchItem> items)
    {
        var processingStartTime = DateTime.UtcNow;
        var batchId = Guid.NewGuid().ToString();
        
        return await ProcessBatchItemsAsync(items, batchId)
            .TryMatchAllAsync(async () =>
            {
                var endTime = DateTime.UtcNow;
                var totalDuration = endTime - processingStartTime;
                
                // Generar reporte independientemente del resultado del procesamiento
                var report = new ProcessingReport
                {
                    BatchId = batchId,
                    StartTime = processingStartTime,
                    EndTime = endTime,
                    TotalDuration = totalDuration,
                    TotalItems = items.Count,
                    ProcessedAt = DateTime.UtcNow,
                    Status = "Completed" // Siempre "Completed" porque el proceso terminó
                };
                
                // Analizar resultados individuales para estadísticas
                var itemResults = await GetIndividualResultsAsync(batchId);
                report.SuccessfulItems = itemResults.Count(r => r.Success);
                report.FailedItems = itemResults.Count(r => !r.Success);
                report.SuccessRate = (double)report.SuccessfulItems / report.TotalItems * 100;
                
                // Generar archivo de reporte
                var reportPath = await _reportGenerator.GenerateReportFileAsync(report, itemResults);
                report.ReportPath = reportPath;
                
                // Enviar notificación de finalización
                await _notificationService.SendBatchCompletionNotificationAsync(report);
                
                // Log de finalización
                Console.WriteLine($"Batch {batchId} processing completed. " +
                    $"Success rate: {report.SuccessRate:F1}%. Report saved to: {reportPath}");
                
                return report;
                
            }, ex => $"Report generation failed: {ex.Message}");
    }
    
    public async Task<MlResult<CleanupReport>> ProcessWithCleanupAsync<T>(
        Task<MlResult<T>> operationAsync,
        List<string> tempFiles,
        string workingDirectory)
    {
        return await operationAsync
            .MatchAllAsync(async () =>
            {
                var cleanupStartTime = DateTime.UtcNow;
                var cleanupResults = new List<CleanupItem>();
                
                // Limpiar archivos temporales
                foreach (var tempFile in tempFiles)
                {
                    try
                    {
                        if (File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                            cleanupResults.Add(new CleanupItem
                            {
                                Path = tempFile,
                                Type = "TempFile",
                                Success = true,
                                Message = "Deleted successfully"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        cleanupResults.Add(new CleanupItem
                        {
                            Path = tempFile,
                            Type = "TempFile",
                            Success = false,
                            Message = $"Delete failed: {ex.Message}"
                        });
                    }
                }
                
                // Limpiar directorio de trabajo si está vacío
                try
                {
                    if (Directory.Exists(workingDirectory) && !Directory.EnumerateFileSystemEntries(workingDirectory).Any())
                    {
                        Directory.Delete(workingDirectory);
                        cleanupResults.Add(new CleanupItem
                        {
                            Path = workingDirectory,
                            Type = "WorkingDirectory",
                            Success = true,
                            Message = "Directory removed"
                        });
                    }
                }
                catch (Exception ex)
                {
                    cleanupResults.Add(new CleanupItem
                    {
                        Path = workingDirectory,
                        Type = "WorkingDirectory",
                        Success = false,
                        Message = $"Directory cleanup failed: {ex.Message}"
                    });
                }
                
                var cleanupEndTime = DateTime.UtcNow;
                
                return new CleanupReport
                {
                    StartTime = cleanupStartTime,
                    EndTime = cleanupEndTime,
                    Duration = cleanupEndTime - cleanupStartTime,
                    CleanupItems = cleanupResults.ToArray(),
                    TotalItems = cleanupResults.Count,
                    SuccessfulItems = cleanupResults.Count(r => r.Success),
                    FailedItems = cleanupResults.Count(r => !r.Success)
                };
            });
    }
}

public class ProcessingReport
{
    public string BatchId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public int TotalItems { get; set; }
    public int SuccessfulItems { get; set; }
    public int FailedItems { get; set; }
    public double SuccessRate { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string Status { get; set; }
    public string ReportPath { get; set; }
}

public class CleanupReport
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public CleanupItem[] CleanupItems { get; set; }
    public int TotalItems { get; set; }
    public int SuccessfulItems { get; set; }
    public int FailedItems { get; set; }
}

public class CleanupItem
{
    public string Path { get; set; }
    public string Type { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class BatchItem
{
    public string Id { get; set; }
    public string Data { get; set; }
}
```

### Ejemplo 3: Reset de Contexto y Nuevas Operaciones

```csharp
public class ContextResetService
{
    public async Task<MlResult<SessionData>> ProcessUserActionWithNewSessionAsync<T>(
        Task<MlResult<T>> userActionAsync,
        string userId)
    {
        // Independientemente del resultado de la acción del usuario,
        // siempre crear una nueva sesión
        return await userActionAsync
            .TryMatchAllAsync(async () =>
            {
                // Crear nueva sesión completamente independiente
                var newSession = new SessionData
                {
                    SessionId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow,
                    IsActive = true,
                    Properties = new Dictionary<string, object>()
                };
                
                // Guardar en cache/base de datos
                await SaveSessionAsync(newSession);
                
                // Inicializar configuración de usuario
                var userConfig = await LoadUserConfigurationAsync(userId);
                newSession.Properties["UserConfig"] = userConfig;
                
                // Registrar nuevo inicio de sesión
                await LogSessionStartAsync(newSession);
                
                return newSession;
                
            }, ex => $"New session creation failed: {ex.Message}");
    }
    
    public MlResult<DefaultResponse> HandleAnyResultWithDefault<T>(MlResult<T> anyResult)
    {
        // Convertir cualquier resultado en una respuesta estándar predeterminada
        return anyResult.MatchAll(() => new DefaultResponse
        {
            Id = Guid.NewGuid().ToString(),
            Timestamp = DateTime.UtcNow,
            Status = "Processed",
            Message = "Request has been processed successfully",
            Version = "1.0"
        });
    }
    
    public async Task<MlResult<HealthCheckResult>> PerformHealthCheckAfterOperation<T>(
        Task<MlResult<T>> operationAsync)
    {
        // Realizar health check independientemente del resultado de la operación
        return await operationAsync
            .TryMatchAllAsync(async () =>
            {
                var healthCheck = new HealthCheckResult
                {
                    CheckId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.UtcNow,
                    Checks = new List<ComponentHealth>()
                };
                
                // Verificar base de datos
                var dbHealth = await CheckDatabaseHealthAsync();
                healthCheck.Checks.Add(new ComponentHealth
                {
                    Component = "Database",
                    IsHealthy = dbHealth.IsConnected,
                    ResponseTime = dbHealth.ResponseTime,
                    Message = dbHealth.Message
                });
                
                // Verificar servicios externos
                var externalServicesHealth = await CheckExternalServicesAsync();
                healthCheck.Checks.AddRange(externalServicesHealth);
                
                // Verificar memoria y CPU
                var systemHealth = await CheckSystemResourcesAsync();
                healthCheck.Checks.Add(systemHealth);
                
                // Calcular estado general
                healthCheck.OverallHealth = healthCheck.Checks.All(c => c.IsHealthy) 
                    ? "Healthy" 
                    : "Degraded";
                    
                return healthCheck;
                
            }, ex => $"Health check failed: {ex.Message}");
    }
    
    // Métodos auxiliares
    private async Task SaveSessionAsync(SessionData session) { /* Implementación */ }
    private async Task<UserConfiguration> LoadUserConfigurationAsync(string userId) => new();
    private async Task LogSessionStartAsync(SessionData session) { /* Implementación */ }
    private async Task<DatabaseHealth> CheckDatabaseHealthAsync() => new() { IsConnected = true, ResponseTime = TimeSpan.FromMilliseconds(50), Message = "OK" };
    private async Task<List<ComponentHealth>> CheckExternalServicesAsync() => new();
    private async Task<ComponentHealth> CheckSystemResourcesAsync() => new() { Component = "System", IsHealthy = true, Message = "OK" };
}

public class SessionData
{
    public string SessionId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}

public class DefaultResponse
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public string Version { get; set; }
}

public class HealthCheckResult
{
    public string CheckId { get; set; }
    public DateTime Timestamp { get; set; }
    public List<ComponentHealth> Checks { get; set; }
    public string OverallHealth { get; set; }
}

public class ComponentHealth
{
    public string Component { get; set; }
    public bool IsHealthy { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string Message { get; set; }
}

public class DatabaseHealth
{
    public bool IsConnected { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string Message { get; set; }
}

public class UserConfiguration
{
    public string Theme { get; set; }
    public string Language { get; set; }
    public Dictionary<string, string> Preferences { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar MatchAll

```csharp
// ✅ Correcto: Operaciones de finalización independientes del resultado
var auditResult = ProcessPayment(payment)
    .MatchAll(() => new AuditLog 
    { 
        Timestamp = DateTime.UtcNow, 
        Operation = "Payment" 
    });

// ✅ Correcto: Reset de contexto para nueva operación
var newSession = HandleUserAction(action)
    .MatchAll(() => CreateNewUserSession(userId));

// ✅ Correcto: Generación de reportes que siempre deben crearse
var report = ProcessBatch(items)
    .TryMatchAll(() => GenerateBatchReport(), 
                 ex => $"Report generation failed: {ex.Message}");

// ❌ Incorrecto: Usar cuando necesitas acceso al resultado original
var result = ProcessData(data)
    .MatchAll(() => Transform(data)); // data no está disponible aquí
```

### 2. Diferencia con Match

```csharp
// Match: Diferentes funciones para éxito/fallo
var response = GetUser(id).Match(
    valid: user => $"Welcome {user.Name}",
    fail: errors => $"Error: {errors.FirstErrorMessage}"
);

// MatchAll: Misma función independiente del resultado
var timestamp = GetUser(id).MatchAll(() => DateTime.UtcNow.ToString());
```

### 3. Uso con TryMatchAll para Operaciones Riesgosas

```csharp
// ✅ Correcto: Usar TryMatchAll para operaciones que pueden fallar
var cleanup = ProcessFiles(files)
    .TryMatchAll(() => CleanupTempFiles(), 
                 ex => $"Cleanup failed: {ex.Message}");

// ✅ Correcto: Operaciones de logging seguras
var logResult = ComplexOperation(data)
    .TryMatchAll(() => WriteToAuditLog(operationId), 
                 ex => $"Audit logging failed: {ex.Message}");

// ❌ Incorrecto: No usar Try si la operación es segura
var timestamp = ProcessData(data)
    .TryMatchAll(() => DateTime.UtcNow, ex => "Failed"); // DateTime.UtcNow nunca falla
```

---

## Comparación con Match y ExecSelf

### Tabla Comparativa

| Método | Recibe Entrada | Retorna | Cuándo Ejecuta | Uso Principal |
|--------|----------------|---------|----------------|---------------|
| `Match` | Sí (valor o errores) | `TReturn` | Según estado | Transformación condicional |
| `MatchAll` | No | `MlResult<TReturn>` | Siempre | Operaciones independientes |
| `ExecSelf` | Sí (valor o errores) | `MlResult<T>` original | Según configuración | Efectos secundarios |

### Ejemplo Comparativo

```csharp
var result = ProcessOrder(order);

// Match: Transformación condicional con acceso al contenido
var response = result.Match(
    valid: order => $"Order {order.Id} confirmed",
    fail: errors => $"Order failed: {errors.FirstErrorMessage}"
);

// MatchAll: Operación independiente sin acceso al contenido
var auditEntry = result.MatchAll(() => new AuditEntry 
{ 
    Timestamp = DateTime.UtcNow,
    Operation = "ProcessOrder" 
});

// ExecSelf: Efecto secundario manteniendo resultado original
var sameResult = result.ExecSelf(
    success => LogSuccess(success),
    failure => LogFailure(failure)
); // Retorna el mismo result
```

---

## Resumen

Los métodos `MatchAll` proporcionan **ejecución incondicional con transformación**:

- **`MatchAll`**: Ejecuta función independientemente del estado del resultado
- **`MatchAllAsync`**: Soporte completo para operaciones asíncronas
- **`TryMatchAll`**: Versiones seguras que capturan excepciones

**Casos de uso ideales**:
- **Operaciones de finalización** que siempre deben ejecutarse
- **Generación de reportes** independientes del resultado
- **Cleanup y auditoría** que no dependen del éxito/fallo
- **Reset de contexto** para nuevas operaciones
- **Health checks** posteriores a cualquier operación

**Ventajas principales**:
- **Independencia total** del resultado anterior
- **Reset de contexto** para nuevas operaciones
- **Simplicidad** en operaciones de finalización
- **Consistencia** en la generación de nuevos resultados