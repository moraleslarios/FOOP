# MlResultActionsExecSelfIfFailWithException - Manejo Específico de Excepciones

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFailWithException](#métodos-execselfiffailwithexception)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryExecSelfIfFailWithException - Doble Protección](#métodos-tryexecselfiffailwithexception---doble-protección)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos `ExecSelfIfFailWithException` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido Y contiene una excepción específica en los detalles del error**. Estos métodos extraen la excepción original de los detalles del error y la pasan a la acción, permitiendo un manejo especializado de excepciones con acceso completo al objeto Exception original.

### Propósito Principal

- **Logging Detallado de Excepciones**: Registrar información específica de la excepción (stack trace, inner exceptions, etc.)
- **Análisis de Errores Técnicos**: Categorizar y analizar tipos específicos de excepciones
- **Integración con Sistemas de Monitoreo**: Enviar excepciones a sistemas como Sentry, ApplicationInsights, etc.
- **Debugging Avanzado**: Acceso completo a la excepción para análisis técnico detallado
- **Escalamiento Automático**: Determinar si una excepción requiere escalamiento inmediato

---

## Análisis de los Métodos

### Estructura y Filosofía

Los métodos `ExecSelfIfFailWithException` implementan el patrón de **manejo específico de excepciones con extracción contextual**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido → Extraer Exception → Ejecutar Acción(exception) → Resultado Fallido (sin cambios)
      ↓                          ↓
Resultado Fallido sin Exception → No acción → Resultado Fallido (sin cambios)
```

### Características Principales

1. **Extracción de Excepciones**: Utiliza `GetDetailException()` para extraer excepciones específicas de los detalles del error
2. **Ejecución Condicional Especial**: Solo ejecuta si es fallido AND contiene una excepción
3. **Acceso Completo a Exception**: Proporciona el objeto Exception completo con todos sus detalles
4. **Inmutabilidad**: El resultado original nunca se modifica
5. **Doble Protección**: Las versiones `Try*` manejan excepciones en las propias acciones de manejo

---

## Métodos ExecSelfIfFailWithException

### `ExecSelfIfFailWithException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido y contiene una excepción específica en los detalles, pasando esa excepción a la acción

```csharp
public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                         Action<Exception> actionFailException)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFailException`: Acción a ejecutar solo si `source` es fallido y contiene una Exception

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido pero no contiene Exception: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido y contiene Exception: Extrae la excepción, ejecuta `actionFailException(exception)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ProcessDataWithTryCatch(inputData)
    .ExecSelfIfFailWithException(exception => 
    {
        _logger.LogError(exception, "Data processing failed with exception");
        
        // Análisis específico del tipo de excepción
        switch (exception)
        {
            case SqlException sqlEx:
                _metrics.IncrementCounter("database.errors");
                break;
            case TimeoutException timeoutEx:
                _metrics.IncrementCounter("timeout.errors");
                break;
            case UnauthorizedAccessException authEx:
                _metrics.IncrementCounter("security.errors");
                break;
        }
    });
```

**Ejemplo con Integración de Monitoreo**:
```csharp
var result = CallExternalAPI(apiRequest)
    .ExecSelfIfFailWithException(exception => 
    {
        // Enviar a sistema de monitoreo
        _sentryClient.CaptureException(exception);
        
        // Logging estructurado con detalles de la excepción
        _logger.LogError(exception, "External API call failed", new
        {
            ApiEndpoint = apiRequest.Endpoint,
            ExceptionType = exception.GetType().Name,
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.Message
        });
        
        // Determinar si requiere escalamiento inmediato
        if (IsCriticalException(exception))
        {
            _alertService.SendCriticalAlert($"Critical exception in API call: {exception.Message}");
        }
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

**Comportamiento**: Ejecuta una acción asíncrona solo si el resultado es fallido y contiene una excepción

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .ExecSelfIfFailWithExceptionAsync(async exception => 
    {
        // Análisis asíncrono detallado de la excepción
        await _exceptionAnalyzer.AnalyzeExceptionAsync(new ExceptionAnalysisContext
        {
            Exception = exception,
            Operation = "DocumentProcessing",
            Timestamp = DateTime.UtcNow,
            UserId = document.UserId
        });
        
        // Envío asíncrono a múltiples sistemas de monitoreo
        var tasks = new[]
        {
            _sentryClient.CaptureExceptionAsync(exception),
            _applicationInsights.TrackExceptionAsync(exception),
            _customLoggingService.LogExceptionAsync(exception, "DocumentProcessing")
        };
        
        await Task.WhenAll(tasks);
        
        // Notificación asíncrona si es necesario
        if (IsUserFacingException(exception))
        {
            await _notificationService.NotifyUserOfProcessingErrorAsync(document.UserId, exception);
        }
    });
```

### `ExecSelfIfFailWithExceptionAsync<T>()` - Fuente Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException)
```

**Ejemplo con Fuente Asíncrona**:
```csharp
var result = await ProcessPaymentAsync(paymentData)
    .ExecSelfIfFailWithExceptionAsync(async exception => 
    {
        // Manejo especializado de excepciones de pago
        await HandlePaymentExceptionAsync(new PaymentExceptionContext
        {
            Exception = exception,
            PaymentAmount = paymentData.Amount,
            CustomerId = paymentData.CustomerId,
            PaymentMethod = paymentData.PaymentMethodId
        });
    });

private async Task HandlePaymentExceptionAsync(PaymentExceptionContext context)
{
    switch (context.Exception)
    {
        case PaymentDeclinedException declined:
            await _fraudDetection.AnalyzeDeclinedPaymentAsync(context.CustomerId, declined);
            await _notificationService.SendPaymentDeclinedNotificationAsync(context.CustomerId);
            break;
            
        case InsufficientFundsException insufficient:
            await _customerService.SuggestPaymentAlternativesAsync(context.CustomerId);
            break;
            
        case PaymentTimeoutException timeout:
            await _paymentGateway.RetryPaymentAsync(context.CustomerId, context.PaymentAmount);
            break;
            
        case FraudDetectedException fraud:
            await _securityService.InitiateFraudInvestigationAsync(context.CustomerId, fraud);
            await _alertService.SendFraudAlertAsync(context.CustomerId, fraud);
            break;
    }
}
```

---

## Métodos TryExecSelfIfFailWithException - Doble Protección

### `TryExecSelfIfFailWithException<T>()` - Versión Ultra-Segura

```csharp
public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            string errorMessage = null!)
```

**Comportamiento Especial**: 
- Proporciona **doble protección** contra excepciones
- Si la acción de manejo de excepción lanza una excepción, la captura y agrega al resultado original
- Útil cuando el manejo de excepciones puede fallar (servicios externos, etc.)

**Ejemplo**:
```csharp
var result = ProcessCriticalOperation(operationData)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Estas operaciones pueden fallar
            _externalLoggingService.LogException(originalException); // Puede estar caído
            _alertingSystem.SendCriticalAlert(originalException); // Puede fallar por rate limiting
            _auditService.RecordFailure(originalException); // Puede tener problemas de conectividad
        },
        ex => $"Failed to handle exception logging/alerting: {ex.Message}"
    );

// Si ProcessCriticalOperation falla: result contiene esa excepción
// Si además el manejo de la excepción falla: result contiene ambos errores
// Esto evita perder información crítica de diagnóstico
```

**Ejemplo Avanzado con Múltiples Sistemas**:
```csharp
var result = ProcessHighVolumeData(data)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Intentar múltiples sistemas de logging/alerting
            try
            {
                _primaryLoggingSystem.LogException(originalException);
            }
            catch (Exception ex1)
            {
                try
                {
                    _fallbackLoggingSystem.LogException(originalException);
                }
                catch (Exception ex2)
                {
                    // Como último recurso, log a archivo local
                    _fileLogger.LogException(originalException);
                    throw new AggregateException("All logging systems failed", ex1, ex2);
                }
            }
            
            // Sistema de alertas que puede fallar
            _unreliableAlertingSystem.SendAlert(originalException); // Puede lanzar excepción
        },
        handlingEx => $"Exception handling chain failed: {handlingEx.Message}"
    );
```

### Versiones Asíncronas de TryExecSelfIfFailWithException

#### `TryExecSelfIfFailWithExceptionAsync<T>()` - Todas las Variantes

```csharp
// Acción asíncrona ultra-segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

// Fuente asíncrona con acción asíncrona ultra-segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Monitoreo y Diagnóstico Avanzado

