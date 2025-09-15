# MlResultActionsMapAlways - Operaciones de Mapeo Incondicional

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos MapAlways Simples](#métodos-mapalways-simples)
4. [Métodos MapAlways Duales](#métodos-mapalways-duales)
5. [Métodos TryMapAlways](#métodos-trymapalways)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

Los métodos **`MapAlways`** proporcionan capacidades para **transformación incondicional** de resultados, ejecutándose independientemente del estado del `MlResult`. Estos métodos garantizan que siempre se produce una transformación, ya sea ignorando completamente el estado del resultado original o aplicando funciones específicas según el estado.

### Propósito Principal

- **Transformación Garantizada**: Asegurar que siempre se produce un resultado transformado
- **Mapeo Condicional**: Aplicar diferentes transformaciones según el estado del resultado
- **Finalización de Pipelines**: Convertir cualquier resultado en un formato específico
- **Logging y Auditoría**: Capturar información independientemente del estado del resultado

### Diferencia Clave con Otros Map

- **`Map`**: Solo transforma valores válidos
- **`MapIfFail`**: Solo transforma valores fallidos
- **`MapAlways`**: **SIEMPRE** ejecuta transformación, ignorando el estado original

---

## Análisis de la Clase

### Estructura y Filosofía

Esta clase implementa dos patrones principales:

1. **Mapeo Incondicional Simple**: `MapAlways<T, TReturn>(Func<TReturn>)`
2. **Mapeo Condicional Dual**: `MapAlways<T, TReturn>(Func<T, TReturn>, Func<MlErrorsDetails, TReturn>)`

### Características Principales

1. **Ejecución Garantizada**: Las funciones siempre se ejecutan
2. **Ignorancia del Estado Original**: Las versiones simples no consideran el estado del resultado fuente
3. **Transformación Dual**: Las versiones duales aplican diferentes funciones según el estado
4. **Resultado Siempre Válido**: El resultado final siempre es un `MlResult<TReturn>` válido

---

## Métodos MapAlways Simples

### `MapAlways<T, TReturn>()`

**Propósito**: Ejecuta una función de transformación independientemente del estado del resultado fuente, ignorando completamente el valor y estado originales

```csharp
public static MlResult<TReturn> MapAlways<T, TReturn>(
    this MlResult<T> source,
    Func<TReturn> funcAlways)
```

**Parámetros**:
- `source`: El resultado fuente (su valor y estado son ignorados)
- `funcAlways`: Función que genera el nuevo valor

**Comportamiento**:
- **SIEMPRE** ejecuta `funcAlways()`
- **SIEMPRE** retorna un `MlResult<TReturn>` válido
- **IGNORA** completamente el estado y valor de `source`

**Ejemplo Básico**:
```csharp
var timestamp = GetUserData(userId)
    .MapAlways(() => DateTime.UtcNow);

// Sin importar si GetUserData fue exitoso o falló,
// timestamp será un MlResult<DateTime> válido con la fecha actual
```

**Casos de Uso Típicos**:
```csharp
// Generar ID de correlación
var correlationId = ProcessData(input)
    .MapAlways(() => Guid.NewGuid().ToString());

// Obtener configuración por defecto
var config = LoadUserConfig(userId)
    .MapAlways(() => GetDefaultConfiguration());

// Timestamp de finalización
var finishTime = ExecuteLongOperation(request)
    .MapAlways(() => DateTime.UtcNow);
```

### Versiones Asíncronas Simples

#### `MapAlwaysAsync` - Función Asíncrona
```csharp
public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(
    this MlResult<T> source,
    Func<Task<TReturn>> funcAlwaysAsync)

public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<Task<TReturn>> funcAlwaysAsync)

public static async Task<MlResult<TReturn>> MapAlwaysAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync,
    Func<TReturn> funcAlways)
```

**Ejemplo Asíncrono**:
```csharp
var auditLog = await ProcessPaymentAsync(payment)
    .MapAlwaysAsync(async () => 
    {
        await _auditService.LogOperationCompletedAsync();
        return new AuditResult { Timestamp = DateTime.UtcNow };
    });

// Se audita independientemente del resultado del pago
```

---

## Métodos MapAlways Duales

### `MapAlways<T, TReturn>()` - Versión Dual

**Propósito**: Ejecuta diferentes funciones según el estado del resultado fuente, pero siempre ejecuta una de las dos funciones

```csharp
public static MlResult<TReturn> MapAlways<T, TReturn>(
    this MlResult<T> source,
    Func<T, TReturn> funcValidAlways,
    Func<MlErrorsDetails, TReturn> funcFailAlways)
```

**Parámetros**:
- `source`: El resultado fuente a evaluar
- `funcValidAlways`: Función a ejecutar si `source` es válido (recibe el valor)
- `funcFailAlways`: Función a ejecutar si `source` es fallido (recibe los errores)

**Comportamiento**:
- Si `source` es válido: Ejecuta `funcValidAlways(value)` y retorna resultado válido
- Si `source` es fallido: Ejecuta `funcFailAlways(errors)` y retorna resultado válido
- **SIEMPRE** retorna un resultado válido
- **NUNCA** preserva errores del resultado original

**Ejemplo Básico**:
```csharp
var report = ProcessUserData(userData)
    .MapAlways(
        validData => new ProcessingReport 
        { 
            Status = "Success", 
            Data = validData,
            ProcessedAt = DateTime.UtcNow 
        },
        errors => new ProcessingReport 
        { 
            Status = "Failed", 
            ErrorMessage = errors.FirstErrorMessage,
            ProcessedAt = DateTime.UtcNow 
        }
    );

// 'report' SIEMPRE será un MlResult<ProcessingReport> válido
// El estado original se convierte en información dentro del ProcessingReport
```

### Versiones Asíncronas Duales

#### Todas las Combinaciones Async
```csharp
// Ambas funciones asíncronas
public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(
    this MlResult<T> source,
    Func<T, Task<TResult>> funcValidAlwaysAsync,
    Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync)

// Fuente asíncrona con funciones asíncronas
public static async Task<MlResult<TResult>> MapAlwaysAsync<T, TResult>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task<TResult>> funcValidAlwaysAsync,
    Func<MlErrorsDetails, Task<TResult>> funcFailAlwaysAsync)

// Combinaciones mixtas de sync/async
// ... (múltiples sobrecargas)
```

---

## Métodos TryMapAlways

### `TryMapAlways<T, TReturn>()` - Versión Segura Simple

**Propósito**: Versión segura del mapeo incondicional que captura excepciones

```csharp
public static MlResult<TReturn> TryMapAlways<T, TReturn>(
    this MlResult<T> source,
    Func<TReturn> funcAlways,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryMapAlways<T, TReturn>(
    this MlResult<T> source,
    Func<TReturn> funcAlways,
    string errorMessage = null!)
```

**Parámetros**:
- `source`: El resultado fuente (ignorado excepto para propagación de errores originales)
- `funcAlways`: Función que puede lanzar excepciones
- `errorMessageBuilder`: Constructor de mensaje para excepciones capturadas
- `errorMessage`: Mensaje fijo para excepciones

**Comportamiento**:
- Si `funcAlways` se ejecuta sin excepción: Retorna resultado válido
- Si `funcAlways` lanza excepción: Captura la excepción y retorna resultado fallido
- Si `source` original era fallido: La información del error original se preserva en el contexto

**Ejemplo con Manejo de Excepciones**:
```csharp
var config = LoadConfiguration(configPath)
    .TryMapAlways(
        () => ParseComplexConfiguration(), // Puede lanzar excepción
        ex => $"Failed to parse configuration: {ex.Message}"
    );

// Si ParseComplexConfiguration() falla, se convierte en MlResult fallido
```

### `TryMapAlways<T, TReturn>()` - Versión Segura Dual

```csharp
public static MlResult<TResult> TryMapAlways<T, TResult>(
    this MlResult<T> source,
    Func<T, TResult> funcValidAlways,
    Func<MlErrorsDetails, TResult> funcFailAlways,
    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: 
- Captura excepciones en ambas funciones (`funcValidAlways` y `funcFailAlways`)
- Si cualquiera de las funciones lanza excepción, se convierte en resultado fallido
- Preserva información del resultado original en el contexto de error

### Versiones Asíncronas de TryMapAlways

```csharp
public static Task<MlResult<TReturn>> TryMapAlwaysAsync<T, TReturn>(
    this MlResult<T> source,
    Func<Task<TReturn>> funcAlwaysAsync,
    Func<Exception, string> errorMessageBuilder)

// Múltiples sobrecargas para diferentes combinaciones
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Tipo | Fuente | Función(es) | Método |
|------|--------|-------------|---------|
| **Simple** | `MlResult<T>` | `() → U` | `MapAlways` |
| **Simple** | `MlResult<T>` | `() → Task<U>` | `MapAlwaysAsync` |
| **Simple** | `Task<MlResult<T>>` | `() → Task<U>` | `MapAlwaysAsync` |
| **Simple** | `Task<MlResult<T>>` | `() → U` | `MapAlwaysAsync` |
| **Dual** | `MlResult<T>` | `T → U`, `Errors → U` | `MapAlways` |
| **Dual** | `MlResult<T>` | `T → Task<U>`, `Errors → Task<U>` | `MapAlwaysAsync` |
| **Dual** | `Task<MlResult<T>>` | `T → Task<U>`, `Errors → Task<U>` | `MapAlwaysAsync` |

Todas las variantes tienen sus correspondientes versiones `Try*` para manejo seguro de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Auditoría Garantizada

```csharp
public class AuditedOperationService
{
    private readonly IOperationService _operationService;
    private readonly IAuditService _auditService;
    private readonly ILogger _logger;
    
    public AuditedOperationService(
        IOperationService operationService,
        IAuditService auditService,
        ILogger logger)
    {
        _operationService = operationService;
        _auditService = auditService;
        _logger = logger;
    }
    
    public async Task<MlResult<OperationAuditReport>> ExecuteWithAuditAsync(OperationRequest request)
    {
        var operationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await _operationService.ExecuteOperationAsync(request)
            .TryMapAlwaysAsync(
                async successResult => 
                {
                    var endTime = DateTime.UtcNow;
                    var duration = endTime - startTime;
                    
                    var auditReport = new OperationAuditReport
                    {
                        OperationId = operationId,
                        Request = request,
                        Result = successResult,
                        Status = OperationStatus.Success,
                        StartTime = startTime,
                        EndTime = endTime,
                        Duration = duration,
                        ErrorDetails = null
                    };
                    
                    await _auditService.LogSuccessfulOperationAsync(auditReport);
                    await _logger.LogInformationAsync($"Operation {operationId} completed successfully in {duration.TotalMilliseconds}ms");
                    
                    return auditReport;
                },
                async errors => 
                {
                    var endTime = DateTime.UtcNow;
                    var duration = endTime - startTime;
                    
                    var auditReport = new OperationAuditReport
                    {
                        OperationId = operationId,
                        Request = request,
                        Result = null,
                        Status = OperationStatus.Failed,
                        StartTime = startTime,
                        EndTime = endTime,
                        Duration = duration,
                        ErrorDetails = errors.AllErrors
                    };
                    
                    await _auditService.LogFailedOperationAsync(auditReport);
                    await _logger.LogErrorAsync($"Operation {operationId} failed after {duration.TotalMilliseconds}ms: {errors.FirstErrorMessage}");
                    
                    return auditReport;
                },
                ex => $"Audit logging failed for operation {operationId}: {ex.Message}"
            );
    }
    
    public async Task<MlResult<ProcessingMetrics>> ProcessBatchWithMetricsAsync(List<DataItem> items)
    {
        var batchId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        var processedCount = 0;
        var errors = new List<string>();
        
        return await ProcessBatchItemsAsync(items)
            .MapAlwaysAsync(async batchResults => 
            {
                // Esta función SIEMPRE se ejecuta, independientemente del resultado del batch
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;
                
                foreach (var result in batchResults)
                {
                    if (result.IsValid)
                        processedCount++;
                    else
                        errors.AddRange(result.ErrorDetails.AllErrors);
                }
                
                var metrics = new ProcessingMetrics
                {
                    BatchId = batchId,
                    TotalItems = items.Count,
                    ProcessedSuccessfully = processedCount,
                    FailedItems = items.Count - processedCount,
                    TotalDuration = duration,
                    AverageTimePerItem = TimeSpan.FromMilliseconds(duration.TotalMilliseconds / items.Count),
                    ErrorSummary = errors.Take(10).ToList(), // Solo los primeros 10 errores
                    ProcessedAt = endTime
                };
                
                await _auditService.LogBatchMetricsAsync(metrics);
                await UpdatePerformanceCountersAsync(metrics);
                
                return metrics;
            });
    }
    
    public async Task<MlResult<string>> GenerateReportIdAsync()
    {
        // Ejemplo de MapAlways simple: siempre generar un ID único
        return await SomeOperationAsync()
            .MapAlwaysAsync(async () => 
            {
                var reportId = $"RPT_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}";
                await _auditService.LogReportIdGeneratedAsync(reportId);
                return reportId;
            });
    }
    
    private async Task<List<MlResult<ProcessedItem>>> ProcessBatchItemsAsync(List<DataItem> items)
    {
        var tasks = items.Select(async item => 
        {
            try
            {
                var processed = await _operationService.ProcessItemAsync(item);
                return MlResult<ProcessedItem>.Valid(processed);
            }
            catch (Exception ex)
            {
                return MlResult<ProcessedItem>.FailWithException($"Failed to process item {item.Id}", ex);
            }
        });
        
        return (await Task.WhenAll(tasks)).ToList();
    }
    
    private async Task UpdatePerformanceCountersAsync(ProcessingMetrics metrics)
    {
        try
        {
            await _auditService.UpdatePerformanceCountersAsync(
                metrics.TotalItems,
                metrics.ProcessedSuccessfully,
                metrics.TotalDuration
            );
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to update performance counters: {ex.Message}");
        }
    }
}

// Clases de apoyo
public class OperationRequest
{
    public string RequestId { get; set; }
    public string OperationType { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public string UserId { get; set; }
}

public class OperationAuditReport
{
    public string OperationId { get; set; }
    public OperationRequest Request { get; set; }
    public object Result { get; set; }
    public OperationStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string[] ErrorDetails { get; set; }
}

public class ProcessingMetrics
{
    public string BatchId { get; set; }
    public int TotalItems { get; set; }
    public int ProcessedSuccessfully { get; set; }
    public int FailedItems { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan AverageTimePerItem { get; set; }
    public List<string> ErrorSummary { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class DataItem
{
    public string Id { get; set; }
    public object Data { get; set; }
}

public class ProcessedItem
{
    public string Id { get; set; }
    public object ProcessedData { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public enum OperationStatus
{
    Success,
    Failed,
    PartialSuccess
}

// Interfaces de servicios
public interface IOperationService
{
    Task<object> ExecuteOperationAsync(OperationRequest request);
    Task<ProcessedItem> ProcessItemAsync(DataItem item);
}

public interface IAuditService
{
    Task LogSuccessfulOperationAsync(OperationAuditReport report);
    Task LogFailedOperationAsync(OperationAuditReport report);
    Task LogBatchMetricsAsync(ProcessingMetrics metrics);
    Task LogReportIdGeneratedAsync(string reportId);
    Task UpdatePerformanceCountersAsync(int totalItems, int successfulItems, TimeSpan duration);
}
```

### Ejemplo 2: Sistema de Reportes Garantizados

```csharp
public class ReportingService
{
    private readonly IDataService _dataService;
    private readonly IReportGenerator _reportGenerator;
    private readonly IReportStorage _reportStorage;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;
    
    public ReportingService(
        IDataService dataService,
        IReportGenerator reportGenerator,
        IReportStorage reportStorage,
        INotificationService notificationService,
        ILogger logger)
    {
        _dataService = dataService;
        _reportGenerator = reportGenerator;
        _reportStorage = reportStorage;
        _notificationService = notificationService;
        _logger = logger;
    }
    
    public async Task<MlResult<ReportResult>> GenerateReportWithGuaranteedNotificationAsync(ReportRequest request)
    {
        var reportId = Guid.NewGuid().ToString();
        
        return await LoadReportDataAsync(request)
            .BindAsync(async data => await GenerateReportAsync(data, reportId))
            .BindAsync(async report => await SaveReportAsync(report))
            .TryMapAlwaysAsync(
                async successReport => 
                {
                    // Caso exitoso: notificar éxito
                    var result = new ReportResult
                    {
                        ReportId = reportId,
                        Status = ReportStatus.Completed,
                        ReportUrl = successReport.Url,
                        GeneratedAt = DateTime.UtcNow,
                        ErrorMessage = null
                    };
                    
                    await _notificationService.NotifyReportCompletedAsync(request.RequesterId, result);
                    await _logger.LogInformationAsync($"Report {reportId} generated successfully");
                    
                    return result;
                },
                async errors => 
                {
                    // Caso fallido: notificar fallo pero siempre crear un resultado
                    var result = new ReportResult
                    {
                        ReportId = reportId,
                        Status = ReportStatus.Failed,
                        ReportUrl = null,
                        GeneratedAt = DateTime.UtcNow,
                        ErrorMessage = errors.FirstErrorMessage
                    };
                    
                    await _notificationService.NotifyReportFailedAsync(request.RequesterId, result);
                    await _logger.LogErrorAsync($"Report {reportId} generation failed: {errors.FirstErrorMessage}");
                    
                    return result;
                },
                ex => $"Failed to process report notification for {reportId}: {ex.Message}"
            );
    }
    
    public async Task<MlResult<AnalyticsReport>> GenerateAnalyticsWithFallbackAsync(AnalyticsRequest request)
    {
        return await GenerateDetailedAnalyticsAsync(request)
            .TryMapAlwaysAsync(
                // Caso exitoso: usar analytics detallado
                validAnalytics => validAnalytics,
                // Caso fallido: generar analytics básico
                async errors => 
                {
                    _logger.LogWarning($"Detailed analytics failed, generating basic report: {errors.FirstErrorMessage}");
                    
                    var basicAnalytics = await GenerateBasicAnalyticsAsync(request);
                    basicAnalytics.IsFallback = true;
                    basicAnalytics.FallbackReason = "Detailed analytics generation failed";
                    
                    return basicAnalytics;
                },
                ex => $"Failed to generate fallback analytics: {ex.Message}"
            );
    }
    
    public async Task<MlResult<ExportResult>> ExportDataWithGuaranteedResponseAsync(ExportRequest request)
    {
        var exportId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await PrepareExportDataAsync(request)
            .BindAsync(async data => await ExportToFormatAsync(data, request.Format))
            .BindAsync(async exportedData => await UploadExportAsync(exportedData, exportId))
            .MapAlwaysAsync(
                // SIEMPRE retorna un ExportResult, sin importar el estado anterior
                async () => 
                {
                    var endTime = DateTime.UtcNow;
                    var duration = endTime - startTime;
                    
                    // Obtener estado de la exportación
                    var exportStatus = await GetExportStatusAsync(exportId);
                    
                    var result = new ExportResult
                    {
                        ExportId = exportId,
                        RequestedBy = request.UserId,
                        RequestedAt = startTime,
                        CompletedAt = endTime,
                        Duration = duration,
                        Status = exportStatus.IsCompleted ? ExportStatus.Completed : ExportStatus.Failed,
                        DownloadUrl = exportStatus.DownloadUrl,
                        ErrorMessage = exportStatus.ErrorMessage,
                        EstimatedSize = exportStatus.EstimatedSize
                    };
                    
                    // Registrar métricas independientemente del resultado
                    await RecordExportMetricsAsync(result);
                    
                    return result;
                }
            );
    }
    
    public async Task<MlResult<ProcessingSummary>> ProcessDocumentsWithSummaryAsync(List<DocumentRequest> documents)
    {
        var batchId = Guid.NewGuid().ToString();
        
        return await ProcessDocumentBatchAsync(documents, batchId)
            .TryMapAlwaysAsync(
                // SIEMPRE generar un resumen, independientemente del resultado del procesamiento
                () => GenerateProcessingSummaryAsync(batchId, documents.Count),
                ex => $"Failed to generate processing summary for batch {batchId}: {ex.Message}"
            );
    }
    
    private async Task<MlResult<ReportData>> LoadReportDataAsync(ReportRequest request)
    {
        try
        {
            var data = await _dataService.LoadDataForReportAsync(request);
            return MlResult<ReportData>.Valid(data);
        }
        catch (Exception ex)
        {
            return MlResult<ReportData>.FailWithException("Failed to load report data", ex);
        }
    }
    
    private async Task<MlResult<GeneratedReport>> GenerateReportAsync(ReportData data, string reportId)
    {
        try
        {
            var report = await _reportGenerator.GenerateAsync(data, reportId);
            return MlResult<GeneratedReport>.Valid(report);
        }
        catch (Exception ex)
        {
            return MlResult<GeneratedReport>.FailWithException($"Failed to generate report {reportId}", ex);
        }
    }
    
    private async Task<MlResult<SavedReport>> SaveReportAsync(GeneratedReport report)
    {
        try
        {
            var savedReport = await _reportStorage.SaveAsync(report);
            return MlResult<SavedReport>.Valid(savedReport);
        }
        catch (Exception ex)
        {
            return MlResult<SavedReport>.FailWithException($"Failed to save report {report.Id}", ex);
        }
    }
    
    private async Task<MlResult<AnalyticsReport>> GenerateDetailedAnalyticsAsync(AnalyticsRequest request)
    {
        try
        {
            var analytics = await _dataService.GenerateDetailedAnalyticsAsync(request);
            return MlResult<AnalyticsReport>.Valid(analytics);
        }
        catch (Exception ex)
        {
            return MlResult<AnalyticsReport>.FailWithException("Detailed analytics generation failed", ex);
        }
    }
    
    private async Task<AnalyticsReport> GenerateBasicAnalyticsAsync(AnalyticsRequest request)
    {
        // Generar analytics básico con datos mínimos
        var basicData = await _dataService.GetBasicStatsAsync(request.DateRange);
        
        return new AnalyticsReport
        {
            Id = Guid.NewGuid().ToString(),
            Period = request.DateRange,
            BasicStats = basicData,
            IsDetailed = false,
            GeneratedAt = DateTime.UtcNow
        };
    }
    
    private async Task<ExportStatus> GetExportStatusAsync(string exportId)
    {
        try
        {
            return await _reportStorage.GetExportStatusAsync(exportId);
        }
        catch
        {
            return new ExportStatus
            {
                IsCompleted = false,
                ErrorMessage = "Could not retrieve export status"
            };
        }
    }
    
    private async Task<ProcessingSummary> GenerateProcessingSummaryAsync(string batchId, int totalDocuments)
    {
        var processingResults = await _dataService.GetBatchProcessingResultsAsync(batchId);
        
        return new ProcessingSummary
        {
            BatchId = batchId,
            TotalDocuments = totalDocuments,
            ProcessedSuccessfully = processingResults.SuccessCount,
            FailedDocuments = processingResults.FailureCount,
            ProcessingErrors = processingResults.Errors,
            CompletedAt = DateTime.UtcNow
        };
    }
    
    private async Task RecordExportMetricsAsync(ExportResult result)
    {
        try
        {
            await _dataService.RecordExportMetricsAsync(new ExportMetrics
            {
                ExportId = result.ExportId,
                Duration = result.Duration,
                Status = result.Status,
                EstimatedSize = result.EstimatedSize,
                Timestamp = result.CompletedAt
            });
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to record export metrics: {ex.Message}");
        }
    }
    
    private async Task<MlResult<object>> PrepareExportDataAsync(ExportRequest request)
    {
        // Implementación de preparación de datos para exportación
        try
        {
            var data = await _dataService.PrepareExportDataAsync(request);
            return MlResult<object>.Valid(data);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException("Export data preparation failed", ex);
        }
    }
    
    private async Task<MlResult<object>> ExportToFormatAsync(object data, string format)
    {
        // Implementación de exportación a formato específico
        try
        {
            var exportedData = await _reportGenerator.ExportToFormatAsync(data, format);
            return MlResult<object>.Valid(exportedData);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException($"Export to {format} failed", ex);
        }
    }
    
    private async Task<MlResult<object>> UploadExportAsync(object exportedData, string exportId)
    {
        // Implementación de subida del archivo exportado
        try
        {
            await _reportStorage.UploadExportAsync(exportedData, exportId);
            return MlResult<object>.Valid(exportedData);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException($"Upload of export {exportId} failed", ex);
        }
    }
    
    private async Task<MlResult<object>> ProcessDocumentBatchAsync(List<DocumentRequest> documents, string batchId)
    {
        // Implementación de procesamiento de lote de documentos
        try
        {
            var results = await _dataService.ProcessDocumentBatchAsync(documents, batchId);
            return MlResult<object>.Valid(results);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException($"Document batch processing failed for {batchId}", ex);
        }
    }
}

// Clases de apoyo adicionales
public class ReportRequest
{
    public string RequesterId { get; set; }
    public string ReportType { get; set; }
    public DateRange DateRange { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}

public class ReportResult
{
    public string ReportId { get; set; }
    public ReportStatus Status { get; set; }
    public string ReportUrl { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string ErrorMessage { get; set; }
}

public class AnalyticsRequest
{
    public DateRange DateRange { get; set; }
    public List<string> Metrics { get; set; }
    public string UserId { get; set; }
}

public class AnalyticsReport
{
    public string Id { get; set; }
    public DateRange Period { get; set; }
    public object BasicStats { get; set; }
    public object DetailedAnalytics { get; set; }
    public bool IsDetailed { get; set; }
    public bool IsFallback { get; set; }
    public string FallbackReason { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class ExportRequest
{
    public string UserId { get; set; }
    public string DataType { get; set; }
    public string Format { get; set; }
    public DateRange DateRange { get; set; }
}

public class ExportResult
{
    public string ExportId { get; set; }
    public string RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public ExportStatus Status { get; set; }
    public string DownloadUrl { get; set; }
    public string ErrorMessage { get; set; }
    public long EstimatedSize { get; set; }
}

public class DocumentRequest
{
    public string DocumentId { get; set; }
    public string DocumentType { get; set; }
    public object DocumentData { get; set; }
}

public class ProcessingSummary
{
    public string BatchId { get; set; }
    public int TotalDocuments { get; set; }
    public int ProcessedSuccessfully { get; set; }
    public int FailedDocuments { get; set; }
    public List<string> ProcessingErrors { get; set; }
    public DateTime CompletedAt { get; set; }
}

public enum ReportStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public enum ExportStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ReportData
{
    public object Data { get; set; }
    public DateTime LoadedAt { get; set; }
}

public class GeneratedReport
{
    public string Id { get; set; }
    public object Content { get; set; }
    public string Format { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class SavedReport
{
    public string Id { get; set; }
    public string Url { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ExportStatus
{
    public bool IsCompleted { get; set; }
    public string DownloadUrl { get; set; }
    public string ErrorMessage { get; set; }
    public long EstimatedSize { get; set; }
}

public class ExportMetrics
{
    public string ExportId { get; set; }
    public TimeSpan Duration { get; set; }
    public ExportStatus Status { get; set; }
    public long EstimatedSize { get; set; }
    public DateTime Timestamp { get; set; }
}

// Interfaces de servicios adicionales
public interface IDataService
{
    Task<ReportData> LoadDataForReportAsync(ReportRequest request);
    Task<AnalyticsReport> GenerateDetailedAnalyticsAsync(AnalyticsRequest request);
    Task<object> GetBasicStatsAsync(DateRange dateRange);
    Task<object> PrepareExportDataAsync(ExportRequest request);
    Task<object> GetBatchProcessingResultsAsync(string batchId);
    Task<object> ProcessDocumentBatchAsync(List<DocumentRequest> documents, string batchId);
    Task RecordExportMetricsAsync(ExportMetrics metrics);
}

public interface IReportGenerator
{
    Task<GeneratedReport> GenerateAsync(ReportData data, string reportId);
    Task<object> ExportToFormatAsync(object data, string format);
}

public interface IReportStorage
{
    Task<SavedReport> SaveAsync(GeneratedReport report);
    Task<ExportStatus> GetExportStatusAsync(string exportId);
    Task UploadExportAsync(object exportedData, string exportId);
}

public interface INotificationService
{
    Task NotifyReportCompletedAsync(string userId, ReportResult result);
    Task NotifyReportFailedAsync(string userId, ReportResult result);
}
```

### Ejemplo 3: Sistema de Métricas y Monitoreo

```csharp
public class MetricsCollectionService
{
    private readonly IMetricsRepository _metricsRepository;
    private readonly ISystemMonitor _systemMonitor;
    private readonly IAlertService _alertService;
    private readonly ILogger _logger;
    
    public MetricsCollectionService(
        IMetricsRepository metricsRepository,
        ISystemMonitor systemMonitor,
        IAlertService alertService,
        ILogger logger)
    {
        _metricsRepository = metricsRepository;
        _systemMonitor = systemMonitor;
        _alertService = alertService;
        _logger = logger;
    }
    
    public async Task<MlResult<OperationMetrics>> ExecuteWithMetricsAsync<T>(Func<Task<MlResult<T>>> operation, string operationName)
    {
        var operationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        var startMemory = GC.GetTotalMemory(false);
        
        return await operation()
            .TryMapAlwaysAsync(
                async operationResult => 
                {
                    var endTime = DateTime.UtcNow;
                    var endMemory = GC.GetTotalMemory(false);
                    var duration = endTime - startTime;
                    var memoryUsed = endMemory - startMemory;
                    
                    var metrics = new OperationMetrics
                    {
                        OperationId = operationId,
                        OperationName = operationName,
                        StartTime = startTime,
                        EndTime = endTime,
                        Duration = duration,
                        MemoryUsed = memoryUsed,
                        Success = operationResult.IsValid,
                        ErrorMessage = operationResult.IsValid ? null : operationResult.ErrorDetails.FirstErrorMessage,
                        SystemMetrics = await _systemMonitor.GetCurrentSystemMetricsAsync()
                    };
                    
                    await RecordOperationMetricsAsync(metrics);
                    await CheckPerformanceThresholdsAsync(metrics);
                    
                    return metrics;
                },
                async errors => 
                {
                    var endTime = DateTime.UtcNow;
                    var endMemory = GC.GetTotalMemory(false);
                    var duration = endTime - startTime;
                    var memoryUsed = endMemory - startMemory;
                    
                    var metrics = new OperationMetrics
                    {
                        OperationId = operationId,
                        OperationName = operationName,
                        StartTime = startTime,
                        EndTime = endTime,
                        Duration = duration,
                        MemoryUsed = memoryUsed,
                        Success = false,
                        ErrorMessage = errors.FirstErrorMessage,
                        SystemMetrics = await _systemMonitor.GetCurrentSystemMetricsAsync()
                    };
                    
                    await RecordOperationMetricsAsync(metrics);
                    await CheckPerformanceThresholdsAsync(metrics);
                    
                    return metrics;
                },
                ex => $"Failed to collect metrics for operation {operationName}: {ex.Message}"
            );
    }
    
    public async Task<MlResult<HealthCheckResult>> PerformHealthCheckAsync()
    {
        var healthCheckId = Guid.NewGuid().ToString();
        
        return await CheckSystemHealthAsync()
            .MapAlwaysAsync(
                // SIEMPRE generar un resultado de health check
                async () => 
                {
                    var systemMetrics = await _systemMonitor.GetCurrentSystemMetricsAsync();
                    var databaseHealth = await CheckDatabaseHealthAsync();
                    var externalServicesHealth = await CheckExternalServicesHealthAsync();
                    
                    var overallHealth = DetermineOverallHealth(databaseHealth, externalServicesHealth, systemMetrics);
                    
                    var healthResult = new HealthCheckResult
                    {
                        CheckId = healthCheckId,
                        Timestamp = DateTime.UtcNow,
                        OverallStatus = overallHealth,
                        SystemMetrics = systemMetrics,
                        DatabaseHealth = databaseHealth,
                        ExternalServicesHealth = externalServicesHealth,
                        Recommendations = GenerateHealthRecommendations(overallHealth, systemMetrics)
                    };
                    
                    await RecordHealthCheckAsync(healthResult);
                    
                    if (overallHealth != HealthStatus.Healthy)
                    {
                        await _alertService.SendHealthAlertAsync(healthResult);
                    }
                    
                    return healthResult;
                }
            );
    }
    
    public async Task<MlResult<string>> RecordEventWithTimestampAsync(string eventName, object eventData)
    {
        // Ejemplo de MapAlways simple: siempre generar timestamp único
        return await ProcessEventAsync(eventName, eventData)
            .MapAlwaysAsync(async () => 
            {
                var timestamp = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}_{Guid.NewGuid():N}";
                await _metricsRepository.RecordEventTimestampAsync(eventName, timestamp);
                await _logger.LogInformationAsync($"Event {eventName} recorded with timestamp {timestamp}");
                return timestamp;
            });
    }
    
    public async Task<MlResult<PerformanceReport>> GeneratePerformanceReportAsync(TimeSpan period)
    {
        var reportId = Guid.NewGuid().ToString();
        var endTime = DateTime.UtcNow;
        var startTime = endTime - period;
        
        return await CollectPerformanceDataAsync(startTime, endTime)
            .TryMapAlwaysAsync(
                validData => AnalyzePerformanceDataAsync(validData, reportId),
                async errors => 
                {
                    // Si no se pueden obtener datos completos, generar reporte básico
                    _logger.LogWarning($"Performance data collection failed, generating basic report: {errors.FirstErrorMessage}");
                    
                    var basicMetrics = await GetBasicPerformanceMetricsAsync(startTime, endTime);
                    return new PerformanceReport
                    {
                        ReportId = reportId,
                        Period = new TimeRange { Start = startTime, End = endTime },
                        BasicMetrics = basicMetrics,
                        IsComplete = false,
                        GeneratedAt = DateTime.UtcNow,
                        Notes = "Basic report due to data collection issues"
                    };
                },
                ex => $"Failed to generate performance report {reportId}: {ex.Message}"
            );
    }
    
    private async Task RecordOperationMetricsAsync(OperationMetrics metrics)
    {
        try
        {
            await _metricsRepository.SaveOperationMetricsAsync(metrics);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to record operation metrics: {ex.Message}");
        }
    }
    
    private async Task CheckPerformanceThresholdsAsync(OperationMetrics metrics)
    {
        const double maxDurationMs = 5000; // 5 segundos
        const long maxMemoryMb = 100 * 1024 * 1024; // 100MB
        
        if (metrics.Duration.TotalMilliseconds > maxDurationMs)
        {
            await _alertService.SendPerformanceAlertAsync(
                $"Operation {metrics.OperationName} exceeded duration threshold: {metrics.Duration.TotalMilliseconds}ms");
        }
        
        if (metrics.MemoryUsed > maxMemoryMb)
        {
            await _alertService.SendPerformanceAlertAsync(
                $"Operation {metrics.OperationName} exceeded memory threshold: {metrics.MemoryUsed / (1024 * 1024)}MB");
        }
    }
    
    private async Task<MlResult<object>> CheckSystemHealthAsync()
    {
        try
        {
            var health = await _systemMonitor.CheckOverallSystemHealthAsync();
            return MlResult<object>.Valid(health);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException("System health check failed", ex);
        }
    }
    
    private async Task<HealthStatus> CheckDatabaseHealthAsync()
    {
        try
        {
            var isHealthy = await _metricsRepository.CheckConnectionAsync();
            return isHealthy ? HealthStatus.Healthy : HealthStatus.Degraded;
        }
        catch
        {
            return HealthStatus.Unhealthy;
        }
    }
    
    private async Task<Dictionary<string, HealthStatus>> CheckExternalServicesHealthAsync()
    {
        var services = new Dictionary<string, HealthStatus>();
        
        // Comprobar diferentes servicios externos
        var serviceNames = new[] { "PaymentService", "NotificationService", "AuthService" };
        
        foreach (var serviceName in serviceNames)
        {
            try
            {
                var isHealthy = await _systemMonitor.CheckExternalServiceAsync(serviceName);
                services[serviceName] = isHealthy ? HealthStatus.Healthy : HealthStatus.Degraded;
            }
            catch
            {
                services[serviceName] = HealthStatus.Unhealthy;
            }
        }
        
        return services;
    }
    
    private HealthStatus DetermineOverallHealth(
        HealthStatus databaseHealth, 
        Dictionary<string, HealthStatus> externalServicesHealth, 
        SystemMetrics systemMetrics)
    {
        if (databaseHealth == HealthStatus.Unhealthy)
            return HealthStatus.Unhealthy;
            
        if (externalServicesHealth.Values.Any(h => h == HealthStatus.Unhealthy))
            return HealthStatus.Degraded;
            
        if (systemMetrics.CpuUsagePercent > 90 || systemMetrics.MemoryUsagePercent > 90)
            return HealthStatus.Degraded;
            
        return HealthStatus.Healthy;
    }
    
    private List<string> GenerateHealthRecommendations(HealthStatus overallHealth, SystemMetrics systemMetrics)
    {
        var recommendations = new List<string>();
        
        if (systemMetrics.CpuUsagePercent > 80)
            recommendations.Add("Consider scaling CPU resources");
            
        if (systemMetrics.MemoryUsagePercent > 80)
            recommendations.Add("Consider increasing memory allocation");
            
        if (overallHealth != HealthStatus.Healthy)
            recommendations.Add("Review system logs for error patterns");
            
        return recommendations;
    }
    
    private async Task RecordHealthCheckAsync(HealthCheckResult result)
    {
        try
        {
            await _metricsRepository.SaveHealthCheckResultAsync(result);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to record health check: {ex.Message}");
        }
    }
    
    private async Task<MlResult<object>> ProcessEventAsync(string eventName, object eventData)
    {
        try
        {
            await _metricsRepository.ProcessEventAsync(eventName, eventData);
            return MlResult<object>.Valid(eventData);
        }
        catch (Exception ex)
        {
            return MlResult<object>.FailWithException($"Event processing failed for {eventName}", ex);
        }
    }
    
    private async Task<MlResult<PerformanceData>> CollectPerformanceDataAsync(DateTime startTime, DateTime endTime)
    {
        try
        {
            var data = await _metricsRepository.GetPerformanceDataAsync(startTime, endTime);
            return MlResult<PerformanceData>.Valid(data);
        }
        catch (Exception ex)
        {
            return MlResult<PerformanceData>.FailWithException("Performance data collection failed", ex);
        }
    }
    
    private async Task<PerformanceReport> AnalyzePerformanceDataAsync(PerformanceData data, string reportId)
    {
        var analysis = await _systemMonitor.AnalyzePerformanceAsync(data);
        
        return new PerformanceReport
        {
            ReportId = reportId,
            Period = new TimeRange { Start = data.StartTime, End = data.EndTime },
            DetailedAnalysis = analysis,
            IsComplete = true,
            GeneratedAt = DateTime.UtcNow
        };
    }
    
    private async Task<BasicPerformanceMetrics> GetBasicPerformanceMetricsAsync(DateTime startTime, DateTime endTime)
    {
        try
        {
            return await _metricsRepository.GetBasicMetricsAsync(startTime, endTime);
        }
        catch
        {
            return new BasicPerformanceMetrics
            {
                StartTime = startTime,
                EndTime = endTime,
                TotalRequests = 0,
                AverageResponseTime = TimeSpan.Zero,
                ErrorRate = 0
            };
        }
    }
}

// Clases de apoyo para métricas
public class OperationMetrics
{
    public string OperationId { get; set; }
    public string OperationName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public long MemoryUsed { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public SystemMetrics SystemMetrics { get; set; }
}

public class HealthCheckResult
{
    public string CheckId { get; set; }
    public DateTime Timestamp { get; set; }
    public HealthStatus OverallStatus { get; set; }
    public SystemMetrics SystemMetrics { get; set; }
    public HealthStatus DatabaseHealth { get; set; }
    public Dictionary<string, HealthStatus> ExternalServicesHealth { get; set; }
    public List<string> Recommendations { get; set; }
}

public class SystemMetrics
{
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public long DiskSpaceUsed { get; set; }
    public int ActiveConnections { get; set; }
}

public class PerformanceReport
{
    public string ReportId { get; set; }
    public TimeRange Period { get; set; }
    public object DetailedAnalysis { get; set; }
    public BasicPerformanceMetrics BasicMetrics { get; set; }
    public bool IsComplete { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string Notes { get; set; }
}

public class PerformanceData
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<OperationMetrics> Operations { get; set; }
    public List<SystemMetrics> SystemSnapshots { get; set; }
}

public class BasicPerformanceMetrics
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalRequests { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
}

public class TimeRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}

// Interfaces de servicios para métricas
public interface IMetricsRepository
{
    Task SaveOperationMetricsAsync(OperationMetrics metrics);
    Task<bool> CheckConnectionAsync();
    Task ProcessEventAsync(string eventName, object eventData);
    Task RecordEventTimestampAsync(string eventName, string timestamp);
    Task SaveHealthCheckResultAsync(HealthCheckResult result);
    Task<PerformanceData> GetPerformanceDataAsync(DateTime startTime, DateTime endTime);
    Task<BasicPerformanceMetrics> GetBasicMetricsAsync(DateTime startTime, DateTime endTime);
}

public interface ISystemMonitor
{
    Task<SystemMetrics> GetCurrentSystemMetricsAsync();
    Task<object> CheckOverallSystemHealthAsync();
    Task<bool> CheckExternalServiceAsync(string serviceName);
    Task<object> AnalyzePerformanceAsync(PerformanceData data);
}

public interface IAlertService
{
    Task SendPerformanceAlertAsync(string message);
    Task SendHealthAlertAsync(HealthCheckResult healthResult);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar MapAlways Simple vs MapAlways Dual

```csharp
// ✅ Correcto: MapAlways simple para valores independientes del resultado
var timestamp = ProcessData(input)
    .MapAlways(() => DateTime.UtcNow); // El timestamp es independiente del procesamiento

var correlationId = ValidateUser(userData)
    .MapAlways(() => Guid.NewGuid().ToString()); // El ID es independiente de la validación

// ✅ Correcto: MapAlways dual para transformaciones que dependen del estado
var response = ProcessOrder(order)
    .MapAlways(
        successOrder => new ApiResponse { Status = "Success", Data = successOrder },
        errors => new ApiResponse { Status = "Error", Message = errors.FirstErrorMessage }
    );

// ❌ Incorrecto: Usar MapAlways simple cuando necesitas el contexto del resultado
var report = GenerateReport(data)
    .MapAlways(() => new Report()); // Se pierde información sobre el éxito/fallo
```

### 2. Garantía de Notificación y Auditoría

```csharp
// ✅ Correcto: Usar MapAlways para garantizar notificaciones
var result = ProcessPayment(payment)
    .TryMapAlways(
        successPayment => successPayment,
        errors => CreateFailedPayment(payment),
        ex => $"Payment processing error: {ex.Message}"
    )
    .ExecSelfAlways(finalResult => NotifyUser(payment.UserId, finalResult)); // Siempre notifica

// ✅ Correcto: Auditoría garantizada independientemente del resultado
var auditedResult = CriticalOperation(request)
    .MapAlways(
        success => new AuditedResult { Success = true, Data = success, AuditId = LogSuccess() },
        errors => new AuditedResult { Success = false, Errors = errors, AuditId = LogFailure() }
    );
```

### 3. Manejo de Recursos y Cleanup

```csharp
// ✅ Correcto: Cleanup garantizado usando MapAlways
public async Task<MlResult<string>> ProcessWithGuaranteedCleanup(ProcessingRequest request)
{
    var resources = new ResourceManager();
    
    return await ProcessWithResources(request, resources)
        .TryMapAlwaysAsync(
            async () => 
            {
                var cleanupId = await resources.CleanupAsync();
                await LogOperationCompleted(request.Id, cleanupId);
                return cleanupId;
            },
            ex => $"Cleanup failed: {ex.Message}"
        );
}

// El cleanup SIEMPRE se ejecuta, independientemente del resultado del procesamiento
```

### 4. Transformación de Tipos Garantizada

```csharp
// ✅ Correcto: Convertir siempre a formato de respuesta API
public async Task<MlResult<ApiResponse<T>>> ToApiResponse<T>(this Task<MlResult<T>> resultAsync)
{
    return await resultAsync.MapAlwaysAsync(
        validData => new ApiResponse<T> 
        { 
            Success = true, 
            Data = validData, 
            Timestamp = DateTime.UtcNow 
        },
        errors => new ApiResponse<T> 
        { 
            Success = false, 
            ErrorMessage = errors.FirstErrorMessage, 
            Timestamp = DateTime.UtcNow 
        }
    );
}

// Uso: cualquier operación se convierte siempre en ApiResponse
var response = await GetUserData(userId).ToApiResponse();
```

### 5. Logging y Métricas Consistentes

```csharp
// ✅ Correcto: Métricas que siempre se registran
var result = await ExecuteOperation(request)
    .TryMapAlwaysAsync(
        async () => 
        {
            var metrics = new OperationMetrics
            {
                OperationId = request.Id,
                CompletedAt = DateTime.UtcNow,
                Success = true // o false según el contexto
            };
            
            await _metricsService.RecordAsync(metrics);
            return metrics.OperationId;
        },
        ex => $"Failed to record metrics: {ex.Message}"
    );
```

### 6. Evitar Efectos Secundarios No Deseados

```csharp
// ❌ Incorrecto: MapAlways que modifica estado global
var result = ProcessOrder(order)
    .MapAlways(() => 
    {
        GlobalState.LastProcessedOrder = order; // Peligroso!
        return "processed";
    });

// ✅ Correcto: Efectos secundarios controlados
var result = ProcessOrder(order)
    .TryMapAlways(
        () => 
        {
            var summary = CreateProcessingSummary(order);
            return summary.Id; // Solo retorna información, no modifica estado
        },
        ex => $"Summary creation failed: {ex.Message}"
    );
```

---

## Consideraciones de Rendimiento

### Ejecución Garantizada

- Los métodos `MapAlways` **SIEMPRE** se ejecutan, lo que puede introducir overhead constante
- Considerar el costo de las funciones que se ejecutan incondicionalmente
- Las versiones `Try*` tienen overhead adicional de manejo de excepciones

### Transformaciones de Tipos

- Las transformaciones que crean objetos complejos pueden impactar el rendimiento
- Considerar reutilización de objetos cuando sea apropiado
- Los métodos duales ejecutan solo una de las dos funciones, no ambas

### Operaciones Asíncronas

- Las versiones `Async` mantienen el contexto de sincronización apropiadamente
- Considerar usar `ConfigureAwait(false)` en contextos de librería
- Las operaciones de cleanup asíncronas pueden impactar la latencia total

---

## Resumen

Los métodos **`MapAlways`** proporcionan capacidades para:

- **Transformación Garantizada**: Asegurar que siempre se produce una transformación
- **Finalización de Pipelines**: Convertir cualquier resultado en un formato específico
- **Auditoría y Logging**: Garantizar que ciertas operaciones siempre se ejecuten
- **Cleanup de Recursos**: Asegurar liberación de recursos independientemente del resultado

### Métodos Principales

- **`MapAlways<T, TReturn>(Func<TReturn>)`**: Transformación incondicional simple
- **`MapAlways<T, TReturn>(Func<T, TReturn>, Func<MlErrorsDetails, TReturn>)`**: Transformación condicional dual
- **`TryMapAlways`**: Versiones seguras con captura de excepciones
- **Variantes Asíncronas**: Soporte completo para operaciones asíncronas

### Casos de Uso Ideales

- **APIs REST** que siempre deben retornar respuestas estructuradas
- **Sistemas de Auditoría** que requieren logging garantizado
- **Pipelines de Métricas** que deben recopilar datos independientemente del resultado
- **Cleanup de Recursos** que debe ejecutarse siempre

La clave está en usar estos métodos cuando se necesita **garantía absoluta** de ejecución, ya sea para transformación, logging, cleanup o notificación, independientemente del estado del resultado original.