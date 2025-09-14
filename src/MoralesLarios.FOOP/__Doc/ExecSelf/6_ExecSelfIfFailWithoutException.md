# MlResultActionsExecSelfIfFailWithException - Manejo de Excepciones Específicas

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFailWithException](#métodos-execselfiffailwithexception)
4. [Métodos ExecSelfIfFailWithoutException](#métodos-execselfiffailwithoutexception)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Métodos TryExecSelfIfFailWithException - Captura de Excepciones](#métodos-tryexecselfiffailwithexception---captura-de-excepciones)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos `ExecSelfIfFailWithException` y `ExecSelfIfFailWithoutException` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido** y basándose en si el fallo **contiene o no contiene una excepción preservada** en los detalles del error.

### Propósito Principal

**ExecSelfIfFailWithException**:
- **Manejo Específico de Excepciones**: Acceder a la excepción original que causó el fallo
- **Logging Detallado**: Registrar stack traces y detalles técnicos de excepciones
- **Análisis de Errores**: Analizar tipos específicos de excepciones para patrones
- **Alertas Técnicas**: Enviar notificaciones específicas para ciertos tipos de excepciones
- **Debugging Avanzado**: Acceso completo a la excepción para diagnóstico

**ExecSelfIfFailWithoutException**:
- **Manejo de Errores de Negocio**: Procesar fallos que no son causados por excepciones técnicas
- **Validación y Reglas**: Manejar errores de validación y reglas de negocio
- **Logging de Negocio**: Registrar errores funcionales sin detalles técnicos
- **Flujos Alternativos**: Ejecutar lógica cuando el fallo es "esperado" y no técnico

---

## Análisis de los Métodos

### Estructura y Filosofía

Estos métodos implementan el patrón de **manejo de errores diferenciado por causa**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido con Excepción → ExecSelfIfFailWithException → Acción(excepción)
      ↓                          ↓
Resultado Fallido sin Excepción → ExecSelfIfFailWithoutException → Acción(errores)
```

### Características Principales

1. **Diferenciación por Causa**: Distintas acciones según si el fallo tiene excepción o no
2. **Extracción de Excepciones**: Utiliza `GetDetailException()` para extraer excepciones preservadas
3. **Inmutabilidad**: El resultado original nunca se modifica
4. **Manejo Específico**: Acciones especializadas para errores técnicos vs errores de negocio
5. **Complementariedad**: Ambos métodos pueden usarse en conjunto para cobertura completa

---

## Métodos ExecSelfIfFailWithException

### `ExecSelfIfFailWithException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido y contiene una excepción preservada

```csharp
public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                         Action<Exception> actionFailException)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFailException`: Acción a ejecutar solo si `source` es fallido y contiene una excepción

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido pero no contiene excepción: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido y contiene excepción: Extrae la excepción, ejecuta `actionFailException(exception)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ProcessData(inputData)
    .ExecSelfIfFailWithException(ex => 
    {
        _logger.LogError(ex, "Technical error occurred during data processing");
        _telemetry.RecordException(ex);
        
        // Manejo específico por tipo de excepción
        switch (ex)
        {
            case SqlException sqlEx:
                _alertService.SendDatabaseAlert(sqlEx);
                break;
            case TimeoutException timeoutEx:
                _alertService.SendTimeoutAlert(timeoutEx);
                break;
            case OutOfMemoryException memEx:
                _alertService.SendCriticalResourceAlert(memEx);
                break;
        }
    });
```

**Ejemplo con Análisis de Excepciones**:
```csharp
var result = CallExternalAPI(request)
    .ExecSelfIfFailWithException(ex => 
    {
        // Logging estructurado con detalles de la excepción
        _logger.LogError(ex, "External API call failed", new 
        {
            ExceptionType = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            InnerException = ex.InnerException?.GetType().Name,
            RequestId = request.Id
        });
        
        // Análisis de patrones de excepción
        AnalyzeExceptionPattern(ex, request);
        
        // Métricas específicas por tipo de excepción
        _metrics.IncrementCounter($"api.exceptions.{ex.GetType().Name}");
    });
```

---

## Métodos ExecSelfIfFailWithoutException

### `ExecSelfIfFailWithoutException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido pero NO contiene una excepción preservada

```csharp
public static MlResult<T> ExecSelfIfFailWithoutException<T>(this MlResult<T> source,
                                                            Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFail`: Acción a ejecutar solo si `source` es fallido y NO contiene una excepción

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFail`
- Si `source` es fallido y contiene excepción: Retorna `source` sin cambios, no ejecuta `actionFail`
- Si `source` es fallido y NO contiene excepción: Ejecuta `actionFail(errorsDetails)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ValidateUser(userData)
    .ExecSelfIfFailWithoutException(errors => 
    {
        // Este es un error de validación/negocio, no técnico
        _logger.LogWarning($"User validation failed: {string.Join(", ", errors.AllErrors)}");
        _metrics.IncrementCounter("user.validation_failures");
        
        // Análisis de patrones de validación
        AnalyzeValidationFailures(errors);
        
        // Notificación al usuario (no es un error técnico)
        _notificationService.SendValidationErrors(userData.Email, errors.AllErrors);
    });
```

**Ejemplo con Reglas de Negocio**:
```csharp
var result = ProcessBusinessRule(businessData)
    .ExecSelfIfFailWithoutException(errors => 
    {
        // Errores de reglas de negocio - esperados y manejables
        _businessLogger.LogRuleViolation(businessData.EntityId, errors.AllErrors);
        
        // Métricas de reglas de negocio
        foreach (var error in errors.AllErrors)
        {
            var ruleType = ExtractRuleType(error);
            _metrics.IncrementCounter($"business.rules.violations.{ruleType}");
        }
        
        // Notificar a stakeholders sobre violaciones de reglas
        _businessNotifications.SendRuleViolationAlert(businessData, errors);
    });
```

---

## Variantes Asíncronas

### `ExecSelfIfFailWithExceptionAsync<T>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync)
```

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .ExecSelfIfFailWithExceptionAsync(async ex => 
    {
        await _logger.LogErrorAsync(ex, "Document processing failed with technical error");
        await _telemetryService.RecordExceptionAsync(ex);
        
        // Envío asíncrono de alertas basado en tipo de excepción
        if (ex is FileNotFoundException)
        {
            await _alertService.SendFileNotFoundAlertAsync(document.FilePath);
        }
        else if (ex is UnauthorizedAccessException)
        {
            await _securityService.LogSecurityIncidentAsync(ex, document.UserId);
        }
    });
```

### `ExecSelfIfFailWithoutExceptionAsync<T>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task> actionFailExceptionAsync)
```

**Ejemplo**:
```csharp
var result = await ValidateOrderAsync(order)
    .ExecSelfIfFailWithoutExceptionAsync(async errors => 
    {
        await _businessLogger.LogValidationFailureAsync(order.Id, errors.AllErrors);
        
        // Análisis asíncrono de patrones de validación
        await AnalyzeValidationPatternsAsync(order.CustomerId, errors);
        
        // Envío de notificaciones de negocio
        await _customerService.SendValidationFailureNotificationAsync(
            order.CustomerId, 
            order.Id, 
            errors.AllErrors);
    });
```

### Versiones con Fuente Asíncrona

```csharp
// ExecSelfIfFailWithException con fuente asíncrona
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException)

// ExecSelfIfFailWithoutException con fuente asíncrona
public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<MlErrorsDetails> actionFail)
```

---

## Métodos TryExecSelfIfFailWithException - Captura de Excepciones

### `TryExecSelfIfFailWithException<T>()` - Versión Segura

```csharp
public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            string errorMessage = null!)
```

**Comportamiento Especial**: 
- Si la acción lanza una excepción, **las versiones `Try*` agregan el error de la excepción a los errores originales**
- El resultado final contiene tanto la excepción original como cualquier error de la acción

**Ejemplo**:
```csharp
var result = ProcessCriticalOperation(data)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Esta acción puede fallar
            SendCriticalAlert(originalException); // Puede lanzar NetworkException
            LogToExternalSystem(originalException); // Puede lanzar ServiceUnavailableException
            UpdateDashboard(originalException); // Puede lanzar TimeoutException
        },
        ex => $"Failed to handle technical error notification: {ex.Message}"
    );

// Si ProcessCriticalOperation falla con una excepción: result contiene esa excepción + detalles
// Si además las acciones de manejo lanzan excepciones: result contiene la excepción original + errores de las acciones
```

### Versiones Asíncronas de TryExecSelfIfFailWithException

```csharp
// Todas las variantes asíncronas seguras
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException,
    Func<Exception, string> errorMessageBuilder)
```

**Nota sobre ExecSelfIfFailWithoutException**: No hay versiones `Try*` para `ExecSelfIfFailWithoutException` en el código mostrado, posiblemente porque los errores de negocio (sin excepciones) típicamente no requieren manejo adicional de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Monitoreo Diferenciado por Tipo de Error

```csharp
public class DifferentiatedErrorMonitoringService
{
    private readonly ITechnicalLogger _technicalLogger;
    private readonly IBusinessLogger _businessLogger;
    private readonly IAlertService _alertService;
    private readonly IMetricsService _metrics;
    private readonly ITelemetryService _telemetry;
    private readonly IIncidentService _incidentService;
    