```csharp
public class AdvancedDiagnosticsService
{
    private readonly ILogger _logger;
    private readonly ISentryClient _sentry;
    private readonly IApplicationInsights _appInsights;
    private readonly ISlackNotifier _slack;
    private readonly IExceptionAnalyzer _analyzer;
    private readonly IPerformanceMonitor _performance;
    private readonly ISecurityMonitor _security;
    private readonly IDatabaseHealthMonitor _dbHealth;
    
    public AdvancedDiagnosticsService(
        ILogger logger,
        ISentryClient sentry,
        IApplicationInsights appInsights,
        ISlackNotifier slack,
        IExceptionAnalyzer analyzer,
        IPerformanceMonitor performance,
        ISecurityMonitor security,
        IDatabaseHealthMonitor dbHealth)
    {
        _logger = logger;
        _sentry = sentry;
        _appInsights = appInsights;
        _slack = slack;
        _analyzer = analyzer;
        _performance = performance;
        _security = security;
        _dbHealth = dbHealth;
    }
    
    public async Task<MlResult<ProcessedData>> ProcessDataWithAdvancedDiagnosticsAsync(DataRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateDataRequest(request)
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleValidationExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle validation exception diagnostics: {ex.Message}")
            .BindAsync(async validRequest => await TransformDataAsync(validRequest))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleTransformationExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle transformation exception diagnostics: {ex.Message}")
            .BindAsync(async transformed => await ValidateBusinessRulesAsync(transformed))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleBusinessRuleExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle business rule exception diagnostics: {ex.Message}")
            .BindAsync(async validated => await PersistDataAsync(validated))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandlePersistenceExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle persistence exception diagnostics: {ex.Message}")
            .ExecSelfIfFailWithExceptionAsync(async exception => 
            {
                var duration = DateTime.UtcNow - startTime;
                await RecordOverallFailureMetricsAsync(correlationId, request, exception, duration);
            });
    }
    
    private async Task HandleValidationExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Validation",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico por tipo de excepción de validación
        switch (exception)
        {
            case ArgumentNullException nullEx:
                await HandleNullArgumentExceptionAsync(context, nullEx);
                break;
                
            case ArgumentException argEx:
                await HandleArgumentExceptionAsync(context, argEx);
                break;
                
            case FormatException formatEx:
                await HandleFormatExceptionAsync(context, formatEx);
                break;
                
            case ValidationException validationEx:
                await HandleValidationExceptionAsync(context, validationEx);
                break;
                
            default:
                await HandleUnknownValidationExceptionAsync(context, exception);
                break;
        }
        
        // Logging estructurado común
        await LogStructuredExceptionAsync(context);
        
        // Envío a sistemas de monitoreo
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandleTransformationExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Transformation",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de transformación
        switch (exception)
        {
            case OutOfMemoryException memEx:
                await HandleOutOfMemoryExceptionAsync(context, memEx, request);
                break;
                
            case StackOverflowException stackEx:
                await HandleStackOverflowExceptionAsync(context, stackEx, request);
                break;
                
            case InvalidOperationException invalidOpEx:
                await HandleInvalidOperationExceptionAsync(context, invalidOpEx);
                break;
                
            case NotSupportedException notSupportedEx:
                await HandleNotSupportedExceptionAsync(context, notSupportedEx);
                break;
                
            default:
                await HandleUnknownTransformationExceptionAsync(context, exception);
                break;
        }
        
        // Análisis de recursos del sistema
        await AnalyzeSystemResourcesAsync(context);
        
        // Logging y monitoreo
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandleBusinessRuleExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "BusinessRules",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de reglas de negocio
        switch (exception)
        {
            case BusinessRuleViolationException ruleEx:
                await HandleBusinessRuleViolationAsync(context, ruleEx);
                break;
                
            case UnauthorizedAccessException authEx:
                await HandleUnauthorizedAccessAsync(context, authEx, request);
                break;
                
            case SecurityException secEx:
                await HandleSecurityExceptionAsync(context, secEx, request);
                break;
                
            case ConcurrencyException concurrencyEx:
                await HandleConcurrencyExceptionAsync(context, concurrencyEx);
                break;
                
            default:
                await HandleUnknownBusinessRuleExceptionAsync(context, exception);
                break;
        }
        
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandlePersistenceExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Persistence",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de persistencia
        switch (exception)
        {
            case SqlException sqlEx:
                await HandleSqlExceptionAsync(context, sqlEx);
                break;
                
            case TimeoutException timeoutEx:
                await HandleDatabaseTimeoutExceptionAsync(context, timeoutEx);
                break;
                
            case InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("connection"):
                await HandleDatabaseConnectionExceptionAsync(context, invalidOpEx);
                break;
                
            case EntityValidationException entityEx:
                await HandleEntityValidationExceptionAsync(context, entityEx);
                break;
                
            case DbUpdateConcurrencyException concurrencyEx:
                await HandleDbConcurrencyExceptionAsync(context, concurrencyEx);
                break;
                
            default:
                await HandleUnknownPersistenceExceptionAsync(context, exception);
                break;
        }
        
        // Verificar salud de la base de datos
        await CheckDatabaseHealthAsync(context);
        
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    // Métodos específicos de manejo por tipo de excepción
    private async Task HandleNullArgumentExceptionAsync(ExceptionDiagnosticContext context, ArgumentNullException exception)
    {
        await _analyzer.AnalyzeNullReferencePatternAsync(new NullReferenceAnalysis
        {
            ParameterName = exception.ParamName,
            StackTrace = exception.StackTrace,
            CorrelationId = context.CorrelationId
        });
        
        await _performance.RecordNullReferenceIncidentAsync(context.Stage, exception.ParamName);
        
        // Si es un parámetro crítico, escalamiento inmediato
        if (IsCriticalParameter(exception.ParamName))
        {
            await _slack.SendCriticalAlertAsync($"Critical null parameter in {context.Stage}: {exception.ParamName}");
        }
    }
    
    private async Task HandleOutOfMemoryExceptionAsync(ExceptionDiagnosticContext context, OutOfMemoryException exception, DataRequest request)
    {
        // Análisis de memoria y recursos
        var memoryAnalysis = await _performance.AnalyzeMemoryUsageAsync();
        
        await _performance.RecordMemoryExhaustionAsync(new MemoryExhaustionEvent
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            RequestSize = EstimateRequestSize(request),
            AvailableMemory = memoryAnalysis.AvailableMemory,
            UsedMemory = memoryAnalysis.UsedMemory,
            Timestamp = DateTime.UtcNow
        });
        
        // Alerta crítica inmediata
        await _slack.SendCriticalAlertAsync($"OutOfMemoryException in {context.Stage} - System may be unstable");
        
        // Iniciar análisis de tendencias de memoria
        await _performance.InitiateMemoryTrendAnalysisAsync(context.CorrelationId);
    }
    
    private async Task HandleSqlExceptionAsync(ExceptionDiagnosticContext context, SqlException exception)
    {
        await _dbHealth.AnalyzeSqlExceptionAsync(new SqlExceptionAnalysis
        {
            SqlState = exception.State,
            ErrorNumber = exception.Number,
            Severity = exception.Class,
            Procedure = exception.Procedure,
            LineNumber = exception.LineNumber,
            Server = exception.Server,
            CorrelationId = context.CorrelationId
        });
        
        // Análisis específico por número de error SQL
        switch (exception.Number)
        {
            case 2: // Timeout
                await HandleSqlTimeoutAsync(context, exception);
                break;
            case 18456: // Login failed
                await HandleSqlLoginFailureAsync(context, exception);
                break;
            case 1205: // Deadlock
                await HandleSqlDeadlockAsync(context, exception);
                break;
            case 8152: // String truncation
                await HandleSqlDataTruncationAsync(context, exception);
                break;
            default:
                await HandleUnknownSqlErrorAsync(context, exception);
                break;
        }
    }
    
    private async Task HandleUnauthorizedAccessAsync(ExceptionDiagnosticContext context, UnauthorizedAccessException exception, DataRequest request)
    {
        // Análisis de seguridad
        await _security.AnalyzeUnauthorizedAccessAsync(new SecurityIncident
        {
            CorrelationId = context.CorrelationId,
            UserId = request.UserId,
            RequestedResource = request.ResourceId,
            IPAddress = request.ClientIPAddress,
            UserAgent = request.UserAgent,
            Timestamp = DateTime.UtcNow,
            Exception = exception
        });
        
        // Verificar patrones de acceso sospechoso
        var suspiciousActivity = await _security.CheckSuspiciousActivityAsync(request.UserId, request.ClientIPAddress);
        if (suspiciousActivity.IsSuspicious)
        {
            await _security.InitiateSecurityInvestigationAsync(suspiciousActivity);
            await _slack.SendSecurityAlertAsync($"Suspicious unauthorized access pattern detected for user {request.UserId}");
        }
        
        // Logging de seguridad especializado
        await _security.LogSecurityEventAsync(new SecurityEvent
        {
            EventType = "UnauthorizedAccess",
            UserId = request.UserId,
            ResourceId = request.ResourceId,
            Details = exception.Message,
            RiskLevel = suspiciousActivity.IsSuspicious ? "High" : "Medium"
        });
    }
    
    private async Task LogStructuredExceptionAsync(ExceptionDiagnosticContext context)
    {
        var logEntry = new StructuredExceptionLog
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            Operation = context.Operation,
            ExceptionType = context.Exception.GetType().FullName,
            ExceptionMessage = context.Exception.Message,
            StackTrace = context.Exception.StackTrace,
            InnerException = context.Exception.InnerException?.ToString(),
            RequestData = context.RequestData,
            Timestamp = context.Timestamp,
            MachineName = Environment.MachineName,
            ProcessId = Environment.ProcessId,
            ThreadId = Thread.CurrentThread.ManagedThreadId
        };
        
        await _logger.LogStructuredExceptionAsync(logEntry);
    }
    
    private async Task SendToMonitoringSystemsAsync(ExceptionDiagnosticContext context)
    {
        var monitoringTasks = new List<Task>();
        
        // Sentry para tracking de excepciones
        monitoringTasks.Add(_sentry.CaptureExceptionAsync(context.Exception, new
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            Operation = context.Operation
        }));
        
        // Application Insights para telemetría
        monitoringTasks.Add(_appInsights.TrackExceptionAsync(context.Exception, new Dictionary<string, string>
        {
            ["CorrelationId"] = context.CorrelationId,
            ["Stage"] = context.Stage,
            ["Operation"] = context.Operation,
            ["Timestamp"] = context.Timestamp.ToString("O")
        }));
        
        // Ejecutar en paralelo con manejo de errores
        await Task.WhenAll(monitoringTasks.Select(async task =>
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                await _logger.LogWarningAsync($"Failed to send exception to monitoring system: {ex.Message}");
            }
        }));
    }
    
    private async Task RecordOverallFailureMetricsAsync(string correlationId, DataRequest request, Exception exception, TimeSpan duration)
    {
        var metrics = new OverallFailureMetrics
        {
            CorrelationId = correlationId,
            UserId = request.UserId,
            RequestSize = EstimateRequestSize(request),
            ProcessingDuration = duration,
            ExceptionType = exception.GetType().FullName,
            FailureTimestamp = DateTime.UtcNow
        };
        
        await _performance.RecordFailureMetricsAsync(metrics);
        
        // Análisis de tendencias de fallo
        await _analyzer.AnalyzeFailureTrendsAsync(metrics);
    }
    
    // Métodos auxiliares
    private string SerializeRequestSafely(DataRequest request)
    {
        try
        {
            return JsonSerializer.Serialize(new
            {
                request.UserId,
                RequestSize = EstimateRequestSize(request),
                request.ResourceId,
                ClientIP = request.ClientIPAddress?.Substring(0, Math.Min(15, request.ClientIPAddress.Length))
            });
        }
        catch
        {
            return $"UserId: {request.UserId}, Size: {EstimateRequestSize(request)}";
        }
    }
    
    private int EstimateRequestSize(DataRequest request) => 
        (request.Data?.Length ?? 0) + (request.Metadata?.Length ?? 0);
    
    private bool IsCriticalParameter(string parameterName) =>
        new[] { "connectionString", "userId", "securityToken", "dataSource" }
            .Contains(parameterName, StringComparer.OrdinalIgnoreCase);
    
    // Implementaciones simplificadas de métodos específicos
    private async Task HandleArgumentExceptionAsync(ExceptionDiagnosticContext context, ArgumentException exception) { }
    private async Task HandleFormatExceptionAsync(ExceptionDiagnosticContext context, FormatException exception) { }
    private async Task HandleValidationExceptionAsync(ExceptionDiagnosticContext context, ValidationException exception) { }
    private async Task HandleUnknownValidationExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task HandleStackOverflowExceptionAsync(ExceptionDiagnosticContext context, StackOverflowException exception, DataRequest request) { }
    private async Task HandleInvalidOperationExceptionAsync(ExceptionDiagnosticContext context, InvalidOperationException exception) { }
    private async Task HandleNotSupportedExceptionAsync(ExceptionDiagnosticContext context, NotSupportedException exception) { }
    private async Task HandleUnknownTransformationExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task AnalyzeSystemResourcesAsync(ExceptionDiagnosticContext context) { }
    private async Task HandleBusinessRuleViolationAsync(ExceptionDiagnosticContext context, BusinessRuleViolationException exception) { }
    private async Task HandleSecurityExceptionAsync(ExceptionDiagnosticContext context, SecurityException exception, DataRequest request) { }
    private async Task HandleConcurrencyExceptionAsync(ExceptionDiagnosticContext context, ConcurrencyException exception) { }
    private async Task HandleUnknownBusinessRuleExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task HandleDatabaseTimeoutExceptionAsync(ExceptionDiagnosticContext context, TimeoutException exception) { }
    private async Task HandleDatabaseConnectionExceptionAsync(ExceptionDiagnosticContext context, InvalidOperationException exception) { }
    private async Task HandleEntityValidationExceptionAsync(ExceptionDiagnosticContext context, EntityValidationException exception) { }
    private async Task HandleDbConcurrencyExceptionAsync(ExceptionDiagnosticContext context, DbUpdateConcurrencyException exception) { }
    private async Task HandleUnknownPersistenceExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task CheckDatabaseHealthAsync(ExceptionDiagnosticContext context) { }
    private async Task HandleSqlTimeoutAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlLoginFailureAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlDeadlockAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlDataTruncationAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleUnknownSqlErrorAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    
    // Implementaciones de operaciones principales (simplificadas)
    private async Task<MlResult<DataRequest>> ValidateDataRequest(DataRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.Data))
                throw new ArgumentException("Data cannot be empty", nameof(request.Data));
            
            return MlResult<DataRequest>.Valid(request);
        }
        catch (Exception ex)
        {
            return MlResult<DataRequest>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<TransformedData>> TransformDataAsync(DataRequest request)
    {
        try
        {
            // Simulación de transformación que puede lanzar diferentes excepciones
            if (request.Data.Length > 1000000)
                throw new OutOfMemoryException("Data too large for processing");
            
            var transformed = new TransformedData { ProcessedData = request.Data.ToUpper() };
            return MlResult<TransformedData>.Valid(transformed);
        }
        catch (Exception ex)
        {
            return MlResult<TransformedData>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<ValidatedData>> ValidateBusinessRulesAsync(TransformedData data)
    {
        try
        {
            // Simulación de validación de reglas de negocio
            if (data.ProcessedData.Contains("FORBIDDEN"))
                throw new BusinessRuleViolationException("Data contains forbidden content");
            
            var validated = new ValidatedData { Data = data };
            return MlResult<ValidatedData>.Valid(validated);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedData>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<ProcessedData>> PersistDataAsync(ValidatedData data)
    {
        try
        {
            // Simulación de persistencia que puede fallar
            if (DateTime.Now.Millisecond % 10 == 0) // Simular fallo aleatorio
                throw new SqlException("Database connection timeout", 2, 0, null, null);
            
            var processed = new ProcessedData { Id = Guid.NewGuid(), Data = data };
            return MlResult<ProcessedData>.Valid(processed);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedData>.FailWithException(ex);
        }
    }
}

// Clases de apoyo
public class ExceptionDiagnosticContext
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public string Operation { get; set; }
    public Exception Exception { get; set; }
    public string RequestData { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DataRequest
{
    public int UserId { get; set; }
    public string Data { get; set; }
    public string Metadata { get; set; }
    public string ResourceId { get; set; }
    public string ClientIPAddress { get; set; }
    public string UserAgent { get; set; }
}

public class TransformedData
{
    public string ProcessedData { get; set; }
}

public class ValidatedData
{
    public TransformedData Data { get; set; }
}

public class ProcessedData
{
    public Guid Id { get; set; }
    public ValidatedData Data { get; set; }
}

public class PaymentExceptionContext
{
    public Exception Exception { get; set; }
    public decimal PaymentAmount { get; set; }
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; }
}

public class NullReferenceAnalysis
{
    public string ParameterName { get; set; }
    public string StackTrace { get; set; }
    public string CorrelationId { get; set; }
}

public class MemoryExhaustionEvent
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public int RequestSize { get; set; }
    public long AvailableMemory { get; set; }
    public long UsedMemory { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SqlExceptionAnalysis
{
    public byte SqlState { get; set; }
    public int ErrorNumber { get; set; }
    public byte Severity { get; set; }
    public string Procedure { get; set; }
    public int LineNumber { get; set; }
    public string Server { get; set; }
    public string CorrelationId { get; set; }
}

public class SecurityIncident
{
    public string CorrelationId { get; set; }
    public int UserId { get; set; }
    public string RequestedResource { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public Exception Exception { get; set; }
}

public class SecurityEvent
{
    public string EventType { get; set; }
    public int UserId { get; set; }
    public string ResourceId { get; set; }
    public string Details { get; set; }
    public string RiskLevel { get; set; }
}

public class StructuredExceptionLog
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public string Operation { get; set; }
    public string ExceptionType { get; set; }
    public string ExceptionMessage { get; set; }
    public string StackTrace { get; set; }
    public string InnerException { get; set; }
    public string RequestData { get; set; }
    public DateTime Timestamp { get; set; }
    public string MachineName { get; set; }
    public int ProcessId { get; set; }
    public int ThreadId { get; set; }
}

public class OverallFailureMetrics
{
    public string CorrelationId { get; set; }
    public int UserId { get; set; }
    public int RequestSize { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public string ExceptionType { get; set; }
    public DateTime FailureTimestamp { get; set; }
}

public class SuspiciousActivityResult
{
    public bool IsSuspicious { get; set; }
    public string Reason { get; set; }
    public int RiskScore { get; set; }
}

public class MemoryAnalysisResult
{
    public long AvailableMemory { get; set; }
    public long UsedMemory { get; set; }
    public double MemoryUsagePercentage { get; set; }
}

// Excepciones personalizadas
public class BusinessRuleViolationException : Exception
{
    public BusinessRuleViolationException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
}

public class EntityValidationException : Exception
{
    public EntityValidationException(string message) : base(message) { }
}

public class PaymentDeclinedException : Exception
{
    public PaymentDeclinedException(string message) : base(message) { }
}

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(string message) : base(message) { }
}

public class PaymentTimeoutException : Exception
{
    public PaymentTimeoutException(string message) : base(message) { }
}

public class FraudDetectedException : Exception
{
    public FraudDetectedException(string message) : base(message) { }
}

public class DbUpdateConcurrencyException : Exception
{
    public DbUpdateConcurrencyException(string message) : base(message) { }
}

// Interfaces de servicios
public interface IExceptionAnalyzer
{
    Task AnalyzeNullReferencePatternAsync(NullReferenceAnalysis analysis);
    Task AnalyzeFailureTrendsAsync(OverallFailureMetrics metrics);
    Task AnalyzeExceptionAsync(ExceptionAnalysisContext context);
}

public interface IPerformanceMonitor
{
    Task RecordNullReferenceIncidentAsync(string stage, string parameterName);
    Task AnalyzeMemoryUsageAsync();
    Task RecordMemoryExhaustionAsync(MemoryExhaustionEvent evt);
    Task InitiateMemoryTrendAnalysisAsync(string correlationId);
    Task RecordFailureMetricsAsync(OverallFailureMetrics metrics);
}

public interface ISecurityMonitor
{
    Task AnalyzeUnauthorizedAccessAsync(SecurityIncident incident);
    Task CheckSuspiciousActivityAsync(int userId, string ipAddress);
    Task InitiateSecurityInvestigationAsync(SuspiciousActivityResult activity);
    Task LogSecurityEventAsync(SecurityEvent evt);
}

public interface IDatabaseHealthMonitor
{
    Task AnalyzeSqlExceptionAsync(SqlExceptionAnalysis analysis);
}

public interface ISentryClient
{
    Task CaptureExceptionAsync(Exception exception);
    Task CaptureExceptionAsync(Exception exception, object additionalData);
}

public interface IApplicationInsights
{
    Task TrackExceptionAsync(Exception exception);
    Task TrackExceptionAsync(Exception exception, Dictionary<string, string> properties);
}

public interface ISlackNotifier
{
    Task SendCriticalAlertAsync(string message);
    Task SendSecurityAlertAsync(string message);
}

public interface INotificationService
{
    Task NotifyUserOfProcessingErrorAsync(int userId, Exception exception);
    Task SendPaymentDeclinedNotificationAsync(int customerId);
}

public class ExceptionAnalysisContext
{
    public Exception Exception { get; set; }
    public string Operation { get; set; }
    public DateTime Timestamp { get; set; }
    public int UserId { get; set; }
}
```

