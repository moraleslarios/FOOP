# MlResultActionsBind - Operaciones de Binding para Manejo de Fallos

## Índice
1. [Introducción](#introducción)
2. [Análisis de la Funcionalidad](#análisis-de-la-funcionalidad)
3. [Variantes de BindIfFail](#variantes-de-bindiffail)
4. [Variantes de TryBindIfFail](#variantes-de-trybindiffail)
5. [Patrones de Uso](#patrones-de-uso)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Introducción

La sección **BindIfFail** de `MlResultActionsBind` proporciona operaciones especializadas para el manejo y recuperación de errores en pipelines funcionales. Estas operaciones implementan patrones de **recuperación de errores**, **logging condicional**, y **transformación de fallos**, permitiendo crear sistemas resilientes que pueden manejar y recuperarse de estados de error de manera elegante.

### Propósito Principal

- **Recuperación de Errores**: Intentar recuperarse de estados de fallo
- **Transformación de Errores**: Convertir errores en valores válidos o diferentes errores
- **Logging Condicional**: Registrar errores sin interrumpir el flujo
- **Manejo Alternativo**: Proporcionar rutas alternativas cuando ocurren fallos

---

## Análisis de la Funcionalidad

### Filosofía de BindIfFail

```
MlResult<T> → ¿Es Fallo?
              ├─ Sí → Función de Recuperación(ErrorDetails) → MlResult<T>
              └─ No → Retorna Valor Original (sin cambios)
```

### Comportamiento Base

1. **Si el resultado fuente es exitoso**: Retorna el valor sin cambios (no ejecuta función)
2. **Si el resultado fuente es fallido**: Ejecuta la función de recuperación con los detalles del error
3. **La función puede**: Recuperar el error, transformarlo, registrarlo, o generar un nuevo error

### Tipos de Operaciones

1. **BindIfFail Simple**: Solo actúa sobre fallos, preserva éxitos
2. **BindIfFail con Transformación**: Maneja tanto éxitos como fallos, puede cambiar tipos
3. **TryBindIfFail**: Versiones seguras que capturan excepciones en las funciones de recuperación

---

## Variantes de BindIfFail

### Variante 1: BindIfFail Simple (Recuperación de Errores)

**Propósito**: Ejecuta una función de recuperación solo cuando el resultado es un fallo

```csharp
public static MlResult<T> BindIfFail<T>(this MlResult<T> source, 
                                        Func<MlErrorsDetails, MlResult<T>> func)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `func`: Función de recuperación que recibe los detalles del error y puede retornar un valor válido

**Comportamiento**:
- Si `source` es exitoso: Retorna el valor sin cambios
- Si `source` es fallido: Ejecuta `func(errorDetails)` para intentar recuperación

### Variante 2: BindIfFail con Transformación Completa

**Propósito**: Proporciona funciones separadas para manejar tanto éxitos como fallos, permitiendo cambio de tipo

```csharp
public static MlResult<TReturn> BindIfFail<T, TReturn>(this MlResult<T> source,
                                                       Func<T, MlResult<TReturn>> funcValid,
                                                       Func<MlErrorsDetails, MlResult<TReturn>> funcFail)
```

**Parámetros**:
- `source`: El resultado a evaluar
- `funcValid`: Función para manejar valores exitosos
- `funcFail`: Función para manejar errores

**Comportamiento**:
- Si `source` es exitoso: Ejecuta `funcValid(value)`
- Si `source` es fallido: Ejecuta `funcFail(errorDetails)`

### Soporte Asíncrono Completo

Ambas variantes incluyen soporte asíncrono completo:
- **Funciones asíncronas**: `Func<T, Task<MlResult<T>>>`
- **Fuente asíncrona**: `Task<MlResult<T>>`
- **Todas las combinaciones**: Función síncrona con fuente asíncrona, etc.

---

## Variantes de TryBindIfFail

### TryBindIfFail Simple

**Propósito**: Versión segura que captura excepciones en la función de recuperación

```csharp
public static MlResult<T> TryBindIfFail<T>(this MlResult<T> source, 
                                           Func<MlErrorsDetails, MlResult<T>> func,
                                           Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Ejecuta la función de recuperación de forma segura
- Si la función de recuperación lanza excepción, captura y crea error contextual
- Útil cuando la recuperación puede fallar

### TryBindIfFail con Transformación

**Propósito**: Versión segura que protege tanto la función de éxito como la de fallo

```csharp
public static MlResult<TReturn> TryBindIfFail<T, TReturn>(this MlResult<T> source,
                                                          Func<T, MlResult<TReturn>> funcValid,
                                                          Func<MlErrorsDetails, MlResult<TReturn>> funcFail,
                                                          Func<Exception, string> errorMessageBuilder)
```

**Comportamiento**:
- Protege ambas funciones contra excepciones
- Proporciona contexto específico para errores en recuperación vs. errores en procesamiento normal

### Sobrecargas con Mensajes Simples

Todas las variantes `Try*` incluyen sobrecargas que aceptan strings simples:

```csharp
public static MlResult<T> TryBindIfFail<T>(this MlResult<T> source, 
                                           Func<MlErrorsDetails, MlResult<T>> func,
                                           string errorMessage = null!)
```

---

## Patrones de Uso

### Patrón 1: Recuperación de Errores

```csharp
// Intentar recuperarse de un fallo con un valor por defecto
var result = riskyOperation
    .BindIfFail(errors => MlResult<string>.Valid("default_value"));
```

### Patrón 2: Logging de Errores

```csharp
// Registrar errores sin interrumpir el flujo
var result = await operation
    .BindIfFailAsync(async errors => 
    {
        await LogErrorAsync(errors);
        return MlResult<Data>.Fail(errors); // Propagar el error después de loggear
    });
```

### Patrón 3: Fuente de Datos Alternativa

```csharp
// Intentar fuente de datos alternativa en caso de fallo
var result = await GetDataFromPrimarySource()
    .BindIfFailAsync(async errors => await GetDataFromBackupSource());
```

### Patrón 4: Transformación de Errores

```csharp
// Transformar errores técnicos en mensajes de usuario
var result = technicalOperation
    .BindIfFail(errors => MlResult<UserMessage>.Valid(
        new UserMessage("Operation temporarily unavailable")));
```

### Patrón 5: Reintentos Controlados

```csharp
// Implementar lógica de reintento
var result = await operation
    .BindIfFailAsync(async errors => await RetryOperation(errors));
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Recuperación de Datos con Fuentes Alternativas

```csharp
public class DataRetrievalService
{
    private readonly IPrimaryDataSource _primarySource;
    private readonly ISecondaryDataSource _secondarySource;
    private readonly ICacheService _cache;
    private readonly ILogger<DataRetrievalService> _logger;
    
    public async Task<MlResult<UserData>> GetUserDataAsync(int userId)
    {
        return await GetUserFromPrimarySourceAsync(userId)
            .BindIfFailAsync(async primaryErrors => 
            {
                await LogPrimarySourceFailureAsync(userId, primaryErrors);
                return await GetUserFromSecondarySourceAsync(userId);
            })
            .BindIfFailAsync(async secondaryErrors =>
            {
                await LogSecondarySourceFailureAsync(userId, secondaryErrors);
                return await GetUserFromCacheAsync(userId);
            })
            .BindIfFailAsync(async cacheErrors =>
            {
                await LogAllSourcesFailureAsync(userId, cacheErrors);
                return CreateDefaultUserData(userId);
            });
    }
    
    public async Task<MlResult<UserData>> GetUserDataSafelyAsync(int userId)
    {
        return await GetUserFromPrimarySourceAsync(userId)
            .TryBindIfFailAsync(
                funcAsync: async errors => await GetUserFromSecondarySourceWithRetryAsync(userId),
                errorMessageBuilder: ex => $"Secondary source failed for user {userId}: {ex.Message}"
            )
            .TryBindIfFailAsync(
                funcAsync: async errors => await GetUserFromCacheWithValidationAsync(userId),
                errorMessage: "Cache retrieval failed"
            );
    }
    
    private async Task<MlResult<UserData>> GetUserFromPrimarySourceAsync(int userId)
    {
        await Task.Delay(100); // Simular latencia de base de datos primaria
        
        // Simular fallos ocasionales en fuente primaria
        if (userId % 10 == 0)
            return MlResult<UserData>.Fail($"Primary database connection failed for user {userId}");
        
        if (userId % 7 == 0)
            return MlResult<UserData>.Fail($"User {userId} not found in primary database");
        
        var userData = new UserData
        {
            Id = userId,
            Name = $"User_{userId}",
            Email = $"user{userId}@primary.com",
            Source = "Primary",
            LastUpdated = DateTime.UtcNow,
            IsComplete = true
        };
        
        return MlResult<UserData>.Valid(userData);
    }
    
    private async Task<MlResult<UserData>> GetUserFromSecondarySourceAsync(int userId)
    {
        await Task.Delay(200); // Fuente secundaria más lenta
        
        // Simular algunos fallos en fuente secundaria también
        if (userId % 15 == 0)
            return MlResult<UserData>.Fail($"Secondary source unavailable for user {userId}");
        
        var userData = new UserData
        {
            Id = userId,
            Name = $"User_{userId}_Secondary",
            Email = $"user{userId}@secondary.com",
            Source = "Secondary",
            LastUpdated = DateTime.UtcNow.AddHours(-1),
            IsComplete = false // Datos menos completos
        };
        
        return MlResult<UserData>.Valid(userData);
    }
    
    private async Task<MlResult<UserData>> GetUserFromCacheAsync(int userId)
    {
        await Task.Delay(50); // Cache es más rápido
        
        // Cache puede estar vacío para algunos usuarios
        if (userId % 20 == 0)
            return MlResult<UserData>.Fail($"User {userId} not found in cache");
        
        var userData = new UserData
        {
            Id = userId,
            Name = $"User_{userId}_Cached",
            Email = $"user{userId}@cached.com",
            Source = "Cache",
            LastUpdated = DateTime.UtcNow.AddDays(-1),
            IsComplete = false,
            IsStale = true
        };
        
        return MlResult<UserData>.Valid(userData);
    }
    
    private MlResult<UserData> CreateDefaultUserData(int userId)
    {
        var userData = new UserData
        {
            Id = userId,
            Name = "Unknown User",
            Email = "unknown@default.com",
            Source = "Default",
            LastUpdated = DateTime.UtcNow,
            IsComplete = false,
            IsDefault = true
        };
        
        return MlResult<UserData>.Valid(userData);
    }
    
    private async Task<MlResult<UserData>> GetUserFromSecondarySourceWithRetryAsync(int userId)
    {
        const int maxRetries = 3;
        var currentAttempt = 0;
        
        while (currentAttempt < maxRetries)
        {
            try
            {
                await Task.Delay(100 * (currentAttempt + 1)); // Backoff exponencial
                
                // Simular operación que puede fallar
                if (currentAttempt < 2 && userId % 5 == 0)
                    throw new TimeoutException($"Timeout on attempt {currentAttempt + 1}");
                
                var userData = new UserData
                {
                    Id = userId,
                    Name = $"User_{userId}_SecondaryRetry",
                    Email = $"user{userId}@secondary-retry.com",
                    Source = "SecondaryWithRetry",
                    LastUpdated = DateTime.UtcNow,
                    IsComplete = true,
                    RetryAttempts = currentAttempt + 1
                };
                
                return MlResult<UserData>.Valid(userData);
            }
            catch (Exception ex)
            {
                currentAttempt++;
                if (currentAttempt >= maxRetries)
                    throw new ApplicationException($"Failed after {maxRetries} attempts: {ex.Message}");
            }
        }
        
        return MlResult<UserData>.Fail($"All retry attempts failed for user {userId}");
    }
    
    private async Task<MlResult<UserData>> GetUserFromCacheWithValidationAsync(int userId)
    {
        await Task.Delay(30);
        
        // Simular validación que puede fallar
        if (userId % 25 == 0)
            throw new InvalidDataException($"Cached data for user {userId} is corrupted");
        
        var userData = new UserData
        {
            Id = userId,
            Name = $"User_{userId}_ValidatedCache",
            Email = $"user{userId}@validated-cache.com",
            Source = "ValidatedCache",
            LastUpdated = DateTime.UtcNow.AddHours(-2),
            IsComplete = false,
            IsValidated = true
        };
        
        return MlResult<UserData>.Valid(userData);
    }
    
    // Métodos de logging
    private async Task LogPrimarySourceFailureAsync(int userId, MlErrorsDetails errors)
    {
        await Task.Delay(10);
        _logger.LogWarning("Primary source failed for user {UserId}: {Errors}", userId, errors.GetMessage());
    }
    
    private async Task LogSecondarySourceFailureAsync(int userId, MlErrorsDetails errors)
    {
        await Task.Delay(10);
        _logger.LogWarning("Secondary source failed for user {UserId}: {Errors}", userId, errors.GetMessage());
    }
    
    private async Task LogAllSourcesFailureAsync(int userId, MlErrorsDetails errors)
    {
        await Task.Delay(10);
        _logger.LogError("All sources failed for user {UserId}: {Errors}", userId, errors.GetMessage());
    }
}

// Clases de apoyo
public class UserData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Source { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsComplete { get; set; }
    public bool IsStale { get; set; }
    public bool IsDefault { get; set; }
    public bool IsValidated { get; set; }
    public int RetryAttempts { get; set; }
}

public interface IPrimaryDataSource { }
public interface ISecondaryDataSource { }
public interface ICacheService { }

public class InvalidDataException : Exception
{
    public InvalidDataException(string message) : base(message) { }
}
```

### Ejemplo 2: Sistema de Procesamiento de Pagos con Recuperación

```csharp
public class PaymentProcessingService
{
    public async Task<MlResult<PaymentResult>> ProcessPaymentAsync(PaymentRequest request)
    {
        return await ProcessPrimaryPaymentAsync(request)
            .BindIfFailAsync(async primaryErrors =>
            {
                await LogPaymentFailureAsync(request.Id, "Primary", primaryErrors);
                return await ProcessBackupPaymentAsync(request);
            })
            .BindIfFailAsync(async backupErrors =>
            {
                await LogPaymentFailureAsync(request.Id, "Backup", backupErrors);
                return await ProcessManualPaymentAsync(request);
            })
            .BindIfFailAsync(async manualErrors =>
            {
                await LogCriticalPaymentFailureAsync(request.Id, manualErrors);
                return CreatePaymentPendingResult(request);
            });
    }
    
    public async Task<MlResult<PaymentResult>> ProcessPaymentSafelyAsync(PaymentRequest request)
    {
        return await ProcessPrimaryPaymentAsync(request)
            .TryBindIfFailAsync(
                funcAsync: async errors => await ProcessBackupPaymentWithValidationAsync(request),
                errorMessageBuilder: ex => $"Backup payment processing failed for request {request.Id}: {ex.Message}"
            )
            .TryBindIfFailAsync(
                funcAsync: async errors => await ProcessEmergencyPaymentAsync(request),
                errorMessage: "Emergency payment processing failed"
            );
    }
    
    private async Task<MlResult<PaymentResult>> ProcessPrimaryPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(200); // Simular procesamiento de pago
        
        // Simular fallos ocasionales en procesador primario
        if (request.Amount > 10000)
            return MlResult<PaymentResult>.Fail($"Amount {request.Amount:C} exceeds primary processor limit");
        
        if (request.Id % 8 == 0)
            return MlResult<PaymentResult>.Fail($"Primary payment processor temporarily unavailable");
        
        if (request.CardNumber.EndsWith("0000"))
            return MlResult<PaymentResult>.Fail("Invalid card number detected");
        
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Completed,
            Processor = "Primary",
            TransactionId = $"PRI_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = request.Amount * 0.025m // 2.5% fee
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    private async Task<MlResult<PaymentResult>> ProcessBackupPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(300); // Procesador de respaldo más lento
        
        // Diferentes condiciones de fallo para el procesador de respaldo
        if (request.Amount < 10)
            return MlResult<PaymentResult>.Fail($"Amount {request.Amount:C} below backup processor minimum");
        
        if (request.Id % 12 == 0)
            return MlResult<PaymentResult>.Fail("Backup payment processor declined transaction");
        
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Completed,
            Processor = "Backup",
            TransactionId = $"BCK_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = request.Amount * 0.035m // 3.5% fee (más caro)
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    private async Task<MlResult<PaymentResult>> ProcessManualPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(500); // Procesamiento manual es más lento
        
        // El procesamiento manual rara vez falla, pero puede pasar
        if (request.Amount > 50000)
            return MlResult<PaymentResult>.Fail($"Amount {request.Amount:C} requires additional authorization");
        
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.PendingManualReview,
            Processor = "Manual",
            TransactionId = $"MAN_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = 25.00m, // Fee fijo para procesamiento manual
            RequiresManualReview = true
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    private MlResult<PaymentResult> CreatePaymentPendingResult(PaymentRequest request)
    {
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Pending,
            Processor = "System",
            TransactionId = $"PND_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = 0m,
            RequiresManualReview = true,
            AdditionalNotes = "All automated processors failed. Manual intervention required."
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    private async Task<MlResult<PaymentResult>> ProcessBackupPaymentWithValidationAsync(PaymentRequest request)
    {
        await Task.Delay(250);
        
        // Validación adicional que puede fallar
        if (request.CardNumber.Length != 16)
            throw new ArgumentException($"Invalid card number length for request {request.Id}");
        
        if (request.ExpiryDate < DateTime.Now)
            throw new InvalidOperationException($"Card expired for request {request.Id}");
        
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Completed,
            Processor = "BackupValidated",
            TransactionId = $"BVL_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = request.Amount * 0.030m, // 3.0% fee
            AdditionalValidation = true
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    private async Task<MlResult<PaymentResult>> ProcessEmergencyPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(400);
        
        // Procesador de emergencia que puede fallar de diferentes maneras
        if (DateTime.Now.Hour < 9 || DateTime.Now.Hour > 17)
            throw new InvalidOperationException("Emergency processor only available during business hours");
        
        if (request.Amount > 25000)
            throw new ArgumentOutOfRangeException(nameof(request.Amount), "Emergency processor amount limit exceeded");
        
        var result = new PaymentResult
        {
            PaymentId = Guid.NewGuid(),
            RequestId = request.Id,
            Amount = request.Amount,
            Status = PaymentStatus.Completed,
            Processor = "Emergency",
            TransactionId = $"EMG_{DateTime.UtcNow:yyyyMMddHHmmss}_{request.Id}",
            ProcessedAt = DateTime.UtcNow,
            Fee = 50.00m + (request.Amount * 0.040m), // Fee base + 4.0%
            IsEmergencyProcessing = true
        };
        
        return MlResult<PaymentResult>.Valid(result);
    }
    
    // Métodos de logging
    private async Task LogPaymentFailureAsync(int requestId, string processor, MlErrorsDetails errors)
    {
        await Task.Delay(10);
        // Log payment failure
        Console.WriteLine($"Payment failure in {processor} processor for request {requestId}: {errors.GetMessage()}");
    }
    
    private async Task LogCriticalPaymentFailureAsync(int requestId, MlErrorsDetails errors)
    {
        await Task.Delay(10);
        // Log critical failure
        Console.WriteLine($"CRITICAL: All payment processors failed for request {requestId}: {errors.GetMessage()}");
    }
}

// Clases de apoyo para pagos
public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    PendingManualReview
}

public class PaymentRequest
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string CardNumber { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Currency { get; set; } = "USD";
    public string MerchantId { get; set; }
}

public class PaymentResult
{
    public Guid PaymentId { get; set; }
    public int RequestId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string Processor { get; set; }
    public string TransactionId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public decimal Fee { get; set; }
    public bool RequiresManualReview { get; set; }
    public bool AdditionalValidation { get; set; }
    public bool IsEmergencyProcessing { get; set; }
    public string AdditionalNotes { get; set; }
}
```

### Ejemplo 3: Sistema de Transformación de Errores y Mensajes de Usuario

```csharp
public class ErrorTransformationService
{
    public async Task<MlResult<UserFriendlyResult>> ProcessUserRequestAsync(UserRequest request)
    {
        return await ProcessInternalRequestAsync(request)
            .BindIfFail<InternalResult, UserFriendlyResult>(
                funcValid: internalResult => TransformToUserFriendly(internalResult),
                funcFail: errors => TransformErrorsToUserFriendly(errors, request)
            );
    }
    
    public async Task<MlResult<UserFriendlyResult>> ProcessUserRequestSafelyAsync(UserRequest request)
    {
        return await ProcessInternalRequestAsync(request)
            .TryBindIfFailAsync<InternalResult, UserFriendlyResult>(
                funcValidAsync: async internalResult => await TransformToUserFriendlyAsync(internalResult),
                funcFailAsync: async errors => await TransformErrorsToUserFriendlyAsync(errors, request),
                errorMessageBuilder: ex => $"Error transformation failed for request {request.Id}: {ex.Message}"
            );
    }
    
    private async Task<MlResult<InternalResult>> ProcessInternalRequestAsync(UserRequest request)
    {
        await Task.Delay(150);
        
        // Simular diferentes tipos de errores internos
        switch (request.Id % 10)
        {
            case 0:
                return MlResult<InternalResult>.Fail("Database connection timeout");
            case 1:
                return MlResult<InternalResult>.Fail("Invalid SQL query: syntax error near 'SELECT'");
            case 2:
                return MlResult<InternalResult>.Fail("Service unavailable: HTTP 503");
            case 3:
                return MlResult<InternalResult>.Fail("Validation failed: UserRequest.Email is required");
            case 4:
                return MlResult<InternalResult>.Fail("Authorization failed: insufficient permissions");
            case 5:
                return MlResult<InternalResult>.Fail("Rate limit exceeded: 100 requests per minute");
            case 6:
                return MlResult<InternalResult>.Fail("External API timeout: payment service");
            case 7:
                return MlResult<InternalResult>.Fail("Resource not found: User with ID 12345");
            default:
                var result = new InternalResult
                {
                    RequestId = request.Id,
                    InternalData = $"InternalData_{request.Id}",
                    ProcessedAt = DateTime.UtcNow,
                    ServiceVersion = "v2.1.5",
                    ProcessingTimeMs = 150
                };
                return MlResult<InternalResult>.Valid(result);
        }
    }
    
    private MlResult<UserFriendlyResult> TransformToUserFriendly(InternalResult internalResult)
    {
        var userResult = new UserFriendlyResult
        {
            RequestId = internalResult.RequestId,
            Message = "Your request has been processed successfully!",
            Status = "Success",
            Timestamp = internalResult.ProcessedAt,
            UserData = $"Data for request {internalResult.RequestId}"
        };
        
        return MlResult<UserFriendlyResult>.Valid(userResult);
    }
    
    private MlResult<UserFriendlyResult> TransformErrorsToUserFriendly(MlErrorsDetails errors, UserRequest request)
    {
        var errorMessage = errors.GetMessage();
        var userMessage = errorMessage switch
        {
            var msg when msg.Contains("Database connection timeout") => 
                "We're experiencing temporary technical difficulties. Please try again in a few minutes.",
            
            var msg when msg.Contains("Invalid SQL query") => 
                "There's a temporary issue with our system. Our technical team has been notified.",
            
            var msg when msg.Contains("Service unavailable") => 
                "The service is temporarily unavailable. Please try again later.",
            
            var msg when msg.Contains("Validation failed") => 
                "Please check that all required fields are filled out correctly.",
            
            var msg when msg.Contains("Authorization failed") => 
                "You don't have permission to perform this action. Please contact support if you believe this is an error.",
            
            var msg when msg.Contains("Rate limit exceeded") => 
                "You're making requests too quickly. Please wait a moment and try again.",
            
            var msg when msg.Contains("External API timeout") => 
                "We're experiencing delays with payment processing. Please try again in a few minutes.",
            
            var msg when msg.Contains("Resource not found") => 
                "The requested information could not be found. Please verify your request and try again.",
            
            _ => "An unexpected error occurred. Please try again or contact support if the problem persists."
        };
        
        var userResult = new UserFriendlyResult
        {
            RequestId = request.Id,
            Message = userMessage,
            Status = "Error",
            Timestamp = DateTime.UtcNow,
            SupportReference = $"REF_{request.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}"
        };
        
        return MlResult<UserFriendlyResult>.Valid(userResult);
    }
    
    private async Task<MlResult<UserFriendlyResult>> TransformToUserFriendlyAsync(InternalResult internalResult)
    {
        await Task.Delay(50); // Simular transformación que puede requerir operaciones async
        
        var userResult = new UserFriendlyResult
        {
            RequestId = internalResult.RequestId,
            Message = "Your request has been processed successfully!",
            Status = "Success",
            Timestamp = internalResult.ProcessedAt,
            UserData = $"Enhanced data for request {internalResult.RequestId}",
            ProcessingDetails = new ProcessingDetails
            {
                ServiceVersion = internalResult.ServiceVersion,
                ProcessingTime = $"{internalResult.ProcessingTimeMs}ms",
                ServerId = Environment.MachineName
            }
        };
        
        return MlResult<UserFriendlyResult>.Valid(userResult);
    }
    
    private async Task<MlResult<UserFriendlyResult>> TransformErrorsToUserFriendlyAsync(
        MlErrorsDetails errors, 
        UserRequest request)
    {
        await Task.Delay(30); // Simular análisis de error que puede requerir lookups
        
        var errorMessage = errors.GetMessage();
        
        // Análisis más sofisticado con posibles excepciones
        if (errorMessage.Contains("Database"))
        {
            // Simular verificación de estado que puede fallar
            if (request.Id % 20 == 0)
                throw new SystemException("Error analysis service unavailable");
            
            var statusCheck = await CheckDatabaseStatusAsync();
            var userMessage = statusCheck ? 
                "We're performing maintenance on our database. Please try again in 10 minutes." :
                "We're experiencing database issues. Our team is working on a fix.";
            
            var userResult = new UserFriendlyResult
            {
                RequestId = request.Id,
                Message = userMessage,
                Status = "TemporaryError",
                Timestamp = DateTime.UtcNow,
                SupportReference = $"DB_REF_{request.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                EstimatedRetryTime = DateTime.UtcNow.AddMinutes(statusCheck ? 10 : 30)
            };
            
            return MlResult<UserFriendlyResult>.Valid(userResult);
        }
        
        // Usar transformación síncrona como fallback
        return TransformErrorsToUserFriendly(errors, request);
    }
    
    private async Task<bool> CheckDatabaseStatusAsync()
    {
        await Task.Delay(25);
        return DateTime.UtcNow.Minute % 2 == 0; // Simular estado variable
    }
}

// Clases de apoyo para transformación de errores
public class UserRequest
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string RequestType { get; set; }
    public DateTime RequestedAt { get; set; }
}

public class InternalResult
{
    public int RequestId { get; set; }
    public string InternalData { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string ServiceVersion { get; set; }
    public int ProcessingTimeMs { get; set; }
}

public class UserFriendlyResult
{
    public int RequestId { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public DateTime Timestamp { get; set; }
    public string UserData { get; set; }
    public string SupportReference { get; set; }
    public DateTime? EstimatedRetryTime { get; set; }
    public ProcessingDetails ProcessingDetails { get; set; }
}

public class ProcessingDetails
{
    public string ServiceVersion { get; set; }
    public string ProcessingTime { get; set; }
    public string ServerId { get; set; }
}
```

---

## Mejores Prácticas

### 1. Estrategias de Recuperación Efectivas

```csharp
// ✅ Correcto: Recuperación progresiva con múltiples alternativas
var result = await GetDataFromPrimarySource()
    .BindIfFailAsync(async errors => 
    {
        await LogFailure("Primary", errors);
        return await GetDataFromSecondarySource();
    })
    .BindIfFailAsync(async errors => 
    {
        await LogFailure("Secondary", errors);
        return await GetDataFromCache();
    })
    .BindIfFailAsync(errors => 
    {
        await LogCriticalFailure(errors);
        return CreateDefaultData();
    });

// ✅ Correcto: Recuperación condicional basada en tipo de error
var result = await riskyOperation
    .BindIfFailAsync(async errors =>
    {
        var errorMessage = errors.GetMessage();
        
        return errorMessage switch
        {
            var msg when msg.Contains("timeout") => await RetryWithTimeout(),
            var msg when msg.Contains("unauthorized") => await RefreshCredentialsAndRetry(),
            var msg when msg.Contains("not found") => await CreateDefaultResource(),
            _ => MlResult<Data>.Fail(errors) // Propagar otros errores
        };
    });

// ❌ Incorrecto: Recuperación genérica que puede ocultar errores importantes
var result = await operation
    .BindIfFail(errors => MlResult<Data>.Valid(defaultData)); // Muy genérico
```

### 2. Logging Efectivo sin Interrumpir el Flujo

```csharp
// ✅ Correcto: Logging que preserva el error original
var result = await operation
    .BindIfFailAsync(async errors =>
    {
        await LogErrorWithContextAsync(errors, "Operation XYZ failed");
        return MlResult<Data>.Fail(errors); // Propagar el error original
    });

// ✅ Correcto: Logging con información contextual
var result = await userOperation
    .BindIfFailAsync(async errors =>
    {
        await LogErrorAsync(new
        {
            Operation = "UserProcessing",
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Error = errors.GetMessage(),
            StackTrace = errors.GetDetails()
        });
        
        return MlResult<ProcessedUser>.Fail(errors);
    });

// ❌ Incorrecto: Logging que puede fallar y no está protegido
var result = await operation
    .BindIfFailAsync(async errors =>
    {
        await riskyLoggingOperation(errors); // Puede lanzar excepción
        return MlResult<Data>.Fail(errors);
    });

// ✅ Mejor: Logging protegido
var result = await operation
    .TryBindIfFailAsync(
        funcAsync: async errors =>
        {
            await riskyLoggingOperation(errors);
            return MlResult<Data>.Fail(errors);
        },
        errorMessage: "Failed to log error details"
    );
```

### 3. Transformación de Errores para Diferentes Audiencias

```csharp
// ✅ Correcto: Transformación específica por audiencia
public class ErrorTransformer
{
    public MlResult<UserMessage> TransformForEndUser(MlErrorsDetails errors)
    {
        var userMessage = errors.GetMessage() switch
        {
            var msg when msg.Contains("SQL") => "Temporary database issue",
            var msg when msg.Contains("HTTP 5") => "Service temporarily unavailable",
            var msg when msg.Contains("timeout") => "Request took too long",
            _ => "An unexpected error occurred"
        };
        
        return MlResult<UserMessage>.Valid(new UserMessage(userMessage));
    }
    
    public MlResult<TechMessage> TransformForTechnicalUser(MlErrorsDetails errors)
    {
        var techMessage = new TechMessage
        {
            OriginalError = errors.GetMessage(),
            ErrorCode = ExtractErrorCode(errors),
            Timestamp = DateTime.UtcNow,
            StackTrace = errors.GetDetails(),
            TroubleshootingSteps = GenerateTroubleshootingSteps(errors)
        };
        
        return MlResult<TechMessage>.Valid(techMessage);
    }
}

// ✅ Correcto: Uso de transformadores específicos
var userResult = await operation
    .BindIfFail<InternalData, UserMessage>(
        funcValid: data => ProcessForUser(data),
        funcFail: errors => errorTransformer.TransformForEndUser(errors)
    );

var techResult = await operation
    .BindIfFail<InternalData, TechMessage>(
        funcValid: data => ProcessForTech(data),
        funcFail: errors => errorTransformer.TransformForTechnicalUser(errors)
    );
```

### 4. Manejo de Recursos en Recuperación

```csharp
// ✅ Correcto: Liberación de recursos en recuperación
var result = await AcquireResourceAndProcess()
    .BindIfFailAsync(async errors =>
    {
        await CleanupResourcesAsync();
        await LogResourceFailureAsync(errors);
        return await TryAlternativeResourceAsync();
    });

// ✅ Correcto: Recuperación con using statements
public async Task<MlResult<ProcessedData>> ProcessWithResourceAsync()
{
    using var resource = await AcquireResourceAsync();
    
    return await ProcessWithResourceAsync(resource)
        .BindIfFailAsync(async errors =>
        {
            // Resource se libera automáticamente por using
            await LogProcessingFailureAsync(errors);
            return await ProcessWithBackupResourceAsync();
        });
}

// ❌ Incorrecto: No considerar liberación de recursos en paths de error
var result = await ExpensiveResourceOperation()
    .BindIfFail(errors => QuickFallback()); // Puede dejar recursos sin liberar
```

### 5. Testing de Recuperación de Errores

```csharp
// ✅ Correcto: Testing completo de scenarios de recuperación
[Test]
public async Task BindIfFail_PrimaryFails_UsesSecondarySource()
{
    // Arrange
    var primarySource = CreateFailingPrimarySource();
    var secondarySource = CreateWorkingSecondarySource();
    
    // Act
    var result = await primarySource.GetDataAsync()
        .BindIfFailAsync(async errors => await secondarySource.GetDataAsync());
    
    // Assert
    result.Should().BeSuccessful();
    result.Value.Source.Should().Be("Secondary");
}

[Test]
public async Task BindIfFail_BothSourcesFail_ReturnsLastError()
{
    // Arrange
    var primarySource = CreateFailingPrimarySource();
    var secondarySource = CreateFailingSecondarySource();
    
    // Act
    var result = await primarySource.GetDataAsync()
        .BindIfFailAsync(async errors => await secondarySource.GetDataAsync());
    
    // Assert
    result.Should().BeFailure();
    result.ErrorsDetails.GetMessage().Should().Contain("Secondary");
}

[Test]
public async Task TryBindIfFail_RecoveryThrows_ReturnsFailureWithContext()
{
    // Arrange
    var source = CreateFailingSource();
    
    // Act
    var result = await source.GetDataAsync()
        .TryBindIfFailAsync(
            funcAsync: async errors => throw new Exception("Recovery failed"),
            errorMessage: "Recovery operation failed"
        );
    
    // Assert
    result.Should().BeFailure();
    result.ErrorsDetails.GetMessage().Should().Contain("Recovery operation failed");
}

[Test]
public async Task BindIfFail_SuccessfulSource_DoesNotExecuteRecovery()
{
    // Arrange
    var source = CreateWorkingSource();
    var recoveryExecuted = false;
    
    // Act
    var result = await source.GetDataAsync()
        .BindIfFailAsync(async errors =>
        {
            recoveryExecuted = true;
            return await CreateFallbackDataAsync();
        });
    
    // Assert
    result.Should().BeSuccessful();
    recoveryExecuted.Should().BeFalse();
}
```

---

## Consideraciones de Rendimiento

### Ejecución Condicional

- **Solo en fallos**: Las funciones de recuperación solo se ejecutan cuando hay fallos
- **Cortocircuito en éxito**: Los valores exitosos pasan sin procesamiento adicional
- **Costo de recuperación**: Considerar el costo de las operaciones de recuperación

### Estrategias de Optimización

```csharp
// ✅ Optimizado: Recuperación rápida primero
var result = await operation
    .BindIfFail(errors => GetFromFastCache()) // Rápido
    .BindIfFail(errors => GetFromSlowCache()) // Medio
    .BindIfFail(errors => GetFromDatabase()); // Lento

// ✅ Optimizado: Evitar recuperación costosa para errores irrelevantes
var result = await operation
    .BindIfFailAsync(async errors =>
    {
        var errorMessage = errors.GetMessage();
        
        // Solo recuperación costosa para errores específicos
        if (errorMessage.Contains("temporary") || errorMessage.Contains("timeout"))
            return await ExpensiveRecoveryOperation();
        
        return MlResult<Data>.Fail(errors); // Propagar otros errores
    });

// ⚠️ Considerar: Recuperación que puede ser más costosa que el fallo
var result = await quickOperation
    .BindIfFail(errors => veryExpensiveRecovery()); // Evaluar trade-off
```

### Patrones de Caching en Recuperación

```csharp
// ✅ Patrón efectivo: Cache de datos de recuperación
public class CachedRecoveryService
{
    private readonly IMemoryCache _cache;
    
    public async Task<MlResult<Data>> GetDataWithRecoveryAsync(string key)
    {
        return await GetDataFromPrimaryAsync(key)
            .BindIfFailAsync(async errors => await GetFromCacheWithFallbackAsync(key));
    }
    
    private async Task<MlResult<Data>> GetFromCacheWithFallbackAsync(string key)
    {
        // Intentar cache primero (rápido)
        if (_cache.TryGetValue(key, out Data cachedData))
            return MlResult<Data>.Valid(cachedData);
        
        // Fallback a fuente lenta
        return await GetDataFromSlowSourceAsync(key);
    }
}
```

---

## Resumen

Las operaciones **BindIfFail** proporcionan capacidades robustas para el manejo y recuperación de errores:

### Variantes Principales

1. **BindIfFail Simple**: Recuperación cuando hay fallos, preserva éxitos
2. **BindIfFail con Transformación**: Manejo completo de ambos casos (éxito/fallo)
3. **TryBindIfFail**: Versiones seguras que protegen las funciones de recuperación

### Características Clave

- **Ejecución Condicional**: Solo actúa sobre fallos (en variante simple)
- **Preservación de Éxitos**: Los valores exitosos pasan sin modificación
- **Soporte Asíncrono Completo**: Todas las combinaciones de operaciones síncronas/asíncronas
- **Manejo Seguro**: Versiones `Try*` para operaciones de recuperación que pueden fallar

### Casos de Uso Ideales

- **Recuperación de Errores**: Fuentes de datos alternativas, valores por defecto
- **Logging de Errores**: Registro sin interrumpir el flujo
- **Transformación de Errores**: Conversión de errores técnicos a mensajes de usuario
- **Reintentos Controlados**: Lógica de reintento con backoff
- **Limpieza de Recursos**: Liberación de recursos en caso de fallo

Las operaciones BindIfFail permiten crear sistemas resilientes que pueden manejar fallos de manera elegante, proporcionando mecanismos de recuperación automática mientras mantienen la claridad y composabilidad