    public DifferentiatedErrorMonitoringService(
        ITechnicalLogger technicalLogger,
        IBusinessLogger businessLogger,
        IAlertService alertService,
        IMetricsService metrics,
        ITelemetryService telemetry,
        IIncidentService incidentService)
    {
        _technicalLogger = technicalLogger;
        _businessLogger = businessLogger;
        _alertService = alertService;
        _metrics = metrics;
        _telemetry = telemetry;
        _incidentService = incidentService;
    }
    
    public async Task<MlResult<ProcessedData>> ProcessWithDifferentiatedMonitoringAsync(InputData data)
    {
        var operationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateInputData(data)
            // Manejo de errores de validación (sin excepción - errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogValidationFailureAsync(new ValidationFailureLog
                {
                    OperationId = operationId,
                    InputDataId = data.Id,
                    ValidationErrors = errors.AllErrors,
                    Timestamp = DateTime.UtcNow,
                    Severity = "Warning"
                });
                
                await _metrics.IncrementCounterAsync("business.validation_failures");
                
                // Análisis de patrones de validación para mejora del negocio
                await AnalyzeValidationPatternsForBusinessImprovementAsync(data, errors);
            })
            // Manejo de errores técnicos de validación (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "InputValidation",
                    InputDataType = data.GetType().Name,
                    InputDataSize = data.EstimatedSizeInBytes,
                    Timestamp = DateTime.UtcNow
                });
                
                await _telemetry.RecordExceptionAsync(ex, new Dictionary<string, string>
                {
                    ["operation_id"] = operationId,
                    ["operation_type"] = "validation",
                    ["data_type"] = data.GetType().Name
                });
                
                // Crear incidente automático para errores técnicos críticos
                if (IsCriticalTechnicalException(ex))
                {
                    await _incidentService.CreateAutomaticIncidentAsync(new TechnicalIncident
                    {
                        OperationId = operationId,
                        Exception = ex,
                        Severity = DetermineSeverity(ex),
                        AffectedComponent = "DataValidation",
                        RequiresImmediateAttention = true
                    });
                }
            },
            ex => $"Failed to handle technical validation error for operation {operationId}: {ex.Message}")
            
            .BindAsync(async validData => await ProcessBusinessLogic(validData))
            
            // Manejo de errores de lógica de negocio (sin excepción)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogBusinessRuleViolationAsync(new BusinessRuleViolationLog
                {
                    OperationId = operationId,
                    EntityId = data.EntityId,
                    RuleViolations = errors.AllErrors,
                    BusinessContext = ExtractBusinessContext(data),
                    Timestamp = DateTime.UtcNow
                });
                
                await _metrics.IncrementCounterAsync("business.rule_violations");
                
                // Notificar a stakeholders de negocio sobre violaciones
                await NotifyBusinessStakeholdersAsync(data, errors);
            })
            
            // Manejo de errores técnicos en lógica de negocio (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "BusinessLogicProcessing",
                    EntityId = data.EntityId,
                    Timestamp = DateTime.UtcNow
                });
                
                // Análisis específico por tipo de excepción técnica
                await HandleSpecificTechnicalExceptionAsync(ex, operationId, data);
                
                // Escalamiento automático para excepciones críticas
                if (RequiresEscalation(ex))
                {
                    await _alertService.SendEscalationAlertAsync(new EscalationAlert
                    {
                        OperationId = operationId,
                        Exception = ex,
                        AffectedData = data.Id,
                        EscalationLevel = DetermineEscalationLevel(ex),
                        RequiresOnCallResponse = true
                    });
                }
            },
            ex => $"Failed to handle technical business logic error for operation {operationId}: {ex.Message}")
            
            .BindAsync(async processedLogic => await SaveResults(processedLogic))
            
            // Manejo de errores de persistencia (sin excepción - posibles errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogDataPersistenceFailureAsync(new DataPersistenceFailureLog
                {
                    OperationId = operationId,
                    EntityId = data.EntityId,
                    PersistenceErrors = errors.AllErrors,
                    AttemptedOperation = "Save",
                    Timestamp = DateTime.UtcNow
                });
                
                // Intentar mecanismos de recuperación de datos
                await TriggerDataRecoveryMechanismsAsync(operationId, data);
            })
            
            // Manejo de errores técnicos de persistencia (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "DataPersistence",
                    EntityId = data.EntityId,
                    Timestamp = DateTime.UtcNow
                });
                
                // Manejo específico de excepciones de persistencia
                switch (ex)
                {
                    case SqlException sqlEx:
                        await HandleDatabaseExceptionAsync(sqlEx, operationId, data);
                        break;
                    case TimeoutException timeoutEx:
                        await HandlePersistenceTimeoutAsync(timeoutEx, operationId, data);
                        break;
                    case OutOfMemoryException memEx:
                        await HandleMemoryExceptionAsync(memEx, operationId);
                        break;
                    default:
                        await HandleGenericPersistenceExceptionAsync(ex, operationId, data);
                        break;
                }
                
                // Activar procedimientos de backup y recuperación
                await ActivateDataBackupProceduresAsync(operationId, data);
            },
            ex => $"Failed to handle technical persistence error for operation {operationId}: {ex.Message}")
            
            // Logging final diferenciado
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _businessLogger.LogOperationCompletionAsync(new OperationCompletionLog
                {
                    OperationId = operationId,
                    Status = "BusinessFailure",
                    Duration = duration,
                    ErrorCount = errors.AllErrors.Length,
                    FinalErrors = errors.AllErrors
                });
            })
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _technicalLogger.LogOperationCompletionAsync(new OperationCompletionLog
                {
                    OperationId = operationId,
                    Status = "TechnicalFailure",
                    Duration = duration,
                    Exception = ex,
                    ExceptionType = ex.GetType().Name
                });
                
                // Actualizar métricas de salud del sistema
                await UpdateSystemHealthMetricsAsync(ex, duration);
            },
            ex => $"Failed to log final technical error for operation {operationId}: {ex.Message}");
    }
    
    private async Task AnalyzeValidationPatternsForBusinessImprovementAsync(InputData data, MlErrorsDetails errors)
    {
        try
        {
            var pattern = new ValidationPattern
            {
                DataType = data.GetType().Name,
                ErrorTypes = errors.AllErrors.Select(e => CategorizeValidationError(e)).ToList(),
                DataCharacteristics = ExtractDataCharacteristics(data),
                Timestamp = DateTime.UtcNow
            };
            
            await _businessLogger.LogValidationPatternAsync(pattern);
            
            // Detectar tendencias que pueden indicar necesidad de mejoras en el negocio
            var recentPatterns = await GetRecentValidationPatternsAsync(data.GetType().Name);
            if (recentPatterns.Count > 10 && HasSignificantPattern(recentPatterns))
            {
                await _alertService.SendBusinessImprovementSuggestionAsync(new BusinessImprovementSuggestion
                {
                    Area = "DataValidation",
                    DataType = data.GetType().Name,
                    SuggestedImprovement = GenerateImprovementSuggestion(recentPatterns),
                    Priority = "Medium",
                    EstimatedImpact = "ReduceValidationFailures"
                });
            }
        }
        catch (Exception ex)
        {
            await _technicalLogger.LogWarningAsync($"Failed to analyze validation patterns: {ex.Message}");
        }
    }
    
    private async Task HandleSpecificTechnicalExceptionAsync(Exception ex, string operationId, InputData data)
    {
        switch (ex)
        {
            case NetworkException netEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.network");
                await _alertService.SendNetworkIssueAlertAsync(netEx, operationId);
                break;
                
            case DatabaseException dbEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.database");
                await _alertService.SendDatabaseIssueAlertAsync(dbEx, operationId);
                await CheckDatabaseHealthAsync();
                break;
                
            case SecurityException secEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.security");
                await _alertService.SendSecurityIncidentAlertAsync(secEx, operationId, data.EntityId);
                await TriggerSecurityResponseAsync(secEx, data);
                break;
                
            case PerformanceException perfEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.performance");
                await _alertService.SendPerformanceIssueAlertAsync(perfEx, operationId);
                await TriggerPerformanceAnalysisAsync(perfEx, data);
                break;
                
            default:
                await _metrics.IncrementCounterAsync("technical.exceptions.unknown");
                await _alertService.SendUnknownTechnicalIssueAlertAsync(ex, operationId);
                break;
        }
    }
    
    private async Task NotifyBusinessStakeholdersAsync(InputData data, MlErrorsDetails errors)
    {
        try
        {
            var stakeholders = await GetRelevantStakeholdersAsync(data.EntityId, errors);
            
            foreach (var stakeholder in stakeholders)
            {
                var notification = new BusinessErrorNotification
                {
                    StakeholderId = stakeholder.Id,
                    EntityId = data.EntityId,
                    ErrorSummary = GenerateBusinessErrorSummary(errors),
                    ImpactAssessment = AssessBusinessImpact(data, errors),
                    SuggestedActions = GenerateBusinessActions(errors),
                    Priority = DetermineBusinessPriority(stakeholder.Role, errors)
                };
                
                await _alertService.SendBusinessNotificationAsync(notification);
            }
        }
        catch (Exception ex)
        {
            await _technicalLogger.LogWarningAsync($"Failed to notify business stakeholders: {ex.Message}");
        }
    }
    
    // Métodos auxiliares específicos
    private bool IsCriticalTechnicalException(Exception ex)
    {
        return ex is OutOfMemoryException ||
               ex is StackOverflowException ||
               ex is AccessViolationException ||
               (ex is SqlException sqlEx && sqlEx.Severity >= 20);
    }
    
    private string DetermineSeverity(Exception ex)
    {
        return ex switch
        {
            OutOfMemoryException => "Critical",
            StackOverflowException => "Critical",
            AccessViolationException => "Critical",
            SecurityException => "High",
            DatabaseException => "High",
            NetworkException => "Medium",
            _ => "Low"
        };
    }
    
    private bool RequiresEscalation(Exception ex)
    {
        return IsCriticalTechnicalException(ex) || 
               ex is SecurityException ||
               (ex is DatabaseException dbEx && dbEx.IsConnectionFailure);
    }
    
    private string DetermineEscalationLevel(Exception ex)
    {
        return ex switch
        {
            OutOfMemoryException => "Immediate",
            SecurityException => "Immediate",
            DatabaseException dbEx when dbEx.IsConnectionFailure => "Urgent",
            NetworkException => "Normal",
            _ => "Normal"
        };
    }
    
    // Métodos de implementación simplificada
    private async Task<MlResult<InputData>> ValidateInputData(InputData data) => MlResult<InputData>.Valid(data);
    private async Task<MlResult<ProcessedLogic>> ProcessBusinessLogic(InputData data) => MlResult<ProcessedLogic>.Valid(new ProcessedLogic());
    private async Task<MlResult<SavedData>> SaveResults(ProcessedLogic logic) => MlResult<SavedData>.Valid(new SavedData());
    private string ExtractBusinessContext(InputData data) => "BusinessContext";
    private string CategorizeValidationError(string error) => "Category";
    private Dictionary<string, object> ExtractDataCharacteristics(InputData data) => new();
    private async Task<List<ValidationPattern>> GetRecentValidationPatternsAsync(string dataType) => new();
    private bool HasSignificantPattern(List<ValidationPattern> patterns) => false;
    private string GenerateImprovementSuggestion(List<ValidationPattern> patterns) => "Suggestion";
    private async Task<List<BusinessStakeholder>> GetRelevantStakeholdersAsync(string entityId, MlErrorsDetails errors) => new();
    private string GenerateBusinessErrorSummary(MlErrorsDetails errors) => "Summary";
    private string AssessBusinessImpact(InputData data, MlErrorsDetails errors) => "Impact";
    private List<string> GenerateBusinessActions(MlErrorsDetails errors) => new();
    private string DetermineBusinessPriority(string role, MlErrorsDetails errors) => "Medium";
    private async Task TriggerDataRecoveryMechanismsAsync(string operationId, InputData data) { }
    private async Task HandleDatabaseExceptionAsync(SqlException ex, string operationId, InputData data) { }
    private async Task HandlePersistenceTimeoutAsync(TimeoutException ex, string operationId, InputData data) { }
    private async Task HandleMemoryExceptionAsync(OutOfMemoryException ex, string operationId) { }
    private async Task HandleGenericPersistenceExceptionAsync(Exception ex, string operationId, InputData data) { }
    private async Task ActivateDataBackupProceduresAsync(string operationId, InputData data) { }
    private async Task UpdateSystemHealthMetricsAsync(Exception ex, TimeSpan duration) { }
    private async Task CheckDatabaseHealthAsync() { }
    private async Task TriggerSecurityResponseAsync(SecurityException ex, InputData data) { }
    private async Task TriggerPerformanceAnalysisAsync(PerformanceException ex, InputData data) { }
}

