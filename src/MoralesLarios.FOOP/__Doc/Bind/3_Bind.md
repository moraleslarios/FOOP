# MlResultActionsBind - Operaciones de Composición y Encadenamiento

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos Bind Básicos](#métodos-bind-básicos)
4. [Métodos TryBind - Captura de Excepciones](#métodos-trybind---captura-de-excepciones)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsBind` contiene las operaciones fundamentales de **binding** o encadenamiento para `MlResult<T>`. Estas operaciones permiten componer funciones que pueden fallar de manera segura, implementando el patrón monádico que es central en la programación funcional. El binding es esencial para crear pipelines de procesamiento donde cada paso puede fallar y los errores se propagan automáticamente.

### Propósito Principal

- **Composición Monádica**: Encadenar operaciones que devuelven `MlResult<T>`
- **Propagación de Errores**: Los errores se propagan automáticamente sin ejecutar pasos subsiguientes
- **Manejo Seguro de Excepciones**: Versiones `Try*` que capturan y convierten excepciones
- **Soporte Asíncrono Completo**: Todas las combinaciones de operaciones síncronas y asíncronas

---

## Análisis de la Clase

### Estructura y Filosofía

La clase implementa el patrón **Railway-Oriented Programming**:

```
Resultado Exitoso → Función → Resultado Exitoso → Función → ...
      ↓                              ↓                ↓
  Resultado Fallido ――――――――――→ Resultado Fallido ――→ ...
```

### Características Principales

1. **Composición Fluida**: Las operaciones se encadenan naturalmente
2. **Cortocircuito en Errores**: Si un paso falla, los siguientes no se ejecutan
3. **Preservación de Tipos**: El sistema de tipos garantiza operaciones seguras
4. **Flexibilidad de Mensajes**: Múltiples formas de manejar mensajes de error

---

## Métodos Bind Básicos

### `Bind<T, TReturn>()`

**Propósito**: Encadena una función que devuelve `MlResult<TReturn>` si el resultado actual es exitoso

```csharp
public static MlResult<TReturn> Bind<T, TReturn>(this MlResult<T> source, 
                                                 Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `func`: Función a ejecutar si `source` es exitoso

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `func(value)` y retorna su resultado
- Si `source` es fallido: Propaga el error sin ejecutar `func`

**Signatura Monádica**: `M<T> → (T → M<U>) → M<U>`

### `BindAsync<T, TReturn>()` - Conversión a Asíncrono

**Propósito**: Convierte el resultado de `Bind` a `Task<MlResult<TReturn>>`

```csharp
public static Task<MlResult<TReturn>> BindAsync<T, TReturn>(this MlResult<T> source, 
                                                            Func<T, MlResult<TReturn>> func)
```

**Comportamiento**: Ejecuta `Bind` y envuelve el resultado en una `Task`

### `BindAsync<T, TReturn>()` - Función Asíncrona

**Propósito**: Encadena una función asíncrona que devuelve `Task<MlResult<TReturn>>`

```csharp
public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this MlResult<T> source,
                                                                  Func<T, Task<MlResult<TReturn>>> funcAsync)
```

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `await funcAsync(value)`
- Si `source` es fallido: Retorna el error envuelto en `Task`

### `BindAsync<T, TReturn>()` - Fuente Asíncrona

**Propósito**: Encadena desde un `Task<MlResult<T>>` hacia una función asíncrona

```csharp
public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, Task<MlResult<TReturn>>> funcAsync)
```

**Comportamiento**: Espera `sourceAsync` y luego aplica el binding asíncrono

### `BindAsync<T, TReturn>()` - Función Síncrona desde Fuente Asíncrona

**Propósito**: Encadena una función síncrona desde un `Task<MlResult<T>>`

```csharp
public static async Task<MlResult<TReturn>> BindAsync<T, TReturn>(this Task<MlResult<T>> sourceAsync,
                                                                  Func<T, MlResult<TReturn>> func)
```

**Comportamiento**: Espera `sourceAsync` y aplica la función síncrona

---

## Métodos TryBind - Captura de Excepciones

### `TryBind<T, TReturn>()` - Mensaje Simple

**Propósito**: Versión segura de `Bind` que captura excepciones y las convierte en errores

```csharp
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T> source,
                                                    Func<T, MlResult<TReturn>> func,
                                                    string exceptionAditionalMessage = null!)