### Ejemplo 2: Sistema de E-commerce con Manejo Especializado de Excepciones de Pago

```csharp
public class PaymentProcessingService
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IFraudDetectionService _fraudDetection;
    private readonly ICustomerService _customerService;
    private readonly INotificationService _notifications;
    private readonly ILogger _logger;
    private readonly IMetricsService _metrics;
    private readonly IAlertService _alerts;
    
    public PaymentProcessingService(
        IPaymentGateway paymentGateway,
        IFraudDetectionService fraudDetection,
        ICustomerService customerService,
        INotificationService notifications,
        ILogger logger,
        IMetricsService metrics,
        IAlertService alerts)
    {
        _paymentGateway = paymentGateway;
        _fraudDetection = fraudDetection;
        _customerService = customerService;
        _notifications = notifications;
        _logger = logger;
        _metrics = metrics;
        _alerts = alerts;
    }
    
    public async Task<MlResult<PaymentResult>> ProcessPaymentWithSpecializedHandlingAsync(PaymentRequest request)
    {
        var paymentId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidatePaymentRequest(request)
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandlePaymentValidationExceptionAsync(paymentId, request, exception),
                ex => $"Failed to handle payment validation exception: {ex.Message}")
            .BindAsync(async validRequest => await CheckFraudRiskAsync(validRequest))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleFraudCheckExceptionAsync(paymentId, request, exception),
                ex => $"Failed to handle fraud check exception: {ex.Message}")
            .BindAsync(async checkedRequest => await ProcessPaymentWithGatewayAsync(checkedRequest))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleGatewayExceptionAsync(paymentId, request, exception),
                ex => $"Failed to handle payment gateway exception: {ex.Message}")
            .BindAsync(async gatewayResult => await FinalizePaymentAsync(gatewayResult))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleFinalizationExceptionAsync(paymentId, request, exception),
                ex => $"Failed to handle payment finalization exception: {ex.Message}")
            .ExecSelfIfFailWithExceptionAsync(async exception => 
            {
                var duration = DateTime.UtcNow - startTime;
                await RecordPaymentFailureMetricsAsync(paymentId, request, exception, duration);
            });
    }
    
    private async Task HandlePaymentValidationExceptionAsync(string paymentId, PaymentRequest request, Exception exception)
    {
        var context = new PaymentExceptionContext
        {
            PaymentId = paymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Exception = exception,
            Stage = "Validation"
        };
        
        switch (exception)
        {
            case ArgumentNullException nullEx:
                await HandleNullPaymentParameterAsync(context, nullEx);
                break;
                
            case ArgumentException argEx:
                await HandleInvalidPaymentArgumentAsync(context, argEx);
                break;
                
            case CurrencyMismatchException currencyEx:
                await HandleCurrencyMismatchAsync(context, currencyEx);
                break;
                
            case PaymentAmountException amountEx:
                await HandleInvalidAmountAsync(context, amountEx);
                break;
                
            default:
                await HandleUnknownValidationExceptionAsync(context, exception);
                break;
        }
        
        await LogPaymentExceptionAsync(context);
    }
    
    private async Task HandleFraudCheckExceptionAsync(string paymentId, PaymentRequest request, Exception exception)
    {
        var context = new PaymentExceptionContext
        {
            PaymentId = paymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Exception = exception,
            Stage = "FraudCheck"
        };
        
        switch (exception)
        {
            case FraudServiceUnavailableException fraudUnavailableEx:
                await HandleFraudServiceUnavailableAsync(context, fraudUnavailableEx, request);
                break;
                
            case SuspiciousActivityException suspiciousEx:
                await HandleSuspiciousActivityDetectedAsync(context, suspiciousEx, request);
                break;
                
            case FraudServiceTimeoutException timeoutEx:
                await HandleFraudServiceTimeoutAsync(context, timeoutEx, request);
                break;
                
            case HighRiskTransactionException highRiskEx:
                await HandleHighRiskTransactionAsync(context, highRiskEx, request);
                break;
                
            default:
                await HandleUnknownFraudExceptionAsync(context, exception);
                break;
        }
        
        await LogPaymentExceptionAsync(context);
    }
    
    private async Task HandleGatewayExceptionAsync(string paymentId, PaymentRequest request, Exception exception)
    {
        var context = new PaymentExceptionContext
        {
            PaymentId = paymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Exception = exception,
            Stage = "Gateway"
        };
        
        switch (exception)
        {
            case CardDeclinedException declinedEx:
                await HandleCardDeclinedAsync(context, declinedEx, request);
                break;
                
            case InsufficientFundsException insufficientEx:
                await HandleInsufficientFundsAsync(context, insufficientEx, request);
                break;
                
            case ExpiredCardException expiredEx:
                await HandleExpiredCardAsync(context, expiredEx, request);
                break;
                
            case InvalidCardException invalidEx:
                await HandleInvalidCardAsync(context, invalidEx, request);
                break;
                
            case GatewayTimeoutException gatewayTimeoutEx:
                await HandleGatewayTimeoutAsync(context, gatewayTimeoutEx, request);
                break;
                
            case GatewayUnavailableException gatewayUnavailableEx:
                await HandleGatewayUnavailableAsync(context, gatewayUnavailableEx, request);
                break;
                
            case ThreeDSecureRequiredException threeDSEx:
                await HandleThreeDSecureRequiredAsync(context, threeDSEx, request);
                break;
                
            default:
                await HandleUnknownGatewayExceptionAsync(context, exception);
                break;
        }
        
        await LogPaymentExceptionAsync(context);
    }
    
    private async Task HandleFinalizationExceptionAsync(string paymentId, PaymentRequest request, Exception exception)
    {
        var context = new PaymentExceptionContext
        {
            PaymentId = paymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Exception = exception,
            Stage = "Finalization"
        };
        
        switch (exception)
        {
            case DatabaseException dbEx:
                await HandleDatabaseExceptionDuringFinalizationAsync(context, dbEx, request);
                break;
                
            case ConcurrencyException concurrencyEx:
                await HandleConcurrencyExceptionDuringFinalizationAsync(context, concurrencyEx, request);
                break;
                
            case NotificationException notificationEx:
                await HandleNotificationExceptionDuringFinalizationAsync(context, notificationEx, request);
                break;
                
            default:
                await HandleUnknownFinalizationExceptionAsync(context, exception);
                break;
        }
        
        await LogPaymentExceptionAsync(context);
    }
    
    // Métodos específicos de manejo por tipo de excepción
    private async Task HandleCardDeclinedAsync(PaymentExceptionContext context, CardDeclinedException exception, PaymentRequest request)
    {
        // Análisis específico de tarjeta declinada
        await _metrics.IncrementCounterAsync("payments.card_declined");
        await _metrics.RecordDeclinedPaymentAsync(new DeclinedPaymentMetrics
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            DeclineReason = exception.DeclineReason,
            DeclineCode = exception.DeclineCode,
            PaymentMethod = request.PaymentMethodType
        });
        
        // Notificar al cliente con sugerencias específicas
        await _notifications.SendCardDeclinedNotificationAsync(new CardDeclinedNotification
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            DeclineReason = exception.DeclineReason,
            SuggestedActions = GenerateDeclineSuggestions(exception.DeclineCode),
            AlternativePaymentMethods = await GetAlternativePaymentMethodsAsync(request.CustomerId)
        });
        
        // Análisis de patrones de declive por cliente
        await AnalyzeCustomerDeclinePatternAsync(request.CustomerId, exception);
    }
    
    private async Task HandleInsufficientFundsAsync(PaymentExceptionContext context, InsufficientFundsException exception, PaymentRequest request)
    {
        await _metrics.IncrementCounterAsync("payments.insufficient_funds");
        
        // Obtener historial del cliente para sugerencias personalizadas
        var customerHistory = await _customerService.GetPaymentHistoryAsync(request.CustomerId);
        
        // Sugerir alternativas basadas en el perfil del cliente
        var suggestions = await GenerateInsufficientFundsSuggestionsAsync(request, customerHistory);
        
        await _notifications.SendInsufficientFundsNotificationAsync(new InsufficientFundsNotification
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Suggestions = suggestions,
            PaymentPlanOptions = await GetPaymentPlanOptionsAsync(request.CustomerId, request.Amount)
        });
        
        // Programa de seguimiento para retry automático
        await SchedulePaymentRetryAsync(request, TimeSpan.FromHours(24));
    }
    
    private async Task HandleFraudServiceUnavailableAsync(PaymentExceptionContext context, FraudServiceUnavailableException exception, PaymentRequest request)
    {
        await _metrics.IncrementCounterAsync("payments.fraud_service_unavailable");
        
        // Evaluar si proceder con verificaciones fallback
        var riskAssessment = await PerformBasicRiskAssessmentAsync(request);
        
        if (riskAssessment.RiskLevel == RiskLevel.Low)
        {
            // Proceder con verificaciones básicas
            await _logger.LogWarningAsync($"Fraud service unavailable, proceeding with basic risk assessment for payment {context.PaymentId}");
            await _notifications.SendFraudServiceUnavailableAlertAsync("Proceeding with fallback verification");
        }
        else
        {
            // Requerir verificación manual para pagos de riesgo medio/alto
            await _logger.LogErrorAsync($"Fraud service unavailable, manual review required for high-risk payment {context.PaymentId}");
            await CreateManualReviewTaskAsync(request, "Fraud service unavailable - manual review required");
            await _alerts.SendCriticalAlertAsync($"High-risk payment requires manual review due to fraud service unavailability: {context.PaymentId}");
        }
    }
    
    private async Task HandleSuspiciousActivityDetectedAsync(PaymentExceptionContext context, SuspiciousActivityException exception, PaymentRequest request)
    {
        await _metrics.IncrementCounterAsync("payments.suspicious_activity");
        
        // Crear caso de investigación de fraude
        var investigationCase = new FraudInvestigationCase
        {
            PaymentId = context.PaymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            SuspiciousIndicators = exception.SuspiciousIndicators,
            RiskScore = exception.RiskScore,
            CreatedAt = DateTime.UtcNow
        };
        
        await _fraudDetection.CreateInvestigationCaseAsync(investigationCase);
        
        // Alerta inmediata al equipo de seguridad
        await _alerts.SendFraudAlertAsync($"Suspicious activity detected in payment {context.PaymentId}", new
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            RiskScore = exception.RiskScore,
            Indicators = exception.SuspiciousIndicators
        });
        
        // Bloqueo temporal del cliente si el riesgo es muy alto
        if (exception.RiskScore > 0.8)
        {
            await _customerService.TemporaryBlockCustomerAsync(request.CustomerId, "High fraud risk detected");
            await _notifications.SendAccountTemporaryBlockNotificationAsync(request.CustomerId);
        }
    }
    
    private async Task HandleGatewayTimeoutAsync(PaymentExceptionContext context, GatewayTimeoutException exception, PaymentRequest request)
    {
        await _metrics.IncrementCounterAsync("payments.gateway_timeout");
        await _metrics.RecordGatewayPerformanceAsync(new GatewayPerformanceMetrics
        {
            GatewayName = exception.GatewayName,
            TimeoutDuration = exception.TimeoutDuration,
            RequestTimestamp = context.Exception.Data["RequestTimestamp"] as DateTime? ?? DateTime.UtcNow
        });
        
        // Verificar si es un problema sistemático del gateway
        var recentTimeouts = await GetRecentGatewayTimeoutsAsync(exception.GatewayName);
        if (recentTimeouts > 5)
        {
            await _alerts.SendGatewayHealthAlertAsync($"Gateway {exception.GatewayName} experiencing high timeout rate");
            
            // Considerar switch automático a gateway alternativo
            if (await ShouldSwitchToAlternativeGatewayAsync(exception.GatewayName))
            {
                await _alerts.SendGatewaySwitchAlertAsync($"Switching to alternative gateway due to timeouts in {exception.GatewayName}");
            }
        }
        
        // Programar retry con gateway alternativo
        await SchedulePaymentRetryWithAlternativeGatewayAsync(request);
    }
    
    private async Task HandleDatabaseExceptionDuringFinalizationAsync(PaymentExceptionContext context, DatabaseException exception, PaymentRequest request)
    {
        await _metrics.IncrementCounterAsync("payments.database_error_during_finalization");
        
        // Situación crítica: el pago pudo haber sido procesado pero no finalizado
        await _alerts.SendCriticalAlertAsync($"Database error during payment finalization - manual investigation required: {context.PaymentId}");
        
        // Crear tarea de investigación manual
        await CreateCriticalInvestigationTaskAsync(new CriticalInvestigationTask
        {
            PaymentId = context.PaymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Issue = "Database error during finalization - payment status uncertain",
            Priority = InvestigationPriority.Critical,
            AssignedTeam = "PaymentOperations"
        });
        
        // Logging detallado para investigación
        await _logger.LogCriticalAsync($"Payment finalization database error", new
        {
            PaymentId = context.PaymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            DatabaseError = exception.Message,
            StackTrace = exception.StackTrace,
            TransactionId = request.TransactionId
        });
    }
    
    private async Task LogPaymentExceptionAsync(PaymentExceptionContext context)
    {
        var logEntry = new PaymentExceptionLog
        {
            PaymentId = context.PaymentId,
            CustomerId = context.CustomerId,
            Amount = context.Amount,
            Stage = context.Stage,
            ExceptionType = context.Exception.GetType().FullName,
            ExceptionMessage = context.Exception.Message,
            StackTrace = context.Exception.StackTrace,
            Timestamp = DateTime.UtcNow
        };
        
        await _logger.LogPaymentExceptionAsync(logEntry);
    }
    
    private async Task RecordPaymentFailureMetricsAsync(string paymentId, PaymentRequest request, Exception exception, TimeSpan duration)
    {
        var metrics = new PaymentFailureMetrics
        {
            PaymentId = paymentId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            Duration = duration,
            ExceptionType = exception.GetType().FullName,
            FailureStage = DetermineFailureStage(exception),
            Timestamp = DateTime.UtcNow
        };
        
        await _metrics.RecordPaymentFailureAsync(metrics);
    }
    
    // Métodos auxiliares
    private List<string> GenerateDeclineSuggestions(string declineCode) =>
        declineCode switch
        {
            "01" => new List<string> { "Contact your bank", "Try a different card" },
            "05" => new List<string> { "Check card details", "Contact card issuer" },
            "51" => new List<string> { "Check account balance", "Try alternative payment method" },
            _ => new List<string> { "Contact customer support" }
        };
    
    private string DetermineFailureStage(Exception exception) =>
        exception.GetType().Name switch
        {
            var name when name.Contains("Validation") => "Validation",
            var name when name.Contains("Fraud") => "FraudCheck",
            var name when name.Contains("Gateway") || name.Contains("Card") => "Gateway",
            var name when name.Contains("Database") || name.Contains("Finalization") => "Finalization",
            _ => "Unknown"
        };
    
    // Implementaciones simplificadas
    private async Task<MlResult<PaymentRequest>> ValidatePaymentRequest(PaymentRequest request) { return MlResult<PaymentRequest>.Valid(request); }
    private async Task<MlResult<FraudCheckedRequest>> CheckFraudRiskAsync(PaymentRequest request) { return MlResult<FraudCheckedRequest>.Valid(new FraudCheckedRequest()); }
    private async Task<MlResult<GatewayResult>> ProcessPaymentWithGatewayAsync(FraudCheckedRequest request) { return MlResult<GatewayResult>.Valid(new Gatew// filepath: c:\PakkkoTFS\MoralesLarios\FOOP\MoralesLarios.FOOP\docs\MlResultActionsExecSelfIfFailWithException.md
# MlResultActionsExecSelfIfFailWithException - Manejo Específico de Excepciones

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFailWithException](#métodos-execselfiffailwithexception)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryExecSelfIfFailWithException - Doble Protección](#métodos-tryexecselfiffailwithexception---doble-protección)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Otros Métodos](#comparación-con-otros-métodos)

---

## Introducción

Los métodos `ExecSelfIfFailWithException` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido Y contiene una excepción específica en los detalles del error**. Estos métodos extraen la excepción original de los detalles del error y la pasan a la acción, permitiendo un manejo especializado de excepciones con acceso completo al objeto Exception original.

### Propósito Principal

- **Logging Detallado de Excepciones**: Registrar información específica de la excepción (stack trace, inner exceptions, etc.)
- **Análisis de Errores Técnicos**: Categorizar y analizar tipos específicos de excepciones
- **Integración con Sistemas de Monitoreo**: Enviar excepciones a sistemas como Sentry, ApplicationInsights, etc.
- **Debugging Avanzado**: Acceso completo a la excepción para análisis técnico detallado
- **Escalamiento Automático**: Determinar si una excepción requiere escalamiento inmediato

---

## Análisis de los Métodos

### Estructura y Filosofía

Los métodos `ExecSelfIfFailWithException` implementan el patrón de **manejo específico de excepciones con extracción contextual**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido → Extraer Exception → Ejecutar Acción(exception) → Resultado Fallido (sin cambios)
      ↓                          ↓
Resultado Fallido sin Exception → No acción → Resultado Fallido (sin cambios)
```