// Clases de apoyo específicas
public class InputData
{
    public string Id { get; set; }
    public string EntityId { get; set; }
    public int EstimatedSizeInBytes { get; set; }
}

public class ProcessedData
{
    public string Id { get; set; }
    public string Status { get; set; }
}

public class ProcessedLogic
{
    public string Result { get; set; }
}

public class SavedData
{
    public string Id { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ValidationFailureLog
{
    public string OperationId { get; set; }
    public string InputDataId { get; set; }
    public string[] ValidationErrors { get; set; }
    public DateTime Timestamp { get; set; }
    public string Severity { get; set; }
}

public class TechnicalErrorContext
{
    public string OperationId { get; set; }
    public string Operation { get; set; }
    public string InputDataType { get; set; }
    public int InputDataSize { get; set; }
    public string EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TechnicalIncident
{
    public string OperationId { get; set; }
    public Exception Exception { get; set; }
    public string Severity { get; set; }
    public string AffectedComponent { get; set; }
    public bool RequiresImmediateAttention { get; set; }
}

public class BusinessRuleViolationLog
{
    public string OperationId { get; set; }
    public string EntityId { get; set; }
    public string[] RuleViolations { get; set; }
    public string BusinessContext { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EscalationAlert
{
    public string OperationId { get; set; }
    public Exception Exception { get; set; }
    public string AffectedData { get; set; }
    public string EscalationLevel { get; set; }
    public bool RequiresOnCallResponse { get; set; }
}

public class DataPersistenceFailureLog
{
    public string OperationId { get; set; }
    public string EntityId { get; set; }
    public string[] PersistenceErrors { get; set; }
    public string AttemptedOperation { get; set; }
    public DateTime Timestamp { get; set; }
}

public class OperationCompletionLog
{
    public string OperationId { get; set; }
    public string Status { get; set; }
    public TimeSpan Duration { get; set; }
    public int ErrorCount { get; set; }
    public string[] FinalErrors { get; set; }
    public Exception Exception { get; set; }
    public string ExceptionType { get; set; }
}

public class ValidationPattern
{
    public string DataType { get; set; }
    public List<string> ErrorTypes { get; set; }
    public Dictionary<string, object> DataCharacteristics { get; set; }
    public DateTime Timestamp { get; set; }
}

public class BusinessImprovementSuggestion
{
    public string Area { get; set; }
    public string DataType { get; set; }
    public string SuggestedImprovement { get; set; }
    public string Priority { get; set; }
    public string EstimatedImpact { get; set; }
}

public class BusinessErrorNotification
{
    public string StakeholderId { get; set; }
    public string EntityId { get; set; }
    public string ErrorSummary { get; set; }
    public string ImpactAssessment { get; set; }
    public List<string> SuggestedActions { get; set; }
    public string Priority { get; set; }
}

public class BusinessStakeholder
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Name { get; set; }
}

// Excepciones personalizadas
public class NetworkException : Exception
{
    public NetworkException(string message) : base(message) { }
    public NetworkException(string message, Exception innerException) : base(message, innerException) { }
}

public class DatabaseException : Exception
{
    public bool IsConnectionFailure { get; set; }
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
    public SecurityException(string message, Exception innerException) : base(message, innerException) { }
}

public class PerformanceException : Exception
{
    public PerformanceException(string message) : base(message) { }
    public PerformanceException(string message, Exception innerException) : base(message, innerException) { }
}

// Interfaces de servicios especializados
public interface ITechnicalLogger
{
    Task LogCriticalErrorAsync(Exception ex, TechnicalErrorContext context);
    Task LogWarningAsync(string message);
    Task LogOperationCompletionAsync(OperationCompletionLog log);
}

public interface IBusinessLogger
{
    Task LogValidationFailureAsync(ValidationFailureLog log);
    Task LogBusinessRuleViolationAsync(BusinessRuleViolationLog log);
    Task LogDataPersistenceFailureAsync(DataPersistenceFailureLog log);
    Task LogOperationCompletionAsync(OperationCompletionLog log);
    Task LogValidationPatternAsync(ValidationPattern pattern);
}

public interface IAlertService
{
    Task SendEscalationAlertAsync(EscalationAlert alert);
    Task SendNetworkIssueAlertAsync(NetworkException ex, string operationId);
    Task SendDatabaseIssueAlertAsync(DatabaseException ex, string operationId);
    Task SendSecurityIncidentAlertAsync(SecurityException ex, string operationId, string entityId);
    Task SendPerformanceIssueAlertAsync(PerformanceException ex, string operationId);
    Task SendUnknownTechnicalIssueAlertAsync(Exception ex, string operationId);
    Task SendBusinessImprovementSuggestionAsync(BusinessImprovementSuggestion suggestion);
    Task SendBusinessNotificationAsync(BusinessErrorNotification notification);
}

public interface IMetricsService
{
    Task IncrementCounterAsync(string counterName);
}

public interface ITelemetryService
{
    Task RecordExceptionAsync(Exception ex);
    Task RecordExceptionAsync(Exception ex, Dictionary<string, string> properties);
}

public interface IIncidentService
{
    Task CreateAutomaticIncidentAsync(TechnicalIncident incident);
}
```

### Ejemplo 2: Sistema de Procesamiento de Archivos con Manejo Diferenciado

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _validator;
    private readonly IFileProcessor _processor;
    private readonly IFileStorage _storage;
    private readonly ITechnicalLogger _techLogger;
    private readonly IBusinessLogger _bizLogger;
    private readonly INotificationService _notifications;
    private readonly ISecurityService _security;
    
    public FileProcessingService(
        IFileValidator validator,
        IFileProcessor processor,
        IFileStorage storage,
        ITechnicalLogger techLogger,
        IBusinessLogger bizLogger,
        INotificationService notifications,
        ISecurityService security)
    {
        _validator = validator;
        _processor = processor;
        _storage = storage;
        _techLogger = techLogger;
        _bizLogger = bizLogger;
        _notifications = notifications;
        _security = security;
    }
    
    public async Task<MlResult<ProcessedFile>> ProcessFileWithDifferentiatedHandlingAsync(FileUpload upload)
    {
        var processingId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateFileUpload(upload)
            // Manejo de errores de validación de negocio (reglas de archivos)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _bizLogger.LogFileValidationFailureAsync(new FileValidationLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    ValidationErrors = errors.AllErrors,
                    FileSize = upload.SizeInBytes,
                    FileType = upload.ContentType,
                    Timestamp = DateTime.UtcNow
                });
                
                // Notificar al usuario sobre problemas de validación
                await _notifications.SendFileValidationErrorsAsync(upload.UserId, upload.FileName, errors.AllErrors);
                
                // Analizar patrones de archivos rechazados para mejorar UX
                await AnalyzeFileRejectionPatternsAsync(upload, errors);
            })
            
            // Manejo de errores técnicos en validación (excepciones del sistema)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _techLogger.LogFileValidationExceptionAsync(ex, new FileValidationExceptionContext
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    FilePath = upload.TempFilePath,
                    FileSize = upload.SizeInBytes,
                    Operation = "FileValidation"
                });
                
                // Manejo específico por tipo de excepción técnica
                switch (ex)
                {
                    case FileNotFoundException:
                        await HandleMissingFileExceptionAsync(ex, upload, processingId);
                        break;
                    case UnauthorizedAccessException:
                        await HandleFileAccessExceptionAsync(ex, upload, processingId);
                        break;
                    case IOException ioEx:
                        await HandleFileIOExceptionAsync(ioEx, upload, processingId);
                        break;
                    case OutOfMemoryException:
                        await HandleLargeFileExceptionAsync(ex, upload, processingId);
                        break;
                }
            },
            ex => $"Failed to handle file validation exception for {upload.FileName}: {ex.Message}")
            
            .BindAsync(async validFile => await ScanFileForSecurity(validFile))
            
            // Manejo de problemas de seguridad detectados (errores de negocio de seguridad)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _bizLogger.LogSecurityScanFailureAsync(new SecurityScanLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    SecurityIssues = errors.AllErrors,
                    ScanTimestamp = DateTime.UtcNow
                });
                
                // Notificar sobre problemas de seguridad encontrados
                await _notifications.SendSecurityScanFailureAsync(upload.UserId, upload.FileName, errors.AllErrors);
                
                // Registrar para análisis de seguridad
                await _security.LogSecurityScanResultAsync(upload, errors.AllErrors, "Failed");
            })
            
            // Manejo de errores técnicos en escaneo de seguridad
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _techLogger.LogSecurityScanExceptionAsync(ex, new SecurityScanExceptionContext
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    ScannerType = "AntiVirus",
                    Operation = "SecurityScan"
                });
                
                // Escalamiento inmediato para fallos en sistemas de seguridad
                await _security.EscalateSecuritySystemFailureAsync(new SecuritySystemFailure
                {
                    ProcessingId = processingId,
                    Exception = ex,
                    AffectedFile = upload.FileName,
                    UserId = upload.UserId,
                    FailureType = "ScannerMalfunction",
                    RequiresImmediateAction = true
                });
            },
            ex => $"Failed to handle security scan exception for {upload.FileName}: {ex.Message}")
            
            .BindAsync(async scannedFile => await ProcessFileContent(scannedFile))
            
            // Manejo de errores de procesamiento de contenido (errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _bizLogger.LogContentProcessingFailureAsync(new ContentProcessingLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    ContentType = upload.ContentType,
                    ProcessingErrors = errors.AllErrors,
                    ProcessingStage = "ContentExtraction",
                    Timestamp = DateTime.UtcNow
                });
                
                // Notificar sobre problemas de formato o contenido
                await _notifications.SendContentProcessingFailureAsync(
                    upload.UserId, 
                    upload.FileName,
                    GenerateUserFriendlyContentErrors(errors.AllErrors));
                
                // Sugerir formatos alternativos o acciones correctivas
                await SuggestContentCorrectionActionsAsync(upload, errors);
            })
            
            // Manejo de errores técnicos en procesamiento de contenido
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _techLogger.LogContentProcessingExceptionAsync(ex, new ContentProcessingExceptionContext
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    ContentType = upload.ContentType,
                    FileSize = upload.SizeInBytes,
                    ProcessingStage = DetermineProcessingStage(ex)
                });
                
                // Manejo específico de excepciones de procesamiento
                await HandleContentProcessingExceptionAsync(ex, upload, processingId);
                
                // Crear tarea de revisión manual para archivos complejos
                if (IsComplexFileProcessingException(ex))
                {
                    await CreateManualReviewTaskAsync(upload, ex, processingId);
                }
            },
            ex => $"Failed to handle content processing exception for {upload.FileName}: {ex.Message}")
            
            .BindAsync(async processedContent => await SaveProcessedFile(processedContent))
            
            // Manejo de errores de almacenamiento (errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _bizLogger.LogStorageFailureAsync(new StorageFailureLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    StorageErrors = errors.AllErrors,
                    AttemptedLocation = DetermineStorageLocation(upload),
                    Timestamp = DateTime.UtcNow
                });
                
                // Intentar localizaciones de almacenamiento alternativas
                await TryAlternativeStorageLocationsAsync(upload, processingId);
                
                // Notificar sobre problemas de capacidad o permisos
                await _notifications.SendStorageIssueNotificationAsync(upload.UserId, upload.FileName);
            })
            
            // Manejo de errores técnicos de almacenamiento
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _techLogger.LogStorageExceptionAsync(ex, new StorageExceptionContext
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    StorageType = "PrimaryStorage",
                    Operation = "FileSave"
                });
                
                // Manejo específico de excepciones de almacenamiento
                switch (ex)
                {
                    case DirectoryNotFoundException:
                        await HandleMissingDirectoryExceptionAsync(ex, upload, processingId);
                        break;
                    case UnauthorizedAccessException:
                        await HandleStoragePermissionExceptionAsync(ex, upload, processingId);
                        break;
                    case IOException ioEx when ioEx.Message.Contains("disk"):
                        await HandleDiskSpaceExceptionAsync(ioEx, upload, processingId);
                        break;
                    default:
                        await HandleGenericStorageExceptionAsync(ex, upload, processingId);
                        break;
                }
                
                // Activar procedimientos de backup de emergencia
                await ActivateEmergencyBackupProceduresAsync(upload, processingId);
            },
            ex => $"Failed to handle storage exception for {upload.FileName}: {ex.Message}")
            
            // Logging final diferenciado por tipo de error
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _bizLogger.LogFileProcessingCompletionAsync(new FileProcessingCompletionLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    Status = "BusinessFailure",
                    Duration = duration,
                    FinalBusinessErrors = errors.AllErrors,
                    FileSize = upload.SizeInBytes
                });
                
                // Actualizar estadísticas de archivos procesados por tipo de error de negocio
                await UpdateBusinessFailureStatisticsAsync(upload, errors, duration);
            })
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _techLogger.LogFileProcessingCompletionAsync(new FileProcessingCompletionLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    Status = "TechnicalFailure",
                    Duration = duration,
                    FinalException = ex,
                    ExceptionType = ex.GetType().Name,
                    FileSize = upload.SizeInBytes
                });
                
                // Actualizar métricas de salud del sistema de archivos
                await UpdateSystemHealthMetricsAsync(ex, upload, duration);
                
                // Trigger análisis de fallos técnicos recurrentes
                await TriggerRecurringFailureAnalysisAsync(ex, upload.ContentType);
            },
            ex => $"Failed to log final technical error for file {upload.FileName}: {ex.Message}");
    }
    
    // Métodos auxiliares específicos para manejo de excepciones
    private async Task HandleMissingFileExceptionAsync(Exception ex, FileUpload upload, string processingId)
    {
        await _techLogger.LogCriticalAsync($"File disappeared during processing: {upload.TempFilePath}");
        await _notifications.SendSystemErrorNotificationAsync(upload.UserId, 
            "File processing failed due to system issue. Our team has been notified.");
    }
    
    private async Task HandleFileAccessExceptionAsync(Exception ex, FileUpload upload, string processingId)
    {
        await _security.LogSecurityIncidentAsync(new SecurityIncident
        {
            Type = "UnauthorizedFileAccess",
            UserId = upload.UserId,
            FileName = upload.FileName,
            AttemptedOperation = "FileValidation",
            Exception = ex
        });
    }
    
    private async Task HandleFileIOExceptionAsync(IOException ex, FileUpload upload, string processingId)
    {
        if (ex.Message.Contains("corrupted"))
        {
            await _notifications.SendCorruptedFileNotificationAsync(upload.UserId, upload.FileName);
        }
        else
        {
            await _techLogger.LogSystemIssueAsync($"IO error processing file {upload.FileName}: {ex.Message}");
        }
    }
    
    private async Task HandleLargeFileExceptionAsync(Exception ex, FileUpload upload, string processingId)
    {
        await _techLogger.LogResourceExhaustionAsync(new ResourceExhaustionEvent
        {
            ResourceType = "Memory",
            Operation = "FileValidation",
            FileSize = upload.SizeInBytes,
            FileName = upload.FileName,
            Exception = ex
        });
        
        // Trigger escalamiento para problemas de recursos
        await _notifications.SendResourceExhaustionAlertAsync("Memory", upload.SizeInBytes);
    }
    
    private async Task HandleContentProcessingExceptionAsync(Exception ex, FileUpload upload, string processingId)
    {
        switch (ex)
        {
            case InvalidDataException:
                await _notifications.SendInvalidContentNotificationAsync(upload.UserId, upload.FileName);
                break;
            case NotSupportedException:
                await _notifications.SendUnsupportedFormatNotificationAsync(upload.UserId, upload.FileName, upload.ContentType);
                break;
            case OutOfMemoryException:
                await TriggerLargeFileProcessingProtocolAsync(upload, processingId);
                break;
        }
    }
    
    private bool IsComplexFileProcessingException(Exception ex)
    {
        return ex is InvalidDataException ||
               ex is NotSupportedException ||
               (ex is ArgumentException argEx && argEx.Message.Contains("format"));
    }
    
    private async Task CreateManualReviewTaskAsync(FileUpload upload, Exception ex, string processingId)
    {
        var reviewTask = new ManualFileReviewTask
        {
            ProcessingId = processingId,
            FileName = upload.FileName,
            UserId = upload.UserId,
            FileSize = upload.SizeInBytes,
            ContentType = upload.ContentType,
            Exception = ex.GetType().Name,
            ExceptionMessage = ex.Message,
            RequiresExpertReview = true,
            Priority = DetermineReviewPriority(upload, ex),
            CreatedAt = DateTime.UtcNow
        };
        
        await _storage.CreateReviewTaskAsync(reviewTask);
    }
    
    // Métodos auxiliares simplificados
    private async Task<MlResult<FileUpload>> ValidateFileUpload(FileUpload upload) => MlResult<FileUpload>.Valid(upload);
    private async Task<MlResult<ScannedFile>> ScanFileForSecurity(FileUpload upload) => MlResult<ScannedFile>.Valid(new ScannedFile());
    private async Task<MlResult<ProcessedContent>> ProcessFileContent(ScannedFile file) => MlResult<ProcessedContent>.Valid(new ProcessedContent());
    private async Task<MlResult<ProcessedFile>> SaveProcessedFile(ProcessedContent content) => MlResult<ProcessedFile>.Valid(new ProcessedFile());
    private async Task AnalyzeFileRejectionPatternsAsync(FileUpload upload, MlErrorsDetails errors) { }
    private List<string> GenerateUserFriendlyContentErrors(string[] errors) => errors.ToList();
    private async Task SuggestContentCorrectionActionsAsync(FileUpload upload, MlErrorsDetails errors) { }
    private string DetermineProcessingStage(Exception ex) => "Unknown";
    private string DetermineStorageLocation(FileUpload upload) => "Primary";
    private async Task TryAlternativeStorageLocationsAsync(FileUpload upload, string processingId) { }
    private async Task HandleMissingDirectoryExceptionAsync(Exception ex, FileUpload upload, string processingId) { }
    private async Task HandleStoragePermissionExceptionAsync(Exception ex, FileUpload upload, string processingId) { }
    private async Task HandleDiskSpaceExceptionAsync(IOException ex, FileUpload upload, string processingId) { }
    private async Task HandleGenericStorageExceptionAsync(Exception ex, FileUpload upload, string processingId) { }
    private async Task ActivateEmergencyBackupProceduresAsync(FileUpload upload, string processingId) { }
    private async Task UpdateBusinessFailureStatisticsAsync(FileUpload upload, MlErrorsDetails errors, TimeSpan duration) { }
    private async Task UpdateSystemHealthMetricsAsync(Exception ex, FileUpload upload, TimeSpan duration) { }
    private async Task TriggerRecurringFailureAnalysisAsync(Exception ex, string contentType) { }
    private async Task TriggerLargeFileProcessingProtocolAsync(FileUpload upload, string processingId) { }
    private string DetermineReviewPriority(FileUpload upload, Exception ex) => "Medium";
}

// Clases de apoyo adicionales
public class FileUpload
{
    public string FileName { get; set; }
    public string TempFilePath { get; set; }
    public long SizeInBytes { get; set; }
    public string ContentType { get; set; }
    public int UserId { get; set; }
}

public class ScannedFile
{
    public FileUpload Upload { get; set; }
    public bool IsClean { get; set; }
}

public class ProcessedContent
{
    public ScannedFile ScannedFile { get; set; }
    public string ExtractedContent { get; set; }
}

public class ProcessedFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Status { get; set; }
}

public class FileValidationLog
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public int UserId { get; set; }
    public string[] ValidationErrors { get; set; }
    public long FileSize { get; set; }
    public string FileType { get; set; }
    public DateTime Timestamp { get; set; }
}

public class FileValidationExceptionContext
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public string Operation { get; set; }
}

public class SecurityScanLog
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public int UserId { get; set; }
    public string[] SecurityIssues { get; set; }
    public DateTime ScanTimestamp { get; set; }
}

public class SecurityScanExceptionContext
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public string ScannerType { get; set; }
    public string Operation { get; set; }
}

public class SecuritySystemFailure
{
    public string ProcessingId { get; set; }
    public Exception Exception { get; set; }
    public string AffectedFile { get; set; }
    public int UserId { get; set; }
    public string FailureType { get; set; }
    public bool RequiresImmediateAction { get; set; }
}

public class ContentProcessingLog
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string[] ProcessingErrors { get; set; }
    public string ProcessingStage { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ContentProcessingExceptionContext
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long FileSize { get; set; }
    public string ProcessingStage { get; set; }
}

public class StorageFailureLog
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public int UserId { get; set; }
    public string[] StorageErrors { get; set; }
    public string AttemptedLocation { get; set; }
    public DateTime Timestamp { get; set; }
}

public class StorageExceptionContext
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public string StorageType { get; set; }
    public string Operation { get; set; }
}

public class FileProcessingCompletionLog
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public TimeSpan Duration { get; set; }
    public string[] FinalBusinessErrors { get; set; }
    public Exception FinalException { get; set; }
    public string ExceptionType { get; set; }
    public long FileSize { get; set; }
}

public class SecurityIncident
{
    public string Type { get; set; }
    public int UserId { get; set; }
    public string FileName { get; set; }
    public string AttemptedOperation { get; set; }
    public Exception Exception { get; set; }
}

public class ResourceExhaustionEvent
{
    public string ResourceType { get; set; }
    public string Operation { get; set; }
    public long FileSize { get; set; }
    public string FileName { get; set; }
    public Exception Exception { get; set; }
}

public class ManualFileReviewTask
{
    public string ProcessingId { get; set; }
    public string FileName { get; set; }
    public int UserId { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public string Exception { get; set; }
    public string ExceptionMessage { get; set; }
    public bool RequiresExpertReview { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Interfaces de servicios específicos para archivos
public interface IFileValidator
{
    Task<bool> ValidateAsync(FileUpload upload);
}

public interface IFileProcessor
{
    Task<string> ExtractContentAsync(ScannedFile file);
}

public interface IFileStorage
{
    Task SaveFileAsync(ProcessedContent content);
    Task CreateReviewTaskAsync(ManualFileReviewTask task);
}

public interface ISecurityService
{
    Task LogSecurityScanResultAsync(FileUpload upload, string[] issues, string result);
    Task EscalateSecuritySystemFailureAsync(SecuritySystemFailure failure);
    Task LogSecurityIncident// filepath: c:\PakkkoTFS\MoralesLarios\FOOP\MoralesLarios.FOOP\docs\MlResultActionsExecSelfIfFailWithException.md
# MlResultActionsExecSelfIfFailWithException - Manejo de Excepciones Específicas

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFailWithException](#métodos-execselfiffailwithexception)
4. [Métodos ExecSelfIfFailWithoutException](#métodos-execselfiffailwithoutexception)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Métodos TryExecSelfIfFailWithException - Captura de Excepciones](#métodos-tryexecselfiffailwithexception---captura-de-excepciones)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)
9. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos `ExecSelfIfFailWithException` y `ExecSelfIfFailWithoutException` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido** y basándose en si el fallo **contiene o no contiene una excepción preservada** en los detalles del error.

### Propósito Principal

**ExecSelfIfFailWithException**:
- **Manejo Específico de Excepciones**: Acceder a la excepción original que causó el fallo
- **Logging Detallado**: Registrar stack traces y detalles técnicos de excepciones
- **Análisis de Errores**: Analizar tipos específicos de excepciones para patrones
- **Alertas Técnicas**: Enviar notificaciones específicas para ciertos tipos de excepciones
- **Debugging Avanzado**: Acceso completo a la excepción para diagnóstico

**ExecSelfIfFailWithoutException**:
- **Manejo de Errores de Negocio**: Procesar fallos que no son causados por excepciones técnicas
- **Validación y Reglas**: Manejar errores de validación y reglas de negocio
- **Logging de Negocio**: Registrar errores funcionales sin detalles técnicos
- **Flujos Alternativos**: Ejecutar lógica cuando el fallo es "esperado" y no técnico

---

## Análisis de los Métodos

### Estructura y Filosofía

Estos métodos implementan el patrón de **manejo de errores diferenciado por causa**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido con Excepción → ExecSelfIfFailWithException → Acción(excepción)
      ↓                          ↓
Resultado Fallido sin Excepción → ExecSelfIfFailWithoutException → Acción(errores)
```

### Características Principales

1. **Diferenciación por Causa**: Distintas acciones según si el fallo tiene excepción o no
2. **Extracción de Excepciones**: Utiliza `GetDetailException()` para extraer excepciones preservadas
3. **Inmutabilidad**: El resultado original nunca se modifica
4. **Manejo Específico**: Acciones especializadas para errores técnicos vs errores de negocio
5. **Complementariedad**: Ambos métodos pueden usarse en conjunto para cobertura completa

---

## Métodos ExecSelfIfFailWithException

### `ExecSelfIfFailWithException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido y contiene una excepción preservada

```csharp
public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                         Action<Exception> actionFailException)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFailException`: Acción a ejecutar solo si `source` es fallido y contiene una excepción

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido pero no contiene excepción: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido y contiene excepción: Extrae la excepción, ejecuta `actionFailException(exception)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ProcessData(inputData)
    .ExecSelfIfFailWithException(ex => 
    {
        _logger.LogError(ex, "Technical error occurred during data processing");
        _telemetry.RecordException(ex);
        
        // Manejo específico por tipo de excepción
        switch (ex)
        {
            case SqlException sqlEx:
                _alertService.SendDatabaseAlert(sqlEx);
                break;
            case TimeoutException timeoutEx:
                _alertService.SendTimeoutAlert(timeoutEx);
                break;
            case OutOfMemoryException memEx:
                _alertService.SendCriticalResourceAlert(memEx);
                break;
        }
    });
```

**Ejemplo con Análisis de Excepciones**:
```csharp
var result = CallExternalAPI(request)
    .ExecSelfIfFailWithException(ex => 
    {
        // Logging estructurado con detalles de la excepción
        _logger.LogError(ex, "External API call failed", new 
        {
            ExceptionType = ex.GetType().Name,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            InnerException = ex.InnerException?.GetType().Name,
            RequestId = request.Id
        });
        
        // Análisis de patrones de excepción
        AnalyzeExceptionPattern(ex, request);
        
        // Métricas específicas por tipo de excepción
        _metrics.IncrementCounter($"api.exceptions.{ex.GetType().Name}");
    });
```

---

## Métodos ExecSelfIfFailWithoutException

### `ExecSelfIfFailWithoutException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido pero NO contiene una excepción preservada

```csharp
public static MlResult<T> ExecSelfIfFailWithoutException<T>(this MlResult<T> source,
                                                            Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFail`: Acción a ejecutar solo si `source` es fallido y NO contiene una excepción

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFail`
- Si `source` es fallido y contiene excepción: Retorna `source` sin cambios, no ejecuta `actionFail`
- Si `source` es fallido y NO contiene excepción: Ejecuta `actionFail(errorsDetails)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ValidateUser(userData)
    .ExecSelfIfFailWithoutException(errors => 
    {
        // Este es un error de validación/negocio, no técnico
        _logger.LogWarning($"User validation failed: {string.Join(", ", errors.AllErrors)}");
        _metrics.IncrementCounter("user.validation_failures");
        
        // Análisis de patrones de validación
        AnalyzeValidationFailures(errors);
        
        // Notificación al usuario (no es un error técnico)
        _notificationService.SendValidationErrors(userData.Email, errors.AllErrors);
    });
```

**Ejemplo con Reglas de Negocio**:
```csharp
var result = ProcessBusinessRule(businessData)
    .ExecSelfIfFailWithoutException(errors => 
    {
        // Errores de reglas de negocio - esperados y manejables
        _businessLogger.LogRuleViolation(businessData.EntityId, errors.AllErrors);
        
        // Métricas de reglas de negocio
        foreach (var error in errors.AllErrors)
        {
            var ruleType = ExtractRuleType(error);
            _metrics.IncrementCounter($"business.rules.violations.{ruleType}");
        }
        
        // Notificar a stakeholders sobre violaciones de reglas
        _businessNotifications.SendRuleViolationAlert(businessData, errors);
    });
```

---

## Variantes Asíncronas

### `ExecSelfIfFailWithExceptionAsync<T>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync)
```

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .ExecSelfIfFailWithExceptionAsync(async ex => 
    {
        await _logger.LogErrorAsync(ex, "Document processing failed with technical error");
        await _telemetryService.RecordExceptionAsync(ex);
        
        // Envío asíncrono de alertas basado en tipo de excepción
        if (ex is FileNotFoundException)
        {
            await _alertService.SendFileNotFoundAlertAsync(document.FilePath);
        }
        else if (ex is UnauthorizedAccessException)
        {
            await _securityService.LogSecurityIncidentAsync(ex, document.UserId);
        }
    });
```

### `ExecSelfIfFailWithoutExceptionAsync<T>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task> actionFailExceptionAsync)
```

**Ejemplo**:
```csharp
var result = await ValidateOrderAsync(order)
    .ExecSelfIfFailWithoutExceptionAsync(async errors => 
    {
        await _businessLogger.LogValidationFailureAsync(order.Id, errors.AllErrors);
        
        // Análisis asíncrono de patrones de validación
        await AnalyzeValidationPatternsAsync(order.CustomerId, errors);
        
        // Envío de notificaciones de negocio
        await _customerService.SendValidationFailureNotificationAsync(
            order.CustomerId, 
            order.Id, 
            errors.AllErrors);
    });
```

### Versiones con Fuente Asíncrona

```csharp
// ExecSelfIfFailWithException con fuente asíncrona
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException)

// ExecSelfIfFailWithoutException con fuente asíncrona
public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithoutExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<MlErrorsDetails> actionFail)
```

---

## Métodos TryExecSelfIfFailWithException - Captura de Excepciones

### `TryExecSelfIfFailWithException<T>()` - Versión Segura

```csharp
public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            string errorMessage = null!)
```

**Comportamiento Especial**: 
- Si la acción lanza una excepción, **las versiones `Try*` agregan el error de la excepción a los errores originales**
- El resultado final contiene tanto la excepción original como cualquier error de la acción

**Ejemplo**:
```csharp
var result = ProcessCriticalOperation(data)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Esta acción puede fallar
            SendCriticalAlert(originalException); // Puede lanzar NetworkException
            LogToExternalSystem(originalException); // Puede lanzar ServiceUnavailableException
            UpdateDashboard(originalException); // Puede lanzar TimeoutException
        },
        ex => $"Failed to handle technical error notification: {ex.Message}"
    );

