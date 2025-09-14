# MlResultActionsSpecializedBind - Operaciones de Binding Especializadas

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Clase](#análisis-de-la-clase)
3. [Métodos BindSaveValueInDetailsIfFaildFuncResult](#métodos-bindsavevalueindetailsiffaildFuncresult)
4. [Métodos ExecSelf](#métodos-execself)
5. [Métodos ExecSelfIfValid](#métodos-execselfifvalid)
6. [Variantes Asíncronas](#variantes-asíncronas)
7. [Ejemplos Prácticos](#ejemplos-prácticos)
8. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La clase `MlResultActionsSpecializedBind` contiene operaciones especializadas de **binding** que cubren casos de uso específicos no cubiertos por las operaciones básicas. Estas operaciones proporcionan funcionalidades únicas como preservar valores en detalles de error, ejecutar acciones sin cambiar el resultado, y manejar casos condicionales específicos.

### Propósito Principal

- **Preservación de Contexto**: Guardar valores de entrada en detalles de error cuando las operaciones fallan
- **Ejecución de Efectos Secundarios**: Ejecutar acciones sin modificar el resultado original
- **Logging y Auditoría Avanzada**: Operaciones especializadas para trazabilidad
- **Debugging y Diagnóstico**: Mantener información de contexto para depuración

---

## Análisis de la Clase

### Estructura y Filosofía

Esta clase implementa tres patrones especializados:

1. **Binding con Preservación de Contexto**: `BindSaveValueInDetailsIfFaildFuncResult`
2. **Ejecución Condicional sin Modificación**: `ExecSelf`
3. **Ejecución Solo en Caso Válido**: `ExecSelfIfValid`

### Características Principales

1. **Preservación de Información**: Los valores se mantienen en los detalles de error
2. **Inmutabilidad del Resultado**: Los métodos `ExecSelf` no modifican el resultado original
3. **Flexibilidad de Ejecución**: Soporte para acciones síncronas y asíncronas
4. **Manejo Seguro de Excepciones**: Versiones `Try*` para todas las operaciones

---

## Métodos BindSaveValueInDetailsIfFaildFuncResult

### `BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>()`

**Propósito**: Ejecuta una función de binding y si la función falla, guarda el valor de entrada en los detalles del error

```csharp
public static MlResult<TReturn> BindSaveValueInDetailsIfFaildFuncResult<T, TReturn>(
    this MlResult<T> source, 
    Func<T, MlResult<TReturn>> func)
```

**Parámetros**:
- `source`: El resultado fuente a evaluar
- `func`: Función que procesa el valor si `source` es válido

**Comportamiento**:
- Si `source` es fallido: Propaga el error sin ejecutar `func`
- Si `source` es válido y `func` es exitosa: Retorna el resultado de `func`
- Si `source` es válido pero `func` falla: Retorna el error de `func` con el valor original añadido a los detalles

**Ejemplo Básico**:
```csharp
var userInput = MlResult<string>.Valid("invalid-email-format");

var result = userInput.BindSaveValueInDetailsIfFaildFuncResult(email => 
{
    if (!IsValidEmail(email))
        return MlResult<User>.Fail("Invalid email format");
    
    return MlResult<User>.Valid(new User { Email = email });
});

// Si falla, el error contendrá:
// - Mensaje: "Invalid email format"
// - Detalles: incluirá "invalid-email-format" como contexto adicional
```

### Versiones Asíncronas

#### `BindSaveValueInDetailsIfFaildFuncResultAsync` - Función Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this MlResult<T> source, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)
```

#### `BindSaveValueInDetailsIfFaildFuncResultAsync` - Fuente Asíncrona
```csharp
public static async Task<MlResult<TReturn>> BindSaveValueInDetailsIfFaildFuncResultAsync<T, TReturn>(
    this Task<MlResult<T>> sourceAsync, 
    Func<T, Task<MlResult<TReturn>>> funcAsync)
```

### Versiones TryBind

#### `TryBindSaveValueInDetailsIfFaildFuncResult` - Captura de Excepciones
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

**Comportamiento**: Versión segura que captura excepciones y las convierte en errores, preservando también el valor original

---

## Métodos ExecSelf

### `ExecSelf<T>()`

**Propósito**: Ejecuta acciones diferentes según el estado del resultado pero siempre retorna el resultado original sin modificaciones

```csharp
public static MlResult<T> ExecSelf<T>(this MlResult<T> source, 
                                      Action<T> actionValid,
                                      Action<MlErrorsDetails> actionFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionValid`: Acción a ejecutar si `source` es válido (recibe el valor)
- `actionFail`: Acción a ejecutar si `source` es fallido (recibe los errores)

**Comportamiento**:
- Si `source` es válido: Ejecuta `actionValid(value)` y retorna `source` sin cambios
- Si `source` es fallido: Ejecuta `actionFail(errorDetails)` y retorna `source` sin cambios
- El resultado original nunca se modifica

**Ejemplo Básico**:
```csharp
var result = GetUser(userId)
    .ExecSelf(
        user => _logger.LogInformation($"User {user.Id} retrieved successfully"),
        errors => _logger.LogError($"Failed to get user: {errors.FirstErrorMessage}")
    );

// 'result' mantiene exactamente el mismo valor que retornó GetUser(userId)
// pero se han ejecutado las acciones de logging apropiadas
```

### Versiones Asíncronas de ExecSelf

#### Todas las Combinaciones
```csharp
// Ambas acciones asíncronas
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this MlResult<T> source,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Fuente asíncrona con acciones asíncronas
public static async Task<MlResult<T>> ExecSelfAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync,
    Func<MlErrorsDetails, Task> actionFailAsync)

// Combinaciones mixtas de acciones síncronas/asíncronas
// ... (múltiples variantes)
```

### Versiones TryExecSelf

#### `TryExecSelf<T>()` - Ejecución Segura
```csharp
public static MlResult<T> TryExecSelf<T>(this MlResult<T> source,
                                         Action<T> actionValid,
                                         Action<MlErrorsDetails> actionFail,
                                         Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**: 
- Ejecuta las acciones de forma segura
- Si alguna acción lanza una excepción, captura la excepción y retorna un resultado fallido
- Utiliza el constructor de mensaje para crear errores descriptivos

**Ejemplo con Manejo de Excepciones**:
```csharp
var result = ProcessOrder(order)
    .TryExecSelf(
        successOrder => SendNotification(successOrder), // Puede lanzar excepción
        errors => LogOrderFailure(errors),              // Puede lanzar excepción
        ex => $"Notification or logging failed: {ex.Message}"
    );

// Si alguna acción falla, result será un MlResult fallido con el mensaje de excepción
```

---

## Métodos ExecSelfIfValid

### `ExecSelfIfValid<T>()`

**Propósito**: Ejecuta una acción solo si el resultado es válido, sin modificar el resultado original

```csharp
public static MlResult<T> ExecSelfIfValid<T>(this MlResult<T> source,
                                             Action<T> actionValid)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `actionValid`: Acción a ejecutar solo si `source` es válido

**Comportamiento**:
- Si `source` es válido: Ejecuta `actionValid(value)` y retorna `source` sin cambios
- Si `source` es fallido: No ejecuta ninguna acción y retorna `source` sin cambios

**Ejemplo Básico**:
```csharp
var result = CreateUser(userData)
    .ExecSelfIfValid(user => _cache.Set($"user:{user.Id}", user))
    .ExecSelfIfValid(user => _metrics.IncrementCounter("users.created"));

// La cache y métricas solo se actualizan si la creación fue exitosa
// El resultado original de CreateUser se mantiene intacto
```

### Versiones Asíncronas de ExecSelfIfValid

#### `ExecSelfIfValidAsync` - Acción Asíncrona
```csharp
public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(
    this MlResult<T> source,
    Func<T, Task> actionValidAsync)

public static async Task<MlResult<T>> ExecSelfIfValidAsync<T>(
    this Task<MlResult<T>> sourceAsync,
    Func<T, Task> actionValidAsync)
```

### Versiones TryExecSelfIfValid

#### `TryExecSelfIfValid<T>()` - Ejecución Segura Solo en Caso Válido
```csharp
public static MlResult<T> TryExecSelfIfValid<T>(this MlResult<T> source,
                                                Action<T> actionValid,
                                                Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Solo ejecuta la acción si `source` es válido
- Si la acción lanza una excepción, captura la excepción y retorna un resultado fallido
- Si `source` ya es fallido, lo retorna sin cambios

---

## Variantes Asíncronas

### Matriz Completa de Combinaciones

| Tipo de Operación | Fuente | Función/Acción | Método |
|-------------------|--------|----------------|---------|
| **BindSaveValue** | `MlResult<T>` | `T → MlResult<U>` | `BindSaveValueInDetailsIfFaildFuncResult` |
| **BindSaveValue** | `MlResult<T>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| **BindSaveValue** | `Task<MlResult<T>>` | `T → Task<MlResult<U>>` | `BindSaveValueInDetailsIfFaildFuncResultAsync` |
| **ExecSelf** | `MlResult<T>` | `Action<T>`, `Action<Errors>` | `ExecSelf` |
| **ExecSelf** | `MlResult<T>` | `Func<T,Task>`, `Func<Errors,Task>` | `ExecSelfAsync` |
| **ExecSelfIfValid** | `MlResult<T>` | `Action<T>` | `ExecSelfIfValid` |
| **ExecSelfIfValid** | `MlResult<T>` | `Func<T,Task>` | `ExecSelfIfValidAsync` |

Todas las variantes tienen sus correspondientes versiones `Try*` para manejo seguro de excepciones.

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Validación con Preservación de Contexto

```csharp
public class UserValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;
    
    public UserValidationService(IUserRepository userRepository, ILogger logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<MlResult<ValidatedUser>> ValidateUserWithContextAsync(UserRegistrationData userData)
    {
        return await ValidateBasicData(userData)
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validData => 
                await ValidateEmailUniquenessAsync(validData))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async emailValidated => 
                await ValidatePasswordStrengthAsync(emailValidated))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async passwordValidated => 
                await ValidateBusinessRulesAsync(passwordValidated))
            .ExecSelfAsync(
                async validUser => 
                {
                    await _logger.LogInformationAsync($"User validation successful for {validUser.Email}");
                    await RecordSuccessMetricsAsync(validUser);
                },
                async errors => 
                {
                    await _logger.LogWarningAsync($"User validation failed: {errors.FirstErrorMessage}");
                    await RecordFailureMetricsAsync(errors);
                }
            );
    }
    
    private MlResult<UserRegistrationData> ValidateBasicData(UserRegistrationData userData)
    {
        if (userData == null)
            return MlResult<UserRegistrationData>.Fail("User data cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(userData.Email))
            errors.Add("Email is required");
        else if (!IsValidEmailFormat(userData.Email))
            errors.Add("Email format is invalid");
            
        if (string.IsNullOrWhiteSpace(userData.Password))
            errors.Add("Password is required");
            
        if (string.IsNullOrWhiteSpace(userData.FirstName))
            errors.Add("First name is required");
            
        if (string.IsNullOrWhiteSpace(userData.LastName))
            errors.Add("Last name is required");
            
        if (errors.Any())
            return MlResult<UserRegistrationData>.Fail(errors.ToArray());
            
        return MlResult<UserRegistrationData>.Valid(userData);
    }
    
    private async Task<MlResult<UserRegistrationData>> ValidateEmailUniquenessAsync(UserRegistrationData userData)
    {
        try
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userData.Email);
            
            if (existingUser != null)
            {
                return MlResult<UserRegistrationData>.Fail(
                    $"Email '{userData.Email}' is already registered");
            }
            
            return MlResult<UserRegistrationData>.Valid(userData);
        }
        catch (Exception ex)
        {
            return MlResult<UserRegistrationData>.Fail(
                $"Failed to validate email uniqueness: {ex.Message}");
        }
    }
    
    private async Task<MlResult<UserRegistrationData>> ValidatePasswordStrengthAsync(UserRegistrationData userData)
    {
        await Task.Delay(50); // Simular validación asíncrona
        
        var password = userData.Password;
        var errors = new List<string>();
        
        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters long");
            
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter");
            
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter");
            
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit");
            
        if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)))
            errors.Add("Password must contain at least one special character");
            
        if (errors.Any())
            return MlResult<UserRegistrationData>.Fail(errors.ToArray());
            
        return MlResult<UserRegistrationData>.Valid(userData);
    }
    
    private async Task<MlResult<ValidatedUser>> ValidateBusinessRulesAsync(UserRegistrationData userData)
    {
        await Task.Delay(30); // Simular validación de reglas de negocio
        
        // Ejemplo: validar que el dominio del email esté en la lista permitida
        var allowedDomains = new[] { "company.com", "partner.com", "client.com" };
        var emailDomain = userData.Email.Split('@')[1].ToLower();
        
        if (!allowedDomains.Contains(emailDomain))
        {
            return MlResult<ValidatedUser>.Fail(
                $"Email domain '{emailDomain}' is not allowed. Allowed domains: {string.Join(", ", allowedDomains)}");
        }
        
        // Ejemplo: validar edad mínima
        if (userData.BirthDate.HasValue)
        {
            var age = DateTime.Today.Year - userData.BirthDate.Value.Year;
            if (userData.BirthDate.Value > DateTime.Today.AddYears(-age)) age--;
            
            if (age < 18)
            {
                return MlResult<ValidatedUser>.Fail("User must be at least 18 years old");
            }
        }
        
        var validatedUser = new ValidatedUser
        {
            Email = userData.Email,
            FirstName = userData.FirstName,
            LastName = userData.LastName,
            BirthDate = userData.BirthDate,
            PasswordHash = ComputePasswordHash(userData.Password),
            ValidationTimestamp = DateTime.UtcNow
        };
        
        return MlResult<ValidatedUser>.Valid(validatedUser);
    }
    
    private async Task RecordSuccessMetricsAsync(ValidatedUser user)
    {
        try
        {
            var metrics = new ValidationMetrics
            {
                UserId = user.Email,
                Success = true,
                ValidationTime = DateTime.UtcNow,
                Steps = new[] { "BasicData", "EmailUniqueness", "PasswordStrength", "BusinessRules" }
            };
            
            await _metricsService.RecordAsync(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to record success metrics: {ex.Message}");
        }
    }
    
    private async Task RecordFailureMetricsAsync(MlErrorsDetails errors)
    {
        try
        {
            var metrics = new ValidationMetrics
            {
                UserId = "unknown",
                Success = false,
                ValidationTime = DateTime.UtcNow,
                ErrorDetails = errors.AllErrors,
                ContextData = errors.Details // Aquí tendremos los valores preservados
            };
            
            await _metricsService.RecordAsync(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to record failure metrics: {ex.Message}");
        }
    }
    
    private bool IsValidEmailFormat(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    private string ComputePasswordHash(string password)
    {
        // Implementación simplificada - usar BCrypt o similar en producción
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "salt"));
        return Convert.ToBase64String(hashedBytes);
    }
}

// Clases de apoyo
public class UserRegistrationData
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
}

public class ValidatedUser
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public string PasswordHash { get; set; }
    public DateTime ValidationTimestamp { get; set; }
}