### Características Principales

1. **Extracción de Excepciones**: Utiliza `GetDetailException()` para extraer excepciones específicas de los detalles del error
2. **Ejecución Condicional Especial**: Solo ejecuta si es fallido AND contiene una excepción
3. **Acceso Completo a Exception**: Proporciona el objeto Exception completo con todos sus detalles
4. **Inmutabilidad**: El resultado original nunca se modifica
5. **Doble Protección**: Las versiones `Try*` manejan excepciones en las propias acciones de manejo

---

## Métodos ExecSelfIfFailWithException

### `ExecSelfIfFailWithException<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido y contiene una excepción específica en los detalles, pasando esa excepción a la acción

```csharp
public static MlResult<T> ExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                         Action<Exception> actionFailException)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFailException`: Acción a ejecutar solo si `source` es fallido y contiene una Exception

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido pero no contiene Exception: Retorna `source` sin cambios, no ejecuta `actionFailException`
- Si `source` es fallido y contiene Exception: Extrae la excepción, ejecuta `actionFailException(exception)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = ProcessDataWithTryCatch(inputData)
    .ExecSelfIfFailWithException(exception => 
    {
        _logger.LogError(exception, "Data processing failed with exception");
        
        // Análisis específico del tipo de excepción
        switch (exception)
        {
            case SqlException sqlEx:
                _metrics.IncrementCounter("database.errors");
                break;
            case TimeoutException timeoutEx:
                _metrics.IncrementCounter("timeout.errors");
                break;
            case UnauthorizedAccessException authEx:
                _metrics.IncrementCounter("security.errors");
                break;
        }
    });
```