// Si ProcessCriticalOperation falla con una excepción: result contiene esa excepción + detalles
// Si además las acciones de manejo lanzan excepciones: result contiene la excepción original + errores de las acciones
```

### Versiones Asíncronas de TryExecSelfIfFailWithException

```csharp
// Todas las variantes asíncronas seguras
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException,
    Func<Exception, string> errorMessageBuilder)
```

**Nota sobre ExecSelfIfFailWithoutException**: No hay versiones `Try*` para `ExecSelfIfFailWithoutException` en el código mostrado, posiblemente porque los errores de negocio (sin excepciones) típicamente no requieren manejo adicional de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Monitoreo Diferenciado por Tipo de Error

```csharp
public class DifferentiatedErrorMonitoringService
{
    private readonly ITechnicalLogger _technicalLogger;
    private readonly IBusinessLogger _businessLogger;
    private readonly IAlertService _alertService;
    private readonly IMetricsService _metrics;
    private readonly ITelemetryService _telemetry;
    private readonly IIncidentService _incidentService;
    
    public DifferentiatedErrorMonitoringService(
        ITechnicalLogger technicalLogger,
        IBusinessLogger businessLogger,
        IAlertService alertService,
        IMetricsService metrics,
        ITelemetryService telemetry,
        IIncidentService incidentService)
    {
        _technicalLogger = technicalLogger;
        _businessLogger = businessLogger;
        _alertService = alertService;
        _metrics = metrics;
        _telemetry = telemetry;
        _incidentService = incidentService;
    }
    
