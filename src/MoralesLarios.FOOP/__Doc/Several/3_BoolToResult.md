# MlResult BoolToResult - Conversión de Condiciones Booleanas a Resultados

## Índice
1. [Introducción](#introducción)
2. [Análisis de los Métodos](#análisis-de-los-métodos)
3. [Métodos BoolToResult para Objetos](#métodos-booltoResult-para-objetos)
4. [Métodos BoolToResult para Bool](#métodos-booltoResult-para-bool)
5. [Variantes Asíncronas](#variantes-asíncronas)
6. [Ejemplos Prácticos](#ejemplos-prácticos)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Comparación con Otros Métodos de Validación](#comparación-con-otros-métodos-de-validación)

---

## Introducción

Los métodos `BoolToResult` proporcionan una forma de **convertir condiciones booleanas en resultados `MlResult<T>`**, permitiendo validar condiciones arbitrarias y transformar valores `false` en errores explícitos. Estos métodos son fundamentales para implementar validaciones personalizadas basadas en lógica de negocio.

### Propósito Principal

- **Validación Condicional**: Convertir condiciones booleanas en resultados manejables
- **Lógica de Negocio**: Implementar reglas de validación personalizadas
- **Control de Flujo**: Permitir o bloquear el procesamiento basado en condiciones
- **Validación Flexible**: Crear validaciones dinámicas según contexto

---

## Análisis de los Métodos

### Filosofía de BoolToResult

```
(T, bool) → BoolToResult(condition, error) → MlResult<T>
    ↓              ↓                            ↓
(value, true) → MlResult<T>.Valid(value)
    ↓              ↓                            ↓
(value, false) → MlResult<T>.Fail(error)
```

### Características Principales

1. **Validación Basada en Condiciones**: Usa expresiones booleanas para determinar validez
2. **Preservación de Valor**: Si la condición es verdadera, mantiene el valor original
3. **Flexibilidad de Errores**: Acepta diferentes tipos de mensaje de error
4. **Dos Variantes**: Para objetos con condición externa y para bool directamente
5. **Soporte Asíncrono**: Variantes para operaciones asíncronas

---

## Métodos BoolToResult para Objetos

### `BoolToResult<T>()` - Con Condición Externa

**Propósito**: Validar un objeto basado en una condición booleana externa

```csharp
public static MlResult<T> BoolToResult<T>(this T source,
                                          bool condition,
                                          MlErrorsDetails errorsDetails)
```

**Comportamiento**:
- Si `condition` es `true`: retorna `MlResult<T>.Valid(source)`
- Si `condition` es `false`: retorna `MlResult<T>.Fail(errorsDetails)`

**Ejemplo Básico**:
```csharp
var user = GetUser(userId);
var result = user.BoolToResult(
    condition: user != null && user.IsActive,
    errorMessage: $"User {userId} is not found or inactive"
);

// Si user existe y está activo: MlResult válido con el usuario
// Si user no existe o está inactivo: MlResult fallido con el error
```

### Variantes con Diferentes Tipos de Error

```csharp
// Con MlError
public static MlResult<T> BoolToResult<T>(this T source,
                                          bool condition,
                                          MlError error)

// Con string
public static MlResult<T> BoolToResult<T>(this T source,
                                          bool condition,
                                          string errorMessage)

// Con IEnumerable<string>
public static MlResult<T> BoolToResult<T>(this T source,
                                          bool condition,
                                          IEnumerable<string> errorsMessage)
```

**Ejemplo con Múltiples Errores**:
```csharp
var order = GetOrder(orderId);
var result = order.BoolToResult(
    condition: order != null && order.Status == "Pending" && order.Amount > 0,
    errorsMessage: new[] {
        $"Order {orderId} validation failed",
        "Order must exist, be pending, and have positive amount",
        "Please verify order details and try again"
    }
);
```

---

## Métodos BoolToResult para Bool

### `BoolToResult()` - Para Valores Bool Directos

**Propósito**: Convertir un valor booleano directamente en `MlResult<bool>`

```csharp
public static MlResult<bool> BoolToResult(this bool source,
                                          MlErrorsDetails errorsDetails)
```

**Comportamiento**:
- Si `source` es `true`: retorna `MlResult<bool>.Valid(true)`
- Si `source` es `false`: retorna `MlResult<bool>.Fail(errorsDetails)`

**Ejemplo Básico**:
```csharp
bool isValidPassword = ValidatePassword(password);
var result = isValidPassword.BoolToResult("Password does not meet security requirements");

// Si password es válido: MlResult<bool>.Valid(true)
// Si password es inválido: MlResult<bool>.Fail("Password does not meet...")
```

### Variantes para Bool

```csharp
// Con MlError
public static MlResult<bool> BoolToResult(this bool source, MlError error)

// Con string
public static MlResult<bool> BoolToResult(this bool source, string errorMessage)

// Con IEnumerable<string>
public static MlResult<bool> BoolToResult(this bool source, IEnumerable<string> errorsMessage)
```

---

## Variantes Asíncronas

### Para Objetos con Condición

```csharp
// Valor síncrono
public static async Task<MlResult<T>> BoolToResultAsync<T>(this T source,
                                                           bool condition,
                                                           MlError error)

// Valor asíncrono
public static async Task<MlResult<T>> BoolToResultAsync<T>(this Task<T> sourceAsync,
                                                           bool condition,
                                                           string errorMessage)
```

### Para Bool Directo

```csharp
// Bool síncrono
public static Task<MlResult<bool>> BoolToResultAsync(this bool source,
                                                     string errorMessage)

// Bool asíncrono
public static async Task<MlResult<bool>> BoolToResultAsync(this Task<bool> sourceAsync,
                                                           MlErrorsDetails errorsDetails)
```

---

## Ejemplos Prácticos

### Ejemplo 1: Sistema de Validación de Reglas de Negocio

```csharp
public class BusinessRuleValidator
{
    private readonly IUserRepository _userRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IPaymentService _paymentService;
    
    public async Task<MlResult<Order>> ValidateOrderCreationAsync(OrderRequest request)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId);
        
        return user
            .BoolToResult(
                condition: user != null && user.IsActive && !user.IsSuspended,
                errorsMessage: new[] {
                    $"User {request.UserId} cannot create orders",
                    "User must be active and not suspended",
                    "Contact support if you believe this is an error"
                })
            .Bind(validUser => ValidateOrderLimits(validUser, request))
            .Bind(limitValidUser => ValidateInventoryAvailability(request))
            .BindAsync(async _ => await CreateOrderAsync(request));
    }
    
    public MlResult<User> ValidateUserForPremiumFeatures(User user)
    {
        return user.BoolToResult(
            condition: user.SubscriptionType == "Premium" && 
                      user.SubscriptionExpiryDate > DateTime.UtcNow &&
                      user.PaymentStatus == "Current",
            errorMessage: "Premium features require active premium subscription"
        );
    }
    
    public async Task<MlResult<PaymentResult>> ProcessPaymentAsync(PaymentRequest request)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId);
        
        return user
            .BoolToResult(
                condition: user != null && user.IsVerified,
                errorMessage: "Payment processing requires verified user account")
            .Bind(verifiedUser => verifiedUser.BoolToResult(
                condition: verifiedUser.CreditLimit >= request.Amount,
                errorMessage: $"Payment amount {request.Amount:C} exceeds credit limit {verifiedUser.CreditLimit:C}"))
            .BindAsync(async validUser => await _paymentService.ProcessPaymentAsync(request));
    }
    
    public MlResult<Document> ValidateDocumentAccess(Document document, User user)
    {
        return document.BoolToResult(
            condition: document.OwnerId == user.Id || 
                      document.SharedWith.Contains(user.Id) ||
                      user.Role == "Admin",
            errorsMessage: new[] {
                "Access denied to document",
                "You can only access documents you own, documents shared with you, or if you're an admin",
                $"Document ID: {document.Id}, Your ID: {user.Id}"
            });
    }
    
    public async Task<MlResult<bool>> ValidateBusinessHoursOperationAsync(string operation)
    {
        var currentTime = DateTime.Now;
        var isBusinessHours = currentTime.Hour >= 9 && currentTime.Hour < 17 && 
                             currentTime.DayOfWeek != DayOfWeek.Saturday && 
                             currentTime.DayOfWeek != DayOfWeek.Sunday;
        
        return await isBusinessHours.BoolToResultAsync(
            errorsMessage: new[] {
                $"Operation '{operation}' can only be performed during business hours",
                "Business hours: Monday-Friday, 9:00 AM - 5:00 PM",
                $"Current time: {currentTime:yyyy-MM-dd HH:mm}",
                "Please try again during business hours"
            });
    }
    
    private MlResult<User> ValidateOrderLimits(User user, OrderRequest request)
    {
        var dailyOrderCount = _orderRepo.GetDailyOrderCount(user.Id);
        var maxDailyOrders = user.SubscriptionType == "Premium" ? 50 : 10;
        
        return user.BoolToResult(
            condition: dailyOrderCount < maxDailyOrders,
            errorMessage: $"Daily order limit reached ({dailyOrderCount}/{maxDailyOrders}). " +
                         "Upgrade to Premium for higher limits."
        );
    }
    
    private MlResult<OrderRequest> ValidateInventoryAvailability(OrderRequest request)
    {
        var allItemsAvailable = request.Items.All(item => 
            _orderRepo.GetAvailableStock(item.ProductId) >= item.Quantity);
        
        return request.BoolToResult(
            condition: allItemsAvailable,
            errorMessage: "One or more items are not available in requested quantities"
        );
    }
}

public class OrderRequest
{
    public int UserId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class User
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsVerified { get; set; }
    public string SubscriptionType { get; set; }
    public DateTime SubscriptionExpiryDate { get; set; }
    public string PaymentStatus { get; set; }
    public decimal CreditLimit { get; set; }
    public string Role { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
}

public class Document
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public List<int> SharedWith { get; set; }
}

public class PaymentRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethodId { get; set; }
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
}
```

### Ejemplo 2: Sistema de Validación de Configuraciones

```csharp
public class ConfigurationValidator
{
    private readonly IConfigurationRepository _configRepo;
    private readonly IEnvironmentService _envService;
    
    public async Task<MlResult<DatabaseConfig>> ValidateDatabaseConfigAsync(DatabaseConfig config)
    {
        return config
            .BoolToResult(
                condition: !string.IsNullOrEmpty(config.ConnectionString),
                errorMessage: "Database connection string is required")
            .Bind(validConfig => validConfig.BoolToResult(
                condition: config.ConnectionTimeout > 0 && config.ConnectionTimeout <= 300,
                errorMessage: "Connection timeout must be between 1 and 300 seconds"))
            .Bind(validConfig => validConfig.BoolToResult(
                condition: config.MaxPoolSize > 0 && config.MaxPoolSize <= 1000,
                errorMessage: "Max pool size must be between 1 and 1000"))
            .BindAsync(async validConfig => await TestDatabaseConnectionAsync(validConfig));
    }
    
    public MlResult<ApiConfig> ValidateApiConfiguration(ApiConfig config)
    {
        return config
            .BoolToResult(
                condition: Uri.IsWellFormedUriString(config.BaseUrl, UriKind.Absolute),
                errorMessage: $"Invalid API base URL: {config.BaseUrl}")
            .Bind(validConfig => validConfig.BoolToResult(
                condition: config.TimeoutSeconds > 0 && config.TimeoutSeconds <= 300,
                errorMessage: "API timeout must be between 1 and 300 seconds"))
            .Bind(validConfig => ValidateApiCredentials(validConfig));
    }
    
    public async Task<MlResult<SecurityConfig>> ValidateSecurityConfigAsync(SecurityConfig config)
    {
        var isProductionEnvironment = await _envService.IsProductionAsync();
        
        return config
            .BoolToResult(
                condition: config.JwtExpirationMinutes > 0,
                errorMessage: "JWT expiration must be positive")
            .Bind(validConfig => validConfig.BoolToResult(
                condition: !isProductionEnvironment || config.RequireHttps,
                errorsMessage: new[] {
                    "HTTPS is required in production environment",
                    "Security configuration validation failed",
                    "Set RequireHttps to true for production deployment"
                }))
            .Bind(validConfig => validConfig.BoolToResult(
                condition: !isProductionEnvironment || !config.AllowInsecureConnections,
                errorMessage: "Insecure connections are not allowed in production"));
    }
    
    public MlResult<EmailConfig> ValidateEmailConfiguration(EmailConfig config)
    {
        return config
            .BoolToResult(
                condition: !string.IsNullOrEmpty(config.SmtpServer) && config.Port > 0,
                errorMessage: "Valid SMTP server and port are required")
            .Bind(validConfig => validConfig.BoolToResult(
                condition: IsValidEmail(config.FromAddress),
                errorMessage: $"Invalid from email address: {config.FromAddress}"))
            .Bind(validConfig => validConfig.BoolToResult(
                condition: config.UseAuthentication ? 
                          !string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password) :
                          true,
                errorsMessage: new[] {
                    "SMTP authentication is enabled but credentials are missing",
                    "Provide both username and password for authenticated SMTP",
                    "Or disable authentication if not required"
                }));
    }
    
    public async Task<MlResult<FeatureFlags>> ValidateFeatureFlagsAsync(FeatureFlags flags)
    {
        var environment = await _envService.GetEnvironmentAsync();
        
        return flags
            .BoolToResult(
                condition: !(environment == "Production" && flags.DebugMode),
                errorMessage: "Debug mode cannot be enabled in production environment")
            .Bind(validFlags => validFlags.BoolToResult(
                condition: !(flags.EnableBetaFeatures && environment == "Production"),
                errorMessage: "Beta features cannot be enabled in production environment"))
            .Bind(validFlags => validFlags.BoolToResult(
                condition: flags.CacheExpirationMinutes > 0,
                errorMessage: "Cache expiration must be positive"));
    }
    
    private async Task<MlResult<DatabaseConfig>> TestDatabaseConnectionAsync(DatabaseConfig config)
    {
        try
        {
            var canConnect = await _configRepo.TestConnectionAsync(config.ConnectionString);
            return canConnect.BoolToResult(
                errorsMessage: new[] {
                    "Database connection test failed",
                    "Please verify connection string and database availability",
                    $"Connection string: {MaskConnectionString(config.ConnectionString)}"
                });
        }
        catch (Exception ex)
        {
            return MlResult<DatabaseConfig>.Fail($"Database connection error: {ex.Message}");
        }
    }
    
    private MlResult<ApiConfig> ValidateApiCredentials(ApiConfig config)
    {
        return config.BoolToResult(
            condition: !string.IsNullOrEmpty(config.ApiKey) || 
                      (!string.IsNullOrEmpty(config.ClientId) && !string.IsNullOrEmpty(config.ClientSecret)),
            errorsMessage: new[] {
                "API credentials are required",
                "Provide either ApiKey or both ClientId and ClientSecret",
                "Check your API provider documentation for correct credential format"
            });
    }
    
    private bool IsValidEmail(string email)
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
    
    private string MaskConnectionString(string connectionString)
    {
        // Implementación para enmascarar información sensible
        return connectionString?.Length > 10 ? 
            connectionString.Substring(0, 10) + "..." : 
            connectionString;
    }
}

public class DatabaseConfig
{
    public string ConnectionString { get; set; }
    public int ConnectionTimeout { get; set; }
    public int MaxPoolSize { get; set; }
}

public class ApiConfig
{
    public string BaseUrl { get; set; }
    public int TimeoutSeconds { get; set; }
    public string ApiKey { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

public class SecurityConfig
{
    public int JwtExpirationMinutes { get; set; }
    public bool RequireHttps { get; set; }
    public bool AllowInsecureConnections { get; set; }
}

public class EmailConfig
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string FromAddress { get; set; }
    public bool UseAuthentication { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class FeatureFlags
{
    public bool DebugMode { get; set; }
    public bool EnableBetaFeatures { get; set; }
    public int CacheExpirationMinutes { get; set; }
}
```

### Ejemplo 3: Sistema de Validación de Permisos y Seguridad

```csharp
public class SecurityValidator
{
    private readonly IUserService _userService;
    private readonly IPermissionService _permissionService;
    private readonly IAuditService _auditService;
    
    public async Task<MlResult<User>> ValidateUserAccessAsync(int userId, string resource, string action)
    {
        var user = await _userService.GetByIdAsync(userId);
        
        return user
            .BoolToResult(
                condition: user != null && user.IsActive,
                errorMessage: $"User {userId} not found or inactive")
            .Bind(validUser => validUser.BoolToResult(
                condition: !validUser.IsLocked,
                errorsMessage: new[] {
                    $"User account {userId} is locked",
                    "Account was locked due to security violations",
                    "Contact administrator to unlock account"
                }))
            .BindAsync(async activeUser => await ValidateUserPermissionsAsync(activeUser, resource, action))
            .ExecSelfIfValidAsync(async validUser => 
                await _auditService.LogAccessGrantedAsync(validUser.Id, resource, action))
            .ExecSelfIfFailAsync(async errors => 
                await _auditService.LogAccessDeniedAsync(userId, resource, action, errors));
    }
    
    public async Task<MlResult<bool>> ValidatePasswordStrengthAsync(string password, User user)
    {
        var hasMinLength = password?.Length >= 8;
        var hasUpperCase = password?.Any(char.IsUpper) ?? false;
        var hasLowerCase = password?.Any(char.IsLower) ?? false;
        var hasDigit = password?.Any(char.IsDigit) ?? false;
        var hasSpecialChar = password?.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c)) ?? false;
        var notContainsUsername = !password?.ToLower().Contains(user.Username.ToLower()) ?? false;
        
        var isStrong = hasMinLength && hasUpperCase && hasLowerCase && 
                      hasDigit && hasSpecialChar && notContainsUsername;
        
        return await isStrong.BoolToResultAsync(new[] {
            "Password does not meet security requirements",
            "Password must be at least 8 characters long",
            "Password must contain uppercase and lowercase letters",
            "Password must contain at least one digit and one special character",
            "Password cannot contain your username"
        });
    }
    
    public MlResult<Session> ValidateSessionAsync(string sessionToken, string ipAddress)
    {
        var session = _userService.GetSessionByToken(sessionToken);
        
        return session
            .BoolToResult(
                condition: session != null && session.IsValid,
                errorMessage: "Invalid or expired session")
            .Bind(validSession => validSession.BoolToResult(
                condition: validSession.ExpiresAt > DateTime.UtcNow,
                errorMessage: "Session has expired"))
            .Bind(activeSession => activeSession.BoolToResult(
                condition: activeSession.IpAddress == ipAddress || !activeSession.RequireIpValidation,
                errorsMessage: new[] {
                    "Session IP address mismatch",
                    $"Session created from: {activeSession.IpAddress}",
                    $"Current request from: {ipAddress}",
                    "Session invalidated for security reasons"
                }));
    }
    
    public async Task<MlResult<bool>> ValidateRateLimitAsync(int userId, string operation)
    {
        var rateLimitInfo = await _permissionService.GetRateLimitAsync(userId, operation);
        var currentCount = await _permissionService.GetCurrentUsageAsync(userId, operation);
        
        var withinLimit = currentCount < rateLimitInfo.MaxRequests;
        
        return await withinLimit.BoolToResultAsync(new[] {
            $"Rate limit exceeded for operation '{operation}'",
            $"Limit: {rateLimitInfo.MaxRequests} requests per {rateLimitInfo.WindowMinutes} minutes",
            $"Current usage: {currentCount}/{rateLimitInfo.MaxRequests}",
            $"Try again in {rateLimitInfo.ResetTimeMinutes} minutes"
        });
    }
    
    public MlResult<FileUpload> ValidateFileUploadAsync(FileUpload upload, User user)
    {
        var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".docx", ".xlsx" };
        var maxSizeBytes = user.SubscriptionType == "Premium" ? 100_000_000 : 10_000_000; // 100MB vs 10MB
        
        return upload
            .BoolToResult(
                condition: upload.Size <= maxSizeBytes,
                errorMessage: $"File size {upload.Size:N0} bytes exceeds limit of {maxSizeBytes:N0} bytes")
            .Bind(validUpload => validUpload.BoolToResult(
                condition: allowedExtensions.Contains(Path.GetExtension(validUpload.FileName).ToLower()),
                errorsMessage: new[] {
                    $"File type '{Path.GetExtension(upload.FileName)}' not allowed",
                    $"Allowed types: {string.Join(", ", allowedExtensions)}",
                    "Please convert your file to a supported format"
                }))
            .Bind(validUpload => validUpload.BoolToResult(
                condition: !IsExecutableFile(validUpload.FileName),
                errorMessage: "Executable files are not allowed for security reasons"));
    }
    
    private async Task<MlResult<User>> ValidateUserPermissionsAsync(User user, string resource, string action)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(user.Id, resource, action);
        
        return user.BoolToResult(
            condition: hasPermission,
            errorsMessage: new[] {
                $"User {user.Id} lacks permission for action '{action}' on resource '{resource}'",
                "Contact administrator if you believe you should have access",
                $"Current user role: {user.Role}"
            });
    }
    
    private bool IsExecutableFile(string fileName)
    {
        var executableExtensions = new[] { ".exe", ".bat", ".cmd", ".com", ".pif", ".scr", ".vbs", ".js" };
        return executableExtensions.Contains(Path.GetExtension(fileName).ToLower());
    }
}

public class Session
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string IpAddress { get; set; }
    public bool RequireIpValidation { get; set; }
    public bool IsValid { get; set; }
}

public class RateLimitInfo
{
    public int MaxRequests { get; set; }
    public int WindowMinutes { get; set; }
    public int ResetTimeMinutes { get; set; }
}

public class FileUpload
{
    public string FileName { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }
}
```

---

## Mejores Prácticas

### 1. Cuándo Usar BoolToResult

```csharp
// ✅ Correcto: Validaciones basadas en lógica de negocio
var user = GetUser(userId)
    .BoolToResult(user != null && user.IsActive, "User not found or inactive");

// ✅ Correcto: Validaciones complejas con múltiples condiciones
var order = GetOrder(orderId)
    .BoolToResult(
        condition: order.Status == "Pending" && order.Amount > 0 && order.UserId == currentUserId,
        errorMessage: "Order cannot be modified");

// ✅ Correcto: Convertir validaciones boolean existentes
bool isValidPassword = ValidatePassword(password);
var result = isValidPassword.BoolToResult("Password validation failed");

// ❌ Incorrecto: Para validaciones simples que tienen métodos específicos
var user = GetUser(userId)
    .BoolToResult(user != null, "User not found"); // Mejor usar NullToFailed
```

### 2. Condiciones Claras y Específicas

```csharp
// ✅ Correcto: Condiciones bien definidas
var result = document.BoolToResult(
    condition: document.OwnerId == currentUserId || document.IsPublic,
    errorMessage: "You don't have permission to access this document");

// ✅ Correcto: Lógica de negocio específica
var orderResult = order.BoolToResult(
    condition: order.Status == "Pending" && 
              order.CreatedAt > DateTime.UtcNow.AddHours(-24) &&
              order.Items.All(i => i.IsAvailable),
    errorsMessage: new[] {
        "Order cannot be processed",
        "Order must be pending, created within 24 hours, and all items available"
    });

// ❌ Incorrecto: Condiciones vagas o complejas
var result = data.BoolToResult(
    condition: ComplexValidationMethod(data), // Mejor extraer la lógica
    errorMessage: "Validation failed");
```

### 3. Mensajes de Error Informativos

```csharp
// ✅ Correcto: Mensajes específicos que explican el problema
var user = GetUser(userId)
    .BoolToResult(
        condition: user.Age >= 18,
        errorsMessage: new[] {
            "User must be at least 18 years old",
            $"Current age: {user.Age}",
            "Age verification is required for this operation"
        });

// ✅ Correcto: Incluir contexto útil para debugging
var payment = GetPayment(paymentId)
    .BoolToResult(
        condition: payment.Amount <= user.CreditLimit,
        errorMessage: $"Payment amount {payment.Amount:C} exceeds credit limit {user.CreditLimit:C}");

// ❌ Incorrecto: Mensajes genéricos
var result = data.BoolToResult(
    condition: data.IsValid,
    errorMessage: "Invalid"); // Muy poco informativo
```

### 4. Uso en Pipelines de Validación

```csharp
// ✅ Correcto: Múltiples validaciones encadenadas
var result = GetUser(userId)
    .NullToFailed("User not found")                    // Validar existencia
    .Bind(user => user.BoolToResult(                   // Validar estado
        condition: user.IsActive && !user.IsLocked,
        errorMessage: "User account is not accessible"))
    .Bind(validUser => validUser.BoolToResult(         // Validar permisos
        condition: validUser.Role == "Admin" || validUser.Id == currentUserId,
        errorMessage: "Insufficient permissions"))
    .Map(authorizedUser => LoadUserData(authorizedUser)); // Procesar

// ✅ Correcto: Validación condicional basada en contexto
var documentResult = GetDocument(docId)
    .NullToFailed("Document not found")
    .Bind(doc => doc.BoolToResult(
        condition: !doc.IsConfidential || currentUser.HasRole("Manager"),
        errorMessage: "Access to confidential documents requires manager role"));
```

---

## Comparación con Otros Métodos de Validación

### Tabla Comparativa

| Método | Entrada | Condición de Validación | Uso Principal |
|--------|---------|------------------------|---------------|
| `BoolToResult` | `T` + `bool` | Condición booleana personalizada | Validaciones de lógica de negocio |
| `NullToFailed` | `T` | `value == null` | Validar no-null |
| `EmptyToFailed` | `IEnumerable<T>` | Colección vacía | Validar no-vacío |
| `Bind` | `MlResult<T>` | Función retorna Fail | Encadenar validaciones |

### Ejemplo Comparativo

```csharp
var user = GetUser(userId);

// BoolToResult: Validación con condición personalizada
var businessRuleResult = user.BoolToResult(
    condition: user != null && user.IsActive && user.Age >= 18,
    errorMessage: "User doesn't meet business requirements");

// NullToFailed: Validación de null específica
var nullCheckResult = user.NullToFailed("User not found");

// Combinación típica en pipeline
var processedUser = GetUser(userId)
    .NullToFailed("User not found")                    // Validar existencia
    .Bind(u => u.BoolToResult(                        // Validar reglas de negocio
        condition: u.IsActive && u.IsVerified,
        errorMessage: "User must be active and verified"))
    .Bind(validUser => LoadUserPermissions(validUser)) // Cargar datos adicionales
    .Map(enrichedUser => CreateUserDto(enrichedUser)); // Transformar resultado
```

---

## Resumen

Los métodos `BoolToResult` proporcionan **validación basada en condiciones booleanas**:

- **`BoolToResult<T>`**: Valida objetos usando condiciones booleanas externas
- **`BoolToResult`**: Convierte valores bool directamente en `MlResult<bool>`
- **`BoolToResultAsync`**: Soporte completo para operaciones asíncronas

**Casos de uso ideales**:
- **Validaciones de reglas de negocio** complejas
- **Control de acceso y permisos** basado en múltiples condiciones
- **Validación de configuraciones** con lógica específica
- **Conversión de validaciones booleanas** existentes a `MlResult`

**Ventajas principales**:
- **Flexibilidad total** en condiciones de validación
- **Integración perfecta** con lógica de negocio existente
- **Mensajes de error contextuales** y específicos
- **Reutilización** de validaciones booleanas existentes