**Ejemplo con Integración de Monitoreo**:
```csharp
var result = CallExternalAPI(apiRequest)
    .ExecSelfIfFailWithException(exception => 
    {
        // Enviar a sistema de monitoreo
        _sentryClient.CaptureException(exception);
        
        // Logging estructurado con detalles de la excepción
        _logger.LogError(exception, "External API call failed", new
        {
            ApiEndpoint = apiRequest.Endpoint,
            ExceptionType = exception.GetType().Name,
            StackTrace = exception.StackTrace,
            InnerException = exception.InnerException?.Message
        });
        
        // Determinar si requiere escalamiento inmediato
        if (IsCriticalException(exception))
        {
            _alertService.SendCriticalAlert($"Critical exception in API call: {exception.Message}");
        }
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

**Comportamiento**: Ejecuta una acción asíncrona solo si el resultado es fallido y contiene una excepción

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .ExecSelfIfFailWithExceptionAsync(async exception => 
    {
        // Análisis asíncrono detallado de la excepción
        await _exceptionAnalyzer.AnalyzeExceptionAsync(new ExceptionAnalysisContext
        {
            Exception = exception,
            Operation = "DocumentProcessing",
            Timestamp = DateTime.UtcNow,
            UserId = document.UserId
        });
        
        // Envío asíncrono a múltiples sistemas de monitoreo
        var tasks = new[]
        {
            _sentryClient.CaptureExceptionAsync(exception),
            _applicationInsights.TrackExceptionAsync(exception),
            _customLoggingService.LogExceptionAsync(exception, "DocumentProcessing")
        };
        
        await Task.WhenAll(tasks);
        
        // Notificación asíncrona si es necesario
        if (IsUserFacingException(exception))
        {
            await _notificationService.NotifyUserOfProcessingErrorAsync(document.UserId, exception);
        }
    });
```

