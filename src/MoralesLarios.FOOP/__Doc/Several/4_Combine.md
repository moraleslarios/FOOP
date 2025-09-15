# MlResult Combine - Combinación de Múltiples Valores en Tuplas

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Tipos de Métodos Combine](#tipos-de-métodos-combine)
4. [Variantes Asíncronas](#variantes-asíncronas)
5. [Ejemplos Prácticos](#ejemplos-prácticos)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Comparación con Bind y Map](#comparación-con-bind-y-map)

---

## Introducción

Los métodos `Combine` proporcionan una forma de **combinar múltiples valores en tuplas**, permitiendo agrupar tanto valores `MlResult<T>` como valores directos en estructuras de datos cohesivas. Estos métodos son fundamentales para operaciones que requieren múltiples datos de entrada y deben preservar el estado de error si alguno de los `MlResult` es fallido.

### Propósito Principal

- **Agregación de Datos**: Combinar múltiples valores relacionados en una estructura
- **Preservación de Errores**: Mantener errores si algún `MlResult` es fallido
- **Construcción de Tuplas**: Crear tuplas tipadas con valores heterogéneos
- **Composición de Resultados**: Unir resultados de operaciones independientes

---

## Análisis de los Métodos

### Filosofía de Combine

```
MlResult<T1> + T2 → Combine → MlResult<(T1, T2)>
     ↓           ↓      ↓           ↓
  Valid(v1) + v2 → Valid((v1, v2))
     ↓           ↓      ↓           ↓
  Fail(err) + v2 → Fail(err)
```

### Características Principales

1. **Combinación Heterogénea**: Mezcla `MlResult<T>` con valores directos
2. **Propagación de Errores**: Si algún `MlResult` falla, el resultado final falla
3. **Tuplas Tipadas**: Retorna tuplas fuertemente tipadas
4. **Escalabilidad**: Soporte hasta 8 elementos en la tupla
5. **Flexibilidad**: Múltiples patrones de combinación

---

## Tipos de Métodos Combine

### 1. MlResult + Valor Directo

**Propósito**: Combinar un `MlResult<T>` con un valor directo

```csharp
public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(
    this MlResult<TResult1> source, 
    TResult2 otherValue)
```

**Ejemplo Básico**:
```csharp
var user = GetUser(userId);  // MlResult<User>
var timestamp = DateTime.UtcNow;  // DateTime

var result = user.Combine(timestamp);
// Si user es válido: MlResult<(User, DateTime)>.Valid((user, timestamp))
// Si user es fallido: MlResult<(User, DateTime)>.Fail(errors)
```

### 2. Valor Directo + MlResult

**Propósito**: Combinar un valor directo con un `MlResult<T>`

```csharp
public static MlResult<(TResult1 value1, TResult2 value2)> Combine<TResult1, TResult2>(
    this TResult1 source, 
    MlResult<TResult2> mlResultValue)
```

**Ejemplo**:
```csharp
var operationId = Guid.NewGuid();  // Guid
var userData = GetUserData(userId);  // MlResult<UserData>

var result = operationId.Combine(userData);
// Si userData es válido: MlResult<(Guid, UserData)>.Valid((operationId, userData))
// Si userData es fallido: MlResult<(Guid, UserData)>.Fail(errors)
```

### 3. Tuplas + Valores Directos

**Propósito**: Extender tuplas existentes con valores directos

```csharp
public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(
    this (TResult1 value1, TResult2 value2) source,
    TResult3 newValue)
```

**Ejemplo**:
```csharp
var userInfo = (userId: 123, userName: "john_doe");
var sessionId = "session_abc123";

var result = userInfo.Combine(sessionId);
// Resultado: MlResult<(int, string, string)>.Valid((123, "john_doe", "session_abc123"))
```

### 4. Tuplas + MlResult

**Propósito**: Extender tuplas con `MlResult<T>`

```csharp
public static MlResult<(TResult1, TResult2, TResult3)> Combine<TResult1, TResult2, TResult3>(
    this (TResult1 value1, TResult2 value2) source, 
    MlResult<TResult3> mlResultValue)
```

**Ejemplo**:
```csharp
var orderInfo = (orderId: 456, customerId: 789);
var paymentResult = ProcessPayment(paymentData);  // MlResult<PaymentInfo>

var result = orderInfo.Combine(paymentResult);
// Si payment es válido: MlResult<(int, int, PaymentInfo)>.Valid((456, 789, paymentInfo))
// Si payment es fallido: MlResult<(int, int, PaymentInfo)>.Fail(errors)
```

---

## Variantes Asíncronas

### `CombineAsync<T>()` - Todas las variantes

```csharp
// MlResult síncrono + valor
public static Task<MlResult<(TResult1, TResult2)>> CombineAsync<TResult1, TResult2>(
    this MlResult<TResult1> source, 
    TResult2 otherValue)

// MlResult asíncrono + valor
public static async Task<MlResult<(TResult1, TResult2)>> CombineAsync<TResult1, TResult2>(
    this Task<MlResult<TResult1>> sourceAsync, 
    TResult2 otherValue)

// Valor + MlResult asíncrono
public static async Task<MlResult<(TResult1, TResult2)>> CombineAsync<TResult1, TResult2>(
    this TResult1 source,
    Task<MlResult<TResult2>> mlResultValueAsync)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Procesamiento de Pedidos

```csharp
public class OrderProcessingService
{
    private readonly IUserService _userService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly IShippingService _shippingService;
    
    public async Task<MlResult<CompleteOrderInfo>> ProcessCompleteOrderAsync(OrderRequest request)
    {
        var processingId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        
        // Combinar ID de procesamiento con validación de usuario
        var userValidation = await GetValidatedUserAsync(request.UserId)
            .CombineAsync(processingId);
        
        if (userValidation.IsFailed)
            return userValidation.Errors.ToMlResultFail<CompleteOrderInfo>();
        
        // Combinar información del usuario con validación de inventario
        var inventoryValidation = await userValidation.Value
            .Combine(await ValidateInventoryAsync(request.Items));
        
        if (inventoryValidation.IsFailed)
            return inventoryValidation.Errors.ToMlResultFail<CompleteOrderInfo>();
        
        // Combinar con información de pago
        var paymentValidation = await inventoryValidation.Value
            .Combine(await ProcessPaymentAsync(request.PaymentInfo));
        
        if (paymentValidation.IsFailed)
            return paymentValidation.Errors.ToMlResultFail<CompleteOrderInfo>();
        
        // Combinar con información de envío y timestamp
        var shippingInfo = await CalculateShippingAsync(request.ShippingAddress);
        var finalResult = paymentValidation.Value
            .Combine(shippingInfo)
            .Map(combined => combined.Item1.Combine(timestamp));
        
        return finalResult.Match(
            valid: data => MlResult<CompleteOrderInfo>.Valid(new CompleteOrderInfo
            {
                ProcessingId = data.Item1.Item1.Item2, // processingId
                User = data.Item1.Item1.Item1.Item1,   // user
                InventoryReservation = data.Item1.Item1.Item1.Item2, // inventory
                PaymentResult = data.Item1.Item2,      // payment
                ShippingDetails = data.Item2,          // shipping
                ProcessedAt = data.Item1.Item3         // timestamp
            }),
            fail: errors => MlResult<CompleteOrderInfo>.Fail(errors.AllErrors)
        );
    }
    
    public async Task<MlResult<OrderSummary>> CreateOrderSummaryAsync(int orderId)
    {
        var order = await GetOrderAsync(orderId);
        var auditId = Guid.NewGuid();
        var reportGeneratedAt = DateTime.UtcNow;
        
        return await order
            .Combine(auditId)
            .CombineAsync(await GetOrderItemsAsync(orderId))
            .CombineAsync(await GetCustomerInfoAsync(order?.CustomerId ?? 0))
            .CombineAsync(await GetPaymentHistoryAsync(orderId))
            .CombineAsync(reportGeneratedAt)
            .MapAsync(async combined =>
            {
                var (((((orderData, auditIdValue), orderItems), customer), paymentHistory), timestamp) = combined;
                
                return new OrderSummary
                {
                    AuditId = auditIdValue,
                    Order = orderData,
                    Items = orderItems.ToArray(),
                    Customer = customer,
                    PaymentHistory = paymentHistory.ToArray(),
                    TotalAmount = orderItems.Sum(i => i.Price * i.Quantity),
                    GeneratedAt = timestamp
                };
            });
    }
    
    public async Task<MlResult<ValidationReport>> ValidateOrderDataAsync(OrderRequest request)
    {
        var validationId = Guid.NewGuid();
        var validationStartTime = DateTime.UtcNow;
        
        // Validaciones paralelas combinadas
        var userValidation = await GetUserAsync(request.UserId)
            .NullToFailedAsync("User not found");
        
        var itemsValidation = request.Items
            .EmptyToFailed("Order must contain items");
        
        var addressValidation = await ValidateShippingAddressAsync(request.ShippingAddress);
        
        // Combinar todas las validaciones
        return validationId
            .Combine(userValidation)
            .Bind(combined => combined.Item2.BoolToResult(
                condition: combined.Item2.IsActive && !combined.Item2.IsSuspended,
                errorMessage: "User account is not active"))
            .Bind(combined => combined.Combine(itemsValidation))
            .Bind(combined => combined.Combine(addressValidation))
            .Combine(validationStartTime)
            .Map(finalCombined =>
            {
                var validationEndTime = DateTime.UtcNow;
                var (((validationIdValue, validUser), validItems), validAddress), startTime) = finalCombined;
                
                return new ValidationReport
                {
                    ValidationId = validationIdValue,
                    UserId = validUser.Id,
                    UserName = validUser.Name,
                    ItemCount = validItems.Count(),
                    ShippingAddress = validAddress,
                    ValidationDuration = validationEndTime - startTime,
                    ValidatedAt = validationEndTime,
                    Status = "Valid"
                };
            });
    }
    
    // Métodos auxiliares
    private async Task<MlResult<User>> GetValidatedUserAsync(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        return user
            .NullToFailed($"User {userId} not found")
            .Bind(u => u.BoolToResult(u.IsActive, "User account is inactive"));
    }
    
    private async Task<MlResult<InventoryReservation>> ValidateInventoryAsync(IEnumerable<OrderItem> items)
    {
        try
        {
            var reservation = await _inventoryService.ReserveItemsAsync(items);
            return MlResult<InventoryReservation>.Valid(reservation);
        }
        catch (Exception ex)
        {
            return MlResult<InventoryReservation>.Fail($"Inventory validation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PaymentResult>> ProcessPaymentAsync(PaymentInfo paymentInfo)
    {
        try
        {
            var result = await _paymentService.ProcessAsync(paymentInfo);
            return result.Success 
                ? MlResult<PaymentResult>.Valid(result)
                : MlResult<PaymentResult>.Fail("Payment processing failed");
        }
        catch (Exception ex)
        {
            return MlResult<PaymentResult>.Fail($"Payment error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ShippingDetails>> CalculateShippingAsync(Address address)
    {
        try
        {
            var details = await _shippingService.CalculateShippingAsync(address);
            return MlResult<ShippingDetails>.Valid(details);
        }
        catch (Exception ex)
        {
            return MlResult<ShippingDetails>.Fail($"Shipping calculation failed: {ex.Message}");
        }
    }
}

// Clases de apoyo
public class CompleteOrderInfo
{
    public Guid ProcessingId { get; set; }
    public User User { get; set; }
    public InventoryReservation InventoryReservation { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public ShippingDetails ShippingDetails { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class OrderSummary
{
    public Guid AuditId { get; set; }
    public Order Order { get; set; }
    public OrderItem[] Items { get; set; }
    public Customer Customer { get; set; }
    public PaymentHistory[] PaymentHistory { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class ValidationReport
{
    public Guid ValidationId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ItemCount { get; set; }
    public Address ShippingAddress { get; set; }
    public TimeSpan ValidationDuration { get; set; }
    public DateTime ValidatedAt { get; set; }
    public string Status { get; set; }
}

public class OrderRequest
{
    public int UserId { get; set; }
    public IEnumerable<OrderItem> Items { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
    public Address ShippingAddress { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class InventoryReservation
{
    public string ReservationId { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
}

public class ShippingDetails
{
    public string Method { get; set; }
    public decimal Cost { get; set; }
    public DateTime EstimatedDelivery { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class PaymentHistory
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
}

public class PaymentInfo
{
    public string PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}
```

### Ejemplo 2: Sistema de Reportes Combinados

```csharp
public class ReportGenerationService
{
    private readonly IDataService _dataService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IFormattingService _formattingService;
    
    public async Task<MlResult<ComprehensiveReport>> GenerateComprehensiveReportAsync(ReportRequest request)
    {
        var reportId = Guid.NewGuid();
        var generationStartTime = DateTime.UtcNow;
        
        // Combinar ID del reporte con datos básicos
        var basicData = await GetBasicReportDataAsync(request)
            .CombineAsync(reportId);
        
        if (basicData.IsFailed)
            return basicData.Errors.ToMlResultFail<ComprehensiveReport>();
        
        // Combinar con datos analíticos
        var analyticsData = await basicData.Value
            .Combine(await GetAnalyticsDataAsync(request));
        
        if (analyticsData.IsFailed)
            return analyticsData.Errors.ToMlResultFail<ComprehensiveReport>();
        
        // Combinar con métricas de rendimiento
        var performanceData = await analyticsData.Value
            .Combine(await GetPerformanceMetricsAsync(request));
        
        if (performanceData.IsFailed)
            return performanceData.Errors.ToMlResultFail<ComprehensiveReport>();
        
        // Combinar con datos de comparación y timestamp
        var comparisonData = await GetComparisonDataAsync(request);
        var finalData = performanceData.Value
            .Combine(comparisonData)
            .Combine(generationStartTime);
        
        return finalData.Match(
            valid: data =>
            {
                var generationEndTime = DateTime.UtcNow;
                var ((((basicInfo, reportIdValue), analytics), performance), comparison), startTime) = data;
                
                return MlResult<ComprehensiveReport>.Valid(new ComprehensiveReport
                {
                    ReportId = reportIdValue,
                    BasicData = basicInfo,
                    Analytics = analytics,
                    Performance = performance,
                    Comparison = comparison,
                    GenerationDuration = generationEndTime - startTime,
                    GeneratedAt = generationEndTime
                });
            },
            fail: errors => MlResult<ComprehensiveReport>.Fail(errors.AllErrors)
        );
    }
    
    public async Task<MlResult<DashboardData>> CreateDashboardDataAsync(int userId)
    {
        var sessionId = Guid.NewGuid();
        var cacheKey = $"dashboard_{userId}_{sessionId}";
        
        // Operaciones paralelas que se combinan
        var userStatsTask = GetUserStatisticsAsync(userId);
        var recentActivityTask = GetRecentActivityAsync(userId);
        var notificationsTask = GetUserNotificationsAsync(userId);
        var preferencesTask = GetUserPreferencesAsync(userId);
        
        // Combinar resultados progresivamente
        return await sessionId
            .Combine(await userStatsTask)
            .CombineAsync(await recentActivityTask)
            .CombineAsync(await notificationsTask)
            .CombineAsync(await preferencesTask)
            .CombineAsync(cacheKey)
            .MapAsync(async combined =>
            {
                var (((((sessionIdValue, stats), activity), notifications), preferences), cacheKeyValue) = combined;
                
                // Procesar datos combinados
                var processedStats = await _analyticsService.ProcessStatisticsAsync(stats);
                var formattedActivity = await _formattingService.FormatActivityAsync(activity);
                
                return new DashboardData
                {
                    SessionId = sessionIdValue,
                    CacheKey = cacheKeyValue,
                    UserStatistics = processedStats,
                    RecentActivity = formattedActivity,
                    Notifications = notifications.ToArray(),
                    UserPreferences = preferences,
                    LastUpdated = DateTime.UtcNow
                };
            });
    }
    
    public MlResult<AuditTrailEntry> CreateAuditEntryAsync(string action, object data, int userId)
    {
        var auditId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var ipAddress = GetCurrentIpAddress();
        var userAgent = GetCurrentUserAgent();
        
        // Combinar información de auditoría
        return auditId
            .Combine(action)
            .Combine(JsonSerializer.Serialize(data))
            .Combine(userId)
            .Combine(timestamp)
            .Combine(ipAddress)
            .Combine(userAgent)
            .Map(combined =>
            {
                var (((((((auditIdValue, actionValue), dataJson), userIdValue), timestampValue), ipAddressValue), userAgentValue) = combined;
                
                return new AuditTrailEntry
                {
                    Id = auditIdValue,
                    Action = actionValue,
                    Data = dataJson,
                    UserId = userIdValue,
                    Timestamp = timestampValue,
                    IpAddress = ipAddressValue,
                    UserAgent = userAgentValue,
                    Hash = CalculateHash(auditIdValue, actionValue, dataJson, userIdValue, timestampValue)
                };
            });
    }
    
    // Métodos auxiliares
    private async Task<MlResult<BasicReportData>> GetBasicReportDataAsync(ReportRequest request)
    {
        try
        {
            var data = await _dataService.GetBasicDataAsync(request);
            return data?.Any() == true 
                ? MlResult<BasicReportData>.Valid(new BasicReportData { Data = data })
                : MlResult<BasicReportData>.Fail("No basic data available for report");
        }
        catch (Exception ex)
        {
            return MlResult<BasicReportData>.Fail($"Failed to get basic data: {ex.Message}");
        }
    }
    
    private async Task<MlResult<AnalyticsData>> GetAnalyticsDataAsync(ReportRequest request)
    {
        try
        {
            var data = await _analyticsService.GetAnalyticsAsync(request);
            return MlResult<AnalyticsData>.Valid(data);
        }
        catch (Exception ex)
        {
            return MlResult<AnalyticsData>.Fail($"Analytics data unavailable: {ex.Message}");
        }
    }
    
    private string GetCurrentIpAddress() => "127.0.0.1"; // Implementación simplificada
    private string GetCurrentUserAgent() => "UserAgent"; // Implementación simplificada
    private string CalculateHash(params object[] values) => "hash"; // Implementación simplificada
}

// Clases adicionales para el ejemplo
public class ComprehensiveReport
{
    public Guid ReportId { get; set; }
    public BasicReportData BasicData { get; set; }
    public AnalyticsData Analytics { get; set; }
    public PerformanceMetrics Performance { get; set; }
    public ComparisonData Comparison { get; set; }
    public TimeSpan GenerationDuration { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class DashboardData
{
    public Guid SessionId { get; set; }
    public string CacheKey { get; set; }
    public UserStatistics UserStatistics { get; set; }
    public RecentActivity RecentActivity { get; set; }
    public Notification[] Notifications { get; set; }
    public UserPreferences UserPreferences { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class AuditTrailEntry
{
    public Guid Id { get; set; }
    public string Action { get; set; }
    public string Data { get; set; }
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Hash { get; set; }
}

public class ReportRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string[] Metrics { get; set; }
}

public class BasicReportData
{
    public IEnumerable<object> Data { get; set; }
}

public class AnalyticsData
{
    public Dictionary<string, object> Metrics { get; set; }
}

public class PerformanceMetrics
{
    public double ResponseTime { get; set; }
    public int ThroughputPerSecond { get; set; }
}

public class ComparisonData
{
    public Dictionary<string, double> PreviousPeriod { get; set; }
}

public class UserStatistics
{
    public int LoginCount { get; set; }
    public DateTime LastLogin { get; set; }
}

public class RecentActivity
{
    public ActivityItem[] Items { get; set; }
}

public class ActivityItem
{
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
}

public class Notification
{
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsRead { get; set; }
}

public class UserPreferences
{
    public string Theme { get; set; }
    public string Language { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar Combine

```csharp
// ✅ Correcto: Agrupar datos relacionados
var orderInfo = GetOrder(orderId)
    .Combine(DateTime.UtcNow)
    .Combine(Guid.NewGuid());

// ✅ Correcto: Combinar validaciones independientes
var userValidation = GetUser(userId);
var inventoryValidation = ValidateInventory(items);
var combined = userValidation.Combine(inventoryValidation);

// ✅ Correcto: Construir contexto de procesamiento
var processingContext = operationId
    .Combine(GetUserData(userId))
    .Combine(GetConfiguration())
    .Combine(DateTime.UtcNow);

// ❌ Incorrecto: Para transformaciones simples
var result = GetUser(userId)
    .Combine("processed"); // Mejor usar Map para agregar información
```

### 2. Manejo de Tuplas Complejas

```csharp
// ✅ Correcto: Usar deconstrucción para claridad
var result = GetUserData(userId)
    .Combine(GetOrderData(orderId))
    .Combine(GetPaymentData(paymentId))
    .Map(combined =>
    {
        var (((userData, orderData), paymentData)) = combined;
        return new ProcessingContext
        {
            User = userData,
            Order = orderData,
            Payment = paymentData
        };
    });

// ✅ Correcto: Crear objetos específicos en lugar de tuplas grandes
var complexResult = basicData
    .Combine(analyticsData)
    .Combine(metricsData)
    .Map(combined => new ReportData
    {
        Basic = combined.Item1.Item1,
        Analytics = combined.Item1.Item2,
        Metrics = combined.Item2
    });

// ❌ Incorrecto: Tuplas excesivamente complejas sin deconstrucción
var badResult = data1.Combine(data2).Combine(data3).Combine(data4)
    .Map(x => ProcessData(x.Item1.Item1.Item1, x.Item1.Item1.Item2)); // Difícil de leer
```

### 3. Propagación de Errores

```csharp
// ✅ Correcto: Verificar errores en cada paso
var step1 = GetUserAsync(userId);
if (step1.Result.IsFailed)
    return step1.Result.Errors.ToMlResultFail<FinalResult>();

var step2 = await step1.CombineAsync(GetOrderAsync(orderId));
if (step2.IsFailed)
    return step2.Errors.ToMlResultFail<FinalResult>();

// ✅ Correcto: Usar Bind para validación continua
var result = GetUserAsync(userId)
    .BindAsync(async user => await user
        .Combine(await GetOrderAsync(orderId))
        .BindAsync(async combined => await ValidateCombinedData(combined)));

// ❌ Incorrecto: No verificar errores intermedios
var badResult = await GetUserAsync(userId)
    .CombineAsync(GetOrderAsync(orderId)) // Puede fallar pero no se verifica
    .CombineAsync(GetPaymentAsync(paymentId)); // Error se propaga sin control
```

### 4. Uso con Validaciones

```csharp
// ✅ Correcto: Combinar después de validaciones individuales
var validatedUser = GetUser(userId)
    .NullToFailed("User not found")
    .Bind(u => u.BoolToResult(u.IsActive, "User inactive"));

var validatedOrder = GetOrder(orderId)
    .NullToFailed("Order not found")
    .Bind(o => o.BoolToResult(o.Status == "Pending", "Order not pending"));

var combinedValidation = validatedUser.Combine(validatedOrder);

// ✅ Correcto: Validación después de combinación
var result = GetUserData(userId)
    .Combine(GetOrderData(orderId))
    .Bind(combined => combined.BoolToResult(
        condition: combined.Item1.UserId == combined.Item2.CustomerId,
        errorMessage: "User and order mismatch"));
```

---

## Comparación con Bind y Map

### Tabla Comparativa

| Método | Propósito | Entrada | Salida | Cuándo Usar |
|--------|-----------|---------|--------|-------------|
| `Combine` | Agrupar valores | `MlResult<T>` + otros | `MlResult<Tupla>` | Combinar datos relacionados |
| `Bind` | Encadenar operaciones | `MlResult<T>` | `MlResult<TResult>` | Operaciones secuenciales |
| `Map` | Transformar valores | `MlResult<T>` | `MlResult<TResult>` | Transformaciones simples |

### Ejemplo Comparativo

```csharp
// Combine: Agrupa múltiples valores
var combined = GetUser(userId)
    .Combine(GetOrder(orderId))
    .Combine(DateTime.UtcNow);

// Bind: Encadena operaciones dependientes
var processed = GetUser(userId)
    .Bind(user => GetUserOrders(user.Id))
    .Bind(orders => ProcessOrders(orders));

// Map: Transforma un valor
var transformed = GetUser(userId)
    .Map(user => user.ToDto());

// Uso combinado típico
var result = GetUser(userId)
    .Bind(user => ValidateUser(user))        // Validar
    .Combine(GetSystemInfo())                // Combinar con contexto
    .Map(combined => CreateUserSession(      // Transformar resultado
        combined.Item1, combined.Item2));
```

---

## Resumen

Los métodos `Combine` proporcionan **combinación de múltiples valores en tuplas**:

- **`Combine`**: Agrupa `MlResult<T>` y valores directos en tuplas tipadas
- **`CombineAsync`**: Soporte completo para operaciones asíncronas
- **Múltiples patrones**: MlResult+valor, valor+MlResult, tuplas+valores

**Casos de uso ideales**:
- **Agregación de datos** relacionados de múltiples fuentes
- **Construcción de contextos** de procesamiento complejos
- **Combinación de validaciones** independientes
- **Construcción de estructuras** de datos cohesivas

**Ventajas principales**:
- **Preservación de tipos** con tuplas fuertemente tipadas
- **Propagación automática de errores** si algún MlResult falla
- **Flexibilidad** en patrones de combinación
- **Escalabilidad** hasta 8 elementos en tuplas