public class ValidationMetrics
{
    public string UserId { get; set; }
    public bool Success { get; set; }
    public DateTime ValidationTime { get; set; }
    public string[] Steps { get; set; }
    public string[] ErrorDetails { get; set; }
    public object ContextData { get; set; }
}

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email);
}

public interface IMetricsService
{
    Task RecordAsync(ValidationMetrics metrics);
}
```

### Ejemplo 2: Pipeline de Procesamiento con Efectos Secundarios

```csharp
public class OrderProcessingPipeline
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;
    private readonly ILogger _logger;
    private readonly IAuditService _auditService;
    
    public OrderProcessingPipeline(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IInventoryService inventoryService,
        INotificationService notificationService,
        ILogger logger,
        IAuditService auditService)
    {
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _notificationService = notificationService;
        _logger = logger;
        _auditService = auditService;
    }
    
    public async Task<MlResult<ProcessedOrder>> ProcessOrderAsync(OrderRequest orderRequest)
    {
        var correlationId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        
        return await ValidateOrderRequest(orderRequest)
            .ExecSelfIfValidAsync(async validRequest => 
                await _auditService.LogOrderStartAsync(correlationId, validRequest))
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validRequest => 
                await ReserveInventoryAsync(validRequest))
            .ExecSelfAsync(
                async reservedOrder => 
                {
                    await _logger.LogInformationAsync($"Inventory reserved for order {correlationId}");
                    await UpdateOrderStatusAsync(reservedOrder.OrderId, OrderStatus.InventoryReserved);
                },
                async errors => 
                {
                    await _logger.LogWarningAsync($"Inventory reservation failed for order {correlationId}");
                    await _auditService.LogInventoryFailureAsync(correlationId, errors);
                }
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async reservedOrder => 
                await ProcessPaymentAsync(reservedOrder))
            .ExecSelfAsync(
                async paidOrder => 
                {
                    await _logger.LogInformationAsync($"Payment processed for order {correlationId}");
                    await UpdateOrderStatusAsync(paidOrder.OrderId, OrderStatus.PaymentProcessed);
                },
                async errors => 
                {
                    await _logger.LogErrorAsync($"Payment failed for order {correlationId}");
                    await _auditService.LogPaymentFailureAsync(correlationId, errors);
                    await ReleaseInventoryAsync(correlationId); // Cleanup en caso de fallo
                }
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async paidOrder => 
                await FinalizeOrderAsync(paidOrder))
            .ExecSelfIfValidAsync(async finalizedOrder => 
                await SendOrderConfirmationAsync(finalizedOrder))
            .TryExecSelfAsync(
                async successOrder => 
                {
                    await _auditService.LogOrderCompletedAsync(correlationId, successOrder, DateTime.UtcNow - startTime);
                    await _logger.LogInformationAsync($"Order {correlationId} completed successfully");
                },
                async errors => 
                {
                    await _auditService.LogOrderFailedAsync(correlationId, errors, DateTime.UtcNow - startTime);
                    await _logger.LogErrorAsync($"Order {correlationId} failed: {errors.FirstErrorMessage}");
                },
                ex => $"Failed to log order completion: {ex.Message}"
            );
    }
    
    private MlResult<OrderRequest> ValidateOrderRequest(OrderRequest request)
    {
        if (request == null)
            return MlResult<OrderRequest>.Fail("Order request cannot be null");
            
        var errors = new List<string>();
        
        if (request.CustomerId <= 0)
            errors.Add("Valid customer ID is required");
            
        if (request.Items?.Any() != true)
            errors.Add("Order must contain at least one item");
        else
        {
            foreach (var item in request.Items.Select((item, index) => new { item, index }))
            {
                if (string.IsNullOrWhiteSpace(item.item.ProductId))
                    errors.Add($"Product ID is required for item {item.index + 1}");
                    
                if (item.item.Quantity <= 0)
                    errors.Add($"Quantity must be positive for item {item.index + 1}");
                    
                if (item.item.Price <= 0)
                    errors.Add($"Price must be positive for item {item.index + 1}");
            }
        }
        
        if (string.IsNullOrWhiteSpace(request.PaymentMethodId))
            errors.Add("Payment method is required");
            
        if (errors.Any())
            return MlResult<OrderRequest>.Fail(errors.ToArray());
            
        return MlResult<OrderRequest>.Valid(request);
    }
    
    private async Task<MlResult<ReservedOrder>> ReserveInventoryAsync(OrderRequest orderRequest)
    {
        try
        {
            var reservations = new List<InventoryReservation>();
            
            foreach (var item in orderRequest.Items)
            {
                var reservation = await _inventoryService.ReserveAsync(item.ProductId, item.Quantity);
                if (!reservation.Success)
                {
                    // Liberar reservas anteriores
                    foreach (var prevReservation in reservations)
                    {
                        await _inventoryService.ReleaseAsync(prevReservation.ReservationId);
                    }
                    
                    return MlResult<ReservedOrder>.Fail(
                        $"Failed to reserve inventory for product {item.ProductId}: {reservation.ErrorMessage}");
                }
                reservations.Add(reservation);
            }
            
            var reservedOrder = new ReservedOrder
            {
                OrderId = Guid.NewGuid(),
                OriginalRequest = orderRequest,
                Reservations = reservations,
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
            
            var paymentRequest = new PaymentRequest
            {
                OrderId = reservedOrder.OrderId,
                CustomerId = reservedOrder.OriginalRequest.CustomerId,
                Amount = totalAmount,
                PaymentMethodId = reservedOrder.OriginalRequest.PaymentMethodId
            };
            
            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);
            
            if (!paymentResult.Success)
            {
                return MlResult<PaidOrder>.Fail($"Payment processing failed: {paymentResult.ErrorMessage}");
            }
            
            var paidOrder = new PaidOrder
            {
                OrderId = reservedOrder.OrderId,
                ReservedOrder = reservedOrder,
                PaymentResult = paymentResult,
                PaidAt = DateTime.UtcNow
            };
            
            return MlResult<PaidOrder>.Valid(paidOrder);
        }
        catch (Exception ex)
        {
            return MlResult<PaidOrder>.Fail($"Payment processing error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<ProcessedOrder>> FinalizeOrderAsync(PaidOrder paidOrder)
    {
        try
        {
            // Confirmar las reservas de inventario
            foreach (var reservation in paidOrder.ReservedOrder.Reservations)
            {
                await _inventoryService.ConfirmReservationAsync(reservation.ReservationId);
            }
            
            // Crear el pedido en la base de datos
            var order = new ProcessedOrder
            {
                OrderId = paidOrder.OrderId,
                CustomerId = paidOrder.ReservedOrder.OriginalRequest.CustomerId,
                Items = paidOrder.ReservedOrder.OriginalRequest.Items,
                TotalAmount = paidOrder.PaymentResult.Amount,
                Status = OrderStatus.Completed,
                PaymentId = paidOrder.PaymentResult.TransactionId,
                ProcessedAt = DateTime.UtcNow
            };
            
            await _orderRepository.SaveOrderAsync(order);
            
            return MlResult<ProcessedOrder>.Valid(order);
        }
        catch (Exception ex)
        {
            return MlResult<ProcessedOrder>.Fail($"Order finalization failed: {ex.Message}");
        }
    }
    
    private async Task SendOrderConfirmationAsync(ProcessedOrder order)
    {
        try
        {
            await _notificationService.SendOrderConfirmationAsync(order.CustomerId, order.OrderId);
        }
        catch (Exception ex)
        {
            // No fallar el proceso por problemas de notificación
            await _logger.LogWarningAsync($"Failed to send order confirmation: {ex.Message}");
        }
    }
    
    private async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        try
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to update order status: {ex.Message}");
        }
    }
    
    private async Task ReleaseInventoryAsync(string correlationId)
    {
        try
        {
            await _inventoryService.ReleaseByCorrelationIdAsync(correlationId);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync($"Failed to release inventory for correlation {correlationId}: {ex.Message}");
        }
    }
}