```

**Comportamiento**:
- Si `source` es exitoso y `func` no lanza excepción: Retorna el resultado de `func`
- Si `source` es exitoso y `func` lanza excepción: Captura la excepción y retorna error
- Si `source` es fallido: Propaga el error original

### `TryBind<T, TReturn>()` - Constructor de Mensaje

**Propósito**: Versión con función constructora de mensaje de error basada en la excepción

```csharp
public static MlResult<TReturn> TryBind<T, TReturn>(this MlResult<T> source,
                                                    Func<T, MlResult<TReturn>> func,
                                                    Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: Permite construir mensajes de error personalizados basados en la excepción capturada

### Versiones Asíncronas de TryBind

Cada variante de `TryBind` tiene sus correspondientes versiones asíncronas:

- `TryBindAsync` con mensaje simple
- `TryBindAsync` con constructor de mensaje
- `TryBindAsync` para funciones asíncronas
- `TryBindAsync` para fuentes asíncronas

---

## Variantes Asíncronas

### Matriz de Combinaciones

| Fuente | Función | Método |
|--------|---------|---------|
| `MlResult<T>` | `T → MlResult<U>` | `Bind` |
| `MlResult<T>` | `T → MlResult<U>` | `BindAsync` (conversión) |
| `MlResult<T>` | `T → Task<MlResult<U>>` | `BindAsync` |
| `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | `BindAsync` |
| `Task<MlResult<T>>` | `T → MlResult<U>` | `BindAsync` |

### Soporte Completo para TryBind

Todas las combinaciones anteriores están disponibles también para `TryBind`:

- Con mensaje de excepción simple (`string`)
- Con constructor de mensaje (`Func<Exception, string>`)

---

## Ejemplos Prácticos

### Ejemplo 1: Pipeline de Procesamiento Básico

```csharp
public class UserRegistrationService
{
    public MlResult<RegistrationResult> RegisterUser(UserRegistrationRequest request)
    {
        return ValidateUserRequest(request)
            .Bind(validRequest => CheckEmailAvailability(validRequest.Email))
            .Bind(availableEmail => CreateUserAccount(request, availableEmail))
            .Bind(createdUser => GenerateWelcomeEmail(createdUser))
            .Bind(emailData => new RegistrationResult 
            { 
                UserId = emailData.UserId, 
                EmailSent = true,
                RegistrationDate = DateTime.UtcNow 
            }.ToMlResult());
    }
    
    private MlResult<UserRegistrationRequest> ValidateUserRequest(UserRegistrationRequest request)
    {
        if (request == null)
            return MlResult<UserRegistrationRequest>.Fail("Registration request cannot be null");
            
        if (string.IsNullOrWhiteSpace(request.Email))
            return MlResult<UserRegistrationRequest>.Fail("Email is required");
            
        if (string.IsNullOrWhiteSpace(request.Password))
            return MlResult<UserRegistrationRequest>.Fail("Password is required");
            
        if (request.Password.Length < 8)
            return MlResult<UserRegistrationRequest>.Fail("Password must be at least 8 characters");
            
        return MlResult<UserRegistrationRequest>.Valid(request);
    }
    
    private MlResult<string> CheckEmailAvailability(string email)
    {
        // Simulación de verificación de disponibilidad
        var existingEmails = new[] { "admin@test.com", "user@test.com" };
        
        return existingEmails.Contains(email.ToLower())
            ? MlResult<string>.Fail($"Email '{email}' is already registered")
            : MlResult<string>.Valid(email);
    }
    
    private MlResult<User> CreateUserAccount(UserRegistrationRequest request, string validatedEmail)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = validatedEmail,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            // Simulación de guardado en base de datos
            return MlResult<User>.Valid(user);
        }
        catch (Exception ex)
        {
            return MlResult<User>.Fail($"Failed to create user account: {ex.Message}");
        }
    }
    
    private MlResult<EmailData> GenerateWelcomeEmail(User user)
    {
        var emailData = new EmailData
        {
            UserId = user.Id,
            To = user.Email,
            Subject = "Welcome to our platform!",
            Body = $"Hello {user.Name}, welcome to our platform!",
            GeneratedAt = DateTime.UtcNow
        };
        
        return MlResult<EmailData>.Valid(emailData);
    }
}