### `ExecSelfIfFailWithExceptionAsync<T>()` - Fuente Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync)

public static async Task<MlResult<T>> ExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<Exception> actionFailException)
```

**Ejemplo con Fuente Asíncrona**:
```csharp
var result = await ProcessPaymentAsync(paymentData)
    .ExecSelfIfFailWithExceptionAsync(async exception => 
    {
        // Manejo especializado de excepciones de pago
        await HandlePaymentExceptionAsync(new PaymentExceptionContext
        {
            Exception = exception,
            PaymentAmount = paymentData.Amount,
            CustomerId = paymentData.CustomerId,
            PaymentMethod = paymentData.PaymentMethodId
        });
    });

private async Task HandlePaymentExceptionAsync(PaymentExceptionContext context)
{
    switch (context.Exception)
    {
        case PaymentDeclinedException declined:
            await _fraudDetection.AnalyzeDeclinedPaymentAsync(context.CustomerId, declined);
            await _notificationService.SendPaymentDeclinedNotificationAsync(context.CustomerId);
            break;
            
        case InsufficientFundsException insufficient:
            await _customerService.SuggestPaymentAlternativesAsync(context.CustomerId);
            break;
            
        case PaymentTimeoutException timeout:
            await _paymentGateway.RetryPaymentAsync(context.CustomerId, context.PaymentAmount);
            break;
            
        case FraudDetectedException fraud:
            await _securityService.InitiateFraudInvestigationAsync(context.CustomerId, fraud);
            await _alertService.SendFraudAlertAsync(context.CustomerId, fraud);
            break;
    }
}
```

---

## Métodos TryExecSelfIfFailWithException - Doble Protección

### `TryExecSelfIfFailWithException<T>()` - Versión Ultra-Segura

```csharp
public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfIfFailWithException<T>(this MlResult<T> source,
                                                            Action<Exception> actionFailException,
                                                            string errorMessage = null!)
```

**Comportamiento Especial**: 
- Proporciona **doble protección** contra excepciones
- Si la acción de manejo de excepción lanza una excepción, la captura y agrega al resultado original
- Útil cuando el manejo de excepciones puede fallar (servicios externos, etc.)

**Ejemplo**:
```csharp
var result = ProcessCriticalOperation(operationData)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Estas operaciones pueden fallar
            _externalLoggingService.LogException(originalException); // Puede estar caído
            _alertingSystem.SendCriticalAlert(originalException); // Puede fallar por rate limiting
            _auditService.RecordFailure(originalException); // Puede tener problemas de conectividad
        },
        ex => $"Failed to handle exception logging/alerting: {ex.Message}"
    );

// Si ProcessCriticalOperation falla: result contiene esa excepción
// Si además el manejo de la excepción falla: result contiene ambos errores
// Esto evita perder información crítica de diagnóstico
```

**Ejemplo Avanzado con Múltiples Sistemas**:
```csharp
var result = ProcessHighVolumeData(data)
    .TryExecSelfIfFailWithException(
        originalException => 
        {
            // Intentar múltiples sistemas de logging/alerting
            try
            {
                _primaryLoggingSystem.LogException(originalException);
            }
            catch (Exception ex1)
            {
                try
                {
                    _fallbackLoggingSystem.LogException(originalException);
                }
                catch (Exception ex2)
                {
                    // Como último recurso, log a archivo local
                    _fileLogger.LogException(originalException);
                    throw new AggregateException("All logging systems failed", ex1, ex2);
                }
            }
            
            // Sistema de alertas que puede fallar
            _unreliableAlertingSystem.SendAlert(originalException); // Puede lanzar excepción
        },
        handlingEx => $"Exception handling chain failed: {handlingEx.Message}"
    );
```

### Versiones Asíncronas de TryExecSelfIfFailWithException

#### `TryExecSelfIfFailWithExceptionAsync<T>()` - Todas las Variantes

```csharp
// Acción asíncrona ultra-segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this MlResult<T> source,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)

// Fuente asíncrona con acción asíncrona ultra-segura
public static async Task<MlResult<T>> TryExecSelfIfFailWithExceptionAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<Exception, Task> actionFailExceptionAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Monitoreo y Diagnóstico Avanzado