    public async Task<MlResult<ProcessedData>> ProcessWithDifferentiatedMonitoringAsync(InputData data)
    {
        var operationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateInputData(data)
            // Manejo de errores de validación (sin excepción - errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogValidationFailureAsync(new ValidationFailureLog
                {
                    OperationId = operationId,
                    InputDataId = data.Id,
                    ValidationErrors = errors.AllErrors,
                    Timestamp = DateTime.UtcNow,
                    Severity = "Warning"
                });
                
                await _metrics.IncrementCounterAsync("business.validation_failures");
                
                // Análisis de patrones de validación para mejora del negocio
                await AnalyzeValidationPatternsForBusinessImprovementAsync(data, errors);
            })
            // Manejo de errores técnicos de validación (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "InputValidation",
                    InputDataType = data.GetType().Name,
                    InputDataSize = data.EstimatedSizeInBytes,
                    Timestamp = DateTime.UtcNow
                });
                
                await _telemetry.RecordExceptionAsync(ex, new Dictionary<string, string>
                {
                    ["operation_id"] = operationId,
                    ["operation_type"] = "validation",
                    ["data_type"] = data.GetType().Name
                });
                
                // Crear incidente automático para errores técnicos críticos
                if (IsCriticalTechnicalException(ex))
                {
                    await _incidentService.CreateAutomaticIncidentAsync(new TechnicalIncident
                    {
                        OperationId = operationId,
                        Exception = ex,
                        Severity = DetermineSeverity(ex),
                        AffectedComponent = "DataValidation",
                        RequiresImmediateAttention = true
                    });
                }
            },
            ex => $"Failed to handle technical validation error for operation {operationId}: {ex.Message}")
            
            .BindAsync(async validData => await ProcessBusinessLogic(validData))
            
            // Manejo de errores de lógica de negocio (sin excepción)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogBusinessRuleViolationAsync(new BusinessRuleViolationLog
                {
                    OperationId = operationId,
                    EntityId = data.EntityId,
                    RuleViolations = errors.AllErrors,
                    BusinessContext = ExtractBusinessContext(data),
                    Timestamp = DateTime.UtcNow
                });
                
                await _metrics.IncrementCounterAsync("business.rule_violations");
                
                // Notificar a stakeholders de negocio sobre violaciones
                await NotifyBusinessStakeholdersAsync(data, errors);
            })
            
            // Manejo de errores técnicos en lógica de negocio (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "BusinessLogicProcessing",
                    EntityId = data.EntityId,
                    Timestamp = DateTime.UtcNow
                });
                
                // Análisis específico por tipo de excepción técnica
                await HandleSpecificTechnicalExceptionAsync(ex, operationId, data);
                
                // Escalamiento automático para excepciones críticas
                if (RequiresEscalation(ex))
                {
                    await _alertService.SendEscalationAlertAsync(new EscalationAlert
                    {
                        OperationId = operationId,
                        Exception = ex,
                        AffectedData = data.Id,
                        EscalationLevel = DetermineEscalationLevel(ex),
                        RequiresOnCallResponse = true
                    });
                }
            },
            ex => $"Failed to handle technical business logic error for operation {operationId}: {ex.Message}")
            
            .BindAsync(async processedLogic => await SaveResults(processedLogic))
            
            // Manejo de errores de persistencia (sin excepción - posibles errores de negocio)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _businessLogger.LogDataPersistenceFailureAsync(new DataPersistenceFailureLog
                {
                    OperationId = operationId,
                    EntityId = data.EntityId,
                    PersistenceErrors = errors.AllErrors,
                    AttemptedOperation = "Save",
                    Timestamp = DateTime.UtcNow
                });
                
                // Intentar mecanismos de recuperación de datos
                await TriggerDataRecoveryMechanismsAsync(operationId, data);
            })
            
            // Manejo de errores técnicos de persistencia (con excepción)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _technicalLogger.LogCriticalErrorAsync(ex, new TechnicalErrorContext
                {
                    OperationId = operationId,
                    Operation = "DataPersistence",
                    EntityId = data.EntityId,
                    Timestamp = DateTime.UtcNow
                });
                
                // Manejo específico de excepciones de persistencia
                switch (ex)
                {
                    case SqlException sqlEx:
                        await HandleDatabaseExceptionAsync(sqlEx, operationId, data);
                        break;
                    case TimeoutException timeoutEx:
                        await HandlePersistenceTimeoutAsync(timeoutEx, operationId, data);
                        break;
                    case OutOfMemoryException memEx:
                        await HandleMemoryExceptionAsync(memEx, operationId);
                        break;
                    default:
                        await HandleGenericPersistenceExceptionAsync(ex, operationId, data);
                        break;
                }
                
                // Activar procedimientos de backup y recuperación
                await ActivateDataBackupProceduresAsync(operationId, data);
            },
            ex => $"Failed to handle technical persistence error for operation {operationId}: {ex.Message}")
            
            // Logging final diferenciado
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _businessLogger.LogOperationCompletionAsync(new OperationCompletionLog
                {
                    OperationId = operationId,
                    Status = "BusinessFailure",
                    Duration = duration,
                    ErrorCount = errors.AllErrors.Length,
                    FinalErrors = errors.AllErrors
                });
            })
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                var duration = DateTime.UtcNow - startTime;
                await _technicalLogger.LogOperationCompletionAsync(new OperationCompletionLog
                {
                    OperationId = operationId,
                    Status = "TechnicalFailure",
                    Duration = duration,
                    Exception = ex,
                    ExceptionType = ex.GetType().Name
                });
                
                // Actualizar métricas de salud del sistema
                await UpdateSystemHealthMetricsAsync(ex, duration);
            },
            ex => $"Failed to log final technical error for operation {operationId}: {ex.Message}");
    }
    
    private async Task AnalyzeValidationPatternsForBusinessImprovementAsync(InputData data, MlErrorsDetails errors)
    {
        try
        {
            var pattern = new ValidationPattern
            {
                DataType = data.GetType().Name,
                ErrorTypes = errors.AllErrors.Select(e => CategorizeValidationError(e)).ToList(),
                DataCharacteristics = ExtractDataCharacteristics(data),
                Timestamp = DateTime.UtcNow
            };
            
            await _businessLogger.LogValidationPatternAsync(pattern);
            
            // Detectar tendencias que pueden indicar necesidad de mejoras en el negocio
            var recentPatterns = await GetRecentValidationPatternsAsync(data.GetType().Name);
            if (recentPatterns.Count > 10 && HasSignificantPattern(recentPatterns))
            {
                await _alertService.SendBusinessImprovementSuggestionAsync(new BusinessImprovementSuggestion
                {
                    Area = "DataValidation",
                    DataType = data.GetType().Name,
                    SuggestedImprovement = GenerateImprovementSuggestion(recentPatterns),
                    Priority = "Medium",
                    EstimatedImpact = "ReduceValidationFailures"
                });
            }
        }
        catch (Exception ex)
        {
            await _technicalLogger.LogWarningAsync($"Failed to analyze validation patterns: {ex.Message}");
        }
    }
    
    private async Task HandleSpecificTechnicalExceptionAsync(Exception ex, string operationId, InputData data)
    {
        switch (ex)
        {
            case NetworkException netEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.network");
                await _alertService.SendNetworkIssueAlertAsync(netEx, operationId);
                break;
                
            case DatabaseException dbEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.database");
                await _alertService.SendDatabaseIssueAlertAsync(dbEx, operationId);
                await CheckDatabaseHealthAsync();
                break;
                
            case SecurityException secEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.security");
                await _alertService.SendSecurityIncidentAlertAsync(secEx, operationId, data.EntityId);
                await TriggerSecurityResponseAsync(secEx, data);
                break;
                
            case PerformanceException perfEx:
                await _metrics.IncrementCounterAsync("technical.exceptions.performance");
                await _alertService.SendPerformanceIssueAlertAsync(perfEx, operationId);
                await TriggerPerformanceAnalysisAsync(perfEx, data);
                break;
                
            default:
                await _metrics.IncrementCounterAsync("technical.exceptions.unknown");
                await _alertService.SendUnknownTechnicalIssueAlertAsync(ex, operationId);
                break;
        }
    }
    
    private async Task NotifyBusinessStakeholdersAsync(InputData data, MlErrorsDetails errors)
    {
        try
        {
            var stakeholders = await GetRelevantStakeholdersAsync(data.EntityId, errors);
            
            foreach (var stakeholder in stakeholders)
            {
                var notification = new BusinessErrorNotification
                {
                    StakeholderId = stakeholder.Id,
                    EntityId = data.EntityId,
                    ErrorSummary = GenerateBusinessErrorSummary(errors),
                    ImpactAssessment = AssessBusinessImpact(data, errors),
                    SuggestedActions = GenerateBusinessActions(errors),
                    Priority = DetermineBusinessPriority(stakeholder.Role, errors)
                };
                
                await _alertService.SendBusinessNotificationAsync(notification);
            }
        }
        catch (Exception ex)
        {
            await _technicalLogger.LogWarningAsync($"Failed to notify business stakeholders: {ex.Message}");
        }
    }
    
    // Métodos auxiliares específicos
    private bool IsCriticalTechnicalException(Exception ex)
    {
        return ex is OutOfMemoryException ||
               ex is StackOverflowException ||
               ex is AccessViolationException ||
               (ex is SqlException sqlEx && sqlEx.Severity >= 20);
    }
    
    private string DetermineSeverity(Exception ex)
    {
        return ex switch
        {
            OutOfMemoryException => "Critical",
            StackOverflowException => "Critical",
            AccessViolationException => "Critical",
            SecurityException => "High",
            DatabaseException => "High",
            NetworkException => "Medium",
            _ => "Low"
        };
    }
    
    private bool RequiresEscalation(Exception ex)
    {
        return IsCriticalTechnicalException(ex) || 
               ex is SecurityException ||
               (ex is DatabaseException dbEx && dbEx.IsConnectionFailure);
    }
    
    private string DetermineEscalationLevel(Exception ex)
    {
        return ex switch
        {
            OutOfMemoryException => "Immediate",
            SecurityException => "Immediate",
            DatabaseException dbEx when dbEx.IsConnectionFailure => "Urgent",
            NetworkException => "Normal",
            _ => "Normal"
        };
    }
    
    // Métodos de implementación simplificada
    private async Task<MlResult<InputData>> ValidateInputData(InputData data) => MlResult<InputData>.Valid(data);
    private async Task<MlResult<ProcessedLogic>> ProcessBusinessLogic(InputData data) => MlResult<ProcessedLogic>.Valid(new ProcessedLogic());
    private async Task<MlResult<SavedData>> SaveResults(ProcessedLogic logic) => MlResult<SavedData>.Valid(new SavedData());
    private string ExtractBusinessContext(InputData data) => "BusinessContext";
    private string CategorizeValidationError(string error) => "Category";
    private Dictionary<string, object> ExtractDataCharacteristics(InputData data) => new();
    private async Task<List<ValidationPattern>> GetRecentValidationPatternsAsync(string dataType) => new();
    private bool HasSignificantPattern(List<ValidationPattern> patterns) => false;
    private string GenerateImprovementSuggestion(List<ValidationPattern> patterns) => "Suggestion";
    private async Task<List<BusinessStakeholder>> GetRelevantStakeholdersAsync(string entityId, MlErrorsDetails errors) => new();
    private string GenerateBusinessErrorSummary(MlErrorsDetails errors) => "Summary";
    private string AssessBusinessImpact(InputData data, MlErrorsDetails errors) => "Impact";
    private List<string> GenerateBusinessActions(MlErrorsDetails errors) => new();
    private string DetermineBusinessPriority(string role, MlErrorsDetails errors) => "Medium";
    private async Task TriggerDataRecoveryMechanismsAsync(string operationId, InputData data) { }
    private async Task HandleDatabaseExceptionAsync(SqlException ex, string operationId, InputData data) { }
    private async Task HandlePersistenceTimeoutAsync(TimeoutException ex, string operationId, InputData data) { }
    private async Task HandleMemoryExceptionAsync(OutOfMemoryException ex, string operationId) { }
    private async Task HandleGenericPersistenceExceptionAsync(Exception ex, string operationId, InputData data) { }
    private async Task ActivateDataBackupProceduresAsync(string operationId, InputData data) { }
    private async Task UpdateSystemHealthMetricsAsync(Exception ex, TimeSpan duration) { }
    private async Task CheckDatabaseHealthAsync() { }
    private async Task TriggerSecurityResponseAsync(SecurityException ex, InputData data) { }
    private async Task TriggerPerformanceAnalysisAsync(PerformanceException ex, InputData data) { }
}