// Clases de apoyo y enums
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
    public Guid OrderId { get; set; }
    public OrderRequest OriginalRequest { get; set; }
    public List<InventoryReservation> Reservations { get; set; }
    public DateTime ReservedAt { get; set; }
}

public class PaidOrder
{
    public Guid OrderId { get; set; }
    public ReservedOrder ReservedOrder { get; set; }
    public PaymentResult PaymentResult { get; set; }
    public DateTime PaidAt { get; set; }
}

public class ProcessedOrder
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentId { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public enum OrderStatus
{
    Created,
    InventoryReserved,
    PaymentProcessed,
    Completed,
    Failed
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
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethodId { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string ErrorMessage { get; set; }
}

// Interfaces de servicios
public interface IOrderRepository
{
    Task SaveOrderAsync(ProcessedOrder order);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
}

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}

public interface IInventoryService
{
    Task<InventoryReservation> ReserveAsync(string productId, int quantity);
    Task ReleaseAsync(string reservationId);
    Task ConfirmReservationAsync(string reservationId);
    Task ReleaseByCorrelationIdAsync(string correlationId);
}

public interface INotificationService
{
    Task SendOrderConfirmationAsync(int customerId, Guid orderId);
}

public interface IAuditService
{
    Task LogOrderStartAsync(string correlationId, OrderRequest request);
    Task LogInventoryFailureAsync(string correlationId, MlErrorsDetails errors);
    Task LogPaymentFailureAsync(string correlationId, MlErrorsDetails errors);
    Task LogOrderCompletedAsync(string correlationId, ProcessedOrder order, TimeSpan duration);
    Task LogOrderFailedAsync(string correlationId, MlErrorsDetails errors, TimeSpan duration);
}
```

### Ejemplo 3: Sistema de Cache con Efectos Secundarios Condicionados

```csharp
public class CachedDataService
{
    private readonly IDataRepository _repository;
    private readonly ICacheService _cache;
    private readonly IMetricsService _metrics;
    private readonly ILogger _logger;
    
    public CachedDataService(
        IDataRepository repository,
        ICacheService cache,
        IMetricsService metrics,
        ILogger logger)
    {
        _repository = repository;
        _cache = cache;
        _metrics = metrics;
        _logger = logger;
    }
    
    public async Task<MlResult<UserProfile>> GetUserProfileAsync(int userId)
    {
        var cacheKey = $"user_profile:{userId}";
        var startTime = DateTime.UtcNow;
        
        return await TryGetFromCacheAsync(cacheKey)
            .ExecSelfAsync(
                async cachedProfile => 
                {
                    await _metrics.IncrementCounterAsync("cache.hit");
                    await _logger.LogDebugAsync($"Cache hit for user {userId}");
                },
                async _ => 
                {
                    await _metrics.IncrementCounterAsync("cache.miss");
                    await _logger.LogDebugAsync($"Cache miss for user {userId}");
                }
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async cacheResult => 
                cacheResult.IsValid 
                    ? cacheResult 
                    : await LoadFromRepositoryAsync(userId))
            .ExecSelfIfValidAsync(async profile => 
                await UpdateCacheAsync(cacheKey, profile))
            .TryExecSelfAsync(
                async profile => 
                {
                    var duration = DateTime.UtcNow - startTime;
                    await _metrics.RecordTimingAsync("user_profile.load_time", duration);
                    await _logger.LogInformationAsync($"User profile {userId} loaded in {duration.TotalMilliseconds}ms");
                },
                async errors => 
                {
                    var duration = DateTime.UtcNow - startTime;
                    await _metrics.RecordTimingAsync("user_profile.error_time", duration);
                    await _logger.LogWarningAsync($"Failed to load user profile {userId} after {duration.TotalMilliseconds}ms: {errors.FirstErrorMessage}");
                },
                ex => $"Failed to record metrics: {ex.Message}"
            );
    }
    
    public async Task<MlResult<SearchResults<T>>> SearchWithCacheAsync<T>(SearchCriteria criteria) where T : class
    {
        var cacheKey = GenerateSearchCacheKey(criteria);
        var requestId = Guid.NewGuid().ToString();
        
        return await ValidateSearchCriteria(criteria)
            .ExecSelfAsync(
                async validCriteria => 
                {
                    await _logger.LogInformationAsync($"Search request {requestId} validated: {validCriteria}");
                    await _metrics.IncrementCounterAsync("search.validated");
                },
                async errors => 
                {
                    await _logger.LogWarningAsync($"Search request {requestId} validation failed: {errors.FirstErrorMessage}");
                    await _metrics.IncrementCounterAsync("search.validation_failed");
                }
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async validCriteria => 
                await TryGetSearchFromCacheAsync<T>(cacheKey))
            .ExecSelfAsync(
                async cachedResults => 
                {
                    await _metrics.IncrementCounterAsync("search.cache_hit");
                    await _logger.LogDebugAsync($"Search cache hit for request {requestId}");
                },
                async _ => 
                {
                    await _metrics.IncrementCounterAsync("search.cache_miss");
                    await _logger.LogDebugAsync($"Search cache miss for request {requestId}");
                }
            )
            .BindSaveValueInDetailsIfFaildFuncResultAsync(async cacheResult => 
                cacheResult.IsValid 
                    ? cacheResult 
                    : await PerformSearchAsync<T>(criteria))
            .ExecSelfIfValidAsync(async searchResults => 
                await CacheSearchResultsAsync(cacheKey, searchResults))
            .ExecSelfIfValidAsync(async results => 
                await RecordSearchSuccessAsync(requestId, criteria, results.TotalResults))
            .TryExecSelfAsync(
                async results => await _logger.LogInformationAsync($"Search {requestId} completed with {results.TotalResults} results"),
                async errors => await _logger.LogErrorAsync($"Search {requestId} failed: {errors.FirstErrorMessage}"),
                ex => $"Failed to log search completion: {ex.Message}"
            );
    }
    
    private async Task<MlResult<UserProfile>> TryGetFromCacheAsync(string cacheKey)
    {
        try
        {
            var cachedData = await _cache.GetAsync<UserProfile>(cacheKey);
            
            return cachedData != null 
                ? MlResult<UserProfile>.Valid(cachedData)
                : MlResult<UserProfile>.Fail("Data not found in cache");
        }
        catch (Exception ex)
        {
            return MlResult<UserProfile>.Fail($"Cache error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<UserProfile>> LoadFromRepositoryAsync(int userId)
    {
        try
        {
            var profile = await _repository.GetUserProfileAsync(userId);
            
            return profile != null 
                ? MlResult<UserProfile>.Valid(profile)
                : MlResult<UserProfile>.Fail($"User profile not found for ID {userId}");
        }
        catch (Exception ex)
        {
            return MlResult<UserProfile>.Fail($"Repository error: {ex.Message}");
        }
    }
    
    private async Task UpdateCacheAsync(string cacheKey, UserProfile profile)
    {
        try
        {
            await _cache.SetAsync(cacheKey, profile, TimeSpan.FromMinutes(30));
            await _logger.LogDebugAsync($"Updated cache key: {cacheKey}");
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to update cache: {ex.Message}");
        }
    }
    
    private MlResult<SearchCriteria> ValidateSearchCriteria(SearchCriteria criteria)
    {
        if (criteria == null)
            return MlResult<SearchCriteria>.Fail("Search criteria cannot be null");
            
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(criteria.Query) && criteria.Filters?.Any() != true)
            errors.Add("Either query or filters must be provided");
            
        if (!string.IsNullOrWhiteSpace(criteria.Query) && criteria.Query.Length < 3)
            errors.Add("Query must be at least 3 characters long");
            
        if (criteria.PageSize <= 0 || criteria.PageSize > 100)
            errors.Add("Page size must be between 1 and 100");
            
        if (criteria.PageNumber < 1)
            errors.Add("Page number must be at least 1");
            
        if (errors.Any())
            return MlResult<SearchCriteria>.Fail(errors.ToArray());
            
        return MlResult<SearchCriteria>.Valid(criteria);
    }
    
    private async Task<MlResult<SearchResults<T>>> TryGetSearchFromCacheAsync<T>(string cacheKey) where T : class
    {
        try
        {
            var cachedResults = await _cache.GetAsync<SearchResults<T>>(cacheKey);
            
            return cachedResults != null 
                ? MlResult<SearchResults<T>>.Valid(cachedResults)
                : MlResult<SearchResults<T>>.Fail("Search results not found in cache");
        }
        catch (Exception ex)
        {
            return MlResult<SearchResults<T>>.Fail($"Cache error: {ex.Message}");
        }
    }
    
    private async Task<MlResult<SearchResults<T>>> PerformSearchAsync<T>(SearchCriteria criteria) where T : class
    {
        try
        {
            var results = await _repository.SearchAsync<T>(criteria);
            return MlResult<SearchResults<T>>.Valid(results);
        }
        catch (Exception ex)
        {
            return MlResult<SearchResults<T>>.Fail($"Search error: {ex.Message}");
        }
    }
    
    private async Task CacheSearchResultsAsync<T>(string cacheKey, SearchResults<T> results) where T : class
    {
        try
        {
            // Cachear por menos tiempo si hay pocos resultados (pueden cambiar más frecuentemente)
            var cacheTime = results.TotalResults < 10 
                ? TimeSpan.FromMinutes(5) 
                : TimeSpan.FromMinutes(15);
                
            await _cache.SetAsync(cacheKey, results, cacheTime);
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to cache search results: {ex.Message}");
        }
    }
    
    private async Task RecordSearchSuccessAsync(string requestId, SearchCriteria criteria, int resultCount)
    {
        try
        {
            var searchMetrics = new SearchMetrics
            {
                RequestId = requestId,
                Query = criteria.Query,
                FilterCount = criteria.Filters?.Count ?? 0,
                ResultCount = resultCount,
                PageSize = criteria.PageSize,
                PageNumber = criteria.PageNumber,
                Timestamp = DateTime.UtcNow
            };
            
            await _metrics.RecordSearchAsync(searchMetrics);
        }
        catch (Exception ex)
        {
            await _logger.LogWarningAsync($"Failed to record search metrics: {ex.Message}");
        }
    }
    
    private string GenerateSearchCacheKey(SearchCriteria criteria)
    {
        var keyComponents = new List<string>
        {
            "search",
            criteria.Query ?? "no_query",
            $"page_{criteria.PageNumber}",
            $"size_{criteria.PageSize}"
        };
        
        if (criteria.Filters?.Any() == true)
        {
            var filterKey = string.Join("_", criteria.Filters.Select(f => $"{f.Key}:{f.Value}"));
            keyComponents.Add($"filters_{filterKey.GetHashCode()}");
        }
        
        return string.Join(":", keyComponents);
    }
}

// Clases de apoyo
public class UserProfile
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime LastLoginAt { get; set; }
    public UserPreferences Preferences { get; set; }
}

public class UserPreferences
{
    public string Language { get; set; }
    public string TimeZone { get; set; }
    public bool EmailNotifications { get; set; }
}

public class SearchCriteria
{
    public string Query { get; set; }
    public Dictionary<string, string> Filters { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class SearchResults<T> where T : class
{
    public List<T> Items { get; set; }
    public int TotalResults { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class SearchMetrics
{
    public string RequestId { get; set; }
    public string Query { get; set; }
    public int FilterCount { get; set; }
    public int ResultCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public DateTime Timestamp { get; set; }
}

// Interfaces de servicios
public interface IDataRepository
{
    Task<UserProfile> GetUserProfileAsync(int userId);
    Task<SearchResults<T>> SearchAsync<T>(SearchCriteria criteria) where T : class;
}

public interface ICacheService
{
    Task<T> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
}

public interface IMetricsService
{
    Task IncrementCounterAsync(string counterName);
    Task RecordTimingAsync(string timerName, TimeSpan duration);
    Task RecordSearchAsync(SearchMetrics metrics);
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar BindSaveValueInDetailsIfFaildFuncResult

```csharp
// ✅ Correcto: Usar para preservar contexto en validaciones complejas
var result = userInput
    .BindSaveValueInDetailsIfFaildFuncResult(input => ValidateComplexBusiness(input))
    .BindSaveValueInDetailsIfFaildFuncResult(validated => ProcessWithExternalService(validated));

// En caso de error, tendremos tanto el error como los valores de entrada que causaron el problema

// ✅ Correcto: Útil para debugging de pipelines largos
var result = data
    .BindSaveValueInDetailsIfFaildFuncResult(step1 => ProcessStep1(step1))
    .BindSaveValueInDetailsIfFaildFuncResult(step2 => ProcessStep2(step2))
    .BindSaveValueInDetailsIfFaildFuncResult(step3 => ProcessStep3(step3));

// Si ProcessStep3 falla, tendremos el contexto de todos los pasos anteriores

// ❌ Incorrecto: No usar para operaciones simples donde el contexto es obvio
var result = userId.BindSaveValueInDetailsIfFaildFuncResult(id => GetUser(id));
// El userId ya es obvio en el error, no necesitamos preservarlo
```

### 2. Cuándo Usar ExecSelf vs ExecSelfIfValid

```csharp
// ✅ Correcto: Usar ExecSelf para logging que debe ocurrir siempre
var result = ProcessOrder(order)
    .ExecSelf(
        success => _logger.LogInfo($"Order {success.Id} processed"),
        failure => _logger.LogError($"Order processing failed: {failure.FirstErrorMessage}")
    );

// ✅ Correcto: Usar ExecSelfIfValid para efectos secundarios que solo tienen sentido en éxito
var result = CreateUser(userData)
    .ExecSelfIfValid(user => _cache.Set($"user:{user.Id}", user))
    .ExecSelfIfValid(user => _metrics.Increment("users.created"));

// ❌ Incorrecto: Usar ExecSelfIfValid para logging de errores
var result = ProcessData(data)
    .ExecSelfIfValid(success => _logger.LogInfo("Success"))  // No loggeará errores
    .ExecSelfIfValid(success => _logger.LogError("Error")); // Nunca se ejecutará
```

### 3. Manejo de Excepciones en Efectos Secundarios

```csharp
// ✅ Correcto: Usar TryExecSelf para efectos secundarios que pueden fallar
var result = ProcessPayment(payment)
    .TryExecSelf(
        success => SendConfirmationEmail(success), // Puede fallar
        failure => SendFailureNotification(failure), // Puede fallar
        ex => $"Notification failed: {ex.Message}"
    );

// ✅ Correcto: Efectos secundarios que no deben afectar el resultado principal
var result = SaveData(data)
    .TryExecSelfIfValid(
        savedData => UpdateSearchIndex(savedData), // Puede fallar, pero no debe afectar el guardado
        ex => $"Search index update failed: {ex.Message}"
    );

// ❌ Incorrecto: No capturar excepciones en operaciones críticas
var result = ProcessOrder(order)
    .ExecSelfIfValid(success => CriticalAuditLog(success)); // Si falla, se pierde la excepción
```

### 4. Preservación de Contexto en Pipelines de Validación

```csharp
// ✅ Correcto: Pipeline que preserva contexto en cada paso
public MlResult<ValidatedData> ValidateWithContext(InputData input)
{
    return ValidateBasicRules(input)
        .BindSaveValueInDetailsIfFaildFuncResult(basic => ValidateBusinessRules(basic))
        .BindSaveValueInDetailsIfFaildFuncResult(business => ValidateExternalConstraints(business))
        .BindSaveValueInDetailsIfFaildFuncResult(external => FinalizeValidation(external));
}

// En caso de error en cualquier paso, tendremos el contexto completo de entrada

// ✅ Correcto: Logging estructurado con contexto
public async Task<MlResult<ProcessedResult>> ProcessWithFullLogging(ProcessingInput input)
{
    var correlationId = Guid.NewGuid().ToString();
    
    return await ValidateInput(input)
        .ExecSelfIfValidAsync(async valid => await LogProcessingStart(correlationId, valid))
        .BindSaveValueInDetailsIfFaildFuncResultAsync(async valid => await ProcessMainLogic(valid))
        .ExecSelfAsync(
            async success => await LogProcessingSuccess(correlationId, success),
            async failure => await LogProcessingFailure(correlationId, failure)
        );
}
```

### 5. Gestión de Recursos y Cleanup

```csharp
// ✅ Correcto: Cleanup que siempre debe ejecutarse
public async Task<MlResult<ProcessResult>> ProcessWithCleanup(ProcessInput input)
{
    var resources = new ResourceManager();
    
    return await ValidateInput(input)
        .BindAsync(async valid => await ProcessWithResources(valid, resources))
        .ExecSelfAsync(
            async success => await CleanupResources(resources, success: true),
            async failure => await CleanupResources(resources, success: false)
        );
}

// ✅ Correcto: Efectos secundarios condicionados al éxito
public async Task<MlResult<SaveResult>> SaveWithBackup(SaveData data)
{
    return await ValidateData(data)
        .BindAsync(async valid => await SaveToDatabase(valid))
        .ExecSelfIfValidAsync(async saved => await CreateBackup(saved))
        .ExecSelfIfValidAsync(async saved => await NotifyBackupCreated(saved));
}

// El backup y notificación solo ocurren si el guardado fue exitoso
```

---

## Consideraciones de Rendimiento

### Preservación de Contexto

- `BindSaveValueInDetailsIfFaildFuncResult` tiene overhead adicional al guardar valores en detalles
- Solo usar cuando el contexto sea realmente necesario para debugging o auditoría
- Considerar el tamaño de los objetos que se preservan

### Efectos Secundarios

- Los métodos `ExecSelf` y `ExecSelfIfValid` no afectan el flujo principal
- Las versiones `Try*` tienen overhead de manejo de excepciones
- Efectos secundarios asíncronos pueden impactar la latencia total

### Operaciones Asíncronas

- Las versiones `Async` mantienen el contexto de sincronización apropiadamente
- Considerar usar `ConfigureAwait(false)` en contextos de librería
- Los efectos secundarios paralelos pueden implementarse fuera de estos métodos

---

## Resumen

La clase `MlResultActionsSpecializedBind` proporciona operaciones especializadas para casos específicos:

- **`BindSaveValueInDetailsIfFaildFuncResult`**: Preserva valores de entrada en detalles de error para mejor contexto de debugging
- **`ExecSelf`**: Ejecuta efectos secundarios condicionales sin modificar el resultado original
- **`ExecSelfIfValid`**: Ejecuta efectos secundarios solo en casos de éxito
- **Versiones `Try*`**: Manejo seguro de excepciones para todas las operaciones

Estas operaciones son especialmente útiles para:
- **Debugging y diagnóstico** con preservación de contexto
- **Logging y auditoría** sin afectar el flujo principal
- **Efectos secundarios** como actualización de cache, métricas y notificaciones
- **Cleanup de recursos** condicional según el resultado

La clave está en usar estas operaciones para funcionalidades auxiliares que no deben afectar la lógica principal del pipeline, pero que proporcionan valor añadido en