```csharp
public class AdvancedDiagnosticsService
{
    private readonly ILogger _logger;
    private readonly ISentryClient _sentry;
    private readonly IApplicationInsights _appInsights;
    private readonly ISlackNotifier _slack;
    private readonly IExceptionAnalyzer _analyzer;
    private readonly IPerformanceMonitor _performance;
    private readonly ISecurityMonitor _security;
    private readonly IDatabaseHealthMonitor _dbHealth;
    
    public AdvancedDiagnosticsService(
        ILogger logger,
        ISentryClient sentry,
        IApplicationInsights appInsights,
        ISlackNotifier slack,
        IExceptionAnalyzer analyzer,
        IPerformanceMonitor performance,
        ISecurityMonitor security,
        IDatabaseHealthMonitor dbHealth)
    {
        _logger = logger;
        _sentry = sentry;
        _appInsights = appInsights;
        _slack = slack;
        _analyzer = analyzer;
        _performance = performance;
        _security = security;
        _dbHealth = dbHealth;
    }
    
    public async Task<MlResult<ProcessedData>> ProcessDataWithAdvancedDiagnosticsAsync(DataRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateDataRequest(request)
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleValidationExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle validation exception diagnostics: {ex.Message}")
            .BindAsync(async validRequest => await TransformDataAsync(validRequest))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleTransformationExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle transformation exception diagnostics: {ex.Message}")
            .BindAsync(async transformed => await ValidateBusinessRulesAsync(transformed))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandleBusinessRuleExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle business rule exception diagnostics: {ex.Message}")
            .BindAsync(async validated => await PersistDataAsync(validated))
            .TryExecSelfIfFailWithExceptionAsync(
                async exception => await HandlePersistenceExceptionAsync(correlationId, request, exception),
                ex => $"Failed to handle persistence exception diagnostics: {ex.Message}")
            .ExecSelfIfFailWithExceptionAsync(async exception => 
            {
                var duration = DateTime.UtcNow - startTime;
                await RecordOverallFailureMetricsAsync(correlationId, request, exception, duration);
            });
    }
    
    private async Task HandleValidationExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Validation",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico por tipo de excepción de validación
        switch (exception)
        {
            case ArgumentNullException nullEx:
                await HandleNullArgumentExceptionAsync(context, nullEx);
                break;
                
            case ArgumentException argEx:
                await HandleArgumentExceptionAsync(context, argEx);
                break;
                
            case FormatException formatEx:
                await HandleFormatExceptionAsync(context, formatEx);
                break;
                
            case ValidationException validationEx:
                await HandleValidationExceptionAsync(context, validationEx);
                break;
                
            default:
                await HandleUnknownValidationExceptionAsync(context, exception);
                break;
        }
        
        // Logging estructurado común
        await LogStructuredExceptionAsync(context);
        
        // Envío a sistemas de monitoreo
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandleTransformationExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Transformation",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de transformación
        switch (exception)
        {
            case OutOfMemoryException memEx:
                await HandleOutOfMemoryExceptionAsync(context, memEx, request);
                break;
                
            case StackOverflowException stackEx:
                await HandleStackOverflowExceptionAsync(context, stackEx, request);
                break;
                
            case InvalidOperationException invalidOpEx:
                await HandleInvalidOperationExceptionAsync(context, invalidOpEx);
                break;
                
            case NotSupportedException notSupportedEx:
                await HandleNotSupportedExceptionAsync(context, notSupportedEx);
                break;
                
            default:
                await HandleUnknownTransformationExceptionAsync(context, exception);
                break;
        }
        
        // Análisis de recursos del sistema
        await AnalyzeSystemResourcesAsync(context);
        
        // Logging y monitoreo
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandleBusinessRuleExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "BusinessRules",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de reglas de negocio
        switch (exception)
        {
            case BusinessRuleViolationException ruleEx:
                await HandleBusinessRuleViolationAsync(context, ruleEx);
                break;
                
            case UnauthorizedAccessException authEx:
                await HandleUnauthorizedAccessAsync(context, authEx, request);
                break;
                
            case SecurityException secEx:
                await HandleSecurityExceptionAsync(context, secEx, request);
                break;
                
            case ConcurrencyException concurrencyEx:
                await HandleConcurrencyExceptionAsync(context, concurrencyEx);
                break;
                
            default:
                await HandleUnknownBusinessRuleExceptionAsync(context, exception);
                break;
        }
        
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    private async Task HandlePersistenceExceptionAsync(string correlationId, DataRequest request, Exception exception)
    {
        var context = new ExceptionDiagnosticContext
        {
            CorrelationId = correlationId,
            Stage = "Persistence",
            Operation = "ProcessData",
            Exception = exception,
            RequestData = SerializeRequestSafely(request),
            Timestamp = DateTime.UtcNow
        };
        
        // Análisis específico de excepciones de persistencia
        switch (exception)
        {
            case SqlException sqlEx:
                await HandleSqlExceptionAsync(context, sqlEx);
                break;
                
            case TimeoutException timeoutEx:
                await HandleDatabaseTimeoutExceptionAsync(context, timeoutEx);
                break;
                
            case InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("connection"):
                await HandleDatabaseConnectionExceptionAsync(context, invalidOpEx);
                break;
                
            case EntityValidationException entityEx:
                await HandleEntityValidationExceptionAsync(context, entityEx);
                break;
                
            case DbUpdateConcurrencyException concurrencyEx:
                await HandleDbConcurrencyExceptionAsync(context, concurrencyEx);
                break;
                
            default:
                await HandleUnknownPersistenceExceptionAsync(context, exception);
                break;
        }
        
        // Verificar salud de la base de datos
        await CheckDatabaseHealthAsync(context);
        
        await LogStructuredExceptionAsync(context);
        await SendToMonitoringSystemsAsync(context);
    }
    
    // Métodos específicos de manejo por tipo de excepción
    private async Task HandleNullArgumentExceptionAsync(ExceptionDiagnosticContext context, ArgumentNullException exception)
    {
        await _analyzer.AnalyzeNullReferencePatternAsync(new NullReferenceAnalysis
        {
            ParameterName = exception.ParamName,
            StackTrace = exception.StackTrace,
            CorrelationId = context.CorrelationId
        });
        
        await _performance.RecordNullReferenceIncidentAsync(context.Stage, exception.ParamName);
        
        // Si es un parámetro crítico, escalamiento inmediato
        if (IsCriticalParameter(exception.ParamName))
        {
            await _slack.SendCriticalAlertAsync($"Critical null parameter in {context.Stage}: {exception.ParamName}");
        }
    }
    
    private async Task HandleOutOfMemoryExceptionAsync(ExceptionDiagnosticContext context, OutOfMemoryException exception, DataRequest request)
    {
        // Análisis de memoria y recursos
        var memoryAnalysis = await _performance.AnalyzeMemoryUsageAsync();
        
        await _performance.RecordMemoryExhaustionAsync(new MemoryExhaustionEvent
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            RequestSize = EstimateRequestSize(request),
            AvailableMemory = memoryAnalysis.AvailableMemory,
            UsedMemory = memoryAnalysis.UsedMemory,
            Timestamp = DateTime.UtcNow
        });
        
        // Alerta crítica inmediata
        await _slack.SendCriticalAlertAsync($"OutOfMemoryException in {context.Stage} - System may be unstable");
        
        // Iniciar análisis de tendencias de memoria
        await _performance.InitiateMemoryTrendAnalysisAsync(context.CorrelationId);
    }
    
    private async Task HandleSqlExceptionAsync(ExceptionDiagnosticContext context, SqlException exception)
    {
        await _dbHealth.AnalyzeSqlExceptionAsync(new SqlExceptionAnalysis
        {
            SqlState = exception.State,
            ErrorNumber = exception.Number,
            Severity = exception.Class,
            Procedure = exception.Procedure,
            LineNumber = exception.LineNumber,
            Server = exception.Server,
            CorrelationId = context.CorrelationId
        });
        
        // Análisis específico por número de error SQL
        switch (exception.Number)
        {
            case 2: // Timeout
                await HandleSqlTimeoutAsync(context, exception);
                break;
            case 18456: // Login failed
                await HandleSqlLoginFailureAsync(context, exception);
                break;
            case 1205: // Deadlock
                await HandleSqlDeadlockAsync(context, exception);
                break;
            case 8152: // String truncation
                await HandleSqlDataTruncationAsync(context, exception);
                break;
            default:
                await HandleUnknownSqlErrorAsync(context, exception);
                break;
        }
    }
    
    private async Task HandleUnauthorizedAccessAsync(ExceptionDiagnosticContext context, UnauthorizedAccessException exception, DataRequest request)
    {
        // Análisis de seguridad
        await _security.AnalyzeUnauthorizedAccessAsync(new SecurityIncident
        {
            CorrelationId = context.CorrelationId,
            UserId = request.UserId,
            RequestedResource = request.ResourceId,
            IPAddress = request.ClientIPAddress,
            UserAgent = request.UserAgent,
            Timestamp = DateTime.UtcNow,
            Exception = exception
        });
        
        // Verificar patrones de acceso sospechoso
        var suspiciousActivity = await _security.CheckSuspiciousActivityAsync(request.UserId, request.ClientIPAddress);
        if (suspiciousActivity.IsSuspicious)
        {
            await _security.InitiateSecurityInvestigationAsync(suspiciousActivity);
            await _slack.SendSecurityAlertAsync($"Suspicious unauthorized access pattern detected for user {request.UserId}");
        }
        
        // Logging de seguridad especializado
        await _security.LogSecurityEventAsync(new SecurityEvent
        {
            EventType = "UnauthorizedAccess",
            UserId = request.UserId,
            ResourceId = request.ResourceId,
            Details = exception.Message,
            RiskLevel = suspiciousActivity.IsSuspicious ? "High" : "Medium"
        });
    }
    
    private async Task LogStructuredExceptionAsync(ExceptionDiagnosticContext context)
    {
        var logEntry = new StructuredExceptionLog
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            Operation = context.Operation,
            ExceptionType = context.Exception.GetType().FullName,
            ExceptionMessage = context.Exception.Message,
            StackTrace = context.Exception.StackTrace,
            InnerException = context.Exception.InnerException?.ToString(),
            RequestData = context.RequestData,
            Timestamp = context.Timestamp,
            MachineName = Environment.MachineName,
            ProcessId = Environment.ProcessId,
            ThreadId = Thread.CurrentThread.ManagedThreadId
        };
        
        await _logger.LogStructuredExceptionAsync(logEntry);
    }
    
    private async Task SendToMonitoringSystemsAsync(ExceptionDiagnosticContext context)
    {
        var monitoringTasks = new List<Task>();
        
        // Sentry para tracking de excepciones
        monitoringTasks.Add(_sentry.CaptureExceptionAsync(context.Exception, new
        {
            CorrelationId = context.CorrelationId,
            Stage = context.Stage,
            Operation = context.Operation
        }));
        
        // Application Insights para telemetría
        monitoringTasks.Add(_appInsights.TrackExceptionAsync(context.Exception, new Dictionary<string, string>
        {
            ["CorrelationId"] = context.CorrelationId,
            ["Stage"] = context.Stage,
            ["Operation"] = context.Operation,
            ["Timestamp"] = context.Timestamp.ToString("O")
        }));
        
        // Ejecutar en paralelo con manejo de errores
        await Task.WhenAll(monitoringTasks.Select(async task =>
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                await _logger.LogWarningAsync($"Failed to send exception to monitoring system: {ex.Message}");
            }
        }));
    }
    
    private async Task RecordOverallFailureMetricsAsync(string correlationId, DataRequest request, Exception exception, TimeSpan duration)
    {
        var metrics = new OverallFailureMetrics
        {
            CorrelationId = correlationId,
            UserId = request.UserId,
            RequestSize = EstimateRequestSize(request),
            ProcessingDuration = duration,
            ExceptionType = exception.GetType().FullName,
            FailureTimestamp = DateTime.UtcNow
        };
        
        await _performance.RecordFailureMetricsAsync(metrics);
        
        // Análisis de tendencias de fallo
        await _analyzer.AnalyzeFailureTrendsAsync(metrics);
    }
    
    // Métodos auxiliares
    private string SerializeRequestSafely(DataRequest request)
    {
        try
        {
            return JsonSerializer.Serialize(new
            {
                request.UserId,
                RequestSize = EstimateRequestSize(request),
                request.ResourceId,
                ClientIP = request.ClientIPAddress?.Substring(0, Math.Min(15, request.ClientIPAddress.Length))
            });
        }
        catch
        {
            return $"UserId: {request.UserId}, Size: {EstimateRequestSize(request)}";
        }
    }
    
    private int EstimateRequestSize(DataRequest request) => 
        (request.Data?.Length ?? 0) + (request.Metadata?.Length ?? 0);
    
    private bool IsCriticalParameter(string parameterName) =>
        new[] { "connectionString", "userId", "securityToken", "dataSource" }
            .Contains(parameterName, StringComparer.OrdinalIgnoreCase);
    
    // Implementaciones simplificadas de métodos específicos
    private async Task HandleArgumentExceptionAsync(ExceptionDiagnosticContext context, ArgumentException exception) { }
    private async Task HandleFormatExceptionAsync(ExceptionDiagnosticContext context, FormatException exception) { }
    private async Task HandleValidationExceptionAsync(ExceptionDiagnosticContext context, ValidationException exception) { }
    private async Task HandleUnknownValidationExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task HandleStackOverflowExceptionAsync(ExceptionDiagnosticContext context, StackOverflowException exception, DataRequest request) { }
    private async Task HandleInvalidOperationExceptionAsync(ExceptionDiagnosticContext context, InvalidOperationException exception) { }
    private async Task HandleNotSupportedExceptionAsync(ExceptionDiagnosticContext context, NotSupportedException exception) { }
    private async Task HandleUnknownTransformationExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task AnalyzeSystemResourcesAsync(ExceptionDiagnosticContext context) { }
    private async Task HandleBusinessRuleViolationAsync(ExceptionDiagnosticContext context, BusinessRuleViolationException exception) { }
    private async Task HandleSecurityExceptionAsync(ExceptionDiagnosticContext context, SecurityException exception, DataRequest request) { }
    private async Task HandleConcurrencyExceptionAsync(ExceptionDiagnosticContext context, ConcurrencyException exception) { }
    private async Task HandleUnknownBusinessRuleExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task HandleDatabaseTimeoutExceptionAsync(ExceptionDiagnosticContext context, TimeoutException exception) { }
    private async Task HandleDatabaseConnectionExceptionAsync(ExceptionDiagnosticContext context, InvalidOperationException exception) { }
    private async Task HandleEntityValidationExceptionAsync(ExceptionDiagnosticContext context, EntityValidationException exception) { }
    private async Task HandleDbConcurrencyExceptionAsync(ExceptionDiagnosticContext context, DbUpdateConcurrencyException exception) { }
    private async Task HandleUnknownPersistenceExceptionAsync(ExceptionDiagnosticContext context, Exception exception) { }
    private async Task CheckDatabaseHealthAsync(ExceptionDiagnosticContext context) { }
    private async Task HandleSqlTimeoutAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlLoginFailureAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlDeadlockAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleSqlDataTruncationAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    private async Task HandleUnknownSqlErrorAsync(ExceptionDiagnosticContext context, SqlException exception) { }
    
    // Implementaciones de operaciones principales (simplificadas)
    private async Task<MlResult<DataRequest>> ValidateDataRequest(DataRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.Data))
                throw new ArgumentException("Data cannot be empty", nameof(request.Data));
            
            return MlResult<DataRequest>.Valid(request);
        }
        catch (Exception ex)
        {
            return MlResult<DataRequest>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<TransformedData>> TransformDataAsync(DataRequest request)
    {
        try
        {
            // Simulación de transformación que puede lanzar diferentes excepciones
            if (request.Data.Length > 1000000)
                throw new OutOfMemoryException("Data too large for processing");
            
            var transformed = new TransformedData { ProcessedData = request.Data.ToUpper() };
            return MlResult<TransformedData>.Valid(transformed);
        }
        catch (Exception ex)
        {
            return MlResult<TransformedData>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<ValidatedData>> ValidateBusinessRulesAsync(TransformedData data)
    {
        try
        {
            // Simulación de validación de reglas de negocio
            if (data.ProcessedData.Contains("FORBIDDEN"))
                throw new BusinessRuleViolationException("Data contains forbidden content");
            
            var validated = new ValidatedData { Data = data };
            return MlResult<ValidatedData>.Valid(validated);
        }
        catch (Exception ex)
        {
            return MlResult<ValidatedData>.FailWithException(ex);
        }
    }
    
    private async Task<MlResult<ProcessedData>> PersistDataAsync(ValidatedData data)
    {
        try
        {
            // Simulación de persistencia que puede fallar
            if (DateTime.Now.Millisecond % 10 == 0) // Simular fallo aleatorio
                throw new SqlException("Database connection timeout", 2, 0, null, null);
            
            var processed = new ProcessedData { Id = Guid.NewGuid(), Data = data };
            return MlResult<ProcessedData>.Valid(processed);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedData>.FailWithException(ex);
        }
    }
}

// Clases de apoyo
public class ExceptionDiagnosticContext
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public string Operation { get; set; }
    public Exception Exception { get; set; }
    public string RequestData { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DataRequest
{
    public int UserId { get; set; }
    public string Data { get; set; }
    public string Metadata { get; set; }
    public string ResourceId { get; set; }
    public string ClientIPAddress { get; set; }
    public string UserAgent { get; set; }
}

public class TransformedData
{
    public string ProcessedData { get; set; }
}

public class ValidatedData
{
    public TransformedData Data { get; set; }
}

public class ProcessedData
{
    public Guid Id { get; set; }
    public ValidatedData Data { get; set; }
}

public class PaymentExceptionContext
{
    public Exception Exception { get; set; }
    public decimal PaymentAmount { get; set; }
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; }
}

public class NullReferenceAnalysis
{
    public string ParameterName { get; set; }
    public string StackTrace { get; set; }
    public string CorrelationId { get; set; }
}

public class MemoryExhaustionEvent
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public int RequestSize { get; set; }
    public long AvailableMemory { get; set; }
    public long UsedMemory { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SqlExceptionAnalysis
{
    public byte SqlState { get; set; }
    public int ErrorNumber { get; set; }
    public byte Severity { get; set; }
    public string Procedure { get; set; }
    public int LineNumber { get; set; }
    public string Server { get; set; }
    public string CorrelationId { get; set; }
}

public class SecurityIncident
{
    public string CorrelationId { get; set; }
    public int UserId { get; set; }
    public string RequestedResource { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public Exception Exception { get; set; }
}

public class SecurityEvent
{
    public string EventType { get; set; }
    public int UserId { get; set; }
    public string ResourceId { get; set; }
    public string Details { get; set; }
    public string RiskLevel { get; set; }
}

public class StructuredExceptionLog
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public string Operation { get; set; }
    public string ExceptionType { get; set; }
    public string ExceptionMessage { get; set; }
    public string StackTrace { get; set; }
    public string InnerException { get; set; }
    public string RequestData { get; set; }
    public DateTime Timestamp { get; set; }
    public string MachineName { get; set; }
    public int ProcessId { get; set; }
    public int ThreadId { get; set; }
}

public class OverallFailureMetrics
{
    public string CorrelationId { get; set; }
    public int UserId { get; set; }
    public int RequestSize { get; set; }
    public TimeSpan ProcessingDuration { get; set; }
    public string ExceptionType { get; set; }
    public DateTime FailureTimestamp { get; set; }
}

public class SuspiciousActivityResult
{…