// Clases de apoyo específicas
public class InputData
{
    public string Id { get; set; }
    public string EntityId { get; set; }
    public int EstimatedSizeInBytes { get; set; }
}

public class ProcessedData
{
    public string Id { get; set; }
    public string Status { get; set; }
}

public class ProcessedLogic
{
    public string Result { get; set; }
}

public class SavedData
{
    public string Id { get; set; }
    public DateTime SavedAt { get; set; }
}

public class ValidationFailureLog
{
    public string OperationId { get; set; }
    public string InputDataId { get; set; }
    public string[] ValidationErrors { get; set; }
    public DateTime Timestamp { get; set; }
    public string Severity { get; set; }
}

public class TechnicalErrorContext
{
    public string OperationId { get; set; }
    public string Operation { get; set; }
    public string InputDataType { get; set; }
    public int InputDataSize { get; set; }
    public string EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}

public class TechnicalIncident
{
    public string OperationId { get; set; }
    public Exception Exception { get; set; }
    public string Severity { get; set; }
    public string AffectedComponent { get; set; }
    public bool RequiresImmediateAttention { get; set; }
}

public class BusinessRuleViolationLog
{
    public string OperationId { get; set; }
    public string EntityId { get; set; }
    public string[] RuleViolations { get; set; }
    public string BusinessContext { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EscalationAlert
{
    public string OperationId { get; set; }
    public Exception Exception { get; set; }
    public string AffectedData { get; set; }
    public string EscalationLevel { get; set; }
    public bool RequiresOnCallResponse { get; set; }
}

public class DataPersistenceFailureLog
{
    public string OperationId { get; set; }
    public string EntityId { get; set; }
    public string[] PersistenceErrors { get; set; }
    public string AttemptedOperation { get; set; }
    public DateTime Timestamp { get; set; }
}

public class OperationCompletionLog
{
    public string OperationId { get; set; }
    public string Status { get; set; }
    public TimeSpan Duration { get; set; }
    public int ErrorCount { get; set; }
    public string[] FinalErrors { get; set; }
    public Exception Exception { get; set; }
    public string ExceptionType { get; set; }
}

public class ValidationPattern
{
    public string DataType { get; set; }
    public List<string> ErrorTypes { get; set; }
    public Dictionary<string, object> DataCharacteristics { get; set; }
    public DateTime Timestamp { get; set; }
}

public class BusinessImprovementSuggestion
{
    public string Area { get; set; }
    public string DataType { get; set; }
    public string SuggestedImprovement { get; set; }
    public string Priority { get; set; }
    public string EstimatedImpact { get; set; }
}

public class BusinessErrorNotification
{
    public string StakeholderId { get; set; }
    public string EntityId { get; set; }
    public string ErrorSummary { get; set; }
    public string ImpactAssessment { get; set; }
    public List<string> SuggestedActions { get; set; }
    public string Priority { get; set; }
}

public class BusinessStakeholder
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Name { get; set; }
}

// Excepciones personalizadas
public class NetworkException : Exception
{
    public NetworkException(string message) : base(message) { }
    public NetworkException(string message, Exception innerException) : base(message, innerException) { }
}

public class DatabaseException : Exception
{
    public bool IsConnectionFailure { get; set; }
    public DatabaseException(string message) : base(message) { }
    public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
    public SecurityException(string message, Exception innerException) : base(message, innerException) { }
}

public class PerformanceException : Exception
{
    public PerformanceException(string message) : base(message) { }
    public PerformanceException(string message, Exception innerException) : base(message, innerException) { }
}

// Interfaces de servicios especializados
public interface ITechnicalLogger
{
    Task LogCriticalErrorAsync(Exception ex, TechnicalErrorContext context);
    Task LogWarningAsync(string message);
    Task LogOperationCompletionAsync(OperationCompletionLog log);
}

public interface IBusinessLogger
{
    Task LogValidationFailureAsync(ValidationFailureLog log);
    Task LogBusinessRuleViolationAsync(BusinessRuleViolationLog log);
    Task LogDataPersistenceFailureAsync(DataPersistenceFailureLog log);
    Task LogOperationCompletionAsync(OperationCompletionLog log);
    Task LogValidationPatternAsync(ValidationPattern pattern);
}

public interface IAlertService
{
    Task SendEscalationAlertAsync(EscalationAlert alert);
    Task SendNetworkIssueAlertAsync(NetworkException ex, string operationId);
    Task SendDatabaseIssueAlertAsync(DatabaseException ex, string operationId);
    Task SendSecurityIncidentAlertAsync(SecurityException ex, string operationId, string entityId);
    Task SendPerformanceIssueAlertAsync(PerformanceException ex, string operationId);
    Task SendUnknownTechnicalIssueAlertAsync(Exception ex, string operationId);
    Task SendBusinessImprovementSuggestionAsync(BusinessImprovementSuggestion suggestion);
    Task SendBusinessNotificationAsync(BusinessErrorNotification notification);
}

public interface IMetricsService
{
    Task IncrementCounterAsync(string counterName);
}

public interface ITelemetryService
{
    Task RecordExceptionAsync(Exception ex);
    Task RecordExceptionAsync(Exception ex, Dictionary<string, string> properties);
}

public interface IIncidentService
{
    Task CreateAutomaticIncidentAsync(TechnicalIncident incident);
}
```

### Ejemplo 2: Sistema de Procesamiento de Archivos con Manejo Diferenciado

```csharp
public class FileProcessingService
{
    private readonly IFileValidator _validator;
    private readonly IFileProcessor _processor;
    private readonly IFileStorage _storage;
    private readonly ITechnicalLogger _techLogger;
    private readonly IBusinessLogger _bizLogger;
    private readonly INotificationService _notifications;
    private readonly ISecurityService _security;
    