// Clases de apoyo
public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class EmailData
{
    public Guid UserId { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class RegistrationResult
{
    public Guid UserId { get; set; }
    public bool EmailSent { get; set; }
    public DateTime RegistrationDate { get; set; }
}
```

### Ejemplo 2: Pipeline Asíncrono con Servicios Externos

```csharp
public class OrderProcessingService
{
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;
    
    public OrderProcessingService(
        IPaymentService paymentService,
        IInventoryService inventoryService,
        INotificationService notificationService)
    {
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _notificationService = notificationService;
    }
    
    public async Task<MlResult<OrderConfirmation>> ProcessOrderAsync(OrderRequest request)
    {
        return await ValidateOrderRequest(request)
            .BindAsync(async validRequest => await ReserveInventoryAsync(validRequest))
            .BindAsync(async reservedOrder => await ProcessPaymentAsync(reservedOrder))
            .BindAsync(async paidOrder => await FinalizeOrderAsync(paidOrder))
            .BindAsync(async finalizedOrder => await SendConfirmationAsync(finalizedOrder));
    }
    
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        if (request.Items?.Any() != true)
            return MlResult<OrderRequest>.Fail("Order must contain at least one item");
            
        if (request.CustomerId <= 0)
            return MlResult<OrderRequest>.Fail("Valid customer ID is required");
            
        var totalAmount = request.Items.Sum(i => i.Price * i.Quantity);
        if (totalAmount <= 0)
            return MlResult<OrderRequest>.Fail("Order total must be greater than zero");
            
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task<MlResult<ReservedOrder>> ReserveInventoryAsync(OrderRequest validRequest)
    {
        try
        {
            var reservationResults = new List<InventoryReservation>();
            
            foreach (var item in validRequest.Items)
            {
                var reservationResult = await _inventoryService.ReserveItemAsync(item.ProductId, item.Quantity);
                if (!reservationResult.Success)
                {
                    // Liberar reservas anteriores
                    await ReleaseReservationsAsync(reservationResults);
                    return MlResult<ReservedOrder>.Fail($"Failed to reserve {item.ProductId}: {reservationResult.ErrorMessage}");
                }
                reservationResults.Add(reservationResult);
            }
            
            var reservedOrder = new ReservedOrder
            {
                OriginalRequest = validRequest,
                Reservations = reservationResults,
                ReservedAt = DateTime.UtcNow
            };
            
            return MlResult<ReservedOrder>.Valid(reservedOrder);
        }
        catch (Exception ex)
        {
            return MlResult<ReservedOrder>.Fail($"Inventory reservation failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<PaidOrder>> ProcessPaymentAsync(ReservedOrder reservedOrder)
    {
        try
        {
            var totalAmount = reservedOrder.OriginalRequest.Items.Sum(i => i.Price * i.Quantity);
            
            var paymentResult = await _paymentService.ProcessPaymentAsync(new PaymentRequest
            {
                CustomerId = reservedOrder.OriginalRequest.CustomerId,
                Amount = totalAmount,
                Currency = "USD",
                PaymentMethodId = reservedOrder.OriginalRequest.PaymentMethodId
            });
            
            if (!paymentResult.Success)
            {
                // Liberar reservas si el pago falla
                await ReleaseReservationsAsync(reservedOrder.Reservations);
                return MlResult<PaidOrder>.Fail($"Payment processing failed: {paymentResult.ErrorMessage}");
            }
            
            var paidOrder = new PaidOrder
            {
                ReservedOrder = reservedOrder,
                PaymentResult = paymentResult,
                PaidAt = DateTime.UtcNow
            };
            
            return MlResult<PaidOrder>.Valid(paidOrder);
        }
        catch (Exception ex)
        {
            await ReleaseReservationsAsync(reservedOrder.Reservations);
            return MlResult<PaidOrder>.Fail($"Payment processing error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<FinalizedOrder>> FinalizeOrderAsync(PaidOrder paidOrder)
    {
        try
        {
            // Confirmar reservas de inventario
            foreach (var reservation in paidOrder.ReservedOrder.Reservations)
            {
                await _inventoryService.ConfirmReservationAsync(reservation.ReservationId);
            }
            
            var finalizedOrder = new FinalizedOrder
            {
                OrderId = Guid.NewGuid(),
                PaidOrder = paidOrder,
                Status = OrderStatus.Confirmed,
                FinalizedAt = DateTime.UtcNow
            };
            
            return MlResult<FinalizedOrder>.Valid(finalizedOrder);
        }
        catch (Exception ex)
        {
            return MlResult<FinalizedOrder>.Fail($"Order finalization failed: {ex.Message}");
        }
    }
    
    private async Task<MlResult<OrderConfirmation>> SendConfirmationAsync(FinalizedOrder finalizedOrder)
    {
        try
        {
            await _notificationService.SendOrderConfirmationAsync(
                finalizedOrder.OrderId,
                finalizedOrder.PaidOrder.ReservedOrder.OriginalRequest.CustomerId);
            
            var confirmation = new OrderConfirmation
            {
                OrderId = finalizedOrder.OrderId,
                ConfirmationNumber = GenerateConfirmationNumber(),
                EstimatedDelivery = DateTime.UtcNow.AddDays(3),
                NotificationSent = true
            };
            
            return MlResult<OrderConfirmation>.Valid(confirmation);
        }
        catch (Exception ex)
        {
            // El pedido está procesado, pero la notificación falló
            var confirmation = new OrderConfirmation
            {
                OrderId = finalizedOrder.OrderId,
                ConfirmationNumber = GenerateConfirmationNumber(),
                EstimatedDelivery = DateTime.UtcNow.AddDays(3),
                NotificationSent = false,
                NotificationError = ex.Message
            };
            
            return MlResult<OrderConfirmation>.Valid(confirmation);
        }
    }
    
    private async Task ReleaseReservationsAsync(List<InventoryReservation> reservations)
    {
        foreach (var reservation in reservations)
        {
            try
            {
                await _inventoryService.ReleaseReservationAsync(reservation.ReservationId);
            }
            catch
            {
                // Log error, pero no fallar el proceso principal
            }
        }
    }
    
    private string GenerateConfirmationNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}

// Clases de apoyo y interfaces
public class OrderRequest
{
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
    public OrderRequest OriginalRequest { get; set; }
    public List<InventoryReservation> Reservations { get; set; }
    public DateTime ReservedAt { get; set; }
}

public class PaidOrder
{
    public ReservedOrder ReservedOrder { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public DateTime PaidAt { get; set; }
}

public class FinalizedOrder
{
    public Guid OrderId { get; set; }
    public PaidOrder PaidOrder { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime FinalizedAt { get; set; }
}

public class OrderConfirmation
{
    public Guid OrderId { get; set; }
    public string ConfirmationNumber { get; set; }
    public DateTime EstimatedDelivery { get; set; }
    public bool NotificationSent { get; set; }
    public string NotificationError { get; set; }
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

// Interfaces de servicios
public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

public interface IInventoryService
{
    Task<InventoryReservation> ReserveItemAsync(string productId, int quantity);
    Task ConfirmReservationAsync(string reservationId);
    Task ReleaseReservationAsync(string reservationId);
}

public interface INotificationService
{
    Task SendOrderConfirmationAsync(Guid orderId, int customerId);
}

public class PaymentRequest
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string PaymentMethodId { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public string ErrorMessage { get; set; }
}

public class InventoryReservation
{
    public string ReservationId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
```

### Ejemplo 3: TryBind para Manejo Seguro de Excepciones

```csharp
public class DataProcessingService
{
    public MlResult<ProcessedData> ProcessDataSafely(RawData rawData)
    {
        return ValidateRawData(rawData)
            .TryBind(validData => ParseJsonData(validData), "Failed to parse JSON data")
            .TryBind(parsedData => ValidateBusinessRules(parsedData), ex => $"Business validation failed: {ex.Message}")
            .TryBind(validatedData => TransformData(validatedData), "Data transformation error")
            .TryBind(transformedData => EnrichWithExternalData(transformedData), ex => 
                $"External data enrichment failed - {ex.GetType().Name}: {ex.Message}");
    }
    
    public async Task<MlResult<ProcessedData>> ProcessDataSafelyAsync(RawData rawData)
    {
        return await ValidateRawData(rawData)
            .TryBindAsync(async validData => await ParseJsonDataAsync(validData), "Failed to parse JSON data")
            .TryBindAsync(async parsedData => await ValidateBusinessRulesAsync(parsedData), 
                ex => $"Async business validation failed: {ex.Message}")
            .TryBindAsync(async validatedData => await TransformDataAsync(validatedData), "Async data transformation error")
            .TryBindAsync(async transformedData => await EnrichWithExternalDataAsync(transformedData), 
                ex => $"Async external data enrichment failed - {ex.GetType().Name}: {ex.Message}");
    }
    
    private MlResult<RawData> ValidateRawData(RawData rawData)
    {
        if (rawData == null)
            return MlResult<RawData>.Fail("Raw data cannot be null");
            
        if (string.IsNullOrWhiteSpace(rawData.JsonContent))
            return MlResult<RawData>.Fail("JSON content is required");
            
        if (rawData.Source == null)
            return MlResult<RawData>.Fail("Data source information is required");
            
        return MlResult<RawData>.Valid(rawData);
    }
    
    // Método que puede lanzar JsonException
    private MlResult<ParsedData> ParseJsonData(RawData rawData)
    {
        // Esta operación puede lanzar JsonException
        var jsonDocument = JsonDocument.Parse(rawData.JsonContent);
        
        var parsedData = new ParsedData
        {
            Document = jsonDocument,
            Source = rawData.Source,
            ParsedAt = DateTime.UtcNow
        };
        
        return MlResult<ParsedData>.Valid(parsedData);
    }
    
    // Método asíncrono que puede lanzar JsonException
    private async Task<MlResult<ParsedData>> ParseJsonDataAsync(RawData rawData)
    {
        await Task.Delay(50); // Simular operación asíncrona
        
        // Esta operación puede lanzar JsonException
        var jsonDocument = JsonDocument.Parse(rawData.JsonContent);
        
        var parsedData = new ParsedData
        {
            Document = jsonDocument,
            Source = rawData.Source,
            ParsedAt = DateTime.UtcNow
        };
        
        return MlResult<ParsedData>.Valid(parsedData);
    }
    
    // Método que puede lanzar BusinessRuleException
    private MlResult<ValidatedData> ValidateBusinessRules(ParsedData parsedData)
    {
        var root = parsedData.Document.RootElement;
        
        // Estas validaciones pueden lanzar excepciones
        if (!root.TryGetProperty("id", out var idProperty))
            throw new BusinessRuleException("Missing required 'id' property");
            
        if (!root.TryGetProperty("timestamp", out var timestampProperty))
            throw new BusinessRuleException("Missing required 'timestamp' property");
            
        if (!DateTime.TryParse(timestampProperty.GetString(), out var timestamp))
            throw new BusinessRuleException("Invalid timestamp format");
            
        if (timestamp > DateTime.UtcNow.AddHours(1))
            throw new BusinessRuleException("Timestamp cannot be in the future");
            
        var validatedData = new ValidatedData
        {
            Id = idProperty.GetString(),
            Timestamp = timestamp,
            ParsedData = parsedData,
            ValidatedAt = DateTime.UtcNow
        };
        
        return MlResult<ValidatedData>.Valid(validatedData);
    }
    
    private async Task<MlResult<ValidatedData>> ValidateBusinessRulesAsync(ParsedData parsedData)
    {
        await Task.Delay(30); // Simular validación asíncrona
        return ValidateBusinessRules(parsedData);
    }
    
    // Método que puede lanzar TransformationException
    private MlResult<TransformedData> TransformData(ValidatedData validatedData)
    {
        // Transformación que puede fallar
        if (validatedData.Id.Length < 5)
            throw new TransformationException("ID must be at least 5 characters long");
            
        var transformedData = new TransformedData
        {
            TransformedId = $"PROC_{validatedData.Id.ToUpper()}",
            ProcessedTimestamp = validatedData.Timestamp,
            ValidatedData = validatedData,
            TransformedAt = DateTime.UtcNow
        };
        
        return MlResult<TransformedData>.Valid(transformedData);
    }
    
    private async Task<MlResult<TransformedData>> TransformDataAsync(ValidatedData validatedData)
    {
        await Task.Delay(40); // Simular transformación asíncrona
        return TransformData(validatedData);
    }
    
    // Método que puede lanzar ExternalServiceException
    private MlResult<ProcessedData> EnrichWithExternalData(TransformedData transformedData)
    {
        // Simulación de llamada a servicio externo que puede fallar
        if (transformedData.TransformedId.Contains("ERROR"))
            throw new ExternalServiceException("External service rejected the data");
            
        var enrichedData = new ProcessedData
        {
            FinalId = transformedData.TransformedId,
            ProcessedTimestamp = transformedData.ProcessedTimestamp,
            ExternalData = "Enriched from external service",
            TransformedData = transformedData,
            ProcessedAt = DateTime.UtcNow
        };
        
        return MlResult<ProcessedData>.Valid(enrichedData);
    }
    
    private async Task<MlResult<ProcessedData>> EnrichWithExternalDataAsync(TransformedData transformedData)
    {
        await Task.Delay(100); // Simular llamada a servicio externo
        return EnrichWithExternalData(transformedData);
    }
}

// Clases de apoyo y excepciones personalizadas
public class RawData
{
    public string JsonContent { get; set; }
    public DataSource Source { get; set; }
}

public class DataSource
{
    public string Name { get; set; }
    public string Version { get; set; }
}

public class ParsedData
{
    public JsonDocument Document { get; set; }
    public DataSource Source { get; set; }
    public DateTime ParsedAt { get; set; }
}

public class ValidatedData
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public ParsedData ParsedData { get; set; }
    public DateTime ValidatedAt { get; set; }
}

public class TransformedData
{
    public string TransformedId { get; set; }
    public DateTime ProcessedTimestamp { get; set; }
    public ValidatedData ValidatedData { get; set; }
    public DateTime TransformedAt { get; set; }
}

public class ProcessedData
{
    public string FinalId { get; set; }
    public DateTime ProcessedTimestamp { get; set; }
    public string ExternalData { get; set; }
    public TransformedData TransformedData { get; set; }
    public DateTime ProcessedAt { get; set; }
}

// Excepciones personalizadas
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

public class TransformationException : Exception
{
    public TransformationException(string message) : base(message) { }
}

public class ExternalServiceException : Exception
{
    public ExternalServiceException(string message) : base(message) { }
}
```

---

## Mejores Prácticas

### 1. Elección entre Bind y TryBind

```csharp
// ✅ Correcto: Usar Bind cuando las funciones devuelven MlResult y no lanzan excepciones
var result = GetUser(id)
    .Bind(user => ValidateUser(user))  // ValidateUser devuelve MlResult
    .Bind(validUser => SaveUser(validUser));

// ✅ Correcto: Usar TryBind cuando las funciones pueden lanzar excepciones
var result = GetRawData()
    .TryBind(data => ParseJson(data), "JSON parsing failed")  // ParseJson puede lanzar JsonException
    .TryBind(parsed => ProcessData(parsed), ex => $"Processing failed: {ex.Message}");

// ❌ Incorrecto: Usar Bind con funciones que pueden lanzar excepciones sin control
var result = GetData()
    .Bind(data => ParseJsonUnsafe(data));  // Puede lanzar excepción no controlada
```

### 2. Manejo de Mensajes de Error

```csharp
// ✅ Correcto: Mensajes descriptivos con contexto
var result = source.TryBind(
    data => ProcessComplexData(data),
    ex => $"Failed to process data for user {userId} at step {currentStep}: {ex.Message}"
);

// ✅ Correcto: Usar función constructora para mensajes dinámicos
var result = source.TryBind(
    data => CallExternalService(data),
    ex => ex switch
    {
        TimeoutException => "External service timed out",
        HttpRequestException => $"HTTP error: {ex.Message}",
        _ => $"Unexpected error: {ex.GetType().Name}"
    }
);

// ❌ Incorrecto: Mensajes genéricos o sin contexto
var result = source.TryBind(data => ProcessData(data), "Error occurred");
```

### 3. Composición de Pipelines

```csharp
// ✅ Correcto: Pipeline fluido y legible
var result = await GetUserInput()
    .BindAsync(async input => await ValidateInputAsync(input))
    .TryBindAsync(async validInput => await ProcessInputAsync(validInput), "Processing failed")
    .BindAsync(async processed => await SaveResultAsync(processed))
    .TryBindAsync(async saved => await NotifyCompletionAsync(saved), "Notification failed");

// ✅ Correcto: Separar pipelines largos en métodos
public async Task<MlResult<FinalResult>> ProcessCompleteWorkflowAsync(InitialData data)
{
    var validatedData = await ValidateAndPrepareDataAsync(data);
    var processedData = await ProcessValidatedDataAsync(validatedData);
    var finalizedData = await FinalizeAndNotifyAsync(processedData);
    
    return finalizedData;
}

// ❌ Incorrecto: Pipeline excesivamente largo y difícil de leer
var result = data.Bind(step1).Bind(step2).Bind(step3)...Bind(step20);
```

### 4. Manejo de Operaciones Asíncronas

```csharp
// ✅ Correcto: Usar BindAsync apropiadamente
var result = await GetDataAsync()  // Task<MlResult<Data>>
    .BindAsync(async data => await ProcessDataAsync(data))  // Función asíncrona
    .BindAsync(processed => ValidateData(processed));  // Función síncrona

// ✅ Correcto: Combinar operaciones síncronas y asíncronas
var result = ValidateInput(input)  // MlResult<Input>
    .BindAsync(async validInput => await FetchExternalDataAsync(validInput))
    .Bind(externalData => ProcessLocally(externalData))
    .BindAsync(async localResult => await SaveAsync(localResult));

// ❌ Incorrecto: No usar await correctamente
var result = GetDataAsync()  // Task<MlResult<Data>>
    .Bind(data => ProcessData(data));  // ¡Error! Necesita BindAsync
```

### 5. Gestión de Recursos y Cleanup

```csharp
// ✅ Correcto: Usar TryBind para operaciones que requieren cleanup
public MlResult<ProcessedFile> ProcessFileWithCleanup(string filePath)
{
    return ValidateFilePath(filePath)
        .TryBind(validPath => 
        {
            using var fileStream = File.OpenRead(validPath);
            return ProcessFileStream(fileStream);
        }, "File processing failed");
}

// ✅ Correcto: Manejo explícito de recursos en operaciones asíncronas
public async Task<MlResult<ProcessedData>> ProcessWithResourcesAsync(string connectionString)
{
    return await ValidateConnectionString(connectionString)
        .TryBindAsync(async connStr =>
        {
            using var connection = new SqlConnection(connStr);
            await connection.OpenAsync();
            return await ProcessDatabaseAsync(connection);
        }, ex => $"Database processing failed: {ex.Message}");
}
```

---

## Consideraciones de Rendimiento

### Propagación de Errores

- Los métodos `Bind` implementan cortocircuito: si un paso falla, los siguientes no se ejecutan
- Costo computacional mínimo para propagar errores
- Los errores se acumulan eficientemente sin duplicación

### Captura de Excepciones

- `TryBind` tiene overhead adicional por el try-catch
- Usar solo cuando sea necesario manejar excepciones
- El costo de crear `MlResult` desde excepción es mínimo

### Operaciones Asíncronas

- Las versiones `Async` mantienen el contexto de sincronización apropiadamente
- `ConfigureAwait(false)` se usa internamente para librerías
- No hay boxing innecesario de valores o tasks

---

## Resumen

La clase `MlResultActionsBind` implementa las operaciones fundamentales de composición monádica:

- **`Bind`**: Composición básica para funciones que devuelven `MlResult<T>`
- **`BindAsync`**: Soporte completo para operaciones asíncronas en todas las combinaciones
- **`TryBind`**: Versiones seguras que capturan excepciones y las convierten en errores
- **`TryBindAsync`**: Versiones asíncronas de las operaciones seguras

Estas operaciones forman el núcleo de la programación funcional con `MlResult<T>`, permitiendo crear pipelines robustos donde los errores se propagan automáticamente y las excepciones se manejan de forma segura y predecible.

La flexibilidad en el manejo de mensajes de error y el soporte completo para operaciones asíncronas hacen que estas operaciones sean adecuadas para una amplia variedad de escenarios, desde validaciones simples hasta pipelines complejos de procesamiento de datos.
