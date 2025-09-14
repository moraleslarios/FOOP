# MlResultActionsBindSaveValueAndExecSelf - Operaciones de Preservación de Contexto y Ejecución con Retorno

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindSaveValueInDetailsIfFaildFuncResult](#métodos-bindsavevalueindetailsiffaildFuncresult)
4. [Métodos ExecSelf](#métodos-execself)
5. [Métodos TryBindSaveValueInDetailsIfFaildFuncResult](#métodos-trybindsavevalueindetailsiffaildFuncresult)
6. [Métodos TryExecSelf](#métodos-tryexecself)
7. [Variantes Asíncronas](#variantes-asíncronas)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBindSaveValueAndExecSelf` contiene operaciones especializadas que se enfocan en **preservar el contexto** y **ejecutar acciones sin cambiar el resultado**. Estas operaciones son fundamentales para:

- **Preservación de Contexto**: Guardar el valor original cuando las operaciones subsiguientes fallan
- **Ejecución de Efectos Secundarios**: Realizar acciones (logging, auditoría) sin modificar el resultado
- **Debugging y Diagnóstico**: Mantener información valiosa para el análisis de errores

### Propósito Principal

- **BindSaveValueInDetailsIfFaildFuncResult**: Preserva el valor original en los detalles del error si la función falla
- **ExecSelf**: Ejecuta acciones basadas en el estado del resultado pero retorna el resultado original sin modificaciones
- **Manejo Seguro**: Versiones `Try*` que capturan excepciones sin perder el contexto

---

## Análisis de la Clase

### Estructura y Filosofía

Esta clase implementa dos patrones importantes:

1. **Preservación de Contexto**: Mantener información valiosa cuando las operaciones fallan
2. **Ejecución Transparente**: Realizar acciones sin afectar el flujo de datos

```
Resultado Exitoso → Función → [Éxito: Nuevo Resultado] | [Fallo: Error + Contexto Original]
      ↓                ↓               ↓                          ↓
Resultado Fallido → [No ejecuta] → Propaga Error Original   N/A
```

### Características Principales

1. **Contexto Preservado**: Los valores originales se mantienen en caso de fallo
2. **Ejecución Condicional**: Las acciones se ejecutan según el estado del resultado
3. **Transparencia**: `ExecSelf` no modifica el resultado, solo ejecuta efectos secundarios
4. **Debugging Mejorado**: Información adicional disponible para diagnóstico

---

## Métodos BindSaveValueInDetailsIfFaildFuncResult

### `BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Ejecuta una función si el resultado es válido, pero si la función falla, preserva el valor original en los detalles del error

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `func`: Función que se ejecuta si `source` es válido

**Comportamiento**:
- Si `source` es fallido: Propaga el error original
- Si `source` es válido y `func(value)` es exitoso: Retorna el resultado de `func`
- Si `source` es válido y `func(value)` falla: Retorna el error de `func` **más** el valor original en los detalles

**Ejemplo Básico**:
```csharp
var user = MlResult<User>.Valid(new User { Id = 123, Name = "John Doe" });

var result = user.BindSaveValueInDetailsIfFaildFuncResult(validUser => 
{
    // Esta operación falla
    return MlResult<ProcessedUser>.Fail("Processing failed");
});

// result contiene:
// - Error: "Processing failed"
// - Detalles adicionales: User { Id = 123, Name = "John Doe" }
// Esto permite saber qué datos se estaban procesando cuando falló
```

### Versiones Asíncronas

#### `BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>()`
```csharp
// Función asíncrona desde resultado síncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

// Función asíncrona desde resultado asíncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

// Función síncrona desde resultado asíncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TReturn>> func)
```

**Ejemplo Asíncrono**:
```csharp
var userData = await GetUserDataAsync(userId);

var result = await userData.BindSaveValueInDetailsIfFaildFuncResultAsync(async validData => 
{
    // Operación asíncrona que puede fallar
    var processed = await ProcessUserDataAsync(validData);
    
    if (processed == null)
        return MlResult<ProcessedUserData>.Fail("External service returned null");
        
    return MlResult<ProcessedUserData>.Valid(processed);
});

// Si falla, tendremos tanto el error como los datos originales del usuario
```

---

## Métodos ExecSelf

### `ExecSelf<T>()`

**Propósito**: Ejecuta diferentes acciones según el estado del resultado, pero siempre retorna el resultado original sin modificaciones

```csharp
public static MlResult<T> ExecSelf<T>(this MlResult<T> source, 
                                      Action<T> actionValid,
                                      Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionValid`: Acción que se ejecuta si `source` es válido (recibe el valor)
- `actionFail`: Acción que se ejecuta si `source` es fallido (recibe los errores)

**Comportamiento**:
- Si `source` es válido: Ejecuta `actionValid(value)` y retorna `source` sin cambios
- Si `source` es fallido: Ejecuta `actionFail(errorDetails)` y retorna `source` sin cambios
- **Siempre retorna el resultado original**

**Ejemplo Básico**:
```csharp
var result = GetUserData(userId)
    .ExecSelf(
        validUser => _logger.LogInformation($"User {validUser.Id} retrieved successfully"),
        errorDetails => _logger.LogError($"Failed to retrieve user {userId}: {errorDetails.FirstErrorMessage}")
    )
    .Bind(user => ProcessUser(user));

// ExecSelf no modifica el resultado, solo ejecuta logging
// El resultado continúa siendo exactamente el mismo que GetUserData devolvió
```

### Versiones Asíncronas del ExecSelf

#### `ExecSelfAsync<T>()` - Todas las Combinaciones
```csharp
// Ambas acciones asíncronas desde resultado síncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(this MlResult<T>                 source,
                                                            Func<T, Task>               actionValidAsync,
                                                            Func<MlErrorsDetails, Task> actionFailAsync)

// Ambas acciones asíncronas desde resultado asíncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Acción válida síncrona, acción fallo asíncrona
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<T> actionValid,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Acción válida asíncrona, acción fallo síncrona
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync,
    Action<MlErrorsDetails> actionFail)

// Ambas acciones síncronas desde resultado asíncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<T> actionValid,
    Action<MlErrorsDetails> actionFail)
```

**Ejemplo Asíncrono**:
```csharp
var result = await GetUserDataAsync(userId)
    .ExecSelfAsync(
        async validUser => await _auditService.LogUserAccessAsync(validUser.Id, "Retrieved"),
        async errorDetails => await _alertService.NotifyFailureAsync($"User retrieval failed: {errorDetails.FirstErrorMessage}")
    );

// El resultado sigue siendo exactamente el mismo, pero se ejecutaron acciones asíncronas
```

---

## Métodos TryBindSaveValueInDetailsIfFaildFuncResult

### `TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Versión segura de `BindSaveValueInDetailsIfFaildFuncResult` que captura excepciones

```csharp
public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    string errorMessage = null!)
```

**Comportamiento**:
- Igual que `BindSaveValueInDetailsIfFaildFuncResult` pero captura excepciones
- Si `func` lanza una excepción, la convierte en error y preserva el valor original
- El valor original se guarda tanto si `func` retorna error como si lanza excepción

**Ejemplo**:
```csharp
var user = MlResult<User>.Valid(new User { Id = 123, Name = "John Doe" });

var result = user.TryBindSaveValueInDetailsIfFaildFuncResult(
    validUser => 
    {
        // Esta función puede lanzar excepciones
        if (validUser.Name.Contains("invalid"))
            throw new ArgumentException("Invalid user name format");
            
        return ProcessUser(validUser);
    },
    ex => $"User processing failed with exception: {ex.Message}"
);

// Si lanza excepción, el resultado incluirá:
// - Error: "User processing failed with exception: Invalid user name format"
// - Detalles: User { Id = 123, Name = "John Doe" }
```

### Versiones Asíncronas de TryBindSaveValueInDetailsIfFaildFuncResult

Todas las combinaciones están disponibles:
- Funciones síncronas y asíncronas
- Resultados síncronos y asíncronos
- Con constructor de mensaje de error y mensaje simple

```csharp
public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Métodos TryExecSelf

### `TryExecSelf<T>()`

**Propósito**: Versión segura de `ExecSelf` que captura excepciones en las acciones

```csharp
public static MlResult<T> TryExecSelf<T>(this MlResult<T> source,
                                         Action<T> actionValid,
                                         Action<MlErrorsDetails> actionFail,
                                         Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelf<T>(this MlResult<T> source,
                                         Action<T> actionValid,
                                         Action<MlErrorsDetails> actionFail,
                                         string errorMessage = null!)
```

**Comportamiento**:
- Si `source` es válido y `actionValid` no lanza excepción: Retorna `source` original
- Si `source` es válido y `actionValid` lanza excepción: Retorna error (se pierde el resultado original)
- Si `source` es fallido y `actionFail` no lanza excepción: Retorna `source` original
- Si `source` es fallido y `actionFail` lanza excepción: Retorna error combinado

**Ejemplo**:
```csharp
var result = GetUserData(userId)
    .TryExecSelf(
        validUser => 
        {
            // Esta acción puede fallar
            _externalLogger.LogUserAccess(validUser); // Puede lanzar HttpException
        },
        errorDetails => 
        {
            // Esta acción también puede fallar
            _externalAlertService.SendAlert(errorDetails); // Puede lanzar TimeoutException
        },
        ex => $"Side effect execution failed: {ex.Message}"
    );

// Si alguna acción falla, se convierte en el resultado principal
// Esto es útil cuando las acciones son críticas
```

### Versiones Asíncronas de TryExecSelf

Todas las combinaciones posibles de acciones síncronas/asíncronas:

```csharp
public static async Task<MlResult<T>> TryExecSelfAsync<T>(
    this MlResult<T> source,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync,
    Func<Exception, string> errorMessageBuilder)

// Y muchas más combinaciones...
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Operación | Fuente | Función/Acción | Método |
|-----------|--------|----------------|--------|
| BindSaveValue | `MlResult<T>` | `T → MlResult<U>` | `BindSaveValueInDetailsIfFaildFuncResult` |
| BindSaveValue | `MlResult<T>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| BindSaveValue | `Task<MlResult<T>>` | `T → MlResult<U>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| BindSaveValue | `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| ExecSelf | `MlResult<T>` | `Action<T>, Action<Errors>` | `ExecSelf` |
| ExecSelf | `MlResult<T>` | `Func<T,Task>, Func<Errors,Task>` | `ExecSelfAsync` |
| ExecSelf | `Task<MlResult<T>>` | Cualquier combinación | `ExecSelfAsync` |

Todas las operaciones tienen sus correspondientes versiones `Try*` con manejo de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Pedidos con Preservación de Contexto

```csharp
public class OrderProcessingService
{
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger _logger;
    private readonly IAuditService _auditService;
    
    public OrderProcessingService(
        IPaymentService paymentService,
        IInventoryService inventoryService,
        ILogger logger,
        IAuditService auditService)
    {
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _logger = logger;
        _auditService = auditService;
    }
    
    public async Task<MlResult<ProcessedOrder>> ProcessOrderWithContextAsync(OrderRequest orderRequest)
    {
        var correlationId = Guid.NewGuid().ToString();
        
        return await ValidateOrderRequest(orderRequest)
            .ExecSelf(
                validOrder => _logger.LogInformation($"[{correlationId}] Order validation successful for order {validOrder.OrderId}"),
                errors => _logger.LogWarning($"[{correlationId}] Order validation failed: {errors.FirstErrorMessage}")
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validOrder => 
                await ReserveInventoryWithContextAsync(validOrder, correlationId))
            .ExecSelfAsync(
                async reservedOrder => await _auditService.LogOrderEventAsync(correlationId, "InventoryReserved", reservedOrder.OrderId),
                async errors => await _auditService.LogOrderEventAsync(correlationId, "InventoryReservationFailed", errors.FirstErrorMessage)
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async reservedOrder => 
                await ProcessPaymentWithContextAsync(reservedOrder, correlationId))
            .ExecSelfAsync(
                async paidOrder => await _auditService.LogOrderEventAsync(correlationId, "PaymentProcessed", paidOrder.OrderId),
                async errors => await _auditService.LogOrderEventAsync(correlationId, "PaymentFailed", errors.FirstErrorMessage)
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async paidOrder => 
                await FinalizeOrderWithContextAsync(paidOrder, correlationId))
            .ExecSelfAsync(
                async finalOrder => 
                {
                    _logger.LogInformation($"[{correlationId}] Order {finalOrder.OrderId} processed successfully");
                    await _auditService.LogOrderCompletionAsync(correlationId, finalOrder);
                },
                async errors => 
                {
                    _logger.LogError($"[{correlationId}] Order processing failed: {errors.FirstErrorMessage}");
                    await _auditService.LogOrderFailureAsync(correlationId, errors);
                }
            );
    }
    
    private async Task<MlResult<ReservedOrder>> ReserveInventoryWithContextAsync(OrderRequest validOrder, string correlationId)
    {
        try
        {
            var reservationResults = new List<InventoryReservation>();
            
            foreach (var item in validOrder.Items)
            {
                _logger.LogDebug($"[{correlationId}] Reserving {item.Quantity} units of {item.ProductId}");
                
                var reservation = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
                
                if (!reservation.Success)
                {
                    // Liberar reservas previas
                    await ReleaseReservationsAsync(reservationResults, correlationId);
                    
                    return MlResult<ReservedOrder>.Fail(
                        $"Inventory reservation failed for product {item.ProductId}: {reservation.ErrorMessage}");
                }
                
                reservationResults.Add(reservation);
            }
            
            var reservedOrder = new ReservedOrder
            {
                OrderId = validOrder.OrderId,
                OriginalOrder = validOrder,
                Reservations = reservationResults,
                ReservedAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<ReservedOrder>.Valid(reservedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Inventory reservation exception: {ex.Message}");
            return MlResult<ReservedOrder>.Fail($"Inventory service error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PaidOrder>> ProcessPaymentWithContextAsync(ReservedOrder reservedOrder, string correlationId)
    {
        try
        {
            var totalAmount = reservedOrder.OriginalOrder.Items.Sum(i => i.Price * i.Quantity);
            
            _logger.LogInformation($"[{correlationId}] Processing payment of ${totalAmount} for order {reservedOrder.OrderId}");
            
            var paymentRequest = new PaymentRequest
            {
                OrderId = reservedOrder.OrderId,
                Amount = totalAmount,
                Currency = "USD",
                PaymentMethodId = reservedOrder.OriginalOrder.PaymentMethodId,
                CorrelationId = correlationId
            };
            
            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);
            
            if (!paymentResult.Success)
            {
                // Liberar reservas ya que el pago falló
                await ReleaseReservationsAsync(reservedOrder.Reservations, correlationId);
                
                return MlResult<PaidOrder>.Fail(
                    $"Payment processing failed: {paymentResult.ErrorMessage}");
            }
            
            var paidOrder = new PaidOrder
            {
                OrderId = reservedOrder.OrderId,
                ReservedOrder = reservedOrder,
                PaymentResult = paymentResult,
                PaidAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<PaidOrder>.Valid(paidOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Payment processing exception: {ex.Message}");
            
            // Liberar reservas en caso de excepción
            await ReleaseReservationsAsync(reservedOrder.Reservations, correlationId);
            
            return MlResult<PaidOrder>.Fail($"Payment service error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedOrder>> FinalizeOrderWithContextAsync(PaidOrder paidOrder, string correlationId)
    {
        try
        {
            // Confirmar todas las reservas
            foreach (var reservation in paidOrder.ReservedOrder.Reservations)
            {
                await _inventoryService.ConfirmReservationAsync(reservation.ReservationId);
                _logger.LogDebug($"[{correlationId}] Confirmed reservation {reservation.ReservationId}");
            }
            
            var processedOrder = new ProcessedOrder
            {
                OrderId = paidOrder.OrderId,
                CustomerId = paidOrder.ReservedOrder.OriginalOrder.CustomerId,
                TotalAmount = paidOrder.PaymentResult.Amount,
                Status = OrderStatus.Completed,
                PaymentTransactionId = paidOrder.PaymentResult.TransactionId,
                ProcessedAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<ProcessedOrder>.Valid(processedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Order finalization exception: {ex.Message}");
            return MlResult<ProcessedOrder>.Fail($"Order finalization error: {ex.Message}");
        }
    }
    
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        if (string.IsNullOrWhiteSpace(request.OrderId))
            return MlResult<OrderRequest>.Fail("Order ID is required");
            
        if (request.CustomerId <= 0)
            return MlResult<OrderRequest>.Fail("Valid customer ID is required");
            
        if (request.Items?.Any() != true)
            return MlResult<OrderRequest>.Fail("Order must contain at least one item");
            
        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ProductId))
                return MlResult<OrderRequest>.Fail($"Product ID is required for all items");
                
            if (item.Quantity <= 0)
                return MlResult<OrderRequest>.Fail($"Quantity must be positive for product {item.ProductId}");
                
            if (item.Price <= 0)
                return MlResult<OrderRequest>.Fail($"Price must be positive for product {item.ProductId}");
        }
        
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task ReleaseReservationsAsync(List<InventoryReservation> reservations, string correlationId)
    {
        foreach (var reservation in reservations)
        {
            try
            {
                await _inventoryService.ReleaseReservationAsync(reservation.ReservationId);
                _logger.LogDebug($"[{correlationId}] Released reservation {reservation.ReservationId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[{correlationId}] Failed to release reservation {reservation.ReservationId}: {ex.Message}");
            }
        }
    }
}

// Clases de apoyo
public class OrderRequest
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public string PaymentMethodId { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class ReservedOrder
{
    public string OrderId { get; set; }
    public OrderRequest OriginalOrder { get; set; }
    public List<InventoryReservation> Reservations { get; set; }
    public DateTime ReservedAt { get; set; }
    public string CorrelationId { get; set; }
}

public class PaidOrder
{
    public string OrderId { get; set; }
    public ReservedOrder ReservedOrder { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public DateTime PaidAt { get; set; }
    public string CorrelationId { get; set; }
}

public class ProcessedOrder
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentTransactionId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string CorrelationId { get; set; }
}

public class InventoryReservation
{
    public string ReservationId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class PaymentRequest
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentMethodId { get; set; }
    public string CorrelationId { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string ErrorMessage { get; set; }
}

public enum OrderStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}

// Interfaces
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

public interface IInventoryService
{
    Task<InventoryReservation> ReserveAsync(string productId, int quantity);
    Task ConfirmReservationAsync(string reservationId);
    Task ReleaseReservationAsync(string reservationId);
}

public interface IAuditService
{
    Task LogOrderEventAsync(string correlationId, string eventType, string details);
    Task LogOrderCompletionAsync(string correlationId, ProcessedOrder order);
    Task LogOrderFailureAsync(string correlationId, MlErrorsDetails errors);
}
```

### Ejemplo 2: Sistema de Análisis de Datos con Preservación de Contexto

```csharp
public class DataAnalysisService
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger _logger;
    
    public DataAnalysisService(
        IDataProcessor dataProcessor,
        IMetricsCollector metricsCollector,
        ILogger logger)
    {
        _dataProcessor = dataProcessor;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }
    
    public async Task<MlResult<AnalysisResult>> AnalyzeDataWithContextAsync(DataSet dataSet)
    {
        var analysisId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateDataSet(dataSet)
            .ExecSelf(
                validDataSet => _logger.LogInformation($"[{analysisId}] DataSet validation successful. Rows: {validDataSet.RowCount}, Columns: {validDataSet.ColumnCount}"),
                errors => _logger.LogWarning($"[{analysisId}] DataSet validation failed: {errors.FirstErrorMessage}")
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validDataSet => 
                await PreprocessDataWithContextAsync(validDataSet, analysisId),
                ex => $"Data preprocessing failed: {ex.Message}")
            .ExecSelfAsync(
                async preprocessedData => 
                {
                    _logger.LogInformation($"[{analysisId}] Data preprocessing completed. Original rows: {preprocessedData.OriginalDataSet.RowCount}, Processed rows: {preprocessedData.ProcessedRowCount}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Preprocessing", 
                        Success = true, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                },
                async errors => 
                {
                    _logger.LogError($"[{analysisId}] Data preprocessing failed: {errors.FirstErrorMessage}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Preprocessing", 
                        Success = false, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                }
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async preprocessedData => 
                await PerformStatisticalAnalysisAsync(preprocessedData, analysisId),
                ex => $"Statistical analysis failed: {ex.Message}")
            .ExecSelfAsync(
                async analysisResult => 
                {
                    _logger.LogInformation($"[{analysisId}] Statistical analysis completed. Insights: {analysisResult.InsightCount}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Analysis", 
                        Success = true, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                },
                async errors => 
                {
                    _logger.LogError($"[{analysisId}] Statistical analysis failed: {errors.FirstErrorMessage}");
                    
                    // Aquí es donde el contexto preservado es valioso
                    // Los errores contendrán información sobre qué datos se estaban analizando
                    await LogDetailedFailureAsync(analysisId, errors, startTime);
                }
            );
    }
    
    public MlResult<ProcessedData> ProcessDataWithLogging(RawData rawData, string userId)
    {
        return ValidateRawData(rawData)
            .ExecSelf(
                validData => _logger.LogInformation($"User {userId} processing valid data: {validData.Size} bytes"),
                errors => _logger.LogWarning($"User {userId} attempted to process invalid data: {errors.FirstErrorMessage}")
            )
            .TryBindSaveValueInDetailsIfFaildFuncResult(validData => 
            {
                // Esta operación puede fallar y queremos preservar los datos originales
                if (validData.Size > 10_000_000) // 10MB
                {
                    return MlResult<ProcessedData>.Fail("Data size exceeds maximum allowed limit");
                }
                
                var processed = _dataProcessor.Process(validData);
                
                if (processed.Quality < 0.8) // 80% quality threshold
                {
                    return MlResult<ProcessedData>.Fail($"Processed data quality too low: {processed.Quality:P}");
                }
                
                return MlResult<ProcessedData>.Valid(processed);
            },
            ex => $"Data processing error: {ex.Message}")
            .ExecSelf(
                processedData => 
                {
                    _logger.LogInformation($"User {userId} successfully processed data. Quality: {processedData.Quality:P}");
                    _metricsCollector.Record(new ProcessingMetrics 
                    { 
                        UserId = userId, 
                        Success = true, 
                        Quality = processedData.Quality 
                    });
                },
                errors => 
                {
                    _logger.LogError($"User {userId} data processing failed: {errors.FirstErrorMessage}");
                    
                    // Si tenemos el contexto preservado, podemos hacer análisis más detallado
                    if (errors.HasValueDetails)
                    {
                        var originalData = errors.GetValueDetail<RawData>();
                        _logger.LogDebug($"Failed processing context - Original data size: {originalData.Size}, Type: {originalData.Type}");
                        
                        _metricsCollector.Record(new ProcessingMetrics 
                        { 
                            UserId = userId, 
                            Success = false, 
                            OriginalDataSize = originalData.Size,
                            FailureReason = errors.FirstErrorMessage
                        });
                    }
                }
            );
    }
    
    private async Task<MlResult<PreprocessedData>> PreprocessDataWithContextAsync(DataSet validDataSet, string analysisId)
    {
        try
        {
            _logger.LogDebug($"[{analysisId}] Starting data preprocessing");
            
            // Simular preprocesamiento que puede fallar
            if (validDataSet.HasMissingValues && validDataSet.MissingValuePercentage > 0.3)
            {
                return MlResult<PreprocessedData>.Fail(
                    $"Too many missing values: {validDataSet.MissingValuePercentage:P} (max allowed: 30%)");
            }
            
            if (validDataSet.HasOutliers && validDataSet.OutlierPercentage > 0.1)
            {
                return MlResult<PreprocessedData>.Fail(
                    $"Too many outliers detected: {validDataSet.OutlierPercentage:P} (max allowed: 10%)");
            }
            
            var preprocessed = await _dataProcessor.PreprocessAsync(validDataSet);
            
            var preprocessedData = new PreprocessedData
            {
                OriginalDataSet = validDataSet,
                ProcessedRowCount = preprocessed.RowCount,
                ProcessedColumnCount = preprocessed.ColumnCount,
                QualityScore = preprocessed.QualityScore,
                PreprocessedAt = DateTime.UtcNow,
                AnalysisId = analysisId
            };
            
            return MlResult<PreprocessedData>.Valid(preprocessedData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{analysisId}] Preprocessing exception: {ex.Message}");
            throw; // Se captura en TryBindSaveValueInDetailsIfFaildFuncResultAsync
        }
    }
    
    private async Task<MlResult<AnalysisResult>> PerformStatisticalAnalysisAsync(PreprocessedData preprocessedData, string analysisId)
    {
        try
        {
            _logger.LogDebug($"[{analysisId}] Starting statistical analysis");
            
            if (preprocessedData.QualityScore < 0.7)
            {
                return MlResult<AnalysisResult>.Fail(
                    $"Data quality too low for reliable analysis: {preprocessedData.QualityScore:P}");
            }
            
            var statisticalResults = await _dataProcessor.PerformStatisticalAnalysisAsync(preprocessedData);
            
            if (statisticalResults.Confidence < 0.95)
            {
                return MlResult<AnalysisResult>.Fail(
                    $"Analysis confidence too low: {statisticalResults.Confidence:P} (required: 95%)");
            }
            
            var analysisResult = new AnalysisResult
            {
                PreprocessedData = preprocessedData,
                StatisticalResults = statisticalResults,
                InsightCount = statisticalResults.Insights.Count,
                Confidence = statisticalResults.Confidence,
                CompletedAt = DateTime.UtcNow,
                AnalysisId = analysisId
            };
            
            return MlResult<AnalysisResult>.Valid(analysisResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{analysisId}] Statistical analysis exception: {ex.Message}");
            throw; // Se captura en TryBindSaveValueInDetailsIfFaildFuncResultAsync
        }
    }
    
    private async Task LogDetailedFailureAsync(string analysisId, MlErrorsDetails errors, DateTime startTime)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogError($"[{analysisId}] Analysis failed after {duration.TotalMinutes:F2} minutes");
            
            // Si tenemos contexto preservado, podemos hacer análisis detallado del fallo
            if (errors.HasValueDetails)
            {
                var failureContext = new AnalysisFailureContext
                {
                    AnalysisId = analysisId,
                    Duration = duration,
                    ErrorMessage = errors.FirstErrorMessage,
                    AllErrors = errors.AllErrors,
                    PreservedContexts = new List<object>()
                };
                
                // Extraer todos los contextos preservados
                foreach (var detail in errors.ValueDetails)
                {
                    failureContext.PreservedContexts.Add(detail.Value);
                    
                    // Log específico según el tipo de contexto
                    switch (detail.Value)
                    {
                        case DataSet dataSet:
                            _logger.LogDebug($"[{analysisId}] Failed with DataSet context - Rows: {dataSet.RowCount}, Columns: {dataSet.ColumnCount}");
                            break;
                        case PreprocessedData preprocessed:
                            _logger.LogDebug($"[{analysisId}] Failed with PreprocessedData context - Quality: {preprocessed.QualityScore:P}");
                            break;
                    }
                }
                
                await _metricsCollector.RecordFailureAsync(failureContext);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"[{analysisId}] Failed to log detailed failure information: {ex.Message}");
        }
    }
    
    // Métodos auxiliares...
    private MlResult<DataSet> ValidateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
            return MlResult<DataSet>.Fail("DataSet cannot be null");
            
        if (dataSet.RowCount <= 0)
            return MlResult<DataSet>.Fail("DataSet must contain at least one row");
            
        if (dataSet.ColumnCount <= 0)
            return MlResult<DataSet>.Fail("DataSet must contain at least one column");
            
        if (dataSet.RowCount < 10)
            return MlResult<DataSet>.Fail("DataSet must contain at least 10 rows for reliable analysis");
            
        return MlResult<DataSet>.Valid(dataSet);
    }
    
    private MlResult<RawData> ValidateRawData(RawData rawData)
    {
        if (rawData == null)
            return MlResult<RawData>.Fail("Raw data cannot be null");
            
        if (rawData.Size <= 0)
            return MlResult<RawData>.Fail("Raw data must have positive size");
            
        if (string.IsNullOrWhiteSpace(rawData.Type))
            return MlResult<RawData>.Fail("Raw data type must be specified");
            
        return MlResult<RawData>.Valid(rawData);
    }
}

// Clases de apoyo
public class DataSet
{
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public bool HasMissingValues { get; set; }
    public double MissingValuePercentage { get; set; }
    public bool HasOutliers { get; set; }
    public double OutlierPercentage { get; set; }
}

public class PreprocessedData
{
    public DataSet OriginalDataSet { get; set; }
    public int ProcessedRowCount { get; set; }
    public int ProcessedColumnCount { get; set; }
    public double QualityScore { get; set; }
    public DateTime PreprocessedAt { get; set; }
    public string AnalysisId { get; set; }
}

public class AnalysisResult
{
    public PreprocessedData PreprocessedData { get; set; }
    public StatisticalResults StatisticalResults { get; set; }
    public int InsightCount { get; set; }
    public double Confidence { get; set; }
    public DateTime CompletedAt { get; set; }
    public string AnalysisId { get; set; }
}

public class StatisticalResults
{
    public List<Insight> Insights { get; set; }
    public double Confidence { get; set; }
}

public class Insight
{
    public string Type { get; set; }
    public string Description { get; set; }
    public double Significance { get; set; }
}

public class RawData
{
    public long Size { get; set; }
    public string Type { get; set; }
}

public class ProcessedData
{
    public double Quality { get; set; }
}

public class ProcessingMetrics
{
    public string AnalysisId { get; set; }
    public string UserId { get; set; }
    public string Stage { get; set; }
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public double Quality { get; set; }
    public long OriginalDataSize { get; set; }
    public string FailureReason { get; set; }
}

public class AnalysisFailureContext
{
    public string AnalysisId { get; set; }
    public TimeSpan Duration { get; set; }
    public string ErrorMessage { get; set; }
    public string[] AllErrors { get; set; }
    public List<object> PreservedContexts { get; set; }
}

// Interfaces
public interface IDataProcessor
{
    ProcessedData Process(RawData rawData);
    Task<PreprocessedData> PreprocessAsync(DataSet dataSet);
    Task<StatisticalResults> PerformStatisticalAnalysisAsync(PreprocessedData data);
}

public interface IMetricsCollector
{
    Task RecordAsync(ProcessingMetrics metrics);
    void Record(ProcessingMetrics metrics);
    Task RecordFailureAsync(AnalysisFailureContext context);
}
```

### Ejemplo 3: Sistema de Debugging y Diagnóstico

```csharp
public class DiagnosticService
{
    private readonly ILogger _logger;
    private readonly IDiagnosticCollector _diagnosticCollector;
    
    public DiagnosticService(ILogger logger, IDiagnosticCollector diagnosticCollector)
    {
        _logger = logger;
        _diagnosticCollector = diagnosticCollector;
    }
    
    public async Task<MlResult<ProcessingResult>> ProcessWithDiagnosticsAsync<T>(
        T inputData, 
        string operationName,
        Func<T, Task<MlResult<ProcessingResult>>> processingFunction)
    {
        var diagnosticSession = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateInput(inputData)
            .ExecSelf(
                validInput => LogDiagnostic(diagnosticSession, "InputValidation", "Success", validInput),
                errors => LogDiagnostic(diagnosticSession, "InputValidation", "Failed", errors.FirstErrorMessage)
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validInput => 
            {
                var result = await processingFunction(validInput);
                return result;
            },
            ex => $"Processing function failed: {ex.Message}")
            .ExecSelfAsync(
                async processingResult => await CollectSuccessDiagnosticsAsync(diagnosticSession, operationName, processingResult, startTime),
                async errors => await CollectFailureDiagnosticsAsync(diagnosticSession, operationName, errors, startTime)
            );
    }
    
    public MlResult<ValidationResult> ValidateWithDiagnostics<T>(T data, IValidator<T> validator)
    {
        var validationId = Guid.NewGuid().ToString();
        
        return MlResult<T>.Valid(data)
            .ExecSelf(
                input => _logger.LogDebug($"[{validationId}] Starting validation for {typeof(T).Name}"),
                _ => { } // No debería llegar aquí ya que empezamos con Valid
            )
            .TryBindSaveValueInDetailsIfFaildFuncResult(validData => 
            {
                var validationResult = validator.Validate(validData);
                
                if (!validationResult.IsValid)
                {
                    var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return MlResult<ValidationResult>.Fail($"Validation failed: {errorMessage}");
                }
                
                return MlResult<ValidationResult>.Valid(validationResult);
            },
            ex => $"Validation process failed: {ex.Message}")
            .ExecSelf(
                successResult => 
                {
                    _logger.LogInformation($"[{validationId}] Validation successful for {typeof(T).Name}");
                    _diagnosticCollector.RecordValidation(validationId, typeof(T).Name, true, null);
                },
                errors => 
                {
                    _logger.LogWarning($"[{validationId}] Validation failed for {typeof(T).Name}: {errors.FirstErrorMessage}");
                    
                    // El contexto preservado nos permite saber exactamente qué datos fallaron
                    if (errors.HasValueDetails)
                    {
                        var originalData = errors.GetValueDetail<T>();
                        var diagnosticInfo = new ValidationDiagnostic
                        {
                            ValidationId = validationId,
                            DataType = typeof(T).Name,
                            OriginalData = originalData,
                            ErrorMessage = errors.FirstErrorMessage,
                            AllErrors = errors.AllErrors,
                            ValidationRules = validator.GetAppliedRules(originalData)
                        };
                        
                        _diagnosticCollector.RecordValidationFailure(diagnosticInfo);
                    }
                }
            );
    }
    
    private async Task CollectSuccessDiagnosticsAsync(
        string sessionId, 
        string operationName, 
        ProcessingResult result, 
        DateTime startTime)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            
            var diagnostic = new OperationDiagnostic
            {
                SessionId = sessionId,
                OperationName = operationName,
                Success = true,
                Duration = duration,
                Result = result,
                CompletedAt = DateTime.UtcNow
            };
            
            await _diagnosticCollector.RecordOperationAsync(diagnostic);
            
            _logger.LogInformation($"[{sessionId}] {operationName} completed successfully in {duration.TotalMilliseconds:F2}ms");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to collect success diagnostics: {ex.Message}");
        }
    }
    
    private async Task CollectFailureDiagnosticsAsync(
        string sessionId, 
        string operationName, 
        MlErrorsDetails errors, 
        DateTime startTime)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            
            var diagnostic = new OperationDiagnostic
            {
                SessionId = sessionId,
                OperationName = operationName,
                Success = false,
                Duration = duration,
                ErrorMessage = errors.FirstErrorMessage,
                AllErrors = errors.AllErrors,
                CompletedAt = DateTime.UtcNow
            };
            
            // Si tenemos contexto preservado, incluirlo en el diagnóstico
            if (errors.HasValueDetails)
            {
                diagnostic.PreservedContexts = errors.ValueDetails
                    .Select(vd => new PreservedContext 
                    { 
                        Type = vd.Value?.GetType().Name ?? "null", 
                        Value = vd.Value 
                    })
                    .ToList();
                    
                _logger.LogError($"[{sessionId}] {operationName} failed after {duration.TotalMilliseconds:F2}ms with preserved context");
                
                // Log detalles específicos del contexto preservado
                foreach (var context in diagnostic.PreservedContexts)
                {
                    _logger.LogDebug($"[{sessionId}] Preserved context: {context.Type}");
                }
            }
            else
            {
                _logger.LogError($"[{sessionId}] {operationName} failed after {duration.TotalMilliseconds:F2}ms without context");
            }
            
            await _diagnosticCollector.RecordOperationAsync(diagnostic);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to collect failure diagnostics: {ex.Message}");
        }
    }
    
    private void LogDiagnostic(string sessionId, string stage, string status, object data)
    {
        try
        {
            _logger.LogDebug($"[{sessionId}] {stage}: {status}");
            
            var diagnostic = new StageDiagnostic
            {
                SessionId = sessionId,
                Stage = stage,
                Status = status,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
            
            _diagnosticCollector.RecordStage(diagnostic);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to log stage diagnostic: {ex.Message}");
        }
    }
    
    private MlResult<T> ValidateInput<T>(T input)
    {
        if (input == null)
            return MlResult<T>.Fail("Input cannot be null");
            
        // Validaciones específicas por tipo
        switch (input)
        {
            case string str when string.IsNullOrWhiteSpace(str):
                return MlResult<T>.Fail("String input cannot be empty or whitespace");
            case ICollection collection when collection.Count == 0:
                return MlResult<T>.Fail("Collection input cannot be empty");
        }
        
        return MlResult<T>.Valid(input);
    }
}

// Clases de apoyo para diagnósticos
public class ProcessingResult
{
    public string Id { get; set; }
    public object Data { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<ValidationError> Errors { get; set; } = new();
}

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}

public class OperationDiagnostic
{
    public string SessionId { get; set; }
    public string OperationName { get; set; }
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public ProcessingResult Result { get; set; }
    public string ErrorMessage { get; set; }
    public string[] AllErrors { get; set; }
    public List<PreservedContext> PreservedContexts { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class ValidationDiagnostic
{
    public string ValidationId { get; set; }
    public string DataType { get; set; }
    public object OriginalData { get; set; }
    public string ErrorMessage { get; set; }
    public string[] AllErrors { get; set; }
    public List<string> ValidationRules { get; set; }
}

public class StageDiagnostic
{
    public string SessionId { get; set; }
    public string Stage { get; set; }
    public string Status { get; set; }
    public object Data { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PreservedContext
{
    public string Type { get; set; }
    public object Value { get; set; }
}

// Interfaces
public interface IValidator<T>
{
    ValidationResult Validate(T data);
    List<string> GetAppliedRules(T data);
}

public interface IDiagnosticCollector
{
    Task RecordOperationAsync(OperationDiagnostic diagnostic);
    void RecordValidation(string validationId, string dataType, bool success, string error);
    void RecordValidationFailure(ValidationDiagnostic diagnostic);
    void RecordStage(StageDiagnostic diagnostic);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar BindSaveValueInDetailsIfFaildFuncResult

```csharp
// ✅ Correcto: Preservar contexto para análisis de fallos
var result = GetUserData(userId)
    .BindSaveValueInDetailsIfFaildFuncResult(userData => 
        ProcessComplexUserData(userData)); // Si falla, sabemos qué userData se procesaba

// ✅ Correcto: Preservar datos valiosos para debugging
var result = ParseJsonData(jsonString)
    .BindSaveValueInDetailsIfFaildFuncResult(parsedData => 
        ValidateBusinessRules(parsedData)); // Si las reglas fallan, mantenemos los datos parseados

// ❌ Incorrecto: Usar cuando no necesitas el contexto
var result = GetSimpleValue()
    .BindSaveValueInDetailsIfFaildFuncResult(value => 
        ProcessValue(value)); // Si ProcessValue es simple, usar Bind normal es suficiente
```

### 2. Cuándo Usar ExecSelf

```csharp
// ✅ Correcto: Logging que no debe afectar el resultado
var result = ProcessData(input)
    .ExecSelf(
        data => _logger.LogInformation($"Processing successful: {data.Id}"),
        errors => _logger.LogError($"Processing failed: {errors.FirstErrorMessage}")
    )
    .Bind(data => NextProcessingStep(data));

// ✅ Correcto: Métricas y auditoría
var result = await CallExternalService()
    .ExecSelfAsync(
        async response => await _metricsService.RecordSuccessAsync(response.Duration),
        async errors => await _metricsService.RecordFailureAsync(errors.FirstErrorMessage)
    );

// ❌ Incorrecto: Acciones que pueden cambiar el estado del sistema
var result = CreateUser(userData)
    .ExecSelf(
        user => SendWelcomeEmail(user), // Esto debería ser parte del pipeline principal
        errors => { } // Vacío no aporta valor
    );
```

### 3. Manejo de Excepciones en Acciones

```csharp
// ✅ Correcto: Usar TryExecSelf cuando las acciones pueden fallar
var result = ProcessOrder(order)
    .TryExecSelf(
        processedOrder => 
        {
            // Esta acción puede lanzar excepciones
            _externalNotificationService.NotifyOrderProcessed(processedOrder);
        },
        errors => 
        {
            // Esta acción también puede fallar
            _externalAlertService.SendAlert(errors);
        },
        ex => $"Side effect execution failed: {ex.Message}"
    );

// ✅ Correcto: ExecSelf normal cuando las acciones son seguras
var result = ProcessData(input)
    .ExecSelf(
        data => _logger.LogInformation($"Success: {data.Id}"), // Logger interno, seguro
        errors => _logger.LogError($"Failed: {errors.FirstErrorMessage}") // Logger interno, seguro
    );

// ❌ Incorrecto: ExecSelf con acciones que pueden lanzar excepciones
var result = ProcessData(input)
    .ExecSelf(
        data => CallUnreliableExternalService(data), // Puede lanzar excepciones no controladas
        errors => WriteToUnreliableStorage(errors) // Puede fallar
    );
```

### 4. Preservación de Contexto Estratégica

```csharp
// ✅ Correcto: Preservar contexto en puntos críticos del pipeline
public async Task<MlResult<FinalResult>> ComplexProcessingPipelineAsync(InputData input)
{
    return await ValidateInput(input)
        .Bind(validInput => EnrichData(validInput))
        .BindSaveValueInDetailsIfFaildFuncResultAsync(async enrichedData => 
            await CallCriticalExternalService(enrichedData)) // Punto crítico: preservar datos enriquecidos
        .Bind(externalResult => ApplyBusinessRules(externalResult))
        .BindSaveValueInDetailsIfFaildFuncResultAsync(async businessResult => 
            await PersistToDatabase(businessResult)) // Punto crítico: preservar resultado de negocio
        .Bind(persistedResult => GenerateFinalOutput(persistedResult));
}

// ✅ Correcto: Usar contexto preservado para mejor logging
public void LogFailureWithContext(MlErrorsDetails errors, string operationName)
{
    _logger.LogError($"Operation {operationName} failed: {errors.FirstErrorMessage}");
    
    if (errors.HasValueDetails)
    {
        foreach (var context in errors.ValueDetails)
        {
            _logger.LogDebug($"Context available: {context.Value?.GetType().Name}");
            
            // Log específico según el tipo de contexto
            switch (context.Value)
            {
                case UserData userData:
                    _logger.LogDebug($"Failed processing user: {userData.UserId}");
                    break;
                case OrderData orderData:
                    _logger.LogDebug($"Failed processing order: {orderData.OrderId}");
                    break;
            }
        }
    }
}
```

### 5. Combinación de Técnicas

```csharp
// ✅ Correcto: Combinar ExecSelf con BindSaveValueInDetailsIfFaildFuncResult
public async Task<MlResult<ProcessedOrder>> ProcessOrderCompleteAsync(OrderRequest request)
{
    var correlationId = Guid.NewGuid().ToString();
    
    return await ValidateOrderRequest(request)
        .ExecSelf(
            validOrder => _logger.LogInformation($"[{correlationId}] Order {validOrder.Id} validation successful"),
            errors => _logger.LogWarning($"[{correlationId}] Order validation failed: {errors.FirstErrorMessage}")
        )
        .BindSaveValueInDetailsIfFaildFuncResultAsync(async validOrder => 
            await ProcessPayment(validOrder, correlationId))
        .ExecSelfAsync(
            async paidOrder => 
            {
                _logger.LogInformation($"[{correlationI// filepath: c:\PakkkoTFS\MoralesLarios\FOOP\MoralesLarios.FOOP\docs\MlResultActionsBindSaveValueAndExecSelf.md
# MlResultActionsBindSaveValueAndExecSelf - Operaciones de Preservación de Contexto y Ejecución con Retorno

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindSaveValueInDetailsIfFaildFuncResult](#métodos-bindsavevalueindetailsiffaildFuncresult)
4. [Métodos ExecSelf](#métodos-execself)
5. [Métodos TryBindSaveValueInDetailsIfFaildFuncResult](#métodos-trybindsavevalueindetailsiffaildFuncresult)
6. [Métodos TryExecSelf](#métodos-tryexecself)
7. [Variantes Asíncronas](#variantes-asíncronas)
8. [Ejemplos Prácticos](#ejemplos-prácticos)
9. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBindSaveValueAndExecSelf` contiene operaciones especializadas que se enfocan en **preservar el contexto** y **ejecutar acciones sin cambiar el resultado**. Estas operaciones son fundamentales para:

- **Preservación de Contexto**: Guardar el valor original cuando las operaciones subsiguientes fallan
- **Ejecución de Efectos Secundarios**: Realizar acciones (logging, auditoría) sin modificar el resultado
- **Debugging y Diagnóstico**: Mantener información valiosa para el análisis de errores

### Propósito Principal

- **BindSaveValueInDetailsIfFaildFuncResult**: Preserva el valor original en los detalles del error si la función falla
- **ExecSelf**: Ejecuta acciones basadas en el estado del resultado pero retorna el resultado original sin modificaciones
- **Manejo Seguro**: Versiones `Try*` que capturan excepciones sin perder el contexto

---

## Análisis de la Clase

### Estructura y Filosofía

Esta clase implementa dos patrones importantes:

1. **Preservación de Contexto**: Mantener información valiosa cuando las operaciones fallan
2. **Ejecución Transparente**: Realizar acciones sin afectar el flujo de datos

```
Resultado Exitoso → Función → [Éxito: Nuevo Resultado] | [Fallo: Error + Contexto Original]
      ↓                ↓               ↓                          ↓
Resultado Fallido → [No ejecuta] → Propaga Error Original   N/A
```

### Características Principales

1. **Contexto Preservado**: Los valores originales se mantienen en caso de fallo
2. **Ejecución Condicional**: Las acciones se ejecutan según el estado del resultado
3. **Transparencia**: `ExecSelf` no modifica el resultado, solo ejecuta efectos secundarios
4. **Debugging Mejorado**: Información adicional disponible para diagnóstico

---

## Métodos BindSaveValueInDetailsIfFaildFuncResult

### `BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Ejecuta una función si el resultado es válido, pero si la función falla, preserva el valor original en los detalles del error

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `func`: Función que se ejecuta si `source` es válido

**Comportamiento**:
- Si `source` es fallido: Propaga el error original
- Si `source` es válido y `func(value)` es exitoso: Retorna el resultado de `func`
- Si `source` es válido y `func(value)` falla: Retorna el error de `func` **más** el valor original en los detalles

**Ejemplo Básico**:
```csharp
var user = MlResult<User>.Valid(new User { Id = 123, Name = "John Doe" });

var result = user.BindSaveValueInDetailsIfFaildFuncResult(validUser => 
{
    // Esta operación falla
    return MlResult<ProcessedUser>.Fail("Processing failed");
});

// result contiene:
// - Error: "Processing failed"
// - Detalles adicionales: User { Id = 123, Name = "John Doe" }
// Esto permite saber qué datos se estaban procesando cuando falló
```

### Versiones Asíncronas

#### `BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>()`
```csharp
// Función asíncrona desde resultado síncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

// Función asíncrona desde resultado asíncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)

// Función síncrona desde resultado asíncrono
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, MlResult<TReturn>> func)
```

**Ejemplo Asíncrono**:
```csharp
var userData = await GetUserDataAsync(userId);

var result = await userData.BindSaveValueInDetailsIfFaildFuncResultAsync(async validData => 
{
    // Operación asíncrona que puede fallar
    var processed = await ProcessUserDataAsync(validData);
    
    if (processed == null)
        return MlResult<ProcessedUserData>.Fail("External service returned null");
        
    return MlResult<ProcessedUserData>.Valid(processed);
});

// Si falla, tendremos tanto el error como los datos originales del usuario
```

---

## Métodos ExecSelf

### `ExecSelf<T>()`

**Propósito**: Ejecuta diferentes acciones según el estado del resultado, pero siempre retorna el resultado original sin modificaciones

```csharp
public static MlResult<T> ExecSelf<T>(this MlResult<T> source, 
                                      Action<T> actionValid,
                                      Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionValid`: Acción que se ejecuta si `source` es válido (recibe el valor)
- `actionFail`: Acción que se ejecuta si `source` es fallido (recibe los errores)

**Comportamiento**:
- Si `source` es válido: Ejecuta `actionValid(value)` y retorna `source` sin cambios
- Si `source` es fallido: Ejecuta `actionFail(errorDetails)` y retorna `source` sin cambios
- **Siempre retorna el resultado original**

**Ejemplo Básico**:
```csharp
var result = GetUserData(userId)
    .ExecSelf(
        validUser => _logger.LogInformation($"User {validUser.Id} retrieved successfully"),
        errorDetails => _logger.LogError($"Failed to retrieve user {userId}: {errorDetails.FirstErrorMessage}")
    )
    .Bind(user => ProcessUser(user));

// ExecSelf no modifica el resultado, solo ejecuta logging
// El resultado continúa siendo exactamente el mismo que GetUserData devolvió
```

### Versiones Asíncronas del ExecSelf

#### `ExecSelfAsync<T>()` - Todas las Combinaciones
```csharp
// Ambas acciones asíncronas desde resultado síncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this MlResult<T> source,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Ambas acciones asíncronas desde resultado asíncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Acción válida síncrona, acción fallo asíncrona
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<T> actionValid,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Acción válida asíncrona, acción fallo síncrona
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync,
    Action<MlErrorsDetails> actionFail)

// Ambas acciones síncronas desde resultado asíncrono
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Action<T> actionValid,
    Action<MlErrorsDetails> actionFail)
```

**Ejemplo Asíncrono**:
```csharp
var result = await GetUserDataAsync(userId)
    .ExecSelfAsync(
        async validUser => await _auditService.LogUserAccessAsync(validUser.Id, "Retrieved"),
        async errorDetails => await _alertService.NotifyFailureAsync($"User retrieval failed: {errorDetails.FirstErrorMessage}")
    );

// El resultado sigue siendo exactamente el mismo, pero se ejecutaron acciones asíncronas
```

---

## Métodos TryBindSaveValueInDetailsIfFaildFuncResult

### `TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Versión segura de `BindSaveValueInDetailsIfFaildFuncResult` que captura excepciones

```csharp
public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    Func<Exception, string> errorMessageBuilder)

public static MlResult<TReturn> TryBindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func,
    string errorMessage = null!)
```

**Comportamiento**:
- Igual que `BindSaveValueInDetailsIfFaildFuncResult` pero captura excepciones
- Si `func` lanza una excepción, la convierte en error y preserva el valor original
- El valor original se guarda tanto si `func` retorna error como si lanza excepción

**Ejemplo**:
```csharp
var user = MlResult<User>.Valid(new User { Id = 123, Name = "John Doe" });

var result = user.TryBindSaveValueInDetailsIfFaildFuncResult(
    validUser => 
    {
        // Esta función puede lanzar excepciones
        if (validUser.Name.Contains("invalid"))
            throw new ArgumentException("Invalid user name format");
            
        return ProcessUser(validUser);
    },
    ex => $"User processing failed with exception: {ex.Message}"
);

// Si lanza excepción, el resultado incluirá:
// - Error: "User processing failed with exception: Invalid user name format"
// - Detalles: User { Id = 123, Name = "John Doe" }
```

### Versiones Asíncronas de TryBindSaveValueInDetailsIfFaildFuncResult

Todas las combinaciones están disponibles:
- Funciones síncronas y asíncronas
- Resultados síncronos y asíncronos
- Con constructor de mensaje de error y mensaje simple

```csharp
public static async Task<MlResult<TReturn>> TryBindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync,
    Func<Exception, string> errorMessageBuilder)
```

---

## Métodos TryExecSelf

### `TryExecSelf<T>()`

**Propósito**: Versión segura de `ExecSelf` que captura excepciones en las acciones

```csharp
public static MlResult<T> TryExecSelf<T>(this MlResult<T> source,
                                         Action<T> actionValid,
                                         Action<MlErrorsDetails> actionFail,
                                         Func<Exception, string> errorMessageBuilder)

public static MlResult<T> TryExecSelf<T>(this MlResult<T> source,
                                         Action<T> actionValid,
                                         Action<MlErrorsDetails> actionFail,
                                         string errorMessage = null!)
```

**Comportamiento**:
- Si `source` es válido y `actionValid` no lanza excepción: Retorna `source` original
- Si `source` es válido y `actionValid` lanza excepción: Retorna error (se pierde el resultado original)
- Si `source` es fallido y `actionFail` no lanza excepción: Retorna `source` original
- Si `source` es fallido y `actionFail` lanza excepción: Retorna error combinado

**Ejemplo**:
```csharp
var result = GetUserData(userId)
    .TryExecSelf(
        validUser => 
        {
            // Esta acción puede fallar
            _externalLogger.LogUserAccess(validUser); // Puede lanzar HttpException
        },
        errorDetails => 
        {
            // Esta acción también puede fallar
            _externalAlertService.SendAlert(errorDetails); // Puede lanzar TimeoutException
        },
        ex => $"Side effect execution failed: {ex.Message}"
    );

// Si alguna acción falla, se convierte en el resultado principal
// Esto es útil cuando las acciones son críticas
```

### Versiones Asíncronas de TryExecSelf

Todas las combinaciones posibles de acciones síncronas/asíncronas:

```csharp
public static async Task<MlResult<T>> TryExecSelfAsync<T>(
    this MlResult<T> source,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync,
    Func<Exception, string> errorMessageBuilder)

// Y muchas más combinaciones...
```

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Operación | Fuente | Función/Acción | Método |
|-----------|--------|----------------|--------|
| BindSaveValue | `MlResult<T>` | `T → MlResult<U>` | `BindSaveValueInDetailsIfFaildFuncResult` |
| BindSaveValue | `MlResult<T>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| BindSaveValue | `Task<MlResult<T>>` | `T → MlResult<U>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| BindSaveValue | `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| ExecSelf | `MlResult<T>` | `Action<T>, Action<Errors>` | `ExecSelf` |
| ExecSelf | `MlResult<T>` | `Func<T,Task>, Func<Errors,Task>` | `ExecSelfAsync` |
| ExecSelf | `Task<MlResult<T>>` | Cualquier combinación | `ExecSelfAsync` |

Todas las operaciones tienen sus correspondientes versiones `Try*` con manejo de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Pedidos con Preservación de Contexto

```csharp
public class OrderProcessingService
{
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger _logger;
    private readonly IAuditService _auditService;
    
    public OrderProcessingService(
        IPaymentService paymentService,
        IInventoryService inventoryService,
        ILogger logger,
        IAuditService auditService)
    {
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _logger = logger;
        _auditService = auditService;
    }
    
    public async Task<MlResult<ProcessedOrder>> ProcessOrderWithContextAsync(OrderRequest orderRequest)
    {
        var correlationId = Guid.NewGuid().ToString();
        
        return await ValidateOrderRequest(orderRequest)
            .ExecSelf(
                validOrder => _logger.LogInformation($"[{correlationId}] Order validation successful for order {validOrder.OrderId}"),
                errors => _logger.LogWarning($"[{correlationId}] Order validation failed: {errors.FirstErrorMessage}")
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validOrder => 
                await ReserveInventoryWithContextAsync(validOrder, correlationId))
            .ExecSelfAsync(
                async reservedOrder => await _auditService.LogOrderEventAsync(correlationId, "InventoryReserved", reservedOrder.OrderId),
                async errors => await _auditService.LogOrderEventAsync(correlationId, "InventoryReservationFailed", errors.FirstErrorMessage)
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async reservedOrder => 
                await ProcessPaymentWithContextAsync(reservedOrder, correlationId))
            .ExecSelfAsync(
                async paidOrder => await _auditService.LogOrderEventAsync(correlationId, "PaymentProcessed", paidOrder.OrderId),
                async errors => await _auditService.LogOrderEventAsync(correlationId, "PaymentFailed", errors.FirstErrorMessage)
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async paidOrder => 
                await FinalizeOrderWithContextAsync(paidOrder, correlationId))
            .ExecSelfAsync(
                async finalOrder => 
                {
                    _logger.LogInformation($"[{correlationId}] Order {finalOrder.OrderId} processed successfully");
                    await _auditService.LogOrderCompletionAsync(correlationId, finalOrder);
                },
                async errors => 
                {
                    _logger.LogError($"[{correlationId}] Order processing failed: {errors.FirstErrorMessage}");
                    await _auditService.LogOrderFailureAsync(correlationId, errors);
                }
            );
    }
    
    private async Task<MlResult<ReservedOrder>> ReserveInventoryWithContextAsync(OrderRequest validOrder, string correlationId)
    {
        try
        {
            var reservationResults = new List<InventoryReservation>();
            
            foreach (var item in validOrder.Items)
            {
                _logger.LogDebug($"[{correlationId}] Reserving {item.Quantity} units of {item.ProductId}");
                
                var reservation = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
                
                if (!reservation.Success)
                {
                    // Liberar reservas previas
                    await ReleaseReservationsAsync(reservationResults, correlationId);
                    
                    return MlResult<ReservedOrder>.Fail(
                        $"Inventory reservation failed for product {item.ProductId}: {reservation.ErrorMessage}");
                }
                
                reservationResults.Add(reservation);
            }
            
            var reservedOrder = new ReservedOrder
            {
                OrderId = validOrder.OrderId,
                OriginalOrder = validOrder,
                Reservations = reservationResults,
                ReservedAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<ReservedOrder>.Valid(reservedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Inventory reservation exception: {ex.Message}");
            return MlResult<ReservedOrder>.Fail($"Inventory service error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PaidOrder>> ProcessPaymentWithContextAsync(ReservedOrder reservedOrder, string correlationId)
    {
        try
        {
            var totalAmount = reservedOrder.OriginalOrder.Items.Sum(i => i.Price * i.Quantity);
            
            _logger.LogInformation($"[{correlationId}] Processing payment of ${totalAmount} for order {reservedOrder.OrderId}");
            
            var paymentRequest = new PaymentRequest
            {
                OrderId = reservedOrder.OrderId,
                Amount = totalAmount,
                Currency = "USD",
                PaymentMethodId = reservedOrder.OriginalOrder.PaymentMethodId,
                CorrelationId = correlationId
            };
            
            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);
            
            if (!paymentResult.Success)
            {
                // Liberar reservas ya que el pago falló
                await ReleaseReservationsAsync(reservedOrder.Reservations, correlationId);
                
                return MlResult<PaidOrder>.Fail(
                    $"Payment processing failed: {paymentResult.ErrorMessage}");
            }
            
            var paidOrder = new PaidOrder
            {
                OrderId = reservedOrder.OrderId,
                ReservedOrder = reservedOrder,
                PaymentResult = paymentResult,
                PaidAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<PaidOrder>.Valid(paidOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Payment processing exception: {ex.Message}");
            
            // Liberar reservas en caso de excepción
            await ReleaseReservationsAsync(reservedOrder.Reservations, correlationId);
            
            return MlResult<PaidOrder>.Fail($"Payment service error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedOrder>> FinalizeOrderWithContextAsync(PaidOrder paidOrder, string correlationId)
    {
        try
        {
            // Confirmar todas las reservas
            foreach (var reservation in paidOrder.ReservedOrder.Reservations)
            {
                await _inventoryService.ConfirmReservationAsync(reservation.ReservationId);
                _logger.LogDebug($"[{correlationId}] Confirmed reservation {reservation.ReservationId}");
            }
            
            var processedOrder = new ProcessedOrder
            {
                OrderId = paidOrder.OrderId,
                CustomerId = paidOrder.ReservedOrder.OriginalOrder.CustomerId,
                TotalAmount = paidOrder.PaymentResult.Amount,
                Status = OrderStatus.Completed,
                PaymentTransactionId = paidOrder.PaymentResult.TransactionId,
                ProcessedAt = DateTime.UtcNow,
                CorrelationId = correlationId
            };
            
            return MlResult<ProcessedOrder>.Valid(processedOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{correlationId}] Order finalization exception: {ex.Message}");
            return MlResult<ProcessedOrder>.Fail($"Order finalization error: {ex.Message}");
        }
    }
    
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        if (string.IsNullOrWhiteSpace(request.OrderId))
            return MlResult<OrderRequest>.Fail("Order ID is required");
            
        if (request.CustomerId <= 0)
            return MlResult<OrderRequest>.Fail("Valid customer ID is required");
            
        if (request.Items?.Any() != true)
            return MlResult<OrderRequest>.Fail("Order must contain at least one item");
            
        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.ProductId))
                return MlResult<OrderRequest>.Fail($"Product ID is required for all items");
                
            if (item.Quantity <= 0)
                return MlResult<OrderRequest>.Fail($"Quantity must be positive for product {item.ProductId}");
                
            if (item.Price <= 0)
                return MlResult<OrderRequest>.Fail($"Price must be positive for product {item.ProductId}");
        }
        
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task ReleaseReservationsAsync(List<InventoryReservation> reservations, string correlationId)
    {
        foreach (var reservation in reservations)
        {
            try
            {
                await _inventoryService.ReleaseReservationAsync(reservation.ReservationId);
                _logger.LogDebug($"[{correlationId}] Released reservation {reservation.ReservationId}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[{correlationId}] Failed to release reservation {reservation.ReservationId}: {ex.Message}");
            }
        }
    }
}

// Clases de apoyo
public class OrderRequest
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public string PaymentMethodId { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class ReservedOrder
{
    public string OrderId { get; set; }
    public OrderRequest OriginalOrder { get; set; }
    public List<InventoryReservation> Reservations { get; set; }
    public DateTime ReservedAt { get; set; }
    public string CorrelationId { get; set; }
}

public class PaidOrder
{
    public string OrderId { get; set; }
    public ReservedOrder ReservedOrder { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public DateTime PaidAt { get; set; }
    public string CorrelationId { get; set; }
}

public class ProcessedOrder
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentTransactionId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string CorrelationId { get; set; }
}

public class InventoryReservation
{
    public string ReservationId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class PaymentRequest
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentMethodId { get; set; }
    public string CorrelationId { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string ErrorMessage { get; set; }
}

public enum OrderStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}

// Interfaces
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

public interface IInventoryService
{
    Task<InventoryReservation> ReserveAsync(string productId, int quantity);
    Task ConfirmReservationAsync(string reservationId);
    Task ReleaseReservationAsync(string reservationId);
}

public interface IAuditService
{
    Task LogOrderEventAsync(string correlationId, string eventType, string details);
    Task LogOrderCompletionAsync(string correlationId, ProcessedOrder order);
    Task LogOrderFailureAsync(string correlationId, MlErrorsDetails errors);
}
```

### Ejemplo 2: Sistema de Análisis de Datos con Preservación de Contexto

```csharp
public class DataAnalysisService
{
    private readonly IDataProcessor _dataProcessor;
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger _logger;
    
    public DataAnalysisService(
        IDataProcessor dataProcessor,
        IMetricsCollector metricsCollector,
        ILogger logger)
    {
        _dataProcessor = dataProcessor;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }
    
    public async Task<MlResult<AnalysisResult>> AnalyzeDataWithContextAsync(DataSet dataSet)
    {
        var analysisId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateDataSet(dataSet)
            .ExecSelf(
                validDataSet => _logger.LogInformation($"[{analysisId}] DataSet validation successful. Rows: {validDataSet.RowCount}, Columns: {validDataSet.ColumnCount}"),
                errors => _logger.LogWarning($"[{analysisId}] DataSet validation failed: {errors.FirstErrorMessage}")
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validDataSet => 
                await PreprocessDataWithContextAsync(validDataSet, analysisId),
                ex => $"Data preprocessing failed: {ex.Message}")
            .ExecSelfAsync(
                async preprocessedData => 
                {
                    _logger.LogInformation($"[{analysisId}] Data preprocessing completed. Original rows: {preprocessedData.OriginalDataSet.RowCount}, Processed rows: {preprocessedData.ProcessedRowCount}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Preprocessing", 
                        Success = true, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                },
                async errors => 
                {
                    _logger.LogError($"[{analysisId}] Data preprocessing failed: {errors.FirstErrorMessage}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Preprocessing", 
                        Success = false, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                }
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async preprocessedData => 
                await PerformStatisticalAnalysisAsync(preprocessedData, analysisId),
                ex => $"Statistical analysis failed: {ex.Message}")
            .ExecSelfAsync(
                async analysisResult => 
                {
                    _logger.LogInformation($"[{analysisId}] Statistical analysis completed. Insights: {analysisResult.InsightCount}");
                    await _metricsCollector.RecordAsync(new ProcessingMetrics 
                    { 
                        AnalysisId = analysisId, 
                        Stage = "Analysis", 
                        Success = true, 
                        Duration = DateTime.UtcNow - startTime 
                    });
                },
                async errors => 
                {
                    _logger.LogError($"[{analysisId}] Statistical analysis failed: {errors.FirstErrorMessage}");
                    
                    // Aquí es donde el contexto preservado es valioso
                    // Los errores contendrán información sobre qué datos se estaban analizando
                    await LogDetailedFailureAsync(analysisId, errors, startTime);
                }
            );
    }
    
    public MlResult<ProcessedData> ProcessDataWithLogging(RawData rawData, string userId)
    {
        return ValidateRawData(rawData)
            .ExecSelf(
                validData => _logger.LogInformation($"User {userId} processing valid data: {validData.Size} bytes"),
                errors => _logger.LogWarning($"User {userId} attempted to process invalid data: {errors.FirstErrorMessage}")
            )
            .TryBindSaveValueInDetailsIfFaildFuncResult(validData => 
            {
                // Esta operación puede fallar y queremos preservar los datos originales
                if (validData.Size > 10_000_000) // 10MB
                {
                    return MlResult<ProcessedData>.Fail("Data size exceeds maximum allowed limit");
                }
                
                var processed = _dataProcessor.Process(validData);
                
                if (processed.Quality < 0.8) // 80% quality threshold
                {
                    return MlResult<ProcessedData>.Fail($"Processed data quality too low: {processed.Quality:P}");
                }
                
                return MlResult<ProcessedData>.Valid(processed);
            },
            ex => $"Data processing error: {ex.Message}")
            .ExecSelf(
                processedData => 
                {
                    _logger.LogInformation($"User {userId} successfully processed data. Quality: {processedData.Quality:P}");
                    _metricsCollector.Record(new ProcessingMetrics 
                    { 
                        UserId = userId, 
                        Success = true, 
                        Quality = processedData.Quality 
                    });
                },
                errors => 
                {
                    _logger.LogError($"User {userId} data processing failed: {errors.FirstErrorMessage}");
                    
                    // Si tenemos el contexto preservado, podemos hacer análisis más detallado
                    if (errors.HasValueDetails)
                    {
                        var originalData = errors.GetValueDetail<RawData>();
                        _logger.LogDebug($"Failed processing context - Original data size: {originalData.Size}, Type: {originalData.Type}");
                        
                        _metricsCollector.Record(new ProcessingMetrics 
                        { 
                            UserId = userId, 
                            Success = false, 
                            OriginalDataSize = originalData.Size,
                            FailureReason = errors.FirstErrorMessage
                        });
                    }
                }
            );
    }
    
    private async Task<MlResult<PreprocessedData>> PreprocessDataWithContextAsync(DataSet validDataSet, string analysisId)
    {
        try
        {
            _logger.LogDebug($"[{analysisId}] Starting data preprocessing");
            
            // Simular preprocesamiento que puede fallar
            if (validDataSet.HasMissingValues && validDataSet.MissingValuePercentage > 0.3)
            {
                return MlResult<PreprocessedData>.Fail(
                    $"Too many missing values: {validDataSet.MissingValuePercentage:P} (max allowed: 30%)");
            }
            
            if (validDataSet.HasOutliers && validDataSet.OutlierPercentage > 0.1)
            {
                return MlResult<PreprocessedData>.Fail(
                    $"Too many outliers detected: {validDataSet.OutlierPercentage:P} (max allowed: 10%)");
            }
            
            var preprocessed = await _dataProcessor.PreprocessAsync(validDataSet);
            
            var preprocessedData = new PreprocessedData
            {
                OriginalDataSet = validDataSet,
                ProcessedRowCount = preprocessed.RowCount,
                ProcessedColumnCount = preprocessed.ColumnCount,
                QualityScore = preprocessed.QualityScore,
                PreprocessedAt = DateTime.UtcNow,
                AnalysisId = analysisId
            };
            
            return MlResult<PreprocessedData>.Valid(preprocessedData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{analysisId}] Preprocessing exception: {ex.Message}");
            throw; // Se captura en TryBindSaveValueInDetailsIfFaildFuncResultAsync
        }
    }
    
    private async Task<MlResult<AnalysisResult>> PerformStatisticalAnalysisAsync(PreprocessedData preprocessedData, string analysisId)
    {
        try
        {
            _logger.LogDebug($"[{analysisId}] Starting statistical analysis");
            
            if (preprocessedData.QualityScore < 0.7)
            {
                return MlResult<AnalysisResult>.Fail(
                    $"Data quality too low for reliable analysis: {preprocessedData.QualityScore:P}");
            }
            
            var statisticalResults = await _dataProcessor.PerformStatisticalAnalysisAsync(preprocessedData);
            
            if (statisticalResults.Confidence < 0.95)
            {
                return MlResult<AnalysisResult>.Fail(
                    $"Analysis confidence too low: {statisticalResults.Confidence:P} (required: 95%)");
            }
            
            var analysisResult = new AnalysisResult
            {
                PreprocessedData = preprocessedData,
                StatisticalResults = statisticalResults,
                InsightCount = statisticalResults.Insights.Count,
                Confidence = statisticalResults.Confidence,
                CompletedAt = DateTime.UtcNow,
                AnalysisId = analysisId
            };
            
            return MlResult<AnalysisResult>.Valid(analysisResult);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[{analysisId}] Statistical analysis exception: {ex.Message}");
            throw; // Se captura en TryBindSaveValueInDetailsIfFaildFuncResultAsync
        }
    }
    
    private async Task LogDetailedFailureAsync(string analysisId, MlErrorsDetails errors, DateTime startTime)
    {
        try
        {
            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogError($"[{analysisId}] Analysis failed after {duration.TotalMinutes:F2} minutes");
            
            // Si tenemos contexto preservado, podemos hacer análisis detallado del fallo
            if (errors.HasValueDetails)
            {
                var failureContext = new AnalysisFailureContext
                {
                    AnalysisId = analysisId,
                    Duration = duration,
                    ErrorMessage = errors.FirstErrorMessage,
                    AllErrors = errors.AllErrors,
                    PreservedContexts = new List<object>()
                };
                
                // Extraer todos los contextos preservados
                foreach (var detail in errors.ValueDetails)
                {
                    failureContext.PreservedContexts.Add(detail.Value);
                    
                    // Log específico según el tipo de contexto
                    switch (detail.Value)
                    {
                        case DataSet dataSet:
                            _logger.LogDebug($"[{analysisId}] Failed with DataSet context - Rows: {dataSet.RowCount}, Columns: {dataSet.ColumnCount}");
                            break;
                        case PreprocessedData preprocessed:
                            _logger.LogDebug($"[{analysisId}] Failed with PreprocessedData context - Quality: {preprocessed.QualityScore:P}");
                            break;
                    }
                }
                
                await _metricsCollector.RecordFailureAsync(failureContext);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"[{analysisId}] Failed to log detailed failure information: {ex.Message}");
        }
    }
    
    // Métodos auxiliares...
    private MlResult<DataSet> ValidateDataSet(DataSet dataSet)
    {
        if (dataSet == null)
            return MlResult<DataSet>.Fail("DataSet cannot be null");
            
        if (dataSet.RowCount <= 0)
            return MlResult<DataSet>.Fail("DataSet must contain at least one row");
            
        if (dataSet.ColumnCount <= 0)
            return MlResult<DataSet>.Fail("DataSet must contain at least one column");
            
        if (dataSet.RowCount < 10)
            return MlResult<DataSet>.Fail("DataSet must contain at least 10 rows for reliable analysis");
            
        return MlResult<DataSet>.Valid(dataSet);
    }
    
    private MlResult<RawData> ValidateRawData(RawData rawData)
    {
        if (rawData == null)
            return MlResult<RawData>.Fail("Raw data cannot be null");
            
        if (rawData.Size <= 0)
            return MlResult<RawData>.Fail("Raw data must have positive size");
            
        if (string.IsNullOrWhiteSpace(rawData.Type))
            return MlResult<RawData>.Fail("Raw data type must be specified");
            
        return MlResult<RawData>.Valid(rawData);
    }
}

// Clases de apoyo
public class DataSet
{
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public bool HasMissingValues { get; set; }
    public double MissingValuePercentage { get; set; }
    public bool HasOutliers { get; set; }
    public double OutlierPercentage { get; set; }
}

public class PreprocessedData
{
    public DataSet OriginalDataSet { get; set; }
    public int ProcessedRowCount { get; set; }
    public int ProcessedColumnCount { get; set; }
    public double QualityScore { get; set; }
    public DateTime PreprocessedAt { get; set; }
    public string AnalysisId { get; set; }
}

public class AnalysisResult
{
    public PreprocessedData PreprocessedData { get; set; }
    public StatisticalResults StatisticalResults { get; set; }
    public int InsightCount { get; set; }
    public double Confidence { get; set; }
    public DateTime CompletedAt { get; set; }
    public string AnalysisId { get; set; }
}

public class StatisticalResults
{
    public List<Insight> Insights { get; set; }
    public double Confidence { get; set; }
}

public class Insight
{
    public string Type { get; set; }
    public string Description { get; set; }
    public double Significance { get; set; }
}

public class RawData
{
    public long Size { get; set; }
    public string Type { get; set; }
}

public class ProcessedData
{
    public double Quality { get; set; }
}

public class ProcessingMetrics
{
    public string AnalysisId { get; set; }
    public string UserId { get; set; }
    public string Stage { get; set; }
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public double Quality { get; set; }
    public long OriginalDataSize { get; set; }
    public string FailureReason { get; set; }
}

public class AnalysisFailureContext
{
    public string AnalysisId { get; set; }
    public TimeSpan Duration { get; set; }
    public string ErrorMessage { get; set; }
    public string[] AllErrors { get; set; }
    public List<object> PreservedContexts { get; set; }
}

// Interfaces
public interface IDataProcessor
{
    ProcessedData Process(RawData rawData);
    Task<PreprocessedData> PreprocessAsync(DataSet dataSet);
    Task<StatisticalResults> PerformStatisticalAnalysisAsync(PreprocessedData data);
}

public interface IMetricsCollector
{
    Task RecordAsync(ProcessingMetrics metrics);
    void Record(ProcessingMetrics metrics);
    Task RecordFailureAsync(AnalysisFailureContext context);
}
```

### Ejemplo 3: Sistema de Debugging y Diagnóstico

```csharp
public class DiagnosticService
{
    private readonly ILogger _logger;
    private readonly IDiagnosticCollector _diagnosticCollector;
    
    public DiagnosticService(ILogger logger, IDiagnosticCollector diagnosticCollector)
    {
        _logger = logger;
        _diagnosticCollector = diagnosticCollector;
    }
    
    public async Task<MlResult<ProcessingResult>> ProcessWithDiagnosticsAsync<T>(
        T inputData, 
        string operationName,
        Func<T, Task<MlResult<ProcessingResult>>> processingFunction)
    {
        var diagnosticSession = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateInput(inputData)
            .ExecSelf(
                validInput => LogDiagnostic(diagnosticSession, "InputValidation", "Success", validInput),
                errors => LogDiagnostic(diagnosticSession, "InputValidation", "Failed", errors.FirstErrorMessage)
            )
            .TryBindSaveValueInDetailsIfFaildFuncResultAsync(async validInput => 
            {
                var result = await processingFunction(validInput);
                return result;
            },
            ex => $"Processing function failed: {ex.Message}")
            .ExecSelfAsync(
                async processingResult => await CollectSuccessDiagnosticsAsync(diagnosticSession, operationName, processingResult, startTime),
                async errors => await CollectFailureDiagnosticsAsync(diagnosticSession, operationName, errors, startTime)
            );
    }
    
    public MlResult<ValidationResult> ValidateWithDiagnostics<T>(T data, I…