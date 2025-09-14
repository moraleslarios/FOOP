# MlResultActionsExecSelfIfFail - Ejecución Condicional en Caso de Fallo

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos ExecSelfIfFail](#métodos-execselfiffail)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Métodos TryExecSelfIfFail - Captura de Excepciones](#métodos-tryexecselfiffail---captura-de-excepciones)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Otros Métodos ExecSelf](#comparación-con-otros-métodos-execself)

---

## Introducción

Los métodos `ExecSelfIfFail` son operaciones especializadas que ejecutan acciones **únicamente cuando el `MlResult<T>` es fallido**, manteniendo el resultado original sin modificaciones. Estos métodos son complementarios a `ExecSelfIfValid` y proporcionan una forma limpia de manejar efectos secundarios específicos para casos de error.

### Propósito Principal

- **Logging de Errores**: Registrar información detallada solo cuando ocurren fallos
- **Notificaciones de Fallo**: Enviar alertas o notificaciones solo en casos de error
- **Métricas de Error**: Recopilar estadísticas específicas de fallos
- **Cleanup Condicional**: Realizar limpieza específica solo cuando algo falla
- **Debugging y Diagnóstico**: Ejecutar lógica de diagnóstico solo en errores

---

## Análisis de los Métodos

### Estructura y Filosofía

Los métodos `ExecSelfIfFail` implementan el patrón de **ejecución condicional en fallo**:

```
Resultado Exitoso → No acción → Resultado Exitoso (sin cambios)
      ↓                          ↓
Resultado Fallido → Acción → Resultado Fallido (sin cambios)
```

### Características Principales

1. **Ejecución Solo en Fallo**: Las acciones solo se ejecutan si el resultado es fallido
2. **Inmutabilidad**: El resultado original nunca se modifica
3. **Acceso a Detalles de Error**: Las acciones reciben `MlErrorsDetails` completo
4. **Transparencia**: No afectan el flujo principal de datos
5. **Soporte Asíncrono**: Todas las variantes asíncronas disponibles

---

## Métodos ExecSelfIfFail

### `ExecSelfIfFail<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es fallido, sin modificar el resultado original

```csharp
public static MlResult<T> ExecSelfIfFail<T>(this MlResult<T> source,
                                            Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionFail`: Acción a ejecutar solo si `source` es fallido (recibe los detalles del error)

**Comportamiento**:
- Si `source` es válido: Retorna `source` sin cambios, no ejecuta `actionFail`
- Si `source` es fallido: Ejecuta `actionFail(errorDetails)` y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = GetUser(userId)
    .ExecSelfIfFail(errors => 
        _logger.LogError($"Failed to get user {userId}: {errors.FirstErrorMessage}"));

// Si GetUser es exitoso: result contiene el usuario, no se loggeó nada
// Si GetUser falla: result contiene el error original + se loggeó el error
```

**Ejemplo con Múltiples Acciones en Fallo**:
```csharp
var result = ProcessPayment(paymentData)
    .ExecSelfIfFail(errors => _logger.LogError($"Payment failed: {errors.FirstErrorMessage}"))
    .ExecSelfIfFail(errors => _metrics.IncrementCounter("payment.failures"))
    .ExecSelfIfFail(errors => _notificationService.SendPaymentFailureAlert(paymentData.UserId));

// Todas las acciones se ejecutan solo si ProcessPayment falla
// El resultado original se mantiene intacto
```

---

## Variantes Asíncronas

### `ExecSelfIfFailAsync<T>()` - Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfIfFailAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task> actionFailAsync)
```

**Comportamiento**: Ejecuta una acción asíncrona solo si el resultado es fallido

**Ejemplo**:
```csharp
var result = await CreateOrder(orderData)
    .ExecSelfIfFailAsync(async errors => 
        await _notificationService.SendOrderFailureEmailAsync(orderData.CustomerId, errors));
```

### `ExecSelfFailAsync<T>()` - Fuente Asíncrona con Acción Asíncrona

```csharp
public static async Task<MlResult<T>> ExecSelfFailAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)
```

**Comportamiento**: Espera el resultado asíncrono y ejecuta la acción asíncrona solo si es fallido

**Ejemplo**:
```csharp
var result = await ProcessDocumentAsync(document)
    .ExecSelfFailAsync(async errors => 
        await _auditService.LogDocumentProcessingFailureAsync(document.Id, errors));
```

### `ExecSelfFailAsync<T>()` - Fuente Asíncrona con Acción Síncrona

```csharp
public static async Task<MlResult<T>> ExecSelfFailAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<MlErrorsDetails> actionFail)
```

**Comportamiento**: Espera el resultado asíncrono y ejecuta la acción síncrona solo si es fallido

**Ejemplo**:
```csharp
var result = await ValidateDataAsync(inputData)
    .ExecSelfFailAsync(errors => 
        _logger.LogWarning($"Validation failed: {string.Join(", ", errors.AllErrors)}"));
```

---

## Métodos TryExecSelfIfFail - Captura de Excepciones

### `TryExecSelfFail<T>()` - Versión Segura

```csharp
public static MlResult<T> TryExecSelfFail<T>(this MlResult<T> source,
                                             Action<MlErrorsDetails> actionFail,
                                             Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelfFail<T>(this MlResult<T> source,
                                             Action<MlErrorsDetails> actionFail,
                                             string errorMessage = null!)
```

**Comportamiento**: 
- Ejecuta `actionFail` solo si `source` es fallido
- Si `actionFail` lanza una excepción, captura la excepción y retorna un resultado fallido
- Si `source` es válido, no ejecuta la acción y retorna `source` sin cambios

**Ejemplo**:
```csharp
var result = ProcessTransaction(transaction)
    .TryExecSelfFail(
        errors => SendCriticalAlert(errors), // Puede lanzar excepción
        ex => $"Failed to send critical alert: {ex.Message}"
    );

// Si ProcessTransaction es exitoso: result contiene la transacción, no se envía alerta
// Si ProcessTransaction falla y SendCriticalAlert funciona: result contiene el error original
// Si ProcessTransaction falla y SendCriticalAlert lanza excepción: result contiene error de la excepción
```

### Versiones Asíncronas de TryExecSelfIfFail

#### `TryExecSelfIfFailAsync<T>()` - Acción Asíncrona Segura
```csharp
public static async Task<MlResult<T>> TryExecSelfIfFailAsync<T>(
    this MlResult<T> source,
    Func<MlErrorsDetails, Task> actionFailAsync,
    Func<Exception, string> errorMessageBuilder)
```

#### `TryExecSelfFailAsync<T>()` - Fuente Asíncrona con Acción Asíncrona Segura
```csharp
public static async Task<MlResult<T>> TryExecSelfFailAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<MlErrorsDetails, Task> actionFailAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Logging y Alertas de Error

```csharp
public class ErrorHandlingService
{
    private readonly ILogger _logger;
    private readonly IAlertService _alertService;
    private readonly IMetricsService _metrics;
    private readonly IAuditService _audit;
    
    public ErrorHandlingService(
        ILogger logger,
        IAlertService alertService,
        IMetricsService metrics,
        IAuditService audit)
    {
        _logger = logger;
        _alertService = alertService;
        _metrics = metrics;
        _audit = audit;
    }
    
    public async Task<MlResult<OrderResult>> ProcessOrderWithErrorHandlingAsync(OrderRequest request)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateOrderRequest(request)
            .ExecSelfIfFailAsync(async errors => 
                await LogValidationFailureAsync(correlationId, request, errors))
            .BindAsync(async validRequest => 
                await ProcessPaymentAsync(validRequest))
            .ExecSelfIfFailAsync(async errors => 
                await HandlePaymentFailureAsync(correlationId, request, errors))
            .BindAsync(async paidOrder => 
                await CreateOrderAsync(paidOrder))
            .ExecSelfIfFailAsync(async errors => 
                await HandleOrderCreationFailureAsync(correlationId, request, errors))
            .BindAsync(async createdOrder => 
                await FinalizeOrderAsync(createdOrder))
            .TryExecSelfIfFailAsync(async errors => 
                await HandleOrderFinalizationFailureAsync(correlationId, request, errors),
                ex => $"Failed to handle order finalization failure: {ex.Message}")
            .ExecSelfIfFailAsync(async errors => 
                await RecordOverallFailureMetricsAsync(correlationId, startTime, errors));
    }
    
    private async Task LogValidationFailureAsync(string correlationId, OrderRequest request, MlErrorsDetails errors)
    {
        var logEntry = new ErrorLogEntry
        {
            CorrelationId = correlationId,
            Stage = "Validation",
            RequestData = SerializeRequest(request),
            Errors = errors.AllErrors,
            Severity = ErrorSeverity.Warning,
            Timestamp = DateTime.UtcNow
        };
        
        await _logger.LogAsync(logEntry);
        await _metrics.IncrementCounterAsync("orders.validation_failures");
    }
    
    private async Task HandlePaymentFailureAsync(string correlationId, OrderRequest request, MlErrorsDetails errors)
    {
        // Log error with higher severity
        var logEntry = new ErrorLogEntry
        {
            CorrelationId = correlationId,
            Stage = "Payment",
            RequestData = SerializeRequest(request),
            Errors = errors.AllErrors,
            Severity = ErrorSeverity.Error,
            Timestamp = DateTime.UtcNow
        };
        
        await _logger.LogAsync(logEntry);
        
        // Record metrics
        await _metrics.IncrementCounterAsync("orders.payment_failures");
        await _metrics.RecordValueAsync("orders.payment_failure_amount", request.TotalAmount);
        
        // Send alert if amount is high
        if (request.TotalAmount > 1000)
        {
            await _alertService.SendHighValuePaymentFailureAlert(correlationId, request, errors);
        }
        
        // Audit for compliance
        await _audit.LogPaymentFailureAsync(correlationId, request.CustomerId, request.TotalAmount, errors);
    }
    
    private async Task HandleOrderCreationFailureAsync(string correlationId, OrderRequest request, MlErrorsDetails errors)
    {
        // Critical error - payment succeeded but order creation failed
        var logEntry = new ErrorLogEntry
        {
            CorrelationId = correlationId,
            Stage = "OrderCreation",
            RequestData = SerializeRequest(request),
            Errors = errors.AllErrors,
            Severity = ErrorSeverity.Critical,
            Timestamp = DateTime.UtcNow,
            RequiresImmediateAttention = true
        };
        
        await _logger.LogAsync(logEntry);
        
        // Critical metrics
        await _metrics.IncrementCounterAsync("orders.creation_failures_after_payment");
        
        // Immediate alert
        await _alertService.SendCriticalAlert(
            $"Order creation failed after successful payment",
            $"CorrelationId: {correlationId}, Amount: {request.TotalAmount:C}",
            errors);
        
        // Trigger compensation workflow
        await TriggerPaymentRefundWorkflow(correlationId, request);
    }
    
    private async Task HandleOrderFinalizationFailureAsync(string correlationId, OrderRequest request, MlErrorsDetails errors)
    {
        // Ultra-critical - order created but finalization failed
        var logEntry = new ErrorLogEntry
        {
            CorrelationId = correlationId,
            Stage = "OrderFinalization",
            RequestData = SerializeRequest(request),
            Errors = errors.AllErrors,
            Severity = ErrorSeverity.Critical,
            Timestamp = DateTime.UtcNow,
            RequiresImmediateAttention = true
        };
        
        await _logger.LogAsync(logEntry);
        
        // Ultra-critical metrics
        await _metrics.IncrementCounterAsync("orders.finalization_failures");
        
        // Escalated alert
        await _alertService.SendEscalatedAlert(
            "Order finalization failure - Manual intervention required",
            $"CorrelationId: {correlationId}, Customer: {request.CustomerId}",
            errors);
        
        // Create manual review task
        await CreateManualReviewTask(correlationId, request, errors);
    }
    
    private async Task RecordOverallFailureMetricsAsync(string correlationId, DateTime startTime, MlErrorsDetails errors)
    {
        var duration = DateTime.UtcNow - startTime;
        
        await _metrics.RecordTimingAsync("orders.failure_processing_time", duration);
        await _metrics.RecordValueAsync("orders.failure_error_count", errors.AllErrors.Length);
        
        // Categorize error types
        var errorCategories = CategorizeErrors(errors);
        foreach (var category in errorCategories)
        {
            await _metrics.IncrementCounterAsync($"orders.failures.{category}");
        }
    }
    
    // Métodos auxiliares
    private string SerializeRequest(OrderRequest request)
    {
        // Serializar request removiendo información sensible
        return JsonSerializer.Serialize(new 
        {
            request.CustomerId,
            request.TotalAmount,
            ItemCount = request.Items?.Count ?? 0,
            PaymentMethod = request.PaymentMethodId?.Substring(0, Math.Min(4, request.PaymentMethodId.Length)) + "***"
        });
    }
    
    private async Task TriggerPaymentRefundWorkflow(string correlationId, OrderRequest request)
    {
        try
        {
            var refundRequest = new RefundRequest
            {
                CorrelationId = correlationId,
                CustomerId = request.CustomerId,
                Amount = request.TotalAmount,
                Reason = "Order creation failed after payment"
            };
            
            await _paymentService.InitiateRefundAsync(refundRequest);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to trigger refund workflow for {correlationId}: {ex.Message}");
        }
    }
    
    private async Task CreateManualReviewTask(string correlationId, OrderRequest request, MlErrorsDetails errors)
    {
        try
        {
            var reviewTask = new ManualReviewTask
            {
                CorrelationId = correlationId,
                TaskType = "OrderFinalizationFailure",
                Priority = Priority.Critical,
                CustomerId = request.CustomerId,
                Amount = request.TotalAmount,
                ErrorDetails = errors.AllErrors,
                CreatedAt = DateTime.UtcNow
            };
            
            await _taskService.CreateReviewTaskAsync(reviewTask);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to create manual review task for {correlationId}: {ex.Message}");
        }
    }
    
    private List<string> CategorizeErrors(MlErrorsDetails errors)
    {
        var categories = new List<string>();
        
        foreach (var error in errors.AllErrors)
        {
            var lowerError = error.ToLower();
            
            if (lowerError.Contains("payment") || lowerError.Contains("card") || lowerError.Contains("transaction"))
                categories.Add("payment");
            else if (lowerError.Contains("validation") || lowerError.Contains("invalid") || lowerError.Contains("required"))
                categories.Add("validation");
            else if (lowerError.Contains("inventory") || lowerError.Contains("stock") || lowerError.Contains("available"))
                categories.Add("inventory");
            else if (lowerError.Contains("network") || lowerError.Contains("timeout") || lowerError.Contains("connection"))
                categories.Add("infrastructure");
            else
                categories.Add("unknown");
        }
        
        return categories.Distinct().ToList();
    }
    
    // Implementaciones de validación y procesamiento...
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        var errors = new List<string>();
        
        if (request.CustomerId <= 0)
            errors.Add("Valid customer ID is required");
            
        if (request.Items?.Any() != true)
            errors.Add("Order must contain at least one item");
            
        if (request.TotalAmount <= 0)
            errors.Add("Total amount must be positive");
            
        if (errors.Any())
            return MlResult<OrderRequest>.Fail(errors.ToArray());
            
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task<MlResult<PaymentResult>> ProcessPaymentAsync(OrderRequest request)
    {
        try
        {
            var paymentResult = await _paymentService.ProcessAsync(request);
            return paymentResult.Success 
                ? MlResult<PaymentResult>.Valid(paymentResult)
                : MlResult<PaymentResult>.Fail(paymentResult.ErrorMessage);
        }
        catch (Exception ex)
        {
            return MlResult<PaymentResult>.Fail($"Payment processing error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<CreatedOrder>> CreateOrderAsync(PaymentResult paymentResult)
    {
        try
        {
            var order = await _orderService.CreateAsync(paymentResult);
            return MlResult<CreatedOrder>.Valid(order);
        }
        catch (Exception ex)
        {
            return MlResult<CreatedOrder>.Fail($"Order creation error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<OrderResult>> FinalizeOrderAsync(CreatedOrder order)
    {
        try
        {
            var finalizedOrder = await _orderService.FinalizeAsync(order);
            return MlResult<OrderResult>.Valid(finalizedOrder);
        }
        catch (Exception ex)
        {
            return MlResult<OrderResult>.Fail($"Order finalization error: {ex.Message}");
        }
    }
}

// Clases de apoyo
public enum ErrorSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

public class ErrorLogEntry
{
    public string CorrelationId { get; set; }
    public string Stage { get; set; }
    public string RequestData { get; set; }
    public string[] Errors { get; set; }
    public ErrorSeverity Severity { get; set; }
    public bool RequiresImmediateAttention { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RefundRequest
{
    public string CorrelationId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
}

public class ManualReviewTask
{
    public string CorrelationId { get; set; }
    public string TaskType { get; set; }
    public Priority Priority { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string[] ErrorDetails { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderRequest
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethodId { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string ErrorMessage { get; set; }
}

public class CreatedOrder
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderResult
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public DateTime CompletedAt { get; set; }
}

// Interfaces de servicios
public interface IAlertService
{
    Task SendHighValuePaymentFailureAlert(string correlationId, OrderRequest request, MlErrorsDetails errors);
    Task SendCriticalAlert(string title, string description, MlErrorsDetails errors);
    Task SendEscalatedAlert(string title, string description, MlErrorsDetails errors);
}

public interface IMetricsService
{
    Task IncrementCounterAsync(string counterName);
    Task RecordValueAsync(string metricName, decimal value);
    Task RecordTimingAsync(string timerName, TimeSpan duration);
}

public interface IAuditService
{
    Task LogPaymentFailureAsync(string correlationId, int customerId, decimal amount, MlErrorsDetails errors);
}

public interface IPaymentService
{
    Task<PaymentResult> ProcessAsync(OrderRequest request);
    Task InitiateRefundAsync(RefundRequest request);
}

public interface IOrderService
{
    Task<CreatedOrder> CreateAsync(PaymentResult paymentResult);
    Task<OrderResult> FinalizeAsync(CreatedOrder order);
}

public interface ITaskService
{
    Task CreateReviewTaskAsync(ManualReviewTask task);
}
```

### Ejemplo 2: Sistema de Monitoreo de Fallos con Circuit Breaker

```csharp
public class CircuitBreakerMonitoringService
{
    private readonly ICircuitBreakerService _circuitBreaker;
    private readonly IHealthMonitor _healthMonitor;
    private readonly ILogger _logger;
    private readonly IMetricsCollector _metrics;
    
    public CircuitBreakerMonitoringService(
        ICircuitBreakerService circuitBreaker,
        IHealthMonitor healthMonitor,
        ILogger logger,
        IMetricsCollector metrics)
    {
        _circuitBreaker = circuitBreaker;
        _healthMonitor = healthMonitor;
        _logger = logger;
        _metrics = metrics;
    }
    
    public async Task<MlResult<ApiResponse>> CallExternalServiceWithMonitoringAsync(string serviceName, ApiRequest request)
    {
        var callId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await _circuitBreaker.ExecuteAsync(serviceName, () => CallExternalServiceAsync(request))
            .ExecSelfIfFailAsync(async errors => 
                await LogServiceCallFailureAsync(serviceName, callId, request, errors))
            .ExecSelfIfFailAsync(async errors => 
                await UpdateFailureMetricsAsync(serviceName, startTime, errors))
            .TryExecSelfIfFailAsync(async errors => 
                await CheckAndUpdateCircuitBreakerStateAsync(serviceName, errors),
                ex => $"Failed to update circuit breaker state: {ex.Message}")
            .TryExecSelfIfFailAsync(async errors => 
                await NotifyHealthMonitorAsync(serviceName, errors),
                ex => $"Failed to notify health monitor: {ex.Message}")
            .ExecSelfIfFailAsync(async errors => 
                await RecordServiceDegradationAsync(serviceName, errors));
    }
    
    public async Task<MlResult<DatabaseResult>> ExecuteDatabaseOperationWithMonitoringAsync(string operation, DatabaseQuery query)
    {
        var operationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ExecuteDatabaseQueryAsync(query)
            .ExecSelfIfFailAsync(async errors => 
                await LogDatabaseFailureAsync(operation, operationId, query, errors))
            .ExecSelfIfFailAsync(async errors => 
                await AnalyzeAndCategorizeDbErrorAsync(operation, errors))
            .TryExecSelfIfFailAsync(async errors => 
                await CheckDatabaseHealthAsync(operation, errors),
                ex => $"Failed to check database health: {ex.Message}")
            .ExecSelfIfFailAsync(async errors => 
                await UpdateDatabaseMetricsAsync(operation, startTime, errors))
            .TryExecSelfIfFailAsync(async errors => 
                await TriggerDatabaseAlertsIfNeededAsync(operation, errors),
                ex => $"Failed to trigger database alerts: {ex.Message}");
    }
    
    private async Task LogServiceCallFailureAsync(string serviceName, string callId, ApiRequest request, MlErrorsDetails errors)
    {
        var logEntry = new ServiceCallLogEntry
        {
            ServiceName = serviceName,
            CallId = callId,
            RequestPath = request.Path,
            RequestMethod = request.Method,
            Errors = errors.AllErrors,
            Timestamp = DateTime.UtcNow,
            Severity = DetermineErrorSeverity(errors)
        };
        
        await _logger.LogServiceCallFailureAsync(logEntry);
    }
    
    private async Task UpdateFailureMetricsAsync(string serviceName, DateTime startTime, MlErrorsDetails errors)
    {
        var duration = DateTime.UtcNow - startTime;
        
        // Métricas básicas de fallo
        await _metrics.IncrementCounterAsync($"service.{serviceName}.failures");
        await _metrics.RecordTimingAsync($"service.{serviceName}.failure_time", duration);
        
        // Categorizar errores para métricas específicas
        foreach (var error in errors.AllErrors)
        {
            var category = CategorizeServiceError(error);
            await _metrics.IncrementCounterAsync($"service.{serviceName}.failures.{category}");
        }
        
        // Calcular tasa de fallo actual
        var currentFailureRate = await CalculateCurrentFailureRate(serviceName);
        await _metrics.RecordGaugeAsync($"service.{serviceName}.failure_rate", currentFailureRate);
    }
    
    private async Task CheckAndUpdateCircuitBreakerStateAsync(string serviceName, MlErrorsDetails errors)
    {
        var currentState = await _circuitBreaker.GetStateAsync(serviceName);
        var errorSeverity = DetermineErrorSeverity(errors);
        
        // Actualizar estado del circuit breaker basado en la severidad del error
        if (errorSeverity == ErrorSeverity.Critical)
        {
            await _circuitBreaker.TripAsync(serviceName, "Critical error detected");
            await _logger.LogWarningAsync($"Circuit breaker tripped for {serviceName} due to critical error");
        }
        else if (errorSeverity == ErrorSeverity.Error)
        {
            await _circuitBreaker.RecordFailureAsync(serviceName);
        }
        
        // Registrar cambios de estado
        var newState = await _circuitBreaker.GetStateAsync(serviceName);
        if (currentState != newState)
        {
            await _logger.LogInformationAsync($"Circuit breaker state changed for {serviceName}: {currentState} -> {newState}");
            await _metrics.IncrementCounterAsync($"circuit_breaker.{serviceName}.state_changes");
        }
    }
    
    private async Task NotifyHealthMonitorAsync(string serviceName, MlErrorsDetails errors)
    {
        var healthStatus = new ServiceHealthStatus
        {
            ServiceName = serviceName,
            Status = HealthStatus.Unhealthy,
            LastError = errors.FirstErrorMessage,
            ErrorCount = errors.AllErrors.Length,
            Timestamp = DateTime.UtcNow
        };
        
        await _healthMonitor.UpdateServiceHealthAsync(healthStatus);
        
        // Si es un servicio crítico, elevar la alerta
        if (IsCriticalService(serviceName))
        {
            await _healthMonitor.TriggerCriticalServiceAlert(serviceName, errors);
        }
    }
    
    private async Task RecordServiceDegradationAsync(string serviceName, MlErrorsDetails errors)
    {
        var degradationEvent = new ServiceDegradationEvent
        {
            ServiceName = serviceName,
            EventType = "ServiceCallFailure",
            Severity = DetermineErrorSeverity(errors),
            ErrorDetails = errors.AllErrors,
            Timestamp = DateTime.UtcNow,
            AffectedOperations = ExtractAffectedOperations(errors)
        };
        
        await _metrics.RecordDegradationEventAsync(degradationEvent);
        
        // Comprobar si necesitamos activar modo de degradación
        var recentFailures = await GetRecentFailureCount(serviceName, TimeSpan.FromMinutes(5));
        if (recentFailures > 10)
        {
            await ActivateDegradedModeAsync(serviceName, "High failure rate detected");
        }
    }
    
    private async Task LogDatabaseFailureAsync(string operation, string operationId, DatabaseQuery query, MlErrorsDetails errors)
    {
        var logEntry = new DatabaseOperationLogEntry
        {
            Operation = operation,
            OperationId = operationId,
            QueryType = query.QueryType,
            TableName = query.TableName,
            Errors = errors.AllErrors,
            Timestamp = DateTime.UtcNow,
            Severity = DetermineDatabaseErrorSeverity(errors)
        };
        
        await _logger.LogDatabaseFailureAsync(logEntry);
    }
    
    private async Task AnalyzeAndCategorizeDbErrorAsync(string operation, MlErrorsDetails errors)
    {
        foreach (var error in errors.AllErrors)
        {
            var category = CategorizeDatabaseError(error);
            await _metrics.IncrementCounterAsync($"database.{operation}.errors.{category}");
            
            // Detectar patrones específicos
            if (IsDeadlockError(error))
            {
                await _metrics.IncrementCounterAsync("database.deadlocks");
                await _logger.LogWarningAsync($"Deadlock detected in operation {operation}: {error}");
            }
            else if (IsTimeoutError(error))
            {
                await _metrics.IncrementCounterAsync("database.timeouts");
                await CheckDatabasePerformanceAsync();
            }
            else if (IsConnectionError(error))
            {
                await _metrics.IncrementCounterAsync("database.connection_errors");
                await CheckDatabaseConnectivityAsync();
            }
        }
    }
    
    private async Task CheckDatabaseHealthAsync(string operation, MlErrorsDetails errors)
    {
        var healthCheck = new DatabaseHealthCheck
        {
            Operation = operation,
            ErrorDetails = errors,
            Timestamp = DateTime.UtcNow
        };
        
        var healthResult = await _healthMonitor.CheckDatabaseHealthAsync(healthCheck);
        
        if (!healthResult.IsHealthy)
        {
            await _logger.LogErrorAsync($"Database health check failed for operation {operation}");
            await _metrics.RecordGaugeAsync("database.health_score", healthResult.HealthScore);
            
            if (healthResult.HealthScore < 0.5) // Less than 50% healthy
            {
                await TriggerDatabaseMaintenanceAlert();
            }
        }
    }
    
    private async Task UpdateDatabaseMetricsAsync(string operation, DateTime startTime, MlErrorsDetails errors)
    {
        var duration = DateTime.UtcNow - startTime;
        
        await _metrics.IncrementCounterAsync($"database.{operation}.failures");
        await _metrics.RecordTimingAsync($"database.{operation}.error_time", duration);
        
        // Métricas de error por tabla si es aplicable
        var tableNames = ExtractTableNamesFromErrors(errors);
        foreach (var tableName in tableNames)
        {
            await _metrics.IncrementCounterAsync($"database.table.{tableName}.errors");
        }
    }
    
    private async Task TriggerDatabaseAlertsIfNeededAsync(string operation, MlErrorsDetails errors)
    {
        var errorSeverity = DetermineDatabaseErrorSeverity(errors);
        
        if (errorSeverity == ErrorSeverity.Critical)
        {
            await TriggerCriticalDatabaseAlert(operation, errors);
        }
        
        // Comprobar si hay patrones de error que requieren atención
        var errorPattern = await AnalyzeErrorPatternAsync(operation, errors);
        if (errorPattern.RequiresAlert)
        {
            await TriggerPatternBasedAlert(operation, errorPattern);
        }
    }
    
    // Métodos auxiliares
    private ErrorSeverity DetermineErrorSeverity(MlErrorsDetails errors)
    {
        foreach (var error in errors.AllErrors)
        {
            var lowerError = error.ToLower();
            
            if (lowerError.Contains("critical") || lowerError.Contains("fatal") || lowerError.Contains("severe"))
                return ErrorSeverity.Critical;
            if (lowerError.Contains("timeout") || lowerError.Contains("connection") || lowerError.Contains("unavailable"))
                return ErrorSeverity.Error;
        }
        
        return ErrorSeverity.Warning;
    }
    
    private string CategorizeServiceError(string error)
    {
        var lowerError = error.ToLower();
        
        if (lowerError.Contains("timeout"))
            return "timeout";
        if (lowerError.Contains("connection") || lowerError.Contains("network"))
            return "connectivity";
        if (lowerError.Contains("authentication") || lowerError.Contains("authorization"))
            return "auth";
        if (lowerError.Contains("rate limit") || lowerError.Contains("throttle"))
            return "rate_limit";
        if (lowerError.Contains("server error") || lowerError.Contains("500"))
            return "server_error";
        
        return "unknown";
    }
    
    private bool IsCriticalService(string serviceName)
    {
        var criticalServices = new[] { "payment", "user-auth", "order-processing", "inventory" };
        return criticalServices.Contains(serviceName.ToLower());
    }
    
    private bool IsDeadlockError(string error) => error.ToLower().Contains("deadlock");
    private bool IsTimeoutError(string error) => error.ToLower().Contains("timeout");
    private bool IsConnectionError(string error) => error.ToLower().Contains("connection");
    
    // Implementaciones adicionales...
    private async Task<double> CalculateCurrentFailureRate(string serviceName) => 0.1; // Implementación simplificada
    private async Task<int> GetRecentFailureCount(string serviceName, TimeSpan timeSpan) => 5; // Implementación simplificada
    private async Task ActivateDegradedModeAsync(string serviceName, string reason) { } // Implementación simplificada
    private List<string> ExtractAffectedOperations(MlErrorsDetails errors) => new(); // Implementación simplificada
    private List<string> ExtractTableNamesFromErrors(MlErrorsDetails errors) => new(); // Implementación simplificada
    private async Task CheckDatabasePerformanceAsync() { } // Implementación simplificada
    private async Task CheckDatabaseConnectivityAsync() { } // Implementación simplificada
    private async Task TriggerDatabaseMaintenanceAlert() { } // Implementación simplificada
    private async Task TriggerCriticalDatabaseAlert(string operation, MlErrorsDetails errors) { } // Implementación simplificada
    private async Task<ErrorPattern> AnalyzeErrorPatternAsync(string operation, MlErrorsDetails errors) => new(); // Implementación simplificada
    private async Task TriggerPatternBasedAlert(string operation, ErrorPattern pattern) { } // Implementación simplificada
}

// Clases de apoyo adicionales
public class ServiceCallLogEntry
{
    public string ServiceName { get; set; }
    public string CallId { get; set; }
    public string RequestPath { get; set; }
    public string RequestMethod { get; set; }
    public string[] Errors { get; set; }
    public DateTime Timestamp { get; set; }
    public ErrorSeverity Severity { get; set; }
}

public class DatabaseOperationLogEntry
{
    public string Operation { get; set; }
    public string OperationId { get; set; }
    public string QueryType { get; set; }
    public string TableName { get; set; }
    public string[] Errors { get; set; }
    public DateTime Timestamp { get; set; }
    public ErrorSeverity Severity { get; set; }
}

public class ServiceHealthStatus
{
    public string ServiceName { get; set; }
    public HealthStatus Status { get; set; }
    public string LastError { get; set; }
    public int ErrorCount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ServiceDegradationEvent
{
    public string ServiceName { get; set; }
    public string EventType { get; set; }
    public ErrorSeverity Severity { get; set; }
    public string[] ErrorDetails { get; set; }
    public DateTime Timestamp { get; set; }
    public List<string> AffectedOperations { get; set; }
}

public class DatabaseHealthCheck
{
    public string Operation { get; set; }
    public MlErrorsDetails ErrorDetails { get; set; }
    public DateTime Timestamp { get; set; }
}

public class DatabaseHealthResult
{
    public bool IsHealthy { get; set; }
    public double HealthScore { get; set; }
}

public class ErrorPattern
{
    public bool RequiresAlert { get; set; }
    public string PatternType { get; set; }
    public string Description { get; set; }
}

public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}

public class ApiRequest
{
    public string Path { get; set; }
    public string Method { get; set; }
}

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Content { get; set; }
}

public class DatabaseQuery
{
    public string QueryType { get; set; }
    public string TableName { get; set; }
    public string SqlText { get; set; }
}

public class DatabaseResult
{
    public object Data { get; set; }
    public int RowsAffected { get; set; }
}

// Interfaces adicionales
public interface ICircuitBreakerService
{
    Task<MlResult<T>> ExecuteAsync<T>(string serviceName, Func<Task<T>> operation);
    Task<string> GetStateAsync(string serviceName);
    Task TripAsync(string serviceName, string reason);
    Task RecordFailureAsync(string serviceName);
}

public interface IHealthMonitor
{
    Task UpdateServiceHealthAsync(ServiceHealthStatus status);
    Task TriggerCriticalServiceAlert(string serviceName, MlErrorsDetails errors);
    Task<DatabaseHealthResult> CheckDatabaseHealthAsync(DatabaseHealthCheck healthCheck);
}

public interface IMetricsCollector
{
    Task IncrementCounterAsync(string counterName);
    Task RecordTimingAsync(string timerName, TimeSpan duration);
    Task RecordGaugeAsync(string gaugeName, double value);
    Task RecordDegradationEventAsync(ServiceDegradationEvent degradationEvent);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar ExecSelfIfFail

```csharp
// ✅ Correcto: Logging específico de errores
var result = ProcessData(data)
    .ExecSelfIfFail(errors => 
        _logger.LogError($"Processing failed: {string.Join("; ", errors.AllErrors)}"));

// ✅ Correcto: Métricas de fallo específicas
var result = CallExternalAPI(request)
    .ExecSelfIfFail(errors => _metrics.IncrementCounter("api.failures"))
    .ExecSelfIfFail(errors => _metrics.RecordErrorDetails(errors));

// ✅ Correcto: Alertas condicionales basadas en tipo de error
var result = ProcessPayment(payment)
    .ExecSelfIfFail(errors => 
    {
        if (errors.AllErrors.Any(e => e.Contains("fraud")))
            _alertService.SendFraudAlert(payment.CustomerId);
    });

// ❌ Incorrecto: Usar para lógica que siempre debe ejecutarse
var result = SaveData(data)
    .ExecSelfIfFail(errors => CleanupResources()); // Esto NO se ejecuta si SaveData es exitoso
```

### 2. Combinando con Otros Métodos ExecSelf

```csharp
// ✅ Correcto: Logging completo de éxito y fallo
var result = ProcessOrder(order)
    .ExecSelfIfValid(success => 
        _logger.LogInformation($"Order {success.Id} processed successfully"))
    .ExecSelfIfFail(errors => 
        _logger.LogError($"Order processing failed: {errors.FirstErrorMessage}"));

// ✅ Correcto: Métricas diferenciadas
var result = ValidateUser(userData)
    .ExecSelfIfValid(user => _metrics.IncrementCounter("user.validation.success"))
    .ExecSelfIfFail(errors => _metrics.IncrementCounter("user.validation.failure"));

// ✅ Correcto: Cleanup condicional
var result = ProcessDocument(document)
    .ExecSelfIfValid(processed => SaveToCache(processed))
    .ExecSelfIfFail(errors => CleanupTempFiles(document.TempFiles));
```

### 3. Manejo de Excepciones en Acciones de Fallo

```csharp
// ✅ Correcto: Usar TryExecSelfIfFail para acciones que pueden fallar
var result = ProcessTransaction(transaction)
    .TryExecSelfIfFail(
        errors => SendCriticalAlert(errors), // Puede lanzar excepción
        ex => $"Failed to send alert: {ex.Message}"
    );

// ✅ Correcto: Logging defensivo en acciones de fallo
var result = SaveUser(user)
    .ExecSelfIfFail(errors => 
    {
        try
        {
            _notificationService.SendFailureNotification(user.Id, errors);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to send failure notification: {ex.Message}");
        }
    });

// ❌ Incorrecto: No manejar excepciones en acciones críticas
var result = ProcessPayment(payment)
    .ExecSelfIfFail(errors => CriticalAuditLog(errors)); // Si falla, se pierde la excepción
```

### 4. Categorización y Análisis de Errores

```csharp
// ✅ Correcto: Análisis detallado de errores
var result = CallExternalService(request)
    .ExecSelfIfFail(errors => 
    {
        var categorizedErrors = CategorizeErrors(errors);
        foreach (var category in categorizedErrors)
        {
            _metrics.IncrementCounter($"service.errors.{category.Key}");
            if (category.Value > 5) // Más de 5 errores de este tipo
            {
                _alertService.SendCategoryAlert(category.Key, category.Value);
            }
        }
    });

// ✅ Correcto: Escalamiento basado en severidad
var result = ProcessCriticalOperation(operation)
    .ExecSelfIfFail(errors => 
    {
        var severity = DetermineErrorSeverity(errors);
        switch (severity)
        {
            case ErrorSeverity.Warning:
                _logger.LogWarning($"Operation warning: {errors.FirstErrorMessage}");
                break;
            case ErrorSeverity.Error:
                _logger.LogError($"Operation error: {errors.FirstErrorMessage}");
                _alertService.SendErrorAlert(operation.Id, errors);
                break;
            case ErrorSeverity.Critical:
                _logger.LogCritical($"Critical operation failure: {errors.FirstErrorMessage}");
                _alertService.SendCriticalAlert(operation.Id, errors);
                _escalationService.TriggerEmergencyResponse(operation.Id);
                break;
        }
    });
```

### 5. Integración con Sistemas de Monitoreo

```csharp
// ✅ Correcto: Integración completa con sistemas de observabilidad
public async Task<MlResult<ProcessResult>> ProcessWithFullObservabilityAsync(ProcessRequest request)
{
    using var activity = _telemetry.StartActivity("ProcessRequest");
    var correlationId = Guid.NewGuid().ToString();
    
    return await ValidateRequest(request)
        .ExecSelfIfFailAsync(async errors => 
        {
            activity?.SetTag("validation_failed", "true");
            await _telemetry.RecordErrorAsync("validation_failure", errors, correlationId);
            await _metrics.IncrementCounterAsync("process.validation_failures");
        })
        .BindAsync(async valid => await ProcessBusinessLogic(valid))
        .ExecSelfIfFailAsync(async errors => 
        {
            activity?.SetTag("processing_failed", "true");
            await _telemetry.RecordErrorAsync("processing_failure", errors, correlationId);
            await _metrics.IncrementCounterAsync("process.business_failures");
            await CheckForSystemDegradation(errors);
        })
        .ExecSelfIfFailAsync(async errors => 
        {
            await _logger.LogErrorAsync($"Process {correlationId} failed: {errors.FirstErrorMessage}");
            activity?.SetStatus(ActivityStatusCode.Error, errors.FirstErrorMessage);
        });
}
```

---

## Comparación con Otros Métodos ExecSelf

### Tabla Comparativa

| Método | Cuándo Ejecuta Acción | Resultado Retornado | Uso Principal |
|--------|----------------------|---------------------|---------------|
| `ExecSelf` | Siempre (con acciones diferentes para éxito/fallo) | Original sin cambios | Logging completo, métricas diferenciadas |
| `ExecSelfIfValid` | Solo si es válido | Original sin cambios | Efectos secundarios de éxito (cache, notificaciones) |
| `ExecSelfIfFail` | Solo si es fallido | Original sin cambios | Manejo específico de errores, alertas, logging de fallos |

### Patrón de Uso Combinado

```csharp
public async Task<MlResult<OrderResult>> ProcessOrderWithCompleteMonitoringAsync(OrderRequest request)
{
    var startTime = DateTime.UtcNow;
    var correlationId = Guid.NewGuid().ToString();
    
    return await ValidateOrder(request)
        .ExecSelfIfValid(validOrder => 
            _logger.LogInformation($"Order {correlationId} validation successful"))
        .ExecSelfIfFail(errors => 
            _logger.LogWarning($"Order {correlationId} validation failed: {errors.FirstErrorMessage}"))
        .BindAsync(async valid => await ProcessPayment(valid))
        .ExecSelfIfValid(paidOrder => 
            _metrics.IncrementCounter("orders.payment_success"))
        .ExecSelfIfFail(errors => 
        {
            _metrics.IncrementCounter("orders.payment_failure");
            if (errors.AllErrors.Any(e => e.Contains("fraud")))
                _alertService.SendFraudAlert(request.CustomerId);
        })
        .BindAsync(async paid => await FinalizeOrder(paid))
        .ExecSelfIfValid(finalOrder => 
        {
            _notificationService.SendOrderConfirmation(request.CustomerId, finalOrder.Id);
            _cache.Set($"order:{finalOrder.Id}", finalOrder);
        })
        .ExecSelfIfFail(errors => 
        {
            _logger.LogError($"Order {correlationId} finalization failed: {errors.FirstErrorMessage}");
            _compensationService.TriggerPaymentRefund(correlationId);
        })
        .ExecSelf(
            finalOrder => 
            {
                var duration = DateTime.UtcNow - startTime;
                _metrics.RecordTiming("orders.total_processing_time", duration);
                _logger.LogInformation($"Order {correlationId} completed in {duration.TotalMilliseconds}ms");
            },
            errors => 
            {
                var duration = DateTime.UtcNow - startTime;
                _metrics.RecordTiming("orders.failed_processing_time", duration);
                _logger.LogError($"Order {correlationId} failed after {duration.TotalMilliseconds}ms");
            }
        );
}
```

---

## Consideraciones de Rendimiento

### Ejecución Condicional

- `ExecSelfIfFail` solo ejecuta cuando hay fallos, no añade overhead en casos de éxito
- Ideal para logging de errores y alertas que no deben impactar el flujo feliz
- Las acciones de fallo suelen ser menos críticas en términos de performance

### Manejo de Excepciones

- Las versiones `TryExecSelfIfFail` tienen overhead adicional por manejo de excepciones
- Solo usar cuando las acciones de fallo pueden lanzar excepciones
- Considerar que las acciones de fallo fallidas pueden enmascarar el error original

### Operaciones de Logging y Alertas

- Las acciones de fallo suelen involucrar I/O (logging, alertas, métricas)
- Considerar el uso de operaciones asíncronas para no bloquear el hilo principal
- Implementar timeouts en acciones de fallo para evitar que ralenticen el sistema

---

## Resumen

Los métodos `ExecSelfIfFail` proporcionan una forma elegante de manejar efectos secundarios específicos para casos de error:

- **`ExecSelfIfFail`**: Ejecuta acciones solo cuando el resultado es fallido
- **`ExecSelfIfFailAsync`**: Soporte completo para operaciones asíncronas
- **`TryExecSelfIfFail`**: Versiones seguras que capturan excepciones

**Casos de uso ideales**:
- **Logging específico de errores** sin contaminar logs de éxito
- **Alertas y notificaciones** que solo deben enviarse en fallos
- **Métricas de error** separadas de métricas de éxito
- **Cleanup condicional** que solo aplica a casos de fallo
- **Análisis y categorización** de errores para observabilidad

**Ventajas principales**:
- **Claridad de intención**: Es obvio que la acción solo se ejecuta en fallos
- **Eficiencia**: No hay overhead en casos de éxito
- **Inmutabilidad**: El resultado original nunca se modifica
- **Composabilidad**: Se combina perfectamente con otros métodos ExecSelf

La clave está en usar `ExecSelfIfFail` para lógica que **específicamente** maneja casos de error, manteniendo el código de manejo de errores separado y