    public FileProcessingService(
        IFileValidator validator,
        IFileProcessor processor,
        IFileStorage storage,
        ITechnicalLogger techLogger,
        IBusinessLogger bizLogger,
        INotificationService notifications,
        ISecurityService security)
    {
        _validator = validator;
        _processor = processor;
        _storage = storage;
        _techLogger = techLogger;
        _bizLogger = bizLogger;
        _notifications = notifications;
        _security = security;
    }
    
    public async Task<MlResult<ProcessedFile>> ProcessFileWithDifferentiatedHandlingAsync(FileUpload upload)
    {
        var processingId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateFileUpload(upload)
            // Manejo de errores de validación de negocio (reglas de archivos)
            .ExecSelfIfFailWithoutExceptionAsync(async errors => 
            {
                await _bizLogger.LogFileValidationFailureAsync(new FileValidationLog
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    UserId = upload.UserId,
                    ValidationErrors = errors.AllErrors,
                    FileSize = upload.SizeInBytes,
                    FileType = upload.ContentType,
                    Timestamp = DateTime.UtcNow
                });
                
                // Notificar al usuario sobre problemas de validación
                await _notifications.SendFileValidationErrorsAsync(upload.UserId, upload.FileName, errors.AllErrors);
                
                // Analizar patrones de archivos rechazados para mejorar UX
                await AnalyzeFileRejectionPatternsAsync(upload, errors);
            })
            
            // Manejo de errores técnicos en validación (excepciones del sistema)
            .TryExecSelfIfFailWithExceptionAsync(async ex => 
            {
                await _techLogger.LogFileValidationExceptionAsync(ex, new FileValidationExceptionContext
                {
                    ProcessingId = processingId,
                    FileName = upload.FileName,
                    FilePath = upload.TempFilePath,
                    FileSize = upload.SizeInBytes,
                    Operation = "FileValidation"
                });
                
                // Manejo específico por tipo de excepción técnica
                switch (ex)
                {
                    case FileNotFoundException:
                        await HandleMissingFileExceptionAsync(ex, upload, processingId);
         …